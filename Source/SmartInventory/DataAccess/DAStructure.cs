using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.ISP;
using DataAccess.ISP;


namespace DataAccess
{
    public class DAStructure : Repository<StructureMaster>
    {
        DAStructure()
        {

        }
        private static DAStructure objStructure = null;
        private static readonly object lockObject = new object();
        public static DAStructure Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objStructure == null)
                    {
                        objStructure = new DAStructure();
                    }
                }
                return objStructure;
            }
        }

        public List<StructureList> getStructureByBuffer(string latlong)
        {
            try
            {
                return repo.ExecuteProcedure<StructureList>("fn_get_structures_in_buffer", new { p_longlat = latlong }).ToList();
            }
            catch { throw; }
        }

        public StructureMaster SaveStructure(StructureMaster structureInfo, string status)
        {
            try
            {
                var objStructure = repo.Get(u => u.system_id == structureInfo.system_id);
                PageMessage objPageValidate = new PageMessage();
                DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                {
                    system_id = structureInfo.system_id,
                    entity_type = EntityType.Structure.ToString(),
                    home_pass = structureInfo.home_pass,
                    business_pass = structureInfo.business_pass,
                    parent_system_id = structureInfo.parent_system_id,
                    parent_entity_type = structureInfo.parent_entity_type
                }, structureInfo.system_id == 0);
                if (!string.IsNullOrEmpty(objMessage.message))
                {
                    objPageValidate.status = ResponseStatus.FAILED.ToString();
                    objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                    structureInfo.pageMsg = objPageValidate;
                    return structureInfo;
                }
                if (objStructure != null)
                {
                    objPageValidate = DAUtility.ValidateModifiedDate(structureInfo.modified_on, objStructure.modified_on, structureInfo.userid, objStructure.modified_by);
                    if (objPageValidate.message != null)
                    {
                        structureInfo.pageMsg = objPageValidate;
                        return structureInfo;
                    }

                    objStructure.no_of_flat = structureInfo.no_of_flat;
                    objStructure.no_of_floor = structureInfo.no_of_floor;
                    objStructure.no_of_shaft = structureInfo.no_of_shaft;
                    objStructure.no_of_occupants = structureInfo.no_of_occupants;
                    objStructure.structure_name = structureInfo.structure_name;
                    objStructure.business_pass = structureInfo.business_pass;
                    objStructure.home_pass = structureInfo.home_pass;
                    objStructure.remarks = structureInfo.remarks;
                    objStructure.modified_by = structureInfo.userid;
                    objStructure.modified_on = DateTimeHelper.Now;
                    objStructure.status_remark = structureInfo.status_remark;
                    var latLong = structureInfo.geom.Split(' ');
                    var longitude = latLong[0];
                    var latitude = latLong[1];
                    objStructure.requested_by = structureInfo.requested_by;
                    objStructure.request_approved_by = structureInfo.request_approved_by;
                    objStructure.request_ref_id = structureInfo.request_ref_id;
                    objStructure.origin_ref_id = structureInfo.origin_ref_id;
                    objStructure.origin_ref_description = structureInfo.origin_ref_description;
                    objStructure.origin_from = structureInfo.origin_from;
                    objStructure.origin_ref_code = structureInfo.origin_ref_code;
                    var result = repo.Update(objStructure);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Structure.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Structure.ToString(), result.province_id);
                    var chkSaveShaft = DAShaft.Instance.SaveShaft(structureInfo.lstShaftInfo, structureInfo.userid, objStructure.system_id);
                    var chkSaveFloor = DAFloor.Instance.SaveFloor(structureInfo.lstFloorInfo, structureInfo.userid, objStructure.system_id,lat: latitude,lng: longitude);
                    result.lstShaftInfo = structureInfo.lstShaftInfo;
                    result.lstFloorInfo = structureInfo.lstFloorInfo;
                    DAFDBInfo.Instance.FDBWithRiser(structureInfo, 0);
                    return result;
                }
                else
                {
                    //structureInfo.status = "A";
                    //structureInfo.network_status = "P";
                    structureInfo.status = String.IsNullOrEmpty(structureInfo.status) ? "A" : structureInfo.status;
                    structureInfo.network_status = String.IsNullOrEmpty(structureInfo.network_status) ? "P" : structureInfo.network_status;
                    structureInfo.created_by = structureInfo.userid;
                    structureInfo.created_on = DateTimeHelper.Now;
                    var result = repo.Insert(structureInfo);
                    //transaction need to implement  there...
                    //insert geometery detail....
                    InputGeom geom = new InputGeom();
                    geom.systemId = result.system_id;
                    geom.longLat = structureInfo.geom.Replace(",", " ");
                    geom.userId = result.userid;
                    geom.entityType = EntityType.Structure.ToString();
                    geom.commonName = result.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Structure.ToString(), result.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Structure.ToString(), result.province_id);

                    if (structureInfo.lstShaftInfo.Count == 0 && structureInfo.isDefault)
                    {
                        structureInfo.lstShaftInfo.Add(new StructureShaftInfo { shaft_name = "Shaft_1", is_virtual = true, shaft_position = "left" });
                        structureInfo.lstShaftInfo.Add(new StructureShaftInfo { shaft_name = "Shaft_2", is_virtual = true, shaft_position = "right" });
                    }

                    var chkSaveShaft = DAShaft.Instance.SaveShaft(structureInfo.lstShaftInfo, result.userid, result.system_id);

                    var latLong = structureInfo.geom.Split(' ');
                    var longitude = latLong[0];
                    var latitude = latLong[1];

                    var chkSaveFloor = DAFloor.Instance.SaveFloor(structureInfo.lstFloorInfo, result.userid, result.system_id,  result.network_id,EntityType.Structure.ToString(), longitude,latitude);

                    result.lstShaftInfo = structureInfo.lstShaftInfo;
                    result.lstFloorInfo = structureInfo.lstFloorInfo;
                   

                    DAFDBInfo.Instance.FDBWithRiser(result, 0);
                    return result;
                }
            }
            catch { throw; }
        }

        public List<StructureMaster> GetStructureByBld(int buildingId)
        {
            try
            {
                var result = repo.ExecuteProcedure<StructureMaster>("fn_get_structure_by_bld", new { p_building_id = buildingId }, true);
                return result != null && result.Count > 0 ? result : new List<StructureMaster>();

            }
            catch { throw; }
        }

        public int DeleteStructureById(int structureId)
        {
            int result = 0;
            try
            {
                //var structureDetails = repo.Get(u => u.system_id == structureId && u.network_status == "P");
                var structureDetails = repo.Get(u => u.system_id == structureId);
                if (structureDetails != null)
                {
                    result = repo.Delete(structureDetails.system_id);
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public StructureMaster UpdateShaftInStructure(int systemId, int userId)
        {
            try
            {

                var objStructure = repo.Get(u => u.system_id == systemId);
                if (objStructure != null)
                {

                    objStructure.no_of_shaft = objStructure.no_of_shaft - 1;

                    objStructure.modified_by = userId;
                    objStructure.modified_on = DateTimeHelper.Now;
                    var result = repo.Update(objStructure);

                    return objStructure;
                }

            }
            catch { throw; }
            return new StructureMaster();
        }

        public StructureMaster getSructureDetailsByCode(string StrCode)
        {
            try
            {
                return repo.GetById(m => m.network_id == StrCode);
            }
            catch { throw; }
        }
        public StructureMaster getSructureDetailsById(int SystemId)
        {
            try
            {
                var result = repo.GetById(m => m.system_id == SystemId);
                return result != null ? result : new StructureMaster();

            }
            catch { throw; }

        }

        public StructureMaster UpdateFloorInStructure(int systemId,int totalUnit, int userId)
        {
            try
            {

                var objStructure = repo.Get(u => u.system_id == systemId);
                if (objStructure != null)
                {

                    objStructure.no_of_floor = objStructure.no_of_floor - 1;

                    objStructure.modified_by = userId;
                    objStructure.modified_on = DateTimeHelper.Now;
                    objStructure.no_of_flat = totalUnit;
                    var result = repo.Update(objStructure);

                    return objStructure;
                }

            }
            catch { throw; }
            return new StructureMaster();
        }
        public string CheckEntityAssociation(StructureMaster objStructure)
        {
            try
            {
                string shaftName = "";
                if (!(string.IsNullOrEmpty(objStructure.prevShaftWithoutRiser)))
                {
                    var prevShaft = objStructure.prevShaftWithoutRiser.Trim(',').Split(',').ToArray();
                    foreach (var item in prevShaft)
                    {
                        var shaftId = Convert.ToInt32(item);
                        var prevShaftRecord = objStructure.lstShaftInfo.Where(m => m.system_id == shaftId).FirstOrDefault();
                        if (prevShaftRecord.with_riser == false)
                        {
                            int asociationCount = repo.ExecuteProcedure<int>("fn_get_entity_association", new { p_shaft_id = shaftId, p_system_id = 0, p_entity_type = EntityType.FDB.ToString(), p_structure_id = objStructure.system_id }).FirstOrDefault();
                            if (!(asociationCount > 0))
                            {
                                repo.ExecuteProcedure("fn_delete_entity_N_association", new { p_shaft_id = shaftId, p_entity_type = EntityType.FDB.ToString(), p_structure_id = objStructure.system_id });
                            }
                            else
                            {
                                prevShaftRecord.with_riser = true;
                                shaftName += prevShaftRecord.shaft_name + ",".Trim(',');
                                //shaftName = "<tr><td>" + prevShaftRecord.shaft_name + ",".Trim(',') + "</td></tr>";
                            }
                        }
                    }
                }
                return shaftName;
            }
            catch { throw; }
        }
        public int getParentStructure(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<int>("fn_get_parent_structure", new { p_system_id = systemId, p_entity_type = entityType }).FirstOrDefault();

            }
            catch { throw; }
        }
        public string getBuildingAddress(int structureId)
        {
            try
            {
                return repo.ExecuteProcedure<string>("fn_get_building_address", new { p_structure_id = structureId }).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }
    }
    public sealed class DAShaft : Repository<ShaftInfo>
    {
        DAShaft()
        {

        }
        private static DAShaft objShaft = null;
        private static readonly object lockObjBL = new object();
        public static DAShaft Instance
        {
            get
            {
                lock (lockObjBL)
                {
                    if (objShaft == null)
                    {
                        objShaft = new DAShaft();
                    }
                    return objShaft;
                }
            }
        }

        public List<StructureShaftInfo> GetShaftByBld(int structureId)
        {
            List<StructureShaftInfo> result = new List<StructureShaftInfo>();
            try
            {
                var structureDetails = repo.GetAll(u => u.structure_id == structureId);
                if (structureDetails != null)
                {
                    result = structureDetails.Select(x => new StructureShaftInfo
                    {
                        system_id = x.system_id,
                        shaft_name = x.shaft_name,
                        is_virtual = x.is_virtual,
                        with_riser = x.with_riser,
                        length = x.length,
                        width = x.width,
                        shaft_position = x.shaft_position,
                        is_partial_shaft = x.is_partial_shaft
                    }).ToList();
                    return result != null && result.Count > 0 ? result : new List<StructureShaftInfo>();
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public bool SaveShaft(List<StructureShaftInfo> shaftInfoLst, int userId, int structureId, string status = "")
        {
            bool result = false;
            try
            {
                ShaftInfo resShaft = new ShaftInfo();
                foreach (StructureShaftInfo oShaft in shaftInfoLst)
                {
                    var objShaft = repo.Get(u => u.structure_id == structureId && u.system_id == oShaft.system_id);
                    if (objShaft != null)
                    {

                        objShaft.shaft_name = oShaft.shaft_name;
                        objShaft.is_virtual = oShaft.is_virtual;
                        objShaft.with_riser = oShaft.with_riser;
                        objShaft.shaft_position = oShaft.shaft_position;
                        objShaft.modified_by = userId;
                        objShaft.modified_on = DateTimeHelper.Now;
                        repo.Update(objShaft);
                        result = true;
                    }
                    else
                    {

                        resShaft.structure_id = structureId;
                        resShaft.shaft_name = oShaft.shaft_name;
                        resShaft.is_virtual = oShaft.is_virtual;
                        resShaft.with_riser = oShaft.with_riser;
                        resShaft.length = oShaft.length;
                        resShaft.width = oShaft.width;
                        resShaft.shaft_position = oShaft.shaft_position;
                        resShaft.status = "A";
                        resShaft.network_status = "P";
                        resShaft.created_by = userId;
                        resShaft.created_on = DateTimeHelper.Now;
                        repo.Insert(resShaft);


                        result = true;
                    }
                }
                return result;
            }
            catch { throw; }
        }

        public bool SaveShaftInfo(ShaftInfo model, int userid)
        {
            bool result = false;
            try
            {
                var objShaft = repo.Get(u => u.structure_id == model.structure_id && u.system_id == model.system_id);
                if (objShaft != null)
                {

                    objShaft.shaft_name = model.shaft_name;
                    objShaft.is_virtual = model.is_virtual;
                    objShaft.with_riser = model.with_riser;
                    objShaft.is_partial_shaft = model.is_partial_shaft;
                    objShaft.modified_by = userid;
                    objShaft.modified_on = DateTimeHelper.Now;
                    repo.Update(objShaft);
                    result = true;
                }
                if (model.with_riser)
                {
                    var structureInfo = new DAMisc().GetEntityDetailById<StructureMaster>(model.structure_id, EntityType.Structure,0);
                    structureInfo.userid = userid;
                    DAFDBInfo.Instance.FDBWithRiser(structureInfo, model.system_id);
                }
                return result;
            }
            catch { throw; }
        }
        public int DeleteShaftById(int shaftId, int userId)
        {
            int result = 0;
            int structureId = 0;
            try
            {
                var objShaft = repo.Get(u => u.system_id == shaftId && u.network_status == "P");
                if (objShaft != null)
                {
                    structureId = objShaft.structure_id;
                    result = repo.Delete(objShaft.system_id);
                    if (result == 1)
                    {
                        var updateShaftCout = DAStructure.Instance.UpdateShaftInStructure(structureId, userId);
                    }
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public ShaftInfo getShaftInfo(int? shaftId)
        {
            return repo.GetById(m => m.system_id == shaftId);
        }
        public bool updateShaftName(int floorId, int structureid, string floorName)
        {
            try
            {
                var shaftInfo = repo.GetById(m => m.system_id == floorId && m.structure_id == structureid);
                if (shaftInfo != null)
                {
                    shaftInfo.shaft_name = floorName;
                    repo.Update(shaftInfo);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

    }
    public sealed class DAFloor : Repository<FloorInfo>
    {
        DAFloor()
        {

        }
        private static DAFloor objFloor = null;
        private static readonly object lockObjBL = new object();
        public static DAFloor Instance
        {
            get
            {
                lock (lockObjBL)
                {
                    if (objFloor == null)
                    {
                        objFloor = new DAFloor();
                    }
                    return objFloor;
                }
            }
        }
        public FloorInfo GetFloorByName(string floorName,int structureId)
        {
            try
            {
                var result = repo.Get(m => m.floor_name == floorName && m.structure_id== structureId);
                return result != null ? result : new FloorInfo();

            }
            catch (Exception)
            {

                throw;
            }
        }
        public FloorInfo GetFloorById(int floorId)
        {
            try
            {
                var result = repo.Get(m => m.system_id == floorId);
                return result != null ? result : new FloorInfo();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<StructureFloorInfo> GetFloorByBld(int structureId)
        {
            List<StructureFloorInfo> result = new List<StructureFloorInfo>();
            try
            {
                //var structureDetails = repo.GetAll(u => u.structure_id == structureId).OrderBy(m => m.system_id);
                var floorDetails = DAISP.Instance.getShaftNFloor(structureId);
                floorDetails = floorDetails.Where(m => m.isshaft == false).OrderBy(m => m.systemid).ToList();
                if (floorDetails != null)
                {
                    result = floorDetails.Select(x => new StructureFloorInfo
                    {
                        system_id = x.systemid,
                        floor_name = x.entityname,
                        no_of_units = x.no_of_units,
                        length = x.length,
                        width = x.width,
                        height = x.height,
                        total_Placed_Units = x.total_Placed_Units
                    }).ToList();
                    return result != null && result.Count > 0 ? result : new List<StructureFloorInfo>();
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public bool SaveFloor(List<StructureFloorInfo> FloorInfoLst, int userId, int structureId,string parent_network_id = "", string parentType = "", string lng = "", string lat = "", string status = "")
        {
            bool result = false;
            try
            {
                FloorInfo resFloor = new FloorInfo();
                foreach (StructureFloorInfo oFloor in FloorInfoLst)
                {
                    var objFloor = repo.Get(u => u.structure_id == structureId && u.system_id == oFloor.system_id);
                    if (objFloor != null)
                    {

                        objFloor.floor_name = oFloor.floor_name;
                        objFloor.no_of_units = oFloor.no_of_units;
                        objFloor.modified_by = userId;
                        objFloor.modified_on = DateTimeHelper.Now;
                        repo.Update(objFloor);
                        result = true;
                    }
                    else
                    {

                        var objNetworkCodeDetail = new DAMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Floor.ToString(), gType = GeometryType.Point.ToString(), eGeom = "", parent_eType = EntityType.Structure.ToString(), parent_sysId = structureId });

                        resFloor.structure_id = structureId;
                        resFloor.floor_name = oFloor.floor_name;
                        resFloor.no_of_units = oFloor.no_of_units;
                        resFloor.length = oFloor.length;
                        resFloor.width = oFloor.width;
                        resFloor.height = oFloor.height;
                        resFloor.status = "A";
                        resFloor.network_status = "P";
                        resFloor.network_id = objNetworkCodeDetail.network_code;
                        resFloor.sequence_id = objNetworkCodeDetail.sequence_id;
                        resFloor.parent_system_id = structureId;
                        resFloor.parent_network_id = parent_network_id;
                        resFloor.parent_entity_type = parentType;
                        resFloor.longitude = Convert.ToDouble(lng);
                        resFloor.latitude = Convert.ToDouble(lat);
                        resFloor.created_by = userId;
                        resFloor.created_on = DateTimeHelper.Now;
                        repo.Insert(resFloor);
                        result = true;
                    }
                }
                return result;
            }
            catch { throw; }
        }

        public int DeleteFloorById(int FloorId, int userId)
        {
            int result = 0;
            int structureId = 0;
            var totalUnit = 0;
            try
            {
                var objFloor = repo.Get(u => u.system_id == FloorId && u.network_status == "P");
                if (objFloor != null)
                {
                    structureId = objFloor.structure_id;
                    result = repo.Delete(objFloor.system_id);
                    if (result == 1)
                    {
                        var allFloors = repo.GetAll(u => u.structure_id == structureId).ToList();
                        if (allFloors.Count() > 0)
                        {
                            totalUnit=allFloors.Sum(m => m.no_of_units);
                        }
                        var updateFloorCout = DAStructure.Instance.UpdateFloorInStructure(structureId, totalUnit, userId);
                    }
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

    }
   

    public class RfSbuilding_status : Repository<BuildingRfSStatus>
    {
        private static RfSbuilding_status objStructure = null;
        private static readonly object lockObject = new object();
        public static RfSbuilding_status Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objStructure == null)
                    {
                        objStructure = new RfSbuilding_status();
                    }
                }
                return objStructure;
            }
        }
        public IEnumerable<BuildingRfSStatus> GetRFS_StatusByBld(int buildingId)
        {
            try
            {
                return repo.GetAll(x => x.building_id == buildingId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
   
}
