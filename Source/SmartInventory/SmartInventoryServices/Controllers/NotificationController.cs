using BusinessLogics;
using Lepton.Entities;
using Models;
using SmartInventoryServices.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Utility;

namespace SmartInventoryServices.Controllers
{
    [Authorize]
    [APIExceptionFilter]
    [CustomAction]
    [RoutePrefix("api/Notification/v1.0")]
    public class NotificationController : ApiController
    {
        [Route("getNotificationHistory")]
        [HttpPost]
        public ApiResponse<dynamic> getNotificationHistory(NotificationRequest data)
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                NotificationHistoryResponse results = new NotificationHistoryResponse();
                results.Notifications = new BLNotification().GetNotificationHistory(data.userId);
                if (results.Notifications.Count > 0)
                {
                    response.results = results;
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.FAILED.ToString();
                    response.error_message = "Record not found.";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getNotificationHistory", "Notification Controller", "", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
    }
}