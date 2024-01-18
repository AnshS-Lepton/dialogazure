using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
   public class BLCompetitor
    {
        public Competitor SaveCompetitor(Competitor objCompetitor, int userId)
        {
            return new DACompetitor().SaveCompetitor(objCompetitor, userId);
        }
        #region Additional-Attributes
        public string GetOtherInfoCompetitor(int systemId)
        {
            return new DACompetitor().GetOtherInfoCompetitor(systemId);
        }
        #endregion
    }
}
