using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;

using Microsoft.Extensions.Logging;


namespace ByGameApi.Domain.Services;

public class ScoreService : IScoreService
{
    private readonly ILogger<ScoreService> _logger;
    private readonly IByRepository _byRepository;

    public ScoreService(ILogger<ScoreService> logger, IByRepository byRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _byRepository = byRepository ?? throw new ArgumentNullException(nameof(byRepository));
    }

    public async Task<ScoreDao> GetScore(string playerName)
    {
        return await _byRepository.GetUnitaryScore(playerName);
    }
}
