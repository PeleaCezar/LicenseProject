using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using LicenseProject.Data;
using LicenseProject.Models;
using LicenseProject.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace LicenseProject.Areas.Identity.Pages.Account
{
    public class CreateProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CreateProfileModel (ApplicationDbContext context,
                                   IWebHostEnvironment webHostEnvironment,
                                   UserManager<MyUser> userManager,
                                   SignInManager<MyUser> signInManager)

        {
            this.webHostEnvironment = webHostEnvironment;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

        }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public UserProjectViewModel model { get; set; } 

            [Required(ErrorMessage="Trebuie să vă introduceți numele")]
            [MaxLength(50, ErrorMessage = "Numele nu poate conține mai mult de 50 de caractere")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Trebuie să alegeți un gen")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "Trebuie să vă introduceți funcția")]
            public string Function { get; set; }

            [Required(ErrorMessage = "Trebuie să vă introduceți vărsta")]
            public int Age { get; set; }

            [Required(ErrorMessage = "Trebuie să vă introduceți adresa")]
            public string Address { get; set; }

            [Phone(ErrorMessage = "Numărul de telefon trebuie sa contina 10 cifre")]
            [Required(ErrorMessage = "Trebuie să vă introduceți numărul de telefon")]
            public string PhoneNumber { get; set; }

            //[Required(ErrorMessage = "Please select file.")]
           // [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed.")]
            public string PhotoPath { get; set; }

        }
        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (email == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (Input.model.Photo != null)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    Input.model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                //else
                //{
                //    ModelState.AddModelError("Input.model.Photo","Trebuie sa atasati o poza cu dumneavoastra. ");
                //    return Page();
                //}

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
         
                    return NotFound();
                }
                else
                {
                    user.FullName = Input.FullName;
                    user.Function = Input.Function;
                    user.Gender = Input.Gender;
                    user.Age = Input.Age;
                    user.Address = Input.Address;
                    var telephone = Input.PhoneNumber.Replace("-", "");
                    telephone = telephone.Replace(")", "");
                    telephone = telephone.Replace("(", "");
                    telephone = telephone.Replace(" ", "");
                    user.PhoneNumber = telephone;
                    user.PhotoPath = uniqueFileName;
                }
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return Page();
        }
    }
}
