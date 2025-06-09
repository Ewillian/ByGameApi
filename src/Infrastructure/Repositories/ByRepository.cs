using ByGameApi.Domain.Dao;

namespace ByGameApi.Infrastructure.Repositories;

public class ByRepository : IByRepository
{
    public ByRepository()
    {
    }

    /// <inheritdoc />
    public async Task<ScoreDao> GetUnitaryScore(string PlayerName)
    {
        return null;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScoreDao>> GetHighestScores(int ScoreCount)
    {
        return null;
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
    /// 
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    Task<bool> UpdateUnitaryScore(ScoreDao score);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    Task<bool> InsertUnitaryScore(ScoreDao score);
}