using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;
using Models.Admin;

namespace DataAccess.Admin
{
    public class DADynamicTheme : Repository<DynamicTheme>
    {
        public List<DynamicTheme> getThemes()
        {
            try
            {
                return repo.GetAll().ToList();
            }
            catch { throw; }
        }
        public DynamicTheme GetCssContentFortheme(int themeId)

        {
            try
            {
                var obj = repo.GetAll();
                DynamicTheme d=new DynamicTheme();
                return repo.Get(m => m.theme_id == themeId);

            }
            catch { throw; }
        }
        //public DynamicTheme setActive(DynamicTheme objJob)

        //{
        //    try
        //    {
        //        var obj = repo.GetAll();
        //        //if (obj != null)
        //        //{
        //        //    //obj.theme_name = objJob.theme_name;
        //        //    //obj.css_file_content = objJob.css_file_content;
        //        //    //obj.thumbnail = objJob.thumbnail;
        //        //    //obj.is_active = true;
        //        //    //repo.Update(obj);
        //        //}
        //        return ;
        //    }
        //    catch { throw; }
        //}


    }
}
