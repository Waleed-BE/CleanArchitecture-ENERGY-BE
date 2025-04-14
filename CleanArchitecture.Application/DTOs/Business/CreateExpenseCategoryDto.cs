using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class CreateExpenseCategoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ExpenseCategoryName { get; set; }

        public string Description { get; set; }
    }
}
