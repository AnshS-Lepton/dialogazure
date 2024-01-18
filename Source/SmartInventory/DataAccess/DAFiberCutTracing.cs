using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAFiberCutTracing : Repository<FiberPath>
    {
        public List<FiberPathGeom> getTracingPath(int systemId, string entityType, int portNo, string nodeType)
        {
            try
            {
                return repo.ExecuteProcedure<FiberPathGeom>("fn_otdr_get_fiber_path_geom", new { p_entity_system_id = systemId, p_entity_port_no = portNo, p_entity_type = entityType, p_node_type = nodeType });
            }
            catch { throw; }
        }
        public FiberCutDetails getFiberCutDetails(int systemId, string entityType, int portNo, double distance, string nodeType,bool isBackWordPath)
        {
            try
            {
                return repo.ExecuteProcedure<FiberCutDetails>("fn_otdr_get_fiber_cut_details", new { p_entity_system_id = systemId, p_entity_port_no = portNo, p_entity_type = entityType, p_distance = distance, p_node_type = nodeType, p_is_backword_path= isBackWordPath }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<FiberNodes> getFiberNodeType(int systemid, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<FiberNodes>("fn_otdr_get_fiber_node_type", new { p_system_id = systemid, p_entity_type = entityType }, true);
            }
            catch { throw; }
        }
    }
}
