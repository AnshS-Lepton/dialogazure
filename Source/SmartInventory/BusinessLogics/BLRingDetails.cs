using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLRingDetails
    {
        public List<RingDetails> getRingDetails(CommonGridAttributes objGridAttributes,string region_name,string segment_code,string ring_code)
        {
            return new DARingDetails().getRingDetails(objGridAttributes, region_name, segment_code, ring_code);
        }
        public List<KeyValueDropDown> GetRegionDetails()
        {
            return new DARingDetails().GetRegionDetails();
        }
        public List<KeyValueDropDown> GetSegmentDetails()
        {
            return new DARingDetails().GetSegmentDetails();
        }
        public List<KeyValueDropDown> GetRingTypeDetails()
        {
            return new DARingDetails().GetRingTypeDetails();
        }
        public vmGeomRingDetailIn getSiteDetails(int site_id)
        {
            return new DARingDetails().getSiteDetails(site_id);
        }
        public vmRingConnectedElementDetails getRingConnectedElementDetail(int site_id,int user_id)
        {
            return new DARingDetails().getRingConnectedElementDetail(site_id, user_id);
        }


    }
}
