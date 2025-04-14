using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOs.Auth
{
    public class RoleExpenseTypeDTO
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public Guid ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
    }
}
