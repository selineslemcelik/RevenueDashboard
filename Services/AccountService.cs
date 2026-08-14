using RevenueDashboard.Infrastructure;
using RevenueDashboard.Models.Entities;
using RevenueDashboard.Repositories;

namespace RevenueDashboard.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;

    public AccountService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        var existing = await _userRepository.GetByUsernameAsync(username);
        if (existing != null)
        {
            return false;
        }

        var user = new User
        {
            Username = username,
            PasswordHash = PasswordHasher.Hash(password),
            Role = "User"
        };

        await _userRepository.CreateAsync(user);
        return true;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return null;
        }

        if (!PasswordHasher.Verify(password, user.PasswordHash))
        {
            return null;
        }

        return user;
    }
}