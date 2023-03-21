using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer
{
    public class IPv4Info
    {
        public int version;
        public int headerLength;
        public int ToS;
        public int totalLength;
        public int identification;
        public int flags;
        public int fragmentOffset;
        public byte TTL;
        public byte protocol;
        public int headerChecksum;
        public IPAddress sourceIP;
        public IPAddress destinationIP;
    }

    public static class IPv4Analyzer
    {

        public static IPv4Info Analyze(byte[] packet)
        {
            IPv4Info info = new IPv4Info();
            info.version = (packet[0] >> 4) & 0xF;
            info.headerLength = (packet[0] & 0xF) * 4;
            info.ToS = packet[1];
            info.totalLength = packet[2] << 8 | packet[3];
            info.identification = packet[4] << 8 | packet[5];
            info.flags = packet[6] >> 5;
            info.fragmentOffset = (packet[6] & 0x1F) << 8 | packet[7];
            info.TTL = packet[8];
            info.protocol = packet[9];
            info.headerChecksum = packet[10] << 8 | packet[11];
            info.sourceIP = new IPAddress(new byte[] { packet[12], packet[13], packet[14], packet[15] });
            info.destinationIP = new IPAddress(new byte[] { packet[16], packet[17], packet[18], packet[19] });
            return info;
        }
    }
}
