using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Contracts.Persistance.Common;
using TaskManager.Domain.Common;
using TaskManager.Persistence.DatabaseContext;

namespace TaskManager.Persistence.Repositories.Common;

public class GenericRepository<T>(TaskManagerDbContext dbContext)
    : IGenericRepository<T> where T : BaseEntity
{
    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        dbContext.Set<T>().Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return result!;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        dbContext.Update(entity);
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
