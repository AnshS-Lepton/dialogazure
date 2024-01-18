using BusinessLogics;
using Models;
using Models.API;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Utility;

namespace SmartInventoryServices.Controllers
{
    //[Authorize]
    [CustomAuthorization]
    [APIExceptionFilter]
    public class CRMController : ApiController
    {
        #region GET USER TICKETS
        [HttpPost]
        public ApiResponse<List<CrmTicketInfo>> GetUserTickets(ReqInput data)
        {
            var response = new ApiResponse<List<CrmTicketInfo>>();
            // crmticket obj
            CrmTicketIn objCrmTicketIn = ReqHelper.GetRequestData<CrmTicketIn>(data);
            try
            {
                List<CrmTicketInfo> lstCrmTicketInfo = new BLMisc().getTicketInfo(objCrmTicketIn.user_id);
                response.results = lstCrmTicketInfo;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getCrmTickets()", "CRMController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion


        #region GET Ticket Steps Detail
        [HttpPost]
        public ApiResponse<List<ticketStepDetails>> getTicketStepsDetail(ReqInput data)
        {
            var response = new ApiResponse<List<ticketStepDetails>>();

            ticketStepsDetailIn objTicketStepsDetailIn = ReqHelper.GetRequestData<ticketStepsDetailIn>(data);
            try
            {
                List<ticketStepDetails> lstTicketSteps = new BLMisc().getTicketStepDetails(objTicketStepsDetailIn.ticket_id, objTicketStepsDetailIn.ticket_type_id, objTicketStepsDetailIn.building_rfs_type);
                response.results = lstTicketSteps;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getTicketStepsDetail()", "CRMController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        #endregion

        #region--COPY
        [HttpPost]
        public ApiResponse<List<CrmTicketInfo>> getCrmTickets(ReqInput data)
        {
            var response = new ApiResponse<List<CrmTicketInfo>>();    
            // crmticket obj
            CrmTicketIn objCrmTicketIn = ReqHelper.GetRequestData<CrmTicketIn>(data);
            try
            {
                List<CrmTicketInfo> lstCrmTicketInfo = new BLMisc().getTicketInfo(objCrmTicketIn.user_id);
                response.results = lstCrmTicketInfo;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getCrmTickets()", "CRMController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        [HttpPost]
        public ApiResponse<StaticPageMasterViewModel> getStaticPageDetails(ReqInput data)
        {
            var response = new ApiResponse<StaticPageMasterViewModel>();
            // crmticket obj
            StaticPageMasterViewModel objStaticPageMaster = new StaticPageMasterViewModel();
            StaticPageMasterIn objStaticPageMasterIn = ReqHelper.GetRequestData<StaticPageMasterIn>(data); 
            try
            { 
                var result = new BLMisc().getStaticPageDetails(objStaticPageMasterIn.name);
                if(result!=null)
                {  
                    objStaticPageMaster.lstStaticPageMasterInfo = result;
                    response.status = StatusCodes.OK.ToString();
                    response.results = objStaticPageMaster;
                }
                else
                {
                    response.status = StatusCodes.ZERO_RESULTS.ToString();

                    response.error_message = "No Record Found";
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getStaticPageDetails()", "CRMController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

    }
}