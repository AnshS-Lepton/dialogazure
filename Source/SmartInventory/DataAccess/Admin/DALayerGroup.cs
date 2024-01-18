using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;
using DataAccess.DBContext;
using DataAccess.Contracts;
using Models.Admin;

namespace DataAccess.Admin
{
    public class DALayerGroup : Repository<LayerGroupMaster>
    {
        public List<LayerGroupMaster> GetLayerGrpDetails()
        {
            try
            {
                return repo.GetAll().OrderByDescending(m => m.group_name).ToList();
            }
            catch { throw; }
        }
        public List<LayerGroupMaster> GetGroupList(CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<LayerGroupMaster>("fn_get_layer_group_details", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    P_TOTALRECORDS = objGridAttributes.totalRecord,
                }, true);
            }
            catch { throw; }
        }

        public LayerGroupMaster GetLayerGroupDetailsByID(int group_id)
        {
            return repo.Get(m => m.group_id == group_id);
        }

        public int DeleteLayerGrouprById(int group_id)
        {
            try
            {
                var objGroupId = repo.Get(x => x.group_id == group_id);
                if (objGroupId != null)
                {
                    return repo.Delete(objGroupId.group_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }

        }

        public string SaveLayerGroupDetails(LayerGroupMaster objLyrGroup)
        {
            try
            {
                var objExisiting = repo.GetById(m => m.group_id == objLyrGroup.group_id);
                var result = "Failed";
                if (objExisiting != null)
                {
                    objLyrGroup.modified_by = objLyrGroup.user_id;
                    objLyrGroup.modified_on = DateTimeHelper.Now;
                    repo.Update(objLyrGroup);
                    result = "Update";
                }
                else
                {
                    objLyrGroup.created_by = objLyrGroup.user_id;
                    objLyrGroup.created_on = DateTimeHelper.Now;
                    repo.Insert(objLyrGroup);
                    result = "Save";
                }
                return result;
            }
            catch { throw; }
        }
    }


    public class DALayerGroupMapping : Repository<LayerGroupMapping>
    {
        public List<LayerGroupMapping> GetLyrGrpMappingList()
        {
            try
            {
                return repo.ExecuteProcedure<LayerGroupMapping>("fn_get_layer_group_mapping_details", new
                {

                    P_TOTALRECORDS = 0,
                }, true);
            }
            catch (Exception ex)
            { throw ex; }
        }

        public LayerGroupMapping GetGroupMappingById(int mappingId)
        {
            try
            {
                return repo.Get(u => u.mapping_id == mappingId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public string SaveLayerGroupMappingDetails(LayerGroupMapping objLyrGroupMpng)
        //{
        //    try
        //    {
        //        var validateGrpMapng = repo.GetAll(m => m.layer_id == objLyrGroupMpng.layer_id && m.group_id == objLyrGroupMpng.group_id).FirstOrDefault();
        //        var objExisiting = repo.GetById(m => m.mapping_id == objLyrGroupMpng.mapping_id);
        //        var result = "Failed";
        //        if (validateGrpMapng == null)
        //        {
        //            if (objExisiting != null)
        //            {
        //                objLyrGroupMpng.modified_by = objLyrGroupMpng.user_id;
        //                objLyrGroupMpng.modified_on = DateTimeHelper.Now;
        //                repo.Update(objLyrGroupMpng);
        //                result = "Update";
        //            }
        //            else
        //            {
        //                objLyrGroupMpng.created_by = objLyrGroupMpng.user_id;
        //                objLyrGroupMpng.created_on = DateTimeHelper.Now;
        //                repo.Insert(objLyrGroupMpng);
        //                result = "Save";
        //            }
        //        }
        //        return result;
        //    }
        //    catch { throw; }
        //}

        public DbMessage SaveLayerGroupMappingDetails(string LstLayerGroupMapping)
        {
            try
            {

                //return repo.Insert(objConnectionInfo);
                var response = repo.ExecuteProcedure<DbMessage>("fn_save_layer_group", new { p_layer_settings_column = LstLayerGroupMapping }).FirstOrDefault();
                return response;
            }
            catch
            {
                throw;
            }
        }
        public string SaveLayerGroupMappingDetailsById(LayerGroupMapping objLyrGroupMpng)
        {
            try
            {
                // var validateGrpMapng = repo.GetAll(m => m.layer_id == objLyrGroupMpng.layer_id && m.group_id == objLyrGroupMpng.group_id).FirstOrDefault();
                var validateGrpMapng = repo.GetAll(m => m.layer_id == objLyrGroupMpng.layer_id).FirstOrDefault();
                // var objExisiting = repo.GetById(m => m.mapping_id == objLyrGroupMpng.mapping_id);
                var result = "Failed";
                if (validateGrpMapng != null)
                {
                    //if (objExisiting != null)
                    //{
                    objLyrGroupMpng.modified_by = objLyrGroupMpng.user_id;
                    objLyrGroupMpng.modified_on = DateTimeHelper.Now;
                    repo.Update(objLyrGroupMpng);
                    result = "Update";
                    //}
                    //else
                    //{
                    //    objLyrGroupMpng.created_by = objLyrGroupMpng.user_id;
                    //    objLyrGroupMpng.created_on = DateTimeHelper.Now;
                    //    repo.Insert(objLyrGroupMpng);
                    //    result = "Save";
                    //}
                }
                return result;
            }
            catch { throw; }
        }


        public int ValidateLayerGroupById(int group_id)
        {
            try
            {
                var objGroupId = repo.Get(x => x.group_id == group_id);
                if (objGroupId != null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch { throw; }

        }
    }

    public class DALayerStyleMaster : Repository<LayerStyleMaster>
    {        
        public List<LayerStyleMaster> GetLayerStyleMaster(CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<LayerStyleMaster>("fn_get_layer_style_master_details", new
                {
                    searchby = objGridAttributes.searchBy = objGridAttributes.searchBy == "layer_name" ? "layer_title" : objGridAttributes.searchBy,
                    searchbyText = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    P_TOTALRECORDS = objGridAttributes.totalRecord,
                    P_RECORDLIST = 0
                }, true);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                throw;
            }
        }
        public List<LayerStyleMaster> GetLayerStyleDetailsByID(int id)
        {
           try
           {
               return repo.ExecuteProcedure<LayerStyleMaster>("fn_get_layer_style_master_details_by_Id", new
               {
                   p_id = id,
               }, true);
           }
           catch { throw; }
        }
        public List<RowCountResult> CheckUpdateLayerSequence(int layer_sequence, int layer_id, bool is_valid)
        {
            try
            {
                var rowcount = repo.ExecuteProcedure<RowCountResult>("fn_check_layer_Sequence_exist", new
                {
                    p_layer_sequence = layer_sequence,
                    layer_id = layer_id,
                    is_valid = is_valid
                }, true);
                return rowcount;
            }

            catch (Exception ex) { throw ex; }
        }
        public bool SaveLayerStyleMasterDetails(LayerStyleMaster objLyrStyMaster)
        {
            try
            {
                bool result = false;
                var objExisiting = repo.GetById(m => m.id == objLyrStyMaster.id);
                if (objExisiting != null)
                {
                    objLyrStyMaster.icon_file_name = objExisiting.icon_file_name;
                    objLyrStyMaster.icon_base_path = objExisiting.icon_base_path;
                    objLyrStyMaster.entity_category = objExisiting.entity_category;
                    objLyrStyMaster.modified_by = objLyrStyMaster.user_id;
                    objLyrStyMaster.modified_on = DateTime.Now;
                    repo.Update(objLyrStyMaster);
                    result = true;
                }
                if (objExisiting.opacity != objLyrStyMaster.opacity)
                {
                    var layerStyles = repo.GetAll(m => m.layer_id == objLyrStyMaster.layer_id).ToList();
                    foreach (var layerStyle in layerStyles)
                    {
                        layerStyle.modified_by = objLyrStyMaster.user_id;
                        layerStyle.modified_on = DateTimeHelper.Now;
                        layerStyle.opacity = objLyrStyMaster.opacity;
                        repo.Update(layerStyle);
                    }
                    repo.Update(objLyrStyMaster);
                    result = true;
                }
                return result;
            }
            catch { throw; }
        }               
    }
}


