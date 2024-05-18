using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using DataAccess.DBContext;
using Models;
using Models.Admin;
using static Mono.Security.X509.X520;

namespace DataAccess
{
    public class DAFetools_Attachement : Repository<FETOOLS_Attachment>
    {
        public FETOOLS_Attachment SaveFetools_attachement(FETOOLS_Attachment objAttachment)
        {
            var resultItem = new FETOOLS_Attachment ();
            try
            {
                
                
                    var objExisiting = repo.GetById(m => m.fe_tool_id == objAttachment.fe_tool_id );

                    if (objExisiting != null)
                    {
                    objAttachment.id = objExisiting.id;
                        resultItem = repo.Update(objAttachment);
                        
                    }
                
                else
                {

                    resultItem = repo.Insert(objAttachment);
                    
                }
                return resultItem;
            }
            catch { throw; }

        }
        public FETOOLS_Attachment getFE_toolsAttachmentsbyid(int id,string uploadtype)
        {
            try
            {
                return repo.GetById(m => m.fe_tool_id == id && m.upload_type == uploadtype);
            }
            catch { throw; }
        }
    }
}
