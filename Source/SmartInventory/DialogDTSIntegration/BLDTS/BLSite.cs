using BusinessLogics;
using DialogDTSIntegration.DADTS;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogDTSIntegration.BLDTS
{
    public class BLSite
    {
        public static Site Save(Site objsite, int userId)
        {
            return new DASite().Save(objsite, userId);
        }
        
    }
}
