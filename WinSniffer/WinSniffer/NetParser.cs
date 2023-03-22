﻿using System;
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

namespace WinSniffer
{
    public static class NetParser
    {
        // 借助库解析数据包
        public static LibraryParsedPacket LibraryParsePacket(int id, RawCapture raw)
        {
            LibraryParsedPacket parsed = new LibraryParsedPacket();
            parsed.id = id;
            parsed.timeval = raw.Timeval;
            var packet = PacketDotNet.Packet.ParsePacket(raw.LinkLayerType, raw.Data);
            parsed.hex = packet.PrintHex();
            parsed.packageLength = raw.PacketLength;
            if (packet is PacketDotNet.EthernetPacket eth)
            {
                parsed.sourceMac = eth.SourceHardwareAddress;
                parsed.destinationMac = eth.DestinationHardwareAddress;
                parsed.ethernetType = eth.Type;
                
                switch (parsed.ethernetType)
                {
                    case EthernetType.IPv4:
                        IPv4Info ipv4Info = new IPv4Info();
                        parsed.ipv4 = ipv4Info;
                        var ipv4 = packet.Extract<PacketDotNet.IPv4Packet>();
                        ipv4Info.version = (int)ipv4.Version;
                        ipv4Info.headerLength = ipv4.HeaderLength;
                        ipv4Info.ToS = ipv4.TypeOfService;
                        ipv4Info.totalLength = ipv4.TotalLength;
                        ipv4Info.identification = ipv4.Id;
                        ipv4Info.flags = ipv4.FragmentFlags;
                        ipv4Info.fragmentOffset = ipv4.FragmentOffset;
                        ipv4Info.TTL = ipv4.TimeToLive;
                        ipv4Info.protocol = (byte)ipv4.Protocol;
                        ipv4Info.headerChecksum = ipv4.Checksum;
                        ipv4Info.sourceIP = ipv4.SourceAddress;
                        ipv4Info.destinationIP = ipv4.DestinationAddress;
                        break;
                    case EthernetType.IPv6:
                        IPv6Info ipv6Info = new IPv6Info();
                        parsed.ipv6 = ipv6Info;
                        var ipv6 = packet.Extract<PacketDotNet.IPv6Packet>();
                        ipv6Info.version = (int)ipv6.Version;
                        ipv6Info.trafficClass = ipv6.TrafficClass;
                        ipv6Info.flowLabel = ipv6.FlowLabel;
                        ipv6Info.payloadLength = ipv6.PayloadLength;
                        ipv6Info.nextHeader = (byte)ipv6.NextHeader;
                        ipv6Info.hopLimit = ipv6.HopLimit;
                        ipv6Info.sourceAddress = ipv6.SourceAddress;
                        ipv6Info.destinationAddress = ipv6.DestinationAddress;
                        break;
                    case EthernetType.Arp:
                        ARPInfo arpInfo = new ARPInfo();
                        parsed.arp = arpInfo;
                        var arp = packet.Extract<PacketDotNet.ArpPacket>();
                        arpInfo.hardwareAddressType = (int)arp.HardwareAddressType;
                        arpInfo.protocolAddressType = (int)arp.ProtocolAddressType;
                        arpInfo.hardwareAddressLength = arp.HardwareAddressLength;
                        arpInfo.protocolAddressLength = arp.ProtocolAddressLength;
                        arpInfo.opCode = (int)arp.Operation;
                        arpInfo.senderHardwareAddress = arp.SenderHardwareAddress;
                        arpInfo.senderProtocolAddress = arp.SenderProtocolAddress;
                        arpInfo.targetHardwareAddress = arp.TargetHardwareAddress;
                        arpInfo.targetProtocolAddress = arp.TargetProtocolAddress;
                        break;
                    default:
                        break;
                }

                if (EthernetType.IPv4 == parsed.ethernetType)
                {
                    switch ((ProtocolType)parsed.ipv4.protocol)
                    {
                        case ProtocolType.Tcp:
                            TCPInfo tcpInfo = new TCPInfo();
                            parsed.tcp = tcpInfo;
                            var tcp = packet.Extract<PacketDotNet.TcpPacket>();
                            tcpInfo.sourcePort = tcp.SourcePort;
                            tcpInfo.destinationPort = tcp.DestinationPort;
                            tcpInfo.sequenceNumber = tcp.SequenceNumber;
                            tcpInfo.acknowledgementNumber = tcp.AcknowledgmentNumber;
                            tcpInfo.dataOffset = tcp.DataOffset;
                            tcpInfo.flags = tcp.Flags;
                            tcpInfo.windowSize = tcp.WindowSize;
                            tcpInfo.checksum = tcp.Checksum;
                            tcpInfo.urgentPointer = tcp.UrgentPointer;
                            tcpInfo.options = new byte[tcp.Options.Length];
                            Array.Copy(tcp.Options, tcpInfo.options, tcp.Options.Length);
                            tcpInfo.payload = new byte[tcp.PayloadData.Length];
                            Array.Copy(tcp.PayloadData, tcpInfo.payload, tcp.PayloadData.Length);

                            break;
                        case ProtocolType.Udp:
                            UDPInfo udpInfo = new UDPInfo();
                            parsed.udp = udpInfo;
                            var udp = packet.Extract<PacketDotNet.UdpPacket>();
                            udpInfo.sourcePort = udp.SourcePort;
                            udpInfo.destinationPort = udp.DestinationPort;
                            udpInfo.length = udp.Length;
                            udpInfo.checksum = udp.Checksum;
                            udpInfo.payload = new byte[udp.PayloadData.Length];
                            Array.Copy(udp.PayloadData, udpInfo.payload, udp.PayloadData.Length);
                            
                            break;
                        default:
                            break;
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
            return builder.ToString();
        }
    }

    // 解析后的数据包结构体
    public class LibraryParsedPacket
    {
        public int id;
        public int packageLength;

        public string hex;

        public PosixTimeval timeval;
        public decimal time; 

        // Ethernet
        public PhysicalAddress sourceMac;
        public PhysicalAddress destinationMac;
        public EthernetType ethernetType;

        public IPv4Info ipv4;
        public IPv6Info ipv6;
        public ARPInfo arp;
        public TCPInfo tcp;
        public UDPInfo udp;

    }
}
