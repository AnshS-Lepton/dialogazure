using Models;
using Models.Admin;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartInventory.ViewModel
{
    interface IScaleConvertor
    {
        double MMToPixel(double mm);
        double PixelToMM(double val);

    }
    public class RoomViewModel : IScaleConvertor
    {
        // double _height;
        // double _width;
        // double _depth;
        //double _border_width;
        public int id { get; set; }
        public int db_id { get; set; }
        public int model_id { get; set; }
        public string name { get; set; }
        public string model_name { get; set; }
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
        public string model_type { get; set; }
        public int model_view_id { get; set; }
        public bool is_editable { get; set; }
        public int no_of_units { get; set; }
        public int? port_number { get; set; }
        public int super_parent { get; set; }
        public string  network_id { get; set; }
        public string short_network_id { get; set; }
        public int lib_id { get; set; }
        public string network_status { get; set; }
        public bool is_internal_connectivity_enabled { get; set; }
        public RoomViewModel()
        {
            position = new Position() { x = 0, y = 0 };
        }

        public double MMToPixel(double mm)
        {
            double pixel = 0;
            pixel = mm * ApplicationSettings.RoomCellSize / ApplicationSettings.RoomScale;
            return pixel;
        }

        public double PixelToMM(double val)
        {
            double mm = 0;
            mm = val * ApplicationSettings.RoomScale / ApplicationSettings.RoomCellSize;
            return mm;
        }

        public double MeterToPixel(double val)
        {
            double mm = 0;
            mm = val * 1000 * ApplicationSettings.RoomCellSize / ApplicationSettings.RoomScale;
            return mm;
        }

        public double MMToPixel_Equipment(double mm)
        {
            double pixel = 0;
            pixel = mm * ApplicationSettings.WorkspaceCellSize / ApplicationSettings.WorkspaceScale;
            return pixel;
        }

        public string font_size { get; set; }
        public string font_color { get; set; }
        public string text_orientation { get; set; }
        public string bg_image { get; set; }
        public bool is_multi_connection { get; set; }
        public bool is_view_enabled { get; set; }
        public string port_status { get; set; }
        public int? port_status_id { get; set; }
        public string port_comment { get; set; }
        public string port_status_color { get; set; }
        public int? rack_id { get; set; }
        public string border_color { get; set; }
        public List<RoomViewModel> ConvertFromModelInfo(List<ISPModelInfo> input, bool isEditable)
        {
            List<RoomViewModel> result = new List<RoomViewModel>();
            foreach (var child in input)
            {
                result.Add(new RoomViewModel
                {
                    id = child.map_id,
                    db_id = child.id,
                    model_id = child.model_id,
                    name = child.alt_name ?? child.model_name,
                    image_data = child.image_data,
                    img_id = child.model_image_id,
                    height = MMToPixel_Equipment(child.height),
                    width = MMToPixel_Equipment(child.width),
                    depth = MMToPixel_Equipment(child.depth),
                    is_static = true,
                    offset_x = 0,
                    offset_y = 0,
                    color = child.color_code,
                    stroke = child.stroke_code,
                    parent = child.parent,
                    position = new Position
                    {
                        x = MMToPixel_Equipment(child.child_x_pos),
                        y = MMToPixel_Equipment(child.child_y_pos)
                    },
                    rotation_angle = child.rotation_angle,
                    model_type_id = child.model_type_id,
                    model_view_id = child.model_view_id,
                    border_width = MMToPixel_Equipment(child.border_width),
                    is_editable = isEditable,
                    db_border_width = child.border_width,
                    db_depth = child.depth,
                    db_height = child.height,
                    db_width = child.width,
                    font_color = child.font_color,
                    font_size = child.font_size,
                    text_orientation = child.text_orientation,
                    bg_image = child.background_image,
                    is_multi_connection = child.is_multi_connection,
                    is_view_enabled = child.is_view_enabled,
                    border_color = child.border_color
                });
            }
            return result;
        }

        public List<RoomViewModel> ConvertFromSpecification(List<VendorSpecificationMaster> input, bool isEditable)
        {
            List<RoomViewModel> result = new List<RoomViewModel>();
            foreach (var child in input)
            {
                result.Add(new RoomViewModel
                {
                    id = child.id,
                    db_id = child.id,
                    model_id = 0,
                    model_name = child.specification,
                    image_data = null,
                    img_id = null,
                    height = (child.length) ?? 0,
                    width = (child.width) ?? 0,
                    depth = (child.height) ?? 0,
                    is_static = false,
                    offset_x = 0,
                    offset_y = 0,
                    color = "green",
                    stroke = "black",
                    parent = -1,
                    position = new Position
                    {
                        x = 0,
                        y = 0
                    },
                    rotation_angle = 0,
                    model_type_id = 0,
                    model_view_id = 0,
                    border_width = (child.border_width) ?? 0,
                    is_editable = isEditable,
                    db_border_width = (child.border_width) ?? 0,
                    db_depth = (child.height) ?? 0,
                    db_height = (child.length) ?? 0,
                    db_width = (child.width) ?? 0,
                    no_of_units = child.no_of_units ?? 0,
                    border_color=child.border_color
                });
            }
            return result;
        }
        public List<RoomViewModel> ConvertFromEquipmentInfo(List<EquipmentInfo> input, bool isEditable)
        {
            List<RoomViewModel> result = new List<RoomViewModel>();
            foreach (var child in input)
            {
                result.Add(new RoomViewModel
                {
                    id = child.system_id,
                    db_id = child.system_id,
                    model_id = child.model_id,
                    name = child.model_name,
                    image_data = child.image_data,
                    img_id = child.model_image_id,
                    height = MMToPixel_Equipment(child.height),
                    width = MMToPixel_Equipment(child.width),
                    depth = MMToPixel_Equipment(child.length),
                    is_static = true,
                    offset_x = 0,
                    offset_y = 0,
                    color = child.color_code,
                    stroke = child.stroke_code,
                    parent = child.parent_system_id,
                    position = new Position
                    {
                        x = MMToPixel_Equipment(child.pos_x),
                        y = MMToPixel_Equipment(child.pos_y)
                    },
                    rotation_angle = child.rotation_angle,
                    model_type_id = child.model_type_id,
                    model_view_id = child.model_view_id,
                    border_width = MMToPixel_Equipment(child.border_width),
                    is_editable = child.isEditable | isEditable,
                    db_border_width = child.border_width,
                    db_depth = child.length,
                    db_height = child.height,
                    db_width = child.width,
                    lib_id = child.model_info_id,
                    network_status = child.network_status,
                    font_color = child.font_color,
                    font_size = child.font_size,
                    text_orientation = child.text_orientation,
                    bg_image = child.background_image,
                    is_multi_connection = child.is_multi_connection,
                    is_view_enabled = child.is_view_enabled,
                    port_status = child.port_status,
                    port_status_color = child.port_status_color,
                    port_status_id=child.port_status_id,
                    rack_id=child.rack_id,
                    border_color=child.border_color,
                    port_number = child.port_number,
                    super_parent = child.super_parent,
                    network_id = child.network_id,
                    model_name = child.model_name,
                    port_comment = child.port_comment,
                    short_network_id = child.short_network_id,
                    is_internal_connectivity_enabled=child.is_internal_connectivity_enabled
                });
            }
            return result;
        }
    }
    public class Position : IScaleConvertor
    {
        //double _x;
        //double _y;
        public double x { get; set; }

        public double y { get; set; }


        public double MMToPixel(double mm)
        {
            double pixel = 0;
            pixel = mm * ApplicationSettings.RoomCellSize / ApplicationSettings.RoomScale;
            return pixel;
        }

        public double PixelToMM(double val)
        {
            double mm = 0;
            mm = val * ApplicationSettings.RoomScale / ApplicationSettings.RoomCellSize;
            return mm;
        }
    }
}