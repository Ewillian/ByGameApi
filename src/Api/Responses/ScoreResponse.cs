using System.Diagnostics.CodeAnalysis;

using ByGameApi.Domain.Dao;

namespace ByGameApi.Api.Responses;

[ExcludeFromCodeCoverage]
public class ScoreResponse
{
    public required ScoreDao Score { get; init; }
}
