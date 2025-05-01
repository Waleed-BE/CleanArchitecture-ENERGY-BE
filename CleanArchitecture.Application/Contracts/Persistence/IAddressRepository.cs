using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Domain.Entities.Business;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        public Task<List<Address>> getAddress();
    }
}
