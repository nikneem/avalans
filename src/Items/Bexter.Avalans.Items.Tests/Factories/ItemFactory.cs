using Bexter.Avalans.Items.Domain;
using Bogus;

namespace Bexter.Avalans.Items.Tests.Factories;

/// <summary>
/// Factory for creating Item domain entities in tests using Bogus.
/// Provides realistic test data with deterministic seed support.
/// </summary>
public static class ItemFactory
{
    private static readonly Faker _faker = new();

    /// <summary>
    /// Creates an Item with optional overrides for testing.
    /// </summary>
    public static Item Create(
        string? name = null,
        string? description = null,
        decimal? length = null,
        decimal? width = null,
        decimal? height = null,
        decimal? weight = null,
        bool? isPerishable = null,
        int? shelfLifeDays = null,
        decimal? value = null)
    {
        var itemName = name ?? _faker.Commerce.ProductName();
        var itemDescription = description ?? _faker.Commerce.ProductDescription();

        var item = Item.Create(itemName, itemDescription);

        // Set dimensions if provided or generate random valid dimensions
        var itemLength = length ?? _faker.Random.Decimal(10m, 200m);
        var itemWidth = width ?? _faker.Random.Decimal(10m, 200m);
        var itemHeight = height ?? _faker.Random.Decimal(5m, 100m);
        item.SetDimensions(itemLength, itemWidth, itemHeight);

        // Set weight if provided or generate random valid weight
        var itemWeight = weight ?? _faker.Random.Decimal(0.1m, 100m);
        item.SetWeight(itemWeight);

        // Set perishability
        var itemIsPerishable = isPerishable ?? _faker.Random.Bool();
        if (itemIsPerishable)
        {
            var itemShelfLife = shelfLifeDays ?? _faker.Random.Int(1, 365);
            item.SetPerishability(true, itemShelfLife);
        }

        // Set value if provided or generate random value
        if (value.HasValue)
        {
            item.SetValue(value.Value);
        }
        else if (_faker.Random.Bool())
        {
            item.SetValue(_faker.Random.Decimal(10m, 10000m));
        }

        return item;
    }

    /// <summary>
    /// Creates a minimal Item with only required fields.
    /// </summary>
    public static Item CreateMinimal(string? name = null)
    {
        var itemName = name ?? _faker.Commerce.ProductName();
        return Item.Create(itemName);
    }

    /// <summary>
    /// Creates a perishable Item with shelf life.
    /// </summary>
    public static Item CreatePerishable(int shelfLifeDays = 7)
    {
        var item = Create(isPerishable: false);
        item.SetPerishability(true, shelfLifeDays);
        return item;
    }
}
