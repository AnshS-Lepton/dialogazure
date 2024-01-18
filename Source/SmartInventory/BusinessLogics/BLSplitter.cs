using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLSplitter
    {
        public SplitterMaster SaveSplitterEntity(SplitterMaster objSplitterMaster, int userId)
        {
            return new DASplitter().SaveSplitterEntity(objSplitterMaster, userId);
        }
        public int DeleteSplitterById(int systemId)
        {
            return new DASplitter().DeleteSplitterById(systemId);
        }
        public SplitterMaster getSplitterDetails(int systemId)
        {
            return new DASplitter().getSplitterDetails(systemId);
        }
		public bool VerifiedMeterReading(int systemId,double meterReading)
		{
			return new DASplitter().VerifiedMeterReading(systemId, meterReading);
		}

		#region Additional-Attributes
		public string GetOtherInfoSplitter(int systemId)
        {
            return new DASplitter().GetOtherInfoSplitter(systemId);
        }
        #endregion
    }
}
