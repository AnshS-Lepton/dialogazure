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
    public class AntennaController : Controller
    {
        // GET: Antenna
        private readonly Common objcommon;
        private readonly BLMisc objBLMisc;
        private readonly BLCommon objBLCommon;

        public AntennaController()
        {
            objcommon = new Common();
            objBLMisc = new BLMisc();
            objBLCommon = new BLCommon();
        }

        public PartialViewResult AddAntenna(string pEntityType, string networkIdType, string geom = "", int systemId = 0, int pSystemId = 0)
        {
            var maximum_gain = 0.00;
            if (string.IsNullOrWhiteSpace(geom) && systemId == 0)
            {
                // get geom by parent id...
                maximum_gain = 1.5;
                geom = objBLCommon.GetPointTypeParentGeom(pSystemId, pEntityType);
            }
            AntennaMaster objEntityMaster = GetDetail(pSystemId, pEntityType, networkIdType, systemId, geom);
            objEntityMaster.pSystemId = pSystemId;
            objEntityMaster.pEntityType = pEntityType;
            objEntityMaster.maximum_gain = objEntityMaster.maximum_gain == 0 ? maximum_gain : objEntityMaster.maximum_gain;
            objEntityMaster.co_polor_horizontal_maximum_gain = objEntityMaster.co_polor_horizontal_maximum_gain == 0 ? maximum_gain : objEntityMaster.co_polor_horizontal_maximum_gain;
            objEntityMaster.co_polor_vertical_maximum_gain = objEntityMaster.co_polor_vertical_maximum_gain == 0 ? maximum_gain : objEntityMaster.co_polor_vertical_maximum_gain;
            objEntityMaster.cross_polor_horizontal_maximum_gain = objEntityMaster.cross_polor_horizontal_maximum_gain == 0 ? maximum_gain : objEntityMaster.cross_polor_horizontal_maximum_gain;
            objEntityMaster.cross_polor_vertical_maximum_gain = objEntityMaster.cross_polor_vertical_maximum_gain == 0 ? maximum_gain : objEntityMaster.cross_polor_vertical_maximum_gain;

            BLItemTemplate.Instance.BindItemDropdowns(objEntityMaster, EntityType.Antenna.ToString());
            BindAntennaDropDown(objEntityMaster);
            objEntityMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
            objBLCommon.fillProjectSpecifications(objEntityMaster);
            //objEntityMaster.unitValue = objEntityMaster.Antenna_ratio;
            BLLayer objBLLayer = new BLLayer();
            var usrDetail = (User)Session["userDetail"];
            var usrId = usrDetail.user_id;
            objEntityMaster.lstUserModule = objBLLayer.GetUserModuleAbbrList(usrId, UserType.Web.ToString());
            return PartialView("_AddAntenna", objEntityMaster);
        }
        public AntennaMaster GetDetail(int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
        {

            AntennaMaster objEntity = new AntennaMaster();
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
                objBLCommon.fillParentDetail(objEntity, new NetworkCodeIn() { eType = EntityType.Antenna.ToString(), gType = GeometryType.Point.ToString(), eGeom = objEntity.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType);
                //Item template binding
                var layerDetails = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.Antenna.ToString().ToUpper()).FirstOrDefault();
                if (layerDetails.is_template_required)
                {
                    var objItem = BLItemTemplate.Instance.GetTemplateDetail<AntennaTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Antenna);
                    Utility.MiscHelper.CopyMatchingProperties(objItem, objEntity);
                }
            }
            else
            {
                objEntity = objBLMisc.GetEntityDetailById<AntennaMaster>(systemId, EntityType.Antenna);
            }
            return objEntity;
        }
        public ActionResult SaveAntenna(AntennaMaster objEntityMaster, bool isDirectSave = false)
        {
            ModelState.Clear();
            PageMessage objPM = new PageMessage();
            var layerDetail = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.Antenna.ToString().ToUpper()).FirstOrDefault() : null;


            // get parent geometry 
            if (string.IsNullOrWhiteSpace(objEntityMaster.geom) && objEntityMaster.system_id == 0)
            {
                objEntityMaster.geom = objBLCommon.GetPointTypeParentGeom(objEntityMaster.pSystemId, objEntityMaster.pEntityType);
            }

            if (objEntityMaster.networkIdType == NetworkIdType.A.ToString() && objEntityMaster.system_id == 0)
            {
                //GET AUTO NETWORK CODE...
                var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Antenna.ToString(), gType = GeometryType.Point.ToString(), eGeom = objEntityMaster.geom, parent_eType = objEntityMaster.pEntityType, parent_sysId = objEntityMaster.pSystemId });
                if (isDirectSave == true)
                {
                    //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                    //objEntityMaster = GetAntennaDetail(objEntityMaster.pSystemId, objEntityMaster.pEntityType, objEntityMaster.networkIdType, objEntityMaster.system_id, objEntityMaster.geom);
                    // INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
                    objEntityMaster.network_name = objNetworkCodeDetail.network_code;
                    var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
                   // var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
                    objEntityMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
                    //objEntityMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
                }
                //SET NETWORK CODE
                objEntityMaster.network_id = objNetworkCodeDetail.network_code;
                objEntityMaster.sequence_id = objNetworkCodeDetail.sequence_id;
            }



            if (TryValidateModel(objEntityMaster))
            {
                var isNew = objEntityMaster.system_id > 0 ? false : true;

                 var resultItem = new BLAntenna().SaveAntennaEntity(objEntityMaster, Convert.ToInt32(Session["user_id"]));

                if (String.IsNullOrEmpty(resultItem.objPM.message))
                {


                    //Save Reference
                    if (objEntityMaster.EntityReference != null && resultItem.system_id > 0)
                    {
                        objBLCommon.SaveReference(objEntityMaster.EntityReference, resultItem.system_id);
                    }
                    if (layerDetail.is_vsat_enabled && objEntityMaster.objVSATAntenna.is_vsat_updated && resultItem.system_id > 0)
                    {
                        SaveVsatAntenna(objEntityMaster);
                    }
                    string[] LayerName = { EntityType.Antenna.ToString() };
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
                BLItemTemplate.Instance.BindItemDropdowns(objEntityMaster, EntityType.Antenna.ToString());
                BindAntennaDropDown(objEntityMaster);
                // RETURN PARTIAL VIEW WITH MODEL DATA
                objBLCommon.fillProjectSpecifications(objEntityMaster);
                objEntityMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();

                return PartialView("_AddAntenna", objEntityMaster);
            }
        }
        public void BindAntennaDropDown(AntennaMaster objEntityMaster)
        {
            var objDDL = objBLMisc.GetDropDownList(EntityType.Antenna.ToString());
            objEntityMaster.lstAntennaType= objDDL.Where(x => x.dropdown_type == DropDownType.Antenna_Type.ToString()).ToList();
            objEntityMaster.lstSubAntennaType= objDDL.Where(x => x.dropdown_type == DropDownType.Antenna_Sub_Type.ToString()).ToList();
            objEntityMaster.lstAntennaOperator = objDDL.Where(x => x.dropdown_type == DropDownType.Antenna_Operator.ToString()).ToList();
            objEntityMaster.lstUsePattern = objDDL.Where(x => x.dropdown_type == DropDownType.Use_Pattern.ToString()).ToList();
            objEntityMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            var _objDDL = new BLMisc().GetDropDownList("");
            objEntityMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
           // objEntityMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
        }

        public PartialViewResult AddVSATAntenna(int SystemId,bool is_vsat_updated=false)
        {
            AntennaMaster objAM = new AntennaMaster();
            var objVsatAntenna = new BLAntenna().GetVsatAntennaById(SystemId);
            if (objVsatAntenna != null)
                objAM.objVSATAntenna = objVsatAntenna;
           // objAM.objVSATAntenna.operational_from_date_bs = Dat.Parse(MiscHelper.FormatDateTime(objAM.objVSATAntenna.operational_from_date_bs.ToString()));
           // objAM.objVSATAntenna.operational_to_date_bs = DateTime.Parse(MiscHelper.FormatDateTime(objAM.objVSATAntenna.operational_to_date_bs.ToString()));

            objAM.objVSATAntenna.is_vsat_updated = is_vsat_updated;
                BindVSATDropDown(objAM);
            return PartialView("_AddVSATAntenna", objAM);
        }
        private void BindVSATDropDown(AntennaMaster objAM)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.VSAT.ToString());
            objAM.objVSATAntenna.VSATSetellite = objDDL.Where(x => x.dropdown_type == DropDownType.VSAT_Setellite_Type.ToString()).ToList();
            objAM.objVSATAntenna.VSATTranspond = objDDL.Where(x => x.dropdown_type == DropDownType.VSAT_Transpond_Type.ToString()).ToList();
            objAM.objVSATAntenna.VSATRFBand = objDDL.Where(x => x.dropdown_type == DropDownType.VSAT_RF_Band_Type.ToString()).ToList();

        }
        private void SaveVsatAntenna(AntennaMaster objAM)
        {
            var Building_System_Id = new BLAntenna().GetBuildingSystemId(objAM.system_id);
            objAM.objVSATAntenna.created_by = objAM.created_by;
            objAM.objVSATAntenna.parent_system_id = objAM.system_id;
            objAM.objVSATAntenna.parent_entity_type = objAM.entityType;
            objAM.objVSATAntenna.parent_network_id = objAM.network_id;
            objAM.objVSATAntenna.building_system_id = Building_System_Id;
            var status = new BLAntenna().SaveVsatAntenna(objAM.objVSATAntenna, Convert.ToInt32(Session["user_id"]));
        }
    }
}