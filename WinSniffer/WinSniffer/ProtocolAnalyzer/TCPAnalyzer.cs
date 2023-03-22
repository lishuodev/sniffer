using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer
{
    public class TCPInfo
    {
        public int sourcePort;
        public int destinationPort;
        public int sequenceNumber;
        public int acknowledgementNumber;
        public int dataOffset;
        public int flags;
        public int windowSize;
        public int checksum;
        public int urgentPointer;
        public byte[] options;
        public byte[] payload;
    }

    public static class TCPAnalyzer
    {
        public static TCPInfo Analyze(byte[] packet)
        {
            TCPInfo info = new TCPInfo();
            info.sourcePort = packet[0] << 8 | packet[1];
            info.destinationPort = packet[2] << 8 | packet[3];
            info.sequenceNumber = packet[4] << 24 | packet[5] << 16 | packet[6] << 8 | packet[7];
            info.acknowledgementNumber = packet[8] << 24 | packet[9] << 16 | packet[10] << 8 | packet[11];
            info.dataOffset = packet[12] >> 4;
            info.flags = packet[13] & 0x3f;
            info.windowSize = packet[14] << 8 | packet[15];
            info.checksum = packet[16] << 8 | packet[17];
            info.urgentPointer = packet[18] << 8 | packet[19];

            return info;
        }
    }
}
