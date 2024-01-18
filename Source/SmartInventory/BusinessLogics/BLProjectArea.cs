using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess;

namespace BusinessLogics
{
   public class BLProjectArea
    {
        //public List<SubAreaIn> GetAreaExist(string geom)
        //{
        //    return new DASubArea().GetAreaExist(geom);
        //}
        public ProjectArea SaveProjectArea(ProjectArea objPArea, int userId)
        {
            return new DAProjectArea().SaveProjectArea(objPArea, userId);
        }
        public int DeleteProjectAreaId(int systemId)
        {
            return new DAProjectArea().DeleteProjectAreaById(systemId); 
        }
    }
}
