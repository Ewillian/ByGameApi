namespace ByGameApi.Domain.Enums;

/// <summary>
/// Represents the result of a score upsert operation (insert or update).
/// </summary>
public enum ScoreUpsertResult
{
    /// <summary>
    /// No changes were made. No rows were affected by the operation.
    /// </summary>
    None,

    /// <summary>
    /// A new score was successfully inserted into the database.
    /// </summary>
    Inserted,

    /// <summary>
    /// An existing score was successfully updated in the database.
    /// </summary>
    Updated
}
