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
        public static int EntitySearchLength = 0;
        public static int Bld_Buffer_Mtr = 0;
        public static int CableExtraLengthPercentage = 0;
        public static List<Models.FormInputSettings> formInputSettings { get; set; }
        public static List<Models.layerDetail> listLayerDetails { get; set; }
        public static int mobileAppLogoutTimeInSec = Convert.ToInt32(ConfigurationManager.AppSettings["mobileAppLogoutTimeInSec"].Trim());
        public static bool isApiRequestLogRequired = Convert.ToBoolean(ConfigurationManager.AppSettings["isApiRequestLogRequired"]);
        public static string validDocumentTypes = "";
        public static string validImageTypes = "";
        public static int MaxuploadFileSize = 0;
        public static double DefaultFloorHeight = 0;
        public static double DefaultFloorLength = 0;
        public static double DefaultFloorWidth = 0;
        public static int MaxFileCountLimit = 0;
        //public static double Floor_Length_Mtr = 0;
        //public static double Floor_Width_Mtr = 0;
        //public static double Floor_Height_Mtr = 0;
        public static double Shaft_Length_Mtr = 0;
        public static double Shaft_Width_Mtr = 0;
        public static bool isterminationpointenable = true;
        public static bool isLbInfoActionEnabled = true;
        public static bool IsGetAddressActionEnable = true;
        public static bool IsbldApprovedEnabled = true;
        public static bool IsAutoSurveyAssignedEnable = true;
        public static bool IsSignUpAllowed = true;
        //public static bool is2FAuthEnabled = false;
        public static bool isADOIDEnabled = false;
        public static bool isPRMSEnabled = false;
        //public static string SecoAuthURL = ConfigurationManager.AppSettings["SecoAuthURL"].ToString().Trim();
        

        public static int IsValidateJPBoundary = 0;
        public static bool IsDormantEnabled = true;
        public static bool IsWMSLayerLoadingEnabled = false;
        public static string GISAddAPIURL = "";
        public static string GISUpdateAPIURL = "";
        public static string CSANEXMLToNASURL = "";
        public static bool isDefaultSplitterAllowed = true;
        public static string GISCreateVersion = "";
        public static string GISPostVersion = "";

        public static int TicketAttachmentMaxSize = 0;
        public static string RestrictedTicketAttachments;
        public static string TicketReceiverMailId;
        public static string NEXMLImportToNAS = "";
        public static bool IsUserActivityLogEnabled = false;
        public static string Policies = ""; //Mayank
        public static string TermsConditions = ""; //Mayank
        public static int isPatternLockEnable = 0;
        public static int TileCacheFlag = 0;
        public static bool IsEntityNamePrefixAllow = true;
        public static int splitterTypeForFat = 0;
        public static int splitterTypeForFdc = 0;
        public static string DuctOffset = "";
        public static string MicroductOffset = "";
        public static bool isLDAPEnabled = false;

        public static void InitializeGlobalSettings()
        {
            var globalSettings = new BLGlobalSetting().GetGlobalSettings("WEB");
            formInputSettings = new BLFormInputSettings().getformInputSettings();
            listLayerDetails = new BLLayer().GetLayerDetails();
            foreach (var objSetting in globalSettings)
            {
                if (objSetting.key == "EntitySearchLength")
                    EntitySearchLength = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "BldBufferInMtr")
                    Bld_Buffer_Mtr = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "validDocumentTypes")
                {
                    validDocumentTypes = objSetting.value;
                }
                if (objSetting.key == "validImageTypes")
                {
                    validImageTypes = objSetting.value;
                }
                if (objSetting.key == "ExternalDataMaxFileSize")
                {
                    MaxuploadFileSize = Convert.ToInt32(objSetting.value);
                }
                //if (objSetting.key == "DefaultFloorLength")
                //    Floor_Length_Mtr = Convert.ToDouble(objSetting.value);

                //if (objSetting.key == "DefaultFloorWidth")
                //    Floor_Width_Mtr = Convert.ToDouble(objSetting.value);

                //if (objSetting.key == "DefaultFloorHeight")
                //    Floor_Height_Mtr = Convert.ToDouble(objSetting.value);
                if (objSetting.key == "DefaultShaftLength")
                    Shaft_Length_Mtr = Convert.ToDouble(objSetting.value);

                if (objSetting.key == "DefaultShaftWidth")
                    Shaft_Width_Mtr = Convert.ToDouble(objSetting.value);

                if (objSetting.key == "isterminationpointenable")
                {
                    isterminationpointenable = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "IsbldApprovedEnabled")
                {
                   
                    IsbldApprovedEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "DefaultFloorHeight")
                    DefaultFloorHeight = Convert.ToDouble(objSetting.value);
                if (objSetting.key == "DefaultFloorLength")
                    DefaultFloorLength = Convert.ToDouble(objSetting.value);
                if (objSetting.key == "DefaultFloorWidth")
                    DefaultFloorWidth = Convert.ToDouble(objSetting.value);

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
                if (objSetting.key == "isvalidatejpboundary")
                    IsValidateJPBoundary = Convert.ToInt32(objSetting.value);
                if (objSetting.key == "IsDormantEnabled")
                {
                    IsDormantEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "GISAddAPIURL")
                {
                    GISAddAPIURL = objSetting.value;
                }
                if (objSetting.key == "GISUpdateAPIURL")
                {
                    GISUpdateAPIURL = objSetting.value;
                }
                if (objSetting.key == "CSANEXMLToNASURL")
                {
                    CSANEXMLToNASURL = objSetting.value;
                }
                if (objSetting.key == "isDefaultSplitterAllowed")
                {
                    isDefaultSplitterAllowed = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "GISCreateVersion")
                {
                    GISCreateVersion = objSetting.value;
                }
                if (objSetting.key == "GISPostVersion")
                {
                    GISPostVersion = objSetting.value;
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
                if (objSetting.key == "NEXMLImportToNAS")
                {
                    NEXMLImportToNAS = objSetting.value;
                }
                if (objSetting.key == "IsUserActivityLogEnabled")
                {
                    IsUserActivityLogEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "MaxFileCountLimit")
                    MaxFileCountLimit = Convert.ToInt32(objSetting.value);
                //Mayank 
                if (objSetting.key == "Policies")
                {
                    Policies = objSetting.value;
                }
                if (objSetting.key == "TermsConditions")
                {
                    TermsConditions = objSetting.value;
                }
                if (objSetting.key == "isPatternLockEnable")
                {
                    isPatternLockEnable = Convert.ToInt32(objSetting.value);
                }
                if (objSetting.key == "IsAutoSurveyAssignedEnable")
                {
                    IsAutoSurveyAssignedEnable = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                
                if (objSetting.key == "IsSignUpAllowed")
                {

                    IsSignUpAllowed = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "TileCacheFlag")
                {

                    TileCacheFlag = Convert.ToInt32(objSetting.value);
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
                if (objSetting.key == "IsWMSLayerLoadingEnabled")
                {
                    IsWMSLayerLoadingEnabled = Convert.ToInt32(objSetting.value) == 0 ? false : true;
                }
                if (objSetting.key == "DuctOffset")
                {
                    DuctOffset = (objSetting.value);
                }
                if (objSetting.key == "MicroductOffset")
                {
                    MicroductOffset = (objSetting.value);
                }
                if (objSetting.key == "isLDAPEnabled")
                {
                    isLDAPEnabled = Convert.ToInt32(objSetting.value) == 1;
                }
            }

        }
    }
}