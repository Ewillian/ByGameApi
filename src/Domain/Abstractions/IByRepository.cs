using ByGameApi.Domain.Dao;

namespace ByGameApi.Domain.Abstractions;

/// <summary>
/// Defines the contract for interacting with the score data source.
/// </summary>
public interface IByRepository
{
    /// <summary>
    /// Retrieves the score of a specific player.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <returns>
    /// An asynchronous task containing a <see cref="ScoreDao"/> representing the player's score,
    /// or an empty object if not found.
    /// </returns>
    Task<ScoreDao> GetUnitaryScore(string playerName);

    /// <summary>
    /// Retrieves the highest scores, ordered from highest to lowest.
    /// </summary>
    /// <param name="scoreCount">The number of top scores to retrieve.</param>
    /// <returns>
    /// An asynchronous task containing a collection of <see cref="ScoreDao"/> representing the top scores.
    /// </returns>
    Task<IEnumerable<ScoreDao>> GetHighestScores(int scoreCount);

    /// <summary>
    /// Updates the score for an existing player if the new score is higher.
    /// </summary>
    /// <param name="score">The score data to update.</param>
    /// <returns>
    /// A task that returns <c>true</c> if the update succeeded; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> UpdateUnitaryScore(ScoreDao score);

    /// <summary>
    /// Inserts a new score for a player.
    /// </summary>
    /// <param name="score">The score data to insert.</param>
    /// <returns>
    /// A task that returns <c>true</c> if the insert succeeded; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> InsertUnitaryScore(ScoreDao score);
}
