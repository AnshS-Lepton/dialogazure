using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using iTextSharp.text;
using Models.Admin;
using System.Data.Entity.Core.Metadata.Edm;
using NPOI.SS.Formula.Functions;

namespace DataAccess
{
    public class DABom : Repository<BOMReport>
    {
        private static DABom objBom = null;
        private static readonly object lockObject = new object();
        public static DABom Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBom == null)
                    {
                        objBom = new DABom();
                    }
                }
                return objBom;
            }
        }
        public List<BOMReport> getBOMReport(BOMInput objInput)
        {
            try
            {
                if (objInput.isAssociatedEntityBomBoq)
                {
                    return repo.ExecuteProcedure<BOMReport>("fn_get_associated_entity_bom_boq_report", new { p_system_id = objInput.systemId, p_entity_type = objInput.entityType, p_network_status = objInput.network_status, p_userid = objInput.userId, p_roleid = objInput.roleId }, true);
                }
                else
                {
                    return repo.ExecuteProcedure<BOMReport>("fn_get_bom_boq_report", new { P_GEOM = objInput.geom, P_NETWORK_STATUS = objInput.network_status, P_RADIUS = objInput.buff_Radius, P_GEOM_TYPE = objInput.geomType, p_provinceids = objInput.SelectedProvinceIds, p_regionids = objInput.SelectedRegionIds, p_userid = objInput.userId, p_roleid = objInput.roleId }, true);
                }
            }
            catch { throw; }
        }

        public List<BOMReport> getISPBOMReport(int structure_id, int building_id)
        {
            try
            {
                return repo.ExecuteProcedure<BOMReport>("FN_ISP_GET_BOM_BOQ_REPORT", new { P_BUILDING_ID = building_id, P_STRUCTURE_ID = structure_id }, true);

            }
            catch { throw; }
        }
        public List<BOMReport> getAssociateEntityBOMReport(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<BOMReport>("fn_get_associated_entity_bom_boq_report", new { p_system_id = systemId, p_entity_type = entityType }, true);

            }
            catch { throw; }
        }

    }
    public class DABomBoq : Repository<BomBoqAdAttribute>
    {
        public List<BOMBOQReport> getBOMBOQReport(BomBoqExportFilter objFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<BOMBOQReport>("fn_get_bom_boq_report_new",
                    new
                    {
                        p_regionids = objFilter.SelectedRegionIds,
                        p_provinceids = objFilter.SelectedProvinceIds,
                        p_network_status = objFilter.SelectedNetworkStatues,
                        p_layerids = objFilter.SelectedLayerIds,
                        p_projectcodes = objFilter.SelectedProjectIds,
                        p_planningcodes = objFilter.SelectedPlanningIds,
                        p_workordercodes = objFilter.SelectedWorkOrderIds,
                        p_purposecodes = objFilter.SelectedPurposeIds,
                        p_durationbasedon = (objFilter.durationbasedon == null || objFilter.durationbasedon == "" ? "Created_On" : objFilter.durationbasedon),
                        p_fromdate = objFilter.fromDate,
                        p_todate = objFilter.toDate,
                        p_geom = objFilter.geom,
                        p_userid = objFilter.userId,
                        p_roleid = objFilter.roleId,
                        p_ownership_type = objFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objFilter.SelectedThirdPartyVendorIds,
                        p_geom_type = objFilter.geomType,
                        p_radius = objFilter.radius,
                        p_parentusers = objFilter.SelectedParentUsers,
                        p_userids = objFilter.SelectedUserIds,
                        p_systemId = objFilter.systemId,
                        p_entityType = objFilter.entityType,
                        p_route = objFilter.selected_route_ids
                    }, true);
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<BomBoqAdAttribute> getBOMBOQExportAttribute(int userid)
        {
            return repo.ExecuteProcedure<BomBoqAdAttribute>("fn_get_BomBoq_Exp_Attribute", new { p_userid = userid }, true).ToList();
        }


        public BomBoqAdAttribute Save_ExportAttribute(BomBoqAdAttribute objInputattr)
        {
            BomBoqAdAttribute result = new BomBoqAdAttribute();

            var objattr = repo.Get(x => x.userid == objInputattr.userid);
            try
            {
                // Save Link Budget detail..
                if (objattr != null)
                {
                    objattr.title = objInputattr.title;
                    //objattr.equipmenttype = objInputattr.equipmenttype;
                    //objattr.equipmentname = objInputattr.equipmentname;
                    //objattr.popname = objInputattr.popname;
                    objattr.estimatedby = objInputattr.estimatedby;
                    objattr.checkedby = objInputattr.checkedby;
                    objattr.re_checkedby = objInputattr.re_checkedby;
                    objattr.approvedby = objInputattr.approvedby;
                    result = repo.Update(objattr);
                }
                else
                {
                    //objattr.userid = objInputattr.userid;
                    //objattr.title = objInputattr.title;
                    ////objattr.equipmenttype = objInputattr.equipmenttype;
                    ////objattr.equipmentname = objInputattr.equipmentname;
                    ////objattr.popname = objInputattr.popname;
                    //objattr.estimatedby = objInputattr.estimatedby;
                    //objattr.checkedby = objInputattr.checkedby;
                    //objattr.re_checkedby = objInputattr.re_checkedby;
                    //objattr.approvedby = objInputattr.approvedby;
                    //objattr.modified_on = DateTimeHelper.Now;
                    result = repo.Insert(objInputattr);

                }


                return result;
            }

            catch { throw; }

        }

        public List<dBLossDetail> getdBLossReport(BomBoqExportFilter objFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<dBLossDetail>("fn_get_bom_boq_loss_detail",
                    new
                    {
                        p_regionids = objFilter.SelectedRegionIds,
                        p_provinceids = objFilter.SelectedProvinceIds,
                        p_network_status = objFilter.SelectedNetworkStatues,
                        p_layerids = objFilter.SelectedLayerIds,
                        p_projectcodes = objFilter.SelectedProjectIds,
                        p_planningcodes = objFilter.SelectedPlanningIds,
                        p_workordercodes = objFilter.SelectedWorkOrderIds,
                        p_purposecodes = objFilter.SelectedPurposeIds,
                        p_durationbasedon = (objFilter.durationbasedon == null || objFilter.durationbasedon == "" ? "Created_On" : objFilter.durationbasedon),
                        p_fromdate = objFilter.fromDate,
                        p_todate = objFilter.toDate,
                        p_geom = objFilter.geom,
                        p_userid = objFilter.userId,
                        p_roleid = objFilter.roleId,
                        p_ownership_type = objFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objFilter.SelectedThirdPartyVendorIds,
                        p_geom_type = objFilter.geomType,
                        p_radius = objFilter.radius,
                        p_misc_loss = objFilter.txt_Miscdbloss,
                        p_wavelength_id = objFilter.SelectedWavelength,
                        p_parentusers = objFilter.SelectedParentUsers,
                        p_userids = objFilter.SelectedUserIds
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public List<BomBoqAdAttribute> getLayersStructuerWise(int userid)
        {
            return repo.ExecuteProcedure<BomBoqAdAttribute>("fn_get_BomBoq_Exp_Attribute", new { p_userid = userid }, true).ToList();
        }


    }

    public class DABomBoqSite : Repository<Models.Admin.VendorSpecificationMaster>
    {
        public List<VendorSpecificationMaster> getSiteBOMBOQReport(CommonGridAttr objGridAttributes,int sitePlanid)
        {
            try
            {
                var lst = repo.ExecuteProcedure<VendorSpecificationMaster>("fn_get_site_bom_boq_report_new",
                new
                {
                    p_entitytype = "POD",
                    p_site_plan_id = sitePlanid,
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    P_TOTALRECORDS = objGridAttributes.totalRecord,
                }, true);
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }
    }
    public class DABomBoqSite1 : Repository<PODMaster>
    {
        public int getSiteplanid(int iBomBoqId)
        {
            //var userDetails = repo.GetAll(x => x.bom_boq_id == iBomBoqId).FirstOrDefault();
            return repo.GetAll(x => x.system_id == iBomBoqId).Select(a=>a.site_plan_id).FirstOrDefault();
        }
    }
    public class DABomBoqInfoSummary : Repository<BomBoqInfoSummary>
    {

        public BomBoqInfoSummary GetBomBoqRevisionSummary(int iBomBoqId)
        {
            //var userDetails = repo.GetAll(x => x.bom_boq_id == iBomBoqId).FirstOrDefault();
            return repo.Get(x => x.bom_boq_id == iBomBoqId);
        }
        public BomBoqInfoSummary GetBOMBOQDesignReport(BomBoqExportFilterDesign objFilter)
        {
            try
            {
                BomBoqInfoSummary bomBoqInfoSummary = new BomBoqInfoSummary();
                List<BomBoqInfo> lstBomBoqInfo = repo.ExecuteProcedure<BomBoqInfo>("fn_get_bom_boq_report_by_boundary",
                    new
                    {
                        p_bom_boq_id = objFilter.bom_boq_id,
                        p_boundary_system_id = objFilter.systemId,
                        p_boundary_name = objFilter.entityName,
                        p_userid = objFilter.userId,
                        p_user_name = objFilter.user_name,
                        p_action=objFilter.action
                    }, true);
                if (objFilter.bom_boq_id == 0)
                {
                    objFilter.bom_boq_id = lstBomBoqInfo.Count > 0 ? lstBomBoqInfo[0].bom_boq_id : 0;
                }
                if (objFilter.bom_boq_id != 0)
                {
                    bomBoqInfoSummary = GetBomBoqRevisionSummary(objFilter.bom_boq_id);
                }
                else
                {
                    bomBoqInfoSummary.status = "NA";
                }
                bomBoqInfoSummary.lstBomBoqInfo = lstBomBoqInfo;
                return bomBoqInfoSummary;
            }
            catch  { throw; }
        }

        public List<BomBoqInfo> GetBOMBOQDependentItems(int iBomBoqId, int iBoundarySystemId, string sEntityClass, string sEntitySubClass, string sAction)
        {
            try
            {               
                List<BomBoqInfo> lstBomBoqInfo = repo.ExecuteProcedure<BomBoqInfo>("fn_get_bom_boq_dependent_items",
                    new
                    {
                        p_bom_boq_id = iBomBoqId,
                        p_boundary_system_id = iBoundarySystemId,                          
                        p_action = sAction,
                        p_entity_class = sEntityClass,
                        p_entity_sub_class = sEntitySubClass
                    }, true); 
                return lstBomBoqInfo;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<string> SaveBOMBOQReportDesign(List<BomBoqInfo> lstBomBoqInfo)
        {
            try
            {
                var lst = repo.ExecuteProcedure<string>("fn_save_bom_boq_info",
                    new
                    {
                        p_input = Newtonsoft.Json.JsonConvert.SerializeObject(lstBomBoqInfo)

                    }, false); ;
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }

        public string UpdateBomBoqStatus(BomBoqInfoSummary objBomBoqInfoSummary)
        {
            //bool retVal = false;
            List<string> sRetVal = new List<string>();
            try
            {
                sRetVal = repo.ExecuteProcedure<string>("fn_update_bom_boq_status",
                    new
                    {
                        p_bom_boq_id = objBomBoqInfoSummary.bom_boq_id,
                        p_status = objBomBoqInfoSummary.status,
                        p_user_id = objBomBoqInfoSummary.modified_by_user_id,
                        p_user_name = objBomBoqInfoSummary.modified_user_name
                    }, false);

            }
            catch (Exception ex) { throw ex; }
            return sRetVal.FirstOrDefault();
        }

        public List<BomBoqInfo> ExportBOMBOQ(BomBoqExportFilterDesign objFilter)
        {
            try
            {
                List<BomBoqInfo> lstBomBoqInfo = repo.ExecuteProcedure<BomBoqInfo>("fn_get_bom_boq_report_by_boundary",
                    new
                    {
                        p_bom_boq_id = objFilter.bom_boq_id,
                        p_boundary_system_id = objFilter.systemId,
                        p_boundary_name = objFilter.entityName,
                        p_userid = objFilter.userId,
                        p_user_name = objFilter.user_name,
                        p_action = objFilter.action
                    }, true);
                return lstBomBoqInfo;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<KeyValueDropDown> GetBomBoqProjectCode(int bom_boq_id)
        {
            try
            {
                List<KeyValueDropDown> lstBomBoqInfo = repo.ExecuteProcedure<KeyValueDropDown>("fn_get_bom_boq_projectcode",
                    new
                    {
                        p_bom_boq_id = bom_boq_id
                        
                    }, true);
                return lstBomBoqInfo;
            }
            catch (Exception ex) { throw ex; }
        }

    }

    // for constructionBOMDetails
    public class DAConstructionBOMDetails : Repository<ConstructionBomDetails>
    {
        public List<ConstructionBomDetailsInfo> GetConstructionBomDetailList(ConstructionBomDetailsVM objConstructionBomDetailsVM)
        {
            try
            {

                return repo.ExecuteProcedure<ConstructionBomDetailsInfo>("fn_get_construction_bom_details", new
                {
                    p_searchby = Convert.ToString(objConstructionBomDetailsVM.viewConstructionBomDetails.searchBy),
                    p_searchtext = Convert.ToString(objConstructionBomDetailsVM.viewConstructionBomDetails.searchText),
                    P_PAGENO = objConstructionBomDetailsVM.viewConstructionBomDetails.currentPage,
                    P_PAGERECORD = objConstructionBomDetailsVM.viewConstructionBomDetails.pageSize,
                    P_SORTCOLNAME = objConstructionBomDetailsVM.viewConstructionBomDetails.sort,
                    P_SORTTYPE = objConstructionBomDetailsVM.viewConstructionBomDetails.orderBy
                }, true);
            }
            catch { throw; }
        }
        //public ConstructionBomDetails GetConstructionBomDetailsByID(int id)
        //{
        //    try
        //    {
        //        var objConstructionBomDetails = repo.Get(m => m.id == id);

        //        return objConstructionBomDetails;
        //    }
        //    catch { throw; }
        //}
        public ConstructionBomDetails GetConstructionBomDetailsByID(int id)
        {
            try
            {

               var condefault= repo.ExecuteProcedure<ConstructionBomDetails>("fn_construction_set_to_default", new
                {
                     p_subarea_system_id =id
                    
                }, true).FirstOrDefault();
                return condefault;
            }
            catch { throw; }
        }
        public ConstructionBomDetails SaveConstructionBomDetails(ConstructionBomDetails objConstructionBomDetails)
        {
            try
            {
                objConstructionBomDetails = repo.Update(objConstructionBomDetails);

                return objConstructionBomDetails;
            }
            catch { throw; }
        }
    }
    public class DAFSABomUnlockDetailsInfo : Repository<FSAlockedDetailsInfo>
    {
        public List<FSAlockedDetailsInfo> GetFSAUnlockBomDetailList(FSAlockedDetailsVM objFSAlockedDetailsVM)
        {
            try
            {

                return repo.ExecuteProcedure<FSAlockedDetailsInfo>("fn_get_locked_subarea_details", new
                {
                    p_searchby = Convert.ToString(objFSAlockedDetailsVM.viewLockedDetails.searchBy),
                    p_searchtext = Convert.ToString(objFSAlockedDetailsVM.viewLockedDetails.searchText),
                    P_PAGENO = objFSAlockedDetailsVM.viewLockedDetails.currentPage,
                    P_PAGERECORD = objFSAlockedDetailsVM.viewLockedDetails.pageSize,
                    P_SORTCOLNAME = objFSAlockedDetailsVM.viewLockedDetails.sort,
                    P_SORTTYPE = objFSAlockedDetailsVM.viewLockedDetails.orderBy
                }, true);
            }
            catch { throw; }
        }
        public FSAlockedDetailsInfo UnlockFSAByID(int id)
        {
            try
            {

                var condefault = repo.ExecuteProcedure<FSAlockedDetailsInfo>("fn_unlock_subarea", new
                {
                    p_boundaryid = id

                }, true).FirstOrDefault();
                return condefault;
            }
            catch { throw; }
        }

    }
}
