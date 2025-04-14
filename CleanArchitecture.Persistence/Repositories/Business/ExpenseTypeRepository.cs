// CleanArchitecture.Persistence/Repositories/Business/ExpenseTypeRepository.cs
using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.Persistence.Repositories.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CleanArchitecture.Persistence.Repositories.Business
{
    public class ExpenseTypeRepository : GenericRepository<ExpenseType>, IExpenseTypeRepository
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExpenseTypeRepository(ApplicationDbContext dbContext, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ExpenseTypeDto>> GetActiveExpenseTypesAsync()
        {

            var createdBy = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var expenseTypes = await _dbContext.Set<ExpenseType>()
                .Include(x => x.ExpenseCategory)
                .Where(et => et.CreatedBy == createdBy && et.Status == StatusEnum.Active)
                .OrderBy(et => et.ExpenseName)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<ExpenseTypeDto>>(expenseTypes);
        }

        public bool IsExpenseCategoryLinkedToExpenseType(Guid ExpenseCategoryId)
        {
            return _dbContext.Set<ExpenseType>()
                .Where(x => x.ExpenseCategoryId == ExpenseCategoryId && x.Status == StatusEnum.Active).Any();
        }

        public async Task<bool> IsExpenseNameUniqueAsync(string expenseName, string UserId)
        {
            return !await _dbContext.Set<ExpenseType>()
                .AnyAsync(et => et.ExpenseName.ToLower() == expenseName.ToLower() && et.CreatedBy == UserId);
        }
        public async Task<int> getExpenseTypeByUserId(Guid UserId)
        {
            return await _dbContext.Set<ExpenseType>().Where(x => x.CreatedBy == UserId.ToString()
                            && x.Status == StatusEnum.Active).CountAsync();
        }
        public async Task<bool> addExpenseTypeOnRegister(Guid UserId)
        {
            var utilityExpenseCategoryTypeId = await _dbContext.Set<ExpenseCategory>()
                        .Where(x => x.CategoryName.ToLower() == _configuration["BaseCategory"].ToLower())
                        .Select(x => x.CategoryId).FirstOrDefaultAsync();
            List <ExpenseType> ExpenseTypes = new List<ExpenseType>()
            {
                new ExpenseType
                        {
                            ExpenseTypeId = Guid.NewGuid(),
                            ExpenseName = "Electricity",
                            Description = "This Expense Type is for the electricity related expenses.",
                            ExpenseCategoryId = utilityExpenseCategoryTypeId,
                            UnitOfMeasurement = "KWH",
                            Status = Domain.Enums.StatusEnum.Active,
                        },
                new ExpenseType
                        {
                            ExpenseTypeId = Guid.NewGuid(),
                            ExpenseName = "Gas",
                            Description = "This Expense Type is for the gas related expenses.",
                            ExpenseCategoryId = utilityExpenseCategoryTypeId,
                            UnitOfMeasurement = "M³",
                            Status = Domain.Enums.StatusEnum.Active,

                        },
                new ExpenseType
                        {
                            ExpenseTypeId = Guid.NewGuid(),
                            ExpenseName = "Water",
                            Description = "This Expense Type is for the water related expenses.",
                            ExpenseCategoryId = utilityExpenseCategoryTypeId,
                            UnitOfMeasurement = "M³",
                            Status = Domain.Enums.StatusEnum.Active,

                        },
                new ExpenseType
                        {
                            ExpenseTypeId = Guid.NewGuid(),
                            ExpenseName = "Fuel",
                            Description = "This Expense Type is for the Fuel related expenses.",
                            ExpenseCategoryId = utilityExpenseCategoryTypeId,
                            UnitOfMeasurement = "L",
                            Status = Domain.Enums.StatusEnum.Active,
                        },
            };

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, UserId.ToString()), // Add jti if needed
                new Claim("UserId", UserId.ToString() ?? ""),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _httpContextAccessor.HttpContext.User = principal;


            await _dbContext.Set<ExpenseType>().AddRangeAsync(ExpenseTypes);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<dynamic> getUserExpenseTypes(Guid UserId)
        {
            return await _dbContext.Set<ExpenseType>().Where(x => x.CreatedBy == UserId.ToString()
                           && x.Status == StatusEnum.Active).ToListAsync();
        }
    }
}