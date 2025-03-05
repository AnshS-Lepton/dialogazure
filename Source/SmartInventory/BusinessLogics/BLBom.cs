using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
using Models.Admin;

namespace BusinessLogics
{
    public class BLBom
    {
        private static BLBom objBLBom = null;
        private static readonly object lockObject = new object();
        public static BLBom Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBLBom == null)
                    {
                        objBLBom = new BLBom();
                    }
                }
                return objBLBom;
            }
        }
        public List<BOMReport> getBOMReport(BOMInput objInput)
        {
            return DABom.Instance.getBOMReport(objInput);
        }

        public List<BOMReport> getISPBOMReport(int structure_id, int building_id = 0)
        {
            return DABom.Instance.getISPBOMReport(structure_id, building_id);
        }
        public List<BOMReport> getAssociateEntityBOMReport(int systemId, string entityType)
        {
            return DABom.Instance.getAssociateEntityBOMReport(systemId, entityType);
        }
        public List<ConstructionBomDetailsInfo> GetConstructionBomDetailList(ConstructionBomDetailsVM objConstructionBomDetailsVM)
        {
            return new DAConstructionBOMDetails().GetConstructionBomDetailList(objConstructionBomDetailsVM);
        }
        public ConstructionBomDetails GetConstructionBomDetailsByID(int id)
        {
            return new DAConstructionBOMDetails().GetConstructionBomDetailsByID(id);
        }
        public ConstructionBomDetails SaveConstructionBomDetails(ConstructionBomDetails objConstructionBomDetails)
        {
            return new DAConstructionBOMDetails().SaveConstructionBomDetails(objConstructionBomDetails);
        }

    }
    public class BomBoq
    {  //////////// Krishna

        public List<BOMBOQReport> getBOMBOQReport(BomBoqExportFilter objFilter)
        {
            return new DABomBoq().getBOMBOQReport(objFilter);
        }
        public List<VendorSpecificationMaster> getSiteBOMBOQReport(CommonGridAttr objFilter,int site_plan_id)
        {
            return new DABomBoqSite().getSiteBOMBOQReport(objFilter, site_plan_id);
        }
        public int  getSiteplanid(int system_id)
        {
            return new DABomBoqSite1().getSiteplanid(system_id);
        }
        public List<BomBoqAdAttribute> getBOMBOQExportAttribute(int userid)
        {
            return new DABomBoq().getBOMBOQExportAttribute(userid);
        }
        public BomBoqAdAttribute Save_ExportAttribute(BomBoqAdAttribute objExpAttr)
        {
            return new DABomBoq().Save_ExportAttribute(objExpAttr);
        }
        public List<dBLossDetail> getdBLossReport(BomBoqExportFilter objFilter)
        {
            return new DABomBoq().getdBLossReport(objFilter);
        }
        //public List<dBLossDetail> getLayersStructuerWise(BomBoqExportFilter objFilter)
        //{
        //    return new DABomBoq().getLayersStructuerWise(objFilter);
        //}

        public BomBoqInfoSummary GetBOMBOQDesignReport(BomBoqExportFilterDesign objFilter)
        {
            return new DABomBoqInfoSummary().GetBOMBOQDesignReport(objFilter);
        }

        public List<BomBoqInfo> GetBOMBOQDependentItems(int BomBoqId, int BoundarySystemId, string EntityClass, string EntitySubClass, string sAction)
        {
            return new DABomBoqInfoSummary().GetBOMBOQDependentItems(BomBoqId, BoundarySystemId, EntityClass, EntitySubClass, sAction);
        }

        public List<string> SaveBOMBOQReportDesign(List<BomBoqInfo> lstBomBoqInfo)
        {
            return new DABomBoqInfoSummary().SaveBOMBOQReportDesign(lstBomBoqInfo);
        }
        public string UpdateBomBoqStatus(BomBoqInfoSummary objBomBoqInfoSummary)
        {
            return new DABomBoqInfoSummary().UpdateBomBoqStatus(objBomBoqInfoSummary);
        }

        public List<BomBoqInfo> ExportBOMBOQ(BomBoqExportFilterDesign objFilter)
        {
            return new DABomBoqInfoSummary().ExportBOMBOQ(objFilter);
        }
        public List<KeyValueDropDown> GetBomBoqProjectCode(int bom_boq_id)
        {
            return new DABomBoqInfoSummary().GetBomBoqProjectCode(bom_boq_id);
        }

    }
    public class BLUnlock
	{
		public List<FSAlockedDetailsInfo> GetFSAUnlockBomDetailList(FSAlockedDetailsVM objFSAlockedDetailsVM)
		{
			return new DAFSABomUnlockDetailsInfo().GetFSAUnlockBomDetailList(objFSAlockedDetailsVM);
		}
		public FSAlockedDetailsInfo UnlockFSAByID(int id)
		{
			return new DAFSABomUnlockDetailsInfo().UnlockFSAByID(id);
		}
	}
}
