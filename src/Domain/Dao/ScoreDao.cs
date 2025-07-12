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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsNotValid()
    {
        if (string.IsNullOrWhiteSpace(PlayerName) || Value < 0)
        {
            return true;
        }

        return false;
    }
}

