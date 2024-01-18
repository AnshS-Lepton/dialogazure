using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLArea
    {
        //public Area GetAreabyId(int systemid)
        //{
        //    return new DAArea().GetAreabyId(systemid);
        //}
        public Area SaveArea(Area objArea, int userId)
        {
            return new DAArea().SaveArea(objArea, userId);
        }
        public int DeleteAreaById(int systemId)
        {
            return new DAArea().DeleteAreaById(systemId);
        }

    }
  
    public class BLSubArea 
    {
        public List<SubAreaIn> GetAreaExist(string geom)
        {
            return new DASubArea().GetAreaExist(geom);
        }
        public SubArea SaveSubArea(SubArea objSubArea, int userId)
        {
            return new DASubArea().SaveSubArea(objSubArea, userId);
        }
        public SubArea UpdateSubAreaBuildingCode(SubArea objSubArea, int userId)
        {
            return new DASubArea().UpdateSubAreaBuildingCode(objSubArea, userId);
        }
        public int DeleteSubAreaById(int systemId)
        {
            return new DASubArea().DeleteSubAreaById(systemId);
        }
       

    }
    public class BLSurveyArea
    {
        public SurveyArea SaveSurveyArea(SurveyArea objsurveyArea, int userId)
        {
            return new DASurveyArea().SaveSurveyArea(objsurveyArea, userId);
        }

        public void SaveMobileSurveyAreaAssigned(int user_id, int systemId)
        {
            new DASurveyArea().SaveMobileSurveyAreaAssigned(user_id, systemId);
        }
        public SurveyArea getSurveyAreaById(int systemId)
        {
            return new DASurveyArea().getSurveyAreaById(systemId);
        }
        public int DeleteSurveyAreaById(int systemId)
        {
            return new DASurveyArea().DeleteSurveyAreaById(systemId);
        }
        
    }
    public class BLrestricted_area
    {
        public RestrictedArea SaveRestrictedArea(RestrictedArea restricted_Area, int userId)
        {
            return new DArestricted_area().SaveRestrictedArea(restricted_Area, userId);
        }
        public int DeleteRestrictedById(int systemId)
        {
            return new DArestricted_area().DeleteRestrictedAreaById(systemId);
        }

    }


}
