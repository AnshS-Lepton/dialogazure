using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;

namespace BusinessLogics
{
    public class BLInstallationInfo
    {
        public InstallationInfo Save(InstallationInfo installation)
        {
            return new DAInstallationInfo().Save(installation);
        }
    }
}
