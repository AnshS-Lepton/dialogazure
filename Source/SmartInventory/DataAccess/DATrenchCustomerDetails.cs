using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DATrenchCustomerDetails : Repository<TrenchCustomerDetails>
    {
        private static DATrenchCustomerDetails objTrenchCustomerDetails = null;
        private static readonly object lockObject = new object();
        public static DATrenchCustomerDetails Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objTrenchCustomerDetails == null)
                    {
                        objTrenchCustomerDetails = new DATrenchCustomerDetails();
                    }
                }
                return objTrenchCustomerDetails;
            }
        }

        public TrenchCustomerDetails getCustomerDetails(int system_id)
        {
            try
            {
                return repo.GetById(m => m.system_id == system_id);
            }
            catch { throw; }
        }

        public TrenchCustomerDetails SaveCustomerDetails(TrenchCustomerDetails objCustomerDetails, int userId)
        {
            var CustomerDetails = repo.GetById(m => m.system_id == objCustomerDetails.system_id);
            if (CustomerDetails != null)
            {
                CustomerDetails.state = objCustomerDetails.state;
                CustomerDetails.customer_name = objCustomerDetails.customer_name;
                CustomerDetails.service_type = objCustomerDetails.service_type;
                CustomerDetails.section_name = objCustomerDetails.section_name;
                CustomerDetails.pair_requirement = objCustomerDetails.pair_requirement;
                CustomerDetails.length_in_kms = objCustomerDetails.length_in_kms;
                CustomerDetails.status = objCustomerDetails.status;
                CustomerDetails.po_no = objCustomerDetails.po_no;
                CustomerDetails.po_length_km = objCustomerDetails.po_length_km;
                CustomerDetails.po_release_date = objCustomerDetails.po_release_date;
                CustomerDetails.period = objCustomerDetails.period;
                CustomerDetails.from_date = objCustomerDetails.from_date;
                CustomerDetails.to_date = objCustomerDetails.to_date;
                CustomerDetails.hoto_no = objCustomerDetails.hoto_no;
                CustomerDetails.urid = objCustomerDetails.urid;
                CustomerDetails.hoto_status = objCustomerDetails.hoto_status;
                CustomerDetails.hoto_date = objCustomerDetails.hoto_date;
                CustomerDetails.route_name = objCustomerDetails.route_name;
                CustomerDetails.hoto_length = objCustomerDetails.hoto_length;
                CustomerDetails.total_core = objCustomerDetails.total_core;
                CustomerDetails.live_core = objCustomerDetails.live_core;
                CustomerDetails.reserved_core = objCustomerDetails.reserved_core;
                CustomerDetails.modified_by = userId;
                CustomerDetails.modified_on = DateTimeHelper.Now;

                return repo.Update(objCustomerDetails);

            }
            else
            {
                objCustomerDetails.created_by = userId;
                //CustomerDetails.created_on =  DateTimeHelper.Now;
                return repo.Insert(objCustomerDetails);
            }

        }

        public List<TrenchCustomerDetails> GetTrenchCustomerDetailsRecords(int Trench_ID, string Enityt_Type)
        {
            try
            {

                return repo.ExecuteProcedure<TrenchCustomerDetails>("fn_get_Trench_CustomerDetails", new { p_trenchyid = Trench_ID }, true).ToList();

            }
            catch { throw; }
        }


        public void DeleteCustomerDetailsByID(int system_id, int trench_id)
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_delete_trench_customerdetails", new { p_system_id = system_id, p_trench_id = trench_id });
            }
            catch { throw; }
        }

    }
}
