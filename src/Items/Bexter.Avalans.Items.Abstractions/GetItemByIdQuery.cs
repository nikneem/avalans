using Bexter.Avalans.Core;

namespace Bexter.Avalans.Items.Abstractions;

/// <summary>
/// Query to retrieve a single item by ID
/// </summary>
public sealed record GetItemByIdQuery(Guid Id) : IQuery;
