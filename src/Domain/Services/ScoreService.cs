using System.Reflection.Metadata.Ecma335;

using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;
using ByGameApi.Domain.Enums;

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
    public async Task<ScoreUpsertResult> UpsertUnitaryScore(string playerName, int scoreValue)
    {
        if (string.IsNullOrEmpty(playerName) || scoreValue <= 0)
            return ScoreUpsertResult.None;

        var existingScore = await _byRepository.GetUnitaryScore(playerName);
        bool isNewPlayer = string.IsNullOrEmpty(existingScore.PlayerName);

        // The case where the player already exists but the proposed score is less than or equal to the existing one.
        if (!isNewPlayer && scoreValue <= existingScore.Value)
            return ScoreUpsertResult.Unchanged;

        var scoreToSave = new ScoreDao
        {
            PlayerName = playerName,
            Value = scoreValue,
            Date = DateTime.UtcNow
        };

        bool success = isNewPlayer
            ? await _byRepository.InsertUnitaryScore(scoreToSave)
            : await _byRepository.UpdateUnitaryScore(scoreToSave);

        if (!success)
            return ScoreUpsertResult.None;

        return isNewPlayer
            ? ScoreUpsertResult.Inserted
            : ScoreUpsertResult.Updated;
    }
}
