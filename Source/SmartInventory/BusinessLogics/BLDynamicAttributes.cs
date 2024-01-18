using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLDynamicAttributes
    {
        public List<DynamicControls> GetDynanicControlsById(int layer_id)
        {
            return DADynamicAttributes.Instance.GetDynanicControlsById(layer_id);
        }

        public List<DynamicControlsDDLMaster> GetDDLByControlsId(int[] controlIds)
        {
            return DAControlsDDLMaster.Instance.GetDDLByControlsId(controlIds);
        }
        public DataTable GetDynamicFormStyle()
        {
            return DADynamicAttributes.Instance.GetDynamicFormStyle();
        }
        public DynamicControlsDDLMaster Save(DynamicControlsDDLMaster dynamicControlsDDLMaster)
        {
            return DAControlsDDLMaster.Instance.Save(dynamicControlsDDLMaster);
        }
        public void Save(List<DynamicControlsDDLMaster> dynamicControlsDDLMaster)
        {
            DAControlsDDLMaster.Instance.Save(dynamicControlsDDLMaster);
        }
        public List<DynamicControlsDDLMaster> GetExisitingFieldsforDDL(int id)
        {
            return DAControlsDDLMaster.Instance.GetExisitingFieldsforDDL(id);
        }
        public void Update(List<DynamicControlsDDLMaster> dynamicControlsDDLMaster)
        {
            DAControlsDDLMaster.Instance.Update(dynamicControlsDDLMaster);
        }
        public void DeleteExistingDDL(int id)
        {
            DAControlsDDLMaster.Instance.DeleteExistingDDL(id);
        }

    }
}
