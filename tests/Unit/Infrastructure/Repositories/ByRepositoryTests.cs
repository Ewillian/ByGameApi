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

        // Act & 
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            var byRepository = new ByRepository(logger!, databaseOptions!, commandExecutor!);
        });

        //Assert
        Assert.Equal(missingElement, exception.ParamName);
    }

    [Fact]
    public async Task GetUnitaryScore_ShouldReturnScoreDao_WhenPlayerExists()
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
    public async Task GetUnitaryScore_ShouldReturnEmptyList_WhenPlayerDoesNotExists()
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
}
