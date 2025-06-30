using TaskManager.Domain.Common;

namespace TaskManager.Application.Contracts.Persistance.Common;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAsync();

    Task<T> GetByIdAsync(Guid id);

    Task CreateAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}
