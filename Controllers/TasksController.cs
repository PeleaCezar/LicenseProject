using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicenseProject.Data;
using LicenseProject.Models;
using LicenseProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NToastNotify;

namespace LicenseProject.Controllers
{
    [Authorize(Roles = "Administrator, Angajat")]
    public class TasksController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IToastNotification _toastNotification;
        private readonly SignInManager<MyUser> _signInManager;

        public TasksController(ApplicationDbContext context,
                                UserManager<MyUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                SignInManager<MyUser> signInManager,
                                IToastNotification toastNotification)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _toastNotification = toastNotification;
        }

        // GET: 
        public async Task<IActionResult> CreateTask(int? id)
        {
            var project = await _context.Projects.FindAsync(id);
            var model = new UserTaskViewModel();
            //model.StartDate = DateTime.UtcNow;
            return View();
        }

        //VIEWBACKLOG
        public async Task<IActionResult> ViewBacklog(int? id)
        {
            var applicationDbContext = await _context.Tasks.Include(e => e.Project).Where(p => p.ProjectID == id).ToListAsync();
            List<UserTaskViewModel> model = new List<UserTaskViewModel>();
            foreach (var task in applicationDbContext)
            {
                var taskToModel = new UserTaskViewModel();
                taskToModel.TaskName = task.Name;
                taskToModel.Priority = task.Priority;
                taskToModel.FullName = task.UserID;
                taskToModel.StartDate = task.StartDate;
                taskToModel.Description = task.Description;
                taskToModel.TaskID = task.ID;
                taskToModel.Status = task.Status;
                taskToModel.Duration = task.Duration;
                ViewBag.NoOfTasksToDo = _context.Tasks.Where(u => u.Status =="De facut").Where(p => p.ProjectID == id).Count();
                ViewBag.NoOfTasksProgress = _context.Tasks.Where(u => u.Status == "In progres").Where(p => p.ProjectID == id).Count();
                ViewBag.NoOfTasksTesting = _context.Tasks.Where(u => u.Status == "In testare").Where(p=>p.ProjectID == id).Count();
                ViewBag.NoOfTasksDone = _context.Tasks.Where(u => u.Status == "Terminat").Where(p => p.ProjectID == id).Count();
                model.Add(taskToModel);
            }

            var teamMembers = new List<string>();
            foreach (MyUser user in await _userManager.Users.Include(e => e.Project).Where(p => p.ProjectID == id).ToListAsync())
            {
                var userRole = await _userManager.GetRolesAsync(user);

                if(userRole[0] == "Angajat")
                {
                    var teamMember =user.FullName;
                    teamMembers.Add(teamMember);
                }
            }
            ViewData["TeamMembers"] = teamMembers;
            
            ViewBag.Project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == id);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask([Bind("ID,Name,Description,StartDate,EndDate" +
            ",Duration,Status,ProjectID")] int id, UserTaskViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                //var user = await _userManager.FindByIdAsync(userId) as MyUser;

                var project = await _context.Projects.FindAsync(id);

                var task = new Models.Task();

                task.ProjectID = project.ID;
                task.Name = model.TaskName;
                task.Description = model.Description;
                task.StartDate = model.StartDate;
              //  task.EndDate = model.EndDate;
                task.Status = "De facut";
                _context.Add(task);
                await _context.SaveChangesAsync();

               /* var userTask = new UserTask();
                userTask.MyUserId = userId;
                userTask.TaskID = task.ID;
                _context.UserTasks.Add(userTask);
                await _context.SaveChangesAsync();*/


                return RedirectToAction("Index", "Projects");
            }

            return View();
        }


        //CREARE
        [AcceptVerbs("Post")]
        public async Task<IActionResult> CreateBacklogTask(string jsonResult)
        {
            var myCleanJsonObject = JObject.Parse(jsonResult).ToString();

           

            var backlogTask = JsonConvert.DeserializeObject<BacklogItemViewModel>(myCleanJsonObject);


            var task = new Models.Task();
            task.Status = "De facut";
            task.ProjectID = int.Parse(backlogTask.ProjectId);
            task.Name = backlogTask.Title;
            task.Description = backlogTask.Description;
            task.Duration = Convert.ToDecimal(backlogTask.NumberOfHours);
            task.Priority = backlogTask.Priority;
            task.UserID = backlogTask.Employee;
            task.StartDate = DateTime.Now;
            _context.Add(task);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Task-ul a fost creat cu succes" });

        }
        //STERGERE
        public async Task<IActionResult> Delete (int id)
        { 
            var task = await _context.Tasks.FirstOrDefaultAsync(m => m.ID == id);
            if (task == null)
            {
                return RedirectToAction("ViewBacklog");
            }
            _context.Remove(task);
            await _context.SaveChangesAsync();
            
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ID == task.ProjectID);
            var budget = await _context.Budgets.FirstOrDefaultAsync(m => m.Id == project.BudgetId);

            if (budget.NoOfHours != 0)
            {
                budget.NoOfHours -= task.Duration;
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Stergere efectuata cu succes !" });
        }

        //STATUS
        [AcceptVerbs("Post")]
        public async Task<IActionResult> UpdateStatus (string statusResult)
        {
            var myCleanJsonObject = JObject.Parse(statusResult).ToString();
            var statusTask = JsonConvert.DeserializeObject<BacklogItemViewModel>(myCleanJsonObject);
            var task = await _context.Tasks.FirstOrDefaultAsync(m => m.ID == statusTask.TaskID);
            if( task != null)
            {
               task.Status = statusTask.Status;
            }
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ID == task.ProjectID);
            var budget = await _context.Budgets.FirstOrDefaultAsync(m => m.Id == project.BudgetId);

            if(task.Status == "Terminat")
            { 
                if(statusTask.NumberOfHours != null)
                {
                    if (task.Duration != Convert.ToDecimal(statusTask.NumberOfHours))
                    {
                        task.Duration = Convert.ToDecimal(statusTask.NumberOfHours);
                    }

                }

                budget.NoOfHours += task.Duration;
            }
            _context.Update(task);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Statusul a fost actualizat" });
        }

        //GET
        //EDIT
        [HttpGet]
        public async Task<IActionResult>EditTask(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(m => m.ID == id);
            var taskModel = new BacklogItemViewModel();
            taskModel.Title = task.Name;
            taskModel.Priority = task.Priority;
            taskModel.Description = task.Description;
            taskModel.NumberOfHours = (task.Duration).ToString("G29");
            taskModel.Employee = task.UserID;
            taskModel.TaskID = task.ID;
                
            return Json(new { success = true, data = taskModel,  message = "Datele au fost trimise catre client." });
        }
        //EDIT
        [AcceptVerbs("Post")]
        public async Task<IActionResult>EditTask(string jsonResult)
        {
            var myCleanJsonObject = JObject.Parse(jsonResult).ToString();
            var backlogTask = JsonConvert.DeserializeObject<BacklogItemViewModel>(myCleanJsonObject);
           // task.ID = int.Parse(backlogTask.TaskID);
            var task = await _context.Tasks.FirstOrDefaultAsync(p => p.ID == backlogTask.TaskID);
            task.Name = backlogTask.Title;
            task.Description = backlogTask.Description;
            task.Duration = Convert.ToDecimal(backlogTask.NumberOfHours);
            task.Priority = backlogTask.Priority;
            task.UserID = backlogTask.Employee;
            _context.Update(task);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Datele au fost actuallizate" });

        }


        [HttpGet]
        public async Task<IActionResult> GetTaskStatus(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.ID == id);
            return Json(new { success = true,status = task.Status });

        }
        //GET
        //DETALII
        [HttpGet]
        public async Task<IActionResult>DetailsTask(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(m => m.ID == id);
            var taskModel = new BacklogItemViewModel();
            taskModel.Priority = task.Priority;
            taskModel.Description = task.Description;
            taskModel.NumberOfHours = (task.Duration).ToString("G29");
            //taskModel.StartDate = Convert.ToString(task.StartDate);
            var date=task.StartDate.ToString("dd.MM.yyyy la ora HH:mm ");
            taskModel.StartDate = date;
            taskModel.Status = task.Status;
            taskModel.Employee = task.UserID;
            taskModel.TaskID = task.ID;
            taskModel.Name = task.Name;
            return Json(new { success = true, data = taskModel, message = "Datele au fost trimise catre client." });
        }

        public async Task<IActionResult> GiveNameTask(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(p => p.ID == id);
            var taskModel = new BacklogItemViewModel();
            taskModel.Title = task.Name;
            return Json(new { success = true, data = taskModel, message = "Numele task-ului a fost trimis in modal." });
        }
        public async Task<IActionResult> GiveStatus(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(p => p.ID == id);
            var taskModel = new BacklogItemViewModel();
            taskModel.Status = task.Status;
            return Json(new { success = true, data = taskModel, message = "Statusul a fost trimis." });

        }

    }
}
