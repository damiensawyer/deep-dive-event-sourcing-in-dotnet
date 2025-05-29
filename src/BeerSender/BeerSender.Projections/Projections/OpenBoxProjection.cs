using BeerSender.Domain.Boxes;
using BeerSender.Projections.Database.Repositories;

namespace BeerSender.Projections.Projections;

public class OpenBoxProjection(OpenBoxRepository openBoxRepo) : IProjection
{
    public List<Type> RelevantEventTypes =>
        [typeof(BoxCreated), typeof(BeerBottleAdded), typeof(BoxClosed)];

    public int BatchSize => 50;
    public int WaitTime => 5000;
    
    public void Project(IEnumerable<StoredEventWithVersion> events)
    {
        foreach (var storedEvent in events)
        {
            var boxId = storedEvent.AggregateId;

            switch (storedEvent.EventData)
            {
                case BoxCreated boxCreated:
                    var capacity = boxCreated.Capacity.NumberOfSpots;
                    openBoxRepo.CreateOpenBox(boxId, capacity);
                    break;
                case BeerBottleAdded:
                    openBoxRepo.AddBottleToBox(boxId);
                    break;
                case BoxClosed:
                    openBoxRepo.RemoveOpenBox(boxId);
                    break;
            }
        }
    }
}

public class UnsentBoxProjection(UnsentBoxRepository unsentBoxRepo) : IProjection
{
    public List<Type> RelevantEventTypes =>
        [typeof(BoxCreated), typeof(BoxClosed), typeof(BoxSent)];

    public int BatchSize => 50;
    public int WaitTime => 5000;
    
    public void Project(IEnumerable<StoredEventWithVersion> events)
    {
        foreach (var storedEvent in events)
        {
            var boxId = storedEvent.AggregateId;

            switch (storedEvent.EventData)
            {
                case BoxCreated:
                    unsentBoxRepo.CreateUnsentBox(boxId);
                    break;
                case BoxClosed:
                    unsentBoxRepo.SetStatus(boxId, "Ready to send");
                    break;
                case BoxSent:
                    unsentBoxRepo.RemoveUnsentBox(boxId);
                    break;
            }
        }
    }
}