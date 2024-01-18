using BusinessLogics;
using Models;
using Models.API;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Utility;

namespace SmartInventoryServices.Controllers
{
    //[Authorize]
    [CustomAuthorization]
    [HandleException]
    [RoutePrefix("api/building")]
    public class BuildingController : ApiController
    {
        /// <summary>
        /// Get the All Buildings inside the survey area and BBOX
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>             
        [HttpPost]
        [Route("GetBulidingInfo")]
        public ApiResponse<List<BuildingInfo>> GetBulidingInfo(ReqInput data)
        {
            var response = new ApiResponse<List<BuildingInfo>>();
            try
            {
                BuildinginfoIn objBuildinginfoIn = ReqHelper.GetRequestData<BuildinginfoIn>(data);

                if (string.IsNullOrEmpty(objBuildinginfoIn.EntityNetwork_Id))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Entity Network Id is required!";
                }
                else if (objBuildinginfoIn.SWLat == null)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "South west  latitude is required!";
                }
                else if (objBuildinginfoIn.SWLng == null)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "South west  longitude is required!";
                }
                else if (objBuildinginfoIn.NELng == null)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "North East longitude is required!";
                }
                else if (objBuildinginfoIn.NELat == null)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "North East latitude is required!";
                }
                var buildingInfo = BLBuilding.Instance.GetBulidingInfo(objBuildinginfoIn.EntityNetwork_Id, objBuildinginfoIn.EntityType, objBuildinginfoIn.SWLng, objBuildinginfoIn.SWLat, objBuildinginfoIn.NELng, objBuildinginfoIn.NELat, objBuildinginfoIn.user_id);
                response.status = StatusCodes.OK.ToString();
                response.results = buildingInfo;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetBulidingInfo()", "Building Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [Route("insertbuilding")]
        [HttpPost]              
        public ApiResponse<InsertUpdateBuildingOut> CreateBuilding(ReqInput data)
        {
            var response = new ApiResponse<InsertUpdateBuildingOut>();
            try
            {
                BuildingMaster objBuildinginfo = ReqHelper.GetRequestData<BuildingMaster>(data);

                var isBuildingExist = BLBuilding.Instance.getbuildingatSameLocation(objBuildinginfo.latitude, objBuildinginfo.longitude);//BLBuilding.Instance.getNearbybuilding(objBuildinginfo.latitude, objBuildinginfo.longitude,0);

                if (isBuildingExist)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "This building already added successfully!";
                    return response;
                }
                if (string.IsNullOrEmpty(objBuildinginfo.building_name))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Building Name is required!";
                    return response;
                }
                //else if (string.IsNullOrEmpty(objBuildinginfo.building_type))
                //{
                //    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //    response.error_message = "Building Type is required!";
                //    return response;
                //}
                else if (string.IsNullOrEmpty(Convert.ToString(objBuildinginfo.home_pass)))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Home Pass is required!";
                    return response;
                }
                else if (string.IsNullOrEmpty(Convert.ToString(objBuildinginfo.business_pass)))
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "Business Pass is required!";
                    return response;
                }
                //else if (objBuildinginfo.home_pass <= 0)
                //{
                //    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //    response.error_message = "Home Pass should be greater then 0!";
                //    return response;
                //}
                //else if (objBuildinginfo.business_pass <= 0)
                //{
                //    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //    response.error_message = "Business Pass should be greater then 0!";
                //    return response;
                //}
               
               else if (objBuildinginfo.category==BuildingCategory.Residential.ToString())
                {
                    if (objBuildinginfo.home_pass <= 0)
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Home Pass should be greater then 0!";
                        return response;
                    }
                }
                else if (objBuildinginfo.category == BuildingCategory.Commercial.ToString())
                {
                    if (objBuildinginfo.business_pass <= 0)
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Business Pass should be greater then 0!";
                        return response;
                    }
                }
                else if (objBuildinginfo.category == BuildingCategory.Both.ToString())
                {
                    if (objBuildinginfo.home_pass <= 0)
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Home Pass should be greater then 0!";
                        return response;
                    }
                    else if (objBuildinginfo.business_pass <= 0)
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Business Pass should be greater then 0!";
                        return response;
                    }
                }
                if(!string.IsNullOrEmpty(objBuildinginfo.tenancy))
                {
                   
                    var dropdownlist = new BLMisc().GetDropDownList(EntityType.Building.ToString());
                    var objTenancy = dropdownlist.Where(x => x.dropdown_key.ToUpper() == objBuildinginfo.tenancy.ToUpper() && x.dropdown_type == DropDownType.Tenancy.ToString()).FirstOrDefault();
                    objBuildinginfo.tenancy = objTenancy != null ? objTenancy.dropdown_value : "";
                    var objsubcategory = dropdownlist.Where(x => x.dropdown_key.ToUpper() == objBuildinginfo.subcategory.ToUpper() && x.dropdown_type == DropDownType.SubCategory.ToString()).FirstOrDefault();
                    objBuildinginfo.subcategory = objsubcategory != null ? objsubcategory.dropdown_value : "";

                }
                bool areaAssign = BLBuilding.Instance.checkAreaAssign(objBuildinginfo.surveyarea_id, objBuildinginfo.userid);
                if (areaAssign)
                {
                    objBuildinginfo.geom = objBuildinginfo.longitude + " " + objBuildinginfo.latitude;

                    //NEW ENTITY->Fill Region and Province Detail..
                    fillRegionProvinceDetail(objBuildinginfo, GeometryType.Point.ToString(), objBuildinginfo.geom);
                    //Fill Parent detail...              
                    fillParentDetail(objBuildinginfo, new NetworkCodeIn() { eType = EntityType.Building.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBuildinginfo.geom }, objBuildinginfo.networkIdType);

                    // fill latlong values
                    string[] lnglat = objBuildinginfo.geom.Split(new string[] { " " }, StringSplitOptions.None);
                    objBuildinginfo.latitude = Convert.ToDouble(lnglat[1].ToString());
                    objBuildinginfo.longitude = Convert.ToDouble(lnglat[0].ToString());
                    //fill survey area detail   

                    objBuildinginfo.created_on = DateTimeHelper.Now;
                    objBuildinginfo.created_by = objBuildinginfo.userid;
                    
                    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Building.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBuildinginfo.geom });
                    objBuildinginfo.network_id = objNetworkCodeDetail.network_code;
                    objBuildinginfo.sequence_id = objNetworkCodeDetail.sequence_id; 
                    var result = BLBuilding.Instance.SaveBuilding(objBuildinginfo, NetworkStatus.P);
                    ////-- UPDATE USER COMMENTS--
                    //BuildingComments objBuildingComments = new BuildingComments();
                    //objBuildingComments.building_system_id = result.system_id;
                    //objBuildingComments.comment = objBuildinginfo.user_comments;
                    //objBuildingComments.created_by = result.created_by;
                    //objBuildingComments.created_on = result.created_on;
                    //objBuildingComments.building_status = result.building_status;
                    //var res = new BLBuildingComment().insertBuildlingStatusComments(objBuildingComments);
                    //----------------
                    response.results = new InsertUpdateBuildingOut() { System_Id = result.system_id };
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "Building added successfully!"; ;
                }
                else
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Survy area not assigned to you. please contact to administrator!";
                }
            }
            catch (Exception ex)
            {
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error while process the request!";
            }

            return response;
        }


        [HttpPost]
        [Route("updatebuildinginfo")]
        public ApiResponse<InsertUpdateBuildingOut> UpdateBuildinginfo(ReqInput data)
        {
            var response = new ApiResponse<InsertUpdateBuildingOut>();
            try
            {
                BuildingMaster objBuildingMaster = ReqHelper.GetRequestData<BuildingMaster>(data);
                if (string.IsNullOrEmpty(objBuildingMaster.building_name))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Building Name is required!";
                    return response;
                }
                //else if (string.IsNullOrEmpty(objBuildingMaster.building_type))
                //{
                //    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //    response.error_message = "Building Type is required!";
                //    return response;
                //}
                else if (string.IsNullOrEmpty(Convert.ToString(objBuildingMaster.home_pass)))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Home Pass should be greater then 0!";
                    return response;
                }
                else if (string.IsNullOrEmpty(Convert.ToString(objBuildingMaster.business_pass)))
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "Business Pass should be greater then 0!";
                    return response;
                }
                //else if (objBuildingMaster.home_pass <= 0)
                //{
                //    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //    response.error_message = "Home Pass should be greater then 0!";
                //    return response;
                //}
                //else if (objBuildingMaster.business_pass <= 0)
                //{
                //    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                //    response.error_message = "Business Pass should be greater then 0!";
                //    return response;
                //}
                else if (objBuildingMaster.category == BuildingCategory.Residential.ToString())
                {
                    if (objBuildingMaster.home_pass <= 0)
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Home Pass should be greater then 0!";
                        return response;
                    }
                }
                else if (objBuildingMaster.category == BuildingCategory.Commercial.ToString())
                {
                    if (objBuildingMaster.business_pass <= 0)
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Business Pass should be greater then 0!";
                        return response;
                    }
                }
                else if (objBuildingMaster.category == BuildingCategory.Both.ToString())
                {
                    if (objBuildingMaster.home_pass <= 0)
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Home Pass should be greater then 0!";
                        return response;
                    }
                    else if (objBuildingMaster.business_pass <= 0)
                    {
                        response.status = StatusCodes.VALIDATION_FAILED.ToString();
                        response.error_message = "Business Pass should be greater then 0!";
                        return response;
                    }
                }
                if (!string.IsNullOrEmpty(objBuildingMaster.tenancy))
                {

                    var dropdownlist = new BLMisc().GetDropDownList(EntityType.Building.ToString());
                    var objTenancy = dropdownlist.Where(x => x.dropdown_key.ToUpper() == objBuildingMaster.tenancy.ToUpper() && x.dropdown_type == DropDownType.Tenancy.ToString()).FirstOrDefault();
                    objBuildingMaster.tenancy = objTenancy != null ? objTenancy.dropdown_value : "";
                    var objsubcategory = dropdownlist.Where(x => x.dropdown_key.ToUpper() == objBuildingMaster.subcategory.ToUpper() && x.dropdown_type == DropDownType.SubCategory.ToString()).FirstOrDefault();
                    objBuildingMaster.subcategory = objsubcategory != null ? objsubcategory.dropdown_value : "";
                }
                objBuildingMaster.bldAction = BuildingAction.Update; 
                var result = BLBuilding.Instance.SaveBuilding(objBuildingMaster, NetworkStatus.P);
                if (string.IsNullOrEmpty(result.pageMsg.message))
                { 
                    response.results = new InsertUpdateBuildingOut() { System_Id = result.system_id };
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "Building updated successfully!";
                }
                else
                {
                     response.status = result.pageMsg.status;
                    response.error_message = result.pageMsg.message;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateBuildinginfo()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }


        [HttpPost]
        [Route("updatebuildinggeometry")]
        public ApiResponse<string> UpdateBuildingGeom(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                UpdateBuildingGeomIn objUpdateBuildingGeomIn = ReqHelper.GetRequestData<UpdateBuildingGeomIn>(data);
                BLBuilding.Instance.UpdateBuildingGeom(objUpdateBuildingGeomIn.latitude, objUpdateBuildingGeomIn.longitude, objUpdateBuildingGeomIn.system_Id, objUpdateBuildingGeomIn.userId);
                response.status = StatusCodes.OK.ToString();
                response.error_message = "Building geometry updated successfully!";
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateBuildingGeom()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }


        [HttpPost]
        [Route("buildinginfo")]
        public ApiResponse<List<BuildingInfo>> BulidingInfoByStatus(ReqInput data)
        {
            var response = new ApiResponse<List<BuildingInfo>>();
            try
            {
                BulidingInfoByStatusIn objBulidingInfoByStatusIn = ReqHelper.GetRequestData<BulidingInfoByStatusIn>(data);
                if (objBulidingInfoByStatusIn.userId == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "User id is required!";
                    return response;
                }
                response.results = BLBuilding.Instance.getBuildingInfoByStatus(objBulidingInfoByStatusIn.userId, objBulidingInfoByStatusIn.surveyAreaId, objBulidingInfoByStatusIn.status,0).ToList(); ;
                response.status = StatusCodes.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("BulidingInfoByStatus()", "Main Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

        [HttpPost]
        [Route("Checknearbybuilding")]
        public ApiResponse<List<BuildingInfo>> Nearbybuilding(ReqInput data)
        {
            var response = new ApiResponse<List<BuildingInfo>>();
            try
            {
                NearByEntitiesIn objNearbybuildingIn = ReqHelper.GetRequestData<NearByEntitiesIn>(data);
                if (objNearbybuildingIn.bufferInMtrs <= 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid buffer!";
                    return response;
                }
                else if (objNearbybuildingIn.latitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid latitude!";
                    return response;
                }
                else if (objNearbybuildingIn.longitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid longitude!";
                    return response;
                }
                response.results = BLBuilding.Instance.getNearbybuilding(objNearbybuildingIn.latitude, objNearbybuildingIn.longitude, objNearbybuildingIn.bufferInMtrs).ToList(); ;
                response.status = StatusCodes.OK.ToString();
                if (response.results.Count > 0)
                {
                    response.error_message = "Near-by Buildings already exists on this location buffer.Try Another location!";
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("Nearbybuilding()", "Building Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

        private void fillRegionProvinceDetail(BuildingMaster objEntityModel, string enType, string geom)
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
        private void fillParentDetail(BuildingMaster objLib, NetworkCodeIn objIn, string networkIdType)
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

        #region GET Building Comment Info
        [HttpPost]
        public ApiResponse<List<BuildingComments>> getbuildingComments(ReqInput data) 
        {
            var response = new ApiResponse<List<BuildingComments>>();
            BuildingCommentsIn objBuildingCommentsIn = ReqHelper.GetRequestData<BuildingCommentsIn>(data);

            try
            {
                List<BuildingComments> objBuildingComments = new List<BuildingComments>();
                objBuildingComments = new BLBuildingComment().getbuildingComments(objBuildingCommentsIn.building_system_id); 
                response.results = objBuildingComments;
                response.status = StatusCodes.OK.ToString();
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getbuildingComments()", "BuildingController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region Get Building details by systemId
        [HttpPost]
        public ApiResponse<List<BuildingInfo>> getBuildingDetailsById(ReqInput data)
        {
            var response = new ApiResponse<List<BuildingInfo>>();
            BuildingDetailsIn objBuildingDetailsIn = ReqHelper.GetRequestData<BuildingDetailsIn>(data);

            try
            {
                response.results = BLBuilding.Instance.getBuildingInfoById(objBuildingDetailsIn.userId, 0,"",objBuildingDetailsIn.system_id).ToList();
                response.status = StatusCodes.OK.ToString(); 
                response.error_message = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getBuildingDetailsById()", "BuildingController", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region Get Building Locality SubLocality BY ANTRA
        [HttpPost]
        public ApiResponse<BuildingSurveyDetails> GetBuildingLocalitySubLocality(ReqInput data)
        {
            var response = new ApiResponse<BuildingSurveyDetails>();
            try
            {
                NearByEntitiesIn objBld = ReqHelper.GetRequestData<NearByEntitiesIn>(data);

                if (objBld.latitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid latitude!";
                    return response;
                }
                else if (objBld.longitude == 0)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Invalid longitude!";
                    return response;
                }
                response.results = BLBuilding.Instance.GetBuildingLocality(objBld.longitude, objBld.latitude);
                response.status = StatusCodes.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetBuildingLocalitySubLocality()", "Building Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion
    }
}
