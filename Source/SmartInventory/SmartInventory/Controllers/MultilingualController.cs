using BusinessLogics;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SmartFeasibility.Multilingual
{
    //[Authorize]
    //[SessionExpire]
   //[HandleException]
    public class MultilingualController : Controller
    {
        // GET: Multilingual
        public ActionResult Index()
        {
            return View();
        }

        public void Change(string lang)
        {
            if (lang != null)
            {

                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                 Session["Language"] = lang;
                Session["fontName"]  = new BLResources().GetCultureFont(lang);
            }

            //HttpCookie cookie = new HttpCookie("Language");
            //cookie.Value = lang;
            //Response.Cookies.Add(cookie);
            //Response.Cookies.Add(cookie);

        }
    }
}