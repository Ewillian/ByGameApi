using System.Diagnostics.CodeAnalysis;

namespace ByGameApi.Domain;

[ExcludeFromCodeCoverage]
public static class Constants
{
    public const string BadRequestTitle = "BadRequest";
    public const string InternalErrorTitle = "InternalError";
    public const string ScoreNotFoundTitle = "ScoreNotFound";
    public const string ScoresNotFoundTitle = "ScoresNotFound";
    public const string ForbiddenTitle = "Forbidden";

    public const string BadRequestMessage = "The request was not processed because it lacked required data.";
    public const string InternalErrorMessage = "Somethin went wrong or is not handle by the service.";
    public const string ScoreNotFoundMessage = "The requested score(s) was not found.";

}
