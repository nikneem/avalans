using Bexter.Avalans.Core;

namespace Bexter.Avalans.Items.Features.CreateItem;

/// <summary>
/// Command to create a new item in the inventory.
/// </summary>
public sealed record CreateItemCommand(
    string Name,
    string? Description,
    decimal? Length,
    decimal? Width,
    decimal? Height,
    decimal? Weight,
    bool IsPerishable,
    int? ShelfLifeDays,
    decimal? Value) : ICommand;
