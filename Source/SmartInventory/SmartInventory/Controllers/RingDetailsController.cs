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
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;


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
            Session["RingFilter"] = null;
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
                if (!string.IsNullOrEmpty(objRingFilter.objRingDetails.SearchbyRegionName)|| objRingFilter.objRingDetails.SearchbyRingTypes != null || !string.IsNullOrEmpty(objRingFilter.objRingDetails.SearchbySegmentName)) 
                {
                    region_id = objRingFilter.objRingDetails.SearchbyRegionName;
                    segment_code = objRingFilter.objRingDetails.SearchbySegmentName;
                    if(objRingFilter.objRingDetails.SearchbyRingTypes != null)
                        ring_code = string.Join("','", objRingFilter.objRingDetails.SearchbyRingTypes);
                }
                else {
                    region_id = objRingFilter.objRingDetails.region_name;
                    segment_code = objRingFilter.objRingDetails.segment_code;
                    ring_code = objRingFilter.objRingDetails.ring_capacity;
                }


            }
            Session["RingFilter"] = objRingFilter;
            var ringdetails = new BLRingDetails().getRingDetails(objRingFilter.objGridAttributes, region_id, segment_code, ring_code);

            objRingFilter.lstRingDetails = ringdetails;
           // objRingFilter.objRingDetails.SearchbyRingType = objRingFilter.objRingDetails.SearchbyRingTypes != null && objRingFilter.objRingDetails.SearchbyRingTypes.Count > 0 ? string.Join(",", objRingFilter.objRingDetails.SearchbyRingTypes.ToArray()) : "";
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

        [HttpPost]
        public ActionResult getRingConnectedElementDetail(int ring_id)
        {
            JsonResponse<vmRingConnectedElementDetails> objResp = new JsonResponse<vmRingConnectedElementDetails>();
           
            objResp.result = new BLRingDetails().getRingConnectedElementDetail(ring_id, Convert.ToInt32(Session["user_id"]));

            return Json(objResp.result, JsonRequestBehavior.AllowGet);
        }
        public string getSitGeometryDetail11(int ring_id)
        {
            string objresp = string.Empty;
            objresp = new BLDataUploader().getsiteShowOnMap(ring_id);
            return objresp;
        }
        public ActionResult GetSchematicView(string key, int ring_id,string ring_code)
        {
            var value = MiscHelper.Decrypt(key);
            var data = value.Split('/');
            int ringid = Convert.ToInt32(data[0]);
            string ringcode = Convert.ToString(data[1]);
            SLDModel obj = new SLDModel();
            obj = new BLOSPSplicing().GetSLDDiagrambyRingId(ringid, ringcode);
          
            if (!string.IsNullOrEmpty(obj.legends))
            {
                obj.lstlegend = JsonConvert.DeserializeObject<List<legend>>(obj.legends);
            }
            if (!string.IsNullOrEmpty(obj.cables))
            {
                obj.lstCableLegend = JsonConvert.DeserializeObject<List<CableLegend>>(obj.cables);
            }
            // obj.title = pSLDType; primary done after discuss with Deepak yadav Sir
            obj.title = "Primary";
            obj.ring_id = ring_id;
            obj.ring_code = ringcode;
            return PartialView("_SLDdiagramRingDetails", obj);
        }
        public void ExportRingDetails()
        {
            RingDetailsFiltter objRingFilter = new RingDetailsFiltter();
            objRingFilter=(RingDetailsFiltter) Session["RingFilter"];

            string region_id = "";
            string segment_code = "";
            string ring_code = "";
            objRingFilter.objGridAttributes.pageSize = 0;
            objRingFilter.objGridAttributes.currentPage = 0;
            //objRingFilter.objGridAttributes.sort = "";
            //objRingFilter.objGridAttributes.orderBy = "";
            if (objRingFilter.objRingDetails != null)
            {
                if (!string.IsNullOrEmpty(objRingFilter.objRingDetails.SearchbyRegionName) || objRingFilter.objRingDetails.SearchbyRingTypes != null || !string.IsNullOrEmpty(objRingFilter.objRingDetails.SearchbySegmentName))
                {
                    region_id = objRingFilter.objRingDetails.SearchbyRegionName;
                    segment_code = objRingFilter.objRingDetails.SearchbySegmentName;
                    if(objRingFilter.objRingDetails.SearchbyRingTypes != null)
                    ring_code = string.Join("','", objRingFilter.objRingDetails.SearchbyRingTypes);
                }
                else
                {
                    region_id = objRingFilter.objRingDetails.region_name;
                    segment_code = objRingFilter.objRingDetails.segment_code;
                    ring_code = objRingFilter.objRingDetails.ring_capacity;
                }


            }

            var ringdetails =new BLRingDetails().getRingDetails(objRingFilter.objGridAttributes, region_id, segment_code, ring_code);
            DataTable dtReport = new DataTable();
            dtReport = MiscHelper.ListToDataTable<RingDetails>(ringdetails);
            dtReport.Columns.Remove("NETWORK_ID");
            dtReport.Columns.Remove("POD_NAME");
            dtReport.Columns.Remove("RING_SITE_ID");
            dtReport.Columns.Remove("TOTALRECORDS");
            dtReport.Columns.Remove("ID");
            dtReport.Columns.Remove("SEARCHBYREGIONNAME");
            dtReport.Columns.Remove("SEARCHBYSEGMENTNAME");
            dtReport.Columns.Remove("SEARCHBYRINGTYPE");
            dtReport.Columns.Remove("SEARCHBYRINGTYPES");
            //dtReport.Columns.Remove("SEARCHBYRINGTYPE");
            dtReport.Columns["SEGMENT_CODE"].SetOrdinal(0);
            dtReport.Columns["RING_CODE"].SetOrdinal(1);
            dtReport.Columns["SITE_ID"].SetOrdinal(2);
            dtReport.Columns["SITE_NAME"].SetOrdinal(3);
            dtReport.Columns["region_name"].SetOrdinal(4);
            dtReport.Columns["AGG1_SITE_ID"].SetOrdinal(5);
            dtReport.Columns["AGG2_SITE_ID"].SetOrdinal(6);
            dtReport.Columns["RING_CAPACITY"].SetOrdinal(7);
            dtReport.Columns["DESCRIPTION"].SetOrdinal(8);
            dtReport.Columns["bh_status"].SetOrdinal(9);
            dtReport.Columns["Peer1"].SetOrdinal(10);
            dtReport.Columns["Peer2"].SetOrdinal(11);
            dtReport.Columns["ring_a_site_distance"].SetOrdinal(12);
            dtReport.Columns["ring_b_site_distance"].SetOrdinal(13);
            dtReport.Columns["region_name"].ColumnName = "Region";
            dtReport.Columns["SEGMENT_CODE"].ColumnName = "Segment";
            dtReport.Columns["RING_CODE"].ColumnName = "Ring Code";
            dtReport.Columns["SITE_ID"].ColumnName = "Site Id";
            dtReport.Columns["SITE_NAME"].ColumnName = "Site Name";
            dtReport.Columns["AGG1_SITE_ID"].ColumnName = "Aggrigate Site 1";
            dtReport.Columns["AGG2_SITE_ID"].ColumnName = "Aggrigate Site 2";
            dtReport.Columns["RING_CAPACITY"].ColumnName = "Capacity";
            dtReport.Columns["DESCRIPTION"].ColumnName = "Description";
            dtReport.Columns["bh_status"].ColumnName = "Site Status of the Access Ring";
            dtReport.Columns["Peer1"].ColumnName = "Peer Site 1";
            dtReport.Columns["Peer2"].ColumnName = "Peer Site 2";
            dtReport.Columns["ring_a_site_distance"].ColumnName = "Distances From Peer Site 1 (KM) ";
            dtReport.Columns["ring_b_site_distance"].ColumnName = "Distances From Peer Site 2 (KM)";
            var filename = "RingDetails";
            
            ExportRingDetails(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        }
        private void ExportRingDetails(DataTable dtReport, string fileName)
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
        //public static IWorkbook DataTableToExcel(string format, DataTable dt)
        //{
        //    IWorkbook workbook = format == "xls" ? (IWorkbook)new HSSFWorkbook() : new XSSFWorkbook();
        //    ISheet sheet = workbook.CreateSheet(dt.TableName);

        //    // Create header row
        //    IRow headerRow = sheet.CreateRow(0);
        //    for (int i = 0; i < dt.Columns.Count; i++)
        //    {
        //        ICell cell = headerRow.CreateCell(i);
        //        cell.SetCellValue(dt.Columns[i].ColumnName);
        //    }

        //    // Add data rows
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        IRow row = sheet.CreateRow(i + 1);
        //        for (int j = 0; j < dt.Columns.Count; j++)
        //        {
        //            ICell cell = row.CreateCell(j);
        //            cell.SetCellValue(dt.Rows[i][j]?.ToString() ?? string.Empty);
        //        }
        //    }

        //    // **Enable Sorting (AutoFilter)**
        //    CellRangeAddress filterRange = new CellRangeAddress(0, dt.Rows.Count, 0, dt.Columns.Count - 1);
        //    sheet.SetAutoFilter(filterRange);

        //    // Adjust column widths
        //    for (int i = 0; i < dt.Columns.Count; i++)
        //    {
        //        sheet.AutoSizeColumn(i);
        //    }

        //    return workbook;
        //}
    }
}