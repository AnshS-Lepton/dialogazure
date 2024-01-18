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
    public class DAInfoSetting : Repository<InfoSetting>
    {
        DAInfoSetting()
        {

        }

        private static DAInfoSetting objDAInfoSetting = null;
        private static readonly object lockObject = new object();
        public static DAInfoSetting Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAInfoSetting == null)
                    {
                        objDAInfoSetting = new DAInfoSetting();
                    }
                }
                return objDAInfoSetting;
            }
        }

        public InfoSetting SaveInfoSetting(InfoSetting objInfo)
        {
            var objExisting = repo.Get(x => x.layer_id == objInfo.layer_id);

            if (objExisting != null)
            {
                objExisting.info_attributes = objInfo.info_attributes != "" ? objInfo.info_attributes : objExisting.info_attributes;
                objExisting.modified_by = objInfo.user_id;
                objExisting.modified_on = DateTimeHelper.Now;
                repo.Update(objExisting);
            }
            else
            {
                objInfo.created_by = objInfo.user_id;
                objInfo.created_on = DateTimeHelper.Now;
                repo.Insert(objInfo);
            }
            return objInfo;
        }

        public InfoSetting GetInfoSetting(int layerId)
        {
            var objInfo = repo.Get(x => x.layer_id == layerId);
            return objInfo ?? new InfoSetting();
        }

    }
}
