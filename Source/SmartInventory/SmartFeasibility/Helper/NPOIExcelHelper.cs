using System.IO;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System;
using NPOI.SS.Util;

namespace SmartFeasibility.Helper
{
    public class NPOIExcelHelper
    {
        public static DataTable ExcelToTable(string filepath)
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
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = GetCellValue(row.GetCell(j), fileExtn).Trim();
                        }
                    }
                    table.Rows.Add(dataRow);
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

        public static IWorkbook DatasetToExcel(string extension, DataSet ds)
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

            foreach (DataTable dt in ds.Tables)
            {
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
            return workbook;
        }

        public static void CreateCustomCell(IRow row, int colIndex, string colText, ICellStyle cellStyle, bool isHeader = false)
        {
            ICell cell = row.CreateCell(colIndex);
            cell.SetCellValue(colText);
            cell.CellStyle = cellStyle;

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

    }
}