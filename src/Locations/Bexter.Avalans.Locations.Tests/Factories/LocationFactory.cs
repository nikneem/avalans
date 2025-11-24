using Bexter.Avalans.Locations.Domain;
using Bogus;

namespace Bexter.Avalans.Locations.Tests.Factories;

/// <summary>
/// Factory for creating test location instances using Bogus.
/// </summary>
public static class LocationFactory
{
    private static readonly Faker _faker = new();

    public static Location Create()
    {
        var address = Address.Create(
            _faker.Address.StreetAddress(),
            _faker.Address.City(),
            _faker.Address.ZipCode(),
            _faker.Address.Country(),
            _faker.Address.State());

        return Location.Create(
            _faker.Company.CompanyName(),
            _faker.Random.AlphaNumeric(10),
            address);
    }

    public static Location CreateWithContact()
    {
        var location = Create();
        location.SetContactInfo(
            _faker.Name.FullName(),
            _faker.Internet.Email(),
            _faker.Phone.PhoneNumber());
        return location;
    }

    public static Location CreateInactive()
    {
        var location = Create();
        location.Deactivate();
        return location;
    }
}
