using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DataAccess.Admin
{
    public class DAVendor : Repository<CreateVendor>
    {

        public CreateVendor SaveEntityVendorDetails(CreateVendor objCreateVendor)
        {
            try
            {
                if (objCreateVendor.id != 0)
                {
                    objCreateVendor.modified_by = objCreateVendor.user_id;
                    objCreateVendor.modified_on = DateTimeHelper.Now;
                    return repo.Update(objCreateVendor);

                }

                else
                {
                    objCreateVendor.created_by = objCreateVendor.user_id;
                    objCreateVendor.modified_by = objCreateVendor.user_id;
                    objCreateVendor.modified_on = DateTimeHelper.Now;
                    return repo.Insert(objCreateVendor);
                }

            }

            catch { throw; }

        }


        public List<ViewVendorList> GetVendorDetailsList(ViewVendorDetailsList model)
        {
            try
            {

                var res = repo.ExecuteProcedure<ViewVendorList>("fn_get_vendordetails", new
                {
                    searchby = Convert.ToString(model.viewVendorDetail.searchBy),
                    searchbyText = Convert.ToString(model.viewVendorDetail.searchText),
                    P_PAGENO = model.viewVendorDetail.currentPage,
                    P_PAGERECORD = model.viewVendorDetail.pageSize,
                    P_SORTCOLNAME = model.viewVendorDetail.sort,
                    P_SORTTYPE = model.viewVendorDetail.orderBy,
                    P_TOTALRECORDS = model.viewVendorDetail.totalRecord,
                    P_RECORDLIST = 0
                }, true);

                return res;
            }
            catch { throw; }
        }


        public CreateVendor GetVendorDetailsByID(int id)
        {
            return repo.Get(m => m.id == id);
        }



        public int DeleteVendorDetailsById(int id)
        {
            try
            {
                var objSystmId = repo.Get(x => x.id == id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }

        public int ChkVendorEmailExist(string email_id, int vendor_id)
        {
            try
            {
                var res = repo.Get(u => u.email_id.Trim().ToLower() == email_id.Trim().ToLower() && u.id != vendor_id);

                return res != null ? 1 : 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<CreateVendor> GetAllVendorsData()
        {
            try
            {
                return (List<CreateVendor>)repo.GetAll(m => m.is_active == true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
