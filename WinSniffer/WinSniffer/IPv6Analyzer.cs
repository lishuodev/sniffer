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
        public byte version;
        public byte trafficClass;
        public ushort flowLabel;
        public int payloadLength;
        public byte nextHeader;
        public byte hopLimit;
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
                info. version = (byte)((packet[0] & 0xF0) >> 4); // extract the IPv6 version
                info. trafficClass = (byte)(((packet[0] & 0x0F) << 4) | ((packet[1] & 0xF0) >> 4)); // extract the traffic class
                info. flowLabel = (ushort)(((packet[1] & 0x0F) << 8) | packet[2]); // extract the flow label
                info. payloadLength = (packet[4] << 8) | packet[5]; // extract the payload length
                info. nextHeader = packet[6]; // extract the next header
                info. hopLimit = packet[7]; // extract the hop limit
                //byte[] sourceAddress = new byte[16]; // allocate buffer for the source address
                //byte[] destinationAddress = new byte[16]; // allocate buffer for the destination address
                //Array.Copy(packet, 8, sourceAddress, 0, 16); // copy the source address from bytes 8-23
                //Array.Copy(packet, 24, destinationAddress, 0, 16); // copy the destination address from bytes 24-39
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
