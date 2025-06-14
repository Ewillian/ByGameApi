using System.Diagnostics.CodeAnalysis;

namespace ByGameApi.Infrastructure.Exception;

[ExcludeFromCodeCoverage]
public class DatabaseException : System.Exception
{
    public DatabaseException(string? message) : base(message)
    {
    }
}
