using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Admin;
using DataAccess.DBHelpers;
using DataAccess.DBContext;
using DataAccess.Contracts;
using Models;

namespace DataAccess.Admin
{
    public class DAOpticalLinkBudget
    {
    }


    //public class DAWaveLength : Repository<WaveLength>
    //{


    //    public WaveLength SaveWaveLength(WaveLength objWaveLength, int userId)
    //    {
    //        try
    //        {

    //            if (objWaveLength.wavelength_id != 0)
    //            {
    //                objWaveLength.modified_by = userId;
    //                objWaveLength.modified_on = DateTimeHelper.Now;
    //                objWaveLength.is_active = true;
    //                return repo.Update(objWaveLength);

    //            }

    //            else
    //            {
    //                objWaveLength.created_by = userId;
    //                objWaveLength.is_active = true;
    //                return repo.Insert(objWaveLength);
    //            }

    //        }

    //        catch { throw; }

    //    }

    //    public List<WaveLength> getWaveLegth()
    //    {
    //        return repo.GetAll().ToList();
    //    }

    //}

    public class DALinkBudget : Repository<LinkBudgetMaster>
    {
        public LinkBudgetMaster GetLinkBudgetDetailByID(int _wavelengthId)
        {
            try
            {
                var obj = repo.GetById(m => m.wavelength_id == _wavelengthId);
                return obj;
            }
            catch { throw; }
        }

        public List<LinkBudgetMaster> GetWaveLength()
        {
            try
            {
                var obj = repo.GetAll().ToList();
                return obj;
            }
            catch { throw; }
        }
        public LinkBudgetMaster GetLinkBudgetDetailByWavelength(int WaveLength_value)
        {
            try
            {
                return repo.Get(w => w.wavelength_value == WaveLength_value);
            }
            catch { throw; }
        }

        public LinkBudgetMaster SaveLinkBudget(LinkBudgetMaster objLinkBudget, int userId)
        {
            LinkBudgetMaster result = new LinkBudgetMaster();
            SplitterLossMaster resultSplitter = new SplitterLossMaster();
            try
            {
                // Save Link Budget detail..
                if (objLinkBudget.wavelength_id != 0)
                {
                    objLinkBudget.modified_by = userId;
                    objLinkBudget.modified_on = DateTimeHelper.Now;
                    result = repo.Update(objLinkBudget);
                }
                else
                {
                    objLinkBudget.created_by = userId;
                    objLinkBudget.created_on = DateTimeHelper.Now;
                    result = repo.Insert(objLinkBudget);
                   
                }

                // Save Wavelength wise Splitter loss detail..

                var isSuccess = new DASplitterLoss().SaveSplitterLosses(objLinkBudget.lstSplitterLoss, userId, objLinkBudget.wavelength_id);

                // get splitter losses

                result.lstSplitterLoss = new DASplitterLoss().GetSplitterLossByWaveLength(result.wavelength_id);

                return result;
            }

            catch { throw; }

        }

        public List<LinkBudgetMaster> GetLinkBudgetList(CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<LinkBudgetMaster>("fn_get_LinkBudget_details", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    is_active = objGridAttributes.is_active,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy
                }, true);
            }
            catch { throw; }
        }
        public int DeleteLinkBudgetDetailsById(int id)
        {
            try
            {
                var objSystmId = repo.Get(x => x.wavelength_id == id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.wavelength_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }

    }

    public class DASplitterLoss : Repository<SplitterLossMaster>
    {
        public List<SplitterLossMaster> GetSplitterLossByWaveLength(int wavelengthId)
        {
            try
            {
                return repo.ExecuteProcedure<SplitterLossMaster>("fn_get_splitter_loss_by_wavelength", new { p_wavelength_id = wavelengthId }, false);
            }
            catch { throw; }
        }

        public bool SaveSplitterLosses(List<SplitterLossMaster> lstSplitterLosses, int userId,int wavelength_id)
        {
            try
            {
                foreach (SplitterLossMaster objSplitterLoss in lstSplitterLosses)
                {
                    if (objSplitterLoss.id != 0)
                    {
                        objSplitterLoss.wavelength_id = wavelength_id;
                        objSplitterLoss.modified_by = userId;
                        objSplitterLoss.modified_on = DateTimeHelper.Now;
                        repo.Update(objSplitterLoss);
                    }
                    else
                    {
                        objSplitterLoss.wavelength_id = wavelength_id;
                        objSplitterLoss.created_by = userId;
                        objSplitterLoss.created_on = DateTimeHelper.Now;
                        repo.Insert(objSplitterLoss);
                    }
                }

                return true;
            }

            catch { throw; }

        }

        public int DeleteSplitterLossByWaveLength(int wavelengthId)
        {
            try
            {
                var objLoss = repo.Get(x => x.wavelength_id == wavelengthId);
                if (objLoss != null)
                {
                    return repo.Delete(objLoss.wavelength_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }

    }

    //public class DALinkBudgetDetails : Repository<OpticalLinkBudgetDetail>
    //{
    //    public OpticalLinkBudgetDetail GetLinkBudgetDetailByID(int id)
    //    {
    //        var obj = repo.GetById(m => m.system_id == id);

    //        return obj;

    //    }


    //    public List<OpticalLinkBudgetDetail> GetLinkBudgetAllDetail()
    //    {
    //        var obj = repo.GetAll().ToList();

    //        return obj;

    //    }

    //}
}
