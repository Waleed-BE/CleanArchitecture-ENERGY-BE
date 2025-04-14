using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Domain.Entities.Business
{
    public class ExpenseCategory : BaseEntity
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; } // e.g., Utilities, Transportation

        public string? Description { get; set; }
        public StatusEnum Status { get; set; }
        // Relationship: One Category can have many ExpenseTypes
        public ICollection<ExpenseType> ExpenseTypes { get; set; } = new List<ExpenseType>();
    }
}
