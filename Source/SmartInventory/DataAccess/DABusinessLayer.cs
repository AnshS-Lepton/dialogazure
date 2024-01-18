using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DABusinessLayer : Repository<BusinessLayer>
    {
        private static DABusinessLayer objBusinessLayer = null;
        private static readonly object lockObject = new object();
        public static DABusinessLayer Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBusinessLayer == null)
                    {
                        objBusinessLayer = new DABusinessLayer();
                    }
                }
                return objBusinessLayer;
            }
        }
        public BusinessLayer SaveBusinessLayer(BusinessLayer businessLayer)
        {
            businessLayer = repo.Insert(businessLayer);
            return businessLayer;
        }
        public void SaveBusinessLayer(List<BusinessLayer> businessLayer)
        {
           repo.Insert(businessLayer);
        }
        public BusinessLayer UpdateBusinessLayerByID(BusinessLayer businessLayer, User objLgnUsrDtl)
        {
            var item = repo.GetAll(x => x.id == businessLayer.id).FirstOrDefault();
            if (item != null)
            {
                item.modified_by = objLgnUsrDtl.user_id;
                item.modified_on = DateTimeHelper.Now;
                item.layer_name = businessLayer.layer_name;
                item.layer_type = businessLayer.layer_type;
                item.display_layer_name = businessLayer.display_layer_name;
                item.is_active = businessLayer.is_active;
                item.base_url = businessLayer.base_url;
                item.authentication_key = businessLayer.authentication_key;
                item.style = businessLayer.style;
                item.version = businessLayer.version;
                item.used_for = businessLayer.used_for;
                item.tilematrixset = businessLayer.tilematrixset;
                item.url_display_name = businessLayer.url_display_name;
            }

            return repo.Update(item);
        }
        public BusinessLayer GetBusinessLayer(int id)
        {
            return repo.GetAll(x => x.id == id).FirstOrDefault();
        }
        public IEnumerable<BusinessLayer> GetBusinessAllLayer()
        {
            return repo.GetAll(a => a.is_active == true);
        }
        public BusinessLayer GetBusinessLayerByLayername(string baseurl,string layer_name)
        {
            return repo.GetAll(x => x.layer_name == layer_name && x.base_url==baseurl).FirstOrDefault();
        }

        public List<BusinessLayer> GetBusinessLayerList(CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<BusinessLayer>("fn_get_business_layer_details", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    is_active = objGridAttributes.is_active,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                }, true);
            }
            catch { throw; }
        }
        public int DeleteBusinessLayerById(int id)
        {
            try
            {
                var objBusinessId = repo.Get(x => x.id == id);
                if (objBusinessId != null)
                {
                    return repo.Delete(objBusinessId.id);
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
