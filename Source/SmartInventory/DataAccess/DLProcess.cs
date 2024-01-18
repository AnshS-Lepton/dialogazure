using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

namespace DataAccess
{
    public class DAPrcocess : Repository<ProcessSummary>
    {
        public ProcessSummary SaveProcess(ProcessSummary objProcess_summary)
        {
            try
            {

                if (objProcess_summary.process_id > 0)
                {
                    var result = repo.GetById(objProcess_summary.process_id);
                    if (result != null)
                    {
                        result.csa_id = objProcess_summary.csa_id;
                        result.file_extension = objProcess_summary.file_extension;
                        result.file_name = objProcess_summary.file_name;
                        result.process_end_time = objProcess_summary.process_end_time;
                        result.total_entity = objProcess_summary.total_entity;
                        result.created_by = objProcess_summary.created_by;
                        result.nas_status = objProcess_summary.nas_status;
                        result.file_version = objProcess_summary.file_version;
                        return repo.Update(result);
                    }
                }
                else
                {
                    objProcess_summary.created_on = DateTimeHelper.Now;
                    var response = repo.Insert(objProcess_summary);
                    return response;
                }
                return null;
            }

            catch { throw; }
        }
        public ProcessedEntities GetProcessedEntitiesCount(ProcessSummary objProcessSummary)
        {
            try
            {

                return repo.ExecuteProcedure<ProcessedEntities>("fn_get_total_processed_entities", new { p_process_id = objProcessSummary.process_id }, false).FirstOrDefault();

            }
            catch (Exception ex) { throw ex; }
        }
        public int SaveEntitySummary(ProcessSummary objProcess_summary)
        {
            try
            {
                var resultItem = repo.Insert(objProcess_summary);

                return resultItem.process_id;

            }

            catch { throw; }
        }

        public EntitySummary SaveEntitySummary(int entityId, string entityType, int process_id, int secondaryPort)
        {
            try
            {

                return repo.ExecuteProcedure<EntitySummary>("fn_process_insert_entity_summary_details", new { p_system_id = entityId, p_entity_type = entityType, p_process_id = process_id, p_secondaryPort = secondaryPort }, false).FirstOrDefault();

            }
            catch { throw; }
        }
        public EntitySummary SaveXMLSummary(int entityId, string entityType, int process_id, int userId)
        {
            try
            {

                return repo.ExecuteProcedure<EntitySummary>("fn_process_insert_entity_summary_details", new { p_system_id = entityId, p_entity_type = entityType, p_process_id = process_id, p_user_id = userId }, false).FirstOrDefault();

            }
            catch { throw; }
        }
        public List<EntitySummary> ValidateEntitySummary(string entityId, string entityType,int UserId, int subareaid)
        {
            try
            {

                return repo.ExecuteProcedure<EntitySummary>("fn_process_validate_network_V2", new { p_system_id = entityId, p_entity_type = entityType, p_user_id = UserId, p_subareaid = subareaid }, true).ToList();

            }
            catch { throw; }
        }
		public List<SecondarySpilterSummary> ValidateSecondarySplitterData(string entityType, int UserId, int psystemId, string p_sourceRefId, string p_action)
		{
			try
			{
                if(p_action== "DesignValidation")
                {
					return repo.ExecuteProcedure<SecondarySpilterSummary>("fn_design_validation_s1s2", new { p_entity_type = entityType, p_user_id = UserId, p_subareaid = psystemId }, true).ToList();

				}
                else
                {
					return repo.ExecuteProcedure<SecondarySpilterSummary>("fn_construction_validation_s1s2", new { p_ticket_id = Convert.ToInt32(p_sourceRefId),p_user_id = UserId }, true).ToList();

				}

			}
			catch { throw; }
		}

		public List<SecondarySpilterLogSummary> DownloadSecondarySplitterValidationLogs(int UserId, string p_action, int? p_requestLogId)
		{
			try
			{
				return repo.ExecuteProcedure<SecondarySpilterLogSummary>("fn_get_Secondary_Splitter_log", new { p_user_id = UserId, p_request_log_id = p_requestLogId, p_action = p_action }, true).ToList();

			}
			catch { throw; }
		}
		public List<Dictionary<string, string>> GetProcessedXMLDetails(ProcessSummaryFilter objFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_processedxmldata", new
                {
                    p_searchby = objFilter.searchByText,
                    p_searchtext = objFilter.searchText,
                    p_pageno = objFilter.currentPage,
                    p_pagerecord = objFilter.pageSize,
                    p_sortcolname = objFilter.sort,
                    p_sorttype = objFilter.sortdir,
                    p_userid = objFilter.objProcessSummary.userId,
                    p_system_id = objFilter.systemId,
                    p_ring_no= objFilter.ps_port
                }, true);
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Dictionary<string, string>> GetNEXMLSplitters(ProcessSummaryFilter objFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("FN_PROCESS_GET_NEXMLSPLITTERS", new
                {
                    searchby = objFilter.searchByText,
                    searchtext = objFilter.searchText,
                    p_pageno = objFilter.currentPage,
                    p_pagerecord = objFilter.pageSize,
                    p_sortcolname = objFilter.sort,
                    p_sorttype = objFilter.sortdir,
                    p_totalrecords = objFilter.totalRecord,
                    p_system_id = objFilter.systemId,
                    p_entity_type= objFilter.entityType,
                    p_user_id = objFilter.objProcessSummary.userId
                }, true);
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
		public List<Dictionary<string, string>> GetSecondarySplitter(SeconarySplitterListFilter objFilter)
		{
			try
			{
				var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_trace_get_secondary_splitter", new
				{
					searchby = objFilter.searchByText,
					searchtext = objFilter.searchText,
					p_sortcolname = objFilter.sort,
					p_sorttype = objFilter.sortdir,
					p_system_id = objFilter.systemId,
					p_entity_type = objFilter.entityType,
					p_source_ref_id= objFilter.source_ref_id,
                    p_action= objFilter.action_name
				}, true);
				return lst;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public Boolean IsDesignSubmittedByEntity(int systemId, string entityType)
		{
			try
			{
				return repo.ExecuteProcedure<Boolean>("fn_is_design_submitted_by_entity",
				  new
				  {
					  p_entity_type = entityType,
					  p_system_id= systemId
				  }).FirstOrDefault();
			}
			catch
			{
				throw;
			}
		}

		//public List<Dictionary<string, string>> BoundaryPushtogis(ProcessSummaryFilter objFilter)
		//{
		//    try
		//    {
		//        var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_push_Boundary_Dashboard", new
		//        {
		//            searchby = objFilter.searchByText,
		//            searchtext = objFilter.searchText,
		//            p_pageno = objFilter.currentPage,
		//            p_pagerecord = objFilter.pageSize,
		//            p_sortcolname = objFilter.sort,
		//            p_sorttype = objFilter.sortdir,
		//            p_totalrecords = objFilter.totalRecord,
		//            p_system_id = objFilter.systemId,
		//            p_entity_type = objFilter.entityType
		//        }, true);
		//        return lst;
		//    }
		//    catch (Exception ex)
		//    {
		//        throw ex;
		//    }
		//}

		//public List<Dictionary<string, string>> GetNEXMLSplitters1(ProcessSummaryFilter objFilter)
		//{
		//    try
		//    {
		//        var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_push_Boundary_Dashboard", new
		//        {
		//            searchby = objFilter.searchByText,
		//            searchtext = objFilter.searchText,
		//            p_pageno = objFilter.currentPage,
		//            p_pagerecord = objFilter.pageSize,
		//            p_sortcolname = objFilter.sort,
		//            p_sorttype = objFilter.sortdir,
		//            p_totalrecords = objFilter.totalRecord,
		//            p_system_id = objFilter.systemId,
		//            p_entity_type = objFilter.entityType
		//        }, true);
		//        return lst;
		//    }
		//    catch (Exception ex)
		//    {
		//        throw ex;
		//    }
		//}



		public DbMessage UpdateXmlFileStatus(string file_name , bool import_status, string file_version,string csa_id,int userId)
        {
            try
            {
                DbMessage obj = new DbMessage();
                var objrequest = repo.GetAll(x => x.file_name == file_name).OrderByDescending(m => m.process_id).FirstOrDefault();
                if (objrequest != null)
                    obj = repo.ExecuteProcedure<DbMessage>("fn_process_update_nestatus", new { p_csa_id = csa_id, p_import_status = import_status }, false).FirstOrDefault();
                //if (objrequest != null &&  !import_status)
                //    obj = repo.ExecuteProcedure<DbMessage>("FN_PROCESS_RESET_SUMMARY_DETAILS", new { p_process_id = objrequest.process_id, p_user_id = userId }, false).FirstOrDefault();
                return obj;
            }

            catch { throw; } 
        }

        public DbMessage ResetProcessedXMLDetails(int Process_Id,int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_process_reset_summary_details", new { p_process_id = Process_Id, p_user_id = userId }, false).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ProcessSummary UpdateFileVersion(ProcessSummary ObjProcess)
        {
            try
            {
                var lstCSAVersions = repo.GetAll(x => x.csa_id == ObjProcess.csa_id && x.import_status == true).ToList();
                if (lstCSAVersions.Count > 0)
                {
                    ObjProcess.file_version = "Version_" + (lstCSAVersions.Count+1);
                    return repo.Update(ObjProcess);
                }
                else
                {
                    ObjProcess.file_version = "Version_1";
                }
                return repo.Update(ObjProcess);
            }
            catch { throw; }
        }

        public string GetEquipmentCSAID(int process_id)
        {
            try
            {
                return repo.ExecuteProcedure<string>("fn_get_equipment_csaid", new { p_process_id = process_id }, false).FirstOrDefault();
            }
            catch {throw ;}
        }


    }

}
