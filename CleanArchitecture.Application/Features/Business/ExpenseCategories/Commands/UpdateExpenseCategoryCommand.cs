using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Enums;
using MediatR;

namespace CleanArchitecture.Application.Features.Business.ExpenseCategories.Commands
{

    public class UpdateExpenseCategoryResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }


    public class UpdateExpenseCategoryCommand : IRequest<UpdateExpenseCategoryResponse>
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public StatusEnum Status { get; set; } // Soft delete by setting Status = Inactive
    }

    public class UpdateExpenseCategoryCommandHandler : IRequestHandler<UpdateExpenseCategoryCommand, UpdateExpenseCategoryResponse>
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;
        private readonly IExpenseTypeRepository _expenseTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateExpenseCategoryCommandHandler(IExpenseCategoryRepository expenseCategoryRepository, IExpenseTypeRepository expenseTypeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
            _expenseTypeRepository = expenseTypeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UpdateExpenseCategoryResponse> Handle(UpdateExpenseCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if the category exists
            var existingCategory = await _expenseCategoryRepository.GetByIdAsync(request.CategoryId);
            if (existingCategory == null)
            {
                return new UpdateExpenseCategoryResponse() { IsSuccess = false, Message = "Expense category you want to update does not found." };
            }


            var categoryIsInUse = _expenseTypeRepository.IsExpenseCategoryLinkedToExpenseType(request.CategoryId);
            if (categoryIsInUse && request.Status == StatusEnum.Inactive)
            {
                return new UpdateExpenseCategoryResponse() { IsSuccess = false, Message = "Expense category you want to delete cannot be deleted because is referenced with Expense type." };
            }

            // Map the changes
            _mapper.Map(request, existingCategory);

            // Update category in DB
            await _expenseCategoryRepository.UpdateAsync(existingCategory);
            await _unitOfWork.CompleteAsync(cancellationToken);

            if (!categoryIsInUse && request.Status == StatusEnum.Inactive)
            {
                return new UpdateExpenseCategoryResponse() { IsSuccess = true, Message = "Expense category deleted successfully" };
            }

            return new UpdateExpenseCategoryResponse() { IsSuccess = true, Message = "Expense category updated." };
        }
    }

}
