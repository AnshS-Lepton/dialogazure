using BusinessLogics.Admin;
using BusinessLogics;
using Models.Admin;
using Models;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartInventory.Filters;
using Utility;
using SmartInventory.Helper;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using SmartInventory.Settings;

namespace SmartInventory.Controllers
{
	[Authorize]
	[SessionExpire]
	[HandleException]
	public class DashboardController : Controller
    {
		
		public ActionResult Index()
		{

			return View();
		}
		public JsonResult HierachyList()
		{
			var result = new BusinessLogics.BLDashboard().GetHierarchyList();
			return Json(new { Data = result });
		}
		public JsonResult DashboardResult(string state, string jc, string town, string partner, string fsa)
		{
			var output = new BusinessLogics.BLDashboard().GetDashboardResult(state, jc, town, partner, fsa);
			return Json(new { Data = output[0] });
		}
        public void ExportDashboardDataDump(string state, string jc, string town, string partner, string fsa, string date)
        {
            
            DataSet ds = new DataSet();
            List<Dictionary<string, string>> lstOutput = new BusinessLogics.BLDashboard().GetDashboardDumpResult(state, jc, town, partner, fsa);
            DataTable dtReport = new DataTable();
            dtReport = MiscHelper.GetDataTableFromDictionaries(lstOutput, true, ApplicationSettings.numberFormatType, new string[] { "Item_Code", "item code", "PTS_code" });
            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                if (dtReport.Columns.Contains("r4g_state_code")) { dtReport.Columns.Remove("r4g_state_code"); }
                if (dtReport.Columns.Contains("jc_sapplant_code")) { dtReport.Columns.Remove("jc_sapplant_code"); }
                if (dtReport.Columns.Contains("town_code")) { dtReport.Columns.Remove("town_code"); }
                if (dtReport.Columns.Contains("partner_prms_id")) { dtReport.Columns.Remove("partner_prms_id"); }
            }
            DataRow dr = dtReport.NewRow();
            if (dtReport.Rows.Count > 0)
            ds.Tables.Add(dtReport);
            if (dtReport != null && dtReport.Rows.Count > 0)
            {
               dtReport.Columns["s_no"].SetOrdinal(0);
                dtReport.Columns["fsa_id"].SetOrdinal(1);
                dtReport.Columns["ring"].SetOrdinal(2);
                dtReport.Columns["jc_id"].SetOrdinal(3);
                dtReport.Columns["town_id"].SetOrdinal(4);
                dtReport.Columns["feeder_scope"].SetOrdinal(5);
                dtReport.Columns["distribution_scope"].SetOrdinal(6);
                dtReport.Columns["s1_scope"].SetOrdinal(7);
                dtReport.Columns["s2_scope"].SetOrdinal(8);
                dtReport.Columns["csa_scope"].SetOrdinal(9);
                dtReport.Columns["cwip"].SetOrdinal(10);
                dtReport.Columns["feeder_asbuilt"].SetOrdinal(11);
                dtReport.Columns["distribution_asbuilt"].SetOrdinal(12);
                dtReport.Columns["s1_asbuilt"].SetOrdinal(13);
                dtReport.Columns["s2_asbuilt"].SetOrdinal(14);
                dtReport.Columns["csa_rfs"].SetOrdinal(15);
                dtReport.Columns["s2_rfs"].SetOrdinal(16);
                dtReport.Columns["HOME_RFS"].SetOrdinal(17);

                dtReport.Columns["s_no"].ColumnName = "Sr No";
                dtReport.Columns["fsa_id"].ColumnName = "FSA ID";
                dtReport.Columns["ring"].ColumnName = "Ring";
                dtReport.Columns["jc_id"].ColumnName = "JC ID";
                dtReport.Columns["town_id"].ColumnName = "Town ID";
                dtReport.Columns["feeder_scope"].ColumnName = "Feeder Scope(km)";
                dtReport.Columns["distribution_scope"].ColumnName = "Dist Scope(km)";
                dtReport.Columns["feeder_asbuilt"].ColumnName = "Feeder As built(km)";
                dtReport.Columns["distribution_asbuilt"].ColumnName = "Dist As built(km)";
                
                dtReport.Columns["s1_scope"].ColumnName = "S1 Scope(Nos)";
                dtReport.Columns["s2_scope"].ColumnName = "S2 Scope(Nos)";
                dtReport.Columns["s1_asbuilt"].ColumnName = "S1 As built(Nos)";
                dtReport.Columns["s2_asbuilt"].ColumnName = "S2 As built(Nos)";
                dtReport.Columns["s2_rfs"].ColumnName = "S2 RFS(Nos)";
                dtReport.Columns["csa_scope"].ColumnName = "CSA Scope(Nos)";
                dtReport.Columns["csa_rfs"].ColumnName = "CSA RFS(Nos)";
                dtReport.Columns["cwip"].ColumnName = "CWIP(Ring)";
                dtReport.Columns["HOME_RFS"].ColumnName = "Home RFS(Nos)";
                //dtReport = Utility.CommonUtility.GetFormattedDataTableOLD(dtReport, ApplicationSettings.numberFormatType);
                //pk
                //foreach (DataRow row in dtReport.Rows)
                //{
                //    // Get the current value in the "s1_scope" column as an integer
                //    int currentValue = Convert.ToInt32(row["s1_scope"]);

                //    // Get the formatted number using the CommonUtility.GetFormattedNumber method
                //    string formattedNumber = Utility.CommonUtility.GetFormattedNumber(currentValue, ApplicationSettings.numberFormatType);

                //    // Update the "s1_scope" column with the formatted number
                //    row["s1_scope"] = formattedNumber;
                //}




            }
            if (ds.Tables.Count > 0)
                ds.Tables[0].TableName = "JFP Export Report FSA Wise";
            var filename = "JFP Export Report FSA Wise";
            ExportData(ds, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"),false,date);
        }
        private void ExportData(DataSet dsReport, string fileName, bool isDataContainBarcode = false,string date="")
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                System.Data.DataTable dtFilters = new System.Data.DataTable();
                if (dsReport != null && dsReport.Tables.Count > 0)
                {
                    IWorkbook workbook = DashBoardDatasetToExcel("xlsx",dsReport, isDataContainBarcode,date);
                    NPOIExcelHelper.DatasetToExcel("xlsx", dsReport, isDataContainBarcode);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        }
        public static IWorkbook DashBoardDatasetToExcel(string extension, DataSet ds, bool isDataContainBarcode = false,string date="")
        {
            IWorkbook workbook;

            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else
            {
                workbook = new HSSFWorkbook();
            }
            foreach (DataTable dt in ds.Tables)
            {
                ISheet sheet1 = workbook.CreateSheet(dt.TableName);
                IRow rowN = sheet1.CreateRow(0);
                ICell cell1 = rowN.CreateCell(1);
                cell1.SetCellValue("Last Updated On:"+ date);
                IRow rowN1 = sheet1.CreateRow(1);
                ICell cell2 = rowN1.CreateCell(1);
                cell2.SetCellValue("JFP- Network & Construction-Export Report FSA Wise");
                IRow row1 = sheet1.CreateRow(2);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns[j].ColumnName.ToString() != "IS_HEADER")
                    {
                        ICell cell = row1.CreateCell(j);
                        String columnName = dt.Columns[j].ToString();
                        cell.SetCellValue(columnName);
                        ICellStyle testeStyle = workbook.CreateCellStyle();
                        testeStyle.FillForegroundColor = IndexedColors.LightGreen.Index;
                        testeStyle.FillPattern = FillPattern.SolidForeground;
                        cell.CellStyle = testeStyle;
                       //Utility.CommonUtility.GetFormattedNumber(Convert.ToInt32(cell.CellStyle), ApplicationSettings.numberFormatType) = testeStyle;
                        //dtReport.Columns["s1_scope"] = Utility.CommonUtility.GetFormattedNumber(Convert.ToInt32(dtReport.Columns["s1_scope"]), ApplicationSettings.numberFormatType);

                    }
                }
                if (isDataContainBarcode)
                {
                    //Setting Last Column Width(Barcode)
                    sheet1.SetColumnWidth(dt.Columns.Count - 1, 70 * 256);
                }
                var picIndex = 0;
                var cellstyle = NPOIExcelHelper.getCellStyle(workbook);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 3);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        if (isDataContainBarcode && j == dt.Columns.Count - 1 && dt.TableName != "Filter Detail")
                        {

                            #region Adding Barcode Image
                            if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                row.Height = 1000;
                                workbook.AddPicture(BarcodeHelper.GenerateBarcode(dt.Rows[i][j].ToString(), true), PictureType.JPEG);
                                IDrawing drawing = sheet1.CreateDrawingPatriarch();
                                IClientAnchor anchor = workbook.GetCreationHelper().CreateClientAnchor();
                                anchor.Dx1 = 0;
                                anchor.Dx2 = 0;
                                anchor.Dy1 = 0;
                                anchor.Dy2 = 0;
                                anchor.Col1 = j;

                                anchor.Row1 = i + 1;
                                IPicture picture = drawing.CreatePicture(anchor, picIndex);
                                picture.Resize();
                                picIndex++;
                            }
                            else
                            {
                                NPOIExcelHelper.CreateCustomCell(row, j, "No Barcode Mapped!", cellstyle);
                            }
                            #endregion
                        }
                        else
                        {
                            ICell cell = row.CreateCell(j);
                            String columnName = dt.Columns[j].ToString();
                            cell.SetCellValue(dt.Rows[i][columnName].ToString());
                            NPOIExcelHelper.CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);
                        }

                    }
                }
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    sheet1.AutoSizeColumn(k, true);
                }

            }
            return workbook;
        }


    }
}