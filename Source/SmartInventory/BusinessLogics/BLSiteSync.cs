using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLSiteSync
    {
        public SiteSync Save(SiteSync objsite)
        {
            return new DASiteSync().Save(objsite);
        }
        public int DeleteById(int Id)
        {
            return new DASiteSync().DeleteById(Id);
        }
        public List<SiteSync> GetAll()
        {
            return new DASiteSync().GelAll();
        }
    }
}
