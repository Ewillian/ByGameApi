using ByGameApi.Domain.Dao;

namespace ByGameApi.Domain.Abstractions;

public interface IScoreService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerName"></param>
    /// <returns></returns>
    Task<ScoreDao> GetScore(string playerName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scoreCount"></param>
    /// <returns></returns>
    Task<IEnumerable<ScoreDao>> GetTopScore(int scoreCount);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    Task<bool> InsertUnitaryScore(ScoreDao score);
}
