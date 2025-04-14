using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Business;
using MediatR;

namespace CleanArchitecture.Application.Features.Business.ExpenseCategories.Queries
{
    public class GetExpenseCategoryListQuery : IRequest<List<ExpenseCategoryDTO>>
    {
        public bool ActiveOnly { get; set; }
    }

    public class GetExpenseCategoryListQueryHandler : IRequestHandler<GetExpenseCategoryListQuery, List<ExpenseCategoryDTO>>
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;
        private readonly IMapper _mapper;


        public GetExpenseCategoryListQueryHandler(IExpenseCategoryRepository expenseCategoryRepository, IMapper mapper)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
            _mapper = mapper;
        }

        public async Task<List<ExpenseCategoryDTO>> Handle(GetExpenseCategoryListQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Business.ExpenseCategory> expenseCategories;

            if (request.ActiveOnly)
            {
                expenseCategories = await _expenseCategoryRepository.GetActiveExpenseCategoriesAsync();
            }
            else
            {
                expenseCategories = await _expenseCategoryRepository.GetAllAsync();
            }

            return _mapper.Map<List<ExpenseCategoryDTO>>(expenseCategories);
        }


    }
}
