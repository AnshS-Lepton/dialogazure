using DataAccess.DBHelpers;
using DataAccess.ISP;
using Models;
using Models.ISP;
using Models.VectorLayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.VectorLayers
{
    public class DAVectorLayers : Repository<dynamic>
    {
        DAVectorLayers()
        {

        }
        private static DAVectorLayers objDAVectorLayers = null;
        private static readonly object lockObject = new object();
        public static DAVectorLayers Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAVectorLayers == null)
                    {
                        objDAVectorLayers = new DAVectorLayers();
                    }
                }
                return objDAVectorLayers;
            }
        }

        #region Pole
        public GeoJson<dynamic> GetAllPolesVector()
        {
            try
            {
                var features = repo.ExecuteProcedure<Feature<dynamic>>("fn_get_pole_vector", new { }, true);
                return new GeoJson<dynamic>() { type = "FeatureCollection", features = features };
            }
            catch { throw; }
        }
        #endregion

        #region FDB
        public GeoJson<dynamic> GetAllFDBVector()
        {
            try
            {
                var features = repo.ExecuteProcedure<Feature<dynamic>>("fn_get_fdb_vector", new { }, true);
                return new GeoJson<dynamic>() { type = "FeatureCollection", features = features };
            }
            catch { throw; }
        }
        #endregion

        #region All
        public List<VectorFeatures<dynamic>> GetAllLayersVector(VectorDataIn vectorDataIn, out DateTime dbServerDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(vectorDataIn.connectionString))
                    connetionString = vectorDataIn.connectionString;
                DateTime dbDate = System.DateTime.Now;
                var allFeatures = repo.ExecuteProcedure<VectorFeatures<dynamic>>("fn_get_vector_layers_data", new { p_prvinceIds = vectorDataIn.PrvinceIds, p_entity_type  = vectorDataIn.entityType}, true);
                dbDate = repo.ExecuteProcedure<DateTime>("fn_get_DB_date", new { }, false).FirstOrDefault();
                dbServerDate = dbDate;
                return allFeatures;
            }
            catch { throw; }
        }

        public List<VectorFeatures<dynamic>> GetAllLayersVectorByGeom(VectorDataIn vectorDataIn, out DateTime dbServerDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(vectorDataIn.connectionString))
                    connetionString = vectorDataIn.connectionString;
                DateTime dbDate = System.DateTime.Now;
                var allFeatures = repo.ExecuteProcedure<VectorFeatures<dynamic>>("fn_get_vector_layers_data_by_geom", new { p_prvinceIds = vectorDataIn.PrvinceIds, p_longitude = vectorDataIn.lng, p_latitude = vectorDataIn.lat, p_ticket_id = vectorDataIn.ticketID }, true);
                dbDate = repo.ExecuteProcedure<DateTime>("fn_get_DB_date", new { }, false).FirstOrDefault();
                dbServerDate = dbDate;
                return allFeatures;
            }
            catch { throw; }
        }
        #endregion

        #region All Delta
        public List<VectorFeatures<dynamic>> GetAllLayersDelta(VectorDeltaIn vectorDeltaIn, out DateTime dbServerDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(vectorDeltaIn.connectionString))
                    connetionString = vectorDeltaIn.connectionString;
                DateTime dbDate = System.DateTime.Now;
                var allFeatures = repo.ExecuteProcedure<VectorFeatures<dynamic>>("fn_get_vector_layers_delta", new { p_lastFetchTime = vectorDeltaIn.LastFetchTime, p_prvinceIds = vectorDeltaIn.PrvinceIds, p_fsaID = vectorDeltaIn.FSAId }, true);
                dbDate = repo.ExecuteProcedure<DateTime>("fn_get_DB_date", new { }, false).FirstOrDefault();
                dbServerDate = dbDate;
                return allFeatures;
            }
            catch { throw; }
        }

        public List<VectorFeatures<dynamic>> GetAllLayersDeltaByGeom(VectorDeltaIn vectorDeltaIn, out DateTime dbServerDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(vectorDeltaIn.connectionString))
                    connetionString = vectorDeltaIn.connectionString;
                DateTime dbDate = System.DateTime.Now;
                var allFeatures = repo.ExecuteProcedure<VectorFeatures<dynamic>>("fn_get_vector_layers_delta_by_geom", new { p_lastFetchTime = vectorDeltaIn.LastFetchTime, p_prvinceIds = vectorDeltaIn.PrvinceIds, p_longitude = vectorDeltaIn.lng, p_latitude = vectorDeltaIn.lat, p_ticket_id = vectorDeltaIn.ticketID }, true);
                dbDate = repo.ExecuteProcedure<DateTime>("fn_get_DB_date", new { }, false).FirstOrDefault();
                dbServerDate = dbDate;
                return allFeatures;
            }
            catch { throw; }
        }
        #endregion

        #region Get Province BBOX
        public List<VectorFeatures<dynamic>> GetVectorProvinceData(VectorProvinceDataIn oVectorProvinceDataIn)
        {
            try
            {
                var allFeatures = repo.ExecuteProcedure<VectorFeatures<dynamic>>("fn_get_vector_layer_province_data", new { p_user_id = oVectorProvinceDataIn.UserId }, true);

                return allFeatures;
            }
            catch { throw; }
        }
        #endregion

        public List<LayerAttribute> GetVectorEntityStyle()
        {
            try
            {
                var allFeatures = repo.ExecuteProcedure<LayerAttribute>("fn_get_vector_layer_properties", new { }, true);

                return allFeatures;
            }
            catch { throw; }
        }
    }
}
