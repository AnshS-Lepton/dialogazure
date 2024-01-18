using DataAccess.DBHelpers;
using Models;
using Models.Feasibility;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Feasibility
{
    public class DAFeasibilityCableType : Repository<FeasibilityCableType>
    {
        public List<FeasibilityCableType> getFeasibilityCableTypes(CommonGridAttributes objGridAttributes)
        {
            try
            {
                // return repo.GetAll(x => !x.is_deleted).OrderByDescending(x => x.cores).ToList();

                return repo.ExecuteProcedure<FeasibilityCableType>("fn_get_sf_cabletype_details", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy
                }, true);
            }
            catch { throw; }
        }
        public List<FeasibilityCableType> getFeasibilityCableTypesddl()
        {
            return repo.GetAll(x => !x.is_deleted).OrderByDescending(x => x.cores).ToList();
        }
        public FeasibilityCableType saveFeasibilityCableTypes(FeasibilityCableType model, int usrId)
        {
            try
            {
                var objitem = repo.Get(x => x.cable_type_id == model.cable_type_id);
                if (objitem != null)
                {
                    objitem.cores = model.cores;
                    objitem.service_price_per_unit = model.service_price_per_unit;
                    objitem.material_price_per_unit = model.material_price_per_unit;
                    objitem.modified_by = usrId;
                    objitem.modified_on = DateTimeHelper.Now;


                    return repo.Update(objitem);
                }
                else
                {
                    model.created_by = usrId;
                    model.created_on = DateTimeHelper.Now;
                    model.display_name = model.display_name.ToUpper();
                    var resultItem = repo.Insert(model);

                    return resultItem;
                }
            }
            catch { throw; }
        }

        public bool deleteCableType(int systemId)
        {
            try
            {
                var CableType = repo.Get(m => m.cable_type_id == systemId);
                if (CableType != null)
                {
                    CableType.is_deleted = true;
                    repo.Update(CableType);
                }
                return true;
            }
            catch { throw; }
        }

        public List<FeasibiltiyCablesearch> GetSearchEquipmentResult(string srchText)
        {
            try
            {

                return repo.ExecuteProcedure<FeasibiltiyCablesearch>("fn_sf_get_feas_cable_types", new { p_searchtext = srchText.Trim() });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool CheckDisplayNameExist(int core, string displayName)
        {
            var data = repo.GetAll(x => x.display_name.ToUpper() == displayName.ToUpper() && x.is_deleted == false).ToList();
            if (data.Count() > 0)
            {
                return true;
            }
            return false;
        }
        public List<T> getBomExportData<T>(int systemId) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_sf_get_export_Bom_data", null, true).ToList();

                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetFeasibilityHistory(int systemid, string eType, Models.Feasibility.FilterHistoryAttr objFilterAttributes)
        {

            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_sf_get_feasibility_history_Record",
                    new
                    {
                        p_searchtext = objFilterAttributes.searchText,
                        p_searchby = objFilterAttributes.searchBy,
                        P_PAGENO = objFilterAttributes.currentPage,
                        P_PAGERECORD = objFilterAttributes.pageSize,
                        P_SORTCOLNAME = objFilterAttributes.sort,
                        P_SORTTYPE = objFilterAttributes.orderBy,
                        p_systemid = systemid,
                        p_entity_name = eType.ToString()
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public FeasibilityCableType getByCores(int cores)
        {
            try
            {
                return repo.GetAll().Where(c => c.cores == cores).FirstOrDefault();
            }
            catch { throw; }
        }

        public FeasibilityCableType getCableTypeByID(int id)
        {
            try
            {
                return repo.GetById(id);
            }
            catch { throw; }
        }
    }

    public class DAFeasibilityDemarcationType : Repository<FeasibilityDemarcationType>
    {
        public List<FeasibilityDemarcationType> getFeasibilityDemarcationTypes()
        {
            try
            {
                return repo.GetAll().ToList();
            }
            catch { throw; }
        }
    }

    public class DAFeasibilityInput : Repository<FeasibilityInput>
    {
        public FeasibilityInput SavefeasibilityDetails(FeasibilityInput objFeasibilityDetails)
        {
            return repo.Insert(objFeasibilityDetails);
        }
        public FeasibilityInput getFeasibilityInput(int feasibilityID)
        {
            return repo.GetAll(m => m.feasibility_id == feasibilityID).LastOrDefault();
        }
    }

    public class DAFeasibilityInputFtth : Repository<FTTHFeasibilityDetailModel>
    {
       
        public FTTHFeasibilityDetailModel SavefeasibilityDetailsFtth(FTTHFeasibilityDetailModel oModel)
        {
            return repo.Insert(oModel);
        }
    }

    public class DAFeasibilityHistory : Repository<FeasibilityHistory>
    {
        public FeasibilityHistory SavefeasibilityHistory(FeasibilityHistory objFeasibilityHistory)
        {
            return repo.Insert(objFeasibilityHistory);
        }
        public List<FeasibilityHistory> getFeasibilityDetails()
        {
            List<FeasibilityHistory> lstFeasibilityHistory = new List<FeasibilityHistory>();
            return repo.GetAll().ToList();

            // return repo.GetAll().OrderBy(m => m.created_on);
        }

        public List<PastFeasibility> getPastFeasibilities(CommonGridAttributes objGridAttributes, string FromDate, string ToDate)
        {
            try
            {
                var lst = repo.ExecuteProcedure<PastFeasibility>("fn_sf_get_past_feasibilities",
                    new
                    {
                        p_searchtext = objGridAttributes.searchText,
                        p_searchby = objGridAttributes.searchBy,
                        P_PAGENO = objGridAttributes.currentPage,
                        P_PAGERECORD = objGridAttributes.pageSize,
                        P_SORTCOLNAME = objGridAttributes.sort,
                        P_SORTTYPE = objGridAttributes.orderBy,
                        p_FromDate = FromDate,
                        p_ToDate = ToDate
                    }, true);
                return lst;
            }
            catch { throw; }
            //try
            //{
            //    return repo.ExecuteProcedure<PastFeasibility>("fn_sf_get_past_feasibilities", new { }, false);
            //}
            //catch { throw; }
        }

        public List<FeasibilityCableGeoms> getFeasibilityDetails(int history_id)
        {
            try
            {
                return repo.ExecuteProcedure<FeasibilityCableGeoms>("fn_sf_get_cable_geoms", new { hid = history_id }, false);
            }
            catch { throw; }
        }

        public List<T> getFeasibilityReport<T>(string history_ids) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_sf_get_past_feasibilities", new
                {
                    p_history_ids = history_ids
                }, true);

                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }

        public List<T> getInsideCables<T>(string history_ids) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_sf_get_inside_cables", new
                {
                    p_history_ids = history_ids
                }, true);

                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }

        public List<T> getPastFeasibilyExportData<T>(CommonGridAttributes objGridAttributes, string FromDate, string ToDate) where T : new()
        {
            try
            {
               //epo.ExecuteProcedure<T>("fn_sf_get_export_Bom_data", null, true).ToList();
                var lstItems =    repo.ExecuteProcedure<T>("fn_sf_get_past_feasibilities", new
                {
                    p_searchtext = objGridAttributes.searchText,
                    p_searchby = objGridAttributes.searchBy,
                    P_PAGENO = 0,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    p_FromDate = FromDate,
                    p_ToDate = ToDate
                }, true);

                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }

       
    }

    public class DAFeasibilityHistoryFtth: Repository<FTTHFeasibilityHistory>
    {
        public string getFeasibilityDetailsFtth(int history_id)
        {
            DataTable oDT = new DataTable();
            string path_geometry = string.Empty;
            string strQuery = "select st_astext(path_geometry) as path_geometry from feasibility_history_ftth where history_id=" + history_id + "";
            //string strQuery = "select address, lat ||' '|| lng   latlng from building  where state='Delhi' and address like '%Malviya Nagar%'  limit 10";
            oDT= repo.GetDataTable(strQuery);
            if (oDT.Rows.Count > 0) {
                path_geometry = oDT.Rows[0]["path_geometry"].ToString();
            }
            return path_geometry;
        }

        public List<PastFeasibilityFtth> getPastFeasibilitiesFtth(CommonGridAttributes objGridAttributes, string FromDate, string ToDate)
        {
            try
            {
                var lst = repo.ExecuteProcedure<PastFeasibilityFtth>("fn_sf_get_past_feasibilities_Ftth",
                    new
                    {
                        p_searchtext = objGridAttributes.searchText,
                        p_searchby = objGridAttributes.searchBy,
                        P_PAGENO = objGridAttributes.currentPage,
                        P_PAGERECORD = objGridAttributes.pageSize,
                        P_SORTCOLNAME = objGridAttributes.sort,
                        P_SORTTYPE = objGridAttributes.orderBy,
                        p_FromDate = FromDate,
                        p_ToDate = ToDate
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public PastFeasibilityFtth getPastFeasibilityDetailsFtth(int history_id)
        {
            PastFeasibilityFtth oPastFeasibilityFtth = new PastFeasibilityFtth();
            DataTable oDT = new DataTable();
            string path_geometry = string.Empty;
            string strQuery = "select fi.feasibility_id, fi.feasibility_name,fi.customer_id,fi.customer_name,fh.entity_id,fi.lat_lng,fi.entity_loc, fh.path_distance,fi.buffer_radius from feasibility_input_ftth fi inner join feasibility_history_ftth fh on fi.feasibility_id = fh.feasibility_id where fh.history_id=" + history_id + "";
            //string strQuery = "select address, lat ||' '|| lng   latlng from building  where state='Delhi' and address like '%Malviya Nagar%'  limit 10";
            oDT = repo.GetDataTable(strQuery);
            if (oDT.Rows.Count > 0)
            {
                oPastFeasibilityFtth.history_id = history_id;
                oPastFeasibilityFtth.lat_lng = oDT.Rows[0]["lat_lng"].ToString();
                oPastFeasibilityFtth.feasibility_name = oDT.Rows[0]["feasibility_name"].ToString();
                oPastFeasibilityFtth.customer_id = oDT.Rows[0]["customer_id"].ToString();
                oPastFeasibilityFtth.customer_name = oDT.Rows[0]["customer_name"].ToString();
                oPastFeasibilityFtth.entity_id = oDT.Rows[0]["entity_id"].ToString();
                oPastFeasibilityFtth.path_distance = oDT.Rows[0]["path_distance"].ToString();
                oPastFeasibilityFtth.feasibility_id = Convert.ToInt32(oDT.Rows[0]["feasibility_id"].ToString());
                oPastFeasibilityFtth.buffer_radius = Convert.ToInt32(oDT.Rows[0]["buffer_radius"].ToString());
                oPastFeasibilityFtth.entity_loc = oDT.Rows[0]["entity_loc"].ToString();
            }
            return oPastFeasibilityFtth;
        }

        public List<T> getPastFeasibilyExportDataFtth<T>(CommonGridAttributes objGridAttributes, string FromDate, string ToDate) where T : new()
        {
            try
            {
                //epo.ExecuteProcedure<T>("fn_sf_get_export_Bom_data", null, true).ToList();
                var lstItems = repo.ExecuteProcedure<T>("fn_sf_get_past_feasibilities_ftth", new
                {
                    p_searchtext = objGridAttributes.searchText,
                    p_searchby = objGridAttributes.searchBy,
                    P_PAGENO = 0,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    p_FromDate = FromDate,
                    p_ToDate = ToDate
                }, true);

                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }

        public int SavefeasibilityHistoryFtth(FTTHFeasibilityHistory ftthFeasibilityHistory)
        {
            return repo.ExecuteProcedure<int>("sf_fn_insert_feasibility_History_Ftth", new { ftthFeasibilityHistory.feasibility_id, ftthFeasibilityHistory.coordinates, ftthFeasibilityHistory.entity_id, ftthFeasibilityHistory.created_by, ftthFeasibilityHistory.path_distance }).FirstOrDefault();
        }
    }

    public class DAFeasibilityGeometry : Repository<FeasibiltyGeometry>
    {
        public int SavefeasibilityGeometry(FeasibiltyGeometry objFeasibilityGeometry)
        {
            return repo.ExecuteProcedure<int>("sf_fn_insert_feasibility_geometry", new { objFeasibilityGeometry.history_id, objFeasibilityGeometry.cable_geometry, objFeasibilityGeometry.path_type, objFeasibilityGeometry.cable_length, objFeasibilityGeometry.created_by, objFeasibilityGeometry.system_id, objFeasibilityGeometry.network_status, objFeasibilityGeometry.available_cores, objFeasibilityGeometry.total_cores }).FirstOrDefault();
        }
        public List<FeasibiltyGeometry> getFeasibilityGeometry(int historyId)
        {
            return repo.GetAll(m => m.history_id == historyId).ToList();
            // return repo.ExecuteProcedure<FeasibiltyGeometry>("vw_sf_getFeasibilityGeometry", null).Where(m=>m.history_id== historyId).ToList();
        }
    }

    public class DAFeasibilityRouting : Repository<RoutingDetail>
    {
		public List<RoutingDetail> getRoutingDirections(string source, string destination, int start_buffer, int end_buffer, int core_required)
		{
			try
			{

				return repo_routing.ExecuteProcedure<RoutingDetail>("fn_sf_get_routes", new { source, destination, start_buffer, end_buffer, core_required });
			}
			catch
			{
				throw;
			}
		}
	}

    #region(FTTH BLOCK)
    public class DAFeasibilitySettingLayers :  Repository<DataTable>
    {
        public DataTable GetAddressList(string searchText)
        {
            try
            {
                // string strQuery = "select network_id, network_id||','||address as address ,latitude ||' '|| longitude latlng from att_details_building  where (coalesce(network_id,'')||coalesce(building_name,'')||coalesce(address,'')||coalesce(building_no,'')||coalesce(pin_code,'')) like '%" + searchText + "%' limit 10";
                string strQuery = "select network_id, (coalesce(network_id,'') ||','||coalesce(address,'')) as address ,latitude ||' '|| longitude latlng from att_details_building  where (coalesce(network_id,'')||coalesce(building_name,'')||coalesce(address,'')||coalesce(building_no,'')||coalesce(pin_code,'')) like '%" + searchText + "%' limit 10";
                //string strQuery = "select address, lat ||' '|| lng   latlng from building  where state='Delhi' and address like '%Malviya Nagar%'  limit 10";
                return repo.GetDataTable(strQuery);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<NEntityLayers> GetNEntityLayers()
        {

            //NpgsqlDataReader rdr = null;
            List<NEntityLayers> NELayers = new List<NEntityLayers>();
            try
            {
                var qryText = "SELECT  layer_title, layer_id AS LayerId, layer_network_group AS LayerGroup, layer_name AS LayerName, layer_table AS LayerTable FROM layer_details where is_feasibility_layer=true and Upper(layer_name)!='SPLITTER' Union "
                                + "SELECT 'Primary Splitter' AS layer_title, layer_id AS LayerId, layer_network_group AS LayerGroup, 'Primary Splitter' AS LayerName, layer_table AS LayerTable FROM layer_details where is_feasibility_layer = true and Upper(layer_name) = 'SPLITTER' Union "
                                + "SELECT 'Secondary Splitter' AS layer_title, layer_id AS LayerId, layer_network_group AS LayerGroup, 'Secondary Splitter' AS LayerName, layer_table AS LayerTable FROM layer_details where is_feasibility_layer = true and Upper(layer_name) = 'SPLITTER'";
                //var qryText = "SELECT rec_id AS RecId, layer_id AS LayerId, nentity_layer_id AS NEntityLayerId, layer_group AS LayerGroup, layer_type AS LayerType, layer_name AS LayerName, layer_table AS LayerTable FROM ne_map_layers";
                DataTable oDT = new DataTable();
                oDT = repo.GetDataTable(qryText);
                if (oDT != null && oDT.Rows.Count > 0)
                {

                    foreach (DataRow rdr in oDT.Rows)
                    {
                        var neLyrs = new NEntityLayers
                        {
                            //RecId = ((rdr["RecId"] == null || rdr["RecId"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["RecId"])),
                            LayerId = ((rdr["LayerId"] == null || rdr["LayerId"] == DBNull.Value) ? "" : rdr["LayerId"].ToString()),
                            //NEntityLayerId = ((rdr["NEntityLayerId"] == null || rdr["NEntityLayerId"] == DBNull.Value) ? "" : rdr["NEntityLayerId"].ToString()),
                            LayerGroup = ((rdr["LayerGroup"] == null || rdr["LayerGroup"] == DBNull.Value) ? "" : rdr["LayerGroup"].ToString()),
                            // LayerType = ((rdr["LayerType"] == null || rdr["LayerType"] == DBNull.Value) ? "" : rdr["LayerType"].ToString()),
                            LayerName = ((rdr["LayerName"] == null || rdr["LayerName"] == DBNull.Value) ? "" : rdr["LayerName"].ToString()),
                            LayerTable = ((rdr["LayerTable"] == null || rdr["LayerTable"] == DBNull.Value) ? "" : rdr["LayerTable"].ToString()),
                            LayerTitle = ((rdr["layer_title"] == null || rdr["layer_title"] == DBNull.Value) ? "" : rdr["layer_title"].ToString())
                        };
                        NELayers.Add(neLyrs);
                    }
                }
                return NELayers;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetNEntityLyrsDetails(string locPoints, string radiusInMtrs, string[] lyrs, string[] lyrsTbl)
        {
            DataTable neLyrsDetails = null;
            //NpgsqlDataReader rdr = null;
            string layers = string.Empty;
            string qryText = string.Empty;
            string qryTextAppend = string.Empty;
            string qrySubAppend = string.Empty;
            int incr = 0;
            try
            {
                if (lyrs != null && lyrs.Length > 0)
                    layers = String.Join("','", lyrs);
                if (layers != string.Empty)
                {
                    qryText = string.Format("SELECT sqlstmt_for, sqlstmt_query FROM FeasibilityNELyrsSql WHERE sqlstmt_for IN('{0}') AND is_active = true order by in_order asc", layers);
                    neLyrsDetails = repo.GetDataTable(qryText);
                    if (neLyrsDetails.Rows.Count>0 && neLyrsDetails!=null)
                    {
                        qryTextAppend = "";
                        foreach(DataRow rdr in neLyrsDetails.Rows)
                        {
                            incr = incr + 1;
                            var qry = ((rdr["sqlstmt_query"] == null || rdr["sqlstmt_query"] == DBNull.Value) ? "" : rdr["sqlstmt_query"].ToString());
                            if (incr == 1)
                                qryTextAppend += string.Format("WITH Q{0} AS({1})", incr, qry);
                            else
                                qryTextAppend += string.Format(", Q{0} AS({1})", incr, qry);
                        }
                        if (incr > 0)
                        {
                            qrySubAppend = "";
                            for (int i = 1; i <= incr; i++)
                            {
                                if (i == 1)
                                    qrySubAppend += string.Format("SELECT Q{0}.* FROM Q{0} ", i);
                                else
                                    qrySubAppend += string.Format("UNION ALL SELECT Q{0}.* FROM Q{0} ", i);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(qryTextAppend) && !string.IsNullOrEmpty(qrySubAppend))
                    {
                        qryText = string.Format("{0} {1}", qryTextAppend, qrySubAppend);
                        qryText = qryText.Replace("lngLat", locPoints);
                        qryText = qryText.Replace("rdsInMtrs", radiusInMtrs);
                        //neLyrsDetails = repo.GetDataTable("select *,ST_Distance(ST_Transform(( st_setsrid(ST_MakePoint(NeLat, NeLng ), 4326) ),2163),ST_Transform(ST_GeomFromText('POINT(77.026649 28.459664)', 4326),2163))as gem_dis from (" + qryText + ") a where Status!='D' order by gem_dis asc ");
                        //---changes for show data a/q to gem_distance----(by navi)

                        neLyrsDetails = repo.GetDataTable("select *,ST_Distance(ST_Transform(( st_setsrid(ST_MakePoint(NeLng, NeLat ), 4326) ),26986),ST_Transform(ST_GeomFromText('POINT(" + locPoints + ")', 4326),26986))as gem_dis from (" + qryText + ") a where Status!='D' order by gem_dis asc ");
                    }
                }

                return neLyrsDetails;
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public string GetBuildingsHere(string Center, string rdsValue)
        {
            string retTxt = "[]";
            string City_sql = string.Format("select file_path grid_file from dtmdsm_files where st_within(st_geomfromtext('Point({0})',4326),sp_geometry)", Center);
            object _city = "vw_buildings"; //Delhi//PGDataAccess.getScalarValue(City_sql);
            DataTable oDT = new DataTable();
            if (_city is string)
            {
                //City_sql = string.Format("select array_to_json(array_agg(z)) from (select building_height h,name n, st_astext(sp_geometry) geom from {0} where st_intersects(sp_geometry,st_buffer_meters(st_geomfromtext('POINT({1})',4326),100)))z", _city, Center);
                //City_sql = string.Format("select array_to_json(array_agg(z)) from (select address,city,state,pincode,locality,house_type, st_astext(geom) geom from {0} where st_intersects(ST_SetSRID(geom,4326),st_buffer_meters(st_geomfromtext('POINT({1})',4326),100)))z", _city, Center);
               // City_sql = string.Format("select array_to_json(array_agg(z)) from (select address,province_name as city,region_name as state,pin_code,'' as locality, '' as house_type, st_astext(sp_geometry) as geom from vw_att_details_building where st_intersects(ST_SetSRID(sp_geometry,4326),st_buffer_meters(st_geomfromtext('POINT({1})',4326)," + rdsValue + ")))z", _city, Center);
                oDT =  repo.ExecuteProcedure<DataTable>("fn_sf_get_feas_building", new { rdsValue, Center }).FirstOrDefault();
                if (oDT != null && oDT.Rows.Count > 0) {

                    retTxt = oDT.Rows[0]["array_to_json"].ToString();

                }
            }
            return retTxt;
        }

        public string GetMaxFSBIdSeq()
        {
            try
            {
                string res = string.Empty;
                var qryText = "SELECT MAX(rec_id) id FROM user_feasibility_history";
                DataTable oDT = new DataTable();
                oDT = repo.GetDataTable(qryText);
                if (oDT != null && oDT.Rows.Count > 0) {
                    res = oDT.Rows[0]["id"].ToString();
                }
              
                if (string.IsNullOrEmpty(res))
                    return "1";
                else
                    return res;//string conversion  
            }
            catch (Exception)
            {
                throw;
            }
        }

        public  void SaveFeasiblityHistory(FeasibilityHistoryModel fsbHist)
        {
            if (fsbHist != null)
            {
                try
                {
                    if (fsbHist != null & !string.IsNullOrEmpty(fsbHist.LocAddress))
                        fsbHist.LocAddress = fsbHist.LocAddress.Replace("'", "''");
                    var qryText = "INSERT INTO user_feasibility_history(feasibility_id, search_loc, loc_address, loc_points, feasibility_by_user, feasibility_on_date, feasibility_from_browser, feasibility_from_machine, feasibility_from_ipadd)VALUES('" + fsbHist.FeasibilityId + "','" + fsbHist.SearchLoc + "','" + fsbHist.LocAddress + "','" + fsbHist.LocPoints + "','" + fsbHist.FeasibilityByUser + "','" + fsbHist.FeasibilityOnDate + "','" + fsbHist.FeasibilityFromBrowser + "','" + fsbHist.FeasibilityFromMachine + "','" + fsbHist.FeasibilityFromIpAdd + "')";
                    repo.ExecuteSQLCommand(qryText);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }

    #endregion
}
