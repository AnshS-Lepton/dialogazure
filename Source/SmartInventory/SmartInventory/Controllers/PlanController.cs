using BusinessLogics;
using DataAccess;
using DataAccess.Admin;
using Models;
using Models.Admin;
using Newtonsoft.Json;
using SmartInventory.Filters;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class PlanController : Controller
    {

        public JsonResult chk_end_buffer(double end_point_buffer)
        {
            //Commented by Ram Date: 04-Sep-2021
            //var AutoPlanEndBuffer = new BLGlobalSetting().GetGlobalSettings().Where(x => x.key.ToUpper() == "AutoPlanEndBufferPoint".ToUpper()).FirstOrDefault();

            //  double minbuffer = AutoPlanEndBuffer.min_value;

            //  double maxbuffer = AutoPlanEndBuffer.max_value;

            if (end_point_buffer >= ApplicationSettings.MinAutoPlanEndPointBuffer && end_point_buffer <= ApplicationSettings.MaxAutoPlanEndPointBuffer)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            // string suggestedbuffer = String.Format(CultureInfo.InvariantCulture, "{0} is not available.", end_point_buffer);
            string suggestedbuffer = "Value should be between " + ApplicationSettings.MinAutoPlanEndPointBuffer + " to " + ApplicationSettings.MaxAutoPlanEndPointBuffer;
            return Json(suggestedbuffer, JsonRequestBehavior.AllowGet);
        }
        // GET: Plan
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowPlanTool()
        {
            NetworkPlanning planobj = new NetworkPlanning();
            List<DisplayPlan> t = new List<DisplayPlan>();
            BindCableTypeDropDown(planobj);
            var userdetails = (User)Session["userDetail"];
            planobj.lstLayers = new BLLayer().GetLayerDetailsForAutoPlanning();

            planobj.end_point_buffer = ApplicationSettings.DefaultAutoPlanEndPointBuffer;
            planobj.MinAutoPlanEndPointBuffer = ApplicationSettings.MinAutoPlanEndPointBuffer;
            planobj.MaxAutoPlanEndPointBuffer = ApplicationSettings.MaxAutoPlanEndPointBuffer;
            planobj.MaxAutoOffsetValue = ApplicationSettings.MaxAutoOffsetValue;
            var lstLayerId = planobj.lstLayers.Select(s => s.layer_id).ToList();

            var lstSpecification = new DAVendorSpecification()
                .GetAllVendorSpecifications()
                  .Where(s => lstLayerId.Contains(s.layer_id)).ToList();

            planobj.lstSpecification = lstSpecification.Select(s => new KeyValueDropDown { key = s.id+ "," + s.code + ","+ s.vendor_id + ","+ s.layer_id, value = s.specification }).ToList();
            var VendorLst = new DAVendor().GetAllVendorsData();
            var vendorIds = lstSpecification.Select(x => x.vendor_id.ToString()).ToList();
            planobj.lstVendor = VendorLst.Where(s => vendorIds.Contains(s.id.ToString())).Select(x => new KeyValueDropDown
                {
                    key = x.id.ToString(),
                    value = x.name.ToString()
                }).ToList();
            return PartialView("_PlanTool", planobj);
        }

        public ActionResult ShowBulkPlanTool()
        {
            return PartialView("_BulkAutoPlan");
        }
        private void BindCableTypeDropDown(NetworkPlanning planobj)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Cable.ToString());
            planobj.listcableType = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Type.ToString() && x.dropdown_value != "Wall Clamped").ToList();
        }

        public JsonResult getNearestManholes(string buildingIDs)
        {
            JsonResponse<List<NearestMahhole>> objResp = new JsonResponse<List<NearestMahhole>>();
            try
            {
                objResp.result = new BLPlan().getNearestManholes(buildingIDs);
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Error while processing!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult processPlan(int building_id, int manhole_id, string geom)
        {
            JsonResponse<List<DbMessage>> objResp = new JsonResponse<List<DbMessage>>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    var usrId = usrDetail.user_id;
                    objResp.result = new BLPlan().processPlan(building_id, manhole_id, geom, usrId);
                    objResp.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Invalid User!";
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Error while processing!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Point2PointPlan(string geom)
        {
            JsonResponse<List<DbMessage>> objResp = new JsonResponse<List<DbMessage>>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    var usrId = usrDetail.user_id;
                    objResp.result = new BLPlan().Point2PointPlan(geom, usrId);
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

        public PartialViewResult GetBomData(NetworkPlanning model)
        {
            List<PlanBom> planobj = new List<PlanBom>();
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            if (user_id != 0)
            {
                planobj = new BLPlan().PlanBom(model, user_id);
                var planbom = planobj?.FirstOrDefault(); // get one record if exists

                if (planbom != null)
                {
                    Session["SiteId"] = planbom.site_id; 
                }
             }
            return PartialView("_BomBoqList", planobj);
        }

        public JsonResult GetTempLine(NetworkPlanning model)
        {
            JsonResponse<List<PlanBom>> objResp = new JsonResponse<List<PlanBom>>();
            if (model.cable_type == "Overhead")
            {
                int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);

                objResp.result = new BLPlan().GetPointOfCable(model, user_id);

                objResp.status = ResponseStatus.OK.ToString();
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTempCableLengthGemo(NetworkPlanning model)
        {
            JsonResponse<List<PlanBom>> objResp = new JsonResponse<List<PlanBom>>();
            if (model.cable_type == "Overhead")
            {
                int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);

                objResp.result = new BLPlan().GetTempCableLengthGemo(model, user_id);

                objResp.status = ResponseStatus.OK.ToString();
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getOffSetPolyLineCurve(string cablegemo, double offset)
        {
            var geom = new BLPlan().getCableGeom(cablegemo, offset);
            return Json(geom, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveProcess(NetworkPlanning objPlan)
        {
           
            if (ModelState.IsValid)
            {
                int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                if (user_id != 0)
                {
                    if (objPlan.is_loop_required == true && objPlan.is_loop_update == false)
                    {
                        new BLtemp_auto_network_plan().UpdateLoopLengthByPlanId(objPlan.temp_plan_id,objPlan.loop_length);
                    }
                    objPlan.created_by = user_id;
                    //objPlan = new BLPlan().SaveNetworkPlanning(objPlan);

                    var objResp = new BLPlan().savePoint2Point(objPlan);
                    objPlan.objPM.message = objResp.message;
                    objPlan.objPM.status = objResp.status.ToString();
                    objPlan.planid = objResp.plan_id;
                }
            }
            BindCableTypeDropDown(objPlan);
            return Json(objPlan, JsonRequestBehavior.AllowGet);
        }
       
        //DeletePlanByPlanId

        public ActionResult DeletePlanByPlanId(int plan_id)
        {
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var objResp = new BLPlan().DeletePlanByPlanId(plan_id, user_id);
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

        //public JsonResult GetEndPointEntity(double lat, double lng, string entity_type, double buffer)
        //{
        //    JsonResponse<List<DbMessage>> Data = new JsonResponse<List<DbMessage>>();
        //    var objResp = new BLPlan().GetNearByEndPoint(lat, lng, entity_type, buffer);
        //    if (objResp.Count > 0)
        //    {
        //        var NearPoints = JsonConvert.SerializeObject(objResp);
        //        Data.status = "true";
        //        Data.message = NearPoints;
        //        return Json(new { strReturn = objResp[0].message, msg = "OK" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else {
        //        Data.status = "false";
        //        Data.message = "No Records Exist !";
        //    }

        //    return Json(new { Data = Data, JsonRequestBehavior.AllowGet });
        //}
        public JsonResult GetEndPointEntity(double lat, double lng, string entity_type, double buffer)
        {
            var objResp = new BLPlan().GetNearByEndPoint(lat, lng, entity_type, buffer);
            if (objResp.Count > 0)
            {
                var data = JsonConvert.SerializeObject(objResp);

                return Json(new { Data = data, msg = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = "No Records Exist !", msg = "false" }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { Data = data, JsonRequestBehavior.AllowGet });
        }


        public JsonResult GetPlanElementPath(int plan_id)
        {
            string objresp = string.Empty;
            objresp = new BLPlan().GetPlanElement(plan_id);
            return new JsonResult { Data = objresp, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public PartialViewResult GetPlanData(ModelNetworkPlanningDetails objfiledetail)
        {
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            objfiledetail.objPlanDataFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objfiledetail.objPlanDataFilter.currentPage = 1;
            objfiledetail.objPlanDataFilter.sort = "";
            objfiledetail.objPlanDataFilter.orderBy = "";
            var fileList = new BLPlan().GetPlanDetails(objfiledetail.objPlanDataFilter, user_id);
            //objfiledetail.objPlanDataFilter.totalRecord = fileList.Count > 0 ? fileList[0].totalRecords : 0;
            string Filename = string.Empty;
            foreach (var item in fileList)
            {
                Filename = item.plan_name;
                //item.login_user = objUser.user_id;
                //item.filepath = filepath + item.type.ToUpper();
                //item.created_by_text = objUser.user_name;
                item.created_on = MiscHelper.FormatDateTime(item.created_on.ToString());
                //item.file_size = BytesToString(Convert.ToInt32(item.file_size));
            }
            objfiledetail.lstPlanDetails = fileList;
            //List <NetworkPlanning> planList= new BLPlan().GetNetworkPlanning(user_id);
            return PartialView("_ViewPlanData", objfiledetail);
        }
       
        public PartialViewResult GetLoopManage(int tempPlanid, double looplength, bool is_loop_updated)
        {
            List<temp_auto_network_plan> list = new BLtemp_auto_network_plan().GetTempNetwork(tempPlanid);
            string SiteId = Session["SiteId"].ToString();
            list.ForEach(x => x.site_id = SiteId);

            //if (!is_loop_updated) { 
            //list.ForEach(x => x.loop_length = looplength);
            //}
            return PartialView("_LoopManage", list);
        }


        public PartialViewResult GetFilterPlanFile(NetworkPlanningDataFilter objFilter)
        {
            ModelNetworkPlanningDetails objfiledetails = new ModelNetworkPlanningDetails();
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            objfiledetails.objPlanDataFilter = objFilter;
            objfiledetails.objPlanDataFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            var fileList = new BLPlan().GetPlanDetails(objfiledetails.objPlanDataFilter, user_id);
            foreach (var item in fileList)
            {
                item.created_by = user_id;
                // item.created_by_text = objUser.user_name;
                item.created_on = MiscHelper.FormatDateTime(item.created_on.ToString());
            }
            objfiledetails.lstPlanDetails = fileList;
            return PartialView("_PlanDataList", objfiledetails.lstPlanDetails);
        }       

        public JsonResult GetPlanDetails(int plan_id)
        {
            JsonResponse<NetworkPlanning> objResp = new JsonResponse<NetworkPlanning>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    var usrId = usrDetail.user_id;
                    objResp.result = new BLPlan().GetNetworkPlanningById(plan_id);
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
        
        public JsonResult GetNetworkForMap(int plan_id)
        {
            JsonResponse<NetworkPlanning> objResp = new JsonResponse<NetworkPlanning>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    var usrId = usrDetail.user_id;
                    objResp.result = new BLPlan().GetNetworkForMap(plan_id);
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

        public ActionResult SaveLoop(List<temp_auto_network_plan> model)
        {
            new BLtemp_auto_network_plan().UpdateTempLoop(model);
            return PartialView("_LoopManage", model);
        }
        public JsonResult getLoopLength(int temp_plan_id)
        {
            NetworkPlanning model = new NetworkPlanning();
            model.temp_plan_id = temp_plan_id;
            model.is_loop_update = true;
            JsonResponse<PlanBom> objResp = new JsonResponse<PlanBom>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                    if (user_id != 0)
                    {
                        objResp.result = new BLPlan().PlanBom(model, user_id).FirstOrDefault();
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
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTempNetworkForMap(int plan_id)
        {
            JsonResponse<List<NetworkPlanning>> objResp = new JsonResponse<List<NetworkPlanning>>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    var usrId = usrDetail.user_id;
                    objResp.result = new BLPlan().GetTempNetworkForMap(plan_id);
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
        public JsonResult GetNetworkPlanningLineLength(string geom)
        {
            JsonResponse<dynamic> objResp = new JsonResponse<dynamic>();
            objResp.result = new BLPlan().GetLineLength(geom);
            objResp.status = ResponseStatus.OK.ToString();
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVendorListById(int vendorId)
        {
            var vendorDetails = new DAVendor().GetVendorDetailsByID(vendorId);

            var result = new KeyValueDropDown
            {
                key = vendorDetails.id.ToString(),
                value = vendorDetails.name
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}