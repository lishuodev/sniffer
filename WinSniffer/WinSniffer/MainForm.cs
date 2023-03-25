using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
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
        private int selectIndex;            // 选中数据包帧号

        // 控件
        private List<CheckBox> checkBoxList;    // 监听过滤
        private List<CheckBox> checkBoxDisplayList; // 显示过滤
        private ListViewItem curItem;           // 选中列表项
        private TraceForm traceForm;            // 流追踪窗口


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

            checkBoxList = new List<CheckBox>
            {
                checkBoxPromiscuous, checkBoxIPv4, checkBoxIPv6, checkBoxICMP, checkBoxARP, checkBoxTCP, checkBoxUDP, checkBoxTLS, checkBoxHTTP,
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

                    listBoxParse.Items.Clear();
                    ParsedPacket pp = parsedPacketDict[selectIndex];

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
            StartCapture();
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

        // 列表项上弹出右键菜单
        private void ListViewPacket_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewPacket.GetItemAt(e.X, e.Y);
            if (item != null && e.Button == MouseButtons.Right)
            {
                curItem = item;
                var sub = curItem.SubItems[0];
                int frame = int.Parse(sub.Text);
                tracePacket = parsedPacketDict[frame];

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
            if (radioButtonBin.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintBin(rawDict[selectIndex]);
        }

        private void RadioButtonHex_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonHex.Checked) textBoxBinary.Text = EthernetAnalyzer.PrintHex(rawDict[selectIndex]);
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
            if (a.transportType != b.transportType) return false;
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
            if (checkBoxDisplayTCP.Checked && packet.transportType == ProtocolType.Tcp) return true;
            if (checkBoxDisplayUDP.Checked && packet.transportType == ProtocolType.Udp) return true;
            if (checkBoxDisplayARP.Checked && packet.ethernetType == EthernetType.Arp) return true;
            if (checkBoxDisplayICMP.Checked && (packet.transportType == ProtocolType.Icmp || packet.transportType == ProtocolType.IcmpV6)) return true;
            if (checkBoxDisplayTLS.Checked && packet.transportType == ProtocolType.Tcp && (packet.sourcePort == 443 || packet.destinationPort == 443)) return true;
            if (checkBoxDisplayHTTP.Checked && packet.transportType == ProtocolType.Tcp && (packet.sourcePort == 80 || packet.destinationPort == 80)) return true;

            return false;
        }

        private void CheckBoxDisplayIPv4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayIPv4.Checked)
                checkBoxDisplayAll.Checked = false;
            Rebuild();
        }

        private void CheckBoxDisplayIPv6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayIPv6.Checked)
                checkBoxDisplayAll.Checked = false;
            Rebuild();
        }

        private void CheckBoxDisplayICMP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayICMP.Checked)
                checkBoxDisplayAll.Checked = false;
            Rebuild();
        }

        private void CheckBoxDisplayARP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayARP.Checked)
                checkBoxDisplayAll.Checked = false;
            Rebuild();
        }

        private void CheckBoxDisplayTCP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayTCP.Checked)
                checkBoxDisplayAll.Checked = false;
            Rebuild();
        }

        private void CheckBoxDisplayUDP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayUDP.Checked)
                checkBoxDisplayAll.Checked = false;
            Rebuild();
        }

        private void CheckBoxDisplayTLS_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayTLS.Checked)
                checkBoxDisplayAll.Checked = false;
            Rebuild();
        }

        private void CheckBoxDisplayHTTP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayAll.Checked && !checkBoxDisplayHTTP.Checked)
                checkBoxDisplayAll.Checked = false;
            Rebuild();
        }

        private void CheckBoxDisplayAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayAll.Checked)
            {
                foreach (var checkbox in checkBoxDisplayList)
                {
                    checkbox.Checked = true;
                }
            }
            Rebuild();
        }
    }
}
