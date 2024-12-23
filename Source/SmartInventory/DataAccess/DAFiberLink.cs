using DataAccess.DBHelpers;
using Models;
using Npgsql;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAFiberLink : Repository<FiberLink>
    {

        public FiberLink SaveFiberLink(FiberLink objFiberLink, int userId)
        {
            try
            {
                var objFiberDetail = repo.Get(u => u.system_id == objFiberLink.system_id);
                if (objFiberDetail != null)
                {
                    objFiberDetail.link_name = objFiberLink.link_name;
                    objFiberDetail.link_type = objFiberLink.link_type;
                    objFiberDetail.start_point_type = objFiberLink.start_point_type;
                    objFiberDetail.start_point_location = objFiberLink.start_point_location;
                    objFiberDetail.start_point_network_id = objFiberLink.start_point_network_id;
                    objFiberDetail.end_point_type = objFiberLink.end_point_type;
                    objFiberDetail.end_point_location = objFiberLink.end_point_location;
                    objFiberDetail.end_point_network_id = objFiberLink.end_point_network_id;
                    objFiberDetail.no_of_lmc = objFiberLink.no_of_lmc;
                    objFiberDetail.each_lmc_length = objFiberLink.each_lmc_length;
                    objFiberDetail.total_route_length = objFiberLink.total_route_length;
                    objFiberDetail.gis_length = objFiberLink.gis_length;
                    objFiberDetail.otdr_distance = objFiberLink.otdr_distance;
                    objFiberDetail.no_of_pair = objFiberLink.no_of_pair;

                    objFiberDetail.tube_and_core_details = objFiberLink.tube_and_core_details;
                    objFiberDetail.existing_route_length_otdr = objFiberLink.existing_route_length_otdr;
                    objFiberDetail.new_building_route_length = objFiberLink.new_building_route_length;
                    objFiberDetail.otm_length = objFiberLink.otm_length;
                    objFiberDetail.otl_length = objFiberLink.otl_length;
                    objFiberDetail.any_row_portion = objFiberLink.any_row_portion;
                    objFiberDetail.row_authority = objFiberLink.row_authority;
                    objFiberDetail.total_row_segments = objFiberLink.total_row_segments;
                    objFiberDetail.total_row_length = objFiberLink.total_row_length;

                    objFiberDetail.total_row_reccuring_charges = objFiberLink.total_row_reccuring_charges;
                    objFiberDetail.handover_date = objFiberLink.handover_date;
                    objFiberDetail.hoto_signoff_date = objFiberLink.hoto_signoff_date;
                    objFiberDetail.remarks = objFiberLink.remarks;
                    objFiberDetail.service_id = objFiberLink.service_id;
                    objFiberDetail.main_link_type = objFiberLink.main_link_type;
                    objFiberDetail.main_link_id = objFiberLink.main_link_id;
                    objFiberDetail.redundant_link_type = objFiberLink.redundant_link_type;
                    objFiberDetail.redundant_link_id = objFiberLink.redundant_link_id;
                    objFiberDetail.modified_on = DateTimeHelper.Now;
                    objFiberDetail.modified_by = userId;
                    objFiberDetail.pageMsg.isNewEntity = false;
                    return repo.Update(objFiberDetail);
                }
                objFiberLink.created_on = DateTimeHelper.Now;
                objFiberLink.fiber_link_status = "Free";
                objFiberLink.created_by = userId;
                objFiberLink.pageMsg.isNewEntity = true;
                var resultItem = repo.Insert(objFiberLink);
                return resultItem;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<Dictionary<string, string>> getFiberLinkDetails(int userId, FiberLinkFilter objFiberLinkFilter)
        {
            try
            {

                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_fiber_link_details", new
                {
                    p_systemid = objFiberLinkFilter.system_id,
                    p_searchby = objFiberLinkFilter.SearchbyText,
                    p_searchtext = objFiberLinkFilter.Searchtext,
                    P_PAGENO = objFiberLinkFilter.currentPage,
                    P_PAGERECORD = objFiberLinkFilter.pageSize,
                    P_SORTCOLNAME = objFiberLinkFilter.sort,
                    P_SORTTYPE = objFiberLinkFilter.orderBy,
                    p_userid = userId,
                    p_searchfrom = objFiberLinkFilter.fromDate,
                    p_searchto = objFiberLinkFilter.toDate
                }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Dictionary<string, string>> getAssociatedFiberLinkDetails(int userId, FiberLinkFilter objFiberLinkFilter)
        {
            try
            {

                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_associated_fiber_link_details", new
                {
                    p_systemid = objFiberLinkFilter.system_id,
                    p_searchby = objFiberLinkFilter.SearchbyText,
                    p_searchtext = objFiberLinkFilter.Searchtext,
                    P_PAGENO = objFiberLinkFilter.currentPage,
                    P_PAGERECORD = objFiberLinkFilter.pageSize,
                    P_SORTCOLNAME = objFiberLinkFilter.sort,
                    P_SORTTYPE = objFiberLinkFilter.orderBy,
                    p_userid = userId,
                    p_searchfrom = objFiberLinkFilter.fromDate,
                    p_searchto = objFiberLinkFilter.toDate
                }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Dictionary<string, string> getLinkInfoForKML(int linkSystemId)
        {
            try
            {

                var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_fiber_link_info_for_kml", new { p_systemid = linkSystemId }, true);
                return result != null && result.Count > 0 ? result[0] : new Dictionary<string, string>();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Dictionary<string, string> getAssociatedLinkInfo(int cable_id, int fiber_number)
        {
            try
            {

                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_associated_fiber_link_info", new
                {
                    p_cable_id = cable_id,
                    p_fiber_number = fiber_number
                }, true).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<fiberLinkStatus> getfiberLinkStatusCounts(FiberLinkFilter objFiberLinkFilter, int userId)
        {
            try
            {
                //return repo.ExecuteProcedure<fiberLinkStatus>("fn_get_fiber_link_status", new { p_userid = userId }).ToList();
                return repo.ExecuteProcedure<fiberLinkStatus>("fn_get_fiber_link_status", new
                {
                    p_systemid = objFiberLinkFilter.system_id,
                    p_searchby = objFiberLinkFilter.SearchbyText,
                    p_searchtext = objFiberLinkFilter.Searchtext,
                    P_PAGENO = objFiberLinkFilter.currentPage,
                    P_PAGERECORD = objFiberLinkFilter.pageSize,
                    P_SORTCOLNAME = objFiberLinkFilter.sort,
                    P_SORTTYPE = objFiberLinkFilter.orderBy,
                    p_userid = userId,
                    p_searchfrom = objFiberLinkFilter.fromDate,
                    p_searchto = objFiberLinkFilter.toDate
                }).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public FiberLink GetFiberLinkById(int system_id)
        {
            try
            {
                return repo.Get(u => u.system_id == system_id);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public FLNetworkCode GetFiberLinkNetworkId()
        {
            try
            {
                var result = repo.ExecuteProcedure<FLNetworkCode>("fn_get_link_network_id", new { });
                return result != null && result.Count > 0 ? result[0] : new FLNetworkCode();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int deleteFiberLinkById(int system_id)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == system_id);
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

        public List<FiberLink> getFiberLinkROWAuthority(string searchText)
        {
            try
            {
                return repo.ExecuteProcedure<FiberLink>("fn_get_fiber_link_row_authority_list", new { searchtext = searchText }, true);
            }
            catch { throw; }
        }
        public FiberLinkPrefix GetlinkPrefixbyPrefixType(string link_prefix)
        {
            try
            {
                var result = repo.ExecuteProcedure<FiberLinkPrefix>("fn_get_fiber_link_prefix", new { p_link_prefix = link_prefix, }, true).FirstOrDefault();
                return result;
            }
            catch { throw; }
        }
        public List<FiberLink> GetAutoFiberLinkId(string searchText)
        {
            try
            {
                return repo.ExecuteProcedure<FiberLink>("fn_get_fiber_link_Id_list", new { searchtext = searchText }, true);
            }
            catch { throw; }
        }
        public FiberLink isFiberLinkIdExist(string linkId, string columnName, int userId)
        {
            try
            {
                return repo.GetAll().Where(x => x.link_id == linkId).FirstOrDefault();

            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Dictionary<string, string>> getExportCableInfoByLinkId(int p_LinkId)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_Export_Cable_Info_ByLinkId", new
                {
                    p_LinkId = p_LinkId,
                }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Dictionary<string, string>> getExportCableInfoByLinkSystemIds(string p_LinkSystemIds)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_cable_info_bylink_systemIds", new
                {
                    p_LinkSystemIds = p_LinkSystemIds,
                }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public vmfiberLinkOnMap getFiberLinkElements(int linkSystemId, int userId)
        {
            try
            {

                return repo.ExecuteProcedure<vmfiberLinkOnMap>("fn_get_fiber_link_path", new
                {
                    p_linkSystemId = linkSystemId,
                    p_userId = userId
                }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public vmfiberLinkOnMap getFiberLinkElementsByLinkSystemIds(string linkSystemIds, int userId)
        {
            try
            {

                return repo.ExecuteProcedure<vmfiberLinkOnMap>("fn_get_fiber_link_path_by_linksystemids", new
                {
                    p_linksystemid = linkSystemIds,
                    p_userId = userId
                }, true).FirstOrDefault();

            }
            catch { throw; }
        }
        public fiberLinkAssociation getAssociatedLinkId(int cable_id, int fiber_number)
        {
            try
            {

                return repo.ExecuteProcedure<fiberLinkAssociation>("fn_get_associated_fiber_Id", new
                {
                    p_cable_id = cable_id,
                    p_fiber_number = fiber_number
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<Dictionary<string, string>> getAssociationCustomer(FiberLinkCustomerFilter objFiberLinkCustomerFilter)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_fiber_link_association_customer", new
                {
                    p_searchby = objFiberLinkCustomerFilter.Searchtext,
                    p_searchtext = objFiberLinkCustomerFilter.SearchbyText,
                    p_link_system_id = objFiberLinkCustomerFilter.link_system_id,
                    p_pageno = objFiberLinkCustomerFilter.currentPage,
                    p_pagerecord = objFiberLinkCustomerFilter.pageSize,
                    p_sortcolname = objFiberLinkCustomerFilter.sort,
                    p_sorttype = objFiberLinkCustomerFilter.sortdir,
                    p_userid = objFiberLinkCustomerFilter.userid,
                }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<FiberLink> checkDuplicaketLinkId(string p_link_id)
        {
            try
            {
                return repo.GetAll(u => u.link_id.Trim() == p_link_id.Trim()).ToList();



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Dictionary<string, string>> GetFiberLinks(int userId, FiberLinkFilter objFiberLinkFilter)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_fiber_links", new
                {
                    p_searchtext = objFiberLinkFilter.Searchtext,
                    P_PAGENO = objFiberLinkFilter.currentPage,
                    P_PAGERECORD = objFiberLinkFilter.pageSize,
                    P_SORTTYPE = objFiberLinkFilter.orderBy,
                    p_userid = userId
                }, true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class DAFiberLinkColumns : Repository<fiberLinkColumnsMapping>
    {

        //public List<Dictionary<string, string>> /*getFiberLinkColumns*/()
        //     {
        //         try
        //         { 
        //             return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_fiber_link_columns_mappings", new { }, true) ; 
        //         }
        //         catch (Exception)
        //         {

        //             throw;
        //         }

        //     }
        public List<fiberLinkColumnsMapping> getFiberLinkColumns()
        {
            try
            {
                //return repo.GetAll().Where(x=>x.is_active=true).OrderBy(x=>x.column_sequence).ToList();
                return repo.ExecuteProcedure<fiberLinkColumnsMapping>("fn_get_fiber_link_columns_mappings", new { }, true).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
    public class DATempFiberMaster : Repository<TempFiberLink>

    {
        private static DATempFiberMaster objFiberLink = null;
        private static readonly object lockObject = new object();
        public static DATempFiberMaster Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objFiberLink == null)
                    {
                        objFiberLink = new DATempFiberMaster();
                    }
                }
                return objFiberLink;
            }
        }
        public void DeleteTempFiberData(int system_id)
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_delete_temp_fiber_link", new { P_Userid = system_id });
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void BulkUploadTempFiber(List<TempFiberLink> TempFiber)
        {
            try
            {
                repo.Insert(TempFiber);
            }
            catch { throw; }
        }
        public DbMessage UploadFiber(int created_by, string network_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_bulk_upload_FiberLink_insert", new { P_UserId = created_by, network_id }).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<TempFiberLink> GetUploadFiberLogs(int UserId)
        {
            try
            {
                return repo.GetAll().Where(x => x.created_by == UserId).OrderBy(x => x.system_id).ToList();
            }
            catch { throw; }
        }
        public Tuple<int, int> getTotalUploadFiberfailureAndSuccess(int UserId)
        {
            try
            {
                var getTotalUploadFiberfailure = repo.GetAll().Where(x => x.created_by == UserId & x.is_valid == false).Count();
                var getTotalUploadFiberSuccess = repo.GetAll().Where(x => x.created_by == UserId & x.is_valid == true).Count();
                return Tuple.Create(getTotalUploadFiberSuccess, getTotalUploadFiberfailure);
            }
            catch { throw; }
        }

    }
}
