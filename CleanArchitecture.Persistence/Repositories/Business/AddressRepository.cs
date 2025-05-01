using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.Persistence.Repositories.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.Repositories.Business
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AddressRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Address>> getAddress()
        {
            var userId = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var userAddressList = await _dbContext.Set<Address>()
                .Where(x => x.CreatedBy == userId && x.Status == Domain.Enums.StatusEnum.Active)
                .ToListAsync();
            return userAddressList;
        }
    }
}
