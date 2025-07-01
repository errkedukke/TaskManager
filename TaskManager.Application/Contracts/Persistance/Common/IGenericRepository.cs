using TaskManager.Domain.Common;

namespace TaskManager.Application.Contracts.Persistance.Common;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<T>> GetAsync(CancellationToken cancellationToken);

    Task CreateAsync(T entity, CancellationToken cancellationToken);

    Task UpdateAsync(T entity, CancellationToken cancellationToken);

    Task DeleteAsync(T entity, CancellationToken cancellationToken);
}
