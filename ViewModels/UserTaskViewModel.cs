using System;
using System.ComponentModel.DataAnnotations;

namespace LicenseProject.ViewModels
{
    public class UserTaskViewModel
    {
       // public string UserId { get; set; }

        //public string UserName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }       

       // public string Function { get; set; }

       // [Display(Name = "Project Name")]
        //public string ProjectName { get; set; }

        public string Priority { get; set; }

        public int TaskID { get; set; }

        public string TaskName { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Duration { get; set; } 

        public string Status { get; set; }

        public int? ProjectID { get; set; }

        public string TeamMembers { get; set; }


        public int NoOfTasksToDo { get; set; }

        public int NoOfTasksProgress { get; set; }

        public int NoOfTasksTesting { get; set; }

        public int NoOfTasksDone{ get; set; }

    }
}
