using BusinessLogics;
using BusinessLogics.VectorLayers;
using Models;
using Models.API;
using Models.VectorLayers;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Utility;

namespace SmartInventoryServices.Controllers
{
 //   [CustomAuthorization]
    [APIExceptionFilter]
    [Compression]
    //[CustomAction]   
    public class VectorLayerController : ApiController
    {
        // GET: VectorLayer
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]       
        public dynamic GetVectorData(ReqInput data)
        {
            VectorDataIn oVectorDataIn = ReqHelper.GetRequestData<VectorDataIn>(data);
            var response = new ApiResponse<dynamic>();

            var moduleAbbr = "NWTLYR";
           // List<ConnectionMaster> con = new BLLayer().GetConnectionString(moduleAbbr);
            ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);
			// foreach (var conn in con)
			// {
			if (con != null)
			{
				oVectorDataIn.connectionString = con.connection_string;
			}
			
           // }
            try
            {
                DateTime _FetchDateTime;               
                var obj = BLVectorLayers.Instance.GetAllLayersVector(oVectorDataIn, out _FetchDateTime);

                response.results = new { LayersData = obj, FetchDateTime = _FetchDateTime.ToString("yyyy-MM-dd HH:mm:ss") };
                response.status = ResponseStatus.OK.ToString();                             
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetVectorData()", "VectorLayer Controller", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error while getting poles vector data!";
            }
            return response;
        }
        [HttpPost]
        public dynamic GetVectorDataByGeom(ReqInput data)
        {
            VectorDataIn oVectorDataIn = ReqHelper.GetRequestData<VectorDataIn>(data);
            var response = new ApiResponse<dynamic>();
            var moduleAbbr = "NWTLYR";           
            ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);            
            if (con != null)
            {
                oVectorDataIn.connectionString = con.connection_string;
            }           
            try
            {
                DateTime _FetchDateTime;
                var obj = BLVectorLayers.Instance.GetAllLayersVectorByGeom(oVectorDataIn, out _FetchDateTime);

                response.results = new { LayersData = obj, FetchDateTime = _FetchDateTime.ToString("yyyy-MM-dd HH:mm:ss") };
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetVectorDataByGeom()", "VectorLayer Controller", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error while getting GetVectorDataByGeom vector data!";
            }
            return response;
        }
        [HttpPost]       
        public dynamic GetVectorDelta(ReqInput data)
        {
            VectorDeltaIn vectorDeltaIn = ReqHelper.GetRequestData<VectorDeltaIn>(data);

            var response = new ApiResponse<dynamic>();
            var moduleAbbr = "NWTLYR";
           // List<ConnectionMaster> con = new BLLayer().GetConnectionString(moduleAbbr);
            ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);
			//foreach (var conn in con)
			//{
			if (con != null)
			{
				vectorDeltaIn.connectionString = con.connection_string;
			}
			
           // }
            try
            {
                DateTime _FetchDateTime;
                var obj = BLVectorLayers.Instance.GetAllLayersDelta(vectorDeltaIn, out _FetchDateTime);

                response.results = new { LayersData = obj, FetchDateTime = _FetchDateTime.ToString("yyyy-MM-dd HH:mm:ss") };
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetVectorDelta()", "VectorLayer Controller", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error while getting VectorDelta data!";
            }
            return response;
        }

        [HttpPost]
        public dynamic GetVectorDeltaByGeom(ReqInput data)
        {
            VectorDeltaIn vectorDeltaIn = ReqHelper.GetRequestData<VectorDeltaIn>(data);

            var response = new ApiResponse<dynamic>();
            var moduleAbbr = "NWTLYR";          
            ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);         
            if (con != null)
            {
                vectorDeltaIn.connectionString = con.connection_string;
            }           
            try
            {
                DateTime _FetchDateTime;
                var obj = BLVectorLayers.Instance.GetAllLayersDeltaByGeom(vectorDeltaIn, out _FetchDateTime);

                response.results = new { LayersData = obj, FetchDateTime = _FetchDateTime.ToString("yyyy-MM-dd HH:mm:ss") };
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetVectorDelta()", "VectorLayer Controller", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error while getting VectorDelta data!";
            }
            return response;
        }

        [HttpPost]       
        public dynamic GetVectorProvinceData(ReqInput data)
        {
            VectorProvinceDataIn oVectorProvinceDataIn = ReqHelper.GetRequestData<VectorProvinceDataIn>(data);
            var response = new ApiResponse<dynamic>();
            try
            {
                var obj = BLVectorLayers.Instance.GetVectorProvinceData(oVectorProvinceDataIn);
                response.results = new { ProvinceData = obj};
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetVectorProvinceData()", "VectorLayer Controller", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error while getting province vector data!";
            }
            return response;
        }
        [HttpPost]
        public dynamic GetVectorEntityStyle()
        {            
            var response = new ApiResponse<dynamic>();
            try
            {
                var obj = BLVectorLayers.Instance.GetVectorEntityStyle();
                response.results = new { LayerAttribute = obj };
                response.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetVectorEntityStyle()", "VectorLayer Controller", null, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error while getting province vector data!";
            }
            return response;
        }
    }
}