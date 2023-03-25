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
using System.Runtime.InteropServices;
using WinSniffer.ProtocolAnalyzer;
using System.Windows.Forms;

namespace WinSniffer
{
    public static class NetParser
    {
        // 借助PacketDotNet库解析数据包
        //public static ParsedPacket LibraryParsePacket(int frame, RawCapture raw)
        //{
        //    ParsedPacket parsed = new ParsedPacket();
        //    parsed.frame = frame;
        //    parsed.timeval = raw.Timeval;
        //    Packet packet = Packet.ParsePacket(raw.LinkLayerType, raw.Data);
        //    parsed.length = raw.PacketLength;
        //    if (packet is EthernetPacket ethernet)
        //    {
        //        parsed.sourceMac = ethernet.SourceHardwareAddress;
        //        parsed.destinationMac = ethernet.DestinationHardwareAddress;
        //        parsed.ethernetType = ethernet.Type;

        //        switch (parsed.ethernetType)
        //        {
        //            case EthernetType.IPv4:
        //                IPv4Info ipv4Info = new IPv4Info();
        //                parsed.ipv4Info = ipv4Info;
        //                IPv4Packet ipv4 = packet.Extract<IPv4Packet>();
        //                ipv4Info.version = (int)ipv4.Version;
        //                ipv4Info.headerLength = ipv4.HeaderLength;
        //                ipv4Info.ToS = ipv4.TypeOfService;
        //                ipv4Info.totalLength = ipv4.TotalLength;
        //                ipv4Info.identification = ipv4.Id;
        //                ipv4Info.flags = ipv4.FragmentFlags;
        //                ipv4Info.fragmentOffset = ipv4.FragmentOffset;
        //                ipv4Info.TTL = ipv4.TimeToLive;
        //                ipv4Info.protocol = (byte)ipv4.Protocol;
        //                ipv4Info.headerChecksum = ipv4.Checksum;
        //                ipv4Info.sourceIP = ipv4.SourceAddress;
        //                ipv4Info.destinationIP = ipv4.DestinationAddress;
        //                break;
        //            case EthernetType.IPv6:
        //                IPv6Info ipv6Info = new IPv6Info();
        //                parsed.ipv6Info = ipv6Info;
        //                IPv6Packet ipv6 = packet.Extract<IPv6Packet>();
        //                ipv6Info.version = (int)ipv6.Version;
        //                ipv6Info.trafficClass = ipv6.TrafficClass;
        //                ipv6Info.flowLabel = ipv6.FlowLabel;
        //                ipv6Info.payloadLength = ipv6.PayloadLength;
        //                ipv6Info.nextHeader = (byte)ipv6.NextHeader;
        //                ipv6Info.hopLimit = ipv6.HopLimit;
        //                ipv6Info.sourceAddress = ipv6.SourceAddress;
        //                ipv6Info.destinationAddress = ipv6.DestinationAddress;
        //                if (ipv6.PayloadData != null)
        //                {
        //                    ipv6Info.payload = new byte[ipv6.PayloadData.Length];
        //                    Array.Copy(ipv6.PayloadData, ipv6Info.payload, ipv6.PayloadData.Length);
        //                }
        //                switch ((ProtocolType)ipv6Info.nextHeader)
        //                {
        //                    case ProtocolType.Tcp:
        //                        if (packet.PayloadPacket is TcpPacket tcp)
        //                        {
        //                            parsed.tcpInfo = new TCPInfo();
        //                            parsed.tcpInfo.sourcePort = tcp.SourcePort;
        //                            parsed.tcpInfo.destinationPort = tcp.DestinationPort;
        //                            parsed.tcpInfo.sequenceNumber = tcp.SequenceNumber;
        //                            parsed.tcpInfo.acknowledgementNumber = tcp.AcknowledgmentNumber;
        //                            parsed.tcpInfo.dataOffset = tcp.DataOffset;
        //                            parsed.tcpInfo.flags = tcp.Flags;
        //                            parsed.tcpInfo.windowSize = tcp.WindowSize;
        //                            parsed.tcpInfo.checksum = tcp.Checksum;

        //                            parsed.tcpInfo.options = new byte[tcp.Options.Length];
        //                            Array.Copy(tcp.Options, parsed.tcpInfo.options, tcp.Options.Length);
        //                            parsed.tcpInfo.payload = new byte[tcp.PayloadData.Length];
        //                            Array.Copy(tcp.PayloadData, parsed.tcpInfo.payload, tcp.PayloadData.Length);

        //                        }
        //                        break;
        //                    case ProtocolType.Udp:
        //                        if (packet.PayloadPacket is UdpPacket udp)
        //                        {
        //                            parsed.udpInfo = new UDPInfo();
        //                            parsed.udpInfo.sourcePort = udp.SourcePort;
        //                            parsed.udpInfo.destinationPort = udp.DestinationPort;
        //                            parsed.udpInfo.length = udp.Length;
        //                            parsed.udpInfo.checksum = udp.Checksum;
        //                            parsed.udpInfo.payload = new byte[udp.PayloadData.Length];
        //                            Array.Copy(udp.PayloadData, parsed.udpInfo.payload, udp.PayloadData.Length);
        //                        }
        //                        break;
        //                    case ProtocolType.IcmpV6:

        //                        break;
        //                    default:
        //                        break;
        //                }
        //                break;
        //            case EthernetType.Arp:
        //                ARPInfo arpInfo = new ARPInfo();
        //                parsed.arpInfo = arpInfo;
        //                var arp = packet.Extract<ArpPacket>();
        //                arpInfo.hardwareAddressType = (int)arp.HardwareAddressType;
        //                arpInfo.protocolAddressType = (int)arp.ProtocolAddressType;
        //                arpInfo.hardwareAddressLength = arp.HardwareAddressLength;
        //                arpInfo.protocolAddressLength = arp.ProtocolAddressLength;
        //                arpInfo.opCode = (int)arp.Operation;
        //                arpInfo.senderHardwareAddress = arp.SenderHardwareAddress;
        //                arpInfo.senderProtocolAddress = arp.SenderProtocolAddress;
        //                arpInfo.targetHardwareAddress = arp.TargetHardwareAddress;
        //                arpInfo.targetProtocolAddress = arp.TargetProtocolAddress;
        //                break;
        //            default:
        //                break;
        //        }

        //        if (EthernetType.IPv4 == parsed.ethernetType)
        //        {
        //            parsed.transportType = (ProtocolType)parsed.ipv4Info.protocol;
        //            switch (parsed.transportType)
        //            {
        //                case ProtocolType.Tcp:
        //                    TCPInfo tcpInfo = new TCPInfo();
        //                    parsed.tcpInfo = tcpInfo;
        //                    var tcp = packet.Extract<TcpPacket>();
        //                    tcpInfo.sourcePort = tcp.SourcePort;
        //                    tcpInfo.destinationPort = tcp.DestinationPort;
        //                    tcpInfo.sequenceNumber = tcp.SequenceNumber;
        //                    tcpInfo.acknowledgementNumber = tcp.AcknowledgmentNumber;
        //                    tcpInfo.dataOffset = tcp.DataOffset;
        //                    tcpInfo.flags = tcp.Flags;
        //                    tcpInfo.windowSize = tcp.WindowSize;
        //                    tcpInfo.checksum = tcp.Checksum;
        //                    tcpInfo.urgentPointer = tcp.UrgentPointer;
        //                    tcpInfo.options = new byte[tcp.Options.Length];
        //                    Array.Copy(tcp.Options, tcpInfo.options, tcp.Options.Length);
        //                    tcpInfo.payload = new byte[tcp.PayloadData.Length];
        //                    Array.Copy(tcp.PayloadData, tcpInfo.payload, tcp.PayloadData.Length);

        //                    break;
        //                case ProtocolType.Udp:
        //                    UDPInfo udpInfo = new UDPInfo();
        //                    parsed.udpInfo = udpInfo;
        //                    var udp = packet.Extract<UdpPacket>();
        //                    udpInfo.sourcePort = udp.SourcePort;
        //                    udpInfo.destinationPort = udp.DestinationPort;
        //                    udpInfo.length = udp.Length;
        //                    udpInfo.checksum = udp.Checksum;
        //                    udpInfo.payload = new byte[udp.PayloadData.Length];
        //                    Array.Copy(udp.PayloadData, udpInfo.payload, udp.PayloadData.Length);
                            
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //    }
        //    return parsed;
        //}

        public static ParsedPacket ParsePacket(int frame, RawCapture raw)
        {
            ParsedPacket pp = new ParsedPacket
            {
                frame = frame,
                timeval = raw.Timeval,
                length = raw.PacketLength,
                linkLayerType = raw.LinkLayerType,
                packet = Packet.ParsePacket(raw.LinkLayerType, raw.Data)
            };

            if (pp.packet is EthernetPacket eth)
            {
                pp.ethernetPacket = eth;
                pp.sourceMac = eth.SourceHardwareAddress;
                pp.destinationMac = eth.DestinationHardwareAddress;
                pp.ethernetType = eth.Type;

                pp.payloadPackets = new List<Packet>();
                IPPacket ip = pp.packet.Extract<IPPacket>();
                if (ip != null)
                {
                    switch (pp.ethernetType)
                    {
                        case EthernetType.IPv4: pp.ipv4Packet = (IPv4Packet)ip; break;
                        case EthernetType.IPv6: pp.ipv6Packet = (IPv6Packet)ip; break;
                    }

                    switch (ip.Protocol)
                    {
                        case ProtocolType.Tcp:
                            TcpPacket tcp = pp.packet.Extract<TcpPacket>();
                            if (tcp != null)
                            {
                                pp.transportType = ProtocolType.Tcp;
                                pp.tcpPacket = tcp;
                                if (pp.tcpPacket.HasPayloadPacket)
                                {
                                    Packet p1 = pp.tcpPacket.PayloadPacket;
                                    pp.payloadPackets.Add(p1);
                                    if (p1.HasPayloadPacket)
                                    {
                                        Packet p2 = p1.PayloadPacket;
                                        pp.payloadPackets.Add(p2);
                                        if (p2.HasPayloadPacket)
                                        {
                                            Packet p3 = p2.PayloadPacket;
                                            pp.payloadPackets.Add(p3);
                                            if (p3.HasPayloadPacket)
                                            {
                                                Packet p4 = p3.PayloadPacket;
                                                pp.payloadPackets.Add(p4);
                                                if (p4.HasPayloadPacket)
                                                {
                                                    Packet p5 = p4.PayloadPacket;
                                                    pp.payloadPackets.Add(p5);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ProtocolType.Udp:
                            UdpPacket udp = pp.packet.Extract<UdpPacket>();
                            if (udp != null)
                            {
                                pp.transportType = ProtocolType.Udp;
                                pp.udpPacket = udp;
                                if (pp.udpPacket.HasPayloadPacket)
                                {
                                    Packet p1 = pp.udpPacket.PayloadPacket;
                                    pp.payloadPackets.Add(p1);
                                    if (p1.HasPayloadPacket)
                                    {
                                        Packet p2 = p1.PayloadPacket;
                                        pp.payloadPackets.Add(p2);
                                        if (p2.HasPayloadPacket)
                                        {
                                            Packet p3 = p2.PayloadPacket;
                                            pp.payloadPackets.Add(p3);
                                            if (p3.HasPayloadPacket)
                                            {
                                                Packet p4 = p3.PayloadPacket;
                                                pp.payloadPackets.Add(p4);
                                                if (p4.HasPayloadPacket)
                                                {
                                                    Packet p5 = p4.PayloadPacket;
                                                    pp.payloadPackets.Add(p5);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ProtocolType.Icmp:
                            IcmpV4Packet icmpv4 = pp.packet.Extract<IcmpV4Packet>();
                            if (icmpv4 != null)
                            {
                                pp.transportType = ProtocolType.Icmp;
                                pp.icmpv4Packet = icmpv4;
                                if (pp.icmpv4Packet.HasPayloadPacket)
                                {
                                    Packet p1 = pp.icmpv4Packet.PayloadPacket;
                                    pp.payloadPackets.Add(p1);
                                    if (p1.HasPayloadPacket)
                                    {
                                        Packet p2 = p1.PayloadPacket;
                                        pp.payloadPackets.Add(p2);
                                        if (p2.HasPayloadPacket)
                                        {
                                            Packet p3 = p2.PayloadPacket;
                                            pp.payloadPackets.Add(p3);
                                            if (p3.HasPayloadPacket)
                                            {
                                                Packet p4 = p3.PayloadPacket;
                                                pp.payloadPackets.Add(p4);
                                                if (p4.HasPayloadPacket)
                                                {
                                                    Packet p5 = p4.PayloadPacket;
                                                    pp.payloadPackets.Add(p5);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ProtocolType.IcmpV6:
                            IcmpV6Packet icmpv6 = pp.packet.Extract<IcmpV6Packet>();
                            if (icmpv6 != null)
                            {
                                pp.transportType = ProtocolType.IcmpV6;
                                pp.icmpv6Packet = icmpv6;
                                if (pp.icmpv6Packet.HasPayloadPacket)
                                {
                                    Packet p1 = pp.icmpv6Packet.PayloadPacket;
                                    pp.payloadPackets.Add(p1);
                                    if (p1.HasPayloadPacket)
                                    {
                                        Packet p2 = p1.PayloadPacket;
                                        pp.payloadPackets.Add(p2);
                                        if (p2.HasPayloadPacket)
                                        {
                                            Packet p3 = p2.PayloadPacket;
                                            pp.payloadPackets.Add(p3);
                                            if (p3.HasPayloadPacket)
                                            {
                                                Packet p4 = p3.PayloadPacket;
                                                pp.payloadPackets.Add(p4);
                                                if (p4.HasPayloadPacket)
                                                {
                                                    Packet p5 = p4.PayloadPacket;
                                                    pp.payloadPackets.Add(p5);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    if (pp.ethernetType == EthernetType.Arp) pp.arpPacket = pp.packet.Extract<ArpPacket>();
                }
            }


            return pp;
        }

    }

    // 解析后的数据包类
    public class ParsedPacket
    {
        public int frame;                       // 帧号
        public int length;                      // 长度
        public Packet packet;                   // 原始数据包

        // 数据链路层
        public EthernetPacket ethernetPacket;   // Ethernet数据包
        public PosixTimeval timeval;            // 绝对接收时间
        public decimal time;                    // 相对接收时间
        public LinkLayers linkLayerType;        // 数据链路层协议

        // 网络层
        public IPv4Packet ipv4Packet;
        public IPv6Packet ipv6Packet;
        public ArpPacket arpPacket;
        public IcmpV4Packet icmpv4Packet;
        public IcmpV6Packet icmpv6Packet;

        public PhysicalAddress sourceMac;       // 源物理地址
        public PhysicalAddress destinationMac;  // 目标物理地址
        public EthernetType ethernetType;       // 网络层协议
        //public IPv4Info ipv4Info;
        //public IPv6Info ipv6Info;
        //public ARPInfo arpInfo;

        // 传输层
        public ProtocolType transportType;      // 传输层协议
        public TcpPacket tcpPacket;
        public UdpPacket udpPacket;
        //public TCPInfo tcpInfo;
        //public UDPInfo udpInfo;

        public List<Packet> payloadPackets;
    }
}
