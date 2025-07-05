using System.Diagnostics.CodeAnalysis;

using ByGameApi.Domain.Dao;
using ByGameApi.Infrastructure.Abstractions;

using Microsoft.Extensions.Logging;

namespace ByGameApi.Infrastructure.DataAccess;

[ExcludeFromCodeCoverage]
public class DbCommandExecutor : IDbCommandExecutor
{
    /// <summary>
    /// The <see cref="ILogger"/> for <see cref="DbCommandExecutor"/>
    /// </summary>
    private readonly ILogger<DbCommandExecutor> _logger;

    /// <summary>
    /// The sql connection factory
    /// </summary>
    private readonly IMySqlConnectionFactory _connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbCommandExecutor"/> class,
    /// used to execute database commands through a provided MySQL connection factory.
    /// </summary>
    /// <param name="logger">The logger used to log database operations and errors.</param>
    /// <param name="connectionFactory">The factory responsible for creating MySQL database connections.</param>
    public DbCommandExecutor(ILogger<DbCommandExecutor> logger, IMySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScoreDao>> ExecuteReaderAsync(string query)
    {
        var connection = _connectionFactory.CreateConnection();
        var results = new List<ScoreDao>();

        try
        {
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new ScoreDao
                {
                    ScoreId = reader.GetInt32("ScoreId"),
                    PlayerName = reader.GetString("PlayerName"),
                    Value = reader.GetInt32("Value")
                });
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "[DB ERROR]: ExecuteReaderAsync");
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<bool> ExecuteChangesAsync(string query, ScoreDao parameters)
    {
        var connection = _connectionFactory.CreateConnection();
        var affectedRows = 0;

        try
        {
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = query;

            var properties = typeof(ScoreDao).GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(parameters) ?? DBNull.Value;

                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = $"@{prop.Name}";
                dbParameter.Value = value;
                command.Parameters.Add(dbParameter);
            }

            affectedRows = await command.ExecuteNonQueryAsync();
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "[DB ERROR: ExecuteUpdaterAsync]");
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }

        return affectedRows > 0;
    }
}