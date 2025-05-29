using BeerSender.Domain.Boxes;
using Marten.Events;
using Marten.Events.Projections;

namespace BeerSender.Domain.Projections;

public class BottleInBoxes
{
    public required string BottleId { get; set; }
    public required BeerBottle Bottle { get; set; }
    public List<Guid> BoxIds { get; set; } = new ();
}

public class BottleInBoxesProjection : MultiStreamProjection<BottleInBoxes, string>
{
    public BottleInBoxesProjection()
    {
        Identity<BeerBottleAdded>(x => x.Bottle.BottleId);
    }
    
    public static BottleInBoxes Create(IEvent<BeerBottleAdded> started)
    {
        return new BottleInBoxes
        {
            BottleId = started.Data.Bottle.BottleId,
            Bottle = started.Data.Bottle,
            BoxIds = [started.StreamId]
        };
    }
    
    public static void Apply(IEvent<BeerBottleAdded> evt, BottleInBoxes bottle)
    {
        bottle.BoxIds.Add(evt.StreamId);
    }
}
