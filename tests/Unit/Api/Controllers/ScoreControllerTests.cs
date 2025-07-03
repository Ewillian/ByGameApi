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
    private readonly DateTime _scoreDate = new(2024, 01, 01, 12, 00, 00);
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
        Assert.Equal(missingElement, exception.ParamName);
    }

    [Fact]
    public async Task GetScore_When_ScoreIsFound_Should_Return200()
    {
        // Arrange
        var expectedDao = new ScoreDao { ScoreId = 0, PlayerName = "John", Value = 100, Date = _scoreDate };
        _scoreServiceMock
            .Setup(s => s.GetScore("TestPlayer"))
            .ReturnsAsync(expectedDao);

        // Act
        var result = await _controller.GetScore("TestPlayer");

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

        var scoreResponse = Assert.IsType<ScoreResponse>(objectResult.Value);
        Assert.Equal(expectedDao.ScoreId, scoreResponse.Score.ScoreId);
        Assert.Equal(expectedDao.PlayerName, scoreResponse.Score.PlayerName);
        Assert.Equal(expectedDao.Value, scoreResponse.Score.Value);
        Assert.Equal(expectedDao.Date, scoreResponse.Score.Date);
    }

    [Theory]
    [InlineData("", "BadRequest", StatusCodes.Status400BadRequest)]
    [InlineData("Select", "Forbidden", StatusCodes.Status403Forbidden)]
    public async Task GetScore_When_ValidationFails_Should_ReturnExpectedErrorResponse(string playerName, string expectedErrorTitle, int expectedStatus)
    {
        // Arrange & Act
        var result = await _controller.GetScore(playerName);

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
        var result = await _controller.GetScore("TestPlayer");

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(expectedStatus, objectResult.StatusCode);

        var error = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal(expectedErrorTitle, error.Title);
    }

    [Fact]
    public async Task GetTopScore_When_ScoresAreFound_Should_Return200()
    {
        // Arrange
        var scores = new List<ScoreDao> { new() { ScoreId = 0, PlayerName = "John", Value = 100, Date = _scoreDate } };

        _scoreServiceMock
            .Setup(s => s.GetTopScore(10))
            .ReturnsAsync(scores);

        // Act
        var result = await _controller.GetTopScore();

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

        var scoreResponse = Assert.IsType<TopScoreResponse>(objectResult.Value);

        for (int i = 0; i < scoreResponse.Scores.Count(); i++)
        {
            Assert.Equal(scores[i].ScoreId, scoreResponse.Scores.ElementAt(i).ScoreId);
            Assert.Equal(scores[i].PlayerName, scoreResponse.Scores.ElementAt(i).PlayerName);
            Assert.Equal(scores[i].Value, scoreResponse.Scores.ElementAt(i).Value);
            Assert.Equal(scores[i].Date, scoreResponse.Scores.ElementAt(i).Date);
        }
    }

    [Fact]
    public async Task GetTopScore_When_ScoreNumberIsZero_Should_ReturnBadRequest()
    {
        // Act
        var result = await _controller.GetTopScore(0);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        Assert.IsType<ErrorResponse>(objectResult.Value);
    }

    [Fact]
    public async Task GetTopScore_When_NoScoresExist_Should_ReturnNotFound()
    {
        // Arrange
        _scoreServiceMock.Setup(s => s.GetTopScore(10)).ReturnsAsync(new List<ScoreDao>());

        // Act
        var result = await _controller.GetTopScore();

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        Assert.IsType<ErrorResponse>(objectResult.Value);
    }

    [Fact]
    public async Task GetTopScore_When_OnException_Should_ReturnInternalServerError()
    {
        // Arrange
        _scoreServiceMock.Setup(s => s.GetTopScore(It.IsAny<int>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetTopScore();

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }

    #endregion Public methods
}
