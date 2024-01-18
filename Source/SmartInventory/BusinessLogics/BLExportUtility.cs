using DataAccess;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLExportUtility
    {


        public static MemoryStream ExportImageToPdf(Rectangle cordinates, string Title, iTextSharp.text.Image img, iTextSharp.text.Image BackImage)
        {
            var t = DAExportUtility.ExportImageToPdf(cordinates, Title, img, BackImage);
            return t;
        }
    }
}
