using System;
using System.Collections.Generic;

namespace LicenseProject.Models
{
    public class Task
    {

        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public decimal Duration { get; set; } // duration for task

        public string Status { get; set; }

        public string Priority { get; set; }

        public string UserID { get; set; }

        public int? ProjectID { get; set; }

        public Project Project { get; set; }



    }
}
