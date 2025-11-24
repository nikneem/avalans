using Bexter.Avalans.Core;
using Bexter.Avalans.Items.Repositories;
using Bexter.Avalans.Items.Api.Endpoints;
using Bexter.Avalans.Items.Data.TableStorage;
using Bexter.Avalans.Items.Features.GetAllItems;
using Bexter.Avalans.Items.Features.GetItemById;
using Bexter.Avalans.Items.Features.CreateItem;
using Bexter.Avalans.Items.Features.UpdateItem;
using GetAllItems = Bexter.Avalans.Items.Features.GetAllItems;
using GetItemById = Bexter.Avalans.Items.Features.GetItemById;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register Azure Table Storage client using Aspire integration
builder.AddAzureTableClient("tables");

// Register repository
builder.Services.AddSingleton<IItemRepository, TableStorageItemRepository>();

// Register query handlers
builder.Services.AddSingleton<IQueryHandler<GetAllItemsQuery, IEnumerable<GetAllItems.ItemDto>>, GetAllItemsQueryHandler>();
builder.Services.AddSingleton<IQueryHandler<GetItemByIdQuery, GetItemById.ItemDto?>, GetItemByIdQueryHandler>();

// Register command handlers
builder.Services.AddSingleton<ICommandHandler<CreateItemCommand, CreateItemResult>, CreateItemCommandHandler>();
builder.Services.AddSingleton<ICommandHandler<UpdateItemCommand>, UpdateItemCommandHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Map Items endpoints
app.MapItemsEndpoints();

app.Run();
