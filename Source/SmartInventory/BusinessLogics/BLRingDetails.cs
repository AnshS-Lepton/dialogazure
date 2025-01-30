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
        public List<RingDetails> getRingDetails(CommonGridAttributes objGridAttributes)
        {
            return new DARingDetails().getRingDetails(objGridAttributes);
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
    }
}
