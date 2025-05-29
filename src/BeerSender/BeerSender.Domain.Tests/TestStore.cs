namespace BeerSender.Domain.Tests;

/// <summary>
/// An in-memory Event Store for unit test purposes
/// </summary>
public class TestStore : IEventStore
{
    /// <summary>
    /// Add any events that have happened before to this collection.
    /// </summary>
    public readonly List<StoredEvent> PreviousEvents = [];
    /// <summary>
    /// Use this collection to verify which events have been raised.
    /// </summary>
    public readonly List<StoredEvent> NewEvents = [];

    /// <summary>
    /// Gets the events from "previousEvents" for the Aggregate ID.
    /// </summary>
    public IEnumerable<StoredEvent> GetEvents(Guid aggregateId)
    {
        return PreviousEvents
            .Where(e => e.AggregateId == aggregateId)
            .ToList();
    }

    /// <summary>
    /// Gets the events from "previousEvents" for the Aggregate ID
    /// up until the specified sequence number.
    /// </summary>
    public IEnumerable<StoredEvent> GetEventsUntilSequence(Guid aggregateId, int sequence)
    {
        return PreviousEvents
            .Where(e => e.AggregateId == aggregateId && e.SequenceNumber <= sequence)
            .ToList();
    }

    /// <summary>
    /// Appends new events to the "newEvents' collection.
    /// </summary>
    public void AppendEvent(StoredEvent @event)
    {
        NewEvents.Add(@event);
    }

    /// <summary>
    /// Not used in command handle tests
    /// </summary>
    public void SaveChanges()
    {
        throw new NotImplementedException();
    }
}