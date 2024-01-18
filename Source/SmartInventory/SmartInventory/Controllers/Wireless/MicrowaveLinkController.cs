using BusinessLogics;
using Models;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SmartInventory.Controllers
{
	public class MicrowaveLinkController : Controller
	{
		// GET: MicrowaveLink
		private readonly Common objCommon;
		private readonly BLMisc objBLMisc;
		private readonly BLCommon objBLCommon;
	

		public MicrowaveLinkController()
		{
			objCommon = new Common();
			objBLMisc = new BLMisc();
			objBLCommon = new BLCommon();
		}

		//LineEntityIn objIn, int pSystemId = 0, string pEntityType = "", string pNetworkId = ""
		public PartialViewResult AddMicrowaveLink(LineEntityIn objIn, string pEntityType, string networkIdType, string geom = "", int systemId = 0, int pSystemId = 0)
		{
			MicrowaveLinkMaster master = new MicrowaveLinkMaster();
			if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
			{
				// get geom by parent id...
				geom = objBLCommon.GetPointTypeParentGeom(pSystemId, pEntityType);
			}
			//MicrowaveLinkViewModel viewModel = new MicrowaveLinkViewModel();
			master = GetDetail(objIn, systemId, EntityType.MicrowaveLink.ToString(), networkIdType, systemId, geom);
			//master = objEntityMaster.microwaveLinkMaster;
			//objEntityMaster.microwaveLinkMaster.pSystemId = pSystemId;
			//objEntityMaster.microwaveLinkMaster.pEntityType = pEntityType;
			BLItemTemplate.Instance.BindItemDropdowns(master, EntityType.MicrowaveLink.ToString());
			BindMicrowaveDropDown(master);

			objBLCommon.fillProjectSpecifications(master);

            objIn.user_id = Convert.ToInt32(Session["user_id"]);
            BLLayer objBLLayer = new BLLayer();
            master.lstUserModule = objBLLayer.GetUserModuleAbbrList(objIn.user_id, UserType.Web.ToString());
            //objEntityMaster.unitValue = objEntityMaster.Antenna_ratio;
            return PartialView("_AddMicrowaveLink", master);
		}
		public MicrowaveLinkMaster GetDetail(LineEntityIn objIn, int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
		{
			//List<MicrowaveLinkViewModel> viewModel = new List<MicrowaveLinkViewModel>();
			MicrowaveLinkMaster objEntity = new MicrowaveLinkMaster();
			 
			objEntity.geom = geom;
			objEntity.networkIdType = networkIdType;
			if (systemId == 0)
			{
				//objEntity.longitude = Convert.ToDouble(geom.Split(' ')[0]);
				//objEntity.latitude = Convert.ToDouble(geom.Split(' ')[1]);
				objEntity.ownership_type = "Own";
				if (pSystemId != 0)
				{
					//EntityType ParentEntityType = (EntityType)Enum.Parse(typeof(EntityType), pEntityType);
					var objParent = objBLMisc.GetEntityDetailById<MicrowaveLinkMaster>(pSystemId, EntityType.MicrowaveLink);
					 
					objEntity.parent_system_id = pSystemId;
					objEntity.parent_entity_type = pEntityType;
				}
				//NEW ENTITY->Fill Region and Province Detail..
				objBLCommon.fillRegionProvinceDetail(objEntity, GeometryType.Line.ToString(), geom);
				//Fill Parent detail...              
				objBLCommon.fillParentDetail(objEntity, new NetworkCodeIn() { eType = EntityType.MicrowaveLink.ToString(), gType = GeometryType.Line.ToString(), eGeom = objEntity.geom, parent_eType = "", parent_sysId = pSystemId }, networkIdType);
				//Item template binding
				//var objItem = BLItemTemplate.Instance.GetTemplateDetail<AntennaMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Antenna);
				//Utility.MiscHelper.CopyMatchingProperties(objItem, objEntity);
				if (objIn.lstTP.Count > 0)
				{
					objEntity.antennaMasterA = objBLMisc.GetEntityDetailById<AntennaMaster>(objIn.lstTP[0].system_id, EntityType.Antenna);
					objEntity.towerMasterA = objBLMisc.GetEntityDetailById<TowerMaster>(objEntity.antennaMasterA.parent_system_id, EntityType.Tower);
					objEntity.tower_a_system_id = objEntity.towerMasterA.system_id;
					objEntity.antenna_a_system_id = objEntity.antennaMasterA.system_id;
					if (objIn.lstTP.Count == 2)
					{
						objEntity.antennaMasterB = objBLMisc.GetEntityDetailById<AntennaMaster>(objIn.lstTP[1].system_id, EntityType.Antenna);
						objEntity.towerMasterB = objBLMisc.GetEntityDetailById<TowerMaster>(objEntity.antennaMasterB.parent_system_id, EntityType.Tower);
						objEntity.tower_b_system_id = objEntity.towerMasterB.system_id;
						objEntity.antenna_b_system_id = objEntity.antennaMasterB.system_id;
					}
				}

			}
			else
			{
 				objEntity = objBLMisc.GetEntityDetailById<MicrowaveLinkMaster>(systemId, EntityType.MicrowaveLink);
				objEntity.antennaMasterA = objBLMisc.GetEntityDetailById<AntennaMaster>(objEntity.antenna_a_system_id, EntityType.Antenna);
				objEntity.antennaMasterB = objBLMisc.GetEntityDetailById<AntennaMaster>(objEntity.antenna_b_system_id, EntityType.Antenna);
				objEntity.towerMasterA = objBLMisc.GetEntityDetailById<TowerMaster>(objEntity.tower_a_system_id, EntityType.Tower);
				objEntity.towerMasterB = objBLMisc.GetEntityDetailById<TowerMaster>(objEntity.tower_b_system_id, EntityType.Tower);
 
			}
			return objEntity;
		}
		public ActionResult SaveMicrowavelink(MicrowaveLinkMaster objEntityMaster, bool isDirectSave = false)
		{
			ModelState.Clear();
			PageMessage objPM = new PageMessage();

			// get parent geometry 
			if (string.IsNullOrWhiteSpace(objEntityMaster.geom) && objEntityMaster.system_id == 0)
			{
				objEntityMaster.geom = objBLCommon.GetPointTypeParentGeom(objEntityMaster.pSystemId, objEntityMaster.pEntityType);
			}

			if (objEntityMaster.networkIdType == NetworkIdType.A.ToString() && objEntityMaster.system_id == 0)
			{
				//GET AUTO NETWORK CODE...
				var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.MicrowaveLink.ToString(), gType = GeometryType.Line.ToString(), eGeom = objEntityMaster.geom, parent_eType = objEntityMaster.pEntityType, parent_sysId = objEntityMaster.pSystemId });
				if (isDirectSave == true)
				{
					//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
					//objEntityMaster = GetAntennaDetail(objEntityMaster.pSystemId, objEntityMaster.pEntityType, objEntityMaster.networkIdType, objEntityMaster.system_id, objEntityMaster.geom);
					// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
					objEntityMaster.network_name = objNetworkCodeDetail.network_code;
					var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
				//	var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
					objEntityMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
					//objEntityMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
				}
				//SET NETWORK CODE
				objEntityMaster.network_id = objNetworkCodeDetail.network_code;
				objEntityMaster.sequence_id = objNetworkCodeDetail.sequence_id;

			}

			//objEntityMaster.microwaveLinkMaster.item_code = objEntityMaster.item_code;
			//objEntityMaster.microwaveLinkMaster.vendor_id = objEntityMaster.vendor_id;
			//objEntityMaster.microwaveLinkMaster.specification = objEntityMaster.specification;
			//objEntityMaster.microwaveLinkMaster.ownership_type = objEntityMaster.ownership_type;



			if (TryValidateModel(objEntityMaster))
			{
				var isNew = objEntityMaster.system_id > 0 ? false : true;

				var resultItem = new BLMicrowave().Save(objEntityMaster, Convert.ToInt32(Session["user_id"]));
				//InstallationInfo installationInfo = objCommon.GetInstallationInfo(objEntityMaster);
				//installationInfo = new BLInstallationInfo().Save(installationInfo);
				if (string.IsNullOrEmpty(resultItem.objPM.message))
				{


					//Save Reference
					//if (objEntityMaster.EntityReference != null && resultItem.system_id > 0)
					//{
					//	objcommon.SaveReference(objEntityMaster.EntityReference, resultItem.system_id);
					//}
					string[] LayerName = { EntityType.MicrowaveLink.ToString() };
					if (isNew)
					{
						objPM.status = ResponseStatus.OK.ToString();
						objPM.isNewEntity = isNew;
						objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
					}
					else
					{
						//if (resultItem.isPortConnected == true)
						//{
						//    objPM.status = ResponseStatus.OK.ToString();
						//    objPM.message = ConvertMultilingual.MultilingualMessageConvert(resultItem.message);//resultItem.message;
						//}
						//else
						//{
						objPM.status = ResponseStatus.OK.ToString();
						objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						//}
					}
					objEntityMaster.objPM = objPM;
				}
			}
			else
			{

				objPM.status = ResponseStatus.FAILED.ToString();
				objPM.message = new LibraryController().getFirstErrorFromModelState();
				objEntityMaster.objPM = objPM;
			}
			if (isDirectSave == true)
			{
				//RETURN MESSAGE AS JSON FOR DIRECT SAVE
				return Json(objEntityMaster.objPM, JsonRequestBehavior.AllowGet);
			}
			else
			{
				BLItemTemplate.Instance.BindItemDropdowns(objEntityMaster, EntityType.MicrowaveLink.ToString());
				BindMicrowaveDropDown(objEntityMaster);
				// RETURN PARTIAL VIEW WITH MODEL DATA
				objBLCommon.fillProjectSpecifications(objEntityMaster);
                objEntityMaster.user_id = Convert.ToInt32(Session["user_id"]);
                BLLayer objBLLayer = new BLLayer();
                objEntityMaster.lstUserModule = objBLLayer.GetUserModuleAbbrList(objEntityMaster.user_id, UserType.Web.ToString());
                return PartialView("_AddMicrowaveLink", objEntityMaster);
			}
		}
		public void BindMicrowaveDropDown(MicrowaveLinkMaster objEntityMaster)
		{
			var objDDL = objBLMisc.GetDropDownList(EntityType.MicrowaveLink.ToString());
			objEntityMaster.LstLinkType = objDDL.Where(x => x.dropdown_type == DropDownType.LinkType.ToString()).ToList();
			//objEntityMaster.lstSubAntennaType = objDDL.Where(x => x.dropdown_type == DropDownType.Sub_Antenna_Type.ToString()).ToList();
			//objEntityMaster.lstAntennaOperator = objDDL.Where(x => x.dropdown_type == DropDownType.Antenna_Operator.ToString()).ToList();
			//objEntityMaster.lstUsePattern = objDDL.Where(x => x.dropdown_type == DropDownType.Use_Pattern.ToString()).ToList();
			objEntityMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objEntityMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//objEntityMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
	}
}