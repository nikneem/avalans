namespace Bexter.Avalans.Core;

/// <summary>
/// Handler for queries that return a result
/// </summary>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
}
