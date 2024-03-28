namespace FileSystemManagementApi.Exceptions.User;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException() : base("User already exists.")
    {
    }
}