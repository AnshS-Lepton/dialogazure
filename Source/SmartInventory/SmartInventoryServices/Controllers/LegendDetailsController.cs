using BusinessLogics;
using Models;
using SmartInventory.Settings;
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;

namespace SmartInventoryServices.Controllers
{
    public class LegendDetailsController : Controller
    {
        // GET: LegendDetails
        public ActionResult GetLegendDetails(string JsonData)
        {
            BLUser objBLUser = new BLUser();
            try
            {
                string JonString = MiscHelper.DecodeTo64(JsonData).ToString();
                string jsonFormattedString = JonString.Replace("\\\"", "\"");
                string ADFSEndPoint = string.Empty;
                dynamic user = null;
                // var tmpObj = JObject.Parse(jsonFormattedString);
                GetLegendDetailsIn request = new JavaScriptSerializer().Deserialize<GetLegendDetailsIn>(jsonFormattedString);
                var Password = MiscHelper.EncodeTo64(request.password);
                //  var user = objDAUser.ChkUserExist(request.userName);
                ADFSEndPoint = System.Configuration.ConfigurationManager.AppSettings["ADFSEndPoint"].ToString().Trim();
                string sSmartInventoryServiceURL = Convert.ToString( System.Configuration.ConfigurationManager.AppSettings["SmartInventoryServiceURL"]).Trim();
                if (ApplicationSettings.isADOIDEnabled || !String.IsNullOrEmpty(ADFSEndPoint))
                {
                    
                    user = objBLUser.ValidateUser(request.userName, "" , UserType.Mobile.ToString());
                }
                else
                {
                    user = objBLUser.ValidateUser(request.userName,  Password , UserType.Mobile.ToString());
                }
                
                if (user != null)
                {
                    if (user.is_active == true)
                    {
                        Legend objLegend = new Legend();
                        objLegend.baseUrl = sSmartInventoryServiceURL;
                        //objLegend.baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                        objLegend.legendList = new BLMisc().GetMobileLegendDetail(user.user_id);
                        return View("_Legend", objLegend);
                    }
                    else
                    {
                        return View("_Unauthorize");
                    }
                }
                else
                {
                    return View("_Unauthorize");
                }
            }
            catch (Exception ex)
            {
                return View("_Error", new HandleErrorInfo(ex, "GetLegendDetails", "Legend Details"));
                throw;
            }
        }

        public ActionResult DownloadMapTiles(string JsonData)
        {
            BLUser objBLUser = new BLUser();
            try
            {
                string JonString = MiscHelper.DecodeTo64(JsonData).ToString();
                string jsonFormattedString = JonString.Replace("\\\"", "\"");
                DownloadMapTilesIn request = new JavaScriptSerializer().Deserialize<DownloadMapTilesIn>(jsonFormattedString);
                var Password = MiscHelper.EncodeTo64(request.password);
                var user = objBLUser.ValidateUser(request.userName, Password, UserType.Mobile.ToString());
                var FileName = new BLRegionProvince().GetRegionProvinceFileNameById(request.provinceId);
                if (user != null)
                {
                    if (user.is_active == true)
                    {
                        var folderPath = Server.MapPath(ConfigurationManager.AppSettings["MapTilesFolder"]);
                        var FileNameWithExtension = FileName + ".zip";
                        var path = folderPath+ FileNameWithExtension;
                        if (path != null && FileName != null)
                        {
                            //byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                            Response.Clear();
                            Response.ContentType = "application/octet-stream";
                            Response.AppendHeader("Content-Disposition", "filename=" + FileNameWithExtension);
                            Response.TransmitFile(path);
                            Response.End();
                            return new FilePathResult(path, FileNameWithExtension);
                            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileNameWithExtension);
                        }
                        else
                        {
                            return View("_Error", new { ErrorMessage = "File not found." });
                        }
                    }
                    else
                    {
                        return View("_Unauthorize");
                    }
                }
                else
                {
                    return View("_Unauthorize");
                }
            }
            catch (Exception ex)
            {
                return View("_Error", new HandleErrorInfo(ex, "DownloadMapTiles", "Download MapTiles"));
                throw;
            }
        }

    }
}