using System.Diagnostics.CodeAnalysis;

namespace ByGameApi.Infrastructure.Exception;

/// <summary>
/// Represents errors that occur during database operations within the application.
/// </summary>
[ExcludeFromCodeCoverage]
public class DatabaseException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DatabaseException(string? message) : base(message)
    {
    }
}
