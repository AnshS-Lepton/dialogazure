using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
using Models.ISP;
using DataAccess.ISP;

namespace BusinessLogics
{
    public class BLADB
    {
        public ADBMaster SaveEntityADB(ADBMaster objADBMaster, int userId)
        {
            return new DAADB().SaveEntityADB(objADBMaster, userId);
        }      
        public List<ADBSubArea> GetADBSubArea(string geom)
        {
            return new DAADB().GetADBSubArea(geom);
        }
        public int DeleteADBById(int systemId)
        {
            return new DAADB().DeleteADBById(systemId);
        }
        public ADBMaster getADBDetails(int systemId)
        {
            return new DAADB().getADBDetails(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoADB(int systemId)
        {
            return new DAADB().GetOtherInfoADB(systemId);
        }
        #endregion
    }
    public class BLCDB
    {
        public CDBMaster SaveEntityCDB(CDBMaster objCDBMaster, int userId)
        {
            return new DACDB().SaveEntityCDB(objCDBMaster, userId);
        }      
        public int DeleteCDBById(int systemId)
        {
            return new DACDB().DeleteCDBById(systemId);
        }
        public CDBMaster getCDBDetails(int systemId)
        {
            return new DACDB().getCDBDetails(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoCDB(int systemId)
        {
            return new DACDB().GetOtherInfoCDB(systemId);
        }
        #endregion
    }
    public class BLBDB
    {
        public BDBMaster SaveEntityBDB(BDBMaster objBDBMaster, int userId)
        {
            return new DABDB().SaveEntityBDB(objBDBMaster, userId);
        }
        public int DeleteBDBById(int systemId)
        {
            return new DABDB().DeleteBDBById(systemId);
        }

        public List<ShaftFloorList> GetShaftFloorByStrucId(int structureId)
        {
            return new DABDB().GetShaftFloorByStrucId(structureId);
        }
        public BDBMaster getBDBDetails(int systemId)
        {
            return new DABDB().getBDBDetails(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoBDB(int systemId)
        {
            return new DABDB().GetOtherInfoBDB(systemId);
        }
        #endregion
    }

}
