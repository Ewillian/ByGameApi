using ByGameApi.Infrastructure.Abstractions;
using ByGameApi.Infrastructure.Options;

using Microsoft.Extensions.Options;

using MySqlConnector;

namespace ByGameApi.Infrastructure.Factories;

/// <summary>
/// Factory responsible for creating and configuring <see cref="MySqlConnection"/> instances
/// based on the provided <see cref="DatabaseOptions"/>.
/// </summary>
public class MySqlConnectionFactory : IMySqlConnectionFactory
{
    /// <summary>
    /// Configuration options for database access.
    /// </summary>
    private readonly DatabaseOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlConnectionFactory"/> class.
    /// </summary>
    /// <param name="options">The database options provided via dependency injection.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
    public MySqlConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public MySqlConnection CreateConnection()
    {
        return new MySqlConnection($"Server={_options.Server};Port={_options.Port};Database={_options.Database};User={_options.User};Password={_options.Password};ConnectionTimeout={_options.ConnectionTimeout};DefaultCommandTimeout={_options.CommandTimeout};MaxPoolSize={_options.MaxConnectionPoolSize};MinPoolSize={_options.MinConnectionPoolSize};Pooling={_options.IsPooling};");
    }
}
