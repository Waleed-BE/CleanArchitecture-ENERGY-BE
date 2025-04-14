using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities.Business;
using MediatR;

namespace CleanArchitecture.Application.Features.Business.StripePurchasePlan.Commands
{
    public class CreateStripePurchaseCommandResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
    public class CreateStripePurchaseCommand : IRequest<CreateStripePurchaseCommandResponse>
    {
        public string UserId { get; set; }
        public string StripePriceId { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Interval { get; set; }
        public string PlanPurchaseType { get; set; }
    }

    public class CreateStripePurchaseCommandHandler : IRequestHandler<CreateStripePurchaseCommand, CreateStripePurchaseCommandResponse>
    {
        private readonly IStripePurchasePlanRepository _stripePurchasePlanRepository;
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;
        public CreateStripePurchaseCommandHandler(IStripePurchasePlanRepository stripePurchasePlanRepository, IUnitOfWork unitOfWork, IAuthService authService)
        {
            _stripePurchasePlanRepository = stripePurchasePlanRepository;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }
        public async Task<CreateStripePurchaseCommandResponse> Handle(CreateStripePurchaseCommand request, CancellationToken cancellationToken)
        {
            PurchaseHistory purchaseHistory = new PurchaseHistory()
            {
                UserId = request.UserId,
                Currency = request.Currency,
                Price = request.Price,
                PlanName = request.PlanName,
                PlanPurchaseType = request.PlanPurchaseType,
                Interval = request.Interval,
                StripePriceId = request.StripePriceId
            };

            await _stripePurchasePlanRepository.AddAsync(purchaseHistory);
            await _unitOfWork.CompleteAsync();
            var token = await _authService.UpgradeUserRoleAsync(request.UserId, request.PlanName, cancellationToken);
            return new CreateStripePurchaseCommandResponse() { IsSuccess = token == null ? false : true, Message = token == null ? "Error occoured in payment" : token };
        }
    }
}