using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
	public class BLItemTemplate
	{
		private static BLItemTemplate objBLItemTemplate = null;
		private static readonly object lockObject = new object();
		public static BLItemTemplate Instance
		{
			get
			{
				lock (lockObject)
				{
					if (objBLItemTemplate == null)
					{
						objBLItemTemplate = new BLItemTemplate();
					}
				}
				return objBLItemTemplate;
			}
		}

		public List<KeyValueDropDown> GetDropDownList()
		{
			try
			{
				return new DAItemTemplate().GetDropDownList();
			}
			catch { throw; }
		}
		public List<KeyValueDropDown> GetItemSpecification(string entitytype, int typeid = 0, int brandid = 0, string specification = "")
		{
			return new DAItemTemplate().GetItemSpecification(entitytype, typeid, brandid, specification);
		}
		public List<itemMaster> GetLayerTemplateDetail(int userid)
		{
			return new DAItemTemplate().GetLayerTemplateDetail(userid);
		}
		public List<KeyValueDropDown> GetVendorList(string specification)
		{
			return new DAItemTemplate().GetVendorList(specification);
		}
		public List<KeyValueDropDown> GetAllVendorList()
		{
			return new DAItemTemplate().GetAllVendorList();
		}
		public List<itemCategory> GetCatSubCatData(string entitytype, string specification, int vendor_id)
		{
			return new DAItemTemplate().GetCatSubCatData(entitytype, specification, vendor_id);
		}
		public List<itemCategory> GetMicroductNoOfWaysData(string entitytype, string specification, int vendor_id)
		{
			return new DAItemTemplate().GetMicroductNoOfWaysData(entitytype, specification, vendor_id);
		}
		public List<KeyValueDropDown> GetBrandData(int typeid, int Layer_id)
		{
			return new DAItemTemplate().GetBrandData(typeid, Layer_id);
		}
		public List<KeyValueDropDown> GetModelData(int brandid)
		{
			return new DAItemTemplate().GetModelData(brandid);
		}
		public T GetTemplateDetail<T>(int userid, EntityType eType, string subEntityType = "") where T : new()
		{
			return new DAItemTemplate().GetTemplateDetail<T>(userid, eType, subEntityType);
		}
		public List<DropDownMaster> GetMicroDuctData()
		{
			return new DAItemTemplate().GetMicroDuctData();
		}

		public void BindItemDropdowns(dynamic objItem, string entityType)
		{
			new DAItemTemplate().BindItemDropdowns(objItem, entityType);
		}

		//public CableItemMaster GetCableTemplateDetail(int userid, EntityType eType, string cableType)
		//{
		//    return new DAItemTemplate().GetCableTemplateDetail(userid, eType, cableType);
		//}

		public List<KeyValueDropDown> GetAccessoriesSpecification(int accessories_id)
		{
			return new DAItemTemplate().GetAccessoriesSpecification(accessories_id);
		}
	}
	public class BLADBItemMaster
	{
		public ADBItemMaster SaveADBItemTemplate(ADBItemMaster ADBItem, int userId)
		{
			return new DAIADBtemMaster().SaveADBItemTemplate(ADBItem, userId);
		}

	}
	public class BLCDBItemMaster
	{

		public CDBItemMaster SaveCDBItemTemplate(CDBItemMaster CDBItem, int userId)
		{
			return new DAICDBtemMaster().SaveCDBItemTemplate(CDBItem, userId);
		}
		public CDBItemMaster getCDBTemplatebyPortNo(int no_of_ports, string entity_type, int vendor_id)
		{
			return new DAICDBtemMaster().getCDBTemplatebyPortNo(no_of_ports, entity_type, vendor_id);
		}
	}
	public class BLBDBItemMaster
	{

		public BDBItemMaster SaveBDBItemTemplate(BDBItemMaster BDBItem, int userId)
		{
			return new DAIBDBtemMaster().SaveBDBItemTemplate(BDBItem, userId);
		}
	}

	public class BLPODItemMaster
	{
		public PODItemMaster SavePODItemTemplate(PODItemMaster PODItem, int userId)
		{
			return new DAPODItemMaster().SavePODItemTemplate(PODItem, userId);
		}
	}

	public class BLTreeItemMaster
	{
		public TreeItemMaster SaveTreeItemTemplate(TreeItemMaster TreeItem, int userId)
		{
			return new DATreeItemMaster().SaveTreeItemTemplate(TreeItem, userId);
		}
	}

	public class BLPoleItemMaster
	{
		public PoleItemMaster GetPoleItemtemplatebyID(int userid, string eType)
		{
			return new DAIPoleItemMaster().GetPoleItemtemplatebyID(userid, eType);
		}
		public PoleItemMaster SavePoleItemTemplate(PoleItemMaster PoleItem, int userId)
		{
			return new DAIPoleItemMaster().SavePoleItemTemplate(PoleItem, userId);
		}
		public bool ChkEntityTemplateExist(string entity_type, int user_id, string subEntityType)
		{
			return new DAIPoleItemMaster().ChkEntityTemplateExist(entity_type, user_id, subEntityType);
		}

	}

	public class BLManholeItemMaster
	{
		public ManholeItemMaster SaveManholeItemTemplate(ManholeItemMaster ManholeItem, int userId)
		{
			return new DAManholeItemMaster().SaveManholeItemTemplate(ManholeItem, userId);
		}
	}

    public class BLHandholeItemMaster
    {
        public HandholeItemMaster SaveHandholeItemTemplate(HandholeItemMaster HandholeItem, int userId)
        {
            return new DAHandholeItemMaster().SaveHandholeItemTemplate(HandholeItem, userId);
        }
    }

    public class BLCouplerItemMaster
	{
		public CouplerItemMaster SaveCouplerItemTemplate(CouplerItemMaster CouplerItem, int userId)
		{
			return new DACouplerItemMaster().SaveCouplerItemTemplate(CouplerItem, userId);
		}
	}
	public class BLSplitterItemMaster
	{
		public SplitterItemMaster SaveSplitterItemTemplate(SplitterItemMaster SplitterItem, int userId)
		{
			return new DASplitterItemMaster().SaveSplitterItemTemplate(SplitterItem, userId);
		}
	}

	public class BLSCItemMaster
	{
		public SCItemMaster SaveSCItemTemplate(SCItemMaster SCItem, int userId)
		{
			return new DASCItemMaster().SaveSCItemTemplate(SCItem, userId);
		}
	}
	public class BLFMSItemMaster
	{
		public FMSItemMaster SaveFMSItemTemplate(FMSItemMaster item, int userId)
		{
			return new DAFMSItemMaster().SaveFMSItemTemplate(item, userId);
		}
	}
	public class BLMPODItemMaster
	{
		public MPODItemMaster SaveMPODItemTemplate(MPODItemMaster PODItem, int userId)
		{
			return new DAMPODItemMaster().SaveMPODItemTemplate(PODItem, userId);
		}
	}



	public class BLCableItemMaster
	{
		public CableItemMaster SaveCableItemTemplate(CableItemMaster CableItem, int userId)
		{
			return new DACableItemMaster().SaveCableItemTemplate(CableItem, userId);
		}
		//public bool IsEntityTemplateExist(string entity_type, int user_id, string subEntityType)
		//{
		//    return new DACableItemMaster().IsEntityTemplateExist(entity_type, user_id, subEntityType);
		//}
	}

	public class BLROWItemMaster
	{
		public ROWItemMaster SaveROWItemTemplate(ROWItemMaster ROWItem, int userId)
		{
			return new DAROWItemMaster().SaveROWItemTemplate(ROWItem, userId);
		}
		//public bool IsEntityTemplateExist(string entity_type, int user_id, string subEntityType)
		//{
		//    return new DACableItemMaster().IsEntityTemplateExist(entity_type, user_id, subEntityType);
		//}
	}
	public class BLONTItemMaster
	{
		public ONTItemMaster SaveONTItemTemplate(ONTItemMaster ONTItem, int userId)
		{
			return new DAONTItemMaster().SaveONTItemTemplate(ONTItem, userId);
		}
	}

	public class BLDuctItemMaster
	{
		public DuctItemMaster SaveDuctItemTemplate(DuctItemMaster DuctItem, int userId)
		{
			return new DADuctItemMaster().SaveDuctItemTemplate(DuctItem, userId);
		}
	}


    public class BLGipipeItemMaster
    {
        public GipipeItemMaster SaveGipipeItemTemplate(GipipeItemMaster GipipeItem, int userId)
        {
            return new DAGipipeItemMaster().SaveGipipeItemTemplate(GipipeItem, userId);
        }
    }

    public class BLConduitItemMaster
    {
        public ConduitItemMaster SaveConduitItemTemplate(ConduitItemMaster ConduitItem, int userId)
        {
            return new DAConduitItemMaster().SaveConduitItemTemplate(ConduitItem, userId);
        }
    }

	public class BLMicroductItemMaster
	{
		public MicroductItemMaster SaveMicroductItemTemplate(MicroductItemMaster DuctItem, int userId)
		{
			return new DAMicroductItemMaster().SaveMicroductItemTemplate(DuctItem, userId);
		}
	}

    public class BLTowerItemMaster
    {
        public TowerItemMaster SaveTowerItemTemplate(TowerItemMaster DuctItem, int userId)
        {
            return new DATowerItemMaster().SaveTowerItemTemplate(DuctItem, userId);
        }
    }

    public class BLSectorItemMaster
    {
        public SectorItemMaster SaveSectorItemTemplate(SectorItemMaster DuctItem, int userId)
        {
            return new DASectorItemMaster().SaveSectorItemTemplate(DuctItem, userId);
        }
    }

    public class BLAntennaItemMaster
    {
        public AntennaItemMaster SaveAntennaItemTemplate(AntennaItemMaster DuctItem, int userId)
        {
            return new DAAntennaItemMaster().SaveAntennaItemTemplate(DuctItem, userId);
        }
    }

    public class BLMicrowaveLinkItemMaster
    {
        public MicrowaveLinkItemMaster SaveMicrowaveLinkItemTemplate(MicrowaveLinkItemMaster DuctItem, int userId)
        {
            return new DAMicrowaveLinkItemMaster().SaveMicrowaveLinkItemTemplate(DuctItem, userId);
        }
    }



    public class BLTrenchItemMaster
	{
		public TrenchItemMaster SaveTrenchItemTemplate(TrenchItemMaster TrenchItem, int userId)
		{
			return new DATrenchItemMaster().SaveTrenchItemTemplate(TrenchItem, userId);
		}
	}
	public class BLWallMountItemMaster
	{
		public WallMountItemMaster SaveWallMountItemTemplate(WallMountItemMaster WallItem, int userId)
		{
			return new DAWallMountItemMaster().SaveWallMountItemTemplate(WallItem, userId);
		}
	}
    public class BLPEPItemMaster
    {
        public PEPItemMaster SavePEPItemTemplate(PEPItemMaster PEPItem, int userId)
        {
            return new DAPEPItemMaster().SavePEPItemTemplate(PEPItem, userId);
        }
    }
    public class BLPatchCordItemMaster
	{
		public PatchCordItemMaster SavePatchCordItemTemplate(PatchCordItemMaster CableItem, int userId)
		{
			return new DAPatchCordItemMaster().SavePatchCordItemTemplate(CableItem, userId);
		}
	}

    //cabinet shazia 
    public class BLCabinetItemMaster
    {
        public CabinetItemMaster SaveCabinetItemTemplate(CabinetItemMaster CabinetItem, int userId)
        {
            return new DACabinetItemMaster().SaveCabinetItemTemplate(CabinetItem, userId);
        }
    }
    //cabinet shazia 
    //vault shazia 
    public class BLVaultItemMaster
    {
        public VaultItemMaster SaveVaultItemTemplate(VaultItemMaster VaultItem, int userId)
        {
            return new DAVaultItemMaster().SaveVaultItemTemplate(VaultItem, userId);
        }
    }
	//vault shazia end 

	public class BLPatchPanelItemMaster
	{
		public PatchPanelItemMaster SavePatchPanelItemTemplate(PatchPanelItemMaster item, int userId)
		{
			return new DAPatchPanelItemMaster().SavePatchPanelItemTemplate(item, userId);
		}
	}
}
