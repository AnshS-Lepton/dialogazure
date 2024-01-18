using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
namespace BusinessLogics
{
    public class BLTower
    {
        public TowerMaster Save(TowerMaster towerMaster, int UserId)
        {
            return new DATower().Save(towerMaster, UserId);
        }
        public List<AssociatedPop> GetPopInBuffer(int towerId, int bufferId)
        {
            return new DATower().GetPopInBuffer(towerId, bufferId);

        }
    }

    public class BLTowerAssociatedPop
    {
        private readonly DATowerAssociatedPop objtowerAssociatedPop = null;
        public BLTowerAssociatedPop()
        {
            objtowerAssociatedPop = new DATowerAssociatedPop();
        }
        public TowerAssociatedPop SaveAssociatedPop(int popId, int towerId, int UserId)
        {
            return objtowerAssociatedPop.SaveAssociatedPop(popId, towerId, UserId);
        }
        public ErrorMessage CheckDuplicate(int popId, int towerId)
        {
            return objtowerAssociatedPop.CheckDuplicate(popId, towerId);
        }
        public List<TowerAssociatedPopView> GetAssociatedPop(int towerId)
        {
            return objtowerAssociatedPop.GetAssociatedPop(towerId);
        }
         public ErrorMessage DeAssociatePop(int popId, int towerId)
        {
            return objtowerAssociatedPop.DeAssociatePop(popId, towerId);
        }
    }
}
