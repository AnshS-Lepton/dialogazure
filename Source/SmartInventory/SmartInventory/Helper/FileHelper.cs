using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace SmartInventory.Helper
{
	public class FileHelper
	{
		public static string WriteExcel(DataTable table, string FileName, string path)
		{
			var memoryStream = new MemoryStream();
			using (var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
			{
				IWorkbook workbook = new XSSFWorkbook();
				ISheet excelSheet = workbook.CreateSheet("Sheet1");

				List<String> columns = new List<string>();
				IRow row = excelSheet.CreateRow(0);
				int columnIndex = 0;

				foreach (System.Data.DataColumn column in table.Columns)
				{
					columns.Add(column.ColumnName);
					row.CreateCell(columnIndex).SetCellValue(column.ColumnName);
					columnIndex++;
				}

				int rowIndex = 1;
				foreach (DataRow dsrow in table.Rows)
				{
					row = excelSheet.CreateRow(rowIndex);
					int cellIndex = 0;
					foreach (String col in columns)
					{
						row.CreateCell(cellIndex).SetCellValue(dsrow[col].ToString());
						cellIndex++;
					}

					rowIndex++;
				}
				workbook.Write(fs);
			}
			return FileName;
		}
	}
}
