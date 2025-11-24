using Bexter.Avalans.Core;
using Bexter.Avalans.Locations.Dtos;
using Bexter.Avalans.Locations.Repositories;

namespace Bexter.Avalans.Locations.Features.GetAllLocations;

/// <summary>
/// Handler for GetAllLocationsQuery.
/// Maps domain entities to DTOs for API consumption.
/// </summary>
public sealed class GetAllLocationsQueryHandler : IQueryHandler<GetAllLocationsQuery, IEnumerable<LocationDto>>
{
    private readonly ILocationRepository _repository;

    public GetAllLocationsQueryHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LocationDto>> Handle(GetAllLocationsQuery query, CancellationToken cancellationToken)
    {
        var locations = await _repository.GetAllAsync(query.IncludeInactive, cancellationToken);

        return locations.Select(l => new LocationDto(
            l.Id,
            l.Name,
            l.Number,
            new AddressDto(
                l.Address.Street,
                l.Address.City,
                l.Address.PostalCode,
                l.Address.Country,
                l.Address.State),
            l.ContactName,
            l.ContactEmail,
            l.ContactPhone,
            l.IsActive,
            l.CreatedAt,
            l.UpdatedAt));
    }
}
