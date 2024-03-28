using FileSystemManagementApi.Models;

namespace FileSystemManagementApi.Repositories.Interfaces;

public interface IFileRepository: IRepository<FileEntity>
{
    public Task<FileEntity> GetByIdAsync(int id);
}