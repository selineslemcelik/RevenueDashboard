using RevenueDashboard.Models.Entities;

namespace RevenueDashboard.Services;

public interface IAccountService
{
    Task<bool> RegisterAsync(string username, string password);
    Task<User?> LoginAsync(string username, string password);
}