using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using MediatR;

namespace CleanArchitecture.Application.Features.Business.ExpenseCategory.Commands
{

    public class CreateCategoryResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class CreateExpenseCategoryCommand : IRequest<CreateCategoryResponse>
    {
        public string ExpenseCategoryName { get; set; }
        public string Description { get; set; }
    }
    public class CreateExpenseCategoryCommandHandler : IRequestHandler<CreateExpenseCategoryCommand, CreateCategoryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;


        public CreateExpenseCategoryCommandHandler(IUnitOfWork unitOfWork, IExpenseCategoryRepository expenseCategoryRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _expenseCategoryRepository = expenseCategoryRepository;

        }

        public async Task<CreateCategoryResponse> Handle(CreateExpenseCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if expense category already exists
            var isUnique = await _expenseCategoryRepository.IsExpenseNCategoryameUniqueAsync(request.ExpenseCategoryName);
            if (!isUnique)
            {
                return new CreateCategoryResponse() { IsSuccess = false, Message = $"{request.ExpenseCategoryName} expense category is already present." };
            }

            var expenseCategory = new Domain.Entities.Business.ExpenseCategory()
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = request.ExpenseCategoryName,
                Description = request.Description,
                Status = Domain.Enums.StatusEnum.Active
            };

            await _expenseCategoryRepository.AddAsync(expenseCategory);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new CreateCategoryResponse() { IsSuccess = true, Message = $"{expenseCategory.CategoryName} expense category is created." };
        }

    }
}