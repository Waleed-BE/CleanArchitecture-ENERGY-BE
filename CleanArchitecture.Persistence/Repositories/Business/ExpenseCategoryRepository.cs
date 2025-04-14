using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.Repositories.Business
{
    public class ExpenseCategoryRepository : GenericRepository<ExpenseCategory>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ExpenseCategory>> GetActiveExpenseCategoriesAsync()
        {
            return await _dbContext.Set<ExpenseCategory>()
                .Where(et => et.Status == StatusEnum.Active)
                .OrderBy(et => et.CategoryName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> IsExpenseNCategoryameUniqueAsync(string expenseCategoryName)
        {
            return !await _dbContext.Set<ExpenseCategory>()
                            .AnyAsync(et => et.CategoryName.ToLower() == expenseCategoryName.ToLower());
        }
    }
}
