using System.Diagnostics.CodeAnalysis;

using ByGameApi.Domain.Dao;
using ByGameApi.Infrastructure.Abstractions;
using ByGameApi.Infrastructure.Exception;

using Microsoft.Extensions.Logging;

namespace ByGameApi.Infrastructure.DataAccess;

[ExcludeFromCodeCoverage]
public class DbCommandExecutor : IDbCommandExecutor
{
    /// <summary>
    /// The sql connection factory
    /// </summary>
    private readonly IMySqlConnectionFactory _connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbCommandExecutor"/> class,
    /// used to execute database commands through a provided MySQL connection factory.
    /// </summary>
    /// <param name="connectionFactory">The factory responsible for creating MySQL database connections.</param>
    public DbCommandExecutor(ILogger<DbCommandExecutor> logger, IMySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
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
            throw new DatabaseException($"[DB ERROR]: ExecuteReaderAsync {ex.Message} {ex.StackTrace}");
        }
        finally
        {
            await connection.CloseAsync();
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<bool> ExecuteChangesAsync(string query, Dictionary<string, object> parameters)
    {
        var connection = _connectionFactory.CreateConnection();
        var affectedRows = 0;

        try
        {
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = query;

            foreach (var param in parameters)
            {
                var dbParam = command.CreateParameter();
                dbParam.ParameterName = param.Key;
                dbParam.Value = param.Value ?? DBNull.Value;
                command.Parameters.Add(dbParam);
            }

            affectedRows = await command.ExecuteNonQueryAsync();
        }
        catch (System.Exception ex)
        {
            throw new DatabaseException($"[DB ERROR]: ExecuteChangesAsync {ex.Message} {ex.StackTrace}");
        }
        finally
        {
            await connection.CloseAsync();
        }

        return affectedRows > 0;
    }
}