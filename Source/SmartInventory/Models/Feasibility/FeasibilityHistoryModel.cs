using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Feasibility
{
    public class FeasibilityHistoryModel
    {
        public int RecId { get; set; }
        public string FeasibilityId { get; set; }
        public string SearchLoc { get; set; }
        public string LocAddress { get; set; }
        public string LocPoints { get; set; }
        public string FeasibilityByUser { get; set; }
        public DateTime FeasibilityOnDate { get; set; }
        public string FeasibilityFromBrowser { get; set; }
        public string FeasibilityFromMachine { get; set; }
        public string FeasibilityFromIpAdd { get; set; }
    }
}
