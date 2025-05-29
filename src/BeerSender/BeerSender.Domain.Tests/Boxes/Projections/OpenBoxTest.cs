using BeerSender.Domain.Boxes;
using BeerSender.Domain.Projections;
using FluentAssertions;
using FluentAssertions.Extensions;
using Marten.Events;

namespace BeerSender.Domain.Tests.Boxes.Projections;

public class OpenBoxTest(MartenFixture fixture) : MartenTest(fixture)
{
    [Fact]
    public async Task WhenBoxIsOpenWithBottles_OpenBoxShouldBeCorrect()
    {
        var boxId = Guid.NewGuid();

        object[] events =
        [
            Box_created_with_capacity(24),
            Beer_bottle_added(gouden_carolus),
            Beer_bottle_added(carte_blanche)
        ];

        await using var session = Store.LightweightSession();

        session.Events.StartStream<Box>(boxId, events);
        await session.SaveChangesAsync();

        await Store.WaitForNonStaleProjectionDataAsync(5.Seconds());

        await using var query = Store.QuerySession();
        
        var openBox = await query.LoadAsync<OpenBox>(boxId);
        
        // Run assertions
        openBox.Should().NotBeNull();
        openBox.BoxId.Should().Be(boxId);
        openBox.Capacity.Should().Be(24);
        openBox.NumberOfBottles.Should().Be(2);
    }
    
    // Events
    protected BoxCreatedWithContainerType Box_created_with_capacity(int capacity)
    {
        return new BoxCreatedWithContainerType(new BoxCapacity(capacity), string.Empty, ContainerType.Bottle);
    }

    protected BeerBottleAdded Beer_bottle_added(BeerBottle bottle)
    {
        return new BeerBottleAdded(bottle);
    }
    
    // Test data
    protected BeerBottle gouden_carolus = new(
        "Gouden Carolus",
        "Quadrupel Whisky Infused",
        12.7,
        BeerType.Quadruple
    );
    
    protected BeerBottle carte_blanche = new(
        "Wolf",
        "Carte Blanche",
        8.5,
        BeerType.Triple
    );
}