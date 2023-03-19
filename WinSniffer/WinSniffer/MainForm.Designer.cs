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
            this.textBoxHex = new System.Windows.Forms.TextBox();
            this.listViewPacket = new System.Windows.Forms.ListView();
            this.columnNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSrc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDst = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProtocal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboBoxDeviceList = new System.Windows.Forms.ComboBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxParse = new System.Windows.Forms.ListBox();
            this.checkBoxPromiscuous = new System.Windows.Forms.CheckBox();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.checkBoxAutoScroll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelCurrentDevice
            // 
            this.labelCurrentDevice.AutoSize = true;
            this.labelCurrentDevice.Location = new System.Drawing.Point(2, 7);
            this.labelCurrentDevice.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCurrentDevice.Name = "labelCurrentDevice";
            this.labelCurrentDevice.Size = new System.Drawing.Size(53, 12);
            this.labelCurrentDevice.TabIndex = 5;
            this.labelCurrentDevice.Text = "当前设备";
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStop.Location = new System.Drawing.Point(598, 26);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(2);
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
            this.buttonStart.Location = new System.Drawing.Point(511, 26);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(83, 23);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.Text = "开始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // textBoxHex
            // 
            this.textBoxHex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxHex.Location = new System.Drawing.Point(0, 0);
            this.textBoxHex.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxHex.Multiline = true;
            this.textBoxHex.Name = "textBoxHex";
            this.textBoxHex.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxHex.Size = new System.Drawing.Size(379, 154);
            this.textBoxHex.TabIndex = 9;
            this.textBoxHex.WordWrap = false;
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
            this.columnInfo});
            this.listViewPacket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPacket.HideSelection = false;
            this.listViewPacket.Location = new System.Drawing.Point(0, 0);
            this.listViewPacket.Margin = new System.Windows.Forms.Padding(2);
            this.listViewPacket.MultiSelect = false;
            this.listViewPacket.Name = "listViewPacket";
            this.listViewPacket.Size = new System.Drawing.Size(766, 162);
            this.listViewPacket.TabIndex = 11;
            this.listViewPacket.UseCompatibleStateImageBehavior = false;
            this.listViewPacket.View = System.Windows.Forms.View.Details;
            this.listViewPacket.SelectedIndexChanged += new System.EventHandler(this.listViewPacket_SelectedIndexChanged);
            // 
            // columnNo
            // 
            this.columnNo.Text = "No.";
            this.columnNo.Width = 30;
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
            this.columnProtocal.Width = 80;
            // 
            // columnInfo
            // 
            this.columnInfo.Text = "Info";
            this.columnInfo.Width = 200;
            // 
            // comboBoxDeviceList
            // 
            this.comboBoxDeviceList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDeviceList.FormattingEnabled = true;
            this.comboBoxDeviceList.Location = new System.Drawing.Point(60, 4);
            this.comboBoxDeviceList.Name = "comboBoxDeviceList";
            this.comboBoxDeviceList.Size = new System.Drawing.Size(708, 20);
            this.comboBoxDeviceList.TabIndex = 12;
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClear.Location = new System.Drawing.Point(685, 26);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(2);
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
            this.splitContainer1.Location = new System.Drawing.Point(4, 54);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewPacket);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(766, 320);
            this.splitContainer1.SplitterDistance = 162;
            this.splitContainer1.TabIndex = 14;
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
            this.listBoxParse.Size = new System.Drawing.Size(383, 154);
            this.listBoxParse.TabIndex = 0;
            // 
            // checkBoxPromiscuous
            // 
            this.checkBoxPromiscuous.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxPromiscuous.AutoSize = true;
            this.checkBoxPromiscuous.Location = new System.Drawing.Point(434, 30);
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
            this.textBoxFilter.Location = new System.Drawing.Point(60, 28);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(290, 21);
            this.textBoxFilter.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "过滤器";
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
            this.splitContainer2.Panel2.Controls.Add(this.textBoxHex);
            this.splitContainer2.Size = new System.Drawing.Size(766, 154);
            this.splitContainer2.SplitterDistance = 383;
            this.splitContainer2.TabIndex = 0;
            // 
            // checkBoxAutoScroll
            // 
            this.checkBoxAutoScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoScroll.AutoSize = true;
            this.checkBoxAutoScroll.Location = new System.Drawing.Point(356, 30);
            this.checkBoxAutoScroll.Name = "checkBoxAutoScroll";
            this.checkBoxAutoScroll.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutoScroll.TabIndex = 18;
            this.checkBoxAutoScroll.Text = "自动滚动";
            this.checkBoxAutoScroll.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 398);
            this.Controls.Add(this.checkBoxAutoScroll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.checkBoxPromiscuous);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.comboBoxDeviceList);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.labelCurrentDevice);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "WinSniffer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelCurrentDevice;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxHex;
        private System.Windows.Forms.ListView listViewPacket;
        private System.Windows.Forms.ColumnHeader columnNo;
        private System.Windows.Forms.ColumnHeader columnTime;
        private System.Windows.Forms.ColumnHeader columnSrc;
        private System.Windows.Forms.ColumnHeader columnDst;
        private System.Windows.Forms.ColumnHeader columnProtocal;
        private System.Windows.Forms.ColumnHeader columnInfo;
        private System.Windows.Forms.ComboBox comboBoxDeviceList;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBoxParse;
        private System.Windows.Forms.CheckBox checkBoxPromiscuous;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckBox checkBoxAutoScroll;
    }
}

