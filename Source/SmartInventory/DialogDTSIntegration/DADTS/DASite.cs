using BusinessLogics;
using DataAccess;
using DialogDTSIntegration.BLDTS;
using Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DialogDTSIntegration.DADTS
{
    public class DASite
    {
        private string connectionString = ConfigurationManager.AppSettings["constr"];
        public Site Save(Site site, int userId)
        {
            //string connectionString = ConfigurationManager.AppSettings["constr"];
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connected to PostgreSQL Server");

                    // Check if the site already exists
                    var query = "SELECT * FROM att_details_site WHERE site_id = @site_id";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@site_id", site.site_id);
                        using (NpgsqlDataReader dr = command.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                
                                // Existing site found, perform update
                                var objSite = new Site
                                {
                                    system_id = dr.GetInt32(dr.GetOrdinal("system_id")),
                                    network_status = dr.GetString(dr.GetOrdinal("network_status")),
                                    site_id = site.site_id != null ? site.site_id: (dr.IsDBNull(dr.GetOrdinal("site_id")) ? null : dr.GetString(dr.GetOrdinal("site_id"))),
                                    address = site.address != null ? site.address: (dr.IsDBNull(dr.GetOrdinal("address")) ? null : dr.GetString(dr.GetOrdinal("address"))),
                                    site_name = site.site_name != null ? site.site_name : (dr.IsDBNull(dr.GetOrdinal("site_name")) ? null : dr.GetString(dr.GetOrdinal("site_name"))),
                                    on_air_date = site.on_air_date == null ? dr.GetDateTime(dr.GetOrdinal("on_air_date")): site.on_air_date,
                                    removed_date = site.removed_date == null ? (DateTime)(dr.IsDBNull(dr.GetOrdinal("removed_date")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("removed_date"))) : site.removed_date,
                                    tx_type = site.tx_type != null ? site.tx_type : (dr.IsDBNull(dr.GetOrdinal("tx_type")) ? null : dr.GetString(dr.GetOrdinal("tx_type"))),
                                    tx_technology = site.tx_technology != null ? site.tx_technology : (dr.IsDBNull(dr.GetOrdinal("tx_technology")) ? null : dr.GetString(dr.GetOrdinal("tx_technology"))),
                                    tx_segment = site.tx_segment != null ? site.tx_segment : (dr.IsDBNull(dr.GetOrdinal("tx_segment")) ? null : dr.GetString(dr.GetOrdinal("tx_segment"))),
                                    tx_ring = site.tx_ring != null ? site.tx_ring : (dr.IsDBNull(dr.GetOrdinal("tx_ring")) ? null : dr.GetString(dr.GetOrdinal("tx_ring"))),
                                    region = site.region != null ? site.region : (dr.IsDBNull(dr.GetOrdinal("region")) ? null : dr.GetString(dr.GetOrdinal("region"))),
                                    province = site.province != null ? site.province : (dr.IsDBNull(dr.GetOrdinal("province")) ? null : dr.GetString(dr.GetOrdinal("province"))),
                                    district = site.district != null ? site.district : (dr.IsDBNull(dr.GetOrdinal("district")) ? null : dr.GetString(dr.GetOrdinal("district"))),
                                    region_address = site.region_address != null ? site.region_address : (dr.IsDBNull(dr.GetOrdinal("region_address")) ? null : dr.GetString(dr.GetOrdinal("region_address"))),
                                    depot = site.depot != null ? site.depot : (dr.IsDBNull(dr.GetOrdinal("depot")) ? null : dr.GetString(dr.GetOrdinal("depot"))),
                                    ds_division = site.ds_division != null ? site.ds_division : (dr.IsDBNull(dr.GetOrdinal("ds_division")) ? null : dr.GetString(dr.GetOrdinal("ds_division"))),
                                    local_authority = site.local_authority != null ? site.local_authority : (dr.IsDBNull(dr.GetOrdinal("local_authority")) ? null : dr.GetString(dr.GetOrdinal("local_authority"))),
                                    latitude = site.latitude == 0 ? dr.GetDouble(dr.GetOrdinal("latitude")) : site.latitude,
                                    longitude = site.longitude == 0 ? dr.GetDouble(dr.GetOrdinal("longitude")) : site.longitude,
                                    owner_name = site.owner_name != null ? site.owner_name : (dr.IsDBNull(dr.GetOrdinal("owner_name")) ? null : dr.GetString(dr.GetOrdinal("owner_name"))),
                                    access_24_7 = site.access_24_7 != null ? site.access_24_7 : (dr.IsDBNull(dr.GetOrdinal("access_24_7")) ? null : dr.GetString(dr.GetOrdinal("access_24_7"))),
                                    tower_type = site.tower_type != null ? site.tower_type : (dr.IsDBNull(dr.GetOrdinal("tower_type")) ? null : dr.GetString(dr.GetOrdinal("tower_type"))),
                                    cabinet_type = site.cabinet_type != null ? site.cabinet_type : (dr.IsDBNull(dr.GetOrdinal("cabinet_type")) ? null : dr.GetString(dr.GetOrdinal("cabinet_type"))),
                                    solution_type = site.solution_type != null ? site.solution_type : (dr.IsDBNull(dr.GetOrdinal("solution_type")) ? null : dr.GetString(dr.GetOrdinal("solution_type"))),
                                    agg_01 = site.agg_01 != null ? site.agg_01 : (dr.IsDBNull(dr.GetOrdinal("agg_01")) ? null : dr.GetString(dr.GetOrdinal("agg_01"))),
                                    agg_02 = site.agg_02 != null ? site.agg_02 : (dr.IsDBNull(dr.GetOrdinal("agg_02")) ? null : dr.GetString(dr.GetOrdinal("agg_02"))),
                                    ring_type = site.ring_type != null ? site.ring_type : (dr.IsDBNull(dr.GetOrdinal("ring_type")) ? null : dr.GetString(dr.GetOrdinal("ring_type"))),
                                    link_id = site.link_id != null ? site.link_id : (dr.IsDBNull(dr.GetOrdinal("link_id")) ? null : dr.GetString(dr.GetOrdinal("link_id"))),
                                    alias_name = site.alias_name != null ? site.alias_name : (dr.IsDBNull(dr.GetOrdinal("alias_name")) ? null : dr.GetString(dr.GetOrdinal("alias_name"))),
                                    tx_agg = site.tx_agg != null ? site.tx_agg : (dr.IsDBNull(dr.GetOrdinal("tx_agg")) ? null : dr.GetString(dr.GetOrdinal("tx_agg"))),
                                    bh_status = site.bh_status != null ? site.bh_status : (dr.IsDBNull(dr.GetOrdinal("bh_status")) ? null : dr.GetString(dr.GetOrdinal("bh_status"))),
                                    elevation = site.elevation != null ? site.elevation : (dr.IsDBNull(dr.GetOrdinal("elevation")) ? null : dr.GetString(dr.GetOrdinal("elevation"))),
                                    segment = site.segment != null ? site.segment : (dr.IsDBNull(dr.GetOrdinal("segment")) ? null : dr.GetString(dr.GetOrdinal("segment"))),
                                    ring = site.ring != null ? site.ring : (dr.IsDBNull(dr.GetOrdinal("ring")) ? null : dr.GetString(dr.GetOrdinal("ring"))),
                                    project_category = site.project_category != null ? site.project_category : (dr.IsDBNull(dr.GetOrdinal("project_category")) ? null : dr.GetString(dr.GetOrdinal("project_category"))),
                                    fiber_link_type = site.fiber_link_type != null ? site.fiber_link_type : (dr.IsDBNull(dr.GetOrdinal("fiber_link_type")) ? null : dr.GetString(dr.GetOrdinal("fiber_link_type"))),
                                    comment = site.comment != null ? site.comment : (dr.IsDBNull(dr.GetOrdinal("comment")) ? null : dr.GetString(dr.GetOrdinal("comment"))),
                                    port_type = site.port_type != null ? site.port_type : (dr.IsDBNull(dr.GetOrdinal("port_type")) ? null : dr.GetString(dr.GetOrdinal("port_type"))),
                                    destination_site_id = site.destination_site_id != null ? site.destination_site_id : (dr.IsDBNull(dr.GetOrdinal("destination_site_id")) ? null : dr.GetString(dr.GetOrdinal("destination_site_id"))),
                                    destination_port_type = site.destination_port_type != null ? site.destination_port_type : (dr.IsDBNull(dr.GetOrdinal("destination_port_type")) ? null : dr.GetString(dr.GetOrdinal("destination_port_type"))),
                                    project_id_dialog = site.project_id_dialog != null ? site.project_id_dialog : (dr.IsDBNull(dr.GetOrdinal("project_id_dialog")) ? null : dr.GetString(dr.GetOrdinal("project_id_dialog"))),
                                    modified_by = userId,
                                    modified_on = DateTime.Now,
                                    site_rank = site.site_rank==0 ? dr.GetInt32(dr.GetOrdinal("site_rank")):site.site_rank,
                                    self_tx_traffic = site.self_tx_traffic==0? dr.GetDecimal(dr.GetOrdinal("self_tx_traffic")):site.self_tx_traffic,
                                    agg_tx_traffic = site.agg_tx_traffic ==0 ?dr.GetDecimal(dr.GetOrdinal("agg_tx_traffic")):site.agg_tx_traffic,
                                    metro_ring_utilization = site.metro_ring_utilization ==0 ? dr.GetDecimal(dr.GetOrdinal("metro_ring_utilization")):site.metro_ring_utilization,
                                    csr_count = site.csr_count == 0 ? dr.GetInt32(dr.GetOrdinal("csr_count")):site.csr_count,
                                    dti_circuit = site.dti_circuit==0 ?dr.GetInt32(dr.GetOrdinal("dti_circuit")):site.dti_circuit,
                                    bandwidth = site.bandwidth==0?dr.GetInt32(dr.GetOrdinal("bandwidth")):site.bandwidth,
                                    project_id = site.project_id==0?dr.GetInt32(dr.GetOrdinal("project_id")):site.project_id,
                                    planning_id =site.planning_id == 0 ? dr.GetInt32(dr.GetOrdinal("planning_id")) : site.planning_id,
                                    purpose_id = site.purpose_id ==0 ?dr.GetInt32(dr.GetOrdinal("purpose_id")):site.purpose_id,
                                    workorder_id = site.workorder_id==0? dr.GetInt32(dr.GetOrdinal("workorder_id")):site.workorder_id,
                                    maximum_cost = site.maximum_cost == 0 ? dr.GetInt32(dr.GetOrdinal("maximum_cost")) : site.maximum_cost,
                                    priority = site.priority==0? dr.GetInt32(dr.GetOrdinal("priority")):site.priority,
                                    no_of_cores =site.no_of_cores==0? dr.GetInt32(dr.GetOrdinal("no_of_cores")):site.no_of_cores,
                                    plan_cost = site.plan_cost==0?dr.GetInt32(dr.GetOrdinal("plan_cost")):site.plan_cost,
                                    fiber_distance = site.fiber_distance==0?dr.GetInt32(dr.GetOrdinal("fiber_distance")):site.fiber_distance,
                                    fiber_link_code = site.fiber_link_code != null ? site.fiber_link_code : (dr.IsDBNull(dr.GetOrdinal("fiber_link_code")) ? null : dr.GetString(dr.GetOrdinal("fiber_link_code"))),
                                    destination_no_of_cores = site.destination_no_of_cores==0?dr.GetDecimal(dr.GetOrdinal("destination_no_of_cores")):site.destination_no_of_cores,
                                };

                                // Validate and check modified date
                                //PageMessage objPageValidate = DAUtility.ValidateModifiedDate(site.modified_on, objSite.modified_on, site.modified_by, objSite.modified_by);
                                //if (objPageValidate.message != null)
                                //{
                                //    site.objPM = objPageValidate;
                                //    return site;
                                //}
                                dr.Close();
                                // Update the existing site
                                var updateQuery = "UPDATE att_details_site SET address = @address, site_name = @siteName, " +
                                                  "on_air_date = @onAirDate, removed_date = @removedDate, tx_type = @txType, " +
                                                  "tx_technology = @txTechnology, tx_segment = @txSegment, tx_ring = @txRing, " +
                                                  "region = @region, province = @province, district = @district, " +
                                                  "region_address = @regionAddress, depot = @depot, ds_division = @dsDivision, " +
                                                  "local_authority = @localAuthority, latitude = @latitude, longitude = @longitude, " +
                                                  "owner_name = @ownerName, modified_by = @modifiedBy, modified_on = @modifiedOn, " +
                                                  "access_24_7 = @access247, tower_type = @towerType, tower_height = @towerHeight, " +
                                                  "cabinet_type = @cabinetType, solution_type = @solutionType, site_rank = @siteRank, " +
                                                  "self_tx_traffic = @selfTxTraffic, agg_tx_traffic = @aggTxTraffic, " +
                                                  "metro_ring_utilization = @metroRingUtilization, csr_count = @csrCount, " +
                                                  "dti_circuit = @dtiCircuit, agg_01 = @agg01, agg_02 = @agg02, bandwidth = @bandwidth, " +
                                                  "ring_type = @ringType, link_id = @linkId, alias_name = @aliasName, " +
                                                  "project_id = @projectId, planning_id = @planningId, purpose_id = @purposeId, " +
                                                  "workorder_id = @workorderId, tx_agg = @txAgg, bh_status = @bhStatus, " +
                                                  "elevation = @elevation, segment = @segment, ring = @ring, " +
                                                  "maximum_cost = @maximumCost, project_category = @projectCategory, priority = @priority, " +
                                                  "no_of_cores = @noOfCores, fiber_link_type = @fiberLinkType, comment = @comment, " +
                                                  "plan_cost = @planCost, fiber_distance = @fiberDistance, fiber_link_code = @fiberLinkCode, " +
                                                  "port_type = @portType, destination_site_id = @destinationSiteId, " +
                                                  "destination_port_type = @destinationPortType, destination_no_of_cores = @destinationNoOfCores, " +
                                                  "project_id_dialog = @projectIdDialog WHERE site_id = @siteId";

                                using (var updateCommand = new NpgsqlCommand(updateQuery, conn))
                                {
                                    updateCommand.Parameters.AddWithValue("@siteId", site.site_id);
                                    updateCommand.Parameters.AddWithValue("@address", objSite.address);
                                    updateCommand.Parameters.AddWithValue("@siteName", objSite.site_name);
                                    updateCommand.Parameters.AddWithValue("@onAirDate", objSite.on_air_date);
                                    updateCommand.Parameters.AddWithValue("@removedDate", (object)objSite.removed_date ?? DBNull.Value);
                                    updateCommand.Parameters.AddWithValue("@txType", objSite.tx_type);
                                    updateCommand.Parameters.AddWithValue("@txTechnology", objSite.tx_technology);
                                    updateCommand.Parameters.AddWithValue("@txSegment", objSite.tx_segment);
                                    updateCommand.Parameters.AddWithValue("@txRing", objSite.tx_ring);
                                    updateCommand.Parameters.AddWithValue("@region", objSite.region);
                                    updateCommand.Parameters.AddWithValue("@province", objSite.province);
                                    updateCommand.Parameters.AddWithValue("@district", objSite.district);
                                    updateCommand.Parameters.AddWithValue("@regionAddress", objSite.region_address);
                                    updateCommand.Parameters.AddWithValue("@depot", objSite.depot);
                                    updateCommand.Parameters.AddWithValue("@dsDivision", objSite.ds_division);
                                    updateCommand.Parameters.AddWithValue("@localAuthority", objSite.local_authority);
                                    if (objSite.network_status == "A")
                                    {
                                        //updateCommand.Parameters.AddWithValue("@latitude", site.latitude);
                                       // updateCommand.Parameters.AddWithValue("@latitude", site.latitude);
                                    }
                                    else
                                    {
                                        updateCommand.Parameters.AddWithValue("@latitude", objSite.latitude);
                                        updateCommand.Parameters.AddWithValue("@latitude", objSite.latitude);
                                    }
                                    //updateCommand.Parameters.AddWithValue("@latitude", objSite.latitude);
                                    //updateCommand.Parameters.AddWithValue("@longitude", objSite.longitude);
                                    updateCommand.Parameters.AddWithValue("@ownerName", objSite.owner_name);
                                    updateCommand.Parameters.AddWithValue("@modifiedBy", userId);
                                    updateCommand.Parameters.AddWithValue("@modifiedOn", DateTime.Now);
                                    updateCommand.Parameters.AddWithValue("@access247", objSite.access_24_7);
                                    updateCommand.Parameters.AddWithValue("@towerType", objSite.tower_type);
                                    updateCommand.Parameters.AddWithValue("@towerHeight", objSite.tower_height);
                                    updateCommand.Parameters.AddWithValue("@cabinetType", objSite.cabinet_type);
                                    updateCommand.Parameters.AddWithValue("@solutionType", objSite.solution_type);
                                    updateCommand.Parameters.AddWithValue("@siteRank", objSite.site_rank);
                                    updateCommand.Parameters.AddWithValue("@selfTxTraffic", objSite.self_tx_traffic);
                                    updateCommand.Parameters.AddWithValue("@aggTxTraffic", objSite.agg_tx_traffic);
                                    updateCommand.Parameters.AddWithValue("@metroRingUtilization", objSite.metro_ring_utilization);
                                    updateCommand.Parameters.AddWithValue("@csrCount", objSite.csr_count);
                                    updateCommand.Parameters.AddWithValue("@dtiCircuit", objSite.dti_circuit);
                                    updateCommand.Parameters.AddWithValue("@agg01", objSite.agg_01);
                                    updateCommand.Parameters.AddWithValue("@agg02", objSite.agg_02);
                                    updateCommand.Parameters.AddWithValue("@bandwidth", objSite.bandwidth);
                                    updateCommand.Parameters.AddWithValue("@ringType", objSite.ring_type);
                                    updateCommand.Parameters.AddWithValue("@linkId", objSite.link_id);
                                    updateCommand.Parameters.AddWithValue("@aliasName", objSite.alias_name);
                                    updateCommand.Parameters.AddWithValue("@projectId", objSite.project_id);
                                    updateCommand.Parameters.AddWithValue("@planningId", objSite.planning_id);
                                    updateCommand.Parameters.AddWithValue("@purposeId", objSite.purpose_id);
                                    updateCommand.Parameters.AddWithValue("@workorderId", objSite.workorder_id);
                                    updateCommand.Parameters.AddWithValue("@txAgg", objSite.tx_agg);
                                    updateCommand.Parameters.AddWithValue("@bhStatus", objSite.bh_status);
                                    updateCommand.Parameters.AddWithValue("@elevation", objSite.elevation);
                                    updateCommand.Parameters.AddWithValue("@segment", objSite.segment);
                                    updateCommand.Parameters.AddWithValue("@ring", objSite.ring);
                                    updateCommand.Parameters.AddWithValue("@maximumCost", objSite.maximum_cost);
                                    updateCommand.Parameters.AddWithValue("@projectCategory", objSite.project_category);
                                    updateCommand.Parameters.AddWithValue("@priority", objSite.priority);
                                    updateCommand.Parameters.AddWithValue("@noOfCores", objSite.no_of_cores);
                                    updateCommand.Parameters.AddWithValue("@fiberLinkType", objSite.fiber_link_type);
                                    updateCommand.Parameters.AddWithValue("@comment", objSite.comment);
                                    updateCommand.Parameters.AddWithValue("@planCost", objSite.plan_cost);
                                    updateCommand.Parameters.AddWithValue("@fiberDistance", objSite.fiber_distance);
                                    updateCommand.Parameters.AddWithValue("@fiberLinkCode", objSite.fiber_link_code);
                                    updateCommand.Parameters.AddWithValue("@portType", objSite.port_type);
                                    updateCommand.Parameters.AddWithValue("@destinationSiteId", objSite.destination_site_id);
                                    updateCommand.Parameters.AddWithValue("@destinationPortType", objSite.destination_port_type);
                                    updateCommand.Parameters.AddWithValue("@destinationNoOfCores", objSite.destination_no_of_cores);
                                    updateCommand.Parameters.AddWithValue("@projectIdDialog", objSite.project_id_dialog);
                                    //updateCommand.Parameters.AddWithValue("@systemId", objSite.system_id);
                                    updateCommand.ExecuteNonQuery();
                                }
                                if(objSite.network_status !="A")
                                {
                                    //Update geom for the site
                                    EditGeomIn geomObj = new EditGeomIn();
                                    geomObj.systemId = objSite.system_id;
                                    geomObj.geomType = "Point";
                                    geomObj.entityType = "Site";
                                    geomObj.userId = userId;
                                    geomObj.longLat = objSite.longitude + " " + objSite.latitude;
                                    geomObj.networkStatus = objSite.network_status;
                                    geomObj.centerLineGeom= null;
                                    EditEntityGeometry(geomObj);

                                }
                                // Optionally handle geojson updates here if needed
                                return objSite;
                            }
                        }
                    }

                    // If site does not exist, perform insert
                    site.created_by = userId;
                    site.created_on = DateTime.Now;
                    site.geom  = site.longitude + " " + site.latitude;
                    NetworkCodeIn objIn = new NetworkCodeIn();
                    objIn.eType = EntityType.Site.ToString();
                    objIn.gType = GeometryType.Point.ToString();
                    objIn.eGeom = site.geom;
                    var objNetworkCodeDetail = GetNetworkCodeDetail(objIn);
                    site.network_id = objNetworkCodeDetail.network_code;
                    site.sequence_id = objNetworkCodeDetail.sequence_id;
                    site.parent_entity_type = objNetworkCodeDetail.parent_entity_type;
                    site.parent_network_id =objNetworkCodeDetail.parent_network_id;
                    site.parent_system_id = objNetworkCodeDetail.parent_system_id;
                    var regionprovice = GetRegionProvinceDetail(objIn);
                    site.region_id= regionprovice.region_id;
                    site.province_id = regionprovice.province_id;
                    site.network_status= "P";

                    var insertQuery = "INSERT INTO att_details_site (site_id, address, site_name, on_air_date, removed_date, " +
                                      "tx_type, tx_technology, tx_segment, tx_ring, region, province, district, " +
                                      "region_address, depot, ds_division, local_authority, latitude, longitude, " +
                                      "owner_name, created_by, created_on, access_24_7, tower_type, tower_height, " +
                                      "cabinet_type, solution_type, site_rank, self_tx_traffic, agg_tx_traffic, " +
                                      "metro_ring_utilization, csr_count, dti_circuit, agg_01, agg_02, bandwidth, " +
                                      "ring_type, link_id, alias_name, project_id, planning_id, purpose_id, workorder_id, " +
                                      "tx_agg, bh_status, elevation, segment, ring, maximum_cost, project_category, " +
                                      "priority, no_of_cores, fiber_link_type, comment, plan_cost, fiber_distance, " +
                                      "fiber_link_code, port_type, destination_site_id, destination_port_type, " +
                                      "destination_no_of_cores, project_id_dialog,network_id,sequence_id,parent_entity_type ,parent_network_id ,parent_system_id,province_id,region_id ,network_status) " +
                                      "VALUES (@siteId, @address, @siteName, @onAirDate, @removedDate, " +
                                      "@txType, @txTechnology, @txSegment, @txRing, @region, @province, @district, " +
                                      "@regionAddress, @depot, @dsDivision, @localAuthority, @latitude, @longitude, " +
                                      "@ownerName, @createdBy, @createdOn, @access247, @towerType, @towerHeight, " +
                                      "@cabinetType, @solutionType, @siteRank, @selfTxTraffic, @aggTxTraffic, " +
                                      "@metroRingUtilization, @csrCount, @dtiCircuit, @agg01, @agg02, @bandwidth, " +
                                      "@ringType, @linkId, @aliasName, @projectId, @planningId, @purposeId, @workorderId, " +
                                      "@txAgg, @bhStatus, @elevation, @segment, @ring, @maximumCost, @projectCategory, " +
                                      "@priority, @noOfCores, @fiberLinkType, @comment, @planCost, @fiberDistance, " +
                                      "@fiberLinkCode, @portType, @destinationSiteId, @destinationPortType, " +
                                      "@destinationNoOfCores, @projectIdDialog,@networkID,@sequenceID,@parentEntityType,@parentNetworkId,@parentSystemId,@provinceID,@regionID ,@networkStatus) RETURNING *";

                    using (var insertCommand = new NpgsqlCommand(insertQuery, conn))
                    {
                        insertCommand.Parameters.AddWithValue("@siteId", site.site_id);
                        insertCommand.Parameters.AddWithValue("@address", site.address);
                        insertCommand.Parameters.AddWithValue("@siteName", site.site_name);
                        insertCommand.Parameters.AddWithValue("@onAirDate", site.on_air_date);
                        insertCommand.Parameters.AddWithValue("@removedDate", (object)site.removed_date ?? DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@txType", site.tx_type);
                        insertCommand.Parameters.AddWithValue("@txTechnology", site.tx_technology);
                        insertCommand.Parameters.AddWithValue("@txSegment", site.tx_segment);
                        insertCommand.Parameters.AddWithValue("@txRing", site.tx_ring);
                        insertCommand.Parameters.AddWithValue("@region", site.region);
                        insertCommand.Parameters.AddWithValue("@province", site.province);
                        insertCommand.Parameters.AddWithValue("@district", site.district);
                        insertCommand.Parameters.AddWithValue("@regionAddress", site.region_address);
                        insertCommand.Parameters.AddWithValue("@depot", site.depot);
                        insertCommand.Parameters.AddWithValue("@dsDivision", site.ds_division);
                        insertCommand.Parameters.AddWithValue("@localAuthority", site.local_authority);
                        insertCommand.Parameters.AddWithValue("@latitude", site.latitude);
                        insertCommand.Parameters.AddWithValue("@longitude", site.longitude);
                        insertCommand.Parameters.AddWithValue("@ownerName", site.owner_name);
                        insertCommand.Parameters.AddWithValue("@createdBy", userId);
                        insertCommand.Parameters.AddWithValue("@createdOn", DateTime.Now);
                        insertCommand.Parameters.AddWithValue("@access247", site.access_24_7);
                        insertCommand.Parameters.AddWithValue("@towerType", site.tower_type);
                        insertCommand.Parameters.AddWithValue("@towerHeight", site.tower_height);
                        insertCommand.Parameters.AddWithValue("@cabinetType", site.cabinet_type);
                        insertCommand.Parameters.AddWithValue("@solutionType", site.solution_type);
                        insertCommand.Parameters.AddWithValue("@siteRank", site.site_rank);
                        insertCommand.Parameters.AddWithValue("@selfTxTraffic", site.self_tx_traffic);
                        insertCommand.Parameters.AddWithValue("@aggTxTraffic", site.agg_tx_traffic);
                        insertCommand.Parameters.AddWithValue("@metroRingUtilization", site.metro_ring_utilization);
                        insertCommand.Parameters.AddWithValue("@csrCount", site.csr_count);
                        insertCommand.Parameters.AddWithValue("@dtiCircuit", site.dti_circuit);
                        insertCommand.Parameters.AddWithValue("@agg01", site.agg_01);
                        insertCommand.Parameters.AddWithValue("@agg02", site.agg_02);
                        insertCommand.Parameters.AddWithValue("@bandwidth", site.bandwidth);
                        insertCommand.Parameters.AddWithValue("@ringType", site.ring_type);
                        insertCommand.Parameters.AddWithValue("@linkId", site.link_id);
                        insertCommand.Parameters.AddWithValue("@aliasName", site.alias_name);
                        insertCommand.Parameters.AddWithValue("@projectId", site.project_id);
                        insertCommand.Parameters.AddWithValue("@planningId", site.planning_id);
                        insertCommand.Parameters.AddWithValue("@purposeId", site.purpose_id);
                        insertCommand.Parameters.AddWithValue("@workorderId", site.workorder_id);
                        insertCommand.Parameters.AddWithValue("@txAgg", site.tx_agg);
                        insertCommand.Parameters.AddWithValue("@bhStatus", site.bh_status);
                        insertCommand.Parameters.AddWithValue("@elevation", site.elevation);
                        insertCommand.Parameters.AddWithValue("@segment", site.segment);
                        insertCommand.Parameters.AddWithValue("@ring", site.ring);
                        insertCommand.Parameters.AddWithValue("@maximumCost", site.maximum_cost);
                        insertCommand.Parameters.AddWithValue("@projectCategory", site.project_category);
                        insertCommand.Parameters.AddWithValue("@priority", site.priority);
                        insertCommand.Parameters.AddWithValue("@noOfCores", site.no_of_cores);
                        insertCommand.Parameters.AddWithValue("@fiberLinkType", site.fiber_link_type);
                        insertCommand.Parameters.AddWithValue("@comment", site.comment);
                        insertCommand.Parameters.AddWithValue("@planCost", site.plan_cost);
                        insertCommand.Parameters.AddWithValue("@fiberDistance", site.fiber_distance);
                        insertCommand.Parameters.AddWithValue("@fiberLinkCode", site.fiber_link_code);
                        insertCommand.Parameters.AddWithValue("@portType", site.port_type);
                        insertCommand.Parameters.AddWithValue("@destinationSiteId", site.destination_site_id);
                        insertCommand.Parameters.AddWithValue("@destinationPortType", site.destination_port_type);
                        insertCommand.Parameters.AddWithValue("@destinationNoOfCores", site.destination_no_of_cores);
                        insertCommand.Parameters.AddWithValue("@projectIdDialog", site.project_id_dialog);
                        insertCommand.Parameters.AddWithValue("@networkID", site.network_id);
                        insertCommand.Parameters.AddWithValue("@sequenceID", site.sequence_id);
                        insertCommand.Parameters.AddWithValue("@parentEntityType", site.parent_entity_type);
                        insertCommand.Parameters.AddWithValue("@parentNetworkId", site.parent_network_id);
                        insertCommand.Parameters.AddWithValue("@parentSystemId", site.parent_system_id);

                        insertCommand.Parameters.AddWithValue("@provinceID", site.province_id);
                        insertCommand.Parameters.AddWithValue("@regionID", site.region_id);
                        insertCommand.Parameters.AddWithValue("@networkStatus", site.network_status);
                        var insertedSite = insertCommand.ExecuteScalar();//

                        //----------save geom------------ -
                        InputGeom geom = new InputGeom();
                        geom.systemId = (int)insertedSite;
                        geom.longLat = site.longitude + " " + site.latitude;
                        geom.userId = userId;
                        geom.entityType = EntityType.Site.ToString();
                        geom.commonName = site.network_id;
                        geom.geomType = GeometryType.Point.ToString();
                        geom.project_id = 0;
                        string chkGeomInsert = SaveEntityGeom(geom);
                        return null;
                    }
                }

                catch (Exception ex)
                {
                    // Log exception and rethrow or handle accordingly
                    throw;
                }
            }
        }
        public DbMessage EditEntityGeometry(EditGeomIn objgeom)
        {
            DbMessage dbMessage = new DbMessage();
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Adjust the query or stored procedure based on your database function and expected return type
                    var query = "SELECT * FROM fn_update_entity_geom(@p_system_id, @p_geom_type, @p_entity_type, @p_userid, @p_longlat, @p_network_status, @p_center_line_geom)";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                    {
                        // Add parameters with their appropriate values
                        command.Parameters.AddWithValue("@p_system_id", objgeom.systemId);
                        command.Parameters.AddWithValue("@p_geom_type", objgeom.geomType);  // Assuming geomType is a variable
                        command.Parameters.AddWithValue("@p_entity_type", objgeom.entityType);  // Assuming entityType is a variable
                        command.Parameters.AddWithValue("@p_userid", objgeom.userId);  // Assuming userId is a variable
                        command.Parameters.AddWithValue("@p_longlat", objgeom.longLat);  // Assuming longLat is a variable
                        command.Parameters.AddWithValue("@p_network_status", objgeom.networkStatus);  // Assuming networkStatus is a variable
                        command.Parameters.AddWithValue("@p_center_line_geom", objgeom.centerLineGeom);  // Assuming centerLineGeom is a variable

                        // Execute the query and handle the results
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Extract the relevant data from the reader and process it
                                if (true)
                                {
                                    // Additional processing or updating child entities
                                    if (objgeom.geomType == GeometryType.Point.ToString())
                                    {
                                       // EditChildEntityGeometry(objgeom);
                                    }

                                    dbMessage.status = true;
                                    dbMessage.message = "Entity geometry updated successfully.";
                                }
                            }
                        }
                    }
                }
                catch (NpgsqlException npgsqlEx)
                {
                    // Specific handling for Npgsql exceptions
                    throw new Exception("A database error occurred while updating entity geometry.", npgsqlEx);
                }
                catch (Exception ex)
                {
                    // General exception handling
                    throw new Exception("An error occurred while retrieving entity geometry.", ex);
                }
                return dbMessage;
            }
        }

        public NetworkCodeDetail GetNetworkCodeDetail(NetworkCodeIn objIn)
        {
            NetworkCodeDetail networkCodeDetail = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Adjust the query or stored procedure based on your database function and expected return type
                    var query = "SELECT * FROM fn_get_network_code(@etype, @gtype, @parent_sysid, @parent_etype, @geometry)";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                    {
                        // Add parameters with their appropriate values
                        command.Parameters.AddWithValue("@etype", objIn.eType);
                        command.Parameters.AddWithValue("@gtype", objIn.gType);
                        command.Parameters.AddWithValue("@parent_sysid", objIn.parent_sysId);
                        command.Parameters.AddWithValue("@parent_etype", objIn.parent_eType);
                        command.Parameters.AddWithValue("@geometry", objIn.eGeom);


                        // Execute the query and handle the results
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Map the result to the NetworkCodeDetail object
                                networkCodeDetail = new NetworkCodeDetail
                                {
                                    network_code = reader.GetString(reader.GetOrdinal("network_code")),
                                    sequence_id = reader.GetInt32(reader.GetOrdinal("sequence_id")),
                                    parent_network_id= reader.GetString(reader.GetOrdinal("parent_network_id")),
                                    parent_entity_type= reader.GetString(reader.GetOrdinal("parent_entity_type")),
                                    parent_system_id=reader.GetInt32(reader.GetOrdinal("parent_system_id"))
                                };
                            }
                        }
                    }
                }
                catch (NpgsqlException npgsqlEx)
                {
                    // Specific handling for Npgsql exceptions
                    throw new Exception("A database error occurred while retrieving network code details.", npgsqlEx);
                }
                catch (Exception ex)
                {
                    // General exception handling
                    throw new Exception("An error occurred while retrieving network code details.", ex);
                }
                return networkCodeDetail ?? new NetworkCodeDetail();
            }
        }
        public string SaveEntityGeom(InputGeom objIn)
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Adjust the query to call the function directly
                    var query = "SELECT fn_save_entity_geom(@p_system_id, @p_geom_type, @p_entity_type, @p_userid, @p_longlat, @p_common_name, @p_network_status, @p_ports, @p_entity_category, @p_center_line_geom, @p_buffer_width, @p_project_id)";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                    {
                        // Add parameters with their appropriate values
                        command.Parameters.AddWithValue("@p_system_id", objIn.systemId);
                        command.Parameters.AddWithValue("@p_geom_type", string.IsNullOrEmpty(objIn.geomType) ? (object)DBNull.Value : objIn.geomType);
                        command.Parameters.AddWithValue("@p_entity_type", string.IsNullOrEmpty(objIn.entityType) ? (object)DBNull.Value : objIn.entityType);
                        command.Parameters.AddWithValue("@p_userid", objIn.userId);
                        command.Parameters.AddWithValue("@p_longlat", string.IsNullOrEmpty(objIn.longLat) ? (object)DBNull.Value : objIn.longLat);
                        command.Parameters.AddWithValue("@p_common_name", string.IsNullOrEmpty(objIn.commonName) ? (object)DBNull.Value : objIn.commonName);
                        command.Parameters.AddWithValue("@p_network_status", "P"); // Assuming this is a constant value
                        command.Parameters.AddWithValue("@p_ports","");
                        command.Parameters.AddWithValue("@p_entity_category", "");
                        command.Parameters.AddWithValue("@p_center_line_geom","");
                        command.Parameters.AddWithValue("@p_buffer_width", Convert.ToDouble(objIn.buffer_width)); // Ensure this is a valid double
                        command.Parameters.AddWithValue("@p_project_id", objIn.project_id ?? (object)DBNull.Value); // Handle project_id if nullable

                        // Log command text and parameters
                        Console.WriteLine("Executing command: " + command.CommandText);
                        foreach (NpgsqlParameter parameter in command.Parameters)
                        {
                            Console.WriteLine($"{parameter.ParameterName}: {parameter.Value}");
                        }

                        // Execute the command and get the result
                        var result = command.ExecuteScalar();
                        return result != null ? result.ToString() : "0"; // Return the result or "0"
                    }
                }
                catch (NpgsqlException npgsqlEx)
                {
                    throw new Exception("A database error occurred while saving the entity geometry.", npgsqlEx);

                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while saving the entity geometry.", ex);
                }

            }
        }

        public InRegionProvince GetRegionProvinceDetail(NetworkCodeIn objIn)
        {
            InRegionProvince regionProvinceDetail = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Adjust the query or stored procedure based on your database function and expected return type
                    var query = "SELECT * FROM fn_getregionprovince(@geometry, @gtype)";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                    {
                        // Add parameters with their appropriate values
                        command.Parameters.AddWithValue("@geometry", objIn.eGeom);
                        command.Parameters.AddWithValue("@gtype", objIn.gType);


                        // Execute the query and handle the results
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Map the result to the InRegionProvince object
                                regionProvinceDetail = new InRegionProvince
                                {
                                    region_id = reader.GetInt32(reader.GetOrdinal("region_id")),
                                    province_id = reader.GetInt32(reader.GetOrdinal("province_id")),
                                };
                            }
                        }
                    }
                }
                catch (NpgsqlException npgsqlEx)
                {
                    // Specific handling for Npgsql exceptions
                    throw new Exception("A database error occurred while retrieving RegionProvinceDetail details.", npgsqlEx);
                }
                catch (Exception ex)
                {
                    // General exception handling
                    throw new Exception("An error occurred while retrieving RegionProvinceDetail details.", ex);
                }
                return regionProvinceDetail ?? new InRegionProvince();
            }
        }
    }
}
