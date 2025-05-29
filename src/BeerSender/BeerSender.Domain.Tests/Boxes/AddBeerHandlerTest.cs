using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;

namespace BeerSender.Domain.Tests.Boxes;

public class AddBeerHandlerTest (MartenFixture fixture) 
    : BoxTest<AddBeerBottle>(fixture)
{
    protected override ICommandHandler<AddBeerBottle> Handler
        => new AddBeerBottleHandler();
    
    [Fact]
    public async Task IfBoxIsEmpty_ThenBottleShouldBeAdded()
    {
        await Given<Box>(
            Box_created_with_capacity(6)
        );
        await When(
            Add_beer_bottle(carte_blanche)
        );
        await Then(
            Beer_bottle_added(carte_blanche)
        );
    }
    
    [Fact]
    public async Task WhenAddedToBoxWithSpace_ShouldAddBottle()
    {
        await Given<Box>(
            Box_created_with_capacity(2),
            Beer_bottle_added(gouden_carolus)
        );
        await When(
            Add_beer_bottle(carte_blanche)
        );
        await Then(
            Beer_bottle_added(carte_blanche)
        );
    }

    [Fact]
    public async Task WhenAddedToFullBox_ShouldFail()
    {
        await Given<Box>(
            Box_created_with_capacity(1),
            Beer_bottle_added(gouden_carolus)
        );
        await When(
            Add_beer_bottle(carte_blanche)
        );
        await Then(
            Box_was_full()
        );
    }
    
    // Commands
    private AddBeerBottle Add_beer_bottle(BeerBottle bottle)
    {
        return new AddBeerBottle(Box_Id, bottle);
    }
}