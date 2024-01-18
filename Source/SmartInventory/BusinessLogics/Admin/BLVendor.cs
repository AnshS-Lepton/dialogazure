using DataAccess.Admin;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
   public class BLVendor 
    {
        public CreateVendor SaveEntityVendorDetails(CreateVendor objCreateVendor)
        {
            return new DAVendor().SaveEntityVendorDetails(objCreateVendor);
        
        }

        
        public IList<ViewVendorList> GetVendorDetailsList(ViewVendorDetailsList model)
        {
            return new DAVendor().GetVendorDetailsList(model);
        }


        public CreateVendor GetVendorDetailsByID(int id)
        {
            return new DAVendor().GetVendorDetailsByID(id);
            
        }


        public int DeleteVendorById(int id)
        {
            return new DAVendor().DeleteVendorDetailsById(id);

        }


        public int ChkVendorEmailExist(string email_id, int vendor_id)
        {
            return new DAVendor().ChkVendorEmailExist(email_id, vendor_id);
            
        }

        public List<CreateVendor> GetAllVendorsData()
        {
            return new DAVendor().GetAllVendorsData();

        }

    }
}
