using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
  public  class DASiteCustomer : Repository<SiteCustomer>
    {
        private static DASiteCustomer objSiteCustomer = null;
        private static readonly object lockObject = new object();
        public static DASiteCustomer Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSiteCustomer == null)
                    {
                        objSiteCustomer = new DASiteCustomer();
                    }
                }
                return objSiteCustomer;
            }
        }
        public SiteCustomer GetSiteCustomerInfoById(int systemId) 
        {
            var result = repo.Get(u => u.system_id == systemId);
            return result != null ? result : new SiteCustomer();
        }
        public List<Dictionary<string, string>> GetSiteCustomerList(int siteId,string lmcType)
        {
            try
            {
                
                 return repo.ExecuteProcedure <Dictionary<string, string>>("fn_get_site_customer_by_id", new { p_siteId = siteId,p_lmcType=lmcType },true);
            }
            catch (Exception)
            {

                throw;
            }
           // return repo.GetAll(u => u.site_id == siteId).ToList();
            
        }

        public SiteCustomer SaveSiteCustomer(SiteCustomer objSiteCustomer, int userId)
        {
            try 
            {
                var result = repo.Get(u => u.system_id == objSiteCustomer.system_id);
                if (result != null)
                {
                    result.customer_name = objSiteCustomer.customer_name;
                    result.customer_site_id = objSiteCustomer.customer_site_id;
                    result.customer_area = objSiteCustomer.customer_area;
                    result.telco_name = objSiteCustomer.telco_name;
                    result.opco = objSiteCustomer.opco;
                    result.electrical_meter_type = objSiteCustomer.electrical_meter_type;
                    result.rtn_name = objSiteCustomer.rtn_name;
                    result.small_cell_installed = objSiteCustomer.small_cell_installed;
                    result.agl = objSiteCustomer.agl;
                    result.is_power_back_up_available = objSiteCustomer.is_power_back_up_available;
                    result.power_back_up_capacity = objSiteCustomer.power_back_up_capacity;
                    result.cluster_name = objSiteCustomer.cluster_name;
                    result.co_name = objSiteCustomer.co_name;
                    result.co_id = objSiteCustomer.co_id;
                    result.co_latitude = objSiteCustomer.co_latitude;
                    result.co_longitude = objSiteCustomer.co_longitude;
                    result.cluster_id = objSiteCustomer.cluster_id;
                    result.paf_no = objSiteCustomer.paf_no;
                    result.paf_signing_date = objSiteCustomer.paf_signing_date;
                    result.paf_expiry_date = objSiteCustomer.paf_expiry_date;

                    result.cable_entry_point = objSiteCustomer.cable_entry_point;
                    result.remote_pop = objSiteCustomer.remote_pop;
                    result.order_tenure = objSiteCustomer.order_tenure;
                    result.site_details = objSiteCustomer.site_details;
                    result.po_number = objSiteCustomer.po_number;
                    result.po_issue_date = objSiteCustomer.po_issue_date;
                    result.po_expiry_date = objSiteCustomer.po_expiry_date;
                    result.rfai_date = objSiteCustomer.rfai_date;
                    result.rfs_date = objSiteCustomer.rfs_date;
                    result.contract_end_date = objSiteCustomer.contract_end_date; 
                    result.modified_by = userId;
                    result.modified_on = DateTimeHelper.Now;
                    return repo.Update(result);
                }
                else
                {
                    objSiteCustomer.created_by = userId;
                    objSiteCustomer.created_on = DateTimeHelper.Now;
                    objSiteCustomer.site_id = objSiteCustomer.site_id;
                    result = repo.Insert(objSiteCustomer);
                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public SiteCustomer getSiteCustomerbyId(int system_id)
        {
            try
            {
                return repo.GetById(m => m.system_id == system_id); 
            }
            catch { throw; }
        }
        public SiteCustomer getSiteCustomerId(string siteCustomerId)
        {
            try
            {
                var result= repo.GetById(m => m.customer_site_id == siteCustomerId);
                return result != null ? result : new SiteCustomer();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public SiteCustomer getSiteCustomerPO(string PONumber)
        {
            try
            {
                var result = repo.GetById(m => m.po_number == PONumber);
                return result != null ? result : new SiteCustomer();
            }
            catch (Exception ex)
            { 

                throw ex;
            }
        }
        public SiteCustomer getSiteCustomerPAF(string PAFNO)
        {
            try
            {
                var result = repo.GetById(m => m.paf_no == PAFNO);
                return result != null ? result : new SiteCustomer();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public SiteCustomer getSiteCustomer(int systemId)
        {
            try
            {
                var result = repo.GetById(x => x.system_id ==systemId);
                return result != null ? result : new SiteCustomer();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public int deleteCustomerbyId(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(m => m.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }
    }
}
