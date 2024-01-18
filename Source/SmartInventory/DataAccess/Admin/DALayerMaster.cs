using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Admin
{
    public class DALayerMaster : Repository<layerDetail>
    {

       
        public List<ViewLayerDetailList> GetLayerSettingDetailsList(ViewLayerSettingDetailList model)
        {
            try
            {
                var res = repo.ExecuteProcedure<ViewLayerDetailList>("fn_get_layer_setting_details", new
                {
                    searchby = Convert.ToString(model.viewLayerDetails.searchBy),
                    searchbyText = Convert.ToString(model.viewLayerDetails.searchText),
                    P_PAGENO = model.viewLayerDetails.currentPage,
                    P_PAGERECORD = model.viewLayerDetails.pageSize,
                    P_SORTCOLNAME = model.viewLayerDetails.sort,
                    P_SORTTYPE = model.viewLayerDetails.orderBy,
                    P_TOTALRECORDS = model.viewLayerDetails.totalRecord
                   
                }, true);

                return res;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetLayerSettingDetails(ViewLayerSettingDetailList model)
        {
            try
            {
                var res = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_layer_setting_details", new
                {
                    searchby = Convert.ToString(model.viewLayerDetails.searchBy),
                    searchbyText = Convert.ToString(model.viewLayerDetails.searchText),
                    P_PAGENO = model.viewLayerDetails.currentPage,
                    P_PAGERECORD = model.viewLayerDetails.pageSize,
                    P_SORTCOLNAME = model.viewLayerDetails.sort,
                    P_SORTTYPE = model.viewLayerDetails.orderBy,
                    P_TOTALRECORDS = model.viewLayerDetails.totalRecord,
                    P_RECORDLIST = 0,
                    is_active = model.viewLayerDetails.is_active
                }, true);

                return res;
            }
            catch { throw; }
        }
        public layerDetail getFullText(int id)
        {
            try
            {
                var queryDetails = repo.GetById(m => m.layer_id == id);
                if (queryDetails != null) { return queryDetails; }
                return null;
            }
            catch { throw; }
        }
        public List<LayerDetailsColumn> GetLayerDetailById(int layer_id)
        {

            try
            {
                var lst = repo.ExecuteProcedure<LayerDetailsColumn>("fn_layer_details_column_data",
                    new
                    {
                        p_layerId = layer_id

                    }, true);
                return lst;
            }
            catch { throw; }
        }


        public DbMessage SavelayerSettingDetails(string objlayerDetail, int layer_id)
        {
            try
            {

                //return repo.Insert(objConnectionInfo);
               var response = repo.ExecuteProcedure<DbMessage>("fn_save_layer_settings_details", new { p_layerlist = objlayerDetail, p_layer_id = layer_id }).FirstOrDefault();
                return response;
            }
            catch { throw; }

        }


    }

}