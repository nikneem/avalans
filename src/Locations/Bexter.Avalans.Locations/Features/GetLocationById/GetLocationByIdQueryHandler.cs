using Bexter.Avalans.Core;
using Bexter.Avalans.Locations.Dtos;
using Bexter.Avalans.Locations.Repositories;

namespace Bexter.Avalans.Locations.Features.GetLocationById;

/// <summary>
/// Handler for GetLocationByIdQuery.
/// Returns null if location is not found.
/// </summary>
public sealed class GetLocationByIdQueryHandler : IQueryHandler<GetLocationByIdQuery, LocationDto?>
{
    private readonly ILocationRepository _repository;

    public GetLocationByIdQueryHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<LocationDto?> Handle(GetLocationByIdQuery query, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(query.Id, cancellationToken);

        if (location is null)
            return null;

        return new LocationDto(
            location.Id,
            location.Name,
            location.Number,
            new AddressDto(
                location.Address.Street,
                location.Address.City,
                location.Address.PostalCode,
                location.Address.Country,
                location.Address.State),
            location.ContactName,
            location.ContactEmail,
            location.ContactPhone,
            location.IsActive,
            location.CreatedAt,
            location.UpdatedAt);
    }
}
