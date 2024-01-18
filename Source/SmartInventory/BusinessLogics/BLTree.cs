using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLTree
    {
       
        public TreeMaster SaveEntityTree(TreeMaster objTreeMaster, int userId)
        {
            return new DATree().SaveEntityTree(objTreeMaster, userId);
        }
        public int DeleteTreeById(int systemId)
        {
            return new DATree().DeleteTreeById(systemId);
        }
        //public List<TreeArea> GetTreeArea(string geom)
        //{
        //    return new DATree().GetTreeArea(geom);
        //}
        #region Additional-Attributes
        public string GetOtherInfoTree(int systemId)
        {
            return new DATree().GetOtherInfoTree(systemId);
        }
        #endregion
    }
}
