using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.Features.Auth.Commands;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Entities.Auth;
using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Security.Claims;
using System.Web;

namespace CleanArchitecture.Application.Features.Auth.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegistrationResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IExpenseTypeRepository _expenseTypeRepository;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IEmailService emailService,
            IConfiguration configuration,
            IExpenseTypeRepository expenseTypeRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
            _expenseTypeRepository = expenseTypeRepository;
        }

        public async Task<RegistrationResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            var existingUserName = await _userManager.FindByNameAsync(request.UserName);

            if (existingUserName != null)
            {
                return new RegistrationResponse
                {
                    IsSuccessful = false,
                    Message = "Username already exists"
                };
            }

            if (existingUser != null)
            {
                return new RegistrationResponse
                {
                    IsSuccessful = false,
                    Message = "Email already exists"
                };
            }

            StripeConfiguration.ApiKey = _configuration["Stripe:APIKey"];

            var options = new CustomerCreateOptions
            {
                Email = request.Email,
                Name = request.UserName,
            };

            var service = new CustomerService();
            var customer = await service.CreateAsync(options);

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = "",
                LastName = "",
                NormalizedUserName = request.UserName,
                UserName = request.UserName,
                Status = StatusEnum.Pending,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System",
                EmailConfirmed = false,
                StripeCustomerId = customer.Id 
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return new RegistrationResponse
                {
                    IsSuccessful = false,
                    Message = result.Errors.Select(e => e.Description).FirstOrDefault()
                };
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "User",
                    Description = "Regular user role",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                });
            }

            await _userManager.AddToRoleAsync(user, "User");
            await _expenseTypeRepository.addExpenseTypeOnRegister(Guid.Parse(user.Id));

            // Generate Email Confirmation Token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmationLink = $"{_configuration["AppUrl"]}/{_configuration["ConfirmEmail"]}?userId={user.Id}&token={encodedToken}";

            var emailMessage = new EmailMessage
            {
                Subject = "Confirm your email",
                Body = $@"<html>
                        <body>
                            <h2>Welcome to {_configuration["AppName"]}!</h2>
                            <p>Please confirm your email by clicking the link below:</p>
                            <p><a href='{confirmationLink}'>Confirm your email</a></p>
                        </body>
                    </html>",
                To = new List<string> { user.Email }
            };

            await _emailService.SendEmailAsync(emailMessage);

            return new RegistrationResponse
            {
                IsSuccessful = true,
                UserId = user.Id,
                Message = "User registered successfully. Please check your email to confirm your account.",
                Url = confirmationLink                
            };
        }
    }
}
