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
    public class BLLandBaseLabelSetting
    {
        public BLLandBaseLabelSetting()
        {

        }
        private static BLLandBaseLabelSetting objBLLabelSetting = null;
        private static readonly object lockObject = new object();
        public static BLLandBaseLabelSetting Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBLLabelSetting == null)
                    {
                        objBLLabelSetting = new BLLandBaseLabelSetting();
                    }
                }
                return objBLLabelSetting;
            }
        }


        public List<AttributeDetail> GetLayerLabelAttributes(int layerId)
        {
            return DALandBaseLabelSetting.Instance.GetLayerLabelAttributes(layerId);
        }
        public Landbase_label_Settings SaveLabelSetting(Landbase_label_Settings objLabelSettings)
        {
            return DALandBaseLabelSetting.Instance.SaveLabelSetting(objLabelSettings);
        }

        public IList<Status> UpdateLabelSettingView(string layer_id, string label_column)
        {
            return DALandBaseLabelSetting.Instance.UpdateLabelSettingView(layer_id, label_column);

        }
    }
}
