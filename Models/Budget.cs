
namespace LicenseProject.Models
{
    public class Budget
    {
        public int Id { get;set; }

        public decimal EstimatedBudget { get; set; }

        public decimal AmountSpent { get; set; }

        public int ProjectDuration { get; set; }

        public string Currency { get; set; }

        public string Faculty { get; set; }

        public string ProjectDirector { get; set; }


        public decimal ExternalFinance{ get; set; }

        public decimal NoOfHours { get; set; }


    }
}
