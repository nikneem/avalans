using Bexter.Avalans.Locations.Domain;
using Bexter.Avalans.Locations.Tests.Factories;

namespace Bexter.Avalans.Locations.Tests.Domain;

/// <summary>
/// Tests for the Location domain entity.
/// </summary>
public class LocationTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var address = Address.Create("123 Main St", "City", "12345", "Country");

        // Act
        var location = Location.Create("Client A", "LOC001", address);

        // Assert
        Assert.NotEqual(Guid.Empty, location.Id);
        Assert.Equal("Client A", location.Name);
        Assert.Equal("LOC001", location.Number);
        Assert.Equal(address, location.Address);
        Assert.True(location.IsActive);
        Assert.True(location.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithNullName_ShouldThrow()
    {
        // Arrange
        var address = Address.Create("123 Main St", "City", "12345", "Country");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Location.Create(null!, "LOC001", address));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string name)
    {
        // Arrange
        var address = Address.Create("123 Main St", "City", "12345", "Country");

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Location.Create(name, "LOC001", address));
    }

    [Fact]
    public void Create_WithNullNumber_ShouldThrow()
    {
        // Arrange
        var address = Address.Create("123 Main St", "City", "12345", "Country");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Location.Create("Client A", null!, address));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyNumber_ShouldThrow(string number)
    {
        // Arrange
        var address = Address.Create("123 Main St", "City", "12345", "Country");

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Location.Create("Client A", number, address));
    }

    [Fact]
    public void Create_WithNullAddress_ShouldThrow()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            Location.Create("Client A", "LOC001", null!));
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrow()
    {
        // Arrange
        var address = Address.Create("123 Main St", "City", "12345", "Country");
        var longName = new string('A', 201);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            Location.Create(longName, "LOC001", address));
    }

    [Fact]
    public void Create_WithNumberTooLong_ShouldThrow()
    {
        // Arrange
        var address = Address.Create("123 Main St", "City", "12345", "Country");
        var longNumber = new string('A', 51);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            Location.Create("Client A", longNumber, address));
    }

    [Fact]
    public void UpdateBasicInfo_WithValidData_ShouldSucceed()
    {
        // Arrange
        var location = LocationFactory.Create();

        // Act
        location.UpdateBasicInfo("New Name", "NEW001");

        // Assert
        Assert.Equal("New Name", location.Name);
        Assert.Equal("NEW001", location.Number);
        Assert.NotNull(location.UpdatedAt);
    }

    [Fact]
    public void UpdateBasicInfo_WithNullName_ShouldThrow()
    {
        // Arrange
        var location = LocationFactory.Create();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            location.UpdateBasicInfo(null!, "NUM001"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateBasicInfo_WithEmptyName_ShouldThrow(string name)
    {
        // Arrange
        var location = LocationFactory.Create();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            location.UpdateBasicInfo(name, "NUM001"));
    }

    [Fact]
    public void UpdateAddress_WithValidAddress_ShouldSucceed()
    {
        // Arrange
        var location = LocationFactory.Create();
        var newAddress = Address.Create("456 New St", "NewCity", "54321", "NewCountry");

        // Act
        location.UpdateAddress(newAddress);

        // Assert
        Assert.Equal(newAddress, location.Address);
        Assert.NotNull(location.UpdatedAt);
    }

    [Fact]
    public void UpdateAddress_WithNull_ShouldThrow()
    {
        // Arrange
        var location = LocationFactory.Create();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => location.UpdateAddress(null!));
    }

    [Fact]
    public void SetContactInfo_WithValidData_ShouldSucceed()
    {
        // Arrange
        var location = LocationFactory.Create();

        // Act
        location.SetContactInfo("John Doe", "john@example.com", "555-1234");

        // Assert
        Assert.Equal("John Doe", location.ContactName);
        Assert.Equal("john@example.com", location.ContactEmail);
        Assert.Equal("555-1234", location.ContactPhone);
        Assert.NotNull(location.UpdatedAt);
    }

    [Fact]
    public void SetContactInfo_WithNullValues_ShouldSucceed()
    {
        // Arrange
        var location = LocationFactory.Create();

        // Act
        location.SetContactInfo(null, null, null);

        // Assert
        Assert.Null(location.ContactName);
        Assert.Null(location.ContactEmail);
        Assert.Null(location.ContactPhone);
    }

    [Fact]
    public void SetContactInfo_WithContactNameTooLong_ShouldThrow()
    {
        // Arrange
        var location = LocationFactory.Create();
        var longName = new string('A', 101);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            location.SetContactInfo(longName, "email@test.com", "555-1234"));
    }

    [Fact]
    public void SetContactInfo_WithContactEmailTooLong_ShouldThrow()
    {
        // Arrange
        var location = LocationFactory.Create();
        var longEmail = new string('A', 101) + "@test.com";

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            location.SetContactInfo("John", longEmail, "555-1234"));
    }

    [Fact]
    public void SetContactInfo_WithContactPhoneTooLong_ShouldThrow()
    {
        // Arrange
        var location = LocationFactory.Create();
        var longPhone = new string('1', 51);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            location.SetContactInfo("John", "email@test.com", longPhone));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var location = LocationFactory.Create();
        Assert.True(location.IsActive);

        // Act
        location.Deactivate();

        // Assert
        Assert.False(location.IsActive);
        Assert.NotNull(location.UpdatedAt);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var location = LocationFactory.CreateInactive();
        Assert.False(location.IsActive);

        // Act
        location.Activate();

        // Assert
        Assert.True(location.IsActive);
        Assert.NotNull(location.UpdatedAt);
    }
}
