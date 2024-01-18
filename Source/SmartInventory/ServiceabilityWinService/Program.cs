using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ServiceabilityWinService.Settings;

namespace ServiceabilityWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            BulkServiceabilitySrv myService = new BulkServiceabilitySrv();
            myService.OnDebug();

            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new BulkServiceabilitySrv()
            //};
            //ServiceBase.Run(ServicesToRun);

        }
    }
}
