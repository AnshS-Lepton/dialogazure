using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
  public  class BLCustomerDistance
    {
        DACustomerDistance objDAMisc = new DACustomerDistance();
        public List<CustDistance> getCustomerDistanceResponse(double latitude, double longitude, string customer_refid)
        {
            return objDAMisc.getCustomerDistanceResponse(latitude, longitude, customer_refid);

        }
        public List<CustDistanceEx> getCustomerSplitterResponse(string customer_refid)
        {
            return objDAMisc.getCustomerSplitterResponse(customer_refid);

        }
    }
}
