using ByGameApi.Domain.Dao;

namespace ByGameApi.Infrastructure.Abstractions;

/// <summary>
/// Defines a contract for executing SQL commands or queries against a relational database,
/// specifically for retrieving or modifying/inserting <see cref="ScoreDao"/> data.
/// </summary>
public interface IDbCommandExecutor
{
    /// <summary>
    /// Executes a read-only SQL query and returns the results as a collection of <see cref="ScoreDao"/> instances.
    /// </summary>
    /// <param name="query">The SQL query to be executed.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains a collection of <see cref="ScoreDao"/>
    /// retrieved from the database.
    /// </returns>
    Task<IEnumerable<ScoreDao>> ExecuteReaderAsync(string query);

    /// <summary>
    /// Executes a SQL command that modifies the database, such as an INSERT or UPDATE,
    /// using the specified parameters.
    /// </summary>
    /// <param name="query">The SQL command to execute.</param>
    /// <param name="parameters">A dictionary of parameter names and values to be used in the query.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result is <c>true</c> if the command was executed successfully; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> ExecuteChangesAsync(string query, Dictionary<string, object> parameters);
}
