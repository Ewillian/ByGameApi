using ByGameApi.Domain.Dao;

namespace ByGameApi.Domain.Abstractions;

public interface IScoreService
{
    Task<ScoreDao> GetScore(string playerName);
}
