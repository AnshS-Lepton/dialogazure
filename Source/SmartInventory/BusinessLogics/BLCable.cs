using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BusinessLogics
{
    public class BLCable
    {
        private static BLCable objCable = null;
        private static readonly object lockObject = new object();
        public static BLCable Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objCable == null)
                    {
                        objCable = new BLCable();
                    }
                }
                return objCable;
            }
        }

        public CableMaster SaveCable(CableMaster objCbl, int userId)
        {
            return DACable.Instance.SaveCable(objCbl, userId);
        } 
        public CableMaster getCableDetailbyvendorId(int vendor_id,string etype )
        {
            return DACable.Instance.getCableDetailbyvendorId(vendor_id, etype);
        }
        
        public DbMessage SetCableColorDetails(int cableId, int NoOfTube, int NoOfCore, int userId)
        {
            return DACable.Instance.SetCableColorDetails(cableId, NoOfTube, NoOfCore, userId);
        }
        public DbMessage SaveTubeCoreColor(string tubeColor, string CoreColor, int systemId)
        {
            return DACable.Instance.SaveTubeCoreColor(tubeColor, CoreColor, systemId);
        }
        public string GetCableType(int system_id)
        {
            return DACable.Instance.GetCableType(system_id);
        }

        public EditLineTP EditCableTPDetail(EditLineTP objTPDetail, int userId)
        {
            return DACable.Instance.EditCableTPDetail(objTPDetail, userId);
        }

        public int DeleteCableById(int cable_Id)
        {
            return DACable.Instance.DeleteCableById(cable_Id);
        }
        public List<TubeCoreInfo> GetTubeCoreInfo(int cableId)
        {
            return DACable.Instance.GetTubeCoreInfo(cableId);
        }
        public OffsetGeometry getCableGeom(int systemId, double offset)
        {
            return DACable.Instance.getCableGeom(systemId, offset);
        }
        public List<CableFiberDetail> GetFiberDetailInfo(int cableId)
        {
            return DACable.Instance.GetCableFiberDetail(cableId);
        }


        public string getReservedUtilization(string cableID)
        {
            return DACable.Instance.getReservedUtilization(cableID);
        }



        public  string getConnectivityUtilization(string cableID)
        {
            return DACable.Instance.getConnectivityUtilization(cableID);
        }

        public CableMaster updateCableType(int system_id, int userId)
        {
            return DACable.Instance.updateCableType(system_id, userId);
        }
 		public IspLineMaster getLinegeom(int cableId)
        {
            return DAIspLine.Instance.getLinegeom(cableId);
        }
        public IspLineMaster updateLinegeom(IspLineMaster objLine)
        {
            return DAIspLine.Instance.saveLineGeom(objLine);
        }
        
        public DbMessage SetConnectionWithSplitCable(string cable_one_network_id, string cable_two_network_id, int old_cable_system_id, int splitentitysystemid, string splitentity_network_id, string splitentitytype, int user_id, string splicing_source)
        {
            return DACable.Instance.SetConnectionWithSplitCable(cable_one_network_id, cable_two_network_id, old_cable_system_id, splitentitysystemid, splitentity_network_id, splitentitytype, user_id, splicing_source);
        }
        public DbMessage SetISPConnectionWithSplitCable(string cable_one_network_id, string cable_two_network_id, int old_cable_system_id, int splitentitysystemid, string splitentity_network_id, string splitentitytype, int user_id, string splicing_source, int split_entity_x, int split_entity_y)
        {
            return DACable.Instance.SetISPConnectionWithSplitCable(cable_one_network_id, cable_two_network_id, old_cable_system_id, splitentitysystemid, splitentity_network_id, splitentitytype, user_id, splicing_source, split_entity_x, split_entity_y);
        }
        public bool updateOSPISPLineGeom(List<OSPISPCable> listLine)
        {
            return DAIspLine.Instance.updateOSPISPLineGeom(listLine);
        }
        public DbMessage isIspLineExists(string shaftList, int systemId,int floorCount,int shaftCount)
        {
            return DAIspLine.Instance.isIspLineExists(shaftList, systemId, floorCount, shaftCount);
        }
        public bool checkIspLine(int systemId)
        {
            return DAIspLine.Instance.checkIspLine(systemId);
        }
        public DbMessage UpdateCablesPath(string cableData)
        {
            return DAIspLine.Instance.UpdateCablesPath(cableData);
        }

        public int GetAvailableCores(int cableId)
        {
            try
            {
                return DACable.Instance.GetAvailableCores(cableId);
            }
            catch { throw; }
        }
        // Merge Cable Module
        public List<CableMergeStatus> CompleteMergecableOperation(int MasterCableId, int SecondCableId, int user_id)
        {
            try
            {
                return DACable.Instance.CompleteMergecableOperation(MasterCableId,SecondCableId,user_id);
            }
            catch { throw; }
        }
        public void InsertKML(string SQLQuery)
        {
            DACable.Instance.InsertKML(SQLQuery);
        }
        public List<KeyValueDropDown> GetAllVendorType(string type)
        {
            return DACable.Instance.GetAllVendorType(type);
        }
        public CableMaster GetCableNameAndLengthForLoop(int CableId)
        {
            return DACable.Instance.GetCableNameAndLengthForLoop(CableId);
        }

        #region Additional-attributes
        public string GetOtherInfoCable(int systemId)
        {
            return DACable.Instance.GetOtherInfoCable(systemId);
        }
        #endregion
    }
}
