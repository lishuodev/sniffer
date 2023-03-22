using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer.ProtocolAnalyzer
{
    public class TCPInfo
    {
        public ushort sourcePort;
        public ushort destinationPort;
        public uint sequenceNumber;
        public uint acknowledgementNumber;
        public int dataOffset;
        public ushort flags;
        public ushort windowSize;
        public ushort checksum;
        public int urgentPointer;
        public byte[] options;
        public byte[] payload;
    }

    public static class TCPAnalyzer
    {
        public static TCPInfo Analyze(byte[] packet)
        {
            TCPInfo info = new TCPInfo();
            info.sourcePort = (ushort)(packet[0] << 8 | packet[1]);
            info.destinationPort = (ushort)(packet[2] << 8 | packet[3]);
            info.sequenceNumber = (uint)(packet[4] << 24 | packet[5] << 16 | packet[6] << 8 | packet[7]);
            info.acknowledgementNumber = (uint)(packet[8] << 24 | packet[9] << 16 | packet[10] << 8 | packet[11]);
            info.dataOffset = packet[12] >> 4;
            info.flags = (ushort)(packet[13] & 0x3f);
            info.windowSize = (ushort)(packet[14] << 8 | packet[15]);
            info.checksum = (ushort)(packet[16] << 8 | packet[17]);
            info.urgentPointer = packet[18] << 8 | packet[19];
            // option
            int optionSize = (info.dataOffset - 5) * 4;
            byte[] options = packet.Skip(20).Take(optionSize).ToArray();
            info.options = new byte[optionSize];
            Array.Copy(options, info.options, optionSize);
            // payload
            byte[] payload = packet.Skip(info.dataOffset * 4).ToArray();
            info.payload = new byte[payload.Length];
            Array.Copy(payload, info.payload, payload.Length);
            return info;
        }
    }
}
