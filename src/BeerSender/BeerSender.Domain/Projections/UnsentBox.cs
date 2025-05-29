using BeerSender.Domain.Boxes;
using Marten.Events;
using Marten.Events.Projections;

namespace BeerSender.Domain.Projections;

public class UnsentBox
{
    public Guid BoxId { get; set; }
    public string? Status { get; set; }
}

public class UnsentBoxProjection : EventProjection
{
    public UnsentBoxProjection()
    {
        Project<IEvent<BoxCreated>>((evt, operations) =>
        {
            operations.Store(new UnsentBox
            {
                BoxId = evt.StreamId
            });
        });

        Project<IEvent<BoxSent>>((evt, operations) =>
        {
            operations.Delete<UnsentBox>(evt.StreamId);
        });

        ProjectAsync<IEvent<BoxClosed>>(async (evt, operations) =>
        {
            var unsentBox = await operations.LoadAsync<UnsentBox>(evt.StreamId);

            if (unsentBox is null)
                return;
            unsentBox.Status = "Ready to send!";
            
            operations.Store(unsentBox);
        });
    }
}