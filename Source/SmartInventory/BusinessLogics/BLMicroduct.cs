
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLMicroduct
    {
        private readonly DAMicroduct objDAMicroduct;
        public BLMicroduct()
        {
            objDAMicroduct = new DAMicroduct();
        }
        public MicroductMaster Save(MicroductMaster objMicrowaveLinkMaster, int userId)
        {
            return objDAMicroduct.Save(objMicrowaveLinkMaster, userId);
        }
        public int DeleteMicrowaveLinkById(int systemId)
        {
            return objDAMicroduct.Delete(systemId);
        }
        public MicroductMaster getMicrowaveLinkDetails(int systemId)
        {
            return objDAMicroduct.Get(systemId);
        }
        public DbMessage Validate(int systemId)
        {
            return objDAMicroduct.Validate(systemId);
        }

        #region Additional-Attributes
        public string GetOtherInfoMicroduct(int systemId)
        {
            return objDAMicroduct.GetOtherInfoMicroduct(systemId);
        }
        #endregion
    }
}
