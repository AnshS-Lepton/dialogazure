using Models;
using System;
using System.Linq;
using Utility.DAUtility.DBHelpers;

namespace Utility.DAUtility
{ 
    public static class DAUtility 
    {

        public static PageMessage  ValidateModifiedDate(DateTime? oldDbDate, DateTime? newDate, int? newModified_by, int? oldModified_by)
        {
            PageMessage objPM = new PageMessage();
            long oldDateticks = 0;
            long newDateticks = 0; 
            if (oldDbDate != null)
            {
                oldDateticks = (long)((DateTime)oldDbDate).TimeOfDay.TotalMilliseconds;
            }
            if (newDate != null)
            {
               // DateTime newDate1 = newDate ?? DateTimeHelper.Now;
                newDateticks = (long)((DateTime)newDate).TimeOfDay.TotalMilliseconds;
            }

           if (oldDateticks < newDateticks && oldModified_by != null  && newModified_by != oldModified_by)
            //if (oldDateticks < newDateticks)
            {
                objPM.status = ResponseStatus.VALIDATION_FAILED.ToString(); 
                objPM.message =Resources.Resources.SI_GBL_GBL_GBL_GBL_006;
                return objPM;
            }
            return objPM;
        }


        public static bool CompareObjectProperties(object oldValue, object newValue)
        {
            bool isMatched = true;
            System.Reflection.FieldInfo[] fields = oldValue.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).ToArray();
            string[] arrIgnoreFields = new string[6] { "<id>k__BackingField", "<created_by>k__BackingField", "<modified_by>k__BackingField", "<created_on>k__BackingField", "<modified_on>k__BackingField", "<lstFaultStatus>k__BackingField" };
            foreach (System.Reflection.FieldInfo fi in fields)
            {
                //arrIgnoreFields.Contains(fi.getkey)
                if (!arrIgnoreFields.Contains(fi.Name) && Convert.ToString(fi.GetValue(oldValue)) != Convert.ToString(fi.GetValue(newValue)))
                {
                    isMatched = false;
                    return isMatched;
                }
            }
            return isMatched;
        }      
    }
    //For GisApiLogs
    public class DAGisUtility : Repository<GisApiLogs>
    {
        public bool SaveGisApiLogs(GisApiLogs objGisapiLogs)
        {
            try
            {
                repo.Insert(objGisapiLogs);
                return true;
            }
            catch { throw; }
        }
    }
}
