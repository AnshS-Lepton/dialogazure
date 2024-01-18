using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLAdditionalAttributes
    {
        public AdditionalAttributes SaveAttributes(AdditionalAttributes objAttributes)
        {
            return new DAAdditionalAttributes().SaveAttributes(objAttributes);
        }
        public AdditionalAttributes getAttributes(int systemId, string entityType)
        {
            return new DAAdditionalAttributes().getAttributes(systemId, entityType);
        }
    }
}
