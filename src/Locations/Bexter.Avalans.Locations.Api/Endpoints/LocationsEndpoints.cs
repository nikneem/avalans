using Bexter.Avalans.Core;
using Bexter.Avalans.Locations.Domain;
using Bexter.Avalans.Locations.Dtos;
using Bexter.Avalans.Locations.Features.CreateLocation;
using Bexter.Avalans.Locations.Features.GetAllLocations;
using Bexter.Avalans.Locations.Features.GetLocationById;
using Bexter.Avalans.Locations.Features.UpdateLocation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bexter.Avalans.Locations.Api.Endpoints;

/// <summary>
/// Locations API endpoints.
/// Implements RESTful operations for location management.
/// </summary>
public static class LocationsEndpoints
{
    public static RouteGroupBuilder MapLocationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/locations")
            .WithTags("Locations")
            .WithOpenApi();

        group.MapGet("/", GetAllLocations)
            .WithName("GetAllLocations")
            .WithSummary("Retrieve all locations")
            .Produces<IEnumerable<LocationDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetLocationById)
            .WithName("GetLocationById")
            .WithSummary("Retrieve a location by ID")
            .Produces<LocationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateLocation)
            .WithName("CreateLocation")
            .WithSummary("Create a new location")
            .Produces<CreateLocationResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", UpdateLocation)
            .WithName("UpdateLocation")
            .WithSummary("Update an existing location")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<Results<Ok<IEnumerable<LocationDto>>, BadRequest<string>>> GetAllLocations(
        IQueryHandler<GetAllLocationsQuery, IEnumerable<LocationDto>> handler,
        [AsParameters] GetAllLocationsQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(query, cancellationToken);
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<Ok<LocationDto>, NotFound, BadRequest<string>>> GetLocationById(
        IQueryHandler<GetLocationByIdQuery, LocationDto?> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetLocationByIdQuery(id);
            var result = await handler.Handle(query, cancellationToken);

            return result is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<Created<CreateLocationResult>, BadRequest<string>>> CreateLocation(
        ICommandHandler<CreateLocationCommand, CreateLocationResult> handler,
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(command, cancellationToken);
            return TypedResults.Created($"/locations/{result.LocationId}", result);
        }
        catch (DomainException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound>> UpdateLocation(
        ICommandHandler<UpdateLocationCommand> handler,
        Guid id,
        UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateLocationCommand(
                id,
                request.Name,
                request.Number,
                request.Street,
                request.City,
                request.PostalCode,
                request.Country,
                request.State,
                request.ContactName,
                request.ContactEmail,
                request.ContactPhone);

            await handler.Handle(command, cancellationToken);
            return TypedResults.NoContent();
        }
        catch (DomainException ex) when (ex.Message.Contains("not found"))
        {
            return TypedResults.NotFound();
        }
        catch (DomainException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}

/// <summary>
/// Request model for updating locations (excludes ID from body).
/// </summary>
public sealed record UpdateLocationRequest(
    string Name,
    string Number,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string? State,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone);
