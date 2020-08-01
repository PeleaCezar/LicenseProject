using System.Threading.Tasks;
using LicenseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LicenseProject.Areas.Identity.Pages.Account
{
    public class GoToProfileModel : PageModel
    {
        private readonly UserManager<MyUser> _userManager;


        public GoToProfileModel(UserManager<MyUser> userManager)
        {
            _userManager = userManager;

        }
        public string Email { get; set; }



        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            await _userManager.FindByEmailAsync(email);
             return RedirectToPage("CreateProfile", new { email});
            //return RedirectToAction("Users", "CreateProfile", new { email });

        }
        
    }
}

