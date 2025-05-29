using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;

namespace BeerSender.Domain.Tests.Boxes;

public class CloseBoxTest(MartenFixture fixture) 
    : BoxTest<CloseBox>(fixture)
{
    protected override ICommandHandler<CloseBox> Handler
        => new CloseBoxHandler();
    
    [Fact]
    public async Task WhenBoxIsNotEmpty_ShouldSucceed()
    {
        await Given<Box>(
            Box_created_with_capacity(24),
            Beer_bottle_added(gouden_carolus)
        );
        await When(
            Close_box()
        );
        await Then(
            Box_was_closed()
        );
    }

    [Fact]
    public async Task WhenBoxIsEmpty_ShouldFail()
    {
        await Given<Box>(
            Box_created_with_capacity(24)
        );
        await When(
            Close_box()
        );
        await Then(
            Box_was_empty()
        );
    }

    protected CloseBox Close_box()
    {
        return new CloseBox(Box_Id);
    }
}