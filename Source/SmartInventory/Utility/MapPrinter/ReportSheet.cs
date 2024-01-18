using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.MapPrinter
{
    public class ReportSheet
    {

        public int Id { get; set; }
        public int Revised_Id { get; set; }
        public string Name { get; set; }
        public MapPoint PixelCenter { get; set; }
        public BoundRect PixelBoundRect { get; set; }
        public MapPoint Index { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public List<MapTile> TilesList { get; set; }

        public ReportSheet()
        {
            TilesList = new List<MapTile>();
        }     
       
    }
    public class SheetBoundRect
    {
        public MapPoint TopLeft { get; set; }
        public MapPoint BottomRight { get; set; }
        public MapPoint NorthEast { get; set; }
        public MapPoint SouthWest { get; set; }
    }
}
