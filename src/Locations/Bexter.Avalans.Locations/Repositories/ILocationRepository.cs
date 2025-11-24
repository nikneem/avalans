using Bexter.Avalans.Locations.Domain;

namespace Bexter.Avalans.Locations.Repositories;

/// <summary>
/// Repository interface for Location aggregate root.
/// Following DDD principles, this works with domain entities, not DTOs.
/// </summary>
public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Location?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Location>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken = default);
    Task UpdateAsync(Location location, CancellationToken cancellationToken = default);
}
