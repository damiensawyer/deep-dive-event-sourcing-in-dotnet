using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;

namespace BeerSender.Domain.Tests.Boxes;

public class AddBeerHandlerTest : BoxTest<AddBeerBottle>
{
    protected override CommandHandler<AddBeerBottle> Handler
        => new AddBeerBottleHandler(eventStore);

    [Fact]
    public void IfBoxIsEmpty_ThenBottleShouldBeAdded()
    {
        Given(
            Box_created_with_capacity(6)
        );
        When(
            Add_beer_bottle(carte_blanche)
        );
        Then(
            Beer_bottle_added(carte_blanche)
        );
    }
    
    [Fact]
    public void WhenAddedToBoxWithSpace_ShouldAddBottle()
    {
        Given(
            Box_created_with_capacity(2),
            Beer_bottle_added(gouden_carolus)
        );
        When(
            Add_beer_bottle(carte_blanche)
        );
        Then(
            Beer_bottle_added(carte_blanche)
        );
    }

    [Fact]
    public void WhenAddedToFullBox_ShouldFail()
    {
        Given(
            Box_created_with_capacity(1),
            Beer_bottle_added(gouden_carolus)
        );
        When(
            Add_beer_bottle(carte_blanche)
        );
        Then(
            Box_was_full()
        );
    }
    
    // Commands
    private AddBeerBottle Add_beer_bottle(BeerBottle bottle)
    {
        return new AddBeerBottle(Box_Id, bottle);
    }
}