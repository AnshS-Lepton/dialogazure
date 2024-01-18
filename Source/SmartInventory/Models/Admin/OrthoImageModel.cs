using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Admin
{
    public class OrthoImageModel
    {
        
        public FilterOrthoImageAttr objFilterAttributes { get; set; }
        public List<OrthoImageMasterModel> listDatas { get; set; }
        public OrthoImageModel()
        {
            objFilterAttributes = new FilterOrthoImageAttr();
            listDatas = new List<OrthoImageMasterModel>();
        }
    }
    public class FilterOrthoImageAttr : CommonGridAttributes
    {
        public int status { get; set; }

    }
    public class OrthoImageMasterModel {
        [Key]
        public int system_id { get; set; }
        public string image_name { get; set; }
        public string image_extension { get; set; } = "tif";
        public string display_name { get; set; }
        public bool is_active { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public OrthoImageMasterModel()
        {
            pageMsg = new PageMessage();
        }
    }
}
