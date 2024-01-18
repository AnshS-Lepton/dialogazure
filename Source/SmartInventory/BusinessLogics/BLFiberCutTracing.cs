using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLFiberCutTracing
    {
        public BLFiberCutTracing()
        {

        }
        private static BLFiberCutTracing objCutTacing = null;
        private static readonly object lockObject = new object();
        public static BLFiberCutTracing Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objCutTacing == null)
                    {
                        objCutTacing = new BLFiberCutTracing();
                    }
                }
                return objCutTacing;
            }
        }
        public List<FiberPathGeom> getTracingPath(int systemId, string entityType, int portNo, string nodeType)
        {
            return new DAFiberCutTracing().getTracingPath(systemId, entityType, portNo, nodeType);
        }
        public FiberCutDetails getFiberCutDetails(int systemId, string entityType, int portNo, double distance, string nodeType,bool isBackWordPath)
        {
            return new DAFiberCutTracing().getFiberCutDetails(systemId, entityType, portNo, distance, nodeType, isBackWordPath);
        }
        public List<FiberNodes> getFiberNodeType(int systemid, string entityType)
        {
            return new DAFiberCutTracing().getFiberNodeType(systemid, entityType);
        }
    }
}
