using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Domain.Entities.Business;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IExpenseTypeRepository : IGenericRepository<ExpenseType>
    {
        Task<List<ExpenseTypeDto>> GetActiveExpenseTypesAsync();
        Task<bool> IsExpenseNameUniqueAsync(string expenseName, string UserId);
        bool IsExpenseCategoryLinkedToExpenseType(Guid ExpenseCategoryId);
        Task<int> getExpenseTypeByUserId(Guid UserId);
        Task<bool> addExpenseTypeOnRegister(Guid UserId);
        Task<dynamic> getUserExpenseTypes(Guid UserId);
    }
}