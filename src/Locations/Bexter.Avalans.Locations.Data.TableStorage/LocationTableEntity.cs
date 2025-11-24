using Azure;
using Azure.Data.Tables;
using Bexter.Avalans.Locations.Domain;

namespace Bexter.Avalans.Locations.Data.TableStorage;

/// <summary>
/// Table Storage entity for Location.
/// Uses PartitionKey for tenant/logical grouping and RowKey as the Location ID.
/// Includes a secondary index field (Number) for business key lookups.
/// </summary>
public sealed class LocationTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "LOCATION";
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // Location properties
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    
    // Address properties (flattened from value object)
    public string AddressStreet { get; set; } = string.Empty;
    public string AddressCity { get; set; } = string.Empty;
    public string AddressPostalCode { get; set; } = string.Empty;
    public string AddressCountry { get; set; } = string.Empty;
    public string? AddressState { get; set; }
    
    // Contact properties
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    
    // Metadata
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Creates a TableEntity from a domain Location entity.
    /// </summary>
    public static LocationTableEntity FromDomainEntity(Location location)
    {
        return new LocationTableEntity
        {
            RowKey = location.Id.ToString(),
            Name = location.Name,
            Number = location.Number,
            AddressStreet = location.Address.Street,
            AddressCity = location.Address.City,
            AddressPostalCode = location.Address.PostalCode,
            AddressCountry = location.Address.Country,
            AddressState = location.Address.State,
            ContactName = location.ContactName,
            ContactEmail = location.ContactEmail,
            ContactPhone = location.ContactPhone,
            IsActive = location.IsActive,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }

    /// <summary>
    /// Reconstructs a domain Location entity from storage.
    /// Uses reflection to bypass private setters for persistence reconstruction.
    /// </summary>
    public Location ToDomainEntity()
    {
        // Create Address value object
        var address = Address.Create(
            AddressStreet,
            AddressCity,
            AddressPostalCode,
            AddressCountry,
            AddressState);

        // Create uninitialized Location instance to bypass constructor validation
        var location = (Location)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Location));

        // Use reflection to set private properties
        typeof(Location).GetProperty(nameof(Location.Id))!.SetValue(location, Guid.Parse(RowKey));
        typeof(Location).GetProperty(nameof(Location.Name))!.SetValue(location, Name);
        typeof(Location).GetProperty(nameof(Location.Number))!.SetValue(location, Number);
        typeof(Location).GetProperty(nameof(Location.Address))!.SetValue(location, address);
        typeof(Location).GetProperty(nameof(Location.ContactName))!.SetValue(location, ContactName);
        typeof(Location).GetProperty(nameof(Location.ContactEmail))!.SetValue(location, ContactEmail);
        typeof(Location).GetProperty(nameof(Location.ContactPhone))!.SetValue(location, ContactPhone);
        typeof(Location).GetProperty(nameof(Location.IsActive))!.SetValue(location, IsActive);
        typeof(Location).GetProperty(nameof(Location.CreatedAt))!.SetValue(location, CreatedAt);
        typeof(Location).GetProperty(nameof(Location.UpdatedAt))!.SetValue(location, UpdatedAt);

        return location;
    }
}
