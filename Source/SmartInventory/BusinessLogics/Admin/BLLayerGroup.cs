using System.Collections.Generic;
using Models.Admin;
using DataAccess.Admin;
using Models;

namespace BusinessLogics.Admin
{
    public class BLLayerGroup
    {
        public List<LayerGroupMaster> GetLayerGrpDetails()
        {
            return new DALayerGroup().GetLayerGrpDetails();
        }
        public List<LayerGroupMaster> GetGroupList(CommonGridAttributes objViewGroup)
        {
            return new DALayerGroup().GetGroupList(objViewGroup);
        }

        public LayerGroupMaster GetLayerGroupDetailsByID(int group_id)
        {
            return new DALayerGroup().GetLayerGroupDetailsByID(group_id);
        }
        public int DeleteLayerGrouprById(int group_id)
        {
            return new DALayerGroup().DeleteLayerGrouprById(group_id);

        }
        public string SaveLayerGroupDetails(LayerGroupMaster objLyrGroup)
        {

            return new DALayerGroup().SaveLayerGroupDetails(objLyrGroup);
        }
        public List<LayerGroupMapping> GetLyrGrpMappingList()
        {
            return new DALayerGroupMapping().GetLyrGrpMappingList();
        }
        public LayerGroupMapping GetGroupMappingById(int mappingId)
        {
            return new DALayerGroupMapping().GetGroupMappingById(mappingId);
        }
        public DbMessage SaveLayerGroupMappingDetails(string LstLayerGroupMapping)
        {

            return new DALayerGroupMapping().SaveLayerGroupMappingDetails(LstLayerGroupMapping);
        }
        public string SaveLayerGroupMappingDetailById(LayerGroupMapping objLyrGroupMpng)
        {

            return new DALayerGroupMapping().SaveLayerGroupMappingDetailsById(objLyrGroupMpng);
        }
        public int ValidateLayerGroupById(int group_id)
        {
            return new DALayerGroupMapping().ValidateLayerGroupById(group_id);

        }
    }

    public class BLLayerStyleMaster
    {        
        public List<LayerStyleMaster> GetLayerStyleMasterDetailsByID(int id)
        {
            return new DALayerStyleMaster().GetLayerStyleDetailsByID(id);
        }
        public List<LayerStyleMaster> GetLayerStyleMaster(CommonGridAttributes objLayerStyleMaster)
        {
            return new DALayerStyleMaster().GetLayerStyleMaster(objLayerStyleMaster);
        }
        public bool SaveLayerStyleMasterDetails(LayerStyleMaster objLyrStyMaster)
        {
            return new DALayerStyleMaster().SaveLayerStyleMasterDetails(objLyrStyMaster);
        }        
        public List<RowCountResult> CheckUpdateLayerSequence(int layer_sequence, int layer_id, bool is_valid )
        {
            return new DALayerStyleMaster().CheckUpdateLayerSequence(layer_sequence,layer_id , is_valid);
        }
    }
}
