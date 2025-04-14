using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain.Entities.Business
{
    public class ExpenseType : BaseEntity
    {
        [Key]
        public Guid ExpenseTypeId { get; set; }

        [Required]
        public string ExpenseName { get; set; } // e.g., Electricity, Gas

        public string? Description { get; set; }

        [Required]
        public string UnitOfMeasurement { get; set; } // e.g., kWh, m³, liters
        public StatusEnum Status { get; set; }

        // Relationship: One ExpenseType belongs to one ExpenseCategory
        [ForeignKey("ExpenseCategory")]
        public Guid ExpenseCategoryId { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        // public Guid ForUserId { get; set; }
    }
}
