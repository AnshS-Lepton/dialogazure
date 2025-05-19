using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics;
using Models;
using SmartInventory.Filters;
using Utility;
using System.Net;
using Models.ISP;
using BusinessLogics.Admin;
using BusinessLogics.ISP;
using System.IO;
using System.Data;
using NPOI.SS.UserModel;
using System.Xml.Linq;
using System.Configuration;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System.Text.RegularExpressions;
using System.Dynamic;
using Newtonsoft.Json;
using ZXing;
using ZXing.QrCode;
using System.Drawing;
using Lepton.Utility;
using Lepton.Entities;
using System.Web.Script.Serialization;
using System.Collections;
using System.Runtime.Remoting;



namespace SmartInventory.Controllers
{
    [Authorize]
    [WebRequestLogFilters]
    [SessionExpire]
    [HandleException]
    public class LibraryController : Controller
    {
        #region Varible
        public static string PhysicallyUtil_ByUsed;
        public static layerDetail layerDetail { get; set; }
        private readonly Common objCommon;
        private readonly BLMisc objBLMisc;
        private readonly BLMicroduct objBLMicroduct;
        private readonly BLCommon objBLCommon;
        #endregion
        public LibraryController()
        {
            objCommon = new Common();
            objBLMisc = new BLMisc();
            objBLMicroduct = new BLMicroduct();
            objBLCommon = new BLCommon();
        }
        #region Area
        public PartialViewResult AddArea(string networkIdType, int systemId = 0, string geom = "")
        {
            //Area objArea = GetAreaDetail(networkIdType, systemId, geom);
            //return PartialView("_AddArea", objArea);
            Area obj = new Area();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<Area>(url, obj, EntityType.Area.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddArea", response.results);

        }
        public Area GetAreaDetail(string networkIdType, int systemId, string geom = "")
        {
            Area objArea = new Area();
            objArea.geom = geom;
            objArea.networkIdType = networkIdType;
            var userdetails = (User)Session["userDetail"];
            var objDDL = new BLMisc().GetDropDownList(EntityType.Area.ToString());
            DropDownMaster drp = new DropDownMaster();
            //objDDL.Insert(0, new DropDownMaster { dropdown_key = "Select", dropdown_status = false, dropdown_type = DropDownType.Area_RFS.ToString(), dropdown_value = "0" });


            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objArea, GeometryType.Polygon.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objArea, new NetworkCodeIn() { eType = EntityType.Area.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objArea.geom }, networkIdType);
            }
            else
            {
                // Get entity detail by Id...
                objArea = new BLMisc().GetEntityDetailById<Area>(systemId, EntityType.Area);
            }

            objArea.lstAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.Area_RFS.ToString()).ToList();
            objArea.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objArea;
        }


        public ActionResult SaveArea(Area objArea, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //var objDDL = new BLMisc().GetDropDownList(EntityType.Area.ToString());
            //DropDownMaster drp = new DropDownMaster();
            //objArea.lstAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.Area_RFS.ToString()).ToList();
            //if (objArea.networkIdType == NetworkIdType.A.ToString() && objArea.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Area.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objArea.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objArea = GetAreaDetail(objArea.networkIdType, objArea.system_id, objArea.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objArea.area_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objArea.network_id = objNetworkCodeDetail.network_code;
            //    objArea.sequence_id = objNetworkCodeDetail.sequence_id;
            //}

            //if (TryValidateModel(objArea))
            //{
            //    var isNew = objArea.system_id > 0 ? false : true;
            //    var resultItem = new BLArea().SaveArea(objArea, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.Area.ToString() };

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        objArea.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objArea.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objArea.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    return PartialView("_AddArea", objArea);
            //}
            objArea.isDirectSave = isDirectSave;
            objArea.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objArea.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objArea.source_ref_type = "NETWORK_TICKET";
                objArea.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<Area>(url, objArea, EntityType.Area.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddArea", response.results);

        }
        #endregion

        #region RestrictedArea
        public PartialViewResult AddRestrictedArea(string networkIdType, int systemId = 0, string geom = "")
        {
            //Area objArea = GetAreaDetail(networkIdType, systemId, geom);
            //return PartialView("_AddArea", objArea);
            RestrictedArea obj = new RestrictedArea();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<RestrictedArea>(url, obj, EntityType.Restricted_Area.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddRestrictedArea", response.results);
        }

        public RestrictedArea GetRestrictedAreaDetail(string networkIdType, int systemId, string geom = "")
        {
            RestrictedArea objArea = new RestrictedArea();
            objArea.geom = geom;
            objArea.networkIdType = networkIdType;

            var objDDL = new BLMisc().GetDropDownList(EntityType.Restricted_Area.ToString());
            DropDownMaster drp = new DropDownMaster();
            //objDDL.Insert(0, new DropDownMaster { dropdown_key = "Select", dropdown_status = false, dropdown_type = DropDownType.Area_RFS.ToString(), dropdown_value = "0" });


            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objArea, GeometryType.Polygon.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objArea, new NetworkCodeIn() { eType = EntityType.Restricted_Area.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objArea.geom }, networkIdType);
            }
            else
            {
                // Get entity detail by Id...
                objArea = new BLMisc().GetEntityDetailById<RestrictedArea>(systemId, EntityType.Restricted_Area);
            }

            objArea.lstRestrictedAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.RestrictedArea_RFS.ToString()).ToList();
            return objArea;
        }
        public ActionResult SaveRestrictedArea(RestrictedArea objResArea, bool isDirectSave = false)
        {
            objResArea.isDirectSave = isDirectSave;
            objResArea.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<RestrictedArea>(url, objResArea, EntityType.Restricted_Area.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddRestrictedArea", response.results);
        }

        #endregion
        #region SubArea
        public PartialViewResult AddSubArea(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "")
        {
            // get parent geometry 
            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    geom = new BLMisc().getEntityGeom(pSystemId, pEntityType);
            //}
            //SubArea objSubArea = GetSubAreaDetail(networkIdType, systemId, geom);
            //return PartialView("_AddSubArea", objSubArea);
            SubArea obj = new SubArea();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SubArea>(url, obj, EntityType.SubArea.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddSubArea", response.results);
        }
        public SubArea GetSubAreaDetail(string networkIdType, int systemId, string geom = "")
        {
            SubArea objSubArea = new SubArea();
            var userdetails = (User)Session["userDetail"];
            objSubArea.geom = geom;
            objSubArea.networkIdType = networkIdType;
            var objDDL = new BLMisc().GetDropDownList(EntityType.SubArea.ToString());
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objSubArea, GeometryType.Polygon.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objSubArea, new NetworkCodeIn() { eType = EntityType.SubArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSubArea.geom }, networkIdType);
            }
            else
            {
                // Get entity detail by Id...
                objSubArea = new BLMisc().GetEntityDetailById<SubArea>(systemId, EntityType.SubArea);
            }

            objSubArea.lstSubAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.SubArea_RFS.ToString()).ToList();
            objSubArea.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objSubArea;
        }

        public ActionResult SaveSubArea(SubArea objSubArea, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //BuildingMaster objBuilding = new BuildingMaster();
            //var objDDL = new BLMisc().GetDropDownList(EntityType.Area.ToString());
            //objSubArea.lstSubAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.SubArea_RFS.ToString()).ToList();

            ////var geom = new BLMisc().getEntityGeom(objSubArea.parent_system_id, objSubArea.parent_entity_type);
            ////objSubArea.geom = geom;
            ////if (objSubArea.subarea_rfs== "A-RFS" && string.IsNullOrWhiteSpace(objSubArea.building_code))
            ////{
            ////  // var geom = new BLMisc().getEntityGeom(objSubArea.parent_system_id, objSubArea.parent_entity_type);
            ////    var buildingNetworkCode  = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Building.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = geom });
            ////    var dropdownlist = new BLMisc().GetDropDownList(EntityType.Building.ToString());
            ////    foreach (var item in dropdownlist.Where(x => x.dropdown_key == "Residential" && x.dropdown_type == "Category"))
            ////    {
            ////        objBuilding.category = item.dropdown_value;

            ////    }

            ////    foreach (var item in dropdownlist.Where(x=>x.dropdown_key=="Multiple" && x.dropdown_type== "Tenancy"))
            ////    {
            ////        objBuilding.tenancy = item.dropdown_value;

            ////    }

            ////    //BuildingMaster objBuilding = GetBuildingDetail(networkIdType, systemId, geom);
            ////    objSubArea.building_code = buildingNetworkCode.network_code;
            ////    objBuilding.network_id= buildingNetworkCode.network_code;
            ////    objBuilding.sequence_id = buildingNetworkCode.sequence_id;
            ////    objBuilding.region_id = objSubArea.region_id;
            ////    objBuilding.province_id = objSubArea.province_id;
            ////    objBuilding.geom = geom;
            ////    objBuilding.region_name = objSubArea.region_name;
            ////    objBuilding.province_name = objSubArea.province_name;
            ////    objBuilding.parent_entity_type = buildingNetworkCode.parent_entity_type;
            ////    objBuilding.parent_system_id = buildingNetworkCode.parent_system_id;
            ////    objBuilding.parent_network_id = buildingNetworkCode.parent_network_id;
            ////    objBuilding.building_name = "Default Building"; 
            ////    objBuilding.business_pass = 1;
            ////    objBuilding.home_pass = 1;
            ////    objBuilding.total_tower = 1;
            ////    objBuilding.rfs_status = "A-RFS";
            ////    objBuilding.rfs_date = DateTimeHelper.Now;
            ////    objBuilding.is_virtual = true;
            ////    objBuilding.building_status = BuildingStatus.Approved.ToString();
            ////    objBuilding.userid = Convert.ToInt32(Session["user_id"]);
            ////    var result = BLBuilding.Instance.SaveBuilding(objBuilding, NetworkStatus.P);
            ////    objSubArea.building_system_id = result.system_id;
            ////}
            //if (objSubArea.networkIdType == NetworkIdType.A.ToString() && objSubArea.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.SubArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSubArea.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objSubArea = GetSubAreaDetail(objSubArea.networkIdType, objSubArea.system_id, objSubArea.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objSubArea.subarea_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objSubArea.network_id = objNetworkCodeDetail.network_code;
            //    objSubArea.sequence_id = objNetworkCodeDetail.sequence_id;
            //}

            //if (TryValidateModel(objSubArea))
            //{
            //    var isNew = objSubArea.system_id > 0 ? false : true;
            //    var resultItem = new BLSubArea().SaveSubArea(objSubArea, Convert.ToInt32(Session["user_id"]));
            //    var result = VirtualBuilding(resultItem);
            //    if (string.IsNullOrEmpty(result.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.SubArea.ToString() };

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        objSubArea.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objSubArea.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objSubArea.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    return PartialView("_AddSubArea", objSubArea);
            //}
            objSubArea.isDirectSave = isDirectSave;
            objSubArea.user_id = Convert.ToInt32(Session["user_id"]);


            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objSubArea.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objSubArea.source_ref_type = "NETWORK_TICKET";
                objSubArea.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SubArea>(url, objSubArea, EntityType.SubArea.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddSubArea", response.results);
        }

        public SubArea VirtualBuilding(SubArea objSubArea)
        {
            BuildingMaster objBuilding = new BuildingMaster();
            EditGeomIn geomObj = new EditGeomIn();
            //GET SUBAREA GEOMETRY, REQUIRE TO UPDATE THE VIRTUAL BUILDING GEOM ASSOCIATED WITH SUBAREA.
            var geom = new BLMisc().getEntityGeom(objSubArea.system_id, EntityType.SubArea.ToString());
            if ((objSubArea.subarea_rfs == "A-RFS" || objSubArea.subarea_rfs == "A-RFS Type1" || objSubArea.subarea_rfs == "A-RFS Type2") && string.IsNullOrWhiteSpace(objSubArea.building_code))
            {
                var buildingNetworkCode = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Building.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = geom });
                var dropdownlist = new BLMisc().GetDropDownList(EntityType.Building.ToString());

                var objCategory = dropdownlist.Where(x => x.dropdown_key.ToUpper() == "RESIDENTIAL" && x.dropdown_type == DropDownType.Category.ToString()).FirstOrDefault();
                objBuilding.category = objCategory != null ? objCategory.dropdown_value : "";

                var objTenancy = dropdownlist.Where(x => x.dropdown_key.ToUpper() == "MULTIPLE" && x.dropdown_type == DropDownType.Tenancy.ToString()).FirstOrDefault();
                objBuilding.tenancy = objTenancy != null ? objTenancy.dropdown_value : "";

                objSubArea.building_code = buildingNetworkCode.network_code;
                objBuilding.network_id = buildingNetworkCode.network_code;
                objBuilding.sequence_id = buildingNetworkCode.sequence_id;
                objBuilding.region_id = objSubArea.region_id;
                objBuilding.province_id = objSubArea.province_id;
                objBuilding.geom = geom;
                objBuilding.region_name = objSubArea.region_name;
                objBuilding.province_name = objSubArea.province_name;
                objBuilding.parent_entity_type = buildingNetworkCode.parent_entity_type;
                objBuilding.parent_system_id = buildingNetworkCode.parent_system_id;
                objBuilding.parent_network_id = buildingNetworkCode.parent_network_id;
                objBuilding.building_name = "Default Building";
                objBuilding.business_pass = 9999;
                objBuilding.home_pass = 9999;
                objBuilding.total_tower = 9999;
                objBuilding.rfs_status = objSubArea.subarea_rfs;
                objBuilding.rfs_date = DateTimeHelper.Now;
                objBuilding.is_virtual = true;
                objBuilding.building_status = BuildingStatus.Approved.ToString();
                objBuilding.userid = Convert.ToInt32(Session["user_id"]);
                var result = BLBuilding.Instance.SaveBuilding(objBuilding, NetworkStatus.P);

                // UPDATE BUILDING CODE AND SYSTEM ID INTO SUBAREA
                objSubArea.building_system_id = result.system_id;
                objSubArea.geom = result.geom;
                var resultItem = new BLSubArea().UpdateSubAreaBuildingCode(objSubArea, Convert.ToInt32(Session["user_id"]));

                // DEFAULT GEOMETRY FOR BUILDING IS ALREADY INSERTING THROUGH TRIGGER.
                // BELOW SECTION WILL UPDATE BUILDING GEOM AS SUBAREA GEOMETRY.
                geomObj.systemId = result.system_id;
                geomObj.longLat = objSubArea.geom;
                geomObj.userId = Convert.ToInt32(Session["user_id"]);
                geomObj.entityType = EntityType.Building.ToString();
                geomObj.geomType = GeometryType.Polygon.ToString();
                var updateGeom = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                return objSubArea;
            }
            else if ((objSubArea.subarea_rfs == "A-RFS" || objSubArea.subarea_rfs == "A-RFS Type1" || objSubArea.subarea_rfs == "A-RFS Type2") && !string.IsNullOrWhiteSpace(objSubArea.building_code))
            {
                //UPDATE BUILDING RFS STATUS ONLY
                var result = BLBuilding.Instance.UpdateBuildingRFSType(objSubArea.subarea_rfs, objSubArea.building_system_id, Convert.ToInt32(Session["user_id"]));

            }
            return objSubArea;
        }
        public JsonResult getSubAreaDetails(int systemId)
        {
            var subAreaDetails = new BLMisc().GetEntityDetailById<SubArea>(systemId, (EntityType)Enum.Parse(typeof(EntityType), EntityType.SubArea.ToString()));
            return Json(new { buildingId = subAreaDetails.building_system_id, rfs_status = subAreaDetails.subarea_rfs }, JsonRequestBehavior.AllowGet);
        }


        //public JsonResult getDuctDetails(int systemId)
        //{

        //    var subAreaDetails = new BLMisc().GetEntityDetailById<DuctMaster>(systemId, (EntityType)Enum.Parse(typeof(EntityType), EntityType.Duct.ToString()));
        //    //if (subAreaDetails.duct_category == null && subAreaDetails.duct_category=="duct")
        //    //{
        //    //    subAreaDetails.duct_category = "duct";
        //    //}
        //    return Json(new { subAreaDetails.duct_category }, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region DSA
        public PartialViewResult AddDSA(string networkIdType, int systemId = 0, string geom = "")
        {
            //DSA objDsa = GetDSADetail(networkIdType, systemId, geom);

            //return PartialView("_AddDSA", objDsa);
            DSA obj = new DSA();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<DSA>(url, obj, EntityType.DSA.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddDSA", response.results);
        }

        public DSA GetDSADetail(string networkIdType, int systemId, string geom = "")
        {
            DSA objDSA = new DSA();
            objDSA.geom = geom;
            objDSA.networkIdType = networkIdType;

            var objDDL = new BLMisc().GetDropDownList(EntityType.DSA.ToString());
            DropDownMaster drp = new DropDownMaster();

            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objDSA, GeometryType.Polygon.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objDSA, new NetworkCodeIn() { eType = EntityType.DSA.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objDSA.geom }, networkIdType);
            }
            else
            {
                // Get entity detail by Id...
                objDSA = new BLMisc().GetEntityDetailById<DSA>(systemId, EntityType.DSA);
            }


            return objDSA;
        }



        public ActionResult SaveDSA(DSA objDsa, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //var objDDL = new BLMisc().GetDropDownList(EntityType.DSA.ToString());



            //if (objDsa.networkIdType == NetworkIdType.A.ToString() && objDsa.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.DSA.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objDsa.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objDsa = GetDSADetail(objDsa.networkIdType, objDsa.system_id, objDsa.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objDsa.dsa_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objDsa.network_id = objNetworkCodeDetail.network_code;
            //    objDsa.sequence_id = objNetworkCodeDetail.sequence_id;
            //}

            //if (TryValidateModel(objDsa))
            //{
            //    var isNew = objDsa.system_id > 0 ? false : true;
            //    var resultItem = new BLDsa().SaveDSA(objDsa, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.DSA.ToString() };

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        objDsa.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objDsa.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objDsa.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    return PartialView("_AddDSA", objDsa);
            //}
            objDsa.isDirectSave = isDirectSave;
            objDsa.user_id = Convert.ToInt32(Session["user_id"]);

            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objDsa.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objDsa.source_ref_type = "NETWORK_TICKET";
                objDsa.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<DSA>(url, objDsa, EntityType.DSA.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddDSA", response.results);

        }
        #endregion

        #region CSA
        public PartialViewResult AddCSA(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "")
        {

            // get parent geometry 
            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    geom = new BLMisc().getEntityGeom(pSystemId, pEntityType);
            //}
            //CSA objCsa = GetCSADetail(networkIdType, systemId, geom);
            //return PartialView("_AddCSA", objCsa);

            CSA obj = new CSA();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CSA>(url, obj, EntityType.CSA.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddCSA", response.results);
        }

        public CSA GetCSADetail(string networkIdType, int systemId, string geom = "")
        {
            CSA objCsa = new CSA();
            objCsa.geom = geom;
            objCsa.networkIdType = networkIdType;
            var userdetails = (User)Session["userDetail"];
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objCsa, GeometryType.Polygon.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objCsa, new NetworkCodeIn() { eType = EntityType.CSA.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objCsa.geom }, networkIdType);
            }
            else
            {
                // Get entity detail by Id...
                objCsa = new BLMisc().GetEntityDetailById<CSA>(systemId, EntityType.CSA);
            }
            objCsa.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objCsa;
        }

        public ActionResult SaveCSA(CSA objCsa, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //BuildingMaster objBuilding = new BuildingMaster();

            //if (objCsa.networkIdType == NetworkIdType.A.ToString() && objCsa.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.CSA.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objCsa.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objCsa = GetCSADetail(objCsa.networkIdType, objCsa.system_id, objCsa.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objCsa.csa_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objCsa.network_id = objNetworkCodeDetail.network_code;
            //    objCsa.sequence_id = objNetworkCodeDetail.sequence_id;
            //}

            //if (TryValidateModel(objCsa))
            //{
            //    var isNew = objCsa.system_id > 0 ? false : true;
            //    var resultItem = new BLCsa().SaveCSA(objCsa, Convert.ToInt32(Session["user_id"]));

            //    //var result = VirtualBuilding(resultItem);
            //    //if (string.IsNullOrEmpty(result.objPM.message))
            //    //{

            //    string[] LayerName = { EntityType.CSA.ToString() };
            //    if (isNew)
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.isNewEntity = isNew;
            //        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //    }
            //    objCsa.objPM = objPM;
            //    //}
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objCsa.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objCsa.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    return PartialView("_AddCSA", objCsa);
            //}
            objCsa.isDirectSave = isDirectSave;
            objCsa.user_id = Convert.ToInt32(Session["user_id"]);

            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objCsa.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objCsa.source_ref_type = "NETWORK_TICKET";
                objCsa.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CSA>(url, objCsa, EntityType.CSA.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddCSA", response.results);
        }

        #endregion

        #region SurveyArea
        //public PartialViewResult AddSurveyArea(string networkIdType, int systemId = 0, string geom = "", string childModel = "")
        //{
        //    SurveyArea objSurveyArea = GetSurveyAreaDetail(networkIdType, systemId, geom);
        //    if (childModel != "")
        //        objSurveyArea.childModel = childModel;
        //    return PartialView("_AddSurveyArea", objSurveyArea);
        //}

        public PartialViewResult AddSurveyArea(string networkIdType, int systemId = 0, string geom = "", string childModel = "")
        {
            SurveyArea obj = new SurveyArea();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SurveyArea>(url, obj, EntityType.SurveyArea.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddSurveyArea", response.results);
            //SurveyArea objSurveyArea = GetSurveyAreaDetail(networkIdType, systemId, geom);
            //if (childModel != "")
            //    objSurveyArea.childModel = childModel;
            //return PartialView("_AddSurveyArea", objSurveyArea);
        }
        public SurveyArea GetSurveyAreaDetail(string networkIdType, int systemId, string geom = "")
        {
            SurveyArea objSurveyArea = new SurveyArea();
            var userdetails = (User)Session["userDetail"];
            objSurveyArea.geom = geom;
            objSurveyArea.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objSurveyArea, GeometryType.Polygon.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objSurveyArea, new NetworkCodeIn() { eType = EntityType.SurveyArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSurveyArea.geom }, networkIdType);
            }
            else
            {
                // Get entity detail by Id...
                objSurveyArea = new BLMisc().GetEntityDetailById<SurveyArea>(systemId, EntityType.SurveyArea);
            }
            objSurveyArea.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objSurveyArea;
        }

        #endregion



        public ActionResult SaveSurveyArea(SurveyArea objSurveyArea, bool isDirectSave = false)
        {
            #region
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();

            //if (objSurveyArea.networkIdType == NetworkIdType.A.ToString() && objSurveyArea.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.SurveyArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSurveyArea.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISE SET REGION PROVINCE DETAILS..
            //        objSurveyArea = GetSurveyAreaDetail(objSurveyArea.networkIdType, objSurveyArea.system_id, objSurveyArea.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objSurveyArea.surveyarea_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objSurveyArea.network_id = objNetworkCodeDetail.network_code;
            //    objSurveyArea.sequence_id = objNetworkCodeDetail.sequence_id;
            //}

            //if (TryValidateModel(objSurveyArea))
            //{
            //    var isNew = objSurveyArea.system_id > 0 ? false : true;
            //    if (objSurveyArea.network_status == null)
            //    {
            //        objSurveyArea.network_status = "P";
            //    }
            //    var resultItem = new BLSurveyArea().SaveSurveyArea(objSurveyArea, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.SurveyArea.ToString() };

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        objSurveyArea.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objSurveyArea.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objSurveyArea.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    return PartialView("_AddSurveyArea", objSurveyArea);
            //}
            #endregion

            objSurveyArea.isDirectSave = isDirectSave;
            objSurveyArea.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objSurveyArea.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objSurveyArea.source_ref_type = "NETWORK_TICKET";
                objSurveyArea.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SurveyArea>(url, objSurveyArea, EntityType.SurveyArea.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddSurveyArea", response.results);
        }
        #region Building

        [HttpPost]
        public ActionResult UploadBuildingData(string PerformAction, List<String> Updatedfields)
        {
            string strReturn = "";
            string msg = "";
            //table for data
            DataTable dtExcelData = new DataTable();
            try
            {
                if (Request != null)
                {
                    var fileName = string.Empty;
                    var filepath = string.Empty;
                    var dataFileName = string.Empty;
                    int userId = Convert.ToInt32(Session["user_id"]);
                    HttpFileCollectionBase files = Request.Files;
                    DataTable dataTable = new DataTable();
                    if (files.Count > 1)
                    {
                        var timeStamp = DateTimeHelper.Now.ToString("yyyyMMddHHmmssfff");
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            fileName = Request.Files[i].FileName;
                            fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), timeStamp, Path.GetExtension(fileName));
                            filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\Buildings\\"), fileName);
                            file.SaveAs(filepath);
                            if (Path.GetExtension(fileName).ToUpper() == ".SHP")
                            {
                                dataFileName = filepath;
                            }
                        }
                    }
                    else
                    {
                        HttpPostedFileBase file = files[0];
                        fileName = AppendTimeStamp(Request.Files[0].FileName);
                        filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\Buildings\\"), fileName);
                        file.SaveAs(filepath);
                        if (Path.GetExtension(fileName).ToUpper() == ".XLS" || Path.GetExtension(fileName).ToUpper() == ".XLSX")
                        {
                            dataTable = NPOIExcelHelper.ExcelToTable(filepath, "");

                        }
                    }


                    if (!string.IsNullOrEmpty(dataFileName) && Path.GetExtension(dataFileName).ToUpper() == ".SHP")
                    {
                        dataTable = NPOIExcelHelper.ShapeToDataTable(dataFileName);
                        string strMappingFilePath = Server.MapPath("~\\Content\\Templates\\Bulk\\BuildingTemplate.xml");
                        Dictionary<string, string> dicColumnMapping = GetBulkUploadShapeColumnMapping(strMappingFilePath);
                        string[] arrColumns = dataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName.ToLower()).ToArray();
                        foreach (var pair in dicColumnMapping)
                        {
                            // if column not found in template and return error..
                            if (arrColumns.Contains(pair.Key.ToLower()))
                            {
                                dataTable.Columns[pair.Key.ToLower()].ColumnName = pair.Value.ToLower();
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(dataFileName) && Path.GetExtension(dataFileName).ToUpper() == ".SHP")
                    {
                        dataTable = NPOIExcelHelper.ShapeToDataTable(dataFileName);
                    }



                    // Remove blank rows...
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        dtExcelData = dataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
                    }

                    if (dtExcelData.Rows.Count > 0)
                    {
                        //get maximum building upload count allowed at a time...
                        if (dtExcelData.Rows.Count <= ApplicationSettings.BulkBuildingUploadMaxCount)
                        {
                            string ErrorMsg = "";
                            // get branch column mapping...
                            string strMappingFilePath = Server.MapPath("~\\Content\\Templates\\Bulk\\BuildingTemplate.xml");
                            Dictionary<string, string> dicColumnMapping = GetBulkUploadColumnMapping(strMappingFilePath);

                            // validate uploaded excel column with template mapping...
                            ErrorMsg = validateTemplateColumn(dicColumnMapping, dtExcelData);
                            if (ErrorMsg != "")
                                return Json(new { strReturn = ErrorMsg, msg = "error" }, JsonRequestBehavior.AllowGet);
                            if (ErrorMsg == "")
                            {
                                //ADD COLUMN TO DTEXCEL.. (UPLOADED_BY)
                                DataColumn dcUploadedBy = new DataColumn("UPLOADED_BY", typeof(int));
                                dcUploadedBy.DefaultValue = userId;
                                dtExcelData.Columns.Add(dcUploadedBy);

                                //ADD COLUMN TO DTEXCEL.. (IS_VALID)
                                DataColumn dcIsValid = new DataColumn("IS_VALID", typeof(int));
                                dtExcelData.Columns.Add(dcIsValid);

                                //ADD COLUMN TO DTEXCEL.. (ERROR_MSG)
                                DataColumn dcErrorMsg = new DataColumn("ERROR_MSG", typeof(string));
                                dcErrorMsg.MaxLength = 200;
                                dtExcelData.Columns.Add(dcErrorMsg);

                            }

                            //delete DATA FROM TEMP TABLE ON THE BASIS OF UPLOADED_BY ID
                            BLTempBuilding.Instance.DeleteTempBuildingData(userId);


                            List<TempBuildingMaster> lstTempBuildings = new List<TempBuildingMaster>();


                            foreach (DataRow dr in dtExcelData.Rows)
                            {


                                TempBuildingMaster objTempBuilding = new TempBuildingMaster();

                                string strErrorMsg = ValidateBuildingData(dr, ref objTempBuilding, dicColumnMapping, PerformAction, Updatedfields);

                                objTempBuilding.uploaded_by = userId;
                                objTempBuilding.address = dr[dicColumnMapping["address"]].ToString();
                                objTempBuilding.building_no = dr[dicColumnMapping["building_no"]].ToString();
                                objTempBuilding.address = dr[dicColumnMapping["address"]].ToString();
                                objTempBuilding.area = dr[dicColumnMapping["area"]].ToString();
                                objTempBuilding.media = dr[dicColumnMapping["media"]].ToString();
                                objTempBuilding.network_id = dr[dicColumnMapping["network_id"]].ToString();
                                objTempBuilding.building_name = dr[dicColumnMapping["building_name"]].ToString();
                                objTempBuilding.category = dr[dicColumnMapping["category"]].ToString();
                                objTempBuilding.pin_code = dr[dicColumnMapping["pin_code"]].ToString();
                                objTempBuilding.latitude = dr[dicColumnMapping["latitude"]].ToString();
                                objTempBuilding.longitude = dr[dicColumnMapping["longitude"]].ToString();
                                objTempBuilding.tenancy = dr[dicColumnMapping["tenancy"]].ToString();
                                objTempBuilding.province_name = dr[dicColumnMapping["Province_Name"]].ToString();
                                objTempBuilding.regionname = dr[dicColumnMapping["regionName"]].ToString();
                                objTempBuilding.business_pass = dr[dicColumnMapping["business_pass"]].ToString();
                                //objTempBuilding.building_type = dr[dicColumnMapping["building_type"]].ToString();
                                objTempBuilding.home_pass = dr[dicColumnMapping["home_pass"]].ToString();
                                objTempBuilding.total_tower = dr[dicColumnMapping["total_tower"]].ToString() == "" ? "0" : dr[dicColumnMapping["total_tower"]].ToString();
                                objTempBuilding.no_of_floor = dr[dicColumnMapping["no_of_floor"]].ToString() == "" ? "0" : dr[dicColumnMapping["no_of_floor"]].ToString();
                                objTempBuilding.rfs_status = dr[dicColumnMapping["rfs_status"]].ToString();
                                objTempBuilding.pod_name = dr[dicColumnMapping["pod_name"]].ToString();
                                objTempBuilding.pod_code = dr[dicColumnMapping["pod_code"]].ToString();
                                objTempBuilding.remarks = dr[dicColumnMapping["remarks"]].ToString();

                                lstTempBuildings.Add(objTempBuilding);

                            }
                            if (lstTempBuildings.Count > 0)
                            {
                                //SAVE DATA INTO TEMP BUILDING TABLE
                                BLTempBuilding.Instance.BulkUploadTempBuilding(lstTempBuildings);
                                //VALIDATE AND UPLAOD BUILDING INTO MAIN TABLE.
                                dynamic result = "";
                                if (PerformAction == "Insert")
                                {
                                    result = BLTempBuilding.Instance.UploadBuildingForInsert(userId);
                                }
                                else
                                {
                                    result = BLTempBuilding.Instance.UploadBuildingForUpdate(userId, Updatedfields);

                                };
                                if (!result.status)
                                {
                                    // exit function if failed..
                                    return Json(new { strReturn = string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_023, result.message), msg = "error" }, JsonRequestBehavior.AllowGet);//"Error in uploading Buildings! <br> Error:" + result.message,
                                }
                            }
                            var getTotalUploadBuildingfailureAndSuccess = BLTempBuilding.Instance.getTotalUploadBuildingfailureAndSuccess(userId);
                            var GetTotalCountOfSuccesAndFailure = "<table border='1' class='alertgrid'><thead><tr><td><b>Status</b></td><td><b>Count</b></td></tr></thead><tbody><tr><td>Success</td><td>" + getTotalUploadBuildingfailureAndSuccess.Item1 + "</td></tr><tr><td>failure</td><td>" + getTotalUploadBuildingfailureAndSuccess.Item2 + "</td></tr></tbody>";
                            strReturn = string.Format(Resources.Resources.SI_GBL_BUL_NET_FRM_001, GetTotalCountOfSuccesAndFailure);// "Building data processed successfully." + "</br>" + GetTotalCountOfSuccesAndFailure;
                        }
                        else
                        {
                            // exit function with max record error...
                            return Json(new { strReturn = string.Format(Resources.Resources.SI_GBL_BUL_NET_FRM_002, ApplicationSettings.BulkBuildingUploadMaxCount), msg = "error" }, JsonRequestBehavior.AllowGet);//"Maximum " + ApplicationSettings.BulkBuildingUploadMaxCount + " buildings can be uploaded at a time!"
                        }
                    }
                    else
                    {
                        // exit function with no record...
                        return Json(new { strReturn = Resources.Resources.SI_OSP_GBL_NET_FRM_327, msg = "error" }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (NPOI.POIFS.FileSystem.NotOLE2FileException ex)
            {
                msg = "error";
                strReturn = Resources.Resources.SI_OSP_GBL_NET_FRM_328;// "Selected file is either corrupted or invalid excel file!";
                ErrorLogHelper.WriteErrorLog("UploadBuildingData()", "Library", ex);

            }
            catch (Exception ex)
            {
                msg = "error";
                strReturn = string.Format(Resources.Resources.SI_GBL_BUL_NET_FRM_003, ex.Message);// "Failed to upload Buildings! <br> Error:" + ex.Message;
                ErrorLogHelper.WriteErrorLog("UploadBuildingData()", "Library", ex);
            }
            return Json(new { strReturn = strReturn, msg = msg == "" ? "success" : msg }, JsonRequestBehavior.AllowGet);
        }

        public void DownloadUploadBuildingLogs()
        {
            DataTable dtlogs = new DataTable();
            dtlogs.Columns.Add("Building_Code", typeof(string));
            dtlogs.Columns.Add("Status", typeof(string));
            dtlogs.TableName = "BuildingLogs";
            int userId = Convert.ToInt32(Session["user_id"]);
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                List<TempBuildingMaster> BulkUploadLogs = BLTempBuilding.Instance.GetUploadBuildingLogs(userId);

                if (BulkUploadLogs.Count() > 0)
                {
                    foreach (var t in BulkUploadLogs)
                    {
                        dtlogs.Rows.Add(t.network_id.ToString(), t.error_msg);
                    }

                    IWorkbook workbook = SmartInventory.Helper.NPOIExcelHelper.DataTableToExcel("xlsx", dtlogs);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", AppendTimeStamp("Buildinglogs.xlsx")));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }

        public string ValidateBuildingData(DataRow dr, ref TempBuildingMaster objBuilding, Dictionary<string, string> dicColumnMapping, string PerformAction, List<String> Updatedfields)
        {

            double latitude = 0.0, longitude = 0.0;
            DateTime RFS_DATe;
            objBuilding.is_valid = true;
            Regex nonNumericRegex = new Regex(@"\.");
            try
            {
                if (PerformAction == "Insert")
                {
                    //validate latitude
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["latitude"]].ToString()))
                    {
                        objBuilding.is_valid = false;
                        objBuilding.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_024;// "Latitude can not be blank!";
                    }
                    else
                    {
                        if (double.TryParse(dr[dicColumnMapping["latitude"]].ToString(), out latitude))
                        {

                            if (!nonNumericRegex.IsMatch(dr[dicColumnMapping["latitude"]].ToString()))
                            {
                                objBuilding.is_valid = false;
                                objBuilding.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_025;// "Invalid Latitude Value!";
                            }
                            if (latitude == 0 || latitude == 0.0)
                            {
                                objBuilding.is_valid = false;
                                objBuilding.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_025;// "Invalid Latitude Value!";
                            }
                        }
                        else
                        {
                            objBuilding.is_valid = false;
                            objBuilding.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_025; //"Invalid Latitude Value!";
                        }
                    }

                    //validate longitude

                    if (string.IsNullOrEmpty(dr[dicColumnMapping["longitude"]].ToString()))
                    {
                        objBuilding.is_valid = false;
                        objBuilding.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_026;// "longitude can not be blank!";
                    }
                    else
                    {
                        if (double.TryParse(dr[dicColumnMapping["longitude"]].ToString(), out longitude))
                        {
                            if (!nonNumericRegex.IsMatch(dr[dicColumnMapping["longitude"]].ToString()))
                            {
                                objBuilding.is_valid = false;
                                objBuilding.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_027;// "Invalid longitude Value!";
                            }
                            if (longitude == 0 || longitude == 0.0)
                            {
                                objBuilding.is_valid = false;
                                objBuilding.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_027;// "Invalid longitude Value!";
                            }
                        }
                        else
                        {
                            objBuilding.is_valid = false;
                            objBuilding.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_027;// "Invalid longitude Value!";
                        }
                    }
                }

                if (PerformAction == "Insert" || (PerformAction == "Update" && Updatedfields[0].Contains("RFS_DATE")))
                {
                    // validation RFS_date
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["rfs_date"]].ToString()))
                    {
                        //objBuilding.is_valid = "0";
                        //objBuilding.error_msg = "RFS Date can not be blank!";
                    }
                    else
                    {
                        if (!DateTime.TryParse(dr[dicColumnMapping["rfs_date"]].ToString(), out RFS_DATe))
                        {
                            objBuilding.is_valid = false;
                            objBuilding.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_028;// "Invalid RfS Date Value!";
                        }
                        else
                        {
                            objBuilding.rfs_date = RFS_DATe.ToString();
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }
        public string AppendTimeStamp(string fileName)
        {
            return string.Concat(
            Path.GetFileNameWithoutExtension(fileName),
            DateTimeHelper.Now.ToString("yyyyMMddHHmmssfff"),
            Path.GetExtension(fileName)
            );

        }

        public Dictionary<string, string> GetBulkUploadColumnMapping(string filepath)
        {
            Dictionary<string, string> dicMapping = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(filepath);
            return dicMapping = doc.Descendants("Mapping").OrderBy(s => (int)int.Parse(s.Attribute("id").Value))
                .Select(p => new
                {
                    DbColName = p.Element("DbColName").Value,
                    TemplateColName = p.Element("TemplateColName").Value
                })
                .ToDictionary(t => t.DbColName, t => t.TemplateColName);
        }

        public string validateTemplateColumn(Dictionary<string, string> dicColumnMapping, DataTable dt)
        {
            string[] arrColumns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.ToLower()).ToArray();
            foreach (var pair in dicColumnMapping)
            {
                // if column not found in template and return error..
                if (!arrColumns.Contains(pair.Value.ToLower()))
                    return string.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_331, pair.Value); //"Selected file does not contain '" + pair.Value + "' column!";
            }
            return "";
        }

        public PartialViewResult AddBuilding(string networkIdType, int systemId = 0, string geom = "", string childModel = "")
        {
            //int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            //BuildingMaster objBuilding = GetBuildingDetail(networkIdType, systemId, geom);
            //objBuilding.CJParent_system_id = systemId;
            //objBuilding.CJParent_entity_type = EntityType.Structure.ToString();
            //objBuilding.role_id = role_id;
            //objBuilding.lstUserModule = (List<string>)Session["ApplicableModuleList"];
            ////Bind dropdowns..
            //BindBuildingDropDown(objBuilding);
            //if (childModel != "")
            //    objBuilding.childModel = childModel;
            //return PartialView("_AddBuilding", objBuilding);
            BuildingMaster obj = new BuildingMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.childModel = childModel;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            obj.lstUserModule = (List<string>)Session["ApplicableModuleList"];
            //var result=UpdateVSATDetails(systemId);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<BuildingMaster>(url, obj, EntityType.Building.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddBuilding", response.results);
        }

        public BuildingMaster GetBuildingDetail(string networkIdType, int systemId, string geom = "")
        {


            BuildingMaster objBM = new BuildingMaster();
            objBM.geom = geom;
            objBM.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objBM, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objBM, new NetworkCodeIn() { eType = EntityType.Building.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBM.geom }, networkIdType);

                // fill latlong values
                string[] lnglat = geom.Split(new string[] { " " }, StringSplitOptions.None);
                objBM.latitude = Convert.ToDouble(lnglat[1].ToString());
                objBM.longitude = Convert.ToDouble(lnglat[0].ToString());
                //fill survey area detail   
                var objSurveyArea = BLBuilding.Instance.GetSurveyAreaExist(geom, GeometryType.Point.ToString());
                if (objSurveyArea != null)
                {
                    objBM.surveyarea_id = objSurveyArea.system_id;
                    objBM.surveyarea_name = objSurveyArea.surveyarea_name;
                }
                objBM.created_on = DateTimeHelper.Now;
            }
            else
            {
                // Get entity detail by Id...
                objBM = new BLMisc().GetEntityDetailById<BuildingMaster>(systemId, EntityType.Building, Convert.ToInt32(Session["user_id"]));
                objBM.createDate = Utility.MiscHelper.FormatDate(objBM.created_on.ToString());
                if (!string.IsNullOrEmpty(objBM.rfs_date.ToString()))
                    objBM.rfsSetDate = Utility.MiscHelper.FormatDate(objBM.rfs_date.ToString());

            }
            return objBM;
        }

        private void BindBuildingDropDown(BuildingMaster objBuilding)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Building.ToString());
            objBuilding.lstTenancy = objDDL.Where(x => x.dropdown_type == DropDownType.Tenancy.ToString()).ToList();
            objBuilding.lstCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Category.ToString()).ToList();
            objBuilding.lstRFSStatus = objDDL.Where(x => x.dropdown_type == DropDownType.RFS_Status.ToString()).ToList();
            objBuilding.lstMedia = objDDL.Where(x => x.dropdown_type == DropDownType.Media.ToString()).ToList();
            objBuilding.lstBuildingType = objDDL.Where(x => x.dropdown_type == DropDownType.Building_Type.ToString()).ToList();
            objBuilding.lstSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.SubCategory.ToString()).ToList();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken()]
        public ActionResult SaveBuilding(BuildingMaster objBuilding, string btnaction = "", bool isDirectSave = false)
        {

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objBuilding.system_id == 0)
            //{ btnaction = "Save"; }
            //else if (string.IsNullOrEmpty(btnaction)) { btnaction = "Update"; }
            //if (objBuilding.networkIdType == NetworkIdType.A.ToString() && objBuilding.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Building.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBuilding.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //       objBuilding = GetBuildingDetail(objBuilding.networkIdType, objBuilding.system_id, objBuilding.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS HERE
            //        //CURRENTLY NO VALUE IS INITIALIZED THERE AS BUILDING CAN NOT SAVED DIRECTLY
            //        // IF IN FUTURE WE NEED TO SAVE IT DIRECTLY THEN WE WOULD HAVE TO CREATE  A TEMPLATE FOR SAME
            //        // AND THEN WILL HAVE TO FILL ALL REQUIRED FILEDS VALUES FROM TEMPLATE
            //        btnaction = "Save";
            //    }
            //    //SET NETWORK CODE
            //    objBuilding.network_id = objNetworkCodeDetail.network_code;
            //    objBuilding.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (TryValidateModel(objBuilding))
            //{


            //    BuildingAction bldAction = (BuildingAction)Enum.Parse(typeof(BuildingAction), btnaction);
            //    objBuilding.userid = Convert.ToInt32(Session["user_id"]);
            //    objBuilding.bldAction = bldAction;
            //    if (!string.IsNullOrEmpty(objBuilding.rfsSetDate))
            //        objBuilding.rfs_date = Convert.ToDateTime(objBuilding.rfsSetDate);

            //    var result = BLBuilding.Instance.SaveBuilding(objBuilding, NetworkStatus.P);
            //    if (string.IsNullOrEmpty(result.pageMsg.message))
            //    {

            //        if (result.system_id != 0)
            //        {
            //            switch (bldAction)
            //            {
            //                case BuildingAction.Save:
            //                    objPM.isNewEntity = true;
            //                    objPM.message = Resources.Resources.SI_OSP_BUL_NET_FRM_002;
            //                    break;
            //                case BuildingAction.Update:
            //                    objPM.message = Resources.Resources.SI_OSP_BUL_NET_FRM_005;
            //                    break;
            //                case BuildingAction.Reject:
            //                    objPM.message = Resources.Resources.SI_OSP_BUL_NET_FRM_006;
            //                    break;
            //                case BuildingAction.Approve:
            //                    objPM.message = Resources.Resources.SI_OSP_BUL_NET_FRM_007;
            //                    break;

            //            }



            //            // For default structure
            //            if (bldAction == BuildingAction.Approve)
            //            //if (bldAction == BuildingAction.Approve && objBuilding.tenancy.ToLower() == "single")
            //            {
            //                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Structure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBuilding.geom, parent_eType = EntityType.Building.ToString(), parent_sysId = objBuilding.system_id });

            //                StructureMaster objStructure = new StructureMaster();

            //                objStructure.userid = Convert.ToInt32(Session["user_id"]);
            //                objStructure.system_id = 0;
            //                objStructure.parent_system_id = objNetworkCodeDetail.parent_system_id;
            //                objStructure.building_id = objNetworkCodeDetail.parent_system_id;
            //                objStructure.parent_entity_type = objNetworkCodeDetail.parent_entity_type;
            //                objStructure.parent_network_id = objNetworkCodeDetail.parent_network_id;
            //                objStructure.sequence_id = objNetworkCodeDetail.sequence_id;
            //                objStructure.network_id = objNetworkCodeDetail.network_code;
            //                //objStructure.no_of_floor = objBuilding.no_of_floor == 0 ? 1 : objBuilding.no_of_floor;
            //                objStructure.no_of_floor = 1;// No_of_floor will be 1 for default structure.
            //                objStructure.no_of_flat = objStructure.no_of_floor;//objBuilding.no_of_flat;
            //                objStructure.business_pass = objBuilding.business_pass == 0 ? 0 : 1;
            //                objStructure.home_pass = objBuilding.home_pass == 0 ? 0 : 1;
            //                objStructure.no_of_shaft = 2;
            //                objStructure.no_of_occupants = 1;
            //                objStructure.structure_name = "Tower-1";
            //                objStructure.geom = objBuilding.longitude.ToString() + " " + objBuilding.latitude.ToString();
            //                objStructure.longitude = objBuilding.longitude;
            //                objStructure.latitude = objBuilding.latitude;
            //                objStructure.region_id = objBuilding.region_id;
            //                objStructure.province_id = objBuilding.province_id;
            //                objStructure.isDefault = true;
            //                //if (objStructure.no_of_shaft > 0)
            //                //{
            //                //    StructureShaftInfo objShaft = new StructureShaftInfo();
            //                //    for (int i = 0; i < objStructure.no_of_shaft; i++)
            //                //    {
            //                //        objShaft.is_virtual = false;
            //                //        objShaft.shaft_name = "Shaft_" + (i + 1);
            //                //        objShaft.shaft_position = "left";
            //                //        objStructure.lstShaftInfo.Add(objShaft);
            //                //    }

            //                //}
            //                if (objStructure.no_of_floor > 0)
            //                {

            //                    for (int i = 0; i < objStructure.no_of_floor; i++)
            //                    {
            //                        StructureFloorInfo objFloor = new StructureFloorInfo();
            //                        objFloor.no_of_units = 1;
            //                        objFloor.floor_name = "floor_" + (i + 1);
            //                        objFloor.height = Convert.ToInt32(ApplicationSettings.DefaultFloorHeight);
            //                        objFloor.width = Convert.ToInt32(ApplicationSettings.DefaultFloorWidth);
            //                        objFloor.length = Convert.ToInt32(ApplicationSettings.DefaultFloorLength);
            //                        objStructure.lstFloorInfo.Add(objFloor);
            //                    }
            //                }

            //                var resultStruct = BLStructure.Instance.SaveStructure(objStructure, NetworkStatus.P.ToString());
            //            }
            //            //save AT Status                        
            //            if (objBuilding.ATAcceptance != null && objBuilding.system_id > 0)
            //            {
            //                SaveATAcceptance(objBuilding.ATAcceptance, objBuilding.system_id);
            //            }
            //            //var isnew = objStructure.system_id == 0 ? true : false;
            //            //var result = BLStructure.Instance.SaveStructure(objStructure, NetworkStatus.P.ToString());
            //            //if (result.system_id != 0)
            //            //{
            //            //    if (isnew)
            //            //    {
            //            //        objPM.status = ResponseStatus.OK.ToString();
            //            //        objPM.isNewEntity = isnew;
            //            //        objPM.message = "Structure  Saved successfully !";
            //            //    }
            //            //    else
            //            //    {
            //            //        objPM.status = ResponseStatus.OK.ToString();
            //            //        objPM.message = "Structure Updated successfully !";
            //            //    }
            //            //    objStructure = result;
            //            //}
            //            //else
            //            //{
            //            //    objPM.status = ResponseStatus.FAILED.ToString();
            //            //    objPM.message = "Error while saving structure detail!";
            //            //}

            //            objPM.status = ResponseStatus.OK.ToString();
            //            objBuilding = result;
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.FAILED.ToString();
            //            objPM.message = Resources.Resources.SI_OSP_BUL_NET_FRM_008;
            //        }
            //        objBuilding.pageMsg = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objBuilding.pageMsg = objPM;
            //}


            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objBuilding.pageMsg, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    // fill dropdowns
            //    BindBuildingDropDown(objBuilding);
            //    objBuilding.createDate = Utility.MiscHelper.FormatDate(objBuilding.created_on.ToString());
            //    if (!string.IsNullOrEmpty(objBuilding.rfs_date.ToString()))
            //        objBuilding.rfsSetDate = Utility.MiscHelper.FormatDate(objBuilding.rfs_date.ToString());
            //    objBuilding.lstUserModule = (List<string>)Session["ApplicableModuleList"];

            //    return PartialView("_AddBuilding", objBuilding);
            //}
            objBuilding.btnaction = btnaction;
            objBuilding.isDirectSave = isDirectSave;
            //objBuilding.lstUserModule = (List<string>)Session["ApplicableModuleList"];
            objBuilding.user_id = Convert.ToInt32(Session["user_id"]);
            //objBuilding.role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objBuilding.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objBuilding.source_ref_type = "NETWORK_TICKET";
                objBuilding.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<BuildingMaster>(url, objBuilding, EntityType.Building.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            response.results.lstUserModule = (List<string>)Session["ApplicableModuleList"];
            return PartialView("_AddBuilding", response.results);
        }

        public PartialViewResult GetBuildingRFSStatus(int buildingId)
        {
            IEnumerable<BuildingRfSStatus> objStrucLst = new List<BuildingRfSStatus>();
            if (buildingId != 0)
            {
                objStrucLst = BLRFSStatus.Instance.GetRFS_StatusByBld(buildingId);
            }
            return PartialView("_BuildingRFSStatus", objStrucLst);
        }


        public JsonResult isValidRFSStatus(string old_rfs_status, string new_rfs_status, string entityType)
        {
            //JsonResponse<string> objResp = new JsonResponse<string>();
            //try
            //{

            //    if (new BLBuildingRFS().isValidRFSStatus(old_rfs_status, new_rfs_status, entityType))
            //    {
            //        objResp.status = ResponseStatus.OK.ToString();
            //    }
            //    else
            //    {
            //        objResp.status = ResponseStatus.FAILED.ToString();
            //        objResp.message = string.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_306, old_rfs_status, new_rfs_status);
            //    }
            //}
            //catch
            //{
            //    objResp.status = ResponseStatus.ERROR.ToString();
            //    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_307;
            //}
            //return Json(objResp, JsonRequestBehavior.AllowGet);
            BuildingMaster obj = new BuildingMaster();
            obj.old_rfs_status = old_rfs_status;
            obj.new_rfs_status = new_rfs_status;
            obj.entityType = entityType;
            string url = "api/main/isValidRFSStatus";
            var response = WebAPIRequest.PostIntegrationAPIRequest<BuildingMaster>(url, obj, "", "");
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public Dictionary<string, string> GetBulkUploadShapeColumnMapping(string filepath)
        {
            Dictionary<string, string> dicMapping = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(filepath);
            return dicMapping = doc.Descendants("Mapping").OrderBy(s => (int)int.Parse(s.Attribute("id").Value))
                .Select(p => new
                {
                    ShapeColName = p.Element("ShapeColName").Value,
                    TemplateColName = p.Element("TemplateColName").Value
                })
                .ToDictionary(t => t.ShapeColName, t => t.TemplateColName);
        }
        #endregion

        #region Barcode

        public PartialViewResult GetBarCode(int system_id, string entity_type)
        {
            BarCode objBarCode = new BarCode();
            objBarCode = new BLMisc().GetEntityDetailForBarCode(system_id, entity_type);
            objBarCode.entity_type = entity_type;
            if (!string.IsNullOrEmpty(objBarCode.barcode))
            {
                var imgBytes = BarcodeHelper.GenerateBarcode(objBarCode.barcode.ToString(), true);
                if (imgBytes != null && imgBytes.Length > 0)
                {
                    objBarCode.barcode_img_bytes = string.Concat("data:image//jpeg;base64,", Convert.ToBase64String(imgBytes));
                }
            }

            return PartialView("_BarCode", objBarCode);
        }


        #endregion

        #region Structure

        public PartialViewResult AddStructure(string networkIdType, string geom = "", int systemId = 0, int building_id = 0)
        {
            //StructureMaster objStruc = GetStructureDetail(networkIdType, systemId, geom, building_id);

            //return PartialView("_AddStructure", objStruc);
            StructureMaster obj = new StructureMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.building_id = building_id;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<StructureMaster>(url, obj, EntityType.Structure.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddStructure", response.results);
        }

        public StructureMaster GetStructureDetail(string networkIdType, int systemId, string geom = "", int building_id = 0)
        {
            StructureMaster objStructure = new StructureMaster();
            var userdetails = (User)Session["userDetail"];
            objStructure.geom = geom;
            objStructure.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objStructure, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objStructure, new NetworkCodeIn() { eType = EntityType.Structure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objStructure.geom, parent_eType = EntityType.Building.ToString(), parent_sysId = building_id }, networkIdType);
                objStructure.created_on = DateTimeHelper.Now;
                objStructure.building_id = building_id;
                // fill latlong values
                string[] lnglat = geom.Split(new string[] { " " }, StringSplitOptions.None);
                objStructure.latitude = Convert.ToDouble(lnglat[1].ToString());
                objStructure.longitude = Convert.ToDouble(lnglat[0].ToString());
                List<StructureShaftInfo> shaftList = new List<StructureShaftInfo>();
                shaftList.Add(new StructureShaftInfo { is_virtual = true, shaft_name = "Shaft_1", shaft_position = "left" });
                shaftList.Add(new StructureShaftInfo { is_virtual = true, shaft_name = "Shaft_2", shaft_position = "right" });
                objStructure.lstShaftInfo = shaftList;
                objStructure.no_of_shaft = 2;
            }
            else
            {

                objStructure = new BLMisc().GetEntityDetailById<StructureMaster>(systemId, EntityType.Structure, Convert.ToInt32(Session["user_id"]));
                objStructure.lstShaftInfo = BLShaft.Instance.GetShaftByBld(systemId);
                objStructure.lstFloorInfo = BLFloor.Instance.GetFloorByBld(systemId);
                objStructure.geom = objStructure.longitude.ToString() + " " + objStructure.latitude.ToString();
            }
            objStructure.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objStructure;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken()]
        public ActionResult SaveStructure(StructureMaster objStructure, string actionTab, bool isDirectSave = false)
        {
            if (actionTab == "SiteInfoTab")
            {
                objStructure.SiteInformation.ActionTab = actionTab;
                return SaveSiteInfo(objStructure.SiteInformation);
            }
            else
            {
                //    bool shaftWithRiser = false;
                //    ModelState.Clear();
                //    DbMessage response = new DbMessage();
                //    PageMessage objPM = new PageMessage();
                //    var strGeom = objStructure.geom;
                //    if (objStructure.networkIdType == NetworkIdType.A.ToString() && objStructure.system_id == 0)
                //    {
                //        //GET AUTO NETWORK CODE...
                //        var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Structure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objStructure.geom, parent_eType = EntityType.Building.ToString(), parent_sysId = objStructure.building_id });
                //        if (isDirectSave == true)
                //        {
                //            //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                //            objStructure = GetStructureDetail(objStructure.networkIdType, objStructure.system_id, objStructure.geom, objStructure.building_id);
                //            // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS HERE
                //            //CURRENTLY NO VALUE IS INITIALIZED THERE AS STRUCTURE CAN NOT SAVED DIRECTLY
                //            // IF IN FUTURE WE NEED TO SAVE IT DIRECTLY THEN WE WOULD HAVE TO CREATE  A TEMPLATE FOR SAME
                //            // AND THEN WILL HAVE TO FILL ALL REQUIRED FILEDS VALUES FROM TEMPLATE
                //        }
                //        //SET NETWORK CODE
                //        objStructure.network_id = objNetworkCodeDetail.network_code;
                //        objStructure.sequence_id = objNetworkCodeDetail.sequence_id;
                //    }
                //    if (TryValidateModel(objStructure))
                //    {
                //        objStructure.userid = Convert.ToInt32(Session["user_id"]);
                //        var isnew = objStructure.system_id == 0 ? true : false;
                //        var shaftList = objStructure.lstShaftInfo.Select(m => new { system_id = m.system_id, shaft_position = m.shaft_position });
                //        //string cable = objStructure.parent_network_id+":"+
                //        //if (objStructure.system_id > 0)
                //        //{
                //        //    response = BLCable.Instance.isIspLineExists(JsonConvert.SerializeObject(shaftList), objStructure.system_id, objStructure.lstFloorInfo.Count(), objStructure.lstShaftInfo.Count());
                //        //}
                //        if (!response.status)
                //        {
                //            for (int i = 0; i < objStructure.lstShaftInfo.Count; i++)
                //            {
                //                if (objStructure.lstShaftInfo[i].with_riser)
                //                { shaftWithRiser = true; }

                //                if (objStructure.lstShaftInfo[i].length == 0)
                //                    objStructure.lstShaftInfo[i].length = Settings.ApplicationSettings.Shaft_Length_Mtr;
                //                if (objStructure.lstShaftInfo[i].width == 0)
                //                    objStructure.lstShaftInfo[i].width = Settings.ApplicationSettings.Shaft_Width_Mtr;
                //            }
                //            var formSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Structure.ToString()).ToList();
                //            var withRiser = formSettings.Count > 0 ? formSettings.Where(m => m.form_feature_name.ToLower() == formFeatureName.with_riser.ToString() && m.form_feature_type.ToLower() == formFeatureType.feature.ToString() && m.is_active == true).FirstOrDefault() : null;
                //            if (withRiser != null && shaftWithRiser)
                //            {
                //                var objItem = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.FDB);
                //                if (objItem.specification == "" || objItem.specification == null)
                //                {
                //                    objPM.status = ResponseStatus.FAILED.ToString();
                //                    objPM.message = Resources.Resources.SI_OSP_STR_NET_FRM_045;
                //                    objStructure.pageMsg = objPM;
                //                    return PartialView("_AddStructure", objStructure);
                //                }
                //            }
                //            for (int i = 0; i < objStructure.lstFloorInfo.Count; i++)
                //            {
                //                if (objStructure.lstFloorInfo[i].length == 0)
                //                    objStructure.lstFloorInfo[i].length = Settings.ApplicationSettings.Floor_Length_Mtr;
                //                if (objStructure.lstFloorInfo[i].width == 0)
                //                    objStructure.lstFloorInfo[i].width = Settings.ApplicationSettings.Floor_Width_Mtr;
                //                if (objStructure.lstFloorInfo[i].height == 0)
                //                    objStructure.lstFloorInfo[i].height = Settings.ApplicationSettings.Floor_Height_Mtr;
                //            }

                //            //add validationn for updating the structure depended on shaft and floor
                //            var shaftName = BLStructure.Instance.CheckEntityAssociation(objStructure);
                //            entityATStatusAtachmentsList objATA = objStructure.ATAcceptance;
                //            //SiteInfo objSite = objStructure.SiteInformation;
                //            objStructure = BLStructure.Instance.SaveStructure(objStructure, NetworkStatus.P.ToString());
                //            if (string.IsNullOrEmpty(objStructure.pageMsg.message))
                //            {
                //                if (objStructure.system_id != 0)
                //                {
                //                    if (isnew)
                //                    {
                //                        objPM.status = ResponseStatus.OK.ToString();
                //                        objPM.isNewEntity = isnew;
                //                        objPM.message = Resources.Resources.SI_OSP_STR_NET_FRM_042;
                //                    }
                //                    else
                //                    {
                //                        objPM.status = ResponseStatus.OK.ToString();
                //                        objPM.message = Resources.Resources.SI_OSP_STR_NET_FRM_043;
                //                    }
                //                    objStructure.geom = strGeom;
                //                }
                //                else
                //                {
                //                    objPM.status = ResponseStatus.FAILED.ToString();
                //                    objPM.message = Resources.Resources.SI_OSP_STR_NET_FRM_044;
                //                }
                //                if (shaftName != "")
                //                {

                //                    objPM.message += "<strong>" + Resources.Resources.SI_OSP_STR_NET_FRM_046 + " " + "[" + shaftName + "]" + " " + Resources.Resources.SI_OSP_STR_NET_FRM_047 + "</strong>";
                //                    //objPM.message += "<strong> FDB is associated with other entity,So you can not set the shaft[" + shaftName + "] without riser!</strong>";
                //                    //objPM.message += "<br/><table class='shaftLstInfo' cellpadding='0' cellspacing='0' style='font-size:smaller;text-align:center;'><thead><tr><th>Shaft Name</th></tr></thead><tbody>" + shaftName + "</tbody></table>";
                //                }
                //                objStructure.pageMsg = objPM;

                //                if (objATA != null && objStructure.system_id > 0)
                //                {
                //                    SaveATAcceptance(objATA, objStructure.system_id);
                //                }
                //                //if (objSite != null && objStructure.system_id > 0)
                //                //{
                //                //    SaveSiteInfo(objSite, objStructure.system_id,EntityType.Structure.ToString(),objStructure.network_id);
                //                //}
                //            }
                //        }
                //        else
                //        {
                //            objPM.status = ResponseStatus.FAILED.ToString();
                //            objPM.message = response.message;
                //            objStructure.pageMsg = objPM;
                //        }
                //    }
                //    else
                //    {
                //        objPM.status = ResponseStatus.FAILED.ToString();
                //        objPM.message = getFirstErrorFromModelState();
                //        objStructure.pageMsg = objPM;
                //    }
                //    if (isDirectSave == true)
                //    {
                //        //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                //        return Json(objStructure.pageMsg, JsonRequestBehavior.AllowGet);
                //    }
                //    else
                //    {
                //        if (objStructure.system_id != 0 && string.IsNullOrEmpty(objStructure.pageMsg.message))
                //        {
                //            objStructure.lstShaftInfo = BLShaft.Instance.GetShaftByBld(objStructure.system_id);
                //            objStructure.lstFloorInfo = BLFloor.Instance.GetFloorByBld(objStructure.system_id);
                //        }
                //        // RETURN PARTIAL VIEW WITH MODEL DATA
                //        return PartialView("_AddStructure", objStructure);
                //    }
                // }
                objStructure.isDirectSave = isDirectSave;
                objStructure.user_id = Convert.ToInt32(Session["user_id"]);
                objStructure.actionTab = actionTab;
                string url = "api/Library/EntityOperations";
                var response = WebAPIRequest.PostIntegrationAPIRequest<StructureMaster>(url, objStructure, EntityType.Structure.ToString(), EntityAction.Save.ToString());
                if (isDirectSave)
                {
                    return Json(response.results.pageMsg, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_AddStructure", response.results);
            }
        }

        public PartialViewResult GetStructureByBld(int buildingId)
        {
            List<StructureMaster> objStrucLst = new List<StructureMaster>();
            if (buildingId != 0)
            {
                objStrucLst = BLStructure.Instance.GetStructureByBld(buildingId);
            }
            return PartialView("_StructureList", objStrucLst);
        }

        public JsonResult IsBldStatApproved(int buildingId)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {


                var bldDetail = new BLMisc().GetEntityDetailById<BuildingMaster>(buildingId, EntityType.Building); ;
                //if (bldDetail.building_status == BuildingStatus.Approved.ToString() && bldDetail.tenancy != BuildingTenancy.Single.ToString())
                if (bldDetail.building_status == BuildingStatus.Approved.ToString())
                {
                    objResp.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();

                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_292;
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_293;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsBuildingApproved(int buildingId)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {

                var bldDetail = new BLMisc().GetEntityDetailById<BuildingMaster>(buildingId, EntityType.Building); ;
                if (bldDetail.building_status == BuildingStatus.Approved.ToString())
                {
                    objResp.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_294;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkIspLine(int systemId)
        {
            var isLineExist = BLCable.Instance.checkIspLine(systemId);
            return Json(isLineExist, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ADB
        public PartialViewResult AddADB(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    // get geom by parent id...
            //    geom = GetPointTypeParentGeom(pSystemId, pEntityType);
            //}
            //ADBMaster objADBMaster = GetADBDetail(pSystemId, pEntityType, networkIdType, systemId, geom);
            //BLItemTemplate.Instance.BindItemDropdowns(objADBMaster, EntityType.ADB.ToString());
            //BindADBDropDown(objADBMaster);
            //fillProjectSpecifications(objADBMaster);
            //new MiscHelper().BindPortDetails(objADBMaster, EntityType.ADB.ToString(), DropDownType.Adb_Port_Ratio.ToString());
            //objADBMaster.pNetworkId = pNetworkId;
            //var objDDL = new BLMisc().GetDropDownList(EntityType.ADB.ToString(), DropDownType.Entity_Type.ToString());
            //objADBMaster.lstEntityType = objDDL;
            //return PartialView("_AddADB", objADBMaster);

            ADBMaster obj = new ADBMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ADBMaster>(url, obj, EntityType.ADB.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddADB", response.results);
        }
        public ADBMaster GetADBDetail(int pSystemId, string pEntityType, string networkIdType, int systemId, string geom = "")
        {
            ADBMaster objADB = new ADBMaster();
            objADB.geom = geom;
            objADB.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objADB, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objADB, new NetworkCodeIn() { eType = EntityType.ADB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objADB.geom }, networkIdType);
                objADB.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objADB.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objADB.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<ADBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.ADB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objADB);
            }
            else
            {
                // Get entity detail by Id...
                objADB = new BLMisc().GetEntityDetailById<ADBMaster>(systemId, EntityType.ADB);
            }
            return objADB;
        }
        public ActionResult SaveADB(ADBMaster objADBMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int pSystemId = objADBMaster.pSystemId;
            //string pEntitytype = objADBMaster.pEntityType;
            //string pNetworkId = objADBMaster.pNetworkId;
            //if (objADBMaster.networkIdType == NetworkIdType.A.ToString() && objADBMaster.system_id == 0)
            //{   // get parent geometry 
            //    if (string.IsNullOrWhiteSpace(objADBMaster.geom) && objADBMaster.system_id == 0)
            //    {
            //        objADBMaster.geom = GetPointTypeParentGeom(objADBMaster.pSystemId, objADBMaster.pEntityType);
            //    }
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ADB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objADBMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objADBMaster = GetADBDetail(objADBMaster.pSystemId, objADBMaster.pEntityType, objADBMaster.networkIdType, objADBMaster.system_id, objADBMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objADBMaster.adb_name = objNetworkCodeDetail.network_code;
            //        objADBMaster.pSystemId = pSystemId;
            //        objADBMaster.pEntityType = pEntitytype;
            //        objADBMaster.pNetworkId = pNetworkId;
            //    }
            //    //SET NETWORK CODE
            //    objADBMaster.network_id = objNetworkCodeDetail.network_code;
            //    objADBMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            ////ADBTemplateMaster objItem = new ADBTemplateMaster();

            //if (TryValidateModel(objADBMaster))
            //{
            //    var isNew = objADBMaster.system_id > 0 ? false : true;
            //    if (objADBMaster.unitValue != null && objADBMaster.unitValue.Contains(":"))
            //    {
            //        objADBMaster.no_of_input_port = Convert.ToInt32(objADBMaster.unitValue.Split(':')[0]);
            //        objADBMaster.no_of_output_port = Convert.ToInt32(objADBMaster.unitValue.Split(':')[1]);
            //    }
            //    var resultItem = new BLADB().SaveEntityADB(objADBMaster, Convert.ToInt32(Session["user_id"]));
            //    //Save Reference
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.ADB.ToString() };

            //        if (objADBMaster.EntityReference != null && resultItem.system_id > 0)
            //        {

            //            SaveReference(objADBMaster.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }


            //        else
            //        {
            //            if (resultItem.isPortConnected == true)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            }
            //        }
            //        objADBMaster.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objADBMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objADBMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objADBMaster, EntityType.ADB.ToString());
            //    new MiscHelper().BindPortDetails(objADBMaster, EntityType.ADB.ToString(), DropDownType.Adb_Port_Ratio.ToString());
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    BindADBDropDown(objADBMaster);
            //    fillProjectSpecifications(objADBMaster);
            //    return PartialView("_AddADB", objADBMaster);
            //}
            objADBMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objADBMaster.isDirectSave = isDirectSave;
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objADBMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objADBMaster.source_ref_type = "NETWORK_TICKET";
                objADBMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ADBMaster>(url, objADBMaster, EntityType.ADB.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddADB", response.results);
        }

        private void BindADBDropDown(ADBMaster objADBMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.ADB.ToString());
            // objADBMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objADBMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objADBMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }

        public JsonResult DeleteADBById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            var result = 0;
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.ADB.ToString());
            //if (isNotAssociated == true) { result = new BLADB().DeleteADBById(systemId); }
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.ADB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = Resources.Resources.SI_OSP_ADB_NET_FRM_010;
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = Resources.Resources.SI_OSP_ADB_NET_FRM_011;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region BDB

        public PartialViewResult AddBDB(string pEntityType, string networkIdType, string geom = "", int systemId = 0, int pSystemId = 0, string pNetworkId = "")
        {
            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    // get geom by parent id...
            //    geom = GetPointTypeParentGeom(pSystemId, pEntityType);
            //}
            //BDBMaster objBDBMaster = GetBDBDetail(pSystemId, pEntityType, networkIdType, systemId, geom);
            //objBDBMaster.pSystemId = pSystemId;
            //objBDBMaster.pEntityType = pEntityType;

            //BLItemTemplate.Instance.BindItemDropdowns(objBDBMaster, EntityType.BDB.ToString());
            //fillProjectSpecifications(objBDBMaster);
            //if (systemId == 0 && pSystemId > 0)
            //{
            //    objBDBMaster.objIspEntityMap.structure_id = pSystemId;
            //    objBDBMaster.objIspEntityMap.AssociateStructure = pSystemId;
            //}
            ////if (objBDBMaster.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
            ////{
            ////    objBDBMaster.objIspEntityMap.structure_id = objBDBMaster.parent_system_id;
            ////    objBDBMaster.objIspEntityMap.AssociateStructure = objBDBMaster.parent_system_id;
            ////}
            //BindBDBDropDown(objBDBMaster);
            //return PartialView("_AddBDB", objBDBMaster);
            BDBMaster obj = new BDBMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<BDBMaster>(url, obj, EntityType.BDB.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddBDB", response.results);
        }
        public BDBMaster GetBDBDetail(int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
        {

            BDBMaster objBDB = new BDBMaster();
            objBDB.geom = geom;
            objBDB.networkIdType = networkIdType;
            if (systemId == 0)
            {
                objBDB.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objBDB.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objBDB.ownership_type = "Own";
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objBDB, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objBDB, new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDB.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType);
                //Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<BDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.BDB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objBDB);

                ////copy pin code and address from building itself for BDB box..
                //if (pEntityType.ToLower() == EntityType.Structure.ToString().ToLower())
                //{
                //    var objStructureDetail = new BLMisc().GetEntityDetailById<StructureMaster>(pSystemId, EntityType.Structure);
                //    objBDB.pincode=objStructureDetail
                //}
                //else if(pEntityType.ToLower() == EntityType.Building.ToString().ToLower())
                //{
                //    var objStructureDetail = new BLMisc().GetEntityDetailById<BuildingMaster>(pSystemId, EntityType.Building);
                //}
            }
            else
            {
                objBDB = new BLMisc().GetEntityDetailById<BDBMaster>(systemId, EntityType.BDB);
            }
            return objBDB;
        }
        private void BindBDBDropDown(BDBMaster objBDB)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objBDB.parent_system_id, objBDB.system_id, EntityType.BDB.ToString());
            if (ispEntityMap != null && ispEntityMap.id > 0)
            {
                objBDB.objIspEntityMap.id = ispEntityMap.id;
                objBDB.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objBDB.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objBDB.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objBDB.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }
            objBDB.objIspEntityMap.AssoType = objBDB.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objBDB.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objBDB.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objBDB.longitude + " " + objBDB.latitude);
            if (objBDB.objIspEntityMap.structure_id > 0)
            {
                var objDDL = new BLBDB().GetShaftFloorByStrucId(objBDB.objIspEntityMap.structure_id);
                objBDB.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
                objBDB.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

                if (objBDB.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objBDB.objIspEntityMap.unitId = objBDB.parent_system_id;
                    //objONT.objIspEntityMap.AssoType = "";
                    //objONT.objIspEntityMap.floor_id = 0;
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.BDB.ToString()).FirstOrDefault() != null)
            {
                objBDB.objIspEntityMap.isValidParent = true;
                objBDB.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objBDB.objIspEntityMap.structure_id, objBDB.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.BDB.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objBDB.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objBDB.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            if (objBDB.objIspEntityMap.entity_type == null) { objBDB.objIspEntityMap.entity_type = EntityType.BDB.ToString(); }
            new BLMisc().BindPortDetails(objBDB, EntityType.BDB.ToString(), DropDownType.BDB_PORT_RATIO.ToString());
            var entityTypeDDL = new BLMisc().GetDropDownList(EntityType.BDB.ToString(), DropDownType.Entity_Type.ToString());
            objBDB.lstEntityType = entityTypeDDL;
            objBDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objBDB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }


        public ActionResult SaveBDB(BDBMaster objBDBMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();

            //// get parent geometry 
            //if (string.IsNullOrWhiteSpace(objBDBMaster.geom) && objBDBMaster.system_id == 0)
            //{
            //    objBDBMaster.geom = GetPointTypeParentGeom(objBDBMaster.pSystemId, objBDBMaster.pEntityType);
            //}

            //if (objBDBMaster.networkIdType == NetworkIdType.A.ToString() && objBDBMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDBMaster.geom, parent_eType = objBDBMaster.pEntityType, parent_sysId = objBDBMaster.pSystemId });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objBDBMaster = GetBDBDetail(objBDBMaster.pSystemId, objBDBMaster.pEntityType, objBDBMaster.networkIdType, objBDBMaster.system_id, objBDBMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objBDBMaster.bdb_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objBDBMaster.network_id = objNetworkCodeDetail.network_code;
            //    objBDBMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}

            //if (TryValidateModel(objBDBMaster))
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.BDB.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    objBDBMaster.objIspEntityMap.structure_id = Convert.ToInt32(objBDBMaster.objIspEntityMap.AssociateStructure);
            //    objBDBMaster.objIspEntityMap.shaft_id = objBDBMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objBDBMaster.objIspEntityMap.shaft_id;
            //    if (string.IsNullOrEmpty(objBDBMaster.objIspEntityMap.AssoType))
            //    {
            //        objBDBMaster.objIspEntityMap.shaft_id = 0; objBDBMaster.objIspEntityMap.floor_id = 0;
            //    }
            //    var isNew = objBDBMaster.system_id > 0 ? false : true;
            //    if (objBDBMaster.unitValue != null && objBDBMaster.unitValue.Contains(":"))
            //    {
            //        objBDBMaster.no_of_input_port = Convert.ToInt32(objBDBMaster.unitValue.Split(':')[0]);
            //        objBDBMaster.no_of_output_port = Convert.ToInt32(objBDBMaster.unitValue.Split(':')[1]);
            //    }
            //    if (objBDBMaster.objIspEntityMap.structure_id == 0)
            //    {
            //        var objIn = new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDBMaster.longitude + " " + objBDBMaster.latitude };
            //        var parentDetails = new BLMisc().getParentInfo(objIn);
            //        if (parentDetails != null)
            //        {
            //            objBDBMaster.parent_system_id = parentDetails.p_system_id;
            //            objBDBMaster.parent_network_id = parentDetails.p_network_id;
            //            objBDBMaster.parent_entity_type = parentDetails.p_entity_type;
            //        }
            //    }
            //    var resultItem = new BLBDB().SaveEntityBDB(objBDBMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {


            //        //Save Reference
            //        if (objBDBMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objBDBMaster.EntityReference, resultItem.system_id);
            //        }

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, EntityType.BDB.ToString()); ;
            //        }
            //        else
            //        {
            //            if (resultItem.isPortConnected)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
            //            }
            //        }
            //        objBDBMaster.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objBDBMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objBDBMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objBDBMaster, EntityType.BDB.ToString());
            //    BindBDBDropDown(objBDBMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA

            //    fillProjectSpecifications(objBDBMaster);
            //    return PartialView("_AddBDB", objBDBMaster);
            //}

            objBDBMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objBDBMaster.isDirectSave = isDirectSave;
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objBDBMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objBDBMaster.source_ref_type = "NETWORK_TICKET";
                objBDBMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<BDBMaster>(url, objBDBMaster, EntityType.BDB.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddBDB", response.results);
        }
        #endregion

        #region CDB

        public PartialViewResult AddCDB(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "", int no_of_ports = 0, int vendor_id = 0, bool isConvert = false, int sc_system_id = 0)
        {

            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    // get geom by parent id...
            //    geom = GetPointTypeParentGeom(pSystemId, pEntityType);
            //}
            //CDBMaster objCDBMaster = GetCDBDetail(pSystemId, pEntityType, networkIdType, systemId, no_of_ports, vendor_id, isConvert, sc_system_id, geom);
            //BLItemTemplate.Instance.BindItemDropdowns(objCDBMaster, EntityType.CDB.ToString());
            //new MiscHelper().BindPortDetails(objCDBMaster, EntityType.CDB.ToString(), DropDownType.Cdb_Port_Ratio.ToString());
            //BindCDBDropDown(objCDBMaster);
            //fillProjectSpecifications(objCDBMaster);
            //objCDBMaster.pNetworkId = pNetworkId;
            //var entityTypeDDL = new BLMisc().GetDropDownList(EntityType.CDB.ToString(), DropDownType.Entity_Type.ToString());
            //objCDBMaster.lstEntityType = entityTypeDDL;
            //return PartialView("_AddCDB", objCDBMaster);

            CDBMaster obj = new CDBMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            obj.no_of_ports = no_of_ports;
            obj.vendor_id = vendor_id;
            obj.isConvert = isConvert;
            obj.sc_system_id = sc_system_id;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CDBMaster>(url, obj, EntityType.CDB.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddCDB", response.results);
        }

        public CDBMaster GetCDBDetail(int pSystemId, string pEntityType, string networkIdType, int systemId, int no_of_ports, int vendor_id, bool isConvert, int sc_system_id, string geom = "")
        {
            CDBMaster objCDB = new CDBMaster();
            objCDB.geom = geom;
            objCDB.isConvert = isConvert;
            objCDB.networkIdType = networkIdType;
            objCDB.sc_system_id = sc_system_id;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objCDB, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objCDB, new NetworkCodeIn() { eType = EntityType.CDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCDB.geom }, networkIdType);
                objCDB.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objCDB.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objCDB.ownership_type = "Own";
                // Item template binding
                if (isConvert)
                {
                    objCDB.is_servingdb = true;
                    //var objItem = new BLCDBItemMaster().getCDBTemplatebyPortNo(no_of_ports, EntityType.CDB.ToString(), vendor_id); 
                    var objItem = new BLVendorSpecification().getEntityTemplatebyPortNo(no_of_ports, EntityType.CDB.ToString(), vendor_id);
                    objCDB.entity_category = "Primary";
                    objCDB.vendor_id = objItem.vendor_id;
                    objCDB.specification = objItem.specification;
                    objCDB.subcategory1 = objItem.subcategory_1;
                    objCDB.subcategory2 = objItem.subcategory_2;
                    objCDB.subcategory3 = objItem.subcategory_3;
                    objCDB.no_of_port = objItem.no_of_port;
                    objCDB.category = objItem.category_reference;
                    objCDB.item_code = objItem.code;
                    //var objItem = BLItemTemplate.Instance.GetTemplateDetail<CDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.CDB);
                    //Utility.MiscHelper.CopyMatchingProperties(objItem, objCDB);
                }
                else
                {
                    var objItem = BLItemTemplate.Instance.GetTemplateDetail<CDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.CDB);
                    Utility.MiscHelper.CopyMatchingProperties(objItem, objCDB);
                }
            }
            else
            {
                // Get entity detail by Id...
                objCDB = new BLMisc().GetEntityDetailById<CDBMaster>(systemId, EntityType.CDB);
            }
            return objCDB;
        }
        public ActionResult SaveCDB(CDBMaster objCDBMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int pSystemId = objCDBMaster.pSystemId;
            //string pEntitytype = objCDBMaster.pEntityType;
            //string pNetworkId = objCDBMaster.pNetworkId;
            //if (objCDBMaster.networkIdType == NetworkIdType.A.ToString() && objCDBMaster.system_id == 0)
            //{// get parent geometry 
            //    if (string.IsNullOrWhiteSpace(objCDBMaster.geom) && objCDBMaster.system_id == 0)
            //    {
            //        objCDBMaster.geom = GetPointTypeParentGeom(objCDBMaster.pSystemId, objCDBMaster.pEntityType);
            //    }
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.CDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCDBMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objCDBMaster = GetCDBDetail(objCDBMaster.pSystemId, objCDBMaster.pEntityType, objCDBMaster.networkIdType, objCDBMaster.system_id, objCDBMaster.no_of_port, objCDBMaster.vendor_id, objCDBMaster.isConvert, objCDBMaster.sc_system_id, objCDBMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objCDBMaster.cdb_name = objNetworkCodeDetail.network_code;
            //        objCDBMaster.pSystemId = pSystemId;
            //        objCDBMaster.pEntityType = pEntitytype;
            //        objCDBMaster.pNetworkId = pNetworkId;
            //    }
            //    //SET NETWORK CODE
            //    objCDBMaster.network_id = objNetworkCodeDetail.network_code;
            //    objCDBMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            ////CDBTemplateMaster objItem = new CDBTemplateMaster();

            //if (TryValidateModel(objCDBMaster))
            //{
            //    var isNew = objCDBMaster.system_id > 0 ? false : true;
            //    if (objCDBMaster.unitValue != null && objCDBMaster.unitValue.Contains(":"))
            //    {
            //        objCDBMaster.no_of_input_port = Convert.ToInt32(objCDBMaster.unitValue.Split(':')[0]);
            //        objCDBMaster.no_of_output_port = Convert.ToInt32(objCDBMaster.unitValue.Split(':')[1]);
            //    }
            //    var resultItem = new BLCDB().SaveEntityCDB(objCDBMaster, Convert.ToInt32(Session["user_id"]));
            //    if (resultItem.isConvert && string.IsNullOrEmpty(resultItem.objPM.message) && isNew)
            //    {
            //        string[] LayerName = { EntityType.SpliceClosure.ToString(), EntityType.CDB.ToString() };
            //        SCMaster objSC = new SCMaster();
            //        objSC = new BLMisc().GetEntityDetailById<SCMaster>(objCDBMaster.sc_system_id, EntityType.SpliceClosure);
            //        //====  VALIDATE CONNECTION AND SPLICING===// 
            //        var response = new BLMisc().EntityConversion(EntityType.SpliceClosure.ToString(), objSC.network_id, objSC.system_id, resultItem.entityType, resultItem.network_id, resultItem.system_id, objSC.geom, Convert.ToInt32(Session["user_id"]));
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SC_NET_FRM_021, ApplicationSettings.listLayerDetails, LayerName);
            //        resultItem.objPM = objPM;

            //        //Save Reference
            //        if (objCDBMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objCDBMaster.EntityReference, resultItem.system_id);
            //        }

            //    }
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.CDB.ToString() };
            //        //Save Reference
            //        if (objCDBMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objCDBMaster.EntityReference, resultItem.system_id);
            //        }

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            if (resultItem.isPortConnected == true)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            }

            //        }
            //        objCDBMaster.objPM = objPM;
            //    }
            //}
            //else
            //{

            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objCDBMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objCDBMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objCDBMaster, EntityType.CDB.ToString());
            //    new MiscHelper().BindPortDetails(objCDBMaster, EntityType.CDB.ToString(), DropDownType.Cdb_Port_Ratio.ToString());
            //    BindCDBDropDown(objCDBMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objCDBMaster);
            //    return PartialView("_AddCDB", objCDBMaster);
            //}

            objCDBMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objCDBMaster.isDirectSave = isDirectSave;

            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objCDBMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objCDBMaster.source_ref_type = "NETWORK_TICKET";
                objCDBMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CDBMaster>(url, objCDBMaster, EntityType.CDB.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddCDB", response.results);
        }
        private void BindCDBDropDown(CDBMaster objCDBMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.CDB.ToString());
            //objCDBMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objCDBMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objCDBMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }

        public JsonResult DeleteCDBById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            var result = 0;
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.CDB.ToString());
            //if (isNotAssociated == true) { result = new BLCDB().DeleteCDBById(systemId); }

            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.CDB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = Resources.Resources.SI_OSP_CDB_NET_FRM_008;
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_295;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region POD
        public PODMaster GetPODDetail(string networkIdType, int systemId, string geom = "")
        {
            PODMaster objPOD = new PODMaster();
            objPOD.geom = geom;
            objPOD.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                objPOD.ownership_type = "Own";
                fillRegionProvinceDetail(objPOD, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objPOD, new NetworkCodeIn() { eType = EntityType.POD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPOD.geom }, networkIdType);
                objPOD.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objPOD.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<PODTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.POD);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objPOD);
            }
            else
            {
                // Get entity detail by Id...
                objPOD = new BLMisc().GetEntityDetailById<PODMaster>(systemId, EntityType.POD);
            }
            return objPOD;
        }
        public PartialViewResult AddPOD(string networkIdType, int systemId = 0, string geom = "")
        {

            //PODMaster objPODMaster = GetPODDetail(networkIdType, systemId, geom);
            //BLItemTemplate.Instance.BindItemDropdowns(objPODMaster, EntityType.POD.ToString());
            //fillProjectSpecifications(objPODMaster);
            ////if (objPODMaster.parent_system_id > 0)
            ////{
            ////    objPODMaster.objIspEntityMap.structure_id = objPODMaster.parent_system_id;
            ////    objPODMaster.objIspEntityMap.AssociateStructure = objPODMaster.parent_system_id;
            ////}
            //BindPODDropDown(objPODMaster);
            //return PartialView("_AddPOD", objPODMaster);
            
            PODMaster obj = new PODMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PODMaster>(url, obj, EntityType.POD.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddPOD", response.results);
        }
        private void BindPODDropDown(PODMaster objPOD)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objPOD.parent_system_id, objPOD.system_id, EntityType.POD.ToString());
            if (ispEntityMap != null)
            {
                objPOD.objIspEntityMap.id = ispEntityMap.id;
                objPOD.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objPOD.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objPOD.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objPOD.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }

            objPOD.objIspEntityMap.AssoType = objPOD.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objPOD.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objPOD.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objPOD.longitude + " " + objPOD.latitude);

            if (objPOD.objIspEntityMap.structure_id > 0)
            {
                var objDDL = new BLBDB().GetShaftFloorByStrucId(objPOD.objIspEntityMap.structure_id);
                objPOD.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
                objPOD.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

                if (objPOD.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objPOD.objIspEntityMap.unitId = objPOD.parent_system_id;
                    //objPOD.objIspEntityMap.AssoType = "";
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.POD.ToString()).FirstOrDefault() != null)
            {
                objPOD.objIspEntityMap.isValidParent = true;
                objPOD.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objPOD.objIspEntityMap.structure_id, objPOD.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.POD.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objPOD.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objPOD.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            if (objPOD.objIspEntityMap.entity_type == null) { objPOD.objIspEntityMap.entity_type = EntityType.POD.ToString(); }
            //objPOD.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            var obj_DDL = new BLMisc().GetDropDownList(EntityType.POD.ToString());
            objPOD.listPODType = obj_DDL.Where(x => x.dropdown_type == DropDownType.POD_Type.ToString()).ToList();
            objPOD.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objPOD.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        public ActionResult SavePOD(PODMaster objPODMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objPODMaster.networkIdType == NetworkIdType.A.ToString() && objPODMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.POD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPODMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        if (objPODMaster.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
            //        {
            //            objPODMaster.objIspEntityMap.structure_id = objPODMaster.parent_system_id;
            //        }
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objPODMaster = GetPODDetail(objPODMaster.networkIdType, objPODMaster.system_id, objPODMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objPODMaster.pod_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objPODMaster.network_id = objNetworkCodeDetail.network_code;
            //    objPODMaster.sequence_id = objNetworkCodeDetail.sequence_id;

            //}
            //if (TryValidateModel(objPODMaster))
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.POD.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    objPODMaster.objIspEntityMap.structure_id = Convert.ToInt32(objPODMaster.objIspEntityMap.AssociateStructure);
            //    if (objPODMaster.objIspEntityMap.structure_id != 0)
            //    {
            //        var structureDetails = new BLISP().GetStructureById(objPODMaster.objIspEntityMap.structure_id);
            //        if (structureDetails != null && structureDetails.Count > 0)
            //        {
            //            objPODMaster.latitude = Convert.ToDecimal(structureDetails.First().latitude);
            //            objPODMaster.longitude = Convert.ToDecimal(structureDetails.First().longitude);
            //        }
            //    }
            //    objPODMaster.objIspEntityMap.shaft_id = objPODMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objPODMaster.objIspEntityMap.shaft_id;
            //    if (string.IsNullOrEmpty(objPODMaster.objIspEntityMap.AssoType))
            //    {
            //        objPODMaster.objIspEntityMap.shaft_id = 0; objPODMaster.objIspEntityMap.floor_id = 0;
            //    }
            //    if (objPODMaster.objIspEntityMap.structure_id == 0)
            //    {
            //        var objIn = new NetworkCodeIn() { eType = EntityType.POD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPODMaster.longitude + " " + objPODMaster.latitude };
            //        var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
            //        objPODMaster.parent_system_id = networkCodeDetail.parent_system_id;
            //        objPODMaster.parent_network_id = networkCodeDetail.parent_network_id;
            //        objPODMaster.parent_entity_type = networkCodeDetail.parent_entity_type;
            //    }
            //    //if (objPODMaster.objIspEntityMap.structure_id != 0)
            //    //{
            //    //    objPODMaster.parent_system_id = Convert.ToInt32(objPODMaster.objIspEntityMap.structure_id);
            //    //    objPODMaster.parent_entity_type = EntityType.Structure.ToString();
            //    //    objPODMaster.latitude = objPODMaster.latitude;
            //    //    objPODMaster.longitude = objPODMaster.longitude;
            //    //}
            //    //else
            //    //{
            //    //    objPODMaster.parent_system_id = 0;
            //    //    objPODMaster.parent_entity_type = "Province";
            //    //}
            //    var isNew = objPODMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLPOD().SaveEntityPOD(objPODMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        //Save Reference
            //        if (objPODMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objPODMaster.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            //// Save geometry
            //            //InputGeom geom = new InputGeom();
            //            //geom.systemId = resultItem.system_id;
            //            //geom.longLat = resultItem.geom;
            //            //geom.userId = Convert.ToInt32(Session["user_id"]);
            //            //geom.entityType = EntityType.POD.ToString();
            //            //geom.commonName = resultItem.network_id;
            //            //geom.geomType = GeometryType.Point.ToString();
            //            //if (resultItem.parent_entity_type == EntityType.Structure.ToString())
            //            //{
            //            //    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
            //            //}
            //            //string chkGeomInsert = BASaveEntityGeometry.Instance.SaveEntityGeometry(geom);

            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_005, layer_title);
            //        }
            //        else
            //        {
            //            if (resultItem.isPortConnected)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
            //            }
            //            else
            //            {
            //                BLLoopMangment.Instance.UpdateLoopDetails(objPODMaster.system_id, EntityType.POD.ToString(), objPODMaster.network_id, objPODMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPODMaster.longitude + " " + objPODMaster.latitude }, Convert.ToInt32(Session["user_id"]));
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
            //            }
            //        }

            //        objPODMaster.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objPODMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objPODMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objPODMaster, EntityType.POD.ToString());
            //    // RETURN PARTIAL VIEW WITH MODEL DATA              
            //    fillProjectSpecifications(objPODMaster);
            //    BindPODDropDown(objPODMaster);
            //    return PartialView("_AddPOD", objPODMaster);
            //}
            objPODMaster.isDirectSave = isDirectSave;
            objPODMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objPODMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objPODMaster.source_ref_type = "NETWORK_TICKET";
                objPODMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PODMaster>(url, objPODMaster, EntityType.POD.ToString(), EntityAction.Save.ToString());

            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddPOD", response.results);
        }

        public JsonResult DeletePODById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();

            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.POD.ToString());
            //if (isNotAssociated == true) { result = new BLPOD().DeletePODById(systemId); }

            var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.POD.ToString().ToUpper()).FirstOrDefault().layer_title;
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.POD.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);

            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = string.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_296, layer_title);
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = string.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_297, layer_title);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);


            //JsonResponse<string> objResp = new JsonResponse<string>();
            //var result = new BLPOD().DeletePODById(systemId);
            //if (result == 1)
            //{
            //    objResp.status = ResponseStatus.OK.ToString();
            //    objResp.message = "POD has deleted successfully!";
            //}
            //else
            //{
            //    objResp.status = ResponseStatus.FAILED.ToString();
            //    objResp.message = "Something went wrong while deleting POD!";
            //}
            //return Json(objResp, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region MPOD
        public MPODMaster GetMPODDetail(string networkIdType, int systemId, string geom = "")
        {
            MPODMaster objMPOD = new MPODMaster();
            objMPOD.geom = geom;
            objMPOD.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objMPOD, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objMPOD, new NetworkCodeIn() { eType = EntityType.MPOD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPOD.geom }, networkIdType);
                objMPOD.longitude = Convert.ToDecimal(geom.Split(' ')[0]);
                objMPOD.latitude = Convert.ToDecimal(geom.Split(' ')[1]);
                objMPOD.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<MPODTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.MPOD);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objMPOD);
            }
            else
            {
                // Get entity detail by Id...
                objMPOD = new BLMisc().GetEntityDetailById<MPODMaster>(systemId, EntityType.MPOD);
            }
            return objMPOD;
        }
        public PartialViewResult AddMPOD(string networkIdType, int systemId = 0, string geom = "")
        {
            //MPODMaster objMPODMaster = GetMPODDetail(networkIdType, systemId, geom);
            //BLItemTemplate.Instance.BindItemDropdowns(objMPODMaster, EntityType.MPOD.ToString());
            //fillProjectSpecifications(objMPODMaster);
            ////if (objMPODMaster.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
            ////{
            ////    objMPODMaster.objIspEntityMap.structure_id = objMPODMaster.parent_system_id;
            ////    objMPODMaster.objIspEntityMap.AssociateStructure = objMPODMaster.parent_system_id;
            ////}
            //BindMPODDropDown(objMPODMaster);
            //return PartialView("_AddMPOD", objMPODMaster);
            MPODMaster obj = new MPODMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<MPODMaster>(url, obj, EntityType.MPOD.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddMPOD", response.results);
        }
        private void BindMPODDropDown(MPODMaster objMPOD)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objMPOD.parent_system_id, objMPOD.system_id, EntityType.MPOD.ToString());
            if (ispEntityMap != null)
            {
                objMPOD.objIspEntityMap.id = ispEntityMap.id;
                objMPOD.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objMPOD.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objMPOD.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objMPOD.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }

            objMPOD.objIspEntityMap.AssoType = objMPOD.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objMPOD.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objMPOD.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objMPOD.longitude + " " + objMPOD.latitude);
            if (objMPOD.objIspEntityMap.structure_id > 0)
            {
                var objDDL = new BLBDB().GetShaftFloorByStrucId(objMPOD.objIspEntityMap.structure_id);
                objMPOD.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
                objMPOD.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

                if (objMPOD.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objMPOD.objIspEntityMap.unitId = objMPOD.parent_system_id;
                    //objMPOD.objIspEntityMap.AssoType = "";
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.MPOD.ToString()).FirstOrDefault() != null)
            {
                objMPOD.objIspEntityMap.isValidParent = true;
                objMPOD.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objMPOD.objIspEntityMap.structure_id, objMPOD.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.MPOD.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objMPOD.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objMPOD.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            if (objMPOD.objIspEntityMap.entity_type == null) { objMPOD.objIspEntityMap.entity_type = EntityType.MPOD.ToString(); }
            objMPOD.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objMPOD.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
            var obj_DDL = new BLMisc().GetDropDownList(EntityType.MPOD.ToString());
            objMPOD.listMPODType = obj_DDL.Where(x => x.dropdown_type == DropDownType.MPOD_Type.ToString()).ToList();
        }
        public ActionResult SaveMPOD(MPODMaster objMPODMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objMPODMaster.networkIdType == NetworkIdType.A.ToString() && objMPODMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.MPOD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPODMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objMPODMaster = GetMPODDetail(objMPODMaster.networkIdType, objMPODMaster.system_id, objMPODMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objMPODMaster.mpod_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objMPODMaster.network_id = objNetworkCodeDetail.network_code;
            //    objMPODMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (TryValidateModel(objMPODMaster))
            //{

            //    objMPODMaster.objIspEntityMap.structure_id = Convert.ToInt32(objMPODMaster.objIspEntityMap.AssociateStructure);
            //    if (objMPODMaster.objIspEntityMap.structure_id != 0)
            //    {
            //        var structureDetails = new BLISP().GetStructureById(objMPODMaster.objIspEntityMap.structure_id);
            //        if (structureDetails != null && structureDetails.Count > 0)
            //        {
            //            objMPODMaster.latitude = Convert.ToDecimal(structureDetails.First().latitude);
            //            objMPODMaster.longitude = Convert.ToDecimal(structureDetails.First().longitude);
            //        }

            //    }
            //    objMPODMaster.objIspEntityMap.shaft_id = objMPODMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objMPODMaster.objIspEntityMap.shaft_id;
            //    if (string.IsNullOrEmpty(objMPODMaster.objIspEntityMap.AssoType))
            //    {
            //        objMPODMaster.objIspEntityMap.shaft_id = 0; objMPODMaster.objIspEntityMap.floor_id = 0;
            //    }
            //    if (objMPODMaster.objIspEntityMap.structure_id == 0)
            //    {
            //        var objIn = new NetworkCodeIn() { eType = EntityType.MPOD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPODMaster.longitude + " " + objMPODMaster.latitude };
            //        var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
            //        objMPODMaster.parent_system_id = networkCodeDetail.parent_system_id;
            //        objMPODMaster.parent_network_id = networkCodeDetail.parent_network_id;
            //        objMPODMaster.parent_entity_type = networkCodeDetail.parent_entity_type;
            //    }
            //    //if (objMPODMaster.objIspEntityMap.structure_id != 0)
            //    //{
            //    //    objMPODMaster.parent_system_id = Convert.ToInt32(objMPODMaster.objIspEntityMap.structure_id);
            //    //    objMPODMaster.parent_entity_type = EntityType.Structure.ToString();
            //    //    objMPODMaster.latitude = objMPODMaster.latitude;
            //    //    objMPODMaster.longitude = objMPODMaster.longitude;
            //    //}
            //    //else
            //    //{
            //    //    objMPODMaster.parent_system_id = 0;
            //    //    objMPODMaster.parent_entity_type = "Province";
            //    //}
            //    var isNew = objMPODMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLMPOD().SaveEntityMPOD(objMPODMaster, Convert.ToInt32(Session["user_id"]));
            //    //Save Reference
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        if (objMPODMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objMPODMaster.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = Resources.Resources.SI_OSP_MPOD_NET_FRM_009;
            //        }
            //        else
            //        {
            //            if (resultItem.isPortConnected)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
            //            }
            //            else
            //            {
            //                BLLoopMangment.Instance.UpdateLoopDetails(objMPODMaster.system_id, EntityType.MPOD.ToString(), objMPODMaster.network_id, objMPODMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPODMaster.longitude + " " + objMPODMaster.latitude }, Convert.ToInt32(Session["user_id"]));
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = Resources.Resources.SI_OSP_MPOD_NET_FRM_008;
            //            }
            //        }
            //        objMPODMaster.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objMPODMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objMPODMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objMPODMaster, EntityType.MPOD.ToString());
            //    BindMPODDropDown(objMPODMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objMPODMaster);
            //    return PartialView("_AddMPOD", objMPODMaster);
            //}
            objMPODMaster.isDirectSave = isDirectSave;
            objMPODMaster.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<MPODMaster>(url, objMPODMaster, EntityType.MPOD.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddMPOD", response.results);
        }
        public JsonResult DeleteMPODById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            var result = 0;

            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.MPOD.ToString());
            //if (isNotAssociated == true) { result = new BLMPOD().DeleteMPODById(systemId); }

            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.MPOD.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = Resources.Resources.SI_OSP_MPOD_NET_FRM_012;
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = Resources.Resources.SI_OSP_MPOD_NET_FRM_013;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);



            //JsonResponse<string> objResp = new JsonResponse<string>();
            //var result = new BLMPOD().DeleteMPODById(systemId);
            //if (result == 1)
            //{
            //    objResp.status = ResponseStatus.OK.ToString();
            //    objResp.message = "MPOD has deleted successfully!";
            //}
            //else
            //{
            //    objResp.status = ResponseStatus.FAILED.ToString();
            //    objResp.message = "Something went wrong while deleting POD!";
            //}
            //return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Pole

        public PartialViewResult AddPole(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //PoleMaster objPoleMaster = GetPoleDetail(networkIdType, system_id,geom);
            //BindPoleDropDown(objPoleMaster);
            // BLItemTemplate.Instance.BindItemDropdowns(objPoleMaster, EntityType.Pole.ToString());
            //fillProjectSpecifications(objPoleMaster);
            //return PartialView("_AddPole", objPoleMaster);
            //public PartialViewResult AddPole(PoleMaster obj)
            PoleMaster obj = new PoleMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PoleMaster>(url, obj, EntityType.Pole.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddPole", response.results);
        }

        public PoleMaster GetPoleDetail(string networkIdType, int systemId, string geom = "")
        {
            PoleMaster objPole = new PoleMaster();
            objPole.geom = geom;
            objPole.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objPole, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objPole, new NetworkCodeIn() { eType = EntityType.Pole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPole.geom }, networkIdType);
                objPole.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objPole.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objPole.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<PoleTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Pole);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objPole);
            }
            else
            {
                // Get entity detail by Id...
                objPole = new BLMisc().GetEntityDetailById<PoleMaster>(systemId, EntityType.Pole);
            }
            return objPole;
        }

        public ActionResult SavePole(PoleMaster objPoleMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objPoleMaster.networkIdType == NetworkIdType.A.ToString() && objPoleMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Pole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPoleMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objPoleMaster = GetPoleDetail(objPoleMaster.networkIdType, objPoleMaster.system_id, objPoleMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objPoleMaster.pole_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objPoleMaster.network_id = objNetworkCodeDetail.network_code;
            //    objPoleMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (TryValidateModel(objPoleMaster))
            //{
            //    var isNew = objPoleMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLPole().SaveEntityPole(objPoleMaster, Convert.ToInt32(Session["user_id"]));

            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {


            //        //Save Reference
            //        if (objPoleMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objPoleMaster.EntityReference, resultItem.system_id);
            //        }

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = Resources.Resources.SI_OSP_POL_NET_FRM_010;
            //        }
            //        else
            //        {
            //            BLLoopMangment.Instance.UpdateLoopDetails(objPoleMaster.system_id, "Pole", objPoleMaster.network_id, objPoleMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPoleMaster.longitude + " " + objPoleMaster.latitude }, Convert.ToInt32(Session["user_id"]));
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = Resources.Resources.SI_OSP_POL_NET_FRM_011;
            //        }
            //        objPoleMaster.objPM = objPM;
            //    }

            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objPoleMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objPoleMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objPoleMaster, EntityType.Pole.ToString());
            //    BindPoleDropDown(objPoleMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objPoleMaster);
            //    return PartialView("_AddPole", objPoleMaster);
            //}
            objPoleMaster.isDirectSave = isDirectSave;
            objPoleMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objPoleMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objPoleMaster.source_ref_type = "NETWORK_TICKET";
                objPoleMaster.status = "D";

            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PoleMaster>(url, objPoleMaster, EntityType.Pole.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddPole", response.results);
        }
        private void BindPoleDropDown(PoleMaster objPoleMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Pole.ToString());
            objPoleMaster.lstPoleType = objDDL.Where(x => x.dropdown_type == DropDownType.Pole_Type.ToString()).ToList();
            //objPoleMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objPoleMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objPoleMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        #endregion

        #region Tree

        public PartialViewResult AddTree(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //TreeMaster objTreeMaster = GetTreeDetail(networkIdType, systemId, geom);
            //BLItemTemplate.Instance.BindItemDropdowns(objTreeMaster, EntityType.Tree.ToString());
            //fillProjectSpecifications(objTreeMaster);
            //return PartialView("_AddTree", objTreeMaster);

            TreeMaster obj = new TreeMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pEntityType = pEntityType;
            obj.pSystemId = pSystemId;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TreeMaster>(url, obj, EntityType.Tree.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddTree", response.results);
        }

        public TreeMaster GetTreeDetail(string networkIdType, int systemId, string geom = "")
        {
            TreeMaster objTree = new TreeMaster();
            var userdetails = (User)Session["userDetail"];
            objTree.geom = geom;
            objTree.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objTree, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objTree, new NetworkCodeIn() { eType = EntityType.Tree.ToString(), gType = GeometryType.Point.ToString(), eGeom = objTree.geom }, networkIdType);
                objTree.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objTree.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<TreeTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Tree);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objTree);
            }
            else
            {
                // Get entity detail by Id...
                objTree = new BLMisc().GetEntityDetailById<TreeMaster>(systemId, EntityType.Tree);
            }
            objTree.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objTree;
        }

        public ActionResult SaveTree(TreeMaster objTreeMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objTreeMaster.networkIdType == NetworkIdType.A.ToString() && objTreeMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Tree.ToString(), gType = GeometryType.Point.ToString(), eGeom = objTreeMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objTreeMaster = GetTreeDetail(objTreeMaster.networkIdType, objTreeMaster.system_id, objTreeMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objTreeMaster.tree_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objTreeMaster.network_id = objNetworkCodeDetail.network_code;
            //    objTreeMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (TryValidateModel(objTreeMaster))
            //{
            //    var isNew = objTreeMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLTree().SaveEntityTree(objTreeMaster, Convert.ToInt32(Session["user_id"]));

            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {


            //        //Save Reference
            //        if (objTreeMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objTreeMaster.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = Resources.Resources.SI_OSP_TRE_NET_FRM_017;
            //        }
            //        else
            //        {
            //            BLLoopMangment.Instance.UpdateLoopDetails(objTreeMaster.system_id, "Tree", objTreeMaster.network_id, objTreeMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objTreeMaster.longitude + " " + objTreeMaster.latitude }, Convert.ToInt32(Session["user_id"]));
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = Resources.Resources.SI_OSP_TRE_NET_FRM_018;
            //        }
            //        objTreeMaster.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objTreeMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objTreeMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objTreeMaster, EntityType.Tree.ToString());

            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objTreeMaster);
            //    return PartialView("_AddTree", objTreeMaster);
            //}

            objTreeMaster.isDirectSave = isDirectSave;
            objTreeMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objTreeMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objTreeMaster.source_ref_type = "NETWORK_TICKET";
                objTreeMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TreeMaster>(url, objTreeMaster, EntityType.Tree.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddTree", response.results);
        }

        #endregion

        #region Manhole
        public PartialViewResult AddManhole(string networkIdType, int systemId = 0, string geom = "", string pEntityType = "", string pNetworkId = "", int pSystemId = 0)
        {
            //ManholeMaster objManholeMaster = GetManholeDetail(networkIdType, systemId, geom);
            //BLItemTemplate.Instance.BindItemDropdowns(objManholeMaster, EntityType.Manhole.ToString());
            //fillProjectSpecifications(objManholeMaster);
            //BindManholeDropDown(objManholeMaster);
            //objManholeMaster.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Manhole.ToString()).ToList();
            //return PartialView("_AddManhole", objManholeMaster);

            ManholeMaster obj = new ManholeMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.pEntityType = pEntityType;
            obj.pSystemId = pSystemId;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ManholeMaster>(url, obj, EntityType.Manhole.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddManhole", response.results);
        }

        public ManholeMaster GetManholeDetail(string networkIdType, int systemId, string geom = "")
        {
            ManholeMaster objManhole = new ManholeMaster();
            objManhole.geom = geom;
            objManhole.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objManhole, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objManhole, new NetworkCodeIn() { eType = EntityType.Manhole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objManhole.geom }, networkIdType);
                objManhole.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objManhole.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objManhole.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<ManholeTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Manhole);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objManhole);
            }
            else
            {
                // Get entity detail by Id...
                objManhole = new BLMisc().GetEntityDetailById<ManholeMaster>(systemId, EntityType.Manhole);
            }
            objManhole.lstUserModule = new BLLayer().GetUserModuleAbbrList(objManhole.user_id, UserType.Web.ToString());
            return objManhole;
        }

        public ActionResult SaveManhole(ManholeMaster objManholeMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objManholeMaster.networkIdType == NetworkIdType.A.ToString() && objManholeMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Manhole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objManholeMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objManholeMaster = GetManholeDetail(objManholeMaster.networkIdType, objManholeMaster.system_id, objManholeMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objManholeMaster.manhole_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objManholeMaster.network_id = objNetworkCodeDetail.network_code;
            //    objManholeMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (TryValidateModel(objManholeMaster))
            //{
            //    var isNew = objManholeMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLManhole().SaveEntityManhole(objManholeMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        //Save Reference
            //        if (objManholeMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objManholeMaster.EntityReference, resultItem.system_id);
            //        }

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = Resources.Resources.SI_OSP_MH_NET_FRM_007;
            //        }
            //        else
            //        {
            //            BLLoopMangment.Instance.UpdateLoopDetails(objManholeMaster.system_id, EntityType.Manhole.ToString(), objManholeMaster.network_id, objManholeMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objManholeMaster.longitude + " " + objManholeMaster.latitude }, Convert.ToInt32(Session["user_id"]));

            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = Resources.Resources.SI_OSP_MH_NET_FRM_008;
            //        }
            //        objManholeMaster.objPM = objPM;


            //        //save AT Status                        
            //        if (objManholeMaster.ATAcceptance != null && objManholeMaster.system_id > 0)
            //        {
            //            SaveATAcceptance(objManholeMaster.ATAcceptance, objManholeMaster.system_id);
            //        }

            //    }

            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objManholeMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objManholeMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objManholeMaster, EntityType.Manhole.ToString());
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objManholeMaster);
            //    BindManholeDropDown(objManholeMaster);
            //    objManholeMaster.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Manhole.ToString()).ToList();
            //    return PartialView("_AddManhole", objManholeMaster);
            //}

            objManholeMaster.isDirectSave = isDirectSave;
            objManholeMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objManholeMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objManholeMaster.source_ref_type = "NETWORK_TICKET";
                objManholeMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ManholeMaster>(url, objManholeMaster, EntityType.Manhole.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_AddManhole", response.results);
        }
        private void BindManholeDropDown(ManholeMaster objManholeMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Manhole.ToString());
            objManholeMaster.listConstructionType = objDDL.Where(x => x.dropdown_type == DropDownType.Construction_Type.ToString()).ToList();
            objManholeMaster.listaerialLocation = objDDL.Where(x => x.dropdown_type == DropDownType.Aerial_Location.ToString()).ToList();
            //objManholeMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objManholeMaster.MCGMWardIn = objDDL.Where(x => x.dropdown_type == DropDownType.MCGM_Ward.ToString()).ToList();
            objManholeMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objManholeMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        #endregion

        #region Coupler
        public PartialViewResult AddCoupler(string networkIdType, int systemId = 0, string geom = "")
        {
            //CouplerMaster objCouplerMaster = GetCouplerDetail(networkIdType, systemId, geom);
            //BLItemTemplate.Instance.BindItemDropdowns(objCouplerMaster, EntityType.Coupler.ToString());
            //fillProjectSpecifications(objCouplerMaster);
            //BindCouplerDropDown(objCouplerMaster);
            //objCouplerMaster.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Coupler.ToString()).ToList();
            //return PartialView("_AddCoupler", objCouplerMaster);
            CouplerMaster obj = new CouplerMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CouplerMaster>(url, obj, EntityType.Coupler.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddCoupler", response.results);
        }

        public CouplerMaster GetCouplerDetail(string networkIdType, int systemId, string geom = "")
        {
            CouplerMaster objCoupler = new CouplerMaster();
            objCoupler.geom = geom;
            objCoupler.networkIdType = networkIdType;
            var userdetails = (User)Session["userDetail"];
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objCoupler, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objCoupler, new NetworkCodeIn() { eType = EntityType.Coupler.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCoupler.geom }, networkIdType);
                objCoupler.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objCoupler.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objCoupler.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<CouplerTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Coupler);
                MiscHelper.CopyMatchingProperties(objItem, objCoupler);
            }
            else
            {
                // Get entity detail by Id...
                objCoupler = new BLMisc().GetEntityDetailById<CouplerMaster>(systemId, EntityType.Coupler);
            }
            return objCoupler;
        }

        public ActionResult SaveCoupler(CouplerMaster objCouplerMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objCouplerMaster.networkIdType == NetworkIdType.A.ToString() && objCouplerMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Coupler.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCouplerMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objCouplerMaster = GetCouplerDetail(objCouplerMaster.networkIdType, objCouplerMaster.system_id, objCouplerMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objCouplerMaster.coupler_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objCouplerMaster.network_id = objNetworkCodeDetail.network_code;
            //    objCouplerMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (TryValidateModel(objCouplerMaster))
            //{
            //    var isNew = objCouplerMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLCoupler().SaveEntityCoupler(objCouplerMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        //Save Reference
            //        if (objCouplerMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objCouplerMaster.EntityReference, resultItem.system_id);
            //        }
            //        string[] LayerName = { EntityType.Coupler.ToString() };
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {

            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        objCouplerMaster.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objCouplerMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objCouplerMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objCouplerMaster, EntityType.Coupler.ToString());
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objCouplerMaster);
            //    BindCouplerDropDown(objCouplerMaster);
            //    objCouplerMaster.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Coupler.ToString()).ToList();
            //    return PartialView("_AddCoupler", objCouplerMaster);
            //}
            objCouplerMaster.isDirectSave = isDirectSave;
            objCouplerMaster.user_id = Convert.ToInt32(Session["user_id"]);

            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CouplerMaster>(url, objCouplerMaster, EntityType.Coupler.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_AddCoupler", response.results);
        }
        private void BindCouplerDropDown(CouplerMaster objCouplerMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Coupler.ToString());
            objCouplerMaster.listCouplerType = objDDL.Where(x => x.dropdown_type == DropDownType.Coupler.ToString()).ToList();
            //objCouplerMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objCouplerMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objCouplerMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        #endregion

        #region Splitter
        public PartialViewResult AddSplitter(string pEntityType, string networkIdType, string geom = "", int systemId = 0, int pSystemId = 0)
        {
            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    // get geom by parent id...
            //    geom = GetPointTypeParentGeom(pSystemId, pEntityType);
            //}
            //SplitterMaster objSplitterMaster = GetSplitterDetail(pSystemId, pEntityType, networkIdType, systemId, geom);
            //objSplitterMaster.pSystemId = pSystemId;
            //objSplitterMaster.pEntityType = pEntityType;
            //BLItemTemplate.Instance.BindItemDropdowns(objSplitterMaster, EntityType.Splitter.ToString());
            //BindSplitterDropDown(objSplitterMaster);
            //fillProjectSpecifications(objSplitterMaster);
            //objSplitterMaster.unitValue = objSplitterMaster.splitter_ratio;
            //return PartialView("_AddSplitter", objSplitterMaster);
            SplitterMaster objSplitterMaster = new SplitterMaster();
            itemMaster objItemMaster = new itemMaster();
            objSplitterMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objSplitterMaster.pEntityType = pEntityType;
            objSplitterMaster.pSystemId = pSystemId;
            objSplitterMaster.networkIdType = networkIdType;
            objSplitterMaster.geom = geom;
            objSplitterMaster.system_id = systemId;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SplitterMaster>(url, objSplitterMaster, EntityType.Splitter.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddSplitter", response.results);
        }
        public SplitterMaster GetSplitterDetail(int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
        {

            SplitterMaster objSplitter = new SplitterMaster();
            objSplitter.geom = geom;
            objSplitter.networkIdType = networkIdType;
            if (systemId == 0)
            {
                objSplitter.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objSplitter.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objSplitter.ownership_type = "Own";
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objSplitter, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objSplitter, new NetworkCodeIn() { eType = EntityType.Splitter.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSplitter.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType);
                //Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<SplitterTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Splitter);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objSplitter);
            }
            else
            {
                objSplitter = new BLMisc().GetEntityDetailById<SplitterMaster>(systemId, EntityType.Splitter);
            }
            return objSplitter;
        }
        public ActionResult SaveSplitter(SplitterMaster objSplitterMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();

            //// get parent geometry 
            //if (string.IsNullOrWhiteSpace(objSplitterMaster.geom) && objSplitterMaster.system_id == 0)
            //{
            //    objSplitterMaster.geom = GetPointTypeParentGeom(objSplitterMaster.pSystemId, objSplitterMaster.pEntityType);
            //}

            //if (objSplitterMaster.networkIdType == NetworkIdType.A.ToString() && objSplitterMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Splitter.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSplitterMaster.geom, parent_eType = objSplitterMaster.pEntityType, parent_sysId = objSplitterMaster.pSystemId });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objSplitterMaster = GetSplitterDetail(objSplitterMaster.pSystemId, objSplitterMaster.pEntityType, objSplitterMaster.networkIdType, objSplitterMaster.system_id, objSplitterMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objSplitterMaster.splitter_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objSplitterMaster.network_id = objNetworkCodeDetail.network_code;
            //    objSplitterMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (objSplitterMaster.unitValue != null && objSplitterMaster.unitValue.Contains(":"))
            //{
            //    objSplitterMaster.splitter_ratio = objSplitterMaster.unitValue;
            //}

            //if (TryValidateModel(objSplitterMaster))
            //{
            //    var isNew = objSplitterMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLSplitter().SaveSplitterEntity(objSplitterMaster, Convert.ToInt32(Session["user_id"]));
            //    if (String.IsNullOrEmpty(resultItem.objPM.message))
            //    {


            //        //Save Reference
            //        if (objSplitterMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objSplitterMaster.EntityReference, resultItem.system_id);
            //        }
            //        string[] LayerName = { EntityType.Splitter.ToString() };
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            if (resultItem.isPortConnected == true)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            }
            //        }
            //        objSplitterMaster.objPM = objPM;
            //    }
            //}
            //else
            //{

            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objSplitterMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objSplitterMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objSplitterMaster, EntityType.Splitter.ToString());
            //    BindSplitterDropDown(objSplitterMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objSplitterMaster);
            //    return PartialView("_AddSplitter", objSplitterMaster);
            //}

            objSplitterMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objSplitterMaster.isDirectSave = isDirectSave;
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objSplitterMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objSplitterMaster.source_ref_type = "NETWORK_TICKET";
                objSplitterMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SplitterMaster>(url, objSplitterMaster, EntityType.Splitter.ToString(), EntityAction.Save.ToString(), objSplitterMaster.structure_id.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddSplitter", response.results);
        }
        private void BindSplitterDropDown(SplitterMaster objSplitterMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Splitter.ToString());
            //objSplitterMaster.lstSplRatio = objDDL.Where(x => x.dropdown_type == DropDownType.Splitter_Ratio.ToString()).ToList();
            new BLMisc().BindPortDetails(objSplitterMaster, EntityType.Splitter.ToString(), DropDownType.Splitter_Ratio.ToString());
            objSplitterMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objSplitterMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        #endregion

        #region Splice Closure
        public SCMaster GetSCDetail(int pSystemId, string pEntityType, string networkIdType, int systemId, int no_of_ports, int vendor_id, bool isConvert, int cdb_system_id, string geom = "")
        {
            SCMaster objSC = new SCMaster();
            var userdetails = (User)Session["userDetail"];
            objSC.geom = geom;
            objSC.networkIdType = networkIdType;
            objSC.isConvert = isConvert;
            objSC.cdb_system_id = cdb_system_id;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objSC, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objSC, new NetworkCodeIn() { eType = EntityType.SpliceClosure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSC.geom }, networkIdType);
                objSC.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objSC.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objSC.ownership_type = "Own";
                // Item template binding
                if (isConvert)
                {
                    var objItem = new BLVendorSpecification().getEntityTemplatebyPortNo(no_of_ports, EntityType.SpliceClosure.ToString(), vendor_id);
                    objSC.vendor_id = objItem.vendor_id;
                    objSC.specification = objItem.specification;
                    objSC.subcategory1 = objItem.subcategory_1;
                    objSC.subcategory2 = objItem.subcategory_2;
                    objSC.subcategory3 = objItem.subcategory_3;
                    objSC.no_of_port = objItem.no_of_port;
                    objSC.category = objItem.category_reference;
                    objSC.item_code = objItem.code;
                }
                else
                {
                    var objItem = BLItemTemplate.Instance.GetTemplateDetail<SCTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.SpliceClosure);
                    Utility.MiscHelper.CopyMatchingProperties(objItem, objSC);
                    objSC.no_of_port = objItem.no_of_ports;
                }

            }
            else
            {
                // Get entity detail by Id...
                objSC = new BLMisc().GetEntityDetailById<SCMaster>(systemId, EntityType.SpliceClosure);
                objSC.no_of_port = objSC.no_of_ports;
            }
            objSC.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objSC;
        }

        public PartialViewResult AddSpliceClosure(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, int no_of_ports = 0, int vendor_id = 0, bool isConvert = false, int cdb_system_id = 0, string pEntityType = "", string pNetworkId = "")
        {
            SCMaster objSCMaster = new SCMaster();
            objSCMaster.networkIdType = networkIdType;
            objSCMaster.system_id = systemId;
            objSCMaster.geom = geom;
            objSCMaster.pSystemId = pSystemId;
            objSCMaster.no_of_ports = no_of_ports;
            objSCMaster.vendor_id = vendor_id;
            objSCMaster.isConvert = isConvert;
            objSCMaster.cdb_system_id = cdb_system_id;
            objSCMaster.pEntityType = pEntityType;
            objSCMaster.pNetworkId = pNetworkId;

            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    // get geom by parent id...
            //    geom = GetPointTypeParentGeom(pSystemId, pEntityType);
            //}
            //SCMaster objSCMaster = GetSCDetail(pSystemId, pEntityType, networkIdType, systemId, no_of_ports, vendor_id, isConvert, cdb_system_id, geom);

            //BLItemTemplate.Instance.BindItemDropdowns(objSCMaster, EntityType.SpliceClosure.ToString());
            //BindSpilceClosureDropdown(objSCMaster);
            //fillProjectSpecifications(objSCMaster);
            //new MiscHelper().BindPortDetails(objSCMaster, EntityType.SpliceClosure.ToString(), DropDownType.SC_Port_Ratio.ToString());
            //objSCMaster.pNetworkId = pNetworkId;
            //return PartialView("_AddSpliceClosure", objSCMaster);
            objSCMaster.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SCMaster>(url, objSCMaster, EntityType.SpliceClosure.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddSpliceClosure", response.results);
        }
        public ActionResult SaveSpliceClosure(SCMaster objSCMaster, bool isDirectSave = false)
        {
            //int pSystemId = objSCMaster.pSystemId;
            //string pEntitytype = objSCMaster.pEntityType;
            //string pNetworkId = objSCMaster.pNetworkId;
            //// get parent geometry 
            //if (string.IsNullOrWhiteSpace(objSCMaster.geom) && objSCMaster.system_id == 0)
            //{
            //    objSCMaster.geom = GetPointTypeParentGeom(objSCMaster.pSystemId, objSCMaster.pEntityType);
            //}

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objSCMaster.networkIdType == NetworkIdType.A.ToString() && objSCMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.SpliceClosure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSCMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objSCMaster = GetSCDetail(objSCMaster.pSystemId, objSCMaster.pEntityType, objSCMaster.networkIdType, objSCMaster.system_id, objSCMaster.no_of_ports, objSCMaster.vendor_id, objSCMaster.isConvert, objSCMaster.cdb_system_id, objSCMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objSCMaster.spliceclosure_name = objNetworkCodeDetail.network_code;
            //        objSCMaster.pSystemId = pSystemId;
            //        objSCMaster.pEntityType = pEntitytype;
            //        objSCMaster.pNetworkId = pNetworkId;
            //    }
            //    //SET NETWORK CODE
            //    objSCMaster.network_id = objNetworkCodeDetail.network_code;
            //    objSCMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (objSCMaster.unitValue != null && objSCMaster.unitValue.Contains(":"))
            //{
            //    objSCMaster.no_of_input_port = Convert.ToInt32(objSCMaster.unitValue.Split(':')[0]);
            //    objSCMaster.no_of_output_port = Convert.ToInt32(objSCMaster.unitValue.Split(':')[1]);
            //}
            //if (TryValidateModel(objSCMaster))
            //{
            //    var isNew = objSCMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLSC().SaveEntitySC(objSCMaster, Convert.ToInt32(Session["user_id"]));
            //    if (resultItem.isConvert && string.IsNullOrEmpty(resultItem.objPM.message) && isNew)
            //    {
            //        string[] LayerName = { EntityType.CDB.ToString(), EntityType.SpliceClosure.ToString() };
            //        CDBMaster objCDB = new CDBMaster();
            //        objCDB = new BLMisc().GetEntityDetailById<CDBMaster>(objSCMaster.cdb_system_id, EntityType.CDB);
            //        //====  VALIDATE CONNECTION AND SPLICING===// 
            //        var response = new BLMisc().EntityConversion(EntityType.CDB.ToString(), objCDB.network_id, objCDB.system_id, EntityType.SpliceClosure.ToString(), resultItem.network_id, resultItem.system_id, objCDB.geom, Convert.ToInt32(Session["user_id"]));
            //        //if (response.status=="OK")
            //        //{
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CDB_NET_FRM_007, ApplicationSettings.listLayerDetails, LayerName);
            //        resultItem.objPM = objPM;

            //        if (objSCMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objSCMaster.EntityReference, resultItem.system_id);
            //        }
            //        // }
            //    }
            //    //Save Reference 
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.SpliceClosure.ToString() };

            //        if (objSCMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objSCMaster.EntityReference, resultItem.system_id);
            //        }

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            if (resultItem.isPortConnected == true)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            }
            //        }
            //        objSCMaster.objPM = objPM;
            //    }
            //}
            //else
            //{

            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objSCMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objSCMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objSCMaster, EntityType.SpliceClosure.ToString());
            //    BindSpilceClosureDropdown(objSCMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objSCMaster);
            //    new MiscHelper().BindPortDetails(objSCMaster, EntityType.SpliceClosure.ToString(), DropDownType.SC_Port_Ratio.ToString());
            //    return PartialView("_AddSpliceClosure", objSCMaster);
            //}

            objSCMaster.isDirectSave = isDirectSave;
            objSCMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objSCMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objSCMaster.source_ref_type = "NETWORK_TICKET";
                objSCMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SCMaster>(url, objSCMaster, EntityType.SpliceClosure.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddSpliceClosure", response.results);
        }
        private void BindSpilceClosureDropdown(SCMaster objSCMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.SpliceClosure.ToString());
            //objSCMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objSCMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objSCMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        #endregion

        #region FMS
        public FMSMaster GetFMSDetail(int pSystemId, string pEntityType, string networkIdType, int systemId, string geom = "")
        {
            FMSMaster objFMS = new FMSMaster();
            objFMS.geom = geom;
            objFMS.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objFMS, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objFMS, new NetworkCodeIn() { eType = EntityType.FMS.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFMS.geom, parent_sysId = pSystemId, parent_eType = pEntityType }, networkIdType);
                objFMS.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objFMS.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objFMS.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<FMSTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.FMS);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objFMS);
            }
            else
            {
                // Get entity detail by Id...
                objFMS = new BLMisc().GetEntityDetailById<FMSMaster>(systemId, EntityType.FMS);

            }
            return objFMS;
        }

        public PartialViewResult AddFMS(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "", string siteIdSiteName = "")
        {
            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    // get geom by parent id...
            //    geom = GetPointTypeParentGeom(pSystemId, pEntityType);
            //}
            //FMSMaster objFMSMaster = GetFMSDetail(pSystemId, pEntityType, networkIdType, systemId, geom);

            //BLItemTemplate.Instance.BindItemDropdowns(objFMSMaster, EntityType.FMS.ToString());
            //BindFMSDropDown(objFMSMaster);
            //fillProjectSpecifications(objFMSMaster);
            //new MiscHelper().BindPortDetails(objFMSMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
            //objFMSMaster.pNetworkId = pNetworkId;
            //return PartialView("_AddFMS", objFMSMaster);
            FMSMaster obj = new FMSMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            obj.SiteIdSiteName = siteIdSiteName;  
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FMSMaster>(url, obj, EntityType.FMS.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddFMS", response.results);
        }
        public ActionResult SaveFMS(FMSMaster objFMSMaster, bool isDirectSave = false)
        {
            //int pSystemId = objFMSMaster.pSystemId;
            //string pEntitytype = objFMSMaster.pEntityType;
            //string pNetworkId = objFMSMaster.pNetworkId;
            //// get parent geometry 
            //if (string.IsNullOrWhiteSpace(objFMSMaster.geom) && objFMSMaster.system_id == 0)
            //{
            //    objFMSMaster.geom = GetPointTypeParentGeom(objFMSMaster.pSystemId, objFMSMaster.pEntityType);
            //}

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objFMSMaster.networkIdType == NetworkIdType.A.ToString() && objFMSMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.FMS.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFMSMaster.geom, parent_sysId = pSystemId, parent_eType = pEntitytype });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objFMSMaster = GetFMSDetail(objFMSMaster.pSystemId, objFMSMaster.pEntityType, objFMSMaster.networkIdType, objFMSMaster.system_id, objFMSMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objFMSMaster.fms_name = objNetworkCodeDetail.network_code;
            //        objFMSMaster.pSystemId = pSystemId;
            //        objFMSMaster.pEntityType = pEntitytype;
            //        objFMSMaster.pNetworkId = pNetworkId;
            //    }
            //    //SET NETWORK CODE
            //    objFMSMaster.network_id = objNetworkCodeDetail.network_code;
            //    objFMSMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (objFMSMaster.unitValue != null && objFMSMaster.unitValue.Contains(":"))
            //{
            //    objFMSMaster.no_of_input_port = Convert.ToInt32(objFMSMaster.unitValue.Split(':')[0]);
            //    objFMSMaster.no_of_output_port = Convert.ToInt32(objFMSMaster.unitValue.Split(':')[1]);
            //}
            //if (TryValidateModel(objFMSMaster))
            //{
            //    var isNew = objFMSMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLFMS().SaveEntityFMS(objFMSMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.FMS.ToString() };

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            if (resultItem.isPortConnected == true)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            }
            //        }
            //        objFMSMaster.objPM = objPM;
            //    }
            //}
            //else
            //{

            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objFMSMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objFMSMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objFMSMaster, EntityType.FMS.ToString());
            //    BindFMSDropDown(objFMSMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objFMSMaster);
            //    new MiscHelper().BindPortDetails(objFMSMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
            //    return PartialView("_AddFMS", objFMSMaster);
            //}
            objFMSMaster.isDirectSave = isDirectSave;
            objFMSMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objFMSMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objFMSMaster.source_ref_type = "NETWORK_TICKET";
                objFMSMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FMSMaster>(url, objFMSMaster, EntityType.FMS.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_AddFMS", response.results);
        }
        private void BindFMSDropDown(FMSMaster objFMSMaster)
        {
            objFMSMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objFMSMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        #endregion

        #region ONT

        public PartialViewResult AddONT(string pEntityType, string networkIdType, string geom = "", int systemId = 0, int pSystemId = 0)
        {
            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    // get geom by parent id...
            //    geom = GetPointTypeParentGeom(pSystemId, pEntityType);
            //}
            //ONTMaster objONTMaster = GetONTDetail(pSystemId, pEntityType, networkIdType, systemId, geom);
            //objONTMaster.pSystemId = pSystemId;
            //objONTMaster.pEntityType = pEntityType;
            //BLItemTemplate.Instance.BindItemDropdowns(objONTMaster, EntityType.ONT.ToString());

            //fillProjectSpecifications(objONTMaster);
            ////if (objONTMaster.pSystemId != 0 && objONTMaster.pEntityType != null && objONTMaster.pEntityType.ToLower() == EntityType.Structure.ToString().ToLower())
            ////if (objONTMaster.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
            ////{
            ////    objONTMaster.objIspEntityMap.structure_id = objONTMaster.parent_system_id;
            ////    objONTMaster.objIspEntityMap.AssociateStructure = objONTMaster.parent_system_id;
            ////}
            //BindONTDropDown(objONTMaster);
            //new MiscHelper().BindPortDetails(objONTMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
            //return PartialView("_AddONT", objONTMaster);
            ONTMaster obj = new ONTMaster();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ONTMaster>(url, obj, EntityType.ONT.ToString(), EntityAction.Get.ToString());
            response.results.lstCpeType = new BLMisc().GetDropDownList("", "CPE TYPE");
            BLLayer objBLLayer = new BLLayer();
            obj.lstUserModule = objBLLayer.GetUserModuleAbbrList(obj.user_id, UserType.Web.ToString());
            return PartialView("_AddONT", response.results);
        }
        public ONTMaster GetONTDetail(int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
        {

            ONTMaster objONT = new ONTMaster();
            objONT.geom = geom;
            objONT.networkIdType = networkIdType;
            if (objONT.objIspEntityMap.structure_id != 0)
            {
                objONT.parent_system_id = Convert.ToInt32(objONT.objIspEntityMap.structure_id);
                objONT.parent_entity_type = EntityType.Structure.ToString();
            }
            else
            {
                objONT.parent_system_id = 0;
                objONT.parent_entity_type = "Province";
            }
            if (systemId == 0)
            {
                objONT.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objONT.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objONT.ownership_type = "Own";
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objONT, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objONT, new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objONT.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType);
                //Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<ONTTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.ONT);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objONT);
            }
            else
            {
                objONT = new BLMisc().GetEntityDetailById<ONTMaster>(systemId, EntityType.ONT);
            }
            return objONT;
        }

        private void BindONTDropDown(ONTMaster objONT)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objONT.parent_system_id, objONT.system_id, EntityType.ONT.ToString());
            if (ispEntityMap != null)
            {
                objONT.objIspEntityMap.id = ispEntityMap.id;
                objONT.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objONT.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objONT.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objONT.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }
            objONT.objIspEntityMap.AssoType = objONT.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objONT.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objONT.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objONT.longitude + " " + objONT.latitude);
            if (objONT.objIspEntityMap.structure_id > 0)
            {
                var objDDL = new BLBDB().GetShaftFloorByStrucId(objONT.objIspEntityMap.structure_id);
                objONT.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
                objONT.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

                if (objONT.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objONT.objIspEntityMap.unitId = objONT.parent_system_id;
                    //objONT.objIspEntityMap.AssoType = "";
                    //objONT.objIspEntityMap.floor_id = 0;
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.ONT.ToString()).FirstOrDefault() != null)
            {
                objONT.objIspEntityMap.isValidParent = true;
                objONT.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objONT.objIspEntityMap.structure_id, objONT.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.ONT.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objONT.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objONT.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            if (objONT.objIspEntityMap.entity_type == null) { objONT.objIspEntityMap.entity_type = EntityType.ONT.ToString(); }
            objONT.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objONT.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }


        public ActionResult SaveONT(ONTMaster objONTMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();

            //// get parent geometry 
            //if (string.IsNullOrWhiteSpace(objONTMaster.geom) && objONTMaster.system_id == 0)
            //{
            //    objONTMaster.geom = GetPointTypeParentGeom(objONTMaster.pSystemId, objONTMaster.pEntityType);
            //}

            //if (objONTMaster.networkIdType == NetworkIdType.A.ToString() && objONTMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objONTMaster.geom, parent_eType = objONTMaster.pEntityType, parent_sysId = objONTMaster.pSystemId });
            //    if (isDirectSave == true)
            //    {
            //        if (objONTMaster.pEntityType != null && objONTMaster.pEntityType.ToLower() == EntityType.Structure.ToString().ToLower())
            //        {
            //            objONTMaster.objIspEntityMap.structure_id = objONTMaster.pSystemId;
            //        }
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objONTMaster = GetONTDetail(objONTMaster.pSystemId, objONTMaster.pEntityType, objONTMaster.networkIdType, objONTMaster.system_id, objONTMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objONTMaster.ont_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objONTMaster.network_id = objNetworkCodeDetail.network_code;
            //    objONTMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}

            //if (TryValidateModel(objONTMaster))
            //{
            //    // var portinfo = new BLPortInfo().ChkPortEXist(objONTMaster.model);
            //    // if (portinfo)
            //    // {
            //    objONTMaster.objIspEntityMap.structure_id = Convert.ToInt32(objONTMaster.objIspEntityMap.AssociateStructure);
            //    objONTMaster.objIspEntityMap.shaft_id = objONTMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objONTMaster.objIspEntityMap.shaft_id;
            //    if (string.IsNullOrEmpty(objONTMaster.objIspEntityMap.AssoType))
            //    {
            //        objONTMaster.objIspEntityMap.shaft_id = 0; objONTMaster.objIspEntityMap.floor_id = 0;
            //    }
            //    if (objONTMaster.unitValue != null && objONTMaster.unitValue.Contains(":"))
            //    {
            //        objONTMaster.no_of_input_port = Convert.ToInt32(objONTMaster.unitValue.Split(':')[0]);
            //        objONTMaster.no_of_output_port = Convert.ToInt32(objONTMaster.unitValue.Split(':')[1]);
            //    }
            //    if (objONTMaster.objIspEntityMap.structure_id != 0)
            //    {
            //        var structureDetails = new BLISP().GetStructureById(objONTMaster.objIspEntityMap.structure_id);
            //        if (structureDetails != null && structureDetails.Count > 0)
            //        {
            //            objONTMaster.latitude = Convert.ToDouble(structureDetails.First().latitude);
            //            objONTMaster.longitude = Convert.ToDouble(structureDetails.First().longitude);
            //        }
            //    }
            //    if (objONTMaster.objIspEntityMap.structure_id == 0)
            //    {
            //        var objIn = new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objONTMaster.longitude + " " + objONTMaster.latitude };
            //        var parentDetails = new BLMisc().getParentInfo(objIn);
            //        if (parentDetails != null)
            //        {
            //            objONTMaster.parent_system_id = parentDetails.p_system_id;
            //            objONTMaster.parent_network_id = parentDetails.p_network_id;
            //            objONTMaster.parent_entity_type = parentDetails.p_entity_type;
            //        }
            //    }
            //    var isNew = objONTMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLONT().SaveONTEntity(objONTMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {


            //        string[] LayerName = { EntityType.ONT.ToString() };
            //        //Save Reference
            //        if (objONTMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objONTMaster.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            if (resultItem.isPortConnected)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            }
            //        }
            //        objONTMaster.objPM = objPM;
            //    }
            //    // }
            //    //else
            //    //{
            //    //    objPM.status = ResponseStatus.FAILED.ToString();
            //    //     objPM.message = "No Port information found for selected model. Either change the model settings or first add port information to save ONT details !";
            //    // }
            //}
            //else
            //{

            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objONTMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objONTMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objONTMaster, EntityType.ONT.ToString());
            //    BindONTDropDown(objONTMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objONTMaster);
            //    new MiscHelper().BindPortDetails(objONTMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
            //    return PartialView("_AddONT", objONTMaster);
            //}
            objONTMaster.isDirectSave = isDirectSave;
            objONTMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objONTMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objONTMaster.source_ref_type = "NETWORK_TICKET";
                objONTMaster.status = "D";
            }
            //objONTMaster.cpe_type =  string.IsNullOrEmpty(objONTMaster.cpe_type)? "ONT" : objONTMaster.cpe_type;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ONTMaster>(url, objONTMaster, EntityType.ONT.ToString(), EntityAction.Save.ToString());
            response.results.lstCpeType = new BLMisc().GetDropDownList("", "CPE TYPE");
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            BLLayer objBLLayer = new BLLayer();
            objONTMaster.lstUserModule = objBLLayer.GetUserModuleAbbrList(objONTMaster.user_id, UserType.Web.ToString());
            return PartialView("_AddONT", response.results);
        }

        public PartialViewResult GetONTPortInfo(int systemId, int modelId)
        {
            List<IspPortInfo> objPortInfo = new List<IspPortInfo>();

            objPortInfo = new BLMisc().GetPortInfo(Convert.ToInt32(systemId), EntityType.ONT.ToString());

            return PartialView("_EntityPortInfo", objPortInfo);
        }
        #endregion

        #region Customer

        public PartialViewResult AddCustomer(string pEntityType, string networkIdType, string geom = "", int systemId = 0, int pSystemId = 0, string pNetworkId = "")
        {
            if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            {
                // get geom by parent id...
                geom = GetPointTypeParentGeom(pSystemId, pEntityType);
            }
            Customer objCustomer = GetCustomerDetail(pSystemId, pEntityType, networkIdType, systemId, geom);
            if (systemId == 0)
            {
                objCustomer.pSystemId = pSystemId;
                objCustomer.pEntityType = pEntityType;
                objCustomer.pNetworkId = pNetworkId;
                objCustomer.parent_system_id = pSystemId;
                objCustomer.parent_entity_type = pEntityType;
                objCustomer.parent_network_id = pNetworkId;
            }


            //if (objCustomer.parent_system_id != 0 && objCustomer.parent_entity_type != null && objCustomer.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
            //{
            //    objCustomer.objIspEntityMap.structure_id = objCustomer.parent_system_id;
            //    objCustomer.objIspEntityMap.AssociateStructure = objCustomer.parent_system_id;
            //}

            BindCustomerDropDown(objCustomer);
            fillProjectSpecifications(objCustomer);
            //Get the layer details to bind additional attributes Customer
            var layerdetails = new BLLayer().getLayer(EntityType.Customer.ToString());
            objCustomer.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
            //End for additional attributes Customer
            if (systemId > 0 && !string.IsNullOrEmpty(objCustomer.lmc_type))
            {
                objCustomer.lstSiteCustomerAttachment = new BLAttachment().getAttachmentDetails(systemId, EntityType.Customer.ToString(), "Document", "SiteCustomer");// new BLSiteCustomerAttachment().getSiteCustomerAttachment(customerDetails.system_id, "Customer", "Document");
                foreach (var item in objCustomer.lstSiteCustomerAttachment)
                {
                    item.file_size_converted = BytesToString(item.file_size);

                }
                var objSiteInfo = new BLSiteInfo().getSitebyId(objCustomer.site_id);
                objCustomer.lstFloorInfo = BLFloor.Instance.GetFloorByBld(objSiteInfo.parent_system_id);
                objCustomer.lstElectricalmeter = new BLMisc().GetDropDownList("", DropDownType.Electrical_Meter_Type.ToString());
                objCustomer.lstCableEntryPoints = new BLMisc().GetDropDownList("", DropDownType.Cable_Entry_Point.ToString());
                var CustomerDetails = BLIspEntityMapping.Instance.GetIspEntityMapByCustomerId(objCustomer.system_id, EntityType.Customer.ToString());
                var floor = BLISP.Instance.getFloorInfo(CustomerDetails.floor_id);
                objCustomer.Floor_Name = floor.floor_name;
                objCustomer.floor_id = floor.system_id;
                objCustomer.childModel = "";
                return PartialView("_AddSiteCustomer", objCustomer);
            }
            return PartialView("_AddCustomer", objCustomer);

        }
        public Customer GetCustomerDetail(int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
        {

            Customer objCustomer = new Customer();
            var userdetails = (User)Session["userDetail"];
            objCustomer.geom = geom;
            objCustomer.networkIdType = networkIdType;
            if (systemId == 0)
            {
                objCustomer.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objCustomer.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objCustomer, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...  Temporary Solution:== Pass the parent_sysId=0 and parent_eType=""  to sove the customer network id duplicacy.            
                fillParentDetail(objCustomer, new NetworkCodeIn() { eType = EntityType.Customer.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCustomer.geom, parent_eType = "", parent_sysId = 0 }, networkIdType);
                objCustomer.other_info = null;  //for additional-attributes
            }
            else
            {
                objCustomer = new BLMisc().GetEntityDetailById<Customer>(systemId, EntityType.Customer);
                //for additional-attributes
                objCustomer.other_info = new BLCustomer().GetOtherInfoCustomer(objCustomer.system_id);
                fillRegionProvAbbr(objCustomer);
            }
            objCustomer.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objCustomer;
        }
        public ActionResult SaveCustomer(Customer objCustomer, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            int pSystemId = objCustomer.pSystemId;
            string pEntitytype = objCustomer.pEntityType;
            string pNetworkId = objCustomer.pNetworkId;
            objCustomer.service_type = objCustomer.SelectedServiceType != null && objCustomer.SelectedServiceType.Count > 0 ? string.Join(",", objCustomer.SelectedServiceType.ToArray()) : "";
            // get parent geometry 
            if (string.IsNullOrWhiteSpace(objCustomer.geom) && objCustomer.system_id == 0)
            {
                objCustomer.geom = GetPointTypeParentGeom(objCustomer.pSystemId, objCustomer.pEntityType);
            }

            if (objCustomer.networkIdType == NetworkIdType.A.ToString() && objCustomer.system_id == 0)
            {
                //GET AUTO NETWORK CODE...Temporary Solution:== Pass the parent_sysId=0 and parent_eType=""  to sove the customer network id duplicacy.
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Customer.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCustomer.geom, parent_eType = "", parent_sysId = 0 });
                if (isDirectSave == true)
                {
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    objCustomer = GetCustomerDetail(objCustomer.pSystemId, objCustomer.pEntityType, objCustomer.networkIdType, objCustomer.system_id, objCustomer.geom);
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    objCustomer.customer_name = objNetworkCodeDetail.network_code;
                    objCustomer.parent_system_id = pSystemId;
                    objCustomer.parent_entity_type = pEntitytype;
                    objCustomer.parent_network_id = pNetworkId;
                }
                //SET NETWORK CODE
                objCustomer.network_id = objNetworkCodeDetail.network_code;
                objCustomer.sequence_id = objNetworkCodeDetail.sequence_id;
            }
            if (string.IsNullOrEmpty(objCustomer.customer_name))
            {
                objCustomer.customer_name = objCustomer.network_id;
            }
            if (TryValidateModel(objCustomer))
            {
                //if (objCustomer.objIspEntityMap.AssociateStructure != 0)
                //{
                //    objCustomer.objIspEntityMap.structure_id = Convert.ToInt32(objCustomer.objIspEntityMap.AssociateStructure);
                //}
                objCustomer.objIspEntityMap.structure_id = Convert.ToInt32(objCustomer.objIspEntityMap.AssociateStructure);
                //objCustomer.objIspEntityMap.structure_id = objCustomer.AssociateStructure != 0 ? objCustomer.AssociateStructure : objCustomer.objIspEntityMap.structure_id;
                objCustomer.objIspEntityMap.shaft_id = objCustomer.objIspEntityMap.AssoType == "Floor" ? 0 : objCustomer.objIspEntityMap.shaft_id;
                if (string.IsNullOrEmpty(objCustomer.objIspEntityMap.AssoType))
                {
                    objCustomer.objIspEntityMap.shaft_id = 0; objCustomer.objIspEntityMap.floor_id = 0;
                }
                if (objCustomer.objIspEntityMap.structure_id == 0 && objCustomer.system_id > 0)
                {

                    var parentDetails = new BLMisc().getParentInfo(new NetworkCodeIn() { eType = EntityType.Customer.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCustomer.longitude + " " + objCustomer.latitude, parent_eType = "", parent_sysId = 0 });
                    if (parentDetails != null)
                    {
                        objCustomer.parent_system_id = parentDetails.p_system_id;
                        objCustomer.parent_network_id = parentDetails.p_network_id;
                        objCustomer.parent_entity_type = parentDetails.p_entity_type;
                    }
                }
                var isNew = objCustomer.system_id > 0 ? false : true;
                objCustomer.is_new_entity = (isNew && objCustomer.source_ref_id != "0" && objCustomer.source_ref_id != "");
                var NWTicketDetails = new NetworkTicket();
                if (Session["NWTicketDetails"] != null)
                {
                    NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                    objCustomer.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                    objCustomer.source_ref_type = "NETWORK_TICKET";
                    objCustomer.status = "D";
                }
                var resultItem = new BLCustomer().SaveCustomer(objCustomer, Convert.ToInt32(Session["user_id"]));
                if (Session["NWTicketDetails"] != null)
                {
                    DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                    NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                    Session["NWTicketDetails"] = NWTicketDetails;

                }
                if (string.IsNullOrEmpty(resultItem.objPM.message))
                {



                    if (isNew)
                    {
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.isNewEntity = isNew;
                        objPM.message = Resources.Resources.SI_OSP_CUS_NET_FRM_005;
                    }
                    else
                    {
                        if (resultItem.isPortConnected)
                        {
                            objPM.status = ResponseStatus.OK.ToString();
                            objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
                        }
                        else
                        {
                            objPM.status = ResponseStatus.OK.ToString();
                            objPM.message = Resources.Resources.SI_OSP_CUS_NET_FRM_006;
                        }

                    }
                    objCustomer.objPM = objPM;
                }
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = getFirstErrorFromModelState();
                objCustomer.objPM = objPM;
            }
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(objCustomer.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {
                BindCustomerDropDown(objCustomer);
                fillProjectSpecifications(objCustomer);
                //Get the layer details to bind additional attributes Customer
                var layerdetails = new BLLayer().getLayer(EntityType.Customer.ToString());
                objCustomer.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
                //End for additional attributes Customer
                // RETURN PARTIAL VIEW WITH MODEL DATA
                return PartialView("_AddCustomer", objCustomer);
            }
        }
        private void BindCustomerDropDown(Customer objCustomer)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Customer.ToString());
            objCustomer.lstActivationStatus = objDDL.Where(x => x.dropdown_type == DropDownType.Activation_Status.ToString()).ToList();
            objCustomer.lstCustomerType = objDDL.Where(x => x.dropdown_type == DropDownType.Customer_Type.ToString()).ToList();
            objCustomer.lstServiceType = objDDL.Where(x => x.dropdown_type.ToUpper() == DropDownType.Customer_Service_Type.ToString().ToUpper()).ToList();
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objCustomer.parent_system_id, objCustomer.system_id, EntityType.Customer.ToString());
            if (ispEntityMap != null)
            {
                objCustomer.objIspEntityMap.id = ispEntityMap.id;
                objCustomer.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objCustomer.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objCustomer.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objCustomer.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }
            objCustomer.objIspEntityMap.AssoType = objCustomer.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objCustomer.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objCustomer.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objCustomer.longitude + " " + objCustomer.latitude);
            if (objCustomer.objIspEntityMap.structure_id > 0)
            {
                var objShaftNFloor = new BLBDB().GetShaftFloorByStrucId(objCustomer.objIspEntityMap.structure_id);
                objCustomer.objIspEntityMap.lstShaft = objShaftNFloor.Where(m => m.isshaft == true).ToList();
                objCustomer.objIspEntityMap.lstFloor = objShaftNFloor.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();
                if (objCustomer.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objCustomer.objIspEntityMap.unitId = objCustomer.parent_system_id;
                    //objCustomer.objIspEntityMap.AssoType = "";
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.Customer.ToString()).FirstOrDefault() != null)
            {
                objCustomer.objIspEntityMap.isValidParent = true;
                objCustomer.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objCustomer.objIspEntityMap.structure_id, objCustomer.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.Customer.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objCustomer.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objCustomer.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            if (objCustomer.objIspEntityMap.entity_type == null) { objCustomer.objIspEntityMap.entity_type = EntityType.Customer.ToString(); }
            var ispEntityMapDetail = BLIspEntityMapping.Instance.GetStructureFloorbyEntityId(objCustomer.parent_system_id, objCustomer.parent_entity_type);
            if (ispEntityMapDetail != null)
            {
                objCustomer.objIspEntityMap.floor_id = ispEntityMapDetail.floor_id;
                objCustomer.objIspEntityMap.shaft_id = ispEntityMapDetail.shaft_id;
                objCustomer.objIspEntityMap.AssoType = objCustomer.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objCustomer.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            }

        }

        #endregion

        #region HTB
        public ActionResult AddHTB(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "")
        {
            //HTBInfo model = getHTBInfo(networkIdType, ModelInfo.templateId, systemId);
            //if (systemId != 0)
            //{
            //    var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.HTB.ToString());
            //    //model.objIspEntityMap.floor_id = ispMapping.floor_id;
            //    //model.objIspEntityMap.structure_id = ispMapping.structure_id;
            //    // model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
            //}
            //else
            //{
            //    model.objIspEntityMap.floor_id = ModelInfo.floorid;
            //    model.objIspEntityMap.structure_id = ModelInfo.structureid;
            //    model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            //}
            //BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
            //BindHTBDropDown(model);
            //new MiscHelper().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //fillProjectSpecifications(model);
            //return PartialView("_AddHTB", model);
            HTBInfo obj = new HTBInfo();
            obj.networkIdType = networkIdType;
            obj.geom = geom;
            obj.system_id = systemId;
            obj.elementType = ModelInfo.elementType;
            obj.objIspEntityMap.structure_id = ModelInfo.structureid;
            obj.objIspEntityMap.template_id = ModelInfo.templateId;
            obj.objIspEntityMap.floor_id = ModelInfo.floorid;
            obj.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            obj.objIspEntityMap.operation = ModelInfo.operation;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HTBInfo>(url, obj, EntityType.HTB.ToString(), EntityAction.Get.ToString(), obj.objIspEntityMap.structure_id.ToString());
            response.results.is_middlewareInLayer = new BLMisc().chkEntityIsMiddleWare(EntityType.HTB.ToString());
            return PartialView("_AddHTB", response.results);

        }
        //private void BindHTBDropDown(HTBInfo objBDB)
        //{
        //    var objDDL = new BLBDB().GetShaftFloorByStrucId(objBDB.parent_system_id);
        //}
        public HTBInfo getHTBInfo(string networkIdType, int templateId, int systemId = 0, string geom = "")
        {
            HTBInfo objHTB = new HTBInfo();
            objHTB.geom = geom;
            if (systemId != 0)
            {
                // objHTB = BLISP.Instance.getHTBDetails(systemId);
                objHTB = new BLMisc().GetEntityDetailById<HTBInfo>(systemId, EntityType.HTB);
            }
            else
            {
                //if (networkIdType == NetworkIdType.M.ToString())
                //{
                objHTB.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objHTB.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objHTB.ownership_type = "Own";
                fillRegionProvinceDetail(objHTB, GeometryType.Point.ToString(), objHTB.geom);
                fillParentDetail(objHTB, new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHTB.geom, parent_eType = "", parent_sysId = 0 }, networkIdType);
                // for Manual network id type 

                var objItem = BLItemTemplate.Instance.GetTemplateDetail<HTBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.HTB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objHTB);
                //var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHTB.geom, parent_eType = "", parent_sysId = 0 }, networkIdType);
                //   objHTB.network_id = ISPNetworkCodeDetail.network_code;  

            }
            return objHTB;
        }

        private void BindHTBDropDown(HTBInfo objHTB)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objHTB.parent_system_id, objHTB.system_id, EntityType.HTB.ToString());
            if (ispEntityMap != null)
            {
                objHTB.objIspEntityMap.id = ispEntityMap.id;
                objHTB.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objHTB.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objHTB.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objHTB.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }
            objHTB.objIspEntityMap.AssoType = objHTB.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objHTB.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objHTB.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objHTB.longitude + " " + objHTB.latitude);
            if (objHTB.objIspEntityMap.structure_id > 0)
            {
                var objDDL = new BLBDB().GetShaftFloorByStrucId(objHTB.objIspEntityMap.structure_id);
                objHTB.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
                objHTB.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

                if (objHTB.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objHTB.objIspEntityMap.unitId = objHTB.parent_system_id;
                    //objONT.objIspEntityMap.AssoType = "";
                    //objONT.objIspEntityMap.floor_id = 0;
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.HTB.ToString()).FirstOrDefault() != null)
            {
                objHTB.objIspEntityMap.isValidParent = true;
                objHTB.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objHTB.objIspEntityMap.structure_id, objHTB.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.HTB.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objHTB.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objHTB.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            if (objHTB.objIspEntityMap.entity_type == null) { objHTB.objIspEntityMap.entity_type = EntityType.HTB.ToString(); }
            //objHTB.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objHTB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objHTB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        public ActionResult SaveHTB(HTBInfo objHTB, string networkIdType, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //// int structure_id = model.objIspEntityMap.structure_id;
            //// int? floor_id = model.objIspEntityMap.floor_id;
            ////int? shaft_id = model.objIspEntityMap.shaft_id;
            //model.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            //if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE... new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDB.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType)
            //    //var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = "", parent_sysId = 0 });
            //    var objISPNetworkCode = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = "", parent_sysId = 0 });
            //    model.longitude = Convert.ToDouble(model.geom.Split(' ')[0]);
            //    model.latitude = Convert.ToDouble(model.geom.Split(' ')[1]);
            //    //NEW ENTITY->Fill Region and Province Detail..
            //    // fillRegionProvinceDetail(model, GeometryType.Point.ToString(), model.geom);

            //    // fillParentDetail(model, new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = "", parent_sysId = 0 }, networkIdType);


            //    if (isDirectSave == true)
            //    {
            //        //objONTMaster.pSystemId, objONTMaster.pEntityType, objONTMaster.networkIdType, objONTMaster.system_id, objONTMaster.geom
            //        model = getHTBInfo(model.networkIdType, model.templateId, model.system_id, model.geom);
            //        // model.objIspEntityMap.floor_id = floor_id;
            //        // model.objIspEntityMap.structure_id = structure_id;
            //        // model.objIspEntityMap.shaft_id = shaft_id;
            //        model.htb_name = objISPNetworkCode.network_code;
            //    }
            //    model.network_id = objISPNetworkCode.network_code;
            //    model.sequence_id = objISPNetworkCode.sequence_id;

            //}

            ////var structureDetails = new BLISP().GetStructureById(structure_id);
            ////if (structureDetails != null)
            ////{
            ////    model.region_id = structureDetails.First().region_id;
            ////    model.province_id = structureDetails.First().province_id;
            ////    model.latitude = structureDetails.First().latitude;
            ////    model.longitude = structureDetails.First().longitude;
            ////}
            //if (TryValidateModel(model))
            //{
            //    model.objIspEntityMap.structure_id = Convert.ToInt32(model.objIspEntityMap.AssociateStructure);
            //    model.objIspEntityMap.shaft_id = model.objIspEntityMap.AssoType == "Floor" ? 0 : model.objIspEntityMap.shaft_id;
            //    if (string.IsNullOrEmpty(model.objIspEntityMap.AssoType))
            //    {
            //        model.objIspEntityMap.shaft_id = 0; model.objIspEntityMap.floor_id = 0;
            //    }
            //    bool isNew = model.system_id == 0 ? true : false;
            //    if (model.unitValue != null && model.unitValue.Contains(":"))
            //    {
            //        model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
            //        model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
            //    }
            //    if (model.objIspEntityMap.structure_id == 0)
            //    {
            //        var objIn = new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude };
            //        var parentDetails = new BLMisc().getParentInfo(objIn);
            //        if (parentDetails != null)
            //        {
            //            model.parent_system_id = parentDetails.p_system_id;
            //            model.parent_network_id = parentDetails.p_network_id;
            //            model.parent_entity_type = parentDetails.p_entity_type;
            //        }
            //    }
            //    var result = new BLISP().SaveHTBDetails(model, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(result.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.HTB.ToString() };
            //        if (model.EntityReference != null && result.system_id > 0)
            //        {
            //            SaveReference(model.EntityReference, result.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString(); ;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            if (result.isPortConnected == true)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);//result.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            }
            //        }
            //        model.objPM = objPM;
            //    }

            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    model.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(model.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
            //    BindHTBDropDown(model);
            //    new MiscHelper().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //    fillProjectSpecifications(model);
            //    model.entityType = EntityType.HTB.ToString();
            //    return PartialView("_AddHTB", model);
            //}
            objHTB.isDirectSave = isDirectSave;
            objHTB.networkIdType = networkIdType;
            objHTB.user_id = Convert.ToInt32(Session["user_id"]);

            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HTBInfo>(url, objHTB, EntityType.HTB.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddHTB", response.results);

        }
        public JsonResult DeleteHTBById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            bool result = false;
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.HTB.ToString());
            //if (isNotAssociated == true) { result = BLISP.Instance.DeleteHTBById(systemId); }
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.HTB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);//response.message;
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);//response.message;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region OpticalRepeater
        public ActionResult AddOpticalRepeater(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            OpticalRepeaterInfo obj = new OpticalRepeaterInfo();
            obj.networkIdType = networkIdType;
            obj.geom = geom;
            obj.system_id = systemId;
            obj.elementType = ModelInfo.elementType;
            obj.objIspEntityMap.structure_id = ModelInfo.structureid;
            obj.objIspEntityMap.template_id = ModelInfo.templateId;
            obj.objIspEntityMap.floor_id = ModelInfo.floorid;
            obj.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            obj.objIspEntityMap.operation = ModelInfo.operation;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            if (systemId == 0)
            {
                obj.amplifier_wavelength = 1530;
                obj.signal_boost_value = 12;
            }
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<OpticalRepeaterInfo>(url, obj, EntityType.OpticalRepeater.ToString(), EntityAction.Get.ToString(), obj.objIspEntityMap.structure_id.ToString());
            return PartialView("_AddOpticalRepeater", response.results);

        }
        public OpticalRepeaterInfo getOpticalRepeaterInfo(string networkIdType, int templateId, int systemId = 0, string geom = "")
        {
            OpticalRepeaterInfo objOpticalRepeater = new OpticalRepeaterInfo();
            objOpticalRepeater.geom = geom;
            if (systemId != 0)
            {
                objOpticalRepeater = new BLMisc().GetEntityDetailById<OpticalRepeaterInfo>(systemId, EntityType.OpticalRepeater);
            }
            else
            {
                objOpticalRepeater.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objOpticalRepeater.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objOpticalRepeater.ownership_type = "Own";
                fillRegionProvinceDetail(objOpticalRepeater, GeometryType.Point.ToString(), objOpticalRepeater.geom);
                fillParentDetail(objOpticalRepeater, new NetworkCodeIn() { eType = EntityType.OpticalRepeater.ToString(), gType = GeometryType.Point.ToString(), eGeom = objOpticalRepeater.geom, parent_eType = "", parent_sysId = 0 }, networkIdType);
                // for Manual network id type 
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<OpticalRepeaterTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.OpticalRepeater);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objOpticalRepeater);

            }
            return objOpticalRepeater;
        }

        private void BindOpticalRepeaterDropDown(OpticalRepeaterInfo objOpticalRepeater)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objOpticalRepeater.parent_system_id, objOpticalRepeater.system_id, EntityType.OpticalRepeater.ToString());
            if (ispEntityMap != null)
            {
                objOpticalRepeater.objIspEntityMap.id = ispEntityMap.id;
                objOpticalRepeater.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objOpticalRepeater.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objOpticalRepeater.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objOpticalRepeater.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }
            objOpticalRepeater.objIspEntityMap.AssoType = objOpticalRepeater.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objOpticalRepeater.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objOpticalRepeater.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objOpticalRepeater.longitude + " " + objOpticalRepeater.latitude);
            if (objOpticalRepeater.objIspEntityMap.structure_id > 0)
            {
                var objDDL = new BLBDB().GetShaftFloorByStrucId(objOpticalRepeater.objIspEntityMap.structure_id);
                objOpticalRepeater.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
                objOpticalRepeater.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

                if (objOpticalRepeater.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objOpticalRepeater.objIspEntityMap.unitId = objOpticalRepeater.parent_system_id;
                    //objONT.objIspEntityMap.AssoType = "";
                    //objONT.objIspEntityMap.floor_id = 0;
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.OpticalRepeater.ToString()).FirstOrDefault() != null)
            {
                objOpticalRepeater.objIspEntityMap.isValidParent = true;
                objOpticalRepeater.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objOpticalRepeater.objIspEntityMap.structure_id, objOpticalRepeater.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.OpticalRepeater.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objOpticalRepeater.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objOpticalRepeater.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            if (objOpticalRepeater.objIspEntityMap.entity_type == null) { objOpticalRepeater.objIspEntityMap.entity_type = EntityType.OpticalRepeater.ToString(); }
            //objHTB.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objOpticalRepeater.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objOpticalRepeater.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        public ActionResult SaveOpticalRepeater(OpticalRepeaterInfo model, string networkIdType, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //// int structure_id = model.objIspEntityMap.structure_id;
            //// int? floor_id = model.objIspEntityMap.floor_id;
            ////int? shaft_id = model.objIspEntityMap.shaft_id;
            //model.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            //if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE... new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDB.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType)
            //    //var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = "", parent_sysId = 0 });
            //    var objISPNetworkCode = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = "", parent_sysId = 0 });
            //    model.longitude = Convert.ToDouble(model.geom.Split(' ')[0]);
            //    model.latitude = Convert.ToDouble(model.geom.Split(' ')[1]);
            //    //NEW ENTITY->Fill Region and Province Detail..
            //    // fillRegionProvinceDetail(model, GeometryType.Point.ToString(), model.geom);

            //    // fillParentDetail(model, new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = "", parent_sysId = 0 }, networkIdType);


            //    if (isDirectSave == true)
            //    {
            //        //objONTMaster.pSystemId, objONTMaster.pEntityType, objONTMaster.networkIdType, objONTMaster.system_id, objONTMaster.geom
            //        model = getHTBInfo(model.networkIdType, model.templateId, model.system_id, model.geom);
            //        // model.objIspEntityMap.floor_id = floor_id;
            //        // model.objIspEntityMap.structure_id = structure_id;
            //        // model.objIspEntityMap.shaft_id = shaft_id;
            //        model.htb_name = objISPNetworkCode.network_code;
            //    }
            //    model.network_id = objISPNetworkCode.network_code;
            //    model.sequence_id = objISPNetworkCode.sequence_id;

            //}

            ////var structureDetails = new BLISP().GetStructureById(structure_id);
            ////if (structureDetails != null)
            ////{
            ////    model.region_id = structureDetails.First().region_id;
            ////    model.province_id = structureDetails.First().province_id;
            ////    model.latitude = structureDetails.First().latitude;
            ////    model.longitude = structureDetails.First().longitude;
            ////}
            //if (TryValidateModel(model))
            //{
            //    model.objIspEntityMap.structure_id = Convert.ToInt32(model.objIspEntityMap.AssociateStructure);
            //    model.objIspEntityMap.shaft_id = model.objIspEntityMap.AssoType == "Floor" ? 0 : model.objIspEntityMap.shaft_id;
            //    if (string.IsNullOrEmpty(model.objIspEntityMap.AssoType))
            //    {
            //        model.objIspEntityMap.shaft_id = 0; model.objIspEntityMap.floor_id = 0;
            //    }
            //    bool isNew = model.system_id == 0 ? true : false;
            //    if (model.unitValue != null && model.unitValue.Contains(":"))
            //    {
            //        model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
            //        model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
            //    }
            //    if (model.objIspEntityMap.structure_id == 0)
            //    {
            //        var objIn = new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude };
            //        var parentDetails = new BLMisc().getParentInfo(objIn);
            //        if (parentDetails != null)
            //        {
            //            model.parent_system_id = parentDetails.p_system_id;
            //            model.parent_network_id = parentDetails.p_network_id;
            //            model.parent_entity_type = parentDetails.p_entity_type;
            //        }
            //    }
            //    var result = new BLISP().SaveHTBDetails(model, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(result.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.HTB.ToString() };
            //        if (model.EntityReference != null && result.system_id > 0)
            //        {
            //            SaveReference(model.EntityReference, result.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString(); ;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            if (result.isPortConnected == true)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);//result.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            }
            //        }
            //        model.objPM = objPM;
            //    }

            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    model.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(model.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
            //    BindHTBDropDown(model);
            //    new MiscHelper().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //    fillProjectSpecifications(model);
            //    model.entityType = EntityType.HTB.ToString();
            //    return PartialView("_AddHTB", model);
            //}
            model.isDirectSave = isDirectSave;
            model.networkIdType = networkIdType;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<OpticalRepeaterInfo>(url, model, EntityType.OpticalRepeater.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddOpticalRepeater", response.results);

        }
        public JsonResult DeleteOpticalRepeaterById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            bool result = false;
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.HTB.ToString());
            //if (isNotAssociated == true) { result = BLISP.Instance.DeleteHTBById(systemId); }
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.OpticalRepeater.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);//response.message;
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);//response.message;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Cable
        private void BindCableDropDown(CableMaster objCableIn)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Cable.ToString());
            var userdetails = (User)Session["userDetail"];
            objCableIn.fiberCount = objDDL.Where(x => x.dropdown_type == DropDownType.Fiber_Count.ToString()).ToList();
            objCableIn.listcableCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Category.ToString()).ToList();
            objCableIn.listcableSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Subcategory.ToString()).ToList();
            objCableIn.listExecutionMethod = objDDL.Where(x => x.dropdown_type == DropDownType.Execution_Method.ToString()).ToList();
            objCableIn.listcableType = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Type.ToString()).ToList();
            // objCableIn.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objCableIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objCableIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }

        public CableMaster GetCableDetail(LineEntityIn objIn)
        {
            CableMaster objCbl = new CableMaster();
            if (objIn.systemId == 0)
            {
                objCbl.geom = objIn.geom;
                objCbl.cable_type = objIn.cableType;
                if (!string.IsNullOrEmpty(objIn.geom))
                    objCbl.cable_measured_length = (float)new BLMisc().GetCableLength(objIn.geom);
                var extraLength = (ApplicationSettings.CableExtraLengthPercentage * objCbl.cable_measured_length) / 100 - objCbl.total_loop_length;
                objCbl.cable_calculated_length = objCbl.cable_measured_length + objCbl.total_loop_length + extraLength;

                objCbl.networkIdType = objIn.networkIdType;
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objCbl, GeometryType.Line.ToString(), objIn.geom);

                // set default value for ownership..
                //objCbl.ownership = "Own";
                objCbl.ownership_type = "Own";

                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<CableTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Cable, objIn.cableType);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objCbl);
                objCbl.unitValue = objItem.total_core.ToString();

            }
            else
            {
                objCbl = new BLMisc().GetEntityDetailById<CableMaster>(objIn.systemId, EntityType.Cable);
                Session["CableNetworkId"] = objCbl.network_id;
            }
            return objCbl;
        }


        public PartialViewResult AddCable(LineEntityIn objIn, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //CableMaster objCbl = new CableMaster();
            //objCbl = GetCableDetail(objIn);
            //if (objIn.systemId == 0)
            //{
            //    //Fill Location detail...    
            //    GetLineNetworkDetail(objCbl, objIn, EntityType.Cable.ToString(), false);

            //}

            //BLItemTemplate.Instance.BindItemDropdowns(objCbl, EntityType.Cable.ToString());
            //BindCableDropDown(objCbl);
            //objCbl.fiberCountIn = objCbl.total_core.ToString();
            //fillProjectSpecifications(objCbl);
            //new MiscHelper().BindPortDetails(objCbl, EntityType.Cable.ToString(), DropDownType.Fiber_Count.ToString());
            //objCbl.unitValue = Convert.ToString(objCbl.total_core);
            //objCbl.pSystemId = pSystemId;
            //objCbl.pNetworkId = pNetworkId;
            //objCbl.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
            //return PartialView("_AddCable", objCbl);
            objIn.pEntityType = pEntityType;
            objIn.pNetworkId = pNetworkId;
            objIn.pSystemId = pSystemId;
            objIn.system_id = objIn.systemId;
            objIn.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CableMaster>(url, objIn, EntityType.Cable.ToString(), EntityAction.Get.ToString());
            CableMaster _obj = new CableMaster();

            BLLayer objBLLayer = new BLLayer();
            _obj.lstUserModule = objBLLayer.GetUserModuleAbbrList(objIn.user_id, UserType.Web.ToString());
            return PartialView("_AddCable", response.results);
        }

        public ActionResult SaveCable(CableMaster objCbl, string actionTab, bool isDirectSave = false)
        {

            if (actionTab == "LMCInfoTab")/*....LMC INFO Tab is not included in API section! */
            {
                objCbl.LMCCableInfo.ActionTab = actionTab;
                return SaveLMCInfo(objCbl.LMCCableInfo);
            }
            else
            {
                //    ModelState.Clear();
                //    PageMessage objPM = new PageMessage();
                //    bool isValid = true;
                //    int pSystemId = objCbl.pSystemId;
                //    string pNetworkId = objCbl.pNetworkId;
                //    if (objCbl.networkIdType == NetworkIdType.A.ToString() && objCbl.system_id == 0)
                //    {
                //        if (isDirectSave == false)
                //        {
                //            objCbl.lstTP.Add(new NetworkDtl { system_id = objCbl.a_system_id, network_id = objCbl.a_location, network_name = objCbl.a_entity_type });
                //            objCbl.lstTP.Add(new NetworkDtl { system_id = objCbl.b_system_id, network_id = objCbl.b_location, network_name = objCbl.b_entity_type });
                //        }
                //        var objLineEntity = new LineEntityIn() { geom = objCbl.geom, systemId = objCbl.system_id, cableType = objCbl.cable_type, networkIdType = objCbl.networkIdType, lstTP = objCbl.lstTP };

                //        if (isDirectSave == true)
                //        {

                //            objCbl = GetCableDetail(objLineEntity);
                //            objCbl.duct_id = pSystemId;
                //            objCbl.pNetworkId = pNetworkId;
                //        }
                //        //GET AUTO NETWORK CODE...
                //        GetLineNetworkDetail(objCbl, objLineEntity, EntityType.Cable.ToString(), true);
                //        if (isDirectSave == true)
                //            objCbl.cable_name = objCbl.network_id;
                //    }
                //    objCbl.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
                //    var startEndReading = objCbl.formInputSettings.Count > 0 ? objCbl.formInputSettings.Where(m => m.form_feature_name.ToLower() == formFeatureName.start_end_reading.ToString() && m.form_feature_type.ToLower() == formFeatureType.feature.ToString() && m.is_active == true).FirstOrDefault() : null;

                //    var multipleTubeCore = objCbl.no_of_core_per_tube * objCbl.no_of_tube;
                //    if (multipleTubeCore != Convert.ToInt32(objCbl.unitValue) || multipleTubeCore == 0)
                //    {
                //        objPM.status = ResponseStatus.FAILED.ToString();
                //        objPM.message = Resources.Resources.SI_OSP_GBL_JQ_FRM_012;
                //        isValid = false;
                //    }
                //    else if (objCbl.cable_measured_length == 0)
                //    {
                //        objPM.status = ResponseStatus.FAILED.ToString();
                //        objPM.message = Resources.Resources.SI_OSP_CAB_NET_FRM_066;
                //        isValid = false;
                //    }
                //    //if (objCbl.ownership_type.ToUpper() != "OWN" && objCbl.circuit_id == null)
                //    //{
                //    //    objPM.status = ResponseStatus.FAILED.ToString();
                //    //    objPM.message = "Circuit ID is mandatory for " + objCbl.ownership + " ownership!";
                //    //    isValid = false;
                //    //}
                //    //if (objCbl.cable_status.ToUpper() != "IN-SERVICE")
                //    //{
                //    //    var response = new BLMisc().isPortConnected(objCbl.system_id, EntityType.Cable.ToString());
                //    //    if (response.status)
                //    //    {
                //    //        objPM.status = ResponseStatus.FAILED.ToString();
                //    //        objPM.message = "Cable status can't be changed to "+ objCbl.cable_status + " as it is spliced!";
                //    //        isValid = false;
                //    //    }
                //    //}
                //    //else if ((startEndReading!=null && objCbl.cable_measured_length > objCbl.cable_calculated_length))
                //    //{
                //    //    objPM.status = ResponseStatus.FAILED.ToString();
                //    //    objPM.message = "Cable calculated length should be equal or greater than measured length !!";
                //    //    isValid = false;
                //    //}
                //    if (!(string.IsNullOrEmpty(objCbl.unitValue)))
                //    {
                //        objCbl.total_core = Convert.ToInt32(objCbl.unitValue);
                //    }

                //    if (isDirectSave)
                //    {
                //        if (startEndReading != null)
                //        {
                //            objCbl.start_reading = 0;
                //            objCbl.end_reading = 0;
                //            objCbl.cable_calculated_length = 0;
                //        }
                //    }

                //    if (TryValidateModel(objCbl) && isValid == true)
                //    {
                //        var isNew = objCbl.system_id > 0 ? false : true;
                //        objCbl.duct_id = pSystemId;
                //        var resultItem = BLCable.Instance.SaveCable(objCbl, Convert.ToInt32(Session["user_id"]));
                //        //var insertTubeCore = BLCable.Instance.SetCableColorDetails(resultItem.system_id, resultItem.no_of_tube, resultItem.no_of_core_per_tube, resultItem.created_by);

                //        if (string.IsNullOrEmpty(resultItem.objPM.message))
                //        {

                //            string[] LayerName = { EntityType.Cable.ToString() };

                //            if (objCbl.lstTubeCore != null)
                //            {
                //                BLCable.Instance.SaveTubeCoreColor(JsonConvert.SerializeObject(objCbl.lstTubeCore.objTube), JsonConvert.SerializeObject(objCbl.lstTubeCore.objCore), resultItem.system_id);
                //            }
                //            if (isNew)
                //            {
                //                objPM.status = ResponseStatus.OK.ToString();
                //                objPM.isNewEntity = isNew;
                //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                //            }
                //            else
                //            {
                //                if (resultItem.isPortConnected)
                //                {
                //                    objPM.status = ResponseStatus.OK.ToString();
                //                    objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
                //                }
                //                else
                //                {
                //                    objPM.status = ResponseStatus.OK.ToString();
                //                    objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                //                }
                //            }
                //            //Save Reference
                //            if (objCbl.EntityReference != null && resultItem.system_id > 0)
                //            {
                //                SaveReference(objCbl.EntityReference, resultItem.system_id);
                //            }
                //            //save AT Status                        
                //            if (objCbl.ATAcceptance != null && objCbl.system_id > 0)
                //            {
                //                SaveATAcceptance(objCbl.ATAcceptance, objCbl.system_id);
                //            }


                //            objCbl.objPM = objPM;
                //        }


                //    }
                //    else
                //    {
                //        objPM.status = ResponseStatus.FAILED.ToString();
                //        objPM.message = isValid == true ? getFirstErrorFromModelState() : objPM.message;
                //        objCbl.objPM = objPM;
                //    }
                //    if (isDirectSave == true)
                //    {
                //        //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                //        return Json(objCbl.objPM, JsonRequestBehavior.AllowGet);
                //    }
                //    else
                //    {

                //        BLItemTemplate.Instance.BindItemDropdowns(objCbl, EntityType.Cable.ToString());
                //        BindCableDropDown(objCbl);
                //        new MiscHelper().BindPortDetails(objCbl, EntityType.Cable.ToString(), DropDownType.Fiber_Count.ToString());
                //        objCbl.unitValue = Convert.ToString(objCbl.total_core);
                //        // RETURN PARTIAL VIEW WITH MODEL DATA

                //        fillProjectSpecifications(objCbl);
                //        return PartialView("_AddCable", objCbl);
                //    }
                //}
                objCbl.isDirectSave = isDirectSave;
                objCbl.actionTab = actionTab;
                objCbl.user_id = Convert.ToInt32(Session["user_id"]);
                if (Session["NWTicketDetails"] != null)
                {
                    var NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                    DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                    NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                    Session["NWTicketDetails"] = NWTicketDetails;
                    objCbl.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                    objCbl.source_ref_type = "NETWORK_TICKET";
                    objCbl.status = "D";
                }
                string url = "api/Library/EntityOperations";
                var response = WebAPIRequest.PostIntegrationAPIRequest<CableMaster>(url, objCbl, EntityType.Cable.ToString(), EntityAction.Save.ToString());
                //"{0} saved succes"
                if (isDirectSave)
                {
                    return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
                }
                Session["Splitcable"] = response.results;
                BLLayer objBLLayer = new BLLayer();
                objCbl.lstUserModule = objBLLayer.GetUserModuleAbbrList(objCbl.user_id, UserType.Web.ToString());
                return PartialView("_AddCable", response.results);
            }
        }

        //private bool fillReferenceentity(EntityReference entityReference)
        //{
        //    var listPointAReference=  entityReference.listPointAReference.Where(x=>) 
        //}
        public JsonResult checkOverlappingDesignId(string design_id, int system_id)
        {
            JsonResponse<dbresponse> objResp = new JsonResponse<dbresponse>();
            int res = BLCable.Instance.checkDuplicateDesignId(design_id, system_id);
            objResp.status = res.ToString();
            objResp.message = Resources.Resources.SI_OSP_CAB_NET_FRM_076;

            return Json(new { data = objResp, JsonRequestBehavior.AllowGet });
        }
        public PartialViewResult GetCableTubeCoreDetail(int? cableId)
        {
            List<TubeCoreInfo> objPortInfo = new List<TubeCoreInfo>();
            TubeCoreLstIn objRes = new TubeCoreLstIn();
            List<TubeCoreInfo> collection = BLCable.Instance.GetTubeCoreInfo(Convert.ToInt32(cableId));

            IEqualityComparer<TubeCoreInfo> customComparer =
                   new PropertyComparer<TubeCoreInfo>("tube_number");
            List<TubeCoreInfo> distinctTubenumber = collection.Distinct(customComparer).ToList();

            IEqualityComparer<TubeCoreInfo> customComparer1 =
                  new PropertyComparer<TubeCoreInfo>("core_number");
            List<TubeCoreInfo> distinctCore = collection.Distinct(customComparer1).ToList();

            objRes.objTube = distinctTubenumber;
            objRes.objCore = distinctCore;
            CableMaster objCable = new CableMaster();
            objCable.lstTubeCore = objRes;
            return PartialView("_CableTubeCoreInfo", objCable);
        }

        public PartialViewResult GetCableFiberDetail(int cableId = 0, string type = null)
        {
            CableFiberDetail objRes = new CableFiberDetail();
            objRes.cable_id = cableId;
            string url = "api/library/GetCableFiberDetail";
            var response = WebAPIRequest.PostIntegrationAPIRequest<List<CableFiberDetail>>
                (url, objRes, "", "");

            //List<CableFiberDetail> getCableFiberDetail = BLCable.Instance.GetFiberDetailInfo(Convert.ToInt32(cableId));
            if (response.results != null)
            {
                objRes.ViewCableFiberDetail = response.results;
            }
            //List<CableFiberDetail> getCableFiberDetail = BLCable.Instance.GetFiberDetailInfo(Convert.ToInt32(cableId));

            //objRes.ViewCableFiberDetail = getCableFiberDetail.ToList();
            var status = objRes.ViewCableFiberDetail.Select(x => x.core_status).Distinct().ToList();
            if (status.Count > 0)
            {
                foreach (var item in status)
                {
                    FibrePortStatusCount obj = new FibrePortStatusCount();
                    obj.PortStatus = item;
                    obj.StatusCount = objRes.ViewCableFiberDetail.Count(x => x.core_status == item);
                    objRes.ViewPortStatusCount.Add(obj);
                }
            }
            objRes.cable_id = cableId;

            objRes.type = type == null ? "" : Convert.ToString(type);
            var cableDetail = new BLMisc().GetEntityDetailById<CableMaster>(objRes.cable_id, EntityType.Cable);
            objRes.cable_network_id = cableDetail.network_id;
            objRes.cable_network_status = cableDetail.network_status;
            if (objRes.ViewCableFiberDetail == null || objRes.ViewCableFiberDetail.Count == 0)
            {
                objRes.PhysicalUtil = "0 %";
            }
            else
            {
                objRes.PhysicalUtil = objRes.ViewCableFiberDetail[0].PhysicalUtil == "" ? objRes.ViewCableFiberDetail[0].PhysicalUtil + " %" : "0 %";
            }
            PhysicallyUtil_ByUsed = objRes.PhysicalUtil;

            return PartialView("_CableFiberDetail", objRes);
        }
        public JsonResult getPhysicalUtil(int? cableID, string selectedValue)
        {
            var objResp = "0 %";

            if (selectedValue == "U")
            {
                objResp = PhysicallyUtil_ByUsed;
                //var objResp = BindPlanning(network_stage, ddlproject_id);
                //return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });

            }
            else if (selectedValue == "UR")
            {

                objResp = BLCable.Instance.getReservedUtilization(Convert.ToString(cableID)) + " %";


            }
            else
            {
                objResp = BLCable.Instance.getConnectivityUtilization(Convert.ToString(cableID)) + " %";
            }

            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public void DownloadCableFiberReport(int? cableId)
        {

            var objResp = "";
            try
            {
                List<CableFiberDetail> lstCableFiberDetail = BLCable.Instance.GetFiberDetailInfo(Convert.ToInt32(cableId));

                DataTable dtReport = new DataTable();
                DataTable dtHeader = new DataTable();

                dtReport = MiscHelper.ListToDataTable<CableFiberDetail>(lstCableFiberDetail);

                if (lstCableFiberDetail.Count > 0)
                {
                    DataColumn dc = new DataColumn("cable_network_id", typeof(String));
                    dtHeader.Columns.Add(dc);
                    dc = new DataColumn("cable_display_name", typeof(String));
                    dtHeader.Columns.Add(dc);
                    DataRow dr = dtHeader.NewRow();
                    dr[0] = lstCableFiberDetail.Select(x => x.cable_network_id).FirstOrDefault();
                    dtHeader.Rows.Add(dr);
                }
                if (dtReport.Rows.Count > 0)
                {
                    dtReport.Columns["CABLE_NETWORK_ID"].SetOrdinal(0);
                    dtReport.Columns.Remove("ViewCableFiberDetail");
                    dtReport.Columns.Remove("PHYSICALUTIL");
                    dtReport.Columns.Remove("cable_id");
                    dtReport.Columns.Remove("CABLE_NETWORK_STATUS");
                    dtReport.Columns.Remove("SYSTEM_ID");
                    dtReport.Columns.Remove("A_END_STATUS");
                    dtReport.Columns.Remove("B_END_STATUS");
                    dtReport.Columns.Remove("TYPE");
                    dtReport.Columns.Remove("LINK_SYSTEM_ID");
                    dtReport.Columns.Remove("VIEWPORTSTATUSCOUNT");
                    dtReport.Columns.Remove("cable_network_id");
                    // dtReport.Columns["FIBER_USAGE_STATUS"].ColumnName = "Status";
                    dtReport.Columns["TUBE_COLOR"].ColumnName = Resources.Resources.SI_OSP_CAB_NET_FRM_033;
                    dtReport.Columns["CORE_COLOR"].ColumnName = Resources.Resources.SI_OSP_CAB_NET_FRM_035;
                    dtReport.Columns["FIBER_NUMBER"].ColumnName = Resources.Resources.SI_OSP_CAB_NET_FRM_032;
                    dtReport.Columns["TUBE_NUMBER"].ColumnName = Resources.Resources.SI_ISP_CAB_NET_FRM_013;
                    dtReport.Columns["TUBE_COLOR_CODE"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_184;
                    dtReport.Columns["CORE_COLOR_CODE"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_185;
                    //dtReport.Columns["A_END_STATUS"].ColumnName = "A End Status";
                    //dtReport.Columns["B_END_STATUS"].ColumnName = "B End Status";
                    dtReport.Columns["CORE_NUMBER"].ColumnName = Resources.Resources.SI_ISP_CAB_NET_FRM_017;
                    dtReport.Columns["CORE_STATUS"].ColumnName = Resources.Resources.SI_OSP_CAB_NET_FRM_070;
                    dtReport.Columns["COMMENT"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_312;
                    //dtReport.Columns["LINK_SYSTEM_ID"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_186;
                    dtReport.Columns["LINK_ID"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_421;
                    //dtReport.Columns["CABLE_NETWORK_ID"].ColumnName = "Cable Network Id";

                    //dtReport.Columns[""].ColumnName = "";
                    //  dtReport.Columns.Add("Link Id");
                    //    for (int i = 0; i < dtReport.Rows.Count; i++)
                    //    {
                    //    if (dtReport.Rows[i + 7]["A_END_STATUS"].ToString().ToUpper() == "CONNECTED")
                    //    {
                    //        MySheet.Cells("D" + (i + 7)).Style.Fill.BackgroundColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);

                    //    }
                    //}
                    var filename = "Cable Fiber Details";
                    //dtReport.TableName = Session["CableNetworkId"].ToString();
                    ExportData(dtReport, dtHeader, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                    objResp = "ok";
                }

                else
                {
                    objResp = Resources.Resources.SI_GBL_GBL_NET_FRM_022;// "Not Found Data";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void ExportData(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        }
        private void ExportData(DataTable dtReport, DataTable header_dt, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport, header_dt);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        }

        public PartialViewResult AddCableComment()
        {
            return PartialView("_AddCableComment");
        }
        #endregion

        #region Split cable
        public PartialViewResult getSplitCable(int systemId, string entityType)
        {
            dynamic entityDetails = null;
            EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), entityType);
            SplitCable model = new SplitCable();
            switch (enType)
            {
                case EntityType.ADB:
                    entityDetails = new BLMisc().GetEntityDetailById<ADBMaster>(systemId, EntityType.ADB);
                    break;
                case EntityType.BDB:
                    entityDetails = new BLMisc().GetEntityDetailById<BDBMaster>(systemId, EntityType.BDB);
                    break;
                case EntityType.CDB:
                    entityDetails = new BLMisc().GetEntityDetailById<CDBMaster>(systemId, EntityType.CDB);
                    break;
                case EntityType.SpliceClosure:
                    entityDetails = new BLMisc().GetEntityDetailById<SCMaster>(systemId, EntityType.SpliceClosure);
                    break;
                case EntityType.FMS:
                    entityDetails = new BLMisc().GetEntityDetailById<FMSMaster>(systemId, EntityType.FMS);
                    break;
                case EntityType.FDB:
                    entityDetails = new BLMisc().GetEntityDetailById<FDBInfo>(systemId, EntityType.FDB);
                    break;
            }
            if (entityDetails != null)
            {
                model.split_entity_networkId = entityDetails.network_id;
                model.split_entity_system_id = entityDetails.system_id;
                model.split_entity_network_status = entityDetails.network_status;
                model.splitCore = (entityType == EntityType.SpliceClosure.ToString() ? entityDetails.no_of_ports : entityDetails.no_of_port);
                model.split_entity_type = entityType;
            }

            model.cables = new BLMisc().getNearByCables(systemId, entityType);
            model.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
            return PartialView("_SplitCable", model);
        }

        public JsonResult getNearCableDetail(int split_entity_system_id, string split_entity_type, int split_cable_system_id, int Split_entity_Core)
        {

            JsonResponse<SplitCableEntity> objResp = new JsonResponse<SplitCableEntity>();
            try
            {


                CableMaster cableDetail = new BLMisc().GetEntityDetailById<CableMaster>(split_cable_system_id, EntityType.Cable);
                //var splitEntitySettings = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == split_entity_type.ToUpper()).FirstOrDefault();
                //bool isVirtualPortAllowed = false;
                //if (splitEntitySettings != null)
                //{
                //    isVirtualPortAllowed = splitEntitySettings.is_virtual_port_allowed;
                //}
                //if (!isVirtualPortAllowed && cableDetail.total_core > Split_entity_Core)
                //{
                //    objResp.status = ResponseStatus.ERROR.ToString();
                //    objResp.message = "Split Entity ports are less than cable cores!";
                //    return Json(objResp, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                objResp.result = new BLMisc().getSplitCableEntity(split_entity_system_id, split_entity_type, split_cable_system_id, EntityType.Cable.ToString());
                //objResp.result.a_location = cableDetail.a_location;
                //objResp.result.b_location = cableDetail.b_location;
                objResp.result.parentCableNetworkId = cableDetail.network_id;
                objResp.result.network_status = cableDetail.network_status;
                //cable 1
                var extraLengthForCable1 = (ApplicationSettings.CableExtraLengthPercentage * objResp.result.cable1Length) / 100;
                objResp.result.cable1CalculatedLength = Math.Round(objResp.result.cable1Length + extraLengthForCable1, 3);

                //cable2
                var extraLengthForCable2 = (ApplicationSettings.CableExtraLengthPercentage * objResp.result.cable2Length) / 100;
                objResp.result.cable2CalculatedLength = Math.Round(objResp.result.cable2Length + extraLengthForCable2, 3);

                objResp.status = ResponseStatus.OK.ToString();
                // }

            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_308;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public CableMaster getCableObject(int cableNo, SplitCable splitModle, CableMaster CableMaster, string cable_a_location, string cable_b_location, float? cable_measured_length, float? cable_calculated_length, string cable_name, string cable_network_id, string geom)
        {
            CableMaster newObj = new CableMaster();

            CableMaster.unitValue = Convert.ToString(CableMaster.total_core);
            CableMaster.system_id = 0;

            if (cableNo == 1)
            {
                //CableMaster.a_entity_type = splitModle.old_cable_a_entity_type;
                //CableMaster.a_system_id = splitModle.old_cable_a_system_id;
                //CableMaster.a_location = splitModle.old_cable_a_location;

                //CableMaster.b_entity_type = splitModle.split_entity_type;
                //CableMaster.b_system_id = splitModle.split_entity_system_id;
                //CableMaster.b_location = splitModle.split_entity_networkId;

                CableMaster.start_reading = splitModle.cable_one_start_reading;
                CableMaster.end_reading = splitModle.cable_one_end_reading;
            }
            else
            {
                // CableMaster.a_entity_type = splitModle.split_entity_type;
                //CableMaster.a_system_id = splitModle.split_entity_system_id;
                //CableMaster.a_location = splitModle.split_entity_networkId;

                //CableMaster.b_entity_type = splitModle.old_cable_b_entity_type;
                // CableMaster.b_system_id = splitModle.old_cable_b_system_id;
                //CableMaster.b_location = splitModle.old_cable_b_location;

                CableMaster.start_reading = splitModle.cable_two_start_reading;
                CableMaster.end_reading = splitModle.cable_two_end_reading;
            }

            CableMaster.cable_calculated_length = (float)cable_calculated_length;
            CableMaster.cable_measured_length = (float)cable_measured_length;
            CableMaster.cable_name = cable_name;
            CableMaster.network_id = cable_network_id;
            CableMaster.geom = geom;

            return CableMaster;
        }

        public ActionResult saveSplitCable(SplitCable model)
        {
            model.userId = Convert.ToInt32(Session["user_id"]);
            PageMessage objPM = new PageMessage();
            try
            {
                int cableOneSystemid = 0;
                int cableTwoSystemid = 0;
                var cableDetail = new BLMisc().GetEntityDetailById<CableMaster>(model.split_cable_system_id, EntityType.Cable);
                model.old_cable_a_entity_type = cableDetail.a_entity_type;
                model.old_cable_a_system_id = cableDetail.a_system_id;
                model.old_cable_a_location = cableDetail.a_location;
                model.old_cable_b_entity_type = cableDetail.b_entity_type;
                model.old_cable_b_system_id = cableDetail.b_system_id;
                model.old_cable_b_location = cableDetail.b_location;
                model.parent_system_id = cableDetail.parent_system_id;
                model.parent_entity_type = cableDetail.parent_entity_type;
                model.parent_network_id = cableDetail.parent_network_id;

                var SplitCablesEntity = new BLMisc().getSplitCableEntity(model.split_entity_system_id, model.split_entity_type, model.split_cable_system_id, EntityType.Cable.ToString());
                var cableobjCable1 = getCableObject(1, model, cableDetail, model.cable_one_a_location, model.cable_one_b_location, model.cable_one_measured_length, model.cable_one_calculated_length, model.cable_one_name, model.cable_one_network_id, SplitCablesEntity.geom_cable1);
                cableobjCable1.a_system_id = SplitCablesEntity.cable1_a_system_id ?? 0;
                cableobjCable1.a_entity_type = SplitCablesEntity.cable1_a_entity_type;
                cableobjCable1.a_location = model.cable_one_a_location;

                cableobjCable1.b_system_id = SplitCablesEntity.cable1_b_system_id ?? 0;
                cableobjCable1.b_entity_type = SplitCablesEntity.cable1_b_entity_type;
                cableobjCable1.b_location = model.cable_one_b_location;
                cableobjCable1.gis_design_id = cableDetail.gis_design_id;
                var start_network_id = "";
                var end_network_id = "";
                var networkCodeDetail = new BLMisc().GetLineNetworkCode(start_network_id, end_network_id, "Cable", SplitCablesEntity.geom_cable1, "OSP");
                cableobjCable1.network_id = networkCodeDetail.network_code;
                cableobjCable1.cable_name = networkCodeDetail.network_code;
                cableobjCable1.sequence_id = networkCodeDetail.sequence_id;
                cableobjCable1.parent_cable_system_id = model.split_cable_system_id.ToString();
                cableobjCable1.parent_cable_netwok_id = model.cable_one_name;
                cableobjCable1.splited_by = model.userId.ToString();
                cableobjCable1.splitted_on = DateTime.Now.ToString();
                cableobjCable1.splitting_system_id = model.split_entity_system_id.ToString();
                cableobjCable1.splitting_netwok_id = model.split_entity_networkId;
                cableobjCable1.splitting_entitytype = model.split_entity_type;
                SaveCable(cableobjCable1, "CableInfo", false);
                var objcablelist1 = Session["Splitcable"];
                CableMaster cm1 = (CableMaster)objcablelist1;
                //-- Add LMC Attribute IF Existes
                CableMaster objCablemaster1 = new CableMaster();
                objCablemaster1.system_id = cm1.system_id;
                objCablemaster1.LMCCableInfo = new BLLmcInfo().GetLMCIfo(model.split_cable_system_id);
                if (!string.IsNullOrWhiteSpace(objCablemaster1.LMCCableInfo.lmc_id))
                {
                    objCablemaster1.LMCCableInfo.lstLMCAttachment = new BLAttachment().getAttachmentDetails(objCablemaster1.LMCCableInfo.system_id, "Cable", "Document", "LMCInfo");

                    var lstAttachmentIds = objCablemaster1.LMCCableInfo.lstLMCAttachment.Select(x => x.id.ToString()).ToList();
                    var strAttachmentIds = String.Join(",", lstAttachmentIds);
                    //var lstAttachmentEntitySystemIds = objCablemaster1.LMCCableInfo.lstLMCAttachment.Select(x => x.entity_system_id.ToString()).ToList();
                    //var strAttachmentEntitySystemIds = String.Join(",", lstAttachmentEntitySystemIds);

                    objCablemaster1.LMCCableInfo.cable_system_id = cableobjCable1.system_id;
                    objCablemaster1.LMCCableInfo.cable_network_id = cableobjCable1.network_id;
                    objCablemaster1.LMCCableInfo.system_id = 0;
                    objCablemaster1.LMCCableInfo.lmc_id = objCablemaster1.LMCCableInfo.lmc_id + "_01";

                    var details = BLLmcInfo.Instance.SaveLMCInfo(objCablemaster1.LMCCableInfo, Convert.ToInt32(Session["user_id"]));
                    string surveyStatus = new BLAttachment().UpdateLibraryAttachmentbyId(strAttachmentIds, details.system_id);
                }
                //---END; 
                cableOneSystemid = objCablemaster1.system_id;
                string[] geomObjLongLat = SplitCablesEntity.geom_cable2.Split(',');
                EditGeomIn geomObj = new EditGeomIn();
                geomObj.entityType = model.split_entity_type;
                geomObj.geomType = "Point";
                geomObj.isExisting = true;
                geomObj.longLat = geomObjLongLat[0];
                geomObj.systemId = model.split_entity_system_id;
                geomObj.networkStatus = model.split_entity_network_status;
                geomObj.userId = Convert.ToInt32(Session["user_id"]);
                BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);

                var cableobjCable2 = getCableObject(2, model, cableDetail, model.cable_two_a_location, model.cable_two_b_location, model.cable_two_measured_length, model.cable_two_calculated_length, model.cable_two_name, model.cable_two_network_id, SplitCablesEntity.geom_cable2);
                cableobjCable2.a_system_id = SplitCablesEntity.cable2_a_system_id ?? 0;
                cableobjCable2.a_entity_type = SplitCablesEntity.cable2_a_entity_type;
                cableobjCable2.a_location = model.cable_two_a_location;

                cableobjCable2.b_system_id = SplitCablesEntity.cable2_b_system_id ?? 0;
                cableobjCable2.b_entity_type = SplitCablesEntity.cable2_b_entity_type;
                cableobjCable2.b_location = model.cable_two_b_location;
                cableobjCable2.gis_design_id = cableDetail.gis_design_id;
                var networkCodeDetail2 = new BLMisc().GetLineNetworkCode(start_network_id, end_network_id, "Cable", SplitCablesEntity.geom_cable2, "OSP");
                cableobjCable2.network_id = networkCodeDetail2.network_code;
                cableobjCable2.cable_name = networkCodeDetail2.network_code;
                cableobjCable2.sequence_id = networkCodeDetail2.sequence_id;
                cableobjCable2.parent_cable_system_id = model.split_cable_system_id.ToString(); ;
                cableobjCable2.parent_cable_netwok_id = model.cable_one_name;
                cableobjCable2.splited_by = model.userId.ToString();
                cableobjCable2.splitted_on = DateTime.Now.ToString();
                cableobjCable2.splitting_system_id = model.split_entity_system_id.ToString();
                cableobjCable2.splitting_netwok_id = model.split_entity_networkId;
                cableobjCable2.splitting_entitytype = model.split_entity_type;
                SaveCable(cableobjCable2, "CableInfo", false);

                var objcablelist = Session["Splitcable"];
                CableMaster cm2 = (CableMaster)objcablelist;
                //-- Add LMC Attribute IF Existes
                CableMaster objCablemaster2 = new CableMaster();
                objCablemaster2.system_id = cm2.system_id;
                objCablemaster2.LMCCableInfo = new BLLmcInfo().GetLMCIfo(model.split_cable_system_id);
                int parentLMCId = objCablemaster2.LMCCableInfo.system_id;
                if (!string.IsNullOrWhiteSpace(objCablemaster1.LMCCableInfo.lmc_id))
                {
                    objCablemaster2.LMCCableInfo.cable_system_id = cableobjCable1.system_id;
                    objCablemaster2.LMCCableInfo.cable_network_id = cableobjCable1.network_id;
                    objCablemaster2.LMCCableInfo.system_id = 0;
                    objCablemaster2.LMCCableInfo.lmc_id = objCablemaster2.LMCCableInfo.lmc_id + "_02";
                    var result = BLLmcInfo.Instance.SaveLMCInfo(objCablemaster2.LMCCableInfo, Convert.ToInt32(Session["user_id"]));
                }
                //---END; 
                cableTwoSystemid = objCablemaster2.system_id;
                // make connection with split cable
                BLCable.Instance.SetConnectionWithSplitCable(cableOneSystemid, cableTwoSystemid, model.split_cable_system_id, model.split_entity_system_id, model.split_entity_networkId, model.split_entity_type, model.userId, model.splicing_source);

                // accociate split cables
                new BLMisc().AssociateSplitEntities(cableOneSystemid, cableTwoSystemid, networkCodeDetail.network_code, networkCodeDetail2.network_code, EntityType.Cable.ToString(), model.split_cable_system_id);

                //BLCable.Instance.DeleteCableById(model.split_cable_system_id);
                // Delete parent cable
                new BLMisc().deleteParentSplitEntity(model.split_cable_system_id, EntityType.Cable.ToString());
                //var abc= new BLMisc().deleteEntity(model.split_cable_system_id, EntityType.Cable.ToString(), GeometryType.Line.ToString());

                string[] LayerName = { EntityType.Cable.ToString() };
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, LayerName);
                model.objPM = objPM;
                model.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
                //----------------optimization of splicing------------------------------------------------
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    var status = new BLOSPSplicing().updatedisplayname();

                }).ContinueWith(tsk =>
                {
                    tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("Library", "saveSplitCable", ex); return true; });
                }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                //--------------------------------------------------------------------------------------------
                return PartialView("_SplitCable", model);
            }
            catch (Exception) { throw; }
        }

        public PartialViewResult getFiberDetail(int cableId, string type = null)
        {
            dynamic expando = new ExpandoObject();
            expando.cableId = cableId;
            expando.type = type;
            return PartialView("_FiberDetail", expando);
        }

        #endregion

        #region Duct


        private void BindDuctDropDown(DuctMaster objDuctIn)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Duct.ToString());
            objDuctIn.NoofDuctsCreated = objDDL.Where(x => x.dropdown_type == DropDownType.No_of_Ducts_Created.ToString()).ToList();
            objDuctIn.DuctTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Type.ToString()).ToList();
            objDuctIn.DuctColorIn = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Color.ToString()).ToList();
            objDuctIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objDuctIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        public DuctMaster GetDuctDetail(LineEntityIn objIn)
        {
            DuctMaster objDuct = new DuctMaster();
            if (objIn.systemId == 0 && objIn.pSystemId != 0)
            {
                objDuct = new BLMisc().GetEntityDetailById<DuctMaster>(objIn.pSystemId, EntityType.Duct);
                objDuct.geom = objIn.geom;
                objDuct.parent_system_id = objDuct.system_id;
                objDuct.parent_entity_type = EntityType.Duct.ToString();
                objDuct.duct_type = EntityType.Microduct.ToString();
                objDuct.a_system_id = objDuct.a_system_id;
                objDuct.a_entity_type = objDuct.a_entity_type;
                objDuct.a_location = objDuct.a_location;
                objDuct.b_system_id = objDuct.b_system_id;
                objDuct.b_entity_type = objDuct.b_entity_type;
                objDuct.b_location = objDuct.b_location;
                objDuct.trench_id = objDuct.trench_id;

                objDuct.manual_length = objDuct.manual_length;
                objDuct.pin_code = objDuct.pin_code;
                //objDuct.duct_category = EntityType.Microduct.ToString();
                objDuct.purpose_id = 0;
                objDuct.project_id = 0;
                objDuct.workorder_id = 0;
                objDuct.planning_id = 0;
                objDuct.specification = "";
                objDuct.purpose_id = 0;
                objDuct.vendor_id = 0;
                objDuct.item_code = "";
                objDuct.category = "";
                objDuct.subcategory1 = "";
                objDuct.subcategory2 = "";
                objDuct.subcategory3 = "";
                objDuct.color_code = "";
                objDuct.duct_type = "";
                objDuct.networkIdType = "M";
                //objDuct.type

                //objDuct.ownership_type = duct.ownership_type;
                //objDuct.region_id = duct.region_id;
                //objDuct.region_name = duct.region_name;
                //objDuct.province_id = duct.province_id;
                //objDuct.province_name = duct.province_name;
                //objDuct.manual_length = objDuct.manual_length;

                //objDuct.pin_code = duct.pin_code;

                objDuct.parent_network_id = objDuct.network_id;

                objDuct.system_id = 0;

            }
            else if (objIn.systemId == 0)
            {
                objDuct.geom = objIn.geom;
                if (!string.IsNullOrEmpty(objIn.geom))
                    objDuct.calculated_length = Math.Round((double)new BLMisc().GetCableLength(objIn.geom), 3);

                objDuct.manual_length = objDuct.calculated_length;
                objDuct.networkIdType = objIn.networkIdType;
                objDuct.ownership_type = VendorType.Own.ToString();
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objDuct, GeometryType.Line.ToString(), objIn.geom);

                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<DuctTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Duct);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objDuct);
            }
            else
            {
                objDuct = new BLMisc().GetEntityDetailById<DuctMaster>(objIn.systemId, EntityType.Duct);
                //objDuct.duct_category = EntityType.Duct.ToString();
            }

            return objDuct;
        }
        // { pSystemId: systemId, pEntityType: entityType, pNetworkId: networkId, pGeomType: geomType, entityType: 'Microduct', geomType: 'Line' }

        public PartialViewResult AddDuct(LineEntityIn objIn)
        {
            DuctMaster objDuct = new DuctMaster();
            //DuctMaster objDuct = new DuctMaster();

            //objDuct = GetDuctDetail(objIn);
            //if (objIn.systemId == 0)
            //{
            //    //Fill Location detail...    
            //    GetLineNetworkDetail(objDuct, objIn, EntityType.Duct.ToString(), false);
            //}
            //BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
            //fillProjectSpecifications(objDuct);
            //BindDuctDropDown(objDuct);
            //objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
            //return PartialView("_AddDuct", objDuct);
            objIn.system_id = objIn.systemId;
            objIn.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<DuctMaster>(url, objIn, EntityType.Duct.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddDuct", response.results);
        }

        public ActionResult SaveDuct(DuctMaster objDuct, bool isDirectSave = false, string pNetworkId = "")
        {
            var response = new ApiResponse<DuctMaster>();
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int pSystemId = objDuct.pSystemId;
            //if (objDuct.networkIdType == NetworkIdType.A.ToString() && objDuct.system_id == 0)
            //{
            //    if (isDirectSave == false)
            //    {
            //        objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.a_system_id, network_id = objDuct.a_location, network_name = objDuct.a_entity_type });
            //        objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.b_system_id, network_id = objDuct.b_location, network_name = objDuct.b_entity_type });
            //    }
            //    var objLineEntity = new LineEntityIn() { geom = objDuct.geom, systemId = objDuct.system_id, networkIdType = objDuct.networkIdType, lstTP = objDuct.lstTP };
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objDuct = GetDuctDetail(objLineEntity);
            //        objDuct.trench_id = pSystemId;
            //        objDuct.pNetworkId = pNetworkId;
            //    }
            //    GetLineNetworkDetail(objDuct, objLineEntity, EntityType.Duct.ToString(), true);
            //    if (isDirectSave == true)
            //        objDuct.duct_name = objDuct.network_id;
            //}

            //if (TryValidateModel(objDuct))
            //{

            //    var isNew = objDuct.system_id > 0 ? false : true;
            //    objDuct.trench_id = pSystemId;
            //    var resultItem = BLDuct.Instance.SaveDuct(objDuct, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.Duct.ToString() };

            //        //Save Reference
            //        if (objDuct.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objDuct.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
            //        }
            //        objDuct.objPM = objPM;
            //    }

            //    //save AT Status                        
            //    if (objDuct.ATAcceptance != null && objDuct.system_id > 0)
            //    {
            //        SaveATAcceptance(objDuct.ATAcceptance, objDuct.system_id);
            //    }

            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objDuct.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objDuct.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objDuct);
            //    BindDuctDropDown(objDuct);
            //    objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
            //    return PartialView("_AddDuct", objDuct);
            //}
            objDuct.isDirectSave = isDirectSave;
            objDuct.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objDuct.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objDuct.source_ref_type = "NETWORK_TICKET";
                objDuct.status = "D";
            }
            string url = "api/Library/EntityOperations";
            if (objDuct.system_id == 0)
            {

                if (objDuct.no_of_ducts_created == null)
                {
                    objDuct.no_of_ducts_created = 1;
                }
                for (int i = 0; i < objDuct.no_of_ducts_created; i++)
                {

                    response = WebAPIRequest.PostIntegrationAPIRequest<DuctMaster>(url, objDuct, EntityType.Duct.ToString(), EntityAction.Save.ToString());

                }
            }
            else
            {

                response = WebAPIRequest.PostIntegrationAPIRequest<DuctMaster>(url, objDuct, EntityType.Duct.ToString(), EntityAction.Save.ToString());

            }
            //var response = WebAPIRequest.PostIntegrationAPIRequest<DuctMaster>(url, objDuct, EntityType.Duct.ToString(), EntityAction.Save.ToString());
            //"{0} saved succes"
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddDuct", response.results);
        }

        #endregion
        #region MicroMicroduct
        private void BindMicroductDropDown(MicroductMaster objMicroductIn)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Microduct.ToString());
            objMicroductIn.NoofMicroductsCreated = objDDL.Where(x => x.dropdown_type == DropDownType.No_of_Microducts_Created.ToString()).ToList();
            objMicroductIn.MicroductTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Microduct_Type.ToString()).ToList();
            objMicroductIn.MicroductColorIn = objDDL.Where(x => x.dropdown_type == DropDownType.Microduct_Color.ToString()).ToList();
            objMicroductIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objMicroductIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        public MicroductMaster GetMicroductDetail(LineEntityIn objIn)
        {
            MicroductMaster objDuct = new MicroductMaster();
            if (objIn.systemId == 0 && objIn.pSystemId != 0)
            {
                objDuct = new BLMisc().GetEntityDetailById<MicroductMaster>(objIn.pSystemId, EntityType.Duct);
                objDuct.geom = objIn.geom;
                objDuct.parent_system_id = objDuct.system_id;
                objDuct.parent_entity_type = EntityType.Microduct.ToString();
                objDuct.microduct_type = EntityType.Microduct.ToString();
                objDuct.a_system_id = objDuct.a_system_id;
                objDuct.a_entity_type = objDuct.a_entity_type;
                objDuct.a_location = objDuct.a_location;
                objDuct.b_system_id = objDuct.b_system_id;
                objDuct.b_entity_type = objDuct.b_entity_type;
                objDuct.b_location = objDuct.b_location;
                objDuct.trench_id = objDuct.trench_id;

                objDuct.manual_length = objDuct.manual_length;
                objDuct.pin_code = objDuct.pin_code;
                //objDuct.duct_category = EntityType.Microduct.ToString();
                objDuct.purpose_id = 0;
                objDuct.project_id = 0;
                objDuct.workorder_id = 0;
                objDuct.planning_id = 0;
                objDuct.specification = "";
                objDuct.purpose_id = 0;
                objDuct.vendor_id = 0;
                objDuct.item_code = "";
                objDuct.category = "";
                objDuct.subcategory1 = "";
                objDuct.subcategory2 = "";
                objDuct.subcategory3 = "";
                objDuct.color_code = "";
                objDuct.microduct_type = "";
                objDuct.networkIdType = "M";

                objDuct.parent_network_id = objDuct.network_id;

                objDuct.system_id = 0;

            }
            else if (objIn.systemId == 0)
            {
                objDuct.geom = objIn.geom;
                if (!string.IsNullOrEmpty(objIn.geom))
                    objDuct.calculated_length = Math.Round((double)new BLMisc().GetCableLength(objIn.geom), 3);

                objDuct.manual_length = objDuct.calculated_length;
                objDuct.networkIdType = objIn.networkIdType;
                objDuct.ownership_type = VendorType.Own.ToString();
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objDuct, GeometryType.Line.ToString(), objIn.geom);

                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<MicroductTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Duct);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objDuct);
            }
            else
            {
                objDuct = new BLMisc().GetEntityDetailById<MicroductMaster>(objIn.systemId, EntityType.Duct);
            }

            return objDuct;
        }


        public PartialViewResult AddMicroduct(LineEntityIn objIn)
        {
            MicroductMaster objMicroduct = new MicroductMaster();
            objIn.system_id = objIn.systemId;
            objIn.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<MicroductMaster>(url, objIn, EntityType.Microduct.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddMicroduct", response.results);
        }

        public ActionResult SaveMicroduct(MicroductMaster objMicroduct, bool isDirectSave = false, string pNetworkId = "")
        {
            var response = new ApiResponse<MicroductMaster>();

            objMicroduct.isDirectSave = isDirectSave;
            objMicroduct.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objMicroduct.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objMicroduct.source_ref_type = "NETWORK_TICKET";
                objMicroduct.status = "D";
            }
            string url = "api/Library/EntityOperations";
            if (objMicroduct.system_id == 0)
            {

                if (objMicroduct.no_of_microducts_created == null)
                {
                    objMicroduct.no_of_microducts_created = 1;
                }
                for (int i = 0; i < objMicroduct.no_of_microducts_created; i++)
                {

                    response = WebAPIRequest.PostIntegrationAPIRequest<MicroductMaster>(url, objMicroduct, EntityType.Microduct.ToString(), EntityAction.Save.ToString());

                }
            }
            else
            {

                response = WebAPIRequest.PostIntegrationAPIRequest<MicroductMaster>(url, objMicroduct, EntityType.Microduct.ToString(), EntityAction.Save.ToString());

            }
            //var response = WebAPIRequest.PostIntegrationAPIRequest<MicroductMaster>(url, objMicroduct, EntityType.Microduct.ToString(), EntityAction.Save.ToString());
            //"{0} saved succes"
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddMicroduct", response.results);
        }

        #endregion
        public PartialViewResult AddGipipe(LineEntityIn objIn)
        {
            GipipeMaster objGipipe = new GipipeMaster();
            objIn.system_id = objIn.systemId;
            objIn.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<GipipeMaster>(url, objIn, EntityType.Gipipe.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddGipipe", response.results);
        }
        public ActionResult SaveGipipe(GipipeMaster objGipipe, bool isDirectSave = false, string pNetworkId = "")
        {

            objGipipe.isDirectSave = isDirectSave;
            objGipipe.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<GipipeMaster>(url, objGipipe, EntityType.Gipipe.ToString(), EntityAction.Save.ToString());
            //"{0} saved succes"
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddGipipe", response.results);
        }

        #region Split Duct
        public PartialViewResult getSplitDuct(int systemId, string entityType)
        {
            SplitDuct model = new SplitDuct();
            dynamic entityDetails = null;
            EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), entityType);
            switch (enType)
            {
                case EntityType.Pole:
                    entityDetails = new BLMisc().GetEntityDetailById<PoleMaster>(systemId, EntityType.Pole);
                    break;
                case EntityType.Manhole:
                    entityDetails = new BLMisc().GetEntityDetailById<ManholeMaster>(systemId, EntityType.Manhole);
                    break;
                case EntityType.Handhole:
                    entityDetails = new BLMisc().GetEntityDetailById<HandholeMaster>(systemId, EntityType.Handhole);
                    break;
                case EntityType.Tree:
                    entityDetails = new BLMisc().GetEntityDetailById<TreeMaster>(systemId, EntityType.Tree);
                    break;
                case EntityType.WallMount:
                    entityDetails = new BLMisc().GetEntityDetailById<WallMountMaster>(systemId, EntityType.WallMount);
                    break;
            }

            model.split_entity_networkId = entityDetails.network_id;
            model.split_entity_system_id = entityDetails.system_id;
            model.split_entity_network_status = entityDetails.network_status;
            model.split_entity_type = entityType;
            model.ducts = new BLMisc().getNearByDucts(systemId, entityType);
            return PartialView("_SplitDuct", model);
        }

        public JsonResult getNearDuctDetail(int split_entity_system_id, string split_entity_type, string splitEnityNetworkId, int split_duct_system_id)
        {

            JsonResponse<SplitDuctEntity> objResp = new JsonResponse<SplitDuctEntity>();
            try
            {
                DuctMaster ductDetail = new BLMisc().GetEntityDetailById<DuctMaster>(split_duct_system_id, EntityType.Duct);
                objResp.result = new BLMisc().getSplitDuctEntity(split_entity_system_id, split_entity_type, splitEnityNetworkId, split_duct_system_id, EntityType.Duct.ToString());

                //objResp.result.a_location = ductDetail.a_location;
                //objResp.result.b_location = ductDetail.b_location;
                objResp.result.parentDuctNetworkId = ductDetail.network_id;
                objResp.result.network_status = ductDetail.network_status;
                //Duct 1
                objResp.result.duct1CalculatedLength = Math.Round(objResp.result.duct1Length, 3);
                //Duct 2
                objResp.result.duct2CalculatedLength = Math.Round(objResp.result.duct2Length, 3);
                objResp.status = ResponseStatus.OK.ToString();

            }
            catch
            {
                string[] LayerName = { EntityType.Duct.ToString() };
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_309, ApplicationSettings.listLayerDetails, LayerName);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public DuctMaster getDuctObject(int ductNo, SplitDuct splitModle, DuctMaster DuctMaster, string duct_a_location, string duct_b_location, float? duct_measured_length, float? duct_calculated_length, string duct_name, string duct_network_id, string geom)
        {
            DuctMaster newObj = new DuctMaster();
            DuctMaster.system_id = 0;

            //if (ductNo == 1)
            //{
            //    DuctMaster.a_entity_type = splitModle.old_duct_a_entity_type;
            //    DuctMaster.a_system_id = splitModle.old_duct_a_system_id;
            //    DuctMaster.a_location = splitModle.old_duct_a_location;

            //    DuctMaster.b_entity_type = splitModle.split_entity_type;
            //    DuctMaster.b_system_id = splitModle.split_entity_system_id;
            //    DuctMaster.b_location = splitModle.split_entity_networkId;

            //}
            //else
            //{
            //    DuctMaster.a_entity_type = splitModle.split_entity_type;
            //    DuctMaster.a_system_id = splitModle.split_entity_system_id;
            //    DuctMaster.a_location = splitModle.split_entity_networkId;

            //    DuctMaster.b_entity_type = splitModle.old_duct_b_entity_type;
            //    DuctMaster.b_system_id = splitModle.old_duct_b_system_id;
            //    DuctMaster.b_location = splitModle.old_duct_b_location;
            //}
            DuctMaster.network_status = splitModle.network_status;
            DuctMaster.calculated_length = (float)duct_measured_length;
            DuctMaster.manual_length = (float)duct_calculated_length;
            DuctMaster.duct_name = duct_name;
            DuctMaster.network_id = duct_network_id;
            DuctMaster.geom = geom;

            return DuctMaster;
        }

        public ActionResult saveSplitDuct(SplitDuct model)
        {
            model.userId = Convert.ToInt32(Session["user_id"]);
            PageMessage objPM = new PageMessage();
            try
            {
                var ductDetail = new BLMisc().GetEntityDetailById<DuctMaster>(model.split_duct_system_id, EntityType.Duct);
                model.old_duct_a_entity_type = ductDetail.a_entity_type;
                model.old_duct_a_system_id = ductDetail.a_system_id;
                model.old_duct_a_location = ductDetail.a_location;
                model.old_duct_b_entity_type = ductDetail.b_entity_type;
                model.old_duct_b_system_id = ductDetail.b_system_id;
                model.old_duct_b_location = ductDetail.b_location;
                model.parent_system_id = ductDetail.parent_system_id;
                model.parent_entity_type = ductDetail.parent_entity_type;
                model.parent_network_id = ductDetail.parent_network_id;
                model.network_status = ductDetail.network_status;

                var SplitductsEntity = new BLMisc().getSplitDuctEntity(model.split_entity_system_id, model.split_entity_type, model.split_entity_networkId, model.split_duct_system_id, EntityType.Duct.ToString());

                var ductobjduct1 = getDuctObject(1, model, ductDetail, model.duct_one_a_location, model.duct_one_b_location, model.duct_one_measured_length, model.duct_one_calculated_length, model.duct_one_name, model.duct_one_network_id, SplitductsEntity.geom_duct1);
                ductobjduct1.a_system_id = SplitductsEntity.duct_one_a_system_id ?? 0;
                ductobjduct1.a_entity_type = SplitductsEntity.duct_one_a_entity_type;
                ductobjduct1.a_location = SplitductsEntity.duct_one_a_location;

                ductobjduct1.b_system_id = SplitductsEntity.duct_one_b_system_id ?? 0;
                ductobjduct1.b_entity_type = SplitductsEntity.duct_one_b_entity_type;
                ductobjduct1.b_location = SplitductsEntity.duct_one_b_location;
                ductobjduct1.parent_duct_system_id = model.split_duct_system_id.ToString();
                ductobjduct1.parent_duct_netwok_id = SplitductsEntity.common_name;
                ductobjduct1.splited_by = model.userId.ToString();
                ductobjduct1.splitted_on = DateTime.Now.ToString();
                ductobjduct1.splitting_system_id = model.split_entity_system_id.ToString();
                ductobjduct1.splitting_netwok_id = model.split_entity_networkId;
                ductobjduct1.splitting_entitytype = model.split_entity_type;
                SaveDuct(ductobjduct1, false);

                var ductobjduct2 = getDuctObject(2, model, ductDetail, model.duct_two_a_location, model.duct_two_b_location, model.duct_two_measured_length, model.duct_two_calculated_length, model.duct_two_name, model.duct_two_network_id, SplitductsEntity.geom_duct2);
                ductobjduct2.a_system_id = SplitductsEntity.duct_two_a_system_id ?? 0;
                ductobjduct2.a_entity_type = SplitductsEntity.duct_two_a_entity_type;
                ductobjduct2.a_location = SplitductsEntity.duct_two_a_location;

                ductobjduct2.b_system_id = SplitductsEntity.duct_two_b_system_id ?? 0;
                ductobjduct2.b_entity_type = SplitductsEntity.duct_two_b_entity_type;
                ductobjduct2.b_location = SplitductsEntity.duct_two_b_location;
                ductobjduct2.parent_duct_system_id = model.split_duct_system_id.ToString();
                ductobjduct2.parent_duct_netwok_id = SplitductsEntity.common_name;
                ductobjduct2.splited_by = model.userId.ToString();
                ductobjduct2.splitted_on = DateTime.Now.ToString();
                ductobjduct2.splitting_system_id = model.split_entity_system_id.ToString();
                ductobjduct2.splitting_netwok_id = model.split_entity_networkId;
                ductobjduct2.splitting_entitytype = model.split_entity_type;
                SaveDuct(ductobjduct2, false);
                // accociate split ducts
                new BLMisc().AssociateSplitEntities(ductobjduct1.system_id, ductobjduct2.system_id, model.duct_one_network_id, model.duct_two_network_id, EntityType.Duct.ToString(), model.split_duct_system_id);

                //BLDuct.Instance.DeleteDuctById(model.split_duct_system_id);
                new BLMisc().deleteParentSplitEntity(model.split_duct_system_id, EntityType.Duct.ToString());

                string[] geomObjLongLat = SplitductsEntity.geom_duct2.Split(',');

                EditGeomIn geomObj = new EditGeomIn();
                geomObj.entityType = model.split_entity_type;
                geomObj.geomType = "Point";
                geomObj.isExisting = true;
                geomObj.longLat = geomObjLongLat[0];
                geomObj.systemId = model.split_entity_system_id;
                geomObj.networkStatus = model.split_entity_network_status;
                geomObj.userId = Convert.ToInt32(Session["user_id"]);

                BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                string[] LayerName = { EntityType.Duct.ToString() };
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_179, ApplicationSettings.listLayerDetails, LayerName);
                model.objPM = objPM;
                return PartialView("_Splitduct", model);
            }
            catch (Exception) { throw; }
        }

        #endregion


        #region Split Microduct
        public PartialViewResult getSplitMicroduct(int systemId, string entityType)
        {
            SplitMicroduct model = new SplitMicroduct();
            dynamic entityDetails = null;
            EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), entityType);
            switch (enType)
            {
                case EntityType.Pole:
                    entityDetails = new BLMisc().GetEntityDetailById<PoleMaster>(systemId, EntityType.Pole);
                    break;
                case EntityType.Manhole:
                    entityDetails = new BLMisc().GetEntityDetailById<ManholeMaster>(systemId, EntityType.Manhole);
                    break;
                case EntityType.Handhole:
                    entityDetails = new BLMisc().GetEntityDetailById<HandholeMaster>(systemId, EntityType.Handhole);
                    break;
                case EntityType.Tree:
                    entityDetails = new BLMisc().GetEntityDetailById<TreeMaster>(systemId, EntityType.Tree);
                    break;
                case EntityType.WallMount:
                    entityDetails = new BLMisc().GetEntityDetailById<WallMountMaster>(systemId, EntityType.WallMount);
                    break;
            }

            model.split_entity_networkId = entityDetails.network_id;
            model.split_entity_system_id = entityDetails.system_id;
            model.split_entity_network_status = entityDetails.network_status;
            model.split_entity_type = entityType;
            model.microducts = new BLMisc().getNearByMicroducts(systemId, entityType);
            return PartialView("_SplitMicroduct", model);
        }

        public JsonResult getNearMicroductDetail(int split_entity_system_id, string split_entity_type, string splitEnityNetworkId, int split_microduct_system_id)
        {

            JsonResponse<SplitMicroductEntity> objResp = new JsonResponse<SplitMicroductEntity>();
            try
            {
                MicroductMaster MicroductDetail = new BLMisc().GetEntityDetailById<MicroductMaster>(split_microduct_system_id, EntityType.Microduct);
                objResp.result = new BLMisc().getSplitMicroductEntity(split_entity_system_id, split_entity_type, splitEnityNetworkId, split_microduct_system_id, EntityType.Microduct.ToString());
                //objResp.result.a_location = MicroductDetail.a_location;
                //objResp.result.b_location = MicroductDetail.b_location;
                objResp.result.parentmicroductNetworkId = MicroductDetail.network_id;
                objResp.result.network_status = MicroductDetail.network_status;
                //Microduct 1
                objResp.result.microduct1CalculatedLength = Math.Round(objResp.result.microduct1Length, 3);
                //Microduct 2
                objResp.result.microduct2CalculatedLength = Math.Round(objResp.result.microduct2Length, 3);
                objResp.status = ResponseStatus.OK.ToString();

            }
            catch
            {
                string[] LayerName = { EntityType.Microduct.ToString() };
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_309, ApplicationSettings.listLayerDetails, LayerName);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public MicroductMaster getMicroductObject(int MicroductNo, SplitMicroduct splitModle, MicroductMaster MicroductMaster, string Microduct_a_location, string Microduct_b_location, float? Microduct_measured_length, float? Microduct_calculated_length, string Microduct_name, string Microduct_network_id, string geom)
        {
            MicroductMaster newObj = new MicroductMaster();
            MicroductMaster.system_id = 0;


            MicroductMaster.network_status = splitModle.network_status;
            MicroductMaster.calculated_length = (float)Microduct_measured_length;
            MicroductMaster.manual_length = (float)Microduct_calculated_length;
            MicroductMaster.microduct_name = Microduct_name;
            MicroductMaster.network_id = Microduct_network_id;
            MicroductMaster.geom = geom;

            return MicroductMaster;
        }

        public ActionResult saveSplitMicroduct(SplitMicroduct model)
        {
            model.userId = Convert.ToInt32(Session["user_id"]);
            PageMessage objPM = new PageMessage();
            try
            {
                var MicroductDetail = new BLMisc().GetEntityDetailById<MicroductMaster>(model.split_microduct_system_id, EntityType.Microduct);
                model.old_microduct_a_entity_type = MicroductDetail.a_entity_type;
                model.old_microduct_a_system_id = MicroductDetail.a_system_id;
                model.old_microduct_a_location = MicroductDetail.a_location;
                model.old_microduct_b_entity_type = MicroductDetail.b_entity_type;
                model.old_microduct_b_system_id = MicroductDetail.b_system_id;
                model.old_microduct_b_location = MicroductDetail.b_location;
                model.parent_system_id = MicroductDetail.parent_system_id;
                model.parent_entity_type = MicroductDetail.parent_entity_type;
                model.parent_network_id = MicroductDetail.parent_network_id;
                model.network_status = MicroductDetail.network_status;

                var SplitMicroductsEntity = new BLMisc().getSplitMicroductEntity(model.split_entity_system_id, model.split_entity_type, model.split_entity_networkId, model.split_microduct_system_id, EntityType.Microduct.ToString());

                var MicroductobjMicroduct1 = getMicroductObject(1, model, MicroductDetail, model.microduct_one_a_location, model.microduct_one_b_location, model.microduct_one_measured_length, model.microduct_one_calculated_length, model.microduct_one_name, model.microduct_one_network_id, SplitMicroductsEntity.geom_microduct1);
                MicroductobjMicroduct1.a_system_id = SplitMicroductsEntity.microduct_one_a_system_id ?? 0;
                MicroductobjMicroduct1.a_entity_type = SplitMicroductsEntity.microduct_one_a_entity_type;
                MicroductobjMicroduct1.a_location = SplitMicroductsEntity.microduct_one_a_location;

                MicroductobjMicroduct1.b_system_id = SplitMicroductsEntity.microduct_one_b_system_id ?? 0;
                MicroductobjMicroduct1.b_entity_type = SplitMicroductsEntity.microduct_one_b_entity_type;
                MicroductobjMicroduct1.b_location = SplitMicroductsEntity.microduct_one_b_location;
                SaveMicroduct(MicroductobjMicroduct1, false);

                var MicroductobjMicroduct2 = getMicroductObject(2, model, MicroductDetail, model.microduct_two_a_location, model.microduct_two_b_location, model.microduct_two_measured_length, model.microduct_two_calculated_length, model.microduct_two_name, model.microduct_two_network_id, SplitMicroductsEntity.geom_microduct2);
                MicroductobjMicroduct2.a_system_id = SplitMicroductsEntity.microduct_two_a_system_id ?? 0;
                MicroductobjMicroduct2.a_entity_type = SplitMicroductsEntity.microduct_two_a_entity_type;
                MicroductobjMicroduct2.a_location = SplitMicroductsEntity.microduct_two_a_location;

                MicroductobjMicroduct2.b_system_id = SplitMicroductsEntity.microduct_two_b_system_id ?? 0;
                MicroductobjMicroduct2.b_entity_type = SplitMicroductsEntity.microduct_two_b_entity_type;
                MicroductobjMicroduct2.b_location = SplitMicroductsEntity.microduct_two_b_location;
                SaveMicroduct(MicroductobjMicroduct2, false);
                // accociate split Microducts
                new BLMisc().AssociateSplitEntities(MicroductobjMicroduct1.system_id, MicroductobjMicroduct2.system_id, model.microduct_one_network_id, model.microduct_two_network_id, EntityType.Microduct.ToString(), model.split_microduct_system_id);

                //BLMicroduct.Instance.DeleteMicroductById(model.split_Microduct_system_id);
                new BLMisc().deleteParentSplitEntity(model.split_microduct_system_id, EntityType.Microduct.ToString());

                string[] geomObjLongLat = SplitMicroductsEntity.geom_microduct2.Split(',');

                EditGeomIn geomObj = new EditGeomIn();
                geomObj.entityType = model.split_entity_type;
                geomObj.geomType = "Point";
                geomObj.isExisting = true;
                geomObj.longLat = geomObjLongLat[0];
                geomObj.systemId = model.split_entity_system_id;
                geomObj.networkStatus = model.split_entity_network_status;
                geomObj.userId = Convert.ToInt32(Session["user_id"]);

                BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                string[] LayerName = { EntityType.Microduct.ToString() };
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_179, ApplicationSettings.listLayerDetails, LayerName);
                model.objPM = objPM;
                return PartialView("_SplitMicroduct", model);
            }
            catch (Exception) { throw; }
        }

        #endregion
        #region Split Trench
        public PartialViewResult getSplitTrench(int systemId, string entityType)
        {
            SplitTrench model = new SplitTrench();
            dynamic entityDetails = null;
            EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), entityType);
            switch (enType)
            {

                case EntityType.Manhole:
                    entityDetails = new BLMisc().GetEntityDetailById<ManholeMaster>(systemId, EntityType.Manhole);
                    break;
                case EntityType.Handhole:
                    entityDetails = new BLMisc().GetEntityDetailById<HandholeMaster>(systemId, EntityType.Handhole);
                    break;

            }
            model.split_entity_networkId = entityDetails.network_id;
            model.split_entity_system_id = entityDetails.system_id;
            model.split_entity_network_status = entityDetails.network_status;
            model.split_entity_type = entityType;
            model.trenchs = new BLMisc().getNearByTrenchs(systemId, entityType);
            return PartialView("_SplitTrench", model);
        }

        public JsonResult getNearTrenchDetail(int split_entity_system_id, string split_entity_type, string splitEnityNetworkId, int split_trench_system_id)
        {

            JsonResponse<SplitTrenchEntity> objResp = new JsonResponse<SplitTrenchEntity>();
            try
            {
                TrenchMaster trenchDetail = new BLMisc().GetEntityDetailById<TrenchMaster>(split_trench_system_id, EntityType.Trench);
                objResp.result = new BLMisc().getSplitTrenchEntity(split_entity_system_id, split_entity_type, splitEnityNetworkId, split_trench_system_id, EntityType.Trench.ToString());
                objResp.result.parentTrenchNetworkId = trenchDetail.network_id;
                objResp.result.network_status = trenchDetail.network_status;
                //Trench 1
                objResp.result.trench1CalculatedLength = Math.Round(objResp.result.trench1Length, 3);
                //Trench 2
                objResp.result.trench2CalculatedLength = Math.Round(objResp.result.trench2Length, 3);
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch
            {
                string[] LayerName = { EntityType.Trench.ToString() };
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_309, ApplicationSettings.listLayerDetails, LayerName);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public TrenchMaster getTrenchObject(int TrenchNo, SplitTrench splitModle, TrenchMaster TrenchMaster, string trench_a_location, string trench_b_location, float? trench_measured_length, float? trench_calculated_length, string trench_name, string trench_network_id, string geom)
        {
            TrenchMaster.system_id = 0;
            TrenchMaster.network_status = splitModle.network_status;
            TrenchMaster.trench_length = (float)trench_calculated_length;
            TrenchMaster.trench_name = trench_name;
            TrenchMaster.network_id = trench_network_id;
            TrenchMaster.geom = geom;
            return TrenchMaster;
        }

        public ActionResult saveSplitTrench(SplitTrench model)
        {
            model.userId = Convert.ToInt32(Session["user_id"]);
            PageMessage objPM = new PageMessage();
            try
            {
                var trenchDetail = new BLMisc().GetEntityDetailById<TrenchMaster>(model.split_trench_system_id, EntityType.Trench);
                model.old_trench_a_entity_type = trenchDetail.a_entity_type;
                model.old_trench_a_system_id = trenchDetail.a_system_id;
                model.old_trench_a_location = trenchDetail.a_location;
                model.old_trench_b_entity_type = trenchDetail.b_entity_type;
                model.old_trench_b_system_id = trenchDetail.b_system_id;
                model.old_trench_b_location = trenchDetail.b_location;
                model.parent_system_id = trenchDetail.parent_system_id;
                model.parent_entity_type = trenchDetail.parent_entity_type;
                model.parent_network_id = trenchDetail.parent_network_id;
                model.network_status = trenchDetail.network_status;

                var SplittrenchsEntity = new BLMisc().getSplitTrenchEntity(model.split_entity_system_id, model.split_entity_type, model.split_entity_networkId, model.split_trench_system_id, EntityType.Trench.ToString());

                var trenchobjtrench1 = getTrenchObject(1, model, trenchDetail, model.trench_one_a_location, model.trench_one_b_location, model.trench_one_measured_length, model.trench_one_calculated_length, model.trench_one_name, model.trench_one_network_id, SplittrenchsEntity.geom_trench1);
                trenchobjtrench1.a_system_id = SplittrenchsEntity.trench_one_a_system_id ?? 0;
                trenchobjtrench1.a_entity_type = SplittrenchsEntity.trench_one_a_entity_type;
                trenchobjtrench1.a_location = SplittrenchsEntity.trench_one_a_location;

                trenchobjtrench1.b_system_id = SplittrenchsEntity.trench_one_b_system_id ?? 0;
                trenchobjtrench1.b_entity_type = SplittrenchsEntity.trench_one_b_entity_type;
                trenchobjtrench1.b_location = SplittrenchsEntity.trench_one_b_location;
                trenchobjtrench1.parent_trench_system_id = model.split_trench_system_id.ToString();
                trenchobjtrench1.parent_trench_netwok_id = SplittrenchsEntity.common_name;
                trenchobjtrench1.splited_by = model.userId.ToString();
                trenchobjtrench1.splitted_on = DateTime.Now.ToString();
                trenchobjtrench1.splitting_system_id = model.split_entity_system_id.ToString();
                trenchobjtrench1.splitting_netwok_id = model.split_entity_networkId;
                trenchobjtrench1.splitting_entitytype = model.split_entity_type;
                SaveTrench(trenchobjtrench1, false);

                var trenchobjtrench2 = getTrenchObject(2, model, trenchDetail, model.trench_two_a_location, model.trench_two_b_location, model.trench_two_measured_length, model.trench_two_calculated_length, model.trench_two_name, model.trench_two_network_id, SplittrenchsEntity.geom_trench2);
                trenchobjtrench2.a_system_id = SplittrenchsEntity.trench_two_a_system_id ?? 0;
                trenchobjtrench2.a_entity_type = SplittrenchsEntity.trench_two_a_entity_type;
                trenchobjtrench2.a_location = SplittrenchsEntity.trench_two_a_location;

                trenchobjtrench2.b_system_id = SplittrenchsEntity.trench_two_b_system_id ?? 0;
                trenchobjtrench2.b_entity_type = SplittrenchsEntity.trench_two_b_entity_type;
                trenchobjtrench2.b_location = SplittrenchsEntity.trench_two_b_location;
                trenchobjtrench2.parent_trench_system_id = model.split_trench_system_id.ToString();
                trenchobjtrench2.parent_trench_netwok_id = SplittrenchsEntity.common_name;
                trenchobjtrench2.splited_by = model.userId.ToString();
                trenchobjtrench2.splitted_on = DateTime.Now.ToString();
                trenchobjtrench2.splitting_system_id = model.split_entity_system_id.ToString();
                trenchobjtrench2.splitting_netwok_id = model.split_entity_networkId;
                trenchobjtrench2.splitting_entitytype = model.split_entity_type;
                SaveTrench(trenchobjtrench2, false);

                new BLMisc().AssociateSplitEntities(trenchobjtrench1.system_id, trenchobjtrench2.system_id, model.trench_one_network_id, model.trench_two_network_id, EntityType.Duct.ToString(), model.split_trench_system_id);
                new BLMisc().deleteParentSplitEntity(model.split_trench_system_id, EntityType.Trench.ToString());

                string[] geomObjLongLat = SplittrenchsEntity.geom_trench2.Split(',');
                EditGeomIn geomObj = new EditGeomIn();
                geomObj.entityType = model.split_entity_type;
                geomObj.geomType = "Point";
                geomObj.isExisting = true;
                geomObj.longLat = geomObjLongLat[0];
                geomObj.systemId = model.split_entity_system_id;
                geomObj.networkStatus = model.split_entity_network_status;
                geomObj.userId = Convert.ToInt32(Session["user_id"]);

                BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                string[] LayerName = { EntityType.Trench.ToString() };
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_179, ApplicationSettings.listLayerDetails, LayerName);
                model.objPM = objPM;
                return PartialView("_SplitTrench", model);
            }
            catch (Exception) { throw; }
        }

        #endregion





        #region Conduit 


        private void BindConduitDropDown(ConduitMaster objConduitIn)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Conduit.ToString());
            objConduitIn.ConduitTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.conduit_Type.ToString()).ToList();
            objConduitIn.ConduitColorIn = objDDL.Where(x => x.dropdown_type == DropDownType.Conduit_Color.ToString()).ToList();
            objConduitIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objConduitIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        public ConduitMaster GetConduitDetail(LineEntityIn objIn)
        {
            ConduitMaster objConduit = new ConduitMaster();
            var userdetails = (User)Session["userDetail"];
            if (objIn.systemId == 0 && objIn.pSystemId != 0)
            {
                objConduit = new BLMisc().GetEntityDetailById<ConduitMaster>(objIn.pSystemId, EntityType.Conduit);
                objConduit.geom = objIn.geom;
                objConduit.parent_system_id = objConduit.system_id;
                objConduit.parent_entity_type = EntityType.Trench.ToString();
                objConduit.conduit_type = EntityType.Conduit.ToString();
                objConduit.a_system_id = objConduit.a_system_id;
                objConduit.a_entity_type = objConduit.a_entity_type;
                objConduit.a_location = objConduit.a_location;
                objConduit.b_system_id = objConduit.b_system_id;
                objConduit.b_entity_type = objConduit.b_entity_type;
                objConduit.b_location = objConduit.b_location;
                objConduit.trench_id = objConduit.trench_id;
                objConduit.manual_length = objConduit.manual_length;
                objConduit.pin_code = objConduit.pin_code;
                //objConduit.duct_category = EntityType.Microduct.ToString();
                objConduit.purpose_id = 0;
                objConduit.project_id = 0;
                objConduit.workorder_id = 0;
                objConduit.planning_id = 0;
                objConduit.specification = "";
                objConduit.purpose_id = 0;
                objConduit.vendor_id = 0;
                objConduit.item_code = "";
                objConduit.category = "";
                objConduit.subcategory1 = "";
                objConduit.subcategory2 = "";
                objConduit.subcategory3 = "";
                objConduit.color_code = "";
                objConduit.conduit_type = "";
                objConduit.networkIdType = "M";
                //objConduit.type

                //objConduit.ownership_type = duct.ownership_type;
                //objConduit.region_id = duct.region_id;
                //objConduit.region_name = duct.region_name;
                //objConduit.province_id = duct.province_id;
                //objConduit.province_name = duct.province_name;
                //objConduit.manual_length = objConduit.manual_length;

                //objConduit.pin_code = duct.pin_code;

                objConduit.parent_network_id = objConduit.network_id;

                objConduit.system_id = 0;

            }
            else if (objIn.systemId == 0)
            {
                objConduit.geom = objIn.geom;
                if (!string.IsNullOrEmpty(objIn.geom))
                    objConduit.calculated_length = Math.Round((double)new BLMisc().GetCableLength(objIn.geom), 3);

                objConduit.manual_length = objConduit.calculated_length;
                objConduit.networkIdType = objIn.networkIdType;
                objConduit.ownership_type = VendorType.Own.ToString();
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objConduit, GeometryType.Line.ToString(), objIn.geom);

                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<ConduitTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Conduit);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objConduit);
            }
            else
            {
                objConduit = new BLMisc().GetEntityDetailById<ConduitMaster>(objIn.systemId, EntityType.Conduit);
                //objConduit.duct_category = EntityType.Duct.ToString();
            }
            objConduit.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objConduit;
        }
        // { pSystemId: systemId, pEntityType: entityType, pNetworkId: networkId, pGeomType: geomType, entityType: 'Microduct', geomType: 'Line' }

        public PartialViewResult AddConduit(LineEntityIn objIn)
        {
            ConduitMaster objConduit = new ConduitMaster();
            //DuctMaster objDuct = new DuctMaster();

            //objDuct = GetDuctDetail(objIn);
            //if (objIn.systemId == 0)
            //{
            //    //Fill Location detail...    
            //    GetLineNetworkDetail(objDuct, objIn, EntityType.Duct.ToString(), false);
            //}
            //BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
            //fillProjectSpecifications(objDuct);
            //BindDuctDropDown(objDuct);
            //objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
            //return PartialView("_AddDuct", objDuct);
            objIn.system_id = objIn.systemId;
            objIn.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ConduitMaster>(url, objIn, EntityType.Conduit.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddConduit", response.results);
        }

        public ActionResult SaveConduit(ConduitMaster objConduit, bool isDirectSave = false, string pNetworkId = "")
        {

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int pSystemId = objDuct.pSystemId;
            //if (objDuct.networkIdType == NetworkIdType.A.ToString() && objDuct.system_id == 0)
            //{
            //    if (isDirectSave == false)
            //    {
            //        objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.a_system_id, network_id = objDuct.a_location, network_name = objDuct.a_entity_type });
            //        objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.b_system_id, network_id = objDuct.b_location, network_name = objDuct.b_entity_type });
            //    }
            //    var objLineEntity = new LineEntityIn() { geom = objDuct.geom, systemId = objDuct.system_id, networkIdType = objDuct.networkIdType, lstTP = objDuct.lstTP };
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objDuct = GetDuctDetail(objLineEntity);
            //        objDuct.trench_id = pSystemId;
            //        objDuct.pNetworkId = pNetworkId;
            //    }
            //    GetLineNetworkDetail(objDuct, objLineEntity, EntityType.Duct.ToString(), true);
            //    if (isDirectSave == true)
            //        objDuct.duct_name = objDuct.network_id;
            //}

            //if (TryValidateModel(objDuct))
            //{

            //    var isNew = objDuct.system_id > 0 ? false : true;
            //    objDuct.trench_id = pSystemId;
            //    var resultItem = BLDuct.Instance.SaveDuct(objDuct, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.Duct.ToString() };

            //        //Save Reference
            //        if (objDuct.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objDuct.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
            //        }
            //        objDuct.objPM = objPM;
            //    }

            //    //save AT Status                        
            //    if (objDuct.ATAcceptance != null && objDuct.system_id > 0)
            //    {
            //        SaveATAcceptance(objDuct.ATAcceptance, objDuct.system_id);
            //    }

            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objDuct.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objDuct.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objDuct);
            //    BindDuctDropDown(objDuct);
            //    objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
            //    return PartialView("_AddDuct", objDuct);
            //}
            objConduit.isDirectSave = isDirectSave;
            objConduit.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ConduitMaster>(url, objConduit, EntityType.Conduit.ToString(), EntityAction.Save.ToString());
            //"{0} saved succes"
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddConduit", response.results);
        }

        #endregion

        #region Split Conduit
        public PartialViewResult getSplitConduit(int systemId, string entityType)
        {
            SplitConduit model = new SplitConduit();
            dynamic entityDetails = null;
            EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), entityType);
            switch (enType)
            {
                case EntityType.Pole:
                    entityDetails = new BLMisc().GetEntityDetailById<PoleMaster>(systemId, EntityType.Pole);
                    break;
                case EntityType.Manhole:
                    entityDetails = new BLMisc().GetEntityDetailById<ManholeMaster>(systemId, EntityType.Manhole);
                    break;
                case EntityType.Tree:
                    entityDetails = new BLMisc().GetEntityDetailById<TreeMaster>(systemId, EntityType.Tree);
                    break;
                case EntityType.WallMount:
                    entityDetails = new BLMisc().GetEntityDetailById<WallMountMaster>(systemId, EntityType.WallMount);
                    break;
            }

            model.split_entity_networkId = entityDetails.network_id;
            model.split_entity_system_id = entityDetails.system_id;
            model.split_entity_network_status = entityDetails.network_status;
            model.split_entity_type = entityType;
            model.conduits = new BLMisc().getNearByConduits(systemId, entityType);
            return PartialView("_SplitConduit", model);
        }

        public JsonResult getNearConduitDetail(int split_entity_system_id, string split_entity_type, string splitEnityNetworkId, int split_Conduit_system_id)
        {

            JsonResponse<SplitConduitEntity> objResp = new JsonResponse<SplitConduitEntity>();
            try
            {
                ConduitMaster conduitDetail = new BLMisc().GetEntityDetailById<ConduitMaster>(split_Conduit_system_id, EntityType.Conduit);
                objResp.result = new BLMisc().getSplitConduitEntity(split_entity_system_id, split_entity_type, splitEnityNetworkId, split_Conduit_system_id, EntityType.Conduit.ToString());
                //objResp.result.a_location = ductDetail.a_location;
                //objResp.result.b_location = ductDetail.b_location;
                objResp.result.parentConduitNetworkId = conduitDetail.network_id;
                objResp.result.network_status = conduitDetail.network_status;
                //Duct 1
                objResp.result.conduit1CalculatedLength = Math.Round(objResp.result.conduit1Length, 3);
                //Duct 2
                objResp.result.conduit2CalculatedLength = Math.Round(objResp.result.conduit2Length, 3);
                objResp.status = ResponseStatus.OK.ToString();

            }
            catch
            {
                string[] LayerName = { EntityType.Conduit.ToString() };
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_309, ApplicationSettings.listLayerDetails, LayerName);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ConduitMaster getConduitObject(int ductNo, SplitConduit splitModle, ConduitMaster ConduitMaster, string conduit_a_location, string conduit_b_location, float? conduit_measured_length, float? conduit_calculated_length, string conduit_name, string conduit_network_id, string geom)
        {
            ConduitMaster newObj = new ConduitMaster();
            ConduitMaster.system_id = 0;

            //if (ductNo == 1)
            //{
            //    DuctMaster.a_entity_type = splitModle.old_duct_a_entity_type;
            //    DuctMaster.a_system_id = splitModle.old_duct_a_system_id;
            //    DuctMaster.a_location = splitModle.old_duct_a_location;

            //    DuctMaster.b_entity_type = splitModle.split_entity_type;
            //    DuctMaster.b_system_id = splitModle.split_entity_system_id;
            //    DuctMaster.b_location = splitModle.split_entity_networkId;

            //}
            //else
            //{
            //    DuctMaster.a_entity_type = splitModle.split_entity_type;
            //    DuctMaster.a_system_id = splitModle.split_entity_system_id;
            //    DuctMaster.a_location = splitModle.split_entity_networkId;

            //    DuctMaster.b_entity_type = splitModle.old_duct_b_entity_type;
            //    DuctMaster.b_system_id = splitModle.old_duct_b_system_id;
            //    DuctMaster.b_location = splitModle.old_duct_b_location;
            //}
            ConduitMaster.network_status = splitModle.network_status;
            ConduitMaster.calculated_length = (float)conduit_measured_length;
            ConduitMaster.manual_length = (float)conduit_calculated_length;
            ConduitMaster.network_name = conduit_name;
            ConduitMaster.network_id = conduit_network_id;
            ConduitMaster.geom = geom;

            return ConduitMaster;
        }

        public ActionResult saveSplitConduit(SplitConduit model)
        {
            model.userId = Convert.ToInt32(Session["user_id"]);
            PageMessage objPM = new PageMessage();
            try
            {
                var conduitDetail = new BLMisc().GetEntityDetailById<ConduitMaster>(model.split_conduit_system_id, EntityType.Conduit);
                model.old_conduit_a_entity_type = conduitDetail.a_entity_type;
                model.old_conduit_a_system_id = conduitDetail.a_system_id;
                model.old_conduit_a_location = conduitDetail.a_location;
                model.old_conduit_b_entity_type = conduitDetail.b_entity_type;
                model.old_conduit_b_system_id = conduitDetail.b_system_id;
                model.old_conduit_b_location = conduitDetail.b_location;
                model.parent_system_id = conduitDetail.parent_system_id;
                model.parent_entity_type = conduitDetail.parent_entity_type;
                model.parent_network_id = conduitDetail.parent_network_id;
                model.network_status = conduitDetail.network_status;

                var SplitconduitsEntity = new BLMisc().getSplitConduitEntity(model.split_entity_system_id, model.split_entity_type, model.split_entity_networkId, model.split_conduit_system_id, EntityType.Conduit.ToString());

                var conduitobjconduit1 = getConduitObject(1, model, conduitDetail, model.conduit_one_a_location, model.conduit_one_b_location, model.conduit_one_measured_length, model.conduit_one_calculated_length, model.conduit_one_name, model.conduit_one_network_id, SplitconduitsEntity.geom_conduit1);
                conduitobjconduit1.a_system_id = SplitconduitsEntity.conduit_one_a_system_id ?? 0;
                conduitobjconduit1.a_entity_type = SplitconduitsEntity.conduit_one_a_entity_type;
                conduitobjconduit1.a_location = SplitconduitsEntity.conduit_one_a_location;

                conduitobjconduit1.b_system_id = SplitconduitsEntity.conduit_one_b_system_id ?? 0;
                conduitobjconduit1.b_entity_type = SplitconduitsEntity.conduit_one_b_entity_type;
                conduitobjconduit1.b_location = SplitconduitsEntity.conduit_one_b_location;
                SaveConduit(conduitobjconduit1, false);

                var conduitobjconduit2 = getConduitObject(2, model, conduitDetail, model.conduit_two_a_location, model.conduit_two_b_location, model.conduit_two_measured_length, model.conduit_two_calculated_length, model.conduit_two_name, model.conduit_two_network_id, SplitconduitsEntity.geom_conduit2);
                conduitobjconduit2.a_system_id = SplitconduitsEntity.conduit_two_a_system_id ?? 0;
                conduitobjconduit2.a_entity_type = SplitconduitsEntity.conduit_two_a_entity_type;
                conduitobjconduit2.a_location = SplitconduitsEntity.conduit_two_a_location;

                conduitobjconduit2.b_system_id = SplitconduitsEntity.conduit_two_b_system_id ?? 0;
                conduitobjconduit2.b_entity_type = SplitconduitsEntity.conduit_two_b_entity_type;
                conduitobjconduit2.b_location = SplitconduitsEntity.conduit_two_b_location;
                SaveConduit(conduitobjconduit2, false);
                // accociate split conduits
                new BLMisc().AssociateSplitEntities(conduitobjconduit1.system_id, conduitobjconduit2.system_id, model.conduit_one_network_id, model.conduit_two_network_id, EntityType.Conduit.ToString(), model.split_conduit_system_id);

                //BLconduit.Instance.DeleteconduitById(model.split_conduit_system_id);
                new BLMisc().deleteEntity(model.split_conduit_system_id, EntityType.Conduit.ToString(), GeometryType.Line.ToString(), model.userId);

                string[] geomObjLongLat = SplitconduitsEntity.geom_conduit2.Split(',');

                EditGeomIn geomObj = new EditGeomIn();
                geomObj.entityType = model.split_entity_type;
                geomObj.geomType = "Point";
                geomObj.isExisting = true;
                geomObj.longLat = geomObjLongLat[0];
                geomObj.systemId = model.split_entity_system_id;
                geomObj.networkStatus = model.split_entity_network_status;
                geomObj.userId = Convert.ToInt32(Session["user_id"]);

                BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                string[] LayerName = { EntityType.Conduit.ToString() };
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_179, ApplicationSettings.listLayerDetails, LayerName);
                model.objPM = objPM;
                return PartialView("_Splitconduit", model);
            }
            catch (Exception) { throw; }
        }

        #endregion


        #region Trench

        private void BindTrenchDropDown(TrenchMaster objTrenchIn)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Trench.ToString());
            objTrenchIn.trenchTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Trench_Type.ToString()).ToList();
            objTrenchIn.MCGMWardIn = objDDL.Where(x => x.dropdown_type == DropDownType.MCGM_Ward.ToString()).ToList();
            objTrenchIn.StrataTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Strata_Type.ToString()).ToList();
            objTrenchIn.SurfaceTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Surface_Type.ToString()).ToList();
            objTrenchIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objTrenchIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }

        public TrenchMaster GetTrenchDetail(LineEntityIn objIn)
        {
            TrenchMaster objTrench = new TrenchMaster();
            var userdetails = (User)Session["userDetail"];
            if (objIn.systemId == 0)
            {
                objTrench.geom = objIn.geom;
                if (!string.IsNullOrEmpty(objIn.geom))
                    objTrench.trench_length = (double)new BLMisc().GetCableLength(objIn.geom);

                objTrench.networkIdType = objIn.networkIdType;
                objTrench.ownership_type = "Own";
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objTrench, GeometryType.Line.ToString(), objIn.geom);

                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<TrenchTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Trench);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objTrench);
            }
            else
            {
                objTrench = new BLMisc().GetEntityDetailById<TrenchMaster>(objIn.systemId, EntityType.Trench);
            }
            objTrench.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objTrench;
        }

        public PartialViewResult AddTrench(LineEntityIn objIn)
        {
            //TrenchMaster objTrench = new TrenchMaster();
            //objTrench = GetTrenchDetail(objIn);
            //if (objIn.systemId == 0)
            //{
            //    //Fill Location detail...    
            //    GetLineNetworkDetail(objTrench, objIn, EntityType.Trench.ToString(), false);
            //}

            //BLItemTemplate.Instance.BindItemDropdowns(objTrench, EntityType.Trench.ToString());
            //BindTrenchDropDown(objTrench);

            //fillProjectSpecifications(objTrench);
            //objTrench.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Trench.ToString()).ToList();
            //return PartialView("_AddTrench", objTrench);
            objIn.system_id = objIn.systemId;
            objIn.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TrenchMaster>(url, objIn, EntityType.Trench.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddTrench", response.results);
        }

        public ActionResult SaveTrench(TrenchMaster objTrench, bool isDirectSave = false)
        {

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objTrench.networkIdType == NetworkIdType.A.ToString() && objTrench.system_id == 0)
            //{
            //    if (isDirectSave == false)
            //    {
            //        objTrench.lstTP.Add(new NetworkDtl { system_id = objTrench.a_system_id, network_id = objTrench.a_location, network_name = objTrench.a_entity_type });
            //        objTrench.lstTP.Add(new NetworkDtl { system_id = objTrench.b_system_id, network_id = objTrench.b_location, network_name = objTrench.b_entity_type });
            //    }
            //    var objLineEntity = new LineEntityIn() { geom = objTrench.geom, systemId = objTrench.system_id, networkIdType = objTrench.networkIdType, lstTP = objTrench.lstTP };
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objTrench = GetTrenchDetail(objLineEntity);
            //    }
            //    GetLineNetworkDetail(objTrench, objLineEntity, EntityType.Trench.ToString(), true);
            //    if (isDirectSave == true)
            //        objTrench.trench_name = objTrench.network_id;
            //}

            //if (TryValidateModel(objTrench))
            //{
            //    var isNew = objTrench.system_id > 0 ? false : true;
            //    var resultItem = BLTrench.Instance.SaveTrench(objTrench, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.Trench.ToString() };
            //        //Save Reference
            //        if (objTrench.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objTrench.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        objTrench.objPM = objPM;
            //    }

            //    //save AT Status                        
            //    if (objTrench.ATAcceptance != null && objTrench.system_id > 0)
            //    {
            //        SaveATAcceptance(objTrench.ATAcceptance, objTrench.system_id);
            //    }


            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objTrench.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objTrench.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objTrench, EntityType.Trench.ToString());
            //    BindTrenchDropDown(objTrench);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objTrench);
            //    objTrench.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Trench.ToString()).ToList();
            //    return PartialView("_AddTrench", objTrench);
            //}
            objTrench.isDirectSave = isDirectSave;
            objTrench.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objTrench.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objTrench.source_ref_type = "NETWORK_TICKET";
                objTrench.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TrenchMaster>(url, objTrench, EntityType.Trench.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            //"{0} saved succes"
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddTrench", response.results);
        }

        #endregion

        #region Termination Point

        public ActionResult TerminationPoint(string geom = "", string geomType = "")
        {
            return PartialView("_TerminationPoint");
        }

        public JsonResult TerminationEntity(TerminationEntityRequest obj)
        {
            //JsonResponse<List<TerminationPointDtl>> objResp = new JsonResponse<List<TerminationPointDtl>>();
            //try
            //{
            //    var respTP = new BLMisc().GetTerminationDetail(txtGeom, mtrBuffer, entityType, Convert.ToInt32(Session["user_id"]));
            //    objResp.result = respTP;
            //    objResp.status = ResponseStatus.OK.ToString();

            //}
            //catch
            //{
            //    objResp.status = ResponseStatus.ERROR.ToString();
            //    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_298;
            //}
            //return Json(objResp, JsonRequestBehavior.AllowGet);
            obj.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/main/TerminationEntity";
            var response = WebAPIRequest.PostIntegrationAPIRequest<List<TerminationPointDtl>>(url, obj, "", "");
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region MergeCables
        public ActionResult MergeCables()
        {
            ViewBag.lstLayers = new BLLayer().GetAllLayerDetail().Where(x => x.isvisible == true).ToList();
            return PartialView("_MergeCable");
        }

        public JsonResult CompleteMergeCablesOperation(int MasterCableId, int SecondCableId)
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
            var Output = BLCable.Instance.CompleteMergecableOperation(MasterCableId, SecondCableId, user_id);
            return Json(Output[0], JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Reference
        private EntityReference FillEntityReference(int entityId, string entityType)
        {
            layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == entityType.ToUpper()).FirstOrDefault();

            EntityReference entityReference = new EntityReference();
            List<Reference> listReference = new List<Reference>();
            var defaultRow = 4;
            var user_id = Convert.ToInt32(Session["user_id"]);

            listReference = BLReference.Instance.GetReference(entityId, entityType);
            listReference.ForEach(x => { x.modified_by = user_id; x.modified_on = DateTimeHelper.Now; });

            List<Reference> listPointAReference = new List<Reference>();
            List<Reference> listPointBReference = new List<Reference>();

            if (listReference.Any())
            {
                listPointAReference = listReference.Where(x => x.entry_point == "PointA").ToList();
                listPointBReference = listReference.Where(x => x.entry_point == "PointB").ToList();
            }

            var pointALoop = defaultRow - listPointAReference.Count;
            var pointBLoop = defaultRow - listPointBReference.Count;

            // fill pointA list 
            if (pointALoop > 0)
            {
                for (int i = 0; i < pointALoop; i++)
                {
                    Reference reference = new Reference();
                    reference.id = 0;
                    reference.landmark = "";
                    reference.entity_type = entityType;
                    reference.system_id = entityId;
                    reference.entry_point = "PointA";
                    reference.created_by = user_id;
                    listPointAReference.Add(reference);
                }
            }

            //pointB list will add only line type entities
            if (layerDetail.geom_type.ToUpper() == GeometryType.Line.ToString().ToUpper())
            {
                if (pointBLoop > 0)
                {
                    for (int i = 0; i < pointBLoop; i++)
                    {
                        Reference reference = new Reference();
                        reference.id = 0;
                        reference.landmark = "";
                        reference.entity_type = entityType;
                        reference.system_id = entityId;
                        reference.entry_point = "PointB";
                        reference.created_by = user_id;
                        listPointBReference.Add(reference);
                    }
                }
            }
            entityReference.entityLayer = layerDetail;
            entityReference.listPointAReference = listPointAReference;
            entityReference.listPointBReference = listPointBReference;
            //Bind the direction drop down
            BindReferenceDirectionDD(entityReference);

            return entityReference;
        }

        private void BindReferenceDirectionDD(EntityReference entityReference)
        {
            var objDDL = new BLMisc().GetDropDownList("", DropDownType.Reference_Direction.ToString());
            entityReference.listRefrencedirection = objDDL.ToList();

        }

        private void SaveReference(EntityReference entityReference, int system_id)
        {
            BLReference.Instance.SaveReference(entityReference, system_id);
        }
        public PartialViewResult GetReferenceEntity(int entityId, string entityType)
        {
            EntityReference entityReference = FillEntityReference(entityId, entityType);
            return PartialView("_AddReference", entityReference);
        }
        #endregion

        #region Splice Tray
        public PartialViewResult GetSpliceTrayInfo(int entityId, string entityType)
        {
            List<SpliceTrayInfo> spliceTrayInf = new List<SpliceTrayInfo>();
            spliceTrayInf = BLSpliceTray.Instance.GetSpliceTrayInfo(entityId, entityType);
            return PartialView("~/Views/Shared/_SpliceTrayInfo.cshtml", spliceTrayInf);
        }
        public JsonResult DeleteSpliceTrayInfoById(int spliceTrayId)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var objShaft = BLSpliceTray.Instance.DeleteSpliceTaryInfoById(spliceTrayId);
                if (objShaft == 1)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_300;
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Acceptance Testing

        public PartialViewResult GetATAcceptance(int entityId, string entityType, string featureName)
        {
            var objDDL = new BLMisc().GetDropDownList("", DropDownType.AT_Status.ToString());
            entityATStatusAtachmentsList objATStatus = new entityATStatusAtachmentsList();
            objATStatus.listAtStatusRecords = BLATAcceptance.Instance.GetATStatus(entityId, entityType);//FillATAcceptance(entityId, entityType);
            objATStatus.listATStatus = objDDL.ToList();
            objATStatus.listAtAttachments = new BLAttachment().getAttachmentDetails(entityId, entityType, "Document", featureName); //new BLATAttachment().getATAAttachmentDetails(entityId, entityType, "Document");
                                                                                                                                    //converting file size

            foreach (var item in objATStatus.listAtAttachments)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            objATStatus.systemId = entityId;
            objATStatus.entityType = entityType;
            objATStatus.createdBy = Convert.ToInt32(Session["user_id"]);
            return PartialView("_ATAcceptance", objATStatus);
        }
        private void SaveATAcceptance(entityATStatusAtachmentsList objList, int system_id)
        {
            BLATAcceptance.Instance.SaveATAcceptance(objList.listAtStatusRecords, system_id, Convert.ToInt32(Session["user_id"]));
        }
        public PartialViewResult getATAttachmentDetails(int system_Id, string entity_type, string featureName)
        {
            var lstDocument = new BLAttachment().getAttachmentDetails(system_Id, entity_type, "Document", featureName); //new BLATAttachment().getATAAttachmentDetails(system_Id, entity_type, "Document");
                                                                                                                        //converting file size

            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            return PartialView("_AtAttachments", lstDocument);
        }

        public PartialViewResult GetExecutionmethod(int entityId, string entityType)
        {
            var objDDL = new BLMisc().GetDropDownList(entityType, DropDownType.execution_Method.ToString());
            trenchExecutionList objExMethodStatus = new trenchExecutionList();
            objExMethodStatus.listExecutionRecords = BLExecution.Instance.GetExecutionStatus(entityId, entityType);//FillATAcceptance(entityId, entityType);
            objExMethodStatus.listExecutionmethod = objDDL.ToList();
            objExMethodStatus.systemId = entityId;
            objExMethodStatus.entityType = entityType;
            objExMethodStatus.createdBy = Convert.ToInt32(Session["user_id"]);
            return PartialView("_trenchExecution", objExMethodStatus);
        }
        private void SaveExecutionmethod(trenchExecutionList objList, int system_id)
        {
            BLExecution.Instance.SaveExecutionmethod(objList.listExecutionRecords, system_id, Convert.ToInt32(Session["user_id"]));
        }
        #endregion

        #region Maintainence Charges
        public PartialViewResult AddMaintenanceCharge(int systemId, int entityId, string entityType)
        {
            EntityMaintainenceCharges objMaintenanceCharge = new EntityMaintainenceCharges();
            objMaintenanceCharge.entity_id = entityId;
            objMaintenanceCharge.entity_type = entityType;
            var chargeDetails = BLMaintainenceCharges.Instance.getChargeDetails(systemId);
            if (chargeDetails != null) { objMaintenanceCharge = chargeDetails; }
            objMaintenanceCharge.listTypeOfActivityCharge = new BLMisc().GetDropDownList("", DropDownType.type_of_activity_charge.ToString());
            objMaintenanceCharge.listChargeCategory = new BLMisc().GetDropDownList("", DropDownType.charge_category.ToString());
            return PartialView("_AddMaintenanceCharge", objMaintenanceCharge);
        }
        public PartialViewResult SaveMaintenanceCharge(EntityMaintainenceCharges objCharge)
        {
            var isNew = objCharge.id > 0;
            PageMessage objPM = new PageMessage();
            objCharge = BLMaintainenceCharges.Instance.SaveChargeDetails(objCharge, Convert.ToInt32(Session["user_id"]));
            objCharge.listTypeOfActivityCharge = new BLMisc().GetDropDownList("", DropDownType.type_of_activity_charge.ToString());
            objCharge.listChargeCategory = new BLMisc().GetDropDownList("", DropDownType.charge_category.ToString());
            objPM.status = StatusCodes.OK.ToString();
            if (isNew)
            {
                objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_165;
            }
            else { objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_166; }
            objCharge.objPM = objPM;
            return PartialView("_AddMaintenanceCharge", objCharge);
        }
        public PartialViewResult MaintenanceChargesList(int entityId, string entityType)
        {
            List<EntityMaintainenceCharges> objEntityMaintainenceChargesList = new List<EntityMaintainenceCharges>();
            objEntityMaintainenceChargesList = BLMaintainenceCharges.Instance.GetEntityMaintainenceChargesRecords(entityId, entityType);
            return PartialView("_MaintenanceChargesList", objEntityMaintainenceChargesList);

        }
        public PartialViewResult GetEntityMaintaincenceCharges(int entityId, string entityType, string featureName)
        {
            var objtype_of_activity_charge = new BLMisc().GetDropDownList("", DropDownType.type_of_activity_charge.ToString());
            var objcharge_category = new BLMisc().GetDropDownList("", DropDownType.charge_category.ToString());

            EntityMaintainenceChargesList objEntityMaintainenceChargesList = new EntityMaintainenceChargesList();
            objEntityMaintainenceChargesList.listEntityMaintainenceChargesRecords = BLMaintainenceCharges.Instance.GetEntityMaintainenceChargesRecords(entityId, entityType);

            objEntityMaintainenceChargesList.listTypeOfActivityCharge = objtype_of_activity_charge.ToList();
            objEntityMaintainenceChargesList.listChargeCategory = objcharge_category.ToList();
            objEntityMaintainenceChargesList.entityId = entityId;
            objEntityMaintainenceChargesList.entityType = entityType;
            objEntityMaintainenceChargesList.listEntityMaintainenceChargesAttachments = new BLAttachment().getAttachmentDetails(entityId, entityType, featureName);
            //new BLMaintainenceChargesFiles().getEntityMaintainenceChargesAttachmentDetails(entityId, entityType, "Document");
            //converting file size
            foreach (var item in objEntityMaintainenceChargesList.listEntityMaintainenceChargesAttachments)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }

            return PartialView("_MaintainenceCharges", objEntityMaintainenceChargesList);
        }

        public PartialViewResult GetEntityMaintaincenceChargesAttachmentDetails(int system_Id, string entity_type, string featureName)
        {
            //var lstDocument = new BLMaintainenceChargesFiles().getEntityMaintainenceChargesAttachmentDetails(system_Id, entity_type, "Document");
            var lstDocument = new BLAttachment().getAttachmentDetails(system_Id, entity_type, "Document", featureName);
            //converting file size
            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            return PartialView("_MaintainenceChargesFiles", lstDocument);
        }

        public static String BytesToString(long byteCount)
        {
            string[] suf = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0 " + suf[1];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }

        #endregion

        #region COMMON

        private void fillProjectSpecifications(dynamic objLib)
        {
            //"P" we need to pass this value as dynamically as network stage selection
            objLib.lstBindProjectCode = new BLProject().getProjectCodeDetails("P");
            objLib.lstBindPlanningCode = new BLProject().getPlanningDetailByIdList(Convert.ToInt32(objLib.project_id ?? 0));
            objLib.lstBindWorkorderCode = new BLProject().getWorkorderDetailByIdList(Convert.ToInt32(objLib.planning_id ?? 0));
            objLib.lstBindPurposeCode = new BLProject().getPurposeDetailByIdList(Convert.ToInt32(objLib.workorder_id ?? 0));
            objLib.lstRouteInfo = new BLLayer().getRouteInfo("0");
        }

        private void fillCableDetails(dynamic objLib)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Cable.ToString());
            objLib.listcableType = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Type.ToString()).ToList();
            objLib.listcableCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Category.ToString()).ToList();
        }
        private void BindFaultDetails(dynamic objLib)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Fault.ToString());
            objLib.listFaultType = objDDL.Where(x => x.dropdown_type == DropDownType.Fault_Status.ToString()).ToList();

        }
        private void BindFilterSplitterTypeDetails(dynamic objLib)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Splitter.ToString());
            objLib.listsplittertype = objDDL.Where(x => x.dropdown_type == DropDownType.Splitter_Type.ToString()).ToList();

        }
        private void BindOwnershipDetails(dynamic objLib)
        {
            objLib.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objLib.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        private void fillParentDetail(dynamic objLib, NetworkCodeIn objIn, string networkIdType)
        {
            //fill parent detail....
            var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
            if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    //FILL NETWORK CODE FORMAT FOR MANUAL
                    objLib.network_id = networkCodeDetail.network_code;
                }
                objLib.parent_entity_type = networkCodeDetail.parent_entity_type;
                objLib.parent_network_id = networkCodeDetail.parent_network_id;
                objLib.parent_system_id = networkCodeDetail.parent_system_id;
            }
        }




        private void fillNetworkCodeDetail(dynamic objLib, NetworkCodeIn objIn)
        {

            //fill Network Code detail....
            var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
            if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
            {
                objLib.network_id = networkCodeDetail.network_code;
                objLib.sequence_id = networkCodeDetail.sequence_id;
            }
        }

        private void fillRegionProvinceDetail(dynamic objEntityModel, string enType, string geom)
        {
            List<InRegionProvince> objRegionProvince = new List<InRegionProvince>();
            objRegionProvince = BLBuilding.Instance.GetRegionProvince(geom, enType);
            if (objRegionProvince != null && objRegionProvince.Count > 0)
            {
                objEntityModel.region_id = objRegionProvince[0].region_id;
                objEntityModel.province_id = objRegionProvince[0].province_id;
                objEntityModel.region_name = objRegionProvince[0].region_name;
                objEntityModel.province_name = objRegionProvince[0].province_name;
            }
            List<InGeographicDetails> obj = new List<InGeographicDetails>();
            obj = BLBuilding.Instance.GetGeographicDetails(geom, enType);
            try
            {
                if (obj != null && obj.Count > 0)
                {
                    foreach (var item in obj)
                    {

                        if (item.entity_type.ToUpper() == EntityType.Area.ToString().ToUpper())
                        {
                            objEntityModel.area_id = item.entity_network_id;
                        }

                        if (item.entity_type.ToUpper() == EntityType.SubArea.ToString().ToUpper())
                        {
                            objEntityModel.subarea_id = item.entity_network_id;
                        }
                        if (item.entity_type.ToUpper() == EntityType.DSA.ToString().ToUpper())
                        {
                            objEntityModel.dsa_id = item.entity_network_id;
                        }
                        if (item.entity_type.ToUpper() == EntityType.CSA.ToString().ToUpper())
                        {
                            objEntityModel.csa_id = item.entity_network_id;
                        }
                    }
                    objEntityModel.region_abbreviation = obj[0].region_abbreviation;
                    objEntityModel.province_abbreviation = obj[0].province_abbreviation;
                }
            }
            catch (Exception ex)
            {
                string exception = ex.Message;
            }
        }

        public string getFirstErrorFromModelState()
        {
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    if (error.ErrorMessage != "")
                        return error.ErrorMessage;
                }
            }
            return "";
        }

        public JsonResult IsNetworkIdExist(string networkId, string entityType, string networkStage)
        {
            if (entityType == EntityType.Microduct.ToString()) entityType = EntityType.Duct.ToString();
            if (entityType == "") entityType = EntityType.Duct.ToString();
            JsonResponse<string> objResp = new JsonResponse<string>();
            var IsNetworkIDExist = new BLMisc().chkNetworkIdExist(networkId, entityType, networkStage);
            objResp.status = IsNetworkIDExist == true ? ResponseStatus.OK.ToString() : ResponseStatus.FAILED.ToString();
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        private string GetPointTypeParentGeom(int pSystemId, string pEntityType)
        {
            string geom = "";
            //get parent detail..
            var dicParentEntityDetail = new BLMisc().GetEntityDetailById<Dictionary<string, string>>(pSystemId, (EntityType)Enum.Parse(typeof(EntityType), pEntityType));
            if (dicParentEntityDetail != null)
            {
                //set geometry value as parent..
                geom = dicParentEntityDetail["longitude"] + " " + dicParentEntityDetail["latitude"];
            }
            return geom;
        }

        private void GetLineNetworkDetail(dynamic objLib, LineEntityIn objIn, string enName, bool isAuto, int pSystemId = 0, string pEntityType = "")
        {
            var startObj = new NetworkDtl();
            var endObj = new NetworkDtl();
            var start_network_id = "";
            var end_network_id = "";
            var start_entity_name = "";
            var end_entity_name = "";
            if (objIn.lstTP != null && objIn.lstTP.Count > 0)
            {
                startObj = objIn.lstTP[0];
                start_network_id = startObj.network_id;
                start_entity_name = startObj.entity_name;
            }
            if (objIn.lstTP != null && objIn.lstTP.Count > 1)
            {
                endObj = objIn.lstTP[objIn.lstTP.Count() - 1];
                end_network_id = endObj.network_id;
                end_entity_name = startObj.entity_name;
            }

            //fill parent detail....
            var networkCodeDetail = new BLMisc().GetLineNetworkCode(start_network_id, end_network_id, enName, objIn.geom, "OSP", pSystemId, pEntityType);

            if (!string.IsNullOrEmpty(networkCodeDetail.network_code))
            {
                if (objIn.networkIdType == NetworkIdType.M.ToString())
                {
                    //FILL NETWORK CODE FORMAT FOR MANUAL
                    objLib.network_id = networkCodeDetail.network_code;
                }
                else if (objIn.networkIdType == NetworkIdType.A.ToString() && isAuto)
                {
                    objLib.network_id = networkCodeDetail.network_code;
                }
            }
            if (networkCodeDetail.parent_system_id != 0)
            {
                objLib.network_id = networkCodeDetail.network_code;

            }
            if (objLib.parent_system_id == 0)
            {
                objLib.parent_entity_type = networkCodeDetail.parent_entity_type;
                objLib.parent_network_id = networkCodeDetail.parent_network_id;
                objLib.parent_system_id = networkCodeDetail.parent_system_id;
                objLib.a_entity_type = startObj.network_name;
                objLib.a_system_id = startObj.system_id;
                objLib.a_location = start_network_id;
                objLib.b_entity_type = endObj.network_name;
                objLib.b_system_id = endObj.system_id;
                objLib.b_location = end_network_id;
            }
            objLib.networkIdType = objIn.networkIdType;
            objLib.sequence_id = networkCodeDetail.sequence_id;

        }

        #endregion

        #region WallMount

        public PartialViewResult AddWallMount(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //WallMountMaster objWallMountMaster = GetWallMountDetail(networkIdType, systemId, geom);
            //BLItemTemplate.Instance.BindItemDropdowns(objWallMountMaster, EntityType.WallMount.ToString());
            //BindWallMountDropDown(objWallMountMaster);
            //fillProjectSpecifications(objWallMountMaster);
            //return PartialView("_AddWallMount", objWallMountMaster);

            WallMountMaster obj = new WallMountMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.pEntityType = pEntityType;
            obj.pSystemId = pSystemId;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<WallMountMaster>(url, obj, EntityType.WallMount.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddWallMount", response.results);
        }

        public WallMountMaster GetWallMountDetail(string networkIdType, int systemId, string geom = "")
        {
            WallMountMaster objWallMount = new WallMountMaster();
            var userdetails = (User)Session["userDetail"];
            objWallMount.geom = geom;
            objWallMount.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objWallMount, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objWallMount, new NetworkCodeIn() { eType = EntityType.WallMount.ToString(), gType = GeometryType.Point.ToString(), eGeom = objWallMount.geom }, networkIdType);
                objWallMount.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objWallMount.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objWallMount.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<WallMountTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.WallMount);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objWallMount);
            }
            else
            {
                // Get entity detail by Id...
                objWallMount = new BLMisc().GetEntityDetailById<WallMountMaster>(systemId, EntityType.WallMount);
            }
            objWallMount.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objWallMount;
        }

        public ActionResult SaveWallMount(WallMountMaster objWallMountMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objWallMountMaster.networkIdType == NetworkIdType.A.ToString() && objWallMountMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.WallMount.ToString(), gType = GeometryType.Point.ToString(), eGeom = objWallMountMaster.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objWallMountMaster = GetWallMountDetail(objWallMountMaster.networkIdType, objWallMountMaster.system_id, objWallMountMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objWallMountMaster.wallmount_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objWallMountMaster.network_id = objNetworkCodeDetail.network_code;
            //    objWallMountMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}
            //if (TryValidateModel(objWallMountMaster))
            //{
            //    var isNew = objWallMountMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLWallMount().SaveEntityWallMount(objWallMountMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {

            //        //Save Reference
            //        if (objWallMountMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objWallMountMaster.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = Resources.Resources.SI_OSP_WMT_NET_FRM_007;
            //        }
            //        else
            //        {
            //            BLLoopMangment.Instance.UpdateLoopDetails(objWallMountMaster.system_id, "WallMount", objWallMountMaster.network_id, objWallMountMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objWallMountMaster.longitude + " " + objWallMountMaster.latitude }, Convert.ToInt32(Session["user_id"]));
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = Resources.Resources.SI_OSP_WMT_NET_FRM_008;
            //        }
            //        objWallMountMaster.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objWallMountMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objWallMountMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objWallMountMaster, EntityType.WallMount.ToString());
            //    BindWallMountDropDown(objWallMountMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(objWallMountMaster);
            //    return PartialView("_AddWallMount", objWallMountMaster);
            //}
            objWallMountMaster.isDirectSave = isDirectSave;
            objWallMountMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objWallMountMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objWallMountMaster.source_ref_type = "NETWORK_TICKET";
                objWallMountMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<WallMountMaster>(url, objWallMountMaster, EntityType.WallMount.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddWallMount", response.results);
        }
        private void BindWallMountDropDown(WallMountMaster objWallMountMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.WallMount.ToString());
            //objWallMountMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objWallMountMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objWallMountMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }

        #endregion

        #region PEP

        public PartialViewResult AddPEP(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            
            PEPMaster obj = new PEPMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.pEntityType = pEntityType;
            obj.pSystemId = pSystemId;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PEPMaster>(url, obj, EntityType.PEP.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddPEP", response.results);
        }

        public PEPMaster GetPEPDetail(string networkIdType, int systemId, string geom = "")
        {
            PEPMaster objPEP = new PEPMaster();
            var userdetails = (User)Session["userDetail"];
            objPEP.geom = geom;
            objPEP.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objPEP, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objPEP, new NetworkCodeIn() { eType = EntityType.PEP.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPEP.geom }, networkIdType);
                objPEP.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objPEP.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objPEP.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<PEPTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.PEP);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objPEP);
            }
            else
            {
                // Get entity detail by Id...
                objPEP = new BLMisc().GetEntityDetailById<PEPMaster>(systemId, EntityType.PEP);
            }
            objPEP.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objPEP;
        }

        public ActionResult SavePEP(PEPMaster objPEPMaster, bool isDirectSave = false)
        {

            objPEPMaster.isDirectSave = isDirectSave;
            objPEPMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objPEPMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objPEPMaster.source_ref_type = "NETWORK_TICKET";
                objPEPMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PEPMaster>(url, objPEPMaster, EntityType.PEP.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddPEP", response.results);
        }
        private void BindPEPDropDown(PEPMaster objPEPMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.PEP.ToString());
            //objWallMountMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objPEPMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objPEPMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }

        #endregion

        #region clone
        public JsonResult SaveCloneEntity(int refId, string entityName, string geomType, string geom)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var resObj = new BLMisc().SaveCloneEntity(refId, entityName, geomType, geom, Convert.ToInt32(Session["user_id"]));
                if (resObj.status)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_005, entityName);
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = BLConvertMLanguage.MultilingualMessageConvert(resObj.message); //resObj.message;
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_299;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveBulkCloneEntity(ViewCloneDependent objCloneDependen)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();

            if (!(string.IsNullOrEmpty(objCloneDependen.geom)))
            {
                objCloneDependen.geom = objCloneDependen.geom.Trim(',');
            }
            var resObj = new BLMisc().SaveBulkCloneEntity(objCloneDependen, Convert.ToInt32(Session["user_id"]));
            var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objCloneDependen.entityName.ToUpper()).FirstOrDefault().layer_title;
            if (resObj.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_FRM_181, layer_title);
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(resObj.message);//resObj.message;
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCloneDependent(string entityType, int systemId, string geomType, string Geom, string networkId)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            ViewCloneDependent objViewCloneDependent = new ViewCloneDependent();

            //Session["viewFaq"] = objFaqFilter;
            objViewCloneDependent.lstCloneDependent = new BLMisc().GetCloneDependent(entityType, systemId);
            objViewCloneDependent.refId = systemId;
            objViewCloneDependent.entityName = entityType;
            objViewCloneDependent.geom = Geom;
            objViewCloneDependent.geomType = geomType;
            objViewCloneDependent.networkid = networkId;
            var childEntity = objViewCloneDependent.lstCloneDependent.Where(m => m.is_child_entity == true).OrderBy(m => m.layer_title).ToList();
            var childAssociated = objViewCloneDependent.lstCloneDependent.Where(m => m.is_associated_entity == true).OrderBy(m => m.layer_title).ToList();
            var ParentAssociated = new BLAccessories().GetAccessories(systemId, entityType);
            if (ParentAssociated != null)
                objViewCloneDependent.is_accessories = ParentAssociated.system_id > 0 ? true : false;
            objViewCloneDependent.childEntityCount = childEntity != null && childEntity.Count > 0 ? childEntity.Count : 0;
            objViewCloneDependent.associatedEntityCount = childAssociated != null && childAssociated.Count > 0 ? childAssociated.Count : 0;
            return View("_ViewCloneDependent", objViewCloneDependent);
        }

        public ActionResult GetGroupLibrary(string entityType, int systemId, string geomType, string Geom, string networkId)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            ViewGroupLibrary objViewGroupLibrary = new ViewGroupLibrary();
            objViewGroupLibrary.lstCloneDependent = new BLMisc().GetCloneDependent(entityType, systemId);
            objViewGroupLibrary.system_id = systemId;
            objViewGroupLibrary.entity_type = entityType;
            var childEntity = objViewGroupLibrary.lstCloneDependent.Where(m => m.is_child_entity == true).OrderBy(m => m.layer_title).ToList();
            var childAssociated = objViewGroupLibrary.lstCloneDependent.Where(m => m.is_associated_entity == true).OrderBy(m => m.layer_title).ToList();
            var ParentAssociated = new BLAccessories().GetAccessories(systemId, entityType);
            if (ParentAssociated != null)
                objViewGroupLibrary.is_accessories_required = ParentAssociated.system_id > 0 ? true : false;
            objViewGroupLibrary.associatedEntityCount = childAssociated != null && childAssociated.Count > 0 ? childAssociated.Count : 0;
            objViewGroupLibrary.childEntityCount = childEntity != null && childEntity.Count > 0 ? childEntity.Count : 0;
            objViewGroupLibrary.geomType = geomType != null ? geomType : "";
            return View("_ViewGroupLibrary", objViewGroupLibrary);
        }
        public JsonResult SaveGroupLibrary(ViewGroupLibrary objGroupLibrary)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            var objLgnUsrDtl = (User)Session["userDetail"];
            objGroupLibrary.created_by = objLgnUsrDtl.user_id;
            objGroupLibrary.created_on = DateTime.Now;
            var parent_id = new BLMisc().SaveGroupLibrary(objGroupLibrary);
            //var entity_type = resObj.entity_type;
            foreach (var item in objGroupLibrary.lstCloneDependent)
            {
                if (item.is_include_in_clone == true)
                {
                    objGroupLibrary.system_id = item.system_id;
                    objGroupLibrary.entity_type = item.entity_type;
                    objGroupLibrary.parent_id = parent_id;
                    objGroupLibrary.is_accessories_required = item.is_accessories_placed;
                    objGroupLibrary.is_associated_entity = item.is_associated_entity;
                    objGroupLibrary.is_child_entity = item.is_include_in_clone;
                    objGroupLibrary.name = "";
                    objGroupLibrary.description = "";
                    objGroupLibrary.created_by = objLgnUsrDtl.user_id;
                    objGroupLibrary.created_on = DateTime.Now;
                    new BLMisc().SaveGroupLibrary(objGroupLibrary);
                }
            }
            //var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == entity_type.ToUpper()).FirstOrDefault().layer_title;
            if (parent_id > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Group library saved successfully.";

            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Error while creating Group Library!";//resObj.message;
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGroupLibraryList()
        {
            var user_id = Convert.ToInt32(Session["user_id"]);
            var resObj = new BLMisc().GetGroupLibrary(user_id);
            return Json(new { Data = resObj, JsonRequestBehavior.AllowGet });
        }

        public ActionResult GetGroupLibraryByid(int id)
        {
            List<GroupLibraryDetails> lstGroupLibraryDetails = new List<GroupLibraryDetails>();

            var user_id = Convert.ToInt32(Session["user_id"]);
            lstGroupLibraryDetails = new BLMisc().GetGroupLibraryByid(id, user_id);


            return View("_ViewGroupEntities", lstGroupLibraryDetails);
        }
        public JsonResult SaveGroupLibraryEntity(Root groupLibrary)
        {

            groupLibrary.user_id = Convert.ToInt32(Session["user_id"]);
            groupLibrary.type = "FeatureCollection";
            string url = "api/Library/SaveGroupLibraryEntitys";
            var response = WebAPIRequest.PostIntegrationAPIRequest<List<GroupLibrary>>(url, groupLibrary);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteGroupLayer(int groupLayerID)
        {


            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {

                var isAllow = new BLMisc().DeleteGroupLibraryById(groupLayerID);
                if (isAllow == "DELETE")
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Group Library Deleted successfully! ";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting Group Library!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Something went wrong while deleting Group Library!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GroupLibraryTP()
        {
            return PartialView("_GroupLibraryTP");
        }
        //public ActionResult SaveLineGroupLibrary(List<GLLineDetails> lstGroupLineTP, List<TerminationPoint> lstGLTerminationPoint)
        //{
        //    DbMessage objpm = new DbMessage();
        //    var user_id = Convert.ToInt32(Session["user_id"]);
        //    objpm = new BLMisc().SaveCablegroupLibrary(lstGroupLineTP, lstGLTerminationPoint, user_id);

        //    //foreach (var item in lstGroupLineTP1)
        //    //{
        //    //    //GroupLibraryDetails objGroupLibraryDetails = new GroupLibraryDetails();

        //    //    var user_id = Convert.ToInt32(Session["user_id"]);
        //    //  var layergroup= new BLMisc().GetLineGroupLibraryByid(item.glid);

        //    //    JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        //    //   var entity_date =(dynamic)json_serializer.DeserializeObject(layergroup.entity_data);

        //    //    if (layergroup.entity_type == EntityType.Cable.ToString())
        //    //    {
        //    //        objpm = new BLMisc().SaveCablegroupLibrary(item.geom, item.glid, user_id, item.lstGLTerminationPoint, layergroup.entity_type);
        //    //    }
        //    //    else if (layergroup.entity_type == EntityType.Duct.ToString())
        //    //    {
        //    //        objpm = new BLMisc().SaveDuctgroupLibrary(item.geom, item.glid, user_id, item.lstGLTerminationPoint, entity_date, layergroup.entity_type);

        //    //    }
        //    //    else if (layergroup.entity_type == EntityType.Trench.ToString())
        //    //    {
        //    //        objpm = new BLMisc().SaveTrenchgroupLibrary(item.geom, item.glid, user_id, item.lstGLTerminationPoint, entity_date, layergroup.entity_type);

        //    //    }
        //    //}

        //    return Json(objpm, JsonRequestBehavior.AllowGet);


        //}
        #endregion


        public ActionResult GetVizButterflyNetwork(string key)
        {
            // Currently running for manhole only further modification reqired in encryption for other entities
            string[] _value = key.Split(',');
            var value = MiscHelper.Decrypt(_value[0]);
            VizButterFlyNetwork objVizNetwork = new VizButterFlyNetwork();
            if (_value[1] == EntityType.Manhole.ToString())
                objVizNetwork = new BLOSPSplicing().GetVizButterflyNetwork(Convert.ToInt32(value), EntityType.Manhole.ToString());
            if (_value[1] == EntityType.Handhole.ToString())
                objVizNetwork = new BLOSPSplicing().GetVizButterflyNetwork(Convert.ToInt32(value), EntityType.Handhole.ToString());
            if (!string.IsNullOrEmpty(objVizNetwork.legends))
            {
                objVizNetwork.lstlegend = JsonConvert.DeserializeObject<List<legend>>(objVizNetwork.legends);
            }
            if (!string.IsNullOrEmpty(objVizNetwork.checkbox))
            {
                objVizNetwork.lstChekbox = JsonConvert.DeserializeObject<List<checkbox>>(objVizNetwork.checkbox);
            }
            return PartialView("_ButterFlyNetworkDiagram", objVizNetwork);
        }
        public ActionResult GetSplicingNetworkDiagram(string key)
        {
            // Currently running for manhole only further modification reqired in encryption for other entities
            string[] _value = key.Split(',');
            var value = MiscHelper.Decrypt(_value[0]);
            VizButterFlyNetwork objVizNetwork = new VizButterFlyNetwork();
            
            if (_value[1] == EntityType.SpliceClosure.ToString())
                objVizNetwork = new BLOSPSplicing().GetSplicingNetworkDiagram(Convert.ToInt32(value), EntityType.SpliceClosure.ToString());
            if (!string.IsNullOrEmpty(objVizNetwork.legends))
            {
                objVizNetwork.lstlegend = JsonConvert.DeserializeObject<List<legend>>(objVizNetwork.legends);
            }
            if (!string.IsNullOrEmpty(objVizNetwork.checkbox))
            {
                objVizNetwork.lstChekbox = JsonConvert.DeserializeObject<List<checkbox>>(objVizNetwork.checkbox);
            }
            return PartialView("_SplicingNetworkDiagram", objVizNetwork);
        }
        public ActionResult GetSLDDiagram(string key)
        {
            string[] value = key.Split(',');
            var pEntityId = MiscHelper.Decrypt(value[0]);
            var pEntityType = MiscHelper.Decrypt(value[1]);
            var pSLDType = MiscHelper.Decrypt(value[2]);
            SLDModel obj = new SLDModel();
            obj = new BLOSPSplicing().GetSLDDiagram(Convert.ToInt32(pEntityId), pEntityType, pSLDType);
            if (!string.IsNullOrEmpty(obj.legends))
            {
                obj.lstlegend = JsonConvert.DeserializeObject<List<legend>>(obj.legends);
            }
            if (!string.IsNullOrEmpty(obj.cables))
            {
                obj.lstCableLegend = JsonConvert.DeserializeObject<List<CableLegend>>(obj.cables);
            }
            obj.title = pSLDType;
            return PartialView("_SLDdiagram", obj);
        }

        public PartialViewResult GetShaftByBld(int structureId)
        {
            List<StructureShaftInfo> objShaftLst = new List<StructureShaftInfo>();
            if (structureId != 0)
            {
                objShaftLst = BLShaft.Instance.GetShaftByBld(structureId);
            }
            return PartialView("_AddShaftInfo", objShaftLst);
        }

        public JsonResult DeleteShaftById(int shaftId)
        {


            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {

                var isAllow = BLIspEntityMapping.Instance.IsShaftAssociated(shaftId);
                if (!isAllow)
                {
                    var objShaft = BLShaft.Instance.DeleteShaftById(shaftId, Convert.ToInt32(Session["user_id"]));
                    if (objShaft == 1)
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                    }
                    else
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_300;
                    }
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_301;
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #region Bulk Project Specification Process View Binding

        public ActionResult ViewProjectSpecific(string ntkStatus, string entity_sub_type)
        {
            ProjectSpecificView objProj = new ProjectSpecificView();
            objProj.network_status = ntkStatus;
            objProj.entity_sub_type = entity_sub_type;
            fillProjectSpecifications(objProj);
            //return PartialView("~/Views/Library/_BulkProjectSpecific.cshtml", objProj);
            return PartialView("_BulkProjectSpecific", objProj);
        }

        //public ActionResult ViewProjectSpecific(BulkProjSpecific objProjSpec)
        //{
        //    ProjectSpecificView objProj = new ProjectSpecificView();
        //    objProj.network_status = objProjSpec.ntk_type;
        //    objProj.planning_id = objProjSpec.planning_id;
        //    objProj.workorder_id = objProjSpec.workorder_id;
        //    objProj.project_id = objProjSpec.project_id;
        //    objProj.purpose_id = objProjSpec.purpose_id;
        //    fillProjectSpecifications(objProj);
        //    //return PartialView("~/Views/Library/_BulkProjectSpecific.cshtml", objProj);
        //    return PartialView("_BulkProjectSpecific", objProj);
        //}
        #endregion


        #region Project Specification Filter Process
        public ActionResult ViewProjectSpecificFilter(ProjectSpecificView objProj)
        // public ActionResult ViewProjectSpecificFilter(string ntkStatus, int project_id, int planning_id, int workorderid, int purpose_id)
        {

            //ProjectSpecificView objProj = new ProjectSpecificView();
            //objProj.network_status = ntkStatus;
            //objProj.project_id = project_id;
            //objProj.planning_id = planning_id;
            //objProj.workorder_id = workorderid;
            //objProj.purpose_id = purpose_id;
            // objProj.ownership_type = "Own";
            objProj.network_status = objProj.network_status == null ? "" : objProj.network_status;

            fillProjectSpecifications(objProj);
            // Fill Building RFS  drop down
            var objDDL = new BLMisc().GetDropDownList(EntityType.Building.ToString());
            // CheckboxMaster chk = new CheckboxMaster();
            List<CheckboxMaster> chkRFS = objDDL.Where(x => x.dropdown_type == DropDownType.RFS_Status.ToString())
                                                 .Select(x => new CheckboxMaster
                                                 {
                                                     Value = x.dropdown_value,
                                                     Text = x.dropdown_key,
                                                     IsChecked = false
                                                 }).ToList();

            objProj.lstBuildingRFS = chkRFS;

            //Bind the cable dropdown lists
            fillCableDetails(objProj);
            BindFaultDetails(objProj);
            BindFilterSplitterTypeDetails(objProj);
            BindOwnershipDetails(objProj);
            BindSectorDropdown(objProj);
            BindPODAssociationDetail(objProj);

            BLLayer objBLLayer = new BLLayer();
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;
            objProj.lstUserModule = objBLLayer.GetUserModuleAbbrList(usrId, UserType.Web.ToString());
            // objProj.lstBuildingRFS = objDDL.Where(x => x.dropdown_type == DropDownType.RFS_Status.ToString()).ToList();

            return PartialView("_ProjectSpeciFilter", objProj);
        }
        #endregion

        private void BindSectorDropdown(ProjectSpecificView objProj)
        {
            BLMisc objBLMisc = new BLMisc();
            var objDDL = objBLMisc.GetDropDownList(EntityType.Sector.ToString());
            objProj.lstSectorType = objDDL.Where(x => x.dropdown_type == DropDownType.SectorType.ToString()).ToList();
            objProj.lstFrequencyType = objDDL.Where(x => x.dropdown_type == DropDownType.Frequency.ToString()).ToList();
        }
        private void BindPODAssociationDetail(ProjectSpecificView objProj)
        {
            objProj.lstPODAssociation = new BLPOD().GetPODDetailForFilter();
        }

        public JsonResult GetShaftNFloorByBld(int structureId, string entityType, int floorId)
        {
            var UnitList = new List<StructureElement>();
            var ShaftFloorList = BLISP.Instance.getShaftNFloor(structureId);
            //var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            //if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == entityType).FirstOrDefault() != null)
            //{
            //    UnitList = BLISP.Instance.getAllParentInFloor(structureId, floorId, EntityType.UNIT.ToString());
            //}

            return Json(new { ShaftList = ShaftFloorList.ShaftList, FloorList = ShaftFloorList.FloorList, UnitList = UnitList }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetFloorByBld(int structureId)
        {
            List<StructureFloorInfo> objFloorLst = new List<StructureFloorInfo>();
            if (structureId != 0)
            {
                objFloorLst = BLFloor.Instance.GetFloorByBld(structureId);
            }
            return PartialView("_AddFloorInfo", objFloorLst);
        }

        public JsonResult DeleteFloorById(int floorId)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {


                var isAllow = BLIspEntityMapping.Instance.IsFloorAssociated(floorId);
                if (!isAllow)
                {
                    var objFloor = BLFloor.Instance.DeleteFloorById(floorId, Convert.ToInt32(Session["user_id"]));
                    if (objFloor == 1)
                    {
                        objResp.message = Resources.Resources.SI_OSP_GBL_NET_RPT_118;
                        objResp.status = ResponseStatus.OK.ToString();
                    }
                    else
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_302;
                    }
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_303;
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckEntityAssociation(string shaftId, string entityType, int structureId)
        {
            //var isAssociated = BLStructure.Instance.CheckEntityAssociation(shaftId, entityType, structureId);
            // return Json(isAssociated, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddParallelCable(int systemId)
        {
            //getCableGeom
            return PartialView("_AddParallelCable");
        }
        public JsonResult getParallelPolyLineCurve(int systemId, double offset)
        {
            var geom = new BLCable().getCableGeom(systemId, offset);
            return Json(geom, JsonRequestBehavior.AllowGet);
        }
        //public PartialViewResult entityAssociation(int systemId, string entityType, string pgeomType)
        //{
        //    AssociateEntity objAssEntity = new AssociateEntity();
        //    var obj = new BLMisc().getAssociateEntity(systemId, entityType);
        //    if (obj != null && entityType != EntityType.Cable.ToString() && entityType != EntityType.Duct.ToString() && entityType != EntityType.Trench.ToString())
        //    {
        //        objAssEntity.listBufferEntity = new BLMisc().getEntityInBuffer(obj.associated_system_id, obj.associated_entity_type, obj.entity_type, pgeomType);
        //        objAssEntity.entity_system_id = obj.entity_system_id;
        //        objAssEntity.entity_type = obj.entity_type;
        //        objAssEntity.entity_network_id = obj.entity_network_id;
        //        objAssEntity.system_id = obj.system_id;
        //    }
        //    return PartialView("_EntityAssociation", objAssEntity);
        //}
        public JsonResult SaveAssociation(AssociateEntity objAssociateEnt)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            objAssociateEnt.created_by = Convert.ToInt32(Session["user_id"]);
            objAssociateEnt.created_on = DateTimeHelper.Now;
            var obj = new BLMisc().SaveAssociation(objAssociateEnt);
            if (objAssociateEnt.system_id > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_304, objAssociateEnt.associated_entity_type); //objAssociateEnt.associated_entity_type + " " + Resources.Resources.SI_OSP_GBL_NET_FRM_304;
            }
            else
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_305, objAssociateEnt.associated_entity_type);//objAssociateEnt.associated_entity_type + " " + Resources.Resources.SI_OSP_GBL_NET_FRM_305;
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BulkAssosiation(int systemId, string entityType, string networkId, string geom)
        {
            //JsonResponse<string> objResp = new JsonResponse<string>();
            AssociateLineEntity objAssociateEnt = new AssociateLineEntity();
            objAssociateEnt.userId = Convert.ToInt32(Session["user_id"]);
            objAssociateEnt.parent_geom = geom;
            objAssociateEnt.parent_system_id = systemId;
            objAssociateEnt.parent_entity_type = entityType;
            string url1 = "api/main/AutoAssosiation";
            var response = WebAPIRequest.PostIntegrationAPIRequest<AssociateLineEntity>(url1, objAssociateEnt, "", "");
            return Json(response.results, JsonRequestBehavior.AllowGet);
        }
        public JsonResult validateBulkAssosiation(int systemId, string entityType)
        {
            int userId = Convert.ToInt32(Session["user_id"]);
            var obj = new BLMisc().validateBulkAssosiation(systemId, entityType, userId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RefreshMessage(int systemId)
        {
            var associationStatus = new BLMisc().BulkAssociationRequestLog(systemId);
            return Json(associationStatus, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult getLineEntityAssociation(AssociateEntityRequest objLineAssociate)
        {
            //AssociateLineEntity objLineAssociate = new AssociateLineEntity();
            //List<LineEntityInfo> objLineEntity = new List<LineEntityInfo>();
            //objLineAssociate.parent_system_id = systemId;
            //objLineAssociate.parent_entity_type = entityType;
            //objLineAssociate.parent_network_id = networkId;
            //var layerDetails = new BLLayer().getLayer(entityType);
            //objLineAssociate.parent_multi_association = layerDetails.is_multi_association;
            ////if (entityType == EntityType.Duct.ToString())
            ////{
            ////    objLineAssociate.listLineEntityInfo = new BLMisc().getLineEntityInLineBuffer(0, EntityType.Cable.ToString(), systemId, entityType);
            ////}
            ////else if (entityType == EntityType.Trench.ToString())
            ////{
            ////    objLineAssociate.listLineEntityInfo = new BLMisc().getLineEntityInLineBuffer(0, EntityType.Duct.ToString(), systemId, entityType);
            ////}
            ////else if (entityType == EntityType.Cable.ToString())
            ////{

            ////    objLineAssociate.listLineEntityInfo = new BLMisc().getLineEntityInLineBuffer(0, EntityType.Cable.ToString(), systemId, entityType);
            ////}
            //objLineAssociate.listLineEntityInfo = new BLMisc().getLineEntityInLineBuffer(0, EntityType.Cable.ToString(), systemId, entityType);
            //var buried = new BLMisc().checkIsBuried(systemId, entityType);
            //if (buried != null)
            //{
            //    objLineAssociate.parent_is_buried = buried.status;
            //}
            //return PartialView("_LineEntityAssociation", objLineAssociate);


            string url = "api/main/GetEntityAssociation";
            var response = WebAPIRequest.PostIntegrationAPIRequest<AssociateLineEntity>(url, objLineAssociate, "", "");
            return PartialView("_LineEntityAssociation", response.results);
        }
        public PartialViewResult getRouteAssociation(AssociateEntityRequest objLineAssociate)
        {
            string url = "api/main/getRouteAssociation";
            var response = WebAPIRequest.PostIntegrationAPIRequest<AssociateRoute>(url, objLineAssociate, "", "");
            return PartialView("_RouteAssociation", response.results);
        }
        public PartialViewResult viewEntityAssociation(AssociateEntityRequest objLineAssociate)
        {
            //AssociateLineEntity objLineAssociate = new AssociateLineEntity();
            //List<LineEntityInfo> objLineEntity = new List<LineEntityInfo>();
            //objLineAssociate.parent_system_id = systemId;
            //objLineAssociate.parent_entity_type = entityType;
            //objLineAssociate.parent_network_id = networkId;
            //objLineAssociate.listLineEntityInfo = new BLMisc().viewEntityAssociation(systemId, entityType);
            //return PartialView("_viewEntityAssociation", objLineAssociate);

            string url = "api/main/ViewOtherEntityAssociation";
            var response = WebAPIRequest.PostIntegrationAPIRequest<AssociateLineEntity>(url, objLineAssociate, "", "");
            return PartialView("_viewEntityAssociation", response.results);
        }
        public PartialViewResult SaveLineEntityAssociate(AssociateLineEntity objLineEntity)
        {

            objLineEntity.manhole_count = objLineEntity.listLineEntityInfo.Where(m => m.entity_type == EntityType.Manhole.ToString() && m.is_associated == true).Count();

            //PageMessage objPM = new PageMessage();
            //var response = new BLMisc().saveLineEntityAssocition(JsonConvert.SerializeObject(objLineEntity.listLineEntityInfo), objLineEntity.parent_system_id, objLineEntity.parent_entity_type, Convert.ToInt32(Session["user_id"]));
            //objPM.status = ResponseStatus.OK.ToString();
            //objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_169;
            //objLineEntity.pageMsg = objPM;
            //return PartialView("_LineEntityAssociation", objLineEntity);
            objLineEntity.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/main/SaveEntityAssociate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<AssociateLineEntity>(url, objLineEntity, "", "");
            return PartialView("_LineEntityAssociation", response.results);
        }
        public PartialViewResult SaveRouteAssociate(AssociateRoute objRoute)
        {
            objRoute.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/main/SaveRouteAssociate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<AssociateRoute>(url, objRoute, "", "");
            return PartialView("_RouteAssociation", response.results);
        }
        public JsonResult getEntityInBuffer(int systemId, string entityType, string pEntityType, string pgeomType)
        {
            var bufferEntity = new BLMisc().getEntityInBuffer(systemId, entityType, pEntityType, pgeomType);
            return Json(bufferEntity, JsonRequestBehavior.AllowGet);
        }
        public void ExportAssociation(int systemId, string entityType, string networkId)
        {
            var listLineEntityInfo = new BLMisc().GetAssociationExportData<Dictionary<string, string>>(systemId, entityType);
            listLineEntityInfo = BLConvertMLanguage.ExportMultilingualConvert(listLineEntityInfo);
            DataTable dtlogs = Utility.MiscHelper.GetDataTableFromDictionaries(listLineEntityInfo);
            dtlogs.Columns["entity_network_id"].ColumnName = "Entity Network ID";
            dtlogs.Columns["is_termination_point"].ColumnName = "Is Termination Point";
            ExportData(dtlogs, "ExportAssociationReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        }

        public void ExportRouteAssociation(int systemId, string entityType, string networkId)
        {
            var listRouteInfo = new BLMisc().GetRouteAssociationExportData<Dictionary<string, string>>(systemId, entityType);
            listRouteInfo = BLConvertMLanguage.ExportMultilingualConvert(listRouteInfo);
            DataTable dtlogs = Utility.MiscHelper.GetDataTableFromDictionaries(listRouteInfo);
            dtlogs.Columns.Remove("cable_id");
            dtlogs.Columns.Remove("entity_type");
            dtlogs.Columns.Remove("entity_id");

            dtlogs.Columns["entity_network_id"].ColumnName = "Cable_Id";
            //dtlogs.Columns["old_column_name2"].ColumnName = "new_column_name2";
            ExportData(dtlogs, "ExportRouteAssociationReport_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        }

        public void DownloadBulkAssociationLog(int systemId)
        {
            int userId = Convert.ToInt32(Session["user_id"]);
            var bulkAssociationLog = new BLMisc().DownloadBulkAssociationLog<Dictionary<string, string>>(systemId, userId);
            bulkAssociationLog = BLConvertMLanguage.ExportMultilingualConvert(bulkAssociationLog);
            DataTable dtReport = Utility.MiscHelper.GetDataTableFromDictionaries(bulkAssociationLog);
            //dtReport = (DataTable)JsonConvert.DeserializeObject("", (typeof(DataTable)));
            for (int i = 0; i < dtReport.Columns.Count; i++)
            {
                string columnName = dtReport.Columns[i].ColumnName;
                dtReport.Columns[i].ColumnName = ((columnName).Replace('_', ' ')).ToUpper();
            }
            ExportData(dtReport, "BulkAssociationLog" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        }
        #region Loop Mangment

        [HttpPost]
        public JsonResult DeleteLoopDetailById(int Loop_system_id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {


                if (BLLoopMangment.Instance.DeleteLoopDetailById(Loop_system_id) > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_287;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_288;
                }

            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_289;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }



        public PartialViewResult GetLoopMangmentDetail(NELoopDetails obj)
        {
            List<NELoopDetails> lstLoopMangment = BLLoopMangment.Instance.GetLoopDetails(obj.longitude, obj.latitude, obj.associated_system_id, obj.associated_System_Type, obj.structure_id);
            return PartialView("_LoopMangment", lstLoopMangment.OrderByDescending(m => m.loop_length).ToList());
        }

        public PartialViewResult getLoopDetailsForCable(NELoopDetails obj)
        {
            List<NELoopDetails> lstLoopMangment = BLLoopMangment.Instance.GetLoopDetailsForCable(obj.cable_system_id);
            return PartialView("_LoopDetailsForCable", lstLoopMangment);

        }



        #endregion

        #region FDB
        public PartialViewResult AddFDB(string pEntityType, string networkIdType, string geom = "", int systemId = 0, int pSystemId = 0, string pNetworkId = "")
        {
            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    // get geom by parent id...
            //    geom = GetPointTypeParentGeom(pSystemId, pEntityType);
            //}
            //FDBInfo objFDBMaster = getFDBInfo(pSystemId, pEntityType, networkIdType, systemId, geom);
            ////objFDBMaster.parent_system_id = pSystemId;
            ////objFDBMaster.parent_entity_type = pEntityType;

            //BLItemTemplate.Instance.BindItemDropdowns(objFDBMaster, EntityType.FDB.ToString());
            //fillProjectSpecifications(objFDBMaster);
            //if (systemId == 0 && pSystemId > 0)
            //{
            //    objFDBMaster.objIspEntityMap.structure_id = pSystemId;
            //    objFDBMaster.objIspEntityMap.AssociateStructure = pSystemId;
            //}
            //BindFDBDropdown(objFDBMaster);
            //return PartialView("_AddFDB", objFDBMaster);
            FDBInfo obj = new FDBInfo();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FDBInfo>(url, obj, EntityType.FDB.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddFDB", response.results);
        }
        //public ActionResult AddFDB(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0)
        //{
        //    FDBInfo model = getFDBInfo(networkIdType, ModelInfo.templateId, systemId, ModelInfo.structureid);
        //    if (systemId != 0)
        //    {
        //        var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.FDB.ToString());
        //        if (ispMapping != null && ispMapping.id > 0)
        //        {
        //            model.objIspEntityMap.floor_id = ispMapping.floor_id;
        //            model.objIspEntityMap.structure_id = ispMapping.structure_id;
        //            model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
        //            model.objIspEntityMap.AssociateStructure = ispMapping.structure_id;
        //        }
        //    }
        //    else
        //    {
        //        model.objIspEntityMap.floor_id = ModelInfo.floorid;
        //        model.objIspEntityMap.structure_id = ModelInfo.structureid;
        //        model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
        //    }
        //    new MiscHelper().BindPortDetails(model, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
        //    BindFDBDropdown(model);
        //    return PartialView("_AddFDB", model);
        //}
        public FDBInfo getFDBInfo(int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
        {
            FDBInfo objFDB = new FDBInfo();
            objFDB.geom = geom;
            objFDB.networkIdType = networkIdType;
            if (systemId == 0)
            {
                objFDB.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objFDB.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objFDB.ownership_type = "Own";
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objFDB, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objFDB, new NetworkCodeIn() { eType = EntityType.FDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFDB.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType);
                //Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.FDB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objFDB);
            }
            else
            {
                objFDB = new BLMisc().GetEntityDetailById<FDBInfo>(systemId, EntityType.FDB);
            }
            return objFDB;
        }
        public ActionResult SaveFDB(FDBInfo objFDBMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();

            //// get parent geometry 
            //if (string.IsNullOrWhiteSpace(objFDBMaster.geom) && objFDBMaster.system_id == 0)
            //{
            //    objFDBMaster.geom = GetPointTypeParentGeom(objFDBMaster.parent_system_id, objFDBMaster.parent_entity_type);
            //}

            //if (objFDBMaster.networkIdType == NetworkIdType.A.ToString() && objFDBMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn()
            //    {
            //        eType = EntityType.FDB.ToString(),
            //        gType = GeometryType.Point.ToString(),
            //        eGeom = objFDBMaster.geom,
            //        parent_eType = objFDBMaster.parent_entity_type,
            //        parent_sysId = objFDBMaster.parent_system_id
            //    });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objFDBMaster = getFDBInfo(objFDBMaster.parent_system_id, objFDBMaster.parent_entity_type, objFDBMaster.networkIdType, objFDBMaster.system_id, objFDBMaster.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objFDBMaster.fdb_name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objFDBMaster.network_id = objNetworkCodeDetail.network_code;
            //    objFDBMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            //}

            //if (TryValidateModel(objFDBMaster))
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    objFDBMaster.objIspEntityMap.structure_id = Convert.ToInt32(objFDBMaster.objIspEntityMap.AssociateStructure);
            //    objFDBMaster.objIspEntityMap.shaft_id = objFDBMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objFDBMaster.objIspEntityMap.shaft_id;
            //    if (string.IsNullOrEmpty(objFDBMaster.objIspEntityMap.AssoType))
            //    {
            //        objFDBMaster.objIspEntityMap.shaft_id = 0; objFDBMaster.objIspEntityMap.floor_id = 0;
            //    }
            //    var isNew = objFDBMaster.system_id > 0 ? false : true;
            //    if (objFDBMaster.unitValue != null && objFDBMaster.unitValue.Contains(":"))
            //    {
            //        objFDBMaster.no_of_input_port = Convert.ToInt32(objFDBMaster.unitValue.Split(':')[0]);
            //        objFDBMaster.no_of_output_port = Convert.ToInt32(objFDBMaster.unitValue.Split(':')[1]);
            //    }
            //    if (objFDBMaster.objIspEntityMap.structure_id == 0)
            //    {
            //        var objIn = new NetworkCodeIn() { eType = EntityType.FDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFDBMaster.longitude + " " + objFDBMaster.latitude };
            //        var parentDetails = new BLMisc().getParentInfo(objIn);
            //        if (parentDetails != null)
            //        {
            //            objFDBMaster.parent_system_id = parentDetails.p_system_id;
            //            objFDBMaster.parent_network_id = parentDetails.p_network_id;
            //            objFDBMaster.parent_entity_type = parentDetails.p_entity_type;
            //        }

            //    }
            //    objFDBMaster.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            //    var resultItem = BLISP.Instance.SaveFDBDetails(objFDBMaster);
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {


            //        //Save Reference
            //        if (objFDBMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objFDBMaster.EntityReference, resultItem.system_id);
            //        }

            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_005, layer_title);
            //        }
            //        else
            //        {
            //            if (resultItem.isPortConnected)
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
            //            }
            //            else
            //            {
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
            //            }
            //        }
            //        objFDBMaster.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objFDBMaster.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objFDBMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objFDBMaster, EntityType.BDB.ToString());
            //    BindFDBDropdown(objFDBMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA

            //    fillProjectSpecifications(objFDBMaster);
            //    return PartialView("_AddFDB", objFDBMaster);
            objFDBMaster.isDirectSave = isDirectSave;
            objFDBMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objFDBMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objFDBMaster.source_ref_type = "NETWORK_TICKET";
                objFDBMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FDBInfo>(url, objFDBMaster, EntityType.FDB.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddFDB", response.results);

        }
        //public FDBInfo getFDBInfo(string networkIdType, int templateId, int systemId, int structureId)
        //{
        //    FDBInfo objFDB = new FDBInfo();
        //    objFDB.networkIdType = networkIdType;
        //    objFDB.parent_system_id = structureId;
        //    objFDB.parent_entity_type = EntityType.Structure.ToString();
        //    if (systemId != 0)
        //    {
        //        objFDB = BLISP.Instance.getFDBDetails(systemId);
        //    }
        //    else
        //    {
        //        if (networkIdType == NetworkIdType.M.ToString())
        //        {
        //            // for Manual network id type 
        //            var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = objFDB.objIspEntityMap.structure_id });
        //            objFDB.network_id = ISPNetworkCodeDetail.network_code;
        //            objFDB.ownership = "Own";
        //        }
        //        var objItem = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.FDB);
        //        Utility.MiscHelper.CopyMatchingProperties(objItem, objFDB);

        //    }
        //    BLItemTemplate.Instance.BindItemDropdowns(objFDB, EntityType.FDB.ToString());
        //    fillProjectSpecifications(objFDB);
        //    return objFDB;
        //}
        //public ActionResult SaveFDB(FDBInfo model, bool isDirectSave = false)
        //{

        //    ModelState.Clear();
        //    PageMessage objPM = new PageMessage();
        //    //int structure_id = model.objIspEntityMap.structure_id;
        //    //int floor_id = model.objIspEntityMap.floor_id ?? 0;
        //    //int shaft_id = model.objIspEntityMap.shaft_id ?? 0;
        //    model.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
        //    if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
        //    {
        //        //GET AUTO NETWORK CODE...
        //        var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDBMaster.geom, parent_eType = objBDBMaster.pEntityType, parent_sysId = objBDBMaster.pSystemId });
        //        if (isDirectSave == true)
        //        {
        //            //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
        //            model = getFDBInfo(model.parent_system_id, model.parent_entity_type, model.networkIdType, model.system_id, model.geom);
        //            // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
        //            model.fdb_name = objNetworkCodeDetail.network_code;
        //        }
        //        //SET NETWORK CODE
        //        model.network_id = objNetworkCodeDetail.network_code;
        //        model.sequence_id = objNetworkCodeDetail.sequence_id;
        //    }
        //    //if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
        //    //{
        //    //    //GET AUTO NETWORK CODE...
        //    //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.objIspEntityMap.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = model.objIspEntityMap.structure_id });
        //    //    if (isDirectSave == true)
        //    //    {
        //    //        model = getFDBInfo(model.pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "");
        //    //            //getFDBInfo(model.networkIdType, model.templateId, model.system_id, model.objIspEntityMap.structure_id);
        //    //        model.objIspEntityMap.floor_id = floor_id;
        //    //        model.objIspEntityMap.structure_id = structure_id;
        //    //        model.objIspEntityMap.shaft_id = shaft_id;
        //    //        model.fdb_name = objISPNetworkCode.network_code;
        //    //    }
        //    //    model.network_id = objISPNetworkCode.network_code;
        //    //    model.sequence_id = objISPNetworkCode.sequence_id;

        //    //}
        //    var structureDetails = new BLISP().GetStructureById(structure_id);
        //    if (structureDetails != null && structureDetails.Count > 0)
        //    {
        //        model.region_id = structureDetails.First().region_id;
        //        model.province_id = structureDetails.First().province_id;
        //        model.latitude = structureDetails.First().latitude;
        //        model.longitude = structureDetails.First().longitude;
        //    }
        //    if (TryValidateModel(model))
        //    {
        //        var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault().layer_title;
        //        if (model.objIspEntityMap.AssociateStructure != 0)
        //        {
        //            model.objIspEntityMap.structure_id = Convert.ToInt32(model.objIspEntityMap.AssociateStructure);
        //        }
        //        model.objIspEntityMap.shaft_id = model.objIspEntityMap.AssoType == "Floor" ? 0 : model.objIspEntityMap.shaft_id;
        //        if (string.IsNullOrEmpty(model.objIspEntityMap.AssoType))
        //        {
        //            model.objIspEntityMap.shaft_id = 0; model.objIspEntityMap.floor_id = 0;
        //        }
        //        bool isNew = model.system_id == 0 ? true : false;
        //        if (model.unitValue != null && model.unitValue.Contains(":"))
        //        {
        //            model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
        //            model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
        //        }
        //        var resultItem = BLISP.Instance.SaveFDBDetails(model);
        //        if (string.IsNullOrEmpty(resultItem.objPM.message))
        //        {
        //            //Save Reference
        //            if (model.EntityReference != null && resultItem.system_id > 0)
        //            {
        //                SaveReference(model.EntityReference, resultItem.system_id);
        //            }
        //            if (isNew)
        //            {
        //                objPM.status = ResponseStatus.OK.ToString(); ;
        //                objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_005, layer_title);
        //            }
        //            else
        //            {
        //                if (resultItem.isPortConnected == true)
        //                {
        //                    objPM.status = ResponseStatus.OK.ToString();
        //                    objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
        //                }
        //                else
        //                {
        //                    objPM.status = ResponseStatus.OK.ToString();
        //                    objPM.message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
        //                }
        //            }
        //            model.objPM = objPM;
        //        }
        //    }
        //    else
        //    {
        //        objPM.status = ResponseStatus.FAILED.ToString();
        //        objPM.message = getFirstErrorFromModelState();
        //        model.objPM = objPM;
        //    }
        //    model.entityType = EntityType.FDB.ToString();
        //    BindFDBDropdown(model);
        //    return PartialView("_AddFDB", model);
        //}
        private void BindFDBDropdown(FDBInfo objFDB)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objFDB.parent_system_id, objFDB.system_id, EntityType.FDB.ToString());
            if (ispEntityMap != null && ispEntityMap.id > 0)
            {
                objFDB.objIspEntityMap.id = ispEntityMap.id;
                objFDB.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objFDB.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objFDB.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objFDB.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }
            objFDB.objIspEntityMap.AssoType = objFDB.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objFDB.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objFDB.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objFDB.longitude + " " + objFDB.latitude);
            if (objFDB.objIspEntityMap.structure_id > 0)
            {
                var shaftFloorList = new BLBDB().GetShaftFloorByStrucId(objFDB.objIspEntityMap.structure_id);
                objFDB.objIspEntityMap.lstShaft = shaftFloorList.Where(m => m.isshaft == true).ToList();
                objFDB.objIspEntityMap.lstFloor = shaftFloorList.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

                if (objFDB.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objFDB.objIspEntityMap.unitId = objFDB.parent_system_id;
                    //objONT.objIspEntityMap.AssoType = "";
                    //objONT.objIspEntityMap.floor_id = 0;
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.FDB.ToString()).FirstOrDefault() != null)
            {
                objFDB.objIspEntityMap.isValidParent = true;
                objFDB.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objFDB.objIspEntityMap.structure_id, objFDB.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objFDB.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objFDB.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            objFDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objFDB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
            new BLMisc().BindPortDetails(objFDB, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
            new BLMisc().BindPortDetails(objFDB, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
        }
        #endregion

        #region Legend
        public PartialViewResult getLegnedDetail()
        {
            var usrDetail = (User)Session["userDetail"];
            Legend objLegend = new Legend();
            objLegend.legendList = new BLMisc().GetLegendDetail(usrDetail.user_id, usrDetail.role_id);
            return PartialView("_Legend", objLegend);
        }
        #endregion

        public JsonResult getParentStructure(int systemId, string entityType)
        {
            var parentStructure = BLStructure.Instance.getParentStructure(systemId, entityType);
            return Json(parentStructure, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getOSPTerminationPoints(string entityType)
        {
            var lstTPPoints = new BLTerminationPointsMaster().getOSPTerminationPoints(entityType);
            return Json(lstTPPoints, JsonRequestBehavior.AllowGet);
        }

        #region ProjectArea
        public PartialViewResult AddProjectArea(string networkIdType, int systemId = 0, string geom = "")
        {
            ProjectArea objPArea = GetProjectAreaDetail(networkIdType, systemId, geom);

            return PartialView("_AddProjectArea", objPArea);
        }

        public ProjectArea GetProjectAreaDetail(string networkIdType, int systemId = 0, string geom = "")
        {
            ProjectArea ObjPArea = new ProjectArea();
            ObjPArea.geom = geom;
            ObjPArea.networkIdType = networkIdType;

            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(ObjPArea, GeometryType.Polygon.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(ObjPArea, new NetworkCodeIn() { eType = EntityType.ProjectArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = ObjPArea.geom }, networkIdType);
            }
            else
            {
                // Get entity detail by Id...
                ObjPArea = new BLMisc().GetEntityDetailById<ProjectArea>(systemId, EntityType.ProjectArea);
            }

            //ObjPArea.lstAreaRFS = ObjPArea.Where(x => x.dropdown_type == DropDownType.Area_RFS.ToString()).ToList();
            return ObjPArea;
        }

        public ActionResult SaveProjectArea(ProjectArea objPArea, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            // var objDDL = new BLMisc().GetDropDownList(EntityType.Area.ToString());
            // DropDownMaster drp = new DropDownMaster();
            //  objArea.lstAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.Area_RFS.ToString()).ToList();
            if (objPArea.networkIdType == NetworkIdType.A.ToString() && objPArea.system_id == 0)
            {
                //GET AUTO NETWORK CODE...
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ProjectArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objPArea.geom });
                if (isDirectSave == true)
                {
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    objPArea = GetProjectAreaDetail(objPArea.networkIdType, objPArea.system_id, objPArea.geom);
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    objPArea.projectarea_name = objNetworkCodeDetail.network_code;
                }
                //SET NETWORK CODE
                objPArea.network_id = objNetworkCodeDetail.network_code;
                objPArea.sequence_id = objNetworkCodeDetail.sequence_id;
            }

            if (TryValidateModel(objPArea))
            {
                var isNew = objPArea.system_id > 0 ? false : true;
                var resultItem = new BLProjectArea().SaveProjectArea(objPArea, Convert.ToInt32(Session["user_id"]));

                if (string.IsNullOrEmpty(resultItem.objPM.message))
                {


                    if (isNew)
                    {
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.isNewEntity = isNew;
                        objPM.message = Resources.Resources.SI_OSP_PA_NET_FRM_003;
                    }
                    else
                    {
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.message = Resources.Resources.SI_OSP_PA_NET_FRM_004;
                    }
                    objPArea.objPM = objPM;
                }
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = getFirstErrorFromModelState();
                objPArea.objPM = objPM;
            }
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(objPArea.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // RETURN PARTIAL VIEW WITH MODEL DATA
                return PartialView("_AddProjectArea", objPArea);
            }

        }


        #endregion

        #region ROW
        public PartialViewResult AddROW(LineEntityIn objIn, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            ROWMaster objROW = new ROWMaster();
            objROW = GetROWDetail(objIn.networkIdType, objIn.systemId, objIn.geom, objIn.centerLineGeom, objIn.buffer_width);
            // BLItemTemplate.Instance.BindItemDropdowns(objROW, EntityType.ROW.ToString());
            // BindROWDropDown(objROW);
            fillProjectSpecifications(objROW);
            return PartialView("_AddROW", objROW);
        }
        public ActionResult SaveRow(ROWMaster objRow, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            if (objRow.networkIdType == NetworkIdType.A.ToString() && objRow.system_id == 0)
            {
                ROWItemMaster objROWItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ROWItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.ROW, "");
                //GET AUTO NETWORK CODE...
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ROW.ToString(), gType = objROWItemMaster.type.ToString(), eGeom = objRow.geom });
                if (isDirectSave == true)
                {
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    objRow = GetROWDetail(objRow.networkIdType, objRow.system_id, objRow.geom, objRow.centerLineGeom);
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    objRow.row_name = objNetworkCodeDetail.network_code;
                }
                //SET NETWORK CODE
                objRow.network_id = objNetworkCodeDetail.network_code;
                objRow.sequence_id = objNetworkCodeDetail.sequence_id;
                objRow.row_name = objNetworkCodeDetail.network_code;
            }

            if (TryValidateModel(objRow))
            {
                var isNew = objRow.system_id > 0 ? false : true;
                var resultItem = new BLROW().SaveROW(objRow, Convert.ToInt32(Session["user_id"]));

                if (isNew)
                {
                    objPM.status = ResponseStatus.OK.ToString(); ;
                    objPM.message = string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_096);
                    objRow.objPM = objPM;
                }
                else
                {
                    if (string.IsNullOrEmpty(resultItem.objPM.message))
                    {
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.message = string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_097);
                        objRow.objPM = objPM;
                    }
                    else { objRow.objPM = resultItem.objPM; }

                }



            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = getFirstErrorFromModelState();
                objRow.objPM = objPM;
            }
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(objRow.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // RETURN PARTIAL VIEW WITH MODEL DATA
                return PartialView("_AddROW", objRow);
            }

        }
        public ROWMaster GetROWDetail(string networkIdType, int systemId, string geom = "", string centerLineGeom = "", decimal bufferWidth = 0)
        {
            ROWMaster objROW = new ROWMaster();
            var userdetails = (User)Session["userDetail"];
            objROW.geom = geom;
            objROW.networkIdType = networkIdType;
            objROW.centerLineGeom = centerLineGeom;
            objROW.buffer_width = bufferWidth;
            //objROW.isPITApplied = BLROW.Instance.isPITApplied(systemId);

            var objDDL = new BLMisc().GetDropDownList(EntityType.ROW.ToString());
            ROWItemMaster objROWItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ROWItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.ROW, "");
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objROW, objROWItemMaster.type.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objROW, new NetworkCodeIn() { eType = EntityType.ROW.ToString(), gType = objROWItemMaster.type.ToString(), eGeom = objROW.geom }, networkIdType);
                objROW.geom_type = objROWItemMaster.type;
            }
            else
            {
                // Get entity detail by Id...
                objROW = new BLMisc().GetEntityDetailById<ROWMaster>(systemId, EntityType.ROW);
            }
            objROW.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objROW;
        }
        public PartialViewResult ApplyStage(int rowSystemId, string geomType, string rowType)
        {
            rowApplyStage objApplyRow = new rowApplyStage();
            objApplyRow = BLROW.Instance.getROWApplyDetails(rowSystemId);
            objApplyRow.rowType = rowType;
            objApplyRow.row_system_id = rowSystemId;
            objApplyRow.listAttachmentType = new BLMisc().getAttachment(EntityType.ROW.ToString(), ROWStage.Apply.ToString());
            if (!string.IsNullOrEmpty(objApplyRow.apply_date.ToString()))
                objApplyRow.applyDate = Utility.MiscHelper.FormatDate(objApplyRow.apply_date.ToString());

            var areaLength = BLROW.Instance.GetAreaLength(rowSystemId, EntityType.ROW.ToString());
            if (areaLength != null)
            {
                objApplyRow.row_area = areaLength.pit_area;
                objApplyRow.applied_pit = areaLength.total_pit;
                objApplyRow.route_length = areaLength.row_length;
            }
            var lstDocument = new BLROW().getROWAttachment(rowSystemId, "Apply", "Document");
            //converting file size
            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            objApplyRow.fileList = lstDocument;
            objApplyRow.remarksList = new BLROW().getROWRemarks(rowSystemId, "Apply");
            if (objApplyRow.system_id == 0) { objApplyRow.applyDate = null; }
            objApplyRow.listRowAuthority = BLROW.Instance.getAuthorityList();
            objApplyRow.ChargesTemplates = BLROW.Instance.getChargesTemplates();
            return PartialView("_ROWApplyStage", objApplyRow);
        }
        public ActionResult SaveApplyROW(rowApplyStage objApplyRow)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            ROWMaster objROW = new ROWMaster();
            var applyResp = BLROW.Instance.ApplyROW(objApplyRow, Convert.ToInt32(Session["user_id"]));
            objPM.status = StatusCodes.OK.ToString();
            objPM.message = Resources.Resources.SI_OSP_ROW_NET_FRM_098;
            objROW = GetROWDetail("A", objApplyRow.row_system_id, "", "", 0);
            objROW.objPM = objPM;
            if (objApplyRow.remarksList.Count > 0)
            {
                new BLROW().SaveROWRemarks(objApplyRow.remarksList, Convert.ToInt32(Session["user_id"]));
            }
            return PartialView("_AddROW", objROW);
        }
        public PartialViewResult ApproveReject(int rowSystemId, string geomType, string rowType)
        {
            rowApproveRejectStage objApproveRow = new rowApproveRejectStage();
            objApproveRow = BLROW.Instance.getROWApproveDetails(rowSystemId);
            objApproveRow.rowType = rowType;
            objApproveRow.row_system_id = rowSystemId;
            objApproveRow.listAttachmentType = new BLMisc().getAttachment(EntityType.ROW.ToString(), ROWStage.Approved.ToString() + "," + ROWStage.Rejected.ToString());
            var lstDocument = new BLROW().getROWAttachment(rowSystemId, objApproveRow.row_status, "Document");
            var areaLength = BLROW.Instance.GetAreaLength(rowSystemId, EntityType.ROW.ToString());
            if (areaLength != null) { objApproveRow.route_length = areaLength.row_length; }
            //converting file size
            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            objApproveRow.fileList = lstDocument;
            objApproveRow.remarksList = new BLROW().getROWRemarks(rowSystemId, objApproveRow.row_status);
            if (!string.IsNullOrEmpty(Convert.ToString(objApproveRow.payment_date)))
                objApproveRow.paymentDate = MiscHelper.FormatDate(Convert.ToString(objApproveRow.payment_date));
            if (!string.IsNullOrEmpty(Convert.ToString(objApproveRow.start_date)))
                objApproveRow.startDate = MiscHelper.FormatDate(Convert.ToString(objApproveRow.start_date));
            if (!string.IsNullOrEmpty(Convert.ToString(objApproveRow.payment_date)))
                objApproveRow.endDate = MiscHelper.FormatDate(Convert.ToString(objApproveRow.end_date));
            if (!string.IsNullOrEmpty(Convert.ToString(objApproveRow.payment_date)))
                objApproveRow.accessChargeStartDate = MiscHelper.FormatDate(Convert.ToString(objApproveRow.access_charges_start_date));
            BindROWDropDown(objApproveRow);
            return PartialView("_ROWApproveReject", objApproveRow);
        }
        public PartialViewResult SaveApproveROW(rowApproveRejectStage objApproveRow)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            ROWMaster objROW = new ROWMaster();
            objApproveRow.payment_date = Convert.ToDateTime(objApproveRow.paymentDate);
            objApproveRow.start_date = Convert.ToDateTime(objApproveRow.startDate);
            objApproveRow.end_date = Convert.ToDateTime(objApproveRow.endDate);
            objApproveRow.access_charges_start_date = Convert.ToDateTime(objApproveRow.accessChargeStartDate);
            BLROW.Instance.ApproveROW(objApproveRow, Convert.ToInt32(Session["user_id"]));
            BindROWDropDown(objApproveRow);
            objPM.status = StatusCodes.OK.ToString();
            objPM.message = "ROW has been " + objApproveRow.row_status + " successfully!";
            objROW = GetROWDetail("A", objApproveRow.row_system_id, "", "", 0);
            objROW.objPM = objPM;
            if (objApproveRow.remarksList.Count > 0)
            {
                new BLROW().SaveROWRemarks(objApproveRow.remarksList, Convert.ToInt32(Session["user_id"]));
            }
            return PartialView("_AddROW", objROW);
        }
        public PartialViewResult ROWNAssociateEntity()
        {
            return PartialView("_ROWNAssociateEntity");
        }
        private void BindROWDropDown(rowApproveRejectStage objROWMaster)
        {
            objROWMaster.listRowStatus = new BLMisc().GetDropDownList(EntityType.ROW.ToString(), "ROW_Status");
            objROWMaster.listRowStartaType = new BLMisc().GetDropDownList(EntityType.ROW.ToString(), "ROW_Starta_Type");
        }
        public JsonResult SavePIT(ROWPIT objPIT)
        {
            fillRegionProvinceDetail(objPIT, EntityType.PIT.ToString(), objPIT.geom);
            var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.PIT.ToString(), gType = GeometryType.Circle.ToString(), eGeom = objPIT.geom });
            objPIT.network_id = objNetworkCodeDetail.network_code;
            objPIT.sequence_id = objNetworkCodeDetail.sequence_id;
            var pitDetails = BLROW.Instance.SavePIT(objPIT, Convert.ToInt32(Session["user_id"]));
            PageMessage objPM = new PageMessage();
            objPM.status = StatusCodes.OK.ToString();
            objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_201;
            pitDetails.objPM = objPM;

            return Json(pitDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPITradius(int systemId, string entityType)
        {
            var radius = BLROW.Instance.GetPITRadius(systemId, entityType);
            return Json(radius, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getROWDetails(int systemId)
        {
            ROWMaster row = new BLMisc().GetEntityDetailById<ROWMaster>(systemId, EntityType.ROW);
            return Json(row, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getPITDefaultRadius(int systemId)
        {
            var pitRadius = new BLROW().getPITDefaultRadius(systemId, Convert.ToInt32(Session["user_id"]));
            return Json(pitRadius, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getROWDetailsByPIT(int systemId)
        {
            ROWMaster row = BLROW.Instance.getROWByPIT(systemId);
            return Json(row, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetROWAttachmentDetails(int systemId, string rowStage, string entityType)
        {
            var lstDocument = new BLROW().getROWAttachment(systemId, rowStage, "Document");
            //converting file size
            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            return PartialView("_ROWAttachmentsFiles", lstDocument);
        }
        public PartialViewResult ROWAssociation(int systemId, string entityType, string networkId)
        {
            ROWAssociation objROWEntity = new ROWAssociation();
            objROWEntity.parent_system_id = systemId;
            objROWEntity.parent_network_id = networkId;
            objROWEntity.parent_entity_type = entityType;
            objROWEntity.entityList = BLROW.Instance.getAssociateEntityList(systemId, entityType);
            if (objROWEntity.entityList.Count > 0 && objROWEntity.entityList.Where(m => m.is_associated == true).ToList().Count > 0)
            { objROWEntity.isExportEnabled = true; }
            return PartialView("_ROWAssociateEntity", objROWEntity);
        }
        public PartialViewResult SaveROWAssociation(ROWAssociation objROWEntity)
        {
            PageMessage objPM = new PageMessage();
            BLROW.Instance.saveROWAssocition(JsonConvert.SerializeObject(objROWEntity.entityList), objROWEntity.parent_system_id, objROWEntity.parent_entity_type, objROWEntity.parent_network_id, Convert.ToInt32(Session["user_id"]));
            objPM.status = ResponseStatus.OK.ToString();
            objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_202;
            objROWEntity.pageMsg = objPM;
            //return Json(objROWEntity,JsonRequestBehavior.AllowGet);
            return PartialView("_ROWAssociateEntity", objROWEntity);
        }
        public void ExportROWAssociation(int systemId, string entityType, string networkId)
        {
            var listLineEntityInfo = new BLMisc().GetROWAssociationExportData<Dictionary<string, string>>(systemId, entityType);
            DataTable dtlogs = Utility.MiscHelper.GetDataTableFromDictionaries(listLineEntityInfo);
            ExportData(dtlogs, networkId + "_Association");
        }
        public JsonResult getCostTemplateById(int id)
        {
            var templatedetails = new BLROW().getTemplateById(id);
            return Json(templatedetails, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Patch Cord        
        public PatchCordMaster GetPatchCordDetail(LineEntityIn objIn)
        {
            PatchCordMaster objPatch = new PatchCordMaster();
            var userdetails = (User)Session["userDetail"];
            objPatch.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            if (objIn.systemId == 0)
            {
                objPatch.geom = objIn.geom;
                objPatch.networkIdType = objIn.networkIdType;
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objPatch, GeometryType.Line.ToString(), objIn.geom);
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<PatchCordTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.PatchCord, objIn.cableType);
                MiscHelper.CopyMatchingProperties(objItem, objPatch);
            }
            else
            {
                objPatch = new BLMisc().GetEntityDetailById<PatchCordMaster>(objIn.systemId, EntityType.PatchCord);
            }
            return objPatch;
        }
        public PartialViewResult AddPatchCord(PatchCordMaster objPatch, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //PatchCordMaster objPatch = new PatchCordMaster();
            bool isEquipmentPatching = objPatch.isEquipmentPatching;
            if (string.IsNullOrEmpty(objPatch.geom))
            {
                var startGeom = new BLMisc().getEntityGeom(objPatch.a_system_id, objPatch.a_entity_type);
                var endGeom = new BLMisc().getEntityGeom(objPatch.b_system_id, objPatch.b_entity_type);
                objPatch.geom = startGeom + "," + endGeom + (startGeom == endGeom ? "1" : "");
            }

            objPatch.lstTP.Add(new NetworkDtl { system_id = objPatch.a_system_id, network_id = objPatch.a_location, network_name = objPatch.a_entity_type });
            objPatch.lstTP.Add(new NetworkDtl { system_id = objPatch.b_system_id, network_id = objPatch.b_location, network_name = objPatch.b_entity_type });

            LineEntityIn objIn = new LineEntityIn() { geom = objPatch.geom, systemId = objPatch.system_id, networkIdType = objPatch.networkIdType, lstTP = objPatch.lstTP };
            objPatch = GetPatchCordDetail(objIn);
            if (objPatch.system_id == 0)
            {
                //Fill Location detail...    
                GetLineNetworkDetail(objPatch, objIn, EntityType.PatchCord.ToString(), false);
            }
            BLItemTemplate.Instance.BindItemDropdowns(objPatch, EntityType.PatchCord.ToString());
            fillProjectSpecifications(objPatch);
            objPatch.pSystemId = pSystemId;
            objPatch.pNetworkId = pNetworkId;
            objPatch.isEquipmentPatching = isEquipmentPatching;
            ModelState.Clear();
            return PartialView("_AddPatchCord", objPatch);
        }
        public JsonResult SavePatchCord(PatchCordMaster objPatch, bool isDirectSave = false)
        {

            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            bool isValid = true;
            int pSystemId = objPatch.pSystemId;
            string pNetworkId = objPatch.pNetworkId;
            int StructureId = objPatch.structure_id;
            if (objPatch.networkIdType == NetworkIdType.A.ToString() && objPatch.system_id == 0)
            {
                if (string.IsNullOrEmpty(objPatch.geom))
                {
                    var startGeom = new BLMisc().getEntityGeom(objPatch.a_system_id, objPatch.a_entity_type);
                    var endGeom = new BLMisc().getEntityGeom(objPatch.b_system_id, objPatch.b_entity_type);
                    objPatch.geom = startGeom + "," + endGeom + (startGeom == endGeom ? "1" : "");
                }

                objPatch.lstTP.Add(new NetworkDtl { system_id = objPatch.a_system_id, network_id = objPatch.a_location, network_name = objPatch.a_entity_type });
                objPatch.lstTP.Add(new NetworkDtl { system_id = objPatch.b_system_id, network_id = objPatch.b_location, network_name = objPatch.b_entity_type });

                var objLineEntity = new LineEntityIn() { geom = objPatch.geom, systemId = objPatch.system_id, networkIdType = objPatch.networkIdType, lstTP = objPatch.lstTP };
                if (isDirectSave == true)
                {
                    objPatch = GetPatchCordDetail(objLineEntity);
                    objPatch.pSystemId = pSystemId;
                    objPatch.pNetworkId = pNetworkId;
                    objPatch.structure_id = StructureId;
                }
                //GET AUTO NETWORK CODE...
                GetLineNetworkDetail(objPatch, objLineEntity, EntityType.PatchCord.ToString(), true);
                if (isDirectSave == true)
                    objPatch.patch_cord_name = objPatch.network_id;
            }
            if (TryValidateModel(objPatch) && isValid == true)
            {
                var isNew = objPatch.system_id > 0 ? false : true;
                var resultItem = BLPatchCord.Instance.SavePatchCord(objPatch, Convert.ToInt32(Session["user_id"]));

                if (string.IsNullOrEmpty(resultItem.objPM.message))
                {
                    if (isNew)
                    {
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.isNewEntity = isNew;
                        objPM.message = Resources.Resources.SI_OSP_PCD_NET_FRM_005;
                    }
                    else
                    {
                        if (resultItem.isPortConnected)
                        {
                            objPM.status = ResponseStatus.OK.ToString();
                            objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);// resultItem.message;
                        }
                        else
                        {
                            objPM.status = ResponseStatus.OK.ToString();
                            objPM.message = Resources.Resources.SI_OSP_PCD_NET_FRM_006;
                        }
                    }
                    //Save Reference
                    if (objPatch.EntityReference != null && resultItem.system_id > 0)
                    {
                        SaveReference(objPatch.EntityReference, resultItem.system_id);
                    }
                    objPatch.objPM = objPM;
                }

            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = isValid == true ? getFirstErrorFromModelState() : objPM.message;
                objPatch.objPM = objPM;
            }
            return Json(objPatch, JsonRequestBehavior.AllowGet);
        }

        #endregion
        public JsonResult GetBuildingAddress(int structureId)
        {
            var address = BLStructure.Instance.getBuildingAddress(structureId);
            return Json(address, JsonRequestBehavior.AllowGet);
        }

        #region SITE INFORMATION

        public PartialViewResult GetSiteInformation(int structrueId)
        {

            StructureMaster objStructureMaster = new StructureMaster();
            var siteVendor = new List<siteVendorList>();
            var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(structrueId, EntityType.Structure);
            objStructureMaster.SiteInformation = new BLSiteInfo().GetSiteIfo(structrueId, EntityType.Structure.ToString());
            if (objStructureMaster.SiteInformation.system_id == 0)
            {
                objStructureMaster.SiteInformation.latitude = structureDetails.latitude;
                objStructureMaster.SiteInformation.longitude = structureDetails.longitude;
            }

            objStructureMaster.SiteInformation.city = structureDetails.province_name;
            objStructureMaster.SiteInformation.parent_entity_type = EntityType.Structure.ToString();
            BindSiteDropDown(objStructureMaster.SiteInformation);
            if (objStructureMaster.SiteInformation.system_id > 0 && objStructureMaster.SiteInformation.lmc_type != null)
            {

                List<Dictionary<string, string>> lstCustomerInfo = new BLCustomer().GetSiteCustomerList(objStructureMaster.SiteInformation.system_id, objStructureMaster.SiteInformation.lmc_type);
                lstCustomerInfo = BLConvertMLanguage.ExportMultilingualConvert(lstCustomerInfo);
                foreach (Dictionary<string, string> dic in lstCustomerInfo)
                {

                    var obj = (IDictionary<string, object>)new ExpandoObject();
                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                    foreach (var col in dic)
                    {
                        if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                        {

                            obj.Add(col.Key, col.Value);
                        }
                    }
                    objStructureMaster.SiteInformation.lstData.Add(obj);

                }

            }
            objStructureMaster.SiteInformation.lstSiteInfoAttachment = new BLAttachment().getAttachmentDetails(objStructureMaster.SiteInformation.parent_system_id, objStructureMaster.SiteInformation.parent_entity_type, "Document", "SiteInfo"); //new BLSiteInfoAttachment().getSiteInfoAttachment(objStructureMaster.SiteInformation.parent_system_id, objStructureMaster.SiteInformation.parent_entity_type, "Document");


            foreach (var item in objStructureMaster.SiteInformation.lstSiteInfoAttachment)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }


            objStructureMaster.SiteInformation.parent_system_id = structrueId;
            objStructureMaster.SiteInformation.parent_entity_type = EntityType.Structure.ToString();
            objStructureMaster.SiteInformation.created_by = Convert.ToInt32(Session["user_id"]);
            return PartialView("_AddSiteInfo", objStructureMaster);
        }

        private void BindSiteDropDown(SiteInfo objsiteInfo)
        {
            objsiteInfo.lstLMCType = new BLMisc().GetDropDownList("", DropDownType.LMC_TYPE.ToString());
            objsiteInfo.lstSITEType = new BLMisc().GetDropDownList("", DropDownType.SITE_TYPE.ToString());
            objsiteInfo.lstStructureType = new BLMisc().GetDropDownList("", DropDownType.Structure_Type.ToString());
            objsiteInfo.lstStructureSize = new BLMisc().GetDropDownList("", DropDownType.Structure_Size.ToString());
            objsiteInfo.lstSiteCircle = new BLSiteCircle().GetCircleList();

        }

        public ActionResult SaveSiteInfo(SiteInfo objSiteInfo)
        {
            ModelState.Clear();
            if (TryValidateModel(objSiteInfo))
            {

                var result = BLSiteInfo.Instance.SaveSiteInfo(objSiteInfo, Convert.ToInt32(Session["user_id"]));
                if (string.IsNullOrEmpty(result.objPM.message))
                {
                    if (objSiteInfo.objPM.isNewEntity)
                    {
                        objSiteInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_203;
                        objSiteInfo.objPM.status = ResponseStatus.OK.ToString();
                    }
                    else
                    {
                        objSiteInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_204;
                        objSiteInfo.objPM.status = ResponseStatus.OK.ToString();
                    }
                }
                else
                {
                    objSiteInfo.objPM.status = ResponseStatus.FAILED.ToString();
                    objSiteInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_205;
                }

            }
            else
            {
                objSiteInfo.objPM.status = ResponseStatus.FAILED.ToString();
                objSiteInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_205;
            }
            BindSiteDropDown(objSiteInfo);
            return Json(objSiteInfo, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetSiteVendor(string searchText)
        {
            SiteInfo objSiteInfo = new SiteInfo();
            var siteVendor = new List<siteVendorList>();
            var result = new BLSiteInfo().GetSiteVendorListbyId(searchText);
            foreach (var item in result)
            {
                siteVendor.Add(new siteVendorList
                {
                    siteVendor = item.site_vendor
                });
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var lstSiteVendor = jsonSerialiser.Serialize(siteVendor);
            if (siteVendor.Count > 0)
            {
                return Json(lstSiteVendor, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(objSiteInfo, JsonRequestBehavior.AllowGet);
            }

        }

        public PartialViewResult AddSiteCustomer(int systemId, int siteId, string entityType, string lmcType, int structureId, string childModel = "", string featureName = "")
        {
            Customer objCustomer = new Customer();

            objCustomer.system_id = systemId;
            var objCustomerDetails = new BLCustomer().getCustomerbyId(systemId);
            if (objCustomerDetails != null) { objCustomer = objCustomerDetails; }
            if (string.IsNullOrEmpty(objCustomer.lmc_type) || objCustomer.site_id == 0)
            {
                objCustomer.lmc_type = lmcType;
                objCustomer.site_id = siteId;
            }
            if (childModel != "")
                objCustomer.childModel = childModel;
            var objSiteInfo = new BLSiteInfo().getSitebyId(objCustomer.site_id);
            objCustomer.lstFloorInfo = BLFloor.Instance.GetFloorByBld(objSiteInfo.parent_system_id);
            objCustomer.lstElectricalmeter = new BLMisc().GetDropDownList("", DropDownType.Electrical_Meter_Type.ToString());
            objCustomer.lstCableEntryPoints = new BLMisc().GetDropDownList("", DropDownType.Cable_Entry_Point.ToString());
            objCustomer.lstSiteCustomerAttachment = new BLAttachment().getAttachmentDetails(systemId, EntityType.Customer.ToString(), "Document", featureName);// new BLSiteCustomerAttachment().getSiteCustomerAttachment(customerDetails.system_id, "Customer", "Document");
            foreach (var item in objCustomer.lstSiteCustomerAttachment)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            if (objCustomer.system_id > 0)
            {
                //var objSiteInfo = new BLSiteInfo().getSitebyId(objCustomer.site_id);
                var CustomerDetails = BLIspEntityMapping.Instance.GetIspEntityMapByCustomerId(objCustomer.system_id, EntityType.Customer.ToString());
                //var UnitDetails = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objSiteInfo.parent_system_id, objCustomer.parent_system_id, EntityType.UNIT.ToString());
                var floor = BLISP.Instance.getFloorInfo(CustomerDetails.floor_id);
                objCustomer.Floor_Name = floor.floor_name;
                objCustomer.floor_id = floor.system_id;
            }

            return PartialView("_AddSiteCustomer", objCustomer);

        }

        public PartialViewResult SaveSiteCustomer(Customer objSiteCustomer, int site_id)
        {
            SiteInfo objSiteInfo = new SiteInfo();
            StructureMaster objStructureMaster = new StructureMaster();
            FloorInfo objFloorInfo = new FloorInfo();
            var isNew = objSiteCustomer.system_id > 0;
            PageMessage objPM = new PageMessage();
            objSiteCustomer.site_id = site_id;
            objSiteInfo = new BLSiteInfo().getSitebyId(site_id);
            objStructureMaster = new BLMisc().GetEntityDetailById<StructureMaster>(objSiteInfo.parent_system_id, EntityType.Structure);
            objFloorInfo = BLFloor.Instance.GetFloorById(objSiteCustomer.floor_id);
            //objFloorInfo = BLFloor.Instance.GetFloorByName(objSiteCustomer.Floor_Name, objSiteInfo.parent_system_id);
            objSiteCustomer.province_id = objStructureMaster.province_id;
            objSiteCustomer.province_name = objStructureMaster.province_name;
            objSiteCustomer.region_id = objStructureMaster.region_id;
            objSiteCustomer.region_name = objStructureMaster.region_name;
            objSiteCustomer.structure_id = objStructureMaster.system_id;
            objSiteCustomer.structure_name = objStructureMaster.structure_name;
            objSiteCustomer.lstElectricalmeter = new BLMisc().GetDropDownList("", DropDownType.Electrical_Meter_Type.ToString());
            objSiteCustomer.lstCableEntryPoints = new BLMisc().GetDropDownList("", DropDownType.Cable_Entry_Point.ToString());

            // ---Start Region--CREATE UNIT BASED ON THE SELECTED FLOOR AND STRUCTUREID
            RoomInfo objRoomInfo = new RoomInfo();
            var customerDetails = new BLCustomer().getCustomerbyId(objSiteCustomer.system_id);
            objRoomInfo.system_id = customerDetails.parent_system_id;
            //if (objRoomInfo.system_id==0)
            //{ //GET AUTO NETWORK CODE...
            var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objSiteInfo.parent_system_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.UNIT.ToString(), structureId = objSiteInfo.parent_system_id });
            objRoomInfo.objIspEntityMap.floor_id = objFloorInfo.system_id;
            objRoomInfo.objIspEntityMap.structure_id = objSiteInfo.parent_system_id;
            objRoomInfo.parent_entity_type = EntityType.Structure.ToString();
            objRoomInfo.network_id = objISPNetworkCode.network_code;
            objRoomInfo.room_name = objISPNetworkCode.network_code;
            objRoomInfo.sequence_id = objISPNetworkCode.sequence_id;
            objRoomInfo.region_id = objStructureMaster.region_id;
            objRoomInfo.province_id = objStructureMaster.province_id;
            objRoomInfo.latitude = objStructureMaster.latitude;
            objRoomInfo.longitude = objStructureMaster.longitude;
            objRoomInfo.room_height = 1;
            objRoomInfo.room_length = 1;
            objRoomInfo.room_width = 1;
            objRoomInfo.unit_type = "1 BHK";
            objRoomInfo.parent_system_id = objSiteInfo.parent_system_id;
            objRoomInfo.area = 1;
            objRoomInfo.floor_id = objFloorInfo.system_id;
            objRoomInfo.structure_id = objSiteInfo.parent_system_id;
            objRoomInfo.userId = Convert.ToInt32(Session["user_id"]);
            objRoomInfo.created_on = DateTimeHelper.Now;

            var result = new BLISP().SaveRoomDetails(objRoomInfo);

            objSiteCustomer.parent_system_id = result.system_id;
            objSiteCustomer.parent_entity_type = EntityType.UNIT.ToString();
            objSiteCustomer.parent_network_id = result.network_id;
            //}

            objSiteCustomer.objIspEntityMap.floor_id = objFloorInfo.system_id;
            // ISP ENTITY MAPPING IF UNIT UPDATE
            if (result.parent_entity_type.ToLower() == "structure" && objRoomInfo.system_id > 0)
            {
                if (objRoomInfo.objIspEntityMap.floor_id != 0)
                {
                    IspEntityMapping objMapping = new IspEntityMapping();
                    var Details = BLIspEntityMapping.Instance.GetIspEntityMapByCustomerId(result.system_id, EntityType.UNIT.ToString());
                    objMapping.id = Details.id;
                    objMapping.structure_id = objStructureMaster.system_id;
                    objMapping.floor_id = objRoomInfo.objIspEntityMap.floor_id ?? 0;
                    objMapping.shaft_id = objRoomInfo.objIspEntityMap.shaft_id ?? 0;
                    objMapping.entity_id = result.system_id;
                    objMapping.entity_type = EntityType.UNIT.ToString();
                    var insertMap = BLIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                }
            }
            //---##END Region--

            objSiteCustomer = new BLCustomer().SaveCustomer(objSiteCustomer, Convert.ToInt32(Session["user_id"]));


            objPM.status = StatusCodes.OK.ToString();
            if (isNew)
            {
                objPM.message = Resources.Resources.SI_OSP_CUS_NET_FRM_006;
            }
            else
            {
                objPM.message = Resources.Resources.SI_OSP_CUS_NET_FRM_007;
            }
            objSiteCustomer.objPM = objPM;
            objSiteCustomer.lstElectricalmeter = new BLMisc().GetDropDownList("", DropDownType.Electrical_Meter_Type.ToString());
            objSiteCustomer.lstCableEntryPoints = new BLMisc().GetDropDownList("", DropDownType.Cable_Entry_Point.ToString());

            return PartialView("_AddSiteCustomer", objSiteCustomer);
        }

        public PartialViewResult SiteCustomerList(int systemId, int siteId, string lmcType)
        {
            SiteInfo SiteInfoCustomer = new SiteInfo();
            SiteInfoCustomer.system_id = siteId;
            SiteInfoCustomer.lmc_type = lmcType;
            List<Dictionary<string, string>> lstCustomerInfo = new BLCustomer().GetSiteCustomerList(siteId, lmcType);
            lstCustomerInfo = BLConvertMLanguage.ExportMultilingualConvert(lstCustomerInfo);
            foreach (Dictionary<string, string> dic in lstCustomerInfo)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {

                        obj.Add(col.Key, col.Value);
                    }
                }
                SiteInfoCustomer.lstData.Add(obj);
            }

            return PartialView("_listsiteCustomer", SiteInfoCustomer);

        }


        public JsonResult GetSmallCellUID(string UID)
        {
            SiteInfo objSiteInfo = new SiteInfo();
            objSiteInfo = BLSiteInfo.Instance.ValidateETPILID(UID);
            return Json(objSiteInfo, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSiteCustomerPAF(string PAFNO)
        {
            Customer objSiteCustomer = new Customer();
            objSiteCustomer = new BLCustomer().getSiteCustomerPAF(PAFNO);
            return Json(objSiteCustomer, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSiteCustomerPO(string PONumber)
        {
            Customer objSiteCustomer = new Customer();
            objSiteCustomer = new BLCustomer().getSiteCustomerPO(PONumber);
            return Json(objSiteCustomer, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerSiteId(string CustomerSiteId)
        {
            Customer objSiteCustomer = new Customer();
            objSiteCustomer = new BLCustomer().getSiteCustomerId(CustomerSiteId);
            return Json(objSiteCustomer, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetSiteInfoAttachment(int system_Id, string entity_type, string featureName)
        {
            var lstDocument = new BLAttachment().getAttachmentDetails(system_Id, entity_type, "Document", featureName); //new BLSiteInfoAttachment().getSiteInfoAttachment(system_Id, entity_type, "Document");
                                                                                                                        //converting file size
            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            return PartialView("_viewSiteInfoAttachment", lstDocument);
        }
        public PartialViewResult GetSiteCustomerAttachment(int system_Id, string entity_type, string featureName)
        {
            var lstDocument = new BLAttachment().getAttachmentDetails(system_Id, entity_type, "Document", featureName);// new BLSiteCustomerAttachment().getSiteCustomerAttachment(system_Id, "Customer", "Document");
                                                                                                                       //converting file size
            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            return PartialView("_ViewSiteCustomerAttachment", lstDocument);
        }

        public void ExportCustomers(int siteId, string lmcType)
        {



            List<Dictionary<string, string>> lstCustomerInfo = new BLCustomer().GetSiteCustomerList(siteId, lmcType);
            lstCustomerInfo = BLConvertMLanguage.ExportMultilingualConvert(lstCustomerInfo);
            DataTable dtReport = new DataTable();
            dtReport = MiscHelper.GetDataTableFromDictionaries(lstCustomerInfo);

            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }
                if (dtReport.Columns.Contains("System Id")) { dtReport.Columns.Remove("System Id"); }
            }
            if (dtReport.Rows.Count > 0)
            {
                ExportReport(dtReport, "Customer_Report_" + Utility.MiscHelper.getTimeStamp());
            }
        }


        private void ExportReport(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }
        public JsonResult ValidateUnitsOnFloor(int floorId, int customerId)
        {
            //FloorInfo objFloorInfo = new FloorInfo();
            //objFloorInfo = BLFloor.Instance.GetFloorById(floorId);
            //return Json(objFloorInfo, JsonRequestBehavior.AllowGet);

            DbMessage objMessage = new BLMisc().validateUnitsOnFloors(new validateEntity
            {
                system_id = customerId,
                entity_type = EntityType.Customer.ToString(),
                floor_id = floorId,
                shaft_id = 0,
            }, customerId == 0);
            return Json(objMessage, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region LMC REGION START
        public PartialViewResult GetLMCInformation(int cableId, string entityType)
        {
            CableMaster objCablemaster = new CableMaster();
            LMCCableInfo objlmcInfo = new LMCCableInfo();
            objCablemaster = new BLMisc().GetEntityDetailById<CableMaster>(cableId, EntityType.Cable);
            objCablemaster.LMCCableInfo = new BLLmcInfo().GetLMCIfo(cableId);
            objCablemaster.LMCCableInfo.cable_system_id = cableId;
            var result = new BLLmcInfo().getCableLatLong(EntityType.Cable.ToString(), objCablemaster.system_id);
            objCablemaster.LMCCableInfo.rtn_building_side_tapping_latitude = result.a_latitude;
            objCablemaster.LMCCableInfo.rtn_building_side_tapping_longitude = result.a_longitude;
            BindLMCDropdownList(objlmcInfo);
            // objCablemaster.LMCCableInfo = objlmcInfo;
            objCablemaster.LMCCableInfo.created_by = Convert.ToInt32(Session["user_id"]);

            BindLMCDropdownList(objCablemaster.LMCCableInfo);
            return PartialView("_AddLMCInfo", objCablemaster);
        }
        public void BindLMCDropdownList(LMCCableInfo objlmcInfo)
        {
            objlmcInfo.lstLMCType = new BLMisc().GetDropDownList("", DropDownType.LMC_TYPE.ToString());
            objlmcInfo.lstLMCCascadedStandalone = new BLMisc().GetDropDownList("", DropDownType.LMC_Cascaded_Standalone.ToString());
            objlmcInfo.lstRTNBuildingSiteTappingPoint = new BLMisc().GetDropDownList("", DropDownType.RTN_Building_Side_Tapping_Point.ToString());
            objlmcInfo.lstFiberCount = new BLMisc().GetDropDownList("", DropDownType.Fiber_Count.ToString());
            objlmcInfo.lstTerminationDetails = new BLMisc().GetDropDownList("", DropDownType.Termination_Details.ToString());
            objlmcInfo.lstLMCAttachment = new BLAttachment().getAttachmentDetails(objlmcInfo.system_id, "Cable", "Document", "LMCInfo");
            //converting file size
            foreach (var item in objlmcInfo.lstLMCAttachment)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
        }

        public ActionResult SaveLMCInfo(LMCCableInfo objLMCInfo)
        {
            ModelState.Clear();
            if (TryValidateModel(objLMCInfo))
            {
                // GENERATE LMC ID
                if (objLMCInfo.system_id == 0)
                {
                    var lmcDetails = BLLmcInfo.Instance.GetLMCId(objLMCInfo.lmc_type, objLMCInfo.lmc_standalone_redundant);
                    objLMCInfo.lmc_id = lmcDetails.lmc_id;
                }
                var result = BLLmcInfo.Instance.SaveLMCInfo(objLMCInfo, Convert.ToInt32(Session["user_id"]));
                if (string.IsNullOrEmpty(result.objPM.message))
                {
                    if (objLMCInfo.objPM.isNewEntity)
                    {
                        objLMCInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_206;
                        objLMCInfo.objPM.status = ResponseStatus.OK.ToString();
                    }
                    else
                    {
                        objLMCInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_207;
                        objLMCInfo.objPM.status = ResponseStatus.OK.ToString();
                    }
                }
                else
                {
                    objLMCInfo.objPM.status = ResponseStatus.FAILED.ToString();
                    objLMCInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_208;
                }

            }
            else
            {
                objLMCInfo.objPM.status = ResponseStatus.FAILED.ToString();
                objLMCInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_208;
            }
            BindLMCDropdownList(objLMCInfo);
            return Json(objLMCInfo, JsonRequestBehavior.AllowGet);

        }

        public JsonResult validateSiteCustomer(string searchText, string columnName, string lmcType)
        {
            JsonResponse<Dictionary<string, string>> objResp = new JsonResponse<Dictionary<string, string>>();
            try
            {
                var siteCustomreInfo = new BLMisc().getSiteCustomerDetails(searchText, columnName, lmcType);
                if (siteCustomreInfo.Count > 0)
                {
                    // siteCustomreInfo.Remove("customer_system_id");
                    objResp.result = siteCustomreInfo;
                    objResp.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
                }
            }
            catch (Exception)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_291;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
            #endregion
        }

        public PartialViewResult GetLMCAttachment(int system_Id, string entity_type, string featureName)
        {
            var lstDocument = new BLAttachment().getAttachmentDetails(system_Id, "Cable", "Document", "LMCInfo"); //new BLSiteInfoAttachment().getSiteInfoAttachment(system_Id, entity_type, "Document");
                                                                                                                  //converting file size
            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            return PartialView("_ViewLMCAttachments", lstDocument);
        }

        public void ExportChildEntityByParentCode(int parentSystemId, string parentNetworkId, string parentEntityType, string childEntityType)
        {
            var exportData = new BLMisc().ExportChildEntityByParentCode<Dictionary<string, string>>(parentSystemId, parentNetworkId, parentEntityType, childEntityType);
            exportData = BLConvertMLanguage.ExportMultilingualConvert(exportData);
            DataTable dtReport = new DataTable();
            dtReport = MiscHelper.GetDataTableFromDictionaries(exportData);

            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }
                if (dtReport.Columns.Contains("System Id")) { dtReport.Columns.Remove("System Id"); }
            }
            if (dtReport.Rows.Count > 0)
            {
                ExportChildEntityReport(dtReport, childEntityType + Utility.MiscHelper.getTimeStamp());
            }
        }


        private void ExportChildEntityReport(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }

        public PartialViewResult GetBuildingComments(int buildingId, string childModel = "")
        {

            List<BuildingComments> objBuildingComments = new List<BuildingComments>();
            if (buildingId != 0)
            {
                objBuildingComments = new BLBuildingComment().getbuildingComments(buildingId);
            }
            return PartialView("_BuildingStatusSummary", objBuildingComments);
        }
        public PartialViewResult AddBuildingComment(string buttonType, string formType, string childModel = "")
        {
            BuildingMaster objBuildingMaster = new BuildingMaster();
            // BuildingComments objBuildingComments = new BuildingComments();
            BuildingComments objBuildingComments = objBuildingMaster.buildingComment;
            var objDDL = new BLMisc().GetDropDownList(EntityType.Building.ToString());
            objBuildingMaster.lstTenancy = objDDL.Where(x => x.dropdown_type == DropDownType.Tenancy.ToString()).ToList();
            objBuildingMaster.lstRejectComment = objDDL.Where(x => x.dropdown_type == DropDownType.RejectComment.ToString()).ToList();
            // objBuildingMaster.buildingComment.lstRejectComment = objBuildingComments.lstRejectComment;
            if (childModel != "")
                objBuildingMaster.childModel = childModel;
            BindBuildingDropDown(objBuildingMaster);
            objBuildingMaster.buttonType = buttonType;
            objBuildingMaster.formType = formType;
            return PartialView("_AddBuildingComment", objBuildingMaster);
        }

        #region Fault Entity
        public PartialViewResult AddFault(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string faultyFiber = "")
        {
            Fault objFaultStatusVeiwModel = new Fault();
            //if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            //{
            //    geom = new BLMisc().getEntityGeom(pSystemId, pEntityType);
            //}
            //objFaultStatusVeiwModel.objFault = GetFaultDetail(networkIdType, systemId, geom);
            //objFaultStatusVeiwModel.objFaultStatusHistory = GetFaultStatusDetail(objFaultStatusVeiwModel.objFault.system_id);
            //BindFaultDropDown(objFaultStatusVeiwModel);
            //return PartialView("_AddFault", objFaultStatusVeiwModel);
            objFaultStatusVeiwModel.networkIdType = networkIdType;
            objFaultStatusVeiwModel.system_id = systemId;
            objFaultStatusVeiwModel.geom = geom;
            objFaultStatusVeiwModel.pSystemId = pSystemId;
            objFaultStatusVeiwModel.pEntityType = pEntityType;
            objFaultStatusVeiwModel.user_id = Convert.ToInt32(Session["user_id"]);
            objFaultStatusVeiwModel.user_name = Convert.ToString(((User)Session["userDetail"]).user_name);
            objFaultStatusVeiwModel.select_entity = faultyFiber;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<Fault>(url, objFaultStatusVeiwModel, EntityType.Fault.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddFault", response.results);
        }

        public Fault GetFaultDetail(string networkIdType, int systemId, string geom = "")
        {
            Fault objFault = new Fault();
            objFault.geom = geom;
            objFault.networkIdType = networkIdType;
            //objFault.up.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objFault, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objFault, new NetworkCodeIn() { eType = EntityType.Fault.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFault.geom }, networkIdType);
                string[] lnglat = geom.Split(new string[] { " " }, StringSplitOptions.None);
                objFault.latitude = Convert.ToDouble(lnglat[1].ToString());
                objFault.longitude = Convert.ToDouble(lnglat[0].ToString());
            }
            else
            {
                // Get entity detail by Id...
                objFault = BLFault.Instance.GetFaultById(systemId);
                //  objFault = new BLMisc().GetEntityDetailById<Fault>(systemId, EntityType.Fault);
            }

            return objFault;
        }

        public FaultStatusHistory GetFaultStatusDetail(int faultSystemId)
        {
            FaultStatusHistory objFault = new FaultStatusHistory();
            if (faultSystemId == 0)
            {
                objFault.updated_by = Convert.ToString(((User)Session["userDetail"]).user_name);
            }
            else
            {
                // Get entity detail by Id... 
                objFault = BLFaultStatusHistory.Instance.GetFaultStatusHistoryById(faultSystemId);
            }

            return objFault;
        }

        private void BindFaultDropDown(FaultStatusViewModel objFaultViewModel)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Fault.ToString());
            objFaultViewModel.objFault.lstTicketType = objDDL.Where(x => x.dropdown_type == DropDownType.Fault_Ticket_Type.ToString()).ToList();
            objFaultViewModel.objFaultStatusHistory.lstFaultStatus = objDDL.Where(x => x.dropdown_type == DropDownType.Fault_Status.ToString()).ToList();
            objFaultViewModel.objFault.lstEntitiesNearbyFault = new BLMisc().getEntitiesNearbyFault(objFaultViewModel.objFault.latitude, objFaultViewModel.objFault.longitude);
            objFaultViewModel.objFault.lstBusinessType = objDDL.Where(x => x.dropdown_type == DropDownType.Business_Type.ToString()).ToList();
            //objFault.objFaultStatusHistory.lstFaultStatus = abc;
            //objFault.lstRFSStatus = objDDL.Where(x => x.dropdown_type == DropDownType.RFS_Status.ToString()).ToList();
            //objFault.lstMedia = objDDL.Where(x => x.dropdown_type == DropDownType.Media.ToString()).ToList();
            //objFault.lstBuildingType = objDDL.Where(x => x.dropdown_type == DropDownType.Building_Type.ToString()).ToList();
            //objFault.lstSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.SubCategory.ToString()).ToList();
        }
        [HttpPost]
        public ActionResult SaveFault(Fault objFaultStatusViewModel, bool isDirectSave = false)
        {

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objFaultStatusViewModel.objFault.networkIdType == NetworkIdType.A.ToString() && objFaultStatusViewModel.objFault.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Fault.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFaultStatusViewModel.objFault.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objFaultStatusViewModel.objFault = GetFaultDetail(objFaultStatusViewModel.objFault.networkIdType, objFaultStatusViewModel.objFault.system_id, objFaultStatusViewModel.objFault.geom);

            //    }
            //    //SET NETWORK CODE
            //    objFaultStatusViewModel.objFault.network_id = objNetworkCodeDetail.network_code;
            //    objFaultStatusViewModel.objFault.sequence_id = objNetworkCodeDetail.sequence_id;
            //    objFaultStatusViewModel.objFault.parent_network_id = objNetworkCodeDetail.parent_network_id;
            //}
            //if (TryValidateModel(objFaultStatusViewModel))
            //{
            //    // GENERATE FAULT ID
            //    if (objFaultStatusViewModel.objFault.system_id == 0)
            //    {
            //        var faultDetails = BLFault.Instance.GetFaultID(objFaultStatusViewModel.objFault.parent_network_id);
            //        objFaultStatusViewModel.objFault.fault_id = faultDetails.fault_id;
            //    }
            //    objFaultStatusViewModel.objFault.fault_status = objFaultStatusViewModel.objFaultStatusHistory.fault_status;
            //    var isNew = objFaultStatusViewModel.objFault.system_id > 0 ? false : true;
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.Fault.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    //var selected_Entity_GeomType = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objFaultStatusViewModel.objFault.fault_entity_type.ToUpper()).FirstOrDefault().geom_type;
            //    var latlong = new BLMisc().GetFaultEntityGeomInfo(objFaultStatusViewModel.objFault.fault_entity_system_id, objFaultStatusViewModel.objFault.fault_entity_type, objFaultStatusViewModel.objFault.latitude, objFaultStatusViewModel.objFault.longitude);
            //    if (latlong.entityGeom != "" && latlong.entityGeom != null)
            //    {
            //        string[] lnglat = latlong.entityGeom.Split(new string[] { " " }, StringSplitOptions.None);
            //        objFaultStatusViewModel.objFault.latitude = Convert.ToDouble(lnglat[1].ToString());
            //        objFaultStatusViewModel.objFault.longitude = Convert.ToDouble(lnglat[0].ToString());
            //    }
            //    var result = BLFault.Instance.SaveFault(objFaultStatusViewModel.objFault, Convert.ToInt32(Session["user_id"]));

            //    if (string.IsNullOrEmpty(result.objPM.message))
            //    {
            //        objFaultStatusViewModel.objFaultStatusHistory.fault_system_id = result.system_id;
            //        var objStatus = BLFaultStatusHistory.Instance.SaveFaultStatusHistory(objFaultStatusViewModel.objFaultStatusHistory, Convert.ToInt32(Session["user_id"]));
            //        string[] LayerName = { layer_title };
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        objFaultStatusViewModel.objFault.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objFaultStatusViewModel.objFault.objPM.status = ResponseStatus.FAILED.ToString();
            //    objFaultStatusViewModel.objFault.objPM.message = getFirstErrorFromModelState();
            //    objFaultStatusViewModel.objFault.objPM = objPM;
            //}


            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objFaultStatusViewModel.objFault.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    // fill dropdowns
            //    BindFaultDropDown(objFaultStatusViewModel);
            //    //objFaultStatusViewModel.objFault.created_on = Utility.MiscHelper.FormatDate(objFaultStatusViewModel.objFault.created_on.ToString());

            //    return PartialView("_AddFault", objFaultStatusViewModel);
            //}
            objFaultStatusViewModel.isDirectSave = isDirectSave;
            objFaultStatusViewModel.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objFaultStatusViewModel.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objFaultStatusViewModel.source_ref_type = "NETWORK_TICKET";
                objFaultStatusViewModel.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<Fault>(url, objFaultStatusViewModel, EntityType.Fault.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddFault", response.results);
        }

        public PartialViewResult GetFaultStautsHistory(int faultSystemId, string childModel = "")
        {

            List<FaultStatusHistory> objFaultStatusHistory = new List<FaultStatusHistory>();
            if (faultSystemId != 0)
            {
                objFaultStatusHistory = BLFaultStatusHistory.Instance.getFaultStatusHistoryList(faultSystemId);
            }
            return PartialView("_AddFaultStatusHistory", objFaultStatusHistory);
        }

        public JsonResult getFaultStatusById(int systemId)
        {
            Fault objFault = BLFault.Instance.GetFaultById(systemId);
            return Json(objFault, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public PartialViewResult GetPortInfo(int systemId, int modelId, string entity_type, string type = null)
        {
            List<IspPortInfo> objPortInfo = new List<IspPortInfo>();
            objPortInfo = new BLMisc().GetPortInfo(Convert.ToInt32(systemId), entity_type);
            for (int i = 0; i < objPortInfo.Count; i++)
            {
                objPortInfo[i].type = string.IsNullOrEmpty(type) ? "" : Convert.ToString(type);
            }
            var status = objPortInfo.Select(x => x.port_status).Distinct().ToList();
            if (status.Count > 0)
            {
                foreach (var item in status)
                {
                    FibrePortStatusCount obj = new FibrePortStatusCount();
                    obj.PortStatus = item;
                    obj.StatusCount = objPortInfo.Count(x => x.port_status == item);
                    objPortInfo[0].ViewPortStatusCount.Add(obj);
                }
            }
            return PartialView("_EntityPortInfo", objPortInfo);
        }
        public void DownloadPortReport(int systemId = 0, string entity_type = null)
        {
            var objResp = "";
            try
            {
                List<IspPortInfo> objPortInfo = new List<IspPortInfo>();
                objPortInfo = new BLMisc().GetPortInfo(Convert.ToInt32(systemId), entity_type);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<IspPortInfo>(objPortInfo);

                if (dtReport.Rows.Count > 0)
                {
                    dtReport.TableName = dtReport.Rows[0]["parent_network_id"].ToString();
                    dtReport.Columns.Remove("system_id");
                    dtReport.Columns.Remove("network_id");
                    dtReport.Columns.Remove("parent_system_id");
                    dtReport.Columns.Remove("parent_network_id");
                    dtReport.Columns.Remove("parent_entity_type");
                    dtReport.Columns.Remove("port_name");
                    dtReport.Columns.Remove("destination_system_id");
                    dtReport.Columns.Remove("destination_network_id");

                    dtReport.Columns.Remove("destination_entity_type");
                    dtReport.Columns.Remove("created_by");
                    dtReport.Columns.Remove("modified_by");
                    dtReport.Columns.Remove("modified_on");
                    dtReport.Columns.Remove("port_status_id");
                    dtReport.Columns.Remove("type");
                    dtReport.Columns.Remove("CREATED_ON");
                    dtReport.Columns.Remove("VIEWPORTSTATUSCOUNT");

                    dtReport.Columns["port_number"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_026;
                    dtReport.Columns["port_type"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_027;
                    dtReport.Columns["input_output"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_028;
                    dtReport.Columns["port_status"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_029;
                    dtReport.Columns["comment"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_312;

                    var filename = entity_type + "Port Details";
                    ExportData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                    objResp = "ok";
                }

                else
                {
                    objResp = "Not Found Data";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public PartialViewResult GetFiberLinkAttachment(int system_Id, string entity_type, string featureName)
        {
            var lstDocument = new BLAttachment().getAttachmentDetails(system_Id, entity_type, "Document", featureName);
            //converting file size
            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            return PartialView("_viewFiberLinkAttachment", lstDocument);
        }

        #region Accessories
        public PartialViewResult GetAccessories(AccessoriesViewModel model, int page = 0, string sort = "", string sortdir = "", string parent_networkStatus = "")
        {
            //dynamic parentDetail = GetPop(model.parent_systemId, model.parent_eType);
            //AccessoriesModel objAccessories = new AccessoriesModel();
            // BindAccessoriesSearchBy(model);
            //BIND VENDOR SPECIFICATION LIST WITH PAGING
            model.objFilterAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            model.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            model.objFilterAttributes.sort = sort;
            model.objFilterAttributes.orderBy = sortdir;
            model.objFilterAttributes.parent_systemId = model.parent_systemId;
            model.objFilterAttributes.parent_entityType = model.parent_eType;
            model.parent_network_status = parent_networkStatus;
            model.lstData = new BLMisc().GetAccessoriesByParent(model);
            model.objFilterAttributes.totalRecord = model.lstData != null && model.lstData.Count > 0 ? model.lstData[0].totalRecords : 0;
            //model.parent_network_id = parentDetail.network_id;
            Session["viewAccessories"] = model.objFilterAttributes;
            return PartialView("_ViewAccessories", model);
        }
        public void DownloadAccessories()
        {
            if (Session["viewAccessories"] != null)
            {
                FilterAccessoriesAttr filterData = (FilterAccessoriesAttr)Session["viewAccessories"];
                filterData.currentPage = 0;
                filterData.pageSize = 0;
                //List<Dictionary<string, string>> lstReportData = new BLMisc().GetAuditLMCInfoById(objViewFilter.LMCId, objViewFilter.lmcType.ToUpper(), objViewFilter.currentPage, objViewFilter.pageSize, objViewFilter.sort, objViewFilter.orderBy);
                List<AccessoriesReportModel> lstReportData = new BLMisc().GetAccessoriesByParent(filterData);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable(lstReportData);

                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                    if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }
                    if (dtReport.Columns.Contains("system_id")) { dtReport.Columns.Remove("system_id"); }

                    if (dtReport.Columns.Contains("entity_type")) { dtReport.Columns["entity_type"].ColumnName = "Entity Type"; }
                    if (dtReport.Columns.Contains("quantity")) { dtReport.Columns["quantity"].ColumnName = "Quantity"; }
                    if (dtReport.Columns.Contains("remarks")) { dtReport.Columns["remarks"].ColumnName = "Remark"; }
                    if (dtReport.Columns.Contains("specification")) { dtReport.Columns["specification"].ColumnName = "Specification"; }
                    if (dtReport.Columns.Contains("vendor_name")) { dtReport.Columns["vendor_name"].ColumnName = "Vendor Name"; }
                    if (dtReport.Columns.Contains("item_code")) { dtReport.Columns["item_code"].ColumnName = "Item Code"; }
                    if (dtReport.Columns.Contains("category")) { dtReport.Columns["category"].ColumnName = "Category"; }
                    if (dtReport.Columns.Contains("subcategory1")) { dtReport.Columns["subcategory1"].ColumnName = "Subcategory 1"; }
                    if (dtReport.Columns.Contains("subcategory2")) { dtReport.Columns["subcategory2"].ColumnName = "Subcategory 2"; }
                    if (dtReport.Columns.Contains("subcategory3")) { dtReport.Columns["subcategory3"].ColumnName = "Subcategory 3"; }
                    if (dtReport.Columns.Contains("parent_network_id")) { dtReport.Columns["parent_network_id"].ColumnName = "Parent Network Id"; }
                    if (dtReport.Columns.Contains("parent_entity_type")) { dtReport.Columns["parent_entity_type"].ColumnName = "Parent Entity Type"; }
                    if (dtReport.Columns.Contains("status")) { dtReport.Columns["status"].ColumnName = "Status"; }
                    if (dtReport.Columns.Contains("network_status")) { dtReport.Columns["network_status"].ColumnName = "Network Status"; }
                    if (dtReport.Columns.Contains("created_by_text")) { dtReport.Columns["created_by_text"].ColumnName = "Created By"; }
                    if (dtReport.Columns.Contains("created_on")) { dtReport.Columns["created_on"].ColumnName = "Created On"; }
                    if (dtReport.Columns.Contains("modified_by_text")) { dtReport.Columns["modified_by_text"].ColumnName = "Modified By"; }
                    if (dtReport.Columns.Contains("modified_on")) { dtReport.Columns["modified_on"].ColumnName = "Modified On"; }



                }
                if (dtReport.Rows.Count > 0)
                {
                    ExportData(dtReport, "Export_Accossories_" + Utility.MiscHelper.getTimeStamp());
                }
            }
        }
        public PartialViewResult AddAccessories(int systemId = 0, int parentSystemId = 0, string parentEType = "", string parentNetworkId = "", string parent_networkStatus = "", string geom = "", string geomType = "")
        {

            AccessoriesInfoModel accessories = GetAccessoriesDetail(systemId, geom, geomType, parentEType);
            if (systemId == 0)
            {
                accessories.parent_entity_type = parentEType;
                accessories.parent_system_id = parentSystemId;
                accessories.parent_network_id = parentNetworkId;
                accessories.parent_network_status = parent_networkStatus;

            }
            return PartialView("_AddAccessories", accessories);
        }
        public ActionResult SaveAccessories(AccessoriesInfoModel objAccessoriesInfoModel, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();

            if (TryValidateModel(objAccessoriesInfoModel))
            {

                var isNew = objAccessoriesInfoModel.system_id > 0 ? false : true;
                bool isDuplicate = new BLAccessories().ChkDuplicateAccessoriesBySpecification(objAccessoriesInfoModel);


                if (isNew == false && !isDuplicate)
                {
                    //for Diffrent Accessories Update
                    var resultItem = new BLAccessories().SaveAccessories(objAccessoriesInfoModel, Convert.ToInt32(Session["user_id"]));
                    objAccessoriesInfoModel.system_id = resultItem.system_id;
                    objAccessoriesInfoModel.created_on = resultItem.created_on;
                    objAccessoriesInfoModel.status = resultItem.status;
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = "Accessories updated successfully!";
                }
                else
                {
                    // bool isDuplicate = new BLAccessories().ChkDuplicateAccessoriesBySpecification(objAccessoriesInfoModel);
                    if (!isDuplicate && isNew)
                    {
                        //for saving
                        var resultItem = new BLAccessories().SaveAccessories(objAccessoriesInfoModel, Convert.ToInt32(Session["user_id"]));
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.message = "Accessories saved successfully!";
                    }
                    else
                    {
                        //for Same Accessories Update
                        objPM.status = ResponseStatus.FAILED.ToString();
                        objPM.message = "Accessories with same Vendor and Specification already exist";
                    }
                }
            }
            //}
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = getFirstErrorFromModelState();

            }
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(objAccessoriesInfoModel.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {


                // RETURN PARTIAL VIEW WITH MODEL DATA
                //input = GetAccessoriesDetail(input.system_id, input.parent_entity_type);
                objAccessoriesInfoModel.objPM = objPM;
                return PartialView("_AddAccessories", objAccessoriesInfoModel);
            }
        }
        [NonAction]
        public AccessoriesInfoModel GetAccessoriesDetail(int systemId, string geom, string geomType, string parent_type = "")
        {
            AccessoriesInfoModel accessories = new AccessoriesInfoModel();
            BLAccessories blAccessories = new BLAccessories();

            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                if (!string.IsNullOrEmpty(geom)) { fillRegionProvinceDetail(accessories, geomType, geom); }

            }
            else
            {
                // Get entity detail by Id...
                accessories = blAccessories.GeteAccessoriesById(systemId);
                accessories.accessories_template.lstSpecification = BLItemTemplate.Instance.GetAccessoriesSpecification(accessories.accessories_id);
                accessories.accessories_template.lstVendor = BLItemTemplate.Instance.GetVendorList(accessories.specification);
                accessories.accessories_template.item_code = accessories.item_code;
                accessories.accessories_template.subcategory1 = accessories.subcategory1;
                accessories.accessories_template.subcategory2 = accessories.subcategory2;

                accessories.accessories_template.subcategory3 = accessories.subcategory3;
                accessories.accessories_template.category = accessories.category;
                accessories.accessories_template.specification = accessories.specification;
                accessories.accessories_template.vendor_id = accessories.vendor_id;
                accessories.accessories_template.audit_item_master_id = accessories.audit_item_master_id;
            }
            accessories.listTypes = blAccessories.GetAccesoriesTypeByLayeKey(parent_type);
            return accessories;
        }

        public JsonResult DeleteAccessoriesById(int systemId)
        {
            //var result = 0;
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.ADB.ToString());
            //if (isNotAssociated == true) { result = new BLADB().DeleteADBById(systemId); }
            // DbMessage response = new DbMessage();
            int response = new BLAccessories().DeleteAccessoriesById(systemId);
            if (response > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Accessories deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Accessories could not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAccessories_NetworkStatus(int SystemId, string NetworkStatus)
        {
            PageMessage objPM = new PageMessage();
            int response = new BLAccessories().UpdateAccessoriesNetworkStatus(SystemId, NetworkStatus);
            if (response != 0)
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = "Network Status of Accessories updated successfully!";
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = "Unable to update Network Status!";
            }

            return Json(objPM, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region COMPETITOR
        public PartialViewResult AddCompetitor(string networkIdType, int systemId = 0, string geom = "")
        {
            //Competitor objCompetitor = GetCompetitorDetail(networkIdType, systemId, geom);
            //BindCompetitorIcons(objCompetitor);
            //return PartialView("_AddCompetitor", objCompetitor);
            Competitor obj = new Competitor();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<Competitor>(url, obj, EntityType.Competitor.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddCompetitor", response.results);
        }
        public Competitor GetCompetitorDetail(string networkIdType, int systemId, string geom = "")
        {
            Competitor objCompetitor = new Competitor();
            objCompetitor.geom = geom;
            objCompetitor.networkIdType = networkIdType;
            var userdetails = (User)Session["userDetail"];
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objCompetitor, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objCompetitor, new NetworkCodeIn() { eType = EntityType.Competitor.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCompetitor.geom }, networkIdType);
                // fill latlong values
                string[] lnglat = geom.Split(new string[] { " " }, StringSplitOptions.None);
                objCompetitor.latitude = Convert.ToDouble(lnglat[1].ToString());
                objCompetitor.longitude = Convert.ToDouble(lnglat[0].ToString());

            }
            else
            {
                // Get entity detail by Id...
                objCompetitor = new BLMisc().GetEntityDetailById<Competitor>(systemId, EntityType.Competitor);
            }
            objCompetitor.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objCompetitor;
        }


        public ActionResult SaveCompetitor(Competitor objCompetitor, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();

            //if (objCompetitor.networkIdType == NetworkIdType.A.ToString() && objCompetitor.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Competitor.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCompetitor.geom });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objCompetitor = GetCompetitorDetail(objCompetitor.networkIdType, objCompetitor.system_id, objCompetitor.geom);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objCompetitor.name = objNetworkCodeDetail.network_code;
            //    }
            //    //SET NETWORK CODE
            //    objCompetitor.network_id = objNetworkCodeDetail.network_code;
            //    objCompetitor.sequence_id = objNetworkCodeDetail.sequence_id;
            //    // fill latlong values
            //    string[] lnglat = objCompetitor.geom.Split(new string[] { " " }, StringSplitOptions.None);
            //    objCompetitor.latitude = Convert.ToDouble(lnglat[1].ToString());
            //    objCompetitor.longitude = Convert.ToDouble(lnglat[0].ToString());
            //}

            //if (TryValidateModel(objCompetitor))
            //{
            //    string[] LayerName = { EntityType.Competitor.ToString() };
            //    var isNew = objCompetitor.system_id > 0 ? false : true;
            //    var resultItem = new BLCompetitor().SaveCompetitor(objCompetitor, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        else
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //        objCompetitor.objPM = objPM;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    objCompetitor.objPM = objPM;
            //}
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objCompetitor.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BindCompetitorIcons(objCompetitor);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    return PartialView("_AddCompetitor", objCompetitor);
            //}
            objCompetitor.isDirectSave = isDirectSave;
            objCompetitor.user_id = Convert.ToInt32(Session["user_id"]);
            if (objCompetitor.icon_path == null)
            {
                objCompetitor.icon_path = "3.png";
            }

            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<Competitor>(url, objCompetitor, EntityType.Competitor.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddCompetitor", response.results);

        }
        private void BindCompetitorIcons(Competitor objCompetitor)
        {
            /// GET COMPETITOR IMAGES FROM FOLDER AND BIND---
            var CompetitorImagePath = ConfigurationManager.AppSettings["CompetitorImagePath"];
            var imageFiles = Directory.GetFiles(Server.MapPath(CompetitorImagePath));

            foreach (var item in imageFiles)
            {
                var iconName = Path.GetFileName(item);
                objCompetitor.lstIcons.Add(iconName.ToString());
            }
            // END
        }
        #endregion

        #region Help & FAQs
        public ActionResult HelpPage()
        {
            // GET FAQ Details
            vmHelp objvmHelp = new vmHelp();
            var userdetails = (User)Session["userDetail"];
            objvmHelp.lstFAQMaster = new BLHelp().getFAQ();
            objvmHelp.lstFAQUserManual = new BLHelp().getUserManual();
            objvmHelp.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            foreach (var item in objvmHelp.lstFAQUserManual)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            return PartialView("_Help", objvmHelp);
        }

        #endregion


        //LineEntityIn objIn, int pSystemId = 0, string pEntityType = "", string pNetworkId = ""
        //public PartialViewResult AddMicroduct(LineEntityIn objIn, string pSystemId, string pEntityType, string pGeomType, string geom = "", string entityType = "", int systemId = 0)
        //{

        //	MicroductMaster objDuct = new MicroductMaster();

        //	objDuct = GetDetail(objIn);
        //	if (objIn.systemId == 0 && !String.IsNullOrEmpty(pSystemId))
        //	{
        //		//Fill Location detail...    
        //		GetLineNetworkDetail(objDuct, objIn, EntityType.Microduct.ToString(), false, Convert.ToInt32(pSystemId), pEntityType);
        //	}
        //	else if (objIn.systemId == 0)
        //	{
        //		//Fill Location detail...    
        //		GetLineNetworkDetail(objDuct, objIn, EntityType.Microduct.ToString(), false);
        //	}
        //	BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Microduct.ToString());
        //	fillProjectSpecifications(objDuct);
        //	BindMicroductDropDown(objDuct);
        //	//if (objDuct.parent_system_id!= 0)
        //	//{
        //	//    objDuct.network_id = objDuct.parent_network_id;
        //	//}
        //	objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Microduct.ToString()).ToList();
        //	//Get the layer details to bind additional attributes Microduct
        //	var layerdetails = new BLLayer().getLayer(EntityType.Microduct.ToString());
        //	objDuct.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
        //	//End for additional attributes Microduct
        //	return PartialView("_AddMicroduct", objDuct);
        //}
        //private void BindMicroductDropDown(MicroductMaster objDuctIn)
        //{
        //	var objDDL = new BLMisc().GetDropDownList(EntityType.Duct.ToString());
        //	objDuctIn.DuctColorIn = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Color.ToString()).ToList();

        //	var objDDLMicroduct = new BLMisc().GetDropDownList(EntityType.Microduct.ToString());
        //	objDuctIn.DuctTypeIn = objDDLMicroduct.Where(x => x.dropdown_type == DropDownType.Microduct_Type.ToString()).ToList();
        //	objDuctIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        //	objDuctIn.lstNoOfWays = objDDLMicroduct.Where(x => x.dropdown_type == DropDownType.Number_of_Ways.ToString()).ToList();
        //	objDuctIn.lstInternalDiameter = objDDLMicroduct.Where(x => x.dropdown_type == DropDownType.Internal_Diameter.ToString()).ToList();
        //	objDuctIn.lstExternalDiameter = objDDLMicroduct.Where(x => x.dropdown_type == DropDownType.External_Diameter.ToString()).ToList();
        //	objDuctIn.lstMaterialType = objDDLMicroduct.Where(x => x.dropdown_type == DropDownType.Material_Type.ToString()).ToList();
        //	var _objDDL = new BLMisc().GetDropDownList("");
        //	objDuctIn.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
        //	// objDuctIn.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
        //}
        //public MicroductMaster GetDetail(LineEntityIn objIn)
        //{
        //	MicroductMaster objDuct = new MicroductMaster();
        //	var usrDetail = (User)Session["userDetail"];
        //	if (objIn.systemId == 0 && objIn.pSystemId != 0)
        //	{
        //		objDuct = new BLMisc().GetEntityDetailById<MicroductMaster>(objIn.pSystemId, EntityType.Duct);
        //		objDuct.geom = objIn.geom;
        //		objDuct.parent_system_id = objDuct.system_id;
        //		objDuct.parent_entity_type = EntityType.Duct.ToString();
        //		objDuct.microduct_type = EntityType.Microduct.ToString();
        //		objDuct.a_system_id = objDuct.a_system_id;
        //		objDuct.a_entity_type = objDuct.a_entity_type;
        //		objDuct.a_location = objDuct.a_location;
        //		objDuct.b_system_id = objDuct.b_system_id;
        //		objDuct.b_entity_type = objDuct.b_entity_type;
        //		objDuct.b_location = objDuct.b_location;
        //		objDuct.trench_id = 0;
        //		objDuct.manual_length = objDuct.manual_length;
        //		objDuct.pin_code = objDuct.pin_code;
        //		objDuct.pEntityType = objIn.pEntityType;
        //		objDuct.pSystemId = objIn.pSystemId;
        //		objDuct.pNetworkId = objIn.pNetworkId;

        //		objDuct.parent_network_id = objDuct.network_id;

        //		objDuct.system_id = 0;
        //		objDuct.other_info = null;  //for additional-attributes
        //	}
        //	if (objIn.systemId == 0)
        //	{
        //		objDuct.geom = objIn.geom;
        //		if (!string.IsNullOrEmpty(objIn.geom))
        //			objDuct.calculated_length = Math.Round((double)new BLMisc().GetCableLength(objIn.geom), 3);

        //		objDuct.manual_length = objDuct.calculated_length;
        //		objDuct.networkIdType = objIn.networkIdType;
        //		objDuct.ownership_type = VendorType.Own.ToString();
        //		//NEW ENTITY->Fill Region and Province Detail..
        //		fillRegionProvinceDetail(objDuct, GeometryType.Line.ToString(), objIn.geom);

        //		// Item template binding
        //		var objItem = BLItemTemplate.Instance.GetTemplateDetail<MicroductTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Microduct);
        //		Utility.MiscHelper.CopyMatchingProperties(objItem, objDuct);
        //		objDuct.other_info = null;  //for additional-attributes
        //	}
        //	else
        //	{
        //		objDuct = new BLMisc().GetEntityDetailById<MicroductMaster>(objIn.systemId, EntityType.Microduct);
        //		//for additional-attributes
        //		objDuct.other_info = new BLMicroduct().GetOtherInfoMicroduct(objDuct.system_id);
        //		fillRegionProvAbbr(objDuct);
        //	}
        //	objDuct.user_id = usrDetail.user_id;
        //	objDuct.lstUserModule = new BLLayer().GetUserModuleAbbrList(objDuct.user_id, UserType.Web.ToString());
        //	return objDuct;
        //}




        //public ActionResult SaveMicroduct(MicroductMaster objDuct, bool isDirectSave = false, string pNetworkId = "")
        //{
        //	ModelState.Clear();

        //	PageMessage objPM = new PageMessage();
        //	int pSystemId = objDuct.pSystemId;
        //	if (objDuct.networkIdType == NetworkIdType.A.ToString() && objDuct.system_id == 0)
        //	{
        //		if (isDirectSave == false)
        //		{
        //			objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.a_system_id, network_id = objDuct.a_location, network_name = objDuct.a_entity_type });
        //			objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.b_system_id, network_id = objDuct.b_location, network_name = objDuct.b_entity_type });
        //		}
        //		var objLineEntity = new LineEntityIn() { geom = objDuct.geom, systemId = objDuct.system_id, networkIdType = objDuct.networkIdType, lstTP = objDuct.lstTP, pEntityType = objDuct.pEntityType, pNetworkId = objDuct.pNetworkId, pSystemId = objDuct.pSystemId };
        //		if (isDirectSave == true)
        //		{
        //			//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
        //			objDuct = GetDetail(objLineEntity);
        //			objDuct.trench_id = pSystemId;
        //			objDuct.pNetworkId = pNetworkId;
        //			var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
        //			// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
        //			objDuct.bom_sub_category = objBOMDDL[0].dropdown_value;
        //			//  objDuct.served_by_ring = objSubCatDDL[0].dropdown_value;
        //		}
        //		GetLineNetworkDetail(objDuct, objLineEntity, EntityType.Microduct.ToString(), true, objDuct.pSystemId, objDuct.pEntityType);
        //		if (isDirectSave == true)
        //			objDuct.network_name = objDuct.network_id;

        //	}
        //	if (string.IsNullOrEmpty(objDuct.network_name))
        //	{
        //		objDuct.network_name = objDuct.network_id;
        //	}
        //	if (TryValidateModel(objDuct))
        //	{

        //		var isNew = objDuct.system_id > 0 ? false : true;
        //		if (pSystemId == 0)
        //		{
        //			objDuct.trench_id = pSystemId;
        //		}
        //		var resultItem = new BLMicroduct().Save(objDuct, Convert.ToInt32(Session["user_id"]));
        //		if (string.IsNullOrEmpty(resultItem.objPM.message))
        //		{
        //			string[] LayerName = { EntityType.Microduct.ToString() };

        //			//Save Reference
        //			if (objDuct.EntityReference != null && resultItem.system_id > 0)
        //			{
        //				SaveReference(objDuct.EntityReference, resultItem.system_id);
        //			}
        //			if (isNew)
        //			{
        //				objPM.status = ResponseStatus.OK.ToString();
        //				objPM.isNewEntity = isNew;
        //				objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
        //			}
        //			else
        //			{
        //				objPM.status = ResponseStatus.OK.ToString();
        //				objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
        //				BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Microduct.ToString());
        //			}
        //			objDuct.objPM = objPM;
        //		}

        //		//save AT Status                        
        //		if (objDuct.ATAcceptance != null && objDuct.system_id > 0)
        //		{
        //			SaveATAcceptance(objDuct.ATAcceptance, objDuct.system_id);
        //		}

        //	}
        //	else
        //	{
        //		objPM.status = ResponseStatus.FAILED.ToString();
        //		objPM.message = getFirstErrorFromModelState();
        //		objDuct.objPM = objPM;
        //	}
        //	if (isDirectSave == true)
        //	{
        //		//RETURN MESSAGE AS JSON FOR DIRECT SAVE
        //		return Json(objDuct.objPM, JsonRequestBehavior.AllowGet);
        //	}
        //	else
        //	{
        //		BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Microduct.ToString());
        //		// RETURN PARTIAL VIEW WITH MODEL DATA
        //		fillProjectSpecifications(objDuct);
        //		BindMicroductDropDown(objDuct);
        //		objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Microduct.ToString()).ToList();
        //		//Get the layer details to bind additional attributes Microduct
        //		var layerdetails = new BLLayer().getLayer(EntityType.Microduct.ToString());
        //		objDuct.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
        //		//End for additional attributes Microduct
        //		return PartialView("_AddMicroduct", objDuct);
        //	}
        //}
        public ActionResult SaveMicrowavelink(MicroductMaster objEntityMaster, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();

            // get parent geometry 
            if (string.IsNullOrWhiteSpace(objEntityMaster.geom) && objEntityMaster.system_id == 0)
            {
                objEntityMaster.geom = objBLCommon.GetPointTypeParentGeom(objEntityMaster.pSystemId, objEntityMaster.pEntityType);
            }

            if (objEntityMaster.networkIdType == NetworkIdType.A.ToString() && objEntityMaster.system_id == 0)
            {
                //GET AUTO NETWORK CODE...
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.MicrowaveLink.ToString(), gType = GeometryType.Line.ToString(), eGeom = objEntityMaster.geom, parent_eType = objEntityMaster.pEntityType, parent_sysId = objEntityMaster.pSystemId });
                if (isDirectSave == true)
                {
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    //objEntityMaster = GetAntennaDetail(objEntityMaster.pSystemId, objEntityMaster.pEntityType, objEntityMaster.networkIdType, objEntityMaster.system_id, objEntityMaster.geom);
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    objEntityMaster.microduct_name = objNetworkCodeDetail.network_code;
                }
                //SET NETWORK CODE
                objEntityMaster.network_id = objNetworkCodeDetail.network_code;
                objEntityMaster.sequence_id = objNetworkCodeDetail.sequence_id;

            }

            //objEntityMaster.microwaveLinkMaster.item_code = objEntityMaster.item_code;
            //objEntityMaster.microwaveLinkMaster.vendor_id = objEntityMaster.vendor_id;
            //objEntityMaster.microwaveLinkMaster.specification = objEntityMaster.specification;
            //objEntityMaster.microwaveLinkMaster.ownership_type = objEntityMaster.ownership_type;



            if (TryValidateModel(objEntityMaster))
            {
                var isNew = objEntityMaster.system_id > 0 ? false : true;

                var resultItem = new BLMicroduct().Save(objEntityMaster, Convert.ToInt32(Session["user_id"]));
                InstallationInfo installationInfo = objCommon.GetInstallationInfo(objEntityMaster);
                installationInfo = new BLInstallationInfo().Save(installationInfo);
                if (String.IsNullOrEmpty(resultItem.objPM.message))
                {


                    //Save Reference
                    //if (objEntityMaster.EntityReference != null && resultItem.system_id > 0)
                    //{
                    //	objcommon.SaveReference(objEntityMaster.EntityReference, resultItem.system_id);
                    //}
                    string[] LayerName = { EntityType.MicrowaveLink.ToString() };
                    if (isNew)
                    {
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.isNewEntity = isNew;
                        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                    }
                    else
                    {
                        //if (resultItem.isPortConnected == true)
                        //{
                        //    objPM.status = ResponseStatus.OK.ToString();
                        //    objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
                        //}
                        //else
                        //{
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                        //}
                    }
                    objEntityMaster.objPM = objPM;
                }
            }
            else
            {

                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = new LibraryController().getFirstErrorFromModelState();
                objEntityMaster.objPM = objPM;
            }
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(objEntityMaster.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {
                BLItemTemplate.Instance.BindItemDropdowns(objEntityMaster, EntityType.MicrowaveLink.ToString());
                BindMicrowaveDropDown(objEntityMaster);
                // RETURN PARTIAL VIEW WITH MODEL DATA
                objBLCommon.fillProjectSpecifications(objEntityMaster);
                return PartialView("_AddMicroDuct", objEntityMaster);
            }
        }
        public void BindMicrowaveDropDown(MicroductMaster objEntityMaster)
        {
            var objDDL = objBLMisc.GetDropDownList(EntityType.MicrowaveLink.ToString());
            //objEntityMaster.LstLinkType = objDDL.Where(x => x.dropdown_type == DropDownType.LinkType.ToString()).ToList();
            //objEntityMaster.lstSubAntennaType = objDDL.Where(x => x.dropdown_type == DropDownType.Sub_Antenna_Type.ToString()).ToList();
            //objEntityMaster.lstAntennaOperator = objDDL.Where(x => x.dropdown_type == DropDownType.Antenna_Operator.ToString()).ToList();
            //objEntityMaster.lstUsePattern = objDDL.Where(x => x.dropdown_type == DropDownType.Use_Pattern.ToString()).ToList();
            objEntityMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objEntityMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }
        public PartialViewResult AdditionalAttributes(int systemId, string entityType)
        {
            PODMaster objPOD = new PODMaster();
            objPOD.extraAttributes = new BLAdditionalAttributes().getAttributes(systemId, entityType);
            if (objPOD.extraAttributes != null && objPOD.extraAttributes.rfs_date != null)
            {
                objPOD.extraAttributes.rfsSetDate = MiscHelper.FormatDate(objPOD.extraAttributes.rfs_date.ToString());


            }

            return PartialView("_AdditionalAttributes", objPOD);
        }

        //cabinet shazia 
        #region cabinet
        public CabinetMaster GetCabinetDetail(string networkIdType, int systemId, string geom = "")
        {
            CabinetMaster objCabinet = new CabinetMaster();
            objCabinet.geom = geom;
            objCabinet.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objCabinet, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objCabinet, new NetworkCodeIn() { eType = EntityType.Cabinet.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCabinet.geom }, networkIdType);
                objCabinet.longitude = Convert.ToDecimal(geom.Split(' ')[0]);
                objCabinet.latitude = Convert.ToDecimal(geom.Split(' ')[1]);
                objCabinet.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<CabinetTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Cabinet);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objCabinet);
            }
            else
            {
                // Get entity detail by Id...
                objCabinet = new BLMisc().GetEntityDetailById<CabinetMaster>(systemId, EntityType.Cabinet);
            }
            return objCabinet;
        }
        public PartialViewResult AddCabinet(string networkIdType, int systemId = 0, string geom = "")
        {
            CabinetMaster obj = new CabinetMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CabinetMaster>(url, obj, EntityType.Cabinet.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddCabinet", response.results);
        }
        private void BindCabinetDropDown(CabinetMaster objCabinet)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objCabinet.parent_system_id, objCabinet.system_id, EntityType.Cabinet.ToString());
            if (ispEntityMap != null)
            {
                objCabinet.objIspEntityMap.id = ispEntityMap.id;
                objCabinet.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objCabinet.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objCabinet.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objCabinet.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }

            objCabinet.objIspEntityMap.AssoType = objCabinet.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objCabinet.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objCabinet.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objCabinet.longitude + " " + objCabinet.latitude);
            if (objCabinet.objIspEntityMap.structure_id > 0)
            {
                var objDDL = new BLBDB().GetShaftFloorByStrucId(objCabinet.objIspEntityMap.structure_id);
                objCabinet.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
                objCabinet.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

                if (objCabinet.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objCabinet.objIspEntityMap.unitId = objCabinet.parent_system_id;
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.Cabinet.ToString()).FirstOrDefault() != null)
            {
                objCabinet.objIspEntityMap.isValidParent = true;
                objCabinet.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objCabinet.objIspEntityMap.structure_id, objCabinet.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.Cabinet.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objCabinet.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objCabinet.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            if (objCabinet.objIspEntityMap.entity_type == null) { objCabinet.objIspEntityMap.entity_type = EntityType.Cabinet.ToString(); }
            objCabinet.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objCabinet.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
            var obj_DDL = new BLMisc().GetDropDownList(EntityType.Cabinet.ToString());
            objCabinet.listCabinetType = obj_DDL.Where(x => x.dropdown_type == DropDownType.Cabinet_Type.ToString()).ToList();
        }
        public ActionResult SaveCabinet(CabinetMaster objCabinetMaster, bool isDirectSave = false)
        {
            objCabinetMaster.isDirectSave = isDirectSave;
            objCabinetMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objCabinetMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objCabinetMaster.source_ref_type = "NETWORK_TICKET";
                objCabinetMaster.status = "D";
            }
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CabinetMaster>(url, objCabinetMaster, EntityType.Cabinet.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddCabinet", response.results);
        }
        public JsonResult DeleteCabinetById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            var result = 0;

            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.Cabinet.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Cabinet has deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Cabinet can not be deleted as it is associated or used!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //cabinet shazia 
        public PartialViewResult GetPODAssociationDetail(PODAssociation obj)
        {

            obj.lstPODAssociation = new BLPOD().GetPODAssociationDetail(obj.geom, obj.associated_system_id, obj.associated_entity_Type);
            return PartialView("POD_Association", obj);

        }
        //siteid
        public PartialViewResult GetSiteProjectDetail(SiteProjectDetails obj)
        {
            SiteProjectDetails pRojectAssociation = new SiteProjectDetails();
            pRojectAssociation.lstSiteProjectDetails = new BLProject().GetProjectByDetails(obj.site_id); // Replace with your actual data access

            //obj.lstPODAssociation = new BLPOD().GetPODAssociationDetail(obj.geom, obj.associated_system_id, obj.associated_entity_Type);
            return PartialView("PODProject", pRojectAssociation);

        }
        //Vaultt shazia 
        #region vault
        public VaultMaster GetVaultDetail(string networkIdType, int systemId, string geom = "")
        {
            VaultMaster objVault = new VaultMaster();
            var userdetails = (User)Session["userDetail"];
            objVault.geom = geom;
            objVault.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objVault, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objVault, new NetworkCodeIn() { eType = EntityType.Vault.ToString(), gType = GeometryType.Point.ToString(), eGeom = objVault.geom }, networkIdType);
                objVault.longitude = Convert.ToDecimal(geom.Split(' ')[0]);
                objVault.latitude = Convert.ToDecimal(geom.Split(' ')[1]);
                objVault.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<VaultTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Vault);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objVault);
            }
            else
            {
                // Get entity detail by Id...
                objVault = new BLMisc().GetEntityDetailById<VaultMaster>(systemId, EntityType.Vault);
            }
            objVault.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return objVault;
        }
        public PartialViewResult AddVault(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            VaultMaster obj = new VaultMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<VaultMaster>(url, obj, EntityType.Vault.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddVault", response.results);
        }
        private void BindVaultDropDown(VaultMaster objVault)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objVault.parent_system_id, objVault.system_id, EntityType.Vault.ToString());
            if (ispEntityMap != null)
            {
                objVault.objIspEntityMap.id = ispEntityMap.id;
                objVault.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objVault.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                objVault.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                objVault.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
            }

            objVault.objIspEntityMap.AssoType = objVault.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objVault.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            objVault.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objVault.longitude + " " + objVault.latitude);
            if (objVault.objIspEntityMap.structure_id > 0)
            {
                var objDDL = new BLBDB().GetShaftFloorByStrucId(objVault.objIspEntityMap.structure_id);
                objVault.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
                objVault.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

                if (objVault.parent_entity_type == EntityType.UNIT.ToString())
                {
                    objVault.objIspEntityMap.unitId = objVault.parent_system_id;
                }
            }
            var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
            if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.Vault.ToString()).FirstOrDefault() != null)
            {
                objVault.objIspEntityMap.isValidParent = true;
                objVault.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objVault.objIspEntityMap.structure_id, objVault.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
            }
            var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.Vault.ToString().ToUpper()).FirstOrDefault();
            if (layerDetails != null)
            {
                objVault.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
                objVault.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
            }
            if (objVault.objIspEntityMap.entity_type == null) { objVault.objIspEntityMap.entity_type = EntityType.Vault.ToString(); }
            objVault.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objVault.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
            var obj_DDL = new BLMisc().GetDropDownList(EntityType.Vault.ToString());
            objVault.listVaultType = obj_DDL.Where(x => x.dropdown_type == DropDownType.Vault_Type.ToString()).ToList();
        }
        public ActionResult SaveVault(VaultMaster objVaultMaster, bool isDirectSave = false)
        {
            objVaultMaster.isDirectSave = isDirectSave;
            objVaultMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objVaultMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objVaultMaster.source_ref_type = "NETWORK_TICKET";
                objVaultMaster.status = "D";
            }
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<VaultMaster>(url, objVaultMaster, EntityType.Vault.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddVault", response.results);
        }
        public JsonResult DeleteVaultById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            var result = 0;

            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.Vault.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Vault has deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Vault can not be deleted as it is associated or used!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //vault shazia

        #region Loop Entity
        public PartialViewResult AddLoop(string networkIdType, int systemId = 0, string geom = "")
        {
            NELoopDetails obj = new NELoopDetails();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<NELoopDetails>(url, obj, EntityType.Loop.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddLoop", response.results);
        }

        public NELoopDetails GetLoopDetail(string networkIdType, int systemId, string geom = "")
        {
            NELoopDetails objLoop = new NELoopDetails();
            if (systemId == 0)
            {
                objLoop.geom = geom;
                objLoop.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objLoop.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objLoop.networkIdType = networkIdType;
                objLoop.lstLoopMangment = BLLoopMangment.Instance.GetLoopDetails(objLoop.longitude, objLoop.latitude, objLoop.associated_system_id, "Cable", objLoop.structure_id);
                //objLoop.networkIdType = networkIdType;

                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objLoop, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objLoop, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objLoop.geom }, networkIdType);
            }
            else
            {
                // Get entity detail by Id...
                objLoop = new BLMisc().GetEntityDetailById<NELoopDetails>(systemId, EntityType.Loop, Convert.ToInt32(Session["user_id"]));
                objLoop.lstLoopMangment = BLLoopMangment.Instance.GetLoopDetails(objLoop.longitude, objLoop.latitude, objLoop.associated_system_id, objLoop.associated_System_Type, objLoop.structure_id);
            }
            return objLoop;
        }
        public JsonResult GetCableNameAndLengthForLoop(int CableId)
        {
            CableMaster obj = new CableMaster();
            obj = new BLCable().GetCableNameAndLengthForLoop(CableId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveLoop(NELoopDetails objLoop, bool isDirectSave = false)
        {
            objLoop.isDirectSave = isDirectSave;
            objLoop.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<NELoopDetails>(url, objLoop, EntityType.Loop.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddLoop", response.results);
        }

        #endregion


        //Tower shazia 
        #region Tower
        public TowerMaster GetTowerDetail(int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
        {

            TowerMaster objEntity = new TowerMaster();
            objEntity.geom = geom;
            objEntity.networkIdType = networkIdType;
            if (systemId == 0)
            {
                objEntity.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objEntity.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objEntity.ownership_type = "Own";
                //NEW ENTITY->Fill Region and Province Detail..
                objBLCommon.fillRegionProvinceDetail(objEntity, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                objBLCommon.fillParentDetail(objEntity, new NetworkCodeIn() { eType = EntityType.Tower.ToString(), gType = GeometryType.Point.ToString(), eGeom = objEntity.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType);
                //Item template binding
                var layerDetails = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.Antenna.ToString().ToUpper()).FirstOrDefault();
                if (layerDetails.is_template_required)
                {
                    var objItem = BLItemTemplate.Instance.GetTemplateDetail<TowerTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Tower);
                    Utility.MiscHelper.CopyMatchingProperties(objItem, objEntity);
                }
            }
            else
            {
                objEntity = objBLMisc.GetEntityDetailById<TowerMaster>(systemId, EntityType.Tower);
                objEntity.lstTowerAssociatedPop = new BLTowerAssociatedPop().GetAssociatedPop(systemId);
            }
            return objEntity;
        }
        public PartialViewResult AddTower(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            TowerMaster obj = new TowerMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                obj.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                obj.source_ref_type = "NETWORK_TICKET";
                obj.status = "D";
            }
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TowerMaster>(url, obj, EntityType.Tower.ToString(), EntityAction.Get.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            return PartialView("~/Views/Tower/_AddTower.cshtml", response.results);
        }
        public ActionResult SaveTower(TowerMaster objEntityMaster, bool isDirectSave = false)
        {

            objEntityMaster.isDirectSave = isDirectSave;
            objEntityMaster.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TowerMaster>(url, objEntityMaster, EntityType.Tower.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("~/Views/Tower/_AddTower.cshtml", response.results);
        }

        //public ActionResult SaveTower(TowerMaster objEntityMaster, bool isDirectSave = false)
        //{
        //    ModelState.Clear();
        //    PageMessage objPM = new PageMessage();

        //    // get parent geometry 
        //    if (string.IsNullOrWhiteSpace(objEntityMaster.geom) && objEntityMaster.system_id == 0)
        //    {
        //        objEntityMaster.geom = objCommon.GetPointTypeParentGeom(objEntityMaster.pSystemId, objEntityMaster.pEntityType);
        //    }

        //    if (objEntityMaster.networkIdType == NetworkIdType.A.ToString() && objEntityMaster.system_id == 0)
        //    {
        //        //GET AUTO NETWORK CODE...
        //        var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Tower.ToString(), gType = GeometryType.Point.ToString(), eGeom = objEntityMaster.geom, parent_eType = objEntityMaster.pEntityType, parent_sysId = objEntityMaster.pSystemId });
        //        if (isDirectSave == true)
        //        {
        //            //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
        //            objEntityMaster = GetTowerDetail(objEntityMaster.pSystemId, objEntityMaster.pEntityType, objEntityMaster.networkIdType, objEntityMaster.system_id, objEntityMaster.geom);
        //            // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
        //            objEntityMaster.network_name = objNetworkCodeDetail.network_code;
        //        }
        //        //SET NETWORK CODE
        //        objEntityMaster.network_id = objNetworkCodeDetail.network_code;
        //        objEntityMaster.sequence_id = objNetworkCodeDetail.sequence_id;
        //    }



        //    if (TryValidateModel(objEntityMaster))
        //    {
        //        var isNew = objEntityMaster.system_id > 0 ? false : true;

        //        //var resultItem = new BLSplitter().SaveSplitterEntity(objEntityMaster, Convert.ToInt32(Session["user_id"]));
        //        TowerMaster resultItem = new BLTower().Save(objEntityMaster, Convert.ToInt32(Session["user_id"]));
        //        //InstallationInfo installationInfo = objCommon.GetInstallationInfo(objEntityMaster);
        //        //installationInfo = new BLInstallationInfo().Save(installationInfo);
        //        if (String.IsNullOrEmpty(resultItem.objPM.message))
        //        {


        //            //Save Reference
        //            if (objEntityMaster.EntityReference != null && resultItem.system_id > 0)
        //            {
        //                objCommon.SaveReference(objEntityMaster.EntityReference, resultItem.system_id);
        //            }
        //            string[] LayerName = { EntityType.Tower.ToString() };
        //            if (isNew)
        //            {
        //                objPM.status = ResponseStatus.OK.ToString();
        //                objPM.isNewEntity = isNew;
        //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
        //            }
        //            else
        //            {
        //                //if (resultItem.isPortConnected == true)
        //                //{
        //                //    objPM.status = ResponseStatus.OK.ToString();
        //                //    objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
        //                //}
        //                //else
        //                //{
        //                //    objPM.status = ResponseStatus.OK.ToString();
        //                //    objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
        //                //}

        //                objPM.status = ResponseStatus.OK.ToString();
        //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);

        //            }
        //            objEntityMaster.objPM = objPM;
        //        }
        //    }
        //    else
        //    {

        //        objPM.status = ResponseStatus.FAILED.ToString();
        //        objPM.message = new LibraryController().getFirstErrorFromModelState();
        //        objEntityMaster.objPM = objPM;
        //    }
        //    if (isDirectSave == true)
        //    {
        //        //RETURN MESSAGE AS JSON FOR DIRECT SAVE
        //        return Json(objEntityMaster.objPM, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        BLItemTemplate.Instance.BindItemDropdowns(objEntityMaster, EntityType.Tower.ToString());
        //        BindAntennaDropDown(objEntityMaster);
        //        // RETURN PARTIAL VIEW WITH MODEL DATA
        //        objCommon.fillProjectSpecifications(objEntityMaster);
        //        return PartialView("_AddTower", objEntityMaster);
        //    }
        //}
        public void BindAntennaDropDown(TowerMaster objEntityMaster)
        {
            var objDDL = objBLMisc.GetDropDownList(EntityType.Building.ToString());
            objEntityMaster.lstTenancy = objDDL.Where(x => x.dropdown_type == DropDownType.Tenancy.ToString()).ToList();

            objEntityMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objEntityMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
        }

        public JsonResult DeleteTowerById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            var result = 0;

            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.Tower.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Tower has deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Tower can not be deleted as it is associated or used!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //tower shazia

        public PartialViewResult GetPodDetailsInBulk(PODAssociation obj, string entity_sub_type)
        {
            obj.lstPODAssociation = new BLPOD().GetPodDetailsInBulk(obj.geom);
            obj.entity_sub_type = entity_sub_type;
            return PartialView("_BulkPODAssociation", obj);

        }

        //HANDHOLE BY ANTRA

        #region Handhole
        public PartialViewResult AddHandhole(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            HandholeMaster obj = new HandholeMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.pEntityType = pEntityType;
            obj.pSystemId = pSystemId;
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HandholeMaster>(url, obj, EntityType.Handhole.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddHandhole", response.results);
        }
        public ActionResult SaveHandhole(HandholeMaster objHandholeMaster, bool isDirectSave = false)
        {

            objHandholeMaster.isDirectSave = isDirectSave;
            objHandholeMaster.user_id = Convert.ToInt32(Session["user_id"]);
            var NWTicketDetails = new NetworkTicket();
            if (Session["NWTicketDetails"] != null)
            {
                NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                objHandholeMaster.source_ref_id = Convert.ToString(NWTicketDetails.ticket_id);
                objHandholeMaster.source_ref_type = "NETWORK_TICKET";
                objHandholeMaster.status = "D";
            }
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HandholeMaster>(url, objHandholeMaster, EntityType.Handhole.ToString(), EntityAction.Save.ToString());
            if (Session["NWTicketDetails"] != null)
            {
                DataTable DT = new BLNetworkTicket().GetNetworkTicketDetailsById(NWTicketDetails.ticket_id);
                NWTicketDetails.ticket_status = DT.Rows[0]["Ticket_Status"].ToString();
                Session["NWTicketDetails"] = NWTicketDetails;

            }
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_AddHandhole", response.results);
        }
        #endregion

        //END HANDHOLE

        //PatchPanel by shazia 
        #region PatchPanel
        public PatchPanelMaster GetPatchPanelDetail(int pSystemId, string pEntityType, string networkIdType, int systemId, string geom = "")
        {
            var userdetails = (User)Session["userDetail"];
            PatchPanelMaster objPatchPanel = new PatchPanelMaster();
            objPatchPanel.geom = geom;
            objPatchPanel.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objPatchPanel, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objPatchPanel, new NetworkCodeIn() { eType = EntityType.PatchPanel.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPatchPanel.geom, parent_sysId = pSystemId, parent_eType = pEntityType }, networkIdType);
                objPatchPanel.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objPatchPanel.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objPatchPanel.ownership_type = "Own";
                objPatchPanel.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<FMSTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.PatchPanel);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objPatchPanel);
            }
            else
            {
                // Get entity detail by Id...
                objPatchPanel = new BLMisc().GetEntityDetailById<PatchPanelMaster>(systemId, EntityType.PatchPanel);

            }
            return objPatchPanel;

        }

        public PartialViewResult AddPatchPanel(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {

            PatchPanelMaster obj = new PatchPanelMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PatchPanelMaster>(url, obj, EntityType.PatchPanel.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddPatchPanel", response.results);
        }
        public ActionResult SavePatchPanel(PatchPanelMaster objPatchPanelMaster, bool isDirectSave = false)
        {

            objPatchPanelMaster.isDirectSave = isDirectSave;
            objPatchPanelMaster.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PatchPanelMaster>(url, objPatchPanelMaster, EntityType.PatchPanel.ToString(), EntityAction.Save.ToString());

            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_AddPatchPanel", response.results);
        }
        private void BindPatchPanelDropDown(PatchPanelMaster objPatchPanelMaster)
        {
            objPatchPanelMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objPatchPanelMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
            var obj_DDL = new BLMisc().GetDropDownList(EntityType.PatchPanel.ToString());
            objPatchPanelMaster.listPatchPanelType = obj_DDL.Where(x => x.dropdown_type == DropDownType.PatchPanel_type.ToString()).ToList();
        }
        #endregion
        // end 
        #region VSAT
        public PartialViewResult AddVSATHub(int vsat_system_id, bool is_vsat_updated = false)
        {
            BuildingMaster objBM = new BuildingMaster();

            var objVsatDetails = new BLMisc().GetVsatById(vsat_system_id);
            if (objVsatDetails != null)
                objBM.objVSATDetails = objVsatDetails;
            objBM.objVSATDetails.is_vsat_updated = is_vsat_updated;
            BindVSATDropDown(objBM);
            return PartialView("_AddVSATHub", objBM);
        }
        private void BindVSATDropDown(BuildingMaster objBM)
        {
            var Antenna_type = "";

            var objDDL = new BLMisc().GetDropDownList(EntityType.VSAT.ToString());
            objBM.objVSATDetails.VSATCategory = objDDL.Where(x => x.dropdown_type == DropDownType.VSAT_Category_Type.ToString()).ToList();

            if (objBM.objVSATDetails.category == "Hub Station")
                Antenna_type = "VSAT_Antenna_Hub_Type";
            else if (objBM.objVSATDetails.category == "Remote Station")
                Antenna_type = "VSAT_Antenna_Remote_Type";
            objBM.objVSATDetails.VSATAntennaType = objDDL.Where(x => x.dropdown_type == Antenna_type).ToList();
            objBM.objVSATDetails.VSATServiceType = objDDL.Where(x => x.dropdown_type == DropDownType.VSAT_Service_Type.ToString()).ToList();
            objBM.objVSATDetails.VSATTransmissionType = objDDL.Where(x => x.dropdown_type == DropDownType.VSAT_Transmission_Type.ToString()).ToList();
            objBM.objVSATDetails.VSATForwardLink = objDDL.Where(x => x.dropdown_type == objBM.objVSATDetails.transmission_type).ToList();
            objBM.objVSATDetails.VSATReturnLink = objDDL.Where(x => x.dropdown_type == DropDownType.VSAT_Return_Type.ToString()).ToList();

        }
        public List<DropDownMaster> BindAntennaType(VSATDetails objVSATDetails)
        {
            var Antenna_type = "";
            List<DropDownMaster> ddlList = new List<DropDownMaster>();
            var objDDL = new BLMisc().GetDropDownList(EntityType.VSAT.ToString());
            if (objVSATDetails.category != null)
            {
                if (objVSATDetails.category == "Hub Station")
                    Antenna_type = "VSAT_Antenna_Hub_Type";
                else if (objVSATDetails.category == "Remote Station")
                    Antenna_type = "VSAT_Antenna_Remote_Type";
                ddlList = objDDL.Where(x => x.dropdown_type == Antenna_type).ToList();
            }
            if (objVSATDetails.transmission_type != null)
                ddlList = objDDL.Where(x => x.dropdown_type == objVSATDetails.transmission_type).ToList();
            return ddlList;
        }
        public JsonResult GetAntennaType(string category)
        {
            VSATDetails objVSATDetails = new VSATDetails();
            objVSATDetails.category = category;
            var objResp = BindAntennaType(objVSATDetails);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        public JsonResult GetForwardLinkType(string TransmissionType)
        {
            VSATDetails objVSATDetails = new VSATDetails();
            objVSATDetails.transmission_type = TransmissionType;
            var objResp = BindAntennaType(objVSATDetails);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        #endregion

        #region Get Additional Attribute
        public vm_dynamic_form GetAdditionalAttributesForm(int layer_id)
        {
            BLDynamicAttributes objBLAdditionalAttributes = new BLDynamicAttributes();
            vm_dynamic_form objDynamicControls = new vm_dynamic_form();
            DynamicFormStyles cssStyle = new DynamicFormStyles();
            DynamicFormStyles labelStyle = new DynamicFormStyles();
            objDynamicControls.lstFormControls = objBLAdditionalAttributes.GetDynanicControlsById(layer_id);
            List<DynamicFormStyles> lstStyles = commonUtil.CreateListFromTable<DynamicFormStyles>(objBLAdditionalAttributes.GetDynamicFormStyle());
            foreach (DynamicControls dc in objDynamicControls.lstFormControls)
            {
                //Enum.TryParse(dc.control_type, out DynamicControlsType controlType);
                DynamicControlsType controlType = (DynamicControlsType)Enum.Parse(typeof(DynamicControlsType), dc.control_type, true);
                switch (controlType)
                {
                    case DynamicControlsType.TEXT:
                        cssStyle = lstStyles.Where(m => m.control_type == DynamicControlsType.TEXT.ToString()).FirstOrDefault();
                        dc.control_css_class = cssStyle.css_class;
                        labelStyle = lstStyles.Where(m => m.control_type == DynamicControlsType.LABEL.ToString()).FirstOrDefault();
                        dc.label_css_class = labelStyle.css_class;
                        break;
                    case DynamicControlsType.DROPDOWN:
                        cssStyle = lstStyles.Where(m => m.control_type == DynamicControlsType.DROPDOWN.ToString()).FirstOrDefault();
                        dc.control_css_class = cssStyle.css_class;
                        labelStyle = lstStyles.Where(m => m.control_type == DynamicControlsType.LABEL.ToString()).FirstOrDefault();
                        dc.label_css_class = labelStyle.css_class;
                        break;
                }

            }

            objDynamicControls.lstFormControlsChunk = objDynamicControls.lstFormControls.ToChunks(2).ToList();
            var controlIds = objDynamicControls.lstFormControls.Where(x => x.control_type == DynamicControlsType.DROPDOWN.ToString()).Select(x => x.id).ToArray();
            objDynamicControls.lstFormDDLValues = objBLAdditionalAttributes.GetDDLByControlsId(controlIds);
            return objDynamicControls;
        }
        #endregion

        #region GeoTaggedImages BY ANTRA
        public ActionResult GeoTaggedImageHistory(GeoTaggedImagesFilter objFilters, int page = 1, string sort = "", string sortdir = "")
        {
            objFilters.pageSize = 10;
            objFilters.currentPage = page == 0 ? 1 : page;
            objFilters.sort = sort;
            objFilters.sortdir = sortdir;
            objFilters.userId = Convert.ToInt32(Session["user_id"]);
            List<Dictionary<string, string>> lstGeoTaggedImghistory = new BLGeoTaggingAttachment().Get_GeoTaggedImageList(objFilters);
            Session["ExportGeoTaggedImgHistory"] = objFilters;
            if (lstGeoTaggedImghistory.Count > 0)
            {
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                foreach (Dictionary<string, string> dic in lstGeoTaggedImghistory)
                {
                    var obj = (IDictionary<string, object>)new ExpandoObject();
                    foreach (var col in dic)
                    {
                        if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                        {
                            obj.Add(col.Key, col.Value);
                        }
                    }
                    objFilters.lstGeoTaggedImghistory.Add(obj);
                }
                objFilters.totalRecord = objFilters.lstGeoTaggedImghistory.Count > 0 ? Convert.ToInt32(lstGeoTaggedImghistory[0].FirstOrDefault().Value) : 0;
            }
            return PartialView("_GeoTaggedImages", objFilters);
        }

        public void ExportGeoImages_History()
        {
            if (Session["ExportGeoTaggedImgHistory"] != null)
            {
                GeoTaggedImagesFilter objGeoTaggedFilters = (GeoTaggedImagesFilter)Session["ExportGeoTaggedImgHistory"];
                var exportData = new BLGeoTaggingAttachment().Get_GeoTaggedImageList(objGeoTaggedFilters);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(exportData);
                dtReport.Columns.Remove("totalrecords");
                dtReport.Columns.Remove("s_no");
                dtReport.Columns.Remove("uploaded_by");
                dtReport.Columns.Remove("uploaded_on");
                dtReport.Columns.Remove("file_location");
                dtReport.Columns.Remove("thumbimage_location");
                dtReport.Columns.Remove("org_file_name");
                dtReport.Columns.Remove("region_id");
                dtReport.Columns.Remove("province_id");
                dtReport.Columns.Remove("file_size");
                dtReport.Columns.Remove("id");
                dtReport.Columns["file_name"].ColumnName = "File Name";
                dtReport.Columns["file_description"].ColumnName = "File Description";
                dtReport.Columns["image_link"].ColumnName = "Image Link";
                dtReport.Columns["longitude"].ColumnName = "longitude";
                dtReport.Columns["latitude"].ColumnName = "latitude";
                dtReport.Columns["file_extension"].ColumnName = "File Extension";
                dtReport.Columns["image_uploaded_by"].ColumnName = "Uploaded By";
                dtReport.Columns["image_uploaded_on"].ColumnName = "Uploaded On";
                dtReport.Columns["region_name"].ColumnName = "Region Name";
                dtReport.Columns["province_name"].ColumnName = "Province Name";
                var filename = "GeoTaggedImage_History";
                dtReport.TableName = "GeoTaggedImageHistoryList";
                ExportHistory(dtReport, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        private void ExportHistory(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }
        public ActionResult ShowLayerOnMap(int id)
        {
            var objLayer = new BLGeoTaggingAttachment().getGeoDocumentById(id);
            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
            string imageUrl = string.Concat(FtpUrl, objLayer.file_location, "Thumb_", objLayer.org_file_name);
            string OrgimageUrl = string.Concat(FtpUrl, objLayer.file_location, objLayer.org_file_name);
            var _imgSrc = ""; var _OrgimgSrc = "";
            WebClient request = new WebClient();
            if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                request.Credentials = new NetworkCredential(UserName, PassWord);

            byte[] objdata = null;
            byte[] obj_data = null;
            var FilePathExist = ExistFile(imageUrl, UserName, PassWord);
            if (FilePathExist)
            {
                objdata = request.DownloadData(imageUrl);
                obj_data = request.DownloadData(OrgimageUrl);
                if (objdata != null && objdata.Length > 0)
                    _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));
                if (obj_data != null && obj_data.Length > 0)
                    _OrgimgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(obj_data));
                objLayer.Thumbgeotaggedpath = _imgSrc;
                objLayer.Org_geotaggedpath = _OrgimgSrc;
                JsonResult result = Json(new { result = objLayer });
                result.MaxJsonLength = Int32.MaxValue;
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            else
            {
                JsonResponse<string> jResp = new JsonResponse<string>();
                jResp.message = "File Not Found!";
                jResp.status = StatusCodes.INVALID_FILE.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
            return Json(objLayer, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGeoTaggedImage_RegionProvince(GeoTaggedImagesFilter obj)
        {
            List<GeoTaggedImagesFilter> lstImageResult = new List<GeoTaggedImagesFilter>();
            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
            obj.userId = Convert.ToInt32(Session["user_id"]);
            obj.SelectedRegionIds = obj.SelectedRegionId != null && obj.SelectedRegionId.Count > 0 ? string.Join(",", obj.SelectedRegionId.ToArray()) : "";
            obj.SelectedProvinceIds = obj.SelectedProvinceId != null && obj.SelectedProvinceId.Count > 0 ? string.Join(",", obj.SelectedProvinceId.ToArray()) : "";
            var lstImages = new BLGeoTaggingAttachment().GetGeoTaggedImageByRegionProvince(obj);
            foreach (var item in lstImages)
            {
                string imageUrl = string.Concat(FtpUrl, item.file_location, "Thumb_", item.org_file_name);
                string OrgimageUrl = string.Concat(FtpUrl, item.file_location, item.org_file_name);
                var _imgSrc = ""; var _OrgimgSrc = "";
                WebClient request = new WebClient();
                if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                    request.Credentials = new NetworkCredential(UserName, PassWord);

                byte[] objdata = null;
                byte[] obj_data = null;
                var FilePathExist = ExistFile(imageUrl, UserName, PassWord);
                if (FilePathExist)
                {
                    objdata = request.DownloadData(imageUrl);
                    obj_data = request.DownloadData(OrgimageUrl);
                    if (objdata != null && objdata.Length > 0)
                        _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));
                    if (obj_data != null && obj_data.Length > 0)
                        _OrgimgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(obj_data));
                    item.Thumbgeotaggedpath = _imgSrc;
                    item.Org_geotaggedpath = _OrgimgSrc;
                    lstImageResult.Add(new GeoTaggedImagesFilter()
                    {
                        latitude = item.latitude,
                        longitude = item.longitude,
                        file_name = item.file_name,
                        Thumbgeotaggedpath = _imgSrc,
                        Org_geotaggedpath = _OrgimgSrc
                    });
                }
            }
            var jsonResult = Json(lstImageResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        private static bool ExistFile(string remoteAddress, string FtpUser, string FtpPass)
        {
            int pos = remoteAddress.LastIndexOf('/');
            string dirPath = remoteAddress.Substring(0, pos); // skip the filename only get the directory

            NetworkCredential credentials = new NetworkCredential(FtpUser, FtpPass);
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(dirPath);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            listRequest.Credentials = credentials;
            using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
            using (Stream listStream = listResponse.GetResponseStream())
            using (StreamReader listReader = new StreamReader(listStream))
            {
                string fileToTest = Path.GetFileName(remoteAddress);
                while (!listReader.EndOfStream)
                {
                    string fileName = listReader.ReadLine();
                    fileName = Path.GetFileName(fileName);
                    if (fileToTest == fileName)
                    {
                        return true;
                    }

                }
            }
            return false;
        }
        #endregion
        public ActionResult AutoCodification(string pEntityType, int pSystemId, string pGeomType)
        {
            int p_user_id = Convert.ToInt32(Session["user_id"]);
            var ProcessData = BLBuilding.Instance.UpdateGeographicDetails(pEntityType, pSystemId, pGeomType, p_user_id);

            return Json(ProcessData, JsonRequestBehavior.AllowGet);
        }
        public void ExportCodificationLogs()
        {
            string fileName = "Codification_Logs_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
            DataTable dtgeo_log = (DataTable)TempData["codification_dt"];
            if (dtgeo_log.Rows.Count > 0)
                //export into excel...
                ExportGeoData(dtgeo_log, fileName);
        }
        private void ExportGeoData(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        }

        private void fillRegionProvAbbr(dynamic objEntityModel)
        {
            List<InRegionProvince> objRegionProvince = new List<InRegionProvince>();
            objRegionProvince = BLBuilding.Instance.GetRegionProvinceById(objEntityModel.region_id, objEntityModel.province_id);
            objEntityModel.region_abbreviation = objRegionProvince[0].region_abbreviation;
            objEntityModel.province_abbreviation = objRegionProvince[0].province_abbreviation;
        }

        #region process splitter
        //public ActionResult ProcessSplitterData(string pEntityType, string pSystemId ,int pUserId)
        // {
        //     var ProcessData = new BLProcess().ValidateEntitySummary(pSystemId, pEntityType, pUserId);
        //     ProcessData.pEntityType = pEntityType;
        //     ProcessData.pSystemId = pSystemId;
        //     return Json(ProcessData, JsonRequestBehavior.AllowGet);
        // }
        // public void DownloadProcessSplitterDataLogs(string pEntityType, string pSystemId,int pUserId)
        // {
        //     var ProcessData = new BLProcess().ValidateEntitySummary(pSystemId, pEntityType,pUserId);
        //     if (!string.IsNullOrEmpty(ProcessData.logs) && !ProcessData.status)
        //     {
        //         List<log> list = new List<log>();
        //         log obj = new log();
        //         ProcessData.listLog = JsonConvert.DeserializeObject<List<log>>(ProcessData.logs);
        //         DataTable dt = MiscHelper.ListToDataTable(ProcessData.listLog);
        //         if (dt.Rows.Count > 0)
        //         {
        //             if (dt.Columns.Contains("entity_id")) { dt.Columns.Remove("entity_id"); }
        //             if (dt.Columns.Contains("entity_type")) { dt.Columns.Remove("entity_type"); }
        //             if (dt.Columns.Contains("is_processed")) { dt.Columns.Remove("is_processed"); }
        //             dt.Columns["entity_title"].SetOrdinal(0);
        //             dt.Columns["gis_design_id"].SetOrdinal(1);
        //             dt.Columns["network_status"].SetOrdinal(2);
        //             dt.Columns["entity_title"].ColumnName = "Entity Type";
        //             for (int i = 0; i < dt.Rows.Count; i++)
        //             {
        //                 if (dt.Rows[i]["network_status"].ToString() == "P")
        //                 {
        //                     dt.Rows[i]["network_status"] = "Planned";
        //                 }
        //             }
        //         }
        //         dt.TableName = "ValidationLogs";
        //         ExportData(dt, "ExportValidationLogs_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        //     }
        // }

        #endregion

        public JsonResult ValidateNoOfCablesForMicroDuct(int systemId)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();
            var dbMessage = new BLMicroduct().Validate(systemId);
            return Json(dbMessage, JsonRequestBehavior.AllowGet);

        }
        #region RESET GIS DESIGN ID

        public string ResetDesignID(string pEntityType, int pSystemId)
        {
            int userId = Convert.ToInt32(Session["user_id"]);
            DbMessage dbMessageresponse = new BLMisc().ResetDesignID(pEntityType, pSystemId, userId);
            return dbMessageresponse.message.ToString();//BLConvertMLanguage.MultilingualMessageConvert(dbMessageresponse.message.ToString());
        }
        public string ResetPartialDesignID(string pEntityType, int pSystemId)
        {
            DbMessage dbMessageresponse = new BLMisc().ResetPartialDesignID(pEntityType, pSystemId, Convert.ToInt32(Session["user_id"]));
            return dbMessageresponse.message.ToString();//ConvertMultilingual.MultilingualMessageConvert(dbMessageresponse.message.ToString());
        }
        #endregion

        #region ProcessedXMLDashboard BY ANTRA
        public ActionResult GetProcessedXMLDashboard(ProcessSummaryFilter objFilters, int page = 1, string sort = "", string sortdir = "", int systemId = 0, string ps_port = "")
        {
            objFilters.pageSize = 10;
            objFilters.currentPage = page == 0 ? 1 : page;
            objFilters.sort = sort;
            objFilters.sortdir = sortdir;
            objFilters.objProcessSummary.userId = Convert.ToInt32(Session["user_id"]);
            List<Dictionary<string, string>> lstProcessedXMLDetails = new BLProcess().GetProcessedXMLDetails(objFilters);
            Session["ExportProcessedXMLDetails"] = objFilters;
            if (lstProcessedXMLDetails.Count > 0)
            {
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                foreach (Dictionary<string, string> dic in lstProcessedXMLDetails)
                {
                    var obj = (IDictionary<string, object>)new ExpandoObject();
                    foreach (var col in dic)
                    {
                        if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                        {
                            obj.Add(col.Key, col.Value);
                        }
                    }
                    objFilters.lstProcessedXMLDetails.Add(obj);
                }
                objFilters.totalRecord = objFilters.lstProcessedXMLDetails.Count > 0 ? Convert.ToInt32(lstProcessedXMLDetails[0].FirstOrDefault().Value) : 0;

            }
            objFilters.lstUserModulePermission = new BLLayer().GetUserModuleAbbrList(objFilters.objProcessSummary.userId, UserType.Web.ToString());
            Binddropdown();
            return View("_ProcessedXMLDashboard", objFilters);
        }

        public ActionResult Binddropdown()
        {
            var result = (List<string>)Session["ApplicableModuleList"];
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "File Name", Value = "file_name" });
            items.Add(new SelectListItem { Text = "CSA Id", Value = "csa_id" });
            items.Add(new SelectListItem { Text = "NE Status", Value = "import_status" });
            items.Add(new SelectListItem { Text = "File Version", Value = "file_version" });
            items.Add(new SelectListItem { Text = "NAS Status", Value = "nas_status" });
            if (Convert.ToInt32(Session["user_id"]) == 1)
            {
                items.Add(new SelectListItem { Text = "Entity Design Id", Value = "Entity" });
            }
            ViewData["listItem"] = items;
            return View();
        }

        public void ExportProcessedXML()
        {
            if (Session["ExportProcessedXMLDetails"] != null)
            {
                ProcessSummaryFilter objFilters = (ProcessSummaryFilter)Session["ExportProcessedXMLDetails"];
                var exportData = new BLProcess().GetProcessedXMLDetails(objFilters);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(exportData);
                dtReport.Columns.Remove("totalrecords");
                dtReport.Columns.Remove("s_no");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("process_id");
                dtReport.Columns.Remove("ticket_id");
                dtReport.Columns.Remove("stataus");
                dtReport.Columns.Remove("xml_status");
                dtReport.Columns.Remove("enitity_id");
                dtReport.Columns.Remove("remarks");
                dtReport.Columns.Remove("process_start_time");
                dtReport.Columns.Remove("process_end_time");
                dtReport.Columns.Remove("entity_type");
                dtReport.Columns.Remove("entity_design_id");
                dtReport.Columns.Remove("processed_entities");
                dtReport.Columns.Remove("file_extension");
                dtReport.Columns.Remove("ring_no");

                dtReport.Columns["csa_id"].SetOrdinal(0);
                dtReport.Columns["total_entity"].SetOrdinal(1);
                dtReport.Columns["file_name"].SetOrdinal(2);
                dtReport.Columns["file_version"].SetOrdinal(3);
                dtReport.Columns["nas_status"].SetOrdinal(4);
                dtReport.Columns["import_status"].SetOrdinal(5);
                dtReport.Columns["data_uploaded_by"].SetOrdinal(6);
                dtReport.Columns["data_uploaded_on"].SetOrdinal(7);

                dtReport.Columns["csa_id"].ColumnName = "CSA Id";
                dtReport.Columns["total_entity"].ColumnName = "Total Entities";
                dtReport.Columns["file_name"].ColumnName = "File Name";
                dtReport.Columns["import_status"].ColumnName = "NE Status";
                dtReport.Columns["nas_status"].ColumnName = "NAS Status";
                dtReport.Columns["file_version"].ColumnName = "File Version";
                dtReport.Columns["data_uploaded_by"].ColumnName = "Processed By";
                dtReport.Columns["data_uploaded_on"].ColumnName = "Processed On";
                var filename = "ProcessedXML_History";
                dtReport.TableName = "ProcessedXMLHistoryList";
                ExportHistory(dtReport, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public string ResetProcessedXML(int Process_Id)
        {
            var resp = new BLProcess().ResetProcessedXMLDetails(Process_Id, Convert.ToInt32(Session["user_id"]));
            return resp.message.ToString();
        }
        #endregion


        #region Trench Customer Details
        public PartialViewResult AddTrenchCustomerDetails(int systemId, int entityId, string entityType)
        {
            TrenchCustomerDetails objCustomerDetails = new TrenchCustomerDetails();
            objCustomerDetails.system_id = systemId;
            objCustomerDetails.trench_id = entityId;
            var TrenchCustomerDetails = BLTrenchCustomerDetails.Instance.getCustomerDetails(systemId);
            if (TrenchCustomerDetails != null)
            {
                objCustomerDetails = TrenchCustomerDetails;
                objCustomerDetails.from_date = DateTimeHelper.DateTimeFormate(TrenchCustomerDetails.from_date);
                objCustomerDetails.to_date = DateTimeHelper.DateTimeFormate(TrenchCustomerDetails.to_date);
                objCustomerDetails.po_release_date = DateTimeHelper.DateTimeFormate(TrenchCustomerDetails.po_release_date);
                objCustomerDetails.hoto_date = DateTimeHelper.DateTimeFormate(TrenchCustomerDetails.hoto_date);

            }
            return PartialView("_TrenchCustomerDetailsAdd", objCustomerDetails);
        }
        public PartialViewResult SaveTrenchCustomerDetails(TrenchCustomerDetails objCustomerDetails)
        {
            var isNew = objCustomerDetails.system_id > 0;
            PageMessage objPM = new PageMessage();
            objCustomerDetails = BLTrenchCustomerDetails.Instance.SaveCustomerDetails(objCustomerDetails, Convert.ToInt32(Session["user_id"]));
            objPM.status = StatusCodes.OK.ToString();
            if (!isNew)
            {
                objPM.message = "Customer Details added successfully!";
            }
            else { objPM.message = "Customer Details updated successfully!"; }
            objCustomerDetails.objPM = objPM;
            return PartialView("_TrenchCustomerDetailsAdd", objCustomerDetails);

        }
        public PartialViewResult TrenchCustomerDetailsList(int entityId, string entityType)
        {
            List<TrenchCustomerDetails> objTrenchCustomerDetailsList = new List<TrenchCustomerDetails>();
            objTrenchCustomerDetailsList = BLTrenchCustomerDetails.Instance.GetTrenchCustomerDetailsRecords(entityId, entityType);
            return PartialView("_TrenchCustomerDetailsList", objTrenchCustomerDetailsList);

        }
        public PartialViewResult GetTrenchCustomerDetails(int entityId)
        {
            TrenchCustomerDetailsList objList = new TrenchCustomerDetailsList();

            objList.listTrenchCustomerDetailsRecords = BLTrenchCustomerDetails.Instance.GetTrenchCustomerDetailsRecords(entityId, "Trench");
            objList.trench_id = entityId;
            objList.lstDocuments = new BLTrenchCustomerDetailsAttachment().getTrenchCustomerAttachment(0, 0);

            //converting file size
            foreach (var item in objList.lstDocuments)
            {
                item.file_size_converted = BytesToString(item.file_size);
            }

            return PartialView("_TrenchCustomerDetails", objList);

        }

        public JsonResult DeleteTrenchCustomerDetails(int systemId, int entityId)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                new BLTrenchCustomerDetails().DeleteCustomerDetailsByID(systemId, entityId);
            }
            catch (Exception ex)
            {
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.message = ex.Message;
            }
            objResp.status = StatusCodes.OK.ToString();
            objResp.message = string.Format(Resources.Resources.SI_OSP_GBL_JQ_GBL_074, "Customer");
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetTrenchCustomerDetailsAttachmentDetails(int system_Id, string entity_type, string featureName)
        {
            var lstDocument = new BLAttachment().getAttachmentDetails(system_Id, entity_type, "Document", featureName);
            //converting file size
            foreach (var item in lstDocument)
            {
                item.file_size_converted = BytesToString(item.file_size);

            }
            return PartialView("_TrenchCustomerDetailsFiles", lstDocument);
        }

        //BLTrenchcustomerDetailsAttachment

        [HttpPost]
        public ActionResult UploadTrenchcustomerDetails(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    var customer_id = collection["customer_id"];
                    var trench_id = collection["trench_id"];
                    var attachmentType = customer_id;
                    var mm = collection["files"];
                    HttpFileCollectionBase upfiles = Request.Files;
                    for (int i = 0; i < upfiles.Count; i++)
                    {
                        HttpPostedFileBase file = upfiles[i];
                        string FileName = file.FileName;
                        string strNewfilename = "TrenchCustomer_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        string strFilePath = "";

                        strFilePath = UploadfileOnFTP("TrenchCustomer", trench_id, file, attachmentType, strNewfilename);


                        // get User Detail..
                        User objUser = (User)(Session["userDetail"]);
                        TrenchCustomerDetailsAttachment objAttachment = new TrenchCustomerDetailsAttachment();
                        objAttachment.customer_id = Convert.ToInt32(customer_id);
                        objAttachment.trench_id = Convert.ToInt32(trench_id);
                        objAttachment.org_file_name = FileName;
                        objAttachment.file_name = strNewfilename;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_location = strFilePath;
                        objAttachment.uploaded_by = objUser.user_id;
                        objAttachment.file_size = Convert.ToInt32(file.ContentLength);
                        objAttachment.uploaded_on = DateTime.Now;
                        var savefile = new BLTrenchCustomerDetailsAttachment().SaveTrenchCustomerAttachment(objAttachment);
                    }
                    jResp.message = "File uploaded successfully";
                    jResp.status = StatusCodes.OK.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("UploadTrenchcustomerDetails()", "TrenchCustomerDetails", ex);
                    jResp.message = "Error in uploading document!";
                    jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                    //Error Logging...
                }
            }
            else
            {
                jResp.message = "No files selected.";
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
        }

        static string UploadfileOnFTP(string FeatureName, string id, HttpPostedFileBase postedFile, string sUploadType, string newfilename)
        {
            try
            {
                string strFTPFilePath = "";
                string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                if (isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {
                    // Create Directory if not exists and get Final FTP path to save file..
                    strFTPFilePath = CreateNestedDirectoryOnFTP(strFTPPath, strFTPUserName, strFTPPassWord, FeatureName, id, sUploadType);

                    if (sUploadType.ToUpper() == "IMAGES")
                    {
                        string thumnailImageName = "Thumb_" + newfilename;
                        FtpWebRequest ftpThumbnailImage = (FtpWebRequest)WebRequest.Create(strFTPFilePath + thumnailImageName);
                        ftpThumbnailImage.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                        ftpThumbnailImage.Method = WebRequestMethods.Ftp.UploadFile;
                        ftpThumbnailImage.UseBinary = true;
                        // var image = System.Drawing.Image.FromStream(postedFile.InputStream);
                        System.Drawing.Bitmap bmThumb = new System.Drawing.Bitmap(postedFile.InputStream);
                        System.Drawing.Image bmp2 = bmThumb.GetThumbnailImage(100, 100, null, IntPtr.Zero);
                        string saveThumnailPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                        bmp2.Save(saveThumnailPath + @"\" + thumnailImageName);
                        byte[] c = System.IO.File.ReadAllBytes(@"" + saveThumnailPath + "/" + thumnailImageName);
                        ftpThumbnailImage.ContentLength = c.Length;
                        using (Stream s = ftpThumbnailImage.GetRequestStream())
                        {
                            s.Write(c, 0, c.Length);
                        }

                        try
                        {
                            ftpThumbnailImage.GetResponse();
                        }
                        catch { throw; }
                        finally
                        {

                        }

                    }
                    FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(strFTPFilePath + newfilename);
                    ftpReq.Credentials = new NetworkCredential(strFTPUserName.Normalize(), strFTPPassWord.Normalize());
                    ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                    ftpReq.UseBinary = true;

                    //Save file temporarily on local path..
                    string savepath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]);
                    postedFile.SaveAs(savepath + @"\" + newfilename);
                    byte[] b = System.IO.File.ReadAllBytes(@"" + savepath + "/" + newfilename);
                    ftpReq.ContentLength = b.Length;
                    using (Stream s = ftpReq.GetRequestStream())
                    {
                        s.Write(b, 0, b.Length);
                    }

                    try
                    {
                        ftpReq.GetResponse();
                    }
                    catch { throw; }
                    finally
                    {
                        //Delete from local path.. 
                        System.IO.File.Delete(@"" + savepath + "/" + newfilename);
                        if (sUploadType.ToUpper() == "IMAGES")
                        {
                            System.IO.File.Delete(@"" + savepath + "/" + "Thumb_" + newfilename);
                        }
                    }
                }
                return strFTPFilePath.Replace(strFTPPath, ""); // return file path
            }
            catch { throw; }
        }

        private static bool isValidFTPConnection(string ftpUrl, string strUserName, string strPassWord)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(strUserName, strPassWord);
                request.GetResponse();
            }
            catch (WebException ex) { throw new Exception("Unable to connect to FTP Server", ex); }
            return true;
        }

        private static string CreateNestedDirectoryOnFTP(string strFTPPath, string strUserName, string strPassWord, params string[] directories)
        {
            try
            {
                FtpWebRequest reqFTP;
                string strFTPFilePath = strFTPPath;
                foreach (string directory in directories)
                {
                    if (!string.IsNullOrEmpty(directory) && directory.Trim() != "")
                    {
                        strFTPFilePath += directory + "/";
                        try
                        {
                            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(strFTPFilePath));
                            reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                            reqFTP.UseBinary = true;
                            reqFTP.Credentials = new NetworkCredential(strUserName, strPassWord);
                            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                            Stream ftpStream = response.GetResponseStream();
                            ftpStream.Close();
                            response.Close();
                        }
                        catch (WebException ex)
                        {
                            FtpWebResponse response = (FtpWebResponse)ex.Response;
                            //Directory already exists
                            if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable) { response.Close(); }
                            //Error in creating new directory on FTP..
                            else { throw new Exception("Error in creating directory/sub-directory!", ex); }
                        }
                    }
                }
                return strFTPFilePath;
            }
            catch { throw; }
        }

        public PartialViewResult GetTrenchCustomerDocument(int systemId, int entityId)
        {

            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

            // TrenchCustomerDetailsAttachmentList objdoc1 = new TrenchCustomerDetailsAttachmentList();
            List<TrenchCustomerDetailsAttachment> objdoc = new List<TrenchCustomerDetailsAttachment>();

            var lstDocument = new BLTrenchCustomerDetailsAttachment().getTrenchCustomerAttachment(systemId, entityId);
            foreach (var item in lstDocument)
            {
                objdoc.Add(new TrenchCustomerDetailsAttachment()
                {
                    id = item.id,
                    customer_id = item.customer_id,
                    trench_id = item.trench_id,
                    file_name = item.file_name,
                    uploaded_by = item.uploaded_by,
                    org_file_name = item.org_file_name,
                    file_extension = item.file_extension,
                    file_location = item.file_location,
                    file_size_converted = MiscHelper.BytesToString(item.file_size),// item.file_size,
                    uploaded_by_name = item.uploaded_by_name,
                    uploadedon = MiscHelper.FormatDateTime(item.uploaded_on.ToString())
                });

            }

            return PartialView("_TrenchCustomerDetailsFiles", objdoc);

        }


        #endregion

        #region SLACK Entity BY ANTRA
        public PartialViewResult AddSlack(string networkIdType, int systemId = 0, string geom = "")
        {
            SlackMaster obj = new SlackMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SlackMaster>(url, obj, EntityType.Slack.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddSlack", response.results);
        }
        public ActionResult SaveSlack(SlackMaster objSlack, bool isDirectSave = false)
        {
            objSlack.isDirectSave = isDirectSave;
            objSlack.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SlackMaster>(url, objSlack, EntityType.Slack.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddSlack", response.results);
        }

        public PartialViewResult GetSlackDetailsForDuct(SlackMaster obj)
        {
            List<SlackMaster> lstSlack = BLSlack.Instance.GetSlackDetailsForDuct(obj.duct_system_id);
            return PartialView("_SlackDetailsForDuct", lstSlack);

        }

        [HttpPost]
        public JsonResult DeleteSlackDetailById(int Slack_system_id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {


                if (BLSlack.Instance.DeleteSlackDetailById(Slack_system_id) > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Slack Detail Deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting Slack!";
                }

            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_289;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDuctNameAndLengthForSlack(int DuctId)
        {
            DuctMaster obj = new DuctMaster();
            obj = new BLDuct().GetDuctNameAndLengthForSlack(DuctId);
            obj.total_slack_count = obj.total_slack_count == null ? 0 : obj.total_slack_count;
            obj.total_slack_length = obj.total_slack_length == null ? 0 : obj.total_slack_length;
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Site by Pawan
        public PartialViewResult AddSite(string networkIdType, int systemId = 0, string geom = "")
        {
            Site obj = new Site();

            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.created_by = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<Site>(url, obj, EntityType.Site.ToString(), EntityAction.Get.ToString());
            return PartialView("_AddSite", response.results);
        }

        public Site GetSiteDetail(string networkIdType, int systemId, string geom = "")
        {
            Site objSite = new Site();
            objSite.geom = geom;
            objSite.networkIdType = networkIdType;
            var userdetails = (User)Session["userDetail"];
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objSite, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillParentDetail(objSite, new NetworkCodeIn() { eType = EntityType.Site.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSite.geom }, networkIdType);
                objSite.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objSite.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                //objSite.ownership_type = "Own";

                // Item template binding
                // var objItem = BLItemTemplate.Instance.GetTemplateDetail<CouplerTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Site);
                // MiscHelper.CopyMatchingProperties(objItem, objSite);
            }
            else
            {
                // Get entity detail by Id...
                objSite = new BLMisc().GetEntityDetailById<Site>(systemId, EntityType.Site);
            }
            return objSite;
        }

        public ActionResult SaveSite(Site objsite, bool isDirectSave = false)
        {
            objsite.created_by = Convert.ToInt32(Session["user_id"]);
            //objsite.network_status = "A";
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<Site>(url, objsite, EntityType.Site.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_AddSite", response.results);

        }




        #endregion

        public ActionResult GetcorePlanLogic()
        {
            FMSMaster obj = new FMSMaster();
            //obj.lstODFdetails = new BLCable().GetODFDetails("").ToList();
            //obj.lstFiberlinkdetails = new BLCable().GetFiberLinkDetails("").ToList();

            return PartialView("_GetcorePlanLogic", obj);
        }
        public JsonResult checkAvailability(string odf1, string odf2, string required_core)
        {

            JsonResponse<string> jResp = new JsonResponse<string>();
            DbMessageConePlanLogic obj = new DbMessageConePlanLogic();
            var cableSourceLatLng = new CableLatLngDetails();
            obj = new BLCable().Validate(odf1, odf2, Convert.ToInt32(required_core), Convert.ToInt32(Session["user_id"]));
            if (obj.source_geometry_extent != null && obj.destination_geometry_extent != null)
            {
                var extentSource = obj.source_geometry_extent.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
                string[] Sourcebounds = extentSource.Split(',');
                string[] SourcesouthWest = Sourcebounds[0].Split(' ');
                string[] SourcenorthEast = Sourcebounds[1].Split(' ');
                cableSourceLatLng.southWest = new latlong { Lat = SourcesouthWest[1], Long = SourcesouthWest[0] };
                cableSourceLatLng.northEast = new latlong { Lat = SourcenorthEast[1], Long = SourcenorthEast[0] };
                obj.source = cableSourceLatLng;
            }
            return Json(obj, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetcheckAvailability()
        {
            JsonResponse<List<CorePlannerLogs>> jResp = new JsonResponse<List<CorePlannerLogs>>();
            int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
           List<CorePlannerLogs> lstCorePlannerLogs = new BLCable().getCorePlanInvalidCable(user_id);

            return Json(lstCorePlannerLogs, JsonRequestBehavior.AllowGet);
        }

        public void ExportPlanLogicReport()
        {

            try
            {

                string fileName = "Core_Planner_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                int user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                List<CorePlannerLogs> lstCorePlannerLogs = new BLCable().GetCorePlanLogsByUserId(user_id);

                DataTable dtReport = MiscHelper.ListToDataTable<CorePlannerLogs>(lstCorePlannerLogs);
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    try
                    {
                        //if (dtReport.Columns.Contains("cable_id")) { dtReport.Columns["cable_id"].ColumnName = "Cable Id"; }
                        if (dtReport.Columns.Contains("cable_network_id")) { dtReport.Columns["cable_network_id"].ColumnName = "Cable Id"; }
                        if (dtReport.Columns.Contains("cable_name")) { dtReport.Columns["cable_name"].ColumnName = "Cable Name"; }
                        if (dtReport.Columns.Contains("network_status")) { dtReport.Columns["network_status"].ColumnName = "Network Status"; }
                        if (dtReport.Columns.Contains("total_core")) { dtReport.Columns["total_core"].ColumnName = "Total Cores"; }
                        if (dtReport.Columns.Contains("avaiable")) { dtReport.Columns["avaiable"].ColumnName = "Available Cores"; }
                        if (dtReport.Columns.Contains("cable_length")) { dtReport.Columns["cable_length"].ColumnName = "Cable Length(m)"; }
                        if (dtReport.Columns.Contains("a_system_id")) { dtReport.Columns.Remove("a_system_id"); }
                        if (dtReport.Columns.Contains("b_system_id")) { dtReport.Columns.Remove("b_system_id"); }
                        if (dtReport.Columns.Contains("a_entity_type")) { dtReport.Columns.Remove("a_entity_type"); }
                        if (dtReport.Columns.Contains("b_entity_type")) { dtReport.Columns.Remove("b_entity_type"); }
                        //if (dtReport.Columns.Contains("error_msg")) { dtReport.Columns.Remove("error_msg"); }
                        if (dtReport.Columns.Contains("error_msg")) { dtReport.Columns["error_msg"].ColumnName = "Message"; }
                        if (dtReport.Columns.Contains("cable_length")) { dtReport.Columns["cable_length"].ColumnName = "Cable Length(m)"; }
                        if (dtReport.Columns.Contains("user_id")) { dtReport.Columns.Remove("user_id"); }
                        if (dtReport.Columns.Contains("is_valid")) { dtReport.Columns.Remove("is_valid"); }
                        if (dtReport.Columns.Contains("a_network_id")) { dtReport.Columns.Remove("a_network_id"); }
                        if (dtReport.Columns.Contains("b_network_id")) { dtReport.Columns.Remove("b_network_id"); }
                        if (dtReport.Columns.Contains("cable_id")) { dtReport.Columns.Remove("cable_id"); }
                        if (dtReport.Columns.Contains("id")) { dtReport.Columns.Remove("id"); }
                        if (dtReport.Columns.Contains("Cable Id"))
                        {
                            dtReport.Columns["Cable Id"].SetOrdinal(0); // Move to the first column
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    dtReport = new DataTable();
                    dtReport.Columns.Add("No record found.", typeof(string));
                    dtReport.Rows.Add("");
                }
                ExportData(dtReport, fileName);
            }
            catch (Exception)
            {
                throw;
            }
            //if (dtReport.Rows.Count > 0)
            //{
            //    ExportData(dtReport, fileName);
            //}

        }
        public JsonResult GetSearchResult(string searchText, string search_type)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();
            FMSMaster obj = new FMSMaster();
            obj.lstCoreLogicSearchdetails = new BLCable().GetSearchResult(searchText, search_type).ToList();
            return Json(obj.lstCoreLogicSearchdetails, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SaveCorePlanLogic(string required_core, string fiber_link_network_id, string source_network_id, string destination_network_id, int buffer)
        {

            JsonResponse<string> jResp = new JsonResponse<string>();
            DbMessageConePlanLogic obj = new DbMessageConePlanLogic();
            obj = new BLCable().SaveCorePlanLogic(required_core, Convert.ToInt32(Session["user_id"]), fiber_link_network_id, source_network_id, destination_network_id, buffer);
            //---for optimization of splicing---------------
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var status = new BLOSPSplicing().updatedisplayname();

            }).ContinueWith(tsk =>
            {
                tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("Library", "SaveCorePlanLogic", ex); return true; });
            }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
            //---------------------------------------------------------------------------------------------------------

            return Json(obj, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult GetlinkPrefixbyPrefixType(string link_prefix)
        {
            BLFiberLink objBLFiberLink = new BLFiberLink();

            FiberLinkPrefix fiberLinkPrefix = new FiberLinkPrefix();
            if (!string.IsNullOrWhiteSpace(link_prefix))
            {
                fiberLinkPrefix = objBLFiberLink.GetlinkPrefixbyPrefixType(link_prefix);
            }

            return Json(new { data = fiberLinkPrefix.link_prefix }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult clearCoreplannerLog()
        {
            new BLCable().ClearCorePlanLogsByUserId(Convert.ToInt32(Session["user_id"]));
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUpdateFiberStatus(int cableId,int fiberNumber,string fiberStatus)
        {
            try
            {
                new BLCable().GetUpdateFiberStatus(cableId, fiberNumber, fiberStatus);

                return Json(new { message = "Fiber Status Updated successfully!", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error: " + ex.Message, status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetCableRingAssociation(Boolean filterSelected, string regionCode,string segementCode, string ringId,string cableId)
        {
            RingAssociationDetails ringAssociationDetails = new RingAssociationDetails();
            int userId = Convert.ToInt32(Session["user_id"]);
            ringAssociationDetails.lstTopologyRegionMaster = new BLProject().getTopologyRegionDetails();
            if (filterSelected)
            {
                ringAssociationDetails.region_id = string.IsNullOrEmpty(regionCode) ? 0 : ringAssociationDetails.lstTopologyRegionMaster.FirstOrDefault(x => x.region_name == regionCode).id;
                var segments = new BLProject().getSegmentDetailByIdList(ringAssociationDetails.region_id);
                ringAssociationDetails.segment_id = string.IsNullOrEmpty(segementCode) || segments == null ? 0 : segments.FirstOrDefault(x => x.segment_code == segementCode).id;
                var ringsdata = new BLProject().getRingCodeDetailByIdList(ringAssociationDetails.segment_id);
                ringAssociationDetails.ring_id = string.IsNullOrEmpty(ringId) || ringsdata == null ? 0 : ringsdata.FirstOrDefault(x => x.ring_code == ringId).id;
            }
            ringAssociationDetails.lstTopologyRegionMaster = new BLProject().getTopologyRegionDetails();
            ringAssociationDetails.cable_id = cableId;
            ringAssociationDetails.ringAssociations = new BLCable().GetRingAssociationDetails(filterSelected, regionCode, segementCode, ringId, userId, cableId);
            return PartialView("_CableRingAssociation", ringAssociationDetails);
        }

        [HttpPost]
        public JsonResult RemoveRingAssociation(int ringId,string cableId)
        {
            DbMessage objResp = new DbMessage();
            int userId = Convert.ToInt32(Session["user_id"]);
            objResp  = new BLCable().GetRemoveRingAssociation(ringId, userId, cableId);
            return Json(new { message = objResp.message, status = objResp.status }, JsonRequestBehavior.AllowGet);
        }
        
        //public PartialViewResult ShowProjectDetails(int page = 0)
        //{
        //   var  projectList = new BLProject().GetProjectDetails(); // Replace with your actual data access
        //    int totalRecords = projectList.Count;

        //    var model = new ProjectDetailsGridViewModel
        //    {
        //        lstProjectdetails = projectList.Skip(page * 10).Take(10).ToList(),
        //        totalRecord = totalRecords,
        //        pageSize = 10,
        //        currentPage = page + 1
        //    };

        //    return PartialView("_ProjectDetails", model);
        //}

        //public PartialViewResult EditProject(int siteId)
        //{
        //    ProjectDetailsGridViewModel siteprojectdetails = new ProjectDetailsGridViewModel();
        //    siteprojectdetails.lstProjectdetails= new BLProject().GetProjectDetailsById(siteId);
        //    return PartialView("_UpdateProjectDetails", siteprojectdetails);
        //}
        public PartialViewResult EditProject(int siteId)
        {
            var projectList = new BLProject().GetProjectDetailsById(siteId);
            
            return PartialView("_UpdateProjectDetails", projectList);
        }
        [HttpPost]
        public JsonResult UpdateProject(int Id,string siteId, string siteName, string projectCategory, string  cablePlanCores, string comment,string siteowner,int maximumcost,string address,string scmcarea)
        {
            try
            {
                // Save the data (e.g., update database)
                // Example: projectService.Update(...)
                var test = new BLProject().UpdateProjectDetails(Id,siteId, siteName, projectCategory, cablePlanCores, comment, siteowner, maximumcost, address, scmcarea);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public JsonResult DeleteProject(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = new BLProject().DeleteProjectById(id, Convert.ToInt32(Session["user_id"]));
            if (result.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
    }
}
