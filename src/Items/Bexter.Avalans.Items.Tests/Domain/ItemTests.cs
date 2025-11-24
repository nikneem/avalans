using Bexter.Avalans.Items.Domain;
using Bexter.Avalans.Items.Tests.Factories;

namespace Bexter.Avalans.Items.Tests.Domain;

/// <summary>
/// Tests for the Item domain entity.
/// Validates business rules and domain logic encapsulation.
/// </summary>
public class ItemTests
{
    [Fact]
    public void Create_WithValidName_CreatesItem()
    {
        // Arrange & Act
        var item = Item.Create("Test Item", "Test Description");

        // Assert
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal("Test Item", item.Name);
        Assert.Equal("Test Description", item.Description);
        Assert.True(item.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ThrowsArgumentException(string? invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Item.Create(invalidName!));
    }

    [Fact]
    public void Create_WithNullName_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Item.Create(null!));
    }

    [Fact]
    public void Create_WithTooLongName_ThrowsDomainException()
    {
        // Arrange
        var longName = new string('A', 201);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => Item.Create(longName));
        Assert.Contains("200", exception.Message);
    }

    [Fact]
    public void UpdateBasicInfo_WithValidData_UpdatesProperties()
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();
        var originalCreatedAt = item.CreatedAt;

        // Act
        item.UpdateBasicInfo("Updated Name", "Updated Description");

        // Assert
        Assert.Equal("Updated Name", item.Name);
        Assert.Equal("Updated Description", item.Description);
        Assert.NotNull(item.UpdatedAt);
        Assert.True(item.UpdatedAt >= originalCreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateBasicInfo_WithEmptyName_ThrowsArgumentException(string? invalidName)
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => item.UpdateBasicInfo(invalidName!, "Description"));
    }

    [Fact]
    public void UpdateBasicInfo_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => item.UpdateBasicInfo(null!, "Description"));
    }

    [Fact]
    public void SetDimensions_WithValidValues_SetsDimensionsAndCalculatesVolume()
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act
        item.SetDimensions(10m, 20m, 5m);

        // Assert
        Assert.Equal(10m, item.Length);
        Assert.Equal(20m, item.Width);
        Assert.Equal(5m, item.Height);
        Assert.Equal(1000m, item.Volume); // 10 * 20 * 5
        Assert.Equal("cm³", item.VolumeUnit);
    }

    [Theory]
    [InlineData(0, 10, 10)]
    [InlineData(10, 0, 10)]
    [InlineData(10, 10, 0)]
    [InlineData(-1, 10, 10)]
    public void SetDimensions_WithInvalidValues_ThrowsDomainException(decimal length, decimal width, decimal height)
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => item.SetDimensions(length, width, height));
        Assert.Contains("greater than zero", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SetWeight_WithValidValue_SetsWeight()
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act
        item.SetWeight(5.5m);

        // Assert
        Assert.Equal(5.5m, item.Weight);
        Assert.Equal("kg", item.WeightUnit);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.1)]
    public void SetWeight_WithInvalidValue_ThrowsDomainException(decimal weight)
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => item.SetWeight(weight));
        Assert.Contains("greater than zero", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SetPerishability_WithPerishableAndShelfLife_SetsProperties()
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act
        item.SetPerishability(true, 7);

        // Assert
        Assert.True(item.IsPerishable);
        Assert.Equal(7, item.ShelfLife);
    }

    [Fact]
    public void SetPerishability_WithPerishableButNoShelfLife_ThrowsDomainException()
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => item.SetPerishability(true, null));
        Assert.Contains("shelf life", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SetPerishability_WithNonPerishable_ClearsShelfLife()
    {
        // Arrange
        var item = ItemFactory.CreatePerishable(7);
        Assert.True(item.IsPerishable);
        Assert.Equal(7, item.ShelfLife);

        // Act
        item.SetPerishability(false, null);

        // Assert
        Assert.False(item.IsPerishable);
        Assert.Null(item.ShelfLife);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SetPerishability_WithInvalidShelfLife_ThrowsDomainException(int shelfLife)
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => item.SetPerishability(true, shelfLife));
        Assert.Contains("positive", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SetValue_WithValidValue_SetsValueAndCurrency()
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act
        item.SetValue(100.50m);

        // Assert
        Assert.Equal(100.50m, item.Value);
        Assert.Equal("USD", item.Currency);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void SetValue_WithInvalidValue_ThrowsDomainException(decimal value)
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => item.SetValue(value));
        Assert.Contains("cannot be negative", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SetCategory_WithValidCategory_SetsCategory()
    {
        // Arrange
        var item = ItemFactory.CreateMinimal();

        // Act
        item.SetCategory("Electronics");

        // Assert
        Assert.Equal("Electronics", item.Category);
    }
}
