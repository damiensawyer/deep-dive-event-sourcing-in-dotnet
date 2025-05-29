using BeerSender.Domain.Boxes;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace BeerSender.Domain.Projections;

public class OpenBox
{
    public Guid BoxId { get; set; }
    public int Capacity { get; set; }
    public int NumberOfBottles { get; set; }
}

public class OpenBoxProjection : SingleStreamProjection<OpenBox>
{
    public OpenBoxProjection()
    {
        DeleteEvent<BoxClosed>();
    }
    
    public static OpenBox Create(BoxCreatedWithContainerType started)
    {
        return new OpenBox
        {
            Capacity = started.BoxType.NumberOfSpots
        };
    }

    public void Apply(BeerBottleAdded _, OpenBox box)
    {
        box.NumberOfBottles++;
    }
}