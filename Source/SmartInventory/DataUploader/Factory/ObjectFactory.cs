using DataUploader.TempUploadObjects;
using Models;

namespace DataUploader
{
    public static class ObjectFactory
    {
        public static dynamic GetInstance(EntityType obj)
        {
            switch (obj)
            {
                case EntityType.Pole:
                    return new UploadTempPole();
                case EntityType.Building:
                    return UploadTempBuilding.Instance;
                //case EntityType.POE:
                //    return new UploadTempPoe();
                case EntityType.Manhole:
                    return new UploadTempManhole();
                case EntityType.SpliceClosure:
                    return new UploadTempSpliceClosure();
                case EntityType.Splitter:
                    return new UploadTempSplitter();
                case EntityType.ADB:
                    return new UploadTempADB();
                case EntityType.Tree:
                    return new UploadTempTree();
                case EntityType.WallMount:
                    return new UploadTempWallMount();
                case EntityType.BDB:
                    return new UploadTempBDB();
                case EntityType.CDB:
                    return new UploadTempCDB();
                case EntityType.FDB:
                    return new UploadTempFDB();
                case EntityType.ONT:
                    return new UploadTempONT();
                case EntityType.HTB:
                    return new UploadTempHTB();
                case EntityType.MPOD:
                    return new UploadTempMPOD();
                case EntityType.Coupler:
                    return new UploadTempCoupler();
                case EntityType.Customer:
                   return new UploadTempCustomerWireless();
                //case EntityType.WirelinePOP:
                //    return new UploadTempWirelinePOP();
                //case EntityType.WirelessPOP:
                //return new UploadTempWirelessPOP();
                case EntityType.Cable:
                    return new UploadTempCable();
                case EntityType.Trench:
                    return new UploadTempTrench();
                case EntityType.Duct:
                    return new UploadTempDuct();
                case EntityType.Microduct:
                    return new UploadTempMicroduct();
                case EntityType.FMS:
                    return new UploadTempFMS();
                case EntityType.Structure:
                    return new UploadTempStructure();
                //case EntityType.WirelessSify:
                //    return new UploadTempBTSLayer();
                //case EntityType.POI:
                //    return new UploadTempPOI();
                //case EntityType.SifySector:
                case EntityType.Loop:
                    return new UploadTempLoop();
                case EntityType.POD:
                    return new UploadTempPOD();
				case EntityType.Tower:
					return new UploadTempTower();
				case EntityType.Antenna:
					return new UploadTempAntenna();
                case EntityType.Sector:
                    return new UploadTempSector();
                case EntityType.Cabinet:
                    return new UploadTempCabinet();
                case EntityType.Vault:
                    return new UploadTempVault();
                case EntityType.LandBase:
                    return new UploadTempLandBase();
                case EntityType.UNIT:
                    return new UploadTempRoom();
                case EntityType.PatchPanel:
                    return new UploadTempPatchPanel();
                case EntityType.Handhole:
                    return new UploadTempHandhole();
                case EntityType.OpticalRepeater:
                    return new UploadTempOpticalrepeater();
                case EntityType.Gipipe:
                    return new UploadTempFMS();
                case EntityType.ROW:
                    return new UploadTempRow();
                default:
                    return null;
            }
        }
        public static dynamic LandBaseGetInstance(LandBaseEntity obj)
        {
            switch (obj)
            {
                case LandBaseEntity.Buildings:
                    return new UploadTempLandBase(); 
                default:
                    return null;
            }
        }
    }
}
