namespace Bexter.Avalans.Items.Domain;

/// <summary>
/// Represents an item that can be transported between locations in the logistics system.
/// This is a rich domain model following DDD principles with encapsulated state and business rules.
/// </summary>
public class Item
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Sku { get; private set; }
    public string? Barcode { get; private set; }

    // Dimensions
    public decimal? Length { get; private set; }
    public decimal? Width { get; private set; }
    public decimal? Height { get; private set; }
    public string? DimensionUnit { get; private set; }

    // Weight
    public decimal? Weight { get; private set; }
    public string? WeightUnit { get; private set; }

    // Additional properties
    public decimal? Volume { get; private set; }
    public string? VolumeUnit { get; private set; }
    public bool IsFragile { get; private set; }
    public bool IsPerishable { get; private set; }
    public int? ShelfLife { get; private set; }
    public decimal? Value { get; private set; }
    public string? Currency { get; private set; }
    public string? Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Private constructor for EF Core and internal use
    private Item()
    {
        Name = string.Empty; // Will be set by factory or EF
    }

    /// <summary>
    /// Factory method to create a new Item with required properties.
    /// </summary>
    public static Item Create(string name, string? description = null, string? sku = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        if (name.Length > 200)
            throw new DomainException("Item name cannot exceed 200 characters");

        return new Item
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Sku = sku,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates the basic information of the item.
    /// </summary>
    public void UpdateBasicInfo(string name, string? description = null, string? sku = null, string? barcode = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        if (name.Length > 200)
            throw new DomainException("Item name cannot exceed 200 characters");

        Name = name;
        Description = description;
        Sku = sku;
        Barcode = barcode;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the dimensions of the item.
    /// </summary>
    public void SetDimensions(decimal? length, decimal? width, decimal? height, string? unit = "cm")
    {
        if (length.HasValue && length.Value <= 0)
            throw new DomainException("Length must be greater than zero");

        if (width.HasValue && width.Value <= 0)
            throw new DomainException("Width must be greater than zero");

        if (height.HasValue && height.Value <= 0)
            throw new DomainException("Height must be greater than zero");

        Length = length;
        Width = width;
        Height = height;
        DimensionUnit = unit;
        UpdatedAt = DateTime.UtcNow;

        // Auto-calculate volume if all dimensions provided
        if (length.HasValue && width.HasValue && height.HasValue)
        {
            Volume = length.Value * width.Value * height.Value;
            VolumeUnit = unit switch
            {
                "cm" => "cm³",
                "m" => "m³",
                "inch" => "in³",
                _ => unit
            };
        }
    }

    /// <summary>
    /// Sets the weight of the item.
    /// </summary>
    public void SetWeight(decimal weight, string unit = "kg")
    {
        if (weight <= 0)
            throw new DomainException("Weight must be greater than zero");

        Weight = weight;
        WeightUnit = unit;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the item as fragile.
    /// </summary>
    public void MarkAsFragile()
    {
        IsFragile = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the item as non-fragile.
    /// </summary>
    public void MarkAsNonFragile()
    {
        IsFragile = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets perishability and shelf life for the item.
    /// </summary>
    public void SetPerishability(bool isPerishable, int? shelfLifeDays = null)
    {
        if (isPerishable && (!shelfLifeDays.HasValue || shelfLifeDays.Value <= 0))
            throw new DomainException("Perishable items must have a positive shelf life in days");

        if (!isPerishable && shelfLifeDays.HasValue)
            throw new DomainException("Non-perishable items cannot have a shelf life");

        IsPerishable = isPerishable;
        ShelfLife = shelfLifeDays;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the monetary value of the item.
    /// </summary>
    public void SetValue(decimal value, string currency = "USD")
    {
        if (value < 0)
            throw new DomainException("Item value cannot be negative");

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new DomainException("Currency must be a valid 3-letter ISO code");

        Value = value;
        Currency = currency.ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the category of the item.
    /// </summary>
    public void SetCategory(string? category)
    {
        Category = category;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the total volume in cubic units based on dimensions.
    /// </summary>
    public decimal? CalculateVolume()
    {
        if (Length.HasValue && Width.HasValue && Height.HasValue)
            return Length.Value * Width.Value * Height.Value;

        return Volume;
    }
}
