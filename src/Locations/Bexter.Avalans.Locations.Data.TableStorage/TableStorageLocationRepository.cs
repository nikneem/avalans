using Azure.Data.Tables;
using Bexter.Avalans.Locations.Domain;
using Bexter.Avalans.Locations.Repositories;

namespace Bexter.Avalans.Locations.Data.TableStorage;

/// <summary>
/// Table Storage implementation of ILocationRepository.
/// Works with domain entities (Location) and handles persistence mapping internally.
/// Supports business key lookups via the Number field.
/// </summary>
public sealed class TableStorageLocationRepository : ILocationRepository
{
    private readonly TableClient _tableClient;
    private const string PartitionKey = "LOCATION";
    private const string TableName = "Locations";

    public TableStorageLocationRepository(TableServiceClient tableServiceClient)
    {
        _tableClient = tableServiceClient.GetTableClient(TableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<LocationTableEntity>(
                PartitionKey,
                id.ToString(),
                cancellationToken: cancellationToken);

            return response.Value.ToDomainEntity();
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<Location?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        // Query by Number field (secondary index)
        var filter = $"PartitionKey eq '{PartitionKey}' and Number eq '{number}'";
        
        await foreach (var entity in _tableClient.QueryAsync<LocationTableEntity>(
            filter: filter,
            cancellationToken: cancellationToken))
        {
            return entity.ToDomainEntity();
        }

        return null;
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var locations = new List<Location>();
        
        // Build filter based on includeInactive flag
        var filter = includeInactive
            ? $"PartitionKey eq '{PartitionKey}'"
            : $"PartitionKey eq '{PartitionKey}' and IsActive eq true";

        await foreach (var entity in _tableClient.QueryAsync<LocationTableEntity>(
            filter: filter,
            cancellationToken: cancellationToken))
        {
            locations.Add(entity.ToDomainEntity());
        }

        return locations;
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        var entity = LocationTableEntity.FromDomainEntity(location);
        await _tableClient.AddEntityAsync(entity, cancellationToken);
        return location.Id;
    }

    public async Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        var entity = LocationTableEntity.FromDomainEntity(location);
        await _tableClient.UpdateEntityAsync(entity, entity.ETag, cancellationToken: cancellationToken);
    }
}
