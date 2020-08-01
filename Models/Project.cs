using System;
using System.Collections.Generic;

namespace LicenseProject.Models
{
    public class Project
    {
        public int ID { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public string TeamMembers { get; set; }

        public string Status { get; set; }


        public int BudgetId { get; set; }

        public Budget Budget { get; set; }



        public virtual ICollection <MyUser> MyUsers { get; set; }


        public virtual ICollection<Task> Tasks { get; set; }


        public virtual ICollection<GrantApplication> GrantApplications { get; set; }

    }
}
