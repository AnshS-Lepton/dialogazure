using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartFeasibility.Helper
{
    public static class ModuleAccessHelper
    {
        private static readonly string FeasibilityAbbr = "SFS";
        public static bool FeasibilityModuleAccess(User u, string ModuleAbbr = "SFS")
        {
            IEnumerable<Modules> modules = BLAdvancedSettings.Instance.GetModuleMasterList().Where(m => m.module_abbr == FeasibilityAbbr && m.type.ToLower() == "web" && m.is_active);
            Modules module;
            if(modules != null && modules.Count() > 0)
            {
                Modules feasibilityMainModule = modules.FirstOrDefault();
                if (ModuleAbbr.Equals(FeasibilityAbbr))
                {
                    module = feasibilityMainModule;
                }
                else
                {
                    module = BLAdvancedSettings.Instance.GetModuleMasterList().Where(m => m.module_abbr == ModuleAbbr && m.parent_module_id == feasibilityMainModule.id && m.is_active).FirstOrDefault();
                }

                if (u.lstUserModuleMapping.Count() == 0)
                {
                    u.lstUserModuleMapping = new BLUserModuleMapping().GetModuleMapping(u.user_id);
                }

                return module != null && u.lstUserModuleMapping.Where(l => l.module_id == module.id).Count() > 0;
            }

            return false;
        }
    }
}