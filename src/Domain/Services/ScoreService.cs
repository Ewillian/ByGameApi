using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;

using Microsoft.IdentityModel.Tokens;

namespace ByGameApi.Domain.Services;

/// <summary>
/// 
/// </summary>
public class ScoreService : IScoreService
{
    /// <summary>
    /// 
    /// </summary>
    private readonly IByRepository _byRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="byRepository"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ScoreService(IByRepository byRepository)
    {
        _byRepository = byRepository ?? throw new ArgumentNullException(nameof(byRepository));
    }

    /// <inheritdoc />
    public async Task<ScoreDao> GetScore(string playerName)
    {
        if (playerName.IsNullOrEmpty()){ return new ScoreDao { PlayerName = "" }; }

        return await _byRepository.GetUnitaryScore(playerName);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScoreDao>> GetTopScore(int scoreCount)
    {
        if (scoreCount < 0) { return []; }

        return await _byRepository.GetHighestScores(scoreCount);
    }

    /// <inheritdoc />
    public async Task<bool> InsertUnitaryScore(ScoreDao score)
    {
        if (score.IsNotValid())
        {
            return false;
        }

        return await _byRepository.InsertUnitaryScore(score);
    }
}
