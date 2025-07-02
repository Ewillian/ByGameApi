using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;
using ByGameApi.Infrastructure.Abstractions;
using ByGameApi.Infrastructure.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using static System.Formats.Asn1.AsnWriter;

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
    /// The sql command executor
    /// </summary>
    private readonly IDbCommandExecutor _commandExecutor;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    /// <param name="commandExecutor"></param>
    /// <exception cref="ArgumentNullException"></exception>
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

        var result = await _commandExecutor.ExecuteReaderAsync($"{_options.SqlQueryGet} WHERE PlayerName = '{playerName}' LIMIT 1;");

        return result.IsNullOrEmpty() ? new ScoreDao() : result.FirstOrDefault()!;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScoreDao>> GetHighestScores(int scoreCount)
    {
        if(scoreCount < 0) {  return []; }

        var result = await _commandExecutor.ExecuteReaderAsync($"SELECT * FROM Scores ORDER BY Value DESC LIMIT {scoreCount};");

        return result;
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

}