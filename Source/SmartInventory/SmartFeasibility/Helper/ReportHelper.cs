using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SmartFeasibility.Helper
{
    public class ReportHelper<T> where T : class
    {
        public partial class AllPdfPageEvents : PdfPageEventHelper
        {
            int PageNumber;
            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                PageNumber++;
            }

            public override void OnOpenDocument(PdfWriter writer, Document doc)
            {
                PdfPTable headerTbl = new PdfPTable(2);
                headerTbl.TotalWidth = doc.PageSize.Width - 12f - 12f;
                Image logo = Image.GetInstance(HttpContext.Current.Server.MapPath("~/Content/images/logo_lepton.png"));
                //logo.ScaleToFit(120f, 80f);
                headerTbl.AddCell(new PdfPCell(logo) { Border = 0, BorderWidthBottom = 1, PaddingBottom = 10 });
                Paragraph pDate = new Paragraph(new Chunk("Date: " + Models.DateTimeHelper.Now.ToString("dd'-'MMM'-'yyyy hh:mm tt"), FontFactory.GetFont("ARIAL", 10)));
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

        private static Dictionary<string, string> ObjToDictionary(T obj)
        {
            PropertyInfo[] infos = obj.GetType().GetProperties();
            Dictionary<string, string> dix = new Dictionary<string, string>();

            foreach (PropertyInfo info in infos)
            {
                dix.Add(info.Name, info.GetValue(obj, null).ToString());
            }

            return dix;
        }

        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
            });
        }

        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
            });
        }
        public static PdfPTable Add_Export_Content_To_PDF(PdfPTable tableLayout, DataTable data)
        {
            if (data != null)
            {
                float[] headers = { 20, 20 };
                // float[] headers = { 20, 20, 12, 12, 12, 12, 12 };
                string columnName;
               
                tableLayout.SetWidths(headers);
                tableLayout.WidthPercentage = 80;

                for (int i = 0; i < data.Columns.Count; i++)
                {
                    columnName = data.Columns[i].ColumnName.Replace("_", " ");
                    if (columnName.ToUpper().EndsWith("LENGTH"))
                        columnName += " (m)";
                    AddCellToHeader(tableLayout, Utility.MiscHelper.ToCamelCase(columnName.ToLower()));
                }

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        AddCellToBody(tableLayout, data.Rows[i][j].ToString());
                    }
                }
            }
            return tableLayout;
        }

        public static PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, T data)
        {
            float[] headers = { 50, 50 };
            string columnName;
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 80;
            AddCellToHeader(tableLayout, "Property");
            AddCellToHeader(tableLayout, "Value");

            if (data != null)
            {
                Dictionary<string, string> feasDix = ObjToDictionary(data);
                feasDix.Remove("insideCables");

                foreach (string key in feasDix.Keys)
                {
                    if(key == "Outside_A_Length")
                    {
                        columnName = "Outside Length (A) (m)";
                    }
                    else if (key == "Outside_B_Length")
                    {
                        columnName = "Outside Length (B) (m)";
                    }
                    else if (key == "LMC_A_Length")
                    {
                        columnName = "LMC Length (A) (m)";
                    }
                    else if(key == "LMC_B_Length")
                    {
                        columnName = "LMC Length (B) (m)";
                    }
                    columnName = key.Replace("_", " ");
                    if (columnName.ToUpper().EndsWith("LENGTH"))
                        columnName += " (m)";
                    AddCellToBody(tableLayout, columnName);
                    AddCellToBody(tableLayout, feasDix[key]);
                }
            }
            return tableLayout;
        }

        public static PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, DataTable data)
        {
            if (data != null)
            {
                // float[] headers = { 20, 20 };
                float[] headers = { 20, 20, 12, 12, 12, 12, 12 };
                string columnName;
                foreach(float header in headers)
                {

                }
                tableLayout.SetWidths(headers);
                tableLayout.WidthPercentage = 80;

                for (int i = 0; i < data.Columns.Count; i++)
                {
                    columnName = data.Columns[i].ColumnName.Replace("_", " ");
                    if (columnName.ToUpper().EndsWith("LENGTH"))
                        columnName += " (m)";
                    AddCellToHeader(tableLayout, Utility.MiscHelper.ToCamelCase(columnName.ToLower()));
                }

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        AddCellToBody(tableLayout, data.Rows[i][j].ToString());
                    }
                }
            }
            return tableLayout;
        }

        public static PdfPTable Add_Phrase_To_PDF(PdfPTable tableLayout, String phraseString)
        {
            tableLayout.WidthPercentage = 80;
            tableLayout.DefaultCell.Border = Rectangle.NO_BORDER;
            Font font = new Font(Font.FontFamily.HELVETICA, 12f, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
            Phrase phrase = new Phrase(phraseString, font);

            tableLayout.AddCell(phrase);
            return tableLayout;
        }

        public static DataTable TransposeDataTable(DataTable dt)
        {
            DataTable transpose = new DataTable();
            string columnName;
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    transpose.Columns.Add();
                }
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    transpose.Rows.Add();
                    columnName = dt.Columns[i].ColumnName.Replace("_", " ");
                    if(columnName.ToUpper().EndsWith("LENGTH"))
                    {
                        columnName += " (m)";
                    }
                    transpose.Rows[i][0] = columnName;
                }
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        transpose.Rows[i][j + 1] = dt.Rows[j][i];
                    }
                }
            }
            return transpose;
        }
    }
}