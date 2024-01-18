using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogics.Admin;
using Models.Admin;

namespace SmartInventory.Areas.Admin.Controllers
{
    public class DynamicThemeController : Controller
    {
       public ActionResult DynamicTheme()
        {
            DynamicTheme obj = new DynamicTheme();
            obj.themelist = new BLDynamicTheme().getThemes();
            return View(obj);
        
        
        }
        public ActionResult SaveTheme(int themeId)
        {
            var mainFile = Settings.ApplicationSettings.ColorStyleSheetFilePath;
            var mainPath = Path.Combine(Server.MapPath(mainFile));
            DynamicTheme objDynamicTheme = new DynamicTheme();
            objDynamicTheme = new BLDynamicTheme().GetCssContentFortheme(themeId);
            String file = objDynamicTheme.css_file_content;
            System.IO.File.WriteAllText(mainPath, file);
            return Json(true);
        }
    }
}