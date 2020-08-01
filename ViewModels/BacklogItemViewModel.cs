using System;

namespace LicenseProject.ViewModels
{
    
    public class BacklogItemViewModel
    {   
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string NumberOfHours { get; set; }
        public string Priority { get; set; }
        public string Employee{ get; set; }


        public int TaskID { get; set; } //update status
        public string Status { get; set; }// update status

        public string StartDate { get; set; } //details
    }
}
