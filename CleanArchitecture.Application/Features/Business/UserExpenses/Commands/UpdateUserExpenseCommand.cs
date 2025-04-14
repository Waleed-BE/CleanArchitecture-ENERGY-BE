using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Enums;
using MediatR;

namespace CleanArchitecture.Application.Features.Business.UserExpenses.Commands
{
    public class UpdateUserExpenseResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
    public class UpdateUserExpenseCommand : IRequest<UpdateUserExpenseResponse>
    {
        public Guid ExpenseId { get; set; }
        public StatusEnum Status { get; set; }

        public decimal Quantity { get; set; } // e.g., 120 kWh
        public decimal RatePerUnit { get; set; } // e.g., $0.12 per kWh

    }

    public class UpdateUserExpenseCommandHandler : IRequestHandler<UpdateUserExpenseCommand, UpdateUserExpenseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserExpenseRepository _userExpenseRepository;
        public UpdateUserExpenseCommandHandler(IUserExpenseRepository userExpenseRepository, IUnitOfWork unitOfWork)
        {
            _userExpenseRepository = userExpenseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateUserExpenseResponse> Handle(UpdateUserExpenseCommand request, CancellationToken cancellationToken)
        {
            var userExpenseExists = await _userExpenseRepository.GetByIdAsync(request.ExpenseId);

            if (userExpenseExists == null)
            {
                return new UpdateUserExpenseResponse() { IsSuccess = false, Message = "User Expense does not exists" };
            }

            userExpenseExists.RatePerUnit = request.RatePerUnit;
            userExpenseExists.Quantity = request.Quantity;
            userExpenseExists.Status = request.Status;

            await _userExpenseRepository.UpdateAsync(userExpenseExists);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new UpdateUserExpenseResponse() { IsSuccess = true, Message = "Record State Updated." };

        }
    }
}
