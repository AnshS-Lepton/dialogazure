using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using BusinessLogics;


namespace SmartInventory.Settings
{
    public class ApplicationSettings
    {
        //To download backup---(by Navi)
        public static string Appbackup_path = "";
        public static string DBbackup_path = "";
        public static string Backuputilitypath = "";
        public static String BarcodeRegx = "";
        public static String BarcodeHint = "";
        public static String Barcodevariants = "";
        //---
        public static string numberFormatType = System.Configuration.ConfigurationManager.AppSettings["numberFormatType"];

        public static string saarcExcelNumberFormat = System.Configuration.ConfigurationManager.AppSettings["SAARCExcelFormat"];
        public static string europeExcelNumberFormat = System.Configuration.ConfigurationManager.AppSettings["EuropeExcelFormat"];

        public static string saarcExcelNumberFormatDecimal = System.Configuration.ConfigurationManager.AppSettings["SAARCExcelFormatDecimal"];
        public static string europeExcelNumberFormatDecimal = System.Configuration.ConfigurationManager.AppSettings["EuropeExcelFormatDecimal"];

        public static string AttachmentLocalPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentLocalPath"];
        public static string UtilizationIconURL = ConfigurationManager.AppSettings["UtilizationIconURL"];
        public static string KMLIconURL = ConfigurationManager.AppSettings["KMLIconURL"];
        public static string DownloadTempPath = ConfigurationManager.AppSettings["DownloadTempPath"];
        public static string MapClient = ConfigurationManager.AppSettings["MapClient"];
        public static string MapChannel = ConfigurationManager.AppSettings["MapChannel"];
        public static string MapKey = ConfigurationManager.AppSettings["MapKey"];
        public static string MapAuthType = ConfigurationManager.AppSettings["MapAuthType"];
        public static string MapServerURL = ConfigurationManager.AppSettings["mapServerURL"];
        public static bool isMapServerURLVirtual = Convert.ToBoolean(ConfigurationManager.AppSettings["isMapServerURLVirtual"]);
        public static string LocalMapServerURL = ConfigurationManager.AppSettings["localmapServerURL"];
        public static string MapDirPath = ConfigurationManager.AppSettings["mapDirPath"];
        public static string ADFSEndPoint = ConfigurationManager.AppSettings["ADFSEndPoint"].ToString().Trim();
        public static string ADFSRelPartyUri = ConfigurationManager.AppSettings["ADFSRelPartyUri"].ToString().Trim();
        public static string ADFSUserNamePreFix = ConfigurationManager.AppSettings["ADFSUserNamePreFix"].ToString().Trim();
        public static string ADFSAutheticationBasedOn = ConfigurationManager.AppSettings["ADFSAutheticationBasedOn"].ToString().Trim();
        public static string RegionProvinceShapeFilePath = ConfigurationManager.AppSettings["RegionProvinceShapeFilePath"].ToString().Trim();
        public static bool isStaticMapEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["isStaticMapEnabled"].ToString().Trim());
        public static string StaticMapURL = ConfigurationManager.AppSettings["StaticMapURL"].ToString().Trim();
        public static int PrintMapRatio = Convert.ToInt32(ConfigurationManager.AppSettings["PrintMapRatio"].ToString().Trim());
        public static string SmartFeasibilityURL = ConfigurationManager.AppSettings["SmartFeasibilityURL"].ToString().Trim();
        public static int uploadSummaryPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["uploadSummaryPageSize"].Trim());
        public static bool isMultiLoginAllowed = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isMultiLoginAllowed"]);
        public static string Environment = ConfigurationManager.AppSettings["Environment"].Trim();
        public static bool IsBomBoqEnabledForAll = true;
        // public static string DbProvider = ConfigurationManager.AppSettings["DbProvider"].Trim();
        public static string PrintLogTimeInterval = "";
        public static string AppVersion = "";
        public static int EntitySearchLength = 0;
        public static int Bld_Buffer_Mtr = 0;
        public static int ViewAdminDashboardGridPageSize = 0;
        public static int ViewAdminLayerStyleGridPageSize = 0;
        public static double Shaft_Length_Mtr = 0;
        public static double Shaft_Width_Mtr = 0;
        public static int BulkUploadMaxCount = 1000;
        public static double Floor_Length_Mtr = 0;
        public static double Floor_Width_Mtr = 0;
        public static double Floor_Height_Mtr = 0;
        public static int SurveyAssignmentGridPaging = 0;
        public static int BulkBuildingUploadMaxCount = 0;
        public static int BulkTicketUploadMaxCount = 0;
        public static int BulkFiberUploadMaxCount = 0;
        public static int MaxBulkAssignSurveyUser = 0;
        public static int MaxMultiCloneLimit = 5;
        public static int DefaultFloorHeight = 0;
        public static int DefaultFloorLength = 0;
        public static int DefaultFloorWidth = 0;
        public static string validDocumentTypes = "";
        public static string validDocumentTypesFetools = "";
        public static string validImageTypes = "";
        public static int ConnectionPathFinderGridPaging = 0;
        public static string validationNotForRFS_Status = "";
        public static int DefaultBulkOperationPaging = 0;
        public static int MaxFileUploadSizeLimit = 0; 
        public static int MaxFileCountLimit = 0;
        public static int MaxAdddocumentRowNumber = 0;

        //public static string allowedDocumentAttachmentType ="";
        //public static string allowedImageAttachmentType = "";

        public static int BulkConnectionUploadMaxCount = 0;
        public static string SplicingConnectionTemplate = "";
        public static string SplicingConnectionTemplateColumns = "";
        public static int CableExtraLengthPercentage = 0;
        public static int SplitCableBuffer = 0;
        public static int SplitDuctBuffer = 0;
        public static int SplitMicroductBuffer = 0;
        public static int SplitTrenchBuffer = 0;
        public static int LoopBuffer = 0;
        public static int AssociateEntityBuffer = 0;
        public static int nearByStructureBuffer = 0;
        public static int JobAssignmentGridPaging = 5;
        public static bool isterminationpointenable = true;
        public static string ApplicationName = ConfigurationManager.AppSettings["ApplicationName"].ToString().Trim();
        public static string ClientName = ConfigurationManager.AppSettings["ClientName"].ToString().Trim();
        public static int SplitISPCableBuffer = 0;
        public static int TraceISPCableBuffer = 0;
        public static double WorkspaceScale = 50;
        public static double WorkspaceCellSize = 50;
        public static double RoomCellSize = 100;
        public static double RoomScale = 1000;
        public static int BarcodeBulkExportLimit = 0;
        public static bool isTicketManagerEnabled = true;
        public static bool isSmartPlannerEnabled = true;
        public static bool isFeasibilityEnabled = true;
        public static bool IsOpenStreetMapEnabled = true;
        public static bool IsMapScaleDynamic = false;
        public static int IsCableExtraValidationRequired = 0;
		public static int IsCustomerExtraValidationRequired = 0;
		public static bool SpliceCablesTerminatedOnDevice = false;
        public static bool isSpliceCablesTerminatedOnDeviceEditable = true;
        public static List<Models.FormInputSettings> formInputSettings { get; set; }
        public static List<Models.layerDetail> listLayerDetails { get; set; }
        public static string rowMaxWidthLimit { get; set; }

        public static string CountryCenterLatLong { get; set; }

        public static int type_abbr_min_length = 0;
        public static int type_abbr_max_length = 0;
        public static double RackUnitSize = 50;
        public static bool IsLMCReportEnabled = true;
        public static bool IsEntityConversionEnable = true;
        public static bool IsDirectionServiceEnable = true;
        public static bool IsVisiblePrintLegendEntityCount { get; set; }
        public static int BufferNearbyFault = 0;
        public static int BulkPoleUploadMaxCount = 0;
        //public static bool isMultilingual;
        public static bool isMultilingual = Convert.ToBoolean(ConfigurationManager.AppSettings["isMultilingual"]);
        public static int ExportReportMaxRecord = 0;
        public static int RegProvExpMaxRecord = 0;
        public static int maxNotificationDurationInMonth = 0;
        public static int RegProvMaxUploadRecords = 0;
        public static int isRegionProvinceEdidAllowed = 0;
        public static double DefaultPODLength = 5;
        public static double DefaultPODWidth = 5;
        public static int ExternalDataMaxFileSize = 0;
        public static int maxExternalDataViewOption = 0;
        public static int FiberLinkBulkKMLExportLimit = 0;
        public static int MapRegionProvinceLimit = 0;
        public static string ClientLogoImageBytesForWeb = string.Empty;
        public static int MaxPDFWihoutThread = 1;
        public static int MaxPagePrintCount = 0;
        public static int DefaultMapZoomLevel = 0;
        public static string Currency = ConfigurationManager.AppSettings["currency"];
        public static string FTPAttachment = ConfigurationManager.AppSettings["FTPAttachment"];
        public static string FTPUserNameAttachment = ConfigurationManager.AppSettings["FTPUserNameAttachment"];
        public static string FTPPasswordAttachment = ConfigurationManager.AppSettings["FTPPasswordAttachment"];
        public static bool isClientNameRequiredOnLoginPage = Convert.ToBoolean(ConfigurationManager.AppSettings["isClientNameRequiredOnLoginPage"]);
        public static int layerOpacity = 70;
        public static int DefaultPortDimension { get; set; }
        public static string lBaseMaxWidthLimit { get; set; }
        public static string BufferMaxWidthLimit { get; set; }

        public static string CsvDelimiter { get; set; }
        public static bool isVerifyActionEnabled { get; set; }

        public static string dBLossUnit = "";
        public static string dBPowerUnit = "";

        public static int TrayRowCount { get; set; }
        //public static int StaticMapScale = Convert.ToInt32(ConfigurationManager.AppSettings["StaticMapScale"].ToString().Trim());
        public static double MinAutoPlanEndPointBuffer = 0.0;
        public static double MaxAutoPlanEndPointBuffer = 0.0;
        public static double DefaultAutoPlanEndPointBuffer = 0.0;
        public static double MaxAutoOffsetValue = 0.0;
        public static int IsNEBufferAllowed = 0;
        public static int NEDefaultBuffer = 0;
        public static int IsRegionProvinceAbbrShowInLeftPnl = 0;
        public static int SLDTypeSpider = 0;
        public static int MaxOffsetEntityAlongDirection = 0;
        public static int MaxOffsetNetworkPlanning = 0;
        public static int InfoToolZoom = 0;
        public static int DefaultTaskCheckinRadius = 0;
        public static string IduTransmitPower = "";
        public static string IduTransmittedFrequency = "";
        public static string IduReceivedFrequency = "";
        public static string IduBandwidth = "";
        public static string OduMinFrequencyReceived = "";
        public static string OduMinFrequencyTransmitted = "";
        public static string OduMaxFrequencyReceived = "";
        public static string OduMaxFrequencyTransmitted = "";
        public static int MaxLandbaseFileUploadSizeLimit = 0;
        public static string FTPServiceURL = "";
        public static string HelpDeskLink = "";
        public static double Minazimuth = 0.0;
        public static double Maxazimuth = 0.0;
        public static string ApplicationLogoImageBytesForWeb = string.Empty;
        public static bool IsDormantEnabled = true;

        public static bool is_otp_enabled_for_admin_web = false;
        //public static bool is2FAuthEnabled = false;
        public static bool isADOIDEnabled = false;
        public static bool isPRMSEnabled = false;
        public static string ClientFavicon = string.Empty;
        public static bool isbuisnesslayer = false;
        public static string ColorStyleSheetFilePath = "";
        public static bool IsMapScaleEnabled = true;
        public static string LoginText = "";
        public static string CopyRightText = "";
        public static bool IsNASEnabled = true;
        public static bool isExistingNetworkVisible = false;
        //public static string CountryAddr = "";
        public static string CountryAbbr = "";
        public static int TicketAttachmentMaxSize = 0;
        public static string RestrictedTicketAttachments;
        public static string TicketReceiverMailId;
        public static double customProjection = 0.0;
        public static bool IsSeprateLandingEnabled = false;
        public static bool IsCustomProjectionAllowed = false;
        public static string SAPPath = "";
        public static int NeXmlS2MaxLimit = 0;
        public static bool IsUserActivityLogEnabled = false;
        public static int InformationBuffer = 0;
        public static bool DeleteAuthorityPermission = false;
        public static string Policies = ""; //Mayank
        public static string TermsConditions = ""; //Mayank
        public static bool IsTrenchCustomerRequired = false;
        public static bool IS_HPSMEnabled = true; // Manoj
        public static bool IsEntityNamePrefixAllow = true;
        public static int TransmediaBuffer = 0;
		public static int MinBarcodeLength = 0;
		public static int MaxBarcodeLength = 0;
        public static int MaxMultiplePushToGis = 0;
        public static int NoHomePassMaxLength = 0;
		public static double MinMeterReading = 0.0;
		public static double MaxMeterReading = 0.0;
        public static int splitterTypeForFat = 0;
        public static int splitterTypeForFdc = 0;
        public static bool IsWMSLayerLoadingEnabled = true;
        public static bool IsVectorLayerEnabled = false;
        public static int IsSignalRIsEnable = 0;
		public static bool IsTraceEnabled = false;
		public static int ExcelReportLimitCount = 0;


        public static string OwnLDAPEndPoint = ConfigurationManager.AppSettings["OwnLDAPEndPoint"].ToString().Trim();
        public static string PartnerLDAPEndPoint = ConfigurationManager.AppSettings["PartnerLDAPEndPoint"].ToString().Trim();



        public static string smsusername = "";
        public static string smspassword = "";
        static string originator = "";
        static string smsapi = "";
        public static int MaxLineEntityLength = 0;
        public static int isCDBAttributeEnabled = 0;
        
        public static bool fetoolsenabled = true;
        public static int CableDefLabelMinZoom = 0;
        public static int IsGeometryUpdateOnAssociationAllowed = 0;
        public static bool CdbEnabled = false;
        public static int isLDAPEnabled = 0;
        public static string ApplicationVersion = "";
        public static void InitializeGlobalSettings()
        {            
            formInputSettings = new BLFormInputSettings().getformInputSettings();
            listLayerDetails = new BLLayer().GetLayerDetails();
            var globalSettings = new BLGlobalSetting().GetGlobalSettings("WEB");
            foreach (var objSetting in globalSettings)
            {
                if (objSetting.key == "AppVersion")
                    AppVersion = objSetting.value;
                if (objSetting.key == "fetoolsenabled")
                    fetoolsenabled =Convert.ToInt32(objSetting.value) == 1 ? true : false;
                if (objSetting.key == "EntitySearchLength")
                    EntitySearchLength = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "BldBufferInMtr")
                    Bld_Buffer_Mtr = Convert.ToInt32(objSetting.value);

                if (objSetting.key == "AdminDashboardGridPage")
                    ViewAdminDashboardGridPageSize = Convert.ToInt32(objSetting.value);

                if (objSetting.key == "AdminLayerStyleGridPage")
                    ViewAdminLayerStyleGridPageSize = Convert.ToInt32(objSetting.value);

                if (objSetting.key == "DefaultShaftLength")
                    Shaft_Length_Mtr = Convert.ToDouble(objSetting.value);

                if (objSetting.key == "DefaultShaftWidth")
                    Shaft_Width_Mtr = Convert.ToDouble(objSetting.value);

                if (objSetting.key == "DefaultFloorLength")
                    Floor_Length_Mtr = Convert.ToDouble(objSetting.value);

                if (objSetting.key == "DefaultFloorWidth")
                    Floor_Width_Mtr = Convert.ToDouble(objSetting.value);

                if (objSetting.key == "DefaultFloorHeight")
                    Floor_Height_Mtr = Convert.ToDouble(objSetting.value);
                if (objSetting.key == "SurveyAssignmentGridPaging")
                    SurveyAssignmentGridPaging = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "BulkSurveyUserAssignment")
                    MaxBulkAssignSurveyUser = Convert.ToInt32(objSetting.value);

                if (objSetting.key == "BulkBuildingUploadMaxCount")
                    BulkBuildingUploadMaxCount = Convert.ToInt32(objSetting.value);

                if (objSetting.key == "BulkTicketUploadMaxCount")
                    BulkTicketUploadMaxCount = Convert.ToInt32(objSetting.value);

                if (objSetting.key == "BulkFiberUploadMaxCount")
                    BulkFiberUploadMaxCount = Convert.ToInt32(objSetting.value);

                if (objSetting.key == "MultiCloneLimit")
                    MaxMultiCloneLimit = Convert.ToInt32(objSetting.value);

                if (objSetting.key == "DefaultFloorHeight")
                    DefaultFloorHeight = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "DefaultFloorLength")
                    DefaultFloorLength = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "DefaultFloorWidth")
                    DefaultFloorWidth = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "ConnectionPathFinderGridPaging")
                    ConnectionPathFinderGridPaging = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "validationNotForRFS_Status")
                    validationNotForRFS_Status = objSetting.value;
                if (objSetting.key == "DefaultBulkOperationPaging")
                    DefaultBulkOperationPaging = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "MaxFileUploadSizeLimit")
                    MaxFileUploadSizeLimit = Convert.ToInt32(objSetting.value); 

                    if (objSetting.key == "MaxFileCountLimit")
                    MaxFileCountLimit = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "validDocumentTypes")
                {
                    validDocumentTypes = objSetting.value;
                }
                if (objSetting.key == "validDocumentTypesFetools")
                {
                    validDocumentTypesFetools = objSetting.value;
                }
                
                if (objSetting.key == "validImageTypes")
                {
                    validImageTypes = objSetting.value;
                }

                if (objSetting.key == "BulkConnectionUploadMaxCount")
                    BulkConnectionUploadMaxCount = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "SplicingConnectionTemplateData")
                    SplicingConnectionTemplate = objSetting.value;
                if (objSetting.key == "SplicingConnectionTemplateColumns")
                    SplicingConnectionTemplateColumns = objSetting.value;
                if (objSetting.key == "CableExtraLengthPercentage")
                    CableExtraLengthPercentage = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "SplitCableBuffer")
                    SplitCableBuffer = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "SplitDuctBuffer")
                    SplitDuctBuffer = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "SplitMicroductBuffer")
                    SplitMicroductBuffer = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "LoopBuffer")
                    LoopBuffer = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "AssociateEntityBuffer")
                { AssociateEntityBuffer = Convert.ToInt32(objSetting.value); }
                if (objSetting.key == "StructureBuffer")
                {
                    nearByStructureBuffer = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "isterminationpointenable")
                {
                    isterminationpointenable = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "isTicketManagerEnabled")
                {
                    isTicketManagerEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "isSmartPlannerEnabled")
                {
                    isSmartPlannerEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "isFeasibilityEnabled")
                {
                    isFeasibilityEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "SplitISPCableBuffer")
                {
                    SplitISPCableBuffer = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "TraceISPCableBuffer")
                {
                    TraceISPCableBuffer = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "WorkspaceScale")
                {
                    WorkspaceScale = Convert.ToDouble(objSetting.value);
                }

                if (objSetting.key == "BarcodeBulkExportLimit")
                {
                    BarcodeBulkExportLimit = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "SpliceCablesTerminatedOnDevice")
                {
                    SpliceCablesTerminatedOnDevice = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }

                if (objSetting.key == "isSpliceCablesTerminatedOnDeviceEditable")
                {
                    isSpliceCablesTerminatedOnDeviceEditable = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }

                if (objSetting.key == "type_abbr")
                {
                    type_abbr_max_length = Convert.ToInt32(objSetting.max_value);
                    type_abbr_min_length = Convert.ToInt32(objSetting.min_value);

                }
                if (objSetting.key == "rowMaxWidthLimit")
                {
                    rowMaxWidthLimit = objSetting.value;
                }
                if (objSetting.key == "CountryCenterLatLong")
                {
                    CountryCenterLatLong = objSetting.value;
                }
                if (objSetting.key == "RackUnitSize")
                {
                    RackUnitSize = Convert.ToDouble(objSetting.value);
                }
                if (objSetting.key == "IsLMCReportEnabled")
                {
                    IsLMCReportEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "IsEntityConversionEnable")
                {
                    IsEntityConversionEnable = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "BulkPoleUploadMaxCount")
                {
                    BulkPoleUploadMaxCount = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "IsDirectionServiceEnable")
                {
                    IsDirectionServiceEnable = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "BufferNearbyFault")
                {
                    BufferNearbyFault = Convert.ToInt32(objSetting.value);
                }

                if (objSetting.key == "ExportReportMaxRecord")
                {
                    ExportReportMaxRecord = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "RegProvExpMaxRecord")
                {
                    RegProvExpMaxRecord = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "maxNotificationDurationInMonth")
                {
                    maxNotificationDurationInMonth = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "RegProvMaxUploadRecords")
                {
                    RegProvMaxUploadRecords = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "BulkUploadMaxCount")
                {
                    BulkUploadMaxCount = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "isEnableRegionProvinceBoundaryEdit")
                {
                    isRegionProvinceEdidAllowed = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "IsOpenStreetMapEnabled")
                {
                    IsOpenStreetMapEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "DefaultPODLength")
                {
                    DefaultPODLength = Convert.ToDouble(objSetting.value);
                }
                if (objSetting.key == "DefaultPODWidth")
                {
                    DefaultPODWidth = Convert.ToDouble(objSetting.value);
                }
                if (objSetting.key == "ExternalDataMaxFileSize")
                {
                    ExternalDataMaxFileSize = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "maxExternalDataViewOption")
                {
                    maxExternalDataViewOption = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "FiberLinkBulkKMLExportLimit")
                {
                    FiberLinkBulkKMLExportLimit = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "MapRegionProvinceLimit")
                {
                    MapRegionProvinceLimit = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "ClientLogoImageBytesForWeb")
                {
                    ClientLogoImageBytesForWeb = objSetting.value;
                }
                if (objSetting.key == "MaxPDFWihoutThread")
                {
                    MaxPDFWihoutThread = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "PrintLogTimeInterval")
                {
                    PrintLogTimeInterval = objSetting.value;
                }
                if (objSetting.key == "LandBaseMaxWidthLimit")
                {
                    lBaseMaxWidthLimit = objSetting.value;
                }
                if (objSetting.key == "BufferMaxWidthLimit")
                {
                    BufferMaxWidthLimit = objSetting.value;
                }

                if (objSetting.key == "MaxPagePrintCount")
                {
                    MaxPagePrintCount = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "CsvDelimiter")
                {
                    CsvDelimiter = objSetting.value;

                }
                if (objSetting.key == "DefaultMapZoomLevel")
                {
                    DefaultMapZoomLevel = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "isVerifyActionEnabled")
                {
                    isVerifyActionEnabled = Convert.ToInt32(objSetting.value) == 1;
                }
                if (objSetting.key == "LayerOpacity")
                {
                    layerOpacity = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "dBLossUnit")
                {
                    dBLossUnit = objSetting.value;
                }
                if (objSetting.key == "dBPowerUnit")
                {
                    dBPowerUnit = objSetting.value;
                }
                if (objSetting.key == "DefaultPortDimension")
                {
                    DefaultPortDimension = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "TrayRowCount")
                {
                    TrayRowCount = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "IsMapScaleDynamic")
                {
                    IsMapScaleDynamic = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }

                if (objSetting.key == "Appbackup_path")
                {
                    Appbackup_path = objSetting.value;
                }
                if (objSetting.key == "DBbackup_path")
                {
                    DBbackup_path = objSetting.value;
                }
                if (objSetting.key == "Backuputilitypath")
                {
                    Backuputilitypath = objSetting.value;
                }


                if (objSetting.key == "MinAutoPlanEndPointBuffer")
                {
                    MinAutoPlanEndPointBuffer = Convert.ToDouble(objSetting.value);
                }
                if (objSetting.key == "MaxAutoPlanEndPointBuffer")
                {
                    MaxAutoPlanEndPointBuffer = Convert.ToDouble(objSetting.value);
                }
                if (objSetting.key == "MaxAutoOffsetValue")
                {
                    MaxAutoOffsetValue = Convert.ToDouble(objSetting.value);
                }
                if (objSetting.key == "DefaultAutoPlanEndPointBuffer")
                {
                    DefaultAutoPlanEndPointBuffer = Convert.ToDouble(objSetting.value);
                }
                if (objSetting.key == "IsCableExtraValidationRequired")
                {
                    IsCableExtraValidationRequired = Convert.ToInt32(objSetting.value);
                }

				if (objSetting.key == "IsCustomerExtraValidationRequired")
				{
					IsCustomerExtraValidationRequired = Convert.ToInt32(objSetting.value);
				}
				if (objSetting.key == "IsNEBufferAllowed")
                {
                    IsNEBufferAllowed = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "NEDefaultBuffer")
                {
                    NEDefaultBuffer = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "IsRegionProvinceAbbrShowInLeftPnl")
                {
                    IsRegionProvinceAbbrShowInLeftPnl = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "IsSpiderDiagram")
                {
                    SLDTypeSpider = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "MaxOffsetEntityAlongDirection")
                {
                    MaxOffsetEntityAlongDirection = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "MaxOffsetNetworkPlanning")
                {
                    MaxOffsetNetworkPlanning = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "DefaultTaskCheckinRadius")
                    DefaultTaskCheckinRadius = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "InfoToolZoom")
                {
                    InfoToolZoom = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "IduTransmitPower")
                {
                    IduTransmitPower = objSetting.value;
                }
                if (objSetting.key == "IduTransmittedFrequency")
                {
                    IduTransmittedFrequency = objSetting.value;
                }
                if (objSetting.key == "IduReceivedFrequency")
                {
                    IduReceivedFrequency = objSetting.value;
                }
                if (objSetting.key == "IduBandwidth")
                {
                    IduBandwidth = objSetting.value;
                }
                if (objSetting.key == "OduMinFrequencyReceived")
                {
                    OduMinFrequencyReceived = objSetting.value;
                }
                if (objSetting.key == "OduMinFrequencyTransmitted")
                {
                    OduMinFrequencyTransmitted = objSetting.value;
                }
                if (objSetting.key == "OduMaxFrequencyReceived")
                {
                    OduMaxFrequencyReceived = objSetting.value;
                }
                if (objSetting.key == "OduMaxFrequencyTransmitted")
                {
                    OduMaxFrequencyTransmitted = objSetting.value;
                }
                if (objSetting.key == "MaxLandbaseFileUploadSizeLimit")
                {
                    MaxLandbaseFileUploadSizeLimit = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "HelpDeskLink")
                {
                    HelpDeskLink = objSetting.value;
                }
                if (objSetting.key == "FTPServiceURL")
                {
                    FTPServiceURL = objSetting.value;
                }
                if (objSetting.key == "Minazimuth")
                {
                    Minazimuth = Convert.ToDouble(objSetting.value);
                }
                if (objSetting.key == "Maxazimuth")
                {
                    Maxazimuth = Convert.ToDouble(objSetting.value);
                }
                if (objSetting.key == "ApplicationLogoImageBytesForWeb")
                {
                    ApplicationLogoImageBytesForWeb = objSetting.value;
                }
                if (objSetting.key == "IsDormantEnabled")
                {
                    IsDormantEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "is_otp_enabled_for_admin_web")
                {
                    is_otp_enabled_for_admin_web = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "isADOIDEnabled")
                {
                    isADOIDEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                //if (objSetting.key == "is2FAuthEnabled")
                //{
                //    is2FAuthEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                //}
                if (objSetting.key == "isPRMSEnabled")
                {
                    isPRMSEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }

                if (objSetting.key == "ClientFavicon")
                {
                    ClientFavicon = objSetting.value;
                }
                if (objSetting.key == "ColorStyleSheetFilePath")
                {
                    ColorStyleSheetFilePath = objSetting.value;
                }
                if (objSetting.key == "IsMapScaleEnabled")
                {
                    IsMapScaleEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "LoginText")
                {
                    LoginText = objSetting.value;
                }
                if (objSetting.key == "CopyRightText")
                {
                    CopyRightText = objSetting.value;
                }
                if (objSetting.key == "IsNASEnabled")
                {
                    IsNASEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "isExistingNetworkVisible")
                {
                    isExistingNetworkVisible = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }

                if (objSetting.key == "TicketAttachmentMaxSize")
                {
                    TicketAttachmentMaxSize = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "RestrictedTicketAttachments")
                {
                    RestrictedTicketAttachments = objSetting.value;
                }
                if (objSetting.key == "TicketReceiverMailId")
                {
                    TicketReceiverMailId = objSetting.value;
                }

                if (objSetting.key == "customProjection")
                {
                    customProjection = Convert.ToDouble(objSetting.value);
                }

                if (objSetting.key == "IsCustomProjectionAllowed")
                {
                    IsCustomProjectionAllowed = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "IsBomBoqEnabledForAll")
                {
                    IsBomBoqEnabledForAll = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "IsSeprateLandingEnabled")
                {
                    IsSeprateLandingEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "SAPPath")
                {
                    SAPPath = objSetting.value;
                }
                if (objSetting.key == "NeXmlS2MaxLimit")
                {
                    NeXmlS2MaxLimit = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "IsUserActivityLogEnabled")
                {
                    IsUserActivityLogEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "InformationBuffer")
                {
                    InformationBuffer = Convert.ToInt32(objSetting.value); 
                }
                if (objSetting.key == "DeleteAuthorityPermission")
                {
                    DeleteAuthorityPermission = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "IsTrenchCustomerRequired")
                {
                    IsTrenchCustomerRequired = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "SplitTrenchBuffer")
                {
                    SplitTrenchBuffer = Convert.ToInt32(objSetting.value);
                }
                //Mayank 
                if (objSetting.key == "Policies")
                {
                    Policies = objSetting.value;
                }
                if (objSetting.key == "TermsConditions")
                {
                    TermsConditions = objSetting.value;
                }
                if (objSetting.key == "IS_HPSMEnabled")
                {
                    IS_HPSMEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true; // Manoj
                }
                if (objSetting.key == "IsVisiblePrintLegendEntityCount")
                {

                    IsVisiblePrintLegendEntityCount = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "IsEntityNamePrefixAllow")
                {
                    IsEntityNamePrefixAllow = Convert.ToInt32(objSetting.value) == 1;
                }
                if (objSetting.key == "splitterTypeForFat")
                {
                    splitterTypeForFat = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "splitterTypeForFdc")
                {
                    splitterTypeForFdc = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "TransmediaBuffer")
                { TransmediaBuffer = Convert.ToInt32(objSetting.value); }
				if (objSetting.key == "MinBarcodeLength")
				{ MinBarcodeLength = Convert.ToInt32(objSetting.value); }
				if (objSetting.key == "MaxBarcodeLength")
				{ MaxBarcodeLength = Convert.ToInt32(objSetting.value); }
                if (objSetting.key == "BarcodeRegx")
                { BarcodeRegx = objSetting.value; }
                if (objSetting.key == "BarcodeHint")
                { BarcodeHint = objSetting.value; }
                if (objSetting.key == "Barcodevariants")
                { Barcodevariants = objSetting.value; }
                if (objSetting.key == "CountryAbbr")
                {
                    CountryAbbr = objSetting.value;
                }
                if (objSetting.key == "MaxMultiplePushToGis")
                {
                    MaxMultiplePushToGis = Convert.ToInt32(objSetting.value);
                }
				if (objSetting.key == "MinMeterReading")
				{ MinMeterReading = Convert.ToInt32(objSetting.value); }
				if (objSetting.key == "MaxMeterReading")
				{ MaxMeterReading = Convert.ToInt32(objSetting.value); }			
                if (objSetting.key == "NoHomePassMaxLength")
                { NoHomePassMaxLength = Convert.ToInt32(objSetting.value); }
                if (objSetting.key == "IsWMSLayerLoadingEnabled")
                {
                    IsWMSLayerLoadingEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "IsVectorLayerEnabled")
                {
                    IsVectorLayerEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "IsSignalRIsEnable")
                {
                    IsSignalRIsEnable = Convert.ToInt32(objSetting.value);
                }           
				if (objSetting.key == "IsTraceEnabled")
				{ IsTraceEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true; }

                if (objSetting.key == "isCDBAttributeEnabled")
                { CdbEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true; }
                if (objSetting.key == "ExcelReportLimitCount")
				{
					ExcelReportLimitCount = Convert.ToInt32(objSetting.value);
				}
                if (objSetting.key == "MaxLineEntityLength")
                {
                    MaxLineEntityLength = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "isCDBAttributeEnabled")
                {
                    isCDBAttributeEnabled = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "CableDefLabelMinZoom")
                {
                    CableDefLabelMinZoom = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "IsGeometryUpdateOnAssociationAllowed")
                {
                    IsGeometryUpdateOnAssociationAllowed = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "AdddocumentRowNumber")
                    MaxAdddocumentRowNumber = Convert.ToInt32(objSetting.value);

                //if (objSetting.key == "allowedDocumentAttachmentType")
                //{
                //    allowedDocumentAttachmentType = objSetting.value;
                //}
                //if (objSetting.key == "allowedImageAttachmentType")
                //{
                //    allowedImageAttachmentType = objSetting.value;
                //}
                if (objSetting.key == "isLDAPEnabled")
                {
                    isLDAPEnabled = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "ApplicationVersion")
                {
                    ApplicationVersion = objSetting.value;
                }
            }
        }
    }
}
