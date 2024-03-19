using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

namespace DataAccess
{
    public class DASaveEntityGeometry : Repository<object>
    {
        private static DASaveEntityGeometry objSaveEntityGeom = null;
        private static readonly object lockObject = new object();
        public static DASaveEntityGeometry Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSaveEntityGeom == null)
                    {
                        objSaveEntityGeom = new DASaveEntityGeometry();
                    }
                }
                return objSaveEntityGeom;
            }
        }
        public string SaveEntityGeom(InputGeom objgeom)
        {
            try
            {//,p_center_line_geom = objgeom.centerLineGeom
                var lstGeom = repo.ExecuteProcedure<object>("fn_save_entity_geom",
                    new
                    {
                        p_system_id = objgeom.systemId,
                        p_geom_type = objgeom.geomType,
                        p_entity_type = objgeom.entityType,
                        p_userid = objgeom.userId,
                        p_longlat = objgeom.longLat,
                        p_common_name = objgeom.commonName,
                        p_network_status = objgeom.networkStatus,
                        p_ports = objgeom.ports,
                        p_entity_category = objgeom.entity_category,                        
                        p_center_line_geom = objgeom.centerLineGeom,
                        p_buffer_width =Convert.ToDouble(objgeom.buffer_width),
                        p_project_id = objgeom.project_id != null ? objgeom.project_id : 0
                    });
                return lstGeom != null && lstGeom.Count > 0 ? lstGeom[0].ToString() : "0";
            }
            catch(Exception ex) { throw ex; }
        }
        public bool BoundaryIntersectCheck(string geom, string entityType)
        {
            try
            {
                var lstSystmId = repo.ExecuteProcedure<object>("fn_boundary_intersect_check", new { p_boundry_geom = geom, p_entity_type = entityType });
                return lstSystmId != null && lstSystmId.Count > 0 ? true : false;
            }
            catch { throw; }


        }
		public bool UpdateMicroductGeom(int systemId)
		{
			var dbMessage = repo.ExecuteProcedure<DbMessage>("fn_update_microduct_geom", new { p_system_id = systemId });
			return true;
		}

		public DbMessage ValidateEntityByGeom(EditGeomIn objgeom)
        {
            try
            {
                DbMessage dbMessage = repo.ExecuteProcedure<DbMessage>("fn_validate_geom_update", new { p_system_id = objgeom.systemId, p_geom_type = objgeom.geomType, p_entity_type = objgeom.entityType, p_userid = objgeom.userId, p_longlat = objgeom.longLat, p_bld_buffer = objgeom.Bld_Buffer, p_source_ref_type = objgeom.source_ref_type, p_source_ref_id = objgeom.source_ref_id }).FirstOrDefault();
                return dbMessage;
            }
            catch { throw; }
        }

        public DbMessage EditEntityGeometry(EditGeomIn objgeom)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_update_entity_geom", new { p_system_id = objgeom.systemId, p_geom_type = objgeom.geomType, p_entity_type = objgeom.entityType, p_userid = objgeom.userId, p_longlat = objgeom.longLat, p_network_status = objgeom.networkStatus, p_center_line_geom=objgeom.centerLineGeom }).FirstOrDefault();
                if (response.status && objgeom.geomType == GeometryType.Point.ToString())
                {
                    EditChildEntityGeometry(objgeom);
                }

                return response;
            }
            catch { throw; }
        }

        public DbMessage EditChildEntityGeometry(EditGeomIn objgeom)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_update_child_entity_geom", new { p_system_id = objgeom.systemId, p_geom_type = objgeom.geomType, p_entity_type = objgeom.entityType, p_userid = objgeom.userId, p_longlat = objgeom.longLat }).FirstOrDefault();

            }
            catch { throw; }
        }

        public bool UpdatePortInGeom(int systemId, string networkId, string entityType, string ports)
        {
            try
            {
                repo.ExecuteProcedure("fn_update_geom_port", new { p_system_id = systemId, p_network_id = networkId, p_entity_type = entityType, p_ports = ports });
                return true;
            }
            catch { throw; }
        }
        public GeometryDetail PointToPolygon(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<GeometryDetail>("fn_get_point_to_polygon", new { p_systemid = systemId, p_entitytype = entityType }).FirstOrDefault();
            }
            catch { throw; }
        }
        public DbMessage PolygonToPoint(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_save_polygon_to_point", new { p_systemid = systemId, p_entitytype = entityType }).FirstOrDefault();
            }
            catch { throw; }
        }

        public bool CheckGeomWithin(string geomBig, string geomSmall)
        {
            try
            {
                var lstSystmId = repo.ExecuteProcedure<bool>("fn_Within", new { biggeom = geomBig, smallgeom = geomSmall });
                return Convert.ToBoolean(lstSystmId[0]);
            }
            catch { throw; }
        }

        public void UpdateDuctLocation(int systemId, string distance, string OffsetDir)
        {
            try
            {
                repo.ExecuteProcedure("fn_update_duct_location", new { p_system_id = systemId, p_distance= distance, offsetdir = OffsetDir });

            }
            catch { throw; }
        }
        public void UpdateMicroductLocation(int systemId, string distance, string OffsetDir)
        {
            try
            {
                repo.ExecuteProcedure("fn_update_microduct_location", new { p_system_id = systemId, p_distance = distance, offsetdir = OffsetDir });

            }
            catch { throw; }
        }
        public void UpdateDuctColorCode(int systemId, int trench_id, int ductcount)
        {
            try
            {
                repo.ExecuteProcedure("fn_update_duct_color_code", new { p_system_id = systemId, trench_system_id = trench_id, ductCount = ductcount });

            }
            catch { throw; }
        }
        public void UpdateMicroductColorCode(int systemId, int trench_id, int ductcount)
        {
            try
            {
                repo.ExecuteProcedure("fn_update_microduct_color_code", new { p_system_id = systemId, trench_system_id = trench_id, ductCount = ductcount });

            }
            catch { throw; }
        }
    }


}
