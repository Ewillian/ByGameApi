using MySqlConnector;

namespace ByGameApi.Infrastructure.Abstractions;
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates the connection using a connection string and options from config
    /// </summary>
    /// <returns>A MySqlConnection</returns>
    MySqlConnection CreateConnection();
}
