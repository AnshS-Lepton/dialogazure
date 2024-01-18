using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DataAccess.WFM;
using Models;
using Models.WFM;

namespace BusinessLogics.WFM
{
    public class BLWFMTicket
    {
        public static int InsertSlotRequest(SlotRequest obj)
        {
            return (new DLWFMTicket().InsertSlotRequest(obj));

        }
        public static int UpdateSlotRequesNew(SlotRequest obj)
        {
            return (new DLWFMTicket().UpdateSlotRequesNew(obj));

        }
        public static int InsertEmailSmsLog(wfm_email_sms_log obj)
        {
            return (new DLWFMEmailsmsLog().InsertEmailSmsLog(obj));

        }
        public static int updateSlotRequest(SlotRequest obj)
        {
            return (new DLWFMTicket().updateSlotRequest(obj));
        }
        public static SlotRequest GetSlotRequest(string refereanceid)
        {
            return (new DLWFMTicket().GetSlotRequest(refereanceid));

        }
        public static List<ManagerList> GetManagerList(double lat, double lng, string role_name = null)
        {
            return DLWFMTicket.GetManagerList(lat, lng, role_name);

        }
        public static string GetBlockName(double lat, double lng)
        {
            return DLWFMTicket.GetBlockName(lat, lng);

        }

        public static int updateSlotRequestAppmntDate(string referenceid, DateTime appointment_date)
        {
            return (new DLWFMTicket().updateSlotRequestAppmntDate(referenceid, appointment_date));
        }

        public static List<FEList> GetFEList(int ManagerUserId)
        {
            return (new DLUserMaster().GetFEList(ManagerUserId));

        }
        public static List<FEList> GetFEListbylocation(int ManagerUserId, double lat, double lng, string role)
        {
            return (new DLUserMaster().GetFEListbylocation(ManagerUserId,  lat,  lng,  role));

        }
        public static UserServiceDetail GetServiceDetailByUserId(int userid)
        {
            return (new DLUserMaster().GetServiceDetailByUserId(userid));

        }
        public static List<UserServiceDetail> GetFEDetailByUserId(int userid)
        {
            return (new DLUserMaster().GetFEDetailByUserId(userid));

        }
        

        public static string GetUserNameById(int ManagerUserId)
        {
           return (new DLUserMaster().GetUserNameById(ManagerUserId));
        }

        public static User_Master GetUserById(int UserId)
        {
            return (new DLUserMaster().GetUserById(UserId));
        }
        public static UserDetails GetUserRoleDetailsById(int UserId)
        {
            return (new DLUserMaster().GetUserRoleDetailsById(UserId));
        }
        public static bool isTimeSheetDefined(int userId, DateTime date_check_in, int day_of_week, string strTime, out string Error)
        {
            return DLWFMTicket.isTimeSheetDefined(userId, date_check_in, day_of_week, strTime, out Error);
        }
        public static RosterVW GetUserTimeSheet(int userId, DateTime date_check_in, int day_of_week, string strTime)
        {
            return DLWFMTicket.GetUserTimeSheet(userId, date_check_in, day_of_week, strTime);
        }
        //TB
        public static ServiceType Getservicetype(string serviceType)
        {
            return (new DLWFMServiceType()).Getservicetype(serviceType);

        }

        public static List<EFSkill> GetSkillFE(string skilid)
        {
            return DLWFMTicket.GetSkillFE(skilid);

        }

        public static List<FEList> GetAVFE(List<FEList> FEList, List<EFSkill> SKFEList)
        {
            return DLWFMTicket.GetAVFE(FEList, SKFEList);

        }


        public static List<SlotResponse> GetSlot(int slotid, int rosterFromTime, int rosterToTime, DateTime appointmentDate, string referenceId, int Avfecount,List<FEList> fELists, int managerUserId, string managerName)
        {
            return (new DLWFMSlot()).GetSlot(slotid, rosterFromTime, rosterToTime, appointmentDate, referenceId, Avfecount, fELists,managerUserId, managerName);
        }

        public static Slot GetSlotDetailById(int slotid)
        {
            return (new DLWFMSlot()).GetSlotDetailById(slotid);
        }

        public static int InsertSlotConfimation(SlotConfirmation obj)
        {
            return (new DLWFMSlotConfirmation().InsertSlotConfirmation(obj));

        }
        public static int UpdateSlotConfirmation(SlotConfirmation obj)
        {
            return (new DLWFMSlotConfirmation().UpdateSlotConfirmation(obj));
        }


        public static int GetFreeSlot(string slotid, int managerUserId)
        {
            return (new DLWFMSlotConfirmation().GetFreeSlot(slotid, managerUserId));

        }

        public static List<VMSlotConfirmation> GetAllFreeSlot(int managerUserId, DateTime appointment_date)
        {
            return (new DLWFMVMSConfirmation().GetAllFreeSlot(managerUserId, appointment_date));

        }

        public static void GetValue(string slotid, out DateTime appointment_date, out int master_slot_id)
        {
            slotid = slotid.Split('_')[0];
            string year = slotid.Substring(0, 4);
            string month = slotid.Substring(4, 2);
            string day = slotid.Substring(6, 2);

            master_slot_id = slotid.Length ==10 ? Convert.ToInt32(slotid.Substring(8, 2)) : Convert.ToInt32(slotid.Substring(8, 1));
            appointment_date = Convert.ToDateTime(year + "-" + month + "-" + day);

        }

        public static SlotConfirmation GetSlotConfirmation(int bookingId)
        {
            return (new DLWFMSlotConfirmation().GetSlotConfirmation(bookingId));

        }

        public static bool InsertHpsm_TicketMasterData(Models.WFM.Task objTicketTask)
        {
            return (new DLWFMTask().InsertHpsm_TicketMasterData(objTicketTask));
        }
        public static bool updateHpsm_TicketMasterData(Models.WFM.Task objTicketTask)
        {
            return (new DLWFMTask().updateHpsm_TicketMasterData(objTicketTask));
        }

        public int priority(string jobId)
        {
            return (new DLWFMTask().priority(jobId));
        }

        public static bool SaveRouteIssue(Route_Issue objRouteIssue)
        {

            return (new DLWFMRouteIssue().SaveRouteIssue(objRouteIssue));
        }

        public static Route_Issue GetRoute_Issue(int hpsmTicketId)
        {
            return (new DLWFMRouteIssue().GetRoute_Issue(hpsmTicketId));
        }
        public static bool SavePaymentDetails(Payment_Details objPaymentDetails)
        {

            return new DLWFMPaymentDetails().SavePaymentDetails(objPaymentDetails);
        }

        public static Route_Issue GetHpsmidByRouteIssuesId(int issueid)
        {
            return (new DLWFMRouteIssue().GetHpsmidByRouteIssuesId(issueid));
        }

        public static bool DeleteTaskTrackingByIssueId(int issue_id, int user_id)
        {
            return (new DLWFMTask().DeleteTaskTrackingByIssueId(issue_id, user_id));
        }

        public static int cancleJobOrder(int hpsmTicketId)
        {
            return (new DLWFMRouteIssue().cancleJobOrder(hpsmTicketId));
        }

        public static bool AssignRouteIssue(ViewManagerRouteIssueApprove objSelf, out List<Vw_Hpsm_Ticket_Status> HpsmStatusList)
        {
            return DLWFMRouteIssue.AssignRouteIssue(objSelf, out HpsmStatusList);
        }

        public static Models.WFM.Task Getjobdetails(string hpsmid)
        {

            return DLHPSMTicket.Getjobdetails(hpsmid);

        }

        public static int newEditTimeSheet(Models.WFM.Task obj)
        {
            return (new DLWFMJobOrderDetail()).newEditTimeSheet(obj);
        }


        public static bool AssingContractorRouteIssue(ViewManagerRouteIssueApprove objSelf)
        {
            return DLWFMRouteIssue.AssingContractorRouteIssue(objSelf);
        }
        public static bool AssignRouteIssueTKT(ViewManagerRouteIssueApprove objSelf, out List<Vw_Hpsm_Ticket_Status> HpsmStatusList)
        {
            return DLWFMRouteIssue.AssignRouteIssueTKT(objSelf, out HpsmStatusList);
        }

        public static bool isJobIdExist(string jobId)
        {
            return DLWFMTask.isJobIdExist(jobId);
        }
        public static int isJobIdExistCount(string jobId)
        {
            return DLWFMTask.isJobIdExistCount(jobId);
        }
        public static List<Payment_Details> GetPaymentDetailsByJobOrder(string job_id)
        {
            return DLWFMTask.GetPaymentDetailsByJobOrder(job_id);
        }

        public static Models.WFM.Task GetJobDetailByJobOrderId(string jobOrderId)
        {
            return DLWFMTask.GetJobDetailByJobOrderId(jobOrderId);
        }
        public static Models.WFM.Task GetJobDetailByJobOrderIdAtomicId(string jobOrderId, string atomicOrderId)
        {
            return DLWFMTask.GetJobDetailByJobOrderIdAtomicId(jobOrderId, atomicOrderId);
        }
        public static Models.WFM.Task GetTTJobDetailByJobOrderId(string jobOrderId)
        {
            return DLWFMTask.GetTTJobDetailByJobOrderId(jobOrderId);
        }
        

        public static bool isbookingIdExist(int bookingId)
        {
            return DLWFMTask.isbookingIdExist(bookingId);
        }
        public static int getrecordbyBookingId(int bookingId)
        {
            return DLWFMTask.getrecordbyBookingId(bookingId);
        }
       

        public static AppointmentDetail GetAppointmentDetail(string referenceId)
        {
            return (new DLWFMSlotConfirmation().GetAppointmentDetail(referenceId));

        }

        public static AppointmentDetail GetAppointmentDetails(string referenceId)
        {
            return (new DLWFMSlotConfirmation().GetAppointmentDetails(referenceId));
        }


        public static SlotConfirmation GetSlotConBySlotRefId(string slotid, string referenceid)
        {
            return (new DLWFMSlotConfirmation().GetSlotConBySlotRefId(slotid, referenceid));

        }
        public static SlotConfirmation GetSlotConfirmationByRefId(string referenceid,int iscanceled)
        {
            return (new DLWFMSlotConfirmation().GetSlotConfirmationByRefId(referenceid, iscanceled));
        }
        public static int cancleAppointmentSlot(int bookingid)
        {
            return (new DLWFMSlotConfirmation().cancleAppointmentSlot(bookingid));

        }

        public static List<AssignedTaskDetail> GetFRTTaskDetails(int userId, string toDate, string fromDate, string status, string jobid)
        {
            return DLWFMTask.GetFRTTaskDetails(userId, toDate, fromDate, status, jobid);
        }

        public static List<AssignedTaskDetailTT> GetTTFRTTaskDetails(int userId, string toDate, string fromDate, string status, string jobid)
        {
            return DLWFMTask.GetTTFRTTaskDetails(userId, toDate, fromDate, status, jobid);
        }
        public static int UpdateJobOrderDetail(JobOrderDetail obj)
        {
            return (new DLWFMJobOrderDetail()).UpdateJobOrderDetail(obj);
        }
        public static int UpdateSendToERP(Models.WFM.Task obj)
        {
            return (new DLWFMJobOrderDetail()).UpdateSendToERP(obj);
        }

        public static int UpdatettStatus(getttStatusIn obj)
        {
            return (new DLWFMJobOrderDetail()).UpdatettStatus(obj);
        }

        public static int updateCpeDetailFromXml(string job_id, string refSerial, string itemCode, string uom, string wh)
        {
            return (new DLWFMJobOrderDetail()).updateCpeDetailFromXml(job_id, refSerial, itemCode, uom, wh);
        }
        public static int UpdateJobOrderStage(string job_id, int stage, string sub_status=null,string remark=null)
        {
            return (new DLWFMJobOrderDetail()).UpdateJobOrderStage(job_id, stage, sub_status, remark);
        }
        public static int DeleteAttachementUpdateStage(string job_id, int stage)
        {
            return (new DLWFMJobOrderDetail()).DeleteAttachementUpdateStage(job_id, stage);
        }

        public static customer_detail getCustomerDetail(string jobId)
        {
            return (new DLWFMJobOrderDetail()).getCustomerDetail(jobId);
        }
        public static cpe_detail getCPEDetail(string jobId)
        {
            return (new DLWFMJobOrderDetail()).getCPEDetail(jobId);
        }
        public static int UpdateStatus(job_order_status obj)
        {
            return (new DLWFMJobOrderDetail()).UpdateStatus(obj);
        }
    
        public static List<ManagerList> GetAllManagerList(double lat, double lng, string role_name = null)
        {
            return DLWFMTicket.GetAllManagerList(lat, lng, role_name);

        }
        public static int UpdateReTrigger(job_order_status obj)
        {
            return (new DLWFMJobOrderDetail()).UpdateReTrigger(obj);
        }
        public static int UpdateStaticIPStatus(string hpsmid, string remark)
        {
            return (new DLWFMJobOrderDetail()).UpdateStaticIPStatus(hpsmid, remark);
        }
        public static int UpdateCpeCollected(string hpsmid,string is_cpe_collected)
        {
            return new DLWFMJobOrderDetail().UpdateCpeCollected(hpsmid,is_cpe_collected);
        }
        public static int UpdatePaymentDetail(string hpsmid, string ar_no, string payment_mode, string payment_status)
        {
            return new DLWFMJobOrderDetail().UpdatePaymentDetail(hpsmid,ar_no,payment_mode, payment_status);
        }
        public static string UpdateStatus_rch(job_order_status obj)
        {
            return (new DLWFMJobOrderDetail()).UpdateStatus_rch(obj);
        }
        public static int Checkin(checkin obj)
        {
            return (new DLWFMJobOrderDetail()).Checkin(obj);
        }
        //public static List<hpsm_ticket_attachments> getAttachmentDetailsbyJobId(string job_id, string upload_type, string screen)
        //{
        //    return new DLHPSMTICKETATTACHMENTS().getAttachmentDetailsbyJobId(job_id, upload_type, screen);
        //}

        public static List<vw_hpsm_ticket_attachments> getAttachmentDetailsbyJobId(string job_id, string upload_type, string screen)
        {
            return new DLHPSMTICKETATTACHMENTSView().getAttachmentDetailsbyJobId(job_id, upload_type, screen);
        }
        public static List<vw_hpsm_ticket_attachments> getAttachmentDetailsbyJobId(string job_id)
        {
            return new DLHPSMTICKETATTACHMENTSView().getAttachmentDetailsbyJobId(job_id);
        }

        public static hpsm_ticket_attachments SaveTicketAttachment(hpsm_ticket_attachments objAttachment)
        {
            return new DLHPSMTICKETATTACHMENTS().SaveTicketAttachment(objAttachment);
        }
        public static hpsm_ticket_attachments getAttachmentDetailsbyId(int DocumentId)
        {
            return new DLHPSMTICKETATTACHMENTS().getAttachmentDetailsbyId(DocumentId);
        }
        public static int DeleteAttachmentById(int DocumentId)
        {
            return new DLHPSMTICKETATTACHMENTS().DeleteAttachmentById(DocumentId);
        }
        public static nmsticket GetNMSTicket_Details(string ticketId)
        {
            return DLHPSMTicket.GetNMSTicket_Details(ticketId);
        }
        public static Models.WFM.Task GetHPSMTicket_Details(string ticketId)
        {
            return DLHPSMTicket.GetHPSMTicket_Detail(ticketId);
        }

        public static Models.WFM.Task GetHPSMTicket_DetailById(int ticketId)
        {
            return DLHPSMTicket.GetHPSMTicket_DetailById(ticketId);
        }
        
        public static List<VSF_TICKET_HISTORY> GetVSFTicketDetails(int ticketId)
        {
            return DLHPSMTicket.GetVSFTicketDetails(ticketId);
        }
        public static List<Models.WFM.ViewMaterialDetail> ViewMaterialDetail(string jobid)
        {
            return DLHPSMTicket.ViewMaterialDetail(jobid);
        }
        public static void SaveNotificationMessageInHistory(NOTIFICATION_ALERTS_HISTORY objNotification)
        {
            DLHPSMTicket.SaveNotificationMessageInHistory(objNotification);
        }
        public static bool UpdateRouteIssueStatus(ViewManagerRouteIssueApprove objSelf)
        {
            return DLHPSMTicket.UpdateRouteIssueStatus(objSelf);
        }
        public static List<Models.ticketStepDetails> getTicketStepDetails(string job_id)
        {
            return (new DLWFMTicket().getTicketStepDetails(job_id));
        }

        public static List<Models.ticketStepDetails> getTTTicketStepDetails(string job_id)
        {
            return (new DLWFMTicket().getTTTicketStepDetails(job_id));
        }

        public static List<AdditionalMaterialMaster> GetAdditionalMaterialMaster(string ticket_type = null)
        {
            return (new DLWFMAddMaterilaMaster().GetAdditionalMaterialMaster(ticket_type));
        }
        public static AdditionalMaterialMaster GetAdditionalMaterialMaster(int material_id)
        {
            return new DLWFMAddMaterilaMaster().GetAdditionalMaterialMasterById(material_id);
        }

        public static List<job_status_type_rca> GetJobOrderstatus(string action, int ticket_source)
        {
            return (new DLwfm_jobstatus().GetJobOrderstatus(action, ticket_source));
        }

        public static List<job_status_type_rca> GetTTRcRca()
        {
            return (new DLWFMTTRC().GetTTRcRca());
        }

        public static List<AdditionalMaterial> GetAdditionalMaterial(string jobid)
        {
            return (new DLWFMAddMaterial().GetAdditionalMaterial(jobid));
        }
        public static int SaveAdditionalMaterial(string jobid, List<AdditionalMaterial> obj)
        {
            return (new DLWFMAddMaterial().SaveAdditionalMaterial(jobid, obj));

        }
        public static dynamic getTriggerActivateDetails(string jobId, out string facility)
        {
            return (new DLWFMJobOrderDetail()).getTriggerActivateDetails(jobId, out facility);
        }
        public static dynamic CPEReplace(string jobId)
        {
            return (new DLWFMJobOrderDetail()).CPEReplace(jobId);
        }
        public static dynamic GetReTriggerRequest(string jobId)
        {
            return (new DLWFMJobOrderDetail()).GetReTriggerRequest(jobId);
        }
        public static JoCategoryRoleMapping GetRoleNameByJoCategory(string jo_category)
        {
            return (new DLJoCatRoleMapping()).GetRoleNameByJoCategory(jo_category);

        }
        public static JoCategoryRoleMapping GetRoleNameByJoCategory(string jo_category,string joType)
        {
            return (new DLJoCatRoleMapping()).GetRoleNameByJoCategory(jo_category, joType);
        }
        public static Issue_Type_Master GetOrderType(string orderType)
        {
            return (new DLWFMOrderType()).GetOrderType(orderType);
        }

        public static wfm_service_facility_master GetServiceFacility(string facility_name)
        {
            return (new DLWFMServiceFacilityMaster()).GetServiceFacility(facility_name);
        }
        public static List<wfm_service_facility_master> GetMainServiceFacility()
        {
            return (new DLWFMServiceFacilityMaster()).GetMainServiceFacility();
        }
        

        public static tbl_wfm_slot_duration GetSlotDurationDetails(int slot_duration)
        {
            return (new DLWFMSlotDuration()).GetSlotDurationDetails(slot_duration);
        }

        public static wfm_notification_template GetTemplateDetail(string type)
        {
            return (new DLWFMNotification()).GetTemplateDetail(type);
        }
        public static wfm_email_sms_log GetbyJobOrderId(string joborderid,string type)
        {
            return (new DLWFMEmailsmsLog()).GetbyJobOrderId(joborderid, type);
        }
        public static wfm_jo_type_master GetJoType(string jo_name)
        {
            return (new DLWFMJotypeMaster()).GetJoType(jo_name);
        }

        public static List<VW_HPSM_Ticket_Master_History> GetHPSMTicket_Detail_Status_History(string ticketId)
        {
            return DLHPSMTicket.GetHPSMTicket_Detail_Status_History(ticketId);
        }

        public static getStatusDetail GetStatusDetailByJobOrderId(getDetailIn obj)
        {
            return DLHPSMTicket.GetStatusDetailByJobOrderId(obj);
        }
        public static List<FRTCapacity> GetTaskTrackingDetail(int? userId)
        {
            return DLHPSMTicket.GetTaskTrackingDetail(userId);
        }
        public static List<FRTCapacity> GetDispatcherCountDetail(int ?userId, DateTime appointment_date)
        {
            return DLHPSMTicket.GetDispatcherCountDetail(userId, appointment_date);
        }
        public static List<FRTCapacity> GetSupervisiorCountDetail(int? userId)
        {
            return DLHPSMTicket.GetSupervisiorCountDetail(userId);
        }
        
        public static RCADetailByJobId getTTRCAbyJobId(string job_id)
        {
            return DLHPSMTicket.getTTRCAbyJobId(job_id);
        }

        public static int GetStepOrder()
        {
            return (new DLHPSMTicket()).GetStepOrder();
        }

        public static int saveConnectedDeviceRequest(ConnectedDeviceRequest connectedDeviceRequest)
        {
            return DLHPSMTicket.saveConnectedDeviceRequest(connectedDeviceRequest);
        }

        public static List<ConnectedDevice> GetConnectedDevice(ConnectedDeviceDetail obj)
        {
            return DLHPSMTicket.GetConnectedDevice(obj);
        }
        public static List<WfmRca> GetRcadetail(rcaIn obj)
        {
            return DLHPSMTicket.GetRcadetail(obj);
        }
    

        public static Models.WFM.ConnectedDeviceRequest ConnectedDeviceDetailByRequestId(string requestId)
        {
            return DLHPSMTicket.ConnectedDeviceDetailByRequestId(requestId);
        }
        public static int updateConnectedDevice(Models.WFM.Root root)
        {
            return DLHPSMTicket.updateConnectedDevice(root);
        }
        public static dynamic getTriggerFinalDetails(string jobId)
        {
            return (new DLWFMJobOrderDetail()).getTriggerFinalDetails(jobId);
        }
        public static Route_Issue nEditTimeSheet(int issueId)
        {
            return (new DLWFMJobOrderDetail()).nEditTimeSheet(issueId);
        }
        public static Models.WFM.Task GetjobdetailsbyId(int? hpsmticketid)
        {

            return DLHPSMTicket.GetjobdetailsbyId(hpsmticketid);

        }
        public static List<IssueType> GetIssueType()
        {
            return new List<IssueType>()
            {
                new IssueType { Key="ABS", Value="ASSIGN BACK TO SOURCE" },
                new IssueType {Key="N_ISSUE", Value= "NORMAL ISSUE" },
                new IssueType {Key ="CPE_REP",Value="CPE REPLACEMENT REQUIRED" }
            };
            //return DLWFMTask.GetFRTTaskDetails(userId, toDate, fromDate, status, jobid);
        }
        public static jo_form GetJo_form(string jobid)
        {
            DateTime? checkin, checkout;
            User u;
            Models.WFM.Task t = DLHPSMTicket.Getjobdetails(jobid);
            DLWFMTicket.GetTaskTracking(t.hpsm_ticket_id, out checkout, out checkin, out u);
            string countryname = DLWFMTicket.GetCountryName(t.stateorprovince);
            jo_form jf = new jo_form
            {
                jo_date = t.created_on.Value.ToString("MMM. dd, yyyy"),
                assigned_name = u.name,
                account_number = t.account_number,
                customer_name = t.subscriber_name,
                address = (!string.IsNullOrWhiteSpace(t.addressline1) ? t.addressline1 + "," : "") +
                (!string.IsNullOrWhiteSpace(t.addressline2) ? t.addressline2 + "," : "") +
                (!string.IsNullOrWhiteSpace(t.addressline3) ? t.addressline3 + "," : "") +
                (!string.IsNullOrWhiteSpace(t.addressline4) ? t.addressline4 + "," : "") +
                (!string.IsNullOrWhiteSpace(t.addressline5) ? t.addressline5 + "," : "") +
                (!string.IsNullOrWhiteSpace(t.city) ? t.city + "," : "") +
                (!string.IsNullOrWhiteSpace(t.stateorprovince) ? t.stateorprovince + "," : "") +
                (!string.IsNullOrWhiteSpace(countryname) ? countryname + "," : "") +
                (!string.IsNullOrWhiteSpace(t.pincode) ? t.pincode : ""),
                customer_rmn = t.customer_rmn,
                package = t.package_name,
                napcode = t.node,
                napport = t.nap_port,
                cpe_serial = t.cpe_serialno,
                ref_serial = t.cpe_ref_serial,
                hpsmid = t.hpsmid,
                origin = t.tasktype,
                slot = DLWFMTicket.GetSchedule(t.hpsm_ticket_id.ToString()),
                checkin = checkin != null ? checkin.Value.ToString("MMM-dd-yyyy HH:mm:ss") : "-",
                checkout = checkout != null ? checkout.Value.ToString("MMM-dd-yyyy HH:mm:ss") : "-",
                material_used = DLWFMTicket.GetMaterialUsed(jobid),
                remarks = t.remarks
            };
            return jf;
        }

        public static int UpdateNotificationLog(napportnotificationdata obj)
        {
            return new DLWFMUpdateNotificationLog().UpdateNotificationLog(obj);
        }

        public static PortManager GetPortManager(string stateorprovince)
        {
            return DLWFMTicket.getPortManager(stateorprovince);
        }
        public static bool UpdateNapDetails(napdetails obj)
        {
            return new DLWFMTicket().UpdateNapDetails(obj);
        }


    }

    public class BLIssues
    {
        public static Issue_Resolution_Type_Master GetIssueResolutonTypeByResolutionCode(string strResolutionCode)
        {
            return DLIssues.GetIssueResolutonTypeByResolutionCode(strResolutionCode);
        }
        public static int IssueCircleCount(string IssueIds, out int Circle_id)
        {
            return DLIssues.IssueCircleCount(IssueIds, out Circle_id);
        }
        public static List<VW_ROUTE_ASSIGNED_ISSUES> GetAssignIssueRouteDetails(ViewAssignJobOrderFilter viewIssueRouteFilter, out int totalRecords)
        {
            return DLIssues.GetAssignIssueRouteDetails(viewIssueRouteFilter, out totalRecords);
        }

       

    }
  



}
