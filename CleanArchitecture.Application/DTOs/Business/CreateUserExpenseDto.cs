using System;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class CreateUserExpenseDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public Guid ExpenseTypeId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public decimal Quantity { get; set; }
    }
}
