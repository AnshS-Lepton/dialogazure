using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
   public  class BLSearch
    {
      public List<SearchResult> GetSearchEntityType(string srchText,int role_id)
        {
            return new DASearch().GetSearchEntityType(srchText, role_id);
        }
        public List<SearchResult> GetSearchLBLayerType(string srchText, int role_id)
        {
            return new DASearch().GetSearchLBLayerType(srchText, role_id);
        }
        public List<SearchResult> GetSearchEntityResult(string entityName, string srchText,int userId, string columnName)
        {
            return new DASearch().GetSearchEntityResult(entityName, srchText, userId, columnName);
        }
        public List<SearchResult> GetSearchLBLayerResult(string entityName, string srchText, int userId, string columnName)
        {
            return new DASearch().GetSearchLBLayerResult(entityName, srchText, userId, columnName);
        }
        public GeometryDetail GetGeometryDetails(GeomDetailIn objGeomDetailIn)
        {
            return new DASearch().GetGeometryDetails(objGeomDetailIn);
        }
        public GeometryDetail GetGeometryDetailsbygeom(int audit_id, string geomType)
        {
            return new DASearch().GetGeometryDetailsbygeom(audit_id, geomType);
        }
        public GeometryDetail GetLBLayerGeometryDetails(GeomDetailIn objGeomDetailIn)
        {
            return new DASearch().GetLBLayerGeometryDetails(objGeomDetailIn);
        }
        public GeometryDetail GetGeometryByLatlang( string geom)
        {
            return new DASearch().GetGeometryByLatlang(geom);
        }
        public List<Geometery> GetGeometryDetailsToPush(BoundaryPushFilter obj)
        {
            return new DASearch().GetGeometryDetailsToPush(obj);
        }
        public GISAttributes GetGisAttributes<GISAttributes>(BoundaryPushFilter obj)
        {
            return new DASearch().GetGisAttributes<GISAttributes>(obj);
        }
        public int UpdateEntityObjectId(int objectId, int system_id, string entityType)
        {
            return new DASearch().UpdateEntityObjectId(objectId,system_id, entityType);
        }
        public string GetGISApi(int system_id, string entityType, int objectId)
        {
            return new DASearch().GetGISApi(system_id, entityType, objectId);
        }
    }
}
