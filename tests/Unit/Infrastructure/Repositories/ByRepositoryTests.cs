using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;
using ByGameApi.Domain.Services;
using ByGameApi.Infrastructure.Options;
using ByGameApi.Infrastructure.Repositories;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using Xunit;

namespace ByGameApi.Infrastructure.Tests.Unit.Repositories;

public class ByRepositoryTests
{
    [Theory]
    [InlineData("logger")]
    [InlineData("options")]
    public void Constructor_When_ArgumentIsMissing_Should_ReturnArgumentNullException(string missingElement)
    {
        // Arrange
        ILogger<ByRepository>? logger = missingElement == "logger" ? null : Mock.Of<ILogger<ByRepository>>();
        IOptions<DatabaseOptions>? databaseOptions = missingElement == "options" ? null : Mock.Of<IOptions<DatabaseOptions>>();

        // Act & 
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            var byRepository = new ByRepository(logger!, databaseOptions!);
        });

        //Assert
        Assert.Equal(missingElement, exception.ParamName);
    }

    [Fact]
    public async Task GetScore_WithEmptyPlayerName_ReturnsEmptyScoreDao()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ScoreService>>();
        var repoMock = new Mock<IByRepository>();
        var service = new ScoreService(loggerMock.Object, repoMock.Object);

        // Act
        var result = await service.GetScore("");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("", result.PlayerName);
        repoMock.Verify(x => x.GetUnitaryScore(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetScore_WithValidPlayerName_ReturnsScoreDaoFromRepository()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ScoreService>>();
        var repoMock = new Mock<IByRepository>();

        var expectedScore = new ScoreDao
        {
            ScoreId = 1,
            PlayerName = "John",
            Value = 100
        };

        repoMock.Setup(r => r.GetUnitaryScore("John"))
                .ReturnsAsync(expectedScore);

        var service = new ScoreService(loggerMock.Object, repoMock.Object);

        // Act
        var result = await service.GetScore("John");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.PlayerName);
        Assert.Equal(100, result.Value);
        repoMock.Verify(x => x.GetUnitaryScore("John"), Times.Once);
    }
}
