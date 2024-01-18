using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;
using DataAccess.DBContext;
using DataAccess.Contracts;
//using System.Web.Security;
//using NPOI.SS.Formula.Functions;

namespace DataAccess
{
    public class DARoles : Repository<RoleMaster>
    {
        public RoleViewModel SaveRole(RoleViewModel objModel, int usrId)
        {
            try
            {
                RoleMaster obj = new RoleMaster();
                var objRoleDetail = repo.Get(u => u.role_id == objModel.objRoleMaster.role_id);
                if (objRoleDetail == null)
                {
                    objModel.objRoleMaster.created_on = DateTimeHelper.Now;
                    objModel.objRoleMaster.created_by = usrId;
                    repo.Insert(objModel.objRoleMaster);
                }
                else
                {
                    objRoleDetail.modified_on = DateTimeHelper.Now;
                    objRoleDetail.modified_by = usrId;
                    objRoleDetail.is_active = objModel.objRoleMaster.is_active;
                    objRoleDetail.remarks = objModel.objRoleMaster.remarks;
                    objRoleDetail.reporting_role_id = objModel.objRoleMaster.reporting_role_id;
                    objRoleDetail.role_name = objModel.objRoleMaster.role_name;
                    repo.Update(objRoleDetail);
                }
            }
            catch { throw; }
            return objModel;
        }

        public List<RoleMaster> GetAllRoles(CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<RoleMaster>("fn_user_rights_get_roles", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy
                }, true);
            }
            catch (Exception ex) { throw ex; }
            // return repo.GetAll(m => m.is_active == true, "role_id" , "ASC", pagesize, pagenumber, out totalRecords).ToList(); 
            //  return repo.GetAll((x => x.cable_id == cableId), "cable_id", "DESC", pagesize, pagenumber, out totalRecords);
        }
        public List<RoleMaster> GetReportingRoleByRoleId(int role_id, CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<RoleMaster>("fn_reporting_role_name_list", new
                {
                    p_role_id = role_id,
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    p_pageno = objGridAttributes.currentPage,
                    p_pagerecord = objGridAttributes.pageSize,
                    p_sortcolname = objGridAttributes.sort,
                    p_sorttype = objGridAttributes.orderBy

                }, true); 
            }
            catch (Exception ex) { throw ex; }
            // return repo.GetAll(m => m.is_active == true, "role_id" , "ASC", pagesize, pagenumber, out totalRecords).ToList(); 
            //  return repo.GetAll((x => x.cable_id == cableId), "cable_id", "DESC", pagesize, pagenumber, out totalRecords);
        }
        public List<RoleMaster> GetAllRoles()
        {
            return repo.GetAll().ToList();

           
        }
       
        public RoleMaster GetRoleByName(string roleName)
        {

            //return repo.GetAll().Where(x=>x.role_name.ToLower()==roleName.ToLower()).FirstOrDefault();
            return repo.Get(x => x.role_name.ToLower() == roleName.ToLower());
        }

        public RoleMaster getRole_with_permission(int roleid)
        {
            try
            {
                var roleDetail = repo.ExecuteProcedure<RoleMaster>("fn_user_rights_get_role_permission", new { vroleid = roleid }, true);
                return roleDetail.Count() > 0 ? roleDetail[0] : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DbMessage DeleteRole(int Roleid)
        {
            return repo.ExecuteProcedure<DbMessage>("fn_user_rights_delete_role_template", new { in_role_id = Roleid }).FirstOrDefault();
        }
        public bool CheckRoleExist(string role_name, int role_id)
        {
            RoleMaster obj = repo.Get(m => m.role_name.ToLower() == role_name.ToLower() && m.role_id != role_id);
            return obj != null ? true : false;
        }
        public RoleMaster getUserRoleNameByRoleId(int roleId)
        {
            try
            {
                return repo.Get(u => u.role_id == roleId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
   
    public class DAReportingRoleMapping : Repository<ReportingRoleMapping>
    {
        public List<ReportingRoleMapping> SaveReportingRoleMapping(List<ReportingRoleMapping> lstReportingRoleMapping, int role_id)
        {
            List<ReportingRoleMapping> OldLst = GetReportingRoleMapping(role_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstReportingRoleMapping);
            return lstReportingRoleMapping;
        }
        public List<ReportingRoleMapping> GetReportingRoleMapping(int role_id)
        {
            return repo.GetAll(m => m.role_id == role_id).ToList();
            // return repo.GetAll(m => m.manager_id == user_id).ToList();
        }
    }

    public class DATicketTypeRoleMapping : Repository<TicketTypeRoleMapping>
    {
        public List<TicketTypeRoleMapping> SaveTicketTypeRoleMapping(List<TicketTypeRoleMapping> lsticketTypeRoleMapping, int role_id)
        {
            List<TicketTypeRoleMapping> OldLst = GetTicketTypeRoleMapping(role_id);
            repo.DeleteRange(OldLst);
            if(lsticketTypeRoleMapping.Count>0)
            {
                repo.Insert(lsticketTypeRoleMapping);
            }          
            return lsticketTypeRoleMapping;
        }
        public List<TicketTypeRoleMapping>GetTicketTypeRoleMapping(int role_id)
        {
            return repo.GetAll(m => m.role_id == role_id).ToList();           
        }
    }
    public class DARolesLayersMapping : Repository<RolePermissionEntity>
    {
        public bool SaveRoleLayerMapping(List<RolePermissionEntity> lstLayerMapping, int roleId, int userId, bool isNew)
        {
            if (isNew)
            {
                //insert
                lstLayerMapping.ForEach(m => m.role_id = roleId);
                lstLayerMapping.ForEach(m => m.created_by = userId);
                lstLayerMapping.ForEach(m => m.created_on = DateTimeHelper.Now);
                repo.Insert(lstLayerMapping);
                UpdateRoleModuleMapping(roleId);
            }
            else
            {
                var lstpermission = repo.GetAll(a => a.role_id == roleId).ToList();
                repo.DeleteRange(lstpermission);
                //update..
                lstLayerMapping.ForEach(m => m.modified_by = userId);
                lstLayerMapping.ForEach(m => m.modified_on = DateTimeHelper.Now);
                lstLayerMapping.ForEach(m => m.role_id = roleId);
                repo.Insert(lstLayerMapping);
            }
            
            return true;
        }
        public DbMessage UpdateRoleModuleMapping(int role_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_insert_role_module_mapping", new { p_role_id = role_id }).FirstOrDefault();
            }
            catch { throw; }
        }

        public List<RolePermissionEntity> GetRolePermissionDetails(int RoleId)
        {
            return repo.GetAll(a => a.role_id == RoleId).ToList();
        }
        public int DeleteRange(List<RolePermissionEntity> lstRolePermission)
        {
            var IsDeleted = repo.DeleteRange(lstRolePermission);
            return IsDeleted;
        }
    }
   
}
