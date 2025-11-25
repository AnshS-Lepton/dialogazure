using BusinessLogics;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Utility;
using SmartInventory.Settings;
using System.Configuration;
using BusinessLogics.DaFiFeasibilityAPI;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
            try
            {
                var userdetails = (User)Session["userDetail"];
                planobj.lstSproutFiber = new BLPlan().GetBackboneFiberTypeDropDownList();
                planobj.lstBackboneFiber = new BLPlan().GetBackboneFiberTypeDropDownList();
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("ShowBackbonePlanTool", "BackBone", ex);
            }
            return PartialView("_BackbonePlanTool", planobj);
        }
        public async Task<ActionResult> SaveBackboneProcess(BackBonePlanning objPlan)
        {
            try
            {
                int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                int routeThreshold = ApplicationSettings.BackboneRouteThreshold;
                double? totalLength = objPlan.cable_length + objPlan.sprout_route_length;
                  new BLPlan().getUpdateBackbonePlan(objPlan.create_plan,objPlan.plan_id);

                if (user_id != 0)
                {
                    objPlan.created_by = user_id;

                    if (totalLength > routeThreshold)
                    {
                        // Set early response message
                        objPlan.objPM.message = "The backbone planning process has started. You will be notified once it is completed and logged in the history.";
                        objPlan.objPM.status = "Success";

                        // Run planning in background (fire-and-forget)
                        _ = Task.Run(() => new BLPlan().saveBackbonePlanning(objPlan));

                        // Return early response
                        return Json(objPlan, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // Run immediately for short routes
                        var objResp = new BLPlan().saveBackbonePlanning(objPlan);

                        objPlan.objPM.message = objResp[0].message;
                        objPlan.objPM.status = objResp[0].status.ToString();
                        objPlan.plan_id = objResp[0].v_plan_id;

                        return Json(objPlan, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("SaveBackboneProcess", "BackBone", ex);
                objPlan.objPM.message = "An error occurred while saving the plan.";
                objPlan.objPM.status = "Error";
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
        public ActionResult DeleteSproutPlanById(int planId,string networkId)
        {
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var objResp = new BLPlan().DeleteSproutPlanById(planId, user_id, networkId);           
            if (objResp[0].status)
            {
                return Json(new { strReturn = objResp[0].message, msg = "OK" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { strReturn = objResp[0].message, msg = "false" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ConvertToAsbuiltNetwork(int planId)
        {
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var objResp = new BLPlan().ConvertToAsbultBackboneNetwork(planId, user_id);
            if (objResp[0].status)
            {
                return Json(new { message = objResp[0].message, status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = objResp[0].message, status = "false" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ConvertToPlannedNetwork(int planId)
        {
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var objResp = new BLPlan().ConvertToPlannedBackboneNetwork(planId, user_id);
            if (objResp[0].status)
            {
                return Json(new { message = objResp[0].message, status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = objResp[0].message, status = "false" }, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult GetBackbonePlanHistoryData(ModelBackbonePlanningDetails objfiledetail)
        {
            try
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
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetBackbonePlanHistoryData", "BackBone", ex);
            }
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
                ErrorLogHelper.WriteErrorLog("GetBackbonePlanDetails", "BackBone", ex);
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
                ErrorLogHelper.WriteErrorLog("GetBackboneForMap", "BackBone", ex);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBackboneNearestSiteList(string geom, double buffer, int planId)
        {
            BackBoneSitePlanDetails backBoneSitePlanDetails = new BackBoneSitePlanDetails();
            try { 
                
             int userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);          
             backBoneSitePlanDetails = new BLPlan().GetNearestSiteList(geom, buffer, planId);           
             backBoneSitePlanDetails.sites = backBoneSitePlanDetails.sites.ToList();
             backBoneSitePlanDetails.lstSproutFiber = new BLPlan().GetBackboneFiberTypeDropDownList();
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetBackboneNearestSiteList", "BackBone", ex);
            }
            return PartialView("_SiteList", backBoneSitePlanDetails);
        }
        public ActionResult GetBackboneSiteHistory(int planId)
        {
            BackBoneSitePlanDetails backBoneSitePlanDetails = new BackBoneSitePlanDetails();
            try { 
                backBoneSitePlanDetails.sites = new BLPlan().getNearestSiteHistoryList(planId);
                backBoneSitePlanDetails.plan_id = planId;
                backBoneSitePlanDetails.lstSproutFiber = new BLPlan().GetBackboneFiberTypeDropDownList();
                foreach (var item in backBoneSitePlanDetails.sites)
                {
                    var matchedFiberType = backBoneSitePlanDetails.lstSproutFiber.DefaultIfEmpty().FirstOrDefault(x => x.dropdown_value == item.fibertype);
                    if (matchedFiberType != null)
                    {
                        item.fibertype = matchedFiberType.dropdown_type.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetBackboneSiteHistory", "BackBone", ex);
            }
            return PartialView("_SiteHistory", backBoneSitePlanDetails);
        }

        public JsonResult GetBackboneNearestSiteBuffer(string geom, double buffer, int planId = 0)
        {
            JsonResponse<dynamic> objResp = new JsonResponse<dynamic>();
            try { 
            BackBoneSitePlanDetails backBoneSitePlanDetails = new BLPlan().GetNearestSiteList(geom, buffer, planId);          
            objResp.result = backBoneSitePlanDetails;
            objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetBackboneNearestSiteBuffer", "BackBone", ex);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBackbonePlanningEntityList(BackBonePlanning plan)
        {
            JsonResponse<dynamic> objResp = new JsonResponse<dynamic>();
            try
            {
                int userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                List<BackBoneSproutFiberDetails> nearestSiteLst = new BLPlan().GetBackbonePlanningList(plan, userId);              
                objResp.status = ResponseStatus.OK.ToString();
                plan.plan_id = nearestSiteLst[0].plan_id;
                objResp.result = plan;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetBackbonePlanningEntityList", "BackBone", ex);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetBomBOQData(BackBonePlanning model)
        {
            List<BackBoneBOMOBOQResponse> bomList = new List<BackBoneBOMOBOQResponse>();
            try
            {
                int userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                bomList = new BLPlan().BackBonePlanBom(model, userId);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetBomBOQData", "BackBone", ex);
            }
            return PartialView("_BomBoqList", bomList);
        }
        public JsonResult GetDraftLineGeometry(int planId)
        {    
            int userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var geometryList = new BLPlan().BackBonePlanDraftLineGeometry(planId, userId);
            var detailedList = geometryList.Select(r => JsonConvert.DeserializeObject<siteLineGeometry>(r.geojson)).ToList();
            return Json( geometryList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBackBonePlanningLineLength(string geom)
        {
            JsonResponse<dynamic> objResp = new JsonResponse<dynamic>();
            try
            {
                objResp.result = new BLPlan().GetLineLength(geom);
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetBackBonePlanningLineLength", "BackBone", ex);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveLoop(List<BackbonePlanNetworkDetails> model)
        {
            new BLPlan().UpdateBackBoneLoopLength(model);
            return PartialView("_LoopManage", model);
        }
        public PartialViewResult GetLoopManage(int planid, double looplength, bool is_loop_updated,string line_geom, double loopSpan)
        {
            List < BackbonePlanNetworkDetails > list = new List<BackbonePlanNetworkDetails>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                list = new BLPlan().GetBackBoneLoopList(planid, usrDetail.user_id, is_loop_updated, line_geom, loopSpan, looplength);

                if (!is_loop_updated)
                {
                    list.ForEach(x => x.loop_length = looplength);
                }
            }catch(Exception ex )
            {
                ErrorLogHelper.WriteErrorLog("GetLoopManage", "BackBone", ex);
            }
            return PartialView("_LoopManage", list);
        }
        public JsonResult getLoopLength(int plan_id,string sproutType,string backboneType,string geometry, bool isCreateDuct,bool isCreateTrench)
        {         
            JsonResponse<BackBoneBOMOBOQResponse> objResp = new JsonResponse<BackBoneBOMOBOQResponse>();
            BackBonePlanning model = new BackBonePlanning
            {
                plan_id = plan_id,
                sprout_fiber_type = sproutType,
                backbone_fiber_type = backboneType,
                geometry = geometry,
                is_create_duct = isCreateDuct,
                is_create_trench = isCreateTrench
            };
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                    if (user_id != 0)
                    {
                        objResp.result = new BLPlan().BackBonePlanBom(model, user_id).FirstOrDefault(x => x.entity_type == "Loop");
                    }
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
                ErrorLogHelper.WriteErrorLog("getLoopLength", "BackBone", ex);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveNearestSites(BackBoneSitePlanDetails model)
        {
            try
            {               
                int userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                model.sites.ForEach(s => s.user_id = userId);
                string mapKey = ConfigurationManager.AppSettings["MapKeyBackend"].Trim();
                List<BackBoneSproutFiberDetails> nearestSiteLst = new BLPlan().SaveNearestSite(model.sites, userId);
                foreach (var sp_geom in nearestSiteLst)
                {
                    if (string.IsNullOrWhiteSpace(sp_geom.site_to_nearestcable_line_geom))
                    {
                        var lst = GoogleDirectionsServiceHelper.GetRouteGeoJsonAndLength(sp_geom.intersect_line_geom, sp_geom.site_geom, mapKey);

                        if (lst.Result.LengthInMeters < 1)
                        {
                            string[] startGeomParts = sp_geom.intersect_line_geom.Split(',');
                            string[] siteGeomParts = sp_geom.site_geom.Split(',');

                            string lineGeom = startGeomParts[1] + " " + startGeomParts[0] + "," + siteGeomParts[1] + " " + siteGeomParts[0];

                            new BLPlan().updateSiteLineGeometry(lineGeom, sp_geom.id, sp_geom.plan_id, userId);
                        }
                        else
                        {
                            var newbuilt = JsonConvert.DeserializeObject<GeoJsonLineString>(lst.Result.GeoJson);
                            string lineGeom = string.Empty;
                            string[] startGeomParts = sp_geom.intersect_line_geom.Split(',');
                            string[] siteGeomParts = sp_geom.site_geom.Split(',');
                            lineGeom = startGeomParts[1] + " " + startGeomParts[0] + ",";
                            foreach (var cordinates in newbuilt.coordinates)
                            {
                                lineGeom += cordinates[0].ToString() + " " + cordinates[1].ToString() + ",";
                            }
                            lineGeom += siteGeomParts[1] + " " + siteGeomParts[0];
                            lineGeom = lineGeom.TrimEnd(',');
                            new BLPlan().updateSiteLineGeometry(lineGeom, sp_geom.id, sp_geom.plan_id, userId);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("SaveNearestSites", "BackBone", ex);
            }
            return Json(new { message = "Sprout Route Saved successfully!" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateSiteRoute(string geom, int planId, int p_systemId)
        {
            var SitePlanList = new SitePlanList();
            try
            {
                int userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                 SitePlanList = new BLPlan().updateSiteRoute(geom, planId, p_systemId, userId);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("UpdateSiteRoute", "BackBone", ex);
            }
            return Json(new { status = true, SpLength = SitePlanList.sprout_route_length, spTotalLength = SitePlanList.total_sp_route_length }, JsonRequestBehavior.AllowGet);
        }        

    }
}