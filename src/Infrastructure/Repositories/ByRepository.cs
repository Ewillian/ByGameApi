using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;
using ByGameApi.Infrastructure.Exception;
using ByGameApi.Infrastructure.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySqlConnector;

namespace ByGameApi.Infrastructure.Repositories;

public class ByRepository : IByRepository
{
    /// <summary>
    /// 
    /// </summary>
    private readonly ILogger<ByRepository> _logger;

    /// <summary>
    /// The database options
    /// </summary>
    private readonly DatabaseOptions _options;

    /// <summary>
    /// The database options
    /// </summary>
    private readonly MySqlConnection _connection;

    public ByRepository(ILogger<ByRepository> logger, IOptions<DatabaseOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _connection = new MySqlConnection($"Server={_options.Server};Port={_options.Port};Database={_options.Database};User={_options.User};Password={_options.Password}");
    }

    /// <inheritdoc />
    public async Task<ScoreDao> GetUnitaryScore(string PlayerName)
    {
        var result = await ExecuteGetQuery($"{_options.SqlQueryGet} LIMIT 1;");
        return result.FirstOrDefault()!;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScoreDao>> GetHighestScores(int ScoreCount)
    {
        return null;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateUnitaryScore(ScoreDao score)
    {
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> InsertUnitaryScore(ScoreDao score)
    {
        return false;
    }

    private async Task<IEnumerable<ScoreDao>> ExecuteGetQuery(string query)
    {
        var scores = new List<ScoreDao>();

        try
        {
            await _connection.OpenAsync();

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                scores.Add(new ScoreDao
                {
                    ScoreId = reader.GetInt32("ScoreId"),
                    PlayerName = reader.GetString("PlayerName"),
                    Value = reader.GetInt32("Value")
                });
            }
        }
        catch (MySqlException ex)
        {
            _logger.LogError($"[MariaDB ERROR] {ex.Message}");
            throw new DatabaseException(ex.Message);
        }
        finally
        {
            _connection.Close();
        }

        return scores;
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="query"></param>
    ///// <returns></returns>
    //private bool ExecuteGetQuery(string query)
    //{
    //    var scores = new List<ScoreDao>();
    //    await _connection.OpenAsync();

    //    var query = "SELECT ScoreId, PlayerName, Value, Date FROM Scores ORDER BY Value DESC";

    //    using var command = new MySqlCommand(query, connection);
    //    using var reader = await command.ExecuteReaderAsync();

    //    while (await reader.ReadAsync())
    //    {
    //        scores.Add(new ScoreDao
    //        {
    //            ScoreId = reader.GetInt32("ScoreId"),
    //            PlayerName = reader.GetString("PlayerName"),
    //            Value = reader.GetInt32("Value"),
    //            Date = reader.GetDateTime("Date")
    //        });
    //    }

    //    return scores;
    //}
}