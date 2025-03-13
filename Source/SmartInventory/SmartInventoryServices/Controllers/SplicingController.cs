using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BusinessLogics;
using Models;
using Newtonsoft.Json;
using Utility;

namespace SmartInventoryServices.Controllers
{
    [Authorize]
    public class SplicingController : Controller
    {
        List<UserModule> lstUserModule= new BLMisc().GetUserModuleMasterList();
        [HttpPost]
        //public ActionResult Index(double latitude = 26.703370427182449, double longitude= 80.817710048281683, double bufferRadius =2)
        public ActionResult Index(double latitude, double longitude, double bufferRadius )
        {
            //User objUser = (User)(Session["userDetail"]);
            //objUser.role_id = 2;
            var splicingEntity = new BLOSPSplicing().getEntityForSplicing(latitude, longitude, bufferRadius, 2);
            return PartialView("_Splicing", splicingEntity);
        }


        public ActionResult CableToCable(connectionInput objSplicingIn)
        {
            //var lstUserModule = (List<string>)Session["ApplicableModuleList"];
            //var a = JsonConvert.SerializeObject(objSplicingIn);
            SplicingViewModel splicingEntity = new SplicingViewModel();
            var userId = 5;
            var lstUserModule = new BLLayer().GetUserModuleAbbrList(userId, UserType.Mobile.ToString());
            splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfo(objSplicingIn, JsonConvert.SerializeObject(objSplicingIn.listConnector));
            var connector = objSplicingIn.listConnector.FirstOrDefault();
            if (splicingEntity.splicingConnections.Count > 0)
            {
                splicingEntity.sourceCable = splicingEntity.splicingConnections.Where(m => m.system_id == objSplicingIn.source_system_id && m.entity_type == EntityType.Cable.ToString() && m.is_cable_a_end == objSplicingIn.is_source_start_point).FirstOrDefault();
                splicingEntity.destinationCable = splicingEntity.splicingConnections.Where(m => m.system_id == objSplicingIn.destination_system_id && m.entity_type == EntityType.Cable.ToString() && m.is_cable_a_end == objSplicingIn.is_destination_start_point).FirstOrDefault();
                splicingEntity.connector = splicingEntity.splicingConnections.Where(m => m.system_id == Convert.ToInt32(connector.system_id) && m.entity_type == connector.entity_type).FirstOrDefault();
            }
            var availablePorts = new BLOSPSplicing().getAvailablePorts(Convert.ToInt32(connector.system_id), connector.entity_type);
            splicingEntity.total_ports = availablePorts.total_ports;
            splicingEntity.available_ports = availablePorts.available_ports;
            splicingEntity.userId = userId;
            splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
            splicingEntity.isEditAllowed = true;
            splicingEntity.lstUserModule = lstUserModule;
            splicingEntity.lstSpliceTray = BLSpliceTray.Instance.GetSpliceTrayInfo(connector.system_id, connector.entity_type);
            splicingEntity.connector_entity_type = objSplicingIn.connector_entity_type;
            splicingEntity.is_middleware_entity = objSplicingIn.is_middleware_entity;
            splicingEntity.is_virtual = connector.is_virtual;
            splicingEntity.is_virtual_entity = connector.is_virtual_entity;
            ViewBag.source_system_id = objSplicingIn.source_system_id;
            ViewBag.destination_system_id = objSplicingIn.destination_system_id;
            return PartialView("_CableToCable", splicingEntity);
        }

        //public ActionResult GetSplicingDetails(string JsonData,string JsonUser_id)
        //{
        //    string JonString = MiscHelper.DecodeTo64(JsonData).ToString();
        //    var user_id = Convert.ToInt32(MiscHelper.DecodeTo64(JsonUser_id));
        //    string jsonFormattedString = JonString.Replace("\\\"", "\"");
        //    string ADFSEndPoint = string.Empty;
        //    // var tmpObj = JObject.Parse(jsonFormattedString);
        //    connectionInput objSplicingIn = new JavaScriptSerializer().Deserialize<connectionInput> (jsonFormattedString);
            
        //    var lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Mobile.ToString()); 
        //    SplicingViewModel splicingEntity = new SplicingViewModel();
        //    splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfo(objSplicingIn, JsonConvert.SerializeObject(objSplicingIn.listConnector));
        //    var connector = objSplicingIn.listConnector.FirstOrDefault();
        //    if (splicingEntity.splicingConnections.Count > 0)
        //    {
        //        splicingEntity.sourceCable = splicingEntity.splicingConnections.Where(m => m.system_id == objSplicingIn.source_system_id && m.entity_type == EntityType.Cable.ToString() && m.is_cable_a_end == objSplicingIn.is_source_start_point).FirstOrDefault();
        //        splicingEntity.destinationCable = splicingEntity.splicingConnections.Where(m => m.system_id == objSplicingIn.destination_system_id && m.entity_type == EntityType.Cable.ToString() && m.is_cable_a_end == objSplicingIn.is_destination_start_point).FirstOrDefault();
        //        splicingEntity.connector = splicingEntity.splicingConnections.Where(m => m.system_id == Convert.ToInt32(connector.system_id) && m.entity_type == connector.entity_type).FirstOrDefault();
        //    }
        //    var availablePorts = new BLOSPSplicing().getAvailablePorts(Convert.ToInt32(connector.system_id), connector.entity_type);
        //    splicingEntity.total_ports = availablePorts.total_ports;
        //    splicingEntity.available_ports = availablePorts.available_ports;
        //    splicingEntity.userId = user_id;
        //    splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
        //    splicingEntity.isEditAllowed = true;// lstUserModule.Contains("EDS");
        //    splicingEntity.lstUserModule = lstUserModule;
        //    splicingEntity.lstSpliceTray = BLSpliceTray.Instance.GetSpliceTrayInfo(connector.system_id, connector.entity_type);
        //    splicingEntity.connector_entity_type = objSplicingIn.connector_entity_type;
        //    splicingEntity.is_middleware_entity = objSplicingIn.is_middleware_entity;
        //    splicingEntity.is_virtual = connector.is_virtual;
        //    splicingEntity.is_virtual_entity = connector.is_virtual_entity;
        //    ViewBag.source_system_id = objSplicingIn.source_system_id;
        //    ViewBag.destination_system_id = objSplicingIn.destination_system_id;

        //    return PartialView("_CableToCable", splicingEntity);
        //}

        public JsonResult ValidtaeConnections(List<ConnectionInfoMaster> connections)
        {
            var resp = new BLOSPSplicing().ValidtaeConnections(JsonConvert.SerializeObject(connections));
            resp.message = BLConvertMLanguage.MultilingualMessageConvert(resp.message);
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveConnectionInfo(List<ConnectionInfoMaster> objConnectionInfo)
        {
            var userdetatils = (User)Session["userDetail"];
            objConnectionInfo.ForEach(p => p.created_by = 5/*Convert.ToInt32(Session["user_id"])*/);
            objConnectionInfo.ForEach(p => p.created_on = DateTimeHelper.Now);
            var objConnection = new BLOSPSplicing().SaveConnectionInfo(JsonConvert.SerializeObject(objConnectionInfo));
            var objConection = objConnectionInfo.FirstOrDefault();
            var module = lstUserModule.Where(x => x.module_abbr.ToUpper() == "NTF").ToString();
            if (module == "NTF")
            {
                if (objConection != null)
                {
                    new Thread(() =>
                    {
                        new BLOSPSplicing().SaveUtilizationNotification(objConection);
                        SmartInventoryHub smartInventoryhub = SmartInventoryHub.Instance;
                        //var UnreadNotificationCount = new BLMisc().GetUnreadNotificationCount(userdetatils.user_id, userdetatils.role_id);
                        var UnreadNotificationCount = new BLMisc().GetUnreadNotificationCount(5, 2);
                        NotificationOutPut objNotification = new NotificationOutPut();
                        objNotification.info = Convert.ToString(UnreadNotificationCount);
                        objNotification.sendToAllUser = false;
                        objNotification.notificationType = notificationType.Utilization.ToString();
                        smartInventoryhub.BroadCastInfo(objNotification);
                    }).Start();
                }
            }
            return Json(objConnection, JsonRequestBehavior.AllowGet);
        }
        public JsonResult deleteConnection(List<deleteConeectionInput> objConnectionInfo)
        {

            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new BLOSPSplicing().deleteConnection(JsonConvert.SerializeObject(objConnectionInfo), 0);
            var module = lstUserModule.Where(x => x.module_abbr.ToUpper() == "NTF").ToString();
            if (module == "NTF")
            {
                new Thread(() =>
                {
                    new BLOSPSplicing().utilizationReset(JsonConvert.SerializeObject(objConnectionInfo));
                }).Start();
            }
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);//response.message;
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCaptureImage(string imgdata)
        {
            string path = string.Empty;
            try
            {
                byte[] uploadedImage = Convert.FromBase64String(imgdata);
                //saving the image on the server
                var reqUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Uploads/temp_LOS/";
                string fileName = "LOS_" + Guid.NewGuid() + ".png";
                path = Server.MapPath(@"~/Uploads/temp_LOS/" + fileName);
                System.IO.File.WriteAllBytes(path, uploadedImage);
            }
            catch (Exception ex)
            {
                throw;
            }

            if (string.IsNullOrEmpty(path))
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = true, file = path }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getGeometryDetail(GeomDetailIn objGeomDetailIn)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            var objGeometryDetail = new BLSearch().GetGeometryDetails(objGeomDetailIn);


            if (objGeometryDetail.geometry_extent != null)
            {
                var extent = objGeometryDetail.geometry_extent.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
                string[] bounds = extent.Split(',');
                string[] southWest = bounds[0].Split(' ');
                string[] northEast = bounds[1].Split(' ');
                objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
                objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
                objResp.result = objGeometryDetail;
                objResp.status = ResponseStatus.OK.ToString();
            }
            else
            {
                objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAvailabePorts(int systemId, string entityType)
        {
            var availablePorts = new BLOSPSplicing().getAvailablePorts(systemId, entityType);
            return Json(availablePorts, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTrayUsedPort(int systemId)
        {
            int usedPorts = new BLSpliceTray().GetTrayUsedPort(systemId);
            return Json(usedPorts, JsonRequestBehavior.AllowGet);
        }
    }
}