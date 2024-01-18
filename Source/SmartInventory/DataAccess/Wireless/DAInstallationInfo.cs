using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
namespace DataAccess
{
    public class DAInstallationInfo : Repository<InstallationInfo>
    {
        public InstallationInfo Save(InstallationInfo installation)
        {
            if (installation.installation_id > 0)
                repo.Delete(installation);
            var objInstall = repo.Insert(installation);
            return objInstall;
        }
    }
}
