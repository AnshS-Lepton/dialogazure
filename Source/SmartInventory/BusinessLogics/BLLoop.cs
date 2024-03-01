using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace BusinessLogics
{
    public class BLLoop
    {
        public PageMessage SaveEntityLoop(string Loops)
        {
              return new DALoop().SaveEntityLoop(Loops);
        }
    }
}
