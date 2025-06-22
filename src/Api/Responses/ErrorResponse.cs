using System.Diagnostics.CodeAnalysis;

namespace ByGameApi.Api.Responses;

[ExcludeFromCodeCoverage]
public class ErrorResponse
{
    public string Title { get; init; } = string.Empty;
    public int Status { get; init; }
    public string Description { get; init; } = string.Empty;
}