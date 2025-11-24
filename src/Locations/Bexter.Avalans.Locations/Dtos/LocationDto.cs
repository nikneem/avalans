namespace Bexter.Avalans.Locations.Dtos;

/// <summary>
/// DTO for location data returned in queries.
/// </summary>
public sealed record LocationDto(
    Guid Id,
    string Name,
    string Number,
    AddressDto Address,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country,
    string? State);
