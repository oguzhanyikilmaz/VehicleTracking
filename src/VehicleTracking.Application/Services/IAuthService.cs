using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface IAuthService
    {
        Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<UserDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> VerifyEmailAsync(string userId, string token);
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    }
} 