using BusinessLogics;
using Lepton.Utility;
using Models;
using System;
using System.Web.Mvc;
using Utility;

namespace SmartInventory.Controllers
{
    public class CommonPathFinderController : Controller
    {
        // GET: CommonPathFinder
        public ActionResult CommonPathFinder()
        {
            return PartialView("_CommonPathFinder");
        }
        public JsonResult GetCableListByLinkIds(string linkIds)
        {
            try
            {
                var lst = new BLSite().getCablesByLinkIds(linkIds);

                //Construct the query string
                string queryString = $"?linkIds={linkIds}";
                string url = "api/FiberLink/GetFiberLinksByLinkIds" + queryString;

                //Call the Api to get the Fiber Links
                string fiberLinksData = WebAPIRequest.GetAPIRequest(url);

                return Json(new { status = "OK", data = lst, fiberData = fiberLinksData });
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetCableListByLinkIds()", "CommonPathFinder", ex);
                return Json(new { status = ResponseStatus.ERROR.ToString(), data = "", fiberData = "" });
            }
        }
        public JsonResult validateLinkIds(string linkIds)
        {
            var lst = new BLSite().validateLinkIds(linkIds);

            //return Json(new { status = "OK", data = lst });
            return Json(new { data = lst }, JsonRequestBehavior.AllowGet);
        }
    }
}