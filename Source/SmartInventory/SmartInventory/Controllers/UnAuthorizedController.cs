using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartInventory.Controllers
{
    public class UnAuthorizedController : Controller
    {
        //
        // GET: /UnAuthorized/
        public ActionResult Index()
        {
            return View();
        }
	}
}