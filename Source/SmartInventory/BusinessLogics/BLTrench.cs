using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLTrench
    {
        private static BLTrench objTrench = null;
        private static readonly object lockObject = new object();
        public static BLTrench Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objTrench == null)
                    {
                        objTrench = new BLTrench();
                    }
                }
                return objTrench;
            }
        }

        public TrenchMaster SaveTrench(TrenchMaster objCbl, int userId)
        {
            return DATrench.Instance.SaveTrench(objCbl, userId);
        }

        public EditLineTP EditTrenchTPDetail(EditLineTP objTPDetail, int userId)
        {
            return DATrench.Instance.EditTrenchTPDetail(objTPDetail, userId);
        }

        public int DeleteTrenchById(int trench_Id)
        {
            return DATrench.Instance.DeleteTrenchById(trench_Id);
        }
        #region Additional-Attributes
        public string GetOtherInfoTrench(int systemId)
        {
            return DATrench.Instance.GetOtherInfoTrench(systemId);
        }
        #endregion
    }
}
