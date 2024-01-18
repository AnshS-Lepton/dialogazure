using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DASector : Repository<SectorMaster>
    {
        public SectorMaster SaveSectorEntity(SectorMaster objSectorMaster, int userId)
        {
            try
            {
                var resultItem = new SectorMaster();
                var objSector = repo.Get(x => x.system_id == objSectorMaster.system_id);
                if (objSector != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objSectorMaster.modified_on, objSector.modified_on, objSectorMaster.modified_by, objSector.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objSectorMaster.objPM = objPageValidate;
                        return objSectorMaster;
                    }
                    objSector.network_name = objSectorMaster.network_name;
                    objSector.azimuth = objSectorMaster.azimuth;
                    objSector.circuit_id = objSectorMaster.circuit_id;
                    objSector.down_link = objSectorMaster.down_link;
                    objSector.EntityReference = objSectorMaster.EntityReference;
                    objSector.ownership_type = objSectorMaster.ownership_type;
                    objSector.port_name = objSectorMaster.port_name;
                    objSector.remark = objSectorMaster.remark;
                    objSector.source_ref_description = objSectorMaster.source_ref_description;
                    objSector.source_ref_id = objSectorMaster.source_ref_id;
                    objSector.source_ref_type = objSectorMaster.source_ref_type;
                    objSector.uplink = objSectorMaster.uplink;
                    objSector.thirdparty_circuit_id = objSectorMaster.thirdparty_circuit_id;
                    objSector.technology = objSectorMaster.technology;
                    objSector.status = objSectorMaster.status;
                    objSector.status_remark = objSectorMaster.status_remark;
                    objSector.modified_by = userId;
                    objSector.modified_on = DateTimeHelper.Now;
                    objSector.project_id = objSectorMaster.project_id ?? 0;
                    objSector.planning_id = objSectorMaster.planning_id ?? 0;
                    objSector.workorder_id = objSectorMaster.workorder_id ?? 0;
                    objSector.purpose_id = objSectorMaster.purpose_id ?? 0;
                    objSector.vendor_id = objSectorMaster.vendor_id;
                    objSector.ownership_type = objSectorMaster.ownership_type;
                    objSector.item_code = objSectorMaster.item_code;
                    objSector.subcategory1 = objSectorMaster.subcategory1;
                    objSector.subcategory2 = objSectorMaster.subcategory2;
                    objSector.subcategory3 = objSectorMaster.subcategory3;
                    objSector.category = objSectorMaster.category;
                    objSector.specification = objSectorMaster.specification;
                    objSector.third_party_vendor_id = objSectorMaster.third_party_vendor_id;
                    objSector.brand_name = objSectorMaster.brand_name;
					objSector.sector_type = objSectorMaster.sector_type;
					objSector.frequency = objSectorMaster.frequency;
					objSector.parent_site_id = objSectorMaster.parent_site_id;
					objSector.node_identity = objSectorMaster.node_identity;
					objSector.sector_layer_id = objSectorMaster.sector_layer_id;

					objSector.installation = objSectorMaster.installation;
					objSector.installation_company = objSectorMaster.installation_company;
					objSector.installation_number = objSectorMaster.installation_number;
					objSector.installation_technician = objSectorMaster.installation_technician;
					objSector.installation_year = objSectorMaster.installation_year;
					objSector.production_year = objSectorMaster.production_year;
                    objSector.remarks = objSectorMaster.remarks;
                    objSector.total_tilt = objSectorMaster.total_tilt;
                    objSector.requested_by = objSectorMaster.requested_by;
                    objSector.request_approved_by = objSectorMaster.request_approved_by;
                    objSector.request_ref_id = objSectorMaster.request_ref_id;
                    objSector.origin_ref_id = objSectorMaster.origin_ref_id;
                    objSector.origin_ref_description = objSectorMaster.origin_ref_description;
                    objSector.origin_from = objSectorMaster.origin_from;
                    objSector.origin_ref_code = objSectorMaster.origin_ref_code;
                    resultItem = repo.Update(objSector);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Sector.ToString(), resultItem.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Sector.ToString(), resultItem.province_id);
                    EditGeomIn geom = new EditGeomIn();
                    geom.systemId = objSectorMaster.system_id;
                    geom.longLat = objSectorMaster.geom;
                    geom.entityType = EntityType.Sector.ToString();
                    geom.geomType = GeometryType.Polygon.ToString();
                    new DASaveEntityGeometry().EditEntityGeometry(geom);

                }
                else
                {
                    objSectorMaster.created_by = userId;
                    objSectorMaster.created_on = DateTimeHelper.Now;
                    objSectorMaster.status = "A";
                    objSectorMaster.network_status = "P";
                    resultItem = repo.Insert(objSectorMaster);

                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.longLat = objSectorMaster.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.Sector.ToString();
                    geom.commonName = resultItem.network_id;

                    geom.geomType = GeometryType.Polygon.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Sector.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Sector.ToString(), resultItem.province_id);
                }
                return resultItem;
            }
            catch { throw; }
        }
        public List<SectorMaster> GetSectorByTowerId(int systemId)
        {
            List<SectorMaster> lstSectors = repo.GetAll(m => m.parent_system_id == systemId).ToList();
            return lstSectors;
        }
        public int DeleteSectorById(int systemId)
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
        public SectorMaster getSectorDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
    }
}
