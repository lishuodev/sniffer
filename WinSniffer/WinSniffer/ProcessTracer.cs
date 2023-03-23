using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WinSniffer
{
    public class ProcessTracer
    {
        public void Trace()
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endPoints = ipProperties.GetActiveTcpListeners();
            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation info in tcpConnections)
            {
                var localAddress = info.LocalEndPoint.Address;
                var localPort = info.LocalEndPoint.Port;
                var remoteAddress = info.RemoteEndPoint.Address;
                var remotePort = info.RemoteEndPoint.Port;
                var state = info.State;
                
            }
        }

        public void Trace2()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    //foreach (TcpConnectionInformation connection in process.)
                }
                catch (Exception)
                {

                }
            }
        }

        public void Trace3()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation c in connections)
            {
                try
                {
                    //Process process = Process.GetProcessById(c.ProcessId);
                }
                catch (ArgumentException)
                {

                }
            }
        }
    }
}
