using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Entities.Auth;
using CleanArchitecture.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web;

namespace CleanArchitecture.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IExpenseTypeRepository _expenseTypeRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IEmailService emailService,
            IExpenseTypeRepository expenseTypeRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
            _expenseTypeRepository = expenseTypeRepository;
        }

        public async Task<RegistrationResponse> RegisterAsync(RegisterRequest request)
        {
            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                return new RegistrationResponse
                {
                    IsSuccessful = false,
                    Message = "Email already exists"
                };
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Status = StatusEnum.Pending,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System",
                EmailConfirmed = false
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

            // Ensure the User role exists
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

         

            // Generate Email Confirmation Token and send email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmationLink = $"{_configuration["AppUrl"]}/confirm-email?userId={user.Id}&token={encodedToken}";
            var confirmationLinkWithoutCreds = $"{_configuration["AppUrl"]}/confirm-email?userId=&token=";
            var emailMessage = new EmailMessage
            {
                Subject = "Confirm your email",
                Body = $@"<html>
                        <body>
                            <h2>Welcome to {_configuration["AppName"]}!</h2>
                            <p>Thanks for registering. Please confirm your email by clicking the link below:</p>
                            <p><a href='{confirmationLink}'>Confirm your email</a></p>
                            <p>If you didn't register for this account, please ignore this email.</p>
                        </body>
                    </html>",
                To = new List<string> { user.Email }
            };

            //await _emailService.SendEmailAsync(emailMessage);

            return new RegistrationResponse
            {
                IsSuccessful = true,
                UserId = user.Id,
                ConfirmationLink = confirmationLink,
                ConfirmationToken = encodedToken,
                ConfirmationLinkWithoutCreds = confirmationLinkWithoutCreds,
                Message = "User registered successfully. Please check your email to confirm your account."
            };
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Update user status to Active once email is confirmed
                user.Status = StatusEnum.Active;
                await _userManager.UpdateAsync(user);
                return true;
            }

            return false;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return null;
            }

            // Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new Exception("Email not confirmed. Please check your email for the confirmation link.");
            }

            // Check if account is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                throw new Exception("Your account is locked. Please try again later or contact support.");
            }

            // Check if account is active
            if (user.Status != StatusEnum.Active)
            {
                throw new Exception("Your account is not active. Please contact support.");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    throw new Exception("Your account is locked. Please try again later or contact support.");
                }
                return null;
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
                ExpiryDate = token.ExpiryDate
            };
        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return new ForgotPasswordResponse
                {
                    IsSuccessful = true,
                    Message = "If your email is registered with us, you will receive a password reset link."
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var resetLink = $"{_configuration["AppUrl"]}/reset-password?email={HttpUtility.UrlEncode(user.Email)}&token={encodedToken}";

            var emailMessage = new EmailMessage
            {
                Subject = "Reset your password",
                Body = $@"<html>
                        <body>
                            <h2>Reset Your Password</h2>
                            <p>You've requested to reset your password. Please click the link below to set a new password:</p>
                            <p><a href='{resetLink}'>Reset your password</a></p>
                            <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>
                            <p>This link is valid for 24 hours.</p>
                        </body>
                    </html>",
                To = new List<string> { user.Email }
            };

            await _emailService.SendEmailAsync(emailMessage);

            return new ForgotPasswordResponse
            {
                IsSuccessful = true,
                Message = "If your email is registered with us, you will receive a password reset link."
            };
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new ResetPasswordResponse
                {
                    IsSuccessful = false,
                    Message = "Error resetting password."
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (!result.Succeeded)
            {
                return new ResetPasswordResponse
                {
                    IsSuccessful = false,
                    Message = result.Errors.Select(e => e.Description).FirstOrDefault()
                };
            }

            // If user is locked out, unlock them after successful password reset
            if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            }

            return new ResetPasswordResponse
            {
                IsSuccessful = true,
                Message = "Password has been reset successfully."
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
        public async Task<string> UpgradeUserRoleAsync(string userId, string planName, CancellationToken cancellationToken)
        { 
            // 1. Check if role exists
            var roleExists = await _roleManager.RoleExistsAsync(planName);
            if (!roleExists)
            {
                return null;
            }

            // 2. Get user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            // 3. Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // 4. Remove existing roles
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return null;
                }
            }

            // 5. Add new role
            var addResult = await _userManager.AddToRoleAsync(user, planName);

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtToken(user, userRoles);
            if (!addResult.Succeeded)
            {
                return null;
            }

            return token.Token;
        }
    }
}