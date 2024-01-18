using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Admin
{
    public class DALandBaseLabelSetting : Repository<Landbase_label_Settings>
    {
        DALandBaseLabelSetting()
        {

        }

        private static DALandBaseLabelSetting objDAInfoSetting = null;
        private static readonly object lockObject = new object();
        public static DALandBaseLabelSetting Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAInfoSetting == null)
                    {
                        objDAInfoSetting = new DALandBaseLabelSetting();
                    }
                }
                return objDAInfoSetting;
            }
        }

        public List<AttributeDetail> GetLayerLabelAttributes(int layerId)
        {
            try
            {
                return repo.ExecuteProcedure<AttributeDetail>("fn_landbase_get_label_attributes", new { p_layerId = layerId }, true);

            }
            catch { throw; }
        }

        public Landbase_label_Settings SaveLabelSetting(Landbase_label_Settings objLabelSettings)
        {
            var objExisting = repo.Get(x => x.landbase_layer_id == objLabelSettings.landbase_layer_id);

            if (objExisting != null)
            {
                
                objExisting.modified_by = objLabelSettings.user_id;
                objExisting.modified_on = DateTimeHelper.Now;
                objExisting.label_columns = objLabelSettings.label_columns;
                repo.Update(objExisting);
            }
            else
            {
                objLabelSettings.created_by = objLabelSettings.user_id;
                objLabelSettings.created_on = DateTimeHelper.Now;
                objLabelSettings.modified_by = objLabelSettings.user_id;
                objLabelSettings.modified_on = DateTimeHelper.Now;
                repo.Insert(objLabelSettings);
            }
            return objLabelSettings;
        }



        public List<Status> UpdateLabelSettingView(string layer_id, string label_column)
        {
            try
            {

                var res = repo.ExecuteProcedure<Status>("fn_updatelabelvalue_map", new
                {
                    entity_type = layer_id,
                    label_column = label_column
                    
                });

                return res;
            }
            catch { throw; }
        }
         
    }
}
