using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class AddressExpenseDto
    {
        public Guid UserAddressExpenseId { get; set; }
        public Guid ExpenseTypeId { get; set; }
        public Guid AddressId { get; set; }
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
