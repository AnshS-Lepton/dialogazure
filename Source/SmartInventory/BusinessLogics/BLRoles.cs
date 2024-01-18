using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Security;
using Thinktecture.IdentityModel.WSTrust;
namespace BusinessLogics
{
    public class BLRoles
    {
        public RoleViewModel SaveRole(RoleViewModel objModel, int usrId)
        {
            return new DARoles().SaveRole(objModel, usrId);
        }
        public List<RoleMaster> GetAllRoles(CommonGridAttributes objGridAttributes)
        {
            return new DARoles().GetAllRoles(objGridAttributes);
        }

        public List<RoleMaster> GetReportingRoleByRoleId(int role_id, CommonGridAttributes objGridAttributes)
        {
            return new DARoles().GetReportingRoleByRoleId(role_id, objGridAttributes);
        }
        public List<RoleMaster> GetAllRoles()
        {
            return new DARoles().GetAllRoles();
        }
        

        public RoleMaster GetRoleByName(string roleName)
        {
            return new DARoles().GetRoleByName(roleName);
        }
        public RoleMaster getRole_with_permission(int Roleid)
        {
            return new DARoles().getRole_with_permission(Roleid);
        }
        public DbMessage DeleteRole(int Roleid)
        {
            return new DARoles().DeleteRole(Roleid);
        }
        public bool CheckRoleExist(string role_name, int role_id)
        {
            return new DARoles().CheckRoleExist(role_name, role_id);
        }
        public RoleMaster getUserRoleNameByRoleId(int roleId)
        {
            return new DARoles().getUserRoleNameByRoleId(roleId);
        }

    }
    public class BLReportingRoleMapping
    {
        public List<ReportingRoleMapping> SaveReportingRoleMapping(List<ReportingRoleMapping> lstReportingRoleMapping, int role_id)
        {
            return new DAReportingRoleMapping().SaveReportingRoleMapping(lstReportingRoleMapping, role_id);
        }
          
        public List<ReportingRoleMapping> GetReportingRoleMapping(int role_id)
        {
            return new DAReportingRoleMapping().GetReportingRoleMapping(role_id);
        }
    }

    public class BLTicketTypeRoleMapping
    {
        public List<TicketTypeRoleMapping> SaveTicketTypeRoleMapping(List<TicketTypeRoleMapping> lsticketTypeRoleMapping,int role_id)
        {
            return new DATicketTypeRoleMapping().SaveTicketTypeRoleMapping(lsticketTypeRoleMapping,role_id);
        }

        public List<TicketTypeRoleMapping> GetTicketTypeRoleMapping(int role_id)
        {
            return new DATicketTypeRoleMapping().GetTicketTypeRoleMapping(role_id);
        }
    }

    public class BLRolesLayersMapping
    {
        public bool SaveRoleLayerMapping(List<RolePermissionEntity> lstLayerMapping, int roleId, int userId, bool isNew)
        {
            return new DARolesLayersMapping().SaveRoleLayerMapping(lstLayerMapping, roleId, userId, isNew);
        }
        public List<RolePermissionEntity> GetRolePermissionDetails(int roleId)
        {
            return new DARolesLayersMapping().GetRolePermissionDetails(roleId);
        }
        public int DeleteRange(List<RolePermissionEntity> lstRolePermission)
        {
            return new DARolesLayersMapping().DeleteRange(lstRolePermission);
        }
    }
}
