using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;

using Microsoft.IdentityModel.Tokens;

namespace ByGameApi.Domain.Services;

public class ScoreService : IScoreService
{
    private readonly IByRepository _byRepository;

    public ScoreService(IByRepository byRepository)
    {
        _byRepository = byRepository ?? throw new ArgumentNullException(nameof(byRepository));
    }

    public async Task<ScoreDao> GetScore(string playerName)
    {
        if (playerName.IsNullOrEmpty()){ return new ScoreDao { PlayerName = "" }; }

        return await _byRepository.GetUnitaryScore(playerName);
    }

    public async Task<IEnumerable<ScoreDao>> GetTopScore(int scoreCount)
    {
        if (scoreCount < 0) { return []; }

        return await _byRepository.GetHighestScores(scoreCount);
    }
}
