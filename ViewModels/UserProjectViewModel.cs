using LicenseProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace LicenseProject.ViewModels
{
    public class UserProjectViewModel
    {
   
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string Gender { get; set; }

        public string Function { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Role { get; set; }
        
        public IFormFile Photo { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public MyUser MyUsers { get; set; }

        public List<ProjectGrantAppViewModel> FinancedProjects { get; set; }
        public bool IsFinancier { get; set; }
        public bool IsEmployee { get; set; }

 

    }
}
