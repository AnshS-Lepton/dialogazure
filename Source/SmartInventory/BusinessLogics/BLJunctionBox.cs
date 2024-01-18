using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    
    public sealed class BLJunctionBox
    {
        BLJunctionBox()
        {

        }
        private static BLJunctionBox objJunctionBox = null;
        private static readonly object lockConduit = new object();
        public static BLJunctionBox Instance
        {
            get
            {
                lock (lockConduit)
                {
                    if (objJunctionBox == null)
                    {
                        objJunctionBox = new BLJunctionBox();
                    }
                    return objJunctionBox;
                }
            }
        }

        public List<JunctionBoxMaster> GetJunctionBoxByld(int parentSystemId, string parentEntityType)
        {
            return DAJunctionBox.Instance.GetJunctionBoxld(parentSystemId, parentEntityType);
        }
        public int DeleteJunctionBoxById(int systemId)
        {
            return DAJunctionBox.Instance.DeleteJunctionBoxById(systemId);
        }
    }
}
