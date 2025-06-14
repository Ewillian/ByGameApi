using ByGameApi.Domain.Dao;
using ByGameApi.Infrastructure.Options;
using Microsoft.Extensions.Options;

using MySqlConnector;

namespace ByGameApi.Infrastructure.Repositories;

public class ByRepository : IByRepository
{
    /// <summary>
    /// The database options
    /// </summary>
    private readonly DatabaseOptions _options;

    /// <summary>
    /// The database options
    /// </summary>
    private readonly MySqlConnection _connection;

    public ByRepository(IOptions<DatabaseOptions> options)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _connection = new MySqlConnection($"Server={_options.Server};Port={_options.Port};Database={_options.Database};User={_options.User};Password={_options.Password}");
    }

    /// <inheritdoc />
    public async Task<ScoreDao> GetUnitaryScore(string PlayerName)
    {
        ExecuteGetQuery("SELECT ScoreId, PlayerName, Value FROM Scores ORDER BY Value DESC");
        return null;
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
            Console.WriteLine($"[MariaDB ERROR] {ex.Message}");
            return null!;
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

public interface IByRepository
{
    /// <summary>
    /// Retrieves a player's scores.
    /// </summary>
    /// <param name="PlayerName">The player name</param>
    /// <returns>An asynchronous task containing a collection of <see cref="ScoreDao"/> representing the top scores.</returns>
    Task<ScoreDao> GetUnitaryScore(string PlayerName);

    /// <summary>
    /// Retrieves the top scores, ordered from highest to lowest.
    /// </summary>
    /// <param name="ScoreCount">The number of top scores to retrieve.</param>
    /// <returns>
    /// An asynchronous task containing a collection of <see cref="ScoreDao"/> representing the top scores.
    /// </returns>
    Task<IEnumerable<ScoreDao>> GetHighestScores(int ScoreCount);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    Task<bool> UpdateUnitaryScore(ScoreDao score);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    Task<bool> InsertUnitaryScore(ScoreDao score);
}