using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FileSystemManagementApi.Data;
using FileSystemManagementApi.Exceptions.User;
using FileSystemManagementApi.Models;
using FileSystemManagementApi.Repositories.Interfaces;

namespace FileSystemManagementApi.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = await _context.Users
            .ToListAsync();

        return users;
    }

    public async Task<User> GetByIdAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            throw new UserNotFoundException();
        }
        return user;
    }

    public Task AddAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public void Update(User entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(User entity)
    {
        throw new NotImplementedException();
    }
}