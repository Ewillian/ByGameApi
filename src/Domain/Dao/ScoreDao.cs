using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace ByGameApi.Domain.Dao;

[ExcludeFromCodeCoverage]
public class ScoreDao
{
    [JsonProperty("scoreId")]
    public int ScoreId { get; init; }

    [JsonProperty("playerName")]
    public string PlayerName { get; init; } = "";

    [JsonProperty("value")]
    public int Value { get; init; }

    [JsonProperty("date")]
    public DateTime Date { get; init; }

    public bool IsNotValid(out string? error)
    {
        if (string.IsNullOrWhiteSpace(PlayerName))
        {
            error = "Player name is required.";
            return false;
        }

        if (Value < 0)
        {
            error = "Score value must be non-negative.";
            return false;
        }

        error = null;
        return true;
    }
}

