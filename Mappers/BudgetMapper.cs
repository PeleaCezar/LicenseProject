using LicenseProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LicenseProject.Mappers
{
    public class BudgetMapper
    {
        public static int GetBudgetId(Budget budget)
        {
            return budget.Id;
        }
    }
}
