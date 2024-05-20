using DataAccess;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLFetools_Attachement
    {
        public FETOOLS_Attachment SaveFetools_attachement(FETOOLS_Attachment objAttachment)
        {
            return new DAFetools_Attachement().SaveFetools_attachement(objAttachment);
        }
        public FETOOLS_Attachment getFE_toolsAttachmentsbyid(int id,string uploadtype)
        {
            return new DAFetools_Attachement().getFE_toolsAttachmentsbyid(id,uploadtype);
        }
    }
}
