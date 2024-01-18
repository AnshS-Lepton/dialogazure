using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using Newtonsoft.Json;
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

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class HomeController : Controller
    {
        //
        // GET: /Admin/Dashboard/
        public ActionResult Index()
        {
     
            return View();
        }
        public string getDashboardData()
        {
            OSPHeader objOspHeader = new OSPHeader();
            objOspHeader.objuser = (User)Session["userDetail"];
            Dashboard objDashboard = new Dashboard();
            // Remove Condition by Nikhil Arora under guidance of Deepak Yadav on 03-08-2023
            //if (objOspHeader.objuser.is_admin_rights_enabled)
            //{
            //    objDashboard.CurrentActiveUsers = BusinessLogics.Admin.BLDashboard.Instance.getDashboardDetails(objOspHeader.objuser.user_id);
            //    objDashboard.LastMonthActiveUsers = BusinessLogics.Admin.BLDashboard.Instance.GetLastMonthLoggedInUsersWise(objOspHeader.objuser.user_id);
            //}
            //else
            //{
            //    objDashboard.CurrentActiveUsers = BusinessLogics.Admin.BLDashboard.Instance.CurrentActiveUsers();
            //    objDashboard.LastMonthActiveUsers = BusinessLogics.Admin.BLDashboard.Instance.LastMonthActiveUsers();
            //}

            objDashboard.CurrentActiveUsers = BusinessLogics.Admin.BLDashboard.Instance.getDashboardDetails(objOspHeader.objuser.user_id);
            objDashboard.LastMonthActiveUsers = BusinessLogics.Admin.BLDashboard.Instance.GetLastMonthLoggedInUsersWise(objOspHeader.objuser.user_id);
          
            return JsonConvert.SerializeObject(objDashboard);

        }
        public ActionResult HeaderInfo_Admin()
        {
            OSPHeader objOspHeader = new OSPHeader();
            BLMisc objBLLayer = new BLMisc();
            objOspHeader.objuser = (User)Session["userDetail"];
            if (objOspHeader.objuser.user_name.ToLower() == "administrator")
            {
                objOspHeader.objuser.lstUserModule= new BLMisc().GetAllModules();
            }
            else
            {
               
                var model = objBLLayer.GetRoleModule(objOspHeader.objuser.role_id).Select(x => x.Id).ToArray();
                objOspHeader.objuser.lstUserModule = new BLMisc().GetUserModuleMasterList().Where(x => x.type.ToUpper() == "ADMIN").ToList();
                var userModuleMapping = new BLUserModuleMapping().GetModuleMapping(objOspHeader.objuser.user_id);
                objOspHeader.objuser.lstUserModule.Where(x => model.Contains(x.Id)).ToList().ForEach(x => x.is_selected = true);
                objOspHeader.objuser.lstUserModule = objOspHeader.objuser.lstUserModule.Where(p => userModuleMapping.Any(p2 => p2.module_id == p.Id)).ToList();
            }
            return PartialView("_MainNavigation", objOspHeader);
        }
        public void Export_Dashboard_Data()
        {
            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();
            tbl = BusinessLogics.Admin.BLDashboard.Instance.getDashboardData();
            ds.Tables.Add(tbl);
            ExportData(ds, "Safaricom_Dashboard_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        }
        private void ExportData(DataSet dsReport, string fileName, bool isDataContainBarcode = false)
        {

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dsReport != null && dsReport.Tables.Count > 0)
                {
                    IWorkbook workbook = NPOIExcelHelper.DatasetToExcel("xlsx", dsReport, isDataContainBarcode);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        }
    }
}