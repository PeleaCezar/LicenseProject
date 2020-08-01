using LicenseProject.Data;
using LicenseProject.Models;
using LicenseProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LicenseProject.ViewComponents
{
    public class GrantApplicationNumberViewComponent : ViewComponent
    {
        public readonly ApplicationDbContext _context;
        public readonly UserManager<MyUser> _userManager;
        public GrantApplicationNumberViewModel model = new GrantApplicationNumberViewModel();
        public GrantApplicationNumberViewComponent(ApplicationDbContext context, UserManager<MyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var applicationDbContext = await _context.GrantApplications.Include(e => e.Project).ToListAsync();
            foreach (var grantApp in applicationDbContext)
                if (grantApp.isConfirmed == false)
                {
                    model.NoOfGrantApp += grantApp.NoOfGrantApp;
                }
            return View(model);
        }
    }
}

