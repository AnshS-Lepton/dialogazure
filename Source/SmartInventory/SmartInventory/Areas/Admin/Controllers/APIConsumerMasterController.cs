using System.Web.Mvc;
using BusinessLogics.Admin;
using BusinessLogics;
using SmartInventory.Filters;
using Models.Admin;
using SmartInventory.Settings;
using Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Utility;
using Lepton.Utility;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class APIConsumerMasterController : Controller
    {
        // GET: Admin/APIConsumerMaster
        public ActionResult GetAPIConsumer(VwAPIConsumerMaster objConsumerMaster, int page = 0, string sort = "", string sortdir = "")
        {
            objConsumerMaster.objAPIConsumerMasterFilter.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objConsumerMaster.objAPIConsumerMasterFilter.currentPage = page == 0 ? 1 : page;
            objConsumerMaster.objAPIConsumerMasterFilter.sort = sort;
            objConsumerMaster.objAPIConsumerMasterFilter.orderBy = sortdir;
            objConsumerMaster.lstAPIConsumerMaster = new BLAPIConsumerMaster().GetAPIConsumer(objConsumerMaster.objAPIConsumerMasterFilter);
            objConsumerMaster.objAPIConsumerMasterFilter.totalRecord = objConsumerMaster.lstAPIConsumerMaster.Count > 0 ? objConsumerMaster.lstAPIConsumerMaster[0].total_records : 0;
            return View("_APIConsumerMaster", objConsumerMaster);
        }

        public ActionResult AddAPIConsumer(int Id = 0)
        {
            APIConsumerMasterRequest objApiConsumer = new APIConsumerMasterRequest();
            if (Id > 0)
            {
                objApiConsumer.objAPIConsumerMaster = new BLAPIConsumerMaster().GetAPIConsumerById(Id);
                objApiConsumer.objAPIConsumerMaster.password = MiscHelper.DecodeTo64(objApiConsumer.objAPIConsumerMaster.password).ToString();
            }

            return View("_AddAPIConsumerMaster", objApiConsumer);
        }


        [HttpPost]
        public JsonResult CheckAPIConsumerUserExists(string userName)
        {
            PageMessage obj = new PageMessage();
            bool IsUserExist = new BLAPIConsumerMaster().ValidateUserName(userName);
            if (IsUserExist)
            {
                return Json(new { status = ResponseStatus.FAILED.ToString(), message = "User Name already Exists!" });
            }
            else
            {
                return Json(new { status = ResponseStatus.OK.ToString() });
            }
        }

        public ActionResult SaveAPIConsumer(APIConsumerMasterRequest objApiConsumer)
        {
            PageMessage objPM = new PageMessage();
            if (ModelState.IsValid)
            {
                objApiConsumer.objAPIConsumerMaster.password = MiscHelper.EncodeTo64(objApiConsumer.objAPIConsumerMaster.password).ToString();
                var resultItem = new BLAPIConsumerMaster().SaveAPIConsumer(objApiConsumer.objAPIConsumerMaster, Convert.ToInt32(Session["user_id"]));
                objPM.status = ResponseStatus.OK.ToString();
                if (objApiConsumer.objAPIConsumerMaster.consumer_id > 0)  // Update 
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = "User Saved Successfully";
                }
                else
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = "User Update Successfully";
                    objApiConsumer.objAPIConsumerMaster = resultItem;
                }
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            }
            objApiConsumer.pageMsg = objPM;
            return View("_AddApiConsumerMaster", objApiConsumer);
        }


    }
}