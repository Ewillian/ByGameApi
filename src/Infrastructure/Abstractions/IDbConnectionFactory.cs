using MySqlConnector;

namespace ByGameApi.Infrastructure.Abstractions;

/// <summary>
/// Defines a contract for creating configured <see cref="MySqlConnection"/> instances
/// using options provided by the application configuration.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates and returns a new <see cref="MySqlConnection"/> instance using
    /// the configured connection string and database options.
    /// </summary>
    /// <returns>
    /// A new <see cref="MySqlConnection"/> instance. The connection is not opened automatically.
    /// </returns>
    MySqlConnection CreateConnection();
}
