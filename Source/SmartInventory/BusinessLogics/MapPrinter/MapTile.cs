using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.MapPrinter
{
    public class MapTile
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public MapPoint PixelCenter { get; set; }
        public BoundRect PixelBoundRect { get; set; }
        public MapPoint Index { get; set; }
        public string GoogleStaticImageUrl
        {
            get
            {
                return string.Format("{0}&center={1},{2}&zoom={3}&size={4}x{5}&maptype={6}&key={7}&scale={8}",
                    StaticMapURL,
                    GeoCenter.X,
                    GeoCenter.Y,
                    MapZoom,
                    Height,
                    Width,
                    MapType,
                    MapKey,
                    Scale);
            }
        }
        public List<string> LocalLayerUrls { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public MapPoint GeoCenter { get; set; }
        public string MapType { get; set; }
        public string StaticMapURL { get; set; }
        public string MapKey { get; set; }
        public int MapZoom { get; set; }
        public BoundRect GeoBoundRect { get; set; }
        public Image ImageData { get; set; }
        public int Scale { get; set; }
        public bool IsTileStartsInSheet { get; set; }
        public BoundRect ScaledPixelBoundRect { get; set; }

     

    }
    public class BoundRect
    {
        public MapPoint TopLeft { get; set; }
        public MapPoint BottomRight { get; set; }
        public MapPoint NorthEast { get; set; }
        public MapPoint SouthWest { get; set; }
    }
}
