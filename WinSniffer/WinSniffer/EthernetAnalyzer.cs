using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer
{
    public class EthernetInfo
    {
        public string destinationMac;
        public string sourceMac;
        public ushort etherType;
    }

    public static class EthernetAnalyzer
    {
        public static EthernetInfo Analyze(byte[] packet)
        {
            EthernetInfo info = new EthernetInfo();
            info.destinationMac = BitConverter.ToString(packet, 0, 6).Replace("-", ":");
            info.sourceMac = BitConverter.ToString(packet, 6, 6).Replace("-", ":");
            info.etherType = (ushort)((packet[12] << 8) | packet[13]);
            return info;
        }
    }
}
