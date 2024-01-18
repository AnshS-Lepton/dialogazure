using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
   public  class BLSiteCustomer
    {
        private static BLSiteCustomer objSiteCustomer = null;
        private static readonly object lockObject = new object();
        public static BLSiteCustomer Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSiteCustomer == null)
                    {
                        objSiteCustomer = new BLSiteCustomer();
                    }
                }
                return objSiteCustomer;
            }
        }

        public SiteCustomer GetSiteCustomerInfoById(int systemId)
        {
            return DASiteCustomer.Instance.GetSiteCustomerInfoById(systemId);
        } 
        public SiteCustomer SaveSiteCustomer(SiteCustomer objSiteCustomer,int userId)
        {
            return DASiteCustomer.Instance.SaveSiteCustomer(objSiteCustomer,userId);
        }
        public List<Dictionary<string, string>> GetSiteCustomerList(int siteId,string lmcType)
        {
            return DASiteCustomer.Instance.GetSiteCustomerList(siteId, lmcType);
        }
        public SiteCustomer getSiteCustomerbyId(int system_id)
        {
            return DASiteCustomer.Instance.getSiteCustomerbyId(system_id);
        }
        public SiteCustomer getSiteCustomerId(string siteCustomerId)
        {
            return DASiteCustomer.Instance.getSiteCustomerId(siteCustomerId);
        }
        public SiteCustomer getSiteCustomerPO(string PONumber)
        {
            return DASiteCustomer.Instance.getSiteCustomerPO(PONumber);
        }
        public SiteCustomer getSiteCustomerPAF(string PAFNO)
        {
            return DASiteCustomer.Instance.getSiteCustomerPAF(PAFNO);
        } 
        public SiteCustomer getSiteCustomer(int systemId)
        {
            return DASiteCustomer.Instance.getSiteCustomer(systemId);
        }




        public int deleteCustomerbyId(int systemId)
        {
            return DASiteCustomer.Instance.deleteCustomerbyId(systemId);
        }
    }
}
