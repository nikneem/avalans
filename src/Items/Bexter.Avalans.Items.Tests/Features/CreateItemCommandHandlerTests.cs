using Bexter.Avalans.Items.Domain;
using Bexter.Avalans.Items.Features.CreateItem;
using Bexter.Avalans.Items.Repositories;
using Moq;

namespace Bexter.Avalans.Items.Tests.Features;

/// <summary>
/// Tests for CreateItemCommandHandler.
/// Validates command handling orchestration and domain interaction.
/// </summary>
public class CreateItemCommandHandlerTests
{
    private readonly Mock<IItemRepository> _repositoryMock;
    private readonly CreateItemCommandHandler _handler;

    public CreateItemCommandHandlerTests()
    {
        _repositoryMock = new Mock<IItemRepository>();
        _handler = new CreateItemCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesItemAndReturnsResult()
    {
        // Arrange
        var command = new CreateItemCommand(
            "Test Item",
            "Test Description",
            Length: 50m,
            Width: 40m,
            Height: 30m,
            Weight: 10m,
            IsPerishable: false,
            ShelfLifeDays: null,
            Value: 100m);

        var expectedId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result.ItemId);
        _repositoryMock.Verify(
            r => r.AddAsync(
                It.Is<Item>(i =>
                    i.Name == "Test Item" &&
                    i.Description == "Test Description" &&
                    i.Length == 50m &&
                    i.Width == 40m &&
                    i.Height == 30m &&
                    i.Weight == 10m &&
                    !i.IsPerishable &&
                    i.Value == 100m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithMinimalCommand_CreatesItemWithRequiredFieldsOnly()
    {
        // Arrange
        var command = new CreateItemCommand(
            "Minimal Item",
            Description: null,
            Length: null,
            Width: null,
            Height: null,
            Weight: null,
            IsPerishable: false,
            ShelfLifeDays: null,
            Value: null);

        var expectedId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result.ItemId);
        _repositoryMock.Verify(
            r => r.AddAsync(
                It.Is<Item>(i => i.Name == "Minimal Item"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithPerishableItem_SetsShelfLife()
    {
        // Arrange
        var command = new CreateItemCommand(
            "Perishable Item",
            "Fresh produce",
            Length: null,
            Width: null,
            Height: null,
            Weight: null,
            IsPerishable: true,
            ShelfLifeDays: 7,
            Value: null);

        var expectedId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result.ItemId);
        _repositoryMock.Verify(
            r => r.AddAsync(
                It.Is<Item>(i => i.IsPerishable && i.ShelfLife == 7),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithPerishableButNoShelfLife_ThrowsDomainException()
    {
        // Arrange
        var command = new CreateItemCommand(
            "Invalid Perishable",
            Description: null,
            Length: null,
            Width: null,
            Height: null,
            Weight: null,
            IsPerishable: true,
            ShelfLifeDays: null,  // Missing shelf life!
            Value: null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("shelf life", exception.Message, StringComparison.OrdinalIgnoreCase);

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithPartialDimensions_DoesNotSetDimensions()
    {
        // Arrange - Only 2 out of 3 dimensions provided
        var command = new CreateItemCommand(
            "Partial Dimensions",
            Description: null,
            Length: 50m,
            Width: 40m,
            Height: null,  // Missing!
            Weight: null,
            IsPerishable: false,
            ShelfLifeDays: null,
            Value: null);

        var expectedId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result.ItemId);
        _repositoryMock.Verify(
            r => r.AddAsync(
                It.Is<Item>(i => i.Length == null && i.Width == null && i.Height == null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithAllDimensions_SetsDimensionsAndCalculatesVolume()
    {
        // Arrange
        var command = new CreateItemCommand(
            "Complete Dimensions",
            Description: null,
            Length: 10m,
            Width: 20m,
            Height: 5m,
            Weight: null,
            IsPerishable: false,
            ShelfLifeDays: null,
            Value: null);

        var expectedId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result.ItemId);
        _repositoryMock.Verify(
            r => r.AddAsync(
                It.Is<Item>(i =>
                    i.Length == 10m &&
                    i.Width == 20m &&
                    i.Height == 5m &&
                    i.Volume == 1000m),  // Auto-calculated
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_PassesCancellationToken_ToRepository()
    {
        // Arrange
        var command = new CreateItemCommand(
            "Test",
            null, null, null, null, null, false, null, null);

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), token))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        await _handler.Handle(command, token);

        // Assert
        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Item>(), token),
            Times.Once);
    }
}
