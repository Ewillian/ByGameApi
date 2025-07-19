using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace ByGameApi.Domain.Dao;

/// <summary>
/// Data Access Object representing a player's score.
/// </summary>
[ExcludeFromCodeCoverage]
public class ScoreDao
{
    /// <summary>
    /// Gets the unique identifier of the score.
    /// </summary>
    [JsonProperty("scoreId")]
    public int ScoreId { get; init; }

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    [JsonProperty("playerName")]
    public string PlayerName { get; init; } = "";

    /// <summary>
    /// Gets the value of the score.
    /// </summary>
    [JsonProperty("value")]
    public int Value { get; init; }

    /// <summary>
    /// Gets the date the score was recorded.
    /// </summary>
    [JsonProperty("date")]
    public DateTime Date { get; init; }

    /// <summary>
    /// Determines whether the current score object is invalid.
    /// A score is considered invalid if the player name is null, empty, or whitespace, or if the score value is negative.
    /// </summary>
    /// <returns><c>true</c> if the score is invalid; otherwise, <c>false</c>.</returns>
    public bool IsNotValid()
    {
        if (string.IsNullOrWhiteSpace(PlayerName) || Value < 0)
        {
            return true;
        }

        return false;
    }
}

