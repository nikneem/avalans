using Bexter.Avalans.Core;
using Bexter.Avalans.Items.Features.GetAllItems;
using Bexter.Avalans.Items.Features.GetItemById;
using GetAllItems = Bexter.Avalans.Items.Features.GetAllItems;
using GetItemById = Bexter.Avalans.Items.Features.GetItemById;

namespace Bexter.Avalans.Items.Api.Endpoints;

/// <summary>
/// Extension methods for mapping Items API endpoints
/// </summary>
public static class ItemsEndpoints
{
    /// <summary>
    /// Maps all Items endpoints using route grouping
    /// </summary>
    public static IEndpointRouteBuilder MapItemsEndpoints(this IEndpointRouteBuilder app)
    {
        var itemsGroup = app.MapGroup("/items")
            .WithTags("Items");

        itemsGroup.MapGet("/", GetAllItems)
            .WithName("GetAllItems")
            .WithSummary("Get all items")
            .WithDescription("Retrieves all items from the inventory");

        itemsGroup.MapGet("/{id:guid}", GetItemById)
            .WithName("GetItemById")
            .WithSummary("Get item by ID")
            .WithDescription("Retrieves a specific item by its unique identifier");

        return app;
    }

    private static async Task<IResult> GetAllItems(
        IQueryHandler<GetAllItemsQuery, IEnumerable<GetAllItems.ItemDto>> handler,
        CancellationToken cancellationToken)
    {
        var items = await handler.Handle(new GetAllItemsQuery(), cancellationToken);
        return Results.Ok(items);
    }

    private static async Task<IResult> GetItemById(
        Guid id,
        IQueryHandler<GetItemByIdQuery, GetItemById.ItemDto?> handler,
        CancellationToken cancellationToken)
    {
        var item = await handler.Handle(new GetItemByIdQuery(id), cancellationToken);
        return item is not null ? Results.Ok(item) : Results.NotFound();
    }
}
