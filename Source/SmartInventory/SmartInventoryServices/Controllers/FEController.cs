using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BusinessLogics;
using BusinessLogics.WFM;
using Models;
using Models.API;
using Models.WFM;
using SmartInventory.Settings;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using Utility;
using SmartInventoryServices.WebService;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Dynamic;
using System.Web.UI.WebControls;
using static Mono.Security.X509.X520;
using Ionic.Zip;

namespace SmartInventoryServices.Controllers
{
    [Authorize]
    [RoutePrefix("wfm/mobile/v1.0")]
    [HandleException]
    public class FEController : ApiController
    {
        [Route("getJobList")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getJobList(AssignedTaskDetailIn obRequest)
        {
            var response = new WFMMobileApiResponse<dynamic>();

            if (ModelState.IsValid)
            {
                try
                {
                    // GetAssignedTaskDetailsIn objData = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAssignedTaskDetailsIn>(userData.data);// ReqHelper.GetRequestData<GetAssignedTaskDetailsIn>(userData);

                    //Decrypt userid
                    obRequest.user_id = WfmEncryption.Decrypt(Convert.ToString(obRequest.user_id));
                    obRequest.jobid = WfmEncryption.Decrypt(Convert.ToString(obRequest.jobid));
                    int dateDiff = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MobileTimeDiff"]);
                    dateDiff = dateDiff == 0 ? -30 : dateDiff;
                    string toDate = ""; //to date is set as blank as this will include future tasks as well... //MiscHelper.FormatDate(DateTime.Now.ToString());
                    string fromDate = Utility.MiscHelper.FormatDate(DateTime.Now.AddDays(dateDiff).ToString());
                    List<AssignedTaskDetail> lstAssignedTaskDetail = BLWFMTicket.GetFRTTaskDetails(Convert.ToInt32(obRequest.user_id), toDate, fromDate, obRequest.status, obRequest.jobid);

                    if (lstAssignedTaskDetail.Count > 0)
                    {
                        response.results = lstAssignedTaskDetail;
                        response.status = StatusCodes.OK.ToString();
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                    logHelper.ApiLogWriter("getJobList()", "FEController", responeData, ex);
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }
        [Route("getTTJobList")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getTTJobList(AssignedTaskDetailIn obRequest)
        {
            var response = new WFMMobileApiResponse<dynamic>();

            if (ModelState.IsValid)
            {
                try
                {
                    // GetAssignedTaskDetailsIn objData = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAssignedTaskDetailsIn>(userData.data);// ReqHelper.GetRequestData<GetAssignedTaskDetailsIn>(userData);
                    //Decrypt userid
                    obRequest.user_id = WfmEncryption.Decrypt(Convert.ToString(obRequest.user_id));
                    obRequest.jobid = WfmEncryption.Decrypt(Convert.ToString(obRequest.jobid));
                    int dateDiff = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MobileTimeDiff"]);
                    dateDiff = dateDiff == 0 ? -30 : dateDiff;
                    string toDate = ""; //to date is set as blank as this will include future tasks as well... //MiscHelper.FormatDate(DateTime.Now.ToString());
                    string fromDate = Utility.MiscHelper.FormatDate(DateTime.Now.AddDays(dateDiff).ToString());
                    List<AssignedTaskDetailTT> lstAssignedTaskDetail = BLWFMTicket.GetTTFRTTaskDetails(Convert.ToInt32(obRequest.user_id), toDate, fromDate, obRequest.status, obRequest.jobid);

                    if (lstAssignedTaskDetail.Count > 0)
                    {
                        response.results = lstAssignedTaskDetail;
                        response.status = StatusCodes.OK.ToString();
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                    logHelper.ApiLogWriter("getTTJobList()", "FEController", responeData, ex);
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }
        [Route("getJobListByLatLng")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getJobListByLatLng(AssignedTaskDetailIn obRequest)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            var resout = new WFMMobileApiResponse<dynamic>();
            try
            {
                List<AssignedTaskDetail> lstres = new List<AssignedTaskDetail>();
                List<AssignedTaskDetail> result = new List<AssignedTaskDetail>();
                if (ModelState.IsValid)
                {
                    response = getJobList(obRequest);
                    List<AssignedTaskDetail> lstAssignedTaskDetail = response.results;

                    if (response.results != null)
                    {
                        result = lstAssignedTaskDetail.Where(w => w.latitude != 0 && w.longitude != 0).ToList();
                        result.ForEach(s =>
                        {
                            s.distance = LatLngDistance.distance(s.latitude, obRequest.latitude, s.longitude, obRequest.longitude);
                            lstres.Add(s);

                        });
                        if (lstres.Count > 0)
                        {
                            resout.results = lstres.OrderBy(o => o.distance).Take(1).ToList();
                            resout.status = StatusCodes.OK.ToString();
                        }
                        else
                        {
                            resout.status = StatusCodes.OK.ToString();
                            resout.error_message = "No record found.";
                        }
                        return resout;
                    }
                }
            }
            catch (Exception ex)
            {
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(obRequest);
                logHelper.ApiLogWriter("getJobListByLatLng()", "FEController", responeData, ex);
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }


        [Route("UpdateJobOrder")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> UpdateJobOrder(JobOrderDetail obj)
        {
            var response = new WFMMobileApiResponse<dynamic>();

            if (ModelState.IsValid)
            {
                try
                {
                    //var step_order = Convert.ToInt32(HttpContext.Current.Request.Params["step_order"]);
                    //if (step_order == 0)
                    //{
                    //    response.error_message = "Invalid input! steps values is zero";
                    //    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    //    return response;

                    //}
                    obj.job_id = WfmEncryption.Decrypt(Convert.ToString(obj.job_id));
                    obj.stage = obj.step_order;
                    int res = BLWFMTicket.UpdateJobOrderDetail(obj);

                    if (res > 0)
                    {
                        //response.results = res;
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "Record with Job Id: " + obj.job_id + " updated.";
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    logHelper.ApiLogWriter("UpdateJobOrder()", "FEController", responeData, ex);
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }
        [Route("getCustomerDetail")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getCustomerDetail(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();

            if (ModelState.IsValid)
            {
                try
                {
                    Models.WFM.getDetailIn obj = ReqHelper.GetRequestData<Models.WFM.getDetailIn>(data);

                    obj.job_id = WfmEncryption.Decrypt(Convert.ToString(obj.job_id));
                    customer_detail lstCustomerDetail = BLWFMTicket.getCustomerDetail(obj.job_id);

                    if (lstCustomerDetail != null)
                    {
                        response.results = lstCustomerDetail;
                        response.status = StatusCodes.OK.ToString();
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    logHelper.ApiLogWriter("UpdateJobOrder()", "FEController", responeData, ex);
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }
        [Route("getCPEDetail")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getCPEDetail(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();

            if (ModelState.IsValid)
            {
                try
                {
                    Models.WFM.getDetailIn obj = ReqHelper.GetRequestData<Models.WFM.getDetailIn>(data);
                    cpe_detail lstCPEDetail = BLWFMTicket.getCPEDetail(obj.job_id);

                    if (lstCPEDetail != null)
                    {
                        response.results = lstCPEDetail;
                        response.status = StatusCodes.OK.ToString();
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    logHelper.ApiLogWriter("getCPEDetail()", "FEController", responeData, ex);
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }
        [Route("UpdateStatus")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> UpdateStatus(job_order_status obj)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            //if (ModelState.IsValid)
            //{
            try
            {
                int res = 0;
                string str = "";
                var iscpeactivateapi = Convert.ToBoolean(ConfigurationManager.AppSettings["iscpeactivateapi"]);
                var task = BLWFMTicket.GetJobDetailByJobOrderId(obj.job_id);
                if ((obj.action.ToUpper() == "Re-Scheduled".ToUpper()))
                {
                    str = BLWFMTicket.UpdateStatus_rch(obj);
                }
                else
                {
                    if ((obj.action.ToUpper() == "CHECK-IN".ToUpper()))
                    {
                        if (task.facility == "STATIC_IP" && task.service_status != "Static IP activated")
                        {
                            response.status = StatusCodes.OK.ToString();
                            response.error_message = "Static IP  not activated, JO:" + obj.job_id;
                            return response;
                        }
                        else
                        {
                            res = BLWFMTicket.UpdateStatus(obj);
                        }

                    }
                    if ((obj.action.ToUpper() == "HOLD".ToUpper()))
                    {
                        res = BLWFMTicket.UpdateStatus(obj);
                        if (task.ticket_source_id == 4)
                        {

                            if (res > 0)
                            {
                                if (iscpeactivateapi)
                                {
                                    // closeTicket

                                    CloseTicket CT = new CloseTicket() { itemID = task.parent_hpsmid, remarks = obj.remarks, outcomeRefId = "HOLD", rc = obj.rc, rca = obj.rca };
                                    response = closeTicket(CT);
                                    if (response.status == StatusCodes.OK.ToString())
                                    {
                                        response.error_message = "Job " + obj.action + " status updated successfully";
                                        response.status = StatusCodes.OK.ToString();

                                    }
                                    else
                                    {
                                        response.error_message = "Job " + obj.action + " status updated successfully" + ", but unable to " + obj.action + "  in hobs!!";
                                        response.status = StatusCodes.OK.ToString();
                                        return response;
                                    }
                                }

                                else
                                {
                                    response.error_message = "Job " + obj.action + " status updated successfully";
                                    response.status = StatusCodes.OK.ToString();
                                }
                            }
                        }
                    }
                    else
                    {

                        res = BLWFMTicket.UpdateStatus(obj);
                    }

                }
                if (str != "")
                {
                    if (str == "success")
                    {
                        res = 1;
                    }
                    else if (str == "fail") { res = 0; }
                    else
                    {
                        res = 0;
                        response.status = StatusCodes.FAILED.ToString();
                        response.error_message = str;
                        return response;
                    }
                }
                if (res > 0)
                {
                    if (obj.action == "Reject")
                    {
                        var objRouteIssue = BLWFMTicket.GetRoute_Issue(res);
                        ViewManagerRouteIssueApprove objMRIA = new ViewManagerRouteIssueApprove();
                        objMRIA.issueId = objRouteIssue.issue_id;
                        objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                        objMRIA.user_id = objRouteIssue.manager_id;
                        objMRIA.frtUserId = objRouteIssue.manager_id;
                        objMRIA.status = "UnAssigned";
                        objMRIA.remarks = "Assigned Ticket To Manager,Reason:" + obj.remarks;
                        objMRIA.assignedDate = DateTime.Now;

                        if (!SaveRouteIssueStatus(objMRIA))
                        {
                            response.error_message = "Unable to Assign Ticket to Manager";
                            response.status = StatusCodes.FAILED.ToString();
                            return response;

                        }

                        else
                        {
                            BLWFMTicket.DeleteTaskTrackingByIssueId(objRouteIssue.issue_id, objRouteIssue.user_id);
                            if (task.ticket_source_id == 4)
                            {

                                if (res > 0)
                                {
                                    if (iscpeactivateapi)
                                    {
                                        // closeTicket

                                        CloseTicket CT = new CloseTicket() { itemID = task.parent_hpsmid, remarks = obj.remarks, outcomeRefId = "Assign Back to Manager", rc = obj.rc, rca = obj.rca };
                                        response = closeTicket(CT);
                                        if (response.status == StatusCodes.OK.ToString())
                                        {
                                            response.error_message = "Job " + obj.action + " status updated successfully";
                                            response.status = StatusCodes.OK.ToString();

                                        }
                                        else
                                        {
                                            response.error_message = "Job " + obj.action + " status updated successfully" + " but unable to " + obj.action + " in hobs!!";
                                            response.status = StatusCodes.OK.ToString();
                                            return response;
                                        }
                                    }

                                    else
                                    {
                                        response.error_message = "Job " + obj.action + " status updated successfully";
                                        response.status = StatusCodes.OK.ToString();
                                    }
                                }
                            }
                            //try
                            //{
                            //    NotificationHelper notificatonHelper = new NotificationHelper();
                            //    var ids = objMRIA.issuesId.Split(',');
                            //    for (int i = 0; i < ids.Length; i++)
                            //    {
                            //        string strNotMess = "Task/Ticket id:" + objTicket.HPSMID + " has been assigned sent without resolve.";
                            //        notificatonHelper.sendNotification(objMRIA.frtUserId, Convert.ToInt32(ids[i]), strNotMess, "Task/Ticket Not Resolved");
                            //    }
                            //}
                            //catch (Exception ec)
                            //{
                            //    ErrorLogHelper logHelper = new ErrorLogHelper();
                            //    logHelper.ApiLogWriter("SendTicketToManager() + notificatonHelper", "FRT Controller", userData.data, ec);
                            //}
                        }
                    }
                    if (obj.action.ToUpper() == "ASSIGN TO MANAGER".ToUpper())
                    {
                        var objRouteIssue = BLWFMTicket.GetRoute_Issue(res);
                        ViewManagerRouteIssueApprove objMRIA = new ViewManagerRouteIssueApprove();
                        objMRIA.issueId = objRouteIssue.issue_id;
                        objMRIA.issuesId = objRouteIssue.issue_id.ToString();
                        objMRIA.user_id = objRouteIssue.manager_id;
                        objMRIA.frtUserId = objRouteIssue.manager_id;
                        objMRIA.status = "UnAssigned";
                        objMRIA.remarks = "Assigned Ticket To Manager,Reason:" + obj.remarks;
                        objMRIA.assignedDate = DateTime.Now;

                        if (!SaveRouteIssueStatus(objMRIA))
                        {
                            response.error_message = "Unable to Assign Ticket to Manager";
                            response.status = StatusCodes.FAILED.ToString();
                            return response;

                        }

                        else
                        {
                            BLWFMTicket.DeleteTaskTrackingByIssueId(objRouteIssue.issue_id, objRouteIssue.user_id);
                            if (task.ticket_source_id == 4)
                            {

                                if (res > 0)
                                {
                                    if (iscpeactivateapi)
                                    {
                                        // closeTicket

                                        CloseTicket CT = new CloseTicket() { itemID = task.parent_hpsmid, remarks = obj.remarks, outcomeRefId = "Assign Back to Manager", rc = obj.rc, rca = obj.rca };
                                        response = closeTicket(CT);
                                        if (response.status == StatusCodes.OK.ToString())
                                        {
                                            response.error_message = "Job " + obj.action + " status updated successfully";
                                            response.status = StatusCodes.OK.ToString();

                                        }
                                        else
                                        {
                                            response.error_message = "Job " + obj.action + " status updated successfully" + " but unable to " + obj.action + " in hobs!!";
                                            response.status = StatusCodes.OK.ToString();
                                            return response;
                                        }
                                    }

                                    else
                                    {
                                        response.error_message = "Job " + obj.action + " status updated successfully";
                                        response.status = StatusCodes.OK.ToString();
                                    }
                                }
                            }
                            //try
                            //{
                            //    NotificationHelper notificatonHelper = new NotificationHelper();
                            //    var ids = objMRIA.issuesId.Split(',');
                            //    for (int i = 0; i < ids.Length; i++)
                            //    {
                            //        string strNotMess = "Task/Ticket id:" + objTicket.HPSMID + " has been assigned sent without resolve.";
                            //        notificatonHelper.sendNotification(objMRIA.frtUserId, Convert.ToInt32(ids[i]), strNotMess, "Task/Ticket Not Resolved");
                            //    }
                            //}
                            //catch (Exception ec)
                            //{
                            //    ErrorLogHelper logHelper = new ErrorLogHelper();
                            //    logHelper.ApiLogWriter("SendTicketToManager() + notificatonHelper", "FRT Controller", userData.data, ec);
                            //}
                        }
                    }
                    response.error_message = "Job " + obj.action + " status updated successfully";
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "No record found.";
                }

                return response;
            }
            catch (Exception ex)
            {
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                logHelper.ApiLogWriter("UpdateStatus()", "FEController", responeData, ex);
                response.error_message = "Error While Processing  Request.";
            }
            // }
            return response;
        }

        private bool SaveRouteIssueStatus(ViewManagerRouteIssueApprove viewRouteIssueApprove)
        {
            VW_Route_Issue objRouteIssue = new VW_Route_Issue();
            bool save = false;
            viewRouteIssueApprove.checkinRadius = 5000;// Convert.ToInt32(ApplicationSettings.DefaultTaskCheckinRadius);
            if (viewRouteIssueApprove.status == "Assigned" || viewRouteIssueApprove.status == "UnAssigned")
            {
                List<Vw_Hpsm_Ticket_Status> hpsmTicketList = new List<Vw_Hpsm_Ticket_Status>();
                save = BLWFMTicket.AssignRouteIssue(viewRouteIssueApprove, out hpsmTicketList);
                //string IsHPSMCall = System.Configuration.ConfigurationManager.AppSettings["HPSMCall"].ToString();
                //if (!string.IsNullOrEmpty(IsHPSMCall))
                //{
                //    //Durgesh unnecessary calls to api 16.9.2021
                //    //if (IsHPSMCall == "true")
                //    //{

                //    //    for (int index = 0; index < hpsmTicketList.Count; index++)
                //    //    {
                //    //        HPSMHelper hpsmHelper = new HPSMHelper(hpsmTicketList[index]);
                //    //    }
                //    //}
                //}
            }
            return save;
        }
        [Route("UploadAttachment")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> UploadAttachment()
        {
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                HttpPostedFile files = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
                if (HttpContext.Current.Request.Files.Count > 0 && files != null)
                {

                    var job_id = HttpContext.Current.Request.Params["job_id"];
                    var screen = HttpContext.Current.Request.Params["screen"];
                    var doc_type = HttpContext.Current.Request.Params["doc_type"];
                    var attachmentType = HttpContext.Current.Request.Params["uploadType"];
                    var remark = HttpContext.Current.Request.Params["remark"];
                    var UserId = HttpContext.Current.Request.Params["userId"];

                    var step_order = Convert.ToInt32(HttpContext.Current.Request.Params["step_order"]);
                    if (step_order == 0)
                    {
                        response.error_message = "Invalid input! steps values is zero";
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        return response;

                    }

                    if (screen == "SIGNATURE" && (string.IsNullOrEmpty(doc_type) || doc_type == "null"))
                    {
                        doc_type = "SIGNATURE";
                    }
                    else if (screen == "CONTESTING" && (string.IsNullOrEmpty(doc_type) || doc_type == "null"))
                    {
                        doc_type = "Connection Testing";
                    }
                    var validDocumentTypes = ApplicationSettings.validDocumentTypes.Split(new string[] { "," }, StringSplitOptions.None);
                    var validImageTypes = ApplicationSettings.validImageTypes.Split(new string[] { "," }, StringSplitOptions.None);

                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {

                        string FileName = files.FileName;
                        var fileExtension = Path.GetExtension(FileName);

                        if ((ApplicationSettings.MaxuploadFileSize < files.ContentLength / 1024 / 1024) && (attachmentType.ToUpper() == "DOCUMENT"))
                        {
                            response.error_message = String.Format(Resources.Resources.SI_OSP_GBL_JQ_FRM_109, ApplicationSettings.MaxuploadFileSize);
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }
                        if ((ApplicationSettings.MaxuploadFileSize < files.ContentLength / 1024 / 1024) && (attachmentType.ToUpper() == "IMAGE"))
                        {
                            response.error_message = String.Format(Resources.Resources.SI_OSP_GBL_JQ_GBL_112, ApplicationSettings.MaxuploadFileSize);
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }

                        if ((attachmentType != null && attachmentType.ToUpper() == "DOCUMENT") && !validDocumentTypes.Contains(fileExtension.ToLower()))
                        {
                            response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_059 + " extension:" + ApplicationSettings.validDocumentTypes;
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }
                        if ((attachmentType != null && attachmentType.ToUpper() == "IMAGE") && !validImageTypes.Contains(fileExtension.ToLower()))
                        {
                            response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_059 + " extension:" + ApplicationSettings.validImageTypes;
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }
                        //var lstDocument = BLWFMTicket.getAttachmentDetailsbyId(job_id, attachmentType, FileName);
                        //if (lstDocument.Count > 0)
                        //{
                        //    response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_055;
                        //    response.status = StatusCodes.INVALID_FILE.ToString();
                        //    return response;
                        //}

                        string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        string strFilePath = "";
                        strFilePath = ReqHelper.UploadfileOnFTP("", job_id, files, attachmentType, strNewfilename);
                        hpsm_ticket_attachments objAttachment = new hpsm_ticket_attachments();
                        objAttachment.job_id = job_id;
                        objAttachment.org_file_name = FileName;
                        objAttachment.file_name = strNewfilename;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_location = strFilePath;
                        objAttachment.upload_type = attachmentType;
                        objAttachment.uploaded_by = Convert.ToInt32(UserId);
                        objAttachment.file_size = files.ContentLength;
                        objAttachment.uploaded_on = DateTime.Now;
                        objAttachment.screen = screen;
                        //objAttachment.remark = remark;
                        objAttachment.doc_type = doc_type;


                        //Save Image on FTP and related detail in database..
                        var savefile = BLWFMTicket.SaveTicketAttachment(objAttachment);

                    }
                    if (screen == "ONT" || screen == "WIFI" || screen == "ANALYSIS" || screen == "JOBDOC")
                    {
                        //update stage
                        // BLWFMTicket.UpdateJobOrderStage(job_id, step_order, null);
                    }
                    else if (screen == "CONTESTING")
                    {
                        //update stage + remark
                        BLWFMTicket.UpdateJobOrderStage(job_id, 0, null, remark);
                        response.error_message = "Connection testing completed Successfully.";
                        response.status = StatusCodes.OK.ToString();
                        return response;
                    }
                    else if (screen == "SIGNATURE")
                    {
                        job_order_status objstatus = new job_order_status();
                        objstatus.job_id = job_id;
                        objstatus.action = "Completed";
                        objstatus.remarks = "Job order completed";



                        var iscpeactivateapi = Convert.ToBoolean(ConfigurationManager.AppSettings["iscpeactivateapi"]);
                        string msg = "Job order completed successfuly.";
                        var task = BLWFMTicket.GetJobDetailByJobOrderId(job_id);
                        if (task.ticket_source_id != 4)
                        {
                            if (iscpeactivateapi)
                            {
                                //Call closer Api
                                var finalTrigger = triggerFinalConfirmation(job_id);
                                if (finalTrigger.results == true)
                                {
                                    //
                                    //  msg = "!!";// for success
                                    int res = BLWFMTicket.UpdateStatus(objstatus);
                                    //update stage
                                    BLWFMTicket.UpdateJobOrderStage(job_id, step_order, null);
                                    response.error_message = msg;
                                    response.status = StatusCodes.OK.ToString();
                                }
                                else
                                {
                                    //msg = msg + " but unable to close in hobs!!";
                                    msg = finalTrigger.error_message;
                                    response.error_message = msg;
                                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();

                                }
                                //if (finalTrigger.results != null && finalTrigger.status == "OK")
                                //{
                                //    var dyn = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivateCpeRes>(finalTrigger.results);
                                //    if (dyn.statusCode == "0000")
                                //    {
                                //        msg = " Close on hobs";
                                //    }

                                //}
                                //else if (finalTrigger.results == null && finalTrigger.status == "UNKNOWN_ERROR")
                                //{
                                //    msg = " But not close on hobs";
                                //}
                            }
                            else
                            {
                                //
                                //  msg = "!!";// for success
                                int res = BLWFMTicket.UpdateStatus(objstatus);
                                //update stage
                                BLWFMTicket.UpdateJobOrderStage(job_id, step_order, null);
                                response.error_message = msg;
                                response.status = StatusCodes.OK.ToString();
                            }
                        }
                        else
                        {
                            if (iscpeactivateapi)
                            {
                                ///(/closeTicket
                                CloseTicket CT = new CloseTicket() { itemID = task.parent_hpsmid, remarks = task.wfmcomment, outcomeRefId = "CloseResolutionConfirmed", rc = task.root_cause_id, rca = task.resolution_close_id };

                                response = closeTicket(CT);
                                if (response.status == StatusCodes.OK.ToString())
                                {
                                    msg = "Ticket closed successfully!";
                                    int res = BLWFMTicket.UpdateStatus(objstatus);
                                    //update stage
                                    BLWFMTicket.UpdateJobOrderStage(job_id, step_order, null);
                                    response.error_message = msg;
                                    response.status = StatusCodes.OK.ToString();
                                }
                                else
                                {
                                    msg = "Unable to close ticket";
                                    response.error_message = msg;
                                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                }
                            }
                            else
                            {
                                msg = "Ticket closed successfully!";
                                int res = BLWFMTicket.UpdateStatus(objstatus);
                                //update stage
                                BLWFMTicket.UpdateJobOrderStage(job_id, step_order, null);
                                response.error_message = msg;
                                response.status = StatusCodes.OK.ToString();
                            }
                        }

                        return response;

                    }

                    else if (screen == "FINISH_INSTALLATION")
                    {
                        job_order_status objstatus = new job_order_status();
                        objstatus.job_id = job_id;
                        objstatus.action = "Completed";
                        int res = BLWFMTicket.UpdateStatus(objstatus);
                        //update stage
                        BLWFMTicket.UpdateJobOrderStage(job_id, step_order, null);

                        var iscpeactivateapi = true;// Convert.ToBoolean(ConfigurationManager.AppSettings["iscpeactivateapi"]);
                        string msg = "Job order completed successfuly.";
                        if (iscpeactivateapi)
                        {
                            //Call closer Api
                            var finalTrigger = triggerFinalConfirmationAxtel(job_id);
                            if (finalTrigger.results == true)
                            {
                                //  msg = "!!";// for success
                            }
                            else
                            {
                                msg = msg + " but unable to close in hobs!!";
                            }
                        }
                        response.error_message = msg;
                        response.status = StatusCodes.OK.ToString();
                        return response;
                    }
                    response.error_message = attachmentType + " uploded successfully against Job ID: " + job_id;
                    response.status = StatusCodes.OK.ToString();
                    return response;
                }
                else
                {
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "File not found for upload.";
                    return response;
                }
            }
            catch (Exception ex)
            {

                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UploadAttachment()", "FEController", null, ex);
                response.error_message = "Error in uploading attachment!";
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                return response;
                //Error Logging...
            }
        }
        [Route("GetImageDocumentByJobId")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> GetImageDocumentByJobId(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                Models.WFM.GetAttachmentDetailsIn objgetAttachmentDetailsIn = ReqHelper.GetRequestData<Models.WFM.GetAttachmentDetailsIn>(data);
                string FtpUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"]);
                string strFTPUserName = System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"];
                var lstDocument = BLWFMTicket.getAttachmentDetailsbyJobId(objgetAttachmentDetailsIn.job_id, objgetAttachmentDetailsIn.upload_type, objgetAttachmentDetailsIn.screen);
                response.status = StatusCodes.OK.ToString();
                if (objgetAttachmentDetailsIn.upload_type.ToUpper() == "IMAGE")
                {
                    //int i = 0;
                    //foreach (var item in lstDocument)
                    //{
                    //    lstDocument[i].ImgSrcThumb = convertBase64Image(item.file_location, "Thumb_" + item.file_name);
                    //    lstDocument[i].ImgSrc = convertBase64Image(item.file_location, item.file_name);
                    //    i++;
                    //}
                }
                else if (objgetAttachmentDetailsIn.upload_type.ToUpper() == "DOCUMENT")
                {
                    int i = 0;
                    foreach (var item in lstDocument)
                    {
                        string attachmentUrl = string.Concat(FtpUrl, item.file_location, item.file_name);

                        WebClient request = new WebClient();
                        if (!string.IsNullOrEmpty(strFTPUserName)) //Authentication require..
                            request.Credentials = new NetworkCredential(strFTPUserName, strFTPPassWord);

                        // lstDocument[i].attachmentSource = request.DownloadData(attachmentUrl);
                        lstDocument[i].file_size_converted = ReqHelper.BytesToString(lstDocument[i].file_size);
                        i++;
                    }
                }

                response.results = lstDocument;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetImageDocumentByJobId()", "FE", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
            }
            return response;
        }


        [Route("GetImageDocumentById")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> GetImageDocumentById(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                Models.WFM.DeleteAttachmentsIn objgetAttachmentDetailsIn = ReqHelper.GetRequestData<Models.WFM.DeleteAttachmentsIn>(data);
                string FtpUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"]);
                string strFTPUserName = System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                var item = BLWFMTicket.getAttachmentDetailsbyId(objgetAttachmentDetailsIn.attachmentId);
                if (item != null)
                {
                    if (item.upload_type.ToUpper() == "IMAGE")
                    {
                        if (!string.IsNullOrEmpty(objgetAttachmentDetailsIn.image_size))
                        {

                            if (objgetAttachmentDetailsIn.image_size.ToUpper() == "SMALL")
                            {
                                item.ImgSrcThumb = convertBase64Image(item.file_location, "Thumb_" + item.file_name);
                            }
                            else if (objgetAttachmentDetailsIn.image_size.ToUpper() == "LARGE")
                            {
                                item.ImgSrc = convertBase64Image(item.file_location, item.file_name);
                            }
                        }
                        else
                        {
                            item.ImgSrcThumb = convertBase64Image(item.file_location, "Thumb_" + item.file_name);
                            item.ImgSrc = convertBase64Image(item.file_location, item.file_name);
                        }


                    }
                    else if (item.upload_type.ToUpper() == "DOCUMENT")
                    {
                        string attachmentUrl = string.Concat(FtpUrl, item.file_location, item.file_name);

                        WebClient request = new WebClient();
                        if (!string.IsNullOrEmpty(strFTPUserName)) //Authentication require..
                            request.Credentials = new NetworkCredential(strFTPUserName, strFTPPassWord);

                        item.attachmentSource = request.DownloadData(attachmentUrl);
                        item.file_size_converted = ReqHelper.BytesToString(item.file_size);
                    }
                    response.status = StatusCodes.OK.ToString();
                    response.results = item;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "No record found.";
                }
            }
            catch (Exception ex)
            {
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetImageDocumentById()", "FE", null, ex);
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
            }
            return response;
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
        [Route("DeleteAttachment")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> DeleteAttachment(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                Models.WFM.DeleteAttachmentsIn objDeleteAttachmentsIn = ReqHelper.GetRequestData<Models.WFM.DeleteAttachmentsIn>(data);
                string sFilePath = "";
                int deleteChk = 0;
                int DocumentId = objDeleteAttachmentsIn.attachmentId;
                //Get File Name and Path...
                var lstAttachmentDetails = BLWFMTicket.getAttachmentDetailsbyId(DocumentId);
                if (lstAttachmentDetails != null)
                {
                    sFilePath = lstAttachmentDetails.file_location + lstAttachmentDetails.file_name;
                    if (!string.IsNullOrWhiteSpace(sFilePath))
                    {
                        deleteChk = BLWFMTicket.DeleteAttachmentById(DocumentId);
                        if (deleteChk == 1)
                        {
                            ReqHelper.DeleteFileFromFTP(sFilePath);
                            if (objDeleteAttachmentsIn.step_order > 0)
                            {
                                objDeleteAttachmentsIn.step_order = objDeleteAttachmentsIn.step_order - 1;
                                BLWFMTicket.DeleteAttachementUpdateStage(objDeleteAttachmentsIn.job_id, objDeleteAttachmentsIn.step_order);
                            }
                        }
                        else
                        {
                            response.status = StatusCodes.INVALID_REQUEST.ToString();
                            response.error_message = "File/Image Not Found!";
                            return response;
                        }


                    }
                    else
                    {
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.error_message = "Invalid File Path!";
                        return response;
                    }
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "File has been deleted sucessfully!";
                    return response;
                }
                else
                {
                    response.status = StatusCodes.INVALID_REQUEST.ToString();
                    response.error_message = "File/Image Not Found!";
                    return response;
                }
            }

            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("DeleteAttachment()", "FE", null, ex);
                response.status = StatusCodes.EXCEPTION.ToString();
                response.error_message = "Failed to Delete";

            }
            //response.status = StatusCodes.OK.ToString();
            //response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_155;
            return response;
        }
        [Route("getTicketStepsDetail")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getTicketStepsDetail(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            Models.WFM.getDetailIn objTicketStepsDetailIn = ReqHelper.GetRequestData<Models.WFM.getDetailIn>(data);
            try
            {
                var lstTicketSteps = BLWFMTicket.getTicketStepDetails(objTicketStepsDetailIn.job_id);
                response.results = lstTicketSteps;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getTicketStepsDetail()", "FEController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }


        [Route("getTTTicketStepsDetail")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getTTTicketStepsDetail(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            Models.WFM.getDetailIn objTicketStepsDetailIn = ReqHelper.GetRequestData<Models.WFM.getDetailIn>(data);
            try
            {
                var lstTicketSteps = BLWFMTicket.getTTTicketStepDetails(objTicketStepsDetailIn.job_id);
                response.results = lstTicketSteps;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getTicketStepsDetail()", "FEController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        [Route("activatecpe")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> activatecpe(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            Models.WFM.ActivateCPEDetail objIn = ReqHelper.GetRequestData<Models.WFM.ActivateCPEDetail>(data);
            try
            {
                var iscpeactivateapi = Convert.ToBoolean(ConfigurationManager.AppSettings["iscpeactivateapi"]);
                var activateCpe = false;
                job_order_status jobOrderStatus = new job_order_status();
                var task = BLWFMTicket.GetJobDetailByJobOrderId(objIn.job_id);
                if (task.ticket_source_id == 4)
                {
                    if (iscpeactivateapi)
                    {
                        var cpeReplaceResponse = new WFMMobileApiResponse<dynamic>();
                        cpeReplaceResponse = CPEReplace(task);
                        if (cpeReplaceResponse.results == true)
                        {
                            activateCpe = true;

                            //var cpe_activate = BLWFMTicket.GetJobOrderstatus("cpe_activate").FirstOrDefault();
                            //int res = BLWFMTicket.UpdateJobOrderStage(objIn.job_id, objIn.step_order, null);
                            jobOrderStatus.job_id = objIn.job_id;
                            jobOrderStatus.action = "cpe_activate_initiate";
                            jobOrderStatus.remarks = "CPE activation is in progress !!";
                            int statusRes = BLWFMTicket.UpdateStatus(jobOrderStatus);
                            if (statusRes > 0)
                            {
                                response.status = StatusCodes.OK.ToString();
                                response.error_message = "CPE activation is in progress, Please check after some time !!";
                            }
                            else
                            {
                                response.status = StatusCodes.OK.ToString();
                                response.error_message = "No record found.";
                            }


                        }
                        else
                        {
                            response.status = cpeReplaceResponse.status;
                            response.error_message = cpeReplaceResponse.error_message;
                            return response;
                        }
                    }
                    else if (iscpeactivateapi == false)
                    {
                        activateCpe = true;
                        jobOrderStatus.job_id = objIn.job_id;
                        jobOrderStatus.action = "cpe_activate";
                        jobOrderStatus.remarks = "cpe activate";
                        int statusRes = BLWFMTicket.UpdateStatus(jobOrderStatus);
                        if (statusRes > 0)
                        {
                            response.status = StatusCodes.OK.ToString();
                            response.error_message = "CPE activate successfully";
                        }
                        else
                        {
                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            response.error_message = "Some error in updating CPE  !!";
                        }
                    }
                }
                else
                {
                    if (iscpeactivateapi)
                    {
                        var tokenResponse = new WFMMobileApiResponse<dynamic>();

                        string action = task.action == "" ? "Add" : task.action;
                        if ((action).ToUpper() == "ADD")
                        {

                            tokenResponse = getTriggerActivateDetails(objIn.job_id);

                            if (tokenResponse.results == true)
                            {
                                activateCpe = true;
                                if (activateCpe)
                                {
                                    //var cpe_activate = BLWFMTicket.GetJobOrderstatus("cpe_activate").FirstOrDefault();
                                    //int res = BLWFMTicket.UpdateJobOrderStage(objIn.job_id, objIn.step_order, null);
                                    jobOrderStatus.job_id = objIn.job_id;
                                    jobOrderStatus.action = "cpe_activate_initiate";
                                    jobOrderStatus.remarks = "CPE activation is in progress !!";
                                    int statusRes = BLWFMTicket.UpdateStatus(jobOrderStatus);
                                    if (statusRes > 0)
                                    {
                                        response.status = StatusCodes.OK.ToString();
                                        response.error_message = "CPE activation is in progress, Please check after some time !!";
                                    }
                                    else
                                    {
                                        response.status = StatusCodes.OK.ToString();
                                        response.error_message = "No record found.";
                                    }
                                }
                                else
                                {
                                    response.status = StatusCodes.FAILED.ToString();
                                    response.error_message = "CPE not activated.";
                                }
                            }
                            else
                            {
                                response.status = tokenResponse.status;
                                response.error_message = tokenResponse.error_message;
                                return response;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(objIn.IsCPECollected))
                            {
                                BLWFMTicket.UpdateCpeCollected(objIn.job_id, objIn.IsCPECollected);
                            }
                            jobOrderStatus.job_id = objIn.job_id;
                            jobOrderStatus.action = "cpe_remove";
                            jobOrderStatus.remarks = "CPE remove successfully";
                            int statusRes = BLWFMTicket.UpdateStatus(jobOrderStatus);
                            if (statusRes > 0)
                            {
                                int res = BLWFMTicket.UpdateJobOrderStage(objIn.job_id, objIn.step_order, null);
                                response.status = StatusCodes.OK.ToString();
                                response.error_message = "CPE remove successfully.";
                                return response;
                            }
                            else
                            {
                                response.status = StatusCodes.OK.ToString();
                                response.error_message = "No record found.";
                            }
                        }
                    }
                    else if (iscpeactivateapi == false)
                    {
                        string action = task.action == "" ? "Add" : task.action;
                        if ((action).ToUpper() == "ADD")
                        {
                            activateCpe = true;
                            jobOrderStatus.job_id = objIn.job_id;
                            jobOrderStatus.action = "cpe_activate";
                            jobOrderStatus.remarks = "cpe activate";
                            int statusRes = BLWFMTicket.UpdateStatus(jobOrderStatus);
                            if (statusRes > 0)
                            {
                                response.status = StatusCodes.OK.ToString();
                                response.error_message = "CPE activate successfully";

                            }
                            else
                            {
                                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                response.error_message = "Some error in updating CPE  !!";
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(objIn.IsCPECollected))
                            {
                                BLWFMTicket.UpdateCpeCollected(objIn.job_id, objIn.IsCPECollected);
                            }
                            jobOrderStatus.job_id = objIn.job_id;
                            jobOrderStatus.action = "cpe_remove";
                            jobOrderStatus.remarks = "CPE remove successfully";
                            int statusRes = BLWFMTicket.UpdateStatus(jobOrderStatus);
                            if (statusRes > 0)
                            {
                                int res = BLWFMTicket.UpdateJobOrderStage(objIn.job_id, objIn.step_order, null);
                                response.status = StatusCodes.OK.ToString();
                                response.error_message = "CPE remove successfully.";
                                return response;
                            }
                            else
                            {
                                response.status = StatusCodes.OK.ToString();
                                response.error_message = "No record found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("activatecpe()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

        //[Route("getTriggerActivateDetails")]
        //[HttpPost]
        public WFMMobileApiResponse<dynamic> getTriggerActivateDetails(string job_id)
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {

                tokenResponse = getAccessToken();
                if (tokenResponse.results != "" && tokenResponse.results != null)
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

                                var dyn = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivateCpeRes>(responses);
                                if (dyn.statusCode != "0000")
                                {
                                    response.results = false;
                                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                                    response.error_message = "CPE not activated :" + " Status desc: " + dyn.statusDesc;
                                    logHelper.ApiLogWriter("getTriggerActivateDetails()", "FEController", "Request:- " + DATA + "|| Response:- " + responses, null);
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
                            logHelper.ApiLogWriter("inner catch:getTriggerActivateDetails()", "FEController", DATA, ex);
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
                logHelper.ApiLogWriter("outer catch:getTriggerActivateDetails()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.results = false;
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        private WFMMobileApiResponse<dynamic> CPEReplace(Models.WFM.Task obj)
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                tokenResponse = getAccessToken();
                if (tokenResponse.results != "" && tokenResponse.results != null)
                {
                    dynamic cpeReplaceDetail = "";
                    string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
                    string URL = "";
                    if (obj.isresolve != "0")
                    {
                        cpeReplaceDetail = BLWFMTicket.GetReTriggerRequest(obj.hpsmid);
                        //URL = Convert.ToString(ConfigurationManager.AppSettings["cpeRETriggerReplacePath"]);
                        URL = basepath + "retriggerForReplaceCPE";
                    }
                    else
                    {
                        cpeReplaceDetail = BLWFMTicket.CPEReplace(obj.hpsmid);
                        //URL = Convert.ToString(ConfigurationManager.AppSettings["cpeReplacePath"]);
                        URL = basepath + "replaceCPE";
                    }

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                    System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    if (cpeReplaceDetail != null)
                    {
                        string Attributes = string.Empty;
                        var DATA = Newtonsoft.Json.JsonConvert.SerializeObject(cpeReplaceDetail);

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

                                var dyn = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivateCpeRes>(responses);
                                if (dyn.statusCode != "0000")
                                {
                                    response.results = false;
                                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                                    response.error_message = "CPE not activated :" + " Status desc: " + dyn.statusDesc;
                                    logHelper.ApiLogWriter("CPEReplace()", "FEController", "Request:- " + Convert.ToString(DATA) + "|| Response:- " + responses, null);
                                }
                                else
                                {
                                    response.results = true;
                                    response.status = StatusCodes.OK.ToString();
                                    if (obj.isresolve == "0")
                                    {
                                        obj.task_id = dyn.orderId;
                                        BLWFMTicket.updateHpsm_TicketMasterData(obj);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logHelper.ApiLogWriter("inner catch:CPEReplace()", "FEController", DATA, ex);
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
                logHelper.ApiLogWriter("outer catch:CPEReplace()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.results = false;
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        public WFMMobileApiResponse<dynamic> getAccessToken()
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var objresponse = new WFMMobileApiResponse<dynamic>();
            string DATA = "";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                //string URL = Convert.ToString(ConfigurationManager.AppSettings["getTokenPath"]);
                string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
                string URL = basepath+ "getToken";
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
                            logHelper.ApiLogWriter("getAccessToken()", "FEController", DATA, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logHelper.ApiLogWriter("catch:getAccessToken()", "FEController", DATA, ex);
                    objresponse.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    objresponse.error_message = "Error While Processing  Request.";
                }
            }
            catch (Exception ex)
            {
                logHelper.ApiLogWriter("outercatch:getAccessToken()", "FEController", DATA, ex);
                objresponse.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objresponse.error_message = "Error While Processing  Request.";
            }
            return objresponse;
        }


        public WFMMobileApiResponse<dynamic> triggerFinalConfirmation(string job_id)
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
            var response = new WFMMobileApiResponse<dynamic>();
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

                            var dyn = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivateCpeRes>(responses);
                            if (dyn.statusCode != "0000")
                            {
                                response.results = false;
                                response.status = StatusCodes.INVALID_INPUTS.ToString();
                                response.error_message = "unable to close in hobs :" + " Status desc: " + dyn.statusDesc;
                                logHelper.ApiLogWriter("triggerFinalConfirmation()", "FEController", "Request:- " + Convert.ToString(responeData) + "|| Response:- " + responses, null);
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

                        logHelper.ApiLogWriter("inner catch:triggerFinalConfirmation()", "FEController", Convert.ToString(responeData), ex);
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
                logHelper.ApiLogWriter("outer catch:triggerFinalConfirmation()", "FEController", "", ex);
                response.results = false;
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        public WFMMobileApiResponse<dynamic> triggerFinalConfirmationAxtel(string job_id)
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                string URL = Convert.ToString(ConfigurationManager.AppSettings["triggerFinalConfirmation"]);
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                dynamic data = new { orderId = job_id };
                var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/json";
                WebHeaderCollection aPIHeaderValues = new WebHeaderCollection();
                aPIHeaderValues.Add("opId", "HOB");
                aPIHeaderValues.Add("buId", "DEFAULT");
                request.Headers.Add(aPIHeaderValues);
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

                        var dyn = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivateCpeResAxtel>(responses);
                        if (dyn.STATUS.statusCode != "0000")
                        {
                            response.results = false;
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.error_message = "not close on hobs :" + " Status code: " + dyn.STATUS.statusCode;
                            logHelper.ApiLogWriter("getTriggerActivateDetailAxtel()", "FEController", ("Request :" + data + "Response :" + responses), null);
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

                    logHelper.ApiLogWriter("inner catch:triggerFinalConfirmationAxtel()", "FEController", Convert.ToString(data), ex);
                    response.results = false;
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            catch (Exception ex)
            {
                logHelper.ApiLogWriter("outer catch:triggerFinalConfirmationAxtel()", "FEController", null, ex);
                response.results = false;
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        [Route("getadditionalmaterialmaster")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getadditionalmaterialmaster(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            //var d = new { job_id = string.Empty };
            var objIn = ReqHelper.GetRequestData<getDetailIn>(data);
            //Get facility from current job
            if (string.IsNullOrEmpty(objIn.job_id))
            {
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                response.error_message = "job id can not be empty.";
                return response;
            }
            var task = BLWFMTicket.GetJobDetailByJobOrderId(objIn.job_id);

            if (string.IsNullOrEmpty(task.facility))
            {
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                response.error_message = "facility not found.";
                return response;
            }

            try
            {
                var lstobj = BLWFMTicket.GetAdditionalMaterialMaster(task.facility);
                if (lstobj.Count > 0)
                {
                    response.results = lstobj;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
                else
                {
                    response.status = StatusCodes.FAILED.ToString();
                    response.error_message = "No record found for " + task.facility;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getadditionalmaterialmaster()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        [Route("saveadditionalmaterial")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> saveadditionalmaterial(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            Models.WFM.AdditionalMaterialIn objIn = ReqHelper.GetRequestData<Models.WFM.AdditionalMaterialIn>(data);
            try
            {
                //var step_order = Convert.ToInt32(HttpContext.Current.Request.Params["step_order"]);
                //if (step_order == 0)
                //{
                //    response.error_message = "Invalid input! steps values is zero";
                //    response.status = StatusCodes.INVALID_INPUTS.ToString();
                //    return response;

                //}

                var res = BLWFMTicket.SaveAdditionalMaterial(objIn.jobid, objIn.additionalmaterial);

                if (res > 0)
                {
                    int stage = BLWFMTicket.UpdateJobOrderStage(objIn.jobid, objIn.step_order, null);
                    response.error_message = "additional material save successfully.";
                    response.status = StatusCodes.OK.ToString();
                    return response;
                }
                else
                {
                    response.error_message = "Unable to save additional material";
                    response.status = StatusCodes.FAILED.ToString();
                    return response;
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("saveadditionalmaterial()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        [Route("getadditionalmaterial")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getadditionalmaterial(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            Models.WFM.getDetailIn objIn = ReqHelper.GetRequestData<Models.WFM.getDetailIn>(data);
            try
            {
                var lstobj = BLWFMTicket.GetAdditionalMaterial(objIn.job_id);

                if (lstobj.Count > 0)
                {
                    response.results = lstobj;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "No record found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getadditionalmaterial()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }


        [Route("getjobstatustype")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getjobstatustype(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            dynamic objIn = ReqHelper.GetRequestData<dynamic>(data);
            string ts = "Job_status_type";
            int ticket_source = Convert.ToInt32(objIn.ticket_source);
            if (ticket_source == 4)
            {
                ts = "Job_status_type_tt";
            }

            try
            {
                var lstobj = BLWFMTicket.GetJobOrderstatus(ts, ticket_source);
                response.results = lstobj;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getjobstatustype()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }



        private string getcpeDetailFromXml(string SerialCode, string job_id)
        {
            string message = "Unable to fetch the stock records";
            try
            {
                string currentDate = (DateTime.Now).ToString("ddMMyyyy");//;
                string refSerial = string.Empty;
                string itemCode = string.Empty;
                string uom = string.Empty;
                string wh = string.Empty;
                string fileName = "Stock_" + currentDate + ".xml";
                string destinationPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["destinationPath"]) + fileName;
                if (File.Exists(destinationPath))
                {
                    var xmlString = File.ReadAllText(destinationPath);
                    var stringReader = new StringReader(xmlString);
                    var dsSet = new System.Data.DataSet();
                    dsSet.ReadXml(stringReader);

                    if (dsSet.Tables.Count > 0)
                    {
                        if (dsSet.Tables.Count > 2)
                        {
                            System.Data.DataRow[] dr = dsSet.Tables[2].Select("Code ='" + SerialCode + "'");
                            refSerial = Convert.ToString(dr[0][1]);
                            string openingBalanceStockId = Convert.ToString(dr[0][2]);
                            System.Data.DataRow[] dr1 = dsSet.Tables[1].Select("Opening_Balance_Stock_Id ='" + openingBalanceStockId + "'");
                            itemCode = Convert.ToString(dr1[0][1]);
                            uom = Convert.ToString(dr1[0][3]);
                            string warehouseId = Convert.ToString(dr1[0][8]);
                            System.Data.DataRow[] dr2 = dsSet.Tables[0].Select("Warehouse_Id ='" + warehouseId + "'");
                            wh = Convert.ToString(dr2[0][2]);
                        }
                    }
                    if (!string.IsNullOrEmpty(refSerial))
                    {
                        BLWFMTicket.updateCpeDetailFromXml(job_id, refSerial, itemCode, uom, wh);
                        message = "";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getcpeDetailFromXml", "FRTController", "jobid: " + job_id + ",Serialno: " + SerialCode, ex);
            }
            return message;
        }

        [Route("fetchCpeDetail")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> fetchCpeDetail(getCPEDetailIn data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            //bool IsSapEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["isSapEnabled"]);
            bool IsSapEnabled = false;
            List<GlobalSetting> globalSettings = new BLGlobalSetting().GetGlobalSettings("Mobile");
            var item = globalSettings.FirstOrDefault(o => o.key == "isSapEnabled");
            if (item != null)
            {
                IsSapEnabled = Convert.ToInt32(item.value) == 0 ? false : true;
            }
            try
            {
                if (string.IsNullOrEmpty(data.type))
                {
                    response.status = StatusCodes.FAILED.ToString();
                    response.error_message = "type can not be null or empty";
                    return response;
                }
                if (!IsSapEnabled)
                {
                    List<UserWarehouseCodeMapping> warehouseCode = null;
                    if (string.IsNullOrEmpty(data.serial_no))
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.error_message = "serial no can not be null or empty";
                        return response;
                    }

                    string currentDate = (DateTime.Today.AddDays(-2)).ToString("ddMMyyyy");//;     
                    string refSerial = string.Empty;
                    string itemCode = string.Empty;
                    string uom = string.Empty;
                    string wh = string.Empty;
                    string fileName = "Stock_" + currentDate + ".xml";
                    string mainpath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["getcpeDetailFromXml"]);
                    //string fileName = "Stock_30052021.xml";
                    string destinationPath = Path.Combine(mainpath, fileName);
                    if (!File.Exists(destinationPath))
                    {
                        destinationPath = Path.Combine(mainpath, "Stock.xml");
                    }

                    if (File.Exists(destinationPath))
                    {
                        var xmlString = File.ReadAllText(destinationPath);
                        var stringReader = new StringReader(xmlString);
                        var dsSet = new System.Data.DataSet();
                        dsSet.ReadXml(stringReader);

                        if (dsSet.Tables.Count > 2)
                        {
                            System.Data.DataRow[] dr = dsSet.Tables[2].Select("Code ='" + data.serial_no + "'");

                            if (dr.Length > 0)
                            {

                                refSerial = Convert.ToString(dr[0][1]);
                                string openingBalanceStockId = Convert.ToString(dr[0][2]);
                                System.Data.DataRow[] dr1 = dsSet.Tables[1].Select("Opening_Balance_Stock_Id ='" + openingBalanceStockId + "'");
                                itemCode = Convert.ToString(dr1[0][1]);
                                uom = Convert.ToString(dr1[0][3]);
                                string warehouseId = Convert.ToString(dr1[0][8]);
                                System.Data.DataRow[] dr2 = dsSet.Tables[0].Select("Warehouse_Id ='" + warehouseId + "'");
                                wh = Convert.ToString(dr2[0][2]);

                                if (data.type == "1")// for cpe information
                                {
                                    string checkwarehouseCode = System.Configuration.ConfigurationManager.AppSettings["checkWarehouseCodeonUserId"];
                                    if (data.user_id != 0 && checkwarehouseCode == "true")
                                    {
                                        warehouseCode = new BLUserWarehouseCodeMapping().CheckWarehouseCodeExistOrNot(data.user_id, wh);
                                        if (warehouseCode.Count > 0)
                                        {

                                            var res = new { cpe_ref_serial = refSerial, cpe_item_code = itemCode, cpe_uom = uom, cpe_wh = wh };
                                            response.results = res;
                                        }
                                        else
                                        {
                                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                                            response.error_message = "Warehouse code " + wh + " ! is not mapped with this user";
                                        }
                                    }
                                    else
                                    {
                                        var res = new { cpe_ref_serial = refSerial, cpe_item_code = itemCode, cpe_uom = uom, cpe_wh = wh };
                                        response.results = res;
                                    }
                                }
                                else if (data.type == "2")// for additional material
                                {
                                    var res = new { item_code = itemCode, uom = uom, wh_code = wh, quantity = "" };
                                    response.results = res;
                                }
                                response.status = StatusCodes.OK.ToString();
                            }
                            else
                            {
                                response.status = StatusCodes.INVALID_INPUTS.ToString();
                                response.error_message = "Serial number not found in stock file !!";
                            }
                        }
                        else
                        {
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.error_message = "Unable to fetch the stock records";
                        }

                    }
                    else
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.error_message = "File not found.";
                    }
                }
                else
                {
                    if (data.type == "1")
                    {
                        if (string.IsNullOrEmpty(data.serial_no))
                        {
                            response.status = StatusCodes.FAILED.ToString();
                            response.error_message = "serial no can not be null or empty";
                            return response;
                        }
                        dynamic dynaobj = CallgetInventoryBalance(data.serial_no, data.user_id);
                        if (dynaobj.results != null)
                        {
                            string refSerial = Convert.ToString(dynaobj.results[0].MAC_ADDRESS);
                            string itemCode = Convert.ToString(dynaobj.results[0].ITEM_CODE);
                            string uom = Convert.ToString(dynaobj.results[0].UOM);
                            string wh = Convert.ToString(dynaobj.results[0].STORAGE_LOCATION);

                            var res = new { cpe_ref_serial = refSerial, cpe_item_code = itemCode, cpe_uom = uom, cpe_wh = wh };
                            response.status = StatusCodes.OK.ToString();
                            response.results = res;
                        }
                        else
                        {
                            response.status = StatusCodes.ZERO_RESULTS.ToString();
                            response.error_message = Convert.ToString(dynaobj.error_message);
                        }
                    }
                    else if (data.type == "2")//additional material
                    {
                        if (data.material_id == 0)
                        {
                            response.status = StatusCodes.FAILED.ToString();
                            response.error_message = "material id can not be null or empty";
                            return response;
                        }
                        dynamic dynaobj = CallgetInventoryBalance(data.serial_no, data.user_id, data.material_id);
                        if (dynaobj.results != null)
                        {
                            string itemCode = Convert.ToString(dynaobj.results[0].ITEM_CODE);
                            string uom = Convert.ToString(dynaobj.results[0].UOM);
                            string wh = Convert.ToString(dynaobj.results[0].STORAGE_LOCATION);
                            decimal qty = Convert.ToDecimal(dynaobj.results[0].QUANTITY);
                            string batch = Convert.ToString(dynaobj.results[0].BATCH);
                            if (data.quantity <= qty)
                            {
                                response.status = StatusCodes.OK.ToString();
                                response.error_message = "Stock available";
                                var res = new { item_code = itemCode, uom = uom, wh_code = wh, quantity = Convert.ToString(data.quantity), batch_no = batch };
                                response.results = res;
                            }
                            else
                            {
                                response.status = StatusCodes.FAILED.ToString();
                                response.error_message = "Not enough quantity available. Available quantity :" + qty;
                            }
                        }
                        else
                        {
                            response.status = StatusCodes.ZERO_RESULTS.ToString();
                            response.error_message = Convert.ToString(dynaobj.error_message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("fetchCpeDetail", "FRTController", null, ex);
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                response.error_message = "Unable to fetch the stock records";
            }
            return response;
        }
        private WFMMobileApiResponse<dynamic> SendSerialnumberToERP(Task task)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            string inputData = "";
            string results = "";
            try
            {

                //   var task = BLWFMTicket.GetJobDetailByJobOrderId(ticketDetails.hpsmid);
                inputData = "Action: " + task.action + ", TranNo: " + task.hpsmid + ", WHCode: " + task.cpe_wh + ", ItemCode: " + task.cpe_item_code + ", Serial: " + task.cpe_serialno;
                string action = task.action == "" ? "Add" : task.action;
                ConvergeERPService.I_ERP_ServiceClient er = new ConvergeERPService.I_ERP_ServiceClient();
                string TranNo = task.hpsmid;  //Order id
                string WHCode = task.cpe_wh;  //CPE_WH
                string ItemCode = task.cpe_item_code;  // CPE_ITEM_CODE
                string Serial = task.cpe_serialno;  //cpe_serialno
                //string TranNo = "T1213126"; //Order id
                //string WHCode = "003829"; //CPE_WH
                //string ItemCode = "CPE00000188";// CPE_ITEM_CODE
                //string Serial = "48575443ECB04EA8";//cpe_serialno

                if ((action).ToUpper() == "ADD")
                {
                    ConvergeERPService.InvIssueDtl[] dtl = new ConvergeERPService.InvIssueDtl[1];
                    ConvergeERPService.InvIssueDtl dd = new ConvergeERPService.InvIssueDtl();
                    dd.ItemCode = ItemCode;
                    dd.Serial = Serial;
                    dd.UOM = "PC";
                    dd.Quantity = 1;
                    dtl[0] = dd;
                    results = er.CreateInvIssue(TranNo, " ", " ", " ", WHCode, dtl);
                }
                else if ((action).ToUpper() == "REMOVE")
                {
                    ConvergeERPService.InvReturn[] dtlreturn = new ConvergeERPService.InvReturn[1];
                    ConvergeERPService.InvReturn dd1 = new ConvergeERPService.InvReturn();
                    dd1.ItemCode = ItemCode;
                    dd1.Serial = Serial;
                    dd1.UOM = "PC";
                    dd1.Quantity = 1;
                    dtlreturn[0] = dd1;
                    results = er.CreateInvReturn(TranNo, WHCode, dtlreturn);

                }
                response.results = results;
            }
            catch (Exception ex)
            {
                response.error_message = Convert.ToString(ex.Message);
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SendSerialnumberToERP()", "FEController", inputData, ex);

            }
            return response;
        }
        private WFMMobileApiResponse<dynamic> SendaddMaterialToERP(List<AdditionalMaterial> additionalMaterialsDetail)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            string inputData = "";
            string results = "";
            try
            {
                ConvergeERPService.I_ERP_ServiceClient er = new ConvergeERPService.I_ERP_ServiceClient();
                List<AdditionalMaterial> additionalMaterials = new List<AdditionalMaterial>();
                additionalMaterials = additionalMaterialsDetail.Where(item => item.wh_code != null).ToList();

                if (additionalMaterials.Count > 0)
                {
                    string jobId = additionalMaterials[0].jobid;
                    string whCode = additionalMaterials[0].wh_code;
                    ConvergeERPService.InvIssueDtl[] dtl = new ConvergeERPService.InvIssueDtl[additionalMaterials.Count];
                    ConvergeERPService.InvIssueDtl dd = new ConvergeERPService.InvIssueDtl();
                    int count = 0;
                    foreach (var item in additionalMaterials)
                    {

                        dd.ItemCode = item.item_code;
                        dd.Serial = item.serial_no;
                        dd.UOM = item.uom;
                        dd.Quantity = Convert.ToInt32(item.details);
                        dtl[count] = dd;
                        count++;
                    }
                    results = er.CreateInvIssue(jobId, " ", " ", " ", whCode, dtl);



                }
                else
                {
                    response.results = "Additional Material not found for :- " + additionalMaterials[0].jobid;
                }




                response.results = results;
            }
            catch (Exception ex)
            {
                response.error_message = Convert.ToString(ex.Message);
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SendaddMaterialToERP()", "FEController", inputData, ex);

            }
            return response;
        }

        [Route("sendtoerp")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> sendtoerp(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            Models.WFM.getDetailIn objIn = ReqHelper.GetRequestData<Models.WFM.getDetailIn>(data);
            try
            {
                //bool IsSapEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["isSapEnabled"]);
                bool IsSapEnabled = false;
                List<GlobalSetting> globalSettings = new BLGlobalSetting().GetGlobalSettings("Mobile");
                var item = globalSettings.FirstOrDefault(o => o.key == "isSapEnabled");
                if (item != null)
                {
                    IsSapEnabled = Convert.ToInt32(item.value) == 0 ? false : true;
                }
                string msg = "";
                bool result = true;
                var result1 = new WFMMobileApiResponse<dynamic>();
                var additionMaterialResult = new WFMMobileApiResponse<dynamic>();
                var lstobj = BLWFMTicket.GetAdditionalMaterial(objIn.job_id);
                Task ticketDetail = new Task();
                ticketDetail.hpsmid = objIn.job_id;
                var task = BLWFMTicket.GetJobDetailByJobOrderId(objIn.job_id);
                bool isValidateERP = Convert.ToBoolean(ConfigurationManager.AppSettings["isvalidateerp"]);
                if (!IsSapEnabled)
                {

                    if (task.ticket_source_id == 4)  //Trouble ticket
                    {
                        if (task.cpe_serialno != task.current_cpesn)//replace cpe case
                        {
                            result1 = SendSerialnumberRemoveToERP(task);//remove old cpe
                            result1 = SendSerialnumberToERP(task);//add
                        }
                        additionMaterialResult = SendaddMaterialToERP(lstobj);
                    }
                    else
                    {
                        result1 = SendSerialnumberToERP(task); //EendSerialnUmberToErp
                        additionMaterialResult = SendaddMaterialToERP(lstobj); //SendaddMaterialToErp
                    }
                    if (isValidateERP)
                    {
                        if (string.IsNullOrEmpty(result1.results))
                        {
                            msg = result1.error_message;
                            result = false;
                            ticketDetail.sendtoerp = false;
                            ticketDetail.erp_response = result1.error_message;
                            BLWFMTicket.UpdateSendToERP(ticketDetail);
                        }
                        else
                        {
                            msg = "Sent to ERP successfully";
                            ticketDetail.sendtoerp = true;
                            ticketDetail.erp_response = "Success";
                            BLWFMTicket.UpdateSendToERP(ticketDetail);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(result1.results))
                        {
                            msg = "Sent to ERP successfully";
                            ticketDetail.sendtoerp = false;
                            ticketDetail.erp_response = result1.error_message;
                            BLWFMTicket.UpdateSendToERP(ticketDetail);
                        }
                        else
                        {
                            msg = "Sent to ERP successfully";
                            ticketDetail.sendtoerp = true;
                            ticketDetail.erp_response = "Success";
                            BLWFMTicket.UpdateSendToERP(ticketDetail);
                        }
                    }

                }
                else
                {
                    msg = "Sent to SAP successfully";
                    if (task.ticket_source_id == 4) //trouble ticket
                    {
                        if (task.cpe_serialno != task.current_cpesn)
                        {
                            result1 = SendSerialnumberRemoveToSAP(task);
                            result1 = SendSerialnumberToSAP(task);
                        }
                        if (lstobj.Count > 0)
                        {
                            result1 = SendaddMaterialToSAP(task, lstobj);
                        }
                    }
                    else //Order fullfillment
                    {
                        result1 = SendSerialnumberToSAP(task); //add and remove both case handled
                        if (result1.status == "success")
                        {
                            msg = "CPE Sent to SAP successfully. ";
                            if (lstobj.Count > 0)
                            {
                                result1 = SendaddMaterialToSAP(task, lstobj);
                                if (result1.status == "success")
                                {
                                    result = true;
                                    msg = "Sent to SAP successfully!!";

                                }
                                else
                                {
                                    if (!isValidateERP)
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        result = false;
                                    }
                                    msg += " but unable to send additional material |" + result1.error_message;
                                }
                            }
                        }
                        else
                        {
                            if (!isValidateERP)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            msg = "Unable to send data |" + result1.error_message;
                        }
                    }


                    if (result1.status != "success")
                    {
                        ticketDetail.sendtoerp = false;
                        ticketDetail.erp_response = result1.error_message;
                        //msg = result1.error_message;
                    }
                    else
                    {
                        ticketDetail.sendtoerp = true;
                        ticketDetail.erp_response = "Success";
                    }
                    BLWFMTicket.UpdateSendToERP(ticketDetail);
                }

                if (result)
                {
                    int stage = BLWFMTicket.UpdateJobOrderStage(objIn.job_id, objIn.step_order);
                    response.error_message = msg;
                    response.status = StatusCodes.OK.ToString();
                    return response;
                }
                else
                {
                    response.status = StatusCodes.FAILED.ToString();
                    response.error_message = msg;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("sendtoerp()", "FEController", Convert.ToString(data), ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [Route("entityconfiguration")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> entityconfiguration(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            job_order_status jobOrderStatus = new job_order_status();
            string commanName = "";
            try
            {
                Models.WFM.entityconfigurationIn objIn = ReqHelper.GetRequestData<Models.WFM.entityconfigurationIn>(data);
                jobOrderStatus.job_id = objIn.jobid;
                if (objIn.entity_type == "ONT")
                {
                    commanName = "ONT";
                    jobOrderStatus.action = "cpe_activate";
                    response.error_message = "CPE activated!!";
                }
                else if (objIn.entity_type == "WIFI")
                {
                    commanName = "WIFI";
                    jobOrderStatus.action = "wifi_activate";
                    response.error_message = "WIFI activated!!";
                }
                //Call hobs api
                var apiResponse = getTriggerActivateDetailAxtel(objIn.jobid, commanName, objIn.serial_no);
                if (apiResponse.results == true)
                {
                    //ont activated
                    jobOrderStatus.cpe_serialno = objIn.serial_no;
                    int statusRes = BLWFMTicket.UpdateStatus(jobOrderStatus);
                    if (statusRes > 0)
                    {
                        response.status = StatusCodes.OK.ToString();

                        if (!string.IsNullOrEmpty(commanName))
                        {
                            var result = new BLMisc().UpdateEntityBarCode(0, commanName, objIn.serial_no, objIn.entity_id);
                            if (result != null && result.status)
                            {
                                response.error_message = result.message;
                                response.status = StatusCodes.OK.ToString();
                            }
                            else
                            {
                                response.status = StatusCodes.VALIDATION_FAILED.ToString();
                                response.error_message = result.message;
                            }
                        }
                        else
                        {
                            response.status = StatusCodes.VALIDATION_FAILED.ToString();
                            response.error_message = "Invalid entity type !!";
                        }
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }
                }
                else
                {
                    response.status = apiResponse.status;
                    response.error_message = apiResponse.error_message;
                    return response;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var request = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                logHelper.ApiLogWriter("entityconfiguration()", "FEController", request, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        public WFMMobileApiResponse<dynamic> getTriggerActivateDetailAxtel(string job_id, string cpeType, string serial_no)
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                var TriggerActivateDetail = BLWFMTicket.GetHPSMTicket_Details(job_id);
                string URL = Convert.ToString(ConfigurationManager.AppSettings["TriggerActivationServicePath"]);
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                if (TriggerActivateDetail != null)
                {
                    var requestData = new
                    {
                        orderId = TriggerActivateDetail.hpsmid,
                        portNumber = TriggerActivateDetail.cpe_portno,
                        serialNo = serial_no,
                        cpeModel = TriggerActivateDetail.cpe_model,
                        cpeMake = TriggerActivateDetail.cpe_brand,
                        macId = TriggerActivateDetail.cpe_mac_address,
                        cpeType = cpeType

                    };
                    //var requestData = new
                    //{
                    //    orderId = "O10222026",
                    //    portNumber = "123",
                    //    serialNo = "ZTEGC4A84955",
                    //    cpeModel = "DPC2100R3",
                    //    cpeMake = "D2_AIR1000_10MB_NEW",
                    //    macId = "12:45:AF:34:ED:92",
                    //    cpeType = "ONT"

                    //};
                    string Attributes = "gponAttributes";
                    var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

                    var DATA = "{ " + "\"" + Attributes + "\"" + ":";
                    DATA = DATA + responeData;
                    DATA = DATA + "}";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    request.ContentType = "application/json";

                    WebHeaderCollection aPIHeaderValues = new WebHeaderCollection();
                    aPIHeaderValues.Add("opId", "HOB");
                    aPIHeaderValues.Add("buId", "DEFAULT");
                    request.Headers.Add(aPIHeaderValues);

                    // request.Headers.Add("Authorization", "Bearer " + tokenResponse.results);
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

                            var dyn = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivateCpeResAxtel>(responses);
                            if (dyn.STATUS.statusCode != "0000")
                            {
                                response.results = false;
                                response.status = StatusCodes.INVALID_INPUTS.ToString();
                                response.error_message = "CPE not activated :" + " Status code: " + dyn.STATUS.statusCode;
                                logHelper.ApiLogWriter("getTriggerActivateDetailAxtel()", "FEController", ("Request :" + DATA + "Response :" + responses), null);
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
                        logHelper.ApiLogWriter("inner catch:getTriggerActivateDetailAxtel()", "FEController", DATA, ex);
                        response.results = false;
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.error_message = "Error While Processing  Request.";
                    }
                }
                else
                {
                    response.error_message = "Job order id not found";
                    response.results = false;
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                }

            }
            catch (Exception ex)
            {
                logHelper.ApiLogWriter("outer catch:getTriggerActivateDetailAxtel()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.results = false;
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        [Route("reschedule")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> reschedule(reschedule reschedule)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    job_order_status obj = new job_order_status()
                    {
                        job_id = reschedule.jobid,
                        action = "Re-Scheduled",
                        date = reschedule.rescheduledate
                    };

                    int res = BLWFMTicket.UpdateStatus(obj);

                    if (res > 0)
                    {
                        response.error_message = "Job " + obj.action + " successfully";
                        response.status = StatusCodes.OK.ToString();
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("reschedule()", "FE", ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }

        [Route("getstatusdetailbyjoborderid")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> GetStatusDetailByJobOrderId(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    Models.WFM.getDetailIn objIn = ReqHelper.GetRequestData<Models.WFM.getDetailIn>(data);
                    getStatusDetail getStatusDetail = BLWFMTicket.GetStatusDetailByJobOrderId(objIn);
                    if (getStatusDetail != null)
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.results = getStatusDetail;
                        response.error_message = getStatusDetail.remarks;

                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("GetStatusDetailByJobOrderId()", "FEController", Convert.ToString(data), ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }

        [Route("UploadEntityAttachment")]
        [HttpPost]
        public ApiResponse<string> UploadEntityAttachment()
        {

            var response = new ApiResponse<string>();
            try
            {
                HttpPostedFile files = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
                if (HttpContext.Current.Request.Files.Count > 0 && files != null)
                {

                    var systemId = HttpContext.Current.Request.Params["entitySystemId"];
                    var entityType = HttpContext.Current.Request.Params["entityType"];
                    var featureName = "";
                    var attachmentType = HttpContext.Current.Request.Params["uploadType"];
                    var UserId = HttpContext.Current.Request.Params["userId"];
                    var validDocumentTypes = ApplicationSettings.validDocumentTypes.Split(new string[] { "," }, StringSplitOptions.None);
                    var validImageTypes = ApplicationSettings.validImageTypes.Split(new string[] { "," }, StringSplitOptions.None);
                    var step_order = Convert.ToInt32(HttpContext.Current.Request.Params["step_order"]);
                    var job_id = HttpContext.Current.Request.Params["job_id"];
                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {

                        string FileName = files.FileName;
                        var fileExtension = Path.GetExtension(FileName);

                        if ((ApplicationSettings.MaxuploadFileSize < files.ContentLength / 1024 / 1024) && (attachmentType.ToUpper() == "DOCUMENT"))
                        {
                            response.error_message = String.Format(Resources.Resources.SI_OSP_GBL_JQ_FRM_109, ApplicationSettings.MaxuploadFileSize);
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }
                        if ((ApplicationSettings.MaxuploadFileSize < files.ContentLength / 1024 / 1024) && (attachmentType.ToUpper() == "IMAGE"))
                        {
                            response.error_message = String.Format(Resources.Resources.SI_OSP_GBL_JQ_GBL_112, ApplicationSettings.MaxuploadFileSize);
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }

                        if ((attachmentType != null && attachmentType.ToUpper() == "DOCUMENT") && !validDocumentTypes.Contains(fileExtension.ToLower()))
                        {
                            response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_059 + " extension:" + ApplicationSettings.validDocumentTypes;
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }
                        if ((attachmentType != null && attachmentType.ToUpper() == "IMAGE") && !validImageTypes.Contains(fileExtension.ToLower()))
                        {
                            response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_059 + " extension:" + ApplicationSettings.validImageTypes;
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }
                        var lstDocument = new BLAttachment().getAttachmentDetailsbyId(Convert.ToInt32(systemId), entityType, attachmentType, FileName, Convert.ToInt32(UserId), "");
                        if (lstDocument.Count > 0)
                        {
                            response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_055;
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }

                        string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        string strFilePath = "";
                        if (entityType == EntityType.ROW.ToString() && !string.IsNullOrEmpty(featureName))
                        {
                            attachmentType = "";
                            strFilePath = ReqHelper.UploadfileOnFTP(featureName, systemId, files, attachmentType, strNewfilename, entityType);
                        }
                        else if (!string.IsNullOrEmpty(featureName))
                        {
                            strFilePath = ReqHelper.UploadfileOnFTP(entityType, systemId, files, attachmentType, strNewfilename, featureName);
                        }
                        else
                        {
                            strFilePath = ReqHelper.UploadfileOnFTP(entityType, systemId, files, attachmentType, strNewfilename);
                        }
                        LibraryAttachment objAttachment = new LibraryAttachment();
                        objAttachment.entity_system_id = Convert.ToInt32(systemId);
                        objAttachment.entity_type = entityType;
                        objAttachment.org_file_name = FileName;
                        objAttachment.file_name = strNewfilename;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_location = strFilePath;
                        objAttachment.upload_type = attachmentType;
                        objAttachment.uploaded_by = UserId;
                        objAttachment.entity_feature_name = featureName;
                        objAttachment.file_size = files.ContentLength;
                        objAttachment.uploaded_on = DateTime.Now;

                        //Save Image on FTP and related detail in database..
                        var savefile = new BLAttachment().SaveLibraryAttachment(objAttachment);
                    }

                    BLWFMTicket.UpdateJobOrderStage(job_id, step_order, null);

                    response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_154;
                    response.status = StatusCodes.OK.ToString();
                    return response;
                }
                else
                {
                    response.error_message = "No attachment selected.";
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    return response;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("UploadEntityAttachment()", "FE", ex);
                response.error_message = "Error in uploading attachment!";
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                return response;
                //Error Logging...
            }
        }


        //09-06-2022

        [Route("RequestConnectedDevice")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> RequestConnectedDevice(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    var GetResponse = new WFMMobileApiResponse<dynamic>();
                    Models.WFM.ConnectedDeviceDetail objIn = ReqHelper.GetRequestData<Models.WFM.ConnectedDeviceDetail>(data);
                    GetResponse = GetEthernetORWIFIConnectedDevice(objIn);


                    if (GetResponse.status == "OK")
                    {
                        response.status = StatusCodes.OK.ToString();
                        //  response.error_message = objIn.devicetype +" request connect device completed";
                        response.results = GetResponse.results;

                    }
                    else
                    {
                        response.status = GetResponse.status;
                        response.error_message = GetResponse.error_message;
                        response.results = false;
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("RequestConnectedDevice()", "FEController", Convert.ToString(data), ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }

        public WFMMobileApiResponse<dynamic> GetEthernetORWIFIConnectedDevice(ConnectedDeviceDetail connectedDeviceDetail)
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                var tokenresponse = new WFMMobileApiResponse<dynamic>();
                tokenresponse = getAccessConnectedDeviceToken();
                if (tokenresponse.results != "")
                {
                    string DATA = "";
                    string URL = "";
                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                    System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    if (connectedDeviceDetail.devicetype == "ONT")
                    {
                        URL = Convert.ToString(ConfigurationManager.AppSettings["axtelEthernetPath"]) + connectedDeviceDetail.serialno;// Convert.ToString(ConfigurationManager.AppSettings["axtelEthernetPath"]);
                    }
                    else if (connectedDeviceDetail.devicetype == "WIFI")
                    {
                        URL = Convert.ToString(ConfigurationManager.AppSettings["axtelWIFIPath"]) + connectedDeviceDetail.serialno;// Convert.ToString(ConfigurationManager.AppSettings["axtelWIFIPath"]);
                    }
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Headers.Add("Authorization", "Basic " + tokenresponse.results);
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
                            if (jsonResponse.status == "Submitted")
                            {
                                ConnectedDeviceRequest ConnectedDeviceRequest = new ConnectedDeviceRequest();
                                ConnectedDeviceRequest.jobid = connectedDeviceDetail.jobid;
                                ConnectedDeviceRequest.serialno = connectedDeviceDetail.serialno;
                                ConnectedDeviceRequest.devicetype = connectedDeviceDetail.devicetype;
                                ConnectedDeviceRequest.requestid = jsonResponse.requestID;
                                ConnectedDeviceRequest.status = jsonResponse.status;
                                ConnectedDeviceRequest.message = jsonResponse.message;
                                BLWFMTicket.saveConnectedDeviceRequest(ConnectedDeviceRequest);

                                if (jsonResponse.requestID > 0)
                                {
                                    response.status = StatusCodes.OK.ToString();
                                    response.results = ConnectedDeviceRequest;
                                }

                                else
                                {
                                    response.results = false;
                                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                                    response.error_message = "Connected device request not submitted !!";
                                    logHelper.ApiLogWriter("catch:GetEthernetORWIFIConnectedDevice()", "FEController", URL, null);
                                }

                            }

                            else
                            {
                                response.results = false;
                                response.status = StatusCodes.INVALID_INPUTS.ToString();
                                response.error_message = "Connected device request not submitted !!";
                                logHelper.ApiLogWriter("catch:GetEthernetORWIFIConnectedDevice()", "FEController", URL, null);


                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logHelper.ApiLogWriter("catch:GetEthernetORWIFIConnectedDevice()", "FEController", DATA, ex);
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.results = false;
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
                logHelper.ApiLogWriter("outer:GetEthernetConnectedDevices()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.results = false;
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        public WFMMobileApiResponse<dynamic> getAccessConnectedDeviceToken()
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var objresponse = new WFMMobileApiResponse<dynamic>();
            string DATA = "";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                string URL = Convert.ToString(ConfigurationManager.AppSettings["axtelGetTokenPath"]);
                string clientId = Convert.ToString(ConfigurationManager.AppSettings["axtelClientId"]);
                string clientSecret = Convert.ToString(ConfigurationManager.AppSettings["axtelClientSecret"]);
                DATA = "username=" + clientId + "&password=" + clientSecret + "&grant_type=password";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string username = Convert.ToString(ConfigurationManager.AppSettings["axtelUserName"]);
                string password = Convert.ToString(ConfigurationManager.AppSettings["axtelPassword"]);
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + svcCredentials);
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
                        if (jsonResponse.access_token != "")
                        {
                            objresponse.status = StatusCodes.OK.ToString();
                            objresponse.results = jsonResponse.access_token;
                        }
                        else
                        {
                            logHelper.ApiLogWriter("getAccessConnectedDeviceToken()", "FEController", DATA, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logHelper.ApiLogWriter("catch:getAccessConnectedDeviceToken()", "FEController", DATA, ex);
                    objresponse.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    objresponse.error_message = "Error While Processing  Request.";
                }
            }
            catch (Exception ex)
            {
                logHelper.ApiLogWriter("outercatch:getAccessConnectedDeviceToken()", "FEController", DATA, ex);
                objresponse.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objresponse.error_message = "Error While Processing  Request.";
            }
            return objresponse;
        }

        [Route("getconnecteddevice")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> GetConnectedDevice(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    ConnectedDeviceDetail objIn = ReqHelper.GetRequestData<ConnectedDeviceDetail>(data);
                    List<ConnectedDevice> connectedDeviceDetail = BLWFMTicket.GetConnectedDevice(objIn);
                    if (connectedDeviceDetail.Count > 0)
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.results = connectedDeviceDetail;
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("GetStatusDetailByJobOrderId()", "FEController", Convert.ToString(data), ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }


        [Route("getrcadetail")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> GetRcadetail(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            if (ModelState.IsValid)
            {
                try
                {
                    rcaIn objIn = ReqHelper.GetRequestData<rcaIn>(data);
                    List<WfmRca> GetRcadetail = BLWFMTicket.GetRcadetail(objIn);
                    if (GetRcadetail.Count > 0)
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.results = GetRcadetail;
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "No record found.";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("GetRcadetail()", "FEController", Convert.ToString(data), ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }


        [Route("searchserialnumber")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> searchserialnumber(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                //bool IsSapEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["isSapEnabled"]);
                bool IsSapEnabled = false;
                List<GlobalSetting> globalSettings = new BLGlobalSetting().GetGlobalSettings("Mobile");
                var item = globalSettings.FirstOrDefault(o => o.key == "isSapEnabled");
                if (item != null)
                {
                    IsSapEnabled = Convert.ToInt32(item.value) == 0 ? false : true;
                }
                Models.WFM.getCPEDetailIn objIn = ReqHelper.GetRequestData<Models.WFM.getCPEDetailIn>(data);

                if (string.IsNullOrEmpty(objIn.serial_no))
                {
                    //response.status = statuscodes.failed.tostring();
                    response.error_message = "serial no can not be null or empty";
                    return response;
                }

                //if (string.IsNullOrEmpty(data.type))
                //{
                //    response.status = StatusCodes.FAILED.ToString();
                //    response.error_message = "type can not be null or empty";
                //    return response;
                //}
                List<CPEDetail> CPEDetail = new List<CPEDetail>();
                if (!IsSapEnabled)
                {
                    string currentDate = (DateTime.Today.AddDays(-2)).ToString("ddMMyyyy");//;     
                    string refserial = string.Empty;
                    //string itemCode = string.Empty;
                    //string uom = string.Empty;
                    // string wh = string.Empty;
                    string fileName = "Stock_" + currentDate + ".xml";
                    string mainpath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["getcpeDetailFromXml"]);
                    //string fileName = "Stock_30052021.xml";
                    string destinationPath = Path.Combine(mainpath, fileName);
                    if (!File.Exists(destinationPath))
                    {
                        destinationPath = Path.Combine(mainpath, "Stock.xml");
                    }
                    if (File.Exists(destinationPath))
                    {
                        var xmlString = File.ReadAllText(destinationPath);
                        var stringReader = new StringReader(xmlString);
                        var dsSet = new System.Data.DataSet();
                        dsSet.ReadXml(stringReader);
                        if (dsSet.Tables.Count > 2)
                        {
                            System.Data.DataRow[] dr = dsSet.Tables[2].Select("Code LIKE '%" + objIn.serial_no + "'");
                            int count = dr.Count();
                            //Select("CREATOR LIKE '%" + searchstring + "%'");
                            for (int i = 0; i < dr.Count(); i++)
                            {
                                CPEDetail sPEDetail = new CPEDetail();
                                sPEDetail.serial_code = Convert.ToString(dr[i].ItemArray[0]);
                                CPEDetail.Add(sPEDetail);
                            }
                            response.status = StatusCodes.OK.ToString();
                            response.results = CPEDetail;
                        }
                        else
                        {
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.error_message = "File not found.";
                        }
                    }
                }
                else
                {
                    dynamic dynaobj = CallgetInventoryBalance(objIn.serial_no, objIn.user_id);
                    if (dynaobj.results != null)
                    {
                        for (int i = 0; i < dynaobj.results.Count; i++)
                        {
                            CPEDetail sPEDetail = new CPEDetail();
                            sPEDetail.serial_code = Convert.ToString(dynaobj.results[i].SERIAL_NUMBER);
                            CPEDetail.Add(sPEDetail);
                        }
                        response.status = StatusCodes.OK.ToString();
                        response.results = CPEDetail;
                    }
                    else
                    {
                        response.status = StatusCodes.ZERO_RESULTS.ToString();
                        response.error_message = Convert.ToString(dynaobj.error_message);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("fetchCpeDetail", "FRTController", null, ex);
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                response.error_message = "Unable to fetch the stock records";
            }
            return response;
        }




        [Route("getttrcrca")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getttrcrca()
        {
            var response = new WFMMobileApiResponse<dynamic>();

            try
            {
                var lstobj = BLWFMTicket.GetTTRcRca();
                response.results = lstobj;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getttrcrca()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }


        [Route("updatettstatus")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> updatettstatus(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            Models.WFM.getttStatusIn objgetttStatusIn = ReqHelper.GetRequestData<Models.WFM.getttStatusIn>(data);
            try
            {
                int res = BLWFMTicket.UpdatettStatus(objgetttStatusIn);
                response.results = null;
                response.status = StatusCodes.OK.ToString();
                response.error_message = "Status updated";
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("updatettstatus()", "FEController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        [Route("getttrcrcabyjobid")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getttrcrcabyjobid(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            Models.WFM.getDetailIn objgetttStatusIn = ReqHelper.GetRequestData<Models.WFM.getDetailIn>(data);
            try
            {
                var lstobj = BLWFMTicket.GetJobDetailByJobOrderId(objgetttStatusIn.job_id);

                if (lstobj != null)
                {
                    response.results = new { rca = lstobj.resolution_close_id, rc = lstobj.root_cause_id };
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
                else
                {
                    response.results = null;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "No record found.";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getttrcrcabyjobid()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }


        [HttpPost]
        [Route("getRescheduleSlots")]
        public WFMApiResponse<dynamic> getRescheduleSlots(Models.WFM.ReqInput data)
        {
            var response = new WFMApiResponse<dynamic>();
            JobRescheduleIn objIn = ReqHelper.GetRequestData<JobRescheduleIn>(data);

            if (objIn.date < DateTime.Now.Date)
            {
                response.status = StatusCodes.FAILED.ToString();
                response.message = "Date is less than current date.";
                return response;
            }


            var lstobj = BLWFMTicket.GetJobDetailByJobOrderId(objIn.job_id);

            string joType = lstobj.tasktype;

            SlotConfirmation Request = BLWFMTicket.GetSlotConfirmation(lstobj.bookingid);
            if (Request == null)
            {
                var message = string.Format("No record found in confirm slot");
                response.status = StatusCodes.OK.ToString();
                response.message = message;
                return response;
            }
            SlotRequest SlotRequest = BLWFMTicket.GetSlotRequest(Request.referenceid);
            SlotRequest.managerid = Request.managerid;
            if (SlotRequest == null)
            {
                var message = string.Format("No record found in slot request");
                response.status = StatusCodes.OK.ToString();
                response.message = message;
                return response;
            }

            var JoRoleMapping = BLWFMTicket.GetRoleNameByJoCategory(SlotRequest.jo_category, joType);
            if (JoRoleMapping == null)
            {
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                response.message = string.Format("Role not found on selected Jo category : {0}", SlotRequest.jo_category);
                return response;
            }
            var roleName = JoRoleMapping.role_name;
            string fe_type = JoRoleMapping.workflow == "Regular process" ? "regular" : "fastlane";

            var service_types = SlotRequest.service_type;
            int slot_duration = 0;
            if (service_types.Contains(","))
            {
                string[] services = service_types.Split(',');
                foreach (var x in services)
                {
                    var service_facility = BLWFMTicket.GetServiceFacility(x);
                    if (service_facility == null)
                    {
                        var message = string.Format("Service facility {0} not found", x);
                        response.results = null;
                        response.status = StatusCodes.OK.ToString();
                        response.message = message;
                        return response;
                    }
                    slot_duration += service_facility.slot_duration;
                }
            }
            else
            {
                var service_facility = BLWFMTicket.GetServiceFacility(service_types);
                if (service_facility == null)
                {
                    var message = string.Format("Service facility {0} not found", service_types);
                    response.results = null;
                    response.status = StatusCodes.OK.ToString();
                    response.message = message;
                    return response;
                }
                slot_duration = service_facility.slot_duration;
            }


            if (slot_duration == 0)
            {
                var message = string.Format("Slot duration not map with service facility.");
                response.status = StatusCodes.OK.ToString();
                response.message = message;
                return response;
            }
            var slotDuration = BLWFMTicket.GetSlotDurationDetails(slot_duration);

            if (slotDuration == null)
            {
                var message = string.Format("Slot duration {0} not found", slot_duration);
                response.status = StatusCodes.OK.ToString();
                response.message = message;
                return response;
            }
            int slotId = slotDuration.sdid;

            string service = SlotRequest.service_type;

            if (service.Contains(","))
            {
                service = service.Split(',').First();
            }
            var AVFEList = new List<FEList>();
            var FEList = BLWFMTicket.GetFEList(SlotRequest.managerid).Where(f => f.user_type == fe_type).ToList();
            if (FEList.Count == 0)
            {
                var message = string.Format("No FE map with contractor");
                response.results = null;
                response.status = StatusCodes.OK.ToString();
                response.message = message;
                return response;

            }
            var _isUserjotype = FEList.Where(j => j.user_Jo_type == joType).ToList();
            if (FEList.Count == 0)
            {
                var message = string.Format("FE user found but not map with jo type");
                response.status = StatusCodes.OK.ToString();
                response.message = message;
                return response;

            }
            var _isUserService = _isUserjotype.Where(j => j.user_service == service).ToList();
            if (_isUserService.Count == 0)
            {
                var message = string.Format("FE user found but not map with service");
                response.status = StatusCodes.OK.ToString();
                response.message = message;
                return response;
            }
            AVFEList = _isUserService;

            #region Check roster

            DateTime dtCurDate = DateTime.Now;
            int intDayOfWeek = ((int)dtCurDate.DayOfWeek) + 1;
            AVFEList.ForEach(r =>
            {
                string strTime = dtCurDate.TimeOfDay.Hours.ToString("0#") + ":" + dtCurDate.TimeOfDay.Minutes.ToString("0#");
                var isTimeSheet = BLWFMTicket.GetUserTimeSheet(r.UserId ?? 0, objIn.date, intDayOfWeek, strTime);

                if (isTimeSheet != null)
                {
                    r.isRosterAvailable = true;
                    r._start_time = isTimeSheet._start_time;
                    r._end_time = isTimeSheet._end_time;
                }
                else
                {
                    r.isRosterAvailable = false;
                }
            });

            if (AVFEList.Count > 0 && !AVFEList.Any(a => a.isRosterAvailable == true))
            {
                response.status = StatusCodes.FAILED.ToString();
                response.message = AVFEList.Count + " FEs found but no roster available";
                return response;
            }
            var AvailableEF = AVFEList.Where(f => f.isRosterAvailable == true).ToList();

            #endregion

            int rosterStartTime = AvailableEF.Min(a => a._start_time);
            int rosterEndTime = AvailableEF.Max(a => a._end_time);

            var SlotList = BLWFMTicket.GetSlot(slotId, rosterStartTime, rosterEndTime, objIn.date, SlotRequest.referenceid, AvailableEF.Count, AvailableEF, SlotRequest.managerid, "");


            var routeIssues = BLWFMTicket.GetRoute_Issue(lstobj.hpsm_ticket_id);
            if (routeIssues == null)
            {
                response.status = StatusCodes.OK.ToString();
                response.message = "not record found in job order assignment";
                return response;
            }
            var lstAssignedTaskDetail = BLWFMTicket.GetFRTTaskDetails(routeIssues.user_id, "", "", "All", "");
            var assignTicket = lstAssignedTaskDetail.Where(x => x.customer_appointment_date == objIn.date.ToString("dd-MMM-yyyy").ToUpper()).ToList();
            List<AssignedTaskMaster> getMasterIds = new List<AssignedTaskMaster>();
            if (assignTicket.Count > 0)
            {
                //var res = assignTicket.Any(w => w.master_slot_id == Request.master_slot_id);
                string APDate = objIn.date.ToString("yyyyMMdd");
                assignTicket.ForEach(item =>
                {
                    getMasterIds.Add
                    (
                       new AssignedTaskMaster()
                       {
                           slotid = (APDate + item.master_slot_id)
                       });
                });
            }
            if (SlotList.Count == 0)
            {
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                response.message = "No slot available";
                return response;
            }
            var result = SlotList.Where(p => !getMasterIds.Any(p2 => p2.slotid == p.slotid));
            var FilterSlot = result.Where(x => x.slotid != Request.slotid).ToList();
            List<SlotResponse> slotResponse = new List<SlotResponse>();
            // change from and to time format
            foreach (var item in FilterSlot)
            {
                SlotResponse slotResponseDetail = new SlotResponse();
                TimeSpan fromspan = TimeSpan.FromHours(Convert.ToInt32((item.from_time).Substring(0, (item.from_time).Length - 2)));
                DateTime fromTime = DateTime.Today + fromspan;
                String from_time = fromTime.ToString("hh:mm tt");
                slotResponseDetail.from_time = from_time;
                TimeSpan tospan = TimeSpan.FromHours(Convert.ToInt32((item.to_time).Substring(0, (item.to_time).Length - 2)));
                DateTime toTime = DateTime.Today + tospan;
                String to_time = toTime.ToString("hh:mm tt");
                slotResponseDetail.to_time = to_time;
                slotResponseDetail.appointment_date = item.appointment_date;
                slotResponseDetail.slotid = item.slotid;
                slotResponseDetail.referenceId = item.referenceId;
                slotResponse.Add(slotResponseDetail);
            }
            //var FilterSlot = SlotList.Where(x => x.slotid != Request.slotid).ToList();
            response.status = StatusCodes.OK.ToString();
            response.results = slotResponse;
            return response;
        }

        [HttpPost]
        [Route("sendBackTicket")]
        public WFMMobileApiResponse<dynamic> sendBackTicket(Models.WFM.ReqInput data)
        {
            var objresponse = new WFMMobileApiResponse<dynamic>();
            ErrorLogHelper logHelper = new ErrorLogHelper();
            try
            {
                ReqSendBackTicket REQ = ReqHelper.GetRequestData<ReqSendBackTicket>(data);


                getttStatusIn objstatusIN = new getttStatusIn();
                objstatusIN.job_id = REQ.job_id;
                objstatusIN.main_issue_type = REQ.main_issue_type;
                int res = BLWFMTicket.UpdatettStatus(objstatusIN);

                if (res > -1)
                {
                    job_order_status objstatus = new job_order_status();
                    objstatus.job_id = REQ.job_id;
                    objstatus.action = "Completed";
                    objstatus.service_status = "Send back to source";
                    objstatus.rca = REQ.rca;
                    objstatus.remarks = REQ.remarks;
                    BLWFMTicket.UpdateStatus(objstatus);
                    var task = BLWFMTicket.GetJobDetailByJobOrderId(REQ.job_id);
                    SendBackTicket objIn = new SendBackTicket();
                    objIn.itemID = task.parent_hpsmid;
                    objIn.remarks = REQ.remarks;
                    objIn.outcomeRefId = "SendBack";

                    objresponse = CALL_SEND_BACK_API(objIn);
                    if (objresponse.status == StatusCodes.OK.ToString())
                    {
                        objresponse.status = StatusCodes.OK.ToString();
                        objresponse.error_message = "Ticket completed successfully and assigned back to source ";
                        objresponse.results = true;
                    }
                    else
                    {
                        objresponse.status = StatusCodes.OK.ToString();
                        objresponse.error_message = "Ticket completed successfully but unable to assign back to source ";
                        objresponse.results = true;
                    }
                }
                else
                {
                    objresponse.status = StatusCodes.INVALID_REQUEST.ToString();
                    objresponse.error_message = "Job id does not exits.";
                    objresponse.results = true;
                }
            }
            catch (Exception ex)
            {
                string RequestData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                logHelper.ApiLogWriter("outercatch:sendBackTicket()", "FEController", RequestData, ex);
                objresponse.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objresponse.error_message = "Error While Processing  Request.";
            }
            return objresponse;
        }
        public WFMMobileApiResponse<dynamic> CALL_SEND_BACK_API(SendBackTicket objIn)
        {
            string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
            string URL = basepath + "sendBackTicket"; 
            //string URL = Convert.ToString(ConfigurationManager.AppSettings["ttsendbackapi"]);
            string DATA = Newtonsoft.Json.JsonConvert.SerializeObject(objIn);
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var response = new WFMMobileApiResponse<dynamic>();
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
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
                        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SendBackTicketResponse>(responses);
                        if (jsonResponse.StatusCode != "0000")
                        {
                            response.results = false;
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.error_message = "Error in sending ticket back :" + " Status desc: " + jsonResponse.StatusDescription;
                            logHelper.ApiLogWriter("CALL_SEND_BACK_API()", "FEController", DATA, null);
                        }
                        else
                        {
                            response.results = true;
                            response.status = StatusCodes.OK.ToString();
                            response.error_message = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    logHelper.ApiLogWriter("outercatch:CALL_SEND_BACK_API()", "FEController", DATA, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Calling API Request.";
                }
            }
            else
            {
                response.error_message = "Access Token is not genrated";
                response.results = false;
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                logHelper.ApiLogWriter("CALL_SEND_BACK_API()", "FEController", DATA, null);
            }

            return response;
        }

        public WFMMobileApiResponse<dynamic> CALL_CLOSE_TICKET_API(string DATA, string URL, string action)
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var response = new WFMMobileApiResponse<dynamic>();
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
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
                        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SendBackTicketResponse>(responses);
                        logHelper.ApiLogWriter("Test:closeapi()", "FEController", "URL:- "+ URL+" Request:- " +DATA+"|| response:- "+responses, null);
                        if (jsonResponse.StatusCode != "0000")
                        {
                            response.results = false;
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.error_message = "Error in " + action + " ticket :" + " Status desc: " + jsonResponse.StatusDescription;
                            logHelper.ApiLogWriter("CALL_CLOSE_TICKET()", "FEController", "Requst : " + DATA + " | Response : " + responses, null);
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
                    logHelper.ApiLogWriter("outercatch:CALL_CLOSE_TICKET_API()", "FEController", DATA, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Calling API Request.";
                }
            }
            else
            {
                response.error_message = "Access Token is not genrated";
                response.results = false;
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                logHelper.ApiLogWriter("CALL_SEND_BACK_API()", "FEController", DATA, null);
            }
            return response;
        }

        [HttpPost]
        [Route("closeTicket")]
        public WFMMobileApiResponse<dynamic> closeTicket(CloseTicket data)
        {
            //ErrorLogHelper logHelper = new ErrorLogHelper();
            var objresponse = new WFMMobileApiResponse<dynamic>();
            string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
            string URL = basepath + "closeTicket";
            //string URL = Convert.ToString(ConfigurationManager.AppSettings["ttcloseTicketapi"]);
            string DATA = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            objresponse = CALL_CLOSE_TICKET_API(DATA, URL, data.outcomeRefId);
            return objresponse;
        }

        [HttpPost]
        [Route("GetIssueType")]
        public WFMMobileApiResponse<dynamic> getIssueType()
        {
            var response = new WFMMobileApiResponse<dynamic>();

            if (ModelState.IsValid)
            {
                try
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("Normal_Issue", "NORMAL ISSUE");
                    data.Add("Assign_Back", "ASSIGN BACK TO SOURCE");
                    var isttcpeactivateapi = Convert.ToBoolean(ConfigurationManager.AppSettings["isttcpeactivateapi"]);
                    if (isttcpeactivateapi)
                    {
                        data.Add("CPE_Replacement", "CPE REPLACEMENT REQUIRED");
                    }
                    // List<IssueType> IT = BLWFMTicket.GetIssueType();
                    response.results = data;
                    response.status = StatusCodes.OK.ToString();
                    return response;
                }
                catch (Exception e)
                {
                    ErrorLogHelper.WriteErrorLog("getIssueType()", "FE", e);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            return response;
        }

        [Route("Encrypt")]
        [HttpPost]
        public string Encrypt(string value)
        {
            string result = WfmEncryption.Encrypt(value);
            return result;
        }

        [Route("Decrypt")]
        [HttpPost]
        public string Decrypt(string value)
        {
            string encryptedtext = value.Replace(" ", "+");
            string result = WfmEncryption.Decrypt(encryptedtext);
            return result;
        }
        [Route("getTTRCAbyJobId")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getTTRCAbyJobId(Models.WFM.ReqInput data)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            getDetailIn objIn = ReqHelper.GetRequestData<getDetailIn>(data);
            try
            {
                var lstobj = BLWFMTicket.getTTRCAbyJobId(objIn.job_id);
                response.results = lstobj;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getTTRCAbyJobId()", "FEController", Convert.ToString(data), ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        [Route("getupdatednapdetails")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> getupdatednapdetails(Models.WFM.ReqInput data)
        {
            dynamic objIn = ReqHelper.GetRequestData<dynamic>(data);
            string job_id = Convert.ToString(objIn.job_id);
            var response = CallgetInventorydetail(job_id);
            return response;
        }
        public WFMMobileApiResponse<dynamic> CallupdateInventorydetail(dynamic reqobject)
        {
            string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
            string URL = basepath + "updateNapportAndName";
            //string URL = Convert.ToString(ConfigurationManager.AppSettings["updatenapdetailsinhobsapi"]);
            var response = new WFMMobileApiResponse<dynamic>();
            ErrorLogHelper logHelper = new ErrorLogHelper();
            string REQUESTDATA = "";
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
            tokenResponse = getAccessToken();
            if (tokenResponse.status == StatusCodes.OK.ToString())
            {
                try
                {
                    REQUESTDATA = Newtonsoft.Json.JsonConvert.SerializeObject(reqobject);
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                    ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers.Add("Authorization", "Bearer " + tokenResponse.results);
                    using (Stream webStream = request.GetRequestStream())
                    using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                    {
                        requestWriter.Write(REQUESTDATA);
                    }
                    WebResponse webResponse = request.GetResponse();
                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        string resp = responseReader.ReadToEnd();
                        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resp);
                        try
                        {
                            if (Convert.ToString(jsonResponse.statusCode) == "0000")
                            {
                                response.status = StatusCodes.OK.ToString();
                                response.error_message = " and also updated in HOBS.";
                            }
                            else
                            {
                                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                response.error_message = " but unable to update in HOBS";
                                logHelper.ApiLogWriter("CallupdateInventorydetail()", "FEController", "Request :" + REQUESTDATA + " | Response :" + resp, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            logHelper.ApiLogWriter("innercatch:CallupdateInventorydetail()", "FEController", REQUESTDATA, ex);
                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            response.error_message = "Error While Parsing Json Response.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    logHelper.ApiLogWriter("outercatch:CallupdateInventorydetail()", "FEController", REQUESTDATA, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Calling API Request.";
                }
            }
            else
            {
                response.error_message = "Access Token is not genrated";
                response.results = false;
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                logHelper.ApiLogWriter("CALLgetInventorydetail()", "FEController", REQUESTDATA, null);
            }
            return response;
        }
        public WFMMobileApiResponse<dynamic> CallgetInventorydetail(string job_id)
        {
            string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
            string URL = basepath + "fetchUpdatedNapDetails";
            //string URL = Convert.ToString(ConfigurationManager.AppSettings["fetchupdatednapdetailsapi"]);
            var response = new WFMMobileApiResponse<dynamic>();
            ErrorLogHelper logHelper = new ErrorLogHelper();

            var tokenResponse = new WFMMobileApiResponse<dynamic>();
            tokenResponse = getAccessToken();
            if (tokenResponse.status == StatusCodes.OK.ToString())
            {
                Task JobOrder = BLWFMTicket.GetJobDetailByJobOrderId(job_id);
                if (JobOrder != null)
                {
                    dynamic reqobject = new ExpandoObject();
                    string orderId = job_id.Split('-')[0];
                    if (JobOrder.ticket_source_id == 1) //sli Ticket
                    {
                        reqobject.orderId = orderId;
                        reqobject.productInstanceId = "";
                    }
                    else//slr tickets
                    {
                        if (!string.IsNullOrEmpty(JobOrder.product_instance_id))
                        {
                            reqobject.orderId = "";
                            reqobject.productInstanceId = JobOrder.product_instance_id;
                        }
                        else
                        {
                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            response.error_message = "Error Product Instance Id not available.";
                            return response;
                        }
                    }

                    try
                    {
                        string REQUESTDATA = Newtonsoft.Json.JsonConvert.SerializeObject(reqobject);
                        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                        ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
                        ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        request.Headers.Add("Authorization", "Bearer " + tokenResponse.results);
                        using (Stream webStream = request.GetRequestStream())
                        using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                        {
                            requestWriter.Write(REQUESTDATA);
                        }

                        WebResponse webResponse = request.GetResponse();
                        using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string responses = responseReader.ReadToEnd();
                            var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responses);
                            try
                            {
                                if (Convert.ToString(jsonResponse.statusCode) == "0000")
                                {
                                    var deviceport = jsonResponse.nimsattributes.napPort;
                                    var devicename = jsonResponse.nimsattributes.napName;

                                    napdetails nap = new napdetails
                                    {
                                        job_id = job_id,
                                        nap = Convert.ToString(devicename),
                                        napport = Convert.ToString(deviceport)
                                    };
                                    if (Convert.ToString(JobOrder.node) != nap.nap || Convert.ToString(JobOrder.nap_port) != nap.napport)
                                    {
                                        if (BLWFMTicket.UpdateNapDetails(nap))
                                        {
                                            response.error_message = "Records updated successfully in WFM  ";
                                            if (JobOrder.ticket_source_id != 1)
                                            {
                                                reqobject = new ExpandoObject();
                                                reqobject.napPort = nap.napport;
                                                reqobject.napName = nap.nap;
                                                reqobject.productInstanceId = JobOrder.product_instance_id;
                                                var responseupdinven = new WFMMobileApiResponse<dynamic>();
                                                responseupdinven = CallupdateInventorydetail(reqobject);
                                                response.error_message += responseupdinven.error_message;
                                            }
                                        }
                                        else
                                            response.error_message = "Some error in updating record in wfm. ";
                                    }
                                    else
                                    {
                                        response.error_message = "Records are already updated!! ";
                                    }
                                    response.status = StatusCodes.OK.ToString();
                                    response.results = nap;
                                }
                                else
                                {
                                    string descripton = Convert.ToString(jsonResponse.statusDesc);
                                    logHelper.ApiLogWriter("CALL_get_Inventory_detail()", "FEController", job_id + " : " + descripton, null);
                                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                    response.error_message = descripton;
                                }
                            }
                            catch (Exception ex)
                            {
                                logHelper.ApiLogWriter("innercatch:CALL_get_Inventory_detail()", "FEController", job_id, ex);
                                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                response.error_message = "Error While Parsing Json Response.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logHelper.ApiLogWriter("outercatch:CALL_get_Inventory_detail()", "FEController", job_id, ex);
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.error_message = "Error While Calling API Request.";
                    }
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = "Job Order not exists.";
                }
            }
            else
            {
                response.error_message = "Access Token is not genrated";
                response.results = false;
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                logHelper.ApiLogWriter("CALLgetInventorydetail()", "FEController", job_id, null);
            }
            return response;
        }

        //public WFMMobileApiResponse<dynamic> CALLgetInventorydetail(string DATA)
        //{
        //    string URL = Convert.ToString(ConfigurationManager.AppSettings["getinventorydetailapi"]);
        //    var response = new WFMMobileApiResponse<dynamic>();
        //    ErrorLogHelper logHelper = new ErrorLogHelper();
        //    Models.WFM.Task JobOrder = BLWFMTicket.GetJobDetailByJobOrderId(DATA);
        //    if (JobOrder != null)
        //    {
        //        try
        //        {
        //            URL += "?id=" + DATA;
        //            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
        //            System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
        //            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
        //            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
        //            WebResponse webResponse = request.GetResponse();
        //            using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
        //            using (StreamReader responseReader = new StreamReader(webStream))
        //            {
        //                string responses = responseReader.ReadToEnd();
        //                var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responses);
        //                try
        //                {
        //                    if (Convert.ToString(jsonResponse.responseCode) == "200")
        //                    {
        //                        var deviceport = jsonResponse.devicePort;
        //                        var devicename = jsonResponse.deviceName;

        //                        napdetails nap = new napdetails
        //                        {
        //                            job_id = DATA,
        //                            nap = Convert.ToString(devicename),
        //                            napport = Convert.ToString(deviceport)
        //                        };
        //                        if (Convert.ToString(JobOrder.node) != nap.nap || Convert.ToString(JobOrder.nap_port) != nap.napport)
        //                        {
        //                            if (BLWFMTicket.UpdateNapDetails(nap))
        //                                response.error_message = "Records updated successfully !!";
        //                            else
        //                                response.error_message = "Some error in updating record.";
        //                        }
        //                        else
        //                        {
        //                            response.error_message = "Records are already updated!!";
        //                        }
        //                        response.status = StatusCodes.OK.ToString();
        //                        response.results = nap;

        //                    }
        //                    else
        //                    {
        //                        string descripton = Convert.ToString(jsonResponse.responseDescription);
        //                        logHelper.ApiLogWriter("outercatch:CALL_get_Inventory_detail()", "FEController", DATA + " : " + descripton, null);
        //                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
        //                        response.error_message = "Error getting response from api";
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    logHelper.ApiLogWriter("outercatch:CALL_get_Inventory_detail()", "FEController", DATA, ex);
        //                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
        //                    response.error_message = "Error While Parsing Json Response.";
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            logHelper.ApiLogWriter("outercatch:CALL_get_Inventory_detail()", "FEController", DATA, ex);
        //            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
        //            response.error_message = "Error While Calling API Request.";
        //        }
        //    }
        //    else
        //    {
        //        response.status = StatusCodes.INVALID_INPUTS.ToString();
        //        response.error_message = "Job Order not exists.";
        //    }
        //    return response;
        //}



        [Route("sendnotification")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> sendnotification(Models.WFM.ReqInput data)
        {
            dynamic objIn = ReqHelper.GetRequestData<dynamic>(data);
            var response = new WFMMobileApiResponse<dynamic>();
            ErrorLogHelper logHelper = new ErrorLogHelper();
            try
            {
                Models.WFM.Task JobOrder = BLWFMTicket.GetJobDetailByJobOrderId(objIn.job_id.ToString());
                Route_Issue ri = BLWFMTicket.GetRoute_Issue(JobOrder.hpsm_ticket_id);
                BLUser objuser = new BLUser();
                Models.User u = objuser.GetUserDetailByID(ri.user_id);
                u.name = MiscHelper.Decrypt(u.name);
                PortManager portManager = BLWFMTicket.GetPortManager(JobOrder.stateorprovince);
                napportnotificationdata nnd = new napportnotificationdata();
                if (portManager != null)
                {
                    if (!string.IsNullOrEmpty(portManager.email))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(" Job ID : " + JobOrder.hpsmid);
                        sb.AppendLine(" Nap : " + JobOrder.node);
                        sb.AppendLine(" Nap port : " + JobOrder.nap_port);
                        sb.AppendLine(" Assigned to : " + u.name);
                        sb.AppendLine(" Issue : " + objIn.issue.ToString());
                        nnd.user_id = objIn.user_Id.ToString();
                        nnd.ticket_id = JobOrder.hpsm_ticket_id;
                        nnd.job_id = objIn.job_id.ToString();
                        nnd.issue = objIn.issue.ToString();
                        nnd.subject_line = "Nap or nap port not working";
                        nnd.assign_to = u.name;
                        nnd.nap = JobOrder.node;
                        nnd.nap_port = JobOrder.nap_port;
                        nnd.to_email = portManager.email;
                        nnd.to_mobile = portManager.mobile_number;
                        nnd.email_message = sb.ToString();
                        nnd.sms_message = "Nap or nap port not working\n" + sb.ToString();
                        bool mailsent = Email.sendMailnnd(nnd.to_email, nnd.subject_line, nnd.email_message);
                        if (mailsent)
                        {
                            nnd.email_result = "success";
                            nnd.email_response = "mail sent successfully";
                            response.error_message = "mail sent successfully";
                            response.status = StatusCodes.PARTIAL_SUCCESS.ToString();
                        }
                        else
                        {
                            nnd.email_result = "failure";
                            nnd.email_response = "unable to send mail";
                            response.error_message = "unable to send mail";
                        }

                        /////////////////////// code to send sms
                        if (!string.IsNullOrEmpty(nnd.to_mobile))
                        {
                            //bool smssent = SMS.SendSms(nnd.to_mobile.ToString(), nnd.sms_message);

                            bool smssent = false;
                            string[] Multi = nnd.to_mobile.Split(',');
                            foreach (string s in Multi)
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    smssent = SMS.SendSms(s, nnd.sms_message);
                                }
                            }

                            if (smssent)
                            {
                                nnd.sms_result = "success";
                                nnd.sms_response = "sms send successfully";
                                response.error_message += " sms send successfully";
                                if (mailsent)
                                    response.status = StatusCodes.OK.ToString();
                                else
                                    response.status = StatusCodes.PARTIAL_SUCCESS.ToString();
                            }
                            else
                            {
                                nnd.sms_result = "failure";
                                nnd.sms_response = "unable to send sms.";
                                response.error_message += " unable to send sms.";
                            }
                        }
                        else
                        {
                            nnd.sms_result = "failure";
                            nnd.sms_response = "mobile number not available.";
                            response.error_message += "mobile number not available.";
                            if (mailsent)
                                response.status = StatusCodes.PARTIAL_SUCCESS.ToString();
                            else
                                response.status = StatusCodes.FAILED.ToString();

                        }
                        UpdateNotificationLog(nnd);
                    }
                    else
                    {
                        nnd.email_result = "failure";
                        nnd.email_response = "email not available";
                        response.error_message = "email not available";
                        response.status = StatusCodes.FAILED.ToString();
                    }
                }
                else
                {
                    response.error_message = "port manager not available";
                    response.status = StatusCodes.FAILED.ToString();
                }
            }
            catch (Exception ex)
            {
                string DATA = Newtonsoft.Json.JsonConvert.SerializeObject(objIn);
                logHelper.ApiLogWriter("outercatch:send_Notification()", "FEController", DATA, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Calling API Request.";
            }
            return response;
        }

        public WFMMobileApiResponse<dynamic> UpdateNotificationLog(napportnotificationdata objIn)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                var res = BLWFMTicket.UpdateNotificationLog(objIn);

                if (res > 0)
                {
                    response.error_message = " save successfully.";
                    response.status = StatusCodes.OK.ToString();
                    return response;
                }
                else
                {
                    response.error_message = "Unable to save ";
                    response.status = StatusCodes.FAILED.ToString();
                    return response;
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("saveUpdateNotificationLog()", "FEController", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing Request.";
            }
            return response;
        }
        [Route("UpdateReTrigger")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> UpdateReTrigger(Models.WFM.ReqInput data)
        {
            Models.WFM.job_order_status obj = ReqHelper.GetRequestData<Models.WFM.job_order_status>(data);
            var response = new WFMMobileApiResponse<dynamic>();
            int res = 0;
            try
            {
                res = BLWFMTicket.UpdateReTrigger(obj);
                if (res > 0)
                {
                    response.error_message = "Job status updated successfully";
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "No record found.";
                }

            }
            catch (Exception ex)
            {
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                ErrorLogHelper logHelper = new ErrorLogHelper();
                var responeData = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                logHelper.ApiLogWriter("UpdateReTrigger()", "FEController", responeData, ex);
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        private WFMMobileApiResponse<dynamic> getSapAccessToken()
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var objresponse = new WFMMobileApiResponse<dynamic>();
            string DATA = "";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                string URL = Convert.ToString(ConfigurationManager.AppSettings["saptokenapi"]);
                Uri Muri = new Uri(URL);
                User_Detail result = new User_Detail();
                result.clientId = Convert.ToString(ConfigurationManager.AppSettings["sapclientId"]);
                result.clientSecret = Convert.ToString(ConfigurationManager.AppSettings["sapclientSecret"]);
                DATA = "grant_type=client_credentials";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                NetworkCredential myNetworkCredential = new NetworkCredential(result.clientId, result.clientSecret);
                CredentialCache myCredentialCache = new CredentialCache();
                myCredentialCache.Add(Muri, "Basic", myNetworkCredential);
                request.PreAuthenticate = true;
                request.Credentials = myCredentialCache;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (Stream webStream = request.GetRequestStream())
                using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                {
                    requestWriter.Write(DATA);
                }
                string responses = "";
                try
                {
                    WebResponse webResponse = request.GetResponse();
                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        responses = responseReader.ReadToEnd();
                        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responses);
                        objresponse.results = Convert.ToString(jsonResponse.access_token);
                        objresponse.status = StatusCodes.OK.ToString();
                        if (objresponse.results == "")
                        {
                            logHelper.ApiLogWriter("getsapAccessToken()", "FEController", DATA, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logHelper.ApiLogWriter("innercatch:getsapAccessToken()", "FEController", "request : " + DATA + "response : " + responses, ex);
                    objresponse.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    objresponse.error_message = "Error While Processing  Request.";
                }
            }
            catch (Exception ex)
            {
                logHelper.ApiLogWriter("outercatch:getsapAccessToken()", "FEController", DATA, ex);
                objresponse.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objresponse.error_message = "Error While Processing  Request.";
            }
            return objresponse;
        }
        private WFMMobileApiResponse<dynamic> CallgetInventoryBalance(string serial_number, int user_id, int material_id = 0)//search value
        {
            var response = new WFMMobileApiResponse<dynamic>();
            ErrorLogHelper logHelper = new ErrorLogHelper();
            string REQUESTDATA = "";
            try
            {
                BLUser bluser = new BLUser();
                Models.User u = bluser.GetUserDetailByID(user_id);
                AdditionalMaterialMaster material = BLWFMTicket.GetAdditionalMaterialMaster(material_id);
                if (u != null)
                {
                    var lstwarehouse = new BLUserWarehouseCodeMapping().GetWarehouseCodeMapping(u.user_id).Select(x => x.warehouse_code);
                    u.warehouse_codes = lstwarehouse.FirstOrDefault();
                    string item_code;
                    if (material != null)
                    {
                        if (!string.IsNullOrEmpty(material.material_code))
                        {
                            item_code = material.material_code;
                        }
                        else
                        {
                            response.error_message = "Material code not available.";
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            return response;
                        }
                    }
                    else
                    {
                        item_code = "";
                    }

                    if (string.IsNullOrEmpty(item_code) && string.IsNullOrEmpty(serial_number))
                    {
                        response.error_message = "Search data Not Available";
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        return response;
                    }
                    string URL = Convert.ToString(ConfigurationManager.AppSettings["sapinvbalapi"]);
                    //string URL = Convert.ToString(ConfigurationManager.AppSettings["sapbasicapi"]) + "inventory_bal";
                    var tokenResponse = new WFMMobileApiResponse<dynamic>();
                    tokenResponse = getSapAccessToken();
                    if (tokenResponse.status == StatusCodes.OK.ToString())
                    {
                        if (!string.IsNullOrEmpty(u.warehouse_codes))
                        {
                            dynamic reqobject = new ExpandoObject();
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND = new ExpandoObject();
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND.INPUT = new ExpandoObject();
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND.INPUT.FROM_DATE = DateTime.Now.ToString("yyyyMMdd");
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND.INPUT.TO_DATE = DateTime.Now.ToString("yyyyMMdd");
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND.INPUT.MESSAGE_ID_PARTNER = "";
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND.INPUT.PARTNER = "";
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND.INPUT.PLANT = "1000";
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND.INPUT.INSTALLER_ID = u.warehouse_codes;// "TDS1000001";//??
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND.INPUT.SERIAL_NUMBER = serial_number;//search value
                            reqobject.ZFM_MMO0001_INVTBALNC_OUTBOUND.INPUT.ITEM_CODE = item_code;//search value

                            REQUESTDATA = Newtonsoft.Json.JsonConvert.SerializeObject(reqobject);
                            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
                            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                            request.Method = "POST";
                            request.ContentType = "application/json";
                            request.Headers.Add("Authorization", "Bearer " + tokenResponse.results);
                            using (Stream webStream = request.GetRequestStream())
                            using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                            {
                                requestWriter.Write(REQUESTDATA);
                            }
                            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
                            using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                            using (StreamReader responseReader = new StreamReader(webStream))
                            {
                                string responses = responseReader.ReadToEnd();
                                var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responses);
                                try
                                {
                                    if (webResponse.StatusCode.ToString() == "OK")
                                    {
                                        response.status = webResponse.StatusCode.ToString();
                                        var OutResponse = jsonResponse.ZFM_MMO0001_INVTBALNC_OUTBOUNDResponse;
                                        var ObjOutput = OutResponse.OUTPUT;
                                        string records = Convert.ToString(ObjOutput.GENERAL.NO_OF_RECORDS);
                                        if (string.IsNullOrEmpty(records))
                                            records = "0";
                                        if (int.TryParse(records, out int Nrecords))
                                        {
                                            if (Nrecords > 0)
                                            {
                                                response.results = ObjOutput.ITEMS.item;
                                                response.status = StatusCodes.OK.ToString();
                                            }
                                            else
                                            {
                                                response.status = StatusCodes.ZERO_RESULTS.ToString();
                                                response.error_message = "" + Convert.ToString(ObjOutput.GENERAL.MESSAGE_DESCRIPTION);
                                            }
                                        }
                                        else
                                        {
                                            response.status = StatusCodes.EXCEPTION.ToString();
                                            response.error_message = "Error in TryParse response";
                                            logHelper.ApiLogWriter("CallgetInventorybalance()", "FEController", "Request : " + REQUESTDATA + "|Response :" + responses, null);
                                        }
                                    }
                                    else
                                    {
                                        logHelper.ApiLogWriter("CallgetInventorybalance()", "FEController", "Request : " + REQUESTDATA, null);
                                        response.status = webResponse.StatusCode.ToString();
                                        response.error_message = "error in getting response";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logHelper.ApiLogWriter("innercatch:CallgetInventorybalance()", "FEController", REQUESTDATA, ex);
                                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                    response.error_message = "Error While Parsing Json Response.";
                                }
                            }
                        }
                        else
                        {
                            response.error_message = "Installer Id not Availablle.";
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                        }
                    }
                    else
                    {
                        response.error_message = "Access Token is not genrated";
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        //logHelper.ApiLogWriter("CallgetInventorybalance()", "FEController", "", null);
                    }
                }
                else
                {
                    response.error_message = "User Not Available";
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                }
            }
            catch (Exception ex)
            {
                logHelper.ApiLogWriter("outercatch:CallgetInventorybalance()", "FEController", REQUESTDATA, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Calling API Request.";
            }
            return response;
        }
        private WFMMobileApiResponse<dynamic> SendSerialnumberRemoveToSAP(Task task)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                string action = task.action == "" ? "Add" : task.action;
                var objRouteIssue = BLWFMTicket.GetRoute_Issue(task.hpsm_ticket_id);
                BLUser bluser = new BLUser();
                Models.User u = bluser.GetUserDetailByID(objRouteIssue.user_id);
                var lstwarehouse = new BLUserWarehouseCodeMapping().GetWarehouseCodeMapping(u.user_id).Select(x => x.warehouse_code);
                u.warehouse_codes = lstwarehouse.FirstOrDefault();
                if (!string.IsNullOrEmpty(u.warehouse_codes))
                {
                    dynamic itm = new ExpandoObject();
                    List<dynamic> lstitem = new List<dynamic>();
                    dynamic reqobject = new ExpandoObject();
                    //#region add
                    //itm.PLANT = "1000";
                    //itm.COST_CENTRE = "1004030010";//hard code value as per document
                    //itm.MOVEMENT_TYPE = "Z01";//in case of cpe item
                    //itm.ORDER_ID = "";//left blank as per instruction in document
                    //itm.MOVEMENT_INDICATOR = "";//left blank as per instruction in document
                    //itm.ENTRY_QUANTITY = 1;
                    //itm.ITEM_TEXT = task.city;
                    //itm.MATERIAL = task.cpe_item_code;
                    //itm.STORAGE_LOCATION = task.cpe_wh;
                    //itm.ENTRY_UOM = task.cpe_uom;
                    //itm.INSTALLER_ID = u.warehouse_codes;
                    //itm.SERIAL_NUMBER = task.cpe_serialno;
                    //itm.BATCH_NUMBER = "";
                    //lstitem.Add(itm);
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND = new ExpandoObject();
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT = new ExpandoObject();
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.POSTING_DATE = DateTime.Now.ToString("yyyyMMdd");
                    ////reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.DOCUMENT_DATE = DateTime.Now.ToString("yyyyMMdd");
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.REF_DOCUMENT_NO = task.hpsmid + "_" + task.city;
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.HEADER_TEXT = task.account_number + "_" + task.subscriber_name;
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.MESSAGE_ID_PARTNER = "";
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.PARTNER = "";
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.INSTALLER_ID = u.warehouse_codes;
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items = new ExpandoObject();
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items.item = new ExpandoObject();
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items.item = lstitem.ToArray();
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.GM_CODE = "03";//add
                    //response = CallgetMaterialConsumption(reqobject);
                    //#endregion

                    #region remove
                    lstitem = new List<dynamic>();
                    itm = new ExpandoObject();
                    itm.PLANT = "1000";
                    itm.COST_CENTRE = "1004030010";//hard code value as per document
                    itm.MOVEMENT_TYPE = "Z01";//in case of cpe item
                    itm.ORDER_ID = "";//left blank as per instruction in document
                    itm.MOVEMENT_INDICATOR = "";//left blank as per instruction in document
                    itm.ENTRY_QUANTITY = 1;
                    itm.ITEM_TEXT = task.city;
                    itm.MATERIAL = task.cpe_item_code; ////// need to get old cpe item code
                    itm.STORAGE_LOCATION = task.cpe_wh;//////  
                    itm.ENTRY_UOM = task.cpe_uom;      ////// need to get old uom of cpe item
                    itm.INSTALLER_ID = u.warehouse_codes;
                    itm.SERIAL_NUMBER = task.current_cpesn;//old cpe sno
                    itm.BATCH_NUMBER = "";
                    lstitem.Add(itm);
                    reqobject = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.POSTING_DATE = DateTime.Now.ToString("yyyyMMdd");
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.DOCUMENT_DATE = DateTime.Now.ToString("yyyyMMdd");
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.REF_DOCUMENT_NO = task.hpsmid + "_" + task.city;
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.HEADER_TEXT = task.account_number + "_" + task.subscriber_name;
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.MESSAGE_ID_PARTNER = "";
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.PARTNER = "";
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.INSTALLER_ID = u.warehouse_codes;
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items.item = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items.item = lstitem.ToArray();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.GM_CODE = "05";//remove
                    response = CallgetMaterialConsumption(reqobject);
                    #endregion

                }
                else
                {
                    response.error_message = "Installer Id not availablle.";
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                }
            }
            catch (Exception ex)
            {
                response.error_message = Convert.ToString(ex.Message);
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SendSerialnumberToSAP()", "FEController", task.hpsmid, ex);
            }
            return response;
        }
        private WFMMobileApiResponse<dynamic> SendSerialnumberToSAP(Task task)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                string action = task.action == "" ? "Add" : task.action;
                var objRouteIssue = BLWFMTicket.GetRoute_Issue(task.hpsm_ticket_id);
                BLUser bluser = new BLUser();
                Models.User u = bluser.GetUserDetailByID(objRouteIssue.user_id);
                var lstwarehouse = new BLUserWarehouseCodeMapping().GetWarehouseCodeMapping(u.user_id).Select(x => x.warehouse_code);
                u.warehouse_codes = lstwarehouse.FirstOrDefault();
                if (!string.IsNullOrEmpty(u.warehouse_codes))
                {
                    List<dynamic> lstitem = new List<dynamic>();
                    dynamic itm = new ExpandoObject();
                    itm.MATERIAL = task.cpe_item_code;
                    itm.PLANT = "1000";
                    itm.STORAGE_LOCATION = task.cpe_wh;
                    itm.MOVEMENT_TYPE = "Z01";//in case of cpe item
                    itm.ENTRY_QUANTITY = 1;
                    itm.ENTRY_UOM = task.cpe_uom;
                    itm.ITEM_TEXT = task.city;
                    itm.ORDER_ID = "";//left blank as per instruction in document
                    itm.MOVEMENT_INDICATOR = "";//left blank as per instruction in document
                    itm.COST_CENTRE = "1004030010";//hard code value as per document
                    itm.INSTALLER_ID = u.warehouse_codes;
                    itm.SERIAL_NUMBER = task.cpe_serialno;
                    itm.BATCH_NUMBER = "";
                    lstitem.Add(itm);
                    dynamic reqobject = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.POSTING_DATE = DateTime.Now.ToString("yyyyMMdd");
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.DOCUMENT_DATE = DateTime.Now.ToString("yyyyMMdd");
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.REF_DOCUMENT_NO = task.hpsmid + "_" + task.city;
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.HEADER_TEXT = task.account_number + "_" + task.subscriber_name;
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.MESSAGE_ID_PARTNER = "";
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.PARTNER = "";
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.INSTALLER_ID = u.warehouse_codes;
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items.item = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items.item = lstitem.ToArray();
                    if (action.ToUpper() == "ADD")
                    {
                        reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.GM_CODE = "03";
                    }
                    else
                    {
                        reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.GM_CODE = "05";
                    }
                    response = CallgetMaterialConsumption(reqobject);
                }
                else
                {
                    response.error_message = "Installer Id not availablle.";
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                }
            }
            catch (Exception ex)
            {
                response.error_message = Convert.ToString(ex.Message);
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SendSerialnumberToSAP()", "FEController", task.hpsmid, ex);
            }
            return response;
        }
        private WFMMobileApiResponse<dynamic> SendaddMaterialToSAP(Task task, List<AdditionalMaterial> additionalMaterialsDetail)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            try
            {
                var objRouteIssue = BLWFMTicket.GetRoute_Issue(task.hpsm_ticket_id);
                BLUser bluser = new BLUser();
                Models.User u = bluser.GetUserDetailByID(objRouteIssue.user_id);
                var lstwarehouse = new BLUserWarehouseCodeMapping().GetWarehouseCodeMapping(u.user_id).Select(x => x.warehouse_code);
                u.warehouse_codes = lstwarehouse.FirstOrDefault();
                if (!string.IsNullOrEmpty(u.warehouse_codes))
                {
                    //var lstmaterialmaster = BLWFMTicket.GetAdditionalMaterialMaster(task.ticket_type);
                    //var lstobj = BLWFMTicket.GetAdditionalMaterial(task.hpsmid);
                    List<dynamic> lstitem = new List<dynamic>();
                    foreach (var item in additionalMaterialsDetail)
                    {
                        dynamic itm = new ExpandoObject();
                        itm.MATERIAL = item.item_code;
                        itm.PLANT = "1000";
                        itm.STORAGE_LOCATION = item.wh_code;
                        itm.MOVEMENT_TYPE = "Z03";//in case of cpe item
                        itm.ENTRY_QUANTITY = item.quantity;
                        itm.ENTRY_UOM = item.uom;
                        itm.ITEM_TEXT = task.city;
                        itm.ORDER_ID = "";//left blank as per instruction in document
                        itm.MOVEMENT_INDICATOR = "";//left blank as per instruction in document
                        itm.COST_CENTRE = "1004030010";//hard code value as per document
                        itm.INSTALLER_ID = u.warehouse_codes;
                        itm.SERIAL_NUMBER = item.serial_no;
                        itm.BATCH_NUMBER = item.batch_no;
                        lstitem.Add(itm);
                    }
                    dynamic reqobject = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.POSTING_DATE = DateTime.Now.ToString("yyyyMMdd");
                    //reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.DOCUMENT_DATE = DateTime.Now.ToString("yyyyMMdd");
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.REF_DOCUMENT_NO = task.hpsmid + "_" + task.city;
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.HEADER_TEXT = task.account_number + "_" + task.subscriber_name;
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.MESSAGE_ID_PARTNER = "";
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.PARTNER = "";
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.INSTALLER_ID = u.warehouse_codes;
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.GM_CODE = "03";
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items.item = new ExpandoObject();
                    reqobject.ZFM_MMI0001_MATCONSP_OUTBOUND.INPUT.Items.item = lstitem.ToArray();
                    response = CallgetMaterialConsumption(reqobject);
                }
                else
                {
                    response.error_message = "Installer Id not Availablle.";
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                }
            }
            catch (Exception ex)
            {
                response.error_message = Convert.ToString(ex.Message);
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SendaddMaterialToSAP()", "FEController", task.hpsmid, ex);
            }
            return response;
        }
        private WFMMobileApiResponse<dynamic> CallgetMaterialConsumption(dynamic reqobject)
        {
            string URL = Convert.ToString(ConfigurationManager.AppSettings["sapmatconapi"]);
            //string URL = Convert.ToString(ConfigurationManager.AppSettings["sapbasicapi"]) + "material_consumption";
            var response = new WFMMobileApiResponse<dynamic>();
            ErrorLogHelper logHelper = new ErrorLogHelper();
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
            tokenResponse = getSapAccessToken();
            if (tokenResponse.status == StatusCodes.OK.ToString())
            {
                string REQUESTDATA = "";
                try
                {
                    REQUESTDATA = Newtonsoft.Json.JsonConvert.SerializeObject(reqobject);
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                    ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers.Add("Authorization", "Bearer " + tokenResponse.results);
                    using (Stream webStream = request.GetRequestStream())
                    using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                    {
                        requestWriter.Write(REQUESTDATA);
                    }
                    HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        string responses = responseReader.ReadToEnd();
                        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responses);
                        try
                        {
                            if (webResponse.StatusCode.ToString() == "OK")
                            {

                                var OutResponse = jsonResponse.ZFM_MMI0001_MATCONSP_OUTBOUNDResponse;
                                var ObjOutput = OutResponse.OUTPUT;
                                if (Convert.ToString(ObjOutput.GENERAL.MESSAGE_CODE) == "S")
                                {
                                    response.status = "success";
                                }
                                else
                                {
                                    response.status = StatusCodes.FAILED.ToString();
                                    response.error_message = "Error in sending request to SAP. " + Convert.ToString(ObjOutput.GENERAL.MESSAGE_DESCRIPTION);
                                    logHelper.ApiLogWriter("CallgetMaterialConsumption()", "FEController", "Request: " + REQUESTDATA + "| Response:" + response.error_message, null);

                                }
                            }
                            else
                            {
                                response.status = webResponse.StatusCode.ToString();
                                response.error_message = "error in getting response";
                                logHelper.ApiLogWriter("CallgetMaterialConsumption()", "FEController", "Request: " + REQUESTDATA, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            logHelper.ApiLogWriter("innercatch:CallgetMaterialConsumption()", "FEController", REQUESTDATA, ex);
                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            response.error_message = "Error While Parsing Json Response.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    logHelper.ApiLogWriter("outercatch:CallgetMaterialConsumption()", "FEController", REQUESTDATA, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Calling API Request.";
                }
            }
            else
            {
                response.error_message = "Access Token is not genrated";
                response.results = false;
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                logHelper.ApiLogWriter("CallgetMaterialConsumption()", "FEController", "", null);
            }
            return response;
        }
        private WFMMobileApiResponse<dynamic> SendSerialnumberRemoveToERP(Task task)
        {
            var response = new WFMMobileApiResponse<dynamic>();
            string inputData = "";
            string results = "";
            try
            {

                //   var task = BLWFMTicket.GetJobDetailByJobOrderId(ticketDetails.hpsmid);
                inputData = "Action: " + task.action + ", TranNo: " + task.hpsmid + ", WHCode: " + task.cpe_wh + ", ItemCode: " + task.cpe_item_code + ", Serial: " + task.cpe_serialno;
                string action = task.action == "" ? "Add" : task.action;
                ConvergeERPService.I_ERP_ServiceClient er = new ConvergeERPService.I_ERP_ServiceClient();
                string TranNo = task.hpsmid;  //Order id
                string WHCode = task.cpe_wh;  //CPE_WH
                string ItemCode = task.old_cpe_item_code;  // CPE_ITEM_CODE
                string Serial = task.current_cpesn;  //cpe_serialno
                                                     //string TranNo = "T1213126"; //Order id
                                                     //string WHCode = "003829"; //CPE_WH
                                                     //string ItemCode = "CPE00000188";// CPE_ITEM_CODE
                                                     //string Serial = "48575443ECB04EA8";//cpe_serialno

                //if ((action).ToUpper() == "ADD")
                //{
                //    ConvergeERPService.InvIssueDtl[] dtl = new ConvergeERPService.InvIssueDtl[1];
                //    ConvergeERPService.InvIssueDtl dd = new ConvergeERPService.InvIssueDtl();
                //    dd.ItemCode = ItemCode;
                //    dd.Serial = Serial;
                //    dd.UOM = "PC";
                //    dd.Quantity = 1;
                //    dtl[0] = dd;
                //    results = er.CreateInvIssue(TranNo, " ", " ", " ", WHCode, dtl);
                //}
                //else if ((action).ToUpper() == "REMOVE")
                //{
                ConvergeERPService.InvReturn[] dtlreturn = new ConvergeERPService.InvReturn[1];
                ConvergeERPService.InvReturn dd1 = new ConvergeERPService.InvReturn();
                dd1.ItemCode = ItemCode;
                dd1.Serial = Serial;
                dd1.UOM = "PC";
                dd1.Quantity = 1;
                dtlreturn[0] = dd1;
                results = er.CreateInvReturn(TranNo, WHCode, dtlreturn);

                //}
                response.results = results;
            }
            catch (Exception ex)
            {
                response.error_message = Convert.ToString(ex.Message);
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SendSerialnumberToERP()", "FEController", inputData, ex);

            }
            return response;
        }
        [Route("GetPaymentDetails")]
        [HttpPost]
        public WFMMobileApiResponse<Payment_Collection_Details> GetPaymentDetails(Models.WFM.ReqInput data)
        {
            WFMMobileApiResponse<Payment_Collection_Details> response = new WFMMobileApiResponse<Payment_Collection_Details>();
            getDetailIn objIn = ReqHelper.GetRequestData<getDetailIn>(data);
            ErrorLogHelper logHelper = new ErrorLogHelper();
            try
            {
                Models.WFM.Task JobOrder = BLWFMTicket.GetJobDetailByJobOrderId(objIn.job_id);
                if (JobOrder != null)
                {
                    Payment_Collection_Details objpaycol = new Payment_Collection_Details();
                    objpaycol.paymentSplitup = BLWFMTicket.GetPaymentDetailsByJobOrder(objIn.job_id);
                    objpaycol.payment_type = JobOrder.payment_type;
                    objpaycol.payment_status = JobOrder.payment_status;
                    objpaycol.total_amount = JobOrder.total_amount;
                    objpaycol.payment_modes = new string[] { "CASH" };
                    response.results = objpaycol;
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = "Job id does not exists";
                    response.results = null;
                }
            }
            catch (Exception ex)
            {
                string DATA = Newtonsoft.Json.JsonConvert.SerializeObject(objIn);
                logHelper.ApiLogWriter("outercatch:GetPaymentDetails()", "FEController", DATA, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
            }
            return response;
        }
        [Route("UpdatePaymentDetails")]
        [HttpPost]
        public WFMMobileApiResponse<dynamic> UpdatePaymentDetails(Models.WFM.ReqInput data)
        {
            string msg = "";
            WFMMobileApiResponse<dynamic> response = new WFMMobileApiResponse<dynamic>();
            dynamic objIn = ReqHelper.GetRequestData<dynamic>(data);
            ErrorLogHelper logHelper = new ErrorLogHelper();
            bool iscpeactivateapi = Convert.ToBoolean(ConfigurationManager.AppSettings["iscpeactivateapi"]);
            try
            {
                string jobid = Convert.ToString(objIn.job_id);
                Models.WFM.Task JobOrder = BLWFMTicket.GetJobDetailByJobOrderId(jobid);
                if (JobOrder != null)
                {
                    Models.User u = new BLUser().GetUserDetailByID(Convert.ToInt32(objIn.user_id));
                    if (JobOrder.payment_status.ToUpper() != "PAID")
                    {
                        dynamic objMakePaymentCod = new ExpandoObject();
                        string orderId = jobid.Split('-')[0];
                        objMakePaymentCod.orderId = orderId;
                        objMakePaymentCod.paymentStatus = "PAID";
                        objMakePaymentCod.amount = JobOrder.total_amount;
                        objMakePaymentCod.ARNumber = Convert.ToString(objIn.ar_no);
                        objMakePaymentCod.paymentMode = Convert.ToString(objIn.payment_mode);
                        objMakePaymentCod.triggeredBy = u.user_name;
                        objMakePaymentCod.paymentSource = "FIELD_SALES_AGENT";
                        int res = 0;
                        JobOrder.payment_status = "PAID";
                        JobOrder.ar_no = Convert.ToString(objIn.ar_no);
                        JobOrder.payment_mode = Convert.ToString(objIn.payment_mode);
                        if (iscpeactivateapi)
                        {
                            WFMMobileApiResponse<dynamic> reshobs = CallmakePaymentCOD(objMakePaymentCod);
                            if (reshobs.results)
                            {
                                res = BLWFMTicket.UpdatePaymentDetail(JobOrder.hpsmid, JobOrder.ar_no, JobOrder.payment_mode, JobOrder.payment_status);
                            }
                            else
                            {
                                res = 0;
                                msg = reshobs.error_message;
                            }
                        }
                        else
                        {
                            res = BLWFMTicket.UpdatePaymentDetail(JobOrder.hpsmid, JobOrder.ar_no, JobOrder.payment_mode, JobOrder.payment_status);
                        }
                        if (res > 0)
                        {
                            response.status = StatusCodes.OK.ToString();
                            response.error_message = "Payment updated successfully.";
                        }
                        else
                        {
                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            response.error_message = "Some error in updating payment records. " + msg;
                            string DATA = Newtonsoft.Json.JsonConvert.SerializeObject(objIn);
                            logHelper.ApiLogWriter("UpdatePaymentDetails()", "FEController", "Request :" + DATA + "|Response :" + res, null);
                        }
                    }
                    else
                    {
                        response.status = StatusCodes.REJECTED.ToString();
                        response.error_message = "Payment already paid.";
                    }
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = "Job id does not exists";
                }
            }
            catch (Exception ex)
            {
                string DATA = Newtonsoft.Json.JsonConvert.SerializeObject(objIn);
                logHelper.ApiLogWriter("outercatch:UpdatePaymentDetails()", "FEController", DATA, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
            }
            return response;
        }
        private WFMMobileApiResponse<dynamic> CallmakePaymentCOD(dynamic reqobject)
        {
            string basepath = Convert.ToString(ConfigurationManager.AppSettings["CircuitServiceBaseApi"]);
            string URL = basepath + "makePaymentCOD"; 
            //string URL = Convert.ToString(ConfigurationManager.AppSettings["paymentcollectionapi"]);
            string DATA = Newtonsoft.Json.JsonConvert.SerializeObject(reqobject);
            var response = new WFMMobileApiResponse<dynamic>();
            var tokenResponse = new WFMMobileApiResponse<dynamic>();
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
                            response.results = false;
                            response.status = StatusCodes.INVALID_INPUTS.ToString();
                            response.error_message = " Status desc : " + Convert.ToString(jsonResponse.statusDesc);
                            logHelper.ApiLogWriter("CallmakePaymentCOD()", "FEController", "Request : " + DATA + "|Response : " + responses, null);
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
                    logHelper.ApiLogWriter("outercatch:CallmakePaymentCOD()", "FEController", "request:" + DATA, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Calling API Request.";
                }
            }
            else
            {
                response.error_message = "Access Token is not genrated";
                response.results = false;
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                logHelper.ApiLogWriter("CallmakePaymentCOD()", "FEController", "request:" + DATA, null);
            }
            return response;
        }
    }
}