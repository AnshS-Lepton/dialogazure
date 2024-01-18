using Models;
using Models.Admin;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics.Admin;
using SmartInventory.Settings;
using Utility;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class ProjectController : Controller
    {
        public ActionResult AddProject()
        {
            ProjectMaster obj = new ProjectMaster();
            BindNetworkStage(obj);
            BindProject(obj.network_stage);
            BindPlanning(obj.network_stage, obj.system_id);
            BindWorkorder();
            BindPurpose();
            return View("AddProject", obj);
        }

        [HttpPost]
        public ActionResult SaveProject(ProjectMaster objProjectMaster)
        {
            PageMessage objMsg = new PageMessage();
         
                    BindNetworkStage(objProjectMaster);
                
                    objProjectMaster.user_id = Convert.ToInt32(Session["user_id"]);

                    var isNew = objProjectMaster.system_id > 0 ? false : true;
                    objProjectMaster = new BLProject().SaveEntityProjectDetails(objProjectMaster);
                    
                    if (isNew)
                    {
                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.isNewEntity = isNew;
                        objMsg.message = "Project Detail Saved successfully!";
                        objProjectMaster.system_id = 0;
                     
                    }
                    else
                    {
                        objMsg.status = ResponseStatus.OK.ToString();
                        objMsg.message = "Project Detail Updated successfully!";
                        BindProjectOnChange(objProjectMaster.network_stage);
                    }

            objProjectMaster.pageMsg = objMsg;

            return View("AddProject", objProjectMaster);
            
        }


        public PartialViewResult AddPlaningDetail(PlanningCodeMaster objPlanningDetail)
        {
            if(objPlanningDetail.system_id !=0)
            { 
            PlanningCodeMaster objPlanning = new BLProject().getPlanningCodeDetailById(objPlanningDetail.system_id);
            objPlanning.project_code = objPlanningDetail.project_code;
            return PartialView("_AddPlanningDetail", objPlanning);
            }

            return PartialView("_AddPlanningDetail", objPlanningDetail);
        }



        public PartialViewResult AddProjectDetail(ProjectCodeMaster objProjectDetail)
        {
            if (objProjectDetail.system_id != 0)
            {
                objProjectDetail = new BLProject().getProjectCodeDetailById(objProjectDetail.system_id);
                return PartialView("_AddProjectDetail", objProjectDetail);

            }
            return PartialView("_AddProjectDetail", objProjectDetail);
        }



        public PartialViewResult AddWorkorderDetail(WorkorderCodeMaster objWorkorderDetail)
        {

            if (objWorkorderDetail.system_id != 0)
            {
                WorkorderCodeMaster objWorkorder = new BLProject().getWorkorderCodeDetailById(objWorkorderDetail.system_id);
                objWorkorder.planning_code = objWorkorderDetail.planning_code;
                return PartialView("_AddWorkorderDetail", objWorkorder);
            }

            return PartialView("_AddWorkorderDetail", objWorkorderDetail);
        }


        public PartialViewResult AddPurposeDetail(PurposeCodeMaster objPurposeDetail)
        {

            if (objPurposeDetail.system_id != 0)
            {
                PurposeCodeMaster objPurpose = new BLProject().getPurposeCodeDetailById(objPurposeDetail.system_id);
                objPurpose.workorder_code = objPurposeDetail.workorder_code;
                return PartialView("_AddPurposeDetail", objPurpose);
            }

            return PartialView("_AddPurposeDetail", objPurposeDetail);
        }

        [HttpPost]
        public ActionResult SavePlanningCode(PlanningCodeMaster objPlanningDetail)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            objPlanningDetail.user_id = Convert.ToInt32(Session["user_id"]);
            
            if (objPlanningDetail.network_stage.ToLower() == "planned")
            {
                objPlanningDetail.network_stage = "P";
            }

            if (objPlanningDetail.network_stage.ToLower() == "as built")
            {
                objPlanningDetail.network_stage = "A";
            }

            var isNew = objPlanningDetail.system_id > 0 ? false : true;

            if(isNew)
            {
                int IsPlanningCodeUnique = new BLProject().IsCodeExistsForProjSpeci(objPlanningDetail.network_stage, "planningcode", objPlanningDetail.planning_code);

                if (IsPlanningCodeUnique > 0)
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "this planning code alredy exists";

                    objPlanningDetail.pageMsg = objMsg;

                    return PartialView("_AddPlanningDetail", objPlanningDetail);
                }
            }

          
            objPlanningDetail = new BLProject().SaveEntityPlanningCodeDetails(objPlanningDetail);

            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Planning Detail Saved successfully!";
               
            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Planning Detail Updated successfully!";
            }
           
            objPlanningDetail.pageMsg = objMsg;

            return PartialView("_AddPlanningDetail", objPlanningDetail);
            

        }



        
        [HttpPost]
        public ActionResult SaveProjectCode(ProjectCodeMaster objProjectCodeDetail)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            objProjectCodeDetail.user_id = Convert.ToInt32(Session["user_id"]);


            if (objProjectCodeDetail.network_stage.ToLower() == "planned")
            {
                objProjectCodeDetail.network_stage = "P";
            }

            if (objProjectCodeDetail.network_stage.ToLower() == "as built")
            {
                objProjectCodeDetail.network_stage = "A";
            }

            var isNew = objProjectCodeDetail.system_id != 0 ? false : true;

            
            if(isNew)
            {
                int IsProjectCodeUnique = new BLProject().IsCodeExistsForProjSpeci(objProjectCodeDetail.network_stage, "projectcode", objProjectCodeDetail.project_code);
                if (IsProjectCodeUnique > 0)
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "this project code alredy exists";

                    objProjectCodeDetail.pageMsg = objMsg;

                    return PartialView("_AddProjectDetail", objProjectCodeDetail);
                }

            }


            var output = new BLProject().SaveEntityProjectCodeDetails(objProjectCodeDetail);

          
            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Project Code Detail Saved successfully!";
              
            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Project Code Detail Updated successfully!";
            }

            objProjectCodeDetail.pageMsg = objMsg;

            return PartialView("_AddProjectDetail", objProjectCodeDetail);
        }


        [HttpPost]
        public ActionResult SaveWorkorderCode(WorkorderCodeMaster objWorkorderCodeDetail)
        {

            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            objWorkorderCodeDetail.user_id = Convert.ToInt32(Session["user_id"]);


            if (objWorkorderCodeDetail.network_stage.ToLower() == "planned")
            {
                objWorkorderCodeDetail.network_stage = "P";
            }

            if (objWorkorderCodeDetail.network_stage.ToLower() == "as built")
            {
                objWorkorderCodeDetail.network_stage = "A";
            }

            var isNew = objWorkorderCodeDetail.system_id > 0 ? false : true;


            if (isNew)
            {
                int IsWorkorderCodeUnique = new BLProject().IsCodeExistsForProjSpeci(objWorkorderCodeDetail.network_stage, "workordercode", objWorkorderCodeDetail.workorder_code);

                if (IsWorkorderCodeUnique > 0)
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "this workorder code alredy exists";

                    objWorkorderCodeDetail.pageMsg = objMsg;

                    return PartialView("_AddWorkorderDetail", objWorkorderCodeDetail);

                }

            }


            
            objWorkorderCodeDetail = new BLProject().SaveEntityWorkorderCodeDetails(objWorkorderCodeDetail);

            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Workorder Code Detail Saved successfully!";
             
            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Workorder Code Detail Updated successfully!";
            }
          

            objWorkorderCodeDetail.pageMsg = objMsg;

            return PartialView("_AddWorkorderDetail", objWorkorderCodeDetail);

        }



        [HttpPost]
        public ActionResult SavePurposeCode(PurposeCodeMaster objPurposeCodeDetail)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

           objPurposeCodeDetail.user_id = Convert.ToInt32(Session["user_id"]);


            if (objPurposeCodeDetail.network_stage.ToLower() == "planned")
            {
                objPurposeCodeDetail.network_stage = "P";
            }

            if (objPurposeCodeDetail.network_stage.ToLower() == "as built")
            {
                objPurposeCodeDetail.network_stage = "A";
            }

            var isNew = objPurposeCodeDetail.system_id > 0 ? false : true;

            if (isNew)
            {
                int IsPurposeCodeUnique = new BLProject().IsCodeExistsForProjSpeci(objPurposeCodeDetail.network_stage, "purposecode", objPurposeCodeDetail.purpose_code);

                if (IsPurposeCodeUnique > 0)
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "this purpose code alredy exists";

                    objPurposeCodeDetail.pageMsg = objMsg;

                    return PartialView("_AddPurposeDetail", objPurposeCodeDetail);

                }
            }

          
            objPurposeCodeDetail = new BLProject().SaveEntityPurposeCodeDetails(objPurposeCodeDetail);

            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Purpose Code Detail Saved successfully!";
              
            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Purpose Code Detail Updated successfully!";
            }
          

            objPurposeCodeDetail.pageMsg = objMsg;

            return PartialView("_AddPurposeDetail", objPurposeCodeDetail);

        }

        public IList<KeyValueDropDown> BindNetworkStage(ListTemplateForDropDown objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "-Select-", value = "" });
            items.Add(new KeyValueDropDown { key = "Planned", value = "P" });
            items.Add(new KeyValueDropDown { key = "As Built", value = "A" });
            return objTemplateForDropDown.lstNetworkstage = items;
        }

        public IList<KeyValueDropDown> BindSearchBy(ListTemplateForDropDown objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown {key = "Project Name",   value = "project_name"});
            items.Add(new KeyValueDropDown {key = "Project Code",   value = "project_code"});
            items.Add(new KeyValueDropDown {key = "Planning Name",  value = "planing_name"});
            items.Add(new KeyValueDropDown {key = "Planning Code",  value = "planing_code"});
            items.Add(new KeyValueDropDown {key = "Purpose Name",   value = "purpose_name"});
            items.Add(new KeyValueDropDown {key = "Purpose Code",   value = "purpose_code"});
            items.Add(new KeyValueDropDown {key = "Workorder Name", value = "workorder_name"});
            items.Add(new KeyValueDropDown { key = "Workorder Code", value = "workorder_code" });

            return objTemplateForDropDown.lstBindSearchBy = items.OrderBy(m=> m.key).ToList();

        }


        public IList<KeyValueDropDown> BindProject(string network_stage ="")
        {
            ListTemplateForDropDown objTemplateForDropDown = new ListTemplateForDropDown();
            return objTemplateForDropDown.lstBindProject = new BLProject().BindProject(network_stage).OrderBy(m=> m.key).ToList();
        }

        public JsonResult BindProjectOnChange(string network_stage)
        {
            var objResp = BindProject(network_stage);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public IList<KeyValueDropDown> BindPlanning(string network_stage = "", int ddlproject_id =0)
        {
            ListTemplateForDropDown objTemplateForDropDown = new ListTemplateForDropDown();
            return objTemplateForDropDown.lstBindPlanning = new BLProject().BindPlanning(network_stage, ddlproject_id).OrderBy(m=> m.key).ToList();
        }

        public JsonResult BindPlanningOnChange(string network_stage, int ddlproject_id=0)
        {
            var objResp = BindPlanning(network_stage, ddlproject_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }


        public IList<KeyValueDropDown> BindWorkorder(string network_stage = "", int ddlplanning_id = 0)
        {
            ListTemplateForDropDown objTemplateForDropDown = new ListTemplateForDropDown();
            return objTemplateForDropDown.lstBindWorkorder = new BLProject().BindWorkorder(network_stage, ddlplanning_id).OrderBy(m=> m.key).ToList();
        }


        public JsonResult BindWokorderOnChange(string network_stage, int ddlplanning_id = 0)
        {
            var objResp = BindWorkorder(network_stage, ddlplanning_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }


        public IList<KeyValueDropDown> BindPurpose(string network_stage = "", int ddlworkorder_id = 0)
        {
            ListTemplateForDropDown objTemplateForDropDown = new ListTemplateForDropDown();
            return objTemplateForDropDown.lstBindPurpose = new BLProject().BindPurpose(network_stage, ddlworkorder_id).OrderBy(m=> m.key).ToList();
        }

        public JsonResult BindPurposeOnChange(string network_stage, int ddlworkorder_id = 0)
        {
            var objResp = BindPurpose(network_stage, ddlworkorder_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public ActionResult ViewProject(ViewProjectDetailsList model, int page = 0, string sort = "", string sortdir = "")
        {
            BindSearchBy(model);
            BindNetworkStage(model);
          
               
            model.viewProjectDetail.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            model.viewProjectDetail.currentPage = page == 0 ? 1 : page;

            model.viewProjectDetail.sort = sort;
            model.viewProjectDetail.orderBy = sortdir;

            model.ProjectDetailList = new BLProject().GetProjectDetailsList(model);
            model.viewProjectDetail.totalRecord = model.ProjectDetailList.Count > 0 ? model.ProjectDetailList[0].totalRecords : 0;
            
            return View("ViewProject", model);

        }

        // Validation Pending if project's details being used furture then it should not be delete (Not decided yet)
        //[HttpPost]
        //public JsonResult DeleteProjectDetailById(int id)
        //{
        //    JsonResponse<string> objResp = new JsonResponse<string>();
        //    try
        //    {
        //        var output = new BLProject().DeleteProjectById(id);
        //        if (output > 0)
        //        {
        //            objResp.status = ResponseStatus.OK.ToString();
        //            objResp.message = "Project Detail Deleted successfully!";
        //        }
        //        else
        //        {
        //            objResp.status = ResponseStatus.FAILED.ToString();
        //            objResp.message = "Something went wrong while deleting Project!";
        //        }
        //    }
        //    catch
        //    {
        //        objResp.status = ResponseStatus.ERROR.ToString();
        //        objResp.message = "Project not deleted!";
        //    }
        //    return Json(objResp, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public JsonResult DeleteProjectCodeDetailById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var IsProjectCodeUsing = new BLProject().getPlanningCodeDetailById(0,id);

                if(IsProjectCodeUsing != null)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Cannot be delete because it is using with planning code:- " + IsProjectCodeUsing.planning_name + " ";
                }

                else
                { 
                var output = new BLProject().DeleteProjectCodeDetailById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Project Detail Deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting Project!";
                }
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Project not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeletePlanningCodeDetailById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var IsPlanningCodeUsing_inworkorder = new BLProject().getWorkorderCodeDetailById(0, id);


                if (IsPlanningCodeUsing_inworkorder != null)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Cannot be delete because it is using with workorder code:- " + IsPlanningCodeUsing_inworkorder.workorder_name + " ";
                }

                else
                {
                    var output = new BLProject().DeletePlanningCodeDetailById(id);
                    if (output > 0)
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                        objResp.message = "Planning Code Detail Deleted successfully!";
                    }
                    else
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = "Something went wrong while deleting Project!";
                    }
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Planning Code not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteWorkorderCodeDetailById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var IsWorkorderCodeUsing_inpurpose = new BLProject().getPurposeCodeDetailById(0, id);


                if (IsWorkorderCodeUsing_inpurpose != null)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Cannot be delete because it is using with purpose code:- " + IsWorkorderCodeUsing_inpurpose + " ";
                }

                else
                {
                    var output = new BLProject().DeleteWorkorderCodeDetailById(id);
                    if (output > 0)
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                        objResp.message = "Workorder Code Detail Deleted successfully!";
                    }
                    else
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = "Something went wrong while deleting Project!";
                    }
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Workorder Code not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

         [HttpPost]
        public JsonResult DeletePurposeCodeDetailById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BLProject().DeletePurposeCodeDetailById(id);
                 if (output > 0)
                 {
                     objResp.status = ResponseStatus.OK.ToString();
                     objResp.message = "Purpose Code Detail Deleted successfully!";
                 }
                 else
                 {
                     objResp.status = ResponseStatus.FAILED.ToString();
                     objResp.message = "Something went wrong while deleting Purpose Code!";
                 }
             }
           
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Purpose Code not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        
       


	}
}