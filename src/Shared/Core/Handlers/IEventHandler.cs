using Core.Aggregates;

namespace Core.Handlers;

public interface IEventHandler<T>
{
    Task<T> GetAggregateByIdAsync(Guid id, CancellationToken ct);
    Task SaveAsync(BaseAggregate item, CancellationToken ct);
}