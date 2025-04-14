using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Business;
using MediatR;

namespace CleanArchitecture.Application.Features.Business.ExpenseTypes.Queries
{
    public class GetExpenseTypeListQuery : IRequest<List<ExpenseTypeDto>>
    {
        public bool ActiveOnly { get; set; }
    }

    public class GetExpenseTypeListQueryHandler : IRequestHandler<GetExpenseTypeListQuery, List<ExpenseTypeDto>>
    {
        private readonly IExpenseTypeRepository _expenseTypeRepository;
        private readonly IMapper _mapper;

        public GetExpenseTypeListQueryHandler(IExpenseTypeRepository expenseTypeRepository, IMapper mapper)
        {
            _expenseTypeRepository = expenseTypeRepository;
            _mapper = mapper;
        }

        public async Task<List<ExpenseTypeDto>> Handle(GetExpenseTypeListQuery request, CancellationToken cancellationToken)
        {
            return await _expenseTypeRepository.GetActiveExpenseTypesAsync();
        }
    }
}