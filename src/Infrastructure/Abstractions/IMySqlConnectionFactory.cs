using MySqlConnector;

namespace ByGameApi.Infrastructure.Abstractions;
public interface IMySqlConnectionFactory
{
    /// <summary>
    /// Creates the connection using a connection string and options from config
    /// </summary>
    /// <returns>A MySqlConnection</returns>
    MySqlConnection CreateConnection();
}
