using Models;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Utility;

namespace DailogExportReport.Helper
{
    public class NPOIExcelHelper
    {
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
                    string cellValue = row[column].ToString();
                    dataRow.CreateCell(column.Ordinal).SetCellValue(cellValue);
                }
                rowIndex++;
            }
        }




        //public static IWorkbook DataTableToExcel(string extension, DataTable dt)
        //{
        //    // dll refered NPOI.dll and NPOI.OOXML  

        //    IWorkbook workbook;
        //    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
        //    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
        //    if (extension == "xlsx")
        //    {
        //        workbook = new XSSFWorkbook();
        //    }
        //    else
        //    {
        //        workbook = new HSSFWorkbook();
        //    }
        //    ISheet sheet1 = workbook.CreateSheet(dt.TableName);
        //    //make a header row  
        //    ICellStyle headerStyle = getCellStyle(workbook, "HEADER");
        //    IRow row1 = sheet1.CreateRow(0);
        //    for (int j = 0; j < dt.Columns.Count; j++)
        //    {
        //        CreateCustomCell(row1, j, dt.Columns[j].ToString(), headerStyle);
        //    }

        //    //loops through data
        //    var cellstyle = getCellStyle(workbook);
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        IRow row = sheet1.CreateRow(i + 1);
        //        for (int j = 0; j < dt.Columns.Count; j++)
        //        {
        //            //String columnName = dt.Columns[j].ToString();
        //            CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);

        //        }
        //    }
        //    for (int k = 0; k < dt.Columns.Count; k++)
        //    {
        //        sheet1.AutoSizeColumn(k, true);
        //    }
        //    return workbook;
        //}
        //public static ICellStyle getCellStyle(IWorkbook workbook, string styleType = "")
        //{
        //    ICellStyle cellStyle = (ICellStyle)workbook.CreateCellStyle();

        //    // create font style
        //    IFont myFont = (IFont)workbook.CreateFont();
        //    myFont.FontHeightInPoints = (short)10;
        //    myFont.FontName = "ARIAL";

        //    cellStyle.SetFont(myFont);
        //    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
        //    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
        //    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
        //    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
        //    cellStyle.Alignment = HorizontalAlignment.Left;
        //    cellStyle.VerticalAlignment = VerticalAlignment.Center;

        //    if (styleType.ToUpper() == "HEADER" || styleType.ToUpper() == "FOOTER")
        //    {

        //        cellStyle.FillForegroundColor = IndexedColors.LightGreen.Index;
        //        cellStyle.FillPattern = FillPattern.SolidForeground;

        //    }
        //    else if (styleType.ToUpper() == "SUB_HEADER")
        //    {
        //        cellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
        //        cellStyle.FillPattern = FillPattern.SolidForeground;
        //    }
        //    return cellStyle;
        //}
        //public static void CreateCustomCell(IRow row, int colIndex, string colText, ICellStyle cellStyle, bool isHeader = false, ICellStyle customStyle = null)
        //{
        //    ICell cell = row.CreateCell(colIndex);

        //    if (customStyle != null && double.TryParse(colText, out double d))
        //    {
        //        cell.SetCellValue(d);
        //        cell.CellStyle = customStyle;
        //    }
        //    else if (customStyle != null && int.TryParse(colText, out int i))
        //    {
        //        cell.SetCellValue(i);
        //        cell.CellStyle = customStyle;
        //    }
        //    else
        //    {
        //        cell.SetCellValue(colText);
        //        cell.CellStyle = cellStyle;
        //    }


        //}

    }
}