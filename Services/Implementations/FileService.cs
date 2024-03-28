using FileSystemManagementApi.Models;
using FileSystemManagementApi.Repositories.Interfaces;
using FileSystemManagementApi.Services.Interfaces;

namespace FileSystemManagementApi.Services.Implementations;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;

    public FileService(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<IEnumerable<FileEntity>> GetAllAsync()
    {
        return await _fileRepository.GetAllAsync();
    }

    public async Task AddAsync(FileEntity entity)
    {
        await _fileRepository.AddAsync(entity); 
    }

    public async Task UpdateAsync(FileEntity entity)
    {
        _fileRepository.Update(entity);
        await _fileRepository.SaveChangesAsync();
    }

    public Task DeleteAsync(FileEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task<FileEntity> GetByIdAsync(int id)
    {
        return await _fileRepository.GetByIdAsync(id);
    }
}