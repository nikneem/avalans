using Bexter.Avalans.Core;

namespace Bexter.Avalans.Locations.Features.GetAllLocations;

/// <summary>
/// Query to retrieve all locations.
/// </summary>
public sealed record GetAllLocationsQuery(bool IncludeInactive = false) : IQuery;
