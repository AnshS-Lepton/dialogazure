using BusinessLogics;
using BusinessLogics.Admin;
using BusinessLogics.ISP;
using Lepton.Utility;
using Models;
using Models.API;
using Models.Extension;
using Newtonsoft.Json;
using SmartInventory.Settings;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;

namespace SmartInventoryServices.Controllers
{
    [System.Web.Http.Authorize]
    [CustomAuthorization]
    [APIExceptionFilter]
    //[CustomAction]
    public class MainController : ApiController
    {

        [System.Web.Http.HttpPost]
        public ApiResponse<List<EntityDropDownItemList>> GetDropDownItems(ReqInput data)
        {
            var response = new ApiResponse<List<EntityDropDownItemList>>();
            try
            {
                ItemDropDownIn objItemDropDownIn = ReqHelper.GetRequestData<ItemDropDownIn>(data);
                if (string.IsNullOrEmpty(objItemDropDownIn.ddlType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Drop down type is required.";
                    return response;
                }

                // commented code becuase it is handled in procedure
                //else if (string.IsNullOrEmpty(objItemDropDownIn.entityType))
                //{
                //    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //    response.error_message = "Entity type is required.";
                //    return response;
                //}
                var ddlList = new BLMisc().GetDropDownList(objItemDropDownIn.entityType, objItemDropDownIn.ddlType).Select(m => (new EntityDropDownItemList { key = m.dropdown_key, value = m.dropdown_value })).ToList();
                if (ddlList.Count > 0)
                {
                    response.results = ddlList;
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "Record not found.";
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ItemDropDown()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        // [HttpPost]
        //public ApiResponse<string> ValidateEntityGeom(ReqInput data)
        //{
        //    var response = new ApiResponse<string>();
        //    try
        //    {
        //        ValidateEntityGeomIn objValidateEntityGeomIn = ReqHelper.GetRequestData<ValidateEntityGeomIn>(data);
        //        if (string.IsNullOrEmpty(objValidateEntityGeomIn.entityType))
        //        {
        //            response.status = StatusCodes.VALIDATION_FAILED.ToString();
        //            response.error_message = "Entity type is required.";
        //            return response;
        //        }
        //        else if (string.IsNullOrEmpty(objValidateEntityGeomIn.geomType))
        //        {
        //            response.status = StatusCodes.VALIDATION_FAILED.ToString();
        //            response.error_message = "Geometry type is required.";
        //            return response;
        //        }
        //        else if (string.IsNullOrEmpty(objValidateEntityGeomIn.longitude))
        //        {
        //            response.status = StatusCodes.VALIDATION_FAILED.ToString();
        //            response.error_message = "Longitude is required.";
        //            return response;
        //        }
        //        else if (string.IsNullOrEmpty(objValidateEntityGeomIn.latitude))
        //        {
        //            response.status = StatusCodes.VALIDATION_FAILED.ToString();
        //            response.error_message = "Latitude is required.";
        //            return response;
        //        }
        //        var objRegPro = BLBuilding.Instance.GetRegionProvince(objValidateEntityGeomIn.longitude + " " + objValidateEntityGeomIn.latitude, objValidateEntityGeomIn.geomType);
        //        if (objRegPro != null && objRegPro.Count == 1)
        //        {
        //            if (objValidateEntityGeomIn.entityType == EntityType.Splitter.ToString())
        //            {
        //                var chkSubAreaExist = new BLMisc().GetNetworkDetails(objValidateEntityGeomIn.longitude + " " + objValidateEntityGeomIn.latitude, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
        //                if (chkSubAreaExist.system_id == 0)
        //                {
        //                    response.status = ResponseStatus.FAILED.ToString();
        //                    response.error_message = "No SubArea exist at this location!";
        //                }
        //                else
        //                    response.status = ResponseStatus.OK.ToString();

        //            }
        //            if (objValidateEntityGeomIn.entityType == EntityType.ADB.ToString())
        //            {
        //                var chkSubAreaExist = new BLMisc().GetNetworkDetails(objValidateEntityGeomIn.longitude + " " + objValidateEntityGeomIn.latitude, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
        //                if (chkSubAreaExist.system_id == 0)
        //                {
        //                    response.status = ResponseStatus.FAILED.ToString();
        //                    response.error_message = "No SubArea exist at this location!";
        //                }
        //                else
        //                    response.status = ResponseStatus.OK.ToString();

        //            }

        //            if (objValidateEntityGeomIn.entityType == EntityType.Customer.ToString())
        //            {
        //                var chkSubAreaExist = new BLMisc().GetNetworkDetails(objValidateEntityGeomIn.longitude + " " + objValidateEntityGeomIn.latitude, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
        //                if (chkSubAreaExist.system_id == 0)
        //                {
        //                    response.status = ResponseStatus.FAILED.ToString();
        //                    response.error_message = "No SubArea exist at this location!";
        //                }
        //                else
        //                    response.status = ResponseStatus.OK.ToString();

        //            }

        //            else if (objValidateEntityGeomIn.entityType == EntityType.BDB.ToString())
        //            {
        //                var chkBuildingExist = new BLMisc().GetNetworkDetails(objValidateEntityGeomIn.longitude + " " + objValidateEntityGeomIn.latitude, GeometryType.Point.ToString(), EntityType.Building.ToString());
        //                if (chkBuildingExist.system_id == 0)
        //                {
        //                    response.status = ResponseStatus.FAILED.ToString();
        //                    response.error_message = "No Building exist at this location!";
        //                }
        //                else
        //                    response.status = ResponseStatus.OK.ToString();
        //            }
        //            else if (objValidateEntityGeomIn.entityType == EntityType.BDB.ToString())
        //            {
        //                var chkSubAreaExist = new BLMisc().GetNetworkDetails(objValidateEntityGeomIn.longitude + " " + objValidateEntityGeomIn.latitude, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
        //                if (chkSubAreaExist.system_id == 0)
        //                {
        //                    response.status = ResponseStatus.FAILED.ToString();
        //                    response.error_message = "No SubArea exist at this location!";
        //                }
        //                else
        //                    response.status = ResponseStatus.OK.ToString();
        //            }
        //            else if (objRegPro != null)
        //            {
        //                response.status = ResponseStatus.OK.ToString();
        //            }


        //        }
        //        else
        //        {
        //            response.status = ResponseStatus.FAILED.ToString();
        //            response.error_message = objRegPro.Count == 0 ? "No Region/Province exist at this location!" : objValidateEntityGeomIn.entityType + " is selected multiple region/province!";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogHelper logHelper = new ErrorLogHelper();
        //        logHelper.ApiLogWriter("ValidateEntityGeom()", "Main Controller", data.data, ex);
        //        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
        //        response.error_message = "Error While Processing  Request.";
        //    }
        //    return response;
        //}
        [System.Web.Http.HttpPost]
        public ApiResponse<string> DeleteEntityFromInfo(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                DeleteEntityFromInfo objDeleteEntityFromInfo = ReqHelper.GetRequestData<DeleteEntityFromInfo>(data);
                EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), objDeleteEntityFromInfo.entityType);
                int deleteChk = 0;
                switch (enType)
                {
                    case EntityType.Building:
                        deleteChk = BLBuilding.Instance.DeleteBuildingById(objDeleteEntityFromInfo.systemId);
                        break;
                }
                if (deleteChk == 1)
                {
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = objDeleteEntityFromInfo.entityType + " has deleted successfully.";
                }
                else
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = objDeleteEntityFromInfo.entityType + " has not deleted!";

                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("DeleteEntityFromInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<dynamic> GetEntityTemplate(ReqInput data)
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                EntityTemplateIn objEntityTemplateIn = ReqHelper.GetRequestData<EntityTemplateIn>(data);
                if (string.IsNullOrEmpty(objEntityTemplateIn.entityType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Please enter the entity type!";
                }
                else if (objEntityTemplateIn.userId == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid user id!";
                }
                EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), objEntityTemplateIn.entityType);
                switch (enType)
                {
                    case EntityType.Splitter:
                        {

                            response.status = StatusCodes.OK.ToString();
                            response.results = getSplitterTemplate(objEntityTemplateIn.userId);

                        }
                        break;
                    case EntityType.ADB:
                        {
                            {

                                response.status = StatusCodes.OK.ToString();
                                response.results = getADBTemplate(objEntityTemplateIn.userId);

                            }
                            break;
                        }
                    case EntityType.CDB:
                        {
                            {

                                response.status = StatusCodes.OK.ToString();
                                response.results = getCDBTemplate(objEntityTemplateIn.userId);

                            }
                            break;
                        }
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntityTemplate()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        public ADBMobileTemplateMaster getADBTemplate(int userid)
        {
            ADBMobileTemplateMaster objADBItem = new ADBMobileTemplateMaster();
            var result = BLItemTemplate.Instance.GetTemplateDetail<ADBItemMaster>(Convert.ToInt32(userid), EntityType.ADB);
            if (result != null && result.specification != null && result.item_code != null)
            {
                objADBItem.category = result.category; objADBItem.item_code = result.item_code;
                objADBItem.specification = result.specification;
                objADBItem.subcategory1 = result.subcategory1; objADBItem.subcategory2 = result.subcategory2;
                objADBItem.subcategory3 = result.subcategory3;
                objADBItem.unitValue = result.unitValue; objADBItem.vendor_id = result.vendor_id;
                objADBItem.id = result.id; objADBItem.entity_category = result.entity_category; objADBItem.created_by = result.created_by;
                objADBItem.no_of_input_port = result.no_of_input_port; objADBItem.no_of_output_port = result.no_of_output_port; objADBItem.no_of_port = result.no_of_port;
            }
            var layerdetails = new BLLayer().getLayer(EntityType.ADB.ToString());
            if (layerdetails != null)
            {
                objADBItem.unit_input_type = layerdetails.unit_input_type;
            }
            return objADBItem;
        }
        public CDBMobileTemplateMaster getCDBTemplate(int userid)
        {
            CDBMobileTemplateMaster objCDBItem = new CDBMobileTemplateMaster();
            var result = BLItemTemplate.Instance.GetTemplateDetail<CDBItemMaster>(Convert.ToInt32(userid), EntityType.CDB);
            if (result != null && result.specification != null && result.item_code != null)
            {
                objCDBItem.category = result.category; objCDBItem.item_code = result.item_code;
                objCDBItem.specification = result.specification;
                objCDBItem.subcategory1 = result.subcategory1; objCDBItem.subcategory2 = result.subcategory2;
                objCDBItem.subcategory3 = result.subcategory3;
                objCDBItem.unitValue = result.unitValue; objCDBItem.vendor_id = result.vendor_id;
                objCDBItem.id = result.id; objCDBItem.entity_category = result.entity_category; objCDBItem.created_by = result.created_by;
                objCDBItem.no_of_input_port = result.no_of_input_port; objCDBItem.no_of_output_port = result.no_of_output_port; objCDBItem.no_of_port = result.no_of_port;

            }
            var layerdetails = new BLLayer().getLayer(EntityType.CDB.ToString());
            if (layerdetails != null)
            {
                objCDBItem.unit_input_type = layerdetails.unit_input_type;
            }
            return objCDBItem;
        }
        public SplitterMobileTemplateMaster getSplitterTemplate(int userid)
        {
            SplitterMobileTemplateMaster objSplitterItem = new SplitterMobileTemplateMaster();
            var result = BLItemTemplate.Instance.GetTemplateDetail<SplitterItemMaster>(userid, EntityType.Splitter);
            if (result != null && result.specification != null && result.item_code != null)
            {

                objSplitterItem.category = result.category; objSplitterItem.item_code = result.item_code;
                objSplitterItem.specification = result.specification;
                objSplitterItem.subcategory1 = result.subcategory1; objSplitterItem.subcategory2 = result.subcategory2;
                objSplitterItem.subcategory3 = result.subcategory3;
                objSplitterItem.unitValue = result.unitValue; objSplitterItem.vendor_id = result.vendor_id;
                objSplitterItem.id = result.id; objSplitterItem.splitter_type = result.splitter_type; objSplitterItem.created_by = result.created_by;
                objSplitterItem.unitValue = result.splitter_ratio;

            }
            var layerdetails = new BLLayer().getLayer(EntityType.Splitter.ToString());
            if (layerdetails != null)
            {
                objSplitterItem.unit_input_type = layerdetails.unit_input_type;
            }
            return objSplitterItem;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<List<EntityDetail>> GetNearByEntities(ReqInput data)
        {
            var response = new ApiResponse<List<EntityDetail>>();
            try
            {

                NearByEntitiesIn objEntityTemplateIn = ReqHelper.GetRequestData<NearByEntitiesIn>(data);
                if (objEntityTemplateIn.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }
                else if (objEntityTemplateIn.latitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid latitude!";
                    return response;
                }
                else if (objEntityTemplateIn.longitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid longitude!";
                    return response;
                }
                //Models.User objUser = new BLUser().GetUserDetailByID(objEntityTemplateIn.user_id);
                response.status = StatusCodes.OK.ToString();
                response.results = new BLMisc().getNearByEntities(objEntityTemplateIn.latitude, objEntityTemplateIn.longitude, objEntityTemplateIn.bufferInMtrs, objEntityTemplateIn.userId);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearByEntities()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<List<EntityDetailWithAttribute>> GetNearByEntitiesWithAttribute(ReqInput data)
        {
            var response = new ApiResponse<List<EntityDetailWithAttribute>>();
            try
            {

                NearByEntitiesIn objEntityTemplateIn = ReqHelper.GetRequestData<NearByEntitiesIn>(data);
                if (objEntityTemplateIn.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }
                else if (objEntityTemplateIn.latitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid latitude!";
                    return response;
                }
                else if (objEntityTemplateIn.longitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid longitude!";
                    return response;
                }                
                response.status = StatusCodes.OK.ToString();
                response.results = new BLMisc().GetNearByEntitiesWithAttribute(objEntityTemplateIn.latitude, objEntityTemplateIn.longitude, objEntityTemplateIn.bufferInMtrs,objEntityTemplateIn.ticket_id, objEntityTemplateIn.userId); ;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearByEntities()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<List<entityInfo>> GetEntityInfo(ReqInput data)
        {
            var response = new ApiResponse<List<entityInfo>>();
            try
            {
                EntityInfoIn objEntityInfoIn = ReqHelper.GetRequestData<EntityInfoIn>(data);
                string[] arrIgnoreColumns = { };
                if (objEntityInfoIn.systemId == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid system id!";
                    return response;
                }
                else if (string.IsNullOrEmpty(objEntityInfoIn.entityType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Entity Type is required!";
                    return response;
                }
                else if (string.IsNullOrEmpty(objEntityInfoIn.geomType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Geometry Type is required!";
                    return response;
                }
                var dicEntityInfo = new BLMisc().getEntityInfo(objEntityInfoIn.systemId, objEntityInfoIn.entityType, objEntityInfoIn.geomType, objEntityInfoIn.user_id);
                response.results = BLConvertMLanguage.MultilingualConvertModel(dicEntityInfo, arrIgnoreColumns);
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntityInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<List<SearchResult>> GetEntitySearchResult(ReqInput data)
        {
            var response = new ApiResponse<List<SearchResult>>();
            try
            {
                SearchResultIn objSearchResultIn = ReqHelper.GetRequestData<SearchResultIn>(data);
                BLSearch objBLSearch = new BLSearch();
                List<SearchResult> lstSearchResult = new List<SearchResult>();
                var serchvalue = objSearchResultIn.SearchText.TrimEnd();
                if (!string.IsNullOrWhiteSpace(serchvalue))
                {
                    var arrSrchText = serchvalue.Split(':');
                    if (arrSrchText.Length == 2)
                    {
                        if (arrSrchText[1].Length >= ApplicationSettings.EntitySearchLength)
                        {
                            lstSearchResult = objBLSearch.GetSearchEntityResult(arrSrchText[0], arrSrchText[1], objSearchResultIn.userId, "");
                        }
                    }
                    else
                    {
                        lstSearchResult = objBLSearch.GetSearchEntityType(arrSrchText[0], objSearchResultIn.userRoleId);
                    }
                }
                if (lstSearchResult.Count > 0)
                {
                    response.results = lstSearchResult;
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.error_message = "Record not found!";
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntitySearchResult()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        [System.Web.Http.HttpPost]
        public ApiResponse<GeometryDetail> getGeometryDetail(ReqInput data)
        {
            GeomDetailIn objGeomDetailIn = ReqHelper.GetRequestData<GeomDetailIn>(data);
            var objGeometryDetail = new BLSearch().GetGeometryDetails(objGeomDetailIn);
            var response = new ApiResponse<GeometryDetail>();
            try
            {

                if (objGeometryDetail.geometry_extent != null)
                {
                    var extent = objGeometryDetail.geometry_extent.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
                    string[] bounds = extent.Split(',');
                    string[] southWest = bounds[0].Split(' ');
                    string[] northEast = bounds[1].Split(' ');
                    objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
                    objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
                    response.results = objGeometryDetail;
                    response.status = ResponseStatus.OK.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getGeometryDetail()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #region SPLITTER
        [System.Web.Http.HttpPost]
        public ApiResponse<SplitterMobileMaster> GetSplitterDetail(ReqInput data)
        {
            var response = new ApiResponse<SplitterMobileMaster>();
            try
            {
                SplitterDetailIn objSplitterDetailIn = ReqHelper.GetRequestData<SplitterDetailIn>(data);
                SplitterMobileMaster objSplitterMobile = new SplitterMobileMaster();
                objSplitterMobile.LayerDetails = new BLLayer().GetSpltParentBoxDetails();
                if (objSplitterDetailIn.systemId == 0)
                {
                    if (string.IsNullOrEmpty(objSplitterDetailIn.geom))
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Please enter the geomtry!";
                        return response;
                    }
                    else if (objSplitterDetailIn.userId == 0)
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Invalid user id!";
                        return response;
                    }
                    if (objSplitterMobile.LayerDetails.Count > 0)
                    {
                        var adbLayerDetails = objSplitterMobile.LayerDetails.Where(m => m.Layer_name == EntityType.ADB.ToString()).FirstOrDefault();
                        var cdbLayerDetails = objSplitterMobile.LayerDetails.Where(m => m.Layer_name == EntityType.CDB.ToString()).FirstOrDefault();
                        if (adbLayerDetails != null && adbLayerDetails.network_id_type == NetworkIdType.M.ToString())
                        {
                            //GET ADB AUTO NETWORK CODE...
                            var objADBNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ADB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSplitterDetailIn.geom });
                            objSplitterMobile.adb_network_id = objADBNetworkCodeDetail.network_code;
                        }
                        if (cdbLayerDetails != null && cdbLayerDetails.network_id_type == NetworkIdType.M.ToString())
                        {
                            //GET ADB AUTO NETWORK CODE...
                            var objCDBNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.CDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSplitterDetailIn.geom });
                            objSplitterMobile.cdb_network_id = objCDBNetworkCodeDetail.network_code;
                        }
                    }
                    objSplitterMobile.longitude = Convert.ToDouble(objSplitterDetailIn.geom.Split(' ')[0]);
                    objSplitterMobile.latitude = Convert.ToDouble(objSplitterDetailIn.geom.Split(' ')[1]);
                    objSplitterMobile.geom = objSplitterDetailIn.geom;
                    objSplitterMobile.userid = objSplitterDetailIn.userId;
                    //Parent details                    
                    var parentNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ADB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSplitterDetailIn.geom });
                    if (parentNetworkCodeDetail.parent_entity_type == EntityType.SubArea.ToString())
                    {
                        //Item template binding                       
                        var result = BLItemTemplate.Instance.GetTemplateDetail<SplitterTemplateMaster>(objSplitterDetailIn.userId, EntityType.Splitter);
                        // Utility.MiscHelper.CopyMatchingBaseProperties(result, objSplitter);
                        objSplitterMobile.parent_network_id = parentNetworkCodeDetail.parent_network_id;
                        objSplitterMobile.splitter_type = result.splitter_type;
                        objSplitterMobile.splitter_ratio = result.splitter_ratio;
                        response.status = StatusCodes.OK.ToString();
                        response.results = objSplitterMobile;
                    }
                    else
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Sub Area does not exist at this location!";
                    }
                }
                else
                {
                    SplitterMaster objSplitter = new SplitterMaster();
                    objSplitter = new BLMisc().GetEntityDetailById<SplitterMaster>(objSplitterDetailIn.systemId, EntityType.Splitter);
                    objSplitterMobile.system_id = objSplitter.system_id;
                    objSplitterMobile.splitter_name = objSplitter.splitter_name;
                    objSplitterMobile.network_id = objSplitter.network_id;
                    objSplitterMobile.splitter_ratio = objSplitter.splitter_ratio;
                    objSplitterMobile.splitter_type = objSplitter.splitter_type;
                    objSplitterMobile.longitude = objSplitter.longitude;
                    objSplitterMobile.latitude = objSplitter.latitude;
                    objSplitterMobile.address = objSplitter.address;
                    objSplitterMobile.boxType = objSplitter.parent_entity_type;
                    if (objSplitter.parent_entity_type == EntityType.ADB.ToString())
                    {
                        objSplitterMobile.boxType = EntityType.ADB.ToString();
                        objSplitterMobile.adb_network_id = objSplitter.parent_network_id;
                    }
                    else if (objSplitter.parent_entity_type == EntityType.CDB.ToString())
                    {
                        objSplitterMobile.boxType = EntityType.CDB.ToString();
                        objSplitterMobile.cdb_network_id = objSplitter.parent_network_id;
                    }
                    response.status = StatusCodes.OK.ToString();
                    response.results = objSplitterMobile;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetSplitterDetail()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        public bool checkTemplateExist(string enType, int userid)
        {
            return new BLPoleItemMaster().ChkEntityTemplateExist(enType, userid, "");
        }
        public ApiResponse<saveSplitterOut> saveSplitter(ReqInput data)
        {
            var response = new ApiResponse<saveSplitterOut>();
            try
            {
                int pSystemId = 0;
                string pEntityType = "";
                string geom = "";
                SaveSplitterMobileIn objSplitterDetail = ReqHelper.GetRequestData<SaveSplitterMobileIn>(data);
                geom = objSplitterDetail.longitude + " " + objSplitterDetail.latitude;
                ADBMaster objADB = new ADBMaster();
                CDBMaster objCDB = new CDBMaster();
                SplitterMaster objSplitterMaster = new SplitterMaster();
                bool chkIstemplate = checkTemplateExist(EntityType.Splitter.ToString(), objSplitterDetail.userid);
                if (!chkIstemplate)
                {
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = "Splitter template is not filled,Please fill the template first!";
                    return response;
                }
                if (objSplitterDetail.system_id == 0 && objSplitterDetail.network_id_type == NetworkIdType.M.ToString() && IsNetworkIdExist(objSplitterDetail.network_id, EntityType.Splitter.ToString()))
                {
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = "Splitter Network id already exist!";
                    return response;
                }
                else if (objSplitterDetail.system_id == 0 && objSplitterDetail.boxType == EntityType.ADB.ToString() && objSplitterDetail.adb_network_id_type == NetworkIdType.M.ToString() && IsNetworkIdExist(objSplitterDetail.adb_network_id, EntityType.ADB.ToString()))
                {
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = "ADB Network id already exist!";
                    return response;
                }
                else if (objSplitterDetail.system_id == 0 && objSplitterDetail.boxType == EntityType.CDB.ToString() && objSplitterDetail.cdb_network_id_type == NetworkIdType.M.ToString() && IsNetworkIdExist(objSplitterDetail.cdb_network_id, EntityType.CDB.ToString()))
                {
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = "CDB Network id already exist!";
                    return response;
                }


                if (objSplitterDetail.system_id == 0 && objSplitterDetail.boxType == EntityType.ADB.ToString())
                {
                    bool chkIsADBtemplate = checkTemplateExist(EntityType.ADB.ToString(), objSplitterDetail.userid);
                    if (!chkIsADBtemplate)
                    {
                        response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                        response.error_message = "ADB template is not filled,Please fill the template first!";
                        return response;
                    }
                    objADB.geom = objSplitterDetail.longitude + " " + objSplitterDetail.latitude;
                    objADB.longitude = objSplitterDetail.longitude;
                    objADB.latitude = objSplitterDetail.latitude;
                    objADB.networkIdType = objSplitterDetail.adb_network_id_type;
                    objADB = SaveADB(objADB, objSplitterDetail.userid, objSplitterDetail.adb_network_id);
                    pSystemId = objADB.system_id;
                    pEntityType = EntityType.ADB.ToString();
                }
                else if (objSplitterDetail.system_id == 0 && objSplitterDetail.boxType == EntityType.CDB.ToString())
                {
                    bool chkIsCDBtemplate = checkTemplateExist(EntityType.CDB.ToString(), objSplitterDetail.userid);
                    if (!chkIsCDBtemplate)
                    {
                        response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                        response.error_message = "CDB template is not filled,Please fill the template first!";
                        return response;
                    }

                    objCDB.geom = objSplitterDetail.longitude + " " + objSplitterDetail.latitude;
                    objCDB.longitude = objSplitterDetail.longitude;
                    objCDB.latitude = objSplitterDetail.latitude;
                    objCDB.networkIdType = objSplitterDetail.cdb_network_id_type;
                    objCDB = SaveCDB(objCDB, objSplitterDetail.userid, objSplitterDetail.cdb_network_id);
                    pSystemId = objCDB.system_id;
                    pEntityType = EntityType.CDB.ToString();
                }

                if (string.IsNullOrEmpty(objSplitterDetail.splitter_ratio))
                {
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = "Splitter Ratio is required!";
                    return response;
                }

                //// get parent geometry 
                if (string.IsNullOrWhiteSpace(objSplitterDetail.longitude + " " + objSplitterDetail.latitude) && objSplitterDetail.system_id == 0)
                {
                    objSplitterDetail.geom = GetPointTypeParentGeom(pSystemId, pEntityType);
                }
                //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                objSplitterMaster = GetSplitter(pSystemId, pEntityType, objSplitterDetail.network_id_type, objSplitterDetail.userid, objSplitterDetail.system_id, objSplitterDetail.geom);
                if (objSplitterDetail.network_id_type == NetworkIdType.A.ToString() && objSplitterMaster.system_id == 0)
                {
                    //    //GET AUTO NETWORK CODE...
                    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Splitter.ToString(), gType = GeometryType.Point.ToString(), eGeom = geom, parent_eType = pEntityType, parent_sysId = pSystemId });
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    //objSplitterMaster.splitter_name = objNetworkCodeDetail.network_code;                    
                    //SET NETWORK CODE
                    objSplitterMaster.network_id = objNetworkCodeDetail.network_code;
                    objSplitterMaster.sequence_id = objNetworkCodeDetail.sequence_id;
                }
                else if (objSplitterDetail.network_id_type == NetworkIdType.M.ToString() && objSplitterMaster.system_id == 0)
                {
                    //SET NETWORK CODE
                    objSplitterMaster.network_id = objSplitterDetail.network_id;
                }

                objSplitterMaster.splitter_type = objSplitterDetail.splitter_type;
                objSplitterMaster.splitter_name = objSplitterDetail.splitter_name;
                objSplitterMaster.splitter_ratio = objSplitterDetail.splitter_ratio;
                objSplitterMaster.latitude = objSplitterDetail.latitude;
                objSplitterMaster.longitude = objSplitterDetail.longitude;
                objSplitterMaster.address = objSplitterDetail.address;
                var isNew = objSplitterMaster.system_id > 0 ? false : true;
                var resultItem = new BLSplitter().SaveSplitterEntity(objSplitterMaster, Convert.ToInt32(objSplitterDetail.userid));
                if (isNew)
                {
                    saveSplitterOut objOut = new saveSplitterOut();
                    objOut.System_id = resultItem.system_id;
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = "Splitter saved successfully.";
                    response.results = objOut;
                }
                else
                {
                    saveSplitterOut objOut = new saveSplitterOut();
                    objOut.System_id = resultItem.system_id;
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = "Splitter updated successfully.";
                    response.results = objOut;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("saveSplitter()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        public SplitterMaster GetSplitter(int pSystemId, string pEntityType, string networkIdType, int userid, int systemId = 0, string geom = "")
        {
            SplitterMaster objSplitter = new SplitterMaster();
            objSplitter.geom = geom;
            objSplitter.networkIdType = networkIdType;
            if (systemId == 0)
            {
                objSplitter.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objSplitter.latitude = Convert.ToDouble(geom.Split(' ')[1]);

                //NEW ENTITY->Fill Region and Province Detail..
                fillSplRegionProvinceDetail(objSplitter, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillSplParentDetail(objSplitter, new NetworkCodeIn() { eType = EntityType.Splitter.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSplitter.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType);
                //Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<SplitterTemplateMaster>(userid, EntityType.Splitter);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objSplitter);
            }
            else
            {
                objSplitter = new BLMisc().GetEntityDetailById<SplitterMaster>(systemId, EntityType.Splitter);
            }
            return objSplitter;
        }
        private void fillSplRegionProvinceDetail(SplitterMaster objEntityModel, string enType, string geom)
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
        }
        private void fillSplParentDetail(SplitterMaster objLib, NetworkCodeIn objIn, string networkIdType)
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
        public bool IsNetworkIdExist(string networkId, string entityType, string networkStage = "P")
        {

            var IsNetworkIDExist = new BLMisc().chkNetworkIdExist(networkId, entityType, networkStage);
            return IsNetworkIDExist;
        }
        public ApiResponse<SplitterMobileTemplateOut> SaveSplitterTemplate(ReqInput data)
        {
            var response = new ApiResponse<SplitterMobileTemplateOut>();
            try
            {

                SplitterMobileTemplateOut objSplitter = new SplitterMobileTemplateOut();
                SplitterItemMaster objSplitterItem = ReqHelper.GetRequestData<SplitterItemMaster>(data);
                objSplitterItem.splitter_ratio = objSplitterItem.unitValue;
                var itemid = objSplitterItem.id;
                if (string.IsNullOrEmpty(objSplitterItem.splitter_ratio))
                {
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = "Splitter Ratio is required!";
                    return response;
                }
                var resultItem = new BLSplitterItemMaster().SaveSplitterItemTemplate(objSplitterItem, objSplitterItem.created_by);

                if (itemid > 0)  // Update 
                {
                    objSplitter.id = resultItem.id;
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = "Splitter template update successfully.";
                    response.results = objSplitter;
                }
                else
                {
                    objSplitter.id = resultItem.id;
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = "Splitter template saved successfully.";
                    response.results = objSplitter;
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveSplitterTemplate()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        #endregion

        #region ADB
        public ADBMaster GetADBDetail(int pSystemId, string pEntityType, string networkIdType, int systemId, int userid, string geom = "")
        {
            ADBMaster objADB = new ADBMaster();
            objADB.geom = geom;
            objADB.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillAdbRegionProvinceDetail(objADB, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillAdbParentDetail(objADB, new NetworkCodeIn() { eType = EntityType.ADB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objADB.geom }, networkIdType);
                objADB.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objADB.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<ADBTemplateMaster>(userid, EntityType.ADB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objADB);
            }
            else
            {
                // Get entity detail by Id...
                objADB = new BLMisc().GetEntityDetailById<ADBMaster>(systemId, EntityType.ADB);
            }
            return objADB;
        }
        private void fillAdbRegionProvinceDetail(ADBMaster objEntityModel, string enType, string geom)
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
        }
        private void fillAdbParentDetail(ADBMaster objLib, NetworkCodeIn objIn, string networkIdType)
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
        public ADBMaster SaveADB(ADBMaster objADBMaster, int userid, string networkId)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            // get parent geometry 
            if (string.IsNullOrWhiteSpace(objADBMaster.geom) && objADBMaster.system_id == 0)
            {
                objADBMaster.geom = GetPointTypeParentGeom(objADBMaster.pSystemId, objADBMaster.pEntityType);
            }

            //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            objADBMaster = GetADBDetail(objADBMaster.pSystemId, objADBMaster.pEntityType, objADBMaster.networkIdType, objADBMaster.system_id, userid, objADBMaster.geom);
            if (objADBMaster.networkIdType == NetworkIdType.A.ToString() && objADBMaster.system_id == 0)
            {
                //GET AUTO NETWORK CODE...
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ADB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objADBMaster.geom });
                // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                objADBMaster.adb_name = objNetworkCodeDetail.network_code;

                //SET NETWORK CODE
                objADBMaster.network_id = objNetworkCodeDetail.network_code;
                objADBMaster.sequence_id = objNetworkCodeDetail.sequence_id;
                // }
            }
            else if (objADBMaster.networkIdType == NetworkIdType.M.ToString() && objADBMaster.system_id == 0)
            {
                objADBMaster.adb_name = networkId;
                objADBMaster.network_id = networkId;
            }

            var isNew = objADBMaster.system_id > 0 ? false : true;
            if (objADBMaster.unitValue != null && objADBMaster.unitValue.Contains(":"))
            {
                objADBMaster.no_of_input_port = Convert.ToInt32(objADBMaster.unitValue.Split(':')[0]);
                objADBMaster.no_of_output_port = Convert.ToInt32(objADBMaster.unitValue.Split(':')[1]);
            }
            var resultItem = new BLADB().SaveEntityADB(objADBMaster, Convert.ToInt32(userid));
            if (isNew)
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.isNewEntity = isNew;
                objPM.message = "ADB saved successfully.";
            }
            else
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = "ADB updated successfully.";
            }
            resultItem.objPM = objPM;
            return resultItem;
        }
        public ApiResponse<ADBMobileTemplateOut> SaveADBTemplate(ReqInput data)
        {
            var response = new ApiResponse<ADBMobileTemplateOut>();
            try
            {
                ADBMobileTemplateOut objResponse = new ADBMobileTemplateOut();
                ADBItemMaster objADBItem = ReqHelper.GetRequestData<ADBItemMaster>(data);

                var itemid = objADBItem.id;
                if (objADBItem.unitValue != null && objADBItem.unitValue.Contains(":"))
                {
                    objADBItem.no_of_input_port = Convert.ToInt32(objADBItem.unitValue.Split(':')[0]);
                    objADBItem.no_of_output_port = Convert.ToInt32(objADBItem.unitValue.Split(':')[1]);
                }
                var resultItem = new BLADBItemMaster().SaveADBItemTemplate(objADBItem, objADBItem.created_by);

                if (itemid > 0)  // Update 
                {
                    objResponse.id = resultItem.id;
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = "ADB template update successfully.";
                    response.results = objResponse;
                }
                else
                {
                    objResponse.id = resultItem.id;
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = "ADB template saved successfully.";
                    response.results = objResponse;
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveADBTemplate()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        #endregion

        #region CDB
        public CDBMaster GetCDBDetail(int pSystemId, string pEntityType, string networkIdType, int systemId, int userid, string geom = "")
        {
            CDBMaster objCDB = new CDBMaster();
            objCDB.geom = geom;
            objCDB.networkIdType = networkIdType;
            if (systemId == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillCdbRegionProvinceDetail(objCDB, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                fillCdbParentDetail(objCDB, new NetworkCodeIn() { eType = EntityType.CDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCDB.geom }, networkIdType);
                objCDB.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objCDB.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<CDBTemplateMaster>(userid, EntityType.CDB);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objCDB);
            }
            else
            {
                // Get entity detail by Id...
                objCDB = new BLMisc().GetEntityDetailById<CDBMaster>(systemId, EntityType.CDB);
            }
            return objCDB;
        }
        private void fillCdbRegionProvinceDetail(CDBMaster objEntityModel, string enType, string geom)
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
        }
        private void fillCdbParentDetail(CDBMaster objLib, NetworkCodeIn objIn, string networkIdType)
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
        public CDBMaster SaveCDB(CDBMaster objCDBMaster, int userid, string networkId)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            //if (objCDBMaster.networkIdType == NetworkIdType.A.ToString() && objCDBMaster.system_id == 0)
            //{
            // get parent geometry 
            if (string.IsNullOrWhiteSpace(objCDBMaster.geom) && objCDBMaster.system_id == 0)
            {
                objCDBMaster.geom = GetPointTypeParentGeom(objCDBMaster.pSystemId, objCDBMaster.pEntityType);
            }
            //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
            objCDBMaster = GetCDBDetail(objCDBMaster.pSystemId, objCDBMaster.pEntityType, objCDBMaster.networkIdType, objCDBMaster.system_id, userid, objCDBMaster.geom);
            if (objCDBMaster.networkIdType == NetworkIdType.A.ToString() && objCDBMaster.system_id == 0)
            {
                //GET AUTO NETWORK CODE...
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.CDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCDBMaster.geom });

                // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                objCDBMaster.cdb_name = objNetworkCodeDetail.network_code;
                //SET NETWORK CODE
                objCDBMaster.network_id = objNetworkCodeDetail.network_code;
                objCDBMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            }
            else if (objCDBMaster.networkIdType == NetworkIdType.M.ToString() && objCDBMaster.system_id == 0)
            {
                objCDBMaster.cdb_name = networkId;
                objCDBMaster.network_id = networkId;
            }

            // }
            //CDBTemplateMaster objItem = new CDBTemplateMaster();

            //if (TryValidateModel(objCDBMaster))
            //{
            var isNew = objCDBMaster.system_id > 0 ? false : true;
            if (objCDBMaster.unitValue != null && objCDBMaster.unitValue.Contains(":"))
            {
                objCDBMaster.no_of_input_port = Convert.ToInt32(objCDBMaster.unitValue.Split(':')[0]);
                objCDBMaster.no_of_output_port = Convert.ToInt32(objCDBMaster.unitValue.Split(':')[1]);
            }
            var resultItem = new BLCDB().SaveEntityCDB(objCDBMaster, userid);

            if (isNew)
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.isNewEntity = isNew;
                objPM.message = "CDB saved successfully.";
            }
            else
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = "CDB updated successfully.";
            }
            resultItem.objPM = objPM;
            return resultItem;
            // }

        }
        public ApiResponse<CDBMobileTemplateOut> SaveCDBTemplate(ReqInput data)
        {
            var response = new ApiResponse<CDBMobileTemplateOut>();
            try
            {
                CDBMobileTemplateOut objResponse = new CDBMobileTemplateOut();
                CDBItemMaster objCDBItem = ReqHelper.GetRequestData<CDBItemMaster>(data);

                var itemid = objCDBItem.id;
                if (objCDBItem.unitValue != null && objCDBItem.unitValue.Contains(":"))
                {
                    objCDBItem.no_of_input_port = Convert.ToInt32(objCDBItem.unitValue.Split(':')[0]);
                    objCDBItem.no_of_output_port = Convert.ToInt32(objCDBItem.unitValue.Split(':')[1]);
                }
                var resultItem = new BLCDBItemMaster().SaveCDBItemTemplate(objCDBItem, objCDBItem.created_by);

                if (itemid > 0)  // Update 
                {
                    objResponse.id = resultItem.id;
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = "CDB template update successfully.";
                    response.results = objResponse;
                }
                else
                {
                    objResponse.id = resultItem.id;
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = "CDB template  saved successfully.";
                    response.results = objResponse;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveCDBTemplate()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        [System.Web.Http.HttpPost]
        public ApiResponse<List<EntityDropDownItemList>> GetItemSpecifications(ReqInput data)
        {
            var response = new ApiResponse<List<EntityDropDownItemList>>();
            try
            {
                ItemDropDownIn objItemDropDownIn = ReqHelper.GetRequestData<ItemDropDownIn>(data);
                if (string.IsNullOrEmpty(objItemDropDownIn.entityType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Entity type is required!";
                    return response;
                }
                var lstSpecifications = BLItemTemplate.Instance.GetItemSpecification(objItemDropDownIn.entityType, 0, 0, "").Where(x => x.ddtype == DropDownType.Specification.ToString()).ToList();

                if (lstSpecifications.Count > 0)
                {

                    response.results = lstSpecifications.Select(m => (new EntityDropDownItemList { key = m.key, value = m.value })).ToList();
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                    response.error_message = "No Specification found!";
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetItemSpecifications()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<List<EntityDropDownItemList>> GetItemVendors(ReqInput data)
        {
            var response = new ApiResponse<List<EntityDropDownItemList>>();
            try
            {
                VendorDropDownIn objVendorDropDownIn = ReqHelper.GetRequestData<VendorDropDownIn>(data);
                if (string.IsNullOrEmpty(objVendorDropDownIn.specification))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Specification is required!";
                    return response;
                }
                var lstVendors = BLItemTemplate.Instance.GetVendorList(objVendorDropDownIn.specification);
                if (lstVendors.Count > 0)
                {
                    response.results = lstVendors.Select(m => (new EntityDropDownItemList { key = m.key, value = m.value })).ToList();
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                    response.error_message = "No vendor found!";
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetItemVendors()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<ItemCategoryDetailOut> GetItemCategoryDetail(ReqInput data)
        {
            var response = new ApiResponse<ItemCategoryDetailOut>();
            try
            {
                ItemCategoryDetailOut objItemCategoryout = new ItemCategoryDetailOut();
                ItemCategoryIn objItemCategoryIn = ReqHelper.GetRequestData<ItemCategoryIn>(data);
                if (string.IsNullOrEmpty(objItemCategoryIn.entitytype))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "EntityType is required!";
                    return response;
                }
                else if (string.IsNullOrEmpty(objItemCategoryIn.specification))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Specification is required!";
                    return response;
                }
                else if (objItemCategoryIn.vendorId == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "VendorId is required!";
                    return response;
                }

                objItemCategoryout = BLItemTemplate.Instance.GetCatSubCatData(objItemCategoryIn.entitytype, objItemCategoryIn.specification, objItemCategoryIn.vendorId)
                .Select(m => new ItemCategoryDetailOut
                {
                    category = m.category,
                    subcategory1 = m.subCategory_1,
                    subcategory2 = m.subCategory_2,
                    subcategory3 = m.subCategory_3,
                    item_code = m.code,
                    no_of_input_port = m.no_of_input_port,
                    no_of_output_port = m.no_of_output_port,
                    no_of_port = m.no_of_port,
                    unit = m.unit,
                    other = m.other,
                    no_of_tube = m.no_of_tube,
                    no_of_core_per_tube = m.no_of_core_per_tube
                }).FirstOrDefault();
                if (objItemCategoryout != null)
                {

                    response.results = objItemCategoryout;
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "No category detail found!";
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetItemCategoryDetail()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        public ApiResponse<string> checkEntityTemplateIsExist(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                EntityTemplateIsExistIn objImpactDetailIn = ReqHelper.GetRequestData<EntityTemplateIsExistIn>(data);
                if (string.IsNullOrEmpty(objImpactDetailIn.entityType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = " Entity Type is required!";
                    return response;
                }
                else if (objImpactDetailIn.userId == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = " User Id is required!";
                    return response;
                }
                bool chkIsTemplate = checkTemplateExist(objImpactDetailIn.entityType.ToString(), objImpactDetailIn.userId);
                if (!chkIsTemplate)
                {
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                    response.error_message = objImpactDetailIn.entityType + " template is not filled,Please fill the template first!";
                }
                else { response.status = StatusCodes.OK.ToString(); }
                return response;
            }
            catch { throw; }

        }

        #region customer
        [System.Web.Http.HttpPost]
        public ApiResponse<Customer> saveCustomerInfo(ReqInput data)
        {
            var response = new ApiResponse<Customer>();
            try
            {

                SaveCustomerIn objSaveCustomerIn = ReqHelper.GetRequestData<SaveCustomerIn>(data);
                Customer objCustomer = new Customer();
                var resultItem = new BLCustomer().SaveCustomerInfo(objSaveCustomerIn.system_id, objSaveCustomerIn.can_id, objSaveCustomerIn.customer_name, objSaveCustomerIn.address, objSaveCustomerIn.building_Code, objSaveCustomerIn.building_rfs_type, objSaveCustomerIn.floor_id, objSaveCustomerIn.latitude, objSaveCustomerIn.longitude, objSaveCustomerIn.structure_id, objSaveCustomerIn.userid);

                if (resultItem.status)
                {
                    var item = new BLCustomer().UpdateTicketStatus(objSaveCustomerIn.ticket_id, objSaveCustomerIn.reference_type, objSaveCustomerIn.step_id, objSaveCustomerIn.address);
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);
                }
                else
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("saveCustomerInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing Request.";
            }
            return response;
        }


        [System.Web.Http.HttpPost]
        public ApiResponse<CustomerInfo> getCustomerInfo(ReqInput data)
        {
            var response = new ApiResponse<CustomerInfo>();
            getCustomerIn objgetCustomerIn = ReqHelper.GetRequestData<getCustomerIn>(data);
            Customer objCustomer = new Customer();
            CustomerInfo objCustomerInfo = new CustomerInfo();
            try
            {
                if (string.IsNullOrEmpty(objgetCustomerIn.can_id))
                {
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = "Can Id is required!";
                    return response;
                }

                if (objgetCustomerIn.userId == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid user id!";
                    return response;
                }
                objCustomerInfo = new BLCustomerInfo().GetCustomerInfoByCanId(objgetCustomerIn.can_id, EntityType.Customer.ToString(), objgetCustomerIn.ticket_id);
                if (objCustomerInfo == null)
                {
                    response.status = ResponseStatus.OK.ToString();
                    response.results = objCustomerInfo;
                    response.error_message = "Customer doesn't exists in GIS System";
                }
                else
                {
                    //var ispEntityMapping = BLIspEntityMapping.Instance.GetStructureFloorbyEntityId(objCustomerInfo.system_id, EntityType.Customer.ToString()); 
                    // objCustomerInfo.structure_id = ispEntityMapping.structure_id;
                    // objCustomerInfo.floor_id = ispEntityMapping.floor_id != null ? Convert.ToInt32(ispEntityMapping.floor_id) : 0;
                    response.status = StatusCodes.OK.ToString();
                    response.results = objCustomerInfo;
                    response.results.entityType = EntityType.Customer.ToString();
                }


            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getCustomerInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }


        private void fillRegionProvinceDetailCustomer(Customer objEntityModel, string enType, string geom)
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
        }


        private void fillParentDetail(Customer objCustomerMobileMaster, NetworkCodeIn objIn, string networkIdType)
        {
            //fill parent detail....
            var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
            if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    //FILL NETWORK CODE FORMAT FOR MANUAL
                    objCustomerMobileMaster.network_id = networkCodeDetail.network_code;
                }
                objCustomerMobileMaster.parent_entity_type = networkCodeDetail.parent_entity_type;
                objCustomerMobileMaster.parent_network_id = networkCodeDetail.parent_network_id;
                objCustomerMobileMaster.parent_system_id = networkCodeDetail.parent_system_id;
            }
        }

        #endregion

        #region Splitter Customer Allocation

        //[HttpPost]
        //public ApiResponse<List<EntityDetail>> GetNearEntitiesByEntityName(ReqInput data)
        //{
        //    var response = new ApiResponse<List<EntityDetail>>();
        //    try
        //    {

        //        NearByEntitiesIn objEntityTemplateIn = ReqHelper.GetRequestData<NearByEntitiesIn>(data);
        //        if (objEntityTemplateIn.bufferInMtrs <= 0)
        //        {
        //            response.status = StatusCodes.VALIDATION_FAILED.ToString();
        //            response.error_message = "Invalid buffer!";
        //            return response;
        //        }
        //        else if (objEntityTemplateIn.latitude == 0)
        //        {
        //            response.status = StatusCodes.VALIDATION_FAILED.ToString();
        //            response.error_message = "Invalid latitude!";
        //            return response;
        //        }
        //        else if (objEntityTemplateIn.longitude == 0)
        //        {
        //            response.status = StatusCodes.VALIDATION_FAILED.ToString();
        //            response.error_message = "Invalid longitude!";
        //            return response;
        //        }

        //        var result = new BLMisc().GetNearEntitiesByEntityName(objEntityTemplateIn.latitude, objEntityTemplateIn.longitude, objEntityTemplateIn.bufferInMtrs, objEntityTemplateIn.entity_name);

        //        if (result != null && result.Count > 0)
        //        {
        //        response.status = StatusCodes.OK.ToString();
        //        response.results = result;
        //        }
        //        else
        //        {
        //            response.status = StatusCodes.ZERO_RESULTS.ToString();
        //            response.results = result;
        //            response.error_message = "No Record Found";

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogHelper logHelper = new ErrorLogHelper();
        //        logHelper.ApiLogWriter("GetNearEntitiesByEntityName()", "Main Controller", data.data, ex);
        //        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
        //        response.error_message = "Error While Processing  Request.";
        //    }

        //    return response;
        //}

        [System.Web.Http.HttpPost]
        public ApiResponse<List<IspPortInfo>> GetPortInfo(ReqInput data)
        {
            var response = new ApiResponse<List<IspPortInfo>>();
            try
            {

                EntityInfoIn objEntityTemplateIn = ReqHelper.GetRequestData<EntityInfoIn>(data);
                if (objEntityTemplateIn.systemId <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid system id!";
                    return response;
                }
                if (String.IsNullOrEmpty(objEntityTemplateIn.entityType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid entity name!";
                    return response;
                }

                var result = new BLMisc().GetPortInfo(objEntityTemplateIn.systemId, objEntityTemplateIn.entityType);

                if (result != null && result.Count > 0)
                {
                    response.status = StatusCodes.OK.ToString();
                    response.results = result;
                }

                else
                {
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                    response.results = result;
                    response.error_message = "No Record Found";
                }



            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetPortInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }


        [System.Web.Http.HttpPost]
        public ApiResponse<string> saveSplitterAllocationPortInfo(ReqInput data)

        {
            var response = new ApiResponse<string>();
            try
            {
                List<IspPortInfo> objGetEntity = ReqHelper.GetRequestData<List<IspPortInfo>>(data);

                int outputresult = new BLMisc().UpdateSplitterAllocationPort(objGetEntity);

                if (outputresult > 0)
                {
                    response.status = ResponseStatus.OK.ToString();
                    response.error_message = "Port Info updated successfully.";

                }
                else
                {

                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = "Port Info not updated successfully.";

                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("saveSplitterAllocationPortInfo()", "Main Controller", data.data, ex);

                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }



        [System.Web.Http.HttpPost]
        public ApiResponse<List<EntityDetail>> GetNearEntitiesByEntityType(ReqInput data)
        {
            var response = new ApiResponse<List<EntityDetail>>();
            try
            {

                NearByEntitiesByType objEntityTemplateIn = ReqHelper.GetRequestData<NearByEntitiesByType>(data);
                if (objEntityTemplateIn.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }

                var result = new BLMisc().GetNearEntitiesByEntityType(objEntityTemplateIn.c_system_id, objEntityTemplateIn.c_entity_type, objEntityTemplateIn.bufferInMtrs, objEntityTemplateIn.search_entity_type);

                if (result != null && result.Count > 0)
                {
                    response.status = StatusCodes.OK.ToString();
                    response.results = result;
                }
                else
                {
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                    response.results = result;
                    response.error_message = "No Record Found";

                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearEntitiesByEntityType()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

        #endregion

        #region GET BUILDING INFO WITH STRUCTURE INFO
        [System.Web.Http.HttpPost]
        public ApiResponse<BuildingMaster> getBuildingDetail(ReqInput data)
        {
            var response = new ApiResponse<BuildingMaster>();
            BuildingDetailIn objBuildingDetailIn = ReqHelper.GetRequestData<BuildingDetailIn>(data);

            try
            {
                if (string.IsNullOrWhiteSpace(objBuildingDetailIn.network_id))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Building Code can not be null or empty!";
                    return response;
                }

                BuildingMaster objBuildingMaster = new BuildingMaster();
                objBuildingMaster = BLBuilding.Instance.GetBuildingByCode(objBuildingDetailIn.network_id);
                if (objBuildingMaster != null)
                {
                    objBuildingMaster.lstStructureDetails = BLStructure.Instance.GetStructureByBld(objBuildingMaster.system_id);
                    response.results = objBuildingMaster;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
                else
                {
                    response.results = objBuildingMaster;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "Building Code doesn't exists in GIS System";
                }
                // objBuildingMaster.lstStructureDetails = BLStructure.Instance.GetStructureByBld(objBuildingMaster.system_id);
                //response.results = objBuildingMaster;
                // response.status = StatusCodes.OK.ToString();
                // response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getBuildingDetail()", "MainController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        [System.Web.Http.HttpPost]
        public ApiResponse<BuildingGeomInfo> getBuildingGeom(ReqInput data)
        {
            var response = new ApiResponse<BuildingGeomInfo>();
            BuildingIn objBuildingIn = ReqHelper.GetRequestData<BuildingIn>(data);
            BuildingGeomInfo objBuildingGeomInfo = new BuildingGeomInfo();

            try
            {

                if (string.IsNullOrWhiteSpace(objBuildingIn.building_code))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = " Building code can't be null or empty!";
                    return response;
                }
                var result = BLBuilding.Instance.GetBuildingGeomInfo(objBuildingIn.building_code, objBuildingIn.system_id);
                if (result != null)
                {
                    objBuildingGeomInfo.lstBuildingGeom = result;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                    response.results = objBuildingGeomInfo;
                }
                else
                {
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                    response.error_message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getBuildingGeom()", "MainController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region GET STRUCTURE INFO WITH SHAFT/FLOOR AND ELEMENTS
        [System.Web.Http.HttpPost]
        public ApiResponse<StructureMaster> getStructureInfo(ReqInput data)
        {
            var response = new ApiResponse<StructureMaster>();
            StructureDetailIn objStructureDetailIn = ReqHelper.GetRequestData<StructureDetailIn>(data);

            try
            {

                //if (string.IsNullOrWhiteSpace(objStructureDetailIn.associated_entity_types))
                //{
                //	response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //	response.error_message = " Associated entity types can't be null or empty!";
                //	return response;
                //}
                StructureMaster objStructureMaster = new StructureMaster();
                objStructureMaster = BLISP.Instance.getSructureDetailsByCode(objStructureDetailIn.system_id);
                objStructureMaster.lstShaftInfo = BLShaft.Instance.GetShaftByBld(objStructureDetailIn.system_id);
                objStructureMaster.lstFloorInfo = BLFloor.Instance.GetFloorByBld(objStructureDetailIn.system_id);
                if (objStructureMaster.lstFloorInfo.Count > 0)
                {
                    objStructureMaster.lstFloorInfo = objStructureMaster.lstFloorInfo.OrderByDescending(m => m.system_id).ToList();
                }
                objStructureMaster.lstStructureElements = new BLMisc().GetStructureElementInfo(objStructureDetailIn.system_id, objStructureDetailIn.associated_entity_types, objStructureDetailIn.can_id, objStructureDetailIn.isOnly_servingdb, objStructureDetailIn.rfs_type, objStructureDetailIn.module_abbr);
                response.results = objStructureMaster;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getStructureInfo()", "MainController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #region Get Mobile legend info

        [System.Web.Http.HttpPost]
        public ApiResponse<List<LegendDetail>> getMobileLegendInfo(ReqInput data)
        {
            var response = new ApiResponse<List<LegendDetail>>();
            LegendDetailIn objLegendDetailIn = ReqHelper.GetRequestData<LegendDetailIn>(data);
            Legend objLegendDetailVeiwModel = new Legend();
            try
            {
                if (string.IsNullOrWhiteSpace(objLegendDetailIn.group_name))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Group name can't be null or empty!";
                    return response;
                }

                List<LegendDetail> lstLegendDetail = new BLMisc().GetMobileLegendDetail(objLegendDetailIn.userid, objLegendDetailIn.group_name);
                if (lstLegendDetail == null)
                {
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                    response.error_message = "No records found!";
                }
                else
                {
                    response.results = lstLegendDetail;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getMobileLegendInfo()", "MainController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region GetDBInfo

        [System.Web.Http.HttpPost]
        public ApiResponse<DistributionBoxInfo> getDistributionBoxInfo(ReqInput data)
        {
            var response = new ApiResponse<DistributionBoxInfo>();
            try
            {
                DistributionBoxInfoIn objEntityTemplateIn = ReqHelper.GetRequestData<DistributionBoxInfoIn>(data);
                if (string.IsNullOrEmpty(objEntityTemplateIn.latitude.ToString()) || string.IsNullOrEmpty(objEntityTemplateIn.longitude.ToString()))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "latitude/longitude can not be null or empty!";
                    return response;
                }
                var result = new BLMisc().GetDistributionBoxInfo(objEntityTemplateIn.userid, objEntityTemplateIn.latitude, objEntityTemplateIn.longitude, objEntityTemplateIn.boxtype);

                if (result != null)
                {
                    response.status = StatusCodes.OK.ToString();
                    response.results = result;
                }
                else
                {
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                    response.results = result;
                    response.error_message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetDistributionBoxInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<DistributionBoxEntityInfo> getDistributionBoxEntityInfo(ReqInput data)
        {
            var response = new ApiResponse<DistributionBoxEntityInfo>();
            try
            {

                DistributionBoxEntityInfoIn objDBEntityInfoIn = ReqHelper.GetRequestData<DistributionBoxEntityInfoIn>(data);
                if (objDBEntityInfoIn.system_id <= 0 || string.IsNullOrEmpty(objDBEntityInfoIn.entitytype.ToString()))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid systemid or entity type!";
                    return response;
                }

                var result = new BLMisc().GetDistributionBoxEntityInfo(objDBEntityInfoIn.userid, objDBEntityInfoIn.system_id, objDBEntityInfoIn.entitytype);
                response.status = StatusCodes.OK.ToString();
                response.results = result;

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetDistributionBoxEntityInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<SplitterPortInfo> getSplitterPortInfo(ReqInput data)
        {
            var response = new ApiResponse<SplitterPortInfo>();
            try
            {

                SplitterPortInfoIn objSplitterPortInfoIn = ReqHelper.GetRequestData<SplitterPortInfoIn>(data);
                if (objSplitterPortInfoIn.system_id <= 0 || string.IsNullOrEmpty(objSplitterPortInfoIn.entitytype.ToString()))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid systemid or entity type!";
                    return response;
                }

                var result = new BLMisc().GetSplitterPortInfo(objSplitterPortInfoIn.userid, objSplitterPortInfoIn.system_id, objSplitterPortInfoIn.entitytype, objSplitterPortInfoIn.can_id);
                response.status = StatusCodes.OK.ToString();
                response.results = result;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetSplitterPortInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion

        [System.Web.Http.HttpPost]
        public ApiResponse<saveCustomerAssociationOut> SaveCustomerAssociation(ReqInput data)
        {
            var response = new ApiResponse<saveCustomerAssociationOut>();
            saveEntityAssociation objsaveEntityAssociation = new saveEntityAssociation();
            try
            {
                SaveCustomerAssociationIn objSaveCustomerAssociationIn = ReqHelper.GetRequestData<SaveCustomerAssociationIn>(data);
                saveCustomerAssociationOut objsave = new saveCustomerAssociationOut();
                List<WCRMaterialIN> lstWCRMaterial = new List<WCRMaterialIN>();
                ONTMaster objONTMaster = new ONTMaster();
                if (string.IsNullOrWhiteSpace(objSaveCustomerAssociationIn.building_rfs_type))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Building RFS Type can not be null or empty!";
                    return response;
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var jsonlstWCRMatrial = jsonSerialiser.Serialize(objSaveCustomerAssociationIn.lstWCRMaterial);
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.WriteDebugLog("Line-1");
                /*********** save/update splitter entity mapping**********************/
                //var result = new BLMisc().SaveCustomerAssociation(objSaveCustomerAssociationIn.source_system_id, objSaveCustomerAssociationIn.source_port_number, objSaveCustomerAssociationIn.source_entity_type, objSaveCustomerAssociationIn.destination_system_id, objSaveCustomerAssociationIn.user_id, objSaveCustomerAssociationIn.destination_network_id, objSaveCustomerAssociationIn.destination_entity_type,objSaveCustomerAssociationIn.building_rfs_type,jsonlstWCRMatrial);

                var result = new BLMisc().SaveCustomerAssociation(objSaveCustomerAssociationIn.destination_network_id, objSaveCustomerAssociationIn.p_box_id, objSaveCustomerAssociationIn.p_box_type, objSaveCustomerAssociationIn.source_system_id, objSaveCustomerAssociationIn.source_port_number, objSaveCustomerAssociationIn.user_id, jsonlstWCRMatrial, objSaveCustomerAssociationIn.building_rfs_type, objSaveCustomerAssociationIn.is_box_changed, objSaveCustomerAssociationIn.ticket_id, objSaveCustomerAssociationIn.routeGeom);
                if (result.status)
                {
                    new Thread(() =>
                    {
                        ConnectionInfoMaster objConection = new ConnectionInfoMaster();
                        objConection.source_system_id = objSaveCustomerAssociationIn.source_system_id;
                        objConection.source_entity_type = EntityType.Splitter.ToString();
                        objConection.created_by = objSaveCustomerAssociationIn.user_id;
                        new BLOSPSplicing().SaveUtilizationNotification(objConection);
                    }).Start();

                    // objsaveEntityAssociation.IspPortInfo = new BLMisc().SaveSplitterCustomerMapping(objSaveCustomerAssociationIn.source_system_id, objSaveCustomerAssociationIn.source_port_number, objSaveCustomerAssociationIn.source_entity_type, objSaveCustomerAssociationIn.destination_system_id, objSaveCustomerAssociationIn.user_id, objSaveCustomerAssociationIn.destination_network_id, objSaveCustomerAssociationIn.destination_entity_type);
                    var item = new BLCustomer().UpdateTicketMaster(objSaveCustomerAssociationIn.ticket_id, objSaveCustomerAssociationIn.reference_type, objSaveCustomerAssociationIn.step_id, objSaveCustomerAssociationIn.user_id);
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = result.message;
                    objsave.customer_system_id = objSaveCustomerAssociationIn.destination_system_id;
                    response.results = objsave;
                    logHelper.WriteDebugLog("Line-2");
                    return response;
                }
                else
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = result.message;
                    return response;
                }

                /*******END SAVE CABLE*************/
            }

            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("saveCustomerAssociation()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        private CableMaster GetCableInfo(LineEntityIn objIn)
        {
            CableMaster objCbl = new CableMaster();
            SaveCustomerAssociationIn objusr = new global::SaveCustomerAssociationIn();
            if (objIn.systemId == 0)
            {
                objCbl.cable_type = objIn.cableType;
                objCbl.networkIdType = objIn.networkIdType;

                var objItem = BLItemTemplate.Instance.GetTemplateDetail<CableTemplateMaster>(objusr.user_id, EntityType.Cable, objIn.cableType);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objCbl);
            }
            else
            {
                objCbl = new BLMisc().GetEntityDetailById<CableMaster>(objIn.systemId, EntityType.Cable);

            }

            return objCbl;
        }
        public Models.ISP.HTBInfo SaveHTB(SaveCustomerAssociationIn objSaveCustomerAssociationIn, WCRMaterialIN item)
        {
            Models.ISP.HTBInfo objHTBInfo = new Models.ISP.HTBInfo();
            try
            {
                objHTBInfo.geom = GetPointTypeParentGeom(objSaveCustomerAssociationIn.destination_system_id, EntityType.Customer.ToString());
                /**B-RFS**/
                if (objSaveCustomerAssociationIn.building_rfs_type == RFSTypes.BRFS.EnumValue())
                {
                    var objCustomer = BLIspEntityMapping.Instance.GetIspEntityMapByCustomerId(objSaveCustomerAssociationIn.destination_system_id, objSaveCustomerAssociationIn.destination_entity_type);
                    var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objCustomer.structure_id, EntityType.Structure);
                    objHTBInfo.floor_id = objCustomer.floor_id.GetValueOrDefault();
                    objHTBInfo.structure_id = objCustomer.structure_id;
                    objHTBInfo.shaft_id = objCustomer.shaft_id.GetValueOrDefault();
                    objHTBInfo.parent_entity_type = EntityType.Structure.ToString();
                    if (structureDetails != null)
                    {
                        objHTBInfo.region_id = structureDetails.region_id;
                        objHTBInfo.province_id = structureDetails.province_id;
                        objHTBInfo.latitude = structureDetails.latitude;
                        objHTBInfo.longitude = structureDetails.longitude;
                        objHTBInfo.parent_system_id = structureDetails.system_id;
                    }
                    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objCustomer.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.HTB.ToString(), structureId = objCustomer.structure_id });
                    objHTBInfo.htb_name = objISPNetworkCode.network_code;
                    objHTBInfo.network_id = objISPNetworkCode.network_code;
                    objHTBInfo.sequence_id = objISPNetworkCode.sequence_id;
                }
                else
                {
                    /** A-C-RFS**/
                    var objISPNetworkCode = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHTBInfo.geom, parent_eType = "", parent_sysId = 0 });
                    // var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHTBInfo.geom, parent_eType = "", parent_sysId = 0 });
                    objHTBInfo.htb_name = objISPNetworkCode.network_code;
                    objHTBInfo.network_id = objISPNetworkCode.network_code;
                    objHTBInfo.sequence_id = objISPNetworkCode.sequence_id;
                    objHTBInfo.parent_entity_type = objISPNetworkCode.parent_entity_type;
                    objHTBInfo.parent_system_id = objISPNetworkCode.parent_system_id;
                    objHTBInfo.parent_network_id = objISPNetworkCode.parent_network_id;
                    objHTBInfo.sequence_id = objISPNetworkCode.sequence_id;

                    var customerDetails = new BLCustomer().GetCustomerByCanId(objSaveCustomerAssociationIn.destination_network_id);
                    if (customerDetails != null)
                    {
                        objHTBInfo.region_id = customerDetails.region_id;
                        objHTBInfo.province_id = customerDetails.province_id;
                        objHTBInfo.latitude = customerDetails.latitude;
                        objHTBInfo.longitude = customerDetails.longitude;
                    }
                }

                var objItem = new BusinessLogics.Admin.BLVendorSpecification().GetItemMasterDetailById(item.id);
                objHTBInfo.userId = objSaveCustomerAssociationIn.user_id;
                objHTBInfo.specification = objItem.specification;
                objHTBInfo.subcategory1 = objItem.subcategory_1;
                objHTBInfo.subcategory2 = objItem.subcategory_2;
                objHTBInfo.subcategory3 = objItem.subcategory_3;
                objHTBInfo.item_code = objItem.code;
                objHTBInfo.no_of_input_port = objItem.no_of_input_port;
                objHTBInfo.no_of_input_port = objItem.no_of_output_port;
                objHTBInfo.no_of_port = objItem.no_of_port;
                objHTBInfo.unit = objItem.unit;
                objHTBInfo.other = objItem.other;
                objHTBInfo.vendor_id = objItem.vendor_id;
                objHTBInfo.category = EntityType.HTB.ToString();

                var result = new BLISP().SaveHTBDetails(objHTBInfo, objSaveCustomerAssociationIn.user_id);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Models.ISP.OpticalRepeaterInfo SaveOpticalRepeater(SaveCustomerAssociationIn objSaveCustomerAssociationIn, WCRMaterialIN item)
        {
            Models.ISP.OpticalRepeaterInfo objOpticalRepeaterInfo = new Models.ISP.OpticalRepeaterInfo();
            try
            {
                objOpticalRepeaterInfo.geom = GetPointTypeParentGeom(objSaveCustomerAssociationIn.destination_system_id, EntityType.Customer.ToString());
                /**B-RFS**/
                if (objSaveCustomerAssociationIn.building_rfs_type == RFSTypes.BRFS.EnumValue())
                {
                    var objCustomer = BLIspEntityMapping.Instance.GetIspEntityMapByCustomerId(objSaveCustomerAssociationIn.destination_system_id, objSaveCustomerAssociationIn.destination_entity_type);
                    var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objCustomer.structure_id, EntityType.Structure);
                    objOpticalRepeaterInfo.floor_id = objCustomer.floor_id.GetValueOrDefault();
                    objOpticalRepeaterInfo.structure_id = objCustomer.structure_id;
                    objOpticalRepeaterInfo.shaft_id = objCustomer.shaft_id.GetValueOrDefault();
                    objOpticalRepeaterInfo.parent_entity_type = EntityType.Structure.ToString();
                    if (structureDetails != null)
                    {
                        objOpticalRepeaterInfo.region_id = structureDetails.region_id;
                        objOpticalRepeaterInfo.province_id = structureDetails.province_id;
                        objOpticalRepeaterInfo.latitude = structureDetails.latitude;
                        objOpticalRepeaterInfo.longitude = structureDetails.longitude;
                        objOpticalRepeaterInfo.parent_system_id = structureDetails.system_id;
                    }
                    var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objCustomer.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.OpticalRepeater.ToString(), structureId = objCustomer.structure_id });
                    objOpticalRepeaterInfo.opticalrepeater_name = objISPNetworkCode.network_code;
                    objOpticalRepeaterInfo.network_id = objISPNetworkCode.network_code;
                    objOpticalRepeaterInfo.sequence_id = objISPNetworkCode.sequence_id;
                }
                else
                {
                    /** A-C-RFS**/
                    var objISPNetworkCode = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.OpticalRepeater.ToString(), gType = GeometryType.Point.ToString(), eGeom = objOpticalRepeaterInfo.geom, parent_eType = "", parent_sysId = 0 });
                    // var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHTBInfo.geom, parent_eType = "", parent_sysId = 0 });
                    objOpticalRepeaterInfo.opticalrepeater_name = objISPNetworkCode.network_code;
                    objOpticalRepeaterInfo.network_id = objISPNetworkCode.network_code;
                    objOpticalRepeaterInfo.sequence_id = objISPNetworkCode.sequence_id;
                    objOpticalRepeaterInfo.parent_entity_type = objISPNetworkCode.parent_entity_type;
                    objOpticalRepeaterInfo.parent_system_id = objISPNetworkCode.parent_system_id;
                    objOpticalRepeaterInfo.parent_network_id = objISPNetworkCode.parent_network_id;
                    objOpticalRepeaterInfo.sequence_id = objISPNetworkCode.sequence_id;

                    var customerDetails = new BLCustomer().GetCustomerByCanId(objSaveCustomerAssociationIn.destination_network_id);
                    if (customerDetails != null)
                    {
                        objOpticalRepeaterInfo.region_id = customerDetails.region_id;
                        objOpticalRepeaterInfo.province_id = customerDetails.province_id;
                        objOpticalRepeaterInfo.latitude = customerDetails.latitude;
                        objOpticalRepeaterInfo.longitude = customerDetails.longitude;
                    }
                }

                var objItem = new BusinessLogics.Admin.BLVendorSpecification().GetItemMasterDetailById(item.id);
                objOpticalRepeaterInfo.userId = objSaveCustomerAssociationIn.user_id;
                objOpticalRepeaterInfo.specification = objItem.specification;
                objOpticalRepeaterInfo.subcategory1 = objItem.subcategory_1;
                objOpticalRepeaterInfo.subcategory2 = objItem.subcategory_2;
                objOpticalRepeaterInfo.subcategory3 = objItem.subcategory_3;
                objOpticalRepeaterInfo.item_code = objItem.code;
                objOpticalRepeaterInfo.no_of_input_port = objItem.no_of_input_port;
                objOpticalRepeaterInfo.no_of_input_port = objItem.no_of_output_port;
                objOpticalRepeaterInfo.no_of_port = objItem.no_of_port;
                objOpticalRepeaterInfo.unit = objItem.unit;
                objOpticalRepeaterInfo.other = objItem.other;
                objOpticalRepeaterInfo.vendor_id = objItem.vendor_id;
                objOpticalRepeaterInfo.category = EntityType.OpticalRepeater.ToString();

                var result = new BLISP().SaveOpticalRepeaterDetails(objOpticalRepeaterInfo, objSaveCustomerAssociationIn.user_id);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ONTMaster SaveONT(SaveCustomerAssociationIn objSaveCustomerAssociationIn, WCRMaterialIN item)
        {
            ONTMaster objONTMaster = new ONTMaster();
            List<InRegionProvince> objRegionProvince = new List<InRegionProvince>();

            if (objSaveCustomerAssociationIn.building_rfs_type == RFSTypes.BRFS.EnumValue())
            {
                var objCustomer = BLIspEntityMapping.Instance.GetIspEntityMapByCustomerId(objSaveCustomerAssociationIn.destination_system_id, objSaveCustomerAssociationIn.destination_entity_type);

                objONTMaster.objIspEntityMap.shaft_id = objCustomer.shaft_id;
                objONTMaster.objIspEntityMap.shaft_id = objCustomer.floor_id;
                objONTMaster.structure_id = objCustomer.structure_id;

                objONTMaster.geom = GetPointTypeParentGeom(objCustomer.structure_id, EntityType.Structure.ToString());
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objONTMaster.geom, parent_eType = EntityType.Structure.ToString(), parent_sysId = objCustomer.structure_id });
                if (string.IsNullOrEmpty(objNetworkCodeDetail.err_msg))
                {
                    objONTMaster.parent_entity_type = objNetworkCodeDetail.parent_entity_type;
                    objONTMaster.parent_network_id = objNetworkCodeDetail.parent_network_id;
                    objONTMaster.parent_system_id = objNetworkCodeDetail.parent_system_id;
                }
                objONTMaster.ont_name = objNetworkCodeDetail.network_code;
                objONTMaster.network_id = objNetworkCodeDetail.network_code;
                objONTMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            }
            else
            {
                objONTMaster.geom = GetPointTypeParentGeom(objSaveCustomerAssociationIn.destination_system_id, EntityType.Customer.ToString());
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objONTMaster.geom, parent_eType = "", parent_sysId = 0 });
                if (string.IsNullOrEmpty(objNetworkCodeDetail.err_msg))
                {
                    objONTMaster.parent_entity_type = objNetworkCodeDetail.parent_entity_type;
                    objONTMaster.parent_network_id = objNetworkCodeDetail.parent_network_id;
                    objONTMaster.parent_system_id = objNetworkCodeDetail.parent_system_id;
                }
                objONTMaster.ont_name = objNetworkCodeDetail.network_code;
                objONTMaster.network_id = objNetworkCodeDetail.network_code;
                objONTMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            }
            objONTMaster.longitude = Convert.ToDouble(objONTMaster.geom.Split(' ')[0]);
            objONTMaster.latitude = Convert.ToDouble(objONTMaster.geom.Split(' ')[1]);
            objRegionProvince = BLBuilding.Instance.GetRegionProvince(objONTMaster.geom, GeometryType.Point.ToString());
            if (objRegionProvince != null && objRegionProvince.Count > 0)
            {
                objONTMaster.region_id = objRegionProvince[0].region_id;
                objONTMaster.province_id = objRegionProvince[0].province_id;
                objONTMaster.region_name = objRegionProvince[0].region_name;
                objONTMaster.province_name = objRegionProvince[0].province_name;
            }
            var objItem = new BusinessLogics.Admin.BLVendorSpecification().GetItemMasterDetailById(item.id);
            objONTMaster.specification = objItem.specification;
            objONTMaster.subcategory1 = objItem.subcategory_1;
            objONTMaster.subcategory2 = objItem.subcategory_2;
            objONTMaster.subcategory3 = objItem.subcategory_3;
            objONTMaster.item_code = objItem.code;
            objONTMaster.no_of_input_port = objItem.no_of_input_port;
            objONTMaster.no_of_output_port = objItem.no_of_output_port;
            objONTMaster.no_of_port = objItem.no_of_port;
            objONTMaster.unit = objItem.unit;
            objONTMaster.other = objItem.other;
            objONTMaster.vendor_id = objItem.vendor_id;
            objONTMaster.specification = objItem.specification;
            objONTMaster.category = EntityType.ONT.ToString();
            var resultItem = new BLONT().SaveONTEntity(objONTMaster, objSaveCustomerAssociationIn.user_id);
            return resultItem;
        }

        public CableMaster GetLineNtkDetail(CableMaster objCableMaster, LineEntityIn objIn, string enName, bool isAuto, string geom)
        {
            var startObj = objIn.lstTP[0];
            var endObj = objIn.lstTP[objIn.lstTP.Count() - 1];
            var start_network_id = startObj.network_id;
            var end_network_id = endObj.network_id;
            //fill parent detail....
            var networkCodeDetail = new BLMisc().GetLineNetworkCode(start_network_id, end_network_id, enName, geom, CableTypes.ISP.ToString());
            objCableMaster.cable_name = networkCodeDetail.network_code;
            objCableMaster.parent_entity_type = networkCodeDetail.parent_entity_type;
            objCableMaster.parent_network_id = networkCodeDetail.parent_network_id;
            objCableMaster.parent_system_id = networkCodeDetail.parent_system_id;
            objCableMaster.cable_type = CableTypes.ISP.ToString();

            if (!string.IsNullOrEmpty(networkCodeDetail.network_code))
            {
                if (objIn.networkIdType == NetworkIdType.M.ToString())
                {
                    //FILL NETWORK CODE FORMAT FOR MANUAL
                    objCableMaster.network_id = networkCodeDetail.network_code;
                }
                else if (objIn.networkIdType == NetworkIdType.A.ToString() && isAuto)
                {
                    objCableMaster.network_id = networkCodeDetail.network_code;
                }
                objCableMaster.a_entity_type = startObj.network_name;
                objCableMaster.a_system_id = startObj.system_id;
                objCableMaster.a_location = start_network_id;

                objCableMaster.b_entity_type = endObj.network_name;
                objCableMaster.b_system_id = endObj.system_id;
                objCableMaster.b_location = end_network_id;
                objCableMaster.sequence_id = networkCodeDetail.sequence_id;
            }
            return objCableMaster;
        }

        public void SaveAutoSplicing(saveEntityAssociation objsaveEntityAssociation, SaveCustomerAssociationIn objSaveCustomerAssociationIn)
        {
            List<ConnectionInfoMaster> objConnectionInfo = new List<ConnectionInfoMaster>();
            ConnectionInfoMaster objConnectionInfoMaster = new ConnectionInfoMaster();
            var objSplitter = new BLMisc().GetEntityDetailById<SplitterMaster>(objSaveCustomerAssociationIn.source_system_id, EntityType.Splitter);

            /**Source:Splitter,Destination:Cable**/
            objConnectionInfoMaster.source_system_id = objSplitter.system_id;
            objConnectionInfoMaster.source_network_id = objSplitter.network_id;
            objConnectionInfoMaster.source_entity_type = EntityType.Splitter.ToString();
            objConnectionInfoMaster.source_port_no = objSaveCustomerAssociationIn.source_port_number;
            objConnectionInfoMaster.is_source_cable_a_end = false;
            objConnectionInfoMaster.destination_entity_type = EntityType.Cable.ToString();
            objConnectionInfoMaster.destination_network_id = objsaveEntityAssociation.lstCableInfo[0].network_id;
            objConnectionInfoMaster.destination_port_no = 1;
            objConnectionInfoMaster.destination_system_id = objsaveEntityAssociation.lstCableInfo[0].system_id;
            objConnectionInfoMaster.is_destination_cable_a_end = true;
            objConnectionInfoMaster.equipment_entity_type = objSplitter.parent_entity_type;
            objConnectionInfoMaster.equipment_network_id = objSplitter.parent_network_id;
            objConnectionInfoMaster.equipment_system_id = objSplitter.parent_system_id;
            objConnectionInfo.Add(objConnectionInfoMaster);

            /**Source:Cable,Destination:HTB**/
            objConnectionInfoMaster = new ConnectionInfoMaster();
            objConnectionInfoMaster.source_system_id = objsaveEntityAssociation.lstCableInfo[0].system_id;
            objConnectionInfoMaster.source_network_id = objsaveEntityAssociation.lstCableInfo[0].network_id;
            objConnectionInfoMaster.source_entity_type = EntityType.Cable.ToString();
            objConnectionInfoMaster.source_port_no = 1;
            objConnectionInfoMaster.is_source_cable_a_end = false;
            objConnectionInfoMaster.destination_entity_type = EntityType.HTB.ToString();
            objConnectionInfoMaster.destination_network_id = objsaveEntityAssociation.lstHTBInfo[0].network_id;
            objConnectionInfoMaster.destination_port_no = -1;
            objConnectionInfoMaster.destination_system_id = objsaveEntityAssociation.lstHTBInfo[0].system_id;
            objConnectionInfoMaster.is_destination_cable_a_end = false;
            objConnectionInfoMaster.equipment_entity_type = "";
            objConnectionInfoMaster.equipment_network_id = "";
            objConnectionInfoMaster.equipment_system_id = 0;
            objConnectionInfo.Add(objConnectionInfoMaster);

            /**Source:HTB,Destination:ONT**/
            objConnectionInfoMaster = new ConnectionInfoMaster();
            objConnectionInfoMaster.source_system_id = objsaveEntityAssociation.lstHTBInfo[0].system_id;
            objConnectionInfoMaster.source_network_id = objsaveEntityAssociation.lstHTBInfo[0].network_id;
            objConnectionInfoMaster.source_entity_type = EntityType.HTB.ToString();
            objConnectionInfoMaster.source_port_no = 1;
            objConnectionInfoMaster.is_source_cable_a_end = false;
            objConnectionInfoMaster.destination_entity_type = EntityType.ONT.ToString();
            objConnectionInfoMaster.destination_network_id = objsaveEntityAssociation.lstONTInfo[0].network_id;
            objConnectionInfoMaster.destination_port_no = -1;
            objConnectionInfoMaster.destination_system_id = objsaveEntityAssociation.lstONTInfo[0].system_id;
            objConnectionInfoMaster.is_destination_cable_a_end = false;
            objConnectionInfoMaster.equipment_entity_type = "";
            objConnectionInfoMaster.equipment_network_id = "";
            objConnectionInfoMaster.equipment_system_id = 0;
            objConnectionInfo.Add(objConnectionInfoMaster);

            /**Source:ONT,Destination:Customer**/
            objConnectionInfoMaster = new ConnectionInfoMaster();
            objConnectionInfoMaster.source_system_id = objsaveEntityAssociation.lstONTInfo[0].system_id;
            objConnectionInfoMaster.source_network_id = objsaveEntityAssociation.lstONTInfo[0].network_id;
            objConnectionInfoMaster.source_entity_type = EntityType.ONT.ToString();
            objConnectionInfoMaster.source_port_no = 1;
            objConnectionInfoMaster.is_source_cable_a_end = false;
            objConnectionInfoMaster.destination_entity_type = EntityType.Customer.ToString();
            objConnectionInfoMaster.destination_network_id = objSaveCustomerAssociationIn.destination_network_id;
            objConnectionInfoMaster.destination_port_no = 1;
            objConnectionInfoMaster.destination_system_id = objSaveCustomerAssociationIn.destination_system_id;
            objConnectionInfoMaster.is_destination_cable_a_end = false;
            objConnectionInfoMaster.equipment_entity_type = "";
            objConnectionInfoMaster.equipment_network_id = "";
            objConnectionInfoMaster.equipment_system_id = 0;
            objConnectionInfo.Add(objConnectionInfoMaster);

            var objConnection = new BLOSPSplicing().SaveConnectionInfo(Newtonsoft.Json.JsonConvert.SerializeObject(objConnectionInfo));
        }

        #region Get WCR Material
        [System.Web.Http.HttpPost]
        public ApiResponse<List<Models.Admin.VendorSpecificationMaster>> GetWCRMaterial(ReqInput data)
        {
            var response = new ApiResponse<List<Models.Admin.VendorSpecificationMaster>>();
            try
            {

                GetWCRMatrialIn objGetWCRMaterialIn = ReqHelper.GetRequestData<GetWCRMatrialIn>(data);
                if (string.IsNullOrWhiteSpace(objGetWCRMaterialIn.rfs_type.ToString()))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = " rfs type can not be null or empty!!";
                    return response;
                }

                List<Models.Admin.VendorSpecificationMaster> lstobj = new BusinessLogics.Admin.BLVendorSpecification().GetWCRMaterial(objGetWCRMaterialIn.rfs_type);
                response.status = StatusCodes.OK.ToString();
                response.results = lstobj;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetWCRMaterial()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion

        [System.Web.Http.HttpPost]
        public ApiResponse<DBInfoViewModel> GetNearByDBInfo(ReqInput data)
        {
            var response = new ApiResponse<DBInfoViewModel>();
            try
            {
                DBInfoViewModel objDBInfoViewModel = new DBInfoViewModel();
                NearByDistributionBoxInfoIn objDBInfoIn = ReqHelper.GetRequestData<NearByDistributionBoxInfoIn>(data);
                if (string.IsNullOrEmpty(objDBInfoIn.latitude.ToString()) || string.IsNullOrEmpty(objDBInfoIn.longitude.ToString()))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "latitude/longitude can not be null or empty!";
                    return response;
                }
                var result = new BLMisc().GetNearByDBInfo(objDBInfoIn.latitude, objDBInfoIn.longitude, objDBInfoIn.box_type, objDBInfoIn.rfs_type, objDBInfoIn.bufferInMtr, objDBInfoIn.module_abbr);

                if (result != null)
                {
                    objDBInfoViewModel.lstDBinfo = result;
                    response.status = StatusCodes.OK.ToString();
                    response.results = objDBInfoViewModel;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();

                    response.error_message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getNearByDistributionBoxInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<string> UpdateEntityBarCode(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                BarCodeIn objBarCodeIn = ReqHelper.GetRequestData<BarCodeIn>(data);
                if (string.IsNullOrEmpty(objBarCodeIn.barcode))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Barcode value can not be blank!";
                    return response;
                }
                var result = new BLMisc().UpdateEntityBarCode(objBarCodeIn.system_id, objBarCodeIn.entity_type, objBarCodeIn.barcode, objBarCodeIn.network_id);

                if (result != null && result.status)
                {
                    response.error_message = result.message;
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = result.message;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateEntityBarCode()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<string> UpdatePowerMeterReading(ReqInput data)
		{
			var response = new ApiResponse<string>();
			try
			{
				PowerMeterReadingIn objPowerMeterReadingIn = ReqHelper.GetRequestData<PowerMeterReadingIn>(data);
                if (objPowerMeterReadingIn.power_meter_reading == 0 )
				{
					response.status = StatusCodes.VALIDATION_FAILED.ToString();
					response.error_message = "Power meter reading value can not be 0";
					return response;
				}
				var result = new BLMisc().UpdatePowerMeterReading(objPowerMeterReadingIn.system_id, objPowerMeterReadingIn.entity_type, objPowerMeterReadingIn.power_meter_reading, objPowerMeterReadingIn.is_manual_meter_reading);

				if (result != null && result.status)
				{
					response.error_message = result.message;
					response.status = StatusCodes.OK.ToString();
				}
				else
				{
					response.status = StatusCodes.VALIDATION_FAILED.ToString();
					response.error_message = result.message;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("UpdatePowerMeterReading()", "Main Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = "Error While Processing  Request.";
			}

			return response;
		}

        [System.Web.Http.HttpPost]
        public ApiResponse<userModuleViewModel> GetUserModule(ReqInput data)
        {
            var response = new ApiResponse<userModuleViewModel>();
            try
            {
                userModuleViewModel objUserModuleViewModel = new userModuleViewModel();

                UserModuleIn objUserModuleIn = ReqHelper.GetRequestData<UserModuleIn>(data);
                var result = new BLMisc().GetUserModule(objUserModuleIn.userId, UserType.Mobile.ToString());
                HeaderAttributes headerAttribute = ReqHelper.getHeaderValue(Request.Headers.ToList());
                UserLoginHistoryInfo objUserLoginHistory = new UserLoginHistoryInfo();
                headerAttribute.authorization = headerAttribute.authorization.Replace("bearer","").Trim();
                objUserLoginHistory= new BLUserLoginHistory().UpdateUserLoginById(objUserModuleIn.userId, headerAttribute.authorization);
                if (result != null)
                {
                    objUserModuleViewModel.lstUserModule = result.Where(c => c.parent_module_id == 0).Select(c => new UserModule()
                    {
                        Id = c.Id,
                        module_name = c.module_name,
                        is_selected = c.is_selected,
                        module_description = c.module_description,
                        icon_content = c.icon_content,
                        type = c.type,
                        icon_class = c.icon_class,
                        module_abbr = c.module_abbr,
                        module_sequence = c.module_sequence,
                        parent_module_id = c.parent_module_id,
                        is_offline_enabled = c.is_offline_enabled,
                        lstSubModule = GetSubModule(result, c.Id)
                    }).ToList();
                    response.status = StatusCodes.OK.ToString();
                    response.results = objUserModuleViewModel;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();

                    response.error_message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetUserModule()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        public static List<UserModule> GetSubModule(List<UserModule> subModules, int moduleId)
        {
            return subModules.Where(c => c.parent_module_id == moduleId).Select(c => new UserModule
            {
                Id = c.Id,
                module_name = c.module_name,
                is_selected = c.is_selected,
                module_description = c.module_description,
                icon_content = c.icon_content,
                type = c.type,
                icon_class = c.icon_class,
                module_abbr = c.module_abbr,
                module_sequence = c.module_sequence,
                parent_module_id = c.parent_module_id,
                lstSubModule = GetSubModule(subModules, c.Id)
            }).ToList();
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<Dictionary<string, string>> GetEntityAdvanceAttribute(ReqInput data)
        {
            var response = new ApiResponse<Dictionary<string, string>>();
            try
            {
                EntityAdvanceAttributeIn objEntityAdvanceAttributeIn = ReqHelper.GetRequestData<EntityAdvanceAttributeIn>(data);
                if (objEntityAdvanceAttributeIn.systemId == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid system id!";
                    return response;
                }
                else if (string.IsNullOrEmpty(objEntityAdvanceAttributeIn.entityType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Entity Type is required!";
                    return response;
                }
                else if (string.IsNullOrEmpty(objEntityAdvanceAttributeIn.geomType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Geometry Type is required!";
                    return response;
                }
                response.results = new BLMisc().getEntityAdvanceAttribute(objEntityAdvanceAttributeIn.systemId, objEntityAdvanceAttributeIn.entityType, objEntityAdvanceAttributeIn.geomType);
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntityAdvanceAttribute()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<EquipementSearchResultViewModel> GetEquipmentSearchResult(ReqInput data)
        {
            var response = new ApiResponse<EquipementSearchResultViewModel>();
            try
            {
                EquipementSearchResultViewModel objEquipementSearchResult = new EquipementSearchResultViewModel();

                GetEquipmentSearchResultIn objGetEquipmentSearchResultIn = ReqHelper.GetRequestData<GetEquipmentSearchResultIn>(data);

                if (string.IsNullOrEmpty(objGetEquipmentSearchResultIn.searchText))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "SearchText is required!";
                    return response;
                }
                var result = new BLOSPSplicing().GetSearchEquipmentResult(objGetEquipmentSearchResultIn.searchText, objGetEquipmentSearchResultIn.entityType, objGetEquipmentSearchResultIn.userId);
                if (result.Count > 0)
                {
                    objEquipementSearchResult.lstEquipementSearchResult = result;
                    response.status = StatusCodes.OK.ToString();
                    response.results = objEquipementSearchResult;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();

                    response.error_message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEquipmentSearchResult()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<EquipementPortViewModel> GetEquipmentPortInfo(ReqInput data)
        {
            var response = new ApiResponse<EquipementPortViewModel>();
            try
            {
                EquipementPortViewModel objEquipementPortViewModel = new EquipementPortViewModel();

                GetEquipmentPortInfoIn objGetEquipmentPortInfoIn = ReqHelper.GetRequestData<GetEquipmentPortInfoIn>(data);
                var result = new BLOSPSplicing().GetEquipmentPort(objGetEquipmentPortInfoIn.entity_id, objGetEquipmentPortInfoIn.entity_type);
                if (result != null)
                {
                    objEquipementPortViewModel.lstEquipementPort = result;
                    response.status = StatusCodes.OK.ToString();
                    response.results = objEquipementPortViewModel;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();

                    response.error_message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEquipmentPortInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<ConnectionInfoViewModel> GetConnectionInfo(ReqInput data)
        {
            var response = new ApiResponse<ConnectionInfoViewModel>();
            try
            {
                ConnectionInfoViewModel objConnectionInfoViewModel = new ConnectionInfoViewModel();
                ConnectionInfoFilter objViewFilter = new ConnectionInfoFilter();


                GetConnectionInfoIn objGetConnectionInfoIn = ReqHelper.GetRequestData<GetConnectionInfoIn>(data);

                objViewFilter.entityid = objGetConnectionInfoIn.entityid;
                objViewFilter.entity_type = objGetConnectionInfoIn.entity_type;
                objViewFilter.port_no = objGetConnectionInfoIn.port_no;
                objViewFilter.searchBy = "";
                objViewFilter.searchText = "";
                objViewFilter.currentPage = 1;
                objViewFilter.pageSize = 0;
                objViewFilter.sort = "";
                var result = new BLOSPSplicing().GetConnectionInfoPath(objViewFilter).lstConnectionInfo;
                if (result != null)
                {
                    objConnectionInfoViewModel.lstConnectionInfo = result.Where(x => (x.source_entity_type.ToUpper() + x.source_system_id.ToString()) != (x.destination_entity_type.ToUpper() + x.destination_system_id.ToString())).ToList();
                    objConnectionInfoViewModel.lstConnectionInfo = (from S in objConnectionInfoViewModel.lstConnectionInfo
                                                                    select new ConnectionInfo
                                                                    {
                                                                        connection_id = S.connection_id,
                                                                        source_system_id = S.source_system_id,
                                                                        source_network_id = S.source_network_id,
                                                                        source_entity_type = S.source_entity_type,
                                                                        source_entity_title = S.source_entity_title,
                                                                        source_port_no = S.source_port_no,
                                                                        is_source_virtual = S.is_source_virtual,
                                                                        destination_system_id = S.destination_system_id,
                                                                        destination_network_id = S.destination_network_id,
                                                                        destination_entity_type = S.destination_entity_type,
                                                                        destination_entity_title = S.destination_entity_title,
                                                                        destination_port_no = S.destination_port_no,
                                                                        is_customer_connected = S.is_customer_connected,
                                                                        is_destination_virtual = S.is_destination_virtual,
                                                                        created_on = S.created_on,
                                                                        created_by = S.created_by,
                                                                        approved_by = S.approved_by,
                                                                        approved_on = S.approved_on,
                                                                        childordering = S.childordering,
                                                                        totalRecords = S.totalRecords,
                                                                        id = S.id,
                                                                        is_backward_path = S.is_backward_path,
                                                                        sp_geometry = S.sp_geometry,
                                                                        cable_calculated_length = S.cable_calculated_length,
                                                                        cable_measured_length = S.cable_measured_length,
                                                                        splitter_ratio = S.splitter_ratio,
                                                                        isprocessed = S.isprocessed,
                                                                        source_display_name = S.source_display_name,
                                                                        destination_display_name = S.destination_display_name
                                                                    }).ToList();
                    response.status = StatusCodes.OK.ToString();
                    response.results = objConnectionInfoViewModel;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();

                    response.error_message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetConnectionInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<CPFElementsViewModel> GetCPFelementPath(ReqInput data)
        {
            var response = new ApiResponse<CPFElementsViewModel>();
            try
            {
                CPFElementsViewModel objCPFElementsViewModel = new CPFElementsViewModel();
                ConnectionInfoFilter objViewFilter = new ConnectionInfoFilter();


                GetCPFelementPathIn objGetCPFelementPathIn = ReqHelper.GetRequestData<GetCPFelementPathIn>(data);

                objViewFilter.entityid = objGetCPFelementPathIn.entityid;
                objViewFilter.entity_type = objGetCPFelementPathIn.entity_type;
                objViewFilter.port_no = objGetCPFelementPathIn.port_no;
                objViewFilter.isStartingPoint = objGetCPFelementPathIn.isStartingPoint;
                var result = new BLOSPSplicing().GetCPFElement(objViewFilter);
                if (result != null)
                {
                    objCPFElementsViewModel.lstCPFElements = result;
                    response.status = StatusCodes.OK.ToString();
                    response.results = objCPFElementsViewModel;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();

                    response.error_message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetCPFelementPath()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<LogicalViewVM> GetEntityLogicalView(ReqInput data)
        {
            var response = new ApiResponse<LogicalViewVM>();
            try
            {
                LogicalViewVM objLogicalViewVM = new LogicalViewVM();
                GetEntityLogicalViewIn objGetEntityLogicalViewIn = ReqHelper.GetRequestData<GetEntityLogicalViewIn>(data);

                var result = new BLOSPSplicing().getEntityLogicalView(objGetEntityLogicalViewIn.systemId, objGetEntityLogicalViewIn.entityType);
                objLogicalViewVM.lstport = result;
                objLogicalViewVM.listPortStatus = new BLPortStatus().getPortStatus();
                response.status = StatusCodes.OK.ToString();
                response.results = objLogicalViewVM;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntityLogicalView()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<LogicalViewEquipementSearchVM> GetLogicalViewEquipmentSearchResult(ReqInput data)
        {
            var response = new ApiResponse<LogicalViewEquipementSearchVM>();
            try
            {
                LogicalViewEquipementSearchVM objLogicalViewEquipementSearchVM = new LogicalViewEquipementSearchVM();

                GetLogicalViewEquipmentSearchIn objGetLogicalViewEquipmentSearchIn = ReqHelper.GetRequestData<GetLogicalViewEquipmentSearchIn>(data);

                if (string.IsNullOrEmpty(objGetLogicalViewEquipmentSearchIn.searchText))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "SearchText is required!";
                    return response;
                }
                var result = new BLOSPSplicing().GetLogicalViewSearchEquipmentResult(objGetLogicalViewEquipmentSearchIn.searchText, objGetLogicalViewEquipmentSearchIn.entityType, objGetLogicalViewEquipmentSearchIn.userId);
                if (result.Count > 0)
                {
                    objLogicalViewEquipementSearchVM.lstLogicalViewEquipementSearch = result;
                    response.status = StatusCodes.OK.ToString();
                    response.results = objLogicalViewEquipementSearchVM;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();

                    response.error_message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEquipmentSearchResult()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }


        [System.Web.Http.HttpPost]
        public ApiResponse<vwLayerActionMapping> GetEntityActions(ReqInput data)
        {
            var response = new ApiResponse<vwLayerActionMapping>();
            try
            {
                vwLayerActionMapping objResult = new vwLayerActionMapping();
                IspEntityMapping objIspEntityMapping = new IspEntityMapping();
                HeaderAttributes headerAttribute = ReqHelper.getHeaderValue(Request.Headers.ToList());
                GetEntityLayerActionsIn objGetEntityLayerActionsIn = ReqHelper.GetRequestData<GetEntityLayerActionsIn>(data);
                // appended the "structure id ,entity system_id and entity_type" in GetEntityActions response.
                objResult.system_id = objGetEntityLayerActionsIn.systemId;
                objResult.entity_type = objGetEntityLayerActionsIn.layer_Name;
                objIspEntityMapping = BLIspEntityMapping.Instance.GetStructureFloorbyEntityId(objGetEntityLayerActionsIn.systemId, objGetEntityLayerActionsIn.layer_Name);
                objResult.structure_id = objIspEntityMapping != null ? objIspEntityMapping.structure_id : 0;
                if(objGetEntityLayerActionsIn.layer_Name == EntityType.SubArea.ToString())
                {
                    SubArea obj = new BLMisc().GetEntityDetailById<SubArea>(objGetEntityLayerActionsIn.systemId, EntityType.SubArea, objGetEntityLayerActionsIn.user_id);
                    objResult.gis_design_id=obj.gis_design_id;
                }
                //////////////////////////////////////////
                objResult.lstLayerActionMapping = new BLMisc().getLayerActions(objGetEntityLayerActionsIn.systemId, objGetEntityLayerActionsIn.layer_Name, false, objGetEntityLayerActionsIn.network_status, objGetEntityLayerActionsIn.role_id, objGetEntityLayerActionsIn.user_id, true, headerAttribute.source_ref_id, headerAttribute.source_ref_type);
                for (int i = 0; i < objResult.lstLayerActionMapping.Count; i++)
                {
                    objResult.lstLayerActionMapping[i].action_title = BLConvertMLanguage.MultilingualMessageConvert(objResult.lstLayerActionMapping[i].action_title);
                }
                response.status = StatusCodes.OK.ToString();
                response.results = objResult;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntityActions()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<List<MobileRegionProvince>> GetRegionProvince(ReqInput data)
        {
            var response = new ApiResponse<List<MobileRegionProvince>>();
            try
            {
                List<MobileRegionProvince> objLayerActionMapping = new List<MobileRegionProvince>();
                GetRegionProvinceLayersIn objGetEntityLayerActionsIn = ReqHelper.GetRequestData<GetRegionProvinceLayersIn>(data);
                objLayerActionMapping = new BLMisc().getRegionProvince(objGetEntityLayerActionsIn.user_id);
                response.status = StatusCodes.OK.ToString();
                response.results = objLayerActionMapping;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntityActions()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<List<EntityLayerActions>> GetAllEntityActions(ReqInput data)
        {
            var response = new ApiResponse<List<EntityLayerActions>>();
            try
            {
                List<EntityLayerActions> objLayerActionMapping = new List<EntityLayerActions>();
                GetAllEntityActionsIn objGetEntityLayerActionsIn = ReqHelper.GetRequestData<GetAllEntityActionsIn>(data);
                objLayerActionMapping = new BLMisc().getEntityLayerActions(objGetEntityLayerActionsIn.user_id);

                response.status = StatusCodes.OK.ToString();
                response.results = objLayerActionMapping;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntityLayerActions()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<vmNearbyentityDetails> GetNearbyEntityDetailsWithAtributes(ReqInput data)
        {
            var response = new ApiResponse<vmNearbyentityDetails>();
            try
            {
                vmNearbyentityDetails objNearbyEntityDetailsVM = new vmNearbyentityDetails();
                GetNearbyEntityDetailsIn GetNearbyEntityDetailsIn = ReqHelper.GetRequestData<GetNearbyEntityDetailsIn>(data);

                if (GetNearbyEntityDetailsIn.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }
                else if (GetNearbyEntityDetailsIn.latitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid latitude!";
                    return response;
                }
                else if (GetNearbyEntityDetailsIn.longitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid longitude!";
                    return response;
                }
                else if (GetNearbyEntityDetailsIn.user_id <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid user Id!";
                    return response;
                }
                else if (GetNearbyEntityDetailsIn.batch_size <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid batch size!";
                    return response;
                }
                objNearbyEntityDetailsVM.lstNearByEntityDetails = new BLMisc().GetNearbyEntityDetailsWithAtributes(GetNearbyEntityDetailsIn.latitude, GetNearbyEntityDetailsIn.longitude, GetNearbyEntityDetailsIn.bufferInMtrs, GetNearbyEntityDetailsIn.user_id, GetNearbyEntityDetailsIn.batch_size, GetNearbyEntityDetailsIn.last_record_number, GetNearbyEntityDetailsIn.pageno, GetNearbyEntityDetailsIn.pagerecord, GetNearbyEntityDetailsIn.layer_name);
                response.status = StatusCodes.OK.ToString();
                response.results = objNearbyEntityDetailsVM;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearbyEntityDetails()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<nearByEntityDetailsCount> GetNearbyEntityDetailsWithAtributesCount(ReqInput data)
        {
            var response = new ApiResponse<nearByEntityDetailsCount>();
            try
            {
                nearByEntityDetailsCount objNearbyEntityCount = new nearByEntityDetailsCount();
                GetNearbyEntityDetailsIn GetNearbyEntityDetailsIn = ReqHelper.GetRequestData<GetNearbyEntityDetailsIn>(data);

                if (GetNearbyEntityDetailsIn.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }
                else if (GetNearbyEntityDetailsIn.latitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid latitude!";
                    return response;
                }
                else if (GetNearbyEntityDetailsIn.longitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid longitude!";
                    return response;
                }
                else if (GetNearbyEntityDetailsIn.user_id <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid user Id!";
                    return response;
                }
                objNearbyEntityCount.totalrecords = new BLMisc().GetNearbyEntityDetailsWithAtributesCount(GetNearbyEntityDetailsIn.latitude, GetNearbyEntityDetailsIn.longitude, GetNearbyEntityDetailsIn.bufferInMtrs, GetNearbyEntityDetailsIn.user_id, GetNearbyEntityDetailsIn.layer_name);
                response.status = StatusCodes.OK.ToString();
                response.results = objNearbyEntityCount;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearbyEntityDetailsWithAtributesCount()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<List<nearByNetworkEntities>> GetNearByNetworkEntities(ReqInput data)
        {
            var response = new ApiResponse<List<nearByNetworkEntities>>();
            try
            {

                GetNearByNetworkEntitiesIn objGetNearByNetworkEntitiesIn = ReqHelper.GetRequestData<GetNearByNetworkEntitiesIn>(data);
                if (objGetNearByNetworkEntitiesIn.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }
                else if ((objGetNearByNetworkEntitiesIn.latitude == 0 || objGetNearByNetworkEntitiesIn.longitude == 0) && ((string.IsNullOrWhiteSpace(objGetNearByNetworkEntitiesIn.buildingCode) || (string.IsNullOrWhiteSpace(objGetNearByNetworkEntitiesIn.buildingName)))))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Please enter lat/long or BuildingCode/BuildingName!";
                    return response;
                }
                //else if (objGetNearByNetworkEntitiesIn.latitude == 0)
                //{
                //    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //    response.error_message = "Invalid latitude!";
                //    return response;
                //}
                //else if (objGetNearByNetworkEntitiesIn.longitude == 0)
                //{
                //    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //    response.error_message = "Invalid longitude!";
                //    return response;
                //}
                //Models.User objUser = new BLUser().GetUserDetailByID(objEntityTemplateIn.user_id);

                response.results = new BLMisc().getNearByNetworkEntities(objGetNearByNetworkEntitiesIn.latitude, objGetNearByNetworkEntitiesIn.longitude, objGetNearByNetworkEntitiesIn.buildingCode, objGetNearByNetworkEntitiesIn.buildingName, objGetNearByNetworkEntitiesIn.entityTypes, objGetNearByNetworkEntitiesIn.bufferInMtrs);
                response.status = StatusCodes.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearByNetworkEntities()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<dynamic> GetThirdPartyVendor()
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                response.results = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
                response.status = StatusCodes.OK.ToString();
            }
            catch (Exception ex)
            {
                response.status = StatusCodes.FAILED.ToString();
                response.error_message = ex.ToString();
            }
            return response;
        }

        #region Upload Image/Document APIs
        [System.Web.Http.HttpPost]
        public ApiResponse<string> UploadAttachment()
        {

            var response = new ApiResponse<string>();
            try
            {
                //HttpPostedFile files = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
                HttpFileCollection files = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files : null;
                if (HttpContext.Current.Request.Files.Count > 0 && files != null)
                {
                    VailidateAttachment obj = new VailidateAttachment();
                    var systemId = HttpContext.Current.Request.Params["entitySystemId"];
                    var entityType = HttpContext.Current.Request.Params["entityType"];
                    var featureName = HttpContext.Current.Request.Params["featureName"];
                    var attachmentType = HttpContext.Current.Request.Params["uploadType"];
                    var UserId = HttpContext.Current.Request.Params["userId"];
                    var validDocumentTypes = ApplicationSettings.validDocumentTypes.Split(new string[] { "," }, StringSplitOptions.None);
                    var validImageTypes = ApplicationSettings.validImageTypes.Split(new string[] { "," }, StringSplitOptions.None);
                    var isBarcodeImage = HttpContext.Current.Request.Params["is_barcode_image"];
					var isMeterReadingImage = HttpContext.Current.Request.Params["is_meter_reading_image"];
                    if (attachmentType.ToUpper() == "DOCUMENT") { obj = ValidateDocumentFileType(HttpContext.Current.Request.Files); }
                    else {  obj = ValidateImageFileType(HttpContext.Current.Request.Files); }
                       
                    if (!string.IsNullOrEmpty(obj.invalidattachmentType))
                    {
                        response.error_message = obj.invalidattachmentType;
                        response.status = StatusCodes.INVALID_FILE.ToString();
                        return response;

                    }
                    else if (!string.IsNullOrEmpty(obj.invalidattachmentsize))
                    {
                        response.error_message = obj.invalidattachmentsize;
                        response.status = StatusCodes.INVALID_FILE.ToString();
                        return response;

                    }
                    else if (!string.IsNullOrEmpty(obj.invalidattachmentename))
                    {
                        response.error_message = obj.invalidattachmentename;
                        response.status = StatusCodes.INVALID_FILE.ToString();
                        return response;

                    }
                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {

                        string FileName = files[i].FileName;
                        var fileExtension = Path.GetExtension(FileName);


                        //if ((ApplicationSettings.MaxuploadFileSize < files.ContentLength / 1024 / 1024) && (attachmentType.ToUpper() == "DOCUMENT"))
                        //{
                        //    response.error_message = String.Format(Resources.Resources.SI_OSP_GBL_JQ_FRM_109, ApplicationSettings.MaxuploadFileSize);
                        //    response.status = StatusCodes.INVALID_FILE.ToString();
                        //    return response;
                        //}
                        //if ((ApplicationSettings.MaxuploadFileSize < files.ContentLength / 1024 / 1024) && (attachmentType.ToUpper() == "IMAGE"))
                        //{
                        //    response.error_message = String.Format(Resources.Resources.SI_OSP_GBL_JQ_GBL_112, ApplicationSettings.MaxuploadFileSize);
                        //    response.status = StatusCodes.INVALID_FILE.ToString();
                        //    return response;
                        //}

                        //if ((attachmentType != null && attachmentType.ToUpper() == "DOCUMENT") && !validDocumentTypes.Contains(fileExtension.ToLower()))
                        //{
                        //    response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_059 + " extension:" + ApplicationSettings.validDocumentTypes;
                        //    response.status = StatusCodes.INVALID_FILE.ToString();
                        //    return response;
                        //}
                        //if ((attachmentType != null && attachmentType.ToUpper() == "IMAGE") && !validImageTypes.Contains(fileExtension.ToLower()))
                        //{
                        //    response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_059 + " extension:" + ApplicationSettings.validImageTypes;
                        //    response.status = StatusCodes.INVALID_FILE.ToString();
                        //    return response;
                        //}
                        var lstDocument = new BLAttachment().getAttachmentDetailsbyId(Convert.ToInt32(systemId), entityType, attachmentType, FileName, Convert.ToInt32(UserId), "");
                        //if (lstDocument.Count > 0)
                        //{
                        //    response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_055;
                        //    response.status = StatusCodes.INVALID_FILE.ToString();
                        //    return response;
                        //}

                        string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        string strFilePath = "";
                        if (entityType == EntityType.ROW.ToString() && !string.IsNullOrEmpty(featureName))
                        {
                            attachmentType = "";
                            strFilePath = ReqHelper.UploadfileOnFTP(featureName, systemId, files[i], attachmentType, strNewfilename, entityType);
                        }
                        else if (!string.IsNullOrEmpty(featureName))
                        {
                            strFilePath = ReqHelper.UploadfileOnFTP(entityType, systemId, files[i], attachmentType, strNewfilename, featureName);
                        }
                        else
                        {
                            strFilePath = ReqHelper.UploadfileOnFTP(entityType, systemId, files[i], attachmentType, strNewfilename);
                        }
                        LibraryAttachment objAttachment = new LibraryAttachment();
                        objAttachment.entity_system_id = Convert.ToInt32(systemId);
                        objAttachment.entity_type = entityType;
                        objAttachment.org_file_name = FileName;
                        objAttachment.file_name = strNewfilename;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_location = strFilePath;
                        objAttachment.upload_type = attachmentType;
                        objAttachment.uploaded_by = UserId;
                        objAttachment.entity_feature_name = featureName;
                        objAttachment.file_size = files[i].ContentLength;
                        objAttachment.uploaded_on = DateTime.Now;
                        objAttachment.is_barcode_image = Convert.ToBoolean(isBarcodeImage);
						objAttachment.is_meter_reading_image = Convert.ToBoolean(isMeterReadingImage);
						//Save Image on FTP and related detail in database..
						var savefile = new BLAttachment().SaveLibraryAttachment(objAttachment);
                        if (Convert.ToBoolean(isBarcodeImage) && entityType== Convert.ToString(EntityType.FDB))//"FDB"
                        {
                            BLISP.Instance.UpdateManualBarcode(Convert.ToInt32(systemId));
                        }
                    }
                    response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_154;
                    response.status = StatusCodes.OK.ToString();
                    return response;
                }
                else
                {
                    response.error_message = "No attachment selected.";
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    return response;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("UploadAttachment()", "Main", ex);
                response.error_message = "Error in uploading attachment!";
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                return response;
                //Error Logging...
            }
        }

        public VailidateAttachment ValidateImageFileType(HttpFileCollection files)
        {
            VailidateAttachment obj = new VailidateAttachment();
            int maxallowedAttachmentSize = ApplicationSettings.MaxuploadFileSize * ApplicationSettings.MaxFileCountLimit;
            List<string> invalidAttachmentType = new List<string>();
            List<string> invalidAttachmentName = new List<string>();
            int totalUploadedAttachmentSize = 0;
            for (int i = 0; i < files.Count; i++)
            {
                var validImageTypes = ApplicationSettings.validImageTypes.Split(new string[] { "," }, StringSplitOptions.None);
                var fileExtension = Path.GetExtension(files[i].FileName);//files[i].FileName.Split('.').LastOrDefault()?.ToLower();
                totalUploadedAttachmentSize = totalUploadedAttachmentSize + files[i].ContentLength;
                if ( !validImageTypes.Contains(fileExtension.ToLower()))
                {
                    invalidAttachmentType.Add(files[i].FileName);
                }
                if (files[i].FileName.Length > 100)
                {
                    invalidAttachmentName.Add(files[i].FileName);

                }
            }
            if (totalUploadedAttachmentSize/1024/1024 > maxallowedAttachmentSize)
            {
                totalUploadedAttachmentSize = 0;
                obj.invalidattachmentsize = String.Format(Resources.Resources.SI_OSP_GBL_JQ_FRM_109, maxallowedAttachmentSize);//"Total file size is too large.Maximum total file size allowed is, " + maxallowedAttachmentSize / 1024 + " MB";
            }
            obj.invalidattachmentType = invalidAttachmentType.Count > 0 ? Resources.Resources.SI_OSP_GBL_JQ_FRM_059 + " extension:" + ApplicationSettings.validImageTypes : string.Empty;//invalidAttachmentType.Count > 0 ? "The following files are not of allowed file type are " + string.Join(", ", invalidAttachmentType) : string.Empty;
            obj.invalidattachmentename = invalidAttachmentName.Count > 0 ? "File Name length should be less than 100 characters. invalid files are " + string.Join(", ", invalidAttachmentType) : string.Empty;
            return obj;
        }
        public VailidateAttachment ValidateDocumentFileType(HttpFileCollection files)
        {
            VailidateAttachment obj = new VailidateAttachment();
            int maxallowedAttachmentSize = ApplicationSettings.MaxuploadFileSize * ApplicationSettings.MaxFileCountLimit;
            List<string> invalidAttachmentType = new List<string>();
            List<string> invalidAttachmentName = new List<string>();
            int totalUploadedAttachmentSize = 0;
            for (int i = 0; i < files.Count; i++)
            {
                var validImageTypes = ApplicationSettings.validDocumentTypes.Split(new string[] { "," }, StringSplitOptions.None);
                var fileExtension = Path.GetExtension(files[i].FileName);//files[i].FileName.Split('.').LastOrDefault()?.ToLower();
                totalUploadedAttachmentSize = totalUploadedAttachmentSize + files[i].ContentLength;
                if (!validImageTypes.Contains(fileExtension.ToLower()))
                {
                    invalidAttachmentType.Add(files[i].FileName);
                }
                if (files[i].FileName.Length > 100)
                {
                    invalidAttachmentName.Add(files[i].FileName);

                }
            }
            if (totalUploadedAttachmentSize / 1024 / 1024 > maxallowedAttachmentSize)
            {
                totalUploadedAttachmentSize = 0;
                obj.invalidattachmentsize = String.Format(Resources.Resources.SI_OSP_GBL_JQ_FRM_109, maxallowedAttachmentSize);//"Total file size is too large.Maximum total file size allowed is, " + maxallowedAttachmentSize / 1024 + " MB";
            }
            obj.invalidattachmentType = invalidAttachmentType.Count > 0 ? Resources.Resources.SI_OSP_GBL_JQ_FRM_059 + " extension:" + ApplicationSettings.validDocumentTypes:string.Empty;//invalidAttachmentType.Count > 0 ? "The following files are not of allowed file type are " + string.Join(", ", invalidAttachmentType) : string.Empty;
            obj.invalidattachmentename = invalidAttachmentName.Count > 0 ? "File Name length should be less than 100 characters. invalid files are " + string.Join(", ", invalidAttachmentType) : string.Empty;
            return obj;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<List<DocumentResult>> GetEntityDocuments(ReqInput data)
        {
            var response = new ApiResponse<List<DocumentResult>>();
            try
            {
                GetAttachmentDetailsIn objgetAttachmentDetailsIn = ReqHelper.GetRequestData<GetAttachmentDetailsIn>(data);
                string FtpUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"]);
                var lstDocument = new BLAttachment().getAttachmentDetails(objgetAttachmentDetailsIn.entitySystemId, objgetAttachmentDetailsIn.entityType, "Document");
                List<DocumentResult> lstDocumentResult = new List<DocumentResult>();

                foreach (var item in lstDocument)
                {
                    lstDocumentResult.Add(new DocumentResult()
                    {
                        Id = item.id,
                        EntitySystemId = item.entity_system_id,
                        FileName = item.file_name,
                        EntityType = item.entity_type,
                        UploadedBy = item.uploaded_by,
                        OrgFileName = item.org_file_name,
                        FileExtension = item.file_extension,
                        FileLocation = item.file_location,
                        UploadType = item.upload_type,
                        file_size = ReqHelper.BytesToString(Convert.ToInt32(item.file_size)),
                        created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString())
                    });

                }

                response.status = StatusCodes.OK.ToString();
                response.results = lstDocumentResult;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetEntityDocuments()", "Main", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
            }
            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<DocumentResult> DeleteAttachment(ReqInput data)
        {
            var response = new ApiResponse<DocumentResult>();
            try
            {
                DeleteAttachmentsIn objDeleteAttachmentsIn = ReqHelper.GetRequestData<DeleteAttachmentsIn>(data);
                string sFilePath = "";
                int deleteChk = 0;
                int DocumentId = objDeleteAttachmentsIn.attachmentId;
                //Get File Name and Path...
                var lstAttachmentDetails = new BLAttachment().getEntityDocumentById(DocumentId);
                if (lstAttachmentDetails != null)
                {
                    sFilePath = lstAttachmentDetails.file_location + lstAttachmentDetails.file_name;
                    if (!string.IsNullOrWhiteSpace(sFilePath))
                    {
                        deleteChk = new BLAttachment().DeleteAttachmentById(DocumentId);
                        if (deleteChk == 1)
                        {
                            ReqHelper.DeleteFileFromFTP(sFilePath);
                        }
                        else
                        {
                            response.status = StatusCodes.INVALID_REQUEST.ToString();
                            response.error_message = "File/Image Not Found!";
                            return response;
                        }


                    }
                    else
                    {
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.error_message = "Invalid File Path!";
                        return response;
                    }
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_155;
                    return response;
                }
                else
                {
                    response.status = StatusCodes.INVALID_REQUEST.ToString();
                    response.error_message = "File/Image Not Found!";
                    return response;
                }
            }

            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DeleteAttachment()", "Main", ex);
                response.status = StatusCodes.EXCEPTION.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_239;

            }
            //response.status = StatusCodes.OK.ToString();
            //response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_155;
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<List<ImageResult>> GetEntityImages(ReqInput data)
        {

            var response = new ApiResponse<List<ImageResult>>();

            try
            {
                GetEntityImagesIn objGetEntityImagesIn = ReqHelper.GetRequestData<GetEntityImagesIn>(data);
                string FtpUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

                var lstImages = new BLAttachment().getEntityImages(objGetEntityImagesIn.entitySystemId, objGetEntityImagesIn.entityType, "Image");
                List<ImageResult> lstImageResult = new List<ImageResult>();

                foreach (var item in lstImages)
                {
                    var _imgSrc = "";
                    string imageUrl = string.Empty;

                    imageUrl = string.Concat(FtpUrl, item.file_location, "Thumb_" + item.file_name);
                    if (!isFileExistOnFTP(imageUrl))
                    {
                        imageUrl = string.Concat(FtpUrl, item.file_location, item.file_name);
                    }
                    //string imageUrl = string.Concat(FtpUrl, item.file_location,item.file_name);
                    WebClient request = new WebClient();
                    if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                        request.Credentials = new NetworkCredential(UserName, PassWord);

                    byte[] objdata = null;
                    objdata = request.DownloadData(imageUrl);
                    if (objdata != null && objdata.Length > 0)
                        _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));

                    lstImageResult.Add(new ImageResult()
                    {
                        ImgName = item.org_file_name,
                        ImgSrc = _imgSrc,
                        uploadedBy = item.uploaded_by,
                        ImgId = item.id,
                        created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                    });

                }
                response.status = StatusCodes.OK.ToString();
                response.results = lstImageResult;

            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("getEntityImages()", "Main", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
            }

            //var jsonResult = Json(objResp, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            return response;
        }
        public bool isFileExistOnFTP(string filepath)
        {
            var request = (FtpWebRequest)WebRequest.Create(filepath);
            string UserName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
            request.Credentials = new NetworkCredential(UserName, PassWord);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode ==
                    FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
                return false;
            }

        }




        [System.Web.Http.HttpPost]
        public ApiResponse<LibraryAttachment> DownloadAttachment(ReqInput data)
        {

            var response = new ApiResponse<LibraryAttachment>();

            try
            {
                LibraryAttachment objLibraryAttachment = new LibraryAttachment();

                DownloadAttachmentIn objDownloadAttachmentIn = ReqHelper.GetRequestData<DownloadAttachmentIn>(data);
                string FtpUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
                objLibraryAttachment = new BLAttachment().getEntityAttachmentDetails(objDownloadAttachmentIn.attachmentId);

                if (objLibraryAttachment != null)
                {
                    string attachmentUrl = string.Concat(FtpUrl, objLibraryAttachment.file_location, objLibraryAttachment.file_name);

                    WebClient request = new WebClient();
                    if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                        request.Credentials = new NetworkCredential(UserName, PassWord);

                    objLibraryAttachment.attachmentSource = request.DownloadData(attachmentUrl);
                    objLibraryAttachment.file_size_converted = ReqHelper.BytesToString(objLibraryAttachment.file_size);
                    response.status = StatusCodes.OK.ToString();
                    response.results = objLibraryAttachment;
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
                    response.results = objLibraryAttachment;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadAttachment()", "Main", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
            }
            return response;
        }

        #endregion;
        [System.Web.Http.HttpPost]
        public ApiResponse<DbMessage> NetworkStage(ReqInput data)
        {
            var response = new ApiResponse<DbMessage>();
            try
            {
                NetworkStage objIn = ReqHelper.GetRequestData<NetworkStage>(data);
                HeaderAttributes headerAttribute = ReqHelper.getHeaderValue(Request.Headers.ToList());
                var layerDetail = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objIn.entity_type.ToUpper()).FirstOrDefault() : null;
                if (headerAttribute.source_ref_id != "0" && headerAttribute.source_ref_type.ToUpper() != "NETWORK_TICKET")
                {
                    var updatenetwork = new BLNetworkStatus().UpdateNetworkStatus(objIn.systemid, objIn.entity_type, objIn.curr_status, objIn.user_id);
                    updatenetwork.message = BLConvertMLanguage.MultilingualMessageConvert(updatenetwork.message);
                    switch (objIn.old_status)
                    {
                        case "P": objIn.old_status = "Planned"; break;
                        case "A": objIn.old_status = "As Built"; break;
                        case "D": objIn.old_status = "Dormant"; break;
                    }
                    switch (objIn.curr_status)
                    {
                        case "P": objIn.curr_status = "Planned"; break;
                        case "A": objIn.curr_status = "As Built"; break;
                        case "D": objIn.curr_status = "Dormant"; break;
                    };
                    if (updatenetwork.status)
                    {
                        response.results = updatenetwork;
                        response.status = StatusCodes.OK.ToString();
                        //response.error_message = Convert.ToString(updatenetwork.message);
                        response.error_message = string.Format(BLConvertMLanguage.MultilingualMessageConvert(Resources.Resources.SI_ISP_GBL_JQ_GBL_021), layerDetail.layer_title, objIn.old_status, objIn.curr_status);

                    }
                    else
                    {
                        response.results = updatenetwork;
                        response.status = StatusCodes.FAILED.ToString();
                        response.error_message = StatusCodes.FAILED.ToString();
                    }
                }
                else
                {

                    EntityInfo objEntityInfo = ReqHelper.GetRequestData<EntityInfo>(data);
                    objEntityInfo.entity_action = "Network Status";
                    objEntityInfo.system_id = objIn.systemid;
                    objEntityInfo.attribute_info = data.data;
                    objEntityInfo.entity_type = objIn.entity_type;
                    objEntityInfo.source_ref_id = headerAttribute.source_ref_id;
                    objEntityInfo.source_ref_type = headerAttribute.source_ref_type.ToUpper();
                    switch (objIn.old_status)
                    {
                        case "P": objIn.old_status = "Planned"; break;
                        case "A": objIn.old_status = "As Built"; break;
                        case "D": objIn.old_status = "Dormant"; break;
                    }
                    switch (objIn.curr_status)
                    {
                        case "P": objIn.curr_status = "Planned"; break;
                        case "A": objIn.curr_status = "As Built"; break;
                        case "D": objIn.curr_status = "Dormant"; break;
                    };
                    var result = new BLNetworkTicket().EditEntityInfo(objEntityInfo); ;
                    if (result.status)
                    {

                        response.error_message = string.Format(BLConvertMLanguage.MultilingualMessageConvert(Resources.Resources.SI_ISP_GBL_JQ_GBL_021), layerDetail.layer_title, objIn.old_status, objIn.curr_status);
                        response.status = ResponseStatus.OK.ToString();
                        response.results = result;
                    }
                    else
                    {
                        response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
                        response.status = ResponseStatus.OK.ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("NetworkStage()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<ImpactDetail> GetDependentChildElements(ReqInput data)
        {
            var response = new ApiResponse<ImpactDetail>();
            try
            {
                ImpactDetailIn objImpactDetailIn = ReqHelper.GetRequestData<ImpactDetailIn>(data);
                ImpactDetail objImpactDetail = new ImpactDetail();
                var lstChildElements = new BLMisc().getDependentChildElements(objImpactDetailIn);
                objImpactDetail.ChildElements = lstChildElements;
                response.results = objImpactDetail;
                //add validationn for structure depended on shaft and floor
                if (!string.IsNullOrWhiteSpace(objImpactDetailIn.impactType) && objImpactDetailIn.impactType.ToUpper() == "MODIFICATION")
                {
                    // for  impactType=Modifcation
                    // SET moveConnected FLAG VALUE
                    // moveConnected WILL BE TRUE FOR THOSE ELEMENT WHICH HAVE DEPENDENCY 
                }
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetDependentChildElements()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }


        public void UpdateOtherGeometry(EditGeomIn geomObj)
        {
            if (geomObj.entityType.ToUpper() == EntityType.Tower.ToString().ToUpper())
            {
                List<SectorMaster> lstSectorMasters = new BLSector().GetSectorByTowerId(geomObj.systemId);
                foreach (var sectorMaster in lstSectorMasters)
                {
                    geomObj.systemId = sectorMaster.system_id;
                    geomObj.entityType = EntityType.Sector.ToString();
                    geomObj.geomType = GeometryType.Polygon.ToString();
                    geomObj.longLat = Common.GetSectorsGeometry(sectorMaster.latitude, sectorMaster.longitude, sectorMaster.azimuth, sectorMaster.sector_type);
                    var result = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);

                }
            }
        }

        #region ValidateRegionProvinceBoundary
        /// <summary>
        /// ValidateRegionProvinceBoundary
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<string> ValidateRegionProvinceBoundary(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                RegionProvinceRequest objProvince = ReqHelper.GetRequestData<RegionProvinceRequest>(data);
                var objValid = BLRegionProvince.Instance.ValidateRegionProvinceBoundary(objProvince.system_id, objProvince.entity_type, objProvince.geom, objProvince.action, objProvince.region_name, objProvince.region_abbreviation, objProvince.province_name, objProvince.province_abbreviation, objProvince.country);
                if (objValid.status == false)
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = BLConvertMLanguage.MultilingualMessageConvert(objValid.message);//objValid.message;
                }
                else
                {
                    response.status = ResponseStatus.OK.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ValidateRegionProvinceBoundary()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion

        #region GetGeometryForRegionProvinceBoundaryEdit
        /// <summary>
        /// GetGeometryForRegionProvinceBoundaryEdit
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<UpdateGeomtaryValue> GetRegionProvinceGeomDetails(ReqInput data)
        {
            var response = new ApiResponse<UpdateGeomtaryValue>();
            try
            {
                UpdateGeomtaryProperties objProvince = ReqHelper.GetRequestData<UpdateGeomtaryProperties>(data);

                var regProvfilters = "";
                regProvfilters = objProvince.boundarytype.ToUpper() == "REGION" ? "r.id in(" + objProvince.id + ") or p.id in (0)" : "r.id in(0) or p.id in (" + objProvince.id + ")";
                DataTable dt = GetRegionProvinceDetailForExport(regProvfilters, "KML");
                UpdateGeomtaryValue objRegionProvince = new UpdateGeomtaryValue();
                objRegionProvince.lstUpdateRegionProvince.Add(new UpdateGeomtaryProperties());
                objRegionProvince.lstUpdateRegionProvince[0].region_name = dt.Rows[0]["region_name"].ToString();
                objRegionProvince.lstUpdateRegionProvince[0].region_abbreviation = dt.Rows[0]["region_abbreviation"].ToString();
                objRegionProvince.lstUpdateRegionProvince[0].province_name = dt.Rows[0]["province_name"].ToString();
                objRegionProvince.lstUpdateRegionProvince[0].province_abbreviation = dt.Rows[0]["province_abbreviation"].ToString();
                objRegionProvince.lstUpdateRegionProvince[0].country_name = dt.Rows[0]["country_name"].ToString();
                objRegionProvince.lstUpdateRegionProvince[0].geomtext = dt.Rows[0]["geom"].ToString();
                objRegionProvince.lstUpdateRegionProvince[0].existing_id = Convert.ToInt32(objProvince.id);
                objRegionProvince.lstUpdateRegionProvince[0].IschkBoundary = dt.Rows.Count > 0 ? true : false;
                response.status = ResponseStatus.OK.ToString();
                response.results = objRegionProvince;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetRegionProvinceGeomDetails()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion

        #region Get Region Province Export Report
        /// <summary> GetRegionProvinceDetailForExport </summary>
        /// <param name="data">obj</param>
        /// <returns>Region Province Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        public DataTable GetRegionProvinceDetailForExport(string Filter, string ReportType)
        {
            ViewRegionProvinces objViewProvince = new ViewRegionProvinces();
            objViewProvince.lstViewRegionProvinceDetails = new BLRegionProvince().GetRegionProvinceList(objViewProvince.objGridAttributes, ReportType, Filter);

            DataTable dt = new DataTable();
            dt = MiscHelper.ListToDataTable(objViewProvince.lstViewRegionProvinceDetails);
            dt.TableName = "RegionProvinceDetails";
            return dt;
        }
        #endregion

        #region GetGeometryForEdit
        /// <summary>
        /// GetGeometryForEdit
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<GeometryDetail> getGeometryForEdit(ReqInput data)
        {
            var response = new ApiResponse<GeometryDetail>();
            try
            {
                ImpactDetailIn objImpactDetailIn = ReqHelper.GetRequestData<ImpactDetailIn>(data);
                var objGeometryDetail = new BLSearch().GetGeometryDetails(new GeomDetailIn { systemId = objImpactDetailIn.systemId.ToString(), entityType = objImpactDetailIn.entityType, geomType = objImpactDetailIn.geomType, user_id = objImpactDetailIn.user_id });
                if (objGeometryDetail.entity_type.ToLower() == "cable")
                {
                    var getCableType = BLCable.Instance.GetCableType(objGeometryDetail.entity_id);
                    objGeometryDetail.entity_sub_type = getCableType;
                    objGeometryDetail.lstTP = getExisitingTPDetails(objImpactDetailIn.systemId, objImpactDetailIn.entityType);
                }
                if (objGeometryDetail.entity_type.ToLower() == "trench" || objGeometryDetail.entity_type.ToLower() == "duct")
                {
                    objGeometryDetail.lstTP = getExisitingTPDetails(objImpactDetailIn.systemId, objImpactDetailIn.entityType);
                }
                if (objGeometryDetail.geometry_extent != null)
                {
                    ImpactDetail objImpactDetail = new ImpactDetail();
                    var lstChildElements = new BLMisc().getDependentChildElements(objImpactDetailIn);
                    objImpactDetail.ChildElements = lstChildElements;
                    objGeometryDetail.childElements = objImpactDetail;

                    var extent = objGeometryDetail.geometry_extent.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
                    string[] bounds = extent.Split(',');
                    string[] southWest = bounds[0].Split(' ');
                    string[] northEast = bounds[1].Split(' ');
                    objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
                    objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
                    response.results = objGeometryDetail;
                    response.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    response.status = ResponseStatus.ZERO_RESULTS.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                }
            }
            catch (Exception ex)
            {
                response.status = ResponseStatus.ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_162;
            }
            return response;

        }
        #endregion

        #region SaveEditGeometry
        /// <summary>
        /// SaveEditGeometry
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<dynamic> SaveEditGeometry(ReqInput data)
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                EditGeomIn geomObj = ReqHelper.GetRequestData<EditGeomIn>(data);
                var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == geomObj.entityType.ToUpper()).FirstOrDefault().layer_title;
				
				HeaderAttributes headerAttribute = ReqHelper.getHeaderValue(Request.Headers.ToList());
				if (headerAttribute.source_ref_id != "0" && headerAttribute.source_ref_type.ToUpper() != "NETWORK_TICKET")
				{
					var dicParentEntityDetail = new BLMisc().GetNetworkTicketIdByEntityId<Dictionary<string, string>>(geomObj.systemId, (Models.EntityType)Enum.Parse(typeof(Models.EntityType), geomObj.entityType));
					if(dicParentEntityDetail["source_ref_id"] !="")
                    {
						headerAttribute.source_ref_id = dicParentEntityDetail["source_ref_id"];
						headerAttribute.source_ref_type = dicParentEntityDetail["source_ref_type"];
					}                  				
				}

				geomObj.Bld_Buffer = SmartInventory.Settings.ApplicationSettings.Bld_Buffer_Mtr;
                headerAttribute.source_ref_type = headerAttribute.source_ref_type == null ? "" : headerAttribute.source_ref_type.ToUpper();
                if (headerAttribute.source_ref_id != "0" && headerAttribute.source_ref_type != "NETWORK_TICKET" || headerAttribute.is_new_entity)
                {
                    if (geomObj.entityType.ToLower() == "subarea")
                    {
                        var getSubArea = new BLMisc().GetEntityDetailById<SubArea>(geomObj.systemId, EntityType.SubArea);
                        //var getBuilding = BLBuilding.Instance.GetBuildingById(getSubArea.building_system_id);
                        if (getSubArea.building_system_id > 0 && !string.IsNullOrWhiteSpace(getSubArea.building_code))
                        {
                            //geomObj.systemId = getSubArea.building_system_id;
                            //  geomObj.entityType = EntityType.Building.ToString();
                            geomObj.systemId = getSubArea.system_id;
                            geomObj.entityType = EntityType.SubArea.ToString();
                            geomObj.geomType = "Polygon";
                            var chekValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
                            if (chekValidate.status == true)
                            {
                                var updateGeom = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                                if (updateGeom.status)
                                {
                                    geomObj.systemId = getSubArea.system_id;
                                    geomObj.entityType = EntityType.SubArea.ToString();
                                    geomObj.geomType = "Polygon";
                                    var checkValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
                                    if (!checkValidate.status)
                                    {
                                        response.status = ResponseStatus.FAILED.ToString();
                                        response.error_message = BLConvertMLanguage.MultilingualMessageConvert(chekValidate.message); //chekValidate.message;
                                        return response;
                                    }
                                    var result = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                                    response.status = ResponseStatus.OK.ToString();
                                    var msg = BLConvertMLanguage.MultilingualMessageConvert(chekValidate.message);
                                    response.error_message = layer_title + " " + msg;
                                }

                            }
                            else
                            {
                                response.status = ResponseStatus.FAILED.ToString();
                                response.error_message = BLConvertMLanguage.MultilingualMessageConvert(chekValidate.message); //chekValidate.message;
                            }
                        }
                        else
                        {
                            var chkValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
                            if (chkValidate.status == true)
                            {
                                var updateGeom = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                                response.status = ResponseStatus.OK.ToString();
                                //Validate JP Boundary----Start-----
                                //if (geomObj.entityType.ToUpper() == EntityType.SubArea.ToString().ToUpper())
                                //{
                                //    response = (dynamic)JPBoundaryValidation(geomObj.longLat, geomObj.geomType);
                                //    if (response.status != StatusCodes.OK.ToString())
                                //    {
                                //        return response;
                                //    }
                                //    else
                                //    {
                                var msg = BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message);
                                response.error_message = layer_title + " " + msg;
                                // }
                                //}
                                //Validate JP Boundary----End-----

                            }
                            else
                            {
                                response.status = ResponseStatus.FAILED.ToString();
                                response.error_message = BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message);
                            }
                        }

                    }
                    else if (geomObj.entityType.ToLower() == "province")
                    {
                        UpdateGeomtaryProperties objInfo = new UpdateGeomtaryProperties();
                        objInfo.boundarytype = EntityType.Province.ToString();
                        objInfo.existing_id = geomObj.systemId;
                        objInfo.geomtext = geomObj.longLat;
                        objInfo.entryStatus = "EDIT";
                        objInfo.created_by = geomObj.userId;
                        objInfo.province_abbreviation = string.Empty;
                        objInfo.province_name = string.Empty;
                        objInfo.region_abbreviation = string.Empty;
                        objInfo.region_name = string.Empty;
                        objInfo.shapefilepath = string.Empty;
                        List<Status> resp = BLRegionProvince.Instance.SaveRegionProvinceGeomatery(objInfo);
                        response.status = ResponseStatus.OK.ToString(); ;
                        response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resp[0].message);
                        if (resp[0].status == "failed")
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                        }

                    }
                    else
                    {
                        EditLineTP objTPDetail = new EditLineTP();
                        objTPDetail.entity_type = geomObj.entityType;
                        objTPDetail.system_id = geomObj.systemId;
                        objTPDetail.tpDetail = geomObj.tpDetail;
                        objTPDetail.entityGeom = geomObj.longLat;

                        var chkValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
                        if (geomObj.entityType.ToLower() == EntityType.Cable.ToString().ToLower())
                        {
                            PageMessage objPageValidate = new PageMessage();
                            DbMessage objMessage = new DbMessage();
                            // workaround.. need to check it thoroughly..
                            //if (ApplicationSettings.isterminationpointenable)
                            //{
                            objMessage = new BLMisc().validateEntity(new validateEntity
                            {
                                system_id = geomObj.systemId,
                                entity_type = EntityType.Cable.ToString(),
                                a_system_id = geomObj.tpDetail[0].system_id,
                                a_entity_type = geomObj.tpDetail[0].entity_type,
                                b_system_id = geomObj.tpDetail[1].system_id,
                                b_entity_type = geomObj.tpDetail[1].entity_type
                            }, false);
                            // }

                            if (!string.IsNullOrEmpty(objMessage.message))
                            {
                                response.status = ResponseStatus.FAILED.ToString();
                                response.error_message = BLConvertMLanguage.MultilingualMessageConvert(objMessage.message);//objMessage.message;
                                return response;
                            }
                            else
                            {
                                if (objTPDetail.entity_type == "Cable")
                                {
                                    objTPDetail = BLCable.Instance.EditCableTPDetail(objTPDetail, geomObj.userId);
                                    var startPoint = geomObj.tpDetail.Where(m => m.mode == "start").FirstOrDefault();
                                    var endPoint = geomObj.tpDetail.Where(m => m.mode == "end").FirstOrDefault();
                                    if (!string.IsNullOrEmpty(startPoint.actualLatLng)) { geomObj.longLat = startPoint.actualLatLng + "," + geomObj.longLat; }
                                    if (!string.IsNullOrEmpty(endPoint.actualLatLng)) { geomObj.longLat = geomObj.longLat + "," + endPoint.actualLatLng; }
                                }
                                if (!string.IsNullOrEmpty(objTPDetail.message))
                                {
                                    response.status = ResponseStatus.FAILED.ToString();
                                    response.error_message = objTPDetail.message;
                                    return response;
                                }
                            }
                        }


                        if (objTPDetail.entity_type == EntityType.Duct.ToString())
                        {
                            objTPDetail = BLDuct.Instance.EditDuctTPDetail(objTPDetail, geomObj.userId);
                            var startPoint = geomObj.tpDetail.Where(m => m.mode == "start").FirstOrDefault();
                            var endPoint = geomObj.tpDetail.Where(m => m.mode == "end").FirstOrDefault();
                            if (!string.IsNullOrEmpty(startPoint.actualLatLng)) { geomObj.longLat = startPoint.actualLatLng + "," + geomObj.longLat; }
                            if (!string.IsNullOrEmpty(endPoint.actualLatLng)) { geomObj.longLat = geomObj.longLat + "," + endPoint.actualLatLng; }
                            BASaveEntityGeometry.Instance.UpdateMicroductGeom(geomObj.systemId);

                        }
                        if (objTPDetail.entity_type == "Trench")
                        {
                            objTPDetail = BLTrench.Instance.EditTrenchTPDetail(objTPDetail, geomObj.userId);
                            var startPoint = geomObj.tpDetail.Where(m => m.mode == "start").FirstOrDefault();
                            var endPoint = geomObj.tpDetail.Where(m => m.mode == "end").FirstOrDefault();
                            if (!string.IsNullOrEmpty(startPoint.actualLatLng)) { geomObj.longLat = startPoint.actualLatLng + "," + geomObj.longLat; }
                            if (!string.IsNullOrEmpty(endPoint.actualLatLng)) { geomObj.longLat = geomObj.longLat + "," + endPoint.actualLatLng; }
                        }

                        if (!string.IsNullOrEmpty(objTPDetail.message))
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = objTPDetail.message;
                            return response;
                        }

                        if (chkValidate.status == true)
                        {


                            var updateGeom = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                            UpdateOtherGeometry(geomObj);
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = layer_title + " " + BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message);//chkValidate.message;
                        }
                        else
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message); //chkValidate.message;
                        }


                    }
                }
                else
                {
                    var chkValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
                    if (chkValidate.status == true)
                    {
                        EditLineTP objTPDetail = new EditLineTP();
                        objTPDetail.entity_type = geomObj.entityType;
                        objTPDetail.system_id = geomObj.systemId;
                        objTPDetail.tpDetail = geomObj.tpDetail;
                        objTPDetail.entityGeom = geomObj.longLat;

                        if (objTPDetail.entity_type == "Cable")
                        {
                            var objCable = new BLMisc().GetEntityDetailById<CableMaster>(geomObj.systemId, EntityType.Cable, geomObj.userId);
                            if (objCable.is_new_entity)
                            { objTPDetail = BLCable.Instance.EditCableTPDetail(objTPDetail, geomObj.userId); }
                            var startPoint = geomObj.tpDetail.Where(m => m.mode == "start").FirstOrDefault();
                            var endPoint = geomObj.tpDetail.Where(m => m.mode == "end").FirstOrDefault();
                            if (!string.IsNullOrEmpty(startPoint.actualLatLng)) { geomObj.longLat = startPoint.actualLatLng + "," + geomObj.longLat; }
                            if (!string.IsNullOrEmpty(endPoint.actualLatLng)) { geomObj.longLat = geomObj.longLat + "," + endPoint.actualLatLng; }
                        }
                        if (objTPDetail.entity_type == EntityType.Duct.ToString())
                        {
                            var objDuct = new BLMisc().GetEntityDetailById<DuctMaster>(geomObj.systemId, EntityType.Duct, geomObj.userId);
                            if (objDuct.is_new_entity)
                            { objTPDetail = BLCable.Instance.EditCableTPDetail(objTPDetail, geomObj.userId); }
                            objTPDetail = BLDuct.Instance.EditDuctTPDetail(objTPDetail, geomObj.userId);
                            var startPoint = geomObj.tpDetail.Where(m => m.mode == "start").FirstOrDefault();
                            var endPoint = geomObj.tpDetail.Where(m => m.mode == "end").FirstOrDefault();
                            if (!string.IsNullOrEmpty(startPoint.actualLatLng)) { geomObj.longLat = startPoint.actualLatLng + "," + geomObj.longLat; }
                            if (!string.IsNullOrEmpty(endPoint.actualLatLng)) { geomObj.longLat = geomObj.longLat + "," + endPoint.actualLatLng; }
                            BASaveEntityGeometry.Instance.UpdateMicroductGeom(geomObj.systemId);

                        }
                        if (objTPDetail.entity_type == "Trench")
                        {
                            var objTrench = new BLMisc().GetEntityDetailById<DuctMaster>(geomObj.systemId, EntityType.Trench, geomObj.userId);
                            if (objTrench.is_new_entity)
                            { objTPDetail = BLCable.Instance.EditCableTPDetail(objTPDetail, geomObj.userId); }
                            objTPDetail = BLTrench.Instance.EditTrenchTPDetail(objTPDetail, geomObj.userId);
                            var startPoint = geomObj.tpDetail.Where(m => m.mode == "start").FirstOrDefault();
                            var endPoint = geomObj.tpDetail.Where(m => m.mode == "end").FirstOrDefault();
                            if (!string.IsNullOrEmpty(startPoint.actualLatLng)) { geomObj.longLat = startPoint.actualLatLng + "," + geomObj.longLat; }
                            if (!string.IsNullOrEmpty(endPoint.actualLatLng)) { geomObj.longLat = geomObj.longLat + "," + endPoint.actualLatLng; }
                        }

                        EntityInfo objEntityInfo = ReqHelper.GetRequestData<EntityInfo>(data);
                        objEntityInfo.entity_action = "Edit Location";
                        objEntityInfo.system_id = geomObj.systemId;
                        objEntityInfo.user_id = geomObj.userId;
                        objEntityInfo.attribute_info = data.data;
                        objEntityInfo.entity_type = geomObj.entityType;
                        objEntityInfo.geometry = geomObj.longLat;
                        objEntityInfo.source_ref_id = headerAttribute.source_ref_id;
                        objEntityInfo.source_ref_type = headerAttribute.source_ref_type == null ? "" : headerAttribute.source_ref_type.ToUpper();

                        var updateGeom = new BLNetworkTicket().EditEntityInfo(objEntityInfo);
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = layer_title + " " + BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message);//chkValidate.message;
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message); //chkValidate.message;
                    }
                }
            }

            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveEditGeometry()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region ValidateEntityGeom
        /// <summary>
        /// ValidateEntityGeom
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<string> ValidateEntityGeom(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                response.status = ResponseStatus.OK.ToString();
                ValidateEntityGeom obj = ReqHelper.GetRequestData<ValidateEntityGeom>(data);
                var objAreaValid = new BLMisc().ValidateEntityCreationArea(obj.txtGeom, obj.user_id, obj.geomType, obj.ticket_id);
                if (!objAreaValid.status)
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = BLConvertMLanguage.MultilingualMessageConvert(objAreaValid.message); //objAreaValid.message;
                    response.results = obj.entityType;
                    return response;
                }
                ////Restrict sales user to create entities...

                var userDetails = new BLUser().getUserDetails(obj.user_id);

                //if (userDetails.role_id == 3)
                //{
                //    response.status = ResponseStatus.FAILED.ToString();
                //    response.error_message = "You are not authorized to create entity!";
                //    response.results = obj.entityType;
                //    return response;
                //}
                if (obj.isTemplate)
                {
                    var chkIstemplate = new BLPoleItemMaster().ChkEntityTemplateExist(obj.entityType, obj.user_id, obj.subEntityType);
                    if (!chkIstemplate)
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_GBL_050;
                        response.results = obj.entityType;
                        return response;
                    }
                }
                DbMessage resp = new BLMisc().validateEntityGeom(obj.geomType, obj.entityType, obj.txtGeom, obj.user_id, obj.system_id);
                if (!resp.status)
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resp.message);
                }
                //if (ApplicationSettings.IsValidateJPBoundary == 1)
                //{
                //    if (resp.status)
                //    {
                //        if (obj.entityType.ToUpper() == EntityType.SubArea.ToString().ToUpper())
                //        {
                //            var res = JPBoundaryValidation(obj.txtGeom, obj.geomType);
                //            response.status = res.status;
                //            response.error_message = res.error_message;
                //            response.results = res.results;
                //        }
                //    }
                //}
                //var objRegPro = BLBuilding.Instance.GetRegionProvince(obj.txtGeom, obj.geomType);
                //if (objRegPro != null && objRegPro.Count == 1 && objRegPro[0].province_abbreviation != null || obj.entityType == EntityType.Cable.ToString() || obj.entityType == EntityType.Duct.ToString() || obj.entityType == EntityType.Trench.ToString())
                //{
                //if (obj.entityType == EntityType.Building.ToString())
                //{

                //	//var objSurveyArea = BLBuilding.Instance.GetSurveyAreaExist(txtGeom, geomType);
                //	//if (objSurveyArea == null)
                //	// {
                //	// objResp.status = ResponseStatus.FAILED.ToString();
                //	//objResp.message = "No Survey Area exist at this location!";
                //	//objResp.result = "surveyarea";
                //	//}
                //	// else
                //	//{
                //	var objBldValidate = BLBuilding.Instance.BldValidateByGeom(obj.geomType, obj.txtGeom, obj.user_id, SmartInventory.Settings.ApplicationSettings.Bld_Buffer_Mtr);
                //	if (objBldValidate.status == false)
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = String.Format(Resources.Resources.SI_OSP_BUL_NET_FRM_009, SmartInventory.Settings.ApplicationSettings.Bld_Buffer_Mtr);
                //		response.results = "surveyarea";
                //	}
                //	//}

                //}
                //else if (obj.entityType == EntityType.Structure.ToString())
                //{
                //	DbMessage dbMessageresponse = BLBuilding.Instance.ValidateStructureGeom(obj.txtGeom, obj.system_id);
                //	if (!dbMessageresponse.status)
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = BLConvertMLanguage.MultilingualMessageConvert(dbMessageresponse.message);//response.message;
                //		response.results = "building";
                //	}
                //}
                //else if (obj.entityType == EntityType.Area.ToString())
                //{
                //	string[] LayerName = { EntityType.Area.ToString() };
                //	bool chkAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.Area.ToString());
                //	if (chkAreaInterSect)
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_AR_NET_FRM_006, ApplicationSettings.listLayerDetails, LayerName);
                //		response.results = "Area";
                //	}
                //}
                //else if (obj.entityType == EntityType.DSA.ToString())
                //{
                //	string[] LayerName = { EntityType.DSA.ToString() };
                //	bool chkAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.DSA.ToString());
                //	if (chkAreaInterSect)
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_DSA_NET_FRM_003, ApplicationSettings.listLayerDetails, LayerName);
                //		response.results = "DSA";
                //	}
                //}
                //else if (obj.entityType == EntityType.CSA.ToString())
                //{
                //	var chkDsaExist = new BLCsa().GetDSAExist(obj.txtGeom);
                //	if (chkDsaExist != null && chkDsaExist.Count > 0)
                //	{
                //		string[] LayerName = { EntityType.CSA.ToString() };
                //		bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.CSA.ToString());
                //		if (chkSubAreaInterSect)
                //		{
                //			response.status = ResponseStatus.FAILED.ToString();
                //			response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CSA_NET_FRM_005, ApplicationSettings.listLayerDetails, LayerName);
                //		}
                //	}
                //	else
                //	{
                //		string[] LayerName = { EntityType.CSA.ToString(), EntityType.DSA.ToString() };
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CSA_NET_FRM_006, ApplicationSettings.listLayerDetails, LayerName);
                //	}
                //}
                //else if (obj.entityType == EntityType.ProjectArea.ToString())
                //{
                //	bool chkProjectAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.ProjectArea.ToString());
                //	if (chkProjectAreaInterSect)
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = Resources.Resources.SI_OSP_PA_NET_FRM_005;
                //		response.results = "Area";
                //	}
                //}
                //else if (obj.entityType == EntityType.SubArea.ToString())
                //{
                //	var chkAreaExist = new BLSubArea().GetAreaExist(obj.txtGeom);
                //	if (chkAreaExist != null && chkAreaExist.Count > 0)
                //	{
                //		bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.SubArea.ToString());
                //		if (chkSubAreaInterSect)
                //		{
                //			string[] LayerName = { EntityType.SubArea.ToString() };
                //			response.status = ResponseStatus.FAILED.ToString();
                //			response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SBA_NET_GBL_002, ApplicationSettings.listLayerDetails, LayerName);
                //		}
                //	}
                //	else
                //	{
                //		string[] LayerName = { EntityType.SubArea.ToString(), EntityType.Area.ToString() };
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SBA_NET_GBL_001, ApplicationSettings.listLayerDetails, LayerName);
                //	}
                //}
                ////else if (enType == EntityType.SurveyArea.ToString())
                ////{

                ////    bool chkSurveyAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.SurveyArea.ToString());
                ////    if (chkSurveyAreaInterSect)
                ////    {
                ////        objResp.status = ResponseStatus.FAILED.ToString();
                ////        objResp.message = "Selected boundary is overlapping other SurveyArea boundary!";
                ////    }

                ////}
                //else if (obj.entityType == EntityType.ADB.ToString())
                //{
                //	string[] LayerName = { EntityType.Area.ToString() };
                //	var chkSubAreaExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
                //	if (chkSubAreaExist.system_id == 0)
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
                //	}


                //}
                //else if (obj.entityType == EntityType.BDB.ToString())
                //{
                //	string[] LayerName = { EntityType.Area.ToString() };
                //	var chkBuildingExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
                //	if (chkBuildingExist.system_id == 0)
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_NET_FRM_211, ApplicationSettings.listLayerDetails, LayerName);
                //	}
                //}
                //else if (obj.entityType == EntityType.FDB.ToString())
                //{
                //	string[] LayerName = { EntityType.Area.ToString() };
                //	var chkBuildingExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
                //	if (chkBuildingExist.system_id == 0)
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_NET_FRM_211, ApplicationSettings.listLayerDetails, LayerName);
                //	}
                //}
                //else if (obj.entityType == EntityType.CDB.ToString())
                //{
                //	string[] LayerName = { EntityType.SubArea.ToString() };
                //	var chkSubAreaExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
                //	if (chkSubAreaExist.system_id == 0)
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
                //	}

                //}

                //else if (obj.entityType == EntityType.ONT.ToString() || obj.entityType == EntityType.HTB.ToString())
                //{

                //	var chkSubAreaExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
                //	if (chkSubAreaExist.system_id == 0)
                //	{
                //		string[] LayerName = { EntityType.Area.ToString() };
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
                //	}
                //}
                //else if (obj.entityType == EntityType.PIT.ToString())
                //{
                //	var chkROWExist = new BLROW().GetROWExist(obj.txtGeom);
                //	if (chkROWExist != null && chkROWExist.Count > 0)
                //	{
                //		bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.PIT.ToString());
                //		if (chkSubAreaInterSect)
                //		{
                //			response.status = ResponseStatus.FAILED.ToString();
                //			response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_228;
                //		}
                //	}
                //	else
                //	{
                //		response.status = ResponseStatus.FAILED.ToString();
                //		response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_229; ;
                //	}


                //}
                //else if (enType == EntityType.ROW.ToString())
                //{
                //    bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.ROW.ToString());
                //    if (chkSubAreaInterSect)
                //    {
                //        objResp.status = ResponseStatus.FAILED.ToString();
                //        objResp.message = "Selected boundary is overlapping other ROW boundary!";
                //    }
                //}

                //}
                //else
                //{
                //	response.status = ResponseStatus.FAILED.ToString();
                //	response.results = "region_province";

                //	if (objRegPro == null)
                //	{
                //		response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_230;
                //	}
                //	else if (objRegPro.Count > 1)
                //	{
                //		response.error_message = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_231, obj.entityType);
                //	}
                //	else if (objRegPro.Count == 1 && objRegPro[0].province_abbreviation == null)
                //	{
                //		response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_232;
                //	}
                //	else
                //	{
                //		response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_230;
                //	}
                //}

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ValidateEntityGeom()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion

        #region GetNetworkLayers
        /// <summary>
        /// GetNetworkLayers
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<NetworkLayersOut> GetNetworkLayers(ReqInput data)
        {
            var response = new ApiResponse<NetworkLayersOut>();
            try
            {
                NetworkLayersIn objNetworkLayersIn = ReqHelper.GetRequestData<NetworkLayersIn>(data);
                BLLayer objBLLayer = new BLLayer();
                var lstNetworkLayers = objBLLayer.GetMobileNetworkLayers(objNetworkLayersIn.User_Id, objNetworkLayersIn.Group_Id, objNetworkLayersIn.isLibraryElement);
                NetworkLayersOut MobileLayers = new NetworkLayersOut();
                var lstMobileNetworkLayers = lstNetworkLayers.Select(
                   m => new NetworkLayers
                   {
                       layerId = m.layerId,
                       layerName = m.layerName,
                       layerTitle = m.layerTitle,
                       networkIdType = m.networkIdType,
                       geomType = m.geomType,
                       mapAbbr = m.mapAbbr,
                       isDirectSave = m.isDirectSave,
                       isNetworkTypeRequired = m.isNetworkTypeRequired,
                       isTemplateRequired = m.isTemplateRequired,
                       is_visible_in_mobile_lib = m.is_visible_in_mobile_lib,
                       layerNetworkGroup = m.layerNetworkGroup,
                       templateId = m.templateId,
                       minZoomLevel = m.minZoomLevel,
                       maxZoomLevel = m.maxZoomLevel,
                       isLogicalViewEnabled = m.isLogicalViewEnabled,
                       isVirtualPortAllowed = m.isVirtualPortAllowed,
                       is_moredetails_enable = m.is_moredetails_enable,
                       planned_view = m.planned_view,
                       asbuild_view = m.asbuild_view,
                       dormant_view = m.dormant_view,
                       mapLayerSeq = m.maplayerseq,
                       is_visible_on_mobile_map = m.is_visible_on_mobile_map,
                       is_remark_required_from_mobile = m.is_remark_required_from_mobile,
                       is_shaft_element = m.is_shaft_element,
                       is_floor_element = m.is_floor_element,
                       is_isp_layer = m.is_isp_layer,
                       is_split_allowed = m.is_split_allowed,
                       is_association_enabled = m.is_association_enabled,
                       is_association_mandatory = m.is_association_mandatory,
                       is_network_ticket_entity = m.is_network_ticket_entity,
                       is_mobile_isp_layer = m.is_mobile_isp_layer,
                       is_offline_allowed = m.is_offline_allowed
                   }).ToList();
                var lstLandBaseLayers = objBLLayer.GetLandBaseLayres(objNetworkLayersIn.User_Id, 0);
                MobileLayers.lstNetworkLayers = lstMobileNetworkLayers;
                MobileLayers.lstLandBaseLayers = lstLandBaseLayers;
                MobileLayers.lstWMSWMTSLayers = objBLLayer.GetWMSWMTSLayres(objNetworkLayersIn.User_Id, 0);
                response.status = StatusCodes.OK.ToString();
                response.results = MobileLayers;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNetworkLayers()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region GetEntityAssociation
        /// <summary>
        /// GetEntityAssociation
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<AssociateLineEntity> GetEntityAssociation(ReqInput data)
        {
            var response = new ApiResponse<AssociateLineEntity>();
            try
            {
                AssociateEntityRequest objIn = ReqHelper.GetRequestData<AssociateEntityRequest>(data);
                AssociateLineEntity objLineAssociate = new AssociateLineEntity();
                objLineAssociate.parent_system_id = objIn.systemId;
                objLineAssociate.parent_entity_type = objIn.entityType;
                objLineAssociate.parent_network_id = objIn.networkId;
                var layerDetails = new BLLayer().getLayer(objIn.entityType);
                objLineAssociate.parent_multi_association = layerDetails.is_multi_association;
                objLineAssociate.listLineEntityInfo = new BLMisc().getLineEntityInLineBuffer(0, EntityType.Cable.ToString(), objIn.systemId, objIn.entityType, objIn.parentGeom, objIn.parentGeomType);
                if (objIn.systemId > 0)
                {
                    var buried = new BLMisc().checkIsBuried(objIn.systemId, objIn.entityType);
                    if (buried != null)
                    {
                        objLineAssociate.parent_is_buried = buried.status;
                    }
                }
                response.status = ResponseStatus.OK.ToString();
                response.results = objLineAssociate;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetLineEntityAssociation()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.ToString();
            }
            return response;
        }
        #endregion

        #region GetRouteAssociation
        /// <summary>
        /// GetEntityAssociation
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Arabind</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<AssociateRoute> GetRouteAssociation(ReqInput data)
        {
            var response = new ApiResponse<AssociateRoute>();
            try
            {
                AssociateRouteRequest objIn = ReqHelper.GetRequestData<AssociateRouteRequest>(data);
                AssociateRoute objLineAssociate = new AssociateRoute();
                objLineAssociate.parent_system_id = objIn.systemId;
                objLineAssociate.parent_entity_type = objIn.entityType;
                objLineAssociate.parent_network_id = objIn.networkId;
               
                objLineAssociate.parent_multi_association = true;
                objLineAssociate.listrouteInfo = new BLMisc().getRouteEntityInLineBuffer(objIn.systemId, objIn.entityType);
                
                response.status = ResponseStatus.OK.ToString();
                response.results = objLineAssociate;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetRouteAssociation()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.ToString();
            }
            return response;
        }
        #endregion

        #region ViewOtherEntityAssociation
        /// <summary>
        /// ViewOtherEntityAssociation
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<AssociateLineEntity> ViewOtherEntityAssociation(ReqInput data)
        {
            var response = new ApiResponse<AssociateLineEntity>();
            try
            {
                AssociateEntityRequest objIn = ReqHelper.GetRequestData<AssociateEntityRequest>(data);
                AssociateLineEntity objLineAssociate = new AssociateLineEntity();
                List<LineEntityInfo> objLineEntity = new List<LineEntityInfo>();
                objLineAssociate.parent_system_id = objIn.systemId;
                objLineAssociate.parent_entity_type = objIn.entityType;
                objLineAssociate.parent_network_id = objIn.networkId;
                objLineAssociate.listLineEntityInfo = new BLMisc().viewEntityAssociation(objIn.systemId, objIn.entityType);
                response.status = ResponseStatus.OK.ToString();
                response.results = objLineAssociate;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ViewOtherEntityAssociation()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region SaveEntityAssociate
        /// <summary>
        /// SaveEntityAssociate
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<AssociateLineEntity> SaveEntityAssociate(ReqInput data)
        {
            var response = new ApiResponse<AssociateLineEntity>();
            try
            {
                AssociateLineEntity objLineEntity = ReqHelper.GetRequestData<AssociateLineEntity>(data);
                var res = new BLMisc().saveLineEntityAssocition(JsonConvert.SerializeObject(objLineEntity.listLineEntityInfo), objLineEntity.parent_system_id, objLineEntity.parent_entity_type, objLineEntity.userId);
                objLineEntity.pageMsg.status = ResponseStatus.OK.ToString();
                objLineEntity.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_169;
                response.status = ResponseStatus.OK.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_169;
                response.results = objLineEntity;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveEntityAssociate()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion

        #region SaveRouteAssociate
        /// <summary>
        /// SaveEntityAssociate
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Arabind</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<AssociateRoute> SaveRouteAssociate(ReqInput data)
        {
            var response = new ApiResponse<AssociateRoute>();
            try
            {
                AssociateRoute objRoute = ReqHelper.GetRequestData<AssociateRoute>(data);
                var res = new BLMisc().saveRouteAssocition(JsonConvert.SerializeObject(objRoute.listrouteInfo), objRoute.parent_system_id, objRoute.parent_entity_type, objRoute.userId);
                objRoute.pageMsg.status = ResponseStatus.OK.ToString();
                objRoute.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_169;
                response.status = ResponseStatus.OK.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_169;
                response.results = objRoute;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveRouteAssociate()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion
        [System.Web.Http.HttpPost]
        public ApiResponse<AssociateLineEntity> AutoAssosiation(ReqInput data)
        {
            var response = new ApiResponse<AssociateLineEntity>();
            try
            {
                AssociateLineEntity objLineEntity = ReqHelper.GetRequestData<AssociateLineEntity>(data);
                BulkAssociationRequestLog objbulkAssociationLog = new BulkAssociationRequestLog();
                objbulkAssociationLog.created_on = DateTime.Now;
                objbulkAssociationLog.user_id = objLineEntity.userId;
                objbulkAssociationLog.subarea_system_id = objLineEntity.parent_system_id;
                var InsertLog = new BLBulkAssociationRequestLog().SaveBulkAssociationLog(objbulkAssociationLog);
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                     new BLMisc().saveLineEntityAutoAssocition(objLineEntity.parent_system_id, objLineEntity.parent_entity_type, objLineEntity.userId);
                }).ContinueWith(tsk =>
                {
                    tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("AutoAssosiation", "Main", ex); return true; });
                }, TaskContinuationOptions.OnlyOnFaulted);
                //objLineEntity.pageMsg.status =  res.status.ToString();
                //objLineEntity.pageMsg.message = res.message;
                //objLineEntity.pageMsg.logData = res.result;
                //response.status = res.status.ToString();
                //response.error_message = res.message;
                response.results = objLineEntity;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("AutoAssosiation()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }

        #region TerminationEntity
        /// <summary>
        /// TerminationEntity
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Sumit Poonia</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<List<TerminationPointDtl>> TerminationEntity(ReqInput data)
        {
            var response = new ApiResponse<List<TerminationPointDtl>>();
            try
            {
                TerminationEntityRequest objIn = ReqHelper.GetRequestData<TerminationEntityRequest>(data);
                List<TerminationPointDtl> objResp = new List<TerminationPointDtl>();
                objResp = new BLMisc().GetTerminationDetail(objIn.txtGeom, objIn.mtrBuffer, objIn.entityType, objIn.userId);
                response.results = objResp;
                response.status = ResponseStatus.OK.ToString();
                return response;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("TerminationEntity()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region Is Valid RFS
        /// <summary> isValidRFSStatus </summary>
        /// <param>old_rfs_status,new_rfs_status,entityType</param>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        public ApiResponse<BuildingMaster> isValidRFSStatus(ReqInput data)
        {
            var response = new ApiResponse<BuildingMaster>();
            try
            {
                BuildingMaster objRFS = ReqHelper.GetRequestData<BuildingMaster>(data);
                JsonResponse<string> objResp = new JsonResponse<string>();
                if (new BLBuildingRFS().isValidRFSStatus(objRFS.old_rfs_status, objRFS.new_rfs_status, objRFS.entityType))
                {
                    response.status = ResponseStatus.OK.ToString();
                    response.results = objRFS;
                }
                else
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = string.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_306, objRFS.old_rfs_status, objRFS.new_rfs_status);
                }
                return response;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("isValidRFSStatus()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_307;
            }
            return response;
        }
        #endregion

        #region DeleteOrRevertEntity
        /// <summary>
        /// DeleteOrRevertEntity
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Rajesh Kumar</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<ImpactDetail> RevertEntityChanges(ReqInput data)
        {
            var response = new ApiResponse<ImpactDetail>();
            try
            {
                ImpactDetailIn objImpactDetailIn = ReqHelper.GetRequestData<ImpactDetailIn>(data);
                RevertEntity objRevertEntity = ReqHelper.GetRequestData<RevertEntity>(data);
                //ImpactDetail objImpactDetail = new ImpactDetail();
                //var lstChildElements = new BLMisc().getDependentChildElements(objImpactDetailIn);
                //objImpactDetail.ChildElements = lstChildElements;
                //objRevertEntity.childElements = objImpactDetail;
                //if (lstChildElements.Count == 0)
                //{
                var result = new BLMisc().RevertEntityChanges(objRevertEntity);
                if (result.status)
                {
                    //response.error_message = result.message;
                    response.error_message = string.Format(result.message);
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.error_message = result.message;
                    response.status = StatusCodes.FAILED.ToString();
                    //response.error_message = StatusCodes.FAILED.ToString();
                }
                //}
                //else
                //{
                //    response.status = ResponseStatus.FAILED.ToString();
                //    response.error_message = string.Format(Resources.Resources.SI_OSP_SBA_JQ_FRM_003, "");
                //    response.results = objRevertEntity.childElements;
                //}
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("RevertEntityChanges()", "Main Controller", data.data, ex);
                response.status = ResponseStatus.ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_162;
            }
            return response;

        }
        #endregion

        [System.Web.Http.HttpPost]
        public ApiResponse<string> ValidateLBEntityGeom(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                response.status = ResponseStatus.OK.ToString();
                ValidateEntityGeom obj = ReqHelper.GetRequestData<ValidateEntityGeom>(data);
                var objAreaValid = new BLMisc().ValidateLBEntityCreationArea(obj.txtGeom, obj.user_id, obj.geomType, obj.ticket_id);
                if (!objAreaValid.status)
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = string.Format(BLConvertMLanguage.MultilingualMessageConvert(objAreaValid.message));//objAreaValid.message;
                    response.results = obj.entityType;
                    return response;
                }
                ////Restrict sales user to create entities...

                var userDetails = new BLUser().getUserDetails(obj.user_id);

                //if (obj.isTemplate)
                //{
                //    var chkIstemplate = new BLPoleItemMaster().ChkEntityTemplateExist(obj.entityType, obj.user_id, obj.subEntityType);
                //    if (!chkIstemplate)
                //    {
                //        response.status = ResponseStatus.FAILED.ToString();
                //        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_GBL_050;
                //        response.results = obj.entityType;
                //        return response;
                //    }
                //}
                var objRegPro = BLBuilding.Instance.GetLBRegionProvince(obj.txtGeom, obj.geomType);
                if (objRegPro != null && objRegPro.Count == 1 && objRegPro[0].province_abbreviation != null || obj.entityType == EntityType.LandBase.ToString())
                {
                    if (obj.entityType == EntityType.Building.ToString())
                    {

                        //var objSurveyArea = BLBuilding.Instance.GetSurveyAreaExist(txtGeom, geomType);
                        //if (objSurveyArea == null)
                        // {
                        // objResp.status = ResponseStatus.FAILED.ToString();
                        //objResp.message = "No Survey Area exist at this location!";
                        //objResp.result = "surveyarea";
                        //}
                        // else
                        //{
                        var objBldValidate = BLBuilding.Instance.BldValidateByGeom(obj.geomType, obj.txtGeom, obj.user_id, SmartInventory.Settings.ApplicationSettings.Bld_Buffer_Mtr);
                        if (objBldValidate.status == false)
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = String.Format(Resources.Resources.SI_OSP_BUL_NET_FRM_009, SmartInventory.Settings.ApplicationSettings.Bld_Buffer_Mtr);
                            response.results = "surveyarea";
                        }
                        //}

                    }
                    else if (obj.entityType == EntityType.Structure.ToString())
                    {
                        DbMessage dbMessageresponse = BLBuilding.Instance.ValidateStructureGeom(obj.txtGeom, obj.system_id);
                        if (!dbMessageresponse.status)
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = BLConvertMLanguage.MultilingualMessageConvert(dbMessageresponse.message);//response.message;
                            response.results = "building";
                        }
                    }
                    else if (obj.entityType == EntityType.Area.ToString())
                    {
                        string[] LayerName = { EntityType.Area.ToString() };
                        bool chkAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.Area.ToString());
                        if (chkAreaInterSect)
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_AR_NET_FRM_006, ApplicationSettings.listLayerDetails, LayerName);
                            response.results = "Area";
                        }
                    }
                    else if (obj.entityType == EntityType.DSA.ToString())
                    {
                        string[] LayerName = { EntityType.DSA.ToString() };
                        bool chkAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.DSA.ToString());
                        if (chkAreaInterSect)
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_DSA_NET_FRM_003, ApplicationSettings.listLayerDetails, LayerName);
                            response.results = "DSA";
                        }
                    }
                    else if (obj.entityType == EntityType.CSA.ToString())
                    {
                        var chkDsaExist = new BLCsa().GetDSAExist(obj.txtGeom);
                        if (chkDsaExist != null && chkDsaExist.Count > 0)
                        {
                            string[] LayerName = { EntityType.CSA.ToString() };
                            bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.CSA.ToString());
                            if (chkSubAreaInterSect)
                            {
                                response.status = ResponseStatus.FAILED.ToString();
                                response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CSA_NET_FRM_005, ApplicationSettings.listLayerDetails, LayerName);
                            }
                        }
                        else
                        {
                            string[] LayerName = { EntityType.CSA.ToString(), EntityType.DSA.ToString() };
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CSA_NET_FRM_006, ApplicationSettings.listLayerDetails, LayerName);
                        }
                    }
                    else if (obj.entityType == EntityType.ProjectArea.ToString())
                    {
                        bool chkProjectAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.ProjectArea.ToString());
                        if (chkProjectAreaInterSect)
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = Resources.Resources.SI_OSP_PA_NET_FRM_005;
                            response.results = "Area";
                        }
                    }
                    else if (obj.entityType == EntityType.SubArea.ToString())
                    {
                        var chkAreaExist = new BLSubArea().GetAreaExist(obj.txtGeom);
                        if (chkAreaExist != null && chkAreaExist.Count > 0)
                        {
                            bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.SubArea.ToString());
                            if (chkSubAreaInterSect)
                            {
                                string[] LayerName = { EntityType.SubArea.ToString() };
                                response.status = ResponseStatus.FAILED.ToString();
                                response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SBA_NET_GBL_002, ApplicationSettings.listLayerDetails, LayerName);
                            }
                        }
                        else
                        {
                            string[] LayerName = { EntityType.SubArea.ToString(), EntityType.Area.ToString() };
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SBA_NET_GBL_001, ApplicationSettings.listLayerDetails, LayerName);
                        }
                    }
                    else if (obj.entityType == EntityType.ADB.ToString())
                    {
                        string[] LayerName = { EntityType.Area.ToString() };
                        var chkSubAreaExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
                        if (chkSubAreaExist.system_id == 0)
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
                        }


                    }
                    else if (obj.entityType == EntityType.BDB.ToString())
                    {
                        string[] LayerName = { EntityType.Area.ToString() };
                        var chkBuildingExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
                        if (chkBuildingExist.system_id == 0)
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_NET_FRM_211, ApplicationSettings.listLayerDetails, LayerName);
                        }
                    }
                    else if (obj.entityType == EntityType.FDB.ToString())
                    {
                        string[] LayerName = { EntityType.Area.ToString() };
                        var chkBuildingExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
                        if (chkBuildingExist.system_id == 0)
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_NET_FRM_211, ApplicationSettings.listLayerDetails, LayerName);
                        }
                    }
                    else if (obj.entityType == EntityType.CDB.ToString())
                    {
                        string[] LayerName = { EntityType.SubArea.ToString() };
                        var chkSubAreaExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
                        if (chkSubAreaExist.system_id == 0)
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
                        }

                    }

                    else if (obj.entityType == EntityType.ONT.ToString() || obj.entityType == EntityType.HTB.ToString())
                    {

                        var chkSubAreaExist = new BLMisc().GetNetworkDetails(obj.txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
                        if (chkSubAreaExist.system_id == 0)
                        {
                            string[] LayerName = { EntityType.Area.ToString() };
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
                        }
                    }
                    else if (obj.entityType == EntityType.PIT.ToString())
                    {
                        var chkROWExist = new BLROW().GetROWExist(obj.txtGeom);
                        if (chkROWExist != null && chkROWExist.Count > 0)
                        {
                            bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + obj.txtGeom + "))", EntityType.PIT.ToString());
                            if (chkSubAreaInterSect)
                            {
                                response.status = ResponseStatus.FAILED.ToString();
                                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_228;
                            }
                        }
                        else
                        {
                            response.status = ResponseStatus.FAILED.ToString();
                            response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_229; ;
                        }


                    }

                }
                else
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.results = "region_province";

                    if (objRegPro == null)
                    {
                        response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_230;
                    }
                    else if (objRegPro.Count > 1)
                    {
                        response.error_message = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_231, obj.entityType);
                    }
                    else if (objRegPro.Count == 1 && objRegPro[0].province_abbreviation == null)
                    {
                        response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_232;
                    }
                    else
                    {
                        response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_230;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ValidateLBEntityGeom()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }

        #region SubmitNetwork
        /// <summary>
        /// Submit Network
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Rahul Tyagi</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<DbMessage> SubmitNetwork(ReqInput data)
        {
            var response = new ApiResponse<DbMessage>();
            try
            {
                SubmitNetworkParam objSubmitNetwork = ReqHelper.GetRequestData<SubmitNetworkParam>(data);
                HeaderAttributes headerAttribute = ReqHelper.getHeaderValue(Request.Headers.ToList());
                objSubmitNetwork.source = headerAttribute.source;
                var result = new BLMisc().SubmitNetwork(objSubmitNetwork);
                if (result.status)
                {
                    #region email sending start
                    if (objSubmitNetwork.source.ToLower() == "mobile")
                    {
                        List<Models.NetworkTicketEmailDetail> objTicketMaster = new BLNetworkTicket().GetNetworkTicketDetail(objSubmitNetwork.ticket_id);
                        if (objTicketMaster[0].ticketcategory.ToLower() == "planning" || objTicketMaster[0].ticketcategory.ToLower() == "construction" || objTicketMaster[0].ticketcategory.ToLower() == "survey")
                        {
                            string EventName = "";
                            if (objTicketMaster[0].ticketcategory.ToLower() == "survey")
                                EventName = EmailEventList.SurveySubmitted.ToString();
                            if (objTicketMaster[0].ticketcategory.ToLower() == "planning")
                                EventName = EmailEventList.DesignSubmitted.ToString();
                            if (objTicketMaster[0].ticketcategory.ToLower() == "construction")
                                EventName = EmailEventList.ImplementationCompleted.ToString();
                            Models.User objUser = new BLUser().GetUserDetailByID(objSubmitNetwork.user_id);
                            objUser.name = MiscHelper.Decrypt(objUser.name);
                            Dictionary<string, string> objDict = new Dictionary<string, string>();
                            objDict.Add("Date", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));
                            objDict.Add("User", objUser.name);
                            objDict.Add("Comments", "Approved");
                            BLUser objBLuser = new BLUser();
                            List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EventName);
                            System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, EmailSettings.AllEmailSettings, null, objTicketMaster[0].projectname, EventName));
                            //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, null, objTicketMaster[0].projectname);


                        }

                    }
                    #endregion


                    response.results = result;
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.results = result;
                    response.status = StatusCodes.FAILED.ToString();
                    response.error_message = StatusCodes.FAILED.ToString();
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SubmitNetwork()", "Main Controller", data.data, ex);
                response.status = ResponseStatus.ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
            }
            return response;

        }

        #endregion

        #region UserDashboard
        [System.Web.Http.HttpPost]
        public ApiResponse<List<UserDashboard>> UserDashboard(ReqInput data)
        {
            var response = new ApiResponse<List<UserDashboard>>();
            try
            {
                UserDashboardParam objUD = ReqHelper.GetRequestData<UserDashboardParam>(data);
                List<UserDashboard> _obj = new List<UserDashboard>();
                var lstobj = new BLMisc().GetTeamStatus(objUD.manager_id);

                foreach (var item in lstobj)
                {
                    _obj.Add(new UserDashboard()
                    {
                        user_email = MiscHelper.Decrypt(item.user_email),
                        user_name = MiscHelper.Decrypt(item.user_name),
                        user_id = item.user_id,
                        last_login_time = item.last_login_time,
                        last_logout_time = item.last_logout_time,
                        last_tracking = item.last_tracking,
                        latitude = item.latitude,
                        longitude = item.longitude,
                        ticket_id = item.ticket_id,
                        ticket_status = item.ticket_status,
                    });
                }
                response.results = _obj;
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UserDashboard()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Getting User Dashboard Details!";
            }
            return response;
        }
        #endregion

        [System.Web.Http.HttpPost]
        public ApiResponse<ItemSpecificationResult> ValidateEntityForConversion(ReqInput data)
        {
            var response = new ApiResponse<ItemSpecificationResult>();
            EntityForConversionIn obj = ReqHelper.GetRequestData<EntityForConversionIn>(data);
            ItemSpecificationResult res = new ItemSpecificationResult();
            //get parent detail..
            var EntityDetail = new BLMisc().GetEntityDetailById<Dictionary<string, string>>(obj.system_id, (EntityType)Enum.Parse(typeof(EntityType), obj.current_entity_type));
            //set geometry value as parent..
            res.geom = EntityDetail["longitude"] + " " + EntityDetail["latitude"];
            res.vendor_id = Convert.ToInt32(EntityDetail["vendor_id"]);
            res.specification = EntityDetail["specification"];
            res.vendor_name = EntityDetail["vendor_name"];
            res.no_of_ports = obj.current_entity_type == "SpliceClosure" ? Convert.ToInt32(EntityDetail["no_of_ports"]) : Convert.ToInt32(EntityDetail["no_of_port"]);
            res.item_code = EntityDetail["item_code"];
            var entity_type = obj.current_entity_type == "SpliceClosure" ? "CDB" : "SpliceClosure";
            var objResult = new BLVendorSpecification().GetEntityTemplateDetails(res.no_of_ports, entity_type, res.vendor_id);
            if (objResult.Count > 0)
            {
                response.status = ResponseStatus.OK.ToString();
                response.results = res;
            }
            else
            {
                response.status = ResponseStatus.VALIDATION_FAILED.ToString();

                if (entity_type == "CDB")
                {
                    var Msg = "<table border='1' class='alertgrid'><tr><td><b>Vendor Name</b></td><td><b>Total Ports<b/></td></tr>";
                    Msg += "<tr><td>" + res.vendor_name + " </td><td> " + res.no_of_ports + "</td></tr>";
                    Msg += "</table>";
                    response.error_message = String.Format(Resources.Resources.SI_OSP_CDB_JQ_FRM_001, Msg);
                }
                else
                {
                    var Msg = "<table border='1' class='alertgrid'><tr><td><b>Vendor Name</b></td><td><b>Total Ports<b/></td></tr>";
                    Msg += "<tr><td>" + res.vendor_name + " </td><td> " + res.no_of_ports + "</td></tr>";
                    Msg += "</table>";
                    response.error_message = String.Format(Resources.Resources.SI_OSP_SC_JQ_FRM_001, Msg);
                }
            }
            return response;
        }

        [System.Web.Http.HttpPost]

        public ApiResponse<List<NECableDetails>> GetNearByCables(ReqInput data)
        {
            var response = new ApiResponse<List<NECableDetails>>();
            try
            {

                NearByCables objCable = ReqHelper.GetRequestData<NearByCables>(data);
                if (objCable.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }
                else if (objCable.latitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid latitude!";
                    return response;
                }
                else if (objCable.longitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid longitude!";
                    return response;
                }
                response.status = StatusCodes.OK.ToString();
                response.results = BLLoopMangment.Instance.GetNearByCableDetails(objCable.longitude, objCable.latitude, objCable.bufferInMtrs);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearByCables()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

        [System.Web.Http.HttpPost]

        public ApiResponse<List<NearByAssociatedEntities>> GetNearbyAssociatedEntity(ReqInput data)
        {
            var response = new ApiResponse<List<NearByAssociatedEntities>>();
            try
            {

                NearByEntities objEntities = ReqHelper.GetRequestData<NearByEntities>(data);
                response.status = StatusCodes.OK.ToString();
                response.results = new BLMisc().getnearbyassociatedentities(objEntities.entity_type, objEntities.geom);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearbyAssociatedEntity()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }


        public List<EditLineTPIn> getExisitingTPDetails(int systemId, string eType)
        {
            var response = new ApiResponse<List<EditLineTPIn>>();
            dynamic objEntityDetail = null;
            List<EditLineTPIn> lstTPs = new List<EditLineTPIn>();

            if (eType == "Cable")
                objEntityDetail = new BLMisc().GetEntityDetailById<CableMaster>(systemId, EntityType.Cable);
            else if (eType == "Duct")
                objEntityDetail = new BLMisc().GetEntityDetailById<DuctMaster>(systemId, EntityType.Duct);
            else if (eType == "Trench")
                objEntityDetail = new BLMisc().GetEntityDetailById<TrenchMaster>(systemId, EntityType.Trench);
            else if (eType == "Conduit")
                objEntityDetail = new BLMisc().GetEntityDetailById<ConduitMaster>(systemId, EntityType.Conduit);


            if (objEntityDetail != null)
            {
                var aEndGeom = new BLSearch().GetGeometryDetails(new GeomDetailIn { systemId = objEntityDetail.a_system_id.ToString(), entityType = objEntityDetail.a_entity_type, geomType = GeometryType.Point.ToString() }); //actualLatLng = (!string.IsNullOrEmpty(aEndGeom.longitude) ? aEndGeom.longitude + " " + aEndGeom.latitude : "") 
                var bEndGeom = new BLSearch().GetGeometryDetails(new GeomDetailIn { systemId = objEntityDetail.b_system_id.ToString(), entityType = objEntityDetail.b_entity_type, geomType = GeometryType.Point.ToString() }); //actualLatLng = (!string.IsNullOrEmpty(bEndGeom.longitude) ? bEndGeom.longitude + " " + bEndGeom.latitude : "")
                lstTPs.Add(new EditLineTPIn() { mode = "start", system_id = objEntityDetail.a_system_id, network_id = objEntityDetail.a_location, entity_type = objEntityDetail.a_entity_type, actualLatLng = (!string.IsNullOrEmpty(aEndGeom.longitude) ? aEndGeom.longitude + " " + aEndGeom.latitude : "") });
                lstTPs.Add(new EditLineTPIn() { mode = "end", system_id = objEntityDetail.b_system_id, network_id = objEntityDetail.b_location, entity_type = objEntityDetail.b_entity_type, actualLatLng = (!string.IsNullOrEmpty(bEndGeom.longitude) ? bEndGeom.longitude + " " + bEndGeom.latitude : "") });
            }
            else
            {
                lstTPs.Add(new EditLineTPIn() { mode = "start" });
                lstTPs.Add(new EditLineTPIn() { mode = "end" });
            }
            return lstTPs;
        }

        #region DeleteEntity
        /// <summary>
        /// DeleteEntity
        /// </summary>
        /// <param name="data"></param>
        /// <returns>DbMessage</returns>
        ///  <Created By>Rajesh Kumar</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<ImpactDetail> DeleteEntity(ReqInput data)
        {
            var response = new ApiResponse<ImpactDetail>();
            try
            {
                ImpactDetailIn objImpactDetailIn = ReqHelper.GetRequestData<ImpactDetailIn>(data);
                DeleteEntityFromInfo objDeleteEntityFromInfo = ReqHelper.GetRequestData<DeleteEntityFromInfo>(data);
                var userId = objDeleteEntityFromInfo.userId;
                var enType = objDeleteEntityFromInfo.entityType;
                var gType = objDeleteEntityFromInfo.geomType;
                var system_id = objDeleteEntityFromInfo.systemId;
                ImpactDetail objImpactDetail = new ImpactDetail();
                var lstChildElements = new BLMisc().getDependentChildElements(objImpactDetailIn);
                objImpactDetail.ChildElements = lstChildElements;
                objDeleteEntityFromInfo.childElements = objImpactDetail;
                if (lstChildElements.Count == 0)
                {
                    var result = new BLMisc().deleteEntity(system_id, enType, gType, userId);
                    if (result.status)
                    {
                        response.error_message = result.message;
                        response.status = StatusCodes.OK.ToString();
                    }
                    else
                    {
                        response.status = StatusCodes.FAILED.ToString();
                        response.error_message = result.message;
                    }
                }
                else
                {
                    response.error_message = "Following are the dependent elements. You need to remove them first";
                    response.status = ResponseStatus.FAILED.ToString();
                    response.results = objDeleteEntityFromInfo.childElements;

                }
            }
            catch (Exception ex)
            {
                response.status = ResponseStatus.ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_162;
            }
            return response;

        }
        #endregion

        #region GEOTAGGING IMAGES BY ANTRA
        [System.Web.Http.HttpPost]
        public ApiResponse<string> UploadGeoTaggedImages()
        {

            var response = new ApiResponse<string>();
            try
            {
                HttpPostedFile files = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
                if (HttpContext.Current.Request.Files.Count > 0 && files != null)
                {
                    var GeoImgName = HttpContext.Current.Request.Params["file_name"];
                    var attachmentType = HttpContext.Current.Request.Params["upload_type"];
                    var FileDescription = HttpContext.Current.Request.Params["file_description"];
                    var ImageLink = HttpContext.Current.Request.Params["image_link"];
                    var latitude = HttpContext.Current.Request.Params["latitude"];
                    var longitude = HttpContext.Current.Request.Params["longitude"];
                    var UserId = HttpContext.Current.Request.Params["userId"];
                    var validImageTypes = ApplicationSettings.validImageTypes.Split(new string[] { "," }, StringSplitOptions.None);
                    var isExist = false;

                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {

                        string FileName = files.FileName;
                        var fileExtension = Path.GetExtension(FileName);
                        if ((ApplicationSettings.MaxuploadFileSize < files.ContentLength / 1024 / 1024) && (attachmentType.ToUpper() == "IMAGE"))
                        {
                            response.error_message = String.Format(Resources.Resources.SI_OSP_GBL_JQ_GBL_112, ApplicationSettings.MaxuploadFileSize);
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }

                        if ((attachmentType != null && attachmentType.ToUpper() == "IMAGE") && !validImageTypes.Contains(fileExtension.ToLower()))
                        {
                            response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_059 + " extension:" + ApplicationSettings.validImageTypes;
                            response.status = StatusCodes.INVALID_FILE.ToString();
                            return response;
                        }
                        var lstDocument = new BLGeoTaggingAttachment().getGeoTaggingAttachmentDetailsbyId(attachmentType, FileName, Convert.ToInt32(UserId));
                        if (lstDocument.Count > 0)
                        {
                            response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_055;
                            response.status = StatusCodes.DUPLICATE_EXIST.ToString();
                            return response;
                        }
                        isExist = new BLGeoTaggingAttachment().CheckGeoFileExist(GeoImgName, attachmentType, fileExtension, Convert.ToInt32(UserId));
                        if (isExist)
                        {
                            response.error_message = attachmentType + " with same name already exist!";
                            response.status = StatusCodes.DUPLICATE_EXIST.ToString();
                            return response;
                        }
                        string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + Path.GetExtension(FileName);
                        string geomtext = "";
                        geomtext = longitude + " " + latitude;
                        List<InRegionProvince> objRegionProvince = new List<InRegionProvince>();
                        objRegionProvince = BLBuilding.Instance.GetRegionProvince(geomtext, GeometryType.Point.ToString());
                        if (objRegionProvince.Count <= 0)
                        {
                            response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_230;
                            response.status = StatusCodes.FAILED.ToString();
                            return response;
                        }
                        string strFilePath = "";
                        strFilePath = ReqHelper.UploadGeoTaggingfileOnFTP(attachmentType, UserId, strNewfilename, files);
                        GeoTaggingImages objAttachment = new GeoTaggingImages();
                        objAttachment.org_file_name = strNewfilename;
                        objAttachment.file_name = GeoImgName;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_location = strFilePath;
                        objAttachment.upload_type = attachmentType;
                        objAttachment.uploaded_by = Convert.ToInt32(UserId);
                        objAttachment.file_size = files.ContentLength;
                        objAttachment.uploaded_on = DateTime.Now;
                        objAttachment.region_id = objRegionProvince[0].region_id;
                        objAttachment.province_id = objRegionProvince[0].province_id;
                        objAttachment.latitude = Convert.ToDouble(latitude);
                        objAttachment.longitude = Convert.ToDouble(longitude);
                        objAttachment.file_description = FileDescription;
                        objAttachment.image_link = ImageLink;
                        objAttachment.thumbimage_location = strFilePath;

                        //Save Image on FTP and related detail in database..
                        var savefile = new BLGeoTaggingAttachment().SaveGeoTaggingAttachment(objAttachment);
                    }
                    response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_154;
                    response.status = StatusCodes.OK.ToString();
                    return response;
                }
                else
                {
                    response.error_message = "No attachment selected.";
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    return response;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("UploadGeoTaggingImages()", "Main", ex);
                response.error_message = "Error in uploading attachment!";
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                return response;
                //Error Logging...
            }
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<DocumentResult> DeleteGeoTaggedImage(ReqInput data)
        {
            var response = new ApiResponse<DocumentResult>();
            try
            {
                DeleteAttachmentsIn objDeleteAttachmentsIn = ReqHelper.GetRequestData<DeleteAttachmentsIn>(data);
                string sFilePath = "";
                int deleteChk = 0;
                int DocumentId = objDeleteAttachmentsIn.attachmentId;
                //Get File Name and Path...
                var lstAttachmentDetails = new BLGeoTaggingAttachment().getGeoDocumentById(DocumentId);
                if (lstAttachmentDetails != null)
                {
                    sFilePath = lstAttachmentDetails.file_location + lstAttachmentDetails.org_file_name;
                    if (!string.IsNullOrWhiteSpace(sFilePath))
                    {
                        deleteChk = new BLGeoTaggingAttachment().DeleteGeoAttachmentById(DocumentId);
                        if (deleteChk == 1)
                        {
                            ReqHelper.DeleteFileFromFTP(sFilePath);
                        }
                        else
                        {
                            response.status = StatusCodes.INVALID_REQUEST.ToString();
                            response.error_message = "File/Image Not Found!";
                            return response;
                        }


                    }
                    else
                    {
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.error_message = "Invalid File Path!";
                        return response;
                    }
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_155;
                    return response;
                }
                else
                {
                    response.status = StatusCodes.INVALID_REQUEST.ToString();
                    response.error_message = "File/Image Not Found!";
                    return response;
                }
            }

            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DeleteGeoTaggingImage()", "Main", ex);
                response.status = StatusCodes.EXCEPTION.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_239;

            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<List<GeoImageResult>> GetAllGeoTaggedImages(ReqInput data)
        {

            var response = new ApiResponse<List<GeoImageResult>>();

            try
            {
                GetGeoImagesIn objGetEntityImagesIn = ReqHelper.GetRequestData<GetGeoImagesIn>(data);
                string FtpUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

                var lstImages = new BLGeoTaggingAttachment().getGeoTaggedImages(objGetEntityImagesIn.user_id, "Image");
                List<GeoImageResult> lstImageResult = new List<GeoImageResult>();

                foreach (var item in lstImages)
                {
                    var _imgSrc = "";
                    string imageUrl = string.Empty;

                    imageUrl = string.Concat(FtpUrl, item.file_location, "Thumb_" + item.org_file_name);
                    if (!isFileExistOnFTP(imageUrl))
                    {
                        imageUrl = string.Concat(FtpUrl, item.file_location, item.org_file_name);
                    }
                    WebClient request = new WebClient();
                    if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                        request.Credentials = new NetworkCredential(UserName, PassWord);

                    byte[] objdata = null;
                    objdata = request.DownloadData(imageUrl);
                    if (objdata != null && objdata.Length > 0)
                        _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));

                    lstImageResult.Add(new GeoImageResult()
                    {
                        ImgName = item.file_name,
                        ImgSrc = _imgSrc,
                        uploadedBy = item.uploaded_by.ToString(),
                        ImgId = item.id,
                        created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                        latitude = item.latitude,
                        longitude = item.longitude,
                        FileExtension = item.file_extension,
                        FileLocation = item.file_location,
                        FileDescription = item.file_description,
                        UploadType = item.upload_type,
                        FileSize = item.file_size,
                        ThumbImgLocation = item.thumbimage_location,
                        RegionId = item.region_id,
                        ProvinceId = item.province_id,
                        ImageLink = item.image_link,

                    });

                }
                response.status = StatusCodes.OK.ToString();
                response.results = lstImageResult;

            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetAllGeoTaggingImages()", "Main", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
            }
            return response;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<GeoTaggingImages> DownloadGeoTaggedAttachment(ReqInput data)
        {

            var response = new ApiResponse<GeoTaggingImages>();

            try
            {
                GeoTaggingImages objGeoTaggedAttachment = new GeoTaggingImages();

                DownloadAttachmentIn objDownloadAttachmentIn = ReqHelper.GetRequestData<DownloadAttachmentIn>(data);
                string FtpUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
                objGeoTaggedAttachment = new BLGeoTaggingAttachment().getGeotaggingAttachmentDetails(objDownloadAttachmentIn.attachmentId);

                if (objGeoTaggedAttachment != null)
                {
                    string attachmentUrl = string.Concat(FtpUrl, objGeoTaggedAttachment.file_location, objGeoTaggedAttachment.org_file_name);

                    WebClient request = new WebClient();
                    if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                        request.Credentials = new NetworkCredential(UserName, PassWord);

                    objGeoTaggedAttachment.attachmentSource = request.DownloadData(attachmentUrl);
                    objGeoTaggedAttachment.file_size_converted = ReqHelper.BytesToString(objGeoTaggedAttachment.file_size);
                    response.status = StatusCodes.OK.ToString();
                    response.results = objGeoTaggedAttachment;
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
                    response.results = objGeoTaggedAttachment;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadGeoTaggedAttachment()", "Main", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
            }
            return response;
        }
        #endregion

        #region LANDBASE LOCATION EDIT BY SHAZIA
        [System.Web.Http.HttpPost]
        public ApiResponse<List<LandBaseDetail>> GetNearByLandbaseEntities(ReqInput data)
        {
            var response = new ApiResponse<List<LandBaseDetail>>();
            try
            {

                NearByEntitiesIn objEntityTemplateIn = ReqHelper.GetRequestData<NearByEntitiesIn>(data);
                if (objEntityTemplateIn.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }
                else if (objEntityTemplateIn.latitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid latitude!";
                    return response;
                }
                else if (objEntityTemplateIn.longitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid longitude!";
                    return response;
                }
                response.status = StatusCodes.OK.ToString();
                response.results = new BLLandBaseLayer().getNearByLandbaseEntities(objEntityTemplateIn.latitude, objEntityTemplateIn.longitude, objEntityTemplateIn.bufferInMtrs, objEntityTemplateIn.userId);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearByLandbaseEntities()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<Dictionary<string, string>> GetLandbaseEntityInfo(ReqInput data)
        {
            var response = new ApiResponse<Dictionary<string, string>>();
            try
            {
                LandbaseEntityInfo objEntityInfoIn = ReqHelper.GetRequestData<LandbaseEntityInfo>(data);
                string[] arrIgnoreColumns = { };
                if (objEntityInfoIn.systemId == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid system id!";
                    return response;
                }
                else if (string.IsNullOrEmpty(objEntityInfoIn.entityType))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Entity Type is required!";
                    return response;
                }
                var dicEntityInfo = new BLLandBaseLayer().getLandBaseEntityInfo(objEntityInfoIn.systemId, objEntityInfoIn.entityType, objEntityInfoIn.settingType);
                var tempDict = new Dictionary<string, string>();
                dicEntityInfo = BLConvertMLanguage.MultilingualConvertinfo(dicEntityInfo);
                if (objEntityInfoIn.geomType == GeometryType.Point.ToString())
                {
                    foreach (var item in dicEntityInfo)
                    {
                        if (item.Key.ToUpper() == "GEOMETRY")
                        {
                            var extent = item.Value.TrimStart("POINT(".ToCharArray()).TrimEnd(")".ToCharArray());
                            string[] lnglat = extent.Split(new string[] { " " }, StringSplitOptions.None);
                            tempDict.Add("Latitude", lnglat[1].ToString());
                            tempDict.Add("Longitude", lnglat[0].ToString());
                        }
                        else
                        {
                            tempDict.Add(item.Key, item.Value);
                        }
                    }
                }
                else
                {
                    tempDict = dicEntityInfo;
                }
                response.results = tempDict;
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetLandbaseEntityInfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<GeometryDetail> GetLandbaseGeometryForEdit(ReqInput data)
        {
            var response = new ApiResponse<GeometryDetail>();
            try
            {

                ImpactDetailIn objImpactDetailIn = ReqHelper.GetRequestData<ImpactDetailIn>(data);

                var objGeometryDetail = new BLLandBaseLayer().GetLandbaseGeometryDetails(new GeomDetailIn { systemId = objImpactDetailIn.systemId.ToString(), entityType = objImpactDetailIn.entityType, geomType = objImpactDetailIn.geomType });

                if (objGeometryDetail.geometry_extent != null)
                {
                    ImpactDetail objImpactDetail = new ImpactDetail();
                    var lstChildElements = new BLMisc().getDependentChildElements(objImpactDetailIn);
                    objImpactDetail.ChildElements = lstChildElements;
                    objGeometryDetail.childElements = objImpactDetail;

                    var extent = objGeometryDetail.geometry_extent.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
                    string[] bounds = extent.Split(',');
                    string[] southWest = bounds[0].Split(' ');
                    string[] northEast = bounds[1].Split(' ');
                    objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
                    objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
                    response.results = objGeometryDetail;
                    response.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    response.status = ResponseStatus.ZERO_RESULTS.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetLandbaseGeometryForEdit()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }


        public ApiResponse<string> SaveLandbaseEditGeometry(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                EditGeomIn geomObj = ReqHelper.GetRequestData<EditGeomIn>(data);
                geomObj.Bld_Buffer = SmartInventory.Settings.ApplicationSettings.Bld_Buffer_Mtr;


                var updateGeom = new BLLandBaseLayer().EditLandbaseEntityGeometry(geomObj);
                response.status = ResponseStatus.OK.ToString();
                response.results = geomObj.entityType + " " + "location updated successfully";//chkValidate.message;

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveLandbaseEditGeometry()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }

        #endregion

        #region REMARKS UPDATE BY SHAZIA
        [System.Web.Http.HttpPost]
        public ApiResponse<string> UpdateRemarks(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                UpdateRemarks values = ReqHelper.GetRequestData<UpdateRemarks>(data);

                // var remarks= string.Join(",", values.Param);

                var paras = String.Format("{0},{1},{2},{3},{4}", values.para1, values.para2, values.para3, values.para4, values.para5);
                string remarks = Regex.Replace(paras, ",{2,}", ",").Trim(',');
                new BLLayer().UpdateRemarks(values.systemId, values.entityType, values.networkId, remarks);
                response.status = ResponseStatus.OK.ToString();
                response.results = "Saved successfully";//chkValidate.message;


            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateRemarks()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region CPE Installation By ANTRA
        [System.Web.Http.HttpPost]
        public ApiResponse<CPEInstallation> CPEInstallation(ReqInput data)
        {
            var response = new ApiResponse<CPEInstallation>();
            try
            {
                CPEInstallationIn objCPEInstallationIn = ReqHelper.GetRequestData<CPEInstallationIn>(data);
                if (string.IsNullOrWhiteSpace(objCPEInstallationIn.source_entity_type))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Source Entity Type can not be null or empty!";
                    return response;
                }
                if (string.IsNullOrWhiteSpace(objCPEInstallationIn.source_network_id))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Source Network ID can not be null or empty!";
                    return response;
                }
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.WriteDebugLog("Line-1");

                var result = new BLMisc().SaveCPEInstallation(objCPEInstallationIn.source_network_id, objCPEInstallationIn.source_entity_type, objCPEInstallationIn.source_port_number, objCPEInstallationIn.user_id, objCPEInstallationIn.latitude, objCPEInstallationIn.longitude);
                if (result.status)
                {
                    response.error_message = result.message;
                    response.results = result;
                    logHelper.WriteDebugLog("Line-2");
                    return response;
                }
                else
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = result.message;
                    return response;
                }

                /*******END SAVE CABLE*************/
            }

            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("CPEInstallation()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        #endregion

        #region JP_Boundary
        public ApiResponse<dynamic> JPBoundaryValidation(string geom, string geom_type)
        {
            var response = new ApiResponse<dynamic>();
            response.status = ResponseStatus.OK.ToString();
            //get Centroid
            DbMessage objCentoidDetails = new BLMisc().GetCentroidByGeom(geom, geom_type);
            if (!objCentoidDetails.status)
            {
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Centroid not Found!";
                return response;
            }

            //get JP_BoundaryGeom
            DbMessage objJpBoundary = new BLMisc().GetJpBoundaryByGeom(objCentoidDetails.result);
            if (String.IsNullOrEmpty(objJpBoundary.result))
            {
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Drawn boundary must be within JP Boundary!";
            }
            else
            {
                //check within
                bool chkWithin = BASaveEntityGeometry.Instance.CheckGeomWithin(objJpBoundary.result, geom);
                if (chkWithin)
                {
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Drawn boundary must be within JP Boundary!";
                }
            }
            return response;
        }

        #endregion

        #region PUSH DATA TO GIS BY ANTRA

        public ApiResponse<string> PushToGis(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {

                BoundaryPushFilter obj = ReqHelper.GetRequestData<BoundaryPushFilter>(data);
                var objGISAPI = GetPushToGISResponse(obj,0);
                if (objGISAPI.addResults != null && objGISAPI.updateResults != null)
                {
                    if (objGISAPI.addResults.Count > 0)
                    {
                        var APIResponse = objGISAPI.addResults.FirstOrDefault();
                        if (APIResponse != null && APIResponse.success)
                        {
                            
                            var UpdateDetails = new BLSearch().UpdateEntityObjectId(APIResponse.objectId, obj.system_id, obj.entity_type);
                            response.status = StatusCodes.OK.ToString();
                            response.results = APIResponse.objectId.ToString();
                        }
                        else
                        {
                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            response.results = "Entity Creation Failed!";
                        }
                    }
                    else
                    {
                        var APIResponse = objGISAPI.updateResults.FirstOrDefault();
                        if (APIResponse != null && APIResponse.success)
                        {
                            var UpdateDetails = new BLSearch().UpdateEntityObjectId(APIResponse.objectId, obj.system_id, obj.entity_type);
                            response.status = StatusCodes.OK.ToString();
                            response.results = APIResponse.objectId.ToString();
                        }
                        else
                        {
                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            response.results = "Entity Updation Failed!";
                        }
                    }
                }
                else
                {
                    if (objGISAPI.error.details != null)
                    {
                        if (objGISAPI.error.details.Count > 0)
                        {
                            if (objGISAPI.error.details[0].ToUpper() == "OBJECT IS MISSING.")
                            {
                                var _objGISAPI = GetPushToGISResponse(obj, 1);
                                if (_objGISAPI.addResults.Count > 0)
                                {
                                    var APIResponse = _objGISAPI.addResults.FirstOrDefault();
                                    if (APIResponse != null && APIResponse.success)
                                    {
                                        var UpdateDetails = new BLSearch().UpdateEntityObjectId(APIResponse.objectId, obj.system_id, obj.entity_type);
                                        response.status = StatusCodes.OK.ToString();
                                        response.results = APIResponse.objectId.ToString();
                                    }
                                    else
                                    {
                                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                        response.results = "Entity Creation Failed!";
                                    }
                                }
                            }
                            else
                            {
                                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                                response.results = objGISAPI.error.message;
                                response.error_message = "Please contact to GIS administrator. " + objGISAPI.error.message;
                            }
                        }

                        else
                        {
                            response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            response.results = objGISAPI.error.message;
                            response.error_message = "Please contact to GIS administrator. " + objGISAPI.error.message;
                        }
                    }
                    else
                    {
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.results = objGISAPI.error.message;
                        response.error_message = "Please contact to GIS administrator. " + objGISAPI.error.message;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("PushToGis()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        public GISAPIResponse GetPushToGISResponse(BoundaryPushFilter obj, int _error = 0)
        {
            int objectId = 0;
            var GisDesignId = "";
            var layer_title = "";
            var URL = "";
            GISAttributes objGISAttributes = new GISAttributes();
            GISAPIResponse objRes = new GISAPIResponse();
            var objGeometryDetail = new BLSearch().GetGeometryDetailsToPush(obj);
            var objGISAttr = new BLSearch().GetGisAttributes<GISAttributes>(obj);
            if (objGISAttr != null)
            {
                if (objGISAttr.status.ToUpper() == "TEST")
                {
                    if (objGISAttr != null)
                    {
                        objGISAttributes = objGISAttr;
                    }
                    ArrayList latlngArray = new ArrayList();
                    ArrayList latlngArray2 = new ArrayList();

                    if (objGeometryDetail.Count > 0)
                    {
                        foreach (var item in objGeometryDetail)
                        {
                            double[] latlng = new double[2];
                            latlng[0] = item.longitude;
                            latlng[1] = item.latitude;
                            latlngArray2.Add(latlng);
                        }
                        var geomDetails = objGeometryDetail.FirstOrDefault();
                        objectId = geomDetails.objectId;
                        GisDesignId = geomDetails.gisdesignId;
                    }
                    latlngArray.Add(latlngArray2);
                    Ring ring = new Ring();
                    ring.rings = latlngArray;
                    GISSpatialReference ObjRefrences = new GISSpatialReference();
                    objGISAttributes.OBJECTID = (_error == 0 ? objectId : 0);
                    GISEntityRequest _objGISEntityRequest = new GISEntityRequest();
                    _objGISEntityRequest.geometry = ring;
                    _objGISEntityRequest.geometry.spatialReference = ObjRefrences;
                    _objGISEntityRequest.attributes = objGISAttributes;
                    ArrayList objGISEntityRequest = new ArrayList();


                    var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == obj.entity_type.ToString().ToUpper()).FirstOrDefault();
                    if (layerDetails.layer_name == EntityType.Area.ToString()) { layer_title = string.Concat("PSA", "_ID=", ' ', "'" + GisDesignId + "'"); }
                    if (layerDetails.layer_name == EntityType.SubArea.ToString()) { layer_title = string.Concat("FSA", "_ID=", ' ', "'" + GisDesignId + "'"); }
                    if (layerDetails.layer_name == EntityType.CSA.ToString()) { layer_title = string.Concat("CSA", "_ID=", ' ', "'" + GisDesignId + "'"); }
                    if (layerDetails.layer_name == EntityType.DSA.ToString()) { layer_title = string.Concat("DSA", "_ID=", ' ', "'" + GisDesignId + "'"); }
                    URL = new BLSearch().GetGISApi(obj.system_id, obj.entity_type, 1);
                    var objFetchGisData = WebAPIRequest.PostPartnerAPI<FetchGISData>(obj.user_id, GisDesignId, obj.system_id, obj.entity_type, "", URL, null, objGISAttributes.OBJECTID, "FetchGISData", layer_title, obj.transaction_id, obj.process_id);

                    if (objFetchGisData.features != null)
                    {
                        if (objFetchGisData.features.Count == 0)
                        {
                            objFetchGisData = WebAPIRequest.PostPartnerAPI<FetchGISData>(obj.user_id, GisDesignId, obj.system_id, obj.entity_type, "", URL, obj.VersionName, objGISAttributes.OBJECTID, "FetchGISData", layer_title, obj.transaction_id, obj.process_id);
                        }
                    }
                    if (objFetchGisData.features != null)
                    {
                        if (objFetchGisData.features.Count == 0)
                        {
                            URL = new BLSearch().GetGISApi(obj.system_id, obj.entity_type, 0);
                            objGISEntityRequest.Add(_objGISEntityRequest);
                            return WebAPIRequest.PostPartnerAPI<GISAPIResponse>(obj.user_id, GisDesignId, obj.system_id, obj.entity_type, objGISEntityRequest, URL, obj.VersionName, objGISAttributes.OBJECTID, "Form-Data", "adds", obj.transaction_id, obj.process_id);
                        }
                        var _URL = new BLSearch().GetGISApi(obj.system_id, obj.entity_type, 2);
                        objFetchGisData.features = objFetchGisData.features.OrderByDescending(m => m.attributes.CREATED_DATE).ToList();
                        var objFetchAttributes = objFetchGisData.features.FirstOrDefault();
                        Utility.MiscHelper.CopyMatchingGISProperties(objFetchAttributes.attributes, _objGISEntityRequest.attributes);
                        if (objFetchGisData.features.Count > 1)
                        {
                            for (int i = 1; i < objFetchGisData.features.Count; i++)
                            {
                                WebAPIRequest.PostPartnerAPI<GISAPIResponse>(obj.user_id, GisDesignId, obj.system_id, obj.entity_type, "", _URL, obj.VersionName, objFetchGisData.features[i].attributes.OBJECTID, "DELETE-DATA", "", obj.transaction_id, obj.process_id);
                            }
                        }
                    }
                    objGISEntityRequest.Add(_objGISEntityRequest);
                    URL = new BLSearch().GetGISApi(obj.system_id, obj.entity_type, 0);
                    return WebAPIRequest.PostPartnerAPI<GISAPIResponse>(obj.user_id, GisDesignId, obj.system_id, obj.entity_type, objGISEntityRequest, URL, obj.VersionName, objGISAttributes.OBJECTID, "Form-Data", "updates", obj.transaction_id, obj.process_id);
                }
                else
                {
                    GISAPIError objerror = new GISAPIError();
                    objerror.message = objGISAttr.message;
                    objRes.error = objerror;
                }
            }
            return objRes;
        }

        public ApiResponse<CreateVersionOut> GISCreateVersion(ReqInput data)
        {

            var response = new ApiResponse<CreateVersionOut>();
            try
            {
                var obj = ReqHelper.GetRequestData<CreateVersionIn>(data);
                var URL = ApplicationSettings.GISCreateVersion;
                response.results = WebAPIRequest.PostPartnerAPI<CreateVersionOut>(obj.user_id, obj.gis_design_id, obj.systemId, obj.entityType, obj, URL,"",-1,"","",obj.transaction_id, obj.process_id);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GISCreateVersion()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        public ApiResponse<PostVersionOut> GISPostVersion(ReqInput data)
        {

            var response = new ApiResponse<PostVersionOut>();
            try
            {
                var obj = ReqHelper.GetRequestData<PostVersionIn>(data);
                var URL = ApplicationSettings.GISPostVersion;
                response.results = WebAPIRequest.PostPartnerAPI<PostVersionOut>(obj.user_id, obj.gis_design_id, obj.systemId, obj.entityType, obj, URL,"",obj.objectId,"","", obj.transaction_id, obj.process_id);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GISPostVersion()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        #endregion

        #region CSA NE XML Copied To NAS BY ANTRA

        public NASStatusOut UpdateXmlNASStatus(ReqInput data)
        {
            var response = new ApiResponse<NASStatusOut>();
            try
            {
                var obj = ReqHelper.GetRequestData<NASStatusIn>(data);
                obj.gis_design_id = null;
                obj.entityType = null;
                obj.systemId = 0;
                var URL = ApplicationSettings.CSANEXMLToNASURL;
                var objGISAPI = WebAPIRequest.PostPartnerAPI<NASStatusOut>(obj.user_id, obj.gis_design_id, obj.systemId, obj.entityType, obj, URL, obj.process_id);
                response.results=objGISAPI;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateXmlNASStatus()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response.results;
        }

        public ApiResponse<NASStatusOut> NEXMLImportToNAS(ReqInput data)
        {
            var response = new ApiResponse<NASStatusOut>();
            try
            {
                var obj = ReqHelper.GetRequestData<NASStatusIn>(data);
                obj.gis_design_id = null;
                obj.entityType = null;
                obj.systemId = 0;
                var URL = ApplicationSettings.NEXMLImportToNAS;
                response.results = WebAPIRequest.PostPartnerAPI<NASStatusOut>(obj.user_id, obj.gis_design_id, obj.systemId, obj.entityType, obj, URL, "", 0, "NAS-FORM-DATA", obj.process_id);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("NEXMLImportToNAS()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region GET VOICE COMMAND JSON DATA
        [System.Web.Http.HttpPost]
        public ApiResponse<dynamic> GetVoiceCommands()
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                response.results = new BLVoiceCommandMaster().GetVoiceCommandJsonString();
                response.status = StatusCodes.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ApiResponse()", "Main Controller", "", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        [System.Web.Http.HttpPost]
        public ApiResponse<List<EntityDropDownItemList>> GetDropDownListParent(ReqInput data)
        {
            var response = new ApiResponse<List<EntityDropDownItemList>>();
            try
            {
                ItemDropDownIn objItemDropDownIn = ReqHelper.GetRequestData<ItemDropDownIn>(data);
                var ddlList = new BLMisc().GetDropDownListParent(objItemDropDownIn.entityType, objItemDropDownIn.ddlType).Select(m => (new EntityDropDownItemList { key = m.dropdown_key, value = m.dropdown_value })).ToList();
                if (ddlList.Count > 0)
                {
                    response.results = ddlList;
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "Record not found.";
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetDropDownListParent()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        #region GetRegionProvinceBasedOnLocation
        /// <summary>
        /// GetRegionProvinceBasedOnLocation
        /// </summary>
        /// <param name="data"></param>
        /// <returns>boolean</returns>
        ///  <Created By>Rahul Sharma</returns>
        [System.Web.Http.HttpPost]
        public ApiResponse<UserRegionProvince> GetRegionProvinceBasedOnLocation(ReqInput data)
        {
            var response = new ApiResponse<UserRegionProvince>();
            try
            {
                response.status = ResponseStatus.OK.ToString();
                UserRegionProvinceFilter obj = ReqHelper.GetRequestData<UserRegionProvinceFilter>(data);
                string txtGeom = obj.lng + " " + obj.lat;


                var objUserRegionProvince = new BLMisc().GetRegionProvinceBasedOnLocation(txtGeom, obj.userId);
                if (objUserRegionProvince != null && !string.IsNullOrEmpty(objUserRegionProvince.provincename))
                {
                    response.status = ResponseStatus.OK.ToString();
                    //response.error_message = BLConvertMLanguage.MultilingualMessageConvert(objAreaValid.message); //objAreaValid.message;
                    response.results = objUserRegionProvince;
                    return response;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                // logHelper.ApiLogWriter("ValidateEntityGeom()", "Main Controller", data.data, ex);
                logHelper.ApiLogWriter("GetRegionProvinceBasedOnLocation()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion

        [System.Web.Http.HttpPost]

        public ApiResponse<List<NEDuctDetails>> GetNearByDucts(ReqInput data)
        {
            var response = new ApiResponse<List<NEDuctDetails>>();
            try
            {

                NearByCables objCable = ReqHelper.GetRequestData<NearByCables>(data);
                if (objCable.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }
                else if (objCable.latitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid latitude!";
                    return response;
                }
                else if (objCable.longitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid longitude!";
                    return response;
                }
                response.status = StatusCodes.OK.ToString();
                response.results = BLSlack.Instance.GetNearByDuctDetails(objCable.longitude, objCable.latitude, objCable.bufferInMtrs);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetNearByDucts()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

    }

}
