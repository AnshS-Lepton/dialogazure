using BusinessLogics;
using BusinessLogics.Admin;
using BusinessLogics.ISP;
using Models;
using Models.ISP;
using Newtonsoft.Json;
using SmartInventory.Filters;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;
using System.Configuration;
using System.Net;
using SmartInventory.ViewModel;
using System.Text;
using Lepton.Utility;
using System.Globalization;
using System.Data;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using SharpKml.Dom;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class ISPController : Controller
    {
        // GET: /ISP/Home/
        private List<NetworkLayer> LayerRoleData
        {
            get { return (List<NetworkLayer>)Session["NerworkLayerDetails"]; }
        }
        public ActionResult Index(string strid)
        {
            var usrDetail = (User)Session["userDetail"];
            //if (usrDetail != null && usrDetail.role_id == 1)
            //{
            //    return RedirectToAction("index", "UnAuthorized");

            //}

            var value = MiscHelper.Decrypt(strid);
            int structureId = Convert.ToInt32(value);
            ISPViewModel objISPRecords = new ISPViewModel();

            var StrDetails = BLISP.Instance.getSructureDetailsByCode(structureId);
            objISPRecords = BLISP.Instance.getShaftNFloor(structureId);
            objISPRecords.StructureId = structureId;
            objISPRecords.NetworkLayers = new BLLayer().GetISPNetworkLayers(usrDetail.user_id, 0, usrDetail.role_id);
            //objISPRecords.NetworkLayers = new BLLayer().GetISPNetworkLayers(usrDetail.user_id, 0);
            objISPRecords.UserRoleId = usrDetail.role_id;
            objISPRecords.StructureElements = BLISP.Instance.StructureElements(structureId, usrDetail.role_id);
            objISPRecords.ParentEntitiesList = BLISP.Instance.getParentEntities(structureId);
            objISPRecords.lstStructurCables = BLISP.Instance.getStructureCables(structureId);
            objISPRecords.lstShaftRange = BLISP.Instance.getShaftRangeByStructure(structureId);
            objISPRecords.network_status = StrDetails.network_status;
            if (StrDetails != null)
            {
                objISPRecords.StructureCode = StrDetails.network_id;
                objISPRecords.StructureName = StrDetails.structure_name;
                objISPRecords.total_unit = StrDetails.no_of_flat;
            }
            objISPRecords.listLayerAction = new BLMisc().getLayerActions(structureId, "Floor", false, StrDetails.network_status, usrDetail.role_id, usrDetail.user_id, false,"", "");
            //objISPRecords.ElementTemplates = BLISP.Instance.getElementTemplate(EntityType.UNIT.ToString(), Convert.ToInt32(Session["user_id"]));
            //objISPRecords.StructureElements = BLISP.Instance.StructureElements(structureId);
            //objISPRecords.SplitterParents = BLISP.Instance.getSplitterParents(structureId).Where(m => m.element_type == EntityType.BDB.ToString()).ToList();
            //objISPRecords.ONTlist = BLISP.Instance.getSplitterParents(structureId).Where(m => m.element_type == EntityType.ONT.ToString()).ToList();
            //objISPRecords.PODMODlist = BLISP.Instance.getSplitterParents(structureId).Where(m => m.element_type == EntityType.POD.ToString() || m.element_type == EntityType.MPOD.ToString()).ToList();
            //objISPRecords.StructureId = structureId;
            //objISPRecords.totalOSPCable = BLISP.Instance.getTotalOSPCable(structureId);
            //var ispLayers = new BLLayer().GetLayerDetails();            
            //objISPRecords.ISPLayers = ispLayers.Where(m => m.is_isp_layer == true).ToList(); 
            objISPRecords.NetworkLayerElements = new BLLayer().GetISPNetworkLayerElements(structureId, usrDetail.role_id);
            objISPRecords.lstUserModule = (List<string>)Session["ApplicableModuleList"];

            TempData["RackRolesData"] = LayerRoleData.FirstOrDefault(x => x.layerName.ToUpper() == EntityType.Rack.ToString().ToUpper());
            TempData["EquipmentRolesData"] = LayerRoleData.FirstOrDefault(x => x.layerName.ToUpper() == EntityType.Equipment.ToString().ToUpper());


            //TempData["RackRolesData"] = null;
            //TempData["EquipmentRolesData"] = null;
            return View(objISPRecords);
        }
        public ActionResult StructureInfo(int structureId)
        {
            var usrDetail = (User)Session["userDetail"];
            var StrDetails = BLISP.Instance.getSructureDetailsByCode(structureId);
            ISPViewModel objISPRecords = BLISP.Instance.getShaftNFloor(structureId);
            objISPRecords.StructureElements = BLISP.Instance.StructureElements(structureId, usrDetail.role_id);
            objISPRecords.totalOSPCable = BLISP.Instance.getTotalOSPCable(structureId);
            objISPRecords.lstStructurCables = BLISP.Instance.getStructureCables(structureId);
            objISPRecords.lstShaftRange = BLISP.Instance.getShaftRangeByStructure(structureId);
            objISPRecords.StructureId = structureId;
            objISPRecords.network_status = StrDetails.network_status;
            if (StrDetails != null)
            {
                objISPRecords.StructureCode = StrDetails.network_id;
                objISPRecords.StructureName = StrDetails.structure_name;
                objISPRecords.total_unit = StrDetails.no_of_flat;
            }
            objISPRecords.listLayerAction = new BLMisc().getLayerActions(structureId, "Floor", false, StrDetails.network_status, usrDetail.role_id, usrDetail.user_id, false,"","");
            return PartialView("_StructureInfo", objISPRecords);
        }
        public JsonResult getStructureCables(int structId)
        {
            var lstStructureCables = BLISP.Instance.getStructureCables(structId);
            return Json(lstStructureCables, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getElementTemplate(string ElementType)
        {
            var Template = BLISP.Instance.getElementTemplate(ElementType, Convert.ToInt32(Session["user_id"]));
            return PartialView("_ElementTemplate", Template);
        }
        public JsonResult getLayerDetails(string layerName)
        {
            var layerDetails = new BLLayer().getLayerDetailsByName(layerName);
            return Json(new { Data = layerDetails }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkTemplateExist(string enType)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            var chkIstemplate = BLISP.Instance.checkTemplateExist(enType, Convert.ToInt32(Session["user_id"]));
            if (!chkIstemplate)
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_050; //"Template is not filled. Please fill the template first!";
                objResp.result = enType;

            }
            else
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.result = enType;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #region Floor
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
        public ActionResult FloorInfo(int floorId)
        {
            FloorInfo Model = BLISP.Instance.getFloorInfo(floorId);
            return PartialView("_FloorInfo", Model);
        }
        public JsonResult UpdateFloorInfo(FloorInfo model)
        {
            PageMessage objPM = new PageMessage();
            var result = BLISP.Instance.UpdateFloorInfo(model);
            objPM.status = ResponseStatus.OK.ToString(); ;
            objPM.message = "Floor updated successfully.";
            result.objPM = objPM;
            //return PartialView("_FloorInfo", result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShaftInfo(int shaftId, int structureId)
        {
            ShaftInfo Model = BLISP.Instance.getShaftInfo(shaftId);
            Model.FloorList = BLISP.Instance.getFloorList(structureId);
            Model.ShaftRangelist = BLISP.Instance.getShaftRangeInfo(shaftId);
            return PartialView("_ShaftInfo", Model);
        }
        public JsonResult UpdateShaftInfo(ShaftInfo model)
        {
            PageMessage objPM = new PageMessage();
            if (!string.IsNullOrEmpty(model.shaft_name))
            {
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.FDB);
                var formSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Structure.ToString()).ToList();
                var withRiser = formSettings.Count > 0 ? formSettings.Where(m => m.form_feature_name.ToLower() == formFeatureName.with_riser.ToString() && m.form_feature_type.ToLower() == formFeatureType.feature.ToString() && m.is_active == true).FirstOrDefault() : null;
                if (withRiser != null && model.with_riser)
                {
                    if (objItem.specification == "" || objItem.specification == null)
                    {
                        string[] LayerName = { EntityType.FDB.ToString() };
                        objPM.status = ResponseStatus.FAILED.ToString();
                        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_STR_NET_FRM_045, ApplicationSettings.listLayerDetails, LayerName);// "FDB Template does not exist for this user!";
                        model.objPM = objPM;
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
                if (model.is_partial_shaft)
                {
                    var result = BLISP.Instance.SaveShaftRange(model.ShaftRangelist, model.system_id, model.structure_id, Convert.ToInt32(((User)Session["userDetail"]).user_id));
                }
                else
                {
                    BLISP.Instance.DeleteShaftRange(model.system_id, model.structure_id);
                }
                var chkSaveShaft = BLISP.Instance.SaveShaft(model, Convert.ToInt32(((User)Session["userDetail"]).user_id));
                model.FloorList = BLISP.Instance.getFloorList(model.structure_id);
                model.ShaftRangelist = BLISP.Instance.getShaftRangeInfo(model.system_id);
                objPM.status = ResponseStatus.OK.ToString(); ;
                objPM.message = Resources.Resources.SI_ISP_GBL_NET_FRM_083;// "Shaft updated successfully.";
                model.objPM = objPM;
                // result.objPM = objPM;
                //return PartialView("_ShaftInfo", model);
            }
            else
            {
                model.FloorList = BLISP.Instance.getFloorList(model.structure_id);
                model.ShaftRangelist = BLISP.Instance.getShaftRangeInfo(model.system_id);
                objPM.status = ResponseStatus.ERROR.ToString();
                objPM.message = "Shaft name is required";
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Room
        public ActionResult AddRoom(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0)
        {

            RoomInfo roomInfo = getRoomInfo(networkIdType, ModelInfo.templateId, systemId, ModelInfo.structureid);
            if (systemId != 0)
            {
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.UNIT.ToString());
                roomInfo.objIspEntityMap.floor_id = ispMapping.floor_id;
                roomInfo.objIspEntityMap.structure_id = ispMapping.structure_id;
            }
            else
            {
                roomInfo.objIspEntityMap.floor_id = ModelInfo.floorid;
                roomInfo.objIspEntityMap.structure_id = ModelInfo.structureid;
            }
            BindUnitDropDown(roomInfo);
            return PartialView("_AddRoom", roomInfo);
        }
        private void BindUnitDropDown(RoomInfo objRoomInfo)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.UNIT.ToString()).Where(x => x.dropdown_type == "Unit_Type").ToList();
            objRoomInfo.lstUnitType = objDDL;
        }
        public RoomInfo getRoomInfo(string networkIdType, int templateId, int systemId, int structureId)
        {
            RoomInfo objRoom = new RoomInfo();
            objRoom.networkIdType = networkIdType;
            objRoom.parent_system_id = structureId;
            objRoom.parent_entity_type = EntityType.Structure.ToString();
            objRoom.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            if (systemId == 0)
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.UNIT.ToString(), structureId = structureId });
                    objRoom.network_id = ISPNetworkCodeDetail.network_code;
                }
                var objItem = BLISP.Instance.getRoomTemplate(Convert.ToInt32(((User)Session["userDetail"]).user_id));
                Utility.MiscHelper.CopyMatchingBaseProperties(objItem, objRoom);

            }
            else
            {
                objRoom = BLISP.Instance.getRoomDetails(systemId);

            }
            return objRoom;
        }
        public ActionResult SaveRoom(RoomInfo model, bool isDirectSave = false)
        {

            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            int structure_id = model.objIspEntityMap.structure_id;
            int floor_id = model.objIspEntityMap.floor_id ?? 0;
            model.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
            {
                //GET AUTO NETWORK CODE...
                var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.objIspEntityMap.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.UNIT.ToString(), structureId = model.objIspEntityMap.structure_id });
                if (isDirectSave == true)
                {

                    model = getRoomInfo(model.networkIdType, model.templateId, model.system_id, model.objIspEntityMap.structure_id);
                    model.objIspEntityMap.floor_id = floor_id;
                    model.objIspEntityMap.structure_id = structure_id;
                    model.room_name = objISPNetworkCode.network_code;



                    //var FloorInfo = BLISP.Instance.getFloorInfo(model.objIspEntityMap.floor_id);
                    //if (model.room_width > FloorInfo.width)
                    //{
                    //    objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    //    objPM.message = "UNIT width can not be greater then floor width!";
                    //}
                    //else if (model.room_height > FloorInfo.height)
                    //{
                    //    objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    //    objPM.message = "UNIT height can not be greater then floor height!";
                    //}
                    //else if (model.room_length > FloorInfo.length)
                    //{
                    //    objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    //    objPM.message = "UNIT length can not be greater then floor length!";
                    //}

                    //var AllRooms = BLISP.Instance.getRoomByFloorId(model.objIspEntityMap.structure_id, model.objIspEntityMap.floor_id);
                    //double AllRoomLength = 0;
                    //double availableLength = (Convert.ToDouble(FloorInfo.length) - AllRoomLength);
                    //foreach (var item in AllRooms)
                    //{
                    //    AllRoomLength = AllRoomLength + item.room_length;
                    //}
                    //if (availableLength == 0)
                    //{
                    //    objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    //    objPM.message = "Space is not available on <b>" + FloorInfo.floor_name + "</b> floor!";
                    //}
                    //if (AllRoomLength + model.room_length > FloorInfo.length)
                    //{
                    //    objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    //    objPM.message = "selected UNIT can not be added into <b>" + FloorInfo.floor_name + "</b> as unit length is greater than the available length(" + availableLength + ")!";
                    //}
                    //if (objPM.status == ResponseStatus.VALIDATION_FAILED.ToString())
                    //{ return Json(new { status = objPM.status, message = objPM.message, Data = model }, JsonRequestBehavior.AllowGet); }
                }
                model.network_id = objISPNetworkCode.network_code;
                model.sequence_id = objISPNetworkCode.sequence_id;

            }
            var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(structure_id, EntityType.Structure);
            if (structureDetails != null)
            {
                model.region_id = structureDetails.region_id;
                model.province_id = structureDetails.province_id;
                model.latitude = structureDetails.latitude;
                model.longitude = structureDetails.longitude;
            }
            if (TryValidateModel(model))
            {
                bool isNew = model.system_id == 0 ? true : false;
                var result = new BLISP().SaveRoomDetails(model);
                if (string.IsNullOrEmpty(result.objPM.message))
                {
                    string[] LayerName = { EntityType.UNIT.ToString() };

                    if (isNew)
                    {
                        objPM.status = ResponseStatus.OK.ToString(); ;
                        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "UNIT saved successfully.";
                        objPM.systemId = result.system_id;
                        objPM.entityType = EntityType.UNIT.ToString();
                        objPM.NetworkId = result.network_id;
                        objPM.structureId = model.objIspEntityMap.structure_id;
                        objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
                        objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
                        objPM.pSystemId = model.parent_system_id;
                        objPM.pEntityType = model.parent_entity_type;
                    }
                    else
                    {
                        objPM.status = ResponseStatus.OK.ToString(); ;
                        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "UNIT updated successfully.";
                    }
                    model.objPM = objPM;
                }

            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = getFirstErrorFromModelState();
                model.objPM = objPM;
            }
            model.entityType = EntityType.UNIT.ToString();
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(model.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {
                BindUnitDropDown(model);
                return PartialView("_AddRoom", model);
            }
        }
        public JsonResult DeleteRoomById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.UNIT.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
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

        #region HTB
        public ActionResult AddHTB(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //HTBInfo model = getHTBInfo(networkIdType, ModelInfo.templateId, systemId, ModelInfo.structureid);
            //if (systemId != 0)
            //{
            //    var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.HTB.ToString());
            //    model.objIspEntityMap.floor_id = ispMapping.floor_id;
            //    model.objIspEntityMap.structure_id = ispMapping.structure_id;
            //    model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
            //}
            //else
            //{
            //    model.objIspEntityMap.floor_id = ModelInfo.floorid;
            //    model.objIspEntityMap.structure_id = ModelInfo.structureid;
            //    model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            //}
            //BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
            //new MiscHelper().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //BindHTBDropDown(model);
            //fillProjectSpecifications(model);
            //return PartialView("_AddHTB", model);
            
            HTBInfo model = new HTBInfo();
            model.networkIdType = networkIdType;
            model.parent_entity_type = pEntityType;
            model.parent_system_id = pSystemId;
            model.parent_network_id = pNetworkId;
            model.system_id = systemId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HTBInfo>(url, model, EntityType.HTB.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            response.results.is_middlewareInLayer = new BLMisc().chkEntityIsMiddleWare(EntityType.HTB.ToString());
            return PartialView("_AddHTB", response.results);
        }
        private void BindHTBDropDown(HTBInfo objBDB)
        {
            var objDDL = new BLBDB().GetShaftFloorByStrucId(objBDB.parent_system_id);
            //objBDB.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objBDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }
        public HTBInfo getHTBInfo(string networkIdType, int templateId, int systemId, int structureId, int parent_system_id = 0, string parent_entity_type = "", string parent_network_id = "")
        {
            HTBInfo objHTB = new HTBInfo();
            objHTB.networkIdType = networkIdType;
            objHTB.parent_system_id = parent_system_id == 0 ? structureId : parent_system_id;
            objHTB.parent_entity_type = parent_entity_type == "" ? EntityType.Structure.ToString() : parent_entity_type;
            objHTB.parent_network_id = parent_network_id;
            if (systemId != 0)
            {
                objHTB = BLISP.Instance.getHTBDetails(systemId);
            }
            else
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.HTB.ToString(), structureId = structureId });
                    objHTB.network_id = ISPNetworkCodeDetail.network_code;
                }
                objHTB.ownership_type = "Own";
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<HTBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.HTB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objHTB);

            }
            return objHTB;
        }
        public ActionResult SaveHTB(HTBInfo objHTB, bool isDirectSave = false)
        {

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int structure_id = model.objIspEntityMap.structure_id;
            //int? floor_id = model.objIspEntityMap.floor_id;
            //int? shaft_id = model.objIspEntityMap.shaft_id;
            //string pNetworkId = model.pNetworkId;
            //int pSystemId = model.pSystemId;
            //string pEntityType = model.pEntityType;
            //model.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            //if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.HTB.ToString(), structureId = structure_id });
            //    if (isDirectSave == true)
            //    {
            //        model = getHTBInfo(model.networkIdType, model.templateId, model.system_id, model.objIspEntityMap.structure_id);
            //        model.objIspEntityMap.floor_id = floor_id;
            //        model.objIspEntityMap.structure_id = structure_id;
            //        model.objIspEntityMap.shaft_id = shaft_id;
            //        model.parent_system_id = pSystemId;
            //        model.parent_entity_type = pEntityType;
            //        model.parent_network_id = pNetworkId;
            //        model.htb_name = objISPNetworkCode.network_code;
            //    }
            //    model.network_id = objISPNetworkCode.network_code;
            //    model.sequence_id = objISPNetworkCode.sequence_id;

            //}
            //var structureDetails = new BLISP().GetStructureById(structure_id);
            //if (structureDetails != null)
            //{
            //    model.region_id = structureDetails.First().region_id;
            //    model.province_id = structureDetails.First().province_id;
            //    model.latitude = structureDetails.First().latitude;
            //    model.longitude = structureDetails.First().longitude;
            //}
            //if (TryValidateModel(model))
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.HTB.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    bool isNew = model.system_id == 0 ? true : false;
            //    if (model.unitValue != null && model.unitValue.Contains(":"))
            //    {
            //        model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
            //        model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
            //    }
            //    var result = new BLISP().SaveHTBDetails(model, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(result.objPM.message))
            //    {

            //        if (isNew)
            //        {
            //            string[] LayerName = { EntityType.HTB.ToString() };
            //            objPM.status = ResponseStatus.OK.ToString(); ;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "HTB saved successfully.";
            //            objPM.systemId = result.system_id;
            //            objPM.entityType = EntityType.HTB.ToString();
            //            objPM.NetworkId = result.network_id;
            //            objPM.structureId = model.objIspEntityMap.structure_id;
            //            objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
            //            objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
            //            objPM.pSystemId = model.parent_system_id;
            //            objPM.pEntityType = model.parent_entity_type;
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
            //                string[] LayerName = { layer_title };
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
            //model.entityType = EntityType.HTB.ToString();
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(model.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
            //    new MiscHelper().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //    BindHTBDropDown(model);
            //    fillProjectSpecifications(model);
            //    return PartialView("_AddHTB", model);
            //}

            objHTB.user_id = Convert.ToInt32(Session["user_id"]);
            objHTB.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HTBInfo>(url, objHTB, EntityType.HTB.ToString(), EntityAction.Save.ToString(), objHTB.objIspEntityMap.structure_id.ToString());
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
            response = new BLMisc().deleteEntity(systemId, EntityType.HTB.ToString(), GeometryType.Point.ToString(),usrDetail.user_id);
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

        #region FDB
        public ActionResult AddFDB(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0)
        {
            //FDBInfo model = getFDBInfo(networkIdType, ModelInfo.templateId, systemId, ModelInfo.structureid);
            //if (systemId != 0)
            //{
            //    var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.FDB.ToString());
            //    model.objIspEntityMap.floor_id = ispMapping.floor_id;
            //    model.objIspEntityMap.structure_id = ispMapping.structure_id;
            //    model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
            //}
            //else
            //{
            //    model.objIspEntityMap.floor_id = ModelInfo.floorid;
            //    model.objIspEntityMap.structure_id = ModelInfo.structureid;
            //    model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            //}
            //new MiscHelper().BindPortDetails(model, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
            //BindFDBDropdown(model);
            //return PartialView("_AddFDB", model);

            FDBInfo model = new FDBInfo();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FDBInfo>(url, model, EntityType.FDB.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddFDB", response.results);

        }
        public FDBInfo getFDBInfo(string networkIdType, int templateId, int systemId, int structureId)
        {
            FDBInfo objFDB = new FDBInfo();
            objFDB.networkIdType = networkIdType;
            objFDB.parent_system_id = structureId;
            objFDB.parent_entity_type = EntityType.Structure.ToString();
            if (systemId != 0)
            {
                objFDB = BLISP.Instance.getFDBDetails(systemId);
            }
            else
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = structureId });
                    objFDB.network_id = ISPNetworkCodeDetail.network_code;
                }
                objFDB.ownership_type = "Own";
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.FDB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objFDB);


            }
            BLItemTemplate.Instance.BindItemDropdowns(objFDB, EntityType.FDB.ToString());
            fillProjectSpecifications(objFDB);
            return objFDB;
        }
        public ActionResult SaveFDB(FDBInfo objFDB, bool isDirectSave = false)
        {

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int structure_id = model.objIspEntityMap.structure_id;
            //int floor_id = model.objIspEntityMap.floor_id ?? 0;
            //int shaft_id = model.objIspEntityMap.shaft_id ?? 0;
            //if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.objIspEntityMap.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = model.objIspEntityMap.structure_id });
            //    if (isDirectSave == true)
            //    {
            //        model = getFDBInfo(model.networkIdType, model.templateId, model.system_id, model.objIspEntityMap.structure_id);
            //        model.objIspEntityMap.floor_id = floor_id;
            //        model.objIspEntityMap.structure_id = structure_id;
            //        model.objIspEntityMap.shaft_id = shaft_id;
            //        model.fdb_name = objISPNetworkCode.network_code;
            //    }
            //    model.network_id = objISPNetworkCode.network_code;
            //    model.sequence_id = objISPNetworkCode.sequence_id;

            //}
            //var structureDetails = new BLISP().GetStructureById(structure_id);
            //if (structureDetails != null && structureDetails.Count > 0)
            //{
            //    model.region_id = structureDetails.First().region_id;
            //    model.province_id = structureDetails.First().province_id;
            //    model.latitude = structureDetails.First().latitude;
            //    model.longitude = structureDetails.First().longitude;
            //}
            //if (TryValidateModel(model))
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault().layer_title;

            //    bool isNew = model.system_id == 0 ? true : false;
            //    model.userId = Convert.ToInt32(Session["user_id"]);
            //    if (model.unitValue != null && model.unitValue.Contains(":"))
            //    {
            //        model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
            //        model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
            //    }
            //    var resultItem = new BLISP().SaveFDBDetails(model);
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {

            //        //Save Reference
            //        if (model.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(model.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            string[] LayerName = { EntityType.FDB.ToString() };
            //            objPM.status = ResponseStatus.OK.ToString(); ;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
            //            objPM.systemId = resultItem.system_id;
            //            objPM.entityType = EntityType.FDB.ToString();
            //            objPM.NetworkId = resultItem.network_id;
            //            objPM.structureId = model.objIspEntityMap.structure_id;
            //            objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
            //            objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
            //            objPM.pSystemId = model.parent_system_id;
            //            objPM.pEntityType = model.parent_entity_type;
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
            //                string[] LayerName = { layer_title };
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
            //model.entityType = EntityType.FDB.ToString();
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(model.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.FDB.ToString());
            //    BindFDBDropdown(model);
            //    fillProjectSpecifications(model);
            //    new MiscHelper().BindPortDetails(model, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
            //    return PartialView("_AddFDB", model);
            //}
            objFDB.user_id = Convert.ToInt32(Session["user_id"]);
            objFDB.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FDBInfo>(url, objFDB, EntityType.FDB.ToString(), EntityAction.Save.ToString(), objFDB.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddFDB", response.results);
        }
        private void BindFDBDropdown(FDBInfo objFDB)
        {
            objFDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }
        public void saveWithRiser(int structureId)
        {
            ISPViewModel objISPViewModel = BLISP.Instance.getShaftNFloor(structureId);
            var objItem = BLISP.Instance.getFDBTemplate(Convert.ToInt32(((User)Session["userDetail"]).user_id));
            foreach (var shaftItem in objISPViewModel.ShaftList)
            {
                foreach (var floorItem in objISPViewModel.FloorList)
                {
                    FDBInfo objFdbInfo = new FDBInfo();
                    Utility.MiscHelper.CopyMatchingBaseProperties(objItem, objFdbInfo);
                    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = structureId });
                    objFdbInfo.networkIdType = "A";
                    objFdbInfo.parent_system_id = structureId;
                    objFdbInfo.parent_entity_type = EntityType.Structure.ToString();
                    objFdbInfo.objIspEntityMap.floor_id = floorItem.systemid;
                    objFdbInfo.objIspEntityMap.structure_id = structureId;
                    objFdbInfo.objIspEntityMap.shaft_id = shaftItem.systemid;
                    objFdbInfo.fdb_name = objISPNetworkCode.network_code;
                    objFdbInfo.network_id = objISPNetworkCode.network_code;
                    objFdbInfo.network_status = "P";
                    var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(structureId, EntityType.Structure);
                    if (structureDetails != null)
                    {
                        objFdbInfo.region_id = structureDetails.region_id;
                        objFdbInfo.province_id = structureDetails.province_id;
                        objFdbInfo.latitude = structureDetails.latitude;
                        objFdbInfo.longitude = structureDetails.longitude;
                    }
                    // new BLISP().FDBWithRiser(objFdbInfo);
                }
            }
        }
        public JsonResult DeleteFDBById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.FDB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id  );
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.FDB.ToString());
            //if (isNotAssociated == true) { result = BLISP.Instance.DeleteFDBById(systemId); }

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

        #region BDB
        public ActionResult AddBDB(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0)
        {
            //BDBMaster model = getBDBInfo(networkIdType, systemId, ModelInfo.structureid);
            //if (systemId != 0)
            //{
            //    var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.BDB.ToString());
            //    model.objIspEntityMap.floor_id = ispMapping.floor_id;
            //    model.parent_system_id = ispMapping.structure_id;
            //    model.objIspEntityMap.structure_id = ispMapping.structure_id;
            //    model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
            //}
            //else
            //{
            //    model.objIspEntityMap.floor_id = ModelInfo.floorid;
            //    model.parent_system_id = ModelInfo.structureid;
            //    model.objIspEntityMap.structure_id = ModelInfo.structureid;
            //    model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            //    var structureDetails = new BLISP().GetStructureById(ModelInfo.structureid);
            //    if (structureDetails != null && structureDetails.Count > 0)
            //    {
            //        model.region_id = structureDetails.First().region_id;
            //        model.province_id = structureDetails.First().province_id;
            //        model.latitude = structureDetails.First().latitude;
            //        model.longitude = structureDetails.First().longitude;
            //        model.parent_network_id = structureDetails.First().network_id;
            //    }
            //}
            //BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.BDB.ToString());
            //BindBDBDropDown(model);
            //fillProjectSpecifications(model);
            //new MiscHelper().BindPortDetails(model, EntityType.BDB.ToString(), DropDownType.BDB_PORT_RATIO.ToString());
            //return PartialView("_AddBDB", model);

            BDBMaster model = new BDBMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<BDBMaster>(url, model, EntityType.BDB.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddBDB", response.results);
        }
        private void fillProjectSpecifications(dynamic objLib)
        {
            //"P" we need to pass this value as dynamically as network stage selection
            objLib.lstBindProjectCode = new BLProject().getProjectCodeDetails(string.IsNullOrWhiteSpace(objLib.network_status) ? "P" : objLib.network_status);
            objLib.lstBindPlanningCode = new BLProject().getPlanningDetailByIdList(Convert.ToInt32(objLib.project_id ?? 0));
            objLib.lstBindWorkorderCode = new BLProject().getWorkorderDetailByIdList(Convert.ToInt32(objLib.planning_id ?? 0));
            objLib.lstBindPurposeCode = new BLProject().getPurposeDetailByIdList(Convert.ToInt32(objLib.workorder_id ?? 0));
        }
        private void BindBDBDropDown(BDBMaster objBDB)
        {
            var objDDL = new BLBDB().GetShaftFloorByStrucId(objBDB.parent_system_id);
            var objTypDDL = new BLMisc().GetDropDownList(EntityType.BDB.ToString(), DropDownType.Entity_Type.ToString());
            objBDB.lstEntityType = objTypDDL;
            objBDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();

        }
        public BDBMaster getBDBInfo(string networkIdType, int systemId, int structureId)
        {
            BDBMaster objBDB = new BDBMaster();
            objBDB.networkIdType = networkIdType;
            objBDB.parent_system_id = structureId;
            objBDB.parent_entity_type = EntityType.Structure.ToString();
            if (systemId != 0)
            {
                objBDB = new BLBDB().getBDBDetails(systemId);
            }
            else
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.BDB.ToString(), structureId = structureId });
                    objBDB.network_id = ISPNetworkCodeDetail.network_code;
                }
                objBDB.ownership_type = "Own";
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<BDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.BDB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objBDB);
                objBDB.address = BLStructure.Instance.getBuildingAddress(structureId);

            }
            return objBDB;
        }
        public ActionResult SaveBDB(BDBMaster objBDB, bool isDirectSave = false)
        {

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int structure_id = model.objIspEntityMap.structure_id;
            //int? floor_id = model.objIspEntityMap.floor_id;
            //int? shaft_id = model.objIspEntityMap.shaft_id;
            //if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.BDB.ToString(), structureId = structure_id });
            //    if (isDirectSave == true)
            //    {
            //        model = getBDBInfo(model.networkIdType, model.system_id, model.parent_system_id);
            //        model.objIspEntityMap.floor_id = floor_id;
            //        model.parent_system_id = structure_id;
            //        model.objIspEntityMap.shaft_id = shaft_id;
            //        model.bdb_name = objISPNetworkCode.network_code;
            //        model.objIspEntityMap.structure_id = structure_id;

            //        var structureDetails = new BLISP().GetStructureById(structure_id);
            //        if (structureDetails != null && structureDetails.Count > 0)
            //        {
            //            model.region_id = structureDetails.First().region_id;
            //            model.province_id = structureDetails.First().province_id;
            //            model.latitude = structureDetails.First().latitude;
            //            model.longitude = structureDetails.First().longitude;
            //            model.parent_network_id = structureDetails.First().network_id;
            //        }
            //    }
            //    model.network_id = objISPNetworkCode.network_code;
            //    model.sequence_id = objISPNetworkCode.sequence_id;

            //}

            //if (model.unitValue != null && model.unitValue.Contains(":"))
            //{
            //    model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
            //    model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
            //}
            //if (TryValidateModel(model))
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.BDB.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    bool isNew = model.system_id == 0 ? true : false;
            //    var resultItem = new BLBDB().SaveEntityBDB(model, Convert.ToInt32(((User)Session["userDetail"]).user_id));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {


            //        //Save Reference
            //        if (model.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(model.EntityReference, resultItem.system_id);
            //        }

            //        if (isNew)
            //        {
            //            string[] LayerName = { EntityType.BDB.ToString() };
            //            objPM.status = ResponseStatus.OK.ToString(); ;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
            //            objPM.systemId = resultItem.system_id;
            //            objPM.entityType = EntityType.BDB.ToString();
            //            objPM.NetworkId = resultItem.network_id;
            //            objPM.structureId = model.objIspEntityMap.structure_id;
            //            objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
            //            objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
            //            objPM.pSystemId = model.parent_system_id;
            //            objPM.pEntityType = model.parent_entity_type;
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
            //                string[] LayerName = { EntityType.BDB.ToString() };
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = BLConvertMLanguage.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName); ;
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
            //model.entityType = EntityType.BDB.ToString();
            ////return Json(new { Status = model.objPM.status, Message = model.objPM.message, Data = model }, JsonRequestBehavior.AllowGet);
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(model.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.BDB.ToString());
            //    BindBDBDropDown(model);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA

            //    fillProjectSpecifications(model);
            //    return PartialView("_AddBDB", model);
            //}

            objBDB.user_id = Convert.ToInt32(Session["user_id"]);
            objBDB.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<BDBMaster>(url, objBDB, EntityType.BDB.ToString(), EntityAction.Save.ToString(), objBDB.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddBDB", response.results);
        }
        public JsonResult DeleteBDBById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.BDB.ToString());
            //if (isNotAssociated == true) { result = new BLBDB().DeleteBDBById(systemId); }
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.BDB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);

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

        #region ADB
        public ActionResult AddADB(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0)
        {
            ADBMaster model = new ADBMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ADBMaster>(url, model, EntityType.ADB.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddADB", response.results);
        }
        private void BindADBDropDown(ADBMaster objADB)
        {
            var objDDL = new BLBDB().GetShaftFloorByStrucId(objADB.parent_system_id);
            var objTypDDL = new BLMisc().GetDropDownList(EntityType.ADB.ToString(), DropDownType.Entity_Type.ToString());
            objADB.lstEntityType = objTypDDL;
            objADB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();

        }
        public ADBMaster getADBInfo(string networkIdType, int systemId, int structureId)
        {
            ADBMaster objADB = new ADBMaster();
            objADB.networkIdType = networkIdType;
            objADB.parent_system_id = structureId;
            objADB.parent_entity_type = EntityType.Structure.ToString();
            if (systemId != 0)
            {
                objADB = new BLADB().getADBDetails(systemId);
            }
            else
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.ADB.ToString(), structureId = structureId });
                    objADB.network_id = ISPNetworkCodeDetail.network_code;
                }
                objADB.ownership_type = "Own";
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<ADBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.ADB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objADB);
                objADB.address = BLStructure.Instance.getBuildingAddress(structureId);

            }
            return objADB;
        }
        public ActionResult SaveADB(ADBMaster objADB, bool isDirectSave = false)
        {
            objADB.user_id = Convert.ToInt32(Session["user_id"]);
            objADB.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ADBMaster>(url, objADB, EntityType.ADB.ToString(), EntityAction.Save.ToString(), objADB.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddADB", response.results);
        }
        public JsonResult DeleteADBById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.ADB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);

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
        #region CDB
        public ActionResult AddCDB(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType = "", string pNetworkId = "", int no_of_ports = 0, int vendor_id = 0, bool isConvert = false, int sc_system_id = 0)
        {
            CDBMaster model = new CDBMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.geom = geom;
            model.no_of_ports = no_of_ports;
            model.vendor_id = vendor_id;
            model.isConvert = isConvert;
            model.sc_system_id = sc_system_id;
            model.system_id = systemId;
            model.isConvert = isConvert;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CDBMaster>(url, model, EntityType.CDB.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddCDB", response.results);
        }
        private void BindCDBDropDown(CDBMaster objCDB)
        {
            var objDDL = new BLBDB().GetShaftFloorByStrucId(objCDB.parent_system_id);
            var objTypDDL = new BLMisc().GetDropDownList(EntityType.CDB.ToString(), DropDownType.Entity_Type.ToString());
            objCDB.lstEntityType = objTypDDL;
            objCDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();

        }
        public CDBMaster getCDBInfo(string networkIdType, int systemId, int structureId)
        {
            CDBMaster objCDB = new CDBMaster();
            objCDB.networkIdType = networkIdType;
            objCDB.parent_system_id = structureId;
            objCDB.parent_entity_type = EntityType.Structure.ToString();
            if (systemId != 0)
            {
                objCDB = new BLCDB().getCDBDetails(systemId);
            }
            else
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.CDB.ToString(), structureId = structureId });
                    objCDB.network_id = ISPNetworkCodeDetail.network_code;
                }
                objCDB.ownership_type = "Own";
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<BDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.CDB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objCDB);
                objCDB.address = BLStructure.Instance.getBuildingAddress(structureId);

            }
            return objCDB;
        }
        public ActionResult SaveCDB(CDBMaster objCDB, bool isDirectSave = false)
        {
            objCDB.user_id = Convert.ToInt32(Session["user_id"]);
            objCDB.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CDBMaster>(url, objCDB, EntityType.CDB.ToString(), EntityAction.Save.ToString(), objCDB.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddCDB", response.results);
        }
        public JsonResult DeleteCDBById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.CDB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);

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

        #region Splitter
        public ActionResult AddSplitter(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "")
        {
            SplitterMaster model = getSplitterInfo(networkIdType, ModelInfo.templateId, systemId, ModelInfo.structureid, pSystemId, pEntityType);
            if (systemId != 0)
            {
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.Splitter.ToString());
                model.parent_system_id = ispMapping.structure_id;
                model.objIspEntityMap.structure_id = ispMapping.structure_id;
                model.pSystemId = pSystemId;
                model.pEntityType = pEntityType;
            }
            else
            {
                model.parent_system_id = ModelInfo.structureid;
                model.objIspEntityMap.structure_id = ModelInfo.structureid;
                model.pSystemId = pSystemId;
                model.pEntityType = pEntityType;
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                model.geom = structureDetails.longitude + " " + structureDetails.latitude;
            }

            BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.Splitter.ToString());
            BindSplitterDropDown(model);
            fillProjectSpecifications(model);
            model.unitValue = model.splitter_ratio;
            //Get the layer details to bind additional attributes Splitter
            var layerdetails = new BLLayer().getLayer(EntityType.Splitter.ToString());
            model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
            //End for additional attributes Splitter
            return PartialView("_AddSplitter", model);
        }
        private void BindSplitterDropDown(SplitterMaster objSplitterMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Splitter.ToString());
            //objSplitterMaster.lstSplRatio = objDDL.Where(x => x.dropdown_type == DropDownType.Splitter_Ratio.ToString()).ToList();
            new BLMisc().BindPortDetails(objSplitterMaster, EntityType.Splitter.ToString(), DropDownType.Splitter_Ratio.ToString());
            objSplitterMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            var _objDDL = new BLMisc().GetDropDownList("");
            objSplitterMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
           // objSplitterMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
        }
        public SplitterMaster getSplitterInfo(string networkIdType, int templateId, int systemId, int structureId, int pSystemId, string pEntityType)
        {
            SplitterMaster objSplitter = new SplitterMaster();
            objSplitter.networkIdType = networkIdType;
            //objSplitter.parent_system_id = structureId;
            //objSplitter.parent_entity_type = EntityType.Structure.ToString();
            objSplitter.structure_id = structureId;
            if (pSystemId != 0 && pEntityType != "")
            {
                var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(structureId, pSystemId, pEntityType.ToString());
                if (ispEntityMap != null)
                {
                    objSplitter.objIspEntityMap.id = ispEntityMap.id;
                    objSplitter.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                    objSplitter.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                    objSplitter.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                }
            }

            if (systemId != 0)
            {
                // Get entity detail by Id...
                objSplitter = new BLMisc().GetEntityDetailById<SplitterMaster>(systemId, EntityType.Splitter);
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.Splitter.ToString());
                if (ispMapping != null)
                {
                    objSplitter.objIspEntityMap.shaft_id = ispMapping.shaft_id;
                    objSplitter.objIspEntityMap.floor_id = ispMapping.floor_id;
                    objSplitter.objIspEntityMap.structure_id = ispMapping.structure_id;
                }
                objSplitter.geom = objSplitter.longitude + " " + objSplitter.latitude;
                //for additional-attributes
                objSplitter.other_info = new BLSplitter().GetOtherInfoSplitter(objSplitter.system_id);
                fillRegionProvAbbr(objSplitter);
            }
            else
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.Splitter.ToString(), structureId = structureId });
                    objSplitter.network_id = ISPNetworkCodeDetail.network_code;
                }
                objSplitter.ownership_type = "Own";
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objSplitter.objIspEntityMap.structure_id, EntityType.Structure);
                fillRegionProvinceDetail(objSplitter, structureDetails, GeometryType.Point.ToString());
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<SplitterTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Splitter);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objSplitter);
                objSplitter.address = BLStructure.Instance.getBuildingAddress(structureId);
                objSplitter.other_info = null;  //for additional-attributes
            }
            return objSplitter;
        }
        public ActionResult SaveSplitter(SplitterMaster model, bool isDirectSave = false)
        {

            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            int structure_id = model.objIspEntityMap.structure_id;
            int pSystemId = model.pSystemId;
            string pEntityType = model.pEntityType;
            model.parent_system_id = pSystemId;
            model.parent_entity_type = pEntityType;

            if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
            {
                //GET AUTO NETWORK CODE...
                var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.pSystemId, parent_eType = model.pEntityType, eType = EntityType.Splitter.ToString(), structureId = structure_id });
                if (isDirectSave == true)
                {
                    model = getSplitterInfo(model.networkIdType, model.templateId, model.system_id, structure_id, model.pSystemId, model.pEntityType);
                    model.objIspEntityMap.structure_id = structure_id;
                    model.parent_system_id = pSystemId;
                    model.parent_entity_type = pEntityType;
                    model.splitter_name = objISPNetworkCode.network_code;
                    model.objIspEntityMap.structure_id = structure_id;
                    var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
                    //var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
                    model.bom_sub_category = objBOMDDL[0].dropdown_value;
                   // model.served_by_ring = objSubCatDDL[0].dropdown_value;
                }
                model.network_id = objISPNetworkCode.network_code;
                model.sequence_id = objISPNetworkCode.sequence_id;

            }
            var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(structure_id, EntityType.Structure);
            if (structureDetails != null)
            {
                model.region_id = structureDetails.region_id;
                model.province_id = structureDetails.province_id;
                model.latitude = structureDetails.latitude;
                model.longitude = structureDetails.longitude;
            }
            if (model.unitValue != null && model.unitValue.Contains(":"))
            {
                model.splitter_ratio = model.unitValue;
            }

            if (TryValidateModel(model))
            {
                bool isNew = model.system_id == 0 ? true : false;
                var resultItem = new BLSplitter().SaveSplitterEntity(model, Convert.ToInt32(((User)Session["userDetail"]).user_id));
                if (string.IsNullOrEmpty(resultItem.objPM.message))
                {

                    if (isNew)
                    {
                        string[] LayerName = { EntityType.Splitter.ToString() };
                        objPM.status = ResponseStatus.OK.ToString(); ;
                        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "Splitter saved successfully.";
                        objPM.systemId = resultItem.system_id;
                        objPM.entityType = EntityType.Splitter.ToString();
                        objPM.NetworkId = resultItem.network_id;
                        objPM.structureId = model.objIspEntityMap.structure_id;
                        objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
                        objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
                        objPM.pSystemId = model.parent_system_id;
                        objPM.pEntityType = model.parent_entity_type;
                    }
                    else
                    {
                        if (resultItem.isPortConnected == true)
                        {
                            objPM.status = ResponseStatus.OK.ToString();
                            objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
                        }
                        else
                        {
                            string[] LayerName = { EntityType.Splitter.ToString() };
                            objPM.status = ResponseStatus.OK.ToString();
                            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "Splitter updated successfully.";
                        }

                    }
                    model.objPM = objPM;
                }

            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = getFirstErrorFromModelState();
                model.objPM = objPM;
            }
            model.entityType = EntityType.Splitter.ToString();
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(model.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {
                BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.Splitter.ToString());
                BindSplitterDropDown(model);
                fillProjectSpecifications(model);
                model.unitValue = model.splitter_ratio;
                //Get the layer details to bind additional attributes Splitter
                var layerdetails = new BLLayer().getLayer(EntityType.Splitter.ToString());
                model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
                //End for additional attributes Splitter
                return PartialView("_AddSplitter", model);
            }
        }
        public JsonResult DeleteSplitterById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            var result = 0;
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.Splitter.ToString());
            //if (isNotAssociated == true) { result = new BLSplitter().DeleteSplitterById(systemId); }
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.Splitter.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
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




            //JsonResponse<string> objResp = new JsonResponse<string>();
            //var result = new BLSplitter().DeleteSplitterById(systemId);
            //if (result == 1)
            //{
            //    objResp.status = ResponseStatus.OK.ToString();
            //    objResp.message = "Splitter has deleted successfully!";
            //}
            //else
            //{
            //    objResp.status = ResponseStatus.FAILED.ToString();
            //    objResp.message = "Something went wrong while deleting room!";
            //}
            //return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region SPLICE CLOSURE
        public ActionResult AddSpliceClosure(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            SCMaster model = new SCMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SCMaster>(url, model, EntityType.SpliceClosure.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddSpliceClosure", response.results);
        }
        public SCMaster getSCInfo(string networkIdType, int templateId, int systemId, int structureId, int pSystemId, string pEntityType)
        {
            SCMaster objSC = new SCMaster();
            objSC.networkIdType = networkIdType;
            //objSC.parent_system_id = structureId;
            //objSC.parent_entity_type = EntityType.Structure.ToString();
            objSC.objIspEntityMap.structure_id = structureId;
            if (pSystemId != 0 && pEntityType != "")
            {
                var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(structureId, pSystemId, pEntityType.ToString());
                if (ispEntityMap != null)
                {
                    objSC.objIspEntityMap.id = ispEntityMap.id;
                    objSC.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                    objSC.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                }
            }
            if (systemId != 0)
            {
                objSC = new BLMisc().GetEntityDetailById<SCMaster>(systemId, EntityType.SpliceClosure);
                objSC.no_of_port = objSC.no_of_ports;
            }
            else
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.SpliceClosure.ToString(), structureId = objSC.objIspEntityMap.structure_id });
                    objSC.network_id = ISPNetworkCodeDetail.network_code;
                }
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<SCTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.SpliceClosure);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objSC);
                objSC.no_of_ports = objItem.no_of_ports;
                objSC.address = BLStructure.Instance.getBuildingAddress(objSC.objIspEntityMap.structure_id);
            }
            return objSC;
        }
        public ActionResult SaveSpliceClosure(SCMaster model, bool isDirectSave = false)
        {

            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SCMaster>(url, model, EntityType.SpliceClosure.ToString(), EntityAction.Save.ToString(), model.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddSpliceClosure", response.results);
        }
        public JsonResult DeleteSCById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            var result = 0;
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.Spliceclosure.ToString());
            //if (isNotAssociated == true) { result = new BLSC().DeleteSCById(systemId); }

            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.SpliceClosure.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
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


            //JsonResponse<string> objResp = new JsonResponse<string>();
            //var result = new BLSC().DeleteSCById(systemId);
            //if (result == 1)
            //{
            //    objResp.status = ResponseStatus.OK.ToString();
            //    objResp.message = "Splice Closure has deleted successfully!";
            //}
            //else
            //{
            //    objResp.status = ResponseStatus.FAILED.ToString();
            //    objResp.message = "Something went wrong while deleting room!";
            //}
            //return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cable Details
        public PartialViewResult AddCable(LineEntityIn objIn)
        {
            CableMaster objCbl = new CableMaster();
            objCbl = GetISPCableInfo(objIn);
            var objStructureDetail = new BLMisc().GetEntityDetailById<StructureMaster>(objIn.ModelInfo.structureid, EntityType.Structure);
            if (objIn.systemId == 0)
            {
                var geom = objStructureDetail.longitude + " " + objStructureDetail.latitude;
                objCbl.geom = geom;
                //Fill Location detail...     
                GetISPLineNtkDetail(objCbl, objIn, EntityType.Cable.ToString(), false, geom);
            }

            BLItemTemplate.Instance.BindItemDropdowns(objCbl, EntityType.Cable.ToString());
            objCbl.structure_id = objIn.ModelInfo.structureid;
            if (objStructureDetail != null)
            {
                objCbl.region_id = objStructureDetail.region_id;
                objCbl.province_id = objStructureDetail.province_id;
            }

            BindCableDropDown(objCbl);
            objCbl.fiberCountIn = objCbl.total_core.ToString();
            fillISPProjectSpecific(objCbl);
            new BLMisc().BindPortDetails(objCbl, EntityType.Cable.ToString(), DropDownType.Fiber_Count.ToString());
            objCbl.unitValue = Convert.ToString(objCbl.total_core);
            objCbl.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
            return PartialView("_AddISPCable", objCbl);
        }

        public PartialViewResult GetISPCableTubeCoreDetail(int? cableId)
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

        private void BindCableDropDown(CableMaster objCableIn)
        {

            var objDDL = new BLMisc().GetDropDownList(EntityType.Cable.ToString());
            objCableIn.fiberCount = objDDL.Where(x => x.dropdown_type == DropDownType.Fiber_Count.ToString()).ToList();
            objCableIn.listcableCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Category.ToString()).ToList();
            objCableIn.listcableSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Subcategory.ToString()).ToList();
            objCableIn.listExecutionMethod = objDDL.Where(x => x.dropdown_type == DropDownType.Execution_Method.ToString()).ToList();
            objCableIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            var _objDDL = new BLMisc().GetDropDownList("");
            objCableIn.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
        }
        private void fillISPProjectSpecific(dynamic objLib)
        {
            //"P" we need to pass this value as dynamically as network stage selection
            objLib.lstBindProjectCode = new BLProject().getProjectCodeDetails(string.IsNullOrWhiteSpace(objLib.network_status) ? "P" : objLib.network_status);
            objLib.lstBindPlanningCode = new BLProject().getPlanningDetailByIdList(Convert.ToInt32(objLib.project_id ?? 0));
            objLib.lstBindWorkorderCode = new BLProject().getWorkorderDetailByIdList(Convert.ToInt32(objLib.planning_id ?? 0));
            objLib.lstBindPurposeCode = new BLProject().getPurposeDetailByIdList(Convert.ToInt32(objLib.workorder_id ?? 0));

        }
        private void GetISPLineNtkDetail(dynamic objLib, LineEntityIn objIn, string enName, bool isAuto, string geometry)
        {
            var startObj = objIn.lstTP[0];
            var endObj = objIn.lstTP[objIn.lstTP.Count() - 1];
            var start_network_id = startObj.network_id;
            var end_network_id = endObj.network_id;
            objLib.cable_type = CableTypes.ISP.ToString();
            //fill parent detail....
            var networkCodeDetail = new BLMisc().GetLineNetworkCode(start_network_id, end_network_id, enName, geometry, "ISP");

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
                objLib.a_entity_type = startObj.network_name;
                objLib.a_system_id = startObj.system_id;
                objLib.a_location = start_network_id;
                objLib.a_node_type = startObj.node_type;

                objLib.b_entity_type = endObj.network_name;
                objLib.b_system_id = endObj.system_id;
                objLib.b_location = end_network_id;
                objLib.b_node_type = endObj.node_type;
                objLib.sequence_id = networkCodeDetail.sequence_id;
            }
            objLib.parent_entity_type = networkCodeDetail.parent_entity_type;
            objLib.parent_network_id = networkCodeDetail.parent_network_id;
            objLib.parent_system_id = networkCodeDetail.parent_system_id;
        }

        private CableMaster GetISPCableInfo(LineEntityIn objIn)
        {
            CableMaster objCbl = new CableMaster();
            if (objIn.systemId == 0)
            {
                objCbl.cable_type = objIn.cableType;
                objCbl.networkIdType = objIn.networkIdType;
                objCbl.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<CableTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Cable, objIn.cableType);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objCbl);

            }
            else
            {
                objCbl = new BLMisc().GetEntityDetailById<CableMaster>(objIn.systemId, EntityType.Cable);
                if (objIn.lstTP.Count > 0)
                {
                    objCbl.a_location = objIn.lstTP[0].network_id;
                    objCbl.a_system_id = objIn.lstTP[0].system_id;
                    objCbl.a_entity_type = objIn.lstTP[0].network_name;
                    objCbl.a_node_type = objIn.lstTP[0].node_type;

                    objCbl.b_location = objIn.lstTP[1].network_id;
                    objCbl.b_system_id = objIn.lstTP[1].system_id;
                    objCbl.b_entity_type = objIn.lstTP[1].network_name;
                    objCbl.b_node_type = objIn.lstTP[1].node_type;
                }
            }

            return objCbl;
        }

        public JsonResult GetCableInfo(int systemId)
        {

            var cableInfo = new BLMisc().GetEntityDetailById<CableMaster>(systemId, EntityType.Cable);

            return Json(cableInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveCable(CableMaster objCbl, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            bool isValid = true;
            if (objCbl.networkIdType == NetworkIdType.A.ToString() && objCbl.system_id == 0)
            {
                if (isDirectSave == false)
                {
                    objCbl.lstTP.Add(new NetworkDtl { system_id = objCbl.a_system_id, network_id = objCbl.a_location, network_name = objCbl.a_entity_type, node_type = objCbl.a_node_type });
                    objCbl.lstTP.Add(new NetworkDtl { system_id = objCbl.b_system_id, network_id = objCbl.b_location, network_name = objCbl.b_entity_type, node_type = objCbl.b_node_type });
                }
                var objLineEntity = new LineEntityIn() { geom = objCbl.geom, systemId = objCbl.system_id, cableType = objCbl.cable_type, networkIdType = objCbl.networkIdType, lstTP = objCbl.lstTP };

                if (isDirectSave == true)
                {

                    objCbl = GetISPCableInfo(objLineEntity);
                    var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
                    objCbl.bom_sub_category = objBOMDDL[0].dropdown_value;
                }
                var listStructure = new BLMisc().GetEntityDetailById<StructureMaster>(objCbl.structure_id, EntityType.Structure);
                objCbl.geom = listStructure.longitude + " " + listStructure.latitude;
                //GET AUTO NETWORK CODE...
                GetISPLineNtkDetail(objCbl, objLineEntity, EntityType.Cable.ToString(), true, objCbl.geom);
                if (isDirectSave == true)
                    objCbl.cable_name = objCbl.network_id;
            }
            objCbl.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
            var multipleTubeCore = objCbl.no_of_core_per_tube * objCbl.no_of_tube;
            if (multipleTubeCore != Convert.ToInt32(objCbl.unitValue) || multipleTubeCore == 0)
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = Resources.Resources.SI_OSP_GBL_JQ_FRM_012;// "Multiple of no of tube and no of core per tube should be equal to fiber count !!";
                isValid = false;
            }
            //if (objCbl.ownership.ToUpper() != "OWN" && objCbl.circuit_id == null)
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_017, objCbl.ownership);// "Circuit ID is mandatory for " + objCbl.ownership + " ownership!";
            //    isValid = false;
            //}
            if (!(string.IsNullOrEmpty(objCbl.unitValue)))
            {
                objCbl.total_core = Convert.ToInt32(objCbl.unitValue);
            }
            if (TryValidateModel(objCbl) && isValid == true)
            {
                var isNew = objCbl.system_id > 0 ? false : true;
                var resultItem = new BLCable().SaveCable(objCbl, Convert.ToInt32(Session["user_id"]));
                if (string.IsNullOrEmpty(resultItem.objPM.message))
                {
                    string[] LayerName = { EntityType.Cable.ToString() };
                    if (objCbl.lstTubeCore != null)
                    {
                        BLCable.Instance.SaveTubeCoreColor(JsonConvert.SerializeObject(objCbl.lstTubeCore.objTube), JsonConvert.SerializeObject(objCbl.lstTubeCore.objCore), resultItem.system_id);
                    }
                    if (isNew)
                    {
                        //var insertTubeCore = BLCable.Instance.SetCableColorDetails(resultItem.system_id, resultItem.no_of_tube, resultItem.no_of_core_per_tube, resultItem.created_by);

                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.isNewEntity = isNew;
                        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "Cable saved successfully.";
                    }
                    else
                    {
                        if (resultItem.isPortConnected)
                        {
                            objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                            objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
                        }
                        else
                        {
                            objPM.status = ResponseStatus.OK.ToString();
                            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "Cable updated successfully.";
                        }
                    }
                    objCbl.objPM = objPM;
                }

            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = isValid == true ? getFirstErrorFromModelState() : objPM.message;
                objCbl.objPM = objPM;
            }
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objCbl.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objCbl, EntityType.Cable.ToString());
            //    BindCableDropDown(objCbl);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    objCbl.objPM = objPM;
            //    fillProjectSpecifications(objCbl);
            //    return PartialView("_AddISPCable", objCbl);
            return Json(objCbl, JsonRequestBehavior.AllowGet);
            //}
        }

        public ActionResult getSplitCable(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, string entityType = "", List<StructureCable> cables = null)
        {
            EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), entityType);
            SplitCableMDU model = new SplitCableMDU();
            dynamic spliterDetail = new BLMisc().GetEntityDetailById<dynamic>(systemId, enType); //getFDBInfo(networkIdType, ModelInfo.templateId, systemId, ModelInfo.structureid);
            model.split_entity_networkId = spliterDetail.network_id;
            model.split_entity_system_id = spliterDetail.system_id;
            model.split_entity_network_status = spliterDetail.network_status;
            model.split_entity_type = networkIdType;
            model.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
            model.cables = cables;
            return PartialView("_SplitCableMDU", model);
        }

        public CableMaster getCableObject(int cableNo, SplitCableMDU splitModle, CableMaster CableMaster, string cable_a_location, string cable_b_location, float? cable_measured_length, float? cable_calculated_length, string cable_name, string cable_network_id, string geom, string a_node_type, string b_node_type)
        {
            CableMaster newObj = new CableMaster();

            CableMaster.unitValue = Convert.ToString(CableMaster.total_core);
            CableMaster.system_id = 0;

            if (cableNo == 1)
            {
                CableMaster.a_entity_type = splitModle.old_cable_a_entity_type;
                CableMaster.a_system_id = splitModle.old_cable_a_system_id;
                CableMaster.a_location = splitModle.old_cable_a_location;
                CableMaster.a_node_type = a_node_type;

                CableMaster.b_entity_type = splitModle.split_entity_type;
                CableMaster.b_system_id = splitModle.split_entity_system_id;
                CableMaster.b_location = splitModle.split_entity_networkId;
                CableMaster.b_node_type = b_node_type;
            }
            else
            {
                CableMaster.a_entity_type = splitModle.split_entity_type;
                CableMaster.a_system_id = splitModle.split_entity_system_id;
                CableMaster.a_location = splitModle.split_entity_networkId;
                CableMaster.a_node_type = a_node_type;

                CableMaster.b_entity_type = splitModle.old_cable_b_entity_type;
                CableMaster.b_system_id = splitModle.old_cable_b_system_id;
                CableMaster.b_location = splitModle.old_cable_b_location;
                CableMaster.b_node_type = b_node_type;
            }

            CableMaster.cable_calculated_length = (float)cable_calculated_length;
            CableMaster.cable_measured_length = (float)cable_measured_length;
            CableMaster.cable_name = cable_name;
            CableMaster.network_id = cable_network_id;
            CableMaster.ispLineGeom = geom;
            CableMaster.geom = "0";

            return CableMaster;
        }

        public ActionResult saveSplitCable(SplitCableMDU model)
        {
            model.userId = Convert.ToInt32(Session["user_id"]);
            PageMessage objPM = new PageMessage();
            try
            {
                CableMaster cableDetail = new BLMisc().GetEntityDetailById<CableMaster>(model.split_cable_system_id, EntityType.Cable);
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


                var cableobjCable1 = getCableObject(1, model, cableDetail, model.cable_one_a_location, model.cable_one_b_location, model.cable_one_measured_length, model.cable_one_calculated_length, model.cable_one_name, model.cable_one_network_id, model.ispLineGeomCableOne, model.cable_one_a_node_type, model.cable_one_b_node_type);
                SaveCable(cableobjCable1, false);

                var cableobjCable2 = getCableObject(2, model, cableDetail, model.cable_two_a_location, model.cable_two_b_location, model.cable_two_measured_length, model.cable_two_calculated_length, model.cable_two_name, model.cable_two_network_id, model.ispLineGeomCableTwo, model.cable_two_a_node_type, model.cable_two_b_node_type);
                SaveCable(cableobjCable2, false);


                model.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
                // make connection with split cable
                BLCable.Instance.SetISPConnectionWithSplitCable(model.cable_one_network_id, model.cable_two_network_id, model.split_cable_system_id, model.split_entity_system_id, model.split_entity_networkId, model.split_entity_type, model.userId, model.splicing_source, model.split_entity_x, model.split_entity_y);

                BLCable.Instance.DeleteCableById(model.split_cable_system_id);
                string[] LayerName = { EntityType.Cable.ToString() };
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, LayerName);//"Cable splited successfully.";
                model.objPM = objPM;




                return PartialView("_SplitCableMDU", model);
            }
            catch (Exception) { throw; }
        }


        public JsonResult SaveCablegeom(int systemId, string cableGeom, int structureId)
        {
            string[] LayerName = { EntityType.Cable.ToString() };
            PageMessage objPM = new PageMessage();
            IspLineMaster objLine = new IspLineMaster();
            objLine.entity_id = systemId;
            objLine.line_geom = cableGeom;
            objLine.structure_id = structureId;
            objLine.entity_type = EntityType.Cable.ToString();
            objLine.modified_by = Convert.ToInt32(Session["user_id"]);
            BLCable.Instance.updateLinegeom(objLine);
            objPM.status = ResponseStatus.OK.ToString();
            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_ISP_CAB_GBL_GBL_001, ApplicationSettings.listLayerDetails, LayerName); //"Cable Location updated successfully.";
            return Json(new { Status = objPM.status, Message = objPM.message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Add Duct

        public PartialViewResult AddDuct(LineEntityIn objIn)
        {
            DuctMaster objDuct = new DuctMaster();
            objDuct = GetISPDuctInfo(objIn);
            var objStructureDetail = new BLMisc().GetEntityDetailById<StructureMaster>(objIn.ModelInfo.structureid, EntityType.Structure);

            if (objIn.systemId == 0)
            {
                //For ISP
                var geom = objStructureDetail.longitude + " " + objStructureDetail.latitude;
                objIn.geom = geom;
                GetISPLineNtkDetail(objDuct, objIn, EntityType.Duct.ToString(), false, geom);

            }
            objDuct.system_id = objIn.systemId;
            objDuct.manual_length = objDuct.calculated_length;
            objDuct.user_id = Convert.ToInt32(Session["user_id"]);
            BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
            objDuct.structure_id = objIn.ModelInfo.structureid;
            if (objStructureDetail != null)
            {
                objDuct.region_id = objStructureDetail.region_id;
                objDuct.province_id = objStructureDetail.province_id;
            }

            fillProjectSpecifications(objDuct);
            //fillRegionProvinceDetail(objDuct, GeometryType.Line.ToString(), objIn.geom);
            BindDuctDropDown(objDuct);
            var layerdetails = new BLLayer().getLayer(EntityType.Duct.ToString());
            objDuct.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
            objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
            objDuct.lstUserModule = new BLLayer().GetUserModuleAbbrList(objDuct.user_id, UserType.Web.ToString());

            return PartialView("_AddISPDuct", objDuct);
        }
        private void BindDuctDropDown(DuctMaster objDuctIn)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Duct.ToString());
            objDuctIn.NoofDuctsCreated = objDDL.Where(x => x.dropdown_type == DropDownType.No_of_Ducts_Created.ToString()).ToList();
            objDuctIn.DuctTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Type.ToString()).ToList();
            objDuctIn.DuctColorIn = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Color.ToString()).ToList();
            objDuctIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            var _objDDL = new BLMisc().GetDropDownList("");
            objDuctIn.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
            // objDuctIn.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
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
                        if (string.IsNullOrEmpty(objEntityModel.area_id))
                        {
                            if (item.entity_type.ToUpper() == EntityType.Area.ToString().ToUpper())
                            {
                                objEntityModel.area_id = item.entity_network_id;
                            }
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

        private DuctMaster GetISPDuctInfo(LineEntityIn objIn)
        {
            DuctMaster objDuct = new DuctMaster();
            if (objIn.systemId == 0)
            {
                objDuct.duct_type = objIn.cableType;
                objDuct.networkIdType = objIn.networkIdType;
                objDuct.ownership_type = "Own";
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<DuctTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Duct, objIn.cableType);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objDuct);

            }
            else
            {
                objDuct = new BLMisc().GetEntityDetailById<DuctMaster>(objIn.systemId, EntityType.Duct);
                if (objIn.lstTP.Count > 0)
                {
                    objDuct.a_location = objIn.lstTP[0].network_id;
                    objDuct.a_system_id = objIn.lstTP[0].system_id;
                    objDuct.a_entity_type = objIn.lstTP[0].network_name;
                    objDuct.a_node_type = objIn.lstTP[0].node_type;

                    objDuct.b_location = objIn.lstTP[1].network_id;
                    objDuct.b_system_id = objIn.lstTP[1].system_id;
                    objDuct.b_entity_type = objIn.lstTP[1].network_name;
                    objDuct.b_node_type = objIn.lstTP[1].node_type;
                }
            }

            return objDuct;
        }

        public ActionResult SaveDuct(DuctMaster objDuct, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            bool isValid = true;
            if (objDuct.networkIdType == NetworkIdType.A.ToString() && objDuct.system_id == 0)
            {
                if (isDirectSave == false)
                {
                    objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.a_system_id, network_id = objDuct.a_location, network_name = objDuct.a_entity_type, node_type = objDuct.a_node_type });
                    objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.b_system_id, network_id = objDuct.b_location, network_name = objDuct.b_entity_type, node_type = objDuct.b_node_type });
                }
                var objLineEntity = new LineEntityIn() { geom = objDuct.geom, systemId = objDuct.system_id, cableType = objDuct.duct_type, networkIdType = objDuct.networkIdType, lstTP = objDuct.lstTP };

                if (isDirectSave == true)
                {

                    objDuct = GetISPDuctInfo(objLineEntity);
                    var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
                    objDuct.bom_sub_category = objBOMDDL[0].dropdown_value;
                }
                var listStructure = new BLMisc().GetEntityDetailById<StructureMaster>(objDuct.structure_id, EntityType.Structure);
                objDuct.geom = listStructure.longitude + " " + listStructure.latitude;

                //GET AUTO NETWORK CODE...
                GetISPLineNtkDetail(objDuct, objLineEntity, EntityType.Cable.ToString(), true, objDuct.geom);
                if (isDirectSave == true)
                    objDuct.duct_name = objDuct.network_id;
                objDuct.geom = listStructure.longitude + " " + listStructure.latitude + "," + listStructure.longitude + " " + listStructure.latitude + "1";
            }

            objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
            if (TryValidateModel(objDuct) && isValid == true)
            {
                var isNew = objDuct.system_id > 0 ? false : true;
                var resultItem = new BLDuct().SaveDuct(objDuct, Convert.ToInt32(Session["user_id"]));
                if (string.IsNullOrEmpty(resultItem.objPM.message))
                {
                    string[] LayerName = { EntityType.Duct.ToString() };
                    if (isNew)
                    {
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.isNewEntity = isNew;
                        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "Cable saved successfully.";
                    }
                    else
                    {
                        objPM.status = ResponseStatus.OK.ToString();
                        objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "Cable updated successfully.";
                    }
                    objDuct.objPM = objPM;
                }
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = isValid == true ? getFirstErrorFromModelState() : objPM.message;
                objDuct.objPM = objPM;
            }
            return Json(objDuct, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ONT

        public PartialViewResult AddONT(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {

            //ONTMaster objONTMaster = GetONTDetail(networkIdType, ModelInfo.templateId, systemId, ModelInfo, pSystemId, pEntityType, pNetworkId);
            //BLItemTemplate.Instance.BindItemDropdowns(objONTMaster, EntityType.ONT.ToString());
            ////BindONTDropDown(objONTMaster);
            //BindONT_Dropdown(objONTMaster);
            //fillProjectSpecifications(objONTMaster);
            ////if (systemId != 0)
            ////{
            ////    var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.ONT.ToString());
            ////    objONTMaster.objIspEntityMap.floor_id = ispMapping.floor_id;
            ////    objONTMaster.objIspEntityMap.shaft_id = ispMapping.shaft_id;
            ////    objONTMaster.objIspEntityMap.structure_id = ispMapping.structure_id;
            ////}
            ////else
            ////{
            ////    var structureDetails = new BLISP().GetStructureById(ModelInfo.structureid);
            ////    if (structureDetails != null && structureDetails.Count > 0)
            ////    {
            ////        objONTMaster.region_id = structureDetails.First().region_id;
            ////        objONTMaster.province_id = structureDetails.First().province_id;
            ////        objONTMaster.latitude = structureDetails.First().latitude;
            ////        objONTMaster.longitude = structureDetails.First().longitude;
            ////        objONTMaster.parent_network_id = structureDetails.First().network_id;
            ////    }
            ////    objONTMaster.objIspEntityMap.floor_id = ModelInfo.floorid;
            ////    objONTMaster.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            ////    objONTMaster.objIspEntityMap.structure_id = ModelInfo.structureid;
            ////}
            //new MiscHelper().BindPortDetails(objONTMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
            //return PartialView("_AddONT", objONTMaster);


            ONTMaster model = new ONTMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.pEntityType = pEntityType;
            model.pSystemId = pSystemId;
            model.pNetworkId = pNetworkId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ONTMaster>(url, model, EntityType.ONT.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            response.results.lstCpeType = new BLMisc().GetDropDownList("", "CPE TYPE");

            BLLayer objBLLayer = new BLLayer();
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;
            model.lstUserModule = objBLLayer.GetUserModuleAbbrList(usrId, UserType.Web.ToString());

            return PartialView("_AddONT", response.results);
        }
        public ONTMaster GetONTDetail(string networkIdType, int templateId, int systemId, ElementInfo ModelInfo, int pSystemId, string pEntityType, string pNetworkId)
        {

            ONTMaster objONT = new ONTMaster();
            if (systemId == 0)
            {
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objONT, structureDetails, GeometryType.Point.ToString());
                //Fill Parent detail...              
                fillParentDetail(objONT, new ISPNetworkCodeIn() { parent_sysId = pSystemId, parent_eType = pEntityType, eType = EntityType.ONT.ToString(), structureId = ModelInfo.structureid }, networkIdType, pSystemId, pEntityType, pNetworkId);
                //Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<ONTTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.ONT);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objONT);
                objONT.objIspEntityMap.shaft_id = ModelInfo.shaftid;
                objONT.objIspEntityMap.floor_id = ModelInfo.floorid;
                objONT.objIspEntityMap.structure_id = ModelInfo.structureid;
                objONT.ownership_type = "Own";
            }
            else
            {
                objONT = new BLMisc().GetEntityDetailById<ONTMaster>(systemId, EntityType.ONT);
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.ONT.ToString());
                if (ispMapping != null && ispMapping.id > 0)
                {
                    objONT.objIspEntityMap.shaft_id = ispMapping.shaft_id;
                    objONT.objIspEntityMap.floor_id = ispMapping.floor_id;
                    objONT.objIspEntityMap.structure_id = ispMapping.structure_id;
                }
            }
            return objONT;
        }
        private void BindONT_Dropdown(ONTMaster ObjONT)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.FMS.ToString());
            ObjONT.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }

        private void BindONTDropDown(ONTMaster objONT)
        {
            var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objONT.parent_system_id, objONT.system_id, EntityType.ONT.ToString());
            if (ispEntityMap != null)
            {
                objONT.objIspEntityMap.id = ispEntityMap.id;
                objONT.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                objONT.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
            }
            objONT.objIspEntityMap.AssoType = objONT.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objONT.objIspEntityMap.floor_id > 0 ? "Floor" : "");
            var objDDL = new BLBDB().GetShaftFloorByStrucId(objONT.parent_system_id);
            objONT.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
            objONT.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).ToList();

        }
        public ActionResult SaveONT(ONTMaster modelMaster, bool isDirectSave = false)
        {

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int structure_id = modelMaster.objIspEntityMap.structure_id;
            //int pSystemId = modelMaster.pSystemId;
            //string pEntityType = modelMaster.pEntityType;
            //int floor_id = modelMaster.objIspEntityMap.floor_id ?? 0;
            //int shaft_id = modelMaster.objIspEntityMap.shaft_id ?? 0;
            //if (modelMaster.networkIdType == NetworkIdType.A.ToString() && modelMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = modelMaster.pSystemId, parent_eType = modelMaster.pEntityType, eType = EntityType.ONT.ToString(), structureId = structure_id });
            //    if (isDirectSave == true)
            //    {
            //        modelMaster = GetONTDetail(modelMaster.networkIdType, modelMaster.templateId, modelMaster.system_id, new ElementInfo { structureid = modelMaster.objIspEntityMap.structure_id, shaftid = Convert.ToInt32(modelMaster.objIspEntityMap.shaft_id), floorid = Convert.ToInt32(modelMaster.objIspEntityMap.floor_id) }, modelMaster.pSystemId, modelMaster.pEntityType, modelMaster.pNetworkId);
            //        //modelMaster.objIspEntityMap.structure_id = structure_id;
            //        //modelMaster.objIspEntityMap.floor_id = floor_id;
            //        //modelMaster.objIspEntityMap.shaft_id = shaft_id;
            //        modelMaster.ont_name = objISPNetworkCode.network_code;
            //        //var structureDetails = new BLISP().GetStructureById(structure_id);
            //        //if (structureDetails != null && structureDetails.Count > 0)
            //        //{
            //        //    modelMaster.region_id = structureDetails.First().region_id;
            //        //    modelMaster.province_id = structureDetails.First().province_id;
            //        //   modelMaster.latitude = structureDetails.First().latitude;
            //        //   modelMaster.longitude = structureDetails.First().longitude;
            //        //   modelMaster.parent_network_id = structureDetails.First().network_id;
            //        //}
            //    }
            //    modelMaster.network_id = objISPNetworkCode.network_code;
            //    modelMaster.sequence_id = objISPNetworkCode.sequence_id;

            //}

            //if (TryValidateModel(modelMaster))
            //{
            //    bool isNew = modelMaster.system_id == 0 ? true : false;
            //    if (modelMaster.unitValue != null && modelMaster.unitValue.Contains(":"))
            //    {
            //        modelMaster.no_of_input_port = Convert.ToInt32(modelMaster.unitValue.Split(':')[0]);
            //        modelMaster.no_of_output_port = Convert.ToInt32(modelMaster.unitValue.Split(':')[1]);
            //    }
            //    var resultItem = new BLONT().SaveONTEntity(modelMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.ONT.ToString() };
            //        if (isNew)
            //        {

            //            objPM.status = ResponseStatus.OK.ToString(); ;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
            //            objPM.systemId = resultItem.system_id;
            //            objPM.entityType = EntityType.ONT.ToString();
            //            objPM.NetworkId = resultItem.network_id;
            //            objPM.structureId = modelMaster.objIspEntityMap.structure_id;
            //            objPM.shaftId = modelMaster.objIspEntityMap.shaft_id ?? 0;
            //            objPM.floorId = modelMaster.objIspEntityMap.floor_id ?? 0;
            //            objPM.pSystemId = modelMaster.parent_system_id;
            //            objPM.pEntityType = modelMaster.parent_entity_type;
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
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "ONT updated successfully.";
            //            }
            //        }
            //        modelMaster.objPM = objPM;
            //    }

            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState();
            //    modelMaster.objPM = objPM;
            //}
            //modelMaster.entityType = EntityType.ONT.ToString();
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(modelMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(modelMaster, EntityType.ONT.ToString());
            //    BindONT_Dropdown(modelMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA
            //    fillProjectSpecifications(modelMaster);
            //    new MiscHelper().BindPortDetails(modelMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
            //    return PartialView("_AddONT", modelMaster);
            //}

            modelMaster.user_id = Convert.ToInt32(Session["user_id"]);
            modelMaster.cpe_type = string.IsNullOrEmpty(modelMaster.cpe_type) ? "ONT" : modelMaster.cpe_type;
            modelMaster.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ONTMaster>(url, modelMaster, EntityType.ONT.ToString(), EntityAction.Save.ToString(), modelMaster.objIspEntityMap.structure_id.ToString());
            response.results.lstCpeType = new BLMisc().GetDropDownList("", "CPE TYPE");
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }

            BLLayer objBLLayer = new BLLayer();
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;
            modelMaster.lstUserModule = objBLLayer.GetUserModuleAbbrList(usrId, UserType.Web.ToString());

            return PartialView("_AddONT", response.results);
        }
        public JsonResult DeleteONTById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            var result = 0;
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.ONT.ToString());
            //if (isNotAssociated == true) { result = new BLONT().DeleteONTById(systemId); }
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.ONT.ToString(), GeometryType.Point.ToString(),usrDetail.user_id);
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

        #region Customer
        public PartialViewResult AddCustomer(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {

            Customer objCustomer = GetCustomerDetail(networkIdType, systemId, ModelInfo.structureid, pSystemId, pEntityType);
            objCustomer.pSystemId = pSystemId;
            objCustomer.pEntityType = pEntityType;
            objCustomer.parent_network_id = pNetworkId;
            BindCustomerDropDown(objCustomer);
            //Get the layer details to bind additional attributes Customer
            var layerdetails = new BLLayer().getLayer(EntityType.Customer.ToString());
            objCustomer.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
            //End for additional attributes Customer
            if (systemId != 0)
            {
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.Customer.ToString());
                objCustomer.objIspEntityMap.floor_id = ispMapping.floor_id;
                objCustomer.objIspEntityMap.shaft_id = ispMapping.shaft_id;
                objCustomer.objIspEntityMap.structure_id = ispMapping.structure_id;


            }
            else
            {
                objCustomer.objIspEntityMap.structure_id = ModelInfo.structureid; objCustomer.parent_system_id = pSystemId; objCustomer.parent_entity_type = pEntityType;
                objCustomer.objIspEntityMap.floor_id = ModelInfo.floorid;
                objCustomer.objIspEntityMap.shaft_id = ModelInfo.shaftid;
                objCustomer.objIspEntityMap.structure_id = ModelInfo.structureid;
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                objCustomer.geom = structureDetails.longitude + " " + structureDetails.latitude;

            }
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
                return PartialView("~/Views/Library/_AddSiteCustomer.cshtml", objCustomer);
            }
            return PartialView("_AddCustomer", objCustomer);

        }
        public Customer GetCustomerDetail(string networkIdType, int systemId, int structureId, int pSystemId, string pEntityType)
        {

            Customer objCustomer = new Customer();
            objCustomer.networkIdType = networkIdType;
            objCustomer.parent_system_id = structureId;
            objCustomer.parent_entity_type = EntityType.Structure.ToString();
            if (pSystemId != 0 && pEntityType != "")
            {
                var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(structureId, pSystemId, pEntityType.ToString());
                if (ispEntityMap != null)
                {
                    objCustomer.objIspEntityMap.id = ispEntityMap.id;
                    objCustomer.objIspEntityMap.floor_id = ispEntityMap.floor_id;
                    objCustomer.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
                    objCustomer.objIspEntityMap.structure_id = ispEntityMap.structure_id;
                }
            }

            if (systemId == 0)
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.Customer.ToString(), structureId = structureId });
                    objCustomer.network_id = ISPNetworkCodeDetail.network_code;
                }
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objCustomer.objIspEntityMap.structure_id, EntityType.Structure);
                fillRegionProvinceDetail(objCustomer, structureDetails, GeometryType.Point.ToString());
                objCustomer.address = BLStructure.Instance.getBuildingAddress(structureId);
                objCustomer.other_info = null;  //for additional-attributes
            }
            else
            {
                objCustomer = new BLMisc().GetEntityDetailById<Customer>(systemId, EntityType.Customer);
                //for additional-attributes
                objCustomer.other_info = new BLCustomer().GetOtherInfoCustomer(objCustomer.system_id);
                fillRegionProvAbbr(objCustomer);
            }
            return objCustomer;
        }
        private void BindCustomerDropDown(Customer objCustomer)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Customer.ToString());
            objCustomer.lstActivationStatus = objDDL.Where(x => x.dropdown_type == DropDownType.Activation_Status.ToString()).ToList();
            objCustomer.lstCustomerType = objDDL.Where(x => x.dropdown_type == DropDownType.Customer_Type.ToString()).ToList();
            objCustomer.lstServiceType = objDDL.Where(x => x.dropdown_type.ToUpper() == DropDownType.Customer_Service_Type.ToString().ToUpper()).ToList();
        }

        public ActionResult SaveCustomer(Customer modelMaster, bool isDirectSave = false)
        {

            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            int structure_id = modelMaster.objIspEntityMap.structure_id;
            int pSystemId = modelMaster.pSystemId;
            string pEntityType = modelMaster.pEntityType;
            string pNetworkId = modelMaster.pNetworkId;
            int floor_id = modelMaster.objIspEntityMap.floor_id ?? 0;
            //modelMaster.parent_entity_type = EntityType.Structure.ToString();

            if (modelMaster.networkIdType == NetworkIdType.A.ToString() && modelMaster.system_id == 0)
            {
                //GET AUTO NETWORK CODE...
                //var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Customer.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCustomer.geom, parent_eType = objCustomer.pEntityType, parent_sysId = objCustomer.pSystemId });
                var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = modelMaster.pSystemId, parent_eType = EntityType.ONT.ToString(), eType = EntityType.Customer.ToString(), structureId = modelMaster.objIspEntityMap.structure_id });
                if (isDirectSave == true)
                {
                    modelMaster = GetCustomerDetail(modelMaster.networkIdType, modelMaster.system_id, structure_id, pSystemId, pEntityType);
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    // modelMaster = GetCustomerDetail(modelMaster.pSystemId, modelMaster.pEntityType, modelMaster.networkIdType, modelMaster.system_id, modelMaster.geom);
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    modelMaster.customer_name = objISPNetworkCode.network_code;
                    modelMaster.parent_system_id = pSystemId;
                    modelMaster.parent_entity_type = pEntityType;
                    modelMaster.parent_network_id = pNetworkId;
                }
                //SET NETWORK CODE
                modelMaster.network_id = objISPNetworkCode.network_code;
                modelMaster.sequence_id = objISPNetworkCode.sequence_id;
            }
            var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(structure_id, EntityType.Structure);
            if (structureDetails != null)
            {
                modelMaster.region_id = structureDetails.region_id;
                modelMaster.province_id = structureDetails.province_id;
                modelMaster.latitude = structureDetails.latitude;
                modelMaster.longitude = structureDetails.longitude;
                modelMaster.geom = structureDetails.longitude + " " + structureDetails.latitude;
            }
            if (TryValidateModel(modelMaster))
            {
                bool isNew = modelMaster.system_id == 0 ? true : false;
                var result = new BLCustomer().SaveCustomer(modelMaster, Convert.ToInt32(Session["user_id"]));
                if (string.IsNullOrEmpty(result.objPM.message))
                {

                    if (isNew)
                    {
                        objPM.status = ResponseStatus.OK.ToString(); ;
                        objPM.message = Resources.Resources.SI_OSP_CUS_NET_FRM_005;// "Customer saved successfully.";
                        objPM.systemId = result.system_id;
                        objPM.entityType = EntityType.Customer.ToString();
                        objPM.NetworkId = result.network_id;
                        objPM.structureId = modelMaster.objIspEntityMap.structure_id;
                        objPM.shaftId = modelMaster.objIspEntityMap.shaft_id ?? 0;
                        objPM.floorId = modelMaster.objIspEntityMap.floor_id ?? 0;
                        objPM.pSystemId = modelMaster.parent_system_id;
                        objPM.pEntityType = modelMaster.parent_entity_type;
                    }
                    else
                    {
                        objPM.status = ResponseStatus.OK.ToString(); ;
                        objPM.message = Resources.Resources.SI_OSP_CUS_NET_FRM_006;// "Customer updated successfully.";
                    }
                    modelMaster.objPM = objPM;
                }

            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = getFirstErrorFromModelState();
                modelMaster.objPM = objPM;
            }
            modelMaster.entityType = EntityType.Customer.ToString();
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(modelMaster.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {
                BindCustomerDropDown(modelMaster);
                //Get the layer details to bind additional attributes Customer
                var layerdetails = new BLLayer().getLayer(EntityType.Customer.ToString());
                modelMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
                //End for additional attributes Customer
                return PartialView("_AddCustomer", modelMaster);
            }
        }
        public JsonResult DeleteCustomerById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();
            //var isNotAssociated = BLISP.Instance.checkEntityAssociation(systemId, EntityType.Customer.ToString());
            //if (isNotAssociated == true) { result = new BLCustomer().DeleteCustomerById(systemId); }
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.Customer.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
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

        #region FMS
        public FMSMaster GetFMSDetail(string networkIdType, int templateId, int systemId, ElementInfo ModelInfo, int pSystemId, string pEntityType, string pNetworkId)
        {
            FMSMaster objFMS = new FMSMaster();
            objFMS.networkIdType = networkIdType;
            //objFMS.objIspEntityMap.structure_id = structureId;
            //if (pSystemId != 0 && pEntityType != "")
            //{
            //    var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(structureId, pSystemId, pEntityType.ToString());
            //    if (ispEntityMap != null)
            //    {
            //        objFMS.objIspEntityMap.id = ispEntityMap.id;
            //        objFMS.objIspEntityMap.floor_id = ispEntityMap.floor_id;
            //        objFMS.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
            //    }
            //}
            if (systemId != 0)
            {
                objFMS = new BLMisc().GetEntityDetailById<FMSMaster>(systemId, EntityType.FMS, objFMS.user_id);
                objFMS.no_of_port = objFMS.no_of_port;
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.FMS.ToString());
                if (ispMapping != null && ispMapping.id > 0)
                {
                    objFMS.objIspEntityMap.shaft_id = ispMapping.shaft_id;
                    objFMS.objIspEntityMap.floor_id = ispMapping.floor_id;
                    objFMS.objIspEntityMap.structure_id = ispMapping.structure_id;
                }
            }
            else
            {
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objFMS, structureDetails, GeometryType.Point.ToString());
                //Fill Parent detail...              
                fillParentDetail(objFMS, new ISPNetworkCodeIn() { parent_sysId = ModelInfo.structureid, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FMS.ToString(), structureId = ModelInfo.structureid }, networkIdType, pSystemId, pEntityType, pNetworkId);
                //if (networkIdType == NetworkIdType.M.ToString())
                //{
                //    // for Manual network id type 
                //    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail();
                //    objFMS.network_id = ISPNetworkCodeDetail.network_code;
                //}
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<FMSTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.FMS);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objFMS);
                objFMS.no_of_port = objItem.no_of_port;
                objFMS.objIspEntityMap.shaft_id = ModelInfo.shaftid;
                objFMS.objIspEntityMap.floor_id = ModelInfo.floorid;
                objFMS.objIspEntityMap.structure_id = ModelInfo.structureid;
                objFMS.address = BLStructure.Instance.getBuildingAddress(ModelInfo.structureid);
                objFMS.ownership_type = "Own";
            }
            return objFMS;
        }

        public PartialViewResult AddFMS(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //FMSMaster objFMSMaster = GetFMSDetail(networkIdType, ModelInfo.templateId, systemId, ModelInfo, pSystemId, pEntityType, pNetworkId);
            ////if (systemId != 0)
            ////{
            ////    var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.FMS.ToString());
            ////    objFMSMaster.parent_system_id = pSystemId;
            ////    objFMSMaster.parent_entity_type = pEntityType;
            ////    objFMSMaster.objIspEntityMap.structure_id = ispMapping.structure_id;
            ////}
            ////else
            ////{
            ////    objFMSMaster.parent_system_id = pSystemId;
            ////    objFMSMaster.parent_entity_type = pEntityType;
            ////    objFMSMaster.objIspEntityMap.structure_id = ModelInfo.structureid;
            ////}
            //BLItemTemplate.Instance.BindItemDropdowns(objFMSMaster, EntityType.FMS.ToString());
            //BindFMSDropdown(objFMSMaster);
            //fillProjectSpecifications(objFMSMaster);
            //new MiscHelper().BindPortDetails(objFMSMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
            ////objFMSMaster.pNetworkId = pNetworkId;
            //return PartialView("_AddFMS", objFMSMaster);
            FMSMaster model = new FMSMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.pEntityType = pEntityType;
            model.pSystemId = pSystemId;
            model.pNetworkId = pNetworkId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FMSMaster>(url, model, EntityType.FMS.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddFMS", response.results);
        }
        public ActionResult SaveFMS(FMSMaster objFMSMaster, bool isDirectSave = false)
        {
            //string pNetworkId = objFMSMaster.pNetworkId;
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int structure_id = objFMSMaster.objIspEntityMap.structure_id;
            //int pSystemId = objFMSMaster.pSystemId;
            //string pEntityType = objFMSMaster.pEntityType;
            //int floor_id = objFMSMaster.objIspEntityMap.floor_id ?? 0;
            //int shaft_id = objFMSMaster.objIspEntityMap.shaft_id ?? 0;
            ////var structureDetails = new BLISP().GetStructureById(structure_id);
            ////if (structureDetails != null)
            ////{
            ////    objFMSMaster.province_id = structureDetails.First().province_id;
            ////}
            //if (objFMSMaster.networkIdType == NetworkIdType.A.ToString() && objFMSMaster.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = pSystemId, parent_eType = pEntityType, eType = EntityType.FMS.ToString(), structureId = objFMSMaster.objIspEntityMap.structure_id });
            //    if (isDirectSave == true)
            //    {
            //        objFMSMaster = GetFMSDetail(objFMSMaster.networkIdType, objFMSMaster.templateId, objFMSMaster.system_id, new ElementInfo { structureid = objFMSMaster.objIspEntityMap.structure_id, shaftid = Convert.ToInt32(objFMSMaster.objIspEntityMap.shaft_id), floorid = Convert.ToInt32(objFMSMaster.objIspEntityMap.floor_id) }, pSystemId, pEntityType, pNetworkId);
            //        //objFMSMaster.objIspEntityMap.structure_id = structure_id;
            //        //objFMSMaster.parent_system_id = pSystemId;
            //        //objFMSMaster.parent_entity_type = pEntityType;
            //        objFMSMaster.fms_name = objISPNetworkCode.network_code;
            //        //objFMSMaster.pSystemId = pSystemId;
            //        //objFMSMaster.pEntityType = pEntityType;
            //        //objFMSMaster.pNetworkId = pNetworkId;
            //        //objFMSMaster.objIspEntityMap.floor_id = floor_id;
            //        //objFMSMaster.objIspEntityMap.shaft_id = shaft_id;
            //    }
            //    objFMSMaster.network_id = objISPNetworkCode.network_code;
            //    objFMSMaster.sequence_id = objISPNetworkCode.sequence_id;

            //}
            ////if (structureDetails != null)
            ////{
            ////    objFMSMaster.region_id = structureDetails.First().region_id;
            ////    objFMSMaster.province_id = structureDetails.First().province_id;
            ////    objFMSMaster.latitude = structureDetails.First().latitude;
            ////    objFMSMaster.longitude = structureDetails.First().longitude;
            ////}
            //if (objFMSMaster.unitValue != null && objFMSMaster.unitValue.Contains(":"))
            //{
            //    objFMSMaster.no_of_input_port = Convert.ToInt32(objFMSMaster.unitValue.Split(':')[0]);
            //    objFMSMaster.no_of_output_port = Convert.ToInt32(objFMSMaster.unitValue.Split(':')[1]);
            //}
            //if (TryValidateModel(objFMSMaster))
            //{
            //    bool isNew = objFMSMaster.system_id == 0 ? true : false;
            //    var resultItem = new BLFMS().SaveEntityFMS(objFMSMaster, Convert.ToInt32(((User)Session["userDetail"]).user_id));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        string[] LayerName = { EntityType.FMS.ToString() };
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString(); ;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "FMS saved successfully.";
            //            objPM.systemId = resultItem.system_id;
            //            objPM.entityType = EntityType.FMS.ToString();
            //            objPM.NetworkId = resultItem.network_id;
            //            objPM.structureId = objFMSMaster.objIspEntityMap.structure_id;
            //            objPM.shaftId = objFMSMaster.objIspEntityMap.shaft_id ?? 0;
            //            objPM.floorId = objFMSMaster.objIspEntityMap.floor_id ?? 0;
            //            objPM.pSystemId = objFMSMaster.parent_system_id;
            //            objPM.pEntityType = objFMSMaster.parent_entity_type;
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
            //                objPM.status = ResponseStatus.OK.ToString(); ;
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "FMS updated successfully.";
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
            //objFMSMaster.entityType = EntityType.FMS.ToString();
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(objFMSMaster.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(objFMSMaster, EntityType.FMS.ToString());
            //    BindFMSDropdown(objFMSMaster);
            //    fillProjectSpecifications(objFMSMaster);
            //    new MiscHelper().BindPortDetails(objFMSMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
            //    objFMSMaster.pNetworkId = pNetworkId;
            //    return PartialView("_AddFMS", objFMSMaster);
            //}

            objFMSMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objFMSMaster.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FMSMaster>(url, objFMSMaster, EntityType.FMS.ToString(), EntityAction.Save.ToString(), objFMSMaster.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddFMS", response.results);
        }
        public JsonResult DeleteFMSById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.FMS.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
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
        private void BindFMSDropdown(FMSMaster objFMS)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.FMS.ToString());
            objFMS.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }
        #endregion

        #region POD
        public PartialViewResult AddPOD(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {

            //PODMaster objPODMaster = GetPODDetail(networkIdType, systemId, ModelInfo, pSystemId, pEntityType, pNetworkId);
            //BLItemTemplate.Instance.BindItemDropdowns(objPODMaster, EntityType.POD.ToString());
            //BindPODDopdown(objPODMaster);
            //fillProjectSpecifications(objPODMaster);
            //return PartialView("_AddPOD", objPODMaster);

            PODMaster model = new PODMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.pEntityType = pEntityType;
            model.pSystemId = pSystemId;
            model.pNetworkId = pNetworkId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PODMaster>(url, model, EntityType.POD.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddPOD", response.results);
        }
        public PODMaster GetPODDetail(string networkIdType, int systemId, ElementInfo ModelInfo = null, int pSystemId = 0, string pEntityType = "", string pNetworkid = "")
        {
            PODMaster objPOD = new PODMaster();
            objPOD.networkIdType = networkIdType;
            if (systemId == 0)
            {
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objPOD, structureDetails, GeometryType.Point.ToString());
                objPOD.ownership_type = "Own";
                //Fill Parent detail...              
                fillParentDetail(objPOD, new ISPNetworkCodeIn() { parent_sysId = structureDetails.province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), parent_networkId = pNetworkid, eType = EntityType.POD.ToString(), structureId = ModelInfo.structureid }, networkIdType, pSystemId, pEntityType, pNetworkid);
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<PODTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.POD);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objPOD);
                objPOD.objIspEntityMap.shaft_id = ModelInfo.shaftid;
                objPOD.objIspEntityMap.floor_id = ModelInfo.floorid;
                objPOD.objIspEntityMap.structure_id = ModelInfo.structureid;
                objPOD.address = BLStructure.Instance.getBuildingAddress(ModelInfo.structureid);
            }
            else
            {
                // Get entity detail by Id...
                objPOD = new BLMisc().GetEntityDetailById<PODMaster>(systemId, EntityType.POD);
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.POD.ToString());
                if (ispMapping != null)
                {
                    objPOD.objIspEntityMap.shaft_id = ispMapping.shaft_id;
                    objPOD.objIspEntityMap.floor_id = ispMapping.floor_id;
                    objPOD.objIspEntityMap.structure_id = ispMapping.structure_id;
                }
            }
            return objPOD;
        }
        public ActionResult SavePOD(PODMaster objPODMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objPODMaster.networkIdType == NetworkIdType.A.ToString() && objPODMaster.system_id == 0)
            //{
            //    var structureDetails = new BLISP().GetStructureById(objPODMaster.objIspEntityMap.structure_id);
            //    //GET AUTO NETWORK CODE...               
            //    var objNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureDetails.First().province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), eType = EntityType.POD.ToString(), structureId = objPODMaster.objIspEntityMap.structure_id });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objPODMaster = GetPODDetail(objPODMaster.networkIdType, objPODMaster.system_id, new ElementInfo { structureid = objPODMaster.objIspEntityMap.structure_id, shaftid = Convert.ToInt32(objPODMaster.objIspEntityMap.shaft_id), floorid = Convert.ToInt32(objPODMaster.objIspEntityMap.floor_id) }, objPODMaster.pSystemId, objPODMaster.pEntityType, objPODMaster.pNetworkId);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objPODMaster.pod_name = objNetworkCodeDetail.network_code;
            //        //if (structureDetails != null && structureDetails.Count > 0)
            //        //{
            //        //    objPODMaster.region_id = structureDetails.First().region_id;
            //        //    objPODMaster.province_id = structureDetails.First().province_id;
            //        //    objPODMaster.latitude = Convert.ToDecimal(structureDetails.First().latitude);
            //        //    objPODMaster.longitude = Convert.ToDecimal(structureDetails.First().longitude);
            //        //    objPODMaster.parent_network_id = structureDetails.First().network_id;
            //        //}
            //    }
            //    //SET NETWORK CODE
            //    objPODMaster.network_id = objNetworkCodeDetail.network_code;
            //    objPODMaster.sequence_id = objNetworkCodeDetail.sequence_id;

            //}
            //if (TryValidateModel(objPODMaster))
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.POD.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    var isNew = objPODMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLPOD().SaveEntityPOD(objPODMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        //Save Reference
            //        if (objPODMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objPODMaster.EntityReference, resultItem.system_id);
            //        }
            //        objPODMaster.extraAttributes.system_id = resultItem.system_id;
            //        objPODMaster.extraAttributes.entity_type = EntityType.POD.ToString();
            //        new BLAdditionalAttributes().SaveAttributes(objPODMaster.extraAttributes);
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, layer_title);
            //            objPM.systemId = resultItem.system_id;
            //            objPM.entityType = resultItem.entityType;
            //            objPM.structureId = objPODMaster.objIspEntityMap.structure_id;
            //            objPM.shaftId = objPODMaster.objIspEntityMap.shaft_id ?? 0;
            //            objPM.floorId = objPODMaster.objIspEntityMap.floor_id ?? 0;
            //            objPM.pSystemId = objPODMaster.parent_system_id;
            //            objPM.pEntityType = objPODMaster.parent_entity_type;
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
            //    BindPODDopdown(objPODMaster);
            //    fillProjectSpecifications(objPODMaster);
            //    return PartialView("_AddPOD", objPODMaster);
            //}

            objPODMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objPODMaster.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PODMaster>(url, objPODMaster, EntityType.POD.ToString(), EntityAction.Save.ToString(), objPODMaster.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddPOD", response.results);
        }
        private void BindPODDopdown(PODMaster objPODMaster)
        {
            objPODMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }
        #endregion

        #region MPOD
        public PartialViewResult AddMPOD(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //MPODMaster objMPODMaster = GetMPODDetail(networkIdType, systemId, ModelInfo, pSystemId, pEntityType, pNetworkId);
            //BLItemTemplate.Instance.BindItemDropdowns(objMPODMaster, EntityType.MPOD.ToString());
            //BindMPODDropdown(objMPODMaster);
            //fillProjectSpecifications(objMPODMaster);
            //return PartialView("_AddMPOD", objMPODMaster);

            MPODMaster model = new MPODMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.pEntityType = pEntityType;
            model.pSystemId = pSystemId;
            model.pNetworkId = pNetworkId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<MPODMaster>(url, model, EntityType.MPOD.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddMPOD", response.results);
        }
        public MPODMaster GetMPODDetail(string networkIdType, int systemId, ElementInfo ModelInfo = null, int pSystemId = 0, string pEntityType = "", string pNetworkid = "")
        {
            MPODMaster objMPOD = new MPODMaster();
            objMPOD.networkIdType = networkIdType;
            if (systemId == 0)
            {
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objMPOD, structureDetails, GeometryType.Point.ToString());
                //Fill Parent detail...              
                fillParentDetail(objMPOD, new ISPNetworkCodeIn() { parent_sysId = structureDetails.province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), parent_networkId = pNetworkid, eType = EntityType.MPOD.ToString(), structureId = ModelInfo.structureid }, networkIdType, pSystemId, pEntityType, pNetworkid);
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<MPODTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.MPOD);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objMPOD);
                objMPOD.objIspEntityMap.shaft_id = ModelInfo.shaftid;
                objMPOD.objIspEntityMap.floor_id = ModelInfo.floorid;
                objMPOD.objIspEntityMap.structure_id = ModelInfo.structureid;
                objMPOD.address = BLStructure.Instance.getBuildingAddress(ModelInfo.structureid);
                objMPOD.ownership_type = "Own";
            }
            else
            {
                // Get entity detail by Id...
                objMPOD = new BLMisc().GetEntityDetailById<MPODMaster>(systemId, EntityType.MPOD);
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.MPOD.ToString());
                if (ispMapping != null)
                {
                    objMPOD.objIspEntityMap.shaft_id = ispMapping.shaft_id;
                    objMPOD.objIspEntityMap.floor_id = ispMapping.floor_id;
                    objMPOD.objIspEntityMap.structure_id = ispMapping.structure_id;
                }
            }
            return objMPOD;
        }
        public ActionResult SaveMPOD(MPODMaster objMPODMaster, bool isDirectSave = false)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //if (objMPODMaster.networkIdType == NetworkIdType.A.ToString() && objMPODMaster.system_id == 0)
            //{
            //    var structureDetails = new BLISP().GetStructureById(objMPODMaster.objIspEntityMap.structure_id);
            //    //GET AUTO NETWORK CODE...               
            //    var objNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureDetails.First().province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), eType = EntityType.MPOD.ToString(), structureId = objMPODMaster.objIspEntityMap.structure_id });
            //    if (isDirectSave == true)
            //    {
            //        //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            //        objMPODMaster = GetMPODDetail(objMPODMaster.networkIdType, objMPODMaster.system_id, new ElementInfo { structureid = objMPODMaster.objIspEntityMap.structure_id, shaftid = Convert.ToInt32(objMPODMaster.objIspEntityMap.shaft_id), floorid = Convert.ToInt32(objMPODMaster.objIspEntityMap.floor_id) }, objMPODMaster.pSystemId, objMPODMaster.pEntityType, objMPODMaster.pNetworkId);
            //        // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
            //        objMPODMaster.mpod_name = objNetworkCodeDetail.network_code;
            //        //if (structureDetails != null && structureDetails.Count > 0)
            //        //{
            //        //    objMPODMaster.region_id = structureDetails.First().region_id;
            //        //    objMPODMaster.province_id = structureDetails.First().province_id;
            //        //    objMPODMaster.latitude = Convert.ToDecimal(structureDetails.First().latitude);
            //        //    objMPODMaster.longitude = Convert.ToDecimal(structureDetails.First().longitude);
            //        //    objMPODMaster.parent_network_id = structureDetails.First().network_id;
            //        //}
            //    }
            //    //SET NETWORK CODE
            //    objMPODMaster.network_id = objNetworkCodeDetail.network_code;
            //    objMPODMaster.sequence_id = objNetworkCodeDetail.sequence_id;

            //}
            //if (TryValidateModel(objMPODMaster))
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.MPOD.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    var isNew = objMPODMaster.system_id > 0 ? false : true;
            //    var resultItem = new BLMPOD().SaveEntityMPOD(objMPODMaster, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(resultItem.objPM.message))
            //    {
            //        //Save Reference
            //        string[] LayerName = { EntityType.MPOD.ToString() };
            //        if (objMPODMaster.EntityReference != null && resultItem.system_id > 0)
            //        {
            //            SaveReference(objMPODMaster.EntityReference, resultItem.system_id);
            //        }
            //        if (isNew)
            //        {
            //            objPM.status = ResponseStatus.OK.ToString();
            //            objPM.isNewEntity = isNew;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
            //            objPM.systemId = resultItem.system_id;
            //            objPM.entityType = resultItem.entityType;
            //            objPM.structureId = objMPODMaster.objIspEntityMap.structure_id;
            //            objPM.shaftId = objMPODMaster.objIspEntityMap.shaft_id ?? 0;
            //            objPM.floorId = objMPODMaster.objIspEntityMap.floor_id ?? 0;
            //            objPM.pSystemId = objMPODMaster.parent_system_id;
            //            objPM.pEntityType = objMPODMaster.parent_entity_type;
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
            //                BLLoopMangment.Instance.UpdateLoopDetails(objMPODMaster.system_id, EntityType.MPOD.ToString(), objMPODMaster.network_id, objMPODMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPODMaster.longitude + " " + objMPODMaster.latitude }, Convert.ToInt32(Session["user_id"]));
            //                objPM.status = ResponseStatus.OK.ToString();
            //                objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName); ;
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
            //    BindMPODDropdown(objMPODMaster);
            //    // RETURN PARTIAL VIEW WITH MODEL DATA              
            //    fillProjectSpecifications(objMPODMaster);
            //    return PartialView("_AddMPOD", objMPODMaster);
            //}

            objMPODMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objMPODMaster.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<MPODMaster>(url, objMPODMaster, EntityType.MPOD.ToString(), EntityAction.Save.ToString(), objMPODMaster.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddMPOD", response.results);
        }
        private void BindMPODDropdown(MPODMaster objMPODMaster)
        {
            objMPODMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }
        #endregion


        private void fillParentDetail(dynamic objLib, ISPNetworkCodeIn objIn, string networkIdType, int pSystemId, string pEntityType, string pNetworkid)
        {
            //fill network code detail....           
            var networkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(objIn);
            if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    //FILL NETWORK CODE FORMAT FOR MANUAL
                    objLib.network_id = networkCodeDetail.network_code;
                }
                objLib.parent_entity_type = pEntityType;
                objLib.parent_network_id = pNetworkid;
                objLib.parent_system_id = pSystemId;
                objLib.pEntityType = pEntityType;
                objLib.pNetworkId = pNetworkid;
                objLib.pSystemId = pSystemId;
            }
        }
        private void fillModelParentDetail(dynamic objLib, ISPNetworkCodeIn objIn, string networkIdType, int pSystemId, string pEntityType, string pNetworkid)
        {
            //fill network code detail....           
            var networkCodeDetail = new BLMisc().GetISPModelNetworkCodeDetail(objIn);
            if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
            {
                //if (networkIdType == NetworkIdType.M.ToString())
                //{
                //    //FILL NETWORK CODE FORMAT FOR MANUAL
                //}
                objLib.network_id = networkCodeDetail.network_code;
                objLib.parent_entity_type = pEntityType;
                objLib.parent_network_id = pNetworkid;
                objLib.parent_system_id = pSystemId;
                objLib.pEntityType = pEntityType;
                objLib.pNetworkId = pNetworkid;
                objLib.pSystemId = pSystemId;
                objLib.sequence_id = networkCodeDetail.sequence_id;
            }
        }
        private void fillRegionProvinceDetail(dynamic objEntityModel, StructureMaster structureDetails, string geomType)
        {
            if (structureDetails != null)
            {
                objEntityModel.region_id = structureDetails.region_id;
                objEntityModel.province_id = structureDetails.province_id;
                System.Reflection.PropertyInfo pi = objEntityModel.GetType().GetProperty("latitude");
                Type t = pi.PropertyType;
                if (t.FullName == "System.Decimal")
                {
                    objEntityModel.latitude = Convert.ToDecimal(structureDetails.latitude);
                    objEntityModel.longitude = Convert.ToDecimal(structureDetails.longitude);
                }
                else
                {
                    objEntityModel.latitude = structureDetails.latitude;
                    objEntityModel.longitude = structureDetails.longitude;
                }

                objEntityModel.region_name = structureDetails.region_name;
                objEntityModel.province_name = structureDetails.province_name;
            }
            var geom = structureDetails.longitude + " " + structureDetails.latitude;
            List<InGeographicDetails> obj = new List<InGeographicDetails>();
            obj = BLBuilding.Instance.GetGeographicDetails(geom, geomType);
            try
            {
                if (obj != null && obj.Count > 0)
                {
                    foreach (var item in obj)
                    {
                        if (string.IsNullOrEmpty(objEntityModel.area_id))
                        {
                            if (item.entity_type.ToUpper() == EntityType.Area.ToString().ToUpper())
                            {
                                objEntityModel.area_id = item.entity_network_id;
                            }
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
        public JsonResult createOSPISPCable(List<OSPISPCable> ospISPCables)
        {
            var response = new BLCable().updateOSPISPLineGeom(ospISPCables);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult updateIspCableGeom(List<IspLineMaster> cableDetails)
        {
            foreach (var item in cableDetails)
            {
                new BLCable().updateLinegeom(new IspLineMaster { entity_id = item.entity_id, line_geom = item.line_geom, structure_id = item.structure_id, modified_by = Convert.ToInt32(Session["user_id"]), entity_type = EntityType.Cable.ToString() });
            }
            return Json(StatusCodes.OK.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateIspCablesPath(List<IspLineMaster> cableDetails)
        {
            cableDetails.ForEach(p => p.modified_by = Convert.ToInt32(Session["user_id"]));
            var result = new BLCable().UpdateCablesPath(JsonConvert.SerializeObject(cableDetails));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetIspInformation(int elementid, string elementtype)
        {
            dynamic expando = new ExpandoObject();
            expando.elementId = elementid;
            expando.elementType = elementtype;

            return PartialView("_ISPInformation", expando);
        }

        public PartialViewResult GetISPEntityInformation(string listObj)
        {
            var lstEntityInformation = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DisplayEntityInformation>>(listObj);
            return PartialView("_InformationList", lstEntityInformation);
        }

        public PartialViewResult GetISPEntityInformationDetail(int systemId, string entityName, string entityTitle, string geomType, string networkStatus)
        {
            var entityInformationDetail = GetEntityInformationDetail(systemId, entityName, entityTitle, geomType, networkStatus);

            return PartialView("_InformationDetail", entityInformationDetail);
        }

        public PartialViewResult GetISPNetworkLayerElements(int structureId)
        {
            var usrDetail = (User)Session["userDetail"];
            var listISPNetworkLayerElements = new BLLayer().GetISPNetworkLayerElements(structureId, usrDetail.role_id);
            return PartialView("_NetworkLayer", listISPNetworkLayerElements);
        }


        private void SaveReference(EntityReference entityReference, int system_id)
        {
            BLReference.Instance.SaveReference(entityReference, system_id);
        }

        private EntityInformationDetail GetEntityInformationDetail(int systemId, string entityName, string entityTitle, string geomType, string networkStatus)
        {
            var currentLang = CultureInfo.CurrentUICulture;
            string culture = currentLang.Name;
            string[] arrIgnoreColumns = { };
            var usrDetail = (User)Session["userDetail"];
            var userId = usrDetail.user_id;
            EntityInformationDetail entityInfoDetail = new EntityInformationDetail();
            entityInfoDetail.system_id = systemId;
            entityInfoDetail.entity_name = entityName;
            entityInfoDetail.entity_title = entityTitle;
            entityInfoDetail.geom_type = geomType;
            entityInfoDetail.network_status = networkStatus;
            entityInfoDetail.EntityKeyValues = new BLMisc().getEntityInfo(systemId, entityName, geomType, userId);
            entityInfoDetail.EntityKeyValues = BLConvertMLanguage.MultilingualConvertModel(entityInfoDetail.EntityKeyValues, arrIgnoreColumns, culture);            //entityInfoDetail.listLayerAction = new BLMisc().getEntityActionInfo(entityName);
            entityInfoDetail.listLayerAction = new BLMisc().getLayerActions(systemId, entityName, false, networkStatus, usrDetail.role_id, userId, false,"","");
            for (int i = 0; i < entityInfoDetail.listLayerAction.Count; i++)
            {
                entityInfoDetail.listLayerAction[i].action_title = BLConvertMLanguage.MultilingualMessageConvert(entityInfoDetail.listLayerAction[i].action_title);
            }
            return entityInfoDetail;
        }
        public JsonResult updateFloorName(int floorSystemId, int structureId, string floorName)
        {
            var response = new BLISP().updateFloorName(floorSystemId, structureId, floorName);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult updateShaftName(int shaftSystemId, int structureId, string shaftName)
        {
            var response = new BLISP().updateShaftName(shaftSystemId, structureId, shaftName);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getParentEntities(int structureId)
        {
            var parentEntities = BLISP.Instance.getParentEntities(structureId);
            return Json(parentEntities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getStructureEntities(int structureId)
        {
            var usrDetail = (User)Session["userDetail"];
            var parentEntities = BLISP.Instance.StructureElements(structureId, usrDetail.role_id);
            return Json(parentEntities, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getNewEntity(int structureId, int systemId, string entityType)
        {
            ISPViewModel objISPRecords = new ISPViewModel();
            objISPRecords.StructureElements = new BLISP().CurrentStructureElements(structureId, systemId, entityType);
            return View("_Element", objISPRecords);
        }
        public ActionResult getCurrentEntity(int structureId, int systemId, string entityType)
        {
            var objISPRecords = new BLISP().CurrentStructureElements(structureId, systemId, entityType);
            if (objISPRecords != null)
            {
                objISPRecords[0] = objISPRecords.FirstOrDefault();
            }
            //return PartialView("_CurrentElement", objISPRecords?[0]);
            return PartialView("_CurrentElement", objISPRecords);
        }
        public ActionResult ShiftEntityPosition(ShiftEntity objShiftEntity)
        {
            new BLISP().getShftEntityDetails(objShiftEntity);
            return PartialView("_ShiftEntity", objShiftEntity);
        }
        public ActionResult SaveEntityPosition(ShiftEntity objShiftEntity)
        {
            PageMessage objPM = new PageMessage();
            DbMessage objResponse = new BLISP().SaveEntityPosition(objShiftEntity);
            objPM.message = BLConvertMLanguage.MultilingualMessageConvert(objResponse.message);//objResponse.message;
            if (objResponse.status)
            {
                objPM.status = ResponseStatus.OK.ToString();
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
            }
            objShiftEntity.objPM = objPM;
            new BLISP().getShftEntityDetails(objShiftEntity);
            return PartialView("_ShiftEntity", objShiftEntity);
        }
        public ActionResult Splicing(int structureId, int systemId, string entityType, int point_x, int point_y)
        {
            User objUser = (User)(Session["userDetail"]);
            var splicingEntity = new BLOSPSplicing().getISPEntityForSplicing(structureId, systemId, entityType, point_x, point_y, objUser.role_id);
            return PartialView("_Splicing", splicingEntity);
        }
        public PartialViewResult getISPEntityImages(int system_Id, string entity_type)
        {
            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

            var lstImages = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, "Image");
            List<DocumentResult> lstImageResult = new List<DocumentResult>();
            foreach (var item in lstImages)
            {
                var _imgSrc = "";
                string imageUrl = string.Concat(FtpUrl, item.FileLocation, item.FileName);

                WebClient request = new WebClient();
                if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                    request.Credentials = new NetworkCredential(UserName, PassWord);

                byte[] objdata = null;
                try
                {
                    objdata = request.DownloadData(imageUrl);
                    if (objdata != null && objdata.Length > 0)
                        _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));
                }
                catch (Exception)
                {
                }

                lstImageResult.Add(new DocumentResult()
                {
                    Id = item.Id,
                    EntitySystemId = item.EntitySystemId,
                    FileName = item.FileName,
                    EntityType = item.EntityType,
                    UploadedBy = item.UploadedBy,
                    created_on = MiscHelper.FormatDateTime(item.Uploaded_on.ToString()),
                    OrgFileName = item.OrgFileName,
                    FileExtension = _imgSrc,
                    FileLocation = item.FileLocation,
                    UploadType = item.UploadType,
                    file_size = BytesToString(Convert.ToInt32(item.file_size)),
                    File_ShortName = Utility.CommonUtility.ConvertStringToShortFormat(item.OrgFileName, 19, 10, 9),
                    categorytype = item.categorytype
                });
            }
            return PartialView("_ImageUpload", lstImageResult);
        }

        public FileResult DownloadFiles(string json, string entity_type)
        {
            var listPathName = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImageDownload>>(json);
            string zipName = string.Empty;
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    zip.AlternateEncodingUsage = Ionic.Zip.ZipOption.AsNecessary;
                    zip.AddDirectoryByName("Files");
                    #region Get the slected files
                    foreach (var item in listPathName)
                    {
                        LibraryAttachment data = new BLAttachment().getEntityDocumentById(item.systemId);
                        string fullPath = FtpUrl + data.file_location + "/" + data.file_name;
                        string FileName = data.file_location + "/" + data.file_name;
                        string localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ReportFolderPath"]) + "/" + data.file_name + "";


                        var request = (FtpWebRequest)WebRequest.Create(fullPath);
                        request.Method = WebRequestMethods.Ftp.DownloadFile;
                        request.Credentials = new NetworkCredential(UserName, PassWord);
                        request.UseBinary = true;
                        try
                        {
                            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                            {
                                using (Stream responseStream = response.GetResponseStream())
                                {
                                    using (FileStream fs = new FileStream(localPath, FileMode.Create))
                                    {
                                        byte[] buffer = new byte[102400];
                                        int read = 0;

                                        while (true)
                                        {
                                            read = responseStream.Read(buffer, 0, buffer.Length);
                                            if (read == 0)
                                                break;

                                            fs.Write(buffer, 0, read);
                                        }
                                        fs.Close();
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }

                        zip.AddFile(localPath, "Files");
                    }
                    #endregion
                    zipName = String.Format("{0}{1}{2}{3}.zip", entity_type.ToUpper(), DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss"));
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        zip.Save(memoryStream);
                        return File(memoryStream.ToArray(), "application/zip", zipName);
                    }
                    System.IO.File.Delete(zipName);
                }
            }
            catch (Exception ex)
            {
                //context.Response.ContentType = "text/plain";
                //context.Response.Write(ex.Message);
            }
            finally
            {
                string FileAddress = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "/Attachments";
                System.IO.DirectoryInfo di = new DirectoryInfo(FileAddress);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            return null;
        }

        public PartialViewResult getAttachmentDetails(int system_Id, string entity_type)
        {
            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            var lstDocument = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, "Document");
            List<DocumentResult> lstDocumentResult = new List<DocumentResult>();
            lstDocumentResult = GetDocumentList(lstDocument);
            return PartialView("_DocumentUpload", lstDocumentResult);

        }

        public PartialViewResult getLegnedDetail()
        {
            return PartialView("_Legend");
        }
        public JsonResult getAllParentInFloor(int structureId, int floorId, string parentType)
        {
            var parentList = new BLISP().getAllParentInFloor(structureId, floorId, parentType);
            return Json(parentList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getShaftRange(int ShaftId)
        {
            var shaftRange = BLISP.Instance.getShaftRangeInfo(ShaftId);
            return Json(shaftRange, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCPEConnections(int structureId)
        {
            var connections = new BLISP().getCPEConnections(structureId);
            return Json(connections, JsonRequestBehavior.AllowGet);
        }

        #region ROOMView
        public JsonResult GetRoomSpaceData(int parentId, string parent_type)
        {
            RoomViewModel roomView = new RoomViewModel();
            List<RoomViewModel> result = new List<RoomViewModel>();
            List<RackInfo> data = BLRack.Instance.GetRacksByParentId(parentId, parent_type, Convert.ToInt32(Session["user_id"]));
            List<EquipmentInfo> Equipmentdata = BLRack.Instance.GetEquipmentinRack(parentId, parent_type, Convert.ToInt32(Session["user_id"]));
            //Get equipment by parent ID

            if (data != null && data.Count > 0)
            {
                //bool isEditable = BLISPModelInfo.Instance.IsEditableModel(rackId);
                //List<Models.Admin.ISPModelInfo> children = BLISPModelInfo.Instance.GetModelChildren(rackId);
                //Get childrens
                foreach (var child in data)
                {
                    result.Add(new RoomViewModel
                    {
                        id = child.system_id,
                        db_id = child.system_id,
                        name = child.rack_name,
                        height = roomView.MeterToPixel(child.length),
                        width = roomView.MeterToPixel(child.width),
                        depth = roomView.MeterToPixel(child.height),
                        is_static = true,
                        offset_x = 0,
                        offset_y = 0,
                        color = "#5897FB",
                        //stroke = "#5897FB",
                        parent = 1,
                        position = new Position
                        {
                            x = roomView.MMToPixel(child.pos_x),
                            y = roomView.MMToPixel(child.pos_y)
                        },
                        //rotation_angle = child.rotation_angle,
                        //model_type_id = child.model_type_id,
                        //model_view_id = child.model_view_id,
                        model_type = "Rack",
                        border_width = 0,
                        is_editable = true,
                        image_data = "<path id=\"rackIconPath1\" d=\"M40 0H0V24H40V0Z\" fill=\"[COLOR]\"/><path d=\"M4 3H6V21H4V3Z\" fill=\"white\"/><path d=\"M12 3H10V21H12V3Z\" fill=\"white\"/><path d=\"M18 3H16V21H18V3Z\" fill=\"white\"/><path d=\"M24 3H22V21H24V3Z\" fill=\"white\"/><path d=\"M30 3H28V21H30V3Z\" fill=\"white\"/><path d=\"M36 3H34V21H36V3Z\" fill=\"white\"/>",
                        no_of_units = child.no_of_units,
                        db_border_width = child.border_width,
                        db_depth = child.height,
                        db_height = child.length,
                        db_width = child.width,
                        network_status = child.network_status,
                        is_view_enabled = child.is_view_enabled,
                        border_color = child.border_color
                    });
                }
            }



            if (Equipmentdata != null && Equipmentdata.Count > 0)
            {
                //bool isEditable = BLISPModelInfo.Instance.IsEditableModel(rackId);
                //List<Models.Admin.ISPModelInfo> children = BLISPModelInfo.Instance.GetModelChildren(rackId);
                //Get childrens
                foreach (var child in Equipmentdata)
                {
                    result.Add(new RoomViewModel
                    {
                        id = child.system_id,
                        db_id = child.system_id,
                        name = child.equipment_name,
                        height = roomView.MMToPixel(child.length),
                        width = roomView.MMToPixel(child.width),
                        depth = roomView.MMToPixel(child.height),
                        is_static = true,
                        offset_x = 0,
                        offset_y = 0,
                        color = "#5897FB",
                        //stroke = "#5897FB",
                        parent = 1,
                        position = new Position
                        {
                            x = roomView.MMToPixel(child.pos_x),
                            y = roomView.MMToPixel(child.pos_y)
                        },
                        //rotation_angle = child.rotation_angle,
                        //model_type_id = child.model_type_id,
                        //model_view_id = child.model_view_id,
                        model_type = "Equipment",
                        border_width = 0,
                        is_editable = true,
                        image_data = "<path fill=\"white\" fill-rule=\"evenodd\" clip -rule=\"evenodd\" d=\"M1 9H24V17H1V9ZM22.7894 10.1429H17.9473V12.4286H22.7894V10.1429ZM22.7894 13.5715H17.9473V15.8572H22.7894V13.5715ZM2.21047 11.2857H4.63152V12.4286H2.21047V11.2857ZM2.21048 13.5715H4.63154V14.7144H2.21048V13.5715ZM8.26319 11.2858H5.84214V12.4286H8.26319V11.2858ZM5.84214 13.5715H8.26319V14.7144H5.84214V13.5715ZM11.8947 11.2858H9.47366V12.4286H11.8947V11.2858ZM9.47366 13.5715H11.8947V14.7144H9.47366V13.5715ZM15.5264 11.2858H13.1053V12.4286H15.5264V11.2858ZM13.1053 13.5715H15.5264V14.7144H13.1053V13.5715Z\" fill =\"#4C6788\" />",
                        //image_data = "<path fill=\"white\"  fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M1 9H24V17H1V9ZM22.7894 10.1429H17.9473V12.4286H22.7894V10.1429ZM22.7894 13.5715H17.9473V15.8572H22.7894V13.5715ZM2.21047 11.2857H4.63152V12.4286H2.21047V11.2857ZM2.21048 13.5715H4.63154V14.7144H2.21048V13.5715ZM8.26319 11.2858H5.84214V12.4286H8.26319V11.2858ZM5.84214 13.5715H8.26319V14.7144H5.84214V13.5715ZM11.8947 11.2858H9.47366V12.4286H11.8947V11.2858ZM9.47366 13.5715H11.8947V14.7144H9.47366V13.5715ZM15.5264 11.2858H13.1053V12.4286H15.5264V11.2858ZM13.1053 13.5715H15.5264V14.7144H13.1053V13.5715Z\" fill=\"#4C6788\"/>",
                        //image_data = "<path id=\"rackIconPath1\" d=\"M40 0H0V24H40V0Z\" fill=\"[COLOR]\"/><path d=\"M4 3H6V21H4V3Z\" fill=\"white\"/><path d=\"M12 3H10V21H12V3Z\" fill=\"white\"/><path d=\"M18 3H16V21H18V3Z\" fill=\"white\"/><path d=\"M24 3H22V21H24V3Z\" fill=\"white\"/><path d=\"M30 3H28V21H30V3Z\" fill=\"white\"/><path d=\"M36 3H34V21H36V3Z\" fill=\"white\"/>",
                        no_of_units = 0,
                        db_border_width = child.border_width,
                        db_depth = child.height,
                        db_height = child.length,
                        db_width = child.width,
                        network_status = child.network_status,
                        is_view_enabled = child.is_view_enabled,
                        border_color = child.border_color
                    });
                }
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetRacksData()
        {

            RoomViewModel roomView = new RoomViewModel();
            List<RoomViewModel> result = new List<RoomViewModel>();
            var layer = (new BLLayer()).GetLayerDetails(EntityType.Rack.ToString());
            List<Models.Admin.VendorSpecificationMaster> data = null;

            if (layer != null && MiscHelper.GetLayerAddPermission(LayerRoleData, NetworkStatus.P.ToString(), EntityType.Rack.ToString()))
                data = (new BLVendorSpecification()).GetVenderSpecificationByLayer(layer.layer_id);
            if (data != null && data.Count > 0)
            {
                result = roomView.ConvertFromSpecification(data, false);
            }
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetEquipments(int modelId)
        {
            RoomViewModel roomView = new RoomViewModel();
            List<RoomViewModel> result = new List<RoomViewModel>();
            List<Models.Admin.ISPModelInfo> data = BLISPModelInfo.Instance.GetModelsWithImage(modelId);
            if (data != null && data.Count > 0)
            {
                List<Models.Admin.ISPModelInfo> children = BLISPModelInfo.Instance.GetModelChildren(modelId);
                //Get childrens
                result = roomView.ConvertFromModelInfo(children, false);
            }
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetRackLibrary()
        {
            List<Models.Admin.ISPModelInfo> data = null;
            if (MiscHelper.GetLayerAddPermission(LayerRoleData, NetworkStatus.P.ToString(), EntityType.Equipment.ToString()))
            {
                data = BLISPModelInfo.Instance.GetModelByType(Models.Admin.ModelType.Equipment.ToString());
                data.AddRange(BLISPModelInfo.Instance.GetModelByType(Models.Admin.ModelType.Tray.ToString()));
                data = data.Where(x => x.status_id == 1).ToList();
            }
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetModelType(int? modelId)
        {
            if (modelId == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            //var hasTypes = BLISPModelInfo.Instance.ModelHasTypes(modelId.Value);
            var result = BLISPModelInfo.Instance.GetModelTypes(modelId.Value);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult AddRack(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "", int specificationId = 0, double pos_x = 0, double pos_y = 0, double pos_z = 0)
        {

            RackInfo input = GetRackDetail(networkIdType, systemId, ModelInfo, pSystemId, pEntityType, pNetworkId, specificationId);
            input.pos_x = pos_x;
            input.pos_z = pos_z;
            input.pos_y = pos_y;
            return PartialView("_AddRack", input);
        }
        public RackInfo GetRackDetail(string networkIdType, int systemId, ElementInfo ModelInfo = null, int pSystemId = 0, string pEntityType = "", string pNetworkid = "", int specificationId = 0)
        {
            RackInfo objRack = new RackInfo();
            objRack.structure_id = ModelInfo.structureid;
            objRack.networkIdType = networkIdType;

            if (systemId == 0)
            {
                var parentDetail = GetEntityParentDetail(pSystemId, pEntityType);
                if (parentDetail != null)
                {
                    objRack.latitude = Convert.ToDouble(parentDetail.latitude);
                    objRack.longitude = Convert.ToDouble(parentDetail.longitude);
                    objRack.province_id = Convert.ToInt32(parentDetail.province_id);
                    objRack.region_id = Convert.ToInt32(parentDetail.region_id);
                }
                if (ModelInfo.structureid > 0)
                {
                    var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                    ////NEW ENTITY->Fill Region and Province Detail..
                    fillRegionProvinceDetail(objRack, structureDetails, GeometryType.Point.ToString());
                }
                ////Fill Parent detail...              
                fillParentDetail(objRack, new ISPNetworkCodeIn() { parent_sysId = pSystemId, parent_eType = pEntityType, parent_networkId = pNetworkid, eType = EntityType.Rack.ToString(), structureId = ModelInfo.structureid }, networkIdType, pSystemId, pEntityType, pNetworkid);


            }
            else
            {
                // Get entity detail by Id...
                objRack = new BLMisc().GetEntityDetailById<RackInfo>(systemId, EntityType.Rack);
            }
            objRack.lstAllVendor = new BLVendorSpecification().GetAllVendorList();
            var objItem = new BLVendorSpecification().GetItemMasterDetailById(specificationId);
            if (objItem != null)
            {
                objRack.length = objItem.length ?? 0;
                objRack.width = objItem.width ?? 0;
                objRack.height = objItem.height ?? 0;
                objRack.no_of_units = objItem.no_of_units ?? 0;
                objRack.subcategory1 = objItem.subcategory_1;
                objRack.subcategory2 = objItem.subcategory_2;
                objRack.subcategory3 = objItem.subcategory_3;
                objRack.vendor_id = objItem.vendor_id;
                objRack.item_code = objItem.code;
                objRack.specification = objItem.specification;
                objRack.border_width = objItem.border_width ?? 0;
                objRack.border_color = objItem.border_color;
                objRack.audit_item_master_id = objItem.audit_id;
            }
            fillProjectSpecifications(objRack);
            var objDDL = new BLMisc().GetDropDownList("");
            objRack.lstBOMSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
            objRack.lstRackType = new BLMisc().GetDropDownList(EntityType.Rack.ToString(), DropDownType.Rack_Type.ToString());
            return objRack;
        }

        public ActionResult SaveRack(RackInfo input, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            int structure_id = input.structure_id;

            if (input.networkIdType == NetworkIdType.A.ToString() && input.system_id == 0)
            {
                //var structureDetails = new BLISP().GetStructureById(input.objIspEntityMap.structure_id);
                ////GET AUTO NETWORK CODE...               
                var objNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.Rack.ToString(), structureId = structure_id });
                if (isDirectSave == true)
                {
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    input = GetRackDetail(input.networkIdType, input.system_id, new ElementInfo { structureid = structure_id }, input.pSystemId, input.pEntityType, input.pNetworkId);
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    input.rack_name = objNetworkCodeDetail.network_code;
                    var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
                   // var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
                    input.bom_sub_category = objBOMDDL[0].dropdown_value;
                   // input.served_by_ring = objSubCatDDL[0].dropdown_value;

                }
                ////SET NETWORK CODE
                input.network_id = objNetworkCodeDetail.network_code;


            }
            if (TryValidateModel(input))
            {
                var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.Rack.ToString().ToUpper()).FirstOrDefault().layer_title;
                var isNew = input.system_id > 0 ? false : true;
                var resultItem = BLRack.Instance.SaveEntityRack(input, Convert.ToInt32(Session["user_id"]));


                if (isNew)
                {
                    input.system_id = resultItem.system_id;
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = string.Format("{0} saved successfully!", layer_title);
                }
                else
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = string.Format("{0} updated successfully!", layer_title);
                }
            }
            input.objPM = objPM;
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(input.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //BLItemTemplate.Instance.BindItemDropdowns(input, EntityType.POD.ToString());
                // RETURN PARTIAL VIEW WITH MODEL DATA              
                //fillProjectSpecifications(input);
                input.lstAllVendor = new BLVendorSpecification().GetAllVendorList();
                BindRackDDropDown(input);
                fillProjectSpecifications(input);
                return PartialView("_AddRack", input);
            }
        }
        private void BindRackDDropDown(RackInfo objrack)
        {
            var objDDL = new BLMisc().GetDropDownList("");
            objrack.lstBOMSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
           // objrack.lstServedByRing = objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
        }
        public ActionResult DeleteRack(int id)
        {
            PageMessage result = BLRack.Instance.DeleteRackById(id);
            return Json(result, JsonRequestBehavior.AllowGet);
            //int result = 0;
            //if (!BLRack.Instance.HasRackChild(id))
            //    result = BLRack.Instance.DeleteRackById(id);
            //return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveRackPosition(int system_id, double pos_x, double pos_y, double pos_z)
        {
            PageMessage objPM = new PageMessage();
            RackInfo equipment = new RackInfo() { system_id = system_id, pos_x = pos_x, pos_y = pos_y, pos_z = pos_z };
            bool result = BLRack.Instance.SaveRackPosition(equipment);
            if (result)
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = Resources.Resources.SI_ISP_GBL_GBL_GBL_056;// "Rack position saved successfully!";
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = Resources.Resources.SI_ISP_GBL_GBL_GBL_057;// "Rack position not saved!";
            }
            return Json(objPM, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRoomParentDetails(int id, string type)
        {
            //RoomInfo room = new RoomInfo();
            //if (type.ToLower() == EntityType.UNIT.ToString().ToLower())
            //{
            //    room = new RoomInfo();
            //    room = BLISP.Instance.getRoomDetails(id);
            //    return Json(room, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    FloorInfo floor = new FloorInfo();
            //    floor = BLISP.Instance.getFloorInfo(id);
            //    return Json(floor, JsonRequestBehavior.AllowGet);
            //}
            var room = GetISPPop(id, type);
            return Json(room, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult AddEquipment(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "", int specificationId = 0, double pos_x = 0, double pos_y = 0, double pos_z = 0, int model_info_id = 0, int rack_id = 0, string fms_network_id = null)
        {

            EquipmentInfo input = GetEquipmentDetail(networkIdType, systemId, ModelInfo, pSystemId, pEntityType, pNetworkId, specificationId, model_info_id, rack_id, fms_network_id);
            input.pos_x = pos_x;
            input.pos_z = pos_z;
            input.pos_y = pos_y;

            BLLayer objBLLayer = new BLLayer();
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;
            input.lstUserModule = objBLLayer.GetUserModuleAbbrList(usrId, UserType.Web.ToString());

            return PartialView("_AddEquipment", input);
        }
        public EquipmentInfo GetEquipmentDetail(string networkIdType, int systemId, ElementInfo ModelInfo = null, int pSystemId = 0, string pEntityType = "", string pNetworkid = "", int specificationId = 0, int model_info_id = 0, int rack_id = 0, string fms_network_id = null)
        {
            EquipmentInfo objEquipment = new EquipmentInfo();

            objEquipment.structure_id = ModelInfo.structureid;
            objEquipment.networkIdType = networkIdType;
            objEquipment.fms_network_id = fms_network_id;
            Models.Admin.ISPModelInfo modelInfo = BLISPModelInfo.Instance.GetById(model_info_id);
            string modelType = "";

            if (systemId == 0)
            {
                var parentDetail = GetEntityParentDetail(pSystemId, pEntityType);
                if (parentDetail != null)
                {
                    objEquipment.latitude = Convert.ToDouble(parentDetail.latitude);
                    objEquipment.longitude = Convert.ToDouble(parentDetail.longitude);
                    objEquipment.province_id = Convert.ToInt32(parentDetail.province_id);
                    objEquipment.region_id = Convert.ToInt32(parentDetail.region_id);
                }
                if (modelInfo != null)
                {

                    if (modelInfo.model_type_id != null)
                    {
                        var type = BLISPModelInfo.Instance.GetModelType(modelInfo.model_type_id.Value);
                        modelType = type.key;
                    }
                    else
                    {
                        var model = BLISPModelInfo.Instance.GetModelMaster(modelInfo.model_id);
                        objEquipment.networkIdType = model.network_id_type;
                        modelType = model.key;
                    }
                    if (ModelInfo.structureid > 0)
                    {
                        var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                        ////NEW ENTITY->Fill Region and Province Detail..
                        fillRegionProvinceDetail(objEquipment, structureDetails, GeometryType.Point.ToString());
                    }
                    ////Fill Parent detail...              
                    fillModelParentDetail(objEquipment, new ISPNetworkCodeIn() { parent_sysId = pSystemId, parent_eType = pEntityType, parent_networkId = pNetworkid, eType = modelType, structureId = ModelInfo.structureid }, objEquipment.networkIdType, pSystemId, pEntityType, pNetworkid);

                    objEquipment.length = modelInfo.height;
                    objEquipment.width = modelInfo.width;
                    objEquipment.height = modelInfo.depth;
                    objEquipment.model_id = modelInfo.model_id;
                    objEquipment.model_type_id = modelInfo.model_type_id;
                    objEquipment.model_info_id = model_info_id;
                    objEquipment.item_template_id = modelInfo.item_template_id;
                    objEquipment.model_view_id = 1;
                    objEquipment.unit_size = modelInfo.unit_size;
                    objEquipment.rack_id = rack_id;
                }
            }
            else
            {
                // Get entity detail by Id...
                objEquipment = new BLMisc().GetEntityDetailById<EquipmentInfo>(systemId, EntityType.Equipment);
                objEquipment.model_name = objEquipment.equipment_name;
                objEquipment.lstEquipmentInfo = BLRack.Instance.GetEquipmentChildren(systemId);
                //if (modelInfo != null)
                //{
                //    objEquipment.item_template_id = modelInfo.item_template_id;
                //}
            }


            objEquipment.lstAllVendor = new BLVendorSpecification().GetAllVendorList();
            var objItem = new BLVendorSpecification().GetItemMasterDetailById(objEquipment.item_template_id);
            BLItemTemplate.Instance.BindItemDropdowns(objEquipment, EntityType.Equipment.ToString());
            if (objEquipment.model_type_id != null)
            {
                var type = BLISPModelInfo.Instance.GetModelType(objEquipment.model_type_id.Value);
                modelType = type.key;
            }
            if (!String.IsNullOrEmpty(modelType))
            {
                var layerDetail = BLISP.Instance.getLayerDetails(modelType);
                if (layerDetail != null && layerDetail.is_middleware_entity)
                {

                    BLItemTemplate.Instance.BindItemDropdowns(objEquipment, modelType);
                    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objEquipment.parent_system_id, parent_eType = objEquipment.parent_entity_type, eType = modelType, structureId = objEquipment.structure_id ?? 0 });
                    if (systemId == 0)
                    {
                        if (!string.IsNullOrEmpty(objEquipment.fms_network_id))
                        {
                            objEquipment.network_id = objEquipment.fms_network_id;
                        }
                        else
                        {
                            objEquipment.network_id = objISPNetworkCode.network_code;
                        }
                    }


                    //TODO:MAKE GENERIC TYPE
                    //if (modelType.ToUpper() == EntityType.FMS.ToString().ToUpper())
                    //{
                    //    //objEquipment = BLItemTemplate.Instance.GetTemplateDetail<ISPModelTemplate>(Convert.ToInt32(Session["user_id"]), EntityType.FMS);

                    //    BLItemTemplate.Instance.BindItemDropdowns(objEquipment, EntityType.FMS.ToString());
                    //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objEquipment.parent_system_id, parent_eType = objEquipment.parent_entity_type, eType = EntityType.FMS.ToString(), structureId = objEquipment.structure_id ?? 0 });
                    //    if (systemId == 0)
                    //    {
                    //        if (!string.IsNullOrEmpty(objEquipment.fms_network_id))
                    //        {
                    //            objEquipment.network_id = objEquipment.fms_network_id;
                    //        }
                    //        else
                    //        {
                    //            objEquipment.network_id = objISPNetworkCode.network_code;
                    //        }
                    //    }

                    //}
                    //if (modelType.ToUpper() == EntityType.HTB.ToString().ToUpper())
                    //{
                    //    //objEquipment = BLItemTemplate.Instance.GetTemplateDetail<ISPModelTemplate>(Convert.ToInt32(Session["user_id"]), EntityType.FMS);

                    //    BLItemTemplate.Instance.BindItemDropdowns(objEquipment, EntityType.HTB.ToString());
                    //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objEquipment.parent_system_id, parent_eType = objEquipment.parent_entity_type, eType = EntityType.HTB.ToString(), structureId = objEquipment.structure_id ?? 0 });
                    //    if (systemId == 0)
                    //    {
                    //        if (!string.IsNullOrEmpty(objEquipment.fms_network_id))
                    //        {
                    //            objEquipment.network_id = objEquipment.fms_network_id;
                    //        }
                    //        else
                    //        {
                    //            objEquipment.network_id = objISPNetworkCode.network_code;
                    //        }
                    //    }

                    //}
                }
            }
            if (objEquipment.item_template_id != 0 && objItem != null)
            {
                //BLItemTemplate.Instance.BindItemDropdowns(objEquipment, EntityType.Equipment.ToString());
                ////objRack.no_of_units = objItem.no_of_units ?? 0;
                objEquipment.subcategory1 = objItem.subcategory_1;
                objEquipment.subcategory2 = objItem.subcategory_2;
                objEquipment.subcategory3 = objItem.subcategory_3;
                objEquipment.vendor_id = objItem.vendor_id;
                objEquipment.item_code = objItem.code;
                objEquipment.specification = objItem.specification;
                objEquipment.no_of_port = objItem.no_of_port;
                objEquipment.audit_item_master_id = objItem.audit_id;
            }
            objEquipment.lstVendor = BLItemTemplate.Instance.GetVendorList(objEquipment.specification);
            objEquipment.unit_input_type = "port";
            if (objEquipment.objPM == null)
                objEquipment.objPM = new PageMessage();
            objEquipment.objPM.isNewEntity = (systemId == 0);
            fillProjectSpecifications(objEquipment);
            var objDDL = new BLMisc().GetDropDownList("");
            objEquipment.lstBOMSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
            return objEquipment;
        }

        public ActionResult SaveEquipment(EquipmentInfo input, bool isDirectSave = false)
        {
            //Validate Port count

            if (string.IsNullOrEmpty(input.specification))
            {
                input.specification = "none";
            }
            if (string.IsNullOrEmpty(input.item_code))
            {
                input.item_code = "none";
            }
            input.height = input.length;
            ModelState.Clear();
            input.objPM = new PageMessage();
            int structure_id = input.structure_id ?? 0;
            string modelType = "";
            if (input.networkIdType == NetworkIdType.A.ToString() && input.system_id == 0)
            {

                if (input.model_type_id != null)
                {
                    var type = BLISPModelInfo.Instance.GetModelType(input.model_type_id.Value);
                    modelType = type.key;
                }
                else
                {
                    var model = BLISPModelInfo.Instance.GetModelMaster(input.model_id);
                    input.networkIdType = model.network_id_type;
                    modelType = model.key;
                }
                //var structureDetails = new BLISP().GetStructureById(input.objIspEntityMap.structure_id);
                ////GET AUTO NETWORK CODE...               
                fillModelParentDetail(input, new ISPNetworkCodeIn() { parent_sysId = input.parent_system_id, parent_eType = input.parent_entity_type, parent_networkId = input.parent_network_id, eType = modelType, structureId = input.structure_id ?? 0 }, input.networkIdType, input.parent_system_id, input.parent_entity_type, input.parent_network_id);
                if (isDirectSave == true)
                {
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    input = GetEquipmentDetail(input.networkIdType, input.system_id, new ElementInfo { structureid = structure_id }, input.pSystemId, input.pEntityType, input.pNetworkId);
                    var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
                  //  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
                    input.bom_sub_category = objBOMDDL[0].dropdown_value;
                   // input.served_by_ring = objSubCatDDL[0].dropdown_value;
                }
            }

            if (TryValidateModel(input))
            {
                //var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.Rack.ToString().ToUpper()).FirstOrDefault().layer_title;

                var isNew = input.system_id > 0 ? false : true;
                input.objPM.isNewEntity = isNew;
                var resultItem = BLRack.Instance.SaveEntityEquipment(input, Convert.ToInt32(Session["user_id"]));
                input.equipment_name = input.model_name;
                if (isNew)
                {
                    ////save equipment children
                    //BLRack.Instance.SaveEquipmentMapping(resultItem.system_id, resultItem.model_info_id, resultItem.model_view_id, Convert.ToInt32(Session["user_id"]));
                    //input.system_id = resultItem.system_id;


                    if (input.model_type_id != null)
                    {
                        var type = BLISPModelInfo.Instance.GetModelType(input.model_type_id.Value);
                        modelType = type.key;
                    }
                    if (!String.IsNullOrEmpty(modelType))
                    {
                        var layerDetail = BLISP.Instance.getLayerDetails(modelType);
                        if (layerDetail != null && layerDetail.is_middleware_entity)
                        {

                            //TODO:MAKE GENERIC TYPE
                            //if (modelType.ToUpper() == EntityType.FMS.ToString().ToUpper())
                            //{
                            //    //Save FMS If New 
                            //    //Get FMS detail
                            //    FMSMaster objFMS = new FMSMaster();
                            //    //var objRack = new BLMisc().GetEntityDetailById<RackInfo>(input.parent_system_id, EntityType.Rack);
                            //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = input.parent_system_id, parent_eType = input.parent_entity_type, eType = EntityType.FMS.ToString(), structureId = structure_id });
                            //    objFMS = GetFMSDetail("A", 0, 0, new ElementInfo { structureid = structure_id }, input.parent_system_id, input.parent_entity_type, input.parent_network_id);
                            //    objFMS.fms_name = input.model_name;
                            //    objFMS.item_code = input.item_code;
                            //    objFMS.specification = input.specification;
                            //    objFMS.category = input.category;
                            //    objFMS.no_of_port = input.no_of_port;


                            //    //Save FMS
                            //    //FMSMaster objFMS = GetFMSDetail(EntityType.FMS.ToString(),0,0,new ElementInfo(),0,"","" );
                            //    if (string.IsNullOrEmpty(input.fms_network_id))
                            //    {
                            //        objFMS.network_id = objISPNetworkCode.network_code;
                            //        objFMS.sequence_id = objISPNetworkCode.sequence_id;
                            //        var FMSItem = new BLFMS().SaveEntityFMS(objFMS, Convert.ToInt32(Session["user_id"]));
                            //        input.network_id = objFMS.network_id;
                            //    }
                            //}
                            //else if (modelType.ToUpper() == EntityType.HTB.ToString().ToUpper())
                            //{
                            //    //Save HTB If New 
                            //    //Get HTB detail
                            //    HTBInfo objHTB = new HTBInfo();
                            //    //var objRack = new BLMisc().GetEntityDetailById<RackInfo>(input.parent_system_id, EntityType.Rack);
                            //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = input.parent_system_id, parent_eType = input.parent_entity_type, eType = EntityType.HTB.ToString(), structureId = structure_id });
                            //    //objHTB = GetHTBDetail("A", 0, 0, new ElementInfo { structureid = structure_id }, input.parent_system_id, input.parent_entity_type, input.parent_network_id);

                            //    objHTB = getHTBInfo("A", 0, 0, structure_id, input.parent_system_id, input.parent_entity_type, input.parent_network_id);

                            //    objHTB.htb_name = input.model_name;
                            //    objHTB.item_code = input.item_code;
                            //    objHTB.specification = input.specification;
                            //    objHTB.category = input.category;
                            //    objHTB.no_of_port = input.no_of_port;


                            //    //Save HTB
                            //    //HTBMaster objHTB = GetHTBDetail(EntityType.HTB.ToString(),0,0,new ElementInfo(),0,"","" );
                            //    if (string.IsNullOrEmpty(input.fms_network_id))
                            //    {
                            //        objHTB.network_id = objISPNetworkCode.network_code;
                            //        objHTB.sequence_id = objISPNetworkCode.sequence_id;
                            //        var HTBItem = new BLISP().SaveHTBDetails(objHTB, Convert.ToInt32(Session["user_id"]));
                            //        input.network_id = objHTB.network_id;
                            //    }
                            //}
                        }
                    }

                    //save equipment children
                    BLRack.Instance.SaveEquipmentMapping(resultItem.system_id, resultItem.model_info_id, resultItem.model_view_id, Convert.ToInt32(Session["user_id"]));
                    input.system_id = resultItem.system_id;
                }
                else
                {
                    if (input.lstEquipmentInfo != null && input.lstEquipmentInfo.Count > 0)
                    {
                        var renameData = input.lstEquipmentInfo.Select(x => new { x.system_id, x.model_name, x.short_network_id }).ToList();
                        BLRack.Instance.RenameEquimentChildren(JsonConvert.SerializeObject(renameData));
                    }
                }

            }

            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(input.objPM, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //BLItemTemplate.Instance.BindItemDropdowns(input, EntityType.POD.ToString());
                // RETURN PARTIAL VIEW WITH MODEL DATA              
                //fillProjectSpecifications(input);
                input.lstAllVendor = new BLVendorSpecification().GetAllVendorList();
                fillProjectSpecifications(input);
                var objDDL = new BLMisc().GetDropDownList("");
                input.lstBOMSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
               // input.lstServedByRing = objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
                return PartialView("_AddEquipment", input);
            }
        }
        public ActionResult DeleteEquipment(int id)
        {

            PageMessage result = new BLRack().DeleteEquipmentById(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteChildEquipment(int id)
        {
            DbMessage result = new BLRack().DeleteChildEquipment(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipmentChildren(int id)
        {
            RoomViewModel roomView = new RoomViewModel();
            List<RoomViewModel> result = new List<RoomViewModel>();
            List<EquipmentInfo> data = BLRack.Instance.GetEquipmentChildren(id);
            if (data != null && data.Count > 0)
            {
                //List<Models.Admin.ISPModelInfo> children = BLISPModelInfo.Instance.GetModelChildren(modelId);
                //Get childrens
                result = roomView.ConvertFromEquipmentInfo(data, false);
            }
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetRackChildren(int rackId, int parent_id, string parent_type)
        {
            RoomViewModel roomView = new RoomViewModel();
            List<RoomViewModel> result = new List<RoomViewModel>();
            List<EquipmentInfo> data = BLRack.Instance.GetEquipmentByRackId(rackId, parent_id, parent_type, Convert.ToInt32(Session["user_id"]));

            if (data != null && data.Count > 0)
            {
                foreach (var model in data)
                {  //Get childrens
                    var found = BLRack.Instance.GetEquipmentChildren(model.system_id);
                    found.ForEach(x => x.rack_id = rackId);
                    if (found.Count > 0)
                    {
                        found[0].parent_system_id = 1;
                        found[0].height = found[0].length;
                        found[0].isEditable = true;
                        found[0].is_view_enabled = model.is_view_enabled;
                        found[0].is_internal_connectivity_enabled = model.is_internal_connectivity_enabled;
                        result.AddRange(roomView.ConvertFromEquipmentInfo(found, false));
                    }
                }
            }

            if (rackId == 999999)
            {
                double lastHeight = 0;
                foreach (var item in result.Where(m => m.model_id == 1).ToList())
                {
                    item.position.x = item.width / 2;
                    item.position.y = lastHeight + item.height / 2;
                    lastHeight = lastHeight + item.db_height + 10;
                }
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult MultipleConnection(viewMultipleConnections objMultiCon)
        {
            var multiConnection = new BLOSPSplicing().getMultipleConnections(objMultiCon.source_system_id, objMultiCon.source_port_no, objMultiCon.portType);
            objMultiCon.connectionsList = multiConnection;
            objMultiCon.isPreviousConnection = true;
            return PartialView("../Splicing/_MultipleConnection", objMultiCon);
        }

        public JsonResult GetEquipmentChildrenList(int equipmentID)
        {
            string network_id;
            List<nodelist> data = BLRack.Instance.GetEquipmentChildrenList(equipmentID);
            var connections = BLRack.Instance.GetModelConnections(equipmentID, EntityType.Equipment.ToString(), 0, "");
            foreach (var item in data.Skip(1))
            {

                var connectedPort = connections.Where(x => (x.source_port_id == item.child_id && x.destination_port_id != null) || (x.destination_port_id == item.child_id)).Select(x => x.source_port_network_id).Distinct().ToList();
                string[] arr = item.network_id.Split('-');
                network_id = arr[arr.Length - 1].ToString();
                item.network_id = network_id;
                item.connection_count = connectedPort.Count;


            }


            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public int GetEquipmentChildrenListCount(int parent_system_id, int port_sequence_id)
        {
            List<nodelist> data = BLRack.Instance.GetEquipmentChildrenListCount(parent_system_id, port_sequence_id);
            int _portConnectionCount = data[0].connection_count;
            return _portConnectionCount;
        }

        public JsonResult GetEquipmentChildrenListSinglecount(int parent_system_id, int port_sequence_id)
        {
            List<nodelist> data = BLRack.Instance.GetEquipmentChildrenListCount(parent_system_id, port_sequence_id);
            int _portConnectionCount = data[0].connection_count;
            var jsonResult = Json(_portConnectionCount, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetPortConnectionCount(int parentId, int portId)
        {
            var connections = BLRack.Instance.GetModelConnections(parentId, EntityType.Equipment.ToString(), 0, "");
            var connectedPort = connections.Where(x => (x.source_port_id == portId && x.destination_port_id != null) || (x.destination_port_id == portId)).Select(x => x.source_port_network_id).Distinct().ToList();

            int _portConnectionCount = connectedPort.Count;
            var jsonResult = Json(_portConnectionCount, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetChildModelDetails(int id)
        {
            EquipmentInfo obj = new EquipmentInfo();
            obj.lstEquipmentInfo = BLRack.Instance.GetChildModelDetails(id);
            obj.lstPortConnection = BLRack.Instance.GetPortConnectionDetails(obj.lstEquipmentInfo[0].parent_network_id, obj.lstEquipmentInfo[0].sequence_id);
            var jsonResult = Json(obj, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetUnmappedFMS(int podId, string layer_name, string parent_entity_type)
        {
            var result = BLRack.Instance.GetUnmappedFMS(podId, layer_name, parent_entity_type);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetMiddlewareModelType()
        {
            var result = BLRack.Instance.GetModelTypes();
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult ResetConnection()
        {
            DbMessage data = BLRack.Instance.resetConnection();
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult SaveEquipmentPosition(int system_id, double pos_x, double pos_y, double pos_z)
        {
            PageMessage objPM = new PageMessage();
            EquipmentInfo equipment = new EquipmentInfo() { system_id = system_id, pos_x = pos_x, pos_y = pos_y, pos_z = pos_z };
            bool result = BLRack.Instance.SaveEquipmentPosition(equipment, Convert.ToInt32(Session["user_id"]));
            if (result)
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_098;// "Equipment position saved successfully!";
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = Resources.Resources.SI_ISP_GBL_GBL_GBL_058;// "Equipment position not saved!";
            }
            return Json(objPM, JsonRequestBehavior.AllowGet);
        }

        public ActionResult POPRoomView(string key)
        {
            var usrDetail = (User)Session["userDetail"];
            if (usrDetail != null && usrDetail.role_id == 1)
            {
                return RedirectToAction("index", "UnAuthorized");

            }
            var value = MiscHelper.Decrypt(key);
            var data = value.Split('-');
            int popID = Convert.ToInt32(data[0]);
            string type = EntityType.POD.ToString();
            if (data.Count() > 1)
            {
                type = data[1];
            }
            ISPViewModel objISPRecords = new ISPViewModel();
            objISPRecords.POPId = popID;
            objISPRecords.POPType = type;
            TempData["RackRolesData"] = LayerRoleData.FirstOrDefault(x => x.layerName.ToUpper() == EntityType.Rack.ToString().ToUpper());
            TempData["EquipmentRolesData"] = LayerRoleData.FirstOrDefault(x => x.layerName.ToUpper() == EntityType.Equipment.ToString().ToUpper());
            return View("Index", objISPRecords);
        }
        [NonAction]
        public dynamic GetPOPRoomDetails(int parentId, string parentType)
        {
            double room_length = ApplicationSettings.DefaultPODLength, room_width = ApplicationSettings.DefaultPODWidth, door_width = 0;
            string door_position = null, door_type = "none";
            int structure_id = 0;
            if (parentType.ToLower() == EntityType.UNIT.ToString().ToLower())
            {
                RoomInfo room = new RoomInfo();
                room = BLISP.Instance.getRoomDetails(parentId);
                room_length = room.room_length;
                room_width = room.room_width;
                door_position = room.door_position;
                door_type = room.door_type;
                door_width = room.door_width ?? 0;
                structure_id = room.structure_id;
            }
            if (parentType.ToLower() == EntityType.Structure.ToString().ToLower())
            {
                FloorInfo floor = new FloorInfo();
                floor = BLISP.Instance.getFloorInfo(parentId);
                room_length = floor.length ?? 0;
                room_width = floor.width ?? 0;
                structure_id = floor.structure_id;
            }
            if (parentType.ToLower() == EntityType.Cabinet.ToString().ToLower())
            {
                CabinetMaster cabinet = new CabinetMaster();
                cabinet = new BLMisc().GetEntityDetailById<CabinetMaster>(parentId, EntityType.Cabinet); ;
                room_length = cabinet.length ?? 0;
                room_width = cabinet.width ?? 0;
            }
            return new
            {
                room_length = room_length,
                room_width = room_width,
                door_position = door_position,
                door_width = door_width,
                door_type = door_type,
                structure_id = structure_id
            };
        }
        [NonAction]
        public dynamic GetISPPop(int entityId, string entityType)
        {
            int parentId = 0;
            string parentType = "", networkId = "", name = "";
            if (entityType.ToLower() == EntityType.POD.ToString().ToLower())
            {
                var pod = new BLMisc().GetEntityDetailById<PODMaster>(entityId, EntityType.POD);
                networkId = pod.network_id;
                name = pod.pod_name;
                if (pod.parent_entity_type.ToLower() == EntityType.UNIT.ToString().ToLower())
                {
                    parentId = pod.parent_system_id;
                    parentType = EntityType.UNIT.ToString();
                }
                if (pod.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
                {
                    //parentId = pod.parent_system_id;
                    parentType = EntityType.Structure.ToString();
                    var map = BLIspEntityMapping.Instance.GetStructureFloorbyEntityId(entityId, entityType);
                    parentId = map.floor_id ?? 0;
                }
            }

            if (entityType.ToLower() == EntityType.MPOD.ToString().ToLower())
            {
                var mpod = new BLMisc().GetEntityDetailById<MPODMaster>(entityId, EntityType.MPOD);
                networkId = mpod.network_id;
                name = mpod.mpod_name;
                if (mpod.parent_entity_type.ToLower() == EntityType.UNIT.ToString().ToLower())
                {
                    parentId = mpod.parent_system_id;
                    parentType = EntityType.UNIT.ToString();
                }
                if (mpod.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
                {
                    //parentId = pod.parent_system_id;
                    parentType = EntityType.Structure.ToString();
                    var map = BLIspEntityMapping.Instance.GetStructureFloorbyEntityId(entityId, entityType);
                    parentId = map.floor_id ?? 0;
                }
            }
            //ROOMMANAGEMENT
            if (entityType.ToLower() == EntityType.UNIT.ToString().ToLower())
            {
                var unit = new BLMisc().GetEntityDetailById<UnitMaster>(entityId, EntityType.UNIT);
                networkId = unit.network_id;
                name = unit.room_name;

                parentId = entityId;
                parentType = EntityType.UNIT.ToString();
            }
            //ROOMMANAGEMENT
            if (entityType.ToLower() == EntityType.Floor.ToString().ToLower())
            {
                var unit = new BLMisc().GetEntityDetailById<FloorMaster>(entityId, EntityType.Floor);
                networkId = unit.network_id;
                name = unit.floor_name;

                parentId = entityId;
                parentType = EntityType.Structure.ToString();
            }
            //cabinet shazia 
            if (entityType.ToLower() == EntityType.Cabinet.ToString().ToLower())
            {
                var cabinet = new BLMisc().GetEntityDetailById<CabinetMaster>(entityId, EntityType.Cabinet);
                networkId = cabinet.network_id;
                name = cabinet.cabinet_name;
                parentId = entityId;
                parentType = EntityType.Cabinet.ToString();
                //if (cabinet.parent_entity_type.ToLower() == EntityType.UNIT.ToString().ToLower())
                //{
                //    parentId = cabinet.parent_system_id;
                //    parentType = EntityType.UNIT.ToString();
                //}
                //if (cabinet.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
                //{
                //    //parentId = pod.parent_system_id;
                //    parentType = EntityType.Structure.ToString();
                //    var map = BLIspEntityMapping.Instance.GetStructureFloorbyEntityId(entityId, entityType);
                //    parentId = map.floor_id ?? 0;
                //}
            }
            //cabinet shazia end
            var details = GetPOPRoomDetails(parentId, parentType);
            return new
            {
                network_id = networkId,
                name = name,
                room_length = details.room_length,
                room_width = details.room_width,
                door_position = details.door_position,
                door_width = details.door_width,
                door_type = details.door_type,
                structure_id = details.structure_id
            };
        }

        [NonAction]
        public dynamic GetEntityParentDetail(int ParentId, string ParentType)
        {
            dynamic result = null;
            if (ParentType.ToLower() == EntityType.POD.ToString().ToLower())
            {
                result = new BLMisc().GetEntityDetailById<PODMaster>(ParentId, EntityType.POD);
            }
            if (ParentType.ToLower() == EntityType.MPOD.ToString().ToLower())
            {
                result = new BLMisc().GetEntityDetailById<MPODMaster>(ParentId, EntityType.MPOD);
            }
            if (ParentType.ToLower() == EntityType.Structure.ToString().ToLower())
            {
                result = new BLMisc().GetEntityDetailById<StructureMaster>(ParentId, EntityType.Structure);
            }
            if (ParentType.ToLower() == EntityType.UNIT.ToString().ToLower())
            {
                result = new BLMisc().GetEntityDetailById<RoomInfo>(ParentId, EntityType.UNIT);
            }
            //cabinet shazia 
            if (ParentType.ToLower() == EntityType.Cabinet.ToString().ToLower())
            {
                result = new BLMisc().GetEntityDetailById<CabinetMaster>(ParentId, EntityType.Cabinet);
            }
            //cabinet shazia end
            return result;
        }
        public ActionResult GetConnectedPorts(int parentId, int portId)
        {
            List<string> connectedPort = new List<string>();
            //Get Connection list
            var connections = BLRack.Instance.GetModelConnections(parentId, EntityType.Equipment.ToString(), 0, "");
            connectedPort = connections.Where(x => (x.source_port_id == portId && x.destination_port_id != null)).Select(x => x.destination_port_network_id).Distinct().ToList();
            var other = connections.Where(x => (x.destination_port_id == portId)).Select(x => x.source_port_network_id).Distinct().ToList();
            connectedPort.AddRange(other);

            //List<string> connectedPort = new List<string>();

            return PartialView("_ConnectedPortList", connectedPort);
        }

        public JsonResult GetModelPortConnections(int sourceId, string sourceType, int destinationId, string destinationType)
        {
            var connections = BLRack.Instance.GetModelConnections(sourceId, sourceType, destinationId, destinationType);
            var jsonResult = Json(connections, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult getConnectionList(int systemId, int portNo)
        {
            var data = BLRack.Instance.getConnectedPorts(systemId, portNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

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

        #region sitecustomer
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
            if (isNew) { objPM.message = Resources.Resources.SI_OSP_CUS_JQ_FRM_003; }// "Customer updated successfully!"; 
            else
            {
                objPM.message = Resources.Resources.SI_OSP_CUS_NET_FRM_007;
            }//"Customer added successfully!";
            objSiteCustomer.objPM = objPM;
            objSiteCustomer.lstElectricalmeter = new BLMisc().GetDropDownList("", DropDownType.Electrical_Meter_Type.ToString());
            objSiteCustomer.lstCableEntryPoints = new BLMisc().GetDropDownList("", DropDownType.Cable_Entry_Point.ToString());

            return PartialView("~/Views/Library/_AddSiteCustomer.cshtml", objSiteCustomer);
        }
        #endregion
        public JsonResult getRackList(int systemId, string entityType)
        {
            var result = BLRack.Instance.getRackList(systemId, entityType);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getEquipmentByRack(int rackId, int parent_id, string parent_type)
        {
            var data = BLRack.Instance.GetEquipmentByParent(rackId, parent_id, parent_type);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public void ExportRoomViewConnections(bool is_source_connected, string source_type, int source_id, string image_path, string exportType, string parent_type, int parent_id)
        {
            DataSet dsResult = new DataSet();

            var exportResult = BLISP.Instance.GetEquipmentExportDetail(source_type, source_id.ToString(), parent_type, parent_id.ToString());
            DataTable dtlogs = MiscHelper.ListToDataTable(exportResult);
            dtlogs.TableName = "Equipment Report";
            dsResult.Tables.Add(dtlogs);
            Dictionary<string, string> dicImages = new Dictionary<string, string>();
            // dicImages.Add("Splicing Image", @"D:\Pawan_Kumar\SVN\Spactra\branches\NT_Wireless_1\SmartInventory\SmartInventory\Uploads\temp_LOS\LOS_54cfc91d-707d-45cd-ae5b-ee75e0ef1c9e.png");
            dicImages.Add("EquipmentImage", image_path);
            if (is_source_connected)
            {
                List<ExportPatchingInfo> lstExportPatching = new BLOSPSplicing().getPatchingConnection(source_id, source_type, parent_type, parent_id);
                DataTable dtReport = MiscHelper.ListToDataTable<ExportPatchingInfo>(lstExportPatching);
                dtReport.TableName = "ConnectionDetails";
                dsResult.Tables.Add(dtReport);
            }
            if (exportType == "EXCEL")
            {
                ExportData(dsResult, "Equipment Details", dicImages);
            }
            else
            {
                PDFHelper.GenerateToPDF(dsResult, "Equipment_Details", "Equipment Details", image_path);
            }
        }
        public PartialViewResult GetLoopMangmentDetail(NELoopDetails obj)
        {
            List<NELoopDetails> lstLoopMangment = BLLoopMangment.Instance.GetLoopDetails(obj.longitude, obj.latitude, obj.associated_system_id, obj.associated_System_Type, obj.structure_id);
            return PartialView("_LoopMangment", lstLoopMangment.OrderByDescending(m => m.loop_length).ToList());
        }
        private void ExportData(DataSet dtReport, string fileName, Dictionary<string, string> dicImages = null)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Tables.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.Tables[0].TableName))
                        dtReport.Tables[0].TableName = "EquipmentDetails";
                    IWorkbook workbook = NPOIExcelHelper.DatasetToExcel("xlsx", dtReport);
                    if (dicImages != null)
                    {
                        workbook = NPOIExcelHelper.AddImageExcel(workbook, dicImages);

                    }
                    //if(dtReport.Tables.Count >1)
                    //{
                    //    if (string.IsNullOrEmpty(dtReport.Tables[1].TableName))
                    //        dtReport.Tables[1].TableName = "ConnectionDetails";
                    //    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport.Tables[0]);

                    //}
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
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




        public PartialViewResult getLoopDetailsForCable(NELoopDetails obj)
        {
            List<NELoopDetails> lstLoopMangment = BLLoopMangment.Instance.GetLoopDetailsForCable(obj.cable_system_id);
            return PartialView("_LoopDetailsForCable", lstLoopMangment);

        }

        #region Cabinet
        public PartialViewResult AddCabinet(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {

            CabinetMaster model = new CabinetMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.pEntityType = pEntityType;
            model.pSystemId = pSystemId;
            model.pNetworkId = pNetworkId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CabinetMaster>(url, model, EntityType.Cabinet.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddCabinet", response.results);
        }
        public CabinetMaster GetCabinetDetail(string networkIdType, int systemId, ElementInfo ModelInfo = null, int pSystemId = 0, string pEntityType = "", string pNetworkid = "")
        {
            CabinetMaster objCabinet = new CabinetMaster();
            objCabinet.networkIdType = networkIdType;
            if (systemId == 0)
            {
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objCabinet, structureDetails, GeometryType.Point.ToString());
                //Fill Parent detail...              
                fillParentDetail(objCabinet, new ISPNetworkCodeIn() { parent_sysId = structureDetails.province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), parent_networkId = pNetworkid, eType = EntityType.Cabinet.ToString(), structureId = ModelInfo.structureid }, networkIdType, pSystemId, pEntityType, pNetworkid);
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<CabinetTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Cabinet);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objCabinet);
                objCabinet.objIspEntityMap.shaft_id = ModelInfo.shaftid;
                objCabinet.objIspEntityMap.floor_id = ModelInfo.floorid;
                objCabinet.objIspEntityMap.structure_id = ModelInfo.structureid;
                objCabinet.address = BLStructure.Instance.getBuildingAddress(ModelInfo.structureid);
                objCabinet.ownership_type = "Own";
            }
            else
            {
                // Get entity detail by Id...
                objCabinet = new BLMisc().GetEntityDetailById<CabinetMaster>(systemId, EntityType.Cabinet);
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.Cabinet.ToString());
                if (ispMapping != null)
                {
                    objCabinet.objIspEntityMap.shaft_id = ispMapping.shaft_id;
                    objCabinet.objIspEntityMap.floor_id = ispMapping.floor_id;
                    objCabinet.objIspEntityMap.structure_id = ispMapping.structure_id;
                }
            }
            return objCabinet;
        }
        public ActionResult SaveCabinet(CabinetMaster objCabinetMaster, bool isDirectSave = false)
        {

            objCabinetMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objCabinetMaster.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CabinetMaster>(url, objCabinetMaster, EntityType.Cabinet.ToString(), EntityAction.Save.ToString(), objCabinetMaster.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddCabinet", response.results);
        }
        private void BindCabinetDropdown(CabinetMaster objCabinetMaster)
        {
            objCabinetMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }
        #endregion

        public PartialViewResult GetPODAssociationDetail(PODAssociation obj)
        {
            obj.lstPODAssociation = new BLPOD().GetPODAssociationDetail(obj.geom, obj.associated_system_id, obj.associated_entity_Type);
            return PartialView("POD_Association", obj);

        }
        public PartialViewResult CreateModel(int? modelId)
        {
            Models.Admin.ISPModelInfo modelInfo = new Models.Admin.ISPModelInfo();
            modelInfo.is_editable = true;
            if (modelId != null)
            {
                ViewBag.ModelId = modelId;
                modelInfo = BLISPModelInfo.Instance.GetEquipmentInfo(modelId.Value).FirstOrDefault();
                if (modelInfo == null)
                    modelInfo = new Models.Admin.ISPModelInfo();
                modelInfo.lstModelType = BLISPModelInfo.Instance.GetModelTypes(modelInfo.model_id);
                modelInfo.is_editable = BLISPModelInfo.Instance.IsEditableModel(modelId.Value);
            }

            //modelInfo.lstModelStatus = BLISPModelInfo.Instance.GetModelStatus();
            modelInfo.lstModel = BLISPModelInfo.Instance.GetModels();

            modelInfo.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            modelInfo.model_template = InitModelTemplate(modelInfo);
            modelInfo.lstLabelColor = BLISPModelInfo.Instance.GetColorByModelKey("label");
            return PartialView("_EquipmentView", modelInfo);
        }
        public ISPModelTemplate InitModelTemplate(Models.Admin.ISPModelInfo modelInfo)
        {
            modelInfo.model_template = BLItemTemplate.Instance.GetTemplateDetail<ISPModelTemplate>(Convert.ToInt32(Session["user_id"]), EntityType.Equipment);
            modelInfo.entity_type = EntityType.Equipment.ToString();
            BLItemTemplate.Instance.BindItemDropdowns(modelInfo.model_template, EntityType.Equipment.ToString());
            if (!String.IsNullOrEmpty(modelInfo.modeltypevalue))
            {
                var layerDetail = BLISP.Instance.getLayerDetails(modelInfo.modeltypevalue);
                if (layerDetail != null && layerDetail.is_middleware_entity)
                {
                    modelInfo.entity_type = modelInfo.modeltypevalue;
                    var eType = modelInfo.modeltypevalue;

                    //if (modelInfo.modeltypevalue.ToUpper() == EntityType.HTB.ToString().ToUpper())
                    //{

                    modelInfo.model_template = BLItemTemplate.Instance.GetTemplateDetail<ISPModelTemplate>(Convert.ToInt32(Session["user_id"]), (EntityType)Enum.Parse(typeof(EntityType), eType));
                    BLItemTemplate.Instance.BindItemDropdowns(modelInfo.model_template, eType);
                }
            }
            if (modelInfo.item_template_id > 0)
            {

                var item = new BLVendorSpecification().GetVendorSpeicificationDetailsByID(modelInfo.item_template_id);
                if (item != null)
                {
                    modelInfo.model_template.lstVendor = BLItemTemplate.Instance.GetVendorList(item.specification);
                    modelInfo.model_template.specification = item.specification;
                    modelInfo.model_template.vendor_id = item.vendor_id;
                    modelInfo.model_template.item_code = item.code;
                    modelInfo.model_template.category = modelInfo.model_template.entityType;
                    modelInfo.model_template.subcategory1 = item.subcategory_1;
                    modelInfo.model_template.subcategory2 = item.subcategory_2;
                    modelInfo.model_template.subcategory3 = item.subcategory_3;
                    modelInfo.model_template.no_of_port = item.no_of_port;
                }
            }
            modelInfo.model_template.unit_input_type = "port";
            return modelInfo.model_template;

        }
        public JsonResult SaveModel(Models.Admin.ISPModelInfo input)
        {
            var usrDetail = (User)Session["userDetail"];

            EquipmentInfo eqInfo = new EquipmentInfo();
            eqInfo.width = input.width;
            eqInfo.height = input.height;
            eqInfo.system_id = input.system_id;
            BLRack.Instance.UpdateEquipment(eqInfo);

            DbMessage resp = BLISP.Instance.UpdateEquipment(input.system_id, JsonConvert.SerializeObject(input.lstModelChildren), usrDetail.user_id);

            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWorkspaceData(int modelId, bool? isStatic, bool isLibCall = false)
        {
            List<Areas.Admin.Models.WorkSpaceViewModel> result = new List<Areas.Admin.Models.WorkSpaceViewModel>();
            List<Models.Admin.ISPModelInfo> data = new List<Models.Admin.ISPModelInfo>();
            if (isLibCall)
            { data = BLISPModelInfo.Instance.GetModelsWithImage(modelId); }
            else { data = BLISPModelInfo.Instance.GetEquipmentWithImage(modelId); }

            if (data != null && data.Count > 0)
            {
                List<Models.Admin.ISPModelInfo> children = new List<Models.Admin.ISPModelInfo>();
                bool isEditable = true; //BLISPModelInfo.Instance.IsEditableModel(modelId);
                if (isLibCall)
                {
                    children = BLISPModelInfo.Instance.GetModelChildren(modelId);
                }
                else
                {
                    children = BLISPModelInfo.Instance.GetEquipmentChildren(modelId);
                }
                //

                //Get childrens
                foreach (var child in children)
                {
                    if (isStatic == null)
                        isStatic = child.parent != -1 && child.parent != modelId;
                    result.Add(new Areas.Admin.Models.WorkSpaceViewModel
                    {
                        id = child.map_id,
                        db_id = child.id,
                        model_id = child.model_id,
                        name = child.alt_name ?? child.model_name,
                        image_data = child.image_data,
                        img_id = child.model_image_id,
                        height = MMToPixel(child.height),
                        width = MMToPixel(child.width),
                        depth = MMToPixel(child.depth),
                        is_static = isStatic.Value,
                        offset_x = 0,
                        offset_y = 0,
                        color = child.color_code,
                        stroke = child.stroke_code,
                        parent = child.parent,
                        position = new Areas.Admin.Models.Position
                        {
                            x = MMToPixel(child.child_x_pos),
                            y = MMToPixel(child.child_y_pos)
                        },
                        rotation_angle = child.rotation_angle,
                        model_type_id = child.model_type_id,
                        model_view_id = child.model_view_id,
                        border_width = MMToPixel(child.border_width),
                        is_editable = isEditable,
                        db_border_width = child.border_width,
                        db_depth = child.depth,
                        db_height = child.height,
                        db_width = child.width,
                        db_parent = child.parent,
                        ref_id = child.map_id,
                        ref_parent = child.parent,
                        font_color = child.font_color,
                        font_size = child.font_size,
                        text_orientation = child.text_orientation,
                        bg_image = child.background_image,
                        model_color_id = child.model_color_id,
                        font_weight = child.font_weight,
                        border_color = child.border_color,
                        isNewEquipment = false
                    });
                }
            }
            result = result.OrderBy(p => p.parent).ToList();
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetModelMasterData()
        {
            var lstModel = BLISPModelInfo.Instance.GetModelMaster().ToList();
            return Json(lstModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLibraryData(int modelId, int? modelType)
        {
            List<Models.Admin.ISPModelInfo> models = BLISPModelInfo.Instance.GetEquipmentRules(modelId, modelType);
            var results = from m in models
                          group m by m.model_id into g
                          select new { ModelId = g.Key, Types = g.ToList() };
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetModelAllRules()
        {
            var result = BLISPModelInfo.Instance.GetModelAllRules();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [NonAction]
        public double MMToPixel(double mm)
        {
            double pixel = 0;
            pixel = mm * ApplicationSettings.WorkspaceCellSize / ApplicationSettings.WorkspaceScale;
            return pixel;


        }
        [NonAction]
        public double PixelToMM(double val)
        {
            double mm = 0;
            mm = val * ApplicationSettings.WorkspaceScale / ApplicationSettings.WorkspaceCellSize;
            return mm;


        }
        public PartialViewResult getISPEntityRefLink(int system_Id, string entity_type)
        {
            //string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            var lstDocument = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, "RefLink");
            List<DocumentResult> lstDocumentResult = new List<DocumentResult>();
            lstDocumentResult = GetDocumentList(lstDocument);
            return PartialView("_RefLinkUpload", lstDocumentResult);

        }
        public List<DocumentResult> GetDocumentList(List<DocumentResult> lstDocument)
        {
            List<DocumentResult> lstDocumentResult = new List<DocumentResult>();

            foreach (var item in lstDocument)
            {
                lstDocumentResult.Add(new DocumentResult()
                {
                    Id = item.Id,
                    EntitySystemId = item.EntitySystemId,
                    FileName = item.FileName,
                    EntityType = item.EntityType,
                    UploadedBy = item.UploadedBy,
                    created_on = MiscHelper.FormatDateTime(item.Uploaded_on.ToString()),
                    OrgFileName = item.OrgFileName,
                    FileExtension = item.FileExtension,
                    FileLocation = item.FileLocation,
                    UploadType = item.UploadType,
                    file_size = BytesToString(Convert.ToInt32(item.file_size)),
                    File_ShortName = Utility.CommonUtility.ConvertStringToShortFormat(item.OrgFileName, 19, 10, 9),
                    categorytype = item.categorytype
                });

            }
            return lstDocumentResult;
        }

        public PartialViewResult uploadReferenceLinks(int system_Id)
        {
            List<DocumentResult> Links = new List<DocumentResult>();
            return PartialView("_UploadRefLinks", Links);
        }


        #region PatchPanel
        public PatchPanelMaster GetPatchPanelDetail(string networkIdType, int templateId, int systemId, ElementInfo ModelInfo, int pSystemId, string pEntityType, string pNetworkId)
        {
            PatchPanelMaster objPatchPanel = new PatchPanelMaster();
            objPatchPanel.networkIdType = networkIdType;
            if (systemId != 0)
            {
                objPatchPanel = new BLMisc().GetEntityDetailById<PatchPanelMaster>(systemId, EntityType.PatchPanel, objPatchPanel.user_id);
                objPatchPanel.no_of_port = objPatchPanel.no_of_port;
                var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.PatchPanel.ToString());
                if (ispMapping != null && ispMapping.id > 0)
                {
                    objPatchPanel.objIspEntityMap.shaft_id = ispMapping.shaft_id;
                    objPatchPanel.objIspEntityMap.floor_id = ispMapping.floor_id;
                    objPatchPanel.objIspEntityMap.structure_id = ispMapping.structure_id;
                }
            }
            else
            {
                var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(ModelInfo.structureid, EntityType.Structure);
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objPatchPanel, structureDetails, GeometryType.Point.ToString());
                //Fill Parent detail...              
                fillParentDetail(objPatchPanel, new ISPNetworkCodeIn() { parent_sysId = ModelInfo.structureid, parent_eType = EntityType.Structure.ToString(), eType = EntityType.PatchPanel.ToString(), structureId = ModelInfo.structureid }, networkIdType, pSystemId, pEntityType, pNetworkId);
               
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<PatchPanelTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.PatchPanel);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objPatchPanel);
                objPatchPanel.no_of_port = objItem.no_of_port;
                objPatchPanel.objIspEntityMap.shaft_id = ModelInfo.shaftid;
                objPatchPanel.objIspEntityMap.floor_id = ModelInfo.floorid;
                objPatchPanel.objIspEntityMap.structure_id = ModelInfo.structureid;
                objPatchPanel.address = BLStructure.Instance.getBuildingAddress(ModelInfo.structureid);
                objPatchPanel.ownership_type = "Own";
            }
            return objPatchPanel;
        }

        public PartialViewResult AddPatchPanel(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            
            PatchPanelMaster model = new PatchPanelMaster();
            model.networkIdType = networkIdType;
            model.system_id = systemId;
            model.pEntityType = pEntityType;
            model.pSystemId = pSystemId;
            model.pNetworkId = pNetworkId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PatchPanelMaster>(url, model, EntityType.PatchPanel.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddPatchPanel", response.results);
        }
        public ActionResult SavePatchPanel(PatchPanelMaster objPatchPanelMaster, bool isDirectSave = false)
        {
            objPatchPanelMaster.user_id = Convert.ToInt32(Session["user_id"]);
            objPatchPanelMaster.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PatchPanelMaster>(url, objPatchPanelMaster, EntityType.PatchPanel.ToString(), EntityAction.Save.ToString(), objPatchPanelMaster.objIspEntityMap.structure_id.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AddPatchPanel", response.results);
        }
        public JsonResult DeletePatchPanelById(int systemId)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new DbMessage();
            response = new BLMisc().deleteEntity(systemId, EntityType.PatchPanel.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
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
        private void BindPatchPanelDropdown(PatchPanelMaster objPatchPanel)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.PatchPanel.ToString());
            objPatchPanel.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();

            objPatchPanel.listPatchPanelType = objDDL.Where(x => x.dropdown_type == DropDownType.PatchPanel_type.ToString()).ToList();
        }
        #endregion

        #region OpticalRepeater
        public ActionResult AddOpticalRepeater(string networkIdType, ElementInfo ModelInfo = null, int systemId = 0, int pSystemId = 0, string pEntityType = "", string pNetworkId = "")
        {
            //HTBInfo model = getHTBInfo(networkIdType, ModelInfo.templateId, systemId, ModelInfo.structureid);
            //if (systemId != 0)
            //{
            //    var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.HTB.ToString());
            //    model.objIspEntityMap.floor_id = ispMapping.floor_id;
            //    model.objIspEntityMap.structure_id = ispMapping.structure_id;
            //    model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
            //}
            //else
            //{
            //    model.objIspEntityMap.floor_id = ModelInfo.floorid;
            //    model.objIspEntityMap.structure_id = ModelInfo.structureid;
            //    model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            //}
            //BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
            //new MiscHelper().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //BindHTBDropDown(model);
            //fillProjectSpecifications(model);
            //return PartialView("_AddHTB", model);

            OpticalRepeaterInfo model = new OpticalRepeaterInfo();
            model.networkIdType = networkIdType;
            model.parent_entity_type = pEntityType;
            model.parent_system_id = pSystemId;
            model.parent_network_id = pNetworkId;
            model.system_id = systemId;
            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.objIspEntityMap.structure_id = ModelInfo.structureid;
            model.objIspEntityMap.floor_id = ModelInfo.floorid;
            model.objIspEntityMap.shaft_id = ModelInfo.shaftid;
            model.objIspEntityMap.template_id = ModelInfo.templateId;
            model.objIspEntityMap.operation = ModelInfo.operation;
            model.objIspEntityMap.element_type = ModelInfo.elementType;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<OpticalRepeaterInfo>(url, model, EntityType.OpticalRepeater.ToString(), EntityAction.Get.ToString(), ModelInfo.structureid.ToString());
            return PartialView("_AddOpticalRepeater", response.results);
        }
        private void BindOpticalRepeaterDropDown(OpticalRepeaterInfo objBDB)
        {
            var objDDL = new BLBDB().GetShaftFloorByStrucId(objBDB.parent_system_id);
            //objBDB.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
            objBDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }
        public OpticalRepeaterInfo getOpticalRepeaterInfo(string networkIdType, int templateId, int systemId, int structureId)
        {
            OpticalRepeaterInfo objOpticalRepeater = new OpticalRepeaterInfo();
            objOpticalRepeater.networkIdType = networkIdType;
            objOpticalRepeater.parent_system_id = structureId;
            objOpticalRepeater.parent_entity_type = EntityType.Structure.ToString();
            if (systemId != 0)
            {
                objOpticalRepeater = BLISP.Instance.getOpticalRepeaterDetails(systemId);
            }
            else
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureId, parent_eType = EntityType.Structure.ToString(), eType = EntityType.OpticalRepeater.ToString(), structureId = structureId });
                    objOpticalRepeater.network_id = ISPNetworkCodeDetail.network_code;
                }
                objOpticalRepeater.ownership_type = "Own";
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<OpticalRepeaterTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.OpticalRepeater);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objOpticalRepeater);

            }
            return objOpticalRepeater;
        }
        public ActionResult SaveOpticalRepeater(OpticalRepeaterInfo model, bool isDirectSave = false)
        {

            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //int structure_id = model.objIspEntityMap.structure_id;
            //int? floor_id = model.objIspEntityMap.floor_id;
            //int? shaft_id = model.objIspEntityMap.shaft_id;
            //string pNetworkId = model.pNetworkId;
            //int pSystemId = model.pSystemId;
            //string pEntityType = model.pEntityType;
            //model.userId = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            //if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
            //{
            //    //GET AUTO NETWORK CODE...
            //    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.HTB.ToString(), structureId = structure_id });
            //    if (isDirectSave == true)
            //    {
            //        model = getHTBInfo(model.networkIdType, model.templateId, model.system_id, model.objIspEntityMap.structure_id);
            //        model.objIspEntityMap.floor_id = floor_id;
            //        model.objIspEntityMap.structure_id = structure_id;
            //        model.objIspEntityMap.shaft_id = shaft_id;
            //        model.parent_system_id = pSystemId;
            //        model.parent_entity_type = pEntityType;
            //        model.parent_network_id = pNetworkId;
            //        model.htb_name = objISPNetworkCode.network_code;
            //    }
            //    model.network_id = objISPNetworkCode.network_code;
            //    model.sequence_id = objISPNetworkCode.sequence_id;

            //}
            //var structureDetails = new BLISP().GetStructureById(structure_id);
            //if (structureDetails != null)
            //{
            //    model.region_id = structureDetails.First().region_id;
            //    model.province_id = structureDetails.First().province_id;
            //    model.latitude = structureDetails.First().latitude;
            //    model.longitude = structureDetails.First().longitude;
            //}
            //if (TryValidateModel(model))
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.HTB.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    bool isNew = model.system_id == 0 ? true : false;
            //    if (model.unitValue != null && model.unitValue.Contains(":"))
            //    {
            //        model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
            //        model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
            //    }
            //    var result = new BLISP().SaveHTBDetails(model, Convert.ToInt32(Session["user_id"]));
            //    if (string.IsNullOrEmpty(result.objPM.message))
            //    {

            //        if (isNew)
            //        {
            //            string[] LayerName = { EntityType.HTB.ToString() };
            //            objPM.status = ResponseStatus.OK.ToString(); ;
            //            objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "HTB saved successfully.";
            //            objPM.systemId = result.system_id;
            //            objPM.entityType = EntityType.HTB.ToString();
            //            objPM.NetworkId = result.network_id;
            //            objPM.structureId = model.objIspEntityMap.structure_id;
            //            objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
            //            objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
            //            objPM.pSystemId = model.parent_system_id;
            //            objPM.pEntityType = model.parent_entity_type;
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
            //                string[] LayerName = { layer_title };
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
            //model.entityType = EntityType.HTB.ToString();
            //if (isDirectSave == true)
            //{
            //    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
            //    return Json(model.objPM, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
            //    new MiscHelper().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //    BindHTBDropDown(model);
            //    fillProjectSpecifications(model);
            //    return PartialView("_AddHTB", model);
            //}

            model.user_id = Convert.ToInt32(Session["user_id"]);
            model.isDirectSave = isDirectSave;
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<OpticalRepeaterInfo>(url, model, EntityType.OpticalRepeater.ToString(), EntityAction.Save.ToString(), model.objIspEntityMap.structure_id.ToString());
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
            response = new BLMisc().deleteEntity(systemId, EntityType.OpticalRepeater.ToString(), GeometryType.Point.ToString(),usrDetail.user_id);
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

        #region UPDATE GEOGRAPHIC DETAILS FEATURE BY ADITYA

        public ActionResult Auto_Codification(string pEntityType, int pSystemId, string pGeomType)
        {
            int p_user_id = Convert.ToInt32(Session["user_id"]);
            var ProcessData = BLBuilding.Instance.UpdateGeographicDetails(pEntityType, pSystemId, pGeomType, p_user_id);
            ProcessData.listLog = JsonConvert.DeserializeObject<List<CodificationLog>>(ProcessData.logs);
            ProcessData.message = BLConvertMLanguage.MultilingualMessageConvert(ProcessData.message);
            DataTable dtgeo_log = MiscHelper.ListToDataTable(ProcessData.listLog);
            TempData["codification_dt"] = dtgeo_log;
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
        #endregion
        private void fillRegionProvAbbr(dynamic objEntityModel)
        {
            List<InRegionProvince> objRegionProvince = new List<InRegionProvince>();
            objRegionProvince = BLBuilding.Instance.GetRegionProvinceById(objEntityModel.region_id, objEntityModel.province_id);
            objEntityModel.region_abbreviation = objRegionProvince[0].region_abbreviation;
            objEntityModel.province_abbreviation = objRegionProvince[0].province_abbreviation;
        }
    }

}