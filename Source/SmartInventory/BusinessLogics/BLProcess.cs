using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.ISP;
using Models;

namespace BusinessLogics
{
    public class BLProcess
    {
        public ProcessSummary SaveProcessSummary(ProcessSummary objProcessSummary)
        {
            return  new DAPrcocess().SaveProcess(objProcessSummary);
        }
        public EntitySummary SaveEntitySummary(int entityId, string entityType, int process_id,int secondaryPort)
        {
            return new DAPrcocess().SaveEntitySummary(entityId,entityType,process_id, secondaryPort);
        }
        public EntitySummary SaveXMLSummary(int entityId, string entityType, int process_id, int userId)
        {
            return new DAPrcocess().SaveXMLSummary(entityId, entityType, process_id, userId);
        }
        public List<EntitySummary> ValidateEntitySummary(string entityId, string entityType,int UserId, int subareaid)
        {
            return new DAPrcocess().ValidateEntitySummary(entityId, entityType,UserId, subareaid);
        }
		public List<SecondarySpilterSummary> ValidateSecondarySplitterData(string entityType, int UserId, int psystemId, string p_sourceRefId, string p_action)
		{
			return new DAPrcocess().ValidateSecondarySplitterData(entityType, UserId, psystemId, p_sourceRefId, p_action);
		}
		public List<SecondarySpilterLogSummary> DownloadSecondarySplitterValidationLogs(int UserId, string p_action, int? p_requestLogId)
		{
			return new DAPrcocess().DownloadSecondarySplitterValidationLogs( UserId, p_action, p_requestLogId);
		}
		public List<Dictionary<string, string>> GetProcessedXMLDetails(ProcessSummaryFilter objfilter)
        {
            return new DAPrcocess().GetProcessedXMLDetails(objfilter);
        }
        public List<Dictionary<string, string>> GetNEXMLSplitters(ProcessSummaryFilter objfilter)
        {
            return new DAPrcocess().GetNEXMLSplitters(objfilter);
        }
		public List<Dictionary<string, string>> GetSecondarySplitter(SeconarySplitterListFilter objfilter)
		{
			return new DAPrcocess().GetSecondarySplitter(objfilter);
		}
		public Boolean IsDesignSubmittedByEntity(int systemId, string entityType)
		{
			return new DAPrcocess().IsDesignSubmittedByEntity(systemId, entityType);
		}
		public ProcessedEntities GetProcessedEntitiesCount(ProcessSummary objProcessSummary)
        {
            return new DAPrcocess().GetProcessedEntitiesCount(objProcessSummary);
        }
        public DbMessage UpdateXmlFileStatus(string file_name,bool import_status, string file_version,string csa_id, int userId)
        {
            return new DAPrcocess().UpdateXmlFileStatus(file_name, import_status, file_version, csa_id, userId); 
        }
        public DbMessage ResetProcessedXMLDetails(int Process_Id,int userId)
        {
            return new DAPrcocess().ResetProcessedXMLDetails(Process_Id, userId);
        }
        public ProcessSummary UpdateFileVersion(ProcessSummary ObjProcess)
        {
            return new DAPrcocess().UpdateFileVersion(ObjProcess);
        }
        public string GetEquipmentCSAID(int process_id)
        {
            return new DAPrcocess().GetEquipmentCSAID(process_id);
        }
    }
}
