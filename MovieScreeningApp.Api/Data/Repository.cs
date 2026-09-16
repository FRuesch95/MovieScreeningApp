using Microsoft.EntityFrameworkCore;
using MovieScreeningApp.Api.Entities;
using MovieScreeningApp.Api.Interfaces;

namespace MovieScreeningApp.Api.Data;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    
    private readonly AppDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }
    
    public IQueryable<T> Query()
    {
        return _dbSet;
    }
    
    public IQueryable<T> ReadonlyQuery()
    {
        return _dbSet.AsNoTracking();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
    
}
