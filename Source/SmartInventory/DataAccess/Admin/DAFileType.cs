using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

namespace DataAccess
{
    public class DAfiletype : Repository<DataUploaderModel>
    {
        public List<fileTypes> getFileType(string moduleAbbr)
        {
            try
            {
                return repo.ExecuteProcedure<fileTypes>("fn_get_file_type", new { p_module_abbr = moduleAbbr }, true);
            }
            catch
            {

                throw ;
            }
        }
        public List<fileTypes> getfiletype_withcablesplit(string moduleAbbr, bool iscablesplit)
        {
            try
            {
                return repo.ExecuteProcedure<fileTypes>("fn_get_file_type_for_splitcable", new { p_module_abbr = moduleAbbr, p_splitcable = iscablesplit }, true);
            }
            catch
            { throw; }
        }
    }
}