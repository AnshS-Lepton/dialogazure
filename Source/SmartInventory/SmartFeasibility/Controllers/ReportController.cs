using BusinessLogics.Feasibility;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Models;
using Models.Feasibility;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using SmartFeasibility.Filters;
using SmartFeasibility.Helper;
using SmartFeasibility.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Utility;


namespace SmartFeasibility.Controllers
{
    [HandleException]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
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

                Response.End();
            }
        }

        private void DownloadFeasibilityPDF()
        {
            if (Session["XLSData"] != null)
            {
                try
                {
                    List<FeasibilityReportData> lstReportData = (List<FeasibilityReportData>)Session["XLSData"];
                    if (lstReportData.Count > 0)
                    {
                        DataTable dtReport = GetFeasibilityReportData(lstReportData);

                        Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 60f, 40f);
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                        pdfWriter.PageEvent = new ReportHelper<FeasibilityReportData>.AllPdfPageEvents();
                        pdfDoc.Open();
                        PdfPTable tableLayout = new PdfPTable(2);
                        pdfDoc.Add(ReportHelper<FeasibilityReportData>.Add_Phrase_To_PDF(new PdfPTable(1), "Feasibility Details"));
                        pdfDoc.Add(new Paragraph(" "));
                        pdfDoc.Add(ReportHelper<FeasibilityReportData>.Add_Export_Content_To_PDF(tableLayout, dtReport));

                        // second table
                        List<FeasibilityInsideCableReport> insideCables = lstReportData[0].insideCables;
                        if (insideCables.Count > 0)
                        {
                            DataTable dtInsideCables = new DataTable();
                            dtInsideCables = MiscHelper.ListToDataTable(insideCables);

                            PdfPTable tableLayout2 = new PdfPTable(7);
                            pdfDoc.Add(new Paragraph(" "));
                            pdfDoc.Add(ReportHelper<FeasibilityReportData>.Add_Phrase_To_PDF(new PdfPTable(1), "Inside Cable Details"));
                            pdfDoc.Add(new Paragraph(" "));
                            pdfDoc.Add(ReportHelper<FeasibilityReportData>.Add_Content_To_PDF(tableLayout2, dtInsideCables));
                        }

                        GeneratePDF(pdfDoc, pdfWriter, tableLayout);
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void GeneratePDF(Document docPDF, PdfWriter pdfWriter, PdfPTable pdfTable)
        {
            string fileName = "Feasibiity_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
            pdfWriter.CloseStream = false;
            docPDF.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(docPDF);
            Response.End();
        }

        private void DownloadFeasibilityKML()
        {
            if (Session["KMLData"] != null)
            {
                StringBuilder sbLine = new StringBuilder();
                StringBuilder sbPoint = new StringBuilder();
                Dictionary<string, string> KMLHexColors = new Dictionary<string, string>();
                sbLine.Append("<Folder>");
                sbPoint.Append("<Folder>");
                try
                {
                    List<FeasibilityKMLData> lstFeasibilityKMLDate = (List<FeasibilityKMLData>)Session["KMLData"];

                    foreach (var objEntity in lstFeasibilityKMLDate)
                    {
                        if (objEntity.geom_type.ToUpper() == "LINE")
                        {
                            sbLine.Append("<Placemark><description>" + objEntity.description + "</description><name>" + objEntity.entity_title + "</name>");
                            sbLine.Append("<styleUrl>#" + objEntity.entity_type + "</styleUrl><LineString><coordinates>");
                            if (!string.IsNullOrEmpty(objEntity.geom))
                            {
                                sbLine.Append(objEntity.geom);
                            }
                            sbLine.Append("</coordinates></LineString></Placemark>");

                            if (!KMLHexColors.ContainsKey(objEntity.entity_type))
                            {
                                KMLHexColors.Add(objEntity.entity_type, objEntity.colorHex_8);
                            }
                        }
                        if (objEntity.geom_type.ToUpper() == "POINT")
                        {
                            sbPoint.Append("<Placemark><name>" + new XText(objEntity.entity_title) + "</name>");
                            sbPoint.Append("<description>" + objEntity.description + "</description>");
                            sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                            if (!string.IsNullOrEmpty(objEntity.geom))
                            {
                                sbPoint.Append(objEntity.geom);
                            }
                            sbPoint.Append("</coordinates></Point></Placemark>");
                        }
                    }

                    sbLine.Append("</Folder>");
                    sbPoint.Append("</Folder>");
                    string defaultHex = "#FF0000FF";

                    string finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                               "<Document>  <!-- Begin Style Definitions -->" +
                                "<Style id =\"inside_A\"><LineStyle><color>" + (KMLHexColors.ContainsKey("inside_A") ? KMLHexColors["inside_A"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                                "<Style id =\"inside\"><LineStyle><color>" + (KMLHexColors.ContainsKey("inside") ? KMLHexColors["inside"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                                "<Style id =\"inside_P\"><LineStyle><color>" + (KMLHexColors.ContainsKey("inside_P") ? KMLHexColors["inside_P"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                                "<Style id =\"outside_start\"><LineStyle><color>" + (KMLHexColors.ContainsKey("outside_start") ? KMLHexColors["outside_start"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                                "<Style id =\"outside_end\"><LineStyle><color>" + (KMLHexColors.ContainsKey("outside_end") ? KMLHexColors["outside_start"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                                "<Style id =\"lmc_start\"><LineStyle><color>" + (KMLHexColors.ContainsKey("lmc_start") ? KMLHexColors["lmc_start"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                                "<Style id =\"lmc_end\"><LineStyle><color>" + (KMLHexColors.ContainsKey("lmc_end") ? KMLHexColors["lmc_end"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href></Icon></IconStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                                sbPoint.ToString() + sbLine.ToString() + "</Document></kml>";


                    string attachment = "attachment; filename=export_Feasibility.kml";
                    Response.ClearContent();
                    Response.ContentType = "application/xml";
                    Response.AddHeader("content-disposition", attachment);
                    Response.Write(finalKMLString);
                    Response.End();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public string getColorByType(string type)
        {
            var clr = "#adc3ca";
            if (type == "outside_start" || type == "outside_end")
                clr = "#FF0000";
            else if (type == "inside_P")
                clr = "#95a832";
            else if (type == "inside_A" || type == "inside")
                clr = "#006400";
            if (type == "lmc_start" || type == "lmc_end")
                clr = "#0000FF";
            return clr;
        }
        public string getTitle(string type)
        {
            if (type == "inside_A")
                return "Inside As-built";
            else if (type == "inside_P")
                return "Inside Planned";
            else if (type == "inside")
                return "Inside";
            else if (type == "outside_start")
                return "Outside A";
            else if (type == "outside_end")
                return "Outside B";
            else if (type == "lmc_start")
                return "LMC A";
            else if (type == "lmc_end")
                return "LMC B";
            else
                return "";
        }
        public string getKMLHexCode(string code_6)
        {
            var Hex_8_dig = "#FF0000FF";
            if (code_6.Length > 0)
            {
                // KML expects color in AABBGGRR format where AA is Alpha (opaqueness), 00 is transparent, FF is full opaque
                Hex_8_dig = ("#FF" + code_6.Substring(5) + code_6.Substring(3, 2) + code_6.Substring(1, 2)).ToUpper();
            }
            return Hex_8_dig;
        }

        public void ExportMergedKML(string search_by, string searchText, string FromDate, string ToDate)
        {

            FromDate = FromDate == "" ? null : Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd");
            ToDate = ToDate == "" ? null : Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");

            CommonGridAttributes objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchBy = search_by;
            objGridAttributes.searchText = searchText;
            objGridAttributes.sort = "feasibility_id";
            objGridAttributes.orderBy = "desc";
            var listHistoryRecord = new BLFeasibilityHistory().getPastFeasibilyExportData<Dictionary<string, string>>(objGridAttributes, FromDate, ToDate);
            if (listHistoryRecord != null && listHistoryRecord.Count > 0)
            {
                string[] feasKMLData = new string[listHistoryRecord.Count];

                for (int i = 0; i < listHistoryRecord.Count; i++)
                {
                    FeasibilityKMLModel rec = new FeasibilityKMLModel();
                    rec.feasibility_name = listHistoryRecord[i]["feasibility_name"];
                    rec.history_id = listHistoryRecord[i]["history_id"];
                    rec.start_lat_lng = listHistoryRecord[i]["start_lat_lng"];
                    rec.end_lat_lng = listHistoryRecord[i]["end_lat_lng"];
                    rec.start_Point_Name = listHistoryRecord[i]["start_point_name"];
                    rec.end_Point_Name = listHistoryRecord[i]["end_point_name"];
                    feasKMLData[i] = JsonConvert.SerializeObject(rec);
                }

                DownloadKMZReport(JsonConvert.SerializeObject(feasKMLData));
            }

        }

        public void DownloadKMZReport(string feasibility_saved)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            List<FeasibilityKMLModel> lstFeasibilityKMLModel = new List<FeasibilityKMLModel>();
            string[] data = jsonSerializer.Deserialize<string[]>(feasibility_saved);


            foreach (string feasData in data)
            {
                FeasibilityKMLModel feasKMLData = jsonSerializer.Deserialize<FeasibilityKMLModel>(feasData);

                feasKMLData.lstFeasibilityGeom = new BLFeasibilityHistory().getFeasibilityDetails(Convert.ToInt16(feasKMLData.history_id));
                var ll1 = feasKMLData.start_lat_lng.Trim().Split(',');
                var ll2 = feasKMLData.end_lat_lng.Trim().Split(',');
                feasKMLData.lstKMLDataPoints = new List<FeasibilityKMLData>();
                foreach (FeasibilityCableGeoms item in feasKMLData.lstFeasibilityGeom)
                {
                    FeasibilityKMLData objFeasibilityKMLData = new FeasibilityKMLData();
                    var geom = "";
                    var geomArray = item.cable_geometry.Replace("LINESTRING(", "").Replace(")", "").Split(',');
                    foreach (var g in geomArray)
                    {
                        var latLng = g.Trim().Split(' ');
                        geom += latLng[0] + ',' + latLng[1] + ",0 ";
                    }
                    string distance = (Convert.ToDouble(item.cable_length) > 999) ? string.Format("{0:0.00} km", (Convert.ToDouble(item.cable_length) / 1000)) : string.Format("{0:0.00} m", (Convert.ToDouble(item.cable_length)));

                    string Description = "<![CDATA[<p><b>Distance:</b> " + distance + "<br/>";
                    Description += (item.cable_id != "" ? ("<b>Cable ID: </b>" + item.cable_id) + "<br/>" : "");
                    Description += (item.cable_name != "" ? ("<b>Cable Name: </b>" + item.cable_name) : "");
                    Description += "</p>]]>";
                    if (geom.Length > 0)
                    {
                        objFeasibilityKMLData.cable_id = item.cable_id;
                        objFeasibilityKMLData.cable_name = item.cable_name;
                        objFeasibilityKMLData.geom = geom;
                        objFeasibilityKMLData.description = Description;
                        objFeasibilityKMLData.geom_type = "LINE";
                        objFeasibilityKMLData.entity_name = item.cable_type;
                        objFeasibilityKMLData.entity_title = getTitle(item.cable_type);
                        objFeasibilityKMLData.entity_type = item.cable_type;
                        objFeasibilityKMLData.distance = item.cable_length;
                        objFeasibilityKMLData.colorHex_8 = getKMLHexCode(getColorByType(item.cable_type));
                    }
                    feasKMLData.lstKMLDataPoints.Add(objFeasibilityKMLData);
                }
                if (ll1.Length > 0)
                {
                    FeasibilityKMLData objFeasibilityKMLData = new FeasibilityKMLData();

                    objFeasibilityKMLData.geom = ll1[0] + "," + ll1[1] + ",0 ";
                    objFeasibilityKMLData.description = "<![CDATA[<h2>(" + ll1[0] + "," + ll1[1] + ")</h2>]]>";
                    objFeasibilityKMLData.geom_type = "POINT";
                    objFeasibilityKMLData.entity_name = "Start Point";
                    objFeasibilityKMLData.entity_title = (feasKMLData.start_Point_Name != null && feasKMLData.start_Point_Name.Length > 0) ? new XText(feasKMLData.start_Point_Name).ToString() : "Start Point";
                    feasKMLData.lstKMLDataPoints.Add(objFeasibilityKMLData);
                }
                if (ll2.Length > 0)
                {
                    FeasibilityKMLData objFeasibilityKMLData = new FeasibilityKMLData();
                    objFeasibilityKMLData.geom = ll2[0] + "," + ll2[1] + ",0 ";
                    objFeasibilityKMLData.description = "<![CDATA[<h2>(" + ll2[0] + "," + ll2[1] + ")</h2>]]>";
                    objFeasibilityKMLData.geom_type = "POINT";
                    objFeasibilityKMLData.entity_name = "End Point";
                    objFeasibilityKMLData.entity_title = (feasKMLData.end_Point_Name != null && feasKMLData.end_Point_Name.Length > 0) ? new XText(feasKMLData.end_Point_Name).ToString() : "End Point";
                    feasKMLData.lstKMLDataPoints.Add(objFeasibilityKMLData);
                }
                lstFeasibilityKMLModel.Add(feasKMLData);
            }
            string finalKMLString = MergeKML(lstFeasibilityKMLModel);
            DownloadMergeKML(finalKMLString);
        }

        public void DownloadKMZReportByIds(string historyIDs)
        {
            CommonGridAttributes objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchBy = "historIDs";
            objGridAttributes.searchText = historyIDs;
            objGridAttributes.sort = "feasibility_id";
            objGridAttributes.orderBy = "desc";
            var listHistoryRecord = new BLFeasibilityHistory().getPastFeasibilyExportData<Dictionary<string, string>>(objGridAttributes, "", "");
            if (listHistoryRecord != null && listHistoryRecord.Count > 0)
            {
                string[] feasKMLData = new string[listHistoryRecord.Count];

                for (int i = 0; i < listHistoryRecord.Count; i++)
                {
                    FeasibilityKMLModel rec = new FeasibilityKMLModel();
                    rec.feasibility_name = listHistoryRecord[i]["feasibility_name"];
                    rec.history_id = listHistoryRecord[i]["history_id"];
                    rec.start_lat_lng = listHistoryRecord[i]["start_lat_lng"];
                    rec.end_lat_lng = listHistoryRecord[i]["end_lat_lng"];
                    rec.start_Point_Name = listHistoryRecord[i]["start_point_name"];
                    rec.end_Point_Name = listHistoryRecord[i]["end_point_name"];
                    feasKMLData[i] = JsonConvert.SerializeObject(rec);
                }

                DownloadKMZReport(JsonConvert.SerializeObject(feasKMLData));
            }
        }

        public void DownloadKMZReportByIdsFtth(string historyIDs)
        {
            CommonGridAttributes objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchBy = "fh.history_id";
            objGridAttributes.searchText = historyIDs;
            objGridAttributes.sort = "feasibility_id";
            objGridAttributes.orderBy = "desc";
            var listHistoryRecordFTTH = new BLFeasibilityHistory().getPastFeasibilyExportDataFtth<Dictionary<string, string>>(objGridAttributes, "", "");
            if (listHistoryRecordFTTH != null && listHistoryRecordFTTH.Count > 0)
            {
                string[] feasKMLData = new string[listHistoryRecordFTTH.Count];

                for (int i = 0; i < listHistoryRecordFTTH.Count; i++)
                {
                    FeasibilityKMLFTTHModel rec = new FeasibilityKMLFTTHModel();
                    rec.feasibility_name = listHistoryRecordFTTH[i]["feasibility_name"];
                    rec.history_id = Convert.ToInt32(listHistoryRecordFTTH[i]["history_id"]);
                    rec.lat_lng = listHistoryRecordFTTH[i]["lat_lng"];
                    rec.entity_id = listHistoryRecordFTTH[i]["entity_id"];
                    rec.feasibility_id = Convert.ToInt32(listHistoryRecordFTTH[i]["feasibility_id"]);
                    rec.customer_id = listHistoryRecordFTTH[i]["customer_id"];
                    rec.customer_name = listHistoryRecordFTTH[i]["customer_name"];
                    rec.entity_loc = listHistoryRecordFTTH[i]["entity_loc"];
                    rec.path_distance = listHistoryRecordFTTH[i]["path_distance"];

                    feasKMLData[i] = JsonConvert.SerializeObject(rec);
                }

                DownloadKMZReportFTTH(JsonConvert.SerializeObject(feasKMLData));
            }
        }

        public void DownloadMergedExcel(string historyIDs)
        {
            CommonGridAttributes objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchBy = "historIDs";
            objGridAttributes.searchText = historyIDs;
            objGridAttributes.sort = "feasibility_id";
            objGridAttributes.orderBy = "desc";
            var history_data = new BLFeasibilityHistory().getPastFeasibilyExportData<Dictionary<string, string>>(objGridAttributes, "", "");

            //var history_data = new BLFeasibilityHistory().getFeasibilityReport<Dictionary<string, string>>(historyIDs);

            if (history_data != null && history_data.Count > 0)
            {
                List<MergedExcelModel> mergedExcelData = new List<MergedExcelModel>();
                List<InsideCablesMergedModel> insideCablesData = new List<InsideCablesMergedModel>();

                for (int i = 0; i < history_data.Count; i++)
                {
                    double totalLength = Math.Round(Convert.ToDouble(history_data[i]["inside_p"]) + Convert.ToDouble(history_data[i]["inside_a"]) +
                        Convert.ToDouble(history_data[i]["outside_a_end"]) + Convert.ToDouble(history_data[i]["outside_b_end"]) +
                        Convert.ToDouble(history_data[i]["lmc_a"]) + Convert.ToDouble(history_data[i]["lmc_b"]), 2);
                    double insidePercentage = Math.Round((Convert.ToDouble(history_data[i]["inside_p"]) + Convert.ToDouble(history_data[i]["inside_a"])) * 100 / totalLength, 2);
                    mergedExcelData.Add(new MergedExcelModel
                    {
                        History_ID = history_data[i]["history_display_id"],
                        Feasibility_ID = history_data[i]["feasibility_name"],
                        Customer_Name = history_data[i]["customer_name"],
                        Customer_ID = history_data[i]["customer_id"],
                        Start_Point_Name = history_data[i]["start_point_name"],
                        Start_Lng_lat = history_data[i]["start_lat_lng"],
                        End_Point_Name = history_data[i]["end_point_name"],
                        End_Lng_Lat = history_data[i]["end_lat_lng"],
                        Cores_Required = Convert.ToInt32(history_data[i]["cores_required"]),
                        Inside_Planned_Length = Math.Round(Convert.ToDouble(history_data[i]["inside_p"]), 2),
                        Inside_AsBuilt_Length = Math.Round(Convert.ToDouble(history_data[i]["inside_a"]), 2),
                        Outside_A_Length = Math.Round(Convert.ToDouble(history_data[i]["outside_a_end"]), 2),
                        Outside_B_Length = Math.Round(Convert.ToDouble(history_data[i]["outside_b_end"]), 2),
                        LMC_A_Length = Math.Round(Convert.ToDouble(history_data[i]["lmc_a"]), 2),
                        LMC_B_Length = Math.Round(Convert.ToDouble(history_data[i]["lmc_b"]), 2),
                        Cable_Type = history_data[i]["cable_type"],
                        is_core_feasibile = history_data[i]["feasibility_result"].Equals("Not Feasible", StringComparison.OrdinalIgnoreCase) ? history_data[i]["feasibility_result"] : history_data[i]["core_level_result"],
                        Status = history_data[i]["feasibility_result"],
                        Total_Length = totalLength,
                        Inside_Length_Percent = insidePercentage
                    });
                }

                // get inside cables data
                var inside_data = new BLFeasibilityHistory().getInsideCables<Dictionary<string, string>>(historyIDs);

                if (inside_data != null && inside_data.Count > 0)
                {

                    for (int i = 0; i < inside_data.Count; i++)
                    {
                        insideCablesData.Add(new InsideCablesMergedModel
                        {
                            history_id = inside_data[i]["historyId"],
                            cable_id = inside_data[i]["cable_id"],
                            cable_name = inside_data[i]["cable_name"],
                            network_status = inside_data[i]["network_status"],
                            total_cores = Convert.ToInt32(inside_data[i]["total_cores"]),
                            used_cores = Convert.ToInt32(inside_data[i]["used_cores"]),
                            available_cores = Convert.ToInt32(inside_data[i]["available_cores"]),
                            cable_length = Convert.ToDouble(inside_data[i]["cable_length"])
                        });
                    }
                }

                string fileName = "Merged_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                DataTable dtFeasibilityData = new DataTable();
                DataTable dtInsideCables = new DataTable();
                DataSet ds = new DataSet();
                dtFeasibilityData = MiscHelper.ListToDataTable(mergedExcelData);
                dtFeasibilityData = formatColumnNames(dtFeasibilityData);
                dtFeasibilityData.TableName = "Feasibility Details";

                foreach (DataColumn col in dtFeasibilityData.Columns)
                {
                    col.ColumnName = MiscHelper.ToCamelCase(col.ColumnName.ToLower());
                }

                ds.Tables.Add(dtFeasibilityData);

                if (insideCablesData.Count > 0)
                {
                    dtInsideCables = MiscHelper.ListToDataTable(insideCablesData);
                    dtInsideCables = formatColumnNames(dtInsideCables);

                    foreach (DataColumn col in dtInsideCables.Columns)
                    {
                        col.ColumnName = MiscHelper.ToCamelCase(col.ColumnName.ToLower());
                    }

                    dtInsideCables.TableName = "Inside Cable Details";

                    ds.Tables.Add(dtInsideCables);
                }

                ExportData(ds, fileName);

            }
        }

        public void DownloadMergedExcelFTTH(string historyIDs)
        {
            CommonGridAttributes objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchBy = "fh.history_id";
            objGridAttributes.searchText = historyIDs;
            objGridAttributes.sort = "feasibility_id";
            objGridAttributes.orderBy = "desc";
            var history_data = new BLFeasibilityHistory().getPastFeasibilyExportDataFtth<Dictionary<string, string>>(objGridAttributes, "", "");

            //var history_data = new BLFeasibilityHistory().getFeasibilityReport<Dictionary<string, string>>(historyIDs);

            if (history_data != null && history_data.Count > 0)
            {
                List<MergedExcelFTTHModel> mergedExcelData = new List<MergedExcelFTTHModel>();
                // List<InsideCablesMergedModel> insideCablesData = new List<InsideCablesMergedModel>();

                for (int i = 0; i < history_data.Count; i++)
                {
                    //double totalLength = Math.Round(Convert.ToDouble(history_data[i]["inside_p"]) + 
                    //    Convert.ToDouble(history_data[i]["inside_a"]) +
                    //    Convert.ToDouble(history_data[i]["outside_a_end"]) + 
                    //    Convert.ToDouble(history_data[i]["outside_b_end"]) +
                    //    Convert.ToDouble(history_data[i]["lmc_a"]) + 
                    //    Convert.ToDouble(history_data[i]["lmc_b"]), 2);
                    //double insidePercentage = Math.Round((Convert.ToDouble(history_data[i]["inside_p"]) + 
                    //    Convert.ToDouble(history_data[i]["inside_a"])) * 100 / totalLength, 2);
                    mergedExcelData.Add(new MergedExcelFTTHModel
                    {
                        History_ID = history_data[i]["history_display_id"],
                        Feasibility_ID = history_data[i]["feasibility_name"],
                        Customer_Name = history_data[i]["customer_name"],
                        Customer_ID = history_data[i]["customer_id"],
                        Lat_Lng = history_data[i]["lat_lng"],
                        Entity_Loc = history_data[i]["entity_loc"],
                        Entity_Id = history_data[i]["entity_id"],
                        Buffer_Radius = history_data[i]["buffer_radius"],
                        path_distance = history_data[i]["path_distance"]
                    });
                }

                // get inside cables data

                string fileName = "Merged_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                DataTable dtFeasibilityData = new DataTable();
                DataTable dtInsideCables = new DataTable();
                DataSet ds = new DataSet();
                dtFeasibilityData = MiscHelper.ListToDataTable(mergedExcelData);
                dtFeasibilityData.Columns["Entity_loc"].ColumnName = "Entity Lat,Lng";
                dtFeasibilityData.Columns["lat_lng"].ColumnName = "Customer Lat,Lng";

                dtFeasibilityData = formatColumnNames(dtFeasibilityData);
                //dtFeasibilityData.Columns.
                dtFeasibilityData.TableName = "Feasibility Details";

                foreach (DataColumn col in dtFeasibilityData.Columns)
                {
                    col.ColumnName = MiscHelper.ToCamelCase(col.ColumnName.ToLower());
                }

                ds.Tables.Add(dtFeasibilityData);



                ExportData(ds, fileName);

            }
        }


        public void DownloadMergeKML(string finalKMLString)
        {
            string attachment = "attachment; filename=Merge_Feasibility.kml";
            Response.ClearContent();
            Response.ContentType = "application/xml";
            Response.AddHeader("content-disposition", attachment);
            Response.Write(finalKMLString);
            Response.End();
        }
        public string MergeKML(List<FeasibilityKMLModel> lstFeasibilityKMLModel)
        {
            string finalKMLString = "";
            string folderData = "";
            try
            {


                Dictionary<string, string> KMLHexColors = new Dictionary<string, string>();
                StringBuilder sbFolder = new StringBuilder();
                foreach (FeasibilityKMLModel obj in lstFeasibilityKMLModel)
                {
                    sbFolder = new StringBuilder();

                    StringBuilder sbLine = new StringBuilder();
                    StringBuilder sbPoint = new StringBuilder();

                    sbLine.Append("<Folder>");
                    sbPoint.Append("<Folder>");

                    foreach (var objEntity in obj.lstKMLDataPoints)
                    {
                        if (objEntity.geom_type.ToUpper() == "LINE")
                        {
                            sbLine.Append("<Placemark><description>" + objEntity.description + "</description><name>" + objEntity.entity_title + "</name>");
                            sbLine.Append("<styleUrl>#" + objEntity.entity_type + "</styleUrl><LineString><coordinates>");
                            if (!string.IsNullOrEmpty(objEntity.geom))
                            {
                                sbLine.Append(objEntity.geom);
                            }
                            sbLine.Append("</coordinates></LineString></Placemark>");

                            if (!KMLHexColors.ContainsKey(objEntity.entity_type))
                            {
                                KMLHexColors.Add(objEntity.entity_type, objEntity.colorHex_8);
                            }
                        }
                        if (objEntity.geom_type.ToUpper() == "POINT")
                        {
                            sbPoint.Append("<Placemark><name>" + objEntity.entity_title + "</name>");
                            sbPoint.Append("<description>" + objEntity.description + "</description>");
                            sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                            if (!string.IsNullOrEmpty(objEntity.geom))
                            {
                                sbPoint.Append(objEntity.geom);
                            }
                            sbPoint.Append("</coordinates></Point></Placemark>");
                        }
                    }

                    sbLine.Append("</Folder>");
                    sbPoint.Append("</Folder>");
                    sbFolder.Append("<Folder><name>" + obj.feasibility_name + "</name>" + sbPoint + sbLine + "</Folder>");
                    folderData += sbFolder.ToString();

                }
                string defaultHex = "#FF0000FF";

                finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                            "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                           "<Document>  <!-- Begin Style Definitions -->" +
                            "<Style id =\"inside_A\"><LineStyle><color>" + (KMLHexColors.ContainsKey("inside_A") ? KMLHexColors["inside_A"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                            "<Style id =\"inside\"><LineStyle><color>" + (KMLHexColors.ContainsKey("inside") ? KMLHexColors["inside"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                            "<Style id =\"inside_P\"><LineStyle><color>" + (KMLHexColors.ContainsKey("inside_P") ? KMLHexColors["inside_P"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                            "<Style id =\"outside_start\"><LineStyle><color>" + (KMLHexColors.ContainsKey("outside_start") ? KMLHexColors["outside_start"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                            "<Style id =\"outside_end\"><LineStyle><color>" + (KMLHexColors.ContainsKey("outside_end") ? KMLHexColors["outside_start"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                            "<Style id =\"lmc_start\"><LineStyle><color>" + (KMLHexColors.ContainsKey("lmc_start") ? KMLHexColors["lmc_start"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                            "<Style id =\"lmc_end\"><LineStyle><color>" + (KMLHexColors.ContainsKey("lmc_end") ? KMLHexColors["lmc_end"] : defaultHex) + "</color><width>4</width></LineStyle></Style>" +
                            "<Style id=\"downArrowIcon\"><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href></Icon></IconStyle></Style>" +
                            "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                            folderData + "</Document></kml>";

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return finalKMLString;
        }

        public void DownloadFeasibilityReport(string reportType)
        {
            if (!string.IsNullOrEmpty(reportType))
            {
                if (reportType.Equals("XLS"))
                    DownloadExcelReport();
                else if (reportType.Equals("KML"))
                    DownloadFeasibilityKML();
                else if (reportType.Equals("PDF"))
                    DownloadFeasibilityPDF();
                else if (reportType.Equals("BOM"))
                    DownloadBOM();
            }
        }
        private DataTable GetFeasibilityReportData(List<FeasibilityReportData> reportData)
        {
            DataTable dtReport = new DataTable();
            dtReport = MiscHelper.ListToDataTable(reportData);

            // remove 'insideCables' from DT as it has been moved to a separate DT
            // to be saved in a separate sheet
            dtReport.Columns.Remove("insideCables");
            dtReport.Columns["OUTSIDE_A_LENGTH"].ColumnName = "Outside Length (A) (m)";
            dtReport.Columns["OUTSIDE_B_LENGTH"].ColumnName = "Outside Length (B) (m)";
            dtReport.Columns["LMC_A_LENGTH"].ColumnName = "LMC Length (A) (m)";
            dtReport.Columns["LMC_B_LENGTH"].ColumnName = "LMC Length (B) (m)";
            dtReport.Columns["INSIDE_ASBUILT_LENGTH"].ColumnName = "Inside As-Built Length (m)";
            foreach (DataColumn ReportCol in dtReport.Columns)
            {
                if (ReportCol.ColumnName == "Outside Length (A) (m)"
                    || ReportCol.ColumnName == "Outside Length (B) (m)"
                    || ReportCol.ColumnName == "LMC Length (A) (m)"
                    || ReportCol.ColumnName == "LMC Length (B) (m)"
                    || ReportCol.ColumnName == "Inside As-Built Length (m)")
                    continue;
                ReportCol.ColumnName = MiscHelper.ToCamelCase(ReportCol.ColumnName.ToLower());
            }
            dtReport = formatColumnNames(dtReport);
            // get transpose of Datatable, rows to columsn & vice-a-versa
            DataTable dtTranspose = ReportHelper<DataTable>.TransposeDataTable(dtReport);
            dtTranspose.Columns[0].ColumnName = "Property";
            dtTranspose.Columns[1].ColumnName = "Value";
            dtTranspose.TableName = "Feasibility Report";
            return dtTranspose;
        }

        private void DownloadExcelReport()
        {
            if (Session["XLSData"] != null)
            {
                string fileName = "Feasibiity_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                List<FeasibilityReportData> reportData = (List<FeasibilityReportData>)Session["XLSData"];

                if (reportData != null && reportData.Count > 0)
                {
                    List<FeasibilityInsideCableReport> insideCables = reportData[0].insideCables;
                    insideCables = insideCables.GroupBy(g => g.cable_id).Select(ic =>
                    new FeasibilityInsideCableReport
                    {
                        cable_id = ic.First().cable_id,
                        cable_length = ic.Sum(s => s.cable_length),
                        available_cores = ic.First().available_cores,
                        cable_name = ic.First().cable_name,
                        network_status = ic.First().network_status,
                        total_cores = ic.First().total_cores,
                        used_cores = ic.First().used_cores,
                    }).ToList();

                    DataTable dtInsideCables = new DataTable();
                    dtInsideCables = MiscHelper.ListToDataTable(insideCables);
                    DataTable dtTranspose = GetFeasibilityReportData(reportData);
                    dtInsideCables = formatColumnNames(dtInsideCables);
                    foreach (DataColumn InsideCol in dtInsideCables.Columns)
                    {
                        InsideCol.ColumnName = MiscHelper.ToCamelCase(InsideCol.ColumnName.ToLower());
                    }
                    dtInsideCables.TableName = "Inside Cable Report";
                    DataSet dsReport = new DataSet();
                    dsReport.Tables.Add(dtTranspose);

                    if (dtInsideCables.Rows.Count > 0)
                        dsReport.Tables.Add(dtInsideCables);
                    ExportData(dsReport, fileName);
                }
            }
        }

        private DataTable formatColumnNames(DataTable dtInsideCables)
        {
            for (int i = 0; i < dtInsideCables.Rows.Count; i++)
            {
                for (int j = 0; j < dtInsideCables.Columns.Count; j++)
                {

                    dtInsideCables.Columns[j].ColumnName = dtInsideCables.Columns[j].ColumnName.Replace("_", " ");

                    if (dtInsideCables.Columns[j].ColumnName.ToUpper().EndsWith("LENGTH"))
                    {
                        dtInsideCables.Columns[j].ColumnName += " (m)";
                    }
                    if (dtInsideCables.Columns[j].ColumnName.ToUpper().EndsWith("PRICE") ||
                        dtInsideCables.Columns[j].ColumnName.ToUpper().EndsWith("COST"))
                    {
                        dtInsideCables.Columns[j].ColumnName += " (" + (ApplicationSettings.CustomerCurrency == null ? "" : ApplicationSettings.CustomerCurrency) + ")";
                    }
                }
            }
            return dtInsideCables;
        }

        private void DownloadBOM()
        {
            if (Session["BOMData"] != null)
            {
                string fileName = "BOM_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                List<FeasibilityBOMData> reportData = (List<FeasibilityBOMData>)Session["BOMData"];

                fillBOMPrices(ref reportData);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable(reportData);

                dtReport.Columns.Remove("cable_type_id");
                dtReport.Columns.Remove("isPastData");
                foreach (DataColumn col in dtReport.Columns)
                {
                    col.ColumnName = MiscHelper.ToCamelCase(col.ColumnName.ToLower());
                }
                dtReport = formatColumnNames(dtReport);
                DataSet dsReport = new DataSet();
                dsReport.Tables.Add(dtReport);
                ExportData(dsReport, fileName);
            }
        }

        private void fillBOMPrices(ref List<FeasibilityBOMData> reportData)
        {
            foreach (FeasibilityBOMData bomData in reportData)
            {
                if (!bomData.isPastData)
                {
                    FeasibilityCableType cblType = new BLFeasibilityCableType().getCableTypeByID(bomData.cable_type_id);
                    if (cblType != null)
                    {
                        bomData.material_unit_price = Math.Round(cblType.material_price_per_unit);
                        bomData.service_unit_price = Math.Round(cblType.service_price_per_unit);
                        bomData.total_material_cost = Math.Round(cblType.material_price_per_unit * bomData.cable_length, 2);
                        bomData.total_service_cost = Math.Round(cblType.service_price_per_unit * bomData.cable_length, 2);
                        bomData.cable_length = Math.Round(bomData.cable_length, 2);
                    }
                }
            }
        }

        public JsonResult SetFeasibiltyData(List<FeasibilityReportData> reportData, List<FeasibilityKMLData> kmlData, List<FeasibilityBOMData> bomData)
        {
            try
            {
                if (reportData == null && kmlData == null && bomData == null)
                {
                    return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
                }

                Session["XLSData"] = reportData;
                Session["KMLData"] = kmlData;
                Session["BOMData"] = bomData;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = "fail", message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region(FTTH FEASIBILITY)

        public void ExportMergedKMLFTTH(string search_by, string searchText, string FromDate, string ToDate)
        {

            FromDate = FromDate == "" ? null : Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd");
            ToDate = ToDate == "" ? null : Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");

            CommonGridAttributes objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchBy = search_by;
            objGridAttributes.searchText = searchText;
            objGridAttributes.sort = "feasibility_id";
            objGridAttributes.orderBy = "desc";
            var listHistoryRecord = new BLFeasibilityHistory().getPastFeasibilyExportDataFtth<Dictionary<string, string>>(objGridAttributes, FromDate, ToDate);
            if (listHistoryRecord != null && listHistoryRecord.Count > 0)
            {
                string[] feasKMLData = new string[listHistoryRecord.Count];

                for (int i = 0; i < listHistoryRecord.Count; i++)
                {
                    FeasibilityKMLFTTHModel rec = new FeasibilityKMLFTTHModel();
                    rec.feasibility_name = listHistoryRecord[i]["feasibility_name"];
                    rec.history_id = Convert.ToInt32(listHistoryRecord[i]["history_id"]);
                    rec.lat_lng = listHistoryRecord[i]["lat_lng"];
                    rec.entity_id = listHistoryRecord[i]["entity_id"];
                    rec.feasibility_id = Convert.ToInt32(listHistoryRecord[i]["feasibility_id"]);
                    rec.customer_id = listHistoryRecord[i]["customer_id"];
                    rec.customer_name = listHistoryRecord[i]["customer_name"];
                    rec.entity_loc = listHistoryRecord[i]["entity_loc"];
                    rec.path_distance = listHistoryRecord[i]["path_distance"];
                    feasKMLData[i] = JsonConvert.SerializeObject(rec);
                }

                DownloadKMZReportFTTH(JsonConvert.SerializeObject(feasKMLData));
            }

        }

        public JsonResult SetFeasibiltyDataFTTH(List<FTTHFeasibilityReportData> reportData, List<FeasibilityKMLDataFTTH> kmlData, List<FeasibilityBOMData> bomData)
        {
            try
            {
                if (reportData == null && kmlData == null && bomData == null)
                {
                    return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
                }

                Session["XLSDataFTTH"] = reportData;
                Session["KMLDataFTTH"] = kmlData;
                Session["BOMDataFTTH"] = bomData;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = "fail", message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void DownloadFeasibilityReportFTTH(string reportType)
        {
            if (!string.IsNullOrEmpty(reportType))
            {
                if (reportType.Equals("XLS"))
                    DownloadExcelReportFTTH();
                else if (reportType.Equals("KML"))
                    DownloadFeasibilityKMLFTTH();
                else if (reportType.Equals("PDF"))
                    DownloadFeasibilityPDFFTTH();
                else if (reportType.Equals("BOM"))
                    DownloadBOM();
            }
        }

        private DataTable GetFeasibilityReportDataFTTH(List<FTTHFeasibilityReportData> reportData)
        {
            DataTable dtReport = new DataTable();
            dtReport = MiscHelper.ListToDataTable(reportData);

            dtReport.Columns["feasibility_id"].ColumnName = "FEASIBILITY ID";
            dtReport.Columns["feasibility_name"].ColumnName = "FEASIBILITY NAME";
            dtReport.Columns["customer_id"].ColumnName = "CUSTOMER ID";
            dtReport.Columns["customer_name"].ColumnName = "CUSTOMER NAME";
            dtReport.Columns["entity_id"].ColumnName = "ENTITY ID";
            dtReport.Columns["lat_lng"].ColumnName = "Customer Lat,Lng";
            dtReport.Columns["path_distance"].ColumnName = "PATH DISTANCE(Km)";
            dtReport.Columns["buffer_radius"].ColumnName = "BUFFER RADIUS(Mtr)";


            dtReport = formatColumnNames(dtReport);
            // get transpose of Datatable, rows to columsn & vice-a-versa
            DataTable dtTranspose = ReportHelper<DataTable>.TransposeDataTable(dtReport);
            dtTranspose.Columns[0].ColumnName = "Property";
            dtTranspose.Columns[1].ColumnName = "Value";
            dtTranspose.TableName = "FTTH Feasibility Report";
            return dtTranspose;

        }
        private void DownloadExcelReportFTTH()
        {
            if (Session["XLSDataFTTH"] != null)
            {
                string fileName = "FTTH Feasibiity_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
                List<FTTHFeasibilityReportData> reportData = (List<FTTHFeasibilityReportData>)Session["XLSDataFTTH"];
                DataTable dtTranspose = GetFeasibilityReportDataFTTH(reportData);
                DataSet dsReport = new DataSet();
                dsReport.Tables.Add(dtTranspose);
                ExportData(dsReport, fileName);
            }
        }

        private void DownloadFeasibilityPDFFTTH()
        {
            if (Session["XLSDataFTTH"] != null)
            {
                try
                {
                    List<FTTHFeasibilityReportData> lstReportData = (List<FTTHFeasibilityReportData>)Session["XLSDataFTTH"];
                    if (lstReportData.Count > 0)
                    {
                        DataTable dtReport = GetFeasibilityReportDataFTTH(lstReportData);

                        Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 60f, 40f);
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                        pdfWriter.PageEvent = new ReportHelper<FeasibilityReportData>.AllPdfPageEvents();
                        pdfDoc.Open();
                        PdfPTable tableLayout = new PdfPTable(2);
                        pdfDoc.Add(ReportHelper<FeasibilityReportData>.Add_Phrase_To_PDF(new PdfPTable(1), "Feasibility Details"));
                        pdfDoc.Add(new Paragraph(" "));
                        pdfDoc.Add(ReportHelper<FeasibilityReportData>.Add_Export_Content_To_PDF(tableLayout, dtReport));




                        GeneratePDFFTTH(pdfDoc, pdfWriter, tableLayout);
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void GeneratePDFFTTH(Document docPDF, PdfWriter pdfWriter, PdfPTable pdfTable)
        {
            string fileName = "FTTH_Feasibiity_Report_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
            pdfWriter.CloseStream = false;
            docPDF.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(docPDF);
            Response.End();
        }

        public void DownloadKMZReportFTTH(string feasibility_saved)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            List<FeasibilityKMLFTTHModel> lstFeasibilityKMLModel = new List<FeasibilityKMLFTTHModel>();
            string[] data = jsonSerializer.Deserialize<string[]>(feasibility_saved);


            foreach (string feasData in data)
            {
                FeasibilityKMLFTTHModel feasKMLData = jsonSerializer.Deserialize<FeasibilityKMLFTTHModel>(feasData);

                feasKMLData.cable_geometry = new BLFeasibilityHistory().getFeasibilityDetailsFTTH(Convert.ToInt32(feasKMLData.history_id));
                var ll1 = feasKMLData.lat_lng.Trim().Split(',');
                var ll2 = feasKMLData.entity_loc.Trim().Split(',');


                var geom = "";
                var geomArray = feasKMLData.cable_geometry.Replace("LINESTRING(", "").Replace(")", "").Split(',');
                foreach (var g in geomArray)
                {
                    var latLng = g.Trim().Split(' ');
                    geom += latLng[1] + ',' + latLng[0] + ",0 ";
                }
                FeasibilityKMLDataFTTH objFeasibilityKMLData = new FeasibilityKMLDataFTTH();

                string Description = "<![CDATA[<p><b>Distance:</b> " + feasKMLData.path_distance + "<br/>";
                Description += ("<b>Entity ID: </b>" + feasKMLData.entity_id) + "<br/>";
                Description += ("<b>Feasibility Name: </b>" + feasKMLData.feasibility_name);
                Description += "</p>]]>";
                if (geom.Length > 0)
                {

                    objFeasibilityKMLData.geometry = geom;
                    objFeasibilityKMLData.description = Description;
                    objFeasibilityKMLData.geom_type = "LINE";
                    objFeasibilityKMLData.colorHex_8 = "#FF0000FF";
                }
                feasKMLData.lstKMLDataPoints.Add(objFeasibilityKMLData);
                if (ll1.Length > 0)
                {
                    FeasibilityKMLDataFTTH FeasibilityKMLData = new FeasibilityKMLDataFTTH();

                    FeasibilityKMLData.geometry = ll1[1] + "," + ll1[0] + ",0 ";
                    FeasibilityKMLData.description = "<![CDATA[<h2>(" + ll1[1] + "," + ll1[0] + ")</h2>]]>";
                    FeasibilityKMLData.geom_type = "POINT";
                    // objFeasibilityKMLData.entity_name = "Customer Location";
                    FeasibilityKMLData.feasibility_title = "Customer Location";
                    feasKMLData.lstKMLDataPoints.Add(FeasibilityKMLData);
                }
                if (ll2.Length > 0)
                {
                    FeasibilityKMLDataFTTH objFeasibilityKMLDataFTTH = new FeasibilityKMLDataFTTH();
                    objFeasibilityKMLDataFTTH.geometry = ll2[1] + "," + ll2[0] + ",0 ";
                    objFeasibilityKMLDataFTTH.description = "<![CDATA[<h2>(" + ll2[1] + "," + ll2[0] + ")</h2>]]>";
                    objFeasibilityKMLDataFTTH.geom_type = "POINT";
                    //objFeasibilityKMLData.entity_name = "End Point";
                    objFeasibilityKMLDataFTTH.feasibility_title = "Entity Location";
                    //objFeasibilityKMLData.entity_title = (feasKMLData.end_Point_Name != null && feasKMLData.end_Point_Name.Length > 0) ? new XText(feasKMLData.end_Point_Name).ToString() : "End Point";
                    feasKMLData.lstKMLDataPoints.Add(objFeasibilityKMLDataFTTH);
                }
                lstFeasibilityKMLModel.Add(feasKMLData);
                //if (ll2.Length > 0)
                //{
                //    FeasibilityKMLDataFTTH FeasibilityKMLData = new FeasibilityKMLDataFTTH();
                //    FeasibilityKMLData.geometry = ll2[1] + "," + ll2[0] + ",0 ";
                //    FeasibilityKMLData.description = "<![CDATA[<h2>(" + ll2[1] + "," + ll2[0] + ")</h2>]]>";
                //    FeasibilityKMLData.geom_type = "POINT";
                //    //objFeasibilityKMLData.entity_name = "End Point";
                //    FeasibilityKMLData.feasibility_title =  "Entity Location";
                //    feasKMLData.lstKMLDataPoints.Add(FeasibilityKMLData);
                //}


            }
            string finalKMLString = MergeKMLFTTH(lstFeasibilityKMLModel);
            DownloadMergeKML(finalKMLString);
        }

        public string MergeKMLFTTH(List<FeasibilityKMLFTTHModel> lstFeasibilityKMLModel)
        {
            string finalKMLString = "";
            string folderData = "";
            try
            {


                // Dictionary<string, string> KMLHexColors = new Dictionary<string, string>();
                StringBuilder sbFolder = new StringBuilder();
                foreach (FeasibilityKMLFTTHModel objEntity in lstFeasibilityKMLModel)
                {
                    sbFolder = new StringBuilder();

                    StringBuilder sbLine = new StringBuilder();
                    StringBuilder sbPoint = new StringBuilder();

                    sbLine.Append("<Folder>");
                    sbPoint.Append("<Folder>");

                    foreach (var obj in objEntity.lstKMLDataPoints)
                    {
                        if (obj.geom_type.ToUpper() == "LINE")
                        {
                            sbLine.Append("<Placemark><description>" + obj.description + "</description><name>Path</name>");
                            sbLine.Append("<styleUrl>#feasibility_id</styleUrl><LineString><coordinates>");
                            if (!string.IsNullOrEmpty(obj.geometry))
                            {
                                sbLine.Append(obj.geometry);
                            }
                            sbLine.Append("</coordinates></LineString></Placemark>");

                            //if (!KMLHexColors.ContainsKey(objEntity.entity_type))
                            //{
                            //    KMLHexColors.Add(objEntity.entity_type, objEntity.colorHex_8);
                            //}
                        }
                        if (obj.geom_type.ToUpper() == "POINT")
                        {
                            sbPoint.Append("<Placemark><name>" + obj.feasibility_title + "</name>");
                            sbPoint.Append("<description>" + obj.description + "</description>");
                            sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                            if (!string.IsNullOrEmpty(obj.geometry))
                            {
                                sbPoint.Append(obj.geometry);
                            }
                            sbPoint.Append("</coordinates></Point></Placemark>");
                        }
                    }


                    sbLine.Append("</Folder>");
                    sbPoint.Append("</Folder>");
                    sbFolder.Append("<Folder><name>" + objEntity.feasibility_name + "</name>" + sbPoint + sbLine + "</Folder>");
                    folderData += sbFolder.ToString();

                }
                string defaultHex = "#FF0000FF";

                finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                            "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                           "<Document>  <!-- Begin Style Definitions -->" +
                            "<Style id =\"feasibility_id\"><LineStyle><color>" + defaultHex + "</color><width>4</width></LineStyle></Style>" +
                            "<Style id=\"downArrowIcon\"><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href></Icon></IconStyle></Style>" +
                            "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                            folderData + "</Document></kml>";

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return finalKMLString;
        }

        private void DownloadFeasibilityKMLFTTH()
        {
            if (Session["KMLDataFTTH"] != null)
            {
                StringBuilder sbLine = new StringBuilder();
                StringBuilder sbPoint = new StringBuilder();
                Dictionary<string, string> KMLHexColors = new Dictionary<string, string>();
                sbLine.Append("<Folder>");
                sbPoint.Append("<Folder>");
                try
                {
                    List<FeasibilityKMLDataFTTH> FTTHFeasibilityKMLData = (List<FeasibilityKMLDataFTTH>)Session["KMLDataFTTH"];



                    foreach (var objEntity in FTTHFeasibilityKMLData)
                    {
                        if (objEntity.geom_type.ToUpper() == "LINE")
                        {
                            sbLine.Append("<Placemark><description>" + objEntity.description + "</description><name>Path</name>");
                            sbLine.Append("<styleUrl>#feasibility_id</styleUrl><LineString><coordinates>");
                            if (!string.IsNullOrEmpty(objEntity.geometry))
                            {
                                sbLine.Append(objEntity.geometry);
                            }
                            sbLine.Append("</coordinates></LineString></Placemark>");

                            //if (!KMLHexColors.ContainsKey(objEntity.entity_type))
                            //{
                            //    KMLHexColors.Add(objEntity.entity_type, objEntity.colorHex_8);
                            //}
                        }
                        if (objEntity.geom_type.ToUpper() == "POINT")
                        {
                            sbPoint.Append("<Placemark><name>" + new XText(objEntity.feasibility_title) + "</name>");
                            sbPoint.Append("<description>" + objEntity.description + "</description>");
                            sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                            if (!string.IsNullOrEmpty(objEntity.geometry))
                            {
                                sbPoint.Append(objEntity.geometry);
                            }
                            sbPoint.Append("</coordinates></Point></Placemark>");
                        }
                    }

                    sbLine.Append("</Folder>");
                    sbPoint.Append("</Folder>");
                    string defaultHex = "#FF0000FF";

                    string finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                               "<Document>  <!-- Begin Style Definitions -->" +
                                "<Style id =\"feasibility_id\"><LineStyle><color>" + "#FF0000FF" + "</color><width>4</width></LineStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href></Icon></IconStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                                sbPoint.ToString() + sbLine.ToString() + "</Document></kml>";


                    string attachment = "attachment; filename=export_Feasibility.kml";
                    Response.ClearContent();
                    Response.ContentType = "application/xml";
                    Response.AddHeader("content-disposition", attachment);
                    Response.Write(finalKMLString);
                    Response.End();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

    }
}