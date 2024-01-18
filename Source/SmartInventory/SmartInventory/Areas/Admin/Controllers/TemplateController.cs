using BusinessLogics;
using Models;
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
    public class TemplateController : Controller
    {
        public ActionResult AddTemplate(int id = 0)
        {
            TemplateViewModel objTemplateViewModel = new TemplateViewModel();
            objTemplateViewModel.template_Id = id;
            objTemplateViewModel = GetTemplatePermissionViewModal(objTemplateViewModel);
            objTemplateViewModel.templateList = bindTemplateDropDown();
            return View(objTemplateViewModel);
        }
        public List<SelectListItem> bindTemplateDropDown()
        {
            List<SelectListItem> lstTemplate = new List<SelectListItem>();
            var templateList = new BLTemplate().GetAllTemplates();
            if (templateList.Count > 0)
            {
                lstTemplate = templateList.Select(m => new SelectListItem() { Text = m.template_name, Value = m.id.ToString() }).ToList();
            }
            return lstTemplate;
        }
        [HttpPost]
        public ActionResult SaveTemplate(TemplateViewModel objModel)
        {
            
            PageMessage objMsg = new PageMessage();
            ModelState.Clear();
            bool IsNew = (objModel.template_Id == 0);
            int usrId = Convert.ToInt32(Session["user_id"]);
            try
            {
                objModel = new BLTemplate().SaveTemplate(objModel, usrId);
                if (objModel.template_Id > 0)
                {
                    objModel.objRoleMaster.lstTemplatePermission = GetTemplatePermissionData(objModel.lstTemplatePermission);
                    if (objModel.objRoleMaster.lstTemplatePermission.Count > 0)
                    {
                        var isSuccess = new BLTemplatePermission().SaveTemplatePermission(objModel.objRoleMaster.lstTemplatePermission, objModel.template_Id, usrId, IsNew);
                        if (isSuccess && IsNew)
                        {
                            objMsg.status = ResponseStatus.OK.ToString();
                            objMsg.message = "Template created successfully!";
                        }
                        else if (isSuccess && !IsNew)
                        {
                            objMsg.status = ResponseStatus.OK.ToString();
                            objMsg.message = "Template updated successfully!";
                        }
                    }
                }
                else
                {
                    objMsg.status = ResponseStatus.ERROR.ToString();
                    objMsg.message = "Template Name is already exist!";
                }
            }
            catch (Exception ex)
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Something went wrong while creating template!!";
            }
            TemplateViewModel objTemplateNew = new TemplateViewModel();
            objTemplateNew.template_Id = objModel.template_Id;
            objTemplateNew.pageMsg = objMsg;
            objTemplateNew = GetTemplatePermissionViewModal(objTemplateNew);
            objTemplateNew.templateList = bindTemplateDropDown();
            return View("AddTemplate", objTemplateNew);

        }
        private List<LayerRightsTemplatePermission> GetTemplatePermissionData(List<LayerRightsTemplatePermission> lstRoleTemplatePermission)
        {
            List<LayerRightsTemplatePermission> lstTemplatePermission = new List<LayerRightsTemplatePermission>();
            if (lstRoleTemplatePermission.Count > 0)
            {
                foreach (var entity in lstRoleTemplatePermission)
                {
                    lstTemplatePermission.Add(new LayerRightsTemplatePermission
                    {
                        layer_id = entity.layer_id,
                        entity_name = entity.entity_name,
                        network_status = NetworkStatus.P.ToString(),

                        add = entity.add_planned,
                        edit = entity.update_planned,
                        delete = entity.delete_planned,
                        viewonly = entity.view_planned
                    });

                    //Add As built Permission 
                    lstTemplatePermission.Add(new LayerRightsTemplatePermission
                    {
                        layer_id = entity.layer_id,
                        entity_name = entity.entity_name,
                        network_status = NetworkStatus.A.ToString(),
                        add = entity.add_asbuilt,
                        edit = entity.update_asbuilt,
                        delete = entity.delete_asbuilt,
                        viewonly = entity.view_asbuilt
                    });


                    //Add Dorment Permission 
                    lstTemplatePermission.Add(new LayerRightsTemplatePermission
                    {
                        layer_id = entity.layer_id,
                        entity_name = entity.entity_name,
                        network_status = NetworkStatus.D.ToString(),
                        add = entity.add_dormant,
                        edit = entity.update_dormant,
                        delete = entity.delete_dormant,
                        viewonly = entity.view_dormant
                    });
                }
            }
            return lstTemplatePermission;
        }
        public ActionResult ViewTemplates(ViewLayerRightsTempModel objViewTempMaster, int page = 0, string sort = "", string sortdir = "")
        {
            if (sort != "" || page != 0)
            {
                objViewTempMaster.objGridAttributes = (CommonGridAttributes)Session["temp_gridAttr"];
            }
            objViewTempMaster.lstSearchBy = GetTempSearchByColumns();
            objViewTempMaster.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewTempMaster.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewTempMaster.objGridAttributes.sort = sort;
            objViewTempMaster.objGridAttributes.orderBy = sortdir;
            objViewTempMaster.lstLayerRightTemplates = new BLTemplate().GetAllTemplates(objViewTempMaster.objGridAttributes);
            objViewTempMaster.objGridAttributes.totalRecord = objViewTempMaster.lstLayerRightTemplates != null && objViewTempMaster.lstLayerRightTemplates.Count > 0 ? objViewTempMaster.lstLayerRightTemplates[0].totalRecords : 0;
            Session["temp_gridAttr"] = objViewTempMaster.objGridAttributes;
            return View("ViewTemplates", objViewTempMaster);
        }
        public List<KeyValueDropDown> GetTempSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Template Name", value = "template_name" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }
        public JsonResult deleteTemplate(int id)
        {
            var resp = new BLTemplate().DeleteLayerTemplate(id);
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public TemplateViewModel GetTemplatePermissionViewModal(TemplateViewModel objTemplateViewModel)
        {
            if (objTemplateViewModel.template_Id > 0)
            {
                objTemplateViewModel = getTemplateInfo(objTemplateViewModel);
                if (objTemplateViewModel.objRoleMaster.lstTemplatePermission.Count > 0)
                { objTemplateViewModel.lstTemplatePermission = objTemplateViewModel.objRoleMaster.lstTemplatePermission; }
                //else if (objTemplateViewModel.lstTemplatePermission.Count > 0) { }
            }
            else
            {
                var lstNetworkLayers = new BLLayer().GetLayerDetails();
                lstNetworkLayers = lstNetworkLayers.Where(m => m.is_layer_for_rights_permission == true).ToList();
                objTemplateViewModel.objRoleMaster.lstTemplatePermission = lstNetworkLayers.Select(m => new LayerRightsTemplatePermission() { layer_id = m.layer_id, entity_name = m.layer_title }).OrderBy(m => m.entity_name).ToList();
            }

            return objTemplateViewModel;
        }
        private TemplateViewModel getTemplateInfo(TemplateViewModel objTempViewModal)
        {
            LayerRightsTemplateMaster objRoleTemplateModel = new BLTemplate().GetTemplate(objTempViewModal.template_Id);
            objTempViewModal.template_name = objRoleTemplateModel.template_name;
            var template = new BLTemplate().getTemplate_with_permission(objTempViewModal.template_Id);
            if (template.lstTemplatePermission.Count > 0)
            {
                IEnumerable<IGrouping<int, LayerRightsTemplatePermission>> permissionGroup = template.lstTemplatePermission.GroupBy(m => m.layer_id).ToList();

                foreach (var lstitem in permissionGroup)
                {
                    LayerRightsTemplatePermission p = new LayerRightsTemplatePermission();

                    foreach (LayerRightsTemplatePermission item in lstitem)
                    {
                        p.layer_id = item.layer_id;
                        p.entity_name = item.entity_name;
                        p.template_id = item.template_id;
                        p.created_by = item.created_by;
                        p.created_on = item.created_on;
                        p.network_status = item.network_status;
                        p.id = item.id;

                        if (item.network_status == NetworkStatus.P.ToString())
                        {
                            p.add_planned = item.add;
                            p.update_planned = item.edit;
                            p.delete_planned = item.delete;
                            p.view_planned = item.viewonly;
                        }
                        if (item.network_status == NetworkStatus.A.ToString())
                        {
                            p.add_asbuilt = item.add;
                            p.update_asbuilt = item.edit;
                            p.delete_asbuilt = item.delete;
                            p.view_asbuilt = item.viewonly;
                        }
                        if (item.network_status == NetworkStatus.D.ToString())
                        {
                            p.add_dormant = item.add;
                            p.update_dormant = item.edit;
                            p.delete_dormant = item.delete;
                            p.view_dormant = item.viewonly;
                        }

                    }

                    // objTempViewModal.lstTemplatePermission.Add(p);
                    objTempViewModal.objRoleMaster.lstTemplatePermission.Add(p);

                }
            }
            return objTempViewModal;
        }

        [HttpPost]
        public JsonResult checkTemplateExists(string templateName, int templateId)
        {
            PageMessage obj = new PageMessage();
            bool IsTempExist = new BLTemplate().checkTemplateExists(templateName, templateId);
            if (IsTempExist)
            {
                return Json(new { status = ResponseStatus.FAILED.ToString(), message = "Template Name already Exists!" });
            }
            else
            {
                return Json(new { status = ResponseStatus.OK.ToString() });
            }
        }
    }
}