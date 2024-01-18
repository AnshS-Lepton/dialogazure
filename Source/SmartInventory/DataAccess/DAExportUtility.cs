using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataAccess
{
    public class DAExportUtility
    {

        public static MemoryStream ExportImageToPdf(Rectangle cordinates, string Title, iTextSharp.text.Image img, iTextSharp.text.Image Backimg)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                Document pdfDoc = new Document(cordinates, 10, 10, 20, 20);
                              //Resize image depend upon your need
                    //For give the size to image
                Backimg.ScaleToFit(3000, 770);

                    //If you want to choose image as background then,

                Backimg.Alignment = iTextSharp.text.Image.UNDERLYING;

                    //If you want to give absolute/specified fix position to image.
                Backimg.SetAbsolutePosition(7, 69);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, System.Web.HttpContext.Current.Response.OutputStream);

                pdfWriter.PageEvent = new AllPdfPageEvents();

                
                var color = new BaseColor(200, 200, 200);

                pdfDoc.Open();

                img.Border = Rectangle.BOX;
                
                img.BorderColor = color;
                img.BorderWidth = 1f;
                img.Alignment = Element.ALIGN_CENTER;

                pdfDoc.Add(new Paragraph(5, "\u00a0"));


                PdfPTable table = new PdfPTable(1);

                PdfPCell cell = new PdfPCell(new Phrase(Title, FontFactory.GetFont("Verdana", 20)));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border=0;
                table.AddCell(cell);

                pdfDoc.Add(table);  

                pdfDoc.Add(Backimg);
                pdfDoc.Add(img);

                pdfWriter.CloseStream = false;

                pdfDoc.Close();
                return stream;
            }
        }

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
                headerTbl.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                // Image logo = Image.GetInstance(HttpContext.Current.Server.MapPath("~/images/logo_black.png"));
                //logo.ScaleToFit(120f, 80f);
                //headerTbl.AddCell(new PdfPCell(logo) { Border = 0, BorderWidthBottom = 1, PaddingBottom = 10 });
                Paragraph pDate = new Paragraph(new Chunk("Date: " + DateTime.Now.ToString("MM'/'dd'/'yyyy hh:mm tt"), FontFactory.GetFont("ARIAL", 10)));
                headerTbl.AddCell(new PdfPCell(pDate) { Border = 0, BorderWidthBottom = 1, HorizontalAlignment = Element.ALIGN_RIGHT });
                headerTbl.WriteSelectedRows(0, -1, doc.LeftMargin, (doc.PageSize.Height - 10), writer.DirectContent);

            }


            public override void OnEndPage(PdfWriter writer, Document document)
            {

                Paragraph PageNumText = new Paragraph(new Chunk("Page " + PageNumber, FontFactory.GetFont("Verdana", 10)));
                PageNumText.Alignment = Element.ALIGN_RIGHT;
                Paragraph PoweredByText = new Paragraph(new Chunk("Powered by Lepton Software", FontFactory.GetFont("Verdana", 10)));
                PoweredByText.Alignment = Element.ALIGN_RIGHT;
                PdfPTable footerTbl = new PdfPTable(new float[] { 50f, 50f });
                //footerTbl.WidthPercentage = 100;
                footerTbl.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
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
                footerTbl.WriteSelectedRows(0, -1, document.LeftMargin, 30, writer.DirectContent);
            }
        }
    }
}
