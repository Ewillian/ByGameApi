using Microsoft.Extensions.Options;

namespace ByGameApi.Infrastructure.Options;

public sealed class DatabaseOptions : IOptions<DatabaseOptions>
{
    /// <summary>
    /// The server.
    /// </summary>
    public required string Server { get; init; }

    /// <summary>
    /// The port.
    /// </summary>
    public required string Port { get; init; }

    /// <summary>
    /// The database.
    /// </summary>
    public required string Database { get; init; }

    /// <summary>
    /// The database.
    /// </summary>
    public required string User { get; init; }

    /// <summary>
    /// The database.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// The score sql table.
    /// </summary>
    public required string Table { get; init; }

    /// <summary>
    /// The min of sql connections per client.
    /// </summary>
    public int MinConnectionPoolSize { get; init; } = 100;

    /// <summary>
    /// The max of sql connections per client.
    /// </summary>
    public int MaxConnectionPoolSize { get; init; } = 1000;

    /// <summary>
    /// The maxTime MS of queries
    /// </summary>
    public int ConnectionTimeout { get; init; } = 10000;

    /// <summary>
    /// The sql get score query
    /// </summary>
    public string SqlQueryGet { get; init; } = "";

    /// <summary>
    /// Value
    /// </summary>
    DatabaseOptions IOptions<DatabaseOptions>.Value => this;
}
