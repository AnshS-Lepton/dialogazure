using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Admin;
using DataAccess.Admin;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Security;
using Thinktecture.IdentityModel.WSTrust;
using Models;

namespace BusinessLogics.Admin
{
    public class BLLinkBudget
    {

        public LinkBudgetMaster GetLinkBudgetDetailByID(int _wavelengthId)
        {
            return new DALinkBudget().GetLinkBudgetDetailByID(_wavelengthId);
        }
        public LinkBudgetMaster GetLinkBudgetDetailByWavelength(int WaveLength_value)
        {
            return new DALinkBudget().GetLinkBudgetDetailByWavelength(WaveLength_value);
        }
        public LinkBudgetMaster SaveLinkBudget(LinkBudgetMaster objLinkBudget, int userId)
        {
            return new DALinkBudget().SaveLinkBudget(objLinkBudget, userId);
        }
        public List<LinkBudgetMaster> GetLinkBudgetList(CommonGridAttributes objGridAttributes)
        {
            return new DALinkBudget().GetLinkBudgetList(objGridAttributes);
        }
        public int DeleteLinkBudgetDetailsById(int id)
        {
            return new DALinkBudget().DeleteLinkBudgetDetailsById(id);
        }
        public List<SplitterLossMaster> GetSplitterLossByWaveLength(int wavelengthId)
        {
            return new DASplitterLoss().GetSplitterLossByWaveLength(wavelengthId);
        }
        //public bool SaveSplitterLosses(List<SplitterLossMaster> lstSplitterLosses, int userId)
        //{
        //    return new DASplitterLoss().SaveSplitterLosses(lstSplitterLosses, userId);
        //}
        public int DeleteSplitterLossByWaveLength(int wavelengthId)
        {
            return new DASplitterLoss().DeleteSplitterLossByWaveLength(wavelengthId);
        }

        public List<LinkBudgetMaster> GetWaveLength()
        {
            return new DALinkBudget().GetWaveLength();
        }
    
    }
}
