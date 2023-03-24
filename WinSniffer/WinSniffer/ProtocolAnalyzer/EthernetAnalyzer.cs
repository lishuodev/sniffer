using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer.ProtocolAnalyzer
{
    public class EthernetInfo
    {
        public PhysicalAddress destinationMac;
        public PhysicalAddress sourceMac;
        public ushort etherType;
    }

    public static class EthernetAnalyzer
    {
        public static EthernetInfo Analyze(byte[] packet)
        {
            EthernetInfo info = new EthernetInfo();
            info.destinationMac = new PhysicalAddress(packet.Take(6).ToArray());
            info.sourceMac = new PhysicalAddress(packet.Skip(6).Take(6).ToArray());
            info.etherType = (ushort)((packet[12] << 8) | packet[13]);
            return info;
        }

        public static string PrintBin(byte[] packet)
        {
            string space = " ";
            string space3 = "   ";
            StringBuilder sb = new StringBuilder();
            StringBuilder asc = new StringBuilder();
            byte b = 0x00;

            for (int i = 0; i < packet.Length; i++)
            {
                // 准备数据
                b = packet[i];
                asc.Append((b >= 33 && b <= 126) ? Encoding.ASCII.GetString(new byte[1] { b }) : ".");

                // 段号
                if (i % 8 == 0)
                {
                    sb.Append(i.ToString("X").PadLeft(4, '0'));
                    sb.Append(space3);
                }

                // 二进制数
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                sb.Append(space);

                // ASCII
                if (i % 8 == 8 - 1)
                {
                    sb.Append(space3);
                    sb.Append(asc.ToString());
                    asc.Clear();
                    sb.AppendLine();
                }
            }
            // 最后一行补完
            int extra = 8 - packet.Length % 8;
            for (int i = 0; i < extra * 9; i++)
            {
                // 二进制制数(空)
                sb.Append(space);
                // 同一行两个byte之间间隔
                //if (i % 8 == 8 - 1) sb.Append(space3);
            }
            // ASCII
            sb.Append(space3);
            sb.Append(asc.ToString());
            asc.Clear();
            sb.AppendLine();

            return sb.ToString();
        }

        public static string PrintHex(byte[] packet)
        {
            string space = " ";
            string space3 = "   ";
            StringBuilder sb = new StringBuilder();
            StringBuilder asc = new StringBuilder();
            byte b = 0x00;
            for (int i = 0; i < packet.Length; i++)
            {
                // 准备数据
                b = packet[i];
                asc.Append((b >= 33 && b <= 126) ? Encoding.ASCII.GetString(new byte[1] { b }) : ".");
                
                // 段号
                if (i % 16 == 0)
                {
                    sb.Append(i.ToString("X").PadLeft(4, '0'));
                    sb.Append(space3);
                }

                // 十六进制数
                sb.Append(b.ToString("X").PadLeft(2, '0'));
                sb.Append(space);

                // 同一行两个byte之间间隔
                if (i % 8 == 8 - 1) sb.Append(space3);

                // ASCII
                if (i % 16 == 16 - 1)
                {
                    sb.Append(asc.ToString());
                    asc.Clear();
                    sb.AppendLine();
                }
            }
            // 最后一行补完
            int extra = 16 - packet.Length % 16;
            for (int i = 0; i < extra; i++)
            {
                // 十六进制数(空)
                sb.Append(space3);
                // 同一行两个byte之间间隔
                if (i % 8 == 8 - 1) sb.Append(space3);
            }
            // ASCII
            sb.Append(space3);
            sb.Append(asc.ToString());
            asc.Clear();
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
