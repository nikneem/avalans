using Bexter.Avalans.Items.Domain;
using Bexter.Avalans.Items.Features.UpdateItem;
using Bexter.Avalans.Items.Repositories;
using Bexter.Avalans.Items.Tests.Factories;
using Moq;

namespace Bexter.Avalans.Items.Tests.Features;

/// <summary>
/// Tests for UpdateItemCommandHandler.
/// Validates update orchestration and domain interaction.
/// </summary>
public class UpdateItemCommandHandlerTests
{
    private readonly Mock<IItemRepository> _repositoryMock;
    private readonly UpdateItemCommandHandler _handler;

    public UpdateItemCommandHandlerTests()
    {
        _repositoryMock = new Mock<IItemRepository>();
        _handler = new UpdateItemCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_UpdatesItemAndSaves()
    {
        // Arrange
        var existingItem = ItemFactory.Create("Original Name");
        var itemId = existingItem.Id;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        var command = new UpdateItemCommand(
            itemId,
            "Updated Name",
            "Updated Description",
            Length: 60m,
            Width: 50m,
            Height: 40m,
            Weight: 20m,
            IsPerishable: false,
            ShelfLifeDays: null,
            Value: 200m);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Updated Name", existingItem.Name);
        Assert.Equal("Updated Description", existingItem.Description);
        Assert.Equal(60m, existingItem.Length);
        Assert.Equal(50m, existingItem.Width);
        Assert.Equal(40m, existingItem.Height);
        Assert.Equal(20m, existingItem.Weight);
        Assert.Equal(200m, existingItem.Value);

        _repositoryMock.Verify(
            r => r.UpdateAsync(existingItem, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ThrowsDomainException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        var command = new UpdateItemCommand(
            nonExistentId,
            "Name",
            null, null, null, null, null,
            false, null, null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Contains(nonExistentId.ToString(), exception.Message);
        Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);

        _repositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ChangingToPerishable_SetsShelfLife()
    {
        // Arrange
        var existingItem = ItemFactory.Create(isPerishable: false);
        var itemId = existingItem.Id;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        var command = new UpdateItemCommand(
            itemId,
            "Perishable Item",
            null, null, null, null, null,
            IsPerishable: true,
            ShelfLifeDays: 14,
            Value: null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(existingItem.IsPerishable);
        Assert.Equal(14, existingItem.ShelfLife);

        _repositoryMock.Verify(
            r => r.UpdateAsync(existingItem, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ChangingToNonPerishable_ClearsShelfLife()
    {
        // Arrange
        var existingItem = ItemFactory.CreatePerishable(7);
        var itemId = existingItem.Id;
        Assert.True(existingItem.IsPerishable);
        Assert.Equal(7, existingItem.ShelfLife);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        var command = new UpdateItemCommand(
            itemId,
            "Non-Perishable Item",
            null, null, null, null, null,
            IsPerishable: false,
            ShelfLifeDays: null,
            Value: null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(existingItem.IsPerishable);
        Assert.Null(existingItem.ShelfLife);

        _repositoryMock.Verify(
            r => r.UpdateAsync(existingItem, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithPerishableButNoShelfLife_ThrowsDomainException()
    {
        // Arrange
        var existingItem = ItemFactory.Create(isPerishable: false);
        var itemId = existingItem.Id;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        var command = new UpdateItemCommand(
            itemId,
            "Invalid",
            null, null, null, null, null,
            IsPerishable: true,
            ShelfLifeDays: null,  // Missing!
            Value: null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("shelf life", exception.Message, StringComparison.OrdinalIgnoreCase);

        _repositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithPartialDimensions_DoesNotUpdateDimensions()
    {
        // Arrange
        var existingItem = ItemFactory.Create();
        existingItem.SetDimensions(100m, 100m, 100m);
        var itemId = existingItem.Id;
        var originalVolume = existingItem.Volume;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        var command = new UpdateItemCommand(
            itemId,
            "Partial Update",
            null,
            Length: 50m,
            Width: 40m,
            Height: null,  // Missing!
            Weight: null,
            false, null, null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - Dimensions should remain unchanged
        Assert.Equal(100m, existingItem.Length);
        Assert.Equal(100m, existingItem.Width);
        Assert.Equal(100m, existingItem.Height);
        Assert.Equal(originalVolume, existingItem.Volume);
    }

    [Fact]
    public async Task Handle_WithAllDimensions_UpdatesDimensionsAndRecalculatesVolume()
    {
        // Arrange
        var existingItem = ItemFactory.Create();
        existingItem.SetDimensions(100m, 100m, 100m);
        var itemId = existingItem.Id;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        var command = new UpdateItemCommand(
            itemId,
            "Updated Dimensions",
            null,
            Length: 20m,
            Width: 10m,
            Height: 5m,
            Weight: null,
            false, null, null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(20m, existingItem.Length);
        Assert.Equal(10m, existingItem.Width);
        Assert.Equal(5m, existingItem.Height);
        Assert.Equal(1000m, existingItem.Volume);  // 20 * 10 * 5
    }

    [Fact]
    public async Task Handle_PassesCancellationToken_ToRepository()
    {
        // Arrange
        var existingItem = ItemFactory.CreateMinimal();
        var itemId = existingItem.Id;
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(itemId, token))
            .ReturnsAsync(existingItem);

        var command = new UpdateItemCommand(
            itemId, "Test", null, null, null, null, null,
            false, null, null);

        // Act
        await _handler.Handle(command, token);

        // Assert
        _repositoryMock.Verify(
            r => r.GetByIdAsync(itemId, token),
            Times.Once);
        _repositoryMock.Verify(
            r => r.UpdateAsync(existingItem, token),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UpdatesTimestamp()
    {
        // Arrange
        var existingItem = ItemFactory.CreateMinimal();
        var itemId = existingItem.Id;
        var originalUpdatedAt = existingItem.UpdatedAt;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        var command = new UpdateItemCommand(
            itemId,
            "Updated",
            "New Description",
            null, null, null, null,
            false, null, null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(existingItem.UpdatedAt);
        Assert.NotEqual(originalUpdatedAt, existingItem.UpdatedAt);
    }
}
