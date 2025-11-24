using Bexter.Avalans.Core;
using Bexter.Avalans.Items.Features.GetAllItems;
using Bexter.Avalans.Items.Features.GetItemById;
using Bexter.Avalans.Items.Features.CreateItem;
using Bexter.Avalans.Items.Features.UpdateItem;
using Bexter.Avalans.Items.Domain;
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
            .WithTags("Items")
            .WithOpenApi();

        itemsGroup.MapGet("/", GetAllItems)
            .WithName("GetAllItems")
            .WithSummary("Get all items")
            .WithDescription("Retrieves all items from the inventory");

        itemsGroup.MapGet("/{id:guid}", GetItemById)
            .WithName("GetItemById")
            .WithSummary("Get item by ID")
            .WithDescription("Retrieves a specific item by its unique identifier");

        itemsGroup.MapPost("/", CreateItem)
            .WithName("CreateItem")
            .WithSummary("Create a new item")
            .WithDescription("Creates a new item in the inventory");

        itemsGroup.MapPut("/{id:guid}", UpdateItem)
            .WithName("UpdateItem")
            .WithSummary("Update an item")
            .WithDescription("Updates an existing item in the inventory");

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

    private static async Task<IResult> CreateItem(
        CreateItemRequest request,
        ICommandHandler<CreateItemCommand, CreateItemResult> handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateItemCommand(
                request.Name,
                request.Description,
                request.Length,
                request.Width,
                request.Height,
                request.Weight,
                request.IsPerishable,
                request.ShelfLifeDays,
                request.Value);

            var result = await handler.Handle(command, cancellationToken);
            return Results.Created($"/items/{result.ItemId}", result);
        }
        catch (DomainException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> UpdateItem(
        Guid id,
        UpdateItemRequest request,
        ICommandHandler<UpdateItemCommand> handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateItemCommand(
                id,
                request.Name,
                request.Description,
                request.Length,
                request.Width,
                request.Height,
                request.Weight,
                request.IsPerishable,
                request.ShelfLifeDays,
                request.Value);

            await handler.Handle(command, cancellationToken);
            return Results.NoContent();
        }
        catch (DomainException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}

/// <summary>
/// HTTP request payload for creating an item
/// </summary>
public sealed record CreateItemRequest(
    string Name,
    string? Description,
    decimal? Length,
    decimal? Width,
    decimal? Height,
    decimal? Weight,
    bool IsPerishable,
    int? ShelfLifeDays,
    decimal? Value);

/// <summary>
/// HTTP request payload for updating an item
/// </summary>
public sealed record UpdateItemRequest(
    string Name,
    string? Description,
    decimal? Length,
    decimal? Width,
    decimal? Height,
    decimal? Weight,
    bool IsPerishable,
    int? ShelfLifeDays,
    decimal? Value);
