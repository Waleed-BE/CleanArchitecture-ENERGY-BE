using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Domain.Entities.Business;

namespace CleanArchitecture.Application.Interfaces
{
    public interface IAddressService
    {
        public Task<AddAddressResponseDto> AddCompanyUserAddress(AddAddressDto addAddressDto);
        public Task<List<Address>> GetCompanyAddress();

    }
}
