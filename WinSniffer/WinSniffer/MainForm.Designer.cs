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
            this.buttonChangeDevice = new System.Windows.Forms.Button();
            this.labelCurrentDevice = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.textBoxBinary = new System.Windows.Forms.TextBox();
            this.textBoxBot = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonChangeDevice
            // 
            this.buttonChangeDevice.Location = new System.Drawing.Point(1003, 82);
            this.buttonChangeDevice.Name = "buttonChangeDevice";
            this.buttonChangeDevice.Size = new System.Drawing.Size(166, 46);
            this.buttonChangeDevice.TabIndex = 1;
            this.buttonChangeDevice.Text = "切换设备";
            this.buttonChangeDevice.UseVisualStyleBackColor = true;
            this.buttonChangeDevice.Click += new System.EventHandler(this.buttonChangeDevice_Click);
            // 
            // labelCurrentDevice
            // 
            this.labelCurrentDevice.AutoSize = true;
            this.labelCurrentDevice.Location = new System.Drawing.Point(3, 9);
            this.labelCurrentDevice.Name = "labelCurrentDevice";
            this.labelCurrentDevice.Size = new System.Drawing.Size(70, 24);
            this.labelCurrentDevice.TabIndex = 5;
            this.labelCurrentDevice.Text = "设备:";
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(1371, 82);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(166, 46);
            this.buttonStop.TabIndex = 6;
            this.buttonStop.Text = "停止";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(1186, 82);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(166, 46);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.Text = "开始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(7, 146);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 82;
            this.dataGridView.RowTemplate.Height = 37;
            this.dataGridView.Size = new System.Drawing.Size(1530, 272);
            this.dataGridView.TabIndex = 8;
            this.dataGridView.SelectionChanged += new System.EventHandler(this.OnSelectionChanged);
            // 
            // textBoxBinary
            // 
            this.textBoxBinary.Location = new System.Drawing.Point(7, 438);
            this.textBoxBinary.Multiline = true;
            this.textBoxBinary.Name = "textBoxBinary";
            this.textBoxBinary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxBinary.Size = new System.Drawing.Size(1530, 264);
            this.textBoxBinary.TabIndex = 9;
            // 
            // textBoxBot
            // 
            this.textBoxBot.Location = new System.Drawing.Point(7, 717);
            this.textBoxBot.Multiline = true;
            this.textBoxBot.Name = "textBoxBot";
            this.textBoxBot.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxBot.Size = new System.Drawing.Size(1530, 328);
            this.textBoxBot.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1548, 1056);
            this.Controls.Add(this.textBoxBot);
            this.Controls.Add(this.textBoxBinary);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.labelCurrentDevice);
            this.Controls.Add(this.buttonChangeDevice);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonChangeDevice;
        private System.Windows.Forms.Label labelCurrentDevice;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox textBoxBinary;
        private System.Windows.Forms.TextBox textBoxBot;
    }
}

