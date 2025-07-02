using System.ComponentModel.DataAnnotations;
using System.Net;

using ByGameApi.Api.Commands;
using ByGameApi.Api.Responses;
using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ByGameApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScoreController : Controller
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
        [HttpGet(template: "{playerName}", Name = nameof(GetScore))]
        public async Task<IActionResult> GetScore([Required][FromHeader] ScoreCommand scoreCommand)
        {
            _logger.LogInformation("Request received");

            scoreCommand ??= new ScoreCommand();

            switch (scoreCommand.IsValid())
            {
                case StatusCodes.Status400BadRequest:
                    _logger.LogInformation("BadRequest");
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse
                    {
                        Title = "BadRequest",
                        Description = "The request was not processed because it lacked required data.",
                        Status = StatusCodes.Status400BadRequest
                    });

                case StatusCodes.Status403Forbidden:
                    _logger.LogInformation("Forbidden");
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse
                    {
                        Title = "Forbidden",
                        Description = "The required data contains forbidden elements.",
                        Status = StatusCodes.Status403Forbidden
                    });

                case StatusCodes.Status200OK:
                    break;

                default:
                    _logger.LogInformation("InternalServerError");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                    {
                        Title = "InternalServerError",
                        Description = "Somethin went wrong or is not handle by the service.",
                        Status = StatusCodes.Status500InternalServerError
                    });
            }

            ScoreDao sqlResult = null!;

            try
            {
                sqlResult = await _scoreService.GetScore(scoreCommand.PlayerName);
            }
            catch (Exception) 
            {
                _logger.LogInformation("InternalServerError");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Title = "InternalServerError",
                    Description = "Somethin went wrong or is not handle by the service.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }

            if (sqlResult.PlayerName.IsNullOrEmpty()) {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse
                {
                    Title = "NotFound",
                    Description = "The requested score was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return StatusCode(StatusCodes.Status200OK, new ScoreResponse
            {
                Status = StatusCodes.Status200OK,
                Score = sqlResult
            });
        }

        [ProducesResponseType(typeof(ScoreResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [HttpGet(template: "top/{scoreCount}", Name = nameof(GetTopScore))]
        public async Task<IActionResult> GetTopScore([FromQuery] int scoreNumber = 10)
        {
            _logger.LogInformation("Request received");

            if (scoreNumber <= 0)
            {
                _logger.LogInformation("BadRequest");
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse
                {
                    Title = "BadRequest",
                    Description = "The request was not processed because it lacked required data.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            IEnumerable<ScoreDao> sqlResult = [];

            try
            {
                sqlResult = await _scoreService.GetTopScore(scoreNumber);
            }
            catch (Exception)
            {
                _logger.LogInformation("InternalServerError");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Title = "InternalServerError",
                    Description = "Somethin went wrong or is not handle by the service.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }

            if (sqlResult.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse
                {
                    Title = "NotFound",
                    Description = "The requested score was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return StatusCode(StatusCodes.Status200OK, new TopScoreResponse
            {
                Score = sqlResult
            });
        }
    }
}
