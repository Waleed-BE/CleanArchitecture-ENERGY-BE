using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.Persistence.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.Repositories.Business
{
    public class StripePurchasePlanRepository : GenericRepository<PurchaseHistory>, IStripePurchasePlanRepository
    {
        public StripePurchasePlanRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
