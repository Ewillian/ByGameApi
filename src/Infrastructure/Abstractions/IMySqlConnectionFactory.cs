using MySqlConnector;

/// <summary>
/// Defines a factory interface for creating configured <see cref="MySqlConnection"/> instances.
/// </summary>
namespace ByGameApi.Infrastructure.Abstractions;
public interface IMySqlConnectionFactory
{
    /// <summary>
    /// Creates and returns a new <see cref="MySqlConnection"/> using the configured connection string.
    /// </summary>
    /// <returns>
    /// A new instance of <see cref="MySqlConnection"/> ready to be opened and used.
    ///</returns>
    MySqlConnection CreateConnection();
}
