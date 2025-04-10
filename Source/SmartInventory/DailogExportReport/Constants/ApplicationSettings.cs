namespace DailogExportReport.Constants
{
    internal class ApplicationSettings
    {
        public static string numberFormatType = System.Configuration.ConfigurationManager.AppSettings["numberFormatType"];

        public static string saarcExcelNumberFormat = System.Configuration.ConfigurationManager.AppSettings["SAARCExcelFormat"];
        public static string europeExcelNumberFormat = System.Configuration.ConfigurationManager.AppSettings["EuropeExcelFormat"];
        public static string AttachmentLocalPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"];
        public static int ExcelReportLimitCount = 0;

    }
}
