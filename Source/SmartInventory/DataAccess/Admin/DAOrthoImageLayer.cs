using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;
using Models.Admin;

namespace DataAccess.Admin
{
    public class DAOrthoImageLayer : Repository<OrthoImageMasterModel>
    {
        public List<OrthoImageMasterModel> GetOrthoImageLayerList(OrthoImageModel objViewOrthoImage, int user_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<OrthoImageMasterModel>("fn_orthoimagelayer_get_list", new
                {
                    p_pageno = objViewOrthoImage.objFilterAttributes.currentPage,
                    p_pagerecord = objViewOrthoImage.objFilterAttributes.pageSize,
                    p_sortcolname = objViewOrthoImage.objFilterAttributes.sort,
                    p_sorttype = objViewOrthoImage.objFilterAttributes.orderBy,
                    p_userid = user_id,
                    p_isactive = objViewOrthoImage.objFilterAttributes.status,
                    p_searchBy = objViewOrthoImage.objFilterAttributes.searchBy,
                    p_searchText = objViewOrthoImage.objFilterAttributes.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }
        public OrthoImageMasterModel GetOrthoImageById(int system_id)
        {
            try
            {
                return repo.Get(u => u.system_id == system_id);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public OrthoImageMasterModel SaveOrthoImage(OrthoImageMasterModel input, int userId)
        {
            try
            {
                var objOrthoImageMst = repo.Get(x => x.system_id == input.system_id);
                if (input.system_id > 0)
                {
                    objOrthoImageMst.is_active = input.is_active;
                    objOrthoImageMst.image_name = input.image_name;
                    objOrthoImageMst.image_extension = input.image_extension;
                    objOrthoImageMst.display_name = input.display_name;
                    objOrthoImageMst.modified_by = userId;
                    objOrthoImageMst.modified_on = DateTimeHelper.Now;
                    return repo.Update(objOrthoImageMst);

                }
                else
                {
                    input.created_by = userId;
                    input.created_on = DateTimeHelper.Now; ;
                    return repo.Insert(input);


                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int DeleteOrthoImageById(int id, int userId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }

        }
    }
}
