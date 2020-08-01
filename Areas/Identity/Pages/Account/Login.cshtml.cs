using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using LicenseProject.Data;
using LicenseProject.Models;

namespace LicenseProject.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly SignInManager<MyUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public LoginModel(SignInManager<MyUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<MyUser> userManager,
            IWebHostEnvironment webHostEnvironment,
             ApplicationDbContext context
            )
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async System.Threading.Tasks.Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                var user = await _userManager.FindByEmailAsync(Input.Email);

                var passwordVerify = _signInManager.CheckPasswordSignInAsync(user,Input.Password,false);

                if ( result.Succeeded && user.Function != null && user.Gender != null && user.Address != null && user.Gender != null && user.PhotoPath != null)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Numele de utilizator nu există");
                    return Page();

                }
                if(user.EmailConfirmed == false)
                {
                    return RedirectToPage("OnHold", new { email = Input.Email });
                }

                if (passwordVerify.Result.Succeeded &&  user.Function == null && user.Gender == null  && user.Address == null && user.Gender == null && user.PhotoPath == null)
                {
                    return RedirectToPage( "GoToProfile", new { email = Input.Email });
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nume sau parolă greșită");
                    return Page();
                }
            }
            //var user = await _userManager.FindByIdAsync(Id);
            // return RedirectToAction( "Users", "CreateProfile", new { id = user.Id});
            // var user = await _userManager.FindByIdAsync(userId) as MyUser;
            // If we got this far, something failed, redisplay form
            return Page();
        }

    }
}
