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
        public PageMessage SaveEntityLoop(string Loops)
        {
            try
            {
                return repo.ExecuteProcedure<PageMessage>("fn_save_loop_details", new { p_loops = Loops}).FirstOrDefault(); ;
                
            }
            catch { throw; }
        }
    }
}
