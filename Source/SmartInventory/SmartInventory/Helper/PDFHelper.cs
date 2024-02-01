using BusinessLogics;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Models;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Utility;
using Utility.MapPrinter;
namespace SmartInventory.Helper
{
    public class PDFHelper
    {
        public static void ExportBOMBOQReportInPDF(List<BOMReport> lstBOMReport, string rptType)
        {

            Document pdfDoc = new Document(PageSize.A4.Rotate(), 0, 0, 40, 50);
            try
            {
                Font font = new Font(GetFont(), 12, Font.NORMAL);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, System.Web.HttpContext.Current.Response.OutputStream);
                writer.PageEvent = new AllPdfPageEvents();
                pdfDoc.Open();
                //ADD REPORT HEADING...
                pdfDoc.Add(new Paragraph(new Chunk(rptType.ToUpper(), font))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                });
                if (lstBOMReport != null && lstBOMReport.Count > 0)
                {
                    #region DEFINE TABLE AND COLUMNS
                    PdfPTable tblPDF = null;
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF = new PdfPTable(19);
                        tblPDF.SetTotalWidth(new float[] { 10f, 17f, 4f, 4f, 4f, 10f, 6f, 7f, 7f, 4f, 10f, 6f, 7f, 7f, 4f, 10f, 6f, 7f, 7f });
                    }
                    else
                    {
                        tblPDF = new PdfPTable(12);
                        tblPDF.SetTotalWidth(new float[] { 11f, 25f, 6f, 8f, 8f, 6f, 8f, 8f,8f, 6f, 8f, 8f });
                    }
                    #endregion

                    #region ADD HEADER COLUMNS
                    // ADD HEADER COLUMNS...
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_144, _rowSpan: 2, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_067, _rowSpan: 2, _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_143, ApplicationSettings.Currency), _rowSpan: 2, _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_142, ApplicationSettings.Currency), _rowSpan: 2, _isHeaderFooter: true));
                    }
                    //add network status column..
                    tblPDF.AddCell(GetCusotmPDFCell("Planned", _colspan: rptType.ToUpper() == "BOQ" ? 5 : 4, _isHeaderFooter: true, alignType: "Center"));
                    tblPDF.AddCell(GetCusotmPDFCell("As-Built", _colspan: rptType.ToUpper() == "BOQ" ? 5 : 4, _isHeaderFooter: true, alignType: "Center"));
                    tblPDF.AddCell(GetCusotmPDFCell("Dormant", _colspan: rptType.ToUpper() == "BOQ" ? 5 : 4, _isHeaderFooter: true, alignType: "Center"));

                    //Planned columns
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_403, _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), _isHeaderFooter: true));
                    }
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_023, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_024, _isHeaderFooter: true));

                    //As-Built columns
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_403, _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), _isHeaderFooter: true));
                    }
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_023, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_024, _isHeaderFooter: true));

                    //Dormant columns
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_403, _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), _isHeaderFooter: true));
                    }
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_023, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_024, _isHeaderFooter: true));
                    #endregion

                    #region ADD ROW DATA
                    foreach (BOMReport objBom in lstBOMReport)
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.is_header ? objBom.entity_type : objBom.entity_sub_type, _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.specification, _isSubheader: objBom.is_header));
                        //priyanka
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.served_by_ring, _isSubheader: objBom.is_header));
                        if (rptType.ToUpper() == "BOQ")
                        {
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.is_header ? "" : objBom.cost_per_unit.ToString(), _isSubheader: objBom.is_header));
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.is_header ? "" : objBom.service_cost_per_unit.ToString(), _isSubheader: objBom.is_header));

                        }
                        //planned Columns
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.total_planned_count.ToString(), _isSubheader: objBom.is_header));
                        if (rptType.ToUpper() == "BOQ")
                        {
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.total_planned_cost.ToString(), _isSubheader: objBom.is_header));
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.total_planned_service_cost.ToString(), _isSubheader: objBom.is_header));
                        }
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.geom_type.ToUpper() == "LINE" ? objBom.total_planned_calc_length.ToString() : "NA", _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.geom_type.ToUpper() == "LINE" ? objBom.total_planned_gis_length.ToString() : "NA", _isSubheader: objBom.is_header));
                        //As-Built Columns
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.total_asbuilt_count.ToString(), _isSubheader: objBom.is_header));
                        if (rptType.ToUpper() == "BOQ")
                        {
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.total_asbuilt_cost.ToString(), _isSubheader: objBom.is_header));
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.total_asbuilt_service_cost.ToString(), _isSubheader: objBom.is_header));
                        }
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.geom_type.ToUpper() == "LINE" ? objBom.total_asbuilt_calc_length.ToString() : "NA", _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.geom_type.ToUpper() == "LINE" ? objBom.total_asbuilt_gis_length.ToString() : "NA", _isSubheader: objBom.is_header));


                        //Dormant Columns
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.total_dormant_count.ToString(), _isSubheader: objBom.is_header));
                        if (rptType.ToUpper() == "BOQ")
                        {
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.total_dormant_cost.ToString(), _isSubheader: objBom.is_header));
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.total_dormant_service_cost.ToString(), _isSubheader: objBom.is_header));
                        }
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.geom_type.ToUpper() == "LINE" ? objBom.total_dormant_calc_length.ToString() : "NA", _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.geom_type.ToUpper() == "LINE" ? objBom.total_dormant_gis_length.ToString() : "NA", _isSubheader: objBom.is_header));
                    }
                    #endregion

                    #region ADD FOOTER COLUMN

                    var query = lstBOMReport.Where(p => p.is_header == true);


                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_041, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));

                    }
                    //planned Columns
                    tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_planned_count).ToString(), _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_planned_cost).ToString(), _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.service_cost_per_unit).ToString(), _isHeaderFooter: true));
                    }
                    tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_planned_calc_length).ToString(), _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_planned_gis_length).ToString(), _isHeaderFooter: true));
                    //As-Built Columns
                    tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_asbuilt_count).ToString(), _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_asbuilt_cost).ToString(), _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_asbuilt_service_cost).ToString(), _isHeaderFooter: true));
                    }
                    tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_asbuilt_calc_length).ToString(), _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_asbuilt_gis_length).ToString(), _isHeaderFooter: true));


                    //Dormant Columns
                    tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_dormant_count).ToString(), _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_dormant_cost).ToString(), _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_dormant_service_cost).ToString(), _isHeaderFooter: true));
                    }
                    tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_dormant_calc_length).ToString(), _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(query.Sum(m => m.total_dormant_gis_length).ToString(), _isHeaderFooter: true));
                    #endregion
                    pdfDoc.Add(tblPDF);
                }
                pdfDoc.Close();
                string filename = rptType + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss") + ".pdf";
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                System.Web.HttpContext.Current.Response.Write(pdfDoc);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write(ex.Message);
            }
        }
        public static void _ExportBOMBOQReportInPDF(BomBoqExportReport BOMReport, string rptType)
        {
            List<BOMBOQReport> lstBOMReport = BOMReport.BomBoqReportList;
            var isAttr = ((List<string>)HttpContext.Current.Session["ApplicableModuleList"]);
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 0, 0, 40, 50);
            try
            {
                Font font = new Font(GetFont(), 12, Font.NORMAL);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, System.Web.HttpContext.Current.Response.OutputStream);
                writer.PageEvent = new AllPdfPageEvents();
                pdfDoc.Open();
                int CblCnt = 0;
                //ADD REPORT HEADING...
                string RptName = rptType.ToUpper() == "BOM" ? Resources.Resources.SI_OSP_GBL_GBL_FRM_211 : rptType.ToUpper() == "BOQ" ? Resources.Resources.SI_OSP_GBL_GBL_FRM_210 : "";
                pdfDoc.Add(new Paragraph(new Chunk(RptName, font))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 21
                });
                if (lstBOMReport != null && lstBOMReport.Count > 0)
                {
                    #region DEFINE TABLE AND COLUMNS
                    PdfPTable tblPDF = null;
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF = new PdfPTable(12);
                        tblPDF.SetTotalWidth(new float[] { 10f, 12f, 12f, 23f, 9f, 12f,12f, 5f, 7.5f, 10f, 8f, 7.5f });
                    }
                    else
                    {
                        //priyanka
                        tblPDF = new PdfPTable(8);
                        tblPDF.SetTotalWidth(new float[] { 11f, 10f, 10f, 25f,25f, 6f, 8f, 8f });
                    }
                    // tblPDF.SetTotalWidth(new float[] { 11f, 25f, 6f, 8f, 8f, 6f, 8f, 8f, 6f, 8f, 8f });
                    #endregion

                    #region Title

                    if (!string.IsNullOrWhiteSpace(BOMReport.objAdAttribute.title))
                    {
                        PdfPTable tblPDFTitle = new PdfPTable(1);
                        tblPDFTitle.SetTotalWidth(new float[] { 150f });
                        tblPDFTitle.SpacingAfter = 21f;
                        tblPDFTitle.AddCell(GetCusotmPDFCell(BOMReport.objAdAttribute.title, alignType: "Center", _fontSize: 16, IsBorder: false));
                        pdfDoc.Add(tblPDFTitle);
                    }

                    #endregion

                    PdfPTable tblPDFComms = new PdfPTable(1);
                    tblPDFComms.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_275, alignType: "Center", _fontSize: 16, _colspan: (rptType.ToUpper() == "BOQ" ? 12 : 8), _isCustomBackgroundColor: true, clrs: System.Drawing.ColorTranslator.FromHtml("#296d98")));


                    #region Filer Details

                    object obj = BOMReport.objReportFilters;
                    var userdetails = (User)HttpContext.Current.Session["userDetail"];
                    CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;
                    PdfPTable tblPDFFilters = new PdfPTable(2);
                    tblPDFFilters.SetTotalWidth(new float[] { 30f, 80f });
                    //tblPDFFilters.SpacingAfter = 20f;

                    //tblPDFFilters.AddCell(GetCusotmPDFCell("Filter Details", alignType: "Left", _fontSize: 16, _colspan: (rptType.ToUpper() == "BOQ" ? 11 : 7), _isCustomBackgroundColor: true, clrs: System.Drawing.ColorTranslator.FromHtml("#296d98")));
                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_GBL_GBL_NET_FRM_166, alignType: "Left", _fontSize: 13, _isHeaderFooter: true));
                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_GBL_GBL_NET_FRM_167, alignType: "Left", _fontSize: 13, _isHeaderFooter: true));

                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_065, alignType: "Left", _fontSize: 13));
                    List<int> regionIds = (List<int>)obj.GetType().GetProperty("SelectedRegionId").GetValue(obj, null);
                    var regionName = regionIds == null ? "All" : string.Join(",", new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(userdetails.user_id) }).Where(x => regionIds.Contains(x.regionId)).Select(x => x.regionName).ToList());
                    tblPDFFilters.AddCell(GetCusotmPDFCell(regionName, alignType: "Left", _fontSize: 13));

                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_066, alignType: "Left", _fontSize: 13));
                    List<int> provinceIds = (List<int>)obj.GetType().GetProperty("SelectedProvinceId").GetValue(obj, null);
                    var provinceName = provinceIds == null ? "All" : string.Join(",", new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = string.Join(",", regionIds), userId = Convert.ToInt32(userdetails.user_id) }).Where(x => provinceIds.Contains(x.provinceId)).Select(x => x.provinceName).ToList());
                    tblPDFFilters.AddCell(GetCusotmPDFCell(provinceName, alignType: "Left", _fontSize: 13));

                    // string networkStatus = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedNetworkStatues").GetValue(obj, null).ToString().Replace("AS BUILT", "AS-BUILT").ToLower()).Replace("'", "");

                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_063, alignType: "Left", _fontSize: 13));
                    List<string> NWStatus = (List<string>)obj.GetType().GetProperty("SelectedNetworkStatus").GetValue(obj, null);
                    string networkStatus = NWStatus == null ? "All" : (string.Join(", ", NWStatus.Select(i => (i.ToUpper() == "P" ? "Planned" : i.ToUpper() == "A" ? "As-Built" : i.ToUpper() == "D" ? "Dorment" : ""))));
                    tblPDFFilters.AddCell(GetCusotmPDFCell(networkStatus, alignType: "Left", _fontSize: 13));

                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_274, alignType: "Left", _fontSize: 13));
                    List<int> parentUser = new List<int>();
                    List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
                    var parentUserName = string.Empty;
                    if (userdetails.role_id == 1)
                    {
                        parentUser.Add(1);
                        parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());
                    }
                    else
                    {
                        parentUserName = parentUserIds == null ? "All" : new BLUser().GetUserDetailByID(userdetails.user_id).user_name;
                    }
                    tblPDFFilters.AddCell(GetCusotmPDFCell(parentUserName, alignType: "Left", _fontSize: 13));

                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_040, alignType: "Left", _fontSize: 13));
                    List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
                    var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());
                    tblPDFFilters.AddCell(GetCusotmPDFCell(userName, alignType: "Left", _fontSize: 13));



                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_GBL_GBL_NET_FRM_098, alignType: "Left", _fontSize: 13));
                    List<int> layerIds = (List<int>)obj.GetType().GetProperty("SelectedLayerId").GetValue(obj, null);
                    var layerName = layerIds == null ? "All" : string.Join(",", new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY").Where(x => layerIds.Contains(x.layer_id)).Select(x => x.layer_title).ToList());
                    tblPDFFilters.AddCell(GetCusotmPDFCell(layerName, alignType: "Left", _fontSize: 13));

                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_GBL_GBL_GBL_GBL_147, alignType: "Left", _fontSize: 13));
                    string ownershipType = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedOwnerShipType").GetValue(obj, null).ToString()).Replace("'", "");
                    ownershipType = string.IsNullOrEmpty(ownershipType) ? "All" : ownershipType;
                    tblPDFFilters.AddCell(GetCusotmPDFCell(ownershipType, alignType: "Left", _fontSize: 13));

                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_FRM_030, alignType: "Left", _fontSize: 13));
                    List<int> thirdPartyVendorId = (List<int>)obj.GetType().GetProperty("SelectedThirdPartyVendorId").GetValue(obj, null);
                    var thirdPartyVendorName = thirdPartyVendorId == null ? "All" : string.Join(",", BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList().Where(x => thirdPartyVendorId.Contains(Convert.ToInt32(x.key))).Select(x => x.value).ToList());
                    tblPDFFilters.AddCell(GetCusotmPDFCell(thirdPartyVendorName, alignType: "Left", _fontSize: 13));

                    //List<int> parentUser = new List<int>();
                    //parentUser.Add(1);
                    //List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
                    //var parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

                    //List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
                    //var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

                    if (isAttr.Contains("PROJ"))
                    {
                        tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_074, alignType: "Left", _fontSize: 13));
                        List<int> projectCodeIds = (List<int>)obj.GetType().GetProperty("SelectedProjectId").GetValue(obj, null);
                        var projectCodeName = projectCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrEmpty(networkStatus) ? "P" : networkStatus.ToUpper() == "PLANNED" ? "P" : networkStatus.ToUpper() == "AS BUILT" ? "A" : networkStatus.ToUpper() == "DORMANT" ? "D" : "P").Where(x => projectCodeIds.Contains(x.system_id)).Select(x => x.project_code).ToList());
                        tblPDFFilters.AddCell(GetCusotmPDFCell(projectCodeName, alignType: "Left", _fontSize: 13));

                        tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_076, alignType: "Left", _fontSize: 13));
                        List<int> planningCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPlanningId").GetValue(obj, null);
                        var planningCodeName = planningCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(projectCodeIds).Where(x => planningCodeIds.Contains(x.system_id)).Select(x => x.planning_code).ToList());
                        tblPDFFilters.AddCell(GetCusotmPDFCell(planningCodeName, alignType: "Left", _fontSize: 13));

                        tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_009, alignType: "Left", _fontSize: 13));
                        List<int> workOrderCodeIds = (List<int>)obj.GetType().GetProperty("SelectedWorkOrderId").GetValue(obj, null);
                        var workOrderCodeName = workOrderCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(planningCodeIds).Where(x => workOrderCodeIds.Contains(x.system_id)).Select(x => x.workorder_code).ToList());
                        tblPDFFilters.AddCell(GetCusotmPDFCell(workOrderCodeName, alignType: "Left", _fontSize: 13));

                        tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_GBL_011, alignType: "Left", _fontSize: 13));
                        List<int> purposeCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPurposeId").GetValue(obj, null);
                        var purposeCodeName = purposeCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(workOrderCodeIds).Where(x => purposeCodeIds.Contains(x.system_id)).Select(x => x.purpose_code).ToList());
                        tblPDFFilters.AddCell(GetCusotmPDFCell(purposeCodeName, alignType: "Left", _fontSize: 13));
                    }
                        tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_071, alignType: "Left", _fontSize: 13));
                    string durationBasedOn = obj.GetType().GetProperty("durationbasedon").GetValue(obj, null).ToString().Replace("_", " ");
                    tblPDFFilters.AddCell(GetCusotmPDFCell(durationBasedOn, alignType: "Left", _fontSize: 13));

                    tblPDFFilters.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_072, alignType: "Left", _fontSize: 13));
                    string duration = "";
                    if (obj.GetType().GetProperty("fromDate").GetValue(obj, null) != null && obj.GetType().GetProperty("toDate").GetValue(obj, null) !=null)
                    {
                        duration = obj.GetType().GetProperty("fromDate").GetValue(obj, null).ToString() + " To " + obj.GetType().GetProperty("toDate").GetValue(obj, null).ToString();
                    }
                    else
                    {
                        duration = "All";
                    }
                    tblPDFFilters.AddCell(GetCusotmPDFCell(duration, alignType: "Left", _fontSize: 13));
                    tblPDFComms.AddCell(tblPDFFilters);
                    //pdfDoc.Add(tblPDFFilters);


                    #endregion

                    #region Equipment Details
                    if (isAttr.Contains("EQPD"))
                    {
                        if (BOMReport.objAdAttribute.isEquipmentEnable == true)
                        {
                            PdfPTable tblPDFTitle = new PdfPTable(2);
                            tblPDFTitle.SetTotalWidth(new float[] { 30f, 80f });
                            //tblPDFTitle.SpacingAfter = 50f;
                            tblPDFTitle.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_272, alignType: "Left", _fontSize: 16, _colspan: 2, _isCustomBackgroundColor: true, clrs: System.Drawing.ColorTranslator.FromHtml("#296d98")));
                            tblPDFTitle.AddCell(GetCusotmPDFCell(Resources.Resources.SI_ISP_GBL_NET_FRM_063, alignType: "Left", _fontSize: 13));
                            tblPDFTitle.AddCell(GetCusotmPDFCell(BOMReport.objAdAttribute.equipmenttype, alignType: "Left", _fontSize: 13));
                            tblPDFTitle.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_118, alignType: "Left", _fontSize: 13));
                            tblPDFTitle.AddCell(GetCusotmPDFCell(BOMReport.objAdAttribute.equipmentname, alignType: "Left", _fontSize: 13));
                            tblPDFTitle.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_GBL_139, alignType: "Left", _fontSize: 13));
                            tblPDFTitle.AddCell(GetCusotmPDFCell(BOMReport.objAdAttribute.popname, alignType: "Left", _fontSize: 13));
                            tblPDFComms.AddCell(tblPDFTitle);
                            //pdfDoc.Add(tblPDFTitle);
                        }
                    }


                    #endregion
                    tblPDFComms.SpacingAfter = (isAttr.Contains("EQPD") == true && BOMReport.objAdAttribute.isEquipmentEnable == true ? 50f : 140f);
                    pdfDoc.Add(tblPDFComms);
                    pdfDoc.NewPage();
                    #region ADD HEADER COLUMNS
                    // ADD HEADER COLUMNS...
                    tblPDF.AddCell(GetCusotmPDFCell(rptType + Resources.Resources.SI_OSP_GBL_NET_FRM_218, alignType: "Center", _fontSize: 16, _colspan: (rptType.ToUpper() == "BOQ" ? 12 : 8), _isCustomBackgroundColor: true, clrs: System.Drawing.ColorTranslator.FromHtml("#296d98")));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_GBL_008, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_077, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_068, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_015, _isHeaderFooter: true));
                    //priyanka
                    tblPDF.AddCell(GetCusotmPDFCell("Served By Ring", _isHeaderFooter: true));

                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(String.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_014.Replace("<br>", ""), ApplicationSettings.Currency), _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_017 + " " + string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_016, ApplicationSettings.Currency), _isHeaderFooter: true));
                    }
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_GBL_226, _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), _isHeaderFooter: true));
                    }
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_476.Replace("<br/>", ""), _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_477, _isHeaderFooter: true));
                    

                    #region extra
                    //if (rptType.ToUpper() == "BOQ")
                    //{
                    //    tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_143, ApplicationSettings.Currency), _isHeaderFooter: true));
                    //    tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_142, ApplicationSettings.Currency), _isHeaderFooter: true));
                    //}
                    ////add network status column..
                    //tblPDF.AddCell(GetCusotmPDFCell("Planned", _colspan: rptType.ToUpper() == "BOQ" ? 5 : 3, _isHeaderFooter: true, alignType: "Center"));
                    //tblPDF.AddCell(GetCusotmPDFCell("As-Built", _colspan: rptType.ToUpper() == "BOQ" ? 5 : 3, _isHeaderFooter: true, alignType: "Center"));
                    //tblPDF.AddCell(GetCusotmPDFCell("Dormant", _colspan: rptType.ToUpper() == "BOQ" ? 5 : 3, _isHeaderFooter: true, alignType: "Center"));

                    //Planned columns
                    //tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_403, _isHeaderFooter: true));
                    //if (rptType.ToUpper() == "BOQ")
                    //{
                    //    tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), _isHeaderFooter: true));
                    //    tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), _isHeaderFooter: true));
                    //}
                    //tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_023, _isHeaderFooter: true));
                    //tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_024, _isHeaderFooter: true));

                    ////As-Built columns
                    //tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_403, _isHeaderFooter: true));
                    //if (rptType.ToUpper() == "BOQ")
                    //{
                    //    tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), _isHeaderFooter: true));
                    //    tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), _isHeaderFooter: true));
                    //}
                    //tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_023, _isHeaderFooter: true));
                    //tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_024, _isHeaderFooter: true));

                    ////Dormant columns
                    //tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_403, _isHeaderFooter: true));
                    //if (rptType.ToUpper() == "BOQ")
                    //{
                    //    tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), _isHeaderFooter: true));
                    //    tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), _isHeaderFooter: true));
                    //}
                    //tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_023, _isHeaderFooter: true));
                    //tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_024, _isHeaderFooter: true));
                    #endregion
                    #endregion

                    #region ADD ROW DATA

                    var NtStatus = "";
                    int LstCnt = 0, PL = 0, AsB = 0, Dmt = 0;

                    foreach (BOMBOQReport objBom in lstBOMReport)
                    {
                        var sts = objBom.NetworkStatus == "P" ? "Planned" : objBom.NetworkStatus == "A" ? "As-Built" : objBom.NetworkStatus == "D" ? "Dormant" : "Combined";

                        /////////// Network Status
                        if (NtStatus != objBom.NetworkStatus)
                        {
                            /////////// Add total Network Status wise
                            if (NtStatus != "")
                            {
                                var q1 = lstBOMReport.Where(p => (p.is_header == true) && (p.NetworkStatus == NtStatus));

                                tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_041, _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                //priyanka
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                if (rptType.ToUpper() == "BOQ")
                                {
                                    tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                    tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                }
                                tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.total_count).ToString(), _isHeaderFooter: true));
                                if (rptType.ToUpper() == "BOQ")
                                {
                                    tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.total_cost).ToString(), _isHeaderFooter: true));
                                    tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.total_service_cost).ToString(), _isHeaderFooter: true));
                                }
                                tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.calculated_length).ToString(), _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.gis_length).ToString(), _isHeaderFooter: true));
                            }


                            PL += (objBom.NetworkStatus.ToUpper() == "P" ? 1 : 0);
                            AsB += (objBom.NetworkStatus.ToUpper() == "A" ? 1 : 0);
                            if (ApplicationSettings.IsDormantEnabled)
                            {
                                Dmt = Dmt + (@objBom.NetworkStatus.ToUpper() == "D" ? 1 : 0);
                            }
                            else { Dmt = Dmt = 1; }
                            if (objBom.NetworkStatus.ToUpper() == "C")
                            {
                                if (PL == 0)
                                {
                                    tblPDF.AddCell(GetCusotmPDFCell("Planned", _colspan: rptType.ToUpper() == "BOQ" ? 12 : 8, alignType: "Left", _isCustomBackgroundColor: true, clrs: System.Drawing.ColorTranslator.FromHtml("#2d3e50")));
                                    tblPDF.AddCell(GetCusotmPDFCell("              " + Resources.Resources.SI_OSP_GBL_GBL_GBL_051, _colspan: rptType.ToUpper() == "BOQ" ? 12 : 8, alignType: "Left"));
                                }
                                if (AsB == 0)
                                {
                                    tblPDF.AddCell(GetCusotmPDFCell("As-Built", _colspan: rptType.ToUpper() == "BOQ" ? 12 : 8, alignType: "Left", _isCustomBackgroundColor: true, clrs: System.Drawing.ColorTranslator.FromHtml("#2d3e50")));
                                    tblPDF.AddCell(GetCusotmPDFCell("              " + Resources.Resources.SI_OSP_GBL_GBL_GBL_051, _colspan: rptType.ToUpper() == "BOQ" ? 12 : 8, alignType: "Left"));
                                }
                                if (Dmt == 0)
                                {
                                    tblPDF.AddCell(GetCusotmPDFCell("Dormant", _colspan: rptType.ToUpper() == "BOQ" ? 12 : 8, alignType: "Left", _isCustomBackgroundColor: true, clrs: System.Drawing.ColorTranslator.FromHtml("#2d3e50")));
                                    tblPDF.AddCell(GetCusotmPDFCell("              " + Resources.Resources.SI_OSP_GBL_GBL_GBL_051, _colspan: rptType.ToUpper() == "BOQ" ? 12 : 8, alignType: "Left"));
                                }
                            }
                            tblPDF.AddCell(GetCusotmPDFCell(sts, _colspan: rptType.ToUpper() == "BOQ" ? 12 : 8, alignType: "Left", _isCustomBackgroundColor: true, clrs: System.Drawing.ColorTranslator.FromHtml("#2d3e50")));
                            NtStatus = objBom.NetworkStatus;
                        }

                        ///////////////// Check Cable count
                        if (objBom.entity_type.ToUpper() == "CABLE")
                        {
                            CblCnt++;
                        }

                        //////// Main body row data
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.is_header ? objBom.entity_type : objBom.entity_sub_type, _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.name, _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.item_code, _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.specification, _isSubheader: objBom.is_header));
                        //priyanka
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.served_by_ring, _isSubheader: objBom.is_header));
                        if (rptType.ToUpper() == "BOQ")
                        {
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.is_header ? "" : objBom.cost_per_unit.ToString(), _isSubheader: objBom.is_header));
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.is_header ? "" : objBom.service_cost_per_unit.ToString(), _isSubheader: objBom.is_header));

                        }
                        //planned Columns
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.total_count.ToString(), _isSubheader: objBom.is_header));
                        if (rptType.ToUpper() == "BOQ")
                        {
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.total_cost.ToString(), _isSubheader: objBom.is_header));
                            tblPDF.AddCell(GetCusotmPDFCell(objBom.total_service_cost.ToString(), _isSubheader: objBom.is_header));
                        }
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.geom_type.ToUpper() == "LINE" ? objBom.calculated_length.ToString() : "", _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.geom_type.ToUpper() == "LINE" ? objBom.gis_length.ToString() : "", _isSubheader: objBom.is_header));

                        LstCnt++;
                        #region ADD FOOTER COLUMN
                        //lstBOMReport = Utility.CommonUtility.GetFormattedDataTable(lstBOMReport, ApplicationSettings.numberFormatType);
                        if ((LstCnt == lstBOMReport.Count))
                        {
                            var NtwkSts = BOMReport.objReportFilters.SelectedNetworkStatues.ToUpper();
                            if (NtwkSts == "P" || NtwkSts == "A" || NtwkSts == "D")
                            {
                                var q1 = lstBOMReport.Where(p => (p.is_header == true) && (p.NetworkStatus == NtStatus));

                                tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_041, _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                if (rptType.ToUpper() == "BOQ")
                                {
                                    tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                    tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                }
                                tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.total_count).ToString(), _isHeaderFooter: true));
                                if (rptType.ToUpper() == "BOQ")
                                {
                                    tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.total_cost).ToString(), _isHeaderFooter: true));
                                    tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.service_cost_per_unit).ToString(), _isHeaderFooter: true));
                                }
                                tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.calculated_length).ToString(), _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.gis_length).ToString(), _isHeaderFooter: true));
                            }


                            if ((NtStatus == "C"))
                            {
                                var q1 = lstBOMReport.Where(p => (p.is_header == true) && (p.NetworkStatus == NtStatus));

                                tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_FRM_042, _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                if (rptType.ToUpper() == "BOQ")
                                {
                                    tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                    tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                                }
                                tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.total_count).ToString(), _isHeaderFooter: true));
                                if (rptType.ToUpper() == "BOQ")
                                {
                                    tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.total_cost).ToString(), _isHeaderFooter: true));
                                    tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.service_cost_per_unit).ToString(), _isHeaderFooter: true));
                                }
                                tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.calculated_length).ToString(), _isHeaderFooter: true));
                                tblPDF.AddCell(GetCusotmPDFCell(q1.Sum(m => m.gis_length).ToString(), _isHeaderFooter: true));
                            }
                        }
                        #endregion

                    }

                    pdfDoc.Add(tblPDF);
                    #endregion

                    #region Loss Details
                    if (isAttr.Contains("dBLOSS"))
                    {
                        if (BOMReport.objReportFilters.isdBLossAttrEnable == true)
                        {
                            if (CblCnt > 0)
                            {
                                PdfPTable tblPDFLoss = tblPDF = new PdfPTable(5);
                                tblPDFLoss.SpacingBefore = 20f;
                                tblPDFLoss.SetTotalWidth(new float[] { 20f, 20f, 20f, 20f, 20f });
                                tblPDFLoss.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_FRM_043, alignType: "Center", _fontSize: 16, _colspan: 5, _isCustomBackgroundColor: true, clrs: System.Drawing.ColorTranslator.FromHtml("#296d98")));

                                tblPDFLoss.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_FRM_044, alignType: "Center", _isHeaderFooter: true));
                                tblPDFLoss.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_302 + " (Km)", alignType: "Center", _isHeaderFooter: true));
                                tblPDFLoss.AddCell(GetCusotmPDFCell(Resources.Resources.SI_GBL_GBL_NET_FRM_100 + "(" + ApplicationSettings.dBLossUnit + ")", alignType: "Center", _isHeaderFooter: true));
                                tblPDFLoss.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_FRM_212 + "(" + ApplicationSettings.dBLossUnit + ")", alignType: "Center", _isHeaderFooter: true));
                                tblPDFLoss.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_FRM_213 + "(" + ApplicationSettings.dBLossUnit + ")", alignType: "Center", _isHeaderFooter: true));
                                if (BOMReport.objdBLoss.Count > 0)
                                {
                                    foreach (dBLossDetail objloss in BOMReport.objdBLoss)
                                    {
                                        tblPDFLoss.AddCell(GetCusotmPDFCell(objloss.cable_category, _isSubheader: false));
                                        tblPDFLoss.AddCell(GetCusotmPDFCell(Convert.ToString(objloss.total_cable_length), _isSubheader: false));
                                        tblPDFLoss.AddCell(GetCusotmPDFCell(Convert.ToString(objloss.cable_total_splice_loss), _isSubheader: false));
                                        tblPDFLoss.AddCell(GetCusotmPDFCell(Convert.ToString(objloss.misc_loss), _isSubheader: false));
                                        tblPDFLoss.AddCell(GetCusotmPDFCell(Convert.ToString(objloss.totalLoss), _isSubheader: false));
                                    }
                                    var ql = BOMReport.objdBLoss;
                                    tblPDFLoss.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_FRM_049 + "(" + ApplicationSettings.dBLossUnit + ")", _colspan: 4, _isHeaderFooter: true));
                                    tblPDFLoss.AddCell(GetCusotmPDFCell(ql.Sum(m => m.totalLoss).ToString(), _isHeaderFooter: true));
                                }
                                else
                                {
                                    tblPDFLoss.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_RPT_001, _colspan: 5));
                                }

                                pdfDoc.Add(tblPDFLoss);
                            }
                        }
                    }

                    #endregion

                    #region Additional Attributes
                    if (isAttr.Contains("FTRD"))
                    {

                        if (BOMReport.objAdAttribute.isAdditionalAttrEnable == true)
                        {
                            PdfPTable tblPDFfooter = tblPDF = new PdfPTable(4);
                            tblPDFfooter.SpacingBefore = 50f;
                            tblPDFfooter.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_270 +":-", alignType: "Left", IsBorder: false, _fontSize: 12));
                            tblPDFfooter.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_PM_NET_FRM_012 + ":-", alignType: "Left", IsBorder: false, _fontSize: 12));
                            tblPDFfooter.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_PM_NET_FRM_028 + ":-", alignType: "Left", IsBorder: false, _fontSize: 12));
                            tblPDFfooter.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_PM_NET_FRM_013 + ":-", alignType: "Left", IsBorder: false, _fontSize: 12));

                            tblPDFfooter.SetTotalWidth(new float[] { 20f, 20f, 20f, 20f });
                            tblPDF.AddCell(GetCusotmPDFCell(BOMReport.objAdAttribute.estimatedby, alignType: "Left", IsBorder: false, _fontSize: 11));
                            //tblPDF.AddCell(GetCusotmPDFCell(""));
                            tblPDF.AddCell(GetCusotmPDFCell(BOMReport.objAdAttribute.checkedby, alignType: "Left", IsBorder: false, _fontSize: 11));
                            //tblPDF.AddCell(GetCusotmPDFCell(""));
                            tblPDF.AddCell(GetCusotmPDFCell(BOMReport.objAdAttribute.re_checkedby, alignType: "Left", IsBorder: false, _fontSize: 11));
                            //tblPDF.AddCell(GetCusotmPDFCell(""));
                            tblPDF.AddCell(GetCusotmPDFCell(BOMReport.objAdAttribute.approvedby, alignType: "Left", IsBorder: false, _fontSize: 11));
                            pdfDoc.Add(tblPDFfooter);
                        }
                    }
                    #endregion


                }
                pdfDoc.Close();
                string filename = rptType + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss") + ".pdf";
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                System.Web.HttpContext.Current.Response.Write(pdfDoc);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write(ex.Message);
            }
        }
        public static void ExportNetowrkStatusWiseBOMBOQReportInPDF(List<BOMReport> lstBOMReport, string networkStatus, string rptType)
        {

            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 40, 50);
            try
            {
                Font font = new Font(GetFont(), 12, Font.NORMAL);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, System.Web.HttpContext.Current.Response.OutputStream);
                writer.PageEvent = new AllPdfPageEvents();
                pdfDoc.Open();
                //ADD REPORT HEADING...
                pdfDoc.Add(new Paragraph(new Chunk(rptType.ToUpper(), font))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                });
                if (lstBOMReport != null && lstBOMReport.Count > 0)
                {
                    #region DEFINE TABLE AND COLUMNS
                    PdfPTable tblPDF = null;
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF = new PdfPTable(9);
                        tblPDF.SetTotalWidth(new float[] { 10f, 30f, 10f, 10f, 10f, 10f, 15f, 15f, 15f });
                    }
                    else
                    {
                        tblPDF = new PdfPTable(5);
                        tblPDF.SetTotalWidth(new float[] { 15f, 40f, 15f, 15f, 15f });
                    }
                    #endregion

                    #region ADD HEADER COLUMNS
                    // ADD HEADER COLUMNS...
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_144, _rowSpan: 2, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_067, _rowSpan: 2, _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_143, ApplicationSettings.Currency), _rowSpan: 2, _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_142, ApplicationSettings.Currency), _rowSpan: 2, _isHeaderFooter: true));
                    }
                    //add network status column..
                    var NetworkStatusText = networkStatus.ToUpper() == "P" ? "Planned" : (networkStatus.ToUpper() == "A" ? " As-Built" : "Dormant");
                    tblPDF.AddCell(GetCusotmPDFCell(NetworkStatusText, _colspan: rptType.ToUpper() == "BOQ" ? 5 : 3, _isHeaderFooter: true, alignType: "Center"));

                    //Count 
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_FRM_403, _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), _isHeaderFooter: true));
                    }
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_023, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_NET_RPT_024, _isHeaderFooter: true));
                    #endregion

                    #region ADD ROWS DATA
                    foreach (BOMReport objBom in lstBOMReport)
                    {

                        var entity_type = objBom.is_header ? objBom.entity_type : objBom.entity_sub_type;
                        var cost_per_unit = objBom.is_header ? "" : objBom.cost_per_unit.ToString();
                        var serviceCostPerUnit = objBom.is_header ? "" : objBom.service_cost_per_unit.ToString();
                        var count = networkStatus.ToUpper() == "P" ? objBom.total_planned_count.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_count.ToString() : objBom.total_dormant_count.ToString());
                        var total_cost = networkStatus.ToUpper() == "P" ? objBom.total_planned_cost.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_cost.ToString() : objBom.total_dormant_cost.ToString());
                        var totalServiceSost = networkStatus.ToUpper() == "P" ? objBom.total_planned_service_cost.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_service_cost.ToString() : objBom.total_dormant_service_cost.ToString());
                        var total_calc_length = objBom.geom_type.ToUpper() == "LINE" ? (networkStatus.ToUpper() == "P" ? objBom.total_planned_calc_length.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_calc_length.ToString() : objBom.total_dormant_calc_length.ToString())) : "NA";
                        var total_gis_length = objBom.geom_type.ToUpper() == "LINE" ? (networkStatus.ToUpper() == "P" ? objBom.total_planned_gis_length.ToString() : (networkStatus.ToUpper() == "A" ? objBom.total_asbuilt_gis_length.ToString() : objBom.total_dormant_gis_length.ToString())) : "NA";

                        tblPDF.AddCell(GetCusotmPDFCell(entity_type, _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(objBom.specification, _isSubheader: objBom.is_header));
                        //priyanka
                        //tblPDF.AddCell(GetCusotmPDFCell(objBom.served_by_ring, _isSubheader: objBom.is_header));
                        if (rptType.ToUpper() == "BOQ")
                        {
                            tblPDF.AddCell(GetCusotmPDFCell(cost_per_unit, _isSubheader: objBom.is_header));
                            tblPDF.AddCell(GetCusotmPDFCell(serviceCostPerUnit, _isSubheader: objBom.is_header));
                        }
                        tblPDF.AddCell(GetCusotmPDFCell(count, _isSubheader: objBom.is_header));

                        if (rptType.ToUpper() == "BOQ")
                        {
                            tblPDF.AddCell(GetCusotmPDFCell(total_cost, _isSubheader: objBom.is_header));
                            tblPDF.AddCell(GetCusotmPDFCell(totalServiceSost, _isSubheader: objBom.is_header));

                        }
                        tblPDF.AddCell(GetCusotmPDFCell(total_calc_length, _isSubheader: objBom.is_header));
                        tblPDF.AddCell(GetCusotmPDFCell(total_gis_length, _isSubheader: objBom.is_header));
                    }
                    #endregion

                    #region ADD FOOTER COLUMN

                    var query = lstBOMReport.Where(p => p.is_header == true);

                    var totalCount = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_count).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_count).ToString() : query.Sum(p => p.total_dormant_count).ToString());
                    var totalCost = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_cost).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_cost).ToString() : query.Sum(p => p.total_dormant_cost).ToString());
                    var total_service_Cost = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_service_cost).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_service_cost).ToString() : query.Sum(p => p.total_dormant_service_cost).ToString());
                    var totalCalcLength = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_calc_length).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_calc_length).ToString() : query.Sum(p => p.total_dormant_calc_length).ToString());
                    var totalGISLength = networkStatus.ToUpper() == "P" ? query.Sum(p => p.total_planned_gis_length).ToString() : (networkStatus.ToUpper() == "A" ? query.Sum(p => p.total_asbuilt_gis_length).ToString() : query.Sum(p => p.total_dormant_gis_length).ToString());

                    tblPDF.AddCell(GetCusotmPDFCell(Resources.Resources.SI_OSP_GBL_GBL_GBL_041, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell("", _isHeaderFooter: true));

                    }
                    tblPDF.AddCell(GetCusotmPDFCell(totalCount, _isHeaderFooter: true));

                    if (rptType.ToUpper() == "BOQ")
                    {
                        tblPDF.AddCell(GetCusotmPDFCell(totalCost, _isHeaderFooter: true));
                        tblPDF.AddCell(GetCusotmPDFCell(total_service_Cost, _isHeaderFooter: true));

                    }
                    tblPDF.AddCell(GetCusotmPDFCell(totalCalcLength, _isHeaderFooter: true));
                    tblPDF.AddCell(GetCusotmPDFCell(totalGISLength, _isHeaderFooter: true));

                    #endregion


                    pdfDoc.Add(tblPDF);
                }
                pdfDoc.Close();
                string filename = rptType + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss") + ".pdf";
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                System.Web.HttpContext.Current.Response.Write(pdfDoc);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write(ex.Message);
            }
        }

        public static MemoryStream ExportMapToPdf(Rectangle cordinates, string Title, iTextSharp.text.Image layerImage, iTextSharp.text.Image legendImg, string legend)
        {
            legend = legend.Replace("alt=\"printLayerImg\">", "alt=\"printLayerImg\"/>");
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                var color = new BaseColor(27, 148, 97);
                Document pdfDoc = new Document(cordinates);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                pdfWriter.PageEvent = new AllPdfPageEvents();

                //OPEN PDFDOC OBJECT TO WRITE CONTENT
                pdfDoc.Open();

                layerImage.Border = Rectangle.BOX;
                layerImage.BorderColor = color;
                layerImage.BorderWidth = 1f;
                layerImage.Alignment = Element.ALIGN_CENTER;

                pdfDoc.Add(new Paragraph(new Chunk(Title, FontFactory.GetFont("Verdana", 14)))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                });

                // ADD LEGNED TEXT AS A IMAGE
                StringReader sr = new StringReader(legend);
                XMLWorkerHelper.GetInstance().ParseXHtml(pdfWriter, pdfDoc, sr);

                //set layer image...
                layerImage = ResizeImageKeepAspectRatio(layerImage, Convert.ToInt32(cordinates.Width - 71), Convert.ToInt32(cordinates.Height - 280));

                pdfDoc.Add(layerImage);

                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                return stream;
            }
        }
        public static MemoryStream ExportMapToPdf1(Rectangle coordinates, string Title, iTextSharp.text.Image layerImage, dynamic objLegend, string pageSize)
        {
            Legend lgnd = new Legend();
            lgnd = objLegend;


            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                var color = new BaseColor(27, 148, 97);
                var legendHeadingColor = new BaseColor(220, 220, 220);
                BaseFont bfTimesRoman = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                Document pdfDoc = new Document(coordinates);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                pdfWriter.PageEvent = new AllPdfPageEvents();
                pdfDoc.SetMargins(12f, 12f, 30f, 20f);
                pdfDoc.Open();

                pdfDoc.Add(new Paragraph(new Chunk(Title, FontFactory.GetFont("Verdana", 7)))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                });


                PdfPTable mapTable = new PdfPTable(2);
                mapTable.WidthPercentage = 100;

                float legendWidth = pdfDoc.PageSize.Width - layerImage.Width;
                mapTable.SetTotalWidth(new float[] { 70f, 30f });

                PdfPCell cell1 = new PdfPCell()
                {
                    PaddingLeft = MapPadding(pageSize),
                    PaddingRight = MapPadding(pageSize),
                    PaddingTop = MapPadding(pageSize),
                    PaddingBottom = MapPadding(pageSize)
                };


                #region Map
                layerImage.Border = Rectangle.BOX; layerImage.BorderColor = color; layerImage.BorderWidth = 0.2f;
                //layerImage = ResizeImageKeepAspectRatio(layerImage, 527, 527);
                cell1.AddElement(layerImage);
                #endregion



                PdfPCell cell2 = new PdfPCell() { PaddingLeft = 5f, PaddingRight = 5f };


                #region Legend Heading
                PdfPTable tblLegendHeading = new PdfPTable(1);
                tblLegendHeading.SpacingBefore = TableSpace(pageSize);
                tblLegendHeading.SpacingAfter = TableSpace(pageSize);
                tblLegendHeading.WidthPercentage = 100;
                tblLegendHeading.AddCell(LegendContentSize(pageSize));
                cell2.AddElement(tblLegendHeading);
                #endregion


                #region Network Status
                PdfPTable tblNetworkStatus = new PdfPTable(3);
                tblNetworkStatus.WidthPercentage = 100;
                tblNetworkStatus.PaddingTop = 5f;
                tblNetworkStatus.SpacingAfter = TableSpace(pageSize);


                iTextSharp.text.Image imgPlanned = iTextSharp.text.Image.GetInstance(objLegend.baseUrl + "Planned.png"); imgPlanned.ScaleAbsolute(EntityImageSize(pageSize), EntityImageSize(pageSize));
                iTextSharp.text.Image imgDormant = iTextSharp.text.Image.GetInstance(objLegend.baseUrl + "Dorment.png"); imgDormant.ScaleAbsolute(EntityImageSize(pageSize), EntityImageSize(pageSize));
                iTextSharp.text.Image imgAsBuilt = iTextSharp.text.Image.GetInstance(objLegend.baseUrl + "AsBuilt.png"); imgAsBuilt.ScaleAbsolute(EntityImageSize(pageSize), EntityImageSize(pageSize));

                var planned = new Paragraph(new Chunk(imgPlanned, 0, 0, true)); planned.Add(new Chunk("  Planned", FontFactory.GetFont("Arial", NetworkContentSize(pageSize), iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                var dormant = new Paragraph(new Chunk(imgDormant, 0, 0, true)); dormant.Add(new Chunk("  Dorment", FontFactory.GetFont("Arial", NetworkContentSize(pageSize), iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                var asbuilt = new Paragraph(new Chunk(imgAsBuilt, 0, 0, true)); asbuilt.Add(new Chunk("  As Built", FontFactory.GetFont("Arial", NetworkContentSize(pageSize), iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));



                PdfPCell cellPlanned = new PdfPCell() { BorderWidth = 0 }; cellPlanned.AddElement(planned); tblNetworkStatus.AddCell(cellPlanned);
                PdfPCell cellDormant = new PdfPCell() { BorderWidth = 0 }; cellDormant.AddElement(dormant); tblNetworkStatus.AddCell(cellDormant);
                PdfPCell cellAsbuilt = new PdfPCell() { BorderWidth = 0 }; cellAsbuilt.AddElement(asbuilt); tblNetworkStatus.AddCell(cellAsbuilt);



                cell2.AddElement(tblNetworkStatus);
                #endregion


                #region  GroupName and Entities
                List<string> lstGroupname = lgnd.legendList.Select(x => x.group_name).Distinct().ToList();
                foreach (var gName in lstGroupname)
                {

                    //Group Name
                    PdfPTable tblGN = new PdfPTable(1);
                    tblGN.WidthPercentage = 100;
                    tblGN.AddCell(GroupContentSize(pageSize, gName));
                    cell2.AddElement(tblGN);


                    //Group Entity Names and Count
                    var lstEntityGrouptName = lgnd.legendList.Where(x => x.group_name == gName).ToList();
                    PdfPTable tblGroup = new PdfPTable(2);
                    tblGroup.WidthPercentage = 100;
                    tblGroup.SpacingAfter = TableSpace(pageSize);
                    foreach (var item in lstEntityGrouptName)
                    {
                        iTextSharp.text.Image entityImage = iTextSharp.text.Image.GetInstance(objLegend.baseUrl + item.icon_path);
                        entityImage.ScaleAbsolute(EntityImageSize(pageSize), EntityImageSize(pageSize));

                        tblGroup.AddCell(EntityCellSize(entityImage, pageSize, item.sub_Layer, item.entity_count.ToString()));
                    }
                    tblGroup.AddCell("");
                    cell2.AddElement(tblGroup);
                }
                #endregion

                mapTable.AddCell(cell1);
                mapTable.AddCell(cell2);
                pdfDoc.Add(mapTable);
                pdfDoc.Close();
                return stream;

            }
        }


        public static PdfPCell LegendContentSize(string PageSize)
        {
            var legendHeadingColor = new BaseColor(220, 220, 220);
            if (PageSize == "A3")
            {
                PdfPCell cellLegendHeading = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    PaddingBottom = 8f,
                    MinimumHeight = 8f
                };
                var Legends = new Paragraph(new Chunk("Legends", FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellLegendHeading.AddElement(Legends);
                return cellLegendHeading;
            }
            else if (PageSize == "A2")
            {
                PdfPCell cellLegendHeading = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    PaddingBottom = 12f,
                    MinimumHeight = 12f
                };
                var Legends = new Paragraph(new Chunk("Legends", FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellLegendHeading.AddElement(Legends);
                return cellLegendHeading;
            }
            else if (PageSize == "A1")
            {
                PdfPCell cellLegendHeading = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    PaddingBottom = 17f,
                    MinimumHeight = 17f
                };
                var Legends = new Paragraph(new Chunk("Legends", FontFactory.GetFont("Arial", 22, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellLegendHeading.AddElement(Legends);
                return cellLegendHeading;
            }
            else if (PageSize == "A0")
            {
                PdfPCell cellLegendHeading = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    MinimumHeight = 25f,
                    PaddingBottom = 25f
                };
                var Legends = new Paragraph(new Chunk("Legends", FontFactory.GetFont("Arial", 30, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellLegendHeading.AddElement(Legends);
                return cellLegendHeading;
            }
            else
            {
                PdfPCell cellLegendHeading = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    PaddingBottom = 5f,
                    MinimumHeight = 5f
                };
                var Legends = new Paragraph(new Chunk("Legends", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellLegendHeading.AddElement(Legends);
                return cellLegendHeading;
            }

        }

        public static PdfPCell GroupContentSize(string PageSize, string groupname)
        {
            var legendHeadingColor = new BaseColor(220, 220, 220);
            if (PageSize == "A3")
            {
                PdfPCell cellGN = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    PaddingTop = 8f,
                    PaddingBottom = 3f

                };
                var pGroupName = new Paragraph(new Chunk(groupname, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellGN.AddElement(pGroupName);
                return cellGN;
            }
            else if (PageSize == "A2")
            {
                PdfPCell cellGN = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    PaddingTop = 10f,
                    PaddingBottom = 3f

                };
                var pGroupName = new Paragraph(new Chunk(groupname, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellGN.AddElement(pGroupName);
                return cellGN;
            }
            else if (PageSize == "A1")
            {
                PdfPCell cellGN = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    PaddingTop = 13f,
                    PaddingBottom = 3f

                };
                var pGroupName = new Paragraph(new Chunk(groupname, FontFactory.GetFont("Arial", 19, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellGN.AddElement(pGroupName);
                return cellGN;
            }
            else if (PageSize == "A0")
            {
                PdfPCell cellGN = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    PaddingTop = 15f,
                    PaddingBottom = 3f

                };
                var pGroupName = new Paragraph(new Chunk(groupname, FontFactory.GetFont("Arial", 28, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellGN.AddElement(pGroupName);
                return cellGN;
            }
            else
            {
                PdfPCell cellGN = new PdfPCell()
                {
                    BackgroundColor = legendHeadingColor,
                    BorderWidth = 0,
                    MinimumHeight = 2f,
                    PaddingBottom = 3f

                };
                var pGroupName = new Paragraph(new Chunk(groupname, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellGN.AddElement(pGroupName);
                return cellGN;
            }
        }

        public static int NetworkContentSize(string PageSize)
        {
            if (PageSize == "A3")
            {
                return 10;
            }
            else if (PageSize == "A2")
            {
                return 15;
            }
            else if (PageSize == "A1")
            {
                return 20;
            }
            else if (PageSize == "A0")
            {
                return 28;
            }
            else
            {
                return 8;
            }
        }
        public static MapReport GetMapReportContent(string PageSize)
        {
            if (PageSize == "A3")
            {
                return MapReport.A3;
            }
            else if (PageSize == "A2")
            {
                return MapReport.A2;
            }
            else if (PageSize == "A1")
            {
                return MapReport.A1;
            }
            else if (PageSize == "A0")
            {
                return MapReport.A0;
            }
            else
            {
                return MapReport.A0;
            }
        }

        public static PdfPCell EntityCellSize(iTextSharp.text.Image entityImage, string pageSize, string entityName, string EntityCount)
        {
            if (pageSize == "A3")
            {
                var p = new Paragraph(new Chunk(entityImage, 0, 0, true));
                p.Add(new Chunk(" " + entityName, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                p.Add(new Chunk("  (" + EntityCount + ")", FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));


                PdfPCell cellLegends = new PdfPCell()
                {
                    BorderWidth = 0.1f,
                    PaddingTop = 7f,
                    PaddingBottom = 3f
                };
                cellLegends.AddElement(p);
                return cellLegends;
            }
            else if (pageSize == "A2")
            {
                var p = new Paragraph(new Chunk(entityImage, 0, 0, true));
                p.Add(new Chunk(" " + entityName, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                p.Add(new Chunk("  (" + EntityCount + ")", FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));


                PdfPCell cellLegends = new PdfPCell()
                {
                    BorderWidth = 0.1f,
                    PaddingTop = 15f,
                    PaddingBottom = 3f
                };
                cellLegends.AddElement(p);
                return cellLegends;
            }
            else if (pageSize == "A1")
            {
                var p = new Paragraph(new Chunk(entityImage, 0, 0, true));
                p.Add(new Chunk(" " + entityName, FontFactory.GetFont("Arial", 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                p.Add(new Chunk("  (" + EntityCount + ")", FontFactory.GetFont("Arial", 18, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));


                PdfPCell cellLegends = new PdfPCell()
                {
                    BorderWidth = 0.1f,
                    PaddingTop = 20f,
                    PaddingBottom = 3f
                };
                cellLegends.AddElement(p);
                return cellLegends;
            }
            else if (pageSize == "A0")
            {
                var p = new Paragraph(new Chunk(entityImage, 0, 0, true));
                p.Add(new Chunk(" " + entityName, FontFactory.GetFont("Arial", 25, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                p.Add(new Chunk("  (" + EntityCount + ")", FontFactory.GetFont("Arial", 25, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));


                PdfPCell cellLegends = new PdfPCell()
                {
                    BorderWidth = 0.1f,
                    PaddingTop = 35f,
                    PaddingBottom = 5f
                };
                cellLegends.AddElement(p);
                return cellLegends;
            }
            else
            {

                var p = new Paragraph(new Chunk(entityImage, 0, 0, true));
                p.Add(new Chunk(" " + entityName, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                p.Add(new Chunk("  (" + EntityCount + ")", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));


                PdfPCell cellLegends = new PdfPCell()
                {
                    BorderWidth = 0.1f
                };
                cellLegends.AddElement(p);
                return cellLegends;
            }
        }

        public static float EntityImageSize(string PageSize)
        {
            if (PageSize == "A3")
            {
                return 11;
            }
            else if (PageSize == "A2")
            {
                return 13;
            }
            else if (PageSize == "A1")
            {
                return 25;
            }
            else if (PageSize == "A0")
            {
                return 30;
            }
            else
            {
                return 8;
            }
        }

        public static float TableSpace(string PageSize)
        {
            if (PageSize == "A3")
            {
                return 13;
            }
            else if (PageSize == "A2")
            {
                return 16;
            }
            else if (PageSize == "A1")
            {
                return 21;
            }
            else if (PageSize == "A0")
            {
                return 30;
            }
            else
            {
                return 7;
            }
        }

        public static float MapPadding(string PageSize)
        {
            if (PageSize == "A3")
            {
                return 13;

            }
            else if (PageSize == "A2")
            {
                return 16;
            }
            else if (PageSize == "A1")
            {
                return 19;
            }
            else if (PageSize == "A0")
            {
                return 22;
            }
            else
            {
                return 10;
            }
        }



        public partial class AllPdfPageEvents : PdfPageEventHelper
        {
            int PageNumber;
            string headerDate = "";
            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                PageNumber++;
            }

            public override void OnOpenDocument(PdfWriter writer, Document doc)
            {
                PdfPTable headerTbl = new PdfPTable(2);
                Paragraph p = new Paragraph();
                headerTbl.TotalWidth = doc.PageSize.Width - 12f - 12f;
                if (!string.IsNullOrEmpty(ApplicationSettings.ClientLogoImageBytesForWeb))
                {
                    if (ApplicationSettings.ClientLogoImageBytesForWeb.Trim() != "")
                    {
                        string[] arr = ApplicationSettings.ClientLogoImageBytesForWeb.Split(',');
                        byte[] imageBytes = Convert.FromBase64String(arr[1]);
                        Image logo = Image.GetInstance(imageBytes);
                        logo.ScaleToFit(90f, 30f);
                        logo.SetAbsolutePosition(0f, 0f);
                        p.Add(new Chunk(logo, 0, -5, true));
                    }
                }
                if (ApplicationSettings.isClientNameRequiredOnLoginPage)
                {
                    p.Add(new Chunk("   " + ApplicationSettings.ClientName, FontFactory.GetFont("Arial", 16, new BaseColor(System.Drawing.Color.Black))));
                }
                headerTbl.AddCell((new PdfPCell(p) { Border = 0, BorderWidthBottom = 1, PaddingBottom = 5 }));

                Paragraph pDate = new Paragraph(new Chunk("Date: " + DateTimeHelper.DateTimeFormatWithTime(DateTimeHelper.Now.ToString()), FontFactory.GetFont("ARIAL", 10)));
                headerTbl.AddCell(new PdfPCell(pDate) { Border = 0, BorderWidthBottom = 1, HorizontalAlignment = Element.ALIGN_RIGHT });
                headerTbl.WriteSelectedRows(0, -1, 12, (doc.PageSize.Height - 10), writer.DirectContent);
            }


            public override void OnEndPage(PdfWriter writer, Document document)
            {

                Paragraph PageNumText = new Paragraph(new Chunk("Page " + PageNumber, FontFactory.GetFont("Verdana", 10)));
                PageNumText.Alignment = Element.ALIGN_RIGHT;
                //Paragraph PoweredByText = new Paragraph(new Chunk("Powered by Lepton Software", FontFactory.GetFont("Verdana", 10)));
                Paragraph PoweredByText = new Paragraph(new Chunk(" ", FontFactory.GetFont("Verdana", 10)));
                PoweredByText.Alignment = Element.ALIGN_RIGHT;
                PdfPTable footerTbl = new PdfPTable(new float[] { 50f, 50f });
                //footerTbl.WidthPercentage = 100;
                footerTbl.TotalWidth = document.PageSize.Width - 12f - 12f;
                footerTbl.HorizontalAlignment = Element.ALIGN_RIGHT;
                PdfPCell cell = new PdfPCell(PoweredByText);
                cell.Border = 0;
                cell.BorderWidthTop = 1;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                footerTbl.AddCell(cell);
                PdfPCell cell2 = new PdfPCell(PageNumText);
                cell2.Border = 0;
                cell2.BorderWidthTop = 1;
                cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                footerTbl.AddCell(cell2);
                footerTbl.WriteSelectedRows(0, -1, 12, 30, writer.DirectContent);
            }
        }

        public static BaseFont GetFont()
        {
            BaseFont unicode = BaseFont.CreateFont();
            var FileName = HttpContext.Current.Server.MapPath("~/Content/LangFonts/" + HttpContext.Current.Session["fontName"] + ".ttf");
            if (File.Exists(FileName))
            {
                unicode = BaseFont.CreateFont(HttpContext.Current.Server.MapPath("~/Content/LangFonts/" + HttpContext.Current.Session["fontName"] + ".ttf"), BaseFont.IDENTITY_H, true);
            }

            return unicode;
        }

        public static Rectangle GetPageSize(string size)
        {
            var pageSize = PageSize.A0;

            switch (size)
            {
                case "A0":
                    pageSize = PageSize.A0;
                    break;
                case "A1":
                    pageSize = PageSize.A1;
                    break;
                case "A2":
                    pageSize = PageSize.A2;
                    break;
                case "A3":
                    pageSize = PageSize.A3;
                    break;
                case "A4":
                    pageSize = PageSize.A4;
                    break;
                default:
                    pageSize = PageSize.A0;
                    break;
            }
            return pageSize;
        }

        public static Image ResizeImageKeepAspectRatio(Image source, int width, int height)
        {
            if (source.Width != width || source.Height != height)
            {
                // Resize image
                float sourceRatio = (float)source.Width / source.Height;

                // Scaling
                float scaling;
                float scalingY = (float)source.Height / height;
                float scalingX = (float)source.Width / width;
                if (scalingX < scalingY) scaling = scalingX; else scaling = scalingY;

                int newWidth = (int)(source.Width / scaling);
                int newHeight = (int)(source.Height / scaling);

                // Correct float to int rounding
                if (newWidth < width) newWidth = width;
                if (newHeight < height) newHeight = height;
                source.ScaleAbsolute(newWidth, newHeight);
                if (newWidth > width || newHeight > height)
                {
                    source.ScaleToFit(width, height);
                }
                //source.RotationDegrees = 90;
                ////if(rotateDegree!=0)
                ////{
                ////    source = RotateImage(source, 0.0);
                ////}
            }
            return source;
        }

        private System.Drawing.Bitmap RotateImage(System.Drawing.Bitmap bmp, float angle)
        {
            System.Drawing.Bitmap rotatedImage = new System.Drawing.Bitmap(bmp.Width, bmp.Height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(rotatedImage))
            {
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform(angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new System.Drawing.Point(0, 0));
            }

            return rotatedImage;
        }

        private static PdfPCell GetCusotmPDFCell(string _text, int _fontSize = 9, string alignType = "LEFT", bool _isHeaderFooter = false, bool _isSubheader = false, int _rowSpan = 1, int _colspan = 1, bool _isCustomBackgroundColor = false, System.Drawing.Color? clrs = null, bool IsBorder = true)
        {
            Font font = new Font(GetFont(), _fontSize, Font.NORMAL);
            if (_isCustomBackgroundColor) { font.SetColor(255, 255, 255); }

            //if (double.TryParse(_text, out double d))
            //    _text = Utility.CommonUtility.GetFormattedNumber(d, ApplicationSettings.numberFormatType);
            //else if (int.TryParse(_text, out int i))
            //    _text = Utility.CommonUtility.GetFormattedNumber(i, ApplicationSettings.numberFormatType);

            return new PdfPCell(new Phrase(_text, font))
            {

                BackgroundColor = new BaseColor(_isHeaderFooter ? System.Drawing.Color.Aquamarine : (_isSubheader ? System.Drawing.Color.LightGray : _isCustomBackgroundColor ? (clrs ?? System.Drawing.Color.Transparent) : System.Drawing.Color.White)),
                BorderColor = new BaseColor(IsBorder ? System.Drawing.Color.Black : System.Drawing.Color.Transparent),
                HorizontalAlignment = alignType.ToUpper() == "CENTER" ? Element.ALIGN_CENTER : (alignType.ToUpper() == "RIGHT" ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT),
                BorderWidth = 1,
                Rowspan = _rowSpan,
                Colspan = _colspan,
                Padding = 3
            };
        }
        public System.Drawing.Bitmap Crop(System.Drawing.Bitmap bmp, System.Drawing.Rectangle selection)
        {
            //System.Drawing.Bitmap bmp = image as System.Drawing.Bitmap;

            // Check if it is a bitmap:
            if (bmp == null)
                throw new ArgumentException("No valid bitmap");

            // Crop the image:
            System.Drawing.Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);

            // Release the resources:
            //bmp.Dispose();

            return cropBmp;
        }
        public static System.Drawing.Bitmap ResizeBitmap(System.Drawing.Bitmap bmp, int width, int height)
        {
            System.Drawing.Bitmap result = new System.Drawing.Bitmap(width, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }


            //if (Directory.Exists("E:\\temp\\splicing\\"))
            //    result.Save(string.Format("E:\\temp\\splicing\\{0}.png", Utility.MiscHelper.getTimeStamp()));

            return result;
        }

        private static void AddImageToPDF(Document pdfDoc, System.Drawing.Bitmap bitmap)
        {
            //int ActualImageWidth = bitmap.Width;
            int ActualImageHeight = bitmap.Height;
            float horizontalMargin = pdfDoc.LeftMargin + pdfDoc.RightMargin + 10;
            float verticalMargin = pdfDoc.TopMargin + pdfDoc.BottomMargin + 25;

            int ActualPageHeight = Convert.ToInt32(pdfDoc.PageSize.Height) - Convert.ToInt32(verticalMargin);
            int ActualPageWidth = Convert.ToInt32(pdfDoc.PageSize.Width) - Convert.ToInt32(horizontalMargin);
            int pageIteration = 0;

            do
            {
                var x1 = 0;
                var y1 = pageIteration * ActualPageHeight;
                var x2 = bitmap.Width;
                //var y2 = ((pageIteration + 1) * ActualPageHeight) > ActualImageHeight ? ActualImageHeight : ((pageIteration + 1) * ActualPageHeight);
                var y2 = ActualPageHeight > ActualImageHeight ? ActualImageHeight : ActualPageHeight;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x1, y1, x2, y2);
                System.Drawing.Bitmap imgCropped = new PDFHelper().Crop(bitmap, rect);
                Image imgPlanned = Image.GetInstance(imgCropped, System.Drawing.Imaging.ImageFormat.Png);
                imgCropped.Dispose();
                imgPlanned.ScaleToFit(ActualPageWidth, ActualPageHeight);
                if (ActualImageHeight > ActualPageHeight)
                    pdfDoc.Add(imgPlanned);
                else
                {
                    pdfDoc.NewPage();
                    pdfDoc.Add(imgPlanned);
                }
                ActualImageHeight = ActualImageHeight - ActualPageHeight;
                pageIteration++;

            } while (ActualImageHeight > 0);
            bitmap.Dispose();
        }

        internal static void GenerateToPDF(DataSet ds, string fileName, string fileHeader, string image_path)
        {

            Document pdfDoc = new Document(PageSize.A3, 5f, 5f, 40f, 50f);
            try
            {
                pdfDoc.SetMargins(5f, 5f, 40f, 40f);

                Font font = new Font(GetFont(), 15, Font.NORMAL);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, HttpContext.Current.Response.OutputStream);
                writer.PageEvent = new AllPdfPageEvents();
                pdfDoc.Open();
                //ADD REPORT HEADING...
                //pdfDoc.Add(new Paragraph(new Chunk(fileHeader.ToUpper(), font))
                //{
                //    Alignment = Element.ALIGN_CENTER,
                //    SpacingAfter = 20
                //});

                float horizontalMargin = pdfDoc.LeftMargin + pdfDoc.RightMargin + 10;
                float verticalMargin = pdfDoc.TopMargin + pdfDoc.BottomMargin + 10;
                if (ds.Tables.Count > 0)
                {

                    #region DEFINE TABLE AND COLUMNS
                    foreach (DataTable dt in ds.Tables)
                    {
                        //ADD REPORT HEADING...
                        if (dt.Rows.Count > 0)
                        {
                            pdfDoc.Add(new Paragraph(new Chunk(dt.TableName, font))
                            {
                                Alignment = Element.ALIGN_CENTER,
                                SpacingAfter = 20
                            });
                            PdfPTable tblPDF = new PdfPTable(dt.Columns.Count);
                            #endregion

                            foreach (DataColumn column in dt.Columns)
                            {
                                PdfPCell cell1 = new PdfPCell(new Phrase("" + column.ColumnName + "", font));
                                cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell1.BackgroundColor = new BaseColor(243, 243, 243);

                                //cell1.FixedHeight = 25f;
                                tblPDF.AddCell(cell1);
                            }

                            font.Size = 9;
                            #region ADD ROWS DATA
                            PdfPCell PdfPCell = null;
                            for (int rows = 0; rows < dt.Rows.Count; rows++)
                            {
                                for (int column = 0; column < dt.Columns.Count; column++)
                                {
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), font)));
                                    tblPDF.AddCell(PdfPCell);
                                }
                            }
                            //tblPDF.SpacingBefore = 15f; // Give some space after the text or it may overlap the table            
                            pdfDoc.Add(tblPDF);

                            pdfDoc.Add(new Chunk("\n"));
                            //pdfDoc.NewPage();
                            font.Size = 15;
                        }
                    }

                    #endregion
                    //Image imgPlanned = Image.GetInstance(image_path);
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image_path);
                    int pageWidth = Convert.ToInt32(pdfDoc.PageSize.Width) - Convert.ToInt32(horizontalMargin);
                    int pageHeight = Convert.ToInt32(pdfDoc.PageSize.Height) - Convert.ToInt32(verticalMargin);
                    int actualImageHeight = Convert.ToInt32(bitmap.Height * (Convert.ToDouble(pageWidth) / Convert.ToDouble(bitmap.Width)));
                    if (actualImageHeight < pageHeight)
                    {
                        pageHeight = actualImageHeight;
                    }
                    if (bitmap.Width > pageWidth)
                    {
                        bitmap = ResizeBitmap(bitmap, pageWidth, pageHeight);
                    }
                    AddImageToPDF(pdfDoc, bitmap);

                    //System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, Convert.ToInt32(pdfDoc.PageSize.Height) - Convert.ToInt32(verticalMargin));
                    //System.Drawing.Bitmap imgCropped = new PDFHelper().Crop(bitmap, rect);
                    //bitmap.Dispose();

                    //Image imgPlanned = Image.GetInstance(imgCropped, System.Drawing.Imaging.ImageFormat.Png);
                    //imgCropped.Dispose();

                    ////imgPlanned.SetAbsolutePosition((PageSize.A4.Width - imgPlanned.ScaledWidth) / 2, (PageSize.A4.Rotate().Height - pdfDoc.BottomMargin - pdfDoc.TopMargin - imgPlanned.ScaledHeight) / 2 + pdfDoc.TopMargin);
                    ////var scalePercent = (((pdfDoc.PageSize.Width / imgPlanned.Width) * 100) - 4);
                    ////imgPlanned.ScalePercent(45);
                    ////To set the image in Size 1200 x 1000 in single PDF page.
                    //imgPlanned.ScaleToFit(1500, 1100);
                    //imgPlanned.ScaleToFit(pdfDoc.PageSize.Width - horizontalMargin, pdfDoc.PageSize.Height - verticalMargin);

                    //pdfDoc.Add(imgPlanned);

                    //pdfDoc.Add(new Chunk("\n"));
                    //pdfDoc.NewPage();


                    //System.Drawing.Bitmap bitmap1 = new System.Drawing.Bitmap(image_path);
                    //System.Drawing.Rectangle rect1 = new System.Drawing.Rectangle(0, Convert.ToInt32(pdfDoc.PageSize.Height) - Convert.ToInt32(verticalMargin), bitmap1.Width, ((Convert.ToInt32(pdfDoc.PageSize.Height) - Convert.ToInt32(verticalMargin)) * 2) > bitmap1.Height ? bitmap1.Height : ((Convert.ToInt32(pdfDoc.PageSize.Height) - Convert.ToInt32(verticalMargin)) * 2));
                    //System.Drawing.Bitmap imgCropped1 = new PDFHelper().Crop(bitmap1, rect1);
                    //bitmap1.Dispose();
                    //Image imgPlanned1 = Image.GetInstance(imgCropped1, System.Drawing.Imaging.ImageFormat.Png);
                    //imgPlanned1.ScaleToFit(pdfDoc.PageSize.Width - horizontalMargin, pdfDoc.PageSize.Height - verticalMargin);
                    //imgCropped1.Dispose();

                    //imgPlanned.SetAbsolutePosition((PageSize.A4.Width - imgPlanned.ScaledWidth) / 2, (PageSize.A4.Rotate().Height - pdfDoc.BottomMargin - pdfDoc.TopMargin - imgPlanned.ScaledHeight) / 2 + pdfDoc.TopMargin);
                    //var scalePercent = (((pdfDoc.PageSize.Width / imgPlanned.Width) * 100) - 4);
                    //imgPlanned.ScalePercent(45);
                    //To set the image in Size 1200 x 1000 in single PDF page.
                    //imgPlanned.ScaleToFit(1500, 1100);

                    //pdfDoc.Add(imgPlanned1);

                    //pdfDoc.Add(tblPDF);
                }


                pdfDoc.Close();
                string filename = fileName + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss") + ".pdf";
                HttpContext.Current.Response.ContentType = "application/pdf";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                HttpContext.Current.Response.Write(pdfDoc);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
        }
    }
}