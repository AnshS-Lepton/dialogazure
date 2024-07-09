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
    public class SectorController : Controller
    {
        // GET: Sector
        private readonly Common objcommon;
        private readonly BLMisc objBLMisc;
        private readonly BLCommon objBLCommon;
        public SectorController()
        {
            objcommon = new Common();
            objBLMisc = new BLMisc();
            objBLCommon = new BLCommon();
        }

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult AddSector(string pEntityType, string networkIdType, string geom = "", int systemId = 0, int pSystemId = 0)
        {
            if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            {
                // get geom by parent id...
                geom = objBLCommon.GetPointTypeParentGeom(pSystemId, pEntityType);
            }
            SectorMaster objEntityMaster = GetDetail(pSystemId, pEntityType, networkIdType, systemId, geom);
            objEntityMaster.pSystemId = pSystemId;
            objEntityMaster.pEntityType = pEntityType;
            BLItemTemplate.Instance.BindItemDropdowns(objEntityMaster, EntityType.Sector.ToString());
            BindSectorDropDown(objEntityMaster);
            objBLCommon.fillProjectSpecifications(objEntityMaster);
            //objEntityMaster.unitValue = objEntityMaster.splitter_ratio;
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;
            BLLayer objBLLayer = new BLLayer();
            objEntityMaster.lstUserModule = objBLLayer.GetUserModuleAbbrList(usrId, UserType.Web.ToString());
            return PartialView("_AddSector", objEntityMaster);
        }

        public SectorMaster GetDetail(int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
        {

            SectorMaster objEntity = new SectorMaster();
            objEntity.geom = geom;
            objEntity.networkIdType = networkIdType;
            if (systemId == 0)
            {
                objEntity.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objEntity.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objEntity.ownership_type = "Own";
                if (pSystemId != 0)
                {
                    //EntityType ParentEntityType = (EntityType)Enum.Parse(typeof(EntityType), pEntityType);
                    var objParent = objBLMisc.GetEntityDetailById<TowerMaster>(pSystemId, EntityType.Tower);
                    objEntity.parent_network_id = objParent.network_id;
                    objEntity.parent_system_id = pSystemId;
                    objEntity.parent_entity_type = pEntityType;
                }
                //NEW ENTITY->Fill Region and Province Detail..
                objBLCommon.fillRegionProvinceDetail(objEntity, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                objBLCommon.fillParentDetail(objEntity, new NetworkCodeIn() { eType = EntityType.Sector.ToString(), gType = GeometryType.Point.ToString(), eGeom = objEntity.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType);
                //Item template binding
                var layerDetails = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.Antenna.ToString().ToUpper()).FirstOrDefault();
                if (layerDetails.is_template_required)
                {
                    var objItem = BLItemTemplate.Instance.GetTemplateDetail<SectorTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Sector);
                    Utility.MiscHelper.CopyMatchingProperties(objItem, objEntity);
                }
            }
            else
            {
                objEntity = objBLMisc.GetEntityDetailById<SectorMaster>(systemId, EntityType.Sector);
            }
            return objEntity;
        }

        public void BindSectorDropDown(SectorMaster objEntityMaster)
        {
            var objDDL = objBLMisc.GetDropDownListJson(EntityType.Sector.ToString());
            objEntityMaster.lstDownlink = objDDL.Where(x => x.dropdown_type == DropDownType.Downlink.ToString()).ToList();
            objEntityMaster.lstUplink = objDDL.Where(x => x.dropdown_type == DropDownType.Uplink.ToString()).ToList();
            objEntityMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objEntityMaster.lstSectorType = objDDL.Where(x => x.dropdown_type == DropDownType.SectorType.ToString()).ToList();
            objEntityMaster.lstFrequencyType = objDDL.Where(x => x.dropdown_type == DropDownType.Frequency.ToString()).ToList();

		}

        public ActionResult SaveSector(SectorMaster objEntityMaster, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            objEntityMaster.geom = Common.GetSectorsGeometry(objEntityMaster.latitude, objEntityMaster.longitude, objEntityMaster.azimuth, objEntityMaster.sector_type);
            // get parent geometry 
            if (string.IsNullOrWhiteSpace(objEntityMaster.geom) && objEntityMaster.system_id == 0)
            {
                objEntityMaster.geom = objBLCommon.GetPointTypeParentGeom(objEntityMaster.pSystemId, objEntityMaster.pEntityType);
            }

            if (objEntityMaster.networkIdType == NetworkIdType.A.ToString() && objEntityMaster.system_id == 0)
            {
                //GET AUTO NETWORK CODE...
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Sector.ToString(), gType = GeometryType.Point.ToString(), eGeom = objEntityMaster.geom, parent_eType = objEntityMaster.pEntityType, parent_sysId = objEntityMaster.pSystemId });
                if (isDirectSave == true)
                {
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    //objEntityMaster = GetSectorDetail(objEntityMaster.pSystemId, objEntityMaster.pEntityType, objEntityMaster.networkIdType, objEntityMaster.system_id, objEntityMaster.geom);
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    objEntityMaster.network_name = objNetworkCodeDetail.network_code;
                }
                //SET NETWORK CODE
                objEntityMaster.network_id = objNetworkCodeDetail.network_code;
                objEntityMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            }
			if (string.IsNullOrEmpty(objEntityMaster.network_name))
			{
				objEntityMaster.network_name = objEntityMaster.network_id;
			}



			if (TryValidateModel(objEntityMaster))
            {
                var isNew = objEntityMaster.system_id > 0 ? false : true;

                var resultItem = new BLSector().SaveSectorEntity(objEntityMaster, Convert.ToInt32(Session["user_id"]));
                //InstallationInfo installationInfo = objcommon.GetInstallationInfo(objEntityMaster);
                //installationInfo = new BLInstallationInfo().Save(installationInfo);
                if (String.IsNullOrEmpty(resultItem.objPM.message))
                {


                    //Save Reference
                    if (objEntityMaster.EntityReference != null && resultItem.system_id > 0)
                    {
                        objBLCommon.SaveReference(objEntityMaster.EntityReference, resultItem.system_id);
                    }
                    string[] LayerName = { EntityType.Sector.ToString() };
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
                BLItemTemplate.Instance.BindItemDropdowns(objEntityMaster, EntityType.Sector.ToString());
                BindSectorDropDown(objEntityMaster);
                // RETURN PARTIAL VIEW WITH MODEL DATA
                objBLCommon.fillProjectSpecifications(objEntityMaster);
                var usrDetail = (User)Session["userDetail"];
                var usrId = usrDetail.user_id;
                BLLayer objBLLayer = new BLLayer();
                objEntityMaster.lstUserModule = objBLLayer.GetUserModuleAbbrList(usrId, UserType.Web.ToString());
                return PartialView("_AddSector", objEntityMaster);
            }
        }

        [HttpGet]
        public JsonResult GetDropDownListJson(string layerName, string ID = "")
        {
            var availablePorts = objBLMisc.GetDropDownListParent(layerName, ID);
            return Json(availablePorts, JsonRequestBehavior.AllowGet);
        }
    }

}
