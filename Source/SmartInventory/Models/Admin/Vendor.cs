using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace Models.Admin
{
    public class CreateVendor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string type { get; set; }
        [Required]
        public string address { get; set; }

        [Required]
        [RegularExpression("^[0-9]{10,10}$",ErrorMessage="Contact Number should have 10 digit!")]
        public string contact { get; set; }
        
        [Required]
        //[RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", ErrorMessage = "Invalid Email Address.")]
        //[Remote("IsEmailExists", "Vendor", HttpMethod="POST", ErrorMessage = "Email Id already exists")]  
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$", ErrorMessage = "Invalid Email Address!")]
        public string email_id { get; set; }
        //[Required]
        public string remarks { get; set; }
        
        public int? created_by { get; set; }
       // public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        [NotMapped]
        public int user_id { get; set; }


        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public Boolean is_active { get; set; }
        public CreateVendor()
        {
            pageMsg = new PageMessage();
            is_active = true;
        }


      


    }

    //[Serializable]
    public class ViewVendorDetail
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
   

    }


    public class ViewVendorDetailsList : TemplateForDropDownVendor
    {
        public ViewVendorDetailsList()
        {
            viewVendorDetail = new ViewVendorDetail();

            viewVendorDetail.searchText = string.Empty;
        }
        public IList<ViewVendorList> VendorDetailList { get; set; }
        public ViewVendorDetail viewVendorDetail { get; set; }
    }


    public class ViewVendorList
    {
        public int id { get; set; }

        public string name { get; set; }
        public string type { get; set; }

        public string address { get; set; }

        public string contact { get; set; }

        public string email_id { get; set; }

        public string remarks { get; set; }

        public int totalRecords { get; set; }



        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string created_by_text { get; set; }
        public string modified_by_text { get; set; }

        public Boolean is_active { get; set; }

    }

    public class TemplateForDropDownVendor
    { 
        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchBy { get; set; }

    }


}
