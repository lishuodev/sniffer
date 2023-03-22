using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer
{
    public class IPv6Info
    {
        public int version;
        public int trafficClass;
        public int flowLabel;
        public int payloadLength;
        public byte nextHeader;
        public int hopLimit;
        public IPAddress sourceAddress;
        public IPAddress destinationAddress;
    }

    public static class IPv6Analyzer
    {
        public static IPv6Info Analyze(byte[] packet)
        {
            if (packet.Length >= 40)
            {
                IPv6Info info = new IPv6Info();
                info.version = packet[0] >> 4;
                info.trafficClass = ((packet[0] & 0x0F) << 4) | (packet[1] >> 4);
                info.flowLabel = ((packet[1] & 0x0F) << 16) | packet[2] << 8 | packet[3];
                info.payloadLength = (packet[4] << 8) | packet[5];
                info.nextHeader = packet[6];
                info.hopLimit = packet[7];
                info.sourceAddress = new IPAddress(packet.Skip(8).Take(16).ToArray());
                info.destinationAddress = new IPAddress(packet.Skip(24).Take(16).ToArray());
                return info;
            }
            else
            {
                return null;
                // packet is too short to be an IPv6 packet
            }
        }
    }
}
