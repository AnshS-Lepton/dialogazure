using BusinessLogics;
using Models;
using Models.Admin;
using Npgsql;
using NPOI.SS.UserModel;
using Resources.Abstract;
using Resources.Concrete;
using Resources.Entities;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Utility;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]

    public class ResourcesController : Controller
    {
        private static string connectionString = null;
        // GET: Admin/Resources

        //[ValidateInput(false)]

        public ActionResult Index(ManageResources objVMResources, int page = 0, string sort = "", string sortdir = "", string btnAction = "")
        {
            if (btnAction != "Reset")
            {
                if (objVMResources.Type == null)
                    Session["AppType"] = "web";
                else
                    Session["AppType"] = objVMResources.Type;
            }
            objVMResources.Type = Session["AppType"].ToString();
            var p_type = Session["AppType"].ToString();
            objVMResources.objGridAttributes.pageSize = 10;
            objVMResources.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objVMResources.objGridAttributes.sort = sort;
            objVMResources.objGridAttributes.orderBy = sortdir;
            Session["res_resources_List"] = objVMResources;
            objVMResources.ResourcesList = new BLResources().GetResourceList(objVMResources);
            objVMResources.ResourceLangugeList = new BLResources().GetResourcelanguageList(p_type);
            objVMResources.objGridAttributes.totalRecord = objVMResources.ResourcesList != null && objVMResources.ResourcesList.Count > 0 ? objVMResources.ResourcesList[0].totalRecords : 0;
            ViewData["lang"] = objVMResources.ResourcesList.Select(m => m.language).FirstOrDefault();
            Session["Culture"] = objVMResources.ResourcesList.Select(m => m.culture).FirstOrDefault();
            //UpdateResourceCS();
            // GetResourceJqueryused();
            var a = Regex.Matches("mobile", "true").Count;
            return View("_ResourceManagement", objVMResources);
        }

        public void DownloadResourceList(ManageResources objVMResources)
        {
            if (Session["res_resources_List"] != null)
            {
                objVMResources = (ManageResources)Session["res_resources_List"];
                objVMResources.objGridAttributes.pageSize = 0;
                objVMResources.objGridAttributes.currentPage = 0;
                var p_is_mobile_key = false;
                if (Session["AppType"].ToString() == "mobile")
                    p_is_mobile_key = true;
                var p_culture = Session["Culture"].ToString();

                objVMResources.ResourcesList = new BLResources().GetResourceList(objVMResources);
                ViewData["lang"] = objVMResources.ResourcesList.Select(m => m.language).FirstOrDefault();
                DataTable dtReport = new DataTable();
                // SurveyAssignment
                dtReport = MiscHelper.ListToDataTable(objVMResources.ResourcesList);
                dtReport.Columns.Remove("culture");
                dtReport.Columns.Remove("description");
                dtReport.Columns.Remove("s_no");
                dtReport.Columns.Remove("is_jquery_used");
                dtReport.Columns.Remove("totalrecords");
                dtReport.Columns["language"].SetOrdinal(0);
                dtReport.Columns["key"].SetOrdinal(1);
                dtReport.Columns["value"].SetOrdinal(2);
                dtReport.Columns["currentvalue"].SetOrdinal(3);
                dtReport.Columns["language"].ColumnName = "Language";
                dtReport.Columns["key"].ColumnName = "Key";
                dtReport.Columns["value"].ColumnName = "English Text";
                dtReport.Columns["currentvalue"].ColumnName = ViewData["lang"] + " " + "Language Text";



                var filename = "ResourceLists";
                ExportAssignmentData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        private void ExportAssignmentData(DataTable dtReport, string fileName)
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

        public ActionResult Edit(List<ManageResources> manageResource)
        {
            var Result = false;
            PageMessage objPM = new PageMessage();
            var res = new BLResources().EditResource(manageResource);
            if (res == "1")
                Result = true;
            if (Result)
            {
                UpdateAllKeyValue();
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = "Resource File Updated Successfully.";
                ViewBag.message = objPM.message;
            }
            return Json(objPM, JsonRequestBehavior.AllowGet);
        }
        public void UpdateAllKeyValue()
        {
            var keys = new BLResources().GetAllKeys();
            var resources = new List<ResourceEntry>();
            for (int i = 0; i < keys.Count; i++)
            {
                resources.Add(new ResourceEntry
                {
                    Name = keys[i].key.ToString(),
                    Value = keys[i].value.ToString(),
                    Culture = keys[i].culture.ToString()
                });
            }
            BaseResourceProvider.resources = resources.ToDictionary(r => string.Format("{0}.{1}", r.Culture.ToLowerInvariant(), r.Name));
        }

        public ActionResult AddnewResourceLang()
        {

            var p_type = Session["AppType"].ToString();
            ManageResources objVMResources = new ManageResources();
            objVMResources.ResourceLangugeList = new BLResources().GetResourceDropdown(p_type);

            return PartialView("_AddNewResourceLang", objVMResources);
        }

        public ActionResult GenerateResource(string newculture)
        {

            var p_is_mobile_key = false;
            if (Session["AppType"].ToString() == "mobile")
                p_is_mobile_key = true;
            PageMessage objPM = new PageMessage();
            var Result = true;
            var created_by = Convert.ToInt32(Session["user_id"]);
            new BLResources().GenerateResource(newculture, created_by, p_is_mobile_key);
            if (Result)
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = "Resource File Added Successfully.";
            }
            return Json(new { Data = objPM, JsonRequestBehavior.AllowGet });
        }


        public ActionResult AddNewResourceKey()
        {

            ResourceKeyMaster objResourcesDList = new ResourceKeyMaster();

            List<res_dropdown_master> lstDropDown = new BLResources().GetDropDownList();
            //dropdownType = "Project";
            if (Session["AppType"].ToString() != "mobile")
            {
                objResourcesDList.lstProjects = lstDropDown.Where(o => o.dropdown_type == "Res_Project" && o.is_used_for_web == true).OrderBy(o => o.dropdown_value).ToList();
                //dropdownType = "Module";
                objResourcesDList.lstModule = lstDropDown.Where(o => o.dropdown_type == "Res_Module" && o.is_used_for_web == true).OrderBy(o => o.dropdown_value).ToList();
                //dropdownType = "Sub_Module";
                objResourcesDList.lstSub_Module = lstDropDown.Where(o => o.dropdown_type == "Res_Sub_Module" && o.is_used_for_web == true).OrderBy(o => o.dropdown_value).ToList();
                //dropdownType = "DotNet_JQuery";
                objResourcesDList.lstDotNet_JQuery = lstDropDown.Where(o => o.dropdown_type == "Res_DotNet_JQuery" && o.is_used_for_web == true).OrderBy(o => o.dropdown_value).ToList();
                //dropdownType = "Purpose_Type";
                objResourcesDList.lstPurpose_Type = lstDropDown.Where(o => o.dropdown_type == "Res_Purpose_Type" && o.is_used_for_web == true).OrderBy(o => o.dropdown_value).ToList();
            }
            else
            {
                objResourcesDList.lstProjects = lstDropDown.Where(o => o.dropdown_type == "Res_Project" && o.is_used_for_mobile == true).OrderBy(o => o.dropdown_value).ToList();
                //dropdownType = "Module";
                objResourcesDList.lstModule = lstDropDown.Where(o => o.dropdown_type == "Res_Module" && o.is_used_for_mobile == true).OrderBy(o => o.dropdown_value).ToList();
                //dropdownType = "Sub_Module";
                objResourcesDList.lstSub_Module = lstDropDown.Where(o => o.dropdown_type == "Res_Sub_Module" && o.is_used_for_mobile == true).OrderBy(o => o.dropdown_value).ToList();
                //dropdownType = "Purpose_Type";
                objResourcesDList.lstPurpose_Type = lstDropDown.Where(o => o.dropdown_type == "Res_Purpose_Type" && o.is_used_for_mobile == true).OrderBy(o => o.dropdown_value).ToList();
            }
            return PartialView("_AddNewResourceKey", objResourcesDList);
        }

        public ActionResult AddNewKey(ResourceKeyMaster reskeyMaster)
        {
            if (Session["AppType"].ToString() != "mobile")
            {
                reskeyMaster.is_mobile_key = false;
            }
            else
                reskeyMaster.is_mobile_key = true;
            var created_by = Convert.ToInt32(Session["user_id"]);
            var NewResourceKey = new BLResources().AddNewKey(reskeyMaster, created_by);
            //TempData["Key"] = NewResourceKey;

            if (Session["AppType"].ToString() != "mobile")
            {
                UpdateResourceCS();
                GetResourceJqueryused();
            }
            ViewBag.test = NewResourceKey;
            UpdateAllKeyValue();
            return Json(new { status = "200", msg = "New Key Generate successfully :-" + NewResourceKey }, JsonRequestBehavior.AllowGet);
        }

        public void UpdateResourceCS()
        {
            connectionString = Convert.ToBoolean(ConfigurationManager.AppSettings["ISEncryptedConnection"]) == true ? MiscHelper.Decrypt(ConfigurationManager.AppSettings["constr"].Trim()) : ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            //connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            var builder = new Resources.Utility.ResourceBuilder();

            string filePath = builder.Create(new DbResourceProvider(
                string.Format(connectionString)), summaryCulture: "en");
        }
        public ActionResult getNewKey(ResourceKeyMaster ReskeyMaster)
        {
            var NewKey = "";
            if (Session["AppType"].ToString() != "mobile")
                NewKey = ReskeyMaster.Projectkey + "_" + ReskeyMaster.Modulekey + "_" + ReskeyMaster.Sub_Modulekey + "_" + ReskeyMaster.DotNet_JQuerykey + "_" + ReskeyMaster.Purpose_Typekey;
            else
                NewKey = "M_" + ReskeyMaster.Projectkey + "_" + ReskeyMaster.Modulekey + "_" + ReskeyMaster.Sub_Modulekey + "_" + ReskeyMaster.Purpose_Typekey;
            return Json(new { Data = NewKey, JsonRequestBehavior.AllowGet });
        }

        
        public void GetResourceJqueryused()
        {
            connectionString = Convert.ToBoolean(ConfigurationManager.AppSettings["ISEncryptedConnection"]) == true ? MiscHelper.Decrypt(ConfigurationManager.AppSettings["constr"].Trim()) : ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            //connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            var builder = new Resources.Utility.ResourceBuilder();

            string filePath = builder.CreateJquerystring(new DbResourceProvider(
                string.Format(connectionString)), summaryCulture: "en");
        }

        public ActionResult UploadResourcesFile()
        {

            return View("_UploadResourceFile");
        }

        public void DownloadResourceTemplate(ManageResources objVMResources)
        {
            var p_is_mobile_key = false;
            if (Session["AppType"].ToString() == "mobile")
                p_is_mobile_key = true;

            objVMResources.ExptResourceTemplate = new BLResources().DownloadResourcesTemplate(p_is_mobile_key);

            DataTable dtReport = new DataTable();
            // SurveyAssignment
            dtReport = MiscHelper.ListToDataTable<ExportResourceTemplate>(objVMResources.ExptResourceTemplate);
            dtReport.Columns["key"].ColumnName = "Key";
            dtReport.Columns["value"].ColumnName = "Value";
            dtReport.Columns["purpose"].ColumnName = "Purpose";
            dtReport.Columns["required_value"].ColumnName = "Required Value";
            dtReport.Columns["required_language"].ColumnName = "Required Language";
            var filename = "Res_Template";
            ExportAssignmentData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        }
        [HttpPost]
        public ActionResult UploadResourceFile()
        {
            string strReturn = "";
            string msg = "";
            var Total = 0;
            var Valid = 0;
            var Invalid = 0;
            //table for data
            DataTable dtExcelData = new DataTable();
            try
            {
                if (Request != null)
                {
                    int userId = Convert.ToInt32(Session["user_id"]);
                    var objfile = Request.Files[0];
                    var fileName = AppendTimeStamp(Request.Files[0].FileName);
                    var filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\Resources\\"), fileName);
                    objfile.SaveAs(filepath);
                    bool isHeaderFound = false;
                    //read uploaded excel files..
                    DataTable dataTable = NPOIExcelHelper.ExcelToTable(filepath, out isHeaderFound);
                    if (!isHeaderFound)
                    {
                        return Json(new { strReturn = "No header found in selected file!	", msg = "error" }, JsonRequestBehavior.AllowGet);
                    }
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        dtExcelData = dataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();

                    }
                    int PurposeCount = dtExcelData.Select().Select(r => r.Field<string>("Purpose")).Distinct().Count();
                    var purposeType = dtExcelData.Select().Select(r => r.Field<string>("Purpose")).Distinct().FirstOrDefault();
                    if (PurposeCount > 1 || purposeType.ToLower() != Session["AppType"].ToString().ToLower())
                    {
                        return Json(new { strReturn = "Only " + purposeType.ToUpper() + " resources can be uploaded!", msg = "error" }, JsonRequestBehavior.AllowGet);
                    }
                    //if(purposeType!= Session["AppType"].ToString())
                    //  {
                    //      return Json(new { strReturn = "Can upload only "+purposeType.ToUpper()+" data!", msg = "error" }, JsonRequestBehavior.AllowGet);
                    //  }

                    if (dtExcelData.Rows.Count > 0)
                    {
                        //get maximum building upload count allowed at a time...

                        string ErrorMsg = "";
                        // get branch column mapping...
                        string strMappingFilePath = Server.MapPath("~\\Content\\Templates\\Bulk\\ResourcesTemplate.xml");
                        Dictionary<string, string> dicColumnMapping = GetBulkUploadColumnMapping(strMappingFilePath);

                        // validate uploaded excel column with template mapping...
                        ErrorMsg = validateTemplateColumn(dicColumnMapping, dtExcelData);
                        if (ErrorMsg != "")
                            return Json(new { strReturn = ErrorMsg, msg = "error" }, JsonRequestBehavior.AllowGet);
                        if (ErrorMsg == "")
                        {
                            //ADD COLUMN TO DTEXCEL.. (UPLOADED_BY)
                            DataColumn dcUploadedBy = new DataColumn("UPLOADED_BY", typeof(int));
                            dcUploadedBy.DefaultValue = userId;
                            dtExcelData.Columns.Add(dcUploadedBy);

                            //ADD COLUMN TO DTEXCEL.. (IS_VALID)
                            DataColumn dcIsValid = new DataColumn("IS_VALID", typeof(int));
                            dtExcelData.Columns.Add(dcIsValid);

                            //ADD COLUMN TO DTEXCEL.. (ERROR_MSG)
                            DataColumn dcErrorMsg = new DataColumn("ERROR_MSG", typeof(string));
                            dcErrorMsg.MaxLength = 200;
                            dtExcelData.Columns.Add(dcErrorMsg);

                        }


                        //delete DATA FROM TEMP TABLE ON THE BASIS OF UPLOADED_BY ID
                        var required_language_count = 0;
                        var required_value_coun = 0;
                        BLResources.Instance.DeleteTempResourcesData(userId);
                        List<TempResources> lstResources = new List<TempResources>();
                        foreach (DataRow dr in dtExcelData.Rows)
                        {
                            TempResources objTempResources = new TempResources();

                            objTempResources.uploaded_by = userId;
                            objTempResources.required_language = dr[dicColumnMapping["required_language"]].ToString();
                            objTempResources.key = dr[dicColumnMapping["key"]].ToString();
                            objTempResources.value = dr[dicColumnMapping["value"]].ToString();
                            objTempResources.required_value = dr[dicColumnMapping["required_value"]].ToString();
                            objTempResources.created_by = userId;
                            objTempResources.created_on = DateTimeHelper.Now;
                            objTempResources.is_valid = true;
                            if (objTempResources.required_language == null || objTempResources.required_language == "")
                            {
                                required_language_count++;
                            }
                            if (objTempResources.required_value == null || objTempResources.required_value == "")
                            {
                                required_value_coun++;
                            }
                            if (purposeType.ToLower() == "web" && objTempResources.key.Substring(0, 2) == "M_")
                            {
                                return Json(new { strReturn = "Can not upload as selected file contains some mobile keys!", msg = "error" }, JsonRequestBehavior.AllowGet);
                            }
                            if (purposeType.ToLower() == "mobile" && objTempResources.key.Substring(0, 2) != "M_")
                            {
                                return Json(new { strReturn = "Can not upload as selected file contains some web keys!", msg = "error" }, JsonRequestBehavior.AllowGet);
                            }
                            lstResources.Add(objTempResources);
                        }

                        if (lstResources.Count == required_value_coun || lstResources.Count == required_language_count)
                        {
                            return Json(new { strReturn = "Required fields cannot be blank!", msg = "error" }, JsonRequestBehavior.AllowGet);
                        }


                        if (lstResources.Count > 0)
                        {
                            BLResources.Instance.BulkUploadTempResource(lstResources);
                            dynamic result = "";
                            result = BLResources.Instance.UploadResources(userId);

                            if (!result.status)
                            {
                                // exit function if failed..
                                return Json(new { strReturn = String.Format("Error in uploading Resources! <br> Error : {0}", result.message), msg = "error" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        var getTotalUploadTicketfailureAndSuccess = BLResources.Instance.getTotalUploadResourcesfailureAndSuccess(userId);
                        Total = getTotalUploadTicketfailureAndSuccess.Item1 + getTotalUploadTicketfailureAndSuccess.Item2;
                        Valid = getTotalUploadTicketfailureAndSuccess.Item1;
                        Invalid = getTotalUploadTicketfailureAndSuccess.Item2;

                    }
                    else
                    {
                        // exit function with no record...
                        return Json(new { strReturn = "No record exists in selected file!	", msg = "error" }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (NPOI.POIFS.FileSystem.NotOLE2FileException ex)
            {
                msg = "error";
                strReturn = "Selected file is either corrupted or invalid excel file!";
                ErrorLogHelper.WriteErrorLog("UploadResourceFile()", "Resources", ex);

            }
            catch (Exception ex)
            {
                msg = "error";
                strReturn = String.Format("Failed to upload Resources! <br> Error : {0}", ex.Message);
                ErrorLogHelper.WriteErrorLog("UploadResourceFile()", "Resources", ex);
            }
            return Json(new { strReturn = strReturn, Total, Valid, Invalid, msg = msg == "" ? "success" : msg }, JsonRequestBehavior.AllowGet);
        }

        public Dictionary<string, string> GetBulkUploadColumnMapping(string filepath)
        {
            Dictionary<string, string> dicMapping = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(filepath);
            return dicMapping = doc.Descendants("Mapping").OrderBy(s => (int)int.Parse(s.Attribute("id").Value))
                .Select(p => new
                {
                    DbColName = p.Element("DbColName").Value,
                    TemplateColName = p.Element("TemplateColName").Value
                })
                .ToDictionary(t => t.DbColName, t => t.TemplateColName);
        }
        public string validateTemplateColumn(Dictionary<string, string> dicColumnMapping, DataTable dt)
        {
            string[] arrColumns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.ToLower()).ToArray();
            foreach (var pair in dicColumnMapping)
            {
                // if column not found in template and return error..
                if (!arrColumns.Contains(pair.Value.ToLower()))
                    return string.Format("Selected file does not contain {0} column!	", pair.Value);
                // return Resources.Resources.SI_OSP_GBL_NET_FRM_331 +" "+ pair.Value + "' column!";
            }
            return "";
        }
        public string AppendTimeStamp(string fileName)
        {
            return string.Concat(
            Path.GetFileNameWithoutExtension(fileName),
            DateTimeHelper.Now.ToString("yyyyMMddHHmmssfff"),
            Path.GetExtension(fileName)
            );

        }

        public void DownloadResourcesLog()
        {

            int userId = Convert.ToInt32(Session["user_id"]);
            DataTable dtReport = new DataTable();
            // SurveyAssignment
            var exportdata = new BLResources().DownloadResourcesLogs<Dictionary<string, string>>(userId);
            dtReport = MiscHelper.GetDataTableFromDictionaries(exportdata);
            dtReport.Columns["language"].ColumnName = "Language";
            dtReport.Columns["key"].ColumnName = "Key";
            dtReport.Columns["value"].ColumnName = "Value";
            dtReport.Columns["status"].ColumnName = "Status";
            var filename = "Res_Logs";
            ExportAssignmentData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        }



    }
}