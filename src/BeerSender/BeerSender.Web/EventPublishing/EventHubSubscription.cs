using BeerSender.Domain;
using Marten;
using Marten.Events.Daemon;
using Marten.Events.Daemon.Internals;
using Marten.Subscriptions;
using Microsoft.AspNetCore.SignalR;
using NotImplementedException = System.NotImplementedException;

namespace BeerSender.Web.EventPublishing;

public class EventHubSubscription(IHubContext<EventHub> hubContext) : ISubscription
{
    public Task<IChangeListener> ProcessEventsAsync(
        EventRange page, 
        ISubscriptionController controller, 
        IDocumentOperations operations,
        CancellationToken cancellationToken)
    {
        foreach (var @event in page.Events)
        {
            hubContext.Clients.Group(@event.StreamId.ToString())
                .SendAsync("PublishEvent", @event.StreamId, @event.Data, @event.Data.GetType().Name);
        }
        
        // If you don't care about being signaled for callback
        return Task.FromResult(NullChangeListener.Instance);
    }
    
    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }
}