using System.Collections.Generic;
using DataAccess.DBHelpers;
using Models;
using System.Linq;
namespace DataAccess
{
    public class DADynamicForm : Repository<DynamicControls>
    {
        DADynamicForm()
        {

        }

        private static DADynamicForm objDADynamicForm = null;
        private static readonly object lockObject = new object();
        public static DADynamicForm Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDADynamicForm == null)
                    {
                        objDADynamicForm = new DADynamicForm();
                    }
                }
                return objDADynamicForm;
            }
        }
        public DynamicControls Save(DynamicControls dynamicControls)
        {
            try
            {
                var res = repo.Insert(dynamicControls);
                return res;
            }
            catch { throw; }
        }
        public DynamicControls Update(DynamicControls dynamicControls)
        {
            try
            {
                var res = repo.Update(dynamicControls);
                return res;
            }
            catch { throw; }
        }
        public bool Duplicate(DynamicControls dynamicControls)
        {
            try
            {
                var res = repo.GetAll(m => m.entity_id == dynamicControls.entity_id &&
                m.field_label == dynamicControls.field_label &&
                m.default_value == dynamicControls.default_value &&
                m.placeholder_text == dynamicControls.placeholder_text &&
                m.min_length == dynamicControls.min_length &&
                m.max_length == dynamicControls.max_length &&
                m.is_mandatory == dynamicControls.is_mandatory &&
                m.control_type == dynamicControls.control_type &&
                m.control_value_type == dynamicControls.control_value_type &&
                m.round_off == dynamicControls.round_off).FirstOrDefault();

                if (res == null)
                    return false;
                else
                    return true;
            }
            catch { throw; }
        }
        public DynamicControls GetByID(int ID)
        {
            try
            {
                var res = repo.Get(m => m.id == ID);
                return res;
            }
            catch { throw; }
        }
        public List<DynamicControls> GetExistingFields(ExistingFieldFilter objFilter)
        {
            var lst = repo.ExecuteProcedure<DynamicControls>("fn_get_existing_fields", new
            {
                p_pagerecord = objFilter.pageSize,
                p_pageno = objFilter.page,
                p_entity_id = objFilter.layerId,
                p_userid = objFilter.userId
            }, true);
            return lst;
        }
        public DynamicControls GetExistingControl(int ControlID)
        {
            var lst = repo.Get(m => m.id == ControlID);
            return lst;
        }

        public List<DynamicControls> GetExistingFields(int layerId, int userId)
        {
            return (List<DynamicControls>)repo.GetAll(m => m.entity_id == layerId);
        }
        public void DeleteSingleField(string control_id, string layerTable, string fieldLabel, int entityid)
        {
            DynamicControls data = repo.GetAll(m => m.control_id == control_id || m.id.ToString() == control_id).FirstOrDefault();
            repo.ExecuteSQLCommand("update dynamic_controls set other_info = other_info - '" + fieldLabel + "' where entity_id=" + entityid + ";");
            repo.ExecuteSQLCommand("delete from dynamic_controls where (control_id='" + control_id + "' ) or (id::character varying='" + control_id + "') ");
            if (data.control_type == "DROPDOWN")
            {
                repo.ExecuteSQLCommand("delete from dynamic_controls_dropdown_master where control_id='" + data.id + "'");
            }
            repo.ExecuteSQLCommand("update " + layerTable + " set other_info = other_info - '" + fieldLabel + "' where other_info is not null");
        }
        public void UpdateSampleJson(string existingfieldLabel, string newfieldLabel, int controlID, string defaultvalue, int entity_id, string layerTable, string auditTable)
        {
            repo.ExecuteProcedure("fn_additional_attribute", new
            {
                p_existingfieldLabel = existingfieldLabel,
                p_newfieldLabel = newfieldLabel,
                p_layerTable = layerTable,
                p_fieldLabel = newfieldLabel,
                p_entityId = entity_id,
                p_auditTable = auditTable
            });
        }
        public void SaveSampleJson(int entity_id, string fieldLabel, int currentid)
        {
            repo.ExecuteSQLCommand("UPDATE dynamic_controls SET other_info = coalesce(other_info,'{}') || '{\"" + fieldLabel + "\": \" \"}'::jsonb WHERE entity_id = " + entity_id + "; ");
            repo.ExecuteSQLCommand("UPDATE dynamic_controls SET other_info = other_info || '{\"record_system_id\": \"0\"}'::jsonb");
            var result = repo.GetAll(x => x.id != currentid && x.entity_id == entity_id);
            if (result.Count() > 0)
            {
                var other_info = result.FirstOrDefault().other_info;
                repo.ExecuteSQLCommand("update dynamic_controls set other_info = '" + other_info + "' where id=" + currentid + ";");
            }
        }
        public void CreateDynamicView(string entityName)
        {
            repo.ExecuteProcedure("fn_refresh_vw_report_oi", new { p_entityName = entityName });
        }
        public void SyncViewColumns()
        {
            repo.ExecuteProcedure("fn_sync_layer_columns", new { });
        }

    }
}
