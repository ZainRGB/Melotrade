using Microsoft.AspNetCore.Identity;

namespace Melotrade.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Optional: Add fields like Department, EmployeeNumber, etc.
        public int AccessControl { get; set; }
        //public int AccessControl { get; set; } = 0;

        public string fname { get; set; } = string.Empty;
        public string sname { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string mobile { get; set; } = string.Empty;
       

    }
}
