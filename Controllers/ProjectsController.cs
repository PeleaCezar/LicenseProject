using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LicenseProject.Data;
using LicenseProject.Models;
using LicenseProject.ViewModels;
using LicenseProject.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace LicenseProject.Controllers
{

    [Authorize(Roles = "Administrator, Angajat,Finantator")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<MyUser> _userManager;
        private readonly IToastNotification _toastNotification;
        public ProjectsController(ApplicationDbContext context, UserManager<MyUser> _userManager,
             IToastNotification toastNotification)
        {
            _context = context;
            this._userManager = _userManager;
            _toastNotification = toastNotification;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId) as MyUser;
            var role = await _userManager.GetRolesAsync(user);

            if (role[0] == "Angajat")
            {
                var project = await _context.Projects.Where(p => p.ID == user.ProjectID).ToListAsync();
                var projectsModel = GetProjectModel(project);
                return View("Index", projectsModel);
            }
            else
            {
                var projects = await _context.Projects.ToListAsync();
                var projectsModel = GetProjectModel(projects);
                return View("Index", projectsModel);
            }
            //var projects = await _context.Projects.ToListAsync();
            //var projectsModel = GetProjectModel(projects);
            //    return View("Index", projectsModel);

        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = new ProjectBudgetViewModel();
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ID == id);
            var budget = await _context.Budgets.FirstOrDefaultAsync(c => c.Id == project.BudgetId);
            model.ID = project.ID;
            model.Title = project.Title;
            model.Description = project.Description;
            var date = project.CreatedAt.ToString("dd/MM/yyyy");
            model.DateCreate = date; ;
            model.Status = project.Status;
            model.BudgetId = budget.Id;
            model.EstimatedBudget = budget.EstimatedBudget;
            model.AmountSpent = budget.AmountSpent;
            model.ProjectDuration = budget.ProjectDuration;
            model.Currency = budget.Currency;
            model.Faculty = budget.Faculty;
            model.ProjectDirector = budget.ProjectDirector;
            model.ExternalFinance = (budget.ExternalFinance).ToString("G29");
            model.NoOfHours = (budget.NoOfHours).ToString("G29");
            model.Employees = await GetEmployeeList(id);


            var listofGrantApp = new StringBuilder();
            var grantApplications = await _context.GrantApplications.Where(u => u.ProjectID == id).ToListAsync();
            foreach (var grantApp in grantApplications)
            {
                listofGrantApp.Append(grantApp.Entity);
                listofGrantApp.Append(Environment.NewLine);

            }
            ViewBag.GrantApplications = listofGrantApp;


            if (budget == null)
            {
                return NotFound();
            }

            return View(model);

        }
        [Authorize(Roles = "Administrator")]
        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            var teamMembers = new List<string>();
            var projectDirectors = new List<string>();
            await FillTeamMembersAndDirectors(teamMembers, projectDirectors);

            ViewData["ProjectDirector"] = projectDirectors;
            ViewData["TeamMembers"] = teamMembers;

            return View();
        }

        private async Task<object> FillTeamMembersAndDirectors(List<string> teamMembers, List<string> projectDirectors)
        {

            foreach (MyUser user in await _userManager.Users.ToListAsync())
            {
                var userRole = await _userManager.GetRolesAsync(user);

                if (userRole[0] == "Angajat")
                {
                    var projectDirector = user.FullName;
                    projectDirectors.Add(projectDirector);

                    var teamMember = $"{user.FullName} - {user.Email}";
                    teamMembers.Add(teamMember);


                }
            }
            return null;
        }

        [Authorize(Roles = "Administrator")]
        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectBudgetViewModel projectModel)
        {
            if (ModelState.IsValid)
            {

                if (projectModel.AmountSpent > projectModel.EstimatedBudget)
                {
                    ModelState.AddModelError("AmountSpent", "Cheltuielile nu pot fi mai mari decat bugetul");
                    var members = new List<string>();
                    var directors = new List<string>();
                    await FillTeamMembersAndDirectors(members, directors);

                    ViewData["ProjectDirector"] = directors;
                    ViewData["TeamMembers"] = members;
                    return View(projectModel);
                }
                var budget = new Budget();
                budget.Currency = "RON";
                budget.AmountSpent = projectModel.AmountSpent;
                budget.EstimatedBudget = projectModel.EstimatedBudget;
                budget.ProjectDuration = projectModel.ProjectDuration;
                budget.ProjectDirector = projectModel.ProjectDirector;
                budget.Faculty = projectModel.Faculty;
                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync();
                var budgetId = BudgetMapper.GetBudgetId(budget);
                var project = new Project();
                project.Status = "In desfasurare";
                project.Title = projectModel.Title;
                project.Description = projectModel.Description;
                project.BudgetId = budgetId;
                project.CreatedAt = projectModel.CreatedAt;
                _context.Add(project);
                await _context.SaveChangesAsync();
                if (projectModel.TeamMembers.Count > 0)
                {
                    foreach (var member in projectModel.TeamMembers)
                    {
                        string[] date = member.Split("-");
                        var email = date[1].Trim();
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            user.ProjectID = project.ID;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var teamMembers = new List<string>();
            var projectDirectors = new List<string>();
            foreach (MyUser user in await _userManager.Users.ToListAsync())
            {
                var userRole = await _userManager.GetRolesAsync(user);

                if (userRole[0] == "Angajat")
                {
                    var projectDirector = user.FullName;
                    projectDirectors.Add(projectDirector);

                    var teamMember = $"{user.FullName} - {user.Email}";
                    teamMembers.Add(teamMember);


                }
            }

            ViewData["ProjectDirector"] = projectDirectors;
            ViewData["TeamMembers"] = teamMembers;
            return View(projectModel);
        }
        [Authorize(Roles = "Administrator")]
        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var project = await _context.Projects.FindAsync(id);
            var projectModel = new ProjectBudgetViewModel();
            projectModel.ID = project.ID;
            projectModel.Title = project.Title;
            projectModel.Description = project.Description;
            projectModel.Status = project.Status;
            var budget = await _context.Budgets.FirstOrDefaultAsync(p => p.Id == project.BudgetId);
            projectModel.BudgetId = budget.Id;
            projectModel.EstimatedBudget = budget.EstimatedBudget;
            projectModel.AmountSpent = budget.AmountSpent;
            projectModel.ProjectDuration = budget.ProjectDuration;
            projectModel.Currency = budget.Currency;
            projectModel.Faculty = budget.Faculty;
            projectModel.ProjectDirector = budget.ProjectDirector;



            var usersInProject = new List<string>();
            foreach (MyUser user in await _userManager.Users.Include(p => p.Project).Where(u => u.ProjectID == id).ToListAsync())
            {
                var userInProject = $"{user.FullName}- {user.Email}";
                usersInProject.Add(userInProject);
            }
            projectModel.TeamMembers = usersInProject;




            var teamMembers = new List<string>();
            var projectDirectors = new List<string>();
            foreach (MyUser user in await _userManager.Users.ToListAsync())
            {
                var userRole = await _userManager.GetRolesAsync(user);

                if (userRole[0] == "Angajat")
                {
                    var projectDirector = user.FullName;
                    projectDirectors.Add(projectDirector);

                    var teamMember = $"{user.FullName}- {user.Email}";
                    teamMembers.Add(teamMember);
                }
            }
            ViewData["ProjectDirector"] = projectDirectors;
            ViewData["TeamMembers"] = teamMembers;

            if (project == null)
            {
                return NotFound();
            }
            return View(projectModel);
        }
        [Authorize(Roles = "Administrator")]
        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectBudgetViewModel projectModel)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(e => e.ID == id);
            project.ID = projectModel.ID;
            project.Title = projectModel.Title;
            project.Description = projectModel.Description;
            project.Status = projectModel.Status;
            var budget = await _context.Budgets.FirstOrDefaultAsync(e => e.Id == project.BudgetId);
            budget.Id = project.BudgetId;
            projectModel.BudgetId = budget.Id;
            budget.EstimatedBudget = projectModel.EstimatedBudget;
            budget.AmountSpent = projectModel.AmountSpent;
            budget.ProjectDuration = projectModel.ProjectDuration;
            budget.Currency = "RON";
            budget.Faculty = projectModel.Faculty;
            budget.ProjectDirector = projectModel.ProjectDirector;

            var usersInProject = await _userManager.Users.Include(p => p.Project).Where(u => u.ProjectID == id).ToListAsync() as List<MyUser>;
            foreach (MyUser user in usersInProject)
            {
                user.ProjectID = null;
                await _context.SaveChangesAsync();
            }

            if (projectModel.TeamMembers.Count > 0)
            {
                foreach (var member in projectModel.TeamMembers)
                {
                    string[] date = member.Split("-");
                    var email = date[1].Trim();
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        user.ProjectID = project.ID;
                    }
                }
            }

            if (id != project.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var project = await _context.Projects
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(project);
        //}
        [Authorize(Roles = "Administrator")]
        // POST: Projects/Delete/5
        public async Task<IActionResult> Delete(int id)
        {

            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ID == id);
            if (project == null)
            {
                return RedirectToAction("Index");
            }
            var users = await _userManager.Users.Include(e => e.Project).Where(u => u.ProjectID == id).ToListAsync() as List<MyUser>;
            foreach (MyUser user in users)
            {
                await _userManager.DeleteAsync(user);
            }
            var grantApps = await _context.GrantApplications.Include(e => e.Project).Where(u => u.ProjectID == id).ToListAsync();
            foreach (GrantApplication grantApplication in grantApps)
            {
                _context.Remove(grantApplication);
            }
            var tasks = await _context.Tasks.Include(e => e.Project).Where(u => u.ProjectID == id).ToListAsync();
            foreach (Models.Task task in tasks)
            {
                _context.Remove(task);
            }
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.Id == project.BudgetId);
            _context.Budgets.Remove(budget);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successfully" });


        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ID == id);
        }

        public List<ProjectViewModel> GetProjectModel(List<Project> projects)
        {
            var projectsModel = new List<ProjectViewModel>();
            foreach (var project in projects)
            {
                var projectModel = new ProjectViewModel();
                projectModel.ID = project.ID;
                projectModel.Status = project.Status;
                projectModel.Title = project.Title;
                projectModel.Description = project.Description;
                projectModel.NoOfMembers = _userManager.Users.Where(u => u.ProjectID == project.ID).Count();
                var date = project.CreatedAt.ToString("dd/MM/yyyy");
                projectModel.CreateDate = date;
                projectsModel.Add(projectModel);
            }
            return projectsModel;
        }

        public async Task<List<EmployeeViewModel>> GetEmployeeList(int? id)
        {
            var listOfEmployees = new List<EmployeeViewModel>();
            var employeeUsers = await _userManager.Users.Where(u => u.ProjectID == id).ToListAsync();
            foreach (var employee in employeeUsers)
            {
                var userRole = await _userManager.GetRolesAsync(employee);
                if (userRole.Count > 0)
                {
                    if (userRole[0] == "Angajat")
                    {
                        var employeeModel = new EmployeeViewModel();
                        employeeModel.Name = employee.FullName;
                        employeeModel.Username = employee.UserName;
                        employeeModel.PhoneNumber = employee.PhoneNumber;
                        listOfEmployees.Add(employeeModel);
                    }
                }
            }
            return listOfEmployees;
        }

        public async Task<IActionResult> GiveNameProject(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == id);

            var projectModel = new ProjectViewModel();

            projectModel.Title = project.Title;

            return Json(new { success = true, data = projectModel, message = "Numele proiectului a fost trimis in modal." });
        }

        //public async Task<IActionResult> Search (string item)
        //{
        //    var projects = from m in _context.Projects select m;

        //     if(!string.IsNullOrEmpty(item))
        //    {
        //        projects = projects.Where(s => s.Title.Contains(item));
        //    }
        //   // return Json(new { success = true, data = projects, message = "Proiectele cu informatiile cerute au fost trimise" });
        //    return View(await projects.ToListAsync());


        //}


    }
}
