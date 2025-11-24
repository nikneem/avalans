using Bexter.Avalans.Locations.Domain;
using Bexter.Avalans.Locations.Features.CreateLocation;
using Bexter.Avalans.Locations.Repositories;
using Bexter.Avalans.Locations.Tests.Factories;
using Moq;

namespace Bexter.Avalans.Locations.Tests.Features;

/// <summary>
/// Tests for CreateLocationCommandHandler.
/// </summary>
public class CreateLocationCommandHandlerTests
{
    private readonly Mock<ILocationRepository> _mockRepository;
    private readonly CreateLocationCommandHandler _handler;

    public CreateLocationCommandHandlerTests()
    {
        _mockRepository = new Mock<ILocationRepository>();
        _handler = new CreateLocationCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateLocation()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var command = new CreateLocationCommand(
            "Client A",
            "LOC001",
            "123 Main St",
            "City",
            "12345",
            "Country",
            "State",
            "John Doe",
            "john@example.com",
            "555-1234");

        _mockRepository.Setup(r => r.GetByNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedId, result.LocationId);
        _mockRepository.Verify(r => r.GetByNumberAsync("LOC001", It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.Is<Location>(l =>
            l.Name == "Client A" &&
            l.Number == "LOC001" &&
            l.Address.Street == "123 Main St" &&
            l.IsActive == true &&
            l.ContactName == "John Doe"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithoutContactInfo_ShouldCreateLocationWithoutContact()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var command = new CreateLocationCommand(
            "Client A",
            "LOC001",
            "123 Main St",
            "City",
            "12345",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.AddAsync(It.Is<Location>(l =>
            l.ContactName == null &&
            l.ContactEmail == null &&
            l.ContactPhone == null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateNumber_ShouldThrowDomainException()
    {
        // Arrange
        var existingLocation = LocationFactory.Create();
        var command = new CreateLocationCommand(
            "Client B",
            existingLocation.Number,
            "456 New St",
            "City",
            "54321",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByNumberAsync(existingLocation.Number, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLocation);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));
        Assert.Contains("already exists", ex.Message);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var command = new CreateLocationCommand(
            null!,
            "LOC001",
            "123 Main St",
            "City",
            "12345",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(command, CancellationToken.None));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("", "LOC001")]
    [InlineData("   ", "LOC001")]
    [InlineData("Client A", "")]
    [InlineData("Client A", "   ")]
    public async Task Handle_WithEmptyNameOrNumber_ShouldThrowArgumentException(string name, string number)
    {
        // Arrange
        var command = new CreateLocationCommand(
            name,
            number,
            "123 Main St",
            "City",
            "12345",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Location>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullStreet_ShouldThrowArgumentNullException()
    {
        // Arrange
        var command = new CreateLocationCommand(
            "Client A",
            "LOC001",
            null!,
            "City",
            "12345",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WithEmptyStreet_ShouldThrowArgumentException(string street)
    {
        // Arrange
        var command = new CreateLocationCommand(
            "Client A",
            "LOC001",
            street,
            "City",
            "12345",
            "Country",
            null,
            null,
            null,
            null);

        _mockRepository.Setup(r => r.GetByNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
