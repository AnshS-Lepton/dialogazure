using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DATree : Repository<TreeMaster>
    {
       
        public TreeMaster SaveEntityTree(TreeMaster objTreeMaster, int userId)
        {
            try
            {
                var objTreeItem = repo.Get(x => x.system_id == objTreeMaster.system_id);
                if (objTreeItem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objTreeMaster.modified_on, objTreeItem.modified_on, objTreeMaster.modified_by, objTreeItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objTreeMaster.objPM = objPageValidate;
                        return objTreeMaster;
                    }
                    objTreeItem.tree_name = objTreeMaster.tree_name;
                    objTreeItem.tree_no = objTreeMaster.tree_no;
                    objTreeItem.tree_height = objTreeMaster.tree_height;
                    objTreeItem.address = objTreeMaster.address;

                    objTreeItem.specification = objTreeMaster.specification;
                    objTreeItem.category = objTreeMaster.category;
                    objTreeItem.subcategory1 = objTreeMaster.subcategory1;
                    objTreeItem.subcategory2 = objTreeMaster.subcategory2;
                    objTreeItem.subcategory3 = objTreeMaster.subcategory3;
                    objTreeItem.item_code = objTreeMaster.item_code;
                    objTreeItem.vendor_id = objTreeMaster.vendor_id;
                    objTreeItem.type = objTreeMaster.type;
                    objTreeItem.brand = objTreeMaster.brand;
                    objTreeItem.model = objTreeMaster.model;
                    objTreeItem.construction = objTreeMaster.construction;
                    objTreeItem.activation = objTreeMaster.activation;
                    objTreeItem.accessibility = objTreeMaster.accessibility;
                    objTreeItem.modified_by = userId;
                    objTreeItem.modified_on = DateTimeHelper.Now;

                    objTreeItem.project_id = objTreeMaster.project_id ?? 0;
                    objTreeItem.planning_id = objTreeMaster.planning_id ?? 0;
                    objTreeItem.workorder_id = objTreeMaster.workorder_id ?? 0;
                    objTreeItem.purpose_id = objTreeMaster.purpose_id ?? 0;
                    objTreeItem.audit_item_master_id = objTreeMaster.audit_item_master_id;
                    objTreeItem.primary_pod_system_id = objTreeMaster.primary_pod_system_id;
                    objTreeItem.secondary_pod_system_id = objTreeMaster.secondary_pod_system_id;
                    objTreeItem.status_remark = objTreeMaster.status_remark;
                    objTreeItem.remarks = objTreeMaster.remarks;

                    if (!string.IsNullOrEmpty(objTreeMaster.source_ref_type))
                        objTreeItem.source_ref_type = objTreeMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objTreeMaster.source_ref_id))
                        objTreeItem.source_ref_id = objTreeMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objTreeMaster.source_ref_description))
                        objTreeItem.source_ref_description = objTreeMaster.source_ref_description;

                    objTreeItem.other_info = objTreeMaster.other_info;  //for additional-attributes
                    objTreeItem.requested_by = objTreeMaster.requested_by;
                    objTreeItem.request_approved_by = objTreeMaster.request_approved_by;
                    objTreeItem.request_ref_id = objTreeMaster.request_ref_id;
                    objTreeItem.origin_ref_id = objTreeMaster.origin_ref_id;
                    objTreeItem.origin_ref_description = objTreeMaster.origin_ref_description;
                    objTreeItem.origin_from = objTreeMaster.origin_from;
                    objTreeItem.origin_ref_code = objTreeMaster.origin_ref_code;
                    objTreeItem.gis_design_id = objTreeMaster.gis_design_id;
                    var TreeResp = repo.Update(objTreeItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(TreeResp.system_id, Models.EntityType.Tree.ToString(), TreeResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Tree.ToString(), TreeResp.province_id);
                    return TreeResp;
                }
                else
                {
                    objTreeMaster.created_by = userId;
                    objTreeMaster.created_on = DateTimeHelper.Now;
                    //objTreeMaster.status = "A";
                    //objTreeMaster.network_status = "P";
                    objTreeMaster.status = String.IsNullOrEmpty(objTreeMaster.status) ? "A" : objTreeMaster.status;
                    objTreeMaster.network_status = String.IsNullOrEmpty(objTreeMaster.network_status) ? "P" : objTreeMaster.network_status;
                    var resultItem = repo.Insert(objTreeMaster);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Tree.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = resultItem.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Tree.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Tree.ToString(), resultItem.province_id);

                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteTreeById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }
        //public List<TreeArea> GetTreeArea(string geom)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<TreeArea>("fn_get_area", new { p_geometry = geom, p_geomtype = GeometryType.Point.ToString() });
        //    }
        //    catch { throw; }
        //}

        #region Additional-Attributes
        public string GetOtherInfoTree(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
    }
}
