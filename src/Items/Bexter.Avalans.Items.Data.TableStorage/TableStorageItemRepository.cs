using Azure.Data.Tables;
using Bexter.Avalans.Items.Domain;
using Bexter.Avalans.Items.Repositories;

namespace Bexter.Avalans.Items.Data.TableStorage;

/// <summary>
/// Table Storage implementation of IItemRepository.
/// Works with domain entities (Item) and handles persistence mapping internally.
/// </summary>
public sealed class TableStorageItemRepository : IItemRepository
{
    private readonly TableClient _tableClient;
    private const string PartitionKey = "ITEM";
    private const string TableName = "Items";

    public TableStorageItemRepository(TableServiceClient tableServiceClient)
    {
        _tableClient = tableServiceClient.GetTableClient(TableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = new List<Item>();

        await foreach (var entity in _tableClient.QueryAsync<ItemTableEntity>(
            filter: $"PartitionKey eq '{PartitionKey}'",
            cancellationToken: cancellationToken))
        {
            items.Add(entity.ToDomainEntity());
        }

        return items;
    }

    public async Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<ItemTableEntity>(
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

    public async Task<Guid> AddAsync(Item item, CancellationToken cancellationToken = default)
    {
        var entity = ItemTableEntity.FromDomainEntity(item);
        await _tableClient.AddEntityAsync(entity, cancellationToken);
        return item.Id;
    }

    public async Task UpdateAsync(Item item, CancellationToken cancellationToken = default)
    {
        var entity = ItemTableEntity.FromDomainEntity(item);
        await _tableClient.UpdateEntityAsync(entity, entity.ETag, cancellationToken: cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _tableClient.DeleteEntityAsync(PartitionKey, id.ToString(), cancellationToken: cancellationToken);
            return true;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }
}
