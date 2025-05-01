using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain.Entities.Business
{
    public class AddressExpenses : BaseEntity
    {
        [Key]
        public Guid UserAddressExpenseId { get; set; }

        [ForeignKey("ExpenseType")]
        public Guid ExpenseTypeId { get; set; }
        public ExpenseType ExpenseType { get; set; }

        [ForeignKey("Address")]
        public Guid AddressId { get; set; }
        public Address Address { get; set; }

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
