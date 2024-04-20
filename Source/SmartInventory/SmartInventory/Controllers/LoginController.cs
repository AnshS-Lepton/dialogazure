using BusinessLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using Models.Admin;
using System.Web.Security;
using SmartInventory.Filters;
using Utility;
using System.Configuration;
using SmartInventory.Settings;
using System.Net;
using SmartInventory.Helper;
using Lepton.Utility;
using Newtonsoft.Json;
using Lepton.Entities;
using System.IO;
using System.Web.SessionState;

namespace SmartInventory.Controllers
{ 

    //[AllowAnonymous]
    [HandleException]
    public class LoginController : Controller
    {
       
        int count = 0;
        //
        // GET: /Login/
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()

        {         
            string cookie = "";
            if (Session["Language"] != null)
            {
                cookie = Session["Language"].ToString();
            }
            ViewBag.cultureInfo = cookie;
            if (Request.IsAuthenticated && Session["user_id"] != null)
            {
                //redirect to page on the bases of user role...
                return RedirectToAction("index", "main");
            }
            else
            {
                ViewBag.langlist = new BLResources().GetResourceCultureList();
                return View();
            }
        }

        [HttpPost]
        public ActionResult Index(User usr, string returnUrl)
        {
            //if (ModelState.IsValid)

            if (usr.user_name != null && usr.password != null && usr.user_name != "" && usr.password != "")
            {
                var globalSettings = new BLGlobalSetting().GetGlobalSettings("WEB");
                OTPAuthenticationSettings oTPAuthenticationSettings = new BLOtpAuthentication().getOtpConfigurationSetting("WEB");

                bool is2FAuthEnabled = false;
                bool isADOIDEnabled = false;
                bool isPRMSEnabled = false;
                string err_msg = string.Empty;

                foreach (var objSetting in globalSettings)
                {
                    if (objSetting.key == "isADOIDEnabled")
                    {
                        isADOIDEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                    }
                    //if (objSetting.key == "is2FAuthEnabled")
                    //{
                    //    is2FAuthEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                    //}
                    if (objSetting.key == "isPRMSEnabled")
                    {
                        isPRMSEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                    }
                }
                if (oTPAuthenticationSettings != null)
                {
                    is2FAuthEnabled = Convert.ToBoolean(oTPAuthenticationSettings.is_otp_enabled);
                }

                bool is_UserAuthenticated = false;
                User u = new User();
                u = new BLUser().ChkUserExist(usr.user_name);
                if (u == null)
                {
                    ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_011);
                }
                else
                {
                    //u = new BLUser().ValidateUser(usr.user_name, "", UserType.Web.ToString());
                    if (u.application_access.ToUpper() != UserType.Mobile.ToString().ToUpper())
                    {
                        if (u.is_active)
                        {
                            var ADFSEndPoint = ApplicationSettings.ADFSEndPoint;
                            bool IsSeprateLandingEnabled = false;

                            if (ApplicationSettings.IsSeprateLandingEnabled && u.role_id == 1)
                            {
                                IsSeprateLandingEnabled = true;
                            }

                            //var ADOIDSecoAuthURL = ApplicationSettings.ADOIDSecoAuthURL;
                            //var PRMSAuthURL = ApplicationSettings.PRMSAuthURL;
                            if (!String.IsNullOrEmpty(ADFSEndPoint) && u != null && u.role_id != 1 && u.user_type.ToLower() == "own")
                            {
                                ADFSInput objADFSInput = new ADFSInput();
                                objADFSInput.user_name = u.user_name;
                                objADFSInput.user_email = MiscHelper.Decrypt(u.user_email);
                                objADFSInput.password = usr.password;
                                objADFSInput.ADFSAutheticationBasedOn = ApplicationSettings.ADFSAutheticationBasedOn;
                                objADFSInput.ADFSEndPoint = ApplicationSettings.ADFSEndPoint;
                                objADFSInput.ADFSRelPartyUri = ApplicationSettings.ADFSRelPartyUri;
                                objADFSInput.ADFSUserNamePreFix = ApplicationSettings.ADFSUserNamePreFix;
                                ADFSDetail ADFSDetail = BLUser.AuthenticateADFS(objADFSInput);
                                if (!string.IsNullOrEmpty(ADFSDetail.tokenId))
                                {
                                    is_UserAuthenticated = true;
                                    Session["TokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
                                }
                                else
                                {
                                    if (ADFSDetail.ADFSException != null)
                                    {
                                        is_UserAuthenticated = false;
                                        ErrorLogHelper.WriteErrorLog("Index", "Login", ADFSDetail.ADFSException, objADFSInput);
                                    }
                                    ModelState.AddModelError("Error", ADFSDetail.errorMsg);
                                }
                            }
                            else
                            {

                                TokenDetail tokenDetail = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
                                if (tokenDetail != null)
                                {
                                    Session["TokenDetail"] = tokenDetail;
                                    is_UserAuthenticated = !string.IsNullOrEmpty(tokenDetail.access_token) ? true : false;
                                }
                                else
                                {
                                    is_UserAuthenticated = false;
                                }

                            }
                            //if (Convert.ToBoolean(isADOIDEnabled))
                            //{
                            //ADOIDSecoAuth aDOIDSecoAuth = new ADOIDSecoAuth();
                            //SecoApiResponse secoApiResponse = null;
                            //ADOIDAuthentication aDOIDAuthentication = null;
                            //string accessToken = string.Empty;
                            //if (!string.IsNullOrEmpty(u.user_type))
                            //{
                            //    if (u != null && u.role_id != 1 && u.user_type.ToLower() == "own")
                            //    {
                            //        accessToken = aDOIDSecoAuth.GenerateSecoToken(usr.user_name, usr.password, false, UserType.Web.ToString(), out secoApiResponse,out aDOIDAuthentication);
                            //        if (!string.IsNullOrEmpty(accessToken))
                            //        {
                            //            aDOIDAuthentication.user_id = u.user_id;
                            //            new BLUserADOIDAuthentication().SaveADOIDAuthentication(aDOIDAuthentication);
                            //        }
                            //        is_UserAuthenticated = string.IsNullOrEmpty(accessToken) ? false : true;
                            //    }
                            //    else if (Convert.ToBoolean(isPRMSEnabled) && u != null && u.role_id != 1 && u.user_type.ToLower() == "partner")
                            //    {
                            //        if (!string.IsNullOrEmpty(u.prms_id))
                            //        {
                            //            accessToken = aDOIDSecoAuth.GenerateSecoToken(u.prms_id, usr.password, true, UserType.Web.ToString(), out secoApiResponse, out aDOIDAuthentication);

                            //            if (!string.IsNullOrEmpty(accessToken))
                            //            {
                            //                aDOIDAuthentication.user_id = u.user_id;
                            //                new BLUserADOIDAuthentication().SaveADOIDAuthentication(aDOIDAuthentication);
                            //                is_UserAuthenticated = true;
                            //            }
                            //            else
                            //            {
                            //                is_UserAuthenticated = false;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            u = new BLUser().ValidateUser(usr.user_name, Utility.MiscHelper.EncodeTo64(usr.password), UserType.Web.ToString());
                            //            is_UserAuthenticated = (u != null && u.is_active) ? true : false;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        u = new BLUser().ValidateUser(usr.user_name, Utility.MiscHelper.EncodeTo64(usr.password), UserType.Web.ToString());
                            //        is_UserAuthenticated = (u != null && u.is_active) ? true : false;
                            //    }
                            //}
                            //else
                            //{
                            //    u = new BLUser().ValidateUser(usr.user_name, Utility.MiscHelper.EncodeTo64(usr.password), UserType.Web.ToString());
                            //    is_UserAuthenticated = (u != null && u.is_active) ? true : false;
                            //}
                            //}
                            //else
                            //{
                            //     u = new BLUser().ValidateUser(usr.user_name, Utility.MiscHelper.EncodeTo64(usr.password), UserType.Web.ToString());
                            //     is_UserAuthenticated = (u != null && u.is_active) ? true : false;
                            // }


                            ////User validated from either thirtd party or our own database
                            if (is_UserAuthenticated)
                            {
                                u = new BLUser().GetUserDetailByUserName(usr.user_name);
                                ////for 2F Authentication
                                if (Convert.ToBoolean(is2FAuthEnabled))
                                {
                                    if (u.role_id != 1 || (u.role_id == 1 && Convert.ToBoolean(ApplicationSettings.is_otp_enabled_for_admin_web)))
                                    {
                                        SendOPTDeliveryOption sendOPTDeliveryOption = new SendOPTDeliveryOption();
                                        sendOPTDeliveryOption.user_id = u.user_id;
                                        sendOPTDeliveryOption.user_name = u.user_name;
                                        sendOPTDeliveryOption.mobile_number = u.mobile_number;
                                        sendOPTDeliveryOption.user_email = MiscHelper.Decrypt(u.user_email);
                                        if (!string.IsNullOrEmpty(sendOPTDeliveryOption.user_email))
                                        {
                                            sendOPTDeliveryOption.mask_user_email = MiscHelper.MaskEmail(sendOPTDeliveryOption.user_email.ToString());
                                        }
                                        if (!string.IsNullOrEmpty(Convert.ToString(sendOPTDeliveryOption.mobile_number)))
                                        {
                                            sendOPTDeliveryOption.mask_mobile_number = MiscHelper.MaskMobile(sendOPTDeliveryOption.mobile_number.ToString());
                                        }
                                        u.password = usr.password;
                                        ////for flushingg the otp reset limit
                                        new BLUserOTPDetails().ResetUserOTPStatus(u.user_id, u.user_name, Convert.ToString(OTPResetType.RESET_RESEND_LIMIT_REACHED));
                                        string otpToken = GenerateOTPToken(u);
                                        if (!string.IsNullOrEmpty(otpToken))
                                        {
                                            //sendOPTDeliveryOption.otptype = "mobile";
                                            TempData["SendOPTDeliveryOption"] = sendOPTDeliveryOption;
                                            return View("SendOtp", sendOPTDeliveryOption);
                                        }
                                    }
                                    else
                                    {
                                        LogUserIn(u, "Web");
                                        //Session["TokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
                                        TokenDetail td = (TokenDetail)Session["TokenDetail"];
                                        Session["TokenDetail"] = WebAPIRequest.GetRefreshToken(td.refresh_token, td.access_token);
                                        ModelState.Clear();
                                        if (IsSeprateLandingEnabled)
                                        {
                                            return RedirectToAction("index", "Admin/User/ViewUsers");
                                        }
                                        else
                                        {
                                            return RedirectToAction("index", "main");
                                        }

                                    }
                                }
                                else
                                {
                                    LogUserIn(u, "Web");
                                    //Session["TokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
                                    TokenDetail td = (TokenDetail)Session["TokenDetail"];
                                    Session["TokenDetail"] = WebAPIRequest.GetRefreshToken(td.refresh_token, td.access_token);
                                    ModelState.Clear();
                                    if (IsSeprateLandingEnabled)
                                    {
                                        return RedirectToAction("index", "Admin/User/ViewUsers");
                                    }
                                    else
                                    {
                                        return RedirectToAction("index", "main");
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_013);
                            }
                        }
                        else
                            ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_010);
                    }
                    else
                        ModelState.AddModelError("Error", Resources.Resources.SI_OSP_GBL_NET_FRM_370); 
                }
            }
            else
            {
                if (usr.user_name == null || usr.user_name == "")
                    ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_014);
                else if (usr.password == null || usr.password == "")
                    ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_015);
                else
                    ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_012);
            }
            usr.password = "";
            ViewBag.langlist = new BLResources().GetResourceCultureList();
            return View(usr);
        }
        //[HttpPost]
        //public ActionResult Index1(User usr, string returnUrl)
        //{
        //    //if (ModelState.IsValid)

        //    if (usr.user_name != null && usr.password != null && usr.user_name != "" && usr.password != "")
        //    {
        //        User u = new User();
        //        //u = new BLUser().ChkUserExist(usr.user_name);
        //        u = new BLUser().ValidateUser(usr.user_name, "", UserType.Web.ToString());
        //        var ADFSEndPoint = ApplicationSettings.ADFSEndPoint;
        //        if (!String.IsNullOrEmpty(ADFSEndPoint) && u != null && u.role_id != 1 && u.user_type == "own")
        //        {

        //            if (u != null)
        //            {
        //                if (u.is_active)
        //                {
        //                    ADFSInput objADFSInput = new ADFSInput();
        //                    objADFSInput.user_name = u.user_name;
        //                    objADFSInput.user_email = u.user_email;
        //                    objADFSInput.password = usr.password;
        //                    objADFSInput.ADFSAutheticationBasedOn = ApplicationSettings.ADFSAutheticationBasedOn;
        //                    objADFSInput.ADFSEndPoint = ApplicationSettings.ADFSEndPoint;
        //                    objADFSInput.ADFSRelPartyUri = ApplicationSettings.ADFSRelPartyUri;
        //                    objADFSInput.ADFSUserNamePreFix = ApplicationSettings.ADFSUserNamePreFix;
        //                    ADFSDetail ADFSDetail = BLUser.AuthenticateADFS(objADFSInput);
        //                    if (!string.IsNullOrEmpty(ADFSDetail.tokenId))
        //                    {
        //                        LogUserIn(u);
        //                        Session["TokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
        //                        TokenDetail td = (TokenDetail)Session["TokenDetail"];
        //                        Session["TokenDetail"] = WebAPIRequest.GetRefreshToken(td.refresh_token);
        //                        ModelState.Clear();
        //                        return RedirectToAction("index", "main");
        //                    }
        //                    else
        //                    {
        //                        if (ADFSDetail.ADFSException != null)
        //                        {
        //                            ErrorLogHelper.WriteErrorLog("Index", "Login", ADFSDetail.ADFSException, objADFSInput);
        //                        }
        //                        ModelState.AddModelError("Error", ADFSDetail.errorMsg);

        //                    }
        //                }
        //                else
        //                    ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_010);

        //            }
        //            else
        //                ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_011);
        //        }
        //        ////for 2F Authentication
        //        else if (Convert.ToBoolean(ApplicationSettings.is2FAuthEnabled))
        //        {
        //            if (u != null)
        //            {
        //                if (u.is_active)
        //                {
        //                    ////for OAIM validation
        //                    if (u.user_type == "own")
        //                    {
        //                        //Helper.OIAMHelper oIAMHelper = new Helper.OIAMHelper();
        //                        //Helper.OAIMApiResponse oAIMApiResponce = oIAMHelper.GenerateOIAMToken(user.user_name, password);
        //                        //if (oAIMApiResponce.access_token != null)
        //                        //{
        //                        //    //call the otp send request here 
        //                        //}
        //                        //else
        //                        //{
        //                        //    user = null;
        //                        //}
        //                    }
        //                    else
        //                    {
        //                        u = new BLUser().ValidateUser(usr.user_name, Utility.MiscHelper.EncodeTo64(usr.password), UserType.Web.ToString());
        //                    }
        //                    if (u != null)
        //                    {
        //                        if (u.user_id > 0)
        //                        {
        //                            if (u.is_active)
        //                            {
        //                                u = new BLUser().GetUserDetailByUserName(usr.user_name);
        //                                u.is_otp_enabled = true;
        //                                if (Convert.ToBoolean(u.is_otp_enabled))
        //                                {

        //                                    SendOPTDeliveryOption sendOPTDeliveryOption = new SendOPTDeliveryOption();
        //                                    sendOPTDeliveryOption.user_id = u.user_id;
        //                                    sendOPTDeliveryOption.user_name = u.user_name;
        //                                    sendOPTDeliveryOption.mobile_number = u.mobile_number;
        //                                    sendOPTDeliveryOption.user_email = u.user_email;
        //                                    if (!string.IsNullOrEmpty(sendOPTDeliveryOption.user_email))
        //                                    {
        //                                        sendOPTDeliveryOption.mask_user_email = MiscHelper.MaskEmail(sendOPTDeliveryOption.user_email.ToString());
        //                                    }
        //                                    if (!string.IsNullOrEmpty(Convert.ToString(sendOPTDeliveryOption.mobile_number)))
        //                                    {
        //                                        sendOPTDeliveryOption.mask_mobile_number = MiscHelper.MaskMobile(sendOPTDeliveryOption.mobile_number.ToString());
        //                                    }
        //                                    u.password = usr.password;
        //                                    ////for flushingg the otp resed limit
        //                                    new BLUserOTPDetails().ResetUserOTPStatus(u.user_id, u.user_name, Convert.ToString(OTPResetType.RESET_RESEND_LIMIT_REACHED));
        //                                    string otpToken = GenerateOTPToken(u);
        //                                    if (!string.IsNullOrEmpty(otpToken))
        //                                    {

        //                                        TempData["SendOPTDeliveryOption"] = sendOPTDeliveryOption;
        //                                        return View("SendOtp", sendOPTDeliveryOption);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    LogUserIn(u);
        //                                    Session["TokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
        //                                    TokenDetail td = (TokenDetail)Session["TokenDetail"];
        //                                    Session["TokenDetail"] = WebAPIRequest.GetRefreshToken(td.refresh_token);
        //                                    ModelState.Clear();
        //                                    if (u.role_id == 1) // for super Admin only...
        //                                    {
        //                                        return RedirectToAction("index", "main");
        //                                    }
        //                                    else
        //                                    {
        //                                        return RedirectToAction("index", "main");
        //                                    }
        //                                }
        //                            }
        //                            else
        //                                ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_010);
        //                        }
        //                        else
        //                            ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_012);
        //                    }
        //                    else
        //                        ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_013);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            u = new BLUser().ValidateUser(usr.user_name, Utility.MiscHelper.EncodeTo64(usr.password), UserType.Web.ToString());
        //            if (u != null)
        //            {
        //                if (u.user_id > 0)
        //                {
        //                    if (u.is_active)
        //                    {
        //                        LogUserIn(u);
        //                        Session["TokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
        //                        TokenDetail td = (TokenDetail)Session["TokenDetail"];
        //                        Session["TokenDetail"] = WebAPIRequest.GetRefreshToken(td.refresh_token);
        //                        ModelState.Clear();
        //                        if (u.role_id == 1) // for super Admin only...
        //                        {
        //                            return RedirectToAction("index", "main");
        //                            //return RedirectToAction("Index", "Home", new { area = "Admin" });
        //                        }
        //                        else
        //                        {
        //                            return RedirectToAction("index", "main");
        //                        }
        //                    }
        //                    else
        //                        ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_010);
        //                }
        //                else
        //                    ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_012);
        //            }
        //            else
        //                ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_013);
        //        }
        //    }
        //    else
        //    {
        //        if (usr.user_name == null || usr.user_name == "")
        //            ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_014);
        //        else if (usr.password == null || usr.password == "")
        //            ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_015);
        //        else
        //            ModelState.AddModelError("Error", Resources.Resources.SI_GBL_GBL_NET_FRM_012);
        //    }
        //    usr.password = "";
        //    ViewBag.langlist = new BLResources().GetResourceCultureList();
        //    return View(usr);
        //}

        private void LogUserIn(User user, string Source)
        {
            SessionIDManager manager = new SessionIDManager();
            string newSessionId = manager.CreateSessionID(System.Web.HttpContext.Current);
            bool redirected = false;
            bool IsAdded = false;
            manager.SaveSessionID(System.Web.HttpContext.Current, newSessionId, out redirected, out IsAdded);

            //SaveLoginHistory(user.user_id);
            Session["user_id"] = user.user_id;
            //get user image
            user.userImgBytes = getUserProfileImage(user.user_id, user.user_img);
            Session["userDetail"] = user;
            var userLoginHistory = new BLUserLogin().GetUserLoginDetailById(user.user_id,Source);
            UpdateBrowserInfo(userLoginHistory.login_id);
            Session["userLoginHistory"] = userLoginHistory;

            //FormsAuthentication.SetAuthCookie(user.user_name, false);

            /* To tightly couple session id with the form auth ticket,
             adding session ID to Forms Authentication ticket */
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1, 
                user.user_name, 
                DateTime.Now, 
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), 
                false, // persistent
                Session.SessionID 
            );
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            Response.Cookies.Add(authCookie);
        }

        void SaveLoginHistory(int userId)
        {
            UserLogin objUserLogin = new UserLogin();

            try
            {
                objUserLogin.user_id = userId;
                objUserLogin.client_ip = IPHelper.GetIPAddress();
                BrowserHelper browserHelper = new BrowserHelper();
                objUserLogin.browser_name = browserHelper.BrowserName;
                objUserLogin.browser_version = browserHelper.BrowserVersion;
                objUserLogin.session_id = Session.SessionID;
                objUserLogin.login_time = DateTimeHelper.Now;
                objUserLogin.source = "Web";
                objUserLogin.server_ip = IPHelper.GetServerIP();
                new BLUserLogin().SaveLoginHistory(objUserLogin, objUserLogin.source);
            }
            catch (Exception ex)
            {
                ErrorLogInput objErrorLog = new ErrorLogInput();
                objErrorLog.userId = Convert.ToInt32(Session["user_id"]);
                objErrorLog.UserName = User.Identity.Name;
                objErrorLog.fromPage = "Login Controller";
                objErrorLog.fromMethod = "SaveLoginHistory()";
                objErrorLog.clientIp = IPHelper.GetIPAddress();
                objErrorLog.serverIp = Dns.GetHostEntry(Dns.GetHostName()).HostName ?? "";
                BrowserHelper browserHelper = new BrowserHelper();
                objErrorLog.browserName = browserHelper.BrowserName;
                objErrorLog.browserVersion = browserHelper.BrowserVersion;
                objErrorLog.exception = ex;
                objErrorLog.EntityObject = objUserLogin;
                LogHelper.GetInstance.WriteErrorLog(objErrorLog);
                throw ex;
            }
        }
        void UpdateBrowserInfo(int login_id)
        {
            UserLogin objUserLogin = new UserLogin();
            
            try
            {
                objUserLogin.client_ip = IPHelper.GetIPAddress();
                BrowserHelper browserHelper = new BrowserHelper();
                objUserLogin.browser_name = browserHelper.BrowserName;
                objUserLogin.browser_version = browserHelper.BrowserVersion;
                objUserLogin.session_id = Session.SessionID;
                objUserLogin.server_ip = IPHelper.GetServerIP();
                objUserLogin.login_id = login_id;
                new BLUserLogin().UpdateBrowserInfo(objUserLogin);
            }
            catch (Exception ex)
            {
                ErrorLogInput objErrorLog = new ErrorLogInput();
                objErrorLog.userId = Convert.ToInt32(Session["user_id"]);
                objErrorLog.UserName = User.Identity.Name;
                objErrorLog.fromPage = "Login Controller";
                objErrorLog.fromMethod = "UpdateBrowserInfo()";
                objErrorLog.clientIp = IPHelper.GetIPAddress();
                objErrorLog.serverIp = Dns.GetHostEntry(Dns.GetHostName()).HostName ?? "";
                BrowserHelper browserHelper = new BrowserHelper();
                objErrorLog.browserName = browserHelper.BrowserName;
                objErrorLog.browserVersion = browserHelper.BrowserVersion;
                objErrorLog.exception = ex;
                objErrorLog.EntityObject = objUserLogin;
                LogHelper.GetInstance.WriteErrorLog(objErrorLog);
                throw ex;
            }
        }

        public ActionResult logout()
        {
            var loginHistory = (UserLogin)Session["userLoginHistory"];
            var signOut_type ="Manual";
            int historyId = loginHistory != null ? loginHistory.history_id : 0;
            if (Request.IsAuthenticated && loginHistory!=null)
            {
                new BLUserLogin().UpdateLogOutTime(Convert.ToInt32(Session["user_id"]), historyId, loginHistory.source, signOut_type);
                new BLUserLoginHistoryInfo().UpdateUserLogOutTime(Convert.ToInt32(Session["user_id"]), historyId, signOut_type);
            }
            KillSession();
            return RedirectToAction("Index");
        }

        private void KillSession()
        {
            //Session for language we manage here due to maintain dropdown of language in login page.
            var Lang = Session["Language"];
            Session.Clear();
            Session.RemoveAll();
            FormsAuthentication.SignOut();

            // Clear the session cookie
            if (Response.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddYears(-1);
            }


            Session["Language"] = Lang;
        }
        public JsonResult checkSession()
        {

            if (Session["user_id"] != null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }

        }

        public string getUserProfileImage(int usrId, string userImgName)
        {
            
            string userImgBytes = "";
            
            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

            string imageUrl = string.Concat(FtpUrl + "/UserProfiles/", userImgName);
            try
            {
                WebClient request = new WebClient();
                if (!string.IsNullOrEmpty(UserName))
                { //Authentication require..
                    request.Credentials = new NetworkCredential(UserName, PassWord);
                }
                byte[] objdata = null;
                //Download Image from FTP..
                objdata = request.DownloadData(imageUrl);

                if (objdata != null && objdata.Length > 0)
                {
                    userImgBytes = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));
                }
            }
            catch (Exception ex)
            {
                // if any exception occurred then return default image url instead of bytes..
                // this is handled on temporary basic . Need to handle the same properly later on..
                //userImgBytes = "/Uploads/profiles/big.png";
            }
            return userImgBytes;

            //Uploads/profiles/big.png
           
        }
        [HttpPost]
        public ActionResult validateuser(string userName)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                if (!ApplicationSettings.isMultiLoginAllowed)
                {
                    User objUser = new Models.User();
                    UserLogin objUserLogin = new UserLogin();
                    //objUser = new BLUser().ChkUserExist(userName);
                    objUser = new BLUser().ValidateUser(userName, "", UserType.Web.ToString());
                    if (objUser != null)
                    {
                        var userLoginDetails = new BLUserLogin().GetUserLoginDetailById(objUser.user_id,"Web");
                        if (userLoginDetails != null)
                        {
                            if (userLoginDetails.logout_time == null)
                            {
                                objResp.message = "This user is already LogedIn in other device, Do you want to force Login?";
                                objResp.status = ResponseStatus.FAILED.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Error while Validating user!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        private string GenerateOTPToken(User usr)
        {
            Session["OTPTokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
            return ((TokenDetail)Session["OTPTokenDetail"]).access_token;
            //return "test";
        }

        private OTPResponse SendResendOTP(NewOTPRequest newOTPRequest)
        {
            Session["TokenDetail"] = (TokenDetail)Session["OTPTokenDetail"];
            OTPResponse oTPResponse = new OTPResponse();
            string url = "api/otp/getotp ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<OTPResponse>(url, newOTPRequest);
            if (response != null)
            {
                if (response.status == "OK")
                {
                    oTPResponse = response.results;
                }
            }
            return oTPResponse;
        }


        private VerifyOTPResponse ValidateOTP(ValidateOTPRequest validateOTPRequest)
        {
            Session["TokenDetail"] = (TokenDetail)Session["OTPTokenDetail"];
            VerifyOTPResponse verifyOTPResponse = new VerifyOTPResponse();
            string url = "/api/otp/verifyotp ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<VerifyOTPResponse>(url, validateOTPRequest);
            if (response != null)
            {
                if (response.status == "OK")
                {
                    verifyOTPResponse = response.results;
                }
            }
            return verifyOTPResponse;
        }

        //public ActionResult SendOtpView(User u)
        //{
        //    return View("SendOtp", u);
        //}
        [HttpPost]
        public ActionResult ProcessOTP(string button, SendOPTDeliveryOption sendOPTDeliveryOption)
        {
            if (button == "SendOTP")
            {
                SendOPTDeliveryOption osendOPTDeliveryOption = (SendOPTDeliveryOption)TempData["SendOPTDeliveryOption"];
                osendOPTDeliveryOption.otptype = sendOPTDeliveryOption.otptype;
                //if (sendOPTDeliveryOption.otptype == "mobile")
                //{
                NewOTPRequest newOTPRequest = new NewOTPRequest();
                newOTPRequest.user_id = osendOPTDeliveryOption.user_id;
                newOTPRequest.user_name = osendOPTDeliveryOption.user_name;
                newOTPRequest.source = "WEB";
                newOTPRequest.otp_channel = osendOPTDeliveryOption.otptype;
                OTPResponse oTPResponse = SendResendOTP(newOTPRequest);


                if (oTPResponse != null)
                {
                    osendOPTDeliveryOption.message = oTPResponse.message;

                    if (Convert.ToBoolean(oTPResponse.is_OTP_generated))
                    {
                        sendOPTDeliveryOption.message = oTPResponse.message;
                        sendOPTDeliveryOption.resend_otp_timer = oTPResponse.resend_otp_timer;
                        osendOPTDeliveryOption.resend_otp_timer = oTPResponse.resend_otp_timer;
                        osendOPTDeliveryOption.OTP_length = oTPResponse.OTP_length;
                    }
                    else if (oTPResponse.is_locked)
                    {
                        sendOPTDeliveryOption.message = oTPResponse.message;
                        sendOPTDeliveryOption.resend_otp_timer = oTPResponse.locked_timer;
                        osendOPTDeliveryOption.resend_otp_timer = oTPResponse.locked_timer;
                        osendOPTDeliveryOption.is_locked = oTPResponse.is_locked;
                        osendOPTDeliveryOption.OTP_length = oTPResponse.OTP_length;
                    }
                    else
                    {
                        sendOPTDeliveryOption.message = oTPResponse.message;
                        sendOPTDeliveryOption.resend_otp_timer = oTPResponse.resend_otp_timer;
                        osendOPTDeliveryOption.resend_otp_timer = oTPResponse.resend_otp_timer;
                        osendOPTDeliveryOption.is_locked = oTPResponse.is_locked;
                        osendOPTDeliveryOption.OTP_length = oTPResponse.OTP_length;
                    }
                    TempData["SendOPTDeliveryOption"] = osendOPTDeliveryOption;
                    TempData.Keep("SendOPTDeliveryOption");
                    Session["SendOtp"] = osendOPTDeliveryOption;
                    return RedirectToAction("VerifyOtp", "login");
                }
                ////Send OTP on mobile code goes here.....
                //}
                //else if (sendOPTDeliveryOption.otptype == "email")
                //{
                //    ////Send OTP on email  code goes here.....
                //}


                ///after succcessful sending the message 
                ///add session details and redirect to veryotp page

                return View("SendOtp", osendOPTDeliveryOption);
            }
            else
            {
                //call verify otp details and rest of the code          
                ValidateOTPRequest validateOTPRequest = new ValidateOTPRequest();
                validateOTPRequest.user_id = sendOPTDeliveryOption.user_id;
                validateOTPRequest.user_name = sendOPTDeliveryOption.user_name;
                validateOTPRequest.OTP = sendOPTDeliveryOption.otp;
                validateOTPRequest.source = "WEB";
                VerifyOTPResponse verifyOTPResponse = ValidateOTP(validateOTPRequest);
                if (verifyOTPResponse != null)
                {
                    sendOPTDeliveryOption.message = verifyOTPResponse.message;
                    sendOPTDeliveryOption.footer_message = verifyOTPResponse.footer_message;
                    sendOPTDeliveryOption.resend_otp_timer = verifyOTPResponse.locked_timer;
                    if (Convert.ToBoolean(verifyOTPResponse.is_verified))
                    {
                        User usr = new BLUser().GetUserDetailByUserName(sendOPTDeliveryOption.user_name);
                        LogUserIn(usr, "Web");
                        TokenDetail td = (TokenDetail)Session["OTPTokenDetail"];
                        Session["TokenDetail"] = WebAPIRequest.GetRefreshToken(td.refresh_token,td.access_token);
                        ModelState.Clear();
                        if (usr.role_id == 1) // for super Admin only...
                        {
                            return RedirectToAction("index", "main");
                        }
                        else
                        {
                            return RedirectToAction("index", "main");
                        }
                    }
                    else if (verifyOTPResponse.is_locked.HasValue && verifyOTPResponse.is_locked.Value)
                    {
                        sendOPTDeliveryOption.resend_otp_timer = verifyOTPResponse.locked_timer;
                        sendOPTDeliveryOption.is_locked = verifyOTPResponse.is_locked.Value;
                    }

                }
                sendOPTDeliveryOption.otp = string.Empty;
                return View("VerifyOtp", sendOPTDeliveryOption);
            }
        }


        public ActionResult VerifyOtp()
        {
            SendOPTDeliveryOption sendOPTDeliveryOption = (SendOPTDeliveryOption)Session["SendOtp"];
            return View("VerifyOtp", sendOPTDeliveryOption);
        }
        public ActionResult Azuread()
        {
            string fullPath = "";
            try
            {
                string idaClientId = Convert.ToString(ConfigurationManager.AppSettings["ida:ClientId"]);
                string idaTenant = Convert.ToString(ConfigurationManager.AppSettings["ida:Tenant"]);
                string idaAADInstance = Convert.ToString(ConfigurationManager.AppSettings["ida:AADInstance"]);
                string idaRedirectUri = Convert.ToString(ConfigurationManager.AppSettings["ida:RedirectUri"]) + "/login/AzureCode";
                //string idaPostLogoutRedirectUri = Request.Url.AbsoluteUri.Replace("Azuread", "AzureCode");
                fullPath = idaAADInstance + idaTenant + "/oauth2/v2.0/authorize?client_id=" + idaClientId
               //  + "&scope=openid%20offline_access%20profile&redirect_uri=" + idaPostLogoutRedirectUri + "&response_type=code";
               + "&response_type=code&redirect_uri=" + idaRedirectUri + "&scope=openid%20offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fuser.read";
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("Azuread()", "LoginController", ex, fullPath);
            }
            return Redirect(fullPath);
        }

        public ActionResult AzureCode(string code)
        {
            string username = "";
            User u = new User();
            var apiresponse = new Models.API.ApiResponse<dynamic>();
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    apiresponse = CallAzureTokenApi(code);
                    if (!string.IsNullOrEmpty(apiresponse.results) && apiresponse.status == "OK")
                    {
                        // return View();
                        username = apiresponse.results;
                        if (username != null && username != "")
                        {
                            username = username.Substring(0, username.IndexOf("@")).Trim();
                            var globalSettings = new BLGlobalSetting().GetGlobalSettings("WEB");
                            OTPAuthenticationSettings oTPAuthenticationSettings = new BLOtpAuthentication().getOtpConfigurationSetting("WEB");

                            bool is2FAuthEnabled = false;
                            bool isADOIDEnabled = false;
                            bool isPRMSEnabled = false;

                            foreach (var objSetting in globalSettings)
                            {
                                if (objSetting.key == "isADOIDEnabled")
                                {
                                    isADOIDEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                                }

                                if (objSetting.key == "isPRMSEnabled")
                                {
                                    isPRMSEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                                }
                            }
                            if (oTPAuthenticationSettings != null)
                            {
                                is2FAuthEnabled = Convert.ToBoolean(oTPAuthenticationSettings.is_otp_enabled);
                            }

                            bool is_UserAuthenticated = false;
                            u = new BLUser().ValidateUser(username, "", UserType.Web.ToString());
                            if (u != null)
                            {
                                if (u.is_active)
                                {
                                    var ADFSEndPoint = ApplicationSettings.ADFSEndPoint;
                                    if (!String.IsNullOrEmpty(ADFSEndPoint) && u != null && u.role_id != 1 && u.user_type.ToLower() == "own")
                                    {
                                        ADFSInput objADFSInput = new ADFSInput();
                                        objADFSInput.user_name = u.user_name;
                                        objADFSInput.user_email = MiscHelper.Decrypt(u.user_email);
                                        objADFSInput.password = "";
                                        objADFSInput.ADFSAutheticationBasedOn = ApplicationSettings.ADFSAutheticationBasedOn;
                                        objADFSInput.ADFSEndPoint = ApplicationSettings.ADFSEndPoint;
                                        objADFSInput.ADFSRelPartyUri = ApplicationSettings.ADFSRelPartyUri;
                                        objADFSInput.ADFSUserNamePreFix = ApplicationSettings.ADFSUserNamePreFix;
                                        ADFSDetail ADFSDetail = BLUser.AuthenticateADFS(objADFSInput);
                                        if (!string.IsNullOrEmpty(ADFSDetail.tokenId))
                                        {
                                            is_UserAuthenticated = true;
                                            Session["TokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(username), MiscHelper.EncodeTo64("").ToString());
                                        }
                                        else
                                        {
                                            if (ADFSDetail.ADFSException != null)
                                            {
                                                is_UserAuthenticated = false;
                                                ErrorLogHelper.WriteErrorLog("Index", "Login", ADFSDetail.ADFSException, objADFSInput);
                                            }
                                            ModelState.AddModelError("AzureError", ADFSDetail.errorMsg);
                                        }
                                    }
                                    else
                                    {

                                        TokenDetail tokenDetail = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(username), MiscHelper.EncodeTo64("").ToString());
                                        if (tokenDetail != null)
                                        {
                                            Session["TokenDetail"] = tokenDetail;
                                            is_UserAuthenticated = !string.IsNullOrEmpty(tokenDetail.access_token) ? true : false;
                                        }
                                        else
                                        {
                                            is_UserAuthenticated = false;
                                        }

                                    }

                                    if (is_UserAuthenticated)
                                    {
                                        u = new BLUser().GetUserDetailByUserName(username);
                                        ////for 2F Authentication
                                        if (Convert.ToBoolean(is2FAuthEnabled))
                                        {
                                            if (u.role_id != 1 || (u.role_id == 1 && Convert.ToBoolean(ApplicationSettings.is_otp_enabled_for_admin_web)))
                                            {
                                                SendOPTDeliveryOption sendOPTDeliveryOption = new SendOPTDeliveryOption();
                                                sendOPTDeliveryOption.user_id = u.user_id;
                                                sendOPTDeliveryOption.user_name = u.user_name;
                                                sendOPTDeliveryOption.mobile_number = u.mobile_number;
                                                sendOPTDeliveryOption.user_email = MiscHelper.Decrypt(u.user_email);
                                                if (!string.IsNullOrEmpty(sendOPTDeliveryOption.user_email))
                                                {
                                                    sendOPTDeliveryOption.mask_user_email = MiscHelper.MaskEmail(sendOPTDeliveryOption.user_email.ToString());
                                                }
                                                if (!string.IsNullOrEmpty(Convert.ToString(sendOPTDeliveryOption.mobile_number)))
                                                {
                                                    sendOPTDeliveryOption.mask_mobile_number = MiscHelper.MaskMobile(sendOPTDeliveryOption.mobile_number.ToString());
                                                }
                                                u.password = "";
                                                ////for flushingg the otp reset limit
                                                new BLUserOTPDetails().ResetUserOTPStatus(u.user_id, u.user_name, Convert.ToString(OTPResetType.RESET_RESEND_LIMIT_REACHED));
                                                string otpToken = GenerateOTPToken(u);
                                                if (!string.IsNullOrEmpty(otpToken))
                                                {
                                                    //sendOPTDeliveryOption.otptype = "mobile";
                                                    TempData["SendOPTDeliveryOption"] = sendOPTDeliveryOption;
                                                    return View("SendOtp", sendOPTDeliveryOption);
                                                }
                                            }
                                            else
                                            {
                                                LogUserIn(u, "Web");
                                                //Session["TokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
                                                TokenDetail td = (TokenDetail)Session["TokenDetail"];
                                                Session["TokenDetail"] = WebAPIRequest.GetRefreshToken(td.refresh_token, td.access_token);
                                                ModelState.Clear();
                                                return RedirectToAction("index", "main");

                                            }
                                        }
                                        else
                                        {
                                            LogUserIn(u,"Web");
                                            //Session["TokenDetail"] = WebAPIRequest.GetAPIToken(MiscHelper.EncodeTo64(usr.user_name), MiscHelper.EncodeTo64(usr.password).ToString());
                                            TokenDetail td = (TokenDetail)Session["TokenDetail"];
                                            Session["TokenDetail"] = WebAPIRequest.GetRefreshToken(td.refresh_token, td.access_token);
                                            ModelState.Clear();
                                            return RedirectToAction("index", "main");
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("AzureError", Resources.Resources.SI_GBL_GBL_NET_FRM_013);
                                    }
                                }
                                else
                                    ModelState.AddModelError("AzureError", Resources.Resources.SI_GBL_GBL_NET_FRM_010);
                            }
                            else
                                ModelState.AddModelError("AzureError", username+": " +Resources.Resources.SI_GBL_GBL_NET_FRM_011);
                        }
                        else
                        {
                            if (username == null || username == "")
                                ModelState.AddModelError("AzureError", "Error in fetching User Name");
                            else if (username == null)
                                ModelState.AddModelError("AzureError", "Error in fetching User Name");
                            else
                                ModelState.AddModelError("AzureError", "Error in fetching User Name");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("AzureError", apiresponse.error_message);
                    }
                }
                else
                {
                    ModelState.AddModelError("AzureError", "Error in fetching Azure Code!!");
                }
                ViewBag.langlist = new BLResources().GetResourceCultureList();
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("Azuread()", "LoginController", ex, null);
            }
            return View("Index", u);

        }
        public Models.API.ApiResponse<dynamic> CallAzureTokenApi(string code)
        {
            string userName = "";
            string DATA = "";
            var apiresponse = new Models.API.ApiResponse<dynamic>();

            ErrorLogHelper logHelper = new ErrorLogHelper();
            try
            {
                string AADinstance = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ida:AADInstance1"]);
                string tenant_id = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ida:Tenant"]);
                string URL = string.Format(AADinstance, tenant_id + "/oauth2") + "/token/";
                string client_id = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ida:ClientId"]);
                string idaRedirectUri = Convert.ToString(ConfigurationManager.AppSettings["ida:RedirectUri"]) + "/login/AzureCode";
                //string redirect_uri = Request.Url.AbsoluteUri;
                //redirect_uri = redirect_uri.Substring(0, redirect_uri.LastIndexOf("AzureCode")+9 ); 
                string grant_type = "authorization_code";
                string client_secret = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ida:client_secret"]);

                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                DATA = "client_id=" + client_id + "&code=" + code + "&redirect_uri=" + idaRedirectUri + "&grant_type=" + grant_type + "&client_secret=" + client_secret;
                request.ContentLength = DATA.Length;
                using (Stream webStream = request.GetRequestStream())
                using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                {
                    requestWriter.Write(DATA);
                }

                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string response = responseReader.ReadToEnd();
                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);

                    //Azure Token details
                    Models.TokenDetail atd = new Models.TokenDetail();
                    atd.access_token = json.access_token.ToString();
                    apiresponse = CallUserInfo(atd.access_token);
                }
            }
            catch (Exception ex)
            {
                apiresponse.results = null;
                apiresponse.status = Models.StatusCodes.UNKNOWN_ERROR.ToString();
                apiresponse.error_message = "Error in fetching token !!";
                logHelper.ApiLogWriter("CallAzureTokenApi", "LoginController", DATA, ex);
            }
            return apiresponse;
        }
        public Models.API.ApiResponse<dynamic> CallUserInfo(string access_token)
        {
            string userName = "";
            string DATA = "";
            var apiresponse = new Models.API.ApiResponse<dynamic>();
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
                    string username = json.userPrincipalName.ToString();
                    apiresponse.results = username;
                    apiresponse.status = Models.StatusCodes.OK.ToString();

                }
            }
            catch (Exception ex)
            {
                apiresponse.results = null;
                apiresponse.status = Models.StatusCodes.UNKNOWN_ERROR.ToString();
                apiresponse.error_message = "Error in fetching User Name";
                logHelper.ApiLogWriter("CallUserInfo", "LoginController", access_token, ex);
            }
            return apiresponse;
        }

        public void UserDetails()
        {
            UserInfo user = new UserInfo();
            var userInfo = new BLUser().GetAllUsersList();
            foreach (var item in userInfo)
            {
                item.user_email = Utility.MiscHelper.Encrypt(item.user_email);
                item.mobile_number = Convert.ToString(Utility.MiscHelper.Encrypt(string.IsNullOrWhiteSpace(item.mobile_number) ? "1234567891" : item.mobile_number));
                item.name = Convert.ToString(Utility.MiscHelper.Encrypt(string.IsNullOrWhiteSpace(item.name) ? "Test" : item.name));
            }
            user.lstUser = userInfo;
            var _User = new BLUser().UpdateUserInfo(user.lstUser);
        }
    }
}
