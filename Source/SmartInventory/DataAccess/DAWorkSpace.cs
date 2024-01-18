using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAWorkSpace : Repository<WorkSpaceMaster>
    {
        public List<WorkSpaceMaster> GetworkSpaceDetails(int userId)
        {
            try
            {
                return repo.GetAll(p => p.user_id == userId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public WorkSpaceMaster GetworkSpaceById(int workSpaceId)
        {
            try
            {
                return repo.GetById(workSpaceId);
            }
            catch { throw; }
        }

        public bool SaveWorkSpace(WorkSpaceMaster workspace)
        {
            try
            {
                // UPDATE LOGIC IS PENDING..
                var objworkspace = repo.GetById(x => x.id == workspace.id, x => x.WSLayers, x => x.WSRegionProvince,x=>x.WSLandbaseLayers);
                if (objworkspace != null)
                {
                    objworkspace.zoom = workspace.zoom;
                    objworkspace.lat = workspace.lat;
                    objworkspace.lng = workspace.lng;
                    objworkspace.has_label = workspace.has_label;
                    objworkspace.modified_on = DateTimeHelper.Now;
                    repo.Update(objworkspace);

                    #region MANAGE WS LAYERS
                    DAWorkSpaceLayer objDAWSLayers = new DAWorkSpaceLayer();
                    //remove exisiting layers
                    if (objworkspace.WSLayers != null && objworkspace.WSLayers.Count > 0)
                    {
                        objDAWSLayers.DeleteWSLayers(objworkspace.WSLayers.ToList());
                    }
                    //add new WS layers
                    if (workspace.WSLayers != null && workspace.WSLayers.Count > 0)
                    {
                        // update workspaceId...
                        foreach (var WSLayer in workspace.WSLayers)
                        {
                            WSLayer.workspace_id = objworkspace.id;
                        }
                        // insert WS Layers....
                        objDAWSLayers.InsertWSLayers(workspace.WSLayers.ToList());
                    }
                    #endregion
                    #region MANAGE WS LANDBASE LAYERS
                    DALandbaseWorkSpaceLayer objLandbaseLayers = new DALandbaseWorkSpaceLayer();
                    //remove exisiting layers
                    if (objworkspace.WSLandbaseLayers != null && objworkspace.WSLandbaseLayers.Count > 0)
                    {
                        objLandbaseLayers.DeleteWSLandbaseLayers(objworkspace.WSLandbaseLayers.ToList());
                    }
                    //add new WS layers
                    if (workspace.WSLandbaseLayers != null && workspace.WSLandbaseLayers.Count > 0)
                    {
                        // update workspaceId...
                        foreach (var WSLayer in workspace.WSLandbaseLayers)
                        {
                            WSLayer.workspace_id = objworkspace.id;
                        }
                        // insert WS Layers....
                        objLandbaseLayers.InsertWSLandbaseLayers(workspace.WSLandbaseLayers.ToList());
                    }
                    #endregion
                    #region MANAGE WS REGION PROVINCES
                    DAWorkSpaceRegionProvince objDAWSRegProvinces = new DAWorkSpaceRegionProvince();
                    //remove exisiting WS Region Provinces
                    if (objworkspace.WSRegionProvince != null && objworkspace.WSRegionProvince.Count > 0)
                    {
                        objDAWSRegProvinces.DeleteWSRegionProvinces(objworkspace.WSRegionProvince.ToList());
                    }
                    //add new WS Region Provinces
                    if (workspace.WSRegionProvince != null && workspace.WSRegionProvince.Count > 0)
                    {
                        // update workspaceId...
                        foreach (var ObjRegProvince in workspace.WSRegionProvince)
                        {
                            ObjRegProvince.workspace_id = objworkspace.id;
                        }
                        // insert WS Region Provinces.
                        objDAWSRegProvinces.InsertWSRegionProvinces(workspace.WSRegionProvince.ToList());
                    }
                    #endregion
                }
                else
                {
                    workspace.created_on = DateTimeHelper.Now;
                    repo.Insert(workspace);
                }
                return true;
            }
            catch { throw; }
        }

        public int DeleteWorkSpace(int workSpaceId)
        {
            try
            {
                int result = 0;
                var objworkspace = repo.Get(u => u.id == workSpaceId);
                if (objworkspace != null)
                {
                    result = repo.Delete(objworkspace);
                }
                return result;
            }
            catch (Exception ex) { throw; }
        }
    }
    public class DAWorkSpaceRegionProvince : Repository<WorkSpaceRegionProvince>
    {
        public int DeleteWSRegionProvinces(List<WorkSpaceRegionProvince> lstWSRegionProvinces)
        {
            try
            {
                int result = repo.DeleteRange(lstWSRegionProvinces);
                return result;
            }
            catch { throw; }
        }
        public void InsertWSRegionProvinces(List<WorkSpaceRegionProvince> lstWSRegionProvinces)
        {
            try
            {
                repo.Insert(lstWSRegionProvinces);
            }
            catch { throw; }
        }

        public ICollection<WorkSpaceRegionProvince> GetWorkSpaceRegionProvince(int userId, int workSpaceId)
        {
            try
            {
                ICollection<WorkSpaceRegionProvince> objRegionProvince = new List<WorkSpaceRegionProvince>();
                objRegionProvince = repo.GetAll(u => u.workspace_id == workSpaceId).ToList();
                return objRegionProvince;
            }
            catch { throw; }
        }
    }

    public class DAWorkSpaceLayer : Repository<WorkSpaceLayers>
    {
        public int DeleteWSLayers(List<WorkSpaceLayers> lstWSLayers)
        {
            try
            {
                int result = repo.DeleteRange(lstWSLayers);
                return result;
            }
            catch { throw; }
        }
        public void InsertWSLayers(List<WorkSpaceLayers> lstWSLayers)
        {
            try
            {
                repo.Insert(lstWSLayers);
            }
            catch { throw; }
        }


        public ICollection<WorkSpaceLayers> GetWorkSpaceLayers(int workSpaceId)
        {
            try
            {
                ICollection<WorkSpaceLayers> objLyrs = new List<WorkSpaceLayers>();
                objLyrs = repo.GetAll(u => u.workspace_id == workSpaceId).ToList();
                return objLyrs;
            }
            catch { throw; }
        }


    }

    public class DALandbaseWorkSpaceLayer : Repository<LandbaseWorkSpaceLayers>
    {
        public int DeleteWSLandbaseLayers(List<LandbaseWorkSpaceLayers> lstWSLayers)
        {
            try
            {
                int result = repo.DeleteRange(lstWSLayers);
                return result;
            }
            catch { throw; }
        }
        public void InsertWSLandbaseLayers(List<LandbaseWorkSpaceLayers> lstWSLayers)
        {
            try
            {
                repo.Insert(lstWSLayers);
            }
            catch { throw; }
        }

        public ICollection<LandbaseWorkSpaceLayers> GetLandbaseWorkSpaceLayers(int workSpaceId)
        {
            try
            {
                ICollection<LandbaseWorkSpaceLayers> objLyrs = new List<LandbaseWorkSpaceLayers>();
                objLyrs = repo.GetAll(u => u.workspace_id == workSpaceId).ToList();
                return objLyrs;
            }
            catch { throw; }
        }
    }
}
