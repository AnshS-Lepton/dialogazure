using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Admin
{
    public class DACBomLogic : Repository<ConstructionOhLogicMaster>
    {
        public int Saveconstructionbomlogic(string formula, int fsa_id, string formula1, string column_name, string oh_expression_json)
        {

            try
            {
                var res = repo.ExecuteProcedure<int>("fn_construction_bom_details_save", new
                {
                    formula = formula,
                    fsa_id = fsa_id,
                    formula1 = formula1,
                    column_name = column_name,
                    oh_expression_json = oh_expression_json

                });
                return res[0];
            }
            catch { throw; }


        }

        public int Savedefaultoverhead(int fsa_id, string defaultvalue, string column_name)
        {

            try
            {
                var res = repo.ExecuteProcedure<int>("fn_construction_bom_details_savedefaultoverhead", new
                {
                    // formula = formula,
                    fsa_id = fsa_id,
                    defaultvalue = defaultvalue,
                    column_name = column_name

                });
                return res[0];
            }
            catch { throw; }


        }


        public bool saveCBomLogic(ConstructionOhLogicMaster objLayerColumnSettings)
        {
            try
            {



                var itemToChange = repo.Get(d => d.id == objLayerColumnSettings.id);
                if (itemToChange != null)
                {
                    itemToChange.oh_display_formula = objLayerColumnSettings.oh_display_formula;
                    itemToChange.oh_logic = objLayerColumnSettings.oh_logic;

                }

                repo.Update(itemToChange);
                return true;
            }
            catch { throw; }
        }
        public bool UpdateCBOMLogic(string defaultoverhead, string columnname)
        {
            try
            {



                var itemToChange = repo.Get(d => d.column_name == columnname);
                if (itemToChange != null)
                {
                    itemToChange.default_overhead = defaultoverhead;


                }

                repo.Update(itemToChange);
                return true;
            }
            catch { throw; }
        }

        public ConstructionOhLogicMaster GetCBOMLogic(string column_name, int fsa_id)
        {
           var res = repo.ExecuteProcedure<ConstructionOhLogicMaster>("fn_construction_bom_logic", new { p_columnName= column_name, p_fsaId=fsa_id }, true).FirstOrDefault();
            return res;
        }
        public ConstructionOhLogicMaster GetCBOMLogicbycolumn(string column_name)
        {
            return repo.Get(m => m.column_name == column_name);
        }

        // public ConstructionOhLogicMaster GetCBOMLogic(int id)
        // {
        //    return repo.Get(m => m.id == id);
        //}

        public List<ConstructionOhLogicMaster> GetAllCBomLogic()
        {
            return repo.GetAll().ToList();
        }
        public class DABomscedular : Repository<SchedulerLog>
        {
            public SchedulerLog getschedulardate()
            {

                return repo.GetAll().OrderByDescending(c => c.id).FirstOrDefault();
                // return new DACBomLogic().getSchedulerLogdate();
            }
        }
        public List<ConstructionOhLogicMaster> GetVariables()
        {
            var res = repo.ExecuteProcedure<ConstructionOhLogicMaster>("fn_construction_get_variables", new { },true).ToList();
            return res;
        }
    }
}
