using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Contracts.Persistance;
using TaskManager.Domain;
using TaskManager.Persistence.DatabaseContext;
using TaskManager.Persistence.Repositories.Common;

namespace TaskManager.Persistence.Repositories;

public class UserRepository(TaskManagerDbContext dbContext)
    : GenericRepository<User>(dbContext), IUserRepository
{
    public async Task<bool> IsUserUniqueAsync(string name)
    {
        return await dbContext.Users.AsNoTracking().AnyAsync(x => x.Name == name);
    }
}
