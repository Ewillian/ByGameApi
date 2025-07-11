using ByGameApi.Api.Commands;
using ByGameApi.Api.Controllers;
using ByGameApi.Api.Responses;
using ByGameApi.Domain;
using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;
using ByGameApi.Domain.Enums;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ByGameApi.Api.Tests.Unit.Controllers;

public class ScoreControllerTests
{
    #region Fields

    private readonly Mock<ILogger<ScoreController>> _loggerMock;
    private readonly Mock<IScoreService> _scoreServiceMock;
    private readonly ScoreController _controller;
    private readonly DateTime _scoreDate = new(2024, 01, 01, 12, 00, 00);
    private readonly ScoreCommand _scoreCommand = new() {PlayerName = "PlayerName",Value = 10};
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
        var scoreCommand = new ScoreCommand
        {
            PlayerName = "PlayerName",
            Value = 10
        };
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
    [InlineData(Constants.ScoreNotFoundTitle, Constants.ScoreNotFoundMessage, StatusCodes.Status404NotFound)]
    [InlineData(Constants.InternalErrorTitle, Constants.InternalErrorMessage, StatusCodes.Status500InternalServerError)]
    public async Task GetScore_When_ScoreIsNotFound_Should_Return404(string expectedErrorTitle,string expectedErrorDesc, int expectedStatus)
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
        Assert.Equal(expectedErrorDesc, error.Description);
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

        var error = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal(Constants.BadRequestTitle, error.Title);
        Assert.Equal(Constants.BadRequestMessage, error.Description);
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

        var error = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal(Constants.ScoresNotFoundTitle, error.Title);
        Assert.Equal(Constants.ScoreNotFoundMessage, error.Description);
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

    [Fact]
    public async Task UpsertScore_When_ScoreCommandIsValidAndInsertSucceeds_Should_ReturnCreatedStatus()
    {
        // Arrange
        _scoreServiceMock.Setup(s => s.UpsertUnitaryScore(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(ScoreUpsertResult.Inserted);

        // Act
        var result = await _controller.UpsertScore(_scoreCommand);

        // Assert
        var objectResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
    }

    [Fact]
    public async Task UpsertScore_When_CommandValidationChecksBadPlayerName_Should_ReturnBadRequest()
    {
        // Arrange
        var scoreCommand = new ScoreCommand
        {
            PlayerName = "",
            Value = 10
        };

        // Act
        var result = await _controller.UpsertScore(scoreCommand);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

        var error = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal(Constants.BadRequestTitle, error.Title);
        Assert.Equal(Constants.BadRequestMessage, error.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UpsertScore_When_CommandValidationChecksBadValue_Should_ReturnBadRequest(int value)
    {
        // Arrange
        var scoreCommand = new ScoreCommand
        {
            PlayerName = "PlayerName",
            Value = value
        };

        // Act
        var result = await _controller.UpsertScore(scoreCommand);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

        var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal(Constants.BadRequestTitle, errorResponse.Title);
        Assert.Equal(Constants.BadRequestMessage, errorResponse.Description);
    }

    [Fact]
    public async Task UpsertScore_When_ExceptionIsThrown_Should_ReturnInternalServerError()
    {
        // Arrange
        _scoreServiceMock.Setup(s => s.UpsertUnitaryScore(_scoreCommand.PlayerName, _scoreCommand.Value))
                         .ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _controller.UpsertScore(_scoreCommand);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

        var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal(Constants.InternalErrorTitle, errorResponse.Title);
        Assert.Equal(Constants.InternalErrorMessage, errorResponse.Description);
    }

    #endregion Public methods
}
