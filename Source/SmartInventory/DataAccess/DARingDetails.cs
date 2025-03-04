using DataAccess.DBHelpers;
using Models;
using Npgsql;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;


namespace DataAccess
{
    public class DARingDetails:Repository<RingDetails>
    {
       
        public List<RingDetails> getRingDetails(CommonGridAttributes objGridAttributes, string region_name, string segment_code, string ring_code)
        {
            try
            {
                return repo.ExecuteProcedure<RingDetails>("fn_get_ring_details", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = ((objGridAttributes.searchBy == "region" || objGridAttributes.searchBy == "ring_code") && (!string.IsNullOrEmpty(objGridAttributes.searchText))) ? MiscHelper.Encrypt(objGridAttributes.searchText) : objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    p_ring_network_id = region_name,
                    p_segment_code = segment_code,
                    p_ring_code = ring_code


                }, true); ;
            }
            catch { throw; }
        }
        public List<KeyValueDropDown> GetSearchByColumnName( )
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_reportcolumn_list", new {  }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> GetRegionDetails()
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_regionlist", new { }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> GetSegmentDetails( )
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_segmentlist", new {  }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> GetRingTypeDetails( )
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_ringtypelist", new {  }, true);

            }
            catch { throw; }

        }

        public List<GeomRingDetailIn> getSiteDetails(int ringId)
        {
            try
            {
                return repo.ExecuteProcedure<GeomRingDetailIn>("fn_get_site_details", new
                {
                    p_ringId = ringId

                }, false); ;
            }
            catch { throw; }
        }

        
    }
}
