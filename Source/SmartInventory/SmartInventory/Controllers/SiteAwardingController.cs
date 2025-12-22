using BusinessLogics.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models.Admin;
using Models;

namespace SmartInventory.Controllers
{
    public class SiteAwardingController : Controller
    {
        public ActionResult ShowSiteAwardDetails(int currentPage = 1, int pageSize = 10, string col = "", string dir = "ASC")
        {
            var siteprojectdetails = new siteprojectdetailsFilter();

            siteprojectdetails = new BLProject().GetSiteAwardingProjectDetails(currentPage, pageSize, col, dir);
            return PartialView("_ShowSiteAwardDetails", siteprojectdetails);
        }
       
    }
}