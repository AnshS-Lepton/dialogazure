using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DASearch : Repository<object>
    {
        public List<SearchResult> GetSearchEntityType(string srchText, int role_id)
        {
            try
            {
                return repo.ExecuteProcedure<SearchResult>("fn_get_search_entity_type", new { searchtext = srchText, p_role_id = role_id });
            }
            catch { throw; }
        }
        public List<SearchResult> GetSearchLBLayerType(string srchText, int role_id)
        {
            try
            {
                return repo.ExecuteProcedure<SearchResult>("fn_landbase_get_search_entity_type", new { searchtext = srchText, p_role_id = role_id });
            }
            catch { throw; }
        }
        public List<SearchResult> GetSearchEntityResult(string enName, string srchText,int userId, string columnName)
        {
            try
            {
                //CURRENTLY PROCEDURE WILL RETURN DATA IRRESPECTIVE OF USER ROLE (PROVINCE OR REGION BASED)
                // PENDING...
                return repo.ExecuteProcedure<SearchResult>("fn_get_search_entity_result", new { p_entityname = enName.Trim(), p_searchtext = srchText.Trim(), p_user_id = userId, p_columnName=columnName});
            }
            catch { throw; }
        }
        public List<SearchResult> GetSearchLBLayerResult(string enName, string srchText, int userId, string columnName)
        {
            try
            { 
                return repo.ExecuteProcedure<SearchResult>("fn_landbase_get_search_entity_result", new { p_entityname = enName.Trim(), p_searchtext = srchText.Trim(), p_user_id = userId, p_columnName = columnName });
            }
            catch { throw; }
        }
        public GeometryDetail GetGeometryDetails(GeomDetailIn objGeomDetailIn)
        {
            try
            {
                if (!string.IsNullOrEmpty(objGeomDetailIn.connectionString))
                    connetionString = objGeomDetailIn.connectionString;
                var lstGeomDetails = repo.ExecuteProcedure<GeometryDetail>("fn_get_geometrydetail", new { p_entitytype = objGeomDetailIn.entityType, p_geomtype = objGeomDetailIn.geomType, p_systemid = objGeomDetailIn.systemId,p_user_id= objGeomDetailIn.user_id });
                return lstGeomDetails != null && lstGeomDetails.Count > 0 ? lstGeomDetails[0] : new GeometryDetail();
            }
            catch { throw; }
        }
        public GeometryDetail GetGeometryDetailsbygeom(int audit_id, string geomType)
        {
            try
            {
                var lstGeomDetails = repo.ExecuteProcedure<GeometryDetail>("fn_get_geometrydetailbygeom", new { p_audit_id = audit_id, p_geomType= geomType });
                return lstGeomDetails != null && lstGeomDetails.Count > 0 ? lstGeomDetails[0] : new GeometryDetail();
            }
            catch { throw; }
        }
        public GeometryDetail GetGeometryByLatlang(string geom)
        {
            try
            {
                var lstGeomDetails = repo.ExecuteProcedure<GeometryDetail>("fn_get_geometryby_latlang", new { p_geom = geom });
                return lstGeomDetails != null && lstGeomDetails.Count > 0 ? lstGeomDetails[0] : new GeometryDetail();
            }
            catch { throw; }
        }
        public GeometryDetail GetLBLayerGeometryDetails(GeomDetailIn objGeomDetailIn)
        {
            try
            {
                var lstGeomDetails = repo.ExecuteProcedure<GeometryDetail>("fn_landbase_get_geometrydetail", new { p_entitytype = objGeomDetailIn.entityType, p_geomtype = objGeomDetailIn.geomType, p_systemid = objGeomDetailIn.systemId, p_user_id = objGeomDetailIn.user_id });
                return lstGeomDetails != null && lstGeomDetails.Count > 0 ? lstGeomDetails[0] : new GeometryDetail();
            }
            catch { throw; }
        }
        public List<Geometery> GetGeometryDetailsToPush(BoundaryPushFilter obj)
        {
            try
            {
                return repo.ExecuteProcedure<Geometery>("fn_get_rlcc_geometry_details", new { p_system_id = obj.system_id, p_entity_type = obj.entity_type },true).ToList();
            }
            catch { throw; }
        }
        public GISAttributes GetGisAttributes<GISAttributes>(BoundaryPushFilter obj)
        {
            try
            {
                return repo.ExecuteProcedure<GISAttributes>("fn_get_gis_attribute", new { p_system_id = obj.system_id, p_entity_type = obj.entity_type },true).FirstOrDefault();
            }
            catch { throw; }
        }
        public int UpdateEntityObjectId(int objectId,int system_id, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<int>("fn_update_rlcc_entity_details", new { p_objectid = objectId, p_system_id = system_id, p_entitytype = entityType }, false).FirstOrDefault();
            }
            catch { throw; }
        }
        public string GetGISApi( int system_id, string entityType, int objectId)
        {
            try
            {
                return repo.ExecuteProcedure<string>("fn_get_gis_api", new { p_system_id = system_id, p_entitytype = entityType, p_objectId=objectId }, false).FirstOrDefault();
            }
            catch { throw; }
        }
    }
}
