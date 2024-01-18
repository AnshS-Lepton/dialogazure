using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
   public class BLDeviceSearch
    {
        BLDeviceSearch()
        {

        }
        private static BLDeviceSearch objDeviceSearch = null;
        private static readonly object lockObject = new object();
        public static BLDeviceSearch Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDeviceSearch == null)
                    {
                        objDeviceSearch = new BLDeviceSearch();
                    }
                }
                return objDeviceSearch;
            }
        }
        public dbresponse GetNearbyDevices(DeviceSearch obj)
        {
            return DADeviceSearch.Instance.GetNearbyDevices(obj);
        }
        public dbresponse UpdatePortStatus(PortStatusUpdateInfo obj)
        {
            return DADeviceSearch.Instance.UpdatePortStatus(obj);
        }
        public dbresponse GetFiberCutDistance(FiberCutTracingInfo obj)
        {
            return DADeviceSearch.Instance.GetFiberCutDistance(obj);          
        }
    }
}
