using Bexter.Avalans.Core;

namespace Bexter.Avalans.Items.Features.UpdateItem;

/// <summary>
/// Command to update an existing item in the inventory.
/// </summary>
public sealed record UpdateItemCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal? Length,
    decimal? Width,
    decimal? Height,
    decimal? Weight,
    bool IsPerishable,
    int? ShelfLifeDays,
    decimal? Value) : ICommand;
