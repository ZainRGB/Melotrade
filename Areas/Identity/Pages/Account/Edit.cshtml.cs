using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Melotrade.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


[Authorize]
public class EditModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public EditModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string? Fname { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string? Sname { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Access Level")]
        public int AccessControl { get; set; } // 0 = No, 1 = Yes
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        //var user = await _userManager.GetUserAsync(User);
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

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
        if (user == null)
        {
            return NotFound();
        }

        //var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.fname = Input.Fname;
        user.sname = Input.Sname;
        user.Email = Input.Email;
        user.UserName = Input.Email;
        user.AccessControl = Input.AccessControl;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["StatusMessage"] = "Your profile has been updated";
        return RedirectToPage("/Index");
    }
}
