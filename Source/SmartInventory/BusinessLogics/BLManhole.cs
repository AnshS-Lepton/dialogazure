using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLManhole
    {
        
        public ManholeMaster SaveEntityManhole(ManholeMaster objManholeMaster, int userId)
        {
            return new DAManhole().SaveEntityManhole(objManholeMaster, userId);
        }
        public int DeleteManholeById(int systemId)
        {
            return new DAManhole().DeleteManholeById(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoManhole(int systemId)
        {
            return new DAManhole().GetOtherInfoManhole(systemId);
        }
        #endregion

    }
}
