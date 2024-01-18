using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using SmartFeasibility.Filters;
using SmartFeasibility.Helper;
using SmartFeasibility.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SmartFeasibility.Controllers
{
    [AllowAnonymous]
    [HandleException]
    public class LoginController : Controller
    {
        // GET: Login
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            string cookie = "";
            if (Session["Language"] !=null)
            {
                cookie = Session["Language"].ToString();
            }
            ViewBag.cultureInfo = cookie;
            //ViewBag.cultureInfo = CultureInfo.CurrentCulture.Name;


            User user = new User();
            if (Request.IsAuthenticated && Session["user_id"] != null)
            {
                //redirect to page on the bases of user role...
                return RedirectToAction("index", "main");
            }
            else
            {
                // check if it has been redirected from SI
                HttpContext ctx = System.Web.HttpContext.Current;

                if (ctx != null && ctx.Request.UrlReferrer != null)
                {
                    var referrer = ctx.Request.UrlReferrer.ToString();
                    var SIReferrer = ApplicationSettings.SIReferrer;
                   

                    if (!string.IsNullOrEmpty(referrer) && referrer.StartsWith(SIReferrer) && !string.IsNullOrEmpty(ctx.User.Identity.Name))
                    {
                        var userName = ctx.User.Identity.Name;
                        // login user
                        // user = new BLUser().ChkUserExist(userName);
                        user = new BLUser().ValidateUser(userName,"", UserType.Web.ToString());
                        if (user != null)
                        {
                            if (user.user_id > 0)
                            {
                                if (user.is_active)
                                {
                                    LogUserIn(user);
                                    ModelState.Clear();
                                    return RedirectToAction("index", "main");
                                }
                                else
                                {
                                    ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_NET_FRM_007);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_NET_FRM_007);
                            }
                            user.password = "";
                           
                            return View(user);
                        }
                    }
                }
                ViewBag.langlist = new BLResources().GetResourceCultureList();
                return View();
            }
        }

        [HttpPost]
        public ActionResult Index(User usr, string returnUrl)
        {
            if (usr.user_name != null && usr.password != null && usr.user_name != "" && usr.password != "")
            {
                User u = new User();
                //  u = new BLUser().ChkUserExist(usr.user_name);
                u = new BLUser().ValidateUser(usr.user_name,"", UserType.Web.ToString());
                var ADFSEndPoint = ApplicationSettings.ADFSEndPoint;
                if (!String.IsNullOrEmpty(ADFSEndPoint) && u != null && u.role_id != 1)
                {
                    if (u != null)
                    {
                        if (u.is_active)
                        {
                            ADFSInput objADFSInput = new ADFSInput();
                            objADFSInput.user_name = u.user_name;
                            objADFSInput.user_email = Utility.MiscHelper.Decrypt(u.user_email);
                            objADFSInput.password = usr.password;
                            objADFSInput.ADFSAutheticationBasedOn = ApplicationSettings.ADFSAutheticationBasedOn;
                            objADFSInput.ADFSEndPoint = ApplicationSettings.ADFSEndPoint;
                            objADFSInput.ADFSRelPartyUri = ApplicationSettings.ADFSRelPartyUri;
                            objADFSInput.ADFSUserNamePreFix = ApplicationSettings.ADFSUserNamePreFix;
                            ADFSDetail ADFSDetail = BLUser.AuthenticateADFS(objADFSInput);
                            if (!string.IsNullOrEmpty(ADFSDetail.tokenId))
                            {
                                bool HasAccessToModule = ModuleAccessHelper.FeasibilityModuleAccess(u);
                                if (HasAccessToModule)
                                {
                                    LogUserIn(u);
                                ModelState.Clear();
                                return RedirectToAction("index", "main");
                                }
                                else
                                    ModelState.AddModelError("Error", "User doesn't have the rights to Feasibility Module!");
                            }
                            else
                                ModelState.AddModelError("Error", ADFSDetail.errorMsg);
                        }
                        else
                            ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_NET_FRM_007);

                    }
                    else
                        ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_NET_FRM_008);
                }
                else
                {
                    u = new BLUser().ValidateUser(usr.user_name, Utility.MiscHelper.EncodeTo64(usr.password),UserType.Web.ToString());
                    if (u != null)
                    {
                        if (u.user_id > 0)
                        {
                            if (u.is_active)
                            {
                                bool HasAccessToModule = ModuleAccessHelper.FeasibilityModuleAccess(u);
                                if (HasAccessToModule)
                                {
                                    LogUserIn(u);
                                    ModelState.Clear();
                                    if (u.role_id == 1) // for super Admin only...
                                    {
                                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                                    }
                                    else
                                    {
                                        return RedirectToAction("index", "main");
                                    }
                                }
                                else
                                    ModelState.AddModelError("Error", "User doesn't have the rights to Feasibility Module!");
                            }
                            else
                                ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_NET_FRM_007);
                        }
                        else
                            ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_NET_FRM_008);
                    }
                    else
                        ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_NET_FRM_006);
                }
            }
            else
            {
                if (usr.user_name == null || usr.user_name == "")
                    ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_GBL_FRM_001);
                else if (usr.password == null || usr.password == "")
                    ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_GBL_FRM_002);
                else
                    ModelState.AddModelError("Error", Resources.Resources.SF_GBL_GBL_NET_FRM_006);
            }
            usr.password = "";
            ViewBag.langlist = new BLResources().GetResourceCultureList();
            return View(usr);
        }

        private void LogUserIn(User user)
        {
            // ?????????????????????? save login history
            //SaveLoginHistory(user.user_id); 
            Session["user_id"] = user.user_id;
            //get user image ???????
            //user.userImgBytes = getUserProfileImage(user.user_id, user.user_img);
            Session["userDetail"] = user;
            FormsAuthentication.SetAuthCookie(user.user_name, false);
        }

        public ActionResult logout()
        {
            string MaltilingualLag = "";
            //if (Request.IsAuthenticated)
            //    new BLUserLogin().UpdateLogOutTime(Convert.ToInt32(Session["user_id"]));
            if (Session["Language"] != null)
            {
                MaltilingualLag = Session["Language"].ToString();
            }
            else {
                MaltilingualLag = "En";
            }
            KillSession();
            Session["Language"] = MaltilingualLag;
            return RedirectToAction("Index");

        }

        private void KillSession()
        {
            Session.Clear();
            Session.RemoveAll();
            FormsAuthentication.SignOut();
        }
    }
}