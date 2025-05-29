using BeerSender.Domain.Boxes.Commands;

namespace BeerSender.Domain.Tests.Boxes;

public class CloseBoxTest : BoxTest<CloseBox>
{
    protected override CommandHandler<CloseBox> Handler
        => new CloseBoxHandler(eventStore);
    
    [Fact]
    public void WhenBoxIsNotEmpty_ShouldSucceed()
    {
        Given(
            Box_created_with_capacity(24),
            Beer_bottle_added(gouden_carolus)
        );
        When(
            Close_box()
        );
        Then(
            Box_was_closed()
        );
    }

    [Fact]
    public void WhenBoxIsEmpty_ShouldFail()
    {
        Given(
            Box_created_with_capacity(24)
        );
        When(
            Close_box()
        );
        Then(
            Box_was_empty()
        );
    }

    protected CloseBox Close_box()
    {
        return new CloseBox(Box_Id);
    }
}