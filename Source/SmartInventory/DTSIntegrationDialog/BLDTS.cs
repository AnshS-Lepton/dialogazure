using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSIntegrationDialog
{
    public class BLDTS
    {
        public static int EntryLogInProcessSummary()
        {
            // Create an instance of AppDbContext
            using (var context = new AppDbContext())
            {
                // Create an instance of DADTS with the context
                var dadts = new DADTS(context);

                // Call the method to log the process summary and return the result
                return dadts.EntryLogInProcessSummary();
            }
        }

        public static void ExitLogOutProcessSummary(int processID) {
          DADTS.ExitLogOutProcessSummary(processID);
        }
        public static void SaveSitesList(List<SiteDetails> siteList, int processID)
        {
            DADTS.SaveSitesList(siteList, processID);
        }
        public static List<string> GetSiteIdsByProcessId(int processID)
        {
            return DADTS.GetSiteIdsByProcessId(processID);
        }
        public static void SaveSiteDetails(SiteAttributes objsite, int userId, int progressID)
        {
            DADTS.SaveSiteDetails(objsite, userId, progressID);
        }
        public static void UpdateSiteList(string site_id, int processID, string message)
        {
            DADTS.UpdateSiteList(site_id, processID, message);
        }
        public static ProcessSiteOutput SaveSiteDetilsInMainTable(int processID)
        {
            return DADTS.SaveSiteDetilsInMainTable(processID);
        }
        public static void UpdateNetworkandGeomDetails(int processID)
        {
            DADTS.UpdateNetworkandGeomDetails(processID);
        }
    }
}
