using Bexter.Avalans.Locations.Domain;
using Bexter.Avalans.Locations.Features.UpdateLocation;
using Bexter.Avalans.Locations.Repositories;
using Bexter.Avalans.Locations.Tests.Factories;
using Moq;

namespace Bexter.Avalans.Locations.Tests.Features;

/// <summary>
/// Tests for UpdateLocationCommandHandler.
/// </summary>
public class UpdateLocationCommandHandlerTests
{
    private readonly Mock<ILocationRepository> _mockRepository;
    private readonly UpdateLocationCommandHandler _handler;

    public UpdateLocationCommandHandlerTests()
    {
        _mockRepository = new Mock<ILocationRepository>();
        _handler = new UpdateLocationCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateLocation()
    {
        // Arrange
        var location = LocationFactory.Create();
        var command = new UpdateLocationCommand(
            location.Id,
            "Updated Name",
            "NEWNUM",
            "456 New St",
            "NewCity",
            "54321",
            "NewCountry",
            "NewState",
            "Jane Doe",
            "jane@example.com",
            "555-5678");

        _mockRepository.Setup(r => r.GetByIdAsync(location.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);
        _mockRepository.Setup(r => r.GetByNumberAsync("NEWNUM", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Updated Name", location.Name);
        Assert.Equal("NEWNUM", location.Number);
        Assert.Equal("456 New St", location.Address.Street);
        Assert.Equal("Jane Doe", location.ContactName);
        _mockRepository.Verify(r => r.UpdateAsync(location, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentLocation_ShouldThrowDomainException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateLocationCommand(
            nonExistentId,
            "Updated Name",
            "NUM001",
            "456 New St",
            "City",
            "54321",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));
        Assert.Contains("not found", ex.Message);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDuplicateNumber_ShouldThrowDomainException()
    {
        // Arrange
        var location = LocationFactory.Create();
        var otherLocation = LocationFactory.Create();
        var command = new UpdateLocationCommand(
            location.Id,
            "Updated Name",
            otherLocation.Number,  // Using another location's number
            "456 New St",
            "City",
            "54321",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByIdAsync(location.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);
        _mockRepository.Setup(r => r.GetByNumberAsync(otherLocation.Number, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherLocation);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));
        Assert.Contains("already exists", ex.Message);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithSameNumber_ShouldNotCheckDuplicates()
    {
        // Arrange
        var location = LocationFactory.Create();
        var command = new UpdateLocationCommand(
            location.Id,
            "Updated Name",
            location.Number,  // Same number as current
            "456 New St",
            "City",
            "54321",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByIdAsync(location.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.GetByNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepository.Verify(r => r.UpdateAsync(location, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var location = LocationFactory.Create();
        var command = new UpdateLocationCommand(
            location.Id,
            null!,
            "NUM001",
            "456 New St",
            "City",
            "54321",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByIdAsync(location.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(command, CancellationToken.None));
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("", "NUM001")]
    [InlineData("   ", "NUM001")]
    [InlineData("Name", "")]
    [InlineData("Name", "   ")]
    public async Task Handle_WithEmptyNameOrNumber_ShouldThrowArgumentException(string name, string number)
    {
        // Arrange
        var location = LocationFactory.Create();
        var command = new UpdateLocationCommand(
            location.Id,
            name,
            number,
            "456 New St",
            "City",
            "54321",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByIdAsync(location.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullContactInfo_ShouldClearContactDetails()
    {
        // Arrange
        var location = LocationFactory.CreateWithContact();
        var command = new UpdateLocationCommand(
            location.Id,
            "Updated Name",
            "NUM001",
            "456 New St",
            "City",
            "54321",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByIdAsync(location.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);
        _mockRepository.Setup(r => r.GetByNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(location.ContactName);
        Assert.Null(location.ContactEmail);
        Assert.Null(location.ContactPhone);
    }
}
