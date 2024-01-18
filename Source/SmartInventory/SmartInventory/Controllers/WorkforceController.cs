using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using BusinessLogics;
using BusinessLogics.WFM;
using Models;
using Models.WFM;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using Utility;
using System.Configuration;
using ICSharpCode.SharpZipLib.Zip;
using System.Dynamic;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class WorkforceController : Controller
    {
        public static string[] _Ticket_Source = { "", "ONT_HOBS", "ONT_FRT_BULK", "OUTAGE", "Trouble Ticket" };
        [HttpPost]
        public PartialViewResult AssignJobOrder(ViewJobOrder objviewJobOrder, int page = 0, string sort = "", string sortdir = "", string status = "")
        {
            int totalRecords = 0;
            //int managerId =486;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            var lstSubourdinateDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();

            if (string.IsNullOrEmpty(objviewJobOrder.vwJobOrderFilter.status))
            {
                objviewJobOrder.vwJobOrderFilter.status = status;
            }

            objviewJobOrder.lstJoType = BLJobOrder.GetJoType();
            objviewJobOrder.lstServiceType = BLJobOrder.GetServicesType();
            objviewJobOrder.lstJoCategory = BLJobOrder.GetJoCategory();
            objviewJobOrder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_assign_status.ToString()).ToList();
            objviewJobOrder.vwJobOrderFilter.userId = managerId;
            objviewJobOrder.vwJobOrderFilter.pageSize = objviewJobOrder.vwJobOrderFilter.pageSize == 0 ? 10 : objviewJobOrder.vwJobOrderFilter.pageSize;
            objviewJobOrder.vwJobOrderFilter.fromDate = MiscHelper.FormatDate(objviewJobOrder.vwJobOrderFilter.fromDate);
            objviewJobOrder.vwJobOrderFilter.toDate = MiscHelper.FormatDate(objviewJobOrder.vwJobOrderFilter.toDate);
            objviewJobOrder.vwJobOrderFilter.page = page;
            objviewJobOrder.vwJobOrderFilter.sort = sort;
            objviewJobOrder.vwJobOrderFilter.sortdir = sortdir;
            objviewJobOrder.vwJobOrderFilter.currentPage = page == 0 ? 1 : page;
            //objviewJobOrder.lstUserDetail = new BLUser().GetAllActiveUsersList().ToList();
            objviewJobOrder.lstUserDetail = lstSubourdinateDetail;

            List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetJobOrderDetails(objviewJobOrder.vwJobOrderFilter, out totalRecords);

            if (lstrouteIssues.Count > 0)
            {
                lstrouteIssues.Select(c =>
                {
                    c.ticket_source = _Ticket_Source[c.ticket_source_id];
                    return c;
                }).ToList();
            }
            objviewJobOrder.lstIssueDetails = lstrouteIssues;
            objviewJobOrder.vwJobOrderFilter.totalRecord = totalRecords;

            Session["viewIssueRouteFilter"] = objviewJobOrder.vwJobOrderFilter;

            return PartialView("_AssignJobOrder", objviewJobOrder);
        }

        [HttpPost]
        public ActionResult ViewAssignJobOrder(ViewAssignJobOrder objViewAssignJobOrder, int page = 0, string sort = "", string sortdir = "")
        {
            int totalRecords = 0;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            //commenting because userRole session throwing exception no data  
            //if (Convert.ToInt32(Session["userRole"].ToString()) == 8) //RH view
            //{
            //    if (Session["managerId"] != null) //RH view
            //    {
            //        managerId = Convert.ToInt32(Session["managerId"].ToString());
            //    }
            //    else
            //    {
            //        return Content("Please select manager");
            //    }
            //}
            var lstUserDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();

            objViewAssignJobOrder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_view_status.ToString()).ToList();
            objViewAssignJobOrder.lstIssueRaisedByDetail = lstUserDetail;
            objViewAssignJobOrder.lstIssueAssignedToDetail = lstUserDetail;
            objViewAssignJobOrder.lstJoType = BLJobOrder.GetJoType();
            objViewAssignJobOrder.lstServiceType = BLJobOrder.GetServicesType();
            objViewAssignJobOrder.lstJoCategory = BLJobOrder.GetJoCategory();
            objViewAssignJobOrder.viewAssignJobOrderFilter.managerId = managerId;
            objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize = objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize == 0 ? 10 : objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize;
            objViewAssignJobOrder.viewAssignJobOrderFilter.fromDate = MiscHelper.FormatDate(objViewAssignJobOrder.viewAssignJobOrderFilter.fromDate);
            objViewAssignJobOrder.viewAssignJobOrderFilter.toDate = MiscHelper.FormatDate(objViewAssignJobOrder.viewAssignJobOrderFilter.toDate);
            objViewAssignJobOrder.viewAssignJobOrderFilter.page = page;
            objViewAssignJobOrder.viewAssignJobOrderFilter.sort = sort;
            objViewAssignJobOrder.viewAssignJobOrderFilter.sortdir = sortdir;
            objViewAssignJobOrder.viewAssignJobOrderFilter.currentPage = page == 0 ? 1 : page;

            //if(objViewAssignJobOrder.viewAssignJobOrderFilter.status==null || objViewAssignJobOrder.viewAssignJobOrderFilter.status == "")
            //{
            //    objViewAssignJobOrder.viewAssignJobOrderFilter.status = "ALL";
            //}
            objViewAssignJobOrder.statusCount = BLJobOrder.GetViewJobOrderStatusCount(objViewAssignJobOrder.viewAssignJobOrderFilter);
            List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAssignJobOrderDetails(objViewAssignJobOrder.viewAssignJobOrderFilter, out totalRecords);

            lstAssignIssueDetails.Select(c =>
            {
                c.ticket_source = _Ticket_Source[c.ticket_source_id];
                return c;
            }).ToList();

            objViewAssignJobOrder.lstAssignJobOrderDetails = lstAssignIssueDetails;

            objViewAssignJobOrder.viewAssignJobOrderFilter.totalRecord = totalRecords;
            Session["viewAssignRouteIssueFilter"] = objViewAssignJobOrder.viewAssignJobOrderFilter;

            return PartialView("_ViewAssignJobOrder", objViewAssignJobOrder);
        }

        [HttpPost]
        public ActionResult ViewTTAssignJobOrder(ViewAssignJobOrder objViewAssignJobOrder, int page = 0, string sort = "", string sortdir = "")
        {
            int totalRecords = 0;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            //commenting because userRole session throwing exception no data  
            //if (Convert.ToInt32(Session["userRole"].ToString()) == 8) //RH view
            //{
            //    if (Session["managerId"] != null) //RH view
            //    {
            //        managerId = Convert.ToInt32(Session["managerId"].ToString());
            //    }
            //    else
            //    {
            //        return Content("Please select manager");
            //    }
            //}
            var lstUserDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();
            objViewAssignJobOrder.lstCustomerCategory = BLJobOrder.GetCustomerCategory();
            objViewAssignJobOrder.lstCustomerSegment = BLJobOrder.GetCustomerSegment();
            objViewAssignJobOrder.lstcategory = BLJobOrder.Getttcatgeory();
            //Models.User objUserDetails = objBLuser.getUserDetails(managerId);
            objViewAssignJobOrder.lsttype = BLJobOrder.Getttype(string.IsNullOrEmpty(objViewAssignJobOrder.viewAssignJobOrderFilter.ttcategory) ? "" : objViewAssignJobOrder.viewAssignJobOrderFilter.ttcategory);
            objViewAssignJobOrder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_view_status.ToString()).ToList();
            objViewAssignJobOrder.lstIssueRaisedByDetail = lstUserDetail;
            objViewAssignJobOrder.lstIssueAssignedToDetail = lstUserDetail;
            objViewAssignJobOrder.lstCustomerCategory = BLJobOrder.GetCustomerCategory();
            objViewAssignJobOrder.lstJoType = BLJobOrder.GetJoType();
            objViewAssignJobOrder.lstServiceType = BLJobOrder.GetServicesType();
            objViewAssignJobOrder.lstJoCategory = BLJobOrder.GetJoCategory();
            objViewAssignJobOrder.viewAssignJobOrderFilter.managerId = managerId;
            objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize = objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize == 0 ? 10 : objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize;
            objViewAssignJobOrder.viewAssignJobOrderFilter.fromDate = MiscHelper.FormatDate(objViewAssignJobOrder.viewAssignJobOrderFilter.fromDate);
            objViewAssignJobOrder.viewAssignJobOrderFilter.toDate = MiscHelper.FormatDate(objViewAssignJobOrder.viewAssignJobOrderFilter.toDate);
            objViewAssignJobOrder.viewAssignJobOrderFilter.page = page;
            objViewAssignJobOrder.viewAssignJobOrderFilter.sort = sort;
            objViewAssignJobOrder.viewAssignJobOrderFilter.sortdir = sortdir;
            objViewAssignJobOrder.viewAssignJobOrderFilter.currentPage = page == 0 ? 1 : page;
            //if(objViewAssignJobOrder.viewAssignJobOrderFilter.status==null || objViewAssignJobOrder.viewAssignJobOrderFilter.status == "")
            //{
            //    objViewAssignJobOrder.viewAssignJobOrderFilter.status = "ALL";
            //}
            objViewAssignJobOrder.statusCount = BLJobOrder.GetViewJobOrderStatusCounttt(objViewAssignJobOrder.viewAssignJobOrderFilter);
            List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAssignJobOrderDetailstt(objViewAssignJobOrder.viewAssignJobOrderFilter, out totalRecords);

            lstAssignIssueDetails.Select(c =>
            {
                c.ticket_source = _Ticket_Source[c.ticket_source_id];
                return c;
            }).ToList();

            objViewAssignJobOrder.lstAssignJobOrderDetails = lstAssignIssueDetails;

            objViewAssignJobOrder.viewAssignJobOrderFilter.totalRecord = totalRecords;
            Session["viewAssignRouteIssueFilter"] = objViewAssignJobOrder.viewAssignJobOrderFilter;

            return PartialView("_ViewTTAssignJobOrder", objViewAssignJobOrder);
        }



        #region NMS Ticket detail
        public PartialViewResult ViewNMSTicketDetails(ViewNMSTicketDetails model, string id, int page = 0, string sort = "", string sortdir = "", string status = "")
        {
            nmsticket objT = BLWFMTicket.GetNMSTicket_Details(id);
            if (objT.ticketId != "")
            {
                model.TaskDetails.Add(objT);
            }
            model.viewTicketFilter.currentPage = 1;
            model.viewTicketFilter.ticketId = id;
            model.viewTicketFilter.totalRecord = model.TaskDetails.Count();
            return PartialView("_ViewNMSTicketDetail", model);
        }

        public PartialViewResult ViewHPSMTicketTaskDetails(ViewHPSMTicketDetails model, string id, int page = 0, string sort = "", string sortdir = "", string status = "")
        {
            // Models.WFM.Task objT = BLWFMTicket.GetHPSMTicket_Details(id);

            //var RCA = BLIssues.GetIssueResolutonTypeByResolutionCode(objT.resolution_close_id);
            //var RC = BLIssues.GetIssueResolutonTypeByResolutionCode(objT.root_cause_id);
            //List<VW_HPSM_Ticket_Master_History> objList


            // model.ticketDetails = BLHPSMTicket.GetHPSMTicketDetails(id);
            model.ticket_Details = BLWFMTicket.GetHPSMTicket_Detail_Status_History(id);
            model.viewTicketFilter.currentPage = 1;
            model.viewTicketFilter.ticketId = id;
            model.viewTicketFilter.totalRecord = model.ticket_Details.Count();
            return PartialView("_ViewHPSMTicketDetail", model);
            //objT.root_cause_id = RCA == null ? objT.root_cause_id : RCA.issue_resolution_description;
            //objT.resolution_close_id = RC == null ? objT.resolution_close_id : RC.issue_resolution_description;
            //////////////////////////////////
            //objT.root_cause_id = "";
            //objT.resolution_close_id = "";

            //if (objT.hpsm_ticket_id != 0)
            //{
            //    //var resType=BLRoute.GetIssueResolutionTypes
            //    model.TaskDetails.Add(objT);
            //}
            //model.viewTicketFilter.currentPage = 1;
            //model.viewTicketFilter.ticketId = id;
            //model.viewTicketFilter.totalRecord = model.TaskDetails.Count();
            //return PartialView("_ViewHPSMTicketDetail", model);
        }

        [HttpPost]
        public PartialViewResult ViewVSFTicketDetails(ViewVSFTicketDetails model, string id, int page = 0, string sort = "", string sortdir = "", string status = "")
        {
            int TicketId = Convert.ToInt32(id);

            model.ticketDetails = BLWFMTicket.GetVSFTicketDetails(TicketId);

            model.totalRecord = model.ticketDetails.Count();
            model.ticketId = TicketId;

            return PartialView("_ViewVSFIssues", model); //display is grid
        }

        #endregion
        public ActionResult ManagerRouteIssueApprove(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            //viewRouteIssueApprove.issueId = issueId;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            //int managerId = 397;

            int intCircle_id = 0;

            int intCircleCount = BLIssues.IssueCircleCount(viewRouteIssueApprove.issuesId, out intCircle_id);
            viewRouteIssueApprove.CircleCount = intCircleCount;

            if (intCircleCount > 1)
            {
                //return RedirectToAction("_ViewMessage");
                return PartialView("_ViewMessage", viewRouteIssueApprove);
            }

            if (viewRouteIssueApprove.status == "Assigned" || viewRouteIssueApprove.status == "Re-Assigned")
            {
                //Need to be FRT than Patroller.
                //viewRouteIssueApprove.lstUserDetail = BLUser.GetSubordinateDetails(managerId, "FRT", intCircle_id).ToList();
                // List<UserManagerMapping> userManagerMapping = new List<UserManagerMapping>();
                // userManagerMapping = BLUser.GetMMSubordinateDetails(managerId, "FRT").ToList();
                List<User_Master> users = new List<User_Master>();
                List<User_Master> userslist = new List<User_Master>();
                users = BLUser.GetMMSubordinateDetails(managerId, "FRT").ToList();
                foreach (var new_user in users)
                {
                    User_Master user_ = new User_Master();
                    user_.user_id = new_user.user_id;
                    user_.name = MiscHelper.Decrypt(new_user.name);
                    user_.user_name = new_user.user_name;
                    userslist.Add(user_);
                }
                viewRouteIssueApprove.lstUserDetail = userslist;
                return PartialView("_AssignRouteIssues", viewRouteIssueApprove);
            }
            else
            {
                return PartialView("_ApproveIssueRoute", viewRouteIssueApprove);
            }
        }
        public ActionResult ContractorManagerRouteIssueApprove(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            //viewRouteIssueApprove.issueId = issueId;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            //int managerId = 397;

            //Need to be FRT than Patroller.
            //viewRouteIssueApprove.lstUserDetail = BLUser.GetSubordinateDetails(managerId, "FRT", intCircle_id).ToList();
            List<User_Master> users = new List<User_Master>();
            List<User_Master> userslist = new List<User_Master>();
            users = BLUser.GetSubordinateContractorDetails(managerId, "").ToList();
            foreach (var new_user in users)
            {
                User_Master user_ = new User_Master();
                user_.user_id = new_user.user_id;
                user_.name = MiscHelper.Decrypt(new_user.name);
                user_.user_name = new_user.user_name;
                user_.group_name = new_user.group_name;
                userslist.Add(user_);
            }
            viewRouteIssueApprove.lstUserDetail = userslist;
            return PartialView("_AssignContractorRouteIssues", viewRouteIssueApprove);


        }
        public ActionResult SaveRouteIssueStatus(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            ActionResult response = null;
            var issuesids = viewRouteIssueApprove.issuesId.Split(',');
            string jobstastus = "";
            for (int j = 0; j < issuesids.Length; j++)
            {


                var issuesId = Convert.ToInt32(issuesids[j]);
                var routeIssues = BLWFMTicket.GetHpsmidByRouteIssuesId(issuesId);
                if (routeIssues == null)
                {
                    var message = string.Format("No record found in job assignment");
                    response = Content("MSG:" + message);
                    return response;
                }
                var JobOrder = BLWFMTicket.GetHPSMTicket_DetailById(routeIssues.hpsm_ticketid ?? 0);
                jobstastus = jobstastus + JobOrder.hpsmid + ":";
                if (JobOrder == null)
                {
                    var message = string.Format("No job found");
                    response = Content("MSG:" + message);
                    return response;
                }
                SlotRequest SlotRequest = new SlotRequest();
                SlotConfirmation Request = BLWFMTicket.GetSlotConfirmation(JobOrder.bookingid);
                bool save = false;
                if (Request == null)
                {
                    var message = string.Format("No record found in confirm slot");
                    response = Content("MSG:" + message);
                    //   return response;
                }
                else
                {
                    SlotRequest = BLWFMTicket.GetSlotRequest(Request.referenceid);
                    if (SlotRequest == null)
                    {
                        var message = string.Format("No record found in slot request");
                        response = Content("MSG:" + message);
                        // return response;
                    }
                    else
                    {




                        string jo_type = "";
                        string service = "";

                        //var orderType = BLWFMTicket.GetOrderType(SlotRequest.order_type);
                        //if (orderType == null)
                        //{
                        //    var message = string.Format("Order type not found");
                        //    response = Content("MSG:" + message);
                        //    return response;
                        //}
                        //if (string.IsNullOrEmpty(orderType.description2))
                        //{
                        //    var message = string.Format("Jo type not map with selected order type : {0}", SlotRequest.order_type);
                        //    response = Content("MSG:" + message);
                        //    return response;
                        //}
                        //jo_type = orderType.description2;

                        //var serviceType = BLWFMTicket.Getservicetype(SlotRequest.service_type);

                        //if (serviceType == null)
                        //{
                        //   var message = string.Format("Service type not found");
                        //    response = Content("MSG:" + message);
                        //    return response;
                        //}
                        //if (string.IsNullOrEmpty(serviceType.add_service))
                        //{
                        //    serviceType.add_service = serviceType.remove_service;
                        //}
                        //if (string.IsNullOrEmpty(serviceType.add_service))
                        //{
                        //   var message = string.Format("Add/Remove service facility  not map with selected service type : {0}", SlotRequest.service_type);
                        //    response = Content("MSG:" + message);
                        //    return response;
                        //}
                        //service = serviceType.add_service;

                        //if (service.Contains(","))
                        //{
                        //    service = service.Split(',').First();
                        //}

                        ////var FEList = BLWFMTicket.GetFEList(SlotRequest.managerid).Where(f => f.UserId == viewRouteIssueApprove.frtUserId).ToList();
                        //var FEList = BLWFMTicket.GetFEDetailByUserId(viewRouteIssueApprove.frtUserId).ToList();

                        //if (FEList.Count == 0)
                        //{
                        //   var message = string.Format("No FE map with contractor");
                        //    response = Content("MSG:" + message);
                        //    return response;
                        //}
                        ////case 6
                        //var _isUserjotype = FEList.Where(j => j.transection.ToUpper() == jo_type.ToUpper()).ToList();
                        //if (FEList.Count == 0)
                        //{
                        //    var message = string.Format("user not map with jo type");
                        //    response = Content("MSG:" + message);
                        //    return response;
                        //}
                        ////case 7
                        //var _isUserService = _isUserjotype.Where(j => j.service.ToUpper() == service.ToUpper()).ToList();
                        //if (_isUserService.Count == 0)
                        //{
                        //   var message = string.Format("user not map with service");
                        //    response = Content("MSG:" + message);
                        //    return response;

                        //}
                        //var _isJoCategory = _isUserService.Where(j => j.jo_category_name.ToUpper() == SlotRequest.jo_category.ToUpper()).ToList();
                        //if (_isJoCategory.Count == 0)
                        //{
                        //    var message = string.Format("user not map with jo category");

                        //    response = Content("MSG:" + message);
                        //    return response;
                        //}

                        int dateDiff = -4;// Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MobileTimeDiff"]);
                        string toDate = ""; //to date is set as blank as this will include future tasks as well... //MiscHelper.FormatDate(DateTime.Now.ToString());
                        string fromDate = "";
                        var lstAssignedTaskDetail = BLWFMTicket.GetFRTTaskDetails(viewRouteIssueApprove.frtUserId, toDate, fromDate, "All", "");
                        var assignTicket = lstAssignedTaskDetail.Where(x => x.customer_appointment_date == SlotRequest.appointment_date.ToString("dd-MMM-yyyy").ToUpper()).ToList();
                        var cntBookin = 0;
                        if (assignTicket.Count > 0)
                        {
                            var res = assignTicket.Any(w => w.master_slot_id == Request.master_slot_id);
                            if (res)
                            {
                                cntBookin = BLWFMTicket.getrecordbyBookingId(JobOrder.bookingid);
                                if (cntBookin == 1)
                                {
                                    response = Content("MSG:This time slot is not available for selected FE.");
                                    //  return response;

                                }
                            }
                        }
                        if (cntBookin != 1)
                        {
                            VW_Route_Issue objRouteIssue = new VW_Route_Issue();

                            viewRouteIssueApprove.issuesId = Convert.ToString(issuesId);
                            viewRouteIssueApprove.user_id = Convert.ToInt32(Session["user_id"].ToString());
                            viewRouteIssueApprove.checkinRadius = ApplicationSettings.DefaultTaskCheckinRadius;
                            if (viewRouteIssueApprove.status == "Assigned" || viewRouteIssueApprove.status == "Re-Assigned")
                            {
                                if (viewRouteIssueApprove.status == "Assigned")
                                {


                                    List<Vw_Hpsm_Ticket_Status> hpsmTicketList = new List<Vw_Hpsm_Ticket_Status>();
                                    save = BLWFMTicket.AssignRouteIssue(viewRouteIssueApprove, out hpsmTicketList);
                                    //string IsHPSMCall = System.Configuration.ConfigurationManager.AppSettings["HPSMCall"].ToString();
                                    //if (!string.IsNullOrEmpty(IsHPSMCall))
                                    //{
                                    //    for (int index = 0; index < hpsmTicketList.Count; index++)
                                    //    {
                                    //        HPSMHelper hpsmHelper = new HPSMHelper(hpsmTicketList[index]);
                                    //    }
                                    //}

                                    try
                                    {
                                        NotificationHelper notificatonHelper = new NotificationHelper();
                                        var ids = viewRouteIssueApprove.issuesId.Split(',');
                                        for (int i = 0; i < ids.Length; i++)
                                        {

                                            //commenting because view does not exist 
                                            // notificatonHelper.sendNotification(viewRouteIssueApprove.frtUserId, Convert.ToInt32(ids[i]));
                                        }
                                    }
                                    catch (Exception ec)
                                    {
                                        //ErrorLogHelper logHelper = new ErrorLogHelper();
                                        //logHelper.ApiLogWriter("SaveRouteIssueStatus() + notificatonHelper", "Main Controller", reqData.data, ec);
                                    }
                                }
                                if (viewRouteIssueApprove.status == "Re-Assigned")
                                {
                                    List<Vw_Hpsm_Ticket_Status> hpsmTicketList = new List<Vw_Hpsm_Ticket_Status>();
                                    save = BLWFMTicket.AssignRouteIssue(viewRouteIssueApprove, out hpsmTicketList);
                                    //string IsHPSMCall = System.Configuration.ConfigurationManager.AppSettings["HPSMCall"].ToString();
                                    //if (!string.IsNullOrEmpty(IsHPSMCall))
                                    //{
                                    //    for (int index = 0; index < hpsmTicketList.Count; index++)
                                    //    {
                                    //        HPSMHelper hpsmHelper = new HPSMHelper(hpsmTicketList[index]);
                                    //    }
                                    //}

                                    try
                                    {
                                        NotificationHelper notificatonHelper = new NotificationHelper();
                                        var ids = viewRouteIssueApprove.issuesId.Split(',');
                                        for (int i = 0; i < ids.Length; i++)
                                        {
                                            notificatonHelper.sendNotification(viewRouteIssueApprove.frtUserId, Convert.ToInt32(ids[i]));
                                        }
                                    }
                                    catch (Exception ec)
                                    {
                                        //ErrorLogHelper logHelper = new ErrorLogHelper();
                                        //logHelper.ApiLogWriter("SaveRouteIssueStatus() + notificatonHelper", "Main Controller", reqData.data, ec);
                                    }
                                }


                            }
                            else
                            {
                                save = BLWFMTicket.UpdateRouteIssueStatus(viewRouteIssueApprove);
                            }
                        }
                    }
                }

                if (save)
                {
                    jobstastus = jobstastus + " Success - Job assigned successfully!!<br>";
                    //  response = Content("Success_UpdateStatus_" + viewRouteIssueApprove.status + "");
                    // response = Content(jobstastus);
                }
                else
                {
                    jobstastus = jobstastus + " Fail - " + ((ContentResult)response).Content + "<br>";
                    // response = Content("Error_UpdateStatus");


                }
            }
            response = Content(jobstastus);
            return response;
        }






        public ActionResult AssingContractorRouteIssue(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            VW_Route_Issue objRouteIssue = new VW_Route_Issue();
            bool save = false;
            ActionResult response = null;
            viewRouteIssueApprove.user_id = Convert.ToInt32(Session["user_id"].ToString());
            save = BLWFMTicket.AssingContractorRouteIssue(viewRouteIssueApprove);
            if (save)
            {
                response = Content("Success_UpdateStatus_" + viewRouteIssueApprove.status + "");
            }
            else
            {
                response = Content("Error_UpdateStatus");
            }
            return response;
        }
        [HttpGet]
        public void DownloadRouteAssignedReport()
        {
            if (Session["viewAssignedRouteFilter"] != null)
            {
                ViewAssignJobOrder objViewAssignedRouteFilter = (ViewAssignJobOrder)Session["viewAssignedRouteFilter"];

                // ViewAssignedRoute objViewAssignedRoute = new ViewAssignedRoute();
                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;

                //objViewAssignedRoute.viewAssignedRouteFilter = objViewAssignedRouteFilter;
                objViewAssignedRouteFilter.viewAssignJobOrderFilter.currentPage = 0;
                objViewAssignedRouteFilter.viewAssignJobOrderFilter.pageSize = 0;

                BLJobOrder.GetAssignedRouteDetails(objViewAssignedRouteFilter, out totalRecords, isExport);


                dtReport = objViewAssignedRouteFilter.viewAssignJobOrderFilter.selectedAssignType == 1 ? MiscHelper.ListToDataTable<RouteAssignedReport>(objViewAssignedRouteFilter.lstRouteAssignedReport) : MiscHelper.ListToDataTable<RouteScheduledReport>(objViewAssignedRouteFilter.lstRouteScheduledReport);
                //below column are not required in report..
                if (objViewAssignedRouteFilter.viewAssignJobOrderFilter.selectedAssignType == 1)
                {
                    dtReport.Columns.Remove("ROUTE_ASSIGNED_ID");
                }
                else
                {
                    dtReport.Columns.Remove("SCHEDULED_ID");
                }
                ///

                string fileName = objViewAssignedRouteFilter.viewAssignJobOrderFilter.selectedAssignType == 1 ? "AssignedReport" : "ScheduledReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }
        [HttpGet]
        public void DownloadRouteAssignIssueReport()
        {
            if (Session["viewAssignRouteIssueFilter"] != null)
            {
                ViewAssignJobOrderFilter objAssignRouteIssueFilter = (ViewAssignJobOrderFilter)Session["viewAssignRouteIssueFilter"];

                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;

                //  objViewRouteAssignIssue.viewAssignJobOrderFilter = objAssignRouteIssueFilter;
                objAssignRouteIssueFilter.currentPage = 0;
                List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAssignJobOrderDetails(objAssignRouteIssueFilter, out totalRecords);
                if (lstAssignIssueDetails.Count > 0)
                {
                    lstAssignIssueDetails.Select(c =>
                    {
                        c.ticket_source = _Ticket_Source[c.ticket_source_id];
                        return c;
                    }).ToList();
                }
                dtReport = MiscHelper.ListToDataTable<VW_Assign_Job_Order>(lstAssignIssueDetails);

                if (dtReport.Rows.Count > 0)
                {

                    dtReport.Columns.Remove("ISSUE_ID");
                    dtReport.Columns.Remove("task_tracking_id");
                    dtReport.Columns.Remove("vsf_ticketid");
                    dtReport.Columns.Remove("patroller_user_name");
                    dtReport.Columns.Remove("patroller_remark");
                    dtReport.Columns.Remove("modified_on");
                    dtReport.Columns.Remove("CREATED_ON");
                    dtReport.Columns.Remove("ADDRESSID");
                    dtReport.Columns.Remove("CLUSTER_ID");
                    dtReport.Columns.Remove("CLUSTER_NAME");
                    dtReport.Columns.Remove("SOCIETY_NAME");
                    dtReport.Columns.Remove("CUSTOMER_ADD");
                    dtReport.Columns.Remove("CIRCLE_NAME");
                    dtReport.Columns.Remove("is_attachment");
                    dtReport.Columns.Remove("TICKET_SOURCE");
                    dtReport.Columns.Remove("hold_rc_code");
                    dtReport.Columns.Remove("hold_rca_code");
                    dtReport.Columns.Remove("hold_remarks");
                    dtReport.Columns.Remove("hold_appointment_date");
                    dtReport.Columns.Remove("rc");
                    dtReport.Columns.Remove("cpe_type");
                    dtReport.Columns.Remove("cpe_portno");
                    dtReport.Columns.Remove("cpe_model");
                    dtReport.Columns.Remove("local_convergence_point");
                    dtReport.Columns.Remove("current_cpesn");
                    dtReport.Columns.Remove("issue_comment");
                    dtReport.Columns.Remove("cpe_mac_address");
                    dtReport.Columns.Remove("customer_category");
                    dtReport.Columns.Remove("customer_segment");
                    dtReport.Columns.Remove("bandwidth");
                    dtReport.Columns.Remove("tasktype");
                    dtReport.Columns.Remove("taskcategory");
                    dtReport.Columns.Remove("tasksubcategory");

                    dtReport.Columns.Remove("checkout_remarks");
                    dtReport.Columns.Remove("checkin_remarks");

                }

                string fileName = "ViewAssignedJobOrderReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }
        private void ExportData(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }


        public void DownloadRouteIssueReport()
        {
            if (Session["viewIssueRouteFilter"] != null)
            {
                ViewJobOrderFilter objviewIssueRouteFilter = (ViewJobOrderFilter)Session["viewIssueRouteFilter"];

                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;
                objviewIssueRouteFilter.currentPage = 0;
                List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetJobOrderDetails(objviewIssueRouteFilter, out totalRecords);
                if (lstrouteIssues.Count > 0)
                {
                    lstrouteIssues.Select(c =>
                    {
                        c.ticket_source = _Ticket_Source[c.ticket_source_id];
                        return c;
                    }).ToList();
                }
                dtReport = MiscHelper.ListToDataTable<VW_Route_Issue>(lstrouteIssues);

                if (dtReport.Rows.Count > 0)
                {

                    //Rename columns..
                    dtReport.Columns["ISSUE_DESC"].ColumnName = "Service_Type";
                    dtReport.Columns["HPSM_TICKETID"].ColumnName = "JOB_ID";

                    dtReport.Columns.Remove("ISSUE_ID");
                    dtReport.Columns.Remove("SUB_STATUS");
                    dtReport.Columns.Remove("ROUTE_NAME");

                    dtReport.Columns.Remove("MOBILE_TIME");

                    dtReport.Columns.Remove("VSF_TICKETID");
                    dtReport.Columns.Remove("CUSTOMER_ID");
                    dtReport.Columns.Remove("CUSTOMER_ADD");
                    dtReport.Columns.Remove("TICKET_SOURCE_ID");

                    dtReport.Columns.Remove("CIRCLE_NAME");
                    dtReport.Columns.Remove("CLUSTER_ID");
                    dtReport.Columns.Remove("CLUSTER_NAME");
                    dtReport.Columns.Remove("SOCIETY_NAME");
                    dtReport.Columns.Remove("IMPACTTYPE");
                    dtReport.Columns.Remove("NETWORKENTITY");
                    dtReport.Columns.Remove("POP_NAME");
                    dtReport.Columns.Remove("TICKETTYPE");
                    dtReport.Columns.Remove("CUSTOMERCOUNT");
                    dtReport.Columns.Remove("LOCATION");
                    dtReport.Columns.Remove("BOUNDARY");


                    dtReport.Columns.Remove("manager_remark");

                    dtReport.Columns.Remove("hpsmid");
                    dtReport.Columns.Remove("cpe_type");
                    dtReport.Columns.Remove("cpe_portno");
                    dtReport.Columns.Remove("cpe_brand");

                    dtReport.Columns.Remove("cpe_model");
                    dtReport.Columns.Remove("local_convergence_point");
                    dtReport.Columns.Remove("current_cpesn");
                    dtReport.Columns.Remove("issue_comment");
                    dtReport.Columns.Remove("site_type");
                    dtReport.Columns.Remove("cpe_mac_address");
                    dtReport.Columns.Remove("customer_category");
                    dtReport.Columns.Remove("customer_segment");
                    dtReport.Columns.Remove("bandwidth");

                    dtReport.Columns.Remove("tasktype");
                    dtReport.Columns.Remove("taskcategory");
                    dtReport.Columns.Remove("tasksubcategory");
                }

                string fileName = "AssignedJobOrderReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }

        [HttpPost]
        public PartialViewResult GetAssignJobOrderRM(ViewJobOrder objviewJobOrder, int page = 0, string sort = "", string sortdir = "", string status = "")
        {
            int totalRecords = 0;
            //int managerId =486;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            BLUser objBLuser = new BLUser();
            BLRoles objBLroles = new BLRoles();
            User objUserDetails = objBLuser.getUserDetails(managerId);
            RoleMaster objRoleMaster = objBLroles.getUserRoleNameByRoleId(objUserDetails.role_id);
            List<User_Master> lstRegionalHead = null;
            List<User_Master> lstGroupDetail = null;
            List<User_Master> lstJoManagerDetail = null;
            if (objRoleMaster.role_name == "Regional Head")
            {
                objviewJobOrder.vwJobOrderFilter.isLoginRole = 2;

                lstJoManagerDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();
                lstGroupDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.jomanagerId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.jomanagerId, "").ToList();
            }
            else if (objRoleMaster.role_name == "CEO")
            {
                //objviewJobOrder.vwJobOrderFilter.isCeoRole = true;
                objviewJobOrder.vwJobOrderFilter.isLoginRole = 1;
                lstRegionalHead = BLUser.GetMMSubordinateDetails(managerId, "").ToList();
                lstJoManagerDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.regionalHeadId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.regionalHeadId, "").ToList();
                lstGroupDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.jomanagerId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.jomanagerId, "").ToList();
            }
            else
            {
                lstGroupDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();

            }
            // var lstSubourdinateDetail = BLUser.GetSubordinateDetails(managerId, "").ToList();
            var lstSubourdinateDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.groupmanagerId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.groupmanagerId, "").ToList();

            if (string.IsNullOrEmpty(objviewJobOrder.vwJobOrderFilter.status))
            {
                objviewJobOrder.vwJobOrderFilter.status = status;
            }

            objviewJobOrder.lstJoType = BLJobOrder.GetJoType();
            objviewJobOrder.lstServiceType = BLJobOrder.GetServicesType();
            objviewJobOrder.lstJoCategory = BLJobOrder.GetJoCategory();
            objviewJobOrder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_assign_status.ToString()).ToList();
            objviewJobOrder.vwJobOrderFilter.userId = managerId;
            objviewJobOrder.vwJobOrderFilter.pageSize = objviewJobOrder.vwJobOrderFilter.pageSize == 0 ? 10 : objviewJobOrder.vwJobOrderFilter.pageSize;
            objviewJobOrder.vwJobOrderFilter.fromDate = MiscHelper.FormatDate(objviewJobOrder.vwJobOrderFilter.fromDate);
            objviewJobOrder.vwJobOrderFilter.toDate = MiscHelper.FormatDate(objviewJobOrder.vwJobOrderFilter.toDate);
            objviewJobOrder.vwJobOrderFilter.page = page;
            objviewJobOrder.vwJobOrderFilter.sort = sort;
            objviewJobOrder.vwJobOrderFilter.sortdir = sortdir;
            objviewJobOrder.vwJobOrderFilter.currentPage = page == 0 ? 1 : page;
            //objviewJobOrder.vwJobOrderFilter.subordinateUserId = page == 0 ? 1 : page;
            //objviewJobOrder.lstUserDetail = new BLUser().GetAllActiveUsersList().ToList();
            objviewJobOrder.lstJoDetailDetails = lstJoManagerDetail;
            objviewJobOrder.lstGroupDetails = lstGroupDetail;
            objviewJobOrder.lstRegionalHead = lstRegionalHead;
            objviewJobOrder.lstUserDetail = lstSubourdinateDetail;
            List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetJobOrderDetailsRM(objviewJobOrder.vwJobOrderFilter, out totalRecords);

            if (lstrouteIssues.Count > 0)
            {
                lstrouteIssues.Select(c =>
                {
                    c.ticket_source = _Ticket_Source[c.ticket_source_id];
                    return c;
                }).ToList();
            }
            objviewJobOrder.lstIssueDetails = lstrouteIssues;
            objviewJobOrder.vwJobOrderFilter.totalRecord = totalRecords;

            Session["viewIssueRouteFilter"] = objviewJobOrder.vwJobOrderFilter;

            return PartialView("_AssignJobOrderRM", objviewJobOrder);
        }
        [HttpPost]
        public ActionResult ViewAssignJobOrderRM(ViewAssignJobOrder objViewAssignJobOrder, int page = 0, string sort = "", string sortdir = "")
        {
            int totalRecords = 0;
            int cictId = Convert.ToInt32(Session["user_id"].ToString());
            BLUser objBLuser = new BLUser();
            BLRoles objBLroles = new BLRoles();
            User objUserDetails = objBLuser.getUserDetails(cictId);
            RoleMaster objRoleMaster = objBLroles.getUserRoleNameByRoleId(objUserDetails.role_id);
            List<User_Master> lstRegionalHead = null;
            List<User_Master> lstGroupDetail = null;
            List<User_Master> lstJoManagerDetail = null;
            if (objRoleMaster.role_name == "Regional Head")
            {
                objViewAssignJobOrder.viewAssignJobOrderFilter.isLoginRole = 2;
                lstJoManagerDetail = BLUser.GetMMSubordinateDetails(cictId, "").ToList();
                lstGroupDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.jomanagerId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.jomanagerId, "").ToList();
            }
            else if (objRoleMaster.role_name == "CEO")
            {
                objViewAssignJobOrder.viewAssignJobOrderFilter.isLoginRole = 1;
                lstRegionalHead = BLUser.GetMMSubordinateDetails(cictId, "").ToList();
                lstJoManagerDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.regionalHeadId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.regionalHeadId, "").ToList();
                lstGroupDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.jomanagerId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.jomanagerId, "").ToList();

            }
            else
            {
                lstGroupDetail = BLUser.GetMMSubordinateDetails(cictId, "").ToList();

            }
            //lstGroupDetail = BLUser.GetSubordinateDetails(cictId, "").ToList();
            var lstContractorDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.groupmanagerId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.groupmanagerId, "").ToList();
            var lstFEUserDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.managerId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.managerId, "").ToList();
            objViewAssignJobOrder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_view_status.ToString()).ToList();
            //objViewAssignJobOrder.lstIssueRaisedByDetail = lstUserDetail;
            objViewAssignJobOrder.lstIssueAssignedToDetail = lstFEUserDetail;
            objViewAssignJobOrder.lstJoType = BLJobOrder.GetJoType();
            objViewAssignJobOrder.lstServiceType = BLJobOrder.GetServicesType();
            objViewAssignJobOrder.lstJoCategory = BLJobOrder.GetJoCategory();
            objViewAssignJobOrder.viewAssignJobOrderFilter.userId = cictId;
            objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize = objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize == 0 ? 10 : objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize;
            objViewAssignJobOrder.viewAssignJobOrderFilter.fromDate = MiscHelper.FormatDate(objViewAssignJobOrder.viewAssignJobOrderFilter.fromDate);
            objViewAssignJobOrder.viewAssignJobOrderFilter.toDate = MiscHelper.FormatDate(objViewAssignJobOrder.viewAssignJobOrderFilter.toDate);
            objViewAssignJobOrder.viewAssignJobOrderFilter.page = page;
            objViewAssignJobOrder.viewAssignJobOrderFilter.sort = sort;
            objViewAssignJobOrder.viewAssignJobOrderFilter.sortdir = sortdir;
            objViewAssignJobOrder.viewAssignJobOrderFilter.currentPage = page == 0 ? 1 : page;
            objViewAssignJobOrder.lstJoDetailDetails = lstJoManagerDetail;
            objViewAssignJobOrder.lstGroupDetails = lstGroupDetail;
            objViewAssignJobOrder.lstCantractorDetails = lstContractorDetail;
            objViewAssignJobOrder.lstRegionalHead = lstRegionalHead;
            objViewAssignJobOrder.statusCount = BLJobOrder.GetViewRMJobOrderStatusCount(objViewAssignJobOrder.viewAssignJobOrderFilter);

            List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAssignJobOrderDetailsRM(objViewAssignJobOrder.viewAssignJobOrderFilter, out totalRecords);

            lstAssignIssueDetails.Select(c =>
            {
                c.ticket_source = _Ticket_Source[c.ticket_source_id];
                return c;
            }).ToList();

            objViewAssignJobOrder.lstAssignJobOrderDetails = lstAssignIssueDetails;

            objViewAssignJobOrder.viewAssignJobOrderFilter.totalRecord = totalRecords;
            Session["viewAssignRouteIssueFilter"] = objViewAssignJobOrder.viewAssignJobOrderFilter;


            return PartialView("_ViewAssignJobOrderRM", objViewAssignJobOrder);
        }

        public JsonResult BindReportingManagerOnChange(int ContractorId)
        {
            var objResp = BLUser.GetMMSubordinateDetails(ContractorId, "");
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public void DownloadRouteIssueReportRM()
        {
            if (Session["viewIssueRouteFilter"] != null)
            {
                ViewJobOrderFilter objviewIssueRouteFilter = (ViewJobOrderFilter)Session["viewIssueRouteFilter"];

                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;
                objviewIssueRouteFilter.currentPage = 0;
                List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetJobOrderDetailsRM(objviewIssueRouteFilter, out totalRecords);
                if (lstrouteIssues.Count > 0)
                {
                    lstrouteIssues.Select(c =>
                    {
                        c.ticket_source = _Ticket_Source[c.ticket_source_id];
                        return c;
                    }).ToList();
                }
                dtReport = MiscHelper.ListToDataTable<VW_Route_Issue>(lstrouteIssues);

                if (dtReport.Rows.Count > 0)
                {

                    //Rename columns..
                    dtReport.Columns["ISSUE_DESC"].ColumnName = "Service_Type";
                    dtReport.Columns["HPSM_TICKETID"].ColumnName = "JOB_ID";
                    dtReport.Columns.Remove("ISSUE_ID");
                    dtReport.Columns.Remove("SUB_STATUS");
                    dtReport.Columns.Remove("ROUTE_NAME");
                    dtReport.Columns.Remove("MOBILE_TIME");
                    dtReport.Columns.Remove("VSF_TICKETID");
                    dtReport.Columns.Remove("CUSTOMER_ID");
                    dtReport.Columns.Remove("CUSTOMER_ADD");
                    dtReport.Columns.Remove("TICKET_SOURCE_ID");
                    dtReport.Columns.Remove("CIRCLE_NAME");
                    dtReport.Columns.Remove("CLUSTER_ID");
                    dtReport.Columns.Remove("CLUSTER_NAME");
                    dtReport.Columns.Remove("SOCIETY_NAME");
                    dtReport.Columns.Remove("IMPACTTYPE");
                    dtReport.Columns.Remove("NETWORKENTITY");
                    dtReport.Columns.Remove("POP_NAME");
                    dtReport.Columns.Remove("TICKETTYPE");
                    dtReport.Columns.Remove("CUSTOMERCOUNT");
                    dtReport.Columns.Remove("LOCATION");
                    dtReport.Columns.Remove("BOUNDARY");
                    dtReport.Columns.Remove("HPSMID");
                    dtReport.Columns.Remove("modified_on");
                    dtReport.Columns.Remove("cpe_mac_address");
                    dtReport.Columns.Remove("ticket_source");
                    dtReport.Columns.Remove("cpe_type");
                    dtReport.Columns.Remove("cpe_portno");
                    dtReport.Columns.Remove("cpe_model");
                    dtReport.Columns.Remove("local_convergence_point");
                    dtReport.Columns.Remove("current_cpesn");
                    dtReport.Columns.Remove("issue_comment");
                    dtReport.Columns.Remove("site_type");
                    dtReport.Columns.Remove("addressid");
                    dtReport.Columns.Remove("is_attachment");
                    dtReport.Columns.Remove("customer_category");
                    dtReport.Columns.Remove("customer_segment");
                    dtReport.Columns.Remove("bandwidth");
                    dtReport.Columns.Remove("tasktype");
                    dtReport.Columns.Remove("taskcategory");
                    dtReport.Columns.Remove("tasksubcategory");
                    dtReport.Columns.Remove("ip");
                    dtReport.Columns.Remove("gw");
                    dtReport.Columns.Remove("sm");
                    dtReport.Columns.Remove("dns");
                    dtReport.Columns.Remove("adns");
                }

                string fileName = "AssignedJobOrderReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }

        public void DownloadRouteAssignIssueReportRM()
        {
            if (Session["viewAssignRouteIssueFilter"] != null)
            {
                ViewAssignJobOrderFilter objAssignRouteIssueFilter = (ViewAssignJobOrderFilter)Session["viewAssignRouteIssueFilter"];

                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;

                //  objViewRouteAssignIssue.viewAssignJobOrderFilter = objAssignRouteIssueFilter;
                objAssignRouteIssueFilter.currentPage = 0;
                List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAssignJobOrderDetailsRM(objAssignRouteIssueFilter, out totalRecords);
                if (lstAssignIssueDetails.Count > 0)
                {
                    lstAssignIssueDetails.Select(c =>
                    {
                        c.ticket_source = _Ticket_Source[c.ticket_source_id];
                        return c;
                    }).ToList();
                }
                dtReport = MiscHelper.ListToDataTable<VW_Assign_Job_Order>(lstAssignIssueDetails);

                if (dtReport.Rows.Count > 0)
                {

                    //Rename columns..
                    dtReport.Columns["HPSMID"].ColumnName = "JOB ID";
                    dtReport.Columns["ISSUE_DESC"].ColumnName = "Service Type";
                    dtReport.Columns.Remove("ISSUE_ID");
                    dtReport.Columns.Remove("task_tracking_id");
                    dtReport.Columns.Remove("vsf_ticketid");
                    dtReport.Columns.Remove("patroller_user_name");
                    dtReport.Columns.Remove("patroller_remark");
                    dtReport.Columns.Remove("modified_on");
                    dtReport.Columns.Remove("CREATED_ON");
                    dtReport.Columns.Remove("ADDRESSID");
                    dtReport.Columns.Remove("HPSM_TICKETID");
                    dtReport.Columns.Remove("CLUSTER_ID");
                    dtReport.Columns.Remove("CLUSTER_NAME");
                    dtReport.Columns.Remove("SOCIETY_NAME");
                    dtReport.Columns.Remove("CUSTOMER_ADD");
                    dtReport.Columns.Remove("TICKET_SOURCE_ID");
                    dtReport.Columns.Remove("CIRCLE_NAME");
                    dtReport.Columns.Remove("checkin_remarks");
                    dtReport.Columns.Remove("checkout_remarks");
                    dtReport.Columns.Remove("hold_rc_code");
                    dtReport.Columns.Remove("hold_rca_code");
                    dtReport.Columns.Remove("hold_remarks");
                    dtReport.Columns.Remove("hold_appointment_date");
                    dtReport.Columns.Remove("cpe_type");
                    dtReport.Columns.Remove("cpe_portno");
                    dtReport.Columns.Remove("cpe_model");
                    dtReport.Columns.Remove("local_convergence_point");
                    dtReport.Columns.Remove("current_cpesn");
                    dtReport.Columns.Remove("issue_comment");
                    dtReport.Columns.Remove("site_type");
                    //dtReport.Columns.Remove("addressid");
                    dtReport.Columns.Remove("cpe_mac_address");
                    dtReport.Columns.Remove("is_attachment");
                    dtReport.Columns.Remove("customer_category");
                    dtReport.Columns.Remove("customer_segment");
                    dtReport.Columns.Remove("bandwidth");
                    dtReport.Columns.Remove("tasktype");
                    dtReport.Columns.Remove("taskcategory");
                    dtReport.Columns.Remove("ip");
                    dtReport.Columns.Remove("gw");
                    dtReport.Columns.Remove("sm");
                    dtReport.Columns.Remove("dns");
                    dtReport.Columns.Remove("adns");
                }

                string fileName = "ViewAssignedJobOrderReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }


        //public PartialViewResult ViewHPSMJobAttachmentsDeatail(ViewHPSMTicketDetails model, string id, int page = 0, string sort = "", string sortdir = "", string status = "")
        public PartialViewResult ViewHPSMJobAttachmentsDeatail(string id, string status = "")
        {
            //id = "JOB002";
            var lstDocument = BLWFMTicket.getAttachmentDetailsbyJobId(id);
            foreach (var item in lstDocument)
            {
                if (item.upload_type.Equals("image", StringComparison.OrdinalIgnoreCase))
                {
                    item.ImgSrcThumb = convertBase64Image(item.file_location, "Thumb_" + item.file_name);
                }
            }
            //lstDocument = lstDocument.Where(x => x.upload_type.Equals("image", StringComparison.OrdinalIgnoreCase)).ToList<vw_hpsm_ticket_attachments>();
            return PartialView("_ViewHPSMTicketAttachmentDetail", lstDocument);
        }

        //public ActionResult GetAttachmentDetailsbyId(int id)
        //{
        //    string strBase64Image = string.Empty;

        //    var item = BLWFMTicket.getAttachmentDetailsbyId(id);

        //    if (item.upload_type.ToUpper() == "IMAGE")
        //    {
        //        strBase64Image = convertBase64Image(item.file_location, "Thumb_" + item.file_name);
        //        //strBase64Image = convertBase64Image(item.file_location, item.file_name);
        //    }
        //    return Content(strBase64Image);
        //}

        public ActionResult GetAttachmentDetailsbyId(int id, string doctype = "")
        {
            string strBase64Image = string.Empty;
            var item = BLWFMTicket.getAttachmentDetailsbyId(id);
            string returnType = "image/png";
            //if (item.upload_type.ToUpper() == "IMAGE")
            //{
            //    strBase64Image = convertBase64Image(item.file_location, "Thumb_" + item.file_name);
            //    //strBase64Image = convertBase64Image(item.file_location, item.file_name);
            //}
            //byte[] b= GetByteImage(item.file_location, "Thumb_" + item.file_name);
            byte[] b = null;
            if (doctype == "imgthumb")
            {
                returnType = "image/png";
                b = GetAttachmentBytesFile(item.file_location, "Thumb_" + item.file_name);
            }
            else if (doctype == "img")
            {
                returnType = "image/png";
                b = GetAttachmentBytesFile(item.file_location, item.file_name);
            }
            else if (doctype == "xlsx")
            {
                returnType = "application/ms-excel";
                b = GetAttachmentBytesFile(item.file_location, item.file_name);
                return File(b, returnType, item.file_name);
            }
            else if (doctype == "pdf")
            {
                returnType = "application/pdf";
                b = GetAttachmentBytesFile(item.file_location, item.file_name);
                return File(b, returnType, item.file_name);
            }
            else if (doctype == "docx")
            {
                returnType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                b = GetAttachmentBytesFile(item.file_location, item.file_name);
                return File(b, returnType, item.file_name);
            }
            return File(b, returnType);
        }

        private string convertBase64Image(string file_location, string file_name)
        {
            var _imgSrc = "";
            string imageUrl = string.Empty;
            string FtpUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"]);
            string strFTPUserName = System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"];
            string strFTPPassWord = System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"];
            imageUrl = string.Concat(FtpUrl, file_location, file_name);
            if (!isFileExistOnFTP(imageUrl))
            {
                imageUrl = string.Concat(FtpUrl, file_location, file_name);
            }
            //string imageUrl = string.Concat(FtpUrl, item.file_location,item.file_name);
            WebClient request = new WebClient();
            if (!string.IsNullOrEmpty(strFTPUserName)) //Authentication require..
                request.Credentials = new NetworkCredential(strFTPUserName, strFTPPassWord);

            byte[] objdata = null;
            objdata = request.DownloadData(imageUrl);
            if (objdata != null && objdata.Length > 0)
                _imgSrc = string.Concat("data:image/png;base64,", Convert.ToBase64String(objdata));

            return _imgSrc;
        }

        private byte[] GetAttachmentBytesFile(string file_location, string file_name)
        {
            byte[] objdata = null;
            try
            {
                string imageUrl = string.Empty;
                string FtpUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"]);
                string strFTPUserName = System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"];
                imageUrl = string.Concat(FtpUrl, file_location, file_name);
                if (!isFileExistOnFTP(imageUrl))
                {
                    imageUrl = string.Concat(FtpUrl, file_location, file_name);
                }
                //string imageUrl = string.Concat(FtpUrl, item.file_location,item.file_name);
                WebClient request = new WebClient();
                if (!string.IsNullOrEmpty(strFTPUserName)) //Authentication require..
                    request.Credentials = new NetworkCredential(strFTPUserName, strFTPPassWord);


                objdata = request.DownloadData(imageUrl);
            }
            catch (Exception ex)
            {

            }
            return objdata;
        }
        public bool isFileExistOnFTP(string filepath)
        {
            var request = (FtpWebRequest)WebRequest.Create(filepath);
            string UserName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
            request.Credentials = new NetworkCredential(UserName, PassWord);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode ==
                    FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
                return false;
            }

        }


        [HttpPost]
        public ActionResult GetServiceDetailByUserId(int userid)
        {
            UserServiceDetail ServiceDetail = null;
            ServiceDetail = BLWFMTicket.GetServiceDetailByUserId(userid);
            return Json(ServiceDetail != null ? ServiceDetail : new UserServiceDetail(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult jobViewUserTimeSheet(string id)
        {
            Task obj = new Task();
            //UserTimeSheetInput objIn = new UserTimeSheetInput();


            if (id != null)
            {
                obj = BusinessLogics.WFM.BLWFMTicket.Getjobdetails(id);
            }



            return PartialView("jobViewUserTimeSheet", obj);

        }



        public ActionResult upDATEjobViewUserTimeSheet(Task obj)
        {

            ActionResult response = null;
            int result = BusinessLogics.WFM.BLWFMTicket.newEditTimeSheet(obj);
            if (result > 0)
            {
                response = Content("Success_UpdateStatus_SUpdate");
            }
            else
            {
                response = Content("Error_UpdateStatus");
            }

            return response;

        }

        public ActionResult ViewNotification(ViewNotification objViewNotification, int page = 0, string sort = "", string sortdir = "", string status = "")
        {
            wfm_notification obj = new wfm_notification();
            //UserTimeSheetInput objIn = new UserTimeSheetInput();
            int totalRecords = 0;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());

            string role = objViewNotification.viewNotificationFilter.roleId == 0 || objViewNotification.viewNotificationFilter.roleId == 3 ? "Patroller" : "FRT";

            //if (string.IsNullOrEmpty(objViewNotification.viewNotificationFilter.status))
            //{
            //    objViewNotification.viewNotificationFilter.status = status;
            //}


            objViewNotification.viewNotificationFilter.managerId = managerId;
            objViewNotification.viewNotificationFilter.pageSize = 5;
            objViewNotification.viewNotificationFilter.fromDate = MiscHelper.FormatDate(objViewNotification.viewNotificationFilter.fromDate);
            objViewNotification.viewNotificationFilter.toDate = MiscHelper.FormatDate(objViewNotification.viewNotificationFilter.toDate);


            objViewNotification.viewNotificationFilter.currentPage = page == 0 ? 1 : page;
            objViewNotification.viewNotificationFilter.sort = sort;
            objViewNotification.viewNotificationFilter.sortdir = sortdir;


            List<wfm_notification> lstNotificationDetail = BLUser.GetNotificationDetail(objViewNotification.viewNotificationFilter, out totalRecords);

            objViewNotification.lstNotificationDetail = lstNotificationDetail;
            objViewNotification.viewNotificationFilter.totalRecord = totalRecords;

            return PartialView("_ViewNotification", objViewNotification);


        }
        public PartialViewResult AssignttJobOrder(ViewJobOrder objviewJobOrder, int page = 0, string sort = "", string sortdir = "", string status = "")
        {
            int totalRecords = 0;
            //int managerId =486;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            var lstSubourdinateDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();

            if (string.IsNullOrEmpty(objviewJobOrder.vwJobOrderFilter.status))
            {
                objviewJobOrder.vwJobOrderFilter.status = status;
            }

            objviewJobOrder.lstCustomerCategory = BLJobOrder.GetCustomerCategory();
            objviewJobOrder.lstCustomerSegment = BLJobOrder.GetCustomerSegment();
            objviewJobOrder.lstcategory = BLJobOrder.Getttcatgeory();
            //Models.User objUserDetails = objBLuser.getUserDetails(managerId);
            objviewJobOrder.lsttype = BLJobOrder.Getttype(string.IsNullOrEmpty(objviewJobOrder.vwJobOrderFilter.ttcategory) ? "" : objviewJobOrder.vwJobOrderFilter.ttcategory);




            objviewJobOrder.lststatus = BLJobOrder.GetDropDownListtt(DropDownType.JO_assign_status.ToString()).ToList();
            objviewJobOrder.vwJobOrderFilter.userId = managerId;
            objviewJobOrder.vwJobOrderFilter.pageSize = objviewJobOrder.vwJobOrderFilter.pageSize == 0 ? 10 : objviewJobOrder.vwJobOrderFilter.pageSize;
            objviewJobOrder.vwJobOrderFilter.fromDate = MiscHelper.FormatDate(objviewJobOrder.vwJobOrderFilter.fromDate);
            objviewJobOrder.vwJobOrderFilter.toDate = MiscHelper.FormatDate(objviewJobOrder.vwJobOrderFilter.toDate);
            objviewJobOrder.vwJobOrderFilter.page = page;
            objviewJobOrder.vwJobOrderFilter.sort = sort;
            objviewJobOrder.vwJobOrderFilter.sortdir = sortdir;
            objviewJobOrder.vwJobOrderFilter.currentPage = page == 0 ? 1 : page;
            //objviewJobOrder.lstUserDetail = new BLUser().GetAllActiveUsersList().ToList();
            objviewJobOrder.lstUserDetail = lstSubourdinateDetail;
            List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetJobOrderDetailstt(objviewJobOrder.vwJobOrderFilter, out totalRecords);

            if (lstrouteIssues.Count > 0)
            {
                lstrouteIssues.Select(c =>
                {
                    c.ticket_source = _Ticket_Source[c.ticket_source_id];
                    return c;
                }).ToList();
            }
            objviewJobOrder.lstIssueDetails = lstrouteIssues;
            objviewJobOrder.vwJobOrderFilter.totalRecord = totalRecords;

            Session["viewIssueRouteFilter"] = objviewJobOrder.vwJobOrderFilter;

            return PartialView("_viewAssignttJobOrder", objviewJobOrder);
        }
        public ActionResult OpenTicket(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {

            return PartialView("_closeticket", viewRouteIssueApprove);

        }

        public ActionResult Closeticket(ViewManagerRouteIssueApprove ViewManagerRouteIssueApprove)
        {
            try
            {
                ActionResult response = null;
                var responsesdata = new Models.API.WFMMobileApiResponse<dynamic>();

                //bool save = false;
                int res = 0;
                Route_Issue obj = new Route_Issue();
                Task obj1 = new Task();
                obj = BLWFMTicket.nEditTimeSheet(ViewManagerRouteIssueApprove.issueId);
                obj1 = BLWFMTicket.GetjobdetailsbyId(obj.hpsm_ticketid);
                job_order_status objstatus = new job_order_status();
                objstatus.job_id = obj1.hpsmid;
                objstatus.action = "Completed";
                objstatus.remarks = "Completed by web module";
                var iscpeactivateapi = Convert.ToBoolean(ConfigurationManager.AppSettings["iscpeactivateapi"]);
                string msg = "Job order completed successfuly.";

                if (obj1.facility == "IPTV" || obj1.facility == "IPTV_EXTN")
                {
                    if (obj1.cpe_serialno != "" && obj1.cpe_serialno != null)
                    {
                        if (iscpeactivateapi)
                        {
                            var tokenResponse = new Models.API.WFMMobileApiResponse<dynamic>();
                            //Call closer Api
                            // tokenResponse = getTriggerActivateDetails(obj1.hpsmid);
                            var finalTrigger = triggerFinalConfirmation(obj1.hpsmid);
                            if (finalTrigger.results == true)
                            {
                                res = BLWFMTicket.UpdateStatus(objstatus);
                            }
                            else
                            {
                                msg = finalTrigger.error_message;
                                //logHelper.ApiLogWriter("Closeticket()", "FEController", msg, "");
                            }
                        }
                        else
                        {
                            res = BLWFMTicket.UpdateStatus(objstatus);
                        }
                    }
                    else
                    {
                        msg = "Please enter serial number";
                        res = 1;
                    }
                }
                else
                {
                    if (iscpeactivateapi)
                    {
                        // call addordernotes
                        dynamic objAddOrderNotes = new ExpandoObject();
                        string orderId = obj1.hpsmid.Split('-')[0];
                        objAddOrderNotes.orderId = orderId;
                        objAddOrderNotes.notes = "Device " + obj1.cpe_type + " delivered Successfully.";
                        Models.API.WFMMobileApiResponse<dynamic> reshobs = Calladdordernotes(objAddOrderNotes);
                        if (Convert.ToBoolean(reshobs.results))
                        {
                            res = BLWFMTicket.UpdateStatus(objstatus);
                        }
                        else
                        {
                            msg = reshobs.error_message;
                        }
                    }
                    else
                    {
                        res = BLWFMTicket.UpdateStatus(objstatus);
                    }
                }

                response = Content(msg);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Models.API.WFMMobileApiResponse<dynamic> Calladdordernotes(dynamic reqobject)
        {
            string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
            string URL = basepath + "addordernotes";
            //string URL = Convert.ToString(ConfigurationManager.AppSettings["addordernotesapi"]);
            string DATA = Newtonsoft.Json.JsonConvert.SerializeObject(reqobject);
            Models.API.WFMMobileApiResponse<dynamic> response = new Models.API.WFMMobileApiResponse<dynamic>();
            response.results = false;
            var tokenResponse = new Models.API.WFMMobileApiResponse<dynamic>();
            ErrorLogHelper logHelper = new ErrorLogHelper();
            tokenResponse = getAccessToken();
            if (tokenResponse != null)
            {
                try
                {
                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                    System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers.Add("Authorization", "Bearer " + tokenResponse.results);
                    using (Stream webStream = request.GetRequestStream())
                    using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                    {
                        requestWriter.Write(DATA);
                    }
                    WebResponse webResponse = request.GetResponse();

                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        string responses = responseReader.ReadToEnd();
                        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responses);

                        if (jsonResponse.statusCode != "0000")
                        {
                            response.error_message = Convert.ToString(jsonResponse.statusDesc);
                            ErrorLogHelper.WriteErrorLog("Calladdordernotes()", "WorkforceController", null, "Request : " + Convert.ToString(DATA) + "|Response : " + responses, 0);
                        }
                        else
                        {
                            response.results = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.error_message = "Error while calling Api.";
                    ErrorLogHelper.WriteErrorLog("inner catch:Calladdordernotes()", "WorkforceController", ex, "request:" + Convert.ToString(DATA));
                }
            }
            else
            {
                response.error_message = "Error in getting api token.";
                ErrorLogHelper.WriteErrorLog("Calladdordernotes()", "WorkforceController", null, "request:" + Convert.ToString(DATA));
            }
            return response;
        }
        public ActionResult ActivateIPTVTicket(Task obj)
        {

            try
            {
                ActionResult response = null;
                var responsesdata = new Models.API.WFMMobileApiResponse<dynamic>();

                //bool save = false;
                int res = 0;
                var obj1 = BusinessLogics.WFM.BLWFMTicket.GetJobDetailByJobOrderId(obj.hpsmid);
                job_order_status objstatus = new job_order_status();
                objstatus.job_id = obj1.hpsmid;
                objstatus.action = "iptv_activate";
                objstatus.remarks = "IPTV activated by web module";
                var iscpeactivateapi = Convert.ToBoolean(ConfigurationManager.AppSettings["iscpeactivateapi"]);
                string msg = "IPTV activate successfuly.";
                if (obj1.cpe_serialno != "" && obj1.cpe_serialno != null)
                {
                    if (iscpeactivateapi)
                    {
                        var tokenResponse = new Models.API.WFMMobileApiResponse<dynamic>();
                        //Call closer Api

                        tokenResponse = getTriggerActivateDetails(obj1.hpsmid);
                        // var finalTrigger = triggerFinalConfirmation(obj1.hpsmid);
                        if (tokenResponse.results == true)
                        {
                            res = BLWFMTicket.UpdateStatus(objstatus);

                        }
                        else
                        {

                            msg = tokenResponse.error_message;
                            //logHelper.ApiLogWriter("Closeticket()", "FEController", msg, "");

                        }
                    }
                    else
                    {
                        res = BLWFMTicket.UpdateStatus(objstatus);

                    }
                }
                else
                {
                    msg = "Please enter serial number";
                    res = 1;
                }

                response = Content(msg);

                return response;
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public Models.API.WFMMobileApiResponse<dynamic> getAccessToken()
        {
            //ErrorLogHelper logHelper = new ErrorLogHelper();
            var objresponse = new Models.API.WFMMobileApiResponse<dynamic>();
            string DATA = "";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
                string URL = basepath + "getToken";
                //string URL = Convert.ToString(ConfigurationManager.AppSettings["getTokenPath"]);
                User_Detail result = new User_Detail();
                result.clientId = Convert.ToString(ConfigurationManager.AppSettings["clientId"]);
                result.clientSecret = Convert.ToString(ConfigurationManager.AppSettings["clientSecret"]);
                result.grantType = "Authorization Code";
                DATA = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/json";
                using (Stream webStream = request.GetRequestStream())
                using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                {
                    requestWriter.Write(DATA);
                }
                try
                {

                    WebResponse webResponse = request.GetResponse();
                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        string responses = responseReader.ReadToEnd();
                        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responses);
                        objresponse.results = jsonResponse.data.access_token;
                        objresponse.status = StatusCodes.OK.ToString();
                        if (objresponse.results == "")
                        {

                            ErrorLogHelper.WriteErrorLog("getAccessToken()", "WorkforceController", null, DATA);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("catch:getAccessToken()", "WorkforceController", ex, DATA);
                    objresponse.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    objresponse.error_message = "Error While Processing  Request.";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("outercatch:getAccessToken()", "WorkforceController", ex, DATA);
                objresponse.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objresponse.error_message = "Error While Processing  Request.";
            }
            return objresponse;
        }
        public Models.API.WFMMobileApiResponse<dynamic> getTriggerActivateDetails(string job_id)
        {
            //ErrorLogHelper logHelper = new ErrorLogHelper();
            var tokenResponse = new Models.API.WFMMobileApiResponse<dynamic>();
            var response = new Models.API.WFMMobileApiResponse<dynamic>();
            try
            {

                tokenResponse = getAccessToken();
                if (tokenResponse.results != "")
                {
                    string facility;
                    var TriggerActivateDetail = BLWFMTicket.getTriggerActivateDetails(job_id, out facility);
                    string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
                    string URL = basepath + "triggerActivation";
                    //string URL = Convert.ToString(ConfigurationManager.AppSettings["TriggerActivationServicePath"]);
                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                    System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    if (TriggerActivateDetail != null)
                    {
                        string Attributes = string.Empty;
                        var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(TriggerActivateDetail);

                        if (facility == "CATV" || facility == "CATV_EXTN")
                        {
                            Attributes = "catvAttributes";
                        }
                        else if (facility == "GPON")
                        {
                            Attributes = "gponAttributes";
                        }
                        else if (facility == "DOCSIS")
                        {
                            Attributes = "docsisAttributes";
                        }
                        else if (facility == "GFAST")
                        {
                            Attributes = "gfastAttributes";
                        }
                        else if (facility == "IPTV" || facility == "IPTV_EXTN")
                        {
                            Attributes = "iptvAttributes";
                        }

                        //if(Attributes== "catvAttributes")
                        //{
                        //    Attributes = "catvAttributes";
                        //}
                        //else if (Attributes == "gponAttributes")
                        //{
                        //    Attributes = "gponAttributes";
                        //}
                        //else if (Attributes == "docsisAttributes")
                        //{
                        //    Attributes = "docsisAttributes";
                        //}
                        //else if (Attributes == "gfastAttributes")
                        //{
                        //    Attributes = "gfastAttributes";
                        //}

                        var DATA = "{ " + "\"" + Attributes + "\"" + ":";
                        DATA = DATA + responeData;
                        DATA = DATA + "}";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        request.Headers.Add("Authorization", "Bearer " + tokenResponse.results);
                        using (Stream webStream = request.GetRequestStream())
                        using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                        {
                            requestWriter.Write(DATA);
                        }
                        try
                        {
                            WebResponse webResponse = request.GetResponse();
                            using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                            using (StreamReader responseReader = new StreamReader(webStream))
                            {
                                string responses = responseReader.ReadToEnd();

                                var dyn = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.API.ActivateCpeRes>(responses);
                                if (dyn.statusCode != "0000")
                                {
                                    response.results = false;
                                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                                    response.error_message = "CPE not activated :" + " Status desc: " + dyn.statusDesc;
                                    ErrorLogHelper.WriteErrorLog("getTriggerActivateDetails()", "WorkforceController", null, DATA);
                                }
                                else
                                {
                                    response.results = true;
                                    response.status = StatusCodes.OK.ToString();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorLogHelper.WriteErrorLog("inner catch:getTriggerActivateDetails()", "WorkforceController", ex, DATA);
                            response.results = false;
                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            response.error_message = "Error While Processing  Request.";
                        }
                    }
                    else
                    {
                        response.error_message = "Some technical error !!";
                        response.results = false;
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                    }
                }
                else
                {
                    response.error_message = "Access Token is not genrated";
                    response.results = false;
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("outer catch:getTriggerActivateDetails()", "WorkforceController", ex, null);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.results = false;
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        public Models.API.WFMMobileApiResponse<dynamic> triggerFinalConfirmation(string job_id)
        {
            //  ErrorLogHelper logHelper = new ErrorLogHelper();
            var tokenResponse = new Models.API.WFMMobileApiResponse<dynamic>();
            var response = new Models.API.WFMMobileApiResponse<dynamic>();
            try
            {

                tokenResponse = getAccessToken();
                if (tokenResponse.results != "")
                {
                    var TriggerFinalDetail = BLWFMTicket.getTriggerFinalDetails(job_id);
                    string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
                    string URL = basepath + "triggerFinalConfirmation";
                    //string URL = Convert.ToString(ConfigurationManager.AppSettings["triggerFinalConfirmation"]);
                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                    System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    dynamic data = TriggerFinalDetail;
                    var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers.Add("Authorization", "Bearer " + tokenResponse.results);
                    using (Stream webStream = request.GetRequestStream())
                    using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                    {
                        requestWriter.Write(responeData);
                    }
                    try
                    {
                        WebResponse webResponse = request.GetResponse();
                        using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string responses = responseReader.ReadToEnd();

                            var dyn = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.API.ActivateCpeRes>(responses);
                            if (dyn.statusCode != "0000")
                            {
                                response.results = false;
                                response.status = StatusCodes.INVALID_INPUTS.ToString();
                                response.error_message = "not close on hobs :" + " Status code: " + dyn.statusCode;
                                ErrorLogHelper.WriteErrorLog("triggerFinalConfirmation()", "WorkforceController", null, Convert.ToString(data), 0);
                            }
                            else
                            {
                                response.results = true;
                                response.status = StatusCodes.OK.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        ErrorLogHelper.WriteErrorLog("inner catch:triggerFinalConfirmation()", "WorkforceController", ex, Convert.ToString(data));
                        response.results = false;
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.error_message = "Error While Processing  Request.";
                    }
                }

                else
                {
                    response.error_message = "Access Token is not genrated";
                    response.results = false;
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("outer catch:triggerFinalConfirmation()", "WorkforceController", ex, job_id);
                response.results = false;
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        public JsonResult BindRttcategoryOnChange(string id)
        {
            var objResp = BLUser.GetttSubordinateDetailstt(id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        public ActionResult ManagerRouteIssueApprovett(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            //viewRouteIssueApprove.issueId = issueId;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            //int managerId = 397;

            int intCircle_id = 0;

            int intCircleCount = BLIssues.IssueCircleCount(viewRouteIssueApprove.issuesId, out intCircle_id);
            viewRouteIssueApprove.CircleCount = intCircleCount;

            if (intCircleCount > 1)
            {
                //return RedirectToAction("_ViewMessage");
                return PartialView("_ViewMessage", viewRouteIssueApprove);
            }

            if (viewRouteIssueApprove.status == "Assigned" || viewRouteIssueApprove.status == "Re-Assigned")
            {
                //Need to be FRT than Patroller.
                //viewRouteIssueApprove.lstUserDetail = BLUser.GetSubordinateDetails(managerId, "FRT", intCircle_id).ToList();

                List<User_Master> users = new List<User_Master>();
                List<User_Master> userslist = new List<User_Master>();
                users = BLUser.GetMMSubordinateDetails(managerId, "FRT").ToList();
                foreach (var new_user in users)
                {
                    User_Master user_ = new User_Master();
                    user_.user_id = new_user.user_id;
                    user_.name = MiscHelper.Decrypt(new_user.name);
                    user_.user_name = new_user.user_name;
                    userslist.Add(user_);
                }
                viewRouteIssueApprove.lstUserDetail = userslist;
                return PartialView("_AssignRouteIssuestt", viewRouteIssueApprove);
            }
            else
            {
                return PartialView("_ApproveIssueRoute", viewRouteIssueApprove);
            }
        }
        public ActionResult SaveRouteIssueStatustt(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            ActionResult response = null;
            var issuesids = viewRouteIssueApprove.issuesId.Split(',');
            for (int j = 0; j < issuesids.Length; j++)
            {


                var issuesId = Convert.ToInt32(issuesids[j]);
                var routeIssues = BLWFMTicket.GetHpsmidByRouteIssuesId(issuesId);
                if (routeIssues == null)
                {
                    var message = string.Format("No record found in job assignment");
                    response = Content("MSG:" + message);
                    return response;
                }
                var JobOrder = BLWFMTicket.GetHPSMTicket_DetailById(routeIssues.hpsm_ticketid ?? 0);

                if (JobOrder == null)
                {
                    var message = string.Format("No job found");
                    response = Content("MSG:" + message);
                    return response;
                }

                //SlotConfirmation Request = BLWFMTicket.GetSlotConfirmation(JobOrder.bookingid);
                //if (Request == null)
                //{
                //    var message = string.Format("No record found in confirm slot");
                //    response = Content("MSG:" + message);
                //    return response;
                //}

                //SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(Request.referenceid);
                //if (SlotRequest == null)
                //{
                //    var message = string.Format("No record found in slot request");
                //    response = Content("MSG:" + message);
                //    return response;
                //}

                //string jo_type = "";
                //string service = "";

                //SlotConfirmation Request = BLWFMTicket.GetSlotConfirmation(JobOrder.bookingid);
                //if (Request == null)
                //{
                //    var message = string.Format("No record found in confirm slot");
                //    response = Content("MSG:" + message);
                //    return response;
                //}
                //SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(Request.referenceid);
                //if (SlotRequest == null)
                //{
                //    var message = string.Format("No record found in slot request");
                //    response = Content("MSG:" + message);
                //    return response;
                //}

                //string jo_type = "";
                //string service = "";

                //var orderType = BLWFMTicket.GetOrderType(SlotRequest.order_type);
                //if (orderType == null)
                //{
                //    var message = string.Format("Order type not found");
                //    response = Content("MSG:" + message);
                //    return response;
                //}
                //if (string.IsNullOrEmpty(orderType.description2))
                //{
                //    var message = string.Format("Jo type not map with selected order type : {0}", SlotRequest.order_type);
                //    response = Content("MSG:" + message);
                //    return response;
                //}
                //jo_type = orderType.description2;

                //var serviceType = BLWFMTicket.Getservicetype(SlotRequest.service_type);

                //if (serviceType == null)
                //{
                //    var message = string.Format("Service type not found");
                //    response = Content("MSG:" + message);
                //    return response;
                //}
                //if (string.IsNullOrEmpty(serviceType.add_service))
                //{
                //    serviceType.add_service = serviceType.remove_service;
                //}
                //if (string.IsNullOrEmpty(serviceType.add_service))
                //{
                //    var message = string.Format("Add/Remove service facility  not map with selected service type : {0}", SlotRequest.service_type);
                //    response = Content("MSG:" + message);
                //    return response;
                //}
                //service = serviceType.add_service;

                //if (service.Contains(","))
                //{
                //    service = service.Split(',').First();
                //}

                //var FEList = BLWFMTicket.GetFEList(SlotRequest.managerid).Where(f => f.UserId == viewRouteIssueApprove.frtUserId).ToList();
                //var FEList = BLWFMTicket.GetFEDetailByUserId(viewRouteIssueApprove.frtUserId).ToList();

                //if (FEList.Count == 0)
                //{
                //    var message = string.Format("No FE map with contractor");
                //    response = Content("MSG:" + message);
                //    return response;
                //}
                ////case 6
                //var _isUserjotype = FEList.Where(j => j.transection.ToUpper() == jo_type.ToUpper()).ToList();
                //if (FEList.Count == 0)
                //{
                //    var message = string.Format("user not map with jo type");
                //    response = Content("MSG:" + message);
                //    return response;
                //}
                ////case 7
                //var _isUserService = _isUserjotype.Where(j => j.service.ToUpper() == service.ToUpper()).ToList();
                //if (_isUserService.Count == 0)
                //{
                //    var message = string.Format("user not map with service");
                //    response = Content("MSG:" + message);
                //    return response;

                //}
                //var _isJoCategory = _isUserService.Where(j => j.jo_category_name.ToUpper() == SlotRequest.jo_category.ToUpper()).ToList();
                //if (_isJoCategory.Count == 0)
                //{
                //    var message = string.Format("user not map with jo category");

                //    response = Content("MSG:" + message);
                //    return response;
                //}


                //int dateDiff = -4;// Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MobileTimeDiff"]);
                //string toDate = ""; //to date is set as blank as this will include future tasks as well... //MiscHelper.FormatDate(DateTime.Now.ToString());
                //string fromDate = "";
                //var lstAssignedTaskDetail = BLWFMTicket.GetFRTTaskDetails(viewRouteIssueApprove.frtUserId, toDate, fromDate, "All", "");
                //var assignTicket = lstAssignedTaskDetail.Where(x => x.customer_appointment_date == SlotRequest.appointment_date.ToString("dd-MMM-yyyy").ToUpper()).ToList();

                //if (assignTicket != null)
                //{
                //    var res = assignTicket.Any(w => w.master_slot_id == Request.master_slot_id);
                //    if (res)
                //    {
                //        response = Content("MSG:job allready assign to FE");
                //        return response;
                //    }
                //}
            }
            VW_Route_Issue objRouteIssue = new VW_Route_Issue();
            bool save = false;

            viewRouteIssueApprove.user_id = Convert.ToInt32(Session["user_id"].ToString());
            viewRouteIssueApprove.checkinRadius = ApplicationSettings.DefaultTaskCheckinRadius;
            if (viewRouteIssueApprove.status == "Assigned" || viewRouteIssueApprove.status == "Re-Assigned")
            {
                if (viewRouteIssueApprove.status == "Assigned")
                {


                    List<Vw_Hpsm_Ticket_Status> hpsmTicketList = new List<Vw_Hpsm_Ticket_Status>();
                    save = BLWFMTicket.AssignRouteIssue(viewRouteIssueApprove, out hpsmTicketList);
                    //string IsHPSMCall = System.Configuration.ConfigurationManager.AppSettings["HPSMCall"].ToString();
                    //if (!string.IsNullOrEmpty(IsHPSMCall))
                    //{
                    //    for (int index = 0; index < hpsmTicketList.Count; index++)
                    //    {
                    //        HPSMHelper hpsmHelper = new HPSMHelper(hpsmTicketList[index]);
                    //    }
                    //}

                    try
                    {
                        NotificationHelper notificatonHelper = new NotificationHelper();
                        var ids = viewRouteIssueApprove.issuesId.Split(',');
                        for (int i = 0; i < ids.Length; i++)
                        {
                            //commenting because view does not exist 
                            // notificatonHelper.sendNotification(viewRouteIssueApprove.frtUserId, Convert.ToInt32(ids[i]));
                        }
                    }
                    catch (Exception ec)
                    {
                        //ErrorLogHelper logHelper = new ErrorLogHelper();
                        //logHelper.ApiLogWriter("SaveRouteIssueStatus() + notificatonHelper", "Main Controller", reqData.data, ec);
                    }
                }
                if (viewRouteIssueApprove.status == "Re-Assigned")
                {
                    List<Vw_Hpsm_Ticket_Status> hpsmTicketList = new List<Vw_Hpsm_Ticket_Status>();
                    save = BLWFMTicket.AssignRouteIssue(viewRouteIssueApprove, out hpsmTicketList);
                    //string IsHPSMCall = System.Configuration.ConfigurationManager.AppSettings["HPSMCall"].ToString();
                    //if (!string.IsNullOrEmpty(IsHPSMCall))
                    //{
                    //    for (int index = 0; index < hpsmTicketList.Count; index++)
                    //    {
                    //        HPSMHelper hpsmHelper = new HPSMHelper(hpsmTicketList[index]);
                    //    }
                    //}

                    try
                    {
                        NotificationHelper notificatonHelper = new NotificationHelper();
                        var ids = viewRouteIssueApprove.issuesId.Split(',');
                        for (int i = 0; i < ids.Length; i++)
                        {
                            notificatonHelper.sendNotification(viewRouteIssueApprove.frtUserId, Convert.ToInt32(ids[i]));
                        }
                    }
                    catch (Exception ec)
                    {
                        //ErrorLogHelper logHelper = new ErrorLogHelper();
                        //logHelper.ApiLogWriter("SaveRouteIssueStatus() + notificatonHelper", "Main Controller", reqData.data, ec);
                    }
                }


            }
            else
            {
                save = BLWFMTicket.UpdateRouteIssueStatus(viewRouteIssueApprove);
            }

            if (save)
            {
                response = Content("Success_UpdateStatus_" + viewRouteIssueApprove.status + "");
            }
            else
            {
                response = Content("Error_UpdateStatus");
            }
            return response;



        }

        public JsonResult GetNotification()
        {
            try
            {

                int managerId = Convert.ToInt32(Session["user_id"].ToString());
                var Count = BLUser.getnotificationcount(managerId);
                //return Json(Count, JsonRequestBehavior.AllowGet);
                return Json(new { Data = Count, JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetNotification()", "WorkforceController", ex, null);
                return null;
            }


        }
        public JsonResult updateNotification(string id)
        {
            try
            {
                var Count = BLUser.updateNotification(id);
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("updateNotification()", "WorkforceController", ex, null);
                return null;
            }
        }


        public PartialViewResult AssignttJobOrderRM(ViewJobOrder objviewJobOrder, int page = 0, string sort = "", string sortdir = "", string status = "")
        {
            int totalRecords = 0;
            //int managerId =486;

            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            BLUser objBLuser = new BLUser();
            BLRoles objBLroles = new BLRoles();
            User objUserDetails = objBLuser.getUserDetails(managerId);
            RoleMaster objRoleMaster = objBLroles.getUserRoleNameByRoleId(objUserDetails.role_id);
            List<User_Master> lstRegionalHead = null;
            List<User_Master> lstGroupDetail = null;
            List<User_Master> lstJoManagerDetail = null;
            if (objRoleMaster.role_name == "Regional Head")
            {
                objviewJobOrder.vwJobOrderFilter.isLoginRole = 2;
                lstJoManagerDetail = BLUser.GetMMSubordinateDetails(managerId, "");
                lstGroupDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.jomanagerId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.jomanagerId, "").ToList();
            }
            else if (objRoleMaster.role_name == "CEO")
            {
                //objviewJobOrder.vwJobOrderFilter.isCeoRole = true;
                objviewJobOrder.vwJobOrderFilter.isLoginRole = 1;
                lstRegionalHead = BLUser.GetMMSubordinateDetails(managerId, "").ToList();
                lstJoManagerDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.regionalHeadId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.regionalHeadId, "").ToList();
                lstGroupDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.jomanagerId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.jomanagerId, "").ToList();
            }
            else
            {
                lstGroupDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();
            }
            var lstSubourdinateDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.groupmanagerId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.groupmanagerId, "").ToList();
            if (string.IsNullOrEmpty(objviewJobOrder.vwJobOrderFilter.status))
            {
                objviewJobOrder.vwJobOrderFilter.status = status;
            }

            objviewJobOrder.lstCustomerCategory = BLJobOrder.GetCustomerCategory();
            objviewJobOrder.lstCustomerSegment = BLJobOrder.GetCustomerSegment();
            objviewJobOrder.lstcategory = BLJobOrder.Getttcatgeory();
            //Models.User objUserDetails = objBLuser.getUserDetails(managerId);
            objviewJobOrder.lsttype = BLJobOrder.Getttype(string.IsNullOrEmpty(objviewJobOrder.vwJobOrderFilter.ttcategory) ? "" : objviewJobOrder.vwJobOrderFilter.ttcategory);
            objviewJobOrder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_assign_status.ToString()).ToList();
            objviewJobOrder.vwJobOrderFilter.userId = managerId;
            objviewJobOrder.vwJobOrderFilter.pageSize = objviewJobOrder.vwJobOrderFilter.pageSize == 0 ? 10 : objviewJobOrder.vwJobOrderFilter.pageSize;
            objviewJobOrder.vwJobOrderFilter.fromDate = MiscHelper.FormatDate(objviewJobOrder.vwJobOrderFilter.fromDate);
            objviewJobOrder.vwJobOrderFilter.toDate = MiscHelper.FormatDate(objviewJobOrder.vwJobOrderFilter.toDate);
            objviewJobOrder.vwJobOrderFilter.page = page;
            objviewJobOrder.vwJobOrderFilter.sort = sort;
            objviewJobOrder.vwJobOrderFilter.sortdir = sortdir;
            objviewJobOrder.vwJobOrderFilter.currentPage = page == 0 ? 1 : page;
            //objviewJobOrder.lstUserDetail = new BLUser().GetAllActiveUsersList().ToList();
            objviewJobOrder.lstJoDetailDetails = lstJoManagerDetail;
            objviewJobOrder.lstUserDetail = lstSubourdinateDetail;
            objviewJobOrder.lstRegionalHead = lstRegionalHead;


            //objviewJobOrder.lstUserDetail = new BLUser().GetAllActiveUsersList().ToList();
            //objviewJobOrder.lstJoDetailDetails = lstJoManagerDetail;
            objviewJobOrder.lstGroupDetails = lstGroupDetail;
            // objviewJobOrder.lstUserDetail = lstSubourdinateDetail;

            List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetttJobOrderDetailsRM(objviewJobOrder.vwJobOrderFilter, out totalRecords);

            //List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetJobOrderDetailsRM(objviewJobOrder.vwJobOrderFilter, out totalRecords);

            if (lstrouteIssues.Count > 0)
            {
                lstrouteIssues.Select(c =>
                {
                    c.ticket_source = _Ticket_Source[c.ticket_source_id];
                    return c;
                }).ToList();
            }
            objviewJobOrder.lstIssueDetails = lstrouteIssues;
            objviewJobOrder.vwJobOrderFilter.totalRecord = totalRecords;

            Session["viewIssueRouteFilter"] = objviewJobOrder.vwJobOrderFilter;

            return PartialView("_viewAssignttJobOrderRM", objviewJobOrder);
        }


        public JsonResult BindReportingManagerOnChangeRM(int ContractorId)
        {
            var objResp = BLUser.GetMMSubordinateDetails(ContractorId, "");
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }



        [HttpPost]
        public ActionResult ViewAssignttJobOrderRM(ViewAssignJobOrder objViewAssignJobOrder, int page = 0, string sort = "", string sortdir = "")
        {


            int totalRecords = 0;
            int cictId = Convert.ToInt32(Session["user_id"].ToString());
            BLUser objBLuser = new BLUser();
            BLRoles objBLroles = new BLRoles();
            User objUserDetails = objBLuser.getUserDetails(cictId);
            RoleMaster objRoleMaster = objBLroles.getUserRoleNameByRoleId(objUserDetails.role_id);
            List<User_Master> lstRegionalHead = null;
            List<User_Master> lstGroupDetail = null;
            List<User_Master> lstJoManagerDetail = null;
            if (objRoleMaster.role_name == "Regional Head")
            {
                objViewAssignJobOrder.viewAssignJobOrderFilter.isLoginRole = 2;
                lstJoManagerDetail = BLUser.GetMMSubordinateDetails(cictId, "").ToList();
                lstGroupDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.jomanagerId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.jomanagerId, "").ToList();
            }
            else if (objRoleMaster.role_name == "CEO")
            {
                objViewAssignJobOrder.viewAssignJobOrderFilter.isLoginRole = 1;
                lstRegionalHead = BLUser.GetMMSubordinateDetails(cictId, " ").ToList();
                lstJoManagerDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.regionalHeadId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.regionalHeadId, "").ToList();
                lstGroupDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.jomanagerId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.jomanagerId, "").ToList();

            }

            else
            {
                lstGroupDetail = BLUser.GetMMSubordinateDetails(cictId, "").ToList();
            }
            var lstContractorDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.groupmanagerId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.groupmanagerId, "").ToList();
            var lstFEUserDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.managerId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.managerId, "").ToList();
            objViewAssignJobOrder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_view_status.ToString()).ToList();
            var lstSubourdinateDetail = BLUser.GetMMSubordinateDetails(objViewAssignJobOrder.viewAssignJobOrderFilter.groupmanagerId == 0 ? -1 : objViewAssignJobOrder.viewAssignJobOrderFilter.groupmanagerId, "").ToList();
            objViewAssignJobOrder.lstIssueAssignedToDetail = lstFEUserDetail;
            objViewAssignJobOrder.lstJoType = BLJobOrder.GetJoType();
            objViewAssignJobOrder.lstServiceType = BLJobOrder.GetServicesType();
            objViewAssignJobOrder.lstJoCategory = BLJobOrder.GetJoCategory();
            objViewAssignJobOrder.viewAssignJobOrderFilter.userId = cictId;
            objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize = objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize == 0 ? 10 : objViewAssignJobOrder.viewAssignJobOrderFilter.pageSize;
            objViewAssignJobOrder.viewAssignJobOrderFilter.fromDate = MiscHelper.FormatDate(objViewAssignJobOrder.viewAssignJobOrderFilter.fromDate);
            objViewAssignJobOrder.viewAssignJobOrderFilter.toDate = MiscHelper.FormatDate(objViewAssignJobOrder.viewAssignJobOrderFilter.toDate);
            objViewAssignJobOrder.viewAssignJobOrderFilter.page = page;
            objViewAssignJobOrder.viewAssignJobOrderFilter.sort = sort;
            objViewAssignJobOrder.viewAssignJobOrderFilter.sortdir = sortdir;
            objViewAssignJobOrder.viewAssignJobOrderFilter.currentPage = page == 0 ? 1 : page;
            objViewAssignJobOrder.lstJoDetailDetails = lstJoManagerDetail;
            objViewAssignJobOrder.lstGroupDetails = lstGroupDetail;
            objViewAssignJobOrder.lstCantractorDetails = lstContractorDetail;
            objViewAssignJobOrder.lstCustomerCategory = BLJobOrder.GetCustomerCategory();
            objViewAssignJobOrder.lstCustomerSegment = BLJobOrder.GetCustomerSegment();
            objViewAssignJobOrder.lstcategory = BLJobOrder.Getttcatgeory();
            objViewAssignJobOrder.lsttype = BLJobOrder.Getttype(string.IsNullOrEmpty(objViewAssignJobOrder.viewAssignJobOrderFilter.ttcategory) ? "" : objViewAssignJobOrder.viewAssignJobOrderFilter.ttcategory);
            objViewAssignJobOrder.lstUserDetail = lstSubourdinateDetail;
            objViewAssignJobOrder.lstRegionalHead = lstRegionalHead;
            List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAssignIssueRouteDetailsttRM(objViewAssignJobOrder.viewAssignJobOrderFilter, out totalRecords);

            lstAssignIssueDetails.Select(c =>
            {
                c.ticket_source = _Ticket_Source[c.ticket_source_id];
                return c;
            }).ToList();

            objViewAssignJobOrder.lstAssignJobOrderDetails = lstAssignIssueDetails;

            objViewAssignJobOrder.statusCount = BLJobOrder.GetViewRMJobOrderStatusTTCount(objViewAssignJobOrder.viewAssignJobOrderFilter);
            objViewAssignJobOrder.viewAssignJobOrderFilter.totalRecord = totalRecords;
            Session["viewAssignRouteIssueFilter"] = objViewAssignJobOrder.viewAssignJobOrderFilter;

            return PartialView("_ViewAssignJobOrderttRM", objViewAssignJobOrder);
        }
        public JsonResult GetContractorListByGroupviewRM(int ContractorId)
        {
            var objResp = BLUser.GetMMSubordinateDetails(ContractorId, "");
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public ActionResult ContractorManagerRouteIssueApprovett(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            //viewRouteIssueApprove.issueId = issueId;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            //int managerId = 397;

            //Need to be FRT than Patroller.
            //viewRouteIssueApprove.lstUserDetail = BLUser.GetSubordinateDetails(managerId, "FRT", intCircle_id).ToList();
            List<User_Master> users = new List<User_Master>();
            List<User_Master> userslist = new List<User_Master>();
            users = BLUser.GetSubordinateContractorDetailstt(managerId, "").ToList();
            foreach (var new_user in users)
            {
                User_Master user_ = new User_Master();
                user_.user_id = new_user.user_id;
                user_.name = MiscHelper.Decrypt(new_user.name);
                user_.user_name = new_user.user_name;
                user_.group_name = new_user.group_name;
                userslist.Add(user_);
            }
            viewRouteIssueApprove.lstUserDetail = userslist;

            return PartialView("_AssignContractorRouteIssuestt", viewRouteIssueApprove);


        }

        public PartialViewResult GetUserPermisionArea()
        {
            int userId = Convert.ToInt32(Session["user_id"].ToString());
            GetUserPermissionArea getUserPermissionArea = new GetUserPermissionArea();
            getUserPermissionArea = BLJobOrder.GetUserPermissionArea(userId);
            return PartialView("_ViewUserPermissionArea", getUserPermissionArea);


        }
        public PartialViewResult ViewMaterialDetail(ViewMaterialDetail model)
        {
            // int TicketId = Convert.ToInt32(id);
            List<ViewMaterialDetail> ViewMaterialDetails = new List<ViewMaterialDetail>();
            ViewMaterialDetail ViewMaterialDetail = new ViewMaterialDetail();
            ViewMaterialDetails = BLWFMTicket.ViewMaterialDetail(model.jobid);
            ViewMaterialDetail.ViewMaterialDetails = ViewMaterialDetails;
            return PartialView("_ViewMaterialDetail", ViewMaterialDetail);

        }
        [HttpGet]
        public FileResult DownloadFiles(string jobid)
        {
            Utility.ErrorLogHelper logHelper = new Utility.ErrorLogHelper();
            var lstDocument = BLWFMTicket.getAttachmentDetailsbyJobId(jobid);
            string zipName = string.Empty;
            string currentfile = "";
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
                string attachmentlocalpath = ConfigurationManager.AppSettings["AttachmentLocalPath"];
                string localPath = System.Web.HttpContext.Current.Server.MapPath(attachmentlocalpath);
                localPath = Path.Combine(localPath, jobid);
                if (Directory.Exists(localPath))
                {
                    Directory.Delete(localPath, true);
                }
                Directory.CreateDirectory(localPath);
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {

                    zip.AlternateEncodingUsage = Ionic.Zip.ZipOption.AsNecessary;
                    foreach (var item in lstDocument)
                    {
                        string fullPath = string.Concat(FtpUrl, item.file_location, item.file_name);
                        string FileName = string.Concat(item.file_location, item.file_name);
                        localPath = Path.Combine(localPath, item.file_name);
                        currentfile = FileName;
                        try
                        {
                            var request = (FtpWebRequest)WebRequest.Create(fullPath);
                            request.Method = WebRequestMethods.Ftp.DownloadFile;
                            request.Credentials = new NetworkCredential(UserName, PassWord);
                            request.UseBinary = true;

                            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                            {
                                using (Stream responseStream = response.GetResponseStream())
                                {
                                    using (FileStream fs = new FileStream(localPath, FileMode.Create))
                                    {
                                        byte[] buffer = new byte[102400];
                                        int read = 0;

                                        while (true)
                                        {
                                            read = responseStream.Read(buffer, 0, buffer.Length);
                                            if (read == 0)
                                                break;

                                            fs.Write(buffer, 0, read);
                                        }
                                        fs.Close();
                                    }
                                }
                            }
                            zip.AddFile(localPath, "");
                            localPath = System.Web.HttpContext.Current.Server.MapPath(attachmentlocalpath);
                            localPath = Path.Combine(localPath, jobid);

                        }
                        catch (Exception ex)
                        {
                            logHelper.ApiLogWriter("outercatch:DownloadFiles", "WorkforceController", currentfile, ex);
                        }
                    }
                    zipName = String.Format("{0}{1}{2}{3}.zip", jobid + "Attachments_", DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss"));
                    zip.Name = zipName;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        zip.Save(memoryStream);
                        return File(memoryStream.ToArray(), "application/zip", zipName);
                    }
                }
            }
            catch (Exception ex)
            {
                logHelper.ApiLogWriter("outercatch:DownloadFiles", "WorkforceController", currentfile, ex);
            }
            return null;
        }
        public ActionResult jo_form(string jobid)
        {
            jo_form jf = BLWFMTicket.GetJo_form(jobid);
            return PartialView("_ViewJoForm", jf);
        }
        public void DownloadTTRouteIssueReport()
        {
            if (Session["viewIssueRouteFilter"] != null)
            {
                ViewJobOrderFilter objviewJobOrder = (ViewJobOrderFilter)Session["viewIssueRouteFilter"];

                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;
                objviewJobOrder.currentPage = 0;
                List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetJobOrderDetailstt(objviewJobOrder, out totalRecords);
                if (lstrouteIssues.Count > 0)
                {
                    lstrouteIssues.Select(c =>
                    {
                        c.ticket_source = _Ticket_Source[c.ticket_source_id];
                        return c;
                    }).ToList();
                }
                dtReport = MiscHelper.ListToDataTable<VW_Route_Issue>(lstrouteIssues);

                if (dtReport.Rows.Count > 0)
                {

                    //Rename columns..
                    dtReport.Columns["ISSUE_DESC"].ColumnName = "Service_Type";
                    dtReport.Columns["HPSM_TICKETID"].ColumnName = "JOB_ID";
                    dtReport.Columns.Remove("ISSUE_ID");
                    dtReport.Columns.Remove("SUB_STATUS");
                    dtReport.Columns.Remove("ROUTE_NAME");
                    dtReport.Columns.Remove("MOBILE_TIME");
                    dtReport.Columns.Remove("VSF_TICKETID");
                    dtReport.Columns.Remove("CUSTOMER_ID");
                    dtReport.Columns.Remove("CUSTOMER_ADD");
                    dtReport.Columns.Remove("TICKET_SOURCE_ID");
                    dtReport.Columns.Remove("CIRCLE_NAME");
                    dtReport.Columns.Remove("CLUSTER_ID");
                    dtReport.Columns.Remove("CLUSTER_NAME");
                    dtReport.Columns.Remove("SOCIETY_NAME");
                    dtReport.Columns.Remove("IMPACTTYPE");
                    dtReport.Columns.Remove("NETWORKENTITY");
                    dtReport.Columns.Remove("POP_NAME");
                    dtReport.Columns.Remove("TICKETTYPE");
                    dtReport.Columns.Remove("CUSTOMERCOUNT");
                    dtReport.Columns.Remove("LOCATION");
                    dtReport.Columns.Remove("BOUNDARY");
                    dtReport.Columns.Remove("hpsmid");
                    dtReport.Columns.Remove("cpe_type");
                    dtReport.Columns.Remove("cpe_portno");
                    dtReport.Columns.Remove("USER_REMARK");
                    dtReport.Columns.Remove("local_convergence_point");
                    dtReport.Columns.Remove("current_cpesn");
                    dtReport.Columns.Remove("issue_comment");
                    dtReport.Columns.Remove("cpe_mac_address");
                    dtReport.Columns.Remove("ip");
                    dtReport.Columns.Remove("gw");
                    dtReport.Columns.Remove("sm");
                    dtReport.Columns.Remove("dns");
                    dtReport.Columns.Remove("adns");
                    dtReport.Columns.Remove("latitude");
                    dtReport.Columns.Remove("longitude");
                    dtReport.Columns.Remove("modified_on");
                    dtReport.Columns.Remove("ticket_source");
                    dtReport.Columns.Remove("bookingid");
                    dtReport.Columns.Remove("atomic_id");
                    dtReport.Columns.Remove("access_type");
                    dtReport.Columns.Remove("package_name");
                    dtReport.Columns.Remove("nap_port");
                    dtReport.Columns.Remove("node");
                    dtReport.Columns.Remove("facility");
                    dtReport.Columns.Remove("customer_rmn");
                    dtReport.Columns.Remove("secondary_contact");
                    dtReport.Columns.Remove("email_id");
                    dtReport.Columns.Remove("listextension_boxsn");
                    dtReport.Columns.Remove("subscriber_name");
                    dtReport.Columns.Remove("addressline4");
                    dtReport.Columns.Remove("barangay_name");
                    dtReport.Columns.Remove("jo_category");
                    dtReport.Columns.Remove("reference_id");
                    dtReport.Columns.Remove("addressid");
                    dtReport.Columns.Remove("cpe_item_code");
                    dtReport.Columns.Remove("cpe_ref_serial");
                    dtReport.Columns.Remove("cpe_uom");
                    dtReport.Columns.Remove("cpe_wh");
                    dtReport.Columns.Remove("appointment_date");
                    dtReport.Columns.Remove("slot_time");
                    dtReport.Columns.Remove("sla");
                    dtReport.Columns.Remove("is_attachment");
                    dtReport.Columns.Remove("group_user");

                }

                string fileName = "AssignedJobOrderReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }
        public void DownloadAssignedRouteIssueReportTT()
        {
            if (Session["viewAssignRouteIssueFilter"] != null)
            {
                ViewAssignJobOrderFilter objViewAssignJobOrder = (ViewAssignJobOrderFilter)Session["viewAssignRouteIssueFilter"];

                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;

                //  objViewRouteAssignIssue.viewAssignJobOrderFilter = objAssignRouteIssueFilter;
                objViewAssignJobOrder.currentPage = 0;
                List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAssignJobOrderDetailstt(objViewAssignJobOrder, out totalRecords);
                if (lstAssignIssueDetails.Count > 0)
                {
                    lstAssignIssueDetails.Select(c =>
                    {
                        c.ticket_source = _Ticket_Source[c.ticket_source_id];
                        return c;
                    }).ToList();
                }
                dtReport = MiscHelper.ListToDataTable<VW_Assign_Job_Order>(lstAssignIssueDetails);

                if (dtReport.Rows.Count > 0)
                {

                    //Rename columns..
                    dtReport.Columns["HPSMID"].ColumnName = "JOB ID";
                    dtReport.Columns["frt_user_name"].ColumnName = "Assigned To";
                    dtReport.Columns.Remove("ISSUE_ID");
                    dtReport.Columns.Remove("task_tracking_id");
                    dtReport.Columns.Remove("vsf_ticketid");
                    dtReport.Columns.Remove("patroller_user_name");
                    dtReport.Columns.Remove("patroller_remark");
                    dtReport.Columns.Remove("modified_on");
                    //dtReport.Columns.Remove("CREATED_ON");
                    dtReport.Columns.Remove("ADDRESSID");
                    dtReport.Columns.Remove("CLUSTER_ID");
                    dtReport.Columns.Remove("CLUSTER_NAME");
                    dtReport.Columns.Remove("SOCIETY_NAME");
                    dtReport.Columns.Remove("CUSTOMER_ADD");
                    dtReport.Columns.Remove("CIRCLE_NAME");
                    dtReport.Columns.Remove("is_attachment");
                    dtReport.Columns.Remove("TICKET_SOURCE");
                    dtReport.Columns.Remove("hold_rc_code");
                    dtReport.Columns.Remove("hold_rca_code");
                    dtReport.Columns.Remove("hold_remarks");
                    dtReport.Columns.Remove("hold_appointment_date");
                    dtReport.Columns.Remove("cpe_type");
                    dtReport.Columns.Remove("cpe_portno");
                    dtReport.Columns.Remove("cpe_model");
                    dtReport.Columns.Remove("local_convergence_point");
                    dtReport.Columns.Remove("current_cpesn");
                    dtReport.Columns.Remove("issue_comment");
                    dtReport.Columns.Remove("cpe_mac_address");
                    dtReport.Columns.Remove("checkout_remarks");
                    dtReport.Columns.Remove("checkin_remarks");
                    dtReport.Columns.Remove("issue_desc");
                    dtReport.Columns.Remove("assigned_date");
                    dtReport.Columns.Remove("manager_remark");
                    dtReport.Columns.Remove("latitude");
                    dtReport.Columns.Remove("longitude");
                    dtReport.Columns.Remove("mobile_checkin_time");
                    dtReport.Columns.Remove("mobile_checkout_time");
                    dtReport.Columns.Remove("customer_id");
                    dtReport.Columns.Remove("atomic_id");
                    dtReport.Columns.Remove("access_type");
                    dtReport.Columns.Remove("package_name");
                    dtReport.Columns.Remove("nap_port");
                    dtReport.Columns.Remove("node");
                    dtReport.Columns.Remove("cpe_brand");
                    dtReport.Columns.Remove("facility");
                    dtReport.Columns.Remove("customer_rmn");
                    dtReport.Columns.Remove("secondary_contact");
                    dtReport.Columns.Remove("email_id");
                    dtReport.Columns.Remove("listextension_boxsn");
                    dtReport.Columns.Remove("jo_category");
                    dtReport.Columns.Remove("reference_id");
                    dtReport.Columns.Remove("cpe_item_code");
                    dtReport.Columns.Remove("cpe_ref_serial");
                    dtReport.Columns.Remove("cpe_uom");
                    dtReport.Columns.Remove("cpe_wh");
                    dtReport.Columns.Remove("appointment_date");
                    dtReport.Columns.Remove("slot_time");
                    //dtReport.Columns.Remove("sla");
                    dtReport.Columns.Remove("ip");
                    dtReport.Columns.Remove("gw");
                    dtReport.Columns.Remove("ticket_source_id");

                    dtReport.Columns.Remove("sm");
                    dtReport.Columns.Remove("dns");
                    dtReport.Columns.Remove("adns");
                    dtReport.Columns.Remove("group_user");
                    dtReport.Columns["hpsm_ticketid"].ColumnName = "JO Source";
                    dtReport.Columns["addressline1"].ColumnName = "Floor Number";
                    dtReport.Columns["addressline2"].ColumnName = "Street";
                    dtReport.Columns["addressline3"].ColumnName = "Building Name";
                    dtReport.Columns["site_type"].ColumnName = "Circuit Id";



                }

                string fileName = "ViewAssignedJobOrderReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }
        public void DownloadTMRouteIssueReport()
        {

            if (Session["viewIssueRouteFilter"] != null)
            {
                ViewJobOrderFilter objviewJobOrder = (ViewJobOrderFilter)Session["viewIssueRouteFilter"];

                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;
                objviewJobOrder.currentPage = 0;
                List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetttJobOrderDetailsRM(objviewJobOrder, out totalRecords);
                if (lstrouteIssues.Count > 0)
                {
                    lstrouteIssues.Select(c =>
                    {
                        c.ticket_source = _Ticket_Source[c.ticket_source_id];
                        return c;
                    }).ToList();
                }
                dtReport = MiscHelper.ListToDataTable<VW_Route_Issue>(lstrouteIssues);

                if (dtReport.Rows.Count > 0)
                {

                    //Rename columns..
                    dtReport.Columns["ISSUE_DESC"].ColumnName = "Service_Type";
                    dtReport.Columns["HPSM_TICKETID"].ColumnName = "JOB_ID";
                    dtReport.Columns.Remove("ISSUE_ID");
                    dtReport.Columns.Remove("SUB_STATUS");
                    dtReport.Columns.Remove("ROUTE_NAME");
                    dtReport.Columns.Remove("MOBILE_TIME");
                    dtReport.Columns.Remove("VSF_TICKETID");
                    dtReport.Columns.Remove("CUSTOMER_ID");
                    dtReport.Columns.Remove("CUSTOMER_ADD");
                    dtReport.Columns.Remove("TICKET_SOURCE_ID");
                    dtReport.Columns.Remove("CIRCLE_NAME");
                    dtReport.Columns.Remove("CLUSTER_ID");
                    dtReport.Columns.Remove("CLUSTER_NAME");
                    dtReport.Columns.Remove("SOCIETY_NAME");
                    dtReport.Columns.Remove("IMPACTTYPE");
                    dtReport.Columns.Remove("NETWORKENTITY");
                    dtReport.Columns.Remove("POP_NAME");
                    dtReport.Columns.Remove("TICKETTYPE");
                    dtReport.Columns.Remove("CUSTOMERCOUNT");
                    dtReport.Columns.Remove("LOCATION");
                    dtReport.Columns.Remove("BOUNDARY");
                    dtReport.Columns.Remove("hpsmid");
                    dtReport.Columns.Remove("cpe_type");
                    dtReport.Columns.Remove("cpe_portno");
                    dtReport.Columns.Remove("USER_REMARK");
                    dtReport.Columns.Remove("local_convergence_point");
                    dtReport.Columns.Remove("current_cpesn");
                    dtReport.Columns.Remove("issue_comment");
                    dtReport.Columns.Remove("cpe_mac_address");
                    dtReport.Columns.Remove("ip");
                    dtReport.Columns.Remove("gw");
                    dtReport.Columns.Remove("sm");
                    dtReport.Columns.Remove("dns");
                    dtReport.Columns.Remove("adns");
                    dtReport.Columns.Remove("latitude");
                    dtReport.Columns.Remove("longitude");
                    dtReport.Columns.Remove("modified_on");
                    dtReport.Columns.Remove("ticket_source");
                    dtReport.Columns.Remove("bookingid");
                    dtReport.Columns.Remove("atomic_id");
                    dtReport.Columns.Remove("access_type");
                    dtReport.Columns.Remove("package_name");
                    dtReport.Columns.Remove("nap_port");
                    dtReport.Columns.Remove("node");
                    dtReport.Columns.Remove("facility");
                    dtReport.Columns.Remove("customer_rmn");
                    dtReport.Columns.Remove("secondary_contact");
                    dtReport.Columns.Remove("email_id");
                    dtReport.Columns.Remove("listextension_boxsn");
                    dtReport.Columns.Remove("subscriber_name");
                    dtReport.Columns.Remove("addressline4");
                    dtReport.Columns.Remove("barangay_name");
                    dtReport.Columns.Remove("jo_category");
                    dtReport.Columns.Remove("reference_id");
                    dtReport.Columns.Remove("addressid");
                    dtReport.Columns.Remove("cpe_item_code");
                    dtReport.Columns.Remove("cpe_ref_serial");
                    dtReport.Columns.Remove("cpe_uom");
                    dtReport.Columns.Remove("cpe_wh");
                    dtReport.Columns.Remove("appointment_date");
                    dtReport.Columns.Remove("slot_time");
                    dtReport.Columns.Remove("sla");
                    dtReport.Columns.Remove("is_attachment");
                    dtReport.Columns.Remove("group_user");
                }

                string fileName = "AssignedJobOrderReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }
        public void DownloadVTMRouteIssueReport()
        {
            if (Session["viewAssignRouteIssueFilter"] != null)
            {
                ViewAssignJobOrderFilter objViewAssignJobOrder = (ViewAssignJobOrderFilter)Session["viewAssignRouteIssueFilter"];

                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;

                //  objViewRouteAssignIssue.viewAssignJobOrderFilter = objAssignRouteIssueFilter;
                objViewAssignJobOrder.currentPage = 0;
                List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAssignIssueRouteDetailsttRM(objViewAssignJobOrder, out totalRecords);
                if (lstAssignIssueDetails.Count > 0)
                {
                    lstAssignIssueDetails.Select(c =>
                    {
                        c.ticket_source = _Ticket_Source[c.ticket_source_id];
                        return c;
                    }).ToList();
                }
                dtReport = MiscHelper.ListToDataTable<VW_Assign_Job_Order>(lstAssignIssueDetails);

                if (dtReport.Rows.Count > 0)
                {

                    //Rename columns..
                    dtReport.Columns["HPSMID"].ColumnName = "JOB ID";
                    dtReport.Columns["frt_user_name"].ColumnName = "Assigned To";
                    dtReport.Columns.Remove("ISSUE_ID");
                    dtReport.Columns.Remove("task_tracking_id");
                    dtReport.Columns.Remove("vsf_ticketid");
                    dtReport.Columns.Remove("patroller_user_name");
                    dtReport.Columns.Remove("patroller_remark");
                    dtReport.Columns.Remove("modified_on");
                    dtReport.Columns.Remove("CREATED_ON");
                    dtReport.Columns.Remove("ADDRESSID");
                    dtReport.Columns.Remove("CLUSTER_ID");
                    dtReport.Columns.Remove("CLUSTER_NAME");
                    dtReport.Columns.Remove("SOCIETY_NAME");
                    dtReport.Columns.Remove("CUSTOMER_ADD");
                    dtReport.Columns.Remove("CIRCLE_NAME");
                    dtReport.Columns.Remove("is_attachment");
                    dtReport.Columns.Remove("TICKET_SOURCE");
                    dtReport.Columns.Remove("hold_rc_code");
                    dtReport.Columns.Remove("hold_rca_code");
                    dtReport.Columns.Remove("hold_remarks");
                    dtReport.Columns.Remove("hold_appointment_date");
                    dtReport.Columns.Remove("cpe_type");
                    dtReport.Columns.Remove("cpe_portno");
                    dtReport.Columns.Remove("cpe_model");
                    dtReport.Columns.Remove("local_convergence_point");
                    dtReport.Columns.Remove("current_cpesn");
                    dtReport.Columns.Remove("issue_comment");
                    dtReport.Columns.Remove("cpe_mac_address");
                    dtReport.Columns.Remove("checkout_remarks");
                    dtReport.Columns.Remove("checkin_remarks");
                    dtReport.Columns.Remove("issue_desc");
                    dtReport.Columns.Remove("assigned_date");
                    dtReport.Columns.Remove("manager_remark");
                    dtReport.Columns.Remove("latitude");
                    dtReport.Columns.Remove("longitude");
                    dtReport.Columns.Remove("mobile_checkin_time");
                    dtReport.Columns.Remove("mobile_checkout_time");
                    dtReport.Columns.Remove("customer_id");
                    dtReport.Columns.Remove("atomic_id");
                    dtReport.Columns.Remove("access_type");
                    dtReport.Columns.Remove("package_name");
                    dtReport.Columns.Remove("nap_port");
                    dtReport.Columns.Remove("node");
                    dtReport.Columns.Remove("cpe_brand");
                    dtReport.Columns.Remove("facility");
                    dtReport.Columns.Remove("customer_rmn");
                    dtReport.Columns.Remove("secondary_contact");
                    dtReport.Columns.Remove("email_id");
                    dtReport.Columns.Remove("listextension_boxsn");
                    dtReport.Columns.Remove("jo_category");
                    dtReport.Columns.Remove("reference_id");
                    dtReport.Columns.Remove("cpe_item_code");
                    dtReport.Columns.Remove("cpe_ref_serial");
                    dtReport.Columns.Remove("cpe_uom");
                    dtReport.Columns.Remove("cpe_wh");
                    dtReport.Columns.Remove("appointment_date");
                    dtReport.Columns.Remove("slot_time");
                    dtReport.Columns.Remove("sla");
                    dtReport.Columns.Remove("ip");
                    dtReport.Columns.Remove("gw");
                    dtReport.Columns.Remove("ticket_source_id");
                    dtReport.Columns.Remove("sm");
                    dtReport.Columns.Remove("dns");
                    dtReport.Columns.Remove("adns");
                    dtReport.Columns.Remove("group_user");
                    dtReport.Columns["hpsm_ticketid"].ColumnName = "JO Source";
                    dtReport.Columns["addressline1"].ColumnName = "Floor Number";
                    dtReport.Columns["addressline2"].ColumnName = "Street";
                    dtReport.Columns["addressline3"].ColumnName = "Building Name";
                    dtReport.Columns["site_type"].ColumnName = "Circuit Id";
                }
                string fileName = "ViewAssignedJobOrderReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }

        public void DownloadAdditionalMaterialReport()
        {
            if (Session["viewAssignRouteIssueFilter"] != null)
            {
                ViewAssignJobOrderFilter objAssignRouteIssueFilter = (ViewAssignJobOrderFilter)Session["viewAssignRouteIssueFilter"];
                DataTable dtReport = new DataTable();
                int totalRecords = 0;
                int isExport = 1;
                objAssignRouteIssueFilter.currentPage = 0;
                List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAdditionalMaterial(objAssignRouteIssueFilter, out totalRecords);
                if (lstAssignIssueDetails.Count > 0)
                {
                    lstAssignIssueDetails.Select(c =>
                    {
                        c.ticket_source = _Ticket_Source[c.ticket_source_id];
                        return c;
                    }).ToList();
                    dtReport = MiscHelper.ListToDataTable<VW_Assign_Job_Order>(lstAssignIssueDetails);
                    if (dtReport.Rows.Count > 0)
                    {

                        dtReport.Columns.Remove("longitude");
                        dtReport.Columns.Remove("latitude");
                        dtReport.Columns.Remove("sla");
                        dtReport.Columns.Remove("slot_time");
                        dtReport.Columns.Remove("appointment_date");
                        dtReport.Columns.Remove("group_user");
                        dtReport.Columns.Remove("reference_id");
                        dtReport.Columns.Remove("frt_user_name");
                        dtReport.Columns.Remove("stateorprovince");
                        dtReport.Columns.Remove("pincode");
                        dtReport.Columns.Remove("city");
                        dtReport.Columns.Remove("BARANGAY_NAME");
                        dtReport.Columns.Remove("addressline1");
                        dtReport.Columns.Remove("addressline3");
                        dtReport.Columns.Remove("addressline2");
                        dtReport.Columns.Remove("addressline4");
                        dtReport.Columns.Remove("subscriber_name");
                        dtReport.Columns.Remove("bookingid");
                        dtReport.Columns.Remove("account_number");
                        dtReport.Columns.Remove("remarks");
                        dtReport.Columns.Remove("rca");
                        dtReport.Columns.Remove("sub_status");
                        dtReport.Columns.Remove("status");
                        dtReport.Columns.Remove("hpsm_ticketid");
                        dtReport.Columns.Remove("adns");
                        dtReport.Columns.Remove("dns");
                        dtReport.Columns.Remove("sm");
                        dtReport.Columns.Remove("gw");
                        dtReport.Columns.Remove("ip");
                        dtReport.Columns.Remove("cpe_wh");
                        dtReport.Columns.Remove("cpe_uom");
                        dtReport.Columns.Remove("cpe_ref_serial");
                        dtReport.Columns.Remove("erp_response");
                        dtReport.Columns.Remove("sendtoerp");
                        dtReport.Columns.Remove("cpe_item_code");
                        dtReport.Columns.Remove("jo_category");
                        dtReport.Columns.Remove("listextension_boxsn");
                        dtReport.Columns.Remove("email_id");
                        dtReport.Columns.Remove("secondary_contact");
                        dtReport.Columns.Remove("customer_rmn");
                        dtReport.Columns.Remove("facility");
                        dtReport.Columns.Remove("cpe_serialno");
                        dtReport.Columns.Remove("cpe_brand");
                        dtReport.Columns.Remove("node");
                        dtReport.Columns.Remove("nap_port");
                        dtReport.Columns.Remove("package_name");
                        dtReport.Columns.Remove("access_type");
                        dtReport.Columns.Remove("mobile_checkout_time");
                        dtReport.Columns.Remove("mobile_checkin_time");
                        dtReport.Columns.Remove("manager_remark");
                        dtReport.Columns.Remove("ASSIGNED_DATE");
                        dtReport.Columns.Remove("issue_desc");
                        dtReport.Columns.Remove("created_on");
                        dtReport.Columns.Remove("customer_id");
                        dtReport.Columns.Remove("atomic_id");
                        dtReport.Columns.Remove("site_type");
                        dtReport.Columns.Remove("wfmcomment");
                        dtReport.Columns.Remove("ISSUE_ID");
                        dtReport.Columns.Remove("task_tracking_id");
                        dtReport.Columns.Remove("vsf_ticketid");
                        dtReport.Columns.Remove("patroller_user_name");
                        dtReport.Columns.Remove("patroller_remark");
                        dtReport.Columns.Remove("modified_on");
                        dtReport.Columns.Remove("ADDRESSID");
                        dtReport.Columns.Remove("CLUSTER_ID");
                        dtReport.Columns.Remove("CLUSTER_NAME");
                        dtReport.Columns.Remove("SOCIETY_NAME");
                        dtReport.Columns.Remove("CUSTOMER_ADD");
                        dtReport.Columns.Remove("CIRCLE_NAME");
                        dtReport.Columns.Remove("is_attachment");
                        dtReport.Columns.Remove("TICKET_SOURCE");
                        dtReport.Columns.Remove("hold_rc_code");
                        dtReport.Columns.Remove("hold_rca_code");
                        dtReport.Columns.Remove("hold_remarks");
                        dtReport.Columns.Remove("hold_appointment_date");
                        dtReport.Columns.Remove("rc");
                        dtReport.Columns.Remove("cpe_type");
                        dtReport.Columns.Remove("cpe_portno");
                        dtReport.Columns.Remove("cpe_model");
                        dtReport.Columns.Remove("local_convergence_point");
                        dtReport.Columns.Remove("current_cpesn");
                        dtReport.Columns.Remove("issue_comment");
                        dtReport.Columns.Remove("cpe_mac_address");
                        dtReport.Columns.Remove("customer_category");
                        dtReport.Columns.Remove("customer_segment");
                        dtReport.Columns.Remove("bandwidth");
                        dtReport.Columns.Remove("tasktype");
                        dtReport.Columns.Remove("taskcategory");
                        dtReport.Columns.Remove("tasksubcategory");
                        dtReport.Columns.Remove("checkout_remarks");
                        dtReport.Columns.Remove("checkin_remarks");
                        dtReport.Columns.Remove("TICKET_SOURCE_ID");
                        dtReport.Columns["HPSMID"].ColumnName = "JOBID";

                    }
                }

                string fileName = "ViewAdditionalMaterialReport";
                fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                ExportData(dtReport, fileName);
            }
        }

        public JsonResult BindProvinceListByRegion(string regionname)
        {
            var objResp = BLJobOrder.getprovince(regionname).Split('|').ToList();
            JsonResult r = Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
            return r;
        }
        public ActionResult wfmcharts(chartsjson objchartdata)
        {
            ViewAssignJobOrder objviewassignjoborder = new ViewAssignJobOrder();
            int page = 0;
            string sort = "";
            string sortdir = "";
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            User objUserDetails = new BLUser().getUserDetails(managerId);
            RoleMaster objRoleMaster = new BLRoles().getUserRoleNameByRoleId(objUserDetails.role_id);
            if (string.IsNullOrEmpty(objchartdata.Chartsfilter.fromDate))
            {
                DateTime ago = DateTime.Now.AddMonths(-1);
                objchartdata.Chartsfilter.fromDate = MiscHelper.FormatDate(ago.ToString());
            }

            if (string.IsNullOrEmpty(objchartdata.Chartsfilter.toDate))
            {
                objchartdata.Chartsfilter.toDate = MiscHelper.FormatDate(DateTime.Now.ToString());
            }

            GetUserPermissionArea objpermissionarea = BLJobOrder.GetAllPermissionArea(managerId);
            List<User_Master> lstUserDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();
            List<User_Master> lstDispatcherDetail = BLUser.GetMMSubordinateDetails(objUserDetails.manager_id, "");
            objviewassignjoborder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_view_status.ToString()).ToList();
            objviewassignjoborder.lstServiceType = BLJobOrder.GetServicesType();
            objviewassignjoborder.lstJoType = BLJobOrder.GetJoType();
            objviewassignjoborder.lstIssueRaisedByDetail = lstDispatcherDetail;
            objviewassignjoborder.lstIssueAssignedToDetail = lstUserDetail;
            objviewassignjoborder.viewAssignJobOrderFilter.managerId = managerId;
            objviewassignjoborder.viewAssignJobOrderFilter.pageSize = 10;
            objviewassignjoborder.viewAssignJobOrderFilter.fromDate = MiscHelper.FormatDate(objchartdata.Chartsfilter.fromDate);
            objviewassignjoborder.viewAssignJobOrderFilter.toDate = MiscHelper.FormatDate(objchartdata.Chartsfilter.toDate);
            objviewassignjoborder.viewAssignJobOrderFilter.page = page;
            objviewassignjoborder.viewAssignJobOrderFilter.sort = sort;
            objviewassignjoborder.viewAssignJobOrderFilter.sortdir = sortdir;
            objviewassignjoborder.viewAssignJobOrderFilter.currentPage = 0;

            //DEFAULT SETTINGS 
            objchartdata.lststatus = objviewassignjoborder.lststatus;
            objchartdata.lstServiceType = objviewassignjoborder.lstServiceType;
            objchartdata.lstJoType = objviewassignjoborder.lstJoType;
            //objchartdata.lstJoCategory = BLJobOrder.GetJoCategory();
            if (objchartdata.Chartsfilter.ticketsource == "SLR")
            {
                objviewassignjoborder.viewAssignJobOrderFilter.ticketSource = "4";
            }
            else if (objchartdata.Chartsfilter.ticketsource == "SLI" || string.IsNullOrEmpty(objchartdata.Chartsfilter.ticketsource))
            {
                objviewassignjoborder.viewAssignJobOrderFilter.ticketSource = "1";
            }

            //List<VW_Assign_Job_Order> lstAssignIssueDetails = BLJobOrder.GetAssignJobOrderDetails(objviewassignjoborder.viewAssignJobOrderFilter, out totalRecords);

            if (string.IsNullOrEmpty(objchartdata.Chartsfilter.JoAssignedTo))
            {
                objchartdata.Chartsfilter.JoAssignedTo = "FE";
            }
            objviewassignjoborder.viewAssignJobOrderFilter.status = objchartdata.Chartsfilter.DboardStatus;
            objviewassignjoborder.viewAssignJobOrderFilter.serviceType = objchartdata.Chartsfilter.DboardService;
            objviewassignjoborder.viewAssignJobOrderFilter.joType = objchartdata.Chartsfilter.DboardOrderType;

            if (objpermissionarea != null)
            {
                objchartdata.lstprovince_name = objpermissionarea.province_name.Split('|').ToList();
                //objchartdata.lstregion_name = objpermissionarea.region_name.Split('|').ToList();
            }

            if (objchartdata.lstprovince_name != null)
            {
                if (string.IsNullOrEmpty(objchartdata.Chartsfilter.province_name))
                {
                    objchartdata.Chartsfilter.province_name = objchartdata.lstprovince_name[0];
                    objchartdata.lstsubdistrict_name = BLJobOrder.getsubdistrict(objchartdata.Chartsfilter.province_name).Split('|').ToList();
                }
                else
                {
                    objchartdata.lstsubdistrict_name = BLJobOrder.getsubdistrict(objchartdata.Chartsfilter.province_name).Split('|').ToList();
                }
            }

            objchartdata.Chartsfilter.user_name = objUserDetails.user_name;
            objchartdata.Chartsfilter.role_name = objRoleMaster.role_name;
            objchartdata.Chartsfilter.action_name = "wfmcharts";
            objchartdata.jostatus = BLJobOrder.getwfmdashboardjostatus(objviewassignjoborder);
            objchartdata.jofacility = BLJobOrder.getwfmdashboardjofacility(objviewassignjoborder);
            objchartdata.citywisechartdata = BLJobOrder.getwfmdashboardjocity(objviewassignjoborder, objchartdata.Chartsfilter.province_name, objchartdata.lstsubdistrict_name);
            objchartdata.issue_description = BLJobOrder.getwfmdashboardjoissuedesc(objviewassignjoborder);
            objchartdata.assgintofrtchartdata = BLJobOrder.getwfmdashboardjoassignto(objviewassignjoborder, objchartdata.Chartsfilter.JoAssignedTo);
            return PartialView("_charts", objchartdata);
        }
        public ActionResult wfmchartsRM(chartsjson objchartdata)
        {
            ViewJobOrder objviewJobOrder = new ViewJobOrder();
            int page = 0;
            string sort = "";
            string sortdir = "";
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            User objUserDetails = new BLUser().getUserDetails(managerId);
            RoleMaster objRoleMaster = new BLRoles().getUserRoleNameByRoleId(objUserDetails.role_id);

            if (string.IsNullOrEmpty(objchartdata.Chartsfilter.fromDate))
            {
                DateTime ago = DateTime.Now.AddMonths(-1);
                objchartdata.Chartsfilter.fromDate = MiscHelper.FormatDate(ago.ToString());
            }
            else
            {
                //
            }

            if (string.IsNullOrEmpty(objchartdata.Chartsfilter.toDate))
            {
                objchartdata.Chartsfilter.toDate = MiscHelper.FormatDate(DateTime.Now.ToString());
            }
            else
            {
                //
            }
            GetUserPermissionArea objpermissionarea = BLJobOrder.GetAllPermissionArea(managerId);
            List<User_Master> lstJoManagerDetail = null;
            List<User_Master> lstGroupDetail = null;
            if (objRoleMaster.role_name == "Regional Head")
            {
                objviewJobOrder.vwJobOrderFilter.isLoginRole = 2;
                lstJoManagerDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();
                lstGroupDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.jomanagerId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.jomanagerId, "").ToList();
            }
            else
            {
                lstGroupDetail = BLUser.GetMMSubordinateDetails(managerId, "").ToList();
            }
            var lstSubourdinateDetail = BLUser.GetMMSubordinateDetails(objviewJobOrder.vwJobOrderFilter.groupmanagerId == 0 ? -1 : objviewJobOrder.vwJobOrderFilter.groupmanagerId, "").ToList();
            objviewJobOrder.lstJoType = BLJobOrder.GetJoType();
            objviewJobOrder.lstServiceType = BLJobOrder.GetServicesType();
            objviewJobOrder.lstJoCategory = BLJobOrder.GetJoCategory();
            objviewJobOrder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_view_status.ToString()).ToList();
            //objviewJobOrder.lststatus = BLJobOrder.GetDropDownList(DropDownType.JO_assign_status.ToString()).ToList();
            objviewJobOrder.vwJobOrderFilter.userId = managerId;
            objviewJobOrder.vwJobOrderFilter.pageSize = objviewJobOrder.vwJobOrderFilter.pageSize == 0 ? 10 : objviewJobOrder.vwJobOrderFilter.pageSize;
            objviewJobOrder.vwJobOrderFilter.fromDate = MiscHelper.FormatDate(objchartdata.Chartsfilter.fromDate);
            objviewJobOrder.vwJobOrderFilter.toDate = MiscHelper.FormatDate(objchartdata.Chartsfilter.toDate);
            objviewJobOrder.vwJobOrderFilter.page = page;
            objviewJobOrder.vwJobOrderFilter.sort = sort;
            objviewJobOrder.vwJobOrderFilter.sortdir = sortdir;
            objviewJobOrder.vwJobOrderFilter.currentPage = page == 0 ? 1 : page;
            objviewJobOrder.lstJoDetailDetails = lstJoManagerDetail;
            objviewJobOrder.lstGroupDetails = lstGroupDetail;
            objviewJobOrder.lstUserDetail = lstSubourdinateDetail;

            objchartdata.lstJoManager = objviewJobOrder.lstJoDetailDetails;
            objchartdata.lststatus = objviewJobOrder.lststatus;
            objchartdata.lstServiceType = objviewJobOrder.lstServiceType;
            objchartdata.lstJoType = objviewJobOrder.lstJoType;
            if (objchartdata.Chartsfilter.ticketsource == "SLR")
            {
                objviewJobOrder.vwJobOrderFilter.ticketSource = "4";
            }
            else if (objchartdata.Chartsfilter.ticketsource == "SLI" || string.IsNullOrEmpty(objchartdata.Chartsfilter.ticketsource))
            {
                objviewJobOrder.vwJobOrderFilter.ticketSource = "1";
            }

            if (string.IsNullOrEmpty(objchartdata.Chartsfilter.JoAssignedTo))
            {
                objchartdata.Chartsfilter.JoAssignedTo = "FE";
            }
            else
            {
                //
            }
            objviewJobOrder.vwJobOrderFilter.status = objchartdata.Chartsfilter.DboardStatus;
            objviewJobOrder.vwJobOrderFilter.serviceType = objchartdata.Chartsfilter.DboardService;
            objviewJobOrder.vwJobOrderFilter.joType = objchartdata.Chartsfilter.DboardOrderType;
            objviewJobOrder.vwJobOrderFilter.jomanagerId = objchartdata.Chartsfilter.jomanagerId;
            if (objpermissionarea != null)
            {
                objchartdata.lstregion_name = objpermissionarea.region_name.Split('|').ToList();
                //objchartdata.lstprovince_name = objpermissionarea.province_name.Split('|').ToList();
            }

            if (objchartdata.lstregion_name != null)
            {
                if (string.IsNullOrEmpty(objchartdata.Chartsfilter.region_name))
                {
                    objchartdata.Chartsfilter.region_name = objchartdata.lstregion_name[0];
                    objchartdata.lstprovince_name = BLJobOrder.getprovince(objchartdata.Chartsfilter.region_name).Split('|').ToList();
                }
                else
                {
                    objchartdata.lstprovince_name = BLJobOrder.getprovince(objchartdata.Chartsfilter.region_name).Split('|').ToList();
                    //objchartdata.province_name = "";
                }
            }

            if (objchartdata.lstprovince_name != null)
            {
                if (string.IsNullOrEmpty(objchartdata.Chartsfilter.province_name))
                {
                    objchartdata.Chartsfilter.province_name = objchartdata.lstprovince_name[0];
                    objchartdata.lstsubdistrict_name = BLJobOrder.getsubdistrict(objchartdata.Chartsfilter.province_name).Split('|').ToList();
                }
                else
                {
                    objchartdata.lstsubdistrict_name = BLJobOrder.getsubdistrict(objchartdata.Chartsfilter.province_name).Split('|').ToList();
                }
            }
            //List<VW_Route_Issue> lstrouteIssues = BLJobOrder.GetJobOrderDetailsRM(objviewJobOrder.vwJobOrderFilter, out int totalRecords);
            objchartdata.Chartsfilter.user_name = objUserDetails.user_name;
            objchartdata.Chartsfilter.role_name = objRoleMaster.role_name;
            objchartdata.Chartsfilter.action_name = "wfmchartsRM";
            objchartdata.jostatus = BLJobOrder.getwfmdashboardjostatusRM(objviewJobOrder);
            objchartdata.jofacility = BLJobOrder.getwfmdashboardjofacilityRM(objviewJobOrder);
            objchartdata.citywisechartdata = BLJobOrder.getwfmdashboardjocityRM(objviewJobOrder, objchartdata.Chartsfilter.province_name, objchartdata.lstsubdistrict_name);
            objchartdata.issue_description = BLJobOrder.getwfmdashboardjoissuedescRM(objviewJobOrder);
            objchartdata.assgintofrtchartdata = BLJobOrder.getwfmdashboardjoassigntoRM(objviewJobOrder, objchartdata.Chartsfilter.JoAssignedTo);

            return PartialView("_charts", objchartdata);
        }

    }
}