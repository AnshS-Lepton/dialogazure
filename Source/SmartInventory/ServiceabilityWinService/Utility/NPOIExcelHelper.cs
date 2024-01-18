using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ServiceabilityWinService.Utility
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
        public static void CreateCustomCell(IRow row, int colIndex, string colText, ICellStyle cellStyle, bool isHeader = false)
        {
            ICell cell = row.CreateCell(colIndex);
            cell.SetCellValue(colText);
            cell.CellStyle = cellStyle;

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
