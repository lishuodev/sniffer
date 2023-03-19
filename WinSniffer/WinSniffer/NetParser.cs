using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using PacketDotNet;
using SharpPcap;
using System.Drawing.Printing;
using PacketDotNet.Lsa;

namespace WinSniffer
{
    public static class NetParser
    {
        public static Dictionary<string, string> etherTypeDict = new Dictionary<string, string>
        {
            {"0800","IPv4"},
            {"0806","ARP"},
            {"86DD","IPv6"},
        };

        // 解析tcp帧
        public static ParsedTcpPacket ParseTcpFrame(byte[] bytes)
        {
            ParsedTcpPacket packet = new ParsedTcpPacket();
            packet.bytes = bytes;
            packet.srcPort = bytes.Skip(0).Take(2).ToArray();
            packet.dstPort = bytes.Skip(2).Take(2).ToArray();

            StringBuilder sb = new StringBuilder();
            for (int i = 1; i <= packet.bytes.Length; i++)
            {
                sb.Append(packet.bytes[i - 1].ToString("X").PadLeft(2, '0'));
            }
            packet.tcpHex = sb.ToString();

            sb.Clear();
            for (int i = 1; i <= packet.srcPort.Length; i++)
            {
                sb.Append(packet.srcPort[i - 1].ToString("X").PadLeft(2, '0'));
            }
            packet.srcPortHex = sb.ToString();

            sb.Clear();
            for (int i = 1; i <= packet.dstPort.Length; i++)
            {
                sb.Append(packet.dstPort[i - 1].ToString("X").PadLeft(2, '0'));
            }
            packet.dstPortHex = sb.ToString();
            return packet;
        }

        // 解析以太网帧
        public static ParsedEthernetPacket ParseEthernetFrame(int frame, RawCapture raw)
        {
            ParsedEthernetPacket packet = new ParsedEthernetPacket();
            packet.frame = frame;
            packet.linkLayerType = raw.LinkLayerType;
            packet.timeval = raw.Timeval;
            packet.packetLength = raw.PacketLength;
            if (raw.Data != null)
            {
                packet.data = new byte[raw.Data.Length];
                Array.Copy(raw.Data, packet.data, raw.Data.Length);

                byte[] bytes = raw.Data;
                StringBuilder sb = new StringBuilder();
                string space = " ";
                for (int i = 1; i <= bytes.Length; i++)
                {
                    sb.Append(bytes[i - 1].ToString("X").PadLeft(2, '0'));
                    sb.Append(space);
                }
                packet.hex = sb.ToString();
                packet.dstMac = new PhysicalAddress(bytes.Skip(0).Take(6).ToArray());
                packet.srcMac = new PhysicalAddress(bytes.Skip(6).Take(6).ToArray());
                packet.etherType = bytes.Skip(12).Take(2).ToArray();

                sb = new StringBuilder();
                for (int i = 1; i <= packet.etherType.Length; i++)
                {
                    sb.Append(packet.etherType[i - 1].ToString("X").PadLeft(2, '0'));
                }
                packet.etherTypeHex = sb.ToString();
                packet.etherTypeString = string.Empty;
                if (packet.etherTypeHex != string.Empty)
                {
                    int value = Convert.ToInt32(packet.etherTypeHex, 16);
                    if (value >= 1536)
                    {
                        // Ethernet II frame
                        packet.frameType = "Ethernet II";
                        if (NetParser.etherTypeDict.ContainsKey(packet.etherTypeHex))
                            packet.etherTypeString = NetParser.etherTypeDict[packet.etherTypeHex];
                    }
                    else if (value <= 1500)
                    {
                        // IEEE 802.3 frame
                        packet.frameType = "IEEE 802.3";
                        packet.length = value;
                    }
                    else
                    {
                        // undefined
                    }
                }
                if (packet.etherTypeString.Equals("IPv4") || packet.etherTypeString.Equals("IPv6"))
                {
                    packet.ipBytes = bytes.Skip(14).Take(bytes.Length - 14 - 4).ToArray();
                    sb.Clear();
                    for (int i = 1; i <= packet.ipBytes.Length; i++)
                    {
                        sb.Append(packet.ipBytes[i - 1].ToString("X").PadLeft(2, '0'));
                    }
                    packet.ipHex = sb.ToString();
                    packet.ipPacket = ParseIpPacket(packet.ipBytes);
                }

            }
            return packet;
        }

        public static ParsedIpPacket ParseIpPacket(byte[] ip)
        {
            ParsedIpPacket packet = new ParsedIpPacket();
            byte b = ip.Take(1).ToArray()[0];
            int high4bit = (b & 0xf0) >> 4;
            int low4bit = (b & 0x0f);
            packet.version = (high4bit.Equals(4)) ? IPVersion.IPv4 : IPVersion.IPv6;
            packet.IHL = low4bit;
            packet.ToS = Convert.ToByte(ip.Skip(1).Take(1).ToArray()[0]);
            byte[] lengthBytes = ip.Skip(2).Take(2).ToArray();
            packet.totalLength = (lengthBytes[0] << 8 | lengthBytes[1]);
            byte[] idBytes = ip.Skip(4).Take(2).ToArray();
            packet.identification = (idBytes[0] << 8 | idBytes[1]);
            byte flagByte = ip.Skip(6).Take(1).ToArray()[0];
            int high3bit = (flagByte & 0xe0) >> 5;
            packet.flags = high3bit;
            byte[] offsetBytes = ip.Skip(6).Take(2).ToArray();
            packet.fragmentOffset = ((offsetBytes[0] << 8 | offsetBytes[1]) & 0x1fff);
            packet.TTL = ip.Skip(8).Take(1).ToArray()[0];
            packet.protocol = (ProtocolType)Convert.ToByte(ip.Skip(9).Take(1).ToArray()[0]);
            byte[] sumBytes = ip.Skip(10).Take(2).ToArray();
            packet.headerChecksum = (sumBytes[0] << 8 | sumBytes[1]);
            packet.srcIp = new IPAddress(ip.Skip(12).Take(4).ToArray());
            packet.dstIp = new IPAddress(ip.Skip(16).Take(4).ToArray());
            return packet;
        }

        // 借助库解析数据包
        public static ParsedPacket ParsePacket(int id, RawCapture raw)
        {
            ParsedPacket parsed = new ParsedPacket();
            parsed.id = id;
            var packet = PacketDotNet.Packet.ParsePacket(raw.LinkLayerType, raw.Data);
            parsed.hex = packet.PrintHex();
            parsed.length = raw.PacketLength;
            if (packet is PacketDotNet.EthernetPacket eth)
            {
                parsed.eth = eth.ToString();
                parsed.MACsrc = eth.SourceHardwareAddress;
                parsed.MACdst = eth.DestinationHardwareAddress;

                var ip = packet.Extract<PacketDotNet.IPPacket>();
                if (ip != null)
                {
                    parsed.ip = ip.ToString();
                    parsed.IPsrc = ip.SourceAddress;
                    parsed.IPdst = ip.DestinationAddress;
                    parsed.IPtimeToLive = ip.TimeToLive;

                    var tcp = packet.Extract<PacketDotNet.TcpPacket>();
                    if (tcp != null)
                    {
                        parsed.tcp = tcp.ToString();
                        parsed.protocal = "TCP";

                        parsed.TCPportsrc = tcp.SourcePort;
                        parsed.TCPportdst = tcp.DestinationPort;
                        parsed.TCPsync = tcp.Synchronize;
                        parsed.TCPfin = tcp.Finished;
                        parsed.TCPack = tcp.Acknowledgment;
                        parsed.TCPwindowsize = tcp.WindowSize;
                        parsed.TCPackno = tcp.AcknowledgmentNumber;
                        parsed.TCPseqno = tcp.SequenceNumber;

                        // info
                        //StringBuilder sb = new StringBuilder();
                        //sb.Append(parsed.TCPportsrc);
                        //sb.Append(" → ");
                        //sb.Append(parsed.TCPportdst);
                        //if (parsed.TCPack)
                        //{
                        //    sb.Append(" [ACK]");
                        //}
                        //sb.Append(" Seq=");
                        //sb.Append(parsed.TCPseqno);
                        //sb.Append(" Ack=");
                        //sb.Append(parsed.TCPackno);
                        //sb.Append(" Win=");
                        //sb.Append(parsed.TCPwindowsize);
                        //parsed.info = sb.ToString();
                    }

                    var udp = packet.Extract<PacketDotNet.UdpPacket>();
                    if (udp != null)
                    {
                        parsed.udp = udp.ToString();
                        parsed.protocal = "UDP";

                        parsed.UDPportsrc = udp.SourcePort;
                        parsed.UDPportdst = udp.DestinationPort;
                    }
                }

            }
            return parsed;
        }

        // byte[]转十六进制数
        public static string HexFromByte(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(string.Format("{0:X2}", bytes[i]));
            }
            return builder.ToString().Trim();
        }
    }

    // 以太网帧
    public class ParsedEthernetPacket
    {
        public int frame;
        public LinkLayers linkLayerType;
        public PosixTimeval timeval;
        public int packetLength;
        public byte[] data;
        public string hex;

        public PhysicalAddress dstMac;
        public PhysicalAddress srcMac;
        public byte[] etherType;
        public string etherTypeHex;
        public string etherTypeString;
        public int length;
        public string frameType;
        public byte[] ipBytes;
        public string ipHex;
        public ParsedIpPacket ipPacket;
    }

    // IP帧
    public class ParsedIpPacket
    {
        public IPVersion version;       // 4bit 4表示IPv4 6表示IPv6
        public int IHL;                 // 4bit Internet Header Length (*32bit) 取值范围5-15 表示20-60 bytes
        public byte ToS;                // 8bit Type of Service
        public int totalLength;         // 16bit 取值范围20-65535 bytes
        public int identification;      // 16bit 用于识别一个IP数据报的多个分段
        public int flags;              // 3bit
        public int fragmentOffset;      // 13bit 表示分段相对于原IP数据报头的偏移量
        public byte TTL;                // 8bit
        public ProtocolType protocol;   // 8bit
        public int headerChecksum;      // 16bit
        public IPAddress srcIp;         // 32bit
        public IPAddress dstIp;         // 32bit
        
    }

    // TCP帧
    public class ParsedTcpPacket
    {
        public byte[] bytes;
        public byte[] srcPort;
        public byte[] dstPort;
        public string tcpHex;
        public string srcPortHex;
        public string dstPortHex;

    }



    // 解析后的数据包结构体
    public class ParsedPacket
    {
        public int id;
        public int length;

        public string hex;
        public string eth;
        public string ip;
        public string tcp;
        public string udp;

        public decimal time;
        // ETH
        public PhysicalAddress MACsrc;
        public PhysicalAddress MACdst;
        // IP
        public IPAddress IPsrc;
        public IPAddress IPdst;
        public int IPtimeToLive;

        public string protocal;
        // TCP
        public ushort TCPportsrc;
        public ushort TCPportdst;
        public bool TCPsync;
        public bool TCPfin;
        public bool TCPack;
        public ushort TCPwindowsize;
        public uint TCPackno;
        public uint TCPseqno;
        // UDP
        public ushort UDPportsrc;
        public ushort UDPportdst;

        public string info;
    }

    public struct Analyzed
    {
        public string dstMac;
        public string srcMac;
        public string etherType;
        public string version;
        public string IHL;
        public string TOS;
        public string totalLength;
        public string id;
        public string flags;
        public string fragOffset;
        public string TTL;
        public string protocal;
        public string headerChecksum;
        public string srcAddr;
        public string dstAddr;
        public string options;
        public string data;
    }

    public class PacketWrapper
    {
        public RawCapture p;
        public int No { get; private set; } // 序号
        public PosixTimeval Time { get { return p.Timeval; } } // 时间
                                                               //public string Data { get { return BitConverter.ToString(p.Data).Replace("-"," "); } }
        public string Source { get { return ((EthernetPacket)p.GetPacket()).SourceHardwareAddress.ToString(); } }
        public string Destination { get { return ((EthernetPacket)p.GetPacket()).DestinationHardwareAddress.ToString(); } }
        public int Length { get { return p.Data.Length; } }

        public string Protocal { get { return "protocal"; } }
        public LinkLayers LinkLayerType { get { return p.LinkLayerType; } }
        public PacketWrapper(int no, RawCapture p)
        {
            this.No = no;
            this.p = p;
        }

    }

    public enum ProtocolType : byte
    {
        IPv6HopByHopOptions = 0,
        Icmp = 1,
        Igmp = 2,
        Ggp = 3,
        IPv4 = 4,
        St = 5,
        Tcp = 6,
        Cbt = 7,
        Egp = 8,
        Igp = 9,
        BbnRccMon = 10,
        NvpII = 11,
        Pup = 12,
        Argus = 13,
        Emcon = 14,
        Xnet = 15,
        Chaos = 16,
        Udp = 17,
        Mux = 18,
        DcnMeas = 19,
        Hmp = 20,
        Prm = 21,
        Idp = 22,
        Ttunk1 = 23,
        Trunk2 = 24,
        Leaf1 = 25,
        Leaf2 = 26,
        Rdp = 27,
        Irtp = 28,
        TP = 29,
        Netblt = 30,
        MfeNsp = 31,
        MeritInp = 32,
        Dccp = 33,
        Tpc = 34,
        Idpr = 35,
        Xtp = 36,
        Ddp = 37,
        IdprCmtp = 38,
        Tppp = 39,
        Il = 40,
        IPv6 = 41,
        Sdrp = 42,
        IPv6RoutingHeader = 43,
        IPv6FragmentHeader = 44,
        Idrp = 45,
        Rsvp = 46,
        Gre = 47,
        Dsr = 48,
        Bna = 49,
        IPSecEncapsulatingSecurityPayload = 50,
        IPSecAuthenticationHeader = 51,
        INlsp = 52,
        SwIPe = 53,
        Narp = 54,
        Mobile = 55,
        Tlsp = 56,
        Skip = 57,
        IcmpV6 = 58,
        IPv6NoNextHeader = 59,
        IPv6DestinationOptions = 60,

        Cftp = 62,

        SatExpak = 64,
        Kryptolan = 65,
        Rvd = 66,
        Ippc = 67,

        SatMon = 69,
        Visa = 70,
        Ipcu = 71,
        Cpnx = 72,
        Cphb =  73,
        Wsn = 74,
        Pvp = 75,
        BrSatMon = 76,
        SunNd = 77,
        WbMon = 78,
        WbExpak = 79,
        IsoIp = 80,
        Vmtp = 81,
        SecureVmtp = 82,
        Vines = 83,
        Ttp = 84,
        NsfnetIgp = 85,
        Dgp = 86,
        Tcf = 87,
        Eigrp = 88,
        Ospf = 89,
        SpriteRpc = 90,
        Larp = 91,
        Mtp = 92,
        Ax25 = 93,
        Os = 94,
        Micp = 95,
        SccSp = 96,
        Etherip = 97,
        Encapsulation = 98,

        Gmtp = 100,
        Ifmp = 101,
        Pnni = 102,
        Pim = 103,
        Aris = 104,
        Scps = 105,
        Qnx = 106,
        An = 107,
        CompressionHeader = 108,
        Snp = 109,
        CompaqPeer = 110,
        IPXinIP = 111,
        Vrrp = 112,
        Pgm = 113,

        L2tp = 115,
        Ddx = 116,
        Iatp = 117,
        Stp = 118,
        Srp = 119,
        Uti = 120,
        Smp = 121,
        Sm = 122,
        Ptp = 123,
        IsIsoverIPv4 = 124,
        Fire = 125,
        Crtp = 126,
        Crudp = 127,
        Sscopmce = 128,
        Iplt = 129,
        Sps = 130,
        Pipe = 131,
        Sctp = 132,
        Fc = 133,
        RsvpE2eIgnore = 134,
        MobilityHeader = 135,
        UdpLite = 136,
        MplsinIp = 137,
        Manet = 138,
        HostIdentity = 139,
        Shim6 = 140,
        Wesp = 141,
        Rohc = 142,
        Ethernet = 143,
        Reserved253 = 253,
        Reserved254 = 254,
        Raw = byte.MaxValue,
        Mask = byte.MaxValue
    }

}
