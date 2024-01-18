using DataAccess.Admin;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLSearchSetting
    {
        public BLSearchSetting()
        {

        }

        private static BLSearchSetting objBLSearchSetting= null;
        private static readonly object lockObject = new object();
        public static BLSearchSetting Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBLSearchSetting == null)
                    {
                        objBLSearchSetting = new BLSearchSetting();
                    }
                }
                return objBLSearchSetting;
            }
        }

        public List<AttributeDetail> GetLayerSearchAttributes(int layerId)
        {
            return DASearchSetting.Instance.GetLayerSearchAttributes(layerId);
        }

        public SearchSetting GetSearchSetting(int layerId)
        {
            return DASearchSetting.Instance.GetSearchSetting(layerId);
        }
        public SearchSetting SaveSearchSetting(SearchSetting objSearch)
        {
            return DASearchSetting.Instance.SaveSearchSetting(objSearch);
        }

    }
}
