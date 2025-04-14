using System;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class ExpenseTypeDto
    {
        public Guid ExpenseTypeId { get; set; }
        public string ExpenseName { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Guid ExpenseCategoryId { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string ExpenseCategoryName { get; set; }
    }
}