using System;

namespace CleanArchitecture.Application.DTOs.Business
{
    public class UserExpenseDto
    {
        public Guid ExpenseId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Guid ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
