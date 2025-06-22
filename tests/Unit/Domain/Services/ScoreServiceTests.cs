using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Dao;
using ByGameApi.Domain.Services;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace ByGameApi.Domain.Tests.Unit.Services;

public class ScoreServiceTests
{
    [Theory]
    [InlineData("logger")]
    [InlineData("byRepository")]
    public void Constructor_When_ArgumentIsMissing_Should_ReturnArgumentNullException(string missingElement)
    {
        // Arrange
        ILogger<ScoreService>? logger = missingElement == "logger" ? null : Mock.Of<ILogger<ScoreService>>();
        IByRepository? service = missingElement == "byRepository" ? null : Mock.Of<IByRepository>();

        // Act & 
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            var scoreService = new ScoreService(logger!, service!);
        });

        //Assert
        Assert.Equal(missingElement, exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetScore_WithNullOrEmptyPlayerName_ReturnsEmptyScore(string? invalidName)
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ScoreService>>();
        var mockRepo = new Mock<IByRepository>();

        var service = new ScoreService(mockLogger.Object, mockRepo.Object);

        // Act
        var result = await service.GetScore(invalidName!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("", result.PlayerName);
        mockRepo.Verify(r => r.GetUnitaryScore(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetScore_ReturnsExpectedScore()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ScoreService>>();
        var mockRepo = new Mock<IByRepository>();

        var expectedPlayerName = "Player1";
        var expectedScore = new ScoreDao
        {
            PlayerName = expectedPlayerName,
            Value = 100
        };

        mockRepo.Setup(r => r.GetUnitaryScore(expectedPlayerName))
                .ReturnsAsync(expectedScore);

        var service = new ScoreService(mockLogger.Object, mockRepo.Object);

        // Act
        var result = await service.GetScore(expectedPlayerName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPlayerName, result.PlayerName);
        Assert.Equal(100, result.Value);
    }
}
