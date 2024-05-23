using System.IO;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System;
using NPOI.SS.Util;
using System.Text;
using ZXing.QrCode;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Web;
using NetTopologySuite.IO;
using System.Globalization;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Models;
using iTextSharp.tool.xml.html.head;
using Microsoft.SqlServer.Server;
using Utility;
using NPOI.SS.Formula.Functions;
using SmartInventory.Settings;

namespace SmartInventory.Helper
{
    public class NPOIExcelHelper
    {

        public static string DatatableToExcelFile(string extention, DataTable dt, string FilePath)
        {
            IWorkbook workbook = DataTableToExcel(extention, dt);

            using (var fs = File.Create(FilePath))
            {
                workbook.Write(fs);
            }

            return FilePath;

        }
        public static DataTable ExcelToTable(string filepath, string longTempColName)
        {
            var table = new DataTable();
            dynamic book = null;
            FileStream excelStream = new FileStream(filepath, FileMode.Open);
            string fileExtn = Path.GetExtension(filepath);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (fileExtn.ToLower() == ".xlsx")
                book = new XSSFWorkbook(excelStream);
            else
                book = new HSSFWorkbook(excelStream);
            excelStream.Close();

            var sheet = book.GetSheetAt(0);
            var headerRow = sheet.GetRow(0);
            if (headerRow == null) return new DataTable();
            var cellCount = headerRow.LastCellNum;

            //var cellCount = headerRow.LastCellNum;
            var rowCount = sheet.LastRowNum;
            //header
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            //body
            for (var i = sheet.FirstRowNum + 1; i <= rowCount; i++)
            {
                var row = sheet.GetRow(i);
                var dataRow = table.NewRow();
                if (row != null)
                {
                    if (row.FirstCellNum > -1)
                    {
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = GetCellValue(row.GetCell(j), fileExtn).Trim();
                        }

                        if (!string.IsNullOrEmpty(longTempColName) && table.Columns.Contains(longTempColName))
                        {
                            string coordinates = Convert.ToString(dataRow[longTempColName]);
                            string outGeometry = string.Empty;
                            if (!string.IsNullOrEmpty(coordinates))
                            {
                                coordinates = coordinates.Replace("  ", "");
                                if (coordinates.Contains("11\n"))
                                {
                                    coordinates = coordinates.Replace("11\n", "");
                                    coordinates = coordinates.Replace("\n", "");
                                    coordinates = coordinates.Replace("\t", "");
                                    coordinates = coordinates.Replace("\r\n", "##");
                                    coordinates = coordinates.Replace("\\", "");
                                    coordinates = coordinates.Replace("\r", "");
                                }
                                if (coordinates.Contains(",0"))
                                {
                                    string Delimiter = ",0";
                                    string[] Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
                                    List<string> lstOutLatLng = new List<string>();

                                    if (Result.Length > 1 && Result[1].ToString() != "")
                                    {
                                        foreach (string latLng in Result)
                                        {
                                            if (!string.IsNullOrEmpty(latLng))
                                            {
                                                lstOutLatLng.Add(latLng.Replace(",", " "));
                                            }
                                        }
                                    }
                                    outGeometry = string.Join(",", lstOutLatLng);
                                    outGeometry = "LINESTRING(" + outGeometry + ")";
                                    dataRow[longTempColName] = outGeometry;
                                }
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                }
            }
            return table;
        }

        internal static void DataTableToSheet(DataTable dtReport, ISheet sheet1)
        {
            IRow headerRow = sheet1.CreateRow(0);
            foreach (DataColumn column in dtReport.Columns)
            {
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
            }

            int rowIndex = 1;
            foreach (DataRow row in dtReport.Rows)
            {
                IRow dataRow = sheet1.CreateRow(rowIndex);
                foreach (DataColumn column in dtReport.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }
                rowIndex++;
            }
        }
        public static DataTable KMLToTable_Old(string filepath, string geomTempColName, string latTempColName, string longTempColName, string entityType)
        {
            var table = new DataTable();

            XmlDocument doc = new XmlDocument();

            using (XmlTextReader tr = new XmlTextReader(filepath))
            {
                tr.Namespaces = false;// (uncomment to ignore namespace)
                doc.Load(tr);  // 'xsi' is an undeclared prefix error here
            }

            XmlElement root = null;
            XmlNodeList elePlacemark = null;
            //try
            // {
            //doc.Load(filepath);

            root = doc.DocumentElement;
            elePlacemark = root.GetElementsByTagName("Placemark");
            //string SQLquery = " Insert into KML_Attributes (user_id,Category,cable_name,Coordinates,uploaded_id) values";
            //string SectionName = "";
            //string coordinates = "";

            foreach (XmlNode node in elePlacemark[0].ChildNodes)
            {
                var attr = node.Attributes["template_column_name"];
                if (attr != null && !string.IsNullOrEmpty(node.Attributes["template_column_name"].Value))
                {
                    var column = new DataColumn(node.Attributes["template_column_name"].Value);
                    table.Columns.Add(column);
                }
                else
                {
                    var column = new DataColumn(node.Name.ToLower());
                    table.Columns.Add(column);
                }
            }
            if (entityType.ToUpper() != Models.EntityType.LandBase.ToString().ToUpper())
            {
                if (!string.IsNullOrEmpty(geomTempColName))
                {
                    table.Columns.Add(geomTempColName);
                }
                else
                {
                    table.Columns.Add(longTempColName);
                    table.Columns.Add(latTempColName);
                }
            }


            for (int i = 0; i <= elePlacemark.Count - 1; i++)
            {
                int j = 0;
                var dataRow = table.NewRow();
                foreach (XmlNode node in elePlacemark[i].ChildNodes)
                {
                    if (node != null)
                    {
                        if (node.InnerText != null)
                        {
                            string coordinates = string.Empty;
                            string outGeometry = string.Empty;
                            if (node.Name.ToLower() == "linestring")
                            {
                                coordinates = node.InnerText;
                                coordinates = coordinates.Replace("  ", "");
                                if (coordinates.Contains("11\n"))
                                {
                                    coordinates = coordinates.Replace("11\n", "");
                                    coordinates = coordinates.Replace("\n", "");
                                    coordinates = coordinates.Replace("\t", "");
                                    coordinates = coordinates.Replace("\r\n", "##");
                                    coordinates = coordinates.Replace("\\", "");
                                    coordinates = coordinates.Replace("\r", "");
                                }
                                if (coordinates.Contains(",0"))
                                {
                                    string Delimiter = ",0";
                                    string[] Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
                                    List<string> lstOutLatLng = new List<string>();

                                    if (Result.Length > 1 && Result[1].ToString() != "")
                                    {
                                        foreach (string latLng in Result)
                                        {
                                            if (!string.IsNullOrEmpty(latLng))
                                            {
                                                lstOutLatLng.Add(latLng.Replace(",", " "));
                                            }
                                        }
                                    }
                                    outGeometry = string.Join(",", lstOutLatLng);
                                    outGeometry = "LINESTRING(" + outGeometry.Trim().TrimEnd(',') + ")";
                                    dataRow[j] = outGeometry;
                                }
                                if (!string.IsNullOrEmpty(coordinates))
                                {
                                    dataRow[geomTempColName] = outGeometry;
                                }
                            }
                            else if (node.Name.ToLower() == "point")
                            {
                                coordinates = node.InnerText;
                                coordinates = coordinates.Replace("  ", "");
                                if (coordinates.Contains("11\n"))
                                {
                                    coordinates = coordinates.Replace("11\n", "");
                                    coordinates = coordinates.Replace("\n", "");
                                    coordinates = coordinates.Replace("\t", "");
                                    coordinates = coordinates.Replace("\r\n", "##");
                                    coordinates = coordinates.Replace("\\", "");
                                    coordinates = coordinates.Replace("\r", "");
                                }
                                if (coordinates.Contains(",0"))
                                {
                                    string Delimiter = ",0";
                                    string[] Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
                                    if (!string.IsNullOrEmpty(Result[0]))
                                    {
                                        dataRow[longTempColName] = Result[0].Split(',')[0];
                                        dataRow[latTempColName] = Result[0].Split(',')[1];
                                    }
                                }
                            }
                            else
                            {
                                dataRow[j] = node.InnerText.Trim();
                            }
                        }
                    }
                    j = j + 1;
                }
                table.Rows.Add(dataRow);
                //SectionName = "";
                //coordinates = "";

                //foreach (XmlNode node in elePlacemark[i].ChildNodes)
                //{
                //    if (node.Name.ToLower() == "name")
                //    {
                //        SectionName = node.InnerText;
                //        break;
                //    }
                //}

                //foreach (XmlNode node in elePlacemark[i].ChildNodes)
                //{
                //    if (node.Name.ToLower() == "linestring")
                //    {
                //        foreach (XmlNode childnode in node.ChildNodes)
                //        {
                //            if (childnode.Name.ToLower() == "coordinates")
                //            {
                //                coordinates = elePlacemark[i].LastChild.LastChild.InnerText.Trim();
                //                coordinates = coordinates.Replace("  ", "");
                //                break;
                //            }
                //        }
                //        break;
                //    }
                //}

                //if (coordinates.Contains("11\n"))
                //{
                //    coordinates = coordinates.Replace("11\n", "");
                //    coordinates = coordinates.Replace("\n", "");
                //    coordinates = coordinates.Replace("\t", "");
                //    coordinates = coordinates.Replace("\r\n", "##");
                //    coordinates = coordinates.Replace("\\", "");
                //    coordinates = coordinates.Replace("\r", "");
                //}
                //if (coordinates.Contains(",0"))
                //{
                //    string Delimiter = ",0";
                //    string[] Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
                //    List<string> lstOutLatLng = new List<string>();
                //    string outGeometry = string.Empty;
                //    if (Result.Length > 1 && Result[1].ToString() != "")
                //    {
                //        foreach (string latLng in Result)
                //        {
                //            if (!string.IsNullOrEmpty(latLng))
                //            {
                //                lstOutLatLng.Add(latLng.Replace(",", " "));
                //            }
                //        }
                //        //coordinates = coordinates.Replace(",0", "$$").Trim();

                //        //SELECT ST_GeomFromText('LINESTRING(-71.160281 42.258729,-71.160837 42.259113,-71.161144 42.25932)');
                //        //LINESTRING(-71.160281 42.258729,-71.160837 42.259113,-71.161144 42.25932)
                //        outGeometry = string.Join(",", lstOutLatLng);
                //        outGeometry = "LINESTRING(" + outGeometry + ")";
                //        //if (coordinates.StartsWith("##"))
                //        //{
                //        //    coordinates = coordinates.Remove(0, 2);
                //        //}

                //        //coordinates = CommonUtility.Wrap(coordinates, 3000, "");
                //        //SQLquery += "(" + summary.user_id + ",'" + summary.entity_type + "','" + SectionName.ToString().Replace("'", "") + "',E'" + coordinates.Trim() + "'," + summary.id + "),";
                //        SQLquery += "(" + summary.user_id + ",'" + summary.entity_type + "','" + SectionName.ToString().Replace("'", "") + "','" + outGeometry + "'," + summary.id + "),";
                //    }
                //    else
                //    {
                //        //error.error_msg = "Placemark No " + (i + 1).ToString() + ") " + SectionName + " : has invalid geometry." + DateTimeHelper.Now.ToShortTimeString();
                //        error.error_msg = "Placemark No " + (i + 1).ToString() + " " + SectionName + " : has invalid geometry!";
                //        error.status = StatusCodes.INVALID_FILE.ToString();
                //        error.is_valid = false;
                //        error.uploaded_by = summary.user_id;
                //        return error;


                //    }
                //}
                //else
                //{
                //    //error.error_msg = "Placemark No " + (i + 1).ToString()+" "  + SectionName + " : has invalid Delimiter." + DateTimeHelper.Now.ToShortTimeString();
                //    error.error_msg = "Placemark No " + (i + 1).ToString() + " " + SectionName + " :" + (string.IsNullOrEmpty(coordinates) ? "has empty geometry!" : "has invalid Delimiter!") + " ";
                //    error.status = StatusCodes.INVALID_FILE.ToString();
                //    error.is_valid = false;
                //    error.uploaded_by = summary.user_id;
                //    return error;

                //}

            }
            return table;
        }
        public static DataTable ExcelToTable(string filepath, out bool isHeaderFound)
        {
            var table = new DataTable();
            isHeaderFound = false;
            dynamic book = null;
            FileStream excelStream = new FileStream(filepath, FileMode.Open);
            string fileExtn = Path.GetExtension(filepath);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (fileExtn.ToLower() == ".xlsx")
                book = new XSSFWorkbook(excelStream);
            else
                book = new HSSFWorkbook(excelStream);
            excelStream.Close();

            var sheet = book.GetSheetAt(0);
            var headerRow = sheet.GetRow(0);
            if (headerRow != null)// To handle blank header
            {
                isHeaderFound = true;

                var cellCount = headerRow.LastCellNum;
                var rowCount = sheet.LastRowNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }
                for (var i = sheet.FirstRowNum + 1; i <= rowCount; i++)
                {
                    var row = sheet.GetRow(i);
                    var dataRow = table.NewRow();
                    if (row != null && row.FirstCellNum >= 0)
                    {
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = GetCellValue(row.GetCell(j), fileExtn).Trim();
                        }
                    }
                    table.Rows.Add(dataRow);
                }
            }
            return table;
        }

        public static DataTable ExcelToTable(string filepath, string sheetName, out bool isHeaderFound)
        {
            var table = new DataTable();
            isHeaderFound = false;
            dynamic book = null;
            FileStream excelStream = new FileStream(filepath, FileMode.Open);
            string fileExtn = Path.GetExtension(filepath);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (fileExtn.ToLower() == ".xlsx")
                book = new XSSFWorkbook(excelStream);
            else
                book = new HSSFWorkbook(excelStream);
            excelStream.Close();

            var sheet = book.GetSheet(sheetName);
            var headerRow = sheet.GetRow(0);
            if (headerRow != null)// To handle blank header
            {
                isHeaderFound = true;

                var cellCount = headerRow.LastCellNum;
                var rowCount = sheet.LastRowNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }
                for (var i = sheet.FirstRowNum + 1; i <= rowCount; i++)
                {
                    var row = sheet.GetRow(i);
                    var dataRow = table.NewRow();
                    if (row != null && row.FirstCellNum >= 0)
                    {
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = GetCellValue(row.GetCell(j), fileExtn).Trim();
                        }
                    }
                    table.Rows.Add(dataRow);
                }
            }
            return table;
        }
        private static string GetCellValue(ICell cell, string fileExtn)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                case CellType.Unknown:
                default:
                    return cell.ToString();//This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        dynamic e = null;
                        if (fileExtn.ToLower() == ".xlsx")
                            e = new XSSFFormulaEvaluator(cell.Sheet.Workbook);
                        else
                            e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }

        public static IWorkbook DataTableToExcel(string extension, DataTable dt)
        {
            // dll refered NPOI.dll and NPOI.OOXML  

            IWorkbook workbook;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else
            {
                workbook = new HSSFWorkbook();
            }
            ISheet sheet1 = workbook.CreateSheet(dt.TableName);
            //make a header row  
            ICellStyle headerStyle = getCellStyle(workbook, "HEADER");
            IRow row1 = sheet1.CreateRow(0);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                CreateCustomCell(row1, j, dt.Columns[j].ToString(), headerStyle);
            }

            //loops through data
            var cellstyle = getCellStyle(workbook);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //String columnName = dt.Columns[j].ToString();
                    CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);

                }
            }
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                sheet1.AutoSizeColumn(k, true);
            }
            return workbook;
        }
        public static IWorkbook DataTableToExcel(string extension, DataTable dt, DataTable header_dt)
        {
            // dll refered NPOI.dll and NPOI.OOXML  

            IWorkbook workbook;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else
            {
                workbook = new HSSFWorkbook();
            }
            ISheet sheet1 = workbook.CreateSheet(dt.TableName);
            //make a header row  
            ICellStyle headerStyle = getCellStyle(workbook, "HEADER");
            var cellstyle = getCellStyle(workbook);

            int finalHeader = 0;
            for (int i = 0; i < header_dt.Columns.Count; i++)
            {
                IRow rows = sheet1.CreateRow(i);
                CreateCustomCell(rows, 0, header_dt.Columns[i].ToString(), headerStyle);
                CreateCustomCell(rows, 1, header_dt.Rows[0][i].ToString(), headerStyle);
                finalHeader = i;
            }


            IRow row1 = sheet1.CreateRow(finalHeader + 2);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                CreateCustomCell(row1, j, dt.Columns[j].ToString(), headerStyle);
            }

            //loops through data

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 4);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns[j].ToString().ToUpper().Contains("COLOR") && dt.Rows[i][j].ToString().Contains("#") && !dt.Rows[i][j].ToString().Contains("##"))
                    {
                        var customcellstyle = getCustomCellStyle(workbook, dt.Rows[i][j].ToString());
                        //String columnName = dt.Columns[j].ToString();
                        CreateCustomCell(row, j, dt.Rows[i][j].ToString(), customcellstyle);
                    }
                    else
                    {
                        CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);
                    }
                }
            }
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                sheet1.AutoSizeColumn(k, true);
            }
            return workbook;
        }
        public static IWorkbook DataTableToExcel(string extension, DataTable dt, bool isDataContainBarcode)
        {
            // dll refered NPOI.dll and NPOI.OOXML  
            IWorkbook workbook;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else
            {
                workbook = new HSSFWorkbook();
            }


            ISheet sheet1 = workbook.CreateSheet(dt.TableName);
            //make a header row  
            ICellStyle headerStyle = getCellStyle(workbook, "HEADER");

            IRow row1 = sheet1.CreateRow(0);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                CreateCustomCell(row1, j, dt.Columns[j].ToString(), headerStyle);
            }

            if (isDataContainBarcode)
            {
                //Setting Last Column Width(Barcode)
                sheet1.SetColumnWidth(dt.Columns.Count - 1, 70 * 256);
            }


            //loops through data
            var picIndex = 0;
            var cellstyle = getCellStyle(workbook);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    if (isDataContainBarcode && j == dt.Columns.Count - 1)
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
                            CreateCustomCell(row, j, "No Barcode Mapped!", cellstyle);
                        }
                        #endregion
                    }
                    else
                    {
                        CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);
                    }
                }
            }
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                if (k != dt.Columns.Count - 1)
                {
                    sheet1.AutoSizeColumn(k, true);
                }
            }

            return workbook;
        }
        public static IWorkbook ROWBudgetDataTableToExcel(string extension, DataTable dt, string roadName, string networkId)
        {
            // dll refered NPOI.dll and NPOI.OOXML  
            IWorkbook workbook;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else
            {
                workbook = new HSSFWorkbook();
            }


            ISheet sheet1 = workbook.CreateSheet(dt.TableName);
            //make a header row  
            ICellStyle headerStyle = getCellStyle(workbook, "HEADER");

            IRow row0 = sheet1.CreateRow(0);
            IRow row1 = sheet1.CreateRow(1);
            IRow row2 = sheet1.CreateRow(2);
            IRow row3 = sheet1.CreateRow(3);
            var cellstyle1 = getCellStyle(workbook);
            CreateCustomCell(row0, 0, "", cellstyle1);
            ICellStyle headerStyle1 = getCellStyle(workbook, "HEADER");
            headerStyle1.FillForegroundColor = IndexedColors.LightYellow.Index;
            headerStyle1.FillPattern = FillPattern.SolidForeground;
            //CreateCustomCell(row1, 0, "   Road Name(" + networkId + ")   ", headerStyle1);
            CreateCustomCell(row1, 0, "   Road Name   ", headerStyle1);
            CreateCustomCell(row1, 1, roadName, headerStyle1);
            CreateCustomCell(row2, 1, "", cellstyle1);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                CreateCustomCell(row3, j, dt.Columns[j].ToString(), headerStyle);
            }
            //loops through data           
            var cellstyle = getCellStyle(workbook);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                IRow row = sheet1.CreateRow(i + 4);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (i == (dt.Rows.Count - 1) && (j == 2 || j == 3))
                    {
                        cellstyle = getCellStyle(workbook);
                        cellstyle.FillForegroundColor = IndexedColors.LightYellow.Index;
                        cellstyle.FillPattern = FillPattern.SolidForeground;
                    }
                    CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);
                }
            }
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                if (k != dt.Columns.Count - 1)
                {
                    sheet1.AutoSizeColumn(k, true);
                }
            }

            return workbook;
        }
        public static IWorkbook uploaderTemplateToExcel(string extension, DataSet ds, string heading1, string heading2)
        {
            IWorkbook workbook;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else
            {
                workbook = new HSSFWorkbook();
            }
            for (int t = 0; t < ds.Tables.Count-1; t++)
            {
                DataTable dt = ds.Tables[t];
                ISheet sheet1 = workbook.CreateSheet(dt.TableName);
                IRow row1 = sheet1.CreateRow(0);
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
                    }
                }
                var cellstyle = getCellStyle(workbook);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        IDataFormat dataFormatCustom = workbook.CreateDataFormat();
                        ICell cell = row.CreateCell(j);
                        String columnName = dt.Columns[j].ToString();
                        cell.SetCellValue(dt.Rows[i][columnName].ToString());


                        CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);
                    }
                }
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    sheet1.AutoSizeColumn(k, true);
                }
            }

            if (ds.Tables[0].TableName == "Cable" && ApplicationSettings.isCDBAttributeEnabled == 1)
            {
                GuideLineCDBAttributesSheet(workbook, ds.Tables[3],ds.Tables[2], heading1, heading2, "                                GUIDELINE FOR FILLING CDB Attributes DETAILS");
                workbook.RemoveSheetAt(2);
            }
            else
            {
                guideLineSheet(workbook, ds.Tables[1], heading1, heading2);
            }
            return workbook;
        }
        public static IWorkbook GuideLineCDBAttributesSheet(IWorkbook workbook, DataTable dt1, DataTable dt2, string heading1, string heading2, string heading3)
        {
            ISheet sheet1 = workbook.CreateSheet(dt1.TableName);

            // Make header row for table 1
            ICellStyle headerStyle = getCellStyle(workbook, "HEADER");
            IRow row1 = sheet1.CreateRow(5);
            IRow row2 = sheet1.CreateRow(6);
            IRow row3 = sheet1.CreateRow(7);
            headerStyle.FillBackgroundColor = IndexedColors.DarkYellow.Index;
            var cellstyle2 = getCellStyle(workbook);
            cellstyle2.FillForegroundColor = IndexedColors.Yellow.Index;
            cellstyle2.FillPattern = FillPattern.SolidForeground;
            cellstyle2.BorderLeft = BorderStyle.None;
            cellstyle2.BorderTop = BorderStyle.None;
            cellstyle2.BorderBottom = BorderStyle.None;
            CreateCustomCell(row1, 1, heading1, cellstyle2);
            var cellstyle3 = getCellStyle(workbook);
            cellstyle3.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            cellstyle3.FillPattern = FillPattern.SolidForeground;
            cellstyle3.BorderLeft = BorderStyle.None;
            cellstyle3.BorderTop = BorderStyle.None;
            IFont myFont = (IFont)workbook.CreateFont();
            myFont.FontHeightInPoints = (short)20;
            myFont.FontName = "ARIAL";
            myFont.IsBold = true;
            cellstyle3.SetFont(myFont);
            CreateCustomCell(row2, 1, heading2, cellstyle3);
            sheet1.AddMergedRegion(new CellRangeAddress(5, 5, 1, 5));
            sheet1.AddMergedRegion(new CellRangeAddress(6, 6, 1, 5));
            ICellStyle headerStyle1 = getCellStyle(workbook, "HEADER");
            headerStyle1.FillForegroundColor = IndexedColors.LightYellow.Index;
            headerStyle1.FillPattern = FillPattern.SolidForeground;
            for (int j = 0; j < dt1.Columns.Count; j++)
            {
                IFont headerFont = (IFont)workbook.CreateFont();
                headerFont.FontHeightInPoints = (short)11;
                headerFont.FontName = "ARIAL";
                headerFont.IsBold = true;

                headerStyle.SetFont(headerFont);
                CreateCustomCell(row3, j + 1, dt1.Columns[j].ToString(), headerStyle);
            }
            var cellstyle = getCellStyle(workbook);
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 8);
                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    if (i == (dt1.Rows.Count - 1) && (j == 2 || j == 3))
                    {
                        cellstyle = getCellStyle(workbook);
                    }
                    if (j == 3 && Convert.ToString(dt1.Rows[i][j]) == "YES")
                    {
                        var coloredCell = getCellStyle(workbook);
                        IFont customFont = (IFont)workbook.CreateFont();
                        customFont.FontHeightInPoints = (short)11;
                        customFont.FontName = "ARIAL";
                        customFont.Color = (short)FontColor.Red;
                        coloredCell.SetFont(customFont);
                        CreateCustomCell(row, j + 1, dt1.Rows[i][j].ToString(), coloredCell);
                    }
                    else
                    {
                        CreateCustomCell(row, j + 1, dt1.Rows[i][j].ToString(), cellstyle);
                    }
                }
            }

            // Make header row for table 2
            ICellStyle headerStyle2 = getCellStyle(workbook, "HEADER");
            headerStyle2.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            headerStyle2.FillPattern = FillPattern.SolidForeground;
            headerStyle2.BorderBottom = BorderStyle.Thin;
            headerStyle2.BottomBorderColor = IndexedColors.Black.Index;
            IFont headerFont2 = workbook.CreateFont();
            headerFont2.FontHeightInPoints = (short)20;
            headerFont2.FontName = "ARIAL";
            headerFont2.IsBold = true; // Set the font to bold for table 2 header
            headerStyle2.SetFont(headerFont2); // Apply the font to the style

            IRow row4 = sheet1.CreateRow(dt1.Rows.Count + 9);
            CreateCustomCell(row4, 1, heading3, headerStyle2); // Use heading3 for table 2
            sheet1.AddMergedRegion(new CellRangeAddress(dt1.Rows.Count + 9, dt1.Rows.Count + 9, 1, dt2.Columns.Count));

            // Add column headers for table 2
            IRow row5 = sheet1.CreateRow(dt1.Rows.Count + 10);
            for (int j = 0; j < dt2.Columns.Count; j++)
            {
                ICell cell = row5.CreateCell(j + 1);
                cell.SetCellValue(dt2.Columns[j].ColumnName);
                cell.CellStyle = headerStyle;
            }

            // Add data for table 2
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(dt1.Rows.Count + i + 11);
                for (int j = 0; j < dt2.Columns.Count; j++)
                {
                    CreateCustomCell(row, j + 1, dt2.Rows[i][j].ToString(), cellstyle);
                }
            }

            // Autofit columns
            for (int k = 1; k <= Math.Max(dt1.Columns.Count, dt2.Columns.Count); k++)
            {
                sheet1.AutoSizeColumn(k, true);
            }

            return workbook;
        }

        public static IWorkbook guideLineSheet(IWorkbook workbook, DataTable dt, string heading1, string heading2)
        {
            ISheet sheet1 = workbook.CreateSheet(dt.TableName);

            //make a header row  
            ICellStyle headerStyle = getCellStyle(workbook, "HEADER");
            IRow row1 = sheet1.CreateRow(5);
            IRow row2 = sheet1.CreateRow(6);
            IRow row3 = sheet1.CreateRow(7);
            headerStyle.FillBackgroundColor = IndexedColors.DarkYellow.Index;
            var cellstyle2 = getCellStyle(workbook);
            cellstyle2.FillForegroundColor = IndexedColors.Yellow.Index;
            cellstyle2.FillPattern = FillPattern.SolidForeground;
            cellstyle2.BorderLeft = BorderStyle.None;
            cellstyle2.BorderTop = BorderStyle.None;
            cellstyle2.BorderBottom = BorderStyle.None;
            CreateCustomCell(row1, 1, heading1, cellstyle2);
            var cellstyle3 = getCellStyle(workbook);
            cellstyle3.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            cellstyle3.FillPattern = FillPattern.SolidForeground;
            cellstyle3.BorderLeft = BorderStyle.None;
            cellstyle3.BorderTop = BorderStyle.None;
            IFont myFont = (IFont)workbook.CreateFont();
            myFont.FontHeightInPoints = (short)20;
            myFont.FontName = "ARIAL";
            myFont.IsBold = true;
            cellstyle3.SetFont(myFont);
            CreateCustomCell(row2, 1, heading2, cellstyle3);
            sheet1.AddMergedRegion(new CellRangeAddress(5, 5, 1, 5));
            sheet1.AddMergedRegion(new CellRangeAddress(6, 6, 1, 5));
            ICellStyle headerStyle1 = getCellStyle(workbook, "HEADER");
            headerStyle1.FillForegroundColor = IndexedColors.LightYellow.Index;
            headerStyle1.FillPattern = FillPattern.SolidForeground;
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                IFont headerFont = (IFont)workbook.CreateFont();
                headerFont.FontHeightInPoints = (short)11;
                headerFont.FontName = "ARIAL";
                headerFont.IsBold = true;

                headerStyle.SetFont(headerFont);
                CreateCustomCell(row3, j + 1, dt.Columns[j].ToString(), headerStyle);
            }


            //loops through data           
            var cellstyle = getCellStyle(workbook);
            for (int i = 0; i < dt.Rows.Count; i++)
            {


                IRow row = sheet1.CreateRow(i + 8);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (i == (dt.Rows.Count - 1) && (j == 2 || j == 3))
                    {
                        cellstyle = getCellStyle(workbook);
                    }
                    if (j == 3 && Convert.ToString(dt.Rows[i][j]) == "YES")
                    {
                        var coloredCell = getCellStyle(workbook);
                        IFont customFont = (IFont)workbook.CreateFont();
                        customFont.FontHeightInPoints = (short)11;
                        customFont.FontName = "ARIAL";
                        customFont.Color = (short)FontColor.Red;
                        coloredCell.SetFont(customFont);
                        CreateCustomCell(row, j + 1, dt.Rows[i][j].ToString(), coloredCell);
                    }
                    else
                    {


                        CreateCustomCell(row, j + 1, dt.Rows[i][j].ToString(), cellstyle);
                    }

                }
            }
            for (int k = 1; k < dt.Columns.Count + 1; k++)
            {

                sheet1.AutoSizeColumn(k, true);

            }



            return workbook;
        }

        public static IWorkbook DatasetToExcel(string extension, DataSet ds, bool isDataContainBarcode = false)
        {
            IWorkbook workbook;
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
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
                LogHelper.GetInstance.WriteDebugLog($"Sheet creation started for : {dt.TableName} on: {DateTime.Now}");
                ISheet sheet1 = workbook.CreateSheet(dt.TableName);
                IRow row1 = sheet1.CreateRow(0);
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
                    }
                }
                if (isDataContainBarcode)
                {
                    //Setting Last Column Width(Barcode)
                    sheet1.SetColumnWidth(dt.Columns.Count - 1, 70 * 256);
                }
                var picIndex = 0;
                var cellstyle = getCellStyle(workbook);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 1);
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
                                CreateCustomCell(row, j, "No Barcode Mapped!", cellstyle);
                            }
                            #endregion
                        }
                        else
                        {
                            ICell cell = row.CreateCell(j);
                            String columnName = dt.Columns[j].ToString();

                            cell.SetCellValue(dt.Rows[i][columnName].ToString());

                            CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);
                        }

                    }
                }
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    sheet1.AutoSizeColumn(k, true);
                }
                LogHelper.GetInstance.WriteDebugLog($"Sheet creation completed for : {dt.TableName} on: {DateTime.Now}");
                LogHelper.GetInstance.WriteDebugLog("\r\n");

            }
            return workbook;
        }

        public static void CreateCustomCell(IRow row, int colIndex, string colText, ICellStyle cellStyle, bool isHeader = false, ICellStyle customStyle = null)
        {
            ICell cell = row.CreateCell(colIndex);

            if (customStyle != null && double.TryParse(colText, out double d))
            {
                cell.SetCellValue(d);
                cell.CellStyle = customStyle;
            }
            else if (customStyle != null && int.TryParse(colText, out int i))
            {
                cell.SetCellValue(i);
                cell.CellStyle = customStyle;
            }
            else
            {
                cell.SetCellValue(colText);
                cell.CellStyle = cellStyle;
            }


        }


        public static void setBordersToMergedCells(IWorkbook workBook, ISheet sheet)
        {
            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                CellRangeAddress mergedRegions = sheet.GetMergedRegion(i);
                RegionUtil.SetBorderTop(1, mergedRegions, sheet, workBook);
                RegionUtil.SetBorderLeft(1, mergedRegions, sheet, workBook);
                RegionUtil.SetBorderRight(1, mergedRegions, sheet, workBook);
                RegionUtil.SetBorderBottom(1, mergedRegions, sheet, workBook);
            }
        }

        public static ICellStyle getCellStyle(IWorkbook workbook, string styleType = "")
        {
            ICellStyle cellStyle = (ICellStyle)workbook.CreateCellStyle();

            // create font style
            IFont myFont = (IFont)workbook.CreateFont();
            myFont.FontHeightInPoints = (short)10;
            myFont.FontName = "ARIAL";

            cellStyle.SetFont(myFont);
            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.Alignment = HorizontalAlignment.Left;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;

            if (styleType.ToUpper() == "HEADER" || styleType.ToUpper() == "FOOTER")
            {

                cellStyle.FillForegroundColor = IndexedColors.LightGreen.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;

            }
            else if (styleType.ToUpper() == "SUB_HEADER")
            {
                cellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;
            }
            return cellStyle;
        }
        public static XSSFCellStyle getCustomCellStyle(IWorkbook workbook, string hexColor)
        {
            XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();

            // create font style
            IFont myFont = (IFont)workbook.CreateFont();
            myFont.FontHeightInPoints = (short)10;
            myFont.FontName = "ARIAL";

            cellStyle.SetFont(myFont);
            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.Alignment = HorizontalAlignment.Left;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;



            byte r = Convert.ToByte(hexColor.Substring(1, 2).ToUpper(), 16);
            byte g = Convert.ToByte(hexColor.Substring(3, 2), 16);
            byte b = Convert.ToByte(hexColor.Substring(5, 2), 16);
            IColor color = new NPOI.XSSF.UserModel.XSSFColor(new byte[] { r, g, b });

            cellStyle.SetFillForegroundColor((XSSFColor)color);
            cellStyle.FillPattern = FillPattern.SolidForeground;
            return cellStyle;
        }
        public static DataTable ShapeToDataTable(string filepath)
        {
            var table = new DataTable();
            try
            {
                //string[] fileEntries = Directory.GetFiles(filepath);
                GeometryFactory factory = new GeometryFactory();
                int shape_file_id = 1;
                using (ShapefileDataReader shapeFileDataReader = new ShapefileDataReader(filepath, factory))
                {
                    ShapefileHeader shpHeader = shapeFileDataReader.ShapeHeader;
                    DbaseFileHeader header = shapeFileDataReader.DbaseHeader;
                    string[] shapeColumns = new string[header.NumFields];
                    for (int i = 0; i < header.NumFields; i++)
                    {
                        table.Columns.Add(header.Fields[i].Name.Trim());
                    }
                    table.Columns.Add("sp_geometry");
                    shapeFileDataReader.Reset();
                    CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;
                    while (shapeFileDataReader.Read())
                    {
                        var dataRow = table.NewRow();
                        for (int i = 0; i < header.NumFields; i++)
                        {
                            dataRow[i] = Convert.ToString(shapeFileDataReader.GetValue(i));

                        }
                        string geometry = ((Geometry)shapeFileDataReader.Geometry).AsText().ToString(CultureInfo.InvariantCulture);
                        if (shpHeader.ShapeType == ShapeGeometryType.Polygon) { }
                        if (shpHeader.ShapeType == ShapeGeometryType.LineString || shpHeader.ShapeType == ShapeGeometryType.LineStringZM)
                        {
                            //geometry = geometry.Substring(0, geometry.Length - 1); geometry = geometry.Substring(12, geometry.Length-12);
                        }
                        if (shpHeader.ShapeType == ShapeGeometryType.Point || shpHeader.ShapeType == ShapeGeometryType.PointZM)
                        {
                            geometry = geometry.Substring(0, geometry.Length - 1);
                            geometry = geometry.Substring(7, geometry.Length - 7);
                            if (table.Columns.Contains("longitude")) { dataRow["longitude"] = geometry.Split(' ')[0]; }
                            if (table.Columns.Contains("latitude")) { dataRow["latitude"] = geometry.Split(' ')[1]; }
                        }
                        dataRow["sp_geometry"] = geometry;
                        table.Rows.Add(dataRow);
                    }
                }
            }
            catch { throw; }
            return table;
        }
        public static IWorkbook AddImageExcel(IWorkbook workbook, Dictionary<string, string> images)
        {

            //add image to workbook sheet 2
            foreach (var item in images)
            {
                ISheet sheet_image = workbook.CreateSheet(item.Key);//Sheet Name
                IRow row_image = sheet_image.CreateRow(0);

                byte[] data = File.ReadAllBytes(item.Value);//Image Path
                int pictureIndex = workbook.AddPicture(data, PictureType.JPEG);
                ICreationHelper helper = workbook.GetCreationHelper();
                IDrawing drawing = sheet_image.CreateDrawingPatriarch();
                IClientAnchor anchor = helper.CreateClientAnchor();
                anchor.Col1 = 1;
                anchor.Row1 = 1;
                IPicture picture = drawing.CreatePicture(anchor, pictureIndex);
                picture.Resize();
                System.IO.File.Delete(item.Value);
            }
            return workbook;
        }

        public static DataTable KMLToTable(string filepath, string geomTempColName, string latTempColName, string longTempColName, string entityType)
        {
            var table = new DataTable();

            XmlDocument doc = new XmlDocument();

            using (XmlTextReader tr = new XmlTextReader(filepath))
            {
                tr.Namespaces = false;// (uncomment to ignore namespace)
                doc.Load(tr);  // 'xsi' is an undeclared prefix error here
            }



            XmlElement root = null;
            XmlNodeList elePlacemark = null;
            //try
            // {
            //doc.Load(filepath);

            root = doc.DocumentElement;
            elePlacemark = root.GetElementsByTagName("Placemark");
            //string SQLquery = " Insert into KML_Attributes (user_id,Category,cable_name,Coordinates,uploaded_id) values";
            //string SectionName = "";
            //string coordinates = "";

            foreach (XmlNode node in elePlacemark[0].ChildNodes)
            {
                if (node.Name.ToUpper() == "EXTENDEDDATA")
                {
                    foreach (XmlNode nodeChild in node.ChildNodes)
                    {
                        // if (nodeChild.Name.ToUpper() == "SCHEMADATA" || node.Name.ToUpper() == "LINESTRING" || node.Name.ToUpper() == "POLYGON" || node.Name.ToUpper() == "POINT")
                        if (nodeChild.Name.ToUpper() == "SCHEMADATA")
                        {
                            foreach (XmlNode nodes in nodeChild)
                            {
                                var column = new DataColumn(nodes.Attributes.GetNamedItem("name").Value.ToLower());
                                table.Columns.Add(column);
                            }
                        }
                        else
                        {
                            var column = new DataColumn(node.Name.ToLower());
                            table.Columns.Add(column);
                        }
                    }
                }
                else if (entityType.ToUpper() == Models.EntityType.LandBase.ToString().ToUpper() && (node.Name.ToUpper() == "LINESTRING" || node.Name.ToUpper() == "POLYGON" || node.Name.ToUpper() == "POINT"))
                {
                    if (!table.Columns.Contains(geomTempColName))
                    {
                        if (!string.IsNullOrEmpty(geomTempColName))
                        {
                            table.Columns.Add(geomTempColName);
                        }
                    }
                    //if (node.Name.ToUpper() == "POINT")
                    //{
                    if (!table.Columns.Contains("latitude"))
                    {
                        table.Columns.Add("latitude");
                    }
                    if (!table.Columns.Contains("longitude"))
                    {
                        table.Columns.Add("longitude");
                    }
                    if (string.IsNullOrEmpty(latTempColName))
                    {
                        latTempColName = "latitude";
                    }
                    if (string.IsNullOrEmpty(longTempColName))
                    {
                        longTempColName = "longitude";
                    }
                    //}
                }
                //else if (node.Name.ToUpper() == "DESCRIPTION")
                //{
                //    foreach (XmlNode nodeChild in node.ChildNodes)
                //   {
                //       if (nodeChild.Name.ToUpper() == "#CDATA-SECTION")
                //       { 
                //            convertStringToDataTable(nodeChild.InnerText);
                ////            var b = nodeChild.InnerText;
                ////            var  a = root.SelectNodes("Document/Folder/Placemark/description/text()");
                ////           var c=  HttpUtility.HtmlDecode(b);
                ////             ExtractText(b);

                //            //            //var queryCDATAXML = from element in doc.DescendantNodes()
                //            //            //                    where element.NodeType == System.Xml.XmlNodeType.CDATA
                //            //            //                    select element.Parent.Value.Trim();



                //            //            foreach (XmlNode nodes in nodeChild)
                //            //            {
                //            //                var column = new DataColumn(nodes.Attributes.GetNamedItem("name").Value.ToLower());
                //            //                table.Columns.Add(column);
                //            //            }
                //        }
                //   }
                //}
            }
            if (entityType.ToUpper() != Models.EntityType.LandBase.ToString().ToUpper())
            {
                if (!string.IsNullOrEmpty(geomTempColName))
                {
                    table.Columns.Add(geomTempColName);
                }
                else
                {
                    if (!table.Columns.Contains(longTempColName))
                    {
                        table.Columns.Add(longTempColName);
                    }
                    if (!table.Columns.Contains(latTempColName))
                    {
                        table.Columns.Add(latTempColName);
                    }
                }

                // for sector entity only
                if (entityType.ToUpper() == Models.EntityType.Sector.ToString().ToUpper())
                {
                    if (string.IsNullOrEmpty(geomTempColName))
                    {
                        table.Columns.Add("sp_geometry");
                        geomTempColName = "sp_geometry";
                    }
                }
            }

            for (int i = 0; i <= elePlacemark.Count - 1; i++)
            {

                var dataRow = table.NewRow();
                table.Rows.Add(dataRow);
                foreach (XmlNode node in elePlacemark[i].ChildNodes)
                {
                    if (node != null)
                    {
                        if (node.Name.ToUpper() == "EXTENDEDDATA" || node.Name.ToUpper() == "LINESTRING" || node.Name.ToUpper() == "POINT" || node.Name.ToUpper() == "POLYGON")
                        {
                            string coordinates = string.Empty;
                            string outGeometry = string.Empty;
                            if (node.Name.ToLower() == "linestring" || (node.Name.ToLower() == "polygon"))
                            {
                                coordinates = node.InnerText;
                                //coordinates = coordinates.Replace("  ", "");
                                if (coordinates.Contains("1\n") || coordinates.Contains("11\n"))
                                {
                                    coordinates = coordinates.Replace("1\n", "");
                                    coordinates = coordinates.Replace("11\n", "");
                                    coordinates = coordinates.Replace("\n", "");
                                    coordinates = coordinates.Replace("\t", "");
                                    coordinates = coordinates.Replace("\r\n", "##");
                                    coordinates = coordinates.Replace("\\", "");
                                    coordinates = coordinates.Replace("\r", "");
                                }
                                //if (coordinates.Contains(",0"))
                                //{
                                string Delimiter = " ";
                                string[] Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
                                List<string> lstOutLatLng = new List<string>();

                                if (Result.Length > 1 && Result[1].ToString() != "")
                                {
                                    foreach (string latLng in Result)
                                    {
                                        //if (!string.IsNullOrEmpty(latLng))
                                        //{
                                        //    lstOutLatLng.Add(latLng.Replace(",", " "));
                                        //}
                                        string[] coords = latLng.Split(',');
                                        if (coords.Length >= 2)
                                        {
                                            lstOutLatLng.Add($"{coords[0]} {coords[1]}");
                                        }
                                    }
                                }

                                outGeometry = string.Join(",", lstOutLatLng);

                                if (node.Name.ToUpper() == "LINESTRING")
                                {
                                    outGeometry = "LINESTRING(" + outGeometry.Trim().TrimEnd(',') + ")";
                                }
                                else if (node.Name.ToUpper() == "POLYGON")
                                {
                                    outGeometry = "POLYGON((" + outGeometry.Trim().TrimEnd(',') + "))";
                                }

                                //dataRow[j] = outGeometry;
                                //}
                                if (!string.IsNullOrEmpty(coordinates))
                                {
                                    dataRow[geomTempColName] = outGeometry;
                                }
                            }
                            else if (node.Name.ToLower() == "point")
                            {
                                XmlNode coordinate = node.SelectSingleNode("coordinates");
                                coordinates = coordinate.InnerText;
                                coordinates = coordinates.Replace("  ", "");
                                if (coordinates.Contains("1\n") || coordinates.Contains("11\n"))
                                {
                                    coordinates = coordinates.Replace("1\n", "");
                                    coordinates = coordinates.Replace("11\n", "");
                                    coordinates = coordinates.Replace("\n", "");
                                    coordinates = coordinates.Replace("\t", "");
                                    coordinates = coordinates.Replace("\r\n", "##");
                                    coordinates = coordinates.Replace("\\", "");
                                    coordinates = coordinates.Replace("\r", "");
                                }
                                if (coordinates.Contains(",0"))
                                {
                                    string Delimiter = ",0";
                                    string[] Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
                                    if (!string.IsNullOrEmpty(Result[0]))
                                    {
                                        dataRow[longTempColName] = Result[0].Split(',').Length >= 1 ? (string.IsNullOrEmpty(Result[0].Split(',')[0]) ? "" : Result[0].Split(',')[0]) : "";
                                        dataRow[latTempColName] = Result[0].Split(',').Length >= 2 ? (string.IsNullOrEmpty(Result[0].Split(',')[1]) ? "" : Result[0].Split(',')[1]) : "";

                                        //dataRow[longTempColName] = Result[0].Split(',').Length >= 1 ? Result[0].Split(',')[0] : "0";
                                        // dataRow[latTempColName] = Result[0].Split(',').Length >= 2 ? Result[0].Split(',')[1] : "0";

                                        if (!string.IsNullOrEmpty(geomTempColName))
                                        {
                                            outGeometry = "POINT(" + dataRow[longTempColName] + " " + dataRow[latTempColName] + ")";
                                            dataRow[geomTempColName] = outGeometry;
                                        }
                                    }
                                }
                            }
                            else if (node.Name.ToUpper() == "EXTENDEDDATA")
                            {
                                //dataRow[j] = node.InnerText.Trim(); 

                                foreach (XmlNode nodeChild in node.ChildNodes)
                                {
                                    if (nodeChild.Name.ToUpper() == "SCHEMADATA")
                                    {
                                        foreach (XmlNode nodes in nodeChild)
                                        {
                                            var column = new DataColumn(nodes.Attributes.GetNamedItem("name").Value.ToLower()).ColumnName;
                                            if (table.Columns.Contains("" + column + ""))
                                            {
                                                table.Rows[i]["" + column + ""] = nodes.InnerText.Trim();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (table.Columns.Contains("linestring"))
            {
                table.Columns.Remove("linestring");
            }
            else if (table.Columns.Contains("point"))
            {
                table.Columns.Remove("point");
            }

            if (entityType.ToUpper() == Models.EntityType.LandBase.ToString().ToUpper())
            {
                if (table.Columns.Contains("latitude"))
                {
                    table.Columns.Remove("latitude");
                }
                if (table.Columns.Contains("longitude"))
                {
                    table.Columns.Remove("longitude");
                }
            }

            return table;
        }

        public static string ExtractText(string html)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            string s = reg.Replace(html, " ");
            s = HttpUtility.HtmlDecode(s);
            for (int i = 0; i < s.Length; i++)
            {

            }
            return s;
        }


        public static DataTable convertStringToDataTable(string data)
        {
            DataTable dataTable = new DataTable();
            bool columnsAdded = false;
            String[] spearator = { "<table" };
            String[] strlist = data.Split(spearator,
               StringSplitOptions.RemoveEmptyEntries);

            foreach (string row in strlist)

            {
                DataRow dataRow = dataTable.NewRow();
                foreach (string cell in row.Split('|'))
                {
                    string[] keyValue = cell.Split('~');
                    if (!columnsAdded)
                    {
                        DataColumn dataColumn = new DataColumn(keyValue[0]);
                        dataTable.Columns.Add(dataColumn);
                    }
                    dataRow[keyValue[0]] = keyValue[1];
                }
                columnsAdded = true;
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
        public static IWorkbook CBOMDataTableToExcel(string extension, DataTable dt, SchedulerLog schedulerlog)
        {
            // dll refered NPOI.dll and NPOI.OOXML  
            ConstructionBomDetailsVM objConstructionBomDetailsVM = new ConstructionBomDetailsVM();
            var modified_date = schedulerlog.action_on;
            objConstructionBomDetailsVM.modified_on = modified_date.HasValue ? modified_date.Value.ToString("dddd, dd MMMM yyyy hh:mm tt") : "";
            IWorkbook workbook;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else
            {
                workbook = new HSSFWorkbook();
            }
            ISheet sheet1 = workbook.CreateSheet(dt.TableName);
            //make a header row  
            ICellStyle headerStyle = getCellStyle(workbook, "HEADER");

            IRow row1 = sheet1.CreateRow(0);

            CreateCustomCell(row1, 0, "Last Updated Design Value on " + objConstructionBomDetailsVM.modified_on, headerStyle);


            IRow row2 = sheet1.CreateRow(1);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                CreateCustomCell(row2, j, dt.Columns[j].ToString(), headerStyle);
            }

            //loops through data
            var cellstyle = getCellStyle(workbook);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 2);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //String columnName = dt.Columns[j].ToString();
                    CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);

                }
            }
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                sheet1.AutoSizeColumn(k, true);
            }
            return workbook;
        }
        public static DataTable ExcelToTableForCDBAttributes(string filepath)
        {
            var table = new DataTable();
            dynamic book = null;
            FileStream excelStream = new FileStream(filepath, FileMode.Open);
            string fileExtn = Path.GetExtension(filepath);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
            if (fileExtn.ToLower() == ".xlsx")
                book = new XSSFWorkbook(excelStream);
            else
                book = new HSSFWorkbook(excelStream);
            excelStream.Close();

            var sheet = book.GetSheetAt(1);
            var headerRow = sheet.GetRow(0);
            if (headerRow == null) return new DataTable();
            var cellCount = headerRow.LastCellNum;
            //var cellCount = headerRow.LastCellNum;
            var rowCount = sheet.LastRowNum;
            //header
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            //body
            for (var i = sheet.FirstRowNum + 1; i <= rowCount; i++)
            {
                var row = sheet.GetRow(i);
                var dataRow = table.NewRow();
                if (row != null)
                {
                    if (row.FirstCellNum > -1)
                    {
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = GetCellValue(row.GetCell(j), fileExtn).Trim();
                        }
                        table.Rows.Add(dataRow);
                    }
                }
            }
            return table;
        }
    }
}