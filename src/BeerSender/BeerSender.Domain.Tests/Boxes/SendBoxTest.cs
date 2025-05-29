using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;

namespace BeerSender.Domain.Tests.Boxes;

public class SendBoxTest(MartenFixture fixture) 
    : BoxTest<SendBox>(fixture)
{
    protected override ICommandHandler<SendBox> Handler
        => new SendBoxHandler();
    
    [Fact]
    public async Task WhenBoxIsClosedAndHasLabel_ShouldSucceed()
    {
        await Given<Box>(
            Box_created_with_capacity(24),
            Beer_bottle_added(gouden_carolus),
            Box_was_closed(),
            Shipping_label_added_with_carrier_and_code(Carrier.UPS, "ABC123")
        );

        await When(
            Send_box()
        );

        await Then(
            Box_was_sent()
        );
    }

    [Fact]
    public async Task WhenBoxHasNoLabel_ShouldFail()
    {
        await Given<Box>(
            Box_created_with_capacity(24),
            Beer_bottle_added(gouden_carolus),
            Box_was_closed()
        );

        await When(
            Send_box()
        );

        await Then(
            Box_has_no_label()
        );
    }

    [Fact]
    public async Task WhenIsNotClosed_ShouldFail()
    {
        await Given<Box>(
            Box_created_with_capacity(24),
            Beer_bottle_added(gouden_carolus),
            Shipping_label_added_with_carrier_and_code(Carrier.UPS, "ABC123")
        );

        await When(
            Send_box()
        );

        await Then(
            Box_was_not_closed()
        );
    }

    [Fact]
    public async Task WhenIsNotClosedAndHasNoLabel_ShouldFail()
    {
        await Given<Box>(
            Box_created_with_capacity(24),
            Beer_bottle_added(gouden_carolus)
        );

        await When(
            Send_box()
        );

        await Then(
            Box_was_not_closed(),
            Box_has_no_label()
        );
    }
    
    protected SendBox Send_box()
    {
        return new SendBox(Box_Id);
    }
}