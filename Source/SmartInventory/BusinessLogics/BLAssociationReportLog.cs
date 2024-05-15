using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess;

namespace BusinessLogics
{
    public class BLAssociationReportLog
    {
        public AssociationReportLog SaveAssociationReportLog(AssociationReportLog associationReportLog)
        {
            return new DAAssociationReportLog().SaveAssociationReportLog(associationReportLog);
        }
        public List<AssociationReportLogInfo> GetAssociationAssociationLogList(CommonGridAttributes objGridAttributes, int user_id, string timeInterval)
        {
            return new DAAssociationReportLog().GetAssociationAssociationLogList(objGridAttributes, user_id, timeInterval);
        }

    }
}
