using FluentAssertions;
using Marten;

namespace BeerSender.Domain.Tests;

[Collection("Marten collection")]
public abstract class CommandHandlerTest<TCommand>
{
    private readonly Dictionary<Guid, long> _streamVersions = new();
    
    MartenFixture fixture;

    protected CommandHandlerTest(MartenFixture fixture)
    {
        this.fixture = fixture;
        Store = fixture.Store;
    }
    
    /// <summary>
    /// If no explicit aggregateId is provided, this one will be used behind the scenes.
    /// </summary>
    protected readonly Guid _aggregateId = Guid.NewGuid();

    /// <summary>
    /// The command handler, to be provided in the Test class.
    /// This to account for additional injections
    /// </summary>
    protected abstract ICommandHandler<TCommand> Handler { get; }

    /// <summary>
    /// A fake, in-memory event store.
    /// </summary>
    protected IDocumentStore Store { get; private set; }

    /// <summary>
    /// Sets a list of previous events for the default aggregate ID.
    /// </summary>
    protected async Task Given<TAggregate>(params object[] events) 
        where TAggregate : class
    {
        await Given<TAggregate>(_aggregateId, events);
    }

    /// <summary>
    /// Sets a list of previous events for a specified aggregate ID.
    /// </summary>
    protected async Task Given<TAggregate>(Guid aggregateId, params object[] events) 
        where TAggregate : class
    {
        if(events.IsEmpty())
            return;
        
        await using var session = Store.LightweightSession();
            
        var stream = session.Events.StartStream<TAggregate>(aggregateId, events);

        _streamVersions[aggregateId] = stream.Version + stream.Events.Count;
                
        await session.SaveChangesAsync();
    }

    /// <summary>
    /// Triggers the handling of a command against the configured events.
    /// </summary>
    protected async Task When(TCommand command)
    {
        await Handler.Handle(command);
    }

    /// <summary>
    /// Asserts that the expected events have been appended to the event store
    /// for the default aggregate ID.
    /// </summary>
    protected async Task Then(params object[] expectedEvents)
    {
        await Then(_aggregateId, expectedEvents);
    }

    /// <summary>
    /// Asserts that the expected events have been appended to the event store
    /// for a specific aggregate ID.
    /// </summary>
    protected async Task Then(Guid aggregateId, params object[] expectedEvents)
    {
        await using var session = Store.LightweightSession();

        var version = _streamVersions.ContainsKey(aggregateId) 
            ? _streamVersions[aggregateId] + 1
            : 0L;
        
        var storedEvents = await session.Events.FetchStreamAsync(aggregateId, fromVersion: version);
            var actualEvents = storedEvents
            .OrderBy(e => e.Version)
            .Select(e => e.Data)
            .ToArray();

        actualEvents.Length.Should().Be(expectedEvents.Length);

        for (var i = 0; i < actualEvents.Length; i++)
        {
            actualEvents[i].Should().BeOfType(expectedEvents[i].GetType());
            try
            {
                actualEvents[i].Should().BeEquivalentTo(expectedEvents[i]);
            }
            catch (InvalidOperationException e)
            {
                // Empty event with matching type is OK. This means that the event class
                // has no properties. If the types match in this situation, the correct
                // event has been appended. So we should ignore this exception.
                if (!e.Message.StartsWith("No members were found for comparison."))
                    throw;
            }
        }
    }
}