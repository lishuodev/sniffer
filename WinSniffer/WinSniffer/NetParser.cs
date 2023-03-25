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
                        case EthernetType.IPv4: 
                            pp.ipv4Packet = (IPv4Packet)ip;
                            pp.sourceAddress = pp.ipv4Packet.SourceAddress;
                            pp.destinationAddress = pp.ipv4Packet.DestinationAddress;
                            break;
                        case EthernetType.IPv6: 
                            pp.ipv6Packet = (IPv6Packet)ip;
                            pp.sourceAddress = pp.ipv6Packet.SourceAddress;
                            pp.destinationAddress = pp.ipv6Packet.DestinationAddress;
                            break;
                    }

                    switch (ip.Protocol)
                    {
                        case ProtocolType.Tcp:
                            TcpPacket tcp = pp.packet.Extract<TcpPacket>();
                            
                            if (tcp != null)
                            {
                                pp.protocolType = ProtocolType.Tcp;
                                pp.tcpPacket = tcp;
                                pp.sourcePort = pp.tcpPacket.SourcePort;
                                pp.destinationPort = pp.tcpPacket.DestinationPort;
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
                                pp.protocolType = ProtocolType.Udp;
                                pp.udpPacket = udp;
                                pp.sourcePort = pp.udpPacket.SourcePort;
                                pp.destinationPort = pp.udpPacket.DestinationPort;
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
                                pp.protocolType = ProtocolType.Icmp;
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
                                pp.protocolType = ProtocolType.IcmpV6;
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

        // 传输层
        public ProtocolType protocolType;      // 传输层协议
        public TcpPacket tcpPacket;
        public UdpPacket udpPacket;

        public List<Packet> payloadPackets;

        public IPAddress sourceAddress;
        public IPAddress destinationAddress;
        public ushort sourcePort;
        public ushort destinationPort;
    }
}
