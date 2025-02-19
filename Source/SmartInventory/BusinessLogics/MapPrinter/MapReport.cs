using iTextSharp.text;
using iTextSharp.text.pdf;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Utility;

namespace BusinessLogics.MapPrinter
{
    public class MapReport
    {
        public static int[] SCALELIMIT = { 10000000, 5000000, 2000000, 1000000, 500000, 200000, 100000, 50000, 20000, 20000, 10000, 5000, 2000, 1000, 500, 200, 100, 50, 20, 20, 10, 5 };
        public const int ZOOMLIMIT = 21;
        static public string ImageDriectory { get; set; } = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["LegendIcon"]); // @"E:\Projects\Bombay Gas\Source_Code\trunck\SmartInventory\SmartInventory\Content\images\icons\lib\Legend\";

        #region Report
        public string pageType { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float ReportLeftMargin { get; set; }
        public float ReportRightMargin { get; set; }
        public float ReportTopMargin { get; set; }
        public float ReportBottomMargin { get; set; }
        public float ReportCellMargin { get; set; }
        public float ReportBorderWidth { get; set; }

        public DateTime CreatedOn { get; set; }
        #endregion

        #region Header


        public float MainHeaderSize { get; set; }
        public float MainHeaderFontSize { get; set; }
        public float CreatedOnFontSize { get; set; }
        public float MainHeaderBottomPadding { get; set; }
        #endregion

        #region Map
        public float MapLeftMargin { get; set; }
        public float MapTopMargin { get; set; }
        public float MapRightMargin { get; set; }
        public float MapBottomMargin { get; set; }
        public float MapHeight { get; set; }
        public float MapWidth { get; set; }
        public float MapWidthWithoutLegend { get; set; }
        public float NavigationIconHeight { get; set; }
        public float MapFooterZoomScaleHeight { get; set; }
        public float NavigationIconWidth { get; set; }
        public float NavigationFontSize { get; set; }
        public float NavigationScaleWidth { get; set; }
        public float MapHeightWithFooterTemplate { get; set; }
        #endregion

        #region Legend
        public float LegendTopMargin { get; set; }
        public float LegendLeftMargin { get; set; }
        public float LegendRightMargin { get; set; }
        public float LegendBottomMargin { get; set; }
        public float LegendMainHeaderSize { get; set; }
        public float LegendMainHeaderSpacingBefore { get; set; }
        public float LegendMainHeaderSpacingAfter { get; set; }
        public float LegendWidth { get; set; }
        public float LegendHeight { get; set; }
        public float LegendHeaderPaddingTop_1 { get; set; }
        public float LegendHeaderPaddingBottom_1 { get; set; }
        public float LegendHeaderPaddingLeft_1 { get; set; }
        public float TempLegendPaddingTop { get; set; }
        public float TempLegendPaddingBottom { get; set; }
        public float TempLegendPaddingLeft { get; set; }
        public float TempLegendPaddingRight { get; set; }
        public float LegendTitle0_FontSize { get; set; }
        public float LegendContent0_FontSize { get; set; }
        public float LegendTitle1_FontSize { get; set; }
        public float LegendContent1_FontSize { get; set; }
        public float TempTitleFontSize { get; set; }
        public float TempContentFontSize { get; set; }

        public float LegendContentHeight { get; set; }
        public float LegendContentWidth { get; set; }
        public float LegendImageWidth { get; set; }
        public float LegendImageHeight { get; set; }
        public int LegendColumnSize { get; set; }
        public float LegendHeightWithFooterTemplate { get; set; }
        #endregion

        #region Footer
        public float FooterHeight { get; set; }
        public float FooterWidth { get; set; }
        #endregion

        #region Footer Template
        public float FooterTemplateWidth { get; set; }
        public float FooterTemplateHeight { get; set; }
        public float FooterTemplate_FontSize { get; set; }
        public float FooterTemplate_HeadingFontSize { get; set; }
        public float FooterTemplate_BottomMargin { get; set; }
        public float FooterTemplate_insideMargin { get; set; }
        public float FooterTemplate_RowHeight { get; set; }
        public float FooterTemplate_SignColumnHeigh { get; set; }
        public float FooterTemplate_CordinateColumnHeigh { get; set; }
        public float FooterTemplate_GridBoxColumnHeight { get; set; }
        public float FooterTemplate_GridBoxWidth { get; set; }
        public float FooterTemplate_GridBoxHeight { get; set; }
        public float FooterTemplate_Section1Width { get; set; }
        public float FooterTemplate_Section2Width { get; set; }
        public float FooterTemplate_Section2TeamMemberWidth { get; set; }
        public float FooterTemplate_Section2SignWidth { get; set; }
        public float FooterTemplate_Section3Width { get; set; }
        public float FooterTemplate_Section3CordinateWidth { get; set; }
        public float FooterTemplate_Section3SignWidth { get; set; }
        public float FooterTemplate_Section3ScaleWidth { get; set; }
        public float FooterTemplate_Section4Width { get; set; }
        public float FooterTemplate_CheckboxWidth { get; set; }
        public float FooterTemplate_CheckboxHeight { get; set; }
        public float FooterTemplate_CheckboxMargin { get; set; }
        public BaseColor footerTemplate_PrimaryFontColor { get; set; }
        public BaseColor footerTemplate_secondaryFontColor { get; set; }
        #endregion

        public static MapReport A0
        {
            get
            {
                return new MapReport()
                {
                    pageType = "A0",
                    Height = 0,
                    Width = 0,
                    ReportLeftMargin = 40,
                    ReportRightMargin = 40,
                    ReportTopMargin = 0,
                    ReportBottomMargin = 72,
                    ReportCellMargin = 40,
                    ReportBorderWidth = 2,
                    MainHeaderSize = 80,
                    MainHeaderFontSize = 36,
                    CreatedOnFontSize = 24,
                    MainHeaderBottomPadding = 0,

                    MapLeftMargin = 40,
                    MapTopMargin = 40,
                    MapRightMargin = 40,
                    MapBottomMargin = 40,
                    MapHeight = 2152,
                    MapWidth = 2152,
                    MapWidthWithoutLegend = 3210,
                    LegendTopMargin = 40,
                    LegendLeftMargin = 40,
                    LegendRightMargin = 40,
                    LegendBottomMargin = 0,
                    LegendMainHeaderSize = 0,
                    LegendWidth = 0,
                    LegendHeight = 0,
                    LegendTitle0_FontSize = 32,
                    LegendContent0_FontSize = 24,
                    LegendTitle1_FontSize = 24,
                    LegendContent1_FontSize = 20,
                    LegendImageHeight = 24,
                    LegendImageWidth = 24,

                    LegendContentHeight = 0,
                    LegendContentWidth = 0,
                    FooterHeight = 72,
                    FooterWidth = 0,
                    LegendHeaderPaddingTop_1 = 20,
                    LegendHeaderPaddingBottom_1 = 14,
                    LegendColumnSize = 3,
                    LegendHeaderPaddingLeft_1 = 16,
                    LegendHeightWithFooterTemplate = 1888,
                    NavigationFontSize = 20,
                    NavigationIconHeight = 56,
                    NavigationIconWidth = 56,
                    NavigationScaleWidth = 220,
                    MapFooterZoomScaleHeight = 48,
                    //footer template columns
                    MapHeightWithFooterTemplate = 1888,
                    FooterTemplateWidth = 3210,
                    FooterTemplateHeight = 224,
                    FooterTemplate_FontSize = 22,
                    FooterTemplate_HeadingFontSize = 26,
                    FooterTemplate_BottomMargin = 40,
                    FooterTemplate_RowHeight = 56,
                    FooterTemplate_SignColumnHeigh = 84,
                    FooterTemplate_insideMargin = 20,
                    FooterTemplate_CordinateColumnHeigh = 84,
                    FooterTemplate_GridBoxColumnHeight = 168,
                    FooterTemplate_GridBoxWidth = 120,
                    FooterTemplate_GridBoxHeight = 120,
                    FooterTemplate_Section1Width = 540,
                    FooterTemplate_Section2Width = 940,
                    FooterTemplate_Section3Width = 1190,
                    FooterTemplate_Section4Width = 540,
                    FooterTemplate_Section2SignWidth = 400,
                    FooterTemplate_Section2TeamMemberWidth = 540,
                    FooterTemplate_Section3SignWidth = 400,
                    FooterTemplate_Section3CordinateWidth = 260,
                    FooterTemplate_Section3ScaleWidth = 530,
                    FooterTemplate_CheckboxWidth = 16,
                    FooterTemplate_CheckboxHeight = 16,
                    FooterTemplate_CheckboxMargin = 5,
                    footerTemplate_PrimaryFontColor = new BaseColor(126, 126, 126),
                    footerTemplate_secondaryFontColor = new BaseColor(64, 61, 61),
                    CreatedOn = DateTimeHelper.Now
                };
            }
        }
        public static MapReport A1
        {
            get
            {
                return new MapReport()
                {
                    pageType = "A1",
                    Height = 0,
                    Width = 0,
                    MainHeaderSize = 64,
                    MainHeaderFontSize = 28,
                    CreatedOnFontSize = 18,
                    ReportLeftMargin = 30,
                    ReportRightMargin = 30,
                    ReportTopMargin = 0,
                    ReportBottomMargin = 48,
                    ReportCellMargin = 30,
                    ReportBorderWidth = 2,
                    MapLeftMargin = 30,
                    MapTopMargin = 30,
                    MapRightMargin = 30,
                    MapBottomMargin = 30,
                    MapHeight = 1512,
                    MapWidth = 1512,
                    MapWidthWithoutLegend = 2264,
                    LegendTopMargin = 30,
                    LegendLeftMargin = 30,
                    LegendRightMargin = 30,
                    LegendBottomMargin = 0,
                    LegendMainHeaderSize = 0,
                    LegendWidth = 0,
                    LegendHeight = 0,
                    LegendTitle0_FontSize = 23,
                    LegendContent0_FontSize = 20,
                    LegendTitle1_FontSize = 20,
                    LegendContent1_FontSize = 16,
                    LegendImageHeight = 20,
                    LegendImageWidth = 20,

                    LegendContentHeight = 0,
                    LegendContentWidth = 0,
                    FooterHeight = 48,
                    FooterWidth = 0,
                    LegendMainHeaderSpacingAfter = 0,
                    LegendMainHeaderSpacingBefore = 0,
                    LegendHeaderPaddingTop_1 = 18,
                    LegendHeaderPaddingBottom_1 = 12,
                    LegendColumnSize = 2,
                    LegendHeaderPaddingLeft_1 = 12,
                    LegendHeightWithFooterTemplate = 1306,
                    NavigationFontSize = 16,
                    NavigationIconHeight = 40,
                    NavigationIconWidth = 40,
                    NavigationScaleWidth = 190,
                    MapFooterZoomScaleHeight = 37,
                    //footer template columns
                    MapHeightWithFooterTemplate = 1306,
                    FooterTemplateWidth = 2264,
                    FooterTemplateHeight = 176,
                    FooterTemplate_FontSize = 16,
                    FooterTemplate_HeadingFontSize = 20,
                    FooterTemplate_BottomMargin = 30,
                    FooterTemplate_RowHeight = 44,
                    FooterTemplate_SignColumnHeigh = 66,
                    FooterTemplate_insideMargin = 16,
                    FooterTemplate_CordinateColumnHeigh = 66,
                    FooterTemplate_GridBoxColumnHeight = 132,
                    FooterTemplate_GridBoxWidth = 90,
                    FooterTemplate_GridBoxHeight = 90,
                    FooterTemplate_Section1Width = 360,
                    FooterTemplate_Section2Width = 660,
                    FooterTemplate_Section3Width = 883,
                    FooterTemplate_Section4Width = 360,
                    FooterTemplate_Section2SignWidth = 300,
                    FooterTemplate_Section2TeamMemberWidth = 360,
                    FooterTemplate_Section3SignWidth = 300,
                    FooterTemplate_Section3CordinateWidth = 200,
                    FooterTemplate_Section3ScaleWidth = 383,
                    FooterTemplate_CheckboxWidth = 14,
                    FooterTemplate_CheckboxHeight = 14,
                    FooterTemplate_CheckboxMargin = 4,
                    footerTemplate_PrimaryFontColor = new BaseColor(126, 126, 126),
                    footerTemplate_secondaryFontColor = new BaseColor(64, 61, 61),
                    CreatedOn = DateTimeHelper.Now
                };
            }
        }
        public static MapReport A2
        {
            get
            {
                return new MapReport()
                {
                    pageType = "A2",
                    Height = 0,
                    Width = 0,
                    MainHeaderSize = 56,
                    MainHeaderFontSize = 20,
                    CreatedOnFontSize = 14,
                    ReportLeftMargin = 20,
                    ReportRightMargin = 20,
                    ReportTopMargin = 0,
                    ReportBottomMargin = 48,
                    ReportCellMargin = 20,
                    ReportBorderWidth = 1,
                    MapLeftMargin = 20,
                    MapTopMargin = 20,
                    MapRightMargin = 20,
                    MapBottomMargin = 20,
                    MapHeight = 1047,
                    MapWidth = 1047,
                    MapWidthWithoutLegend = 1568,
                    LegendTopMargin = 20,
                    LegendLeftMargin = 20,
                    LegendRightMargin = 20,
                    LegendBottomMargin = 0,
                    LegendMainHeaderSize = 0,
                    LegendWidth = 0,
                    LegendHeight = 0,
                    LegendTitle0_FontSize = 19,
                    LegendContent0_FontSize = 14,
                    LegendTitle1_FontSize = 14,
                    LegendContent1_FontSize = 12,
                    LegendImageHeight = 16,
                    LegendImageWidth = 16,

                    LegendContentHeight = 0,
                    LegendContentWidth = 0,
                    FooterHeight = 48,
                    FooterWidth = 0,
                    LegendMainHeaderSpacingAfter = 0,
                    LegendMainHeaderSpacingBefore = 0,
                    LegendHeaderPaddingTop_1 = 16,
                    LegendHeaderPaddingBottom_1 = 10,
                    LegendColumnSize = 2,
                    LegendHeaderPaddingLeft_1 = 10,
                    LegendHeightWithFooterTemplate = 897,
                    NavigationFontSize = 13,
                    NavigationIconHeight = 32,
                    NavigationIconWidth = 32,
                    NavigationScaleWidth = 160,
                    MapFooterZoomScaleHeight = 28,
                    //footer template columns
                    MapHeightWithFooterTemplate = 897,
                    FooterTemplateWidth = 1566,
                    FooterTemplateHeight = 128,
                    FooterTemplate_FontSize = 12,
                    FooterTemplate_HeadingFontSize = 15,
                    FooterTemplate_insideMargin = 12,
                    FooterTemplate_BottomMargin = 20,
                    FooterTemplate_RowHeight = 32,
                    FooterTemplate_SignColumnHeigh = 48,
                    FooterTemplate_CordinateColumnHeigh = 48,
                    FooterTemplate_GridBoxColumnHeight = 96,
                    FooterTemplate_GridBoxWidth = 66,
                    FooterTemplate_GridBoxHeight = 66,
                    FooterTemplate_Section1Width = 260,
                    FooterTemplate_Section2Width = 480,
                    FooterTemplate_Section3Width = 568,
                    FooterTemplate_Section4Width = 260,
                    FooterTemplate_Section2SignWidth = 220,
                    FooterTemplate_Section2TeamMemberWidth = 260,
                    FooterTemplate_Section3SignWidth = 220,
                    FooterTemplate_Section3CordinateWidth = 136,
                    FooterTemplate_Section3ScaleWidth = 212,
                    FooterTemplate_CheckboxWidth = 12,
                    FooterTemplate_CheckboxHeight = 12,
                    FooterTemplate_CheckboxMargin = 3,
                    footerTemplate_PrimaryFontColor = new BaseColor(126, 126, 126),
                    footerTemplate_secondaryFontColor = new BaseColor(64, 61, 61),
                    CreatedOn = DateTimeHelper.Now
                };
            }
        }
        public static MapReport A3
        {
            get
            {
                return new MapReport()
                {
                    pageType = "A3",
                    Height = 0,
                    Width = 0,
                    MainHeaderSize = 40,
                    MainHeaderFontSize = 16,
                    CreatedOnFontSize = 12,
                    ReportLeftMargin = 16,
                    ReportRightMargin = 16,
                    ReportTopMargin = 0,
                    ReportBottomMargin = 32,
                    ReportCellMargin = 16,
                    ReportBorderWidth = 1,
                    MapLeftMargin = 16,
                    MapTopMargin = 16,
                    MapRightMargin = 16,
                    MapBottomMargin = 16,
                    MapHeight = 738,
                    MapWidth = 738,
                    MapWidthWithoutLegend = 1127,
                    LegendTopMargin = 16,
                    LegendLeftMargin = 16,
                    LegendRightMargin = 16,
                    LegendBottomMargin = 0,
                    LegendMainHeaderSize = 0,
                    LegendWidth = 0,
                    LegendHeight = 0,
                    LegendTitle0_FontSize = 15,
                    LegendContent0_FontSize = 12,
                    LegendTitle1_FontSize = 12,
                    LegendContent1_FontSize = 10,
                    LegendHeightWithFooterTemplate = 624,
                    LegendImageHeight = 12,
                    LegendImageWidth = 12,

                    LegendContentHeight = 0,
                    LegendContentWidth = 0,
                    FooterHeight = 32,
                    FooterWidth = 0,
                    LegendMainHeaderSpacingAfter = 0,
                    LegendMainHeaderSpacingBefore = 0,
                    LegendHeaderPaddingTop_1 = 14,
                    LegendHeaderPaddingBottom_1 = 8,
                    LegendColumnSize = 2,
                    LegendHeaderPaddingLeft_1 = 7,
                    NavigationFontSize = 10,
                    NavigationIconHeight = 32,
                    NavigationIconWidth = 32,
                    NavigationScaleWidth = 130,
                    MapFooterZoomScaleHeight = 24,
                    //footer template columns
                    MapHeightWithFooterTemplate = 624,
                    FooterTemplateWidth = 1127,
                    FooterTemplateHeight = 96,
                    FooterTemplate_FontSize = 9,
                    FooterTemplate_HeadingFontSize = 13,
                    FooterTemplate_insideMargin = 8,
                    FooterTemplate_BottomMargin = 22,
                    FooterTemplate_RowHeight = 24,
                    FooterTemplate_SignColumnHeigh = 36,
                    FooterTemplate_CordinateColumnHeigh = 36,
                    FooterTemplate_GridBoxColumnHeight = 72,
                    FooterTemplate_GridBoxWidth = 54,
                    FooterTemplate_GridBoxHeight = 54,
                    FooterTemplate_Section1Width = 188,
                    FooterTemplate_Section2Width = 328,
                    FooterTemplate_Section3Width = 353,
                    FooterTemplate_Section4Width = 188,
                    FooterTemplate_Section2SignWidth = 140,
                    FooterTemplate_Section2TeamMemberWidth = 188,
                    FooterTemplate_Section3SignWidth = 140,
                    FooterTemplate_Section3CordinateWidth = 95,
                    FooterTemplate_Section3ScaleWidth = 118,
                    FooterTemplate_CheckboxWidth = 10,
                    FooterTemplate_CheckboxHeight = 10,
                    FooterTemplate_CheckboxMargin = 2,
                    footerTemplate_PrimaryFontColor = new BaseColor(126, 126, 126),
                    footerTemplate_secondaryFontColor = new BaseColor(64, 61, 61),
                    CreatedOn = DateTimeHelper.Now

                };
            }
        }
        public static MapReport A4
        {
            get
            {
                return new MapReport()
                {
                    pageType = "A4",
                    Height = 0,
                    Width = 0,
                    MainHeaderSize = 28,
                    MainHeaderFontSize = 12,
                    CreatedOnFontSize = 8,
                    ReportLeftMargin = 12,
                    ReportRightMargin = 12,
                    ReportTopMargin = 0,
                    ReportBottomMargin = 20,
                    ReportCellMargin = 10,
                    ReportBorderWidth = 1,
                    MapLeftMargin = 10,
                    MapTopMargin = 10,
                    MapRightMargin = 12,
                    MapBottomMargin = 10,
                    MapHeight = 527,
                    MapWidth = 527,
                    MapWidthWithoutLegend = 796,
                    LegendTopMargin = 10,
                    LegendLeftMargin = 8,
                    LegendRightMargin = 8,
                    LegendBottomMargin = 0,
                    LegendMainHeaderSize = 0,
                    LegendWidth = 0,
                    LegendHeight = 0,
                    LegendTitle0_FontSize = 11,
                    LegendContent0_FontSize = 10,
                    LegendTitle1_FontSize = 10,
                    LegendContent1_FontSize = 8,
                    LegendImageHeight = 15,
                    LegendImageWidth = 15,

                    LegendContentHeight = 0,
                    LegendContentWidth = 0,
                    FooterHeight = 20,
                    FooterWidth = 0,
                    LegendMainHeaderSpacingAfter = 0,
                    LegendMainHeaderSpacingBefore = 0,
                    LegendHeaderPaddingTop_1 = 12,
                    LegendHeaderPaddingBottom_1 = 6,
                    LegendColumnSize = 2,
                    LegendHeaderPaddingLeft_1 = 6,
                    LegendHeightWithFooterTemplate = 445,
                    NavigationFontSize = 8,
                    NavigationIconHeight = 24,
                    NavigationIconWidth = 24,
                    NavigationScaleWidth = 100,
                    MapFooterZoomScaleHeight = 16,
                    //footer template columns
                    MapHeightWithFooterTemplate = 445,
                    FooterTemplateWidth = 798,
                    FooterTemplateHeight = 72,
                    FooterTemplate_FontSize = 7,
                    FooterTemplate_HeadingFontSize = 10,
                    FooterTemplate_insideMargin = 4,
                    FooterTemplate_BottomMargin = 10,
                    FooterTemplate_RowHeight = 18,
                    FooterTemplate_SignColumnHeigh = 27,
                    FooterTemplate_CordinateColumnHeigh = 27,
                    FooterTemplate_GridBoxColumnHeight = 54,
                    FooterTemplate_GridBoxWidth = 42,
                    FooterTemplate_GridBoxHeight = 42,
                    FooterTemplate_Section1Width = 136,
                    FooterTemplate_Section2Width = 230,
                    FooterTemplate_Section3Width = 296,
                    FooterTemplate_Section4Width = 136,
                    FooterTemplate_Section2SignWidth = 110,
                    FooterTemplate_Section2TeamMemberWidth = 120,
                    FooterTemplate_Section3SignWidth = 110,
                    FooterTemplate_Section3CordinateWidth = 72,
                    FooterTemplate_Section3ScaleWidth = 114,
                    FooterTemplate_CheckboxWidth = 8,
                    FooterTemplate_CheckboxHeight = 8,
                    FooterTemplate_CheckboxMargin = 1,
                    footerTemplate_PrimaryFontColor = new BaseColor(126, 126, 126),
                    footerTemplate_secondaryFontColor = new BaseColor(64, 61, 61),
                    CreatedOn = DateTimeHelper.Now

                };
            }
        }


        public static MapReport GetMapReportContent(PrintMap printMap)
        {
            var PageType = printMap.pageSize;
            var mapReport = new MapReport();
            if (PageType == "A4")
            {
                mapReport = MapReport.A4;
            }
            else if (PageType == "A3")
            {
                mapReport = MapReport.A3;
            }
            else if (PageType == "A2")
            {
                mapReport = MapReport.A2;
            }
            else if (PageType == "A1")
            {
                mapReport = MapReport.A1;
            }
            else if (PageType == "A0")
            {
                mapReport = MapReport.A0;
            }
            else
            {
                mapReport = MapReport.A0;
            }
            if (!printMap.printLegend || printMap.pageScale > 0)
            {
                mapReport.MapWidth = mapReport.MapWidthWithoutLegend;
            }
            if (printMap.isFooterTemplateEnabled)
            {
                mapReport.MapHeight = mapReport.MapHeightWithFooterTemplate;
            }
            else
            {
                mapReport.FooterTemplateHeight = 0;
                mapReport.FooterTemplateWidth = 0;
            }
            return mapReport;
        }

        public static MemoryStream ExportPdf(Rectangle cordinates, iTextSharp.text.Image layerImage, List<LegendGroup> legendGroupData, MapReport mapReport, PrintMap printMap, List<ReportSheet> pdfSheetsList, int sheetCurrentIndex, int printZoom = 21, double mapDistance = 1)
        {
            try
            {
                using (MemoryStream stream = new System.IO.MemoryStream())
                {

                    sheetCurrentIndex = sheetCurrentIndex == 0 ? 1 : sheetCurrentIndex;
                    Document pdfDoc = new Document(cordinates, 0f, 0f, mapReport.MainHeaderSize, mapReport.ReportBottomMargin);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);

                    float mapWidth = 0;
                    float leftRightMargin = ((mapReport.ReportLeftMargin + mapReport.ReportRightMargin) * 100 / cordinates.Width);
                    //Header Footer operation

                    //Left side : Map column
                    if (printMap.pageScale == 0)
                    {
                        if (printZoom < ZOOMLIMIT)
                            layerImage = ResizeImageKeepAspectRatio(layerImage, (int)(mapReport.MapWidth), (int)(mapReport.MapHeight));

                        if (layerImage.ScaledWidth >= mapReport.MapWidth || layerImage.ScaledHeight >= mapReport.MapHeight)
                        {
                            layerImage = ResizeImageKeepAspectRatio(layerImage, (int)(mapReport.MapWidth), (int)(mapReport.MapHeight));
                        }
                    }
                    else
                    {
                        if (layerImage.ScaledWidth >= mapReport.MapWidth || layerImage.ScaledHeight >= mapReport.MapHeight)
                        {
                            layerImage = ResizeImageKeepAspectRatio(layerImage, (int)(mapReport.MapWidth), (int)(mapReport.MapHeight));
                        }
                    }
                    mapWidth = layerImage.ScaledWidth;
                    var reportPageEvent = new MapReportPageEvents()
                    {
                        mapReport = mapReport,
                        LeftRightMargin = leftRightMargin,
                        ReportTitle = printMap.pageTitle,
                        PrintZoom = printZoom,
                        MapDistance = mapDistance,
                        MapWidth = mapWidth,
                        printMap = printMap,
                        PageNumber = sheetCurrentIndex == 0 ? 1 : sheetCurrentIndex// print current page number on the page itself.
                    };
                    pdfWriter.PageEvent = reportPageEvent;


                    //pdfWriter.PageEvent = new ITextEvents();

                    //OPEN PDFDOC OBJECT TO WRITE CONTENT
                    pdfDoc.Open();

                    PdfPTable parentTable = new PdfPTable(1);
                    parentTable.PaddingTop = mapReport.MainHeaderSize;
                    parentTable.WidthPercentage = 100f - leftRightMargin;
                    parentTable.SplitLate = false;
                    //2. add content : left side image , right side legend view
                    // int PdfCellCnt = printMap.pageScale > 0 ? 3 : 1;////krish
                    PdfPTable contentTable = new PdfPTable(printMap.PdfCellCnt);
                    contentTable.SplitLate = false;
                    //contentTable.PaddingTop = mapReport.MainHeaderSize;

                    contentTable.WidthPercentage = 100f;
                    float mapCellWidth = mapReport.MapWidth + mapReport.MapLeftMargin + mapReport.MapRightMargin + mapReport.ReportLeftMargin;
                    if (printMap.PdfCellCnt > 1)////krish
                        contentTable.SetWidths(new float[] { mapCellWidth, mapReport.ReportCellMargin, (cordinates.Width - mapCellWidth - mapReport.ReportCellMargin) });


                    PdfPCell mapCell = new PdfPCell(layerImage, false);
                    mapCell.PaddingRight = mapReport.MapRightMargin;

                    if (layerImage.ScaledWidth < mapReport.MapWidth)
                    {
                        mapCell.PaddingLeft = printMap.pageScale == 0 || printMap.mapCurrentZoom >= printMap.pageScale ? Math.Abs(layerImage.ScaledWidth - mapReport.MapWidth) / 2 : 0;
                        mapCell.PaddingRight = Math.Abs(layerImage.ScaledWidth - mapReport.MapWidth) / 2;
                    }
                    if (layerImage.ScaledHeight < mapReport.MapHeight)
                    {
                        mapCell.PaddingTop = printMap.pageScale == 0 || printMap.mapCurrentZoom >= printMap.pageScale ? Math.Abs(layerImage.ScaledHeight - mapReport.MapHeight) / 2 : 0;

                        if (printMap.pageScale == 0 || printMap.mapCurrentZoom >= printMap.pageScale)
                        {
                            mapCell.PaddingBottom = (Math.Abs(layerImage.ScaledHeight - mapReport.MapHeight) / 2);
                        }
                        else
                        {
                            mapCell.PaddingBottom = (Math.Abs(layerImage.ScaledHeight - mapReport.MapHeight) / 2) + (Math.Abs(layerImage.ScaledHeight - mapReport.MapHeight) / 2);
                        }
                    }
                    mapCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    mapCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    if (pdfSheetsList.Count > 1)
                    {
                        mapCell.HorizontalAlignment = Element.ALIGN_TOP;
                        mapCell.VerticalAlignment = Element.ALIGN_LEFT;
                    }


                    //mapCell.BorderWidth = 0;
                    mapCell.BorderWidth = mapReport.ReportBorderWidth;
                    mapCell.BorderColor = new BaseColor(216, 216, 216);
                    mapCell.Colspan = (printMap.printLegend && printMap.pageScale == 0) ? 1 : 3;
                    contentTable.AddCell(mapCell);
                    int LegendColSize = printMap.PdfCellCnt > 1 ? mapReport.LegendColumnSize : 6;
                    PdfPTable legendTable = new PdfPTable(LegendColSize);////krishna


                    if (printMap.printLegend && printMap.pageScale == 0)
                    {
                        if (printMap.PdfCellCnt > 1)
                            contentTable.AddCell(new PdfPCell() { BorderWidth = 0 }); // blank cell in middle of legend and map
                                                                                      // get legend table.
                        mapReport.LegendColumnSize = LegendColSize;
                        legendTable = GetLegendTable(mapReport, printMap, legendGroupData);

                        if (printMap.PdfCellCnt > 1)
                        {
                            contentTable.AddCell(new PdfPCell(legendTable)
                            {
                                BorderWidth = mapReport.ReportBorderWidth,
                                BorderColor = new BaseColor(216, 216, 216)
                            });
                        }
                    }
                    if (printMap.isFooterTemplateEnabled && printMap.ApplicableModuleList.Contains("PMA"))
                    {
                        // blank row as a seperator..
                        contentTable.AddCell(new PdfPCell() { Colspan = 3, MinimumHeight = mapReport.ReportCellMargin, BorderWidth = 0 });
                        // PdfPTable footerTemplateTable = GetFooterTemplateTable(mapReport, printMap, reportPageEvent.PrintZoom, pdfSheetsList, sheetCurrentIndex);
                        PdfPTable footerTemplateTable = GetFooterTemplateTable(mapReport, printMap, printZoom, pdfSheetsList, sheetCurrentIndex);
                        contentTable.AddCell(new PdfPCell(footerTemplateTable)
                        {
                            Colspan = 3
                        });
                    }

                    parentTable.AddCell(new PdfPCell(contentTable)
                    {
                        BorderWidth = mapReport.ReportBorderWidth,
                        BorderColor = new BaseColor(112, 112, 112),
                        Padding = mapReport.ReportCellMargin
                    });
                    ////krishna
                    if (printMap.PdfCellCnt == 1)
                    {
                        parentTable.AddCell(new PdfPCell(legendTable)
                        {
                            BorderWidth = mapReport.ReportBorderWidth,
                            BorderColor = new BaseColor(112, 112, 112),
                            Padding = mapReport.ReportCellMargin
                        });
                    }


                    pdfDoc.Add(parentTable);
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return stream;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("ExportPdf()", "MapReport", ex);
                throw ex;
            }
        }

        public static int[] getFooterPageMatrix(List<ReportSheet> pdfSheetsList, int currentIndex)
        {
            int[] arrPageIndex = new int[9];
            var objSheet = pdfSheetsList.Find(x => x.Revised_Id == currentIndex);
            double currentX = objSheet != null ? objSheet.Index.X : 0;//column
            double currentY = objSheet != null ? objSheet.Index.Y : 0;//row       
            arrPageIndex[0] = getReportSheetNumberByIndex(pdfSheetsList, currentX - 1, currentY - 1);
            arrPageIndex[1] = getReportSheetNumberByIndex(pdfSheetsList, currentX, currentY - 1);
            arrPageIndex[2] = getReportSheetNumberByIndex(pdfSheetsList, currentX + 1, currentY - 1);
            arrPageIndex[3] = getReportSheetNumberByIndex(pdfSheetsList, currentX - 1, currentY);
            arrPageIndex[4] = objSheet != null ? getReportSheetNumberByIndex(pdfSheetsList, currentX, currentY) : 1;// to handle the single page scnerio as sheet list is blank for same.
            arrPageIndex[5] = getReportSheetNumberByIndex(pdfSheetsList, currentX + 1, currentY);
            arrPageIndex[6] = getReportSheetNumberByIndex(pdfSheetsList, currentX - 1, currentY + 1);
            arrPageIndex[7] = getReportSheetNumberByIndex(pdfSheetsList, currentX, currentY + 1);
            arrPageIndex[8] = getReportSheetNumberByIndex(pdfSheetsList, currentX + 1, currentY + 1);
            return arrPageIndex;
        }
        public static int getReportSheetNumberByIndex(List<ReportSheet> pdfSheetsList, double x, double y)
        {
            var objSheet = pdfSheetsList.Find(m => m.Index.X == x && m.Index.Y == y);
            return objSheet != null ? objSheet.Revised_Id : 0;
        }
        public static PdfPTable GetFooterTemplateTable(MapReport mapReport, PrintMap printMap, int printZoom, List<ReportSheet> pdfSheetsList, int sheetCurrentIndex)
        {
            PdfPTable footerTemplate = new PdfPTable(4);
            footerTemplate.TotalWidth = mapReport.FooterTemplateWidth;
            footerTemplate.LockedWidth = true;
            float[] widths = new float[] { mapReport.FooterTemplate_Section1Width, mapReport.FooterTemplate_Section2Width, mapReport.FooterTemplate_Section3Width, mapReport.FooterTemplate_Section4Width };
            footerTemplate.SetWidths(widths);
            iTextSharp.text.Font fonts = new iTextSharp.text.Font(GetFonts(), mapReport.FooterTemplate_FontSize, iTextSharp.text.Font.NORMAL, mapReport.footerTemplate_secondaryFontColor);


            #region  ROW 1- COL1 Text
            PdfPTable row1Col1Table = new PdfPTable(2);
            PdfPCell row1Col1_1 = new PdfPCell() { BorderWidth = 0 };

            row1Col1_1 = getFooterTextWithIcon(row1Col1_1, printMap.checkBox_ImagePath, Resources.Resources.SI_OSP_PM_NET_FRM_022, mapReport, mapReport.footerTemplate_PrimaryFontColor, mapReport.FooterTemplate_FontSize);
            PdfPCell row1Col1_2 = new PdfPCell() { BorderWidth = 0 };
            row1Col1_2 = getFooterTextWithIcon(row1Col1_2, printMap.checkBox_ImagePath, "As-Built Design", mapReport, mapReport.footerTemplate_PrimaryFontColor, mapReport.FooterTemplate_FontSize);
            row1Col1Table.AddCell(row1Col1_1);
            row1Col1Table.AddCell(row1Col1_2);
            footerTemplate.AddCell(new PdfPCell(row1Col1Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            #endregion

            #region ROW 1- COL2 Text
            footerTemplate.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_024, fonts)))
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            #endregion

            #region ROW 1- COL3 Text
            PdfPCell row1Cell3 = new PdfPCell()
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            row1Cell3 = getFooterText(row1Cell3, ConfigurationManager.AppSettings["ClientName"].ToString().Trim().ToUpper(), mapReport, mapReport.footerTemplate_secondaryFontColor, mapReport.FooterTemplate_HeadingFontSize);
            footerTemplate.AddCell(row1Cell3);
            #endregion

            #region  ROW 1- COL4 Text
            PdfPTable row1Col4Table = new PdfPTable(new float[] { 60f, 20f, 20f });
            row1Col4Table.WidthPercentage = 100f;
            //Resources.Resources.SI_OSP_PM_NET_FRM_025
            row1Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk("Dept.: " + printMap.printMapAttr.department, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row1Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_016, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row1Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.phase, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            footerTemplate.AddCell(new PdfPCell(row1Col4Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            #endregion

            #region ROW 2- COL1 Text
            PdfPTable row2Col1Table = new PdfPTable(new float[] { 20f, 80f });
            row2Col1Table.WidthPercentage = 100f;
            row2Col1Table.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_008, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col1Table.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.jobId, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            footerTemplate.AddCell(new PdfPCell(row2Col1Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            #endregion

            #region ROW 2- COL2 Text
            PdfPTable row2Col2Table = new PdfPTable(2);
            row2Col2Table.TotalWidth = mapReport.FooterTemplate_Section2Width;
            row2Col2Table.LockedWidth = true;
            float[] row2Col2TableWidths = new float[] { mapReport.FooterTemplate_Section2TeamMemberWidth, mapReport.FooterTemplate_Section2SignWidth };
            row2Col2Table.SetWidths(row2Col2TableWidths);
            // contains Team Members related detail..
            PdfPTable row2Col2Table_1 = new PdfPTable(1);
            row2Col2Table_1.WidthPercentage = 100f;
            row2Col2Table_1.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_026, fonts)))
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            });
            row2Col2Table_1.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.team, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_PrimaryFontColor))))
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight * 2,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            });
            //contains Signature & detail
            PdfPTable row2Col2Table_2 = new PdfPTable(1);
            row2Col2Table_2.WidthPercentage = 100f;

            //contains first signature detail..
            PdfPTable row2Col2Table_2_1 = new PdfPTable(new float[] { 35f, 65f });
            row2Col2Table_2_1.WidthPercentage = 100f;
            row2Col2Table_2_1.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_011, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col2Table_2_1.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.drawnBy, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col2Table_2_1.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_027, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col2Table_2_1.AddCell(new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            //contains second signature detail..
            PdfPTable row2Col2Table_2_2 = new PdfPTable(new float[] { 35f, 65f });
            row2Col2Table_2_2.WidthPercentage = 100f;
            row2Col2Table_2_2.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_012, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col2Table_2_2.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.checkedBy, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col2Table_2_2.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_027, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col2Table_2_2.AddCell(new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            row2Col2Table_2.AddCell(new PdfPCell(row2Col2Table_2_1)
            {
                FixedHeight = mapReport.FooterTemplate_SignColumnHeigh,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            row2Col2Table_2.AddCell(new PdfPCell(row2Col2Table_2_2)
            {
                FixedHeight = mapReport.FooterTemplate_SignColumnHeigh,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            row2Col2Table.AddCell(new PdfPCell(row2Col2Table_1) { BorderWidth = 0 });
            row2Col2Table.AddCell(new PdfPCell(row2Col2Table_2) { BorderWidth = 0 });

            footerTemplate.AddCell(new PdfPCell(row2Col2Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight * 3,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Rowspan = 3
            });

            #endregion

            #region ROW 2- COL3 Text

            PdfPTable row2Col3Table = new PdfPTable(3);
            row2Col3Table.TotalWidth = mapReport.FooterTemplate_Section3Width;
            row2Col3Table.LockedWidth = true;
            float[] row2Col3TableWidths = new float[] { mapReport.FooterTemplate_Section3SignWidth, mapReport.FooterTemplate_Section3CordinateWidth, mapReport.FooterTemplate_Section3ScaleWidth };
            row2Col3Table.SetWidths(row2Col3TableWidths);

            //contains Signature & detail
            PdfPTable row2Col3Table_1 = new PdfPTable(1);
            row2Col3Table_1.WidthPercentage = 100f;

            //contains first signature detail..
            PdfPTable row2Col3Table_1_1 = new PdfPTable(new float[] { 40f, 60f });
            row2Col3Table_1_1.WidthPercentage = 100f;
            row2Col3Table_1_1.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_028, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col3Table_1_1.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.recheckedBy, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col3Table_1_1.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_027, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col3Table_1_1.AddCell(new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            //contains second signature detail..
            PdfPTable row2Col3Table_1_2 = new PdfPTable(new float[] { 40f, 60f });
            row2Col3Table_1_2.WidthPercentage = 100f;
            row2Col3Table_1_2.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_013, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col3Table_1_2.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.approvedBy, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col3Table_1_2.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_027, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col3Table_1_2.AddCell(new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            row2Col3Table_1.AddCell(new PdfPCell(row2Col3Table_1_1)
            {
                FixedHeight = mapReport.FooterTemplate_SignColumnHeigh,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            row2Col3Table_1.AddCell(new PdfPCell(row2Col3Table_1_2)
            {
                FixedHeight = mapReport.FooterTemplate_SignColumnHeigh,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            row2Col3Table.AddCell(new PdfPCell(row2Col3Table_1) { BorderWidth = 0 });


            //contains X & Y Cordinate Details
            PdfPTable row2Col3Table_2 = new PdfPTable(1);
            row2Col3Table_2.WidthPercentage = 100f;
            //contains first signature detail..
            PdfPTable row2Col3Table_2_1 = new PdfPTable(1);
            row2Col3Table_2_1.WidthPercentage = 100f;
            row2Col3Table_2_1.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_029, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col3Table_2_1.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.X_Document_Index, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            //contains second signature detail..
            PdfPTable row2Col3Table_2_2 = new PdfPTable(1);
            row2Col3Table_2_2.WidthPercentage = 100f;
            row2Col3Table_2_2.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_030, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col3Table_2_2.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.Y_Document_Index, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            row2Col3Table_2.AddCell(new PdfPCell(row2Col3Table_2_1)
            {
                FixedHeight = mapReport.FooterTemplate_SignColumnHeigh,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            row2Col3Table_2.AddCell(new PdfPCell(row2Col3Table_2_2)
            {
                FixedHeight = mapReport.FooterTemplate_SignColumnHeigh,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            row2Col3Table.AddCell(new PdfPCell(row2Col3Table_2) { BorderWidth = 0 });

            // Scale related info..
            PdfPTable row2Col3Table_3 = new PdfPTable(2);
            row2Col3Table_3.WidthPercentage = 100f;

            PdfPTable row2Col3Table_3_1 = new PdfPTable(new float[] { mapReport.FooterTemplate_GridBoxWidth / 3, mapReport.FooterTemplate_GridBoxWidth / 3, mapReport.FooterTemplate_GridBoxWidth / 3 });
            row2Col3Table_3_1.TotalWidth = mapReport.FooterTemplate_GridBoxWidth;
            var arrFooterPageMatrix = getFooterPageMatrix(pdfSheetsList, sheetCurrentIndex);
            for (int i = 0; i < 9; i++)
            {
                row2Col3Table_3_1.AddCell(new PdfPCell(new Phrase(new Chunk(arrFooterPageMatrix[i] == 0 ? "" : arrFooterPageMatrix[i].ToString(), FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_PrimaryFontColor))))
                {
                    FixedHeight = mapReport.FooterTemplate_GridBoxWidth / 3,
                    BorderWidth = mapReport.ReportBorderWidth,
                    BackgroundColor = arrFooterPageMatrix[i] == sheetCurrentIndex ? new BaseColor(245, 245, 245) : BaseColor.WHITE,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                });
            }

            row2Col3Table_3.AddCell(new PdfPCell(row2Col3Table_3_1) { BorderWidth = 0, Padding = mapReport.FooterTemplate_insideMargin });

            //contains scale value
            PdfPTable row2Col3Table_3_2 = new PdfPTable(1);
            row2Col3Table_3_2.WidthPercentage = 100f;
            //row2Col3Table_3_2.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_031 + GetMapScaleRatio(printZoom), fonts)))
            string sclR = "";
            if (printMap.mapScaleRatio != null && printMap.mapScaleRatio.Contains("-"))
            {
                string[] Scratio = printMap.mapScaleRatio.Split('-');
                sclR = Scratio[0] + "\n" + Scratio[1];
            }
            row2Col3Table_3_2.AddCell(new PdfPCell(new Phrase(new Chunk("", fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col3Table_3_2.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_032 + (pdfSheetsList.Count() > 0 ? pdfSheetsList.Count() : 1), fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            row2Col3Table_3.AddCell(new PdfPCell(row2Col3Table_3_2) { BorderWidth = 0 });


            row2Col3Table.AddCell(new PdfPCell(row2Col3Table_3) { BorderWidth = 0 });

            footerTemplate.AddCell(new PdfPCell(row2Col3Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight * 3,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Rowspan = 3
            });

            #endregion

            #region ROW 2- COL4 Text
            PdfPTable row2Col4Table = new PdfPTable(new float[] { 20f, 80f });
            row2Col4Table.WidthPercentage = 100f;
            row2Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_017, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row2Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.plan, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            footerTemplate.AddCell(new PdfPCell(row2Col4Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            #endregion

            #region ROW 3- COL1 Text
            PdfPTable row3Col1Table = new PdfPTable(new float[] { 15f, 35f, 15f, 35f });
            row3Col1Table.WidthPercentage = 100f;
            row3Col1Table.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_GBL_GBL_GBL_027, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row3Col1Table.AddCell(new PdfPCell(new Phrase(new Chunk(MiscHelper.FormatDate(printMap.CreatedOn.ToString()), FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row3Col1Table.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_033, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row3Col1Table.AddCell(new PdfPCell(new Phrase(new Chunk(MiscHelper.FormatTime(printMap.CreatedOn.ToString()), FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            footerTemplate.AddCell(new PdfPCell(row3Col1Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            #endregion

            #region ROW 3- COL4 Text
            PdfPTable row3Col4Table = new PdfPTable(new float[] { 30f, 70f });
            row3Col4Table.WidthPercentage = 100f;
            row3Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_034, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row3Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.provDir, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            footerTemplate.AddCell(new PdfPCell(row3Col4Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            #endregion


            #region ROW 4- COL1 Text
            PdfPTable row4Col1Table = new PdfPTable(new float[] { 35f, 65f });
            row4Col1Table.WidthPercentage = 100f;
            row4Col1Table.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_009, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row4Col1Table.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.plottedBy, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            footerTemplate.AddCell(new PdfPCell(row4Col1Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            #endregion 

            #region ROW 4- COL4 Text
            PdfPTable row4Col4Table = new PdfPTable(new float[] { 25f, 22f, 28f, 22f });
            row4Col4Table.WidthPercentage = 100f;
            row4Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_035, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row4Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.FDCNo, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row4Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(Resources.Resources.SI_OSP_PM_NET_FRM_036, fonts)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            row4Col4Table.AddCell(new PdfPCell(new Phrase(new Chunk(printMap.printMapAttr.OLT, FontFactory.GetFont("Roboto", mapReport.FooterTemplate_FontSize, mapReport.footerTemplate_secondaryFontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });

            footerTemplate.AddCell(new PdfPCell(row4Col4Table)
            {
                FixedHeight = mapReport.FooterTemplate_RowHeight,
                BorderWidth = mapReport.ReportBorderWidth,
                BorderColor = new BaseColor(112, 112, 112),
                PaddingLeft = mapReport.FooterTemplate_insideMargin,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            #endregion

            //footerTemplate.AddCell("Row 3, Col 4");

            //footerTemplate.AddCell("Row 3, Col 1");
            //footerTemplate.AddCell("Row 3, Col 2");
            //footerTemplate.AddCell("Row 3, Col 3");
            //footerTemplate.AddCell("Row 3, Col 4");
            return footerTemplate;
        }

        public static BaseFont GetFonts()
        {
            BaseFont unicode = BaseFont.CreateFont();
            if (HttpContext.Current != null)
            {
                var FileName = HttpContext.Current.Server.MapPath("~/Content/LangFonts/" + HttpContext.Current.Session["fontName"] + ".ttf");
                if (File.Exists(FileName))
                {
                    unicode = BaseFont.CreateFont(HttpContext.Current.Server.MapPath("~/Content/LangFonts/" + HttpContext.Current.Session["fontName"] + ".ttf"), BaseFont.IDENTITY_H, true);
                }
            }
            return unicode;
        }

        public static PdfPCell GetFooterPdfPCell(MapReport mapReport, Phrase phrase = null)
        {
            return GetPdfPCell(mapReport.FooterTemplate_RowHeight, mapReport.ReportBorderWidth, mapReport.FooterTemplate_insideMargin, phrase);
        }
        public static PdfPCell GetPdfPCell(float height, float borderWidth, float padding, Phrase phrase = null)
        {
            PdfPCell cell = null;
            if (phrase != null)
            {
                cell = new PdfPCell(phrase)
                {
                    FixedHeight = height,
                    BorderWidth = borderWidth,
                    BorderColor = new BaseColor(112, 112, 112),
                    PaddingLeft = padding,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
            }
            else
            {
                cell = new PdfPCell()
                {
                    FixedHeight = height,
                    BorderWidth = borderWidth,
                    BorderColor = new BaseColor(112, 112, 112),
                    PaddingLeft = padding,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };

            }
            return cell;
        }

        public static PdfPTable GetLegendTable(MapReport mapReport, PrintMap printMap, List<LegendGroup> legendGroupData, bool isheaderRequired = true)
        {
            PdfPTable mainLegend = new PdfPTable(1);
            //Right side
            mapReport.TempTitleFontSize = mapReport.LegendTitle0_FontSize;
            mapReport.TempContentFontSize = mapReport.LegendContent0_FontSize;
            //PdfPTable mainLegend = new PdfPTable(1);
            mainLegend.WidthPercentage = 100f;

            mainLegend.SplitLate = false;
            if (isheaderRequired)
            {
                PdfPCell legendHeaderCell = new PdfPCell();
                mapReport.TempLegendPaddingTop = mapReport.LegendTitle0_FontSize / 2;
                mapReport.TempLegendPaddingBottom = mapReport.LegendTitle0_FontSize / 2;
                mapReport.TempLegendPaddingLeft = mapReport.LegendTitle0_FontSize / 2;

                legendHeaderCell.AddElement(AddLegendHeader("Legends", mapReport, true));
                legendHeaderCell.BorderWidth = 0;
                //legendHeaderCell.BorderWidthLeft = 0.2f;
                //legendHeaderCell.BorderWidthTop = 0.2f;
                mainLegend.AddCell(legendHeaderCell);
            }

            PdfPCell legendCell = new PdfPCell();
            legendCell.PaddingTop = mapReport.LegendTopMargin;
            legendCell.PaddingRight = mapReport.LegendRightMargin;
            legendCell.PaddingLeft = mapReport.LegendLeftMargin;
            legendCell.PaddingBottom = mapReport.MapBottomMargin;
            legendCell.BorderWidth = 0;
            //legendCell.BorderWidthLeft = 0.2f;
            var statusLegend = new List<LegendCell>
                {
                    new LegendCell {ImageUrl= "Planned.png", Text="Planned" },
                    new LegendCell { ImageUrl = "AsBuilt.png", Text = "As Built" },
                    new LegendCell { ImageUrl = "Dorment.png", Text = "Dormant" },

                };

            AddLegendEntity("", legendCell, statusLegend, 3, mapReport, 0);

            mapReport.TempTitleFontSize = mapReport.LegendTitle1_FontSize;
            mapReport.TempContentFontSize = mapReport.LegendContent1_FontSize;
            mapReport.TempLegendPaddingTop = mapReport.LegendHeaderPaddingTop_1;
            mapReport.TempLegendPaddingBottom = mapReport.LegendHeaderPaddingBottom_1;
            mapReport.TempLegendPaddingLeft = mapReport.LegendHeaderPaddingLeft_1;
            if (legendGroupData != null && legendGroupData.Count > 0)
                foreach (var group in legendGroupData)
                    AddLegendEntity(group.Group, legendCell, group.Legends, mapReport.LegendColumnSize, mapReport, 0.2f, false);
            mainLegend.AddCell(legendCell);
            return mainLegend;
        }


        //public static PdfPTable GetLegendTable(MapReport mapReport, PrintMap printMap, List<LegendGroup> legendGroupData, bool isheaderRequired = true)
        //{
        //    PdfPTable mainLegend = new PdfPTable(1);
        //    //Right side
        //    var NewmapReport = new MapReport();
        //    NewmapReport = MapReport.A4;

        //    mapReport.TempTitleFontSize = NewmapReport.LegendTitle0_FontSize;
        //    mapReport.TempContentFontSize = mapReport.LegendContent0_FontSize;
        //    //PdfPTable mainLegend = new PdfPTable(1);
        //    mainLegend.WidthPercentage = 100f;

        //    mainLegend.SplitLate = false;
        //    if (isheaderRequired)
        //    {
        //        PdfPCell legendHeaderCell = new PdfPCell();
        //        mapReport.TempLegendPaddingTop = NewmapReport.LegendHeaderPaddingTop_1;
        //        mapReport.TempLegendPaddingBottom = mapReport.LegendHeaderPaddingBottom_1;
        //        mapReport.TempLegendPaddingLeft = mapReport.LegendHeaderPaddingLeft_1;

        //        legendHeaderCell.AddElement(AddLegendHeader("Legends", mapReport, true));
        //        legendHeaderCell.BorderWidth = 0;
        //        //legendHeaderCell.BorderWidthLeft = 0.2f;
        //        //legendHeaderCell.BorderWidthTop = 0.2f;
        //        mainLegend.AddCell(legendHeaderCell);
        //    }

        //    PdfPCell legendCell = new PdfPCell();
        //    legendCell.PaddingTop = NewmapReport.LegendTopMargin;
        //    legendCell.PaddingRight = NewmapReport.LegendRightMargin;
        //    legendCell.PaddingLeft = NewmapReport.LegendLeftMargin;
        //    legendCell.PaddingBottom = NewmapReport.MapBottomMargin;
        //    legendCell.BorderWidth = 0;
        //    //legendCell.BorderWidthLeft = 0.2f;
        //    var statusLegend = new List<LegendCell>
        //        {
        //            new LegendCell {ImageUrl= "Planned.png", Text="Planned" },
        //            new LegendCell { ImageUrl = "AsBuilt.png", Text = "As Built" },
        //            new LegendCell { ImageUrl = "Dorment.png", Text = "Dormant" },

        //        };

        //    AddLegendEntity("", legendCell, statusLegend, 3, mapReport, 0);

        //    mapReport.TempTitleFontSize = NewmapReport.LegendTitle1_FontSize;
        //    mapReport.TempContentFontSize = NewmapReport.LegendContent1_FontSize;
        //    mapReport.TempLegendPaddingTop = NewmapReport.LegendHeaderPaddingTop_1;
        //    mapReport.TempLegendPaddingBottom = NewmapReport.LegendHeaderPaddingBottom_1;
        //    mapReport.TempLegendPaddingLeft = NewmapReport.LegendHeaderPaddingLeft_1;
        //    if (legendGroupData != null && legendGroupData.Count > 0)
        //        foreach (var group in legendGroupData)
        //            AddLegendEntity(group.Group, legendCell, group.Legends, NewmapReport.LegendColumnSize, mapReport, 0.2f, false);
        //    mainLegend.AddCell(legendCell);
        //    return mainLegend;
        //}
        public static double AddNavigationContent(MapReport mapReport, int printZoom, double mapDistance, Document pdfDoc, PdfWriter pdfWriter, float mapWidth, PrintMap printMap)
        {
            string scaleText = string.Format("{0} m ", SCALELIMIT[printZoom]);
            if (printZoom <= 13)
            {

                scaleText = string.Format("{0} km ", SCALELIMIT[printZoom] / 1000);
            }
            mapDistance *= 1000;
            float scaleWidth = (mapWidth / (float)mapDistance) * SCALELIMIT[printZoom];
            AddNavigation(pdfWriter, pdfDoc, mapReport, printMap, scaleText, scaleWidth, printZoom);
            return mapDistance;
        }

        public static string GetMapScaleRatio(int printZoom)
        {
            string scaleText = string.Format("1:{0} m ", SCALELIMIT[printZoom]);
            if (printZoom <= 13)
            {

                scaleText = string.Format("1:{0} km ", SCALELIMIT[printZoom] / 1000);
            }
            return scaleText;
        }


        public static PdfPTable CreateFooter(MapReport mapReport, float leftRightMargin, int pageNumber = 0)
        {
            PdfPTable footerTable = new PdfPTable(2);
            footerTable.WidthPercentage = 100f - leftRightMargin;
            footerTable.DefaultCell.BorderColor = BaseColor.WHITE;
            //PdfPCell footerTitleCell = new PdfPCell(new Paragraph(new Chunk("Powered by Lepton Software", FontFactory.GetFont("Roboto", mapReport.CreatedOnFontSize)))
            //{
            //    Alignment = Element.ALIGN_LEFT
            //});
            PdfPCell footerTitleCell = new PdfPCell(new Paragraph(new Chunk(" ", FontFactory.GetFont("Roboto", mapReport.CreatedOnFontSize)))
            {
                Alignment = Element.ALIGN_LEFT
            });
            footerTitleCell.BorderWidth = 0;
            footerTitleCell.FixedHeight = mapReport.MainHeaderSize;
            footerTitleCell.PaddingBottom = mapReport.MainHeaderBottomPadding;
            PdfPCell footerPageCell = new PdfPCell(new Phrase(string.Format("Page : {0}", pageNumber), FontFactory.GetFont("Roboto", mapReport.CreatedOnFontSize)));
            footerPageCell.BorderWidth = 0;
            footerPageCell.FixedHeight = mapReport.MainHeaderSize;
            footerPageCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            footerPageCell.VerticalAlignment = Element.ALIGN_TOP;
            footerPageCell.PaddingBottom = mapReport.MainHeaderBottomPadding;
            footerTable.AddCell(footerTitleCell);
            footerTable.AddCell(footerPageCell);
            return footerTable;
        }

        public static PdfPTable CreateHeader(string Title, MapReport mapReport, float leftRightMargin, PrintMap printMap)
        {
            PdfPTable headerTable = new PdfPTable(new float[] { 30, 40, 30 });
            headerTable.WidthPercentage = 100f - leftRightMargin;
            headerTable.DefaultCell.BorderColor = BaseColor.WHITE;

            // PdfPCell logoCell = new PdfPCell();
            string[] arr = printMap.ClientLogoImageBytesForWeb.Split(',');
            byte[] imageBytes = Convert.FromBase64String(arr[1]);
            Image logo = Image.GetInstance(imageBytes);
            logo.ScaleToFit(90f, 30f);
            logo.SetAbsolutePosition(0f, 0f);
            PdfPCell logoCell = new PdfPCell(logo);
            logoCell.Border = 0;
            logoCell.BorderWidthBottom = 1;
            logoCell.PaddingBottom = 5;
            logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell titleCell = new PdfPCell(new Paragraph(new Chunk(Title, FontFactory.GetFont("Roboto", mapReport.MainHeaderFontSize, Font.BOLD))))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE

            };
            titleCell.BorderWidth = 0;
            titleCell.FixedHeight = mapReport.MainHeaderSize;
            titleCell.PaddingBottom = mapReport.MainHeaderBottomPadding;

            // PdfPCell createdOnCell = new PdfPCell(new Phrase(string.Format("Created On : {0}", DateTimeHelper.Now.ToString("dd'-'MMM'-'yyyy hh:mm tt")), FontFactory.GetFont("Roboto", mapReport.CreatedOnFontSize)));

            ////krishna chaturvedi - Header datetime for all pages
            PdfPCell createdOnCell = new PdfPCell(new Phrase(string.Format("Created On : {0}", MiscHelper.FormatDateTime(printMap.CreatedOn.ToString())), FontFactory.GetFont("Roboto", mapReport.CreatedOnFontSize)));


            createdOnCell.BorderWidth = 0;
            createdOnCell.FixedHeight = mapReport.MainHeaderSize;
            createdOnCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            createdOnCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            createdOnCell.PaddingBottom = mapReport.MainHeaderBottomPadding;

            headerTable.AddCell(logoCell);
            headerTable.AddCell(titleCell);
            headerTable.AddCell(createdOnCell);
            return headerTable;
        }

        public static PdfPTable AddLegendHeader(string headerTitle, MapReport mapReport, bool isMainHeader = false)
        {
            PdfPTable header = new PdfPTable(1);
            //header.PaddingTop = mapReport.LegendPaddingTop;
            header.SpacingBefore = (mapReport.LegendMainHeaderSpacingBefore);
            header.SpacingAfter = (mapReport.LegendMainHeaderSpacingAfter);
            header.WidthPercentage = 100;
            if (!isMainHeader)
            {
                header.AddCell(new PdfPCell() { BorderWidth = 0, MinimumHeight = mapReport.TempLegendPaddingTop });
            }
            header.AddCell(new PdfPCell(new Phrase(new Chunk(headerTitle, FontFactory.GetFont("Roboto", mapReport.TempTitleFontSize, Font.BOLD))))
            {
                BorderWidth = 0,
                //PaddingTop = mapReport.TempLegendPaddingTop,
                PaddingBottom = mapReport.TempLegendPaddingBottom,
                PaddingLeft = mapReport.TempLegendPaddingLeft,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = isMainHeader ? new BaseColor(216, 216, 216) : new BaseColor(244, 244, 244)
            });
            return header;
        }
        public static PdfPCell AddLegendEntity(string headerTitle, PdfPCell legendCell, List<LegendCell> content, int contentCol, MapReport mapReport, float borderWidth = 1, bool isMainHeader = false)
        {
            if (!string.IsNullOrEmpty(headerTitle))
                legendCell.AddElement(AddLegendHeader(headerTitle, mapReport, isMainHeader));

            legendCell.AddElement(AddLegendContent(contentCol, content, mapReport, borderWidth));
            return legendCell;
        }
        public static PdfPTable AddLegendContent(int contentCol, List<LegendCell> allContent, MapReport mapReport, float borderWidth = 0)
        {
            PdfPTable contentTable = new PdfPTable(contentCol);
            contentTable.WidthPercentage = 100;
            contentTable.PaddingTop = mapReport.LegendHeaderPaddingTop_1;
            contentTable.SpacingAfter = mapReport.LegendMainHeaderSpacingAfter;
            int count = 0;
            foreach (var content in allContent)
            {
                var newCell = new PdfPCell();
                newCell.BorderWidth = borderWidth;
                newCell.BorderColor = new BaseColor(112, 112, 112);
                contentTable.AddCell(FillLegendCell(newCell, content, mapReport));
                count++;
                if (count % contentCol == 0)
                    count = 0;
            }
            if (count > 0)
                while (count < contentCol)
                {
                    var newCell = new PdfPCell(new Phrase(""));
                    newCell.BorderWidth = borderWidth;
                    newCell.BorderColor = new BaseColor(112, 112, 112);
                    contentTable.AddCell(newCell);
                    count++;
                }

            return contentTable;
        }

        public static PdfPCell FillLegendCell(PdfPCell cell, LegendCell legendCell, MapReport mapReport)
        {
            //string rootPath = @"E:\Projects\Bombay Gas\Source_Code\trunck\SmartInventory\SmartInventory\Content\images\icons\lib\Legend\";
            var content = new PdfPTable(new float[] { 20, 80 });
            content.SplitRows = false;
            content.WidthPercentage = 100f;
            try
            {

                if (!string.IsNullOrEmpty(legendCell.ImageUrl) && File.Exists(ImageDriectory + legendCell.ImageUrl))
                {
                    legendCell.ImageUrl = legendCell.ImageUrl.Replace(".svg", ".png");
                    Image imgCell = Image.GetInstance(ImageDriectory + legendCell.ImageUrl);
                    imgCell = ResizeImageKeepAspectRatio(imgCell, (int)mapReport.LegendImageWidth, (int)mapReport.LegendImageHeight);
                    content.AddCell(new PdfPCell(imgCell) { BorderWidth = 0, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                }
                else
                {
                    content.AddCell(new PdfPCell(new Phrase("")) { BorderWidth = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
                }
            }
            catch { }
            content.AddCell(new PdfPCell(new Phrase(new Chunk(legendCell.Text, FontFactory.GetFont("Roboto", mapReport.TempContentFontSize))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_BOTTOM });
            cell.AddElement(content);

            return cell;
        }
        public static Image ResizeImageKeepAspectRatio(Image source, int width, int height)
        {
            if (source.Width != width || source.Height != height)
            {
                // Resize image
                float sourceRatio = (float)source.Width / source.Height;

                // Scaling
                float scaling;
                float scalingY = (float)source.Height / height;
                float scalingX = (float)source.Width / width;
                if (scalingX < scalingY) scaling = scalingX; else scaling = scalingY;

                int newWidth = (int)(source.Width / scaling);
                int newHeight = (int)(source.Height / scaling);

                // Correct float to int rounding
                if (newWidth < width) newWidth = width;
                if (newHeight < height) newHeight = height;
                source.ScaleAbsolute(newWidth, newHeight);
                if (newWidth > width || newHeight > height)
                {
                    source.ScaleToFit(width, height);
                }
                //source.RotationDegrees = 90;
                ////if(rotateDegree!=0)
                ////{
                ////    source = RotateImage(source, 0.0);
                ////}
            }
            return source;
        }

        public static void AddNavigation(PdfWriter writer, Document document, MapReport mapReport, PrintMap printMap, string scaleText, float scaleWidth, int printZoom = 21)
        {
            PdfPTable iconTbl = new PdfPTable(1);
            PdfPCell iconCell = new PdfPCell();
            if (File.Exists(ImageDriectory + "north.png"))
            {
                //System.Drawing.Image test = System.Drawing.Image.FromFile(ImageDriectory + "north.svg");
                Image logo = Image.GetInstance(ImageDriectory + "north.png");
                logo.ScaleToFit(mapReport.NavigationIconWidth, mapReport.NavigationIconHeight);
                iconCell = new PdfPCell(logo) { BorderWidth = 0 };
            }
            iconTbl.AddCell(iconCell);
            iconTbl.TotalWidth = mapReport.NavigationIconWidth;
            mapReport.MapFooterZoomScaleHeight = printMap.isFooterTemplateEnabled ? mapReport.MapFooterZoomScaleHeight : 6;
            float zoomWidth = (mapReport.NavigationScaleWidth) * 0.5f;
            float scaleTextWidth = (mapReport.NavigationScaleWidth) * (printMap.mapScaleRatio.Length / (20.0f));
            float xPos = mapReport.ReportLeftMargin + mapReport.MapLeftMargin + mapReport.MapWidth - 5;
            float yPos = mapReport.ReportBottomMargin + mapReport.MapBottomMargin + mapReport.NavigationIconHeight + mapReport.NavigationFontSize + mapReport.MapFooterZoomScaleHeight + mapReport.FooterTemplateHeight;
            iconTbl.WriteSelectedRows(0, -1, xPos - (scaleWidth / 2) - (mapReport.NavigationIconWidth / 2), yPos + mapReport.MapHeight - mapReport.NavigationIconHeight - mapReport.MapTopMargin - mapReport.MainHeaderFontSize, writer.DirectContent);


            //PdfPTable contentTbl = new PdfPTable(new float[] { zoomWidth, scaleTextWidth });
            //contentTbl.TotalWidth = mapReport.NavigationScaleWidth;
            //contentTbl.TotalWidth = zoomWidth+ scaleTextWidth;
            PdfPTable contentTbl = new PdfPTable(new float[] { scaleTextWidth });
            contentTbl.TotalWidth = scaleTextWidth;
            /////////// Set Zoom level
            //PdfPCell zoomCell = new PdfPCell(new Paragraph(new Chunk(string.Format("(Zoom : {0})", printZoom), FontFactory.GetFont("Roboto", mapReport.NavigationFontSize, Font.NORMAL))))
            // {
            //    HorizontalAlignment = Element.ALIGN_LEFT,
            //    BorderWidth = 0,                
            //    VerticalAlignment = Element.ALIGN_TOP,
            //    BackgroundColor = new BaseColor(225, 225, 225)
            //};
            //contentTbl.AddCell(zoomCell);
            //PdfPCell zoomCell = new PdfPCell(new Paragraph(new Chunk("", FontFactory.GetFont("Roboto", mapReport.NavigationFontSize, Font.NORMAL))))
            //{
            //    HorizontalAlignment = Element.ALIGN_LEFT,
            //    BorderWidth = 0,
            //    VerticalAlignment = Element.ALIGN_TOP,
            //    BackgroundColor = new BaseColor(225, 225, 225)
            //};
            //contentTbl.AddCell(zoomCell);

            /////////////// Set map scale ratio

            ////PdfPCell unitCell = new PdfPCell(new Paragraph(new Chunk(string.Format("{0}", scaleText), FontFactory.GetFont("Roboto", mapReport.NavigationFontSize, Font.NORMAL))))
            PdfPCell unitCell = new PdfPCell(new Paragraph(new Chunk(printMap.pageScale == 0 ? "" : printMap.mapScaleRatio, FontFactory.GetFont("Roboto", mapReport.NavigationFontSize, Font.NORMAL))))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderWidth = 0,
                VerticalAlignment = Element.ALIGN_TOP,
                BackgroundColor = new BaseColor(225, 225, 225),
                NoWrap = true
            };
            contentTbl.AddCell(unitCell);
            contentTbl.WriteSelectedRows(0, -1, xPos - (mapReport.NavigationScaleWidth) - (scaleTextWidth - 70), yPos - mapReport.NavigationIconHeight, writer.DirectContent);
            //contentTbl.WriteSelectedRows(0, -1, xPos - (mapReport.NavigationScaleWidth) - (scaleWidth), yPos - mapReport.NavigationIconHeight, writer.DirectContent);
            ////////// Add scale cell
            //PdfPTable scaleTbl = new PdfPTable(1);
            //scaleTbl.TotalWidth = scaleWidth;
            //PdfPCell scaleCell = new PdfPCell(new Paragraph(new Chunk(string.Format(""), FontFactory.GetFont("Roboto", mapReport.NavigationFontSize, Font.NORMAL))))
            //{
            //    HorizontalAlignment = Element.ALIGN_LEFT,
            //    BorderWidthLeft = 1,
            //    BorderWidthRight = 1,
            //    BorderWidthBottom = 1,
            //    BorderWidthTop = 0,
            //    FixedHeight = mapReport.NavigationFontSize + 5,
            //    VerticalAlignment = Element.ALIGN_MIDDLE,
            //    // PaddingTop = mapReport.NavigationFontSize ,
            //    BackgroundColor = new BaseColor(225, 225, 225)
            //};
            //scaleTbl.AddCell(scaleCell);
            //scaleTbl.WriteSelectedRows(0, -1, xPos - (scaleWidth), yPos - mapReport.NavigationIconHeight /*- (mapReport.NavigationFontSize / 2)*/, writer.DirectContent);
        }
        public static PdfPCell getFooterTextWithIcon(PdfPCell cell, string ImgUrl, string Text, MapReport mapReport, BaseColor fontColor, float fontSize)
        {
            //string rootPath = @"E:\Projects\Bombay Gas\Source_Code\trunck\SmartInventory\SmartInventory\Content\images\icons\lib\Legend\";
            iTextSharp.text.Font font = new iTextSharp.text.Font(GetFonts(), fontSize, iTextSharp.text.Font.NORMAL, fontColor);
            var content = new PdfPTable(new float[] { 10, 80 });
            content.SplitRows = false;
            content.WidthPercentage = 100f;
            try
            {

                if (!string.IsNullOrEmpty(ImgUrl) && File.Exists(ImgUrl))
                {
                    Image imgCell = Image.GetInstance(ImgUrl);
                    imgCell = ResizeImageKeepAspectRatio(imgCell, (int)mapReport.FooterTemplate_CheckboxWidth, (int)mapReport.FooterTemplate_CheckboxHeight);
                    content.AddCell(new PdfPCell(imgCell) { BorderWidth = 0, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, PaddingTop = mapReport.FooterTemplate_CheckboxMargin });
                }
                else
                {
                    content.AddCell(new PdfPCell(new Phrase("")) { BorderWidth = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
                }
            }
            catch { }
            content.AddCell(new PdfPCell(new Phrase(new Chunk(Text, font)))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            cell.AddElement(content);

            return cell;
        }
        public static PdfPCell getFooterText(PdfPCell cell, string Text, MapReport mapReport, BaseColor fontColor, float fontSize)
        {
            var content = new PdfPTable(1);
            content.SplitRows = false;
            content.WidthPercentage = 100f;
            content.AddCell(new PdfPCell(new Phrase(new Chunk(Text, FontFactory.GetFont("Roboto", fontSize, fontColor))))
            { BorderWidth = 0, VerticalAlignment = Element.ALIGN_CENTER });
            cell.AddElement(content);
            return cell;
        }

        public static byte[] mergeMultiplePDFs(iTextSharp.text.Rectangle pageSize, string[] filePaths)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0))
                {
                    using (PdfCopy copy = new PdfCopy(pdfDoc, ms))
                    {
                        pdfDoc.Open();

                        foreach (string file in filePaths)
                        {
                            PdfReader reader = new PdfReader(File.ReadAllBytes(file));
                            // loop over the pages in that document
                            int n = reader.NumberOfPages;
                            for (int page = 0; page < n;)
                            {
                                copy.AddPage(copy.GetImportedPage(reader, ++page));
                            }
                        }
                        pdfDoc.Close();
                        copy.Close();
                    }
                }
                return ms.ToArray();
            }

        }

        //public static byte[] GetLegendPDF(List<LegendGroup> legendGroupData, MapReport mapReport, PrintMap printMap)
        //{

        //    using (MemoryStream stream = new System.IO.MemoryStream())
        //    {
        //        var NewmapReport = new MapReport();
        //        NewmapReport = MapReport.A4;
        //        Document pdfDoc = new Document(PageSize.A4, 0f, 0f, NewmapReport.MainHeaderSize, NewmapReport.ReportBottomMargin);


        //        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
        //        float leftRightMargin = ((NewmapReport.ReportLeftMargin + NewmapReport.ReportRightMargin) * 100 / PageSize.A4.Width);
        //        //Header Footer operation
        //        var reportPageEvent = new MapReportPageEvents()
        //        {
        //            mapReport = mapReport,
        //            LeftRightMargin = leftRightMargin,
        //            ReportTitle = "Legend",
        //            printMap = printMap,
        //            PageNumber = 1
        //        };
        //        pdfWriter.PageEvent = reportPageEvent;
        //        //OPEN PDFDOC OBJECT TO WRITE CONTENT

        //        mapReport.MainHeaderFontSize = NewmapReport.MainHeaderFontSize;
        //        mapReport.MainHeaderSize = NewmapReport.MainHeaderSize;
        //        mapReport.MainHeaderBottomPadding = NewmapReport.MainHeaderBottomPadding;
        //        mapReport.CreatedOnFontSize = NewmapReport.CreatedOnFontSize;
        //        pdfDoc.Open();

        //        PdfPTable parentTable = new PdfPTable(1);
        //        parentTable.PaddingTop = NewmapReport.MainHeaderSize;
        //        parentTable.SplitLate = false;

        //        parentTable.WidthPercentage = 100f - NewmapReport.LegendLeftMargin;
        //        PdfPTable legendTable = GetLegendTable(mapReport, printMap, legendGroupData, false);
        //        var height = legendTable.CalculateHeights();
        //        parentTable.AddCell(new PdfPCell(legendTable)
        //        {
        //            BorderWidth = NewmapReport.ReportBorderWidth,
        //            BorderColor = new BaseColor(112, 112, 112),
        //            Padding = NewmapReport.ReportCellMargin
        //        });
        //        pdfDoc.Add(parentTable);
        //        pdfWriter.CloseStream = false;
        //        pdfDoc.Close();
        //        return stream.ToArray();
        //    }
        //}

        public static byte[] GetLegendPDF(List<LegendGroup> legendGroupData, MapReport mapReport, PrintMap printMap)
        {

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                // Document pdfDoc = new Document(PageSize.A4, 0f, 0f, mapReport.MainHeaderSize / 2, mapReport.ReportBottomMargin / 2);

                Document pdfDoc = new Document(PageSize.A4, 0f, 0f, mapReport.MainHeaderSize, mapReport.ReportBottomMargin);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                float leftRightMargin = ((mapReport.ReportLeftMargin + mapReport.ReportRightMargin) * 100 / PageSize.A4.Width);
                //Header Footer operation
                var reportPageEvent = new MapReportPageEvents()
                {
                    mapReport = mapReport,
                    LeftRightMargin = leftRightMargin,
                    ReportTitle = "Legend",
                    printMap = printMap,
                    PageNumber = 1
                };
                pdfWriter.PageEvent = reportPageEvent;
                //OPEN PDFDOC OBJECT TO WRITE CONTENT
                pdfDoc.Open();

                PdfPTable parentTable = new PdfPTable(1);
                parentTable.PaddingTop = mapReport.MainHeaderSize;
                parentTable.SplitLate = false;

                parentTable.WidthPercentage = 100f - mapReport.LegendLeftMargin;
                PdfPTable legendTable = GetLegendTable(mapReport, printMap, legendGroupData, false);
                var height = legendTable.CalculateHeights();
                parentTable.AddCell(new PdfPCell(legendTable)
                {
                    BorderWidth = mapReport.ReportBorderWidth,
                    BorderColor = new BaseColor(112, 112, 112),
                    Padding = mapReport.ReportCellMargin
                });
                pdfDoc.Add(parentTable);
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                return stream.ToArray();
            }
        }



    }

    public class LegendCell
    {
        public string ImageUrl { get; set; }
        public string Text { get; set; }
    }
    public class LegendGroup
    {
        public string Group { get; set; }
        public List<LegendCell> Legends { get; set; }
    }

    public class MapReportPageEvents : PdfPageEventHelper
    {
        public int PageNumber { get; set; }
        public string ReportTitle { get; set; }
        public MapReport mapReport { get; set; }
        public float LeftRightMargin { get; set; }
        public PrintMap printMap { get; set; }
        public int PrintZoom { get; set; }
        public float MapWidth { get; set; }
        public double MapDistance { get; set; }
        //public override void OnStartPage(PdfWriter writer, Document doc)
        //{
        //    PageNumber++;
        //}

        public override void OnOpenDocument(PdfWriter writer, Document doc)
        {
            PdfPTable headerTbl = MapReport.CreateHeader(ReportTitle, mapReport, LeftRightMargin, printMap);
            headerTbl.TotalWidth = doc.PageSize.Width - mapReport.ReportLeftMargin - mapReport.ReportRightMargin;
            headerTbl.WriteSelectedRows(0, -1, mapReport.ReportLeftMargin, (doc.PageSize.Height), writer.DirectContent);

        }


        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfPTable footerTbl = MapReport.CreateFooter(mapReport, LeftRightMargin, PageNumber);
            footerTbl.TotalWidth = document.PageSize.Width - mapReport.ReportLeftMargin - mapReport.ReportRightMargin;
            footerTbl.WriteSelectedRows(0, -1, mapReport.ReportLeftMargin, mapReport.FooterHeight, writer.DirectContent);
            if (PrintZoom > 0 && PageNumber == 1 && printMap.pageScale == 0)
            {
                MapReport.AddNavigationContent(mapReport, PrintZoom, MapDistance, document, writer, MapWidth, printMap);
            }
            if (PrintZoom > 0 && printMap.pageScale != 0)
            {
                MapReport.AddNavigationContent(mapReport, PrintZoom, MapDistance, document, writer, MapWidth, printMap);
            }
            PageNumber++;
        }
    }

    public static class DistanceAlgorithm
    {
        const double PIx = 3.141592653589793;
        const double RADIUS = 6371;

        /// <summary>
        /// Convert degrees to Radians
        /// </summary>
        /// <param name="x">Degrees</param>
        /// <returns>The equivalent in radians</returns>
        public static double Radians(double x)
        {
            return x * PIx / 180;
        }

        /// <summary>
        /// Calculate the distance between two places.
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public static double DistanceBetweenPlaces(
            double lon1,
            double lat1,
            double lon2,
            double lat2)
        {
            double dlon = Radians(lon2 - lon1);
            double dlat = Radians(lat2 - lat1);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(lat1)) * Math.Cos(Radians(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * RADIUS;
        }

    }


}
