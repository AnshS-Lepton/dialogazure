using Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSIntegrationDialog
{
    internal class DADTS
    {
        private readonly AppDbContext _context;
        public DADTS(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public int EntryLogInProcessSummary()
        {
            try
            {
                // Create a new instance of the ProcessSite entity
                var processSummary = new ProcessSiteSummary
                {
                    process_start_time = DateTime.Now,
                    created_by = 1, // Adjust the user ID as needed
                    created_on = DateTime.Now,
                    remarks = "DTS Integration",
                    entity_type = "Site"
                };

                // Add the process summary record to the DbContext
                _context.ProcessSites.Add(processSummary);

                // Save the changes to the database
                _context.SaveChanges();

                // Log the inserted process ID for debugging purposes
                Console.WriteLine("Inserted process_id: " + processSummary.process_id);

                // Return the generated process_id
                return processSummary.process_id;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                Console.WriteLine($"Error occurred: {ex.Message}");
                throw;
            }
        }
        public static void ExitLogOutProcessSummary(int processID) {
            using (var context = new AppDbContext())
            {
                var existingRecord = context.ProcessSites.FirstOrDefault(ps => ps.process_id == processID);
                if (existingRecord != null)
                {
                    // Update the fields as necessary
                    existingRecord.process_end_time = DateTime.Now;
                    existingRecord.stataus = "Site Data Updated";
                }
                context.SaveChanges();
            }
        }


        public static void SaveSitesList(List<SiteDetails> siteList, int processID)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    foreach (var site in siteList)
                    {
                        var processSiteList = new ProcessSiteList
                        {
                            process_id = processID,
                            site_id = site.Site_Id,
                            site_name = site.Site_Name,
                            status = site.Status,
                            is_valid = true,
                            error_message = "Site List Saved"
                        };

                        context.ProcessSiteLists.Add(processSiteList);
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    WriteLog.WriteLogFile("Error occurred while saving site list: " + ex.Message);
                }
            }
        }
        public static void UpdateSiteList(string site_id, int processID, string message)
        {
            using (var context = new AppDbContext())
            {
                var existingRecord = context.ProcessSiteLists
                .FirstOrDefault(ps => ps.process_id == processID && ps.site_id == site_id);

                if (existingRecord != null)
                {
                    existingRecord.error_message = message;
                    existingRecord.is_valid = false;
                }
                context.SaveChanges();
            }
        }
        public static List<string> GetSiteIdsByProcessId(int processID)
        {
            using (var context = new AppDbContext())
            {
                var siteIds = context.ProcessSiteLists
                                        .Where(p => p.process_id == processID)
                                        .Select(p => p.site_id)
                                        .ToList();
                return siteIds;
            }
        }

        public static void SaveSiteDetails(SiteAttributes objsite, int userId, int progressID)
        {
            using (var context = new AppDbContext())
            {
                //first check if the site already exists
                var existingRecord = context.PopDetails.Select(ps => new
                {
                    ps.site_id,
                    province_id = ps.province_id != null ? (int?)ps.province_id : 0,
                    region_id = ps.region_id != null ? (int?)ps.region_id : 0,
                    network_status = ps.network_status ?? "Unknown",
                    network_id = ps.network_id ?? "Unknown",
                    parent_entity_type = ps.parent_entity_type ?? "Unknown",
                    parent_network_id = ps.parent_network_id ?? "Unknown",
                    parent_system_id = ps.parent_system_id != null ? (int?)ps.parent_system_id : 0,
                    sequence_id = ps.sequence_id != null ? (int?)ps.sequence_id : 0
                })
        .FirstOrDefault(ps => ps.site_id == objsite.site_id);

                var siteDetails = new SiteAttributes
                {
                    process_id = progressID,    // Assuming objsite contains ProcessId
                    site_id = objsite.site_id,
                    site_name = objsite.site_name ?? "",
                    on_air_date = objsite.on_air_date,
                    removed_date = objsite.removed_date,
                    tx_type = objsite.tx_type ?? "",
                    tx_technology = objsite.tx_technology ?? "",
                    tx_segment = objsite.tx_segment ?? "",
                    tx_ring = objsite.tx_ring ?? "",
                    address = objsite.address ?? "",
                    region = objsite.region,
                    province = objsite.province,
                    district = objsite.district ?? "",
                    region_address = objsite.region_address,
                    depot = objsite.depot,
                    ds_division = objsite.ds_division,
                    local_authority = objsite.local_authority,
                    latitude = objsite.latitude,
                    longitude = objsite.longitude,
                    owner_name = objsite.owner_name ?? "",
                    access_24_7 = objsite.access_24_7,
                    tower_type = objsite.tower_type,
                    tower_height = objsite.tower_height,
                    cabinet_type = objsite.cabinet_type,
                    solution_type = objsite.solution_type,
                    site_rank = objsite.site_rank,
                    self_tx_traffic = objsite.self_tx_traffic,
                    agg_tx_traffic = objsite.agg_tx_traffic,
                    metro_ring_utilization = objsite.metro_ring_utilization,
                    csr_count = objsite.csr_count,
                    dti_circuit = objsite.dti_circuit,
                    agg_01 = objsite.agg_01,
                    agg_02 = objsite.agg_02,
                    bandwidth = objsite.bandwidth,
                    ring_type = objsite.ring_type,
                    link_id = objsite.link_id ?? "",
                    alias_name = objsite.alias_name,
                    created_on = DateTime.Now,
                    created_by = userId,    // Use the userId parameter
                    is_visible_on_map = objsite.is_visible_on_map,
                    source_ref_id = objsite.source_ref_id,
                    source_ref_type = objsite.source_ref_type,
                    target_ref_id = objsite.target_ref_id,
                    target_ref_code = objsite.target_ref_code,
                    target_ref_description = objsite.target_ref_description,
                    gis_design_id = objsite.gis_design_id,
                    project_id = objsite.project_id,
                    planning_id = objsite.planning_id,
                    purpose_id = objsite.purpose_id,
                    workorder_id = objsite.workorder_id,
                    is_used = objsite.is_used,
                    tx_agg = objsite.tx_agg,
                    bh_status = objsite.bh_status,
                    elevation = objsite.elevation,
                    segment = objsite.segment,
                    ring = objsite.ring,
                    maximum_cost = objsite.maximum_cost,
                    project_category = objsite.project_category,
                    priority = objsite.priority,
                    no_of_cores = objsite.no_of_cores,
                    fiber_link_type = objsite.fiber_link_type,
                    comment = objsite.comment,
                    plan_cost = objsite.plan_cost,
                    fiber_distance = objsite.fiber_distance,
                    fiber_link_code = objsite.fiber_link_code,
                    port_type = objsite.port_type,
                    destination_site_id = objsite.destination_site_id,
                    destination_port_type = objsite.destination_port_type,
                    destination_no_of_cores = objsite.destination_no_of_cores,
                    project_id_dialog = objsite.project_id_dialog,
                    is_valid = true,
                    network_status = objsite.network_status,
                    network_id = objsite.network_id,
                    parent_entity_type = objsite.parent_entity_type,
                    parent_network_id = objsite.parent_network_id,
                    parent_system_id = objsite.parent_system_id,
                    sequence_id = objsite.sequence_id,
                };
                if (objsite.status_updated_on.HasValue)
                {
                    siteDetails.status_updated_on = objsite.status_updated_on;
                }

                // If SiteId is not null, update the existing record
                if (existingRecord != null)
                {
                    siteDetails.is_new_entity = false;
                    siteDetails.province_id = existingRecord.province_id ?? 0;
                    siteDetails.region_id = existingRecord.region_id ?? 0;
                    siteDetails.network_status = existingRecord.network_status;
                    siteDetails.network_id = existingRecord.network_id;
                    siteDetails.parent_entity_type = existingRecord.parent_entity_type;
                    siteDetails.parent_network_id = existingRecord.parent_network_id;
                    siteDetails.parent_system_id = existingRecord.parent_system_id ?? 0;
                    siteDetails.sequence_id = existingRecord.sequence_id ?? 0;
                }
                else
                {
                    var geom = objsite.longitude + " " + objsite.latitude;
                    NetworkCodeIn objIn = new NetworkCodeIn();
                    objIn.eType = EntityType.POD.ToString();
                    objIn.gType = GeometryType.Point.ToString();
                    objIn.eGeom = geom;
                    var regionprovice = GetRegionProvinceDetail(objIn,context);
                    siteDetails.region_id = regionprovice.region_id;
                    siteDetails.province_id = regionprovice.province_id;
                    siteDetails.network_status = "P";
                    siteDetails.is_new_entity = true;
                }

                context.SiteDetails.Add(siteDetails);

                context.SaveChanges();
            }
        }
        public static InRegionProvince GetRegionProvinceDetail(NetworkCodeIn objIn, AppDbContext context)
        {
            InRegionProvince regionProvinceDetail = null;

            try
            {
                // Define the SQL query to execute the function
                var query = @"
            SELECT * FROM fn_getregionprovince(@geometry, @gtype)";

                // Execute the query using raw SQL and map it to InRegionProvince
                var result = context.Database.SqlQuery<InRegionProvince>(query,
                        new NpgsqlParameter("geometry", objIn.eGeom),
                        new NpgsqlParameter("gtype", objIn.gType))
                    .ToList();

                // Take the first result if available
                regionProvinceDetail = result.FirstOrDefault();

            }
            catch (DbUpdateException dbEx)
            {
                // Handle database-specific errors
                throw new Exception("A database error occurred while retrieving RegionProvinceDetail.", dbEx);
            }
            catch (Exception ex)
            {
                // General error handling
                throw new Exception("An error occurred while retrieving RegionProvinceDetail.", ex);
            }

            return regionProvinceDetail ?? new InRegionProvince();
        }

        public static ProcessSiteOutput SaveSiteDetilsInMainTable(int processID)
        {
            ProcessSiteOutput records = null;

            try
            {
                using (var context = new AppDbContext()) {
                    // Define the SQL query
                    //var query = @"
                    //SELECT * FROM fn_process_site_details(@process_id_input)";
                    var query = @"
                SELECT * FROM fn_process_save_pod_details(@process_id_input)";

                    var result = context.Database.SqlQuery<ProcessSiteOutput>(query,new NpgsqlParameter("process_id_input", processID)).ToList();
                records = result.FirstOrDefault();
            }
            }
            catch (DbUpdateException dbEx)
            {
                WriteLog.WriteLogFile("A database error occurred while retrieving network code details: " + dbEx.ToString());

            }
            catch (Exception ex)
            {
                WriteLog.WriteLogFile("An error occurred while retrieving network code details: " + ex.Message);
            }
            WriteLog.WriteLogFile("Inserted Count: " + records.inserted_count + " Updated Count: " + records.updated_count);
            return records ?? new ProcessSiteOutput();
        }

        public static NetworkCodeDetail GetNetworkCodeDetail(NetworkCodeIn objIn, AppDbContext context)
        {
            NetworkCodeDetail networkCodeDetail = null;

            try
            {
                // Define the SQL query
                var query = @"
            SELECT * FROM fn_get_network_code(@etype, @gtype, @parent_sysid, @parent_etype, @geometry)";

                // Execute the query using raw SQL and map it to NetworkCodeDetail
                var result = context.Database.SqlQuery<NetworkCodeDetail>
                    (query,
                        new NpgsqlParameter("etype", objIn.eType),
                        new NpgsqlParameter("gtype", objIn.gType),
                        new NpgsqlParameter("parent_sysid", objIn.parent_sysId),
                        new NpgsqlParameter("parent_etype", objIn.parent_eType),
                        new NpgsqlParameter("geometry", objIn.eGeom))
                    .ToList();

                // Take the first result if available
                networkCodeDetail = result.FirstOrDefault();
            }
            catch (DbUpdateException dbEx)
            {
                // Specific handling for database-related exceptions
                throw new Exception("A database error occurred while retrieving network code details.", dbEx);
            }
            catch (Exception ex)
            {
                // General exception handling
                throw new Exception("An error occurred while retrieving network code details.", ex);
            }

            return networkCodeDetail ?? new NetworkCodeDetail();
        }

        public static void SaveEntityGeom(InputGeom objgeom, AppDbContext context)
        {
            try
            {
                var query = @"
            SELECT fn_save_entity_geom(@p_system_id, @p_geom_type, @p_entity_type, @p_userid, @p_longlat, @p_common_name,@p_network_status,@p_ports,@p_entity_category, @p_center_line_geom,@p_buffer_width, @p_project_id)";

                // Execute the query using raw SQL
                context.Database.SqlQuery<object>(query,
                    new NpgsqlParameter("@p_system_id", objgeom.systemId),
                    new NpgsqlParameter("@p_geom_type", objgeom.geomType),
                    new NpgsqlParameter("@p_entity_type", objgeom.entityType),
                    new NpgsqlParameter("@p_userid", objgeom.userId),
                    new NpgsqlParameter("@p_longlat", objgeom.longLat),
                    new NpgsqlParameter("@p_common_name", ""),
                    new NpgsqlParameter("@p_network_status", objgeom.networkStatus),
                    new NpgsqlParameter("@p_ports", ""),
                    new NpgsqlParameter("@p_entity_category", ""),
                    new NpgsqlParameter("@p_center_line_geom", ""),
                    new NpgsqlParameter("@p_buffer_width", objgeom.buffer_width),
                    new NpgsqlParameter("@p_project_id", objgeom.project_id)).ToList();
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogFile("An error occurred while updating entity geometry: " + ex.Message);
            }

        }

        public static DbMessage updateGeojsonEntityAttribute(int system_id, string entityType, int? province_id, int action_id)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    return context.Database.SqlQuery<DbMessage>(
     "SELECT * FROM fn_geojson_update_entity_attribute(@p_system_id, @p_entity_type, @p_province_id, @p_action_id, @is_location_edit)",
     new NpgsqlParameter("p_system_id", system_id),
     new NpgsqlParameter("p_entity_type", entityType),
     new NpgsqlParameter("p_province_id", province_id),
     new NpgsqlParameter("p_action_id", action_id),
     new NpgsqlParameter("is_location_edit", false)
 ).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogFile("An error occurred while updating GeoJSON entity attribute: " + ex.Message);
                return null; // Ensure a value is returned in case of an exception
            }
        }


        public static void EditEntityGeometry(EditGeomIn objgeom, AppDbContext context)
        {
            try
            {
                var query = @"
            SELECT fn_update_entity_geom(@p_system_id, @p_geom_type, @p_entity_type, @p_userid, @p_longlat, @p_network_status, @p_center_line_geom)";

                // Execute the query using raw SQL
                context.Database.SqlQuery<object>(query,
                    new NpgsqlParameter("@p_system_id", objgeom.systemId),
                    new NpgsqlParameter("@p_geom_type", objgeom.geomType),
                    new NpgsqlParameter("@p_entity_type", objgeom.entityType),
                    new NpgsqlParameter("@p_userid", objgeom.userId),
                    new NpgsqlParameter("@p_longlat", objgeom.longLat),
                    new NpgsqlParameter("@p_network_status", objgeom.networkStatus),
                    new NpgsqlParameter("@p_center_line_geom", objgeom.centerLineGeom)).ToList();
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogFile("An error occurred while updating entity geometry: " + ex.Message);
            }

        }
        public static void UpdateNetworkandGeomDetails(int progressID)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var siteIds = context.SiteDetails
                                        .Where(p => p.process_id == progressID)
                                        .Select(ps => new
                                        {
                                            ps.site_id,
                                            ps.is_new_entity
                                        })
                                        .ToList();

                    foreach (var sites in siteIds)
                    {
                        //Update Network Details
                        var existingRecord = context.PopDetails
                        .FirstOrDefault(ps => ps.site_id == sites.site_id);

                        if (existingRecord != null && sites.is_new_entity)
                        {
                            var geom = ((double)existingRecord.longitude) + " " + ((double)existingRecord.latitude);
                            NetworkCodeIn objIn = new NetworkCodeIn();
                            objIn.eType = EntityType.POD.ToString();
                            objIn.gType = GeometryType.Point.ToString();
                            objIn.eGeom = geom;
                            var networkDetils = GetNetworkCodeDetail(objIn,context);
                            existingRecord.network_id = networkDetils.network_code;
                            if (existingRecord.pod_name == null)
                            {
                                existingRecord.pod_name = networkDetils.network_code;
                            }
                            existingRecord.sequence_id = networkDetils.sequence_id;
                            existingRecord.specification = "Generic";
                            existingRecord.category =EntityType.Site.ToString();
                            existingRecord.parent_entity_type = networkDetils.parent_entity_type;
                            existingRecord.parent_network_id = networkDetils.parent_network_id;
                            existingRecord.parent_system_id = networkDetils.parent_system_id;
                            context.SaveChanges();
                        }
                        //Update Geom Details
                        if (existingRecord != null && existingRecord.network_status != "A")
                        {
                            if (sites.is_new_entity)
                            {
                                InputGeom inputGeom = new InputGeom();
                                inputGeom.systemId = existingRecord.system_id;
                                inputGeom.geomType = "Point";
                                inputGeom.entityType = "POD";
                                inputGeom.userId = existingRecord.created_by;
                                inputGeom.longLat = existingRecord.longitude + " " + existingRecord.latitude;
                                inputGeom.networkStatus = existingRecord.network_status;
                                inputGeom.centerLineGeom = null;
                                inputGeom.buffer_width = 0;
                                inputGeom.project_id = existingRecord.project_id;
                                SaveEntityGeom(inputGeom,context);
                                updateGeojsonEntityAttribute(existingRecord.system_id, Models.EntityType.POD.ToString(),existingRecord.province_id,0);
                        }
                        else
                        {
                            EditGeomIn geomObj = new EditGeomIn();
                            geomObj.systemId = existingRecord.system_id;
                            geomObj.geomType = "Point";
                            geomObj.entityType = "POD";
                            geomObj.userId = existingRecord.created_by;
                            geomObj.longLat = existingRecord.longitude + " " + existingRecord.latitude;
                            geomObj.networkStatus = existingRecord.network_status;
                            geomObj.centerLineGeom = "";
                            EditEntityGeometry(geomObj, context);
                            updateGeojsonEntityAttribute(existingRecord.system_id, Models.EntityType.POD.ToString(), existingRecord.province_id, 1);
                            }
                    }
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        WriteLog.WriteLogFile($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogFile("Error occurred while updating network and geom details: " + ex.Message);

            }
        }

    }
}
