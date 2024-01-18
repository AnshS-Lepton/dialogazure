using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAItemTemplate : Repository<object>
    {
        public List<KeyValueDropDown> GetItemSpecification(string entitytype, int typeid, int brandid, string specification)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_item_specification", new { p_entitytype = entitytype, p_typeid = typeid, p_brandid = brandid, p_specification = specification });
                //return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_item_specification", new { p_entitytype = entitytype, p_typeid = typeid, p_brandid = brandid, p_specification = specification });
            }
            catch { throw; }
        }
        public List<itemMaster> GetLayerTemplateDetail(int userid)
        {

            try
            {
                var lstdetails = repo.ExecuteProcedure<itemMaster>("fn_offline_get_template_detail", new { p_userid = userid }, true);
                return lstdetails;
            }
            catch { throw; }
        }
        public List<KeyValueDropDown> GetDropDownList()
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_dropdown_list", null, false);
            }
            catch { throw; }
        }

        public List<KeyValueDropDown> GetVendorList(string specification)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_vendor_list", new { p_specification = specification });
            }
            catch { throw; }
        }
        public List<KeyValueDropDown> GetAllVendorList()
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_all_vendor_list", null);
            }
            catch { throw; }
        }
        public List<itemCategory> GetCatSubCatData(string entitytype, string specification, int vendor_id)
        {
            try
            {
                return repo.ExecuteProcedure<itemCategory>("fn_get_item_template_data", new { p_entitytype = entitytype, p_specification = specification, p_vandor_id = vendor_id });
            }
            catch { throw; }
        }
        public List<itemCategory> GetMicroductNoOfWaysData(string entitytype, string specification, int vendor_id)
        {
            try
            {
                return repo.ExecuteProcedure<itemCategory>("fn_get_vendor_microductnoofways", new { p_entitytype = entitytype, p_specification = specification, p_vandor_id = vendor_id });
            }
            catch { throw; }
        }
        public List<KeyValueDropDown> GetBrandData(int typeid)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_isp_brand", new { typeid = typeid });
            }
            catch { throw; }
        }
        public List<KeyValueDropDown> GetModelData(int brandid)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_isp_model", new { brandid = brandid });
            }
            catch { throw; }
        }
        public List<DropDownMaster> GetMicroDuctData()
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_MicrDuctNoOfWays", new { });
            }
            catch { throw; }
        }

        public T GetTemplateDetail<T>(int userid, EntityType eType, string subEntityType) where T : new()
        {

            try
            {
                var lstItem = repo.ExecuteProcedure<T>("fn_get_template_detail", new { p_userid = userid, p_entitytype = eType.ToString(), p_sub_entitytype = subEntityType }, true);
                return lstItem != null && lstItem.Count > 0 ? lstItem[0] : new T();
            }
            catch { throw; }
        }
        public void BindItemDropdowns(dynamic objItem, string entityType)
        {
            //INITIALIZE DEFAULT VALUES FOR TYPE BRNAD AND SPECIFICATIONS...
            int type = objItem != null && objItem.type != null ? objItem.type : 0;
            int brand = objItem != null && objItem.brand != null ? objItem.brand : 0;
            string specification = objItem != null && objItem.specification != null ? objItem.specification : "";
            var lstDropDownValues = GetItemSpecification(entityType, type, brand, specification);
            //  var rowDropDownValues = GetRowSpecification(entityType, rtype, rwidth);
            // fill dropdown values...
            objItem.lstSpecification = lstDropDownValues.Where(x => x.ddtype == DropDownType.Specification.ToString()).ToList();
            var listType = lstDropDownValues.Where(x => x.ddtype == DropDownType.TypeMaster.ToString()).ToList();
            objItem.lstType = lstDropDownValues.Where(x => x.ddtype == DropDownType.TypeMaster.ToString()).ToList();
            objItem.lstVendor = lstDropDownValues.Where(x => x.ddtype == DropDownType.Vendor.ToString()).ToList();
            objItem.lstActivation = lstDropDownValues.Where(x => x.ddtype == DropDownType.Activation.ToString()).ToList();
            objItem.lstConstruction = lstDropDownValues.Where(x => x.ddtype == DropDownType.Construction.ToString()).ToList();
            objItem.lstAccessibility = lstDropDownValues.Where(x => x.ddtype == DropDownType.Accessibility.ToString()).ToList();
            objItem.lstBrand = lstDropDownValues.Where(x => x.ddtype == DropDownType.Brand.ToString()).ToList();
            objItem.lstModel = lstDropDownValues.Where(x => x.ddtype == DropDownType.Model.ToString()).ToList();
            objItem.entityType = entityType;
            //objItem.lstSplitterRatio = objDDL.Where(x => x.dropdown_type == DropDownType.Splitter_Ratio.ToString()).ToList();
        }

        //public CableItemMaster GetCableTemplateDetail(int userid, EntityType eType, string cableType)
        //{
        //    var cblObj=new CableItemMaster();
        //    try
        //    {
        //        var lstItem = repo.ExecuteProcedure<CableItemMaster>("fn_get_template_detail", new { p_userid = userid, p_entitytype = eType.ToString() }, true);
        //        //Where(x => x.cable_type == cableType)
        //        if (lstItem != null)
        //            foreach (CableItemMaster cbl in lstItem)
        //            {
        //                if (cbl.cable_type == cableType)
        //                {
        //                     cblObj=cbl;
        //                }
        //            }
        //        //else
        //        //   return new CableItemMaster();
        //        return cblObj;
        //       // return lstItem != null ? lstItem : new CableItemMaster();

        //    }
        //    catch { throw; }
        //}

        public List<KeyValueDropDown> GetAccessoriesSpecification(int accessories_id)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_accessories_specification", new { p_accessories_id = accessories_id });
            }
            catch { throw; }
        }

    }

    public class DAIADBtemMaster : Repository<ADBItemMaster>
    {
        public ADBItemMaster SaveADBItemTemplate(ADBItemMaster ADBItem, int userId)
        {
            try
            {
                var objADBItem = repo.Get(x => x.id == ADBItem.id);
                if (objADBItem != null)
                {
                    objADBItem.specification = ADBItem.specification;
                    objADBItem.category = ADBItem.category;
                    objADBItem.subcategory1 = ADBItem.subcategory1;
                    objADBItem.subcategory2 = ADBItem.subcategory2;
                    objADBItem.subcategory3 = ADBItem.subcategory3;
                    objADBItem.vendor_id = ADBItem.vendor_id;
                    objADBItem.item_code = ADBItem.item_code;
                    objADBItem.type = ADBItem.type;
                    objADBItem.brand = ADBItem.brand;
                    objADBItem.model = ADBItem.model;
                    objADBItem.construction = ADBItem.construction;
                    objADBItem.activation = ADBItem.activation;
                    objADBItem.accessibility = ADBItem.accessibility;
                    objADBItem.modified_by = userId;
                    objADBItem.modified_on = DateTimeHelper.Now;
                    objADBItem.no_of_input_port = ADBItem.no_of_input_port;
                    objADBItem.no_of_output_port = ADBItem.no_of_output_port;
                    objADBItem.no_of_port = ADBItem.no_of_port;
                    objADBItem.entity_category = ADBItem.entity_category;
                    objADBItem.audit_item_master_id = ADBItem.audit_item_master_id;
                    return repo.Update(objADBItem);
                }
                else
                {
                    ADBItem.created_by = userId;
                    ADBItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(ADBItem);
                }
            }
            catch { throw; }
        }
    }

    public class DAICDBtemMaster : Repository<CDBItemMaster>
    {
        public CDBItemMaster SaveCDBItemTemplate(CDBItemMaster CDBItem, int userId)
        {
            try
            {
                var objCDBItem = repo.Get(x => x.id == CDBItem.id);
                if (objCDBItem != null)
                {
                    objCDBItem.specification = CDBItem.specification;
                    objCDBItem.category = CDBItem.category;
                    objCDBItem.subcategory1 = CDBItem.subcategory1;
                    objCDBItem.subcategory2 = CDBItem.subcategory2;
                    objCDBItem.subcategory3 = CDBItem.subcategory3;
                    objCDBItem.vendor_id = CDBItem.vendor_id;
                    objCDBItem.item_code = CDBItem.item_code;
                    objCDBItem.type = CDBItem.type;
                    objCDBItem.brand = CDBItem.brand;
                    objCDBItem.model = CDBItem.model;
                    objCDBItem.construction = CDBItem.construction;
                    objCDBItem.activation = CDBItem.activation;
                    objCDBItem.accessibility = CDBItem.accessibility;
                    objCDBItem.modified_by = userId;
                    objCDBItem.modified_on = DateTimeHelper.Now;
                    objCDBItem.no_of_input_port = CDBItem.no_of_input_port;
                    objCDBItem.no_of_output_port = CDBItem.no_of_output_port;
                    objCDBItem.no_of_port = CDBItem.no_of_port;
                    objCDBItem.entity_category = CDBItem.entity_category;
                    return repo.Update(objCDBItem);
                }
                else
                {
                    CDBItem.created_by = userId;
                    CDBItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(CDBItem);
                }
            }
            catch { throw; }
        }
        public CDBItemMaster getCDBTemplatebyPortNo(int no_of_ports, string entity_type, int vendor_id)
        {
            try
            {
                var result = repo.GetAll(x => x.no_of_port == no_of_ports && x.entity_category.ToUpper() == entity_type.ToString().ToUpper() && x.vendor_id == vendor_id).FirstOrDefault();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class DAIBDBtemMaster : Repository<BDBItemMaster>
    {
        public BDBItemMaster SaveBDBItemTemplate(BDBItemMaster BDBItem, int userId)
        {
            try
            {
                var objBDBItem = repo.Get(x => x.created_by == userId);
                if (objBDBItem != null)
                {
                    objBDBItem.specification = BDBItem.specification;
                    objBDBItem.category = BDBItem.category;
                    objBDBItem.subcategory1 = BDBItem.subcategory1;
                    objBDBItem.subcategory2 = BDBItem.subcategory2;
                    objBDBItem.subcategory3 = BDBItem.subcategory3;
                    objBDBItem.vendor_id = BDBItem.vendor_id;
                    objBDBItem.item_code = BDBItem.item_code;
                    objBDBItem.type = BDBItem.type;
                    objBDBItem.brand = BDBItem.brand;
                    objBDBItem.model = BDBItem.model;
                    objBDBItem.construction = BDBItem.construction;
                    objBDBItem.activation = BDBItem.activation;
                    objBDBItem.accessibility = BDBItem.accessibility;
                    objBDBItem.modified_by = userId;
                    objBDBItem.modified_on = DateTimeHelper.Now;
                    objBDBItem.no_of_input_port = BDBItem.no_of_input_port;
                    objBDBItem.no_of_output_port = BDBItem.no_of_output_port;
                    objBDBItem.no_of_port = BDBItem.no_of_port;
                    objBDBItem.entity_category = BDBItem.entity_category;
                    objBDBItem.audit_item_master_id = BDBItem.audit_item_master_id;
                    return repo.Update(objBDBItem);
                }
                else
                {
                    BDBItem.created_by = userId;
                    BDBItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(BDBItem);
                }
            }
            catch { throw; }
        }
    }
    public class DAIPoleItemMaster : Repository<PoleItemMaster>
    {
        public PoleItemMaster GetPoleItemtemplatebyID(int userid, string eType)
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<PoleItemMaster>("fn_get_itemtemplate_byid", new { p_userid = userid, p_entitytype = eType });
                return lstItems != null && lstItems.Count > 0 ? lstItems[0] : new PoleItemMaster();
            }
            catch { throw; }
        }
        public PoleItemMaster SavePoleItemTemplate(PoleItemMaster PoleItem, int userId)
        {
            try
            {
                var objPoleItem = repo.Get(x => x.created_by == userId);
                if (objPoleItem != null)
                {
                    objPoleItem.specification = PoleItem.specification;
                    objPoleItem.category = PoleItem.category;
                    objPoleItem.subcategory1 = PoleItem.subcategory1;
                    objPoleItem.subcategory2 = PoleItem.subcategory2;
                    objPoleItem.subcategory3 = PoleItem.subcategory3;
                    objPoleItem.vendor_id = PoleItem.vendor_id;
                    objPoleItem.item_code = PoleItem.item_code;
                    objPoleItem.type = PoleItem.type;
                    objPoleItem.brand = PoleItem.brand;
                    objPoleItem.model = PoleItem.model;
                    objPoleItem.construction = PoleItem.construction;
                    objPoleItem.activation = PoleItem.activation;
                    objPoleItem.accessibility = PoleItem.accessibility;
                    objPoleItem.pole_type = PoleItem.pole_type;
                    objPoleItem.modified_by = userId;
                    objPoleItem.modified_on = DateTimeHelper.Now;
                    objPoleItem.audit_item_master_id = PoleItem.audit_item_master_id;
                    return repo.Update(objPoleItem);
                }
                else
                {
                    PoleItem.created_by = userId;
                    PoleItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(PoleItem);
                }
            }
            catch { throw; }
        }
        public bool ChkEntityTemplateExist(string entity_type, int user_id, string subEntityType)
        {
            try
            {
                var lstItem = repo.ExecuteProcedure<object>("fn_get_template_detail", new { p_userid = user_id, p_entitytype = entity_type.ToString(), p_sub_entitytype = subEntityType }, true);
                return lstItem != null && lstItem.Count > 0 ? true : false;
            }
            catch { throw; }
        }
    }

    public class DAPODItemMaster : Repository<PODItemMaster>
    {
        public PODItemMaster SavePODItemTemplate(PODItemMaster PODItem, int userId)
        {
            try
            {
                var objPODItem = repo.Get(x => x.id == PODItem.id);
                if (objPODItem != null)
                {
                    objPODItem.specification = PODItem.specification;
                    objPODItem.category = PODItem.category;
                    objPODItem.subcategory1 = PODItem.subcategory1;
                    objPODItem.subcategory2 = PODItem.subcategory2;
                    objPODItem.subcategory3 = PODItem.subcategory3;
                    objPODItem.vendor_id = PODItem.vendor_id;
                    objPODItem.item_code = PODItem.item_code;
                    objPODItem.type = PODItem.type;
                    objPODItem.brand = PODItem.brand;
                    objPODItem.model = PODItem.model;
                    objPODItem.construction = PODItem.construction;
                    objPODItem.activation = PODItem.activation;
                    objPODItem.accessibility = PODItem.accessibility;
                    objPODItem.modified_by = userId;
                    objPODItem.modified_on = DateTimeHelper.Now;
                    objPODItem.pod_type = PODItem.pod_type;
                    objPODItem.audit_item_master_id = PODItem.audit_item_master_id;
                    return repo.Update(objPODItem);
                }
                else
                {
                    PODItem.created_by = userId;
                    PODItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(PODItem);
                }
            }
            catch { throw; }
        }

    }


    public class DATreeItemMaster : Repository<TreeItemMaster>
    {
        public TreeItemMaster SaveTreeItemTemplate(TreeItemMaster TreeItem, int userId)
        {
            try
            {
                var objTreeItem = repo.Get(x => x.id == TreeItem.id);
                if (objTreeItem != null)
                {
                    objTreeItem.specification = TreeItem.specification;
                    objTreeItem.category = TreeItem.category;
                    objTreeItem.subcategory1 = TreeItem.subcategory1;
                    objTreeItem.subcategory2 = TreeItem.subcategory2;
                    objTreeItem.subcategory3 = TreeItem.subcategory3;
                    objTreeItem.vendor_id = TreeItem.vendor_id;
                    objTreeItem.item_code = TreeItem.item_code;
                    objTreeItem.type = TreeItem.type;
                    objTreeItem.brand = TreeItem.brand;
                    objTreeItem.model = TreeItem.model;
                    objTreeItem.construction = TreeItem.construction;
                    objTreeItem.activation = TreeItem.activation;
                    objTreeItem.accessibility = TreeItem.accessibility;
                    objTreeItem.modified_by = userId;
                    objTreeItem.modified_on = DateTimeHelper.Now;
                    objTreeItem.audit_item_master_id = TreeItem.audit_item_master_id;
                    return repo.Update(objTreeItem);
                }
                else
                {
                    TreeItem.created_by = userId;
                    TreeItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(TreeItem);
                }
            }
            catch { throw; }
        }



    }

    public class DASCItemMaster : Repository<SCItemMaster>
    {
        public SCItemMaster SaveSCItemTemplate(SCItemMaster SCItem, int userId)
        {
            try
            {
                var objSCItem = repo.Get(x => x.id == SCItem.id);
                if (objSCItem != null)
                {
                    objSCItem.specification = SCItem.specification;
                    objSCItem.category = SCItem.category;
                    objSCItem.subcategory1 = SCItem.subcategory1;
                    objSCItem.subcategory2 = SCItem.subcategory2;
                    objSCItem.subcategory3 = SCItem.subcategory3;
                    objSCItem.vendor_id = SCItem.vendor_id;
                    objSCItem.item_code = SCItem.item_code;
                    objSCItem.type = SCItem.type;
                    objSCItem.brand = SCItem.brand;
                    objSCItem.model = SCItem.model;
                    objSCItem.construction = SCItem.construction;
                    objSCItem.activation = SCItem.activation;
                    objSCItem.accessibility = SCItem.accessibility;
                    objSCItem.no_of_ports = SCItem.no_of_ports;
                    objSCItem.modified_by = userId;
                    objSCItem.modified_on = DateTimeHelper.Now;
                    objSCItem.no_of_input_port = SCItem.no_of_input_port;
                    objSCItem.no_of_output_port = SCItem.no_of_output_port;
                    objSCItem.audit_item_master_id = SCItem.audit_item_master_id;
                    return repo.Update(objSCItem);
                }
                else
                {
                    SCItem.created_by = userId;
                    SCItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(SCItem);
                }
            }
            catch { throw; }
        }
    }

    public class DAFMSItemMaster : Repository<FMSItemMaster>
    {
        public FMSItemMaster SaveFMSItemTemplate(FMSItemMaster Item, int userId)
        {
            try
            {
                var objItem = repo.Get(x => x.id == Item.id);
                if (objItem != null)
                {
                    objItem.specification = Item.specification;
                    objItem.category = Item.category;
                    objItem.subcategory1 = Item.subcategory1;
                    objItem.subcategory2 = Item.subcategory2;
                    objItem.subcategory3 = Item.subcategory3;
                    objItem.vendor_id = Item.vendor_id;
                    objItem.item_code = Item.item_code;
                    objItem.type = Item.type;
                    objItem.brand = Item.brand;
                    objItem.model = Item.model;
                    objItem.construction = Item.construction;
                    objItem.activation = Item.activation;
                    objItem.accessibility = Item.accessibility;
                    objItem.no_of_port = Item.no_of_port;
                    objItem.modified_by = userId;
                    objItem.modified_on = DateTimeHelper.Now;
                    objItem.no_of_input_port = Item.no_of_input_port;
                    objItem.no_of_output_port = Item.no_of_output_port;
                    objItem.audit_item_master_id = Item.audit_item_master_id;
                    return repo.Update(objItem);
                }
                else
                {
                    Item.created_by = userId;
                    Item.created_on = DateTimeHelper.Now;
                    return repo.Insert(Item);
                }
            }
            catch { throw; }
        }
    }


    public class DAManholeItemMaster : Repository<ManholeItemMaster>
    {
        public ManholeItemMaster SaveManholeItemTemplate(ManholeItemMaster ManholeItem, int userId)
        {
            try
            {
                var objManholeItem = repo.Get(x => x.id == ManholeItem.id);
                if (objManholeItem != null)
                {
                    objManholeItem.specification = ManholeItem.specification;
                    objManholeItem.category = ManholeItem.category;
                    objManholeItem.subcategory1 = ManholeItem.subcategory1;
                    objManholeItem.subcategory2 = ManholeItem.subcategory2;
                    objManholeItem.subcategory3 = ManholeItem.subcategory3;
                    objManholeItem.vendor_id = ManholeItem.vendor_id;
                    objManholeItem.item_code = ManholeItem.item_code;
                    objManholeItem.type = ManholeItem.type;
                    objManholeItem.brand = ManholeItem.brand;
                    objManholeItem.model = ManholeItem.model;
                    objManholeItem.construction = ManholeItem.construction;
                    objManholeItem.activation = ManholeItem.activation;
                    objManholeItem.accessibility = ManholeItem.accessibility;
                    objManholeItem.modified_by = userId;
                    objManholeItem.modified_on = DateTimeHelper.Now;
                    objManholeItem.audit_item_master_id = ManholeItem.audit_item_master_id;
                    objManholeItem.manhole_types = ManholeItem.manhole_types;
                    return repo.Update(objManholeItem);
                }
                else
                {
                    ManholeItem.created_by = userId;
                    ManholeItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(ManholeItem);
                }
            }
            catch { throw; }
        }
    }
    public class DACouplerItemMaster : Repository<CouplerItemMaster>
    {
        public CouplerItemMaster SaveCouplerItemTemplate(CouplerItemMaster CouplerItem, int userId)
        {
            try
            {
                var objCouplerItem = repo.Get(x => x.id == CouplerItem.id);
                if (objCouplerItem != null)
                {
                    objCouplerItem.specification = CouplerItem.specification;
                    objCouplerItem.category = CouplerItem.category;
                    objCouplerItem.subcategory1 = CouplerItem.subcategory1;
                    objCouplerItem.subcategory2 = CouplerItem.subcategory2;
                    objCouplerItem.subcategory3 = CouplerItem.subcategory3;
                    objCouplerItem.vendor_id = CouplerItem.vendor_id;
                    objCouplerItem.item_code = CouplerItem.item_code;
                    objCouplerItem.type = CouplerItem.type;
                    objCouplerItem.brand = CouplerItem.brand;
                    objCouplerItem.model = CouplerItem.model;
                    objCouplerItem.construction = CouplerItem.construction;
                    objCouplerItem.activation = CouplerItem.activation;
                    objCouplerItem.accessibility = CouplerItem.accessibility;
                    objCouplerItem.modified_by = userId;
                    objCouplerItem.modified_on = DateTimeHelper.Now;
                    objCouplerItem.audit_item_master_id = CouplerItem.audit_item_master_id;
                    return repo.Update(objCouplerItem);
                }
                else
                {
                    CouplerItem.created_by = userId;
                    CouplerItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(CouplerItem);
                }
            }
            catch { throw; }
        }
    }

    public class DASplitterItemMaster : Repository<SplitterItemMaster>
    {
        public SplitterItemMaster SaveSplitterItemTemplate(SplitterItemMaster SplitterItem, int userId)
        {
            try
            {
                var objSplitterItem = repo.Get(x => x.created_by == userId);
                if (objSplitterItem != null)
                {
                    objSplitterItem.specification = SplitterItem.specification;
                    objSplitterItem.category = SplitterItem.category;
                    objSplitterItem.subcategory1 = SplitterItem.subcategory1;
                    objSplitterItem.subcategory2 = SplitterItem.subcategory2;
                    objSplitterItem.subcategory3 = SplitterItem.subcategory3;
                    objSplitterItem.vendor_id = SplitterItem.vendor_id;
                    objSplitterItem.item_code = SplitterItem.item_code;
                    objSplitterItem.type = SplitterItem.type;
                    objSplitterItem.brand = SplitterItem.brand;
                    objSplitterItem.model = SplitterItem.model;
                    objSplitterItem.construction = SplitterItem.construction;
                    objSplitterItem.activation = SplitterItem.activation;
                    objSplitterItem.accessibility = SplitterItem.accessibility;
                    objSplitterItem.splitter_ratio = SplitterItem.splitter_ratio;
                    objSplitterItem.modified_by = userId;
                    objSplitterItem.modified_on = DateTimeHelper.Now;
                    objSplitterItem.splitter_type = SplitterItem.splitter_type;
                    objSplitterItem.audit_item_master_id = SplitterItem.audit_item_master_id;
                    return repo.Update(objSplitterItem);
                }
                else
                {
                    SplitterItem.created_by = userId;
                    SplitterItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(SplitterItem);
                }
            }
            catch { throw; }
        }


    }
    public class DAMPODItemMaster : Repository<MPODItemMaster>
    {
        public MPODItemMaster SaveMPODItemTemplate(MPODItemMaster MPODItem, int userId)
        {
            try
            {
                var objMPODItem = repo.Get(x => x.id == MPODItem.id);
                if (objMPODItem != null)
                {
                    objMPODItem.specification = MPODItem.specification;
                    objMPODItem.category = MPODItem.category;
                    objMPODItem.subcategory1 = MPODItem.subcategory1;
                    objMPODItem.subcategory2 = MPODItem.subcategory2;
                    objMPODItem.subcategory3 = MPODItem.subcategory3;
                    objMPODItem.vendor_id = MPODItem.vendor_id;
                    objMPODItem.item_code = MPODItem.item_code;
                    objMPODItem.type = MPODItem.type;
                    objMPODItem.brand = MPODItem.brand;
                    objMPODItem.model = MPODItem.model;
                    objMPODItem.construction = MPODItem.construction;
                    objMPODItem.activation = MPODItem.activation;
                    objMPODItem.accessibility = MPODItem.accessibility;
                    objMPODItem.modified_by = userId;
                    objMPODItem.modified_on = DateTimeHelper.Now;
                    objMPODItem.mpod_type = MPODItem.mpod_type;
                    objMPODItem.audit_item_master_id = MPODItem.audit_item_master_id;
                    return repo.Update(objMPODItem);
                }
                else
                {
                    MPODItem.created_by = userId;
                    MPODItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(MPODItem);
                }
            }
            catch { throw; }
        }
    }

    public class DACableItemMaster : Repository<CableItemMaster>
    {
        public CableItemMaster SaveCableItemTemplate(CableItemMaster CableItem, int userId)
        {
            try
            {
                var objCableItem = repo.Get(x => x.created_by == userId);
                if (objCableItem != null)
                {
                    objCableItem.specification = CableItem.specification;
                    objCableItem.category = CableItem.category;
                    objCableItem.subcategory1 = CableItem.subcategory1;
                    objCableItem.subcategory2 = CableItem.subcategory2;
                    objCableItem.subcategory3 = CableItem.subcategory3;
                    objCableItem.vendor_id = CableItem.vendor_id;
                    objCableItem.item_code = CableItem.item_code;
                    objCableItem.type = CableItem.type;
                    objCableItem.brand = CableItem.brand;
                    objCableItem.model = CableItem.model;
                    objCableItem.construction = CableItem.construction;
                    objCableItem.activation = CableItem.activation;
                    objCableItem.accessibility = CableItem.accessibility;
                    objCableItem.modified_by = userId;
                    objCableItem.modified_on = DateTimeHelper.Now;
                    objCableItem.total_core = CableItem.total_core;
                    objCableItem.no_of_tube = CableItem.no_of_tube;
                    objCableItem.no_of_core_per_tube = CableItem.no_of_core_per_tube;
                    objCableItem.cable_category = CableItem.cable_category;
                    objCableItem.cable_sub_category = CableItem.cable_sub_category;
                    objCableItem.cable_type = CableItem.cable_type;
                    objCableItem.audit_item_master_id = CableItem.audit_item_master_id;
                    return repo.Update(objCableItem);
                }
                else
                {
                    CableItem.created_by = userId;
                    CableItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(CableItem);
                }
            }
            catch { throw; }
        }
    }

    public class DAROWItemMaster : Repository<ROWItemMaster>
    {
        public ROWItemMaster SaveROWItemTemplate(ROWItemMaster ROWItem, int userId)
        {
            try
            {
                var objROWItem = repo.Get(x => x.id == ROWItem.id);
                if (objROWItem != null)
                {

                    if (ROWItem.width == 0)
                    {
                        objROWItem.width = ROWItem.customized;
                    }
                    else
                    {
                        objROWItem.width = ROWItem.width;
                    }
                    //if (ROWItem.type=="Polygon")
                    //{
                    //    objROWItem.width = null;
                    //}
                    //else
                    //{
                    //    objROWItem.width = ROWItem.width;
                    //}
                    objROWItem.type = ROWItem.type;
                    objROWItem.customized = ROWItem.customized;
                    objROWItem.modified_by = userId;
                    objROWItem.modified_on = DateTimeHelper.Now;
                    return repo.Update(objROWItem);
                }
                else
                {
                    if (ROWItem.width == 0)
                    {
                        ROWItem.width = ROWItem.customized;
                    }
                    ROWItem.created_by = userId;
                    ROWItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(ROWItem);
                }
            }
            catch { throw; }
        }
    }
    //public bool IsEntityTemplateExist(string entity_type, int user_id, string subEntityType)
    //{
    //    try
    //    {
    //        bool result = false;
    //        var lstItem = repo.ExecuteProcedure<CableItemMaster>("fn_get_template_detail", new { p_userid = user_id, p_entitytype = entity_type.ToString() }, true);
    //        //return lstItem != null && lstItem.Count > 0 ? true : false;
    //        if (lstItem != null)
    //            foreach (CableItemMaster cbl in lstItem)
    //            {
    //                if (cbl.cable_type == subEntityType)
    //                {
    //                    result=true;
    //                }
    //            }

    //        return result;
    //    }
    //    catch { throw; }
    //}

    public class DAONTItemMaster : Repository<ONTItemMaster>
    {
        public ONTItemMaster SaveONTItemTemplate(ONTItemMaster ONTItem, int userId)
        {
            try
            {
                var objONTItem = repo.Get(x => x.id == ONTItem.id);
                if (objONTItem != null)
                {
                    objONTItem.specification = ONTItem.specification;
                    objONTItem.category = ONTItem.category;
                    objONTItem.subcategory1 = ONTItem.subcategory1;
                    objONTItem.subcategory2 = ONTItem.subcategory2;
                    objONTItem.subcategory3 = ONTItem.subcategory3;
                    objONTItem.vendor_id = ONTItem.vendor_id;
                    objONTItem.item_code = ONTItem.item_code;
                    objONTItem.type = ONTItem.type;
                    objONTItem.brand = ONTItem.brand;
                    objONTItem.model = ONTItem.model;
                    objONTItem.construction = ONTItem.construction;
                    objONTItem.activation = ONTItem.activation;
                    objONTItem.accessibility = ONTItem.accessibility;
                    objONTItem.modified_by = userId;
                    objONTItem.modified_on = DateTimeHelper.Now;
                    objONTItem.no_of_input_port = ONTItem.no_of_input_port;
                    objONTItem.no_of_output_port = ONTItem.no_of_output_port;
                    objONTItem.audit_item_master_id = ONTItem.audit_item_master_id;
                    return repo.Update(objONTItem);
                }
                else
                {
                    ONTItem.created_by = userId;
                    ONTItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(ONTItem);
                }
            }
            catch { throw; }
        }


    }

    public class DATrenchItemMaster : Repository<TrenchItemMaster>
    {
        public TrenchItemMaster SaveTrenchItemTemplate(TrenchItemMaster TrenchItem, int userId)
        {
            try
            {
                var objTrenchItem = repo.Get(x => x.id == TrenchItem.id);
                if (objTrenchItem != null)
                {
                    objTrenchItem.specification = TrenchItem.specification;
                    objTrenchItem.category = TrenchItem.category;
                    objTrenchItem.subcategory1 = TrenchItem.subcategory1;
                    objTrenchItem.subcategory2 = TrenchItem.subcategory2;
                    objTrenchItem.subcategory3 = TrenchItem.subcategory3;
                    objTrenchItem.vendor_id = TrenchItem.vendor_id;
                    objTrenchItem.item_code = TrenchItem.item_code;
                    objTrenchItem.type = TrenchItem.type;
                    objTrenchItem.brand = TrenchItem.brand;
                    objTrenchItem.model = TrenchItem.model;
                    objTrenchItem.construction = TrenchItem.construction;
                    objTrenchItem.activation = TrenchItem.activation;
                    objTrenchItem.accessibility = TrenchItem.accessibility;
                    objTrenchItem.modified_by = userId;
                    objTrenchItem.modified_on = DateTimeHelper.Now;
                    objTrenchItem.trench_type = TrenchItem.trench_type;
                    objTrenchItem.trench_height = TrenchItem.trench_height;
                    objTrenchItem.trench_width = TrenchItem.trench_width;
                    objTrenchItem.trench_serving_type = TrenchItem.trench_serving_type;
                    objTrenchItem.audit_item_master_id = TrenchItem.audit_item_master_id;
                    return repo.Update(objTrenchItem);
                }
                else
                {
                    TrenchItem.created_by = userId;
                    TrenchItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(TrenchItem);
                }
            }
            catch { throw; }
        }



    }

    public class DADuctItemMaster : Repository<DuctItemMaster>
    {
        public DuctItemMaster SaveDuctItemTemplate(DuctItemMaster DuctItem, int userId)
        {
            try
            {
                var objDuctItem = repo.Get(x => x.id == DuctItem.id);
                if (objDuctItem != null)
                {
                    objDuctItem.specification = DuctItem.specification;
                    objDuctItem.category = DuctItem.category;
                    objDuctItem.subcategory1 = DuctItem.subcategory1;
                    objDuctItem.subcategory2 = DuctItem.subcategory2;
                    objDuctItem.subcategory3 = DuctItem.subcategory3;
                    objDuctItem.vendor_id = DuctItem.vendor_id;
                    objDuctItem.item_code = DuctItem.item_code;
                    objDuctItem.type = DuctItem.type;
                    objDuctItem.brand = DuctItem.brand;
                    objDuctItem.model = DuctItem.model;
                    objDuctItem.construction = DuctItem.construction;
                    objDuctItem.activation = DuctItem.activation;
                    objDuctItem.accessibility = DuctItem.accessibility;
                    objDuctItem.modified_by = userId;
                    objDuctItem.modified_on = DateTimeHelper.Now;
                    objDuctItem.audit_item_master_id = DuctItem.audit_item_master_id;
                    return repo.Update(objDuctItem);
                }
                else
                {
                    DuctItem.created_by = userId;
                    DuctItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(DuctItem);
                }
            }
            catch { throw; }
        }



    }
    public class DAGipipeItemMaster : Repository<GipipeItemMaster>
    {
        public GipipeItemMaster SaveGipipeItemTemplate(GipipeItemMaster GipipeItem, int userId)
        {
            try
            {
                var objGipipeItem = repo.Get(x => x.id == GipipeItem.id);
                if (objGipipeItem != null)
                {
                    objGipipeItem.specification = GipipeItem.specification;
                    objGipipeItem.category = GipipeItem.category;
                    objGipipeItem.subcategory1 = GipipeItem.subcategory1;
                    objGipipeItem.subcategory2 = GipipeItem.subcategory2;
                    objGipipeItem.subcategory3 = GipipeItem.subcategory3;
                    objGipipeItem.vendor_id = GipipeItem.vendor_id;
                    objGipipeItem.item_code = GipipeItem.item_code;
                    objGipipeItem.type = GipipeItem.type;
                    objGipipeItem.brand = GipipeItem.brand;
                    objGipipeItem.model = GipipeItem.model;
                    objGipipeItem.construction = GipipeItem.construction;
                    objGipipeItem.activation = GipipeItem.activation;
                    objGipipeItem.accessibility = GipipeItem.accessibility;
                    objGipipeItem.modified_by = userId;
                    objGipipeItem.modified_on = DateTimeHelper.Now;
                    objGipipeItem.audit_item_master_id = GipipeItem.audit_item_master_id;
                    return repo.Update(objGipipeItem);
                }
                else
                {
                    GipipeItem.created_by = userId;
                    GipipeItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(GipipeItem);
                }
            }
            catch { throw; }
        }



    }


    public class DAConduitItemMaster : Repository<ConduitItemMaster>
    {
        public ConduitItemMaster SaveConduitItemTemplate(ConduitItemMaster ConduitItem, int userId)
        {
            try
            {
                var objConduitItem = repo.Get(x => x.id == ConduitItem.id);
                if (objConduitItem != null)
                {
                    objConduitItem.specification = ConduitItem.specification;
                    objConduitItem.category = ConduitItem.category;
                    objConduitItem.subcategory1 = ConduitItem.subcategory1;
                    objConduitItem.subcategory2 = ConduitItem.subcategory2;
                    objConduitItem.subcategory3 = ConduitItem.subcategory3;
                    objConduitItem.vendor_id = ConduitItem.vendor_id;
                    objConduitItem.item_code = ConduitItem.item_code;
                    objConduitItem.type = ConduitItem.type;
                    objConduitItem.brand = ConduitItem.brand;
                    objConduitItem.model = ConduitItem.model;
                    objConduitItem.construction = ConduitItem.construction;
                    objConduitItem.activation = ConduitItem.activation;
                    objConduitItem.accessibility = ConduitItem.accessibility;
                    objConduitItem.modified_by = userId;
                    objConduitItem.modified_on = DateTimeHelper.Now;
                    objConduitItem.audit_item_master_id = ConduitItem.audit_item_master_id;
                    return repo.Update(objConduitItem);
                }
                else
                {
                    ConduitItem.created_by = userId;
                    ConduitItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(ConduitItem);
                }
            }
            catch { throw; }
        }



    }

    public class DAMicroductItemMaster : Repository<MicroductItemMaster>
    {
        public MicroductItemMaster SaveMicroductItemTemplate(MicroductItemMaster DuctItem, int userId)
        {
            try
            {
                var objDuctItem = repo.Get(x => x.id == DuctItem.id);
                if (objDuctItem != null)
                {
                    objDuctItem.specification = DuctItem.specification;
                    objDuctItem.category = DuctItem.category;
                    objDuctItem.subcategory1 = DuctItem.subcategory1;
                    objDuctItem.subcategory2 = DuctItem.subcategory2;
                    objDuctItem.subcategory3 = DuctItem.subcategory3;
                    objDuctItem.vendor_id = DuctItem.vendor_id;
                    objDuctItem.item_code = DuctItem.item_code;
                    objDuctItem.type = DuctItem.type;
                    objDuctItem.brand = DuctItem.brand;
                    objDuctItem.model = DuctItem.model;
                    objDuctItem.construction = DuctItem.construction;
                    objDuctItem.activation = DuctItem.activation;
                    objDuctItem.accessibility = DuctItem.accessibility;
                    objDuctItem.modified_by = userId;
                    objDuctItem.modified_on = DateTimeHelper.Now;
                    objDuctItem.audit_item_master_id = DuctItem.audit_item_master_id;
                    objDuctItem.no_of_ways = DuctItem.no_of_ways;
                    return repo.Update(objDuctItem);
                }
                else
                {
                    DuctItem.created_by = userId;
                    DuctItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(DuctItem);
                }
            }
            catch { throw; }
        }



    }



    public class DATowerItemMaster : Repository<TowerItemMaster>
    {
        public TowerItemMaster SaveTowerItemTemplate(TowerItemMaster DuctItem, int userId)
        {
            try
            {
                var objDuctItem = repo.Get(x => x.id == DuctItem.id);
                if (objDuctItem != null)
                {
                    objDuctItem.specification = DuctItem.specification;
                    objDuctItem.category = DuctItem.category;
                    objDuctItem.subcategory1 = DuctItem.subcategory1;
                    objDuctItem.subcategory2 = DuctItem.subcategory2;
                    objDuctItem.subcategory3 = DuctItem.subcategory3;
                    objDuctItem.vendor_id = DuctItem.vendor_id;
                    objDuctItem.item_code = DuctItem.item_code;
                    objDuctItem.type = DuctItem.type;
                    objDuctItem.brand = DuctItem.brand;
                    objDuctItem.model = DuctItem.model;
                    objDuctItem.construction = DuctItem.construction;
                    objDuctItem.activation = DuctItem.activation;
                    objDuctItem.accessibility = DuctItem.accessibility;
                    objDuctItem.modified_by = userId;
                    objDuctItem.modified_on = DateTimeHelper.Now;
                    objDuctItem.audit_item_master_id = DuctItem.audit_item_master_id;
                    return repo.Update(objDuctItem);
                }
                else
                {
                    DuctItem.created_by = userId;
                    DuctItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(DuctItem);
                }
            }
            catch { throw; }
        }



    }


    public class DASectorItemMaster : Repository<SectorItemMaster>
    {
        public SectorItemMaster SaveSectorItemTemplate(SectorItemMaster DuctItem, int userId)
        {
            try
            {
                var objDuctItem = repo.Get(x => x.id == DuctItem.id);
                if (objDuctItem != null)
                {
                    objDuctItem.specification = DuctItem.specification;
                    objDuctItem.category = DuctItem.category;
                    objDuctItem.subcategory1 = DuctItem.subcategory1;
                    objDuctItem.subcategory2 = DuctItem.subcategory2;
                    objDuctItem.subcategory3 = DuctItem.subcategory3;
                    objDuctItem.vendor_id = DuctItem.vendor_id;
                    objDuctItem.item_code = DuctItem.item_code;
                    objDuctItem.type = DuctItem.type;
                    objDuctItem.brand = DuctItem.brand;
                    objDuctItem.model = DuctItem.model;
                    objDuctItem.construction = DuctItem.construction;
                    objDuctItem.activation = DuctItem.activation;
                    objDuctItem.accessibility = DuctItem.accessibility;
                    objDuctItem.modified_by = userId;
                    objDuctItem.modified_on = DateTimeHelper.Now;
                    objDuctItem.audit_item_master_id = DuctItem.audit_item_master_id;
                    return repo.Update(objDuctItem);
                }
                else
                {
                    DuctItem.created_by = userId;
                    DuctItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(DuctItem);
                }
            }
            catch { throw; }
        }



    }


    public class DAAntennaItemMaster : Repository<AntennaItemMaster>
    {
        public AntennaItemMaster SaveAntennaItemTemplate(AntennaItemMaster DuctItem, int userId)
        {
            try
            {
                var objDuctItem = repo.Get(x => x.id == DuctItem.id);
                if (objDuctItem != null)
                {
                    objDuctItem.specification = DuctItem.specification;
                    objDuctItem.category = DuctItem.category;
                    objDuctItem.subcategory1 = DuctItem.subcategory1;
                    objDuctItem.subcategory2 = DuctItem.subcategory2;
                    objDuctItem.subcategory3 = DuctItem.subcategory3;
                    objDuctItem.vendor_id = DuctItem.vendor_id;
                    objDuctItem.item_code = DuctItem.item_code;
                    objDuctItem.type = DuctItem.type;
                    objDuctItem.brand = DuctItem.brand;
                    objDuctItem.model = DuctItem.model;
                    objDuctItem.construction = DuctItem.construction;
                    objDuctItem.activation = DuctItem.activation;
                    objDuctItem.accessibility = DuctItem.accessibility;
                    objDuctItem.modified_by = userId;
                    objDuctItem.modified_on = DateTimeHelper.Now;
                    objDuctItem.audit_item_master_id = DuctItem.audit_item_master_id;
                    return repo.Update(objDuctItem);
                }
                else
                {
                    DuctItem.created_by = userId;
                    DuctItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(DuctItem);
                }
            }
            catch { throw; }
        }



    }


    public class DAMicrowaveLinkItemMaster : Repository<MicrowaveLinkItemMaster>
    {
        public MicrowaveLinkItemMaster SaveMicrowaveLinkItemTemplate(MicrowaveLinkItemMaster DuctItem, int userId)
        {
            try
            {
                var objDuctItem = repo.Get(x => x.id == DuctItem.id);
                if (objDuctItem != null)
                {
                    objDuctItem.specification = DuctItem.specification;
                    objDuctItem.category = DuctItem.category;
                    objDuctItem.subcategory1 = DuctItem.subcategory1;
                    objDuctItem.subcategory2 = DuctItem.subcategory2;
                    objDuctItem.subcategory3 = DuctItem.subcategory3;
                    objDuctItem.vendor_id = DuctItem.vendor_id;
                    objDuctItem.item_code = DuctItem.item_code;
                    objDuctItem.type = DuctItem.type;
                    objDuctItem.brand = DuctItem.brand;
                    objDuctItem.model = DuctItem.model;
                    objDuctItem.construction = DuctItem.construction;
                    objDuctItem.activation = DuctItem.activation;
                    objDuctItem.accessibility = DuctItem.accessibility;
                    objDuctItem.modified_by = userId;
                    objDuctItem.modified_on = DateTimeHelper.Now;
                    objDuctItem.audit_item_master_id = DuctItem.audit_item_master_id;
                    return repo.Update(objDuctItem);
                }
                else
                {
                    DuctItem.created_by = userId;
                    DuctItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(DuctItem);
                }
            }
            catch { throw; }
        }



    }


    public class DAWallMountItemMaster : Repository<WallMountItemMaster>
    {
        public WallMountItemMaster SaveWallMountItemTemplate(WallMountItemMaster WallMountItem, int userId)
        {
            try
            {
                var objWallMountItem = repo.Get(x => x.id == WallMountItem.id);
                if (objWallMountItem != null)
                {
                    objWallMountItem.specification = WallMountItem.specification;
                    objWallMountItem.category = WallMountItem.category;
                    objWallMountItem.subcategory1 = WallMountItem.subcategory1;
                    objWallMountItem.subcategory2 = WallMountItem.subcategory2;
                    objWallMountItem.subcategory3 = WallMountItem.subcategory3;
                    objWallMountItem.vendor_id = WallMountItem.vendor_id;
                    objWallMountItem.item_code = WallMountItem.item_code;
                    objWallMountItem.type = WallMountItem.type;
                    objWallMountItem.brand = WallMountItem.brand;
                    objWallMountItem.model = WallMountItem.model;
                    objWallMountItem.construction = WallMountItem.construction;
                    objWallMountItem.activation = WallMountItem.activation;
                    objWallMountItem.accessibility = WallMountItem.accessibility;
                    objWallMountItem.modified_by = userId;
                    objWallMountItem.modified_on = DateTimeHelper.Now;
                    objWallMountItem.audit_item_master_id = WallMountItem.audit_item_master_id;
                    return repo.Update(objWallMountItem);
                }
                else
                {
                    WallMountItem.created_by = userId;
                    WallMountItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(WallMountItem);
                }
            }
            catch { throw; }
        }



    }
    public class DAPatchCordItemMaster : Repository<PatchCordItemMaster>
    {
        public PatchCordItemMaster SavePatchCordItemTemplate(PatchCordItemMaster PatchCordItem, int userId)
        {
            try
            {
                var objCableItem = repo.Get(x => x.id == PatchCordItem.id && x.created_by == userId);
                if (objCableItem != null)
                {
                    objCableItem.specification = PatchCordItem.specification;
                    objCableItem.category = PatchCordItem.category;
                    objCableItem.subcategory1 = PatchCordItem.subcategory1;
                    objCableItem.subcategory2 = PatchCordItem.subcategory2;
                    objCableItem.subcategory3 = PatchCordItem.subcategory3;
                    objCableItem.vendor_id = PatchCordItem.vendor_id;
                    objCableItem.item_code = PatchCordItem.item_code;
                    objCableItem.type = PatchCordItem.type;
                    objCableItem.brand = PatchCordItem.brand;
                    objCableItem.model = PatchCordItem.model;
                    objCableItem.construction = PatchCordItem.construction;
                    objCableItem.activation = PatchCordItem.activation;
                    objCableItem.accessibility = PatchCordItem.accessibility;
                    objCableItem.modified_by = userId;
                    objCableItem.modified_on = DateTimeHelper.Now;
                    objCableItem.patch_cord_category = PatchCordItem.patch_cord_category;
                    objCableItem.patch_cord_sub_category = PatchCordItem.patch_cord_sub_category;
                    objCableItem.audit_item_master_id = PatchCordItem.audit_item_master_id;
                    return repo.Update(objCableItem);
                }
                else
                {
                    PatchCordItem.created_by = userId;
                    PatchCordItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(PatchCordItem);
                }
            }
            catch { throw; }
        }
    }


    //cabinet -shazia 
    public class DACabinetItemMaster : Repository<CabinetItemMaster>
    {
        public CabinetItemMaster SaveCabinetItemTemplate(CabinetItemMaster CabinetItem, int userId)
        {
            try
            {
                var objCabinetItem = repo.Get(x => x.id == CabinetItem.id);
                if (objCabinetItem != null)
                {
                    objCabinetItem.specification = CabinetItem.specification;
                    objCabinetItem.category = CabinetItem.category;
                    objCabinetItem.subcategory1 = CabinetItem.subcategory1;
                    objCabinetItem.subcategory2 = CabinetItem.subcategory2;
                    objCabinetItem.subcategory3 = CabinetItem.subcategory3;
                    objCabinetItem.vendor_id = CabinetItem.vendor_id;
                    objCabinetItem.item_code = CabinetItem.item_code;
                    objCabinetItem.type = CabinetItem.type;
                    objCabinetItem.brand = CabinetItem.brand;
                    objCabinetItem.model = CabinetItem.model;
                    objCabinetItem.construction = CabinetItem.construction;
                    objCabinetItem.activation = CabinetItem.activation;
                    objCabinetItem.accessibility = CabinetItem.accessibility;
                    objCabinetItem.modified_by = userId;
                    objCabinetItem.modified_on = DateTimeHelper.Now;
                    objCabinetItem.cabinet_type = CabinetItem.cabinet_type;
                    objCabinetItem.audit_item_master_id = CabinetItem.audit_item_master_id;
                    return repo.Update(objCabinetItem);
                }
                else
                {
                    CabinetItem.created_by = userId;
                    CabinetItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(CabinetItem);
                }
            }
            catch { throw; }
        }
    }
    //cabinet shazia 
    //vault shazia 
    //cabinet -shazia 
    public class DAVaultItemMaster : Repository<VaultItemMaster>
    {
        public VaultItemMaster SaveVaultItemTemplate(VaultItemMaster VaultItem, int userId)
        {
            try
            {
                var objVaultItem = repo.Get(x => x.id == VaultItem.id);
                if (objVaultItem != null)
                {
                    objVaultItem.specification = VaultItem.specification;
                    objVaultItem.category = VaultItem.category;
                    objVaultItem.subcategory1 = VaultItem.subcategory1;
                    objVaultItem.subcategory2 = VaultItem.subcategory2;
                    objVaultItem.subcategory3 = VaultItem.subcategory3;
                    objVaultItem.vendor_id = VaultItem.vendor_id;
                    objVaultItem.item_code = VaultItem.item_code;
                    objVaultItem.type = VaultItem.type;
                    objVaultItem.brand = VaultItem.brand;
                    objVaultItem.model = VaultItem.model;
                    objVaultItem.construction = VaultItem.construction;
                    objVaultItem.activation = VaultItem.activation;
                    objVaultItem.accessibility = VaultItem.accessibility;
                    objVaultItem.modified_by = userId;
                    objVaultItem.modified_on = DateTimeHelper.Now;
                    objVaultItem.vault_type = VaultItem.vault_type;
                    objVaultItem.audit_item_master_id = VaultItem.audit_item_master_id;
                    return repo.Update(objVaultItem);
                }
                else
                {
                    VaultItem.created_by = userId;
                    VaultItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(VaultItem);
                }
            }
            catch { throw; }
        }
    }
    //end vault shazia 

    //HANDHOLE BY ANTRA

    public class DAHandholeItemMaster : Repository<HandholeItemMaster>
    {
        public HandholeItemMaster SaveHandholeItemTemplate(HandholeItemMaster HandholeItem, int userId)
        {
            try
            {
                var objHandholeItem = repo.Get(x => x.id == HandholeItem.id);
                if (objHandholeItem != null)
                {
                    objHandholeItem.specification = HandholeItem.specification;
                    objHandholeItem.category = HandholeItem.category;
                    objHandholeItem.subcategory1 = HandholeItem.subcategory1;
                    objHandholeItem.subcategory2 = HandholeItem.subcategory2;
                    objHandholeItem.subcategory3 = HandholeItem.subcategory3;
                    objHandholeItem.vendor_id = HandholeItem.vendor_id;
                    objHandholeItem.item_code = HandholeItem.item_code;
                    objHandholeItem.type = HandholeItem.type;
                    objHandholeItem.brand = HandholeItem.brand;
                    objHandholeItem.model = HandholeItem.model;
                    objHandholeItem.construction = HandholeItem.construction;
                    objHandholeItem.activation = HandholeItem.activation;
                    objHandholeItem.accessibility = HandholeItem.accessibility;
                    objHandholeItem.modified_by = userId;
                    objHandholeItem.modified_on = DateTimeHelper.Now;
                    objHandholeItem.audit_item_master_id = HandholeItem.audit_item_master_id;
                    return repo.Update(objHandholeItem);
                }
                else
                {
                    HandholeItem.created_by = userId;
                    HandholeItem.created_on = DateTimeHelper.Now;
                    return repo.Insert(HandholeItem);
                }
            }
            catch { throw; }
        }
    }
    //END HANDHOLE BY ANTRA


    //PATCHPANEL BY SHAZIA
    public class DAPatchPanelItemMaster : Repository<PatchPanelItemMaster>
    {
        public PatchPanelItemMaster SavePatchPanelItemTemplate(PatchPanelItemMaster Item, int userId)
        {
            try
            {
                var objItem = repo.Get(x => x.id == Item.id);
                if (objItem != null)
                {
                    objItem.specification = Item.specification;
                    objItem.category = Item.category;
                    objItem.subcategory1 = Item.subcategory1;
                    objItem.subcategory2 = Item.subcategory2;
                    objItem.subcategory3 = Item.subcategory3;
                    objItem.vendor_id = Item.vendor_id;
                    objItem.item_code = Item.item_code;
                    objItem.type = Item.type;
                    objItem.brand = Item.brand;
                    objItem.model = Item.model;
                    objItem.construction = Item.construction;
                    objItem.activation = Item.activation;
                    objItem.accessibility = Item.accessibility;
                    objItem.no_of_port = Item.no_of_port;
                    objItem.modified_by = userId;
                    objItem.modified_on = DateTimeHelper.Now;
                    objItem.no_of_input_port = Item.no_of_input_port;
                    objItem.no_of_output_port = Item.no_of_output_port;
                    objItem.audit_item_master_id = Item.audit_item_master_id;
                    return repo.Update(objItem);
                }
                else
                {
                    Item.created_by = userId;
                    Item.created_on = DateTimeHelper.Now;
                    return repo.Insert(Item);
                }
            }
            catch { throw; }
        }
    }

}
//END  PATCHPANEL BY SHAZIA



