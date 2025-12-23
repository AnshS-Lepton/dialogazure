using BusinessLogics;
using Lepton.Entities;
using Models;
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
    [RoutePrefix("api/NetworkTicket")]
    [CustomAuthorization]
    [APIExceptionFilter]
    [CustomAction]
    public class NetworkTicketController : ApiController
    {
        [HttpPost]
        //public ApiResponse<List<NetworkTicket>> GetAllNetworkTicket(ReqInput data)
        public ApiResponse<NWTicket_List_Status> GetAllNetworkTicket(ReqInput data)
        {
            //var response = new ApiResponse<List<NetworkTicket>>();
            var response = new ApiResponse<NWTicket_List_Status>();
            try
            {
                NetworkTicketFilter objRequestIn = ReqHelper.GetRequestData<NetworkTicketFilter>(data);
                //List<NetworkTicket> obj = new BLNetworkTicket().GetAPINetworkTicket(objRequestIn);
                var obj = new BLNetworkTicket().GetAPINetworkTicket(objRequestIn);
                obj.lstNWDetails = obj.lstNWDetails == null ? new List<NetworkTicketList>() : obj.lstNWDetails;
                obj.lstNWStatus = obj.lstNWStatus == null ? new List<NetworkTicketStatus>() : obj.lstNWStatus;
                obj.lstNWTypes = obj.lstNWTypes == null ? new List<NetworkTicketType>() : obj.lstNWTypes;

                response.results = obj;
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNetworkTicket()", "NetworkTicket Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Get NetworkTicket Details!";
            }
            return response;
        }
        [HttpPost]
        public ApiResponse<List<TicketTypeMaster>> GetTicketType(ReqInput data)
        {
            var response = new ApiResponse<List<TicketTypeMaster>>();
            try
            {
                int user_id = 0;
                TicketTypeMaster objRequestIn = ReqHelper.GetRequestData<TicketTypeMaster>(data);
                List<TicketTypeMaster> obj = new BLNetworkTicket().GetTicketTypeByModule(objRequestIn.module,user_id);
                response.results = obj;
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetTicketType()", "NetworkTicket Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Get NetworkTicket Details!";
            }
            return response;
        }
        [HttpPost]
        public ApiResponse<NWTAcknowledgement> UpdateAcknowledgement(ReqInput data)
        {
            var response = new ApiResponse<NWTAcknowledgement>();
            if (ModelState.IsValid)
            {    
                try
                {
                    NWTAcknowledgement ack = ReqHelper.GetRequestData<NWTAcknowledgement>(data);
                    var resultNW = new BLNetworkTicket().UpdateAcknowledgement(ack);
                    if (resultNW.status)
                    {
                        response.error_message = resultNW.message;
                        response.status = ResponseStatus.OK.ToString();
                    }
                    else
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.error_message = resultNW.message;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("GetTicketType()", "NetworkTicket Controller", data.data, ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Get NetworkTicket Details!";
                }
            }
            else 
            {
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Get NetworkTicket Details!";
            }
            return response;
        }

        [HttpPost]
       
        public ApiResponse<GetGeometryByTicketId> getNetworkTicketGeometry(ReqInput data)
        {
           
            var response = new ApiResponse<GetGeometryByTicketId>();
            try
            {
                NetworkTicketList obj = ReqHelper.GetRequestData<NetworkTicketList>(data);
                var geomobj = new BLNetworkTicket().getNetworkTicketGeometry(obj.ticket_id);
                response.results = geomobj;
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getNetworkTicketGeometry()", "NetworkTicket Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Get NetworkTicket Geometry!";
            }
            return response;
        }
        [HttpPost]
        public ApiResponse<NWTktEntityLst> GetNetworkTicketEntityList(ReqInput data)
        {
            var  response = new ApiResponse<NWTktEntityLst>();
            try
            {
                NetworkTicketEntityListParam objparam = ReqHelper.GetRequestData<NetworkTicketEntityListParam>(data);
                var obj = new BLNetworkTicket().GetNetworkTicketEntityList(objparam);
                obj.lstNWEntityDetails = obj.lstNWEntityDetails == null ? new List<NetworkTicketEntityList>() : obj.lstNWEntityDetails;
                obj.lstNWEntityStatus = obj.lstNWEntityStatus == null ? new List<NetworkTicketStatus>() : obj.lstNWEntityStatus;
                response.results = obj;
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNetworkTicketEntityList()", "NetworkTicket Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Get NetworkTicketEntity List!";
            }
            return response;
        }
    }
}
