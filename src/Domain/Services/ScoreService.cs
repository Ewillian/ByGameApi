using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;
using ByGameApi.Domain.Enums;

using Microsoft.IdentityModel.Tokens;

namespace ByGameApi.Domain.Services;

/// <summary>
/// Service responsible for managing score-related operations.
/// </summary>
public class ScoreService : IScoreService
{
    /// <summary>
    /// The repository interface used to interact with the database.
    /// </summary>
    private readonly IByRepository _byRepository;

    /// <summary>
    /// Constructs a new instance of <see cref="ScoreService"/>.
    /// </summary>
    /// <param name="byRepository">The repository implementation.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="byRepository"/> is null.</exception>
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
