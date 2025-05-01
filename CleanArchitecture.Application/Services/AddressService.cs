using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities.Business;

namespace CleanArchitecture.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddressService(IAddressRepository addressTypeRepository, IUnitOfWork unitOfWork)
        {
            _addressTypeRepository = addressTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AddAddressResponseDto> AddCompanyUserAddress(AddAddressDto addAddressDto)
        {
            if(addAddressDto == null)
            {
                return new AddAddressResponseDto { IsSuccess = false, Message = "Invalid Input" };
            }

            Address address = new Address()
            {
                AddressId = Guid.NewGuid(),
                AddressName = addAddressDto.AddressName,
                AddressType = addAddressDto.AddressType,
                CompleteAddress = addAddressDto.CompleteAddress,
                Description = addAddressDto.Description,
                Status = Domain.Enums.StatusEnum.Active,
            };

            await _addressTypeRepository.AddAsync(address);
            await _unitOfWork.CompleteAsync();

            return new AddAddressResponseDto { IsSuccess = true, Message = "Address Added successfully" };
        }

        public async Task<List<Address>> GetCompanyAddress()
        {
            return await _addressTypeRepository.getAddress();
        }
    }
}
