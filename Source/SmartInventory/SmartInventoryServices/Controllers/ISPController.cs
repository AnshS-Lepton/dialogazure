using BusinessLogics;
using BusinessLogics.Admin;
using BusinessLogics.ISP;
using Models;
using Models.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartInventory.Settings;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Serialization;
using Utility;
using Models.ISP;


namespace SmartInventoryServices.Controllers
{    

    [RoutePrefix("api/ISP")]
    [CustomAuthorization]
    [CustomAction]    
    public class ISPController : ApiController
    {
        #region Entity Operations
        /// <summary>
        /// Entity Operations Generic Api
        /// </summary>
        /// <param name="data">Json Data</param>
        /// <returns>Result According TO pass Action</returns>
        /// <Created By>Antra Mathur</Created By>
        /// <Created Date>22-Feb-2021</Created Date>
        [HttpPost]
        public dynamic EntityOperations(ReqInput data)
        {
            var response = new ApiResponse<dynamic>();
            HeaderAttributes headerAttribute = ReqHelper.getHeaderValue(Request.Headers.ToList());
            data.data = ReqHelper.MergeHeaderAttributeInJsonObject(data, headerAttribute);
            this.Validate(headerAttribute);
            if (ModelState.IsValid)
            {
                if (headerAttribute.entity_type.ToUpper() == EntityType.FDB.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return AddFDB(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveFDB(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    return response;
                }
            }
            return response;
        }
        #endregion

        #region FDB
        #region Add FDB
        /// <summary> Add FDB </summary>
        /// <param name="data">networkIdType,systemId,geom,userId</param>
        /// <returns>FDB Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>

        public ApiResponse<FDBInfo> AddFDB(ReqInput data)
        {
            var response = new ApiResponse<FDBInfo>();
            try
            {
                FDBInfo objFDB = ReqHelper.GetRequestData<FDBInfo>(data);
                FDBInfo model = getFDBInfo(objFDB);
                if (objFDB.systemId != 0)
                {
                    var ispMapping = new BLISP().getMappingByEntityId(objFDB.systemId, EntityType.FDB.ToString());
                    model.objIspEntityMap.floor_id = ispMapping.floor_id;
                    model.objIspEntityMap.structure_id = ispMapping.structure_id;
                    model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
                }
                else
                {
                    model.objIspEntityMap.floor_id = objFDB.floorid;
                    model.objIspEntityMap.structure_id = objFDB.structureid;
                    model.objIspEntityMap.shaft_id = objFDB.shaftid;
                }
                new BLMisc().BindPortDetails(model, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
                BindFDBDropdown(model);
                response.status = StatusCodes.OK.ToString();
                response.results = model;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("AddFDB()", "ISP Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message;
            }
            return response;
        }
        #endregion

        #region Get FDB Details
        /// <summary> GetFDBDetail</summary>
        /// <param >networkIdType,systemId,structureId,templateId</param>
        /// <returns>FDB Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        public FDBInfo getFDBInfo(FDBInfo objFDB)
        {
            objFDB.parent_system_id = objFDB.structureid;
            objFDB.parent_entity_type = EntityType.Structure.ToString();
            if (objFDB.systemId != 0)
            {
                objFDB = BLISP.Instance.getFDBDetails(objFDB.systemId);
            }
            else
            {
                if (objFDB.networkIdType == NetworkIdType.M.ToString())
                {
                    // for Manual network id type 
                    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objFDB.structureid, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = objFDB.structureid });
                    objFDB.network_id = ISPNetworkCodeDetail.network_code;
                }
                objFDB.ownership_type = "Own";
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplateMaster>(objFDB.user_id, EntityType.FDB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objFDB);


            }
            BLItemTemplate.Instance.BindItemDropdowns(objFDB, EntityType.FDB.ToString());
            fillProjectSpecifications(objFDB);
            return objFDB;
        }
        #endregion

        #region Save FDB
        /// <summary> SaveFDB </summary>
        /// <param name="data">ReqInput</param>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        public ApiResponse<FDBInfo> SaveFDB(ReqInput data)
        {
            var response = new ApiResponse<FDBInfo>();
            FDBInfo model = ReqHelper.GetRequestData<FDBInfo>(data);
            try
            {
                ModelState.Clear();
                int structure_id = model.objIspEntityMap.structure_id;
                int floor_id = model.objIspEntityMap.floor_id ?? 0;
                int shaft_id = model.objIspEntityMap.shaft_id ?? 0;
                if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
                {
                    //GET AUTO NETWORK CODE...
                    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.objIspEntityMap.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = model.objIspEntityMap.structure_id });
                    if (model.isDirectSave == true)
                    {
                        model = getFDBInfo(model);
                        model.objIspEntityMap.floor_id = floor_id;
                        model.objIspEntityMap.structure_id = structure_id;
                        model.objIspEntityMap.shaft_id = shaft_id;
                        model.fdb_name = objISPNetworkCode.network_code;
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
                this.Validate(model);
                if (ModelState.IsValid)
                {
                    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault().layer_title;

                    bool isNew = model.system_id == 0 ? true : false;
                    model.userId = model.user_id;
                    if (model.unitValue != null && model.unitValue.Contains(":"))
                    {
                        model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
                        model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
                    }
                    var resultItem = new BLISP().SaveFDBDetails(model);
                    if (string.IsNullOrEmpty(resultItem.objPM.message))
                    {

                        //Save Reference
                        if (model.EntityReference != null && resultItem.system_id > 0)
                        {
                            SaveReference(model.EntityReference, resultItem.system_id);
                        }
                        if (isNew)
                        {
                            string[] LayerName = { EntityType.FDB.ToString() };
                           model.objPM.status = ResponseStatus.OK.ToString(); ;
                            model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                            model.objPM.systemId = resultItem.system_id;
                            model.objPM.entityType = EntityType.FDB.ToString();
                            model.objPM.NetworkId = resultItem.network_id;
                            model.objPM.structureId = model.objIspEntityMap.structure_id;
                            model.objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
                            model.objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
                            model.objPM.pSystemId = model.parent_system_id;
                            model.objPM.pEntityType = model.parent_entity_type;
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                            response.status = ResponseStatus.OK.ToString();
                        }
                        else
                        {
                            if (resultItem.isPortConnected == true)
                            {
                                model.objPM.status = ResponseStatus.OK.ToString();
                                model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
                                response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
                                response.status = ResponseStatus.OK.ToString();
                            }
                            else
                            {
                                string[] LayerName = { EntityType.FDB.ToString() };
                                model.objPM.status = ResponseStatus.OK.ToString();
                                model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                                response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                                response.status = ResponseStatus.OK.ToString();
                            }
                        }
                    }

                }
                else
                {
                    model.objPM.status = ResponseStatus.FAILED.ToString();
                    model.objPM.message = getFirstErrorFromModelState();
                    response.error_message = getFirstErrorFromModelState();
                    response.status = ResponseStatus.FAILED.ToString();
                }
                model.entityType = EntityType.FDB.ToString();
                if (model.isDirectSave == true)
                {
                    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                    response.results = model;
                    response.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.FDB.ToString());
                    BindFDBDropdown(model);
                    fillProjectSpecifications(model);
                    new BLMisc().BindPortDetails(model, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
                    response.results = model;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveFDB()", "ISP Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message;
            }
            return response;
        }
        #endregion

        #region Bind FDB Dropdown
        /// <summary>BindFDBDropDown </summary>
        /// <param name="objFDB"></param>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        private void BindFDBDropdown(FDBInfo objFDB)
        {
            objFDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }
        #endregion

        #endregion

        #region Error Handling
        /// <summary> Error Handling </summary>
        /// <returns>Error Message</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        public string getFirstErrorFromModelState()
        {
            var Error = ModelState.Values.Select(e => e.Errors).FirstOrDefault();
            if (Error != null)
                return Error[0].ErrorMessage;
            else
                return "";
        }
        #endregion

        #region Fill Project Specifications Details
        /// <summary>fillProjectSpecifications in modal </summary>
        /// <param>objLib</param>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        private void fillProjectSpecifications(dynamic objLib)
        {
            //"P" we need to pass this value as dynamically as network stage selection
            objLib.lstBindProjectCode = new BLProject().getProjectCodeDetails("P");
            objLib.lstBindPlanningCode = new BLProject().getPlanningDetailByIdList(Convert.ToInt32(objLib.project_id ?? 0));
            objLib.lstBindWorkorderCode = new BLProject().getWorkorderDetailByIdList(Convert.ToInt32(objLib.planning_id ?? 0));
            objLib.lstBindPurposeCode = new BLProject().getPurposeDetailByIdList(Convert.ToInt32(objLib.workorder_id ?? 0));
        }
        #endregion

        #region Save Reference
        /// <summary> SaveReference </summary>
        /// <param>entityReference , system_id</param>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        private void SaveReference(EntityReference entityReference, int system_id)
        {
            BLReference.Instance.SaveReference(entityReference, system_id);
        }
        #endregion
    }
}