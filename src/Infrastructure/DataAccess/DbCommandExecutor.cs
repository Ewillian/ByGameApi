using System.Diagnostics.CodeAnalysis;

using ByGameApi.Domain.Dao;
using ByGameApi.Infrastructure.Abstractions;

using Microsoft.Extensions.Logging;

namespace ByGameApi.Infrastructure.DataAccess;

[ExcludeFromCodeCoverage]
public class DbCommandExecutor : IDbCommandExecutor
{
    /// <summary>
    /// The sql connection factory
    /// </summary>
    private readonly IDbConnectionFactory _connectionFactory;

    /// <summary>
    /// 
    /// </summary>
    private readonly ILogger<DbCommandExecutor> _logger;

    public DbCommandExecutor(IDbConnectionFactory connectionFactory, ILogger<DbCommandExecutor> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

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
            _logger.LogError($"[DB ERROR] {ex.Message}");
            throw;
        }
        finally
        {
            connection.Close();
        }

        return results;
    }
}