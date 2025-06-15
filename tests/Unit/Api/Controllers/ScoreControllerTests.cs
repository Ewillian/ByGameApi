using ByGameApi.Api.Commands;
using ByGameApi.Api.Controllers;
using ByGameApi.Api.Responses;
using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace ByGameApi.Api.Tests.Unit.Controllers;

public class ScoreControllerTests
{
    #region Fields

    private readonly Mock<ILogger<ScoreController>> _loggerMock;
    private readonly Mock<IScoreService> _scoreServiceMock;
    private readonly ScoreController _controller;
    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ScoreControllerTests"/> class
    /// </summary>
    public ScoreControllerTests()
    {
        _loggerMock = new Mock<ILogger<ScoreController>>();
        _scoreServiceMock = new Mock<IScoreService>();
        _controller = new ScoreController(_loggerMock.Object, _scoreServiceMock.Object);
    }

    #endregion Constructors

    #region Public methods

    [Theory]
    [InlineData("logger")]
    [InlineData("scoreService")]
    public void Constructor_When_ArgumentIsMissing_Should_ReturnArgumentNullException(string missingElement)
    {
        // Arrange
        ILogger<ScoreController>? logger = missingElement == "logger" ? null : Mock.Of<ILogger<ScoreController>>();
        IScoreService? service = missingElement == "scoreService" ? null : Mock.Of<IScoreService>();

        // Act & 
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            var controller = new ScoreController(logger!, service!);
        });

        //Assert
        Assert.Equal(missingElement == "logger" ? "logger" : "scoreService", exception.ParamName);
    }

    [Fact]
    public async Task GetScore_When_ScoreIsFound_Should_Return200()
    {
        // Arrange
        var scoreCommand = new ScoreCommand { PlayerName = "TestPlayer" };

        var expectedDao = new ScoreDao { PlayerName = "TestPlayer", Value = 42 };
        _scoreServiceMock
            .Setup(s => s.GetScore("TestPlayer"))
            .ReturnsAsync(expectedDao);

        // Act
        var result = await _controller.GetScore(scoreCommand);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

        var scoreResponse = Assert.IsType<ScoreResponse>(objectResult.Value);
        Assert.Equal(expectedDao.PlayerName, scoreResponse.Score.PlayerName);
        Assert.Equal(expectedDao.Value, scoreResponse.Score.Value);
    }

    [Theory]
    [InlineData("", "BadRequest", StatusCodes.Status400BadRequest)]
    [InlineData("Select", "Forbidden", StatusCodes.Status403Forbidden)]
    public async Task GetScore_When_ValidationFails_Should_ReturnExpectedErrorResponse(string playerName, string expectedErrorTitle, int expectedStatus)
    {
        // Arrange
        var scoreCommand = new ScoreCommand { PlayerName = playerName };

        // Act
        var result = await _controller.GetScore(scoreCommand);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(expectedStatus, objectResult.StatusCode);

        var error = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal(expectedErrorTitle, error.Title);
    }

    [Theory]
    [InlineData("NotFound", StatusCodes.Status404NotFound)]
    [InlineData("InternalServerError", StatusCodes.Status500InternalServerError)]
    public async Task GetScore_When_ScoreIsNotFound_Should_Return404(string expectedErrorTitle, int expectedStatus)
    {
        // Arrange
        var scoreCommand = new ScoreCommand { PlayerName = "TestPlayer" };

        if (expectedStatus == 404)
        {
            _scoreServiceMock
                .Setup(s => s.GetScore("TestPlayer"))
                .ReturnsAsync(new ScoreDao { PlayerName = "", Value = 0 });
        }
        else
        {
            _scoreServiceMock
            .Setup(s => s.GetScore(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Simulated failure"));
        }

        // Act
        var result = await _controller.GetScore(scoreCommand);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(expectedStatus, objectResult.StatusCode);

        var error = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal(expectedErrorTitle, error.Title);
    }

    #endregion Public methods

}