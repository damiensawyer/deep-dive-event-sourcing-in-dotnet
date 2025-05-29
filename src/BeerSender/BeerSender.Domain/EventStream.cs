namespace BeerSender.Domain;

public class EventStream<TEntity>(IEventStore eventStore, Guid aggregateId)
    where TEntity: AggregateRoot, new()
{
    private int _lastSequenceNumber;
    
    public TEntity GetEntity()
    {
        var events = eventStore.GetEvents(aggregateId);

        TEntity entity = new();
        foreach (var @event in events)
        {
            entity.Apply((dynamic)@event.EventData);
            _lastSequenceNumber = @event.SequenceNumber;
        }

        return entity;
    }
    
    public TEntity GetEntityBySequence(int sequenceNumber)
    {
        var events = eventStore.GetEventsUntilSequence(aggregateId, sequenceNumber);

        TEntity entity = new();
        foreach (var @event in events)
        {
            entity.Apply((dynamic)@event.EventData);
        }

        return entity;
    }

    public void Append(object @event)
    {
        _lastSequenceNumber++;

        StoredEvent storedEvent = new(
            aggregateId,
            _lastSequenceNumber,
            DateTime.UtcNow,
            @event);
        
        eventStore.AppendEvent(storedEvent);
    }
}