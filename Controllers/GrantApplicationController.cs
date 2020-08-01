using LicenseProject.Data;
using LicenseProject.Models;
using LicenseProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace LicenseProject.Controllers
{

    public class GrantApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<MyUser> _userManager;
        public GrantApplicationController(ApplicationDbContext context, UserManager<MyUser> _userManager)
        {
            _context = context;
            this._userManager = _userManager;
        }
        [Authorize(Roles = "Finantator")]
        //GET
        public async Task<IActionResult> AddGrantApplication(int? id)
        {
            var model = new ProjectGrantAppViewModel();
            if (id == null)
            {
                return NotFound();
            }
            var project = await _context.Projects.FindAsync(id);
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId) as MyUser;
            model.Representative = user.FullName;
            model.ProjectName = project.Title;
            model.ProjectID = project.ID;
            model.UserID = userId;
            model.Currency = "RON";
            return View(model);
        }
        [Authorize(Roles = "Finantator")]
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGrantApplication([Bind(
            "ProjectID,GrantApplicationID,Entity,Date,AmountOfFunding,Status,Currency,FullName")]  int? id, ProjectGrantAppViewModel model)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var project = await _context.Projects.FindAsync(id);
            if (ModelState.IsValid)
            {

                var grantApp = new GrantApplication();
                grantApp.UserID = userId;
                grantApp.ProjectID = project.ID;
                grantApp.Entity = model.Entity;
                grantApp.AmountOfFunding = model.AmountOfFunding;
                grantApp.Status = "In asteptare";
                grantApp.Currency = "RON";
                grantApp.Date = DateTime.Now;
                grantApp.Representantive = model.Representative;
                _context.Add(grantApp);
                grantApp.NoOfGrantApp++;
                await _context.SaveChangesAsync();
                return RedirectToAction("StatusGrantApplication", new { grantApp.ID });
            }
            return View(model);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> PermissionGrantApplication()
        {
            var applicationDbContext = await _context.GrantApplications.Include(e => e.Project).ToListAsync();
            List<ProjectGrantAppViewModel> model = new List<ProjectGrantAppViewModel>();

            foreach (var grantApp in applicationDbContext)
                if (grantApp.isConfirmed == false)
                {
                    {
                        var grantAppViewModel = new ProjectGrantAppViewModel();

                        grantAppViewModel.GrantApplicationID = grantApp.ID;
                        grantAppViewModel.Entity = grantApp.Entity;
                        grantAppViewModel.Date = grantApp.Date;
                        grantAppViewModel.AmountOfFunding = grantApp.AmountOfFunding;
                        grantAppViewModel.Status = grantApp.Status;
                        grantAppViewModel.Currency = grantApp.Currency;
                        grantAppViewModel.Representative = grantApp.Representantive;

                        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == grantApp.ProjectID);
                        if (project == null)
                        {
                            grantAppViewModel.ProjectName = ("Aceasta cerere nu are proiect");
                            //return NotFound("Cererea nu a fost gasita");
                        }
                        else
                        {
                            grantAppViewModel.ProjectName = project.Title;

                        }
                        model.Add(grantAppViewModel);
                    }
                }
            return View(model);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> AcceptGrantApplication(int id)
        {
            var grantApp = await _context.GrantApplications.FindAsync(id);
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == grantApp.ProjectID);
            var budget = await _context.Budgets.FirstOrDefaultAsync(c => c.Id == project.BudgetId);
            if (grantApp.isConfirmed == false)
            {
                grantApp.isConfirmed = true;
                if (grantApp.isConfirmed == true)
                {
                    budget.ExternalFinance += grantApp.AmountOfFunding;
                    grantApp.Status = "Aprobat";
                    grantApp.NoOfGrantApp--;
                    await _context.SaveChangesAsync();
                }
            }
            _context.Update(grantApp);
            await _context.SaveChangesAsync();
            return RedirectToAction("PermissionGrantApplication");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> RefusedGrantApplication(int? id)
        {
            var grantApp = await _context.GrantApplications.FirstOrDefaultAsync(m => m.ID == id);

            if (grantApp == null)
            {
                ViewBag.ErrorMessage = $" User with Id={id} cannot be found";
                return NotFound();
            }
            else
            {
                _context.GrantApplications.Remove(grantApp);
                await _context.SaveChangesAsync();
                grantApp.NoOfGrantApp--;
                await _context.SaveChangesAsync();
                return RedirectToAction("PermissionGrantApplication");
            }
        }

        [Authorize(Roles = "Finantator")]
        [HttpGet]
        public async Task<IActionResult> StatusGrantApplication(int? id)
        {
            var grantApp = await _context.GrantApplications.FindAsync(id);
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == grantApp.ProjectID);

            var model = new ProjectGrantAppViewModel();

            if (grantApp == null)
            {
                ViewBag.ErrorMessage = $" User with Id={id} cannot be found";
                return NotFound();
            }
            else
            {
                model.Representative = grantApp.Representantive;
                model.GrantApplicationID = grantApp.ID;
                model.AmountOfFunding = grantApp.AmountOfFunding;
                model.Currency = grantApp.Currency;
                model.Entity = grantApp.Entity;
                model.ProjectName = project.Title;
                model.Status = grantApp.Status;

            }

            return View(model);

        }
        [Authorize(Roles = "Finantator")]
        [HttpGet]
        public async Task<IActionResult> FinancerGrantApplication()
        {
            var applicationDbContext = await _context.GrantApplications.Include(e => e.MyUser).ToListAsync();
            List<ProjectGrantAppViewModel> model = new List<ProjectGrantAppViewModel>();
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId) as MyUser;

            foreach (var grantApp in applicationDbContext)
                if (grantApp.UserID == userId)
                {
                    {
                        var grantAppViewModel = new ProjectGrantAppViewModel();
                        grantAppViewModel.GrantApplicationID = grantApp.ID;
                        grantAppViewModel.Entity = grantApp.Entity;
                        grantAppViewModel.Date = grantApp.Date;
                        grantAppViewModel.AmountOfFunding = grantApp.AmountOfFunding;
                        grantAppViewModel.Status = grantApp.Status;
                        grantAppViewModel.Currency = grantApp.Currency;
                        grantAppViewModel.Representative = grantApp.Representantive;

                        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == grantApp.ProjectID);
                        if (project == null)
                        {
                            grantAppViewModel.ProjectName = ("Aceasta cerere nu are proiect");
                            //return NotFound("Cererea nu a fost gasita");
                        }
                        else
                        {
                            grantAppViewModel.ProjectName = project.Title;

                        }
                        model.Add(grantAppViewModel);
                    }
                }
            return View(model);
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> ListAllGrantApplication()
        {
            var applicationDbContext = await _context.GrantApplications.Include(e => e.MyUser).ToListAsync();
            List<ProjectGrantAppViewModel> model = new List<ProjectGrantAppViewModel>();
            foreach (var grantApplication in applicationDbContext)
                if (grantApplication.isConfirmed == true)
                {
                    {
                        var grantAppinModel = new ProjectGrantAppViewModel();
                        grantAppinModel.GrantApplicationID = grantApplication.ID;
                        grantAppinModel.Entity = grantApplication.Entity;
                        grantAppinModel.Date = grantApplication.Date;
                        grantAppinModel.AmountOfFunding = grantApplication.AmountOfFunding;
                        grantAppinModel.Status = grantApplication.Status;
                        grantAppinModel.Currency = grantApplication.Currency;
                        grantAppinModel.Representative = grantApplication.Representantive;
                        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == grantApplication.ProjectID);
                        if (project == null)
                        {
                            grantAppinModel.ProjectName = "Aceasta cerere nu are proiect";
                        }
                        else
                        {
                            grantAppinModel.ProjectName = project.Title;
                        }
                        model.Add(grantAppinModel);
                    }
                }
            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {

            var grantApplication = await _context.GrantApplications.FirstOrDefaultAsync(m => m.ID == id);
            if (grantApplication == null)
            {
                return RedirectToAction("ListAllGrantApplication");
            }
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ID == grantApplication.ProjectID);
            var budget = await _context.Budgets.FirstOrDefaultAsync(c => c.Id == project.BudgetId);

            if (budget.ExternalFinance != 0)
            {
                budget.ExternalFinance -= grantApplication.AmountOfFunding;
            }
            _context.Remove(grantApplication);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successfully" });
        }

    }
}