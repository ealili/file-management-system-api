using FileSystemManagementApi.Models;

namespace FileSystemManagementApi.Services.Interfaces;

public interface IFileService: IService<FileEntity>
{
    Task<FileEntity> GetByIdAsync(int id);
}