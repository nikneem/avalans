using Bexter.Avalans.Core;

namespace Bexter.Avalans.Locations.Features.UpdateLocation;

/// <summary>
/// Command to update an existing location.
/// </summary>
public sealed record UpdateLocationCommand(
    Guid Id,
    string Name,
    string Number,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string? State,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone) : ICommand;
