using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;
using ByGameApi.Infrastructure.Abstractions;
using ByGameApi.Infrastructure.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ByGameApi.Infrastructure.Repositories;

/// <summary>
/// Repository responsible for interacting with the Scores database.
/// Implements logic related to inserting, updating, and retrieving scores.
/// </summary>
public class ByRepository : IByRepository
{
    /// <summary>
    /// Logger instance used for diagnostics and error logging.
    /// </summary>
    private readonly ILogger<ByRepository> _logger;

    /// <summary>
    /// Configuration options for database access.
    /// </summary>
    private readonly DatabaseOptions _options;

    /// <summary>
    /// Abstraction for executing SQL commands and queries.
    /// </summary>
    private readonly IDbCommandExecutor _commandExecutor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByRepository"/> class.
    /// </summary>
    /// <param name="logger">Logger used for diagnostic purposes.</param>
    /// <param name="options">Injected configuration options for database queries.</param>
    /// <param name="commandExecutor">Component responsible for executing database commands.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the dependencies are null.</exception>
    public ByRepository(ILogger<ByRepository> logger, IOptions<DatabaseOptions> options, IDbCommandExecutor commandExecutor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
    }

    /// <inheritdoc />
    public async Task<ScoreDao> GetUnitaryScore(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            return new ScoreDao();

        var result = await _commandExecutor.ExecuteReaderAsync($"SELECT ScoreId, PlayerName, Value FROM {_options.Table} WHERE PlayerName = '{playerName}' LIMIT 1;");

        return result.IsNullOrEmpty() ? new ScoreDao() : result.FirstOrDefault()!;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScoreDao>> GetHighestScores(int scoreCount)
    {
        if(scoreCount < 0) {  return []; }

        return await _commandExecutor.ExecuteReaderAsync($"SELECT * FROM {_options.Table} ORDER BY Value DESC LIMIT {scoreCount};");
    }

    /// <inheritdoc />
    public async Task<bool> UpdateUnitaryScore(ScoreDao score)
    {
        if (score.IsNotValid())
        {
            _logger.LogError("[UpdateUnitaryScore: ScoreDao IsNotValid]");
            return false;
        }

        Dictionary<string, object> dbParameters = new()
        {
            { "@name", score.PlayerName },
            { "@value", score.Value }
        };

        var isRowAffected = await _commandExecutor.ExecuteChangesAsync($"UPDATE {_options.Table} SET Value = @value WHERE PlayerName = @name;", dbParameters);


        if (!isRowAffected)
        {
            _logger.LogWarning("[UpdateUnitaryScore] No rows affected");
        }

        return isRowAffected;
    }

    /// <inheritdoc />
    public async Task<bool> InsertUnitaryScore(ScoreDao score)
    {
        if (score.IsNotValid())
        {
            _logger.LogError("[InsertUnitaryScore: ScoreDao IsNotValid] ");
            return false;
        }

        Dictionary<string, object> dbParameters = new() 
        {
            { "@name", score.PlayerName },
            { "@value", score.Value },
            { "@date", score.Date }
        };

        var isRowAffected = await _commandExecutor.ExecuteChangesAsync($"INSERT INTO {_options.Table} (PlayerName, Value, CreatedAt) VALUES (@name, @value, @date);", dbParameters);


        if (!isRowAffected)
        {
            _logger.LogWarning("[InsertUnitaryScore] No rows affected");
        }

        return isRowAffected;
    }
}
