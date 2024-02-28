
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLMicroduct
    {


        private static BLMicroduct objMicroduct = null;
        private static readonly object lockObject = new object();
        public static BLMicroduct Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objMicroduct == null)
                    {
                        objMicroduct = new BLMicroduct();
                    }
                }
                return objMicroduct;
            }
        }
        public MicroductMaster Save(MicroductMaster objMicrowaveLinkMaster, int userId)
        {
            return DAMicroduct.Instance.Save(objMicrowaveLinkMaster, userId);
        }
        public int DeleteMicrowaveLinkById(int systemId)
        {
            return DAMicroduct.Instance.Delete(systemId);
        }
        public MicroductMaster getMicrowaveLinkDetails(int systemId)
        {
            return DAMicroduct.Instance.Get(systemId);
        }
        public DbMessage Validate(int systemId)
        {
            return DAMicroduct.Instance.Validate(systemId);
        }

        #region Additional-Attributes
        public string GetOtherInfoMicroduct(int systemId)
        {
            return DAMicroduct.Instance.GetOtherInfoMicroduct(systemId);
        }
        public int getMicroductCount(int microduct_Id)
        {
            return DAMicroduct.Instance.getMicroDuctCount(microduct_Id);
        }
        #endregion
    }
}
