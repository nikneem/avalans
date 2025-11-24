using Bexter.Avalans.Core;
using Bexter.Avalans.Locations.Domain;
using Bexter.Avalans.Locations.Repositories;

namespace Bexter.Avalans.Locations.Features.UpdateLocation;

/// <summary>
/// Handler for UpdateLocationCommand.
/// Orchestrates location updates through the domain model.
/// </summary>
public sealed class UpdateLocationCommandHandler : ICommandHandler<UpdateLocationCommand>
{
    private readonly ILocationRepository _repository;

    public UpdateLocationCommandHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        // Load aggregate root
        var location = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (location is null)
        {
            throw new DomainException($"Location with ID {command.Id} not found");
        }

        // Check if number is being changed and if new number already exists
        if (location.Number != command.Number)
        {
            var existingWithNumber = await _repository.GetByNumberAsync(command.Number, cancellationToken);
            if (existingWithNumber is not null && existingWithNumber.Id != command.Id)
            {
                throw new DomainException($"A location with number '{command.Number}' already exists");
            }
        }

        // Update through domain methods (validates business rules)
        location.UpdateBasicInfo(command.Name, command.Number);

        // Update address
        var address = Address.Create(
            command.Street,
            command.City,
            command.PostalCode,
            command.Country,
            command.State);
        location.UpdateAddress(address);

        // Update contact information
        location.SetContactInfo(command.ContactName, command.ContactEmail, command.ContactPhone);

        // Persist valid state
        await _repository.UpdateAsync(location, cancellationToken);
    }
}
