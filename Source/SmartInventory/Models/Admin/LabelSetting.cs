using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Admin
{
    public class LabelSetting
    {

        [Key]
        public int id { get; set; }
        public int layer_id { get; set; }
        public string label_columns { get; set; }
        [NotMapped]
        public List<layerDetail> lstLayers { get; set; }
        //[NotMapped]
        //public List<AttributeDetail> lstLabelAttributes { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int user_id { get; set; }

        [NotMapped]
        public List<AttributeDetail> lstLabelAttributes { get; set; }

        public LabelSetting()
        {
            lstLayers = new List<layerDetail>();
            lstLabelAttributes = new List<AttributeDetail>();
            pageMsg = new PageMessage();
            label_columns = string.Empty;
        }



    }



}