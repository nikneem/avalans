using Bexter.Avalans.Core;
using Bexter.Avalans.Locations.Api.Endpoints;
using Bexter.Avalans.Locations.Dtos;
using Bexter.Avalans.Locations.Features.CreateLocation;
using Bexter.Avalans.Locations.Features.GetAllLocations;
using Bexter.Avalans.Locations.Features.GetLocationById;
using Bexter.Avalans.Locations.Features.UpdateLocation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register query handlers
builder.Services.AddTransient<IQueryHandler<GetAllLocationsQuery, IEnumerable<LocationDto>>, GetAllLocationsQueryHandler>();
builder.Services.AddTransient<IQueryHandler<GetLocationByIdQuery, LocationDto?>, GetLocationByIdQueryHandler>();

// Register command handlers
builder.Services.AddTransient<ICommandHandler<CreateLocationCommand, CreateLocationResult>, CreateLocationCommandHandler>();
builder.Services.AddTransient<ICommandHandler<UpdateLocationCommand>, UpdateLocationCommandHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapLocationsEndpoints();

app.Run();
