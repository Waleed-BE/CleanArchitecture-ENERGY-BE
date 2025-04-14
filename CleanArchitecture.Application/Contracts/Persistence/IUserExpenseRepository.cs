using CleanArchitecture.Domain.Entities.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IUserExpenseRepository : IGenericRepository<UserExpense>
    {
        Task<IEnumerable<UserExpense>> GetUserExpensesAsync(string userId);
        Task<UserExpense> GetUserExpenseByIdAsync(Guid expenseId);

        public bool isExpenseTypeActiveInAnyExpenseById(Guid expenseTypeId);
    }
}
