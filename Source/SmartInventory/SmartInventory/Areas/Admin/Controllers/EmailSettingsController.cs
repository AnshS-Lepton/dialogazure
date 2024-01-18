using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics;
using Models;
using Models.Admin;

namespace SmartInventory.Areas.Admin.Controllers
{
    public class EmailSettingsController : Controller
    {
        // GET: Admin/EmailSettings
        public ActionResult Index()
        {
            EmailSettingsModel obj = new EmailSettingsModel();
            obj = new BLMisc().getEmailSettings();

            return View("_EmailSettings", obj);
        }
        public ActionResult UpdateEmailSettings(EmailSettingsModel _objEmailSettingsModel)
        {
            PageMessage objMsg = new PageMessage();
            int user_id = _objEmailSettingsModel.id;
            _objEmailSettingsModel = new BLMisc().updateEmailSettings(_objEmailSettingsModel, user_id);
            if (_objEmailSettingsModel != null)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Email Settings Updated Successfully";
            }
            else
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Error";
            }
            _objEmailSettingsModel.pageMsg = objMsg;
            return View("_EmailSettings", _objEmailSettingsModel);
        }
    }
}