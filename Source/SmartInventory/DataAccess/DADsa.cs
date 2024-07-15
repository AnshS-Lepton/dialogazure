using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

namespace DataAccess
{
   public class DADsa:Repository<DSA>
    {
        public DSA SaveDSA(DSA objDsa, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == objDsa.system_id);
                if (objitem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objDsa.modified_on, objitem.modified_on, objDsa.modified_by, objitem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objDsa.objPM = objPageValidate;
                        return objDsa;
                    }
                    objitem.dsa_name = objDsa.dsa_name;
                    objitem.remarks = objDsa.remarks;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    objitem.status_remark = objDsa.status_remark;
                    objitem.requested_by = objDsa.requested_by;
                    objitem.request_approved_by = objDsa.request_approved_by;
                    objitem.request_ref_id = objDsa.request_ref_id;
                    objitem.origin_ref_id = objDsa.origin_ref_id;
                    objitem.origin_ref_description = objDsa.origin_ref_description;
                    objitem.origin_from = objDsa.origin_from;
                    objitem.origin_ref_code = objDsa.origin_ref_code;
                    objitem.no_of_home_pass = objDsa.no_of_home_pass;
                    objitem.served_by_ring = objDsa.served_by_ring;
                    objitem.gis_design_id = objDsa.gis_design_id;
                    var DSAResp = repo.Update(objitem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(DSAResp.system_id,Models.EntityType.DSA.ToString(), DSAResp.province_id,1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata( Models.EntityType.DSA.ToString(), DSAResp.province_id);
                    return DSAResp;
                }
                else
                {
                    objDsa.created_by = userId;
                    objDsa.created_on = DateTimeHelper.Now;
                    objDsa.status = (string.IsNullOrEmpty(objDsa.status) ? "A" : objDsa.status);

                    var resultItem = repo.Insert(objDsa);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.DSA.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Polygon.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.DSA.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.DSA.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }

        public int DeleteDSAById(int systemId)
        {
            try
            {
                var objDsa = repo.Get(x => x.system_id == systemId);
                if (objDsa != null)
                {
                    return repo.Delete(objDsa.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }
        
    }


    public class DACsa:Repository<CSA>
    {
        public List<CSAIn> GetDSAExist(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<CSAIn>("fn_get_dsa_exist", new { p_geometry = geom, p_geomtype = GeometryType.Polygon.ToString() });
            }
            catch { throw; }
        }

        

        public CSA SaveCSA(CSA objCsa, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == objCsa.system_id);
                if (objitem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objCsa.modified_on, objitem.modified_on, objCsa.modified_by, objitem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objCsa.objPM = objPageValidate;
                        return objCsa;
                    }
                    objitem.csa_name = objCsa.csa_name;
                    objitem.remarks = objCsa.remarks;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    objitem.status_remark = objCsa.status_remark;
                    objitem.requested_by = objCsa.requested_by;
                    objitem.request_approved_by = objCsa.request_approved_by;
                    objitem.request_ref_id = objCsa.request_ref_id;
                    objitem.origin_ref_id = objCsa.origin_ref_id;
                    objitem.origin_ref_description = objCsa.origin_ref_description;
                    objitem.origin_from = objCsa.origin_from;
                    objitem.origin_ref_code = objCsa.origin_ref_code;
                    objitem.no_of_home_pass = objCsa.no_of_home_pass;
                    var resultItem = repo.Update(objitem);
                    DbMessage entityObj =new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.CSA.ToString(), resultItem.province_id, 1);
                    //DbMessage geojsonObj = updateGeojsonMetadata(Models.EntityType.CSA.ToString(), resultItem.province_id);
                    return resultItem;
                }
                else
                {
                    objCsa.created_by = userId;
                    objCsa.created_on = DateTimeHelper.Now;
                    objCsa.status = (string.IsNullOrEmpty(objCsa.status) ? "A" : objCsa.status);

                    //objCsa.building_code = objCsa.building_code;
                    var resultItem = repo.Insert(objCsa);
                    //TRANSACTION NEED TO IMPLEMENT THERE...
                    //Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.CSA.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Polygon.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj =new DAMisc(). updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.CSA.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = updateGeojsonMetadata(Models.EntityType.CSA.ToString(), resultItem.province_id);
                    return resultItem;

                }
            }
            catch { throw; }
        }


        public DbMessage CalculateHomePasses(int system_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_calculate_home_passes", new { p_system_id = system_id }).FirstOrDefault();
            }
            catch { throw; }
        }

        public CSA UpdateRfsStatus(string designId, string rfsStaus)
        {
            try
            {
                var objitem = repo.Get(x => x.gis_design_id == designId);
                if (objitem != null && rfsStaus.ToUpper()== "S2_RFS")
                { 

                    objitem.rfs_status = rfsStaus;
                    return repo.Update(objitem);
                }
                else
                { 
                    return null;
                }
            }
            catch { throw; }
        }
       
    }
}
