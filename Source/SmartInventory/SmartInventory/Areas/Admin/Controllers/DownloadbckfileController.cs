using Ionic.Zip;
using Ionic.Zlib;
using Models;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Threading;
using SmartInventory.Settings;
using System.Diagnostics;
using System.Data;
using BusinessLogics.Feasibility;
using BusinessLogics.Admin;
using Models.Admin;
using Utility;
using BusinessLogics;

namespace SmartInventory.Areas.Admin.Controllers
{
    public class DownloadbckfileController : Controller
    {
        //GET: Admin/Downloadbckfile
        [AdminOnly]
        [Authorize]
        [SessionExpire]
        [HandleException]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public void DownloadFiles(String FileType)
        {
            try
            {
                int username = Convert.ToInt32(Session["user_id"]);
                System.Diagnostics.Process pr = new System.Diagnostics.Process();
                pr.StartInfo.Arguments = FileType + " " + username;
                pr.StartInfo.FileName = ApplicationSettings.Backuputilitypath;
                pr.Start();
            }
            catch(Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadFiles Function", "DownloadBackFile", ex);
            }
           
        }
        [HttpPost]
        public JsonResult DwnldStatus()
        {
            try
            {
                List<downloadbckupfile> lst = new List<downloadbckupfile>();
                lst = new BLMisc().downloadstatusbck();
                return Json(new { lst = lst }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("Download Status Function", "DownloadBackFile", ex);
                return null;
            }
        }
        public ActionResult DownloadDBFiles(string filetype)
        {
            try
            {
                string filename = null;
                string file = filetype == "App_download" ? ApplicationSettings.Appbackup_path : ApplicationSettings.DBbackup_path; ;
                FileInfo fi = new FileInfo(file);
                string extn = fi.Extension;
                filename = "BackUP" + extn;
                return File(file, "application/bak", filename);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("DownloadDbFiles Function", "DownloadBackFile", ex);
                return null;
            }
        }
    }
}