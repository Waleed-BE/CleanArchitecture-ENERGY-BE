using CleanArchitecture.Domain.Entities.Business;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IExpenseCategoryRepository : IGenericRepository<ExpenseCategory>
    {
        Task<IEnumerable<ExpenseCategory>> GetActiveExpenseCategoriesAsync();
        Task<bool> IsExpenseNCategoryameUniqueAsync(string expenseCategoryName);
    }
}
