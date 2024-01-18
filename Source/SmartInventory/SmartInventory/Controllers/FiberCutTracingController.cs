using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Data;
using Utility;
using BusinessLogics;
namespace SmartInventory.Controllers
{
    public class FiberCutTracingController : Controller
    {
        public ActionResult index()
        {
            return PartialView("index");
        }
        public ActionResult getFiberTracingPath(int systemId, string entityType, int portNo, string nodeType)
        {
            var fiberPath = BLFiberCutTracing.Instance.getTracingPath(systemId, entityType, portNo, nodeType);
            return Json(fiberPath, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getFiberCutDetails(int systemId, string entityType, int portNo, double distance, string nodeType,bool isBackWordPath)
        {
            FiberCutDetails objCutDetails = new FiberCutDetails();
            objCutDetails = BLFiberCutTracing.Instance.getFiberCutDetails(systemId, entityType, portNo, distance, nodeType, isBackWordPath);
            return PartialView("_FiberCutDetails", objCutDetails);
        }
        public ActionResult GetEquipmentSearchResult(string SearchText)
        {
            BLOSPSplicing objSplicing = new BLOSPSplicing();
            List<EquipementSearchResult> lstEquipment = new List<EquipementSearchResult>();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                lstEquipment = objSplicing.GetSearchEquipmentResult(SearchText, Convert.ToInt32(Session["user_id"]));
            }
            return Json(lstEquipment, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getFiberNodeType(int systemId, string entityType)
        {
            var nodeList = BLFiberCutTracing.Instance.getFiberNodeType(systemId, entityType);
            return Json(nodeList, JsonRequestBehavior.AllowGet);
        }
    }
}