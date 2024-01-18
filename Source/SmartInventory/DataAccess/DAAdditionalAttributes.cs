using DataAccess.DBHelpers;
using DataAccess.ISP;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAAdditionalAttributes : Repository<AdditionalAttributes>
    {
        public AdditionalAttributes SaveAttributes(AdditionalAttributes objAttributes)
        {
            try
            {
                if (!string.IsNullOrEmpty(objAttributes.rfsSetDate))
                    objAttributes.rfs_date = Convert.ToDateTime(objAttributes.rfsSetDate);
                var attributes = repo.GetAll(m => m.system_id == objAttributes.system_id && m.entity_type == objAttributes.entity_type).FirstOrDefault();
                if (attributes != null)
                {
                    attributes.erp_code = objAttributes.erp_code;
                    attributes.erp_name = objAttributes.erp_name;
                    attributes.hm_earthing_rating = objAttributes.hm_earthing_rating;
                    attributes.hm_fms = objAttributes.hm_fms;
                    attributes.hm_olt_bb = objAttributes.hm_olt_bb;
                    attributes.hm_power_bb = objAttributes.hm_power_bb;
                    attributes.hm_rack = objAttributes.hm_rack;
                    attributes.hub_maintained = objAttributes.hub_maintained;
                    attributes.l3_updation_on_inms = objAttributes.l3_updation_on_inms;
                    attributes.laptop_with_giga_port = objAttributes.laptop_with_giga_port;
                    attributes.optical_power_meter = objAttributes.optical_power_meter;
                    attributes.otdr = objAttributes.otdr;
                    attributes.rfs_date = Convert.ToDateTime(objAttributes.rfsSetDate);
                    attributes.splicing_machine = objAttributes.splicing_machine;
                    attributes.status = objAttributes.splicing_machine;
                    attributes.zone = objAttributes.zone;
                    attributes.ef_customers = objAttributes.ef_customers;
                    return repo.Update(attributes);
                }
                else { return repo.Insert(objAttributes); }
            }
            catch
            {
                throw;
            }
        }
        public AdditionalAttributes getAttributes(int systemId, string entityType)
        {
            try
            {
                return repo.GetAll(m => m.system_id == systemId && m.entity_type == entityType).FirstOrDefault();
            }
            catch { throw; }
        }
    }
}
