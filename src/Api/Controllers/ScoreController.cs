using System.ComponentModel.DataAnnotations;
using System.Net;

using ByGameApi.Api.Commands;
using ByGameApi.Api.Responses;
using ByGameApi.Domain;
using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ByGameApi.Api.Controllers
{
    [ApiController]
    [Route("score")]
    public class ScoreController : ControllerBase
    {
        #region Private fields
        private readonly ILogger<ScoreController> _logger;
        private readonly IScoreService _scoreService;
        #endregion Fields

        public ScoreController(ILogger<ScoreController>? logger, IScoreService scoreService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scoreService = scoreService ?? throw new ArgumentNullException(nameof(scoreService));
        }

        [ProducesResponseType(typeof(ScoreResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpGet("unitary", Name = nameof(GetScore))]
        public async Task<IActionResult> GetScore([Required][FromQuery] string playerName)
        {
            _logger.LogInformation("Request received: '{playerName}'", playerName);

            ScoreCommand scoreCommand = new() { PlayerName = playerName };

            switch (scoreCommand.IsValid())
            {
                case StatusCodes.Status400BadRequest:
                    _logger.LogInformation(Constants.BadRequestTitle);
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse
                    {
                        Title = Constants.BadRequestTitle,
                        Description = Constants.BadRequestMessage,
                        Status = StatusCodes.Status400BadRequest
                    });

                case StatusCodes.Status403Forbidden:
                    _logger.LogInformation(Constants.ForbiddenTitle);
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse
                    {
                        Title = Constants.ForbiddenTitle,
                        Description = "The required data contains forbidden elements.",
                        Status = StatusCodes.Status403Forbidden
                    });

                case StatusCodes.Status200OK:
                    break;

                default:
                    _logger.LogInformation(Constants.InternalErrorTitle);
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                    {
                        Title = Constants.InternalErrorTitle,
                        Description = Constants.InternalErrorMessage,
                        Status = StatusCodes.Status500InternalServerError
                    });
            }

            ScoreDao sqlResult = null!;

            try
            {
                sqlResult = await _scoreService.GetScore(scoreCommand.PlayerName);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "{errorTitle}", Constants.InternalErrorTitle);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Title = Constants.InternalErrorTitle,
                    Description = Constants.InternalErrorMessage,
                    Status = StatusCodes.Status500InternalServerError
                });
            }

            if (sqlResult.PlayerName.IsNullOrEmpty()) {
                _logger.LogInformation(Constants.ScoreNotFoundTitle);
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse
                {
                    Title = Constants.ScoreNotFoundTitle,
                    Description = Constants.ScoreNotFoundMessage,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return StatusCode(StatusCodes.Status200OK, new ScoreResponse
            {
                Score = sqlResult
            });
        }

        [ProducesResponseType(typeof(ScoreResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpGet("top", Name = nameof(GetTopScore))]
        public async Task<IActionResult> GetTopScore([FromQuery] int scoreNumber = 10)
        {
            _logger.LogInformation("Request received");

            if (scoreNumber <= 0)
            {
                _logger.LogInformation(Constants.BadRequestTitle);
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse
                {
                    Title = Constants.BadRequestTitle,
                    Description = Constants.BadRequestMessage,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            try
            {
                var sqlResult = await _scoreService.GetTopScore(scoreNumber);

                if (sqlResult.IsNullOrEmpty())
                {
                    _logger.LogInformation(Constants.ScoresNotFoundTitle);
                    return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse
                    {
                        Title = Constants.ScoresNotFoundTitle,
                        Description = Constants.ScoreNotFoundMessage,
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return StatusCode(StatusCodes.Status200OK, new TopScoreResponse
                {
                    Scores = sqlResult
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{errorTitle}", Constants.InternalErrorTitle);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Title = Constants.InternalErrorTitle,
                    Description = Constants.InternalErrorMessage,
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
