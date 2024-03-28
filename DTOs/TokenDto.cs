namespace FileSystemManagementApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class TokenDto
{
    [Required] public required string Token { get; set; }

    [Required] public required string RefreshToken { get; set; }
}