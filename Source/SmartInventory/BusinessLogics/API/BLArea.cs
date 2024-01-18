using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.API;
using Models.API;


namespace BusinessLogics.API
{  
    public class BLAPISurveyArea
    {
        public List<SurveyArea> getSurveyAreaById(int systemId, string status ="assigned")
        {
            return new DASurveyArea().getAllSurveyAreaById(systemId, status);
        }

        public List<SurveyArea> GetSurveyInfobyBuildingid(int buildingId, int Userid)
        {
            return new DASurveyArea().GetSurveyInfobyBuildingid(buildingId, Userid);
        }              
    }    
}
