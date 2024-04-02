using DataAccess;
using Models;
using System.Collections.Generic;


namespace BusinessLogics
{
    public class BLSlack
    {
        private static BLSlack objSlack = null;
        private static readonly object lockObject = new object();
        public static BLSlack Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSlack == null)
                    {
                        objSlack = new BLSlack();
                    }
                }
                return objSlack;
            }
        }

        public List<SlackMaster> GetSlackDetails(double longitude, double latitude, int associated_SystemId, string associated_System_Type, int structure_id)
        {
            return DASlack.Instance.GetSlackDetails(longitude, latitude, associated_SystemId, associated_System_Type, structure_id);
        }
        public int DeleteSlackDetailById(int system_id)
        {
            return DASlack.Instance.DeleteSlackDetailById(system_id);

        }
        public List<SlackMaster> GetSlackDetailsForDuct(int duct_system_id)
        {
            return DASlack.Instance.GetSlackDetailsForDuct(duct_system_id);
        }
        public PageMessage SaveEntitySlack(string Loops)
        {
            return new DASlack().SaveEntitySlack(Loops);
        }
        public List<NEDuctDetails> GetNearByDuctDetails(double longitude, double latitude, int bufferInMtrs)
        {
            return DASlack.Instance.GetNearByDuctDetails(longitude, latitude, bufferInMtrs);
        }
    }
}
