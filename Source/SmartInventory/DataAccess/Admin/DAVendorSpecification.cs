using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using Models.WFM;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mono.Security.X509.X520;


namespace DataAccess.Admin
{
    public class DAVendorSpecification : Repository<VendorSpecificationMaster>
    {


        public VendorSpecificationMaster SaveVendorSpecification(VendorSpecificationMaster objVendorSpecification)
        {
            try
            {
                if (objVendorSpecification.id != 0)
                {

                    objVendorSpecification.modified_by = objVendorSpecification.user_id;
                    objVendorSpecification.modified_on = DateTimeHelper.Now;
                    return repo.Update(objVendorSpecification);

                }

                else
                {
                    objVendorSpecification.created_by = objVendorSpecification.user_id;
                    objVendorSpecification.created_on = DateTimeHelper.Now;
                    //objVendorSpecification.modified_by = objVendorSpecification.user_id;
                    //objVendorSpecification.modified_on = DateTimeHelper.Now;
                    return repo.Insert(objVendorSpecification);
                }

            }

            catch { throw; }

        }

        public void SaveSpecificationService(ViewSpecificationServiceList objVendorSpecification)
        {
            try
            {
                if (objVendorSpecification.id != 0)
                {

                    objVendorSpecification.modified_on = DateTimeHelper.Now;

                    repo.ExecuteProcedure<bool>("fn_update_specification_service_data", new
                    {
                        ids = Convert.ToInt32(objVendorSpecification.id),
                        layer_ids = Convert.ToInt32(objVendorSpecification.layer_id),
                        item_template_ids = Convert.ToInt32(objVendorSpecification.specification),
                        service_names = (objVendorSpecification.service_name).ToString(),
                        service_costs = Convert.ToDouble(objVendorSpecification.service_cost),
                        modified_bys = Convert.ToInt32(objVendorSpecification.created_by),
                        modified_ons = objVendorSpecification.modified_on,
                        is_actives = Convert.ToBoolean(objVendorSpecification.is_active)
                    }, false);

                }

                else
                {

                    repo.ExecuteProcedure<bool>("fn_insert_specification_service_data", new
                    {
                        layer_ids = Convert.ToInt32(objVendorSpecification.layer_id),
                        item_template_ids = Convert.ToInt32(objVendorSpecification.specification),
                        service_names = (objVendorSpecification.service_name).ToString(),
                        service_costs = Convert.ToDouble(objVendorSpecification.service_cost),
                        created_bys = Convert.ToInt32(objVendorSpecification.created_by),
                        is_actives = Convert.ToBoolean(objVendorSpecification.is_active)
                    }, false);
                }

            }

            catch { throw; }

        }

        public List<ViewSpecificationServiceList> GetSpecificationServicesDetailsList(ViewSpecificationServiceDetailsList model)
        {
            try
            {
                var res = repo.ExecuteProcedure<ViewSpecificationServiceList>("fn_get_specification_service_details", new
                {
                    searchby = Convert.ToString(model.viewSpecificationServiceDetail.searchBy),
                    searchbyText = Convert.ToString(model.viewSpecificationServiceDetail.searchText),
                    P_PAGENO = model.viewSpecificationServiceDetail.currentPage,
                    P_PAGERECORD = model.viewSpecificationServiceDetail.pageSize,
                    P_SORTCOLNAME = model.viewSpecificationServiceDetail.sort,
                    P_SORTTYPE = model.viewSpecificationServiceDetail.orderBy,
                    P_TOTALRECORDS = model.viewSpecificationServiceDetail.totalRecord,
                    P_RECORDLIST = 0,
                    is_active = model.viewSpecificationServiceDetail.is_active
                }, true);

                return res;
            }
            catch { throw; }
        }

        public List<ViewVendorSpecificationList> GetVendorSpecificationDetailsList(ViewVendorSpecificationDetailsList model)
        {
            try
            {
                var res = repo.ExecuteProcedure<ViewVendorSpecificationList>("fn_get_vendor_specification_details", new
                {
                    searchby = Convert.ToString(model.viewVendorSpecificationDetail.searchBy),
                    searchbyText = Convert.ToString(model.viewVendorSpecificationDetail.searchText),
                    P_PAGENO = model.viewVendorSpecificationDetail.currentPage,
                    P_PAGERECORD = model.viewVendorSpecificationDetail.pageSize,
                    P_SORTCOLNAME = model.viewVendorSpecificationDetail.sort,
                    P_SORTTYPE = model.viewVendorSpecificationDetail.orderBy,
                    P_TOTALRECORDS = model.viewVendorSpecificationDetail.totalRecord,
                    P_RECORDLIST = 0,
                    is_active = model.viewVendorSpecificationDetail.is_active
                }, true);

                return res;
            }
            catch { throw; }
        }


        public List<KeyValueDropDown> GetAllVendorList()
        {
            try
            {
                var lstVendors = repo.ExecuteProcedure<KeyValueDropDown>("fn_get_allvendor_list", new { }, true);
                return lstVendors != null ? lstVendors.OrderBy(m => m.key).ToList() : new List<KeyValueDropDown>();
            }
            catch { throw; }
        }


        public List<KeyValueDropDown> GetAllEntityList()
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_allentity_list", new { }, true);


            }
            catch { throw; }

        }

        public ViewSpecificationServiceList GetSpeicificationServiceDetailsByID(int id)
        {
            try
            {
                var res = repo.ExecuteProcedure<ViewSpecificationServiceList>("fn_get_spec_service_byid", new { ids = id }, true);
                return res != null && res.Count > 0 ? res[0] : new ViewSpecificationServiceList();
            }
            catch
            { throw; }
        }

        public VendorSpecificationMaster GetVendorSpeicificationDetailsByID(int id, int Vendorid = 0)
        {
            return Vendorid != 0 ? repo.Get(m => m.vendor_id == Vendorid) : repo.Get(m => m.id == id);
        }
        public List<VendorSpecificationMaster> ItemVendorCost(CommonGridAttr objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<VendorSpecificationMaster>("fn_get_item_vendor_cost", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    P_TOTALRECORDS = objGridAttributes.totalRecord,
                }, true);
            }
            catch { throw; }
        }
        

        public VendorSpecificationMaster GetItemMasterDetailById(int wcr_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<VendorSpecificationMaster>("fn_get_item_template_master_byId",
                    new
                    {
                        p_item_template_id = wcr_id

                    }, true);
                return lst != null && lst.Count > 0 ? lst[0] : new VendorSpecificationMaster();
            }
            catch { throw; }
        }

        public List<VendorSpecificationMaster> GetWCRMaterial(string rfs_type)
        {
            try
            {
                //List<VendorSpecificationMaster> result = new List<VendorSpecificationMaster>();
                //if (rfs_type == "A-RFS")
                //{
                //    result = repo.GetAll(x => x.item_type == "WCR" && x.is_arfs == true).ToList();
                //    return result != null && result.Count > 0 ? result : new List<VendorSpecificationMaster>();
                //}
                //else if (rfs_type == "B-RFS")
                //{
                //    result = repo.GetAll(x => x.item_type == "WCR" && x.is_brfs == true).ToList();
                //    return result != null && result.Count > 0 ? result : new List<VendorSpecificationMaster>();

                //}
                //else if (rfs_type == "C-RFS")
                //{
                //    result = repo.GetAll(x => x.item_type == "WCR" && x.is_crfs == true).ToList();
                //    return result != null && result.Count > 0 ? result : new List<VendorSpecificationMaster>();
                //}
                //return result;
                var lst = repo.ExecuteProcedure<VendorSpecificationMaster>("fn_get_entity_wcr_matrial_by_rfs_type",
                   new
                   {
                       p_rfs_type = rfs_type

                   }, true);
                return lst != null ? lst.OrderBy(x => x.id).ToList() : new List<VendorSpecificationMaster>();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<VendorSpecificationMaster> GetVenderSpecificationByLayer(int layer_id)
        {
            try
            {
                List<VendorSpecificationMaster> result = new List<VendorSpecificationMaster>();
                result = repo.GetAll(x => x.layer_id == layer_id && x.is_active).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<VendorSpecificationMaster> GetEntityTemplateDetails(int no_of_ports, string entityType, int vendor_id)
        {
            try
            {
                //List<VendorSpecificationMaster> result = new List<VendorSpecificationMaster>();
                var result = repo.GetAll(x => x.no_of_port == no_of_ports && x.vendor_id == vendor_id && x.category_reference.ToUpper() == entityType.ToUpper() && x.is_active == true).ToList();
                return result != null && result.Count > 0 ? result : new List<VendorSpecificationMaster>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteVendorSpecificationById(int id)
        {
            try
            {
                var objSystmId = repo.Get(x => x.id == id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }

        public void DeleteSpecificationServiceById(int id)
        {

            try
            {
                repo.ExecuteProcedure<bool>("fn_delete_specification_service_data", new { ids = id });
            }
            catch { throw; }

        }
        public VendorSpecificationMaster getEntityTemplatebyPortNo(int no_of_ports, string entity_type, int vendor_id)
        {
            try
            {
                var result = repo.GetAll(x => x.no_of_port == no_of_ports && x.category_reference.ToUpper() == entity_type.ToUpper() && x.vendor_id == vendor_id).FirstOrDefault();
                return result != null ? result : new VendorSpecificationMaster();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public int ChkVendorSpecifiationDetailExist(VendorSpecificationMaster objSaveVendorSpecification)
        {
            try
            {
                var res = repo.Get(u => u.layer_id == objSaveVendorSpecification.layer_id && u.code.Trim().ToLower() == objSaveVendorSpecification.code.Trim().ToLower() && u.vendor_id == objSaveVendorSpecification.vendor_id && u.id != objSaveVendorSpecification.id);
                return res != null ? 1 : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        DAMisc objDAMisc = new DAMisc();
        public List<DropDownMaster> BindIOPDetails(string enType, string ddType = "")
        {
            return objDAMisc.GetDropDownList(enType, ddType);
        }

        public List<Dictionary<string, string>> GetVendorSpecificationHistoryDetailById(FilterHistoryAttr objhistoryParam)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_vendorspecificationhistory_by_id",
                    new
                    {
                        P_PAGENO = objhistoryParam.currentPage,
                        P_PAGERECORD = objhistoryParam.pageSize,
                        P_SORTCOLNAME = objhistoryParam.sort,
                        P_SORTTYPE = objhistoryParam.orderBy,
                        p_systemid = objhistoryParam.systemid,
                        p_entity_name = objhistoryParam.entityType
                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public DbMessage validateSpecification(int itemId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_validate_specification",
                    new
                    {
                        p_template_id = itemId
                    }, false).FirstOrDefault();

            }
            catch { throw; }
        }
        public List<VendorSpecificationCategory> GetVendorCategory()
        {
            try
            {
                return repo.ExecuteProcedure<VendorSpecificationCategory>("fn_get_vendor_category", new { }, true).ToList();
            }
            catch { throw; }
        }
        public List<KeyValueDropDown> GetItemMasterCode(int layerId)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_vendor_spec_item_code", new { p_layer_id = layerId }).ToList();
            }
            catch (Exception ex) { throw ex; }
        }
        public List<KeyValueDropDown> GetItemVendorCode(int layerId, string specification)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_item_vendor_spec_item_code", new { p_layer_id = layerId, p_specification = specification }).ToList();
            }
            catch (Exception ex) { throw ex; }
        }
        public List<IvcKeyValueDropDown> GetVendorItemCategory()
        {
            try
            {
                return repo.ExecuteProcedure<IvcKeyValueDropDown>("fn_item_vendor_spec_item_category", new { }).ToList();
            }
            catch (Exception ex) { throw ex; }
        }
        public List<KeyValueDropDown> GetItemSpec(int layerId)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_vendor_spec_item_specification", new { p_layer_id = layerId }).ToList();
            }
            catch (Exception ex) { throw ex; }
        }
        public List<KeyValueDropDown> GetAllItemTypeList()
        {
            try
            {
                var listItem_type = repo.ExecuteProcedure<KeyValueDropDown>("fn_get_allItem_type_list", new { }, true);
                return listItem_type != null ? listItem_type.OrderBy(m => m.key).ToList() : new List<KeyValueDropDown>();
            }
            catch { throw; }
        }

        public List<KeyValueDropDown> GetAllUnitMeasurement()
        {
            try
            {
                var listUnit_measument_type = repo.ExecuteProcedure<KeyValueDropDown>("fn_get_allUnit_measurement_list", new { }, true);
                return listUnit_measument_type != null ? listUnit_measument_type.OrderBy(m => m.key).ToList() : new List<KeyValueDropDown>();
            }
            catch { throw; }
        }
        public string GetUOM(int layer_id, string specification, string ItemCode)
        {
            try
            {
                return repo.GetAll(m => m.layer_id == layer_id && m.specification== specification && m.code== ItemCode).Select(a=>a.unit_measurement).FirstOrDefault();
              
            }
            catch { throw; }
        }



        public DbMessage UploadVendorSpecificationForInsert(int userID)
        {
            try
            {
                // return repo.ExecuteProcedure<DbMessage>("fn_bulk_upload_VendorSpecifation_insert", new { P_UserId = userID }).FirstOrDefault();
                return repo.ExecuteProcedure<DbMessage>("fn_bulk_upload_vendorspecifation_insert", new { P_UserId = userID }).FirstOrDefault();
            }
            catch { throw; }
        }

        public List<KeyValueDropDown> GetVendorList()
        {
            try
            {
                var lstVendors = repo.ExecuteProcedure<KeyValueDropDown>("fn_get_vendor_list", new { }, true);
                return lstVendors != null ? lstVendors.OrderBy(m => m.key).ToList() : new List<KeyValueDropDown>();
            }
            catch { throw; }
        }

        public List<VendorSpecificationMaster> GetAllVendorSpecifications()
        {
            try
            {
                return repo.GetAll(m => m.is_active == true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<KeyValueDropDown> GetSpecifyTypeList(int layer_id, string type)
        {
            try
            {
                var lstSpecifyTypes = repo.ExecuteProcedure<KeyValueDropDown>("fn_get_specify_type_list", new { P_layer_id = layer_id, P_type = type }, true);
                return lstSpecifyTypes != null ? lstSpecifyTypes.OrderBy(m => m.key).ToList() : new List<KeyValueDropDown>();
            }
            catch { throw; }
        }

        public List<VendorSpecificationMaster> GetSplitterPortRatio()
        {
            try
            {
                return repo.GetAll(m => m.is_default == true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<KeyValueDropDown> GetVendorOldCost(int id)
        {
            try
            {
                var lstSpecifyTypes = repo.ExecuteProcedure<KeyValueDropDown>("fn_get_item_cost", new { P_id = id }, true);
                return lstSpecifyTypes;
            }
            catch { throw; }

        }
        public List<KeyValueDropDown> GetCatgegoryReference(int id)
        {
            try
            {
                var lstSpecifyTypes = repo.ExecuteProcedure<KeyValueDropDown>("fn_get_item_category", new { P_id = id }, true);
                return lstSpecifyTypes;
            }
            catch { throw; }

        }
        public List<KeyValueDropDown> GetAllSpecificationList(int layerId)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_allspecification_list", new { p_layer_id = layerId }).ToList();
            }
            catch (Exception ex) { throw ex; }

        }


    }
    public class DADropDownMaster : Repository<dropdown_master>
    {
        public int SaveAttributeDetails(dropdown_master objddmaster_Detail)
        {
            try
            {
                //if (!string.IsNullOrEmpty(objddmaster_Detail.dropdown_value))
                //if (objddmaster_Detail.id !=0)
                //{

                //    objddmaster_Detail.modified_by = objddmaster_Detail.created_by;
                //    objddmaster_Detail.modified_on = DateTimeHelper.Now;
                //    objddmaster_Detail.dropdown_status = true;

                //    repo.Update(objddmaster_Detail);

                //    return 1;

                //}

                //else
                {
                    objddmaster_Detail.created_by = objddmaster_Detail.created_by;
                    objddmaster_Detail.modified_by = objddmaster_Detail.created_by;
                    objddmaster_Detail.modified_on = DateTimeHelper.Now;
                    objddmaster_Detail.dropdown_status = true;

                    repo.Insert(objddmaster_Detail);
                    return 0;

                }

            }
            catch { throw; }
        }

        public int ChkAttributeDetailExist(dropdown_master objddmaster_Detail)
        {
            try
            {
                var res = repo.Get(u => u.dropdown_value.Trim().ToLower() == objddmaster_Detail.dropdown_value.Trim().ToLower() && u.layer_id == objddmaster_Detail.layer_id);
                return res != null ? 1 : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public dropdown_master getAttributeDetailsById(dropdown_master objddmaster_Detail)
        {
            var objgetAttributeDetailsById = repo.Get(m => m.dropdown_value == objddmaster_Detail.dropdown_value && m.layer_id == objddmaster_Detail.layer_id);

            if (objddmaster_Detail.entity_name.ToLower() == "splitter")
            {

                objgetAttributeDetailsById.inputport = Convert.ToInt32(objddmaster_Detail.dropdown_value.Split(':')[0]);
                objgetAttributeDetailsById.outputport = Convert.ToInt32(objddmaster_Detail.dropdown_value.Split(':')[1]);
            }


            return objgetAttributeDetailsById;

        }



        public List<dropdown_master> GetDropDownList()
        {
            return (List<dropdown_master>)repo.GetAll(m => m.layer_id == 0);
        }

        public dropdown_master GetDropDownListbyId(int id)
        {
            return (dropdown_master)repo.Get(m => m.id == id);
        }
        public dropdown_master GetDropDownParentListbyId(int id)
        {
            return (dropdown_master)repo.Get(m => m.id == id);
        }
        public dropdown_master GetParentDropDownListbyId(string Mapping_DrpType)
        {
            return (dropdown_master)repo.Get(m => m.dropdown_type == Mapping_DrpType);
        }


        public List<dropdown_master> GetDropDownListbyLayerId(int Layer_id, bool Isfilter)
        {
            if (Isfilter == true)
            {
                return (List<dropdown_master>)repo.GetAll(m => m.layer_id == Layer_id && m.is_active == true && m.is_action_allowed == true);

            }
            else
            {
                return (List<dropdown_master>)repo.GetAll(m => m.layer_id == Layer_id && m.is_active == true);
            }

        }
        public List<ParentDropdownMasterMapping> GetPrentDetailsByLayerId(int Layer_id, string dropdown_type)
        {
            try
            {
                return repo.ExecuteProcedure<ParentDropdownMasterMapping>("fn_dropdownmastermappinglist", new { p_LayerId = Layer_id, p_DropDownType = dropdown_type });
            }
            catch { throw; }

        }

        public List<dropdown_master> GetDropDownListbyLayerId(int Layer_id, string FieldName)
        {
            return (List<dropdown_master>)repo.GetAll(m => m.layer_id == Layer_id && m.dropdown_type == FieldName);

        }

        public int GetDropDownListbyDropdowndetails(int Layer_id, string FieldName, string Value)
        {
            var result = repo.GetAll(m => m.dropdown_type == FieldName &&
                            m.dropdown_value == Value &&
                            m.layer_id == Layer_id);
            return result.Count();

        }

        public int SaveDropdownMaster(dropdown_master objAddDropdownMaster)
        {
            try
            {
                var result = repo.Get(m => m.dropdown_type == objAddDropdownMaster.dropdown_type &&
                               m.dropdown_value == objAddDropdownMaster.dropdown_value &&
                               m.layer_id == objAddDropdownMaster.layer_id);
                if (result == null)
                {
                    repo.Insert(objAddDropdownMaster);
                    return 1;
                }
                else
                {
                    return -1;
                }

            }
            catch { throw; }

        }

        public List<RCA_master> GetRCAMasterSettingsList
            (ViewEntityRCAMasterSettingsFilter objDropdownMasterSettingsFilter)
        {
            try
            {
                return repo.ExecuteProcedure<RCA_master>("fn_get_entity_rca", new
                {
                    P_PAGENO = objDropdownMasterSettingsFilter.currentPage,
                    P_PAGERECORD = objDropdownMasterSettingsFilter.pageSize,
                    P_SORTCOLNAME = objDropdownMasterSettingsFilter.sort,
                    P_SORTTYPE = objDropdownMasterSettingsFilter.orderBy,
                    layer_id = objDropdownMasterSettingsFilter.layer_id,
                    fieldname = string.IsNullOrEmpty(objDropdownMasterSettingsFilter.status) ? "" : (objDropdownMasterSettingsFilter.status.ToUpper()),
                    value = string.IsNullOrEmpty(objDropdownMasterSettingsFilter.RCA) ? "" : (objDropdownMasterSettingsFilter.RCA.ToUpper()),
                    id = objDropdownMasterSettingsFilter.id
                }, true);
            }
            catch { throw; }
        }

        public List<ViewDropDownMasterSetting> GetDropdownMasterSettingsList
            (ViewEntityDropdownMasterSettingsFilter objDropdownMasterSettingsFilter)
        {
            try
            {
                return repo.ExecuteProcedure<ViewDropDownMasterSetting>("fn_get_entity_dropdown_settings", new
                {
                    P_PAGENO = objDropdownMasterSettingsFilter.currentPage,
                    P_PAGERECORD = objDropdownMasterSettingsFilter.pageSize,
                    P_SORTCOLNAME = objDropdownMasterSettingsFilter.sort,
                    P_SORTTYPE = objDropdownMasterSettingsFilter.orderBy,
                    layer_id = objDropdownMasterSettingsFilter.layer_id,
                    fieldname = objDropdownMasterSettingsFilter.dropdown_type,
                    value = objDropdownMasterSettingsFilter.dropdown_value,
                    id = objDropdownMasterSettingsFilter.id
                }, true);
            }
            catch { throw; }
        }

        public List<dropdown_master> GetAllDropdownData()
        {
            try
            {
                return repo.GetAll(m => m.is_active == true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<RowCountResult> GetDropdownMasterRowCount(int layer_id, string layer_name, string fieldname,
            string value)
        {
            try
            {
                var rowcount = repo.ExecuteProcedure<RowCountResult>("fn_get_dropdown_value_count", new
                {

                    layer_id = layer_id,
                    layername = layer_name,
                    fieldname = fieldname,
                    value = value
                }, true);
                return rowcount;
            }

            catch (Exception ex) { throw ex; }

        }


        public List<int> DeleteDropdownMasters(int id)
        {
            try
            {
                var deleteStatus = repo.ExecuteProcedure<int>("fn_delete_rca_value", new
                {
                    id = id
                }, false);
                return deleteStatus;
            }
            catch
            {
                throw;
            }
        }

        public List<int> DeleteDropdownMaster(int id)
        {
            try
            {
                var deleteStatus = repo.ExecuteProcedure<int>("fn_delete_dropdown_value", new
                {
                    id = id
                }, false);
                return deleteStatus;
            }
            catch
            {
                throw;
            }
        }
        public List<int> UpdateDropdownMaster(int Id, int layer_id, string fieldname, string OldValue, string Value, bool IsVisible, int UserId)
        {
            try
            {
                var UpdateStatus = repo.ExecuteProcedure<int>("fn_update_dropdown_value", new
                {
                    id = Id,
                    layerid = layer_id,
                    fieldname = fieldname,
                    OldValue = OldValue,
                    value = Value,
                    IsVisible = IsVisible,
                    UserId = UserId
                }, false);
                return UpdateStatus;
            }
            catch
            {
                throw;
            }
        }




    }

    public class DADropdownMapping : Repository<DropdownMasterMapping>
    {
        public DropdownMasterMapping GetMappingDetailsbyId(int id)
        {
            var result = repo.GetAll(x => x.parent_mapping_id == id).FirstOrDefault();
            return result != null ? result : new DropdownMasterMapping();
        }
    }

    public class DAVendorSpecificationbulk : Repository<TempVendorSpecificationMaster>

    {
        public void VendorSpecificationBulkUpload(List<TempVendorSpecificationMaster> VendorSpecificationlist)
        {
            try
            {
                repo.Insert(VendorSpecificationlist);
            }
            catch { throw; }
        }
        public Tuple<int, int> getTotalUploadVendorSpecificationfailureAndSuccess(int UserId)
        {
            try
            {
                var getTotalUploadVendorSpecificationfailure = repo.GetAll().Where(x => x.created_by == UserId & x.is_valid == false).Count();
                var getTotalUploadVendorSpecificationSuccess = repo.GetAll().Where(x => x.created_by == UserId & x.is_valid == true).Count();
                return Tuple.Create(getTotalUploadVendorSpecificationSuccess, getTotalUploadVendorSpecificationfailure);
            }
            catch { throw; }
        }
        public void DeleteTempVendorSpecificationData(int UserId)
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_delete_temp_vendorspecifation_data", new { P_Userid = UserId });
            }
            catch { throw; }
        }

        public List<TempVendorSpecificationMaster> GetUploadVendorSpecificationLogs(int UserId)
        {
            try
            {
                return repo.GetAll().Where(x => x.created_by == UserId).ToList();
            }
            catch { throw; }
        }






    }

    public class DARCAMaster : Repository<Models.WFM.WfmRca>
    {

        public List<WfmRca> GetRCAListbyLayerId(string Status)
        {
            return (List<WfmRca>)repo.GetAll(m => m.status == Status);
        }



        public List<RCA_master> GetRCAbyLayerId(int id)
        {

            return (List<RCA_master>)repo.GetAll(m => m.id == id);

        }

        public List<int> UpdatercaMaster(int Id, int layer_id, string fieldname, string OldValue, string Value, bool IsVisible, int UserId)
        {
            try
            {
                var UpdateStatus = repo.ExecuteProcedure<int>("fn_update_RCA_value", new
                {
                    id = Id,
                    layerid = layer_id,
                    fieldname = fieldname,
                    OldValue = OldValue,
                    value = Value,
                    IsVisible = IsVisible,
                    UserId = UserId
                }, false);
                return UpdateStatus;
            }
            catch
            {
                throw;
            }
        }




        public int GetrcabyDropdowndetails(int id, string status, string rca)
        {
            var result = repo.GetAll(m => m.status == status &&
                            m.rca == rca);
            return result.Count();

        }

        public WfmRca GetrcaListbyId(int id)
        {
            return (WfmRca)repo.Get(m => m.id == id);
        }


        public int SaveRCAMaster(WfmRca objAddDropdownMaster)
        {
            try
            {
                var result = repo.Get(m => m.status == objAddDropdownMaster.status &&
                               m.rca == objAddDropdownMaster.rca);
                if (result == null)
                {
                    repo.Insert(objAddDropdownMaster);
                    return 1;
                }
                else
                {
                    return -1;
                }

            }
            catch { throw; }

        }
    }
    public class DAItemVendorCostMaster : Repository<Models.Admin.ItemVendorCostMaster>
    {
        public string SaveItemVendorCostDetails(ItemVendorCostMaster objItemVendorCostMaster,int userid)
        {
            try
            {
                var objExisiting = repo.GetById(m => m.item_code == objItemVendorCostMaster.item_code && m.user_id== objItemVendorCostMaster.user_id);
                var result = "Failed";
                if (objExisiting != null)
                {
                    objExisiting.modified_by = userid ;
                    objExisiting.modified_on = DateTimeHelper.Now;
                    objExisiting.item_cost = objItemVendorCostMaster.item_cost;
                    repo.Update(objExisiting);
                    result = "Update";
                }
                else
                {
                    objItemVendorCostMaster.created_by = userid;
                    objItemVendorCostMaster.created_on = DateTimeHelper.Now;
                    repo.Insert(objItemVendorCostMaster);
                    result = "Save";
                }
                return result;
            }
            catch { throw; }
        }
    }

    public class DASiteAwardDetails : Repository<DbMessage>
    {
        public DbMessage SaveSiteAwardDetails(List<SiteAwardDetails> obSiteAwardDetails, int userid)
        {
            var itemcod = string.Join(",", obSiteAwardDetails.Select(a => a.item_code).ToArray());
            var specification = string.Join(",", obSiteAwardDetails.Select(a => a.specification).ToArray());
            var userId = string.Join(",", obSiteAwardDetails.Select(a => a.user_id).ToArray());

           // List<SiteAwardDetails> newRecords = new List<SiteAwardDetails>();
           // var result = "Failed";
          //  bool flagduplicateRec = true;
            // var objExisiting = repo.GetById(m => m.item_code == obSiteAwardDetails.item_code && m.user_id == obSiteAwardDetails.user_id);
            try
            {

                //foreach (var siteAward in obSiteAwardDetails)
                //{                  
                //    var existingRecord = repo.GetById(m => m.item_code == siteAward.item_code && m.user_id == siteAward.user_id);

                //    if (existingRecord == null) // Only add new records
                //    {
                //        flagduplicateRec = false;
                //        siteAward.created_by = userid;
                //        siteAward.created_on = DateTimeHelper.Now;
                //        newRecords.Add(siteAward);
                //    }

                //}

                //if (flagduplicateRec == false) { repo.Insert(obSiteAwardDetails); result = "Save"; }
                //else 
                //{
                //    result = "Failed";
                //}                                           
                //return result;
                var lst = repo.ExecuteProcedure<DbMessage>("fn_insert_site_award_details",
                    new
                    {
                        p_itemcode= itemcod,
                        p_specification = specification,
                        p_userid = userId                      
                    }, false).FirstOrDefault();
                return lst;


            }
            catch { throw; }
        }
    }

}


