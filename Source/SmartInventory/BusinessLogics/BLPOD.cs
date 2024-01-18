using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLPOD
    {
        public PODMaster SaveEntityPOD(PODMaster objPODMaster, int userId)
        {
            return new DAPOD().SaveEntityPOD(objPODMaster, userId);
        }
        public int DeletePODById(int systemId)
        {
            return new DAPOD().DeletePODById(systemId);
        }
        public List<PODDetail> GetPODAssociationDetail(string geom, int associated_SystemId, string associated_entity_Type)
        {
            return new DAPOD().GetPODAssociationDetail(geom, associated_SystemId, associated_entity_Type);
        }
        public List<KeyValueDropDown> GetPODDetailForFilter()
        {
            return new DAPOD().GetPODDetailForFilter();

        }
        public List<PODDetail> GetPodDetailsInBulk(string geom)
        {
            return new DAPOD().GetPodDetailsInBulk(geom);
        }
        #region Additional-Attributes
        public string GetOtherInfoPOD(int systemId)
        {
            return new DAPOD().GetOtherInfoPOD(systemId);
        }
        #endregion
    }
}
