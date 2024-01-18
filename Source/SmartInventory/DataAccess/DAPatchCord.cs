using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;

namespace DataAccess
{
    public class DAPatchCord : Repository<PatchCordMaster>
    {
        private static DAPatchCord objPatch = null;
        private static readonly object lockObject = new object();
        public static DAPatchCord Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objPatch == null)
                    {
                        objPatch = new DAPatchCord();
                    }
                }
                return objPatch;
            }
        }
        public PatchCordMaster SavePatchCord(PatchCordMaster objPatch, int userId)
        {
            try
            {
                var objPatchCord = repo.Get(u => u.system_id == objPatch.system_id);
                if (objPatchCord != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objPatch.modified_on, objPatchCord.modified_on, objPatch.modified_by, objPatchCord.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objPatch.objPM = objPageValidate;
                        return objPatch;
                    }
                    objPatchCord.a_location = objPatch.a_location;
                    objPatchCord.b_location = objPatch.b_location;
                    objPatchCord.pin_code = objPatch.pin_code;
                    objPatchCord.patch_cord_name = objPatch.patch_cord_name;
                    objPatchCord.specification = objPatch.specification;
                    objPatchCord.category = objPatch.category;
                    objPatchCord.subcategory1 = objPatch.subcategory1;
                    objPatchCord.subcategory2 = objPatch.subcategory2;
                    objPatchCord.subcategory3 = objPatch.subcategory3;
                    objPatchCord.item_code = objPatch.item_code;
                    objPatchCord.vendor_id = objPatch.vendor_id;
                    objPatchCord.type = objPatch.type;
                    objPatchCord.brand = objPatch.brand;
                    objPatchCord.model = objPatch.model;
                    objPatchCord.construction = objPatch.construction;
                    objPatchCord.activation = objPatch.activation;
                    objPatchCord.accessibility = objPatch.accessibility;
                    objPatchCord.modified_on = DateTimeHelper.Now;
                    objPatchCord.modified_by = userId;
                    objPatchCord.project_id = objPatch.project_id ?? 0;
                    objPatchCord.planning_id = objPatch.planning_id ?? 0;
                    objPatchCord.workorder_id = objPatch.workorder_id ?? 0;
                    objPatchCord.purpose_id = objPatch.purpose_id ?? 0;
                    objPatchCord.patch_cord_category = objPatch.patch_cord_category;
                    objPatchCord.patch_cord_sub_category = objPatch.patch_cord_sub_category;
                    objPatchCord.remarks = objPatch.remarks;                   
                    objPatchCord.execution_method = objPatch.execution_method;
                    objPatchCord.a_system_id = objPatch.a_system_id > 0 ? objPatch.a_system_id : objPatchCord.a_system_id;
                    objPatchCord.a_entity_type = !string.IsNullOrWhiteSpace(objPatch.a_entity_type) ? objPatch.a_entity_type : objPatchCord.a_entity_type;
                    objPatchCord.b_system_id = objPatch.b_system_id > 0 ? objPatch.b_system_id : objPatchCord.b_system_id; ;
                    objPatchCord.b_entity_type = !string.IsNullOrWhiteSpace(objPatch.b_entity_type) ? objPatch.b_entity_type : objPatchCord.b_entity_type;
                    objPatchCord.audit_item_master_id = objPatch.audit_item_master_id;
                    var response = repo.Update(objPatchCord);
                    return response;
                }
                else
                {
                    var latLong = objPatch.geom;
                    objPatch.status = "A";
                    objPatch.network_status = NetworkStatus.A.ToString();                   
                    objPatch.created_on = DateTimeHelper.Now;
                    objPatch.created_by = userId;
                    objPatch = repo.Insert(objPatch);

                    InputGeom geom = new InputGeom();
                    geom.systemId = objPatch.system_id;
                    geom.longLat = latLong;
                    geom.userId = userId;
                    geom.entityType = EntityType.PatchCord.ToString();
                    geom.commonName = objPatch.network_id;
                    geom.geomType = GeometryType.Line.ToString();
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);

                    return objPatch;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
