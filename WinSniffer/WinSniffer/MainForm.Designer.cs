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
            this.columnFrame = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSrc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDst = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnEtherType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTransport = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboBoxDeviceList = new System.Windows.Forms.ComboBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.listViewTrace = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.textBoxBinary = new System.Windows.Forms.TextBox();
            this.listBoxParse = new System.Windows.Forms.ListBox();
            this.checkBoxPromiscuous = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoScroll = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBoxDisplayAll = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayHTTP = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxDisplayTLS = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxDisplayUDP = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayARP = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayICMP = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayTCP = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayIPv6 = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayIPv4 = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioButtonHex = new System.Windows.Forms.RadioButton();
            this.radioButtonBin = new System.Windows.Forms.RadioButton();
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
            this.traceUDPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            this.labelCurrentDevice.Text = "监听设备";
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(576, 78);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(40, 40);
            this.buttonStop.TabIndex = 6;
            this.buttonStop.Text = "停止";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.ButtonStop_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(532, 78);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(40, 40);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.Text = "开始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.ButtonStart_Click);
            // 
            // listViewPacket
            // 
            this.listViewPacket.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewPacket.AutoArrange = false;
            this.listViewPacket.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFrame,
            this.columnTime,
            this.columnSrc,
            this.columnDst,
            this.columnLength,
            this.columnEtherType,
            this.columnTransport,
            this.columnHeader8,
            this.columnHeader9});
            this.listViewPacket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPacket.HideSelection = false;
            this.listViewPacket.Location = new System.Drawing.Point(0, 0);
            this.listViewPacket.Margin = new System.Windows.Forms.Padding(2);
            this.listViewPacket.MultiSelect = false;
            this.listViewPacket.Name = "listViewPacket";
            this.listViewPacket.Size = new System.Drawing.Size(408, 112);
            this.listViewPacket.TabIndex = 11;
            this.listViewPacket.UseCompatibleStateImageBehavior = false;
            this.listViewPacket.View = System.Windows.Forms.View.Details;
            this.listViewPacket.SelectedIndexChanged += new System.EventHandler(this.ListViewPacket_SelectedIndexChanged);
            this.listViewPacket.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListViewPacket_MouseClick);
            // 
            // columnFrame
            // 
            this.columnFrame.Text = "No.";
            this.columnFrame.Width = 50;
            // 
            // columnTime
            // 
            this.columnTime.Text = "Time";
            this.columnTime.Width = 50;
            // 
            // columnSrc
            // 
            this.columnSrc.Text = "Source IP";
            this.columnSrc.Width = 100;
            // 
            // columnDst
            // 
            this.columnDst.Text = "Destination IP";
            this.columnDst.Width = 100;
            // 
            // columnLength
            // 
            this.columnLength.Text = "Length";
            this.columnLength.Width = 50;
            // 
            // columnEtherType
            // 
            this.columnEtherType.Text = "EthernetType";
            this.columnEtherType.Width = 100;
            // 
            // columnTransport
            // 
            this.columnTransport.Text = "TransportType";
            this.columnTransport.Width = 100;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Source Port";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Destination Port";
            // 
            // comboBoxDeviceList
            // 
            this.comboBoxDeviceList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDeviceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDeviceList.FormattingEnabled = true;
            this.comboBoxDeviceList.Location = new System.Drawing.Point(61, 6);
            this.comboBoxDeviceList.Name = "comboBoxDeviceList";
            this.comboBoxDeviceList.Size = new System.Drawing.Size(686, 20);
            this.comboBoxDeviceList.TabIndex = 12;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(620, 78);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(40, 40);
            this.buttonClear.TabIndex = 13;
            this.buttonClear.Text = "清空";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClear_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 125);
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
            this.splitContainer1.Size = new System.Drawing.Size(794, 236);
            this.splitContainer1.SplitterDistance = 112;
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
            this.splitContainer3.Panel2.Controls.Add(this.listViewTrace);
            this.splitContainer3.Size = new System.Drawing.Size(794, 112);
            this.splitContainer3.SplitterDistance = 408;
            this.splitContainer3.SplitterWidth = 2;
            this.splitContainer3.TabIndex = 12;
            // 
            // listViewTrace
            // 
            this.listViewTrace.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewTrace.AutoArrange = false;
            this.listViewTrace.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader10,
            this.columnHeader11});
            this.listViewTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTrace.HideSelection = false;
            this.listViewTrace.Location = new System.Drawing.Point(0, 0);
            this.listViewTrace.Margin = new System.Windows.Forms.Padding(2);
            this.listViewTrace.MultiSelect = false;
            this.listViewTrace.Name = "listViewTrace";
            this.listViewTrace.Size = new System.Drawing.Size(384, 112);
            this.listViewTrace.TabIndex = 12;
            this.listViewTrace.UseCompatibleStateImageBehavior = false;
            this.listViewTrace.View = System.Windows.Forms.View.Details;
            this.listViewTrace.SelectedIndexChanged += new System.EventHandler(this.ListViewTrace_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "No.";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Time";
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Source IP";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Destination IP";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Length";
            this.columnHeader5.Width = 50;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "EthernetType";
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "TransportType";
            this.columnHeader7.Width = 100;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Source Port";
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Destination Port";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.textBoxBinary);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listBoxParse);
            this.splitContainer2.Size = new System.Drawing.Size(794, 120);
            this.splitContainer2.SplitterDistance = 408;
            this.splitContainer2.TabIndex = 0;
            // 
            // textBoxBinary
            // 
            this.textBoxBinary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBinary.Location = new System.Drawing.Point(0, 0);
            this.textBoxBinary.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxBinary.Multiline = true;
            this.textBoxBinary.Name = "textBoxBinary";
            this.textBoxBinary.ReadOnly = true;
            this.textBoxBinary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxBinary.Size = new System.Drawing.Size(408, 120);
            this.textBoxBinary.TabIndex = 0;
            this.textBoxBinary.WordWrap = false;
            // 
            // listBoxParse
            // 
            this.listBoxParse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxParse.FormattingEnabled = true;
            this.listBoxParse.HorizontalScrollbar = true;
            this.listBoxParse.ItemHeight = 12;
            this.listBoxParse.Location = new System.Drawing.Point(0, 0);
            this.listBoxParse.Name = "listBoxParse";
            this.listBoxParse.Size = new System.Drawing.Size(382, 120);
            this.listBoxParse.TabIndex = 0;
            // 
            // checkBoxPromiscuous
            // 
            this.checkBoxPromiscuous.AutoSize = true;
            this.checkBoxPromiscuous.Checked = true;
            this.checkBoxPromiscuous.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPromiscuous.Location = new System.Drawing.Point(671, 78);
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
            this.checkBoxAutoScroll.Location = new System.Drawing.Point(671, 102);
            this.checkBoxAutoScroll.Name = "checkBoxAutoScroll";
            this.checkBoxAutoScroll.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutoScroll.TabIndex = 18;
            this.checkBoxAutoScroll.Text = "自动滚动";
            this.checkBoxAutoScroll.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox5);
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
            this.panel1.Size = new System.Drawing.Size(750, 361);
            this.panel1.TabIndex = 19;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.checkBoxDisplayAll);
            this.groupBox2.Controls.Add(this.checkBoxDisplayHTTP);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.checkBoxDisplayTLS);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.checkBoxDisplayUDP);
            this.groupBox2.Controls.Add(this.checkBoxDisplayARP);
            this.groupBox2.Controls.Add(this.checkBoxDisplayICMP);
            this.groupBox2.Controls.Add(this.checkBoxDisplayTCP);
            this.groupBox2.Controls.Add(this.checkBoxDisplayIPv6);
            this.groupBox2.Controls.Add(this.checkBoxDisplayIPv4);
            this.groupBox2.Location = new System.Drawing.Point(3, 78);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(467, 41);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "列表过滤";
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(406, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(2, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "label5";
            // 
            // checkBoxDisplayAll
            // 
            this.checkBoxDisplayAll.AutoSize = true;
            this.checkBoxDisplayAll.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisplayAll.Location = new System.Drawing.Point(417, 17);
            this.checkBoxDisplayAll.Name = "checkBoxDisplayAll";
            this.checkBoxDisplayAll.Size = new System.Drawing.Size(48, 16);
            this.checkBoxDisplayAll.TabIndex = 8;
            this.checkBoxDisplayAll.Text = "全选";
            this.checkBoxDisplayAll.UseVisualStyleBackColor = false;
            this.checkBoxDisplayAll.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayAll_CheckedChanged);
            // 
            // checkBoxDisplayHTTP
            // 
            this.checkBoxDisplayHTTP.AutoSize = true;
            this.checkBoxDisplayHTTP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisplayHTTP.Location = new System.Drawing.Point(358, 17);
            this.checkBoxDisplayHTTP.Name = "checkBoxDisplayHTTP";
            this.checkBoxDisplayHTTP.Size = new System.Drawing.Size(48, 16);
            this.checkBoxDisplayHTTP.TabIndex = 1;
            this.checkBoxDisplayHTTP.Text = "HTTP";
            this.checkBoxDisplayHTTP.UseVisualStyleBackColor = false;
            this.checkBoxDisplayHTTP.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayHTTP_CheckedChanged);
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(350, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(2, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "label3";
            // 
            // checkBoxDisplayTLS
            // 
            this.checkBoxDisplayTLS.AutoSize = true;
            this.checkBoxDisplayTLS.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisplayTLS.Location = new System.Drawing.Point(307, 17);
            this.checkBoxDisplayTLS.Name = "checkBoxDisplayTLS";
            this.checkBoxDisplayTLS.Size = new System.Drawing.Size(42, 16);
            this.checkBoxDisplayTLS.TabIndex = 3;
            this.checkBoxDisplayTLS.Text = "TLS";
            this.checkBoxDisplayTLS.UseVisualStyleBackColor = false;
            this.checkBoxDisplayTLS.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayTLS_CheckedChanged);
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(207, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(2, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "label4";
            // 
            // checkBoxDisplayUDP
            // 
            this.checkBoxDisplayUDP.AutoSize = true;
            this.checkBoxDisplayUDP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisplayUDP.Location = new System.Drawing.Point(259, 17);
            this.checkBoxDisplayUDP.Name = "checkBoxDisplayUDP";
            this.checkBoxDisplayUDP.Size = new System.Drawing.Size(42, 16);
            this.checkBoxDisplayUDP.TabIndex = 2;
            this.checkBoxDisplayUDP.Text = "UDP";
            this.checkBoxDisplayUDP.UseVisualStyleBackColor = false;
            this.checkBoxDisplayUDP.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayUDP_CheckedChanged);
            // 
            // checkBoxDisplayARP
            // 
            this.checkBoxDisplayARP.AutoSize = true;
            this.checkBoxDisplayARP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisplayARP.Location = new System.Drawing.Point(162, 17);
            this.checkBoxDisplayARP.Name = "checkBoxDisplayARP";
            this.checkBoxDisplayARP.Size = new System.Drawing.Size(42, 16);
            this.checkBoxDisplayARP.TabIndex = 5;
            this.checkBoxDisplayARP.Text = "ARP";
            this.checkBoxDisplayARP.UseVisualStyleBackColor = false;
            this.checkBoxDisplayARP.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayARP_CheckedChanged);
            // 
            // checkBoxDisplayICMP
            // 
            this.checkBoxDisplayICMP.AutoSize = true;
            this.checkBoxDisplayICMP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisplayICMP.Location = new System.Drawing.Point(110, 17);
            this.checkBoxDisplayICMP.Name = "checkBoxDisplayICMP";
            this.checkBoxDisplayICMP.Size = new System.Drawing.Size(48, 16);
            this.checkBoxDisplayICMP.TabIndex = 4;
            this.checkBoxDisplayICMP.Text = "ICMP";
            this.checkBoxDisplayICMP.UseVisualStyleBackColor = false;
            this.checkBoxDisplayICMP.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayICMP_CheckedChanged);
            // 
            // checkBoxDisplayTCP
            // 
            this.checkBoxDisplayTCP.AutoSize = true;
            this.checkBoxDisplayTCP.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisplayTCP.Location = new System.Drawing.Point(215, 17);
            this.checkBoxDisplayTCP.Name = "checkBoxDisplayTCP";
            this.checkBoxDisplayTCP.Size = new System.Drawing.Size(42, 16);
            this.checkBoxDisplayTCP.TabIndex = 1;
            this.checkBoxDisplayTCP.Text = "TCP";
            this.checkBoxDisplayTCP.UseVisualStyleBackColor = false;
            this.checkBoxDisplayTCP.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayTCP_CheckedChanged);
            // 
            // checkBoxDisplayIPv6
            // 
            this.checkBoxDisplayIPv6.AutoSize = true;
            this.checkBoxDisplayIPv6.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisplayIPv6.Location = new System.Drawing.Point(58, 17);
            this.checkBoxDisplayIPv6.Name = "checkBoxDisplayIPv6";
            this.checkBoxDisplayIPv6.Size = new System.Drawing.Size(48, 16);
            this.checkBoxDisplayIPv6.TabIndex = 3;
            this.checkBoxDisplayIPv6.Text = "IPv6";
            this.checkBoxDisplayIPv6.UseVisualStyleBackColor = false;
            this.checkBoxDisplayIPv6.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayIPv6_CheckedChanged);
            // 
            // checkBoxDisplayIPv4
            // 
            this.checkBoxDisplayIPv4.AutoSize = true;
            this.checkBoxDisplayIPv4.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisplayIPv4.Location = new System.Drawing.Point(6, 17);
            this.checkBoxDisplayIPv4.Name = "checkBoxDisplayIPv4";
            this.checkBoxDisplayIPv4.Size = new System.Drawing.Size(48, 16);
            this.checkBoxDisplayIPv4.TabIndex = 0;
            this.checkBoxDisplayIPv4.Text = "IPv4";
            this.checkBoxDisplayIPv4.UseVisualStyleBackColor = false;
            this.checkBoxDisplayIPv4.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayIPv4_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButtonHex);
            this.groupBox5.Controls.Add(this.radioButtonBin);
            this.groupBox5.Location = new System.Drawing.Point(487, 32);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(155, 41);
            this.groupBox5.TabIndex = 24;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "字节显示";
            // 
            // radioButtonHex
            // 
            this.radioButtonHex.AutoSize = true;
            this.radioButtonHex.Checked = true;
            this.radioButtonHex.Location = new System.Drawing.Point(10, 16);
            this.radioButtonHex.Name = "radioButtonHex";
            this.radioButtonHex.Size = new System.Drawing.Size(71, 16);
            this.radioButtonHex.TabIndex = 1;
            this.radioButtonHex.TabStop = true;
            this.radioButtonHex.Text = "十六进制";
            this.radioButtonHex.UseVisualStyleBackColor = true;
            this.radioButtonHex.CheckedChanged += new System.EventHandler(this.RadioButtonHex_CheckedChanged);
            // 
            // radioButtonBin
            // 
            this.radioButtonBin.AutoSize = true;
            this.radioButtonBin.Location = new System.Drawing.Point(87, 16);
            this.radioButtonBin.Name = "radioButtonBin";
            this.radioButtonBin.Size = new System.Drawing.Size(59, 16);
            this.radioButtonBin.TabIndex = 0;
            this.radioButtonBin.Text = "二进制";
            this.radioButtonBin.UseVisualStyleBackColor = true;
            this.radioButtonBin.CheckedChanged += new System.EventHandler(this.RadioButtonBin_CheckedChanged);
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
            this.groupBox1.Text = "监听过滤";
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
            this.traceTCPToolStripMenuItem,
            this.traceUDPToolStripMenuItem});
            this.流追踪ToolStripMenuItem.Name = "流追踪ToolStripMenuItem";
            this.流追踪ToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.流追踪ToolStripMenuItem.Text = "追踪流";
            // 
            // traceTCPToolStripMenuItem
            // 
            this.traceTCPToolStripMenuItem.Name = "traceTCPToolStripMenuItem";
            this.traceTCPToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.traceTCPToolStripMenuItem.Text = "TCP流";
            this.traceTCPToolStripMenuItem.Click += new System.EventHandler(this.TraceTCPToolStripMenuItem_Click);
            // 
            // traceUDPToolStripMenuItem
            // 
            this.traceUDPToolStripMenuItem.Name = "traceUDPToolStripMenuItem";
            this.traceUDPToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.traceUDPToolStripMenuItem.Text = "UDP流";
            this.traceUDPToolStripMenuItem.Click += new System.EventHandler(this.TraceUDPToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 361);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "MainForm";
            this.Text = "网络嗅探器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
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
        private System.Windows.Forms.ColumnHeader columnFrame;
        private System.Windows.Forms.ColumnHeader columnTime;
        private System.Windows.Forms.ColumnHeader columnSrc;
        private System.Windows.Forms.ColumnHeader columnDst;
        private System.Windows.Forms.ColumnHeader columnEtherType;
        private System.Windows.Forms.ColumnHeader columnTransport;
        private System.Windows.Forms.ComboBox comboBoxDeviceList;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox checkBoxPromiscuous;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckBox checkBoxAutoScroll;
        private System.Windows.Forms.ColumnHeader columnLength;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBoxParse;
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
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioButtonHex;
        private System.Windows.Forms.RadioButton radioButtonBin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewTrace;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ToolStripMenuItem traceUDPToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxDisplayHTTP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxDisplayTLS;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxDisplayUDP;
        private System.Windows.Forms.CheckBox checkBoxDisplayARP;
        private System.Windows.Forms.CheckBox checkBoxDisplayICMP;
        private System.Windows.Forms.CheckBox checkBoxDisplayTCP;
        private System.Windows.Forms.CheckBox checkBoxDisplayIPv6;
        private System.Windows.Forms.CheckBox checkBoxDisplayIPv4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxDisplayAll;
    }
}

