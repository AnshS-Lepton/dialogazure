using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DACable : Repository<CableMaster>
    {
       
        private static DACable objCable = null;
        private static readonly object lockObject = new object();
        public static DACable Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objCable == null)
                    {
                        objCable = new DACable();
                    }
                }
                return objCable;
            }
        }
        public void InsertKML(string SQLQuery)
        {
            repo.ExecuteSQLCommand(SQLQuery);
        }
        public CableMaster SaveCable(CableMaster cableInfo, int userId)
        {
            try
            {
                var objCable = repo.Get(u => u.system_id == cableInfo.system_id);
                var objcdb = DACDBAttribute.Instance.Get(cableInfo.system_id);
                if (objCable != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(cableInfo.modified_on, objCable.modified_on, cableInfo.modified_by, objCable.modified_by);
                    if (objPageValidate.message != null)
                    {
                        cableInfo.objPM = objPageValidate;
                        return cableInfo;
                    }
                    int oldNoOfTube = objCable.no_of_tube;
                    int oldCorePerTube = objCable.no_of_core_per_tube;
                    //objCable.a_system_id = cableInfo.a_system_id > 0 ? cableInfo.a_system_id : objCable.a_system_id;
                    //objCable.a_entity_type = !string.IsNullOrWhiteSpace(cableInfo.a_entity_type) ? cableInfo.a_entity_type : objCable.a_entity_type;
                    objCable.a_location = cableInfo.a_location;
                    objCable.b_location = cableInfo.b_location;

					objCable.a_location_name = cableInfo.a_location_name;
					objCable.b_location_name = cableInfo.b_location_name;
					//objCable.b_system_id = cableInfo.b_system_id > 0 ? cableInfo.b_system_id : objCable.b_system_id; ;
					//objCable.b_entity_type = !string.IsNullOrWhiteSpace(cableInfo.b_entity_type) ? cableInfo.b_entity_type : objCable.b_entity_type;
					objCable.pin_code = cableInfo.pin_code;
                    objCable.no_of_core_per_tube = cableInfo.no_of_core_per_tube;
                    objCable.no_of_tube = cableInfo.no_of_tube;
                    objCable.cable_calculated_length = cableInfo.cable_calculated_length;
                    objCable.cable_name = cableInfo.cable_name;


                    objCable.specification = cableInfo.specification;
                    objCable.category = cableInfo.category;
                    objCable.subcategory1 = cableInfo.subcategory1;
                    objCable.subcategory2 = cableInfo.subcategory2;
                    objCable.subcategory3 = cableInfo.subcategory3;
                    objCable.item_code = cableInfo.item_code;
                    objCable.vendor_id = cableInfo.vendor_id;
                    objCable.type = cableInfo.type;
                    objCable.brand = cableInfo.brand;
                    objCable.model = cableInfo.model;
                    objCable.construction = cableInfo.construction;
                    objCable.activation = cableInfo.activation;
                    objCable.accessibility = cableInfo.accessibility;
                    objCable.cable_measured_length = cableInfo.cable_measured_length;
                    objCable.modified_on = DateTimeHelper.Now;
                    objCable.modified_by = userId;
                    //objCable.cable_status = cableInfo.cable_status;
                   // objCable.cable_status_comment = cableInfo.cable_status_comment;

                    objCable.project_id = cableInfo.project_id ?? 0;
                    objCable.planning_id = cableInfo.planning_id ?? 0;
                    objCable.workorder_id = cableInfo.workorder_id ?? 0;
                    objCable.purpose_id = cableInfo.purpose_id ?? 0;
                    objCable.cable_category = cableInfo.cable_category;
                    objCable.cable_sub_category = cableInfo.cable_sub_category;
                    objCable.manhole_count = cableInfo.manhole_count;
                    objCable.remarks = cableInfo.remarks;
                    objCable.route_id = cableInfo.route_id;
                    objCable.start_reading = cableInfo.start_reading;
                    objCable.end_reading = cableInfo.end_reading;
                    objCable.execution_method = cableInfo.execution_method;
                    objCable.circuit_id = cableInfo.circuit_id;
                    objCable.thirdparty_circuit_id = cableInfo.thirdparty_circuit_id;
                    objCable.acquire_from = cableInfo.acquire_from;
                    objCable.ownership_type = cableInfo.ownership_type;
                    objCable.third_party_vendor_id = cableInfo.third_party_vendor_id;
                    objCable.audit_item_master_id = cableInfo.audit_item_master_id;
                    objCable.primary_pod_system_id = cableInfo.primary_pod_system_id;
                    objCable.secondary_pod_system_id = cableInfo.secondary_pod_system_id;
                    objCable.status_remark = cableInfo.status_remark;
                    objCable.inner_dimension = cableInfo.inner_dimension;
                    objCable.outer_dimension = cableInfo.outer_dimension;
                    objCable.calculated_length_remark = cableInfo.calculated_length_remark;
                    objCable.is_acquire_from = cableInfo.is_acquire_from;
                    objCable.a_location_code = cableInfo.a_location_code;
					objCable.b_location_code = cableInfo.b_location_code;
					if (objCable.total_core != cableInfo.total_core)
                    {
                        var checkConnection = new DAMisc().isPortConnected(objCable.system_id, EntityType.Cable.ToString(), objCable.specification, objCable.vendor_id, objCable.item_code);
                        if (checkConnection.status)
                        {
                            objCable.isPortConnected = checkConnection.status;
                            objCable.message = Resources.Helper.MultilingualMessageConvert(checkConnection.message); //checkConnection.message;
                            return objCable;
                        }
                    }
                    objCable.total_core = cableInfo.total_core;
                    if (String.Concat(objCable.a_entity_type, objCable.a_system_id) != String.Concat(cableInfo.a_entity_type, cableInfo.a_system_id)
                         && new DAConnectionInfo().checkIsLineEntitySpliced(cableInfo.system_id, EntityType.Cable.ToString(), true))
                    {
                        objCable.isPortConnected = true;
                        objCable.message = "Cable Start point can not be change as it is spliced with some other entity!"; ;
                        return objCable;
                    }
                    else if (String.Concat(objCable.b_entity_type, objCable.b_system_id) != String.Concat(cableInfo.b_entity_type, cableInfo.b_system_id)
                        && new DAConnectionInfo().checkIsLineEntitySpliced(cableInfo.system_id, EntityType.Cable.ToString(), false))
                    {
                        objCable.isPortConnected = true;
                        objCable.message = "Cable End point can not be change as it is spliced with some other entity!"; ;
                        return objCable;
                    }

                    //if (((objCable.a_system_id != cableInfo.a_system_id || objCable.a_entity_type != cableInfo.a_entity_type)
                    //    || (objCable.b_system_id != cableInfo.b_system_id || objCable.b_entity_type != cableInfo.b_entity_type))
                    //    && new DAConnectionInfo().checkIsCableSpliced(cableInfo.system_id))
                    //{
                    //    objCable.isPortConnected = true;
                    //    objCable.message = "Cable termination point can not be change as it is spliced with some other entity!"; ;
                    //    return objCable;
                    //}
                    objCable.a_system_id = cableInfo.a_system_id > 0 ? cableInfo.a_system_id : objCable.a_system_id;
                    objCable.a_entity_type = !string.IsNullOrWhiteSpace(cableInfo.a_entity_type) ? cableInfo.a_entity_type : objCable.a_entity_type;
                    objCable.b_system_id = cableInfo.b_system_id > 0 ? cableInfo.b_system_id : objCable.b_system_id; ;
                    objCable.b_entity_type = !string.IsNullOrWhiteSpace(cableInfo.b_entity_type) ? cableInfo.b_entity_type : objCable.b_entity_type;

                    var association = new DAAssociateEntity().getAssociateEntity(objCable.system_id, EntityType.Cable.ToString());
                    if (objCable.cable_type != "ISP" && objCable.cable_type != cableInfo.cable_type && association.Count > 0)
                    {
                        objCable.isPortConnected = true;
                        objCable.message = "You can not convert <b>" + objCable.cable_type + "</b> to <b>" + cableInfo.cable_type + "</b> cable because it is associated with some entity, Please remove association!"; ;
                        return objCable;
                    }

                    objCable.cable_type = cableInfo.cable_type;
                    objCable.other_info = cableInfo.other_info; //for additional-attributes
                    objCable.requested_by = cableInfo.requested_by;
                    objCable.request_approved_by = cableInfo.request_approved_by;
                    objCable.request_ref_id = cableInfo.request_ref_id;
                    objCable.origin_ref_id = cableInfo.origin_ref_id;
                    objCable.origin_ref_description = cableInfo.origin_ref_description;
                    objCable.origin_from = cableInfo.origin_from;
                    objCable.origin_ref_code = cableInfo.origin_ref_code;
                    objCable.bom_sub_category = cableInfo.bom_sub_category;
                    objCable.route_name = cableInfo.route_name;
                    objCable.gis_design_id = cableInfo.gis_design_id;
                    objCable.aerial_location = cableInfo.aerial_location;
                    objCable.own_vendor_id = cableInfo.own_vendor_id;
                    objCable.generic_section_name = cableInfo.generic_section_name;
                    objCable.section_name = cableInfo.section_name;
                    objCable.hierarchy_type = cableInfo.hierarchy_type;
                    ////objCable.served_by_ring= cableInfo.served_by_ring;
                    var response = repo.Update(objCable);
                    RouteCreation routeObj = new DAMisc().createRouteId(response.system_id, Models.EntityType.Cable.ToString());
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.Cable.ToString(), response.province_id, 1);
                    //DbMessage geojsonObj = updateGeojsonMetadata(Models.EntityType.Cable.ToString(), response.province_id);
                    if (cableInfo.no_of_tube != oldNoOfTube || cableInfo.no_of_core_per_tube != oldCorePerTube)
                    {
                        SetCableColorDetails(response.system_id, response.no_of_tube, response.no_of_core_per_tube, userId);
                    }
                    if (!string.IsNullOrWhiteSpace(cableInfo.ispLineGeom))
                    {
                        DAIspLine.Instance.UpdateLineGeom(new IspLineMaster { entity_id = response.system_id, structure_id = cableInfo.structure_id, line_geom = cableInfo.ispLineGeom, a_node_type = cableInfo.a_node_type, b_node_type = cableInfo.b_node_type });
                    }
                    if ((objCable.a_system_id != cableInfo.a_system_id && objCable.a_entity_type != cableInfo.a_entity_type)
                     || (objCable.b_system_id != cableInfo.b_system_id && objCable.b_entity_type != cableInfo.b_entity_type))
                    {
                        DAIspLine.Instance.setCableEndPoint(response.system_id);
                    }
                    // update cdb attributes if cdb null then update it otherwise insert new record
                    if (objcdb != null)
                    {
                        DACDBAttribute.Instance.UpdateCDBAttribute(objcdb, cableInfo);
                    }
                    else
                    {
                        DACDBAttribute.Instance.SaveCDBAttribute(cableInfo);
                    }
                    return response;
                }
                else
                {
                    InputGeom geom = new InputGeom();
                    if (cableInfo.lstTP.Count > 0)
                    {
                        var startPoint = cableInfo.lstTP.Where(m => m.mode == "start").FirstOrDefault();
                        var endPoint = cableInfo.lstTP.Where(m => m.mode == "end").FirstOrDefault();
                        if (startPoint!=null && !string.IsNullOrEmpty(startPoint.actualLatLng))
                        {
                            cableInfo.a_system_id = startPoint.system_id;
                            cableInfo.a_entity_type = startPoint.network_name;
                            cableInfo.a_location = startPoint.network_id;
                            cableInfo.a_location_name = startPoint.entity_name;
                        }
                        if (endPoint != null && !string.IsNullOrEmpty(endPoint.actualLatLng))
                        {
                            cableInfo.b_system_id = endPoint.system_id;
                            cableInfo.b_entity_type = endPoint.network_name;
                            cableInfo.b_location = endPoint.network_id;
                            cableInfo.b_location_name = endPoint.entity_name;
                        } 
                        if (startPoint != null && !string.IsNullOrEmpty(startPoint.actualLatLng)) { cableInfo.geom = startPoint.actualLatLng + "," + cableInfo.geom; }
                        if (endPoint != null && !string.IsNullOrEmpty(endPoint.actualLatLng)) { cableInfo.geom = cableInfo.geom + "," + endPoint.actualLatLng; }
                    }
                    var latLong = cableInfo.geom;
                    //cableInfo.status = "A";
                    //cableInfo.network_status = "P";
                    cableInfo.status = String.IsNullOrEmpty(cableInfo.status) ? "A" : cableInfo.status;
                    cableInfo.network_status = String.IsNullOrEmpty(cableInfo.network_status) ? "P" : cableInfo.network_status;
                    cableInfo.created_on = DateTimeHelper.Now;
                    cableInfo.created_by = userId;
                    cableInfo.utilization = "L";
                    //cableInfo = repo.Insert(cableInfo);
                    var CableResp = repo.Insert(cableInfo);
                   
                    SetCableColorDetails(cableInfo.system_id, cableInfo.no_of_tube, cableInfo.no_of_core_per_tube, userId);
                    // save cdb attributes
                    DACDBAttribute.Instance.SaveCDBAttribute(cableInfo);
                    // Save geometry                    
                    if (cableInfo.cable_type != "ISP" && latLong != "" && latLong != "0")
                    {
                       
                        geom.systemId = cableInfo.system_id;
                        geom.longLat = latLong;
                        geom.userId = userId;
                        geom.entityType = EntityType.Cable.ToString();
                        geom.commonName = cableInfo.network_id;
                        geom.geomType = GeometryType.Line.ToString();
                        geom.project_id = cableInfo.project_id;
                        geom.networkStatus = String.IsNullOrEmpty(cableInfo.network_status) ? "P" : cableInfo.network_status;

                        string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                        RouteCreation routeObj = new DAMisc().createRouteId(CableResp.system_id, Models.EntityType.Cable.ToString());
                        DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(CableResp.system_id, Models.EntityType.Cable.ToString(), CableResp.province_id, 0);
                        //DbMessage geojsonObj = updateGeojsonMetadata(Models.EntityType.Cable.ToString(), CableResp.province_id);
                        DAIspLine.Instance.CreateOSPCable(cableInfo.system_id);
                    }
                    else
                    {
                        IspLineMaster objLine = new IspLineMaster();
                        objLine.entity_id = cableInfo.system_id;
                        objLine.entity_type = EntityType.Cable.ToString();
                        objLine.line_geom = cableInfo.ispLineGeom;
                        objLine.structure_id = cableInfo.structure_id;
                        objLine.created_by = userId;
                        objLine.created_on = DateTimeHelper.Now;
                        objLine.a_node_type = cableInfo.a_node_type;
                        objLine.b_node_type = cableInfo.b_node_type;
                        DAIspLine.Instance.saveLineGeom(objLine);
                    }

                    if (cableInfo.a_system_id > 0)
                    {
                        AssociateEntity assStartPt = new AssociateEntity();
                        assStartPt.associated_entity_type = cableInfo.a_entity_type;
                        assStartPt.associated_system_id = cableInfo.a_system_id;
                        assStartPt.associated_network_id = cableInfo.a_location;
                        assStartPt.entity_network_id = cableInfo.network_id;
                        assStartPt.entity_system_id = cableInfo.system_id;
                        assStartPt.entity_type = EntityType.Cable.ToString();
                        assStartPt.is_termination_point = true;
                        assStartPt.created_on = DateTimeHelper.Now;
                        assStartPt.created_by = userId;
                        new DAAssociateEntity().SaveAssociationForEndToEnd(assStartPt);
                    }
                    if (cableInfo.b_system_id > 0)
                    {
                        AssociateEntity assStartPt = new AssociateEntity();
                        assStartPt.associated_entity_type = cableInfo.b_entity_type;
                        assStartPt.associated_system_id = cableInfo.b_system_id;
                        assStartPt.associated_network_id = cableInfo.b_location;
                        assStartPt.entity_network_id = cableInfo.network_id;
                        assStartPt.entity_system_id = cableInfo.system_id;
                        assStartPt.entity_type = EntityType.Cable.ToString();
                        assStartPt.is_termination_point = true;
                        assStartPt.created_on = DateTimeHelper.Now;
                        assStartPt.created_by = userId;
                        new DAAssociateEntity().SaveAssociationForEndToEnd(assStartPt);
                    }
                    //  int systemId = int.Parse(cableInfo.a_system_id.ToString() + cableInfo.b_system_id.ToString());


                    if (!string.IsNullOrEmpty(cableInfo.a_entity_type) && !string.IsNullOrEmpty(cableInfo.b_entity_type) && String.Concat(cableInfo.a_entity_type.ToUpper(), (cableInfo.a_system_id)) != String.Concat(cableInfo.b_entity_type.ToUpper(), (cableInfo.b_system_id)))
                    {
                        AssociateEntity assEndPt = new AssociateEntity();
                        assEndPt.associated_entity_type = cableInfo.b_entity_type;
                        assEndPt.associated_system_id = cableInfo.b_system_id;
                        assEndPt.associated_network_id = cableInfo.b_location;
                        assEndPt.entity_network_id = cableInfo.network_id;
                        assEndPt.entity_system_id = cableInfo.system_id;
                        assEndPt.entity_type = EntityType.Cable.ToString();
                        assEndPt.is_termination_point = true;
                        assEndPt.created_on = DateTimeHelper.Now;
                        assEndPt.created_by = userId;
                        new DAAssociateEntity().SaveAssociationForEndToEnd(assEndPt);
                    }
                    if (cableInfo.pSystemId != 0)
                    {
                        AssociateEntity objAsso = new AssociateEntity();
                        objAsso.associated_entity_type = EntityType.Cable.ToString();
                        objAsso.associated_system_id = cableInfo.system_id;
                        objAsso.associated_network_id = cableInfo.network_id;
                        objAsso.entity_network_id = cableInfo.pNetworkId;
                        objAsso.entity_system_id = cableInfo.pSystemId;
                        objAsso.entity_type = cableInfo.pEntityType;
                        objAsso.created_on = DateTimeHelper.Now;
                        objAsso.created_by = userId;
                        new DAAssociateEntity().SaveAssociationForEndToEnd(objAsso);
                    }


                    DAIspLine.Instance.setCableEndPoint(cableInfo.system_id);
                    return CableResp;
                }

            }
            catch
            {
                throw;
            }
        }
        public List<CoreLogicSearchdetails> GetSearchResult(string searchText, string search_type)
        {

            try
            {
                return repo.ExecuteProcedure<CoreLogicSearchdetails>("fn_get_corelogicsearchdetails", new { p_searchText = searchText, p_searchType = search_type }, true);
            }
            catch { throw; }
        }
        public List<Fiberlinkdetails> GetFiberLinkDetails(string network_id)
        {

            try
            {
                return repo.ExecuteProcedure<Fiberlinkdetails>("fn_get_fiberdetails", new { network_id = network_id }, true);
            }
            catch { throw; }
        }
        public DbMessageConePlanLogic Validate(string odf1, string odf2, int require_core, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<DbMessageConePlanLogic>("fn_get_core_planner_validation", new { source_network_id = odf1, destination_network_id = odf2, buffer = 5, required_core = require_core, p_user_id = user_id }, true).FirstOrDefault();
                return res;
            }
            catch { throw; }
        }
        public List<CorePlannerLogs> GetCorePlanLogsByUserId(int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<CorePlannerLogs>("fn_get_core_planner_logs", new { p_user_id = user_id }, true);
                return res;
            }
            catch { throw; }
        }
        public DbMessageConePlanLogic SaveCorePlanLogic(string required_core, int user_id, string link_system_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessageConePlanLogic>("fn_get_core_planner_splicing", new { required_core = Convert.ToInt32(required_core), p_user_id = user_id, fiber_link_network_id = link_system_id }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<KeyValueDropDown> GetAllVendorType(string type)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_vendor_by_type", new { p_type = type });
                
            }
            catch { throw; }
        }

        public CableMaster getCableDetailbyvendorId(int venor_id, string eType)
        {
            try
            {
                var result = repo.GetById(m => m.vendor_id == venor_id);
                return result != null ? result : new CableMaster();
            }
            catch
            {
                throw;
            }
        }

        public CableMaster GetCableNameAndLengthForLoop(int CableId)
        {
            try
            {
                //var df = repo.GetAll();
                var result = repo.GetById(m => m.system_id == CableId);
                return result != null ? result : new CableMaster();
            }
            catch
            {
                throw;
            }
        }

        public DbMessage SetCableColorDetails(int cableId, int NoOfTube, int NoOfCore, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_set_cable_color_info", new { p_system_id = cableId, p_user_id = userId, p_tube_count = NoOfTube, p_core_count = NoOfCore }).FirstOrDefault();

            }
            catch { throw; }
        }
        public DbMessage SaveTubeCoreColor(string tubeColor, string CoreColor, int systemId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_update_tube_core_color", new { p_tube_color = tubeColor, p_core_color = CoreColor, p_system_id = systemId }).FirstOrDefault();

            }
            catch { throw; }
        }

        public string GetCableType(int system_id)
        {
            try
            {
                string cableType = string.Empty;
                var objCable = repo.Get(u => u.system_id == system_id);
                if (objCable != null)
                {
                    cableType = objCable.cable_type;
                }
                return cableType;
            }
            catch { throw; }
        }

        public EditLineTP EditCableTPDetail(EditLineTP objTPDetail, int userId)
        {
            try
            {
                var objCable = repo.Get(u => u.system_id == objTPDetail.system_id);
                if (objCable != null)
                {
                    //if ( String.Concat(objCable.b_entity_type, objCable.b_system_id) != String.Concat(objTPDetail.tpDetail[1].entity_type, objTPDetail.tpDetail[1].system_id)
                    //&& new DAConnectionInfo().checkIsLineEntitySpliced(objTPDetail.system_id,EntityType.Cable.ToString(),false))
                    //{
                    //    objTPDetail.message = "Cable Start point can not be change as it is spliced with some other entity!"; ;
                    //    return objTPDetail;
                    //}else if (String.Concat(objCable.a_entity_type, objCable.a_system_id) != String.Concat(objTPDetail.tpDetail[0].entity_type, objTPDetail.tpDetail[0].system_id)                    
                    //&& new DAConnectionInfo().checkIsLineEntitySpliced(objTPDetail.system_id, EntityType.Cable.ToString(),true))
                    //{
                    //    objTPDetail.message = "Cable End point can not be change as it is spliced with some other entity!"; ;
                    //    return objTPDetail;
                    //}


                    //PageMessage objPageValidate = new PageMessage();
                    //DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    //{
                    //    system_id = objTPDetail.system_id,
                    //    entity_type = EntityType.Cable.ToString(),                       
                    //    a_system_id = objTPDetail.tpDetail[0].system_id,
                    //    a_entity_type = objTPDetail.tpDetail[0].entity_type,
                    //    b_system_id = objTPDetail.tpDetail[1].system_id,
                    //    b_entity_type = objTPDetail.tpDetail[1].entity_type
                    //}, true);

                    //if (!string.IsNullOrEmpty(objMessage.message))
                    //{
                    //    objTPDetail.message = objMessage.message;
                    //    return objTPDetail;
                    //}


                    //if (!string.IsNullOrEmpty(objTPDetail.tpDetail[0].network_id))
                    //{
                    objCable.a_location = objTPDetail.tpDetail[0].network_id ?? "";
                    objCable.a_system_id = objTPDetail.tpDetail[0].system_id;
                    objCable.a_entity_type = objTPDetail.tpDetail[0].entity_type ?? "";
                    //}
                    //if (!string.IsNullOrEmpty(objTPDetail.tpDetail[1].network_id))
                    //{
                    objCable.b_location = objTPDetail.tpDetail[1].network_id ?? "";
                    objCable.b_system_id = objTPDetail.tpDetail[1].system_id;
                    objCable.b_entity_type = objTPDetail.tpDetail[1].entity_type ?? "";
                    //}
                    //var networkCodeDetail = new DAMisc().GetLineNetworkCode(objCable.a_location, objCable.b_location, EntityType.Cable.ToString(), objTPDetail.entityGeom, "OSP");
                    //if (!string.IsNullOrEmpty(networkCodeDetail.network_code))
                    //objCable.network_id = networkCodeDetail.network_code;
                    objCable.a_location_code = "A";
                    objCable.b_location_code = "B";
                    objCable.modified_on = DateTimeHelper.Now;
                    objCable.modified_by = userId;
                    objCable.bom_sub_category = objCable.bom_sub_category;

                    repo.Update(objCable);
                    new DAAssociateEntity().UpdateTPAssociation(objTPDetail.system_id, EntityType.Cable.ToString(), userId);
                    if (objCable.cable_type != "ISP")
                    {
                        DAIspLine.Instance.CreateOSPCable(objCable.system_id);
                    }
                }
                return objTPDetail;
            }
            catch { throw; }
        }

        public int DeleteCableById(int cable_Id)
        {
            int result = 0;
            try
            {
                var cableDetails = repo.Get(u => u.system_id == cable_Id);
                // var cableDetails = repo.Get(u => u.system_id == cable_Id && u.network_status == "P");
                if (cableDetails != null)
                {
                    result = repo.Delete(cableDetails.system_id);
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public int checkDuplicateDesignId(string design_id, int system_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<int>("fn_get_checkDuplicateDesignId", new { p_design_id = design_id,p_system_id= system_id }).FirstOrDefault();
                return lst;


            }
            catch { throw; }
        }
        public DbMessage SetConnectionWithSplitCable(string cable_one_network_id, string cable_two_network_id, int old_cable_system_id, int splitentitysystemid, string splitentity_network_id, string splitentitytype, int user_id, string splicing_source)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_save_osp_split_connection", new { p_cable_one_network_id = cable_one_network_id, p_cable_two_network_id = cable_two_network_id, p_old_cable_system_id = old_cable_system_id, p_split_entity_system_id = splitentitysystemid, p_split_entity_network_id = splitentity_network_id, p_split_entity_type = splitentitytype, p_user_id = user_id, p_splicing_source = splicing_source }).FirstOrDefault();
            }
            catch { throw; }
        }


        public DbMessage SetISPConnectionWithSplitCable(string cable_one_network_id, string cable_two_network_id, int old_cable_system_id, int splitentitysystemid, string splitentity_network_id, string splitentitytype, int user_id, string splicing_source, int split_entity_x, int split_entity_y)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_splicing_save_isp_split_connection", new { p_cable_one_network_id = cable_one_network_id, p_cable_two_network_id = cable_two_network_id, p_old_cable_system_id = old_cable_system_id, p_split_entity_system_id = splitentitysystemid, p_split_entity_network_id = splitentity_network_id, p_split_entity_type = splitentitytype, p_user_id = user_id, p_splicing_source = splicing_source, p_split_entity_x = split_entity_x, p_split_entity_y = split_entity_y }).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<TubeCoreInfo> GetTubeCoreInfo(int cableId)
        {
            try
            {
                var lst = repo.ExecuteProcedure<TubeCoreInfo>("fn_get_cable_tubecore_info", new { p_cableid = cableId });
                return lst;


            }
            catch { throw; }
        }


        public OffsetGeometry getCableGeom(int systemId, double offset)
        {
            try
            {
                return repo.ExecuteProcedure<OffsetGeometry>("fn_get_cableGeom", new { p_cableid = systemId, p_offset = offset }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<CableFiberDetail> GetCableFiberDetail(int cableId)
        {
            try
            {
                var lst = repo.ExecuteProcedure<CableFiberDetail>("fn_get_cable_fiber_detail", new { p_cableid = cableId });
                return lst;


            }
            catch { throw; }
        }



        public string getReservedUtilization(string cableID)
        {
            try
            {

                var value = repo.ExecuteProcedure<string>("fn_getReservedUtilization", new { p_system_id = cableID }).FirstOrDefault();

                return value;
                //return value != null && value.Count > 0 ? value[0].ToString() : "0";

            }
            catch { throw; }


        }




        public string getConnectivityUtilization(string cableID)
        {

            try
            {

                var value = repo.ExecuteProcedure<string>("fn_getconnectedutilization", new { p_system_id = cableID }).FirstOrDefault();

                return value;
                //return value != null && value.Count > 0 ? value[0].ToString() : "0";

            }
            catch { throw; }


        }

        public CableMaster updateCableType(int system_id, int user_id)
        {
            //CableMaster objUpdatedCableDetail = new CableMaster();
            try
            {
                var objCable = repo.Get(u => u.system_id == system_id);
                var cable_type = objCable.cable_type == "Underground" ? "Overhead" : "Underground";
                if (objCable != null)
                {
                    objCable.cable_type = cable_type;
                    objCable.modified_on = DateTimeHelper.Now;
                    objCable.modified_by = user_id;
                    return repo.Update(objCable);
                }
                return new CableMaster();
            }
            catch (Exception)
            { throw; }

        }

        public int getTotalOSPCable(int structureId)
        {
            return repo.ExecuteProcedure<int>("fn_get_total_osp_cable", new { p_structure_id = structureId }).FirstOrDefault();
        }

        public int GetAvailableCores(int cableId)
        {
            try
            {
                return repo.ExecuteProcedure<int>("fn_sf_get_available_cores", new { p_cableid = cableId }).FirstOrDefault();
            }
            catch { throw; }
        }

        // Merge Cable Module
        public List<CableMergeStatus> CompleteMergecableOperation(int MasterCableId,int SecondCableId,int user_id)
        {
            try
            {
                var MergeStatus = repo.ExecuteProcedure<CableMergeStatus>("fn_merge_cable", new {
                    mastercableid = MasterCableId,
                    secondcableid = SecondCableId,
                    user_id = user_id
                }, true);
                return MergeStatus;

                
            }
            catch
            {
                throw;
            }
        }
        //public DbMessage SetCableColorDetails(int cableId, int NoOfTube, int NoOfCore, int userId)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<DbMessage>("fn_set_cable_color_info", new { p_system_id = cableId, p_user_id = userId, p_tube_count = NoOfTube, p_core_count = NoOfCore }).FirstOrDefault();

        //    }
        //    catch { throw; }
        //}
        #region Additional-attributes
        public string GetOtherInfoCable(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
        
    }
    public class DAIspLine : Repository<IspLineMaster>
    {
        private static DAIspLine objLine = null;
        private static readonly object lockObject = new object();
        public static DAIspLine Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objLine == null)
                    {
                        objLine = new DAIspLine();
                    }
                }
                return objLine;
            }
        }
        public bool UpdateLineGeom(IspLineMaster Line)
        {
            try
            {
                var linegeomDetail = repo.Get(u => u.entity_id == Line.entity_id && u.structure_id == Line.structure_id && u.entity_type == EntityType.Cable.ToString());
                if (linegeomDetail != null)
                {
                    linegeomDetail.line_geom = Line.line_geom;
                    linegeomDetail.a_node_type = Line.a_node_type;
                    linegeomDetail.b_node_type = Line.b_node_type;
                    linegeomDetail.modified_on = DateTimeHelper.Now;
                    linegeomDetail.modified_by = Line.modified_by;
                    repo.Update(linegeomDetail);
                }
                return true;
            }
            catch { throw; }

        }
        public IspLineMaster saveLineGeom(IspLineMaster Line)
        {
            try
            {
                Line.cable_type = "ISP";
                var linegeomDetails = repo.Get(u => u.entity_id == Line.entity_id && u.entity_type == Line.entity_type);
                if (linegeomDetails != null)
                {
                    linegeomDetails.line_geom = Line.line_geom;
                    linegeomDetails.modified_on = DateTimeHelper.Now;
                    linegeomDetails.structure_id = Line.structure_id;
                    linegeomDetails.modified_by = Line.modified_by;
                    linegeomDetails.entity_type = Line.entity_type;
                    return repo.Update(linegeomDetails);
                }
                else
                {
                    //return repo.Insert(Line);
                   repo.ExecuteProcedure("fn_insert_isp_line_master", new {
                        p_entity_id = Line.entity_id,
                        p_entity_type = Line.entity_type,
                        p_cable_type = Line.cable_type,
                        p_line_geom = Line.line_geom,
                        p_created_by = Line.created_by,
                        p_structure_id = Line.structure_id,
                        p_a_node_type = Line.a_node_type,
                        p_b_node_type = Line.b_node_type,
                        p_geom_source = Line.geom_source,
                    });
                    return Line; 

                }
            }
            catch { throw; }

        }
        public bool updateOSPISPLineGeom(List<OSPISPCable> listLine)
        {
            try
            {
                foreach (var item in listLine)
                {
                    var lineDetails = repo.GetAll(m => m.structure_id == item.StructureId && m.entity_id == item.system_id && m.entity_type == EntityType.Cable.ToString()).FirstOrDefault();
                    if (lineDetails != null)
                    {
                        lineDetails.line_geom = item.line_geom;
                        lineDetails.geom_source = item.geom_source;
                        if (!string.IsNullOrEmpty(item.a_node_type))
                            lineDetails.a_node_type = item.a_node_type;
                        if (!string.IsNullOrEmpty(item.b_node_type))
                            lineDetails.b_node_type = item.b_node_type;
                    }
                    repo.Update(lineDetails);
                }
                return true;
            }
            catch { throw; }

        }
        public IspLineMaster getLinegeom(int cableId)
        {
            try
            {
                return repo.Get(u => u.entity_id == cableId);
            }
            catch { throw; }
        }

        public DbMessage isIspLineExists(string shaftList, int systemId, int floorCount, int shaftCount)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_check_isp_cable_exist", new { p_shaftList = shaftList, p_system_id = systemId, p_floor_count = floorCount, p_shaft_count = shaftCount }).FirstOrDefault();
            }
            catch { throw; }
        }
        public bool checkIspLine(int systemId)
        {
            try
            {
                var ispLine = repo.GetAll(m => m.structure_id == systemId && m.entity_type != null && m.entity_type.ToUpper() == "CABLE").ToList();
                if (ispLine.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { throw; }
        }

        public void CreateOSPCable(int cableSystemId)
        {

            repo.ExecuteProcedure<object>("fn_isp_create_OSP_Cable", new
            {
                p_cable_system_id = cableSystemId
            });
        }

        public void setCableEndPoint(int cableSystemId)
        {

            repo.ExecuteProcedure<object>("fn_cable_set_end_point", new
            {
                p_cable_system_id = cableSystemId
            });
        }

        public DbMessage UpdateCablesPath(string cableData)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_isp_snap_cable_endpoints", new { p_connections = cableData }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        
    }
    public class DATempCable : Repository<TempCable>
    {

        public void SaveBulkTempCable(List<TempCable> lstTempCable)
        {
            repo.Insert(lstTempCable);
        }

        public void InsertCableIntoMainTable(UploadSummary summary)
        {
            repo.ExecuteSQLCommand(string.Format("select * from fn_bulkupload_cable({0},{1})", summary.id, "''"));
        }
    }
    public class DACDBAttribute : Repository<CDBAttribute>
    {
        private static DACDBAttribute objLine = null;
        private static readonly object lockObject = new object();
        public static DACDBAttribute Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objLine == null)
                    {
                        objLine = new DACDBAttribute();
                    }
                }
                return objLine;
            }
        }
        public void SaveCDBAttribute(CableMaster cableInfo)
        {
            CDBAttribute cdbAttribute = new CDBAttribute();
            cdbAttribute.cable_id = cableInfo.system_id;
            cdbAttribute.circle_name = cableInfo.LstCDBAttribute.circle_name;
            cdbAttribute.major_route_name = cableInfo.LstCDBAttribute.major_route_name;
            cdbAttribute.route_id = cableInfo.LstCDBAttribute.route_id;
            cdbAttribute.section_name = cableInfo.LstCDBAttribute.section_name;
            cdbAttribute.section_id = cableInfo.LstCDBAttribute.section_id;
            cdbAttribute.route_type = cableInfo.LstCDBAttribute.route_type;
            cdbAttribute.operator_type = cableInfo.LstCDBAttribute.operator_type;
            cdbAttribute.route_category = cableInfo.LstCDBAttribute.route_category;
            cdbAttribute.distance = cableInfo.LstCDBAttribute.distance;
            cdbAttribute.fiber_type = cableInfo.LstCDBAttribute.fiber_type;
            cdbAttribute.fiber_pairs_laid = cableInfo.LstCDBAttribute.fiber_pairs_laid;
            cdbAttribute.total_used_pair = cableInfo.LstCDBAttribute.total_used_pair;
            cdbAttribute.fiber_pairs_used_by_vil = cableInfo.LstCDBAttribute.fiber_pairs_used_by_vil;
            cdbAttribute.fiber_pairs_given_to_airtel = cableInfo.LstCDBAttribute.fiber_pairs_given_to_airtel;
            cdbAttribute.fiber_pairs_given_to_others = cableInfo.LstCDBAttribute.fiber_pairs_given_to_others;
            cdbAttribute.fiber_pairs_free = cableInfo.LstCDBAttribute.fiber_pairs_free;
            cdbAttribute.faulty_fiber_pairs = cableInfo.LstCDBAttribute.faulty_fiber_pairs;
            cdbAttribute.start_latitude = cableInfo.LstCDBAttribute.start_latitude;
            cdbAttribute.start_longitude = cableInfo.LstCDBAttribute.start_longitude;
            cdbAttribute.end_latitude = cableInfo.LstCDBAttribute.end_latitude;
            cdbAttribute.end_longitude = cableInfo.LstCDBAttribute.end_longitude;
            cdbAttribute.count_non_vil_tenancies_on_route = cableInfo.LstCDBAttribute.count_non_vil_tenancies_on_route;
            cdbAttribute.route_lit_up_date = Convert.ToDateTime(cableInfo.LstCDBAttribute.route_lit_up_date);
            cdbAttribute.aerial_km = cableInfo.LstCDBAttribute.aerial_km;
            cdbAttribute.avg_loss_per_km = cableInfo.LstCDBAttribute.avg_loss_per_km;
            cdbAttribute.avg_last_six_months_fiber_cut = cableInfo.LstCDBAttribute.avg_last_six_months_fiber_cut;
            cdbAttribute.row = cableInfo.LstCDBAttribute.row;
            cdbAttribute.material = cableInfo.LstCDBAttribute.material;
            cdbAttribute.execution = cableInfo.LstCDBAttribute.execution;
            cdbAttribute.row_availablity = cableInfo.LstCDBAttribute.row_availablity;
            cdbAttribute.iru_given_airtel = cableInfo.LstCDBAttribute.iru_given_airtel;
            cdbAttribute.iru_given_jio = cableInfo.LstCDBAttribute.iru_given_jio;
            cdbAttribute.iru_given_ttsl_or_ttml = cableInfo.LstCDBAttribute.iru_given_ttsl_or_ttml;
            cdbAttribute.iru_given_tcl = cableInfo.LstCDBAttribute.iru_given_tcl;
            cdbAttribute.iru_given_others = cableInfo.LstCDBAttribute.iru_given_others;
            cdbAttribute.network_category = cableInfo.LstCDBAttribute.network_category;
            cdbAttribute.row_valid_or_exp = Convert.ToDateTime(cableInfo.LstCDBAttribute.row_valid_or_exp);
            cdbAttribute.remarks = cableInfo.LstCDBAttribute.remarks;
            cdbAttribute.cable_owner = cableInfo.LstCDBAttribute.cable_owner;
            repo.Insert(cdbAttribute);
        }
        public void UpdateCDBAttribute(CDBAttribute objcdb, CableMaster cableInfo)
        {
            objcdb.circle_name = cableInfo.LstCDBAttribute.circle_name;
            objcdb.major_route_name = cableInfo.LstCDBAttribute.major_route_name;
            objcdb.route_id = cableInfo.LstCDBAttribute.route_id;
            objcdb.section_name = cableInfo.LstCDBAttribute.section_name;
            objcdb.section_id = cableInfo.LstCDBAttribute.section_id;
            objcdb.route_type = cableInfo.LstCDBAttribute.route_type;
            objcdb.operator_type = cableInfo.LstCDBAttribute.operator_type;
            objcdb.route_category = cableInfo.LstCDBAttribute.route_category;
            objcdb.distance = cableInfo.LstCDBAttribute.distance;
            objcdb.fiber_type = cableInfo.LstCDBAttribute.fiber_type;
            objcdb.fiber_pairs_laid = cableInfo.LstCDBAttribute.fiber_pairs_laid;
            objcdb.total_used_pair = cableInfo.LstCDBAttribute.total_used_pair;
            objcdb.fiber_pairs_used_by_vil = cableInfo.LstCDBAttribute.fiber_pairs_used_by_vil;
            objcdb.fiber_pairs_given_to_airtel = cableInfo.LstCDBAttribute.fiber_pairs_given_to_airtel;
            objcdb.fiber_pairs_given_to_others = cableInfo.LstCDBAttribute.fiber_pairs_given_to_others;
            objcdb.fiber_pairs_free = cableInfo.LstCDBAttribute.fiber_pairs_free;
            objcdb.faulty_fiber_pairs = cableInfo.LstCDBAttribute.faulty_fiber_pairs;
            objcdb.start_latitude = cableInfo.LstCDBAttribute.start_latitude;
            objcdb.start_longitude = cableInfo.LstCDBAttribute.start_longitude;
            objcdb.end_latitude = cableInfo.LstCDBAttribute.end_latitude;
            objcdb.end_longitude = cableInfo.LstCDBAttribute.end_longitude;
            objcdb.count_non_vil_tenancies_on_route = cableInfo.LstCDBAttribute.count_non_vil_tenancies_on_route;
            objcdb.route_lit_up_date = cableInfo.LstCDBAttribute.route_lit_up_date;
            objcdb.aerial_km = cableInfo.LstCDBAttribute.aerial_km;
            objcdb.avg_loss_per_km = cableInfo.LstCDBAttribute.avg_loss_per_km;
            objcdb.avg_last_six_months_fiber_cut = cableInfo.LstCDBAttribute.avg_last_six_months_fiber_cut;
            objcdb.row = cableInfo.LstCDBAttribute.row;
            objcdb.material = cableInfo.LstCDBAttribute.material;
            objcdb.execution = cableInfo.LstCDBAttribute.execution;
            objcdb.row_availablity = cableInfo.LstCDBAttribute.row_availablity;
            objcdb.iru_given_airtel = cableInfo.LstCDBAttribute.iru_given_airtel;
            objcdb.iru_given_jio = cableInfo.LstCDBAttribute.iru_given_jio;
            objcdb.iru_given_ttsl_or_ttml = cableInfo.LstCDBAttribute.iru_given_ttsl_or_ttml;
            objcdb.iru_given_tcl = cableInfo.LstCDBAttribute.iru_given_tcl;
            objcdb.iru_given_others = cableInfo.LstCDBAttribute.iru_given_others;
            objcdb.network_category = cableInfo.LstCDBAttribute.network_category;
            objcdb.row_valid_or_exp = cableInfo.LstCDBAttribute.row_valid_or_exp;
            objcdb.remarks = cableInfo.LstCDBAttribute.remarks;
            objcdb.cable_owner = cableInfo.LstCDBAttribute.cable_owner;
            repo.Update(objcdb);
        }
        public CDBAttribute Get(int system_id)
        {
            return repo.Get(u => u.cable_id == system_id);
        }
    }
}
