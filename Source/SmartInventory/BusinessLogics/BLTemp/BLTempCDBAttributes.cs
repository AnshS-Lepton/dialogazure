using DataAccess;
using DataAccess.TempUpload;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.BLTemp
{
    public class BLTempCDBAttributes : BLDataUploader
    {
        DATempCDBAttributes DATempCDB;
        public BLTempCDBAttributes()
        {
            DATempCDB = new DATempCDBAttributes();
        }
        public void Save(List<TempCDBAttributes> lst)
        {
            DATempCDB.Save(lst);
        }
    }
}
