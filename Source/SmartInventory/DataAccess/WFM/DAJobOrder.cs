using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBContext;
using DataAccess.DBHelpers;
using Models;
using Models.WFM;
using Npgsql;





namespace DataAccess.WFM
{
    public class DAJobOrder
    {
        DAJobOrder()
        {

        }
        public static List<VW_Route_Issue> GetJobOrderDetails(ViewJobOrderFilter viewJobOrderFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<VW_Route_Issue> routeDetails = null;
                using (MainContext context = new MainContext())
                {
                    var rId = new NpgsqlParameter("@USERID", viewJobOrderFilter.userId);
                    var rsubordinateId = new NpgsqlParameter("@SUBUSERID", viewJobOrderFilter.subordinateUserId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewJobOrderFilter.searchText) ? "" : viewJobOrderFilter.searchText.ToUpper());
                    var rstatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewJobOrderFilter.status) ? "" : viewJobOrderFilter.status.ToUpper());
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewJobOrderFilter.joType) ? "" : viewJobOrderFilter.joType.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewJobOrderFilter.fromDate) ? "" : viewJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewJobOrderFilter.toDate) ? "" : viewJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewJobOrderFilter.sort) ? "" : viewJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewJobOrderFilter.sortdir) ? "" : viewJobOrderFilter.sortdir.ToUpper());
                    var rTicketSource = new NpgsqlParameter("@P_TICKETSOURCE", string.IsNullOrEmpty(viewJobOrderFilter.ticketSource) ? "" : viewJobOrderFilter.ticketSource.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewJobOrderFilter.serviceType) ? "" : viewJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewJobOrderFilter.joCategory) ? "" : viewJobOrderFilter.joCategory.ToUpper());
                    var rsearchby= new NpgsqlParameter("@P_SEARCHBY", viewJobOrderFilter.searchby);

                    var parameters = new NpgsqlParameter[15] { rId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rsubordinateId, rstatus, rissueType, rTicketSource, rserviceType, rjoCategory, rsearchby };
                    routeDetails = DbHelper.ExecutePostgresProcedure<VW_Route_Issue>(context, "fn_get_job_order_details", parameters, out totalRecords);
                    return routeDetails;
                }
            }
            catch
            {
                throw;
            }
        }
        public static string GetViewRMJobOrderStatusCount(ViewAssignJobOrderFilter viewAssignJobOrderFilter)
        {
            try
            {
                string statusCountString = "";
                int holdCount = 0;
                int inProgressCount = 0;
                int assignedCount = 0;
                int completedCount = 0;
                int rescheduledCount = 0;
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewAssignJobOrderFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewAssignJobOrderFilter.searchText) ? "" : viewAssignJobOrderFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewAssignJobOrderFilter.fromDate) ? "" : viewAssignJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewAssignJobOrderFilter.toDate) ? "" : viewAssignJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewAssignJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewAssignJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewAssignJobOrderFilter.sort) ? "" : viewAssignJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewAssignJobOrderFilter.sortdir) ? "" : viewAssignJobOrderFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewAssignJobOrderFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewAssignJobOrderFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewAssignJobOrderFilter.joType) ? "" : viewAssignJobOrderFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewAssignJobOrderFilter.ticketSource) ? "" : viewAssignJobOrderFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewAssignJobOrderFilter.status) ? "" : viewAssignJobOrderFilter.status.ToUpper());
                    var rUserId = new NpgsqlParameter("@p_user_id", viewAssignJobOrderFilter.userId);
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewAssignJobOrderFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewAssignJobOrderFilter.jomanagerId);
                    var rregionalHeadId = new NpgsqlParameter("@REGIONALHEADID", viewAssignJobOrderFilter.regionalHeadId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewAssignJobOrderFilter.isLoginRole);
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewAssignJobOrderFilter.serviceType) ? "" : viewAssignJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewAssignJobOrderFilter.joCategory) ? "" : viewAssignJobOrderFilter.joCategory.ToUpper());

                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewAssignJobOrderFilter.searchby);

                    var parameters = new NpgsqlParameter[21] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rUserId, rsubordinateGroupId, rsubordinatejoManagerId, rregionalHeadId, isregionalHead, rserviceType, rjoCategory, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_rm_job_order_status_count", parameters);
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        if (statuscount[i].status == "Hold")
                        {
                            holdCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "In-Progress")
                        {
                            inProgressCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Assigned")
                        {
                            assignedCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Completed")
                        {
                            completedCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Re-Scheduled")
                        {
                            rescheduledCount = statuscount[i].count;
                        }
                    }
                    statusCountString = "In-Progress: " + inProgressCount + " || Assigned: " + assignedCount + " || Re-Scheduled: " + rescheduledCount + " || Hold: " + holdCount + " || Completed: " + completedCount;
                    return statusCountString;
                }
            }
            catch
            {
                throw;
            }
        }
        public static string GetViewRMJobOrderStatusTTCount(ViewAssignJobOrderFilter viewIssueRouteFilter)
        {
            try
            {
                string statusCountString = "";
                int holdCount = 0;
                int inProgressCount = 0;
                int assignedCount = 0;
                int completedCount = 0;
                int rescheduledCount = 0;
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.joType) ? "" : viewIssueRouteFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());

                    var rcategory = new NpgsqlParameter("@P_CATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.customerCatagory) ? "" : viewIssueRouteFilter.customerCatagory.ToUpper());
                    var rsegment = new NpgsqlParameter("@P_SEGMENT", String.IsNullOrEmpty(viewIssueRouteFilter.customerSegment) ? "" : viewIssueRouteFilter.customerSegment.ToUpper());
                    var rttcategory = new NpgsqlParameter("@P_TTCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.ttcategory) ? "" : viewIssueRouteFilter.ttcategory.ToUpper());
                    var rtttype = new NpgsqlParameter("@P_TTID", String.IsNullOrEmpty(viewIssueRouteFilter.tttype) ? "" : viewIssueRouteFilter.tttype.ToUpper());
                    var rUserId = new NpgsqlParameter("@p_user_id", viewIssueRouteFilter.userId);
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewIssueRouteFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewIssueRouteFilter.jomanagerId);
                    var rregionalHeadId = new NpgsqlParameter("@REGIONALHEADID", viewIssueRouteFilter.regionalHeadId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewIssueRouteFilter.isLoginRole);
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRouteFilter.searchby);

                    var parameters = new NpgsqlParameter[23] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rcategory, rsegment, rttcategory, rtttype, rUserId, rsubordinateGroupId, rsubordinatejoManagerId, rregionalHeadId, isregionalHead, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_assig_route_issue_det_count_tt_rm", parameters);
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        if (statuscount[i].status == "Hold")
                        {
                            holdCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "In-Progress")
                        {
                            inProgressCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Assigned")
                        {
                            assignedCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Completed")
                        {
                            completedCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Re-Scheduled")
                        {
                            rescheduledCount = statuscount[i].count;
                        }
                    }
                    statusCountString = "In-Progress: " + inProgressCount + " || Assigned: " + assignedCount + " || Re-Scheduled: " + rescheduledCount + " || Hold: " + holdCount + " || Completed: " + completedCount;
                    return statusCountString;
                }
            }
            catch
            {
                throw;
            }
        }
        public static string GetViewJobOrderStatusCount(ViewAssignJobOrderFilter viewIssueRouteFilter)
        {
            try
            {
                string statusCountString = "";
                int holdCount = 0;
                int inProgressCount = 0;
                int assignedCount = 0;
                int completedCount = 0;
                int rescheduledCount = 0;
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.joType) ? "" : viewIssueRouteFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.serviceType) ? "" : viewIssueRouteFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.joCategory) ? "" : viewIssueRouteFilter.joCategory.ToUpper());

                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRouteFilter.searchby);


                    var parameters = new NpgsqlParameter[16] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_status_count", parameters);
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        if (statuscount[i].status == "Hold")
                        {
                            holdCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "In-Progress")
                        {
                            inProgressCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Assigned")
                        {
                            assignedCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Completed")
                        {
                            completedCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Re-Scheduled")
                        {
                            rescheduledCount = statuscount[i].count;
                        }
                    }
                    statusCountString = "In-Progress: " + inProgressCount + " || Assigned: " + assignedCount + " || Re-Scheduled: " + rescheduledCount + " || Hold: " + holdCount + " || Completed: " + completedCount;
                    return statusCountString;
                }
            }
            catch
            {
                throw;
            }
        }

        public static string GetViewJobOrderStatusCounttt(ViewAssignJobOrderFilter viewIssueRouteFilter)
        {
            try
            {
                string statusCountString = "";
                int holdCount = 0;
                int inProgressCount = 0;
                int assignedCount = 0;
                int completedCount = 0;
                int rescheduledCount = 0;
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.joType) ? "" : viewIssueRouteFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.serviceType) ? "" : viewIssueRouteFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.joCategory) ? "" : viewIssueRouteFilter.joCategory.ToUpper());

                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRouteFilter.searchby);


                    var parameters = new NpgsqlParameter[16] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_status_counttt", parameters);
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        if (statuscount[i].status == "Hold")
                        {
                            holdCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "In-Progress")
                        {
                            inProgressCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Assigned")
                        {
                            assignedCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Completed")
                        {
                            completedCount = statuscount[i].count;
                        }
                        else if (statuscount[i].status == "Re-Scheduled")
                        {
                            rescheduledCount = statuscount[i].count;
                        }
                    }
                    statusCountString = "In-Progress: " + inProgressCount + " || Assigned: " + assignedCount + " || Re-Scheduled: " + rescheduledCount + " || Hold: " + holdCount + " || Completed: " + completedCount;
                    return statusCountString;
                }
            }
            catch
            {
                throw;
            }
        }

        public static List<VW_Assign_Job_Order> GetAssignIssueRouteDetails(ViewAssignJobOrderFilter viewIssueRouteFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<VW_Assign_Job_Order> routeAssignDetails = null;
                using (MainContext context = new MainContext())
                {


                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.joType) ? "" : viewIssueRouteFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.serviceType) ? "" : viewIssueRouteFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.joCategory) ? "" : viewIssueRouteFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRouteFilter.searchby);

                    var parameters = new NpgsqlParameter[16] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory , rsearchby };
                    routeAssignDetails = DbHelper.ExecutePostgresProcedure<VW_Assign_Job_Order>(context, "FN_GET_ASSIG_ROUTE_ISSUE_DET", parameters, out totalRecords);

                    return routeAssignDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<VW_Assign_Job_Order> GetAssignIssueRouteDetailstt(ViewAssignJobOrderFilter viewIssueRouteFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<VW_Assign_Job_Order> routeAssignDetails = null;
                using (MainContext context = new MainContext())
                {

                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.joType) ? "" : viewIssueRouteFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.serviceType) ? "" : viewIssueRouteFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.joCategory) ? "" : viewIssueRouteFilter.joCategory.ToUpper());

                   
                    var rcategory = new NpgsqlParameter("@P_CATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.customerCatagory) ? "" : viewIssueRouteFilter.customerCatagory.ToUpper());
                    var rsegment = new NpgsqlParameter("@P_SEGMENT", String.IsNullOrEmpty(viewIssueRouteFilter.customerSegment) ? "" : viewIssueRouteFilter.customerSegment.ToUpper());


                    var rttcategory = new NpgsqlParameter("@P_TTCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.ttcategory) ? "" : viewIssueRouteFilter.ttcategory.ToUpper());
                    var rtttype = new NpgsqlParameter("@P_TTID", String.IsNullOrEmpty(viewIssueRouteFilter.tttype) ? "" : viewIssueRouteFilter.tttype.ToUpper());

                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRouteFilter.searchby);
                    var parameters = new NpgsqlParameter[20] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rcategory, rsegment, rttcategory, rtttype, rsearchby };
                    routeAssignDetails = DbHelper.ExecutePostgresProcedure<VW_Assign_Job_Order>(context, "fn_get_assig_route_issue_det_tt", parameters, out totalRecords);

                    return routeAssignDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void GetAssignedRouteDetails(ViewAssignJobOrder objViewAssignedRoute, out int totalRecords, int isExport = 0, int isApi = 0)
        {
            try
            {
                totalRecords = 0;


                List<VW_Route_Assigned> vw_Route_Assigned = null;
                List<VW_Route_Scheduled> vw_Route_Scheduled = null;
                List<RouteAssignedReport> routeAssignedReport = null;
                List<RouteScheduledReport> routeScheduledReport = null;
                using (MainContext context = new MainContext())
                {



                    var rId = new NpgsqlParameter("@P_MANAGERID", objViewAssignedRoute.viewAssignJobOrderFilter.userId);
                    var rText = new NpgsqlParameter("@P_SEARCHTEXT", string.IsNullOrEmpty(objViewAssignedRoute.viewAssignJobOrderFilter.searchText) ? "" : objViewAssignedRoute.viewAssignJobOrderFilter.searchText.ToUpper());
                    var rAssignedId = new NpgsqlParameter("@P_ASSIGNEDID", objViewAssignedRoute.viewAssignJobOrderFilter.assignId);
                    var rFDate = new NpgsqlParameter("@P_FROMDATE", string.IsNullOrEmpty(objViewAssignedRoute.viewAssignJobOrderFilter.fromDate) ? "" : objViewAssignedRoute.viewAssignJobOrderFilter.fromDate);
                    var rTDate = new NpgsqlParameter("@P_TODATE", string.IsNullOrEmpty(objViewAssignedRoute.viewAssignJobOrderFilter.toDate) ? "" : objViewAssignedRoute.viewAssignJobOrderFilter.toDate);
                    var rPNo = new NpgsqlParameter("@P_PAGENO", objViewAssignedRoute.viewAssignJobOrderFilter.currentPage);
                    var rPRecord = new NpgsqlParameter("@P_PAGERECORD", objViewAssignedRoute.viewAssignJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@P_SORTCOLNAME", string.IsNullOrEmpty(objViewAssignedRoute.viewAssignJobOrderFilter.sort) ? "" : objViewAssignedRoute.viewAssignJobOrderFilter.sort.ToUpper());
                    var rSortDir = new NpgsqlParameter("@P_SORTTYPE", string.IsNullOrEmpty(objViewAssignedRoute.viewAssignJobOrderFilter.sortdir) ? "" : objViewAssignedRoute.viewAssignJobOrderFilter.sortdir.ToUpper());
                    var rIsExport = new NpgsqlParameter("@P_ISEXPORT", isExport);


                    if (objViewAssignedRoute.viewAssignJobOrderFilter.selectedAssignType == 1)//ASSIGNED
                    {
                        if (isExport == 1) //code for export assigned route detail.
                        {
                            var parameters = new NpgsqlParameter[10] { rId, rText, rAssignedId, rFDate, rTDate, rPNo, rPRecord, rSortCol, rSortDir, rIsExport };
                            objViewAssignedRoute.lstRouteAssignedReport = DbHelper.ExecutePostgresProcedure<RouteAssignedReport>(context, "FN_GET_ASSIGN_ROUTE", parameters, out totalRecords);
                        }
                        else
                        {
                            if (isApi == 1) //code for api calling only in assigned route.
                            {
                                var parameters = new NpgsqlParameter[10] { rId, rText, rAssignedId, rFDate, rTDate, rPNo, rPRecord, rSortCol, rSortDir, rIsExport };
                                objViewAssignedRoute.lstRouteAssignedDetails = DbHelper.ExecutePostgresProcedure<VW_Route_Assigned>(context, "FN_GET_MY_TASK", parameters, out totalRecords);
                            }
                            else
                            {
                                var parameters = new NpgsqlParameter[10] { rId, rText, rAssignedId, rFDate, rTDate, rPNo, rPRecord, rSortCol, rSortDir, rIsExport };
                                objViewAssignedRoute.lstRouteAssignedDetails = DbHelper.ExecutePostgresProcedure<VW_Route_Assigned>(context, "FN_GET_ASSIGN_ROUTE", parameters, out totalRecords);
                            }
                        }
                    }
                    else //SCHEDULED
                    {
                        if (isExport == 1)//code for export scheduled route detail.
                        {
                            var parameters = new NpgsqlParameter[10] { rId, rText, rAssignedId, rFDate, rTDate, rPNo, rPRecord, rSortCol, rSortDir, rIsExport };
                            objViewAssignedRoute.lstRouteScheduledReport = DbHelper.ExecutePostgresProcedure<RouteScheduledReport>(context, "FN_GET_SCHEDULE_ROUTE", parameters, out totalRecords);
                        }
                        else
                        {
                            if (isApi == 1)//code for api calling only in scheduled route.
                            {
                                var parameters = new NpgsqlParameter[10] { rId, rText, rAssignedId, rFDate, rTDate, rPNo, rPRecord, rSortCol, rSortDir, rIsExport };
                                objViewAssignedRoute.lstRouteScheduledDetails = DbHelper.ExecutePostgresProcedure<VW_Route_Scheduled>(context, "FN_GET_MY_PLAN", parameters, out totalRecords);
                            }
                            else
                            {
                                var parameters = new NpgsqlParameter[10] { rId, rText, rAssignedId, rFDate, rTDate, rPNo, rPRecord, rSortCol, rSortDir, rIsExport };
                                objViewAssignedRoute.lstRouteScheduledDetails = DbHelper.ExecutePostgresProcedure<VW_Route_Scheduled>(context, "FN_GET_SCHEDULE_ROUTE", parameters, out totalRecords);
                            }
                        }
                    }
                }



            }
            catch
            {
                throw;
            }
        }

        public static List<Issue_Type_Master> GetIssueType()
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    return context.Issue_Type_Master.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static List<wfm_jo_type_master> GetJoType()
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    return context.wfm_jo_type_master.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static List<wfm_service_facility_master> GetServicesType()
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    return context.wfm_service_facility_master.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static List<JoCategoryMaster> GetJoCategory()
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    return context.JoCategoryMaster.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static JoCategoryMaster GetJoCategoryByCode(string Code)
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    return context.JoCategoryMaster.Where(x => x.jo_category_code == Code).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static List<DropDownMaster> GetDropDownList(string ddType = "")
        {
            try
            {
                List<DropDownMaster> lstDropDownMaster = null;
                using (MainContext context = new MainContext())
                {

                    var dd_Type = new NpgsqlParameter("@dropdowntype", ddType);
                    var parameters = new NpgsqlParameter[1] { dd_Type };
                    lstDropDownMaster = DbHelper.ExecutePostgresProcedure<DropDownMaster>(context, "fn_nwt_get_dropdownlist", parameters);
                }
                return lstDropDownMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<VW_Route_Issue> GetJobOrderDetailsRM(ViewJobOrderFilter viewJobOrderFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<VW_Route_Issue> routeDetails = null;
                using (MainContext context = new MainContext())
                {
                    var rId = new NpgsqlParameter("@USERID", viewJobOrderFilter.userId);
                    var rsubordinateId = new NpgsqlParameter("@SUBUSERID", viewJobOrderFilter.subordinateUserId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewJobOrderFilter.searchText) ? "" : viewJobOrderFilter.searchText.ToUpper());
                    var rstatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewJobOrderFilter.status) ? "" : viewJobOrderFilter.status.ToUpper());
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewJobOrderFilter.joType) ? "" : viewJobOrderFilter.joType.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewJobOrderFilter.fromDate) ? "" : viewJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewJobOrderFilter.toDate) ? "" : viewJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewJobOrderFilter.sort) ? "" : viewJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewJobOrderFilter.sortdir) ? "" : viewJobOrderFilter.sortdir.ToUpper());
                    var rTicketSource = new NpgsqlParameter("@P_TICKETSOURCE", string.IsNullOrEmpty(viewJobOrderFilter.ticketSource) ? "" : viewJobOrderFilter.ticketSource.ToUpper());
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewJobOrderFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewJobOrderFilter.jomanagerId);
                    var rregionalHeadId = new NpgsqlParameter("@REGIONALHEADID", viewJobOrderFilter.regionalHeadId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewJobOrderFilter.isLoginRole);
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewJobOrderFilter.serviceType) ? "" : viewJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewJobOrderFilter.joCategory) ? "" : viewJobOrderFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewJobOrderFilter.searchby);


                    var parameters = new NpgsqlParameter[19] { rId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rsubordinateId, rstatus, rissueType, rTicketSource, rsubordinateGroupId, rsubordinatejoManagerId, isregionalHead, rregionalHeadId, rserviceType, rjoCategory, rsearchby };
                    routeDetails = DbHelper.ExecutePostgresProcedure<VW_Route_Issue>(context, "fn_get_job_order_details_by_hierarchy", parameters, out totalRecords);

                    return routeDetails;
                }
            }
            catch
            {
                throw;
            }
        }

        public static List<VW_Route_Issue> GetttJobOrderDetailsRM(ViewJobOrderFilter viewJobOrderFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<VW_Route_Issue> routeDetails = null;
                using (MainContext context = new MainContext())
                {
                    var rId = new NpgsqlParameter("@USERID", viewJobOrderFilter.userId);
                    var rsubordinateId = new NpgsqlParameter("@SUBUSERID", viewJobOrderFilter.subordinateUserId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewJobOrderFilter.searchText) ? "" : viewJobOrderFilter.searchText.ToUpper());
                    var rstatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewJobOrderFilter.status) ? "" : viewJobOrderFilter.status.ToUpper());
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewJobOrderFilter.joType) ? "" : viewJobOrderFilter.joType.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewJobOrderFilter.fromDate) ? "" : viewJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewJobOrderFilter.toDate) ? "" : viewJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewJobOrderFilter.sort) ? "" : viewJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewJobOrderFilter.sortdir) ? "" : viewJobOrderFilter.sortdir.ToUpper());
                    // var rTicketSource = new NpgsqlParameter("@P_TICKETSOURCE", string.IsNullOrEmpty(viewJobOrderFilter.ticketSource) ? "" : viewJobOrderFilter.ticketSource.ToUpper());
                    var rcategory = new NpgsqlParameter("@P_CATEGORY", String.IsNullOrEmpty(viewJobOrderFilter.customerCatagory) ? "" : viewJobOrderFilter.customerCatagory.ToUpper());
                    var rsegment = new NpgsqlParameter("@P_SEGMENT", String.IsNullOrEmpty(viewJobOrderFilter.customerSegment) ? "" : viewJobOrderFilter.customerSegment.ToUpper());
                    var rttcategory = new NpgsqlParameter("@P_TTCATEGORY", String.IsNullOrEmpty(viewJobOrderFilter.ttcategory) ? "" : viewJobOrderFilter.ttcategory.ToUpper());
                    var rtttype = new NpgsqlParameter("@P_TTID", String.IsNullOrEmpty(viewJobOrderFilter.tttype) ? "" : viewJobOrderFilter.tttype.ToUpper());
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewJobOrderFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewJobOrderFilter.jomanagerId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewJobOrderFilter.isLoginRole);
                    var rregionalHeadId = new NpgsqlParameter("@REGIONALHEADID", viewJobOrderFilter.regionalHeadId);


                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewJobOrderFilter.searchby);

                    var parameters = new NpgsqlParameter[20] { rId, rsubordinateId, rtext, rstatus, rissueType, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rcategory, rsegment, rttcategory, rtttype, rsubordinateGroupId, rsubordinatejoManagerId, isregionalHead, rregionalHeadId, rsearchby };
                    routeDetails = DbHelper.ExecutePostgresProcedure<VW_Route_Issue>(context, "fn_get_tt_job_order_details_by_hierarchy", parameters, out totalRecords);

                    return routeDetails;
                }
            }
            catch
            {
                throw;
            }
        }

        public static List<VW_Assign_Job_Order> GetAssignIssueRouteDetailsRM(ViewAssignJobOrderFilter viewIssueRouteFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<VW_Assign_Job_Order> routeAssignDetails = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.joType) ? "" : viewIssueRouteFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());
                    var rUserId = new NpgsqlParameter("@p_user_id", viewIssueRouteFilter.userId);
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewIssueRouteFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewIssueRouteFilter.jomanagerId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewIssueRouteFilter.isLoginRole);
                    var rregionalHeadId = new NpgsqlParameter("@REGIONALHEADID", viewIssueRouteFilter.regionalHeadId);
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.serviceType) ? "" : viewIssueRouteFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.joCategory) ? "" : viewIssueRouteFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRouteFilter.searchby);


                    var parameters = new NpgsqlParameter[21] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rUserId, rsubordinateGroupId, rsubordinatejoManagerId, isregionalHead, rregionalHeadId, rserviceType, rjoCategory, rsearchby };
                    routeAssignDetails = DbHelper.ExecutePostgresProcedure<VW_Assign_Job_Order>(context, "fn_get_assig_route_issue_det_hierarchy", parameters, out totalRecords);

                    return routeAssignDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<VW_Assign_Job_Order> GetAssignIssueRouteDetailsttRM(ViewAssignJobOrderFilter viewIssueRouteFilter, out int totalRecords)
        {

            try
            {
                totalRecords = 0;
                List<VW_Assign_Job_Order> routeAssignDetails = null;
                using (MainContext context = new MainContext())
                {


                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.joType) ? "" : viewIssueRouteFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());

                    var rcategory = new NpgsqlParameter("@P_CATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.customerCatagory) ? "" : viewIssueRouteFilter.customerCatagory.ToUpper());
                    var rsegment = new NpgsqlParameter("@P_SEGMENT", String.IsNullOrEmpty(viewIssueRouteFilter.customerSegment) ? "" : viewIssueRouteFilter.customerSegment.ToUpper());
                    var rttcategory = new NpgsqlParameter("@P_TTCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.ttcategory) ? "" : viewIssueRouteFilter.ttcategory.ToUpper());
                    var rtttype = new NpgsqlParameter("@P_TTID", String.IsNullOrEmpty(viewIssueRouteFilter.tttype) ? "" : viewIssueRouteFilter.tttype.ToUpper());
                    var rUserId = new NpgsqlParameter("@p_user_id", viewIssueRouteFilter.userId);
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewIssueRouteFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewIssueRouteFilter.jomanagerId);
                    var rregionalHeadId = new NpgsqlParameter("@REGIONALHEADID", viewIssueRouteFilter.regionalHeadId);

                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewIssueRouteFilter.isLoginRole);
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRouteFilter.searchby);

                    var parameters = new NpgsqlParameter[23] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rcategory, rsegment, rttcategory, rtttype, rUserId, rsubordinateGroupId, rsubordinatejoManagerId, rregionalHeadId, isregionalHead, rsearchby };
                    routeAssignDetails = DbHelper.ExecutePostgresProcedure<VW_Assign_Job_Order>(context, "fn_get_assig_route_issue_det_tt_RM", parameters, out totalRecords);

                    return routeAssignDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public static List<DropDownMaster> GetDropDownListtt(string ddType = "")
        {
            try
            {
                List<DropDownMaster> lstDropDownMaster = null;
                using (MainContext context = new MainContext())
                {

                    var dd_Type = new NpgsqlParameter("@dropdowntype", ddType);
                    var parameters = new NpgsqlParameter[1] { dd_Type };
                    lstDropDownMaster = DbHelper.ExecutePostgresProcedure<DropDownMaster>(context, "fn_nwt_get_dropdownlist", parameters);
                }
                return lstDropDownMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<VW_Route_Issue> GetJobOrderDetailstt(ViewJobOrderFilter viewJobOrderFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<VW_Route_Issue> routeDetails = null;
                using (MainContext context = new MainContext())
                {
                    var rId = new NpgsqlParameter("@USERID", viewJobOrderFilter.userId);
                    var rsubordinateId = new NpgsqlParameter("@SUBUSERID", viewJobOrderFilter.subordinateUserId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewJobOrderFilter.searchText) ? "" : viewJobOrderFilter.searchText.ToUpper());
                    var rstatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewJobOrderFilter.status) ? "" : viewJobOrderFilter.status.ToUpper());
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewJobOrderFilter.issueType) ? "" : viewJobOrderFilter.issueType.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewJobOrderFilter.fromDate) ? "" : viewJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewJobOrderFilter.toDate) ? "" : viewJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewJobOrderFilter.sort) ? "" : viewJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewJobOrderFilter.sortdir) ? "" : viewJobOrderFilter.sortdir.ToUpper());
                    //var rTicketSource = new NpgsqlParameter("@P_TICKETSOURCE", string.IsNullOrEmpty(viewJobOrderFilter.ticketSource) ? "" : viewJobOrderFilter.ticketSource.ToUpper());
                    //var rServiceType = new NpgsqlParameter("@P_SERVICETYPE", string.IsNullOrEmpty(viewJobOrderFilter.ticketSource) ? "" : viewJobOrderFilter.ticketSource.ToUpper());

                    var rcategory = new NpgsqlParameter("@P_CATEGORY", String.IsNullOrEmpty(viewJobOrderFilter.customerCatagory) ? "" : viewJobOrderFilter.customerCatagory.ToUpper());
                    var rsegment = new NpgsqlParameter("@P_SEGMENT", String.IsNullOrEmpty(viewJobOrderFilter.customerSegment) ? "" : viewJobOrderFilter.customerSegment.ToUpper());


                    var rttcategory = new NpgsqlParameter("@P_TTCATEGORY", String.IsNullOrEmpty(viewJobOrderFilter.ttcategory) ? "" : viewJobOrderFilter.ttcategory.ToUpper());
                    var rtttype = new NpgsqlParameter("@P_TTID", String.IsNullOrEmpty(viewJobOrderFilter.tttype) ? "" : viewJobOrderFilter.tttype.ToUpper());

                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewJobOrderFilter.searchby);


                    // var rjocategory = new NpgsqlParameter("@P_JOCATEGORY", string.IsNullOrEmpty(viewJobOrderFilter.ticketSource) ? "" : viewJobOrderFilter.ticketSource.ToUpper());
                    var parameters = new NpgsqlParameter[16] { rId, rsubordinateId, rtext, rstatus, rissueType, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rcategory, rsegment, rttcategory, rtttype, rsearchby };
                    routeDetails = DbHelper.ExecutePostgresProcedure<VW_Route_Issue>(context, "fn_get_tt_job_order_details", parameters, out totalRecords);

                    return routeDetails;
                }
            }
            catch
            {
                throw;
            }
        }
        public static List<TT_Customer_Category> GetCustomerCategory()
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    return context.TT_Customer_Category.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public static List<TT_Customer_Segment> GetCustomerSegment()
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    return context.TT_Customer_Segment.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public static List<TT_category> Getttcatgeory()
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    return context.TT_category.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static TT_category GetttcatgeoryByName(string name)
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    TT_category objTTCategory = null;
                     objTTCategory= context.TT_category.Where(m => m.name == name).FirstOrDefault();
                    return objTTCategory;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static TT_Type GettTypeByName(string name)
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    TT_Type objTTType = null;
                    objTTType = context.TT_Type.Where(m => m.name == name).FirstOrDefault();
                    return objTTType;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static TT_Customer_Segment GettCustomerSegmentByName(string name)
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    TT_Customer_Segment objTTCustomerSegment = null;
                    objTTCustomerSegment = context.TT_Customer_Segment.Where(m => m.name == name).FirstOrDefault();
                    return objTTCustomerSegment;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static TT_Customer_Category GettCustomerCategoryByName(string name)
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    TT_Customer_Category objTTCustomerCategory = null;
                    objTTCustomerCategory = context.TT_Customer_Category.Where(m => m.name == name).FirstOrDefault();
                    return objTTCustomerCategory;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static List<TT_Type> Getttype(string type)
        {
            using (MainContext context = new MainContext())
            {
                try

                {
                    List<TT_Type> objUserDetail = null;

                    TT_category objTTCategory = null;
                    objTTCategory = context.TT_category.Where(m => m.name == type).FirstOrDefault();
                    int? id = null;
                    if (objTTCategory != null)
                    {
                        id = objTTCategory.id;
                    }
                    objUserDetail = context.TT_Type.Where(u => u.tt_category_id == id).ToList();

                    return objUserDetail;
                }

                //{
                //    return context.TT_Type.ToList();
                //}
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static GetUserPermissionArea GetUserPermissionArea(int userId)
        {
            GetUserPermissionArea GetUserPermissionAreaDetail = null;
            using (MainContext context = new MainContext())
            {                
                var user_Id = new NpgsqlParameter("@USERID", userId);
                var parameters = new NpgsqlParameter[1] { user_Id };
                GetUserPermissionAreaDetail = DbHelper.ExecutePostgresProcedure<GetUserPermissionArea>(context, "fn_get_user_permission_area", parameters).FirstOrDefault();
                return GetUserPermissionAreaDetail;
            }
        }
        public static List<VW_Assign_Job_Order> GetAdditionalMaterial(ViewAssignJobOrderFilter viewIssueRouteFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<VW_Assign_Job_Order> routeAssignDetails = null;
                using (MainContext context = new MainContext())
                {


                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.joType) ? "" : viewIssueRouteFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.serviceType) ? "" : viewIssueRouteFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.joCategory) ? "" : viewIssueRouteFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRouteFilter.searchby);

                    var parameters = new NpgsqlParameter[16] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rsearchby };
                    routeAssignDetails = DbHelper.ExecutePostgresProcedure<VW_Assign_Job_Order>(context, "fn_additional_material_detail", parameters, out totalRecords);

                    return routeAssignDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static GetUserPermissionArea GetAllPermissionArea(int userId)
        {
            using (MainContext context = new MainContext())
            {
                var user_Id = new NpgsqlParameter("@USERID", userId);
                var parameters = new NpgsqlParameter[1] { user_Id };
                GetUserPermissionArea GetUserPermissionAreaDetail = DbHelper.ExecutePostgresProcedure<GetUserPermissionArea>(context, "fn_get_all_permission_area", parameters).FirstOrDefault();
                return GetUserPermissionAreaDetail;
            }
        }

        public static string getprovince(string regionname)
        {
            using (MainContext context = new MainContext())
            {
                var province = new NpgsqlParameter("@REGIONNAME", regionname);
                var parameters = new NpgsqlParameter[1] { province };
                GetUserPermissionArea GetUserPermissionAreaDetail = DbHelper.ExecutePostgresProcedure<GetUserPermissionArea>(context, "fn_get_all_province", parameters).FirstOrDefault();
                return GetUserPermissionAreaDetail.province_name;
            }

        }
        
        public static string getsubdistrict(string provincename)
        {
            using (MainContext context = new MainContext())
            {
                var province = new NpgsqlParameter("@PROVINCENAME", provincename);
                var parameters = new NpgsqlParameter[1] { province };
                GetUserPermissionArea GetUserPermissionAreaDetail = DbHelper.ExecutePostgresProcedure<GetUserPermissionArea>(context, "fn_get_all_subdisctrict", parameters).FirstOrDefault();
                return GetUserPermissionAreaDetail.subdistrict_name;
            }
        }
        
        public static charts GetViewJobOrderStatusCountDashboard(ViewAssignJobOrder viewIssueRoute)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRoute.viewAssignJobOrderFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.searchText) ? "" : viewIssueRoute.viewAssignJobOrderFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.fromDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.toDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.viewAssignJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.viewAssignJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sort) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sortdir) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRoute.viewAssignJobOrderFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRoute.viewAssignJobOrderFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.ticketSource) ? "" : viewIssueRoute.viewAssignJobOrderFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.status) ? "" : viewIssueRoute.viewAssignJobOrderFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.serviceType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joCategory) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.viewAssignJobOrderFilter.searchby);

                    var parameters = new NpgsqlParameter[16] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_status_count_dashboard", parameters);
                    //var DATASET = DbHelper.ExecutePostgresProcedure<System.Object>(context, "fn_get_view_job_order_status_count_dash", parameters);
                    foreach (var item in viewIssueRoute.lststatus)
                    {
                        bool exits = false;
                        for (int i = 0; i < statuscount.Count; i++)
                        {
                            var status = statuscount[i];
                            if (status.status.ToUpper() == item.dropdown_value.ToUpper() || item.dropdown_value.ToUpper() == "ALL")
                            {
                                exits = true;
                            }

                        }
                        if (!exits)
                        {
                            statuscount.Add(new StatusCount { status = item.dropdown_value, count = 0 });
                        }
                    }
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                    }

                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }

        public static charts GetViewJobOrderfacilityCountDashboard(ViewAssignJobOrder viewIssueRoute)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRoute.viewAssignJobOrderFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.searchText) ? "" : viewIssueRoute.viewAssignJobOrderFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.fromDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.toDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.viewAssignJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.viewAssignJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sort) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sortdir) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRoute.viewAssignJobOrderFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRoute.viewAssignJobOrderFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.ticketSource) ? "" : viewIssueRoute.viewAssignJobOrderFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.status) ? "" : viewIssueRoute.viewAssignJobOrderFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.serviceType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joCategory) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joCategory.ToUpper());

                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.viewAssignJobOrderFilter.searchby);


                    var parameters = new NpgsqlParameter[16] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_facility_count", parameters);
                    foreach (var item in viewIssueRoute.lstServiceType)
                    {
                        bool exits = false;
                        for (int i = 0; i < statuscount.Count; i++)
                        {
                            var status = statuscount[i];
                            if (status.status == item.service_facility_code)
                            {
                                exits = true;
                            }

                        }
                        if (!exits)
                        {
                            statuscount.Add(new StatusCount { status = item.service_facility_code, count = 0 });
                        }
                    }

                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                    }
                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }

        public static charts GetViewJobOrderissuedescCountDashboard(ViewAssignJobOrder viewIssueRoute)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRoute.viewAssignJobOrderFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.searchText) ? "" : viewIssueRoute.viewAssignJobOrderFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.fromDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.toDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.viewAssignJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.viewAssignJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sort) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sortdir) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRoute.viewAssignJobOrderFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRoute.viewAssignJobOrderFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.ticketSource) ? "" : viewIssueRoute.viewAssignJobOrderFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.status) ? "" : viewIssueRoute.viewAssignJobOrderFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.serviceType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joCategory) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.viewAssignJobOrderFilter.searchby);


                    var parameters = new NpgsqlParameter[16] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_issue_desc_count", parameters);
                    foreach (var item in viewIssueRoute.lstJoType)
                    {
                        bool exits = false;
                        for (int i = 0; i < statuscount.Count; i++)
                        {
                            var status = statuscount[i];
                            if (status.status == item.jo_description)
                            {
                                exits = true;
                            }

                        }
                        if (!exits)
                        {
                            statuscount.Add(new StatusCount { status = item.jo_description, count = 0 });
                        }
                    }
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                    }


                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }

        public static charts GetViewJobOrdercityCountDashboard(ViewAssignJobOrder viewIssueRoute, string stateorprovince, IList<string> lstdistrictnames)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRoute.viewAssignJobOrderFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.searchText) ? "" : viewIssueRoute.viewAssignJobOrderFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.fromDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.toDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.viewAssignJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.viewAssignJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sort) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sortdir) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRoute.viewAssignJobOrderFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRoute.viewAssignJobOrderFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.ticketSource) ? "" : viewIssueRoute.viewAssignJobOrderFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.status) ? "" : viewIssueRoute.viewAssignJobOrderFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.serviceType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joCategory) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.viewAssignJobOrderFilter.searchby);
                    var rstateorprovince = new NpgsqlParameter("@P_STATEORPROVINCE", stateorprovince);


                    var parameters = new NpgsqlParameter[17] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rsearchby, rstateorprovince };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_city_count", parameters);
                    foreach (var item in lstdistrictnames)
                    {
                        bool exits = false;
                        for (int i = 0; i < statuscount.Count; i++)
                        {
                            var status = statuscount[i];
                            if (status.status == item)
                            {
                                exits = true;
                            }

                        }
                        if (!exits)
                        {
                            statuscount.Add(new StatusCount { status = item, count = 0 });
                        }
                    }
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                    }
                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }

        public static charts GetViewJobOrderassigntoCountDashboard(ViewAssignJobOrder viewIssueRoute, string assignto)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRoute.viewAssignJobOrderFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.searchText) ? "" : viewIssueRoute.viewAssignJobOrderFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.fromDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.toDate) ? "" : viewIssueRoute.viewAssignJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.viewAssignJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.viewAssignJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sort) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.sortdir) ? "" : viewIssueRoute.viewAssignJobOrderFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRoute.viewAssignJobOrderFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRoute.viewAssignJobOrderFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.ticketSource) ? "" : viewIssueRoute.viewAssignJobOrderFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.status) ? "" : viewIssueRoute.viewAssignJobOrderFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.serviceType) ? "" : viewIssueRoute.viewAssignJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.viewAssignJobOrderFilter.joCategory) ? "" : viewIssueRoute.viewAssignJobOrderFilter.joCategory.ToUpper());

                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.viewAssignJobOrderFilter.searchby);


                    var parameters = new NpgsqlParameter[16] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rsearchby };
                    if (assignto == "FE")
                    {
                        statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_assign_to_frt_count", parameters);
                        foreach (var item in viewIssueRoute.lstIssueAssignedToDetail)
                        {
                            bool exits = false;
                            for (int i = 0; i < statuscount.Count; i++)
                            {
                                var status = statuscount[i];
                                if (status.status == item.user_name)
                                {
                                    exits = true;
                                }

                            }
                            if (!exits)
                            {
                                statuscount.Add(new StatusCount { status = item.user_name, count = 0 });
                            }
                        }
                    }
                    else
                    {
                        statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_assign_to_dispatcher_count", parameters);
                        foreach (var item in viewIssueRoute.lstIssueRaisedByDetail)
                        {
                            bool exits = false;
                            for (int i = 0; i < statuscount.Count; i++)
                            {
                                var status = statuscount[i];
                                if (status.status == item.user_name)
                                {
                                    exits = true;
                                }

                            }
                            if (!exits)
                            {
                                statuscount.Add(new StatusCount { status = item.user_name, count = 0 });
                            }
                        }
                    }
                    int sum = 0;
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                        sum += statuscount[i].count;
                    }
                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    retchart.sum = sum;
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }

        public static charts GetViewJobOrderstateorprovinceCountDashboard(ViewAssignJobOrderFilter viewIssueRouteFilter)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rManagerId = new NpgsqlParameter("@MANAGERID", viewIssueRouteFilter.managerId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRouteFilter.searchText) ? "" : viewIssueRouteFilter.searchText.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRouteFilter.fromDate) ? "" : viewIssueRouteFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRouteFilter.toDate) ? "" : viewIssueRouteFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRouteFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRouteFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRouteFilter.sort) ? "" : viewIssueRouteFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRouteFilter.sortdir) ? "" : viewIssueRouteFilter.sortdir.ToUpper());
                    var rissueRaisedBy = new NpgsqlParameter("@P_ISSUERAISEDBY", viewIssueRouteFilter.issueRaisedBy);
                    var rissueAssignedTo = new NpgsqlParameter("@P_ISSUEASSIGNEDTO", viewIssueRouteFilter.issueAssignedTo);
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.joType) ? "" : viewIssueRouteFilter.joType.ToUpper());
                    var rticketSource = new NpgsqlParameter("@P_TICKETSOURCE", String.IsNullOrEmpty(viewIssueRouteFilter.ticketSource) ? "" : viewIssueRouteFilter.ticketSource.ToUpper());
                    var rStatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRouteFilter.status) ? "" : viewIssueRouteFilter.status.ToUpper());
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRouteFilter.serviceType) ? "" : viewIssueRouteFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRouteFilter.joCategory) ? "" : viewIssueRouteFilter.joCategory.ToUpper());

                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRouteFilter.searchby);


                    var parameters = new NpgsqlParameter[16] { rManagerId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rissueRaisedBy, rissueAssignedTo, rissueType, rticketSource, rStatus, rserviceType, rjoCategory, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_stateorprovice_count", parameters);
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status);
                        series.Add(statuscount[i].count);
                    }
                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }


        public static charts GetViewJobOrderStatusCountDashboardRM(ViewJobOrder viewIssueRoute)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;
                using (MainContext context = new MainContext())
                {
                    var rId = new NpgsqlParameter("@USERID", viewIssueRoute.vwJobOrderFilter.userId);
                    var rsubordinateId = new NpgsqlParameter("@SUBUSERID", viewIssueRoute.vwJobOrderFilter.subordinateUserId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.searchText) ? "" : viewIssueRoute.vwJobOrderFilter.searchText.ToUpper());
                    var rstatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.status) ? "" : viewIssueRoute.vwJobOrderFilter.status.ToUpper());
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joType) ? "" : viewIssueRoute.vwJobOrderFilter.joType.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.fromDate) ? "" : viewIssueRoute.vwJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.toDate) ? "" : viewIssueRoute.vwJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.vwJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.vwJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sort) ? "" : viewIssueRoute.vwJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sortdir) ? "" : viewIssueRoute.vwJobOrderFilter.sortdir.ToUpper());
                    var rTicketSource = new NpgsqlParameter("@P_TICKETSOURCE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.ticketSource) ? "" : viewIssueRoute.vwJobOrderFilter.ticketSource.ToUpper());
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewIssueRoute.vwJobOrderFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewIssueRoute.vwJobOrderFilter.jomanagerId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewIssueRoute.vwJobOrderFilter.isLoginRole == 1);
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.serviceType) ? "" : viewIssueRoute.vwJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joCategory) ? "" : viewIssueRoute.vwJobOrderFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.vwJobOrderFilter.searchby);

                    var parameters = new NpgsqlParameter[18] { rId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rsubordinateId, rstatus, rissueType, rTicketSource, rsubordinateGroupId, rsubordinatejoManagerId, isregionalHead, rserviceType, rjoCategory, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_status_count_dashboard_RM", parameters);
                    foreach (var item in viewIssueRoute.lststatus)
                    {
                        bool exits = false;
                        for (int i = 0; i < statuscount.Count; i++)
                        {
                            var status = statuscount[i];
                            if (status.status.ToUpper() == item.dropdown_value.ToUpper() || item.dropdown_value.ToUpper() == "ALL")
                            {
                                exits = true;
                            }

                        }
                        if (!exits)
                        {
                            statuscount.Add(new StatusCount { status = item.dropdown_value, count = 0 });
                        }
                    }
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                    }
                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }

        public static charts GetViewJobOrderassigntoCountDashboardRM(ViewJobOrder viewIssueRoute, string assignto)
        {
            charts retchart = new charts();
            try
            {
                int sum = 0;
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;

                using (MainContext context = new MainContext())
                {
                    var rId = new NpgsqlParameter("@USERID", viewIssueRoute.vwJobOrderFilter.userId);
                    var rsubordinateId = new NpgsqlParameter("@SUBUSERID", viewIssueRoute.vwJobOrderFilter.subordinateUserId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.searchText) ? "" : viewIssueRoute.vwJobOrderFilter.searchText.ToUpper());
                    var rstatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.status) ? "" : viewIssueRoute.vwJobOrderFilter.status.ToUpper());
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joType) ? "" : viewIssueRoute.vwJobOrderFilter.joType.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.fromDate) ? "" : viewIssueRoute.vwJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.toDate) ? "" : viewIssueRoute.vwJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.vwJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.vwJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sort) ? "" : viewIssueRoute.vwJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sortdir) ? "" : viewIssueRoute.vwJobOrderFilter.sortdir.ToUpper());
                    var rTicketSource = new NpgsqlParameter("@P_TICKETSOURCE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.ticketSource) ? "" : viewIssueRoute.vwJobOrderFilter.ticketSource.ToUpper());
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewIssueRoute.vwJobOrderFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewIssueRoute.vwJobOrderFilter.jomanagerId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewIssueRoute.vwJobOrderFilter.isLoginRole == 1);
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.serviceType) ? "" : viewIssueRoute.vwJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joCategory) ? "" : viewIssueRoute.vwJobOrderFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.vwJobOrderFilter.searchby);

                    var parameters = new NpgsqlParameter[18] { rId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rsubordinateId, rstatus, rissueType, rTicketSource, rsubordinateGroupId, rsubordinatejoManagerId, isregionalHead, rserviceType, rjoCategory, rsearchby };
                    if (assignto == "FE")
                    {
                        statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_assign_to_frt_count_rm", parameters);
                    }
                    else
                    {
                        statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_assign_to_dispatcher_count_rm", parameters);
                    }
                    foreach (var item in viewIssueRoute.lstUserDetail)
                    {
                        bool exits = false;
                        for (int i = 0; i < statuscount.Count; i++)
                        {
                            var status = statuscount[i];
                            if (status.status == item.user_name)
                            {
                                exits = true;
                            }
                        }
                        if (!exits)
                        {
                            statuscount.Add(new StatusCount { status = item.user_name, count = 0 });
                        }
                    }
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                        sum += statuscount[i].count;
                    }
                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    retchart.sum = sum;
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }

        public static charts GetViewJobOrdercityCountDashboardRM(ViewJobOrder viewIssueRoute, string stateorprovince, IList<string> lstdistrictnames)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;

                using (MainContext context = new MainContext())
                {
                    var rId = new NpgsqlParameter("@USERID", viewIssueRoute.vwJobOrderFilter.userId);
                    var rsubordinateId = new NpgsqlParameter("@SUBUSERID", viewIssueRoute.vwJobOrderFilter.subordinateUserId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.searchText) ? "" : viewIssueRoute.vwJobOrderFilter.searchText.ToUpper());
                    var rstatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.status) ? "" : viewIssueRoute.vwJobOrderFilter.status.ToUpper());
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joType) ? "" : viewIssueRoute.vwJobOrderFilter.joType.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.fromDate) ? "" : viewIssueRoute.vwJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.toDate) ? "" : viewIssueRoute.vwJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.vwJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.vwJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sort) ? "" : viewIssueRoute.vwJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sortdir) ? "" : viewIssueRoute.vwJobOrderFilter.sortdir.ToUpper());
                    var rTicketSource = new NpgsqlParameter("@P_TICKETSOURCE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.ticketSource) ? "" : viewIssueRoute.vwJobOrderFilter.ticketSource.ToUpper());
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewIssueRoute.vwJobOrderFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewIssueRoute.vwJobOrderFilter.jomanagerId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewIssueRoute.vwJobOrderFilter.isLoginRole );
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.serviceType) ? "" : viewIssueRoute.vwJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joCategory) ? "" : viewIssueRoute.vwJobOrderFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.vwJobOrderFilter.searchby);
                    var rstateorprovince = new NpgsqlParameter("@P_STATEORPROVINCE", stateorprovince);

                    var parameters = new NpgsqlParameter[19] { rId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rsubordinateId, rstatus, rissueType, rTicketSource, rsubordinateGroupId, rsubordinatejoManagerId, isregionalHead, rserviceType, rjoCategory, rsearchby, rstateorprovince };

                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_city_count_rm", parameters);

                    foreach (var item in lstdistrictnames)
                    {
                        bool exits = false;
                        for (int i = 0; i < statuscount.Count; i++)
                        {
                            var status = statuscount[i];
                            if (status.status == item)
                            {
                                exits = true;
                            }
                        }
                        if (!exits)
                        {
                            statuscount.Add(new StatusCount { status = item, count = 0 });
                        }
                    }
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                    }
                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }
        public static charts GetViewJobOrderfacilityCountDashboardRM(ViewJobOrder viewIssueRoute)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;

                using (MainContext context = new MainContext())
                {
                    var rId = new NpgsqlParameter("@USERID", viewIssueRoute.vwJobOrderFilter.userId);
                    var rsubordinateId = new NpgsqlParameter("@SUBUSERID", viewIssueRoute.vwJobOrderFilter.subordinateUserId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.searchText) ? "" : viewIssueRoute.vwJobOrderFilter.searchText.ToUpper());
                    var rstatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.status) ? "" : viewIssueRoute.vwJobOrderFilter.status.ToUpper());
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joType) ? "" : viewIssueRoute.vwJobOrderFilter.joType.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.fromDate) ? "" : viewIssueRoute.vwJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.toDate) ? "" : viewIssueRoute.vwJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.vwJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.vwJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sort) ? "" : viewIssueRoute.vwJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sortdir) ? "" : viewIssueRoute.vwJobOrderFilter.sortdir.ToUpper());
                    var rTicketSource = new NpgsqlParameter("@P_TICKETSOURCE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.ticketSource) ? "" : viewIssueRoute.vwJobOrderFilter.ticketSource.ToUpper());
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewIssueRoute.vwJobOrderFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewIssueRoute.vwJobOrderFilter.jomanagerId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewIssueRoute.vwJobOrderFilter.isLoginRole );
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.serviceType) ? "" : viewIssueRoute.vwJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joCategory) ? "" : viewIssueRoute.vwJobOrderFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.vwJobOrderFilter.searchby);

                    var parameters = new NpgsqlParameter[18] { rId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rsubordinateId, rstatus, rissueType, rTicketSource, rsubordinateGroupId, rsubordinatejoManagerId, isregionalHead, rserviceType, rjoCategory, rsearchby };

                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_facility_count_rm", parameters);

                    foreach (var item in viewIssueRoute.lstServiceType)
                    {
                        bool exits = false;
                        for (int i = 0; i < statuscount.Count; i++)
                        {
                            var status = statuscount[i];
                            if (status.status == item.service_facility_code)
                            {
                                exits = true;
                            }

                        }
                        if (!exits)
                        {
                            statuscount.Add(new StatusCount { status = item.service_facility_code, count = 0 });
                        }
                    }
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                    }
                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }
        public static charts GetViewJobOrderissuedescCountDashboardRM(ViewJobOrder viewIssueRoute)
        {
            charts retchart = new charts();
            try
            {
                List<string> lables = new List<string>();
                List<int> series = new List<int>();
                List<StatusCount> statuscount = null;

                using (MainContext context = new MainContext())
                {
                    var rId = new NpgsqlParameter("@USERID", viewIssueRoute.vwJobOrderFilter.userId);
                    var rsubordinateId = new NpgsqlParameter("@SUBUSERID", viewIssueRoute.vwJobOrderFilter.subordinateUserId);
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.searchText) ? "" : viewIssueRoute.vwJobOrderFilter.searchText.ToUpper());
                    var rstatus = new NpgsqlParameter("@P_STATUS", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.status) ? "" : viewIssueRoute.vwJobOrderFilter.status.ToUpper());
                    var rissueType = new NpgsqlParameter("@P_ISSUETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joType) ? "" : viewIssueRoute.vwJobOrderFilter.joType.ToUpper());
                    var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.fromDate) ? "" : viewIssueRoute.vwJobOrderFilter.fromDate);
                    var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.toDate) ? "" : viewIssueRoute.vwJobOrderFilter.toDate);
                    var rpageNo = new NpgsqlParameter("@PAGENO", viewIssueRoute.vwJobOrderFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", viewIssueRoute.vwJobOrderFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sort) ? "" : viewIssueRoute.vwJobOrderFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.sortdir) ? "" : viewIssueRoute.vwJobOrderFilter.sortdir.ToUpper());
                    var rTicketSource = new NpgsqlParameter("@P_TICKETSOURCE", string.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.ticketSource) ? "" : viewIssueRoute.vwJobOrderFilter.ticketSource.ToUpper());
                    var rsubordinateGroupId = new NpgsqlParameter("@USERGROUPID", viewIssueRoute.vwJobOrderFilter.groupmanagerId);
                    var rsubordinatejoManagerId = new NpgsqlParameter("@USERJOMANAGERID", viewIssueRoute.vwJobOrderFilter.jomanagerId);
                    var isregionalHead = new NpgsqlParameter("@ISREGIONALHEAD", viewIssueRoute.vwJobOrderFilter.isLoginRole );
                    var rserviceType = new NpgsqlParameter("@P_SERVICETYPE", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.serviceType) ? "" : viewIssueRoute.vwJobOrderFilter.serviceType.ToUpper());
                    var rjoCategory = new NpgsqlParameter("@P_JOCATEGORY", String.IsNullOrEmpty(viewIssueRoute.vwJobOrderFilter.joCategory) ? "" : viewIssueRoute.vwJobOrderFilter.joCategory.ToUpper());
                    var rsearchby = new NpgsqlParameter("@P_SEARCHBY", viewIssueRoute.vwJobOrderFilter.searchby);

                    var parameters = new NpgsqlParameter[18] { rId, rtext, rfromDate, rToDate, rpageNo, rpageRecord, rSortCol, rSortType, rsubordinateId, rstatus, rissueType, rTicketSource, rsubordinateGroupId, rsubordinatejoManagerId, isregionalHead, rserviceType, rjoCategory, rsearchby };
                    statuscount = DbHelper.ExecutePostgresProcedure<StatusCount>(context, "fn_get_view_job_order_issue_desc_count_rm", parameters);
                    foreach (var item in viewIssueRoute.lstJoType)
                    {
                        bool exits = false;
                        for (int i = 0; i < statuscount.Count; i++)
                        {
                            var status = statuscount[i];
                            if (status.status == item.jo_description)
                            {
                                exits = true;
                            }

                        }
                        if (!exits)
                        {
                            statuscount.Add(new StatusCount { status = item.jo_description, count = 0 });
                        }
                    }
                    for (int i = 0; i < statuscount.Count; i++)
                    {
                        lables.Add(statuscount[i].status + " (" + statuscount[i].count + ")");
                        series.Add(statuscount[i].count);
                    }
                    retchart.labels = lables.ToArray();
                    retchart.series = series.ToArray();
                    return retchart;
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
