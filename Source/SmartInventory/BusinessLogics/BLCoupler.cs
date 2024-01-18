using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLCoupler
    {
        
        public CouplerMaster SaveEntityCoupler(CouplerMaster objCouplerMaster, int userId)
        {
            return new DACoupler().SaveEntityCoupler(objCouplerMaster, userId);
        }
        public int DeleteCouplerById(int systemId)
        {
            return new DACoupler().DeleteCouplerById(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoCoupler(int systemId)
        {
            return new DACoupler().GetOtherInfoCoupler(systemId);
        }
        #endregion
    }
}
