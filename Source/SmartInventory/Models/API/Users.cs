using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.API
{

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }

        [Required(ErrorMessage = "User name should not be blank.")]
        [MaxLength(50, ErrorMessage = "User name cannot be longer than 20 characters.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "<br/>No white space allowed")]
        public string user_name { get; set; }

        [Required(ErrorMessage = "Password should not be blank.")]
        [MaxLength(20, ErrorMessage = "Password cannot be longer than 20 characters.")]
        public string password { get; set; }

        public string user_email { get; set; }

        public bool? isactive { get; set; }

        public Nullable<DateTime> created_date { get; set; }
    }
    public class UserApiLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SNo { get; set; }
        public int user_id { get; set; }
        public Nullable<DateTime> login_date { get; set; }
        public Nullable<DateTime> logout_date { get; set; }
        public string client_host { get; set; }
        public string session_id { get; set; }
        public string client_host_address { get; set; }
        public string application { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
    }
    public class Token
    {
        public string refresh_token { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
    }
}
