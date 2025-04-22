using CleanArchitecture.Application.DTOs.Auth;

namespace CleanArchitecture.Application.Interfaces
{
    public interface IAuthService
    {
        Task<RegistrationResponse> RegisterAsync(RegisterRequest request);
        Task<string> UpgradeUserRoleAsync(string userId, string planName, CancellationToken cancellationToken);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(string email);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}