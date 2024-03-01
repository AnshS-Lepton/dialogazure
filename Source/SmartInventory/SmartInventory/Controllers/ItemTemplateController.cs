using BusinessLogics;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartInventory.Filters;
using BusinessLogics.Admin;
using BusinessLogics.ISP;
using Utility;
using SmartInventory.Settings;
using Lepton.Utility;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class ItemTemplateController : Controller
    {
        //
        // GET: /ItemTemplate/
        public ActionResult Index()
        {
            return View();
        }

        #region ADB
        public PartialViewResult ADBTemplate(string eType)
        {

            //ADBItemMaster objADBItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ADBItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.ADB);
            //BLItemTemplate.Instance.BindItemDropdowns(objADBItemMaster, EntityType.ADB.ToString());
            //new MiscHelper().BindPortDetails(objADBItemMaster, EntityType.ADB.ToString(), DropDownType.Adb_Port_Ratio.ToString());
            //var objDDL = new BLMisc().GetDropDownList(EntityType.ADB.ToString(), DropDownType.Entity_Type.ToString());
            //objADBItemMaster.lstEntityType = objDDL;
            //return PartialView("_ADBTemplate", objADBItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ADBItemMaster>(url, obj, EntityType.ADB.ToString(), EntityAction.Get.ToString());
            return PartialView("_ADBTemplate", response.results);
        }
        public ActionResult SaveADBTemplate(ADBItemMaster objADBItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objADBItem.id;
            //    if (objADBItem.unitValue != null && objADBItem.unitValue.Contains(":"))
            //    {
            //        objADBItem.no_of_input_port = Convert.ToInt32(objADBItem.unitValue.Split(':')[0]);
            //        objADBItem.no_of_output_port = Convert.ToInt32(objADBItem.unitValue.Split(':')[1]);
            //    }
            //    var resultItem = new BLADBItemMaster().SaveADBItemTemplate(objADBItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_ADB_NET_FRM_008;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_ADB_NET_FRM_009;
            //        objADBItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objADBItem, EntityType.ADB.ToString());
            //new MiscHelper().BindPortDetails(objADBItem, EntityType.ADB.ToString(), DropDownType.Adb_Port_Ratio.ToString());
            //objADBItem.objPM = objPM;
            //return PartialView("_ADBTemplate", objADBItem);

            objADBItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ADBItemMaster>(url, objADBItem, EntityType.ADB.ToString(), EntityAction.Save.ToString());
            return PartialView("_ADBTemplate", response.results);
        }

        #endregion
        public ActionResult GetVendorList(string specification)
        {
            JsonResponse<List<KeyValueDropDown>> objResp = new JsonResponse<List<KeyValueDropDown>>();

            List<KeyValueDropDown> lst = BLItemTemplate.Instance.GetVendorList(specification);
            if (lst.Count > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.result = lst;
            }
            else
            {
                objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCatSubcatData(string entitytype, string specification, int vendorId)
        {
            JsonResponse<List<itemCategory>> objResp = new JsonResponse<List<itemCategory>>();

            List<itemCategory> lst = BLItemTemplate.Instance.GetCatSubCatData(entitytype, specification, vendorId);
            if (lst.Count > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.result = lst;
            }
            else
            {
                objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAccessoriesSpecification(int accessoriesId)
        {
            JsonResponse<List<KeyValueDropDown>> objResp = new JsonResponse<List<KeyValueDropDown>>();

            List<KeyValueDropDown> lst = BLItemTemplate.Instance.GetAccessoriesSpecification(accessoriesId);
            if (lst.Count > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.result = lst;
            }
            else
            {
                objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBrand(Int32 typeId)
        {
            JsonResponse<List<KeyValueDropDown>> objResp = new JsonResponse<List<KeyValueDropDown>>();

            List<KeyValueDropDown> lst = BLItemTemplate.Instance.GetBrandData(typeId);
            if (lst.Count > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.result = lst;
            }
            else
            {
                objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetModel(Int32 brandId)
        {
            JsonResponse<List<KeyValueDropDown>> objResp = new JsonResponse<List<KeyValueDropDown>>();

            List<KeyValueDropDown> lst = BLItemTemplate.Instance.GetModelData(brandId);
            if (lst.Count > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.result = lst;
            }
            else
            {
                objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #region CDB
        public PartialViewResult CDBTemplate(string eType)
        {
            //CDBItemMaster objCDBItemMaster = BLItemTemplate.Instance.GetTemplateDetail<CDBItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.CDB);
            //BLItemTemplate.Instance.BindItemDropdowns(objCDBItemMaster, EntityType.CDB.ToString());
            //new MiscHelper().BindPortDetails(objCDBItemMaster, EntityType.CDB.ToString(), DropDownType.Cdb_Port_Ratio.ToString());
            //var objDDL = new BLMisc().GetDropDownList(EntityType.CDB.ToString(), DropDownType.Entity_Type.ToString());
            //objCDBItemMaster.lstEntityType = objDDL;
            //return PartialView("_CDBTemplate", objCDBItemMaster);

            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CDBItemMaster>(url, obj, EntityType.CDB.ToString(), EntityAction.Get.ToString());
            return PartialView("_CDBTemplate", response.results);
        }
        public ActionResult SaveCDBTemplate(CDBItemMaster objCDBItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objCDBItem.id;
            //    if (objCDBItem.unitValue != null && objCDBItem.unitValue.Contains(":"))
            //    {
            //        objCDBItem.no_of_input_port = Convert.ToInt32(objCDBItem.unitValue.Split(':')[0]);
            //        objCDBItem.no_of_output_port = Convert.ToInt32(objCDBItem.unitValue.Split(':')[1]);
            //    }
            //    var resultItem = new BLCDBItemMaster().SaveCDBItemTemplate(objCDBItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_018;// "CDB template update successfully.";
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_099;// "CDB template  saved successfully.";
            //        objCDBItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;// "Please fill mandatory field !";
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objCDBItem, EntityType.CDB.ToString());
            //new MiscHelper().BindPortDetails(objCDBItem, EntityType.CDB.ToString(), DropDownType.Cdb_Port_Ratio.ToString());
            //objCDBItem.objPM = objPM;
            //return PartialView("_CDBTemplate", objCDBItem);

            objCDBItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CDBItemMaster>(url, objCDBItem, EntityType.CDB.ToString(), EntityAction.Save.ToString());
            return PartialView("_CDBTemplate", response.results);
        }
        #endregion

        #region BDB
        public PartialViewResult BDBTemplate(string eType)
        {
            //BDBItemMaster objBDBItemMaster = BLItemTemplate.Instance.GetTemplateDetail<BDBItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.BDB);
            //BLItemTemplate.Instance.BindItemDropdowns(objBDBItemMaster, EntityType.BDB.ToString());
            //new MiscHelper().BindPortDetails(objBDBItemMaster, EntityType.BDB.ToString(), DropDownType.BDB_PORT_RATIO.ToString());
            //var objDDL = new BLMisc().GetDropDownList(EntityType.BDB.ToString(), DropDownType.Entity_Type.ToString());
            //objBDBItemMaster.lstEntityType = objDDL;
            //return PartialView("_BDBTemplate", objBDBItemMaster);

            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<BDBItemMaster>(url, obj, EntityType.BDB.ToString(), EntityAction.Get.ToString());
            return PartialView("_BDBTemplate", response.results);
        }
        public ActionResult SaveBDBTemplate(BDBItemMaster objBDBItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.BDB.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    var itemid = objBDBItem.id;
            //    if (objBDBItem.unitValue != null && objBDBItem.unitValue.Contains(":"))
            //    {
            //        objBDBItem.no_of_input_port = Convert.ToInt32(objBDBItem.unitValue.Split(':')[0]);
            //        objBDBItem.no_of_output_port = Convert.ToInt32(objBDBItem.unitValue.Split(':')[1]);
            //    }
            //    var resultItem = new BLBDBItemMaster().SaveBDBItemTemplate(objBDBItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//{0} template update successfully.
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//{0} template saved successfully.
            //        objBDBItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objBDBItem, EntityType.BDB.ToString());
            //new MiscHelper().BindPortDetails(objBDBItem, EntityType.BDB.ToString(), DropDownType.BDB_PORT_RATIO.ToString());
            //objBDBItem.objPM = objPM;
            //return PartialView("_BDBTemplate", objBDBItem);

            objBDBItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<BDBItemMaster>(url, objBDBItem, EntityType.BDB.ToString(), EntityAction.Save.ToString());
            return PartialView("_BDBTemplate", response.results);
        }
        #endregion

        #region Pole
        public PartialViewResult PoleTemplate(string eType)
        {

            //PoleItemMaster objPoleItemMaster = BLItemTemplate.Instance.GetTemplateDetail<PoleItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Pole);
            //BindpoleDropDown(objPoleItemMaster);
            //BLItemTemplate.Instance.BindItemDropdowns(objPoleItemMaster, EntityType.Pole.ToString());
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PoleItemMaster>(url, obj, EntityType.Pole.ToString(), EntityAction.Get.ToString());
            return PartialView("_PoleTemplate", response.results);
        }

        private void BindpoleDropDown(PoleItemMaster objPoleItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Pole.ToString());
            objPoleItemMaster.lstPoleType = objDDL.Where(x => x.dropdown_type == DropDownType.Pole_Type.ToString()).ToList();
        }
        public ActionResult SavePoleTemplate(PoleItemMaster objPoleItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objPoleItem.id;
            //    var resultItem = new BLPoleItemMaster().SavePoleItemTemplate(objPoleItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_POL_NET_FRM_008;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_POL_NET_FRM_009;
            //        objPoleItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BindpoleDropDown(objPoleItem);
            //BLItemTemplate.Instance.BindItemDropdowns(objPoleItem, EntityType.Pole.ToString());
            // objPoleItem.objPM = objPM;

            objPoleItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PoleItemMaster>(url, objPoleItem, EntityType.Pole.ToString(), EntityAction.Save.ToString());
            return PartialView("_PoleTemplate", response.results);
        }
        #endregion

        #region Pod
        public PartialViewResult PODTemplate(string eType)
        {
            //PODItemMaster objPODItemMaster = BLItemTemplate.Instance.GetTemplateDetail<PODItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.POD);
            //BLItemTemplate.Instance.BindItemDropdowns(objPODItemMaster, EntityType.POD.ToString());
            //BindPODDropdown(objPODItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PODItemMaster>(url, obj, EntityType.POD.ToString(), EntityAction.Get.ToString());
            return PartialView("_PODTemplate", response.results);
        }
        private void BindPODDropdown(PODItemMaster objPODItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.POD.ToString());
            objPODItemMaster.listPODType = objDDL.Where(x => x.dropdown_type == DropDownType.POD_Type.ToString()).ToList();
        }
        public ActionResult SavePODTemplate(PODItemMaster objPODItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.POD.ToString().ToUpper()).FirstOrDefault().layer_title;
            //    var itemid = objPODItem.id;
            //    var resultItem = new BLPODItemMaster().SavePODItemTemplate(objPODItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//{0} template update successfully.
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//{0} template saved successfully.
            //        objPODItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objPODItem, EntityType.POD.ToString());
            //BindPODDropdown(objPODItem);
            //objPODItem.objPM = objPM;
            //return PartialView("_PODTemplate", objPODItem);

            objPODItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PODItemMaster>(url, objPODItem, EntityType.POD.ToString(), EntityAction.Save.ToString());
            return PartialView("_PODTemplate", response.results);
        }

        #endregion

        #region Tree
        public PartialViewResult TreeTemplate(string eType)
        {

            //TreeItemMaster objTreeItemMaster = BLItemTemplate.Instance.GetTemplateDetail<TreeItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Tree);
            //BLItemTemplate.Instance.BindItemDropdowns(objTreeItemMaster, EntityType.Tree.ToString());
            //return PartialView("_TreeTemplate", objTreeItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TreeItemMaster>(url, obj, EntityType.Tree.ToString(), EntityAction.Get.ToString());
            return PartialView("_TreeTemplate", response.results);
        }

        public ActionResult SaveTreeTemplate(TreeItemMaster objTreeItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objTreeItem.id;
            //    var resultItem = new BLTreeItemMaster().SaveTreeItemTemplate(objTreeItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_TRE_NET_FRM_016;// "Tree template update successfully.";
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_TRE_NET_FRM_020;// "Tree template saved successfully.";
            //        objTreeItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objTreeItem, EntityType.Tree.ToString());

            //objTreeItem.objPM = objPM;
            //return PartialView("_TreeTemplate", objTreeItem);

            objTreeItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TreeItemMaster>(url, objTreeItem, EntityType.Tree.ToString(), EntityAction.Save.ToString());
            return PartialView("_TreeTemplate", response.results);
        }
        #endregion

        #region Manhole
        public PartialViewResult ManholeTemplate(string eType)
        {

            //ManholeItemMaster objManholeItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ManholeItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Manhole);
            //BLItemTemplate.Instance.BindItemDropdowns(objManholeItemMaster, EntityType.Manhole.ToString());
            //return PartialView("_ManholeTemplate", objManholeItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ManholeItemMaster>(url, obj, EntityType.Manhole.ToString(), EntityAction.Get.ToString());
            return PartialView("_ManholeTemplate", response.results);
        }

        public ActionResult SaveManholeTemplate(ManholeItemMaster objManholeItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objManholeItem.id;
            //    var resultItem = new BLManholeItemMaster().SaveManholeItemTemplate(objManholeItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_MH_NET_FRM_002;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_MH_NET_FRM_003;
            //        objManholeItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objManholeItem, EntityType.Manhole.ToString());

            //objManholeItem.objPM = objPM;
            //return PartialView("_ManholeTemplate", objManholeItem);

            objManholeItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ManholeItemMaster>(url, objManholeItem, EntityType.Manhole.ToString(), EntityAction.Save.ToString());
            return PartialView("_ManholeTemplate", response.results);
        }
        #endregion

        #region Coupler
        public PartialViewResult CouplerTemplate(string eType)
        {

            //CouplerItemMaster objCouplerItemMaster = BLItemTemplate.Instance.GetTemplateDetail<CouplerItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Coupler);
            //BLItemTemplate.Instance.BindItemDropdowns(objCouplerItemMaster, EntityType.Coupler.ToString());
            //return PartialView("_CouplerTemplate", objCouplerItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CouplerItemMaster>(url, obj, EntityType.Coupler.ToString(), EntityAction.Get.ToString());
            return PartialView("_CouplerTemplate", response.results);
        }

        public ActionResult SaveCouplerTemplate(CouplerItemMaster objCouplerItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objCouplerItem.id;
            //    var resultItem = new BLCouplerItemMaster().SaveCouplerItemTemplate(objCouplerItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_CPR_NET_FRM_003;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_CPR_NET_FRM_004;
            //        objCouplerItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objCouplerItem, EntityType.Coupler.ToString());

            //objCouplerItem.objPM = objPM;
            //return PartialView("_CouplerTemplate", objCouplerItem);
            objCouplerItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CouplerItemMaster>(url, objCouplerItem, EntityType.Coupler.ToString(), EntityAction.Save.ToString());
            return PartialView("_CouplerTemplate", response.results);
        }
        #endregion

        #region SpliceClosure
        public PartialViewResult SCTemplate(string eType)
        {

            //SCItemMaster objSCItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SCItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.SpliceClosure);
            //objSCItemMaster.no_of_port = objSCItemMaster.no_of_ports;
            //BLItemTemplate.Instance.BindItemDropdowns(objSCItemMaster, EntityType.SpliceClosure.ToString());
            //new MiscHelper().BindPortDetails(objSCItemMaster, EntityType.SpliceClosure.ToString(), DropDownType.SC_Port_Ratio.ToString());
            //return PartialView("_SCTemplate", objSCItemMaster);

            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SCItemMaster>(url, obj, EntityType.SpliceClosure.ToString(), EntityAction.Get.ToString());
            return PartialView("_SCTemplate", response.results);
        }

        public ActionResult SaveSCTemplate(SCItemMaster objSCItem)
        {
            //PageMessage objPM = new PageMessage();
            //objSCItem.no_of_ports = objSCItem.no_of_port;
            //if (ModelState.IsValid)
            //{
            //    if (objSCItem.unitValue != null && objSCItem.unitValue.Contains(":"))
            //    {
            //        objSCItem.no_of_input_port = Convert.ToInt32(objSCItem.unitValue.Split(':')[0]);
            //        objSCItem.no_of_output_port = Convert.ToInt32(objSCItem.unitValue.Split(':')[1]);
            //    }
            //    var itemid = objSCItem.id;
            //    var resultItem = new BLSCItemMaster().SaveSCItemTemplate(objSCItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_SC_NET_FRM_016;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_SC_NET_FRM_017;
            //        objSCItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objSCItem, EntityType.SpliceClosure.ToString());
            //new MiscHelper().BindPortDetails(objSCItem, EntityType.SpliceClosure.ToString(), DropDownType.SC_Port_Ratio.ToString());
            //objSCItem.objPM = objPM;
            //return PartialView("_SCTemplate", objSCItem);
            objSCItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SCItemMaster>(url, objSCItem, EntityType.SpliceClosure.ToString(), EntityAction.Save.ToString());
            return PartialView("_SCTemplate", response.results);
        }

        #endregion

        #region FMS
        public PartialViewResult FMSTemplate(string eType)
        {

            //FMSItemMaster objFMSItemMaster = BLItemTemplate.Instance.GetTemplateDetail<FMSItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.FMS);
            //BLItemTemplate.Instance.BindItemDropdowns(objFMSItemMaster, EntityType.FMS.ToString());
            //new MiscHelper().BindPortDetails(objFMSItemMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
            //return PartialView("_FMSTemplate", objFMSItemMaster);

            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FMSItemMaster>(url, obj, EntityType.FMS.ToString(), EntityAction.Get.ToString());
            return PartialView("_FMSTemplate", response.results);
        }

        public ActionResult SaveFMSTemplate(FMSItemMaster objFMSItem)
        {
            //PageMessage objPM = new PageMessage();
            //if (ModelState.IsValid)
            //{
            //    if (objFMSItem.unitValue != null && objFMSItem.unitValue.Contains(":"))
            //    {
            //        objFMSItem.no_of_input_port = Convert.ToInt32(objFMSItem.unitValue.Split(':')[0]);
            //        objFMSItem.no_of_output_port = Convert.ToInt32(objFMSItem.unitValue.Split(':')[1]);
            //    }
            //    var itemid = objFMSItem.id;
            //    var resultItem = new BLFMSItemMaster().SaveFMSItemTemplate(objFMSItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = "FMS template update successfully.";
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = "FMS template saved successfully.";
            //        objFMSItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = "Please fill mandatory field !";
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objFMSItem, EntityType.FMS.ToString());
            //new MiscHelper().BindPortDetails(objFMSItem, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
            //objFMSItem.objPM = objPM;
            //return PartialView("_FMSTemplate", objFMSItem);
            objFMSItem.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FMSItemMaster>(url, objFMSItem, EntityType.FMS.ToString(), EntityAction.Save.ToString());
            return PartialView("_FMSTemplate", response.results);
        }

        #endregion

        #region Splitter
        public PartialViewResult SplitterTemplate(string eType,string pEntityType)
        {
            //SplitterItemMaster objSplitterItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SplitterItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Splitter);
            //BindSplitterDropDown(objSplitterItemMaster);
            //BLItemTemplate.Instance.BindItemDropdowns(objSplitterItemMaster, EntityType.Splitter.ToString());
            //objSplitterItemMaster.unitValue = objSplitterItemMaster.splitter_ratio;
            //return PartialView("_SplitterTemplate", objSplitterItemMaster);
            SplitterItemMaster objSplitterItemMaster = new SplitterItemMaster();
            itemMaster objItemMaster = new itemMaster();
            objSplitterItemMaster.pEntityType = pEntityType;
            var obj = new { pEntityType = pEntityType,eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SplitterItemMaster>(url, obj,EntityType.Splitter.ToString(), EntityAction.Get.ToString());
            return PartialView("_SplitterTemplate", response.results);
        }

		public PartialViewResult MicroductTemplate(string eType)
		{
			//SplitterItemMaster objSplitterItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SplitterItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Splitter);
			//BindSplitterDropDown(objSplitterItemMaster);
			//BLItemTemplate.Instance.BindItemDropdowns(objSplitterItemMaster, EntityType.Splitter.ToString());
			//objSplitterItemMaster.unitValue = objSplitterItemMaster.splitter_ratio;
			//return PartialView("_SplitterTemplate", objSplitterItemMaster);

			var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
			string url = "api/ItemTemplate/EntityTemplate";
			var response = WebAPIRequest.PostIntegrationAPIRequest<MicroductItemMaster>(url, obj, EntityType.Microduct.ToString(), EntityAction.Get.ToString());
			return PartialView("_MicroductTemplate", response.results);
		}

		public ActionResult SaveMicroductTemplate(MicroductItemMaster objMicroductItemMaster)
		{

			objMicroductItemMaster.user_Id = Convert.ToInt32(Session["user_id"]);
			string url = "api/ItemTemplate/EntityTemplate";
			var response = WebAPIRequest.PostIntegrationAPIRequest<MicroductItemMaster>(url, objMicroductItemMaster, EntityType.Microduct.ToString(), EntityAction.Save.ToString());
			return PartialView("_MicroductTemplate", response.results);
		}

        public PartialViewResult TowerTemplate(string eType)
        {
            //SplitterItemMaster objSplitterItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SplitterItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Splitter);
            //BindSplitterDropDown(objSplitterItemMaster);
            //BLItemTemplate.Instance.BindItemDropdowns(objSplitterItemMaster, EntityType.Splitter.ToString());
            //objSplitterItemMaster.unitValue = objSplitterItemMaster.splitter_ratio;
            //return PartialView("_SplitterTemplate", objSplitterItemMaster);

            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TowerItemMaster>(url, obj, EntityType.Tower.ToString(), EntityAction.Get.ToString());
            return PartialView("~/Views/Tower/ItemTemplate/_TowerTemplate.cshtml", response.results);
        }

        public ActionResult SaveTowerTemplate(TowerItemMaster objTowerItemMaster)
        {

            objTowerItemMaster.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TowerItemMaster>(url, objTowerItemMaster, EntityType.Tower.ToString(), EntityAction.Save.ToString());
            return PartialView("~/Views/Tower/ItemTemplate/_TowerTemplate.cshtml", response.results);
        }

        public PartialViewResult SectorTemplate(string eType)
        {
            //SplitterItemMaster objSplitterItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SplitterItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Splitter);
            //BindSplitterDropDown(objSplitterItemMaster);
            //BLItemTemplate.Instance.BindItemDropdowns(objSplitterItemMaster, EntityType.Splitter.ToString());
            //objSplitterItemMaster.unitValue = objSplitterItemMaster.splitter_ratio;
            //return PartialView("_SplitterTemplate", objSplitterItemMaster);

            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SectorItemMaster>(url, obj, EntityType.Sector.ToString(), EntityAction.Get.ToString());
            return PartialView("~/Views/Sector/ItemTemplate/_SectorTemplate.cshtml", response.results);
        }

        public ActionResult SaveSectorTemplate(SectorItemMaster objSectorItemMaster)
        {

            objSectorItemMaster.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SectorItemMaster>(url, objSectorItemMaster, EntityType.Sector.ToString(), EntityAction.Save.ToString());
            return PartialView("~/Views/Sector/ItemTemplate/_SectorTemplate.cshtml", response.results);
        }

        public PartialViewResult AntennaTemplate(string eType)
        {
            //SplitterItemMaster objSplitterItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SplitterItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Splitter);
            //BindSplitterDropDown(objSplitterItemMaster);
            //BLItemTemplate.Instance.BindItemDropdowns(objSplitterItemMaster, EntityType.Splitter.ToString());
            //objSplitterItemMaster.unitValue = objSplitterItemMaster.splitter_ratio;
            //return PartialView("_SplitterTemplate", objSplitterItemMaster);

            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<AntennaItemMaster>(url, obj, EntityType.Antenna.ToString(), EntityAction.Get.ToString());
            return PartialView("~/Views/Antenna/ItemTemplate/_AntennaTemplate.cshtml", response.results);
        }

        public ActionResult SaveAntennaTemplate(AntennaItemMaster objAntennaItemMaster)
        {

            objAntennaItemMaster.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<AntennaItemMaster>(url, objAntennaItemMaster, EntityType.Antenna.ToString(), EntityAction.Save.ToString());
            return PartialView("~/Views/Antenna/ItemTemplate/_AntennaTemplate.cshtml", response.results);
        }



        public PartialViewResult MicrowaveLinkTemplate(string eType)
        {
            //SplitterItemMaster objSplitterItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SplitterItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Splitter);
            //BindSplitterDropDown(objSplitterItemMaster);
            //BLItemTemplate.Instance.BindItemDropdowns(objSplitterItemMaster, EntityType.Splitter.ToString());
            //objSplitterItemMaster.unitValue = objSplitterItemMaster.splitter_ratio;
            //return PartialView("_SplitterTemplate", objSplitterItemMaster);

            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<MicrowaveLinkItemMaster>(url, obj, EntityType.MicrowaveLink.ToString(), EntityAction.Get.ToString());
            return PartialView("~/Views/MicrowaveLink/ItemTemplate/_MicrowaveLinkTemplate.cshtml", response.results);
        }

        public ActionResult SaveMicrowaveLinkTemplate(MicrowaveLinkItemMaster objMicrowaveLinkItemMaster)
        {

            objMicrowaveLinkItemMaster.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<MicrowaveLinkItemMaster>(url, objMicrowaveLinkItemMaster, EntityType.MicrowaveLink.ToString(), EntityAction.Save.ToString());
            return PartialView("~/Views/MicrowaveLink/ItemTemplate/_MicrowaveLinkTemplate.cshtml", response.results);
        }



        public ActionResult SaveSplitterTemplate(SplitterItemMaster objSplitterItem)
        {
            //ModelState.Clear();
            //PageMessage objPM = new PageMessage();
            //objSplitterItem.splitter_ratio = objSplitterItem.unitValue;
            //if (ModelState.IsValid)
            //{
            //    var itemid = objSplitterItem.id;
            //    var resultItem = new BLSplitterItemMaster().SaveSplitterItemTemplate(objSplitterItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = "Splitter template update successfully.";
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = "Splitter template saved successfully.";
            //        objSplitterItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = getFirstErrorFromModelState(); //"Please fill mandatory field !";
            //}
            ////fill dropdown
            //BindSplitterDropDown(objSplitterItem);
            //BLItemTemplate.Instance.BindItemDropdowns(objSplitterItem, EntityType.Splitter.ToString());

            //objSplitterItem.objPM = objPM;
            //return PartialView("_SplitterTemplate", objSplitterItem);

            objSplitterItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<SplitterItemMaster>(url, objSplitterItem, EntityType.Splitter.ToString(), EntityAction.Save.ToString());
            return PartialView("_SplitterTemplate", response.results);
        }
        private void BindSplitterDropDown(SplitterItemMaster objSplitterItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Splitter.ToString());
            //objSplitterItemMaster.lstSplRatio = objDDL.Where(x => x.dropdown_type == DropDownType.Splitter_Ratio.ToString()).ToList();
            new BLMisc().BindPortDetails(objSplitterItemMaster, EntityType.Splitter.ToString(), DropDownType.Splitter_Ratio.ToString());
        }
        #endregion

        #region Mpod
        public PartialViewResult MPODTemplate(string eType)
        {
            //MPODItemMaster objMPODItemMaster = BLItemTemplate.Instance.GetTemplateDetail<MPODItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.MPOD);
            //BLItemTemplate.Instance.BindItemDropdowns(objMPODItemMaster, EntityType.MPOD.ToString());
            //BindMPODDropdown(objMPODItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<MPODItemMaster>(url, obj, EntityType.MPOD.ToString(), EntityAction.Get.ToString());
            return PartialView("_MPODTemplate", response.results);
        }
        private void BindMPODDropdown(MPODItemMaster objMPODItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.MPOD.ToString());
            objMPODItemMaster.listMPODType = objDDL.Where(x => x.dropdown_type == DropDownType.MPOD_Type.ToString()).ToList();
        }
        public ActionResult SaveMPODTemplate(MPODItemMaster objMPODItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objMPODItem.id;
            //    var resultItem = new BLMPODItemMaster().SaveMPODItemTemplate(objMPODItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_MPOD_NET_FRM_010;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_MPOD_NET_FRM_011;
            //        objMPODItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objMPODItem, EntityType.MPOD.ToString());
            //BindMPODDropdown(objMPODItem);
            //objMPODItem.objPM = objPM;
            //return PartialView("_MPODTemplate", objMPODItem);
            objMPODItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<MPODItemMaster>(url, objMPODItem, EntityType.MPOD.ToString(), EntityAction.Save.ToString());
            return PartialView("_MPODTemplate", response.results);
        }
        #endregion

        #region Cable
        public PartialViewResult CableTemplate(string cblType)
        {
            //    CableItemMaster objCableItemMaster = BLItemTemplate.Instance.GetTemplateDetail<CableItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Cable, cblType);
            //    BindCableDropDown(objCableItemMaster);
            //    objCableItemMaster.fiberCountIn = objCableItemMaster.total_core.ToString();
            //    BLItemTemplate.Instance.BindItemDropdowns(objCableItemMaster, EntityType.Cable.ToString());
            //    new MiscHelper().BindPortDetails(objCableItemMaster, EntityType.Cable.ToString(), DropDownType.Fiber_Count.ToString());
            //    objCableItemMaster.cable_type = (cblType == "ISP" ? "ISP" : objCableItemMaster.cable_type);
            //    objCableItemMaster.unitValue = Convert.ToString(objCableItemMaster.cable_type = (cblType == "ISP" ? "ISP" : objCableItemMaster.cable_type);
            //    objCableItemMaster.unitValue = Convert.ToString(objCableItemMaster.total_core);
            //    objCableItemMaster.formInputSettings = Settings.ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
            //    //  objCableItemMaster.total_core=
            //    return PartialView("_CableTemplate", objCableItemMaster);
            CableItemMaster obj = new CableItemMaster();
            obj.cblType = cblType;
            //obj.entityType = eType;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CableItemMaster>(url, obj, EntityType.Cable.ToString(), EntityAction.Get.ToString());
            return PartialView("_CableTemplate", response.results);
        }

        private void BindCableDropDown(CableItemMaster objCableItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Cable.ToString());
            objCableItemMaster.fiberCount = objDDL.Where(x => x.dropdown_type == DropDownType.Fiber_Count.ToString()).ToList();
            objCableItemMaster.listcableCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Category.ToString()).ToList();
            objCableItemMaster.listcableSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Subcategory.ToString()).ToList();
            objCableItemMaster.listcableType = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Type.ToString()).ToList();
        }

        public ActionResult SaveCableTemplate(CableItemMaster objCableItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objCableItem.id;
            //    if (!(string.IsNullOrEmpty(objCableItem.unitValue)))
            //    {
            //        objCableItem.total_core = Convert.ToInt32(objCableItem.unitValue);
            //    }
            //    var resultItem = new BLCableItemMaster().SaveCableItemTemplate(objCableItem, Convert.ToInt32(Session["user_id"]));
            //    string[] LayerName = { EntityType.Cable.ToString() };
            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();

            //        objPM.message = objCableItem.cable_type + " " + ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CAB_NET_FRM_063, ApplicationSettings.listLayerDetails, LayerName); //objCableItem.cable_type + " " + Resources.Resources.SI_OSP_CAB_NET_FRM_063;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = objCableItem.cable_type + " " + ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CAB_NET_FRM_064, ApplicationSettings.listLayerDetails, LayerName); //objCableItem.cable_type + " " + Resources.Resources.SI_OSP_CAB_NET_FRM_064;
            //        objCableItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_OSP_CAB_NET_FRM_065;
            //}
            ////fill dropdown
            //BindCableDropDown(objCableItem);
            //new MiscHelper().BindPortDetails(objCableItem, EntityType.Cable.ToString(), DropDownType.Fiber_Count.ToString());
            //BLItemTemplate.Instance.BindItemDropdowns(objCableItem, EntityType.Cable.ToString());

            //objCableItem.objPM = objPM;
            //objCableItem.formInputSettings = Settings.ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
            objCableItem.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CableItemMaster>(url, objCableItem, EntityType.Cable.ToString(), EntityAction.Save.ToString());
            return PartialView("_CableTemplate", response.results);
        }
        #endregion
        public PartialViewResult ONTTemplate(string eType)
        {
            //ONTItemMaster objONTItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ONTItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.ONT);
            //BLItemTemplate.Instance.BindItemDropdowns(objONTItemMaster, EntityType.ONT.ToString());
            //new MiscHelper().BindPortDetails(objONTItemMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
            //return PartialView("_ONTTemplate", objONTItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ONTItemMaster>(url, obj, EntityType.ONT.ToString(), EntityAction.Get.ToString());
            return PartialView("_ONTTemplate", response.results);
        }
        public ActionResult SaveONTTemplate(ONTItemMaster objONTItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objONTItem.id;
            //    if (objONTItem.unitValue != null && objONTItem.unitValue.Contains(":"))
            //    {
            //        objONTItem.no_of_input_port = Convert.ToInt32(objONTItem.unitValue.Split(':')[0]);
            //        objONTItem.no_of_output_port = Convert.ToInt32(objONTItem.unitValue.Split(':')[1]);
            //    }
            //    var resultItem = new BLONTItemMaster().SaveONTItemTemplate(objONTItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_ONT_NET_FRM_014;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_ONT_NET_FRM_015;
            //        objONTItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objONTItem, EntityType.ONT.ToString());
            //new MiscHelper().BindPortDetails(objONTItem, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
            //objONTItem.objPM = objPM;
            //return PartialView("_ONTTemplate", objONTItem);
            objONTItem.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ONTItemMaster>(url, objONTItem, EntityType.ONT.ToString(), EntityAction.Save.ToString());
            return PartialView("_ONTTemplate", response.results);
        }

        #region Trench Template

        public PartialViewResult TrenchTemplate(string eType)
        {

            //TrenchItemMaster objTrenchItemMaster = BLItemTemplate.Instance.GetTemplateDetail<TrenchItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Trench);
            //BindTrenchDropDown(objTrenchItemMaster);
            //BLItemTemplate.Instance.BindItemDropdowns(objTrenchItemMaster, EntityType.Trench.ToString());
            //return PartialView("_TrenchTemplate", objTrenchItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TrenchItemMaster>(url, obj, EntityType.Trench.ToString(), EntityAction.Get.ToString());
            return PartialView("_TrenchTemplate", response.results);
        }

        private void BindTrenchDropDown(TrenchItemMaster objTrenchItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Trench.ToString());
            objTrenchItemMaster.trenchTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Trench_Type.ToString()).ToList();
        }

        public ActionResult SaveTrenchTemplate(TrenchItemMaster objTrenchItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objTrenchItem.id;
            //    var resultItem = new BLTrenchItemMaster().SaveTrenchItemTemplate(objTrenchItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_TCH_NET_FRM_018;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_TCH_NET_FRM_019;
            //        objTrenchItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BindTrenchDropDown(objTrenchItem);
            //BLItemTemplate.Instance.BindItemDropdowns(objTrenchItem, EntityType.Trench.ToString());

            //objTrenchItem.objPM = objPM;
            //return PartialView("_TrenchTemplate", objTrenchItem);
            objTrenchItem.user_Id = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TrenchItemMaster>(url, objTrenchItem, EntityType.Trench.ToString(), EntityAction.Save.ToString());
            return PartialView("_TrenchTemplate", response.results);
        }

        #endregion

        #region Duct Template

        public PartialViewResult DuctTemplate(string eType)
        {

            //DuctItemMaster objDuctItemMaster = BLItemTemplate.Instance.GetTemplateDetail<DuctItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Duct);
            //BLItemTemplate.Instance.BindItemDropdowns(objDuctItemMaster, EntityType.Duct.ToString());
            //return PartialView("_DuctTemplate", objDuctItemMaster);
            var obj = new { eType = eType, user_Id = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<DuctItemMaster>(url, obj, EntityType.Duct.ToString(), EntityAction.Get.ToString());
            return PartialView("_DuctTemplate", response.results);
        }

        public ActionResult SaveDuctTemplate(DuctItemMaster objDuctItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objDuctItem.id;
            //    var resultItem = new BLDuctItemMaster().SaveDuctItemTemplate(objDuctItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
            //        objDuctItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objDuctItem, EntityType.Duct.ToString());

            //objDuctItem.objPM = objPM;
            //return PartialView("_DuctTemplate", objDuctItem);
            objDuctItem.user_Id = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<DuctItemMaster>(url, objDuctItem, EntityType.Duct.ToString(), EntityAction.Save.ToString());
            return PartialView("_DuctTemplate", response.results);
        }

        #endregion


        #region Conduit Template

        public PartialViewResult ConduitTemplate(string eType)
        {

            //DuctItemMaster objDuctItemMaster = BLItemTemplate.Instance.GetTemplateDetail<DuctItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Duct);
            //BLItemTemplate.Instance.BindItemDropdowns(objDuctItemMaster, EntityType.Duct.ToString());
            //return PartialView("_DuctTemplate", objDuctItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ConduitItemMaster>(url, obj, EntityType.Conduit.ToString(), EntityAction.Get.ToString());
            return PartialView("_ConduitTemplate", response.results);
        }

        public ActionResult SaveConduitTemplate(ConduitItemMaster objConduitItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objDuctItem.id;
            //    var resultItem = new BLDuctItemMaster().SaveDuctItemTemplate(objDuctItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
            //        objDuctItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objDuctItem, EntityType.Duct.ToString());

            //objDuctItem.objPM = objPM;
            //return PartialView("_DuctTemplate", objDuctItem);
            objConduitItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<ConduitItemMaster>(url, objConduitItem, EntityType.Conduit.ToString(), EntityAction.Save.ToString());
            return PartialView("_ConduitTemplate", response.results);
        }

        #endregion



        #region WallMount
        public PartialViewResult WallMountTemplate(string eType)
        {

            //WallMountItemMaster objWallMountItemMaster = BLItemTemplate.Instance.GetTemplateDetail<WallMountItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.WallMount);
            //BLItemTemplate.Instance.BindItemDropdowns(objWallMountItemMaster, EntityType.WallMount.ToString());
            //return PartialView("_WallMountTemplate", objWallMountItemMaster);
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<WallMountItemMaster>(url, obj, EntityType.WallMount.ToString(), EntityAction.Get.ToString());
            return PartialView("_WallMountTemplate", response.results);
        }

        public ActionResult SaveWallMountTemplate(WallMountItemMaster objWallMountItem)
        {
            //PageMessage objPM = new PageMessage();

            //if (ModelState.IsValid)
            //{
            //    var itemid = objWallMountItem.id;
            //    var resultItem = new BLWallMountItemMaster().SaveWallMountItemTemplate(objWallMountItem, Convert.ToInt32(Session["user_id"]));

            //    if (itemid > 0)  // Update 
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_WMT_NET_FRM_009;
            //    }
            //    else
            //    {
            //        objPM.status = ResponseStatus.OK.ToString();
            //        objPM.message = Resources.Resources.SI_OSP_WMT_NET_FRM_010;
            //        objWallMountItem = resultItem;
            //    }
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.FAILED.ToString();
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(objWallMountItem, EntityType.WallMount.ToString());

            //objWallMountItem.objPM = objPM;
            //return PartialView("_WallMountTemplate", objWallMountItem);
            objWallMountItem.UserId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<WallMountItemMaster>(url, objWallMountItem, EntityType.WallMount.ToString(), EntityAction.Save.ToString());
            return PartialView("_WallMountTemplate", response.results);
        }

        #endregion

        #region ProjectSpeicification

        public PartialViewResult AddProjectSpecification()
        {
            ProjectSpecificationMaster objProjSpeicification = new ProjectSpecificationMaster();

            //"P" we need to pass this value as dynamically as network stage selection

            objProjSpeicification.lstBindProjectCode = new BLProject().getProjectCodeDetails("P");

            return PartialView("_ProjectSpecificationMapping", objProjSpeicification);

        }


        public JsonResult GetPlanning(int ddlproject_id = 0)
        {
            var objResp = new BLProject().getPlanningDetailByIdList(ddlproject_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }


        public JsonResult GetWokorder(int ddlplanning_id = 0)
        {
            var objResp = new BLProject().getWorkorderDetailByIdList(ddlplanning_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public JsonResult GetPurpose(int ddlworkorder_id = 0)
        {
            var objResp = new BLProject().getPurposeDetailByIdList(ddlworkorder_id);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public ActionResult SaveProjectSpecificationMapping(ProjectSpecificationMaster objProjectSpeciMaster)
        {
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();

            int user_id = Convert.ToInt32(Session["user_id"]);

            var isNew = objProjectSpeciMaster.system_id > 0 ? false : true;
            //objProjectSpeciMaster = new BLItemTemplate(). BLProject().SaveEntityPlanningCodeDetails(objPlanningDetail, user_id);

            if (isNew)
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.isNewEntity = isNew;
                objMsg.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_102;// "ProjectSpeicification Detail Updated successfully!";
                //objPlanningDetail.planning_name = string.Empty;
                //objPlanningDetail.planning_code = string.Empty;
                //objPlanningDetail.system_id = 0;

            }
            else
            {
                objMsg.status = ResponseStatus.OK.ToString();
                objMsg.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_103;//"Planning Detail Updated successfully!";
            }

            objProjectSpeciMaster.pageMsg = objMsg;

            return PartialView("_ProjectSpecificationMapping", objProjectSpeciMaster);


        }


        #endregion ProjectSpeicification

        public PartialViewResult UnitTemplate()
        {

            var objRoomTemplate = BLISP.Instance.getRoomTemplate(Convert.ToInt32(((User)Session["userDetail"]).user_id));

            objRoomTemplate.area = objRoomTemplate.area == 0 ? 1 : objRoomTemplate.area;
            objRoomTemplate.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var objDDL = new BLMisc().GetDropDownList(EntityType.UNIT.ToString()).Where(x => x.dropdown_type == "Unit_Type").ToList();
            objRoomTemplate.lstUnitType = objDDL;

            return PartialView("_RoomTemplate", objRoomTemplate);
        }
        public ActionResult SaveUnitTemplate(RoomTemplate objUnitTemplate)
        {
            PageMessage objPM = new PageMessage();
            var unitTemplate = new RoomTemplate();


            int unitSystemId = objUnitTemplate.system_id;
            unitTemplate = new BLISP().UpdateUnitTemplate(objUnitTemplate);
            if (unitSystemId > 0)
            {

                objPM.status = ResponseStatus.OK.ToString(); ;
                objPM.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_104;// "Unit template updated successfully.";
            }
            else
            {
                objPM.status = ResponseStatus.OK.ToString(); ;
                objPM.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_105;// "Unit template saved successfully.";
                objUnitTemplate = unitTemplate;
            }
            objUnitTemplate.lstUnitType = new BLMisc().GetDropDownList(EntityType.UNIT.ToString(), "Unit_Type");
            objUnitTemplate.objPM = objPM;
            return PartialView("_RoomTemplate", objUnitTemplate);
        }

        public PartialViewResult HTBTemplate()
        {
            //var objHTBTemplate = BLISP.Instance.getHTBTemplate(Convert.ToInt32(((User)Session["userDetail"]).user_id));
            //HTBTemplate objHTBItemMaster = BLItemTemplate.Instance.GetTemplateDetail<HTBTemplate>(Convert.ToInt32(Session["user_id"]), EntityType.HTB);
            //objHTBItemMaster.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            //BLItemTemplate.Instance.BindItemDropdowns(objHTBItemMaster, EntityType.HTB.ToString());
            //new MiscHelper().BindPortDetails(objHTBItemMaster, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //return PartialView("_HTBTemplate", objHTBItemMaster);
            var obj = new { userId = (Convert.ToInt32(((User)Session["userDetail"]).user_id)) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HTBTemplate>(url, obj, EntityType.HTB.ToString(), EntityAction.Get.ToString());
            return PartialView("_HTBTemplate", response.results);
        }

        public ActionResult saveHTBTemplate(HTBTemplate objHTBTemplate)
        {
            //PageMessage objPM = new PageMessage();
            //var htbTemplate = new HTBTemplate();


            //int unitSystemId = objHTBTemplate.system_id;
            //if (objHTBTemplate.unitValue != null && objHTBTemplate.unitValue.Contains(":"))
            //{
            //    objHTBTemplate.no_of_input_port = Convert.ToInt32(objHTBTemplate.unitValue.Split(':')[0]);
            //    objHTBTemplate.no_of_output_port = Convert.ToInt32(objHTBTemplate.unitValue.Split(':')[1]);
            //}
            //htbTemplate = new BLISP().saveHTBTemplate(objHTBTemplate);
            //if (unitSystemId > 0)
            //{

            //    objPM.status = ResponseStatus.OK.ToString(); ;
            //    objPM.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_147;// "HTB template updated successfully.";
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.OK.ToString(); ;
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_019;// "HTB template saved successfully.";
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(htbTemplate, EntityType.HTB.ToString());
            //new MiscHelper().BindPortDetails(htbTemplate, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //htbTemplate.objPM = objPM;
            //return PartialView("_HTBTemplate", htbTemplate);
            objHTBTemplate.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HTBTemplate>(url, objHTBTemplate, EntityType.HTB.ToString(), EntityAction.Save.ToString());
            return PartialView("_HTBTemplate", response.results);
        }

        public PartialViewResult FDBTemplate()
        {
            //var objFDBTemplate = BLISP.Instance.getFDBTemplate(Convert.ToInt32(((User)Session["userDetail"]).user_id));
            //objFDBTemplate.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            //BLItemTemplate.Instance.BindItemDropdowns(objFDBTemplate, EntityType.FDB.ToString());
            //new MiscHelper().BindPortDetails(objFDBTemplate, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
            //return PartialView("_FDBTemplate", objFDBTemplate);
            var obj = new {userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FDBTemplate>(url, obj, EntityType.FDB.ToString(), EntityAction.Get.ToString());
            return PartialView("_FDBTemplate", response.results);
        }

        public ActionResult saveFDBTemplate(FDBTemplate objFDBTemplate)
        {
            //PageMessage objPM = new PageMessage();
            //var fdbTemplate = new FDBTemplate();


            //int unitSystemId = objFDBTemplate.system_id;
            //if (objFDBTemplate.unitValue != null && objFDBTemplate.unitValue.Contains(":"))
            //{
            //    objFDBTemplate.no_of_input_port = Convert.ToInt32(objFDBTemplate.unitValue.Split(':')[0]);
            //    objFDBTemplate.no_of_output_port = Convert.ToInt32(objFDBTemplate.unitValue.Split(':')[1]);
            //}
            //fdbTemplate = new BLISP().saveFDBTemplate(objFDBTemplate);

            //var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault().layer_title;
            //if (unitSystemId > 0)
            //{

            //    objPM.status = ResponseStatus.OK.ToString(); ;
            //    objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//{0} template updated successfully.
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.OK.ToString(); ;
            //    objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title); //{0} template saved successfully."
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(fdbTemplate, EntityType.FDB.ToString());
            //new MiscHelper().BindPortDetails(fdbTemplate, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
            //fdbTemplate.objPM = objPM;
            //return PartialView("_FDBTemplate", fdbTemplate);
            objFDBTemplate.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<FDBTemplate>(url, objFDBTemplate, EntityType.FDB.ToString(), EntityAction.Save.ToString());
            return PartialView("_FDBTemplate", response.results);
        }

        public PartialViewResult TestTemplate(string eType)
        {
            MPODItemMaster objMPODItemMaster = BLItemTemplate.Instance.GetTemplateDetail<MPODItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.MPOD);
            BLItemTemplate.Instance.BindItemDropdowns(objMPODItemMaster, EntityType.MPOD.ToString());
            return PartialView("_TestTemplate", objMPODItemMaster);
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

        #region ROW Template
        public PartialViewResult ROWTemplate(string eType)
        {
            ROWItemMaster objROWItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ROWItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.ROW, eType);

            var objDDL = new BLMisc().GetDropDownList(EntityType.ROW.ToString()).ToList();
            if (objDDL.Where(m => m.dropdown_key == objROWItemMaster.width.ToString()).FirstOrDefault() == null)
            {
                objROWItemMaster.isCustomWidthEnable = true;
                objROWItemMaster.customized = objROWItemMaster.width;
                objROWItemMaster.width = 0;
            }
            BindROWDropDown(objROWItemMaster);

            return PartialView("_ROWTemplate", objROWItemMaster);
        }

        private void BindROWDropDown(ROWItemMaster objROWItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.ROW.ToString());
            // objROWItemMaster.width = objDDL.Where(x => x.dropdown_type == DropDownType.ROW_Width.ToString()).ToList();
            objROWItemMaster.rowtypelist = objDDL.Where(x => x.dropdown_type == DropDownType.ROW_Type.ToString()).ToList();
            objROWItemMaster.rowwidthlist = objDDL.Where(x => x.dropdown_type == DropDownType.ROW_Width.ToString()).ToList();
        }

        public ActionResult SaveROWTemplate(ROWItemMaster objROWItem)
        {
            PageMessage objPM = new PageMessage();
            if (ModelState.IsValid)
            {
                var itemid = objROWItem.id;

                var resultItem = new BLROWItemMaster().SaveROWItemTemplate(objROWItem, Convert.ToInt32(Session["user_id"]));

                if (itemid > 0)  // Update 
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = Resources.Resources.SI_OSP_ROW_NET_FRM_104;
                }
                else
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = Resources.Resources.SI_OSP_ROW_NET_FRM_105;
                }
                objROWItem = resultItem;
            }

            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = getFirstErrorFromModelState(); //"Please fill ROW mandatory field !";
            }

            //fill dropdown
            BindROWDropDown(objROWItem);
            objROWItem.objPM = objPM;
            return PartialView("_ROWTemplate", objROWItem);
        }
        #endregion

        #region Patch Cord
        public PartialViewResult PatchCordTemplate(string PatchType)
        {
            PatchCordItemMaster objPatchItemMaster = BLItemTemplate.Instance.GetTemplateDetail<PatchCordItemMaster>(Convert.ToInt32(Session["user_id"]), EntityType.PatchCord, PatchType);
            BLItemTemplate.Instance.BindItemDropdowns(objPatchItemMaster, EntityType.PatchCord.ToString());
            return PartialView("_PatchCordTemplate", objPatchItemMaster);
        }



        public ActionResult SavePatchCordTemplate(PatchCordItemMaster objPatchCordItem)
        {
            PageMessage objPM = new PageMessage();

            if (ModelState.IsValid)
            {
                var isNewTemplate = (objPatchCordItem.id > 0 && objPatchCordItem.created_by == Convert.ToInt32(Session["user_id"]));
                var resultItem = new BLPatchCordItemMaster().SavePatchCordItemTemplate(objPatchCordItem, Convert.ToInt32(Session["user_id"]));

                if (isNewTemplate)  // Update 
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = Resources.Resources.SI_OSP_PCD_NET_FRM_004;
                }
                else
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = Resources.Resources.SI_OSP_PCD_NET_FRM_003;
                    objPatchCordItem = resultItem;
                }
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            }
            //fill dropdown                     
            BLItemTemplate.Instance.BindItemDropdowns(objPatchCordItem, EntityType.PatchCord.ToString());
            objPatchCordItem.objPM = objPM;
            return PartialView("_PatchCordTemplate", objPatchCordItem);
        }
        #endregion

        #region FMS
        public PartialViewResult PITTemplate(string eType)
        {

            PITTemplateMaster objFMSItemMaster = BLItemTemplate.Instance.GetTemplateDetail<PITTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.PIT);
            return PartialView("_PITTemplate", objFMSItemMaster);
        }

        public ActionResult SavePITTemplate(PITTemplateMaster objPITTemplate)
        {
            PageMessage objPM = new PageMessage();
            if (ModelState.IsValid)
            {
                bool isNew = objPITTemplate.id > 0;
                var resultItem = new BLROW().SavePitTemplate(objPITTemplate, Convert.ToInt32(Session["user_id"]));

                if (isNew)  // Update 
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_020;// "PIT template update successfully.";
                }
                else
                {
                    objPM.status = ResponseStatus.OK.ToString();
                    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_021;// "PIT template saved successfully.";
                    objPITTemplate = resultItem;
                }
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
            }

            objPITTemplate.objPM = objPM;
            return PartialView("_PITTemplate", objPITTemplate);
        }

        #endregion

        //cabinet shazia 
        #region Cabinet
        public PartialViewResult CabinetTemplate(string eType)
        {
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CabinetItemMaster>(url, obj, EntityType.Cabinet.ToString(), EntityAction.Get.ToString());
            return PartialView("_CabinetTemplate", response.results);
		}
        public PartialViewResult OpticalRepeaterTemplate()
        {
            //var objHTBTemplate = BLISP.Instance.getHTBTemplate(Convert.ToInt32(((User)Session["userDetail"]).user_id));
            //HTBTemplate objHTBItemMaster = BLItemTemplate.Instance.GetTemplateDetail<HTBTemplate>(Convert.ToInt32(Session["user_id"]), EntityType.HTB);
            //objHTBItemMaster.created_by = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            //BLItemTemplate.Instance.BindItemDropdowns(objHTBItemMaster, EntityType.HTB.ToString());
            //new MiscHelper().BindPortDetails(objHTBItemMaster, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //return PartialView("_HTBTemplate", objHTBItemMaster);
            var obj = new { userId = (Convert.ToInt32(((User)Session["userDetail"]).user_id)) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<OpticalRepeaterTemplate>(url, obj, EntityType.OpticalRepeater.ToString(), EntityAction.Get.ToString());
            return PartialView("_OpticalRepeaterTemplate", response.results);
        }
        private void BindCabinetDropdown(CabinetItemMaster objCabinetItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Cabinet.ToString());
            objCabinetItemMaster.listCabinetType = objDDL.Where(x => x.dropdown_type == DropDownType.Cabinet_Type.ToString()).ToList();
        }
        public ActionResult SaveCabinetTemplate(CabinetItemMaster objCabinetItem)
        {
            objCabinetItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<CabinetItemMaster>(url, objCabinetItem, EntityType.Cabinet.ToString(), EntityAction.Save.ToString());
            return PartialView("_CabinetTemplate", response.results);
        }
        #endregion

        //cabinet shazia end 
        //Vault shazia 
        #region Vault
        public PartialViewResult VaultTemplate(string eType)
        {
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<VaultItemMaster>(url, obj, EntityType.Vault.ToString(), EntityAction.Get.ToString());
            return PartialView("_VaultTemplate", response.results);
        }
        private void BindVaultDropdown(VaultItemMaster objVaultItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Vault.ToString());
            objVaultItemMaster.listVaultType = objDDL.Where(x => x.dropdown_type == DropDownType.Vault_Type.ToString()).ToList();
        }
        public ActionResult SaveVaultTemplate(VaultItemMaster objVaultItem)
        {
            objVaultItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<VaultItemMaster>(url, objVaultItem, EntityType.Vault.ToString(), EntityAction.Save.ToString());
            return PartialView("_VaultTemplate", response.results);
        }
        #endregion

        //Vault shazia end 
    

        public ActionResult saveOpticalRepeaterTemplate(OpticalRepeaterTemplate objOpticalRepeaterTemplate)
        {
            //PageMessage objPM = new PageMessage();
            //var htbTemplate = new HTBTemplate();


            //int unitSystemId = objHTBTemplate.system_id;
            //if (objHTBTemplate.unitValue != null && objHTBTemplate.unitValue.Contains(":"))
            //{
            //    objHTBTemplate.no_of_input_port = Convert.ToInt32(objHTBTemplate.unitValue.Split(':')[0]);
            //    objHTBTemplate.no_of_output_port = Convert.ToInt32(objHTBTemplate.unitValue.Split(':')[1]);
            //}
            //htbTemplate = new BLISP().saveHTBTemplate(objHTBTemplate);
            //if (unitSystemId > 0)
            //{

            //    objPM.status = ResponseStatus.OK.ToString(); ;
            //    objPM.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_147;// "HTB template updated successfully.";
            //}
            //else
            //{
            //    objPM.status = ResponseStatus.OK.ToString(); ;
            //    objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_019;// "HTB template saved successfully.";
            //}
            ////fill dropdown
            //BLItemTemplate.Instance.BindItemDropdowns(htbTemplate, EntityType.HTB.ToString());
            //new MiscHelper().BindPortDetails(htbTemplate, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
            //htbTemplate.objPM = objPM;
            //return PartialView("_HTBTemplate", htbTemplate);
            objOpticalRepeaterTemplate.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<OpticalRepeaterTemplate>(url, objOpticalRepeaterTemplate, EntityType.OpticalRepeater.ToString(), EntityAction.Save.ToString());
            return PartialView("_OpticalRepeaterTemplate", response.results);
        }

        //Handhole BY ANTRA
        #region handhole
        public PartialViewResult HandholeTemplate(string eType)
        {
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HandholeItemMaster>(url, obj, EntityType.Handhole.ToString(), EntityAction.Get.ToString());
            return PartialView("_HandholeTemplate", response.results);
        }

        public ActionResult SaveHandholeTemplate(HandholeItemMaster objHandholeItem)
        {
            objHandholeItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<HandholeItemMaster>(url, objHandholeItem, EntityType.Handhole.ToString(), EntityAction.Save.ToString());
            return PartialView("_HandholeTemplate", response.results);
        }
        #endregion
        //

        //PatchPanel by shazia 
        #region PatchPanel
        public PartialViewResult PatchPanelTemplate(string eType)
        { 
            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PatchPanelItemMaster>(url, obj, EntityType.PatchPanel.ToString(), EntityAction.Get.ToString());
            return PartialView("_PatchPanelTemplate", response.results);
        }

        public ActionResult SavePatchPanelTemplate(PatchPanelItemMaster objFMSItem)
        {
           
            objFMSItem.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<PatchPanelItemMaster>(url, objFMSItem, EntityType.PatchPanel.ToString(), EntityAction.Save.ToString());
            return PartialView("_PatchPanelTemplate", response.results);
        }

        #endregion
        //end

        public ActionResult GetMicroductNoOfWaysData(string entitytype, string specification, int vendorId)
        {
            JsonResponse<List<itemCategory>> objResp = new JsonResponse<List<itemCategory>>();

            List<itemCategory> lst = BLItemTemplate.Instance.GetMicroductNoOfWaysData(entitytype, specification, vendorId);
            if (lst.Count > 0)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.result = lst;
            }
            else
            {
                objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
            }



            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GipipeTemplate(string eType)
        {

            var obj = new { eType = eType, userId = Convert.ToInt32(Session["user_id"]) };
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<GipipeItemMaster>(url, obj, EntityType.Gipipe.ToString(), EntityAction.Get.ToString());
            return PartialView("_GipipeTemplate", response.results);
        }

        public ActionResult SaveGipipeTemplate(GipipeItemMaster objGipipeItem)
        {
            objGipipeItem.userId = Convert.ToInt32(Session["user_id"]);
            string url = "api/ItemTemplate/EntityTemplate";
            var response = WebAPIRequest.PostIntegrationAPIRequest<GipipeItemMaster>(url, objGipipeItem, EntityType.Gipipe.ToString(), EntityAction.Save.ToString());
            return PartialView("_GipipeTemplate", response.results);
        }

    }
}