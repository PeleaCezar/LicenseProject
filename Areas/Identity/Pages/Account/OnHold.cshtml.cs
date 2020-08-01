using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicenseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LicenseProject.Areas.Identity.Pages.Account
{
    public class OnHoldModel : PageModel
    {
        private readonly UserManager<MyUser> _userManager;


        public OnHoldModel(UserManager<MyUser> userManager)
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
    }
}
