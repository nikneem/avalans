namespace Bexter.Avalans.Locations.Domain;

/// <summary>
/// Value object representing a physical address.
/// Following pragmatic DDD - use value objects for multi-field concepts with interdependent validation.
/// </summary>
public record Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string PostalCode { get; init; }
    public string Country { get; init; }
    public string? State { get; init; }

    private Address(string street, string city, string postalCode, string country, string? state = null)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
        State = state;
    }

    /// <summary>
    /// Factory method to create an Address with validation.
    /// </summary>
    public static Address Create(string street, string city, string postalCode, string country, string? state = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street, nameof(street));
        ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode, nameof(postalCode));
        ArgumentException.ThrowIfNullOrWhiteSpace(country, nameof(country));

        if (street.Length > 200)
            throw new DomainException("Street address cannot exceed 200 characters");

        if (city.Length > 100)
            throw new DomainException("City name cannot exceed 100 characters");

        if (postalCode.Length > 20)
            throw new DomainException("Postal code cannot exceed 20 characters");

        if (country.Length > 100)
            throw new DomainException("Country name cannot exceed 100 characters");

        return new Address(street, city, postalCode, country, state);
    }

    public override string ToString()
    {
        var parts = new[] { Street, City, State, PostalCode, Country }.Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(", ", parts);
    }
}
