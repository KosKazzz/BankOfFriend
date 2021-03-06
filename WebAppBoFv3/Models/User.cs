using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppBoFv3.Models
{
    public class User

    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }
        public  List<Company> UserCompanies { get; set; }
        public User() 
        {
            UserCompanies = new List<Company>();
        }
    }
}
