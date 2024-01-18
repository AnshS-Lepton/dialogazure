using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Models;
using BusinessLogics;
using System.Text;
using SmartInventory.Settings;
using Lepton.Entities;


namespace SmartInventory.Areas.Admin.Controllers
{
    public class DynamicFormController : Controller
    {
        private readonly BLDynamicsForm bLDynamics;
        private readonly BLLayer blLayer;

        public DynamicFormController()
        {
            bLDynamics = new BLDynamicsForm();
            blLayer = new BLLayer();
        }
        // GET: Admin/DynamicForm
        public ActionResult Index()
        {
            AdditionalAttributeModel additionalAttributeModel = new AdditionalAttributeModel();
            additionalAttributeModel.layers = blLayer.GetAdditionalAttributesLayers();
            return View(additionalAttributeModel);
        }
        [HttpPost]
        public JsonResult Save(DynamicControls dynamicControls)
        {
            ApiResponse<DynamicControls> apiError = new ApiResponse<DynamicControls>();

            try
            {
                User objUser = (User)(Session["userDetail"]);
                var layerDetails = blLayer.GetLayerDetailsbyID(dynamicControls.entity_id);
                dynamicControls.created_by = objUser.user_id;
                dynamicControls.created_on = DateTime.Now;
                dynamicControls.is_visible = true;
                dynamicControls.field_name = dynamicControls.field_label.ToLower().Trim().Replace(" ", "_");
                dynamicControls.field_label = dynamicControls.field_label.Trim();
                bLDynamics.Save(dynamicControls);
                apiError.status = StatusCodes.OK.ToString();
                apiError.error_message = "Field Saved Successfully";
                apiError.results = dynamicControls;
                bLDynamics.SaveSampleJson(dynamicControls.entity_id, dynamicControls.field_name, dynamicControls.id);
                bLDynamics.CreateDynamicView(layerDetails.layer_name);
                bLDynamics.SyncViewColumns();
            }
            catch (Exception ex)
            {
                apiError.status = StatusCodes.FAILED.ToString();
                apiError.error_message = "Failed! " + ex.Message;

            }
            return Json(apiError, JsonRequestBehavior.AllowGet);
        }
        //public bool Duplicate(DynamicControls dynamicControls)
        //{
        //    dynamicControls.created_on = DateTime.Now;
        //    dynamicControls.is_visible = true;
        //    dynamicControls.field_name = dynamicControls.field_label.ToLower().Trim().Replace(" ", "_");
        //    return bLDynamics.Duplicate(dynamicControls);

        //}
        [HttpPost]
        public JsonResult SaveForm(List<DynamicFormData> fields)
        {
            var response = UpdateFields(fields);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        private List<DynamicControls> UpdateFields(List<DynamicFormData> fields)
        {

            List<DynamicControls> dynamicControls = new List<DynamicControls>();
            foreach (DynamicFormData dynamicFormData in fields)
            {
                DynamicControls dc = bLDynamics.GetByID(dynamicFormData.ID);
                dc.id = dynamicFormData.ID;
                dc.field_label = dynamicFormData.Title;
                //dc.entity_id = dynamicFormData.EntityID;
                dc.field_name = dynamicFormData.Title.ToLower().Replace(" ", "_");
                //dc.control_css_class = "";
                dc.control_type = dc.control_type.ToUpper();
                dc.control_value_type = dynamicFormData.Typeof;
                dc.is_mandatory = dynamicFormData.IsRequired;
                dc.max_length = dynamicFormData.RangeMax;
                dc.min_length = dynamicFormData.RangeMin;
                dc.format = dynamicFormData.Format;
                dc.default_value = dynamicFormData.DefaultVal;
                dc.field_order = dynamicFormData.Pos;
                dc.placeholder_text = dynamicFormData.PlaceHolderText;
                //dc.control_type = dynamicFormData.RangeType;
                dc.control_css_class = dynamicFormData.ClassNames;

                bLDynamics.Update(dc);
            }

            return dynamicControls;
        }
        public JsonResult ExistingFields(int layerId)
        {
            AdditionalAttributeModel additionalAttributeModel = new AdditionalAttributeModel();
            BLDynamicAttributes bLDynamicAttributes = new BLDynamicAttributes();
            User objUser = (User)(Session["userDetail"]);
            additionalAttributeModel.dynamicControls = bLDynamics.GetExistingFields(layerId, objUser.user_id);
            foreach (var item in additionalAttributeModel.dynamicControls)
            {
                if (item.control_type == "DROPDOWN")
                {
                    item.dynamicControlsDDLMasters = new List<DynamicControlsDDLMaster>();
                    item.dynamicControlsDDLMasters.AddRange(bLDynamicAttributes.GetExisitingFieldsforDDL(item.id));
                }
            }
            return Json(additionalAttributeModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExistingControl(int controlID)
        {
            DynamicControls dynamicControls = new DynamicControls();
            BLDynamicAttributes bLDynamicAttributes = new BLDynamicAttributes();
            User objUser = (User)(Session["userDetail"]);
            dynamicControls = bLDynamics.GetExistingControl(controlID);
            if (dynamicControls != null && dynamicControls.control_type == "DROPDOWN")
            {
                dynamicControls.dynamicControlsDDLMasters = new List<DynamicControlsDDLMaster>();
                dynamicControls.dynamicControlsDDLMasters.AddRange(bLDynamicAttributes.GetExisitingFieldsforDDL(controlID));
            }
            return Json(dynamicControls, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Update(DynamicControls dynamicControls)
        {
            ApiResponse<DynamicControls> apiError = new ApiResponse<DynamicControls>();
            DynamicControls existingval = bLDynamics.GetExistingControl(Convert.ToInt32(dynamicControls.control_id));
            try
            {
                User objUser = (User)(Session["userDetail"]);
                var layerDetails = blLayer.GetLayerDetailsbyID(dynamicControls.entity_id);
                dynamicControls.created_by = objUser.user_id;
                dynamicControls.created_on = DateTime.Now;
                dynamicControls.is_visible = true;
                dynamicControls.field_name = dynamicControls.field_label.ToLower().Trim().Replace(" ", "_");
                dynamicControls.field_label = dynamicControls.field_label.Trim();
                dynamicControls.other_info = existingval.other_info;
                bLDynamics.Update(dynamicControls);
                apiError.status = StatusCodes.OK.ToString();
                apiError.error_message = "Field Updated Successfully";
                apiError.results = dynamicControls;
                var auditTable = layerDetails.audit_table_name;
                bLDynamics.UpdateSampleJson(existingval.field_name, dynamicControls.field_name, Convert.ToInt32(dynamicControls.control_id), dynamicControls.default_value, dynamicControls.entity_id, layerDetails.layer_table, auditTable);
                bLDynamics.CreateDynamicView(layerDetails.layer_name);
                bLDynamics.SyncViewColumns();
            }
            catch (Exception ex)
            {
                apiError.status = StatusCodes.FAILED.ToString();
                apiError.error_message = "Failed! " + ex.Message;

            }
            return Json(apiError, JsonRequestBehavior.AllowGet);
        }

        public void SaveDropDown(List<string> DropDownAdd, List<string> is_default, List<string> IsVisible, int id)
        {
            List<DynamicControlsDDLMaster> lstDynamicControlsDDLMaster = new List<DynamicControlsDDLMaster>();
            BLDynamicAttributes bLDynamicAttributes = new BLDynamicAttributes();
            int i = 0;
            foreach (string strValue in DropDownAdd)
            {
                DynamicControlsDDLMaster dynamicControlsDDLMaster = new DynamicControlsDDLMaster();
                dynamicControlsDDLMaster.display_text = strValue;
                dynamicControlsDDLMaster.value_text = strValue;
                dynamicControlsDDLMaster.control_id = id;
                dynamicControlsDDLMaster.is_default = Convert.ToBoolean(is_default[i]);
                dynamicControlsDDLMaster.isvisible = Convert.ToBoolean(IsVisible[i]);
                if (dynamicControlsDDLMaster.display_text != "")
                    lstDynamicControlsDDLMaster.Add(dynamicControlsDDLMaster);

                i++;
            }
            bLDynamicAttributes.Save(lstDynamicControlsDDLMaster);
        }
        public void UpdateDropDown(List<string> DropDownAdd, List<string> is_default, List<string> IsVisible, int id)
        {
            BLDynamicAttributes bLDynamicAttributes = new BLDynamicAttributes();
            var item = bLDynamicAttributes.GetExisitingFieldsforDDL(id);
            if (item.Count() > 0)
            {
                bLDynamicAttributes.DeleteExistingDDL(id);
            }
            List<DynamicControlsDDLMaster> lstDynamicControlsDDLMaster = new List<DynamicControlsDDLMaster>();
            int i = 0;
            foreach (string strValue in DropDownAdd)
            {
                DynamicControlsDDLMaster dynamicControlsDDLMaster = new DynamicControlsDDLMaster();
                dynamicControlsDDLMaster.display_text = strValue;
                dynamicControlsDDLMaster.value_text = strValue;
                dynamicControlsDDLMaster.control_id = id;
                dynamicControlsDDLMaster.is_default = Convert.ToBoolean(is_default[i]);
                dynamicControlsDDLMaster.isvisible = Convert.ToBoolean(IsVisible[i]);
                if (dynamicControlsDDLMaster.display_text != "")
                    lstDynamicControlsDDLMaster.Add(dynamicControlsDDLMaster);

                i++;
            }
            bLDynamicAttributes.Save(lstDynamicControlsDDLMaster);
        }
        public JsonResult DeleteSingleField(string control_id, string entityId, string entityName, string fieldLabel)
        {
            ErrorMessage err = new ErrorMessage();
            try
            {
                int entityid = Convert.ToInt32(entityId);
                var layerDetails = blLayer.GetLayerDetailsbyID(entityid);
                var layerTable = layerDetails.layer_table;
                fieldLabel = fieldLabel.ToLower().Trim().Replace(" ", "_");
                bLDynamics.DeleteSingleField(control_id, layerTable, fieldLabel, entityid);
                bLDynamics.CreateDynamicView(layerDetails.layer_name);
                bLDynamics.SyncViewColumns();
                err.is_valid = true;
                err.status = StatusCodes.OK.ToString();
                err.error_msg = "Field Deleted Successfully.";
            }
            catch (Exception ex)
            {
                err.status = StatusCodes.FAILED.ToString();
                err.error_msg = "Failed! " + ex.Message;
            }
            return Json(err, JsonRequestBehavior.AllowGet);
        }
    }
}