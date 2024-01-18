using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BusinessLogics
{
    public class BLTrenchCustomerDetails
    {
        private static BLTrenchCustomerDetails objTrenchCustomerDetails = null;
        private static readonly object lockObject = new object();

        public static BLTrenchCustomerDetails Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objTrenchCustomerDetails == null)
                    {
                        objTrenchCustomerDetails = new BLTrenchCustomerDetails();
                    }
                }
                return objTrenchCustomerDetails;
            }
        }

        public List<TrenchCustomerDetails> GetTrenchCustomerDetailsRecords(int Trench_ID, string Enityt_Type)
        {
            return DATrenchCustomerDetails.Instance.GetTrenchCustomerDetailsRecords(Trench_ID, Enityt_Type);
        }

        public TrenchCustomerDetails getCustomerDetails(int customer_id)
        {
            return DATrenchCustomerDetails.Instance.getCustomerDetails(customer_id);
        }

        public TrenchCustomerDetails SaveCustomerDetails(TrenchCustomerDetails objCharges, int userId)
        {
            return DATrenchCustomerDetails.Instance.SaveCustomerDetails(objCharges, userId);
        }

        public void DeleteCustomerDetailsByID(int system_id, int trench_id)
        {
            new DATrenchCustomerDetails().DeleteCustomerDetailsByID(system_id, trench_id);
        }
    }
}
