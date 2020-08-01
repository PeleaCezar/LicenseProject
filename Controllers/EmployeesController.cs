using LicenseProject.Data;
using LicenseProject.Models;
using LicenseProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LicenseProject.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EmployeesController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeesController(ApplicationDbContext context,
                                        UserManager<MyUser> userManager,
                                        RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> ListUsers()
        {
            var applicationDbContext = await _userManager.Users.Include(e => e.Project).ToListAsync();
            List<UserProjectViewModel> model = new List<UserProjectViewModel>();
            foreach (var user in applicationDbContext)
                if (user.EmailConfirmed == true)
                {
                    {
                        var userToModel = new UserProjectViewModel();
                        userToModel.Id = user.Id;
                        var theUser = await _userManager.GetRolesAsync(user);
                        if(theUser.Count > 0)
                        {
                            userToModel.Role = theUser[0];

                        }
                        else
                        {
                            userToModel.Role = "Inca nu s-a alocat un rol pentru acest utilizator!";
                        }
                        if (theUser[0] == "Finantator")
                        {
                            var noOfGrantApp = _context.GrantApplications.Where(p => p.UserID == user.Id).Count();
                            //if (noOfGrantApp > 0)
                            //{
                            //    userToModel.ProjectName = $" Acest finantator finanteaza {noOfGrantApp} proiecte";
                            //}
                            //else
                            //{
                            //    userToModel.ProjectName = "Acest finantator nu finanteaza niciun proiect";
                            //}
                            switch (noOfGrantApp)
                            {
                                case 0:
                                    userToModel.ProjectName = "Acest utilizator nu finanteaza niciun proiect";
                                    break;
                                case 1:
                                    userToModel.ProjectName = $" Acest utilizator finanteaza 1 proiect";
                                    break;
                                default:
                                    userToModel.ProjectName = $" Acest utilizator finanteaza {noOfGrantApp} proiecte";
                                    break;

                            }
                        }
                        userToModel.UserName = user.UserName;
                        userToModel.FullName = user.FullName;
                        userToModel.Function = user.Function;
                        userToModel.MyUsers = user;
                        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == user.ProjectID);

                        if (project == null)
                        {

                            if (theUser[0] == "Administrator")
                            {
                                userToModel.ProjectName = "Administratorul nu poate avea proiect alocat";
                            }
                            if (theUser[0] == "Angajat")
                            {
                                userToModel.ProjectName = "Acest angajat nu are un proiect alocat";
                            }

                        }
                        else
                        {
                            userToModel.ProjectName = project.Title;
                        }
                        /*  se sterge proiectul .. se sterg si oamenii din echipa pentru acel proiect.
                         *  
                          if (project == null)
                          {
                              userToModel.ProjectName = "The project has been deleted";
                          }
                          else
                          {
                              userToModel.ProjectName = project.Title;

                          }*/
                        model.Add(userToModel);
                    }
                }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> DetailsUser (string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            List<UserProjectViewModel> model = new List<UserProjectViewModel>();
            var user = await _userManager.FindByIdAsync(Id);
            var userToModel = new UserProjectViewModel();
            var theUser = await _userManager.GetRolesAsync(user);
            if (theUser.Count > 0)
            {
                userToModel.Role = theUser[0];

            }
            else
            {
                userToModel.Role = "Nu are rol alocat!";
            }
            userToModel.Role = theUser[0];
            userToModel.MyUsers = user;
            userToModel.Id = user.Id;
            userToModel.FullName = user.FullName;
            userToModel.UserName = user.UserName;
            userToModel.Function = user.Function;          
            userToModel.PhoneNumber = user.PhoneNumber;
            userToModel.Address = user.Address;
            userToModel.Age = user.Age;
            userToModel.Gender = user.Gender;

            if (theUser[0] == "Finantator")
            {
                var noOfGrantApp = _context.GrantApplications.Where(p => p.UserID == user.Id).Count();

                switch (noOfGrantApp)
                {
                    case 0:
                        userToModel.ProjectName = "Acest utilizator nu finanteaza niciun proiect";
                        break;
                    case 1:
                        userToModel.ProjectName = $" Acest utilizator finanteaza 1 proiect";
                        break;
                    default:
                        userToModel.ProjectName = $" Acest utilizator finanteaza {noOfGrantApp} proiecte";
                        break;

                }
            }
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == user.ProjectID);
            if (project == null)
            {

                if (theUser[0] == "Administrator")
                {
                    userToModel.ProjectName = "Administratorul nu poate avea proiect alocat";
                }
                if (theUser[0] == "Angajat")
                {
                    userToModel.ProjectName = "Acest angajat nu are un proiect alocat";
                }

            }
            else
            {
                userToModel.ProjectName = project.Title;
            }
            model.Add(userToModel);
            return View(model);
        }




        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $" User with Id={id} cannot be found";
                return NotFound();
            }


            var model = new UserProjectViewModel();
            model.Id = user.Id;
            model.UserName = user.UserName;
            model.Email = user.Email;
            model.FullName = user.FullName;
            model.Gender = user.Gender;
            model.Function = user.Function;
            model.Age = user.Age;
            model.Address = user.Address;
            model.PhoneNumber = user.PhoneNumber;
            model.MyUsers = user;
            var financedProjects = new List<ProjectGrantAppViewModel>();
            foreach(var financedProject in await _context.GrantApplications.Where(g => g.UserID == user.Id).ToListAsync())
            {
                var projectFinanced = await _context.Projects.FirstOrDefaultAsync(p => p.ID == financedProject.ProjectID);
                var finaincedProject = new ProjectGrantAppViewModel()
                {
                    ProjectName = projectFinanced.Title,
                    Status = projectFinanced.Status,
                    AmountOfFunding = financedProject.AmountOfFunding,
                    Currency = financedProject.Currency
                };
                financedProjects.Add(finaincedProject);
            }

            var userRole= await _userManager.GetRolesAsync(user);
            if(userRole.Count > 0)
            {
                model.Role = userRole[0];
               // ViewData["Role"] = new SelectList(_roleManager.Roles, "Name", "Name");
                if(userRole[0] == "Finantator")
                {
                    model.IsFinancier = true;
                    model.FinancedProjects = financedProjects;
                }
                if (userRole[0] == "Angajat")
                {
                    model.IsEmployee = true;
                }

            }


            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ID == user.ProjectID);

            if (project == null) 
            {
               
                if (userRole[0]=="Angajat")
                {
                    ViewData["ProjectName"] = new SelectList(_context.Projects, "Title", "Title");
                }
                
            }
            else
            {
                model.ProjectName = project.Title;
                ViewData["ProjectName"] = new SelectList(_context.Projects, "Title", "Title");
            }
            
            
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string Id, UserProjectViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.Title.Equals(model.ProjectName));
       
            if (user == null)
            {
                ViewBag.ErrorMessage = $" User with Id={model.Id} cannot be found";
                return NotFound();
            }
            else
            {
              
                if (ModelState.IsValid)
                {
                    try
                    {
                        
                        //var userId = _userManager.FindByIdAsync(Id);
                        //var myUser = await _userManager.FindByIdAsync(Id) as MyUser;
                        var roles = await _userManager.GetRolesAsync(user);
                        //await _userManager.RemoveFromRolesAsync(myUser, roles.ToArray());
                        //var role = new IdentityRole();
                        //role.Name = model.Role;
                       // await _userManager.AddToRoleAsync(user, role.Name);
                        //await _userManager.UpdateAsync(user);
                        if (roles[0] == "Angajat")
                         {
                            user.ProjectID = project.ID;
                         }
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(model.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("ListUsers");
                }

                return View(model);
            }

        }
        private bool UserExists(string id)
        {
            return _userManager.Users.Any(e => e.Id == id);
        }

        //GET
        public async Task<IActionResult> Delete(string id)
        {

            var user = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $" User with Id={id} cannot be found";
                return RedirectToAction("ListUsers");
            }
            else
            {
                var grantApps = await _context.GrantApplications.Include(e => e.Project).Where(u => u.UserID == id).ToListAsync();
                foreach (GrantApplication grantApplication in grantApps)
                {
                    _context.Remove(grantApplication);
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    ViewBag.ErrorMessage = $" User with Id={id} cannot be deleted";
                    return NotFound();

                }
            }
             
        }

        public async Task<IActionResult> GiveFullName(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(p => p.Id == id);

            var userModel = new UserProjectViewModel();

            userModel.FullName = user.FullName;

            return Json(new { success = true, data = userModel, message = "Numele utilizatorului a fost trimis in modal." });
        }
    }

}
