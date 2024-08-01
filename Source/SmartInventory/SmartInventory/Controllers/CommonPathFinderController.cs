using BusinessLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartInventory.Controllers
{
    public class CommonPathFinderController : Controller
    {
        // GET: CommonPathFinder
        public ActionResult CommonPathFinder()
        {
            return PartialView("_CommonPathFinder");
        }
        public JsonResult getCableListByLinkIds(string linkIds)
        {
            var lst = new BLSite().getCablesByLinkIds(linkIds);

            return Json(new { status = "OK", data = lst });

        }
    }
}