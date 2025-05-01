using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class AddressExpenseListResponseDto
    {
        public Guid UserAddressExpenseId { get; set; }
        public Guid ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
        public Guid AddressId { get; set; }
        public string AddressName { get; set; }
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
