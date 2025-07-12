using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;
using ByGameApi.Domain.Enums;
using ByGameApi.Domain.Services;

using Moq;

using Xunit;

namespace ByGameApi.Domain.Tests.Unit.Services;

public class ScoreServiceTests
{
    private readonly Mock<IByRepository> _byRepositoryMock;
    private readonly ScoreService _scoreService;

    public ScoreServiceTests()
    {
        _byRepositoryMock = new();
        _scoreService = new(_byRepositoryMock.Object);
    }

    [Fact]
    public void Constructor_When_ArgumentIsMissing_Should_ReturnArgumentNullException()
    {
        // Arrange
        IByRepository? service = null;

        // Act & 
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            var scoreService = new ScoreService(service!);
        });

        //Assert
        Assert.Equal("byRepository", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetScore_When_PlayerNameIsNullOrEmpty_Should_ReturnEmptyScore(string? invalidName)
    {
        // Arrange & Act
        var result = await _scoreService.GetScore(invalidName!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("", result.PlayerName);
        _byRepositoryMock.Verify(r => r.GetUnitaryScore(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetScore_When_PlayerExists_Should_ReturnExpectedScore()
    {
        // Arrange
        var expectedPlayerName = "Player1";
        var expectedScore = new ScoreDao
        {
            PlayerName = expectedPlayerName,
            Value = 100
        };

        _byRepositoryMock.Setup(r => r.GetUnitaryScore(expectedPlayerName))
                .ReturnsAsync(expectedScore);

        var service = new ScoreService(_byRepositoryMock.Object);

        // Act
        var result = await service.GetScore(expectedPlayerName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPlayerName, result.PlayerName);
        Assert.Equal(100, result.Value);
    }

    [Fact]
    public async Task GetTopScore_When_ScoreCountIsNegative_Should_ReturnEmptyList()
    {
        // Arrange & Act
        var result = await _scoreService.GetTopScore(-1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTopScore_When_ValidScoreCountProvided_Should_ReturnScores()
    {
        // Arrange
        var expectedScores = new List<ScoreDao>
    {
        new ScoreDao { ScoreId = 1, PlayerName = "Player1", Value = 100 },
        new ScoreDao { ScoreId = 2, PlayerName = "Player2", Value = 90 }
    };

        _byRepositoryMock.Setup(r => r.GetHighestScores(2))
                         .ReturnsAsync(expectedScores);

        // Act
        var result = await _scoreService.GetTopScore(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Player1", result.First().PlayerName);
    }


    [Fact]
    public async Task UpsertUnitaryScore_When_PlayerNameIsEmpty_Should_ReturnNone()
    {
        var result = await _scoreService.UpsertUnitaryScore("", 100);
        Assert.Equal(ScoreUpsertResult.None, result);
    }

    [Fact]
    public async Task UpsertUnitaryScore_When_ScoreValueIsLessThanOrEqualZero_Should_ReturnNone()
    {
        var result = await _scoreService.UpsertUnitaryScore("Player1", 0);
        Assert.Equal(ScoreUpsertResult.None, result);
    }

    [Fact]
    public async Task UpsertUnitaryScore_When_PlayerNotFoundAndInsertSucceeds_Should_ReturnInserted()
    {
        // Arrange
        _byRepositoryMock.Setup(r => r.GetUnitaryScore("Player1"))
            .ReturnsAsync(new ScoreDao());

        _byRepositoryMock.Setup(r => r.InsertUnitaryScore(It.IsAny<ScoreDao>()))
            .ReturnsAsync(true);

        // Act
        var result = await _scoreService.UpsertUnitaryScore("Player1", 150);

        // Assert
        Assert.Equal(ScoreUpsertResult.Inserted, result);
    }

    [Fact]
    public async Task UpsertUnitaryScore_When_PlayerNotFoundAndInsertFails_Should_ReturnNone()
    {
        _byRepositoryMock.Setup(r => r.GetUnitaryScore("Player1"))
            .ReturnsAsync(new ScoreDao());

        _byRepositoryMock.Setup(r => r.InsertUnitaryScore(It.IsAny<ScoreDao>()))
            .ReturnsAsync(false);

        var result = await _scoreService.UpsertUnitaryScore("Player1", 150);

        Assert.Equal(ScoreUpsertResult.None, result);
    }

    [Fact]
    public async Task UpsertUnitaryScore_When_PlayerFoundAndUpdateSucceeds_Should_ReturnUpdated()
    {
        _byRepositoryMock.Setup(r => r.GetUnitaryScore("Player1"))
            .ReturnsAsync(new ScoreDao { PlayerName = "Player1" });

        _byRepositoryMock.Setup(r => r.UpdateUnitaryScore(It.IsAny<ScoreDao>()))
            .ReturnsAsync(true);

        var result = await _scoreService.UpsertUnitaryScore("Player1", 150);

        Assert.Equal(ScoreUpsertResult.Updated, result);
    }

    [Fact]
    public async Task UpsertUnitaryScore_When_PlayerFoundAndUpdateFails_Should_ReturnNone()
    {
        _byRepositoryMock.Setup(r => r.GetUnitaryScore("Player1"))
            .ReturnsAsync(new ScoreDao { PlayerName = "Player1" });

        _byRepositoryMock.Setup(r => r.UpdateUnitaryScore(It.IsAny<ScoreDao>()))
            .ReturnsAsync(false);

        var result = await _scoreService.UpsertUnitaryScore("Player1", 150);

        Assert.Equal(ScoreUpsertResult.None, result);
    }
}
