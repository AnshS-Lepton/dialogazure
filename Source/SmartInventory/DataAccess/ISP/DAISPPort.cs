using DataAccess.DBHelpers;
using Models.ISP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ISP
{
    public class DAISPPort : Repository<ISPPortModel>
    {
        DAISPPort()
        {

        }
        private static DAISPPort instance = null;
        private static readonly object lockObject = new object();
        public static DAISPPort Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new DAISPPort();
                    }
                }
                return instance;
            }
        }

        public List<ISPPortModel> GetPortImages()
        {
            try
            {
                return repo.ExecuteProcedure<ISPPortModel>("fn_isp_get_port_images", null,true).ToList();
            }
            catch { throw; }
        }
    }
}
