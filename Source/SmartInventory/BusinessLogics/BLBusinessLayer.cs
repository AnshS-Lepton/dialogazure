using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLBusinessLayer
    {
        private static BLBusinessLayer objBusinessLayer = null;
        private static readonly object lockObject = new object();
        public static BLBusinessLayer Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBusinessLayer == null)
                    {
                        objBusinessLayer = new BLBusinessLayer();
                    }
                }
                return objBusinessLayer;
            }
        }

        public BusinessLayer SaveBusinessLayer(BusinessLayer businessLayer)
        {
            return DABusinessLayer.Instance.SaveBusinessLayer(businessLayer);
        }

        public void SaveBusinessLayer(List<BusinessLayer> businessLayer)
        {
            DABusinessLayer.Instance.SaveBusinessLayer(businessLayer);
        }
        public BusinessLayer UpdateBusinessLayerByID(BusinessLayer businessLayer,User objLgnUsrDtl)
        {
            return DABusinessLayer.Instance.UpdateBusinessLayerByID(businessLayer, objLgnUsrDtl);
        }
        public List<BusinessLayer> GetBusinessLayerList(CommonGridAttributes objGridAttributes)
        {
            return new DABusinessLayer().GetBusinessLayerList(objGridAttributes);
        }
        public BusinessLayer GetBusinessLayer(int id)
        {
            return new DABusinessLayer().GetBusinessLayer(id);
        }
        public int DeleteBusinessLayerById(int id)
        {
            return new DABusinessLayer().DeleteBusinessLayerById(id);

        }
        public IEnumerable<BusinessLayer> GetBusinessAllLayer()
        {
            return new DABusinessLayer().GetBusinessAllLayer();
        }
        public BusinessLayer GetBusinessLayerByLayername(string baseurl,string layer_name)
        {
            return new DABusinessLayer().GetBusinessLayerByLayername(baseurl,layer_name);
        }
    }
    
}
