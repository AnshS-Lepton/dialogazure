using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
   public  class BLReference
    {
        private static BLReference objReference = null;
        private static readonly object lockObject = new object();
        public static BLReference Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objReference == null)
                    {
                        objReference = new BLReference();
                    }
                }
                return objReference;
            }
        }

        public List<Reference> GetReference(int entityid,string entityType)
        {
            return DLReference.Instance.GetReference(entityid, entityType);
        }
        public void SaveReference(EntityReference objReference, int system_id)
        {
            DLReference.Instance.SaveReference(objReference, system_id);
        }

           
    }
}
