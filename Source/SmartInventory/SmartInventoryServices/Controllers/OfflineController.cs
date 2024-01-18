using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using Models.API;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using Utility;

namespace SmartInventoryServices.Controllers
{
    [CustomAuthorization]
    [HandleException]
    [CustomAction]
    public class OfflineController : ApiController
    {
        #region GetAllVendorSpecifications 
        [HttpPost]
        public ApiResponse<List<Models.Admin.VendorSpecificationMaster>> GetAllVendorSpecifications(ReqInput data)
        {
            var response = new ApiResponse<List<Models.Admin.VendorSpecificationMaster>>();
            try
            {
                List<Models.Admin.VendorSpecificationMaster> lstobj = new BusinessLogics.Admin.BLVendorSpecification().GetAllVendorSpecifications();
                response.status = StatusCodes.OK.ToString();
                response.results = lstobj;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetAllVendorSpecifications()", "Offline Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion

        #region GetAllVendors 
        [HttpPost]
        public ApiResponse<List<Models.Admin.CreateVendor>> GetAllVendors(ReqInput data)
        {
            var response = new ApiResponse<List<Models.Admin.CreateVendor>>();
            try
            {
                List<Models.Admin.CreateVendor> lstobj = new BusinessLogics.Admin.BLVendor().GetAllVendorsData();
                response.status = StatusCodes.OK.ToString();
                response.results = lstobj;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetAllVendors()", "Offline Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion

        #region GetAllDropdownData 
        [HttpPost]
        public ApiResponse<List<Models.Admin.dropdown_master>> GetAllDropdownData(ReqInput data)
        {
            var response = new ApiResponse<List<Models.Admin.dropdown_master>>();
            try
            {
                List<Models.Admin.dropdown_master> lstobj = new BusinessLogics.Admin.BLVendorSpecification().GetAllDropdownData();
                response.status = StatusCodes.OK.ToString();
                response.results = lstobj;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetAllDropdownData()", "Offline Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion

        #region GetAllLayerMapping 
        [HttpPost]
        public ApiResponse<List<ParentChildLayerMapping>> GetAllLayerMapping(ReqInput data)
        {
            var response = new ApiResponse<List<ParentChildLayerMapping>>();
            try
            {
                List<ParentChildLayerMapping> layerMapping = new BLLayer().GetParentChildLayerMappings();
                response.status = StatusCodes.OK.ToString();
                response.results = layerMapping;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetAllLayerMapping()", "Offline Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion

        #region GetProvinceDetail 
        [HttpPost]
        public ApiResponse<List<_RegionProvince>> GetProvinceDetail(ReqInput data)
        {
            var response = new ApiResponse<List<_RegionProvince>>();
            _RegionProvince obj = ReqHelper.GetRequestData<_RegionProvince>(data);
            List<_RegionProvince> provDetail = new BLMisc().GetProvinceDetail(obj.province_id);
            response.status = StatusCodes.OK.ToString();
            response.results = provDetail;

            return response;
        }
        #endregion

        #region getLegendDetail
        [HttpPost]
        public ApiResponse<List<LegendDetail>> getLegendDetail(ReqInput data)
        {
            var response = new ApiResponse<List<LegendDetail>>();
            try
            {
                Legend objLegend = ReqHelper.GetRequestData<Legend>(data);
                response.results = new BLMisc().GetLegendDetail(objLegend.userId, objLegend.userRoleId);
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getLegnedDetail()", "Offline Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        #endregion


        #region getLayerColumnsSettingsDetail
        [HttpPost]
        public ApiResponse<List<layerColumnSettings>> getLayerColumnsSettingsDetail(ReqInput data)
        {
            var response = new ApiResponse<List<layerColumnSettings>>();
            try
            {
                response.results = BLAdvancedSettings.Instance.GetLayerColumnSettingsForOffline(LayerSettingType.Info.ToString());
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getLayerColumnsSettingsDetail()", "Offline Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;

        }
        #endregion

        #region getItemTemplateDetail
        [HttpPost]
        public ApiResponse<List<itemMaster>> getItemTemplateDetail(ReqInput data)
        {
            var response = new ApiResponse<List<itemMaster>>();
            Legend objLegend = ReqHelper.GetRequestData<Legend>(data);
            try
            {
                response.results = BLItemTemplate.Instance.GetLayerTemplateDetail(objLegend.userId);
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getItemTemplateDetail()", "Offline Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing Request.";
            }
            return response;
        }
        #endregion
    }
}