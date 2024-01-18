using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLGipipe
    {
        private static BLGipipe objGipipe = null;
        private static readonly object lockObject = new object();
        public static BLGipipe Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objGipipe == null)
                    {
                        objGipipe = new BLGipipe();
                    }
                }
                return objGipipe;
            }
        }

        public GipipeMaster SaveGipipe(GipipeMaster objCbl, int userId)
        {
            return DAGipipe.Instance.SaveGipipe(objCbl, userId);
        }

        public EditLineTP EditGipipeTPDetail(EditLineTP objTPDetail, int userId)
        {
            return DAGipipe.Instance.EditGipipeTPDetail(objTPDetail, userId);
        }

        public int DeleteGipipeById(int gipipe_Id)
        {
            return DAGipipe.Instance.DeleteGipipeById(gipipe_Id);
        }
        #region Additional-Attributes
        public string GetOtherInfoGipipe(int systemId)
        {
            return DAGipipe.Instance.GetOtherInfoGipipe(systemId);
        }
        #endregion

    }

}
