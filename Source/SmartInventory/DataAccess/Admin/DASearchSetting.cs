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
    public class DASearchSetting : Repository<SearchSetting>
    {
        DASearchSetting()
        {

        }

        private static DASearchSetting objDASearchSetting = null;
        private static readonly object lockObject = new object();
        public static DASearchSetting Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDASearchSetting == null)
                    {
                        objDASearchSetting = new DASearchSetting();
                    }
                }
                return objDASearchSetting;
            }
        }
        public List<AttributeDetail> GetLayerSearchAttributes(int layerId)
        {
            try
            {
                var res = repo.ExecuteProcedure<AttributeDetail>("fn_get_layer_search_attributes", new { p_layerId = layerId });
                return res;
            }
            catch { throw; }
        }

        public SearchSetting GetSearchSetting(int layerId)
        {
            var objSearchSetting = repo.Get(x => x.layer_id == layerId);
            return objSearchSetting ?? new SearchSetting();
        }

        public SearchSetting SaveSearchSetting(SearchSetting objSearch)
        {
            var objExisting = repo.Get(x => x.layer_id == objSearch.layer_id);

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
