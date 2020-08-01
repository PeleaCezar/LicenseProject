using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LicenseProject.Models
{
    public class MyUser: IdentityUser

    {
        public string FullName { get; set; }

        public string Gender { get; set; }

        public string Function { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }

        public string PhotoPath { get; set; }

        public DateTime CreateDate { get; set; }

        public int NoOfUsers { get; set; }

        public int? ProjectID { get; set; }

        public virtual Project Project { get; set; }


        public virtual ICollection<GrantApplication> GrantApplications { get; set; }

    }
}
