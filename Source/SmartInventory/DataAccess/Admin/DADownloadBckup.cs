using DataAccess.DBHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using DataAccess.DBHelpers;
using Models;
using Models.Feasibility;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Admin;

namespace DataAccess.Admin
{
public class DADownloadBckup:Repository<downloadbckupfile>
    {

        public List<downloadbckupfile> downloadstatusbck()
        {
            List<downloadbckupfile> lst = new List<downloadbckupfile>();
            DataTable dwnldstatus = null;
            dwnldstatus = repo.GetDataTable("select distinct on (filetype) filetype,status,id from DownLoadStatus where download_strt > current_date order by filetype,id desc");
            foreach (DataRow row in dwnldstatus.Rows)
            {
                downloadbckupfile obj = new downloadbckupfile();
                obj.status = row["status"].ToString();
                obj.file_type=row["filetype"].ToString();
                lst.Add(obj);
            }
                
            return lst;
        }
    }
}
