using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Web.Http;
using BusinessLogics;
using Ionic.Zip;
using Lepton.Entities;
using Models;
using SmartInventoryServices.Filters;
using Utility;

namespace SmartInventoryServices.Controllers
{
    [Authorize]
    [CustomAuthorization]
    [HandleException]
    
    public class MiscellaneousTasksController : ApiController
    {
        [CustomAction]
        [HttpGet]
        public ApiResponse<object> CheckGeomWithin(GeomCheck data)
        {
            var response = new ApiResponse<object>();
            try
            {
                bool chkWithin = BASaveEntityGeometry.Instance.CheckGeomWithin(data.geomA, data.geomB);
                response.status = StatusCodes.OK.ToString();
                response.error_message = "First Geom with in Second Geom";
                response.results = chkWithin;

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("CheckGeomWithin()", "MiscellaneousTasks Controller", data.geomA + " " + data.geomB, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        #region GET ICONS
        [HttpPost]
        public HttpResponseMessage GetIcons()
        {
            string jsonContent = "[{\"Message\":\"Something went wrong\"}]";
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                string directloc = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\images\\icons\\map\\");//@"D:\appbackupfile1";
                
                using (ZipFile zip = new ZipFile())
                {
                    zip.UseZip64WhenSaving = Zip64Option.Always;
                    foreach (var item in Directory.GetDirectories(directloc))
                    {
                        string folderName = new DirectoryInfo(item).Name;
                         zip.AddDirectory(item, folderName);
                    }
                    string zipName = "EntityIcon.zip";
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        zip.Save(memoryStream);
                        response.Content = new ByteArrayContent(memoryStream.ToArray());
                        response.Content.Headers.ContentLength = memoryStream.ToArray().LongLength;
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = zipName;
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("DownloadZipFile()", "ZipAPI Controller", "", ex);
                // response = this.Request.CreateResponse(HttpStatusCode.NotFound);
                response = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }
            return response;
        }
        #endregion
    }
}