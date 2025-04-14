using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class CreateExpenseTypeDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ExpenseName { get; set; }
        public string Description { get; set; }
        public Guid ExpenseCategoryId { get; set; }
        public string UnitOfMeasurement { get; set; }
    }
}