using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLDuct
    {
        private static BLDuct objDuct = null;
        private static readonly object lockObject = new object();
        public static BLDuct Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDuct == null)
                    {
                        objDuct = new BLDuct();
                    }
                }
                return objDuct;
            }
        }

        public DuctMaster SaveDuct(DuctMaster objCbl, int userId)
        {
            return DADuct.Instance.SaveDuct(objCbl, userId);
        }

        public EditLineTP EditDuctTPDetail(EditLineTP objTPDetail, int userId)
        {
            return DADuct.Instance.EditDuctTPDetail(objTPDetail, userId);
        }

        public int DeleteDuctById(int duct_Id)
        {
            return DADuct.Instance.DeleteDuctById(duct_Id);
        } 
        public int getDuctCount(int duct_Id)
        {
            return DADuct.Instance.getDuctCount(duct_Id);
        }
        #region Additional-Attributes
        public string GetOtherInfoDuct(int systemId)
        {
            return DADuct.Instance.GetOtherInfoDuct(systemId);
        }
        #endregion

    }

}
