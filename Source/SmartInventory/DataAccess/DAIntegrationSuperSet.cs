using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;
using Models.Admin;

namespace DataAccess
{
    public class DAIntegrationSuperSet : Repository<IntegrationSuperSet>
    {
        public void SaveIntegrationSuperSet(List<IntegrationSuperSet> listObj)
        {
            try
            {
                repo.Insert(listObj);

            }
            catch { throw; }
        }

        /// <summary>
        /// Delete the plan data from integration_superset Table, if is_processed is "false"
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public List<Status> DeleteIntegrationSuperset(int planId)
        {
            try
            {
                var res = repo.ExecuteProcedure<Status>("fn_delete_Integration_superset", new { p_plan_id = planId });
                return res;
            }
            catch { throw; }
        }

        /// <summary>
        /// returns the unique plans
        /// </summary>
        /// <returns>List of DisplayPlan</returns>
        public List<DisplayPlan> GetUniquePlans()
        {
            try
            {
                var res = repo.ExecuteProcedure<DisplayPlan>("fn_get_integration_plans", null);
                return res;
            }
            catch { throw; }
        }

        /// <summary>
        /// Process the integration superset of given plan ID
        /// </summary>
        /// <param name="planId">Plan OD</param>
        /// <returns>Status</returns>
        public List<Status> ProcessIntegrationSuperset(int planId)
        {
            try
            {
                var res = repo.ExecuteProcedure<Status>("fn_process_integration_superset", new { p_plan_id = planId });
                return res;
            }
            catch { throw; }
        }

        /// <summary>
        /// Deletes the processed integration superset
        /// </summary>
        /// <param name="planId"> plan id</param>
        /// <returns>status</returns>
        public List<Status> DeleteProcessedIntegrationSuperset(int planId)
        {
            try
            {
                var res = repo.ExecuteProcedure<Status>("fn_delete_process_Integration_superset", new { p_plan_id = planId });
                return res;
            }
            catch { throw; }
        }

        /// <summary>
        /// returns the region, province, POD lat, lng, and layers
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        public PlanRegionProvince GetPlanRegionProvince(int planId)
        {
            try
            {
                PlanRegionProvince planRegionProvince = new PlanRegionProvince();
                var res = repo.ExecuteProcedure<PlanRegionProvince>("fn_get_plan_region_provinces", new { p_plan_id = planId });

                if (res.Count > 0)
                    planRegionProvince = res.ElementAt(0);

                return planRegionProvince;
            }
            catch { throw; }
        }

    }
    public class DAUpdateTargetDetails : Repository<object>

    {
        public void UpdateTargetDetails(int TargetRefID, string TargetRefCode, string DesignID, string ClassName, string CategoryName)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_update_target_details", new
                {
                    p_target_ref_id = TargetRefID,
                    p_target_ref_code = TargetRefCode,
                    p_design_id = DesignID,
                    p_class_name = ClassName,
                    p_category_name = CategoryName
                }).FirstOrDefault();
            }
            catch { throw; }

        }

    }
}
