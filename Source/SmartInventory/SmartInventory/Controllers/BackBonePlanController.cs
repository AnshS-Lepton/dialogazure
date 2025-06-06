using BusinessLogics;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;
using SmartInventory.Settings;
using System.Configuration;
using BusinessLogics.DaFiFeasibilityAPI;
using iTextSharp.tool.xml.html;
using Newtonsoft.Json;
using System.Web.Helpers;
using NPOI.SS.Formula.Functions;
using static Mono.Security.X509.X520;
using System.Windows.Media;
using SharpKml.Dom;

namespace SmartInventory.Controllers
{
    public class BackBonePlanController : Controller
    {
     
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowBackbonePlanTool()
        {
            BackBonePlanning planobj = new BackBonePlanning();
            List<DisplayPlan> t = new List<DisplayPlan>();
            var userdetails = (User)Session["userDetail"];
            planobj.lstSproutFiber = new BLPlan().GetBackboneFiberTypeDropDownList();
            planobj.lstBackboneFiber = new BLPlan().GetBackboneFiberTypeDropDownList();
            return PartialView("_BackbonePlanTool", planobj);
        }
        public ActionResult SaveBackboneProcess(BackBonePlanning objPlan)
        {

                int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                if (user_id != 0)
                {

                    objPlan.created_by = user_id;
                    objPlan.backbone_fiber = objPlan.backbone_fiber.TrimStart('(');
                    objPlan.sprout_fiber = objPlan.sprout_fiber.TrimStart('(');
                    var objResp = new BLPlan().saveBackbonePlanning(objPlan);
                    objPlan.objPM.message = objResp[0].message;
                    objPlan.objPM.status = objResp[0].status.ToString();
                    objPlan.plan_id = objResp[0].v_plan_id;
                }

            return Json(objPlan, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBackbonePlanByPlanId(int plan_id)
        {
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var objResp = new BLPlan().DeleteBackbonePlanByPlanId(plan_id, user_id);
            objResp[0].message = BLConvertMLanguage.MultilingualMessageConvert(objResp[0].message);
            if (objResp[0].status)
            {
                return Json(new { strReturn = objResp[0].message, msg = "OK" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { strReturn = objResp[0].message, msg = "false" }, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult GetBackbonePlanHistoryData(ModelBackbonePlanningDetails objfiledetail)
        {
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            objfiledetail.objPlanDataFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objfiledetail.objPlanDataFilter.currentPage = 1;
            objfiledetail.objPlanDataFilter.sort = "";
            objfiledetail.objPlanDataFilter.orderBy = "";
            var fileList = new BLPlan().GetBackbonePlanHistoryDetails(objfiledetail.objPlanDataFilter, user_id);
            string Filename = string.Empty;
            foreach (var item in fileList)
            {
                Filename = item.plan_name;
                item.created_on = MiscHelper.FormatDateTime(item.created_on.ToString());
            }
            objfiledetail.lstPlanDetails = fileList;
            return PartialView("_BackboneViewPlanData", objfiledetail);
        }
        public JsonResult GetBackbonePlanDetails(int plan_id)
        {
            JsonResponse<BackBonePlanning> objResp = new JsonResponse<BackBonePlanning>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    var usrId = usrDetail.user_id;
                    objResp.result = new BLPlan().GetBackbonePlanningById(plan_id);
                    objResp.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_145;
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_146;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBackboneForMap(int plan_id)
        {
            JsonResponse<BackBonePlanning> objResp = new JsonResponse<BackBonePlanning>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    var usrId = usrDetail.user_id;
                    objResp.result = new BLPlan().GetBackboneForMap(plan_id);
                    objResp.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_145;
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_146;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBackboneNearestSiteList(string geom, double buffer,string startPointNetworkId,string endPointNetworkId)
        {
            JsonResponse<dynamic> objResp = new JsonResponse<dynamic>();
            var siteLst = new BLPlan().GetNearestSiteList(geom, buffer);
            var filterSiteLst = siteLst.sites.Where(s =>
              (string.IsNullOrEmpty(startPointNetworkId) || s.common_name != startPointNetworkId) &&
              (string.IsNullOrEmpty(endPointNetworkId) || s.common_name != endPointNetworkId));
            siteLst.sites = filterSiteLst.ToList();
            objResp.result = siteLst;
            objResp.status = ResponseStatus.OK.ToString();
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBackbonePlanningEntityList(BackBonePlanning plan)
        {
            JsonResponse<dynamic> objResp = new JsonResponse<dynamic>();
            var geoJsonFeatures = new List<string>();
            int userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            string mapKey = ConfigurationManager.AppSettings["MapKeyBackend"].Trim();
            List<BackBoneSproutFiberDetails> nearestSiteLst = new BLPlan().GetBackbonePlanningList(plan, userId);
            if (nearestSiteLst[0].site_geom != null && nearestSiteLst.Count > 0)
            {
                foreach (var sp_geom in nearestSiteLst)
                {
                    var lst = GoogleDirectionsServiceHelper.GetRouteGeoJsonAndLength(sp_geom.intersect_line_geom, sp_geom.site_geom, mapKey);

                    if (lst.Result.LengthInMeters < 1)
                    {
                        string[] startGeomParts = sp_geom.intersect_line_geom.Split(',');
                        string[] siteGeomParts = sp_geom.site_geom.Split(',');

                        string lineGeom = startGeomParts[1] + " " + startGeomParts[0] + "," + siteGeomParts[1] + " " + siteGeomParts[0];

                        new BLPlan().updateSiteLineGeometry(lineGeom, sp_geom.system_id, lst.Result.LengthInMeters, plan.threshold, sp_geom.plan_id);
                    }
                    else
                    {
                        var newbuilt = JsonConvert.DeserializeObject<GeoJsonLineString>(lst.Result.GeoJson);
                        string lineGeom = string.Empty;
                        string[] startGeomParts = sp_geom.intersect_line_geom.Split(',');
                        string[] siteGeomParts = sp_geom.site_geom.Split(',');
                        lineGeom = startGeomParts[1] + " " + startGeomParts[0]+",";
                        foreach (var cordinates in newbuilt.coordinates)
                        {
                            lineGeom += cordinates[0].ToString() + " " + cordinates[1].ToString() + ",";
                        }
                        lineGeom += siteGeomParts[1] + " " + siteGeomParts[0];
                        lineGeom = lineGeom.TrimEnd(',');
                        new BLPlan().updateSiteLineGeometry(lineGeom, sp_geom.system_id, lst.Result.LengthInMeters, plan.threshold, sp_geom.plan_id);
                    }
                }
            }

            objResp.status = ResponseStatus.OK.ToString();
            plan.plan_id = nearestSiteLst[0].plan_id ;
            objResp.result = plan;
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetBomBOQData(BackBonePlanning model)
        {
            int userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            List<BackBoneBOMOBOQResponse> bomList = new List<BackBoneBOMOBOQResponse>();            
            bomList = new BLPlan().BackBonePlanBom(model, userId);
            return PartialView("_BomBoqList", bomList);
        }
        public JsonResult GetDraftLineGeometry(int planId)
        {
            int userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var geometryList = new BLPlan().BackBonePlanDraftLineGeometry(planId, userId);
            var detailedList = geometryList.Select(r => JsonConvert.DeserializeObject<siteLineGeometry>(r.geojson)).ToList();
            return Json( geometryList, JsonRequestBehavior.AllowGet);
        }
    }
}