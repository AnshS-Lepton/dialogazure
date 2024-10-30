using DialogDTSIntegration;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSDialogIntegration
{
    public class BLDTSIntegration
    {
        private readonly DADTSIntegration _daSite;

        public BLDTSIntegration(IConfiguration configuration)
        {
            _daSite = new DADTSIntegration(configuration);
        }

        public int EntryLogInProcessSummary()
        {
            return _daSite.EntryLogInProcessSummary();
        }

        public void ExitLogInProcessSummary(int processID)
        {
            _daSite.ExitLogInProcessSummary(processID);
        }
        public void SaveSitesList(List<SiteDetails> siteList, int processID)
        {
            _daSite.SaveSitesList(siteList, processID);
        }
        public List<string> GetSiteIdsByProcessId(int processID)
        {
            return _daSite.GetSiteIdsByProcessId(processID);
        }
        public void UpdateSiteAsync(string site_id, int processID, string message)
        {
            _daSite.UpdateSiteAsync(site_id, processID,message);
        }

        public void SaveSiteDetails(SiteAttributes objsite, int userId, int progressID)
        {
            _daSite.SaveSiteDetails(objsite, userId, progressID);
        }

        public ProcessSiteOutput SaveSiteDetilsInMainTable(int processID)
        {
            return _daSite.SaveSiteDetilsInMainTable(processID);
        }
        public void UpdateNetworkandGeomDetails(int processID)
        {
            _daSite.UpdateNetworkandGeomDetails(processID);
        }
    }
}
