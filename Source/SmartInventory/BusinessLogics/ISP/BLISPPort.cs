using DataAccess.ISP;
using Models.ISP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.ISP
{
    public class BLISPPort
    {
        private static BLISPPort instance = null;
        private static readonly object lockObject = new object();
        public static BLISPPort Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new BLISPPort();
                    }
                }
                return instance;
            }
        }

       

        public List<ISPPortModel> GetPortImages()
        {
            try
            {
                return DAISPPort.Instance.GetPortImages(); 
            }
            catch { throw; }
        }
    }
}
