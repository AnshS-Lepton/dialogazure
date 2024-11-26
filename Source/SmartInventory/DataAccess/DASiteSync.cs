using DataAccess.DBHelpers;
using Models;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DASiteSync : Repository<SiteSync>
    {
        public SiteSync Save(SiteSync site)
        {
            try
            {
                var objSite = repo.Get(x => x.id == site.id);
                if (objSite != null)
                {
                   objSite.end_datetime = site.end_datetime;
                    objSite.status = site.status;
                    objSite.message= site.message;
                    objSite.lastsuccess_sync = site.lastsuccess_sync;
                    var resultItem = repo.Update(site);
                    return resultItem;
                }
                else
                {
                    var resultItem = repo.Insert(site);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteById(int Id)
        {
            try
            {
                var objSystmId = repo.Get(x => x.id == Id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }

        public List<SiteSync> GelAll()
        {
            List<SiteSync> lst = new List<SiteSync>();
            try
            {
                lst = repo.GetAll(a=>a.status== "Success").ToList();
            }
            catch { throw; }
            return lst;
        }

        public List<String> getCablesByLinkIds(string linkids)
        {
            try
            {
                return repo.ExecuteProcedure<String>("fn_cable_list_by_linkids", new { v_linkids = linkids },false);
            }
            catch { throw; }
        }
        public List<String> validateLinkIds(string linkids)
        {
            try
            {
                return repo.ExecuteProcedure<String>("fn_validate_linkids", new { v_linkids = linkids }, false);
            }
            catch { throw; }
        }
    }
}
