using CleanArchitecture.Application.DTOs.Business;

namespace CleanArchitecture.Application.Interfaces
{
    public interface IAddressExpenseService
    {
        public Task<AddressExpenseResponseDto> AddAddressExpense(AddressExpenseDto addressExpenseDto);
        public Task<List<AddressExpenseListResponseDto>> GetAddressExpenseList();
    }
}
