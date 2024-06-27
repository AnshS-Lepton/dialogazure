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
    }
}