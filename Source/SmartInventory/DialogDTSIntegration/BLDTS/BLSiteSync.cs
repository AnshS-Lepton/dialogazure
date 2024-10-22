using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogDTSIntegration.BLDTS
{
    public class BLSiteSync
    {
        public SiteSync Save(SiteSync objsite)
        {
            return new DADTS.DASiteSync().Save(objsite);
        }
        public List<SiteSync> GetAll()
        {
            return new DADTS.DASiteSync().GelAll();
        }
    }
}
