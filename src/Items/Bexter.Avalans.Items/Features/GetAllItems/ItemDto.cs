namespace Bexter.Avalans.Items.Features.GetAllItems;

/// <summary>
/// Data Transfer Object for Item entity
/// </summary>
public sealed record ItemDto(
    Guid Id,
    string Name,
    string? Description,
    string? Sku,
    string? Barcode,
    decimal? Length,
    decimal? Width,
    decimal? Height,
    string? DimensionUnit,
    decimal? Weight,
    string? WeightUnit,
    decimal? Volume,
    string? VolumeUnit,
    bool IsFragile,
    bool IsPerishable,
    int? ShelfLife,
    decimal? Value,
    string? Currency,
    string? Category,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
