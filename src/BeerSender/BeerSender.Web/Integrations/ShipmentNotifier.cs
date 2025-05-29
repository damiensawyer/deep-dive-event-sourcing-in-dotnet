using Marten;
using Marten.Events.Daemon;
using Marten.Events.Daemon.Internals;
using Marten.Subscriptions;

namespace BeerSender.Web.Integrations;

public class ShipmentNotifier(ILogger<ShipmentNotifier> logger) : ISubscription
{
    public Task<IChangeListener> ProcessEventsAsync(
        EventRange page, 
        ISubscriptionController controller, 
        IDocumentOperations operations,
        CancellationToken cancellationToken)
    {
        foreach (var @event in page.Events)
        {
            // Call external system here.
            logger.LogDebug("Called carrier's API to notify for pickup.");
        }
    
        // If you don't care about being signaled for callback
        return Task.FromResult(NullChangeListener.Instance);
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }
}