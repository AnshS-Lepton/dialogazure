using System;
using System.Collections.Generic;
using Models;
using Models.Admin;
using DataAccess.DBHelpers;
using System.Linq;
using Newtonsoft.Json;
using System.Data;
//using static Mono.Security.X509.X520;
//using Utility;


namespace DataAccess
{
    public class DANetworkTicket : Repository<NetworkTicket>
    {

        public DashboardInfo GetNetworkTicket(NetworkTicketFilter objNetworkTicketFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<DashboardInfo>("fn_nwt_get_ticket_details", new
                {
                    p_searchby = objNetworkTicketFilter.searchByText,
                    p_searchtext = objNetworkTicketFilter.searchText,
                    p_pageno = objNetworkTicketFilter.currentPage,
                    p_pagerecord = objNetworkTicketFilter.pageSize,
                    p_sortcolname = objNetworkTicketFilter.sort,
                    p_sorttype = objNetworkTicketFilter.sortdir,
                    p_userid = objNetworkTicketFilter.userId,
                    p_searchfrom = objNetworkTicketFilter.fromDate,
                    p_searchto = objNetworkTicketFilter.toDate,
                    p_ticket_type_id = objNetworkTicketFilter.objNetworkTicket.ticket_type_id,
                    p_region_id = objNetworkTicketFilter.objNetworkTicket.region_id ?? 0,
                    p_province_id = objNetworkTicketFilter.objNetworkTicket.province_id ?? 0,
                    p_ticket_status_id = objNetworkTicketFilter.objNetworkTicket.ticket_status_id,
                }, true).FirstOrDefault();
                return lst != null ? lst : new DashboardInfo();

            }
            catch { throw; }
        }


        public NWTicket_List_Status GetAPINetworkTicket(NetworkTicketFilter objNetworkTicketFilter)
        {
            try
            {
                var obj_NWTicket_List_Status = repo.ExecuteProcedure<NWTicket_List_Status>("fn_nwt_api_get_tickets", new
                {
                    p_userid = objNetworkTicketFilter.userId,
                    p_ticket_type_id = objNetworkTicketFilter.ticketTypeId,
                    p_searchtext = objNetworkTicketFilter.searchText,
                    p_fromdate = objNetworkTicketFilter.fromDate,
                    p_todate = objNetworkTicketFilter.toDate,

                }, true).FirstOrDefault();
                return obj_NWTicket_List_Status != null ? obj_NWTicket_List_Status : new NWTicket_List_Status();
            }
            catch { throw; }
        }
        public string SaveNetworkTicket(NetworkTicket objNetworkTicket, int userId)
        {
            try
            {
                var dt = DateTimeHelper.DateTimeFormate(objNetworkTicket.target_date.ToString());
                var result = repo.ExecuteProcedure<string>("fn_nwt_insert_update_ticket", new
                {
                    p_ticket_id = objNetworkTicket.ticket_id,
                    p_ticket_type_id = objNetworkTicket.ticket_type_id,
                    p_reference_type = objNetworkTicket.reference_type,
                    p_reference_description = objNetworkTicket.reference_description,
                    p_regionId = objNetworkTicket.region_id ?? 0,
                    P_provinceId = objNetworkTicket.province_id ?? 0,
                    p_network_id = objNetworkTicket.network_id,
                    p_name = objNetworkTicket.name,
                    p_assigned_to = objNetworkTicket.assigned_to,
                    p_target_date = DateTimeHelper.DateTimeFormate(objNetworkTicket.target_date.ToString()),
                    p_network_status = objNetworkTicket.for_network_type,
                    p_remarks = objNetworkTicket.remarks,
                    p_created_by = userId,
                    p_geom = objNetworkTicket.geom,
                    p_reference_ticket_id = objNetworkTicket.reference_ticket_id,
                    p_project_code = objNetworkTicket.project_code,
                    p_account_code = objNetworkTicket.account_code

                });
				return result.Last(); ;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string SaveNetworkTicketfromItemVCost(NetworkTicket objNetworkTicket, int userId)
        {
            try
            {
                var dt = DateTimeHelper.DateTimeFormate(objNetworkTicket.target_date.ToString());
                var result = repo.ExecuteProcedure<string>("fn_nwt_insert_update_ticket_IVCM", new
                {
                    p_ticket_id = objNetworkTicket.ticket_id,
                    p_ticket_type_id = objNetworkTicket.ticket_type_id,
                    p_reference_type = objNetworkTicket.reference_type,
                    p_reference_description = objNetworkTicket.reference_description,
                    p_regionId = objNetworkTicket.region_id ?? 0,
                    P_provinceId = objNetworkTicket.province_id ?? 0,
                    p_network_id = objNetworkTicket.network_id,
                    p_name = objNetworkTicket.name,
                    p_assigned_to = objNetworkTicket.assigned_to,
                    p_target_date = DateTimeHelper.DateTimeFormate(objNetworkTicket.target_date.ToString()),
                    p_network_status = objNetworkTicket.for_network_type,
                    p_remarks = objNetworkTicket.remarks,
                    p_created_by = userId,
                    p_geom = objNetworkTicket.geom,
                    p_reference_ticket_id = objNetworkTicket.reference_ticket_id,
                    p_project_code = objNetworkTicket.project_code,
                    p_account_code = objNetworkTicket.account_code

                });
                return result.Last(); ;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public DbMessage EditEntityInfo(EntityInfo objEntityInfo)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_nwt_update_entity_info", new { p_entity_system_id = objEntityInfo.system_id, p_entity_type = objEntityInfo.entity_type, p_entity_action = objEntityInfo.entity_action, p_source_ref_id = objEntityInfo.source_ref_id, p_source_ref_type = objEntityInfo.source_ref_type,p_attribute_info = objEntityInfo.attribute_info, p_updated_geom = objEntityInfo.geometry, p_updated_by = objEntityInfo.user_id }).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage DeleteNetworkTicketById(int ticket_id, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_nwt_delete_ticket", new { p_ticket_id = ticket_id, p_userId = userId }).FirstOrDefault();
            }
            catch { throw; }

        }
        public NetworkTicket GetNetworkTicketById(int ticket_id)
        {
            try
            {
                return repo.Get(u => u.ticket_id == ticket_id);
            }
            catch (Exception)
            {

                throw;
            }
        }
		public DataTable GetNetworkTicketDetailsById(int ticket_id)
		{
			try
			{
				string sQuery = string.Format("select  * from vw_att_details_networktickets  where ticket_id = {0} ", ticket_id);
				return repo.GetDataTable(sQuery);
			}
			catch
			{
				throw;
			}
		}
		public NWEntityInfo GetNetworkTicketEntityDetails(NetworkTicketFilter objNetworkTicketFilters)
        {
            try
            {
                var lst = repo.ExecuteProcedure<NWEntityInfo>("fn_nwt_get_ticket_entity_details",
                    new
                    {
                        p_pageno = objNetworkTicketFilters.currentPage,
                        p_pagerecord = objNetworkTicketFilters.pageSize,
                        p_userid = objNetworkTicketFilters.userId,
                        p_ticket_id = objNetworkTicketFilters.objNetworkTicket.ticket_id,
                        p_sortcolname = objNetworkTicketFilters.sort,
                        p_sorttype = objNetworkTicketFilters.sortdir,
                        p_searchtext= objNetworkTicketFilters.searchText,
                    }, true).FirstOrDefault();
                return lst != null ? lst : new NWEntityInfo();
            }
            catch { throw; }
        }
        public List<NetworkTicketEntityList> GetNetworkTicketRejectedEntityStatus(int ticket_id)
        {
            try
            {
                return repo.ExecuteProcedure<NetworkTicketEntityList>("fn_nwt_get_entity_list", new { p_ticket_id = ticket_id });
            }
            catch { throw; }
        }

		public List<Dictionary<string, string>> Get_NWEntity_History( NetworkTicketFilter objNetworkTicketFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_nwt_get_entity_status_history", new
                {
                    p_pageno = objNetworkTicketFilter.currentPage,
                    p_pagerecord = objNetworkTicketFilter.pageSize,
                    p_sortcolname = objNetworkTicketFilter.sort,
                    p_sorttype = objNetworkTicketFilter.sortdir,
                    p_userid = objNetworkTicketFilter.userId,
                    p_system_id = objNetworkTicketFilter.system_id,
                    p_entity_type = objNetworkTicketFilter.entity_type.ToUpper(),
                }, true);
                return lst;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public List<Dictionary<string, string>> GetDraftEntityInfo(NetworkTicketFilter objNetworkTicketFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_entity_info", new
                {
                 
                    p_systemid = objNetworkTicketFilter.system_id,
                    p_entitytype = objNetworkTicketFilter.entity_type,
                    p_geomtype = objNetworkTicketFilter.geom_type,
                    p_userid = objNetworkTicketFilter.userId
                }, true);
                return lst;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public GetGeometryByTicketId getNetworkTicketGeometry(int  ticket_id)
        {
            try
            {
                return repo.ExecuteProcedure<GetGeometryByTicketId>("fn_nwt_get_ticket_geometry", new { p_ticket_id = ticket_id}).FirstOrDefault();
            }
            catch { throw; }
        }
        public NWTktEntityLst GetNetworkTicketEntityList(NetworkTicketEntityListParam objparam)
        {
            try
            {
                var lst = repo.ExecuteProcedure<NWTktEntityLst>("fn_nwt_get_ticket_entity_details",
                    new
                    {
                        p_pageno = 0,
                        p_pagerecord = 0,
                        p_userid = objparam.user_id,
                        p_ticket_id = objparam.ticket_id,
                        p_sortcolname = "",
                        p_sorttype = "",
                    }, true).FirstOrDefault();
                return lst != null ? lst : new NWTktEntityLst();
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> NWEntitySummaryReport(int ticket_id,int user_id )
        {
            try
            {
                var list= repo.ExecuteProcedure<Dictionary<string, string>>("fn_nwt_get_entity_summary_report", new
                {
                    p_userid = user_id,
                    p_ticket_id = ticket_id,
                }, true);
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<DropDownMaster> GetDropDownList(string ddType = "")
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_nwt_get_dropdownlist", new {dropdownType = ddType });
            }
            catch { throw; }
        }
        public string getTicketEntityBounds(int ticketId)
        {
            try
            {

                return repo.ExecuteProcedure<string>("fn_nwt_get_entity_bound", new
                {
                    p_ticket_id = ticketId
                }).FirstOrDefault();
            }
            catch { throw; }
        }
		public string getTicketBounds(int ticketId)
		{
			try
			{

				return repo.ExecuteProcedure<string>("fn_nwt_get_ticket_bound", new
				{
					p_ticket_id = ticketId
				}).FirstOrDefault();
			}
			catch { throw; }
		}
		public string getTicketId(int systemId, string entityType,string network_id)
        {
            var value = repo.ExecuteProcedure<string>("fn_get_ticket_id", new { p_system_id = systemId, p_entityType = entityType, p_network_id= network_id }).FirstOrDefault();
            return value;
        }

        public List<NetworkTicketEmailDetail> GetNetworkTicketDetail(int ticket_id)
        {
           
            try
            {
                return repo.ExecuteProcedure<NetworkTicketEmailDetail>("fn_get_networkticketdetail", new { p_ticketid = ticket_id }, true);
            }
            catch { throw; }
        }
    }
}

public class DANetworkTicketTypeMaster : Repository<TicketTypeMaster>
{
    public List<TicketTypeMaster> GetTicketTypeByModule(string module,int uid=0, int role_id = 0,int ticked_id=0)
    {
        try
        {
            //return repo.GetAll().Where(x => x.module.ToUpper() == (module).ToUpper()).ToList(); ;
            return repo.ExecuteProcedure<TicketTypeMaster>("fn_Get_Ticket_Type_By_Role_Permission", new
            {
                p_user_id = uid,
                ticket_type = module,
                role_id = role_id,
                ticked_id = ticked_id

            }, true) ;
        }
        catch { throw; }
      
    }
    public List<TicketTypeMaster> GetHPSMTicketType(string module)
    {
        try
        {
            return repo.ExecuteProcedure<TicketTypeMaster>("fn_hpsm_ticket_type", new
            {
                ticket_type = module
            }, true);
        }
        catch { throw; }
    }
}

