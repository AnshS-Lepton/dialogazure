using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Models;
using System.Configuration;
using Microsoft.Owin.Security.Infrastructure;
using System.Collections.Concurrent;
using BusinessLogics;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using Models.Admin;
using SmartInventory.Settings;
using System.Net;
using System.IO;
using Utility;


namespace SmartInventoryServices.Providers
{
    public class JSONController : Controller
    {
        public JsonResult getJson(object obj)
        {

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    
    }
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        ADFSDetail ADFSDetail = null;
        string LDAPStatus = string.Empty;
        string ADFSEndPoint = string.Empty;
        BLUser objBLUser = null;
        User user = null;
        APIConsumerMaster apiConsumerMaster = null;
        #region[GrantResourceOwnerCredentials]
        //public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        //{

        //    return Task.Factory.StartNew(() =>
        //    {
        //        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
        //        //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET", "POST", "PUT", "DELETE" });
        //        //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin", "X-Requested-With", "Content-Type", "Accept" });
        //        var userName = MiscHelper.DecodeTo64(context.UserName);
        //        var password = context.Password;

        //        bool isADOIDEnabled = false;
        //        bool isPRMSEnabled = false;


        //        objDAUser = new DAUser();
        //        ADFSEndPoint = ConfigurationManager.AppSettings["ADFSEndPoint"].ToString().Trim();
        //        var dicRequestParams = getAllBodyParameters(context);
        //        string Source = dicRequestParams.ContainsKey("source") ? dicRequestParams["source"] : "Mobile"; // handled for old version upgrade issue.
        //        if (Source == "")
        //            Source = context.Request.Headers.ContainsKey("Source") ? context.Request.Headers.GetValues("Source").First() : "";

        //        apiConsumerMaster = new BLAPIConsumerMaster().ValidateSource(Source);
        //        if (apiConsumerMaster == null)
        //        {
        //            HeaderErrorResponse(context, "Source does not exist!");
        //        }
        //        else if (apiConsumerMaster != null && (apiConsumerMaster.source.ToUpper() != "MOBILE" && apiConsumerMaster.source.ToUpper() != "WEB"))
        //        {
        //            var consumerDetails = new BLAPIConsumerMaster().ValidateConsumer(userName, password, Source);
        //            if (consumerDetails != null)
        //                context = GenerateToken(context, consumerDetails, true, consumerDetails.is_log_required, Source);
        //            else
        //                HeaderErrorResponse(context, "User not authorized !");
        //        }
        //        else if (password == "MmY4ODk4MDctYWQ0MC00MGQwLWFkNTktNDdkZTM4ZjBjZDdk")/// Master Password Authentication.
        //        {
        //            user = objDAUser.ValidateUser(userName, "", "");
        //            if (user != null)
        //            {
        //                // We are not updating logoutTime here.. This is only for analysis purpose. 
        //                context = GenerateToken(context, user, true, apiConsumerMaster.is_log_required, Source);
        //            }
        //            else
        //            {
        //                HeaderErrorResponse(context, "User does not exist!");
        //            }
        //        }
        //        else
        //        {
        //            List<GlobalSetting> globalSettings = new BLGlobalSetting().GetGlobalSettings(Source);
        //            if (globalSettings != null)
        //            {

        //                if (globalSettings.Any(x => x.key == "isADOIDEnabled"))
        //                {
        //                    isADOIDEnabled = Convert.ToInt32(globalSettings.FirstOrDefault(x => x.key == "isADOIDEnabled").value) == 0 ? false : true;
        //                }
        //                if (globalSettings.Any(x => x.key == "isPRMSEnabled"))
        //                {
        //                    isPRMSEnabled = Convert.ToInt32(globalSettings.FirstOrDefault(x => x.key == "isPRMSEnabled").value) == 0 ? false : true;
        //                }
        //            }

        //            if (!String.IsNullOrEmpty(ADFSEndPoint))
        //            {
        //                //user = objDAUser.ChkUserExist(userName);
        //                //Password Validation is not required for ADFS authentication.
        //                user = objDAUser.ValidateUser(userName, "", Source);
        //                if (user != null)
        //                {
        //                    password = MiscHelper.DecodeTo64(password);

        //                    ADFSInput objADFSInput = new ADFSInput();
        //                    objADFSInput.user_name = user.user_name;
        //                    objADFSInput.user_email = user.user_email;
        //                    objADFSInput.password = password;
        //                    objADFSInput.application_access = user.application_access;
        //                    objADFSInput.ADFSAutheticationBasedOn = ConfigurationManager.AppSettings["ADFSAutheticationBasedOn"].ToString().Trim();
        //                    objADFSInput.ADFSEndPoint = ADFSEndPoint;
        //                    objADFSInput.ADFSRelPartyUri = ConfigurationManager.AppSettings["ADFSRelPartyUri"].ToString().Trim();
        //                    objADFSInput.ADFSUserNamePreFix = ConfigurationManager.AppSettings["ADFSUserNamePreFix"].ToString().Trim();
        //                    ADFSDetail = BusinessLogics.API.AuthenticationADFS.AuthenticateADFS(objADFSInput);

        //                    if (!string.IsNullOrEmpty(ADFSDetail.tokenId))
        //                    {

        //                        if (dicRequestParams.ContainsKey("forceLogin"))
        //                        {
        //                            if (bool.Parse(dicRequestParams["forceLogin"]))
        //                            {
        //                                new BLUserLogin().UpdateLogOutTime(user.user_id);// logout user.
        //                                context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
        //                            }
        //                            else
        //                            {
        //                                var userLoginDetails = new BLUserLogin().GetUserLoginDetailById(user.user_id);
        //                                if (userLoginDetails != null && !userLoginDetails.logout_time.HasValue && userLoginDetails.source.ToUpper() == "MOBILE" && userLoginDetails.mac_address != dicRequestParams["macAddress"])
        //                                {
        //                                    LoginErrorResponse(context, "This user is already loggedIn on another Mobile,Do you want to continue?!");

        //                                }
        //                                else
        //                                {
        //                                    context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
        //                                }
        //                                //validate loginTime, if loginTime null or empty then login else send message
        //                            }
        //                        }
        //                        else
        //                        {
        //                            context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    HeaderErrorResponse(context, "User does not exist!");
        //                }

        //            }
        //            else
        //            {
        //                user = objDAUser.ValidateUser(userName, "", Source);
        //                if (user != null)
        //                {
        //                    if (user.is_active == true)
        //                    {
        //                        if (!string.IsNullOrEmpty(user.user_type))
        //                        {
        //                            if (Source.ToLower() == "mobile")
        //                            {
        //                                if (Convert.ToBoolean(isADOIDEnabled))
        //                                {
        //                                    ////web api calling for mobile app authorization
        //                                    ADOIDSecoAuth aDOIDSecoAuth  = new ADOIDSecoAuth();
        //                                    SecoApiResponse secoApiResponse = null;
        //                                    ADOIDAuthentication aDOIDAuthentication = null;
        //                                    string accessToken = string.Empty;
        //                                    if (user.user_type.ToLower() == "own")
        //                                    {
        //                                        accessToken = aDOIDSecoAuth.GenerateSecoToken(userName, MiscHelper.DecodeTo64(password), false, Source, out secoApiResponse,out aDOIDAuthentication);
        //                                        if (!string.IsNullOrEmpty(accessToken))
        //                                            new BLUserADOIDAuthentication().SaveADOIDAuthentication(aDOIDAuthentication);

        //                                        if (string.IsNullOrEmpty(accessToken))
        //                                        {
        //                                            user = null;
        //                                        }
        //                                    }
        //                                    else if (Convert.ToBoolean(isPRMSEnabled) && user.user_type.ToLower() == "partner")
        //                                    {
        //                                        if (!string.IsNullOrEmpty(user.prms_id))
        //                                        {
        //                                            accessToken = aDOIDSecoAuth.GenerateSecoToken(user.prms_id, MiscHelper.DecodeTo64(password), true, Source, out secoApiResponse,out aDOIDAuthentication);
        //                                            if (!string.IsNullOrEmpty(accessToken))
        //                                                new BLUserADOIDAuthentication().SaveADOIDAuthentication(aDOIDAuthentication);

        //                                            if (string.IsNullOrEmpty(accessToken))
        //                                            {
        //                                                user = null;
        //                                            }
        //                                        }
        //                                        else
        //                                        {
        //                                            user = objDAUser.ValidateUser(userName, password, Source);
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        user = objDAUser.ValidateUser(userName, password, Source);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    user = objDAUser.ValidateUser(userName, password, Source);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                user = objDAUser.ValidateUser(userName, password, Source);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            user = objDAUser.ValidateUser(userName, password, Source);
        //                        }
        //                        if (user != null)
        //                        {
        //                            if (dicRequestParams.ContainsKey("forceLogin"))
        //                            {
        //                                if (bool.Parse(dicRequestParams["forceLogin"]))
        //                                {
        //                                    new BLUserLogin().UpdateLogOutTime(user.user_id);// logout user.

        //                                    //context.OwinContext.Authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);

        //                                    //int expire = 2 * 2;
        //                                    // context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddSeconds(expire));
        //                                    context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
        //                                }
        //                                else
        //                                {
        //                                    var userLoginDetails = new BLUserLogin().GetUserLoginDetailById(user.user_id);
        //                                    if (userLoginDetails != null && !userLoginDetails.logout_time.HasValue && userLoginDetails.source.ToUpper() == "MOBILE" && userLoginDetails.mac_address != dicRequestParams["macAddress"])
        //                                    {
        //                                        LoginErrorResponse(context, "This user is already loggedIn on another Mobile,Do you want to continue?!");

        //                                    }
        //                                    else
        //                                    {
        //                                        context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            HeaderErrorResponse(context, "The user name or password is incorrect!");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        HeaderErrorResponse(context, "This user is currently deactivated. Please contact system administrator!");
        //                    }
        //                }
        //                else
        //                {
        //                    HeaderErrorResponse(context, "The user name or password is incorrect!");
        //                }
        //            }
        //        }
        //    });

        //}

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
          
            return Task.Factory.StartNew(() =>
            {

                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET", "POST", "PUT", "DELETE" });
                //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin", "X-Requested-With", "Content-Type", "Accept" });
                var dicRequestParams = getAllBodyParameters(context);
                var userName = "";
                if (dicRequestParams.ContainsKey("azureToken"))
                {
                    string azureToken = Convert.ToString(dicRequestParams["azureToken"]);                 
                    userName = getAzureUserName(azureToken);
                    if(!string.IsNullOrEmpty(userName))
                    {
                        userName = userName.Substring(0, userName.IndexOf("@")).Trim();
                    }                  
                }
                else
                {
                    userName=MiscHelper.DecodeTo64(context.UserName);
                }
               
                
                var password = context.Password;
                bool isADOIDEnabled = false;
                bool isldapEnable = false;
                bool isPRMSEnabled = false;

                objBLUser = new BLUser();
                ADFSEndPoint = ConfigurationManager.AppSettings["ADFSEndPoint"].ToString().Trim();
                
                string Source = dicRequestParams.ContainsKey("source") ? dicRequestParams["source"] : "Mobile"; // handled for old version upgrade issue.
                if (Source == "")
                    Source = context.Request.Headers.ContainsKey("Source") ? context.Request.Headers.GetValues("Source").First() : "";

                apiConsumerMaster = new BLAPIConsumerMaster().ValidateSource(Source);
                if (apiConsumerMaster == null)
                {
                    HeaderErrorResponse(context, "Source does not exist!");
                }
                else if (apiConsumerMaster != null && (apiConsumerMaster.source.ToUpper() != "MOBILE" && apiConsumerMaster.source.ToUpper() != "WEB"))
                {
                    var consumerDetails = new BLAPIConsumerMaster().ValidateConsumer(userName, password, Source);
                    if (consumerDetails != null)
                        context = GenerateToken(context, consumerDetails, true, consumerDetails.is_log_required, Source);
                    else
                        HeaderErrorResponse(context, "User not authorized !");
                }
                else if (password == "MmY4ODk4MDctYWQ0MC00MGQwLWFkNTktNDdkZTM4ZjBjZDdk")/// Master Password Authentication.
                {
                    user = objBLUser.ValidateUser(userName, "", "");
                    if (user != null)
                    {
                        // We are not updating logoutTime here.. This is only for analysis purpose. 
                        context = GenerateToken(context, user, true, apiConsumerMaster.is_log_required, Source);
                    }
                    else
                    {
                        HeaderErrorResponse(context, "User does not exist!");
                    }
                }
                else
                {
                    List<GlobalSetting> globalSettings = new BLGlobalSetting().GetGlobalSettings(Source);
                    if (globalSettings != null)
                    {

                        if (globalSettings.Any(x => x.key == "isADOIDEnabled"))
                        {
                            isADOIDEnabled = Convert.ToInt32(globalSettings.FirstOrDefault(x => x.key == "isADOIDEnabled").value) == 0 ? false : true;
                        }
                        if (globalSettings.Any(x => x.key == "isPRMSEnabled"))
                        {
                            isPRMSEnabled = Convert.ToInt32(globalSettings.FirstOrDefault(x => x.key == "isPRMSEnabled").value) == 0 ? false : true;
                        }
                         
                    }
                    if (!String.IsNullOrEmpty(ADFSEndPoint) || Convert.ToBoolean(isADOIDEnabled)|| ApplicationSettings.isLDAPEnabled)
                    {
                        //user = objDAUser.ChkUserExist(userName);
                        //Password Validation is not required for ADFS authentication.
                        user = objBLUser.ValidateUser(userName, "", Source);
                        if (user != null)
                        {
                            if (!String.IsNullOrEmpty(ADFSEndPoint))
                            {
                                password = MiscHelper.DecodeTo64(password);
                                user.user_email= MiscHelper.Decrypt(user.user_email);

                                ADFSInput objADFSInput = new ADFSInput();
                                objADFSInput.user_name = user.user_name;
                                objADFSInput.user_email = user.user_email;
                                objADFSInput.password = password;
                                objADFSInput.application_access = user.application_access;
                                objADFSInput.ADFSAutheticationBasedOn = ConfigurationManager.AppSettings["ADFSAutheticationBasedOn"].ToString().Trim();
                                objADFSInput.ADFSEndPoint = ADFSEndPoint;
                                objADFSInput.ADFSRelPartyUri = ConfigurationManager.AppSettings["ADFSRelPartyUri"].ToString().Trim();
                                objADFSInput.ADFSUserNamePreFix = ConfigurationManager.AppSettings["ADFSUserNamePreFix"].ToString().Trim();
                                ADFSDetail = BusinessLogics.API.AuthenticationADFS.AuthenticateADFS(objADFSInput);
                                user = !string.IsNullOrEmpty(ADFSDetail.tokenId) ? user : null;
                            }
                           
                            else if (Convert.ToBoolean(isADOIDEnabled))
                            {
                                ////web api calling for mobile app authorization
                                ADOIDSecoAuth aDOIDSecoAuth = new ADOIDSecoAuth();
                                SecoApiResponse secoApiResponse = null;
                                ADOIDAuthentication aDOIDAuthentication = null;
                                string accessToken = string.Empty;
                                if (!string.IsNullOrEmpty(user.user_type))
                                {
                                    if (user.user_type.ToLower() == "own")
                                    {
                                        accessToken = aDOIDSecoAuth.GenerateSecoToken(userName, MiscHelper.DecodeTo64(password), false, Source, out secoApiResponse, out aDOIDAuthentication);

                                        //ADFSDetail.tokenId = accessToken;
                                        if (!string.IsNullOrEmpty(accessToken))
                                        {
                                            aDOIDAuthentication.user_id = user.user_id;
                                            new BLUserADOIDAuthentication().SaveADOIDAuthentication(aDOIDAuthentication);
                                            if (!string.IsNullOrEmpty(aDOIDAuthentication.mobile) && !string.IsNullOrEmpty(aDOIDAuthentication.email_id))
                                            {
                                                new BLUser().UpdateEmailPhone(user.user_id, Convert.ToDouble(aDOIDAuthentication.mobile), aDOIDAuthentication.email_id);
                                            }
                                        }
                                        if (string.IsNullOrEmpty(accessToken))
                                        {
                                            user = null;
                                        }
                                    }
                                    else if (Convert.ToBoolean(isPRMSEnabled) && user.user_type.ToLower() == "partner")
                                    {
                                        
                                        if (!string.IsNullOrEmpty(user.prms_id))
                                        {
                                            accessToken = aDOIDSecoAuth.GenerateSecoToken(user.prms_id, MiscHelper.DecodeTo64(password), true, Source, out secoApiResponse, out aDOIDAuthentication);
                                            //ADFSDetail.tokenId = accessToken;
                                            if (!string.IsNullOrEmpty(accessToken))
                                            {
                                                aDOIDAuthentication.user_id = user.user_id;
                                                new BLUserADOIDAuthentication().SaveADOIDAuthentication(aDOIDAuthentication);
                                                if (!string.IsNullOrEmpty(aDOIDAuthentication.mobile) && !string.IsNullOrEmpty(aDOIDAuthentication.email_id))
                                                {
                                                    new BLUser().UpdateEmailPhone(user.user_id, Convert.ToDouble(aDOIDAuthentication.mobile), aDOIDAuthentication.email_id);
                                                }
                                            }
                                            if (string.IsNullOrEmpty(accessToken))
                                            {
                                                user = null;
                                            }
                                        }
                                        else
                                        {
                                            user = objBLUser.ValidateUser(userName, password, Source);
                                        }
                                    }
                                    else
                                    {
                                        user = objBLUser.ValidateUser(userName, password, Source);
                                    }
                                }
                                else
                                {
                                    user = objBLUser.ValidateUser(userName, password, Source);
                                }
                            }
                            else if (Convert.ToBoolean(isldapEnable) && user.user_type.ToLower() == "own")
                            {
                                SecoApiResponse secoApiResponse = null;
                                LDAPAuthentication aLDAPAuthentication = null;
                                ADOIDSecoAuth aDOIDSecoAuth = new ADOIDSecoAuth();
                                bool ldap = false;
                                ldap = aDOIDSecoAuth.GenerateLdapToken(userName, MiscHelper.DecodeTo64(password), false, Source, out secoApiResponse, out aLDAPAuthentication);
                                if (!ldap)
                                {
                                    user = null;
                                }
                            }
                            else
                            {
                                user = objBLUser.ValidateUser(userName, password, Source);
                            }
                        }
                        else
                        {
                            HeaderErrorResponse(context, "User does not exist!");
                        }
                        /////Check for force login check

                        if (user != null)
                        {

                            if (dicRequestParams.ContainsKey("forceLogin"))
                            {
                                if (bool.Parse(dicRequestParams["forceLogin"]))
                                {
                                    new BLUserLogin().UpdateLogOutTime(user.user_id, Source);// logout user.
                                    context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
                                }
                                else
                                {
                                    var userLoginDetails = new BLUserLogin().GetUserLoginDetailById(user.user_id, Source);
                                    if (userLoginDetails != null && !userLoginDetails.logout_time.HasValue && userLoginDetails.source.ToUpper() == "MOBILE" && userLoginDetails.mac_address != dicRequestParams["macAddress"])
                                    {
                                        LoginErrorResponse(context, "You are logging from another mobile device, older mobile login shall AutoLogout");

                                    }
                                    else
                                    {
                                        context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
                                    }
                                    //validate loginTime, if loginTime null or empty then login else send message
                                }
                            }
                            else
                            {
                                context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
                            }
                        }
                        else
                        {
                            HeaderErrorResponse(context, "The user name or password is incorrect!");
                        }
                    }
                    
                    else
                    {
                        user = objBLUser.ValidateUser(userName, password, Source);

                        if (user != null)
                        {
                            if (user.is_active == true)
                            {

                                if (dicRequestParams.ContainsKey("forceLogin"))
                                {
                                    if (bool.Parse(dicRequestParams["forceLogin"]))
                                    {
                                        new BLUserLogin().UpdateLogOutTime(user.user_id, Source);// logout user.

                                        //context.OwinContext.Authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);

                                        //int expire = 2 * 2;
                                        // context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddSeconds(expire));
                                        context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
                                    }
                                    else
                                    {
                                        var userLoginDetails = new BLUserLogin().GetUserLoginDetailById(user.user_id, Source);
                                        if (userLoginDetails != null && !userLoginDetails.logout_time.HasValue && userLoginDetails.source.ToUpper() == "MOBILE" && userLoginDetails.mac_address != dicRequestParams["macAddress"])
                                        {
                                            LoginErrorResponse(context, "You are logging from another mobile device, older mobile login shall AutoLogout");

                                        }
                                        else
                                        {
                                            context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
                                        }
                                        //validate loginTime, if loginTime null or empty then login else send message
                                    }
                                }
                                else
                                {
                                    context = GenerateToken(context, user, false, apiConsumerMaster.is_log_required, Source);
                                }
                                // context = GenerateToken(context, user);
                            }
                            else
                            {
                                HeaderErrorResponse(context, "This user is currently deactivated. Please contact system administrator!");
                            }
                        }
                        else
                        {
                            HeaderErrorResponse(context, "The user name or password is incorrect!");
                        }

                    }
                }

            });

        }

        #endregion

        #region[ValidateClientAuthentication]
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }
        #endregion

        #region[TokenEndpoint]
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {

            object inputs;
            context.OwinContext.Environment.TryGetValue("Microsoft.Owin.Form#collection", out inputs);
            string grant_type = ((Microsoft.Owin.FormCollection)inputs)?.GetValues("grant_type")[0].ToString();

            context.AdditionalResponseParameters.Add("status", StatusCodes.OK.ToString());
            string source = null;
            context.AdditionalResponseParameters.Add("error_message", "");
            ////when otp validation done then generate the refresh token to get application to access the resource
            if (grant_type == "refresh_token")
            {
                context.Properties.Dictionary.Remove("OTP");
            }
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                if (property.Value != "OTP")
                {
                    context.AdditionalResponseParameters.Add(property.Key, property.Value);
                }
                if (property.Key == "source")
                    source = property.Value;
            }
            // add global setting parameters

            List<GlobalSetting> globalSettings = new BLGlobalSetting().GetGlobalSettings(source);
            OTPAuthenticationSettings oTPAuthenticationSettings = new BLOtpAuthentication().getOtpConfigurationSetting(source);

            var item = globalSettings.FirstOrDefault(o => o.key == "is2FAuthEnabled");
            if (item != null)
            {
                item.value = Convert.ToBoolean(oTPAuthenticationSettings.is_otp_enabled) == true ? "1" : "0";
            }
            if (globalSettings != null)
            {
                var lst = globalSettings.Select(m => new { key = m.key, value = m.value, description = m.description }).ToList();
                //lst.Add(new { key = "is2FAuthEnabled", value =Convert.ToBoolean(oTPAuthenticationSettings.is_otp_enabled)==true?"1":"0", description = "" });
                var json = JsonConvert.SerializeObject(lst);
                context.AdditionalResponseParameters.Add("GlobalSettings", json);
            }
            return Task.FromResult<object>(null);

        }

        #endregion

        //public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        //{
        //    try
        //    {
        //        // chance to change authentication ticket for refresh token requests
        //        var newId = new ClaimsIdentity(context.Ticket.Identity);
        //        newId.AddClaim(new Claim("newClaim", "refreshToken"));
        //        var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
        //        context.Validated(newTicket);
        //        context.OwinContext.Set<string>("historyid", newId.FindFirst("historyid").Value);

        //    }
        //    catch (Exception ex)
        //    {
        //        //LogHelper.GetInstance.WriteLog("AuthClasses => SimpleAuthorizationServerProvider => GrantRefreshToken", ex);
        //    }

        //}

        public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
        {
            private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

            public async Task CreateAsync(AuthenticationTokenCreateContext context)
            {
                try
                {
                    int loginId = Convert.ToInt32(context.OwinContext.Get<string>("loginId"));
                    int userId = Convert.ToInt32(context.OwinContext.Get<string>("userId"));
                    var refreshToken = Guid.NewGuid().ToString();
                    new BLUserLogin().UpdateRefreshToken(userId, loginId, refreshToken);
                    // maybe only create a handle the first time, then re-use

                    _refreshTokens.TryAdd(refreshToken, context.Ticket);
                    // consider storing only the hash of the handle
                    context.SetToken(refreshToken);
                }
                catch (Exception ex)
                {
                    //LogHelper.GetInstance.WriteLog("AuthClasses => SimpleRefreshTokenProvider => CreateAsync", ex);
                }
            }

            public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
            {
                try
                {
                    var clientip = context.Request.RemoteIpAddress;

                    //  UserLogin user_api_login = new UserLogin();

                    //UserLogin user_api_login = BusinessLogics.API.BLUserApiLogin.GetUserApiHistoryByRefreshToken(context.Token);// BLUserApiLogin.GetUserApiHistoryByRefreshToken(context.Token);

                    //if (user_api_login != null)
                    //{
                    //    //check IP Address of client
                    //    if (clientip != user_api_login.client_ip)
                    //    {
                    //        return;
                    //    }
                    //}
                    AuthenticationTicket ticket;

                    if (_refreshTokens.TryRemove(context.Token, out ticket))
                    {
                        context.SetTicket(ticket);
                    }
                }
                catch (Exception ex)
                {
                    //LogHelper.GetInstance.WriteLog("AuthClasses => SimpleRefreshTokenProvider => ReceiveAsync", ex);
                }
            }

            public void Create(AuthenticationTokenCreateContext context)
            {
                throw new NotImplementedException();
            }

            public void Receive(AuthenticationTokenReceiveContext context)
            {
                throw new NotImplementedException();
            }
        }

        #region[HeaderErrorResponse]
        public Models.API.ApiResponse<object> HeaderErrorResponse(OAuthGrantResourceOwnerCredentialsContext context, string sErrorMsg)
        {
            var resp = new Models.API.ApiResponse<object>();
            resp.error_message = sErrorMsg;
            resp.status = StatusCodes.REQUEST_DENIED.ToString();

            string jsonString = JsonConvert.SerializeObject(resp);
            context.SetError(new string(' ', jsonString.Length - 12));

            context.Response.StatusCode = 400;
            context.Response.Write(jsonString);

            return resp;
        }

        #endregion
        public Models.API.ApiResponse<object> LoginErrorResponse(OAuthGrantResourceOwnerCredentialsContext context, string sErrorMsg)
        {
            var resp = new Models.API.ApiResponse<object>();
            resp.error_message = sErrorMsg;
            resp.status = StatusCodes.PARTIAL_SUCCESS.ToString();

            string jsonString = JsonConvert.SerializeObject(resp);
            context.SetError(new string(' ', jsonString.Length - 12));

            context.Response.StatusCode = 400;
            context.Response.Write(jsonString);

            return resp;
        }

        #region[GenerateToken]
        public OAuthGrantResourceOwnerCredentialsContext GenerateToken(OAuthGrantResourceOwnerCredentialsContext context, dynamic user, bool isMasterLogin, bool isLogRequired, string source)
        {
            try
            {
                var claims = new List<Claim>();
                IDictionary<string, string> dicResponseParams = new Dictionary<string, string>();
                var dicRequestParams = getAllBodyParameters(context);
                bool is2FAuthEnabled = false;
                OTPAuthenticationSettings oTPAuthenticationSettings = new BLOtpAuthentication().getOtpConfigurationSetting(source);
              
                if (oTPAuthenticationSettings != null)
                {
                    is2FAuthEnabled = Convert.ToBoolean(oTPAuthenticationSettings.is_otp_enabled);
                }
                //for testing
                //is2FAuthEnabled = true;
                if (source.ToUpper() != "WEB" && source.ToUpper() != "MOBILE")
                {
                    dicResponseParams.Add("isLogRequired", Convert.ToString(isLogRequired));
                    dicResponseParams.Add("userName", user.user_name);
                    dicResponseParams.Add("isActive", Convert.ToString(user.is_active));
                    dicResponseParams.Add("source", source);
                }
                else
                {
                    claims = new List<Claim>() {
                             new Claim("userid", Convert.ToString(user.user_id)),
                             new Claim("username", user.user_name),
                                new Claim("email", user.user_email)
                          //new Claim("IsMasterLogin",Convert.ToString(isMasterLogin))                     
                     };

                    UserLogin objUserLogin = new UserLogin();
                    objUserLogin.user_id = user.user_id;
                    objUserLogin.login_time = DateTimeHelper.Now;
                    objUserLogin.client_ip = context.OwinContext.Request.RemoteIpAddress;
                    objUserLogin.server_ip = IPHelper.GetServerIP();
                    //Add Information For Web Token Generation
                    if (dicRequestParams != null && source.ToUpper() == "WEB")
                    {
                        //UserLogin objResult = new BLUserLogin().GetUserLoginDetailById(objUserLogin.user_id);
                        objUserLogin.source = "Web";
                        UserLogin objResult = new BLUserLogin().SaveLoginHistory(objUserLogin, source);

                        context.OwinContext.Set<string>("loginId", objResult.login_id.ToString());
                        context.OwinContext.Set<string>("userId", objResult.user_id.ToString());
                        //Add  additional parameter to token response..
                        dicResponseParams.Add("isLogRequired", Convert.ToString(isLogRequired));
                        dicResponseParams.Add("userId", objUserLogin.user_id.ToString());
                        dicResponseParams.Add("loginHistoryId", objResult.history_id.ToString());
                        dicResponseParams.Add("userName", user.user_name);
                        dicResponseParams.Add("email", user.user_email);
                        dicResponseParams.Add("isActive", Convert.ToString(user.is_active));
                        dicResponseParams.Add("userRoleId", user.role_id.ToString());
                        dicResponseParams.Add("userRole", user.role_name);
                        dicResponseParams.Add("source", source);
                    }
                    //Add Information For Mobile Token Generation
                    else
                    {
                        foreach (var item in dicRequestParams)
                        {
                            if (item.Key == "osName")
                                objUserLogin.os_name = item.Value;
                            else if (item.Key == "osType")
                                objUserLogin.os_type = item.Value;
                            else if (item.Key == "osVersion")
                                objUserLogin.os_version = item.Value;
                            else if (item.Key == "macAddress")
                                objUserLogin.mac_address = item.Value;
                            else if (item.Key == "appVersion")
                                objUserLogin.mobile_app_version = item.Value;
                        }

                        objUserLogin.source = source;
                        UserLogin objResult = new BLUserLogin().SaveLoginHistory(objUserLogin,source);
                        List<GlobalSetting> globalSettings = new BLGlobalSetting().GetGlobalSettings(source);
                       
                        var appVersion = String.Empty;
                       

                        if (globalSettings != null)
                        {
                            if (!string.IsNullOrEmpty(objUserLogin.os_name) && objUserLogin.os_name.ToUpper() == "IOS")
                            {
                                appVersion = globalSettings.FirstOrDefault(x => x.key == "IOS_Version").value;
                            }
                            else if (!string.IsNullOrEmpty(objUserLogin.os_name) && objUserLogin.os_name.ToUpper() == "ANDROID")
                            {
                                appVersion = globalSettings.FirstOrDefault(x => x.key == "appVersion").value;
                            }

                            //if (globalSettings.Any(x => x.key == "is2FAuthEnabled"))
                            //{
                            //    is2FAuthEnabled = Convert.ToInt32(globalSettings.FirstOrDefault(x => x.key == "is2FAuthEnabled").value) == 0 ? false : true;
                            //}
                            
                        }


                        context.OwinContext.Set<string>("loginId", objResult.login_id.ToString());
                        context.OwinContext.Set<string>("userId", objResult.user_id.ToString());
                        //Add  additional parameter to token response..
                        dicResponseParams.Add("userId", user.user_id.ToString());
                        dicResponseParams.Add("isLogRequired", Convert.ToString(apiConsumerMaster.is_log_required));
                        dicResponseParams.Add("userName", user.user_name);
                        dicResponseParams.Add("email", MiscHelper.Decrypt(user.user_email));
                        dicResponseParams.Add("loginHistoryId", objResult.history_id.ToString());
                        dicResponseParams.Add("isActive", Convert.ToString(user.is_active));
                        dicResponseParams.Add("userRoleId", user.role_id.ToString());
                        dicResponseParams.Add("userRole", user.role_name);
                        dicResponseParams.Add("appVersion", appVersion);
                        dicResponseParams.Add("osName", objResult.os_name);
                        dicResponseParams.Add("source", objResult.source.ToString());
                        dicResponseParams.Add("IsMasterLogin", Convert.ToString(isMasterLogin));
                    }
                }
                
                ////when is2FAuthEnabled is true in global setting  then first token will always be applicable for otp
                ////validation
                if (is2FAuthEnabled && source.ToUpper() == "MOBILE" && user.role_id != 1)
                {
                    ////for flushingg the otp reset limit
                    new BLUserOTPDetails().ResetUserOTPStatus(user.user_id, user.user_name, Convert.ToString(OTPResetType.RESET_RESEND_LIMIT_REACHED));
                    dicResponseParams.Add("OTP", "OTP");
                    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                    var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties(dicResponseParams));
                    context.Validated(ticket);
                }
                ////when is2FAuthEnabled is true in global setting  then first token will always be applicable for otp
                ////validation
                else if (is2FAuthEnabled && source.ToUpper() == "WEB" && user.role_id != 1)
                {
                    dicResponseParams.Add("OTP", "OTP");
                    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                    var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties(dicResponseParams));
                    context.Validated(ticket);
                }
                //// generate always application access token
                else
                {
                    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                    var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties(dicResponseParams));
                    context.Validated(ticket);
                }
            }
            catch (KeyNotFoundException)
            {
                HeaderErrorResponse(context, "Some parameters are missing in request!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return context;

        }


        private static Dictionary<string, string> getAllBodyParameters(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var dicParams = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            var formCollectionTask = context.Request.ReadFormAsync();
            formCollectionTask.Wait();

            foreach (var pair in formCollectionTask.Result)
            {
                var value = pair.Value != null ? string.Join(",", pair.Value) : null;
                dicParams.Add(pair.Key, value);
            }
            return dicParams;
        }
        #endregion

        //  This method for get azure user name 
        public string getAzureUserName(string access_token)
        {
            string username = "";
            string DATA = "";
            //TokenDetail tokenDetail = ReqHelper.GetRequestData<TokenDetail>(data);
            ErrorLogHelper logHelper = new ErrorLogHelper();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

                string Apiinstance = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ida:userinfoapi"]);
                string URL = Apiinstance;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + access_token);
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string response = responseReader.ReadToEnd();
                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);
                    username = json.userPrincipalName.ToString();


                }
            }
            catch (Exception ex)
            {
                //logHelper.ApiLogWriter("getAzureUserName", "OAuthProvider", access_token, ex);

            }
            return username;
        }

       
    }

}