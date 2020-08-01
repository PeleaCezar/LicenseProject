using System;

namespace LicenseProject.Models
{
    public class GrantApplication
    {
        public int ID { get; set; }

        public string Entity { get; set; }

        public DateTime Date { get; set; }

        public decimal AmountOfFunding { get; set; }

        public string Status { get; set; }

        public string Currency { get; set; }

        public bool isConfirmed { get; set; }

        public int? ProjectID { get; set; }

        public string Representantive { get; set; }

        public int NoOfGrantApp { get; set; }

        public virtual Project Project { get; set; }


        public string UserID { get; set; }

        public virtual MyUser MyUser { get; set; }
    }
}
