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
    public class DALabelSetting : Repository<LabelSetting>
    {
        DALabelSetting()
        {

        }

        private static DALabelSetting objDAInfoSetting = null;
        private static readonly object lockObject = new object();
        public static DALabelSetting Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAInfoSetting == null)
                    {
                        objDAInfoSetting = new DALabelSetting();
                    }
                }
                return objDAInfoSetting;
            }
        }

        public List<AttributeDetail> GetLayerLabelAttributes(int layerId)
        {
            try
            {
                return repo.ExecuteProcedure<AttributeDetail>("fn_get_layer_label_attributes", new { p_layerId = layerId }, true);

            }
            catch { throw; }
        }

        public LabelSetting SaveLabelSetting(LabelSetting objLabelSettings)
        {
            var objExisting = repo.Get(x => x.layer_id == objLabelSettings.layer_id);

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


        //public InfoSetting GetInfoSetting(int layerId)
        //{
        //    var objInfo = repo.Get(x => x.layer_id == layerId);
        //    return objInfo ?? new InfoSetting();
        //}
        public List<AttributeDetail> GetCategorylistbylayername(string layer_name)
        {
            try
            {
                return repo.ExecuteProcedure<AttributeDetail>("fn_get_entity_category_list", new { p_layer_name = layer_name }, true);

            }
            catch { throw; }
        }

        public List<AttributeDetail> GetLayerStyleColumn(int layerId)
        {
            try
            {
                return repo.ExecuteProcedure<AttributeDetail>("fn_get_label_columns", new { p_layerId = layerId }, true);

            }
            catch { throw; }
        }
    }
}
