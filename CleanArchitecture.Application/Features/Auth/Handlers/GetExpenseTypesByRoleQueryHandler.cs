using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.Features.Auth.Queries;
using CleanArchitecture.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Auth.Handlers
{
    public class GetExpenseTypesByRoleQueryHandler : IRequestHandler<GetExpenseTypesByRoleQuery, List<RoleExpenseTypeDTO>>
    {
        private readonly IExpenseTypeRepository _expenseTypeRepository;
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public GetExpenseTypesByRoleQueryHandler(IExpenseTypeRepository expenseTypeRepository
            ,UserManager<ApplicationUser> userManager
            ,RoleManager<ApplicationRole> roleManager
            , IExpenseCategoryRepository expenseCategoryRepository
            )
        {
            _expenseTypeRepository = expenseTypeRepository;
            _roleManager = roleManager;
            _userManager = userManager;
            _expenseCategoryRepository = expenseCategoryRepository;
        }

        public async Task<List<RoleExpenseTypeDTO>> Handle(GetExpenseTypesByRoleQuery request, CancellationToken cancellationToken)
        {
            var roles = _roleManager.Roles
                   .Select(r => new { r.Id, r.Name, r.NormalizedName }) // Include NormalizedName
                   .ToList(); var expensesCategories = await _expenseCategoryRepository.GetActiveExpenseCategoriesAsync();
            var expenseTypes = await _expenseTypeRepository.GetActiveExpenseTypesAsync();

            var result = new List<RoleExpenseTypeDTO>();

            foreach (var role in roles)
            {
                // If role is "USER", only include categories where name is "Utility"
                var filteredExpenseTypes = role.NormalizedName == "USER"
                    ? expenseTypes.Where(et => et.ExpenseCategoryName == "Utility").ToList()
                    : expenseTypes;

                foreach (var expense in filteredExpenseTypes)
                {
                    result.Add(new RoleExpenseTypeDTO
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        ExpenseTypeId = expense.ExpenseTypeId,
                        ExpenseTypeName = expense.ExpenseName
                    });
                }
            }
            return result;
        }
    }
}
