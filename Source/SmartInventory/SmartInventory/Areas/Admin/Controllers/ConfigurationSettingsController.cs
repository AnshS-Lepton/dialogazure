using BusinessLogics;
using BusinessLogics.Admin;
using BusinessLogics.Feasibility;
using Models;
using Models.Admin;
using SmartInventory.Filters;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class ConfigurationSettingsController : Controller
    {
        #region Configruation Setting
        DataTable dt = new DataTable();
        public ActionResult AddConfigurationSetting()
        {
            ConfigurationSetting obj = new ConfigurationSetting();
            GetAllEntityList(obj);
            return View("AddConfigurationSetting", obj);
        }

        public IList<KeyValueDropDown> GetAllEntityList(ConfigurationSetting obj)
        {
            return obj.lstBindIspEntityType = new BLVendorSpecification().GetAllEntityList().OrderBy(m => m.key).ToList();

        }


        #region Equipment Details

        public JsonResult BindEqiupmenttypeOnChange(int entity_id = 0)
        {
            var objResp = new BLConfigurationSetting().getEquipmentTypeDetailByIdList(entity_id).OrderBy(m => m.type);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public PartialViewResult AddEquipmentTypeDetail(EquipmentTypeMaster objEquipmentTypeDetail)
        {
            if (objEquipmentTypeDetail.id != 0)
            {

                EquipmentTypeMaster objEquipmentType = new BLConfigurationSetting().getEquipmentTypeDetailById(objEquipmentTypeDetail.id);
                return PartialView("_AddEquipmentType", objEquipmentType);
            }

            return PartialView("_AddEquipmentType", objEquipmentTypeDetail);
        }

        [HttpPost]
        public ActionResult SaveEquipmentTypeCode(EquipmentTypeMaster objEquipmentDetail)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            int user_id = Convert.ToInt32(Session["user_id"]);

            var isNew = objEquipmentDetail.id > 0 ? false : true;

            if (isNew)
            {

                int IsEquipmentUnique = new BLConfigurationSetting().IsValueExistsForConfigSettings("equipment", objEquipmentDetail.type, objEquipmentDetail.entity_id);

                if (IsEquipmentUnique > 0)
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "this Equipment name alredy exists";

                    objEquipmentDetail.pageMsg = objMsg;

                    return PartialView("_AddEquipmentType", objEquipmentDetail);
                }
            }

            objEquipmentDetail = new BLConfigurationSetting().SaveEquipmentTypeDetails(objEquipmentDetail, user_id);

            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Equipment Type Detail Saved successfully!";
            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Equipment Type Detail Updated successfully!";
            }

            objEquipmentDetail.pageMsg = objMsg;

            return PartialView("_AddEquipmentType", objEquipmentDetail);

        }


        [HttpPost]
        public JsonResult DeleteEquipmentTypeDetailById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {

                var IsEquipmentTypeUsing_inbrand = new BLConfigurationSetting().getBrandTypeDetailById(0, id);


                if (IsEquipmentTypeUsing_inbrand != null)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Cannot be delete because it is using with brand type:- " + IsEquipmentTypeUsing_inbrand.brand + " ";
                }

                else
                {

                    var output = new BLConfigurationSetting().DeleteEquipmentTypeDetailById(id);
                    if (output > 0)
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                        objResp.message = "Equipment Type Detail Deleted successfully!";

                    }
                    else
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = "Something went wrong while deleting Equipment!";
                    }
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Equipment Type Detail not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region Brand Details

        public JsonResult BindBrandTypeOnChange(int equipmentType_id = 0)
        {
            var objResp = new BLConfigurationSetting().getBrandTypeDetailByIdList(equipmentType_id).OrderBy(m => m.brand);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public PartialViewResult AddBrandTypeDetail(BrandTypeMaster objBrandTypeDetail)
        {
            if (objBrandTypeDetail.id != 0)
            {

                BrandTypeMaster objBrandType = new BLConfigurationSetting().getBrandTypeDetailById(objBrandTypeDetail.id);
                return PartialView("_AddBrandType", objBrandType);
            }

            return PartialView("_AddBrandType", objBrandTypeDetail);
        }


        [HttpPost]
        public ActionResult SaveBrandTypeDetail(BrandTypeMaster objBrandTypeDetail)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            int user_id = Convert.ToInt32(Session["user_id"]);

            var isNew = objBrandTypeDetail.id > 0 ? false : true;


            if (isNew)
            {

                int IsBrandUnique = new BLConfigurationSetting().IsValueExistsForConfigSettings("brand", objBrandTypeDetail.brand, objBrandTypeDetail.type_id);

                if (IsBrandUnique > 0)
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "this Brand name alredy exists";

                    objBrandTypeDetail.pageMsg = objMsg;

                    return PartialView("_AddBrandType", objBrandTypeDetail);
                }
            }


            objBrandTypeDetail = new BLConfigurationSetting().SaveBrandTypeDetails(objBrandTypeDetail, user_id);

            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Brand Type Detail Saved successfully!";
            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Brand Type Detail Updated successfully!";
            }

            objBrandTypeDetail.pageMsg = objMsg;

            return PartialView("_AddBrandType", objBrandTypeDetail);

        }


        [HttpPost]
        public JsonResult DeleteBrandTypeDetailById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {

                var IsBrandTypeUsing_inmodel = new BLConfigurationSetting().getModelTypeDetailById(0, id);


                if (IsBrandTypeUsing_inmodel != null)
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Cannot be delete because it is using with model type:- " + IsBrandTypeUsing_inmodel.model + " ";
                }

                else
                {

                    var output = new BLConfigurationSetting().DeleteBrandTypeDetailById(id);
                    if (output > 0)
                    {
                        objResp.status = ResponseStatus.OK.ToString();
                        objResp.message = "Brand Type Detail Deleted successfully!";
                    }
                    else
                    {
                        objResp.status = ResponseStatus.FAILED.ToString();
                        objResp.message = "Something went wrong while deleting Brand Type!";
                    }
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Brand Type Detail not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region Model Details

        public JsonResult BindModelTypeOnChange(int BrandType_id = 0)
        {
            var objResp = new BLConfigurationSetting().getModelTypeDetailByIdList(BrandType_id).OrderBy(m => m.model);

            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public PartialViewResult AddModelTypeDetail(ModelTypePortMaster objModelTypeDetail)
        {
            if (objModelTypeDetail.modelTypeMaster.id != 0)
            {
                objModelTypeDetail.modelTypeMaster = new BLConfigurationSetting().getModelTypeDetailById(objModelTypeDetail.modelTypeMaster.id, 0);
                objModelTypeDetail.lstPortInfo = new BLConfigurationSetting().GetModelTypePortInforDetails(objModelTypeDetail.modelTypeMaster.id).Where(m => m.input_output.ToString().ToLower() == "i".ToString()).ToList();
                objModelTypeDetail.lstOutputPortInfo = new BLConfigurationSetting().GetModelTypePortInforDetails(objModelTypeDetail.modelTypeMaster.id).Where(m => m.input_output.ToString().ToLower() == "o".ToString()).ToList();

                return PartialView("_AddModelType", objModelTypeDetail);
            }

            return PartialView("_AddModelType", objModelTypeDetail);
        }

        [HttpPost]
        public ActionResult SaveModelTypeDetail(ModelTypePortMaster objModelTypeDetail)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            int user_id = Convert.ToInt32(Session["user_id"]);

            var isNew = objModelTypeDetail.modelTypeMaster.id > 0 ? false : true;


            if (isNew)
            {

                int IsModelUnique = new BLConfigurationSetting().IsValueExistsForConfigSettings("model", objModelTypeDetail.modelTypeMaster.model, objModelTypeDetail.modelTypeMaster.brand_id);

                if (IsModelUnique > 0)
                {
                    objMsg.status = ResponseStatus.FAILED.ToString();
                    objMsg.message = "this Model name alredy exists";

                    objModelTypeDetail.modelTypeMaster.pageMsg = objMsg;

                    return PartialView("_AddModelType", objModelTypeDetail);
                }
            }




            objModelTypeDetail.modelTypeMaster = new BLConfigurationSetting().SaveModelTypeDetails(objModelTypeDetail.modelTypeMaster, user_id);

            //int inputsequence = 1;
            for (int n = 0; n < objModelTypeDetail.lstPortInfo.Count; n++)
            {
                //inputsequence = inputsequence + 1;
                //PortInfo objInputPort = new PortInfo();
                //objInputPort.model_id = objModelTypeDetail.modelTypeMaster.id;
                //objInputPort.input_output = "I";
                ////objInputPort.port_no = inputsequence;
                //objInputPort.port_type = objModelTypeDetail.lstPortInfo[n].port_type;
                //objInputPort.port_name = objModelTypeDetail.lstPortInfo[n].port_name;
                //objInputPort.port_no = objModelTypeDetail.lstPortInfo[n].port_no;
                //objInputPort.id = objModelTypeDetail.lstPortInfo[n].id;
                objModelTypeDetail.lstPortInfo[n].input_output = "I";
                objModelTypeDetail.lstPortInfo[n].model_id = objModelTypeDetail.modelTypeMaster.id;


                new BLConfigurationSetting().SavePortInfoDetails(objModelTypeDetail.lstPortInfo[n], user_id);


            }

            for (int n = 0; n < objModelTypeDetail.lstOutputPortInfo.Count; n++)
            {

                //PortInfo objOutputPortNew = new PortInfo();
                //objOutputPortNew.model_id = objModelTypeDetail.modelTypeMaster.id;
                //objOutputPortNew.input_output = "O";
                //objOutputPortNew.port_type = objModelTypeDetail.lstOutputPortInfo[n].port_type;
                //objOutputPortNew.port_name = objModelTypeDetail.lstOutputPortInfo[n].port_name;
                //objOutputPortNew.port_no = objModelTypeDetail.lstOutputPortInfo[n].port_no;
                //objOutputPortNew.id = objModelTypeDetail.lstOutputPortInfo[n].id;
                objModelTypeDetail.lstOutputPortInfo[n].input_output = "O";
                objModelTypeDetail.lstOutputPortInfo[n].model_id = objModelTypeDetail.modelTypeMaster.id;

                new BLConfigurationSetting().SavePortInfoDetails(objModelTypeDetail.lstOutputPortInfo[n], user_id);
            }



            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = "Model Type Detail Saved successfully!";
            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Model Type Detail Updated successfully!";
            }

            objModelTypeDetail.modelTypeMaster.pageMsg = objMsg;

            return PartialView("_AddModelType", objModelTypeDetail);

        }





        [HttpPost]
        public JsonResult DeleteModelTypeDetailById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {

                var output = new BLConfigurationSetting().DeleteModelTypeDetailById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Model Type Detail Deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting Model Type!";
                }
            }

            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Model Type Detail not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteInputOutputPortById(int id)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {

                var output = new BLConfigurationSetting().DeleteInputOutputPortById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting!";
                }
            }

            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }



        #endregion


        #region Configuration Changes


        private static DataTable CreateTable(string tableName)
        {
            // Create a test DataTable with two columns and a few rows.
            DataTable table = new DataTable();

            DataColumn column = new DataColumn("key_type", typeof(System.String));
            table.Columns.Add(column);

            column = new DataColumn("key_value", typeof(System.String));
            table.Columns.Add(column);

            column = new DataColumn("section", typeof(System.String));
            table.Columns.Add(column);

            column = new DataColumn("description", typeof(System.String));
            table.Columns.Add(column);

            table.AcceptChanges();
            return table;
        }
        public ActionResult Index(ChangeConfigurationSetting objCS, int page = 0, string sort = "", string sortdir = "")
        {
            dt = CreateTable("WebConfigTable");

            string URLString = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~").FilePath;

            XmlDocument xDoc = new XmlDocument();

            //load up the xml from the location 
            xDoc.Load(URLString);

            foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
            {
                // first node is the url ... have to go to nexted loc node 
                foreach (XmlNode locNode in node)
                {
                    // thereare a couple child nodes here so only take data from node named loc 
                    if (locNode.Name == "sessionState")
                    {
                        // get the content of the loc node 

                        DataRow row;
                        row = dt.NewRow();
                        row["key_type"] = locNode.Name.ToString();
                        row["key_value"] = locNode.Attributes.GetNamedItem("timeout").Value;
                        row["section"] = locNode.ParentNode.Name.ToString();
                        row["description"] = "Session Time Out value in minutes";
                        dt.Rows.Add(row);
                    }
                }
            }

            Session["sessionStateDt"] = dt;
            var objLgnUsrDtl = (User)Session["userDetail"];

            objCS.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objCS.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objCS.objGridAttributes.sort = sort;
            objCS.objGridAttributes.orderBy = sortdir;
            objCS.lstSearchBy = GetRoleSearchByColumns();
            if (objCS.objGridAttributes.searchBy != null && objCS.objGridAttributes.searchText != null)
            {
                var resultRows = dt.AsEnumerable().Where(row => row.Field<string>(objCS.objGridAttributes.searchBy).Contains(objCS.objGridAttributes.searchText)).ToArray();
                if (resultRows.Length > 0)
                {
                    DataTable resultDataTable = resultRows.CopyToDataTable();
                    objCS.listCCS = CommonHelpers.ConvertDataTableToList<ChangeConfigurationSetting>(resultDataTable);
                }
            }
            else
            {
                objCS.listCCS = CommonHelpers.ConvertDataTableToList<ChangeConfigurationSetting>(dt);
            }
            objCS.totalRecord = objCS.listCCS != null && objCS.listCCS.Count > 0 ? objCS.listCCS.Count : 0;

            return View("_ChangeConfigSetting", objCS);
        }
        public List<KeyValueDropDown> GetRoleSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Section", value = "section" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Key", value = "key_type" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }
        public ActionResult EditConfigurationSettings(int id)
        {
            ChangeConfigurationSetting objCS = new ChangeConfigurationSetting();
            dt = Session["sessionStateDt"] as DataTable;
            objCS.key_type = dt.Rows[0]["key_type"].ToString();
            objCS.key_value = dt.Rows[0]["key_value"].ToString();
            objCS.section = dt.Rows[0]["section"].ToString();
            objCS.description= dt.Rows[0]["description"].ToString();
            return PartialView("_EditConfigSetting", objCS);
        }
        #endregion
        [HttpPost]
        public JsonResult SaveConfigSetting(ChangeConfigurationSetting obj)
        {
            ModelState.Clear();
            var user_id = Convert.ToInt32(Session["user_id"]);
            PageMessage objMsg = new PageMessage();


            obj.created_by = user_id;
            obj.section = Request.Form["section"];
            obj.key_type = Request.Form["key_type"];

            //Save data in database 
            var status = new BLConfigurationSetting().SaveConfigSetting(obj, user_id);
            if (status)
            {
                Configuration objConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                SystemWebSectionGroup syswebSection = (SystemWebSectionGroup)objConfig.GetSectionGroup("system.web");

                //you can change the value as you want
                syswebSection.SessionState.Timeout = new TimeSpan(0, Convert.ToInt32(obj.key_value), 0);
                objConfig.Save();

                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = "Session timeout updated successfully!";
            }

            obj.pageMsg = objMsg;
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}