using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
namespace BusinessLogics
{
    public class BLROW
    {
        private static BLROW objROW = null;
        private static readonly object lockObject = new object();
        public static BLROW Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objROW == null)
                    {
                        objROW = new BLROW();
                    }
                }
                return objROW;
            }
        }
        public ROWMaster SaveROW(ROWMaster objRow, int userId)
        {
            return DAROW.Instance.SaveROW(objRow, userId);
        }
        public rowApplyStage ApplyROW(rowApplyStage objROWApply, int userId)
        {
            return DAROWApply.Instance.ApplyROW(objROWApply, userId);
        }
        public rowApplyStage getROWApplyDetails(int rowSystemid)
        {
            return DAROWApply.Instance.getROWApplyDetails(rowSystemid);
        }
        public rowApproveRejectStage getROWApproveDetails(int rowSystemid)
        {
            return DAROWApprove.Instance.getROWApproveDetails(rowSystemid);
        }
        public rowApproveRejectStage ApproveROW(rowApproveRejectStage objApplyRow, int userId)
        {
            return DAROWApprove.Instance.ApproveROW(objApplyRow, userId);
        }
        public ROWPIT SavePIT(ROWPIT objROW, int userId)
        {
            return DAROWPIT.Instance.SavePIT(objROW, userId);
        }
        public List<ROWDetails> GetROWExist(string geom)
        {
            return DAROWPIT.Instance.GetROWExist(geom);
        }
        public double GetPITRadius(int systemId, string entityType)
        {
            return DAROWPIT.Instance.GetPITRadius(systemId, entityType);
        }
        public ROWArea GetAreaLength(int systemId, string entityType)
        {
            return DAROWPIT.Instance.GetAreaLength(systemId, entityType);
        }

        public ROWMaster getROWByPIT(int systemId)
        {
            return DAROWPIT.Instance.getROWByPIT(systemId);
        }

        public List<ROWAttachments> getROWAttachment(int systemId, string rowStage, string uploadType)
        {
            return DAROW.Instance.getROWAttachments(systemId, rowStage, uploadType);
        }


        public void SaveROWRemarks(List<ROWRemarks> objRemarks, int userId)
        {
            DAROWRemarks.Instance.SaveROWRemarks(objRemarks, userId);
        }
        public List<ROWRemarks> getROWRemarks(int systemId, string rowStage)
        {
            return DAROWRemarks.Instance.getROWRemarks(systemId, rowStage);
        }

        public List<ROWAssociateEntityList> getAssociateEntityList(int pSystemId, string pEntityType)
        {
            return DAROW.Instance.getAssociateEntityList(pSystemId, pEntityType);
        }
        public DbMessage saveROWAssocition(string objEntityList, int parentSystemId, string parentEntityType, string parentNetworkId, int userId)
        {
            return DAROW.Instance.saveROWAssocition(objEntityList, parentSystemId, parentEntityType, parentNetworkId, userId);
        }


        public List<Dictionary<string, string>> getAssociatedEntitylist(int entityid)
        {
            return DAROW.Instance.getAssociatedEntitylist(entityid);
        }
        public PITTemplateMaster SavePitTemplate(PITTemplateMaster objPITTemplate, int userId)
        {
            return DAPITTemplate.Instance.SavePitTemplate(objPITTemplate, userId);
        }
        public double getPITDefaultRadius(int rowSystemId, int userId)
        {
            return DAROW.Instance.getPITDefaultRadius(rowSystemId, userId);
        }
        public List<Dictionary<string, string>> GetExportApprovelReportData(ExportReportFilter objReportFilter)
        {
            return DAROW.Instance.GetExportApprovelReportData(objReportFilter);
        }
        public List<Dictionary<string, string>> GetExportReportData(ExportReportFilter objReportFilter)
        {
            return DAROW.Instance.GetExportReportData(objReportFilter);
        }
        public List<ExportReportKML> GetExportReportDataKML(ExportReportFilter objReportFilter)
        {
            return DAROW.Instance.GetExportReportDataKML(objReportFilter);
        }
        public ROWStageRecordCount GetROWStageRecordCount(ExportReportFilter objReportFilter)
        {
            return DAROW.Instance.GetROWStageRecordCount(objReportFilter);
        }
        public List<Dictionary<string, string>> GetExportRecurringReportData(ExportReportFilter objReportFilter)
        {
            return DAROW.Instance.GetExportRecurringReportData(objReportFilter);
        }
        public List<Dictionary<string, string>> GetExportBudgetReportData(int systemId)
        {
            return DAROW.Instance.GetExportBudgetReportData(systemId);
        }
        public List<DropDownMaster> getAuthorityList()
        {
            return DAROWAuthority.Instance.getAuthorityList();
        }
        public List<ROWChargesTemplate> getChargesTemplates()
        {
            return DAROWOtherCharges.Instance.getChargesTemplates();
        }
        public ROWChargesTemplate getTemplateById(int id)
        {
            return DAROWOtherCharges.Instance.getTemplateById(id);
        }
    }
}
