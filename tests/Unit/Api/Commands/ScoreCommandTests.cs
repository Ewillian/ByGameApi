using ByGameApi.Api.Commands;

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

using Xunit;

namespace ByGameApi.Api.Tests.Unit.Commands;

public class ScoreCommandTests
{
    #region Fields

    /// <summary>
    /// 
    /// </summary>
    private readonly ScoreCommand _scoreCommand;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ScoreCommandTests"/> class
    /// </summary>
    public ScoreCommandTests()
    {
        _scoreCommand = new();
    }

    #endregion Constructors

    [Fact]
    public void Constructor_When_PlayerNameMissing_Should_InitEmptyPlayerName()
    {
        // Arrange & Act &Assert
        Assert.True(_scoreCommand.PlayerName.IsNullOrEmpty());
    }

    [Fact]
    public void IsValid_When_PlayerNameIsEmpty_Should_ReturnBadRequestStatus()
    {
        // Arrange
        int expectedStatus = StatusCodes.Status400BadRequest;

        // Act
        int result = _scoreCommand.IsValid();

        // Assert
        Assert.True(expectedStatus.Equals(result));
    }

    [Theory]
    [InlineData("SELECT")]
    [InlineData("insert into table")]
    [InlineData("DROP DATABASE")]
    [InlineData("delete * from")]
    [InlineData("update users")]
    [InlineData("-- commentaire")]
    [InlineData("Nom;")]
    [InlineData("L'utilisateur")]
    [InlineData("'")]
    [InlineData("\"")]
    [InlineData(";")]
    [InlineData("\\")]
    [InlineData("-")]
    [InlineData("Nom d'utilisateur\"")]
    [InlineData("Hello\\World")]
    [InlineData("Test-Input")]
    [InlineData("\u0000")]
    [InlineData("Test\u200BInvisible")]
    [InlineData("Attention\u202E")]
    [InlineData("Séparateur\uFEFF")]
    public void IsValid_When_PlayerNameContainsForbid_Should_ReturnForbidden(string forbiddenChar)
    {
        // Arrange
        ScoreCommand scoreCommand = new ScoreCommand { PlayerName = forbiddenChar };
        int expectedStatus = StatusCodes.Status403Forbidden;

        // Act
        int result = scoreCommand.IsValid();

        // Assert
        Assert.True(expectedStatus.Equals(result));
    }

    [Fact]
    public void IsValid_Should_ReturnOK()
    {
        // Arrange
        ScoreCommand scoreCommand = new ScoreCommand { PlayerName = "playerName" };
        int expectedStatus = StatusCodes.Status200OK;

        // Act
        int result = scoreCommand.IsValid();

        // Assert
        Assert.True(expectedStatus.Equals(result));
    }
}
