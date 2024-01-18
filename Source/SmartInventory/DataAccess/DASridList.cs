using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

namespace DataAccess
{
    public class DASridList : Repository<SourceIdList>
    {
        public List<SourceIdList> getDxfSourceList()
        {
            try
            {
                return repo.GetAll().ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
