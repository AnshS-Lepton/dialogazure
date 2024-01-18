using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace BusinessLogics
{
   public class BLLoopMangment
    {
        private static BLLoopMangment objLoopMangment = null;
        private static readonly object lockObject = new object();
        public static BLLoopMangment Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objLoopMangment == null)
                    {
                        objLoopMangment = new BLLoopMangment();
                    }
                }
                return objLoopMangment;
            }
        }

        public List<NELoopDetails> GetLoopDetails(double longitude, double latitude, int associated_SystemId, string associated_System_Type, int structure_id)
        {
            return DALoopMangment.Instance.GetLoopDetails(longitude, latitude, associated_SystemId, associated_System_Type, structure_id);
        }

        public NELoopDetails UpdateLoopDetails(int associated_system_id, string associated_entity_type, string associated_network_id, List<NELoopDetails> lstLoop,  NetworkCodeIn objIn, int userId = 0)
        {

            return DALoopMangment.Instance.UpdateLoopDetails(associated_system_id, associated_entity_type, associated_network_id, lstLoop, objIn, userId);
        }

        public int DeleteLoopDetailById(int system_id)
        {
            return DALoopMangment.Instance.DeleteLoopDetailById(system_id);

        }


        public List<NELoopDetails> GetLoopDetailsForCable(int associated_SystemId)
        {
            return DALoopMangment.Instance.GetLoopDetailsForCable(associated_SystemId);
        }


        public List<NECableDetails> GetNearByCableDetails(double longitude, double latitude, int bufferInMtrs)
        {
            return DALoopMangment.Instance.GetNearByCableDetails(longitude, latitude, bufferInMtrs);
        }


    }
}
