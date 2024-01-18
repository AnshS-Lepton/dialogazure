using System;
using DataAccess;
using Models;

namespace BusinessLogics
{
   public class BLHandhole
    {

        public HandholeMaster SaveEntityHandhole(HandholeMaster objHandholeMaster, int userId)
        {
            return new DAHandhole().SaveEntityHandhole(objHandholeMaster, userId);
        }
        public int DeleteHandholeById(int systemId)
        {
            return new DAHandhole().DeleteHandholeById(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoHandhole(int systemId)
        {
            return new DAHandhole().GetOtherInfoHandhole(systemId);
        }
        #endregion

        
    }
}
