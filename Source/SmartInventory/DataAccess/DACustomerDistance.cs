using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

namespace DataAccess
{
    public class DACustomerDistance : Repository<DropDownMaster>
    {
        public List<CustDistance> getCustomerDistanceResponse(double latitude, double longitude, string customer_refid)
        {
            try
            {
                return repo.ExecuteProcedure<CustDistance>("fn_api_getnewcustomerdistance", new { lat = latitude, lng = longitude, refid = customer_refid });
            }
            catch { throw; }
        }
        public List<CustDistanceEx> getCustomerSplitterResponse(string customer_refid)
        {
            try
            {
                return repo.ExecuteProcedure<CustDistanceEx>("fn_api_getcustomerdistance", new { refid = customer_refid });
            }
            catch { throw; }
        }
    }
}
