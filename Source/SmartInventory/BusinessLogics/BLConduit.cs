using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public sealed class BLConduit
    {
        BLConduit()
        {

        }
        private static BLConduit objConduit = null;
        private static readonly object lockConduit = new object();
        public static BLConduit Instance
        {
            get
            {
                lock (lockConduit)
                {
                    if (objConduit == null)
                    {
                        objConduit = new BLConduit();
                    }
                    return objConduit;
                }
            }
        }

      

        public int DeleteConduitById(int systemId)
        {
            return DAConduit.Instance.DeleteConduitById(systemId);
        }
        public ConduitMaster SaveConduit(ConduitMaster objCbl, int userId)
        {
            return DAConduit.Instance.SaveConduit(objCbl, userId);
        }

        public EditLineTP EditConduitTPDetail(EditLineTP objTPDetail, int userId)
        {
            return DAConduit.Instance.EditConduitTPDetail(objTPDetail, userId);
        }
        #region Additional-Attributes
        public string GetOtherInfoConduit(int systemId)
        {
            return DAConduit.Instance.GetOtherInfoConduit(systemId);
        }
        #endregion
    }
}
