using System;
using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class UpdateExpenseCategoryDto
    {
        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public string? Description { get; set; }

        [Required]
        public StatusEnum Status { get; set; } // Active or Inactive
    }
}
