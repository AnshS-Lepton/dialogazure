using System;
using System.Collections.Generic;
using Models;
using Models.Admin;
using DataAccess.DBHelpers;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAXMLBuilderDashboard : Repository<XMLBuilderDashboardMaster>
    {

        public List<XMLBuilderDashboardMaster> GetXMLBuilderDashboard(XMLBuilderDashboardFilter objXMLBuilderDashboardFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<XMLBuilderDashboardMaster>("fn_process_get_xml_summary", new
                {
                    p_searchText = objXMLBuilderDashboardFilter.searchText,
                    p_pageno = objXMLBuilderDashboardFilter.pageno,
                    p_pagerecord = objXMLBuilderDashboardFilter.pagerecord,
                    p_sortcolname = objXMLBuilderDashboardFilter.sortcolname,
                    p_sorttype = objXMLBuilderDashboardFilter.sorttype,
                    p_totalrecords = objXMLBuilderDashboardFilter.totalrecords

                }, true).ToList();
                return lst;
               
            }
            catch (Exception ex) 
            { 
                throw; 
            }
        }
    }

   

}