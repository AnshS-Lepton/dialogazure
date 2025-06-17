using DataAccess;
using Models;
using System;
using System.Collections.Generic;

namespace BusinessLogics
{
    public class BLSite
    {
        public Site Save(Site objsite, int userId)
        {
            return new DASite().Save(objsite, userId);
        }
        public int DeleteById(int systemId)
        {
            return new DASite().DeleteById(systemId);
        }
        public List<Site> GetAll(DateTime lastSuccessDate)
        {
            return new DASite().GelAll(lastSuccessDate);
        }
        public List<string> getCablesByLinkIds(string linkids)
        {
            return new DASiteSync().getCablesByLinkIds(linkids);
        }
        public List<string> GetCablesByFiberLinkIds(string linkids)
        {
            return new DASiteSync().GetCablesByFiberLinkIds(linkids);
        }
        public List<String> validateLinkIds(string linkids)
        {
            return new DASiteSync().validateLinkIds(linkids);
        }
        public List<Dictionary<string, string>> GetSiteReportData(ExportReportFilter objReportFilter)
        {
            return new DASite().GetSiteReportData(objReportFilter);
        }
        public List<NearestSiteDetails> GetSitelistData(int systemId)
        {
            return new DASite().GetSitelistData(systemId);
        }
        public List<NearestSiteDetails> getNearrestSitelistData(int system_id, string network_id, int buffer)
        {
            return new DASite().getNearestSitelistData(system_id, network_id, buffer);
        }

        public List<NearestSiteDetails> getUpdateSiteFiberDistance(string linestring, int nearestsite_system_id, int system_id, double nearestsiteDistance)
        {
            return new DASite().getUpdateSiteFiberDistance(linestring, nearestsite_system_id, system_id, nearestsiteDistance);
        }
        public List<ExportReportKML> GetExportReportDataKML(ExportReportFilter objReportFilter)
        {
            return new DASite().GetExportReportDataKML(objReportFilter);
        }
        public List<Dictionary<string, string>> GetSegmentReportData(ExportReportFilter objReportFilter)
        {
            return new DASite().GetSegmentReportData(objReportFilter);
        }
    }
}
