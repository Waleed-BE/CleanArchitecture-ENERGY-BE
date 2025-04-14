using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Entities.Business;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace CleanArchitecture.Application.Features.Business.ExpenseTypes.Commands
{
    public class CreateExpenseTypeResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class CreateExpenseTypeCommand : IRequest<CreateExpenseTypeResponse>
    {
        public string ExpenseName { get; set; }
        public string Description { get; set; }
        public Guid ExpenseCategoryId { get; set; }
        public string UnitOfMeasurement { get; set; }
    }

    public class CreateExpenseTypeCommandHandler : IRequestHandler<CreateExpenseTypeCommand, CreateExpenseTypeResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IExpenseTypeRepository _expenseTypeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;


        public CreateExpenseTypeCommandHandler(IUnitOfWork unitOfWork, IExpenseTypeRepository expenseTypeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _expenseTypeRepository = expenseTypeRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<CreateExpenseTypeResponse> Handle(CreateExpenseTypeCommand request, CancellationToken cancellationToken)
        {
            string loggedInUserId = "";
            var User = _httpContextAccessor.HttpContext.User;
            if (User.Identity.IsAuthenticated)
            {
                // Retrieve a specific claim (e.g., "Role", "UserId", etc.)
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                loggedInUserId = userId;

                var expenseTypeCount = await _expenseTypeRepository.getExpenseTypeByUserId(Guid.Parse(loggedInUserId));

                // Check if the claim exists and do something with it
                if (!string.IsNullOrEmpty(userRole))
                {
                    if(userRole.ToLower().Equals(_configuration["ROLES:User"].ToLower()) && expenseTypeCount == 4)
                    {
                        return new CreateExpenseTypeResponse() { IsSuccess = false, Message = "User cannot add other expense type as its a basic user."};
                    }


                    if (userRole.ToLower().Equals(_configuration["ROLES:Premium"].ToLower()) && expenseTypeCount == 7)
                    {
                        return new CreateExpenseTypeResponse() { IsSuccess = false, Message = "User cannot add other expense type as its a Premium user that can create 3 custom expense types." };
                    }
                }
            }

            // Check if expense name already exists
            var isUnique = await _expenseTypeRepository.IsExpenseNameUniqueAsync(request.ExpenseName, loggedInUserId);
            if (!isUnique)
            {
                return new CreateExpenseTypeResponse() { IsSuccess = false, Message = $"{request.ExpenseName} expense type is already present." };
            }

            var expenseType = new ExpenseType
            {
                ExpenseTypeId = Guid.NewGuid(),
                ExpenseName = request.ExpenseName,
                Description = request.Description,
                ExpenseCategoryId = request.ExpenseCategoryId,
                UnitOfMeasurement = request.UnitOfMeasurement,
                Status = Domain.Enums.StatusEnum.Active,
            };

            await _expenseTypeRepository.AddAsync(expenseType);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new CreateExpenseTypeResponse() { IsSuccess = true, Message = $"{expenseType.ExpenseName} expense type is created."};
            }

    }
}