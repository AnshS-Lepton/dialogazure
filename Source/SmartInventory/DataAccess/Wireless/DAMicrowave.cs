using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
  public  class DAMicrowave : Repository<MicrowaveLinkMaster>
    {
        public MicrowaveLinkMaster Save(MicrowaveLinkMaster objMicrowaveLinkMaster, int userId)
        {
            try
            {
                var resultItem = new MicrowaveLinkMaster();
                var objMicrowaveLink = repo.Get(x => x.system_id == objMicrowaveLinkMaster.system_id);
                if (objMicrowaveLink != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objMicrowaveLinkMaster.modified_on, objMicrowaveLink.modified_on, objMicrowaveLinkMaster.modified_by, objMicrowaveLink.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objMicrowaveLinkMaster.objPM = objPageValidate;
                        return objMicrowaveLinkMaster;
                    }
                    objMicrowaveLink.network_name = objMicrowaveLinkMaster.network_name;
                     objMicrowaveLink.accessibility = objMicrowaveLinkMaster.accessibility;
                    
                    objMicrowaveLink.category = objMicrowaveLinkMaster.category;
                   
                    objMicrowaveLink.modified_by = userId;
                    objMicrowaveLink.modified_on = DateTimeHelper.Now;
                     objMicrowaveLink.project_id = objMicrowaveLinkMaster.project_id ?? 0;
                    objMicrowaveLink.planning_id = objMicrowaveLinkMaster.planning_id ?? 0;
                    objMicrowaveLink.workorder_id = objMicrowaveLinkMaster.workorder_id ?? 0;
                    objMicrowaveLink.purpose_id = objMicrowaveLinkMaster.purpose_id ?? 0;
                    objMicrowaveLink.item_code = objMicrowaveLinkMaster.item_code;
                    objMicrowaveLink.network_name = objMicrowaveLinkMaster.network_name;
                    objMicrowaveLink.network_status = objMicrowaveLinkMaster.network_status;
					objMicrowaveLink.service_id = objMicrowaveLinkMaster.service_id;
					objMicrowaveLink.link_type = objMicrowaveLinkMaster.link_type;
					objMicrowaveLink.link_name = objMicrowaveLinkMaster.link_name;
					objMicrowaveLink.ownership_type=objMicrowaveLinkMaster.ownership_type;
					objMicrowaveLink.hierarchy_type = objMicrowaveLinkMaster.hierarchy_type;
					objMicrowaveLink.free_capacity = objMicrowaveLinkMaster.free_capacity;
					objMicrowaveLink.total_capacity = objMicrowaveLinkMaster.total_capacity;
					objMicrowaveLink.specification = objMicrowaveLinkMaster.specification;
                    objMicrowaveLink.subcategory1 = objMicrowaveLinkMaster.subcategory1;
                    objMicrowaveLink.subcategory2 = objMicrowaveLinkMaster.subcategory2;
                    objMicrowaveLink.subcategory3= objMicrowaveLinkMaster.subcategory3;
					objMicrowaveLink.third_party_vendor_id = objMicrowaveLinkMaster.third_party_vendor_id;

					objMicrowaveLink.workorder_id = objMicrowaveLinkMaster.workorder_id;
                    objMicrowaveLink.vendor_id = objMicrowaveLinkMaster.vendor_id;

					objMicrowaveLink.installation = objMicrowaveLinkMaster.installation;
					objMicrowaveLink.installation_company = objMicrowaveLinkMaster.installation_company;
					objMicrowaveLink.installation_number = objMicrowaveLinkMaster.installation_number;
					objMicrowaveLink.installation_technician = objMicrowaveLinkMaster.installation_technician;
					objMicrowaveLink.installation_year = objMicrowaveLinkMaster.installation_year;
					objMicrowaveLink.production_year = objMicrowaveLinkMaster.production_year;
                    objMicrowaveLink.status_remark = objMicrowaveLinkMaster.status_remark;
                    objMicrowaveLink.main_link_type = objMicrowaveLinkMaster.main_link_type;
                    objMicrowaveLink.main_link_id = objMicrowaveLinkMaster.main_link_id;
                    objMicrowaveLink.redundant_link_type = objMicrowaveLinkMaster.redundant_link_type;
                    objMicrowaveLink.redundant_link_id = objMicrowaveLinkMaster.redundant_link_id;
                    objMicrowaveLink.min_frequency_received = objMicrowaveLinkMaster.min_frequency_received;
                    objMicrowaveLink.max_frequency_received = objMicrowaveLinkMaster.max_frequency_received;
                    objMicrowaveLink.min_frequency_transmitted = objMicrowaveLinkMaster.min_frequency_transmitted;
                    objMicrowaveLink.odu_type = objMicrowaveLinkMaster.odu_type;
                    objMicrowaveLink.max_frequency_transmitted = objMicrowaveLinkMaster.max_frequency_transmitted;
                    objMicrowaveLink.nms_ip = objMicrowaveLinkMaster.nms_ip;
                    objMicrowaveLink.user_name = objMicrowaveLinkMaster.user_name;
                    objMicrowaveLink.password = objMicrowaveLinkMaster.password;
                    objMicrowaveLink.manufacturer_name = objMicrowaveLinkMaster.manufacturer_name;
                    objMicrowaveLink.model_number = objMicrowaveLinkMaster.model_number;
                    objMicrowaveLink.license_number = objMicrowaveLinkMaster.license_number;
                    objMicrowaveLink.idu_transmit_power = objMicrowaveLinkMaster.idu_transmit_power;
                    objMicrowaveLink.modulation = objMicrowaveLinkMaster.modulation;
                    objMicrowaveLink.idu_transmitted_frequency = objMicrowaveLinkMaster.idu_transmitted_frequency;
                    objMicrowaveLink.idu_received_frequency = objMicrowaveLinkMaster.idu_received_frequency;
                    objMicrowaveLink.bandwidth = objMicrowaveLinkMaster.bandwidth;
                    objMicrowaveLink.polarization = objMicrowaveLinkMaster.polarization;
                    objMicrowaveLink.requested_by = objMicrowaveLinkMaster.requested_by;
                    objMicrowaveLink.request_approved_by = objMicrowaveLinkMaster.request_approved_by;
                    objMicrowaveLink.request_ref_id = objMicrowaveLinkMaster.request_ref_id;
                    objMicrowaveLink.origin_ref_id = objMicrowaveLinkMaster.origin_ref_id;
                    objMicrowaveLink.origin_ref_description = objMicrowaveLinkMaster.origin_ref_description;
                    objMicrowaveLink.origin_from = objMicrowaveLinkMaster.origin_from;
                    objMicrowaveLink.origin_ref_code = objMicrowaveLinkMaster.origin_ref_code;
                    objMicrowaveLink.bom_sub_category = objMicrowaveLinkMaster.bom_sub_category;
                    // objMicrowaveLink.served_by_ring= objMicrowaveLinkMaster.served_by_ring;

                    resultItem = repo.Update(objMicrowaveLink);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.MicrowaveLink.ToString(), resultItem.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.MicrowaveLink.ToString(), resultItem.province_id);

                }
                else
                {
                    objMicrowaveLinkMaster.created_by = userId;
                    objMicrowaveLinkMaster.created_on = DateTimeHelper.Now;
                    objMicrowaveLinkMaster.status = "A";
                    objMicrowaveLinkMaster.network_status = "P";
                    resultItem = repo.Insert(objMicrowaveLinkMaster);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
					geom.longLat = objMicrowaveLinkMaster.geom;
					geom.userId = userId;
                    geom.entityType = EntityType.MicrowaveLink.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Line.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.MicrowaveLink.ToString(), resultItem.province_id, 0);
                   // DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.MicrowaveLink.ToString(), resultItem.province_id);

                }


                return resultItem;
            }
            catch(Exception ex) 
            { throw ex; }
        }
        public int DeleteMicrowaveLinkById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }
        public MicrowaveLinkMaster getMicrowaveLinkDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
    }

    public class DAMicrowavelinkFeeder : Repository<MicrowavelinkFeederSystem>
    {
        public MicrowavelinkFeederSystem Save(MicrowavelinkFeederSystem objMicrowaveLinkFeeder, int userId)
        {
            try
            {
                var resultItem = new MicrowavelinkFeederSystem();
                var objMicrowaveFeeder = repo.Get(x => x.system_id == objMicrowaveLinkFeeder.system_id);
                if (objMicrowaveFeeder != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objMicrowaveLinkFeeder.modified_on, objMicrowaveFeeder.modified_on, objMicrowaveLinkFeeder.modified_by, objMicrowaveFeeder.modified_by);
               
                    objMicrowaveFeeder.feeder_name = objMicrowaveLinkFeeder.feeder_name;
                    objMicrowaveFeeder.feeder_type = objMicrowaveLinkFeeder.feeder_type;
                    objMicrowaveFeeder.length = objMicrowaveLinkFeeder.length;
                    objMicrowaveFeeder.loss = objMicrowaveLinkFeeder.loss;
                    objMicrowaveFeeder.loss_rx = objMicrowaveLinkFeeder.loss_rx;
                    objMicrowaveFeeder.loss_tx = objMicrowaveLinkFeeder.loss_tx;
                    objMicrowaveFeeder.mwlink_sys_id = objMicrowaveLinkFeeder.mwlink_sys_id;
                    objMicrowaveFeeder.side = objMicrowaveLinkFeeder.side;
                    objMicrowaveFeeder.modified_by = userId;
                    objMicrowaveFeeder.modified_on = DateTimeHelper.Now;

                    resultItem = repo.Update(objMicrowaveFeeder);


                }
                else
                {
                    resultItem = repo.Insert(objMicrowaveLinkFeeder);
                }


                return resultItem;
            }
            catch { throw; }
        }
        public int DeleteMicrowaveLinkFeederById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }
        public MicrowavelinkFeederSystem getMicrowaveLinkFeederDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
    }

    public class DAMicrowavelinkPower : Repository<MicrowavelinkPower>
    {
        public MicrowavelinkPower Save(MicrowavelinkPower objMicrowaveLinkPowerMaster, int userId)
        {
            try
            {
                var resultItem = new MicrowavelinkPower();
                var objMicrowaveLink = repo.Get(x => x.system_id == objMicrowaveLinkPowerMaster.system_id);
                if (objMicrowaveLink != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objMicrowaveLinkPowerMaster.modified_on, objMicrowaveLink.modified_on, objMicrowaveLinkPowerMaster.modified_by, objMicrowaveLink.modified_by);
                   
                    objMicrowaveLink.fade_margin = objMicrowaveLinkPowerMaster.fade_margin;
                    objMicrowaveLink.eirp = objMicrowaveLinkPowerMaster.eirp;
                    objMicrowaveLink.flat_margin_multipath = objMicrowaveLinkPowerMaster.flat_margin_multipath;
                    objMicrowaveLink.flat_margin_rain = objMicrowaveLinkPowerMaster.flat_margin_rain;
                    objMicrowaveLink.flat_margin_refraction = objMicrowaveLinkPowerMaster.flat_margin_refraction;
                    objMicrowaveLink.interface_td_multipath = objMicrowaveLinkPowerMaster.interface_td_multipath;
                    objMicrowaveLink.interface_td_rain = objMicrowaveLinkPowerMaster.interface_td_rain;
                    objMicrowaveLink.interface_td_refraction = objMicrowaveLinkPowerMaster.interface_td_refraction;
                    objMicrowaveLink.mwlink_sys_id = objMicrowaveLinkPowerMaster.mwlink_sys_id;
                    objMicrowaveLink.other_margin_multipath = objMicrowaveLinkPowerMaster.other_margin_multipath;
                    objMicrowaveLink.other_margin_rain = objMicrowaveLinkPowerMaster.other_margin_rain;
                    objMicrowaveLink.other_margin_refraction = objMicrowaveLinkPowerMaster.other_margin_refraction;
                    objMicrowaveLink.power_type = objMicrowaveLinkPowerMaster.power_type;
                    objMicrowaveLink.modified_by = userId;
                    objMicrowaveLink.modified_on = DateTimeHelper.Now;
                    objMicrowaveLink.rx_power = objMicrowaveLinkPowerMaster.rx_power;
                    objMicrowaveLink.rx_power_diversity = objMicrowaveLinkPowerMaster.rx_power_diversity;
                    objMicrowaveLink.side = objMicrowaveLinkPowerMaster.side;
                    objMicrowaveLink.threshold= objMicrowaveLinkPowerMaster.threshold;
                    objMicrowaveLink.tot_performance_month = objMicrowaveLinkPowerMaster.tot_performance_month;
                    objMicrowaveLink.tot_performance_year = objMicrowaveLinkPowerMaster.tot_performance_year;
                    objMicrowaveLink.tx_power = objMicrowaveLinkPowerMaster.tx_power;

                    resultItem = repo.Update(objMicrowaveLink);


                }
                else
                {
                    objMicrowaveLinkPowerMaster.created_by = userId;
                    objMicrowaveLinkPowerMaster.created_on = DateTimeHelper.Now;
                    resultItem = repo.Insert(objMicrowaveLinkPowerMaster);
                }


                return resultItem;
            }
            catch { throw; }
        }
        public int DeleteMicrowaveLinkPowerById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }
        public MicrowavelinkPower getMicrowaveLinkPowerDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
    }

    public class DAMicrowaveAntenna : Repository<MicrowavelinkAntenna>
    {
        public MicrowavelinkAntenna Save(MicrowavelinkAntenna objMicrowaveLinkAntennaMaster, int userId)
        {
            try
            {
                var resultItem = new MicrowavelinkAntenna();
                var objMicrowaveLink = repo.Get(x => x.system_id == objMicrowaveLinkAntennaMaster.system_id);
                if (objMicrowaveLink != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objMicrowaveLinkAntennaMaster.modified_on, objMicrowaveLink.modified_on, objMicrowaveLinkAntennaMaster.modified_by, objMicrowaveLink.modified_by);
                 
                    objMicrowaveLink.antenna_loss = objMicrowaveLinkAntennaMaster.antenna_loss;
                    objMicrowaveLink.antenna_name = objMicrowaveLinkAntennaMaster.antenna_name;
                    objMicrowaveLink.antenna_operator = objMicrowaveLinkAntennaMaster.antenna_operator;
                    objMicrowaveLink.antenna_type = objMicrowaveLinkAntennaMaster.antenna_type;
                    objMicrowaveLink.azimuth = objMicrowaveLinkAntennaMaster.azimuth;
                    objMicrowaveLink.coupling_loss = objMicrowaveLinkAntennaMaster.coupling_loss;
                    objMicrowaveLink.diameter= objMicrowaveLinkAntennaMaster.diameter;
                    objMicrowaveLink.far_end_id = objMicrowaveLinkAntennaMaster.far_end_id;
                    objMicrowaveLink.gain = objMicrowaveLinkAntennaMaster.gain;
                    objMicrowaveLink.height = objMicrowaveLinkAntennaMaster.height;
                    objMicrowaveLink.loss_rx = objMicrowaveLinkAntennaMaster.loss_rx;
                    objMicrowaveLink.loss_tx= objMicrowaveLinkAntennaMaster.loss_tx;
                    objMicrowaveLink.mwlink_sys_id = objMicrowaveLinkAntennaMaster.mwlink_sys_id;
                    objMicrowaveLink.radome_loss = objMicrowaveLinkAntennaMaster.radome_loss;
                    objMicrowaveLink.side = objMicrowaveLinkAntennaMaster.side;
                    objMicrowaveLink.modified_by = userId;
                    objMicrowaveLink.modified_on = DateTimeHelper.Now;
                    objMicrowaveLink.tilt= objMicrowaveLinkAntennaMaster.tilt;

                    resultItem = repo.Update(objMicrowaveLink);


                }
                else
                {
                    objMicrowaveLinkAntennaMaster.created_by = userId;
                    objMicrowaveLinkAntennaMaster.created_on = DateTimeHelper.Now;
                    
                    resultItem = repo.Insert(objMicrowaveLinkAntennaMaster);
                    

                }


                return resultItem;
            }
            catch { throw; }
        }
        public int DeleteMicrowaveLinkAntennaById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }
        public MicrowavelinkAntenna getMicrowaveLinkAntennaDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
    }
}
