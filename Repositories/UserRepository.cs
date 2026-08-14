using Npgsql;
using RevenueDashboard.Infrastructure;
using RevenueDashboard.Models.Entities;

namespace RevenueDashboard.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        const string sql = @"
            SELECT id, username, password_hash, role, created_at
            FROM users
            WHERE username = @username;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("username", username);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            Role = reader.GetString(3),
            CreatedAt = reader.GetDateTime(4)
        };
    }

    public async Task CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO users (username, password_hash, role)
            VALUES (@username, @passwordHash, @role);";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("username", user.Username);
        command.Parameters.AddWithValue("passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("role", user.Role);

        await command.ExecuteNonQueryAsync();
    }
}