using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DLReference : Repository<Reference>
    {


        private static DLReference objreference = null;
        private static readonly object lockObject = new object();
        public static DLReference Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objreference == null)
                    {
                        objreference = new DLReference();
                    }
                }
                return objreference;
            }
        }

        public List<Reference> GetReference(int entityid, string entitytype)
        {
            try
            {
             return repo.GetAll(m => m.system_id== entityid && m.entity_type.ToUpper() == entitytype.ToUpper()).ToList();       
            }
            catch { throw; }
        }

        public void SaveReference(EntityReference objEntityReference, int system_id)
        {
            try
            {
                if (objEntityReference.listPointAReference != null)
                {
                    List<Reference> objReference = new List<Reference>();
                    objReference.AddRange(objEntityReference.listPointAReference);
                    if (objEntityReference.listPointBReference != null)
                    {
                        objReference.AddRange(objEntityReference.listPointBReference);
                    }
                    var listReferenceDelete = objReference.Where(x => x.id > 0 && (x.landmark == string.Empty || x.landmark == null)).ToList();
                    var listReference = objReference.Where(x => x.landmark != string.Empty && !string.IsNullOrEmpty(x.distance.ToString())).ToList();


                    var listReferenceUpdate = listReference.Where(x => x.id > 0).ToList();
                    var listReferenceInsert = listReference.Where(x => x.id == 0).ToList();

                    if (listReferenceDelete.Any())
                    {
                        repo.DeleteRange(listReferenceDelete);
                    }
                    if (listReferenceUpdate.Any())
                    {
                        repo.Update(listReferenceUpdate);
                    }

                    if (listReferenceInsert.Any())
                    {
                        // modify the new system_id for new entries save 
                        if (listReferenceInsert[0].system_id == 0)
                        {
                            listReferenceInsert.ForEach(x => x.system_id = system_id);
                        }
                        repo.Insert(listReferenceInsert);
                    }

                }
            }
            catch { throw; }
        }

    }
}
