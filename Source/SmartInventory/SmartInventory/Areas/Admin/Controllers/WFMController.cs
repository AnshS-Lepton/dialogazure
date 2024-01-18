using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System.Data;
using System.IO;
using System.Web.Mvc;
using Utility;
using System.Net;
using System.Configuration;
using System.Web.Http.Results;
using Models.WFM;
using BusinessLogics.Admin;
using Models;
using BusinessLogics;
using Models.Admin;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class WFMController : Controller
    {
        private readonly BLVendorSpecification blVendorSpecification;
        public WFMController()
        {
            blVendorSpecification = new BLVendorSpecification();
        }
        // GET: Admin/WFM

        public ActionResult RCAMaster(ViewEntityDropdownMasterSettings objDropdownMasterSettings, int page = 0, string sort = "", string sortdir = "", string msg = "", string status = "")

        {
            var newDropdownMasters = new WfmRca();
            objDropdownMasterSettings.ViewRCADetails = new BLLayer().GetAllRCADetail().ToList();
            objDropdownMasterSettings.objRCAFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objDropdownMasterSettings.objRCAFilter.currentPage = page == 0 ? 1 : page;
            objDropdownMasterSettings.objRCAFilter.sort = sort;
            objDropdownMasterSettings.objRCAFilter.orderBy = sortdir;
            objDropdownMasterSettings.objRCAFilter.RCA = objDropdownMasterSettings.RCA;
            objDropdownMasterSettings.objRCAFilter.status = objDropdownMasterSettings.status;
            // objDropdownMasterSettings.lstDropdownTypes = objDropdownMasterSettings.layer_id > 0 ? blVendorSpecification.GetDropDownListbyLayerId(objDropdownMasterSettings.layer_id, false).DistinctBy(x => x.dropdown_type).ToList() : new List<dropdown_master>();

            objDropdownMasterSettings.rcaDropdownTypes = blVendorSpecification.GetRCAMasterSettingsList(objDropdownMasterSettings.objRCAFilter);
            objDropdownMasterSettings.objRCAFilter.totalRecords = objDropdownMasterSettings.rcaDropdownTypes.Count > 0 ?
            objDropdownMasterSettings.rcaDropdownTypes[0].totalRecords : 0;
            if (!string.IsNullOrWhiteSpace(msg))
            {
                objDropdownMasterSettings.pageMsg = new PageMessage() { message = msg, status = status };
            }
            Session["viewDropdownFilters"] = objDropdownMasterSettings.objRCAFilter;
            return View(objDropdownMasterSettings);
        }

        public ActionResult AddEntityDropdownmasterrca(int id)
        {
            var newDropdownMasters = new WfmRca();
            var objdropdownmasterSetting = new ViewEntityDropdownMasterSettings();
            var objdropdownmasterMapping = new DropdownMasterMapping();
            objdropdownmasterSetting.ViewRCADetails = new BLLayer().GetAllRCADetail().ToList();
            if (id > 0)
            {
                WfmRca objdropdownMaster = blVendorSpecification.GetrcaListbyId(id);
                // objdropdownmasterSetting.layer_id = objdropdownMaster.layer_id;
                //newDropdownMasters.status = objdropdownMaster.status;
                //newDropdownMasters.rca = objdropdownMaster.rca;
                // objdropdownmasterSetting.IsVisible = objdropdownMaster.dropdown_status;
                // objdropdownmasterSetting.OldValue = objdropdownMaster.dropdown_value;

                objdropdownmasterSetting.id = objdropdownMaster.id;
                objdropdownmasterSetting.Value = objdropdownMaster.rca;
                objdropdownmasterSetting.status = objdropdownMaster.status;
            }
            else
            {
                objdropdownmasterSetting.IsVisible = true;
            }

            return PartialView("AddRCAMaster", objdropdownmasterSetting);
        }



        public JsonResult AddEntityRCAmaster(string layer_name, string fieldname, string value)
        {

            var result = blVendorSpecification.GetDropdownMasterRowCount(0, layer_name, fieldname, value);

            return Json(result[0], JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        //public JsonResult getRCADetailsById(int Layer_Id, bool IsFilter = false)
        //{
        //    var result = blVendorSpecification.GetDropDownListbyLayerId(Layer_Id, IsFilter);
        //    var list = result.DistinctBy(x => x.dropdown_type);
        //    return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public JsonResult GetRCADropdownDetails(int layer_id, string fieldname, string Value)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            var checkRecord = blVendorSpecification.GetDropDownListbyDropdowndetails(layer_id, fieldname, Value.Trim());
            if (checkRecord > 0)
            {
                objResp.message = "Record already exists!";
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.result = checkRecord.ToString();
            }
            else
            {
                objResp.status = ResponseStatus.OK.ToString();

            }
            return Json(objResp, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SaveRCAMaster(ViewEntityDropdownMasterSettings _objdrpSetting)
        {
            //ModelState.Clear();

            PageMessage objMsg = new PageMessage();
            var objdropdownmasterSetting = new ViewEntityDropdownMasterSettings();
            objdropdownmasterSetting.ViewRCADetails = new BLLayer().GetAllRCADetail().ToList();

            if (_objdrpSetting.id != 0)
            {
                var checkRecord = blVendorSpecification.GetrcabyDropdowndetails(_objdrpSetting.id, _objdrpSetting.status, _objdrpSetting.RCA);
                if (checkRecord == 0 || _objdrpSetting.OldValue == _objdrpSetting.Value)
                {
                    int UserId = Convert.ToInt32(Session["user_id"]);
                    var finalresult = blVendorSpecification.UpdatercaMaster
                       (_objdrpSetting.id, _objdrpSetting.layer_id, _objdrpSetting.dropdown_type, _objdrpSetting.OldValue, _objdrpSetting.Value,
                       _objdrpSetting.IsVisible, UserId);
                    if (finalresult[0] == 1)
                    {
                        objMsg.message = "Dropdown Master updated successfully!";
                        objMsg.status = ResponseStatus.OK.ToString();
                    }
                    else
                    {
                        objMsg.message = "Record not found";
                        objMsg.status = ResponseStatus.FAILED.ToString();

                    }
                }
            }
            else
            {
                var newDropdownMasters = new WfmRca();
                var newDropdownMaster = new dropdown_master();
                newDropdownMaster.created_by = Convert.ToInt32(Session["user_id"]);
                // newDropdownMaster.layer_id = _objdrpSetting.layer_id;
                // newDropdownMaster.dropdown_key = _objdrpSetting.Value;
                newDropdownMasters.status = _objdrpSetting.status;
                newDropdownMasters.rca = _objdrpSetting.Value;
                newDropdownMaster.dropdown_status = _objdrpSetting.IsVisible;
                // newDropdownMaster.db_column_name = _objdrpSetting.dropdown_type;
                // newDropdownMaster.is_action_allowed = true;
                // newDropdownMaster.is_active = true;
                // newDropdownMaster.parent_value = _objdrpSetting.parent_value;
                // newDropdownMaster.parent_id = _objdrpSetting.dropdown_mapping_key;
                // var NewRecord = blVendorSpecification.SaveDropDownMasterdetails(newDropdownMaster);
                var NewRecord = blVendorSpecification.SaveRCAMaster(newDropdownMasters);
                if (NewRecord == 1)
                {
                    objMsg.message = "Dropdown Master saved successfully!";
                    objMsg.status = ResponseStatus.OK.ToString();
                }
                else if (NewRecord == -1)
                {
                    objMsg.message = "Record already exists!";
                    objMsg.status = ResponseStatus.OK.ToString();

                }

            }
            _objdrpSetting.dropdown_type = " ";
            _objdrpSetting.Value = "";
            _objdrpSetting.OldValue = "";
            _objdrpSetting.id = 0;
            _objdrpSetting.layer_id = 0;
            RCAMaster(_objdrpSetting);
            _objdrpSetting.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
            //we can also set the msg here after calling obj
            return PartialView("AddRCAMaster", _objdrpSetting);
            //return PartialView("DropdownMaster", _objdrpSetting);
        }



        public JsonResult DeletercaMaster(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();

            var result = blVendorSpecification.DeleteDropdownMasters(id);
            if (result[0] == 1)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Record has been successfully deleted!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "No record found";
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        //public JsonResult getDropMasterDetailsByLayerId(int Layer_Id, bool IsFilter = false)
        //{
        //   // var result = blVendorSpecification.GetDropDownListbyLayerId(Layer_Id, IsFilter);
        //    // var list = result.DistinctBy(x => x.dropdown_type);
        //    // return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public JsonResult getRCADetailsByLayerId(string Status)
        {
            var result = blVendorSpecification.GetRCAListbyLayerId(Status);
            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }





        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}
