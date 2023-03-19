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
        private Dictionary<int, ParsedEthernetPacket> parsedPacketDict2;
        private ParsedPacket curPacket;
        private ParsedEthernetPacket curPacket2;
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
            parsedPacketDict2 = new Dictionary<int, ParsedEthernetPacket>();

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
                                ParsedPacket parsed = NetParser.ParsePacket(count, packet);
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
                                item.SubItems.Add(parsed.length.ToString());
                                item.SubItems.Add(parsed.info);
                                //item.SubItems.Add(parsed.hex);
                                listViewPacket.Items.Add(item);

                                //自定义解析
                                ParsedEthernetPacket parsed2 = NetParser.ParseEthernetFrame(count, packet);
                                parsedPacketDict2.Add(parsed2.frame, parsed2);
                                // TBD

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

        private void OnSelectPacketChanged()
        {
            if (listViewPacket.SelectedIndices.Count > 0)
            {
                // 更新右边
                ListViewItem item = listViewPacket.SelectedItems[0];
                int id = int.Parse(item.SubItems[0].Text);
                curPacket = parsedPacketDict[id];
                //textBoxHex.Text = curPacket.hex;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("protocol:" + curPacket.protocal);
                sb.AppendLine("TTL:" + curPacket.IPtimeToLive);
                textBoxHex.Text = sb.ToString();

                // 更新左边
                listBoxParse.Items.Clear();
                //StringBuilder sb = new StringBuilder();
                //sb.Append(string.Format("Frame {0}: {1} bytes captured ({2} bits) on interface {3}", id, curPacket.length, curPacket.length*8, device.Name));

                //listBoxParse.Items.Add(sb.ToString());

                curPacket2 = parsedPacketDict2[id];
                if (curPacket2.hex != null)
                {
                    listBoxParse.Items.Add(curPacket2.hex);
                }

                listBoxParse.Items.Add("Destination Mac:" + curPacket2.dstMac.ToString());
                listBoxParse.Items.Add("Source Mac:" + curPacket2.srcMac.ToString());
                listBoxParse.Items.Add("EtherType(HEX):" + curPacket2.etherTypeHex);
                listBoxParse.Items.Add("EtherType:" + curPacket2.etherTypeString);
                listBoxParse.Items.Add("FrameType:" + curPacket2.frameType);
                listBoxParse.Items.Add("IP(HEX):" + curPacket2.ipHex);

                ParsedIpPacket ipPacket = curPacket2.ipPacket;
                if (ipPacket != null)
                {
                    listBoxParse.Items.Add("IPVersion:" + ipPacket.version.ToString());
                    listBoxParse.Items.Add("IHL:" + ipPacket.IHL);
                    listBoxParse.Items.Add("ToS:" + ipPacket.ToS);
                    listBoxParse.Items.Add("TotalLength:" + ipPacket.totalLength);
                    listBoxParse.Items.Add("Identification:" + ipPacket.identification);
                    listBoxParse.Items.Add("Flags:" + ipPacket.flags);
                    listBoxParse.Items.Add("FragmentOffset:" + ipPacket.fragmentOffset);
                    listBoxParse.Items.Add("TTL:" + ipPacket.TTL);
                    listBoxParse.Items.Add("Protocal:" + ipPacket.protocol.ToString());
                    listBoxParse.Items.Add("HeaderChecksum:" + ipPacket.headerChecksum);
                    listBoxParse.Items.Add("Source IP:" + ipPacket.srcIp.ToString());
                    listBoxParse.Items.Add("Destination IP:" + ipPacket.dstIp.ToString());

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
            if (listViewPacket.Items != null)
                listViewPacket.Items.Clear();
            if (parsedPacketDict != null)
                parsedPacketDict.Clear();
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
