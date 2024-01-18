using System.Collections.Generic;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLDynamicsForm
    {
        public DynamicControls Save(DynamicControls dynamicControls)
        {
            return DADynamicForm.Instance.Save(dynamicControls);
        }
        public DynamicControls Update(DynamicControls dynamicControls)
        {
            return DADynamicForm.Instance.Update(dynamicControls);
        }
        public bool Duplicate(DynamicControls dynamicControls)
        {
            return DADynamicForm.Instance.Duplicate(dynamicControls);
        }
        public DynamicControls GetByID(int ID)
        {
            return DADynamicForm.Instance.GetByID(ID);
        }
        public List<DynamicControls> GetExistingFields(ExistingFieldFilter filter)
        {
            return DADynamicForm.Instance.GetExistingFields(filter);
        }
        public List<DynamicControls> GetExistingFields(int layerId,int userId)
        {
            return DADynamicForm.Instance.GetExistingFields(layerId,userId);
        }
        public DynamicControls GetExistingControl(int controlID)
        {
            return DADynamicForm.Instance.GetExistingControl(controlID);
        }
        public void DeleteSingleField(string control_id,string layerTable,string fieldLabel,int entityid)
        {
            DADynamicForm.Instance.DeleteSingleField(control_id,layerTable, fieldLabel, entityid);
        }
        public void UpdateSampleJson(string existingfieldLabel,string newfieldLabel,int controlID,string defaultvalue,int entity_id,string layertable,string auditTable)
        {
            DADynamicForm.Instance.UpdateSampleJson(existingfieldLabel, newfieldLabel, controlID, defaultvalue, entity_id, layertable, auditTable);
        }
        public void SaveSampleJson(int entity_id, string fieldLabel,int currentid )
        {
            DADynamicForm.Instance.SaveSampleJson(entity_id, fieldLabel, currentid);
        }
        public void CreateDynamicView(string entityName)
        {
            DADynamicForm.Instance.CreateDynamicView(entityName);
        }
        public void SyncViewColumns()
        {
            DADynamicForm.Instance.SyncViewColumns();
        }
    }
}
