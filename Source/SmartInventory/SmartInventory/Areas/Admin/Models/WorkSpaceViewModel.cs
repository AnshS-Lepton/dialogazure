using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartInventory.Areas.Admin.Models
{
    public class WorkSpaceViewModel
    {
        public int id { get; set; }
        public int db_id { get; set; }
        public int model_id { get; set; }
        public string name { get; set; }
        public Position position { get; set; }
        public int parent { get; set; }
        public string color { get; set; }
        public int? img_id { get; set; }
        public string stroke { get; set; }
        public double height { get; set; }
        public double width { get; set; }
        public double depth { get; set; }

        public double border_width { get; set; }
        public double db_height { get; set; }
        public double db_width { get; set; }
        public double db_depth { get; set; }
        public double db_border_width { get; set; }
        public int offset_x { get; set; }
        public int offset_y { get; set; }
        public string image_data { get; set; }
        public bool is_static { get; set; }
        public double rotation_angle { get; set; }
        public int? model_type_id { get; set; }
        public int model_view_id { get; set; }
        public bool is_editable { get; set; }
        public int db_parent { get; set; }
        public int ref_parent { get; set; }
        public int ref_id { get; set; }
        public string font_size { get; set; }
        public string font_color { get; set; }
        public string text_orientation { get; set; }
        public string bg_image { get; set; }
       
        public int? model_color_id { get; set; }
        
        public string font_weight { get; set; }
        public string border_color { get; set; }
        public bool isNewEquipment { get; set; }
        public WorkSpaceViewModel()
        {
            position = new Position() { x = 0, y = 0 };
        }
    }
    public class Position
    {
        public double x { get; set; }
        public double y { get; set; }
    }
}