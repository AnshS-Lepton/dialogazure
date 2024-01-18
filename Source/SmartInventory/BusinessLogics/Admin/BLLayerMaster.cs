using DataAccess;
using DataAccess.Admin;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLLayerMaster
    {


        public IList<ViewLayerDetailList> GetLayerSettingDetailsList(ViewLayerSettingDetailList model)
        {
            return new DALayerMaster().GetLayerSettingDetailsList(model);
        }
        public List<Dictionary<string, string>> GetLayerSettingDetails(ViewLayerSettingDetailList model)
        {
            return new DALayerMaster().GetLayerSettingDetails(model);
        }

        public layerDetail getFullText(int id)
        {
            return new DALayerMaster().getFullText(id);

        }
        public List<LayerDetailsColumn> GetLayerDetailById(int id)
        {
            return new DALayerMaster().GetLayerDetailById(id);
        }
        public DbMessage SaveLayerSettings(string objlayerdetail, int layer_id)
        {
            return new DALayerMaster().SavelayerSettingDetails(objlayerdetail,layer_id);
        }
}
}
