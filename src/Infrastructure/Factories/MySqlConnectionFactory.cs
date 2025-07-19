using ByGameApi.Infrastructure.Abstractions;
using ByGameApi.Infrastructure.Options;

using Microsoft.Extensions.Options;

using MySqlConnector;

namespace ByGameApi.Infrastructure.Factories;

public class MySqlConnectionFactory : IMySqlConnectionFactory
{
    private readonly DatabaseOptions _options;

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
