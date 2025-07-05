using ByGameApi.Domain.Dao;

namespace ByGameApi.Infrastructure.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IDbCommandExecutor
{
    /// <summary>
    /// Executes a read-only SQL query and returns the result as a collection of <see cref="ScoreDao"/>.
    /// </summary>
    /// <param name="query">The SQL query to be executed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of <see cref="ScoreDao"/> returned by the query.</returns>
    Task<IEnumerable<ScoreDao>> ExecuteReaderAsync(string query);

    /// <summary>
    /// Executes a SQL command that modifies the database (e.g., INSERT, UPDATE).
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the operation completed successfully.</returns>
    Task<bool> ExecuteChangesAsync(string query);
}
