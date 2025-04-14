using CleanArchitecture.Application.DTOs.Auth;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Queries
{
    public class GetExpenseTypesByRoleQuery : IRequest<List<RoleExpenseTypeDTO>>
    {
        public bool Active { get; set; }
    }
}
