using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Application.Interfaces;

namespace CleanArchitecture.Application.Services
{
    public class AddressExpenseService : IAddressExpenseService
    {
        private readonly IAddressExpenseRepository _addressExpenseRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddressExpenseService(IAddressExpenseRepository addressExpenseRepository, IUnitOfWork unitOfWork)
        {
            _addressExpenseRepository = addressExpenseRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<AddressExpenseResponseDto> AddAddressExpense(AddressExpenseDto addressExpenseDto)
        {
            var response = await _addressExpenseRepository.AddAddressExpense(addressExpenseDto);
            await _unitOfWork.CompleteAsync();
            return response;
        }

        public async Task<List<AddressExpenseListResponseDto>> GetAddressExpenseList()
        {
            return await _addressExpenseRepository.GetAddressExpenseList();
        }
    }
}
