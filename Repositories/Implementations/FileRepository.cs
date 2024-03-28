using FileSystemManagementApi.Data;
using FileSystemManagementApi.Models;
using FileSystemManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using FileNotFoundException = FileSystemManagementApi.Exceptions.File.FileNotFoundException;

namespace FileSystemManagementApi.Repositories.Implementations;

public class FileRepository : IFileRepository
{
    private readonly AppDbContext _context;

    public FileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<FileEntity> GetByIdAsync(int id)
    {
        var file = await _context.Files.FindAsync(id);

        if (file == null)
        {
            throw new FileNotFoundException();
        }

        return file;
    }

    public async Task<IEnumerable<FileEntity>> GetAllAsync()
    {
        var files = await _context.Files
            .OrderByDescending(f => f.CreationDate)
            .ToListAsync();

        return files;
    }

    public async Task AddAsync(FileEntity entity)
    {
        await _context.Files.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public void Update(FileEntity entity)
    {
        _context.Files.Update(entity);
    }

    public Task Delete(FileEntity entity)
    {
        throw new NotImplementedException();
    }
}