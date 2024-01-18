using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLCustomer
    {
        public Customer SaveCustomer(Customer objCustomer, int userId)
        {
            return new DACustomer().SaveCustomer(objCustomer, userId);
        }

        public bool IsCustomerCodeExists(string customre_code, int userid = 0)
        {
            return new DACustomer().IsCustomerCodeExists(customre_code, userid);
        }
        public List<Customer> getCustomers(int parentSystemId, string parentEntityType)
        {
            return new DACustomer().getCustomers(parentSystemId, parentEntityType);
        }
        public Customer GetCustomerByCanId(string canId)
        {
            return new DACustomer().GetCustomerByCanId(canId);
        }
        public Customer getCustomerbyId(int systemId)
        {
            return new DACustomer().getCustomerbyId(systemId);
        }
        public TicketMaster UpdateTicketStatus(int ticket_id, string reference_type, int step_id, string address)
        {
            return new DAticketMaster().UpdateTicketStatus(ticket_id, reference_type, step_id, address);
        }
        public TicketMaster UpdateTicketMaster(int ticket_id, string reference_type, int step_id, int user_id)
        {
            return new DAticketMaster().UpdateTicketMaster(ticket_id, reference_type, step_id, user_id);
        }

        public TicketStepsMaster UpdateTicketIsProcessed(int ticket_id, int ticket_type_id)
        {
            return new DATicketStepMaster().UpdateTicketIsProcessed(ticket_id, ticket_type_id);
        }

        public DbMessage SaveCustomerInfo(int system_id, string can_id, string customer_name, string address, string building_code, string rfs_type, int floor_id, double latitude, double longitude, int structure_id, int user_id)
        {
            return new DACustomer().SaveCustomerInfo(system_id, can_id, customer_name, address, building_code, rfs_type, floor_id, latitude, longitude, structure_id, user_id);
        }
        public List<Dictionary<string, string>> GetSiteCustomerList(int siteId, string lmcType)
        {
            return new DACustomer().GetSiteCustomerList(siteId, lmcType);
        }
        public int deleteCustomerbyId(int systemId)
        {
            return new DACustomer().deleteCustomerbyId(systemId);
        }
        public Customer getSiteCustomerId(string siteCustomerId)
        {
            return new DACustomer().getSiteCustomerId(siteCustomerId);
        }
        public Customer getSiteCustomerPO(string PONumber)
        {
            return new DACustomer().getSiteCustomerPO(PONumber);
        }
        public Customer getSiteCustomerPAF(string PAFNO)
        {
            return new DACustomer().getSiteCustomerPAF(PAFNO);
        }
        public List<Customer> getDarkFiberCustomers()
        {
            return new DACustomer().getDarkFiberCustomers();
        }
        #region Additional-Attributes
        public string GetOtherInfoCustomer(int systemId)
        {
            return new DACustomer().GetOtherInfoCustomer(systemId);
        }
        #endregion
    }
}
