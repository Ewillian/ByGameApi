using ByGameApi.Domain.Dao;
using ByGameApi.Domain.Enums;

namespace ByGameApi.Domain.Abstractions;

public interface IScoreService
{
    /// <summary>
    /// Retrieves the score for a given player by name.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <returns>The <see cref="ScoreDao"/> associated with the player. If the name is null or empty, returns a default object.</returns>
    Task<ScoreDao> GetScore(string playerName);

    /// <summary>
    /// Retrieves the top scores up to the given count.
    /// </summary>
    /// <param name="scoreCount">The number of top scores to retrieve.</param>
    /// <returns>A collection of <see cref="ScoreDao"/> objects. Returns an empty list if <paramref name="scoreCount"/> is less than 0.</returns>
    Task<IEnumerable<ScoreDao>> GetTopScore(int scoreCount);

    /// <summary>
    /// Inserts or updates a score for the specified player depending on the existing data.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="scoreValue">The score value to upsert.</param>
    /// <returns>
    /// A <see cref="ScoreUpsertResult"/> indicating the result:
    /// - <see cref="ScoreUpsertResult.Inserted"/>: New score inserted.
    /// - <see cref="ScoreUpsertResult.Updated"/>: Existing score updated.
    /// - <see cref="ScoreUpsertResult.Unchanged"/>: No update performed because the new score is lower or equal.
    /// - <see cref="ScoreUpsertResult.None"/>: Invalid input or persistence failed.
    /// </returns>
    Task<ScoreUpsertResult> UpsertUnitaryScore(string playerName, int scoreValue);
}
