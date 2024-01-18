using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using SmartInventory.Filters;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class OpticalLinkBudgetController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddLinkBudget(int wavelength_id = 0)
        {
            LinkBudgetMaster objLinkBudget = wavelength_id != 0 ? new BLLinkBudget().GetLinkBudgetDetailByID(wavelength_id) : new LinkBudgetMaster();
            objLinkBudget.lstSplitterLoss = new BLLinkBudget().GetSplitterLossByWaveLength(wavelength_id);
            return View("AddLinkBudget", objLinkBudget);
        }

        public ActionResult getSplitterLossDetails(int wavelength_id)
        {
            LinkBudgetMaster objLinkBudget = new LinkBudgetMaster();
            objLinkBudget.lstSplitterLoss = new BLLinkBudget().GetSplitterLossByWaveLength(wavelength_id);
            objLinkBudget.is_active = false;
            return PartialView("_SplitterLossDetail", objLinkBudget);
        }

        [HttpPost]
        public ActionResult SaveLinkBudget(LinkBudgetMaster objLinkBudget)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            int user_id = Convert.ToInt32(Session["user_id"]);

            if (TryValidateModel(objLinkBudget))
            {
                if (objLinkBudget.wavelength_id != 0)
                {
                    //UPDATE EXISTING LINK BUDGET DETAIL...   
                    objLinkBudget = new BLLinkBudget().SaveLinkBudget(objLinkBudget, user_id);
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.isNewEntity = false;
                    objMsg.message = "Link budget detail updated successfully.";
                }
                else
                {
                    // INSERT NEW LINK BUDGET DETAIL...
                    var objExisting = new BLLinkBudget().GetLinkBudgetDetailByWavelength(objLinkBudget.wavelength_value);
                    if (objExisting != null && objExisting.wavelength_id != 0)
                    {
                        objMsg.status = ResponseStatus.ERROR.ToString();
                        objMsg.message = "WaveLength value already exist!";
                    }
                    else
                    {
                        objLinkBudget = new BLLinkBudget().SaveLinkBudget(objLinkBudget, user_id);
                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.isNewEntity = true;
                        objMsg.message = "Link budget detail saved successfully.";
                    }
                }
            }
            else
            {
                objMsg.status = ResponseStatus.FAILED.ToString();
                objMsg.message = getFirstErrorFromModelState();
            }
            objLinkBudget.pageMsg = objMsg;
            return View("AddLinkBudget", objLinkBudget);
        }


        public ActionResult ViewLinkBudget(ViewLinkBudgetDetails objViewLinkBudgetDetail, int page = 0, string sort = "", string sortdir = "")
        {
            objViewLinkBudgetDetail.lstSearchBy = GetSearchByColumns();
            objViewLinkBudgetDetail.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewLinkBudgetDetail.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewLinkBudgetDetail.objGridAttributes.sort = sort;
            objViewLinkBudgetDetail.objGridAttributes.orderBy = sortdir;
            objViewLinkBudgetDetail.lstLinkBudgetDetail = new BLLinkBudget().GetLinkBudgetList(objViewLinkBudgetDetail.objGridAttributes);
            objViewLinkBudgetDetail.objGridAttributes.totalRecord = objViewLinkBudgetDetail.lstLinkBudgetDetail != null && objViewLinkBudgetDetail.lstLinkBudgetDetail.Count > 0 ? objViewLinkBudgetDetail.lstLinkBudgetDetail[0].totalRecords : 0;
            return View("ViewLinkBudget", objViewLinkBudgetDetail);
        }

        [HttpPost]
        public JsonResult DeleteLinkBudgetDetailById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            var output = new BLLinkBudget().DeleteLinkBudgetDetailsById(id);
            if (output > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Link Budget Detail deleted successfully!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Something went wrong while deleting Vendor!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public List<KeyValueDropDown> GetSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Created By", value = "created_by_text" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Modified By", value = "modified_by_text" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Wave Length", value = "wavelength_value" });

            return lstSearchBy.OrderBy(m => m.key).ToList();
        }

        public string getFirstErrorFromModelState()
        {
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    if (error.ErrorMessage != "")
                        return error.ErrorMessage;
                }
            }
            return "";
        }

    }
}