using ByGameApi.Domain.Dao;

namespace ByGameApi.Infrastructure.Abstractions;

public interface IDbCommandExecutor
{
    Task<IEnumerable<ScoreDao>> ExecuteReaderAsync(string query);
}
