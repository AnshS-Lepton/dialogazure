using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLWallMount
    {
       
        public WallMountMaster SaveEntityWallMount(WallMountMaster objWallMountMaster, int userId)
        {
            return new DAWallMount().SaveEntityWallMount(objWallMountMaster, userId);
        }
        public int DeleteWallMountById(int systemId)
        {
            return new DAWallMount().DeleteWallMountById(systemId);
        }
        //public List<WallMountArea> GetWallMountArea(string geom)
        //{
        //    return new DAWallMount().GetWallMountArea(geom);
        //}
        #region Additional-Attributes
        public string GetOtherInfoWallMount(int systemId)
        {
            return new DAWallMount().GetOtherInfoWallMount(systemId);
        }
        #endregion
    }
}
