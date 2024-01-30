using BusinessLogics;
using BusinessLogics.ISP;
using Models;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.tool.xml;
//using iTextSharp.text.html.simpleparser;
using Utility;
using System.Configuration;
using System.Net;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using Ionic.Zip;
using Lepton.Utility;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using BusinessLogics.Admin;
using Newtonsoft.Json;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using Org.BouncyCastle.Bcpg;
using Microsoft.AspNet.SignalR.Json;
using NPOI.SS.Formula.Functions;
using System.Runtime.Remoting;
using System.Web.Services.Description;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;
using Models.WFM;
using static Mono.Security.X509.X520;
//using Models.WFM;
//using System.Data.Entity.Core.Metadata.Edm;
//using Models.Admin;

namespace SmartInventory.Controllers
{
    // [UserPermission]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class MainController : Controller
    {
        //
        // GET: /Main/

        public ActionResult Index()
        {
            var usrDetail = (User)Session["userDetail"];
           
            //if (usrDetail != null && usrDetail.role_id == 1)
            //{
            //    return RedirectToAction("index", "UnAuthorized");
            //}

            var usrId = usrDetail.user_id;
            var role_Id = usrDetail.role_id;
            if (!string.IsNullOrEmpty(Convert.ToString(usrId)))
            {
                MainViewModel objMain = new MainViewModel();
                BLLayer objBLLayer = new BLLayer();
                GlobalSetting globalSetting = new GlobalSetting();
                var session = Session["userDetail"];
                if (Session["NerworkLayerDetails"] == null)
                {
                    globalSetting = new BLGlobalSetting().getValueFullText("IsBroadcastMessageEnabled");//ankit
                }
                var moduleAbbr = "NWTLYR";
                var connectionString = "";
               // List<ConnectionMaster> con = new BLLayer().GetConnectionString(moduleAbbr);
                ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);
              //  foreach (var conn in con)
              //  {
                if(con != null)
                {
					connectionString = con.connection_string;
				}
                    
              //  }

                objMain.lstNetworkLayers = objBLLayer.GetNetworkLayers(usrId, 0, role_Id, connectionString);
                Session["NerworkLayerDetails"] = objBLLayer.GetAllNetworkLayersPermissions(usrId);
                objMain.lstRegionProvinceLayers = objBLLayer.GetRegionProvinceLayers(usrId);
                objMain.lstLandBaseLayers = objBLLayer.GetLandBaseLayres(usrId, role_Id);
                objMain.lstUserModule = objBLLayer.GetUserModuleAbbrList(usrId, UserType.Web.ToString());


                Models.Admin.OrthoImageModel objViewFilter = new Models.Admin.OrthoImageModel();

                //validation for Buisnesslayer
                ApplicationSettings.isbuisnesslayer = objMain.lstUserModule.Contains("BLLYR") ? true : false;

                objViewFilter.objFilterAttributes.currentPage = 0;
                objViewFilter.objFilterAttributes.pageSize = 0;
                objMain.lstOrthoImageLayers = new BLOrthoImageLayer().GetOrthoImageLayerList(objViewFilter, usrId);

                //Bind Business Layer WMS
                objMain.lstBusinessLayer = new BLBusinessLayer().GetBusinessAllLayer();

                objMain.IsBroadcastMessageEnabled = globalSetting.value == "1" ? true : false;//ankit

                Session["ApplicableModuleList"] = objMain.lstUserModule;
                objMain.userDetail = usrDetail;

                var ticketPermission = new BLTicketTypeRoleMapping().GetTicketTypeRoleMapping(role_Id);
                objMain.objRoleMaster.lstTemplateTicketTypePermission = ticketPermission;
                return View(objMain);
            }
            else
                return View("Login/Index");
        }

		public ActionResult HierachyList()
		{
			var result = new BusinessLogics.BLDashboard().GetHierarchyList();
			return Json(new { Data = result });
		}
		public ActionResult DashboardResult(string state, string jc, string town, string partner, string fsa)
		{
			var output = new BusinessLogics.BLDashboard().GetDashboardResult(state, jc, town, partner, fsa);
			return Json(new { Data = output[0] });
		}
		[HttpPost]
        public ActionResult GetEntitySearchResult(string SearchText)
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
                            lstSearchResult = objBLSearch.GetSearchEntityResult(arrSrchText[0], arrSrchText[1], usrDetail.user_id, "");
                        }
                        else if (arrSrchText[1].Length >= ApplicationSettings.EntitySearchLength)
                        {
                            lstSearchResult = objBLSearch.GetSearchEntityResult(arrSrchText[0], arrSrchText[1], usrDetail.user_id, "");
                        }
                    }
                }
                else
                {
                    lstSearchResult = objBLSearch.GetSearchEntityType(arrSrchText[0], usrDetail.role_id);
                }
            }
            return Json(new { geonames = lstSearchResult }, JsonRequestBehavior.AllowGet);
        } 
        [HttpPost]
        public ActionResult GetSiteVendorAutoResult(string SearchText)
        {
            BLSiteInfo objBLSiteInfo = new BLSiteInfo();
            List<SiteInfo> lstSiteVendor = new List<SiteInfo>();
            var serchvalue = SearchText.TrimEnd();
            if (!string.IsNullOrWhiteSpace(serchvalue))
            {
                lstSiteVendor = objBLSiteInfo.GetSiteVendorList(serchvalue);
            }

            return Json(new { geonames = lstSiteVendor }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetFaultTypeAutoResult(string SearchText)
        {
            BLFault objFaultInfo = new BLFault();
            List<Fault> lstFaultType = new List<Fault>();
            var serchvalue = SearchText.TrimEnd();
            if (!string.IsNullOrWhiteSpace(serchvalue))
            {
                lstFaultType = objFaultInfo.GetFaultTypeList(serchvalue);
            }

            return Json(new { geonames = lstFaultType }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetSiteUIDAutoResult(string SearchText)
        {
            BLSiteInfo objBLSiteInfo = new BLSiteInfo();
            List<SiteInfo> lstSiteVendor = new List<SiteInfo>();
            var serchvalue = SearchText.TrimEnd();
            if (!string.IsNullOrWhiteSpace(serchvalue))
            {
                lstSiteVendor = objBLSiteInfo.GetSiteUIDList(serchvalue);
            }

            return Json(new { geonames = lstSiteVendor }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetFiberLinkROWAuthorityResult(string SearchText)
        {
            BLFiberLink objBLFiberLink = new BLFiberLink();
            List<FiberLink> lstFiberLink = new List<FiberLink>();
            var serchvalue = SearchText.TrimEnd();
            if (!string.IsNullOrWhiteSpace(serchvalue))
            {
                lstFiberLink = objBLFiberLink.getFiberLinkROWAuthority(serchvalue);
            }

            return Json(new { geonames = lstFiberLink }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetAutoFiberLinkId(string SearchText)
        {
            BLFiberLink objBLFiberLink = new BLFiberLink();
            List<FiberLink> lstFiberLink = new List<FiberLink>();
            var serchvalue = SearchText.TrimEnd();
            if (!string.IsNullOrWhiteSpace(serchvalue))
            {
                lstFiberLink = objBLFiberLink.GetAutoFiberLinkId(serchvalue);
            }

            return Json(new { geonames = lstFiberLink }, JsonRequestBehavior.AllowGet);
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

        public ActionResult getGeometryDetailbyGeom(int audit_id, string geomType)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            var objGeometryDetail = new BLSearch().GetGeometryDetailsbygeom(audit_id, geomType);


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

        public ActionResult getGeometry(string geomType, string geom)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();


            var objGeometryDetail = new BLSearch().GetGeometryByLatlang(geom);


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

        public ActionResult getGeometryForEdit(string geomType, string geom)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            // user related check will be there.. wheather logged in user can modify the entity or not.
            try
            {


                var objGeometryDetail = new BLSearch().GetGeometryByLatlang(geom);
                if (objGeometryDetail.entity_type.ToLower() == "cable")

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
                        objResp.message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                    }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_162;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult GetNearByEntities(double latitude, double longitude, int bufferInMtrs)
        {
            var usrDetail = (User)Session["userDetail"];
            var lstEntities = new BLMisc().getNearByEntities(latitude, longitude, bufferInMtrs, usrDetail.user_id);

            return PartialView("_Information", lstEntities);
        }

        [HttpPost]
        public ActionResult GetInfo()
        {
            var lstEntities = new List<EntityDetail>();
            return PartialView("_Information", lstEntities);
        }


        public JsonResult ComputeHomePass(int system_id, string entity_name)
        {
            var result = new BLMisc().ComputeHomePass(system_id, entity_name);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        //Merge Cable Module
        [HttpPost]
        public JsonResult GetNearByCables(double latitude, double longitude, int bufferInMtrs, int user_id = 0)
        {
            var usrDetail = (User)Session["userDetail"];
            var lstEntities = new BLMisc().getNearByEntities(latitude, longitude, bufferInMtrs, usrDetail.user_id).Where(x => x.entity_type == "Cable");
            return Json(lstEntities, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetEntityInfo(int systemId, string entityType, string geomType)
        {
            //JsonResponse<Dictionary<string, string>> objResp = new JsonResponse<Dictionary<string, string>>();
            JsonResponse<List<entityInfo>> objResp = new JsonResponse<List<entityInfo>>();
            try
            {

                var dicEntityInfo = new BLMisc().getEntityInfo(systemId, entityType, geomType, Convert.ToInt32(Session["user_id"]));
                string[] arrIgnoreColumns = { };
                var currentLang = CultureInfo.CurrentUICulture;
                string culture = currentLang.Name;


                //if (dicEntityInfo.ContainsKey("status"))
                //{
                //    dicEntityInfo["status"] = dicEntityInfo["status"] == "A" ? "Approved" : dicEntityInfo["status"];

                //}
                //if (dicEntityInfo.ContainsKey("network_status"))
                //{
                //    dicEntityInfo["network_status"] = dicEntityInfo["network_status"] == "P" ? "Planned" : dicEntityInfo["network_status"] == "A" ? "As Built" : "Dormant";
                //}
                //if (dicEntityInfo.ContainsKey("due_date"))
                //{
                //    dicEntityInfo["due_date"] = MiscHelper.FormatDate(dicEntityInfo["due_date"]);
                //}

                //if (dicEntityInfo.ContainsKey("rfs_date"))
                //{
                //    dicEntityInfo["rfs_date"] = MiscHelper.FormatDate(dicEntityInfo["rfs_date"]);
                //}
                //if (dicEntityInfo.ContainsKey("cable_measured_length"))
                //{

                //    dicEntityInfo["cable_measured_length"] = Math.Round(Convert.ToDouble(dicEntityInfo["cable_measured_length"]), 2).ToString();
                //}
                //if (dicEntityInfo.ContainsKey("cable_calculated_length"))
                //{
                //    dicEntityInfo["cable_calculated_length"] = Math.Round(Convert.ToDouble(dicEntityInfo["cable_calculated_length"]), 2).ToString();
                //}

                dicEntityInfo = BLConvertMLanguage.MultilingualConvertModel(dicEntityInfo, arrIgnoreColumns, culture);


                objResp.result = dicEntityInfo;
                //Session["EntityDetail"]= objResp.result;
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
        public ActionResult GetLayerDetail(string layerName)
        {
            JsonResponse<Dictionary<string, string>> objResp = new JsonResponse<Dictionary<string, string>>();
            try
            {
                var dicLayerDetail = new BLLayer().getLayerDetail(0, layerName);
                objResp.result = dicLayerDetail;
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_158;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetLayerMapping(string layerName)
        {
            JsonResponse<List<LayerMapping>> objResp = new JsonResponse<List<LayerMapping>>();
            try
            {
                var layerMapping = new BLLayer().getLayerMapping(layerName);
                objResp.result = layerMapping;
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_159;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getDependentChildElements(ImpactDetailIn objImpactDetailIn)
        {
            //JsonResponse<ImpactDetail> objResp = new JsonResponse<ImpactDetail>();
            //try
            //{
            //    //Restrict sales user to create entities...
            //    AuthorizeMessage obj = new AuthorizeMessage();
            //    obj = new AuthorizationHelper().IsAuthrozedForEntityCreation();

            //    if (!obj.status)
            //    {
            //        objResp.status = ResponseStatus.FAILED.ToString();
            //        objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_160;

            //        return Json(objResp, JsonRequestBehavior.AllowGet);
            //    }


            //    ImpactDetail objImpactDetail = new ImpactDetail();
            //    var lstChildElements = new BLMisc().getDependentChildElements(objImpactDetailIn);
            //    objImpactDetail.ChildElements = lstChildElements;
            //    objResp.result = objImpactDetail;
            //    //add validationn for structure depended on shaft and floor
            //    if (!string.IsNullOrWhiteSpace(objImpactDetailIn.impactType) && objImpactDetailIn.impactType.ToUpper() == "MODIFICATION")
            //    {
            //        // for  impactType=Modifcation
            //        // SET moveConnected FLAG VALUE
            //        // moveConnected WILL BE TRUE FOR THOSE ELEMENT WHICH HAVE DEPENDENCY 
            //    }
            //    objResp.status = ResponseStatus.OK.ToString();
            //}
            //catch (Exception ex)
            //{
            //    objResp.status = ResponseStatus.ERROR.ToString();
            //    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_160;
            //}
            //return Json(objResp, JsonRequestBehavior.AllowGet);

            objImpactDetailIn.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Main/GetDependentChildElements ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ImpactDetail>(url, objImpactDetailIn, "", "");
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteEntityFromInfo(int systemId, string entityType, string geomType)
        {
            var usrDetail = (User)Session["userDetail"];
            JsonResponse<string> objResp = new JsonResponse<string>();
            GISAPIResponse objGISresp= new GISAPIResponse();
            int deleteChk = 0;
            bool DelSurveyAreaChk = false;
            EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), entityType);
            //Restrict sales user to create entities...
            AuthorizeMessage objchk = new AuthorizeMessage();
            DbMessage response = new DbMessage();
            ImpactDetailIn objImpactDetailIn = new ImpactDetailIn();

            try
            {
                objchk = new AuthorizationHelper().IsAuthrozedForEntityCreation();

                if (!objchk.status)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_007;

                    return Json(objResp, JsonRequestBehavior.AllowGet);
                }
                //Boundary Deletion in JFP to be synced in GIS BY ANTRA
                string URL = new BLSearch().GetGISApi(systemId, entityType, 2);
                BoundaryPushFilter objGIS = new BoundaryPushFilter();
                objGIS.entity_type = entityType;
                objGIS.system_id= systemId;
                var objEntityDetails = new BLSearch().GetGeometryDetailsToPush(objGIS);
                string gisDesignId = new BLTicketType().GetDesignId(systemId, entityType);
                var objDetails = objEntityDetails.FirstOrDefault();
                if (objDetails != null)
                {
                    if (objDetails.objectId > 0)
                    {
                        var VersionName = "R4G_FTTX." + gisDesignId;
                        objGISresp = WebAPIRequest.PostPartnerAPI<GISAPIResponse>(usrDetail.user_id, gisDesignId, systemId, entityType, "", URL, VersionName, objDetails.objectId, "DELETE-DATA","");
                        if (objGISresp.deleteResults.Count > 0)
                        {
                            response = new BLMisc().deleteEntity(systemId, entityType, GeometryType.Polygon.ToString(), usrDetail.user_id);
                            deleteChk = response.status == true ? 1 : 0;
                        }
                    }
                    //END
                    else
                    {

                        switch (enType)
                        {
                            case EntityType.Area:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Area.ToString(), GeometryType.Polygon.ToString(), usrDetail.user_id);
                                deleteChk = response.status == true ? 1 : 0;
                                break;

                            case EntityType.SubArea:

                                response = new BLMisc().deleteEntity(systemId, EntityType.SubArea.ToString(), GeometryType.Polygon.ToString(), usrDetail.user_id);
                                deleteChk = response.status == true ? 1 : 0;
                                break;

                            case EntityType.CSA:
                                response = new BLMisc().deleteEntity(systemId, EntityType.CSA.ToString(), GeometryType.Polygon.ToString(), usrDetail.user_id);
                                deleteChk = response.status == true ? 1 : 0;
                                break;

                            case EntityType.Building:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Building.ToString(), GeometryType.Polygon.ToString(), usrDetail.user_id);
                                deleteChk = response.status == true ? 1 : 0;
                                break;

                            case EntityType.Structure:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Structure.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                if (!response.status)
                                {
                                    objResp.status = ResponseStatus.FAILED.ToString();
                                    objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);//response.message;
                                    return Json(objResp, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    string[] LayerName = { entityType };
                                    objResp.status = ResponseStatus.OK.ToString();
                                    objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_JQ_FRM_003, ApplicationSettings.listLayerDetails, LayerName);
                                    return Json(objResp, JsonRequestBehavior.AllowGet);
                                }

                            case EntityType.SurveyArea:
                                var obj = new BLMisc().GetEntityDetailById<SurveyArea>(systemId, EntityType.SurveyArea);
                                if (obj != null)
                                {
                                    if (obj.surveyarea_status == "New")
                                    {
                                        deleteChk = new BLSurveyArea().DeleteSurveyAreaById(systemId);
                                    }
                                    else
                                    {
                                        deleteChk = 1;
                                        DelSurveyAreaChk = true;
                                    }
                                }
                                break;

                            case EntityType.ADB:
                                response = new BLMisc().deleteEntity(systemId, EntityType.ADB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.CDB:
                                response = new BLMisc().deleteEntity(systemId, EntityType.CDB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.BDB:
                                response = new BLMisc().deleteEntity(systemId, EntityType.BDB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;

                            case EntityType.Pole:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Pole.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.POD:
                                response = new BLMisc().deleteEntity(systemId, EntityType.POD.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Splitter:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Splitter.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Tree:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Tree.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Manhole:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Manhole.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Coupler:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Coupler.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.SpliceClosure:
                                response = new BLMisc().deleteEntity(systemId, EntityType.SpliceClosure.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.FMS:
                                response = new BLMisc().deleteEntity(systemId, EntityType.FMS.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.MPOD:
                                response = new BLMisc().deleteEntity(systemId, EntityType.MPOD.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.ONT:
                                response = new BLMisc().deleteEntity(systemId, EntityType.ONT.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Customer:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Customer.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Gipipe:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Gipipe.ToString(), GeometryType.Line.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Cable:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Cable.ToString(), GeometryType.Line.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Trench:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Trench.ToString(), GeometryType.Line.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Duct:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Duct.ToString(), GeometryType.Line.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Conduit:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Conduit.ToString(), GeometryType.Line.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.WallMount:
                                response = new BLMisc().deleteEntity(systemId, EntityType.WallMount.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.FDB:
                                response = new BLMisc().deleteEntity(systemId, EntityType.FDB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.HTB:
                                response = new BLMisc().deleteEntity(systemId, EntityType.HTB.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.ProjectArea:
                                deleteChk = new BLProjectArea().DeleteProjectAreaId(systemId);
                                break;
                            case EntityType.UNIT:
                                response = new BLMisc().deleteEntity(systemId, EntityType.UNIT.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.DSA:
                                response = new BLMisc().deleteEntity(systemId, EntityType.DSA.ToString(), GeometryType.Polygon.ToString(), usrDetail.user_id);
                                deleteChk = response.status == true ? 1 : 0;
                                break;

                            case EntityType.ROW:
                                response = new BLMisc().deleteEntity(systemId, EntityType.ROW.ToString(), geomType, usrDetail.user_id);
                                break;
                            case EntityType.PIT:
                                response = new BLMisc().deleteEntity(systemId, EntityType.PIT.ToString(), geomType, usrDetail.user_id);
                                break;
                            case EntityType.Tower:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Tower.ToString(), geomType, usrDetail.user_id);
                                break;
                            case EntityType.Antenna:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Antenna.ToString(), geomType, usrDetail.user_id);
                                break;
                            case EntityType.Sector:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Sector.ToString(), geomType, usrDetail.user_id);
                                break;
                            case EntityType.Fault:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Fault.ToString(), geomType, usrDetail.user_id);
                                if (response.status)
                                {
                                    int result = BLFaultStatusHistory.Instance.DeleteStatusHistorybyFaultId(systemId);
                                }
                                break;
                            case EntityType.Competitor:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Competitor.ToString(), geomType, usrDetail.user_id);
                                break;
                            case EntityType.MicrowaveLink:
                                response = new BLMisc().deleteEntity(systemId, EntityType.MicrowaveLink.ToString(), geomType, usrDetail.user_id);
                                break;
                            case EntityType.Microduct:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Microduct.ToString(), GeometryType.Line.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.Network_Ticket:
                                var resultNW = new BLNetworkTicket().DeleteNetworkTicketById(systemId, Convert.ToInt32(Session["user_id"]));
                                if (resultNW.status)
                                {
                                    response.status = true;
                                    response.message = resultNW.message;
                                }
                                else
                                {
                                    response.status = false;
                                    response.message = resultNW.message;
                                }
                                break;
                            //cabinet shazia
                            case EntityType.Cabinet:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Cabinet.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            //cabinet shazia end 
                            case EntityType.OpticalRepeater:
                                response = new BLMisc().deleteEntity(systemId, EntityType.OpticalRepeater.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;

                            //vault shazia
                            case EntityType.Vault:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Vault.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            //Vault shazia end 
                            case EntityType.Loop:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Loop.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            //HANDHOLE BY ANTRA
                            case EntityType.Handhole:
                                response = new BLMisc().deleteEntity(systemId, EntityType.Handhole.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            //PATCHPANEL BY SHAZIA 
                            case EntityType.PatchPanel:
                                response = new BLMisc().deleteEntity(systemId, EntityType.PatchPanel.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                                break;
                            case EntityType.RestrictedArea:
                                response = new BLMisc().deleteEntity(systemId, EntityType.RestrictedArea.ToString(), GeometryType.Polygon.ToString(), usrDetail.user_id);
                                break;
                        }
                    }
                }
                if (deleteChk == 1)
                {
                    string[] LayerName = { entityType };
                    if (!DelSurveyAreaChk)
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                        objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_JQ_FRM_003, ApplicationSettings.listLayerDetails, LayerName);
                    }
                    else
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                        // var Res_elm = Resources.Resources.SI_OSP_GBL_NET_FRM_152;
                        //Res_elm = Res_elm.Replace("{0}", entityType);
                        objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_152, ApplicationSettings.listLayerDetails, LayerName);
                    }
                }
                else
                {
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
            }
            catch (Exception)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                // objResp.message = entityType + " has not deleted.";
                throw;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getGeometryForEdit(ImpactDetailIn objImpactDetailIn)
        {
            //JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            //// user related check will be there.. wheather logged in user can modify the entity or not.
            //try
            //{


            //    //Restrict sales user to create entities...
            //    //AuthorizeMessage obj = new AuthorizeMessage();
            //    //obj = new AuthorizationHelper().IsAuthrozedForEntityCreation();

            //    //if (!obj.status)
            //    //{
            //    //    objResp.status = ResponseStatus.FAILED.ToString();
            //    //    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_160;

            //    //    return Json(objResp, JsonRequestBehavior.AllowGet);
            //    //}

            //    var objGeometryDetail = new BLSearch().GetGeometryDetails(new GeomDetailIn { systemId = objImpactDetailIn.systemId.ToString(), entityType = objImpactDetailIn.entityType, geomType = objImpactDetailIn.geomType });
            //    if (objGeometryDetail.entity_type.ToLower() == "cable")
            //    {
            //        var getCableType = BLCable.Instance.GetCableType(objGeometryDetail.entity_id);
            //        objGeometryDetail.entity_sub_type = getCableType;
            //    }
            //    if (objGeometryDetail.geometry_extent != null)
            //    {
            //        var extent = objGeometryDetail.geometry_extent.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
            //        string[] bounds = extent.Split(',');
            //        string[] southWest = bounds[0].Split(' ');
            //        string[] northEast = bounds[1].Split(' ');
            //        objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
            //        objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
            //        objResp.result = objGeometryDetail;
            //        objResp.status = ResponseStatus.OK.ToString();
            //    }
            //    else
            //    {
            //        objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            //        objResp.message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    objResp.status = ResponseStatus.ERROR.ToString();
            //    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_162;
            //}
            //return Json(objResp, JsonRequestBehavior.AllowGet);

            objImpactDetailIn.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Main/getGeometryForEdit ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<GeometryDetail>(url, objImpactDetailIn, "", "");
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveEditGeometry(EditGeomIn geomObj)
        {
            //JsonResponse<string> objResp = new JsonResponse<string>();
            ////try
            ////{
            //geomObj.userId = Convert.ToInt32(Session["user_id"]);
            //var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == geomObj.entityType.ToUpper()).FirstOrDefault().layer_title;

            //geomObj.Bld_Buffer = Settings.ApplicationSettings.Bld_Buffer_Mtr;


            //if (geomObj.entityType.ToLower() == "subarea")
            //{
            //    var getSubArea = new BLSubArea().GetSubAreaById(geomObj.systemId);
            //    //var getBuilding = BLBuilding.Instance.GetBuildingById(getSubArea.building_system_id);
            //    if (getSubArea.building_system_id > 0 && !string.IsNullOrWhiteSpace(getSubArea.building_code))
            //    {
            //        //geomObj.systemId = getSubArea.building_system_id;
            //        //  geomObj.entityType = EntityType.Building.ToString();
            //        geomObj.systemId = getSubArea.system_id;
            //          geomObj.entityType = EntityType.SubArea.ToString();
            //        geomObj.geomType = "Polygon";
            //        var chekValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
            //        if (chekValidate.status == true)
            //        {
            //            var updateGeom = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
            //            if (updateGeom.status)
            //            {
            //                geomObj.systemId = getSubArea.system_id;
            //                geomObj.entityType = EntityType.SubArea.ToString();
            //                geomObj.geomType = "Polygon";
            //                var checkValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
            //                if (!checkValidate.status)
            //                {
            //                    objResp.status = ResponseStatus.FAILED.ToString();
            //                    objResp.message = BLConvertMLanguage.MultilingualMessageConvert(chekValidate.message); //chekValidate.message;
            //                    return Json(objResp, JsonRequestBehavior.AllowGet);
            //                }
            //                var result = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
            //                objResp.status = ResponseStatus.OK.ToString();
            //                var msg = BLConvertMLanguage.MultilingualMessageConvert(chekValidate.message);
            //                objResp.message = layer_title + " " + msg;
            //            }

            //        }
            //        else
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = BLConvertMLanguage.MultilingualMessageConvert(chekValidate.message); //chekValidate.message;
            //        }
            //    }
            //    else
            //    {
            //        var chkValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
            //        if (chkValidate.status == true)
            //        {
            //            var updateGeom = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
            //            objResp.status = ResponseStatus.OK.ToString();
            //            var msg = BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message);
            //            objResp.message = layer_title + " " + msg;
            //        }
            //        else
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message);
            //        }
            //    }

            //}
            //else
            //{
            //    var chkValidate = BASaveEntityGeometry.Instance.ValidateEntityByGeom(geomObj);
            //    if (geomObj.entityType.ToLower() == EntityType.Cable.ToString().ToLower())
            //    {
            //        PageMessage objPageValidate = new PageMessage();
            //        DbMessage objMessage = new BLMisc().validateEntity(new validateEntity
            //        {
            //            system_id = geomObj.systemId,
            //            entity_type = EntityType.Cable.ToString(),
            //            a_system_id = geomObj.tpDetail[0].system_id,
            //            a_entity_type = geomObj.tpDetail[0].entity_type,
            //            b_system_id = geomObj.tpDetail[1].system_id,
            //            b_entity_type = geomObj.tpDetail[1].entity_type
            //        }, false);

            //        if (!string.IsNullOrEmpty(objMessage.message))
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = BLConvertMLanguage.MultilingualMessageConvert(objMessage.message);//objMessage.message;
            //            return Json(objResp, JsonRequestBehavior.AllowGet);
            //        }
            //    }

            //    if (chkValidate.status == true)
            //    {
            //        var updateGeom = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
            //        objResp.status = ResponseStatus.OK.ToString();
            //        objResp.message = layer_title + " " + BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message);//chkValidate.message;
            //    }
            //    else
            //    {
            //        objResp.status = ResponseStatus.FAILED.ToString();
            //        objResp.message = BLConvertMLanguage.MultilingualMessageConvert(chkValidate.message); //chkValidate.message;
            //    }
            //}
            ////}
            ////catch (Exception ex)
            ////{
            ////    objResp.status = ResponseStatus.ERROR.ToString();
            ////    objResp.message = "Error while updating geometery!";
            ////}
            //return Json(objResp, JsonRequestBehavior.AllowGet);
            geomObj.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/Main/SaveEditGeometry ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<dynamic>(url, geomObj, "", "");
            return Json(response, JsonRequestBehavior.AllowGet);
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

        public ActionResult MapManager()
        {
            MainViewModel objMain = new MainViewModel();
            BLLayer objBLLayer = new BLLayer();
            objMain.lstBusinessLayer = new BLBusinessLayer().GetBusinessAllLayer();

            return PartialView("_MapManager", objMain.lstBusinessLayer);
        }


        public JsonResult EditCableTPDetail(EditLineTP objTPDetail)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                EditLineTP objLineTp = new EditLineTP();

                if (objTPDetail.entity_type == "Cable")
                {
                    objLineTp = BLCable.Instance.EditCableTPDetail(objTPDetail, Convert.ToInt32(Session["user_id"]));
                }
                else if (objTPDetail.entity_type == "Duct")
                {
                    objLineTp = BLDuct.Instance.EditDuctTPDetail(objTPDetail, Convert.ToInt32(Session["user_id"]));
                }
                else if (objTPDetail.entity_type == "Trench")
                {
                    objLineTp = BLTrench.Instance.EditTrenchTPDetail(objTPDetail, Convert.ToInt32(Session["user_id"]));
                }
                if (!string.IsNullOrEmpty(objLineTp.message))
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = objLineTp.message;
                }
                else { objResp.status = ResponseStatus.OK.ToString(); }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_163;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateParentEntityGeom(string pGeomType, string pEntityType, int pSystemId, string enType, bool isTemplate = false)
        {
            string[] LayerName = { EntityType.SubArea.ToString() };
            JsonResponse<string> objResp = new JsonResponse<string>();
            objResp.status = ResponseStatus.OK.ToString();
            var usrDetail = (User)Session["userDetail"];
            string txtGeom = string.Empty;
            if (pGeomType == GeometryType.Polygon.ToString())
            {
                txtGeom = new BLMisc().getEntityGeom(pSystemId, pEntityType);
            }
            else
            {
                txtGeom = GetPointTypeParentGeom(pSystemId, pEntityType);
            }

            if (isTemplate)
            {
                var chkIstemplate = new BLPoleItemMaster().ChkEntityTemplateExist(enType, Convert.ToInt32(Session["user_id"]), "");

                if (!chkIstemplate)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_050;
                    objResp.result = enType;
                    return Json(objResp, JsonRequestBehavior.AllowGet);
                }
            }

            //if (enType == EntityType.ADB.ToString())
            //{

            //    var chkSubAreaExist = new BLMisc().GetNetworkDetails(txtGeom, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
            //    if (chkSubAreaExist.system_id == 0)
            //    {
            //        objResp.status = ResponseStatus.FAILED.ToString();
            //        objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
            //    }


            //}
            //else if (enType == EntityType.CDB.ToString())
            //{

            //    var chkSubAreaExist = new BLMisc().GetNetworkDetails(txtGeom, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
            //    if (chkSubAreaExist.system_id == 0)
            //    {
            //        objResp.status = ResponseStatus.FAILED.ToString();
            //        objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
            //    }

            //}
            //else
            if (enType == EntityType.SubArea.ToString())
            {
                bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.SubArea.ToString());
                if (chkSubAreaInterSect)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SBA_NET_GBL_002, ApplicationSettings.listLayerDetails, LayerName);
                }
            }
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
        [HttpPost]
        public JsonResult ValidateEntityGeom(string geomType, string enType, string txtGeom, int systemId = 0, bool isTemplate = false, string subEntityType = "")
        {
            //JsonResponse<string> objResp = new JsonResponse<string>();
            //objResp.status = ResponseStatus.OK.ToString();
            //var usrDetail = (User)Session["userDetail"];

            //var objAreaValid = new BLMisc().ValidateEntityCreationArea(txtGeom, Convert.ToInt32(Session["user_id"]), geomType);
            //if(!objAreaValid.status)
            //{
            //    objResp.status = ResponseStatus.FAILED.ToString();
            //    objResp.message = objAreaValid.message;
            //    objResp.result = enType;
            //    return Json(objResp, JsonRequestBehavior.AllowGet);
            //}
            //////Restrict sales user to create entities...
            //AuthorizeMessage obj = new AuthorizeMessage();
            //obj = new AuthorizationHelper().IsAuthrozedForEntityCreation();

            //if (!obj.status)
            //{
            //    objResp.status = ResponseStatus.FAILED.ToString();
            //    objResp.message = obj.message;
            //    objResp.result = enType;
            //    return Json(objResp, JsonRequestBehavior.AllowGet);
            //}
            //if (isTemplate)
            //{
            //    var chkIstemplate = new BLPoleItemMaster().ChkEntityTemplateExist(enType, Convert.ToInt32(Session["user_id"]), subEntityType);

            //    if (!chkIstemplate)
            //    {
            //        objResp.status = ResponseStatus.FAILED.ToString();
            //        objResp.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_050;
            //        objResp.result = enType;
            //        return Json(objResp, JsonRequestBehavior.AllowGet);
            //    }
            //}


            //var objRegPro = BLBuilding.Instance.GetRegionProvince(txtGeom, geomType);

            //if (objRegPro != null && objRegPro.Count == 1 && objRegPro[0].province_abbreviation != null || enType == EntityType.Cable.ToString() || enType == EntityType.Duct.ToString() || enType == EntityType.Trench.ToString())
            //{
            //    if (enType == EntityType.Building.ToString())
            //    {

            //        //var objSurveyArea = BLBuilding.Instance.GetSurveyAreaExist(txtGeom, geomType);
            //        //if (objSurveyArea == null)
            //        // {
            //        // objResp.status = ResponseStatus.FAILED.ToString();
            //        //objResp.message = "No Survey Area exist at this location!";
            //        //objResp.result = "surveyarea";
            //        //}
            //        // else
            //        //{
            //        var userid = Convert.ToInt32(Session["user_id"]);
            //        var objBldValidate = BLBuilding.Instance.BldValidateByGeom(geomType, txtGeom, userid, Settings.ApplicationSettings.Bld_Buffer_Mtr);
            //        if (objBldValidate.status == false)
            // {
            // objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = String.Format(Resources.Resources.SI_OSP_BUL_NET_FRM_009, Settings.ApplicationSettings.Bld_Buffer_Mtr);
            //objResp.result = "surveyarea";
            //}
            //        //}

            //    }
            //    else if (enType == EntityType.Structure.ToString())
            //{
            //        DbMessage response = BLBuilding.Instance.ValidateStructureGeom(txtGeom, systemId);
            //        if (!response.status)
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);//response.message;
            //            objResp.result = "building";
            //}

            //    }
            //    else if (enType == EntityType.Area.ToString())
            //    {
            //        string[] LayerName = { EntityType.Area.ToString() };
            //        bool chkAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.Area.ToString());
            //        if (chkAreaInterSect)
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_AR_NET_FRM_006, ApplicationSettings.listLayerDetails, LayerName);
            //            objResp.result = "Area";
            //        }


            //    }
            //    else if (enType == EntityType.DSA.ToString())
            //    {
            //        string[] LayerName = { EntityType.DSA.ToString() };
            //        bool chkAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.DSA.ToString());
            //        if (chkAreaInterSect)
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_DSA_NET_FRM_003, ApplicationSettings.listLayerDetails, LayerName);
            //            objResp.result = "DSA";
            //        }


            //    }

            //    else if (enType == EntityType.CSA.ToString())
            //    {
            //        var chkDsaExist = new BLCsa().GetDSAExist(txtGeom);
            //        if (chkDsaExist != null && chkDsaExist.Count > 0)
            //        {
            //            string[] LayerName = { EntityType.CSA.ToString() };
            //            bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.CSA.ToString());
            //            if (chkSubAreaInterSect)
            //            {
            //                objResp.status = ResponseStatus.FAILED.ToString();
            //                objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CSA_NET_FRM_005, ApplicationSettings.listLayerDetails, LayerName);
            //            }
            //        }
            //        else
            //        {
            //            string[] LayerName = { EntityType.CSA.ToString(), EntityType.DSA.ToString() };
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CSA_NET_FRM_006, ApplicationSettings.listLayerDetails, LayerName);
            //        }


            //    }

            //    else if (enType == EntityType.ProjectArea.ToString())
            //    {

            //        bool chkProjectAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.ProjectArea.ToString());
            //        if (chkProjectAreaInterSect)
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = Resources.Resources.SI_OSP_PA_NET_FRM_005;
            //            objResp.result = "Area";
            //        }


            //    }
            //    else if (enType == EntityType.SubArea.ToString())
            //    {
            //        var chkAreaExist = new BLSubArea().GetAreaExist(txtGeom);
            //        if (chkAreaExist != null && chkAreaExist.Count > 0)
            //        {
            //            bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.SubArea.ToString());
            //            if (chkSubAreaInterSect)
            //            {
            //                string[] LayerName = { EntityType.SubArea.ToString() };
            //                objResp.status = ResponseStatus.FAILED.ToString();
            //                objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SBA_NET_GBL_002, ApplicationSettings.listLayerDetails, LayerName);
            //            }
            //        }
            //        else
            //        {
            //            string[] LayerName = { EntityType.SubArea.ToString(), EntityType.Area.ToString() };
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SBA_NET_GBL_001, ApplicationSettings.listLayerDetails, LayerName);
            //        }


            //    }




            //    //else if (enType == EntityType.SurveyArea.ToString())
            //    //{

            //    //    bool chkSurveyAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.SurveyArea.ToString());
            //    //    if (chkSurveyAreaInterSect)
            //    //    {
            //    //        objResp.status = ResponseStatus.FAILED.ToString();
            //    //        objResp.message = "Selected boundary is overlapping other SurveyArea boundary!";
            //    //    }

            //    //}
            //    else if (enType == EntityType.ADB.ToString())
            //{
            //        string[] LayerName = { EntityType.SubArea.ToString() };
            //        var chkSubAreaExist = new BLMisc().GetNetworkDetails(txtGeom, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
            //        if (chkSubAreaExist.system_id == 0)
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
            //        }


            //    }
            //    else if (enType == EntityType.BDB.ToString())
            //    {
            //        string[] LayerName = { EntityType.Area.ToString() };
            //        var chkBuildingExist = new BLMisc().GetNetworkDetails(txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
            //        if (chkBuildingExist.system_id == 0)
            //        {
            //        objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_NET_FRM_211, ApplicationSettings.listLayerDetails, LayerName);
            //    }
            //    }
            //    else if (enType == EntityType.FDB.ToString())
            //    {
            //        string[] LayerName = { EntityType.Area.ToString() };
            //        var chkBuildingExist = new BLMisc().GetNetworkDetails(txtGeom, GeometryType.Point.ToString(), EntityType.Area.ToString());
            //        if (chkBuildingExist.system_id == 0)
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_NET_FRM_211, ApplicationSettings.listLayerDetails, LayerName);
            //        }
            //    }
            //    else if (enType == EntityType.CDB.ToString())
            //    {
            //        string[] LayerName = { EntityType.SubArea.ToString() };
            //        var chkSubAreaExist = new BLMisc().GetNetworkDetails(txtGeom, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
            //        if (chkSubAreaExist.system_id == 0)
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails, LayerName);
            //        }

            //}

            //    else if (enType == EntityType.ONT.ToString() || enType == EntityType.HTB.ToString())
            //    {

            //        var chkSubAreaExist = new BLMisc().GetNetworkDetails(txtGeom, GeometryType.Point.ToString(), EntityType.SubArea.ToString());
            //        if (chkSubAreaExist.system_id == 0)
            //        {
            //            string[] LayerName = { EntityType.SubArea.ToString() };
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_153, ApplicationSettings.listLayerDetails,LayerName);
            //        }
            //    }
            //    else if (enType == EntityType.PIT.ToString())
            //    {
            //        var chkROWExist = new BLROW().GetROWExist(txtGeom);
            //        if (chkROWExist != null && chkROWExist.Count > 0)
            //        {
            //            bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.PIT.ToString());
            //            if (chkSubAreaInterSect)
            //            {
            //                objResp.status = ResponseStatus.FAILED.ToString();
            //                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_228;
            //            }
            //        }
            //        else
            //        {
            //            objResp.status = ResponseStatus.FAILED.ToString();
            //            objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_229; ;
            //        }


            //    }
            //    //else if (enType == EntityType.ROW.ToString())
            //    //{
            //    //    bool chkSubAreaInterSect = BASaveEntityGeometry.Instance.BoundaryIntersectCheck("POLYGON((" + txtGeom + "))", EntityType.ROW.ToString());
            //    //    if (chkSubAreaInterSect)
            //    //    {
            //    //        objResp.status = ResponseStatus.FAILED.ToString();
            //    //        objResp.message = "Selected boundary is overlapping other ROW boundary!";
            //    //    }
            //    //}

            //}
            //else
            //{
            //    objResp.status = ResponseStatus.FAILED.ToString();
            //    objResp.result = "region_province";

            //    if (objRegPro == null)
            //{
            //        objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_230;
            //    }
            //    else if (objRegPro.Count > 1)
            //    {
            //        objResp.message = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_231, enType);
            //    }
            //    else if (objRegPro.Count == 1 && objRegPro[0].province_abbreviation == null)
            //    {
            //        objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_232;
            //}
            //    else
            //    {
            //        objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_230;
            //    }
            //}
            //return Json(objResp, JsonRequestBehavior.AllowGet);

            ValidateEntityGeom obj = new ValidateEntityGeom();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.entityType = enType;
            obj.geomType = geomType;
            obj.isTemplate = isTemplate;
            obj.subEntityType = subEntityType;
            obj.system_id = systemId;
            obj.txtGeom = txtGeom;
            if (Session["NWTicketDetails"] != null)
            {
                var NWTicketDetails = (NetworkTicket)Session["NWTicketDetails"];
                obj.ticket_id = NWTicketDetails.ticket_id;

			}
				string url = "api/main/ValidateEntityGeom ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<dynamic>(url, obj, "", "");
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateLBEntityGeom(string geomType, string enType, string txtGeom, int systemId = 0, bool isTemplate = false, string subEntityType = "")
        {
            ValidateEntityGeom obj = new ValidateEntityGeom();
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            obj.entityType = enType;
            obj.geomType = geomType;
            obj.isTemplate = isTemplate;
            obj.subEntityType = subEntityType;
            obj.system_id = systemId;
            obj.txtGeom = txtGeom;
            string url = "api/main/ValidateLBEntityGeom ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<dynamic>(url, obj, "", "");
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ChkEntityTemplateExist(string entityType, string entitysubType)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            objResp.status = StatusCodes.OK.ToString();
            var chkIstemplate = new BLPoleItemMaster().ChkEntityTemplateExist(entityType, Convert.ToInt32(Session["user_id"]), entitysubType);

            if (!chkIstemplate)
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_050;
                objResp.result = entityType;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCableType(int systemId)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();

            var objUpdatedCableDetail = BLCable.Instance.GetCableType(systemId);
            if (objUpdatedCableDetail != "")
            {
                var NewCableType = objUpdatedCableDetail == "Underground" ? "Overhead" : "Underground";
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = String.Format(Resources.Resources.SI_OSP_CAB_NET_FRM_067, objUpdatedCableDetail, NewCableType);
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = Resources.Resources.SI_OSP_CAB_NET_FRM_060;
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLayerTitle(string layerName)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            var objlayerDetails = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == layerName.ToUpper()).FirstOrDefault();
            if (objlayerDetails != null)
            {
                objResp.result = objlayerDetails.layer_title;
            }
            else
            {
                objResp.result = layerName;
            }
            objResp.status = ResponseStatus.OK.ToString();
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLayerName(string layerTitle)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            var objlayerDetails = ApplicationSettings.listLayerDetails.Where(x => x.layer_title.ToUpper() == layerTitle.ToUpper()).FirstOrDefault();
            if (objlayerDetails != null)
            {
                objResp.result = objlayerDetails.layer_name;
            }
            else
            {
                objResp.result = layerTitle;
            }
            objResp.status = ResponseStatus.OK.ToString();
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult convertCableTypeFromInfo(int systemId, string entityType)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            var user_id = Convert.ToInt32(Session["user_id"]);
            var obj = new BLMisc().getAssociateEntity(systemId, entityType);
            var objUpdatedCableDetail = BLCable.Instance.updateCableType(systemId, user_id);
            var oldCableType = objUpdatedCableDetail.cable_type == "Underground" ? "Overhead" : "Underground";

            if (obj.Count > 0)
            {
                objResp.status = ResponseStatus.VALIDATION_FAILED.ToString();
                // objResp.message = "You can not convert <b>" + oldCableType + "</b> to <b>" + objUpdatedCableDetail.cable_type + "</b> cable because it is associated with " + (obj.entity_type.ToUpper() == "CABLE" ? obj.associated_entity_type : obj.entity_type) + ", Please remove association!";
                return Json(objResp, JsonRequestBehavior.AllowGet);
            }
            if (objUpdatedCableDetail.system_id > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = String.Format(Resources.Resources.SI_OSP_CAB_NET_FRM_068, oldCableType, objUpdatedCableDetail.cable_type);
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = Resources.Resources.SI_OSP_CAB_NET_FRM_060;
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        # region Export Entity
        [HttpPost]
        public JsonResult CheckEntityData(int systemId, string entityType, string networkStage)
        {
            var lstexist = new BLMisc().chkEntityDataExist(systemId, entityType, networkStage);

            return Json(lstexist == true ? true : false, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public void ExportInfoEntity(int systemId, string entityType, string networkStage)
        {
            //Filter the Layer Detail
            var layerDetail = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == entityType.ToUpper()).FirstOrDefault();

            var exportData = new BLMisc().GetEntityExportData<Dictionary<string, string>>(systemId, entityType, networkStage);            
            exportData = BLConvertMLanguage.ExportMultilingualConvert(exportData);

            DataTable dtlogs = Utility.MiscHelper.GetDataTableFromDictionaries(exportData, true, ApplicationSettings.numberFormatType, new string[] { "Longitude", "Latitude","Created By","Modified By", "Created By ID", "Item_Code","item code", "PTS_code" });
            dtlogs.TableName = layerDetail.layer_title;
            //if (entityType == "Cable")
            //  dtlogs = Utility.CommonUtility.GetFormattedDataTable(dtlogs, ApplicationSettings.numberFormatType);                  
            DataSet ds = new DataSet();
            ds.Tables.Add(dtlogs);
            if (layerDetail.is_reference_allowed)
            {
                #region Add the new sheet for entity Reference and export with Data set

                exportData = new BLMisc().GetEntityReferenceExportData<Dictionary<string, string>>(systemId, entityType, networkStage);
                DataTable dtReference = MiscHelper.GetDataTableFromDictionaries(exportData);
                dtReference.TableName = layerDetail.layer_title + "_Reference";
                ds.Tables.Add(dtReference);

                #endregion


                //if (entityType == "DSA" || entityType == "CSA")
                //{
                //    var LabelInfo = new BLMisc().GetEntityLabel<Dictionary<string, string>>(systemId, entityType);
                //    DataTable dtLabelInfo = Utility.MiscHelper.GetDataTableFromDictionaries(LabelInfo);
                //    dtLabelInfo.TableName = "Label_Details";
                //    ds.Tables.Add(dtLabelInfo);
                //}

                //ExportData(ds, "Export_" + layerDetail.layer_title);
            }
            if (layerDetail.is_trayinfo_enabled)
            {
                #region Add the new sheet for entity tray info and export with Data set

                exportData = new BLMisc().GetSpliceTrayExportData<Dictionary<string, string>>(systemId, entityType, networkStage);
                DataTable dtSpliceTray = MiscHelper.GetDataTableFromDictionaries(exportData);
                dtSpliceTray.TableName = layerDetail.layer_title + "_SpliceTray";
                ds.Tables.Add(dtSpliceTray);
                #endregion
            }
            if (layerDetail.is_at_enabled)
            {
                #region Adding New Sheet for AT-Acceptance Testing
                List<ATExport> listATAcceptance = new List<ATExport>();
                listATAcceptance = BLATAcceptance.Instance.GetATDetails(systemId, entityType);
                DataTable dtATDetails = Utility.MiscHelper.ListToDataTable(listATAcceptance);
                dtATDetails.TableName = layerDetail.layer_title + "_Acceptance_Testing";
                ds.Tables.Add(dtATDetails);
                #endregion
            }

            if (entityType == "DSA" || entityType == "CSA")
            {
                var LabelInfo = new BLMisc().GetEntityLabel(systemId, entityType);
                DataTable dtLabelInfo = Utility.MiscHelper.ListToDataTable(LabelInfo);
                dtLabelInfo.TableName = layerDetail.layer_title + "_Summary";
                ds.Tables.Add(dtLabelInfo);

            }
            if (entityType == EntityType.ROW.ToString())
            {
                ds.Tables.RemoveAt(0);
                DataTable dtROWInfo = Utility.MiscHelper.GetDataTableFromDictionaries(exportData, true);
                dtROWInfo.TableName = "ROW Information";
                ds.Tables.Add(dtROWInfo);
                //var applyExportData = new BLROW().getROWApply(systemId);
                //DataTable dtApplyDetails = Utility.MiscHelper.GetDataTableFromDictionaries(applyExportData,true);
                //if (dtApplyDetails.Rows.Count > 0)
                //{
                //    dtApplyDetails.TableName = "Applied Stage";
                //    ds.Tables.Add(dtApplyDetails);
                //}


                //var rowStageDetails = new BLROW().getROWApproveReject(systemId);
                //DataTable dtStageDetails = Utility.MiscHelper.GetDataTableFromDictionaries(rowStageDetails,true);
                //if (dtStageDetails.Rows.Count > 0)
                //{
                //    dtStageDetails.TableName = "Approved_Reject Stage";
                //    ds.Tables.Add(dtStageDetails);
                //}

                var rowAssociateEntity = new BLROW().getAssociatedEntitylist(systemId);
                DataTable dtROWAssociatedEntityDetails = Utility.MiscHelper.GetDataTableFromDictionaries(rowAssociateEntity);
                if (dtROWAssociatedEntityDetails.Rows.Count > 0)
                {
                    dtROWAssociatedEntityDetails.TableName = "NE Association";
                    ds.Tables.Add(dtROWAssociatedEntityDetails);
                }


            }
            if (layerDetail.is_maintainence_charges_enabled)
            {
                #region Adding New Sheet for Maintainence Charges
                List<EMCExport> listEntityMaintainenceCharges = new List<EMCExport>();

                listEntityMaintainenceCharges = BLMaintainenceCharges.Instance.GetEMChargesDetails(systemId, entityType);
                DataTable dtEMChargesDetails = Utility.MiscHelper.ListToDataTable(listEntityMaintainenceCharges);
                dtEMChargesDetails.TableName = "Recurring charges";
                ds.Tables.Add(dtEMChargesDetails);
                #endregion
            }
            if (exportData.Count > 0 && layerDetail.layer_name.ToUpper() == "BUILDING")
            {


                // var objBuildingComments = new BLBuildingComment().getbuildingComments(systemId);
                var lstBuildingComment = new BLBuildingComment().getBulkbuildingComments(systemId.ToString());
                if (lstBuildingComment.Count > 0)
                {
                    DataTable dtReport2 = new DataTable();
                    dtReport2 = MiscHelper.ListToDataTable<ExportBuildingComments>(lstBuildingComment);
                    dtReport2.Columns.Remove("id");
                    dtReport2.Columns.Remove("building_system_id");
                    dtReport2.Columns.Add("MODIFIED_ON", typeof(System.String));
                    dtReport2.TableName = "Building_Status_History";
                    dtReport2.Columns["BUILDING_CODE"].ColumnName = "Building Code";
                    dtReport2.Columns["BUILDING_STATUS"].ColumnName = "Building Status";
                    dtReport2.Columns["Comment"].ColumnName = "Comment";
                    dtReport2.Columns["MODIFIED_BY"].ColumnName = "Status Updated By";

                    foreach (DataRow dr in dtReport2.Rows)
                    {
                        dr["MODIFIED_ON"] = MiscHelper.FormatDateTime((dr["created_on"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy"); 
                    }
                    dtReport2.Columns["MODIFIED_ON"].ColumnName = "Status Updated On";
                    dtReport2.Columns.Remove("created_on");
                    ds.Tables.Add(dtReport2);
                }
            }
            if (exportData.Count > 0 && layerDetail.layer_name.ToUpper() == "FAULT")
            {


                var lstFaultStatusHistory = BLFaultStatusHistory.Instance.exportFaultStatusHistoryList(systemId);
                DataTable dtFaultStatusHistory = new DataTable();
                dtFaultStatusHistory = MiscHelper.ListToDataTable<ExportFaultHistory>(lstFaultStatusHistory);
                dtFaultStatusHistory.Columns.Remove("id");
                dtFaultStatusHistory.Columns.Remove("fault_system_id");
                // dtFaultStatusHistory.Columns.Add("MODIFIED ON", typeof(System.String));
                dtFaultStatusHistory.Columns.Add("Select Date", typeof(System.String));
                dtFaultStatusHistory.Columns.Add("MODIFIED ON", typeof(System.String));
                foreach (DataRow dr in dtFaultStatusHistory.Rows)
                {
                    dr["MODIFIED ON"] = MiscHelper.FormatDateTime((dr["created_on"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                    dr["Select Date"] = MiscHelper.FormatDate((dr["FAULT_STATUS_UPDATED_ON"].ToString()));
                }
                dtFaultStatusHistory.Columns["FAULT_STATUS"].ColumnName = "Status";
                dtFaultStatusHistory.Columns["rca"].ColumnName = "RCA";
                //dtFaultStatusHistory.Columns["MODIFIED_BY"].ColumnName = "Modified By";

                dtFaultStatusHistory.Columns["UPDATED_BY"].ColumnName = "Updated By";
                dtFaultStatusHistory.Columns["network_id"].ColumnName = "Network Id";
                dtFaultStatusHistory.Columns["REQUESTED_BY"].ColumnName = "Requested By";
                dtFaultStatusHistory.Columns["REQUEST_COMMENT"].ColumnName = "Request Comment";
                dtFaultStatusHistory.Columns["MODIFIED ON"].ColumnName = "Modified On";
                dtFaultStatusHistory.Columns.Remove("FAULT_STATUS_UPDATED_ON");
                dtFaultStatusHistory.Columns.Remove("created_on");
                //dtFaultStatusHistory.Columns.Remove("LSTFAULTSTATUS");
                dtFaultStatusHistory.Columns.Remove("MODIFIED_BY");
                // dtFaultStatusHistory.Columns.Remove("MODIFIED_ON");
                dtFaultStatusHistory.TableName = "Fault_Status_History";
                ds.Tables.Add(dtFaultStatusHistory);

            }
            if (entityType == "Tower")
            {
                //var AssociatedPopInfo = new BLMisc().GetEntityLabel(systemId, entityType);
                var lstTowerAssociatedPop = new BLTowerAssociatedPop().GetAssociatedPop(systemId);
                if (lstTowerAssociatedPop.Count > 0)
                {
                    DataTable dtAssociatedPopInfo = Utility.MiscHelper.ListToDataTable(lstTowerAssociatedPop);
                    dtAssociatedPopInfo.Columns.Remove("system_id");
                    dtAssociatedPopInfo.Columns.Remove("created_by");
                    dtAssociatedPopInfo.Columns.Remove("pop_id");
                    dtAssociatedPopInfo.Columns.Remove("tower_id");
                    dtAssociatedPopInfo.TableName = layerDetail.layer_title + "_Associated_Pop";
                    
                    ds.Tables.Add(dtAssociatedPopInfo);

                   
                }

            }
            ExportData(ds, "Export_" + layerDetail.layer_title);
            //else
            //{
            //    if (entityType == "DSA" || entityType == "CSA")
            //    {
            //        DataSet ds = new DataSet();
            //        var LabelInfo = new BLMisc().GetEntityLabel<Dictionary<string, string>>(systemId, entityType);
            //        DataTable dtLabelInfo = Utility.MiscHelper.GetDataTableFromDictionaries(LabelInfo);
            //        dtLabelInfo.TableName = "Label_Details";
            //        ds.Tables.Add(dtLabelInfo);
            //        ds.Tables.Add(dtlogs);
            //        ExportData(ds, "Export_" + layerDetail.layer_title);
            //    }
            //    else
            //    {
            //        ExportData(dtlogs, "Export_" + layerDetail.layer_title);
            //    }

            //}
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
                    if (!Response.Headers.AllKeys.Contains("Content-Disposition"))
                    {
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    }
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
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
        #endregion

        public JsonResult getClonedEntityDtl(int systemId, string entityType, string geomType)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();

            WallMountMaster objWallMount = new WallMountMaster();
            objWallMount = new BLMisc().GetEntityDetailById<WallMountMaster>(systemId, EntityType.WallMount);
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #region Bulk Entity Search

        /// <summary>
        /// Get Bulk Entity By Geometry of Selected Region on Map
        /// </summary>
        public ActionResult GetEntityByGeom(string geom, string geomType, double buff_Radius = 0.0)
        {
            JsonResponse<List<EntityLstCount>> objResp = new JsonResponse<List<EntityLstCount>>();

            try
            {
                var objValid = new BLMisc().ValidateEntityByGeom(geom, Convert.ToInt32(Session["user_id"]), geomType, buff_Radius);
                if (objValid.status == true)
                {
                    var objEntity = new BLMisc().GetEntityLstByGeom(geom, Convert.ToInt32(Session["user_id"]), geomType, buff_Radius);

                    if (objEntity.Count == 0)
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_233;
                    }
                    else
                    {
                        objResp.result = objEntity;
                        objResp.status = ResponseStatus.OK.ToString();
                        return PartialView("~/Views/Library/_BulkEntityInfo.cshtml", objEntity);
                    }
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = BLConvertMLanguage.MultilingualMessageConvert(objValid.message);//objValid.message;
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_234;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckValidEntityByGeom(string geom, string geomType, double buff_Radius = 0.0)
        {
            JsonResponse<List<EntityLstCount>> objResp = new JsonResponse<List<EntityLstCount>>();

            try
            {
                var objValid = new BLMisc().ValidateEntityByGeom(geom, Convert.ToInt32(Session["user_id"]), geomType, buff_Radius);
                if (objValid.status == true)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    // objResp.message = objValid.message;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = BLConvertMLanguage.MultilingualMessageConvert(objValid.message); //objValid.message;
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_234;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBulkOperationEntityByGeom(ViewEntityLstCountModel objEntityLstCount, string geom, string geomType, string entityType, double buff_Radius = 0.0, int page = 0, string sort = "", string sortdir = "")
        {
            var usrDetail = (User)Session["userDetail"];
            if (objEntityLstCount.objFilterAttributes.dd_networkStatus == null)
            {
                objEntityLstCount.objFilterAttributes.geom = geom;
                objEntityLstCount.objFilterAttributes.selection_type = geomType;
                objEntityLstCount.objFilterAttributes.buff_Radius = buff_Radius;
                objEntityLstCount.objFilterAttributes.dd_networkStatus = "P";

            }
            if (entityType != "" && objEntityLstCount.objFilterAttributes.entityType != null)
            {
                entityType = objEntityLstCount.objFilterAttributes.entityType;
            }
            objEntityLstCount.objFilterAttributes.pageSize = ApplicationSettings.DefaultBulkOperationPaging;
            objEntityLstCount.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objEntityLstCount.objFilterAttributes.sort = sort;
            objEntityLstCount.objFilterAttributes.orderBy = sortdir;
            objEntityLstCount.objFilterAttributes.entityType = entityType;
            //project code           
            //objEntityLstCount.lstNetworkStates = GetNetworkStage();
            objEntityLstCount.objFilterAttributes.userid = Convert.ToInt32(Session["user_id"]);
            objEntityLstCount.lstNetworkStatus = new BLMisc().GetDropDownList("", DropDownType.ddlNetworkStatus.ToString());
            Models.Admin.ListTemplateForDropDown objTemplateForDropDown = new Models.Admin.ListTemplateForDropDown();
            objTemplateForDropDown.lstBindProject = new BLProject().BindProject(objEntityLstCount.objFilterAttributes.dd_networkStatus).OrderBy(m => m.key).ToList();
            objEntityLstCount.lstBindProjectCode = objTemplateForDropDown.lstBindProject;

            objEntityLstCount.objFilterAttributes.SelectedParentUsers = objEntityLstCount.objFilterAttributes.SelectedParentUser != null && objEntityLstCount.objFilterAttributes.SelectedParentUser.Count > 0 ? string.Join(",", objEntityLstCount.objFilterAttributes.SelectedParentUser.ToArray()) : "";
            objEntityLstCount.objFilterAttributes.SelectedUserIds = objEntityLstCount.objFilterAttributes.SelectedUserId != null && objEntityLstCount.objFilterAttributes.SelectedUserId.Count > 0 ? string.Join(",", objEntityLstCount.objFilterAttributes.SelectedUserId.ToArray()) : "";

            objEntityLstCount.objFilterAttributes.roleid = usrDetail.role_id;
            // user and list of user
            List<int> parentUser = new List<int>();
            parentUser.Add(1);
            if (usrDetail.role_id == 1)
                objEntityLstCount.lstParentUsers = new BLUser().GetUsersListByMGRIds(parentUser).OrderBy(x => x.user_name).ToList();
            else
            {
                objEntityLstCount.lstParentUsers = new List<Models.User>();
                objEntityLstCount.lstParentUsers.Add(usrDetail);
            }
            if (objEntityLstCount.objFilterAttributes.SelectedParentUser != null)
            {
                if (usrDetail.role_id != 1)
                {
                    objEntityLstCount.lstUsers = new BLUser().GetUsersListByMGRIds(objEntityLstCount.objFilterAttributes.SelectedParentUser).OrderBy(x => x.user_name).ToList();
                }
                else
                {
                    objEntityLstCount.lstUsers = new BLUser().GetUsersListByMGRIds(objEntityLstCount.objFilterAttributes.SelectedParentUser).OrderBy(x => x.user_name).ToList();

                    var parentUser_ids = string.Join(",", objEntityLstCount.objFilterAttributes.SelectedParentUser.Select(n => n.ToString()).ToArray());
                    objEntityLstCount.lstUsers = new BLUser().GetUserReportDetailsList(parentUser_ids).ToList();
                }
            }

            objEntityLstCount.lstEntityList = new BLMisc().GeBulkOperationLstCount(objEntityLstCount.objFilterAttributes);
            // objEntityLstCount.objFilterAttributes.totalRecord = objEntityLstCount.lstEntityList != null && objEntityLstCount.lstEntityList.Count > 0 ? objEntityLstCount.lstEntityList[0].totalRecords : 0;
            //objEntityLstCount.objFilterAttributes.totalRecord = objEntityLstCount.lstEntityList != null ? objEntityLstCount.lstEntityList.Count : 0;
            return PartialView("_BulkOperatons", objEntityLstCount);



        }
        public List<KeyValueDropDown> GetNetworkStage()
        {
            List<KeyValueDropDown> lstNetworkStates = new List<KeyValueDropDown>();
            lstNetworkStates.Add(new KeyValueDropDown { key = "P", value = "Planned" });
            lstNetworkStates.Add(new KeyValueDropDown { key = "A", value = "As-Built" });
            lstNetworkStates.Add(new KeyValueDropDown { key = "D", value = "Dormant" });

            return lstNetworkStates;
        }
        public ActionResult ConvertBulkAsBuiltDormantSingleEntity(string changeNetworkStatus, string currentStatus, string entityType, string geom, string geomType, string entitySubtype, double buff_Radius = 0.0)
        {
            var updatenetwork = new BLNetworkStatus().ConvertBulkNetworkEntity(geom, Convert.ToInt32(Session["user_id"]), geomType, buff_Radius, currentStatus, entityType, changeNetworkStatus, entitySubtype);
            if (updatenetwork)
            {
                return Json(new { strReturn = "", msg = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { strReturn = "", msg = "Error" }, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult ConvertBulkAsBuiltDormantMultipleEntity(string currentStatus, string newStatus, string geom, string geomType, double buff_Radius = 0.0)
        {
            var updatenetwork = new BLNetworkStatus().BulkAsBuiltDormant(geom, Convert.ToInt32(Session["user_id"]), geomType, buff_Radius, currentStatus, newStatus);
            if (updatenetwork)
            {
                return Json(new { strReturn = "", msg = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { strReturn = "", msg = "Error" }, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// Save Bulk Projection Specification Process
        /// </summary>
        public JsonResult SaveBulkProjSpecific(BulkProjSpecific objProjSpec)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                objProjSpec.user_id = Convert.ToInt32(Session["user_id"]);
                var objEntity = new BLMisc().SaveBulkProjSpecific(objProjSpec);
                if (objEntity.status == true)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = Resources.Resources.SI_OSP_PA_NET_FRM_006;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_235;
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_236;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Building upload
        //sapna 
        public FileResult DownloadBuildingUploadTemplate(string FileName)
        {
            var file = "~//Content//Templates//Bulk//BuildingData.xlsx";
            string contentType = "";
            try
            {
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            catch (Exception ex)
            {
                //DACommon.WriteAdminErrorLogDB("DownloadTemplate", "DownloadTemplate[Maker]", ex);
            }
            return File(file, contentType, FileName + ".xlsx");
        }
        #endregion

        #region Network Conversion
        public ActionResult NetworkStage(NetworkStage obj)
        {
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            //var updatenetwork = new BLNetworkStatus().UpdateNetworkStatus(obj.systemid, obj.entity_type, obj.curr_status, obj.user_id);
            //updatenetwork.message = BLConvertMLanguage.MultilingualMessageConvert(updatenetwork.message);
            //if (updatenetwork.status)
            //{
            //    return Json(new { strReturn = updatenetwork, msg = "Ok" }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json(new { strReturn = updatenetwork, msg = "Error" }, JsonRequestBehavior.AllowGet);
            //}
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Main/NetworkStage ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<DbMessage>(url, obj, "", "");
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region FileUpload code by Nihal Singh started on 7th feb 2019
        //Code by Nihal Singh on 7th feb 2019
        //getAttachmentDetails Action is use for get the Documents details from database and view in UI
        public JsonResult getAttachmentDetails(int system_Id, string entity_type)
        {
            //  System.Threading.Thread.Sleep(8000);
            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                var lstDocument = new BLAttachment().getAttachmentDetails(system_Id, entity_type, "Document");
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
                        created_on = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                        OrgFileName = item.org_file_name,
                        FileExtension = item.file_extension,
                        FileLocation = item.file_location,
                        UploadType = item.upload_type,
                        file_size = BytesToString(Convert.ToInt32(item.file_size)),
                        File_ShortName = Utility.CommonUtility.ConvertStringToShortFormat(item.org_file_name, 19, 10, 9)
                    });

                }

                objResp.status = StatusCodes.OK.ToString();
                objResp.result = lstDocumentResult;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("getAttachmentDetails()", "Main", ex);
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
                //ErrorLog.LogErrorToLogFile(ex, "[getAttachmentDetails][uploadHandler.ashx]");
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
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

        //Code by Nihal Singh on 11th feb 2019
        //DeleteEntityImage Action is use for delete the image from database and from the folder
        public JsonResult DeleteEntityImage(int system_Id)
        {
            //PGDataAccess objPGDataAccess = new PGDataAccess();
            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();
            try
            {
                string sImagePath = "";
                int deleteChk = 0;
                int ImageId = system_Id;
                //Get File Name and Path...
                var lstImageDetail = new BLAttachment().getEntityImageById(ImageId);
                if (lstImageDetail != null)
                {
                    sImagePath = lstImageDetail.file_location + lstImageDetail.file_name;
                    if (!string.IsNullOrWhiteSpace(sImagePath))
                    {
                        //objPGDataAccess.BeginTransaction();
                        //Delete entry from database
                        deleteChk = new BLAttachment().DeleteFromLibraryImage(ImageId);
                        if (deleteChk == 1)
                            DeleteFileFromFTP(sImagePath);
                        else
                            objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();

                        //objPGDataAccess.CommitTransaction(true);
                    }
                    else
                    {
                        objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_239;
                    }
                }
            }
            catch (Exception ex)
            {
                //objPGDataAccess.CommitTransaction(false);
                //ErrorLog.LogErrorToLogFile(ex, "[DeleteEntityImage][UploadHandler.ashx]");
            }
            objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
            objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_238;
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteEntityImages(List<int> ListSystem_Id, string DeletedType = "")
        {
            //PGDataAccess objPGDataAccess = new PGDataAccess();
            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();
            try
            {
                string sImagePath = "";
                int deleteChk = 0;
                foreach (var systeId in ListSystem_Id)
                {
                    int ImageId = systeId;
                    //Get File Name and Path...
                    var lstImageDetail = new BLAttachment().getEntityImageById(ImageId);
                    if (lstImageDetail != null)
                    {
                        sImagePath = lstImageDetail.file_location + lstImageDetail.file_name;
                        if (!string.IsNullOrWhiteSpace(sImagePath))
                        {
                            //Delete entry from database
                            deleteChk = new BLAttachment().DeleteFromLibraryImage(ImageId);
                            if (deleteChk == 1)
                                DeleteFileFromFTP(sImagePath);
                            else
                                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();

                        }
                        else
                        {
                            objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.message = ex.Message;
            }
            objResp.status = StatusCodes.OK.ToString();
            objResp.message = string.Format(Resources.Resources.SI_OSP_GBL_JQ_GBL_074, DeletedType);
            //Resources.Resources.SI_OSP_GBL_JQ_GBL_074.Replace("{0}", "");
            //Resources.Resources.SI_OSP_GBL_NET_FRM_238;
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        //Code by Nihal Singh on 7th feb 2019
        //DeleteAttachmentFile Action is use for delete the Documents from the database and from the folder
        public JsonResult DeleteAttachmentFile(int system_Id)
        {
            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();
            //PGDataAccess objPGDataAccess = new PGDataAccess();
            try
            {
                string sFilePath = "";
                int deleteChk = 0;
                int DocumentId = system_Id;
                //Get File Name and Path...
                var lstImageDetail = new BLAttachment().getEntityDocumentById(DocumentId);
                if (lstImageDetail != null)
                {
                    sFilePath = lstImageDetail.file_location + lstImageDetail.file_name;
                    if (!string.IsNullOrWhiteSpace(sFilePath))
                    {
                        //objPGDataAccess.BeginTransaction();
                        //Delete entry from database
                        deleteChk = new BLAttachment().DeleteAttachmentById(DocumentId);
                        if (deleteChk == 1)
                            DeleteFileFromFTP(sFilePath);
                        else
                            objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();

                        //objPGDataAccess.CommitTransaction(true);
                    }
                    else
                    {
                        objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_239;
                    }
                }
            }
            catch (Exception ex)
            {
                //objPGDataAccess.CommitTransaction(false);
                //ErrorLog.LogErrorToLogFile(ex, "[DeleteAttachmentFile][UploadHandler.ashx]");

            }
            objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
            objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_155;
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAttachmentFiles(List<int> ListSystem_Id)
        {
            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();
            try
            {
                foreach (var systeId in ListSystem_Id)
                {
                    string sFilePath = "";
                    int deleteChk = 0;
                    int DocumentId = systeId;
                    //Get File Name and Path...
                    var lstImageDetail = new BLAttachment().getEntityDocumentById(DocumentId);
                    if (lstImageDetail != null)
                    {
                        sFilePath = lstImageDetail.file_location + lstImageDetail.file_name;
                        if (!string.IsNullOrWhiteSpace(sFilePath))
                        {
                            //Delete entry from database
                            deleteChk = new BLAttachment().DeleteAttachmentById(DocumentId);
                            if (deleteChk == 1)
                                DeleteFileFromFTP(sFilePath);
                            else
                                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        }
                        else
                        {
                            objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_239;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //objPGDataAccess.CommitTransaction(false);
                //ErrorLog.LogErrorToLogFile(ex, "[DeleteAttachmentFile][UploadHandler.ashx]");

            }
            objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
            objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_155;
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        //Code by Nihal Singh on 11th feb 2019
        //DeleteFileFromFTP Action is use for delete the Documents from database and from the folder
        static void DeleteFileFromFTP(string filePath)
        {
            try
            {
                string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                if (isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {

                    System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(strFTPPath + @"\" + filePath);

                    //If you need to use network credentials
                    request.Credentials = new System.Net.NetworkCredential(strFTPUserName, strFTPPassWord);
                    //additionally, if you want to use the current user's network credentials, just use:
                    //System.Net.CredentialCache.DefaultNetworkCredentials
                    request.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                    System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse();
                    response.Close();
                }
            }
            catch { throw; }
        }

        //Code by Nihal Singh on 8th feb 2019
        //getEntityImages Action is use for get the Images details from the database and view in UI

        //public ActionResult getEntityImagesList(int system_Id, string entity_type)
        //{
        //    //System.Threading.Thread.Sleep(8000);
        //    List<ImageResult> lstImageResult = new List<ImageResult>();
        //    try
        //    {
        //        string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
        //        string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
        //        string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
        //        var lstImages = new BLAttachment().getEntityImages(system_Id, entity_type, "Image");
        //        foreach (var item in lstImages)
        //        {
        //            var _imgSrc = "";
        //            string imageUrl = string.Concat(FtpUrl, item.file_location, item.file_name);

        //            WebClient request = new WebClient();
        //            if (!string.IsNullOrEmpty(UserName)) //Authentication require..
        //                request.Credentials = new NetworkCredential(UserName, PassWord);

        //            byte[] objdata = null;
        //            objdata = request.DownloadData(imageUrl);
        //            if (objdata != null && objdata.Length > 0)
        //                _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));

        //            ImageResult Imr = new ImageResult();
        //            getLatLongFromImage(objdata, Imr);
        //            lstImageResult.Add(new ImageResult()
        //            {
        //                ImgName = item.org_file_name,
        //                ImgSrc = _imgSrc,
        //                uploadedBy = item.uploaded_by,
        //                ImgId = item.id,
        //                longitude = string.IsNullOrEmpty(Convert.ToString(Imr.longitude)) ? null : Imr.longitude,
        //                latitude = string.IsNullOrEmpty(Convert.ToString(Imr.latitude)) ? null : Imr.latitude,
        //                uploadedOn = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
        //                file_ShortName = Utility.CommonUtility.ConvertStringToShortFormat(item.org_file_name, 19, 10, 9)
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogHelper.WriteErrorLog("getEntityImagesList()", "Main", ex);
        //    }

        //    return PartialView("_ImageList", lstImageResult);
        //}

        public JsonResult getEntityImages(int system_Id, string entity_type)
        {
            //System.Threading.Thread.Sleep(8000);
            JsonResponse<List<ImageResult>> objResp = new JsonResponse<List<ImageResult>>();
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

                var lstImages = new BLAttachment().getEntityImages(system_Id, entity_type, "Image");
                List<ImageResult> lstImageResult = new List<ImageResult>();


                foreach (var item in lstImages)
                {
                    var _imgSrc = "";
                    string imageUrl = string.Concat(FtpUrl, item.file_location, item.file_name);

                    WebClient request = new WebClient();
                    if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                        request.Credentials = new NetworkCredential(UserName, PassWord);

                    byte[] objdata = null;
                    objdata = request.DownloadData(imageUrl);
                    if (objdata != null && objdata.Length > 0)
                        _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));

                    ImageResult Imr = new ImageResult();

                    getLatLongFromImage(objdata, Imr);

                    lstImageResult.Add(new ImageResult()
                    {
                        ImgName = item.org_file_name,
                        ImgSrc = _imgSrc,
                        uploadedBy = item.uploaded_by,
                        ImgId = item.id,
                        longitude = string.IsNullOrEmpty(Convert.ToString(Imr.longitude)) ? null : Imr.longitude,
                        latitude = string.IsNullOrEmpty(Convert.ToString(Imr.latitude)) ? null : Imr.latitude,
                        uploadedOn = MiscHelper.FormatDateTime(item.uploaded_on.ToString()),
                        file_ShortName = Utility.CommonUtility.ConvertStringToShortFormat(item.org_file_name, 19, 10, 9)
                    });

                }
                objResp.status = StatusCodes.OK.ToString();
                objResp.result = lstImageResult;

            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("getEntityImages()", "Main", ex);
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;

                //ErrorLog.LogErrorToLogFile(ex, "[getEntityImagesNew][uploadHandler.ashx]");
            }

            var jsonResult = Json(objResp, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

            //return Json(jsonResult, JsonRequestBehavior.AllowGet,);

            //return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        //Code by Nihal Singh on 12th feb 2019
        //UploadImage Action is use for upload the image
        [HttpPost]
        public ActionResult UploadImage(FormCollection collection)
        {

            JsonResponse<string> jResp = new JsonResponse<string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    var systemId = collection["system_Id"];
                    var entityType = collection["entity_type"];
                    var featureName = collection["feature_name"];
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string FileName = file.FileName;
                        string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        string strFilePath = UploadfileOnFTP(entityType, systemId, file, "Images", strNewfilename, featureName);
                        // get User Detail..
                        User objUser = (User)(Session["userDetail"]);
                        LibraryAttachment objAttachment = new LibraryAttachment();
                        objAttachment.entity_system_id = Convert.ToInt32(systemId);
                        objAttachment.entity_type = entityType;
                        objAttachment.org_file_name = FileName;
                        objAttachment.file_name = strNewfilename;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_location = strFilePath;
                        objAttachment.upload_type = "Image";
                        objAttachment.uploaded_by = objUser.user_id.ToString();
                        objAttachment.file_size = file.ContentLength;
                        objAttachment.entity_feature_name = featureName;
                        objAttachment.uploaded_on = DateTime.Now;
                        //Save Image on FTP and related detail in database..
                        var savefile = new BLAttachment().SaveLibraryAttachment(objAttachment);
                    }
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_242;
                    jResp.status = StatusCodes.OK.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("UploadImage()", "Main", ex);
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_243;
                    jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                    //Error Logging...
                }
            }
            else
            {
                jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_244;
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult CheckFileExist(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();
            var isExist = false;
            if (Request.Files.Count > 0)
            {
                try
                {
                    var systemId = collection["system_Id"];
                    var entityType = collection["entity_type"];
                    var featureName = collection["feature_name"];
                    var documentType = collection["document_type"];
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string FileName = file.FileName;
                        var entity_system_id = Convert.ToInt32(systemId);
                        var DocExtension = "." + file.FileName.Split('.')[1];

                        isExist = new BLAttachment().CheckEntityFileExist(FileName, entity_system_id, documentType, DocExtension);
                    }
                    if (isExist)
                    {
                        jResp.message = documentType + " with same name already exist. Do you still want to continue?";
                        jResp.status = StatusCodes.DUPLICATE_EXIST.ToString();
                    }
                    else
                    {
                        jResp.message = "";
                        jResp.status = StatusCodes.OK.ToString();
                    }

                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("CheckImageExist()", "Main", ex);
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_243;
                    jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                    //Error Logging...
                }
            }
            else
            {
                jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_244;
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
        }



        //Code by Nihal Singh on 12th feb 2019
        //UploadDocument Action is use for upload the Documents
        [HttpPost]
        public ActionResult UploadDocument(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    var systemId = collection["system_Id"];
                    var entityType = collection["entity_type"];
                    var featureName = collection["feature_name"];
                    var attachmentType = "Document";
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string FileName = file.FileName;
                        string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        string strFilePath = "";
                        if (entityType == EntityType.ROW.ToString() && !string.IsNullOrEmpty(featureName))
                        {
                            attachmentType = (!string.IsNullOrEmpty(collection["attachment_type"]) ? collection["attachment_type"] : attachmentType);
                            strFilePath = UploadfileOnFTP(featureName, systemId, file, attachmentType, strNewfilename, entityType);
                        }
                        else if (!string.IsNullOrEmpty(featureName))
                        {
                            strFilePath = UploadfileOnFTP(entityType, systemId, file, attachmentType, strNewfilename, featureName);
                        }
                        else
                        {
                            strFilePath = UploadfileOnFTP(entityType, systemId, file, attachmentType, strNewfilename);
                        }

                        // get User Detail..
                        User objUser = (User)(Session["userDetail"]);
                        LibraryAttachment objAttachment = new LibraryAttachment();
                        objAttachment.entity_system_id = Convert.ToInt32(systemId);
                        objAttachment.entity_type = entityType;
                        objAttachment.org_file_name = FileName;
                        objAttachment.file_name = strNewfilename;
                        objAttachment.file_extension = Path.GetExtension(FileName);
                        objAttachment.file_location = strFilePath;
                        objAttachment.upload_type = attachmentType;
                        objAttachment.uploaded_by = objUser.user_id.ToString();
                        objAttachment.entity_feature_name = featureName;
                        objAttachment.file_size = file.ContentLength;
                        objAttachment.uploaded_on = DateTime.Now;
                        //Save Image on FTP and related detail in database..
                        var savefile = new BLAttachment().SaveLibraryAttachment(objAttachment);
                    }
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_154;
                    jResp.status = StatusCodes.OK.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("UploadDocument()", "Main", ex);
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_243;
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

        #region Acceptance Testing       

        [HttpGet]
        public void ExportATDetails(int system_id, string entity_type)
        {
            ATExport ataAcceptance = new ATExport();
            List<ATExport> listATAcceptance = new List<ATExport>();

            listATAcceptance = BLATAcceptance.Instance.GetATDetails(system_id, entity_type);

            DataTable dtATDetails = Utility.MiscHelper.ListToDataTable(listATAcceptance);
            dtATDetails.Columns["entity_network_id"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_182.Replace("_", " ");
            dtATDetails.Columns["status"].ColumnName = Resources.Resources.SI_OSP_STR_NET_HIS_010.Replace("_", " ");
            dtATDetails.Columns["status_date"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_183.Replace("_", " ");
            dtATDetails.Columns["remark"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_006.Replace("_", " ");

            dtATDetails.TableName = "AT Report";
            ExportData(dtATDetails, entity_type + "_Acceptance_Testing_Details");
        }
        #endregion


        #region Maintainence Charges       
        [HttpGet]
        public void ExportEMChargesDetails(int system_id, string entity_type)
        {
            string fileName = "";
            List<EMCExport> listEntityMaintainenceCharges = new List<EMCExport>();

            listEntityMaintainenceCharges = BLMaintainenceCharges.Instance.GetEMChargesDetails(system_id, entity_type);

            DataTable dtEMChargesDetails = Utility.MiscHelper.ListToDataTable(listEntityMaintainenceCharges);
            dtEMChargesDetails.Columns["entity_network_id"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_182;
            dtEMChargesDetails.Columns["type_of_activity_charge"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_195;
            dtEMChargesDetails.Columns["charge_category"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_053;
            dtEMChargesDetails.Columns["activity_start_date"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_054;
            dtEMChargesDetails.Columns["activity_end_date"].ColumnName = Resources.Resources.SI_OSP_ROW_NET_FRM_084;
            dtEMChargesDetails.Columns["total_cost"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_196;
            dtEMChargesDetails.Columns["remark"].ColumnName = Resources.Resources.SI_OSP_STR_NET_HIS_011;
            dtEMChargesDetails.TableName = "Maintainence Charges Report";
            fileName = entity_type == "ROW" ? entity_type + "_Recurring_Charges" : entity_type + "_Maintainence_Charges";
            ExportData(dtEMChargesDetails, fileName);

        }
        #endregion
        static string UploadfileOnFTP(string sEntityType, string sEntityId, HttpPostedFileBase postedFile, string sUploadType, string newfilename, string featureType = null)
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
                    strFTPFilePath = CreateNestedDirectoryOnFTP(strFTPPath, strFTPUserName, strFTPPassWord, featureType, sEntityType, sEntityId, sUploadType);

                    //Prepare FTP Request..
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
                            //Delete from local path.. 
                            // System.IO.File.Delete(@"" + saveThumnailPath + "/" + newfilename);
                        }
                        //Image image = Image.FromFile(fileName);
                        //Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                        //thumb.Save(Path.ChangeExtension(fileName, "thumb"));
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
                    }
                }
                return strFTPFilePath.Replace(strFTPPath, ""); // return file path
            }
            catch { throw; }
        }

        static string UploadProfileImageOnFTP(HttpPostedFileBase postedFile, string newfilename)
        {
            try
            {
                string strFTPFilePath = "";
                string strFTPPath = ConfigurationManager.AppSettings["FTPAttachment"];
                string strFTPUserName = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
                string strFTPPassWord = ConfigurationManager.AppSettings["FTPPasswordAttachment"];

                if (isValidFTPConnection(strFTPPath, strFTPUserName, strFTPPassWord))
                {
                    //Prepare FTP Request..
                    FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(strFTPPath + "UserProfiles/" + newfilename);
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
                    }
                }
                return strFTPFilePath.Replace(strFTPPath, ""); // return file path
            }
            catch { throw; }
        }

        //Code by Nihal Singh on 12th feb 2019
        //isValidFTPConnection is use for valid user check
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

        //Code by Nihal Singh on 7th feb 2019
        //CreateNestedDirectoryOnFTP is create the Directory on FTP
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

        public static string ServerMapPath(string path)
        {
            return System.Web.HttpContext.Current.Server.MapPath(path);
        }

        public static HttpResponse GetHttpResponse()
        {
            return System.Web.HttpContext.Current.Response;
        }

        public FileResult Download(string filename)
        {
            string contentType = "application/pdf";
            return File(filename, contentType, "Report.pdf");
        }
        public FileResult DownloadFileById(int id)
        {

            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();

            try
            {
                LibraryAttachment data = new BLAttachment().getEntityDocumentById(id);
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);


                string fullPath = FtpUrl + data.file_location + "/" + data.file_name;
                string FileName = data.file_location + "/" + data.file_name;
                string localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ReportFolderPath"]) + "/" + data.file_name + "";


                var request = (FtpWebRequest)WebRequest.Create(fullPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(UserName, PassWord);
                request.UseBinary = true;

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
                byte[] fileBytes = System.IO.File.ReadAllBytes(localPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, data.org_file_name);
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
        //Code by Nihal Singh on 12th feb 2019
        //DownloadFile Single Document Download
        public FileResult DownloadFile(string path, string name)
        {

            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();

            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);


                string fullPath = FtpUrl + path + "/" + name;
                string FileName = path + "/" + name;
                string localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ReportFolderPath"]) + "/" + name + "";


                var request = (FtpWebRequest)WebRequest.Create(fullPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(UserName, PassWord);
                request.UseBinary = true;

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
                byte[] fileBytes = System.IO.File.ReadAllBytes(localPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
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

        public void DownloadFileOnLocalPath(LibraryAttachment objAttachment, string localPath)
        {
            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
            string path = FtpUrl;
            if (objAttachment.entity_type == EntityType.ROW.ToString() && !string.IsNullOrEmpty(objAttachment.entity_feature_name))
            {
                path = path + "/" + objAttachment.entity_type + "/" + objAttachment.entity_feature_name;
            }
            else if (!string.IsNullOrEmpty(objAttachment.entity_feature_name))
            {
                path = path + "/" + objAttachment.entity_feature_name + "/" + objAttachment.entity_type;
            }
            else { path = path + "/" + objAttachment.entity_type; }
            path = path + "/" + objAttachment.entity_system_id + "/" + objAttachment.upload_type + "//" + objAttachment.file_name;
            string fileName = objAttachment.file_name;
            var request = (FtpWebRequest)WebRequest.Create(path);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(UserName, PassWord);
            request.UseBinary = true;
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

        //Code by Nihal Singh on 13th feb 2019
        //DownloadAll is Use for Multiple Documents download 

        public ActionResult DownloadAll(int system_Id, string entity_type, string entityFeature = "", string DocumentType = "")
        {
            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();
            FileModel _File = new FileModel();
            List<FileModel> files = new List<FileModel>();
            string zipName = string.Empty;
            try
            {
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    DocumentType = DocumentType == "" ? "Document" : DocumentType;
                    var lstDocument = new BLAttachment().getAttachmentDetails(system_Id, entity_type, DocumentType, entityFeature);
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    zip.AddDirectoryByName("Files");
                    foreach (var item in lstDocument)
                    {
                        string localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "/Attachments/" + item.file_name + "";
                        DownloadFileOnLocalPath(item, localPath);
                        zip.AddFile(localPath, "Files");
                    }
                    zipName = String.Format("{0}{1}{2}{3}.zip", entity_type.ToUpper(), DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss"));
                    if (!string.IsNullOrEmpty(entityFeature))
                    { zipName = String.Format("{0}{1}{2}{3}{4}{5}.zip", entity_type.ToUpper(), "", entityFeature, DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss")); }

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
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
                //ErrorLog.LogErrorToLogFile(ex, "[getAttachmentDetails][uploadHandler.ashx]");
            }
            finally
            {
                string localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "/Attachments";
                System.IO.DirectoryInfo di = new DirectoryInfo(localPath);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            return null;
        }
        public FileResult DownloadUserManual(int id)
        {

            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();

            try
            {
                FAQ_UserManual data = new BLHelp().getUserManualById(id);
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);


                // string fullPath = FtpUrl  + data.file_url ;
                string fullPath = FtpUrl + "UserManual" + "/" + data.category + "/" + data.file_name + data.file_extension;
                string fileName = data.display_name + data.file_extension;
                string localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"]) + "/Attachments/" + fileName + "";
                var request = (FtpWebRequest)WebRequest.Create(fullPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(UserName, PassWord);
                request.UseBinary = true;

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
                byte[] fileBytes = System.IO.File.ReadAllBytes(localPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {

            }
            return null;

        }
        public JsonResult isValidFileSize(FormCollection collection)
        {
            JsonResponse<List<ImageResult>> objResp = new JsonResponse<List<ImageResult>>();
            try
            {
                if (Request.Files.Count > 0)
                {
                    //HttpPostedFile postedFile = context.Request.Files[0];
                    HttpFileCollectionBase files = Request.Files;
                    if (files != null)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            if (file.ContentLength > ApplicationSettings.MaxFileUploadSizeLimit) //10 MB validation
                            {
                                objResp.message = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_240, ApplicationSettings.MaxFileUploadSizeLimit / 1024);
                                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                            }
                            else
                            {
                                objResp.status = StatusCodes.OK.ToString();
                                //objResp.message = "data";
                            }
                        }
                    }
                    else
                    {
                        objResp.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_099;
                    }

                }
            }
            catch (Exception ex)
            {
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_241;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #endregion
        public JsonResult Encrypt(string systemId)
        {
            return Json(MiscHelper.Encrypt(systemId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EncryptMultiple(string entityid, string entity_type, string port_no)
        {
            var genericResult = new { entityid = MiscHelper.Encrypt(entityid), entity_type = MiscHelper.Encrypt(entity_type), port_no = MiscHelper.Encrypt(port_no) };
            return Json(genericResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getEntityGeom(int systemId, string entityType)
        {
            var entityGeom = new BLMisc().getEntityGeom(systemId, entityType);
            return Json(entityGeom, JsonRequestBehavior.AllowGet);
        }

        //Code by Nihal Singh on 30th april 2019
        //UploadProfilePic Action is use for upload the image
        [HttpPost]
        public ActionResult UploadProfileImage(FormCollection collection)
        {
            JsonResponse<User> jResp = new JsonResponse<User>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string FileName = file.FileName;
                    string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                    string strFilePath = UploadProfileImageOnFTP(file, strNewfilename);
                    // get User Detail..

                    // get user detail..
                    var userId = ((User)Session["userDetail"]).user_id;
                    var objUserDetail = new BLUser().GetUserDetailByID(userId);

                    if (objUserDetail != null)
                    {
                        objUserDetail.user_img = strNewfilename;
                        objUserDetail = new BLUser().SaveUser(objUserDetail, userId);
                        // get user image bytes...
                        objUserDetail.userImgBytes = getUserProfileImage(objUserDetail.user_id, objUserDetail.user_img);
                    }
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_245;
                    jResp.status = StatusCodes.OK.ToString();
                    //jResp.result = objUserDetail;
                    Session["userProfilePicName"] = objUserDetail.user_img;
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("UploadProfilePic()", "Main", ex);
                    jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_246;
                    jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                    //Error Logging...
                }
            }
            else
            {
                jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_244;
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult getUserProfilePic(int userId)
        {
            User objUser = new User();
            JsonResponse<string> jResp = new JsonResponse<string>();
            string userImgName = Session["userProfilePicName"].ToString();
            string userImgBytes = "";
            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

            string imageUrl = string.Concat(FtpUrl + "/UserProfiles/", userImgName);

            WebClient request = new WebClient();
            if (!string.IsNullOrEmpty(UserName))
            { //Authentication require..
                request.Credentials = new NetworkCredential(UserName, PassWord);
            }
            byte[] objdata = null;
            //Download Image from FTP..
            objdata = request.DownloadData(imageUrl);

            if (objdata != null && objdata.Length > 0)
            {
                userImgBytes = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));
            }
            jResp.result = userImgBytes;
            return Json(jResp, JsonRequestBehavior.AllowGet);
        }
        public string getUserProfileImage(int usrId, string userImgName)
        {
            string userImgBytes = "";
            string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
            string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
            string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

            string imageUrl = string.Concat(FtpUrl + "/UserProfiles/", userImgName);

            WebClient request = new WebClient();
            if (!string.IsNullOrEmpty(UserName))
            { //Authentication require..
                request.Credentials = new NetworkCredential(UserName, PassWord);
            }
            byte[] objdata = null;
            //Download Image from FTP..
            objdata = request.DownloadData(imageUrl);

            if (objdata != null && objdata.Length > 0)
            {
                userImgBytes = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));
            }
            return userImgBytes;
        }
        
        public JsonResult getUserProfile()
        {
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;
            //System.Threading.Thread.Sleep(8000);
            JsonResponse<List<ImageResult>> objResp = new JsonResponse<List<ImageResult>>();
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

                var lstImages = new BLUser().GetUserDetailByID(usrId);
                List<ImageResult> lstImageResult = new List<ImageResult>();

                //foreach (var item in lstImages)
                //{
                var _imgSrc = "";
                string imageUrl = string.Concat(FtpUrl + "/UserProfile/", lstImages.user_img);

                WebClient request = new WebClient();
                if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                    request.Credentials = new NetworkCredential(UserName, PassWord);

                byte[] objdata = null;
                objdata = request.DownloadData(imageUrl);
                if (objdata != null && objdata.Length > 0)
                    _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));

                lstImageResult.Add(new ImageResult()
                {
                    ImgName = lstImages.user_img,
                    ImgSrc = _imgSrc
                    //uploadedBy = item.uploaded_by,
                    //ImgId = item.id
                });

                //}
                objResp.status = StatusCodes.OK.ToString();
                objResp.result = lstImageResult;

            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("getUserProfile()", "Main", ex);
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
            }

            var jsonResult = Json(objResp, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

            //return Json(jsonResult, JsonRequestBehavior.AllowGet,);

            //return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getExisitingTPDetails(int systemId, string eType)
        {
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
                //var aEndGeom = new BLSearch().GetGeometryDetails(new GeomDetailIn { systemId = objEntityDetail.a_system_id.ToString(), entityType = objEntityDetail.a_entity_type, geomType = GeometryType.Point.ToString() }); actualLatLng = (!string.IsNullOrEmpty(aEndGeom.longitude) ? aEndGeom.longitude + " " + aEndGeom.latitude : "") 
                // var bEndGeom = new BLSearch().GetGeometryDetails(new GeomDetailIn { systemId = objEntityDetail.b_system_id.ToString(), entityType = objEntityDetail.b_entity_type, geomType = GeometryType.Point.ToString() }); actualLatLng = (!string.IsNullOrEmpty(bEndGeom.longitude) ? bEndGeom.longitude + " " + bEndGeom.latitude : "")
                lstTPs.Add(new EditLineTPIn() { mode = "start", system_id = objEntityDetail.a_system_id, network_id = objEntityDetail.a_location, entity_type = objEntityDetail.a_entity_type });
                lstTPs.Add(new EditLineTPIn() { mode = "end", system_id = objEntityDetail.b_system_id, network_id = objEntityDetail.b_location, entity_type = objEntityDetail.b_entity_type });
            }
            else
            {
                lstTPs.Add(new EditLineTPIn() { mode = "start" });
                lstTPs.Add(new EditLineTPIn() { mode = "end" });
            }
            return Json(lstTPs, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult convertPointToPolygon(int systemId, string entityType)
        {
            JsonResponse<GeometryDetail> objResp = new JsonResponse<GeometryDetail>();
            // user related check will be there.. wheather logged in user can modify the entity or not.
            try
            {
                //Restrict sales user to create entities...
                AuthorizeMessage obj = new AuthorizeMessage();
                obj = new AuthorizationHelper().IsAuthrozedForEntityCreation();

                if (!obj.status)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_160;

                    return Json(objResp, JsonRequestBehavior.AllowGet);
                }

                var objGeometryDetail = BASaveEntityGeometry.Instance.PointToPolygon(systemId, entityType);

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
                    objResp.message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_162;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult convertPolygonToPoint(int systemId, string entityType)
        {
            var response = BASaveEntityGeometry.Instance.PolygonToPoint(systemId, entityType);
            response.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getLayerDetailByName(string layer_name)
        {
            var layerDetail = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == layer_name.ToUpper()).FirstOrDefault() : null;
            return Json(new { Data = layerDetail, JsonRequestBehavior.AllowGet });
        }

        public ActionResult feasibility()
        {
            string FeasibilityLoginURL = ApplicationSettings.SmartFeasibilityURL + "/Login";
            return Redirect(FeasibilityLoginURL);
        }

        //[HttpGet]
        //public void ExportSiteInfoDetails(int system_id, string entity_type)
        //{
        //    List<EMCExport> listEntityMaintainenceCharges = new List<EMCExport>();

        //    listEntityMaintainenceCharges = BLMaintainenceCharges.Instance.GetEMChargesDetails(system_id, entity_type);

        //    DataTable dtEMChargesDetails = Utility.MiscHelper.ListToDataTable(listEntityMaintainenceCharges);

        //    dtEMChargesDetails.TableName = "Maintainence Charges Report";
        //    ExportData(dtEMChargesDetails, entity_type + "_Maintainence_Charges"); 

        //}

        public JsonResult DeleteSiteCustomer(List<int> ListSystem_Id)
        {
            var usrDetail = (User)Session["userDetail"];
            Customer objSiteCustomer = new Customer();
            try
            {
                foreach (var systemId in ListSystem_Id)
                {
                    int deleteChk = 0;
                    int DocumentId = systemId;
                    //Get File Name and Path...
                    var lstsiteCustomer = new BLCustomer().getCustomerbyId(DocumentId);
                    objSiteCustomer.site_id = lstsiteCustomer.site_id;
                    objSiteCustomer.lmc_type = lstsiteCustomer.lmc_type;
                    if (lstsiteCustomer != null)
                    {
                        deleteChk = new BLCustomer().deleteCustomerbyId(DocumentId);
                        // Delete unit if no child
                        if (deleteChk == 1)
                        {
                            ImpactDetailIn objImpactDetailIn = new ImpactDetailIn();
                            objImpactDetailIn.systemId = lstsiteCustomer.parent_system_id;
                            objImpactDetailIn.geomType = "POINT";
                            objImpactDetailIn.entityType = EntityType.UNIT.ToString();
                            var lstChildElements = new BLMisc().getDependentChildElements(objImpactDetailIn);
                            if (lstChildElements.Count == 0)
                            {
                                var response = new BLMisc().deleteEntity(lstsiteCustomer.parent_system_id, EntityType.UNIT.ToString(), GeometryType.Point.ToString(), usrDetail.user_id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            objSiteCustomer.objPM.status = StatusCodes.UNKNOWN_ERROR.ToString();
            objSiteCustomer.objPM.message = Resources.Resources.SI_OSP_CUS_NET_FRM_008;
            return Json(objSiteCustomer, JsonRequestBehavior.AllowGet);
            //SiteCustomer objSiteCustomer = new SiteCustomer();
            //try
            //{
            //    foreach (var systemId in ListSystem_Id)
            //    {
            //        int deleteChk = 0;
            //        int DocumentId = systemId;
            //        //Get File Name and Path...
            //        var lstsiteCustomer = BLSiteCustomer.Instance.getSiteCustomerbyId(DocumentId);
            //        objSiteCustomer.site_id = lstsiteCustomer.site_id;
            //        objSiteCustomer.lmc_type = lstsiteCustomer.lmc_type;
            //        if (lstsiteCustomer != null)
            //        {
            //            deleteChk = BLSiteCustomer.Instance.deleteCustomerbyId(DocumentId);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //objSiteCustomer.objPM.status = StatusCodes.UNKNOWN_ERROR.ToString();
            //objSiteCustomer.objPM.message = "Site Customer has been Deleted Sucessfully";
            //return Json(objSiteCustomer, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DeleteSite(int siteId, int structureId)
        {
            DbMessage response = new DbMessage();
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                response = new BLMisc().deleteSite(siteId, structureId);
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
            }
            catch (Exception)
            {

                throw;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);

        }
        public ActionResult DeleteLMC(int lmcId, int cableId)
        {
            DbMessage response = new DbMessage();
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                response = new BLMisc().deleteLMC(lmcId, cableId);
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
            }
            catch (Exception)
            {

                throw;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPointTypeEntityDetails(int pSystemId, string pEntityType)
        {
            string geom = "";
            //get parent detail..
            var EntityDetail = new BLMisc().GetEntityDetailById<Dictionary<string, string>>(pSystemId, (EntityType)Enum.Parse(typeof(EntityType), pEntityType));
            if (EntityDetail != null)
            {
                //set geometry value as parent..
                geom = EntityDetail["longitude"] + " " + EntityDetail["latitude"];
                EntityDetail.Add("geom", geom);
            }
            return Json(EntityDetail, JsonRequestBehavior.AllowGet);
        }

        #region UtilizationNotifications
        public ActionResult GetUtilizationNotifications(EntityNotificationsVM objutil)
        {
            var userdetails = (User)Session["userDetail"];
            BindUtilizationDropdown(ref objutil);
            objutil.objEntityNotiFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objutil.objEntityNotiFilter.currentPage = 1;
            objutil.objEntityNotiFilter.userId = Convert.ToInt32(userdetails.user_id);
            objutil.objEntityNotiFilter.roleId = Convert.ToInt32(userdetails.role_id);
            objutil.lstViewEntityNotifications = new BLMisc().GetEntityNotificationList(objutil.objEntityNotiFilter);
            objutil.objEntityNotiFilter.totalRecord = objutil.lstViewEntityNotifications.Count > 0 ? objutil.lstViewEntityNotifications[0].totalRecords : 0;
            return PartialView("_UtilizationNotifications", objutil);

        }
        public void BindUtilizationDropdown(ref EntityNotificationsVM objUtil)
        {
            //rt
            var userdetails = (User)Session["userDetail"];
            //Bind Layers..   
            objUtil.lstUtilLayers = new BLLayer().GetReportLayers(userdetails.role_id, "UTILIZATION");
        }

        public ActionResult UpdateNotificationCloseStatus(string commentText, int notificationId)
        {
            //save comment
            var objUser = ((User)Session["userDetail"]);
            EntityNotificationComments objNotificationComment = new EntityNotificationComments();
            objNotificationComment.comment_text = commentText;
            objNotificationComment.notification_id = notificationId;
            objNotificationComment.created_by = objUser.user_id;
            objNotificationComment.created_on = DateTimeHelper.Now;
            var objResult = new BLMisc().SaveEntityNotificationComment(objNotificationComment);
            // update notification status is_closed as true.
            var objNotification = new BLMisc().UpdateNotificationCloseStatus(notificationId, objUser.user_id);
            //It is require to show the last added comment immideatily after saveing into db.
            objResult.created_by_text = objUser.user_name;
            return Json(new { closeStatus = objNotification.is_closed, data = objResult }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetUtilizationNotificationData(int page = 0, string searchBy = "", string searchText = "")
        {
            EntityNotificationsFilter objFilter = new EntityNotificationsFilter();
            objFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objFilter.currentPage = page == 0 ? 1 : page;
            objFilter.searchBy = searchBy;
            objFilter.searchText = searchText;
            var lstViewEntityNotifications = new BLMisc().GetEntityNotificationList(objFilter);
            return PartialView("_UtilizationNotificationData", lstViewEntityNotifications);
        }
        public ActionResult SaveEntityNotificationComment(string commentText, int notificationId)
        {
            var objUser = ((User)Session["userDetail"]);
            EntityNotificationComments objNotificationComment = new EntityNotificationComments();
            objNotificationComment.comment_text = commentText;
            objNotificationComment.notification_id = notificationId;
            objNotificationComment.created_by = objUser.user_id;
            objNotificationComment.created_on = DateTimeHelper.Now;
            var objResult = new BLMisc().SaveEntityNotificationComment(objNotificationComment);
            SmartInventoryHub smartInventoryhub = SmartInventoryHub.Instance;
            var UnreadNotificationCount = new BLMisc().GetUnreadNotificationCount(objUser.user_id, objUser.role_id);
            NotificationOutPut objNotification = new NotificationOutPut();
            objNotification.info = Convert.ToString(UnreadNotificationCount);
            objNotification.sendToAllUser = false;
            objNotification.notificationType = notificationType.Utilization.ToString();
            smartInventoryhub.BroadCastInfo(objNotification);
            //It is require to show the last added comment immideatily after saveing into db.
            objResult.created_by_text = objUser.user_name;
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult getEntityNotificationComment(int notificationId)
        {
            var lstNotificationComment = new BLMisc().getEntityNotificationComment(notificationId);
            return PartialView("_NotificationComment", lstNotificationComment);
        }
        #endregion

        #region Header
        public ActionResult HeaderInfo()
        {
            OSPHeader objOspHeader = new OSPHeader();
            objOspHeader.objuser = (User)Session["userDetail"];
            objOspHeader.ApplicationName = ApplicationSettings.ApplicationName;
            objOspHeader.ADFSEndPoint = ApplicationSettings.ADFSEndPoint;
            objOspHeader.UnreadNotificationCount = new BLMisc().GetUnreadNotificationCount(objOspHeader.objuser.user_id, objOspHeader.objuser.role_id);
            return PartialView("_Header", objOspHeader);
        }
        #endregion
        public ActionResult GetRegionProvince()
        {
            var usrDetail = (User)Session["userDetail"];
            //if (usrDetail != null && usrDetail.role_id == 1)
            //{
            //    return RedirectToAction("index", "UnAuthorized");
            //}
            var usrId = usrDetail.user_id;
            BLLayer objBLLayer = new BLLayer();
            var model = objBLLayer.GetRegionProvinceLayers(usrId);

            return PartialView("_RegionAndProvince", model);
        }

        [HttpPost]
        public PartialViewResult GetEntityInfoToolbar(int systemId, string layerName, string network_status, bool isBackButtonRequired = true)
        {
            List<LayerActionMapping> objlayers = new List<LayerActionMapping>();
            var usrDetail = (User)Session["userDetail"];

            var userId = usrDetail.user_id;
            var entityInformationDetail = new BLMisc().getLayerActions(systemId, layerName, true, network_status, usrDetail.role_id, userId, false, "", "");
            // entityInformationDetail = ConvertMultilingual.MultilingualConvertinfo(entityInformationDetail);
            for (int i = 0; i < entityInformationDetail.Count; i++)
            {
                entityInformationDetail[i].action_title = BLConvertMLanguage.MultilingualMessageConvert(entityInformationDetail[i].action_title);
                if (entityInformationDetail[i].action_name.ToUpper() == "GROUPLIBRARY" && (usrDetail.user_name.ToLower() == "sa" || usrDetail.user_name.ToLower() == "admin"))
                {
                    entityInformationDetail[i].is_visible = true;
                    entityInformationDetail[i].is_enabled = true;
                }
                else if (entityInformationDetail[i].action_name.ToUpper() == "GROUPLIBRARY" && (usrDetail.user_name.ToLower() != "sa" || usrDetail.user_name.ToLower() != "admin"))
                {
                    entityInformationDetail[i].is_enabled = false;
                    entityInformationDetail[i].is_visible = false;
                }
            }
            ViewBag.IsBackButtonRequired = isBackButtonRequired;           
            return PartialView("_InformationToolbar", entityInformationDetail);
        }


        public ActionResult NotificationStatus(int systemId, string entityType)
        {
            EntityNotificationStatus objStatus = new EntityNotificationStatus();
            objStatus.systemId = systemId;
            objStatus.entityType = entityType;
            objStatus.status = new BLMisc().getNotificationStatus(systemId, entityType);
            objStatus.lstComment = new BLMisc().GetDropDownList("", DropDownType.BlockNotification.ToString());
            var unBlockList = new BLMisc().GetDropDownList("", DropDownType.UnBlockNotification.ToString());
            foreach (var item in unBlockList) { objStatus.lstComment.Add(item); }
            return PartialView("_NotificationStatus", objStatus);
        }
        public ActionResult SaveNotificationStatus(EntityNotificationStatus objNotification)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            var NotificationStatus = new BLMisc().SaveNotificationStatus(objNotification, Convert.ToInt32(Session["user_id"]));
            if (NotificationStatus.id > 0)
            {
                objPM.status = ResponseStatus.OK.ToString(); ;
                objPM.message = string.Format(objNotification.status ? Resources.Resources.SI_OSP_GBL_NET_FRM_341 : Resources.Resources.SI_OSP_GBL_NET_FRM_342);
                NotificationStatus.objPM = objPM;
            }
            NotificationStatus.lstComment = new BLMisc().GetDropDownList("", DropDownType.BlockNotification.ToString());
            var unBlockList = new BLMisc().GetDropDownList("", DropDownType.UnBlockNotification.ToString());
            foreach (var item in unBlockList) { NotificationStatus.lstComment.Add(item); }
            return PartialView("_NotificationStatus", NotificationStatus);
        }
        public ActionResult GetNotificationStatusHistory(int systemId = 0, string eType = "", int page = 0, string sort = "", string sortdir = "")
        {
            ViewAuditMasterModel objAudit = new ViewAuditMasterModel();
            objAudit.systemId = systemId;
            objAudit.eType = eType;
            objAudit.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.systemid = systemId;
            objAudit.objFilterAttributes.entityType = eType;

            List<Dictionary<string, string>> lstReportData = new BLMisc().getNotificationStatusHistory(systemId, eType.ToUpper(), objAudit.objFilterAttributes.currentPage, objAudit.objFilterAttributes.pageSize, objAudit.objFilterAttributes.sort, objAudit.objFilterAttributes.orderBy);
            string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
            foreach (Dictionary<string, string> dic in lstReportData)
            {
                var obj = (IDictionary<string, object>)new System.Dynamic.ExpandoObject();

                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        obj.Add(col.Key, col.Value);
                    }
                }
                objAudit.lstData.Add(obj);
            }
            objAudit.lstData = BLConvertMLanguage.MultilingualConvert(objAudit.lstData, arrIgnoreColumns);

            objAudit.objFilterAttributes.totalRecord = lstReportData.Count > 0 ? Convert.ToInt32(lstReportData[0].FirstOrDefault().Value) : 0;
            Session["viewAuditHistory"] = objAudit.objFilterAttributes;
            return PartialView("_NotificationStatusHistory", objAudit);
        }
        public JsonResult BlockNotification(EntityNotificationStatus objNotification)
        {
            var NotificationStatus = new BLMisc().SaveNotificationStatus(objNotification, Convert.ToInt32(Session["user_id"]));
            return Json(NotificationStatus, JsonRequestBehavior.AllowGet);
        }

        #region

        public JsonResult BulkDeleteProcess(BulkDelete objBulkDelete)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                List<bulkDeleteOperation> objBulkDeleteOperation = new List<bulkDeleteOperation>();
                objBulkDelete.user_id = Convert.ToInt32(Session["user_id"]);
                objBulkDeleteOperation = new BLMisc().BulkDeleteProcess(objBulkDelete);
                Session["objBulkDeleteOperation"] = objBulkDeleteOperation;
                var result = new
                {
                    successCount = objBulkDeleteOperation.Count(x => x.is_deleted),
                    failureCount = objBulkDeleteOperation.Count(x => !x.is_deleted)
                };
                if (result.successCount > 0 && result.failureCount == 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_061;// "Entity Deleted Successfully!";
                }
                else if (result.failureCount > 0 && result.successCount == 0)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_062;// "There are some dependent elements, Please remove them first. Please check the downloaded logs.";
                }
                else if (result.successCount > 0 && result.failureCount > 0)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_063;// "The selected entites deleted partially. Please check the downloaded logs.";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("BulkDeleteProcess()", "Main", ex);
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_064;// "Something went wrong while performing bulk delete entity operation!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public void DownloadBulkDeleteProcessLogs()
        {

            if (Session["objBulkDeleteOperation"] != null)
            {

                try
                {
                    var sessionData = Session["objBulkDeleteOperation"] as IEnumerable<bulkDeleteOperation>;
                    // bulkDeleteOperation objbulkDeleteOperation = (bulkDeleteOperation)Session["objBulkDeleteOperation"];
                    DataTable dtTable = new DataTable();

                    dtTable = Utility.MiscHelper.ListToDataTable(sessionData.ToList());

                    dtTable.TableName = "Bulk_Delete_Operation_Logs";
                    if (dtTable.Rows.Count > 0)
                    {
                        dtTable.Columns.Remove("IS_DELETED");
                        dtTable.Columns["ENTITY_NAME"].ColumnName = "Entity Name";
                        dtTable.Columns["NETWORK_CODE"].ColumnName = "Network Id";
                        dtTable.Columns["DELETED_STATUS"].ColumnName = "Delete Status";
                        dtTable.Columns["MESSAGE"].ColumnName = "Message";
                        ExportData(dtTable, "BulkDeleteOperationLogs_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }


        public PartialViewResult GetChangePassword()
        {
            var usrId = Convert.ToInt32(Session["user_id"]);

            return PartialView("_ChangePassword");
        }
        [HttpPost]
        public JsonResult SavePassword(ChangePassword objChangePassword)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            int user_id = Convert.ToInt32(Session["user_id"]);
            BLUser objBLuser = new BLUser();
            User objUserDetails = objBLuser.getUserDetails(user_id);
            if (objChangePassword.currentPassword != Utility.MiscHelper.DecodeTo64(objUserDetails.password))
            {
                objResp.status = "Invalid";
                objResp.message = Resources.Resources.SI_GBL_CHG_NET_FRM_001;
            }
            else
            {
                objBLuser.saveChangePassword(Utility.MiscHelper.EncodeTo64(objChangePassword.confirmNewPassword), user_id);
                objResp.status = "Valid";
                objResp.message = Resources.Resources.SI_GBL_CHG_NET_FRM_002; ;
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Abhimanyu

        public void getLatLongFromImage(byte[] imgBytes, ImageResult objResult)
        {

            Stream stream = new MemoryStream(imgBytes);
            Bitmap bmp = new Bitmap(stream);
            // set Variable Values
            double? latitdue = null;
            double? longitude = null;
            string ns = null, ew = null;

            foreach (PropertyItem propItem in bmp.PropertyItems)
            {
                switch (propItem.Type)
                {
                    case 2:
                        if (propItem.Id == 1) // North or South
                            ns = System.Text.Encoding.ASCII.GetString(new byte[1] { propItem.Value[0] });
                        if (propItem.Id == 3) // East or West
                            ew = System.Text.Encoding.ASCII.GetString(new byte[1] { propItem.Value[0] });
                        break;
                    case 5:
                        if (propItem.Id == 2) // Latitude Array
                            latitdue = GetLatitudeAndLongitude(propItem);
                        if (propItem.Id == 4) //Longitude Array
                            longitude = GetLatitudeAndLongitude(propItem);
                        break;
                }
            }

            if (ew == "W") { longitude = 0 - longitude; }
            if (ns == "S") { latitdue = 0 - latitdue; }

            objResult.latitude = latitdue;
            objResult.longitude = longitude;
            stream.Flush();
            stream.Close();
        }

        private static double? GetLatitudeAndLongitude(PropertyItem propItem)
        {
            try
            {
                uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
                uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
                uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
                uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
                uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
                uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
                return (Convert.ToDouble(degreesNumerator) / Convert.ToDouble(degreesDenominator)) + (Convert.ToDouble(Convert.ToDouble(minutesNumerator) / Convert.ToDouble(minutesDenominator)) / 60) +
                       (Convert.ToDouble((Convert.ToDouble(secondsNumerator) / Convert.ToDouble(secondsDenominator)) / 3600));
            }
            catch (Exception)
            {

                return null;
            }
        }

        public JsonResult ShowOnMapImage(double Latitude, double Longitude)
        {
            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();
            try
            {

            }
            catch (Exception ex)
            {

            }
            objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
            objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_238;
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult ValidateItemSpecificaton(int no_of_ports, string entityType, int vendor_id)
        {
            List<Models.Admin.VendorSpecificationMaster> objResult = new List<Models.Admin.VendorSpecificationMaster>();
            //var layerDetail = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == entityType.ToUpper()).FirstOrDefault() : null;
            objResult = new BLVendorSpecification().GetEntityTemplateDetails(no_of_ports, entityType, vendor_id);
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// BY ANTRA
        /// Save Bulk Pod Association Process
        /// </summary>
        public JsonResult SaveBulkPodAssociation(BulkPodAssociation objPodAssctn)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            var layerDetails = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.POD.ToString()).FirstOrDefault();
            try
            {
                objPodAssctn.user_id = Convert.ToInt32(Session["user_id"]);
                var objEntity = new BLMisc().SaveBulkPodAssociation(objPodAssctn);

                if (objEntity.status == true)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = layerDetails.layer_title + " " + Resources.Resources.SI_OSP_GBL_NET_FRM_478;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_479 + " " + layerDetails.layer_title + " " + Resources.Resources.SI_OSP_GBL_NET_FRM_481;
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_480 + " " + layerDetails.layer_title + " " + Resources.Resources.SI_OSP_GBL_NET_FRM_481;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getEntityImagesList(int system_Id, string entity_type)
        {
            List<DocumentResult> lstImageResult = new List<DocumentResult>();
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
                // var lstImages = new BLAttachment().getEntityImages(system_Id, entity_type, "Image");
                var lstImages = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, "Image");
                foreach (var item in lstImages)
                {
                    var _imgSrc = "";
                    string imageUrl = string.Concat(FtpUrl, item.FileLocation, item.FileName);

                    WebClient request = new WebClient();
                    if (!string.IsNullOrEmpty(UserName)) //Authentication require..
                        request.Credentials = new NetworkCredential(UserName, PassWord);

                    byte[] objdata = null;
                    if (isFileExistOnFTP(imageUrl))
                    {
                        objdata = request.DownloadData(imageUrl);
                    }
                    if (objdata != null && objdata.Length > 0)
                        _imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));
                    ImageResult Imr = new ImageResult();

                    getLatLongFromImage(objdata, Imr);

                    lstImageResult.Add(new DocumentResult()
                    {
                        Id = item.Id,
                        EntitySystemId = item.EntitySystemId,
                        FileName = item.FileName,
                        EntityType = item.EntityType,
                        UploadedBy = item.UploadedBy,
                        created_on = MiscHelper.FormatDateTime(item.Uploaded_on.ToString()),
                        OrgFileName = item.OrgFileName,
                        FileExtension = item.FileExtension,
                        FileLocation = _imgSrc,
                        UploadType = item.UploadType,
                        file_size = BytesToString(Convert.ToInt32(item.file_size)),
                        File_ShortName = Utility.CommonUtility.ConvertStringToShortFormat(item.OrgFileName, 19, 10, 9),
                        categorytype = item.categorytype,
                        delete_action = item.delete_action
                    });
                }


            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("getEntityImagesList()", "Main", ex);
            }

            return PartialView("_ImageList", lstImageResult);
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
        public ActionResult getAttachmentDetailsList(int system_Id, string entity_type)
        {
            //  System.Threading.Thread.Sleep(8000);
            List<DocumentResult> lstDocumentResult = new List<DocumentResult>();
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                var lstDocument = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, "Document");
                lstDocumentResult = GetDocumentList(lstDocument);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("getAttachmentDetailsList()", "Main", ex);
            }
            return PartialView("_DocumentList", lstDocumentResult);
        }

        public ActionResult UploadRefLink(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();

            try
            {
                var systemId = collection["system_Id"];
                var DisplayTxt = collection["refDisplayTxt"];
                var RefLink = collection["refLink"]; ;
                var entity_type = collection["entity_type"];


                // get User Detail..
                User objUser = (User)(Session["userDetail"]);
                LibraryAttachment objAttachment = new LibraryAttachment();
                objAttachment.entity_system_id = Convert.ToInt32(systemId);
                objAttachment.upload_type = "RefLink";
                objAttachment.entity_type = entity_type;
                //objAttachment.refDisplayTxt = DisplayTxt;
                //objAttachment.refLink = RefLink;
                objAttachment.org_file_name = DisplayTxt;
                objAttachment.file_location = RefLink;

                objAttachment.uploaded_by = objUser.user_id.ToString();
                objAttachment.uploaded_on = DateTime.Now;
                //Save Ref link details
                var savefile = new BLAttachment().SaveLibraryAttachment(objAttachment);

                jResp.message = "Saved successfully.";// Resources.Resources.SI_OSP_GBL_NET_FRM_242;
                jResp.status = StatusCodes.OK.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("UploadRefLink()", "Main", ex);
                jResp.message = "Something went wroung !";// Resources.Resources.SI_OSP_GBL_NET_FRM_243;
                jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
                //Error Logging...
            }
        }

        public JsonResult getReferenceLink(int system_Id, string entity_type)
        {
            JsonResponse<List<DocumentResult>> objResp = new JsonResponse<List<DocumentResult>>();
            try
            {
                var lstDocument = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, "RefLink");
                List<DocumentResult> lstDocumentResult = new List<DocumentResult>();
                lstDocumentResult = GetDocumentList(lstDocument);
                objResp.status = StatusCodes.OK.ToString();
                objResp.result = lstDocumentResult;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("getReferenceLink()", "Main", ex);
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_237;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getReferenceLinkList(int system_Id, string entity_type)
        {
            List<DocumentResult> lstDocumentResult = new List<DocumentResult>();
            try
            {
                var lstDocument = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, "RefLink");
                lstDocumentResult = GetDocumentList(lstDocument);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("getReferenceLinkList()", "Main", ex);
            }
            return PartialView("_RefLinkList", lstDocumentResult);
        }
        public FileResult DownloadFiles(string json, string entity_type = "")
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
                    //zip.AddDirectoryByName("Files");
                    #region Get the slected files
                    foreach (var item in listPathName)
                    {
                        string fullPath = "", FileName = "", localPath = "";
                        if (item.location.ToLower() == "entity")
                        {
                            var data = new BLAttachment().getEntityDocumentById(item.systemId);
                            fullPath = FtpUrl + data.file_location + "/" + data.file_name;
                            FileName = data.file_location + "/" + data.file_name;
                            localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ReportFolderPath"]) + "/" + data.file_name + "";
                        }
                        else if (item.location.ToLower() == "specification")
                        {
                            var data = new BLAttachment().getSpecificationAttachmentsbyid(item.systemId);
                            fullPath = FtpUrl + data.file_location + "/" + data.file_name;
                            FileName = data.file_location + "/" + data.file_name;
                            localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ReportFolderPath"]) + "/" + data.file_name + "";
                        }
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
                            zip.AddFile(localPath, "");
                        }
                        catch (Exception)
                        {
                        }
                        //zip.AddFile(localPath, "Files");

                    }
                    #endregion
                    zipName = String.Format("{0}{1}{2}{3}.zip", entity_type, DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss"));
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


        public FileResult DownloadFilesAll(int system_Id, string entity_type, string entityFeature = "", string DocumentType = "")
        {

            string zipName = string.Empty;
            try
            {
                string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
                string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
                string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);

                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    DocumentType = DocumentType == "" ? "Document" : DocumentType;
                    var lstDocument = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, DocumentType, entityFeature);
                    zip.AlternateEncodingUsage = Ionic.Zip.ZipOption.AsNecessary;
                    //zip.AddDirectoryByName("Files");
                    #region Get the slected files                  

                    foreach (var item in lstDocument)
                    {
                        string fullPath = "", FileName = "", localPath = "";

                        //LibraryAttachment data = new BLAttachment().getEntityDocumentById(item.entity_system_id);
                        // fullPath = FtpUrl + item.file_location + "/" + item.file_name;
                        // FileName = item.file_location + "/" + item.file_name;
                        // localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ReportFolderPath"]) + "/" + item.file_name + "";

                        if (item.categorytype.ToLower() == "entity")
                        {
                            var data = new BLAttachment().getEntityDocumentById(item.Id);
                            fullPath = FtpUrl + data.file_location + "/" + data.file_name;
                            FileName = data.file_location + "/" + data.file_name;
                            localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ReportFolderPath"]) + "/" + data.file_name + "";
                        }
                        else if (item.categorytype.ToLower() == "specification")
                        {
                            var data = new BLAttachment().getSpecificationAttachmentsbyid(item.Id);
                            fullPath = FtpUrl + data.file_location + "/" + data.file_name;
                            FileName = data.file_location + "/" + data.file_name;
                            localPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ReportFolderPath"]) + "/" + data.file_name + "";
                        }


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
                            zip.AddFile(localPath, "");
                        }
                        catch (Exception)
                        {
                        }
                        //zip.AddFile(localPath, "Files");

                    }

                    #endregion
                    zipName = String.Format("{0}{1}{2}{3}.zip", entity_type + "" + DocumentType, DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss"));
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

        public ActionResult DownlTextFormatFile(int system_Id, string entity_type = "", string DocType = "", string category = "")
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            var lstDocument = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, DocType, category);
            if (category.ToLower() == "specification" && lstDocument.Count == 0)
            {
                if (entity_type == "" || entity_type == null)
                {
                    lstDocument = new BLAttachment().getAttachmentDetailsDocs(system_Id, entity_type, DocType, category);
                }
            }
            else
            {
                if ((entity_type == "" || entity_type == null) && category.ToLower() != "specification")
                {
                    var othes = new BLAttachment().getEntityDocumentById(system_Id);
                    lstDocument.Add(new DocumentResult()
                    {
                        Id = othes.id,
                        EntitySystemId = othes.entity_system_id,
                        EntityType = othes.entity_type,
                        UploadedBy = othes.uploaded_by,
                        created_on = MiscHelper.FormatDateTime(othes.uploaded_on.ToString()),
                        Uploaded_on = othes.uploaded_on,
                        OrgFileName = othes.org_file_name,
                        FileLocation = othes.file_location,
                        UploadType = othes.upload_type
                    });

                }
            }

            for (int i = 0; i < lstDocument.Count; i++)
            {
                tw.WriteLine("Display Text   :  " + lstDocument[i].OrgFileName);
                tw.WriteLine("Reference Link :  " + lstDocument[i].FileLocation);
                tw.WriteLine("Uploaded By    :  " + lstDocument[i].UploadedBy);
                tw.WriteLine("Uploaded On    :  " + MiscHelper.FormatDateTime(lstDocument[i].Uploaded_on.ToString()));
                if (lstDocument.Count > 1 && i < lstDocument.Count - 1)
                    tw.WriteLine("-----------------------------------------------------------------------------");
                tw.WriteLine("  ");
            }
            tw.Flush();
            tw.Close();
            string FName = String.Format("{0}{1}{2}{3}.txt", entity_type + "" + DocType, DateTimeHelper.Now.ToString("ddMMyyyy"), "-", DateTimeHelper.Now.ToString("HHmmss"));
            return File(memoryStream.GetBuffer(), "text/plain", FName);
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

        public PartialViewResult EntityAlongDirection(string path)
        {
            EntityDirection entityDir = new EntityDirection();
            var userdetails = (User)Session["userDetail"];
            entityDir.lstLayers = new BLLayer().GetLayerDetailsForEntityDirection();
            return PartialView("~/Views/Shared/_EntityAlongDirection.cshtml", entityDir);
        }
        public JsonResult CreateEntityALongDirection(string entities, string geom)
        {
            JsonResponse<List<DbMessage>> objResp = new JsonResponse<List<DbMessage>>();
            try
            {
                var usrDetail = (User)Session["userDetail"];
                if (usrDetail != null)
                {
                    var usrId = usrDetail.user_id;
                    objResp.result = new BLLayer().CreateEntityAlongDirection(entities, geom, usrDetail.user_id);
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

        //push to gis bulk of data List<
        //BoundaryPushFilter>
        [HttpPost]
        public JsonResult BoundaryGetDataToList(List<BoundaryPushFilter> data,int psystemId,string pentityType)
        { 
            var userdetail= (User)Session["userDetail"];
            var token = (TokenDetail)Session["TokenDetail"];
            GisApiLogs _objGisapiLogs = new GisApiLogs();
            var TransactionId = DateTime.Now.Ticks.ToString();
            JsonPlannerResponse<string> objResp = new JsonPlannerResponse<string>();
            string url;
            CreateVersionIn objIn = new CreateVersionIn();
            string gisDesignId = new BLTicketType().GetDesignId(psystemId, pentityType);
             var process_id = DateTime.Now.Ticks.ToString();
            if (string.IsNullOrEmpty(gisDesignId))
            {

                objResp.results = "Please generate design Id!";
                return Json(objResp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var objGISAttr = new BLSearch().GetGisAttributes<GISAttributes>(data[0]);
                if (objGISAttr.status.ToLower() == "false")
                {
                    objResp.results = objGISAttr.message;
                    _objGisapiLogs.system_id = psystemId;
                    _objGisapiLogs.entity_type = pentityType;
                    _objGisapiLogs.message = objResp.results;
                    _objGisapiLogs.status = "Failed";
                    _objGisapiLogs.gis_design_id = gisDesignId;
                    _objGisapiLogs.request_time = DateTime.Now;
                    _objGisapiLogs.user_id = userdetail.user_id;
                    _objGisapiLogs.transaction_id = TransactionId;
                    _objGisapiLogs.process_id = process_id;
                    new Utility.BlUtility.BLErrorLog().SaveGisApiLogs(_objGisapiLogs);
                    return Json(objResp, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    //var userDetail = "";
                    if (userdetail == null || userdetail.user_id == 0)
                    {
                        userdetail = (User)Session["userDetail"];
                    }

                    objIn.UserName = "JFP_" + userdetail.user_name;
                    objIn.VersionName = gisDesignId;
                    objIn.user_id = userdetail.user_id;
                    objIn.gis_design_id = gisDesignId;
                    objIn.systemId = psystemId;
                    objIn.entityType = pentityType;
                    objIn.transaction_id = TransactionId;
                    objIn.process_id = process_id;
                    url = "api/main/GISCreateVersion";
                    var response = WebAPIRequest.PostIntegrationAPIRequest<CreateVersionOut>(url, objIn, "", "", "", token, false);
                    LogHelper.GetInstance.WriteDebugLog("Response-2:" + JsonConvert.SerializeObject(response));
                    if (response.results != null)
                    {
                        TempData["NE_System_id"] = response.results;
                        TempData["txn_id"] = TransactionId;
                        if (response.results.Status.ToUpper() == "SUCCESS")
                        {                           
                            Pushgislogrequest pushgisrequestobj = new Pushgislogrequest();
                            pushgisrequestobj.id = Convert.ToInt32(psystemId);
                            pushgisrequestobj.system_id = Convert.ToInt32(psystemId);
                            pushgisrequestobj.entity_type = pentityType;
                            pushgisrequestobj.created_by = userdetail.user_id;
                            pushgisrequestobj.created_on = DateTime.Now;
                            pushgisrequestobj = new BLTicketType().Getpushrequest(pushgisrequestobj.system_id, pushgisrequestobj.entity_type,
                            pushgisrequestobj.created_by, "INSERT", process_id);
                            //threading start int psystemId,string pentityType

                            System.Threading.Tasks.Task.Factory.StartNew(() =>
                            {
                                foreach (var item in data)
                                {
                                    TransactionId = DateTime.Now.Ticks.ToString();
                                    item.gis_design_id = gisDesignId;
                                    item.user_id = userdetail.user_id;
                                    item.VersionName = "R4G_FTTX." + gisDesignId;
                                    item.transaction_id = TransactionId;
                                    item.process_id = process_id;
                                    url = "api/main/PushToGis";
                                    var PushToGISResponse = WebAPIRequest.PostIntegrationAPIRequest<dynamic>(url, item, "", "", "", token, false);
                                }

                                //pushgisrequestobj = new BLTicketType().Getpushrequest(pushgisrequestobj.system_id,
                                //    pushgisrequestobj.entity_type, pushgisrequestobj.created_by, "Update", process_id, "Request has been processed successfully!");

                                pushgisrequestobj = (Pushgislogrequest)GISPostVersion(objIn.transaction_id, process_id, psystemId, pentityType, userdetail, token);

                            }).ContinueWith(tsk =>
                            {
                                tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("BoundaryGetDataToList", "Main", ex); return true; });
                            }, TaskContinuationOptions.OnlyOnFaulted);

                          
                            //threading end

                            //PostVersionIn ObjPostVer = new PostVersionIn();
                            //ObjPostVer.user_id = userdetail.user_id;
                            //ObjPostVer.gis_design_id = gisDesignId;
                            //ObjPostVer.systemId = psystemId;
                            //ObjPostVer.entityType = pentityType;
                            //ObjPostVer.VersionName = objIn.VersionName;
                            //ObjPostVer.RequestedBy = objIn.UserName;
                            //ObjPostVer.objectId = Convert.ToInt32(response.results);
                            //ObjPostVer.transaction_id = TransactionId;
                            //url = "api/main/GISPostVersion";
                            //var PostVerResponse = WebAPIRequest.PostIntegrationAPIRequest<PostVersionOut>(url, ObjPostVer, "", "", "", token, false);
                            //objResp.results = PostVerResponse.results.Message;
                            return Json(pushgisrequestobj, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            objResp.results = response.results.Status;
                            return Json(objResp, JsonRequestBehavior.AllowGet);
                        }
                        return Json(objResp, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(objResp, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public object GISPostVersion(string transactionId, string processid,int systemId, string EntityType, User userDetails, TokenDetail token)
        {
            string url;
          //  var TransactionId = TempData.Peek("txn_id").ToString();
            string gisDesignId = new BLTicketType().GetDesignId(systemId, EntityType);
            PostVersionIn ObjPostVer = new PostVersionIn();
            ObjPostVer.user_id = userDetails.user_id;
            ObjPostVer.gis_design_id = gisDesignId;
            ObjPostVer.systemId = 0;
            ObjPostVer.entityType = EntityType;
            ObjPostVer.VersionName = gisDesignId;
            //ObjPostVer.objectId = Convert.ToInt32(TempData.Peek("postGisCreate"));
            ObjPostVer.RequestedBy = "JFP_" + userDetails.user_name;
            ObjPostVer.transaction_id = transactionId;
            ObjPostVer.process_id = processid;
            url = "api/main/GISPostVersion";
            var PostVerResponse = WebAPIRequest.PostIntegrationAPIRequest<PostVersionOut>(url, ObjPostVer, "", "", "", token, false);
           
            Pushgislogrequest pushgisrequestobj = new Pushgislogrequest();
            pushgisrequestobj =  new BLTicketType().Getpushrequest(systemId, EntityType, userDetails.user_id, "Update", processid, "Request has been processed successfully!");
            return pushgisrequestobj;

            //return Json(PostVerResponse, JsonRequestBehavior.AllowGet);
        }


        //public void UpdateOtherGeometry(EditGeomIn geomObj)
        //{
        //    if (geomObj.entityType.ToUpper() == EntityType.Tower.ToString().ToUpper())
        //    {
        //        List<SectorMaster> lstSectorMasters = new BLSector().GetSectorByTowerId(geomObj.systemId);
        //        foreach (var sectorMaster in lstSectorMasters)
        //        {
        //            geomObj.systemId = sectorMaster.system_id;
        //            geomObj.entityType = EntityType.Sector.ToString();
        //            geomObj.geomType = GeometryType.Polygon.ToString();
        //            geomObj.longLat = Common.GetSectorsGeometry(sectorMaster.latitude, sectorMaster.longitude, sectorMaster.azimuth, sectorMaster.sector_type);
        //            var result = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);

        //        }
        //    }
        //}

        public ActionResult RefreshMessagepush(int systemId, string entitytype)
        {
            Pushgislogrequest pushgisrequestobj = new Pushgislogrequest();
            var userdetails = (User)Session["userDetail"];
            pushgisrequestobj.id = systemId;
            pushgisrequestobj.system_id = systemId;
            pushgisrequestobj.entity_type = entitytype;
            pushgisrequestobj.created_by = userdetails.user_id;
            pushgisrequestobj.created_on = DateTime.Now;
            pushgisrequestobj = new BLTicketType().Getpushrequest(pushgisrequestobj.system_id, pushgisrequestobj.entity_type,
            pushgisrequestobj.created_by, "SELECT","");
            return Json(pushgisrequestobj, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PushDataToGIS(BoundaryPushFilter obj, User userdetail, TokenDetail token)
        {
            GisApiLogs _objGisapiLogs = new GisApiLogs();
            var TransactionId=  DateTime.Now.Ticks.ToString();
            JsonPlannerResponse<string> objResp = new JsonPlannerResponse<string>();
            string url;
            CreateVersionIn objIn = new CreateVersionIn();
            string gisDesignId = new BLTicketType().GetDesignId(obj.system_id, obj.entity_type);

            if (string.IsNullOrEmpty(gisDesignId))
            {

                objResp.results = "Please generate design Id!";
                return Json(objResp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var objGISAttr = new BLSearch().GetGisAttributes<GISAttributes>(obj);
                if (objGISAttr.status.ToLower() == "false")
                {
                    objResp.results = objGISAttr.message;
                    _objGisapiLogs.system_id = obj.system_id;
                    _objGisapiLogs.entity_type = obj.entity_type;
                    _objGisapiLogs.message = objResp.results;
                    _objGisapiLogs.status = "Failed";
                    _objGisapiLogs.gis_design_id = gisDesignId;
                    _objGisapiLogs.request_time = DateTime.Now;
                    _objGisapiLogs.user_id = userdetail.user_id;
                    _objGisapiLogs.transaction_id = TransactionId;
                    new Utility.BlUtility.BLErrorLog().SaveGisApiLogs(_objGisapiLogs);
                    return Json(objResp, JsonRequestBehavior.AllowGet);

                }
                else
                {                   
                    //var userDetail = "";
                    if (userdetail == null || userdetail.user_id == 0)
                    {
                        userdetail = (User)Session["userDetail"];                        
                    }
                    objIn.UserName = "JFP_" + userdetail.user_name;
                    objIn.VersionName = gisDesignId;
                    objIn.user_id = userdetail.user_id;
                    objIn.gis_design_id = gisDesignId;
                    objIn.systemId = obj.system_id;
                    objIn.entityType = obj.entity_type;
                    objIn.transaction_id = TransactionId;
                    url = "api/main/GISCreateVersion";
                    var response = WebAPIRequest.PostIntegrationAPIRequest<CreateVersionOut>(url, objIn, "", "","",token,false);
                    LogHelper.GetInstance.WriteDebugLog("Response-2:" + JsonConvert.SerializeObject(response));
                    if (response.results != null)
                    {
                        if (response.results.Status.ToUpper() == "SUCCESS")
                        {
                            obj.gis_design_id = gisDesignId;
                            obj.user_id = userdetail.user_id;
                            obj.VersionName = "R4G_FTTX." + gisDesignId;
                            obj.transaction_id = TransactionId;
                            url = "api/main/PushToGis";
                            var PushToGISResponse = WebAPIRequest.PostIntegrationAPIRequest<dynamic>(url, obj, "", "","",token, false);
                         
                            if (PushToGISResponse.status == StatusCodes.OK.ToString())
                            {
                                PostVersionIn ObjPostVer = new PostVersionIn();
                                ObjPostVer.user_id = userdetail.user_id;
                                ObjPostVer.gis_design_id= gisDesignId;
                                ObjPostVer.systemId = obj.system_id;
                                ObjPostVer.entityType = obj.entity_type;
                                ObjPostVer.VersionName = objIn.VersionName;
                                ObjPostVer.RequestedBy = objIn.UserName;
                                ObjPostVer.objectId = Convert.ToInt32(PushToGISResponse.results);
                                ObjPostVer.transaction_id = TransactionId;
                                url = "api/main/GISPostVersion";
                                var PostVerResponse = WebAPIRequest.PostIntegrationAPIRequest<PostVersionOut>(url, ObjPostVer, "", "", "", token, false);
                                objResp.results = PostVerResponse.results.Message;
                                return Json(objResp, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                objResp.results = PushToGISResponse.error_message.ToString();
                                return Json(objResp, JsonRequestBehavior.AllowGet);
                            }
                           
                        }
                        else
                        {
                            objResp.results = response.results.Status;
                            return Json(objResp, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(objResp, JsonRequestBehavior.AllowGet);
            }

        }
       
        public ActionResult GetProcessedXMLDashboard(ProcessSummaryFilter objFilters, int page = 1, string sort = "", string sortdir = "", int systemId = 0, string ps_port = "")
        {
            objFilters.pageSize = 10;
            objFilters.currentPage = page == 0 ? 1 : page;
            objFilters.sort = sort;
            objFilters.sortdir = sortdir;
            objFilters.objProcessSummary.userId = Convert.ToInt32(Session["user_id"]);
            if (objFilters.systemId > 0)
            {
                TempData["NE_System_id"] = objFilters.systemId;
            }
            if (objFilters.systemId == 0)
            {
                objFilters.systemId = (int)TempData.Peek("NE_System_id");
            }
            List<Dictionary<string, string>> lstProcessedXMLDetails = new BLProcess().GetNEXMLSplitters(objFilters);
            Session["ExportProcessedXMLDetails"] = objFilters;
            if (lstProcessedXMLDetails.Count > 0)
            {
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                var ProcessedSplitter = (List<EntitySummary>)Session["ProcessedSplitterXMLDetails"];
                foreach (Dictionary<string, string> dic in lstProcessedXMLDetails)
                {
                    if (ProcessedSplitter != null)
                    {
                        var entry = ProcessedSplitter.FirstOrDefault(e => e.gis_design_id == dic["common_name"]);
                        if (entry != null)
                        {
                            //dic["trace_validation_status"] = entry.status.ToString();
                            Session["spl_system_id"] = dic["system_id"];
                        }
                    }
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
            return PartialView("_ProcessedNEXMLDashboard", objFilters);
        }
		//GetDesignValidationDashboard
		public ActionResult GetSecondarySplitter(SeconarySplitterListFilter objFilters, int page = 1, string sort = "", string sortdir = "")
		{
			objFilters.sort = sort;
			objFilters.sortdir = sortdir;
			
			List<Dictionary<string, string>> lstSeconarySplitterDetails = new BLProcess().GetSecondarySplitter(objFilters);
			if (lstSeconarySplitterDetails.Count > 0)
			{
				string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
				foreach (Dictionary<string, string> dic in lstSeconarySplitterDetails)
				{
					
					var obj = (IDictionary<string, object>)new ExpandoObject();
					foreach (var col in dic)
					{
						if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
						{
							obj.Add(col.Key, col.Value);
						}

					}
					objFilters.lstSeconarySplitterDetails.Add(obj);

				}
				objFilters.totalRecord = objFilters.lstSeconarySplitterDetails.Count > 0 ? Convert.ToInt32(lstSeconarySplitterDetails[0].FirstOrDefault().Value) : 0;
           }
			objFilters.column_filter = BindSeconarySplitterdropdown();
			return PartialView("_DesignValidationDashboard", objFilters);
		}
		public JsonResult IsDesignSubmittedByEntity(int systemId, string entityType)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
            var result = new BLProcess().IsDesignSubmittedByEntity(systemId, entityType);
			if (!result)
			{
				objResp.status = ResponseStatus.FAILED.ToString();
			}
			else
			{
				objResp.status = ResponseStatus.OK.ToString();
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}
		public dynamic BindSeconarySplitterdropdown()
		{
     		List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem { Text = "S2 ID", Value = "common_name" });
			items.Add(new SelectListItem { Text = "CSA Id", Value = "csa_id" });
			items.Add(new SelectListItem { Text = "DSA Id", Value = "dsa_id" });
            return items;
		}

		public ActionResult Binddropdown()
        {
            var result = (List<string>)Session["ApplicableModuleList"];
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "S2 ID", Value = "common_name" });
            items.Add(new SelectListItem { Text = "CSA Id", Value = "csa_id" });
            items.Add(new SelectListItem { Text = "DSA Id", Value = "dsa_id" });
            items.Add(new SelectListItem { Text = "Trace Validation Status", Value = "trace_validation_status" });
            items.Add(new SelectListItem { Text = "Group XML Filename", Value = "group_xml_filename" });
            items.Add(new SelectListItem { Text = "NAS Status", Value = "nas_status" });
            items.Add(new SelectListItem { Text = "NE Status", Value = "ne_status" });
            items.Add(new SelectListItem { Text = "CSA RFS Status", Value = "rfs_status" });
            if (Convert.ToInt32(Session["user_id"]) == 1)
            {
                items.Add(new SelectListItem { Text = "Entity Design Id", Value = "Entity" });
            }
            ViewData["listItem"] = items;
            return View();
        }
        public ActionResult ProcessSplitterData(string pEntityType, string pSystemId, int pUserId, int p_subareaid)
        {
            // List<EntitySummary> ProcessData = new List<EntitySummary>();
            var ProcessData = new BLProcess().ValidateEntitySummary(pSystemId, pEntityType, pUserId, p_subareaid);
            Session["ProcessedSplitterXMLDetails"] = ProcessData;
            return Json(ProcessData, JsonRequestBehavior.AllowGet);
        }

        // Below method for S2 trace validation 
		public ActionResult ValidateSecondarySplitterData(string pEntityType, int pUserId, int p_systemId, string p_sourceRefId,string p_action)
		{
			var ProcessData = new BLProcess().ValidateSecondarySplitterData(pEntityType, pUserId, p_systemId, p_sourceRefId, p_action);
			return Json(ProcessData, JsonRequestBehavior.AllowGet);
		}
		public void DownloadProcessSplitterDataLogs(string pEntityType, string pSystemId, int pUserId, int p_subareaid)
        {
            var ProcessedData = (List<EntitySummary>)Session["ProcessedSplitterXMLDetails"];
            if (ProcessedData.Count > 0)
            {
                DataSet ds = new DataSet();
                foreach (var item in ProcessedData)
                {

                    if (!string.IsNullOrEmpty(item.logs) && !item.status)
                    {
                        List<log> list = new List<log>();
                        log obj = new log();
                        item.listLog = JsonConvert.DeserializeObject<List<log>>(item.logs);
                        DataTable dt = MiscHelper.ListToDataTable(item.listLog);
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Columns.Contains("entity_id")) { dt.Columns.Remove("entity_id"); }
                            if (dt.Columns.Contains("entity_type")) { dt.Columns.Remove("entity_type"); }
                            if (dt.Columns.Contains("is_processed")) { dt.Columns.Remove("is_processed"); }
                            dt.Columns["entity_title"].SetOrdinal(0);
                            dt.Columns["gis_design_id"].SetOrdinal(1);
                            dt.Columns["network_status"].SetOrdinal(2);
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i]["network_status"].ToString() == "P")
                                {
                                    dt.Rows[i]["network_status"] = "Planned";
                                }
                            }
                            dt.Columns["entity_title"].ColumnName = "Entity Type";
                            dt.Columns["gis_design_id"].ColumnName = "GIS DESIGN ID";
                            dt.Columns["network_status"].ColumnName = "Network Status";
                        }
                        dt.TableName = item.gis_design_id + "_path" + item.path;
                        ds.Tables.Add(dt);

                    }
                }
                if (ds.Tables.Count > 0)
                {
                    ExportData(ds, "ExportValidationLogs_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                }
            }
        }

        //Below Method for download S2 trace validation failed log 
		public void DownloadSecondarySplitterValidationLogs(string pEntityType, int pUserId, int p_systemId, string p_sourceRefId, string p_action, int? p_requestLogId)
		{
			var ProcessedData = new BLProcess().DownloadSecondarySplitterValidationLogs( pUserId, p_action, p_requestLogId);
			if (ProcessedData.Count > 0)
			{
				DataTable dt = MiscHelper.ListToDataTable(ProcessedData);
				if (dt.Rows.Count > 0)
				{

					//if (dt.Columns.Contains("DESIGN_ID")) { dt.Columns.Remove("DESIGN_ID"); }
					if (dt.Columns.Contains("ENTITY_TYPE")) { dt.Columns.Remove("ENTITY_TYPE"); }
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						if (dt.Rows[i]["network_status"].ToString() == "P")
						{
							dt.Rows[i]["network_status"] = "Planned";
						}
						if (dt.Rows[i]["network_status"].ToString() == "A")
						{
							dt.Rows[i]["network_status"] = "As Built";
						}
					}
					dt.Columns["ENTITY_TITLE"].ColumnName = "Entity Name";
					dt.Columns["DESIGN_ID"].ColumnName = "Design Id";
					dt.Columns["NETWORK_ID"].ColumnName = "Network Id";
					dt.Columns["NETWORK_STATUS"].ColumnName = "Network Status";
					dt.Columns["ERR_MESSAGE"].ColumnName = "Message";
					
				}
				string fileName = "";
				if (p_action == "DesignValidation")
				{
					fileName = "DesignValidationLogs";
					dt.TableName = "DesignValidationLogs";
				}
				else
				{
					fileName = "ConstructionValidationLogs";
					dt.TableName = "ConstructionValidationLogs";
				}
                if (dt.Rows.Count > 0)
                {
                    ExportData(dt, fileName + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                }
			}
		}

		
		#region XMLDashboard
		public ActionResult XmlBuilderDashboard(XMLBuilderDashboardFilter objXMLBuilderDashboardFilter, int page = 0, string sort = "", string sortdir = "")
        {
            objXMLBuilderDashboardFilter.pagerecord = 10;
            objXMLBuilderDashboardFilter.pageno = page == 0 ? 1 : page;
            objXMLBuilderDashboardFilter.sortcolname = sort;
            objXMLBuilderDashboardFilter.sorttype = sortdir;
            objXMLBuilderDashboardFilter.lstXML = new BLXMLBuilderDashboard().GetXMLBuilderDashboard(objXMLBuilderDashboardFilter);
            objXMLBuilderDashboardFilter.totalrecords = objXMLBuilderDashboardFilter.lstXML.Count > 0 ? objXMLBuilderDashboardFilter.lstXML[0].totalrecords : 0;
            Session["viewXMLBuilderDashboardFilter"] = objXMLBuilderDashboardFilter;
            return PartialView("_XMLBuilderDashboard", objXMLBuilderDashboardFilter);
        }

        public void ExportXMLBuilderDashboard()
        {
            try
            {
                if (Session["viewXMLBuilderDashboardFilter"] != null)
                {
                    XMLBuilderDashboardFilter objViewFilter = (XMLBuilderDashboardFilter)Session["viewXMLBuilderDashboardFilter"];
                    objViewFilter.pageno = 0;
                    objViewFilter.pagerecord = 0;
                    var exportData = new BLXMLBuilderDashboard().GetXMLBuilderDashboard(objViewFilter);
                    DataTable dtReport = new DataTable();
                    dtReport = Utility.MiscHelper.ListToDataTable(exportData);
                    dtReport.Columns.Remove("totalrecords");
                    dtReport.Columns["file_name"].ColumnName = "File Name";
                    dtReport.Columns["nas_status"].ColumnName = "NAS Status";
                    dtReport.Columns["created_by"].ColumnName = "Created By";
                    dtReport.Columns["created_on"].ColumnName = "Created On";
                    dtReport.Columns["reset_by"].ColumnName = "Reset By";
                    dtReport.Columns["reset_on"].ColumnName = "Reset On";
                    var Exportfilename = "XMLBuilderDashboard_Report";
                    dtReport.TableName = "XMLBuilderDashboardList";
                    ExportData(dtReport, Exportfilename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        [HttpPost]
        public ActionResult GetRouteAssetDetails(string latitude, string longitude, int bufferInMtrs)
        {
            var lst = new RouteAssetDeatils();
            //var lst = new List<RouteAssetDeatils>();
            try
            {
                lst = new BLRouteAssetDetails().getRouteDetails(latitude, longitude);
                //lst = JsonConvert.DeserializeObject<RouteAssetDeatils>("{\"state\":\"Telangana\",\"urid\":\"DEPL/AP/03\",\"depl_section_name\":\"DEPL/AP/03-RAMAYAMPET-NARSINGI-CHEGUNTA-TUPRAN-HYDERABAD(HAKIMPET)\",\"google_length\":67633.30299999999,\"civil_route_length\":67626.4519296875,\"section_length\":8966.685,\"backbone_civil_length\":63225.410929687496,\"last_mile_civil_length\":4401.041,\"optical_cable_length\":148221.73800280766,\"aerial_route_length\":4772.5279981689455,\"total_duct\":23,\"total_row_permission_received\":\"0\",\"network_cable_length\":[{\"cable_length\":{\"substring\":\"DEPL/AP/03/D1\",\"right\":\"21\",\"sum\":32.22}},{\"cable_length\":{\"substring\":\"DEPL/AP/03/D1\",\"right\":\"C1\",\"sum\":65347.50001135254}},{\"cable_length\":{\"substring\":\"DEPL/AP/03/D2\",\"right\":\"C1\",\"sum\":65159.70999328614}},{\"cable_length\":{\"substring\":\"DEPL/AP/03/D2\",\"right\":\"C2\",\"sum\":10122.68}}],\"duct_fiber_details\":[{\"fiber_details\":{\"duct_name\":\"DEPL/AP/03/D1\",\"cable_name\":\"DEPL/AP/03/D1/C1\",\"total_core\":48,\"used_fiber_core\":6,\"reserved_fiber_core\":2,\"available_fiber_core\":40,\"duct_status\":\"USED\",\"feasibility\":\"Fiber Feasible\"}},{\"fiber_details\":{\"duct_name\":\"DEPL/AP/03/D2\",\"cable_name\":\"DEPL/AP/03/D2/C1\",\"total_core\":48,\"used_fiber_core\":6,\"reserved_fiber_core\":0,\"available_fiber_core\":42,\"duct_status\":\"USED\",\"feasibility\":\"Fiber Feasible\"}},{\"fiber_details\":{\"duct_name\":\"DEPL/AP/03/D2\",\"cable_name\":\"DEPL/AP/03/D2/C2\",\"total_core\":48,\"used_fiber_core\":0,\"reserved_fiber_core\":0,\"available_fiber_core\":48,\"duct_status\":\"USED\",\"feasibility\":\"Fiber Feasible\"}},{\"fiber_details\":{\"duct_name\":\"DEPL/AP/03/D2\",\"cable_name\":\"DEPL/AP/03/D2/C1\",\"total_core\":48,\"used_fiber_core\":4,\"reserved_fiber_core\":0,\"available_fiber_core\":44,\"duct_status\":\"USED\",\"feasibility\":\"Fiber Feasible\"}}],\"row_status\":[{\"row_details\":{\"row_section_name\":\"SBI NARSINGI TO K NARAYAN COLLEGE\",\"depl_row_document_no\":\"\",\"row_authority\":\"MCGM-B\",\"row_permission_length\":605,\"application_no\":\"APPLICATION NO- ZM/APIIC/CBZ/IALA/NNK/ROAD CUTTING/2013-14/738\\r\\n\",\"application_received_date\":\"02-Nov-22\",\"total_row_permission_received\":925}},{\"row_details\":{\"row_section_name\":\"RPT001(RAMYAMPET) TO CHOWK\",\"depl_row_document_no\":\"\",\"row_authority\":\"MCGM-A\",\"row_permission_length\":320,\"application_no\":\"APPLICATION NO-ZM/APIIC/CBZ/IALA/NNK/ROAD CUTTING/2013-14/738\\r\\n\",\"application_received_date\":\"23-Oct-22\",\"total_row_permission_received\":925}}],\"customer_details\":[{\"customer_table\":{\"no_of_customer\":\"cust 1\",\"customer_name\":\"Sify\",\"unique_po_no\":\"Po1\",\"customer_live_fiber_in_duct\":\"1) DEPL/AP/01/D1 2) DEPL/AP/01/D2\",\"total_hoto_length\":\"47.8\",\"section_hoto_length\":\"9.87\",\"available_optical_cable\":\"9.87\",\"link_in_pair\":\"4 Pair\"}}]}");
                lst = JsonConvert.DeserializeObject<RouteAssetDeatils>("{\"state\":\"Telangana\",\"urid\":\"DEPL/AP/03\",\"depl_section_name\":\"DEPL/AP/03-RAMAYAMPET-NARSINGI-CHEGUNTA-TUPRAN-HYDERABAD(HAKIMPET)\",\"google_length\":67633.303,\"civil_route_length\":67626.4519296875,\"section_length\":7450.6708984375,\"backbone_civil_length\":63225.4109296875,\"last_mile_civil_length\":4401.041,\"optical_cable_length\":148221.738002808,\"aerial_route_length\":4772.52799816895,\"total_duct\":24,\"total_row_permission_received\":925.00,\"network_cable_length\":[{\"cable_length\":{\"substring\":\"DEPL/AP/03/D1\",\"right\":\"21\",\"sum\":32.22}},{\"cable_length\":{\"substring\":\"DEPL/AP/03/D1\",\"right\":\"C1\",\"sum\":65347.5000113525}},{\"cable_length\":{\"substring\":\"DEPL/AP/03/D2\",\"right\":\"C1\",\"sum\":65159.7099932861}},{\"cable_length\":{\"substring\":\"DEPL/AP/03/D2\",\"right\":\"C2\",\"sum\":10122.68}}],\"duct_fiber_details\":[{\"fiber_details\":{\"duct_name\":\"DEPL/AP/03/D1\",\"cable_name\":\"DEPL/AP/03/D1/C1\",\"total_core\":48,\"used_fiber_core\":6,\"reserved_fiber_core\":2,\"available_fiber_core\":40,\"duct_status\":\"USED\",\"feasibility\":\"Fiber Feasible\"}},{\"fiber_details\":{\"duct_name\":\"DEPL/AP/03/D2\",\"cable_name\":\"DEPL/AP/03/D2/C2\",\"total_core\":48,\"used_fiber_core\":0,\"reserved_fiber_core\":0,\"available_fiber_core\":48,\"duct_status\":\"USED\",\"feasibility\":\"Fiber Feasible\"}},{\"fiber_details\":{\"duct_name\":\"DEPL/AP/03/D2\",\"cable_name\":\"DEPL/AP/03/D2/C1\",\"total_core\":48,\"used_fiber_core\":4,\"reserved_fiber_core\":0,\"available_fiber_core\":44,\"duct_status\":\"USED\",\"feasibility\":\"Fiber Feasible\"}},{\"fiber_details\":{\"duct_name\":\"DEPL/AP/03/D3\",\"cable_name\":\"\",\"total_core\":0,\"used_fiber_core\":0,\"reserved_fiber_core\":0,\"available_fiber_core\":0,\"duct_status\":\"SPARE\",\"feasibility\":\"DUCT+FIBER FEASIBLE\"}}],\"row_status\":[{\"row_details\":{\"row_section_name\":\"SBI NARSINGI TO K NARAYAN COLLEGE\",\"depl_row_document_no\":\"\",\"row_authority\":\"MCGM-B\",\"row_permission_length\":605.00,\"application_no\":\"APPLICATION NO- ZM/APIIC/CBZ/IALA/NNK/ROAD CUTTING/2013-14/738\\r\\n\",\"application_received_date\":\"02-Nov-22\",\"total_row_permission_received\":\"0\"}},{\"row_details\":{\"row_section_name\":\"RPT001(RAMYAMPET) TO CHOWK\",\"depl_row_document_no\":\"\",\"row_authority\":\"MCGM-A\",\"row_permission_length\":320.00,\"application_no\":\"APPLICATION NO-ZM/APIIC/CBZ/IALA/NNK/ROAD CUTTING/2013-14/738\\r\\n\",\"application_received_date\":\"23-Oct-22\",\"total_row_permission_received\":\"0\"}}],\"customer_details\":[{\"customer_table\":{\"no_of_customer\":\"Cust 1\",\"customer_name\":\"Airtel\",\"unique_po_no\":\"1234567\",\"customer_live_fiber_in_duct\":\"1) DEPL/AP/01/D1 2) DEPL/AP/01/D2\",\"total_hoto_length\":12,\"section_hoto_length\":\"0\",\"available_optical_cable\":\"0\",\"link_in_pair\":\"4\",\"section_name\":\"A to B\",\"po_end_date\":\"01-07-2036\",\"live_core\":3,\"status\":\"Active\"}},{\"customer_table\":{\"no_of_customer\":\"Cust 2\",\"customer_name\":\"VIL\",\"unique_po_no\":\"98765\",\"customer_live_fiber_in_duct\":\"1) DEPL/AP/01/D1 2) DEPL/AP/01/D2\",\"total_hoto_length\":1,\"section_hoto_length\":\"0\",\"available_optical_cable\":\"0\",\"link_in_pair\":\"10\",\"section_name\":\"B TO C\",\"po_end_date\":\"\",\"live_core\":10,\"status\":\"ACTIVE\"}}]}");
                // lst = JsonConvert.DeserializeObject<RouteAssetDeatils>("{\"state\":null,\"urid\":\"GGN-TRE000001\",\"depl_section_name\":null,\"google_length\":105.39,\"civil_route_length\":null,\"section_length\":null,\"backbone_civil_length\":null,\"last_mile_civil_length\":null,\"optical_cable_length\":null,\"aerial_route_length\":null,\"total_duct\":1,\"total_row_permission_received\":null,\"network_cable_length\":null,\"duct_fiber_details\":[{\"fiber_details\":{\"duct_name\":\"GGN-DUC000001\",\"cable_name\":\"\",\"total_core\":0,\"used_fiber_core\":0,\"reserved_fiber_core\":0,\"available_fiber_core\":0,\"duct_status\":\"SPARE\",\"feasibility\":\"DUCT+FIBER FEASIBLE\"}}],\"row_status\":null,\"customer_details\":null}");
                //if (lst != null)
                //{
                //    if (lst.civil_route_length == null)
                //    {
                //        lst.civil_route_length = 0;

                //    }
                //}
                var all = from cablename in lst.duct_fiber_details[0].fiber_details.cable_name select cablename;

                all = all.OrderByDescending(cablename => ((short)cablename));


            }
            catch (Exception ex)
            {

            }
            return View("GetRouteAssetDetails", lst);

            

        }


        public ActionResult BoundaryPushToGis(BoundaryPushFilter objBoundaryPush, int page = 0, string sort = "", string sortdir = "")
        {
            //int page = 0,
            var usrDetail = (User)Session["userDetail"];
            string gisDesignId = new BLTicketType().GetDesignId(objBoundaryPush.system_id, objBoundaryPush.entity_type);
            if (!string.IsNullOrEmpty(gisDesignId))
            {
                objBoundaryPush.VersionName = "R4G_FTTX." + gisDesignId;
            }
            //BIND SERACH BY DROPDOWN.. STATIC VALUES
            BindSearchBy(objBoundaryPush);
            objBoundaryPush.viewBoundaryPush.pageSize = 10;
            objBoundaryPush.viewBoundaryPush.currentPage = page == 0 ? 1 : page;
            objBoundaryPush.viewBoundaryPush.sort = sort;
            objBoundaryPush.viewBoundaryPush.orderBy = sortdir;
            objBoundaryPush.pushboundaryDetailList = new BLLayer().GetBoundaryPushToGis(objBoundaryPush);
            var request_times = objBoundaryPush.pushboundaryDetailList.Where(C => C.request_time != null).Select(x => x.request_time).FirstOrDefault();
            objBoundaryPush.request_time = request_times.HasValue ? request_times.Value.ToString("dd,MMM yy hh:mm tt") : "";
            objBoundaryPush.lstUserModule = new BLLayer().GetUserModuleAbbrList(usrDetail.user_id, UserType.Web.ToString());
            objBoundaryPush.viewBoundaryPush.totalRecord = objBoundaryPush.pushboundaryDetailList != null && objBoundaryPush.pushboundaryDetailList.Count > 0 ? objBoundaryPush.pushboundaryDetailList[0].totalRecords : 0;
            Session["BoundarypushDashboard"] = objBoundaryPush.viewBoundaryPush;
            Session["Boundarypushlist"] = objBoundaryPush;
            objBoundaryPush.user_id=usrDetail.user_id;
            var _objData= new BLLayer().GetBoundaryPushStatus(objBoundaryPush);
            if (_objData != null)
            {
                objBoundaryPush.status = _objData.status;
                objBoundaryPush.process_message = _objData.process_message;
            }
            return PartialView("_BoundarypushDashboard", objBoundaryPush);

        }
        public ActionResult BoundaryPushToGisList(BoundaryPushFilter objBoundaryPush, int page = 0, string sort = "", string sortdir = "")
        {
            var usrDetail = (User)Session["userDetail"];
            var _objBoundaryPushFilter = (BoundaryPushFilter)Session["Boundarypushlist"];
            if (objBoundaryPush.system_id == 0)
            {
                objBoundaryPush.system_id = _objBoundaryPushFilter.system_id;
                objBoundaryPush.entity_type = _objBoundaryPushFilter.entity_type;
            }
            string gisDesignId = new BLTicketType().GetDesignId(objBoundaryPush.system_id, objBoundaryPush.entity_type);
            if (!string.IsNullOrEmpty(gisDesignId))
            {
                objBoundaryPush.VersionName = "R4G_FTTX." + gisDesignId;
            }
            //BIND SERACH BY DROPDOWN.. STATIC VALUES
            BindSearchBy(objBoundaryPush);
            objBoundaryPush.viewBoundaryPush.pageSize = 10;
            objBoundaryPush.viewBoundaryPush.currentPage = page == 0 ? 1 : page;
            objBoundaryPush.viewBoundaryPush.sort = sort;
            objBoundaryPush.viewBoundaryPush.orderBy = sortdir;
            objBoundaryPush.pushboundaryDetailList = new BLLayer().GetBoundaryPushToGis(objBoundaryPush);
            var request_times = objBoundaryPush.pushboundaryDetailList.Where(C => C.request_time != null).Select(x => x.request_time).FirstOrDefault();
            objBoundaryPush.request_time = request_times.HasValue ? request_times.Value.ToString("dd,MMM yy hh:mm tt") : "";
            objBoundaryPush.lstUserModule = new BLLayer().GetUserModuleAbbrList(usrDetail.user_id, UserType.Web.ToString());
            objBoundaryPush.viewBoundaryPush.totalRecord = objBoundaryPush.pushboundaryDetailList != null && objBoundaryPush.pushboundaryDetailList.Count > 0 ? objBoundaryPush.pushboundaryDetailList[0].totalRecords : 0;
            return PartialView("_BoundaryPushDashboardList", objBoundaryPush);

        }


        public IList<KeyValueDropDown> BindSearchBy(TemplateForDropDownBoundaryPush objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Boundary Type", value = "entity_type" });
            items.Add(new KeyValueDropDown { key = "Boundary Design ID", value = "gis_design_id" });
            return objTemplateForDropDown.lstBoundaryBindSearchBy = items.OrderBy(m => m.key).ToList();
        }


        //pk
        public void ExportBoundaryGetDataToList()
        {
            if (Session["BoundarypushDashboard"] != null)
            {
                BoundaryPushFilter objBoundaryPushFilter = new BoundaryPushFilter();
                objBoundaryPushFilter = (BoundaryPushFilter)Session["Boundarypushlist"];
                objBoundaryPushFilter.viewBoundaryPush = (ViewBoundaryPush)Session["BoundarypushDashboard"];
                objBoundaryPushFilter.viewBoundaryPush.currentPage = 0;
                objBoundaryPushFilter.viewBoundaryPush.pageSize = 0;   
                objBoundaryPushFilter.pushboundaryDetailList = new BLLayer().GetBoundaryPushToGis(objBoundaryPushFilter);
                DataTable dtReport = new DataTable();
                dtReport = Utility.MiscHelper.ListToDataTable(objBoundaryPushFilter.pushboundaryDetailList);
                dtReport.Columns.Remove("totalRecords");
                dtReport.Columns.Remove("S_No");
                dtReport.Columns.Remove("page_count");
                dtReport.Columns.Remove("modified_on");
                dtReport.Columns.Remove("system_id");
                dtReport.Columns.Remove("display_name");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("user_id");
                dtReport.Columns.Remove("entity_type");
                dtReport.Columns.Remove("VersionName");
                dtReport.Columns.Remove("process_message");

                dtReport.Columns["entity_title"].SetOrdinal(0);
                dtReport.Columns["gis_design_id"].SetOrdinal(1);
                dtReport.Columns["status"].SetOrdinal(2);
                dtReport.Columns["message"].SetOrdinal(3);
                dtReport.Columns["gis_object_id"].SetOrdinal(4);
                dtReport.Columns["user_name"].SetOrdinal(5);
                dtReport.Columns["request_time"].SetOrdinal(6);

                dtReport.Columns["entity_title"].ColumnName = "Boundary Type";
                dtReport.Columns["gis_design_id"].ColumnName = "Boundary Design ID";
                dtReport.Columns["status"].ColumnName = "Status";
                dtReport.Columns["message"].ColumnName = "GIS Message";
                dtReport.Columns["gis_object_id"].ColumnName = "GIS Object ID";
                dtReport.Columns["user_name"].ColumnName = "Last Pushed By";
                dtReport.Columns["request_time"].ColumnName = "Last Pushed On";
                var Exportfilename = "Boundary Push To Gis";
                dtReport.TableName = "pushboundaryDetailList";
                ExportBoundaryGetData(dtReport, Exportfilename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        private void ExportBoundaryGetData(DataTable dtReport, string fileName)
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

        public ActionResult SaveWorkAreaMarking(WorkAreaMarking objMarking)
        {
            JsonResponse<WorkAreaMarking> objResp = new JsonResponse<WorkAreaMarking>();
            try
            {
                objMarking.created_by = Convert.ToInt32(Session["user_id"]);
                var resObj = new BLWorkAreaMarking().SaveWorkAreaMarking(objMarking);
                if (resObj.status)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = resObj.message;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while saving workarea markings.";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Something went wrong while saving workarea markings.";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowWorkAreaMarkingOnMap(int id)
        {
            var objLayer = new BLWorkAreaMarking().GetWorkAreaMarkingById(id);
            return Json(objLayer, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWorkareaByWorkspaceId(int id)
        {
            WorkAreaMarking obj = new WorkAreaMarking();
            obj.lstMarkings = new BLWorkAreaMarking().GetWorkareaByWorkspaceId(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteMarkings(int workSpaceId)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BLWorkAreaMarking().DeleteMarkings(workSpaceId);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "WorkArea Marking Deleted Successfully!";
                }
                else if(output == -1)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "No Markings Found!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting workarea marking!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Something went wrong while deleting workarea marking!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

    }
}
