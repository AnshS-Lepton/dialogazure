using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
   public class BLSiteInfo
    {
        private static BLSiteInfo objSiteInfo = null;
        private static readonly object lockObject = new object();
        public static BLSiteInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSiteInfo == null)
                    {
                        objSiteInfo = new BLSiteInfo();
                    }
                }
                return objSiteInfo;
            }
        }
        public SiteInfo SaveSiteInfo(SiteInfo objSiteInfo, int userId)
        {
            return DASiteInfo.Instance.SaveSiteInfo(objSiteInfo, userId);
        }
        public SiteInfo ValidateETPILID(string UID)
        {
            return DASiteInfo.Instance.ValidateETPILID(UID);
        }
        public SiteInfo GetSiteIfo(int systemId,string entityType)
        {
            return DASiteInfo.Instance.GetSiteInfo(systemId,entityType);
        }
        public List<SiteInfo> GetSiteVendorListbyId(string searchText)
        {
            return DASiteInfo.Instance.GetSiteVendorListbyId(searchText);
        } 
        public List<SiteInfo> GetSiteVendorList(string searchText)
        {
            return DASiteInfo.Instance.GetSiteVendorList(searchText); 
        }
        public List<SiteInfo> GetSiteUIDList(string searchText)
        {
            return DASiteInfo.Instance.GetSiteUIDList(searchText);
        }
        public SiteInfo getSitebyId(int siteId)
        {
            return DASiteInfo.Instance.getSitebyId(siteId);
        }
    }
}
