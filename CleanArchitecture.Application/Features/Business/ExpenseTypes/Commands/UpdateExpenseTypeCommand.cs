using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Enums;
using MediatR;

namespace CleanArchitecture.Application.Features.Business.ExpenseTypes.Commands
{

    public class UpdateExpenseTypeResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
    public class UpdateExpenseTypeCommand : IRequest<UpdateExpenseTypeResponse>
    {
        public Guid ExpenseTypeId { get; set; }
        public string ExpenseName { get; set; }
        public string? Description { get; set; }
        public string UnitOfMeasurement { get; set; }
        public Guid ExpenseCategoryId { get; set; }
        public StatusEnum Status { get; set; } // Soft delete by setting Status = Inactive
    }

    public class UpdateExpenseTypeCommandHandler : IRequestHandler<UpdateExpenseTypeCommand, UpdateExpenseTypeResponse>
    {
        private readonly IExpenseTypeRepository _expenseTypeRepository;
        private readonly IUserExpenseRepository _userExpenseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateExpenseTypeCommandHandler(IExpenseTypeRepository expenseTypeRepository, IUserExpenseRepository userExpenseRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _expenseTypeRepository = expenseTypeRepository;
            _userExpenseRepository = userExpenseRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateExpenseTypeResponse> Handle(UpdateExpenseTypeCommand request, CancellationToken cancellationToken)
        {
            // Check if the ExpenseType exists
            var existingExpenseType = await _expenseTypeRepository.GetByIdAsync(request.ExpenseTypeId);
            if (existingExpenseType == null)
            {
                return new UpdateExpenseTypeResponse() { IsSuccess = false, Message = "Expense Type not found" }; // Not found
            }

            var expenseTypeIsInUse = _userExpenseRepository.isExpenseTypeActiveInAnyExpenseById(request.ExpenseTypeId);
            if (expenseTypeIsInUse && request.Status == StatusEnum.Inactive)
            {
                return new UpdateExpenseTypeResponse() { IsSuccess = false, Message = "Expense Type you want to delete cannot be deleted because is referenced with User's Expense." };
            }
            existingExpenseType.UnitOfMeasurement = request.UnitOfMeasurement;
            existingExpenseType.ExpenseName = request.ExpenseName;
            existingExpenseType.ExpenseCategoryId = request.ExpenseCategoryId;


            if (!expenseTypeIsInUse && request.Status == StatusEnum.Active)
            {
                existingExpenseType.Status = request.Status;
            }


            // Update in DB
            await _expenseTypeRepository.UpdateAsync(existingExpenseType);
            await _unitOfWork.CompleteAsync(cancellationToken);


            if (!expenseTypeIsInUse && request.Status == StatusEnum.Active)
            {
                return new UpdateExpenseTypeResponse() { IsSuccess = false, Message = "Expense Type reactivated successfully" };
            }

            if (!expenseTypeIsInUse && request.Status == StatusEnum.Inactive)
            {
                return new UpdateExpenseTypeResponse() { IsSuccess = true, Message = "Expense type deleted successfully" };
            }

            return new UpdateExpenseTypeResponse() { IsSuccess = true, Message = "Expense type updated successfully" };
        }
    }
}
