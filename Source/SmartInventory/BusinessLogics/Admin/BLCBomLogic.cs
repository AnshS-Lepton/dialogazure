using DataAccess;
using DataAccess.Admin;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Admin.DACBomLogic;

namespace BusinessLogics.Admin
{
    public class BLCBomLogic
    {

        public int Savebomlogic(string formula, int fsa_id, string formula1, string column_name, string oh_expression_json)
        {
            return new DACBomLogic().Saveconstructionbomlogic(formula, fsa_id, formula1, column_name, oh_expression_json);
        }

        public bool UpdateCBOMLogic(string defalutoverhead, string columnname)
        {
            return new DACBomLogic().UpdateCBOMLogic(defalutoverhead, columnname);
        }
        public int Savedefaultoverhead(int fsa_id, string defaultvalue, string column_name)
        {
            return new DACBomLogic().Savedefaultoverhead(fsa_id, defaultvalue, column_name);
        }
        public List<ConstructionOhLogicMaster> GetAllCBOMLogic()
        {
            return new DACBomLogic().GetAllCBomLogic();
        }
        public SchedulerLog getschedulardate()
        {
            return new DABomscedular().getschedulardate();
        }
        public ConstructionOhLogicMaster GetCBOMLogic(string column_name, int fsa_id)
        {
            return new DACBomLogic().GetCBOMLogic(column_name, fsa_id);
        }
        public bool SaveCBOMLogic(ConstructionOhLogicMaster objLayerColumnSettings)
        {
            return new DACBomLogic().saveCBomLogic(objLayerColumnSettings);
        }
        public ConstructionOhLogicMaster GetCBOMLogicbycolumn(string column_name)
        {
            return new DACBomLogic().GetCBOMLogicbycolumn(column_name);
        }
        public List<ConstructionOhLogicMaster> GetVariables()
        {
            return new DACBomLogic().GetVariables();
        }
    }

     
}
