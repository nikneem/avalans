using Azure;
using Azure.Data.Tables;
using Bexter.Avalans.Items.Domain;

namespace Bexter.Avalans.Items.Data.TableStorage;

/// <summary>
/// Table Storage entity for Item
/// </summary>
public sealed class ItemTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "ITEM";
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // Item properties
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public double? Length { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
    public string? DimensionUnit { get; set; }
    public double? Weight { get; set; }
    public string? WeightUnit { get; set; }
    public double? Volume { get; set; }
    public string? VolumeUnit { get; set; }
    public bool IsFragile { get; set; }
    public bool IsPerishable { get; set; }
    public int? ShelfLife { get; set; }
    public double? Value { get; set; }
    public string? Currency { get; set; }
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Creates a TableEntity from a domain Item entity.
    /// </summary>
    public static ItemTableEntity FromDomainEntity(Item item)
    {
        return new ItemTableEntity
        {
            RowKey = item.Id.ToString(),
            Name = item.Name,
            Description = item.Description,
            Sku = item.Sku,
            Barcode = item.Barcode,
            Length = item.Length.HasValue ? (double)item.Length.Value : null,
            Width = item.Width.HasValue ? (double)item.Width.Value : null,
            Height = item.Height.HasValue ? (double)item.Height.Value : null,
            DimensionUnit = item.DimensionUnit,
            Weight = item.Weight.HasValue ? (double)item.Weight.Value : null,
            WeightUnit = item.WeightUnit,
            Volume = item.Volume.HasValue ? (double)item.Volume.Value : null,
            VolumeUnit = item.VolumeUnit,
            IsFragile = item.IsFragile,
            IsPerishable = item.IsPerishable,
            ShelfLife = item.ShelfLife,
            Value = item.Value.HasValue ? (double)item.Value.Value : null,
            Currency = item.Currency,
            Category = item.Category,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    /// <summary>
    /// Reconstructs a domain Item entity from storage.
    /// Uses reflection to bypass private setters for persistence reconstruction.
    /// </summary>
    public Item ToDomainEntity()
    {
        var item = (Item)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Item));

        typeof(Item).GetProperty(nameof(Item.Id))!.SetValue(item, Guid.Parse(RowKey));
        typeof(Item).GetProperty(nameof(Item.Name))!.SetValue(item, Name);
        typeof(Item).GetProperty(nameof(Item.Description))!.SetValue(item, Description);
        typeof(Item).GetProperty(nameof(Item.Sku))!.SetValue(item, Sku);
        typeof(Item).GetProperty(nameof(Item.Barcode))!.SetValue(item, Barcode);
        typeof(Item).GetProperty(nameof(Item.Length))!.SetValue(item, Length.HasValue ? (decimal?)Length.Value : null);
        typeof(Item).GetProperty(nameof(Item.Width))!.SetValue(item, Width.HasValue ? (decimal?)Width.Value : null);
        typeof(Item).GetProperty(nameof(Item.Height))!.SetValue(item, Height.HasValue ? (decimal?)Height.Value : null);
        typeof(Item).GetProperty(nameof(Item.DimensionUnit))!.SetValue(item, DimensionUnit);
        typeof(Item).GetProperty(nameof(Item.Weight))!.SetValue(item, Weight.HasValue ? (decimal?)Weight.Value : null);
        typeof(Item).GetProperty(nameof(Item.WeightUnit))!.SetValue(item, WeightUnit);
        typeof(Item).GetProperty(nameof(Item.Volume))!.SetValue(item, Volume.HasValue ? (decimal?)Volume.Value : null);
        typeof(Item).GetProperty(nameof(Item.VolumeUnit))!.SetValue(item, VolumeUnit);
        typeof(Item).GetProperty(nameof(Item.IsFragile))!.SetValue(item, IsFragile);
        typeof(Item).GetProperty(nameof(Item.IsPerishable))!.SetValue(item, IsPerishable);
        typeof(Item).GetProperty(nameof(Item.ShelfLife))!.SetValue(item, ShelfLife);
        typeof(Item).GetProperty(nameof(Item.Value))!.SetValue(item, Value.HasValue ? (decimal?)Value.Value : null);
        typeof(Item).GetProperty(nameof(Item.Currency))!.SetValue(item, Currency);
        typeof(Item).GetProperty(nameof(Item.Category))!.SetValue(item, Category);
        typeof(Item).GetProperty(nameof(Item.CreatedAt))!.SetValue(item, CreatedAt);
        typeof(Item).GetProperty(nameof(Item.UpdatedAt))!.SetValue(item, UpdatedAt);

        return item;
    }
}
