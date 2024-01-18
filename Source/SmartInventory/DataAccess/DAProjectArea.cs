using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;

namespace DataAccess
{
 public class DAProjectArea : Repository<ProjectArea>
    {
        public ProjectArea SaveProjectArea(ProjectArea objPArea, int userId)
        {  
            try
            {
                var objitem = repo.Get(x => x.system_id == objPArea.system_id);
                if (objitem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objPArea.modified_on, objitem.modified_on,objPArea.modified_by,objitem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objPArea.objPM = objPageValidate;
                        return objPArea;
                    }
                    objitem.projectarea_name = objPArea.projectarea_name;
                    objitem.remarks = objPArea.remarks;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    objitem.status_remark = objPArea.status_remark;
                    //objitem.area_rfs = objArea.area_rfs;
                    objitem.requested_by = objPArea.requested_by;
                    objitem.request_approved_by = objPArea.request_approved_by;
                    objitem.request_ref_id = objPArea.request_ref_id;
                    objitem.origin_ref_id = objPArea.origin_ref_id;
                    objitem.origin_ref_description = objPArea.origin_ref_description;
                    objitem.origin_from = objPArea.origin_from;
                    objitem.origin_ref_code = objPArea.origin_ref_code;
                    return repo.Update(objitem);
                }
                else
                {
                    objPArea.created_by = userId;
                    objPArea.created_on = DateTimeHelper.Now;
                    var resultItem = repo.Insert(objPArea);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.ProjectArea.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Polygon.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    return resultItem;
                }
            }
            catch { throw; }
        }

        public int DeleteProjectAreaById(int systemId)
        {
            try
            {
                var objArea = repo.Get(x => x.system_id == systemId);
                if (objArea != null)
                {
                    return repo.Delete(objArea.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }
    }
}
