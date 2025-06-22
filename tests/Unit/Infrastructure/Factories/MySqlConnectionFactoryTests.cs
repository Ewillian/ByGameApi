using ByGameApi.Infrastructure.Factories;
using ByGameApi.Infrastructure.Options;

using Microsoft.Extensions.Options;

using Moq;

using MySqlConnector;

using Xunit;

namespace ByGameApi.Infrastructure.Tests.Unit.Factories;

public class MySqlConnectionFactoryTests
{
    [Fact]
    public void Constructor_When_ArgumentIsMissing_Should_ReturnArgumentNullException()
    {
        // Arrange & Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            var factory = new MySqlConnectionFactory(null!);
        });

        //Assert
        Assert.Equal("options", exception.ParamName);
    }

    [Fact]
    public void CreateConnection_Should_Return_MySqlConnection_With_Valid_ConnectionString()
    {
        // Arrange
        var _databaseOptions = new DatabaseOptions
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

        var optionsMock = Mock.Of<IOptions<DatabaseOptions>>(opt => opt.Value == _databaseOptions);
        var factory = new MySqlConnectionFactory(optionsMock);

        // Act
        var connection = factory.CreateConnection();

        // Assert
        Assert.NotNull(connection);
        Assert.IsType<MySqlConnection>(connection);
        Assert.Contains($"Server={_databaseOptions.Server}", connection.ConnectionString);
        Assert.Contains($"Port={_databaseOptions.Port}", connection.ConnectionString);
        Assert.Contains($"Database={_databaseOptions.Database}", connection.ConnectionString);
        Assert.Contains($"User={_databaseOptions.User}", connection.ConnectionString);
        Assert.Contains($"Password={_databaseOptions.Password}", connection.ConnectionString);
    }
}