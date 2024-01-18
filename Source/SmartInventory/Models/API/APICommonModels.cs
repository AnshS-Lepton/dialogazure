using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.API
{
    public class GetSurveyAreaInfoIn
    {
        public int userid { get; set; }
    }
    public class SurveyStatus
    {
        public string status { get; set; }
        public int userid { get; set; }
    }
    public class BuildingId
    {
        public int id { get; set; }
        public int userid { get; set; }
    }
}
