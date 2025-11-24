namespace Bexter.Avalans.Locations.Domain;

/// <summary>
/// Represents a location (client) in the logistics system where items can be sent.
/// This is a rich domain model following DDD principles with encapsulated state and business rules.
/// Locations act as clients - items travel between locations via transactions.
/// </summary>
public class Location
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Number { get; private set; }
    public Address Address { get; private set; }
    public string? ContactName { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Private constructor for EF Core and internal use
    private Location()
    {
        Name = string.Empty;
        Number = string.Empty;
        Address = null!; // Will be set by factory or EF
    }

    /// <summary>
    /// Factory method to create a new Location with required properties.
    /// </summary>
    public static Location Create(string name, string number, Address address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(number, nameof(number));
        ArgumentNullException.ThrowIfNull(address, nameof(address));

        if (name.Length > 200)
            throw new DomainException("Location name cannot exceed 200 characters");

        if (number.Length > 50)
            throw new DomainException("Location number cannot exceed 50 characters");

        return new Location
        {
            Id = Guid.NewGuid(),
            Name = name,
            Number = number,
            Address = address,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates the basic location information.
    /// </summary>
    public void UpdateBasicInfo(string name, string number)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(number, nameof(number));

        if (name.Length > 200)
            throw new DomainException("Location name cannot exceed 200 characters");

        if (number.Length > 50)
            throw new DomainException("Location number cannot exceed 50 characters");

        Name = name;
        Number = number;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the location's address.
    /// </summary>
    public void UpdateAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address, nameof(address));

        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets or updates contact information for the location.
    /// </summary>
    public void SetContactInfo(string? contactName, string? contactEmail, string? contactPhone)
    {
        if (contactName is not null && contactName.Length > 100)
            throw new DomainException("Contact name cannot exceed 100 characters");

        if (contactEmail is not null && contactEmail.Length > 100)
            throw new DomainException("Contact email cannot exceed 100 characters");

        if (contactPhone is not null && contactPhone.Length > 50)
            throw new DomainException("Contact phone cannot exceed 50 characters");

        ContactName = contactName;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the location.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the location (soft delete).
    /// Locations are never physically deleted as they are part of transaction history.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
