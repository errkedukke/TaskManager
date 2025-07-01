using TaskManager.Application.Contracts.Persistance.Common;
using TaskManager.Domain;

namespace TaskManager.Application.Contracts.Persistance;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> IsUserUniqueAsync(string name);
}
