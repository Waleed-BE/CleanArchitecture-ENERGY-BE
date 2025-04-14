using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Business.UserExpenses.Commands
{
    public class CreateUserExpenseCommand : IRequest<Guid>
    {
        public string UserId { get; set; }
        public Guid ExpenseTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal RatePerUnit { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime? ExpenseForDate { get; set; }
    }

    public class CreateUserExpenseCommandHandler : IRequestHandler<CreateUserExpenseCommand, Guid>
    {
        private readonly IUserExpenseRepository _userExpenseRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserExpenseCommandHandler(IUserExpenseRepository userExpenseRepository, IUnitOfWork unitOfWork,IMapper mapper)
        {
            _userExpenseRepository = userExpenseRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateUserExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = _mapper.Map<Domain.Entities.Business.UserExpense>(request);
            expense.ExpenseId = Guid.NewGuid();
            expense.ExpenseForDate = request.ExpenseForDate;
            await _userExpenseRepository.AddAsync(expense);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return expense.ExpenseId;
        }
    }
}
