using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Admin;
using Models.Admin;

namespace BusinessLogics.Admin
{
    public class BLOrthoImageLayer
    {
        public List<OrthoImageMasterModel> GetOrthoImageLayerList(OrthoImageModel objViewOrthoImage, int userId)
        {
            return new DAOrthoImageLayer().GetOrthoImageLayerList(objViewOrthoImage, userId);
        }
        public OrthoImageMasterModel GetOrthoImageById(int system_id)
        {
            return new DAOrthoImageLayer().GetOrthoImageById(system_id);
        }
        public OrthoImageMasterModel SaveOrthoImage(OrthoImageMasterModel input, int userId)
        {
            return new DAOrthoImageLayer().SaveOrthoImage(input, userId);
        }
        public int DeleteOrthoImageById(int id, int userId)
        {
            return new DAOrthoImageLayer().DeleteOrthoImageById(id, userId);
        }
    }
}
