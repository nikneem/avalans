using Bexter.Avalans.Core;
using Bexter.Avalans.Items.Repositories;

namespace Bexter.Avalans.Items.Features.GetAllItems;

public sealed class GetAllItemsQueryHandler : IQueryHandler<GetAllItemsQuery, IEnumerable<ItemDto>>
{
    private readonly IItemRepository _repository;

    public GetAllItemsQueryHandler(IItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ItemDto>> Handle(GetAllItemsQuery query, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(MapToDto);
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
