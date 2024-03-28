using Microsoft.AspNetCore.Identity;
using FileSystemManagementApi.Models;

namespace FileSystemManagementApi.Services.Interfaces;

public interface IUserService : IService<User>
{
    public Task<IEnumerable<string>> GetAllUserRolesAsync(User user);
    
    Task<User> GetByIdAsync(string id);
}