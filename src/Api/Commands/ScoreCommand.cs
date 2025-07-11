using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace ByGameApi.Api.Commands;

public class ScoreCommand
{
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty(nameof(PlayerName))]
    public string PlayerName { get; init; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty(nameof(Value))]
    public int Value { get; init; } = 0;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int IsValid(bool completeCheck = false)
    {
        if(PlayerName == string.Empty || Value <= 0 && completeCheck) { return StatusCodes.Status400BadRequest; }

        if (ContainsSqlKeywords(PlayerName) || ContainsSuspiciousSqlChars(PlayerName) || ContainsUnusualUnicode(PlayerName))
        {
            return StatusCodes.Status403Forbidden;
        }

        return StatusCodes.Status200OK;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static bool ContainsSqlKeywords(string input)
    {
        string[] keywords = { "SELECT", "INSERT", "DELETE", "DROP", "UPDATE", "--", ";", "'" };

        return keywords.Any(keyword => input.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static bool ContainsSuspiciousSqlChars(string input)
    {
        return Regex.IsMatch(input, @"['"";\\-]{1,}", RegexOptions.None, TimeSpan.FromMilliseconds(100));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsUnusualUnicode(string input)
    {
        char[] suspicious = ['\u0000', '\u200B', '\u202E', '\uFEFF'];

        return input.Any(c => suspicious.Contains(c));
    }
}
