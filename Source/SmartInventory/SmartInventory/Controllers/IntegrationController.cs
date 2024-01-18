using BusinessLogics;
using Models;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class IntegrationController : Controller
    {
        // GET: Integration
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// returns the partial view result of SmartPlanner Integration Pop-up
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowPlannerIntegration()
        {
            List<DisplayPlan> plans = new BLIntegrationSuperSet().GetUniquePlans();
            return PartialView("_PlannerIntegration", plans);
        }

        /// <summary>
        /// Processes the plan
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        public ActionResult ProcessIntegrationSuperset(int planID)
        {
            var response = new BLIntegrationSuperSet().ProcessIntegrationSuperset(planID);
            response[0].message = BLConvertMLanguage.MultilingualMessageConvert(response[0].message);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// returns all plans
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllPlans()
        {
            List<DisplayPlan> plans = new BLIntegrationSuperSet().GetUniquePlans();
            return PartialView("_PlansList", plans);
        }

        /// <summary>
        /// deleted the processed plan
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        public ActionResult DeleteProcessedIntegrationSuperset(int planID)
        {
            var response = new BLIntegrationSuperSet().DeleteProcessedIntegrationSuperset(planID);
            response[0].message = BLConvertMLanguage.MultilingualMessageConvert(response[0].message);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// returns the region, province, POD lat, lng, and layers
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        public ActionResult GetPlanRegionProvince(int planID)
        {
            var response = new BLIntegrationSuperSet().GetPlanRegionProvince(planID);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}