using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.Admin;
using Models.Admin;
using System.Data;

namespace BusinessLogics.Admin
{
    public class BLDynamicTheme
    {
        public List<DynamicTheme> getThemes()
        {
            return new DADynamicTheme().getThemes();

        }
        public DynamicTheme GetCssContentFortheme(int Id)
        {
            return new DADynamicTheme().GetCssContentFortheme(Id);
        }

        //public DynamicTheme setActive(DynamicTheme objJob)
        //{
        //    return new DADynamicTheme().setActive(objJob);

        //}
    }
}

