using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLWorkSpace
    {
        DAWorkSpace objDAWorkSpace = new DAWorkSpace();
        public IEnumerable<WorkSpaceMaster> GetworkSpaceDetails(int userId)
        {
            return objDAWorkSpace.GetworkSpaceDetails(userId).OrderByDescending(x => x.modified_on ?? x.created_on);
        }

        public bool SaveWorkSpace(WorkSpaceMaster workspace)
        {
            return objDAWorkSpace.SaveWorkSpace(workspace);
        }
        public int DeleteWorkSpace(int workSpaceId)
        {
            return objDAWorkSpace.DeleteWorkSpace(workSpaceId);
        }
        public WorkSpaceMaster GetworkSpaceById(int workSpaceId)
        {
            return objDAWorkSpace.GetworkSpaceById(workSpaceId);
        }
    }

    public class BLWorkSpaceRegionProvince
    {
        DAWorkSpaceRegionProvince objDAWorkSpaceRegionProvince = new DAWorkSpaceRegionProvince();
        public ICollection<WorkSpaceRegionProvince> GetWorkSpaceRegionProvince(int usrId, int workSpaceId)
        {
            return objDAWorkSpaceRegionProvince.GetWorkSpaceRegionProvince(usrId, workSpaceId);
        }  
    }

    public class BLWorkSpaceLayer
    {
        DAWorkSpaceLayer objDAWorkSpaceLyr = new DAWorkSpaceLayer();
        public ICollection<WorkSpaceLayers> GetWorkSpaceLayers(int workSpaceId)
        {
            return objDAWorkSpaceLyr.GetWorkSpaceLayers(workSpaceId);
        }     
    }

    public class BLLandbaseWorkSpaceLayer
    {
        DALandbaseWorkSpaceLayer objDAWorkSpaceLyr = new DALandbaseWorkSpaceLayer();
        public ICollection<LandbaseWorkSpaceLayers> GetLandbaseWorkSpaceLayers(int workSpaceId)
        {
            return objDAWorkSpaceLyr.GetLandbaseWorkSpaceLayers(workSpaceId);
        }
    }
}
