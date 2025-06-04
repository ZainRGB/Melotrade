using Melotrade.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Melotrade.Areas.Admin.Pages
{
    //[Authorize(Roles = "Admin")]
    public class EditUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EditUserModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string Id { get; set; }

            [Required]
            public string? Fname { get; set; }

            [Required]
            public string? Sname { get; set; }

            [Required]
            [EmailAddress]
            public string? Email { get; set; }

            [Display(Name = "Access Control")]
            public int AccessControl { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = user.Id,
                Fname = user.fname,
                Sname = user.sname,
                Email = user.Email,
                AccessControl = user.AccessControl
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null) return NotFound();

            user.fname = Input.Fname;
            user.sname = Input.Sname;
            user.Email = Input.Email;
            user.UserName = Input.Email; // Optional: sync username
            user.AccessControl = Input.AccessControl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            return RedirectToPage("./Manage");
        }
    }
}
