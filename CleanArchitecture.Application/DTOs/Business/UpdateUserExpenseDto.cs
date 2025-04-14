using System;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class UpdateUserExpenseDto
    {
        [Required]
        public Guid ExpenseId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public decimal Quantity { get; set; }
    }
}
