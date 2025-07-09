using ByGameApi.Domain.Dao;
using ByGameApi.Domain.Enums;

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
    /// <param name="playerName"></param>
    /// <param name="scoreValue"></param>
    /// <returns></returns>
    Task<ScoreUpsertResult> UpsertUnitaryScore(string playerName, int scoreValue);
}
