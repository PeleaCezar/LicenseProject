using System;

namespace LicenseProject.ViewModels
{
    public class ProjectViewModel
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public int NoOfMembers { get; set; }

        public string CreateDate { get; set; }
    }
}
