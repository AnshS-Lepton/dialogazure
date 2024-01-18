using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
using BusinessLogics;

namespace BusinessLogics
{
    public class BLLmcInfo
    {
        private static BLLmcInfo objLMCInfo = null;
        private static readonly object lockObject = new object();
        public static BLLmcInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objLMCInfo == null)
                    {
                        objLMCInfo = new BLLmcInfo();
                    }
                }
                return objLMCInfo;
            }
        }
        public LMCCableInfo SaveLMCInfo(LMCCableInfo objLMCInfo, int userId)
        {
            return DALmcInfo.Instance.SaveLMCInfo(objLMCInfo, userId);
        }
        public LMCCableInfo GetLMCIfo(int cableId)
        {
            return DALmcInfo.Instance.GetLMCIfo(cableId);
        }
        public lmcdetails GetLMCId(string lmcType, string standalone_redundant)
        {
            return DALmcInfo.Instance.GetLMCId(lmcType, standalone_redundant);
        }
        public cableStartLatlong getCableLatLong(string entity_type, int system_id)
        {
            return DALmcInfo.Instance.getCableLatLong(entity_type, system_id);
        }
        public List<Dictionary<string, string>> GetExportLMCReportData(ExportLMCReportFilter objReportFilter)
        {
            return DALmcInfo.Instance.GetExportLMCReportData(objReportFilter);
        }
        public List<Dictionary<string, string>> GetExportLMCReportDataKML(ExportLMCReportFilter objReportFilter)
        {
            return DALmcInfo.Instance.GetExportLMCReportDataKML(objReportFilter);
        }
        public string GetColumnNameByDisplayName(string displayName,string lmcType,string entityType)
        {
            return DALmcInfo.Instance.GetColumnNameByDisplayName(displayName, lmcType, entityType);
        }
    }
}
