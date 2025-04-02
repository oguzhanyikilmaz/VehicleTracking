using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Application.Services;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;

namespace VehicleTracking.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            ITenantRepository tenantRepository,
            ITokenService tokenService,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _tenantRepository = tenantRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            
            if (user == null)
            {
                throw new UnauthorizedAccessException("Kullanıcı adı veya şifre hatalı");
            }
            
            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Hesabınız devre dışı bırakılmış. Lütfen yöneticinize başvurun");
            }
            
            if (!_passwordService.VerifyPassword(user.PasswordHash, loginDto.Password))
            {
                throw new UnauthorizedAccessException("Kullanıcı adı veya şifre hatalı");
            }
            
            // Kullanıcının rollerini getir
            var roleIds = user.RoleIds;
            var roles = await _roleRepository.GetRolesByIdsAsync(roleIds);
            var roleNames = roles.Select(r => r.Name).ToList();
            
            // Kullanıcının izinlerini getir
            var permissionIds = roles.SelectMany(r => r.PermissionIds).Distinct().ToList();
            var permissions = await _permissionRepository.GetPermissionsByIdsAsync(permissionIds);
            var permissionNames = permissions.Select(p => p.Name).ToList();
            
            // Tenant bilgisini getir
            Tenant tenant = null;
            string tenantName = "";
            if (!string.IsNullOrEmpty(user.TenantId))
            {
                tenant = await _tenantRepository.GetByIdAsync(user.TenantId);
                if (tenant != null)
                {
                    tenantName = tenant.Name;
                }
            }
            
            // Refresh token oluştur
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryTime = _tokenService.CalculateRefreshTokenExpiryTime();
            
            // Kullanıcı bilgilerini güncelle
            await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiryTime);
            await _userRepository.UpdateLastLoginTimeAsync(user.Id);
            
            // User DTO oluştur
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                TenantId = user.TenantId,
                TenantName = tenantName,
                Language = user.Language,
                Theme = user.Theme,
                RoleIds = user.RoleIds,
                Roles = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive
                }).ToList()
            };
            
            // Token yanıtı oluştur
            var tokenResponse = new TokenResponseDto
            {
                AccessToken = _tokenService.GenerateAccessToken(user, roleNames, permissionNames),
                RefreshToken = refreshToken,
                ExpiresIn = 60 * 60, // 1 saat (saniye cinsinden)
                User = userDto
            };
            
            return tokenResponse;
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedAccessException("Geçersiz refresh token");
            }
            
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token süresi dolmuş veya geçersiz");
            }
            
            // Kullanıcının rollerini getir
            var roleIds = user.RoleIds;
            var roles = await _roleRepository.GetRolesByIdsAsync(roleIds);
            var roleNames = roles.Select(r => r.Name).ToList();
            
            // Kullanıcının izinlerini getir
            var permissionIds = roles.SelectMany(r => r.PermissionIds).Distinct().ToList();
            var permissions = await _permissionRepository.GetPermissionsByIdsAsync(permissionIds);
            var permissionNames = permissions.Select(p => p.Name).ToList();
            
            // Tenant bilgisini getir
            Tenant tenant = null;
            string tenantName = "";
            if (!string.IsNullOrEmpty(user.TenantId))
            {
                tenant = await _tenantRepository.GetByIdAsync(user.TenantId);
                if (tenant != null)
                {
                    tenantName = tenant.Name;
                }
            }
            
            // Yeni refresh token oluştur
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryTime = _tokenService.CalculateRefreshTokenExpiryTime();
            
            // Kullanıcı bilgilerini güncelle
            await _userRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken, refreshTokenExpiryTime);
            
            // User DTO oluştur
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                TenantId = user.TenantId,
                TenantName = tenantName,
                Language = user.Language,
                Theme = user.Theme,
                RoleIds = user.RoleIds,
                Roles = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive
                }).ToList()
            };
            
            // Token yanıtı oluştur
            var tokenResponse = new TokenResponseDto
            {
                AccessToken = _tokenService.GenerateAccessToken(user, roleNames, permissionNames),
                RefreshToken = newRefreshToken,
                ExpiresIn = 60 * 60, // 1 saat (saniye cinsinden)
                User = userDto
            };
            
            return tokenResponse;
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }
            
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            
            if (user == null)
            {
                return false;
            }
            
            return await _userRepository.RevokeRefreshTokenAsync(user.Id);
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            // Kullanıcı adı ve email kontrolü
            if (await _userRepository.ExistsByUsernameAsync(registerDto.Username))
            {
                throw new InvalidOperationException("Bu kullanıcı adı zaten kullanılıyor");
            }
            
            if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
            {
                throw new InvalidOperationException("Bu e-posta adresi zaten kullanılıyor");
            }
            
            // Tenant kontrolü
            if (!string.IsNullOrEmpty(registerDto.TenantId) && !await _tenantRepository.ExistsByIdAsync(registerDto.TenantId))
            {
                throw new KeyNotFoundException($"Kiracı bulunamadı (ID: {registerDto.TenantId})");
            }
            
            // Şifre kontrolü
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new InvalidOperationException("Şifreler eşleşmiyor");
            }
            
            // Yeni kullanıcı oluştur
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                TenantId = registerDto.TenantId,
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };
            
            // Varsayılan "User" rolünü bul ve ata
            var defaultUserRole = await _roleRepository.GetByNameAsync("User");
            if (defaultUserRole != null)
            {
                user.RoleIds.Add(defaultUserRole.Id);
            }
            
            // Kullanıcıyı veritabanına ekle
            var createdUser = await _userRepository.AddAsync(user);
            
            // User DTO oluştur
            var userDto = new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                PhoneNumber = createdUser.PhoneNumber,
                IsActive = createdUser.IsActive,
                LastLoginAt = createdUser.LastLoginAt,
                CreatedAt = createdUser.CreatedAt,
                UpdatedAt = createdUser.UpdatedAt,
                TenantId = createdUser.TenantId,
                RoleIds = createdUser.RoleIds
            };
            
            // Tenant adını ekle
            if (!string.IsNullOrEmpty(createdUser.TenantId))
            {
                var tenant = await _tenantRepository.GetByIdAsync(createdUser.TenantId);
                if (tenant != null)
                {
                    userDto.TenantName = tenant.Name;
                }
            }
            
            return userDto;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            
            if (user == null)
            {
                return false;
            }
            
            // Burada şifre sıfırlama e-postası gönderme işlemi yapılabilir
            // Şimdilik sadece kullanıcı var mı kontrolü yapıyoruz
            
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                throw new InvalidOperationException("Şifreler eşleşmiyor");
            }
            
            var user = await _userRepository.GetByIdAsync(resetPasswordDto.UserId);
            
            if (user == null)
            {
                return false;
            }
            
            // Normalde şifre sıfırlama token kontrolü yapılır
            // Şimdilik token kontrolünü atlıyoruz
            
            var passwordHash = _passwordService.HashPassword(resetPasswordDto.NewPassword);
            return await _userRepository.UpdatePasswordAsync(user.Id, passwordHash);
        }

        public async Task<bool> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }
            
            // Normalde token doğrulaması yapılır
            // Şimdilik token kontrolünü atlıyoruz
            
            return await _userRepository.ConfirmEmailAsync(user.Id);
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                throw new InvalidOperationException("Şifreler eşleşmiyor");
            }
            
            var user = await _userRepository.GetByIdAsync(changePasswordDto.UserId);
            
            if (user == null)
            {
                return false;
            }
            
            if (!_passwordService.VerifyPassword(user.PasswordHash, changePasswordDto.CurrentPassword))
            {
                throw new UnauthorizedAccessException("Mevcut şifre hatalı");
            }
            
            var passwordHash = _passwordService.HashPassword(changePasswordDto.NewPassword);
            return await _userRepository.UpdatePasswordAsync(user.Id, passwordHash);
        }
    }
} 