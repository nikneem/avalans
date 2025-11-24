using Bexter.Avalans.Core;
using Bexter.Avalans.Items.Repositories;

namespace Bexter.Avalans.Items.Features.GetItemById;

public sealed class GetItemByIdQueryHandler : IQueryHandler<GetItemByIdQuery, ItemDto?>
{
    private readonly IItemRepository _repository;

    public GetItemByIdQueryHandler(IItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<ItemDto?> Handle(GetItemByIdQuery query, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(query.Id, cancellationToken);
        return item is null ? null : MapToDto(item);
    }

    private static ItemDto MapToDto(Domain.Item item) => new(
        item.Id,
        item.Name,
        item.Description,
        item.Sku,
        item.Barcode,
        item.Length,
        item.Width,
        item.Height,
        item.DimensionUnit,
        item.Weight,
        item.WeightUnit,
        item.Volume,
        item.VolumeUnit,
        item.IsFragile,
        item.IsPerishable,
        item.ShelfLife,
        item.Value,
        item.Currency,
        item.Category,
        item.CreatedAt,
        item.UpdatedAt
    );
}
