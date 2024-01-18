using BusinessLogics;
using Models;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SmartInventory.Controllers
{
    public class LandBaseLayerController : Controller
    {
        // GET: LandBaseLayer
        public PartialViewResult AddLandBase(string networkIdType, string geomType, string centerLineGeom, bool _isBufferEnable, int systemId = 0, string geom = "")
        {
            LandBaseLayer objLandBaseLayer = new LandBaseLayer();
            objLandBaseLayer.buffer_geom = centerLineGeom; 
            
             objLandBaseLayer = GetLandBaseDetail(networkIdType, geomType, systemId, _isBufferEnable, geom);
            if (objLandBaseLayer.buffer_geom ==null)
            {
                objLandBaseLayer.buffer_geom = centerLineGeom;
            }
            objLandBaseLayer.geomType = geomType;
            objLandBaseLayer.userId = Convert.ToInt32(Session["user_id"]);
            return PartialView("AddLandBaseLayer", objLandBaseLayer);
        }
        public LandBaseLayer GetLandBaseDetail(string networkIdType, string geomType, int id, bool _isBufferEnable, string geom = "")
        {
            LandBaseLayer objLandBaseLayer = new LandBaseLayer();
            objLandBaseLayer.sp_geometry = geom;
            objLandBaseLayer.networkIdType = networkIdType; 
            var layerNameList = new BLLandBaseLayer().getlayerNamebyGeom(geomType, _isBufferEnable);
            objLandBaseLayer.lstlandBaseLyaerName = layerNameList;
            var objDDL = new BLMisc().GetDropDownList(EntityType.LandBase.ToString());
            DropDownMaster drp = new DropDownMaster(); 

            if (id == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objLandBaseLayer, geomType, geom);
                // Fill Parent detail...
                fillParentDetail(objLandBaseLayer, networkIdType, objLandBaseLayer.province_id);

                // fill latlong values for Point Geom
                if (geomType == GeometryType.Point.ToString())
                {
                    string[] lnglat = geom.Split(new string[] { " " }, StringSplitOptions.None);
                    objLandBaseLayer.latitude = Convert.ToDouble(lnglat[1].ToString());
                    objLandBaseLayer.longitude = Convert.ToDouble(lnglat[0].ToString());
                }

            }
            else
            {
               
                // Get entity detail by Id...
                //objLandBaseLayer = new BLLandBaseLayer().GetEntityDetailById(id);
                objLandBaseLayer = new BLLandBaseLayer().GetLandbaseEntityById(id,geomType);
                objLandBaseLayer.landbaseClassificationList = new BLLandBaseLayer().GetLandbaseDropdown(objLandBaseLayer.landbase_layer_id, "classification", 0);
                objLandBaseLayer.landbaseCategoryList = new BLLandBaseLayer().GetLandbaseDropdown(objLandBaseLayer.landbase_layer_id, "category", 0);
                if (objLandBaseLayer.category_id != 0)
                {
                objLandBaseLayer.landbaseSubCategoryList = new BLLandBaseLayer().GetLandbaseDropdown(objLandBaseLayer.landbase_layer_id, "subcategory", objLandBaseLayer.category_id);
                }


                objLandBaseLayer.lstlandBaseLyaerName = layerNameList;
                if (objLandBaseLayer.geomType == GeometryType.Point.ToString())
                {
                    string[] lnglat = objLandBaseLayer.sp_geometry.Split(new string[] { " " }, StringSplitOptions.None);
                    objLandBaseLayer.latitude = Convert.ToDouble(lnglat[1].ToString());
                    objLandBaseLayer.longitude = Convert.ToDouble(lnglat[0].ToString());
                }
            }

            return objLandBaseLayer;
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
        }
        private void fillParentDetail(dynamic objLib, string networkIdType, int province_id)
        {
            ////fill parent detail....
            //var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
            ////var objNetworkCodeDetail = new BLLandBaseLayer().GetNetworkId(objLandbaseNetworkCode);
            //if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
            //{
            //    if (networkIdType == NetworkIdType.M.ToString())
            //    {
            //        //FILL NETWORK CODE FORMAT FOR MANUAL
            //        objLib.network_id = networkCodeDetail.network_code;
            //    }
            //    objLib.parent_entity_type = networkCodeDetail.parent_entity_type;
            //    objLib.parent_network_id = networkCodeDetail.parent_network_id;
            //    objLib.parent_system_id = networkCodeDetail.parent_system_id;
            //}

            // GET Province Details
            var provinceinfo = new BLProvinceBoundary().getProvinceInfo(province_id);
            objLib.parent_entity_type = "Province";
            objLib.parent_network_id = provinceinfo.province_abbreviation;
            objLib.parent_system_id = provinceinfo.id;
        }

        public ActionResult SaveLandBaselayer(LandBaseLayer objLandBase, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage pageMsg = new PageMessage();
            //-- GET LAYER NAME BY ID--
            objLandBase.landbase_layer_id = objLandBase.landbase_layer_id;
            var layerDetails = new BLLandBaseLayer().getLayerNamebyId(objLandBase.landbase_layer_id);
            if (objLandBase.networkIdType == NetworkIdType.A.ToString() && objLandBase.id == 0)
            {
                LandbaseNetworkCodeIn objLandbaseNetworkCode = new LandbaseNetworkCodeIn();
                objLandbaseNetworkCode.landbase_layer_id = objLandBase.landbase_layer_id;
                objLandbaseNetworkCode.geomType = objLandBase.geomType;
                objLandbaseNetworkCode.sp_geometry = objLandBase.sp_geometry;
                objLandbaseNetworkCode.parent_entity_type = objLandBase.parent_entity_type;
                objLandbaseNetworkCode.parent_network_id = objLandBase.parent_network_id;
                objLandbaseNetworkCode.parent_system_id = objLandBase.parent_system_id;
                //GET AUTO NETWORK CODE...
                var objNetworkCodeDetail = new BLLandBaseLayer().GetNetworkId(objLandbaseNetworkCode);

                // var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Competitor.ToString(), gType = GeometryType.Point.ToString(), eGeom = objLandBase.sp_geometry });
                if (isDirectSave == true)
                {
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    objLandBase = GetLandBaseDetail(objLandBase.networkIdType, objLandBase.geomType, objLandBase.id,objLandBase._isBufferEnable, objLandBase.sp_geometry);
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    objLandBase.name = objNetworkCodeDetail.network_id;
                }
                //SET NETWORK CODE
                objLandBase.network_id = objNetworkCodeDetail.network_id;
                objLandBase.sequence_id = objNetworkCodeDetail.sequence_id;
                // fill latlong values
                if (objLandBase.geomType == GeometryType.Point.ToString())
                {
                    string[] lnglat = objLandBase.sp_geometry.Split(new string[] { " " }, StringSplitOptions.None);
                    objLandBase.latitude = Convert.ToDouble(lnglat[1].ToString());
                    objLandBase.longitude = Convert.ToDouble(lnglat[0].ToString());
                }
            }

            if (TryValidateModel(objLandBase))
            {
                string[] LayerName = { EntityType.LandBase.ToString() };
                var isNew = objLandBase.id > 0 ? false : true;
                var resultItem = new BLLandBaseLayer().SaveLandBase(objLandBase, Convert.ToInt32(Session["user_id"]));

                if (string.IsNullOrEmpty(resultItem.pageMsg.message))
                {
                    if (isNew)
                    {
                        pageMsg.status = ResponseStatus.OK.ToString();
                        pageMsg.isNewEntity = isNew;
                        //pageMsg.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                        //pageMsg.message = "<b>" + layerDetails.layer_name + "</b> Created Successfully.";
                        pageMsg.message = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_449, layerDetails.layer_name);
                    }
                    else
                    {
                        pageMsg.status = ResponseStatus.OK.ToString();
                        //pageMsg.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                        //pageMsg.message = "<b>" + layerDetails.layer_name + "</b> Updated Successfully.";
                        pageMsg.message = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_450, layerDetails.layer_name);
                    }
                    objLandBase.pageMsg = pageMsg;
                }
            }
            else
            {
                pageMsg.status = ResponseStatus.FAILED.ToString();
                pageMsg.message = getFirstErrorFromModelState();
                objLandBase.pageMsg = pageMsg;
            }
            if (isDirectSave == true)
            {
                //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                return Json(objLandBase.pageMsg, JsonRequestBehavior.AllowGet);
            }
            else
            {
                BindCompetitorIcons(objLandBase);
                // RETURN PARTIAL VIEW WITH MODEL DATA
                return PartialView("AddLandBaseLayer", objLandBase);
            }
        }
        private void BindCompetitorIcons(LandBaseLayer objLandBase)
        {
            objLandBase.lstlandBaseLyaerName = new BLLandBaseLayer().getlayerNamebyGeom(objLandBase.geomType, objLandBase.buffer_width>0?true :false);
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
        [HttpPost]
        public ActionResult GetNearByLandbaseEntities(double latitude, double longitude, int bufferInMtrs)
        {
            var usrDetail = (User)Session["userDetail"];
            var lstEntities = new BLLandBaseLayer().getNearByLandbaseEntities(latitude, longitude, bufferInMtrs, usrDetail.user_id);
            return PartialView("_LandbaseInformation", lstEntities);
        }


        [HttpPost]
        public ActionResult GetLandbaseEntityInfo(int systemId, string entityType, string settingType,string geomType)
        {
            JsonResponse<Dictionary<string, string>> objResp = new JsonResponse<Dictionary<string, string>>();
            try
            {
                var dicEntityInfo = new BLLandBaseLayer().getLandBaseEntityInfo(systemId, entityType, settingType);
                var tempDict = new Dictionary<string, string>();
                dicEntityInfo = BLConvertMLanguage.MultilingualConvertinfo(dicEntityInfo);
                if (geomType == GeometryType.Point.ToString())
                {
                    foreach (var item in dicEntityInfo)
                    {
                        if (item.Key.ToUpper() == "GEOMETRY")
                        {
                            var extent = item.Value.TrimStart("POINT(".ToCharArray()).TrimEnd(")".ToCharArray());
                            string[] lnglat = extent.Split(new string[] { " " }, StringSplitOptions.None);
                            tempDict.Add("Latitude",lnglat[1].ToString());
                            tempDict.Add("Longitude",lnglat[0].ToString());
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
                objResp.result = tempDict;
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_157;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getLandbaseGeometryDetail(GeomDetailIn objGeomDetailIn)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            var objGeometryDetail = new BLLandBaseLayer().GetLandbaseGeometryDetails(objGeomDetailIn);

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


        [HttpPost]
        public ActionResult DeleteLandbaseEntityFromInfo(int systemId, string entityType, string geomType)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            int deleteChk = 0;

            AuthorizeMessage objchk = new AuthorizeMessage();
            DbMessage response = new DbMessage();
            ImpactDetailIn objImpactDetailIn = new ImpactDetailIn();

            try
            {

                response = new BLLandBaseLayer().deleteEntity(systemId, entityType, geomType);

                if (response.status)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);//response.message;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message); ;
                }


            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                // objResp.message = entityType + " has not deleted.";
                throw;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckLandbaseEntityData(int systemId)
        {
            var lstexist = new BLLandBaseLayer().chkEntityDataExist(systemId);

            return Json(lstexist == true ? true : false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void ExportInfoEntity(int systemId, string entityType, string settingType)
        {
            var exportData = new BLLandBaseLayer().GetLandbaseEntityExportData<Dictionary<string, string>>(systemId, entityType, settingType);
            exportData = BLConvertMLanguage.ExportMultilingualConvert(exportData);

            DataTable dtlogs = Utility.MiscHelper.GetDataTableFromDictionaries(exportData);
            dtlogs.TableName = entityType;

            DataSet ds = new DataSet();
            ds.Tables.Add(dtlogs);

            ExportData(ds, "Export_" + entityType);
        }

        private void ExportData(DataSet dsReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();

                if (dsReport != null && dsReport.Tables.Count > 0)
                {
                    IWorkbook workbook = NPOIExcelHelper.DatasetToExcel("xlsx", dsReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }

        [HttpPost]
        public ActionResult GetLandbaseGeometryForEdit(ImpactDetailIn data)
        {
            var response = new JsonResponse<GeometryDetail>();
            try
            {
                ImpactDetailIn objImpactDetailIn = data;
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
                    response.result = objGeometryDetail;
                    response.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    response.status = ResponseStatus.ZERO_RESULTS.ToString();
                    response.message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                }
            }
            catch (Exception ex)
            {
                response.status = ResponseStatus.ERROR.ToString();
                response.message = Resources.Resources.SI_OSP_GBL_NET_FRM_162;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SaveEditGeometry(EditGeomIn geomObj)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                geomObj.userId = Convert.ToInt32(Session["user_id"]);
                var layer_title = geomObj.entityType;

                geomObj.Bld_Buffer = Settings.ApplicationSettings.Bld_Buffer_Mtr;


                //var chkValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
                

                //if (chkValidate.status == true)
                //{
                    var updateGeom = new BLLandBaseLayer().EditLandbaseEntityGeometry(geomObj);
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = layer_title + " " + "location updated successfully";//chkValidate.message;
                //}
                //else
                //{
                //    objResp.status = ResponseStatus.FAILED.ToString();
                //    objResp.message = ConvertMultilingual.MultilingualMessageConvert(chkValidate.message); //chkValidate.message;
                //}
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Error while updating geometery!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);

        }

        public JsonResult BindLandbaseDropdown(int landbase_layer_id, string category_type, int category_parent_id)
        {
           
            var objResp = new BLLandBaseLayer().GetLandbaseDropdown(landbase_layer_id, category_type, category_parent_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        [HttpPost]
        public ActionResult GetLBLayerSearchResult(string SearchText)
        {
            var usrDetail = (User)Session["userDetail"];
            BLSearch objBLSearch = new BLSearch();
            List<SearchResult> lstSearchResult = new List<SearchResult>();
            var serchvalue = SearchText.TrimEnd();
            if (!string.IsNullOrWhiteSpace(serchvalue))
            {
                var arrSrchText = serchvalue.Split(new[] { ':' }, 2);
                if (arrSrchText.Length == 2)
                {
                    if (arrSrchText[1].Length > 0)
                    {
                        int searchTest = 0;
                        if (int.TryParse(arrSrchText[1], out searchTest))
                        {
                            lstSearchResult = objBLSearch.GetSearchLBLayerResult(arrSrchText[0], arrSrchText[1], usrDetail.user_id, "");
                        }
                        else if (arrSrchText[1].Length >= ApplicationSettings.EntitySearchLength)
                        {
                            lstSearchResult = objBLSearch.GetSearchLBLayerResult(arrSrchText[0], arrSrchText[1], usrDetail.user_id, "");
                        }
                    }
                }
                else
                {
                    lstSearchResult = objBLSearch.GetSearchLBLayerType(arrSrchText[0], usrDetail.role_id);
                }
            }
            return Json(new { geonames = lstSearchResult }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getLBLayerGeometryDetail(GeomDetailIn objGeomDetailIn)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            var objGeometryDetail = new BLSearch().GetLBLayerGeometryDetails(objGeomDetailIn);


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
    }
}