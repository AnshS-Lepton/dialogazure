using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLXMLBuilderDashboard
    {
        //start ycode
        public List<XMLBuilderDashboardMaster> GetXMLBuilderDashboard(XMLBuilderDashboardFilter objXMLBuilderDashboardFilter)
        {
            return new DAXMLBuilderDashboard().GetXMLBuilderDashboard(objXMLBuilderDashboardFilter);
        }

    }
}
