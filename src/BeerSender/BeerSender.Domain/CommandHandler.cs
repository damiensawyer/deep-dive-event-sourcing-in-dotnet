namespace BeerSender.Domain;

public abstract class CommandHandler<TCommand>(IEventStore eventStore)
{
    protected EventStream<TEntity> GetStream<TEntity>(Guid aggregateId)
        where TEntity: AggregateRoot, new()
    {
        return new EventStream<TEntity>(eventStore, aggregateId);
    }
    
    public abstract void Handle(TCommand command);
}