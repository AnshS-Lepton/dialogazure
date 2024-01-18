using DataAccess.Feasibility;
using Models;
using Models.Feasibility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Feasibility
{
    public class BLFeasibilityCableType
    {
        public List<FeasibilityCableType> getFeasibilityCableTypes(CommonGridAttributes objGridAttributes)
        {
            return new DAFeasibilityCableType().getFeasibilityCableTypes(objGridAttributes);
        }
        public List<FeasibilityCableType> getFeasibilityCableTypesddl()
        {
            return new DAFeasibilityCableType().getFeasibilityCableTypesddl();
        }


        public FeasibilityCableType saveFeasibilityCableTypes(FeasibilityCableType model, int usrId)
        {
            return new DAFeasibilityCableType().saveFeasibilityCableTypes(model, usrId);
        }

        public bool deleteCableType(int systemId)
        {
            return new DAFeasibilityCableType().deleteCableType(systemId);
        }

        public List<FeasibiltiyCablesearch> GetSearchEquipmentResult(string srchText)
        {
            return new DAFeasibilityCableType().GetSearchEquipmentResult(srchText);
        }

        public bool CheckDisplayNameExist(int core, string displayName)
        {
            return new DAFeasibilityCableType().CheckDisplayNameExist(core, displayName);
        }
        public List<T> getBomExportData<T>(int systemId) where T : new()
        {
            return new DAFeasibilityCableType().getBomExportData<T>(systemId);
        }
        public List<Dictionary<string, string>> GetFeasibilityHistory(int systemid, string eType, Models.Feasibility.FilterHistoryAttr objFilterAttributes)
        {
            return new DAFeasibilityCableType().GetFeasibilityHistory(systemid, eType,objFilterAttributes);
        }

        public FeasibilityCableType getByCores(int cores)
        {
            return new DAFeasibilityCableType().getByCores(cores);
        }

        public FeasibilityCableType getCableTypeByID(int id)
        {
            return new DAFeasibilityCableType().getCableTypeByID(id);
        }
    }

    public class BLFeasibilityDemarcationType
    {
        public List<FeasibilityDemarcationType> getFeasibilityDemarcationTypes()
        {
            return new DAFeasibilityDemarcationType().getFeasibilityDemarcationTypes();
        }
    }

    public class BLFeasibilityInput
    {
        public FeasibilityInput SavefeasibilityDetails(FeasibilityInput objFeasibilityDetails)
        {
            return new DAFeasibilityInput().SavefeasibilityDetails(objFeasibilityDetails);
        }
        public FeasibilityInput getFeasibilityInput(int feasibilityID)
        {
            return new DAFeasibilityInput().getFeasibilityInput(feasibilityID);
        }

        public FTTHFeasibilityDetailModel SavefeasibilityDetailsFtth(FTTHFeasibilityDetailModel oModel)
        {
            return new DAFeasibilityInputFtth().SavefeasibilityDetailsFtth(oModel);
        }
    }

    public class BLFeasibilityHistory
    {
        public FeasibilityHistory SavefeasibilityHistory(FeasibilityHistory objFeasibilityHistory)
        {
            return new DAFeasibilityHistory().SavefeasibilityHistory(objFeasibilityHistory);
        }
        public List<FeasibilityHistory> getFeasibilityDetails()
        {
            return new DAFeasibilityHistory().getFeasibilityDetails();
        }

        public List<PastFeasibility> getPastFeasibilities(CommonGridAttributes objGridAttributes, string FromDate, string ToDate)
        {
            return new DAFeasibilityHistory().getPastFeasibilities(objGridAttributes, FromDate, ToDate);
        }

        public List<FeasibilityCableGeoms> getFeasibilityDetails(int history_id)
        {
            return new DAFeasibilityHistory().getFeasibilityDetails(history_id);
        }

        public List<T> getFeasibilityReport<T>(string history_ids) where T : new()
        {
            return new DAFeasibilityHistory().getFeasibilityReport<T>(history_ids);
        }

        public List<T> getInsideCables<T>(string history_ids) where T : new()
        {
            return new DAFeasibilityHistory().getInsideCables<T>(history_ids);
        }

        public List<T> getPastFeasibilyExportData<T>(CommonGridAttributes objGridAttributes, string FromDate, string ToDate) where T : new()
        {
            return new DAFeasibilityHistory().getPastFeasibilyExportData<T>(objGridAttributes, FromDate, ToDate);
        }

        public int SavefeasibilityHistoryFtth(FTTHFeasibilityHistory ftthFeasibilityHistory)
        {
            return new DAFeasibilityHistoryFtth().SavefeasibilityHistoryFtth(ftthFeasibilityHistory);
        }

        public List<PastFeasibilityFtth> getPastFeasibilitiesFtth(CommonGridAttributes objGridAttributes, string FromDate, string ToDate)
        {
            return new DAFeasibilityHistoryFtth().getPastFeasibilitiesFtth(objGridAttributes, FromDate, ToDate);
        }

        public List<T> getPastFeasibilyExportDataFtth<T>(CommonGridAttributes objGridAttributes, string FromDate, string ToDate) where T : new()
        {
            return new DAFeasibilityHistoryFtth().getPastFeasibilyExportDataFtth<T>(objGridAttributes, FromDate, ToDate);
        }

        public string getFeasibilityDetailsFTTH(int history_id)
        {
            return new DAFeasibilityHistoryFtth().getFeasibilityDetailsFtth(history_id);
        }

        public PastFeasibilityFtth getFeasibilityDetailsFtth(int history_id)
        {
            return new DAFeasibilityHistoryFtth().getPastFeasibilityDetailsFtth(history_id);
        }
    }

    public class BLFeasibiltyGeometry
    {
        public int SavefeasibilityGeometry(FeasibiltyGeometry objFeasibiltyGeometry)
        {
            return new DAFeasibilityGeometry().SavefeasibilityGeometry(objFeasibiltyGeometry);
        }
        public List<FeasibiltyGeometry> getFeasibilityGeometry(int historyId)
        {
            return new DAFeasibilityGeometry().getFeasibilityGeometry(historyId);
        }
    }

    public class BLFeasibilityRouting
    {
		public List<RoutingDetail> getRoutingDirections(string source, string destination, int start_buffer, int end_buffer, int core_required)
		{
			return new DAFeasibilityRouting().getRoutingDirections(source, destination, start_buffer, end_buffer, core_required);
		}
	}

    public class FeasibilitySettingsBL
    {
        public List<NEntityLayers> GetNEntityLayers()
        {
            return new DAFeasibilitySettingLayers().GetNEntityLayers();
        }

        public DataTable GetMatchedAddressList(string searchAdd)
        {
            return new DAFeasibilitySettingLayers().GetAddressList(searchAdd);
        }

        public  List<NEntityDetails> GetNEntityLyrsDetails(string locPoints, string radiusInMtrs, string[] lyrs, string[] lyrsTbl)
        {
            List<NEntityDetails> neDetails = null;
            DataTable details = new DAFeasibilitySettingLayers().GetNEntityLyrsDetails(locPoints, radiusInMtrs, lyrs, lyrsTbl);
            if (details != null && details.Rows.Count > 0)
                neDetails = CommonHelpers.DataTableToList<NEntityDetails>(details);
            else
                neDetails = new List<NEntityDetails>();
            return neDetails;
        }

        public string GetBuildingsHere(string centerLatLngPoints, string rdsValue)
        {
            return new DAFeasibilitySettingLayers().GetBuildingsHere(centerLatLngPoints, rdsValue);
        }

        public  void SaveFeasiblityHistory(string searchLoc, string locAdd, string locPoints, string fromBrowser, string fromMachine, string fromIpAddress)
        {
            var maxIdSeq = new DAFeasibilitySettingLayers().GetMaxFSBIdSeq();
            var newFSBId = CommonHelpers.GetUniqueId(12, maxIdSeq, "FSB", true);
            var fsbHist = new FeasibilityHistoryModel()
            {
                SearchLoc = searchLoc,
                LocAddress = locAdd,
                LocPoints = locPoints,
                FeasibilityId = newFSBId,
                FeasibilityByUser = "Admin",
                FeasibilityFromIpAdd = fromIpAddress,
                FeasibilityFromBrowser = fromBrowser,
                FeasibilityFromMachine = fromMachine,
                FeasibilityOnDate = DateTime.Now
            };
            new DAFeasibilitySettingLayers().SaveFeasiblityHistory(fsbHist);
        }
    }

}
