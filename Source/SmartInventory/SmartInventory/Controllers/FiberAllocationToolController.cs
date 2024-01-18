using BusinessLogics;
using BusinessLogics.ISP;
using Models;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;
using System.Configuration;
using System.Net;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using Ionic.Zip;
using Lepton.Utility;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using BusinessLogics.Admin;
using Newtonsoft.Json;
using System.Dynamic;
using Models.ISP;
using Models.WFM;
using System.Threading;
using System.Threading.Tasks;
//using System.Threading.Tasks;

namespace SmartInventory.Views
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class FiberAllocationToolController : Controller
    {
        // GET: FibreAllocationTool
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetConnectionDetails(FATDetailprocess objfat, int systemId = 0, int page = 1, string sort = "", string sortdir = "", string p_connection_status = "")
        {
            BLMisc oBLMisc = new BLMisc();
            List<FATDetail> lstResult;
            var userdetails = (User)Session["userDetail"];
            FatProcessRunningStatus oFatProcessRunningStatus = new FatProcessRunningStatus();
            objfat.p_connection_status = "PERMANENT";
            objfat.pageSize = 10;
            objfat.currentPage = page == 0 ? 1 : page;
            objfat.sort = sort;
            objfat.sortdir = sortdir;
            if (objfat.systemId > 0)
            {
                TempData["NE_System_id1"] = objfat.systemId;
            }
            if (objfat.systemId == 0)
            {
                objfat.systemId = (int)TempData.Peek("NE_System_id1");
            }
            SubArea objSubArea = new BLMisc().GetEntityDetailById<SubArea>(objfat.systemId, EntityType.SubArea, userdetails.user_id);
            objfat.fsa_design_id = string.IsNullOrEmpty(objSubArea.gis_design_id) ? objSubArea.network_id : objSubArea.gis_design_id;
            objfat.fsa_name = objSubArea.subarea_name;
            //objfat.BackgroundProcessStatus = new BLFATConnection().GetSetBKGStatus(objfat.systemId, "SELECT", "Task is running in background, please wait few minuts...");
            oFatProcessRunningStatus = new BLFATConnection().GetSetBKGStatus(objfat.systemId, "SELECT", "Task is running in background, please wait few minuts...");
            objfat.BackgroundProcessStatus = oFatProcessRunningStatus.process_message;
            objfat.bt_lock = oFatProcessRunningStatus.bt_lock;
            lstResult = new BLFATConnection().GetConnectionDetails(objfat).ToList();
            if (!string.IsNullOrEmpty(sort))
            {
                System.Reflection.PropertyInfo prop = typeof(FATDetail).GetProperty(sort);
                if(sortdir == "ASC")
                    lstResult = lstResult.OrderBy(x => prop.GetValue(x)).ToList();
                else
                    lstResult = lstResult.OrderByDescending(x => prop.GetValue(x, null)).ToList();
            }
            objfat.lstfatdetails = lstResult;
            objfat.fsa_connection_status = objfat.lstfatdetails.Select(p => p.connection_status).FirstOrDefault();
            return PartialView("_FAT_Allocation", objfat);
        }

        [HttpPost]
        public ActionResult CreateSplicing(FATDetailprocess objfat, string commandName, string ActionName)
        {
            if (string.IsNullOrEmpty(commandName) && !string.IsNullOrEmpty(ActionName))
            {
                commandName = ActionName;
            }
            if (string.IsNullOrEmpty(commandName) && string.IsNullOrEmpty(ActionName))
            {
                commandName = "PageRefresh";
            }
            FatProcessRunningStatus oFatProcessRunningStatus = new FatProcessRunningStatus();
            objfat.p_connection_status = "PERMANENT";
            objfat.pageSize = 20;
            objfat.currentPage = 1;
            if (objfat.systemId > 0)
            {
                TempData["NE_System_id1"] = objfat.systemId;
            }
            if (objfat.systemId == 0)
            {
                objfat.systemId = (int)TempData.Peek("NE_System_id1");
            }
            var userdetails = (User)Session["userDetail"];
            if (commandName == "GenerateTempSplicing")
            {
                FatProcessSummary oFatProcessSummary = new FatProcessSummary();
                oFatProcessSummary.sub_area_system_id = objfat.systemId;
                oFatProcessSummary.sub_area_name = "";
                oFatProcessSummary.created_by = userdetails.user_id;
                oFatProcessSummary.created_on = DateTime.Now;
                oFatProcessSummary.modified_by = userdetails.user_id;
                oFatProcessSummary.modified_on = DateTime.Now;
                oFatProcessSummary.process_start_time = DateTime.Now;
                oFatProcessRunningStatus = new BLFATConnection().GetSetBKGStatus(objfat.systemId, "INSERT", "Request has been submitted and running in background. It may take 5-10 minutes. You can wait or come back after sometime.");
                objfat.BackgroundProcessStatus = oFatProcessRunningStatus.process_message;
                objfat.bt_lock = oFatProcessRunningStatus.bt_lock;
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    new BLFATConnection().CreateSplicing(oFatProcessSummary);
                }).ContinueWith(tsk =>
                {
                    tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("GenerateSplicing", "FiberAllocationTool", ex); return true; });
                }, TaskContinuationOptions.OnlyOnFaulted);
                //var oResult = new BLFATConnection().CreateSplicing(oFatProcessSummary);
                //objfat.fat_process_id= oResult.fat_process_id;                
            }
            else if (commandName == "AcceptTempSplicing")
            {
                if (new BLFATConnection().GetConnectionCount(objfat.systemId) > 0)
                {
                    objfat.message = "Tool will run on Green Field only";
                }
                else
                {
                    try
                    {
                        oFatProcessRunningStatus = new BLFATConnection().GetSetBKGStatus(objfat.systemId, "INSERT", "Request has been submitted and running in background. It may take few minutes. You can wait or come back after sometime.");
                        objfat.BackgroundProcessStatus = oFatProcessRunningStatus.process_message;
                        objfat.bt_lock = oFatProcessRunningStatus.bt_lock;
                        var oResult = new BLFATConnection().UpdateSplicingStatus(objfat.systemId, userdetails.user_id, "AcceptTempSplicing");
                        //objfat.message = oResult.MESSAGE;                        
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            new BLFATConnection().UpdatePortStatus(oResult.fat_process_id, "INSERT");
                        }).ContinueWith(tsk =>
                        {
                            tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("AcceptTempSplicing:UpdatePortStatus", "FiberAllocationTool", ex); return true; });
                        }, TaskContinuationOptions.OnlyOnFaulted);
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            new BLFATConnection().UpdatSnapCableEndPoint(oResult.fat_process_id);
                        }).ContinueWith(tsk =>
                        {
                            tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("AcceptTempSplicing:UpdatSnapCableEndPoint", "FiberAllocationTool", ex); return true; });
                        }, TaskContinuationOptions.OnlyOnFaulted);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogHelper.WriteErrorLog("AcceptTempSplicing", "FiberAllocationTool", ex);
                        throw;
                    }
                }
            }
            else if (commandName == "ResetTempSplicing")
            {
                oFatProcessRunningStatus = new BLFATConnection().GetSetBKGStatus(objfat.systemId, "INSERT", "Request has been submitted and running in background. It may take few minutes. You can wait or come back after sometime.");
                var oResult = new BLFATConnection().UpdateSplicingStatus(objfat.systemId, userdetails.user_id, "ResetTempSplicing");
                objfat.message = oResult.MESSAGE;
            }
            else if (commandName == "ResetFSASplicing")
            {
                try
                {
                    oFatProcessRunningStatus = new BLFATConnection().GetSetBKGStatus(objfat.systemId, "INSERT", "Request has been submitted and running in background. It may take few minutes. You can wait or come back after sometime.");
                    objfat.BackgroundProcessStatus = oFatProcessRunningStatus.process_message;
                    objfat.bt_lock = oFatProcessRunningStatus.bt_lock;
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        new BLFATConnection().UpdateSplicingStatus(objfat.systemId, userdetails.user_id, "ResetFSASplicing");
                    }).ContinueWith(tsk =>
                    {
                        tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("ResetFSASplicing", "FiberAllocationTool", ex); return true; });
                    }, TaskContinuationOptions.OnlyOnFaulted);                    
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.WriteErrorLog("ResetFSASplicing", "FiberAllocationTool", ex);
                    throw;
                }
            }
            else if (commandName == "PageRefresh")
            {
                oFatProcessRunningStatus = new BLFATConnection().GetSetBKGStatus(objfat.systemId, "SELECT", "Task is running in background, please wait few minuts...");
                objfat.BackgroundProcessStatus = oFatProcessRunningStatus.process_message;
                objfat.bt_lock = oFatProcessRunningStatus.bt_lock;
            }
            SubArea objSubArea = new BLMisc().GetEntityDetailById<SubArea>(objfat.systemId, EntityType.SubArea, userdetails.user_id);
            objfat.fsa_design_id = string.IsNullOrEmpty(objSubArea.gis_design_id) ? objSubArea.network_id : objSubArea.gis_design_id;
            objfat.fsa_name = objSubArea.subarea_name;           
            objfat.lstfatdetails = new BLFATConnection().GetConnectionDetails(objfat).ToList();
            objfat.fsa_connection_status = objfat.lstfatdetails.Select(p => p.connection_status).FirstOrDefault();
            return PartialView("_FAT_Allocation", objfat);
        }

        public ActionResult ExportTempSplicing(int id, string status)
        {
            var userdetails = (User)Session["userDetail"];
            string sDesignId = string.Empty;
            FATDetailprocess objfat = new FATDetailprocess();           
            objfat.systemId = id;
            objfat.p_connection_status = "Export";
            objfat.lstfatdetails = new BLFATConnection().GetConnectionDetails(objfat).ToList();
            SubArea objSubArea = new BLMisc().GetEntityDetailById<SubArea>(objfat.systemId, EntityType.SubArea, userdetails.user_id);
            sDesignId = string.IsNullOrEmpty(objSubArea.gis_design_id) ? objSubArea.network_id : objSubArea.gis_design_id;

            Dictionary<string, string> lstExportColumn = new Dictionary<string, string>();            
            lstExportColumn.Add("splitter_type", "Splitter Type");
            lstExportColumn.Add("fdc_name", "Splitter Name");
            lstExportColumn.Add("connection_ring_name", "Connection Type");
            lstExportColumn.Add("is_connected", "Is Spliced");
            lstExportColumn.Add("input_1_cable_category", "Input-1 Cable Type");
            lstExportColumn.Add("input_1_total_core", "Input-1 Cable Total Core");
            lstExportColumn.Add("input_1_cable_name", "Input-1 Cable");
            lstExportColumn.Add("input_1_tube_number", "Input-1 Cable Tube");
            lstExportColumn.Add("input_1_cable_fiber_no", "Input-1 Cable Core");
            lstExportColumn.Add("input_2_cable_category", "Input-2 Cable Type");
            lstExportColumn.Add("input_2_total_core", "Input-2 Cable Total Core");
            lstExportColumn.Add("input_2_cable_name", "Input-2 Cable");
            lstExportColumn.Add("input_2_tube_number", "Input-2 Cable Tube");
            lstExportColumn.Add("input_2_cable_fiber_no", "Input-2 Cable Core");
            lstExportColumn.Add("remark", "Remark");

            try
            {
                string[] ExportColName = lstExportColumn.Select(i => i.Key.ToString()).ToArray();
                //string sFSAId = string.Empty;
                DataTable dt1 = MiscHelper.ListToDataTable<FATDetail>(objfat.lstfatdetails);
                dt1.TableName = "SplicingDetails";
                if (dt1.Rows.Count > 0)
                {
                    DataView view = new DataView(dt1);
                    DataTable dt = view.ToTable(false, ExportColName);
                    foreach (var item in lstExportColumn)
                    {
                        dt.Columns[item.Key].ColumnName = item.Value;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        var filename = "SplicingDetails_" + sDesignId + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "_" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";
                        //commented and updated by priyanka
                        //string filepath = System.Web.HttpContext.Current.Server.MapPath("~/uploads/temp/") + filename;
                        //commented and updated by priyanka
                        string filepath = System.Web.HttpContext.Current.Server.MapPath(ApplicationSettings.DownloadTempPath) + filename;
                        string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, filepath);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                    }
                    else
                    {
                        return Json("File not Exists");
                    }
                }
                else
                {
                    return Json("File not Exists");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                ErrorLogHelper.WriteErrorLog("ExportTempSplicing", "FiberAllocationTool", ex);
                return null;
            }            
        }

        [HttpPost]
        public ActionResult RefreshMessage(int fsa_system_id)
        {
            FatProcessRunningStatus oFatProcessRunningStatus = new FatProcessRunningStatus();
            oFatProcessRunningStatus = new BLFATConnection().GetSetBKGStatus(fsa_system_id, "SELECT", "");
            JsonResponse<FatProcessRunningStatus> oJsonResponse = new Models.JsonResponse<FatProcessRunningStatus>();          
            oJsonResponse.status = "OK";
            oJsonResponse.message = "";
            oJsonResponse.result = oFatProcessRunningStatus;
            return Json(oJsonResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteMessage(int fsa_system_id)
        {
            FatProcessRunningStatus oFatProcessRunningStatus = new FatProcessRunningStatus();
            oFatProcessRunningStatus = new BLFATConnection().GetSetBKGStatus(fsa_system_id, "DELETE", "");
            JsonResponse<FatProcessRunningStatus> oJsonResponse = new Models.JsonResponse<FatProcessRunningStatus>();
            oJsonResponse.status = "OK";
            oJsonResponse.message = "";
            oJsonResponse.result = oFatProcessRunningStatus;
            return Json(oJsonResponse, JsonRequestBehavior.AllowGet);
        }
    }
}