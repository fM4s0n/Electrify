using Electrify.Server.Extensions;
using FluentAssertions;

namespace Electrify.Server.UnitTests.Extensions;

public class EnumerableExtensionsTests
{
    [Fact]
    public void MaxOrDefault_SourceIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<int> source = null;

        // Act
        Action act = () => source.MaxOrDefault();

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void MaxOrDefault_SourceIsEmpty_ReturnsDefault()
    {
        // Arrange
        var source = new List<int>();

        // Act
        var result = source.MaxOrDefault();

        // Assert
        result.Should().Be(default(int));
    }

    [Fact]
    public void MaxOrDefault_SingleElement_ReturnsThatElement()
    {
        // Arrange
        var source = new List<int> { 42 };

        // Act
        var result = source.MaxOrDefault();

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void MaxOrDefault_MaxValueAtBeginning_ReturnsMaximum()
    {
        // Arrange
        var source = new List<int> { 5, 1, 2, 3, 4 };

        // Act
        var result = source.MaxOrDefault();

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void MaxOrDefault_MaxValueInMiddle_ReturnsMaximum()
    {
        // Arrange
        var source = new List<int> { 1, 5, 2, 3, 4 };

        // Act
        var result = source.MaxOrDefault();

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void MaxOrDefault_MaxValueAtEnd_ReturnsMaximum()
    {
        // Arrange
        var source = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        var result = source.MaxOrDefault();

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void MaxOrDefault_AllElementsAreEqual_ReturnsThatElement()
    {
        // Arrange
        var source = new List<int> { 2, 2, 2 };

        // Act
        var result = source.MaxOrDefault();

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void MaxOrDefault_SourceContainsNulls_ReturnsMaximum()
    {
        // Arrange
        var source = new List<string> { "apple", null, "banana", null, "cherry" };

        // Act
        var result = source.MaxOrDefault();

        // Assert
        result.Should().Be("cherry");
    }

    [Fact]
    public void MaxOrDefault_SourceContainsOnlyNulls_ReturnsNull()
    {
        // Arrange
        var source = new List<string> { null, null, null };

        // Act
        var result = source.MaxOrDefault();

        // Assert
        result.Should().BeNull();
    }
}
