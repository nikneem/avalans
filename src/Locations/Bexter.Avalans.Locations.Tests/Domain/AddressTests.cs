using Bexter.Avalans.Locations.Domain;

namespace Bexter.Avalans.Locations.Tests.Domain;

/// <summary>
/// Tests for the Address value object.
/// </summary>
public class AddressTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var address = Address.Create("123 Main St", "City", "12345", "Country", "State");

        // Assert
        Assert.NotNull(address);
        Assert.Equal("123 Main St", address.Street);
        Assert.Equal("City", address.City);
        Assert.Equal("12345", address.PostalCode);
        Assert.Equal("Country", address.Country);
        Assert.Equal("State", address.State);
    }

    [Fact]
    public void Create_WithoutState_ShouldSucceed()
    {
        // Arrange & Act
        var address = Address.Create("123 Main St", "City", "12345", "Country");

        // Assert
        Assert.NotNull(address);
        Assert.Null(address.State);
    }

    [Fact]
    public void Create_WithNullStreet_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Address.Create(null!, "City", "12345", "Country"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyStreet_ShouldThrow(string street)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Address.Create(street, "City", "12345", "Country"));
    }

    [Fact]
    public void Create_WithNullCity_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Address.Create("123 Main St", null!, "12345", "Country"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCity_ShouldThrow(string city)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Address.Create("123 Main St", city, "12345", "Country"));
    }

    [Fact]
    public void Create_WithNullPostalCode_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Address.Create("123 Main St", "City", null!, "Country"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyPostalCode_ShouldThrow(string postalCode)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Address.Create("123 Main St", "City", postalCode, "Country"));
    }

    [Fact]
    public void Create_WithNullCountry_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Address.Create("123 Main St", "City", "12345", null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCountry_ShouldThrow(string country)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Address.Create("123 Main St", "City", "12345", country));
    }

    [Fact]
    public void Create_WithStreetTooLong_ShouldThrow()
    {
        // Arrange
        var longStreet = new string('A', 201);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            Address.Create(longStreet, "City", "12345", "Country"));
    }

    [Fact]
    public void Create_WithCityTooLong_ShouldThrow()
    {
        // Arrange
        var longCity = new string('A', 101);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            Address.Create("123 Main St", longCity, "12345", "Country"));
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var address = Address.Create("123 Main St", "City", "12345", "Country", "State");

        // Act
        var result = address.ToString();

        // Assert
        Assert.Contains("123 Main St", result);
        Assert.Contains("City", result);
        Assert.Contains("12345", result);
        Assert.Contains("Country", result);
        Assert.Contains("State", result);
    }
}
