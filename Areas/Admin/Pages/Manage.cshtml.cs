using Melotrade.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Melotrade.Areas.Admin.Pages
{
    //[Authorize(Roles = "Admin")]
    public class ManageModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<ApplicationUser> Users { get; set; } = new();

        public void OnGet()
        {
            Users = _userManager.Users.ToList();
        }
    }
}
