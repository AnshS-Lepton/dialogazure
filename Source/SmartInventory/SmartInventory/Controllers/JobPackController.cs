using System;
using System.Web.Mvc;
using BusinessLogics;
using Models;
using SmartInventory.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SmartInventory.Controllers
{
    public class JobPackController : Controller
    {
        // GET: JobPack
        public ActionResult AddJobPack(int systemId = 0,bool isParentModelType=true)
        {
            JobPackMaster objJobPack = new JobPackMaster();
            if (systemId > 0)
            {
                objJobPack = new BLJobPack().getJobPackDetail(systemId);
            }
            objJobPack.isParentModelType = isParentModelType;
            return View("_AddJobPack", objJobPack);
        }

        //SaveJobPack
        public ActionResult SaveJobPack(JobPackMaster objJobPackMaster)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            if (TryValidateModel(objJobPackMaster))
            {
                //HttpPostedFileBase file = Request.Files["ImageFile"];
                //objJobPackMaster.ImageFile = ConvertToBytes(file);
                var isNew = objJobPackMaster.system_id > 0 ? false : true;
                var resultItem = new BLJobPack().SaveJobPack(objJobPackMaster, Convert.ToInt32(Session["user_id"]));
                if (isNew)
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.isNewEntity = isNew;
                    objPM.message = "Job Created successfully.";
                }
                else
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = "Job updated successfully.";
                }
                objJobPackMaster = resultItem;
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = getFirstErrorFromModelState();
            }
            // RETURN PARTIAL VIEW WITH MODEL DATA
            objJobPackMaster.objPM = objPM;
            return PartialView("_AddJobPack", objJobPackMaster);
        }
        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
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
        public ActionResult ViewJobPacks(ViewJobAssignment objViewJobAssignment, int page = 0, string sort = "", string sortdir = "")
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            objViewJobAssignment.objFilterAttributes.userid = user_id;
            objViewJobAssignment.lstSearchBy = GetJobSearchByColumns();
            BindSurveyStatusDropDown(objViewJobAssignment);
            BindJobUser(objViewJobAssignment);
            objViewJobAssignment.objFilterAttributes.pageSize = ApplicationSettings.JobAssignmentGridPaging;
            objViewJobAssignment.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objViewJobAssignment.objFilterAttributes.sort = sort;
            objViewJobAssignment.objFilterAttributes.orderBy = sortdir;
            objViewJobAssignment.objFilterAttributes.role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);

            if (objViewJobAssignment.objFilterAttributes.JobStatus == null)
            {
                if (role_id != 3)
                {
                    objViewJobAssignment.objFilterAttributes.JobContractUser = 0;
                    objViewJobAssignment.objFilterAttributes.JobStatus = "All";
                }
                else
                {
                    // Sales user
                    objViewJobAssignment.objFilterAttributes.JobContractUser = user_id;
                    objViewJobAssignment.objFilterAttributes.JobStatus = "Assigned";
                }
            }

            objViewJobAssignment.lstJobAssignment = new BLJobPack().GetJobAssignmetDetail(objViewJobAssignment.objFilterAttributes);
            objViewJobAssignment.objFilterAttributes.totalRecord = objViewJobAssignment.lstJobAssignment != null && objViewJobAssignment.lstJobAssignment.Count > 0 ? objViewJobAssignment.lstJobAssignment[0].totalRecords : 0;
            objViewJobAssignment.AllJobAssignment = new BLJobPack().GetJobAssignmetDetail(new JobFilterAttribute { searchFrom = null, searchTo = null, userid = user_id, JobStatus = "All" });
            return View("_ViewJobPacks", objViewJobAssignment);
        }
        public ActionResult AssignJobPack(assignJob model)
        {           
            int user_id = Convert.ToInt32(Session["user_id"]);
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            var obj = new BLJobPack().GetJobContractUser(user_id, role_id);
            model.lstJobContractUser = obj.Select(i => new JobContractUser() { user_id = i.user_id, user_name = i.user_name.ToString() }).ToList();
            return PartialView("_AssignJobPack", model);
        }
        public JsonResult saveAssignJobPack(assignJob model)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            model.assignedBy = Convert.ToInt32(Session["user_id"]);
            var result = new BLJobPack().assignJob(model);
            if (result!=null && result.status == "Assigned")
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = "Job assigned successfully.";
            }                                 
            return Json(new { Data = objPM, JsonRequestBehavior.AllowGet });
        }
        public JsonResult deleteJob(int systemId)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();           
           var response = new BLJobPack().deleteJob(systemId);
            if (response)
            {
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = "Job deleted successfully.";
            }
            return Json(new { Data = objPM, JsonRequestBehavior.AllowGet });
        }
        public List<KeyValueDropDown> GetJobSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Job Type", value = "job_type" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Job Description", value = "job_description" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Manual Address", value = "manual_address" });
            return lstSearchBy;
        }
        private void BindSurveyStatusDropDown(ViewJobAssignment obj)
        {
            List<DropDownMaster> objDDL = new List<DropDownMaster>();
            objDDL.Add(new DropDownMaster { dropdown_key = "All", dropdown_status = false, dropdown_type = "Job_status", dropdown_value = "0" });
            objDDL.Add(new DropDownMaster { dropdown_key = "New", dropdown_status = false, dropdown_type = "Job_status", dropdown_value = "1" });
            objDDL.Add(new DropDownMaster { dropdown_key = "Assigned", dropdown_status = false, dropdown_type = "Job_status", dropdown_value = "3" });
            objDDL.Add(new DropDownMaster { dropdown_key = "Stage1", dropdown_status = false, dropdown_type = "Job_status", dropdown_value = "4" });
            objDDL.Add(new DropDownMaster { dropdown_key = "Stage2", dropdown_status = false, dropdown_type = "Job_status", dropdown_value = "5" });
            objDDL.Add(new DropDownMaster { dropdown_key = "Stage3", dropdown_status = false, dropdown_type = "Job_status", dropdown_value = "6" });
            objDDL.Add(new DropDownMaster { dropdown_key = "Completed", dropdown_status = false, dropdown_type = "Job_status", dropdown_value = "7" });
            obj.ListJobstatus = objDDL;
        }
        public void BindJobUser(ViewJobAssignment objview)
        {
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            var obj = new BLJobPack().GetJobContractUser(objview.objFilterAttributes.userid, role_id);            
            objview.lstJobContractUser = obj.Select(i => new JobContractUser() { user_id = i.user_id, user_name = i.user_name.ToString() }).ToList();           
        }
    }
}