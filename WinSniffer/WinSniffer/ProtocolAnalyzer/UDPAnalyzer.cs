using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer.ProtocolAnalyzer
{
    public class UDPInfo
    {
        public ushort sourcePort;
        public ushort destinationPort;
        public int length;
        public ushort checksum;
        public byte[] payload;
    }

    public static class UDPAnalyzer
    {
        public static UDPInfo Analyze(byte[] packet)
        {
            UDPInfo info = new UDPInfo();
            info.sourcePort = (ushort)(packet[0] << 8 | packet[1]);
            info.destinationPort = (ushort)(packet[2] << 8 | packet[3]);
            info.length = packet[4] << 8 | packet[5];
            info.checksum = (ushort)(packet[6] << 8 | packet[7]);
            byte[] payload = packet.Skip(8).ToArray();
            info.payload = new byte[payload.Length];
            Array.Copy(payload, info.payload, payload.Length);
            return info;
        }
    }
}
