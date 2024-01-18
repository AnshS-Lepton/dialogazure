using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationConfig
{
    public class AppConfig
    {
        public static int WebUserMaxLimit { get { return 30000; } }
        public static int MobileUserMaxLimit { get { return 30000; } }
        public static string AdminPassword { get { return "QGRtaW5pc3RyQHQwckAxMjM="; } }// base 64 encrypted
        public static string MobileResourcesKeyPassword { get { return "QGRtaW5pc3RyQHQwcgRtaW5pc3RyQHQwcgc3RyQHQwcgQwcg=="; } }
    }

}
