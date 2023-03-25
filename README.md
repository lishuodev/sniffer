# WinSniffer
 
## GUI框架、语言、环境：
1. GUI框架：WinForm框架
2. 语言：C#
3. 环境：Visual Studio 2022

## 外部动态链接库
1. 网络嗅探功能：SharpPcap (基于libpcap)
2. 协议解析功能：PacketDotNet

## 主要功能：
1. 抓取网络数据包
2. 根据需求过滤网络数据包
   1. 抓包时过滤
   2. 抓包后过滤
3. 以多种格式展示数据包源码
   1. 二进制
   2. 十六进制
   3. ASCII
4. 解析网络数据包协议
   1. 数据链路层：Ethernet
   2. 网络层：IPv4, IPv6, ARP
   3. 传输层：TCP, UDP, ICMP, ICMPv6
5. 流追踪功能
   1. TCP流追踪
   2. UDP流追踪
6. 实现了类似于Wireshark的GUI图形界面