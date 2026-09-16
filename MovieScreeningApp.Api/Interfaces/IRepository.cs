using MovieScreeningApp.Api.Entities;

namespace MovieScreeningApp.Api.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Query();
    IQueryable<T> ReadonlyQuery();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync();
}
