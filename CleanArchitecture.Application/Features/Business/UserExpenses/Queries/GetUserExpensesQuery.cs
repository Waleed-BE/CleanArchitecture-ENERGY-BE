using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Business;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Business.UserExpense.Queries
{
    public class GetUserExpensesQuery : IRequest<List<UserExpenseDto>>
    {
        public string UserId { get; set; }
    }

    public class GetUserExpensesQueryHandler : IRequestHandler<GetUserExpensesQuery, List<UserExpenseDto>>
    {
        private readonly IUserExpenseRepository _userExpenseRepository;
        private readonly IMapper _mapper;

        public GetUserExpensesQueryHandler(IUserExpenseRepository userExpenseRepository, IMapper mapper)
        {
            _userExpenseRepository = userExpenseRepository;
            _mapper = mapper;
        }

        public async Task<List<UserExpenseDto>> Handle(GetUserExpensesQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _userExpenseRepository.GetUserExpensesAsync(request.UserId);
            return _mapper.Map<List<UserExpenseDto>>(expenses);
        }
    }
}
