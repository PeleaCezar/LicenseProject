using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LicenseProject.ViewModels
{
    public class ProjectBudgetViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Trebuie să introduceți titlul proiectului")]
        [MaxLength(100, ErrorMessage = "Titlul nu poate conține mai mult de 100 de caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Trebuie să introduceți descrierea proiectului")]
        [MaxLength(400, ErrorMessage = "Descrierea nu poate conține mai mult de 400 de caractere")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Trebuie să introduceți data proiectului")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Trebuie să alocați membri proiectului")]
        public List<string> TeamMembers { get; set; }

        public string Status { get; set; }

        public int BudgetId { get; set; }

        
        [Required(ErrorMessage = "Trebuie să introduceți bugetul proiectului")]
        public decimal EstimatedBudget { get; set; }
     
     
        [Required(ErrorMessage = "Trebuie să introduceți cheltuielile estimative ale proiectului")]
        public decimal AmountSpent { get; set; }

        [Range(0, 3660, ErrorMessage ="Proiectul nu poate dura mai mult de 3660 zile")]
        [Required(ErrorMessage = "Trebuie să introduceți durata de timp")]
        public int ProjectDuration { get; set; }

        public string Currency { get; set; }

        [Required(ErrorMessage = "Trebuie să alegeți beneficiarul proiectului")]
        public string Faculty { get; set; }

        [Required(ErrorMessage = "Trebuie să selectați directorul de proiect")]
        public string ProjectDirector { get; set; }

        public string ExternalFinance { get; set; }

        public string NoOfHours { get; set; }

        public string DateCreate { get; set; }

        public List<EmployeeViewModel> Employees { get; set; }

        public List <string> GrantApplications { get; set; }


    }


}
