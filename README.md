# WinSniffer
 
## 1. 开发环境、GUI框架、语言、IDE：
	1.1	操作系统：Windows 10
	1.2	GUI框架：WinForm
	1.3	开发语言：C#
	1.4	IDE：Visual Studio 2022
## 2.	外部动态链接库
	1.1	网络嗅探：SharpPcap (基于libpcap)  https://github.com/dotpcap/sharppcap
	1.2	协议解析：PacketDotNet	https://github.com/dotpcap/packetnet
## 3.	主要功能
	1.1	抓取局域网数据包(可开启混杂模式)
	1.2	根据需求自定义过滤
	1.3	可过滤的协议类型包括：IPv4,IPv6,ARP,ICMPv4,ICMPv6,TCP,UDP,TLS,HTTP
	1.4	抓取中过滤，适用于数据量大时减少负载
	1.5	抓取后过滤，方便用户查看
## 4.	解析网络数据包协议
	1.1	数据链路层：Ethernet
	1.2	网络层：IPv4, IPv6, ARP, ICMP, ICMPv6
	1.3	传输层：TCP, UDP
	1.4	参考了Wireshark, 对解析结果进行分层展示, 可以展开/折叠以展示详细信息
## 5.	流追踪功能
	1.1	TCP流追踪/UDP流追踪
	1.2	可以用ASCII,二进制,十六进制三种格式显示数据流
	1.3	可在追踪结果中进行查找
	1.4	可以筛选数据流方向(正向,逆向,全部)
## 6.	GUI交互界面
	1.1	基于WinForm框架实现
	1.2	参考Wireshark的交互设计
	1.3	窗口大小可调，内部控件自适应