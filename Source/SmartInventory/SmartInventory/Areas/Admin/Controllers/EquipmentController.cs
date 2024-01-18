using BusinessLogics;
using BusinessLogics.Admin;
using BusinessLogics.ISP;
using Models;
using Models.Admin;
using Newtonsoft.Json;
using SmartInventory.Areas.Admin.Models;
using SmartInventory.Filters;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Utility;
using System.Net;
using System.Configuration;
using Aspose.Svg;
using SharpKml.Dom;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class EquipmentController : Controller
    {
        //GET: Admin/Equipment
        public ActionResult ViewModels(ISPModelInfo objISPModelMaster, int modelID = 0)
        {
            objISPModelMaster.model_template = InitModelTemplate(objISPModelMaster);
            objISPModelMaster.lstEquipments = BLISPModelInfo.Instance.GetAllEquipments();
            return View("_ViewModels", objISPModelMaster);

        }

        public ActionResult GetModelDetails(ISPModelInfo objISPModelMaster, int page = 0, string sort = "", string sortdir = "", int modelID = 0, string searchText = "", string searchBy = "")
        {
            if (sort != "" || page != 0)
            {
                objISPModelMaster.objOptionsEquipmentDetails = (OptionsEquipmentDetails)Session["viewModelEquipment"];
            }

            objISPModelMaster.objOptionsEquipmentDetails.searchText = searchText == "" ? objISPModelMaster.objOptionsEquipmentDetails.searchText : searchText;
            objISPModelMaster.objOptionsEquipmentDetails.searchBy = searchBy == "" ? objISPModelMaster.objOptionsEquipmentDetails.searchBy : searchBy;
            objISPModelMaster.model_id = modelID;
            objISPModelMaster.objOptionsEquipmentDetails.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objISPModelMaster.objOptionsEquipmentDetails.currentPage = page == 0 ? 1 : page;
            objISPModelMaster.objOptionsEquipmentDetails.sort = sort;
            objISPModelMaster.objOptionsEquipmentDetails.orderBy = sortdir;
            objISPModelMaster.lstModelDetails = BLISPModelInfo.Instance.GetModelDetails(objISPModelMaster, modelID, objISPModelMaster.objOptionsEquipmentDetails.searchText, objISPModelMaster.objOptionsEquipmentDetails.searchBy);
            objISPModelMaster.objOptionsEquipmentDetails.totalRecord = objISPModelMaster.lstModelDetails != null && objISPModelMaster.lstModelDetails.Count > 0 ? objISPModelMaster.lstModelDetails[0].totalRecords : 0;
            Session["viewModelEquipment"] = objISPModelMaster.objOptionsEquipmentDetails;
            return PartialView("_ModelDetails", objISPModelMaster);
        }

        public ActionResult GetModelStatusCount(ISPModelInfo objISPModelMaster, int modelID = 0)
        {

            objISPModelMaster.lstModelStatusCount = BLISPModelInfo.Instance.GetModelStatusCount(modelID);
            return PartialView("_ModelStatusCount", objISPModelMaster);
        }


        public JsonResult DeleteModelDetailsById(string modelIds)
        {
            int rows = 0;
            JsonResponse<string> objResp = new JsonResponse<string>();
            string[] values = modelIds.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                if(BLISPModelInfo.Instance.IsEditableModel(Convert.ToInt32(values[i])))
                rows = rows + BLISPModelInfo.Instance.DeleteModelDetailsById(Convert.ToInt32(values[i]));
            }
            objResp.status = ResponseStatus.OK.ToString();
            if(rows > 0)
            objResp.message = rows + " record deleted successfully!";
            if (rows == 0)
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Model could not be deleted because it is used by other model!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }




        public JsonResult SaveModelType(ISPModelTypeMaster record)
        {
            PageMessage pageMessage = new PageMessage();
            record.created_by = Convert.ToInt32(Session["user_id"]);

            if (record.color_code == null) { record.color_code = "transparent"; }
            if (record.stroke_code == null) { record.stroke_code = "transparent"; }

            if (BLISPModelInfo.Instance.CheckModelTypeExists(record.value).Count > 0)
            {
                pageMessage.status = ResponseStatus.FAILED.ToString();
                pageMessage.message = "Same type already exists!";
            }
            else
            {
                BLISPModelInfo.Instance.SaveModelType(record);
                pageMessage.status = ResponseStatus.OK.ToString();
                pageMessage.message = "Details Saved Successfully!";

            }
            return Json(pageMessage, JsonRequestBehavior.AllowGet);
        }



        public JsonResult EditModelInfo(int id)
        {
            ISPModelInfo modelInfo = new ISPModelInfo();
            modelInfo = BLISPModelInfo.Instance.GetModelInfo(id).FirstOrDefault();
            modelInfo.lstModelStatus = BLISPModelInfo.Instance.GetModelStatus();

            modelInfo.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            modelInfo.model_template = InitModelTemplate(modelInfo);
            return Json(modelInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateModel(int? modelId)
        {
            ISPModelInfo modelInfo = new ISPModelInfo();
            modelInfo.is_editable = true;
            if (modelId != null)
            {
                ViewBag.ModelId = modelId;
                modelInfo = BLISPModelInfo.Instance.GetModelInfo(modelId.Value).FirstOrDefault();
                if (modelInfo == null)
                    modelInfo = new ISPModelInfo();
                modelInfo.lstModelType = BLISPModelInfo.Instance.GetModelTypes(modelInfo.model_id);
                modelInfo.is_editable = BLISPModelInfo.Instance.IsEditableModel(modelId.Value);
            }

            modelInfo.lstModelStatus = BLISPModelInfo.Instance.GetModelStatus();
            modelInfo.lstModel = BLISPModelInfo.Instance.GetModels();

            modelInfo.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            modelInfo.model_template = InitModelTemplate(modelInfo);
            modelInfo.lstLabelColor = BLISPModelInfo.Instance.GetColorByModelKey("label");
            return View("CreateModel", modelInfo);
        }

        public ActionResult CreateRule()
        { 
            ISPModelRule objModelRule = new ISPModelRule();
            objModelRule.lstModel = BLISPModelInfo.Instance.GetModels();
            objModelRule.lstMiddleWare = BLISPModelInfo.Instance.GetMiddleWareLayers();
            objModelRule.lstHasTypeModel = BLISPModelInfo.Instance.GetModels().Where(x => x.has_type).ToList();
            return PartialView("_CreateRules",objModelRule);
        }

        public JsonResult checkModalHasType()
        {
            var lstModel = BLISPModelInfo.Instance.GetModels().Where(x => x.has_type == false).ToList();
            return Json(lstModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRule(ISPModelRule objISPModelRule)
        {
            
            PageMessage pageMessage = new PageMessage();
            objISPModelRule.created_by = Convert.ToInt32(Session["user_id"]);
            if (objISPModelRule.parent_model_type_id == 0)
                objISPModelRule.parent_model_type_id = null;
            if(objISPModelRule.child_model_type_id == 0)
                objISPModelRule.child_model_type_id = null;

            if(BLISPModelInfo.Instance.CheckRulesExists(objISPModelRule.parent_model_id,objISPModelRule.parent_model_type_id,objISPModelRule.child_model_id,objISPModelRule.child_model_type_id).Count>0)
            {
                pageMessage.status = ResponseStatus.FAILED.ToString();
                pageMessage.message = "This rule already exists!";
            }
            else
            {
                BLISPModelInfo.Instance.SaveRule(objISPModelRule);
                pageMessage.status = ResponseStatus.OK.ToString();
                pageMessage.message = "Details Saved Successfully!";
            }
            return Json(pageMessage, JsonRequestBehavior.AllowGet);
            
        }
        [HttpPost]
        public JsonResult SaveBulkRule(List<ISPModelRule> ispModelRules)
        {

            PageMessage pageMessage = new PageMessage();
            //ispModelRules.ForEach(x => x.created_by = Convert.ToInt32(Session["user_id"]));
            foreach (var rule in ispModelRules) {
                rule.created_by = Convert.ToInt32(Session["user_id"]);
                if (rule.parent_model_type_id == 0)
                    rule.parent_model_type_id = null;
                if (rule.child_model_type_id == 0)
                    rule.child_model_type_id = null;

                if (BLISPModelInfo.Instance.CheckRulesExists(rule.parent_model_id, rule.parent_model_type_id, rule.child_model_id, rule.child_model_type_id).Count > 0)
                {
                    //pageMessage.status = ResponseStatus.FAILED.ToString();
                    //pageMessage.message = "This rule already exists!";
                }
                else
                {
                    BLISPModelInfo.Instance.SaveRule(rule);
                    //pageMessage.status = ResponseStatus.OK.ToString();
                    //pageMessage.message = "Details Saved Successfully!";
                }
            }
            pageMessage.status = ResponseStatus.OK.ToString();
            pageMessage.message = "Details Saved Successfully!";
            return Json(pageMessage, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetRuleDetails(ISPModelRule objISPModelRule, int page = 0, string sort = "", string sortdir = "", int modelID = 0, string searchText = "", string searchBy = "")
        {

            objISPModelRule.objOptionsRule.pageSize = 5;//ApplicationSettings.ViewAdminDashboardGridPageSize;
            objISPModelRule.objOptionsRule.currentPage = page == 0 ? 1 : page;
            objISPModelRule.objOptionsRule.sort = sort;
            objISPModelRule.objOptionsRule.orderBy = sortdir.Replace("undefined", "");
            objISPModelRule.lstModelRule = BLISPModelInfo.Instance.GetRuleDetails(objISPModelRule, searchText, searchBy);
            objISPModelRule.objOptionsRule.totalRecord = objISPModelRule.lstModelRule != null && objISPModelRule.lstModelRule.Count > 0 ? objISPModelRule.lstModelRule[0].totalRecords : 0;
            return PartialView("_RuleDetails", objISPModelRule);
        }
            
        public ActionResult GetModalTypeDetails(ISPModelTypeMaster objISPModelTypeMaster, int page = 0, string sort = "", string sortdir = "", int modelID = 0, string searchText = "", string searchBy = "")
        {
            if (sort != "" || page != 0)
            {
                objISPModelTypeMaster.objOptionsModelType = (OptionsModelType)Session["viewModelType"];
            }
            objISPModelTypeMaster.objOptionsModelType.searchText = searchText == "" ? objISPModelTypeMaster.objOptionsModelType.searchText : searchText;
            objISPModelTypeMaster.objOptionsModelType.searchBy = searchBy == "" ? objISPModelTypeMaster.objOptionsModelType.searchBy : searchBy;
            objISPModelTypeMaster.objOptionsModelType.pageSize = 5;//ApplicationSettings.ViewAdminDashboardGridPageSize;
            objISPModelTypeMaster.objOptionsModelType.currentPage = page == 0 ? 1 : page;
            objISPModelTypeMaster.objOptionsModelType.sort = sort;
            objISPModelTypeMaster.objOptionsModelType.orderBy = sortdir.Replace("undefined","");
            objISPModelTypeMaster.lstModelType = BLISPModelInfo.Instance.GetModalTypeDetails(objISPModelTypeMaster, objISPModelTypeMaster.objOptionsModelType.searchText, objISPModelTypeMaster.objOptionsModelType.searchBy);
            objISPModelTypeMaster.objOptionsModelType.totalRecord = objISPModelTypeMaster.lstModelType != null && objISPModelTypeMaster.lstModelType.Count > 0 ? objISPModelTypeMaster.lstModelType[0].totalRecords : 0;
         
            Session["viewModelType"] = objISPModelTypeMaster.objOptionsModelType;
            return PartialView("_ModelTypeDetails", objISPModelTypeMaster);
        }

        public JsonResult DeleteModalType(string modeltypeids)
        {
            int rows = 0; List<string> model_type_name = new List<string>();string msg = "";
            List<CheckRulesExists> lstRules = new List<CheckRulesExists>();
            JsonResponse<string> objResp = new JsonResponse<string>();
            string[] values = modeltypeids.Split(',');
            
            for (int i = 0; i < values.Length; i++)
            {
                lstRules = BLISPModelInfo.Instance.CheckModelTypeExists(Convert.ToInt32(values[i]));
                if (lstRules.Count > 0)
                {
                    model_type_name.Add(lstRules[0].model_type_name);
                }
                else
                { 
                    rows = rows + BLISPModelInfo.Instance.DeleteModalType(Convert.ToInt32(values[i]));                    
                    
                }
            }
            if (rows > 0)
            {
                
                msg = rows + " records deleted successfully!";
            }
            if (lstRules.Count >0)
            {
                foreach(var item in model_type_name)
                {
                    msg +=  item + ", ";
                }

                msg +=  " could not be deleted because it is used by other model!";
            }

            objResp.status = ResponseStatus.OK.ToString();
            objResp.message = msg;
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteRule(string ruleids)
        {
            int rows = 0; string msg = "";
            List<CheckRulesExists> lstTypes = new List<CheckRulesExists>();
            JsonResponse<string> objResp = new JsonResponse<string>();
            string[] values = ruleids.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                lstTypes = BLISPModelInfo.Instance.CheckRulesExists(Convert.ToInt32(values[i]));
                if (lstTypes.Count > 0)
                {
                    
                }
                else
                {
                    rows = rows + BLISPModelInfo.Instance.DeleteRule(Convert.ToInt32(values[i]));
                }
                
            }
            if (rows > 0)
            {

                msg = rows + " records deleted successfully!";
            }
            if (lstTypes.Count > 0)
            {
                if(rows==0)
                {
                    msg += "Rules could not be deleted because it is used by other model!";
                }
                else
                {
                    msg += "Some rules could not be deleted because it is used by other model!";
                }
            }
            objResp.status = ResponseStatus.OK.ToString();
            objResp.message = msg;
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditRule(int id)
        {
           List<ISPModelRule> lstRuleInfo = new List<ISPModelRule>();
            lstRuleInfo = BLISPModelInfo.Instance.EditRuleDetails(id);
                        
            return Json(lstRuleInfo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRule(ISPModelRule objISPModelRule)
        {
            PageMessage pageMessage = new PageMessage();
            List<ISPModelRule> lstRuleInfo = new List<ISPModelRule>();
            lstRuleInfo = BLISPModelInfo.Instance.UpdateRuleDetails(objISPModelRule, Convert.ToInt32(Session["user_id"]));
            if (lstRuleInfo.Count == 1)
            {
                pageMessage.status = ResponseStatus.OK.ToString();
                pageMessage.message = "Details Saved Successfully!";
            }
            else
            {
                pageMessage.status = ResponseStatus.FAILED.ToString();
                pageMessage.message = "Please fill required field!";
            }
            return Json(pageMessage, JsonRequestBehavior.AllowGet);
        }


        public JsonResult EditModelType(int id)
        {
            List<ISPModelTypeMaster> lstModelTypeInfo = new List<ISPModelTypeMaster>();
            lstModelTypeInfo = BLISPModelInfo.Instance.EditModelTypeDetails(id);
            return Json(lstModelTypeInfo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateModelType(ISPModelTypeMaster objISPModelTypeMaster)
        {
            PageMessage pageMessage = new PageMessage();
            List<ISPModelTypeMaster> lstRuleInfo = new List<ISPModelTypeMaster>();
            lstRuleInfo = BLISPModelInfo.Instance.UpdateModelTypeDetails(objISPModelTypeMaster, Convert.ToInt32(Session["user_id"]));
            if (lstRuleInfo.Count == 1)
            {
                pageMessage.status = ResponseStatus.OK.ToString();
                pageMessage.message = "Details Updated Successfully!";
            }
            else
            {
                pageMessage.status = ResponseStatus.FAILED.ToString();
                pageMessage.message = "Please fill required field!";
            }
            return Json(pageMessage, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetModelSubType(int modelId)
        {
            var data = BLISPModelInfo.Instance.GetModelTypes(modelId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetModelSubTypeColors(int modelId)
        {
            var data = BLISPModelInfo.Instance.GetColorByModelId(modelId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetPortImages()
        {
            var result = BLISPPort.Instance.GetPortImages();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetModelType(int? modelId)
        {
            if (modelId == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            var result = BLISPModelInfo.Instance.GetModelTypes(modelId.Value);
            var hasTypes = BLISPModelInfo.Instance.ModelHasTypes(modelId.Value);
            //if (hasTypes)
            //{

            //    result.Insert(result.Count, new ISPModelTypeMaster
            //    {
            //        value = "+ Create New Type"
            //    });

            //}
            return Json(new { has_types = hasTypes, result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveModel(ISPModelInfo input)
        {
            PageMessage pageMessage = new PageMessage();
            if (ModelState.IsValid)
            {
                var id = input.id;
                var resultItem = new ISPModelInfo();

                if (BLISPModelInfo.Instance.IsEditableModel(id))
                {
                    resultItem = BLISPModelInfo.Instance.SaveModelInfo(input, Convert.ToInt32(Session["user_id"]));

                    if (id > 0)  // Update 
                    {
                        pageMessage.status = ResponseStatus.OK.ToString();
                        pageMessage.message = input.model_master_name + " updated successfully!";
                    }
                    else
                    {
                        pageMessage.status = ResponseStatus.OK.ToString();
                        pageMessage.message = input.model_master_name + " saved successfully!";
                        input = resultItem;
                    }
                    if (input.lstModelChildren != null && input.lstModelChildren.Count > 0 && input.isChangeMapping)
                    {
                        input.lstModelChildren.ForEach(p =>
                        {
                            if (p.parent_model_mapping_id == 1)
                            {
                                p.modified_by = Convert.ToInt32(Session["user_id"]);
                                p.parent_model_info_id = resultItem.id;
                            }
                            p.child_x_pos = PixelToMM(p.child_x_pos);
                            p.child_y_pos = PixelToMM(p.child_y_pos);
                        });
                        var result = BLISPModelInfo.Instance.SaveModelMapping(resultItem.id, JsonConvert.SerializeObject(input.lstModelChildren));
                    }
                    else {
                        if(input.isChangeMapping)
                        BLISPModelInfo.Instance.DeleteMapBySuperParent(id);
                    }
                }
                else
                {
                    pageMessage.status = ResponseStatus.OK.ToString();
                    pageMessage.message = "This model could not be modified because it is used by other model!";
                }
            }
            else
            {
                pageMessage.status = ResponseStatus.FAILED.ToString();
                pageMessage.message = "Please fill required field!";
            }
            input.page_message = pageMessage;
            return Json(input, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetWorkspaceData(int modelId,bool? isStatic, bool isLibCall = false)
        {
            List<WorkSpaceViewModel> result = new List<WorkSpaceViewModel>();
            List<ISPModelInfo> data = BLISPModelInfo.Instance.GetModelsWithImage(modelId);
            if (data != null && data.Count > 0)
            {
                bool isEditable = BLISPModelInfo.Instance.IsEditableModel(modelId);
                
                List<ISPModelInfo> children = BLISPModelInfo.Instance.GetModelChildren(modelId);
                //Get childrens
                foreach (var child in children)
                {
                    if (isStatic == null)
                        isStatic = child.parent != -1 && child.parent != modelId;
                    result.Add(new WorkSpaceViewModel
                    {
                        id = child.map_id,
                        db_id = child.id,
                        model_id = child.model_id,
                        name = child.alt_name??child.model_name,
                        image_data = child.image_data,
                        img_id = child.model_image_id,
                        height = MMToPixel(child.height),
                        width = MMToPixel(child.width),
                        depth = MMToPixel(child.depth),
                        is_static = isStatic.Value,
                        offset_x = 0,
                        offset_y = 0,
                        color = child.color_code,
                        stroke = child.stroke_code,
                        parent = child.parent,
                        position = new Position
                        {
                            x = MMToPixel(child.child_x_pos),
                            y = MMToPixel(child.child_y_pos)
                        },
                        rotation_angle = child.rotation_angle,
                        model_type_id = child.model_type_id,
                        model_view_id = child.model_view_id,
                        border_width = MMToPixel(child.border_width),
                        is_editable = isEditable,
                        db_border_width = child.border_width,
                        db_depth=child.depth,
                        db_height=child.height,
                        db_width=child.width,
                        db_parent = child.parent,
                        ref_id = child.map_id,
                        ref_parent = child.parent,
                        font_color=child.font_color,
                        font_size=child.font_size,
                        text_orientation=child.text_orientation,
                        bg_image = child.background_image,
                        model_color_id = child.model_color_id,
                        font_weight = child.font_weight,
                        border_color = child.border_color
                    });
                }
            }
            result = result.OrderBy(p => p.parent).ToList();
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        //public JsonResult Delete()
        //{

        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetLibraryData(int modelId, int? modelType)
        {
            List<ISPModelInfo> models = BLISPModelInfo.Instance.GetModelRules(modelId, modelType);
            var results = from m in models
                          group m by m.model_id into g
                          select new { ModelId = g.Key, Types = g.ToList() };
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModelSubTypes(int? parent_id, int? parent_type, int? child_model)
        {
            if (parent_id == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            var result = BLISPModelInfo.Instance.GetModelSubTypes(parent_id, parent_type, child_model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetModelAllRules()
        {
            var result = BLISPModelInfo.Instance.GetModelAllRules();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [NonAction]
        public double MMToPixel(double mm)
        {
            double pixel = 0;
            pixel = mm * ApplicationSettings.WorkspaceCellSize / ApplicationSettings.WorkspaceScale;
            return pixel;


        }
        [NonAction]
        public double PixelToMM(double val)
        {
            double mm = 0;
            mm = val *ApplicationSettings.WorkspaceScale/ ApplicationSettings.WorkspaceCellSize  ;
            return mm;


        }

        [NonAction]
        public ISPModelTemplate InitModelTemplate(ISPModelInfo modelInfo)
        {
            modelInfo.model_template = BLItemTemplate.Instance.GetTemplateDetail<ISPModelTemplate>(Convert.ToInt32(Session["user_id"]), EntityType.Equipment);
            modelInfo.entity_type = EntityType.Equipment.ToString();
            BLItemTemplate.Instance.BindItemDropdowns(modelInfo.model_template, EntityType.Equipment.ToString());
            if (!String.IsNullOrEmpty(modelInfo.modeltypevalue))
            {
                var layerDetail = BLISP.Instance.getLayerDetails(modelInfo.modeltypevalue);
                if (layerDetail != null && layerDetail.is_middleware_entity)
                {
                    modelInfo.entity_type = modelInfo.modeltypevalue;
                    var eType = modelInfo.modeltypevalue;

                    //if (modelInfo.modeltypevalue.ToUpper() == EntityType.HTB.ToString().ToUpper())
                    //{
                    
                        modelInfo.model_template = BLItemTemplate.Instance.GetTemplateDetail<ISPModelTemplate>(Convert.ToInt32(Session["user_id"]), (EntityType)Enum.Parse(typeof(EntityType), eType));
                        BLItemTemplate.Instance.BindItemDropdowns(modelInfo.model_template, eType);
                    //}

                    //TODO:MAKE GENERIC TYPE
                    //if (modelInfo.modeltypevalue.ToUpper() == EntityType.FMS.ToString().ToUpper())
                    //{
                    //    modelInfo.model_template = BLItemTemplate.Instance.GetTemplateDetail<ISPModelTemplate>(Convert.ToInt32(Session["user_id"]), EntityType.FMS);
                    //    BLItemTemplate.Instance.BindItemDropdowns(modelInfo.model_template, EntityType.FMS.ToString());
                    //}
                    //if (modelInfo.modeltypevalue.ToUpper() == EntityType.HTB.ToString().ToUpper())
                    //{
                    //    modelInfo.model_template = BLItemTemplate.Instance.GetTemplateDetail<ISPModelTemplate>(Convert.ToInt32(Session["user_id"]), EntityType.HTB);
                    //    BLItemTemplate.Instance.BindItemDropdowns(modelInfo.model_template, EntityType.HTB.ToString());
                    //}

                }
            }
            if (modelInfo.item_template_id > 0)
            {

                var item = new BLVendorSpecification().GetVendorSpeicificationDetailsByID(modelInfo.item_template_id);
                if (item != null)
                {
                    modelInfo.model_template.lstVendor = BLItemTemplate.Instance.GetVendorList(item.specification);
                    modelInfo.model_template.specification = item.specification;
                    modelInfo.model_template.vendor_id = item.vendor_id;
                    modelInfo.model_template.item_code = item.code;
                    modelInfo.model_template.category = modelInfo.model_template.entityType;
                    modelInfo.model_template.subcategory1 = item.subcategory_1;
                    modelInfo.model_template.subcategory2 = item.subcategory_2;
                    modelInfo.model_template.subcategory3 = item.subcategory_3;
                    modelInfo.model_template.no_of_port = item.no_of_port;
                }
            }
            modelInfo.model_template.unit_input_type = "port";
            return modelInfo.model_template;

        }

        public JsonResult GetEquipmentSpecificaitons(string typeKey)
        {
            ISPModelInfo modelInfo = new ISPModelInfo();
            modelInfo.modeltypevalue = typeKey;
            modelInfo.model_template = InitModelTemplate(modelInfo);
            var result = modelInfo.model_template;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModelMasterData()
        {
            var lstModel = BLISPModelInfo.Instance.GetModelMaster().ToList();
            return Json(lstModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewModelsImage(ViewIspModelImage objIspModelImage, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            objIspModelImage.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objIspModelImage.currentPage = page == 0 ? 1 : page;
            objIspModelImage.sort = sort;
            objIspModelImage.orderBy = sortdir;
            Session["ViewModelImage"] = objIspModelImage;
            objIspModelImage.listISPModelImage =  BLISPModelInfo.Instance.GetIspModelImage(objIspModelImage);
            objIspModelImage.totalRecord = objIspModelImage.listISPModelImage != null && objIspModelImage.listISPModelImage.Count > 0 ? objIspModelImage.listISPModelImage[0].totalRecords : 0;
            return View("_ViewModelImage", objIspModelImage);
        }
        public ActionResult UploadNewModleImage(int id)
        {
            ISPModelImageMaster objISPModelImageMaster = new ISPModelImageMaster();
            objISPModelImageMaster = id > 0 ?  BLISPModelInfo.Instance.GetModleImageById(id) : new ISPModelImageMaster();
            if (id == 0)
                objISPModelImageMaster.is_active = true;
            var PortList =  BLISPModelInfo.Instance.GetModelImage();
            objISPModelImageMaster.lstTypeModel = PortList.Where(m => m.is_model_image_allowed == true).OrderBy(m => m.key).ToList();
            return PartialView("_UploadModelImage", objISPModelImageMaster);
        }

        public ActionResult UploadModleImage(FormCollection collection)
        {
            JsonResponse<string> jResp = new JsonResponse<string>();
            try
            {
                var DefaultPortDimension = ApplicationSettings.DefaultPortDimension;
                var modelType = collection["modelType"];
                var id = collection["id"];
                var featureName = collection["feature_name"];
                var attachmentType = "Document";
                var is_active = false;
                var image_data = "";
                if (collection["is_active"] == "true")
                    is_active = true;
                if (Request.Files.Count > 0)
            {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        
                       
                            HttpPostedFileBase file = files[i];
                            string FileName = file.FileName;
                            string strNewfilename = Path.GetFileNameWithoutExtension(FileName) + "_" + MiscHelper.getTimeStamp() + Path.GetExtension(FileName);
                        //commented by priyanka
                        //var urlpath = Path.Combine(Server.MapPath("~/Uploads/Temp"), strNewfilename);
                        //commented by priyanka end

                            var urlpath = Path.Combine(Server.MapPath(Settings.ApplicationSettings.DownloadTempPath), strNewfilename);
                            file.SaveAs(urlpath);
                            SVGDocument document = new SVGDocument(urlpath);
                            var pathElement = document.QuerySelector("path").ParentElement;
                            var svgElement = document.RootElement;
                            var svg = document.QuerySelector("svg");
                            svg.SetAttribute("width", DefaultPortDimension + "Px");
                            svg.SetAttribute("height", DefaultPortDimension + "Px");
                            var path1 = document.QuerySelector("path");
                            path1.SetAttribute("fill", "[STATUS_COLOR]");
                            path1.SetAttribute("class", "[BLINK_CLASS] ");
                            if (pathElement.FirstChild.NextSibling != path1)
                                pathElement.RemoveChild(document.QuerySelector("path"));
                            pathElement.InsertBefore(path1, pathElement.FirstChild);
                            image_data = document.Children[0].OuterHTML;
                            image_data = image_data.Replace("svg:svg", "svg");

                        
                        // get User Detail..
                        User objUser = (User)(Session["userDetail"]);
                        ISPModelImageMaster objAttachment = new ISPModelImageMaster();
                        //objAttachment.id = Convert.ToInt32(id);
                        objAttachment.model_id = Convert.ToInt32(modelType) ;
                        objAttachment.image_data = image_data;
                        objAttachment.is_active = is_active;
                        objAttachment.created_by = objUser.user_id;
                        var savefile = BLISPModelInfo.Instance.SaveModleImage(objAttachment);
                    }
                    jResp.message = "Model Image has been uploaded successfully!";
                    jResp.status = StatusCodes.OK.ToString();
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                
            }
            else
            {
                    if (modelType != "" && Convert.ToInt32(id) != 0)
                    {
                        User objUser = (User)(Session["userDetail"]);
                        ISPModelImageMaster objAttachment = new ISPModelImageMaster();
                        objAttachment.id = Convert.ToInt32(id);
                        objAttachment.model_id = Convert.ToInt32(modelType);
                        objAttachment.image_data = image_data;
                        objAttachment.is_active = is_active;
                        objAttachment.created_by = objUser.user_id;
                        var savefile = BLISPModelInfo.Instance.SaveModleImage(objAttachment);
                        jResp.message = "Model Image has been updated successfully!";
                        jResp.status = StatusCodes.OK.ToString();
                    }
                    else { 
                jResp.message = "No files selected.";
                jResp.status = StatusCodes.INVALID_INPUTS.ToString();
               
                    }
                    return Json(jResp, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("UploadModleImage()", "Equipment", ex);
                jResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_243;
                jResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                return Json(jResp, JsonRequestBehavior.AllowGet);
                //Error Logging...
            }
        }
        public JsonResult DeleteISPModelImageById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();


            var result = BLISPModelInfo.Instance.DeleteModleImageById(id);
            if (result == "DELETE")
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = "Model Image has been deleted successfully!";
            }
           else if (result == "USED")
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "Model Image already used!";
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = result;
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
    }
}