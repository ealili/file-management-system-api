namespace FileSystemManagementApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required] public required string EmailAddress { get; set; }
    [Required] public required string Password { get; set; }
}