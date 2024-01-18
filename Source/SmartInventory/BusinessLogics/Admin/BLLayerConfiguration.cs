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
using DataAccess.Admin;
using static Models.layerDetail;

namespace BusinessLogics.Admin
{
    public class BLLayerConfiguration
    {
        public List<layerDetail> GetLayerConfiguration()
        {
            return new DALayerConfiguration().GetLayerConfiguration();
        }
        public layerDetail GetLayerConfigurationById(int layerId)
        {
            return new DALayerConfiguration().GetLayerConfigurationById(layerId);
        }
        public layerDetail SaveLayerConfigurationById(layerDetail obj)
        {
            return new DALayerConfiguration().SaveLayerConfigurationById(obj);
        }

    }
    public class BLMapScales
    {
        public List<MapScaleSettings> GetZoomLevel()
        {
            return new DAMapScles().GetZoomLevel();
        }

    }
}
