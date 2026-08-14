using RevenueDashboard.Models.Entities;

namespace RevenueDashboard.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task CreateAsync(User user);
}