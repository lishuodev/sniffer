using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        private BindingSource bs;
        private Queue<PacketWrapper> packetStrings;
        private int count;
        private PosixTimeval startTime;
        private PacketWrapper rawPacket;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
        }

        private void buttonChangeDevice_Click(object sender, EventArgs e)
        {
            // 打开设备窗口并注册监听
            deviceForm = new DeviceForm();
            deviceForm.OnItemSelected += OnDeviceItemSelected;
            deviceForm.OnCancel += OnCancel;
            deviceForm.Show();
        }

        private void OnDeviceItemSelected(int id)
        {
            // 选择新设备的回调方法
            deviceForm.Hide();
            deviceId = id;
            device = CaptureDeviceList.Instance[deviceId];
            // 更新界面显示
            var str = String.Format("{0} {1}", device.Name, device.Description);
            labelCurrentDevice.Text = "设备:" + str;
        }

        private void OnCancel()
        {
            // 取消选择设备的回调方法
            deviceForm.Hide();
        }

        private void StopCapture()
        {
            // 停止监听
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

        private void StartCapture()
        {
            // 开始新的监听进程
            count = 0;
            startTime = new PosixTimeval(DateTime.Now);
            bs = new BindingSource();
            dataGridView.DataSource = bs;
            packetStrings = new Queue<PacketWrapper>();

            backgroundThreadStop = false;
            backgroundThread = new System.Threading.Thread(BackgroundThread);
            backgroundThread.Start();

            arrivalEventHandler = new PacketArrivalEventHandler(OnPacketArrival);
            device.OnPacketArrival += arrivalEventHandler;
            captureStoppedEventHandler = new CaptureStoppedEventHandler(OnCaptureStopped);
            device.OnCaptureStopped += captureStoppedEventHandler;

            device.Open();
            device.StartCapture();
        }

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
                    System.Threading.Thread.Sleep(250);
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

                    var now = new PosixTimeval(DateTime.Now);

                    foreach (var packet in curQueue)
                    {
                        // 处理队列
                        count++;
                        var packetWrapper = new PacketWrapper(count, packet, now);
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            packetStrings.Enqueue(packetWrapper);
                        }));
                    }

                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        bs.DataSource = packetStrings.Reverse();
                    }
                    ));
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

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
            buttonStart.Enabled = true;
            buttonChangeDevice.Enabled = true;
            // 停止监听
            StopCapture();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            buttonChangeDevice.Enabled = false;
            // 开始监听
            StartCapture();
        }

        public class PacketWrapper
        {
            public RawCapture p;
            private PosixTimeval startTime;
            public int No { get; private set; } // 序号
            public PosixTimeval Time { get { return p.Timeval; } } // 时间
            //public string Data { get { return BitConverter.ToString(p.Data).Replace("-"," "); } }
            public string Source { get { return ((EthernetPacket)p.GetPacket()).SourceHardwareAddress.ToString(); } }
            public string Destination { get { return ((EthernetPacket)p.GetPacket()).DestinationHardwareAddress.ToString(); } }
            public int Length { get { return p.Data.Length; } }

            public string Protocal { get { return "protocal"; } }
            public LinkLayers LinkLayerType { get { return p.LinkLayerType; } }
            public PacketWrapper(int no, RawCapture p, PosixTimeval startTime)
            {
                this.No = no;
                this.p = p;
                this.startTime = startTime;
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count == 0)
                return;

            rawPacket = (PacketWrapper)dataGridView.Rows[dataGridView.SelectedCells[0].RowIndex].DataBoundItem;
            //var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            //textBoxDetail.Text = packet.ToString()
            //

            textBoxBinary.Text = "";
            byte[] bytes = rawPacket.p.Data;
            int len = bytes.Length; // 到这里是没问题的

            //StringBuilder sb = new StringBuilder();
            string hex = HexFromByte(bytes);
            textBoxBinary.Text = hex;
            //int len2 = sb.Length;
            //string binary = sb.ToString();
            //int len3 = binary.Length;
            //textBoxBinary.Text += binary;
            //string binary = curPacketWrapper.Data.Replace(" ", "");
            textBoxBot.Text = "";
            //textBoxBot.Text += "Source: " + binary.Substring(0, 16) + Environment.NewLine;
            //textBoxBot.Text += "Destination: " + binary.Substring(16, 16) + Environment.NewLine;
            //byte[] bytes = curPacketWrapper.p.Data;
            textBoxBot.Text += "Length: " + len + Environment.NewLine;

            Analyzed analyzed = new Analyzed();
            analyzed.dstMac = hex.Substring(0, 12);
            analyzed.srcMac = hex.Substring(12, 12);
            analyzed.etherType = hex.Substring(24, 4);
            //analyzed.version = hex.Substring(28, 1);
            //analyzed.IHL = int.Parse(hex.Substring(28, 1));
            analyzed.TOS = hex.Substring(29, 1);
            analyzed.totalLength = hex.Substring(30, 2);
            analyzed.id = hex.Substring(32, 2);
            //analyzed.flag
            //analyzed.fragOffset
            analyzed.TTL = hex.Substring(36, 1);
            analyzed.protocal = hex.Substring(37, 1);
            analyzed.headerChecksum = hex.Substring(38, 2);
            analyzed.srcAddr = hex.Substring(40, 4);
            analyzed.dstAddr = hex.Substring(44, 4);
            //analyzed.options = hex.Substring(48, 40);
            //analyzed.data

            textBoxBot.Text += "dstMac: " + analyzed.dstMac + Environment.NewLine;
            textBoxBot.Text += "srcMac: " + analyzed.srcMac + Environment.NewLine;
            textBoxBot.Text += "etherType: " + analyzed.etherType + Environment.NewLine;
            textBoxBot.Text += "TOS: " + analyzed.TOS + Environment.NewLine;
            textBoxBot.Text += "totalLength: " + analyzed.totalLength + Environment.NewLine;
            textBoxBot.Text += "id: " + analyzed.id + Environment.NewLine;
            textBoxBot.Text += "TTL: " + analyzed.TTL + Environment.NewLine;
            textBoxBot.Text += "protocal: " + analyzed.protocal + Environment.NewLine;
            textBoxBot.Text += "headerChecksum: " + analyzed.headerChecksum + Environment.NewLine;
            textBoxBot.Text += "srcAddr: " + analyzed.srcAddr + Environment.NewLine;
            textBoxBot.Text += "dstAddr " + analyzed.dstAddr + Environment.NewLine;



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

        private string TakeBits(byte[] bits, int skip, int len)
        {
            return BitConverter.ToString(bits.Skip(skip).Take(len).ToArray()).Replace("-", " ");
        }

        public string HexFromByte(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(string.Format("{0:X2}", bytes[i]));
            }
            return builder.ToString().Trim();
        }
    }
}
