using DataAccess.Admin;
using Models;
using Models.Admin;
using Models.WFM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLVendorSpecification
    {
        public List<KeyValueDropDown> GetAllVendorList()
        {
            return new DAVendorSpecification().GetAllVendorList();

        }



        public List<KeyValueDropDown> GetAllEntityList()
        {
            return new DAVendorSpecification().GetAllEntityList();

        }
        public VendorSpecificationMaster SaveVendorSpecification(VendorSpecificationMaster objSaveVendorSpecification)
        {

            return new DAVendorSpecification().SaveVendorSpecification(objSaveVendorSpecification);
        }


        public void SaveSpecificationService(ViewSpecificationServiceList objSaveVendorSpecification)
        {

            new DAVendorSpecification().SaveSpecificationService(objSaveVendorSpecification);
        }

        public IList<ViewVendorSpecificationList> GetVendorSpecificationDetailsList(ViewVendorSpecificationDetailsList model)
        {
            return new DAVendorSpecification().GetVendorSpecificationDetailsList(model);


        }
        public IList<ViewSpecificationServiceList> GetSpecificationServicesDetailsList(ViewSpecificationServiceDetailsList model)
        {
            return new DAVendorSpecification().GetSpecificationServicesDetailsList(model);

        }

        public VendorSpecificationMaster GetVendorSpeicificationDetailsByID(int id, int Vendorid = 0)
        {
            return new DAVendorSpecification().GetVendorSpeicificationDetailsByID(id, Vendorid);
        }

        public ViewSpecificationServiceList GetSpeicificationServiceDetailsByID(int id)
        {
            return new DAVendorSpecification().GetSpeicificationServiceDetailsByID(id);
        }


        public List<VendorSpecificationMaster> GetWCRMaterial(string rfs_type)
        {
            return new DAVendorSpecification().GetWCRMaterial(rfs_type);
        }
        public VendorSpecificationMaster GetItemMasterDetailById(int wcr_id)
        {
            return new DAVendorSpecification().GetItemMasterDetailById(wcr_id);
        }



        public int DeleteVendorSpecificationById(int id)
        {
            return new DAVendorSpecification().DeleteVendorSpecificationById(id);


        }

        public void DeleteSpecificationServiceById(int id)
        {
            new DAVendorSpecification().DeleteSpecificationServiceById(id);
        }


        public int ChkVendorSpecifiationDetailExist(VendorSpecificationMaster objSaveVendorSpecification)
        {
            return new DAVendorSpecification().ChkVendorSpecifiationDetailExist(objSaveVendorSpecification);

        }

        public List<DropDownMaster> BindIOPDetails(string enType, string ddtype = "")
        {
            return new DAVendorSpecification().BindIOPDetails(enType, ddtype);
        }

        public int SaveAttributeDetails(dropdown_master objddmaster_Detail)
        {
            return new DADropDownMaster().SaveAttributeDetails(objddmaster_Detail);
        }


        public int ChkAttributeDetailExist(dropdown_master objddmaster_Detail)
        {
            return new DADropDownMaster().ChkAttributeDetailExist(objddmaster_Detail);

        }


        public dropdown_master getAttributeDetailsById(dropdown_master objddmaster_Detail)
        {
            return new DADropDownMaster().getAttributeDetailsById(objddmaster_Detail);
        }

        public WfmRca GetrcaListbyId(int id)
        {
            return new DARCAMaster().GetrcaListbyId(id);
        }

        public dropdown_master GetDropDownListbyId(int id)
        {
            return new DADropDownMaster().GetDropDownListbyId(id);
        }
        public dropdown_master GetDropDownParentListbyId(int id)
        {
            return new DADropDownMaster().GetDropDownParentListbyId(id);
        }
        public dropdown_master GetParentDropDownListbyId(string Mapping_DrpType)
        {
            return new DADropDownMaster().GetParentDropDownListbyId(Mapping_DrpType);
        }
        public DropdownMasterMapping GetMappingDetailsbyId(int id)
        {
            return new DADropdownMapping().GetMappingDetailsbyId(id);
        }
        public List<dropdown_master> GetDropDownListbyLayerId(int Layer_id, bool IsFilter)
        {
            return new DADropDownMaster().GetDropDownListbyLayerId(Layer_id, IsFilter);
        }
        public List<ParentDropdownMasterMapping> GetPrentDetailsByLayerId(int Layer_id, string dropdown_type)
        {
            return new DADropDownMaster().GetPrentDetailsByLayerId(Layer_id, dropdown_type);
        }

        public List<dropdown_master> GetDropDownListForGridBind(int Layer_id, string FieldName)
        {
            return new DADropDownMaster().GetDropDownListbyLayerId(Layer_id, FieldName);
        }

        public List<RowCountResult> GetDropdownMasterRowCount(int layer_id, string layer_name, string fieldname, string value)
        {
            return new DADropDownMaster().GetDropdownMasterRowCount(layer_id, layer_name, fieldname, value);
        }
        public List<int> DeleteDropdownMasters(int id)
        {
            return new DADropDownMaster().DeleteDropdownMasters(id);

        }

        public List<int> DeleteDropdownMaster(int id)
        {
            return new DADropDownMaster().DeleteDropdownMaster(id);

        }

        public List<int> UpdatercaMaster(int Id, int layer_id, string fieldname, string OldValue, string Value, bool IsVisible, int UserId)
        {
            return new DARCAMaster().UpdatercaMaster(Id, layer_id, fieldname, OldValue, Value, IsVisible, UserId);
        }

        public List<int> UpdateDropdownMaster(int Id, int layer_id, string fieldname, string OldValue, string Value, bool IsVisible, int UserId)
        {
            return new DADropDownMaster().UpdateDropdownMaster(Id, layer_id, fieldname, OldValue, Value, IsVisible, UserId);
        }

        public int SaveDropDownMasterdetails(dropdown_master objAddDropdownMaster)
        {
            return new DADropDownMaster().SaveDropdownMaster(objAddDropdownMaster);
        }

        public int GetrcabyDropdowndetails(int id, string status, string rca)
        {
            return new DARCAMaster().GetrcabyDropdowndetails(id, status, rca);
        }

        public int SaveRCAMaster(WfmRca objAddDropdownMaster)
        {
            return new DARCAMaster().SaveRCAMaster(objAddDropdownMaster);
        }

        public int GetDropDownListbyDropdowndetails(int Layer_id, string FieldName, string Value)
        {
            return new DADropDownMaster().GetDropDownListbyDropdowndetails(Layer_id, FieldName, Value);
        }


        public List<ViewDropDownMasterSetting> GetDropdownMasterSettingsList
            (ViewEntityDropdownMasterSettingsFilter objDropdownMasterSettingsFilter)
        {
            return new DADropDownMaster().GetDropdownMasterSettingsList(objDropdownMasterSettingsFilter);
        }

        public List<RCA_master> GetRCAMasterSettingsList
           (ViewEntityRCAMasterSettingsFilter objDropdownMasterSettingsFilter)
        {
            return new DADropDownMaster().GetRCAMasterSettingsList(objDropdownMasterSettingsFilter);
        }
        public List<WfmRca> GetRCAListbyLayerId(string Status)
        {
            return new DARCAMaster().GetRCAListbyLayerId(Status);
        }

        public List<dropdown_master> GetAllDropdownData()
        {
            return new DADropDownMaster().GetAllDropdownData();
        }
        public List<VendorSpecificationMaster> GetVenderSpecificationByLayer(int layer_id)
        {
            return new DAVendorSpecification().GetVenderSpecificationByLayer(layer_id);
        }
        public List<Dictionary<string, string>> GetVendorSpecificationHistoryDetailById(FilterHistoryAttr objhistoryParam)
        {
            return new DAVendorSpecification().GetVendorSpecificationHistoryDetailById(objhistoryParam);
        }
        public DbMessage validateSpecification(int itemId)
        {
            return new DAVendorSpecification().validateSpecification(itemId);
        }
        public List<VendorSpecificationMaster> GetEntityTemplateDetails(int no_of_ports, string entityType, int vendor_id)
        {
            return new DAVendorSpecification().GetEntityTemplateDetails(no_of_ports, entityType, vendor_id);
        }
        public VendorSpecificationMaster getEntityTemplatebyPortNo(int no_of_ports, string entity_type, int vendor_id)
        {
            return new DAVendorSpecification().getEntityTemplatebyPortNo(no_of_ports, entity_type, vendor_id);
        }
        public List<VendorSpecificationCategory> GetVendorCategory()
        {
            return new DAVendorSpecification().GetVendorCategory();
        }
        public List<KeyValueDropDown> GetItemMasterCode(int layerId)
        {
            return new DAVendorSpecification().GetItemMasterCode(layerId);
        }
        public List<KeyValueDropDown> GetAllItemTypeList()
        {
            return new DAVendorSpecification().GetAllItemTypeList();

        }
        public List<KeyValueDropDown> GetAllUnitMeasurement()
        {
            return new DAVendorSpecification().GetAllUnitMeasurement();
        }
        public void BulkUploadVendorSpecification(List<TempVendorSpecificationMaster> BulkUploadVendorSpecification)
        {
            new DAVendorSpecificationbulk().VendorSpecificationBulkUpload(BulkUploadVendorSpecification);
        }

        public DbMessage UploadVendorSpecificationForInsert(int userID)
        {
            return new DAVendorSpecification().UploadVendorSpecificationForInsert(userID);
        }
        public Tuple<int, int> getTotalUploadBuildingfailureAndSuccess(int UserId)
        {
            return new DAVendorSpecificationbulk().getTotalUploadVendorSpecificationfailureAndSuccess(UserId);
        }
        public void DeleteTempVendorSpecificationData(int UserId)
        {
            new DAVendorSpecificationbulk().DeleteTempVendorSpecificationData(UserId);
        }
        public List<TempVendorSpecificationMaster> GetUploadVendorSpecificationLogs(int userId)
        {
            return new DAVendorSpecificationbulk().GetUploadVendorSpecificationLogs(userId);
        }

        public List<KeyValueDropDown> GetVendorList()
        {
            return new DAVendorSpecification().GetVendorList();

        }
        public List<VendorSpecificationMaster> GetAllVendorSpecifications()
        {
            return new DAVendorSpecification().GetAllVendorSpecifications();

        }
        public List<KeyValueDropDown> GetSpecifyTypeList(int layer_id, string type)
        {
            return new DAVendorSpecification().GetSpecifyTypeList(layer_id, type);

        }
        public List<VendorSpecificationMaster> GetSplitterPortRatio()
        {
            return new DAVendorSpecification().GetSplitterPortRatio();

        }
        public List<KeyValueDropDown> GetVendorOldCost(int id)
        {
            return new DAVendorSpecification().GetVendorOldCost(id);

        }
        public List<KeyValueDropDown> GetCatgegoryReference(int id)
        {
            return new DAVendorSpecification().GetCatgegoryReference(id);

        }
        public List<KeyValueDropDown> GetAllSpecificationList(int layerId)
        {
            return new DAVendorSpecification().GetAllSpecificationList(layerId);

        }

    }
}
