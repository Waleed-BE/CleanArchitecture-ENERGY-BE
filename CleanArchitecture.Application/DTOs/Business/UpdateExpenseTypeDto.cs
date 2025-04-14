using CleanArchitecture.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class UpdateExpenseTypeDto
    {
        [Required]
        public Guid ExpenseTypeId { get; set; }

        [Required]
        public string ExpenseName { get; set; } // e.g., Electricity, Gas

        public string? Description { get; set; }

        [Required]
        public string UnitOfMeasurement { get; set; } // e.g., kWh, m³, liters

        [Required]
        public Guid ExpenseCategoryId { get; set; }

        [Required]
        public StatusEnum Status { get; set; } // Active or Inactive
    }
}