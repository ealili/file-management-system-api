using System.ComponentModel.DataAnnotations;

namespace FileSystemManagementApi.DTOs
{
    public class AuthDto
    {
        [Required] public required string Token { get; set; }

        [Required] public required string RefreshToken { get; set; }

        public required DateTime ExpiresAt { get; set; }
    }
}