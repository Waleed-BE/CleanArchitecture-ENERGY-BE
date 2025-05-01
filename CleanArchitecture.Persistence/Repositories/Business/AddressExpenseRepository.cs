using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.Persistence.Repositories.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.Repositories.Business
{
    public class AddressExpenseRepository : GenericRepository<AddressExpenses>, IAddressExpenseRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AddressExpenseRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AddressExpenseResponseDto> AddAddressExpense(AddressExpenseDto addressExpenseDto)
        {
            if(addressExpenseDto == null)
            {
                return new AddressExpenseResponseDto() { IsSuccess = false, Message = "Input is not valid" };
            }

            AddressExpenses addressExpenses = new AddressExpenses()
            {
                UserAddressExpenseId = Guid.NewGuid(),
                ExpenseTypeId = addressExpenseDto.ExpenseTypeId,
                AddressId = addressExpenseDto.AddressId,
                ExpenseForDate = addressExpenseDto.ExpenseForDate,
                Quantity = addressExpenseDto.Quantity,
                RatePerUnit = addressExpenseDto.RatePerUnit,
                Status = Domain.Enums.StatusEnum.Active
            };

            await _dbContext.Set<AddressExpenses>().AddAsync(addressExpenses);
            return new AddressExpenseResponseDto() { IsSuccess = true, Message = "Expense Per address is added" };
        }
        public async Task<List<AddressExpenseListResponseDto>> GetAddressExpenseList()
        {
            var userId = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            var expenses = await _dbContext.Set<AddressExpenses>()
                .Where(ae => ae.Address.CreatedBy == userId)
                .Include(ae => ae.Address)
                .Include(ae => ae.ExpenseType)
                .Select(ae => new AddressExpenseListResponseDto
                {
                    UserAddressExpenseId = ae.UserAddressExpenseId,
                    ExpenseTypeId = ae.ExpenseTypeId,
                    ExpenseTypeName = ae.ExpenseType.ExpenseName,
                    AddressId = ae.AddressId,
                    AddressName = ae.Address.AddressName,
                    Status = ae.Status,
                    Quantity = ae.Quantity,
                    RatePerUnit = ae.RatePerUnit,
                    ExpenseForDate = ae.ExpenseForDate
                    // TotalCost will be calculated inside DTO from Quantity * RatePerUnit
                })
                .ToListAsync();

            return expenses;
        }

    }
}
