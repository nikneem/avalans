using Bexter.Avalans.Core;

namespace Bexter.Avalans.Locations.Features.CreateLocation;

/// <summary>
/// Command to create a new location.
/// </summary>
public sealed record CreateLocationCommand(
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
