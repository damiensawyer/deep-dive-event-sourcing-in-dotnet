namespace BeerSender.Domain;

public interface IEventStore
{
    IEnumerable<StoredEvent> GetEvents(Guid aggregateId);
    IEnumerable<StoredEvent> GetEventsUntilSequence(Guid aggregateId, int sequence);
    void AppendEvent(StoredEvent @event);
    void SaveChanges();
}