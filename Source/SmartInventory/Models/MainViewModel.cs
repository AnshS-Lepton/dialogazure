using Models.Feasibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MainViewModel
    {
        public List<NetworkLayer> lstNetworkLayers { get; set; }
        public List<RegionProvinceLayer> lstRegionProvinceLayers { get; set; }
        public List<landBaseLayres> lstLandBaseLayers { get; set; }
        public List<Admin.OrthoImageMasterModel> lstOrthoImageLayers { get; set; }
        public User userDetail { get; set; }
        public List<string> lstUserModule { get; set; }
        public List<ViewGroupLibrary> lstGroupLibrary { get; set; }
        public List<NEntityLayers> NELayers { get; set; }
        public IEnumerable<BusinessLayer> lstBusinessLayer { get; set; } = new List<BusinessLayer>();
        public RoleViewModel roleViewModel { get; set; }
        public List<RoleViewModel> lstTemplateTicketTypePermission { get; set; }
        public List<DropDownMaster> listcableCategory { get; set; }

        public RoleMaster objRoleMaster { get; set; }
        public MainViewModel()
        {
           objRoleMaster = new RoleMaster();
            lstGroupLibrary = new List<ViewGroupLibrary>();
            lstNetworkLayers = new List<NetworkLayer>();
            lstRegionProvinceLayers = new List<RegionProvinceLayer>();
            userDetail = new User();
            lstUserModule = new List<string>();
            lstLandBaseLayers = new List<landBaseLayres>();
            NELayers = new List<NEntityLayers>();
            lstOrthoImageLayers = new List<Admin.OrthoImageMasterModel>();
            lstTemplateTicketTypePermission = new List<RoleViewModel>();
            listcableCategory = new List<DropDownMaster>();
        }

        public Boolean IsBroadcastMessageEnabled { get; set; }//ankit

        public string connectionstring { get; set; }
    }
}
