using Microsoft.Owin.Security.OAuth;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using Utility;

namespace SmartInventoryServices.Helper
{
    public class ErrorLogHelper
    {
        public void GetUserInfo(ref int id, ref string userName)
        {
            try
            {
                ClaimsPrincipal icp = Thread.CurrentPrincipal as ClaimsPrincipal;
                // Access IClaimsIdentity which contains claims
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)icp.Identity;
                // Access claims
                foreach (Claim claim in claimsIdentity.Claims)
                {
                    if (claim.Type == "username") 
                        userName = claim.Value; 
                    else if (claim.Type == "userid")
                        id = Convert.ToInt32(claim.Value);
                }
            }
            catch
            {
                id = 0; 
                userName = "";
            }
        }
        public void ApiLogWriter(string methodName, string ControllerName, string requestData, Exception ex, int userId = 0)
        {

            int id = 0;
            string userName = "";
            ApiErrorLogInput objErrorLog = new ApiErrorLogInput();
            GetUserInfo(ref id, ref userName);
            objErrorLog.userId = id == 0 ? userId : id;  // in case token does not exists pass userId
            objErrorLog.UserName = userName;
            objErrorLog.fromPage = ControllerName;
            objErrorLog.fromMethod = methodName;
            objErrorLog.requestData = requestData;
            objErrorLog.exception = ex;
            ApiLogHelper.GetInstance.WriteErrorLog(objErrorLog);
        }
    }
}