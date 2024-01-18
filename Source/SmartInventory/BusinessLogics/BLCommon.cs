using BusinessLogics.Admin;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLCommon
    {
        public void fillParentDetail(dynamic objLib, NetworkCodeIn objIn, string networkIdType)
        {
            //fill parent detail....
            var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
            if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
            {
                if (networkIdType == NetworkIdType.M.ToString())
                {
                    //FILL NETWORK CODE FORMAT FOR MANUAL
                    objLib.network_id = networkCodeDetail.network_code;
                }
                objLib.parent_entity_type = networkCodeDetail.parent_entity_type;
                objLib.parent_network_id = networkCodeDetail.parent_network_id;
                objLib.parent_system_id = networkCodeDetail.parent_system_id;
            }
        }
        public void SaveReference(EntityReference entityReference, int system_id)
        {
            BLReference.Instance.SaveReference(entityReference, system_id);
        }
        public void fillProjectSpecifications(dynamic objLib)
        {
            BLProject objblProject = new BLProject();
            //"P" we need to pass this value as dynamically as network stage selection
            objLib.lstBindProjectCode = objblProject.getProjectCodeDetails("P");
            objLib.lstBindPlanningCode = objblProject.getPlanningDetailByIdList(Convert.ToInt32(objLib.project_id ?? 0));
            objLib.lstBindWorkorderCode = objblProject.getWorkorderDetailByIdList(Convert.ToInt32(objLib.planning_id ?? 0));
            objLib.lstBindPurposeCode = objblProject.getPurposeDetailByIdList(Convert.ToInt32(objLib.workorder_id ?? 0));
        }

        public string GetPointTypeParentGeom(int pSystemId, string pEntityType)
        {
            string geom = "";
            //get parent detail..
            var dicParentEntityDetail = new BLMisc().GetEntityDetailById<Dictionary<string, string>>(pSystemId, (EntityType)Enum.Parse(typeof(EntityType), pEntityType));
            if (dicParentEntityDetail != null)
            {
                //set geometry value as parent..
                geom = dicParentEntityDetail["longitude"] + " " + dicParentEntityDetail["latitude"];
            }
            return geom;
        }

        public void fillRegionProvinceDetail(dynamic objEntityModel, string enType, string geom)
        {
            List<InRegionProvince> objRegionProvince = new List<InRegionProvince>();
            objRegionProvince = BLBuilding.Instance.GetRegionProvince(geom, enType);
            if (objRegionProvince != null && objRegionProvince.Count > 0)
            {
                objEntityModel.region_id = objRegionProvince[0].region_id;
                objEntityModel.province_id = objRegionProvince[0].province_id;
                objEntityModel.region_name = objRegionProvince[0].region_name;
                objEntityModel.province_name = objRegionProvince[0].province_name;
            }
        }

        

    }
}
