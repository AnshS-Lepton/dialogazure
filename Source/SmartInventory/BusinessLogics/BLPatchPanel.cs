using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;


namespace BusinessLogics
{
    public class BLPatchPanel
    {

        public PatchPanelMaster SaveEntityPatchPanel(PatchPanelMaster objPatchPanelMaster, int userId)
        {
            return new DAPatchPanel().SaveEntityPatchPanel(objPatchPanelMaster, userId);
        }
        public int DeletePatchPanelById(int systemId)
        {
            return new DAPatchPanel().DeletePatchPanelById(systemId);
        }

    }
}
