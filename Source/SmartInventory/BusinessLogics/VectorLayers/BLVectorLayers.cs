using BusinessLogics.ISP;
using DataAccess;
using DataAccess.VectorLayers;
using Models;
using Models.VectorLayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.VectorLayers
{
    public class BLVectorLayers
    {
        private static BLVectorLayers objBLVectorLayers = null;
        private static readonly object lockObject = new object();
        public static BLVectorLayers Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBLVectorLayers == null)
                    {
                        objBLVectorLayers = new BLVectorLayers();
                    }
                }
                return objBLVectorLayers;
            }
        }

        #region Pole Data
        public GeoJson<dynamic> GetAllPolesVector()
        {
            return DAVectorLayers.Instance.GetAllPolesVector();
        }
        #endregion

        #region FDB
        public GeoJson<dynamic> GetAllFDBVector()
        {
            return DAVectorLayers.Instance.GetAllFDBVector();
        }
        #endregion

        #region All
        public List<VectorFeatures<dynamic>> GetAllLayersVector(VectorDataIn vectorDataIn, out DateTime dbServerDate)
        {
            return DAVectorLayers.Instance.GetAllLayersVector(vectorDataIn, out dbServerDate);
        }
        #endregion

        #region All Delta
        public List<VectorFeatures<dynamic>> GetAllLayersDelta(VectorDeltaIn vectorDeltaIn, out DateTime dbServerDate)
        {
            return DAVectorLayers.Instance.GetAllLayersDelta(vectorDeltaIn, out dbServerDate);
        }
        #endregion
        #region Get Province BBOX
        public List<VectorFeatures<dynamic>> GetVectorProvinceData(VectorProvinceDataIn oVectorProvinceDataIn)
        {
            return DAVectorLayers.Instance.GetVectorProvinceData(oVectorProvinceDataIn);
        }
        #endregion
        public List<LayerAttribute> GetVectorEntityStyle()
        {
            return DAVectorLayers.Instance.GetVectorEntityStyle();
        }

    }
}
