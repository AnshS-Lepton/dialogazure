using System.Web.Mvc;
using BusinessLogics;
using Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;
using System;
using System.Text;
using System.Data;
using System.IO;
using SmartInventory.Filters;
using SmartInventory.Settings;
using System.Data.Entity.Infrastructure;
//using static iTextSharp.text.pdf.AcroFields;
//using System.Data.Entity.Infrastructure;
//using System.Web.Security;
//using Utility.MapPrinter;
//using static Mono.Security.X509.X520;
//using NPOI.SS.Formula.Functions;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class RolesController : Controller
    {
        private List<RoleMaster> roles;

        public ActionResult AddRole(int id = 0)
        {
            RoleViewModel objRoleViewModel = new RoleViewModel();
            objRoleViewModel.objRoleMaster.role_id = id;
            objRoleViewModel = GetRoleModelWithPermission(objRoleViewModel);
            GlobalSetting globalSetting = new BLGlobalSetting().getValueFullText("IsCrossDomainAllowed");
            if (globalSetting != null)
            {
                objRoleViewModel.IsCrossDomainAllowed = globalSetting.value == "1" ? true : false;
            }
            return View(objRoleViewModel);
        }
        private RoleViewModel GetRoleModelWithPermission(RoleViewModel objRoleViewModel)
        {
            objRoleViewModel.objRoleMaster.lstTemplateTicketTypePermission = new BLTicketTypeRoleMapping().GetTicketTypeRoleMapping(objRoleViewModel.objRoleMaster.role_id);


            var template = new BLRoles().getRole_with_permission(objRoleViewModel.objRoleMaster.role_id);
            if (template != null)
            {
                objRoleViewModel.objRoleMaster.role_name = template.role_name;
                objRoleViewModel.objRoleMaster.remarks = template.remarks;
                objRoleViewModel.objRoleMaster.reporting_role_id = template.reporting_role_id;
                objRoleViewModel.objRoleMaster.is_active = template.is_active;
                objRoleViewModel.objRoleMaster.reporting_role_name = template.reporting_role_name;
                if (template.lstTemplatePermission != null)
                {
                    objRoleViewModel.objRoleMaster.lstTemplatePermission = GetBindPermissionList(template.lstTemplatePermission);
                }
                else
                {
                    var lstNetworkLayers = new BLLayer().GetLayerDetails();
                    lstNetworkLayers = lstNetworkLayers.Where(m => m.is_layer_for_rights_permission == true).ToList();
                    objRoleViewModel.objRoleMaster.lstTemplatePermission = lstNetworkLayers.Select(m => new LayerRightsTemplatePermission() { layer_id = m.layer_id, entity_name = m.layer_name }).OrderBy(m => m.entity_name).ToList();
                }
            }
            objRoleViewModel.templateList = new TemplateController().bindTemplateDropDown();
            objRoleViewModel.lstRoles = bindRoleDropDown(objRoleViewModel.objRoleMaster.role_id);
            objRoleViewModel.multi_reporting_role_ids = string.Join(",", new BLReportingRoleMapping().GetReportingRoleMapping(objRoleViewModel.objRoleMaster.role_id).Select(x => x.reporting_role_id).ToList());
            if (string.IsNullOrEmpty(objRoleViewModel.multi_reporting_role_ids))
            {

                objRoleViewModel.multi_reporting_role_ids = Convert.ToString(objRoleViewModel.objRoleMaster.reporting_role_id);
            }
            return objRoleViewModel;
        }

        private List<SelectListItem> bindRoleDropDown(int roleId = 0)
        {//, Selected = (m.role_id == m.reporting_role_id)
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            var roles = new BLRoles().GetAllRoles();        
            if (roles.Count > 0)
            {


                lstRoles = roles.Where(m => m.role_id != roleId).Select(m => new SelectListItem() { Text = m.role_name, Value = m.role_id.ToString() }).ToList();
            }
            return lstRoles;
        }
        public List<LayerRightsTemplatePermission> GetBindPermissionList(List<LayerRightsTemplatePermission> tmpList)
        {
            List<LayerRightsTemplatePermission> List = new List<LayerRightsTemplatePermission>();
            IEnumerable<IGrouping<int, LayerRightsTemplatePermission>> permissionGroup = tmpList.GroupBy(m => m.layer_id).ToList();
            foreach (var lstitem in permissionGroup)
            {
                LayerRightsTemplatePermission p = new LayerRightsTemplatePermission();
                foreach (LayerRightsTemplatePermission item in lstitem)
                {
                    p.layer_id = item.layer_id;
                    p.entity_name = item.entity_name;
                    p.role_id = item.role_id;
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
                List.Add(p);
            }
            return List;
        }

        public PartialViewResult ViewLayerRightsTemplate(int id = 0)
        {
            TemplateViewModel objTempModal = new TemplateViewModel();
            objTempModal.template_Id = id;
            objTempModal = new TemplateController().GetTemplatePermissionViewModal(objTempModal);
            RoleMaster objRoleMaster = new RoleMaster();
            objRoleMaster.lstTemplatePermission = objTempModal.lstTemplatePermission;
            return PartialView("_ViewRoleTemplatePermission", objTempModal.objRoleMaster);


        }
        public ActionResult LoadRoleTemplatePermission(int role_id, bool isDisabled = false)
        {
            RoleViewModel objRoleViewModel = new RoleViewModel();
            objRoleViewModel.objRoleMaster.role_id = role_id;
            objRoleViewModel.objRoleMaster.isDisabled = isDisabled;
            objRoleViewModel.lstRoles = bindRoleDropDown(role_id);
            objRoleViewModel = GetRoleModelWithPermission(objRoleViewModel);

            return PartialView("_ViewRoleTemplatePermission", objRoleViewModel.objRoleMaster);
        }
        [HttpPost]
        public ActionResult SaveRole(RoleViewModel objModel)
        {
            //RoleViewModel objRoleViewModel = new RoleViewModel();
            ModelState.Clear();
            bool IsNew = (objModel.objRoleMaster.role_id == 0);
            PageMessage objMsg = new PageMessage();
            int usrId = Convert.ToInt32(Session["user_id"]);
            bool IsRoleExist = false;
            List<ReportingRoleMapping> lstReportingRoleMapping = new List<ReportingRoleMapping>();
            List<TicketTypeRoleMapping> lstTicketTypeRoleMapping = new List<TicketTypeRoleMapping>();
            try
            {
                IsRoleExist = new BLRoles().CheckRoleExist(objModel.objRoleMaster.role_name, objModel.objRoleMaster.role_id);
                if (IsRoleExist)
                {
                    objMsg.status = ResponseStatus.ERROR.ToString();
                    objMsg.message = "Role Name is already exist!";
                    objModel.pageMsg = objMsg;
                    objModel.templateList = new TemplateController().bindTemplateDropDown();
                    objModel.lstRoles = bindRoleDropDown(objModel.objRoleMaster.role_id);
                    objModel.objRoleMaster.lstTemplatePermission = objModel.lstTemplatePermission;
                    return View("AddRole", objModel);
                }
                objModel = new BLRoles().SaveRole(objModel, usrId);
                             
                //-------------------multiroleId------------------
                // if (objModel.multi_reporting_role_ids != null && objModel.is_multi_role_allowed == true)
                // {
                var listRole = objModel.multi_reporting_role_ids.Split(',').ToList();
                    if (listRole.Count > 0)
                    {
                        foreach (string item in listRole)
                        {
                            lstReportingRoleMapping.Add(new ReportingRoleMapping() { role_id = objModel.objRoleMaster.role_id, reporting_role_id = Convert.ToInt32(item) });
                        }
                    }

                lstReportingRoleMapping = new BLReportingRoleMapping().SaveReportingRoleMapping(lstReportingRoleMapping, objModel.objRoleMaster.role_id);
                //------------------------------------------------end ReportingRoleMapping
                foreach (var objTtype in objModel.objRoleMaster.lstTemplateTicketTypePermission)
                {
                    //Add Planned Permission 
                    lstTicketTypeRoleMapping.Add(new TicketTypeRoleMapping
                    {
                        ticket_type_id = objTtype.ticket_type_id,
                        ticket_type = objTtype.ticket_type,
                        role_id = objModel.objRoleMaster.role_id,
                        created_by = objTtype.created_by,
                        created_on = objTtype.created_on,
                        modified_by = usrId,
                        modified_on = DateTimeHelper.Now,
                        is_create = objTtype.is_create,
                        is_edit = objTtype.is_edit,
                        is_view = objTtype.is_view,
                        is_approve = objTtype.is_approve


                    }); ;                  
                }
              
               lstTicketTypeRoleMapping = new BLTicketTypeRoleMapping().SaveTicketTypeRoleMapping(lstTicketTypeRoleMapping, objModel.objRoleMaster.role_id);
                //-------------------------------------------------------end TicketTypeRoleMapping

                if (objModel.objRoleMaster.role_id > 0)
                {
                    objModel.objRoleMaster.lstTemplatePermission = objModel.lstTemplatePermission;
                    List<RolePermissionEntity> lstRoleLayersMapping = new List<RolePermissionEntity>();
                    lstRoleLayersMapping = SetRolePermissionList(objModel.objRoleMaster.lstTemplatePermission);
                    if (lstRoleLayersMapping.Count > 0)
                    {
                        var isSuccess = new BLRolesLayersMapping().SaveRoleLayerMapping(lstRoleLayersMapping, objModel.objRoleMaster.role_id, usrId, IsNew);
                        if (isSuccess && IsNew)
                        {
                            objMsg.status = ResponseStatus.OK.ToString();
                            objMsg.message = "Role created successfully!";
                        }
                        else if (isSuccess && !IsNew)
                        {
                            objMsg.status = ResponseStatus.OK.ToString();
                            objMsg.message = "Role updated successfully!";
                        }
                    }
                }
                else
                {
                    objMsg.status = ResponseStatus.ERROR.ToString();
                    objMsg.message = "Role Name is already exist!";
                }
            }
            catch (Exception ex)
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Something went wrong while creating role!!";
            }

            objModel.pageMsg = objMsg;
            objModel.templateList = new TemplateController().bindTemplateDropDown();
            objModel.lstRoles = bindRoleDropDown(objModel.objRoleMaster.role_id);
            return View("AddRole", objModel);

        }
        public List<RolePermissionEntity> SetRolePermissionList(List<LayerRightsTemplatePermission> lstLayerRightsTempPermission)
        {
            // lstLayerRightsTempPermission.Where(m => m.add != false || m.delete != false || m.edit != false || m.viewonly != false);
            List<RolePermissionEntity> lstRolePermission = new List<RolePermissionEntity>();
            foreach (var objLayer in lstLayerRightsTempPermission)
            {
                //Add Planned Permission 
                lstRolePermission.Add(new RolePermissionEntity
                {
                    layer_id = objLayer.layer_id,
                    entity_name = objLayer.entity_name,
                    network_status = NetworkStatus.P.ToString(),
                    role_id = objLayer.role_id,
                    created_by = objLayer.created_by,
                    created_on = objLayer.created_on,


                    add = objLayer.add_planned,
                    edit = objLayer.update_planned,
                    delete = objLayer.delete_planned,
                    viewonly = objLayer.view_planned
                });

                //Add As built Permission 
                lstRolePermission.Add(new RolePermissionEntity
                {
                    layer_id = objLayer.layer_id,
                    entity_name = objLayer.entity_name,
                    network_status = NetworkStatus.A.ToString(),
                    role_id = objLayer.role_id,
                    created_by = objLayer.created_by,
                    created_on = objLayer.created_on,

                    add = objLayer.add_asbuilt,
                    edit = objLayer.update_asbuilt,
                    delete = objLayer.delete_asbuilt,
                    viewonly = objLayer.view_asbuilt
                });


                //Add Dorment Permission 
                lstRolePermission.Add(new RolePermissionEntity
                {
                    layer_id = objLayer.layer_id,
                    entity_name = objLayer.entity_name,
                    network_status = NetworkStatus.D.ToString(),
                    role_id = objLayer.role_id,
                    created_by = objLayer.created_by,
                    created_on = objLayer.created_on,
                    add = objLayer.add_dormant,
                    edit = objLayer.update_dormant,
                    delete = objLayer.delete_dormant,
                    viewonly = objLayer.view_dormant
                });
            }
            return lstRolePermission;
        }

        public ActionResult ViewRoles(RoleViewModel objViewRoleMaster, int page = 0, string sort = "", string sortdir = "")
        {
            if (sort != "" || page != 0)
            {
                objViewRoleMaster.objGridAttributes = (CommonGridAttributes)Session["gridAttr"];
            }

            // objViewRoleMaster.objGridAttributes.searchText = ViewBag.searchText;
            objViewRoleMaster.lstSearchBy = GetRoleSearchByColumns();
            objViewRoleMaster.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewRoleMaster.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewRoleMaster.objGridAttributes.sort = sort;

            objViewRoleMaster.objGridAttributes.orderBy = sortdir;
            objViewRoleMaster.lstRoleMaster = new BLRoles().GetAllRoles(objViewRoleMaster.objGridAttributes);
            objViewRoleMaster.objGridAttributes.totalRecord = objViewRoleMaster.lstRoleMaster != null && objViewRoleMaster.lstRoleMaster.Count > 0 ? objViewRoleMaster.lstRoleMaster[0].totalRecords : 0;
            Session["gridAttr"] = objViewRoleMaster.objGridAttributes;
            return View("ViewRoles", objViewRoleMaster);
        }
        public ActionResult ViewReportingRoleManagerListByRoleId(int role_id, int page = 0, string sort = "", string sortdir = "")
        {
            RoleViewModel objViewRoleMaster = new RoleViewModel();
            objViewRoleMaster.objRoleMaster.role_id = role_id;
            objViewRoleMaster.lstSearchBy = GetRoleSearchByColumns();
            objViewRoleMaster.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewRoleMaster.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewRoleMaster.objGridAttributes.sort = sort;

            objViewRoleMaster.objGridAttributes.orderBy = sortdir;
            objViewRoleMaster.lstRoleMaster = new BLRoles().GetReportingRoleByRoleId(role_id, objViewRoleMaster.objGridAttributes);
            objViewRoleMaster.objGridAttributes.totalRecord = objViewRoleMaster.lstRoleMaster != null && objViewRoleMaster.lstRoleMaster.Count > 0 ? objViewRoleMaster.lstRoleMaster[0].totalRecords : 0;

            return PartialView("_ViewReportingRoleManager", objViewRoleMaster);
        }

        public ActionResult UpdateRoleModule(int update_Status = -1)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            PageMessage objMsg = new PageMessage();
            RoleModuleMapping mapping = new RoleModuleMapping();
            mapping.lstUserModule = new BLMisc().GetUserModuleMasterList();
            mapping.lstRole = new BLUser().GetAllRole(objLgnUsrDtl.role_id, objLgnUsrDtl.user_id);
            if (update_Status == 1)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Role Module Mapping has been saved successfully!";
            }
            else if (update_Status == 0)
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                objMsg.message = "Something went wrong while mapping role with module!!";
            }
            mapping.pageMsg = objMsg;


            return View(mapping);
        }

        public ActionResult GetRoleModuleMapping(int role_id)
        {

            BLMisc objBLLayer = new BLMisc();
            var model = objBLLayer.GetRoleModule(role_id).Select(x => x.Id).ToArray();
            var lstUserModule = new BLMisc().GetUserModuleMasterList();
            lstUserModule.Where(x => model.Contains(x.Id)).ToList().ForEach(x => x.is_selected = true);
            //lstUserModule = lstUserModule.OrderBy(m => m.module_sequence).ToList();

			return PartialView("_ModuleList", lstUserModule);
        }

        public ActionResult GetUserRoleModuleMapping(int role_id, int user_id)
        {

            BLMisc objBLLayer = new BLMisc();
            var model = objBLLayer.GetRoleModule(role_id);
            if (user_id > 0)
            {
                var allActiveModule = new BLMisc().GetUserModuleMasterList().Select(x => x.Id).ToArray();
                model = model.Where(x => allActiveModule.Contains(x.Id)).ToList();
                var userModuleMapping = new BLUserModuleMapping().GetModuleMapping(user_id);
                foreach (var item in model)
                {
                    var selmoduleId = userModuleMapping.Where(m => m.module_id == item.Id).Select(m => m.module_id);
                    if (selmoduleId.Count() > 0)
                    {
                        item.is_selected = true;


                    }
                }
            }
            return PartialView("_ModuleList", model);
        }


        [HttpPost]
        public ActionResult SaveRoleModuleMapping(RoleModuleMapping mapping, int role_id)
        {
            int usrId = Convert.ToInt32(Session["user_id"]);
            var updateStatus = -1;
            List<RoleSeviceFacilityMapping> lstRoleSeviceFacilityMapping = null;
            List<RoleJoTypeMapping> lstRoleJoTypeMapping = null;
            List<RoleJoCategoryMapping> lstRoleJoCategoryMapping = null;
            List<TicketTypeRoleMapping> lstReportingRoleMapping = new List<TicketTypeRoleMapping>();
            TicketTypeMaster ticketType = new TicketTypeMaster();
            int[] idList = { 77,76 };
            var moduleName = mapping.lstUserModule.Where(t =>t.module_abbr== "NWT" && t.is_selected == true ).Select(x=>x.module_abbr).ToList(); ;
            var isExistTicketType = new BLTicketTypeRoleMapping().GetTicketTypeRoleMapping(role_id);
			
                if (moduleName.Count > 0)
                {
				  if (isExistTicketType.Count == 0)
				  {
                    var ticket_type = "Network_Ticket";
                    // ticketType.module = moduleName;
                    var type = new BLTicketType().GetAllTicketType();
                    var lstId = type.Where(m => m.module == ticket_type).Select(m => new { m.id, m.ticket_type }).ToList();
                    foreach (var item in lstId)
                    {
                        lstReportingRoleMapping.Add(new TicketTypeRoleMapping() { role_id = role_id, ticket_type_id = item.id, created_by = usrId, created_on = DateTimeHelper.Now, ticket_type = item.ticket_type });
                    }
                    lstReportingRoleMapping = new BLTicketTypeRoleMapping().SaveTicketTypeRoleMapping(lstReportingRoleMapping, role_id);
                  }
                }
                else {
                    lstReportingRoleMapping = new BLTicketTypeRoleMapping().SaveTicketTypeRoleMapping(lstReportingRoleMapping, role_id);
                }          
            try
            {
                RoleMaster objRoleMaster = new BLRoles().getRole_with_permission(role_id);
                mapping.lstRoleModuleMapping = mapping.lstUserModule.Where(m => m.is_selected == true).Select(m => new RoleModuleMapping() { module_id = m.Id, role_id = role_id }).ToList();

                //[[Execute the blow code when role name is "WFM Contractor"
                if ((objRoleMaster.role_name.Equals("WFM Contractor", StringComparison.OrdinalIgnoreCase)) || (objRoleMaster.role_name.Equals("COFG Dispatcher", StringComparison.OrdinalIgnoreCase)) || (objRoleMaster.role_name.Equals("Contractor Dispatcher", StringComparison.OrdinalIgnoreCase)) || (objRoleMaster.role_name.Equals("In House Dispatcher", StringComparison.OrdinalIgnoreCase)) || (objRoleMaster.role_name.Equals("MSP Dispatcher", StringComparison.OrdinalIgnoreCase)))
                {
                    if (mapping.lstRoleJobOrderMapping != null)
                    {
                        lstRoleSeviceFacilityMapping = mapping.lstRoleServiceFacilityMapping.Where(m => m.is_selected == true).Select(m => new RoleSeviceFacilityMapping() { service_facility_id = m.id, role_id = role_id }).ToList();
                    }
                    if (mapping.lstRoleJobOrderMapping != null)
                    {
                        lstRoleJoTypeMapping = mapping.lstRoleJobOrderMapping.Where(m => m.is_selected == true).Select(m => new RoleJoTypeMapping() { jo_type_id = m.id, role_id = role_id }).ToList();
                    }
                    if (mapping.lstRoleJobCategoryMapping != null)
                    {
                        lstRoleJoCategoryMapping = mapping.lstRoleJobCategoryMapping.Where(m => m.is_selected == true).Select(m => new RoleJoCategoryMapping() { jo_category_id = m.id, role_id = role_id }).ToList();
                    }
                    lstRoleSeviceFacilityMapping = new BLRoleServiceFacilityMapping().SaveRoleServiceFacilityMapping(lstRoleSeviceFacilityMapping, role_id);
                    lstRoleJoTypeMapping = new BLRoleRoleJoTypeMapping().SaveRoleJoTypeMapping(lstRoleJoTypeMapping, role_id);
                    lstRoleJoCategoryMapping = new BLRoleRoleJoCategoryMapping().SaveRoleJoCategoryMapping(lstRoleJoCategoryMapping, role_id);
                }
                //]]
                mapping.lstRoleModuleMapping = new BLRoleModuleMapping().SaveRoleModuleMapping(mapping.lstRoleModuleMapping, role_id);

                // mapping.role_id = role_id;
                //mapping.lstRole = new BLUser().GetAllRole();
                updateStatus = 1;
            }
            catch (Exception)
            {
                updateStatus = 0;
                throw;
            }
            return RedirectToAction("UpdateRoleModule", new { update_Status = updateStatus });
        }

        public List<KeyValueDropDown> GetRoleSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Role Name", value = "role_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Role Description", value = "remarks" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Reporting Role Name", value = "reporting_role_name" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }
        public JsonResult deleteRole(int id)
        {
            PageMessage objMsg = new PageMessage();
            DbMessage resp = new BLRoles().DeleteRole(id);
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public int CheckRoleAssignToUser(int role_id)
        {
            return new BLUser().CheckRoleAssignToUser(role_id).Count();
        }
        [HttpPost]
        public JsonResult CheckRoleExist(string role_name, int role_id)
        {
            PageMessage obj = new PageMessage();
            bool IsRoleExist = new BLRoles().CheckRoleExist(role_name, role_id);
            if (IsRoleExist)
            {
                return Json(new { status = ResponseStatus.FAILED.ToString(), message = "Role Name already Exists!" });
            }
            else
            {
                return Json(new { status = ResponseStatus.OK.ToString() });
            }
        }


        #region Pankaj
        /////
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRoleServiceFacilityJobOrder(int role_id)
        {
            //int role_id = 142;
            BLMisc objBLLayer = new BLMisc();
            var lstFacilityJobOrder = objBLLayer.GetRoleServiceFacilityJobOrder(role_id);
            return PartialView("_ServiceFacilityJobOrderList", lstFacilityJobOrder);
        }

        public ActionResult GetUserServiceFacilityJobOrder(int role_id,int user_id)
        {
            //int role_id = 142;
            BLMisc objBLLayer = new BLMisc();
            var lstFacilityJobOrder = objBLLayer.GetUserServiceFacilityJobOrder(role_id, user_id);
            return PartialView("_ServiceFacilityJobOrderList", lstFacilityJobOrder);
        }

        public ActionResult GetUserServiceFacilityJobOrderForFE(string rm_id, int user_id)
        {
            //int role_id = 142;
            BLMisc objBLLayer = new BLMisc();
            var lstFacilityJobOrder = objBLLayer.GetUserServiceFacilityJobOrderFE(rm_id, user_id);
            return PartialView("_ServiceFacilityJobOrderList", lstFacilityJobOrder);
        }
        #endregion
    }
}