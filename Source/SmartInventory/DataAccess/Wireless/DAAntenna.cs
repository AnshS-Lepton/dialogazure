using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAAntenna : Repository<AntennaMaster>
    {
        public AntennaMaster SaveAntennaEntity(AntennaMaster objAntennaMaster, int userId)
        {
            try
            {
                var resultItem = new AntennaMaster();
                var objAntenna = repo.Get(x => x.system_id == objAntennaMaster.system_id);
                if (objAntenna != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objAntennaMaster.modified_on, objAntenna.modified_on, objAntennaMaster.modified_by, objAntenna.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objAntennaMaster.objPM = objPageValidate;
                        return objAntennaMaster;
                    }
                    objAntenna.network_name = objAntennaMaster.network_name;
                    objAntenna.antenna_operator = objAntennaMaster.antenna_operator;
                    objAntenna.antenna_sub_type = objAntennaMaster.antenna_sub_type;
                    objAntenna.antenna_type = objAntennaMaster.antenna_type;
                    objAntenna.azimuth = objAntennaMaster.azimuth;
                    objAntenna.boresight_gain = objAntennaMaster.boresight_gain;
                    objAntenna.circuit_id = objAntennaMaster.circuit_id;
                    objAntenna.minimum_frequency = objAntennaMaster.minimum_frequency;
                    objAntenna.maximum_gain = objAntennaMaster.maximum_gain;
                    objAntenna.maximum_frequency = objAntennaMaster.maximum_frequency;
                    objAntenna.diameter_in_meter = objAntennaMaster.diameter_in_meter;
                    objAntenna.height = objAntennaMaster.height;
                    objAntenna.co_polor_vertical_bw = objAntennaMaster.co_polor_vertical_bw;
                    objAntenna.co_polor_vertical_fb = objAntennaMaster.co_polor_vertical_fb;
                    objAntenna.co_polor_vertical_maximum_gain = objAntennaMaster.co_polor_vertical_maximum_gain;
                    objAntenna.modified_by = userId;
                    objAntenna.modified_on = DateTimeHelper.Now;
					objAntenna.mechanical_tilt = objAntennaMaster.mechanical_tilt;
					objAntenna.electrical_tilt = objAntennaMaster.electrical_tilt;
					objAntenna.total_tilt = objAntennaMaster.total_tilt;
                    objAntenna.user_cross_polor_pattern = objAntennaMaster.user_cross_polor_pattern;
                    objAntenna.co_polor_vertical_boresight = objAntennaMaster.co_polor_vertical_boresight;
                    objAntenna.project_id = objAntennaMaster.project_id ?? 0;
                    objAntenna.planning_id = objAntennaMaster.planning_id ?? 0;
                    objAntenna.workorder_id = objAntennaMaster.workorder_id ?? 0;
                    objAntenna.purpose_id = objAntennaMaster.purpose_id ?? 0;
                    //objAntenna.Antenna_type = objAntennaMaster.Antenna_type;
                    objAntenna.ownership_type = objAntennaMaster.ownership_type;
                    //objAntenna.acquire_from = objAntennaMaster.acquire_from;
                    objAntenna.third_party_vendor_id = objAntennaMaster.third_party_vendor_id;
                    objAntenna.cross_polor_vertical_maximum_gain = objAntennaMaster.cross_polor_vertical_maximum_gain;
                    objAntenna.cross_polor_vertical_fb = objAntennaMaster.cross_polor_vertical_fb;
                    objAntenna.cross_polor_horizontal_fb = objAntennaMaster.cross_polor_horizontal_fb;
                    objAntenna.cross_polor_horizontal_maximum_gain = objAntennaMaster.cross_polor_horizontal_maximum_gain;
                    objAntenna.cross_polor_horizontal_bw = objAntennaMaster.cross_polor_horizontal_bw;
                    objAntenna.cross_polor_horizontal_boresight = objAntennaMaster.cross_polor_horizontal_boresight;
                    objAntenna.cross_polor_vertical_bw = objAntennaMaster.cross_polor_vertical_bw;
                    objAntenna.cross_polor_vertical_boresight = objAntennaMaster.cross_polor_vertical_boresight;
                    objAntenna.co_polor_horizontal_maximum_gain = objAntennaMaster.co_polor_horizontal_maximum_gain;
                    objAntenna.co_polor_horizontal_fb = objAntennaMaster.co_polor_horizontal_fb;
                    objAntenna.co_polor_horizontal_bw = objAntennaMaster.co_polor_horizontal_bw;
                    objAntenna.co_polor_horizontal_boresight = objAntennaMaster.co_polor_horizontal_boresight;
                    objAntenna.status_remark = objAntennaMaster.status_remark;
                    objAntenna.remarks = objAntennaMaster.remarks;
                    objAntenna.model_number = objAntennaMaster.model_number;
                    objAntenna.polarization = objAntennaMaster.polarization;
                    objAntenna.manufacturer_name = objAntennaMaster.manufacturer_name;
                    objAntenna.requested_by = objAntennaMaster.requested_by;
                    objAntenna.request_approved_by = objAntennaMaster.request_approved_by;
                    objAntenna.request_ref_id = objAntennaMaster.request_ref_id;
                    objAntenna.origin_ref_id = objAntennaMaster.origin_ref_id;
                    objAntenna.origin_ref_description = objAntennaMaster.origin_ref_description;
                    objAntenna.origin_from = objAntennaMaster.origin_from;
                    objAntenna.origin_ref_code = objAntennaMaster.origin_ref_code;
                    //   objAntenna.served_by_ring = objAntennaMaster.served_by_ring;
                    objAntenna.bom_sub_category = objAntennaMaster.bom_sub_category;
                    resultItem = repo.Update(objAntenna);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Antenna.ToString(), resultItem.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Antenna.ToString(), resultItem.province_id);
                }
                else
                {
                    objAntennaMaster.created_by = userId;
                    objAntennaMaster.created_on = DateTimeHelper.Now;
                    objAntennaMaster.status = "A";
                    objAntennaMaster.network_status = "P";
                    resultItem = repo.Insert(objAntennaMaster);

                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Antenna.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Antenna.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Antenna.ToString(), resultItem.province_id);
                }

             
                return resultItem;
            }
            catch(Exception ex){ throw  ex; }
        }
        public int DeleteAntennaById(int systemId)
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
        public AntennaMaster getAntennaDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
    }

    public class DAVSATAntenna : Repository<VSATAntenna>
    {
        public VSATAntenna GetVsatAntennaById(int id)
        {
            try
            {
                return repo.GetById(m => m.id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string SaveVsatAntenna(VSATAntenna objVSATAntenna,int user_id)
        {
            try
            {

                var result = "Failed";

                var objExisiting = repo.GetById(m => m.parent_system_id == objVSATAntenna.parent_system_id);
                if (objExisiting != null)
                {
                    objExisiting.status = objVSATAntenna.status;
                    objExisiting.satellite_type = objVSATAntenna.satellite_type;
                    objExisiting.transponder_type = objVSATAntenna.transponder_type;
                    objExisiting.look_angle_deg = objVSATAntenna.look_angle_deg;
                    objExisiting.elevation_deg = objVSATAntenna.elevation_deg;
                    objExisiting.operational_from_date_bs = objVSATAntenna.operational_from_date_bs;
                    objExisiting.operational_to_date_bs = objVSATAntenna.operational_to_date_bs;
                    objExisiting.radio_model = objVSATAntenna.radio_model;
                    objExisiting.orientation = objVSATAntenna.orientation;
                    objExisiting.purpose = objVSATAntenna.purpose;
                    objExisiting.rf_band_type = objVSATAntenna.rf_band_type;
                    objExisiting.uplink_ghz = objVSATAntenna.uplink_ghz;
                    objExisiting.downlink_ghz = objVSATAntenna.downlink_ghz;
                    objExisiting.modified_by = user_id;
                    objExisiting.modified_on = DateTimeHelper.Now;
                    repo.Update(objExisiting);
                    result = "Update";
                }


                else
                {
                    objVSATAntenna.created_by = user_id;
                objVSATAntenna.created_on = DateTimeHelper.Now;
                repo.Insert(objVSATAntenna);
                result = "Save";
                }


                return result;
            }
            catch { throw; }
        }

        public int GetBuildingSystemId( int id)
        {
            try
            {
                var result = repo.ExecuteProcedure<int>("fn_get_building_system_id", new {p_id=id  }).FirstOrDefault();
                return result;
            }
            catch { throw; }

        }
    }
    }
