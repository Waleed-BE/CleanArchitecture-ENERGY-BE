using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.Repositories.Business
{
    public class UserExpenseRepository : GenericRepository<UserExpense>, IUserExpenseRepository
    {
        public UserExpenseRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<UserExpense>> GetUserExpensesAsync(string userId)
        {
            return await _dbContext.Set<UserExpense>()
                .Include(e => e.ExpenseType)
                .Include(e => e.User)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserExpense> GetUserExpenseByIdAsync(Guid expenseId)
        {
            return await _dbContext.Set<UserExpense>()
                .Include(e => e.ExpenseType)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId) ?? new UserExpense();
        }


        public bool isExpenseTypeActiveInAnyExpenseById(Guid expenseTypeId)
        {
            return _dbContext.Set<UserExpense>()
                .Where(x => x.ExpenseTypeId == expenseTypeId && x.Status == Domain.Enums.StatusEnum.Active).Any();
        }
    }
}
