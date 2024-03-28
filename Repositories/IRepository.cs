namespace FileSystemManagementApi.Repositories;

public interface IRepository<TEntity>
{
    public Task SaveChangesAsync();
    
    public Task<IEnumerable<TEntity>> GetAllAsync();
    
    public Task AddAsync(TEntity entity);
    
    public void Update(TEntity entity);
    
    public Task Delete(TEntity entity);
}