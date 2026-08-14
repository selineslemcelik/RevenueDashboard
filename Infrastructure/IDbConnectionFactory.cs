using Npgsql;

namespace RevenueDashboard.Infrastructure;

public interface IDbConnectionFactory
{
    NpgsqlConnection CreateConnection();
}