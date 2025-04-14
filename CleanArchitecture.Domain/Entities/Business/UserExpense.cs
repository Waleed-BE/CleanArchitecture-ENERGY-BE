using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.Auth;
using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain.Entities.Business
{
    public class UserExpense : BaseEntity
    {
        [Key]
        public Guid ExpenseId { get; set; }

        // Relationship: Each Expense belongs to a User
        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Relationship: Each Expense is associated with an ExpenseType
        [Required]
        [ForeignKey("ExpenseType")]
        public Guid ExpenseTypeId { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public StatusEnum Status { get; set; }
        [Required]
        public decimal Quantity { get; set; } // e.g., 120 kWh
        [Required]
        public decimal RatePerUnit { get; set; } // e.g., $0.12 per kWh
        [Required]
        public decimal TotalCost => Quantity * RatePerUnit; // Auto-calculated
        public DateTime? ExpenseForDate { get; set; }
    }
}
