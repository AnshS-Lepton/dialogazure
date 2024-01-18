using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLVault
    {
        public VaultMaster SaveEntityVault(VaultMaster objVaultMaster, int userId)
        {
            return new DAVault().SaveEntityVault(objVaultMaster, userId);
        }
        public int DeleteVaultById(int systemId)
        {
            return new DAVault().DeleteVaultById(systemId);
        }
    }
}
