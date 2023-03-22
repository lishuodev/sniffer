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
            this.labelCurrentDevice = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.listViewPacket = new System.Windows.Forms.ListView();
            this.columnNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSrc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDst = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProtocal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboBoxDeviceList = new System.Windows.Forms.ComboBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.textBoxBinary = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listBoxParse = new System.Windows.Forms.ListBox();
            this.listBoxParse2 = new System.Windows.Forms.ListBox();
            this.checkBoxPromiscuous = new System.Windows.Forms.CheckBox();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxAutoScroll = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.SuspendLayout();
            // 
            // labelCurrentDevice
            // 
            this.labelCurrentDevice.AutoSize = true;
            this.labelCurrentDevice.Location = new System.Drawing.Point(2, 3);
            this.labelCurrentDevice.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCurrentDevice.Name = "labelCurrentDevice";
            this.labelCurrentDevice.Size = new System.Drawing.Size(53, 12);
            this.labelCurrentDevice.TabIndex = 5;
            this.labelCurrentDevice.Text = "当前设备";
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStop.Location = new System.Drawing.Point(546, 22);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(83, 23);
            this.buttonStop.TabIndex = 6;
            this.buttonStop.Text = "停止";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Location = new System.Drawing.Point(459, 22);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(83, 23);
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
            this.columnProtocal,
            this.columnLength,
            this.columnType});
            this.listViewPacket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPacket.HideSelection = false;
            this.listViewPacket.Location = new System.Drawing.Point(0, 0);
            this.listViewPacket.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.listViewPacket.MultiSelect = false;
            this.listViewPacket.Name = "listViewPacket";
            this.listViewPacket.Size = new System.Drawing.Size(358, 195);
            this.listViewPacket.TabIndex = 11;
            this.listViewPacket.UseCompatibleStateImageBehavior = false;
            this.listViewPacket.View = System.Windows.Forms.View.Details;
            this.listViewPacket.SelectedIndexChanged += new System.EventHandler(this.listViewPacket_SelectedIndexChanged);
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
            // columnProtocal
            // 
            this.columnProtocal.Text = "Protocal";
            this.columnProtocal.Width = 150;
            // 
            // columnLength
            // 
            this.columnLength.Text = "Length";
            this.columnLength.Width = 100;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 50;
            // 
            // comboBoxDeviceList
            // 
            this.comboBoxDeviceList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDeviceList.FormattingEnabled = true;
            this.comboBoxDeviceList.Location = new System.Drawing.Point(56, 0);
            this.comboBoxDeviceList.Name = "comboBoxDeviceList";
            this.comboBoxDeviceList.Size = new System.Drawing.Size(666, 20);
            this.comboBoxDeviceList.TabIndex = 12;
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClear.Location = new System.Drawing.Point(633, 22);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(83, 23);
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
            this.splitContainer1.Location = new System.Drawing.Point(0, 51);
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
            this.splitContainer1.Size = new System.Drawing.Size(716, 386);
            this.splitContainer1.SplitterDistance = 195;
            this.splitContainer1.TabIndex = 14;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.listViewPacket);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.textBoxBinary);
            this.splitContainer3.Size = new System.Drawing.Size(716, 195);
            this.splitContainer3.SplitterDistance = 358;
            this.splitContainer3.SplitterWidth = 2;
            this.splitContainer3.TabIndex = 12;
            // 
            // textBoxBinary
            // 
            this.textBoxBinary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBinary.Location = new System.Drawing.Point(0, 0);
            this.textBoxBinary.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxBinary.Multiline = true;
            this.textBoxBinary.Name = "textBoxBinary";
            this.textBoxBinary.Size = new System.Drawing.Size(356, 195);
            this.textBoxBinary.TabIndex = 0;
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
            this.splitContainer2.Size = new System.Drawing.Size(716, 187);
            this.splitContainer2.SplitterDistance = 357;
            this.splitContainer2.TabIndex = 0;
            // 
            // listBoxParse
            // 
            this.listBoxParse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxParse.FormattingEnabled = true;
            this.listBoxParse.HorizontalScrollbar = true;
            this.listBoxParse.ItemHeight = 12;
            this.listBoxParse.Location = new System.Drawing.Point(0, 0);
            this.listBoxParse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.listBoxParse.Name = "listBoxParse";
            this.listBoxParse.Size = new System.Drawing.Size(357, 187);
            this.listBoxParse.TabIndex = 0;
            // 
            // listBoxParse2
            // 
            this.listBoxParse2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxParse2.FormattingEnabled = true;
            this.listBoxParse2.ItemHeight = 12;
            this.listBoxParse2.Location = new System.Drawing.Point(0, 0);
            this.listBoxParse2.Name = "listBoxParse2";
            this.listBoxParse2.Size = new System.Drawing.Size(355, 187);
            this.listBoxParse2.TabIndex = 0;
            // 
            // checkBoxPromiscuous
            // 
            this.checkBoxPromiscuous.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxPromiscuous.AutoSize = true;
            this.checkBoxPromiscuous.Checked = true;
            this.checkBoxPromiscuous.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPromiscuous.Location = new System.Drawing.Point(382, 26);
            this.checkBoxPromiscuous.Name = "checkBoxPromiscuous";
            this.checkBoxPromiscuous.Size = new System.Drawing.Size(72, 16);
            this.checkBoxPromiscuous.TabIndex = 15;
            this.checkBoxPromiscuous.Text = "混杂模式";
            this.checkBoxPromiscuous.UseVisualStyleBackColor = true;
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.Location = new System.Drawing.Point(56, 24);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(242, 21);
            this.textBoxFilter.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "过滤器";
            // 
            // checkBoxAutoScroll
            // 
            this.checkBoxAutoScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoScroll.AutoSize = true;
            this.checkBoxAutoScroll.Checked = true;
            this.checkBoxAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoScroll.Location = new System.Drawing.Point(304, 26);
            this.checkBoxAutoScroll.Name = "checkBoxAutoScroll";
            this.checkBoxAutoScroll.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutoScroll.TabIndex = 18;
            this.checkBoxAutoScroll.Text = "自动滚动";
            this.checkBoxAutoScroll.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonClear);
            this.panel1.Controls.Add(this.checkBoxPromiscuous);
            this.panel1.Controls.Add(this.buttonStop);
            this.panel1.Controls.Add(this.buttonStart);
            this.panel1.Controls.Add(this.checkBoxAutoScroll);
            this.panel1.Controls.Add(this.labelCurrentDevice);
            this.panel1.Controls.Add(this.textBoxFilter);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comboBoxDeviceList);
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(722, 437);
            this.panel1.TabIndex = 19;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 437);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainForm";
            this.Text = "WinSniffer";
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
        private System.Windows.Forms.ColumnHeader columnProtocal;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ComboBox comboBoxDeviceList;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBoxParse;
        private System.Windows.Forms.CheckBox checkBoxPromiscuous;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckBox checkBoxAutoScroll;
        private System.Windows.Forms.ColumnHeader columnLength;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBoxParse2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TextBox textBoxBinary;
    }
}

