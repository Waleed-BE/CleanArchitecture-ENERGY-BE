using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.Features.Auth.Commands;
using CleanArchitecture.Domain.Entities.Auth;
using CleanArchitecture.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CleanArchitecture.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IExpenseTypeRepository _expenseTypeRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IExpenseTypeRepository expenseTypeRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
            _expenseTypeRepository = expenseTypeRepository;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            string Message = "";
            if (user == null)
            {
                Message = "Email not found.";
                return new AuthResponse() { IsSuccessfull = false, ResponseMessage = Message };
            }

            // Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                Message = "Email is not confirmed. Please check your email for the confirmation link.";
                return new AuthResponse() { IsSuccessfull = false, ResponseMessage = Message };
            }

            // Check if account is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                Message = "Your account is locked. Please try again later or contact support.";
                return new AuthResponse() { IsSuccessfull = false, ResponseMessage = Message };
            }

            // Check if account is active
            if (user.Status != StatusEnum.Active)
            {
                Message = "Your account is not active. Please contact support.";
                return new AuthResponse() { IsSuccessfull = false, ResponseMessage = Message };

            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    Message = "Your account is locked. Please try again later or contact support.";
                    return new AuthResponse() { IsSuccessfull = false, ResponseMessage = Message };
                }
                return new AuthResponse() { IsSuccessfull = false, ResponseMessage = "Email or password does not match."};
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtToken(user, userRoles);

            return new AuthResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.UserName,
                Roles = userRoles.ToList(),
                IsVerified = user.EmailConfirmed,
                Token = token.Token,
                ExpiryDate = token.ExpiryDate,
                IsSuccessfull = true,
                ResponseMessage = "Login Successfull"
            };
        }

        private async Task<(string Token, DateTime ExpiryDate)> GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var expenseTypes = await _expenseTypeRepository.getUserExpenseTypes(Guid.Parse(user.Id));
            var expenseTypesJson = JsonSerializer.Serialize(expenseTypes);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim("UserId", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("StripeCustomerId", user.StripeCustomerId ?? ""),
                new Claim("ExpenseTypes", expenseTypesJson)
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

    }
}
