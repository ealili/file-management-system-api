namespace FileSystemManagementApi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable
public class RefreshToken
{
    [Key] public int Id { get; set; }

    public required string Token { get; set; }
    public required string JwtId { get; set; }
    public required bool IsRevoked { get; set; }
    public required DateTime DateAdded { get; set; }
    public required DateTime DateExpired { get; set; }

    public required string UserId { get; set; }

    [ForeignKey(nameof(UserId))] // Not necessary
    public User User { get; set; }
}