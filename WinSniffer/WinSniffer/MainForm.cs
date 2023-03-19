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
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketDotNet;
using SharpPcap;

namespace WinSniffer
{
    // 主窗体
    public partial class MainForm : Form
    {
        private DeviceForm deviceForm;
        private ICaptureDevice device;
        private System.Threading.Thread backgroundThread; // 监听线程
        private PacketArrivalEventHandler arrivalEventHandler; // 包到达事件处理句柄
        private CaptureStoppedEventHandler captureStoppedEventHandler; // 停止事件处理句柄
        private object queueLock = new object(); // 队列锁
        private List<RawCapture> packetQueue = new List<RawCapture>(); // 包队列
        private bool backgroundThreadStop = false;
        private int deviceId;
        private Queue<PacketWrapper> packetStrings;
        
        private int count;
        private PosixTimeval startTime;
        private PacketWrapper rawPacket;

        private Dictionary<int, ParsedPacket> parsedPacketDict;
        private ParsedPacket curPacket;
        private const int threadDelay = 100;

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

            foreach (var dev in CaptureDeviceList.Instance)
            {
                var str = String.Format("{0} {1}", dev.Name, dev.Description);
                comboBoxDeviceList.Items.Add(str);
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
                backgroundThreadStop = true;
                backgroundThread.Join();
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

            count = 0;
            startTime = new PosixTimeval(DateTime.Now);
            packetStrings = new Queue<PacketWrapper>();
            parsedPacketDict = new Dictionary<int, ParsedPacket>();

            backgroundThreadStop = false;
            backgroundThread = new System.Threading.Thread(BackgroundThread);
            backgroundThread.Start();

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

        // 后台进程，用于解析队列中的包并添加到listview
        private void BackgroundThread()
        {
            while (!backgroundThreadStop)
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
                    System.Threading.Thread.Sleep(threadDelay);
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

                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        if (listViewPacket != null)
                        {
                            ListViewItem item = null;
                            foreach (var packet in curQueue)
                            {
                                count++;
                                ParsedPacket parsed = ParsePacket(count, packet);
                                parsedPacketDict.Add(parsed.id, parsed);
                                item = new ListViewItem(count.ToString());
                                item.SubItems.Add(parsed.time.ToString());
                                if (parsed.protocal == "TCP" || parsed.protocal == "UDP")
                                {
                                    item.SubItems.Add(parsed.IPsrc.ToString());
                                    item.SubItems.Add(parsed.IPdst.ToString());
                                }
                                else
                                {
                                    item.SubItems.Add("");
                                    item.SubItems.Add("");
                                }
                                item.SubItems.Add(parsed.protocal);
                                item.SubItems.Add(parsed.info);
                                //item.SubItems.Add(parsed.hex);
                                listViewPacket.Items.Add(item);
                                
                            }
                            if (checkBoxAutoScroll.Checked)
                            {
                                item.EnsureVisible();
                            }
                        }
                    }
                    ));
                }
            }
        }

        // 解析后的数据包结构体
        private class ParsedPacket
        {
            public int id;

            public string hex;
            public string eth;
            public string ip;
            public string tcp;
            public string udp;

            public decimal time;
            // ETH
            public PhysicalAddress MACsrc;
            public PhysicalAddress MACdst;
            // IP
            public IPAddress IPsrc;
            public IPAddress IPdst;
            public int IPtimeToLive;

            public string protocal;
            // TCP
            public ushort TCPportsrc;
            public ushort TCPportdst;
            public bool TCPsync;
            public bool TCPfin;
            public bool TCPack;
            public ushort TCPwindowsize;
            public uint TCPackno;
            public uint TCPseqno;
            // UDP
            public ushort UDPportsrc;
            public ushort UDPportdst;

            public string info;
        }

        // 解析数据包
        private ParsedPacket ParsePacket(int id, RawCapture raw)
        {
            ParsedPacket parsed = new ParsedPacket();
            parsed.id = id;
            parsed.time = raw.Timeval.Value - startTime.Value;
            var packet = PacketDotNet.Packet.ParsePacket(raw.LinkLayerType, raw.Data);
            parsed.hex = packet.PrintHex();
            if (packet is PacketDotNet.EthernetPacket eth)
            {
                parsed.eth = eth.ToString();
                parsed.MACsrc = eth.SourceHardwareAddress;
                parsed.MACdst = eth.DestinationHardwareAddress;

                var ip = packet.Extract<PacketDotNet.IPPacket>();
                if (ip != null)
                {
                    parsed.ip = ip.ToString();
                    parsed.IPsrc = ip.SourceAddress;
                    parsed.IPdst = ip.DestinationAddress;
                    parsed.IPtimeToLive = ip.TimeToLive;

                    var tcp = packet.Extract<PacketDotNet.TcpPacket>();
                    if (tcp != null)
                    {
                        parsed.tcp = tcp.ToString();
                        parsed.protocal = "TCP";

                        parsed.TCPportsrc = tcp.SourcePort;
                        parsed.TCPportdst = tcp.DestinationPort;
                        parsed.TCPsync = tcp.Synchronize;
                        parsed.TCPfin = tcp.Finished;
                        parsed.TCPack = tcp.Acknowledgment;
                        parsed.TCPwindowsize = tcp.WindowSize;
                        parsed.TCPackno = tcp.AcknowledgmentNumber;
                        parsed.TCPseqno = tcp.SequenceNumber;

                        StringBuilder sb = new StringBuilder();
                        sb.Append(parsed.TCPportsrc);
                        sb.Append(" → ");
                        sb.Append(parsed.TCPportdst);
                        if (parsed.TCPack)
                        {
                            sb.Append(" [ACK]");
                        }
                        sb.Append(" Seq=");
                        sb.Append(parsed.TCPseqno);
                        sb.Append(" Ack=");
                        sb.Append(parsed.TCPackno);
                        sb.Append(" Win=");
                        sb.Append(parsed.TCPwindowsize);
                        parsed.info = sb.ToString();
                    }

                    var udp = packet.Extract<PacketDotNet.UdpPacket>();
                    if (udp != null)
                    {
                        parsed.udp = udp.ToString();
                        parsed.protocal = "UDP";

                        parsed.UDPportsrc = udp.SourcePort;
                        parsed.UDPportdst = udp.DestinationPort;
                    }
                }

            }
            return parsed;
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

        public class PacketWrapper
        {
            public RawCapture p;
            public int No { get; private set; } // 序号
            public PosixTimeval Time { get { return p.Timeval; } } // 时间
            //public string Data { get { return BitConverter.ToString(p.Data).Replace("-"," "); } }
            public string Source { get { return ((EthernetPacket)p.GetPacket()).SourceHardwareAddress.ToString(); } }
            public string Destination { get { return ((EthernetPacket)p.GetPacket()).DestinationHardwareAddress.ToString(); } }
            public int Length { get { return p.Data.Length; } }

            public string Protocal { get { return "protocal"; } }
            public LinkLayers LinkLayerType { get { return p.LinkLayerType; } }
            public PacketWrapper(int no, RawCapture p)
            {
                this.No = no;
                this.p = p;
            }
        }

        // 手动解析
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            //if (dataGridView.SelectedCells.Count == 0)
            //    return;

            //rawPacket = (PacketWrapper)dataGridView.Rows[dataGridView.SelectedCells[0].RowIndex].DataBoundItem;
            //var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            //textBoxDetail.Text = packet.ToString()
            //

            //textBoxBinary.Text = "";
            //byte[] bytes = rawPacket.p.Data;
            //int len = bytes.Length; // 到这里是没问题的

            ////StringBuilder sb = new StringBuilder();
            //string hex = HexFromByte(bytes);
            //textBoxBinary.Text = hex;
            ////int len2 = sb.Length;
            ////string binary = sb.ToString();
            ////int len3 = binary.Length;
            ////textBoxBinary.Text += binary;
            ////string binary = curPacketWrapper.Data.Replace(" ", "");
            //textBoxBot.Text = "";
            ////textBoxBot.Text += "Source: " + binary.Substring(0, 16) + Environment.NewLine;
            ////textBoxBot.Text += "Destination: " + binary.Substring(16, 16) + Environment.NewLine;
            ////byte[] bytes = curPacketWrapper.p.Data;
            //textBoxBot.Text += "Length: " + len + Environment.NewLine;

            //Analyzed analyzed = new Analyzed();
            //analyzed.dstMac = hex.Substring(0, 12);
            //analyzed.srcMac = hex.Substring(12, 12);
            //analyzed.etherType = hex.Substring(24, 4);
            ////analyzed.version = hex.Substring(28, 1);
            ////analyzed.IHL = int.Parse(hex.Substring(28, 1));
            //analyzed.TOS = hex.Substring(29, 1);
            //analyzed.totalLength = hex.Substring(30, 2);
            //analyzed.id = hex.Substring(32, 2);
            ////analyzed.flag
            ////analyzed.fragOffset
            //analyzed.TTL = hex.Substring(36, 1);
            //analyzed.protocal = hex.Substring(37, 1);
            //analyzed.headerChecksum = hex.Substring(38, 2);
            //analyzed.srcAddr = hex.Substring(40, 4);
            //analyzed.dstAddr = hex.Substring(44, 4);
            ////analyzed.options = hex.Substring(48, 40);
            ////analyzed.data

            //textBoxBot.Text += "dstMac: " + analyzed.dstMac + Environment.NewLine;
            //textBoxBot.Text += "srcMac: " + analyzed.srcMac + Environment.NewLine;
            //textBoxBot.Text += "etherType: " + analyzed.etherType + Environment.NewLine;
            //textBoxBot.Text += "TOS: " + analyzed.TOS + Environment.NewLine;
            //textBoxBot.Text += "totalLength: " + analyzed.totalLength + Environment.NewLine;
            //textBoxBot.Text += "id: " + analyzed.id + Environment.NewLine;
            //textBoxBot.Text += "TTL: " + analyzed.TTL + Environment.NewLine;
            //textBoxBot.Text += "protocal: " + analyzed.protocal + Environment.NewLine;
            //textBoxBot.Text += "headerChecksum: " + analyzed.headerChecksum + Environment.NewLine;
            //textBoxBot.Text += "srcAddr: " + analyzed.srcAddr + Environment.NewLine;
            //textBoxBot.Text += "dstAddr " + analyzed.dstAddr + Environment.NewLine;



        }

        private void Reset()
        {
            if (listViewPacket.Items != null)
                listViewPacket.Items.Clear();
            if (parsedPacketDict != null)
                parsedPacketDict.Clear();
        }

        public struct Analyzed
        {
            public string dstMac;
            public string srcMac;
            public string etherType;
            public string version;
            public string IHL;
            public string TOS;
            public string totalLength;
            public string id;
            public string flags;
            public string fragOffset;
            public string TTL;
            public string protocal;
            public string headerChecksum;
            public string srcAddr;
            public string dstAddr;
            public string options;
            public string data;
        }

        // byte[]截取
        private string TakeBits(byte[] bits, int skip, int len)
        {
            return BitConverter.ToString(bits.Skip(skip).Take(len).ToArray()).Replace("-", " ");
        }

        // byte[]转十六进制数
        public string HexFromByte(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(string.Format("{0:X2}", bytes[i]));
            }
            return builder.ToString().Trim();
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

        private void OnSelectPacketChanged()
        {
            if (listViewPacket.SelectedIndices.Count > 0)
            {
                ListViewItem item = listViewPacket.SelectedItems[0];
                int id = int.Parse(item.SubItems[0].Text);
                curPacket = parsedPacketDict[id];
                textBoxHex.Text = curPacket.hex;
            }
        }
    }
}
