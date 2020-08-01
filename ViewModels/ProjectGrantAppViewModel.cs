using System;
using System.ComponentModel.DataAnnotations;

namespace LicenseProject.ViewModels
{
    public class ProjectGrantAppViewModel
    {
        public int GrantApplicationID { get; set; }

        [Required(ErrorMessage = "Trebuie să introduceți numele companiei")]
        [MaxLength(100, ErrorMessage = "Numele nu poate conține mai mult de 100 de caractere")]
        public string Entity { get; set; }

        public DateTime Date { get; set; }

        [Range(1000, 1000000, ErrorMessage = "Suma de finanțare trebuie sa fie intre 1000 de lei si 1.000.000 lei")]
        [Required(ErrorMessage = "Trebuie să introduceți durata de timp")]
        public decimal AmountOfFunding { get; set; }

        public string Status { get; set; }

        public string Currency { get; set; }

        public string ProjectName { get; set; }

        public string UserID { get; set; }

        [Required(ErrorMessage = "Trebuie sa introduceți numele reprezentantului")]
        [MinLength(7, ErrorMessage = "Numele reprezentantului trebuie să conțina minim 7 caractere")]
        public string Representative { get; set; }

        public int? ProjectID { get; set; }

        

 


    }
}
