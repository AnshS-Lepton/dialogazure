using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using BusinessLogics;
using Models;
using Models.Admin;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using Utility;
using BusinessLogics.Admin;
using System.Text;
using static Mono.Security.X509.X520;
using System.Data.Entity.Infrastructure;
using NPOI.SS.Formula;
using Models.WFM;
using System.Runtime.Remoting;
using System.Dynamic;
using static iTextSharp.text.pdf.AcroFields;

namespace SmartInventory.Controllers
{
	//[Authorize]
	//[SessionExpire]
	[HandleException]
	public class BOMBOQController : Controller
	{
		// GET: BOMBOQ
		public ActionResult BomBoqReport(BomBoqExportReport objBomBoq)
		{
			var userdetails = (User)Session["userDetail"];
			objBomBoq.objReportFilters.SelectedRegionIds = objBomBoq.objReportFilters.SelectedRegionId != null && objBomBoq.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedRegionId.ToArray()) : "";
			objBomBoq.objReportFilters.SelectedProvinceIds = objBomBoq.objReportFilters.SelectedProvinceId != null && objBomBoq.objReportFilters.SelectedProvinceId.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedProvinceId.ToArray()) : "";
			objBomBoq.objReportFilters.SelectedNetworkStatues = objBomBoq.objReportFilters.SelectedNetworkStatus != null && objBomBoq.objReportFilters.SelectedNetworkStatus.Count > 0 ? "'" + string.Join("','", objBomBoq.objReportFilters.SelectedNetworkStatus.ToArray()) + "'" : "";
			objBomBoq.objReportFilters.SelectedParentUsers = objBomBoq.objReportFilters.SelectedParentUser != null && objBomBoq.objReportFilters.SelectedParentUser.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedParentUser.ToArray()) : "";
			objBomBoq.objReportFilters.SelectedUserIds = objBomBoq.objReportFilters.SelectedUserId != null && objBomBoq.objReportFilters.SelectedUserId.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedUserId.ToArray()) : "";
			objBomBoq.objReportFilters.SelectedLayerIds = objBomBoq.objReportFilters.SelectedLayerId != null && objBomBoq.objReportFilters.SelectedLayerId.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedLayerId.ToArray()) : "";
			objBomBoq.objReportFilters.SelectedProjectIds = objBomBoq.objReportFilters.SelectedProjectId != null && objBomBoq.objReportFilters.SelectedProjectId.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedProjectId.ToArray()) : "";
			objBomBoq.objReportFilters.SelectedPlanningIds = objBomBoq.objReportFilters.SelectedPlanningId != null && objBomBoq.objReportFilters.SelectedPlanningId.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedPlanningId.ToArray()) : "";
			objBomBoq.objReportFilters.SelectedWorkOrderIds = objBomBoq.objReportFilters.SelectedWorkOrderId != null && objBomBoq.objReportFilters.SelectedWorkOrderId.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedWorkOrderId.ToArray()) : "";
			objBomBoq.objReportFilters.SelectedPurposeIds = objBomBoq.objReportFilters.SelectedPurposeId != null && objBomBoq.objReportFilters.SelectedPurposeId.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedPurposeId.ToArray()) : "";
			objBomBoq.objReportFilters.userId = Convert.ToInt32(userdetails.user_id);
			objBomBoq.objReportFilters.roleId = Convert.ToInt32(userdetails.role_id);
			objBomBoq.objReportFilters.SelectedOwnerShipType = objBomBoq.objReportFilters.SelectedOwnerShipType != null ? objBomBoq.objReportFilters.SelectedOwnerShipType : "";
			objBomBoq.objReportFilters.SelectedThirdPartyVendorIds = objBomBoq.objReportFilters.SelectedThirdPartyVendorId != null && objBomBoq.objReportFilters.SelectedThirdPartyVendorId.Count > 0 ? string.Join(",", objBomBoq.objReportFilters.SelectedThirdPartyVendorId.ToArray()) : "";
			objBomBoq.objReportFilters.radius = objBomBoq.objReportFilters.radius;
			objBomBoq.lstDurationBasedOn = new BLMisc().GetDropDownList("", DropDownType.Export_Report.ToString());
			objBomBoq.lstWave_length = new BusinessLogics.Admin.BLLinkBudget().GetWaveLength();
            objBomBoq.objReportFilters.selected_route_ids = objBomBoq.objReportFilters.selected_route_ids != null && objBomBoq.selected_route_ids.Count > 0 ? string.Join(",", objBomBoq.selected_route_ids.ToArray()) : "";

            //if ((objBomBoq.objReportFilters.SelectedRegionIds == null || objBomBoq.objReportFilters.SelectedRegionIds == "") && string.IsNullOrEmpty(objBomBoq.objReportFilters.geom) == false && string.IsNullOrEmpty(objBomBoq.objReportFilters.geomType) == false)
            //{
            //    string gtype = Convert.ToString(objBomBoq.objReportFilters.geomType).ToLower() == "circle" ? "Point" : objBomBoq.objReportFilters.geomType;
            //    var result = BLBuilding.Instance.GetRegionProvince(objBomBoq.objReportFilters.geom, gtype);
            //    if (result.Count == 1)
            //    {
            //        objBomBoq.objReportFilters.SelectedRegionIds = Convert.ToString(result[0].region_id);
            //        objBomBoq.objReportFilters.SelectedProvinceIds = Convert.ToString(result[0].province_id);                
            //    }
            //}

            BindReportDropdownNew(ref objBomBoq);
			Session["BomBoqExportSummaryData"] = objBomBoq;
			return PartialView("_BomBoqReport", objBomBoq);

		}

		public ActionResult BomBoqFilter(BomBoqExportReport objBomBoq)
		{
			objBomBoq.lstNetworkStatus = new BLMisc().GetDropDownList("", DropDownType.ddlNetworkStatus.ToString());
			return PartialView("_BomBoqFilter", objBomBoq);
		}

		public ActionResult SiteBomBoqSummary(ViewItemVendorCost objViewItemVendorCost, int page = 0, string sort = "", string sortdir = "", string refrenceData = "")
		{
			var userdetails = (User)Session["userDetail"];
            CommonGridAttr objGridAttributes = new CommonGridAttr();
            BindSearchBy(objViewItemVendorCost);
            if (sort != "" || page != 0)
            {
                objViewItemVendorCost.objGridAttributes = (CommonGridAttr)Session["ViewItemVendorCost"];
            }
            objViewItemVendorCost.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewItemVendorCost.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewItemVendorCost.objGridAttributes.sort = sort;
            objViewItemVendorCost.objGridAttributes.orderBy = sortdir;
            var firstItem = refrenceData.Split(',')[0];
            var siteplanid = new BomBoq().getSiteplanid(Convert.ToInt32(firstItem));
            // objViewItemVendorCost.objGridAttributes.SelectedLayerIds =  string.Join(",", objBomBoq.objReportFilters.SelectedLayerId.ToArray());
            objViewItemVendorCost.lstItem = new BomBoq().getSiteBOMBOQReport(objViewItemVendorCost.objGridAttributes, siteplanid);
           
            var users = objViewItemVendorCost.lstItem.Where(u=>u.user_id!=0)
                .Select(x => new { x.user_id, x.user_name })
                .Distinct()
                .OrderBy(x => x.user_id) // Ensure ordering is based on user_name
                .ToList();

			foreach (var item in users)
			{
                parentuser newUser = new parentuser
                {
                    user_id = item.user_id,
                    user_name = item.user_name
                };
				objViewItemVendorCost.lstUserDetails.Add(newUser);
            }
			//objViewItemVendorCost.lstUserDetails = users.Cast<parentuser>().ToList(); //new BLUser().GetPartnerUser().ToList();

            Session["ViewItemVendorCost"] = objViewItemVendorCost.objGridAttributes;
            // Transform data dynamically
            var transformedData = objViewItemVendorCost.lstItem
                    .GroupBy(x => new { x.code, x.specification, x.category_reference, x.unit_measurement, x.totalqty})
                    .Select(g =>
                    {
                        dynamic row = new ExpandoObject();
                        var dict = (IDictionary<string, object>)row;

                        // Fixed properties
                        dict["code"] = g.Key.code;
                        dict["specification"] = g.Key.specification;
                        dict["entity_type"] = g.Key.category_reference;
                        dict["uom"] = g.Key.unit_measurement;
                        dict["totalqty"] = g.Key.totalqty;


                        // Dynamically add user columns
                        foreach (var user in users)
                        {
                            var costValue = g.FirstOrDefault(x => x.user_id == user.user_id)?.cost_per_unit.ToString() ?? "";
                            //dict[$"User_{user.user_name}"] = costValue+"/"+user.user_id;
                            dict[$"User_{user.user_name + "/" + user.user_id}"] = costValue;
                        }

                        return row;
                    })
                .ToList();

            ViewBag.transformedData = transformedData;


            objViewItemVendorCost.objGridAttributes.totalRecord = objViewItemVendorCost.lstItem.Select(a => a.totalRecord).FirstOrDefault();

            return PartialView("_ItemSiteAwarding", objViewItemVendorCost);
		}
        
        [HttpPost]
        public JsonResult AwardSitetoUser(List<SiteAwardDetails> objivcm)
        {
            
                PageMessage objMsg = new PageMessage();

            SiteAwardDetails objResp = new SiteAwardDetails();
            DbMessage objDBMessage=  new DbMessage ();
            // var status = new BLVendorSpecification().SaveSiteAwardDetails(objivcm, Convert.ToInt32(Session["user_id"]));
            try
            {
               objDBMessage = new BLVendorSpecification().SaveSiteAwardDetails(objivcm, Convert.ToInt32(Session["user_id"]));
				if (!objDBMessage.status)
				{
					objResp.pageMsg.status = ResponseStatus.OK.ToString();
					objResp.pageMsg.message = objDBMessage.message;

				}
				else
				{
                    objResp.pageMsg.status = ResponseStatus.OK.ToString();
                    objResp.pageMsg.message = objDBMessage.message;

                }
               
            }
            catch
            {
                objResp.pageMsg.status = ResponseStatus.ERROR.ToString();
                objResp.pageMsg.message = "Some error occured  while site awarding";
            }
           
            //objSA.pageMsg = objDBMessage;
            return Json(objDBMessage, JsonRequestBehavior.AllowGet);
            
        }
        public IList<KeyValueDropDown> BindSearchBy(ViewItemVendorCost objTemplateForDropDown)
        {
            List<KeyValueDropDown> items = new List<KeyValueDropDown>();
            items.Add(new KeyValueDropDown { key = "Item Code", value = "code" });
            items.Add(new KeyValueDropDown { key = "Specification", value = "specification" });
            items.Add(new KeyValueDropDown { key = "Entity Type", value = "category_reference" });
            items.Add(new KeyValueDropDown { key = "UOM", value = "unit_measurement" });
            return objTemplateForDropDown.lstBindSearchBy = items.OrderBy(m => m.key).ToList();

        }
        public ActionResult BomBoqSummary(BomBoqExportReport objBOMViewModel)
        {
            var userdetails = (User)Session["userDetail"];
            // var moduleAbbr = "BMQ";
            // List<ConnectionMaster> con = new BLLayer().GetConnectionString(moduleAbbr);
            // ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);
            // foreach (var conn in con)
            // {
            //   objBOMViewModel.objReportFilters.connectionString = con.connection_string;
            //  }

            objBOMViewModel.objReportFilters.durationbasedon = (objBOMViewModel.objReportFilters.durationbasedon == null || objBOMViewModel.objReportFilters.durationbasedon == "" ? "Created_On" : objBOMViewModel.objReportFilters.durationbasedon);

            objBOMViewModel.objReportFilters.SelectedRegionIds = objBOMViewModel.objReportFilters.SelectedRegionId != null && objBOMViewModel.objReportFilters.SelectedRegionId.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedRegionId.ToArray()) : "";
            objBOMViewModel.objReportFilters.SelectedProvinceIds = objBOMViewModel.objReportFilters.SelectedProvinceId != null && objBOMViewModel.objReportFilters.SelectedProvinceId.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedProvinceId.ToArray()) : "";
            objBOMViewModel.objReportFilters.SelectedNetworkStatues = objBOMViewModel.objReportFilters.SelectedNetworkStatus != null && objBOMViewModel.objReportFilters.SelectedNetworkStatus.Count > 0 ? "" + string.Join(",", objBOMViewModel.objReportFilters.SelectedNetworkStatus.ToArray()) + "" : "";
            objBOMViewModel.objReportFilters.SelectedParentUsers = objBOMViewModel.objReportFilters.SelectedParentUser != null && objBOMViewModel.objReportFilters.SelectedParentUser.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedParentUser.ToArray()) : "";
            objBOMViewModel.objReportFilters.SelectedUserIds = objBOMViewModel.objReportFilters.SelectedUserId != null && objBOMViewModel.objReportFilters.SelectedUserId.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedUserId.ToArray()) : "";
            objBOMViewModel.objReportFilters.SelectedLayerIds = objBOMViewModel.objReportFilters.SelectedLayerId != null && objBOMViewModel.objReportFilters.SelectedLayerId.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedLayerId.ToArray()) : "";
            objBOMViewModel.objReportFilters.SelectedProjectIds = objBOMViewModel.objReportFilters.SelectedProjectId != null && objBOMViewModel.objReportFilters.SelectedProjectId.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedProjectId.ToArray()) : "";
            objBOMViewModel.objReportFilters.SelectedPlanningIds = objBOMViewModel.objReportFilters.SelectedPlanningId != null && objBOMViewModel.objReportFilters.SelectedPlanningId.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedPlanningId.ToArray()) : "";
            objBOMViewModel.objReportFilters.SelectedWorkOrderIds = objBOMViewModel.objReportFilters.SelectedWorkOrderId != null && objBOMViewModel.objReportFilters.SelectedWorkOrderId.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedWorkOrderId.ToArray()) : "";
            objBOMViewModel.objReportFilters.SelectedPurposeIds = objBOMViewModel.objReportFilters.SelectedPurposeId != null && objBOMViewModel.objReportFilters.SelectedPurposeId.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedPurposeId.ToArray()) : "";
            objBOMViewModel.objReportFilters.userId = Convert.ToInt32(userdetails.user_id);
            objBOMViewModel.objReportFilters.roleId = Convert.ToInt32(userdetails.role_id);
            objBOMViewModel.objReportFilters.SelectedOwnerShipType = objBOMViewModel.objReportFilters.SelectedOwnerShipType != null ? objBOMViewModel.objReportFilters.SelectedOwnerShipType : "";
            objBOMViewModel.objReportFilters.SelectedThirdPartyVendorIds = objBOMViewModel.objReportFilters.SelectedThirdPartyVendorId != null && objBOMViewModel.objReportFilters.SelectedThirdPartyVendorId.Count > 0 ? string.Join(",", objBOMViewModel.objReportFilters.SelectedThirdPartyVendorId.ToArray()) : "";
            objBOMViewModel.objReportFilters.SelectedWavelength = objBOMViewModel.objReportFilters.SelectedWavelength;
            objBOMViewModel.objReportFilters.txt_Miscdbloss = objBOMViewModel.objReportFilters.txt_Miscdbloss;
            objBOMViewModel.objReportFilters.isdBLossAttrEnable = objBOMViewModel.objReportFilters.isdBLossAttrEnable;
            objBOMViewModel.objReportFilters.radius = objBOMViewModel.objReportFilters.radius;
            objBOMViewModel.objReportFilters.systemId = objBOMViewModel.objReportFilters.systemId;
            objBOMViewModel.objReportFilters.entityType = objBOMViewModel.objReportFilters.entityType;
            string gom = Convert.ToString(objBOMViewModel.objReportFilters.geom);
            string enti = Convert.ToString(objBOMViewModel.objReportFilters.entityType);
            objBOMViewModel.objReportFilters.selected_route_ids = objBOMViewModel.selected_route_ids != null && objBOMViewModel.selected_route_ids.Count > 0 ? string.Join(",", objBOMViewModel.selected_route_ids.ToArray()) : "";

            objBOMViewModel.objReportFilters.geom = (gom == null ? "" : (enti != null ? (enti.ToUpper() == "STRUCTURE" ? "" : gom) : gom));
            Session["BomBoqReportFilters"] = objBOMViewModel.objReportFilters;
            objBOMViewModel.BomBoqReportList = new BomBoq().getBOMBOQReport(objBOMViewModel.objReportFilters);
            objBOMViewModel.objdBLoss = new BomBoq().getdBLossReport(objBOMViewModel.objReportFilters);
            Session["BomBoqExportSummaryData"] = objBOMViewModel;
            objBOMViewModel.lstNetworkStatus = new BLMisc().GetDropDownList("", DropDownType.ddlNetworkStatus.ToString());
            return PartialView("_BomBoqSummary", objBOMViewModel);
        }

        public ActionResult BomBoqExport(BomBoqExportReport objBomBoq)
		{
			return PartialView("_BomBoqSummary", objBomBoq);
		}

		public void BindReportDropdownNew(ref BomBoqExportReport objExportEntitiesReport)
		{
			var userdetails = (User)Session["userDetail"];
			//Bind Layers..
			string lyrType = string.IsNullOrEmpty(objExportEntitiesReport.objReportFilters.entityType) == true ? "" : objExportEntitiesReport.objReportFilters.entityType;
			objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(userdetails.role_id, (lyrType.ToUpper() == "STRUCTURE" ? "ISP" : "BOMBOQ"));
			//Bind Regions..
			objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
			//Bind Provinces..
			if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
			{
				objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
			}

			//for project code 
			objExportEntitiesReport.lstBindProjectCode = new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedNetworkStatues) ? "P" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "PLANNED" ? "P" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "AS BUILT" ? "A" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "DORMANT" ? "D" : "P");

			//////// for  planning code
			if (objExportEntitiesReport.objReportFilters.SelectedProjectId != null)
				objExportEntitiesReport.lstBindPlanningCode = new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(objExportEntitiesReport.objReportFilters.SelectedProjectId);
			////////// for  workordercode
			if (objExportEntitiesReport.objReportFilters.SelectedPlanningId != null)
				objExportEntitiesReport.lstBindWorkorderCode = new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(objExportEntitiesReport.objReportFilters.SelectedPlanningId);

			/////// for  purpose code
			if (objExportEntitiesReport.objReportFilters.SelectedWorkOrderId != null)
				objExportEntitiesReport.lstBindPurposeCode = new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(objExportEntitiesReport.objReportFilters.SelectedWorkOrderId);

			//for duration based on 
			objExportEntitiesReport.lstDurationBasedOn = new BLMisc().GetDropDownList("", DropDownType.Export_Report.ToString());

			//objExportEntitiesReport.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objExportEntitiesReport.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();

			// user and list of user
			List<int> parentUser = new List<int>();
			parentUser.Add(1);
			if (userdetails.role_id == 1)
				objExportEntitiesReport.lstParentUsers = new BLUser().GetUsersListByMGRIds(parentUser).OrderBy(x => x.user_name).ToList();
			else
			{
				objExportEntitiesReport.lstParentUsers = new List<Models.User>();
				objExportEntitiesReport.lstParentUsers.Add(userdetails);
			}
			if (objExportEntitiesReport.objReportFilters.SelectedParentUser != null)
			{
				if (userdetails.role_id != 1)
				{
					var parentUser_ids = string.Join(",", objExportEntitiesReport.objReportFilters.SelectedParentUser.Select(n => n.ToString()).ToArray());
					objExportEntitiesReport.lstUsers = new BLUser().GetUserReportDetailsList(parentUser_ids).ToList();
				}
				else
				{
					objExportEntitiesReport.lstUsers = new BLUser().GetUsersListByMGRIds(objExportEntitiesReport.objReportFilters.SelectedParentUser).OrderBy(x => x.user_name).ToList();
				}
			}
			objExportEntitiesReport.lstNetworkStatus = new BLMisc().GetDropDownList("", DropDownType.ddlNetworkStatus.ToString());
			objExportEntitiesReport.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            objExportEntitiesReport.lstRouteInfo = new BLLayer().getRouteInfo("0");
        }

		public ActionResult BomBoqAdAttribute(BomBoqAdAttribute objBomBoqAttr)
		{
			var userdetails = (User)Session["userDetail"];
			var datalist = new BomBoq().getBOMBOQExportAttribute(userdetails.user_id);
			BomBoqAdAttribute objatt = new BomBoqAdAttribute();
			objatt.userid = userdetails.user_id;
			if (datalist.Count > 0)
			{
				objatt.title = datalist[0].title;
				objatt.equipmenttype = datalist[0].equipmenttype;
				objatt.equipmentname = datalist[0].equipmentname;
				objatt.estimatedby = datalist[0].estimatedby;
				objatt.checkedby = datalist[0].checkedby;
				objatt.re_checkedby = datalist[0].re_checkedby;
				objatt.approvedby = datalist[0].approvedby;
			}
			var objAttr = Session["BomBoqExportSummaryData"] as BomBoqExportReport;
			objAttr.objAdAttribute = objatt;
			Session["BomBoqExportSummaryData"] = objAttr;
			return PartialView("_BomBoqAdAttribute", objAttr);
		}

		public JsonResult GetMiscdBLoss(int wavelength_id = 0)
		{
			LinkBudgetMaster objLinkBudget = wavelength_id != 0 ? new BusinessLogics.Admin.BLLinkBudget().GetLinkBudgetDetailByID(wavelength_id) : new LinkBudgetMaster();
			return Json(new { Data = objLinkBudget.misc_db_loss, JsonRequestBehavior.AllowGet });
		}
		public JsonResult PrintBOMBOQ(BomBoqExportReport objBomBoq, string ExpRptType)
		{
			int success = 0;
			string strMsg = "";
			if (objBomBoq.objAdAttribute.isAdditionalAttrEnable == true)
			{
				var ExpAttr = new BomBoq().Save_ExportAttribute(objBomBoq.objAdAttribute);
			}
			var objAttr = Session["BomBoqExportSummaryData"] as BomBoqExportReport;
			objAttr.objAdAttribute = objBomBoq.objAdAttribute;
			Session["BomBoqExportSummaryData"] = objAttr;

			return Json(new { success = success, message = strMsg }, JsonRequestBehavior.AllowGet);
		}
		public JsonResult SetBomBoqAdAttribute(BomBoqAdAttribute objBomBoqAttr)
		{
			var success = false;
			string strMsg = "";
			try
			{
				var userdetails = (User)Session["userDetail"];
				objBomBoqAttr.userid = userdetails.user_id;
				if (objBomBoqAttr.isAdditionalAttrEnable == true)
				{
					var ExpAttr = new BomBoq().Save_ExportAttribute(objBomBoqAttr);
				}
				var objAttr = Session["BomBoqExportSummaryData"] as BomBoqExportReport;
				objAttr.objAdAttribute = objBomBoqAttr;
				Session["BomBoqExportSummaryData"] = objAttr;
				success = true;
			}
			catch (Exception ex)
			{
				string Msg = ex.Message;
				strMsg = Resources.Resources.SI_OSP_GBL_NET_RPT_134;
			}

			return Json(new { success = success, message = strMsg }, JsonRequestBehavior.AllowGet);
		}

		public void ExportBOMBOQReport(string type)
		{
			if (Session["BomBoqExportSummaryData"] != null && !string.IsNullOrWhiteSpace(type))
			{
				BomBoqExportReport objBomBoq = (BomBoqExportReport)Session["BomBoqExportSummaryData"];
				foreach (var item in objBomBoq.BomBoqReportList)
				{
					if (item.NetworkStatus == "A")
					{
						item.item_code = "";
					}
				}
				string[] tys = type.Split('-');
				if (tys[0].ToUpper() == "PDF".ToUpper())
				{
					PDFHelper._ExportBOMBOQReportInPDF(objBomBoq, tys[1]);
				}
				else if (tys[0].ToUpper() == "EXCEL".ToUpper())
				{
					IWorkbook workbook = GetBOMBOQWorkBook(objBomBoq, tys[1], tys[1], "BOMBOQ");
					string FileName = tys[1] + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss");
					ExportWorkBookToExcel(workbook, FileName);
					//ExportBOMBOQReportExcel(objBomBoq, tys[1]);
				}
			}
		}

		public void ExportBOMBOQReportExcel(BomBoqExportReport objBomBoq, string rptType)
		{
			if (Session["BomBoqExportSummaryData"] != null && !string.IsNullOrWhiteSpace(rptType))
			{
				try
				{
					List<BOMBOQReport> lstBOMReport = objBomBoq.BomBoqReportList;
					var isAttr = ((List<string>)Session["ApplicableModuleList"]);
					#region Add Report section

					System.Data.DataTable dtReport = new System.Data.DataTable(),
						dtTitile = new System.Data.DataTable(),
						dtFilters = new System.Data.DataTable(),
						dtEqp = new System.Data.DataTable();
					System.Data.DataSet ds = new System.Data.DataSet();
					dtReport.TableName = Resources.Resources.SI_OSP_GBL_NET_FRM_064;
					System.Data.DataRow drt;
					int CblCnt = 0, LstCnt = 0;
					var NtStatus = "";
					System.Data.DataRow dr;

					#region Filer Details


					object obj = objBomBoq.objReportFilters;
					var userdetails = (User)Session["userDetail"];
					System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
					System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;

					dtFilters.TableName = "Filters";
					dtFilters.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_166);
					dtFilters.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_167);
					drt = dtFilters.NewRow();
					drt[0] = Resources.Resources.SI_OSP_GBL_GBL_FRM_229;
					dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_065;
					List<int> regionIds = (List<int>)obj.GetType().GetProperty("SelectedRegionId").GetValue(obj, null);
					var regionName = regionIds == null ? "All" : string.Join(",", new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(userdetails.user_id) }).Where(x => regionIds.Contains(x.regionId)).Select(x => x.regionName).ToList());
					drt[1] = regionName; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_066;
					List<int> provinceIds = (List<int>)obj.GetType().GetProperty("SelectedProvinceId").GetValue(obj, null);
					var provinceName = provinceIds == null ? "All" : string.Join(",", new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = string.Join(",", regionIds), userId = Convert.ToInt32(userdetails.user_id) }).Where(x => provinceIds.Contains(x.provinceId)).Select(x => x.provinceName).ToList());
					drt[1] = provinceName; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_063;
					List<string> NWStatus = (List<string>)obj.GetType().GetProperty("SelectedNetworkStatus").GetValue(obj, null);
					string networkStatus = NWStatus == null ? "All" : (string.Join(", ", NWStatus.Select(i => (i.ToUpper() == "P" ? "PLANNED" : i.ToUpper() == "A" ? "AS-BUILT" : i.ToUpper() == "D" ? "DORMANT" : ""))));
					drt[1] = networkStatus; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_274;
					List<int> parentUser = new List<int>();
					List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
					var parentUserName = string.Empty;
					if (userdetails.role_id == 1)
					{
						parentUser.Add(1);
						parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());
					}
					else
					{
						parentUserName = parentUserIds == null ? "All" : new BLUser().GetUserDetailByID(userdetails.user_id).user_name;
					}
					drt[1] = parentUserName; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_RPT_040;
					List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
					var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());
					drt[1] = userName; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_GBL_GBL_NET_FRM_098;
					List<int> layerIds = (List<int>)obj.GetType().GetProperty("SelectedLayerId").GetValue(obj, null);
					var layerName = layerIds == null ? "All" : string.Join(",", new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY").Where(x => layerIds.Contains(x.layer_id)).Select(x => x.layer_title).ToList());
					drt[1] = layerName; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_GBL_GBL_GBL_GBL_147;
					string ownershipType = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedOwnerShipType").GetValue(obj, null).ToString()).Replace("'", "");
					ownershipType = string.IsNullOrEmpty(ownershipType) ? "All" : ownershipType;
					drt[1] = ownershipType; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_FRM_030;
					List<int> thirdPartyVendorId = (List<int>)obj.GetType().GetProperty("SelectedThirdPartyVendorId").GetValue(obj, null);
					var thirdPartyVendorName = thirdPartyVendorId == null ? "All" : string.Join(",", BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList().Where(x => thirdPartyVendorId.Contains(Convert.ToInt32(x.key))).Select(x => x.value).ToList());
					drt[1] = thirdPartyVendorName; dtFilters.Rows.Add(drt);
					//List<int> parentUser = new List<int>();
					//parentUser.Add(1);
					//List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
					//var parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

					//List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
					//var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());


					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_074;
					List<int> projectCodeIds = (List<int>)obj.GetType().GetProperty("SelectedProjectId").GetValue(obj, null);
					var projectCodeName = projectCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrEmpty(networkStatus) ? "P" : networkStatus.ToUpper() == "PLANNED" ? "P" : networkStatus.ToUpper() == "AS BUILT" ? "A" : networkStatus.ToUpper() == "DORMANT" ? "D" : "P").Where(x => projectCodeIds.Contains(x.system_id)).Select(x => x.project_code).ToList());
					drt[1] = projectCodeName; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_076;
					List<int> planningCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPlanningId").GetValue(obj, null);
					var planningCodeName = planningCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(projectCodeIds).Where(x => planningCodeIds.Contains(x.system_id)).Select(x => x.planning_code).ToList());
					drt[1] = planningCodeName; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_009;
					List<int> workOrderCodeIds = (List<int>)obj.GetType().GetProperty("SelectedWorkOrderId").GetValue(obj, null);
					var workOrderCodeName = workOrderCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(planningCodeIds).Where(x => workOrderCodeIds.Contains(x.system_id)).Select(x => x.workorder_code).ToList());
					drt[1] = workOrderCodeName; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_GBL_011;
					List<int> purposeCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPurposeId").GetValue(obj, null);
					var purposeCodeName = purposeCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(workOrderCodeIds).Where(x => purposeCodeIds.Contains(x.system_id)).Select(x => x.purpose_code).ToList());
					drt[1] = purposeCodeName; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_071;
					string durationBasedOn = obj.GetType().GetProperty("durationbasedon").GetValue(obj, null).ToString().Replace("_", " ");
					drt[1] = durationBasedOn; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_072;
					string duration = obj.GetType().GetProperty("fromDate").GetValue(obj, null).ToString() + " To " + obj.GetType().GetProperty("toDate").GetValue(obj, null).ToString();
					drt[1] = duration; dtFilters.Rows.Add(drt);

					drt = dtFilters.NewRow(); dtFilters.Rows.Add(drt);
					drt = dtFilters.NewRow(); dtFilters.Rows.Add(drt);
					dtFilters.AcceptChanges();
					ds.Tables.Add(dtFilters);

					#endregion


					#region Equipment Details
					if (isAttr.Contains("EQPD"))
					{
						if (objBomBoq.objAdAttribute.isEquipmentEnable == true)
						{
							dtEqp.TableName = "Equipments";
							dtEqp.Columns.Add("Equipments");
							dtEqp.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_167);
							drt = dtEqp.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_272;
							dtEqp.Rows.Add(drt);

							drt = dtEqp.NewRow(); drt[0] = Resources.Resources.SI_ISP_GBL_NET_FRM_063;
							drt[1] = objBomBoq.objAdAttribute.equipmenttype; dtEqp.Rows.Add(drt);

							drt = dtEqp.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_118;
							drt[1] = objBomBoq.objAdAttribute.equipmentname; dtEqp.Rows.Add(drt);

							drt = dtEqp.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_GBL_139;
							drt[1] = objBomBoq.objAdAttribute.popname; dtEqp.Rows.Add(drt);
							drt = dtEqp.NewRow(); dtEqp.Rows.Add(drt);
							drt = dtEqp.NewRow(); dtEqp.Rows.Add(drt);
							dtEqp.AcceptChanges();
							ds.Tables.Add(dtEqp);
						}
					}

					#endregion

					#region Extra

					//#region Equipment Details
					//if (isAttr.Contains("EQPD"))
					//{
					//    if (objBomBoq.objAdAttribute.isEquipmentEnable == true)
					//    {
					//        dr = dtReport.NewRow();
					//        dr[0] = "Equipments";
					//        dtReport.Rows.Add(dr);
					//        dr = dtReport.NewRow();
					//        dr[0] = "Value";
					//        dr[1] = "Equipment Details";
					//        dtReport.Rows.Add(dr);

					//        dr = dtReport.NewRow(); dr[0] = "Equipment Type";
					//        dr[1] = objBomBoq.objAdAttribute.equipmenttype; dtReport.Rows.Add(dr);

					//        dr = dtReport.NewRow(); dr[0] = "Equipment Name";
					//        dr[1] = objBomBoq.objAdAttribute.equipmentname; dtReport.Rows.Add(dr);

					//        dr = dtReport.NewRow(); dr[0] = "POP Name";
					//        dr[1] = objBomBoq.objAdAttribute.popname; dtReport.Rows.Add(dr);
					//        dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
					//        dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
					//    }
					//}

					//#endregion



					//#region Title

					//if (!string.IsNullOrWhiteSpace(objBomBoq.objAdAttribute.title))
					//{
					//    dr = dtReport.NewRow();
					//    dr[0] = objBomBoq.objAdAttribute.title;
					//    dtReport.Rows.Add(dr);
					//    dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
					//    dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
					//}

					//#endregion

					#endregion

					#region ADD HEADER COLUMNS
					// ADD HEADER COLUMNS...

					dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_NET_GBL_008);
					dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_GBL_GBL_068);
					dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_NET_RPT_015);
					//priyanka
					dtReport.Columns.Add("Served By Ring");
					if (rptType.ToUpper() == "BOQ")
					{
						dtReport.Columns.Add(String.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_014.Replace("<br>", ""), Settings.ApplicationSettings.Currency));
						dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_NET_RPT_017 + " " + string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_016, Settings.ApplicationSettings.Currency));
					}
					dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_NET_GBL_226);
					if (rptType.ToUpper() == "BOQ")
					{
						dtReport.Columns.Add(string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, Settings.ApplicationSettings.Currency));
						dtReport.Columns.Add(string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, Settings.ApplicationSettings.Currency));
					}
					dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_NET_RPT_023);
					dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_NET_RPT_024);


					#endregion





					#region Title

					if (!string.IsNullOrWhiteSpace(objBomBoq.objAdAttribute.title))
					{
						dr = dtReport.NewRow();
						dr[0] = objBomBoq.objAdAttribute.title;
						dtReport.Rows.Add(dr);
						dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
						dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
					}

					#endregion

					#region ADD ROW DATA






					foreach (BOMBOQReport objBom in lstBOMReport)
					{
						var sts = objBom.NetworkStatus == "P" ? "Planned" : objBom.NetworkStatus == "A" ? "As-Built" : objBom.NetworkStatus == "D" ? "Dormant" : "Combined";

						/////////// Network Status
						if (NtStatus != objBom.NetworkStatus)
						{
							/////////// Add total Network Status wise
							if (NtStatus != "")
							{
								var q1 = lstBOMReport.Where(p => (p.is_header == true) && (p.NetworkStatus == NtStatus));
								dr = dtReport.NewRow();
								dr[Resources.Resources.SI_OSP_GBL_NET_GBL_008] = Resources.Resources.SI_OSP_GBL_GBL_GBL_041;
								dr[Resources.Resources.SI_OSP_GBL_NET_RPT_021] = q1.Sum(m => m.total_count).ToString();
								if (rptType.ToUpper() == "BOQ")
								{
									dr[string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, Settings.ApplicationSettings.Currency)] = q1.Sum(m => m.total_cost).ToString();
									dr[string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, Settings.ApplicationSettings.Currency)] = q1.Sum(m => m.service_cost_per_unit).ToString();
								}
								dr[Resources.Resources.SI_OSP_GBL_NET_RPT_023] = q1.Sum(m => m.calculated_length).ToString();
								dr[Resources.Resources.SI_OSP_GBL_NET_RPT_024] = q1.Sum(m => m.gis_length).ToString();
								dtReport.Rows.Add(dr);
							}
							dr = dtReport.NewRow();
							dr[Resources.Resources.SI_OSP_GBL_NET_GBL_008] = sts;
							NtStatus = objBom.NetworkStatus;
							dtReport.Rows.Add(dr);
						}

						///////////////// Check Cable count
						if (objBom.entity_type.ToUpper() == "CABLE")
						{
							CblCnt++;
						}

						//////// Main body row data
						dr = dtReport.NewRow();
						dr[Resources.Resources.SI_OSP_GBL_NET_GBL_008] = (objBom.is_header ? objBom.entity_type : objBom.entity_sub_type);
						dr[Resources.Resources.SI_OSP_GBL_GBL_GBL_068] = objBom.item_code;
						dr[Resources.Resources.SI_OSP_GBL_NET_RPT_015] = objBom.specification;
						//priyanka
						dr["Served By Ring"] = objBom.served_by_ring;

						if (rptType.ToUpper() == "BOQ")
						{
							dr[String.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_014.Replace("<br>", ""), Settings.ApplicationSettings.Currency)] = objBom.cost_per_unit.ToString();
							dr[Resources.Resources.SI_OSP_GBL_NET_RPT_017 + " " + string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_016, Settings.ApplicationSettings.Currency)] = objBom.service_cost_per_unit.ToString();
						}

						dr[Resources.Resources.SI_OSP_GBL_NET_GBL_226] = objBom.total_count.ToString();
						if (rptType.ToUpper() == "BOQ")
						{
							dr[string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, Settings.ApplicationSettings.Currency)] = objBom.total_cost.ToString();
							dr[string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, Settings.ApplicationSettings.Currency)] = objBom.total_service_cost.ToString();
						}
						dr[Resources.Resources.SI_OSP_GBL_NET_RPT_023] = objBom.geom_type.ToUpper() == "LINE" ? objBom.calculated_length.ToString() : "NA";
						dr[Resources.Resources.SI_OSP_GBL_NET_RPT_024] = objBom.geom_type.ToUpper() == "LINE" ? objBom.gis_length.ToString() : "NA";
						dtReport.Rows.Add(dr);

						LstCnt++;

						#region ADD FOOTER COLUMN
						if (LstCnt == lstBOMReport.Count)
						{
							var NtwkSts = objBomBoq.objReportFilters.SelectedNetworkStatues.ToUpper();
							if (NtwkSts == "P" || NtwkSts == "A" || NtwkSts == "D")
							{
								var q1 = lstBOMReport.Where(p => (p.is_header == true) && (p.NetworkStatus == NtStatus));
								dr = dtReport.NewRow();
								dr[1] = Resources.Resources.SI_OSP_GBL_GBL_GBL_041;
								dr[Resources.Resources.SI_OSP_GBL_NET_RPT_021] = q1.Sum(m => m.total_count).ToString();
								if (rptType.ToUpper() == "BOQ")
								{
									dr[string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, Settings.ApplicationSettings.Currency)] = q1.Sum(m => m.total_cost).ToString();
									dr[string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, Settings.ApplicationSettings.Currency)] = q1.Sum(m => m.service_cost_per_unit).ToString();
								}
								dr[Resources.Resources.SI_OSP_GBL_NET_RPT_023] = q1.Sum(m => m.calculated_length).ToString();
								dr[Resources.Resources.SI_OSP_GBL_NET_RPT_024] = q1.Sum(m => m.gis_length).ToString();
								dtReport.Rows.Add(dr);
							}


							if ((NtStatus == "C"))
							{
								dr = dtReport.NewRow();
								var q1 = lstBOMReport.Where(p => (p.is_header == true) && (p.NetworkStatus == NtStatus));
								dr[Resources.Resources.SI_OSP_GBL_NET_GBL_008] = Resources.Resources.SI_OSP_GBL_GBL_FRM_042;
								dr[Resources.Resources.SI_OSP_GBL_NET_RPT_021] = q1.Sum(m => m.total_count).ToString();
								if (rptType.ToUpper() == "BOQ")
								{
									dr[string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, Settings.ApplicationSettings.Currency)] = q1.Sum(m => m.total_cost).ToString();
									dr[string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, Settings.ApplicationSettings.Currency)] = q1.Sum(m => m.service_cost_per_unit).ToString();
								}
								dr[Resources.Resources.SI_OSP_GBL_NET_RPT_023] = q1.Sum(m => m.calculated_length).ToString();
								dr[Resources.Resources.SI_OSP_GBL_NET_RPT_024] = q1.Sum(m => m.gis_length).ToString();
								dtReport.Rows.Add(dr);
							}
						}
						#endregion

					}

					#endregion

					#region Loss Details

					if (isAttr.Contains("dBLOSS"))
					{
						if (objBomBoq.objReportFilters.isdBLossAttrEnable == true)
						{
							if (CblCnt > 0)
							{
								dr = dtReport.NewRow(); dtReport.Rows.Add(dr); dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
								dr = dtReport.NewRow(); dtReport.Rows.Add(dr); dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
								dr = dtReport.NewRow();

								dr[0] = "Loss Details";
								dtReport.Rows.Add(dr);
								dr = dtReport.NewRow();
								dr[0] = Resources.Resources.SI_OSP_GBL_GBL_FRM_044;
								dr[1] = Resources.Resources.SI_OSP_GBL_GBL_GBL_302 + "(Km)";
								dr[2] = Resources.Resources.SI_GBL_GBL_NET_FRM_100 + "(" + ApplicationSettings.dBLossUnit + ")";
								dr[3] = Resources.Resources.SI_OSP_GBL_GBL_FRM_212 + "(" + ApplicationSettings.dBLossUnit + ")";
								dr[4] = Resources.Resources.SI_OSP_GBL_GBL_FRM_213 + "(" + ApplicationSettings.dBLossUnit + ")";
								dtReport.Rows.Add(dr);
								if (objBomBoq.objdBLoss.Count > 0)
								{
									foreach (dBLossDetail objloss in objBomBoq.objdBLoss)
									{
										dr = dtReport.NewRow();
										dr[0] = objloss.cable_category;
										dr[1] = Convert.ToString(objloss.total_cable_length);
										dr[2] = Convert.ToString(objloss.cable_total_splice_loss);
										dr[3] = Convert.ToString(objloss.misc_loss);
										dr[4] = Convert.ToString(objloss.totalLoss);
										dtReport.Rows.Add(dr);
									}
									var ql = objBomBoq.objdBLoss;
									dr = dtReport.NewRow();
									dr[0] = Resources.Resources.SI_OSP_GBL_GBL_FRM_049 + "(" + ApplicationSettings.dBLossUnit + ")";
									dr[4] = ql.Sum(m => m.totalLoss).ToString();
									dtReport.Rows.Add(dr);
								}
								else
								{
									dr = dtReport.NewRow();
									dr[1] = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
									dtReport.Rows.Add(dr);
								}
							}
						}
					}

					#endregion

					#region  Signature section 

					if (isAttr.Contains("FTRD"))
					{
						if (objBomBoq.objAdAttribute.isAdditionalAttrEnable == true)
						{
							dr = dtReport.NewRow(); dtReport.Rows.Add(dr); dr = dtReport.NewRow(); dtReport.Rows.Add(dr); dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
							dr = dtReport.NewRow(); dtReport.Rows.Add(dr); dr = dtReport.NewRow(); dtReport.Rows.Add(dr);
							dr = dtReport.NewRow();
							dr[Resources.Resources.SI_OSP_GBL_NET_GBL_008] = Resources.Resources.SI_OSP_GBL_GBL_GBL_270 + ":-";
							dr[Resources.Resources.SI_OSP_GBL_NET_RPT_015] = Resources.Resources.SI_OSP_PM_NET_FRM_012 + ":-";
							dr[Resources.Resources.SI_OSP_GBL_NET_RPT_021] = Resources.Resources.SI_OSP_PM_NET_FRM_028 + ":-";
							dr[Resources.Resources.SI_OSP_GBL_NET_RPT_023] = Resources.Resources.SI_OSP_PM_NET_FRM_013 + ":-";
							dtReport.Rows.Add(dr);
							dr = dtReport.NewRow();
							dr[Resources.Resources.SI_OSP_GBL_NET_GBL_008] = objBomBoq.objAdAttribute.estimatedby;
							dr[Resources.Resources.SI_OSP_GBL_NET_RPT_015] = objBomBoq.objAdAttribute.checkedby;
							dr[Resources.Resources.SI_OSP_GBL_NET_RPT_021] = objBomBoq.objAdAttribute.re_checkedby;
							dr[Resources.Resources.SI_OSP_GBL_NET_RPT_023] = objBomBoq.objAdAttribute.approvedby;
							dtReport.Rows.Add(dr);
						}
					}

                    #endregion
                    
                    dtReport.TableName = rptType + Resources.Resources.SI_OSP_GBL_NET_FRM_218;
                    ds.Tables.Add(dtReport);

                    
                    #endregion

					if (dtReport != null && dtReport.Rows.Count > 0)
					{
						ExportData(ds, "ExportSummary_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}

		public System.Data.DataTable GetExportReportFilter(object obj)
		{
			System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
			System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
			System.Data.DataTable dt = new System.Data.DataTable(Resources.Resources.SI_GBL_GBL_NET_FRM_165);
			dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_166);
			dt.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_167);
			System.Data.DataRow dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_065; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_066; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_063; dt.Rows.Add(dr); dr = dt.NewRow();
			// dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_068; dt.Rows.Add(dr); dr = dt.NewRow();
			// dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_069; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_GBL_GBL_NET_FRM_098; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_GBL_GBL_GBL_GBL_147; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_FRM_030; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_074; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_GBL_GBL_076; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_010; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_GBL_011; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_071; dt.Rows.Add(dr); dr = dt.NewRow();
			dr[Resources.Resources.SI_GBL_GBL_NET_FRM_166] = Resources.Resources.SI_OSP_GBL_NET_FRM_072; dt.Rows.Add(dr); dr = dt.NewRow();


			List<int> regionIds = (List<int>)obj.GetType().GetProperty("SelectedRegionId").GetValue(obj, null);
			var regionName = regionIds == null ? "All" : string.Join(",", new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) }).Where(x => regionIds.Contains(x.regionId)).Select(x => x.regionName).ToList());

			List<int> provinceIds = (List<int>)obj.GetType().GetProperty("SelectedProvinceId").GetValue(obj, null);
			var provinceName = provinceIds == null ? "All" : string.Join(",", new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = string.Join(",", regionIds), userId = Convert.ToInt32(Session["user_id"]) }).Where(x => provinceIds.Contains(x.provinceId)).Select(x => x.provinceName).ToList());

			string networkStatus = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedNetworkStatues").GetValue(obj, null).ToString().Replace("AS BUILT", "AS-BUILT").ToLower()).Replace("'", "");

			string ownershipType = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedOwnerShipType").GetValue(obj, null).ToString()).Replace("'", "");
			ownershipType = string.IsNullOrEmpty(ownershipType) ? "All" : ownershipType;

			List<int> thirdPartyVendorId = (List<int>)obj.GetType().GetProperty("SelectedThirdPartyVendorId").GetValue(obj, null);
			var thirdPartyVendorName = thirdPartyVendorId == null ? "All" : string.Join(",", BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList().Where(x => thirdPartyVendorId.Contains(Convert.ToInt32(x.key))).Select(x => x.value).ToList());

			//List<int> parentUser = new List<int>();
			//parentUser.Add(1);
			//List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
			//var parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

			//List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
			//var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());


			var userdetails = (User)Session["userDetail"];
			List<int> layerIds = (List<int>)obj.GetType().GetProperty("SelectedLayerId").GetValue(obj, null);
			var layerName = layerIds == null ? "All" : string.Join(",", new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY").Where(x => layerIds.Contains(x.layer_id)).Select(x => x.layer_title).ToList());

			List<int> projectCodeIds = (List<int>)obj.GetType().GetProperty("SelectedProjectId").GetValue(obj, null);
			var projectCodeName = projectCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrEmpty(networkStatus) ? "P" : networkStatus.ToUpper() == "PLANNED" ? "P" : networkStatus.ToUpper() == "AS BUILT" ? "A" : networkStatus.ToUpper() == "DORMANT" ? "D" : "P").Where(x => projectCodeIds.Contains(x.system_id)).Select(x => x.project_code).ToList());

			List<int> planningCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPlanningId").GetValue(obj, null);
			var planningCodeName = planningCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(projectCodeIds).Where(x => planningCodeIds.Contains(x.system_id)).Select(x => x.planning_code).ToList());

			List<int> workOrderCodeIds = (List<int>)obj.GetType().GetProperty("SelectedWorkOrderId").GetValue(obj, null);
			var workOrderCodeName = workOrderCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(planningCodeIds).Where(x => workOrderCodeIds.Contains(x.system_id)).Select(x => x.workorder_code).ToList());

			List<int> purposeCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPurposeId").GetValue(obj, null);
			var purposeCodeName = purposeCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(workOrderCodeIds).Where(x => purposeCodeIds.Contains(x.system_id)).Select(x => x.purpose_code).ToList());

			string durationBasedOn = obj.GetType().GetProperty("durationbasedon").GetValue(obj, null).ToString().Replace("_", " ");

			string duration = obj.GetType().GetProperty("fromDate").GetValue(obj, null).ToString() + " To " + obj.GetType().GetProperty("toDate").GetValue(obj, null).ToString();


			dt.Rows[0][1] = regionName;
			dt.Rows[1][1] = provinceName;
			dt.Rows[2][1] = String.IsNullOrEmpty(networkStatus) ? "All" : networkStatus;
			// dt.Rows[3][1] = parentUserName;
			// dt.Rows[4][1] = userName;
			dt.Rows[5][1] = layerName;

			dt.Rows[6][1] = ownershipType;
			dt.Rows[7][1] = thirdPartyVendorName;

			dt.Rows[8][1] = projectCodeName;
			dt.Rows[9][1] = planningCodeName;
			dt.Rows[10][1] = workOrderCodeName;
			dt.Rows[11][1] = purposeCodeName;
			dt.Rows[12][1] = durationBasedOn;
			dt.Rows[13][1] = duration;
			return dt;
		}

		private void ExportData(System.Data.DataSet dsReport, string fileName, bool isDataContainBarcode = false)
		{
			using (var exportData = new System.IO.MemoryStream())
			{
				Response.Clear();
				if (dsReport != null && dsReport.Tables.Count > 0)
				{
					NPOI.SS.UserModel.IWorkbook workbook = NPOIExcelHelper.DatasetToExcel("xlsx", dsReport, isDataContainBarcode);
					workbook.Write(exportData);
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
					Response.BinaryWrite(exportData.ToArray());
					Response.End();
				}
				Response.End();
			}
		}




		public IWorkbook GetBOMBOQWorkBook(BomBoqExportReport objBomBoq, string sheetName, string rptType, string MainReport)
		{
			List<BOMBOQReport> lstBomReport = objBomBoq.BomBoqReportList;
			var isAttr = ((List<string>)Session["ApplicableModuleList"]);
			System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
			IWorkbook workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();

			System.Data.DataTable dtFilters = new System.Data.DataTable();
			System.Data.DataRow drt;
			System.Data.DataSet dsReport = new System.Data.DataSet();

			#region Filer Details


			object obj = objBomBoq.objReportFilters;
			var userdetails = (User)Session["userDetail"];            
            System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
			System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;

			dtFilters.TableName = "Filters";
			dtFilters.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_166);
			dtFilters.Columns.Add(Resources.Resources.SI_GBL_GBL_NET_FRM_167);
			drt = dtFilters.NewRow();
			drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_275;
			dtFilters.Rows.Add(drt);

			drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_065;
			List<int> regionIds = (List<int>)obj.GetType().GetProperty("SelectedRegionId").GetValue(obj, null);
			var regionName = regionIds == null ? "All" : string.Join(",", new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(userdetails.user_id) }).Where(x => regionIds.Contains(x.regionId)).Select(x => x.regionName).ToList());
			drt[1] = regionName; dtFilters.Rows.Add(drt);

			drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_066;
			List<int> provinceIds = (List<int>)obj.GetType().GetProperty("SelectedProvinceId").GetValue(obj, null);
			var provinceName = provinceIds == null ? "All" : string.Join(",", new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = string.Join(",", regionIds), userId = Convert.ToInt32(userdetails.user_id) }).Where(x => provinceIds.Contains(x.provinceId)).Select(x => x.provinceName).ToList());
			drt[1] = provinceName; dtFilters.Rows.Add(drt);

			drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_063;
			List<string> NWStatus = (List<string>)obj.GetType().GetProperty("SelectedNetworkStatus").GetValue(obj, null);
			string networkStatus = NWStatus == null ? "All" : (string.Join(", ", NWStatus.Select(i => (i.ToUpper() == "P" ? "Planned" : i.ToUpper() == "A" ? "As-Built" : i.ToUpper() == "D" ? "Dormant" : ""))));
			drt[1] = networkStatus; dtFilters.Rows.Add(drt);

			drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_274;
			List<int> parentUser = new List<int>();
			List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
			var parentUserName = string.Empty;
			if (userdetails.role_id == 1)
			{
				parentUser.Add(1);
				parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());
			}
			else
			{
				parentUserName = parentUserIds == null ? "All" : new BLUser().GetUserDetailByID(userdetails.user_id).user_name;
			}
			drt[1] = parentUserName; dtFilters.Rows.Add(drt);

			drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_RPT_040;
			List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
			var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());
			drt[1] = userName; dtFilters.Rows.Add(drt);

			drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_GBL_GBL_NET_FRM_098;
			List<int> layerIds = (List<int>)obj.GetType().GetProperty("SelectedLayerId").GetValue(obj, null);
			var layerName = layerIds == null ? "All" : string.Join(",", new BLLayer().GetReportLayers(userdetails.role_id, "ENTITY").Where(x => layerIds.Contains(x.layer_id)).Select(x => x.layer_title).ToList());
			drt[1] = layerName; dtFilters.Rows.Add(drt);

			drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_GBL_GBL_GBL_GBL_147;
			string ownershipType = textInfo.ToTitleCase(obj.GetType().GetProperty("SelectedOwnerShipType").GetValue(obj, null).ToString()).Replace("'", "");
			ownershipType = string.IsNullOrEmpty(ownershipType) ? "All" : ownershipType;
			drt[1] = ownershipType; dtFilters.Rows.Add(drt);

			drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_FRM_030;
			List<int> thirdPartyVendorId = (List<int>)obj.GetType().GetProperty("SelectedThirdPartyVendorId").GetValue(obj, null);
			var thirdPartyVendorName = thirdPartyVendorId == null ? "All" : string.Join(",", BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList().Where(x => thirdPartyVendorId.Contains(Convert.ToInt32(x.key))).Select(x => x.value).ToList());
			drt[1] = thirdPartyVendorName; dtFilters.Rows.Add(drt);
			//List<int> parentUser = new List<int>();
			//parentUser.Add(1);
			//List<int> parentUserIds = (List<int>)obj.GetType().GetProperty("SelectedParentUser").GetValue(obj, null);
			//var parentUserName = parentUserIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUser).Where(x => parentUserIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

			//List<int> userIds = (List<int>)obj.GetType().GetProperty("SelectedUserId").GetValue(obj, null);
			//var userName = userIds == null ? "All" : string.Join(",", new BLUser().GetUsersListByMGRIds(parentUserIds).Where(x => userIds.Contains(x.user_id)).Select(x => x.user_name).ToList());

			if (isAttr.Contains("PROJ"))
			{
				drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_074;
				List<int> projectCodeIds = (List<int>)obj.GetType().GetProperty("SelectedProjectId").GetValue(obj, null);
				var projectCodeName = projectCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrEmpty(networkStatus) ? "P" : networkStatus.ToUpper() == "PLANNED" ? "P" : networkStatus.ToUpper() == "AS BUILT" ? "A" : networkStatus.ToUpper() == "DORMANT" ? "D" : "P").Where(x => projectCodeIds.Contains(x.system_id)).Select(x => x.project_code).ToList());
				drt[1] = projectCodeName; dtFilters.Rows.Add(drt);

				drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_GBL_GBL_076;
				List<int> planningCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPlanningId").GetValue(obj, null);
				var planningCodeName = planningCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(projectCodeIds).Where(x => planningCodeIds.Contains(x.system_id)).Select(x => x.planning_code).ToList());
				drt[1] = planningCodeName; dtFilters.Rows.Add(drt);

				drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_009;
				List<int> workOrderCodeIds = (List<int>)obj.GetType().GetProperty("SelectedWorkOrderId").GetValue(obj, null);
				var workOrderCodeName = workOrderCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(planningCodeIds).Where(x => workOrderCodeIds.Contains(x.system_id)).Select(x => x.workorder_code).ToList());
				drt[1] = workOrderCodeName; dtFilters.Rows.Add(drt);

				drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_GBL_011;
				List<int> purposeCodeIds = (List<int>)obj.GetType().GetProperty("SelectedPurposeId").GetValue(obj, null);
				var purposeCodeName = purposeCodeIds == null ? "All" : string.Join(",", new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(workOrderCodeIds).Where(x => purposeCodeIds.Contains(x.system_id)).Select(x => x.purpose_code).ToList());
				drt[1] = purposeCodeName; dtFilters.Rows.Add(drt);
			}
			drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_071;
			string durationBasedOn = obj.GetType().GetProperty("durationbasedon").GetValue(obj, null).ToString().Replace("_", " ");
			drt[1] = durationBasedOn; dtFilters.Rows.Add(drt);

            drt = dtFilters.NewRow(); drt[0] = Resources.Resources.SI_OSP_GBL_NET_FRM_072;
            string duration = "";
            if (obj.GetType().GetProperty("fromDate").GetValue(obj, null) != null && obj.GetType().GetProperty("toDate").GetValue(obj, null) != null)
            {
                duration = obj.GetType().GetProperty("fromDate").GetValue(obj, null).ToString() + " To " + obj.GetType().GetProperty("toDate").GetValue(obj, null).ToString();
            }
            else
            {
                duration = "All";
            }
            drt[1] = duration; dtFilters.Rows.Add(drt);
			if (isAttr.Contains("PROJ"))
			{
                drt = dtFilters.NewRow(); dtFilters.Rows.Add(drt);
                drt = dtFilters.NewRow(); dtFilters.Rows.Add(drt);
                drt = dtFilters.NewRow(); dtFilters.Rows.Add(drt);
            }
            dtFilters.AcceptChanges();
            //Priyanka 
            //dtFilters = Utility.CommonUtility.GetFormattedDataTable(dtFilters, ApplicationSettings.numberFormatType);
            dsReport.Tables.Add(dtFilters);

            
            #endregion




            workbook = NPOIExcelHelper.DatasetToExcel("xlsx", dsReport);
            ICellStyle headerStyle = NPOIExcelHelper.getCellStyle(workbook, "HEADER");
            ICellStyle SubheaderStyle = NPOIExcelHelper.getCellStyle(workbook, "SUB_HEADER");
            ICellStyle mainheaderStyle = getCellStyleOther(wb: workbook, styleType: "MAIN_HEADER", FntSize: 14, TextAlignment: "CENTER");
            ICellStyle TitleStyle = getCellStyleOther(wb: workbook, styleType: "TITLE", iSBorderOnCell: false, FntSize: 16, TextAlignment: "CENTER");
            ICellStyle SubheadStyle = getCellStyleOther(wb: workbook, styleType: "SUB_HEADER_1", FntSize: 10);
            var cellstyleN = NPOIExcelHelper.getCellStyle(workbook, "");
            var cellstyleWbBorder = getCellStyleOther(wb: workbook, iSBorderOnCell: false);
            int rowCount = 0, colCount = 0;
            ICellStyle customStyle = workbook.CreateCellStyle();
			if(ApplicationSettings.numberFormatType.ToLower()=="saarc")
				customStyle.DataFormat = workbook.CreateDataFormat().GetFormat(ApplicationSettings.saarcExcelNumberFormat);
			else if (ApplicationSettings.numberFormatType.ToLower() == "europe")
				customStyle.DataFormat = workbook.CreateDataFormat().GetFormat(ApplicationSettings.europeExcelNumberFormat);
			//for decimal value
            ICellStyle customStyleDecimal = workbook.CreateCellStyle();
            if (ApplicationSettings.numberFormatType.ToLower() == "saarc")
                customStyleDecimal.DataFormat = workbook.CreateDataFormat().GetFormat(ApplicationSettings.saarcExcelNumberFormatDecimal);
            else if (ApplicationSettings.numberFormatType.ToLower() == "europe")
                customStyleDecimal.DataFormat = workbook.CreateDataFormat().GetFormat(ApplicationSettings.europeExcelNumberFormatDecimal);
            if (MainReport == "BOMBOQ")
			{
				ISheet bomSheet = workbook.CreateSheet(sheetName);
				int CblCnt = 0, LstCnt = 0, PL = 0, AsB = 0, Dmt = 0;
				var NtStatus = "";
				int lstCmn = (rptType.ToUpper() == "BOQ" ? 11 : 7),
					   FstCl = 0;

				#region Logo
				IRow rowLogo = bomSheet.CreateRow(rowCount);
				rowLogo.HeightInPoints = 40;

				if (!string.IsNullOrEmpty(ApplicationSettings.ClientLogoImageBytesForWeb))
				{
					//NPOIExcelHelper.CreateCustomCell(rowLogo, FstCl, "", cellstyleN);
					string[] arr = ApplicationSettings.ClientLogoImageBytesForWeb.Split(',');
					byte[] imageBytes = Convert.FromBase64String(arr[1]);
					int pictureIdx = workbook.AddPicture(imageBytes, PictureType.JPEG);
					ICreationHelper helper = workbook.GetCreationHelper();
					IDrawing drawing = bomSheet.CreateDrawingPatriarch();
					// add a picture shape
					IClientAnchor anchor = helper.CreateClientAnchor();
					//subsequent call of Picture#resize() will operate relative to it
					anchor.Col1 = 0;
					anchor.Row1 = 0;
					IPicture pict = drawing.CreatePicture(anchor, pictureIdx);
					//auto-size picture relative to its top-left corner
					anchor.Dx2 = 15;
					pict.Resize(1.001, 0.780);
				}
				if (ApplicationSettings.isClientNameRequiredOnLoginPage)
				{
					NPOIExcelHelper.CreateCustomCell(rowLogo, FstCl + 1, ApplicationSettings.ClientName, getCellStyleOther(wb: workbook, FntSize: 12, iSBorderOnCell: false, iSBorderSpecific: "BOTTOM"));
				}
				NPOIExcelHelper.CreateCustomCell(rowLogo, lstCmn, DateTimeHelper.DateTimeFormatWithTime(DateTimeHelper.Now.ToString()), getCellStyleOther(wb: workbook, FntSize: 10, iSBorderOnCell: false, iSBorderSpecific: "BOTTOM"));
				rowLogo.RowStyle = getCellStyleOther(wb: workbook, iSBorderOnCell: false, iSBorderSpecific: "BOTTOM");
				rowCount = 2;
				#endregion

				#region Title

				if (!string.IsNullOrWhiteSpace(objBomBoq.objAdAttribute.title))
				{
					IRow rowTi = bomSheet.CreateRow(rowCount);
					rowTi.HeightInPoints = 50;
					bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, FstCl, lstCmn));
					NPOIExcelHelper.CreateCustomCell(rowTi, FstCl, objBomBoq.objAdAttribute.title, TitleStyle);
					rowCount++;
				}

				#endregion

				#region Equipment Details
				if (isAttr.Contains("EQPD"))
				{
					if (objBomBoq.objAdAttribute.isEquipmentEnable == true)
					{
						rowCount++;
						IRow rowEq1 = bomSheet.CreateRow(rowCount);
						// bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, (rptType.ToUpper() == "BOQ" ? 10 : 6)));
						bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 1));
						NPOIExcelHelper.CreateCustomCell(rowEq1, 0, "Equipments" + Resources.Resources.SI_OSP_GBL_NET_FRM_218, mainheaderStyle);
						rowCount++;
						IRow rowEq2 = bomSheet.CreateRow(rowCount);
						NPOIExcelHelper.CreateCustomCell(rowEq2, 0, "Equipments", headerStyle);
						NPOIExcelHelper.CreateCustomCell(rowEq2, 1, Resources.Resources.SI_GBL_GBL_NET_FRM_167, headerStyle);
						rowCount++;
						IRow rowEq4 = bomSheet.CreateRow(rowCount);
						NPOIExcelHelper.CreateCustomCell(rowEq4, 0, Resources.Resources.SI_ISP_GBL_NET_FRM_063, cellstyleN);
						NPOIExcelHelper.CreateCustomCell(rowEq4, 1, objBomBoq.objAdAttribute.equipmenttype, cellstyleN);
						rowCount++;
						IRow rowEq5 = bomSheet.CreateRow(rowCount);
						NPOIExcelHelper.CreateCustomCell(rowEq5, 0, Resources.Resources.SI_OSP_GBL_GBL_GBL_118, cellstyleN);
						NPOIExcelHelper.CreateCustomCell(rowEq5, 1, objBomBoq.objAdAttribute.equipmentname, cellstyleN);
						rowCount++;
						IRow rowEq6 = bomSheet.CreateRow(rowCount);
						NPOIExcelHelper.CreateCustomCell(rowEq6, 0, Resources.Resources.SI_OSP_GBL_NET_GBL_139, cellstyleN);
						NPOIExcelHelper.CreateCustomCell(rowEq6, 1, objBomBoq.objAdAttribute.popname, cellstyleN);
						rowCount++;
					}
				}

				#endregion

				#region ADD HEADER COLUMNS

				rowCount++;
				IRow rowHd1 = bomSheet.CreateRow(rowCount);
				bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, (rptType.ToUpper() == "BOQ" ? 11 : 7)));
				NPOIExcelHelper.CreateCustomCell(rowHd1, 0, rptType + Resources.Resources.SI_OSP_GBL_NET_FRM_218, mainheaderStyle);
				rowCount++;
				IRow rowHd2 = bomSheet.CreateRow(rowCount);
				NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, Resources.Resources.SI_OSP_GBL_NET_GBL_008, headerStyle); colCount++;
				NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, Resources.Resources.SI_OSP_GBL_GBL_GBL_077, headerStyle); colCount++;
				NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, Resources.Resources.SI_OSP_GBL_GBL_GBL_068, headerStyle); colCount++;
				NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, Resources.Resources.SI_OSP_GBL_NET_RPT_015, headerStyle); colCount++;
				//priyanka
				NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, "Served By Ring", headerStyle); colCount++;
				if (rptType.ToUpper() == "BOQ")
				{
					NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, String.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_014.Replace("<br>", ""), ApplicationSettings.Currency), headerStyle); colCount++;
                    NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, String.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_017,ApplicationSettings.Currency), headerStyle); colCount++;
                }
				NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, Resources.Resources.SI_OSP_GBL_NET_GBL_226, headerStyle); colCount++;

				if (rptType.ToUpper() == "BOQ")
				{
					NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, string.Format(Resources.Resources.SI_OSP_ROW_NET_FRM_085, ApplicationSettings.Currency), headerStyle); colCount++;
					NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, string.Format(Resources.Resources.SI_OSP_GBL_NET_RPT_022, ApplicationSettings.Currency), headerStyle); colCount++;
				}
				NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, string.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_476.Replace("<br/>", "")), headerStyle); colCount++;

				NPOIExcelHelper.CreateCustomCell(rowHd2, colCount, Resources.Resources.SI_OSP_GBL_NET_FRM_477, headerStyle); colCount++;

				#endregion

				#region ADD ROW DATA

				foreach (BOMBOQReport objBom in lstBomReport)
				{

					var sts = objBom.NetworkStatus == "P" ? "Planned" : objBom.NetworkStatus == "A" ? "As-Built" : objBom.NetworkStatus == "D" ? "Dormant" : "Combined";

                    /////////// Network Status
                    if (NtStatus != objBom.NetworkStatus)
                    {
                        /////////// Add total Network Status wise
                        if (NtStatus != "")
                        {
                            rowCount++; colCount = 0;
                            var q1 = lstBomReport.Where(p => (p.is_header == true) && (p.NetworkStatus == NtStatus));
                            IRow newRowTo = bomSheet.CreateRow(rowCount);
                            bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, (rptType.ToUpper() == "BOQ" ? 6 : 4)));
                            NPOIExcelHelper.CreateCustomCell(newRowTo, colCount, Resources.Resources.SI_OSP_GBL_GBL_GBL_041, headerStyle, false, customStyle);
                            //boq right
                            colCount = (rptType.ToUpper() == "BOQ" ? 7 :5);
                            NPOIExcelHelper.CreateCustomCell(newRowTo, colCount, q1.Sum(m => m.total_count).ToString(), headerStyle, false, customStyle); colCount++;
                            if (rptType.ToUpper() == "BOQ")
                            {
                                NPOIExcelHelper.CreateCustomCell(newRowTo, colCount, q1.Sum(m => m.total_cost).ToString(), headerStyle, false, customStyle); colCount++;
                                NPOIExcelHelper.CreateCustomCell(newRowTo, colCount, q1.Sum(m => m.total_service_cost).ToString(), headerStyle, false, customStyle); colCount++;
                            }
                            NPOIExcelHelper.CreateCustomCell(newRowTo, colCount, q1.Sum(m => m.calculated_length).ToString(), headerStyle, false, customStyleDecimal); colCount++;
                            NPOIExcelHelper.CreateCustomCell(newRowTo, colCount, q1.Sum(m => m.gis_length).ToString(), headerStyle, false, customStyleDecimal); colCount++;

						}


                        PL += (objBom.NetworkStatus.ToUpper() == "P" ? 1 : 0);
                        AsB += (objBom.NetworkStatus.ToUpper() == "A" ? 1 : 0);
                        if (ApplicationSettings.IsDormantEnabled)
                        {
                            Dmt = Dmt + (@objBom.NetworkStatus.ToUpper() == "D" ? 1 : 0);
                        }
                        else { Dmt = Dmt = 1; }
                        if (objBom.NetworkStatus.ToUpper() == "C")
                        {
                            if (PL == 0)
                            {
                                rowCount++;
                                IRow newRowToPN = bomSheet.CreateRow(rowCount);
                                bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, lstCmn));
                                NPOIExcelHelper.CreateCustomCell(newRowToPN, 0, "Planned", SubheadStyle, false, customStyle);

                                rowCount++;
                                IRow newRowToPNT = bomSheet.CreateRow(rowCount);
                                bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, lstCmn));
                                NPOIExcelHelper.CreateCustomCell(newRowToPNT, 0, "              " + Resources.Resources.SI_OSP_GBL_GBL_GBL_051, cellstyleN, false, customStyle);

                            }
                            if (AsB == 0)
                            {
                                rowCount++;
                                IRow newRowToPN = bomSheet.CreateRow(rowCount);
                                bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, lstCmn));
                                NPOIExcelHelper.CreateCustomCell(newRowToPN, 0, "As-Built", SubheadStyle, false, customStyle);

                                rowCount++;
                                IRow newRowToPNT = bomSheet.CreateRow(rowCount);
                                bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, lstCmn));
                                NPOIExcelHelper.CreateCustomCell(newRowToPNT, 0, "              " + Resources.Resources.SI_OSP_GBL_GBL_GBL_051, cellstyleN, false, customStyle);

                            }
                            if (Dmt == 0)
                            {
                                rowCount++;
                                IRow newRowToPN = bomSheet.CreateRow(rowCount);
                                bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, lstCmn));
                                NPOIExcelHelper.CreateCustomCell(newRowToPN, 0, "Dormant", SubheadStyle, false, customStyle);

                                rowCount++;
                                IRow newRowToPNT = bomSheet.CreateRow(rowCount);
                                bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, lstCmn));
                                NPOIExcelHelper.CreateCustomCell(newRowToPNT, 0, "              " + Resources.Resources.SI_OSP_GBL_GBL_GBL_051, cellstyleN, false, customStyle);

							}
						}


                        rowCount++; colCount = 0;
                        IRow newRowM = bomSheet.CreateRow(rowCount);
   
                        bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, (rptType.ToUpper() == "BOQ" ? 11 : 7)));
                        NPOIExcelHelper.CreateCustomCell(newRowM, 0, sts, SubheadStyle, false, customStyle);
                        
                        NtStatus = objBom.NetworkStatus;
                    }





                    ///////////////// Check Cable count
                    if (objBom.entity_type.ToUpper() == "CABLE")
                    {
                        CblCnt++;
                    }
                    rowCount++; colCount = 0;
                    //////// Main body row data
                    IRow newRowData = bomSheet.CreateRow(rowCount);
                    var cellstyle = NPOIExcelHelper.getCellStyle(workbook, objBom.is_header ? "SUB_HEADER" : "");
                    NPOIExcelHelper.CreateCustomCell(newRowData, colCount, (objBom.is_header ? objBom.entity_type : objBom.entity_sub_type), cellstyle, false, customStyle); colCount++;
                    NPOIExcelHelper.CreateCustomCell(newRowData, colCount, objBom.name, cellstyle, false, customStyle); colCount++;
                    NPOIExcelHelper.CreateCustomCell(newRowData, colCount, objBom.item_code, cellstyle, false, customStyle); colCount++;
                    NPOIExcelHelper.CreateCustomCell(newRowData, colCount, objBom.specification, cellstyle, false, customStyle); colCount++;
                    NPOIExcelHelper.CreateCustomCell(newRowData, colCount, objBom.served_by_ring, cellstyle, false, customStyle); colCount++;
                    if (rptType.ToUpper() == "BOQ")
                    {
                        NPOIExcelHelper.CreateCustomCell(newRowData, colCount, objBom.cost_per_unit.ToString(), cellstyle, false, customStyle); colCount++;
                        NPOIExcelHelper.CreateCustomCell(newRowData, colCount, objBom.service_cost_per_unit.ToString(), cellstyle, false, customStyle); colCount++;
                    }
                    NPOIExcelHelper.CreateCustomCell(newRowData, colCount, objBom.total_count.ToString(), cellstyle, false, customStyle); colCount++;

                    if (rptType.ToUpper() == "BOQ")
                    {
                        NPOIExcelHelper.CreateCustomCell(newRowData, colCount, objBom.total_cost.ToString(), cellstyle, false, customStyle); colCount++;
                        NPOIExcelHelper.CreateCustomCell(newRowData, colCount, objBom.total_service_cost.ToString(), cellstyle, false, customStyle); colCount++;
                    }
                    //NPOIExcelHelper.CreateCustomCell(newRowData, colCount, (objBom.geom_type.ToUpper() == "LINE" ? objBom.calculated_length.ToString() : "NA"), cellstyle); colCount++;
                    //NPOIExcelHelper.CreateCustomCell(newRowData, colCount, (objBom.geom_type.ToUpper() == "LINE" ? objBom.gis_length.ToString() : "NA"), cellstyle); colCount++;

                    NPOIExcelHelper.CreateCustomCell(newRowData, colCount, (objBom.geom_type.ToUpper() == "LINE" ? objBom.calculated_length.ToString() : ""), cellstyle,false, customStyleDecimal); colCount++;
                    NPOIExcelHelper.CreateCustomCell(newRowData, colCount, (objBom.geom_type.ToUpper() == "LINE" ? objBom.gis_length.ToString() : ""), cellstyle,false, customStyleDecimal); colCount++;


                    LstCnt++;
                    #region ADD FOOTER COLUMN
                    if (LstCnt == lstBomReport.Count)
                    {
                        var NtwkSts = objBomBoq.objReportFilters.SelectedNetworkStatues.ToUpper();
                        if (NtwkSts == "P" || NtwkSts == "A" || NtwkSts == "D")
                        {
                            rowCount++; colCount = 0;
                            IRow newRowFtr = bomSheet.CreateRow(rowCount);
                            var q1 = lstBomReport.Where(p => (p.is_header == true) && (p.NetworkStatus == NtStatus));
                            bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, (rptType.ToUpper() == "BOQ" ? 6 : 4)));
                            NPOIExcelHelper.CreateCustomCell(newRowFtr, colCount, Resources.Resources.SI_OSP_GBL_GBL_GBL_041, headerStyle, false, customStyle);
                            colCount = (rptType.ToUpper() == "BOQ" ? 6 : 5);
                            NPOIExcelHelper.CreateCustomCell(newRowFtr, colCount, q1.Sum(m => m.total_count).ToString(), headerStyle, false, customStyle); colCount++;
                            if (rptType.ToUpper() == "BOQ")
                            {
                                NPOIExcelHelper.CreateCustomCell(newRowFtr, colCount, q1.Sum(m => m.total_cost).ToString(), headerStyle, false, customStyle); colCount++;
                                NPOIExcelHelper.CreateCustomCell(newRowFtr, colCount, q1.Sum(m => m.service_cost_per_unit).ToString(), headerStyle, false, customStyle); colCount++;
                            }
                            NPOIExcelHelper.CreateCustomCell(newRowFtr, colCount, q1.Sum(m => m.calculated_length).ToString(), headerStyle, false, customStyleDecimal); colCount++;
                            NPOIExcelHelper.CreateCustomCell(newRowFtr, colCount, q1.Sum(m => m.gis_length).ToString(), headerStyle, false, customStyleDecimal);

						}


                        if ((NtStatus == "C"))
                        {
                            rowCount++; colCount = 0;
                            IRow newRowCom = bomSheet.CreateRow(rowCount);
                            bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, (rptType.ToUpper() == "BOQ" ? 6 : 4)));
                            var q1 = lstBomReport.Where(p => (p.is_header == true) && (p.NetworkStatus == NtStatus));
                            NPOIExcelHelper.CreateCustomCell(newRowCom, colCount, Resources.Resources.SI_OSP_GBL_GBL_GBL_041, headerStyle, false, customStyle);
                            //boq right
                                colCount = (rptType.ToUpper() == "BOQ" ? 7 : 5);
                            NPOIExcelHelper.CreateCustomCell(newRowCom, colCount, q1.Sum(m => m.total_count).ToString(), headerStyle, false, customStyle); colCount++;
                            if (rptType.ToUpper() == "BOQ")
                            {
                                NPOIExcelHelper.CreateCustomCell(newRowCom, colCount, q1.Sum(m => m.total_cost).ToString(), headerStyle, false, customStyle); colCount++;
                                NPOIExcelHelper.CreateCustomCell(newRowCom, colCount, q1.Sum(m => m.service_cost_per_unit).ToString(), headerStyle, false, customStyle); colCount++;
                            }
                            NPOIExcelHelper.CreateCustomCell(newRowCom, colCount, q1.Sum(m => m.calculated_length).ToString(), headerStyle, false, customStyleDecimal); colCount++;
                            NPOIExcelHelper.CreateCustomCell(newRowCom, colCount, q1.Sum(m => m.gis_length).ToString(), headerStyle, false, customStyleDecimal);

						}
					}
					#endregion

				}


				#endregion

				#region Loss Details

                if (isAttr.Contains("dBLOSS"))
                {
                    if (objBomBoq.objReportFilters.isdBLossAttrEnable == true)
                    {
                        if (CblCnt > 0)
                        {
                            rowCount += 3; colCount = 0;
                            IRow newRowLos1 = bomSheet.CreateRow(rowCount);
                            bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 4));
                            NPOIExcelHelper.CreateCustomCell(newRowLos1, 0, Resources.Resources.SI_OSP_GBL_GBL_FRM_043, mainheaderStyle, false, customStyle);
                            rowCount++;
                            IRow newRowLos2 = bomSheet.CreateRow(rowCount);
                            NPOIExcelHelper.CreateCustomCell(newRowLos2, colCount, Resources.Resources.SI_OSP_GBL_GBL_FRM_044, headerStyle, false, customStyle); colCount++;
                            NPOIExcelHelper.CreateCustomCell(newRowLos2, colCount, Resources.Resources.SI_OSP_GBL_GBL_GBL_302 + "(Km)", headerStyle, false, customStyle); colCount++;
                            NPOIExcelHelper.CreateCustomCell(newRowLos2, colCount, Resources.Resources.SI_GBL_GBL_NET_FRM_100 + "(" + ApplicationSettings.dBLossUnit + ")", headerStyle, false, customStyle); colCount++;
                            NPOIExcelHelper.CreateCustomCell(newRowLos2, colCount, Resources.Resources.SI_OSP_GBL_GBL_FRM_212 + "(" + ApplicationSettings.dBLossUnit + ")", headerStyle, false, customStyle); colCount++;
                            NPOIExcelHelper.CreateCustomCell(newRowLos2, colCount, Resources.Resources.SI_OSP_GBL_GBL_FRM_213 + "(" + ApplicationSettings.dBLossUnit + ")", headerStyle, false, customStyle); colCount++;

                            if (objBomBoq.objdBLoss.Count > 0)
                            {
                                foreach (dBLossDetail objloss in objBomBoq.objdBLoss)
                                {
                                    rowCount++; colCount = 0;
                                    IRow newRowLos3 = bomSheet.CreateRow(rowCount);
                                    NPOIExcelHelper.CreateCustomCell(newRowLos3, colCount, objloss.cable_category, cellstyleN, false, customStyle); colCount++;
                                    NPOIExcelHelper.CreateCustomCell(newRowLos3, colCount, Convert.ToString(objloss.total_cable_length), cellstyleN, false, customStyle); colCount++;
                                    NPOIExcelHelper.CreateCustomCell(newRowLos3, colCount, Convert.ToString(objloss.cable_total_splice_loss), cellstyleN, false, customStyle); colCount++;
                                    NPOIExcelHelper.CreateCustomCell(newRowLos3, colCount, Convert.ToString(objloss.misc_loss), cellstyleN, false, customStyle); colCount++;
                                    NPOIExcelHelper.CreateCustomCell(newRowLos3, colCount, Convert.ToString(objloss.totalLoss), cellstyleN, false, customStyle); colCount++;
                                }
                                rowCount++; colCount = 0;
                                IRow newRowLos4 = bomSheet.CreateRow(rowCount);
                                bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 3));
                                var ql = objBomBoq.objdBLoss;
                                NPOIExcelHelper.CreateCustomCell(newRowLos4, 0, Resources.Resources.SI_OSP_GBL_GBL_FRM_049 + "(" + ApplicationSettings.dBLossUnit + ")", headerStyle, false, customStyle);
                                NPOIExcelHelper.CreateCustomCell(newRowLos4, 0, Resources.Resources.SI_OSP_GBL_GBL_FRM_049 + "(" + ApplicationSettings.dBLossUnit + ")", headerStyle, false, customStyle);
                                NPOIExcelHelper.CreateCustomCell(newRowLos4, 0, Resources.Resources.SI_OSP_GBL_GBL_FRM_049 + "(" + ApplicationSettings.dBLossUnit + ")", headerStyle, false, customStyle);
                                NPOIExcelHelper.CreateCustomCell(newRowLos4, 4, ql.Sum(m => m.totalLoss).ToString(), headerStyle);
                            }
                            else
                            {
                                rowCount++; colCount = 0;
                                IRow newRowLosN = bomSheet.CreateRow(rowCount);
                                bomSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 4));
                                NPOIExcelHelper.CreateCustomCell(newRowLosN, 0, Resources.Resources.SI_OSP_GBL_GBL_RPT_001, cellstyleN, false, customStyle);
                            }
                        }
                    }
                }

				#endregion

				#region  Signature section 

                if (isAttr.Contains("FTRD"))
                {
                    if (objBomBoq.objAdAttribute.isAdditionalAttrEnable == true)
                    {
                        rowCount += 3;
                        IRow newRowSi1 = bomSheet.CreateRow(rowCount);
                        NPOIExcelHelper.CreateCustomCell(newRowSi1, 0, Resources.Resources.SI_OSP_GBL_GBL_GBL_270 + ":-", cellstyleWbBorder, false, customStyle);
                        NPOIExcelHelper.CreateCustomCell(newRowSi1, 2, Resources.Resources.SI_OSP_PM_NET_FRM_012 + ":-", cellstyleWbBorder, false, customStyle);
                        NPOIExcelHelper.CreateCustomCell(newRowSi1, 4, Resources.Resources.SI_OSP_PM_NET_FRM_028 + ":-", cellstyleWbBorder, false, customStyle);
                        NPOIExcelHelper.CreateCustomCell(newRowSi1, 6, Resources.Resources.SI_OSP_PM_NET_FRM_013 + ":-", cellstyleWbBorder, false, customStyle);
                        rowCount++;
                        IRow newRowSi2 = bomSheet.CreateRow(rowCount);
                        NPOIExcelHelper.CreateCustomCell(newRowSi2, 0, objBomBoq.objAdAttribute.estimatedby, cellstyleWbBorder, false, customStyle);
                        NPOIExcelHelper.CreateCustomCell(newRowSi2, 2, objBomBoq.objAdAttribute.checkedby, cellstyleWbBorder, false, customStyle);
                        NPOIExcelHelper.CreateCustomCell(newRowSi2, 4, objBomBoq.objAdAttribute.re_checkedby, cellstyleWbBorder, false, customStyle);
                        NPOIExcelHelper.CreateCustomCell(newRowSi2, 6, objBomBoq.objAdAttribute.approvedby, cellstyleWbBorder, false, customStyle);
                    }
                }

				#endregion

				NPOIExcelHelper.setBordersToMergedCells(workbook, bomSheet);

				for (int i = 0; i < 15; i++)
				{
					bomSheet.AutoSizeColumn(i, true);
				}

			}
			return workbook;
		}
		public static ICellStyle getCellStyleOther(IWorkbook wb, string styleType = "", bool iSBorderOnCell = true, string TextAlignment = "Left", int FntSize = 10, string iSBorderSpecific = "")
		{
			string txt = TextAlignment.ToUpper();
			ICellStyle cellStyle = (ICellStyle)wb.CreateCellStyle();
			cellStyle.Alignment = (txt == "RIGHT" ? HorizontalAlignment.Right : txt == "CENTER" ? HorizontalAlignment.Center : HorizontalAlignment.Left);
			cellStyle.VerticalAlignment = VerticalAlignment.Center;
			NPOI.XSSF.UserModel.XSSFFont ffont = (NPOI.XSSF.UserModel.XSSFFont)wb.CreateFont();
			ffont.FontName = "ARIAL";
			if (iSBorderOnCell)
			{
				cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
				cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
				cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
				cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
			}
			else if (iSBorderSpecific != "")
			{
				cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.None;
				cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.None;
				cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.None;
				cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.None;

				if (iSBorderSpecific.ToUpper() == "LEFT")
					cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
				if (iSBorderSpecific.ToUpper() == "TOP")
					cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
				if (iSBorderSpecific.ToUpper() == "RIGHT")
					cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
				if (iSBorderSpecific.ToUpper() == "BOTTOM")
					cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
			}
			if (styleType.ToUpper() == "TITLE")
			{
				ffont.FontHeightInPoints = ((short)FntSize);
				cellStyle.FillForegroundColor = IndexedColors.White.Index;
				cellStyle.WrapText = true;
				ffont.IsBold = true;
				cellStyle.BorderDiagonalColor = 0;
				cellStyle.SetFont(ffont);
			}
			else if (styleType.ToUpper() == "MAIN_HEADER")
			{
				ffont.FontHeightInPoints = ((short)FntSize);
				cellStyle.FillForegroundColor = IndexedColors.LemonChiffon.Index;
				cellStyle.FillPattern = FillPattern.SolidForeground;
				ffont.Color = IndexedColors.Black.Index;
				ffont.IsBold = true;
				cellStyle.SetFont(ffont);
			}
			else if (styleType.ToUpper() == "SUB_HEADER_1")
			{
				ffont.Color = IndexedColors.White.Index;
				ffont.FontHeightInPoints = ((short)FntSize);
				ffont.IsBold = true;
				cellStyle.FillForegroundColor = IndexedColors.SkyBlue.Index;
				cellStyle.FillPattern = FillPattern.SolidForeground;
			}
			else
			{
				ffont.FontHeightInPoints = (short)FntSize;
				cellStyle.SetFont(ffont);
			}
			return cellStyle;
		}

		public void ExportWorkBookToExcel(IWorkbook workbook, string fileName)
		{

			using (var exportData = new System.IO.MemoryStream())
			{
				Response.Clear();
				workbook.Write(exportData);
				Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
				Response.BinaryWrite(exportData.ToArray());
				Response.End();
			}
		}

		public ActionResult BOMBOQView(BomBoqInfoSummary objbomBoqInfoSummary)
		{

			List<BomBoqInfo> lstBomBoqInfo = new List<BomBoqInfo>();
			BomBoqExportFilterDesign bomBoqExportFilterDesign = new BomBoqExportFilterDesign();
			var userdetails = (User)Session["userDetail"];
			bomBoqExportFilterDesign.userId = Convert.ToInt32(userdetails.user_id);
			bomBoqExportFilterDesign.user_name = Convert.ToString(userdetails.user_name);
			bomBoqExportFilterDesign.systemId = objbomBoqInfoSummary.fsa_system_id;//Convert.ToInt32("212");
			bomBoqExportFilterDesign.entityName = Convert.ToString("SubArea");
			bomBoqExportFilterDesign.bom_boq_id = 0;
			bomBoqExportFilterDesign.action = objbomBoqInfoSummary.action;
			//objBOMViewModel.objReportFilters.roleId = Convert.ToInt32(userdetails.role_id);
			//lstBomBoqInfo = new BomBoq().GetBOMBOQDesignReport(bomBoqExportFilterDesign);
			BomBoqInfoSummary bomBoqInfoSummary = new BomBoq().GetBOMBOQDesignReport(bomBoqExportFilterDesign);
			bomBoqInfoSummary.action = bomBoqExportFilterDesign.action;
			//BindReportDropdownNew(ref objBomBoq);
			//Session["BomBoqExportSummaryData"] = objBomBoq;
			//bomBoqInfoSummary.status = "temp";
			return PartialView("_BOMBOQView_Revised", bomBoqInfoSummary);
			//return PartialView("_BOMBOQView", lstBomBoqInfo);
		}

		[HttpPost]
		public ActionResult SaveBOMBOQView(List<BomBoqInfo> objBomBoqInfo)
		{
			PageMessage pg = new PageMessage();
			pg.message = "Quantity updated successfully";
			pg.status = "OK";
			var userdetails = (User)Session["userDetail"];
			objBomBoqInfo.ToList().ForEach(c =>
			{
				c.modified_by_user_id = Convert.ToInt32(userdetails.user_id);
				c.modified_user_name = Convert.ToString(userdetails.user_name);
			});
			var lst = new BomBoq().SaveBOMBOQReportDesign(objBomBoqInfo);
			return Json(pg, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult GetDependentItems(List<BomBoqInfo> objBomBoqInfo)
		{
			JsonResponse<List<BomBoqInfo>> oJsonResponse = new Models.JsonResponse<List<BomBoqInfo>>();
			List<BomBoqInfo> lstBomBoqInfo = new List<BomBoqInfo>();
			var userdetails = (User)Session["userDetail"];
			string sEntityClass = objBomBoqInfo.Select(s => s.entity_class).FirstOrDefault();
			string sEntitySubClass = objBomBoqInfo.Select(s => s.entity_sub_class).FirstOrDefault();
			int iBomBoQId = objBomBoqInfo.Select(s => s.bom_boq_id).FirstOrDefault();
			int iFSASystemId = objBomBoqInfo.Select(s => s.fsa_system_id).FirstOrDefault();
			lstBomBoqInfo = new BomBoq().GetBOMBOQDependentItems(iBomBoQId, iFSASystemId, sEntityClass, sEntitySubClass, "RefreshBomItem");
			oJsonResponse.status = "OK";
			oJsonResponse.message = "";
			oJsonResponse.result = lstBomBoqInfo;
			return Json(oJsonResponse, JsonRequestBehavior.AllowGet);
		}

        public ActionResult SubmitBOMBOQ(BomBoqInfoSummary bomBoqInfoSummary)
        {
            string sStatus = string.Empty;
            PageMessage pg = new PageMessage();
            if (@ApplicationSettings.IsTraceEnabled == true)
            {
                pg.message = "Design BOM has been submitted successfully and FSA has been locked for S2 addition and Splicing modification!";
            }
            else
            {
                pg.message = "BOM has been submitted successfully.";
            }
            pg.status = "OK";
            string actaualStatus = bomBoqInfoSummary.status;
            bomBoqInfoSummary.status = bomBoqInfoSummary.status == "TEMP" ? "SAVED" : "SUBMITTED";
            var userdetails = (User)Session["userDetail"];
            bomBoqInfoSummary.modified_user_name = Convert.ToString(userdetails.user_name);
            bomBoqInfoSummary.modified_by_user_id = Convert.ToInt32(userdetails.user_id);


            string filePath = "";
            string projectName = "NA";
            List<KeyValueDropDown> objList = new BomBoq().GetBomBoqProjectCode(bomBoqInfoSummary.bom_boq_id);


            try
            {
                if (objList != null && objList.Count > 0 && objList[0].value != null)
                    projectName = objList[0].value.ToString();
                string message = ":bom_boq_id" + bomBoqInfoSummary.bom_boq_id + ":modified_by_user_id-" + bomBoqInfoSummary.modified_by_user_id + ":actaualStatus:" + actaualStatus + ":modified_user_name" + bomBoqInfoSummary.modified_user_name;
                commonUtil.CaptureErrorInFile(message, "", "");
                filePath = new BOMBOQController().BOMBOQAttachmentReport(bomBoqInfoSummary.bom_boq_id, actaualStatus, bomBoqInfoSummary.modified_by_user_id, bomBoqInfoSummary.modified_user_name);
            }
            catch (Exception ex)
            {
                string message = ex.Message + ":bom_boq_id" + bomBoqInfoSummary.bom_boq_id + ":modified_by_user_id-" + bomBoqInfoSummary.modified_by_user_id + ":actaualStatus:" + actaualStatus + ":modified_user_name" + bomBoqInfoSummary.modified_user_name;
                string innerException = ex.InnerException != null ? ex.InnerException.Message + "." + (ex.InnerException.InnerException != null ? ex.InnerException.InnerException.ToString() : "") : string.Empty;
                string stackTrace = ex.StackTrace;
                commonUtil.CaptureErrorInFile(message, innerException, stackTrace);
            }

            sStatus = new BomBoq().UpdateBomBoqStatus(bomBoqInfoSummary);
            if (sStatus != "OK")
            {
                pg.message = sStatus;
                pg.status = "Fail";
            }
            else
            {
                #region email sending start               
                Models.User objUser = new BLUser().GetUserDetailByID(bomBoqInfoSummary.modified_by_user_id);
                objUser.name = MiscHelper.Decrypt(objUser.name);
                // string subject = objEventEmailTemplateDetail[0].subject.Replace("XXX", bomBoqInfoSummary.fsa_name);
                Dictionary<string, string> objDict = new Dictionary<string, string>();
                objDict.Add("Date", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));
                objDict.Add("User", objUser.name);
                objDict.Add("Comments", "Approved");
                if (bomBoqInfoSummary.status.ToLower() == "saved")
                {
                    BLUser objBLuser = new BLUser();
                    List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EmailEventList.ManagerReview.ToString());
                    List<string> objAttachmentFileList = new List<string>();
                    if (filePath != null && filePath != "")
                        objAttachmentFileList.Add(filePath);
                    System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, EmailSettings.AllEmailSettings, objAttachmentFileList, projectName, EmailEventList.ManagerReview.ToString()));
                    //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, objAttachmentFileList, projectName);
                }
                else
                {
                    BLUser objBLuser = new BLUser();
                    List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EmailEventList.MaterialRequestSubmitted.ToString());
                    List<string> objAttachmentFileList = new List<string>();
                    objAttachmentFileList.Add(filePath);
                    System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, EmailSettings.AllEmailSettings, objAttachmentFileList, projectName, EmailEventList.MaterialRequestSubmitted.ToString()));
                    //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, objAttachmentFileList, projectName);

                }
                #endregion
            }
            return Json(pg, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExportBOMBOQ(int id, string status)
		{
			BomBoqExportFilterDesign bomBoqExportFilterDesign = new BomBoqExportFilterDesign();
			var userdetails = (User)Session["userDetail"];
			bomBoqExportFilterDesign.userId = Convert.ToInt32(userdetails.user_id);
			bomBoqExportFilterDesign.user_name = Convert.ToString(userdetails.user_name);
			bomBoqExportFilterDesign.systemId = 0;
			bomBoqExportFilterDesign.entityName = Convert.ToString("SubArea");
			bomBoqExportFilterDesign.bom_boq_id = id;
			bomBoqExportFilterDesign.action = "export";// status;

            Dictionary<string, string> lstExportColumn = new Dictionary<string, string>();
            
            lstExportColumn.Add("fsa_id", "FSA ID");
            lstExportColumn.Add("fsa_name", "FSA Name");
            lstExportColumn.Add("entity_class", "Entity Class");
            lstExportColumn.Add("entity_sub_class", "Entity Type");            
            lstExportColumn.Add("specification", "Model Name");
            lstExportColumn.Add("item_code", "Item Code");
            lstExportColumn.Add("sub_category_1", "Short Description");
            //lstExportColumn.Add("sub_category_2", "Long Desc.");
            //lstExportColumn.Add("gis_uom", "GIS UOM");
            lstExportColumn.Add("uom_sap", "UOM");
            //lstExportColumn.Add("gis_qty", "GIS QTY");          
            //lstExportColumn.Add("overhead_percentage", "Overhead Percentage");
            if (status == "TEMP")
            {
                lstExportColumn.Add("design_qty", "Design QTY");
            }
            else
            {
                lstExportColumn.Add("design_qty", "Construction BOM QTY");
            }
            if(status != "TEMP")
            {
                lstExportColumn.Add("additional_qty", "Custom QTY");
                //lstExportColumn.Add("additional_non_design_material_qty", "Additional NON Design Material QTY");
                lstExportColumn.Add("final_qty", "Final QTY");
            }           
            lstExportColumn.Add("cost_per_unit", "Cost Per Unit ("+ApplicationSettings.Currency+")");
            lstExportColumn.Add("pts_code", "PTS Code");
            lstExportColumn.Add("revision", "Revision");      
            
            try
            {
               
                List<BomBoqInfo> lstBomBoqInfo = new BomBoq().ExportBOMBOQ(bomBoqExportFilterDesign);
                string[] ExportColName = lstExportColumn.Select(i => i.Key.ToString()).ToArray();
                string sFSAId = string.Empty;
                DataTable dt1 = MiscHelper.ListToDataTable<BomBoqInfo>(lstBomBoqInfo, true, ApplicationSettings.numberFormatType, new string[] { "Actual Length(Km)  (Calculated and Loop Length)", "GIS Length(Km)", "Item Code", "PTS_code" });
                dt1.TableName = status == "TEMP" ? "Design BOM BOQ": "Construction BOM BOQ";
                //pk
                //dt1 = Utility.CommonUtility.GetFormattedDataTable(dt1, ApplicationSettings.numberFormatType);

                if (dt1.Rows.Count > 0)
                {
                    DataView view = new DataView(dt1);
                    DataTable dt = view.ToTable(false, ExportColName);
                    foreach (var item in lstExportColumn)
                    {
                        dt.Columns[item.Key].ColumnName = item.Value;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        sFSAId = Convert.ToString(dt1.Rows[0]["fsa_system_id"]);
                        var filename = sFSAId +"_" + (status == "TEMP" ? "Design" : "Construction") + "_BOMBOQ_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "_" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";
                        //commented by priyanka
                        //string filepath = System.Web.HttpContext.Current.Server.MapPath("~/uploads/temp/") + filename;
                        //end commented by priyanka
                        
                        string filepath = System.Web.HttpContext.Current.Server.MapPath(Settings.ApplicationSettings.DownloadTempPath) + filename;
                        string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, filepath);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                    }
                    else
                    {
                        return Json("File not Exists");
                    }
                }
                else
                {
                    return Json("File not Exists");
                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
			catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
			{
				ErrorLogHelper.WriteErrorLog("DownloadBulkUserUploadLogs", "UserController", ex);
				return null;
			}
		}
        public string BOMBOQAttachmentReport(int id, string status, int user_id, string user_name)
        {
            BomBoqExportFilterDesign bomBoqExportFilterDesign = new BomBoqExportFilterDesign();
            bomBoqExportFilterDesign.userId = user_id;
            bomBoqExportFilterDesign.user_name = user_name;
            bomBoqExportFilterDesign.systemId = 0;
            bomBoqExportFilterDesign.entityName = Convert.ToString("SubArea");
            bomBoqExportFilterDesign.bom_boq_id = id;
            bomBoqExportFilterDesign.action = "export";// status;

            Dictionary<string, string> lstExportColumn = new Dictionary<string, string>();

            lstExportColumn.Add("fsa_id", "FSA ID");
            lstExportColumn.Add("fsa_name", "FSA Name");
            lstExportColumn.Add("entity_class", "Entity Class");
            lstExportColumn.Add("entity_sub_class", "Entity Type");
            lstExportColumn.Add("specification", "Model Name");
            lstExportColumn.Add("item_code", "Item Code");
            lstExportColumn.Add("sub_category_1", "Short Description");
            //lstExportColumn.Add("sub_category_2", "Long Desc.");
            //lstExportColumn.Add("gis_uom", "GIS UOM");
            lstExportColumn.Add("uom_sap", "UOM");
            //lstExportColumn.Add("gis_qty", "GIS QTY");          
            //lstExportColumn.Add("overhead_percentage", "Overhead Percentage");
            if (status == "TEMP")
            {
                lstExportColumn.Add("design_qty", "Design QTY");
            }
            else
            {
                lstExportColumn.Add("design_qty", "Construction BOM QTY");
            }
            if (status != "TEMP")
            {
                lstExportColumn.Add("additional_qty", "Custom QTY");
                //lstExportColumn.Add("additional_non_design_material_qty", "Additional NON Design Material QTY");
                lstExportColumn.Add("final_qty", "Final QTY");
            }
            lstExportColumn.Add("cost_per_unit", "Cost Per Unit");
            lstExportColumn.Add("jpf_total_capex", "JFP Total Capex");
            lstExportColumn.Add("pts_code", "PTS Code");
            lstExportColumn.Add("revision", "Revision");
            lstExportColumn.Add("jc_sap_id", "JC SAP ID");
            lstExportColumn.Add("jc_name", "JC Name");
            try
            {

                List<BomBoqInfo> lstBomBoqInfo = new BomBoq().ExportBOMBOQ(bomBoqExportFilterDesign);
                string[] ExportColName = lstExportColumn.Select(i => i.Key.ToString()).ToArray();
                string sFSAId = string.Empty;
                DataTable dt1 = MiscHelper.ListToDataTable<BomBoqInfo>(lstBomBoqInfo);
                dt1.TableName = status == "TEMP" ? "Design BOM BOQ" : "Construction BOM BOQ";
                if (dt1.Rows.Count > 0)
                {
                    DataView view = new DataView(dt1);
                    DataTable dt = view.ToTable(false, ExportColName);
                    foreach (var item in lstExportColumn)
                    {
                        dt.Columns[item.Key].ColumnName = item.Value;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        sFSAId = Convert.ToString(dt1.Rows[0]["fsa_id"]);
                        var filename = sFSAId + "_" + (status == "TEMP" ? "Design" : "Construction") + "_BOMBOQ_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "_" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";
                        string filepath = System.Web.HttpContext.Current.Server.MapPath("~/uploads/temp/") + filename;
                        string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, filepath);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                        return filepath;
                        // return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                    }
                    else
                    {
                        return null;
                        // return Json("File not Exists");
                    }
                }
                else
                {
                    return null;
                    //return Json("File not Exists");
                }
            }
            // The variable 'ex' is declared but never used
            catch (Exception ex)
            // The variable 'ex' is declared but never used
            {
                ErrorLogHelper.WriteErrorLog("BOMBOQAttachmentReport", "BOMBOQController", ex);
                return null;
            }
        }


        public ActionResult ConstructionBOMDetail(ConstructionBomDetailsVM objConstructionBomDetailsVM, int page = 0, string sort = "", string sortdir = "")
		{
			var usrDetail = (User)Session["userDetail"];
			//BIND SERACH BY DROPDOWN.. STATIC VALUES
			BindSearchBy(objConstructionBomDetailsVM);
			SchedulerLog schedulerlog = new SchedulerLog();
			schedulerlog = new BLCBomLogic().getschedulardate();
			//BIND VENDOR SPECIFICATION LIST WITH PAGING
			objConstructionBomDetailsVM.viewConstructionBomDetails.pageSize = 10;
			objConstructionBomDetailsVM.viewConstructionBomDetails.currentPage = page == 0 ? 1 : page;
			objConstructionBomDetailsVM.viewConstructionBomDetails.sort = sort;
			objConstructionBomDetailsVM.viewConstructionBomDetails.orderBy = sortdir;
			objConstructionBomDetailsVM.constructionBomDetailList = new BLBom().GetConstructionBomDetailList(objConstructionBomDetailsVM);

			// objConstructionBomDetailsVM.constructionBomDetailList = new BLBom().GetConstructionBomDetailList(objConstructionBomDetailsVM);
			//var modified_date = objConstructionBomDetailsVM.constructionBomDetailList.Select(x => x.modified_on).SingleOrDefault()  ;
			//var modified_date = objConstructionBomDetailsVM.constructionBomDetailList.Where(C => C.modified_on != null).Select(x => x.modified_on).FirstOrDefault();
			var modified_date = schedulerlog.action_on;
			objConstructionBomDetailsVM.modified_on = modified_date.HasValue ? modified_date.Value.ToString("dddd, dd MMMM yyyy hh:mm tt") : "";
			objConstructionBomDetailsVM.lstUserModule = new BLLayer().GetUserModuleAbbrList(usrDetail.user_id, UserType.Web.ToString());
			objConstructionBomDetailsVM.viewConstructionBomDetails.totalRecord = objConstructionBomDetailsVM.constructionBomDetailList != null && objConstructionBomDetailsVM.constructionBomDetailList.Count > 0 ? objConstructionBomDetailsVM.constructionBomDetailList[0].totalRecords : 0;
			Session["ConstructionBomDetails"] = objConstructionBomDetailsVM.viewConstructionBomDetails;
			return PartialView("_ConstructionBOMDetails", objConstructionBomDetailsVM);
		}
		public IList<KeyValueDropDown> BindSearchBy(TemplateForDropDownConstructionBom objTemplateForDropDown)
		{
			List<KeyValueDropDown> items = new List<KeyValueDropDown>();
			items.Add(new KeyValueDropDown { key = "FSA ID", value = "fsa_id" });
			items.Add(new KeyValueDropDown { key = "State", value = "state" });
			items.Add(new KeyValueDropDown { key = "JC", value = "jc" });
			return objTemplateForDropDown.lstBindSearchBy = items.OrderBy(m => m.key).ToList();
		}
		//public PartialViewResult EditConstructionBomDetails(int id)
		//{
		//    string source = "";
		//    ConstructionBomDetails objConstructionBomDetails = new ConstructionBomDetails();
		//    objConstructionBomDetails.source = source;

		//        if (id != 0)
		//        {
		//            objConstructionBomDetails = new BLBom().GetConstructionBomDetailsByID(id);

		//        }

		//    return PartialView("_EditConstructionBomDetails", objConstructionBomDetails);
		//}
		public ActionResult Constructionsetdefault(int id)
		{
			PageMessage pg = new PageMessage();
			pg.message = "updated successfully";
			ConstructionBomDetails objConstructionBomDetails = new ConstructionBomDetails();
			if (id != 0)
			{
				objConstructionBomDetails = new BLBom().GetConstructionBomDetailsByID(id);

			}
			return Json(pg, JsonRequestBehavior.AllowGet);
		}
		public ActionResult SaveConstructionBomDetails(ConstructionBomDetails objConstructionBomDetails)
		{
			ModelState.Clear();
			if (ModelState.IsValid)
			{
				objConstructionBomDetails = new BLBom().SaveConstructionBomDetails(objConstructionBomDetails);
			}
			return Json(objConstructionBomDetails, JsonRequestBehavior.AllowGet);
		}
		public void ExportConstructionBomDetail()

		{
			if (Session["ConstructionBomDetails"] != null)
			{
				IWorkbook workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();

                ConstructionBomDetailsVM objConstructionBomDetailsVM = new ConstructionBomDetailsVM();
                objConstructionBomDetailsVM.viewConstructionBomDetails = (ViewConstructionBomDetails)Session["ConstructionBomDetails"];
                objConstructionBomDetailsVM.viewConstructionBomDetails.currentPage = 0;
                objConstructionBomDetailsVM.viewConstructionBomDetails.pageSize = 0;
                objConstructionBomDetailsVM.constructionBomDetailList = new BLBom().GetConstructionBomDetailList(objConstructionBomDetailsVM);
                DataTable dtReport = new DataTable();
                dtReport = Utility.MiscHelper.ListToDataTable(objConstructionBomDetailsVM.constructionBomDetailList,true,ApplicationSettings.numberFormatType,new string[] {"Item Code", "PTS code" });

                dtReport.Columns["state"].SetOrdinal(0);
                dtReport.Columns["jc"].SetOrdinal(1);
                dtReport.Columns["town_name"].SetOrdinal(2);
                dtReport.Columns["town_code"].SetOrdinal(3);
                dtReport.Columns["fsa_id"].SetOrdinal(4);
                dtReport.Columns["feeder_length"].SetOrdinal(5);
                dtReport.Columns["distribution_length"].SetOrdinal(6);
                dtReport.Columns["pole_count"].SetOrdinal(7);
                dtReport.Columns["oh_feeder_cable"].SetOrdinal(8);
                dtReport.Columns["oh_distribution_cable"].SetOrdinal(9);
                dtReport.Columns["oh_clamps"].SetOrdinal(10);
                dtReport.Columns["oh_jclamps"].SetOrdinal(11);
                dtReport.Columns["oh_termination_set"].SetOrdinal(12);
                dtReport.Columns["oh_suspension_set"].SetOrdinal(13);

                dtReport.Columns.Remove("totalrecords");
                dtReport.Columns.Remove("s_no");
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("page_count");
                dtReport.Columns.Remove("MODIFIED_ON");
                dtReport.Columns.Remove("FEEDER_GIS_LENGTH");
                dtReport.Columns.Remove("DISTRIBUTION_GIS_LENGTH");
                dtReport.Columns.Remove("JUBILEE_CLAMP");
                dtReport.Columns.Remove("TERMINATION_SET");
                dtReport.Columns.Remove("SUSPENSION_SET");
                dtReport.Columns.Remove("CLAMP");
                dtReport.Columns.Remove("FSA_SYSTEM_ID");
                dtReport.Columns["state"].ColumnName = "State";
                dtReport.Columns["jc"].ColumnName = "JC";
                dtReport.Columns["town_name"].ColumnName = "Town Name";
                dtReport.Columns["town_code"].ColumnName = "Town Code";
                dtReport.Columns["fsa_id"].ColumnName = "FSA ID";
                dtReport.Columns["feeder_length"].ColumnName = "Feeder Length - GIS(KM)";
                dtReport.Columns["distribution_length"].ColumnName = "Distribution Length- GIS(KM)";
                dtReport.Columns["pole_count"].ColumnName = "Pole Count";
                dtReport.Columns["oh_feeder_cable"].ColumnName = "Feeder Cable (%)";
                dtReport.Columns["oh_distribution_cable"].ColumnName = "Distribution Cable (%)";
                dtReport.Columns["oh_clamps"].ColumnName = "Clamps";
                dtReport.Columns["oh_jclamps"].ColumnName = "Jubliee Clamp";
                dtReport.Columns["oh_termination_set"].ColumnName = "Termination Set";
                dtReport.Columns["oh_suspension_set"].ColumnName = "Suspension Set";
                var Exportfilename = "Construction BOM Details";
                dtReport.TableName = "ConstructionBOMDetailsList";

                //dtReport = Utility.CommonUtility.GetFormattedDataTable(dtReport,ApplicationSettings.numberFormatType);

                SchedulerLog schedulerlog = new SchedulerLog();
                schedulerlog = new BLCBomLogic().getschedulardate();
                ExportConstructionBOMDetails(schedulerlog, dtReport, Exportfilename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }

		private void ExportConstructionBOMDetails(SchedulerLog schedulerlog, DataTable dtReport, string fileName)
		{
			using (var exportData = new MemoryStream())
			{
				Response.Clear();
				if (dtReport != null && dtReport.Rows.Count > 0)
				{
					var modified_date = schedulerlog.action_on;
					ConstructionBomDetailsVM objConstructionBomDetailsVM = new ConstructionBomDetailsVM();
					objConstructionBomDetailsVM.modified_on = modified_date.HasValue ? modified_date.Value.ToString("dddd, dd MMMM yyyy hh:mm tt") : "";
					IWorkbook workbook = NPOIExcelHelper.CBOMDataTableToExcel("xlsx", dtReport, schedulerlog);
					workbook.Write(exportData);
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
					Response.BinaryWrite(exportData.ToArray());
					Response.End();
				}
			}
		}

		public ActionResult GetVariables(string column_name, int fsa_id, int execution_sequence)
		{
			ConstructionOhLogicMaster OhLogicModel = new ConstructionOhLogicMaster();
			OhLogicModel = new BLCBomLogic().GetCBOMLogic(column_name, fsa_id);
			OhLogicModel.isDefaultOh = (column_name == "feeder_gis_length" || column_name == "distribution_gis_length");
			OhLogicModel.constructionohlogicmasterlist = new BLCBomLogic().GetVariables().Where(c => c.is_active).OrderBy(c => c.execution_sequence).ToList().DistinctBy(c => c.column_name).Take(OhLogicModel.execution_sequence - 1).ToList();
			OhLogicModel.subarea_system_id = fsa_id;
			return PartialView("_CBOMVariable", OhLogicModel);
			//var userdetails = (User)Session["userDetail"];
			//ConstructionOhLogicMaster objVoiceCommand1 = new ConstructionOhLogicMaster();
			//// ConstructionOhLogicMaster objVoiceCommand1 = new BLCBomLogic().GetVariables();
			//objVoiceCommand1 = new BLCBomLogic().GetCBOMLogic(column_name, fsa_id);
			//if (objVoiceCommand1 != null)
			//{
			//    if (column_name == "feeder_gis_length" || column_name == "distribution_gis_length")
			//    {
			//        ViewBag.default_overhead = objVoiceCommand1.default_overhead;
			//        ViewBag.column_name = objVoiceCommand1.column_name;
			//        ViewBag.fsa_id = fsa_id;
			//        return PartialView("_CBOMVariable", objVoiceCommand1);
			//    }
			//    else
			//    {
			//        ConstructionOhLogicMaster objVoiceCommand = new ConstructionOhLogicMaster();

			//        objVoiceCommand.constructionohlogicmasterlist = new BLCBomLogic().GetVariables().Where(c => c.is_active).OrderBy(c => c.execution_sequence).ToList().DistinctBy(c => c.column_name).Take(objVoiceCommand1.execution_sequence - 1).ToList();
			//        //objVoiceCommand.constructionohlogicmasterlist = new BLCBomLogic().GetAllCBOMLogic().Take(objVoiceCommand1.execution_sequence - 1).ToList().Distinct().OrderBy(objVoiceCommand1.execution_sequence).ToList();
			//        ViewBag.logic = objVoiceCommand1.oh_display_formula;
			//        ViewBag.logic1 = objVoiceCommand1.oh_logic;
			//        ViewBag.column_name = objVoiceCommand1.column_name;
			//        ViewBag.fsa_id = fsa_id;
			//        ViewBag.oh_expression_json = objVoiceCommand1.oh_expression_json;
			//        return PartialView("_CBOMVariable", objVoiceCommand);
			//    }
			//}
			//else
			//{
			//    ConstructionOhLogicMaster objVoiceCommand = new ConstructionOhLogicMaster();
			//    //ConstructionOhLogicMaster objVoiceCommandbycolumnname = new BLCBomLogic().GetCBOMLogicbycolumn(column_name);
			//    ConstructionOhLogicMaster objVoiceCommandbycolumnname = new BLCBomLogic().GetCBOMLogic(column_name, fsa_id);
			//    objVoiceCommand.constructionohlogicmasterlist = new BLCBomLogic().GetVariables().Where(c => c.is_active).OrderBy(c => c.execution_sequence).ToList().DistinctBy(c => c.column_name).Take(objVoiceCommandbycolumnname.execution_sequence - 1).ToList();
			//   // new BLCBomLogic().GetAllCBOMLogic().OrderBy(c => c.execution_sequence).Take(objVoiceCommandbycolumnname.execution_sequence - 1).ToList();

			//    if (objVoiceCommandbycolumnname.oh_display_formula != "")
			//    {
			//        ViewBag.logic = objVoiceCommandbycolumnname.oh_display_formula;
			//        ViewBag.logic1 = objVoiceCommandbycolumnname.oh_logic;
			//        ViewBag.column_name = objVoiceCommandbycolumnname.column_name;
			//        ViewBag.oh_expression_json = objVoiceCommandbycolumnname.oh_expression_json;
			//        ViewBag.fsa_id = fsa_id;
			//    }
			//    else if(column_name== "feeder_gis_length" || column_name == "distribution_gis_length")
			//    {
			//        ViewBag.default_overhead = objVoiceCommandbycolumnname.default_overhead;
			//        ViewBag.column_name = objVoiceCommandbycolumnname.column_name;
			//        ViewBag.fsa_id = fsa_id;
			//    }

			//    return PartialView("_CBOMVariable", objVoiceCommand);
		}
		//ConstructionOhLogicMaster objVoiceCommand = new ConstructionOhLogicMaster();
		//ViewBag.column_name = column_name;
		//ViewBag.fsa_id = fsa_id;
		//objVoiceCommand.constructionohlogicmasterlist = new BLCBomLogic().GetAllCBOMLogic().Where(c => c.is_default == true).OrderBy(c => c.execution_sequence).Take(execution_sequence - 1).ToList();
		//ConstructionOhLogicMaster objVoiceCommand1 = new BLCBomLogic().GetCBOMLogic(column_name, fsa_id);
		//ViewBag.logic = objVoiceCommand1.oh_display_formula;
		//ViewBag.logic1 = objVoiceCommand1.oh_logic;

		//}
		//else
		//{
		//    return PartialView("_CBOMVariable");
		//}

		//[HttpPost]

		//public ActionResult savebomlogic(string formula, string fsa_id, string formula1, string column_name)
		//{
		//    StringBuilder sb = new StringBuilder();
		//    sb.Append(formula);
		//    //int subarea_system_id = Convert.ToInt32(fsa_id);

		//    ConstructionOhLogicMaster objVoiceCommanddictionary = new ConstructionOhLogicMaster();

		//    objVoiceCommanddictionary.constructionohlogicmasterlist = new BLCBomLogic().GetAllCBOMLogic().ToList();
		//    Dictionary<string, string> dict = new Dictionary<string, string>();
		//    dict = objVoiceCommanddictionary.constructionohlogicmasterlist.ToList().Where(c => c.is_active).DistinctBy(c => c.column_name).ToDictionary(d => d.label, d => d.column_name);
		//    //int i = 1;

		//    foreach (KeyValuePair<string, string> replacement in dict)
		//    {

		//        sb.Replace(replacement.Key, replacement.Value.ToString());

		//    }
		//    string formulaforlogic = sb.ToString();
		//    int subarea_system_id = Convert.ToInt32(fsa_id);
		//    int Success = new BLCBomLogic().Savebomlogic(formula, subarea_system_id, formulaforlogic, column_name, string.Empty);
		//    string isSuccess= Success.ToString();
		//    if (isSuccess=="1")
		//    {
		//        isSuccess = "true";
		//    }
		//    return Json(new
		//    {
		//            Message = isSuccess,
		//            formula = formula,
		//            formula1 = formula1,
		//            JsonRequestBehavior.AllowGet
		//        });


		//    //return Json(new { Message = message, formula = formula, formula1 = formula1, ex_sequence= execution_Sequence, id=id, JsonRequestBehavior.AllowGet });
		//}
		public ActionResult savebomlogic(OverheadLogicsDTO overheadLogicsDTO)
		{

			int Success = new BLCBomLogic().Savebomlogic(overheadLogicsDTO.DisplayFormula, Convert.ToInt32(overheadLogicsDTO.FSAId), overheadLogicsDTO.OhLogic, overheadLogicsDTO.ColumnName, overheadLogicsDTO.OhExpressionJson);
			ConstructionOhLogicMaster objVoiceCommand = new BLCBomLogic().GetCBOMLogic(overheadLogicsDTO.ColumnName, Convert.ToInt32(overheadLogicsDTO.FSAId));

			return Json(new
			{
				Message = Success == 1,
				formula = overheadLogicsDTO.DisplayFormula,
				id = objVoiceCommand.id,
				JsonRequestBehavior.AllowGet
			});

		}

		public ActionResult saveDefaultvalue(string defaultvalue, string columnname, int fsaid)
		{
			var status = new BLCBomLogic().Savedefaultoverhead(fsaid, defaultvalue, columnname);

			return Json(new
			{
				Message = status,

				JsonRequestBehavior.AllowGet
			});
		}
		//public ActionResult savebomlogic(string formula, string fsa_id, string formula1, string column_name)

		//{
		//    string message = null;
		//    int system_fsa_id =Convert.ToInt32(fsa_id);
		//    ConstructionOhLogicMaster objVoiceCommand = new ConstructionOhLogicMaster();
		//    ConstructionOhLogicMaster objVoiceCommand1 = new BLCBomLogic().GetCBOMLogic(column_name, system_fsa_id);
		//    if (objVoiceCommand1 != null)
		//    {

		//        objVoiceCommand.oh_display_formula = formula;
		//        objVoiceCommand.oh_logic = formula1;

		//        var status = new BLCBomLogic().SaveCBOMLogic(objVoiceCommand);

		//        if (status == true)
		//        {
		//            message = "SUCCESS";
		//        }
		//        else
		//        {
		//            message = "Fail";
		//        }
		//        return Json(new
		//        {
		//            Message = message,
		//            formula = formula,
		//            formula1 = formula1,
		//            JsonRequestBehavior.AllowGet
		//        });
		//    }
		//    else
		//    {
		//        objVoiceCommand.oh_display_formula = formula;
		//        objVoiceCommand.oh_logic = formula1;

		//        var status = new BLCBomLogic().SaveCBOMLogic(objVoiceCommand);

		//        if (status == true)
		//        {
		//            message = "SUCCESS";
		//        }
		//        else
		//        {
		//            message = "Fail";
		//        }
		//        return Json(new
		//        {
		//            Message = message,
		//            formula = formula,
		//            formula1 = formula1,
		//            JsonRequestBehavior.AllowGet
		//        });
		//    }
		//    //return Json(new { Message = message, formula = formula, formula1 = formula1, ex_sequence= execution_Sequence, id=id, JsonRequestBehavior.AllowGet });
		//}

		[HttpGet]
		public ActionResult Validatebomformula(string formula)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(formula);
			StringBuilder sb1 = new StringBuilder();
			ConstructionOhLogicMaster objVoiceCommand = new ConstructionOhLogicMaster();
			objVoiceCommand.constructionohlogicmasterlist = new BLCBomLogic().GetAllCBOMLogic().ToList();
			Dictionary<string, int> dict = new Dictionary<string, int>();
			int i = 1;
			//foreach (var item in objVoiceCommand.constructionohlogicmasterlist.Select(s => s.column_name).Distinct())
			foreach (var item in objVoiceCommand.constructionohlogicmasterlist.Select(s => s.label).Distinct().OrderByDescending(c => c.Length))
			{


				dict.Add(item, i);
				i = i + 1;
			}

			foreach (KeyValuePair<string, int> replacement in dict)
			{
				sb.Replace(replacement.Key, replacement.Value.ToString());

			}
			string total = sb.ToString();

			return Json(new { total = total }, JsonRequestBehavior.AllowGet);
		}
		public ActionResult FSAlocked(FSAlockedDetailsVM objFSAlockedDetailsVM, int page = 0, string sort = "", string sortdir = "")
		{
			var usrDetail = (User)Session["userDetail"];
			//BIND SERACH BY DROPDOWN.. STATIC VALUES
			BindSearchBy(objFSAlockedDetailsVM);
			objFSAlockedDetailsVM.viewLockedDetails.pageSize = 10;
			objFSAlockedDetailsVM.viewLockedDetails.currentPage = page == 0 ? 1 : page;
			objFSAlockedDetailsVM.viewLockedDetails.sort = sort;
			objFSAlockedDetailsVM.viewLockedDetails.orderBy = sortdir;
			objFSAlockedDetailsVM.lockedFSADetailList = new BLUnlock().GetFSAUnlockBomDetailList(objFSAlockedDetailsVM);
			objFSAlockedDetailsVM.viewLockedDetails.totalRecord = objFSAlockedDetailsVM.lockedFSADetailList != null && objFSAlockedDetailsVM.lockedFSADetailList.Count > 0 ? objFSAlockedDetailsVM.lockedFSADetailList[0].totalRecords : 0;
			Session["FSAlockedDetailsInfo"] = objFSAlockedDetailsVM.viewLockedDetails;

			return PartialView("_FSAlockedDetails", objFSAlockedDetailsVM);
		}
		public ActionResult UnlockFSA(int id)
		{
			PageMessage pm = new PageMessage();
			pm.message = "Unlock successfully";
			FSAlockedDetailsInfo objFSABomUnlockDetailsInfo = new FSAlockedDetailsInfo();
			if (id != 0)
			{
				new BLUnlock().UnlockFSAByID(id);

			}
			return Json(pm, JsonRequestBehavior.AllowGet);
		}
		public void ExportlockedFSADetail()
		{
			if (Session["FSAlockedDetailsInfo"] != null)
			{
				FSAlockedDetailsVM objFSAlockedDetailsVM = new FSAlockedDetailsVM();
				objFSAlockedDetailsVM.viewLockedDetails = (ViewLockedDetails)Session["FSAlockedDetailsInfo"];
				objFSAlockedDetailsVM.viewLockedDetails.currentPage = 0;
				objFSAlockedDetailsVM.viewLockedDetails.pageSize = 0;
				objFSAlockedDetailsVM.lockedFSADetailList = new BLUnlock().GetFSAUnlockBomDetailList(objFSAlockedDetailsVM);
				DataTable dtReport = new DataTable();
				dtReport = Utility.MiscHelper.ListToDataTable(objFSAlockedDetailsVM.lockedFSADetailList);
				dtReport.Columns["state"].SetOrdinal(0);
				dtReport.Columns["jc"].SetOrdinal(1);
				dtReport.Columns["town_name"].SetOrdinal(2);
				dtReport.Columns["town_code"].SetOrdinal(3);
				dtReport.Columns["fsa_id"].SetOrdinal(4);
                dtReport.Columns["modified_on"].SetOrdinal(5);
                dtReport.Columns["modified_by"].SetOrdinal(6);
                dtReport.Columns["state"].ColumnName = "State";
				dtReport.Columns["jc"].ColumnName = "JC";
				dtReport.Columns["town_name"].ColumnName = "Town Name";
				dtReport.Columns["town_code"].ColumnName = "Town Code";
				dtReport.Columns["fsa_id"].ColumnName = "FSA ID";
                dtReport.Columns["modified_on"].ColumnName = "Modified On";
                dtReport.Columns["modified_by"].ColumnName = "Modified By";
                dtReport.Columns.Remove("id");
				dtReport.Columns.Remove("totalRecords");
				dtReport.Columns.Remove("page_count");
				dtReport.Columns.Remove("S_No");
				//dtReport.Columns.Remove("modified_on");
				dtReport.Columns.Remove("FSA_SYSTEM_ID");
				var Exportfilename = ("LockedFSA");
				dtReport.TableName = ("LockedFSA");
				ExportFsaLockedData(dtReport, Exportfilename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
			}
		}
		private void ExportFsaLockedData(DataTable dtReport, string fileName)
		{
			using (var exportData = new MemoryStream())
			{
				Response.Clear();
				if (dtReport != null && dtReport.Rows.Count > 0)
				{
					IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
					workbook.Write(exportData);
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
					Response.BinaryWrite(exportData.ToArray());
					Response.End();
				}
			}
		}
		public IList<KeyValueDropDown> BindSearchBy(TemplateForlockedFSA objTemplateForDropDown)
		{
			List<KeyValueDropDown> items = new List<KeyValueDropDown>();
			items.Add(new KeyValueDropDown { key = "FSA ID", value = "fsa_id" });
			items.Add(new KeyValueDropDown { key = "State", value = "region_name" });
			items.Add(new KeyValueDropDown { key = "JC", value = "province_name" });
			return objTemplateForDropDown.lstBindSearchBy = items.OrderBy(m => m.key).ToList();
		}
	}
}
