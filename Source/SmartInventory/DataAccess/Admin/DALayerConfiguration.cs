using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;
using DataAccess.DBContext;
using DataAccess.Contracts;
using Models.Admin;

namespace DataAccess.Admin
{
    public class DALayerConfiguration : Repository<layerDetail>
    {
        public List<layerDetail> GetLayerConfiguration()
        {
            try
            {
                return repo.GetAll()
                    .Where(m => m.isvisible==true) // Filter for IsVisible = true
                    .OrderBy(m => m.layer_title).ToList();
            }
            catch { throw; }
        }
        public layerDetail GetLayerConfigurationById(int layerId)
        {
            try
            {
                return repo.Get(m => m.layer_id == layerId);
            }
            catch { throw; }
        }
        public layerDetail SaveLayerConfigurationById(layerDetail obj)
        {
            try
            {
                var newObj = repo.Get(m => m.layer_id == obj.layer_id);
                if (obj.layer_id != 0)
                {
                    newObj.is_mobile_layer = obj.is_mobile_layer;
                    newObj.is_visible_in_mobile_lib = obj.is_visible_in_mobile_lib;
                    newObj.is_visible_on_mobile_map = obj.is_visible_on_mobile_map;
                    newObj.minzoomlevel = obj.minzoomlevel;
                    //newObj.maxzoomlevel = obj.maxzoomlevel;
                    return repo.Update(newObj);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }
    }
    public class DAMapScles : Repository<MapScaleSettings>
    {
        public List<MapScaleSettings> GetZoomLevel()
        {
            try
            {
                return repo.GetAll().OrderBy(m => m.zoom).GroupBy(m => m.zoom).Select(g => g.First()).ToList();

            }
            catch { throw; }
        }

    }
}
