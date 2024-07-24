using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLTicketManager
    {
        public TicketMaster SaveTicket(TicketMaster objTicketMaster, int userId)
        {
            return new DATicketMaster().SaveTicket(objTicketMaster, userId);
        }
        public List<TicketTypeMaster> GetTicketType()
        {
            return new DATicketTypeMaster().GetTicketType();
        }
        public int GetTicketTypeByID(string ticket_type)
        {
            return new DATicketTypeMaster().GetTicketTypeByID(ticket_type);
        }
        public List<TicketMasterGrid> getTicketList(int userId, TicketManagerFilter objTicketManagerFilter)
        {
            return new DATicketTypeMaster().getTicketList(userId, objTicketManagerFilter);
        }
        public List<ticketStatus> getTicketStatusCounts(int userId)
        {
            return new DATicketTypeMaster().getTicketStatusCounts(userId);
        }
        public bool DeleteTicketById(string ticket_ids, int userId)
        {
            return new DATicketMaster().DeleteTicketById(ticket_ids, userId);
        }
        public TicketMaster GetTicketById(int ticket_id)
        {
            return new DATicketMaster().GetTicketById(ticket_id);
        }
        public List<TicketMasterGrid> GetTicketDetails(int userId, TicketManagerFilter objViewFilter)
        {
            return new DATicketMaster().GetTicketDetails(userId, objViewFilter);
        }
        public TicketMaster GetTicketDetailByCanId(string canId)
        {
            return new DATicketMaster().GetTicketDetailByCanId(canId);
        }
        public DbMessage ValidateRfsType(string rfs_type)
        {
            return new DATicketMaster().ValidateRfsType(rfs_type);
        }
        public Customer_Response SaveCustomerTicket(CustomerTicketMaster objTicketMaster)
        {
            return new DATicketMaster().SaveCustomerTicket(objTicketMaster);
        }
        public customerTicketStatus GetcustomerTicketStatus(int id)
        {
            return new DATicketMaster().GetcustomerTicketStatus(id);
        }
    }

    public class BLTempTicketMaster
    {
        private static BLTempTicketMaster objTicketMaster = null;
        private static readonly object lockObject = new object();
        public static BLTempTicketMaster Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objTicketMaster == null)
                    {
                        objTicketMaster = new BLTempTicketMaster();
                    }
                }
                return objTicketMaster;
            }
        }

        public void DeleteTempTicketData(int UserId)
        {
           DATempTicketMaster.Instance.DeleteTempTicketData(UserId);
        }
        public void BulkUploadTempTicket(List<TempTicketMaster> BulkUploadTempTicket)
        {
            DATempTicketMaster.Instance.BulkUploadTempTicket(BulkUploadTempTicket);
        }
        public DbMessage UploadTickets(int UserId)
        {
            return DATempTicketMaster.Instance.UploadTickets(UserId);
        }

        public List<TempTicketMaster> GetUploadTicketLogs(int userId) 
        {
            return DATempTicketMaster.Instance.GetUploadTicketLogs(userId);
        }

        public Tuple<int, int> getTotalUploadTicketfailureAndSuccess(int UserId)
        {
            return DATempTicketMaster.Instance.getTotalUploadTicketfailureAndSuccess(UserId);
        }
    }

    public class BLHPSMTicket
    {
        public TicketMaster SaveHPSMTicket(TicketMaster objTicketMaster)
        {
            return new DAHPSMTicket().SaveHPSMTicket(objTicketMaster);
        }
        public bool SaveTicketAttachments(List<TicketAttachments> obj, int user_id)
        {
            return new DATicketAttachments().SaveTicketAttachments(obj, user_id);
        }
    }
}
