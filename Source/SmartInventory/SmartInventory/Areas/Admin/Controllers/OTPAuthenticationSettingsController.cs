using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics;
using Models;
using Models.Admin;
using Models.API;

namespace SmartInventory.Areas.Admin.Controllers
{
    public class OTPAuthenticationSettingsController : Controller
    {
        // GET: Admin/OTPAuthenticationSettings
        public ActionResult Index()
        {
            return View("OTPAuthenticationSettings");
        }
        public ActionResult Msg()
        {
            return View("OtpMessageSetting");
        }
        [HttpPost]
        public ActionResult getOtpConfigurationSetting(string applicationType = "")
        {
            //ModelState.Clear();
            OTPAuthenticationSettings obj = new OTPAuthenticationSettings();
            BLOtpAuthentication Blobj = new BLOtpAuthentication();
            obj = Blobj.getOtpConfigurationSetting(applicationType);
            return PartialView("_OTPAuthenticationSettings", obj);
        }
        [HttpPost]
        public ActionResult updateOtpConfigurationSetting(OTPAuthenticationSettings OTPAuthenticationSetting)
        {
            PageMessage objMsg = new PageMessage();
            BLOtpAuthentication BLOtpAuthentication = new BLOtpAuthentication();
            if (OTPAuthenticationSetting != null)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "OTP Configuration Updated Successfully";
            }
            else
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Error";
            }
            //OTPAuthenticationSetting.pageMsg = objMsg;
            var applicationType = OTPAuthenticationSetting.application_name;
            var updatedData = BLOtpAuthentication.updateOtpConfigurationSetting(OTPAuthenticationSetting, applicationType);
            updatedData.pageMsg = objMsg;
            return View("OTPAuthenticationSettings", updatedData);
        }

       

        //public ActionResult EmailConfiguration(string EmailEvent = "",EmailBinder objEmailBinder = null)
        //{
        //    List<EmailEventTemplate> objEmailEventTemplates = new List<EmailEventTemplate>();
        //    BLOtpAuthentication Blobj = new BLOtpAuthentication();
        //    objEmailEventTemplates = Blobj.GetEmailTemplateDetail();
        //   // EmailBinder objEmailBinder = new EmailBinder();
        //   // string EmailEvent = objEmailBinder.trigger_event;
        //    objEmailBinder.templatelist = objEmailEventTemplates.Select(o => new EmailTemplate
        //    {
        //        trigger_event = o.trigger_event
        //    }).ToList();
        //    if(!string.IsNullOrEmpty(EmailEvent))
        //    {
        //        objEmailBinder.objEmailEventTemplate = objEmailEventTemplates.Where(x => x.trigger_event == EmailEvent).FirstOrDefault();
        //    }


        //    return View("EmailTemplateConfiguration",objEmailBinder);
        //}

        public ActionResult EmailConfiguration(EmailBinder objEmailBinder = null,string submitButton="")
        {
            
            ViewBag.SuccessMessage = "";
            if (submitButton == "Search")
            {
                ModelState.Clear();
                ViewBag.SuccessMessage = "";
                objEmailBinder.objEmailEventTemplate = null;
                BindEmailConfigurationPage(objEmailBinder);
                return View("EmailTemplateConfiguration", objEmailBinder);
            }
            else if (submitButton == "Update")            
            {
                string message  = UpdateEmailConfiguration(objEmailBinder.objEmailEventTemplate);
                ViewBag.SuccessMessage  = message;
                objEmailBinder.objEmailEventTemplate = null;
                BindEmailConfigurationPage(objEmailBinder);
                return View("EmailTemplateConfiguration", objEmailBinder);
            }
            else
            {
                ModelState.Clear();
                ViewBag.SuccessMessage = "";
                objEmailBinder.templatelist = null;
                objEmailBinder.trigger_event = null;
                objEmailBinder.objEmailEventTemplate = null;
                objEmailBinder.objEmailEventTemplate = null;
                BindEmailConfigurationPage(objEmailBinder);
                return View("EmailTemplateConfiguration", objEmailBinder);
            }

            
        }

        public void BindEmailConfigurationPage(EmailBinder objEmailBinder)
        {
            List<EmailEventTemplate> objEmailEventTemplates = new List<EmailEventTemplate>();
            BLOtpAuthentication Blobj = new BLOtpAuthentication();
            objEmailEventTemplates = Blobj.GetEmailTemplateDetail();
            // EmailBinder objEmailBinder = new EmailBinder();
            string EmailEvent = objEmailBinder.trigger_event;
            objEmailBinder.templatelist = objEmailEventTemplates.Select(o => new EmailTemplate
            {
                trigger_event = o.trigger_event
            }).ToList();
            if (!string.IsNullOrEmpty(EmailEvent))
            {
                objEmailBinder.objEmailEventTemplate = objEmailEventTemplates.Where(x => x.trigger_event == EmailEvent).FirstOrDefault();
            }
        }
       
        public string   UpdateEmailConfiguration(EmailEventTemplate objEmailEventTemplate)
        {
            try
            {

                if (objEmailEventTemplate != null)
                {
                    BLOtpAuthentication BLOtpAuthentication = new BLOtpAuthentication();
                    if (objEmailEventTemplate != null)
                    {

                        var updatedData = BLOtpAuthentication.updateEmailConfigurationSetting(objEmailEventTemplate);
                    }

                }
                return "Updated Successfully";
            }
            catch(Exception ex)
            {
                return "Some error occured while updating";
            }              
                      
        }
    }
}