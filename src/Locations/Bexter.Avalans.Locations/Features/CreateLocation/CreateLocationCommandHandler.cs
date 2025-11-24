using Bexter.Avalans.Core;
using Bexter.Avalans.Locations.Domain;
using Bexter.Avalans.Locations.Repositories;

namespace Bexter.Avalans.Locations.Features.CreateLocation;

/// <summary>
/// Handler for CreateLocationCommand.
/// Orchestrates location creation through the domain model.
/// </summary>
public sealed class CreateLocationCommandHandler : ICommandHandler<CreateLocationCommand, CreateLocationResult>
{
    private readonly ILocationRepository _repository;

    public CreateLocationCommandHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateLocationResult> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        // Check if location number already exists
        var existing = await _repository.GetByNumberAsync(command.Number, cancellationToken);
        if (existing is not null)
        {
            throw new DomainException($"A location with number '{command.Number}' already exists");
        }

        // Create address value object through domain model factory (validates business rules)
        var address = Address.Create(
            command.Street,
            command.City,
            command.PostalCode,
            command.Country,
            command.State);

        // Create location through domain model factory method (validates business rules)
        var location = Location.Create(command.Name, command.Number, address);

        // Set optional contact information if provided
        if (!string.IsNullOrWhiteSpace(command.ContactName) ||
            !string.IsNullOrWhiteSpace(command.ContactEmail) ||
            !string.IsNullOrWhiteSpace(command.ContactPhone))
        {
            location.SetContactInfo(command.ContactName, command.ContactEmail, command.ContactPhone);
        }

        // Persist through repository
        var locationId = await _repository.AddAsync(location, cancellationToken);

        return new CreateLocationResult(locationId);
    }
}
