using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PCM.API.Data;
using PCM.API.DTOs;
using PCM.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PCM.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
            return null;

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == user.Id);

        var token = await GenerateJwtToken(user, roles.ToList());

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email ?? "",
            FullName = user.FullName ?? "",
            Roles = roles.ToList(),
            MemberId = member?.Id
        };
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
            return null;

        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
            return null;

        await _userManager.AddToRoleAsync(user, "Member");

        // Create Member record
        var member = new Member
        {
            UserId = user.Id,
            FullName = registerDto.FullName,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            DateOfBirth = registerDto.DateOfBirth,
            RankLevel = 3.0,
            JoinDate = DateTime.UtcNow,
            Status = MemberStatus.Active
        };

        _context.Members.Add(member);
        await _context.SaveChangesAsync();

        var roles = new List<string> { "Member" };
        var token = await GenerateJwtToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email ?? "",
            FullName = user.FullName ?? "",
            Roles = roles,
            MemberId = member.Id
        };
    }

    public async Task<UserInfoDto?> GetCurrentUserInfoAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

        return new UserInfoDto
        {
            UserId = user.Id,
            Email = user.Email ?? "",
            FullName = user.FullName ?? "",
            Roles = roles.ToList(),
            MemberId = member?.Id,
            RankLevel = member?.RankLevel
        };
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user, List<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForPCMProject2024MinLength32Characters";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.FullName ?? "")
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"] ?? "1440")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
