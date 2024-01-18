using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Models
{
   
    #region History
 
    public class ViewAuditMasterModel
    {
        public int systemId { get; set; }

        public FilterHistoryAttr objFilterAttributes { get; set; }

        public string eType { get; set; }

        public List<dynamic> lstData { get; set; }
        public List<dynamic> lstGeometryData { get; set; }
        public List<WebGridColumn> webColumns { get; set; }
        public List<Models.layerReportDetail> lstLayers { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }

        public ViewAuditMasterModel()
        {
            objFilterAttributes = new FilterHistoryAttr();
            lstData = new List<dynamic>();
            lstGeometryData = new List<dynamic>();
            webColumns = new List<WebGridColumn>();
            lstLayers = new List<layerReportDetail>();
            lstUserModule = new List<string>();
        }

    }



    public class ViewLandbaseAuditMasterModel
    {
        public int systemId { get; set; }

        public FilterLandBaseHistoryAttr objFilterAttributes { get; set; }

        public int landbase_layer_id{ get; set; }

        public List<dynamic> lstData { get; set; }
        public List<WebGridColumn> webColumns { get; set; }

        public ViewLandbaseAuditMasterModel()
        {
            objFilterAttributes = new FilterLandBaseHistoryAttr();
            lstData = new List<dynamic>();
            webColumns = new List<WebGridColumn>();
        }

    }
    public class FilterHistoryAttr : CommonGridAttributes
    {
        public int systemid { get; set; }
        public string entityType { get; set; }

    }
    #endregion History
    public class ViewAuditSiteInfo 
    {
        public int siteId { get; set; }

        public FilterSiteHistoryAttr objFilterAttributes { get; set; }

        public string lmcType { get; set; }
         
        public List<dynamic> lstData { get; set; }

        public ViewAuditSiteInfo() 
        {
            objFilterAttributes = new FilterSiteHistoryAttr();
            lstData = new List<dynamic>();
        }

    }
    public class ViewAuditLMCInfo
    {
        public int LMCId { get; set; }

        public FilterLMCHistoryAttr objFilterAttributes { get; set; }

        public string lmcType { get; set; }

        public List<dynamic> lstData { get; set; }

        public ViewAuditLMCInfo()
        {
            objFilterAttributes = new FilterLMCHistoryAttr();
            lstData = new List<dynamic>(); 
        }

    }
    public class ViewAuditAccessoriesInfo
    {
        public int accessories_id { get; set; }

        public FilterAccessoriesHistoryAttr objFilterAttributes { get; set; }

        public List<AccessoriesAuditModel> lstData { get; set; }

        public ViewAuditAccessoriesInfo()
        {
            objFilterAttributes = new FilterAccessoriesHistoryAttr();
            lstData = new List<AccessoriesAuditModel>();
        }

    }
    public class FilterSiteHistoryAttr : CommonGridAttributes
    {
        public int siteId { get; set; }
        public string lmcType { get; set; }
    }
    public class FilterLMCHistoryAttr : CommonGridAttributes
    {
        public int LMCId { get; set; }  
        public string lmcType { get; set; }
    }
    public class FilterAccessoriesHistoryAttr : CommonGridAttributes
    {
        public int accessories_id { get; set; }
        
    }
    public class ViewAuditFiberLinkInfo
    {
        public int LinkSystemId { get; set; }

        public FilterFiberLinkHistoryAttr objFilterAttributes { get; set; } 

        public List<dynamic> lstData { get; set; }

        public ViewAuditFiberLinkInfo()
        {
            objFilterAttributes = new FilterFiberLinkHistoryAttr();
            lstData = new List<dynamic>();
        }

    }
    public class FilterFiberLinkHistoryAttr : CommonGridAttributes
    {
        public int LinkSystemId { get; set; } 
    }

    //public class ViewAuditLandBaseInfo 
    //{
    //    public int id  { get; set; }

    //    public FilterLandBaseHistoryAttr objFilterAttributes { get; set; }

    //    public int landbase_layer_id { get; set; }

    //    public List<dynamic> lstData { get; set; }

    //    public ViewAuditLandBaseInfo()
    //    {
    //        objFilterAttributes = new FilterLandBaseHistoryAttr();
    //        lstData = new List<dynamic>();
    //    }

    //}
    public class FilterLandBaseHistoryAttr : CommonGridAttributes
    {
        public int id { get; set; }
        public int  landbase_layer_id { get; set; }
    }
}
