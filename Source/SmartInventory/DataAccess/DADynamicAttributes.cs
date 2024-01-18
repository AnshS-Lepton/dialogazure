using System;
using System.Collections.Generic;
using Models;
using Models.Admin;
using DataAccess.DBHelpers;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data;

namespace DataAccess
{
    public class DADynamicAttributes : Repository<DynamicControls>
    {
        private static DADynamicAttributes objDynamicAttributes = null;
        private static readonly object lockObject = new object();
        public static DADynamicAttributes Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDynamicAttributes == null)
                    {
                        objDynamicAttributes = new DADynamicAttributes();
                    }
                }
                return objDynamicAttributes;
            }
        }

        public DataTable GetDynamicFormStyle()
        {
            DataTable dt = repo.GetDataTable("select * from dynamic_form_styles");
            return dt;
        }
        public List<DynamicControls> GetDynanicControlsById(int layer_id)
        {
            try
            {
                return repo.GetAll(x => x.entity_id == layer_id && x.is_visible == true).OrderBy(x => x.field_order).Select(x => new DynamicControls
                {
                    cal_id = "img_" + x.field_name + "_cal",
                    required_text = x.field_name + "_is_required.",
                    id = x.id,
                    entity_id = x.entity_id,
                    field_label = x.field_label,
                    field_name = x.field_name,
                    control_type = x.control_type,
                    control_value_type = x.control_value_type,
                    is_mandatory = x.is_mandatory,
                    is_visible = x.is_visible,
                    min_length = x.min_length,
                    max_length = x.max_length,
                    format = x.format,
                    default_value = x.default_value,
                    placeholder_text = x.placeholder_text,
                    control_css_class = x.control_css_class,
                    field_order = x.field_order,
                    created_by = x.created_by,
                    created_on = x.created_on,
                    modified_by = x.modified_by,
                    modified_on = x.modified_on,
                    label_css_class = x.label_css_class

                }).ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }


    public class DAControlsDDLMaster : Repository<DynamicControlsDDLMaster>
    {
        private static DAControlsDDLMaster objControlsDDLMaster = null;
        private static readonly object lockObject = new object();
        public static DAControlsDDLMaster Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objControlsDDLMaster == null)
                    {
                        objControlsDDLMaster = new DAControlsDDLMaster();
                    }
                }
                return objControlsDDLMaster;
            }
        }
        public List<DynamicControlsDDLMaster> GetDDLByControlsId(int[] controlIds)
        {
            try
            {
                return repo.GetAll(s => controlIds.Contains(s.control_id) && s.isvisible == true).ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public DynamicControlsDDLMaster Save(DynamicControlsDDLMaster dynamicControlsDDLMaster)
        {
            return repo.Insert(dynamicControlsDDLMaster);

        }
        public void Save(List<DynamicControlsDDLMaster> dynamicControlsDDLMaster)
        {
            repo.Insert(dynamicControlsDDLMaster);

        }
        public void Update(List<DynamicControlsDDLMaster> dynamicControlsDDLMaster)
        {
            repo.Update(dynamicControlsDDLMaster);

        }
        public List<DynamicControlsDDLMaster> GetExisitingFieldsforDDL(int id)
        {
            return repo.GetAll(m => m.control_id == id).ToList();

        }
        public void DeleteExistingDDL(int id)
        {
            repo.ExecuteSQLCommand("delete from dynamic_controls_dropdown_master where control_id=" + id);

        }
    }
}
