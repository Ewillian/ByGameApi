using System.Diagnostics.CodeAnalysis;

namespace ByGameApi.Domain;

/// <summary>
/// Provides application-wide constant values used for error handling and messages.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Constants
{
    /// <summary>
    /// Title used for a Bad Request error response.
    /// </summary>
    public const string BadRequestTitle = "BadRequest";

    /// <summary>
    /// Title used for an Internal Server Error response.
    /// </summary>
    public const string InternalErrorTitle = "InternalError";

    /// <summary>
    /// Title used when a specific score is not found.
    /// </summary>
    public const string ScoreNotFoundTitle = "ScoreNotFound";

    /// <summary>
    /// Title used when multiple scores are not found.
    /// </summary>
    public const string ScoresNotFoundTitle = "ScoresNotFound";

    /// <summary>
    /// Title used when access to the resource is forbidden.
    /// </summary>
    public const string ForbiddenTitle = "Forbidden";

    /// <summary>
    /// Message explaining that the request could not be processed due to missing required data.
    /// </summary>
    public const string BadRequestMessage = "The request was not processed because it lacked required data.";


    /// <summary>
    /// Message indicating that an unexpected internal error occurred or the case was not handled.
    /// </summary>
    public const string InternalErrorMessage = "Something went wrong or is not handled by the service.";


    /// <summary>
    /// Message returned when a specific score cannot be found.
    /// </summary>
    public const string ScoreNotFoundMessage = "The requested score(s) was not found.";
}
