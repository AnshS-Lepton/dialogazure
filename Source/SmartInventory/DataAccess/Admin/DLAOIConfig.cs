using DataAccess.DBHelpers;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Admin
{
    public class DLAOIConfig : Repository<ViewAOIConfiguration>
    {
        public List<ViewAOIConfiguration> GetAOIDetails()
        {
            return repo.GetAll().ToList();
        }
    }
}
