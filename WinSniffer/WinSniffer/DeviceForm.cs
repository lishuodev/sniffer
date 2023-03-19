using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpPcap;

namespace WinSniffer
{
    // 切换设备窗体
    public partial class DeviceForm : Form
    {
        public delegate void OnItemSelectedDelegate(int id);
        public event OnItemSelectedDelegate OnItemSelected; // 选中事件的委托

        public delegate void OnCancelDelegate();
        public event OnCancelDelegate OnCancel; // 取消事件的委托

        public DeviceForm()
        {
            InitializeComponent();
        }

        private void DeviceForm_Load(object sender, EventArgs e)
        {
            foreach (var dev in CaptureDeviceList.Instance)
            {
                var str = String.Format("{0} {1}", dev.Name, dev.Description);
                listBoxDevice.Items.Add(str);
            }
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            // 若当前有设备被选中，则调用选中回调方法
            if (listBoxDevice.SelectedItem != null)
            {
                OnItemSelected(listBoxDevice.SelectedIndex);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // 取消按钮，调用取消的回调方法
            OnCancel();
        }
    }
}
