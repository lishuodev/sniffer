using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSniffer.ProtocolAnalyzer;

namespace WinSniffer
{
    public partial class TraceForm : Form
    {
        private enum Direction
        {
            both = 0,
            forward = 1,
            backward = 2,
        }

        private enum TextFormat
        {
            ASCII = 0,
            Bin = 1,
            Hex = 2,
        }
        private readonly Color masterColor = Color.DarkBlue;
        private readonly Color slaveColor = Color.DarkRed;

        private List<ParsedPacket> traceList;
        private Direction curDirection;

        private bool searchReset = false;
        private int lastSearchIndex = 0;
        private string lastSearchString = string.Empty;

        private List<int> indexList;
        private int curPos;

        private int forwardCount;
        private int backwardCount;

        public TraceForm()
        {
            InitializeComponent();
        }

        private void TraceForm_Load(object sender, EventArgs e)
        {
            UpdateDirection();
            UpdateData();
        }

        private void UpdateDirection()
        {
            traceList = DataCache.tracePacketList;

            ParsedPacket first = traceList[0];
            IPAddress sourceAddress = first.sourceAddress;
            IPAddress destinationAddress = first.destinationAddress;

            forwardCount = 0;
            backwardCount = 0;
            foreach (ParsedPacket packet in traceList)
            {
                if (packet.sourceAddress.Equals(sourceAddress))
                    forwardCount++;
                else
                    backwardCount++;
            }

            comboBoxDirection.Items.Clear();
            ComboBoxListItem item0 = new ComboBoxListItem(0, string.Format("整个对话 ({0} packets)", forwardCount + backwardCount));
            ComboBoxListItem item1 = new ComboBoxListItem(1, string.Format("{0} → {1} ({2} packets)", sourceAddress.ToString(), destinationAddress.ToString(), forwardCount));
            ComboBoxListItem item2 = new ComboBoxListItem(2, string.Format("{0} → {1} ({2} packets)", destinationAddress.ToString(), sourceAddress.ToString(), backwardCount));
            comboBoxDirection.Items.Add(item0);
            comboBoxDirection.Items.Add(item1);
            comboBoxDirection.Items.Add(item2);
            comboBoxDirection.SelectedIndex = 0;    // 默认 Direction.both
        }

        private void Rebuild(TextFormat format)
        {
            richTextBoxTrace.Text = "";
            ParsedPacket first = traceList[0];
            IPAddress sourceAddress = first.sourceAddress;
            IPAddress destinationAddress = first.destinationAddress;
            searchReset = true;

            foreach (ParsedPacket packet in traceList)
            {
                if (curDirection == Direction.both)
                { }
                else if (curDirection == Direction.forward && packet.sourceAddress.Equals(sourceAddress))
                { }
                else if (curDirection == Direction.backward && packet.sourceAddress.Equals(destinationAddress))
                { }
                else continue;

                string txt = string.Empty;
                switch (format)
                {
                    case TextFormat.ASCII:
                        txt = EthernetAnalyzer.PureASCII(packet.packet.Bytes); break;
                    case TextFormat.Bin:
                        txt = EthernetAnalyzer.PureBin(packet.packet.Bytes); break;
                    case TextFormat.Hex:
                        txt = EthernetAnalyzer.PureHex(packet.packet.Bytes); break;
                }
                int startIndex = richTextBoxTrace.TextLength;
                int length = txt.Length;
                richTextBoxTrace.AppendText(txt);
                richTextBoxTrace.Select(startIndex, length);

                if (packet.sourceAddress.Equals(sourceAddress))
                {
                    richTextBoxTrace.SelectionColor = masterColor;
                }
                else
                {
                    richTextBoxTrace.SelectionColor = slaveColor;
                }
            }

            richTextBoxTrace.DeselectAll();
        }

        private void UpdateData()
        {
            switch (comboBoxFormat.SelectedIndex)
            {
                case -1:
                    comboBoxFormat.SelectedIndex = 0;
                    Rebuild(TextFormat.ASCII);
                    break;
                case 0:
                    Rebuild(TextFormat.ASCII);
                    break;
                case 1:
                    Rebuild(TextFormat.Bin);
                    break;
                case 2:
                    Rebuild(TextFormat.Hex);
                    break;
            }
        }

        private void ComboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void ButtonReturn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ComboBoxDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            curDirection = (Direction)comboBoxDirection.SelectedIndex;
            UpdateData();
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            string all = richTextBoxTrace.Text;
            string target = textBoxSearch.Text;
            if (target.Equals(string.Empty) || all.Equals(string.Empty)) return;

            if (searchReset || !target.Equals(lastSearchString))
            {
                NewSearch(all, target);
            }
            else
            {
                NextSearch(target);
            }
        }

        private void NewSearch(string all, string target)
        {
            searchReset = false;
            lastSearchIndex = 0;
            lastSearchString = target;
            curPos = -1;
            indexList = new List<int>();

            int length = target.Length;
            int index;
            do
            {
                index = all.IndexOf(target, lastSearchIndex);
                if (index != -1)
                {
                    lastSearchIndex = index + length;
                    indexList.Add(index);
                }
            }
            while (index != -1);

            NextSearch(target);
        }

        private void NextSearch(string target)
        {
            if (indexList.Count > 0)
            {
                if (curPos + 1 < indexList.Count)
                    curPos++;
                else
                    curPos = 0;

                int index = indexList[curPos];
                richTextBoxTrace.Select(index, target.Length);
                richTextBoxTrace.ScrollToCaret();
                richTextBoxTrace.Focus();
                labelIndex.Text = string.Format("当前查找位置: {0}/{1}", curPos + 1, indexList.Count);
            }
            else
            {
                richTextBoxTrace.DeselectAll();
                labelIndex.Text = "当前查找位置: 0/0";
            }
        }
    }
}
