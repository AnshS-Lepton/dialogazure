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
    public class DASearchLandBaseLayerSetting : Repository<SearchLandBaseLayerSetting>
    {
        DASearchLandBaseLayerSetting()
        {

        }

        private static DASearchLandBaseLayerSetting objDASearchSetting = null;
        private static readonly object lockObject = new object();
        public static DASearchLandBaseLayerSetting Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDASearchSetting == null)
                    {
                        objDASearchSetting = new DASearchLandBaseLayerSetting();
                    }
                }
                return objDASearchSetting;
            }
        }
        public List<AttributeDetail> GetLayerSearchAttributes(int layerId)
        {
            try
            {    
                   var res = repo.ExecuteProcedure<AttributeDetail>("fn_landbase_layer_get_search_attributes", new { p_layerId = layerId });
                return res;
            }
            catch { throw; }
        } 
        

        public SearchLandBaseLayerSetting SaveSearchSetting(SearchLandBaseLayerSetting objSearch)
        {
            var objExisting = repo.Get(x => x.landbase_layer_id == objSearch.landbase_layer_id);

            if (objExisting != null)
            {
                objExisting.search_columns = objSearch.search_columns;
                objExisting.modified_by = objSearch.user_id;
                objExisting.modified_on = DateTimeHelper.Now;
                repo.Update(objExisting);
            }
            else
            {
                objSearch.created_by = objSearch.user_id;
                objSearch.created_on = DateTimeHelper.Now;
                repo.Insert(objSearch);
            }
            return objSearch;
        }
    }
}
