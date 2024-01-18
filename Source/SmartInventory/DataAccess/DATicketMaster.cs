using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DATicketMaster : Repository<TicketMaster>
    {
        public TicketMaster SaveTicket(TicketMaster objTicketMaster, int userId)
        {
            try
            {
                var objTicket = repo.Get(u => u.ticket_id == objTicketMaster.ticket_id);
                if (objTicket!=null)
                {
                    objTicket.ticket_type_id = objTicketMaster.ticket_type_id;
                    objTicket.ticket_reference = objTicketMaster.ticket_reference;
                    objTicket.ticket_description = objTicketMaster.ticket_description;
                    objTicket.reference_type = objTicketMaster.reference_type;
                    objTicket.reference_ticket_id = objTicketMaster.reference_ticket_id;
                    objTicket.can_id = objTicketMaster.can_id;
                    objTicket.customer_name = objTicketMaster.customer_name;
                    objTicket.pin_code = objTicketMaster.pin_code;
                    objTicket.address = objTicketMaster.address;
                    objTicket.building_code = objTicketMaster.building_code;
                    objTicket.bld_rfs_type = objTicketMaster.bld_rfs_type;
                    objTicket.assigned_to = objTicketMaster.assigned_to;
                    objTicket.target_date = objTicketMaster.target_date;
                    objTicket.modified_on = DateTimeHelper.Now;
                    objTicketMaster.pageMsg.isNewEntity = false;  
                    return repo.Update(objTicket);
                }
                objTicketMaster.created_on = DateTimeHelper.Now;
                objTicketMaster.ticket_status = "Assigned";
                objTicketMaster.ticket_status_id = 1;
                objTicketMaster.created_by = userId;
                objTicketMaster.assigned_by = userId;
                objTicketMaster.assigned_date = DateTimeHelper.Now;
                objTicketMaster.pageMsg.isNewEntity = true;
                objTicketMaster.address = objTicketMaster.address;
                objTicketMaster.building_code = objTicketMaster.building_code.ToUpper();
                var resultItem = repo.Insert(objTicketMaster);
                return resultItem;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public TicketMaster GetTicketById(int ticket_id)
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

        public TicketMaster GetTicketDetailByCanId(string canId)
        {
            try
            {
                return repo.Get(u => u.can_id == canId && u.ticket_status != "Completed");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DbMessage ValidateRfsType( string rfs_type )
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("FN_VALIDATE_RFS_TYPE", new { P_RFS_TYPE = rfs_type}).FirstOrDefault();

                bool chkstatus = false; 
                if (response.status)
                {
                    chkstatus = true;
                }

                return response;
            }
            catch { throw; } 
        }

        public List<TicketMasterGrid> GetTicketDetails(int userId, TicketManagerFilter objViewFilter)
        {
            //try
            //{
            //    return repo.ExecuteProcedure<TicketMasterGrid>("fn_get_ticket_export_report", new
            //    {
            //        P_fromDate = objViewFilter.fromDate,
            //        P_toDate = objViewFilter.toDate,
            //        p_searchby = objViewFilter.Searchtext,
            //        p_searchbytext = objViewFilter.SearchbyText,
            //        p_pageno = objViewFilter.currentPage,
            //        p_pagerecord = objViewFilter.pageSize,

            //        //P_SORTCOLNAME = objViewFilter.sort,
            //        //P_SORTTYPE = objViewFilter.orderBy,
            //        // p_userid = objViewFilter.userid,
            //        //p_grouptype = objViewFilter.role_id,
            //        // p_dduser = objViewFilter.dd_user,
            //        //p_status = objViewFilter.dd_surveyStatus,
            //        p_sortdir = objViewFilter.sortdir,

            //    }, true);
            //}
            try
            {
                //return repo.ExecuteProcedure<TicketMasterGrid>("fn_get_tickets_by_id", new
                return repo.ExecuteProcedure<TicketMasterGrid>("fn_get_tickets_by_id", new
                {
                    p_searchby = objViewFilter.SearchbyText,
                    p_searchtext = objViewFilter.Searchtext,
                    P_PAGENO = objViewFilter.currentPage,
                    P_PAGERECORD = objViewFilter.pageSize,
                    P_SORTCOLNAME = objViewFilter.sort,
                    P_SORTTYPE = objViewFilter.orderBy,
                    p_userid = userId,
                    p_searchfrom = objViewFilter.fromDate,
                    p_searchto = objViewFilter.toDate

                }).ToList();
            }
            catch { throw; }
        }
        public bool DeleteTicketById(string ticket_ids, int userId)
        {
            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_delete_ticket_by_ids", new { p_ticket_ids = ticket_ids, p_userId = userId });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;
            }
            catch { throw; }

        }

        
    }
    //public int DeleteTicketById(int ticket_id, int userId)
    //{
    //    int result = 0;
    //    try
    //    {
    //        var objTicket = repo.Get(u => u.ticket_id == ticket_id && u.assigned_by == userId);
    //        if (objTicket != null)
    //        { 
    //            result = repo.Delete(objTicket.ticket_id); 
    //            return result;
    //        }
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }
    //    return result;
    //}
    public class DADropDownItems : Repository<DropDownMaster>
    {
        public List<DropDownMaster> GetTicketDropdownList(string ddType, string enType = "")
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_dropdownlist", new { entitytype = enType, dropdownType = ddType });
            }
            catch { throw; }
        }
    }
    public class DATicketTypeMaster : Repository<TicketTypeMaster>
    {
        public List<TicketTypeMaster> GetTicketType()
        {
            try
            {
                return repo.GetAll().OrderBy(x => x.id).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetTicketTypeByID(string ticket_type)
        {
            int result = 0;
             
            try
            {
                 var objTicket= repo.Get(x => x.ticket_type == ticket_type);
                if (objTicket!=null)
                {
                    return result = objTicket.id;
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<TicketMasterGrid> getTicketList(int userId, TicketManagerFilter objTicketManagerFilter )
        {
            try
            {
                return repo.ExecuteProcedure<TicketMasterGrid>("fn_get_tickets_by_id", new
                { p_searchby = objTicketManagerFilter.SearchbyText,
                    p_searchtext = objTicketManagerFilter.Searchtext,
                    P_PAGENO = objTicketManagerFilter.currentPage,
                    P_PAGERECORD = objTicketManagerFilter.pageSize,
                    P_SORTCOLNAME = objTicketManagerFilter.sort,
                    P_SORTTYPE = objTicketManagerFilter.orderBy,
                    p_userid = userId,
                    p_searchfrom = objTicketManagerFilter.fromDate,
                    p_searchto = objTicketManagerFilter.toDate,
                    //p_lstTicketStatus = lstTicketSatatus,
                    //p_lstTicketType = lstTicketType
                    //  p_ticketStatus = objTicketManagerFilter.objticketstatus.Where(x => x.isStatusChecked = true)

                }).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ticketStatus> getTicketStatusCounts(int userId)
        {
            try
            {
                return repo.ExecuteProcedure<ticketStatus>("fn_get_ticket_status", new { p_userid = userId }).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

    public class DATempTicketMaster:Repository<TempTicketMaster>
    {
        private static DATempTicketMaster objTicketMaster = null;
        private static readonly object lockObject = new object();
        public static DATempTicketMaster Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objTicketMaster == null)
                    {
                        objTicketMaster = new DATempTicketMaster();
                    }
                }
                return objTicketMaster;
            }
        }
         public void DeleteTempTicketData(int user_id)
        { 
            try
            {
                repo.ExecuteProcedure<bool>("fn_delete_temp_ticket_master", new { P_Userid = user_id });
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Tuple<int, int> getTotalUploadTicketfailureAndSuccess(int UserId)
        {
            try
            {
                var getTotalUploadTicketfailure = repo.GetAll().Where(x => x.uploaded_by == UserId & x.is_valid == false).Count();
                var getTotalUploadTicketSuccess = repo.GetAll().Where(x => x.uploaded_by == UserId & x.is_valid == true).Count();
                return Tuple.Create(getTotalUploadTicketSuccess, getTotalUploadTicketfailure);
            }
            catch { throw; }
        } 
    public void BulkUploadTempTicket(List<TempTicketMaster> TempTicket)
        {
            try
            {
                repo.Insert(TempTicket);
            }
            catch { throw; }
        }

        public List<TempTicketMaster> GetUploadTicketLogs(int UserId)
        {
            try
            {
                return repo.GetAll().Where(x => x.uploaded_by == UserId).OrderBy(x=>x.ticket_id).ToList(); 
            }
            catch { throw; }
        }
        public DbMessage UploadTickets(int userID)
        {
            try
            { 
                return repo.ExecuteProcedure<DbMessage>("fn_bulk_upload_ticket_insert", new { P_UserId = userID}).FirstOrDefault();
            }
            catch { throw; }
        }
    }
    public class DAHPSMTicket : Repository<TicketMaster>
    {
        public TicketMaster SaveHPSMTicket(TicketMaster objTicketMaster)
        {
            try
            {
                var resultItem = repo.Insert(objTicketMaster);
                return resultItem;
            }
            catch
            {
                throw ;
            }
        }
    }
    public class DATicketAttachments : Repository<TicketAttachments>
    {
        public bool SaveTicketAttachments(List<TicketAttachments> obj, int user_id)
        {
            try
            {
                repo.Insert(obj);
                return true;
            }
            catch { throw; }
        }
    }
}

