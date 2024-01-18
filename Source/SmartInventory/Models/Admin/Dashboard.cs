using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Admin
{

    public class CurrentActiveUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int srno { get; set; }
        public int total_user { get; set; }
        public int total_curent_login { get; set; }
    }

    public class LastMonthActiveUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int srno { get; set; }
        public int users { get; set; }
        public DateTime login_date { get; set; }
    }

    public class Dashboard
    {
        public CurrentActiveUsers CurrentActiveUsers { get; set; }
        public List<LastMonthActiveUsers> LastMonthActiveUsers { get; set; }

    }
}
