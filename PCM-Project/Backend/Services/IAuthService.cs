using PCM.API.DTOs;

namespace PCM.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
    Task<UserInfoDto?> GetCurrentUserInfoAsync(string userId);
}
