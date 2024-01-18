using DataAccess.Admin;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLLabelSetting
    {
        public BLLabelSetting()
      {

      }
        private static BLLabelSetting objBLLabelSetting = null;
      private static readonly object lockObject = new object();
      public static BLLabelSetting Instance
      {
          get
          {
              lock (lockObject)
              {
                  if (objBLLabelSetting == null)
                  {
                      objBLLabelSetting = new BLLabelSetting();
                  }
              }
              return objBLLabelSetting;
          }
      }


      public List<AttributeDetail> GetLayerLabelAttributes(int layerId)
      {   
          return DALabelSetting.Instance.GetLayerLabelAttributes(layerId);
      }
      public LabelSetting SaveLabelSetting(LabelSetting objLabelSettings)
      {
          return DALabelSetting.Instance.SaveLabelSetting(objLabelSettings);
         
      }

      public IList<Status> UpdateLabelSettingView(string layer_id, string label_column)
      {
          return DALabelSetting.Instance.UpdateLabelSettingView(layer_id, label_column);

      }

        public List<AttributeDetail> GetCategorylistbylayername(string layer_name)
        {
            return DALabelSetting.Instance.GetCategorylistbylayername(layer_name);
        }
        public List<AttributeDetail> GetLayerStyleColumn(int layerId)
        {
            return DALabelSetting.Instance.GetLayerStyleColumn(layerId);
        }
    }
}
