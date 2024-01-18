using DataAccess;
using Models;
using Models.TempUpload;
using System.Collections.Generic;

namespace BusinessLogics
{
    public class BLWirelineCustomer
    {
        public Customer SaveCustomer(Customer objCustomer, int userId)
        {
            return new DACustomerWireline().SaveCustomer(objCustomer, userId);
        }
        public int DeleteCustomerById(int systemId)
        {
            return new DACustomerWireline().DeleteCustomerById(systemId);
        }

        public ErrorLog SaveBulkCustomer(List<Customer> lstCustomer)
        {
            return new DACustomerWireline().SaveBulkCustomer(lstCustomer);
        }

        public bool IsCustomerCodeExists(string customre_code, int userid = 0)
        {
            return new DACustomerWireline().IsCustomerCodeExists(customre_code, userid);
        }
        public List<Customer> getCustomers(int parentSystemId, string parentEntityType)
        {
            return new DACustomerWireline().getCustomers(parentSystemId, parentEntityType);
        }
        public void InsertWirelineCustomerIntoMainTable(UploadSummary summary)
        {
            new DATempCustomer().InsertWirelineCustomerIntoMainTable(summary);
        }
        public ErrorLog SaveBulkCustomer(List<TempWirelineCustomer> lstCustomer)
        {
            return new DATempCustomer().SaveBulkCustomer(lstCustomer);
        }

    }
    
}
