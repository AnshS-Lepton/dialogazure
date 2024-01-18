using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models.Feasibility;
using System.Data.Entity;
using DataAccess.DBContext;
using BusinessLogics.Feasibility;
using Models;
using static Models.Feasibility.FeasibilityKMLData;
using Newtonsoft.Json;
using SmartFeasibility.Filters;
using System.Data;
using Utility;
using System.IO;
using NPOI.SS.UserModel;
using SmartFeasibility.Helper;
using System.Web.Script.Serialization;
using System.Text;

namespace SmartFeasibility.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class FeasibilityDetailsController : Controller
    {
        // GET: FeasibilityDetails
        public ActionResult Index(FeasibilityDetailsViewModel feasibilityDetailsInfo)
        {

            FeasibilityCableTypeViewModel model = new FeasibilityCableTypeViewModel();
            feasibilityDetailsInfo.lstCableTypes = new BLFeasibilityCableType().getFeasibilityCableTypesddl();
            return View(feasibilityDetailsInfo);
        }
        public ActionResult ViewFeasibilityForm()
        {
            return View();
        }
        public JsonResult SavefeasibilityDetails(FeasibilityDetailsViewModel objFeasibilityDetailsView)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            objFeasibilityDetailsView.userDetail = (User)Session["userDetail"];
            objFeasibilityDetailsView.objFeasibilityModel.created_by = objFeasibilityDetailsView.userDetail.user_id;
            objFeasibilityDetailsView.objFeasibilityModel.created_on = DateTimeHelper.Now;
            if (objFeasibilityDetailsView.objFeasibilityModel.feasibility_id == 0)
            {
                objFeasibilityDetailsView.objFeasibilityModel = new BLFeasibilityInput().SavefeasibilityDetails(objFeasibilityDetailsView.objFeasibilityModel);
            }
            FeasibilityHistory objFeasibilityHistory = new FeasibilityHistory();
            if (objFeasibilityDetailsView.objFeasibilityModel.feasibility_id > 0)
            {
                objFeasibilityHistory.feasibility_id = objFeasibilityDetailsView.objFeasibilityModel.feasibility_id;
                objFeasibilityHistory.created_by = objFeasibilityDetailsView.objFeasibilityModel.created_by;
                objFeasibilityHistory.created_on = objFeasibilityDetailsView.objFeasibilityModel.created_on;
                objFeasibilityHistory.feasibility_result = objFeasibilityDetailsView.objFeasibilityModel.feasibility_result;
                objFeasibilityHistory.core_level_result = objFeasibilityDetailsView.objFeasibilityModel.core_level_result;

                objFeasibilityHistory = new BLFeasibilityHistory().SavefeasibilityHistory(objFeasibilityHistory);
            }
            List<FeasibiltyGeometry> lstFeasilityGeometry = objFeasibilityDetailsView.lstFeasibilityGeometry.Where(m => m.isSelected == true).ToList();
            if (objFeasibilityHistory.history_id > 0)
            {
                foreach (var objFeasilityGeometry in lstFeasilityGeometry)
                {
                    if (objFeasilityGeometry.cable_geometry != null)
                    {
                        objFeasilityGeometry.created_by = objFeasibilityDetailsView.objFeasibilityModel.created_by;
                        objFeasilityGeometry.history_id = objFeasibilityHistory.history_id;
                        objFeasilityGeometry.cable_geometry = objFeasilityGeometry.cable_geometry.TrimEnd(' ').TrimEnd(',');

                        int GeometryID = new BLFeasibiltyGeometry().SavefeasibilityGeometry(objFeasilityGeometry);
                    }
                }
            }
            objResp.status = ResponseStatus.OK.ToString();
            objResp.message = "Feasibility record saved successfully!";
            FeasibilityKMLModel feasKMLModel = new FeasibilityKMLModel();
            feasKMLModel.start_lat_lng = objFeasibilityDetailsView.objFeasibilityModel.start_lat_lng;
            feasKMLModel.end_lat_lng = objFeasibilityDetailsView.objFeasibilityModel.end_lat_lng;
            feasKMLModel.feasibility_name = objFeasibilityDetailsView.objFeasibilityModel.feasibility_name;
            feasKMLModel.history_id = objFeasibilityHistory.history_id.ToString();
            feasKMLModel.start_Point_Name = objFeasibilityDetailsView.objFeasibilityModel.start_point_name;
            feasKMLModel.end_Point_Name = objFeasibilityDetailsView.objFeasibilityModel.end_point_name;

            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            objResp.result = jsonSerializer.Serialize(feasKMLModel);
            return Json(objResp, JsonRequestBehavior.AllowGet);
            // return View(objFeasibilityDetailsView);
        }
        public ActionResult PastFeasibilityView(FeasibilityDetailsViewModel feasibilityDetailsInfo)
        {
            feasibilityDetailsInfo = new FeasibilityDetailsViewModel();
            feasibilityDetailsInfo.lstFeasibilityDetails = new List<FeasibilityDetails>();
            feasibilityDetailsInfo.lstFeasibilityGeometry = new List<FeasibiltyGeometry>();
            feasibilityDetailsInfo.lstCableTypes = new BLFeasibilityCableType().getFeasibilityCableTypesddl();

            List<FeasibilityHistory> lstFeasibilityHistory = new BLFeasibilityHistory().getFeasibilityDetails().ToList();
            // List<FeasibilityDemarcationType> lstDemarcationType = new BLFeasibilityDemarcationType().getFeasibilityDemarcationTypes();
            foreach (var obj in lstFeasibilityHistory)
            {
                obj.FeasibiltyGeometry = new BLFeasibiltyGeometry().getFeasibilityGeometry(obj.history_id);
                obj.FeasibilityInput = new BLFeasibilityInput().getFeasibilityInput(obj.feasibility_id);
                FeasibilityDetails objFeasibilityDetails = new FeasibilityDetails();
                objFeasibilityDetails.feasibilityName = obj.FeasibilityInput.feasibility_name;
                objFeasibilityDetails.feasibility_id = obj.FeasibilityInput.feasibility_id;
                objFeasibilityDetails.start_lat = Convert.ToDouble(obj.FeasibilityInput.start_lat_lng.Split(',')[1]);
                objFeasibilityDetails.start_lng = Convert.ToDouble(obj.FeasibilityInput.start_lat_lng.Split(',')[0]);
                objFeasibilityDetails.end_lat = Convert.ToDouble(obj.FeasibilityInput.end_lat_lng.Split(',')[1]);
                objFeasibilityDetails.end_lng = Convert.ToDouble(obj.FeasibilityInput.end_lat_lng.Split(',')[0]);
                var i = 0;
                foreach (var objFeasibiltyGeometry in obj.FeasibiltyGeometry)
                {
                    switch (objFeasibiltyGeometry.type_id)
                    {
                        /*   1;"inside"
                             2;"outside"
                             3;"inside_P"
                             4;"inside_A"
                             5;"lmc_start"
                             6;"lmc_end"
                         */
                        //case 1:
                        //    objFeasibilityDetails.ExistingLength = objFeasibiltyGeometry.cable_length;
                        //    break;
                        case 2:
                            //objFeasibilityDetails.NewOutsideLength += objFeasibiltyGeometry.cable_length;
                            break;
                        case 3:
                            objFeasibilityDetails.ExistingLength_P += objFeasibiltyGeometry.cable_length;
                            break;
                        case 4:
                            objFeasibilityDetails.ExistingLength_A += objFeasibiltyGeometry.cable_length;
                            break;
                        case 5:
                            objFeasibilityDetails.lmc_A_End_Path += objFeasibiltyGeometry.cable_length;
                            break;
                        case 6:
                            objFeasibilityDetails.lmc_B_End_Path += objFeasibiltyGeometry.cable_length;
                            break;
                    }

                    objFeasibilityDetails.CableGeom += string.Join(", ", obj.FeasibiltyGeometry.ToArray()[i].cable_geometry);
                    i++;
                    objFeasibilityDetails.totalLength = objFeasibilityDetails.NewOutside_A_Length + objFeasibilityDetails.NewOutside_B_Length + objFeasibilityDetails.ExistingLength_P + objFeasibilityDetails.ExistingLength_A + objFeasibilityDetails.lmc_A_End_Path + objFeasibilityDetails.lmc_B_End_Path;
                    feasibilityDetailsInfo.lstFeasibilityGeometry.Add(objFeasibiltyGeometry);
                }
                feasibilityDetailsInfo.lstFeasibilityDetails.Add(objFeasibilityDetails);
            }
            return PartialView("PastFeasibility", feasibilityDetailsInfo);
        }

        public ActionResult pastFeasibilities(PastFeasibilityViewModel model, int page = 0, string sort = "", string sortdir = "", string searchBy = "", string searchText = "")
        {
            model.objGridAttributes.searchBy = searchBy != "" ? searchBy : model.objGridAttributes.searchBy;
            model.objGridAttributes.searchText = searchText.Trim() != "" ? searchText.Trim() : (model.objGridAttributes.searchText != null ? model.objGridAttributes.searchText.Trim() : model.objGridAttributes.searchText);

            model.lstSearchBy = GetPastFeasibilitySearchByColumns();
            model.objGridAttributes.pageSize = 10;
            model.objGridAttributes.currentPage = page == 0 ? 1 : page;
            model.objGridAttributes.sort = sort == "" ? "feasibility_id" : sort;
            model.objGridAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            // model.objGridAttributes.searchText = searchText;
            //  model.lstPastFeasibility = new BLFeasibilityHistory().getPastFeasibilities(model.objGridAttributes); // new BLFeasibilityCableType().getFeasibilityCableTypes(model.objGridAttributes);
            List<PastFeasibility> pastFeasibilities = new BLFeasibilityHistory().getPastFeasibilities(model.objGridAttributes, model.FromDate, model.ToDate);
            foreach (PastFeasibility dic in pastFeasibilities)
            {
                //dic.start_point_name = HttpUtility.HtmlDecode(dic.start_point_name)?.Replace("\t", "");
                dic.created_on = MiscHelper.FormatDateTime(dic.created_on);
                model.lstPastFeasibility.Add(dic);
            }

            HTMLEncoded(ref model);

            model.objGridAttributes.totalRecord = model.lstPastFeasibility != null && model.lstPastFeasibility.Count > 0 ? Convert.ToInt32(model.lstPastFeasibility[0].totalRecords) : 0;

            //  model.lstPastFeasibility = Json.De
            // List<PastFeasibility> pastFeasibilities = new BLFeasibilityHistory().getPastFeasibilities();

            // return PartialView("_PastFeasibilities", JsonConvert.SerializeObject(pastFeasibilities));
            Session["pastFeas"] = model.lstPastFeasibility;
            return PartialView("_PastFeasibilities", model);
        }

        private void HTMLEncoded(ref PastFeasibilityViewModel model)
        {
            foreach (PastFeasibility rec in model.lstPastFeasibility)
            {
               
                rec.start_point_name = !string.IsNullOrEmpty(rec.start_point_name)?HttpUtility.HtmlEncode(rec.start_point_name).Replace(@"\", @"\\"): string.Empty;
                rec.end_point_name = !string.IsNullOrEmpty(rec.end_point_name) ? HttpUtility.HtmlEncode(rec.end_point_name).Replace(@"\", @"\\"): string.Empty;
                rec.feasibility_name = !string.IsNullOrEmpty(rec.feasibility_name) ? HttpUtility.HtmlEncode(rec.feasibility_name).Replace(@"\", @"\\"): string.Empty;
                rec.customer_id = !string.IsNullOrEmpty(rec.customer_id) ? HttpUtility.HtmlEncode(rec.customer_id).Replace(@"\", @"\\"): string.Empty;
                rec.customer_name = !string.IsNullOrEmpty(rec.customer_name) ? HttpUtility.HtmlEncode(rec.customer_name).Replace(@"\", @"\\"): string.Empty;
            }
        }

        public List<KeyValueDropDown> GetPastFeasibilitySearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Customer Name", value = "fi.customer_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Customer ID", value = "fi.customer_id" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Feasibility ID", value = "fi.feasibility_name" });

            return lstSearchBy.OrderBy(m => m.key).ToList();
        }
        public void ExportPastFeasibilityRecord(string search_by, string searchText, string FromDate, string ToDate)
        {

            FromDate = FromDate == "" ? null : Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd");
            ToDate = ToDate == "" ? null : Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");

            // FromDate = string.Format("dd-MM-yyyy", FromDate);
            CommonGridAttributes objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchBy = search_by;
            objGridAttributes.searchText = searchText;
            objGridAttributes.sort = "feasibility_id";
            objGridAttributes.orderBy = "desc";
            var listHistoryRecord = new BLFeasibilityHistory().getPastFeasibilyExportData<Dictionary<string, string>>(objGridAttributes, FromDate, ToDate);
            DataTable dtlogs = MiscHelper.GetDataTableFromDictionaries(listHistoryRecord);
            string[] arrIgnoreColumns = { "totalrecords", "s_no", "feasibility_id", "history_id", "cores_required", "core_level_result", "feasibility_result", "cable_type_id", "cable_type" };
            foreach (var IgnoreColumn in arrIgnoreColumns)
            {
                if (dtlogs.Columns.Contains(IgnoreColumn))
                {
                    dtlogs.Columns.Remove(IgnoreColumn);
                }
            }
            foreach (DataRow dr in dtlogs.Rows)
            {
                dr["created_on"] = MiscHelper.FormatDateTime(dr["created_on"].ToString());

            }
            dtlogs.Columns["feasibility_name"].ColumnName = "feasibility_id";
            dtlogs.Columns["history_display_id"].ColumnName = "history_id";
            dtlogs.Columns["inside_p"].ColumnName = "Inside Planned Length (m)";
            dtlogs.Columns["inside_a"].ColumnName = "Inside As-Built Length (m)";
            dtlogs.Columns["outside_a_end"].ColumnName = "Outside Length (A) (m)";
            dtlogs.Columns["outside_b_end"].ColumnName = "Outside Length (B) (m)";
            dtlogs.Columns["buffer_radius_a"].ColumnName = "Buffer Radius(A) (m)";
            dtlogs.Columns["buffer_radius_b"].ColumnName = "Buffer Radius(B) (m)";
            dtlogs.Columns["start_lat_lng"].ColumnName = "Start LngLat";
            dtlogs.Columns["end_lat_lng"].ColumnName = "End LngLat";
            dtlogs.Columns["lmc_a"].ColumnName = "LMC Length (A) (m)";
            dtlogs.Columns["lmc_b"].ColumnName = "LMC Length (B) (m)";

            // 
            foreach (DataColumn col in dtlogs.Columns)
            {
                col.ColumnName = MiscHelper.ToCamelCase(col.ColumnName);
            }


            dtlogs.TableName = "Feasibility_History_Report";
            ExportData(dtlogs, "Feasibility_History_Report");
        }
        public JsonResult getDetails(int history_id)
        {
            List<FeasibilityCableGeoms> pastFeasibilities = new BLFeasibilityHistory().getFeasibilityDetails(history_id);

            return Json(new { status = "success", data = pastFeasibilities, pastFeas = Session["pastFeas"] }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getfeasibilityHistoryDetails(string selectedFeasibilityID)
        {
            FeasibilityInput objInputModel = new FeasibilityInput();
            objInputModel = new BLFeasibilityInput().getFeasibilityInput(Convert.ToInt16(selectedFeasibilityID));
            return Json(objInputModel, JsonRequestBehavior.AllowGet);
        }
        private void ExportData(DataTable dtReport, string fileName)
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

                Response.End();
            }
        }

        #region(FEASIBILITY FTTH)

        public ActionResult Ftth(FTTHFeasibilityModel feasibilityDetailsFtthInfo)
        {
            var usrDetail = (User)Session["userDetail"];
            if (usrDetail != null)
            {
                if (usrDetail.role_id == 1)
                {
                    return RedirectToAction("index", "UnAuthorized");
                }

                feasibilityDetailsFtthInfo.feasibility_id = 0;
                return View(feasibilityDetailsFtthInfo);
            }
            else
            {
                return RedirectToAction("index", "Login");
            }
        }

        public JsonResult SavefeasibilityDetailsFtth(FTTHFeasibilityModel objFeasibilityModel)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            if (objFeasibilityModel !=null)
            {
                FTTHFeasibilityDetailModel objFtthInput = new FTTHFeasibilityDetailModel()
                {
                    feasibility_name = objFeasibilityModel.feasibility_name,
                    customer_id = objFeasibilityModel.customer_id,
                    customer_name = objFeasibilityModel.customer_name,
                    lat_lng = objFeasibilityModel.lat + "," + objFeasibilityModel.lng,
                    created_by = ((User)Session["userDetail"]).user_name,
                    buffer_radius = objFeasibilityModel.buffer_radius,
                    entity_loc= objFeasibilityModel.entity_location
                };


                if (objFeasibilityModel.feasibility_id == 0)
                {
                    objFtthInput = new BLFeasibilityInput().SavefeasibilityDetailsFtth(objFtthInput);
                }
                StringBuilder coordinates = new StringBuilder();
                foreach (var item in objFeasibilityModel.path_geometry)
                {
                    string str = item.Replace(',', ' ');
                    coordinates.Append(str + ",");
                }
                FTTHFeasibilityHistory objFtthHistory = new FTTHFeasibilityHistory();
                if (objFtthInput.feasibility_id > 0)
                {
                    objFtthHistory.feasibility_id = objFtthInput.feasibility_id;
                    objFtthHistory.created_by = objFtthInput.created_by;
                    objFtthHistory.coordinates = coordinates.ToString().Remove(coordinates.ToString().LastIndexOf(','));
                    objFtthHistory.path_distance = objFeasibilityModel.path_distance;
                    objFtthHistory.entity_id = objFeasibilityModel.entity_id;

                    int GeometryID = new BLFeasibilityHistory().SavefeasibilityHistoryFtth(objFtthHistory);

                    FeasibilityKMLFTTHModel feasKMLModel = new FeasibilityKMLFTTHModel();
                    feasKMLModel.feasibility_name = objFtthInput.feasibility_name;
                    feasKMLModel.history_id = GeometryID;
                    feasKMLModel.lat_lng = objFtthInput.lat_lng;
                    feasKMLModel.entity_id = objFtthHistory.entity_id;
                    feasKMLModel.feasibility_id = objFtthInput.feasibility_id;
                    feasKMLModel.customer_id = objFtthInput.customer_id;
                    feasKMLModel.customer_name = objFtthInput.customer_name;
                    feasKMLModel.entity_loc = objFtthInput.entity_loc;
                    feasKMLModel.path_distance = objFtthHistory.path_distance;
                    
                    
                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    objResp.result = jsonSerializer.Serialize(feasKMLModel);
                }
                //List<FeasibiltyGeometry> lstFeasilityGeometry = objFeasibilityDetailsView.lstFeasibilityGeometry.Where(m => m.isSelected == true).ToList();
                //if (objFeasibilityHistory.history_id > 0)
                //{
                //    foreach (var objFeasilityGeometry in lstFeasilityGeometry)
                //    {
                //        if (objFeasilityGeometry.cable_geometry != null)
                //        {
                //            objFeasilityGeometry.created_by = objFeasibilityDetailsView.objFeasibilityModel.created_by;
                //            objFeasilityGeometry.history_id = objFeasibilityHistory.history_id;
                //            objFeasilityGeometry.cable_geometry = objFeasilityGeometry.cable_geometry.TrimEnd(' ').TrimEnd(',');

                //            int GeometryID = new BLFeasibiltyGeometry().SavefeasibilityGeometry(objFeasilityGeometry);
                //        }
                //    }
                //}
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Feasibility record saved successfully!";
                //objResp.message = "Feasibility record saved successfully!";
                
                //FeasibilityKMLModel feasKMLModel = new FeasibilityKMLModel();
                //feasKMLModel.start_lat_lng = objFeasibilityDetailsView.objFeasibilityModel.start_lat_lng;
                //feasKMLModel.end_lat_lng = objFeasibilityDetailsView.objFeasibilityModel.end_lat_lng;
                //feasKMLModel.feasibility_name = objFeasibilityDetailsView.objFeasibilityModel.feasibility_name;
                //feasKMLModel.history_id = objFeasibilityHistory.history_id.ToString();
                //feasKMLModel.start_Point_Name = objFeasibilityDetailsView.objFeasibilityModel.start_point_name;
                //feasKMLModel.end_Point_Name = objFeasibilityDetailsView.objFeasibilityModel.end_point_name;

                //  JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                // objResp.result = jsonSerializer.Serialize(feasKMLModel);

                // return View(objFeasibilityDetailsView);
            }
            else
            {
                objResp.status = ResponseStatus.ERROR.ToString();

                objResp.message = "Please fill mandatory fields!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult pastFeasibilitiesFtth(PastFeasibilityFtthViewModel model, int page = 0, string sort = "", string sortdir = "", string searchBy = "", string searchText = "")
        {
            model.objGridAttributes.searchBy = searchBy != "" ? searchBy : model.objGridAttributes.searchBy;
            model.objGridAttributes.searchText = searchText.Trim() != "" ? searchText.Trim() : (model.objGridAttributes.searchText != null ? model.objGridAttributes.searchText.Trim() : model.objGridAttributes.searchText);

            model.lstSearchBy = GetPastFeasibilityFtthSearchByColumns();
            model.objGridAttributes.pageSize = 10;
            model.objGridAttributes.currentPage = page == 0 ? 1 : page;
            model.objGridAttributes.sort = sort == "" ? "feasibility_id" : sort;
            model.objGridAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            // model.objGridAttributes.searchText = searchText;
            //  model.lstPastFeasibility = new BLFeasibilityHistory().getPastFeasibilities(model.objGridAttributes); // new BLFeasibilityCableType().getFeasibilityCableTypes(model.objGridAttributes);
            List<PastFeasibilityFtth> pastFeasibilities = new BLFeasibilityHistory().getPastFeasibilitiesFtth(model.objGridAttributes, model.FromDate, model.ToDate);
            foreach (PastFeasibilityFtth dic in pastFeasibilities)
            {
                dic.created_on = MiscHelper.FormatDateTime(dic.created_on);
                model.lstPastFeasibilityFtth.Add(dic);
            }

           // HTMLEncoded(ref model);

            model.objGridAttributes.totalRecord = model.lstPastFeasibilityFtth != null && model.lstPastFeasibilityFtth.Count > 0 ? Convert.ToInt32(model.lstPastFeasibilityFtth[0].totalRecords) : 0;

            //  model.lstPastFeasibility = Json.De
            // List<PastFeasibility> pastFeasibilities = new BLFeasibilityHistory().getPastFeasibilities();

            // return PartialView("_PastFeasibilities", JsonConvert.SerializeObject(pastFeasibilities));
            Session["pastFeasFtth"] = model.lstPastFeasibilityFtth;
            return PartialView("_PastFeasibilitiesFtth", model);
        }
        public List<KeyValueDropDown> GetPastFeasibilityFtthSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Customer Name", value = "fi.customer_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Customer ID", value = "fi.customer_id" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Feasibility Name", value = "fi.feasibility_name" });

            return lstSearchBy.OrderBy(m => m.key).ToList();
        }

        public JsonResult getDetailsFtth(int history_id)
        {
            PastFeasibilityFtth pastFeasibilitiesFtth = new BLFeasibilityHistory().getFeasibilityDetailsFtth(history_id);
            pastFeasibilitiesFtth.geometry = new BLFeasibilityHistory().getFeasibilityDetailsFTTH(history_id);
            return Json(new { status = "success", data = pastFeasibilitiesFtth, pastFeasFtth = Session["pastFeasFtth"] }, JsonRequestBehavior.AllowGet);
        }

        public void ExportPastFeasibilityRecordFTTH(string search_by, string searchText, string FromDate, string ToDate)
        {

            FromDate = FromDate == "" ? null : Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd");
            ToDate = ToDate == "" ? null : Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");

            // FromDate = string.Format("dd-MM-yyyy", FromDate);
            CommonGridAttributes objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchBy = search_by;
            objGridAttributes.searchText = searchText;
            objGridAttributes.sort = "feasibility_id";
            objGridAttributes.orderBy = "desc";
            var listHistoryRecord = new BLFeasibilityHistory().getPastFeasibilyExportDataFtth<Dictionary<string, string>>(objGridAttributes, FromDate, ToDate);
            DataTable dtlogs = MiscHelper.GetDataTableFromDictionaries(listHistoryRecord);
            string[] arrIgnoreColumns = { "totalrecords", "s_no", "feasibility_id"};
            foreach (var IgnoreColumn in arrIgnoreColumns)
            {
                if (dtlogs.Columns.Contains(IgnoreColumn))
                {
                    dtlogs.Columns.Remove(IgnoreColumn);
                }
            }
            foreach (DataRow dr in dtlogs.Rows)
            {
                dr["created_on"] = MiscHelper.FormatDateTime(dr["created_on"].ToString());

            }
            dtlogs.Columns["feasibility_name"].ColumnName = "feasibility_id";
            //dtlogs.Columns["history_display_id"].ColumnName = "history_id";
            dtlogs.Columns["customer_id"].ColumnName = "Customer Id";
            dtlogs.Columns["customer_name"].ColumnName = "Customer Name";
            dtlogs.Columns["lat_lng"].ColumnName = "Lat,Lng";
            dtlogs.Columns["path_distance"].ColumnName = "Path distance(km)";
            dtlogs.Columns["entity_id"].ColumnName = "Entity Id";
        //    dtlogs.Columns["material_cost"].ColumnName = "Material Cost";
         //   dtlogs.Columns["service_cost"].ColumnName = "Service Cost";
           // dtlogs.Columns["end_lat_lng"].ColumnName = "End LngLat";
           // dtlogs.Columns["lmc_a"].ColumnName = "LMC Length (A) (m)";
           // dtlogs.Columns["lmc_b"].ColumnName = "LMC Length (B) (m)";

            // 
            foreach (DataColumn col in dtlogs.Columns)
            {
                col.ColumnName = MiscHelper.ToCamelCase(col.ColumnName);
            }


            dtlogs.TableName = "Feasibility_History_Report_Ftth";
            ExportData(dtlogs, "Feasibility_History_Report_Ftth");
        }
        #endregion
    }
}