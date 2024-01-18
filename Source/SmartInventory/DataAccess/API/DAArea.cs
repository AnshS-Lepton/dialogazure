using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models.API;

namespace DataAccess.API
{   
    public class DASurveyArea : Repository<SurveyArea>
    {    
        public List<SurveyArea> getAllSurveyAreaById(int sUserId, string status)
        {
            try
            {
                return repo.ExecuteProcedure<SurveyArea>("fn_get_surveyarea_info", new { p_user_id = sUserId, p_status = status.ToLower() }, true);
            }
            catch { throw; }

        }

        public List<SurveyArea> GetSurveyInfobyBuildingid(int buildingId, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<SurveyArea>("fn_get_surveyarea_info_by_building_id", new { p_user_id = userId, p_building_id = buildingId }, true);
            }
            catch { throw; }
        }     

    }

}
