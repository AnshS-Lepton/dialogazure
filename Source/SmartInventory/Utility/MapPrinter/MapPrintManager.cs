using Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using System.Threading;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using NPOI.SS.Formula.Functions;


namespace Utility.MapPrinter
{
    public class MapPrintManager
    {
        const int MAX_ZOOM = 21;
        const int MIN_ZOOM = 5;
        public double TILE_HEIGHT = 640;
        public double TILE_WIDTH = 640;
        public static double GOOGLE_MARK_HEIGHT = 0;//23;
        //public double GOOGLE_MARK_HEIGHT = 23;
        public double ScreenWidth { get; set; }
        public double ScreenHeight { get; set; }
        public double LeftLongitude { get; set; }
        public double RightLongitude { get; set; }
        public double TopLatitude { get; set; }
        public double BottomLatitude { get; set; }
        public double ReportWidth { get; set; }
        public double ReportHeight { get; set; }
        public double LongitudeDelta
        {
            get
            {
                return RightLongitude - LeftLongitude;
            }
        }
        public double BottomLatitudeDegree
        {
            get
            {
                return BottomLatitude * Math.PI / 180;
            }
        }
        public int ZoomLevel { get; set; }
        public List<MapTile> MapTiles { get; set; }
        public int PrintZoomLevel { get; set; }
        public double PrintScreenWidth { get; set; }
        public double PrintScreenHeight { get; set; }
        public string StaticMapURL { get; set; }
        public string MapKey { get; set; }
        public string MapType { get; set; }
        public List<string> LocalLayerUrls { get; set; }
        public bool IsStaticMapEnabled { get; set; }
        //public bool IsScale2Enabled { get; set; }
        public int StaticScaleMap { get; set; }
        public List<MapPoint> SelectedGeoMapArea { get; set; }
        public bool IsStaticMapClipEnabled { get; set; }
        public bool IsLayerMapClipEnabled { get; set; }
        public MapPrintManager(double screenWidth, double screenHeight, double leftLongitude, double rightLongitude, double topLatitude, double bottomLatitude, double reportWidth, double reportHeight, int mapZoom)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            LeftLongitude = leftLongitude;
            RightLongitude = rightLongitude;
            BottomLatitude = bottomLatitude;
            TopLatitude = topLatitude;
            ReportHeight = reportHeight;
            ReportWidth = reportWidth;
            ZoomLevel = mapZoom;
            PrintScreenHeight = ReportHeight;
            PrintScreenWidth = ReportWidth;
        }
        public MapPoint ConvertGeoToPixel(double latitude, double longitude)
        {
            MapPoint result = new MapPoint();
            result.X = (longitude - LeftLongitude) * (PrintScreenWidth / LongitudeDelta);

            latitude = latitude * Math.PI / 180;
            var worldMapWidth = ((PrintScreenWidth / LongitudeDelta) * 360) / (2 * Math.PI);
            var mapOffsetY = (worldMapWidth / 2 * Math.Log((1 + Math.Sin(BottomLatitudeDegree)) / (1 - Math.Sin(BottomLatitudeDegree))));
            result.Y = PrintScreenHeight - ((worldMapWidth / 2 * Math.Log((1 + Math.Sin(latitude)) / (1 - Math.Sin(latitude)))) - mapOffsetY);
            result.Y = result.Y < 0 ? 0 : result.Y;


            return result;
        }
        public MapPoint ConvertPixelToGeo(double tx, double ty)
        {
            MapPoint result = new MapPoint();
            //          /* called worldMapWidth in Raphael's Code, but I think that's the radius since it's the map width or circumference divided by 2*PI  */
            var worldMapRadius = PrintScreenWidth / LongitudeDelta * 360 / (2 * Math.PI);
            var mapOffsetY = (worldMapRadius / 2 * Math.Log((1 + Math.Sin(BottomLatitudeDegree)) / (1 - Math.Sin(BottomLatitudeDegree))));
            var equatorY = PrintScreenHeight + mapOffsetY;
            var a = (equatorY - ty) / worldMapRadius;
            result.X = 180 / Math.PI * (2 * Math.Atan(Math.Exp(a)) - Math.PI / 2);
            result.Y = LeftLongitude + tx / PrintScreenWidth * LongitudeDelta;
            return result;
        }
        public List<MapTile> CreateMapTiles(int printMapZoomLevel)
        {
            List<MapTile> result = new List<MapTile>();
            //TILE_HEIGHT = (StaticScaleMap == 2) ? TILE_HEIGHT * 2 : TILE_HEIGHT;
            //TILE_WIDTH = (StaticScaleMap == 2) ? TILE_WIDTH * 2 : TILE_WIDTH;
            //GOOGLE_MARK_HEIGHT = (StaticScaleMap == 2) ? GOOGLE_MARK_HEIGHT * 2 : GOOGLE_MARK_HEIGHT;
            EntitySize size = GetTilesGridSize(GetScreenSizeByReportSize(new EntitySize { Height = ReportHeight, Width = ReportWidth }, printMapZoomLevel));
            int index = 1;
            double xOffset = TILE_WIDTH / 2;
            double yOffset = TILE_HEIGHT / 2;
            double centerX = xOffset;
            double centerY = yOffset;

            double xOffsetScaled = (StaticScaleMap == 2) ? TILE_WIDTH : TILE_WIDTH / 2;
            double yOffsetScaled = (StaticScaleMap == 2) ? TILE_HEIGHT : TILE_HEIGHT / 2;
            double scaledCenterX = xOffsetScaled;
            double scaledCenterY = yOffsetScaled;
            for (int row = 0; row < size.Height; row++)
            {
                centerX = TILE_WIDTH / 2;
                scaledCenterX = xOffsetScaled;
                for (int col = 0; col < size.Width; col++)
                {
                    result.Add(new MapTile
                    {
                        Id = index,
                        Name = "test",
                        Height = TILE_HEIGHT,//(StaticScaleMap == 2) ? TILE_HEIGHT * 2 : TILE_HEIGHT , //TILE_HEIGHT,
                        Width = TILE_WIDTH,//(StaticScaleMap == 2) ? TILE_WIDTH * 2 : TILE_WIDTH ,//TILE_WIDTH,
                        PixelCenter = new MapPoint { X = centerX, Y = centerY },
                        GeoCenter = ConvertPixelToGeo(centerX, centerY),
                        Index = new MapPoint { X = col, Y = row },
                        StaticMapURL = StaticMapURL,
                        MapKey = MapKey,
                        MapType = MapType,
                        MapZoom = PrintZoomLevel,
                        PixelBoundRect = new BoundRect
                        {
                            TopLeft = new MapPoint { X = centerX - xOffset, Y = centerY - yOffset },
                            BottomRight = new MapPoint { X = centerX + xOffset, Y = centerY + yOffset },
                            NorthEast = new MapPoint { X = centerX + xOffset, Y = centerY - yOffset },
                            SouthWest = new MapPoint { X = centerX - xOffset, Y = centerY + yOffset }
                        },
                        GeoBoundRect = new BoundRect
                        {
                            TopLeft = ConvertPixelToGeo(centerX - xOffset, centerY - yOffset),
                            BottomRight = ConvertPixelToGeo(centerX + xOffset, centerY + yOffset),
                            NorthEast = ConvertPixelToGeo(centerX + xOffset, centerY - yOffset),
                            SouthWest = ConvertPixelToGeo(centerX - xOffset, centerY + yOffset)
                        },
                        Scale = StaticScaleMap,
                        ScaledPixelBoundRect = new BoundRect
                        {
                            TopLeft = new MapPoint { X = scaledCenterX - xOffsetScaled, Y = scaledCenterY - yOffsetScaled },
                            BottomRight = new MapPoint { X = scaledCenterX + xOffsetScaled, Y = scaledCenterY + yOffsetScaled },
                            NorthEast = new MapPoint { X = scaledCenterX + xOffsetScaled, Y = scaledCenterY - yOffsetScaled },
                            SouthWest = new MapPoint { X = scaledCenterX - xOffsetScaled, Y = scaledCenterY + yOffsetScaled }
                        }
                    });
                    index++;

                    //Add offset parameter for override google logo on map
                    centerX += TILE_WIDTH;
                    scaledCenterX += (2 * xOffsetScaled);
                }
                centerY += TILE_HEIGHT - GOOGLE_MARK_HEIGHT;
                scaledCenterY += (2 * yOffsetScaled) - (StaticScaleMap * GOOGLE_MARK_HEIGHT);
            }
            return result;
        }
        public List<ReportSheet> CreateReportSheets(int printMapZoomLevel, MapReport mapReport)
        {
            List<ReportSheet> result = new List<ReportSheet>();
            EntitySize size = GetReportSheetGridSize(GetScreenSizeByReportSize(new EntitySize { Height = ReportHeight, Width = ReportWidth }, printMapZoomLevel), mapReport);
            int index = 1;
            double xOffset = mapReport.MapWidth / 2;
            double yOffset = mapReport.MapHeight / 2;
            double centerX = xOffset;
            double centerY = yOffset;


            double xOffsetScaled = (StaticScaleMap == 2) ? mapReport.MapWidth : mapReport.MapWidth / 2;
            double yOffsetScaled = (StaticScaleMap == 2) ? mapReport.MapHeight : mapReport.MapHeight / 2;
            double scaledCenterX = xOffsetScaled;
            double scaledCenterY = yOffsetScaled;
            for (int row = 0; row < size.Height; row++)
            {
                centerX = mapReport.MapWidth / 2;
                scaledCenterX = xOffsetScaled;
                for (int col = 0; col < size.Width; col++)
                {
                    result.Add(new ReportSheet
                    {
                        Id = index,
                        Name = "test",
                        Height = mapReport.MapHeight,
                        Width = mapReport.MapWidth,
                        PixelCenter = new MapPoint { X = centerX, Y = centerY },
                        Index = new MapPoint { X = col, Y = row },
                        PixelBoundRect = new BoundRect
                        {
                            TopLeft = new MapPoint { X = centerX - xOffset, Y = centerY - yOffset },
                            BottomRight = new MapPoint { X = centerX + xOffset, Y = centerY + yOffset },
                            NorthEast = new MapPoint { X = centerX + xOffset, Y = centerY - yOffset },
                            SouthWest = new MapPoint { X = centerX - xOffset, Y = centerY + yOffset }
                        }

                    });
                    index++;

                    //Add offset parameter for override google logo on map
                    centerX += mapReport.MapWidth;
                    scaledCenterX += (2 * xOffsetScaled);
                }
                centerY += mapReport.MapHeight - GOOGLE_MARK_HEIGHT;
                scaledCenterY += (2 * yOffsetScaled) - (StaticScaleMap * GOOGLE_MARK_HEIGHT);
            }
            return result;

        }
        public double PdfSheetCount(int printMapZoomLevel, MapReport mapReport)
        {
            EntitySize size = GetReportSheetGridSize(GetScreenSizeByReportSize(new EntitySize { Height = ReportHeight, Width = ReportWidth }, printMapZoomLevel), mapReport);

            var pdfCount = size.Height * size.Width;
            return pdfCount;
        }
        public List<ReportSheet> MapSheetWithTiles(List<MapTile> maptiles, List<ReportSheet> reportSheet)
        {
            foreach (var reportitem in reportSheet)
            {
                foreach (var tile in maptiles)
                {
                    if ((tile.PixelBoundRect.TopLeft.X < reportitem.PixelBoundRect.BottomRight.X) && (tile.PixelBoundRect.BottomRight.X > reportitem.PixelBoundRect.TopLeft.X) &&
                        (tile.PixelBoundRect.TopLeft.Y < reportitem.PixelBoundRect.BottomRight.Y) && (tile.PixelBoundRect.BottomRight.Y > reportitem.PixelBoundRect.TopLeft.Y))
                    {
                        reportitem.TilesList.Add(tile);
                    }
                }
            }

            return reportSheet;
        }
        public EntitySize GetTilesGridSize(EntitySize screenSize)
        {
            EntitySize result = new EntitySize();
            //Calculate tile size by zoom level and report size
            result.Height = TILE_HEIGHT;
            result.Width = TILE_WIDTH;

            double row = Math.Ceiling(screenSize.Height / result.Height);
            double col = Math.Ceiling(screenSize.Width / result.Width);
            result.Height = row;
            result.Width = col;
            return result;
        }
        public EntitySize GetReportSheetGridSize(EntitySize screenSize, MapReport mapReport)
        {
            EntitySize result = new EntitySize();
            //Calculate tile size by zoom level and report size
            result.Height = mapReport.MapHeight;
            result.Width = mapReport.MapWidth;

            double row = Math.Ceiling(screenSize.Height / result.Height);
            double col = Math.Ceiling(screenSize.Width / result.Width);
            result.Height = row;
            result.Width = col;
            return result;
        }
        public EntitySize GetScreenSizeByZoom(int zoom)
        {
            EntitySize result = new EntitySize();
            int zoomDiff = zoom - ZoomLevel;

            result.Height = ScreenHeight * Math.Pow(2, zoomDiff);
            result.Width = ScreenWidth * Math.Pow(2, zoomDiff);

            return result;
        }
        public EntitySize GetScreenSizeByReportSize(EntitySize reportSize, int printMapZoomLevel)
        {
            EntitySize result = new EntitySize();

            if ((ReportHeight < ScreenHeight || ReportWidth < ScreenWidth) & printMapZoomLevel == 0)
            {
                result = GetScreenSizeByZoomOut(reportSize);
            }
            else
            {
                result = GetScreenSizeByZoomIn(reportSize, printMapZoomLevel);
            }
            return result;
        }
        public EntitySize GetScreenSizeByZoomIn(EntitySize reportSize, int userZoomLevel)
        {
            EntitySize result = new EntitySize();
            result.Height = ScreenHeight;
            result.Width = ScreenWidth;
            PrintZoomLevel = userZoomLevel == 0 ? ZoomLevel : userZoomLevel;

            if (userZoomLevel == 0)
            {
                result = GetScreenSizeByZoom(PrintZoomLevel);
                double widthDiff = Math.Abs(reportSize.Width - result.Width), heightDiff = Math.Abs(reportSize.Height - result.Height);
                for (int zoom = ZoomLevel + 1; zoom <= MAX_ZOOM; zoom++)
                {

                    //PrintZoomLevel = zoom;
                    if (reportSize.Height > result.Height || reportSize.Width > result.Width)
                    {
                        result = GetScreenSizeByZoom(zoom);
                        if (Math.Abs(reportSize.Width - result.Width) < widthDiff || Math.Abs(reportSize.Height - result.Height) < heightDiff)
                            PrintZoomLevel = zoom;
                        else
                            PrintZoomLevel = zoom - 1;
                    }
                    else break;

                    widthDiff = Math.Abs(reportSize.Width - result.Width);
                    heightDiff = Math.Abs(reportSize.Height - result.Height);
                }
            }
            result = GetScreenSizeByZoom(PrintZoomLevel);
            PrintScreenHeight = result.Height;
            PrintScreenWidth = result.Width;
            return result;
        }
        public EntitySize GetScreenSizeByZoomOut(EntitySize reportSize)
        {
            EntitySize result = new EntitySize();
            result.Height = ScreenHeight;
            result.Width = ScreenWidth;
            PrintZoomLevel = ZoomLevel;


            result = GetScreenSizeByZoom(PrintZoomLevel);
            double widthDiff = Math.Abs(reportSize.Width - result.Width), heightDiff = Math.Abs(reportSize.Height - result.Height);
            for (int zoom = ZoomLevel - 1; zoom >= MIN_ZOOM; zoom--)
            {
                //PrintZoomLevel = zoom;
                if (reportSize.Height < result.Height || reportSize.Width < result.Width)
                {
                    result = GetScreenSizeByZoom(zoom);
                    if (Math.Abs(reportSize.Width - result.Width) < widthDiff || Math.Abs(reportSize.Height - result.Height) < heightDiff)
                        PrintZoomLevel = zoom;
                    else
                        PrintZoomLevel = zoom + 1;
                }
                else break;

                widthDiff = Math.Abs(reportSize.Width - result.Width);
                heightDiff = Math.Abs(reportSize.Height - result.Height);
            }
            result = GetScreenSizeByZoom(PrintZoomLevel);
            PrintScreenHeight = result.Height;
            PrintScreenWidth = result.Width;
            return result;
        }
        public Image ConvertUrlToBitmap(string url, WebProxy proxy = null)
        {
            Image bitmapImage = null;
            // Loop URLs

            try
            {
                WebClient wc = new WebClient();
                // If proxy setting then set
                if (proxy != null)
                    wc.Proxy = proxy;
                if (proxy == null && ConfigurationManager.AppSettings["IsProxyEnable"] != null && ConfigurationManager.AppSettings["IsProxyEnable"].ToString() == "true")
                {
                    if (ConfigurationManager.AppSettings["ProxyUrl"] != null && ConfigurationManager.AppSettings["ProxyUrl"].ToString() != "")
                    {
                        wc.Proxy = ProxyReturn();
                    }
                }
                // Download image
                byte[] bytes = wc.DownloadData(url);
                MemoryStream ms = new MemoryStream(bytes);
                bitmapImage = Image.FromStream(ms);

            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("ConvertUrlToBitmap()", "Utility/MapPrintManager", ex);
                throw new InvalidOperationException(Resources.Resources.SI_GBL_GBL_JQ_FRM_037);
                //Console.Write(ex.Message);
            }

            return bitmapImage;
        }
        //public void StartBrowser(string source,string localFilepath)
        //{
        //    source = @"<div style=""\""height:10px;width:10px;\"">"+ source + "</div>";
        //    var th = new Thread(() =>
        //    {
        //        var webBrowser = new WebBrowser();
        //        webBrowser.ScrollBarsEnabled = false;
        //        webBrowser.IsWebBrowserContextMenuEnabled = true;
        //        webBrowser.AllowNavigation = true;

        //        webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
        //        webBrowser.DocumentText = source;

        //        Application.Run();
        //    });
        //    th.SetApartmentState(ApartmentState.STA);
        //    th.Start();
        //}
        //public void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    var webBrowser = (WebBrowser)sender;
        //    using (Bitmap bitmap =
        //        new Bitmap(
        //            webBrowser.Width,
        //            webBrowser.Height))
        //    {
        //        webBrowser
        //            .DrawToBitmap(
        //            bitmap,
        //            new System.Drawing
        //                .Rectangle(0, 0, bitmap.Width, bitmap.Height));
        //        bitmap.Save(@"D:\Avnish\Spectra\Source_Code\trunck\SmartInventory\SmartInventory\Uploads\Temp\print_remarks.jpg",
        //            System.Drawing.Imaging.ImageFormat.Jpeg);
        //    }

        //}

        public WebProxy ProxyReturn()
        {
            WebProxy wp = null;
            try

            {
                string url = ConfigurationManager.AppSettings["ProxyUrl"].ToString();
                wp = new WebProxy(url, true);
                wp.Credentials = CredentialCache.DefaultCredentials;
                wp.UseDefaultCredentials = true;
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                return wp;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("ConvertUrlToBitmap()", "Utility/MapPrintManager", ex);
            }
            return wp;
        }
        public PrintMapOut MergeImageTiles(List<MapTile> tiles, List<ReportSheet> sheets, iTextSharp.text.Rectangle pageSize, MapReport mapReport, PrintMap printMap, PrintExportLog printLog, bool isFromThreading)
        {

            PrintMapOut objResult = new PrintMapOut();
            try
            {
                var width = (int)ReportWidth;
                var height = (int)ReportHeight;
                // Get max width and height of the image
                int len = tiles.Count;
                if (len > 0)
                {
                    width = (int)tiles[len - 1].PixelBoundRect.BottomRight.X;
                    height = (int)tiles[len - 1].PixelBoundRect.BottomRight.Y;
                }
                if (printMap.isCropByScreen)
                {
                    width = (int)PrintScreenWidth;
                    height = (int)PrintScreenHeight;
                }
                if (printMap.pageScale > 0 && printMap.pageScale != printMap.mapCurrentZoom)
                {
                    width = Convert.ToInt32(mapReport.MapWidth);
                    height = Convert.ToInt32(mapReport.MapHeight);
                }
                if (StaticScaleMap == 2)
                {
                    width *= 2;
                    height *= 2;
                }
                var canvasWidth = width;
                var canvasHeight = height;
                // merge images

                var bitmap = new Bitmap(width, height);
                var bitmapGoogleMap = new Bitmap(width, height);
                var bitmapLayerMap = new Bitmap(width, height);
                // folder name in which files will be saved..
                printMap.printFolderName = printMap.pageSize + "_" + Utility.MiscHelper.getTimeStamp().ToString();
                var totalFileSize = 0;
                if (isValidFTPConnection(printMap.ftpPath, printMap.ftpUserName, printMap.ftpPassword))
                {
                    // Create Directory if not exists and get Final FTP path to save file..
                    printMap.ftpFinalPath = CreateNestedDirectoryOnFTP(printMap.ftpPath, printMap.ftpUserName, printMap.ftpPassword, "PrintMap", printMap.userId.ToString());
                }
                if (printMap.pageScale == 0)
                {
                    GraphicsPath gp = new GraphicsPath();
                    var clippingPoints = GetClippingPoints().ToArray();

                    gp.AddPolygon(clippingPoints);
                    using (var g = Graphics.FromImage(bitmapGoogleMap))
                    {
                        // ParallelProcessTiles(tiles, g);
                        if (IsStaticMapEnabled)
                        {
                            //Crop by selected region
                            if (clippingPoints.Length > 0 && IsStaticMapClipEnabled)
                                g.Clip = new System.Drawing.Region(gp);
                            ParallelProcessTilesImg(tiles);
                            foreach (var tile in tiles)
                            {
                                //tile.ImageData.Save("E:\\temp\\splitMaps\\splitMapsTiles\\bitmapGoogleMap" + Utility.MiscHelper.getTimeStamp() + ".png", ImageFormat.Png);
                                g.DrawImage(tile.ImageData, (float)tile.ScaledPixelBoundRect.TopLeft.X, (float)tile.ScaledPixelBoundRect.TopLeft.Y);
                                tile.ImageData.Dispose();
                            }
                        }
                    }

                    //if (Directory.Exists("E:\\temp\\splitMaps\\"))
                    //    bitmapGoogleMap.Save(string.Format("E:\\temp\\splitMaps\\{0}_GoogleMap.png", Utility.MiscHelper.getTimeStamp()));

                    using (var g = Graphics.FromImage(bitmapLayerMap))
                    {

                        //Crop by selected region
                        if (clippingPoints.Length > 0 && IsLayerMapClipEnabled)
                            g.Clip = new System.Drawing.Region(gp);
                        ParallelProcessTilesLayers(tiles, g, 0, null);
                    }
                    //if (Directory.Exists("E:\\temp\\splitMaps\\"))
                    //    bitmapLayerMap.Save(string.Format("E:\\temp\\splitMaps\\{0}_LayerMap.png", Utility.MiscHelper.getTimeStamp()));
                    using (var g = Graphics.FromImage(bitmap))
                    {

                        //Merge All Images
                        g.DrawImage(bitmapGoogleMap, 0, 0);
                        g.DrawImage(bitmapLayerMap, 0, 0);

                        if (IsStaticMapClipEnabled)
                            g.DrawPolygon(new Pen(Brushes.Gray, 1), clippingPoints);
                        else
                            g.DrawRectangle(new Pen(Brushes.Gray, 1), new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1));
                    }


                    //if (Directory.Exists("E:\\temp\\splitMaps\\"))
                    //    bitmap.Save(string.Format("E:\\temp\\splitMaps\\{0}.png", Utility.MiscHelper.getTimeStamp()));

                    downImagePDF(0, totalFileSize, bitmap, printMap, pageSize, sheets, printLog);
                }
                else
                {
                    var sheetIndex = 0;

                    foreach (var pdfSheet in sheets)
                    {


                        var selectedTiles = pdfSheet.TilesList;

                        var splicedWidth = width;
                        var splicedHeight = height;

                        //if (printMap.pageScale != printMap.mapCurrentZoom)
                        //{
                        //    width = Convert.ToInt32(mapReport.MapWidth);
                        //    height = Convert.ToInt32(mapReport.MapHeight);
                        //}


                        if ((pdfSheet.Width * StaticScaleMap) * pdfSheet.Index.X < (PrintScreenWidth * StaticScaleMap) && ((pdfSheet.Width * StaticScaleMap) * pdfSheet.Index.X) + (pdfSheet.Width * StaticScaleMap) > (PrintScreenWidth * StaticScaleMap))
                        {
                            var unwantedWidth = (((pdfSheet.Width * StaticScaleMap) * pdfSheet.Index.X) + (pdfSheet.Width * StaticScaleMap)) - (PrintScreenWidth * StaticScaleMap);
                            splicedWidth = Convert.ToInt32((pdfSheet.Width * StaticScaleMap) - unwantedWidth);
                        }

                        if ((pdfSheet.Height * StaticScaleMap) * pdfSheet.Index.Y < (PrintScreenHeight * StaticScaleMap) && ((pdfSheet.Height * StaticScaleMap) * pdfSheet.Index.Y) + (pdfSheet.Height * StaticScaleMap) > (PrintScreenHeight * StaticScaleMap))
                        {
                            var unwantedHeight = (((pdfSheet.Height * StaticScaleMap) * pdfSheet.Index.Y) + (pdfSheet.Height * StaticScaleMap)) - (PrintScreenHeight * StaticScaleMap);
                            splicedHeight = Convert.ToInt32((pdfSheet.Height * StaticScaleMap) - unwantedHeight);
                        }
                        if (splicedHeight > (mapReport.MapHeight * StaticScaleMap) - 1)
                        {
                            splicedHeight = Convert.ToInt32((mapReport.MapHeight * StaticScaleMap) - 1);
                        }

                        bitmap = new Bitmap(splicedWidth + 1, splicedHeight + 1);
                        bitmapGoogleMap = new Bitmap(splicedWidth, splicedHeight);
                        bitmapLayerMap = new Bitmap(splicedWidth, splicedHeight);
                        GraphicsPath gp = new GraphicsPath();
                        //var clippingPoints = GetClippingPoints().ToArray();
                        var points = GetSheetClippingPoints(pdfSheet);
                        var clippingPoints = points.ToArray();
                        // var clippingPoints = GetSheetWiseClippingPoints(pdfSheet).ToArray();

                        if (findIsSheetContainsPolygon(points, pdfSheet))
                        {

                            gp.AddPolygon(clippingPoints);
                            using (var g = Graphics.FromImage(bitmapGoogleMap))
                            {
                                if (IsStaticMapEnabled)
                                {
                                    //Crop by selected region
                                    //     if (clippingPoints.Length > 0 && IsStaticMapClipEnabled && printMap.pageScale == 0)
                                    if (clippingPoints.Length > 0 && IsStaticMapClipEnabled)
                                        g.Clip = new System.Drawing.Region(gp);


                                    ParallelProcessTilesImg(selectedTiles);
                                    // ParallelProcessTilesImg(selectedTiles);
                                    foreach (var tile in selectedTiles)
                                    {

                                        //if(printMap.printScale)
                                        tile.ScaledPixelBoundRect.TopLeft.X = (double)(((tile.Width * StaticScaleMap) * tile.Index.X) - ((mapReport.MapWidth * StaticScaleMap) * pdfSheet.Index.X));
                                        tile.ScaledPixelBoundRect.TopLeft.Y = (double)(((tile.Height * StaticScaleMap) * tile.Index.Y) - ((mapReport.MapHeight * StaticScaleMap) * pdfSheet.Index.Y) - (tile.Index.Y * (GOOGLE_MARK_HEIGHT * StaticScaleMap)));

                                        g.DrawImage(tile.ImageData, (float)tile.ScaledPixelBoundRect.TopLeft.X, (float)tile.ScaledPixelBoundRect.TopLeft.Y);
                                        //tile.ImageData.Save(string.Format("E:\\temp\\splitMaps\\splitMapsTiles\\{0}_" + tile.Index.X + tile.Index.Y + "_tile_GoogleMap.png", Utility.MiscHelper.getTimeStamp()));
                                        tile.ImageData.Dispose();
                                        //if (Directory.Exists("E:\\temp\\splitMaps\\"))
                                        //    bitmapGoogleMap.Save(string.Format("E:\\temp\\splitMaps\\{0}_GoogleMap.png", Utility.MiscHelper.getTimeStamp()));
                                    }
                                }
                            }

                            using (var g = Graphics.FromImage(bitmapLayerMap))
                            {
                                //Crop by selected region
                                //  if (clippingPoints.Length > 0 && IsLayerMapClipEnabled && printMap.pageScale == 0)
                                if (clippingPoints.Length > 0 && IsLayerMapClipEnabled)
                                    g.Clip = new System.Drawing.Region(gp);
                                ParallelProcessTilesLayers(selectedTiles, g, sheetIndex, printMap);
                                //if (Directory.Exists("E:\\temp\\splitMaps"))
                                //    bitmapLayerMap.Save(string.Format("E:\\temp\\splitMaps\\{0}_tilesMap.png", Utility.MiscHelper.getTimeStamp()));
                            }

                            using (var g = Graphics.FromImage(bitmap))
                            {
                                //Merge All Images
                                g.DrawImage(bitmapGoogleMap, 0, 0);
                                g.DrawImage(bitmapLayerMap, 0, 0);

                                // if (IsStaticMapClipEnabled && printMap.pageScale == 0)
                                if (IsStaticMapClipEnabled)
                                    g.DrawPolygon(new Pen(Brushes.Gray, 1), clippingPoints);
                                else
                                    g.DrawRectangle(new Pen(Brushes.Gray, 1), new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1));
                            }
                            sheetIndex++;
                            if (Directory.Exists("E:\\temp\\splitMaps\\"))
                                bitmap.Save(string.Format("E:\\temp\\splitMaps\\{0}_GoogleMap.png", Utility.MiscHelper.getTimeStamp()));
                            totalFileSize = downImagePDF(sheetIndex, totalFileSize, bitmap, printMap, pageSize, sheets, printLog);

                        }
                    }

                }
                objResult = UploadFilesToFTP(printMap, pageSize, printLog);
            }
            catch (Exception ex)
            {
                // write error log..
                ErrorLogHelper.WriteErrorLog("MergeImageTiles()", "Utility/MapPrintManager", ex);
                //write log into print_export_log Table
                printLog.export_status = "Failed";
                printLog.error_message = ex.Message ?? "";

                var obj = new BusinessLogics.BLPrintLog().SavePrintLog(printLog);
                if (!isFromThreading) { throw; }
            }

            return objResult;
        }


        private int downImagePDF(int sheetIndex, int totalFileSize, Bitmap bitmap, PrintMap printMap, iTextSharp.text.Rectangle pageSize, List<ReportSheet> sheets, PrintExportLog printLog)
        {
            string pdfFileName = string.Format("{0}.{1}", sheetIndex == 0 ? printMap.printFolderName : sheetIndex.ToString(), fileExtensions.pdf.ToString());
            totalFileSize = totalFileSize + downloadImageToPDF(pdfFileName, bitmap, printMap, pageSize, sheets, sheetIndex);

            var progressPercentage = printMap.pageScale > 0 ? ((sheetIndex * 100) / sheets.Count()) : 99;
            ///// Call SignalR for the refresh of progress 
            SmartInventoryHub smartInventoryhub = SmartInventoryHub.Instance;
            string message = progressPercentage.ToString();
            NotificationOutPut objNotification = new NotificationOutPut();
            objNotification.info = message;
            objNotification.notificationType = notificationType.PrintMap.ToString();
            smartInventoryhub.BroadCastInfo(objNotification);
            smartInventoryhub.BroadCastPrintMapStatus(message);


            var exportProgress = progressPercentage.ToString() + "%";
            printLog.export_progress = exportProgress;
            printLog.file_size = FileSizeFormatter.FormatSize(totalFileSize);
            printLog = new BusinessLogics.BLPrintLog().SavePrintLog(printLog);
            return totalFileSize;
        }

        private PrintMapOut UploadFilesToFTP(PrintMap printMap, iTextSharp.text.Rectangle pageSize, PrintExportLog printLog)
        {

            PrintMapOut objResult = new PrintMapOut();
            // MERGE INTO SINGLE PDF FILE.
            MergeIntoSinglePDF(printMap, pageSize);

            //EXPORT LEGEND IN A SEPERATE PAGE.. TBD
            ExportLegendIntoPDF(printMap);

            if (isValidFTPConnection(printMap.ftpPath, printMap.ftpUserName, printMap.ftpPassword))
            {
                //READ FILE BYTES AND DELTE FROM TEMP FOLDER..
                var localFolderPath = printMap.AttachmentLocalPath + @"\" + printMap.printFolderName;
                if (Directory.GetFiles(localFolderPath).Count() > 1)
                {
                    // ZIP REQUIRED..
                    objResult.fileExtension = "." + fileExtensions.zip.ToString();
                    var zipFilePath = localFolderPath + objResult.fileExtension;
                    using (var zip = new ZipFile())
                    {
                        zip.AddDirectory(localFolderPath);
                        zip.Save(zipFilePath);
                    }
                    if (System.IO.File.Exists(zipFilePath))
                    {
                        Directory.Delete(localFolderPath, true);
                    }
                    objResult.localResultFilePath = zipFilePath;
                    objResult.fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
                }
                else
                {
                    objResult.fileExtension = "." + fileExtensions.pdf.ToString();
                    var pdfFilePath = Directory.GetFiles(localFolderPath).FirstOrDefault();
                    objResult.fileBytes = System.IO.File.ReadAllBytes(pdfFilePath);
                    objResult.localResultFilePath = pdfFilePath;

                }
                //Prepare FTP Request..
                //printMap.ftpFinalPath - complete path with userID
                //printMap.printFolderName - folder name and file name will be same..
                FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(printMap.ftpFinalPath + printMap.printFolderName + objResult.fileExtension);
                ftpReq.Credentials = new NetworkCredential(printMap.ftpUserName.Normalize(), printMap.ftpPassword.Normalize());
                ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                ftpReq.UseBinary = true;
                ftpReq.ContentLength = objResult.fileBytes.Length;
                using (Stream s = ftpReq.GetRequestStream())
                {
                    s.Write(objResult.fileBytes, 0, objResult.fileBytes.Length);
                }
                try
                {
                    ftpReq.GetResponse();

                }
                catch { throw; }
                finally
                {
                    //Delete from local path.. 
                    System.IO.File.Delete(objResult.localResultFilePath);
                }
                printLog.file_path = printMap.ftpFinalPath.Replace(printMap.ftpPath, "") + printMap.printFolderName + objResult.fileExtension;
                printLog.file_extension = objResult.fileExtension;
                printLog.export_progress = "100%";
                printLog.end_on = DateTimeHelper.Now;
                printLog.export_status = "Success";
                var obj = new BusinessLogics.BLPrintLog().SavePrintLog(printLog);
            }
            return objResult;
        }

        private bool MergeIntoSinglePDF(PrintMap printMap, iTextSharp.text.Rectangle pageSize)
        {
            if (printMap.pageScale > 0 && printMap.sheetCount > 1)
            {
                var localFolderPath = printMap.AttachmentLocalPath + @"\" + printMap.printFolderName;
                DirectoryInfo dir = new DirectoryInfo(localFolderPath);
                string[] filePaths = dir.GetFiles().OrderBy(p => p.CreationTime).Select(m => m.FullName).ToArray();
                var mergedFileBytes = MapReport.mergeMultiplePDFs(pageSize, filePaths);
                File.WriteAllBytes(printMap.AttachmentLocalPath + @"\" + printMap.printFolderName + @"\" + printMap.printFolderName + ".pdf", mergedFileBytes);
                // Delete files which we merged above and already included into a single PDF file
                foreach (string filepath in filePaths)
                {
                    File.Delete(filepath);
                }
            }
            return true;
        }

        private bool ExportLegendIntoPDF(PrintMap printMap)
        {
            if (printMap.pageScale > 0 && printMap.printLegend)
            {
                printMap.pageSize = "A4";
                MapReport mapReport = MapReport.GetMapReportContent(printMap);
                List<LegendGroup> legendGroup = new List<LegendGroup>();
                mapReport.LegendColumnSize = 3;
                var allLegends = new BusinessLogics.BLMisc().GetLegendByGeom(printMap.mapSelectedGeom, JsonConvert.SerializeObject(printMap.layerMapFilter), printMap.roleId);
                if (printMap.IsVisiblePrintLegendEntityCount)
                {
                    legendGroup = (from p in allLegends
                                   group p by p.group_name into g
                                   select new LegendGroup { Group = string.Format("{0} ({1})", g.Key, g.ToList().Sum(x => x.entity_count).ToString()), Legends = g.ToList().Select(x => new LegendCell { ImageUrl = x.icon_path, Text = string.Format("{0} ({1})", x.sub_Layer, x.entity_count) }).ToList() }).OrderBy(x => x.Group).ToList();
                }
                else
                {
                    legendGroup = (from p in allLegends
                                   group p by p.group_name into g
                                   select new LegendGroup { Group = string.Format("{0}", g.Key, g.ToList().Sum(x => x.entity_count).ToString()), Legends = g.ToList().Select(x => new LegendCell { ImageUrl = x.icon_path, Text = string.Format("{0}", x.sub_Layer, x.entity_count) }).ToList() }).OrderBy(x => x.Group).ToList();
                }

                var legendBytes = MapReport.GetLegendPDF(legendGroup, mapReport, printMap);
                File.WriteAllBytes(printMap.AttachmentLocalPath + @"\" + printMap.printFolderName + @"\Legend.pdf", legendBytes);
            }

            return true;
        }

        private int downloadImageToPDF(string fileName, Bitmap bitmap, PrintMap printMap, iTextSharp.text.Rectangle pageSize, List<ReportSheet> sheets, int sheetIndex)
        {
            iTextSharp.text.Image finalLayerImg = null;

            finalLayerImg = iTextSharp.text.Image.GetInstance(bitmap, System.Drawing.Imaging.ImageFormat.Png);
            List<LegendGroup> legendGroup = new List<LegendGroup>();
            //printMap.printLegend =printMap.printLegend && printMap.pageScale == 0 ;
            if (printMap.printLegend && printMap.pageScale == 0)
            {
                var allLegends = new BusinessLogics.BLMisc().GetLegendByGeom(printMap.mapSelectedGeom, JsonConvert.SerializeObject(printMap.layerMapFilter), printMap.roleId);

                if (printMap.IsVisiblePrintLegendEntityCount)
                {
                    legendGroup = (from p in allLegends
                                   group p by p.group_name into g
                                   select new LegendGroup { Group = string.Format("{0} {1} ", g.Key, g.ToList().Sum(x => x.entity_count).ToString()), Legends = g.ToList().Select(x => new LegendCell { ImageUrl = x.icon_path, Text = string.Format("{0} ({1})", x.sub_Layer, x.entity_count) }).ToList() }).OrderBy(x => x.Group).ToList();

                }
                else
                {
                    legendGroup = (from p in allLegends
                                   group p by p.group_name into g
                                   select new LegendGroup { Group = string.Format("{0}", g.Key, g.ToList().Sum(x => x.entity_count).ToString()), Legends = g.ToList().Select(x => new LegendCell { ImageUrl = x.icon_path, Text = string.Format("{0}", x.sub_Layer, x.entity_count) }).ToList() }).OrderBy(x => x.Group).ToList();
                }
            }

            //// krishna 20210629
            MapReport mapReport = MapReport.GetMapReportContent(printMap);
            int LegendTblHeight = Convert.ToInt32(GetLegendTableHeight(legendGroup, mapReport));
            if (printMap.pageScale == 0 && LegendTblHeight > mapReport.MapHeight)
            {
                printMap.PdfCellCnt = 1;
            }


            var pdf = MapReport.ExportPdf(pageSize, finalLayerImg, legendGroup, mapReport, printMap, sheets, sheetIndex, PrintZoomLevel, GetMapDistance(printMap));

            var bytes = pdf.ToArray();
            //save file into temp folder..
            SaveFileTemp(printMap, fileName, bytes);
            return bytes.Length;
        }
        public static double GetLegendTableHeight(List<LegendGroup> legendGroupData, MapReport mapReport)
        {
            double TotalHt = 0.0;
            TotalHt += legendGroupData.Count;
            for (int i = 0; i < legendGroupData.Count; i++)
            {
                TotalHt += legendGroupData[i].Legends.Count / (double)(mapReport.LegendColumnSize);
            }
            //TotalHt = (TotalHt * 8)+50; // 1 pixel (X) = 0.26 mm + 50mm (margin)
            TotalHt = TotalHt * 30;
            return Math.Round(TotalHt, 2);
        }
        static bool SaveFileTemp(PrintMap printMap, string newfilename, byte[] fileByte)
        {
            try
            {
                //Save file temporarily on local path..
                string savepath = printMap.AttachmentLocalPath;
                if (Directory.Exists(savepath + @"\" + printMap.printFolderName).Equals(false))
                    Directory.CreateDirectory(savepath + @"\" + printMap.printFolderName);

                File.WriteAllBytes(savepath + @"\" + printMap.printFolderName + @"\" + newfilename, fileByte);
                return true;
            }
            catch { throw; }
        }
        public static class FileSizeFormatter
        {
            // Load all suffixes in an array  
            static readonly string[] suffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB" };
            public static string FormatSize(Int64 bytes)
            {
                int counter = 0;
                decimal number = (decimal)bytes;
                while (Math.Round(number / 1024) >= 1)
                {
                    number = number / 1024;
                    counter++;
                }
                return string.Format("{0:n1} {1}", number, suffixes[counter]);
            }
        }
        private static string CreateNestedDirectoryOnFTP(string strFTPPath, string strUserName, string strPassWord, params string[] directories)
        {
            try
            {
                FtpWebRequest reqFTP;
                string strFTPFilePath = strFTPPath;
                foreach (string directory in directories)
                {
                    if (!string.IsNullOrEmpty(directory) && directory.Trim() != "")
                    {
                        strFTPFilePath += directory + "/";
                        try
                        {
                            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(strFTPFilePath));
                            reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                            reqFTP.UseBinary = true;
                            reqFTP.Credentials = new NetworkCredential(strUserName, strPassWord);
                            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                            Stream ftpStream = response.GetResponseStream();
                            ftpStream.Close();
                            response.Close();
                        }
                        catch (WebException ex)
                        {
                            FtpWebResponse response = (FtpWebResponse)ex.Response;
                            //Directory already exists
                            if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable) { response.Close(); }
                            //Error in creating new directory on FTP..
                            else { throw new Exception("Error in creating directory/sub-directory!", ex); }
                        }
                    }
                }
                return strFTPFilePath;
            }
            catch { throw; }
        }

        private static bool isValidFTPConnection(string ftpUrl, string strUserName, string strPassWord)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(strUserName, strPassWord);
                request.GetResponse();
            }
            catch (WebException ex) { throw new Exception("Unable to connect to FTP Server", ex); }
            return true;
        }

        private double GetMapDistance(PrintMap model)
        {
            //string result = "";
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(model.mapLeftLng, model.mapTopLat, model.mapRightLng, model.mapTopLat);
            // double ratio = distance / model.mapCanvasWidth;
            return distance;
        }
        private void ProcessTiles(List<MapTile> tiles, Graphics g)
        {
            Image mapTileImage = null;
            Image layerMapImage = null;
            foreach (var tile in tiles)
            {

                mapTileImage = ConvertUrlToBitmap(tile.GoogleStaticImageUrl);
                g.DrawImage(mapTileImage, (float)tile.PixelBoundRect.TopLeft.X, (float)tile.PixelBoundRect.TopLeft.Y);
                if (LocalLayerUrls != null && LocalLayerUrls.Count > 0)
                {

                    for (int indx = 0; indx < LocalLayerUrls.Count; indx++)
                    {
                        string bbox = string.Format("BBOX={0},{1},{2},{3}", tile.GeoBoundRect.SouthWest.Y, tile.GeoBoundRect.SouthWest.X, tile.GeoBoundRect.NorthEast.Y, tile.GeoBoundRect.NorthEast.X);
                        LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "BBOX=", "&", true), bbox);
                        LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "HEIGHT=", "&", true), "HEIGHT=" + tile.Height);
                        LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "WIDTH=", "&", true), "WIDTH=" + tile.Width);

                        layerMapImage = ConvertUrlToBitmap(LocalLayerUrls[indx]);
                        //layerMapImage.Save(string.Format("E:\\temp\\splitMaps\\{0}.png",tile.Id));
                        g.DrawImage(layerMapImage, (float)tile.PixelBoundRect.TopLeft.X, (float)tile.PixelBoundRect.TopLeft.Y);
                    }

                }
            }
        }
        private void ParallelProcessTiles(List<MapTile> tiles, Graphics g)
        {
            Image mapTileImage = null;
            Image layerMapImage = null;
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            Parallel.ForEach(tiles, options, (tile) =>
            {
                lock (lockObj)
                {
                    mapTileImage = ConvertUrlToBitmap(tile.GoogleStaticImageUrl);
                    g.DrawImage(mapTileImage, (float)tile.PixelBoundRect.TopLeft.X, (float)tile.PixelBoundRect.TopLeft.Y);
                    if (LocalLayerUrls != null && LocalLayerUrls.Count > 0)
                    {

                        for (int indx = 0; indx < LocalLayerUrls.Count; indx++)
                        {
                            string bbox = string.Format("BBOX={0},{1},{2},{3}", tile.GeoBoundRect.SouthWest.Y, tile.GeoBoundRect.SouthWest.X, tile.GeoBoundRect.NorthEast.Y, tile.GeoBoundRect.NorthEast.X);
                            LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "BBOX=", "&", true), bbox);
                            LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "HEIGHT=", "&", true), "HEIGHT=" + tile.Height);
                            LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "WIDTH=", "&", true), "WIDTH=" + tile.Width);

                            layerMapImage = ConvertUrlToBitmap(LocalLayerUrls[indx]);

                            g.DrawImage(layerMapImage, (float)tile.PixelBoundRect.TopLeft.X, (float)tile.PixelBoundRect.TopLeft.Y);
                        }

                    }
                }
            });
        }

        private void ParallelProcessTilesImg(List<MapTile> tiles)
        {
            //Image layerMapImage = null;
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            Parallel.ForEach(tiles, options, (tile) =>
            {
                lock (lockObj)
                {
                    tile.ImageData = ConvertUrlToBitmap(tile.GoogleStaticImageUrl);

                    //tile.ImageData = ResizeImage(tile.ImageData, (int) tile.Width, (int)tile.Height);
                    /* Bitmap bitmapTile = new Bitmap(tile.ImageData);
                     //Logic for layer image
                     using (Graphics gTile = Graphics.FromImage(bitmapTile))
                     {
                         if (LocalLayerUrls != null && LocalLayerUrls.Count > 0)
                         {

                             for (int indx = 0; indx < LocalLayerUrls.Count; indx++)
                             {
                                 string bbox = string.Format("BBOX={0},{1},{2},{3}", tile.GeoBoundRect.SouthWest.Y, tile.GeoBoundRect.SouthWest.X, tile.GeoBoundRect.NorthEast.Y, tile.GeoBoundRect.NorthEast.X);
                                 LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "BBOX=", "&", true), bbox);
                                 LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "HEIGHT=", "&", true), "HEIGHT=" + tile.Height);
                                 LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "WIDTH=", "&", true), "WIDTH=" + tile.Width);

                                 layerMapImage = ConvertUrlToBitmap(LocalLayerUrls[indx]);
                                 gTile.DrawImage(layerMapImage, 0, 0);

                             }
                         }
                         tile.ImageData = bitmapTile;
                     }*/
                }
            });
        }

        private void ParallelProcessTilesLayers(List<MapTile> tiles, Graphics g, int sheetIndex, PrintMap printMap)
        {



            Image layerMapImage = null;
            var lastTile = tiles[tiles.Count - 1];
            var lastRow = lastTile.Index.Y;

            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            object lockObj = new object();
            Parallel.ForEach(tiles, options, (tile) =>
            {
                lock (lockObj)
                {

                    if (LocalLayerUrls != null && LocalLayerUrls.Count > 0)
                    {

                        //var tileHeight = ((tile.Scale == 2? tile.Scale*2: tile.Scale) * tile.Height) > 4096? 4096: ((tile.Scale == 2 ? tile.Scale * 2 : tile.Scale) * tile.Height);
                        //var tileWidth = ((tile.Scale == 2 ? tile.Scale * 2 : tile.Scale) * tile.Width) > 4096?4096:((tile.Scale == 2 ? tile.Scale * 2 : tile.Scale) * tile.Width);

                        string bbox = "";
                        for (int indx = 0; indx < LocalLayerUrls.Count; indx++)
                        {



                            try
                            {
                                bbox = string.Format("BBOX={0},{1},{2},{3}", tile.GeoBoundRect.SouthWest.Y, tile.GeoBoundRect.SouthWest.X, tile.GeoBoundRect.NorthEast.Y, tile.GeoBoundRect.NorthEast.X);
                                LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "BBOX=", "&", true), bbox);
                                LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "HEIGHT=", "&", true), "HEIGHT=" + (tile.Scale * tile.Height));
                                LocalLayerUrls[indx] = LocalLayerUrls[indx].Replace(commonUtil.getBetween(LocalLayerUrls[indx], "WIDTH=", "&", true), "WIDTH=" + (tile.Scale * tile.Width));
                                //   Appending the given texts




                                layerMapImage = ConvertUrlToBitmap(LocalLayerUrls[indx]);

                                if (tile.Index.Y != lastRow)
                                {
                                    layerMapImage = CropLayerToAvoidOverlapping(layerMapImage);
                                }
                                //if (Directory.Exists("E:\\temp\\splitMaps\\splitMapsLayerTiles\\"))
                                //     layerMapImage.Save(string.Format("E:\\temp\\splitMaps\\splitMapsLayerTiles\\{0}_"+ tile.Index.X + tile.Index.Y + "_LayerTiles.png", Utility.MiscHelper.getTimeStamp()));
                                g.DrawImage(layerMapImage, (float)tile.ScaledPixelBoundRect.TopLeft.X, (float)tile.ScaledPixelBoundRect.TopLeft.Y);
                                //if (Directory.Exists("E:\\temp\\splitMaps\\splitMapsLayerTiles\\"))
                                //    bitmapLayerMap.Save(string.Format("E:\\temp\\splitMaps\\splitMapsLayerTiles\\{0}_LayerTiles.png", Utility.MiscHelper.getTimeStamp()));
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }
                        }
                    }
                }
            });

        }

        public Image CropLayerToAvoidOverlapping(Image currentBitmap)
        {
            Bitmap bmap = (Bitmap)currentBitmap;
            var width = currentBitmap.Width;
            var height = currentBitmap.Height - (int)GOOGLE_MARK_HEIGHT * StaticScaleMap;
            Rectangle rect = new Rectangle(0, 0, width, height);
            return (Image)bmap.Clone(rect, bmap.PixelFormat);
        }


        public Bitmap CropByScreen(Image image)
        {

            var bitmap = new Bitmap((int)PrintScreenWidth, (int)PrintScreenHeight);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image, 0, 0);
                image = null;
            }
            return bitmap;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public List<System.Drawing.Point> GetClippingPoints()
        {
            List<System.Drawing.Point> result = new List<Point>();
            MapPoint tempPoint;
            foreach (var geoPoint in SelectedGeoMapArea)
            {
                tempPoint = ConvertGeoToPixel(geoPoint.X, geoPoint.Y);
                result.Add(new Point { X = StaticScaleMap * (int)tempPoint.X, Y = StaticScaleMap * (int)tempPoint.Y });
            }
            return result;
        }


        public List<System.Drawing.Point> GetSheetClippingPoints(ReportSheet pdfSheet)
        {
            List<System.Drawing.Point> result = new List<Point>();
            MapPoint tempPoint;
            foreach (var geoPoint in SelectedGeoMapArea)
            {
                tempPoint = ConvertGeoToPixel(geoPoint.X, geoPoint.Y);

                tempPoint.X = (StaticScaleMap * (int)tempPoint.X) - (((int)pdfSheet.Index.X) * pdfSheet.Width);
                tempPoint.Y = (StaticScaleMap * (int)tempPoint.Y) - (((int)pdfSheet.Index.Y) * pdfSheet.Height);

                result.Add(new Point { X = (int)tempPoint.X, Y = (int)tempPoint.Y });
            }
            return result;
        }



        public bool findIsSheetContainsPolygon(List<Point> clippingPoints, ReportSheet pdfSheet)
        {
            List<System.Drawing.Point> result = new List<Point>();
            try
            {
                MapPoint tempPoint, currentPoint;
                var previousPoint = new MapPoint { X = 0, Y = 0 };
                List<MapPoint> sheetCoordinates = new List<MapPoint>();

                sheetCoordinates.Add(new MapPoint { X = pdfSheet.PixelBoundRect.TopLeft.X, Y = pdfSheet.PixelBoundRect.TopLeft.Y, });
                sheetCoordinates.Add(new MapPoint { X = pdfSheet.PixelBoundRect.SouthWest.X, Y = pdfSheet.PixelBoundRect.SouthWest.Y, });
                sheetCoordinates.Add(new MapPoint { X = pdfSheet.PixelBoundRect.BottomRight.X, Y = pdfSheet.PixelBoundRect.BottomRight.Y, });
                sheetCoordinates.Add(new MapPoint { X = pdfSheet.PixelBoundRect.NorthEast.X, Y = pdfSheet.PixelBoundRect.NorthEast.Y, });
                var allCoordinates = GetClippingPointsofPolygon();
                Dictionary<string, Point> final_points = new Dictionary<string, Point>();
                var intersection = false;
                var isEnitrePolygonInside = true;
                foreach (var geoPoint in allCoordinates)
                {
                    tempPoint = new MapPoint { X = geoPoint.X, Y = geoPoint.Y };


                    intersection = polygonSheetIntersection(previousPoint, tempPoint, sheetCoordinates);

                    if (intersection)
                    {
                        break;
                    }

                    previousPoint = tempPoint;
                }


                var sheetCornerPoint = new List<Point>();
                var IsSheetCornerInside = false;
                foreach (var item in sheetCoordinates)
                {
                    var isInside = InsidePolygon(allCoordinates, allCoordinates.Count, item);
                    if (isInside)
                    {
                        IsSheetCornerInside = true;
                        break;
                    }
                }
                if (intersection || IsSheetCornerInside)
                {
                    return true;
                }
                else
                {
                    //The whole polygon is inside the sheet
                    foreach (var geoPoint in allCoordinates)
                    {

                        if (!((geoPoint.X >= pdfSheet.PixelBoundRect.TopLeft.X && geoPoint.Y >= pdfSheet.PixelBoundRect.TopLeft.Y) && (geoPoint.X <= pdfSheet.PixelBoundRect.BottomRight.X && geoPoint.Y <= pdfSheet.PixelBoundRect.BottomRight.Y)))
                        {
                            isEnitrePolygonInside = false;
                        }
                    }

                    if (isEnitrePolygonInside)
                    {
                        return true;
                    }
                }


                return false;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;

        }

        public List<MapPoint> GetClippingPointsofPolygon()
        {
            List<MapPoint> result = new List<MapPoint>();
            MapPoint tempPoint;
            foreach (var geoPoint in SelectedGeoMapArea)
            {
                tempPoint = ConvertGeoToPixel(geoPoint.X, geoPoint.Y);
                result.Add(new MapPoint { X = StaticScaleMap * (int)tempPoint.X, Y = StaticScaleMap * (int)tempPoint.Y });
            }
            return result;
        }

        public static bool polygonSheetIntersection(MapPoint A, MapPoint B, List<MapPoint> sheetCoordinates)
        {
            List<MapPoint> intersectingPoints = new List<MapPoint>();

            bool checkXIsSmaller = false;
            bool checkYIsSmaller = false;
            bool IsXPosition = false;
            bool IsYPosition = false;
            if (B.X < A.X)
            {
                checkXIsSmaller = true;
            }

            if (B.Y < A.Y)
            {
                checkYIsSmaller = true;
            }


            for (int i = 0; i < sheetCoordinates.Count; i++)
            {
                var C = sheetCoordinates[i];

                var D = sheetCoordinates.Count == i + 1 ? sheetCoordinates[0] : sheetCoordinates[i + 1];

                // Line AB represented as a1x + b1y = c1 
                double a1 = B.Y - A.Y;
                double b1 = A.X - B.X;
                double c1 = a1 * (A.X) + b1 * (A.Y);

                // Line CD represented as a2x + b2y = c2 
                double a2 = D.Y - C.Y;
                double b2 = C.X - D.X;
                double c2 = a2 * (C.X) + b2 * (C.Y);

                double determinant = a1 * b2 - a2 * b1;
                double x = 0;
                double y = 0;
                if (determinant == 0)
                {
                    // The lines are parallel. This is simplified 
                    // by returning a pair of FLT_MAX 
                    //return new Point(double.MaxValue, double.MaxValue);
                }
                else
                {
                    x = (b2 * c1 - b1 * c2) / determinant;
                    y = (a1 * c2 - a2 * c1) / determinant;
                }
                if (x >= 0 && y >= 0)
                {




                    if (checkXIsSmaller)
                    {
                        IsXPosition = x < A.X && x > B.X;
                    }
                    else
                    {
                        IsXPosition = x > A.X && x < B.X;
                    }


                    if (checkYIsSmaller)
                    {
                        IsYPosition = y < A.Y && y > B.Y;
                    }
                    else
                    {
                        IsYPosition = y > A.Y && y < B.Y;
                    }

                    //var isInside =   InsidePolygon(sheetCoordinates, sheetCoordinates.Count, new MapPoint { X = Convert.ToInt32(x), Y = Convert.ToInt32(y) });

                    var isInside = findInsideSquare(sheetCoordinates[0], sheetCoordinates[2], new MapPoint { X = Convert.ToInt32(x), Y = Convert.ToInt32(y) });

                    if (IsXPosition && IsYPosition && isInside)
                    {
                        return true;
                    }
                    else if ((C.X == A.X && C.Y == A.Y) ||
                       (D.X == A.X && D.Y == A.Y) || (C.X == B.X && C.Y == B.Y) ||
                       (D.X == B.X && D.Y == B.Y))
                    {
                        return true;
                    }

                }
            }


            //foreach (var item in intersectingPoints)
            //{
            //    if (checkXIsSmaller)
            //    {
            //        intersectingPoints.Sort((a, b) => { return b.X.CompareTo(a.X); }); 
            //    }
            //    else
            //    {
            //        intersectingPoints.Sort((a, b) => { return a.X.CompareTo(b.X); }); 
            //    }

            //}

            return false;
        }

        public static bool findInsideSquare(MapPoint A, MapPoint B, MapPoint intersectionPoint)
        {
            if (intersectionPoint.X >= A.X && intersectionPoint.X <= B.X &&
                intersectionPoint.Y >= A.Y && intersectionPoint.Y <= B.Y)
                return true;

            return false;
        }

        //public List<System.Drawing.Point> GetSheetWiseClippingPoints(ReportSheet pdfSheet)
        //{
        //    List<System.Drawing.Point> result = new List<Point>();
        //    try
        //    {
        //        MapPoint tempPoint, currentPoint;
        //        var previousPoint = new MapPoint { X = 0, Y = 0 };
        //        List<MapPoint> sheetCoordinates = new List<MapPoint>();

        //        sheetCoordinates.Add(new MapPoint { X = pdfSheet.PixelBoundRect.TopLeft.X, Y = pdfSheet.PixelBoundRect.TopLeft.Y, });
        //        sheetCoordinates.Add(new MapPoint { X = pdfSheet.PixelBoundRect.SouthWest.X, Y = pdfSheet.PixelBoundRect.SouthWest.Y, });
        //        sheetCoordinates.Add(new MapPoint { X = pdfSheet.PixelBoundRect.BottomRight.X, Y = pdfSheet.PixelBoundRect.BottomRight.Y, });
        //        sheetCoordinates.Add(new MapPoint { X = pdfSheet.PixelBoundRect.NorthEast.X, Y = pdfSheet.PixelBoundRect.NorthEast.Y, });
        //        var allCoordinates = GetClippingPointsofPolygon();
        //        Dictionary<string, Point> final_points = new Dictionary<string, Point>();
        //        foreach (var geoPoint in SelectedGeoMapArea)
        //        {
        //            tempPoint = ConvertGeoToPixel(geoPoint.X, geoPoint.Y);
        //            //currentPoint = tempPoint;

        //            if (!((tempPoint.X >= pdfSheet.PixelBoundRect.TopLeft.X && tempPoint.Y >= pdfSheet.PixelBoundRect.TopLeft.Y) && (tempPoint.X <= pdfSheet.PixelBoundRect.BottomRight.X && tempPoint.Y <= pdfSheet.PixelBoundRect.BottomRight.Y)) && !((previousPoint.X >= pdfSheet.PixelBoundRect.TopLeft.X && previousPoint.Y >= pdfSheet.PixelBoundRect.TopLeft.Y) && (previousPoint.X <= pdfSheet.PixelBoundRect.BottomRight.X && previousPoint.Y <= pdfSheet.PixelBoundRect.BottomRight.Y))) // if both the points lies outside
        //            {
        //                //tempPoint = polygonSheetIntersection(previousPoint, tempPoint, sheetCoordinates);
        //                //final_points.Add("I_" + tempPoint.X + tempPoint.Y, new Point { X = (StaticScaleMap * (int)tempPoint.X) - (int)pdfSheet.PixelBoundRect.TopLeft.X, Y = (StaticScaleMap * (int)tempPoint.Y) - (int)pdfSheet.PixelBoundRect.TopLeft.Y });
        //                var intersection = polygonSheetIntersection(previousPoint, tempPoint, sheetCoordinates);
        //                addPointsToArray(StaticScaleMap, intersection, pdfSheet, final_points);
        //            }
        //            else if (((tempPoint.X >= pdfSheet.PixelBoundRect.TopLeft.X && tempPoint.Y >= pdfSheet.PixelBoundRect.TopLeft.Y) && (tempPoint.X <= pdfSheet.PixelBoundRect.BottomRight.X && tempPoint.Y <= pdfSheet.PixelBoundRect.BottomRight.Y)) && !((previousPoint.X >= pdfSheet.PixelBoundRect.TopLeft.X && previousPoint.Y >= pdfSheet.PixelBoundRect.TopLeft.Y) && (previousPoint.X <= pdfSheet.PixelBoundRect.BottomRight.X && previousPoint.Y <= pdfSheet.PixelBoundRect.BottomRight.Y))) // if the point lies inside and the previous point lies outside
        //            {

        //                //tempPoint = polygonSheetIntersection(previousPoint, tempPoint, sheetCoordinates);
        //                //final_points.Add("I_" + tempPoint.X + tempPoint.Y, new Point { X = (StaticScaleMap * (int)tempPoint.X) - (int)pdfSheet.PixelBoundRect.TopLeft.X, Y = (StaticScaleMap * (int)tempPoint.Y) - (int)pdfSheet.PixelBoundRect.TopLeft.Y });
        //                var intersection = polygonSheetIntersection(previousPoint, tempPoint, sheetCoordinates);
        //                addPointsToArray(StaticScaleMap, intersection, pdfSheet, final_points);

        //                //tempPoint = currentPoint;

        //                var x = (StaticScaleMap * (int)tempPoint.X) - (int)pdfSheet.PixelBoundRect.TopLeft.X;
        //                var y = (StaticScaleMap * (int)tempPoint.Y) - (int)pdfSheet.PixelBoundRect.TopLeft.Y;
        //                var firstPoint = final_points.Values.ElementAt(0);
        //                if (!(firstPoint.X == x && firstPoint.Y == y))
        //                {
        //                    final_points.Add("P_" + tempPoint.X + tempPoint.Y, new Point { X = x, Y = y });
        //                }
        //            }
        //            else if ((tempPoint.X >= pdfSheet.PixelBoundRect.TopLeft.X && tempPoint.Y >= pdfSheet.PixelBoundRect.TopLeft.Y) && (tempPoint.X <= pdfSheet.PixelBoundRect.BottomRight.X && tempPoint.Y <= pdfSheet.PixelBoundRect.BottomRight.Y) && (final_points.Count > 0 ? ((final_points.Values.ElementAt(0).X != tempPoint.X) && (final_points.Values.ElementAt(0).Y != tempPoint.Y)) : true))// if the point lies inside the sheet and does not repeat again
        //            {
        //                final_points.Add("P_" + tempPoint.X + tempPoint.Y, new Point { X = (StaticScaleMap * (int)tempPoint.X) - (int)pdfSheet.PixelBoundRect.TopLeft.X, Y = (StaticScaleMap * (int)tempPoint.Y) - (int)pdfSheet.PixelBoundRect.TopLeft.Y });
        //            }
        //            else
        //            {
        //                if (final_points.Count > 0)// to check that we have the starting point are not
        //                {
        //                    if (((previousPoint.X >= pdfSheet.PixelBoundRect.TopLeft.X && previousPoint.Y >= pdfSheet.PixelBoundRect.TopLeft.Y) && (previousPoint.X <= pdfSheet.PixelBoundRect.BottomRight.X && previousPoint.Y <= pdfSheet.PixelBoundRect.BottomRight.Y)) || ((final_points.Values.ElementAt(0).X == tempPoint.X) && (final_points.Values.ElementAt(0).Y == tempPoint.Y))) // check that previous points lies inside the Sheet or not
        //                    {

        //                        var intersection = polygonSheetIntersection(previousPoint, tempPoint, sheetCoordinates);

        //                        addPointsToArray(StaticScaleMap, intersection, pdfSheet, final_points);

        //                        //final_points.Add("I_" + tempPoint.X + tempPoint.Y, new Point { X = (StaticScaleMap * (int)tempPoint.X) - (int)pdfSheet.PixelBoundRect.TopLeft.X, Y = (StaticScaleMap * (int)tempPoint.Y) - (int)pdfSheet.PixelBoundRect.TopLeft.Y });
        //                        //tempPoint = currentPoint;
        //                    }
        //                }

        //            }



        //            previousPoint.X = tempPoint.X;
        //            previousPoint.Y = tempPoint.Y;

        //        }
        //        var sheetCornerPoint = new List<Point>();

        //        foreach (var item in sheetCoordinates)
        //        {
        //            var isInside = InsidePolygon(allCoordinates, allCoordinates.Count, item);
        //            if (isInside)
        //            {
        //                var point = new Point { X = (int)item.X - (int)pdfSheet.PixelBoundRect.TopLeft.X, Y = (int)item.Y - (int)pdfSheet.PixelBoundRect.TopLeft.Y };
        //                sheetCornerPoint.Add(point);
        //            }
        //        }
        //        result = PointShorting(final_points, sheetCornerPoint);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return result;

        //}

        //public void addPointsToArray(int StaticScaleMap, List<MapPoint> temp, ReportSheet pdfSheet, Dictionary<string, Point> final_points)
        //{
        //    foreach (var tempPoint in temp)
        //    {
        //        var x = (StaticScaleMap * (int)tempPoint.X) - (int)pdfSheet.PixelBoundRect.TopLeft.X;
        //        var y = (StaticScaleMap * (int)tempPoint.Y) - (int)pdfSheet.PixelBoundRect.TopLeft.Y;

        //        if (x >= 0 & y >= 0)
        //        {
        //            final_points.Add("I_" + tempPoint.X + tempPoint.Y, new Point { X = x, Y = y });
        //        }
        //    }
        //}

        public List<Point> PointShorting(Dictionary<string, Point> result, List<Point> sheetCornerPoint)
        {
            List<Point> shortedPoints = new List<Point>();
            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    var key = result.Keys.ElementAt(i);
                    var type = key.Split('_');
                    var currentKey = type[0];
                    var currentValue = result.Values.ElementAt(i);

                    //shortedPoints.Add(currentValue);
                    if (currentKey == "I")
                    {
                        shortedPoints.Add(currentValue);
                        var nextItem = result.Count == i + 1 ? result.ElementAt(0) : result.ElementAt(i + 1);
                        var nexttype = nextItem.Key.Split('_');
                        var nextValue = nextItem.Value;
                        var nextKey = nexttype[0];

                        if (nextKey == "I")
                        {


                            //shortedPoints.Add(nextValue);
                            foreach (var corner in sheetCornerPoint)
                            {
                                //both point should not be on the same line && current point should be on the corner line either left or right &&  next point should be on the corner line either left or right
                                if ((currentValue.X != nextValue.X && currentValue.Y != nextValue.Y))
                                {
                                    if ((corner.X == currentValue.X || corner.Y == currentValue.Y) && (corner.X == nextValue.X || corner.Y == nextValue.Y))//if both the points lies on adjacent line
                                    {
                                        if (shortedPoints.Where(x => x.X == corner.X).Where(x => x.Y == corner.Y).ToList().Count == 0)
                                        {
                                            shortedPoints.Add(corner);
                                            currentValue = corner;
                                        }
                                    }
                                    else
                                    {
                                        shortedPoints.Add(nextValue);
                                        if (shortedPoints.Where(x => x.X == corner.X).Where(x => x.Y == corner.Y).ToList().Count == 0)
                                        {

                                            var newCorderOrder = sheetCornerPoint.Where(x => x.X == nextValue.X);

                                            shortedPoints.Add(corner);
                                            currentValue = corner;
                                        }
                                    }


                                }
                            }
                        }
                        //else
                        //{
                        //    var nextPoint = result.Values.ElementAt(i);
                        //    shortedPoints.Add(nextPoint);
                        //}
                    }
                    else
                    {
                        shortedPoints.Add(currentValue);
                    }

                }
            }
            else if (sheetCornerPoint.Count > 0)
            {
                foreach (var item in sheetCornerPoint)
                {
                    shortedPoints.Add(item);
                }
            }
            else if (sheetCornerPoint.Count == 0 && result.Count == 0)
            {
                shortedPoints.Add(new Point { X = 0, Y = 0 });
            }
            return shortedPoints;
        }

        public static bool InsidePolygon(List<MapPoint> polygon, int N, MapPoint p)
        {
            int counter = 0;
            int i;
            double xinters;
            MapPoint p1, p2;

            p1 = polygon[0];
            for (i = 1; i <= N; i++)
            {
                p2 = polygon[i % N];
                if (p.Y >= Math.Min(p1.Y, p2.Y))
                {
                    if (p.Y <= Math.Max(p1.Y, p2.Y))
                    {
                        if (p.X <= Math.Max(p1.X, p2.X))
                        {
                            if (p1.Y != p2.Y)
                            {
                                xinters = (p.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                                if (p1.X == p2.X || p.X <= xinters)
                                    counter++;
                            }
                        }
                    }
                }
                p1 = p2;
            }

            if (counter % 2 == 0)
                return false; //outside
            else
                return true;
        }

    }




    public class MapPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class EntitySize
    {
        public double Height { get; set; }
        public double Width { get; set; }
    }

}
