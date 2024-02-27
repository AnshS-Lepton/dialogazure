using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using DataAccess.DBHelpers;

namespace DataAccess
{
    public class DALoop : Repository<NELoopDetails>
    {
        public PageMessage SaveEntityLoop(string Loops, int province_id)
        {
            try
            {
                var r = repo.ExecuteProcedure<PageMessage>("fn_save_loop_details", new { p_loops = Loops }).FirstOrDefault();
                DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(r.systemId, Models.EntityType.Loop.ToString(), province_id, 0);
                return r;
            }
            catch { throw; }
        }
    }
}
