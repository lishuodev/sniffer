using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WinSniffer.ProtocolAnalyzer;

namespace WinSniffer
{
    // 主窗体
    public partial class MainForm : Form
    {
        private ICaptureDevice device; // 设备
        private Thread listviewThread; // listview管理线程
        private Thread analyzerThread; // 数据包分析线程
        private PacketArrivalEventHandler arrivalEventHandler; // 包到达事件处理句柄
        private CaptureStoppedEventHandler captureStoppedEventHandler; // 停止事件处理句柄
        private readonly object queueLock = new object(); // 队列锁
        private readonly object parsedListLock = new object(); // 队列锁
        private readonly object listViewLock = new object();
        private readonly object parsedDictLock = new object();

        // 状态
        private bool analyzeThreadStop = false;
        private bool listviewThreadStop = false;
        private bool running = false;   // 抓包开启
        private bool tracing = false;   // 流追踪开启

        // 配置
        private const int threadDelay = 200;

        // 数据集合
        private List<RawCapture> packetQueue = new List<RawCapture>();  // 包队列
        private Dictionary<int, ParsedPacket> parsedPacketDict;         // 已解析数据包字典
        private List<ParsedPacket> parsedPacketList;                    // 待插入到listview的数据包列表
        private Dictionary<int, byte[]> rawDict;                        // 原始数据包字典

        // 数据
        private int deviceId;               // 当前设备序号
        private int frame;                  // 当前数据包帧号
        private PosixTimeval startTime;     // 抓包开始时间
        private ParsedPacket tracePacket;
        private int selectFrame;            // 选中数据包帧号

        // 控件
        private List<CheckBox> disableList;     // 开始后禁用列表
        private List<CheckBox> checkBoxListenList;  // 监听过滤
        private List<CheckBox> checkBoxDisplayList; // 显示过滤
        private TraceForm traceForm;            // 流追踪窗口

        private bool checkingDisplayAll = false;
        private bool checkingDisplaySingle = false;

        private bool checkingListenAll = false;
        private bool checkingListenSingle = false;

        private bool folding = false;

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

            parsedPacketDict = new Dictionary<int, ParsedPacket>();

            disableList = new List<CheckBox>
            {
                checkBoxPromiscuous, checkBoxListenIPv4, checkBoxListenIPv6, checkBoxListenICMP, checkBoxListenARP, checkBoxListenTCP, checkBoxListenUDP, checkBoxListenTLS, checkBoxListenHTTP, checkBoxListenAll,
            };

            checkBoxListenList = new List<CheckBox>
            {
                checkBoxListenIPv4, checkBoxListenIPv6, checkBoxListenICMP, checkBoxListenARP, checkBoxListenTCP, checkBoxListenUDP, checkBoxListenTLS, checkBoxListenHTTP,
            };

            checkBoxDisplayList = new List<CheckBox>
            {
                checkBoxDisplayIPv4, checkBoxDisplayIPv6, checkBoxDisplayICMP, checkBoxDisplayARP, checkBoxDisplayTCP, checkBoxDisplayUDP, checkBoxDisplayTLS, checkBoxDisplayHTTP,
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
            analyzerThread?.Abort();
            listviewThread?.Abort();
            Application.Exit();
        }

        // 更新列表显示
        private void UpdateListView()
        {
            List<ParsedPacket> curList;
            lock (parsedListLock)
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
                lock (parsedListLock)
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
                    catch (InvalidOperationException)
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

            foreach (var checkBox in disableList)
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

            foreach (var checkBox in disableList)
            {
                checkBox.Enabled = false;
            }

            frame = 0;
            startTime = new PosixTimeval(DateTime.Now);
            parsedPacketList = new List<ParsedPacket>();
            parsedPacketDict = new Dictionary<int, ParsedPacket>();

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

            device.Filter = MakeFilters();

            device.StartCapture();
        }


        // 设置过滤器
        private string MakeFilters()
        {
            // 全选
            if (checkBoxListenAll.Checked) return string.Empty;

            List<string> filters = new List<string>();
            // 网络层
            if (checkBoxListenIPv4.Checked) filters.Add("ip");
            if (checkBoxListenIPv6.Checked) filters.Add("ip6");
            if (checkBoxListenICMP.Checked) filters.Add("icmp or icmp6");
            if (checkBoxListenARP.Checked) filters.Add("arp");
            // 传输层
            if (checkBoxListenTCP.Checked)
            {
                if (checkBoxListenIPv4.Checked) filters.Add("(ip proto \\tcp)");
                else if (checkBoxListenIPv6.Checked) filters.Add("(ip6 proto \\tcp)");
                else filters.Add("(ip proto \\tcp) or (ip6 proto \\tcp)");
            }
            if (checkBoxListenUDP.Checked)
            {
                if (checkBoxListenIPv4.Checked) filters.Add("(ip proto \\udp)");
                else if (checkBoxListenIPv6.Checked) filters.Add("(ip6 proto \\udp)");
                else filters.Add("(ip proto \\udp) or (ip6 proto \\udp)");
            }

            if (checkBoxListenTLS.Checked) filters.Add("(tcp port 443)");

            // 应用层
            if (checkBoxListenHTTP.Checked) filters.Add("(tcp port 80)");

            return string.Join(" or ", filters);
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

                        ParsedPacket pp = NetParser.ParsePacket(frame, raw);
                        pp.time = raw.Timeval.Value - startTime.Value;

                        lock (parsedDictLock)
                        {
                            parsedPacketDict.Add(frame, pp);
                        }

                        rawDict.Add(frame, raw.Data);

                        lock (parsedListLock)
                        {
                            parsedPacketList.Add(pp);
                        }
                    }
                }
            }
        }

        // ▶▼▲◀
        // 选中一个数据包
        private void OnSelectPacketChanged(object sender)
        {
            if (sender is ListView view)
            {
                if (view.SelectedIndices.Count > 0)
                {
                    ListViewItem item = view.SelectedItems[0];
                    selectFrame = int.Parse(item.SubItems[0].Text);

                    if (radioButtonBin.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintBin(rawDict[selectFrame]);
                    else if (radioButtonHex.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintHex(rawDict[selectFrame]);

                    listBoxParse.Items.Clear();
                    ParsedPacket pp = parsedPacketDict[selectFrame];

                    string s1 = string.Format("＞ [1] Frame {0}: {1} bytes ({2} bits) on interface {3}", selectFrame, pp.length, pp.length * 8, device.Name);
                    listBoxParse.Items.Add(s1);

                    string s2 = string.Format("＞ [2] {0}, Src: ({1}), Dst: ({2}), ", pp.linkLayerType, BitConverter.ToString(pp.sourceMac.GetAddressBytes()).Replace("-", ":"), BitConverter.ToString(pp.destinationMac.GetAddressBytes()).Replace("-", ":"), pp.ethernetType);
                    listBoxParse.Items.Add(s2);

                    // Type:{3}(0x{4})
                    // pp.ethernetType.ToString("X").PadLeft(4)
                    switch (pp.ethernetType)
                    {
                        case EthernetType.IPv4:
                        case EthernetType.IPv6:
                            string sip = string.Format("＞ [3] {0}, Src: {1}, Dst: {2}", pp.ethernetType, pp.sourceAddress.ToString(), pp.destinationAddress.ToString());
                            listBoxParse.Items.Add(sip);
                            break;
                        case EthernetType.Arp:
                            string sarp = string.Format("＞ [3] {0} ({1})", pp.ethernetType, pp.arpPacket.Operation);
                            listBoxParse.Items.Add(sarp);
                            break;
                    }

                    switch (pp.protocolType)
                    {
                        case ProtocolType.Tcp:
                            string stcp = string.Format("＞ [4] {0}, Src Port: {1}, Dst Prot: {2}, Seq: {3}, Len: {4}", pp.protocolType, pp.sourcePort, pp.destinationPort, pp.tcpPacket.SequenceNumber, pp.tcpPacket.PayloadDataSegment.Length);
                            listBoxParse.Items.Add(stcp);
                            break;
                        case ProtocolType.Udp:
                            string sudp = string.Format("＞ [4] {0}, Src Port: {1}, Dst Prot: {2}", pp.protocolType, pp.sourcePort, pp.destinationPort);
                            listBoxParse.Items.Add(sudp);
                            break;
                        case ProtocolType.Icmp:
                        case ProtocolType.IcmpV6:
                            string sicmp = string.Format("＞ [4] {0}", pp.protocolType);
                            listBoxParse.Items.Add(sicmp);
                            break;
                    }

                    if (pp.protocolType == ProtocolType.Tcp)
                    {
                        if (pp.sourcePort == 443 || pp.destinationPort == 443) // TLS
                        {
                            string stls = string.Format("＞ [5] Transport Layer Security");
                            listBoxParse.Items.Add(stls);
                        }
                    }

                    /*
                    
                    listBoxParse.Items.Add("--------------------");
                    listBoxParse.Items.Add("Data Link Layer");
                    listBoxParse.Items.Add("--------------------");
                    listBoxParse.Items.Add("LinkLayerType:" + pp.linkLayerType);
                    listBoxParse.Items.Add("sourceMac:" + BitConverter.ToString(pp.sourceMac.GetAddressBytes()).Replace("-", ":"));
                    listBoxParse.Items.Add("destinationMac:" + BitConverter.ToString(pp.destinationMac.GetAddressBytes()).Replace("-", ":"));
                    listBoxParse.Items.Add("ethernetType:" + pp.ethernetType);
                    switch (pp.ethernetType)
                    {
                        case EthernetType.IPv4:
                            if (pp.ipv4Packet != null)
                            {
                                listBoxParse.Items.Add("--------------------");
                                listBoxParse.Items.Add("IPv4");
                                listBoxParse.Items.Add("--------------------");
                                listBoxParse.Items.Add("version:" + pp.ipv4Packet.Version);
                                listBoxParse.Items.Add("headerLength:" + pp.ipv4Packet.HeaderLength);
                                listBoxParse.Items.Add("ToS:" + pp.ipv4Packet.TypeOfService);
                                listBoxParse.Items.Add("totalLength:" + pp.ipv4Packet.TotalLength);
                                listBoxParse.Items.Add("identification:" + pp.ipv4Packet.Id);
                                listBoxParse.Items.Add("flags:" + pp.ipv4Packet.FragmentFlags);
                                listBoxParse.Items.Add("fragmentOffset:" + pp.ipv4Packet.FragmentOffset);
                                listBoxParse.Items.Add("TTL:" + pp.ipv4Packet.TimeToLive);
                                listBoxParse.Items.Add("protocol:" + pp.ipv4Packet.Protocol);
                                listBoxParse.Items.Add("headerChecksum:" + pp.ipv4Packet.Checksum);
                                listBoxParse.Items.Add("sourceIP:" + pp.ipv4Packet.SourceAddress.ToString());
                                listBoxParse.Items.Add("destinationIP:" + pp.ipv4Packet.DestinationAddress.ToString());
                            }
                            else
                            {
                                listBoxParse.Items.Add("IPv4 null");
                            }
                            break;
                        case EthernetType.IPv6:
                            if (pp.ipv6Packet != null)
                            {
                                listBoxParse.Items.Add("--------------------");
                                listBoxParse.Items.Add("IPv6");
                                listBoxParse.Items.Add("--------------------");
                                listBoxParse.Items.Add("version:" + pp.ipv6Packet.Version);
                                listBoxParse.Items.Add("trafficClass:" + pp.ipv6Packet.TrafficClass);
                                listBoxParse.Items.Add("flowLabel:" + pp.ipv6Packet.FlowLabel);
                                listBoxParse.Items.Add("payloadLength:" + pp.ipv6Packet.PayloadLength);
                                listBoxParse.Items.Add("nextHeader:" + pp.ipv6Packet.NextHeader);
                                listBoxParse.Items.Add("hopLimit:" + pp.ipv6Packet.HopLimit);
                                listBoxParse.Items.Add("sourceAddress:" + pp.ipv6Packet.SourceAddress.ToString());
                                listBoxParse.Items.Add("destinationAddress:" + pp.ipv6Packet.DestinationAddress.ToString());
                            }
                            else
                            {
                                listBoxParse.Items.Add("IPv6 null");
                            }
                            break;
                        case EthernetType.Arp:
                            if (pp.arpPacket != null)
                            {
                                listBoxParse.Items.Add("--------------------");
                                listBoxParse.Items.Add("ARP");
                                listBoxParse.Items.Add("--------------------");
                                listBoxParse.Items.Add("hardwareAddressType:" + pp.arpPacket.HardwareAddressType);
                                listBoxParse.Items.Add("protocolAddressType:" + pp.arpPacket.ProtocolAddressType);
                                listBoxParse.Items.Add("hardwareAddressLength:" + pp.arpPacket.HardwareAddressLength);
                                listBoxParse.Items.Add("protocolAddressLength:" + pp.arpPacket.ProtocolAddressLength);
                                listBoxParse.Items.Add("opCode:" + pp.arpPacket.Operation.ToString());
                                listBoxParse.Items.Add("senderHardwareAddress:" + pp.arpPacket.SenderHardwareAddress.ToString());
                                listBoxParse.Items.Add("senderProtocolAddress:" + pp.arpPacket.SenderProtocolAddress.ToString());
                                listBoxParse.Items.Add("targetHardwareAddress:" + pp.arpPacket.TargetHardwareAddress.ToString());
                                listBoxParse.Items.Add("targetProtocolAddress:" + pp.arpPacket.TargetProtocolAddress.ToString());
                            }
                            else
                            {
                                listBoxParse.Items.Add("ARP null");
                            }
                            break;
                    }
                    switch (pp.transportType)
                    {
                        case ProtocolType.Tcp:
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("TCP");
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("sourcePort:" + pp.tcpPacket.SourcePort);
                            listBoxParse.Items.Add("destinationPort:" + pp.tcpPacket.DestinationPort);
                            listBoxParse.Items.Add("sequenceNumber:" + pp.tcpPacket.SequenceNumber);
                            listBoxParse.Items.Add("acknowledgementNumber:" + pp.tcpPacket.AcknowledgmentNumber);
                            listBoxParse.Items.Add("dataOffset:" + pp.tcpPacket.DataOffset);
                            listBoxParse.Items.Add("flags:" + pp.tcpPacket.Flags);
                            listBoxParse.Items.Add("windowSize:" + pp.tcpPacket.WindowSize);
                            listBoxParse.Items.Add("checksum:" + pp.tcpPacket.Checksum);
                            listBoxParse.Items.Add("urgentPointer:" + pp.tcpPacket.UrgentPointer);
                            listBoxParse.Items.Add("options:" + BitConverter.ToString(pp.tcpPacket.Options).Replace("-", ":"));
                            listBoxParse.Items.Add("payload:" + BitConverter.ToString(pp.tcpPacket.PayloadData).Replace("-", " "));
                            listBoxParse.Items.Add("hasPayloadPacket:" + pp.tcpPacket.HasPayloadPacket);
                            listBoxParse.Items.Add("hasPayloadData:" + pp.tcpPacket.HasPayloadData);
                            if (pp.tcpPacket.HasPayloadPacket)
                            {
                                for (int i = 0; i < pp.payloadPackets.Count; i++)
                                {
                                    listBoxParse.Items.Add("payload[" + i + "]:" + BitConverter.ToString(pp.payloadPackets[i].Bytes).Replace("-", " "));
                                }
                            }
                            break;
                        case ProtocolType.Udp:
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("UDP");
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("sourcePort:" + pp.udpPacket.SourcePort);
                            listBoxParse.Items.Add("destinationPort:" + pp.udpPacket.DestinationPort);
                            listBoxParse.Items.Add("length:" + pp.udpPacket.Length);
                            listBoxParse.Items.Add("checksum:" + pp.udpPacket.Checksum);
                            listBoxParse.Items.Add("payload:" + BitConverter.ToString(pp.udpPacket.PayloadData).Replace("-", " "));
                            listBoxParse.Items.Add("hasPayloadPacket:" + pp.udpPacket.HasPayloadPacket);
                            listBoxParse.Items.Add("hasPayloadData:" + pp.udpPacket.HasPayloadData);
                            if (pp.udpPacket.HasPayloadPacket)
                            {
                                for (int i = 0; i < pp.payloadPackets.Count; i++)
                                {
                                    listBoxParse.Items.Add("payload[" + i + "]:" + BitConverter.ToString(pp.payloadPackets[i].Bytes).Replace("-", " "));
                                }
                            }
                            break;
                        case ProtocolType.Icmp:
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("ICMPv4");
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("checksum:" + pp.icmpv4Packet.Checksum);
                            listBoxParse.Items.Add("identification:" + pp.icmpv4Packet.Id);
                            listBoxParse.Items.Add("hasPayloadPacket:" + pp.icmpv4Packet.HasPayloadPacket);
                            listBoxParse.Items.Add("hasPayloadData:" + pp.icmpv4Packet.HasPayloadData);
                            if (pp.icmpv4Packet.HasPayloadPacket)
                            {
                                for (int i = 0; i < pp.payloadPackets.Count; i++)
                                {
                                    listBoxParse.Items.Add("payload[" + i + "]:" + BitConverter.ToString(pp.payloadPackets[i].Bytes).Replace("-", " "));
                                }
                            }
                            break;
                        case ProtocolType.IcmpV6:
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("ICMPv6");
                            listBoxParse.Items.Add("--------------------------------------------------");
                            listBoxParse.Items.Add("checksum:" + pp.icmpv6Packet.Checksum);
                            listBoxParse.Items.Add("totalLength:" + pp.icmpv6Packet.TotalPacketLength);
                            listBoxParse.Items.Add("hasPayloadPacket:" + pp.icmpv6Packet.HasPayloadPacket);
                            listBoxParse.Items.Add("hasPayloadData:" + pp.icmpv6Packet.HasPayloadData);
                            listBoxParse.Items.Add("code:" + pp.icmpv6Packet.Code);
                            if (pp.icmpv6Packet.HasPayloadPacket)
                            {
                                for (int i = 0; i < pp.payloadPackets.Count; i++)
                                {
                                    listBoxParse.Items.Add("payload[" + i + "]:" + BitConverter.ToString(pp.payloadPackets[i].Bytes).Replace("-", " "));
                                }
                            }
                            break;
                    }
                    */
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
        private void ButtonStop_Click(object sender, EventArgs e)
        {
            StopCapture();
        }

        // 开始按钮
        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (comboBoxDeviceList.SelectedIndex == -1)
            {
                MessageBox.Show("请先选择监听设备!", "提示");
            }
            else
            {
                StartCapture();
            }
        }

        private void Reset()
        {
            // 数据
            parsedPacketDict?.Clear();
            parsedPacketList?.Clear();
            rawDict?.Clear();
            packetQueue?.Clear();

            // 显示
            listViewPacket.Items?.Clear();
            listViewTrace.Items?.Clear();
            listBoxParse.Items?.Clear();
            textBoxBinary?.Clear();
        }

        // 清空按钮
        private void ButtonClear_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("确定要清空所有数据吗?", "清空", MessageBoxButtons.OKCancel);
            switch (result)
            {
                case DialogResult.OK:
                    Reset();
                    break;
                case DialogResult.Cancel:
                    break;
            }
        }

        private void ListViewPacket_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectPacketChanged(sender);
        }

        private void ListBoxParse_MouseClick(object sender, MouseEventArgs e)
        {
            int index = listBoxParse.IndexFromPoint(e.Location);
            if (index < 0 || index >= listBoxParse.Items.Count)
                listBoxParse.ClearSelected();
            FoldListBoxParse();
        }

        // 列表项上弹出右键菜单
        private void ListViewPacket_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewPacket.GetItemAt(e.X, e.Y);
            if (item != null && e.Button == MouseButtons.Right)
            {
                int frame = int.Parse(item.SubItems[0].Text);

                tracePacket = parsedPacketDict[frame];

                traceTCPToolStripMenuItem.Enabled = false;
                traceUDPToolStripMenuItem.Enabled = false;
                switch (tracePacket.ethernetType)
                {
                    case EthernetType.IPv4:
                    case EthernetType.IPv6:
                        switch (tracePacket.protocolType)
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
        private void TraceTCPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tracePacket != null)
            {
                tracing = true;

                List<ParsedPacket> traceList = TraceIP(tracePacket);

                listViewTrace.Items.Clear();
                AppendPacketListToListView(traceList, listViewTrace, true);

                DataCache.tracePacketList = traceList;

                traceForm?.Close();
                traceForm = new TraceForm();
                traceForm.Show();
            }
        }

        private void RadioButtonBin_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBin.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintBin(rawDict[selectFrame]);
        }

        private void RadioButtonHex_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonHex.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintHex(rawDict[selectFrame]);
        }

        private List<ParsedPacket> TraceIP(ParsedPacket target)
        {
            List<ParsedPacket> list = new List<ParsedPacket>();
            ParsedPacket packet;
            for (int i = 0; i < frame; i++)
            {
                if (parsedPacketDict.ContainsKey(i))
                {
                    packet = parsedPacketDict[i];
                    if (Match(packet, target)) list.Add(packet);
                }
            }
            return list;
        }

        private bool Match(ParsedPacket a, ParsedPacket b)
        {
            if (a.ethernetType != b.ethernetType) return false;
            if (a.protocolType != b.protocolType) return false;
            if (!(a.ethernetType == EthernetType.IPv4 || a.ethernetType == EthernetType.IPv6)) return false;

            if (a.sourceAddress.Equals(b.sourceAddress) && a.destinationAddress.Equals(b.destinationAddress))
            { }
            else if (a.sourceAddress.Equals(b.destinationAddress) && a.destinationAddress.Equals(b.sourceAddress))
            { }
            else return false;

            if (a.sourcePort.Equals(b.sourcePort) && a.destinationPort.Equals(b.destinationPort))
            { }
            else if (a.sourcePort.Equals(b.destinationPort) && a.destinationPort.Equals(b.sourcePort))
            { }
            else return false;

            return true;
        }

        private ListViewItem AppendPacketToListView(ParsedPacket parsed, ListView listView, bool checkMatch)
        {
            if (checkMatch && !tracing) return null;
            if (!FilterPacket(parsed)) return null;

            if (checkMatch && tracing)
            {
                if (!Match(parsed, tracePacket)) return null;
            }

            ListViewItem item = new ListViewItem(parsed.frame.ToString());
            item.SubItems.Add(parsed.time.ToString());

            switch (parsed.ethernetType)
            {
                case EthernetType.IPv4:
                case EthernetType.IPv6:
                    item.SubItems.Add(parsed.sourceAddress.ToString());
                    item.SubItems.Add(parsed.destinationAddress.ToString());
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
            item.SubItems.Add(parsed.protocolType.ToString());

            switch (parsed.protocolType)
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

            listView.Items.Add(item);
            return item;
        }


        private ListViewItem AppendPacketListToListView(List<ParsedPacket> list, ListView listView, bool checkMatch)
        {
            if (checkMatch && !tracing) return null;

            ListViewItem item = null;
            ParsedPacket parsed;

            lock (listViewLock)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    parsed = list[i];
                    item = AppendPacketToListView(parsed, listView, checkMatch);
                }
            }
            return item;
        }

        private void ListViewTrace_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectPacketChanged(sender);
        }

        private void TraceUDPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TraceTCPToolStripMenuItem_Click(sender, e);
        }

        // 重建listViewPacket
        private void Rebuild()
        {
            lock (listViewLock)
            {
                listViewPacket.Items.Clear();
                ParsedPacket packet;
                ListViewItem item = null;

                lock (parsedDictLock)
                {
                    for (int frame = 1; frame <= parsedPacketDict.Count; frame++)
                    {
                        packet = parsedPacketDict[frame];
                        item = AppendPacketToListView(packet, listViewPacket, false);
                    }
                }

                if (checkBoxAutoScroll.Checked)
                {
                    item?.EnsureVisible();
                }
            }
        }

        // 过滤
        private bool FilterPacket(ParsedPacket packet)
        {
            if (checkBoxDisplayAll.Checked) return true;
            if (checkBoxDisplayIPv4.Checked && packet.ethernetType == EthernetType.IPv4) return true;
            if (checkBoxDisplayIPv6.Checked && packet.ethernetType == EthernetType.IPv6) return true;
            if (checkBoxDisplayTCP.Checked && packet.protocolType == ProtocolType.Tcp) return true;
            if (checkBoxDisplayUDP.Checked && packet.protocolType == ProtocolType.Udp) return true;
            if (checkBoxDisplayARP.Checked && packet.ethernetType == EthernetType.Arp) return true;
            if (checkBoxDisplayICMP.Checked && (packet.protocolType == ProtocolType.Icmp || packet.protocolType == ProtocolType.IcmpV6)) return true;
            if (checkBoxDisplayTLS.Checked && packet.protocolType == ProtocolType.Tcp && (packet.sourcePort == 443 || packet.destinationPort == 443)) return true;
            if (checkBoxDisplayHTTP.Checked && packet.protocolType == ProtocolType.Tcp && (packet.sourcePort == 80 || packet.destinationPort == 80)) return true;

            return false;
        }

        private void CheckBoxDisplayIPv4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplayAll) return;
            checkingDisplaySingle = true;
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayIPv4.Checked)
                checkBoxDisplayAll.Checked = false;
            checkingDisplaySingle = false;
            Rebuild();
        }

        private void CheckBoxDisplayIPv6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplayAll) return;
            checkingDisplaySingle = true;
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayIPv6.Checked)
                checkBoxDisplayAll.Checked = false;
            checkingDisplaySingle = false;
            Rebuild();
        }

        private void CheckBoxDisplayICMP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplayAll) return;
            checkingDisplaySingle = true;
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayICMP.Checked)
                checkBoxDisplayAll.Checked = false;
            checkingDisplaySingle = false;
            Rebuild();
        }

        private void CheckBoxDisplayARP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplayAll) return;
            checkingDisplaySingle = true;
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayARP.Checked)
                checkBoxDisplayAll.Checked = false;
            checkingDisplaySingle = false;
            Rebuild();
        }

        private void CheckBoxDisplayTCP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplayAll) return;
            checkingDisplaySingle = true;
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayTCP.Checked)
                checkBoxDisplayAll.Checked = false;
            checkingDisplaySingle = false;
            Rebuild();
        }

        private void CheckBoxDisplayUDP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplayAll) return;
            checkingDisplaySingle = true;
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayUDP.Checked)
                checkBoxDisplayAll.Checked = false;
            checkingDisplaySingle = false;
            Rebuild();
        }

        private void CheckBoxDisplayTLS_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplayAll) return;
            checkingDisplaySingle = true;
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayTLS.Checked)
                checkBoxDisplayAll.Checked = false;
            checkingDisplaySingle = false;
            Rebuild();
        }

        private void CheckBoxDisplayHTTP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplayAll) return;
            checkingDisplaySingle = true;
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayHTTP.Checked)
                checkBoxDisplayAll.Checked = false;
            checkingDisplaySingle = false;
            Rebuild();
        }

        private void CheckBoxDisplayAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplaySingle) return;
            checkingDisplayAll = true;
            if (checkBoxDisplayAll.Checked)
            {
                foreach (var checkbox in checkBoxDisplayList)
                    checkbox.Checked = true;
            }
            else
            {
                foreach (var checkbox in checkBoxDisplayList)
                    checkbox.Checked = false;
            }
            checkingDisplayAll = false;
            Rebuild();
        }

        private void CheckBoxListenAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingListenSingle) return;
            checkingListenAll = true;
            if (checkBoxListenAll.Checked)
            {
                foreach (var checkbox in checkBoxListenList)
                    checkbox.Checked = true;
            }
            else
            {
                foreach (var checkbox in checkBoxListenList)
                    checkbox.Checked = false;
            }
            checkingListenAll = false;
        }

        private void CheckBoxListenHTTP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingListenAll) return;
            checkingListenSingle = true;
            if (checkBoxListenAll.Checked && !checkBoxListenHTTP.Checked)
                checkBoxListenAll.Checked = false;
            checkingListenSingle = false;
        }

        private void CheckBoxListenTLS_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingListenAll) return;
            checkingListenSingle = true;
            if (checkBoxListenAll.Checked && !checkBoxListenTLS.Checked)
                checkBoxListenAll.Checked = false;
            checkingListenSingle = false;
        }

        private void CheckBoxListenUDP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingDisplayAll) return;
            checkingListenSingle = true;
            if (checkBoxListenAll.Checked && !checkBoxListenUDP.Checked)
                checkBoxListenAll.Checked = false;
            checkingListenSingle = false;
        }

        private void CheckBoxListenTCP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingListenAll) return;
            checkingListenSingle = true;
            if (checkBoxListenAll.Checked && !checkBoxListenTCP.Checked)
                checkBoxListenAll.Checked = false;
            checkingListenSingle = false;
        }

        private void CheckBoxListenARP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingListenAll) return;
            checkingListenSingle = true;
            if (checkBoxListenAll.Checked && !checkBoxListenARP.Checked)
                checkBoxListenAll.Checked = false;
            checkingListenSingle = false;
        }

        private void CheckBoxListenICMP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingListenAll) return;
            checkingListenSingle = true;
            if (checkBoxListenAll.Checked && !checkBoxListenICMP.Checked)
                checkBoxListenAll.Checked = false;
            checkingListenSingle = false;
        }

        private void CheckBoxListenIPv6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingListenAll) return;
            checkingListenSingle = true;
            if (checkBoxListenAll.Checked && !checkBoxListenIPv6.Checked)
                checkBoxListenAll.Checked = false;
            checkingListenSingle = false;
        }

        private void CheckBoxListenIPv4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkingListenAll) return;
            checkingListenSingle = true;
            if (checkBoxListenAll.Checked && !checkBoxListenIPv4.Checked)
                checkBoxListenAll.Checked = false;
            checkingListenSingle = false;
        }

        private void FoldListBoxParse() 
        { 
            if (folding) return;
            if (listBoxParse.SelectedItem == null) return;
            string s = listBoxParse.SelectedItem as string;
            int index = listBoxParse.SelectedIndex;
            ParsedPacket packet = parsedPacketDict[selectFrame];
            string tab = "       ";
            if (s.StartsWith("＞"))
            {
                folding = true;
                
                listBoxParse.Items.RemoveAt(index);
                listBoxParse.Items.Insert(index, s.Replace("＞", "∨"));
                listBoxParse.SelectedIndex = index;
                string c = s.Substring(3, 1);
                int id = int.Parse(c);
                switch (id)
                {
                    case 1:
                        // Interface name:
                        // Interface description:
                        // Encapsulation Type: Ethernet (1)
                        // Arrival Time: Mar 25, 2023 20:38:06.xxxxxxxxx XXX时间
                        // Epoch Time: xxx.xxx seconds
                        // Frame Number: x
                        // Frame Length: xxx bytes (xxx bits)
                        // Capture Length: xxx bytes (xxx bits)
                        listBoxParse.Items.Insert(index + 1, string.Format("{0}Interface name: {1}", tab, device.Name));
                        listBoxParse.Items.Insert(index + 2, string.Format("{0}Interface description: {1}", tab, device.Description));
                        listBoxParse.Items.Insert(index + 3, string.Format("{0}Encapsulation Type: {1}", tab, packet.linkLayerType));
                        listBoxParse.Items.Insert(index + 4, string.Format("{0}Epoch Time: {1} seconds", tab, packet.timeval));
                        listBoxParse.Items.Insert(index + 5, string.Format("{0}Frame Number: {1}", tab, packet.frame));
                        listBoxParse.Items.Insert(index + 6, string.Format("{0}Frame Length: {1} bytes ({2} bits)", tab, packet.length, packet.length * 8));
                        break;
                    case 2:
                        listBoxParse.Items.Insert(index + 1, string.Format("{0}Destination: {1}", tab, BitConverter.ToString(packet.destinationMac.GetAddressBytes()).Replace("-", ":")));
                        listBoxParse.Items.Insert(index + 2, string.Format("{0}Source: {1}", tab, BitConverter.ToString(packet.sourceMac.GetAddressBytes()).Replace("-", ":")));
                        listBoxParse.Items.Insert(index + 3, string.Format("{0}Type: {1} (0x{2})", tab, packet.ethernetType, packet.ethernetType.ToString("X").PadLeft(4)));
                        break;

                    case 3:
                        switch (packet.ethernetType)
                        {
                            case EthernetType.IPv4:
                                listBoxParse.Items.Insert(index + 1, string.Format("{0}Version: {1} ({2})", tab, packet.ipv4Packet.Version, (int)packet.ipv4Packet.Version));
                                listBoxParse.Items.Insert(index + 2, string.Format("{0}Header Length: {1} bytes ({2})", tab, packet.ipv4Packet.HeaderLength * 4, packet.ipv4Packet.HeaderLength));
                                listBoxParse.Items.Insert(index + 3, string.Format("{0}Total Length: {1}", tab, packet.ipv4Packet.TotalLength));
                                listBoxParse.Items.Insert(index + 4, string.Format("{0}Identification: 0x{1} ({2})", tab, packet.ipv4Packet.Id.ToString("X").PadLeft(4, '0'), packet.ipv4Packet.Id));
                                listBoxParse.Items.Insert(index + 5, string.Format("{0}Flags: {1}", tab, packet.ipv4Packet.FragmentFlags.ToString("X").PadLeft(2, '0')));
                                listBoxParse.Items.Insert(index + 6, string.Format("{0}Fragment Offset: {1}", tab, packet.ipv4Packet.FragmentOffset));
                                listBoxParse.Items.Insert(index + 7, string.Format("{0}Time to Live: {1}", tab, packet.ipv4Packet.TimeToLive));
                                listBoxParse.Items.Insert(index + 8, string.Format("{0}Protocol: {1} ({2})", tab, packet.ipv4Packet.Protocol, (int)packet.ipv4Packet.Protocol));
                                listBoxParse.Items.Insert(index + 9, string.Format("{0}Header Checksum: 0x{1}", tab, packet.ipv4Packet.Checksum.ToString("X").PadLeft(2, '0')));
                                listBoxParse.Items.Insert(index + 10, string.Format("{0}Source Address: {1}", tab, packet.ipv4Packet.SourceAddress));
                                listBoxParse.Items.Insert(index + 11, string.Format("{0}Destination Address: {1}", tab, packet.ipv4Packet.DestinationAddress));
                                break;
                            case EthernetType.IPv6:
                                listBoxParse.Items.Insert(index + 1, string.Format("{0}Version: {1} ({2})", tab, packet.ipv6Packet.Version, (int)packet.ipv6Packet.Version));
                                listBoxParse.Items.Insert(index + 2, string.Format("{0}Traffic Class: 0x{1}", tab, packet.ipv6Packet.TrafficClass.ToString("X").PadLeft(2, '0')));
                                listBoxParse.Items.Insert(index + 3, string.Format("{0}Flow Label: 0x{1}", tab, packet.ipv6Packet.FlowLabel.ToString("X").PadLeft(2, '0')));
                                listBoxParse.Items.Insert(index + 4, string.Format("{0}Payload Length: {1}", tab, packet.ipv6Packet.PayloadLength));
                                listBoxParse.Items.Insert(index + 5, string.Format("{0}Next Header: {1} ({2})", tab, packet.ipv6Packet.NextHeader, (int)packet.ipv6Packet.NextHeader));
                                listBoxParse.Items.Insert(index + 6, string.Format("{0}Hop Limit: {1}", tab, packet.ipv6Packet.HopLimit));
                                listBoxParse.Items.Insert(index + 7, string.Format("{0}Source Address: {1}", tab, packet.ipv6Packet.SourceAddress));
                                listBoxParse.Items.Insert(index + 8, string.Format("{0}Destination Address: {1}", tab, packet.ipv6Packet.DestinationAddress));
                                break;
                            case EthernetType.Arp:
                                listBoxParse.Items.Insert(index + 1, string.Format("{0}Hardware Type: {1} ({2})", tab, packet.arpPacket.HardwareAddressType, (int)packet.arpPacket.HardwareAddressType));
                                listBoxParse.Items.Insert(index + 2, string.Format("{0}Protocl Type: {1} (0x{2})", tab, packet.arpPacket.ProtocolAddressType, packet.arpPacket.ProtocolAddressType.ToString("X").PadLeft(4, '0')));
                                listBoxParse.Items.Insert(index + 3, string.Format("{0}Hardware Size: {1}", tab, packet.arpPacket.HardwareAddressLength));
                                listBoxParse.Items.Insert(index + 4, string.Format("{0}Protocl Size: {1}", tab, packet.arpPacket.ProtocolAddressLength));
                                listBoxParse.Items.Insert(index + 5, string.Format("{0}Opcode: {1} ({2})", tab, packet.arpPacket.Operation, (int)packet.arpPacket.Operation));
                                listBoxParse.Items.Insert(index + 6, string.Format("{0}Sender MAC Address: {1}", tab, BitConverter.ToString(packet.arpPacket.SenderHardwareAddress.GetAddressBytes()).Replace("-", ":")));
                                listBoxParse.Items.Insert(index + 7, string.Format("{0}Sender IP Address: {1}", tab, packet.arpPacket.SenderProtocolAddress));
                                listBoxParse.Items.Insert(index + 8, string.Format("{0}Target MAC Address: {1}", tab, BitConverter.ToString(packet.arpPacket.TargetHardwareAddress.GetAddressBytes()).Replace("-", ":")));
                                listBoxParse.Items.Insert(index + 9, string.Format("{0}Target IP Address: {1}", tab, packet.arpPacket.TargetProtocolAddress));
                                break;
                        }
                        break;

                    case 4:
                        switch (packet.protocolType)
                        {
                            case ProtocolType.Tcp:
                                listBoxParse.Items.Insert(index + 1, string.Format("{0}Source Port: {1}", tab, packet.tcpPacket.SourcePort));
                                listBoxParse.Items.Insert(index + 2, string.Format("{0}Destination Port: {1}", tab, packet.tcpPacket.DestinationPort));
                                listBoxParse.Items.Insert(index + 3, string.Format("{0}Sequence Number: {1}", tab, packet.tcpPacket.SequenceNumber));
                                listBoxParse.Items.Insert(index + 4, string.Format("{0}Acknowledgment Number: {1}", tab, packet.tcpPacket.AcknowledgmentNumber));
                                listBoxParse.Items.Insert(index + 5, string.Format("{0}Header Length: {1} bytes ({2})", tab, packet.tcpPacket.HeaderSegment.Length * 4, packet.tcpPacket.HeaderSegment.Length));
                                listBoxParse.Items.Insert(index + 6, string.Format("{0}Flags: 0x{1}", tab, packet.tcpPacket.Flags.ToString("X").PadLeft(3, '0')));
                                listBoxParse.Items.Insert(index + 7, string.Format("{0}Window: {1}", tab, packet.tcpPacket.WindowSize));
                                listBoxParse.Items.Insert(index + 8, string.Format("{0}Checksum: 0x{1}", tab, packet.tcpPacket.Checksum.ToString("X").PadLeft(4, '0')));
                                listBoxParse.Items.Insert(index + 9, string.Format("{0}Urgent Pointer: {1}", tab, packet.tcpPacket.UrgentPointer));
                                listBoxParse.Items.Insert(index + 10, string.Format("{0}Options: ({1} bytes)", tab, packet.tcpPacket.Options.Length));
                                listBoxParse.Items.Insert(index + 11, string.Format("{0}TCP payload: ({1} bytes)", tab, packet.tcpPacket.PayloadData.Length));
                                break;
                            case ProtocolType.Udp:
                                listBoxParse.Items.Insert(index + 1, string.Format("{0}Source Port: {1}", tab, packet.udpPacket.SourcePort));
                                listBoxParse.Items.Insert(index + 2, string.Format("{0}Destination Port: {1}", tab, packet.udpPacket.DestinationPort));
                                listBoxParse.Items.Insert(index + 3, string.Format("{0}Length: {1}", tab, packet.udpPacket.Length));
                                listBoxParse.Items.Insert(index + 4, string.Format("{0}Checksum: 0x{1}", tab, packet.udpPacket.Checksum.ToString("X").PadLeft(4, '0')));
                                listBoxParse.Items.Insert(index + 5, string.Format("{0}UDP payload: ({1} bytes)", tab, packet.udpPacket.PayloadData.Length));
                                break;
                            case ProtocolType.Icmp:
                                break;
                            case ProtocolType.IcmpV6:
                                listBoxParse.Items.Insert(index + 1, string.Format("{0}Type: {1} ({2})", tab, packet.icmpv6Packet.Type, (int)packet.icmpv6Packet.Type));
                                listBoxParse.Items.Insert(index + 2, string.Format("{0}Code: {1}", tab, packet.icmpv6Packet.Code));
                                listBoxParse.Items.Insert(index + 3, string.Format("{0}Checksum: 0x{1}", tab, packet.icmpv6Packet.Checksum.ToString("X").PadLeft(4, '0')));
                                // Flags
                                // Target Address
                                // ICMPv6 Option
                                break;
                        }
                        break;

                    case 5:
                        break;
                }

                folding = false;

            }
            else if (s.StartsWith("∨"))
            {
                folding = true;

                listBoxParse.Items.RemoveAt(index);
                listBoxParse.Items.Insert(index, s.Replace("∨", "＞"));
                listBoxParse.SelectedIndex = index;
                string c = s.Substring(3, 1);
                int id = int.Parse(c);
                switch (id)
                {
                    case 1:
                        for (int i = 6; i > 0; i--)
                        {
                            listBoxParse.Items.RemoveAt(index + i);
                        }
                        break;
                    case 2:
                        for (int i = 3; i > 0; i--)
                        {
                            listBoxParse.Items.RemoveAt(index + i);
                        }
                        break;

                    case 3:
                        switch (packet.ethernetType)
                        {
                            case EthernetType.IPv4:
                                for (int i = 11; i > 0; i--)
                                {
                                    listBoxParse.Items.RemoveAt(index + i);
                                }
                                break;
                            case EthernetType.IPv6:
                                for (int i = 8; i > 0; i--)
                                {
                                    listBoxParse.Items.RemoveAt(index + i);
                                }
                                break;
                            case EthernetType.Arp:
                                for (int i = 9; i > 0; i--)
                                {
                                    listBoxParse.Items.RemoveAt(index + i);
                                }
                                break;
                        }
                        break;

                    case 4:
                        switch (packet.protocolType)
                        {
                            case ProtocolType.Tcp:
                                for (int i = 11; i > 0; i--)
                                {
                                    listBoxParse.Items.RemoveAt(index + i);
                                }
                                break;
                            case ProtocolType.Udp:
                                for (int i = 5; i > 0; i--)
                                {
                                    listBoxParse.Items.RemoveAt(index + i);
                                }
                                break;
                            case ProtocolType.Icmp:
                                break;
                            case ProtocolType.IcmpV6:
                                for (int i = 3; i > 0; i--)
                                {
                                    listBoxParse.Items.RemoveAt(index + i);
                                }
                                break;
                        }
                        break;

                    case 5:
                        break;
                }
                
                folding = false;
            }
        }
    }
}
