
using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.ISP;

namespace BusinessLogics
{
    public class BLStructure
    {
        BLStructure()
        {

        }
        private static BLStructure objBStructure = null;
        private static readonly object lockObjBL = new object();
        public static BLStructure Instance
        {
            get
            {
                lock (lockObjBL)
                {
                    if (objBStructure == null)
                    {
                        objBStructure = new BLStructure();
                    }
                }
                return objBStructure;
            }
        }

       

        public StructureMaster SaveStructure(StructureMaster structureInfo, string status)
        {
            return DAStructure.Instance.SaveStructure(structureInfo, status);
        }

        public List<StructureMaster> GetStructureByBld(int buildingId)
        {
            return DAStructure.Instance.GetStructureByBld(buildingId);
        }

        public int DeleteStructureById(int structureId)
        {
            return DAStructure.Instance.DeleteStructureById(structureId);
        }
        public StructureMaster getSructureDetailsByCode(string StrCode)
        {
            return DAStructure.Instance.getSructureDetailsByCode(StrCode);
        }
        public List<StructureList> getStructureByBuffer(string latlong)
        {
            return DAStructure.Instance.getStructureByBuffer(latlong);
        }
        public string CheckEntityAssociation(StructureMaster objStructure)
        {
            return DAStructure.Instance.CheckEntityAssociation(objStructure);
        }
        public int getParentStructure(int systemId, string entityType)
        {
            return DAStructure.Instance.getParentStructure(systemId, entityType);

        }
        public string getBuildingAddress(int structureId)
        {
            return DAStructure.Instance.getBuildingAddress(structureId);
        }
    }

        public sealed class BLShaft
        {
            BLShaft()
            {

            }
            private static BLShaft objShaft = null;
            private static readonly object lockObjBL = new object();
            public static BLShaft Instance
            {
                get
                {
                    lock (lockObjBL)
                    {
                        if (objShaft == null)
                        {
                            objShaft = new BLShaft();
                        }
                        return objShaft;
                    }
                }
            }

            public List<StructureShaftInfo> GetShaftByBld(int structureId)
            {
                return DAShaft.Instance.GetShaftByBld(structureId);
            }

            public int DeleteShaftById(int shaftId, int userId)
            {
                return DAShaft.Instance.DeleteShaftById(shaftId, userId);
            }
        }

        public sealed class BLFloor
        {
            BLFloor()
            {

            }
            private static BLFloor objFloor = null;
            private static readonly object lockObjBL = new object();
            public static BLFloor Instance
            {
                get
                {
                    lock (lockObjBL)
                    {
                        if (objFloor == null)
                        {
                            objFloor = new BLFloor();
                        }
                        return objFloor;
                    }
                }
            }

            public List<StructureFloorInfo> GetFloorByBld(int structureId)
            {
                return DAFloor.Instance.GetFloorByBld(structureId);
            }
        public FloorInfo GetFloorByName(string floorName,int structureId)
        {
            return DAFloor.Instance.GetFloorByName(floorName, structureId);
        }
        public FloorInfo GetFloorById(int floorId)
        {
            return DAFloor.Instance.GetFloorById(floorId);
        }
            public int DeleteFloorById(int FloorId, int userId)
            {
                return DAFloor.Instance.DeleteFloorById(FloorId, userId);
            }
        }

        public class BLRFSStatus
        {
            BLRFSStatus()
            {

            }
            private static BLRFSStatus objBStructure = null;
            private static readonly object lockObjBL = new object();
            public static BLRFSStatus Instance
            {
                get
                {
                    lock (lockObjBL)
                    {
                        if (objBStructure == null)
                        {
                            objBStructure = new BLRFSStatus();
                        }
                    }
                    return objBStructure;
                }
            }

            public IEnumerable<BuildingRfSStatus> GetRFS_StatusByBld(int buildingId)
            {
                return RfSbuilding_status.Instance.GetRFS_StatusByBld(buildingId);
            }
        }

    
}
