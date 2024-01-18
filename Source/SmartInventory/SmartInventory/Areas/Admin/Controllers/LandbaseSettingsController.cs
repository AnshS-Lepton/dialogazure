using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using NPOI.SS.UserModel;
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

namespace SmartInventory.Areas.Admin.Controllers
{
    public class LandbaseSettingsController : Controller
    {
        // GET: Admin/LandbaseSettings
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddLayer(int id = 0)
        {
            LandBaseMaster obj = new LandBaseMaster();
            if (id != 0)
            {
                obj.lstLayers = new BLLandBaseSettings().GetLandBaseLayerDetailByID(id);
                foreach (var item in obj.lstLayers)
                {
                    obj.id = item.id;
                    obj.layer_name = item.layer_name;
                    obj.geom_type = item.geom_type;
                    obj.is_active = item.is_active;
                    obj.is_center_line_enable = item.is_center_line_enable;
                    obj.layer_abbr = item.layer_abbr;
                    obj.map_abbr = item.map_abbr;
                    obj.map_border_color = item.map_border_color;
                    obj.map_seq = item.map_seq;
                    obj.map_border_thickness = item.map_border_thickness;
                    obj.map_bg_color = item.map_bg_color;
                    obj.map_bg_opacity = item.map_bg_opacity;
                    obj.network_id_type = item.network_id_type;
                    obj.created_on = item.created_on;
                    obj.created_by = item.created_by;
                    obj.modified_on = item.modified_on;
                    obj.modified_by = item.modified_by;
                    obj.report_view_name = item.report_view_name;
                    obj.audit_view_name = item.audit_view_name;
                    obj.IsSubmit = item.IsSubmit;
                    obj.is_icon_display_enabled = item.is_icon_display_enabled;
                    obj.icon_name = item.icon_name;
                    obj.icon_path = item.icon_path;
                    obj.network_code_seperator = item.network_code_seperator;
                }
            }
            return View("AddLandbaseLayer", obj);
        }

        [HttpPost]
        public ActionResult SaveLayer(LandBaseMaster obj, FormCollection collection)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            int user_id = Convert.ToInt32(Session["user_id"]);
            if (Request.Files.Count > 0)
            {
                string path = ApplicationSettings.MapDirPath + "icons/Landbase";
                DirectoryInfo dic = new DirectoryInfo(path);
                if (!dic.Exists) {
                    dic.Create();
                }
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string FileName = file.FileName;
                    string strNewfilename = Path.GetFileNameWithoutExtension(collection["layer_name"].ToLower()) + Path.GetExtension(FileName);
                    obj.icon_name = strNewfilename;
                    obj.icon_path = "Landbase/" + strNewfilename;
                    var urlpath = Path.Combine(path, strNewfilename);
                    file.SaveAs(urlpath);
                }
            }
            else
            {
                obj.icon_name = collection["icon_name"];
                obj.icon_path = collection["icon_path"];
            }
            obj.id = Int32.Parse(collection["id"]);
            obj.layer_name = collection["layer_name"];
            obj.geom_type = collection["geom_type"];
            obj.layer_abbr = collection["layer_abbr"];
            obj.map_abbr = collection["map_abbr"];
            obj.map_border_color = collection["map_border_color"];
            obj.map_seq = Int32.Parse(collection["map_seq"]);
            obj.map_border_thickness = Int32.Parse(collection["map_border_thickness"]);
            obj.map_bg_color = collection["map_bg_color"];
            obj.map_bg_opacity = Int32.Parse(collection["map_bg_opacity"]);
            obj.network_id_type = collection["network_id_type"];
            obj.network_code_seperator = collection["network_code_seperator"];
            if (collection["is_active"].ToUpper() == "TRUE")
                obj.is_active = true;
            if (collection["is_icon_display_enabled"].ToUpper() == "TRUE")
                obj.is_icon_display_enabled = true;

            obj = new BLLandBaseSettings().SaveLandBaseLayer(obj, user_id);
            if (obj != null)
            {
                obj.IsSubmit = true;
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Data saved successfully.";
                obj.pageMsg = objMsg;

                obj.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
                obj.objGridAttributes.currentPage = 1;
                obj.objGridAttributes.sort = "";
                obj.objGridAttributes.orderBy = "";
                obj.lstLayers = new BLLandBaseSettings().GetLayerList(obj.objGridAttributes);
                obj.objGridAttributes.totalRecord = obj.lstLayers != null && obj.lstLayers.Count > 0 ? obj.lstLayers[0].totalRecords : 0;
                obj.pageMsg.status= ResponseStatus.OK.ToString();
                obj.pageMsg.message = "Data saved successfully.";

                //return View("ViewLandbaseLayers", obj);
                return Json(obj, JsonRequestBehavior.AllowGet);
                //return View("AddLandbaseLayer", obj);
            }
            else
            {
                objMsg.message = "Some error occured. Please contact administrator!";
                obj.pageMsg = objMsg;
                //return View("AddLandbaseLayer", obj);
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ViewLayers(LandBaseMaster obj, int page = 0, string sort = "", string sortdir = "")
        {
            if (sort != "" || page != 0)
            {
                obj.objGridAttributes = (CommonGridAttributes)Session["viewLandBaseSettingsDetails"];
            }
            obj.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            obj.objGridAttributes.currentPage = page == 0 ? 1 : page;
            obj.objGridAttributes.sort = sort;
            obj.objGridAttributes.orderBy = sortdir;
            obj.lstLayers = new BLLandBaseSettings().GetLayerList(obj.objGridAttributes);
            obj.objGridAttributes.totalRecord = obj.lstLayers != null && obj.lstLayers.Count > 0 ? obj.lstLayers[0].totalRecords : 0;
            Session["viewLandBaseSettingsDetails"] = obj.objGridAttributes;
            return View("ViewLandbaseLayers", obj);
           // return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public void DownloadDetail()
        {
            if (Session["viewLandBaseSettingsDetails"] != null)
            {
                CommonGridAttributes objViewFilter = (CommonGridAttributes)Session["viewLandBaseSettingsDetails"];
                List<LandBaseMaster> lstDetails = new List<LandBaseMaster>();
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstDetails = new BLLandBaseSettings().GetLayerList(objViewFilter);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<LandBaseMaster>(lstDetails);
                dtReport.Columns.Add("Status", typeof(string));
                dtReport.Columns.Add("Center Line Enable", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));

                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_active"]) == true ? "Active" : "InActive";
                    dtReport.Rows[i]["Center Line Enable"] = Convert.ToBoolean(dtReport.Rows[i]["is_center_line_enable"]) == true ? "Yes" : "No";
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.Columns.Remove("ID");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");
                dtReport.Columns.Remove("is_active");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("REPORT_VIEW_NAME");
                dtReport.Columns.Remove("AUDIT_VIEW_NAME");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("ISSUBMIT");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("lstLayers");
                dtReport.Columns.Remove("objGridAttributes");
                dtReport.Columns.Remove("is_center_line_enable");

                dtReport.Columns["s_no"].SetOrdinal(0);
                dtReport.Columns["layer_name"].SetOrdinal(1);
                dtReport.Columns["geom_type"].SetOrdinal(2);
                dtReport.Columns["layer_abbr"].SetOrdinal(3);
                dtReport.Columns["map_abbr"].SetOrdinal(4);
                dtReport.Columns["map_border_color"].SetOrdinal(5);
                dtReport.Columns["map_seq"].SetOrdinal(6);
                dtReport.Columns["status"].SetOrdinal(7);
                dtReport.Columns["map_border_thickness"].SetOrdinal(8);
                dtReport.Columns["map_bg_color"].SetOrdinal(9);
                dtReport.Columns["map_bg_opacity"].SetOrdinal(10);
                dtReport.Columns["Center Line Enable"].SetOrdinal(11);
                dtReport.Columns["network_id_type"].SetOrdinal(12);
                dtReport.Columns["network_code_seperator"].SetOrdinal(13);
                dtReport.Columns["created on"].SetOrdinal(14);
                dtReport.Columns["modified_by_text"].SetOrdinal(15);
                dtReport.Columns["modified on"].SetOrdinal(16);

                dtReport.Columns["layer_name"].ColumnName = "Layer Name";
                dtReport.Columns["geom_type"].ColumnName = "Geom Type";
                dtReport.Columns["layer_abbr"].ColumnName = "Layer Abbreviation";
                dtReport.Columns["map_abbr"].ColumnName = "Map Abbreviation";
                dtReport.Columns["map_border_color"].ColumnName = "Map Border Color";
                dtReport.Columns["map_seq"].ColumnName = "Map Sequence";
                dtReport.Columns["map_border_thickness"].ColumnName = "Map Border Thickness";
                dtReport.Columns["map_bg_color"].ColumnName = "Map Background Color";
                dtReport.Columns["map_bg_opacity"].ColumnName = "Map Background Opacity";
                //dtReport.Columns["is_center_line_enable"].ColumnName = "Is Center Line Enable";
                dtReport.Columns["network_id_type"].ColumnName = "Network Type";
                dtReport.Columns["network_code_seperator"].ColumnName = "Network Code Seperator";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";

                var filename = "LandBaseSettingsDetails";
                ExportDataTableToExcel(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }


        [HttpPost]
        public JsonResult GetLandBaseLayerSettingRowCount(int layer_id)
        {
            var result = new BLLandBaseSettings().GetLandBaseLayerSettingRowCount(layer_id);
            return Json(result[0], JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteLandBaseLayerSettingById(int id)
        {

            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BLLandBaseSettings().DeleteLandBaseSettingById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "LandBase Layer Deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting User!";
                }
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "LandBase Layer not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        public JsonResult IsDuplicate_abbrAvailable(string Layer_Abbr, string Map_Abbr, string Layer_name)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            List<LandBaseMaster> objDataList = new List<LandBaseMaster>();
            LandBaseMaster objDataExist = new LandBaseMaster();
            objDataExist.layer_name = Layer_name;
            objDataExist.layer_abbr = Layer_Abbr;
            objDataExist.map_abbr = Map_Abbr;
            try
            {
                var data = new BLLandBaseSettings().ChkDuplicate_abbrExist(objDataExist);
                if (data != null)
                {
                    objDataExist.pageMsg.status = ResponseStatus.ERROR.ToString();
                    if (data[0].layer_name.Trim().ToLower() == objDataExist.layer_name.ToLower())
                        objDataExist.pageMsg.message = "Layer Name already exist!";
                    else if (data[0].layer_abbr.Trim().ToLower() == objDataExist.layer_abbr.ToLower())
                        objDataExist.pageMsg.message = "Layer Abbreviation already exist!";
                    else if (data[0].map_abbr.Trim().ToLower() == objDataExist.map_abbr.ToLower())
                        objDataExist.pageMsg.message = "Map Abbreviation already exist!";
                }
            }
            catch (Exception)
            {
                objDataExist.pageMsg.status = "";
            }
            return Json(objDataExist.pageMsg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddLandBaseDropdownMaster(int id)
        {
            var obj = new ViewLandBaseDropdownMasterSettings();
            obj.lstLayers = new BLLandBaseLayer().getLandBaseLayers();

            if (id > 0)
            {
                ManageDropdownValues obj1 = new ManageDropdownValues();
                obj1 = new BLLandBaseSettings().GetLandBasedropdown_master_DetailByID(id);
                obj.landbaseCategoryList = new BLLandBaseLayer().GetLandbaseDropdown(obj1.landbase_layer_id, "category", 0);

                obj.landbase_layer_id = obj1.landbase_layer_id;
                obj.type = obj1.type;
                if (obj.type == "SubCategory")
                {
                    obj.category_id = obj1.parent_id;
                }
                obj.parent_id = obj1.parent_id;
                obj.value = obj1.value;
                obj.is_active = obj1.is_active;
            }
            else
            {
                obj.landbaseCategoryList = new BLLandBaseLayer().GetLandbaseDropdown(0, "category", 0);
            }
            return PartialView("_AddLandBaseDropdownMaster", obj);
        }

        [HttpPost]
        public JsonResult getCategoryByLayerId(int Layer_Id)
        {
            var result = new BLLandBaseLayer().GetLandbaseDropdown(Layer_Id, "category", 0);
            var list = result.DistinctBy(x => x.value);
            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveDropdownValues(ViewLandBaseDropdownMasterSettings obj)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            ManageDropdownValues obj1 = new ManageDropdownValues();
            obj1.id = obj.id;
            if (obj.id > 0)
            {
                var objData = obj1 = new BLLandBaseSettings().GetLandBasedropdown_master_DetailByID(obj.id);
                obj1.landbase_layer_id = objData.landbase_layer_id;
                obj1.type = objData.type;
                obj1.parent_id = objData.parent_id;
            }
            else
            {
                obj1.landbase_layer_id = obj.landbase_layer_id;
                obj1.type = obj.type;
                if (obj1.type == "SubCategory")
                {
                    obj1.parent_id = obj.category_id;
                }
                else
                {
                    obj1.parent_id = 0;
                }
            }

            obj1.value = obj.value;
            obj1.is_active = obj.is_active;

            int user_id = Convert.ToInt32(Session["user_id"]);

            int result = new BLLandBaseSettings().SaveDropdownValues(obj1, user_id);
            if (result == 1)
            {
                objMsg.message = "Record saved successfully!";
                objMsg.status = ResponseStatus.OK.ToString();
            }
            obj.type = "";
            obj.value = "";
            obj.OldValue = "";
            obj.id = 0;
            obj.landbase_layer_id = 0;
            obj.is_active = false;
            obj.landbaseCategoryList = new BLLandBaseLayer().GetLandbaseDropdown(0, "category", 0);
            obj.pageMsg = objMsg;
            ManageDropdownValues(obj);
            return PartialView("_AddLandBaseDropdownMaster", obj);
            //return View("ManageDropdownValues", obj);

        }

        public ActionResult ManageDropdownValues(ViewLandBaseDropdownMasterSettings obj, int page = 0, string sort = "", string sortdir = "")
        {
            if (sort != "" || page != 0)
            {
                obj.objdrpFilter = (ViewLandBaseDropdownMasterSettingsFilter)Session["viewDropdownFilters"];
                obj.landbase_layer_id = obj.objdrpFilter.layer_id;
                obj.type = obj.objdrpFilter.type;
                obj.status = obj.objdrpFilter.is_active;
                obj.value = obj.objdrpFilter.value == null ? obj.objdrpFilter.value : obj.objdrpFilter.value.Trim();
            }
            else
            {
                obj.objdrpFilter.id = 0;
                obj.objdrpFilter.layer_id = obj.landbase_layer_id;
                obj.objdrpFilter.type = obj.type;
                obj.objdrpFilter.is_active = obj.status;
                obj.objdrpFilter.value = obj.value == null ? obj.value : obj.value.Trim();
            }

            obj.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
            obj.objdrpFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            obj.objdrpFilter.currentPage = page == 0 ? 1 : page;
            obj.objdrpFilter.sort = sort;
            obj.objdrpFilter.orderBy = sortdir;
            obj.lstViewDropdownMasterSetting = new BLLandBaseSettings().GetDropdownMasterSettingsList(obj.objdrpFilter);
            obj.objdrpFilter.totalRecord = obj.lstViewDropdownMasterSetting.Count > 0 ?
                obj.lstViewDropdownMasterSetting[0].totalrecords : 0;
            Session["viewDropdownFilters"] = obj.objdrpFilter;
            return View(obj);
        }

        [HttpGet]
        public void ExportDropdownMasterDetails()
        {
            if (Session["viewDropdownFilters"] != null)
            {
                ViewLandBaseDropdownMasterSettingsFilter objdropFilter = (ViewLandBaseDropdownMasterSettingsFilter)Session["viewDropdownFilters"];
                List<ManageDropdownValues> lstViewdropDetails = new List<ManageDropdownValues>();
                objdropFilter.currentPage = 0;
                objdropFilter.pageSize = 0;
                lstViewdropDetails = new BLLandBaseSettings().GetDropdownMasterSettingsList(objdropFilter);

                DataTable dtReport = new DataTable();

                dtReport = MiscHelper.ListToDataTable<ManageDropdownValues>(lstViewdropDetails);
                dtReport.TableName = "DropdownMaster_Details";
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));
                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                }
                dtReport.DefaultView.Sort = "S_NO asc";
                dtReport = dtReport.DefaultView.ToTable();

                dtReport.Columns["S_NO"].SetOrdinal(0);

                dtReport.Columns.Remove("ID");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("landbase_layer_id");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("modified_by");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("modified_on");
                dtReport.Columns.Remove("OLDVALUE");
                dtReport.Columns.Remove("CATEGORY_ID");
                dtReport.Columns.Remove("PARENT_ID");
                dtReport.Columns.Remove("IS_ACTIVE");
                dtReport.Columns.Remove("ISSUBMIT");
                dtReport.Columns.Remove("PAGEMSG");
                dtReport.Columns.Remove("LSTLAYERS");
                dtReport.Columns.Remove("LANDBASECATEGORYLIST");

                dtReport.Columns["S_NO"].ColumnName = "Sr.No";
                dtReport.Columns["layer_name"].ColumnName = "Entity Name";
                dtReport.Columns["type"].ColumnName = "Type";
                dtReport.Columns["value"].ColumnName = "Value";
                dtReport.Columns["status"].ColumnName = "Status";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";



                var filename = "LandbaseDropdownDetails";
                ExportDataTableToExcel(dtReport, "Export_" + filename + "_" +
                    DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        private void ExportDataTableToExcel(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }


        [HttpPost]
        public JsonResult GetLandBaseDropdownMasterRowCount(int layer_id, int id, string layer_name, string fieldname, string value)
        {
            var result = new BLLandBaseSettings().GetLandBaseDropdownMasterRowCount(layer_id, id, layer_name, fieldname, value);
            return Json(result[0], JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteLandBaseDropdownMasterById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BLLandBaseSettings().DeleteLandBaseDropdownMasterById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "LandBase Layer Deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting User!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "LandBase Layer not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsDuplicate_Value_abbrAvailable(int id,int Layer_id, string type, string Dvalue)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            List<ManageDropdownValues> objDataList = new List<ManageDropdownValues>();
            ManageDropdownValues objDataExist = new ManageDropdownValues();
            objDataExist.id = id;
            objDataExist.landbase_layer_id = Layer_id;
            objDataExist.type = type;
            objDataExist.value = Dvalue;
            try
            {
                var data = new BLLandBaseSettings().ChkDuplicate_ValueExist(objDataExist);
                if (data.Count()>0)
                {
                    int dbId = Convert.ToInt32(data[0].id);
                  //  string cate  = Convert.ToString(data[0].type);
                    if (dbId != id && data != null)
                    { 
                            objDataExist.pageMsg.status = ResponseStatus.ERROR.ToString();
                            if (data[0].value.Trim().ToLower() == objDataExist.value.ToLower())
                                objDataExist.pageMsg.message = "Value already exist!";
 
                    }
                }
                 
                //if (data != null)
                //{
                //    objDataExist.pageMsg.status = ResponseStatus.ERROR.ToString();
                //    if (data[0].value.Trim().ToLower() == objDataExist.value.ToLower())
                //        objDataExist.pageMsg.message = "Value already exist!";

                //}
            }
            catch (Exception ex)
            {
                objDataExist.pageMsg.status = "";
            }
            return Json(objDataExist.pageMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LandBaseLayerSearchSetting(int id = 0)
        {
            SearchLandBaseLayerSetting obj = new SearchLandBaseLayerSetting();
            obj.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
            if (id > 0)
            {
                obj.lstSearchAttributes = BLSearchLandBaseLayerSetting.Instance.GetLayerSearchAttributes(id);
                obj.landbase_layer_id = id;
            }
            return View("SearchLandBaseLayerSetting", obj);
        }
        [HttpPost]
        public ActionResult LandBaseLayerSaveSearchSettings(SearchLandBaseLayerSetting objSearchSettings)
        {
            PageMessage objMsg = new PageMessage();

            // prepare comma seperated selected columns list
            if (objSearchSettings != null && objSearchSettings.lstSearchAttributes != null & objSearchSettings.lstSearchAttributes.Count > 0)
            {
                var arrSelectedColumns = objSearchSettings.lstSearchAttributes.Where(m => m.is_selected == true).Select(s => s.column_name).ToArray();
                if (arrSelectedColumns.Length > 0)
                {
                    objSearchSettings.search_columns = arrSelectedColumns != null ? string.Join(",", arrSelectedColumns) : "";
                    // update layer attributes in Search setting table...
                    objSearchSettings.user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);
                    objSearchSettings = BLSearchLandBaseLayerSetting.Instance.SaveSearchSetting(objSearchSettings);
                    if (objSearchSettings != null)
                    {
                        //success
                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.message = "Search settings updated successfully!";
                    }
                    else
                    {
                        //failure
                        objMsg.status = ResponseStatus.FAILED.ToString();
                        objMsg.message = "Error in updating Search settings!";
                    }
                }
                else
                {
                    //failure
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "No attribute selected for entity!";
                }
            }
            else
            {
                //failure
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Error in updating Search settings!";
            }

            objSearchSettings.pageMsg = objMsg;
            objSearchSettings.lstLayers = new BLLandBaseLayer().getLandBaseLayers();

            return View("SearchLandBaseLayerSetting", objSearchSettings);
        }

        #region LABEL SETTINGS
        public ActionResult LandbaselabelSettings(int id = 0)
        {
            Landbase_label_Settings  objLabelSettings = new Landbase_label_Settings();
            BindLandBaseAttribute(id);
            if (id != 0)
            {
                objLabelSettings.lstLabelAttributes = BLLandBaseLabelSetting.Instance.GetLayerLabelAttributes(id);
                objLabelSettings.landbase_layer_id = id;
            }
            objLabelSettings.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
            return View("LandBaseLabelSettings", objLabelSettings);
        } 
        public JsonResult BindLandBaseAttributOnChange(int layer_id)
        {
            var objResp = BindLandBaseAttribute(layer_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public IList<AttributeDetail> BindLandBaseAttribute(int layer_id = 0)
        {
            LabelSetting objLabelSettings = new LabelSetting();
            return objLabelSettings.lstLabelAttributes = BLLandBaseLabelSetting.Instance.GetLayerLabelAttributes(layer_id);
        }

        [HttpPost]
        public ActionResult SaveLandBaseLabelSettings(Landbase_label_Settings objLabelSettings)
        {
            PageMessage objMsg = new PageMessage();
            List<Status> message = new List<Status>();

            // update layer attributes in info setting table...
            objLabelSettings.user_id = Convert.ToInt32(((User)Session["userDetail"]).user_id);

            objLabelSettings = BLLandBaseLabelSetting.Instance.SaveLabelSetting(objLabelSettings);

            if (objLabelSettings != null)
            {
                //success

                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Label settings updated successfully!";
            }
            else
            {
                //failure
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = "Error in updating Label settings!";
            }

            objLabelSettings.pageMsg = objMsg;
            objLabelSettings.lstLayers = new BLLandBaseLayer().getLandBaseLayers();
            objLabelSettings.lstLabelAttributes = BindLandBaseAttribute(objLabelSettings.landbase_layer_id).ToList(); 
            return View("LandBaseLabelSettings", objLabelSettings);
        }
        #endregion

    }
}
