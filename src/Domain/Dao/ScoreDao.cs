using Newtonsoft.Json;

namespace ByGameApi.Domain.Dao;

public class ScoreDao
{
    [JsonProperty("scoreId")]
    public int ScoreId { get; set; }

    [JsonProperty("playerName")]
    public string PlayerName { get; set; } = "";

    [JsonProperty("value")]
    public int Value { get; set; }
}

