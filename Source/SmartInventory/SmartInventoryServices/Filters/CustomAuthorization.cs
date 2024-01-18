using BusinessLogics;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SmartInventoryServices.Filters
{
    public class CustomAuthorization : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string Languge = "";
            if (AuthorizeRequest(actionContext))
            {
                if (actionContext.Request.Headers.Contains("Languge"))
                {
                    Languge = actionContext.Request.Headers.GetValues("Languge").First();
                }
                Resources.Helper.SetApplicationLanguge(Languge);

                return;
            }
            HandleUnauthorizedRequest(actionContext);
        }
        protected void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //Code to handle unauthorized request

            //actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "Authorization has been denied for this request.");
            //actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
        }
        private bool AuthorizeRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {

            //var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            //if (principal.Claims.Count() > 0)
            //{
            //   var claimList=principal.Claims.ToList();
            //    if (principal.Claims.FirstOrDefault().Value == "OTP")
            //    {
            //        actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "Authorization has been denied for this request.");
            //        return false;
            //    }
            //}

            var appVersion = String.Empty;
            var iso_version = string.Empty;
            //UserLogin objUserLogin = new UserLogin();
            //---GET ACCESS TOKEN and DECODE
            var access_token = actionContext.Request.Headers.Authorization.Parameter;// GET Access Token From Header Request
            var secureDataFormat = new Microsoft.Owin.Security.DataHandler.TicketDataFormat(new MachineKeyProtector());
            Microsoft.Owin.Security.AuthenticationTicket ticket = secureDataFormat.Unprotect(access_token);


            if (ticket.Properties.Dictionary.ContainsKey("OTP"))
            {
                if (ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "OTP").Value.ToString() == "OTP")
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "Authorization has been denied for this request.");
                    return false;
                }
            }
            //-- GET ACCESS TOKEN PARAMATERS
            var userName = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userName").Value;
            var UserId = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userId").Value;
            var tokenExpiresIn = Convert.ToDateTime(ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == ".expires").Value);
            //user = objDAUser.ValidateUser(userName, ticket.ToString());
            var tokenAppVersion = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "appVersion").Value;
            var tokenLoginHistoryId = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "loginHistoryId").Value;
            var tokenSourceName = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "source").Value;
            var tokenOsName = ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "osName").Value;
			//--------------Below code for get user id from request-------------
			var requestUserId = "";
			dynamic requestData = "";
			string request = actionContext.Request.Content.ReadAsStringAsync().Result;
			if (!string.IsNullOrEmpty(request))
			{
				request = request.ToLower();
				JObject Obj = JObject.Parse(request);
				var innerdata = (string)Obj["data"];
				if (!string.IsNullOrEmpty(innerdata))
				{
					JToken token = JToken.Parse(innerdata);
					if (token is JArray)
					{
						List<JObject> jsonObjectList = JsonConvert.DeserializeObject<List<JObject>>(innerdata);
						if (jsonObjectList.Count > 0)
						{
							requestData = (jsonObjectList[0]);
							requestUserId = Convert.ToString(requestData.user_id);
							if (requestUserId == null)
							{
								requestUserId = Convert.ToString(requestData.userid);
							}
						}
					}
					else if (token is JObject)
					{
						requestData = JObject.Parse((string)Obj["data"]);
						requestUserId = Convert.ToString(requestData.user_id);
						if (requestUserId == null)
						{
							requestUserId = Convert.ToString(requestData.userid);
						}
					}
				}
			}
			//-----------------------------------
			// VALIDATIONS
			if (tokenSourceName.ToUpper() == "WEB" || tokenSourceName.ToUpper() == "MOBILE")
            {
                GlobalSetting globalSettings = new BLGlobalSetting().GetMobileAppVersionByKey("appVersion");
                GlobalSetting globalSettings2 = new BLGlobalSetting().GetMobileAppVersionByKey("IOS_Version");

                var UserSourceName = new BLUserLogin().GetSourceName(Convert.ToInt32(UserId));
                int userLoginHistoryId = new BLUserLogin().GetUserLoginHistoryId(Convert.ToInt32(UserId), tokenSourceName);
                if (globalSettings != null)
                {
                    appVersion = globalSettings.value;
                    // appVersion = globalSettings.FirstOrDefault(x => x.key == "appVersion").value;
                }
                if (globalSettings2 != null)
                {
                    iso_version = globalSettings2.value;
                    // appVersion = globalSettings.FirstOrDefault(x => x.key == "appVersion").value;
                }
                if (DateTimeHelper.Now > tokenExpiresIn)
                {
                    // Update logout Time
                    var result = new BLUserLoginHistoryInfo().UpdateUserLogOutTime(Convert.ToInt32(UserId), userLoginHistoryId);
                    actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "Authorization has been denied for this request.");
                    return false;
                }
                else if (!string.IsNullOrEmpty(tokenAppVersion) && !string.IsNullOrEmpty(tokenOsName) && appVersion != tokenAppVersion && tokenOsName.ToUpper() == "ANDROID" && tokenSourceName.ToUpper() == "MOBILE")
                {
                    var result = new BLUserLoginHistoryInfo().UpdateUserLogOutTime(Convert.ToInt32(UserId), userLoginHistoryId);
                    actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "App Version updated.");
                    return false;
                }
                else if (!string.IsNullOrEmpty(tokenAppVersion) && !string.IsNullOrEmpty(tokenOsName) && iso_version != tokenAppVersion && tokenOsName.ToUpper() != "ANDROID" && tokenSourceName.ToUpper() == "MOBILE")
                {
                    var result = new BLUserLoginHistoryInfo().UpdateUserLogOutTime(Convert.ToInt32(UserId), userLoginHistoryId);
                    actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "App Version updated.");
                    return false;
                }
                else if (userLoginHistoryId != Convert.ToInt32(tokenLoginHistoryId) && tokenSourceName.ToUpper() == "MOBILE")
                {
                    var result = new BLUserLoginHistoryInfo().UpdateUserLogOutTime(Convert.ToInt32(UserId), userLoginHistoryId);
                    actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "User is already logged-In on other device.");
                    return false;
                    // }
                }
				else if (tokenSourceName.ToUpper() == "MOBILE" && !String.IsNullOrEmpty(requestUserId) && requestUserId != "0")
				{
					if (Convert.ToInt32(requestUserId) != Convert.ToInt32(UserId))
					{
						var result = new BLUserLoginHistoryInfo().UpdateUserLogOutTime(Convert.ToInt32(UserId), userLoginHistoryId);
						actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "Invalid User.");
						return false;
					}
				}
			}
            else
            {
                var consumerDetails = new BLAPIConsumerMaster().ValidateConsumer(userName, "", tokenSourceName);
                if (DateTimeHelper.Now > tokenExpiresIn)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "Authorization has been denied for this request.");
                    return false;
                }
                else if (!consumerDetails.is_active)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, "User not active.");
                    return false;
                }
            }

            return true; ;
        }

        /// <summary>
        /// Helper method to decrypt the OWIN ticket
        /// </summary>
        public class MachineKeyProtector : IDataProtector
        {
            private readonly string[] _purpose =
            {
        typeof(OAuthAuthorizationServerMiddleware).Namespace,
        "Access_Token",
        "v1"
    };

            public byte[] Protect(byte[] userData)
            {
                throw new NotImplementedException();
            }

            public byte[] Unprotect(byte[] protectedData)
            {
                return System.Web.Security.MachineKey.Unprotect(protectedData, _purpose);
            }
        }
    }
}
