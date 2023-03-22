using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer
{
    public class ARPInfo
    {
        public int hardwareAddressType;
        public int protocolAddressType;
        public int hardwareAddressLength;
        public int protocolAddressLength;
        public int opCode;
        public PhysicalAddress senderHardwareAddress;
        public IPAddress senderProtocolAddress;
        public PhysicalAddress targetHardwareAddress;
        public IPAddress targetProtocolAddress;
    }

    public static class ARPAnalyzer
    {
        public static ARPInfo Analyze(byte[] packet)
        {
            ARPInfo info = new ARPInfo();
            info.hardwareAddressType = packet[0] << 8 | packet[1];
            info.protocolAddressType = packet[2] << 8 | packet[3];
            info.hardwareAddressLength = packet[4];
            info.protocolAddressLength = packet[5];
            info.opCode = packet[6] << 8 | packet[7];
            info.senderHardwareAddress = new PhysicalAddress(packet.Skip(8).Take(6).ToArray());
            info.senderProtocolAddress = new IPAddress(packet.Skip(14).Take(4).ToArray());
            info.targetHardwareAddress = new PhysicalAddress(packet.Skip(18).Take(6).ToArray());
            info.targetProtocolAddress = new IPAddress(packet.Skip(24).Take(4).ToArray());

            return info;
        }
    }
}
