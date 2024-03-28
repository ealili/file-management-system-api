using FileSystemManagementApi.Infrastructure;

namespace FileSystemManagementApi.Exceptions.User;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException() : base("User not found.")
    {
    }
}