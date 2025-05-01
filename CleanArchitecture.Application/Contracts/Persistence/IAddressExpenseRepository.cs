using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Domain.Entities.Business;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IAddressExpenseRepository : IGenericRepository<AddressExpenses>
    {
        public Task<AddressExpenseResponseDto> AddAddressExpense(AddressExpenseDto addressExpenseDto);
        public Task<List<AddressExpenseListResponseDto>> GetAddressExpenseList();
    }
}
