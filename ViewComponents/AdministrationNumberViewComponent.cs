using LicenseProject.Data;
using LicenseProject.Models;
using LicenseProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LicenseProject.ViewComponents
{
    public class AdministrationNumberViewComponent : ViewComponent
    {
        public readonly ApplicationDbContext _context;
        public readonly UserManager<MyUser> _userManager;
        public AdministrationNumberViewModel model = new AdministrationNumberViewModel();
        public AdministrationNumberViewComponent(ApplicationDbContext context,UserManager<MyUser>userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var applicationDbContext = await _userManager.Users.Include(e => e.Project).ToListAsync();
             foreach (var user in applicationDbContext)
            if (user.EmailConfirmed == false)
            {
                model.NoOfUsers += user.NoOfUsers;
            }
            return View(model);
        }
    }
}
