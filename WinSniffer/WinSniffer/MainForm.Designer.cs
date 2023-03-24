namespace WinSniffer
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelCurrentDevice = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.listViewPacket = new System.Windows.Forms.ListView();
            this.columnNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSrc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDst = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnEtherType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProtocol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboBoxDeviceList = new System.Windows.Forms.ComboBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.textBoxBinary = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listBoxParse = new System.Windows.Forms.ListBox();
            this.listBoxParse2 = new System.Windows.Forms.ListBox();
            this.checkBoxPromiscuous = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoScroll = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioButtonHex = new System.Windows.Forms.RadioButton();
            this.radioButtonBin = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonCancelTrace = new System.Windows.Forms.Button();
            this.textBoxTrace = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxHTTP = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxTLS = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxUDP = new System.Windows.Forms.CheckBox();
            this.checkBoxARP = new System.Windows.Forms.CheckBox();
            this.checkBoxICMP = new System.Windows.Forms.CheckBox();
            this.checkBoxTCP = new System.Windows.Forms.CheckBox();
            this.checkBoxIPv6 = new System.Windows.Forms.CheckBox();
            this.checkBoxIPv4 = new System.Windows.Forms.CheckBox();
            this.listviewMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.流追踪ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.traceTCPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.listviewMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelCurrentDevice
            // 
            this.labelCurrentDevice.AutoSize = true;
            this.labelCurrentDevice.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrentDevice.Location = new System.Drawing.Point(8, 9);
            this.labelCurrentDevice.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCurrentDevice.Name = "labelCurrentDevice";
            this.labelCurrentDevice.Size = new System.Drawing.Size(53, 12);
            this.labelCurrentDevice.TabIndex = 5;
            this.labelCurrentDevice.Text = "当前设备";
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(658, 63);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(100, 23);
            this.buttonStop.TabIndex = 6;
            this.buttonStop.Text = "停止";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(658, 36);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(100, 23);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.Text = "开始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // listViewPacket
            // 
            this.listViewPacket.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewPacket.AutoArrange = false;
            this.listViewPacket.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnNo,
            this.columnTime,
            this.columnSrc,
            this.columnDst,
            this.columnEtherType,
            this.columnLength,
            this.columnProtocol});
            this.listViewPacket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPacket.HideSelection = false;
            this.listViewPacket.Location = new System.Drawing.Point(0, 0);
            this.listViewPacket.Margin = new System.Windows.Forms.Padding(2);
            this.listViewPacket.MultiSelect = false;
            this.listViewPacket.Name = "listViewPacket";
            this.listViewPacket.Size = new System.Drawing.Size(392, 183);
            this.listViewPacket.TabIndex = 11;
            this.listViewPacket.UseCompatibleStateImageBehavior = false;
            this.listViewPacket.View = System.Windows.Forms.View.Details;
            this.listViewPacket.SelectedIndexChanged += new System.EventHandler(this.listViewPacket_SelectedIndexChanged);
            this.listViewPacket.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewPacket_MouseClick);
            // 
            // columnNo
            // 
            this.columnNo.Text = "No.";
            this.columnNo.Width = 50;
            // 
            // columnTime
            // 
            this.columnTime.Text = "Time";
            this.columnTime.Width = 100;
            // 
            // columnSrc
            // 
            this.columnSrc.Text = "Source";
            this.columnSrc.Width = 150;
            // 
            // columnDst
            // 
            this.columnDst.Text = "Destination";
            this.columnDst.Width = 150;
            // 
            // columnEtherType
            // 
            this.columnEtherType.Text = "EtherType";
            this.columnEtherType.Width = 100;
            // 
            // columnLength
            // 
            this.columnLength.Text = "Length";
            this.columnLength.Width = 100;
            // 
            // columnProtocol
            // 
            this.columnProtocol.Text = "Protocol";
            this.columnProtocol.Width = 100;
            // 
            // comboBoxDeviceList
            // 
            this.comboBoxDeviceList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDeviceList.FormattingEnabled = true;
            this.comboBoxDeviceList.Location = new System.Drawing.Point(61, 6);
            this.comboBoxDeviceList.Name = "comboBoxDeviceList";
            this.comboBoxDeviceList.Size = new System.Drawing.Size(697, 20);
            this.comboBoxDeviceList.TabIndex = 12;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(658, 90);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(100, 23);
            this.buttonClear.TabIndex = 13;
            this.buttonClear.Text = "清空";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 130);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(758, 376);
            this.splitContainer1.SplitterDistance = 183;
            this.splitContainer1.TabIndex = 14;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.listViewPacket);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.textBoxBinary);
            this.splitContainer3.Size = new System.Drawing.Size(758, 183);
            this.splitContainer3.SplitterDistance = 392;
            this.splitContainer3.SplitterWidth = 2;
            this.splitContainer3.TabIndex = 12;
            // 
            // textBoxBinary
            // 
            this.textBoxBinary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBinary.Location = new System.Drawing.Point(0, 0);
            this.textBoxBinary.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxBinary.Multiline = true;
            this.textBoxBinary.Name = "textBoxBinary";
            this.textBoxBinary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxBinary.Size = new System.Drawing.Size(364, 183);
            this.textBoxBinary.TabIndex = 0;
            this.textBoxBinary.WordWrap = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listBoxParse);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listBoxParse2);
            this.splitContainer2.Size = new System.Drawing.Size(758, 189);
            this.splitContainer2.SplitterDistance = 392;
            this.splitContainer2.TabIndex = 0;
            // 
            // listBoxParse
            // 
            this.listBoxParse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxParse.FormattingEnabled = true;
            this.listBoxParse.HorizontalScrollbar = true;
            this.listBoxParse.ItemHeight = 12;
            this.listBoxParse.Location = new System.Drawing.Point(0, 0);
            this.listBoxParse.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxParse.Name = "listBoxParse";
            this.listBoxParse.Size = new System.Drawing.Size(392, 189);
            this.listBoxParse.TabIndex = 0;
            // 
            // listBoxParse2
            // 
            this.listBoxParse2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxParse2.FormattingEnabled = true;
            this.listBoxParse2.ItemHeight = 12;
            this.listBoxParse2.Location = new System.Drawing.Point(0, 0);
            this.listBoxParse2.Name = "listBoxParse2";
            this.listBoxParse2.Size = new System.Drawing.Size(362, 189);
            this.listBoxParse2.TabIndex = 0;
            // 
            // checkBoxPromiscuous
            // 
            this.checkBoxPromiscuous.AutoSize = true;
            this.checkBoxPromiscuous.Checked = true;
            this.checkBoxPromiscuous.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPromiscuous.Location = new System.Drawing.Point(583, 40);
            this.checkBoxPromiscuous.Name = "checkBoxPromiscuous";
            this.checkBoxPromiscuous.Size = new System.Drawing.Size(72, 16);
            this.checkBoxPromiscuous.TabIndex = 15;
            this.checkBoxPromiscuous.Text = "混杂模式";
            this.checkBoxPromiscuous.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoScroll
            // 
            this.checkBoxAutoScroll.AutoSize = true;
            this.checkBoxAutoScroll.Checked = true;
            this.checkBoxAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoScroll.Location = new System.Drawing.Point(583, 67);
            this.checkBoxAutoScroll.Name = "checkBoxAutoScroll";
            this.checkBoxAutoScroll.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutoScroll.TabIndex = 18;
            this.checkBoxAutoScroll.Text = "自动滚动";
            this.checkBoxAutoScroll.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox5);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.buttonClear);
            this.panel1.Controls.Add(this.checkBoxPromiscuous);
            this.panel1.Controls.Add(this.buttonStop);
            this.panel1.Controls.Add(this.buttonStart);
            this.panel1.Controls.Add(this.checkBoxAutoScroll);
            this.panel1.Controls.Add(this.labelCurrentDevice);
            this.panel1.Controls.Add(this.comboBoxDeviceList);
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(764, 506);
            this.panel1.TabIndex = 19;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButtonHex);
            this.groupBox5.Controls.Add(this.radioButtonBin);
            this.groupBox5.Location = new System.Drawing.Point(417, 32);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(160, 41);
            this.groupBox5.TabIndex = 24;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "字节显示";
            // 
            // radioButtonHex
            // 
            this.radioButtonHex.AutoSize = true;
            this.radioButtonHex.Checked = true;
            this.radioButtonHex.Location = new System.Drawing.Point(82, 17);
            this.radioButtonHex.Name = "radioButtonHex";
            this.radioButtonHex.Size = new System.Drawing.Size(71, 16);
            this.radioButtonHex.TabIndex = 1;
            this.radioButtonHex.TabStop = true;
            this.radioButtonHex.Text = "十六进制";
            this.radioButtonHex.UseVisualStyleBackColor = true;
            this.radioButtonHex.CheckedChanged += new System.EventHandler(this.radioButtonHex_CheckedChanged);
            // 
            // radioButtonBin
            // 
            this.radioButtonBin.AutoSize = true;
            this.radioButtonBin.Location = new System.Drawing.Point(10, 17);
            this.radioButtonBin.Name = "radioButtonBin";
            this.radioButtonBin.Size = new System.Drawing.Size(59, 16);
            this.radioButtonBin.TabIndex = 0;
            this.radioButtonBin.Text = "二进制";
            this.radioButtonBin.UseVisualStyleBackColor = true;
            this.radioButtonBin.CheckedChanged += new System.EventHandler(this.radioButtonBin_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonCancelTrace);
            this.groupBox4.Controls.Add(this.textBoxTrace);
            this.groupBox4.Location = new System.Drawing.Point(3, 79);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(389, 45);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "流追踪";
            // 
            // buttonCancelTrace
            // 
            this.buttonCancelTrace.Location = new System.Drawing.Point(283, 15);
            this.buttonCancelTrace.Name = "buttonCancelTrace";
            this.buttonCancelTrace.Size = new System.Drawing.Size(100, 23);
            this.buttonCancelTrace.TabIndex = 1;
            this.buttonCancelTrace.Text = "取消";
            this.buttonCancelTrace.UseVisualStyleBackColor = true;
            this.buttonCancelTrace.Click += new System.EventHandler(this.buttonCancelTrace_Click);
            // 
            // textBoxTrace
            // 
            this.textBoxTrace.Enabled = false;
            this.textBoxTrace.Location = new System.Drawing.Point(6, 16);
            this.textBoxTrace.Name = "textBoxTrace";
            this.textBoxTrace.Size = new System.Drawing.Size(273, 21);
            this.textBoxTrace.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxHTTP);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.checkBoxTLS);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.checkBoxUDP);
            this.groupBox1.Controls.Add(this.checkBoxARP);
            this.groupBox1.Controls.Add(this.checkBoxICMP);
            this.groupBox1.Controls.Add(this.checkBoxTCP);
            this.groupBox1.Controls.Add(this.checkBoxIPv6);
            this.groupBox1.Controls.Add(this.checkBoxIPv4);
            this.groupBox1.Location = new System.Drawing.Point(3, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(408, 41);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "协议筛选";
            // 
            // checkBoxHTTP
            // 
            this.checkBoxHTTP.AutoSize = true;
            this.checkBoxHTTP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxHTTP.Location = new System.Drawing.Point(358, 17);
            this.checkBoxHTTP.Name = "checkBoxHTTP";
            this.checkBoxHTTP.Size = new System.Drawing.Size(48, 16);
            this.checkBoxHTTP.TabIndex = 1;
            this.checkBoxHTTP.Text = "HTTP";
            this.checkBoxHTTP.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(350, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(2, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "label2";
            // 
            // checkBoxTLS
            // 
            this.checkBoxTLS.AutoSize = true;
            this.checkBoxTLS.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxTLS.Location = new System.Drawing.Point(307, 17);
            this.checkBoxTLS.Name = "checkBoxTLS";
            this.checkBoxTLS.Size = new System.Drawing.Size(42, 16);
            this.checkBoxTLS.TabIndex = 3;
            this.checkBoxTLS.Text = "TLS";
            this.checkBoxTLS.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(207, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(2, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "label1";
            // 
            // checkBoxUDP
            // 
            this.checkBoxUDP.AutoSize = true;
            this.checkBoxUDP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUDP.Location = new System.Drawing.Point(259, 17);
            this.checkBoxUDP.Name = "checkBoxUDP";
            this.checkBoxUDP.Size = new System.Drawing.Size(42, 16);
            this.checkBoxUDP.TabIndex = 2;
            this.checkBoxUDP.Text = "UDP";
            this.checkBoxUDP.UseVisualStyleBackColor = false;
            // 
            // checkBoxARP
            // 
            this.checkBoxARP.AutoSize = true;
            this.checkBoxARP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxARP.Location = new System.Drawing.Point(162, 17);
            this.checkBoxARP.Name = "checkBoxARP";
            this.checkBoxARP.Size = new System.Drawing.Size(42, 16);
            this.checkBoxARP.TabIndex = 5;
            this.checkBoxARP.Text = "ARP";
            this.checkBoxARP.UseVisualStyleBackColor = false;
            // 
            // checkBoxICMP
            // 
            this.checkBoxICMP.AutoSize = true;
            this.checkBoxICMP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxICMP.Location = new System.Drawing.Point(110, 17);
            this.checkBoxICMP.Name = "checkBoxICMP";
            this.checkBoxICMP.Size = new System.Drawing.Size(48, 16);
            this.checkBoxICMP.TabIndex = 4;
            this.checkBoxICMP.Text = "ICMP";
            this.checkBoxICMP.UseVisualStyleBackColor = false;
            // 
            // checkBoxTCP
            // 
            this.checkBoxTCP.AutoSize = true;
            this.checkBoxTCP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxTCP.Location = new System.Drawing.Point(215, 17);
            this.checkBoxTCP.Name = "checkBoxTCP";
            this.checkBoxTCP.Size = new System.Drawing.Size(42, 16);
            this.checkBoxTCP.TabIndex = 1;
            this.checkBoxTCP.Text = "TCP";
            this.checkBoxTCP.UseVisualStyleBackColor = false;
            // 
            // checkBoxIPv6
            // 
            this.checkBoxIPv6.AutoSize = true;
            this.checkBoxIPv6.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxIPv6.Location = new System.Drawing.Point(58, 17);
            this.checkBoxIPv6.Name = "checkBoxIPv6";
            this.checkBoxIPv6.Size = new System.Drawing.Size(48, 16);
            this.checkBoxIPv6.TabIndex = 3;
            this.checkBoxIPv6.Text = "IPv6";
            this.checkBoxIPv6.UseVisualStyleBackColor = false;
            // 
            // checkBoxIPv4
            // 
            this.checkBoxIPv4.AutoSize = true;
            this.checkBoxIPv4.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxIPv4.Location = new System.Drawing.Point(6, 17);
            this.checkBoxIPv4.Name = "checkBoxIPv4";
            this.checkBoxIPv4.Size = new System.Drawing.Size(48, 16);
            this.checkBoxIPv4.TabIndex = 0;
            this.checkBoxIPv4.Text = "IPv4";
            this.checkBoxIPv4.UseVisualStyleBackColor = false;
            // 
            // listviewMenuStrip
            // 
            this.listviewMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.流追踪ToolStripMenuItem});
            this.listviewMenuStrip.Name = "listviewMenuStrip";
            this.listviewMenuStrip.Size = new System.Drawing.Size(113, 26);
            // 
            // 流追踪ToolStripMenuItem
            // 
            this.流追踪ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.traceTCPToolStripMenuItem});
            this.流追踪ToolStripMenuItem.Name = "流追踪ToolStripMenuItem";
            this.流追踪ToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.流追踪ToolStripMenuItem.Text = "流追踪";
            // 
            // traceTCPToolStripMenuItem
            // 
            this.traceTCPToolStripMenuItem.Name = "traceTCPToolStripMenuItem";
            this.traceTCPToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.traceTCPToolStripMenuItem.Text = "TCP流追踪";
            this.traceTCPToolStripMenuItem.Click += new System.EventHandler(this.listviewMenuStripTraceTCP_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 506);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(780, 39);
            this.Name = "MainForm";
            this.Text = "WinSniffer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.listviewMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelCurrentDevice;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.ListView listViewPacket;
        private System.Windows.Forms.ColumnHeader columnNo;
        private System.Windows.Forms.ColumnHeader columnTime;
        private System.Windows.Forms.ColumnHeader columnSrc;
        private System.Windows.Forms.ColumnHeader columnDst;
        private System.Windows.Forms.ColumnHeader columnEtherType;
        private System.Windows.Forms.ColumnHeader columnProtocol;
        private System.Windows.Forms.ComboBox comboBoxDeviceList;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBoxParse;
        private System.Windows.Forms.CheckBox checkBoxPromiscuous;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckBox checkBoxAutoScroll;
        private System.Windows.Forms.ColumnHeader columnLength;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBoxParse2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TextBox textBoxBinary;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxICMP;
        private System.Windows.Forms.CheckBox checkBoxIPv6;
        private System.Windows.Forms.CheckBox checkBoxUDP;
        private System.Windows.Forms.CheckBox checkBoxTCP;
        private System.Windows.Forms.CheckBox checkBoxIPv4;
        private System.Windows.Forms.CheckBox checkBoxTLS;
        private System.Windows.Forms.CheckBox checkBoxHTTP;
        private System.Windows.Forms.CheckBox checkBoxARP;
        private System.Windows.Forms.ContextMenuStrip listviewMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem 流追踪ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem traceTCPToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBoxTrace;
        private System.Windows.Forms.Button buttonCancelTrace;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioButtonHex;
        private System.Windows.Forms.RadioButton radioButtonBin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

