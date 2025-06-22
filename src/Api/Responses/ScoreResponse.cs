using System.Diagnostics.CodeAnalysis;

using ByGameApi.Domain.Dao;

namespace ByGameApi.Api.Responses;

[ExcludeFromCodeCoverage]
public class ScoreResponse
{
    public int Status { get; init; }
    public required ScoreDao Score { get; init; }
}
