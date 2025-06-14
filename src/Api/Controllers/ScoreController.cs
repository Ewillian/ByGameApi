using ByGameApi.Infrastructure.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace ByGameApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScoreController : Controller
    {
        #region Private fields
        private readonly ILogger<ScoreController> _logger;
        #endregion Fields

        public ScoreController(ILogger<ScoreController>? logger, IByRepository byRepository)
        {
            _logger = logger;
        }

        [HttpGet("", Name = nameof(GetScores))]
        public IActionResult GetScores()
        {
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
