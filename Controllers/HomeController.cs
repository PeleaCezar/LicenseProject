using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LicenseProject.Models;
using LicenseProject.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using LicenseProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LicenseProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private UserManager<MyUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<MyUser> _userManager)
        {
            _context = context;
            _logger = logger;
            this._userManager = _userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectsWithSum()
        {
            var listOfProjects = new List<ProjectsWithBothSum>();
            await PopulateList(listOfProjects);
            return Json(new { success = true, data = listOfProjects });
        }

        public async System.Threading.Tasks.Task PopulateList(List<ProjectsWithBothSum> listOfProjects)
        {
            foreach (var project in await _context.Projects.Include(p => p.MyUsers).ToListAsync())
            {
                var projectWithSum = new ProjectsWithBothSum();
                projectWithSum.ProjectName = project.Title;
                var budget = await _context.Budgets.FirstOrDefaultAsync(p => p.Id == project.BudgetId);
                projectWithSum.InitialAmount = budget.EstimatedBudget;
                var totalAmount = await _context.GrantApplications.Where(g => g.ProjectID == project.ID).Where(n => n.isConfirmed == true).SumAsync(g => g.AmountOfFunding);
                projectWithSum.Amount = totalAmount;
                listOfProjects.Add(projectWithSum);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectsWithAmountSpent()
        {
            var listOfProjects = new List<ProjectWithAmountSpent>();
            await PopulateListWithProjects(listOfProjects);
            return Json(new { success = true, data = listOfProjects });
        }
        public async System.Threading.Tasks.Task PopulateListWithProjects(List<ProjectWithAmountSpent> listOfProjects)
        {
            foreach (var project in await _context.Projects.Include(p => p.MyUsers).ToListAsync())
            {
                var projectWithAmountSpent = new ProjectWithAmountSpent();
                projectWithAmountSpent.ProjectName = project.Title;
                var budget = await _context.Budgets.FirstOrDefaultAsync(p => p.Id == project.BudgetId);
                projectWithAmountSpent.AmountSpent = budget.AmountSpent;
                listOfProjects.Add(projectWithAmountSpent);
            }
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //[AcceptVerbs("Get", "Post")]
        //[AllowAnonymous]
        //public async Task<IActionResult> IsEmailInUse(string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user == null)
        //    {
        //        return Json(true);
        //    }
        //    else
        //    {
        //        return Json($"Email {email} is already in use.");
        //    }
        //}
    }
}
