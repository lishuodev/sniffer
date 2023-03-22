using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketDotNet;
using SharpPcap;
using WinSniffer.ProtocolAnalyzer;

namespace WinSniffer
{
    // 主窗体
    public partial class MainForm : Form
    {
        private ICaptureDevice device;
        private Thread listviewThread; // 列表管理线程
        private Thread analyzerThread; // 分析线程
        private PacketArrivalEventHandler arrivalEventHandler; // 包到达事件处理句柄
        private CaptureStoppedEventHandler captureStoppedEventHandler; // 停止事件处理句柄
        private object queueLock = new object(); // 队列锁
        private object parsedLock = new object(); // lock for parsedPacketDict
        private List<RawCapture> packetQueue = new List<RawCapture>(); // 包队列
        private bool analyzeThreadStop = false;
        private bool listviewThreadStop = false;
        private int deviceId;
        
        private int id;
        private PosixTimeval startTime;

        private Dictionary<int, LibraryParsedPacket> parsedPacketDict;
        private List<LibraryParsedPacket> parsedPacketList;
        private LibraryParsedPacket curPacket;
        private const int threadDelay = 250;

        private Dictionary<int, EthernetInfo> ethernetInfoDict;
        private Dictionary<int, IPv4Info> ipv4InfoDict;
        private Dictionary<int, IPv6Info> ipv6InfoDict;
        private Dictionary<int, ARPInfo> arpInfoDict;
        private Dictionary<int, TCPInfo> tcpInfoDict;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            buttonClear.Enabled = true;
            listViewPacket.FullRowSelect = true;

            // 更新设备下拉列表
            foreach (var dev in CaptureDeviceList.Instance)
            {
                var str = String.Format("{0} {1}", dev.Name, dev.Description);
                comboBoxDeviceList.Items.Add(str);
            }
        }

        // 更新列表显示
        private void UpdateListView()
        {
            List<LibraryParsedPacket> curList;
            lock (parsedLock)
            {
                curList = parsedPacketList;
                parsedPacketList = new List<LibraryParsedPacket>();
            }

            ListViewItem item = null;
            LibraryParsedPacket parsed = null;
            for (int i = 0; i < curList.Count; i++)
            {
                parsed = curList[i];
                item = new ListViewItem(parsed.id.ToString());
                item.SubItems.Add(parsed.time.ToString());
                switch (parsed.ethernetType)
                {
                    case EthernetType.IPv4:
                        item.SubItems.Add(parsed.ipv4.sourceIP.ToString());
                        item.SubItems.Add(parsed.ipv4.destinationIP.ToString());
                        break;
                    case EthernetType.IPv6:
                        item.SubItems.Add(parsed.ipv6.sourceAddress.ToString());
                        item.SubItems.Add(parsed.ipv6.destinationAddress.ToString());
                        break;
                    case EthernetType.Arp:
                        item.SubItems.Add(parsed.arp.senderProtocolAddress.ToString());
                        item.SubItems.Add(parsed.arp.targetProtocolAddress.ToString());
                        break;
                    default:
                        item.SubItems.Add("");
                        item.SubItems.Add("");
                        break;
                }
                item.SubItems.Add(parsed.ethernetType.ToString());
                item.SubItems.Add(parsed.packageLength.ToString());
                if (parsed.ethernetType == EthernetType.IPv4)
                {
                    item.SubItems.Add(((ProtocolType)parsed.ipv4.protocol).ToString());
                }
                listViewPacket.Items.Add(item);
            }

            if (item != null && checkBoxAutoScroll.Checked)
            {
                item.EnsureVisible();
            }
        }

        // 更新列表的线程
        private void ListViewThread()
        {
            while (!listviewThreadStop)
            {
                bool sleep = true;
                lock (parsedLock)
                {
                    if (parsedPacketList.Count != 0)
                    {
                        sleep = false;
                    }
                }

                if (sleep)
                {
                    if (analyzeThreadStop)
                    {
                        listviewThreadStop = true;
                    }
                    else
                    {
                        Thread.Sleep(threadDelay);
                    }
                }
                else
                {
                    BeginInvoke(new MethodInvoker(UpdateListView));
                }
            }
        }

        // 停止监听
        private void StopCapture()
        {
            buttonStop.Enabled = false;
            buttonStart.Enabled = true;
            buttonClear.Enabled = true;
            comboBoxDeviceList.Enabled = true;
            checkBoxPromiscuous.Enabled = true;

            if (device != null)
            {
                device.StopCapture();
                device.Close();
                device.OnPacketArrival -= arrivalEventHandler;
                device.OnCaptureStopped -= captureStoppedEventHandler;
                //device = null;
                analyzeThreadStop = true;
                analyzerThread.Join();
                listviewThread.Join();
            }
        }

        // 开始监听
        private void StartCapture()
        {
            Reset();

            deviceId = comboBoxDeviceList.SelectedIndex;
            if (deviceId < 0)
            {
                return;
            }
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            buttonClear.Enabled = false;
            comboBoxDeviceList.Enabled = false;
            checkBoxPromiscuous.Enabled = false;

            id = 0;
            startTime = new PosixTimeval(DateTime.Now);
            parsedPacketDict = new Dictionary<int, LibraryParsedPacket>();
            parsedPacketList = new List<LibraryParsedPacket>();

            ethernetInfoDict = new Dictionary<int, EthernetInfo>();
            ipv4InfoDict = new Dictionary<int, IPv4Info>();
            ipv6InfoDict = new Dictionary<int, IPv6Info>();
            arpInfoDict = new Dictionary<int, ARPInfo>();
            tcpInfoDict = new Dictionary<int, TCPInfo>();

            analyzeThreadStop = false;
            analyzerThread = new Thread(new ThreadStart(AnalyzerThread));
            analyzerThread.Start();

            listviewThreadStop = false;
            listviewThread = new Thread(new ThreadStart(ListViewThread));
            listviewThread.Start();

            device = CaptureDeviceList.Instance[deviceId];

            arrivalEventHandler = new PacketArrivalEventHandler(OnPacketArrival);
            device.OnPacketArrival += arrivalEventHandler;
            captureStoppedEventHandler = new CaptureStoppedEventHandler(OnCaptureStopped);
            device.OnCaptureStopped += captureStoppedEventHandler;

            // 设置混杂模式
            if (checkBoxPromiscuous.Checked)
            {
                device.Open(DeviceModes.Promiscuous);
            }
            else
            {
                device.Open();
            }

            // 设置过滤器
            if (textBoxFilter.Text != string.Empty)
            {
                device.Filter = textBoxFilter.Text;
            }
            
            device.StartCapture();
        }

        // 解析包的线程
        private void AnalyzerThread()
        {
            while (!analyzeThreadStop)
            {
                bool sleep = true;
                // 处理队列
                lock (queueLock)
                {
                    if (packetQueue.Count != 0)
                    {
                        sleep = false;
                    }
                }

                if (sleep)
                {
                    // 队列为空，线程休眠一会儿
                    Thread.Sleep(threadDelay);
                }
                else
                {
                    // 队列不为空，处理队列
                    List<RawCapture> curQueue;
                    lock (queueLock)
                    {
                        curQueue = packetQueue;
                        packetQueue = new List<RawCapture>();
                    }

                   
                    foreach (var packet in curQueue)
                    {
                        id++;
                        // 用 PacketDotNet 库解析数据包
                        LibraryParsedPacket libPacket = NetParser.LibraryParsePacket(id, packet);
                        libPacket.time = libPacket.timeval.Value - startTime.Value;
                        parsedPacketDict.Add(id, libPacket);
                        
                        lock (parsedLock)
                        {
                            parsedPacketList.Add(libPacket);
                        }
                        
                        // 手动解析 Ethernet Packet
                        EthernetInfo ethernetInfo = EthernetAnalyzer.Analyze(packet.Data);
                        ethernetInfoDict.Add(id, ethernetInfo);

                        // 根据 etherType 值范围判断是否 Ethernet Packet
                        if (ethernetInfo.etherType >= 0x0600 && ethernetInfo.etherType <= 0xFFFF)
                        {
                            switch (ethernetInfo.etherType)
                            {
                                case 0x0800:    // IPv4
                                    // 手动解析 IPv4 Packet
                                    byte[] ipv4Data = packet.Data.Skip(14).ToArray();
                                    IPv4Info ipv4Info = IPv4Analyzer.Analyze(ipv4Data);
                                    ipv4InfoDict.Add(id, ipv4Info);
                                    break;
                                case 0x86DD:    // IPv6
                                    // 手动解析 IPv6 Packet
                                    byte[] ipv6Data = packet.Data.Skip(14).ToArray();
                                    IPv6Info ipv6Info = IPv6Analyzer.Analyze(ipv6Data);
                                    ipv6InfoDict.Add(id, ipv6Info);
                                    break;
                                case 0x0806:    // ARP
                                    byte[] arpData = packet.Data.Skip(14).ToArray();
                                    ARPInfo arpInfo = ARPAnalyzer.Analyze(arpData);
                                    arpInfoDict.Add(id, arpInfo);
                                    break;
                                case 0x0808:    // IEEE 802.2
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (0x8000 == ethernetInfo.etherType)
                        {
                            // TBD
                        }
                    }
                }
            }
        }

        // 选中一个数据包
        private void OnSelectPacketChanged()
        {
            if (listViewPacket.SelectedIndices.Count > 0)
            {
                ListViewItem item = listViewPacket.SelectedItems[0];
                int id = int.Parse(item.SubItems[0].Text);
                curPacket = parsedPacketDict[id];

                textBoxBinary.Text = curPacket.hex;

                listBoxParse.Items.Clear();

                listBoxParse.Items.Add("--------------------------------------------------");
                listBoxParse.Items.Add("PacketDotNet");
                listBoxParse.Items.Add("--------------------------------------------------");

                listBoxParse.Items.Add("Ethernet");
                listBoxParse.Items.Add("--------------------------------------------------");
                listBoxParse.Items.Add("Source Mac:" + BitConverter.ToString(curPacket.sourceMac.GetAddressBytes()).Replace("-", ":"));
                listBoxParse.Items.Add("Destination Mac:" + BitConverter.ToString(curPacket.destinationMac.GetAddressBytes()).Replace("-", ":"));
                listBoxParse.Items.Add("ethernetType:" + curPacket.ethernetType.ToString());

                switch (curPacket.ethernetType)
                {
                    case EthernetType.IPv4:
                        IPv4Info ipv4 = curPacket.ipv4;
                        listBoxParse.Items.Add("--------------------------------------------------");
                        listBoxParse.Items.Add("IPv4");
                        listBoxParse.Items.Add("--------------------------------------------------");
                        listBoxParse.Items.Add("version:" + ipv4.version);
                        listBoxParse.Items.Add("headerLength:" + ipv4.headerLength);
                        listBoxParse.Items.Add("ToS:" + ipv4.ToS);
                        listBoxParse.Items.Add("totalLength:" + ipv4.totalLength);
                        listBoxParse.Items.Add("identification:" + ipv4.identification);
                        listBoxParse.Items.Add("flags:" + ipv4.flags);
                        listBoxParse.Items.Add("fragmentOffset:" + ipv4.fragmentOffset);
                        listBoxParse.Items.Add("TTL:" + ipv4.TTL);
                        listBoxParse.Items.Add("protocol:" + ipv4.protocol);
                        listBoxParse.Items.Add("headerChecksum:" + ipv4.headerChecksum);
                        listBoxParse.Items.Add("Source IP:" + ipv4.sourceIP.ToString());
                        listBoxParse.Items.Add("Destination IP:" + ipv4.destinationIP.ToString());
                        break;
                    case EthernetType.IPv6:
                        IPv6Info ipv6 = curPacket.ipv6;
                        listBoxParse.Items.Add("--------------------------------------------------");
                        listBoxParse.Items.Add("IPv6");
                        listBoxParse.Items.Add("--------------------------------------------------");
                        listBoxParse.Items.Add("version:" + ipv6.version);
                        listBoxParse.Items.Add("trafficClass:" + ipv6.trafficClass);
                        listBoxParse.Items.Add("flowLabel:" + ipv6.flowLabel);
                        listBoxParse.Items.Add("payloadLength:" + ipv6.payloadLength);
                        listBoxParse.Items.Add("nextHeader:" + (ProtocolType)ipv6.nextHeader);
                        listBoxParse.Items.Add("hopLimit:" + ipv6.hopLimit);
                        listBoxParse.Items.Add("sourceAddress:" + ipv6.sourceAddress.ToString());
                        listBoxParse.Items.Add("destinationAddress:" + ipv6.destinationAddress.ToString());
                        if (ipv6.payload != null)
                        {
                            listBoxParse.Items.Add("payload:" + BitConverter.ToString(ipv6.payload).Replace("-", " "));
                        }
                        switch ((ProtocolType)ipv6.nextHeader)
                        {
                            case ProtocolType.Tcp:
                                listBoxParse.Items.Add("TCP");
                                break;
                            case ProtocolType.Udp:
                                listBoxParse.Items.Add("UDP");
                                break;
                            case ProtocolType.IcmpV6:
                                listBoxParse.Items.Add("ICMPv6");
                                break;
                            default:
                                break;
                        }
                        break;
                    case EthernetType.Arp:
                        ARPInfo arp = curPacket.arp;
                        listBoxParse.Items.Add("--------------------------------------------------");
                        listBoxParse.Items.Add("ARP");
                        listBoxParse.Items.Add("--------------------------------------------------");
                        listBoxParse.Items.Add("hardwareAddressType:" + arp.hardwareAddressType);
                        listBoxParse.Items.Add("protocolAddressType:" + arp.protocolAddressType);
                        listBoxParse.Items.Add("hardwareAddressLength:" + arp.hardwareAddressLength);
                        listBoxParse.Items.Add("protocolAddressLength:" + arp.protocolAddressLength);
                        listBoxParse.Items.Add("opCode:" + arp.opCode.ToString());
                        listBoxParse.Items.Add("senderHardwareAddress:" + arp.senderHardwareAddress.ToString());
                        listBoxParse.Items.Add("senderProtocolAddress:" + arp.senderProtocolAddress.ToString());
                        listBoxParse.Items.Add("targetHardwareAddress:" + arp.targetHardwareAddress.ToString());
                        listBoxParse.Items.Add("targetProtocolAddress:" + arp.targetProtocolAddress.ToString());
                        break;
                }

                if (EthernetType.IPv4 == curPacket.ethernetType)
                {
                    switch ((ProtocolType)curPacket.ipv4.protocol)
                    {
                        case ProtocolType.Tcp:
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("TCP");
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("sourcePort:" + curPacket.tcp.sourcePort);
                            listBoxParse.Items.Add("destinationPort:" + curPacket.tcp.destinationPort);
                            listBoxParse.Items.Add("sequenceNumber:" + curPacket.tcp.sequenceNumber);
                            listBoxParse.Items.Add("acknowledgementNumber:" + curPacket.tcp.acknowledgementNumber);
                            listBoxParse.Items.Add("dataOffset:" + curPacket.tcp.dataOffset);
                            listBoxParse.Items.Add("flags:" + curPacket.tcp.flags);
                            listBoxParse.Items.Add("windowSize:" + curPacket.tcp.windowSize);
                            listBoxParse.Items.Add("checksum:" + curPacket.tcp.checksum);
                            listBoxParse.Items.Add("urgentPointer:" + curPacket.tcp.urgentPointer);
                            listBoxParse.Items.Add("options:" + BitConverter.ToString(curPacket.tcp.options).Replace("-", " "));
                            listBoxParse.Items.Add("payload:" + BitConverter.ToString(curPacket.tcp.payload).Replace("-", " "));
                            break;
                        case ProtocolType.Udp:
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("UDP");
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("sourcePort:" + curPacket.udp.sourcePort);
                            listBoxParse.Items.Add("destinationPort:" + curPacket.udp.destinationPort);
                            listBoxParse.Items.Add("length:" + curPacket.udp.length);
                            listBoxParse.Items.Add("checksum:" + curPacket.udp.checksum);
                            listBoxParse.Items.Add("payload:" + BitConverter.ToString(curPacket.udp.payload).Replace("-", " "));
                            break;
                        default:
                            break;
                    }
                }

                listBoxParse2.Items.Clear();
                listBoxParse2.Items.Add("--------------------------------------------------");
                listBoxParse2.Items.Add("Binary Analysis");
                EthernetInfo ethernetInfo = ethernetInfoDict[id];
                listBoxParse2.Items.Add("--------------------------------------------------");
                listBoxParse2.Items.Add("Ethernet");
                listBoxParse2.Items.Add("--------------------------------------------------");
                listBoxParse2.Items.Add("Source Mac:" + BitConverter.ToString(ethernetInfo.sourceMac.GetAddressBytes()).Replace("-",":"));
                listBoxParse2.Items.Add("Destination Mac:" + BitConverter.ToString(ethernetInfo.destinationMac.GetAddressBytes()).Replace("-", ":"));
                listBoxParse2.Items.Add("EtherType:" +  ethernetInfo.etherType + " " + (EthernetType)ethernetInfo.etherType);

                if (ethernetInfo.etherType >= 0x0600 && ethernetInfo.etherType <= 0xFFFF)
                {
                    switch (ethernetInfo.etherType)
                    {
                        case 0x0800:    // IPv4
                            IPv4Info ipv4Info = ipv4InfoDict[id];
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("IPv4");
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("version:" + ipv4Info.version);
                            listBoxParse2.Items.Add("headerLength:" + ipv4Info.headerLength);
                            listBoxParse2.Items.Add("ToS:" + ipv4Info.ToS);
                            listBoxParse2.Items.Add("totalLength:" + ipv4Info.totalLength);
                            listBoxParse2.Items.Add("identification:" + ipv4Info.identification);
                            listBoxParse2.Items.Add("flags:" + ipv4Info.flags);
                            listBoxParse2.Items.Add("fragmentOffset:" + ipv4Info.fragmentOffset);
                            listBoxParse2.Items.Add("TTL:" + ipv4Info.TTL);
                            listBoxParse2.Items.Add("protocol:" + ipv4Info.protocol);
                            listBoxParse2.Items.Add("headerChecksum:" + ipv4Info.headerChecksum);
                            listBoxParse2.Items.Add("Source IP:" + ipv4Info.sourceIP.ToString());
                            listBoxParse2.Items.Add("Destination IP:" + ipv4Info.destinationIP.ToString());
                            switch (ipv4Info.protocol)
                            {
                                case 6:
                                    // TCP
                                    TCPInfo tcpInfo = TCPAnalyzer.Analyze(ipv4Info.payload);
                                    listBoxParse2.Items.Add("--------------------------------------------------");
                                    listBoxParse2.Items.Add("TCP");
                                    listBoxParse2.Items.Add("--------------------------------------------------");
                                    listBoxParse2.Items.Add("sourcePort:" + tcpInfo.sourcePort);
                                    listBoxParse2.Items.Add("destinationPort:" + tcpInfo.destinationPort);
                                    listBoxParse2.Items.Add("sequenceNumber:" + tcpInfo.sequenceNumber);
                                    listBoxParse2.Items.Add("acknowledgementNumber:" + tcpInfo.acknowledgementNumber);
                                    listBoxParse2.Items.Add("dataOffset:" + tcpInfo.dataOffset);
                                    listBoxParse2.Items.Add("flags:" + tcpInfo.flags);
                                    listBoxParse2.Items.Add("windowSize:" + tcpInfo.windowSize);
                                    listBoxParse2.Items.Add("checksum:" + tcpInfo.checksum);
                                    listBoxParse2.Items.Add("urgentPointer:" + tcpInfo.urgentPointer);
                                    listBoxParse2.Items.Add("options:" + BitConverter.ToString(tcpInfo.options).Replace("-", ":"));
                                    listBoxParse2.Items.Add("payload:" + BitConverter.ToString(tcpInfo.payload).Replace("-", ":"));
                                    break;
                                case 17:
                                    // UDP
                                    UDPInfo udpInfo = UDPAnalyzer.Analyze(ipv4Info.payload);
                                    listBoxParse2.Items.Add("--------------------------------------------------");
                                    listBoxParse2.Items.Add("UDP");
                                    listBoxParse2.Items.Add("--------------------------------------------------");
                                    listBoxParse2.Items.Add("sourcePort:" + udpInfo.sourcePort);
                                    listBoxParse2.Items.Add("destinationPort:" + udpInfo.destinationPort);
                                    listBoxParse2.Items.Add("length:" + udpInfo.length);
                                    listBoxParse2.Items.Add("checksum:" + udpInfo.checksum);
                                    listBoxParse2.Items.Add("payload:" + BitConverter.ToString(udpInfo.payload).Replace("-", ":"));

                                    break;
                                default:
                                    break;
                            }

                            break;
                        case 0x86DD:    // IPv6
                            IPv6Info ipv6Info = ipv6InfoDict[id];
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("IPv6");
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("version:" + ipv6Info.version);
                            listBoxParse2.Items.Add("trafficClass:" + ipv6Info.trafficClass);
                            listBoxParse2.Items.Add("flowLabel:" + ipv6Info.flowLabel);
                            listBoxParse2.Items.Add("payloadLength:" + ipv6Info.payloadLength);
                            listBoxParse2.Items.Add("nextHeader:" + (ProtocolType)ipv6Info.nextHeader);
                            listBoxParse2.Items.Add("hopLimit:" + ipv6Info.hopLimit);
                            listBoxParse2.Items.Add("Source Address:" + ipv6Info.sourceAddress.ToString());
                            listBoxParse2.Items.Add("Destination Address:" + ipv6Info.destinationAddress.ToString());
                            if (ipv6Info.payload != null)
                            {
                                listBoxParse2.Items.Add("payload:" + BitConverter.ToString(ipv6Info.payload).Replace("-", " "));
                            }
                            switch ((ProtocolType)ipv6Info.nextHeader)
                            {
                                case ProtocolType.Tcp:
                                    TCPInfo tcpInfo = TCPAnalyzer.Analyze(ipv6Info.payload);
                                    listBoxParse2.Items.Add("--------------------------------------------------");
                                    listBoxParse2.Items.Add("TCP");
                                    listBoxParse2.Items.Add("--------------------------------------------------");
                                    listBoxParse2.Items.Add("sourcePort:" + tcpInfo.sourcePort);
                                    listBoxParse2.Items.Add("destinationPort:" + tcpInfo.destinationPort);
                                    listBoxParse2.Items.Add("sequenceNumber:" + tcpInfo.sequenceNumber);
                                    listBoxParse2.Items.Add("acknowledgementNumber:" + tcpInfo.acknowledgementNumber);
                                    listBoxParse2.Items.Add("dataOffset:" + tcpInfo.dataOffset);
                                    listBoxParse2.Items.Add("flags:" + tcpInfo.flags);
                                    listBoxParse2.Items.Add("windowSize:" + tcpInfo.windowSize);
                                    listBoxParse2.Items.Add("checksum:" + tcpInfo.checksum);
                                    listBoxParse2.Items.Add("urgentPointer:" + tcpInfo.urgentPointer);
                                    listBoxParse2.Items.Add("options:" + BitConverter.ToString(tcpInfo.options).Replace("-", ":"));
                                    listBoxParse2.Items.Add("payload:" + BitConverter.ToString(tcpInfo.payload).Replace("-", ":"));
                                    break;
                                case ProtocolType.Udp:
                                    UDPInfo udpInfo = UDPAnalyzer.Analyze(ipv6Info.payload);
                                    listBoxParse2.Items.Add("--------------------------------------------------");
                                    listBoxParse2.Items.Add("UDP");
                                    listBoxParse2.Items.Add("--------------------------------------------------");
                                    listBoxParse2.Items.Add("sourcePort:" + udpInfo.sourcePort);
                                    listBoxParse2.Items.Add("destinationPort:" + udpInfo.destinationPort);
                                    listBoxParse2.Items.Add("length:" + udpInfo.length);
                                    listBoxParse2.Items.Add("checksum:" + udpInfo.checksum);
                                    listBoxParse2.Items.Add("payload:" + BitConverter.ToString(udpInfo.payload).Replace("-", ":"));
                                    break;
                                case ProtocolType.IcmpV6:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 0x0806:    // ARP
                            ARPInfo arpInfo = arpInfoDict[id];
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("ARP");
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("hardwareAddressType:" + arpInfo.hardwareAddressType);
                            listBoxParse2.Items.Add("protocolAddressType:" + arpInfo.protocolAddressType);
                            listBoxParse2.Items.Add("hardwareAddressLength:" + arpInfo.hardwareAddressLength);
                            listBoxParse2.Items.Add("protocolAddressLength:" + arpInfo.protocolAddressLength);
                            listBoxParse2.Items.Add("opCode:" + arpInfo.opCode);
                            listBoxParse2.Items.Add("senderHardwareAddress:" + arpInfo.senderHardwareAddress);
                            listBoxParse2.Items.Add("senderProtocolAddress:" + arpInfo.senderProtocolAddress);
                            listBoxParse2.Items.Add("targetHardwareAddress:" + arpInfo.targetHardwareAddress);
                            listBoxParse2.Items.Add("targetProtocolAddress:" + arpInfo.targetProtocolAddress);
                            break;
                        case 0x0808:    // IEEE 802.2
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void OnPacketArrival(object sender, PacketCapture e)
        {
            // 收到包
            lock (queueLock)
            {
                packetQueue.Add(e.GetPacket());
            }
        }
        private void OnCaptureStopped(object sender, CaptureStoppedEventStatus status)
        {
            // 处理错误
        }

        // 停止按钮
        private void buttonStop_Click(object sender, EventArgs e)
        {
            StopCapture();
        }

        // 开始按钮
        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartCapture();
        }

        private void Reset()
        {
            if (listViewPacket.Items != null) listViewPacket.Items.Clear();

            if (parsedPacketDict != null) parsedPacketDict.Clear();
            if (parsedPacketList != null) parsedPacketList.Clear();

            if (ethernetInfoDict != null) ethernetInfoDict.Clear();
            if (ipv4InfoDict != null) ipv4InfoDict.Clear();
            if (ipv6InfoDict != null) ipv6InfoDict.Clear();
            if (arpInfoDict != null) arpInfoDict.Clear();
            if (tcpInfoDict != null) tcpInfoDict.Clear();
        }

        // 清空按钮
        private void buttonClear_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void listViewPacket_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectPacketChanged();
        }

    }
}
