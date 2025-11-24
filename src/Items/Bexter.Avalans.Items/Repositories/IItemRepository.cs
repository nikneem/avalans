using Bexter.Avalans.Items.Domain;

namespace Bexter.Avalans.Items.Repositories;

/// <summary>
/// Repository interface for Item aggregate root.
/// Following DDD principles, this works with domain entities, not DTOs.
/// </summary>
public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(Item item, CancellationToken cancellationToken = default);
    Task UpdateAsync(Item item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
