using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.SqlServer.Server;
using Models;
using Models.Admin;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Threading;

namespace BusinessLogics
{
    public class BLExportData
    {
        public List<fileTypes> getfiletype(string moduleAbbr)
        {
            return new DAfiletype().getFileType(moduleAbbr);
        }
        public List<fileTypes> getfiletype_withcablesplit(string moduleAbbr, bool iscablesplit)
        {
            return new DAfiletype().getfiletype_withcablesplit(moduleAbbr, iscablesplit);
        }
    }
}
