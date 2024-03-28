namespace FileSystemManagementApi.DTOs;

using System.Collections.Generic;

public class UserDto
{
    public string Id { get; set; } = string.Empty;

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public required string UserName { get; set; }

    public List<string>? Roles { get; set; }
}