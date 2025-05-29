using BeerSender.Domain;
using Dapper;

namespace BeerSender.EventStore;

public class EventStore(
    EventStoreConnectionFactory dbConnectionFactory,
    INotificationService notificationService) 
    : IEventStore
{
    public IEnumerable<StoredEvent> GetEvents(Guid aggregateId)
    {   
        const string query = """
                             SELECT [AggregateId], [SequenceNumber], [Timestamp]
                                   ,[EventTypeName], [EventBody], [RowVersion]
                             FROM dbo.[Events]
                             WHERE [AggregateId] = @AggregateId
                             ORDER BY [SequenceNumber]
                             """;

        using var connection = dbConnectionFactory.Create();

        return connection.Query<DatabaseEvent>(
                query,
                new { AggregateId = aggregateId })
            .Select(e => e.ToStoredEvent());
    }

    public IEnumerable<StoredEvent> GetEventsUntilSequence(Guid aggregateId, int sequence)
    {
        const string query = """
                             SELECT [AggregateId], [SequenceNumber], [Timestamp]
                                   ,[EventTypeName], [EventBody], [RowVersion]
                             FROM dbo.[Events]
                             WHERE [AggregateId] = @AggregateId
                                   AND [SequenceNumber] <= @Sequence
                             ORDER BY [SequenceNumber]
                             """;

        using var connection = dbConnectionFactory.Create();

        return connection.Query<DatabaseEvent>(
                query,
                new { AggregateId = aggregateId, Sequence = sequence })
            .Select(e => e.ToStoredEvent());
    }

    private readonly List<StoredEvent> _newEvents = [];
    
    public void AppendEvent(StoredEvent @event)
    {
        _newEvents.Add(@event);
    }

    public void SaveChanges()
    {
        const string insertCommand = """
                                     INSERT INTO dbo.[Events]
                                                ([AggregateId], [SequenceNumber], [Timestamp]
                                                ,[EventTypeName], [EventBody])    
                                     VALUES
                                                (@AggregateId, @SequenceNumber,@Timestamp
                                                ,@EventTypeName, @EventBody)
                                     """;

        using var connection = dbConnectionFactory.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        connection.Execute(
            insertCommand,
            _newEvents.Select(DatabaseEvent.FromStoredEvent),
            transaction);
        
        transaction.Commit();

        foreach (var storedEvent in _newEvents)
        {
            notificationService.PublishEvent(
                storedEvent.AggregateId,
                storedEvent.EventData);
        }
        
        _newEvents.Clear();
    }
}