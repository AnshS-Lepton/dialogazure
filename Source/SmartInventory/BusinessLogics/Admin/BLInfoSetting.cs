using DataAccess.Admin;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
  public  class BLInfoSetting
    {
      public BLInfoSetting()
      {

      }
      private static BLInfoSetting objBLInfoSetting = null;
      private static readonly object lockObject = new object();
      public static BLInfoSetting Instance
      {
          get
          {
              lock (lockObject)
              {
                  if (objBLInfoSetting == null)
                  {
                      objBLInfoSetting = new BLInfoSetting();
                  }
              }
              return objBLInfoSetting;
          }
      }


    
      public InfoSetting SaveInfoSetting(InfoSetting objInfo)
      {
          return DAInfoSetting.Instance.SaveInfoSetting(objInfo);
      }

      public InfoSetting GetInfoSetting(int layerId)
      {
          return DAInfoSetting.Instance.GetInfoSetting(layerId);
      }
    }
}
