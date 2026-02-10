using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using static DataAccess.DANetworkTicket;

namespace BusinessLogics
{
    public class BLNetworkTicket

    {

        public NWTicket_List_Status GetAPINetworkTicket(NetworkTicketFilter objNetworkTicketFilter)
        {
            return new DANetworkTicket().GetAPINetworkTicket(objNetworkTicketFilter);
        }
        
        public NetworkTicketResponse SaveNetworkTicket(NetworkTicket objTicketMaster, int userId)
        {
            return new DANetworkTicket().SaveNetworkTicket(objTicketMaster, userId);
        }
        public string SaveNetworkTicketfromItemVCost(NetworkTicket objTicketMaster, int userId)
        {
            return new DANetworkTicket().SaveNetworkTicketfromItemVCost(objTicketMaster, userId);
        }
        public List<TicketTypeMaster> GetTicketTypeByModule(string module, int uid=0,int role_id=0,int ticked_id=0)
        {
            return new DANetworkTicketTypeMaster().GetTicketTypeByModule(module,uid,role_id, ticked_id);
        }
        public List<TicketTypeMaster> GetHPSMTicketType(string module)
        {
            return new DANetworkTicketTypeMaster().GetHPSMTicketType(module);
        }
        public DashboardInfo GetNetworkTicket(NetworkTicketFilter objNetworkTicketFilter)
        {
            return new DANetworkTicket().GetNetworkTicket(objNetworkTicketFilter);
        }
        public NetworkTicket GetNetworkTicketById(int ticket_id)
        {
            return new DANetworkTicket().GetNetworkTicketById(ticket_id);
        }
        public NetworkTicket GetNetworkTicketByNetworkId(string network_id)
        {
            return new DANetworkTicket().GetNetworkTicketByNetworkId(network_id);
        }

		public DataTable GetNetworkTicketDetailsById(int ticket_id)
		{
			return new DANetworkTicket().GetNetworkTicketDetailsById(ticket_id);
		}
		public DbMessage DeleteNetworkTicketById(int ticket_id, int userId)
        {
            return new DANetworkTicket().DeleteNetworkTicketById(ticket_id, userId);
        }
        public DbMessage UpdateAcknowledgement(NWTAcknowledgement ack)
        {
            return new DANetworkTicket().UpdateAcknowledgement(ack);
        }
        public NWEntityInfo GetNetworkTicketEntityDetails(NetworkTicketFilter objNetworkTicketFilters)
        {
            return new DANetworkTicket().GetNetworkTicketEntityDetails(objNetworkTicketFilters);
        }
		public List<NetworkTicketEntityList> GetNetworkTicketRejectedEntityStatus(int ticket_id)
		{
			return new DANetworkTicket().GetNetworkTicketRejectedEntityStatus(ticket_id);
		}
		public List<Dictionary<string, string>> Get_NWEntity_History( NetworkTicketFilter objNetworkTicketFilter)
        {
            return new DANetworkTicket().Get_NWEntity_History(objNetworkTicketFilter);
        }
        public List<Dictionary<string, string>> GetDraftEntityInfo( NetworkTicketFilter objNetworkTicketFilter)
        {
            return new DANetworkTicket().GetDraftEntityInfo(objNetworkTicketFilter);
        }
        public DbMessage EditEntityInfo(EntityInfo objEntityInfo)
        {
            return new DANetworkTicket().EditEntityInfo(objEntityInfo);
        }
        public GetGeometryByTicketId getNetworkTicketGeometry(int ticket_id)
        {
            return new DANetworkTicket().getNetworkTicketGeometry(ticket_id);
        }
        public NWTktEntityLst GetNetworkTicketEntityList(NetworkTicketEntityListParam objparam)
        {
            return new DANetworkTicket().GetNetworkTicketEntityList(objparam);
        }
        public List<Dictionary<string, string>> NWEntitySummaryReport(int ticket_id, int user_id)
        {
            return new DANetworkTicket().NWEntitySummaryReport(ticket_id, user_id);
        }
        public List<DropDownMaster> GetDropDownList(string ddType)
        {
            return new DANetworkTicket().GetDropDownList(ddType);
        }
        public string getTicketEntityBounds(int ticketId)
        {
            return new DANetworkTicket().getTicketEntityBounds(ticketId);
        }
		public string getTicketBounds(int ticketId)
		{
			return new DANetworkTicket().getTicketBounds(ticketId);
		}
		public string getTicketId(int system_id, string entityType, string network_id)
        {
            return new DANetworkTicket().getTicketId(system_id, entityType, network_id);
        }
        public List<NetworkTicketEmailDetail> GetNetworkTicketDetail(int ticket_id)
        {
            return new DANetworkTicket().GetNetworkTicketDetail(ticket_id);
        }

    }
}
