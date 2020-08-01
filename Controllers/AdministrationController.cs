using System.Collections.Generic;
using System.Threading.Tasks;
using LicenseProject.Data;
using LicenseProject.Models;
using LicenseProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace LicenseProject.Controllers

{
    [Authorize(Roles = "Administrator")]
    public class AdministrationController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<MyUser> _signInManager;
        private readonly string _emailfrom;
        private readonly string _username;
        private readonly string _password;
        private readonly string _client;

        public AdministrationController(ApplicationDbContext context,
                                        UserManager<MyUser> userManager,
                                        RoleManager<IdentityRole> roleManager,
                                        SignInManager<MyUser> signInManager,
                                        IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailfrom = configuration["EmailConfiguration:EmailFrom"];
            _username = configuration["EmailConfiguration:Username"];
            _password = configuration["EmailConfiguration:Password"];
            _client = configuration["EmailConfiguration:Client"];
        }

        public async Task<IActionResult> GiveRoleEmployee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userRole = await _userManager.GetRolesAsync(user);

            var userProjectModel = new UserProjectViewModel();

            userProjectModel.Role = userRole[0];
            userProjectModel.FullName = user.FullName;
            userProjectModel.Id = user.Id;
            userProjectModel.Email = user.Email;

            return Json(new { success = true, data = userProjectModel, message = "Rolul a fost trimis in modal" });
        }

        [HttpGet]
        public async Task<IActionResult> PermissionEmployee()
        {
            var applicationDbContext = await _userManager.Users.Include(e => e.Project).ToListAsync();
            List<UserProjectViewModel> model = new List<UserProjectViewModel>();

            foreach (var user in applicationDbContext)
                if (user.EmailConfirmed == false)
                {
                    {
                        var userToModel = new UserProjectViewModel();

                        //ViewData["ProjectName"] = new SelectList(_context.Projects, "Title", "Title");
                        var theUser = await _userManager.GetRolesAsync(user);
                        if (theUser.Count > 0)
                        {
                            userToModel.Role = theUser[0];
                        }
                        else
                        {
                            userToModel.Role = "Inca nu s-a alocat un rol pentru acest utilizator!";
                        }
                        userToModel.Id = user.Id;
                        userToModel.Email = user.Email;
                        userToModel.UserName = user.UserName;
                        userToModel.FullName = user.FullName;
                        userToModel.CreateDate = user.CreateDate;
                        userToModel.MyUsers = user;

                        model.Add(userToModel);
                    }
                }
            return View(model);
        }

        [AcceptVerbs("Post")]
        public async Task<IActionResult> Accept(string email,string role)
        {
            if (role != null)
            {
                var myUser = await _userManager.FindByEmailAsync(email);
                var roleFromDb = await _userManager.GetRolesAsync(myUser);
                await _userManager.RemoveFromRolesAsync(myUser, roleFromDb.ToArray());
                var newRole = new IdentityRole();
                newRole.Name = role;
                await _userManager.AddToRoleAsync(myUser, newRole.Name);
                await _userManager.UpdateAsync(myUser);
                await _context.SaveChangesAsync();
            }

            UserProjectViewModel model = new UserProjectViewModel();
            var user = await _userManager.FindByEmailAsync(email);

            model.IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (user.EmailConfirmed == false)
            {
                user.EmailConfirmed = true;
                user.NoOfUsers--;
                if (user.EmailConfirmed == true)
                {
                    MimeMessage message = new MimeMessage();
                    MailboxAddress from = new MailboxAddress(_emailfrom);
                    message.From.Add(from);

                    MailboxAddress to = new MailboxAddress(email);
                    message.To.Add(to);
                    message.Subject = "Cererea de inregistrare a fost acceptata";
                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.TextBody = "Contul dumneavostra a fost autorizat pentru folosirea aplicatiei de cercetare din cadrul UCV.";
                    message.Body = bodyBuilder.ToMessageBody();
                    SmtpClient client = new SmtpClient();
                    client.Connect(_client, 465, true);
                    client.Authenticate(_username, _password);
                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();
                }

            }

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("PermissionEmployee");
        }

        /* [HttpGet]
         public async Task<IActionResult> ConfirmRole(string id)
         {
             var user =  await _userManager.FindByIdAsync(id);
             return PartialView("ConfirmRoleModal", user);
         }*/


        [HttpGet]
        public async Task<IActionResult> Refused(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ViewBag.ErrorMessage = $" User with Id={email} cannot be found";
                return NotFound();
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);
                user.NoOfUsers--;

                if (result.Succeeded)
                {
                    MimeMessage message = new MimeMessage();
                    MailboxAddress from = new MailboxAddress(_emailfrom);
                    message.From.Add(from);

                    MailboxAddress to = new MailboxAddress(email);
                    message.To.Add(to);
                    message.Subject = "Cererea de inregistrare a fost refuzata";
                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.TextBody = "Contul dumneavostra nu a fost autorizat pentru folosirea aplicatiei de cercetare din cadrul UCV.";
                    message.Body = bodyBuilder.ToMessageBody();
                    SmtpClient client = new SmtpClient();
                    client.Connect(_client, 465, true);
                    client.Authenticate(_username, _password);
                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();

                    return RedirectToAction("PermissionEmployee");
                }
                else
                {
                    ViewBag.ErrorMessage = $" User with Id={email} cannot be deleted";
                    return NotFound();

                }


            }
        }
    }

}

