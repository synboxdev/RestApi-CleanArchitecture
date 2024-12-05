using Hestia.Persistence.Utility.Helpers;

namespace Hestia.Persistence.Tests.Utility.Helpers;

public class StringHelperTests
{
    [Fact]
    public void Hash_ShouldReturnHashedString()
    {
        // Arrange
        string inputString = "testString";

        // Act
        string hashedString = inputString.Hash();

        // Assert
        Assert.NotNull(hashedString);
        Assert.NotEqual(inputString, hashedString);
        Assert.True(BCrypt.Net.BCrypt.Verify(inputString, hashedString));
    }

    [Fact]
    public void Verify_ShouldReturnTrue_ForMatchingStrings()
    {
        // Arrange
        string inputString = "testString";
        string hashedString = inputString.Hash();

        // Act
        bool isVerified = inputString.Verify(hashedString);

        // Assert
        Assert.True(isVerified);
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForNonMatchingStrings()
    {
        // Arrange
        string inputString = "testString";
        string differentString = "differentString";
        string hashedString = inputString.Hash();

        // Act
        bool isVerified = differentString.Verify(hashedString);

        // Assert
        Assert.False(isVerified);
    }
}