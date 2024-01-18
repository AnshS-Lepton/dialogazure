using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using ZXing.QrCode;
using System.Drawing;
using System.IO;
using iTextSharp;
using ZXing;
using iTextSharp.text.pdf;

namespace SmartInventory.Helper
{
    public class BarcodeHelper
    {

        public static byte[] GenerateBarcode(string BarcodeString, bool isPureBarcode)
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 40,
                    //Width = 350, 
                    PureBarcode = isPureBarcode
                }
            };
            Bitmap bm = new Bitmap(writer.Write(BarcodeString));

            using (var stream = new MemoryStream())
            {
                    ResizeBitmap(bm, 0, 40).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return stream.ToArray();
            }
        }


        public static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width == 0 ? bmp.Width : width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, bmp.Width, height);
            }
            return result;
        }


    }

}