using BusinessLogics;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
namespace DataUploader
{
    public class ItemSpecification : ISpecification
    {
        public List<KeyValueDropDown> GetSpecification()
        {
            return BLItemTemplate.Instance.GetDropDownList();
        }
        public static int GetIdByColumnName(string columnName, List<KeyValueDropDown> keyValueDropDown, string rowCurrentValue)
        {
            KeyValueDropDown keyValue = keyValueDropDown.Where(m => m.value == columnName.ToLower() && m.ddtype == rowCurrentValue.ToLower()).FirstOrDefault();
            if (keyValue == null) return 0;
            return commonUtil.ToInt(keyValue.key);
        }
    }
}
