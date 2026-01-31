using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.API.DTOs;
using PCM.API.Services;
using System.Security.Claims;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        if (result == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);
        if (result == null)
            return BadRequest(new { message = "User already exists or registration failed" });

        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var userInfo = await _authService.GetCurrentUserInfoAsync(userId);
        if (userInfo == null)
            return NotFound();

        return Ok(userInfo);
    }
}
