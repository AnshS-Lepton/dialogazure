using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Web;
using System.Linq;

namespace Utility
{
    public class IPHelper
    {
        public static string GetIPAddress()
        {
            string IP4Address = string.Empty;
            try
            {
                IPAddress[] addr = Dns.GetHostAddresses(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString());
                foreach (IPAddress IPA in addr)
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        IP4Address = IPA.ToString();
                        break;
                    }
                }
                if (IP4Address != string.Empty)
                {
                    return IP4Address;
                }
                addr = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress IPA in addr)
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        IP4Address = IPA.ToString();
                        break;
                    }
                }
            }catch { }
            return IP4Address;
        }

        public static string GetIPAddressOne()
        {
            Ping ping = new Ping();
            string hostName = Dns.GetHostName();
            var replay = ping.Send(hostName);

            if (replay.Status == IPStatus.Success)
            {
                return replay.Address.ToString();
            }
            return "";
        }

        public static string GetIPAddressTwo()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public static string GetIPAddressThree()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }

        public static string GetIPAddressFour()
        {
            string result = string.Empty;
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ip))
            {
                string[] ipRange = ip.Split(',');
                int le = ipRange.Length - 1;
                result = ipRange[0];
            }
            else
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return result;
        }

        #region(FTTH)
        public static string GetHostName()
        {
            string addr = Dns.GetHostName();
            return addr;
        }
        #endregion


        public static string GetServerIP()
        {
            string ServerIp = string.Empty;
            System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            foreach (System.Net.IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ServerIp = ip.ToString();
                    break;
                }

            }
            
            return ServerIp;
        }

    }
}
