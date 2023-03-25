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
        private bool running = false;
        private int deviceId;
        
        private int frame;
        private PosixTimeval startTime;

        //private Dictionary<int, ParsedPacket> parsedPacketDict;
        private Dictionary<int, ParsedPacket> parsedPacketDict2;
        private List<ParsedPacket> parsedPacketList;
        private ParsedPacket curPacket;
        private const int threadDelay = 200;

        private Dictionary<int, EthernetInfo> ethernetInfoDict;
        private Dictionary<int, IPv4Info> ipv4InfoDict;
        private Dictionary<int, IPv6Info> ipv6InfoDict;
        private Dictionary<int, ARPInfo> arpInfoDict;
        private Dictionary<int, TCPInfo> tcpInfoDict;
        private Dictionary<int, byte[]> rawDict;

        private List<CheckBox> checkBoxList;
        private ListViewItem curItem;
        private ParsedPacket tracePacket;
        private bool tracing;

        private int selectIndex;

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
            listViewTrace.FullRowSelect = true;

            checkBoxList = new List<CheckBox>
            {
                checkBoxPromiscuous, checkBoxIPv4, checkBoxIPv6, checkBoxICMP, checkBoxARP, checkBoxTCP, checkBoxUDP, checkBoxTLS, checkBoxHTTP,
            };

            // 更新设备下拉列表
            foreach (var dev in CaptureDeviceList.Instance)
            {
                var str = String.Format("{0} {1}", dev.Name, dev.Description);
                comboBoxDeviceList.Items.Add(str);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (running)
            {
                e.Cancel = true;
                var result = MessageBox.Show("监听中,确定要退出吗?", "监听中", MessageBoxButtons.OKCancel);
                switch (result)
                {
                    case DialogResult.OK:
                        ExitAfterThreadEnd();
                        break;
                    case DialogResult.Cancel:
                        break;
                }
            }
        }

        private void ExitAfterThreadEnd()
        {
            StopCapture();
            if (analyzerThread != null)
            {
                analyzerThread.Join();
            }
            if (listviewThread != null)
            {
                listviewThread.Join();
            }
            Application.Exit();
        }

        // 更新列表显示
        private void UpdateListView()
        {
            List<ParsedPacket> curList;
            lock (parsedLock)
            {
                curList = parsedPacketList;
                parsedPacketList = new List<ParsedPacket>();
            }

            ListViewItem item = AppendPacketListToListView(curList, listViewPacket, false);

            if (item != null && checkBoxAutoScroll.Checked)
            {
                item.EnsureVisible();
            }

            if (tracing)
            {
                AppendPacketListToListView(curList, listViewTrace, true);
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
                    try
                    {
                        BeginInvoke(new MethodInvoker(UpdateListView));
                    }
                    catch (InvalidOperationException e)
                    {

                    }
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
            
            foreach (var checkBox in checkBoxList)
            {
                checkBox.Enabled = true;
            }

            if (device != null)
            {
                device.StopCapture();
                device.Close();
                device.OnPacketArrival -= arrivalEventHandler;
                device.OnCaptureStopped -= captureStoppedEventHandler;
                analyzeThreadStop = true;
                analyzerThread.Abort();
                running = false;
            }
        }

        // 开始监听
        private void StartCapture()
        {
            Reset();
            running = true;

            deviceId = comboBoxDeviceList.SelectedIndex;
            if (deviceId < 0)
            {
                return;
            }
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            buttonClear.Enabled = false;
            comboBoxDeviceList.Enabled = false;
            
            foreach (var checkBox in checkBoxList)
            {
                checkBox.Enabled = false;
            }

            frame = 0;
            startTime = new PosixTimeval(DateTime.Now);
            parsedPacketList = new List<ParsedPacket>();
            parsedPacketDict2 = new Dictionary<int, ParsedPacket>();

            rawDict = new Dictionary<int, byte[]>();

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
            List<string> filters = new List<string>();
            // 网络层
            if (checkBoxIPv4.Checked) filters.Add("ip");
            if (checkBoxIPv6.Checked) filters.Add("ip6");
            if (checkBoxICMP.Checked) filters.Add("icmp or icmp6");
            if (checkBoxARP.Checked) filters.Add("arp");
            // 传输层
            if (checkBoxTCP.Checked)
            {
                if (checkBoxIPv4.Checked) filters.Add("(ip proto \\tcp)");
                else if (checkBoxIPv6.Checked) filters.Add("(ip6 proto \\tcp)");
                else filters.Add("(ip proto \\tcp) or (ip6 proto \\tcp)");
            }
            if (checkBoxUDP.Checked)
            {
                if (checkBoxIPv4.Checked) filters.Add("(ip proto \\udp)");
                else if (checkBoxIPv6.Checked) filters.Add("(ip6 proto \\udp)");
                else filters.Add("(ip proto \\udp) or (ip6 proto \\udp)");
            }

            if (checkBoxTLS.Checked) filters.Add("(tcp port 443)");

            // 应用层
            if (checkBoxHTTP.Checked) filters.Add("(tcp port 80)");

            device.Filter = string.Join(" or ", filters);

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
                    // 交换队列
                    List<RawCapture> curQueue;
                    lock (queueLock)
                    {
                        curQueue = packetQueue;
                        packetQueue = new List<RawCapture>();
                    }

                    foreach (var raw in curQueue)
                    {
                        frame++;
                        // 用 PacketDotNet 库解析数据包
                        //ParsedPacket libPacket = NetParser.LibraryParsePacket(frame, raw);
                        //libPacket.time = libPacket.timeval.Value - startTime.Value;
                        //parsedPacketDict.Add(frame, libPacket);

                        //libPacket.linkLayerType = raw.LinkLayerType;

                        ParsedPacket pp = NetParser.ParsePacket(frame, raw);
                        pp.time = raw.Timeval.Value - startTime.Value;
                        parsedPacketDict2.Add(frame, pp);
                        rawDict.Add(frame, raw.Data);

                        //// 新实现
                        //Packet packet1 = Packet.ParsePacket(raw.LinkLayerType, raw.Data);
                        //if (packet1 is EthernetPacket ethernet)
                        //{
                        //    libPacket.ethernetType = ethernet.Type;

                        //    Packet packet2 = ethernet.PayloadPacket;
                        //    if (packet2 is IPv4Packet ipv4)
                        //    {
                        //    }
                        //    else if (packet2 is IPv6Packet ipv6)
                        //    {
                        //    }
                        //    else if (packet2 is IcmpV4Packet icmpv4)
                        //    {

                        //    }
                        //    else if (packet2 is IcmpV6Packet icmpv6)
                        //    {

                        //    }
                        //}

                        
                        //// 手动解析 Ethernet Packet
                        //EthernetInfo ethernetInfo = EthernetAnalyzer.Analyze(raw.Data);
                        //ethernetInfoDict.Add(frame, ethernetInfo);

                        //// 根据 etherType 值范围判断是否 Ethernet Packet
                        //if (ethernetInfo.etherType >= 0x0600 && ethernetInfo.etherType <= 0xFFFF)
                        //{
                        //    switch (ethernetInfo.etherType)
                        //    {
                        //        case 0x0800:    // IPv4
                        //            // 手动解析 IPv4 Packet
                        //            byte[] ipv4Data = raw.Data.Skip(14).ToArray();
                        //            IPv4Info ipv4Info = IPv4Analyzer.Analyze(ipv4Data);
                        //            ipv4InfoDict.Add(frame, ipv4Info);
                        //            break;
                        //        case 0x86DD:    // IPv6
                        //            // 手动解析 IPv6 Packet
                        //            byte[] ipv6Data = raw.Data.Skip(14).ToArray();
                        //            IPv6Info ipv6Info = IPv6Analyzer.Analyze(ipv6Data);
                        //            ipv6InfoDict.Add(frame, ipv6Info);
                        //            break;
                        //        case 0x0806:    // ARP
                        //            byte[] arpData = raw.Data.Skip(14).ToArray();
                        //            ARPInfo arpInfo = ARPAnalyzer.Analyze(arpData);
                        //            arpInfoDict.Add(frame, arpInfo);
                        //            break;
                        //        case 0x0808:    // IEEE 802.2
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //}

                        //if (0x8000 == ethernetInfo.etherType)
                        //{
                        //    // TBD
                        //}

                        lock (parsedLock)
                        {
                            parsedPacketList.Add(pp);
                        }
                    }
                }
            }
        }

        // 选中一个数据包
        private void OnSelectPacketChanged(object sender)
        {
            if (sender is ListView view)
            {
                if (view.SelectedIndices.Count > 0)
                {
                    ListViewItem item = view.SelectedItems[0];
                    selectIndex = int.Parse(item.SubItems[0].Text);

                    if (radioButtonBin.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintBin(rawDict[selectIndex]);
                    else if (radioButtonHex.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintHex(rawDict[selectIndex]);

                    listBoxParse2.Items.Clear();
                    ParsedPacket pp = parsedPacketDict2[selectIndex];

                    listBoxParse2.Items.Add("--------------------");
                    listBoxParse2.Items.Add("Data Link Layer");
                    listBoxParse2.Items.Add("--------------------");
                    listBoxParse2.Items.Add("LinkLayerType:" + pp.linkLayerType);
                    listBoxParse2.Items.Add("sourceMac:" + BitConverter.ToString(pp.sourceMac.GetAddressBytes()).Replace("-", ":"));
                    listBoxParse2.Items.Add("destinationMac:" + BitConverter.ToString(pp.destinationMac.GetAddressBytes()).Replace("-", ":"));
                    listBoxParse2.Items.Add("ethernetType:" + pp.ethernetType);
                    switch (pp.ethernetType)
                    {
                        case EthernetType.IPv4:
                            if (pp.ipv4Packet != null)
                            {
                                listBoxParse2.Items.Add("--------------------");
                                listBoxParse2.Items.Add("IPv4");
                                listBoxParse2.Items.Add("--------------------");
                                listBoxParse2.Items.Add("version:" + pp.ipv4Packet.Version);
                                listBoxParse2.Items.Add("headerLength:" + pp.ipv4Packet.HeaderLength);
                                listBoxParse2.Items.Add("ToS:" + pp.ipv4Packet.TypeOfService);
                                listBoxParse2.Items.Add("totalLength:" + pp.ipv4Packet.TotalLength);
                                listBoxParse2.Items.Add("identification:" + pp.ipv4Packet.Id);
                                listBoxParse2.Items.Add("flags:" + pp.ipv4Packet.FragmentFlags);
                                listBoxParse2.Items.Add("fragmentOffset:" + pp.ipv4Packet.FragmentOffset);
                                listBoxParse2.Items.Add("TTL:" + pp.ipv4Packet.TimeToLive);
                                listBoxParse2.Items.Add("protocol:" + pp.ipv4Packet.Protocol);
                                listBoxParse2.Items.Add("headerChecksum:" + pp.ipv4Packet.Checksum);
                                listBoxParse2.Items.Add("sourceIP:" + pp.ipv4Packet.SourceAddress.ToString());
                                listBoxParse2.Items.Add("destinationIP:" + pp.ipv4Packet.DestinationAddress.ToString());
                            }
                            else
                            {
                                listBoxParse2.Items.Add("IPv4 null");
                            }
                            break;
                        case EthernetType.IPv6:
                            if (pp.ipv6Packet != null)
                            {
                                listBoxParse2.Items.Add("--------------------");
                                listBoxParse2.Items.Add("IPv6");
                                listBoxParse2.Items.Add("--------------------");
                                listBoxParse2.Items.Add("version:" + pp.ipv6Packet.Version);
                                listBoxParse2.Items.Add("trafficClass:" + pp.ipv6Packet.TrafficClass);
                                listBoxParse2.Items.Add("flowLabel:" + pp.ipv6Packet.FlowLabel);
                                listBoxParse2.Items.Add("payloadLength:" + pp.ipv6Packet.PayloadLength);
                                listBoxParse2.Items.Add("nextHeader:" + pp.ipv6Packet.NextHeader);
                                listBoxParse2.Items.Add("hopLimit:" + pp.ipv6Packet.HopLimit);
                                listBoxParse2.Items.Add("sourceAddress:" + pp.ipv6Packet.SourceAddress.ToString());
                                listBoxParse2.Items.Add("destinationAddress:" + pp.ipv6Packet.DestinationAddress.ToString());
                            }
                            else
                            {
                                listBoxParse2.Items.Add("IPv6 null");
                            }
                            break;
                        case EthernetType.Arp:
                            if (pp.arpPacket != null)
                            {
                                listBoxParse2.Items.Add("--------------------");
                                listBoxParse2.Items.Add("ARP");
                                listBoxParse2.Items.Add("--------------------");
                                listBoxParse2.Items.Add("hardwareAddressType:" + pp.arpPacket.HardwareAddressType);
                                listBoxParse2.Items.Add("protocolAddressType:" + pp.arpPacket.ProtocolAddressType);
                                listBoxParse2.Items.Add("hardwareAddressLength:" + pp.arpPacket.HardwareAddressLength);
                                listBoxParse2.Items.Add("protocolAddressLength:" + pp.arpPacket.ProtocolAddressLength);
                                listBoxParse2.Items.Add("opCode:" + pp.arpPacket.Operation.ToString());
                                listBoxParse2.Items.Add("senderHardwareAddress:" + pp.arpPacket.SenderHardwareAddress.ToString());
                                listBoxParse2.Items.Add("senderProtocolAddress:" + pp.arpPacket.SenderProtocolAddress.ToString());
                                listBoxParse2.Items.Add("targetHardwareAddress:" + pp.arpPacket.TargetHardwareAddress.ToString());
                                listBoxParse2.Items.Add("targetProtocolAddress:" + pp.arpPacket.TargetProtocolAddress.ToString());
                            }
                            else
                            {
                                listBoxParse2.Items.Add("ARP null");
                            }
                            break;
                    }
                    switch (pp.transportType)
                    {
                        case ProtocolType.Tcp:
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("TCP");
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("sourcePort:" + pp.tcpPacket.SourcePort);
                            listBoxParse2.Items.Add("destinationPort:" + pp.tcpPacket.DestinationPort);
                            listBoxParse2.Items.Add("sequenceNumber:" + pp.tcpPacket.SequenceNumber);
                            listBoxParse2.Items.Add("acknowledgementNumber:" + pp.tcpPacket.AcknowledgmentNumber);
                            listBoxParse2.Items.Add("dataOffset:" + pp.tcpPacket.DataOffset);
                            listBoxParse2.Items.Add("flags:" + pp.tcpPacket.Flags);
                            listBoxParse2.Items.Add("windowSize:" + pp.tcpPacket.WindowSize);
                            listBoxParse2.Items.Add("checksum:" + pp.tcpPacket.Checksum);
                            listBoxParse2.Items.Add("urgentPointer:" + pp.tcpPacket.UrgentPointer);
                            listBoxParse2.Items.Add("options:" + BitConverter.ToString(pp.tcpPacket.Options).Replace("-", ":"));
                            listBoxParse2.Items.Add("payload:" + BitConverter.ToString(pp.tcpPacket.PayloadData).Replace("-", " "));
                            listBoxParse2.Items.Add("hasPayloadPacket:" + pp.tcpPacket.HasPayloadPacket);
                            listBoxParse2.Items.Add("hasPayloadData:" + pp.tcpPacket.HasPayloadData);
                            if (pp.tcpPacket.HasPayloadPacket)
                            {
                                for (int i = 0; i < pp.payloadPackets.Count; i++)
                                {
                                    listBoxParse2.Items.Add("payload[" + i + "]:" + BitConverter.ToString(pp.payloadPackets[i].Bytes).Replace("-", " "));
                                }
                            }
                            break;
                        case ProtocolType.Udp:
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("UDP");
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("sourcePort:" + pp.udpPacket.SourcePort);
                            listBoxParse2.Items.Add("destinationPort:" + pp.udpPacket.DestinationPort);
                            listBoxParse2.Items.Add("length:" + pp.udpPacket.Length);
                            listBoxParse2.Items.Add("checksum:" + pp.udpPacket.Checksum);
                            listBoxParse2.Items.Add("payload:" + BitConverter.ToString(pp.udpPacket.PayloadData).Replace("-", " "));
                            listBoxParse2.Items.Add("hasPayloadPacket:" + pp.udpPacket.HasPayloadPacket);
                            listBoxParse2.Items.Add("hasPayloadData:" + pp.udpPacket.HasPayloadData);
                            if (pp.udpPacket.HasPayloadPacket)
                            {
                                for (int i = 0; i < pp.payloadPackets.Count; i++)
                                {
                                    listBoxParse2.Items.Add("payload[" + i + "]:" + BitConverter.ToString(pp.payloadPackets[i].Bytes).Replace("-", " "));
                                }
                            }
                            break;
                        case ProtocolType.Icmp:
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("ICMPv4");
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("checksum:" + pp.icmpv4Packet.Checksum);
                            listBoxParse2.Items.Add("identification:" + pp.icmpv4Packet.Id);
                            listBoxParse2.Items.Add("hasPayloadPacket:" + pp.icmpv4Packet.HasPayloadPacket);
                            listBoxParse2.Items.Add("hasPayloadData:" + pp.icmpv4Packet.HasPayloadData);
                            if (pp.icmpv4Packet.HasPayloadPacket)
                            {
                                for (int i = 0; i < pp.payloadPackets.Count; i++)
                                {
                                    listBoxParse2.Items.Add("payload[" + i + "]:" + BitConverter.ToString(pp.payloadPackets[i].Bytes).Replace("-", " "));
                                }
                            }
                            break;
                        case ProtocolType.IcmpV6:
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("ICMPv6");
                            listBoxParse2.Items.Add("--------------------------------------------------");
                            listBoxParse2.Items.Add("checksum:" + pp.icmpv6Packet.Checksum);
                            listBoxParse2.Items.Add("totalLength:" + pp.icmpv6Packet.TotalPacketLength);
                            listBoxParse2.Items.Add("hasPayloadPacket:" + pp.icmpv6Packet.HasPayloadPacket);
                            listBoxParse2.Items.Add("hasPayloadData:" + pp.icmpv6Packet.HasPayloadData);
                            listBoxParse2.Items.Add("code:" + pp.icmpv6Packet.Code);
                            if (pp.icmpv6Packet.HasPayloadPacket)
                            {
                                for (int i = 0; i < pp.payloadPackets.Count; i++)
                                {
                                    listBoxParse2.Items.Add("payload[" + i + "]:" + BitConverter.ToString(pp.payloadPackets[i].Bytes).Replace("-", " "));
                                }
                            }
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

            if (parsedPacketDict2 != null) parsedPacketDict2.Clear();
            if (parsedPacketList != null) parsedPacketList.Clear();
        }

        // 清空按钮
        private void buttonClear_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void listViewPacket_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectPacketChanged(sender);
        }

        // 列表项上弹出右键菜单
        private void listViewPacket_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewPacket.GetItemAt(e.X, e.Y);
            if (item != null && e.Button == MouseButtons.Right)
            {
                curItem = item;
                int frame = curItem.Index + 1;
                tracePacket = parsedPacketDict2[frame];

                traceTCPToolStripMenuItem.Enabled = false;
                traceUDPToolStripMenuItem.Enabled = false;
                switch (tracePacket.ethernetType)
                {
                    case EthernetType.IPv4:
                    case EthernetType.IPv6:
                        switch (tracePacket.transportType)
                        {
                            case ProtocolType.Tcp:
                                traceTCPToolStripMenuItem.Enabled = true;
                                break;
                            case ProtocolType.Udp:
                                traceUDPToolStripMenuItem.Enabled = true;
                                break;
                        }
                        break;
                }
                listviewMenuStrip.Show(listViewPacket, e.X, e.Y);
            }
        }

        // 右键菜单中点"TCP流"按钮
        private void traceTCPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tracePacket != null)
            {
                tracing = true;
                
                IPAddress sourceIP = null, destinationIP = null;
                if (tracePacket.ethernetType == EthernetType.IPv4)
                {
                    sourceIP = tracePacket.ipv4Packet.SourceAddress;
                    destinationIP = tracePacket.ipv4Packet.DestinationAddress;
                }
                else if (tracePacket.ethernetType == EthernetType.IPv6)
                {
                    sourceIP = tracePacket.ipv6Packet.SourceAddress;
                    destinationIP = tracePacket.ipv6Packet.DestinationAddress;
                }

                textBoxTrace.Text = sourceIP.ToString() + " <-> " + destinationIP.ToString();

                List<ParsedPacket> list = TraceIP(tracePacket);

                listViewTrace.Items.Clear();
                AppendPacketListToListView(list, listViewTrace, true);


            }
        }

        private void radioButtonBin_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBin.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintBin(rawDict[selectIndex]);
        }

        private void radioButtonHex_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonHex.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintHex(rawDict[selectIndex]);
        }

        private List<ParsedPacket> TraceIP(ParsedPacket target)
        {
            List<ParsedPacket> list = new List<ParsedPacket>();
            ParsedPacket packet;
            for (int i = 0; i < frame; i++)
            {
                if (parsedPacketDict2.ContainsKey(i))
                {
                    packet = parsedPacketDict2[i];
                    if (Match(packet, target)) list.Add(packet);
                }
            }
            return list;
        }



        // 比较两者的IP和Port
        // TODO:当处于追踪状态时收到新数据包要判断是否插入列表

        private bool Match(ParsedPacket a, ParsedPacket b)
        {
            if (a.ethernetType != b.ethernetType) return false;
            if (a.transportType != b.transportType) return false;
            if (a.ethernetType == EthernetType.IPv4)
            { }
            else if (a.ethernetType == EthernetType.IPv6)
            { }
            else return false;
            if (a.transportType == ProtocolType.Tcp)
            {
                if (a.tcpPacket.SourcePort.Equals(b.tcpPacket.SourcePort) && a.tcpPacket.DestinationPort.Equals(b.tcpPacket.DestinationPort))
                { }
                else if (a.tcpPacket.SourcePort.Equals(b.tcpPacket.DestinationPort) && a.tcpPacket.DestinationPort.Equals(b.tcpPacket.SourcePort))
                { }
                else return false;
            }
            else if (a.transportType == ProtocolType.Udp)
            {
                if (a.udpPacket.SourcePort.Equals(b.udpPacket.SourcePort) && a.udpPacket.DestinationPort.Equals(b.udpPacket.DestinationPort))
                { }
                else if (a.udpPacket.SourcePort.Equals(b.udpPacket.DestinationPort) && a.udpPacket.DestinationPort.Equals(b.udpPacket.SourcePort))
                { }
                else return false;
            }
            else return false;
            return true;
        }


        private ListViewItem AppendPacketListToListView(List<ParsedPacket> list, ListView curListView, bool checkMatch)
        {
            if (checkMatch && !tracing) return null;

            ListViewItem item = null;
            ParsedPacket parsed;
            for (int i = 0; i < list.Count; i++)
            {
                parsed = list[i];

                if (checkMatch && tracing)
                {
                    if (!Match(parsed, tracePacket)) continue;
                }

                item = new ListViewItem(parsed.frame.ToString());
                item.SubItems.Add(parsed.time.ToString());

                switch (parsed.ethernetType)
                {
                    case EthernetType.IPv4:
                        item.SubItems.Add(parsed.ipv4Packet.SourceAddress.ToString());
                        item.SubItems.Add(parsed.ipv4Packet.DestinationAddress.ToString());
                        break;
                    case EthernetType.IPv6:
                        item.SubItems.Add(parsed.ipv6Packet.SourceAddress.ToString());
                        item.SubItems.Add(parsed.ipv6Packet.DestinationAddress.ToString());
                        break;
                    case EthernetType.Arp:
                        item.SubItems.Add(parsed.arpPacket.SenderProtocolAddress.ToString());
                        item.SubItems.Add(parsed.arpPacket.TargetProtocolAddress.ToString());
                        break;
                    default:
                        item.SubItems.Add("");
                        item.SubItems.Add("");
                        break;
                }
                item.SubItems.Add(parsed.length.ToString());
                item.SubItems.Add(parsed.ethernetType.ToString());
                item.SubItems.Add(parsed.transportType.ToString());

                switch (parsed.transportType)
                {
                    case ProtocolType.Tcp:
                        item.SubItems.Add(parsed.tcpPacket.SourcePort.ToString());
                        item.SubItems.Add(parsed.tcpPacket.DestinationPort.ToString());
                        break;
                    case ProtocolType.Udp:
                        item.SubItems.Add(parsed.udpPacket.SourcePort.ToString());
                        item.SubItems.Add(parsed.udpPacket.DestinationPort.ToString());
                        break;
                }

                curListView.Items.Add(item);

                
            }
            return item;
        }

        // 取消追踪
        private void buttonCancelTrace_Click(object sender, EventArgs e)
        {
            tracing = false;

            textBoxTrace.Text = string.Empty;

            listViewTrace.Items.Clear();

            //listViewPacket.Items.Clear();

            //List<ParsedPacket> list = new List<ParsedPacket>();
            //for (int i = 0; i < parsedPacketDict2.Count; i++)
            //{
            //    if (parsedPacketDict2.ContainsKey(i))
            //        list.Add(parsedPacketDict2[i]);
            //}
            //AppendPacketListToListView(list);
        }

        private void listViewTrace_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectPacketChanged(sender);
        }

        private void traceUDPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            traceTCPToolStripMenuItem_Click(sender, e);
        }
    }
}
