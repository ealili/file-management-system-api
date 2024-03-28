using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FileSystemManagementApi.Data;
using FileSystemManagementApi.DTOs;
using FileSystemManagementApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FileSystemManagementApi.Services.Interfaces;

namespace FileSystemManagementApi.Services.Implementations;

public class JwtManagerService : IJwtManagerService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JwtManagerService(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext context,
        IConfiguration configuration,
        TokenValidationParameters tokenValidationParameters
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _configuration = configuration;
        _tokenValidationParameters = tokenValidationParameters;
    }


    public async Task<AuthDto> VerifyAndGenerateTokenAsync(TokenDto tokenDto)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenDto.RefreshToken);
        var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);

        try
        {
            var tokenCheckResult =
                jwtTokenHandler.ValidateToken(tokenDto.Token, _tokenValidationParameters, out var validatedToken);
            return await GenerateJWTTokenAsync(dbUser, storedToken);
        }
        catch (SecurityTokenException)
        {
            if (storedToken.DateExpired >= DateTime.UtcNow)
            {
                return await GenerateJWTTokenAsync(dbUser, storedToken);
            }

            return await GenerateJWTTokenAsync(dbUser, null);
        }
    }

    public async Task<AuthDto> GenerateJWTTokenAsync(User user, RefreshToken rToken)
    {
        
        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Add user roles claim
        var userRoles = await _userManager.GetRolesAsync(user);
        
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }
        
        
        var jwtSecret = _configuration["JWT:Secret"] 
                        ?? throw new InvalidOperationException("Allowed Origins config is missing.");

        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(15),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);


        if (rToken != null)
        {
            var rTokenResponse = new AuthDto()
            {
                Token = jwtToken,
                ExpiresAt = token.ValidTo,
                RefreshToken = rToken.Token
            };
            return rTokenResponse;
        }
        
        var response = new AuthDto()
        {
            Token = jwtToken,
            ExpiresAt = token.ValidTo,
            RefreshToken = await GenerateRefreshToken(token.Id, user.Id)
        };

        return response;
    }

    private async Task<string> GenerateRefreshToken(string tokenId, string userId)
    {
        var refreshToken = new RefreshToken()
        {
            JwtId = tokenId,
            IsRevoked = false,
            UserId = userId,
            DateAdded = DateTime.UtcNow,
            DateExpired = DateTime.UtcNow.AddMonths(6),
            Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken.Token;
    }
}