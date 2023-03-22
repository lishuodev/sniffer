using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer
{
    public class EthernetInfo
    {
        public PhysicalAddress destinationMac;
        public PhysicalAddress sourceMac;
        public ushort etherType;
    }

    public static class EthernetAnalyzer
    {
        public static EthernetInfo Analyze(byte[] packet)
        {
            EthernetInfo info = new EthernetInfo();
            info.destinationMac = new PhysicalAddress(packet.Take(6).ToArray());
            info.sourceMac = new PhysicalAddress(packet.Skip(6).Take(6).ToArray());
            info.etherType = (ushort)((packet[12] << 8) | packet[13]);
            return info;
        }
    }
}
