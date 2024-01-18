using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
   public class DASiteInfo : Repository<SiteInfo>
    {
        private static DASiteInfo objSiteInfo = null;
        private static readonly object lockObject = new object();
        public static DASiteInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSiteInfo == null)
                    {
                        objSiteInfo = new DASiteInfo();
                    }
                }
                return objSiteInfo;
            }
        }
        public SiteInfo GetSiteInfo(int systemId,string entityType)
        {
            var result = repo.Get(u => u.parent_system_id == systemId && u.parent_entity_type == entityType);
            return result != null ? result : new SiteInfo();
        }
        public SiteInfo ValidateETPILID(string UID)
        {
            var result = repo.Get(u => u.etpil_id == UID);
            return result != null ? result : new SiteInfo();
        }
        public SiteInfo SaveSiteInfo(SiteInfo objSiteInfo, int userId)
        {
            try
            {
                var result = repo.Get(u => u.system_id == objSiteInfo.system_id);
                if (result!=null)
                {
                    result.acquisition_date = objSiteInfo.acquisition_date;
                    result.acq_vendor = objSiteInfo.acq_vendor;
                    result.building_height = objSiteInfo.building_height;
                    result.circle = objSiteInfo.circle;
                    result.city = objSiteInfo.city;
                    result.etpil_id = objSiteInfo.etpil_id;
                    result.home_pass = objSiteInfo.home_pass;
                    result.is_ladder_required = objSiteInfo.is_ladder_required;
                    result.latitude = objSiteInfo.latitude;
                    result.longitude = objSiteInfo.longitude;
                    result.lmc_type = objSiteInfo.lmc_type;
                    result.site_area = objSiteInfo.site_area;
                    result.site_name = objSiteInfo.site_name;
                    result.site_type = objSiteInfo.site_type;
                    result.site_vendor = objSiteInfo.site_vendor;
                    result.structure_height = objSiteInfo.structure_height;
                    result.structure_size = objSiteInfo.structure_size;
                    result.structure_type = objSiteInfo.structure_type;
                    result.parent_system_id = objSiteInfo.parent_system_id;
                    result.parent_network_id = objSiteInfo.parent_network_id;
                    result.parent_entity_type = objSiteInfo.parent_entity_type;
                    result.modified_by = userId;
                    result.modified_on = DateTimeHelper.Now;
                    result.objPM.isNewEntity = false;
                    return repo.Update(result);
                }
                else
                {
                    objSiteInfo.created_by = userId;
                    objSiteInfo.created_on = DateTimeHelper.Now;
                    objSiteInfo.objPM.isNewEntity = true;
                     result = repo.Insert(objSiteInfo);
                    return result;
                } 
            }
            catch (Exception) 
            {

                throw;
            }
        }

        public List<SiteInfo> GetSiteVendorListbyId(string searchText)
        {
            try
            {
                return repo.GetAll(x=>x.site_vendor.Contains(searchText)).Take(10).ToList();
                 
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<SiteInfo> GetSiteVendorList(string searchText) 
        {
            try
            {
                //return repo.GetAll(x => x.site_vendor.ToUpper().Contains(searchText.ToUpper())).Take(10).Distinct().ToList();
                return repo.ExecuteProcedure<SiteInfo>("fn_get_site_vendor_list", new { searchtext = searchText },true);
            }
            catch { throw; }
        }
        public List<SiteInfo> GetSiteUIDList(string searchText)
        {
            try
            {
                return repo.ExecuteProcedure<SiteInfo>("fn_get_site_UID_list", new { searchtext = searchText }, true);
            }
            catch { throw; }
        }

        public SiteInfo getSitebyId(int siteId)
        {
            try
            {
                var result = repo.Get(u => u.system_id == siteId); 
                return result != null ? result : new SiteInfo();
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
