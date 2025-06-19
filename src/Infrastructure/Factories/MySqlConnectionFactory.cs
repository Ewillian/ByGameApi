using ByGameApi.Infrastructure.Abstractions;
using ByGameApi.Infrastructure.Options;

using Microsoft.Extensions.Options;

using MySqlConnector;

namespace ByGameApi.Infrastructure.Factories;

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseOptions _options;

    public MySqlConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public MySqlConnection CreateConnection()
    {
        return new MySqlConnection($"Server={_options.Server};Port={_options.Port};Database={_options.Database};User={_options.User};Password={_options.Password}");
    }
}
