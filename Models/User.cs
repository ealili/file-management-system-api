using Microsoft.AspNetCore.Identity;

namespace FileSystemManagementApi.Models;

public class User : IdentityUser
{
    public required string FirstName { get; set; } 
    public required string LastName { get; set; }

    public List<string>? Roles { get; set; }
}