using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Application.Models;
using VehicleTracking.Application.Services;

namespace VehicleTracking.API.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                {
                    return BadRequest("Giriş bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(loginDto.Username))
                {
                    return BadRequest("Kullanıcı adı boş olamaz");
                }

                if (string.IsNullOrEmpty(loginDto.Password))
                {
                    return BadRequest("Şifre boş olamaz");
                }

                var tokenResponse = await _authService.LoginAsync(loginDto);
                return Ok(tokenResponse, "Giriş başarılı");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (registerDto == null)
                {
                    return BadRequest("Kayıt bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(registerDto.Username))
                {
                    return BadRequest("Kullanıcı adı boş olamaz");
                }

                if (string.IsNullOrEmpty(registerDto.Email))
                {
                    return BadRequest("E-posta adresi boş olamaz");
                }

                if (string.IsNullOrEmpty(registerDto.Password))
                {
                    return BadRequest("Şifre boş olamaz");
                }

                var user = await _authService.RegisterAsync(registerDto);
                return Ok(user, "Kullanıcı başarıyla kaydedildi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        {
            try
            {
                if (refreshTokenRequest == null)
                {
                    return BadRequest("Token bilgisi boş olamaz");
                }

                if (string.IsNullOrEmpty(refreshTokenRequest.RefreshToken))
                {
                    return BadRequest("Yenileme token'ı boş olamaz");
                }

                var tokenResponse = await _authService.RefreshTokenAsync(refreshTokenRequest.RefreshToken);
                return Ok(tokenResponse, "Token başarıyla yenilendi");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (changePasswordDto == null)
                {
                    return BadRequest("Şifre değiştirme bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(changePasswordDto.Id))
                {
                    return BadRequest("Kullanıcı ID boş olamaz");
                }

                if (string.IsNullOrEmpty(changePasswordDto.CurrentPassword))
                {
                    return BadRequest("Mevcut şifre boş olamaz");
                }

                if (string.IsNullOrEmpty(changePasswordDto.NewPassword))
                {
                    return BadRequest("Yeni şifre boş olamaz");
                }

                var result = await _authService.ChangePasswordAsync(changePasswordDto);
                if (!result)
                {
                    return BadRequest("Şifre değiştirilemedi");
                }

                return Ok("Şifre başarıyla değiştirildi");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (resetPasswordDto == null)
                {
                    return BadRequest("Şifre sıfırlama bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(resetPasswordDto.UserId))
                {
                    return BadRequest("Kullanıcı ID boş olamaz");
                }

                if (string.IsNullOrEmpty(resetPasswordDto.Token))
                {
                    return BadRequest("Sıfırlama token'ı boş olamaz");
                }

                if (string.IsNullOrEmpty(resetPasswordDto.NewPassword))
                {
                    return BadRequest("Yeni şifre boş olamaz");
                }

                var result = await _authService.ResetPasswordAsync(resetPasswordDto);
                if (!result)
                {
                    return BadRequest("Şifre sıfırlanamadı");
                }

                return Ok("Şifreniz başarıyla sıfırlandı");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Kullanıcı kimliği alınamadı");
                }

                var result = await _authService.LogoutAsync(userId);
                if (!result)
                {
                    return BadRequest("Çıkış yapılamadı");
                }

                return Ok("Başarıyla çıkış yapıldı");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}