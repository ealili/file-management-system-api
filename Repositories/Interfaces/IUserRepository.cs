using FileSystemManagementApi.DTOs;
using FileSystemManagementApi.Models;

namespace FileSystemManagementApi.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    public Task<User> GetByIdAsync(string id);
}