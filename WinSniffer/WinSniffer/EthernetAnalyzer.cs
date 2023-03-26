using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace WinSniffer.ProtocolAnalyzer
{
    public static class EthernetAnalyzer
    {
        public static string PrintBin(byte[] data)
        {
            string space = " ";
            string space3 = "   ";
            StringBuilder sb = new StringBuilder();
            StringBuilder asc = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                // 准备数据
                byte b = data[i];
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
            int extra = 8 - data.Length % 8;
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

        public static string PrintHex(byte[] data)
        {
            string space = " ";
            string space3 = "   ";
            StringBuilder sb = new StringBuilder();
            StringBuilder asc = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                // 准备数据
                byte b = data[i];
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
            int extra = 16 - data.Length % 16;
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

        public static string PureBin(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                // 准备数据
                byte b = data[i];

                // 二进制数
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                //sb.Append(space);

            }
            sb.AppendLine();

            return sb.ToString();
        }

        public static string PureHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                // 准备数据
                byte b = data[i];

                // 十六进制数
                sb.Append(b.ToString("X").PadLeft(2, '0'));
            }
            sb.AppendLine();

            return sb.ToString();
        }

        public static string PureASCII(byte[] data)
        {
            StringBuilder asc = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                // 准备数据
                byte b = data[i];
                asc.Append((b >= 33 && b <= 126) ? Encoding.ASCII.GetString(new byte[1] { b }) : ".");

            }
            asc.AppendLine();

            return asc.ToString();
        }
 
    }
}
