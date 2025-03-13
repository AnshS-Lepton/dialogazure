using BusinessLogics;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Models;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;
using Models.API;
using NetTopologySuite.Noding;


namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class RingDetailsController : Controller
    {
        // GET: RingDetails
        public ActionResult ShowTopologyRingDetails(RingDetailsFiltter objRingFilter, int page = 0, string sort = "", string sortdir = "")
        {
            string SearchVar = "";
            string region_id = "";
            string segment_code = "";
            string ring_code = "";
            objRingFilter.objGridAttributes.pageSize = 10;
            objRingFilter.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objRingFilter.objGridAttributes.sort = sort;
            objRingFilter.objGridAttributes.orderBy = sortdir;
            if ((objRingFilter.objGridAttributes.searchBy == "region_name" || objRingFilter.objGridAttributes.searchBy == "ring_code")
                && !string.IsNullOrEmpty(objRingFilter.objGridAttributes.searchText))
            {
                SearchVar = objRingFilter.objGridAttributes.searchText;
                objRingFilter.objGridAttributes.searchText = "";
                objRingFilter.objGridAttributes.currentPage = 0;
            }
            if (objRingFilter.objRingDetails != null)
            {
                region_id = objRingFilter.objRingDetails.region_name;
                segment_code = objRingFilter.objRingDetails.segment_code;
                ring_code = objRingFilter.objRingDetails.ring_capacity;


            }
            var ringdetails = new BLRingDetails().getRingDetails(objRingFilter.objGridAttributes, region_id, segment_code, ring_code);

            objRingFilter.lstRingDetails = ringdetails;
            objRingFilter.objGridAttributes.totalRecord = objRingFilter.lstRingDetails != null && objRingFilter.lstRingDetails.Count > 0 ? SearchVar != "" ? objRingFilter.lstRingDetails.Count : objRingFilter.lstRingDetails[0].totalRecords : 0;
            objRingFilter.lstRegionName = new BLRingDetails().GetRegionDetails();
            objRingFilter.lstSegmentName = new BLRingDetails().GetSegmentDetails();
            objRingFilter.lstRingType = new BLRingDetails().GetRingTypeDetails();

            return PartialView("_ShowTopologyRingDetails", objRingFilter);
        }

        [HttpPost]
        public ActionResult getSitGeometryDetail(int ring_id)
        {
            JsonResponse<vmGeomRingDetailIn> objResp = new JsonResponse<vmGeomRingDetailIn>();
            // var objGeometryDetail = new BLSearch().GetGeometryDetails(objGeomDetailIn);
            objResp.result = new BLRingDetails().getSiteDetails(ring_id);


            //if (objGeometryDetail.geometry_extent != null)
            //{
            //    var extent = objGeometryDetail.geometry_extent.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
            //    string[] bounds = extent.Split(',');
            //    string[] southWest = bounds[0].Split(' ');
            //    string[] northEast = bounds[1].Split(' ');
            //    objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
            //    objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
            //    objResp.result = objGeometryDetail;
            //    objResp.status = ResponseStatus.OK.ToString();
            //}
            //else
            //{
            //    objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            //}
            return Json(objResp.result, JsonRequestBehavior.AllowGet);
        }


        public string getSitGeometryDetail11(int ring_id)
        {
            string objresp = string.Empty;
            objresp = new BLDataUploader().getsiteShowOnMap(ring_id);
            return objresp;
        }
    }
}