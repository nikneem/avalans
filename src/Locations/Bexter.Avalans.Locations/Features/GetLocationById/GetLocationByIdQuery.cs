using Bexter.Avalans.Core;

namespace Bexter.Avalans.Locations.Features.GetLocationById;

/// <summary>
/// Query to retrieve a location by its unique identifier.
/// </summary>
public sealed record GetLocationByIdQuery(Guid Id) : IQuery;
