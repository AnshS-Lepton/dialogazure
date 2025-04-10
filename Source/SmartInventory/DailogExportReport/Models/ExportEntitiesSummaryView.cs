using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace DailogExportReport.Models
{
    public class ExportEntitiesSummaryView
    {
       
            public ExportEntitiesSummaryViewFilter objReportFilters { get; set; }
            public List<dynamic> lstReportData { get; set; }

            public List<WebGridColumn> webColumns { get; set; }
            public List<layerReportDetail> lstLayers { get; set; }
            public List<KeyValueDropDown> lstLayerColumns { get; set; }
            public List<ReportAdvanceFilter> lstAdvanceFilters { get; set; }
            public ExportEntitiesSummaryView()
            {
                lstLayers = new List<layerReportDetail>();
                lstLayerColumns = new List<KeyValueDropDown>();
                lstReportData = new List<dynamic>();
                webColumns = new List<WebGridColumn>();
                objReportFilters = new ExportEntitiesSummaryViewFilter();
                lstAdvanceFilters = new List<ReportAdvanceFilter>();
            }
        }
    
}
