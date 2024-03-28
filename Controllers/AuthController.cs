using FileSystemManagementApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FileSystemManagementApi.DTOs;
using FileSystemManagementApi.Services.Interfaces;

namespace FileSystemManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtManagerService _jwtManagerService;

    public AuthController(
        UserManager<User> userManager,
        IJwtManagerService jwtManagerService
    )
    {
        _userManager = userManager;
        _jwtManagerService = jwtManagerService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var userExists = await _userManager.FindByEmailAsync(loginDto.EmailAddress);

        if (userExists == null || !await _userManager.CheckPasswordAsync(userExists, loginDto.Password))
            return Unauthorized();

        var tokenValue = await _jwtManagerService.GenerateJWTTokenAsync(userExists, null);

        return Ok(tokenValue);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
    {
        var result = await _jwtManagerService.VerifyAndGenerateTokenAsync(tokenDto);
        return Ok(result);
    }
}