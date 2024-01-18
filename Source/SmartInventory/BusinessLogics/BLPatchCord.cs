using Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLPatchCord
    {
        private static BLPatchCord objPatch = null;
        private static readonly object lockObject = new object();
        public static BLPatchCord Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objPatch == null)
                    {
                        objPatch = new BLPatchCord();
                    }
                }
                return objPatch;
            }
        }
        public PatchCordMaster SavePatchCord(PatchCordMaster objPatch, int userId)
        {
            return DAPatchCord.Instance.SavePatchCord(objPatch, userId);
        }
    }
}
