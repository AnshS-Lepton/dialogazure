using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace BusinessLogics
{
   public class BLCustomerInfo
    {
        public CustomerInfo GetCustomerInfoByCanId(string canId, string entity_type, int ticket_id)
        {        
            return new DACustomerInfo().GetCustomerInfoByCanId(canId, entity_type, ticket_id);
        } 

    }
}
