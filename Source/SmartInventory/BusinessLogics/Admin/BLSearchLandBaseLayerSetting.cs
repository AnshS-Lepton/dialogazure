using DataAccess.Admin;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLSearchLandBaseLayerSetting
    {
        public BLSearchLandBaseLayerSetting()
        {

        }
        private static BLSearchLandBaseLayerSetting objBLSearchSetting = null;
        private static readonly object lockObject = new object();
        public static BLSearchLandBaseLayerSetting Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBLSearchSetting == null)
                    {
                        objBLSearchSetting = new BLSearchLandBaseLayerSetting();
                    }
                }
                return objBLSearchSetting;
            }
        }

        public List<AttributeDetail> GetLayerSearchAttributes(int layerId)
        {
            return DASearchLandBaseLayerSetting.Instance.GetLayerSearchAttributes(layerId);
        } 
         
        public SearchLandBaseLayerSetting SaveSearchSetting(SearchLandBaseLayerSetting objSearch)
        {
            return DASearchLandBaseLayerSetting.Instance.SaveSearchSetting(objSearch);
        }
    }
}
