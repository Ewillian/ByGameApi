using ByGameApi.Domain.Dao;

namespace ByGameApi.Infrastructure.Abstractions;

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
    /// Updates the score with a high one for a specific player
    /// </summary>
    /// <param name="score">The score to update</param>
    /// <returns>The query status result</returns>
    Task<bool> UpdateUnitaryScore(ScoreDao score);

    /// <summary>
    /// Inserts a new score for a new player
    /// </summary>
    /// <param name="score">The score to insert</param>
    /// <returns>The query status result</returns>
    Task<bool> InsertUnitaryScore(ScoreDao score);
}
