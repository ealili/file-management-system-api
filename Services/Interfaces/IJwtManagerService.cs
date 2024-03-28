using FileSystemManagementApi.DTOs;
using FileSystemManagementApi.Models;

namespace FileSystemManagementApi.Services.Interfaces;

public interface IJwtManagerService
{
    public Task<AuthDto> VerifyAndGenerateTokenAsync(TokenDto tokenDto);
    public Task<AuthDto> GenerateJWTTokenAsync(User user, RefreshToken rToken);
}