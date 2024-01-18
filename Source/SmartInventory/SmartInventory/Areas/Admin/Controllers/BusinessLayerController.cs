using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using SmartInventory.Settings;
using Utility;
using SmartInventory.Filters;
using System.Drawing;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class BusinessLayerController : Controller
    {

        public ActionResult AddBusinessLayer(int id = 0)
        {
            BusinessLayer businessLayer = new BusinessLayer();
            ViewBag.LayerType= new List<SelectListItem>() { new SelectListItem(){Text="WMS", Value="WMS"}, new SelectListItem() { Text = "WMTS", Value = "WMTS" }, new SelectListItem() { Text = "OTHERS", Value = "OTHERS" }  };
            if (id > 0)
            {
                businessLayer = new BLBusinessLayer().GetBusinessLayer(id);
            }

            return View(businessLayer);
        }
        [HttpPost]
        public JsonResult SaveBusinessLayer(BusinessViewModel businessLayer)
        {
            var userid = ((User)Session["userDetail"]).user_id;
            ModelState.Clear();
            var f = Request.Files.Count;
            List<BusinessLayer> lstbusiness=   new List<BusinessLayer>();
            if (businessLayer.id==0)
            {
                if (businessLayer.LayerDetails != null)
                {
                    foreach (var lyr in businessLayer.LayerDetails)
                    {

                        string dirFullPath = Server.MapPath("~/Content/images/mapManager/");
                        string filePath = dirFullPath + lyr.layer_name + ".png";

                        if (lyr.imagepath != null)
                            Utility.commonUtil.Base64toImage(filePath, lyr.imagepath.Replace("data:image/png;base64,", ""));

                        lstbusiness.Add(new BusinessLayer()
                        {
                            authentication_key = businessLayer.authentication_key,
                            base_url = businessLayer.base_url,
                            created_by = userid,
                            created_on = DateTime.Now,
                            layer_type = businessLayer.layer_type,
                            tilematrixset = businessLayer.tilematrixset,
                            version = businessLayer.version,
                            url_display_name = businessLayer.url_display_name,
                            map_file_path = businessLayer.map_file_path,

                            display_layer_name = lyr.display_layer_name,
                            is_active = lyr.is_active,
                            layer_name = lyr.layer_name,
                            style = lyr.style,
                            used_for = lyr.used_for,
                            isbaselayer = lyr.isbaselayer,
                            #region priyanka
                            srs = lyr.srs,
                            crs = lyr.crs,
                            request = lyr.request,
                            service = lyr.service,
                            format = lyr.format,
                            reqver = lyr.reqver,
                            transparent = lyr.transparent,
                            #endregion
                            imagepath = lyr.imagepath != null ? "Content/images/mapManager/" + lyr.layer_name + ".png" : ""
                        });
                    }
                    new BLBusinessLayer().SaveBusinessLayer(lstbusiness);
                    return Json(new { status = ResponseStatus.OK.ToString(), message = "Business Layer saved successfully." });
                }
                else
                {
                    return Json(new { status = ResponseStatus.ERROR.ToString(), message = "Enter required fields values." });
                }

            }
            return Json(new { status = ResponseStatus.ERROR.ToString(), message = "Oops! something went worng." });
        }
        [HttpPost]
        public ActionResult UpdateBusinessLayer(BusinessLayer businessLayer)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            PageMessage objMsg = new PageMessage();
            ModelState.Clear();
            ViewBag.LayerType = new List<SelectListItem>() { new SelectListItem() { Text = "WMS", Value = "WMS" }, new SelectListItem() { Text = "WMTS", Value = "WMTS" }, new SelectListItem() { Text = "OTHERS", Value = "OTHERS" } };
            var objbusinessExist = new BLBusinessLayer().GetBusinessLayerByLayername(businessLayer.base_url,businessLayer.layer_name);
            if (businessLayer.id > 0)
            {
                if (businessLayer.hdnlayername != businessLayer.layer_name)
                {
                    if ((objbusinessExist != null && objbusinessExist.layer_name.Trim().ToLower() == businessLayer.layer_name.Trim().ToLower()))
                    {
                        objMsg.status = ResponseStatus.ERROR.ToString();
                        objMsg.message = "Layer Name with same Url already exist!";
                        businessLayer.pageMsg = objMsg;
                        return View("AddBusinessLayer", businessLayer);
                    }
                }
                objMsg.status = ResponseStatus.OK.ToString();
                businessLayer = new BLBusinessLayer().UpdateBusinessLayerByID(businessLayer, objLgnUsrDtl);
                objMsg.message = "Business Layer details updated successfully.";
            }
            businessLayer.pageMsg = objMsg;
            return View("AddBusinessLayer", businessLayer);
        }

        public JsonResult checklayerExists(string baseurl,string layername)
        {
            var objbusinessExist = new BLBusinessLayer().GetBusinessLayerByLayername(baseurl,layername);
            if ((objbusinessExist != null && String.IsNullOrEmpty(layername)) || (objbusinessExist != null && objbusinessExist.layer_name.Trim().ToLower() == layername.Trim().ToLower()))
            {
                return Json(new { status = ResponseStatus.ERROR.ToString(), message= "Layer Name already exist!" });
            }else
            {
                return Json(new { status = ResponseStatus.OK.ToString(), message = "Layer Name not exist!" });
            }
        }

        public ActionResult ViewBusinessLayer(ViewBusinessLayerModel objViewBusinessLayer, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            //ModelState.Clear();
            if (sort != "" || page != 0)
            {
                objViewBusinessLayer.objGridAttributes = (CommonGridAttributes)Session["viewBusinessLayerDetails"];
            }
            objViewBusinessLayer.lstSearchBy = GetBusinessLayerSearchByColumns();
            objViewBusinessLayer.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewBusinessLayer.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewBusinessLayer.objGridAttributes.sort = sort;
            objViewBusinessLayer.objGridAttributes.orderBy = sortdir;
            objViewBusinessLayer.lstBusinessLayer = new BLBusinessLayer().GetBusinessLayerList(objViewBusinessLayer.objGridAttributes);
            objViewBusinessLayer.Id = objLgnUsrDtl.is_admin_rights_enabled ? objLgnUsrDtl.user_id : 0;
            objViewBusinessLayer.objGridAttributes.totalRecord = objViewBusinessLayer.lstBusinessLayer != null && objViewBusinessLayer.lstBusinessLayer.Count > 0 ? objViewBusinessLayer.lstBusinessLayer[0].totalRecords : 0;
            Session["viewBusinessLayerDetails"] = objViewBusinessLayer.objGridAttributes;
            return View("ViewBusinessLayer", objViewBusinessLayer);
        }
        public List<KeyValueDropDown> GetBusinessLayerSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Layer Name", value = "layer_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Url Display Name", value = "url_display_name" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }

        public JsonResult DeleteBusinessLayerByID(int id)
        {

            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BLBusinessLayer().DeleteBusinessLayerById(id);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "Business Layer Deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting Business Layer!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Business Layer not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


    }
}