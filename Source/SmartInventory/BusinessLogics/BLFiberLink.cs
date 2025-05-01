using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLFiberLink
    {
        public List<Dictionary<string, string>> getFiberLinkDetails(int userId, FiberLinkFilter objFiberLinkFilter)
        {
            return new DAFiberLink().getFiberLinkDetails(userId, objFiberLinkFilter);
        }
        public List<Dictionary<string, string>> getAssociatedFiberLinkDetails(int userId, FiberLinkFilter objFiberLinkFilter)
        {
            return new DAFiberLink().getAssociatedFiberLinkDetails(userId, objFiberLinkFilter);
        }
        public Dictionary<string, string> getLinkInfoForKML(int linkSystemId)
        {
            return new DAFiberLink().getLinkInfoForKML(linkSystemId);
        }
        public List<fiberLinkStatus> getfiberLinkStatusCounts(FiberLinkFilter objFiberLinkFilter,int userId)
        {
            return new DAFiberLink().getfiberLinkStatusCounts(objFiberLinkFilter,userId);
        }
        public FiberLink GetFiberLinkById(int system_id)
        {
            return new DAFiberLink().GetFiberLinkById(system_id);
        }
        public FiberLink SaveFiberLink(FiberLink objFiberLink, int userId)
        {
            return new DAFiberLink().SaveFiberLink(objFiberLink, userId);
        }
        public FLNetworkCode GetFiberLinkNetworkId()
        {
            return new DAFiberLink().GetFiberLinkNetworkId();
        }
        public int deleteFiberLinkById(int system_id)
        {
            return new DAFiberLink().deleteFiberLinkById(system_id);
        }
        public DbMessage disconnectFiberLinkById(int system_id)
        {
            return new DAFiberLink().disconnectFiberLinkById(system_id);
        }
        public List<FiberLink> getFiberLinkROWAuthority(string searchText)
        {
            return new DAFiberLink().getFiberLinkROWAuthority(searchText);
        }
        public FiberLinkPrefix GetlinkPrefixbyPrefixType(string link_prefix)
        {
            return new DAFiberLink().GetlinkPrefixbyPrefixType(link_prefix);
        }
        public List<FiberLink> GetAutoFiberLinkId(string searchText)
        {
            return new DAFiberLink().GetAutoFiberLinkId(searchText);
        }
        public FiberLink isFiberLinkIdExist(string linkId, string columnName, int userId)
        {
            return new DAFiberLink().isFiberLinkIdExist(linkId, columnName, userId);
        }
        public  Dictionary<string, string> getAssociatedLinkInfo(int cable_id, int fiber_number)
        {
            return new DAFiberLink().getAssociatedLinkInfo(cable_id, fiber_number);
        }

        public List<Dictionary<string, string>> getExportCableInfoByLinkId(int p_LinkId)
        {
            return new DAFiberLink().getExportCableInfoByLinkId(p_LinkId);
        }
        public List<Dictionary<string, string>> getExportCableInfoByLinkSystemIds(string p_LinkSystemIds)
        {
            return new DAFiberLink().getExportCableInfoByLinkSystemIds(p_LinkSystemIds);
        }
        public vmfiberLinkOnMap getFiberLinkElements(int linkSystemId, int userId)
        {
            return new DAFiberLink().getFiberLinkElements(linkSystemId,userId);
        }
        public vmfiberLinkOnMap getFiberLinkElementsByLinkSystemIds(string linkSystemIds, int userId)
        {
            return new DAFiberLink().getFiberLinkElementsByLinkSystemIds(linkSystemIds,userId);
        }
        public fiberLinkAssociation getAssociatedLinkId(int cable_id, int fiber_number)
        {
            return new DAFiberLink().getAssociatedLinkId(cable_id, fiber_number);
        }
        public List<Dictionary<string, string>> getAssociationCustomer(FiberLinkCustomerFilter objFiberLinkCustomerFilter)
        {
            return new DAFiberLink().getAssociationCustomer(objFiberLinkCustomerFilter);
        }
        public List<FiberLink> checkDuplicaketLinkId(string link_id)
        {
            return new DAFiberLink().checkDuplicaketLinkId(link_id);
        }
        public List<Dictionary<string, string>> GetFiberLinks(int userId, FiberLinkFilter objFiberLinkFilter)
        {
            return new DAFiberLink().GetFiberLinks(userId, objFiberLinkFilter);
        }
        public List<string> GetFiberLinksByLinkIds(string linkids)
        {
            return new DAFiberLink().GetFiberLinksByLinkIds(linkids);
        }
    }
    public class BLFiberLinkColumns
    {
        //public List<Dictionary<string,string>> getFiberLinkColumns()
        //{
        //    return new DAFiberLinkColumns().getFiberLinkColumns();
        //}
        public List<fiberLinkColumnsMapping> getFiberLinkColumns()
        {
            return new DAFiberLinkColumns().getFiberLinkColumns();
        }
    }
    public class BLTempFiberLink
    {
        private static BLTempFiberLink objFiberLink = null;
        private static readonly object lockObject = new object();
        public static BLTempFiberLink Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objFiberLink == null)
                    {
                        objFiberLink = new BLTempFiberLink();
                    }
                }
                return objFiberLink;
            }
        }

        public void DeleteTempFiberData(int created_by)
        {
            DATempFiberMaster.Instance.DeleteTempFiberData(created_by);
        }
        public void BulkUploadTempFiber(List<TempFiberLink> BulkUploadTempFiber)
        {
            DATempFiberMaster.Instance.BulkUploadTempFiber(BulkUploadTempFiber);
        }
        public DbMessage UploadFiber(int created_by, string network_id)
        {
            return DATempFiberMaster.Instance.UploadFiber(created_by,network_id);
        }
        public Tuple<int, int> getTotalUploadFiberfailureAndSuccess(int created_by)
        {
            return DATempFiberMaster.Instance.getTotalUploadFiberfailureAndSuccess(created_by);
        }
        public List<TempFiberLink> GetUploadFiberLogs(int created_by)
        {
            return DATempFiberMaster.Instance.GetUploadFiberLogs(created_by);
        }

    }    
}
