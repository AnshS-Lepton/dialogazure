using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
using Models.Admin;

namespace BusinessLogics
{
    public class BLIntegrationSuperSet
    {
        public void SaveIntegrationSuperSet(List<IntegrationSuperSet> listObjintegrationSuperset)
        {
            new DAIntegrationSuperSet().SaveIntegrationSuperSet(listObjintegrationSuperset);
        }

        public List<Status> DeleteIntegrationSuperset(int planId)
        {
            return new DAIntegrationSuperSet().DeleteIntegrationSuperset(planId);
        }

        public List<DisplayPlan> GetUniquePlans()
        {
            return new DAIntegrationSuperSet().GetUniquePlans();
        }

        public List<Status> ProcessIntegrationSuperset(int planId)
        {
            return new DAIntegrationSuperSet().ProcessIntegrationSuperset(planId);
        }

        public List<Status> DeleteProcessedIntegrationSuperset(int planId)
        {
            try
            {
                return new DAIntegrationSuperSet().DeleteProcessedIntegrationSuperset(planId);
            }
            catch { throw; }
        }

        public PlanRegionProvince GetPlanRegionProvince(int planID)
        {
            try
            {
                return new DAIntegrationSuperSet().GetPlanRegionProvince(planID);
            }
            catch { throw; }
        }
        public void BLUpdateTargetDetails(int TargetRefID, string TargetRefCode, string DesignID, string ClassName, string CategoryName)
        {
            new DAUpdateTargetDetails().UpdateTargetDetails(TargetRefID, TargetRefCode, DesignID, ClassName, CategoryName);
        }
    }
}
