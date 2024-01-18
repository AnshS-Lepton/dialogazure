using BusinessLogics;
using Models;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;

namespace SmartInventory.Controllers
{
    public class RedlineController : Controller
    {
        // GET: RedLine
        public ActionResult ShowRedlineDashboard(RedlineFilter objRedLineFilter, int page = 0, string sort = "", string sortdir = "")
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
            objRedLineFilter.pageSize = 10;
            objRedLineFilter.userId = user_id;
            objRedLineFilter.currentPage = page == 0 ? 1 : page;
            objRedLineFilter.sort = sort;
            objRedLineFilter.orderBy = sortdir;
            User objUser = (User)Session["userDetail"];
            var result = new BLRedline().GetRedLine(objRedLineFilter);
            objRedLineFilter.lstNetworkTicket = result;
            if (result.Count > 0)
            {
                objRedLineFilter.totalRecord = objRedLineFilter.lstNetworkTicket.First().totalrecords;
                Session["viewRedlineDashboardFilter"] = objRedLineFilter;
                BindRedlineDropDown(objRedLineFilter.objRedline);
                return PartialView("_RedlineList", objRedLineFilter);
            }
            else
            {
                Session["viewRedlineDashboardFilter"] = objRedLineFilter;
                BindRedlineDropDown(objRedLineFilter.objRedline);
                return PartialView("_RedlineList", objRedLineFilter);
            }
        }

        public void BindRedlineDropDown(Redline objRedlineMaster)
        {
            objRedlineMaster.AssignedBy = new BLUser().GetAllUsers(Convert.ToInt32(Session["user_id"]));
        }


        public PartialViewResult AddRedlineDetails(RedlineMaster objRedlineMaster)
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
            if (objRedlineMaster.system_id > 0)
            {
                User objUser = (User)Session["userDetail"];
                List<dynamic> list = new List<dynamic>();
                list.Add(Convert.ToString(objRedlineMaster.assigned_to_id));
                objRedlineMaster = new BLRedline().GetRedlineById(objRedlineMaster.system_id);
                objRedlineMaster.assigned_by = new BLUser().GetUserDetailByID(objRedlineMaster.created_by).user_name;
                objRedlineMaster.status_id = new BLRedline().GetAssignedUsersById(objRedlineMaster.system_id, Convert.ToInt32(list[0])).status;
                objRedlineMaster.task_remarks = new BLRedline().GetAssignedUsersById(objRedlineMaster.system_id, Convert.ToInt32(list[0])).remarks;
                objRedlineMaster.lstAssignedUsers = list;
            }
            else
            {
                objRedlineMaster.assigned_by = new BLUser().GetUserDetailByID(user_id).user_name;
            }

            BindRedlineMasterDropDown(objRedlineMaster);
            return PartialView("_AddRedlineDetails", objRedlineMaster);
        }

        public void BindRedlineMasterDropDown(RedlineMaster objRedlineMaster)
        {
            objRedlineMaster.lstAssignedTo = new BLUser().GetUserByManagerId(1/*Convert.ToInt32(Session["user_id"])*/);
            objRedlineMaster.statusDropdown = new BLRedline().getStatusDropdown();
        }




        public void ExportRedline()
        {
            if (Session["viewRedlineDashboardFilter"] != null)
            {
                RedlineFilter objViewFilter = (RedlineFilter)Session["viewRedlineDashboardFilter"];
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                var exportData = new BLRedline().GetRedLine(objViewFilter);
                DataTable dtReport = new DataTable();
                dtReport = Utility.MiscHelper.ListToDataTable(exportData);
                dtReport.Columns["task_name"].SetOrdinal(0);
                dtReport.Columns["task_type"].SetOrdinal(1);
                dtReport.Columns["task_remarks"].SetOrdinal(2);
                dtReport.Columns["task_action"].SetOrdinal(3);
                dtReport.Columns["assigned_by"].SetOrdinal(4);
                dtReport.Columns["created_date"].SetOrdinal(5);
                dtReport.Columns["assigned_to"].SetOrdinal(6);
                dtReport.Columns["modified_date"].SetOrdinal(7);
                dtReport.Columns["task_status"].SetOrdinal(8);

                dtReport.Columns.Remove("AssignedBy");
                dtReport.Columns.Remove("AllUsers");
                dtReport.Columns.Remove("task_system_id");
                dtReport.Columns.Remove("assigned_by_id");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("action");
                dtReport.Columns.Remove("assigned_to_id");
                dtReport.Columns.Remove("status_id");
                dtReport.Columns.Remove("status");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("user_name");
                dtReport.Columns.Remove("remarks");
                dtReport.Columns.Remove("totalrecords");
                
                dtReport.Columns["task_name"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_FRM_149;
                dtReport.Columns["task_type"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_FRM_150;
                dtReport.Columns["task_remarks"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_279;
                dtReport.Columns["task_action"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_FRM_151;
                dtReport.Columns["assigned_by"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_011;
                dtReport.Columns["created_date"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_192;
                dtReport.Columns["assigned_to"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_105;
                dtReport.Columns["modified_date"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_193;
                dtReport.Columns["task_status"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_280;


                var Exportfilename = "Redline_Report";
                dtReport.TableName = "RedlineList";
                ExportRedline(dtReport, Exportfilename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        public ActionResult SaveRedline(RedlineMaster objRedlineMaster)
        {
            ModelState.Clear();
            objRedlineMaster.pageMsg = new PageMessage();
            if (TryValidateModel(objRedlineMaster))
            {
                var isNew = objRedlineMaster.system_id > 0 ? false : true;
                var resultItem = new BLRedline().SaveRedline(objRedlineMaster, Convert.ToInt32(Session["user_id"]));
                if (string.IsNullOrEmpty(resultItem.pageMsg.message))
                {
                    if (isNew)
                    {
                        objRedlineMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_278;
                        objRedlineMaster.pageMsg.status = "Save";
                    }
                    else
                    {
                        objRedlineMaster.pageMsg.message =Resources.Resources.SI_OSP_GBL_GBL_GBL_281;
                        objRedlineMaster.pageMsg.status = "Update";
                    }
                }
                else
                    objRedlineMaster.pageMsg.message = "Error while creating Redline!";
            }
            return Json(objRedlineMaster, JsonRequestBehavior.AllowGet);
        }


        private void ExportRedline(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }


        public ActionResult Redline_Entity_History(RedlineFilter objRedlineFilters, int task_id = 0, int page = 1, string sort = "", string sortdir = "")
        {
            objRedlineFilters.pageSize = 10;
            objRedlineFilters.currentPage = page == 0 ? 1 : page;
            objRedlineFilters.sort = sort;
            objRedlineFilters.sortdir = sortdir;
            //objRedlineFilters.userId = Convert.ToInt32(Session["user_id"]);
            // objRedlineFilters.entity_type = objRedlineFilters.entity_type == null ? "" : objRedlineFilters.entity_type;
            List<Dictionary<string, string>> lstRedlineEntityhistory = new BLRedline().Get_RedlineEntity_History(objRedlineFilters);
            Session["ExportRedlineEntity_History"] = objRedlineFilters;
            if (lstRedlineEntityhistory.Count > 1)
            {
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                foreach (Dictionary<string, string> dic in lstRedlineEntityhistory)
                {
                    var obj = (IDictionary<string, object>)new ExpandoObject();
                    foreach (var col in dic)
                    {
                        if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                        {
                            obj.Add(col.Key, col.Value);
                        }
                    }
                    objRedlineFilters.lstRedlineEntityHistory.Add(obj);
                }
                objRedlineFilters.totalRecord = Convert.ToInt32(lstRedlineEntityhistory.FirstOrDefault()["totalrecords"]);
                return PartialView("_RedlineEntityHistory", objRedlineFilters);
            }
            else
            {
                return PartialView("_RedlineEntityHistory", objRedlineFilters);
            }

        }

        public void ExportRedlineEntity_History()
        {
            if (Session["ExportRedlineEntity_History"] != null)
            {
                RedlineFilter objRedlineFilters = (RedlineFilter)Session["ExportRedlineEntity_History"];
                var exportData = new BLRedline().Get_RedlineEntity_History(objRedlineFilters);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(exportData);
                dtReport.Columns.Remove("totalrecords");
                dtReport.Columns.Remove("s_no");
                dtReport.Columns.Remove("system_id");
                dtReport.Columns["task_name"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_FRM_149;
                dtReport.Columns["task_type"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_FRM_150;
                dtReport.Columns["status"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_GBL_160;
                dtReport.Columns["created_on"].ColumnName = Resources.Resources.SI_OSP_BUL_NET_GBL_007;
                dtReport.Columns["user_name"].ColumnName = Resources.Resources.SI_OSP_BUL_NET_GBL_006;
                dtReport.Columns["remarks"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_006;
                var filename = "RedlineEntity_History";
                dtReport.TableName = "RedlineEntityHistoryList";
                ExportRedline(dtReport, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

        public ActionResult ShowLayerOnMap(int id)
        {
            var objLayer = new BLRedline().GetRedlineById(id);
            return Json(objLayer, JsonRequestBehavior.AllowGet);
        }
    }
}