using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AdditionalAttributeModel
    {
        public List<layerDetail> layers { get; set; }
        public string LayerName { get; set; }
        public int TotalRecords { get; set; }
        public List<DynamicControls> dynamicControls { get; set; }
    }
    public class ExistingFieldFilter
    {
        public int layerId { get; set; }
        public int pageSize { get; set; }
        public int page { get; set; }
        public int totalRecords { get; set; }
        public int userId { get; set; }
    }
}
