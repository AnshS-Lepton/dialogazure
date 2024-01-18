using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.API
{
    public class SurveyArea
    {
        public string surveyAreaId { get; set; }      
        public string surveyarea_name { get; set; }
        public string surveyarea_code { get; set; }
        public int user_id { get; set; }      
        public Nullable<DateTime> created_date { get; set; }      
        public Nullable<DateTime> assigned_date { get; set; }      
        public Nullable<DateTime> due_date { get; set; }
        public string status { get; set; }
        public string assignedby { get; set; }
        public string sp_geometry { get; set; }
    }

}
