using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLSite
    {
        public Site Save(Site objsite, int userId)
        {
            return new DASite().Save(objsite, userId);
        }
        public int DeleteById(int systemId)
        {
            return new DASite().DeleteById(systemId);
        }
        public List<Site> GetAll(DateTime lastSuccessDate)
        {
            return new DASite().GelAll(lastSuccessDate);
        }
        public List<String> getCablesByLinkIds(string linkids)
        {
            return new DASiteSync().getCablesByLinkIds(linkids);
        }

    }
}
