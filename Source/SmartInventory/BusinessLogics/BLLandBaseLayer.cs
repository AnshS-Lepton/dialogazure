using BusinessLogics;
using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLLandBaseLayer
    {
        DALandBaseLayer objDALandbase = new DALandBaseLayer();
        public List<LandBaseMaster> getlayerNamebyGeom(string geomType, bool is_buffer_enable)
        {
            return new DALandBaseMaster().getlayerNamebyGeom(geomType, is_buffer_enable);
        } 

        public LandBaseLayer GetLandbaseEntityById(int id, string geomType)
        {
            return new DALandBaseLayer().GetLandbaseEntityById(id, geomType);
        }

        public LandBaseLayer SaveLandBase(LandBaseLayer objLandBase, int userId)
        {
            return new DALandBaseLayer().SaveLandBase(objLandBase, userId);
        }
        public LandBaseNetworkCodeOut GetNetworkId(LandbaseNetworkCodeIn objLandbaseNetworkCode)
        {
            return new DALandBaseLayer().GetNetworkId(objLandbaseNetworkCode);
        }
        public LandBaseMaster getLayerNamebyId(int layerId)
        {
            return new DALandBaseMaster().getLayerNamebyId(layerId);
        }

        public List<LandBaseDetail> getNearByLandbaseEntities(double latitude, double longitude, int bufferInMtr, int user_id = 0)
        {
            return objDALandbase.getNearByLandbaseEntities(latitude, longitude, bufferInMtr, user_id);

        }
        public List<LandBaseLayerSummaryReport> GetExportReportSummary(ExportLandBaseLayerReportFilter objReportFilter)
        {
            return new DALandBaseLayer().GetExportReportSummary(objReportFilter);
        }
        public List<Dictionary<string, string>> GetLandBaseExportReportSummaryView(ExportLandBaseLayerSummaryViewFilter objReportFilter)
        {
            return new DALandBaseLayer().GetLandBaseExportReportSummaryView(objReportFilter);
        }
        public List<Dictionary<string, string>> GetExportLandBaseLayerSummaryViewKML(ExportLandBaseLayerSummaryViewFilter objReportFilter)
        {
            return new DALandBaseLayer().GetExportLandBaseLayerSummaryViewKML(objReportFilter);
        }
        

        public Dictionary<string, string> getLandBaseEntityInfo(int systemId, string entityType, string settingType)
        {
            return objDALandbase.getLandbaseEntityInfo(systemId, entityType, settingType);
        }
        public List<T> GetLandbaseEntityExportData<T>(int systemid, string eType, string settingType) where T : new()
        {
            return objDALandbase.GetLandbaseEntityExportData<T>(systemid, eType, settingType);
        }

        public GeometryDetail GetLandbaseGeometryDetails(GeomDetailIn objGeomDetailIn)
        {
            return new DALandBaseLayer().GetLandbaseGeometryDetails(objGeomDetailIn);
        }

        public DbMessage deleteEntity(int systemId, string entityType, string geomType)
        {
            return new DALandBaseLayer().deleteEntity(systemId, entityType, geomType);
        }

        public bool chkEntityDataExist(int systemid)
        {
            return new DALandBaseLayer().chkEntityDataExist(systemid);
        }

        public DbMessage EditLandbaseEntityGeometry(EditGeomIn objgeom)
        {
            return new DALandBaseLayer().EditLandbaseEntityGeometry(objgeom);
        }

        public List<LandbaseDropdownMaster> GetLandbaseDropdown(int landbase_layer_id, string category_type, int? category_parent_id)
        {
            return new DALandBaseLayer().GetLandbaseDropdown(landbase_layer_id, category_type, category_parent_id);
        }
        public List<LandBaseMaster> getLandBaseLayers()
        {
            return new DALandBaseMaster().getLandBaseLayers();
        }
        public List<LandBaseMaster> getLandBaseLayersByName(string layerName)
        {
            return new DALandBaseMaster().getLandBaseLayersByName(layerName);
        }
        
    }

}
