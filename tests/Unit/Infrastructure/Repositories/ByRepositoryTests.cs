using ByGameApi.Domain.Dao;
using ByGameApi.Infrastructure.Abstractions;
using ByGameApi.Infrastructure.Options;
using ByGameApi.Infrastructure.Repositories;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using Xunit;

namespace ByGameApi.Infrastructure.Tests.Unit.Repositories;

public class ByRepositoryTests
{
    private readonly Mock<ILogger<ByRepository>> _loggerMock;
    private readonly Mock<IDbCommandExecutor> _commandExecutorMock;
    private readonly DatabaseOptions _databaseOptions;
    private readonly ByRepository _byRepository;

    public ByRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<ByRepository>>();
        _commandExecutorMock = new Mock<IDbCommandExecutor>();
        _databaseOptions = new DatabaseOptions
        {
            Server = "localhost",
            Port = "3306",
            Database = "testdb",
            User = "root",
            Password = "password",
            Table = "Scores",
            SqlQueryGet = "SELECT * FROM Scores",
            MinConnectionPoolSize = 10,
            MaxConnectionPoolSize = 100,
            ConnectionTimeout = 5000
        };

        _byRepository = new ByRepository(_loggerMock.Object, _databaseOptions, _commandExecutorMock.Object);
    }

    [Theory]
    [InlineData("logger")]
    [InlineData("options")]
    [InlineData("commandExecutor")]
    public void Constructor_When_ArgumentIsMissing_Should_ReturnArgumentNullException(string missingElement)
    {
        // Arrange
        ILogger<ByRepository>? logger = missingElement == "logger" ? null : Mock.Of<ILogger<ByRepository>>();
        IOptions<DatabaseOptions>? databaseOptions = missingElement == "options" ? null : Mock.Of<IOptions<DatabaseOptions>>(opt => opt.Value == _databaseOptions);
        IDbCommandExecutor? commandExecutor = missingElement == "commandExecutor" ? null : Mock.Of<IDbCommandExecutor>();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            var byRepository = new ByRepository(logger!, databaseOptions!, commandExecutor!);
        });

        //Assert
        Assert.Equal(missingElement, exception.ParamName);
    }

    [Fact]
    public async Task GetUnitaryScore_When_PlayerExists_Should_ReturnScoreDao()
    {
        // Arrange
        var expected = new ScoreDao { ScoreId = 1, PlayerName = "Test", Value = 100 };

        _commandExecutorMock.Setup(e => e.ExecuteReaderAsync(It.IsAny<string>()))
                    .ReturnsAsync(new List<ScoreDao> { expected });

        // Act
        var result = await _byRepository.GetUnitaryScore("Test");

        // Assert
        Assert.Equal(expected.PlayerName, result.PlayerName);
        Assert.Equal(expected.Value, result.Value);
    }

    [Fact]
    public async Task GetUnitaryScore_When_PlayerDoesNotExist_Should_ReturnEmptyScoreDao()
    {
        // Arrange
        var expected = new ScoreDao { ScoreId = 1, PlayerName = "Test", Value = 100 };

        _commandExecutorMock.Setup(e => e.ExecuteReaderAsync(It.IsAny<string>()))
                    .ReturnsAsync(new List<ScoreDao>());

        // Act
        var result = await _byRepository.GetUnitaryScore("Test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.ScoreId);
        Assert.Empty(result.PlayerName);
        Assert.Equal(0, result.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetUnitaryScore_When_PlayerNameIsNullOrEmpty_Should_ReturnEmptyScoreDao(string playerName)
    {
        // Arrange & Act
        var result = await _byRepository.GetUnitaryScore(playerName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.ScoreId);
        Assert.Empty(result.PlayerName);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public async Task GetHighestScores_When_ScoreCountIsNegative_Should_ReturnEmptyList()
    {
        // Arrange & Act
        var result = await _byRepository.GetHighestScores(-1);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetHighestScores_When_ValidCountProvided_Should_ReturnScoreList()
    {
        // Arrange
        var expected = new List<ScoreDao>
        {
            new() { ScoreId = 0, PlayerName = "John", Value = 100, Date = DateTime.UtcNow }
        };

        _commandExecutorMock
            .Setup(exec => exec.ExecuteReaderAsync(It.IsAny<string>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _byRepository.GetHighestScores(10);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task InsertUnitaryScore_When_ScoreIsValidAndInsertSucceeds_Should_ReturnTrue()
    {
        // Arrange
        var score = new ScoreDao
        {
            PlayerName = "Player1",
            Value = 100,
            Date = DateTime.UtcNow
        };

        _commandExecutorMock
            .Setup(x => x.ExecuteChangesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _byRepository.InsertUnitaryScore(score);

        // Assert
        Assert.True(result);
        _commandExecutorMock.Verify(x => x.ExecuteChangesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public async Task InsertUnitaryScore_When_ScoreIsInvalid_Should_ReturnFalseAndLogError()
    {
        // Arrange
        var invalidScore = new ScoreDao();

        // Act
        var result = await _byRepository.InsertUnitaryScore(invalidScore);

        // Assert
        Assert.False(result);
        _loggerMock.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("IsNotValid")),
            It.IsAny<System.Exception>(),
            It.IsAny<Func<It.IsAnyType, System.Exception?, string>>()), Times.Once);

        _commandExecutorMock.Verify(x => x.ExecuteChangesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Never);
    }

    [Fact]
    public async Task InsertUnitaryScore_When_InsertFails_Should_ReturnFalseAndLogWarning()
    {
        // Arrange
        var validScore = new ScoreDao
        {
            PlayerName = "Player2",
            Value = 150,
            Date = DateTime.UtcNow
        };

        _commandExecutorMock
            .Setup(x => x.ExecuteChangesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(false);

        var repository = new ByRepository(_loggerMock.Object, _databaseOptions, _commandExecutorMock.Object);

        // Act
        var result = await repository.InsertUnitaryScore(validScore);

        // Assert
        Assert.False(result);
        _loggerMock.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No rows affected")),
            It.IsAny<System.Exception>(),
            It.IsAny<Func<It.IsAnyType, System.Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUnitaryScore_When_ScoreIsInvalid_Should_ReturnFalseAndLogError()
    {
        // Arrange
        var invalidScore = new Mock<ScoreDao>();

        // Act
        var result = await _byRepository.UpdateUnitaryScore(invalidScore.Object);

        // Assert
        Assert.False(result);

        _loggerMock.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[UpdateUnitaryScore: ScoreDao IsNotValid]")),
            It.IsAny<System.Exception>(),
            It.IsAny<Func<It.IsAnyType, System.Exception?, string>>()), Times.Once);

        _commandExecutorMock.Verify(
            x => x.ExecuteChangesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateUnitaryScore_When_NoRowsAreAffected_Should_ReturnFalseAndLogWarning()
    {
        // Arrange
        var validScore = new ScoreDao { PlayerName = "Alice", Value = 100 };

        _commandExecutorMock
            .Setup(x => x.ExecuteChangesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(false);

        // Act
        var result = await _byRepository.UpdateUnitaryScore(validScore);

        // Assert
        Assert.False(result);

        _loggerMock.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No rows affected")),
            It.IsAny<System.Exception>(),
            It.IsAny<Func<It.IsAnyType, System.Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUnitaryScore_When_RowIsAffected_Should_ReturnTrue()
    {
        // Arrange
        var validScore = new ScoreDao { PlayerName = "Bob", Value = 300 };

        _commandExecutorMock
            .Setup(x => x.ExecuteChangesAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _byRepository.UpdateUnitaryScore(validScore);

        // Assert
        Assert.True(result);
        _loggerMock.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No rows affected")),
            It.IsAny<System.Exception>(),
            It.IsAny<Func<It.IsAnyType, System.Exception?, string>>()), Times.Never);
    }
}
